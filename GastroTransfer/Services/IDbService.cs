using GastroTransfer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GastroTransfer.Services
{
    interface IDbService
    {
        bool CheckConnection();
        string GetConnectionString(int timeout);

    }
}
