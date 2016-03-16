using System;
using System.Net;

using ApiManagementSample.Api.Exceptions;
using ApiManagementSample.Api.Responses;

using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace ApiManagementSample.Api.Filters
{
    /// <summary>
    /// This represents the filter attribute entity for API authorisation.
    /// </summary>
    public class ApiAuthorizationFilterAttribute : AuthorizationFilterAttribute
    {
        /// <summary>
        /// Called while performing authorisation.
        /// </summary>
        /// <param name="context"><see cref="Microsoft.AspNet.Mvc.Filters.AuthorizationContext"/> instance.</param>
        public override void OnAuthorization(AuthorizationContext context)
        {
            base.OnAuthorization(context);

            StringValues value;
            var header = context.HttpContext.Request.Headers.TryGetValue("Secret", out value) ? value : StringValues.Empty;
            try
            {
                if (string.IsNullOrWhiteSpace(header.ToString()))
                {
                    throw new HttpResponseException(HttpStatusCode.Forbidden, "Forbidden");
                }

                if (header.ToString() != "Pa$$W0rd")
                {
                    throw new HttpResponseException(HttpStatusCode.Forbidden, "Forbidden");
                }
            }
            catch (HttpResponseException ex)
            {
                var response = new ErrorResponse() { Message = ex.Message };
#if DEBUG
                response.StackTrace = ex.StackTrace;
#endif
                context.Result = new ObjectResult(response)
                                     {
                                         StatusCode = (int)ex.HttpStatusCode,
                                         DeclaredType = typeof(ErrorResponse)
                                     };
            }
        }
    }
}