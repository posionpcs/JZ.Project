namespace FrameWork.WebApi
{
    public interface IProcessor
    {
        BaseResponse Process(BaseRequest request);
    }
}