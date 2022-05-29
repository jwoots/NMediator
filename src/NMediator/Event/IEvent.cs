using NMediator.Core.Message;
using NMediator.Core.Result;
using System;
using System.Collections.Generic;
using System.Text;

namespace NMediator.Event
{
    public interface IEvent : IMessage<Nothing>
    {
    }
}
