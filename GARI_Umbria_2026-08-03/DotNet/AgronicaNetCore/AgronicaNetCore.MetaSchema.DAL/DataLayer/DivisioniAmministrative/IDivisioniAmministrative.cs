using AgronicaCoreDTOStd.InData.Metaschema;
using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.DivisioniAmministrative
{
    public interface IDivisioniAmministrative
    {
        Task<DataTable> LeggiStatiAsync(string codice, AgronicaCoreParametriServer objParametriServer);
        
        Task<DataTable> LeggiRegioniAsync(LeggiRegioni_IN leggiRegioniIN, AgronicaCoreParametriServer objParametriServer);
        
        Task<DataTable> LeggiProvinceAsync(LeggiProvince_IN leggiProvinceIN, AgronicaCoreParametriServer objParametriServer);
        
        Task<DataTable> LeggiComuniAsync(LeggiComuni_IN leggiComuni_IN, AgronicaCoreParametriServer objParametriServer);
    }
}
