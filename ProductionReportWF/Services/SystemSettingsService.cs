using Newtonsoft.Json;
using ProductionReportWF.Models;
using System;
using System.IO;

namespace ProductionReportWF.Services
{
    class SystemSettingsService
    {
        private string SystemConfigPath { get; set; }
        private ICryptoService _cryptoService { get; set; }
        public ErrorMessage Message { get; protected set; }
        public SystemSettingsService(ICryptoService cryptoService)
        {
            SystemConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "SystemConfig.json");
            _cryptoService = cryptoService;
            InitializeSettings();
            Message = new ErrorMessage { IsError = false, Message = "" };
        }

        public void InitializeSettings()
        {
            if (!File.Exists(SystemConfigPath))
            {
                try
                {
                    using (StreamWriter sr = new StreamWriter(SystemConfigPath))
                    {
                        SystemSettings settings = new SystemSettings
                        {
                            DatabaseAddress = ".",
                            DatabaseName = "GastroTransfer",
                            UserName = "sa",
                            UserPassword = _cryptoService.Encrypt("l_2009_Citronex")
                        };
                        sr.Write(JsonConvert.SerializeObject(settings, Formatting.Indented));
                    }
                }
                catch (Exception ex)
                {
                    Message.IsError = true;
                    Message.Message = ex.Message;
                }
            }
        }

        public SystemSettings GetSystemSettings()
        {
            try
            {
                using (StreamReader sr = new StreamReader(SystemConfigPath))
                {
                    var systemSettings = JsonConvert.DeserializeObject<SystemSettings>(sr.ReadToEnd());
                    systemSettings.UserPassword = _cryptoService.Decrypt(systemSettings.UserPassword);
                    return systemSettings;
                }
            }
            catch (Exception ex)
            {
                Message.IsError = true;
                Message.Message = ex.Message;
            }
            return null;
        }

        public void SetSystemSettings(SystemSettings systemSettings)
        {
            try
            {
                using (StreamWriter sr = new StreamWriter(SystemConfigPath))
                {
                    systemSettings.UserPassword = _cryptoService.Encrypt(systemSettings.UserPassword);
                    sr.Write(JsonConvert.SerializeObject(systemSettings, Formatting.Indented));
                }
            }
            catch (Exception ex)
            {
                Message.IsError = true;
                Message.Message = ex.Message;
            }
        }
    }
}
