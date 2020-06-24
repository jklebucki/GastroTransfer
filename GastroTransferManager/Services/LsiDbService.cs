using GastroTransferManager.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace GastroTransferManager.Services
{
    public class LsiDbService
    {
        private CryptoService cryptoService { get; set; }
        private string ConfigPath { get; set; }
        public bool IsError { get; set; }
        public string Message { get; set; }

        public LsiDbService()
        {
            cryptoService = new CryptoService();
            ConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "AJKSoftware", "GastroTransfer", "LsiDbConfig.json");
        }

        public bool InitializeConfig()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath));
            if (File.Exists(ConfigPath))
                using (StreamWriter sw = new StreamWriter(ConfigPath, false, Encoding.UTF8))
                {
                    try
                    {
                        sw.Write(JsonConvert.SerializeObject(new LsiDbConfig()
                        {
                            ServerAddress = ".",
                            DatabaseName = "GastroTransfer",
                            UserName = "sa",
                            Password = cryptoService.EncodePassword("#Password123!"),
                            IsTrustedConnection = false
                        }, Formatting.Indented));
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Message = ex.Message;
                        IsError = true;
                        return false;
                    }
                }
            return true;
        }
    }
}
