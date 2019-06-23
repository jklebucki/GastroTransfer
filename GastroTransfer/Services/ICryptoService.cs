using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GastroTransfer.Services
{
    interface ICryptoService
    {
        string EncodePassword(string plainPassword);
        string DecodePassword(string hashedPassword);
    }
}
