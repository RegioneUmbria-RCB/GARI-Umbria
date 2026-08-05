using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.MisureIndiciMaturitaAnagrafiche
{
    public interface IMisureIndiciMaturitaAnagrafiche_APP
    {
        Task<DataTable> ReadAsync(AgronicaCoreParametriServer objParametriServer);
    }
}