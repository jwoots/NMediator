using NMediator.Core.Result;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NMediator.Core.Transport
{
    public interface ITransportLevelHandlerExecutor
    {
        Task<IRequestResult> ExecuteHandler(object message, CancellationToken token, IDictionary<string, string> headers);
    }
}
