using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.ParcoMacchine
{
    public interface IParcoMacchine_APP
    {
        Task<DataTable> ReadAsync(string piva, AgronicaCoreParametriServer objParametriServer);
    }
}
