using NMediator.Core.Result;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NMediator.Core.Transport
{
    public interface ITransportLevelHandlerExecutor
    {
        Task<IRequestResult> ExecuteHandler(object message, IDictionary<string, string> headers);
    }
}
