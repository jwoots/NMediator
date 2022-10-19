using FluentAssertions;
using NMediator.NMediator.Http;
using NMediator.NMediator.Http.Reflection;
using System;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using Xunit;

namespace NMediator.Http.Tests
{
    public class HttpDescriptorBuilderTests
    {
        public HttpDescriptorBuilderTests()
        {
        }

        [Fact]
        public void TestOverride()
        {
            HttpDescriptor descriptor = new HttpDescriptor();
            Expression<Func<HttpDescriptor,object>> exp = d => d.Method;
            Expression<Func<HttpDescriptor,object>> exp2 = d => d.Method;
            
            Console.WriteLine(((MemberExpression)exp.Body).Member);
        }
    }
}