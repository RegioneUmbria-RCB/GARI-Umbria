
using System.Data;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Base.Interfaces
{
    public interface IDBConnection
    {
        Task OpenConnectionAsync(AgronicaCoreParametri objParametri, bool OpenTransaction = true, IsolationLevel? level = null);
        void CloseConnection(AgronicaCoreParametri objParametri, bool RollbackTransaction = false);
    }
}
