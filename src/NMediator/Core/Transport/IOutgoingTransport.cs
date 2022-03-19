using NMediator.Core.Message;
using NMediator.Core.Result;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NMediator.Core.Transport
{
    /// <summary>
    /// Outgoing transport allow to push message to the communication layer (inprocess, http, soap...)
    /// </summary>
    public interface IOutgoingTransport
    {
        /// <summary>
        /// Send message to the communication layer (transport)
        /// </summary>
        /// <typeparam name="TMessage"> the message type</typeparam>
        /// <typeparam name="TResult"> the result type</typeparam>
        /// <param name="message">the message to send</param>
        /// <param name="headers">headers (meta-datas)</param>
        /// <returns></returns>
        Task<IRequestResult> SendMessage<TMessage, TResult>(TMessage message, IDictionary<string, string> headers) where TMessage : IMessage<TResult>;
    }
}
