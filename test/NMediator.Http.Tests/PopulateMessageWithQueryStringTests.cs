using FluentAssertions;
using NMediator.NMediator.Http.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Xunit;

namespace NMediator.Http.Tests
{
    public class PopulateMessageWithQueryStringTests
    {
        private readonly HttpDescriptors descriptors = new HttpDescriptors();

        public PopulateMessageWithQueryStringTests()
        {
            descriptors.AddFor<Message>(b =>
            {
                b.CallRelativeUri("/test", HttpMethod.Get, ParameterLocation.QUERY_STRING);
            });
        }

        [Fact]
        public void PopulateMessageWithQueryString_must_populate_bool()
        {
            var message = PopulateNewMessage("?booleanproperty=true");
            message.BooleanProperty.Should().BeTrue();
        }

        [Fact]
        public void PopulateMessageWithQueryString_must_populate_int()
        {
            var message = PopulateNewMessage("?Intproperty=18");
            message.IntProperty.Should().Be(18);
        }

        [Fact]
        public void PopulateMessageWithQueryString_must_populate_decimal()
        {
            var message = PopulateNewMessage("?decimalProperty=208.5");
            message.DecimalProperty.Should().Be(208.5m);
        }

        [Fact]
        public void PopulateMessageWithQueryString_must_populate_array_from_multiparameter_style()
        {
            var message = PopulateNewMessage("?longarrayproperty=999&longarrayproperty=1000&longarrayproperty=1001");
            message.LongArrayProperty.Should().BeEquivalentTo(new long[] { 999, 1000, 1001 });
        }

        [Fact]
        public void PopulateMessageWithQueryString_must_populate_array_from_singleparameter_style()
        {
            var message = PopulateNewMessage("?longarrayproperty=999,1000 , 1001");
            message.LongArrayProperty.Should().BeEquivalentTo(new long[] { 999, 1000, 1001 });
        }

        [Fact]
        public void PopulateMessageWithQueryString_must_populate_enumerable_from_multiparameter_style()
        {
            var message = PopulateNewMessage("?floatEnumerableProperty=9.1&floatEnumerableProperty=9.2&floatEnumerableProperty=9.3");
            message.FloatEnumerableProperty.Should().BeEquivalentTo(new float[] { 9.1F, 9.2F, 9.3F });
        }

        [Fact]
        public void PopulateMessageWithQueryString_must_populate_enumerable_from_singleparameter_style()
        {
            var message = PopulateNewMessage("?floatEnumerableProperty=9.1, 9.2 , 9.3");
            message.FloatEnumerableProperty.Should().BeEquivalentTo(new float[] { 9.1F, 9.2F, 9.3F });
        }

        [Fact]
        public void PopulateMessageWithQueryString_must_populate_enum()
        {
            var message = PopulateNewMessage("?enumProperty=enum_3");
            message.EnumProperty.Should().Be(MessageEnum.ENUM_3);
        }

        [Fact]
        public void PopulateMessageWithQueryString_must_populate_datetime()
        {
            var message = PopulateNewMessage("?datetimeProperty=2024-06-05T00:00:00");
            message.DateTimeProperty.Should().Be(new DateTime(2024,6,5));
        }

        [Fact]
        public void PopulateMessageWithQueryString_must_populate_datetimeoffset()
        {
            var message = PopulateNewMessage("?datetimeoffsetProperty=2024-06-05T13:08:42%2B02:00");
            message.DateTimeOffsetProperty.Should().Be(new DateTimeOffset(2024, 6, 5,13,08,42,TimeSpan.FromHours(2)));
        }

        private Message PopulateNewMessage(string querystring)
        {
            var message = new Message();
            var descriptor = descriptors.GetFor(typeof(Message));

            descriptor.PopulateMessageWithQueryString(
                HttpUtility.ParseQueryString(querystring),
                message,
                descriptors.QueryParametersBinders
            );

            return message;
        }

        class Message
        {
            public bool BooleanProperty { get; set; }
            public int IntProperty { get; set; }
            public decimal DecimalProperty { get; set; }
            public long[] LongArrayProperty { get; set; }
            public IEnumerable<float> FloatEnumerableProperty { get; set; }
            public MessageEnum EnumProperty { get; set; }
            public DateTime DateTimeProperty { get; set; }
            public DateTimeOffset DateTimeOffsetProperty { get; set; }
        }

        enum MessageEnum
        {
            ENUM_1, ENUM_2, ENUM_3
        }
    }
}
