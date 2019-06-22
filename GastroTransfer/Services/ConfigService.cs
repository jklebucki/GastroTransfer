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

        public ConfigService()
        {
            ConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "AJKSoftware", "GastroTransfer", "config.json");
            Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath));
        }

        public Config GetConfig()
        {
            return null;
        }

        public bool SaveConfig(Config config)
        {
            return true;
        }

        public bool InitializeConfig()
        {
            using (StreamWriter sr = new StreamWriter(ConfigPath))
            {
                try
                {
                    sr.Write(JsonConvert.SerializeObject(new Config(), Formatting.Indented));
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
