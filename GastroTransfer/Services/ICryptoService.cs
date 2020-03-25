using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GastroTransfer.Services
{
    public interface ICryptoService
    {
        string EncodePassword(string plainPassword);
        string DecodePassword(string hashedPassword);
    }
}
