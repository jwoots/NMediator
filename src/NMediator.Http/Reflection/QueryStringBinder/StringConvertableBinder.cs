using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace NMediator.Http.Reflection.QueryStringBinder
{
    internal class StringConvertableBinder : IQueryStringBinder
    {
        public IEnumerable<string> Bind(Type type, object value)
        {
            yield return TypeDescriptor.GetConverter(type).ConvertToInvariantString(value);
        }

        public bool CanBind(Type type, object value)
        {
            return TypeDescriptor.GetConverter(type).CanConvertFrom(typeof(string));
        }
    }
}
