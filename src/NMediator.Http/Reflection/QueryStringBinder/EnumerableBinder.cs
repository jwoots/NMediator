using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NMediator.Http.Reflection.QueryStringBinder
{
    internal class EnumerableBinder : IQueryStringBinder
    {
        public IEnumerable<string> BindToString(Type type, object value)
        {
            type.TryGetEnumerableGenericArgument(out var argType);
            var result = ((IEnumerable)value)
                .Cast<object>()
                .Select(x => argType.GetConverter().ConvertToInvariantString(x)).ToArray();

            return result;
        }

        public object BindToType(Type type, IEnumerable<string> value)
        {
            type.TryGetEnumerableGenericArgument(out var argType);
            var converter = argType.GetConverter();

            if (argType != typeof(string)
                && value.Count() == 1
                && value.First().Contains(","))
            {
                value = value.First().Split([','],StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim());
            }

            if (type.IsInterface)
            {
                var array = Array.CreateInstance(argType, value.Count());
                value.ForEach((index, item) =>
                {
                    array.SetValue(converter.ConvertFromInvariantString(item), index);
                });

                return array;
            }
            else
            {
                var list = Activator.CreateInstance(type) as IList;
                foreach(var item in value)
                {
                    list.Add(converter.ConvertFromInvariantString(item));
                }

                return list;
            }
        }

        public bool CanBindToString(Type type, object value)
        {
            return type.TryGetEnumerableGenericArgument(out var argType)
                && argType.GetConverter().CanConvertFrom(typeof(string));
        }

        public bool CanBindToType(Type type, IEnumerable<string> value)
        {
            return type.TryGetEnumerableGenericArgument(out var argType)
               && argType.GetConverter().CanConvertFrom(typeof(string));
        }
    }
}
