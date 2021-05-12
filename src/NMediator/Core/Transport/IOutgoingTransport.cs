using NMediator.Core.Result;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NMediator.Core.Transport
{
    public interface IOutgoingTransport
    {
        Task<IRequestResult> SendMessage(object message, IDictionary<string, string> headers);
    }
}
