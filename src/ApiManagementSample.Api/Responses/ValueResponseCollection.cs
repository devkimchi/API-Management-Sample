using System.Collections.Generic;

namespace ApiManagementSample.Api.Responses
{
    /// <summary>
    /// This represents the response entity for value collection.
    /// </summary>
    public class ValueResponseCollection
    {
        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        public List<ValueResponse> Items { get; set; }
    }
}