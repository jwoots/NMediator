using FluentAssertions;
using NMediator.Core.Activator;
using NMediator.Core.Configuration;
using NMediator.Core.Result;
using NMediator.Event;
using System.Threading.Tasks;
using Xunit;

namespace NMediator.Tests.Event
{
    public class EventTests
    {
        [Fact]
        public async Task Event_must_be_executed_correctly()
        {
            //ARRANGE
            var config = new MediatorConfiguration();
            var activator = new SimpleHandlerActivator();
            bool handlerCalled = false;

            activator.RegisterMessage<MyEvent, Nothing>(request => { 
                handlerCalled = true; 
                return Task.FromResult(RequestResult.Success<Nothing>(new Nothing())); 
            });

            config.WithActivator(activator)
                    .Event(e => e.PublishWithInProcess(typeof(MyEvent)));

            BaseConfiguration.Configure(config);

            //ACT
            var processor = config.Container.Get<IEventPublisher>();
            var result = await processor.Publish(new MyEvent() { Name = "jwoots" });

            //ASSERT
            result.Data.Should().BeOfType<Nothing>();
            handlerCalled.Should().BeTrue();
        }

        class MyEvent : IEvent
        {
            public string Name { get; set; }
        }
    }
}
