using System.Collections.Generic;

namespace GastroTransfer.Models
{
    public class ProductGroup
    {
        public int ProductGroupId { get; set; }
        public string GroupName { get; set; }
        public List<ProducedItem> ProducedItems { get; set; }
    }
}
