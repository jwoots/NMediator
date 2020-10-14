using System;
using System.Collections.Generic;
using System.Text;

namespace NMediator.Routing
{
    interface IRouter
    {
        Pipeline Route(object message);
    }
}
