﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;

using PGPSnippet.Keys;
using PGPSnippet.PGPEncryption;

namespace GDPR.Common.Encryption
{
    public class EncryptionHelper
    {
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

            PgpEncryptedDataGenerator dataGenerator = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.Aes256, withIntegrityCheck, new SecureRandom());
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
            string filePath = @"c:\temp\code.txt";

            //create temp file...
            if (File.Exists(filePath))
                File.Delete(filePath);

            File.AppendAllText(filePath, code);

            //create the PGPKey object
            PgpPublicKey publicKey = PgpEncryptionKeys.ReadPublicKeyFromString(File.ReadAllText(@"c:\KeyBase\publickey.txt"));
            PgpSecretKey secretKey = PgpEncryptionKeys.ReadSecretKeyFromString(File.ReadAllText(@"c:\KeyBase\privatekey.txt"));
            PgpPrivateKey privKey = PgpEncryptionKeys.ReadPrivateKey(secretKey, "Seattle123");

            FileInfo fi = new FileInfo(@"c:\temp\code.txt");
            FileStream outputStream = new FileStream(@"c:\temp\encrypted_code.txt", FileMode.Create);
            EncryptFile(outputStream, fi, publicKey, true, true);
            outputStream.Close();

            return File.ReadAllText(@"c:\temp\encrypted_code.txt");
        }

        public static string GetSystemKey()
        {
            //send the message to the processor endpoint...
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(ConfigurationManager.AppSettings["SystemUrl"]);
            var result = client.GetAsync("/Home/GetSystemPublicKey");
            string resultContent = result.Result.Content.ReadAsStringAsync().Result;
            return resultContent.Trim();
        }

        public static string GetPrivateKey(string filePath, string id)
        {
            filePath = string.Format("{0}\\{1}.key", filePath, id);
            return File.ReadAllText(filePath);
        }

        public static string Encrypt(string data)
        {
            string publicKeyStr = GetSystemKey();
            string privateKeyStr = GetPrivateKey(ConfigurationManager.AppSettings["PrivateKeyPath"], ConfigurationManager.AppSettings["ApplicationId"]);

            PgpSecretKey secretKey = PgpEncryptionKeys.ReadSecretKeyFromString(privateKeyStr);
            PgpPublicKey publicKey = PgpEncryptionKeys.ReadPublicKeyFromString(publicKeyStr);

            string passPhrase = ConfigurationManager.AppSettings["PrivateKeyPassword"];

            PgpEncryptionKeys encryptionKeys = new PgpEncryptionKeys(publicKey, secretKey, passPhrase);
            PgpEncrypt encrypter = new PgpEncrypt(encryptionKeys);

            Stream inputData = Utility.GenerateStreamFromString(data);
            Stream encryptedMessageStream = new MemoryStream();

            encrypter.SignAndEncryptStream(inputData, encryptedMessageStream, passPhrase.ToCharArray(), true, true, publicKey, secretKey);
            string encryptedMessage = Utility.StreamToString(encryptedMessageStream);
            return encryptedMessage;
        }

        internal static string EncryptPGP(string key, string code)
        {
            string filePath = @"c:\temp\code.txt";

            //create temp file...
            if (File.Exists(filePath))
                File.Delete(filePath);

            File.AppendAllText(filePath, code);

            //create the PGPKey object
            PgpPublicKey publicKey = PgpEncryptionKeys.ReadPublicKeyFromString(File.ReadAllText(@"c:\KeyBase\publickey.txt"));
            PgpSecretKey secretKey = PgpEncryptionKeys.ReadSecretKeyFromString(File.ReadAllText(@"c:\KeyBase\privatekey.txt"));
            PgpPrivateKey privKey = PgpEncryptionKeys.ReadPrivateKey(secretKey, "Seattle123");

            EncryptPgpFile(@"c:\temp\code.txt", @"c:\temp\encrypted_code.txt", publicKey, privKey, true, true);

            return File.ReadAllText(@"c:\temp\encrypted_code.txt");
        }

        internal static string EncryptPGP2(string key, string code)
        {
            string filePath = @"c:\temp\code.txt";

            //create temp file...
            if (File.Exists(filePath))
                File.Delete(filePath);

            File.AppendAllText(filePath, code);

            //create the PGPKey object
            PgpPublicKey publicKey = PgpEncryptionKeys.ReadPublicKeyFromString(File.ReadAllText(@"c:\KeyBase\publickey.txt"));
            PgpSecretKey secretKey = PgpEncryptionKeys.ReadSecretKeyFromString(File.ReadAllText(@"c:\KeyBase\privatekey.txt"));
            PgpPrivateKey privKey = PgpEncryptionKeys.ReadPrivateKey(secretKey, "Seattle123");

            SignAndEncryptFile(@"c:\temp\code.txt", @"c:\temp\encrypted_code.txt", true, true, publicKey, privKey);

            return File.ReadAllText(@"c:\temp\encrypted_code.txt");
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
            PgpEncryptedDataGenerator encryptedDataGenerator = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.Cast5, withIntegrityCheck, new SecureRandom());
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

        internal static string DecryptPGP(string publicKey, string check)
        {
            throw new NotImplementedException();
        }
    }
}
