using NMediator.Activator;
using NMediator.Request;
using NMediator.Result;
using System.Threading.Tasks;

namespace NMediator.InProcess
{
    public class InProcessRequestProcessor : IRequestProcessor
    {
        private readonly IInstanceActivator _instanceActivator;

        public InProcessRequestProcessor(IInstanceActivator instanceActivator)
        {
            _instanceActivator = instanceActivator;
        }

        public Task<RequestResult<TResult>> Process<TRequest, TResult>(TRequest request) 
            where TRequest : IRequest<TResult>
        {
            var instance = _instanceActivator.GetInstance<IRequestHandler<TRequest, TResult>>();
            return instance.Handle(request);

        }
    }
}
