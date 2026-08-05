using AgronicaCoreDTOStd.InData.Metaschema;
using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.UnitaMisuraConversione
{
    public interface IUnitaMisuraConversione
    {
        Task<DataTable> ReadAsync(UnitaMisuraConversione_IN leggiUnitaMisuraConversioneIN, AgronicaCoreParametriServer objParametriServer);
    }
}
