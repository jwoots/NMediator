using NMediator.Core.Message;
using NMediator.Core.Result;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NMediator.Core.Transport
{
    public interface IOutgoingTransport
    {
        Task<IRequestResult> SendMessage<TMessage, TResult>(TMessage message, IDictionary<string, string> headers) where TMessage : IMessage<TResult>;
    }
}
