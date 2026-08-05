using AgronicaCoreDTOStd.InData.Metaschema;
using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.MetaSchema.BIZ.Services.UnitaMisuraConversione
{
    public interface IUnitaMisuraConversioneService
    {
        Task<DataTable> ReadAsync(UnitaMisuraConversione_IN leggiUnitaMisuraConversioneIN, AgronicaCoreParametriServer objParametriServer);
    }
}
