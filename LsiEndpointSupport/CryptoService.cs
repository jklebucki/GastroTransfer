using System;
using System.Collections.Generic;
using System.Text;

namespace LsiEndpointSupport
{
    public class CryptoService
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
