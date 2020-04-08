using System.Collections.Generic;

namespace LsiEndpointSupport.Models
{
    public class SystemInfo
    {
        public string EndpointUrl { get; set; }
        public string EndpointName { get; set; }
        public List<Warehouse> Warehouses { get; set; }
    }
}
