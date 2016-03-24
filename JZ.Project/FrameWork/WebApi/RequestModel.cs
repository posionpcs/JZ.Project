using System;

namespace FrameWork.WebApi
{
    public class RequestModel
    {
        /// <summary>
        /// 登录的用户
        /// </summary>
        public class UserData
        {
            public string EPlusAccountId { get; set; }
            public string IDCard { get; set; }
            public string InstitutionNo { get; set; }
            public string Mobile { get; set; }
            public string NickName { get; set; }
            public string RealName { get; set; }
            public string UserId { get; set; }
            public string Usrnbr { get; set; }
        }
        /// <summary>
        /// 请求参数
        /// </summary>
        public class RequestData
        {
            public object body { get; set; }
            public string cmd { get; set; }
            public Head head { get; set; }
        }

        /// <summary>
        /// 请求参数
        /// </summary>
        public class Head
        {
            public string appVersion { get; set; }
            public string clientModel { get; set; }
            public string iMEI { get; set; }
            public string oSVersion { get; set; }
            public string reqTime { get; set; }
            public string sN { get; set; }
            public string sessionid { get; set; }
            public string longitude { get; set; }
            public string latitude { get; set; }
            public string ip { get; set; }
            public string location { get; set; }

            /// <summary>
            /// 操作系统类型
            /// </summary>
            public OsEnum OS
            {
                get
                {
                    if (String.IsNullOrEmpty(oSVersion))
                    {
                        return OsEnum.Unknown;
                    }

                    OsEnum os = OsEnum.Unknown;
                    string str = oSVersion.Trim().ToLower().Substring(0, 1);
                    switch (str)
                    {
                        case "a":
                            os = OsEnum.Android;
                            break;
                        case "i":
                            os = OsEnum.IOS;
                            break;
                        case "w":
                            os = OsEnum.Web;
                            break;
                    }
                    return os;
                }
            }
        }

        /// <summary>
        /// 操作系统类型
        /// </summary>
        public enum OsEnum
        {
            /// <summary>
            /// 未知
            /// </summary>
            Unknown = 0,
            IOS = 1,
            Android = 2,
            Web
        }
    }
}