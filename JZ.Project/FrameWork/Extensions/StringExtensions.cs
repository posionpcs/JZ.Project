using System;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Web;

namespace FrameWork
{
    public static class StringExtensions
    {
        private static Regex _dateregex = new Regex(@"(\d{4})-(\d{1,2})-(\d{1,2})");
        private static Regex _emailregex = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", RegexOptions.IgnoreCase);
        private static Regex _ipregex = new Regex(@"^(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])$");
        private static Regex _mobileregex = new Regex("^(13|14|15|16|18|19|17)[0-9]{9}$");
        private static Regex _numericregex = new Regex(@"^[-]?[0-9]+(\.[0-9]+)?$");
        private static Regex _phoneregex = new Regex(@"^(\d{3,4}-?)?\d{7,8}$");
        private static Regex _zipcoderegex = new Regex(@"^\d{6}$");

        private static bool CheckIDCard15(string Id)
        {
            long result = 0L;
            if (!long.TryParse(Id, out result) || (result < Math.Pow(10.0, 14.0)))
            {
                return false;
            }
            string str = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (str.IndexOf(Id.Remove(2)) == -1)
            {
                return false;
            }
            string s = Id.Substring(6, 6).Insert(4, "-").Insert(2, "-");
            DateTime time = new DateTime();
            if (!DateTime.TryParse(s, out time))
            {
                return false;
            }
            return true;
        }

        private static bool CheckIDCard18(string Id)
        {
            long result = 0L;
            if ((!long.TryParse(Id.Remove(0x11), out result) || (result < Math.Pow(10.0, 16.0))) || !long.TryParse(Id.Replace('x', '0').Replace('X', '0'), out result))
            {
                return false;
            }
            string str = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (str.IndexOf(Id.Remove(2)) == -1)
            {
                return false;
            }
            string s = Id.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            DateTime time = new DateTime();
            if (!DateTime.TryParse(s, out time))
            {
                return false;
            }
            string[] strArray = "1,0,x,9,8,7,6,5,4,3,2".Split(new char[] { ',' });
            string[] strArray2 = "7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2".Split(new char[] { ',' });
            char[] chArray = Id.Remove(0x11).ToCharArray();
            int a = 0;
            for (int i = 0; i < 0x11; i++)
            {
                a += int.Parse(strArray2[i]) * int.Parse(chArray[i].ToString());
            }
            int num4 = -1;
            Math.DivRem(a, 11, out num4);
            if (strArray[num4] != Id.Substring(0x11, 1).ToLower())
            {
                return false;
            }
            return true;
        }

        public static int Count(this string str, string compare)
        {
            int index = str.IndexOf(compare);
            if (index != -1)
            {
                return (1 + str.Substring(index + compare.Length).Count(compare));
            }
            return 0;
        }

        public static string Fmt(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        public static string FromNow(this string formatter)
        {
            return DateTime.Now.ToString(formatter);
        }

        public static string FromTime(this string formatter, DateTime time)
        {
            return time.ToString(formatter);
        }

        public static bool IsChinese(this string str)
        {
            return Regex.IsMatch(@"^[\u4e00-\u9fa5]+$", str);
        }

        public static bool IsDate(this string s)
        {
            return _dateregex.IsMatch(s);
        }

        public static bool IsEmail(this string s)
        {
            return (string.IsNullOrEmpty(s) || _emailregex.IsMatch(s));
        }

        public static bool IsIdCard(this string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return true;
            }
            if (id.Length == 0x12)
            {
                return CheckIDCard18(id);
            }
            return ((id.Length == 15) && CheckIDCard15(id));
        }

        public static bool IsImgFileName(this string fileName)
        {
            if (fileName.IndexOf(".") == -1)
            {
                return false;
            }
            string str = fileName.Trim().ToLower();
            string str2 = str.Substring(str.LastIndexOf("."));
            if ((!(str2 == ".png") && !(str2 == ".bmp")) && (!(str2 == ".jpg") && !(str2 == ".jpeg")))
            {
                return (str2 == ".gif");
            }
            return true;
        }

        public static bool IsIP(this string s)
        {
            return _ipregex.IsMatch(s);
        }

        public static bool IsIPV4(this string str)
        {
            string[] strArray = str.Split(new char[] { '.' });
            for (int i = 0; i < strArray.Length; i++)
            {
                if (!Regex.IsMatch(@"^\d+$", strArray[i]))
                {
                    return false;
                }
                if (Convert.ToUInt16(strArray[i]) > 0xff)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsIPV6(this string str)
        {
            string input = "";
            string str3 = str;
            if (str3.Split(new char[] { ':' }).Length > 8)
            {
                return false;
            }
            int num = str.Count("::");
            if (num > 1)
            {
                return false;
            }
            if (num == 0)
            {
                input = @"^([\da-f]{1,4}:){7}[\da-f]{1,4}$";
                return Regex.IsMatch(input, str);
            }
            input = @"^([\da-f]{1,4}:){0,5}::([\da-f]{1,4}:){0,5}[\da-f]{1,4}$";
            return Regex.IsMatch(input, str);
        }

        public static bool IsMobile(this string s)
        {
            return (string.IsNullOrEmpty(s) || _mobileregex.IsMatch(s));
        }

        public static bool IsNull<T>(this T obj) where T: class
        {
            return (obj == null);
        }

        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static bool IsNumeric(this string numericStr)
        {
            return _numericregex.IsMatch(numericStr);
        }

        public static bool IsPhone(this string s)
        {
            return (string.IsNullOrEmpty(s) || _phoneregex.IsMatch(s));
        }

        public static bool IsZipCode(this string s)
        {
            return (string.IsNullOrEmpty(s) || _zipcoderegex.IsMatch(s));
        }

        public static string ValueOfAppSetting(this string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static string ValueOfConnectionString(this string str)
        {
            return ConfigurationManager.ConnectionStrings[str].ConnectionString;
        }

        public static string ValueOfForm(this string key, string nullValue = null)
        {
            string str = HttpContext.Current.Request.Form[key];
            if (str == null)
            {
                return nullValue;
            }
            return str.Trim();
        }
    }
}

