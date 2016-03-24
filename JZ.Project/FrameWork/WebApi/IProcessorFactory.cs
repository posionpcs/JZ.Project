namespace FrameWork.WebApi
{
    public interface IProcessorFactory
    {
        IProcessor Create(string bizCode);
    }
}
