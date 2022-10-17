using System;
using System.Collections.Generic;
using System.Text;

namespace NMediator.Core.Handling
{
    /// <summary>
    /// Provide handler interface type (IMessageHandler<,>) by message type
    /// </summary>
    public interface IHandlerInterfaceTypeProvider
    {
        /// <summary>
        /// Get handler type corresponding to message type
        /// </summary>
        /// <param name="messageType"></param>
        /// <returns></returns>
        Type GetHandlerInterfaceTypeByMessageType(Type messageType);
    }
}
