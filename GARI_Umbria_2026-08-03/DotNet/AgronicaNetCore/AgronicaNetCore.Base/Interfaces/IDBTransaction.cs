using System.Data;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Base.Interfaces
{
    public interface IDBTransaction
    {
        Task OpenTransactionAsync(AgronicaCoreParametri objParametri, IsolationLevel? level = null);
        void CloseTransaction(AgronicaCoreParametri objParametri, bool Rollback = false);
    }
}
