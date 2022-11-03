using System.Security.Cryptography.X509Certificates;
using System.Linq;
using NMediator.Http.Reflection.BodyConverter;
using NMediator.Http.Reflection.QueryStringBinder;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;

namespace NMediator.NMediator.Http.Reflection
{
    public class HttpDescriptors
    {
        private readonly IDictionary<Type, HttpDescriptor> _descriptors = new Dictionary<Type, HttpDescriptor>();
        public ICollection<IQueryStringBinder> QueryParametersBinders { get; protected set; } = new List<IQueryStringBinder>()
        {
            new StringConvertableBinder(),
            new EnumerableBinder(),
            new ArrayBinder()
        };

        public IBodyConverter BodyConverter { get; protected set; } = new JsonBodyConverter();


        public void AddFor<T>(HttpDescriptor descriptor)
        {
            _descriptors[typeof(T)] = descriptor;
        }

        public void AddFor<T>(Action<HttpDescriptorBuilder<T>> builderAction)
        {
            var builder = new HttpDescriptorBuilder<T>();
            builderAction(builder);
            AddFor<T>(builder.Build());
        }

        public HttpDescriptor GetFor(Type type)
        {
            return _descriptors[type];
        }

        public IEnumerable<Type> GetRegisteredTypes() => _descriptors.Keys;
    }

    public class HttpDescriptor
    {
        public string RelativeUri { get; set; }
        public ParameterLocation ParameterLocation { get; set; }
        public HttpMethod Method { get; set; }
        public IDictionary<MemberInfo, ParameterLocation> ParameterLocationOverride {get;} = new Dictionary<MemberInfo, ParameterLocation>();

        public IDictionary<PropertyInfo, object> GetPropertiesForLocation(object message, ParameterLocation location)
        {
            Type t = message.GetType();
            Dictionary<PropertyInfo, object> dico = new Dictionary<PropertyInfo, object>();

            foreach(var p in t.GetProperties())
            {
                if(ParameterLocationOverride.TryGetValue(p, out var locationOverride))
                {
                    if(locationOverride == location)
                        dico[p] = p.GetValue(message);
                }
                else if(ParameterLocation == location)
                {
                    dico[p] = p.GetValue(message);
                }
            }

            return dico;
        }
    }
}
