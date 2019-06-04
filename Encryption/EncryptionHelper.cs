using GDPR.Common.Core;
using GDPR.Common.Exceptions;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using PGPSnippet.Keys;
using PGPSnippet.PGPDecryption;
using PGPSnippet.PGPEncryption;
using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace GDPR.Common.Encryption
{
    public static class EncryptionExtension
    {
        public static byte[] ReadAllBytes(this BinaryReader reader)
        {
            const int bufferSize = 4096;
            using (var ms = new MemoryStream())
            {
                byte[] buffer = new byte[bufferSize];
                int count;
                while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
                    ms.Write(buffer, 0, count);
                return ms.ToArray();
            }

        }
    }

    public class EncryptionHelper
    {
        static public SymmetricKeyAlgorithmTag Algorithm = SymmetricKeyAlgorithmTag.Aes256;

        private static byte[][] GetHashKeys(string key)
        {
            byte[][] result = new byte[2][];
            Encoding enc = Encoding.UTF8;

            SHA256 sha2 = new SHA256CryptoServiceProvider();

            byte[] rawKey = enc.GetBytes(key);
            byte[] rawIV = enc.GetBytes(key);

            byte[] hashKey = sha2.ComputeHash(rawKey);
            byte[] hashIV = sha2.ComputeHash(rawIV);

            Array.Resize(ref hashIV, 16);

            result[0] = hashKey;
            result[1] = hashIV;

            return result;
        }

        public static byte[] EncryptStream(string key, byte[] data)
        {
            byte[] encrypted = null;

            byte[][] keys = GetHashKeys(key);

            try
            {
                encrypted = EncryptStream(data, keys[0], keys[1]);
            }
            catch (CryptographicException) { }
            catch (ArgumentNullException) { }

            return encrypted;
        }

        public static byte[] DecryptStream(string key, byte[] data)
        {
            byte[] unEnc = null;

            byte[][] keys = GetHashKeys(key);

            try
            {
                unEnc = DecryptStream(data, keys[0], keys[1]);
            }
            catch (CryptographicException) { }
            catch (ArgumentNullException) { }

            return unEnc;
        }
        
        public static byte[] EncryptStream(byte[] plain, byte[] Key, byte[] IV)
        {
            byte[] encrypted;
            using (MemoryStream mstream = new MemoryStream())
            {
                using (AesManaged aesProvider = new AesManaged())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(mstream,
                        aesProvider.CreateEncryptor(Key, IV), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(plain, 0, plain.Length);
                    }
                    encrypted = mstream.ToArray();
                }
            }
            return encrypted;
        }
        public static byte[] DecryptStream(byte[] encrypted, byte[] Key, byte[] IV)
        {
            byte[] data = null;
            using (MemoryStream mStream = new MemoryStream(encrypted)) //add encrypted
            {
                using (AesManaged aesProvider = new AesManaged())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(mStream,
                        aesProvider.CreateDecryptor(Key, IV), CryptoStreamMode.Read))
                    {
                        //cryptoStream.Read(encrypted, 0, encrypted.Length);
                        using (BinaryReader stream = new BinaryReader(cryptoStream))
                        {
                            data = stream.ReadAllBytes();
                        }
                    }
                }
            }

            return data;
        }

        

        public static void GenerateKeys(string username, string password, string keyStoreUrl, string applicationId)
        {
            IAsymmetricCipherKeyPairGenerator kpg = new RsaKeyPairGenerator();
            kpg.Init(new RsaKeyGenerationParameters(BigInteger.ValueOf(0x13), new SecureRandom(), 1024, 8));
            AsymmetricCipherKeyPair kp = kpg.GenerateKeyPair();
            FileStream out1 = new FileInfo(string.Format("{0}/{1}_PGPPrivateKey.asc", keyStoreUrl, applicationId)).OpenWrite();
            FileStream out2 = new FileInfo(string.Format("{0}/{1}_PGPPublicKey.asc", keyStoreUrl, applicationId)).OpenWrite();
            ExportKeyPair(out1, out2, true, kp.Public, kp.Private, username, password.ToCharArray(), true, null);
            out1.Close();
            out2.Close();
        }

        public static void GenerateKeys(string username, string password, string applicationId, Stream out1, Stream out2)
        {
            IAsymmetricCipherKeyPairGenerator kpg = new RsaKeyPairGenerator();
            kpg.Init(new RsaKeyGenerationParameters(BigInteger.ValueOf(0x13), new SecureRandom(), 1024, 8));
            
            AsymmetricCipherKeyPair kp = kpg.GenerateKeyPair();

            double secondsToExpire = (DateTime.Now.AddYears(1) - DateTime.Now).TotalSeconds;

            PgpSignatureSubpacketGenerator subpacketGenerator = new PgpSignatureSubpacketGenerator();
            PgpSignatureSubpacketVector subpacketVector = null;
            subpacketGenerator.SetKeyExpirationTime(false, (long)secondsToExpire);
            subpacketVector = subpacketGenerator.Generate();

            ExportKeyPair(out1, out2, true, kp.Public, kp.Private, username, password.ToCharArray(), true, subpacketVector);
        }

        private static void ExportKeyPair(Stream secretOut, Stream publicOut, bool close, AsymmetricKeyParameter publicKey, AsymmetricKeyParameter privateKey, string identity, char[] passPhrase, bool armor, PgpSignatureSubpacketVector vector)
        {
            if (armor)
            {
                secretOut = new ArmoredOutputStream(secretOut);
            }

            PgpSecretKey secretKey = new PgpSecretKey(PgpSignature.DefaultCertification, PublicKeyAlgorithmTag.RsaGeneral, publicKey, privateKey, DateTime.Now, identity, SymmetricKeyAlgorithmTag.Cast5, passPhrase, vector, null, new SecureRandom()
                //                ,"BC"
            );

            secretKey.Encode(secretOut);

            if (close)
                secretOut.Close();

            if (armor)
            {
                publicOut = new ArmoredOutputStream(publicOut);
            }

            PgpPublicKey key = secretKey.PublicKey;

            key.Encode(publicOut);

            if (close)
                publicOut.Close();
        }

        /*
public static byte[] encrypt(byte[] clearData, PgpPublicKey encKey, String fileName, bool withIntegrityCheck, bool armor)
{

   if (fileName == null) {
       fileName = PgpLiteralData.Console;
   }

   Stream encOut = new MemoryStream();

   Stream out1 = encOut;
   if (armor) {
       out1 = new ArmoredOutputStream(out1);
   }

   Stream bOut = new MemoryStream();

   PgpCompressedDataGenerator comData = new PgpCompressedDataGenerator(CompressionAlgorithmTag.Zip);
   Stream cos = comData.Open(bOut); // open it with the final
                                    // destination
   PgpLiteralDataGenerator lData = new PgpLiteralDataGenerator();

   // we want to generate compressed data. This might be a user option
   // later,
   // in which case we would pass in bOut.
   Stream pOut = lData.Open(cos, PgpLiteralData.Binary, fileName, clearData.Length, DateTime.Now);
   pOut.Write(clearData, 0, clearData.Length);

   lData.Close();
   comData.Close();

   PgpEncryptedDataGenerator cPk = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.Cast5, withIntegrityCheck, new SecureRandom(),"BC");

   cPk.AddMethod(encKey);

   byte[] bytes = bOut.ToByteArray();

   Stream cOut = cPk.Open(out1, bytes.Length);
   cOut.Write(bytes); // obtain the actual bytes from the compressed stream
   cOut.Close();
   out1.Close();

   return encOut.ToByteArray();
}
*/

        public static void EncryptPgpFile(string inputFile, string outputFile, PgpPublicKey pubKey, PgpPrivateKey privKey, bool armor, bool withIntegrityCheck)
        {
            //using (MemoryStream outputBytes = new MemoryStream())
            //{
            //PgpCompressedDataGenerator dataCompressor = new PgpCompressedDataGenerator(CompressionAlgorithmTag.Zip);
            //PgpUtilities.WriteFileToLiteralData(dataCompressor.Open(outputBytes), PgpLiteralData.Text, new FileInfo(inputFile));
            //dataCompressor.Close();
            byte[] dataBytes = File.ReadAllBytes(inputFile);

            PgpEncryptedDataGenerator dataGenerator = new PgpEncryptedDataGenerator(EncryptionHelper.Algorithm, withIntegrityCheck, new SecureRandom());
            dataGenerator.AddMethod(pubKey);

            using (Stream outputStream = new MemoryStream())
            {
                using (Stream clean = new MemoryStream())
                {
                    if (armor)
                    {
                        using (ArmoredOutputStream armoredStream = new ArmoredOutputStream(outputStream))
                        {
                            WriteStream(dataGenerator.Open(armoredStream, dataBytes.Length), ref dataBytes);
                        }
                    }
                    else
                    {
                        WriteStream(dataGenerator.Open(outputStream, dataBytes.Length), ref dataBytes);
                    }

                    PgpLiteralDataGenerator lData = new PgpLiteralDataGenerator();
                    Stream pOut = lData.Open(outputStream, PgpLiteralData.Binary, "", dataBytes.Length, DateTime.Now);
                    pOut.Write(dataBytes, 0, dataBytes.Length);

                    //this is for signing...

                    const bool Iscritical = false;
                    const bool IsNested = false;
                    PublicKeyAlgorithmTag tag = pubKey.Algorithm;
                    PgpSignatureGenerator pgpSigGen = new PgpSignatureGenerator(tag, HashAlgorithmTag.Sha1);
                    pgpSigGen.InitSign(PgpSignature.BinaryDocument, privKey);
                    foreach (string userID in pubKey.GetUserIds())
                    {
                        PgpSignatureSubpacketGenerator subPackGen = new PgpSignatureSubpacketGenerator();
                        subPackGen.SetSignerUserId(Iscritical, userID);
                        pgpSigGen.SetHashedSubpackets(subPackGen.Generate());
                        break;
                    }
                    pgpSigGen.GenerateOnePassVersion(IsNested).Encode(clean);

                    string fileName = @"c:\temp\encrypted_code.txt";

                    if (File.Exists(fileName))
                        File.Delete(fileName);

                    var fileStream = File.Create(fileName);
                    clean.Seek(0, SeekOrigin.Begin);
                    clean.CopyTo(fileStream);
                    fileStream.Close();
                    clean.Close();
                }
            }

            //}
        }

        public static void WriteStream(Stream inputStream, ref byte[] dataBytes)
        {
            using (Stream outputStream = inputStream)
            {
                outputStream.Write(dataBytes, 0, dataBytes.Length);
            }
        }

        private PgpSignatureGenerator sigGen(Stream compressedOut, PgpPublicKey pubKey, PgpPrivateKey privKey)
        {
            const bool Iscritical = false;
            const bool IsNested = false;
            PublicKeyAlgorithmTag tag = pubKey.Algorithm;
            PgpSignatureGenerator pgpSigGen = new PgpSignatureGenerator(tag, HashAlgorithmTag.Sha1);
            pgpSigGen.InitSign(PgpSignature.BinaryDocument, privKey);
            foreach (string userID in pubKey.GetUserIds())
            {
                PgpSignatureSubpacketGenerator subPackGen = new PgpSignatureSubpacketGenerator();
                subPackGen.SetSignerUserId(Iscritical, userID);
                pgpSigGen.SetHashedSubpackets(subPackGen.Generate());
                break;
            }
            pgpSigGen.GenerateOnePassVersion(IsNested).Encode(compressedOut);
            return pgpSigGen;
        }

        public static string EncryptPGP3(string key, string code)
        {
            //create the PGPKey object
            PgpPublicKey publicKey = PgpEncryptionKeys.ReadPublicKeyFromString(key, true);
            
            Stream inputData = Utility.GenerateStreamFromString(code);
            Stream encryptedMessageStream = new MemoryStream();

            EncryptStream(encryptedMessageStream, inputData, publicKey, true, true);
            
            string encryptedMessage = Utility.StreamToString(encryptedMessageStream);
            return encryptedMessage;
        }        

        public static string GetPrivateKey(string filePath, string id, string version)
        {
            string path = string.Format("{0}\\{1}_{2}.key", filePath, id, version);
            return File.ReadAllText(path);
        }

        public static string Encrypt(string publicKeyStr, string privateKeyStr, string data, string passPhrase)
        {
            PgpPublicKey publicKey = PgpEncryptionKeys.ReadPublicKeyFromString(publicKeyStr, false);
            PgpSecretKey secretKey = PgpEncryptionKeys.ReadSecretKeyFromString(privateKeyStr);

            return SignAndEncrypt(publicKey, secretKey, passPhrase, data);
        }

        public static string Encrypt(string data, string keyPath, string id, string passPhrase, bool isSystem, string version)
        {
            string publicKeyStr = "";
            string privateKeyStr = "";

            if (isSystem)
            {
                //encrypt key
                publicKeyStr = GDPRCore.Current.GetSystemKey(GDPRCore.Current.GetSystemId().ToString(), Configuration.SystemKeyVersion);

                //signing key is system...
                privateKeyStr = GetPrivateKey(keyPath, GDPRCore.Current.GetSystemId().ToString(), Configuration.SystemKeyVersion);
                passPhrase = EncryptionContext.Default.Password;
            }
            else
            {
                //encrypt key
                publicKeyStr = GDPRCore.Current.GetApplicationKey(id, version);

                //signing key is system..
                privateKeyStr = GetPrivateKey(keyPath, GDPRCore.Current.GetSystemId().ToString(), version);
                passPhrase = EncryptionContext.Default.Password;
            }

            PgpPublicKey publicKey = PgpEncryptionKeys.ReadPublicKeyFromString(publicKeyStr, false);
            PgpSecretKey secretKey = PgpEncryptionKeys.ReadSecretKeyFromString(privateKeyStr);

            return SignAndEncrypt(publicKey, secretKey, passPhrase, data);
        }

        public static string Encrypt(string data, EncryptionContext ctx)
        {
            return Encrypt(data, ctx.Path, ctx.Id, ctx.Password, !ctx.IsApplication, ctx.Version.ToString());
        }

        public static string Encrypt(string data)
        {
            return Encrypt(data, Utility.GetConfigurationValue("PrivateKeyPath"),
                    Utility.GetConfigurationValue("ApplicationId"),
                    Utility.GetConfigurationValue("PrivateKeyPassword"),
                    true,
                    Utility.GetConfigurationValue("SystemKeyVersion")
                );
        }

        private static void EncryptFile(Stream outputStream, FileInfo fileInfo, PgpPublicKey encKey, bool armor, bool withIntegrityCheck)
        {
            if (armor) outputStream = new ArmoredOutputStream(outputStream);

            using (var buffer = new MemoryStream())
            {
                var compressor = new PgpCompressedDataGenerator(CompressionAlgorithmTag.Zip);
                PgpUtilities.WriteFileToLiteralData(
                    compressor.Open(buffer),
                    PgpLiteralData.Binary,
                    fileInfo);
                compressor.Close();

                var encryptor = new PgpEncryptedDataGenerator(
                    SymmetricKeyAlgorithmTag.Cast5,
                    withIntegrityCheck,
                    new SecureRandom());
                encryptor.AddMethod(encKey);

                var bytes = buffer.ToArray();
                using (var cOut = encryptor.Open(outputStream, bytes.Length))
                {
                    cOut.Write(bytes, 0, bytes.Length);
                }
            }

            if (armor) outputStream.Close();
        }

        private static void EncryptStream(Stream outputStream, Stream data, PgpPublicKey pubKey, bool armor, bool withIntegrityCheck)
        {
            const int BUFFER_SIZE = 1 << 16; // should always be power of 2

            if (armor)
                outputStream = new ArmoredOutputStream(outputStream);

            // Init encrypted data generator
            PgpEncryptedDataGenerator encryptedDataGenerator = new PgpEncryptedDataGenerator(EncryptionHelper.Algorithm, withIntegrityCheck, new SecureRandom());
            encryptedDataGenerator.AddMethod(pubKey);
            Stream encryptedOut = encryptedDataGenerator.Open(outputStream, new byte[BUFFER_SIZE]);

            // Init compression
            PgpCompressedDataGenerator compressedDataGenerator = new PgpCompressedDataGenerator(CompressionAlgorithmTag.Zip);
            Stream compressedOut = compressedDataGenerator.Open(encryptedOut);

            // Create the Literal Data generator output stream
            PgpLiteralDataGenerator literalDataGenerator = new PgpLiteralDataGenerator();

            Stream literalOut = literalDataGenerator.Open(compressedOut,
                PgpLiteralData.Binary,
                "Code",
                DateTime.Now,
                new byte[BUFFER_SIZE]);
            // Open the input file

            byte[] buf = new byte[BUFFER_SIZE];
            int len;
            while ((len = data.Read(buf, 0, buf.Length)) > 0)
            {
                literalOut.Write(buf, 0, len);
            }

            literalOut.Close();
            literalDataGenerator.Close();
            compressedOut.Close();
            compressedDataGenerator.Close();
            encryptedOut.Close();
            encryptedDataGenerator.Close();

            if (armor)
                outputStream.Close();
        }


        static public void SignAndEncryptFile(string actualFileName,
                                        string OutputFileName,
                                        bool armor,
                                        bool withIntegrityCheck,
                                        PgpPublicKey pubKey,
                                        PgpPrivateKey privKey
                                        )
        {
            const int BUFFER_SIZE = 1 << 16; // should always be power of 2

            Stream outputStream = File.Open(OutputFileName, FileMode.Create);

            if (armor)
                outputStream = new ArmoredOutputStream(outputStream);

            // Init encrypted data generator
            PgpEncryptedDataGenerator encryptedDataGenerator = new PgpEncryptedDataGenerator(EncryptionHelper.Algorithm, withIntegrityCheck, new SecureRandom());
            encryptedDataGenerator.AddMethod(pubKey);
            Stream encryptedOut = encryptedDataGenerator.Open(outputStream, new byte[BUFFER_SIZE]);

            // Init compression
            PgpCompressedDataGenerator compressedDataGenerator = new PgpCompressedDataGenerator(CompressionAlgorithmTag.Zip);
            Stream compressedOut = compressedDataGenerator.Open(encryptedOut);

            // Init signature
            PgpSignatureGenerator signatureGenerator = new PgpSignatureGenerator(pubKey.Algorithm, HashAlgorithmTag.Sha1);
            signatureGenerator.InitSign(PgpSignature.BinaryDocument, privKey);
            foreach (string userId in pubKey.GetUserIds())
            {
                PgpSignatureSubpacketGenerator spGen = new PgpSignatureSubpacketGenerator();
                spGen.SetSignerUserId(false, userId);
                signatureGenerator.SetHashedSubpackets(spGen.Generate());
                // Just the first one!
                break;
            }
            signatureGenerator.GenerateOnePassVersion(false).Encode(compressedOut);

            // Create the Literal Data generator output stream
            PgpLiteralDataGenerator literalDataGenerator = new PgpLiteralDataGenerator();
            FileInfo actualFile = new FileInfo(actualFileName);

            Stream literalOut = literalDataGenerator.Open(compressedOut,
                PgpLiteralData.Binary,
                actualFile.Name,
                actualFile.LastWriteTime,
                new byte[BUFFER_SIZE]);
            // Open the input file

            FileStream inputStream = actualFile.OpenRead();
            byte[] buf = new byte[BUFFER_SIZE];
            int len;
            while ((len = inputStream.Read(buf, 0, buf.Length)) > 0)
            {
                literalOut.Write(buf, 0, len);
                signatureGenerator.Update(buf, 0, len);
            }

            literalOut.Close();
            literalDataGenerator.Close();
            signatureGenerator.Generate().Encode(compressedOut);
            compressedOut.Close();
            compressedDataGenerator.Close();
            encryptedOut.Close();
            encryptedDataGenerator.Close();
            inputStream.Close();

            if (armor)
                outputStream.Close();
        }

        public static string DecryptPGP(string publicKey, string check)
        {
            EncryptionContext ctx = EncryptionContext.Default;
            ctx.PublicKey = publicKey;
            return DecryptPGP(check, ctx);
        }

        public static string DecryptPGP(string data, EncryptionContext ctx)
        {
            try
            {
                Stream inputStream = Utility.GenerateStreamFromString(data);
                string passPhrase = ctx.Password;
                string privateKeyStr = EncryptionHelper.GetPrivateKey(ctx.Path, ctx.Id, ctx.Version.ToString());
                Stream keyIn = Utility.GenerateStreamFromString(privateKeyStr);
                //PgpSecretKey keyIn = PgpEncryptionKeys.ReadSecretKeyFromString(privateKeyStr);
                PgpPublicKey publicKey = PgpEncryptionKeys.ReadPublicKeyFromString(ctx.PublicKey, false);
                Stream outputStream = new MemoryStream();
                PGPDecrypt.Decrypt(inputStream, publicKey, keyIn, passPhrase, outputStream);
                return Utility.StreamToString(outputStream);
            }
            catch (Exception ex)
            {
                GDPRCore.Current.Log(ex, Enums.LogLevel.Error);
                return null;
            }
        }

        public static string SignAndEncrypt(PgpPublicKey publicKey, PgpSecretKey secretKey, string passPhrase, string data)
        {
            try
            {
                PgpEncryptionKeys encryptionKeys = new PgpEncryptionKeys(publicKey, secretKey, passPhrase);
                PgpEncrypt encrypter = new PgpEncrypt(encryptionKeys);

                Stream inputData = Utility.GenerateStreamFromString(data);
                Stream encryptedMessageStream = new MemoryStream();

                encrypter.SignAndEncryptStream(inputData, encryptedMessageStream, passPhrase.ToCharArray(), true, true, publicKey, secretKey);

                string encryptedMessage = Utility.StreamToString(encryptedMessageStream);
                return encryptedMessage;
            }
            catch (Exception e)
            {
                if (e.Message.Contains("Checksum mismatch"))
                    throw new GDPRException("Private Key Password is invalid");
            }

            return null;
        }

        public static string DecryptAndVerify(PgpPublicKey publicKey, Stream secretKey, string passPhrase, string data)
        {
            try
            {
                //PgpEncryptionKeys encryptionKeys = new PgpEncryptionKeys(publicKey, secretKey, passPhrase);
                
                Stream inputData = Utility.GenerateStreamFromString(data);
                Stream decryptedMessageStream = new MemoryStream();

                PGPDecrypt.Decrypt(inputData, publicKey, secretKey, passPhrase, decryptedMessageStream);
                
                string message = Utility.StreamToString(decryptedMessageStream);
                return message;
            }
            catch (Exception e)
            {
                if (e.Message.Contains("Checksum mismatch"))
                    throw new GDPRException("Private Key Password is invalid");
                else
                    throw;
            }

            return null;
        }

        public static string SignAndEncrypt(string publicKeyStr, string privateKeyStr, string passPhrase, string message)
        {
            PgpPublicKey publicKey = PgpEncryptionKeys.ReadPublicKeyFromString(publicKeyStr, false);
            PgpSecretKey secretKey = PgpEncryptionKeys.ReadSecretKeyFromString(privateKeyStr);

            return SignAndEncrypt(publicKey, secretKey, passPhrase, message);
        }

        public static string DecryptAndVerify(string publicKeyStr, string privateKeyStr, string passPhrase, string encMessage)
        {
            PgpPublicKey publicKey = PgpEncryptionKeys.ReadPublicKeyFromString(publicKeyStr,false);
            PgpSecretKey secretKey = PgpEncryptionKeys.ReadSecretKeyFromString(privateKeyStr);

            Stream keyIn = Utility.GenerateStreamFromString(privateKeyStr);

            return DecryptAndVerify(publicKey, keyIn, passPhrase, encMessage);
        }
    }
}
