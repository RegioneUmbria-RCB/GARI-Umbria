using System.Data;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.AttivitaxCentriAziendali
{
    public interface IAttivitaxCentriAziendaliAPP
    {
        Task<DataTable> ReadAsync(string piva, AgronicaCoreParametriServer objParametriServer);
    }
}
