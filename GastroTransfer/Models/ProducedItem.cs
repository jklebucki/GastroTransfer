using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GastroTransfer.Models
{
    /// <summary>
    /// Positions to create buttons
    /// </summary>
    class ProducedItem
    {
        public int ProducedItemId { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }
        public bool IsActive { get; set; }
        public string UnitOfMesure { get; set; }
        public decimal ConversionRate { get; set; }
        public int? ExternalId { get; set; }
        public string ExternalIndex { get; set; }
        public string ExternalName { get; set; }
        public string ExternalUnitOfMesure { get; set; }
        public int ProductGroupID { get; set; }
        public ProductGroup ProductGroup { get; set; }

    }
}
