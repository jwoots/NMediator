using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NMediator.Http.Reflection.BodyConverter
{
    public class JsonBodyConverter : IBodyConverter
    {
        private readonly JsonSerializerOptions _options;

        public JsonBodyConverter(JsonSerializerOptions options = null)
        {
            _options = options ?? new JsonSerializerOptions(JsonSerializerDefaults.Web) 
            { 
                Converters = { new JsonStringEnumConverter() }
            };
        }
        public string Convert(object objetToConvert)
        {
            var content = JsonSerializer.Serialize(objetToConvert, _options);
            return content;
        }

        public object ConvertToType(Type type, string body)
        {
            return JsonSerializer.Deserialize(body, type, _options);
        }
    }
}
