using NMediator.Result;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NMediator.Transport
{
    public interface ITransportLevelHandlerExecutor
    {
        Task<IRequestResult> ExecuteHandler(object message, IDictionary<string, string> headers);
    }
}
