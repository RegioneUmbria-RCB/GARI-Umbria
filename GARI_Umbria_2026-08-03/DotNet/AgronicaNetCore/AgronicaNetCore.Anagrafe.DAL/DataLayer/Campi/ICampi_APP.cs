using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.Campi
{
    public interface ICampi_APP
    {
        Task<DataTable> ReadAsync(string piva, AgronicaCoreParametriServer objParametriServer);
    }
}
