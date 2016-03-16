using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiManagementSample.Api.Responses
{
    public class ValueResponse
    {
        public string Value { get; set; }
    }

    public class ValueResponseCollection
    {
        public List<ValueResponse> Items { get; set; }
    }
}
