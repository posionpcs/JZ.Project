using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameWork.ESB
{
       
      
        using System;
        using System.Diagnostics;
        using System.Runtime.CompilerServices;
        using System.Runtime.InteropServices;
        using System.Threading.Tasks;

        //public class ServiceBus : IServiceBus, IDisposable
        //{
        //    private IBus _bus;
        //    private string connString = "bus".ValueOfAppSetting();

        //    public ServiceBus()
        //    {
        //        this._bus = RabbitHutch.CreateBus(this.connString);
        //    }

        //    public void Dispose()
        //    {
        //        if (this._bus != null)
        //        {
        //            this._bus.Dispose();
        //            this._bus = null;
        //        }
        //    }

        //    public void Publish<T>(T message) where T : class
        //    {
        //        this._bus.Publish<T>(message);
        //    }

        //    public void Publish<T>(T message, string topic) where T : class
        //    {
        //        this._bus.Publish<T>(message, topic);
        //    }

        //    public async Task PublishAsync<T>(T message) where T : class
        //    {
        //        await this._bus.PublishAsync<T>(message);
        //    }

        //    public async Task PublishAsync<T>(T message, string topic) where T : class
        //    {
        //        await this._bus.PublishAsync<T>(message, topic);
        //    }

        //    public void Subscribe<T>(string subscriptionId, Action<T> onMessage, ushort? prefetchCount = new ushort?(), string topic = null, bool? autoDelete = new bool?(), int? priority = new int?()) where T : class
        //    {
        //        Action<ISubscriptionConfiguration> configure = delegate(ISubscriptionConfiguration x)
        //        {
        //            if (!topic.IsNullOrEmpty())
        //            {
        //                x.WithTopic(topic);
        //            }
        //            if (autoDelete.HasValue)
        //            {
        //                x.WithAutoDelete(autoDelete.Value);
        //            }
        //            if (priority.HasValue)
        //            {
        //                x.WithPrefetchCount(prefetchCount.Value);
        //            }
        //            if (autoDelete.HasValue)
        //            {
        //                x.WithAutoDelete(autoDelete.Value);
        //            }
        //        };
        //        this._bus.Subscribe<T>(subscriptionId, onMessage, configure);
        //    }


        //}
   


}
