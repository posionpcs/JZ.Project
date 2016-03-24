
namespace FrameWork.WebApi
{

    /// <summary>
    /// 请求实体的父类
    /// </summary>
    public class BaseRequest
    {
        public RequestModel.UserData UserData { get; set; }

        public RequestModel.Head Head { get; set; }

        public string RequestId { get; set; }

        public string Cmd { get; set; }

    }
}