using LinqToDB.Mapping;
namespace ProductionReportWF.DbSettings.Models
{
    [Table(Name = "[PSP].[ProducedItems]")]
    public class Product
    {
        [PrimaryKey, Identity]
        public int ProducedItemId { get; set; }

        [Column(Name = "Name"), NotNull]
        public string Name { get; set; }
        [Column(Name = "IsActive"), NotNull]
        public bool IsActive { get; set; }
        [Column(Name = "UnitOfMesure"), NotNull]
        public string UnitOfMesure { get; set; }
        [Column(Name = "ConversionRate"), NotNull]
        public decimal ConversionRate { get; set; }
        [Column(Name = "ExternalId"), NotNull]
        public string ExternalId { get; set; }
        [Column(Name = "ExternalIndex"), NotNull]
        public string ExternalIndex { get; set; }
        [Column(Name = "ExternalName"), NotNull]
        public string ExternalName { get; set; }
        [Column(Name = "ExternalUnitOfMesure"), NotNull]
        public string ExternalUnitOfMesure { get; set; }
        [Column(Name = "ExternalGroupId"), NotNull]
        public int ExternalGroupId { get; set; }
    }
}