using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DataStruct
{
    public class EncriptionData
    {
        public enum Supported_HA
        {
            SHA256, SHA384, SHA512
        }
        
        public static string ComputeHash(string plainText, Supported_HA hash, byte[] salt)
        {
            if (plainText == null) plainText = "";
            int minSaltLength = 4, maxSaltLength = 16;
            byte[] SaltBytes = null;
            if (salt != null)
            {
                SaltBytes = salt;
            }
            else
            {
                Random r = new Random();
                int SaltLength = r.Next(minSaltLength, maxSaltLength);
                SaltBytes = new byte[SaltLength];
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                rng.GetNonZeroBytes(SaltBytes);
                rng.Dispose();
            }

            byte[] plainData = ASCIIEncoding.UTF8.GetBytes(plainText);
            byte[] plaintDataWithSalt = new byte[plainData.Length + SaltBytes.Length];

            for (int x = 0; x < plainData.Length; x++)
                plaintDataWithSalt[x] = plainData[x];
            for (int n = 0; n < SaltBytes.Length; n++)
                plaintDataWithSalt[plainData.Length + n] = SaltBytes[n];

            byte[] hashValue = null;

            switch (hash)
            {
                case Supported_HA.SHA256:
                    SHA256Managed sha = new SHA256Managed();
                    hashValue = sha.ComputeHash(plaintDataWithSalt);
                    sha.Dispose();
                    break;
                case Supported_HA.SHA384:
                    SHA384Managed sha1 = new SHA384Managed();
                    hashValue = sha1.ComputeHash(plaintDataWithSalt);
                    sha1.Dispose();
                    break;
                case Supported_HA.SHA512:
                    SHA512Managed sha2 = new SHA512Managed();
                    hashValue = sha2.ComputeHash(plaintDataWithSalt);
                    sha2.Dispose();
                    break;
            }

            byte[] result = new byte[hashValue.Length + SaltBytes.Length];
            for (int x = 0; x < hashValue.Length; x++)
                result[x] = hashValue[x];
            for (int n = 0; n < SaltBytes.Length; n++)
                result[hashValue.Length + n] = SaltBytes[n];

            return Convert.ToBase64String(result);
        }


        /// <summary>
        /// Проверяет, является ли текст такой же, как и оригинального текста, который был шифрованным  hashvalue.
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="hashValue"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static bool Confirm(string plainText, string hashValue, Supported_HA hash)
        {
            if (plainText == null) plainText = "";
            byte[] hashBytes = Convert.FromBase64String(hashValue);
            int hashSize = 0;

            switch (hash)
            {
                case Supported_HA.SHA256:
                    hashSize = 32;
                    break;
                case Supported_HA.SHA384:
                    hashSize = 48;
                    break;
                case Supported_HA.SHA512:
                    hashSize = 64;
                    break;
            }
            byte[] saltBytes = new byte[hashBytes.Length - hashSize];

            for (int x = 0; x < saltBytes.Length; x++)
                saltBytes[x] = hashBytes[hashSize + x];

            string newHash = ComputeHash(plainText, hash, saltBytes);

            return (hashValue == newHash);
        }

    }
}
