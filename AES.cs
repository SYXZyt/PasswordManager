using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace PasswordManager.AES
{
    class Encrypt
    {
        public static byte[] EncryptString(byte[] key, byte[] iv, string textToEncrypt)
        {
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using MemoryStream msEncrypt = new();
                using CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write);
                using (StreamWriter swEncrypt = new(csEncrypt))
                {
                    //Write all data to the stream.
                    swEncrypt.Write(textToEncrypt);
                }
                encrypted = msEncrypt.ToArray();
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }
    }

    class Decrypt
    {
        public static string DecryptString(byte[] key, byte[] iv, byte[] encryptedText)
        {
            Aes aes = Aes.Create();

            aes.Key = key;
            aes.IV = iv;

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            MemoryStream memoryStream = new(encryptedText);
            CryptoStream cryptoStream = new(memoryStream, decryptor, CryptoStreamMode.Read);
            StreamReader streamReader = new(cryptoStream);
            string decrypted = streamReader.ReadToEnd();

            return decrypted;
        }
    }
}