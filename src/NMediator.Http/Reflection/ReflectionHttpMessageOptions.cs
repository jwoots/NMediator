using NMediator.Http.Reflection.BodyConverter;
using System;

namespace NMediator.NMediator.Http.Reflection
{
    public class ReflectionHttpMessageOptions
    {
        public HttpDescriptors Descriptors { get; set; }
        public Uri BaseUri { get; set; }
        public IBodyConverter BodyConverter { get; set; } = new JsonBodyConverter();
    }
}
