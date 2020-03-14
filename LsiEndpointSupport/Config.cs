using LsiEndpointSupport.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LsiEndpointSupport
{
    class Config
    {
        public List<Endpoint> Endpoints { get; protected set; }
        public Config()
        {
            InitializeEndpoints();
        }

        public void InitializeEndpoints()
        {
            string endpointsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "AJKSoftware", "GastroTransfer", "endpoints.json");
            Directory.CreateDirectory(Path.GetDirectoryName(endpointsPath));
            Endpoints = new List<Endpoint>();
            if (!File.Exists(endpointsPath))
            {
                Endpoints.Add(new Endpoint { Id = 1, Name = "Restauracja", Url = "http://192.168.71.70:8089/icws.asmx", Selected = false });
                Endpoints.Add(new Endpoint { Id = 2, Name = "Restauracja", Url = "http://192.168.81.70:8089/icws.asmx", Selected = true });
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
