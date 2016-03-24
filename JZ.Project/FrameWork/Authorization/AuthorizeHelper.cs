namespace FrameWork.Authorization
{
    
    using System;
    using System.Web;
    using System.Web.Security;

    public static class AuthorizeHelper
    {
        private const string iv = "9&a;^Q1)";
        private const string key = "u*;aEo~3";

        public static SystemUser GetCurrentUser()
        {
            if (HttpContext.Current != null)
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                if (cookie != null)
                {
                    return FormsAuthentication.Decrypt(cookie.Value).UserData.FromJson<SystemUser>();
                }
            }
            return null;
        }

        public static string GetEncPassword(string password)
        {
            return password.EncToMD5();
        }

        public static void SignIn(SystemUser user)
        {
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, user.UserId, DateTime.Now, DateTime.Now.AddYears(10), false, user.ToJson(false), FormsAuthentication.FormsCookiePath);
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket)) {
                HttpOnly = true,
                Path = FormsAuthentication.FormsCookiePath,
                Secure = FormsAuthentication.RequireSSL
            };
            if (FormsAuthentication.CookieDomain != null)
            {
                cookie.Domain = FormsAuthentication.CookieDomain;
            }
            HttpContext.Current.Response.Cookies.Add(cookie);
            HttpContext.Current.Response.Cookies[FormsAuthentication.FormsCookieName].Expires = DateTime.Now.AddYears(10);
        }

        public static void SignOut()
        {
            FormsAuthentication.SignOut();
        }
    }
}

