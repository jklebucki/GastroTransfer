namespace GastroTransfer.Models
{
    public class ServiceMessage
    {
        public int ItemId { get; set; }
        public string Message { get; set; }
        public bool IsError { get; set; }
    }
}
