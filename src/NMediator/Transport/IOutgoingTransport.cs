using NMediator.Result;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NMediator.Transport
{
    public interface IOutgoingTransport
    {
        Task<IRequestResult> SendMessage(object message);
    }
}
