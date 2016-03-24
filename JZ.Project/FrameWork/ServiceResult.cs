namespace FrameWork
{
    using System;
    using System.Collections.Generic;

    public class ServiceResult
    {
        public Dictionary<string, object> Data;
        public string Message;
        public int ResultCode;

        public ServiceResult()
        {
            this.Data = new Dictionary<string, object>();
        }

        public ServiceResult(string msg)
        {
            this.Data = new Dictionary<string, object>();
            this.Message = msg;
        }

        public bool Success
        {
            get
            {
                return (this.ResultCode == ServiceResultCode.SUCCEED);
            }
        }
    }
}

