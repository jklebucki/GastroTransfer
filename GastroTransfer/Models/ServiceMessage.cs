using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GastroTransfer.Models
{
    class ServiceMessage
    {
        public int ItemId { get; set; }
        public string Message { get; set; }
        public bool IsError { get; set; }
    }
}
