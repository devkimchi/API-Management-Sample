using System.Collections.Generic;
using System.Net;

using ApiManagementSample.Api.Filters;
using ApiManagementSample.Api.Responses;

using Microsoft.AspNet.Mvc;

using Swashbuckle.SwaggerGen.Annotations;

namespace ApiManagementSample.Api.Controllers
{
    [Route("values")]
    public class ValuesController : Controller
    {
        // GET: api/values
        [HttpGet]
        [Route("", Name = "GetValues")]
        [ApiAuthorizationFilter]
        [Produces(typeof(ValueResponseCollection))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ValueResponseCollection))]
        public IActionResult Get()
        {
            var results = new ValueResponseCollection()
                              {
                                  Items =
                                      new List<ValueResponse>()
                                          {
                                              new ValueResponse()
                                                  {
                                                      Value
                                                          =
                                                          "value1"
                                                  },
                                              new ValueResponse()
                                                  {
                                                      Value
                                                          =
                                                          "value2"
                                                  },
                                          }
                              };
            return this.Ok(results);
        }

        // GET api/values/5
        [HttpGet]
        [Route("{id}", Name = "GetValueById")]
        [Produces(typeof(ValueResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ValueResponse))]
        public IActionResult Get(int id)
        {
            var result = new ValueResponse() { Value = $"value{id}" };
            return this.Ok(result);
        }
    }
}