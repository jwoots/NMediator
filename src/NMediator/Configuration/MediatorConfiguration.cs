using NMediator.SInjector;

namespace NMediator.Configuration
{
    public class MediatorConfiguration : BaseConfiguration
    {
        public Container Container { get; } = new Container();

        public MediatorConfiguration()
        {

        }
    }
}
