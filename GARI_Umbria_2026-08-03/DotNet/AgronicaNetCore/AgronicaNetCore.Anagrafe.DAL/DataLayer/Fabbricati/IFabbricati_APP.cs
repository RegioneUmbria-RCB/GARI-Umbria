using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.Fabbricati
{
    public interface IFabbricati_APP
    {
        Task<DataTable> ReadAsync(string piva, AgronicaCoreParametriServer objParametriServer);
    }
}
