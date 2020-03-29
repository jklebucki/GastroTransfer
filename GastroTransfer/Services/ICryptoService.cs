namespace GastroTransfer.Services
{
    public interface ICryptoService
    {
        string EncodePassword(string plainPassword);
        string DecodePassword(string hashedPassword);
    }
}
