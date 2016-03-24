using System;
using System.Text.RegularExpressions;
using FrameWork;


namespace QD.Web.AppApi.Common
{
    public class Common
    {
        #region 获取图片地址
        /// <summary>
        /// 获取图片地址
        /// </summary>
        /// <param name="picId">图片Id</param>
        /// <param name="picType">0:原图 1：缩略图 2：中图 3：小图</param>
        /// <returns></returns>
        public static string PicPath(string picId, int picType, string domain = "")
        {
            var picUrl = string.Empty;
            string[] _PicTypeMap_ = { "jpg", "jpeg", "bmp", "png", "gif" };

            if (!string.IsNullOrWhiteSpace(picId))
            {
                try
                {
                    var picArr = picId.Split('-');
                    var picDomain = domain.IsNullOrWhiteSpace() ? "PicServiceUrl".ValueOfAppSetting() : domain;
                    picUrl = picDomain + '/' + picType + '/' + picArr[1] + '/' + picId + '.' + _PicTypeMap_[int.Parse(picArr[3])];
                }
                catch (Exception ex)
                {
                    
                }
            }
            return picUrl;
        }
        #endregion

        #region 判断密码强度
        /// <summary>
        /// 判断密码强度
        /// </summary>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public static int GetPwdstrength(string pwd)
        {
            int length = pwd.Length;
            if (length >= 12)
            {
                return 3;
            }
            else if (length >= 9)
            {
                return 2;
            }
            else
            {
                return 1;
            }
        } 
        #endregion

        /// <summary>
        /// 验证密码
        /// </summary>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public static ServiceResult ValidPassword(string pwd)
        {
            ServiceResult result = new ServiceResult();
            result.IsSucceed();
            var validWhite = Regex.IsMatch(pwd, @"/\s/g", RegexOptions.IgnoreCase);
            if (validWhite)
                result.IsFailed("不能输入空格");

            var validPayPwd = Regex.IsMatch(pwd,
                @"^(?![0-9]+$)(?![a-zA-Z]+$)(?![\W]+$)([0-9A-Za-z]{6,15}|[\d\W]{6,15}|[a-zA-Z\W]{6,15}|[A-Za-z\d\W]{6,15})$",
                RegexOptions.IgnoreCase);
            if (!validPayPwd)
                result.IsFailed("长度为6~15位，且必须包含字母、数字、字符中的任意两种以上；字母要区分大小写；");

            return result;
        }

    }
}
