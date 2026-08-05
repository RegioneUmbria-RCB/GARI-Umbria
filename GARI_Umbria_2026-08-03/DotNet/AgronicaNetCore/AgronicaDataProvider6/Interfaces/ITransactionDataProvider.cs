using System.Data;
using System.Data.Common;

namespace AgronicaDataProvider6.Interfaces
{
    public interface ITransactionDataProvider
    {
        Task<DbTransaction?> OpenTransactionAsync(DbConnection connection, IsolationLevel? level = null);
        void CloseTransaction(ref DbTransaction transaction,bool Rollback=false);
    }
}
