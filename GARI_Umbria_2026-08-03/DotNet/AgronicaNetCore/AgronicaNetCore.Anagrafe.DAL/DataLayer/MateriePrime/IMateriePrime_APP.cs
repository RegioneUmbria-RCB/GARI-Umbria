using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.MateriePrime
{
    public interface IMateriePrime_APP
    {
        Task<DataTable> ReadAnagraficaAsync(string piva, int elemCod, List<int> joinSpecieUtilizzate, AgronicaCoreParametriServer objParametriServer);
    }
}
