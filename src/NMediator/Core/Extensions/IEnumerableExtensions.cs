using System.Linq;

namespace System.Collections.Generic
{
    public static class IEnumeratorExtensions
    {
        
        public static void ForEach<T>(this IEnumerable<T> enumerator, Action<T> action)
        {
            foreach (var e in enumerator)
                action(e);
        }

        public static void ForEach<T>(this IEnumerable<T> enumerator, Action<int,T> action)
        {
            for(int i=0;i<enumerator.Count();i++)
            {
                action(i,enumerator.ElementAt(i));
            }
        }
    }
}
