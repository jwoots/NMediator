using System;
using System.Collections.Generic;
using System.Text;

namespace NMediator.Pipeline
{
    public class Pipeline : List<IPipelineStep>, IPipeline
    {
        public object ExecutePipeline(object message)
        {
            
        }
    }
}
