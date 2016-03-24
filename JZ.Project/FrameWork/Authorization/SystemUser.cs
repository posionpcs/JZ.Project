namespace FrameWork.Authorization
{
    using System;
    using System.Runtime.CompilerServices;

    public class SystemUser
    {
        public string Email { get; set; }

        public DateTime LoginTime { get; set; }

        public string Mobile { get; set; }

        public string RealName { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public int UserType { get; set; }
    }
}

