using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.UnitaMisura
{
    public interface IUnitaMisura
    {
        Task<DataTable> LeggiAsync(int udmCod, int udmCodAux, AgronicaCoreParametriServer objParametriServer);
        Task<DataTable> LeggiAsyncxProtocolli(AgronicaCoreParametriServer objParametriServer);
    }
}
