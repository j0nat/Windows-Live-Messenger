using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Cryptography;
using System.IO;

namespace WLMServer.Database
{
    class PasswordEncrypter
    {
        private static String Encrypt(String s, byte[] key, byte[] IV)
        {
            String result;
            RijndaelManaged rijn = new RijndaelManaged();
            rijn.Mode = CipherMode.ECB;
            rijn.Padding = PaddingMode.Zeros;
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (ICryptoTransform encryptor = rijn.CreateEncryptor(key, IV))
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(s);
                        }
                    }
                }
                result = Convert.ToBase64String(msEncrypt.ToArray());
            }

            rijn.Clear();
            return result;
        }

        private static String Decrypt(String s, byte[] key, byte[] IV)
        {
            String result;
            RijndaelManaged rijn = new RijndaelManaged();
            rijn.Mode = CipherMode.ECB;
            rijn.Padding = PaddingMode.Zeros;
            using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(s)))
            {
                using (ICryptoTransform decryptor = rijn.CreateDecryptor(key, IV))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader swDecrypt = new StreamReader(csDecrypt))
                        {
                            result = swDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            rijn.Clear();
            return result;
        }

        public static string GetEncryptedPassword(string value)
        {
            Encoding byteEncoder = Encoding.UTF8;

            byte[] rijnKey = byteEncoder.GetBytes(Config.Properties.DATABASE_PASSWORD_ENCRYPTION_KEY);
            byte[] rijnIV = byteEncoder.GetBytes(Config.Properties.DATABASE_PASSWORD_ENCRYPTION_IV);

            return Encrypt(value, rijnKey, rijnIV);
        }
    }
}
