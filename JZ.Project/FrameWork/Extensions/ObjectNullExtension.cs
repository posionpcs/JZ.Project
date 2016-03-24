using System;

namespace QD.Web.AppApi.Common
{
    public static class ObjectNullExtension
    {
        /// <summary>
        /// 非空字符串
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string NotNullValue(this string val)
        {
            return val == null?"":val;
        }

        /// <summary>
        /// 非空整型
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static int NotNullValue(this int? val)
        {
            return val.HasValue ? val.Value : 0;
        }

        /// <summary>
        /// 非空实数
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static decimal NotNullValue(this decimal? val)
        {
            return val.HasValue ? val.Value : 0;
        }

        /// <summary>
        /// 时间转换为字符串
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string NotNullValue(this DateTime? val)
        {
            return val.HasValue?val.Value.ToString("yyyy-MM-dd HH:mm:ss"):"";
        }
    }
}