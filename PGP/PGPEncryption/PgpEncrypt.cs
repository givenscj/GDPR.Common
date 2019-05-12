using GDPR.Common.Encryption;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Security;
using PGPSnippet.Keys;
using System;
using System.IO;

namespace PGPSnippet.PGPEncryption
{
    public class PgpEncrypt
    {

        private PgpEncryptionKeys m_encryptionKeys;

        private const int BufferSize = 0x10000; // should always be power of 2 

        /// <summary>

        /// Instantiate a new PgpEncrypt class with initialized PgpEncryptionKeys.

        /// </summary>

        /// <param name="encryptionKeys"></param>

        /// <exception cref="ArgumentNullException">encryptionKeys is null</exception>

        public PgpEncrypt(PgpEncryptionKeys encryptionKeys)
        {

            if (encryptionKeys == null)

                throw new ArgumentNullException("encryptionKeys", "encryptionKeys is null.");

            m_encryptionKeys = encryptionKeys;

        }

        /// <summary>

        /// Encrypt and sign the file pointed to by unencryptedFileInfo and

        /// write the encrypted content to outputStream.

        /// </summary>

        /// <param name="outputStream">The stream that will contain the

        /// encrypted data when this method returns.</param>

        /// <param name="fileName">FileInfo of the file to encrypt</param>

        public void EncryptAndSign(Stream outputStream, FileInfo unencryptedFileInfo)
        {

            if (outputStream == null)

                throw new ArgumentNullException("outputStream", "outputStream is null.");

            if (unencryptedFileInfo == null)

                throw new ArgumentNullException("unencryptedFileInfo", "unencryptedFileInfo is null.");

            if (!File.Exists(unencryptedFileInfo.FullName))

                throw new ArgumentException("File to encrypt not found.");

            using (Stream encryptedOut = ChainEncryptedOut(outputStream))

            using (Stream compressedOut = ChainCompressedOut(encryptedOut))
            {

                PgpSignatureGenerator signatureGenerator = InitSignatureGenerator(compressedOut);

                using (Stream literalOut = ChainLiteralOut(compressedOut, unencryptedFileInfo))

                using (FileStream inputFile = unencryptedFileInfo.OpenRead())
                {

                    WriteOutputAndSign(compressedOut, literalOut, inputFile, signatureGenerator);

                }

            }

        }

        public void SignAndEncryptStream(Stream messageData, Stream outputStream,
            char[] password, bool armor, bool withIntegrityCheck, PgpPublicKey encKey, PgpSecretKey pgpSecKey)
        {
            const int BUFFER_SIZE = 1 << 16; // should always be power of 2

            if (armor)
                outputStream = new ArmoredOutputStream(outputStream);

            // Init encrypted data generator
            PgpEncryptedDataGenerator encryptedDataGenerator =
                new PgpEncryptedDataGenerator(EncryptionHelper.Algorithm, withIntegrityCheck, new SecureRandom());
            encryptedDataGenerator.AddMethod(encKey);
            Stream encryptedOut = encryptedDataGenerator.Open(outputStream, new byte[BUFFER_SIZE]);
 
            // Init compression
            PgpCompressedDataGenerator compressedDataGenerator = new PgpCompressedDataGenerator(CompressionAlgorithmTag.Zip);
            Stream compressedOut = compressedDataGenerator.Open(encryptedOut);

            // Init signature
            PgpPrivateKey pgpPrivKey = pgpSecKey.ExtractPrivateKey(password);

            PgpSignatureGenerator signatureGenerator = new PgpSignatureGenerator(pgpSecKey.PublicKey.Algorithm, HashAlgorithmTag.Sha1);
            signatureGenerator.InitSign(PgpSignature.BinaryDocument, pgpPrivKey);
            foreach (string userId in pgpSecKey.PublicKey.GetUserIds())
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

            Stream literalOut = literalDataGenerator.Open(compressedOut, PgpLiteralData.Binary, "Data", DateTime.Now, new byte[BUFFER_SIZE]);
            
            byte[] buf = new byte[BUFFER_SIZE];
            int len;
            while ((len = messageData.Read(buf, 0, buf.Length)) > 0)
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

            if (armor)
                outputStream.Close();
        }

        public void EncryptAndSign(Stream outputStream, Stream unencryptedData, bool armor)
        {

            if (outputStream == null)

                throw new ArgumentNullException("outputStream", "outputStream is null.");

            if (unencryptedData == null)

                throw new ArgumentNullException("unencryptedData", "unencryptedData is null.");

            if (armor)
                outputStream = new ArmoredOutputStream(outputStream);

            using (Stream encryptedOut = ChainEncryptedOut(outputStream))

            using (Stream compressedOut = ChainCompressedOut(encryptedOut))
            {

                PgpSignatureGenerator signatureGenerator = InitSignatureGenerator(compressedOut);

                using (Stream literalOut = ChainLiteralOut(compressedOut, unencryptedData))

                {

                    WriteOutputAndSign(compressedOut, literalOut, unencryptedData, signatureGenerator);

                }

            }

        }

