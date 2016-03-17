using System.Net;

using ApiManagementSample.Api.Exceptions;

using Microsoft.AspNet.Mvc.Filters;

namespace ApiManagementSample.Api.Filters
{
    /// <summary>
    /// This represents the filter attribute entity for global actions.
    /// </summary>
    public class GlobalActionFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Called while an action is being executed.
        /// </summary>
        /// <param name="context"><see cref="ActionExecutingContext"/> instance.</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            //if (!context.HttpContext.Request.IsHttps)
            //{
            //    throw new HttpResponseException(HttpStatusCode.BadRequest, "HTTPS Connection Required");
            //}
        }
    }
}