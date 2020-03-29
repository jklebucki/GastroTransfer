namespace GastroTransfer.Services
{
    interface IDbService
    {
        bool CheckConnection();
        bool CheckLsiConnection();
        string GetConnectionString(int timeout);
        string GetLsiConnectionString(int timeout);
    }
}
