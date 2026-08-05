using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.Magazzino.DAL.DataLayer.CategorieMagazzino
{
    public interface ICategorieMagazzino
    {
        Task<DataTable> LeggiAsync(
            int elemCod,
            string cauMov,
            bool flagCantina,
            AgronicaCoreParametriServer objParametriServer);
    }
}
