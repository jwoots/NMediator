using NMediator.Core.SInjector;
using NMediator.Core.Transport;
using NMediator.NMediator.Http.Reflection;
using System.Web;

namespace NMediator.AspnetCore
{
    public static class EndpointRouteBuilderExtensions
    {
        public static IApplicationBuilder MapMessages(this WebApplication builder, Container container, HttpDescriptors descriptors, ErrorToHttpResponseMapper errorMapper)
        {
            var handlerExecutor = container.Get<ITransportLevelHandlerExecutor>();

            foreach (var type in descriptors.GetRegisteredTypes())
            {
                var descriptor = descriptors.GetFor(type);
                var message = Activator.CreateInstance(type);

                builder.MapMethods(descriptor.RelativeUri, new[] { descriptor.Method.ToString() }, async ctx =>
                {
                    //build from body
                    using (var stream = new StreamReader(ctx.Request.Body))
                    {
                        var body = stream.ReadToEnd();
                        if(!string.IsNullOrEmpty(body))
                            message = descriptor.CreateMessageWithBody(type, body, descriptors.BodyConverter);
                    }

                    //build from query string
                    var parsedQueryString = HttpUtility.ParseQueryString(ctx.Request.QueryString.ToString());
                    descriptor.PopulateMessageWithQueryString(parsedQueryString, message, descriptors.QueryParametersBinders);

                    //execute handler
                    var mediatorHeaders = new Dictionary<string, string>();
                    ctx.Request.Headers.ForEach(header =>
                    {
                        mediatorHeaders.Add(header.Key, header.Value);
                    });

                    //result
                    try
                    {
                        var result = await handlerExecutor.ExecuteHandler(message, ctx.RequestAborted, mediatorHeaders);

                        if (result.IsSuccess)
                        {
                            var content = descriptors.BodyConverter.Convert(result.Data);
                      
                            ctx.Response.StatusCode = 200;
                            ctx.Response.ContentType = "application/json";
                            await ctx.Response.WriteAsync(content, ctx.RequestAborted);
                        }
                        else
                        {
                            var httpResponse = errorMapper.GetResponseFromErrorOrDefault(result.Error);
                            
                            ctx.Response.StatusCode = httpResponse.StatusCode;
                            ctx.Response.ContentType = "application/json";
                            await ctx.Response.WriteAsync(httpResponse.Body, ctx.RequestAborted);
                        }
                    }
                    catch (Exception ex)
                    {
                        var httpResponse = errorMapper.GetResponseFromExceptionOrDefault(ex);
                        await ctx.Response.WriteAsync(httpResponse.Body, ctx.RequestAborted);
                        ctx.Response.StatusCode = httpResponse.StatusCode;
                        ctx.Response.ContentType = "application/json";
                    }
                });
            }
            return builder;
        }
    }
}