        public void EncryptA(Stream outputStream, Stream unencryptedData, bool armor)
        {

            if (outputStream == null)

                throw new ArgumentNullException("outputStream", "outputStream is null.");

            if (unencryptedData == null)

                throw new ArgumentNullException("unencryptedData", "unencryptedData is null.");

            if (armor)
                outputStream = new ArmoredOutputStream(outputStream);

            using (Stream encryptedOut = ChainEncryptedOut(outputStream))

            using (Stream compressedOut = ChainCompressedOut(encryptedOut))
            {

                using (Stream literalOut = ChainLiteralOut(compressedOut, unencryptedData))

                {

                    WriteOutput(compressedOut, literalOut, unencryptedData);

                }

            }

        }

        private static void WriteOutput(Stream compressedOut,
            Stream literalOut,
            Stream inputFile)
        {
            inputFile.Position = 0;

            int length = 0;

            byte[] buf = new byte[BufferSize];

            while ((length = inputFile.Read(buf, 0, buf.Length)) > 0)
            {
                literalOut.Write(buf, 0, length);
            }
        }

        private static void WriteOutputAndSign(Stream compressedOut,

            Stream literalOut,

            Stream inputFile,

            PgpSignatureGenerator signatureGenerator)
        {
            inputFile.Position = 0;

            int length = 0;

            byte[] buf = new byte[BufferSize];

            while ((length = inputFile.Read(buf, 0, buf.Length)) > 0)
            {

                literalOut.Write(buf, 0, length);

                signatureGenerator.Update(buf, 0, length);

            }

            signatureGenerator.Generate().Encode(compressedOut);

        }

        private Stream ChainEncryptedOut(Stream outputStream)
        {

            PgpEncryptedDataGenerator encryptedDataGenerator;

            encryptedDataGenerator =

                new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.TripleDes,
                                              new SecureRandom());

            encryptedDataGenerator.AddMethod(m_encryptionKeys.PublicKey);

            return encryptedDataGenerator.Open(outputStream, new byte[BufferSize]);

        }

        private static Stream ChainCompressedOut(Stream encryptedOut)
        {

            PgpCompressedDataGenerator compressedDataGenerator =

                new PgpCompressedDataGenerator(CompressionAlgorithmTag.Zip);

            return compressedDataGenerator.Open(encryptedOut);

        }

        private static Stream ChainLiteralOut(Stream compressedOut, FileInfo file)
        {
            PgpLiteralDataGenerator pgpLiteralDataGenerator = new PgpLiteralDataGenerator();
            return pgpLiteralDataGenerator.Open(compressedOut, PgpLiteralData.Binary, file);

        }

        private static Stream ChainLiteralOut(Stream compressedOut, Stream input)
        {
            PgpLiteralDataGenerator pgpLiteralDataGenerator = new PgpLiteralDataGenerator();

            byte[] data;

            using (var memoryStream = new MemoryStream())
            {
                input.CopyTo(memoryStream);
                data = memoryStream.ToArray();
            }

            return pgpLiteralDataGenerator.Open(compressedOut, PgpLiteralData.Binary, "Data", DateTime.Now, data);
        }

        private PgpSignatureGenerator InitSignatureGenerator(Stream compressedOut)
        {

            const bool IsCritical = false;

            const bool IsNested = false;

            PublicKeyAlgorithmTag tag = m_encryptionKeys.SecretKey.PublicKey.Algorithm;

            PgpSignatureGenerator pgpSignatureGenerator =

                new PgpSignatureGenerator(tag, HashAlgorithmTag.Sha1);

            pgpSignatureGenerator.InitSign(PgpSignature.BinaryDocument, m_encryptionKeys.PrivateKey);

            foreach (string userId in m_encryptionKeys.SecretKey.PublicKey.GetUserIds())
            {

                PgpSignatureSubpacketGenerator subPacketGenerator =
                   new PgpSignatureSubpacketGenerator();

                subPacketGenerator.SetSignerUserId(IsCritical, userId);

                pgpSignatureGenerator.SetHashedSubpackets(subPacketGenerator.Generate());

                // Just the first one!

                break;

            }

            pgpSignatureGenerator.GenerateOnePassVersion(IsNested).Encode(compressedOut);

            return pgpSignatureGenerator;

        }

    }

}
