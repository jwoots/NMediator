using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace NMediator.Core.Configuration
{
    [ExcludeFromCodeCoverage]
    public abstract class BaseConfiguration : IDisposable
    {
        public static void Configure(BaseConfiguration config)
        {
            config.RegisterConfiguration();
            config.ResolveConfiguration();
            config.StartConfiguration();
        }

        public ConfigurationExtender Extender { get; } = new ConfigurationExtender();

        private ICollection<BaseConfiguration> _children => _namedChildren.Values;
        private IDictionary<string, BaseConfiguration> _namedChildren = new Dictionary<string, BaseConfiguration>();

        public void RegisterConfiguration(BaseConfiguration config) => RegisterNamedConfiguration(config, Guid.NewGuid().ToString());

        public void RegisterNamedConfiguration<T>(T config) where T : BaseConfiguration
        {
            RegisterNamedConfiguration(config, typeof(T).AssemblyQualifiedName);
        }
        public void RegisterNamedConfiguration(BaseConfiguration config, string name)
        {
            this._namedChildren[name] = config;
            foreach (var kvp in config._namedChildren)
                _namedChildren.Add(kvp);

            config._namedChildren = this._namedChildren;
        }

        public T GetConfiguration<T>(string name) where T : BaseConfiguration => _namedChildren.ContainsKey(name) ? (T)_namedChildren[name] : default(T);
        public T GetConfiguration<T>() where T : BaseConfiguration => _namedChildren.ContainsKey(typeof(T).AssemblyQualifiedName) ? (T)_namedChildren[typeof(T).AssemblyQualifiedName] : default(T);

        public T GetOrRegisterConfiguration<T>(Func<T> factory) where T : BaseConfiguration
        {
            T config = GetConfiguration<T>();
            if (config == null)
            {
                config = factory();
                RegisterNamedConfiguration(config);
            }

            return config;
        }

        public virtual void Dispose()
        {
            _children.Reverse().ForEach(c => {
                c.Extender._toDispose.ForEach(action => action());
                c.OnDispose();
            });

            Extender._toDispose.ForEach(actionToDispose => actionToDispose());
            OnDispose();
        }

        private void RegisterConfiguration()
        {
            Register();
            Extender._toRegister.ForEach(actionToRegister => actionToRegister());

            _children.ForEach(c => {
                c.Register();
                c.Extender._toRegister.ForEach(actionToRegister => actionToRegister());
            });

        }

        private void ResolveConfiguration()
        {
            Resolve();
            Extender._toResolve.ForEach(actionToResolve => actionToResolve());

            _children.ForEach(c => {
                c.Resolve();
                c.Extender._toResolve.ForEach(action => action());
            });
        }

        private void StartConfiguration()
        {
            Start();
            Extender._toStart.ForEach(actionToStart => actionToStart());

            _children.ForEach(c => {
                c.Start();
                c.Extender._toStart.ForEach(action => action());
            });
        }

        protected virtual void Register()
        {

        }
        protected virtual void Resolve()
        {

        }
        protected virtual void Start()
        {

        }

        protected virtual void OnDispose()
        {

        }
    }

    [ExcludeFromCodeCoverage]
    public class ConfigurationExtender
    {
        internal readonly ICollection<Action> _toRegister = new List<Action>();
        internal readonly ICollection<Action> _toResolve = new List<Action>();
        internal readonly ICollection<Action> _toStart = new List<Action>();
        internal readonly ICollection<Action> _toDispose = new List<Action>();

        private readonly IDictionary<Type, object> _dico = new Dictionary<Type, object>();


        public void OnResolving(Action a)
        {
            _toResolve.Add(a);
        }

        public void OnRegistering(Action a)
        {
            _toRegister.Add(a);
        }

        public void OnStarting(Action a)
        {
            _toStart.Add(a);
        }

        public void OnDisposing(Action a)
        {
            _toDispose.Add(a);
        }

        public void Put<T>(T context)
        {
            _dico.Add(typeof(T), context);
        }

        public T Get<T>()
        {
            return _dico.ContainsKey(typeof(T)) ? (T)_dico[typeof(T)] : default(T);
        }

    }
}