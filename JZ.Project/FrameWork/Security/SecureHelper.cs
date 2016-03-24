using System.Security.Cryptography;
using System.Text;

namespace FrameWork.Security
{
   public class SecureHelper
    {
        /// <summary>
        /// 获取MD5字符串
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Md5Hash(string input)
        {
            MD5 md5Hash = MD5.Create();
          
            byte[] data = md5Hash.ComputeHash(Encoding.Default.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("X2"));
            }
            return sBuilder.ToString();
        }

    }
}
