
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
        public string ExternalDbServerAddress { get; set; }
        public string ExternalDbDatabaseName { get; set; }
        public string ExternalDbUserName { get; set; }
        public string ExternalDbPassword { get; set; }
        public bool ExternalDbIsTrustedConnection { get; set; }
        public string ExternalDbAdditionalConnectionStringDirective { get; set; }
        public bool WeightComIsConnected { get; set; }
        public string WeightComPortName { get; set; }
        public int WeightComBaudRate { get; set; }
        public int WeightComStopBits { get; set; }
        public int WeightComDataBits { get; set; }
        public int WeightComParity { get; set; }
        public string EndpointUrl { get; set; }
        public string WarehouseSymbol { get; set; }
        public string ProductionDocumentSymbol { get; set; }
        public string SystemPassword { get; set; }
        public bool OnPasswordProduction { get; set; }
        public bool OnPasswordProductsImport { get; set; }
    }
}
