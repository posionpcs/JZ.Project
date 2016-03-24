namespace FrameWork.MQ
{
    using System;

    public interface IRabbitProxy
    {
        void Dequeue(string queue, Action<string> callback);
        bool Enqueue(string queue, string message);
    }
}

