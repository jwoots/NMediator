using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace NMediator.Http.Reflection.QueryStringBinder
{
    internal class ArrayBinder : IQueryStringBinder
    {
        public IEnumerable<string> Bind(Type type, object value)
        {
            var argType = type.GetElementType();
            return ((IEnumerable)value).Cast<object>().Select(x => argType.GetConverter().ConvertToInvariantString(x)).ToArray();
        }

        public bool CanBind(Type type, object value)
        {
            return type.IsArray && type.GetElementType().GetConverter().CanConvertFrom(typeof(string));
        }
    }
}
