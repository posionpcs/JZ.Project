namespace FrameWork.SMS
{
    public interface ISms
    {
        ReplyResult[] PullReply();
        bool Send(string mobile, string content);
    }
}
