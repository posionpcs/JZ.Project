namespace QD.Web.AppApi.Common
{
    /// <summary>
    /// 用户信息
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public string Usrnbr { get; set; }
        /// <summary>
        /// 平台机构号
        /// </summary>
        public string InstitutionNo { get; set; }
        /// <summary>
        /// 身份证号码
        /// </summary>
        public string IDCard { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 用户昵称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 用户真实姓名
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// E+账户Id
        /// </summary>
        public string EPlusAccountId { get; set; }
    }
}