using System.Data.SqlClient;

namespace ProductionReportWF.Models
{
    public class SystemSettings
    {
        public string DatabaseAddress { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string DatabaseName { get; set; }

        public string ConnStr()
        {
            var cs = new SqlConnectionStringBuilder();
            cs.DataSource = DatabaseAddress;
            cs.UserID = UserName;
            cs.Password = UserPassword;
            cs.InitialCatalog = DatabaseName;
            return cs.ConnectionString;
        }
    }
}
