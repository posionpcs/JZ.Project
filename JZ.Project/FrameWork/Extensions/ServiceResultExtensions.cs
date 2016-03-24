using System;

namespace FrameWork
{
    public static class ServiceResultExtensions
    {
        public static T Get<T>(this ServiceResult svr, string key) where T: class
        {
            if (svr.Data.ContainsKey(key))
            {
                return (svr.Data[key] as T);
            }
            return default(T);
        }

        public static ServiceResult IsFailed(this ServiceResult svr)
        {
            svr.ResultCode = ServiceResultCode.FAILED;
            return svr;
        }

        public static ServiceResult IsFailed(this ServiceResult svr, Exception ex)
        {
            svr.ResultCode = ServiceResultCode.FAILED;
            setMessage(svr, ex);
            return svr;
        }

        public static ServiceResult IsFailed(this ServiceResult svr, string msg)
        {
            svr.ResultCode = ServiceResultCode.FAILED;
            svr.Message = msg;
            return svr;
        }

        public static ServiceResult IsSucceed(this ServiceResult svr)
        {
            svr.ResultCode = ServiceResultCode.SUCCEED;
            return svr;
        }

        public static ServiceResult IsSucceed(this ServiceResult svr, Exception ex)
        {
            svr.ResultCode = ServiceResultCode.SUCCEED;
            setMessage(svr, ex);
            return svr;
        }

        public static ServiceResult IsSucceed(this ServiceResult svr, string msg)
        {
            svr.ResultCode = ServiceResultCode.SUCCEED;
            svr.Message = msg;
            return svr;
        }

        public static void Set(this ServiceResult svr, string name, object value)
        {
            svr.Data[name] = value;
        }

        private static void setMessage(ServiceResult svr, Exception ex)
        {
            svr.Message = (ex.InnerException == null) ? ex.ToString() : (ex.ToString() + "\r\nInnerException:\r\n" + ex.InnerException.ToString());
        }

        public static bool TryGet(this ServiceResult svr, string name, out object value)
        {
            return svr.Data.TryGetValue(name, out value);
        }
    }
}

