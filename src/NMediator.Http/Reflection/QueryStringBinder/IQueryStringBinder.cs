using System;
using System.Collections.Generic;
using System.Text;

namespace NMediator.Http.Reflection.QueryStringBinder
{
    public interface IQueryStringBinder
    {
        bool CanBind(Type type, object value);
        IEnumerable<string> Bind(Type type, object value);
    }
}
