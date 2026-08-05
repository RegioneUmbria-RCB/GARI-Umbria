using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.Progetti
{
    public interface IProgetti_APP
    {
        Task<DataTable> ReadAsync(string piva, AgronicaCoreParametriServer objParametriServer);
    }
}
