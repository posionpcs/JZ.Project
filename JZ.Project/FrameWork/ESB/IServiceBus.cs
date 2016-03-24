using System;
using System.Threading.Tasks;

namespace FrameWork.ESB
{
    public interface IServiceBus : IDisposable
    {
        void Publish<T>(T message) where T: class;
        void Publish<T>(T message, string topic) where T: class;
        Task PublishAsync<T>(T message) where T: class;
        Task PublishAsync<T>(T message, string topic) where T: class;
        void Subscribe<T>(string subscriptionId, Action<T> onMessage, ushort? prefetchCount = new ushort?(), string topic = null, bool? autoDelete = new bool?(), int? priority = new int?()) where T: class;
    }
}

