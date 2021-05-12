using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace NMediator.Core.Context
{
    [Serializable]
    public class MessageContext
    {
        const string KEY_ID = "id-message";
        const string KEY_DATE = "date-message";

        private static AsyncLocal<MessageContext> _local = new AsyncLocal<MessageContext>();
      
        public static MessageContext Current => _local.Value ??= new MessageContext();
      
        public static void SetContext(MessageContext context)
        {
            _local.Value = context;
        }

        private IDictionary<string, string> _values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public string Id  => _values[KEY_ID];
        public DateTimeOffset Date => DateTimeOffset.Parse(_values[KEY_DATE]);

        public IDictionary<string, string> Values => _values;

        public MessageContext()
        {
            _values[KEY_ID] = Guid.NewGuid().ToString();
            _values[KEY_DATE] = DateTimeOffset.Now.ToString("O");
        }
    }
}
