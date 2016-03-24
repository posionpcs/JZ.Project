using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace FrameWork
{
    public static class EncryptionExtensions
    {
        private static string EncFromDES(this string input, string key)
        {
            return string.Empty;
        }
        public static string EncFromDES(this string input, string key, string iv)
        {
            string[] strArray = input.Split("-".ToCharArray());
            byte[] inputBuffer = new byte[strArray.Length];
            for (int i = 0; i < strArray.Length; i++)
            {
                inputBuffer[i] = byte.Parse(strArray[i], NumberStyles.HexNumber);
            }
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider {
                Key = Encoding.ASCII.GetBytes(key),
                IV = Encoding.ASCII.GetBytes(iv)
            };
            byte[] bytes = provider.CreateDecryptor().TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
            return Encoding.UTF8.GetString(bytes);
        }

        public static string EncToDES(this string input, string key)
        {
            return string.Empty;
        }

        public static string EncToDES(this string input, string key, string iv)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider {
                Key = Encoding.ASCII.GetBytes(key),
                IV = Encoding.ASCII.GetBytes(iv)
            };
            return BitConverter.ToString(provider.CreateEncryptor().TransformFinalBlock(bytes, 0, bytes.Length));
        }

        public static string EncToMD5(this string input)
        {
            byte[] buffer = MD5.Create().ComputeHash(Encoding.Default.GetBytes(input));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < buffer.Length; i++)
            {
                builder.AppendFormat("{0:x2}", buffer[i]);
            }
            return builder.ToString();
        }

        public static string EncToMD5(this string input, Encoding encoding)
        {
            byte[] buffer = MD5.Create().ComputeHash(encoding.GetBytes(input));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < buffer.Length; i++)
            {
                builder.AppendFormat("{0:x2}", buffer[i]);
            }
            return builder.ToString();
        }
    }
}

