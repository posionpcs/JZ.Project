using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace QD.Web.AppApi.Common
{
    public class AESUtil
    {
        public static byte[] _key1 = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF, 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
      
        #region 加密
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="toEncrypt"></param>
        /// <returns></returns>
        public static string Encrypt(string toEncrypt)
        {
            // 256-AES key    
            // byte[] keyArray = UTF8Encoding.UTF8.GetBytes("12345678901234567890123456789012");
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = _key1;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        } 
        #endregion

        #region 解密
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="toDecrypt"></param>
        /// <returns></returns>
        public static string Decrypt(string toDecrypt)
        {
            // 256-AES key    
            // byte[] keyArray = UTF8Encoding.UTF8.GetBytes("12345678901234567890123456789012");
            byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = _key1;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return UTF8Encoding.UTF8.GetString(resultArray);
        } 
        #endregion
    }
}