
using GastroTransfer.Models;

namespace GastroTransfer.Services

{
    interface IConfigService
    {
        bool SaveConfig(Config config);
        Config GetConfig();
        bool InitializeConfig();
    }
}
