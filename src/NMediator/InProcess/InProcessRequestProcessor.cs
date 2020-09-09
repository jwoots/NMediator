using NMediator.Activator;
using NMediator.Request;
using NMediator.Result;
using System.Threading.Tasks;

namespace NMediator.InProcess
{
    public class InProcessRequestProcessor : IRequestProcessor
    {
        private readonly IHandlerActivator _instanceActivator;

        public InProcessRequestProcessor(IHandlerActivator instanceActivator)
        {
            _instanceActivator = instanceActivator;
        }

        public Task<RequestResult<TResult>> Execute<TRequest, TResult>(TRequest request) 
            where TRequest : IRequest<TResult>
        {
            var instance = _instanceActivator.GetInstance<IRequestHandler<TRequest, TResult>>();
            return instance.Handle(request);

        }
    }
}
