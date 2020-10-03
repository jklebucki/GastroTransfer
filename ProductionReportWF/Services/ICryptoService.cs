namespace ProductionReportWF.Services
{
    interface ICryptoService
    {
        string Encrypt(string plainPassword);
        string Decrypt(string encryptedPassword);
    }
}
