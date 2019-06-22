using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GastroTransfer.Models
{
    class TransferredItem
    {
        public int TransferredItemId { get; set; }
        public decimal Quantity { get; set; }
        public int TransferType { get; set; }
        public bool IsSentToExternalSystem { get; set; }
        public DateTime Registered { get; set; }
        public DateTime SentToExternalSystem { get; set; }
        public int? PackageNumber { get; set; }
    }
}
