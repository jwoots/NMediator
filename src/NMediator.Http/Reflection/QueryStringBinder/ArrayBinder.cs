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
        public IEnumerable<string> BindToString(Type type, object value)
        {
            var argType = type.GetElementType();
            return ((IEnumerable)value).Cast<object>().Select(x => argType.GetConverter().ConvertToInvariantString(x)).ToArray();
        }

        public object BindToType(Type type, IEnumerable<string> value)
        {
            var converter = type.GetElementType().GetConverter();

            if (type.GetElementType() != typeof(string)
                && value.Count() == 1
                && value.First().Contains(","))
            {
                value = value.First().Split([','], StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim());
            }

            var array = Array.CreateInstance(type.GetElementType(), value.Count());
            value.ForEach((index, item) =>
            {
                array.SetValue(converter.ConvertFromInvariantString(item), index);
            });

            return array;
        }

        public bool CanBindToString(Type type, object value)
        {
            return type.IsArray && type.GetElementType().GetConverter().CanConvertFrom(typeof(string));
        }

        public bool CanBindToType(Type type, IEnumerable<string> value)
        {
            return type.IsArray && type.GetElementType().GetConverter().CanConvertFrom(typeof(string));
        }
    }
}
