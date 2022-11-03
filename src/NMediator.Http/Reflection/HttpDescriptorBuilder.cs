using System;
using System.Linq.Expressions;
using System.Net.Http;

namespace NMediator.NMediator.Http.Reflection
{
    public class HttpDescriptorBuilder<T>
    {
        private readonly HttpDescriptor _descriptor = new HttpDescriptor();

        public HttpDescriptorBuilder<T> CallRelativeUri(string relativeUri, HttpMethod method, ParameterLocation defaultParamterLocation)
        {
            _descriptor.RelativeUri = relativeUri;
            _descriptor.Method = method;
            _descriptor.ParameterLocation = defaultParamterLocation;
            return this;
        }

        public HttpDescriptorBuilder<T> OverrideParameterLocation(Expression<Func<T,object>> expression, ParameterLocation parameterLocation)
        {
            if(expression.Body is MemberExpression member)
            {
                _descriptor.ParameterLocationOverride[member.Member] = parameterLocation;
                return this;
            }

            throw new InvalidOperationException("expression body must be a MemberExpression");
        }

        public HttpDescriptor Build() => _descriptor;
    }


}
