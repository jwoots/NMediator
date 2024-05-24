using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NMediator.Http.Reflection.BodyConverter
{
    public interface IBodyConverter
    {
        string Convert(object objetToConvert);
        object ConvertToType(Type type, string body);
    }
}
