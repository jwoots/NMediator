using System;
using System.Collections.Generic;
using System.Text;

namespace NMediator.Pipeline
{
    interface IPipeline
    {
        object ExecutePipeline(object message);
    }
}
