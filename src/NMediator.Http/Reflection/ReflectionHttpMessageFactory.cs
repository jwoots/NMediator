using NMediator.Core.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;

namespace NMediator.NMediator.Http.Reflection
{
    public class ReflectionHttpMessageFactory : SimpleHttpMessageFactory
    {
        private readonly Uri _baseUri;
        private readonly HttpDescriptors _descriptors;

        public ReflectionHttpMessageFactory(Uri baseUri, HttpDescriptors descriptors) : base(descriptors.BodyConverter)
        {
            _baseUri = baseUri;
            _descriptors = descriptors;
            _descriptors.ErrorMapper = base.ErrorFactory;
        }

        protected override RequestResult<HttpRequestMessage> CreateRequest(object message)
        {
            var result = base.CreateRequest(message);
            if (result.IsSuccess)
                return result;

            var descriptor = _descriptors.GetFor(message.GetType());
            var messageProperties = message.GetType().GetProperties().ToDictionary(pi => pi, pi => pi.GetValue(message));
            HttpRequestMessage toReturn = new HttpRequestMessage();
            toReturn.Method = descriptor.Method;

            var uriResult = CreateUri(descriptor, messageProperties);
            toReturn.RequestUri = uriResult.Uri;
            var messagePropertiesRemaining = messageProperties.Where(x => !uriResult.UsedProperties.Contains(x.Key))
                                                              .ToDictionary(x => x.Key, x => x.Value);

            //body
            var bodyProperties = descriptor.GetPropertiesForLocation(message, ParameterLocation.BODY);
            var propertiesToSerialize = bodyProperties.Where(x => messagePropertiesRemaining.ContainsKey(x.Key))
                .ToDictionary(m => m.Key.Name, m => m.Value);
            var body = _descriptors.BodyConverter.Convert(propertiesToSerialize);
            toReturn.Content = new StringContent(body, BodyConverter.Encoding, BodyConverter.ContentType);
                

            //query string
            var queryStringBuilder = HttpUtility.ParseQueryString(string.Empty);
            var queryProperties = descriptor.GetPropertiesForLocation(message, ParameterLocation.QUERY_STRING);

            foreach (var entry in queryProperties.Where(x => messagePropertiesRemaining.ContainsKey(x.Key)))
            {
                if (entry.Value == null) continue;

                foreach (var parameter in BuildHttpParameter(entry.Key, entry.Value))
                    queryStringBuilder.Add(entry.Key.Name, parameter);
            }
            
            //build uri
            toReturn.RequestUri = new UriBuilder(toReturn.RequestUri)
            {
                Query = queryStringBuilder.ToString()
            }.Uri;
          
            return RequestResult.Success(toReturn);
        }

        private UriResult CreateUri(HttpDescriptor descriptor, IDictionary<PropertyInfo, object> parameters)
        {
            var uri = _baseUri.ToString();
            if (!uri.EndsWith("/"))
                uri += "/";

            var relativeUri = descriptor.RelativeUri;
            if (relativeUri.StartsWith("/"))
                relativeUri = relativeUri.Substring(1);

            uri += relativeUri;

            HashSet<PropertyInfo> usedProperties = new();
            foreach (var parameter in parameters)
            {
                if (parameter.Value == null)
                    if (Regex.IsMatch(uri, parameter.Key.Name, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100)))
                        throw new InvalidOperationException("can not place null value in path url parameter");
                    else
                        continue;

                var values = BuildHttpParameter(parameter.Key, parameter.Value);
                var newUri = Regex.Replace(uri, $"{{{parameter.Key.Name}}}", string.Join(",", values), RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100));

                if (newUri != uri)
                    usedProperties.Add(parameter.Key);

                uri = newUri;
            }

            return new UriResult { Uri = new Uri(uri), UsedProperties = usedProperties };
        }

        private string[] BuildHttpParameter(PropertyInfo pi, object value)
        {
            var typeToEvaluate = Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType;

            return _descriptors.QueryParametersBinders
                .FirstOrDefault(x => x.CanBindToString(typeToEvaluate, value))
                ?.BindToString(typeToEvaluate, value)?.ToArray()
                ?? throw new InvalidOperationException($"Can not build query string for property {pi.Name}");
        }

        private sealed class UriResult
        {
            public Uri Uri { get; set; } = null!;
            public IEnumerable<PropertyInfo> UsedProperties { get; set; } = null!;
        }
    }
}
