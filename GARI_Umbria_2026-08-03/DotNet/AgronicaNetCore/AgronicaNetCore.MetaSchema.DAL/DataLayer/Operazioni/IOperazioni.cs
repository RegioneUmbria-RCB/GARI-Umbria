using AgronicaCoreDTOStd.InData.Metaschema;
using AgronicaCoreDTOStd.OutData.FiltroRicerca;
using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.Operazioni
{
    public interface IOperazioni
    {
        Task<DtConVisibilita_OUT> Operazioni_GestioneFiltroUtente_LeggiAsync(LeggiOperazioni_IN leggiOperazioni, DataTable utentiImpostazioniDt, AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriUtenti objParametriUtenti);
        Task<DataTable> LeggiOperazioniPerTipoAsync(string tipoGruppoOperazione, AgronicaCoreParametriServer objParametriServer);
    }
}
