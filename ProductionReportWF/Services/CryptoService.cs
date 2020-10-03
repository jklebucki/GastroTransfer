using System;
using System.Text;

namespace ProductionReportWF.Services
{
    class CryptoService : ICryptoService
    {
        public string Decrypt(string encryptedPassword)
        {
            var decryptedByteArray = Convert.FromBase64String(encryptedPassword);
            return Encoding.UTF8.GetString(decryptedByteArray);
        }

        public string Encrypt(string plainPassword)
        {
            var pass = Encoding.UTF8.GetBytes(plainPassword);
            return Convert.ToBase64String(pass);
        }
    }
}
