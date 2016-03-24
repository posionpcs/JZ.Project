using System;
using System.Net;
using System.Web;

namespace FrameWork.Utils
{
    public static class IpHelper
    {
        private static string _cachedLocalIP;

        public static string GetHttpRequestIP()
        {
            if (HttpContext.Current == null)
            {
                return null;
            }
            string userHostAddress = string.Empty;
            HttpRequest request = HttpContext.Current.Request;
            string str2 = request.Headers["x-forwarded-for"];
            if (!string.IsNullOrEmpty(str2))
            {
                userHostAddress = str2;
            }
            else
            {
                try
                {
                    string str3 = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (!string.IsNullOrEmpty(str3))
                    {
                        userHostAddress = str3.Split(new char[] { ',' })[0].Trim();
                    }
                    else
                    {
                        userHostAddress = request.ServerVariables["REMOTE_ADDR"];
                    }
                    if (string.IsNullOrEmpty(userHostAddress))
                    {
                        userHostAddress = request.UserHostAddress;
                    }
                }
                catch (Exception)
                {
                }
            }
            if (!string.IsNullOrEmpty(userHostAddress))
            {
                return userHostAddress;
            }
            return "127.0.0.1";
        }

        public static string GetLocalIP()
        {
            if (string.IsNullOrEmpty(_cachedLocalIP))
            {
                foreach (IPAddress address in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
                {
                    if (address.AddressFamily.ToString() == "InterNetwork")
                    {
                        _cachedLocalIP = address.ToString();
                        break;
                    }
                }
            }
            return _cachedLocalIP;
        }

        public static bool InIP(string sourceIP, string targetIP)
        {
            if (!string.IsNullOrEmpty(sourceIP) && !string.IsNullOrEmpty(targetIP))
            {
                string[] strArray = sourceIP.Split(new char[] { '.' });
                string[] strArray2 = targetIP.Split(new char[] { '.' });
                int length = strArray.Length;
                for (int i = 0; i < length; i++)
                {
                    if (strArray2[i] == "*")
                    {
                        return true;
                    }
                    if (strArray[i] != strArray2[i])
                    {
                        return false;
                    }
                    if (i == 3)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool InIPList(string sourceIP, string[] targetIPList)
        {
            if ((targetIPList != null) && (targetIPList.Length > 0))
            {
                foreach (string str in targetIPList)
                {
                    if (InIP(sourceIP, str))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool InIPList(string sourceIP, string targetIPStr)
        {
            string[] targetIPList = targetIPStr.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            return InIPList(sourceIP, targetIPList);
        }
    }
}
