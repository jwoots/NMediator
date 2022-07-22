using System;
using System.Collections.Generic;
using System.Text;

namespace NMediator.Core.Activator
{
    /// <summary>
    /// Provide handler type by message type
    /// </summary>
    public interface IHandlerProvider
    {
        /// <summary>
        /// Get handler type corresponding to message type
        /// </summary>
        /// <param name="messageType"></param>
        /// <returns></returns>
        Type GetHandlerTypeByMessageType(Type messageType);
    }
}
