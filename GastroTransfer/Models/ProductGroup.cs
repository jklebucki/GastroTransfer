namespace GastroTransfer.Models
{
    public class ProductGroup
    {
        public int ProductGroupId { get; set; }
        public int ExternalGroupId { get; set; }
        public string GroupName { get; set; }
        public bool IsActive { get; set; }
    }
}
