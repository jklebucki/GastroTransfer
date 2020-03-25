using System;

namespace GastroTransfer.Models
{
    public class ProductionItem
    {
        public int ProductionItemId { get; set; }
        public int ProducedItemId { get; set; }
        public decimal Quantity { get; set; }
        public int TransferType { get; set; }
        public bool IsSentToExternalSystem { get; set; }
        public DateTime Registered { get; set; }
        public DateTime SentToExternalSystem { get; set; }
        public int? PackageNumber { get; set; }
        public int? DocumentType { get; set; }
    }
}
