using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace NMediator.Http.Reflection.BodyConverter
{
    public interface IBodyConverter
    {
        HttpContent Convert(object objetToConvert);
    }
}
