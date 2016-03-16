using System;
using System.Net;

using ApiManagementSample.Api.Exceptions;
using ApiManagementSample.Api.Responses;

using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;

namespace ApiManagementSample.Api.Filters
{
    /// <summary>
    /// This represents the filter entity for global exceptions.
    /// </summary>
    public class GlobalExceptionFilter : IExceptionFilter
    {
        /// <summary>
        /// Performs while an exception arises.
        /// </summary>
        /// <param name="context"><see cref="ExceptionContext"/> instance.</param>
        public void OnException(ExceptionContext context)
        {
            var response = new ErrorResponse() { Message = context.Exception.Message };
#if DEBUG
            response.StackTrace = context.Exception.StackTrace;
#endif
            context.Result = new ObjectResult(response)
                                 {
                                     StatusCode = GetHttpStatusCode(context.Exception),
                                     DeclaredType = typeof(ErrorResponse)
                                 };
        }

        private static int GetHttpStatusCode(Exception ex)
        {
            if (ex is HttpResponseException)
            {
                return (int)(ex as HttpResponseException).HttpStatusCode;
            }

            return (int)HttpStatusCode.InternalServerError;
        }
    }
}