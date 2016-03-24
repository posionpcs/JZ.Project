using System;
using System.Text;

namespace FrameWork.Security
{
    public class Base64Utils
    {
        /// <summary> 
        /// 将字符串使用base64算法加密 
        /// </summary> 
        /// <param name="encodingName">编码类型（编码名称） 
        /// * 代码页 名称 
        /// * 1200 "UTF-16LE"、"utf-16"、"ucs-2"、"unicode"或"ISO-10646-UCS-2" 
        /// * 1201 "UTF-16BE"或"unicodeFFFE" 
        /// * 1252 "windows-1252"
        /// * 65000 "utf-7"、"csUnicode11UTF7"、"unicode-1-1-utf-7"、"unicode-2-0-utf-7"、"x-unicode-1-1-utf-7"或"x-unicode-2-0-utf-7" 
        /// * 65001 "utf-8"、"unicode-1-1-utf-8"、"unicode-2-0-utf-8"、"x-unicode-1-1-utf-8"或"x-unicode-2-0-utf-8" 
        /// * 20127 "us-ascii"、"us"、"ascii"、"ANSI_X3.4-1968"、"ANSI_X3.4-1986"、"cp367"、"csASCII"、"IBM367"、"iso-ir-6"、"ISO646-US"或"ISO_646.irv:1991" 
        /// * 54936 "GB18030"
        /// </param>
        /// <param name="source">待加密的字符串</param>
        /// <returns>加密后的字符串</returns> 
        public static string EncodeBase64String(string source, string encodingName = "UTF-8")
        {
            byte[] bytes = Encoding.GetEncoding(encodingName).GetBytes(source); //将一组字符编码为一个字节序列. 
            return Convert.ToBase64String(bytes); //将8位无符号整数数组的子集转换为其等效的,以64为基的数字编码的字符串形式. 
        }

        /// <summary> 
        /// 将字符串使用base64算法解密 
        /// </summary> 
        /// <param name="encodingName">编码类型</param> 
        /// <param name="base64String">已用base64算法加密的字符串</param> 
        /// <returns>解密后的字符串</returns> 
        public static string DecodeBase64String(string base64String, string encodingName = "UTF-8")
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(base64String); //将2进制编码转换为8位无符号整数数组. 
                return Encoding.GetEncoding(encodingName).GetString(bytes); //将指定字节数组中的一个字节序列解码为一个字符串。
            }
            catch(Exception ex)
            {
                return string.Empty;
            }
        }
    }

}