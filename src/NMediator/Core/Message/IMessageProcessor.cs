using NMediator.Core.Result;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NMediator.Core.Message
{
    public interface IMessageProcessor
    {
        Task<RequestResult<TResult>> Process<TMessage, TResult>(TMessage message, CancellationToken cancellationToken, IDictionary<string,string> headers = null) 
            where TMessage : IMessage<TResult>;
    }

    public interface IMessageHandler<TMessage,TResult> where TMessage : IMessage<TResult>
    {
        Task<RequestResult<TResult>> Handle(TMessage message, CancellationToken cancellationToken);
    }

    public interface IMessage<TResult>
    { }

    public interface IMessage : IMessage<Nothing> 
    { }
}
