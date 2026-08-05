using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace AgronicaDataProvider6.Models
{
    public class DataProvider6Settings
    {
        public string ConnectionString { get; set; }
        public DbConnection? Connection { get; set; } = null;
        public DbTransaction? Transaciton { get; set; } = null;
        public int LanguageCode { get; set; }
        public bool UseReadUncommited { get; set; } = false;
    }
}
