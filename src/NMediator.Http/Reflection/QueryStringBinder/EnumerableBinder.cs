using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NMediator.Http.Reflection.QueryStringBinder
{
    internal class EnumerableBinder : IQueryStringBinder
    {
        public IEnumerable<string> Bind(Type type, object value)
        {
            type.TryGetEnumerableGenericArgument(out var argType);
            var result = ((IEnumerable)value)
                .Cast<object>()
                .Select(x => argType.GetConverter().ConvertToInvariantString(x)).ToArray();

            return result;
        }

        public bool CanBind(Type type, object value)
        {
            return type.TryGetEnumerableGenericArgument(out var argType)
                && argType.GetConverter().CanConvertTo(typeof(string));
        }
    }
}
