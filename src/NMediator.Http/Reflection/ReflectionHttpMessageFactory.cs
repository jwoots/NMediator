using NMediator.Core.Result;
using NMediator.Http;
using NMediator.Http.Reflection.BodyConverter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Web;

namespace NMediator.NMediator.Http.Reflection
{
    public  class ReflectionHttpMessageFactory : SimpleHttpMessageFactory
    {
        private readonly ReflectionHttpMessageOptions _options;

        public ReflectionHttpMessageFactory(ReflectionHttpMessageOptions options)
        {
            _options = options;
        }

        protected override RequestResult<HttpRequestMessage> CreateRequest(object message)
        {
            var result =  base.CreateRequest(message);
            if (result.IsSuccess)
                return result;

            var descriptor = _options.Descriptors.GetFor(message.GetType());
            var messageProperties = message.GetType().GetProperties().ToDictionary(pi => pi, pi => pi.GetValue(message));
            HttpRequestMessage toReturn = new HttpRequestMessage();

            toReturn.RequestUri = CreateUri(descriptor, messageProperties);

            if (descriptor.ParameterLocation == ParameterLocation.BODY)
            {
                var propertiesToSerialize = messageProperties.ToDictionary(m => m.Key.Name, m => m.Value);
                toReturn.Content = _options.BodyConverter.Convert(propertiesToSerialize);
            }
            else if (descriptor.ParameterLocation == ParameterLocation.QUERY_STRING)
            {
                var queryStringBuilder = HttpUtility.ParseQueryString(string.Empty);

                foreach (var entry in messageProperties)
                {
                    if (entry.Value == null) continue;

                    foreach (var parameter in BuildHttpParameter(entry.Key, entry.Value))
                        queryStringBuilder.Add(entry.Key.Name, parameter);
                }

                toReturn.RequestUri = new UriBuilder(toReturn.RequestUri)
                {
                    Query = queryStringBuilder.ToString()
                }.Uri;
            }
            else
                throw new InvalidOperationException();

            return RequestResult.Success(toReturn);
        }

        private Uri CreateUri(HttpDescriptor descriptor, IDictionary<PropertyInfo, object> parameters)
        {
            var uri = _options.BaseUri.ToString();
            if (!uri.EndsWith("/"))
                uri += "/";

            var relativeUri = descriptor.RelativeUri;
            if(relativeUri.StartsWith("/"))
                relativeUri = relativeUri.Substring(1);

            uri += relativeUri;

            foreach(var parameter in parameters)
            {
                if (parameter.Value == null)
                    if (Regex.IsMatch(uri, parameter.Key.Name, RegexOptions.IgnoreCase))
                        throw new InvalidOperationException("can not place null value in path url parameter");
                    else
                        continue;

                var values = BuildHttpParameter(parameter.Key, parameter.Value);
                var newUri = Regex.Replace(uri, $"{{{parameter.Key.Name}}}", string.Join(",",values), RegexOptions.IgnoreCase);
                
                if(newUri != uri)
                    parameters.Remove(parameter.Key);
                
                uri = newUri;
            }

            return new Uri(uri);
        }

        private string[] BuildHttpParameter(PropertyInfo pi, object value)
        {
            var typeToEvaluate = Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType;

            foreach(var binder in NMeditatorHttpConfigurations.Binders)
            {
                if (binder.CanBind(typeToEvaluate, value))
                    return binder.Bind(typeToEvaluate, value).ToArray();
            }
            
            throw new InvalidOperationException($"Can not build query string for property {pi.Name}");
        }
    }

    public class HttpDescriptors
    {
        private readonly IDictionary<Type, HttpDescriptor> _descriptors = new Dictionary<Type, HttpDescriptor>(); 

        public void AddFor<T>(HttpDescriptor descriptor)
        {
            _descriptors[typeof(T)] = descriptor; 
        }

        public HttpDescriptor GetFor<T>()
        {
            return GetFor(typeof(T));
        }

        public HttpDescriptor GetFor(Type type)
        {
            return _descriptors[type];
        }
    }

    public class HttpDescriptor
    {
        private readonly IDictionary<Expression, object> _customizations = new Dictionary<Expression, object>();
        public string RelativeUri { get; set; }
        public ParameterLocation ParameterLocation { get; set; }
        public HttpMethod Method { get; set; }

       // public void AddCustomization<T, TProperty>(Expression<Func<T, TProperty>> expression, )
    }

    public class HttpParameterDescriptor<T, TProperty>
    {
        public Expression<Func<T, TProperty>> Expression { get; }

        public HttpParameterDescriptor(Expression<Func<T, TProperty>> expression)
        {
            Expression = expression;
        }

        public ParameterLocation ParamterLocation { get; set; }
        public Func<TProperty, string> Serializer { get; set; }
    }

    public class HttpDescriptorBuilder<T>
    {
        private readonly HttpDescriptor _descriptor;

        public HttpDescriptorBuilder<T> CallRelativeUri(string relativeUri, HttpMethod method, ParameterLocation defaultParamterLocation)
        {
            _descriptor.RelativeUri = relativeUri;
            _descriptor.Method = method;
            _descriptor.ParameterLocation = defaultParamterLocation;
            return this;
        }
        
        //public HttpParameterDescriptor<T, TProperty> AddParameterCustomization<TProperty>(Expression<Func<T, TProperty>> expression)
        //{
        //    var p = new HttpParameterDescriptor<T, TProperty>(expression);
        //}

       
    }

    public class HttpParameterDescriptorBuilder<TProperty>
    {

    }
        

    public enum ParameterLocation
    {
        BODY, QUERY_STRING
    }

    public class ReflectionHttpMessageOptions
    {
        public HttpDescriptors Descriptors { get; set; }    
        public Uri BaseUri { get; set; }
        public IBodyConverter BodyConverter { get; set; } = new JsonBodyConverter();
    }
}
