using NMediator.Activator;
using NMediator.SInjector;

namespace NMediator.Configuration
{
    public class MediatorConfiguration : BaseConfiguration
    {
        public Container Container { get; } = new Container();

        public MediatorConfiguration()
        {

        }

        public MediatorConfiguration WithActivator(IHandlerActivator activator)
        {
            Container.Register<IHandlerActivator>(ctx => activator);
            return this;
        }
    }
}
