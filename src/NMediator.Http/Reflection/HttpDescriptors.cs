using NMediator.Http.Reflection.BodyConverter;
using NMediator.Http.Reflection.QueryStringBinder;
using System;
using System.Collections.Generic;
using System.Net.Http;

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
    }
}
