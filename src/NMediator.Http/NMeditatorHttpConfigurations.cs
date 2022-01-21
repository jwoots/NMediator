using NMediator.Http.Reflection.QueryStringBinder;
using System;
using System.Collections.Generic;
using System.Text;

namespace NMediator.Http
{
    public static class NMeditatorHttpConfigurations
    {
        public static List<IQueryStringBinder> Binders = new List<IQueryStringBinder>()
        {
            new StringConvertableBinder(),
            new EnumerableBinder(),
            new ArrayBinder()
        };
    }
}
