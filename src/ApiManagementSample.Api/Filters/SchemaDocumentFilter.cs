using System.Collections.Generic;

using Swashbuckle.SwaggerGen;

namespace ApiManagementSample.Api.Filters
{
    /// <summary>
    /// This represents the document filter entity for Swagger schema.
    /// </summary>
    public class SchemaDocumentFilter : IDocumentFilter
    {
        /// <summary>
        /// Applies custom document properties based on context.
        /// </summary>
        /// <param name="swaggerDoc"><see cref="SwaggerDocument"/> instance.</param>
        /// <param name="context"><see cref="DocumentFilterContext"/> instance.</param>
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.Host = "localhost:44304";
#if !DEBUG
            swaggerDoc.Host = "ase-dev-api-demo.azurewebsites.net";
#endif
            swaggerDoc.BasePath = "/";
            swaggerDoc.Schemes = new List<string>() { "https" };
        }
    }
}