
namespace GastroTransfer.Models
{
    public class Config
    {
        public string ServerAddress { get; set; }
        public string DatabaseName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsTrustedConnection { get; set; }
        public string AdditionalConnectionStringDirective { get; set; }
        public bool WeightComIsConnected { get; set; }
        public string WeightComPortName { get; set; }
        public int WeightComBaudRate { get; set; }
        public int WeightComStopBits { get; set; }
        public int WeightComDataBits { get; set; }
        public int WeightComParity { get; set; }
        public int EndpointId { get; set; }
    }
}
