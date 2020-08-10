using FluentAssertions;
using NMediator.Result;
using Xunit;

namespace NMediator.Tests.Result
{
    public class RequestResultTests
    {
        [Fact]
        public void RequestResult_must_init_IsSuccess_to_true_when_passing_data()
        {
            //ACT
            var rr = new RequestResult<string>("data");

            //ASSERT
            rr.IsSuccess.Should().Be(true);
            rr.Data.Should().Be("data");
            rr.Error.Should().BeNull();
        }

        [Fact]
        public void RequestResult_must_init_IsSuccess_to_false_when_passing_error()
        {
            //ARRANGE
            var expectedError = new Error();

            //ACT
            var rr = new RequestResult<string>(expectedError);

            //ASSERT
            rr.IsSuccess.Should().Be(false);
            rr.Error.Should().Be(expectedError);
            rr.Data.Should().BeNull();
        }

        [Fact]
        public void Success_must_return_a_success_request_result()
        {
            //ACT
            var rr = RequestResult.Success<string>("data");

            //ASSERT
            rr.IsSuccess.Should().Be(true);
            rr.Data.Should().Be("data");
            rr.Error.Should().BeNull();
        }

        [Fact]
        public void RequestResult_must_return_a_fail_request_when_passing_error()
        {
            //ARRANGE
            var expectedError = new Error();

            //ACT
            var rr = RequestResult.Fail<string>(expectedError);

            //ASSERT
            rr.IsSuccess.Should().Be(false);
            rr.Error.Should().Be(expectedError);
            rr.Data.Should().BeNull();
        }
    }
}
