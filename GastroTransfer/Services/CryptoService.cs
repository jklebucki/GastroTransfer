using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GastroTransfer.Services
{
    public class CryptoService : ICryptoService
    {
        public string EncodePassword(string plainPassword)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainPassword));
        }
        public string DecodePassword(string hashedPassword)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(hashedPassword));
        }
    }
}
