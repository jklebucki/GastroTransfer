using GastroTransfer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;

namespace GastroTransfer.Services
{
    /// <summary>
    /// App config service
    /// </summary>
    public class ConfigService : IConfigService
    {
        public string ConfigPath { get; protected set; }
        private ICryptoService cryptoService { get; set; }
        public string Message { get; protected set; }
        public List<Endpoint> Endpoints { get; protected set; }

        /// <summary>
        /// ConfigService constructor  
        /// </summary>
        /// <param name="cryptoService">Encryption service</param>
        public ConfigService(ICryptoService cryptoService)
        {
            CreateConfigDirectory();
            this.cryptoService = cryptoService;
        }

        /// <summary>
        /// Creating config directory at first startup
        /// </summary>
        private void CreateConfigDirectory()
        {
            ConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "AJKSoftware", "GastroTransfer", "config.json");
            if (!Directory.Exists(Path.GetDirectoryName(ConfigPath)))
            {
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath));
                    DirectoryInfo directory = new DirectoryInfo(Path.GetDirectoryName(ConfigPath));
                    DirectorySecurity security = directory.GetAccessControl();
                    var allDomainUsers = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
                    var authenticatedUser = new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null);
                    var allUsers = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
                    security.AddAccessRule(new FileSystemAccessRule(allUsers, FileSystemRights.FullControl,
                                    InheritanceFlags.None,
                                    PropagationFlags.NoPropagateInherit,
                                    AccessControlType.Allow));
                    security.AddAccessRule(new FileSystemAccessRule(authenticatedUser, FileSystemRights.FullControl,
                                    InheritanceFlags.ObjectInherit,
                                    PropagationFlags.InheritOnly,
                                    AccessControlType.Allow));
                    security.AddAccessRule(new FileSystemAccessRule(allDomainUsers, FileSystemRights.FullControl,
                                    InheritanceFlags.None,
                                    PropagationFlags.NoPropagateInherit,
                                    AccessControlType.Allow));
                    directory.SetAccessControl(security);
                }
                catch (Exception ex)
                {
                    Message = $"Error 100:\n {ex.Message}";
                }
            }
        }

        /// <summary>
        /// Reading config object from file
        /// </summary>
        /// <returns></returns>
        public Config GetConfig()
        {
            try
            {
                using (StreamReader sr = new StreamReader(ConfigPath))
                {
                    var config = JsonConvert.DeserializeObject<Config>(sr.ReadToEnd());
                    config.Password = cryptoService.DecodePassword(config.Password);
                    config.SystemPassword = cryptoService.DecodePassword(config.SystemPassword);
                    config.ExternalDbPassword = cryptoService.DecodePassword(config.ExternalDbPassword);
                    config.ProductionPassword = cryptoService.DecodePassword(config.ProductionPassword);
                    return config;
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return null;
            }
        }
        /// <summary>
        /// Saving serialized config object to json file
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public bool SaveConfig(Config config)
        {
            using (StreamWriter sw = new StreamWriter(ConfigPath, false, Encoding.UTF8))
            {
                try
                {
                    config.Password = cryptoService.EncodePassword(config.Password);
                    config.SystemPassword = cryptoService.EncodePassword(config.SystemPassword);
                    config.ExternalDbPassword = cryptoService.EncodePassword(config.ExternalDbPassword);
                    config.ProductionPassword = cryptoService.EncodePassword(config.ProductionPassword);
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

        /// <summary>
        /// Initialize config with default values
        /// </summary>
        /// <returns></returns>
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
                        SystemPassword = cryptoService.EncodePassword("admin"),
                        IsTrustedConnection = false,
                        ExternalDbPassword = cryptoService.EncodePassword("#Password123!"),
                        WeightComBaudRate = 9600,
                        WeightComDataBits = 8,
                        WeightComStopBits = (int)(System.IO.Ports.StopBits.One),
                        WeightComParity = (int)(System.IO.Ports.Parity.None),
                        OnPasswordProduction = true,
                        OnPasswordProductsImport = true
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


        /// <summary>
        /// Getting endpoints list from endpoints.json file  
        /// </summary>
        /// <param name="endpointsPath"></param>
        private void GetEndpoints(string endpointsPath)
        {
            using (StreamReader sr = new StreamReader(endpointsPath))
            {
                Endpoints = JsonConvert.DeserializeObject<List<Endpoint>>(sr.ReadToEnd());
            }
        }
    }
}
