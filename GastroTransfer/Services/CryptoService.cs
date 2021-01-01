using System;
using System.Text;

namespace GastroTransfer.Services
{
    public class CryptoService : ICryptoService
    {
        public string EncodePassword(string plainPassword)
        {
            try
            {
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainPassword));
            }
            catch
            {
                return null;
            }

        }
        public string DecodePassword(string hashedPassword)
        {
            try
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(hashedPassword));
            }
            catch
            {
                return null;
            }

        }
    }
}
