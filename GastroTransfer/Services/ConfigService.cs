using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GastroTransfer.Models;
using Newtonsoft.Json;

namespace GastroTransfer.Services
{
    class ConfigService : IConfigService
    {
        public string ConfigPath { get; protected set; }
        private ICryptoService cryptoService { get; set; }
        public string Message { get; protected set; }
        public List<Endpoint> Endpoints { get; protected set; }

        public ConfigService(ICryptoService cryptoService)
        {
            ConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "AJKSoftware", "GastroTransfer", "config.json");
            Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath));
            this.cryptoService = cryptoService;
            InitializeEndpoints();
        }

        public Config GetConfig()
        {
            try
            {
                using (StreamReader sr = new StreamReader(ConfigPath))
                {
                    var config = JsonConvert.DeserializeObject<Config>(sr.ReadToEnd());
                    config.Password = cryptoService.DecodePassword(config.Password);
                    return config;
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return null;
            }
        }

        public bool SaveConfig(Config config)
        {
            using (StreamWriter sw = new StreamWriter(ConfigPath, false, Encoding.UTF8))
            {
                try
                {
                    config.Password = cryptoService.EncodePassword(config.Password);
                    sw.Write(JsonConvert.SerializeObject(config, Formatting.Indented));
                    return true;
                }
                catch (Exception ex)
                {
                    Message = ex.Message;
                    return false;
                }
            }
        }

        public bool InitializeConfig()
        {
            using (StreamWriter sw = new StreamWriter(ConfigPath, false, Encoding.UTF8))
            {
                try
                {
                    sw.Write(JsonConvert.SerializeObject(new Config()
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
                    return false;
                }
            }
        }

        public void InitializeEndpoints()
        {
            string endpointsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "AJKSoftware", "GastroTransfer", "endpoints.json");
            Directory.CreateDirectory(Path.GetDirectoryName(endpointsPath));
            Endpoints = new List<Endpoint>();
            if (!File.Exists(endpointsPath))
            {
                Endpoints.Add(new Endpoint { Id = 1, Name = "Restauracja", Url = "http://192.168.71.70:8089/icws.asmx", Selected = true });
                Endpoints.Add(new Endpoint { Id = 2, Name = "Restauracja", Url = "http://192.168.81.70:8089/icws.asmx", Selected = false });
                using (StreamWriter sr = new StreamWriter(endpointsPath))
                {
                    sr.Write(JsonConvert.SerializeObject(Endpoints, Formatting.Indented));
                }
            }
            else
            {
                GetEndpoints(endpointsPath);
            }
        }

        private void GetEndpoints(string endpointsPath)
        {
            using (StreamReader sr = new StreamReader(endpointsPath))
            {
                Endpoints = JsonConvert.DeserializeObject<List<Endpoint>>(sr.ReadToEnd());
            }
        }
    }
}
