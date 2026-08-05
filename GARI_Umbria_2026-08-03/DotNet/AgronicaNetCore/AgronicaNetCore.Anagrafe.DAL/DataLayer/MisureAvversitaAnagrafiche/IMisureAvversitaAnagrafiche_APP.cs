using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.MisureAvversitaAnagrafiche
{
    public interface IMisureAvversitaAnagrafiche_APP
    {
        Task<DataTable> ReadAsync(AgronicaCoreParametriServer objParametriServer);
    }
}