using System;
using System.Net.Mime;
using System.Text;

namespace NMediator.Http.BodyConverter
{
    public interface IBodyConverter
    {
        string Convert(object objetToConvert);
        object? ConvertToType(Type type, string body);
        T? ConvertToType<T>(string body);
        Encoding Encoding { get; }
        string ContentType { get; }
        }
}
