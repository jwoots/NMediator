using NMediator.Core.Result;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NMediator.Core.Transport
{
    /// <summary>
    /// Execute handler for message from transport layer
    /// </summary>
    public interface ITransportLevelHandlerExecutor
    {
        /// <summary>
        /// Execute handler
        /// </summary>
        /// <param name="message"></param>
        /// <param name="token"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        Task<IRequestResult> ExecuteHandler(object message, CancellationToken token, IDictionary<string, string> headers);
    }
}
