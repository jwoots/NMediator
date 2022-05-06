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

        public HttpDescriptor Build() => _descriptor;
    }
}
