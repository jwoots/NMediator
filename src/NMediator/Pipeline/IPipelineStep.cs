using System;

namespace NMediator.Pipeline
{
    public interface IPipelineStep
    {
        object Invoke(object message, IPipelineStep next);
    }
}