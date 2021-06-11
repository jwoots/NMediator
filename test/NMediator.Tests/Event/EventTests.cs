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
        public async Task Publish_must_be_executed_correctly()
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

        [Fact]
        public async Task Publish_must_be_execute_multiple_handlers()
        {
            //ARRANGE
            var config = new MediatorConfiguration();
            var activator = new SimpleHandlerActivator();
            bool handlerCalled = false;
            bool handlerCalled2 = false;

            activator.RegisterMessage<MyEvent, Nothing>(request => {
                handlerCalled = true;
                return Task.FromResult(RequestResult.Success<Nothing>(new Nothing()));
            });

            activator.RegisterMessage<MyEvent, Nothing>(request => {
                handlerCalled2 = true;
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
            handlerCalled2.Should().BeTrue();
        }

        class MyEvent : IEvent
        {
            public string Name { get; set; }
        }
    }
}
