using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.LavorazioniAttivitaCDG
{
    public interface ILavorazioniAttivitaCDG_APP
    {
        Task<DataTable> ReadAsync(AgronicaCoreParametriServer objParametriServer);
    }
}
