using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace NMediator.Context
{
    [Serializable]
    public class MessageContext
    {
        private static AsyncLocal<MessageContext> _local = new AsyncLocal<MessageContext>();
      
        public static MessageContext Current => _local.Value ??= new MessageContext();
      
        public static void SetContext(MessageContext context)
        {
            _local.Value = context;
        }

        private IDictionary<string, string> _values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public string Id { get; set; }
        public DateTimeOffset Date { get; set; }
        public IDictionary<string, string> Values => _values;

        public MessageContext()
        {
            Id = Guid.NewGuid().ToString();
            Date = DateTimeOffset.Now;
        }
    }
}
