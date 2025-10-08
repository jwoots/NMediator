using NMediator.Http;
using NMediator.Http.Reflection.BodyConverter;
using NMediator.Http.Reflection.QueryStringBinder;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace NMediator.NMediator.Http.Reflection
{
    public class HttpDescriptors
    {
        private readonly IDictionary<Type, HttpDescriptor> _descriptors = new Dictionary<Type, HttpDescriptor>();
        public ICollection<IQueryStringBinder> QueryParametersBinders { get; protected set; } = new List<IQueryStringBinder>()
        {
            new StringConvertableBinder(),
            new ArrayBinder(),
            new EnumerableBinder()
            
        };

        public IBodyConverter BodyConverter { get; protected set; } = new JsonBodyConverter();
        public HttpResponseToErrorMapper ErrorMapper { get; internal set; } = new HttpResponseToErrorMapper();


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

        public void PopulateMessageWithQueryString(NameValueCollection parsedQueryString, object messageToPopulate, IEnumerable<IQueryStringBinder> queryStringBinders)
        {
            Type t = messageToPopulate.GetType();
            var properties = GetPropertiesForLocation(messageToPopulate, ParameterLocation.QUERY_STRING).Keys;

            foreach (var key in parsedQueryString.Keys)
            {
                var stringKey = key.ToString();
                var property = properties.SingleOrDefault(x => string.Compare(x.Name, stringKey, ignoreCase: true) == 0);

                if ( property != null)
                {
                    var stringValues = parsedQueryString.GetValues(stringKey);
                    var queryStringBinder = queryStringBinders.FirstOrDefault(x => x.CanBindToType(property.PropertyType, stringValues));
                    if (queryStringBinder == null)
                    {
                        throw new InvalidOperationException($"no binder found to convert {stringValues} to type {property.PropertyType}");
                    }
                    object typedValue = queryStringBinder.BindToType(property.PropertyType, stringValues);
                        
                    property.SetValue(messageToPopulate, typedValue);
                }
            }
        }

        public void PopulateMessageWithUri(IDictionary<string, string> routeValues, object messageToPopulate, IEnumerable<IQueryStringBinder> queryStringBinders)
        {
            Type t = messageToPopulate.GetType();
            var properties =t.GetProperties();

            foreach (var key in routeValues.Keys)
            {
                var stringKey = key.ToString();
                var property = properties.SingleOrDefault(x => string.Compare(x.Name, stringKey, ignoreCase: true) == 0);

                if (property != null)
                {
                    var stringValues = new string[] { routeValues[key] };
                    object typedValue = queryStringBinders
                        .FirstOrDefault(x => x.CanBindToType(property.PropertyType, stringValues))
                        ?.BindToType(property.PropertyType, stringValues)
                        ?? throw new InvalidOperationException($"no binder found to convert {stringValues} to type {property.PropertyType}");

                    property.SetValue(messageToPopulate, typedValue);
                }
            }
        }

        /// <summary>
        /// Create a new message from serialized body
        /// </summary>
        /// <param name="type"></param>
        /// <param name="body"></param>
        /// <param name="converter"></param>
        /// <returns></returns>
        public Task<object> CreateMessageWithBody(Type type, string body, IBodyConverter converter)
        {
            return Task.FromResult(converter.ConvertToType(type, body));
        }
    }
}
