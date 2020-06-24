namespace GastroTransferManager.Models
{
    class LsiDbConfig
    {
        public string ServerAddress { get; set; }
        public string DatabaseName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsTrustedConnection { get; set; }
        public string AdditionalConnectionStringDirective { get; set; }
    }
}
