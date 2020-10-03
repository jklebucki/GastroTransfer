using LinqToDB.Mapping;
using System;

namespace ProductionReportWF.DbSettings.Models
{
    [Table(Name = "[PSP].[Production]")]
    public class Production
    {
        [PrimaryKey, Identity]
        public int ProductionItemId { get; set; }
        [Column(Name = "ProducedItemId"), NotNull]
        public int ProducedItemId { get; set; }
        [Column(Name = "Quantity"), NotNull]
        public decimal Quantity { get; set; }
        [Column(Name = "TransferType"), NotNull]
        public int TransferType { get; set; }
        [Column(Name = "IsSentToExternalSystem"), NotNull]
        public bool IsSentToExternalSystem { get; set; }
        [Column(Name = "Registered"), NotNull]
        public DateTime Registered { get; set; }
        [Column(Name = "SentToExternalSystem"), NotNull]
        public DateTime SentToExternalSystem { get; set; }
        [Column(Name = "PackageNumber"), Nullable]
        public int? PackageNumber { get; set; }
        [Column(Name = "DocumentType"), Nullable]
        public int? DocumentType { get; set; }
    }
}
