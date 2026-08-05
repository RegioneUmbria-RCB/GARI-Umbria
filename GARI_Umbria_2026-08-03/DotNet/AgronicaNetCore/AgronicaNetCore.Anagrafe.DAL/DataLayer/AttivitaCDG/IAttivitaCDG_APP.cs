using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.AttivitaCDG
{
    public interface IAttivitaCDG_APP
    {
        Task<DataTable> ReadAsync(AgronicaCoreParametriServer objParametriServer);
    }
}
