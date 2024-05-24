using NMediator.Core.Result;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Text;

namespace NMediator.Http
{
    public class HttpResponseToErrorMapper
    {
        private readonly IDictionary<HttpStatusCode, Func<HttpResponseMessage, Error>> _errorfactories = new Dictionary<HttpStatusCode, Func<HttpResponseMessage, Error>>();
        private readonly IDictionary<HttpStatusCode, Func<HttpResponseMessage, Exception>> _exceptionfactories = new Dictionary<HttpStatusCode, Func<HttpResponseMessage, Exception>>();
       

        public HttpResponseToErrorMapper()
        {
            
        }

        /// <summary>
        /// Add a factory to provide a request result Error from HttpResponseMessage for a http status code
        /// </summary>
        /// <param name="httpStatusCode">the status code for which apply the factory</param>
        /// <param name="factory">the factory de create RequestResult Error from Http response message</param>
        public void AddErrorFactory(HttpStatusCode httpStatusCode, Func<HttpResponseMessage, Error> factory)
        {
            _errorfactories[httpStatusCode] = factory;
        }

        /// <summary>
        /// Add a factory to create an exception from HttpResponseMessage for a http status code
        /// </summary>
        /// <param name="httpStatusCode">the status code for which apply the factory</param>
        /// <param name="factory">the factory to create Exception from http response message</param>
        public void AddExceptionFactory(HttpStatusCode httpStatusCode, Func<HttpResponseMessage, Exception> factory)
        {
            _exceptionfactories[httpStatusCode] = factory;
        }

       

        /// <summary>
        /// Try to get error corresponding to HttpResponseMessage
        /// </summary>
        /// <param name="message"></param>
        /// <param name="error"></param>
        /// <returns>true if corresponding error found</returns>
        public bool TryGetError(HttpResponseMessage message, out Error error)
        {
            if(_errorfactories.TryGetValue(message.StatusCode, out var errorFactory))
            {
                error = errorFactory(message);
                return true;
            }

            error = null;
            return false;
        }

        /// <summary>
        /// Try to get exception corresponding to HttpResponseMessage
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public bool TryGetException(HttpResponseMessage message, out Exception exception)
        {
            if(_exceptionfactories.TryGetValue(message.StatusCode, out var exceptionFactory))
            {
                exception = exceptionFactory(message);
                return true;
            }

            exception = null;
            return false;
        }
    }
}
