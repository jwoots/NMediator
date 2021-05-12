using NMediator.Core.Result;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NMediator.Core.Message
{
    public interface IMessageProcessor
    {
        Task<RequestResult<TResult>> Process<TMessage, TResult>(TMessage message, IDictionary<string,string> headers) where TMessage : IMessage<TResult>;
    }

    public interface IMessageHandler<TMessage,TResult> where TMessage : IMessage<TResult>
    {
        Task<RequestResult<TResult>> Handle(TMessage message);
    }

    public interface IMessage<TResult>
    { }

    public interface IMessage : IMessage<Nothing> 
    { }
}
