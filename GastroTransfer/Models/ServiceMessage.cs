namespace GastroTransfer.Models
{
    class ServiceMessage
    {
        public int ItemId { get; set; }
        public string Message { get; set; }
        public bool IsError { get; set; }
    }
}
