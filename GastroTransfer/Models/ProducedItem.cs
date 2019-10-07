
namespace GastroTransfer.Models
{
    /// <summary>
    /// Positions to create buttons
    /// </summary>
    public class ProducedItem
    {
        public int ProducedItemId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string UnitOfMesure { get; set; }
        public decimal ConversionRate { get; set; }
        public int? ExternalId { get; set; }
        public string ExternalIndex { get; set; }
        public string ExternalName { get; set; }
        public string ExternalUnitOfMesure { get; set; }
        public int ProductGroupId { get; set; }
    }
}
