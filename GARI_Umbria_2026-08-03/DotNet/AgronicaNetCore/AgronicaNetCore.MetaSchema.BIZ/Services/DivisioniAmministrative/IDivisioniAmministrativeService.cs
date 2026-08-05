using AgronicaCoreDTOStd.InData.Metaschema;
using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.MetaSchema.BIZ.Services.DivisioniAmministrative
{
    public interface IDivisioniAmministrativeService
    {
        Task<DataTable> LeggiStatiAsync(string codice, AgronicaCoreParametriServer objParametriServer);

        Task<DataTable> LeggiRegioniAsync(LeggiRegioni_IN leggiRegioni_IN, AgronicaCoreParametriServer objParametriServer);

        Task<DataTable> LeggiProvinceAsync(LeggiProvince_IN leggiProvince_IN, AgronicaCoreParametriServer objParametriServer);

        Task<DataTable> LeggiComuniAsync(LeggiComuni_IN leggiComuni_IN, AgronicaCoreParametriServer objParametriServer);

    }
}
