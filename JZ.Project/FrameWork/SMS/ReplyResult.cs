using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameWork.SMS
{

    [Serializable]
    public class ReplyResult
    {
        public string Ext { get; set; }
        public string Message { get; set; }
        public string Mobile { get; set; }
        public DateTime Time { get; set; }
    }


}
