using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GastroTransfer.Services
{
    class DbService : IDbService
    {
        public bool CheckConnection()
        {
            try
            {
                return true;
            } catch
            {
                return false;
            }
        }
    }
}
