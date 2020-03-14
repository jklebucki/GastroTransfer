using System;
using System.Collections.Generic;
using System.Text;

namespace LsiEndpointSupport.Models
{
    class Endpoint
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public bool Selected { get; set; }
    }
}
