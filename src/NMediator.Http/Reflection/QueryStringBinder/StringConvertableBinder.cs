using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace NMediator.Http.Reflection.QueryStringBinder
{
    internal class StringConvertableBinder : IQueryStringBinder
    {
        public IEnumerable<string> BindToString(Type type, object value)
        {
            yield return TypeDescriptor.GetConverter(type).ConvertToInvariantString(value);
        }

        public object BindToType(Type type, IEnumerable<string> value)
        {
            var converter = type.GetConverter();
            return converter.ConvertFromInvariantString(value.First());
        }

        public bool CanBindToString(Type type, object value)
        {
            return TypeDescriptor.GetConverter(type).CanConvertFrom(typeof(string));
        }

        public bool CanBindToType(Type type, IEnumerable<string> value)
        {
            return TypeDescriptor.GetConverter(type).CanConvertFrom(typeof(string));
        }
    }
}
