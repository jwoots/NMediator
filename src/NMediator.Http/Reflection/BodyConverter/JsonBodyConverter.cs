using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NMediator.Http.Reflection.BodyConverter
{
    public class JsonBodyConverter : IBodyConverter
    {
        private readonly JsonSerializerOptions _options;

        public JsonBodyConverter(JsonSerializerOptions options = null)
        {
            _options = options ?? new JsonSerializerOptions() 
            { 
                Converters = { new JsonStringEnumConverter() }
            };
        }
        public HttpContent Convert(object objetToConvert)
        {
            var content = JsonSerializer.Serialize(objetToConvert, _options);
            return new StringContent(content,Encoding.UTF8,"application/json");
        }
    }
}
