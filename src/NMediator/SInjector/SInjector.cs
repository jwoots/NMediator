using System;
using System.Collections.Generic;
using System.Linq;

namespace NMediator.SInjector
{
    public class Container
    {
        private readonly IDictionary<Type, RegistrationContext> _services = new Dictionary<Type, RegistrationContext>();

        public void Register(Type serviceType, Func<IResolutionContext, object> implemType)
        {
            if (_services.TryGetValue(serviceType, out var rc))
            {
                rc.ImplemType = implemType;
                return;
            }

            rc = new RegistrationContext(serviceType, this);
            _services[serviceType] = rc;
            rc.ImplemType = implemType;
        }

        public void Register<TService>(Func<IResolutionContext, TService> implementation)
            => Register(typeof(TService), rc => implementation(rc));


        public object Get(Type serviceType)
        {
            if (_services.TryGetValue(serviceType, out var rc))
            {
                return new ResolutionContext(rc).Get(serviceType);
            }

            throw new InvalidOperationException($"No registation found for service type {serviceType}");
        }

        public TService Get<TService>() => (TService)Get(typeof(TService));

        public void Decorate(Type serviceType, Func<IResolutionContext, object> decorator)
        {
            if (_services.TryGetValue(serviceType, out var rc))
            {
                rc.Decorators.Add(decorator);
                return;
            }

            rc = new RegistrationContext(serviceType, this);
            _services[serviceType] = rc;
            rc.Decorators.Add(decorator);
        }

        public void Decorate<TService>(Func<IResolutionContext, TService> decorator)
            => Decorate(typeof(TService), rc => decorator(rc));
    }

    /// <summary>
    /// Registration context containing all informations for a service type
    /// </summary>
    internal class RegistrationContext
    {
        public RegistrationContext(Type serviceType, Container container)
        {
            ServiceType = serviceType;
            Container = container;
        }

        public ICollection<Func<IResolutionContext, object>> Decorators { get; } = new List<Func<IResolutionContext, object>>();
        public Func<IResolutionContext, object> ImplemType { get; set; }
        public Type ServiceType { get; }
        public Container Container { get; }
    }

    /// <summary>
    /// Resolution context containing all informations allowing to run one service type resolution
    /// </summary>
    public interface IResolutionContext
    {
        object Get(Type serviceType);
        TService Get<TService>();
    }

    internal class ResolutionContext : IResolutionContext
    {
        private readonly RegistrationContext _rc;
        private int _index = 0;

        public ResolutionContext(RegistrationContext rc)
        {
            _rc = rc;
        }

        public object Get(Type serviceType)
        {
            //the requested service type is different of this context service type => Get from container
            if (serviceType != _rc.ServiceType)
                return _rc.Container.Get(serviceType);

            //if any decorators, resolve decorator
            if (_index < _rc.Decorators.Count)
                return _rc.Decorators.ElementAt(_index++)(this);

            //resolve implem type
            return _rc.ImplemType(this);
        }

        public TService Get<TService>()
            => (TService)Get(typeof(TService));
    }
}
