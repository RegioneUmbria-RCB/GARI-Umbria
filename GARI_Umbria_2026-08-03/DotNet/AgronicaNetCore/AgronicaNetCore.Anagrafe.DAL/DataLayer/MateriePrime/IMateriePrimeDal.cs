using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.MateriePrime
{
    public interface IMateriePrimeDal
    {
        Task<DataTable> LeggiAsync(string piva, int elemCod, int matCod, AgronicaCoreParametriServer parametriServer);
    }
}
