
using System.Data;
using System.Data.Common;

namespace AgronicaDataProvider6.Interfaces
{
    public interface IConnectionDataProvider
    {
        Task<(DbConnection?, DbTransaction?)> OpenConnectionAsync(bool OpenTransaction = true,IsolationLevel? level = null);
        void CloseConnection(ref DbConnection? objConnessione,bool RollbackTransaction = false);
    }
}
