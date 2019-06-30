﻿using System;
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
        public ConfigService(ICryptoService cryptoService)
        {
            ConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "AJKSoftware", "GastroTransfer", "config.json");
            Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath));
            this.cryptoService = cryptoService;
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
            catch
            {
                return null;
            }

        }

        public bool SaveConfig(Config config)
        {
            using (StreamWriter sw = new StreamWriter(ConfigPath, false, Encoding.UTF8))
            {
                try
                {
                    sw.Write(JsonConvert.SerializeObject(config, Formatting.Indented));
                    return true;
                }
                catch
                {
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
                        Password = cryptoService.EncodePassword("#sa2015!"),
                        IsTrustedConnection = false
                    }, Formatting.Indented));
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