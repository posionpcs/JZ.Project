using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameWork.SMS
{
    public interface ISms
    {
        ReplyResult[] PullReply();
        bool Send(string mobile, string content);
    }
}
