using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace System
{
    public static class TypeExtensions
    {
        public static bool IsEnumerable(this Type source)
        {
            return typeof(IEnumerable).IsAssignableFrom(source);
        }

        /// <summary>
        /// return true et provide generic argument if current type is IEnumerable<>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="genericArgument"></param>
        /// <returns></returns>
        public static bool TryGetEnumerableGenericArgument(this Type source, out Type genericArgument)
        {
            genericArgument = source
                .GetInterfaces().Union(new[] {source})
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                ?.GetGenericArguments()
                ?.FirstOrDefault();
            return true;
        }

        public static TypeConverter GetConverter(this Type source) => TypeDescriptor.GetConverter(source);
    }
}
