using System;
using System.Collections.Generic;

namespace NMediator.Extensions
{
    public static class IEnumeratorExtensions
    {
        
        public static void ForEach<T>(this IEnumerable<T> enumerator, Action<T> action)
        {
            foreach (var e in enumerator)
                action(e);
        }
    }
}
