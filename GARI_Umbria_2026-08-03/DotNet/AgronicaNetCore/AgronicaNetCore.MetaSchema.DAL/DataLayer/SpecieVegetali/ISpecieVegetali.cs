using AgronicaCoreDTOStd.InData.Metaschema;
using AgronicaCoreDTOStd.OutData.FiltroRicerca;
using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.SpecieVegetali
{
    public interface ISpecieVegetali
    {
        Task<DtConVisibilita_OUT> SpecieVegetali_GestioneFiltroUtente_LeggiAsync(LeggiSpecieVegetali_IN leggiSpecie_IN, DataTable utentiImpostazioniMonoDt, AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer);

        Task<DataTable> Cultivar_GestioneFiltroUtente_LeggiAsync(Cultivar_GestioneFiltroUtente_Leggi_IN leggiCultivar_IN, DataTable utentiImpostazioniMonoDt, AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer);

        Task<DataTable> GruppoVegetale_GestioneFiltroUtente_LeggiAsync(int gru_cod, AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer);

        Task<DataTable> LeggiGruppiVarietaliAsync(LeggiGruppiVarietali_IN leggiGruppiVarietali_IN, AgronicaCoreParametriServer objParametriServer);
        Task<string> VegDesFromVegCodAsync(int vegCod, AgronicaCoreParametriServer objParametriServer);
        Task<DataTable> LeggiVarietaAsync(int culCod, int vegCod, AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer);
        Task<DataTable> LeggiSpecieAziendaliAsync(string piva, AgronicaCoreParametriServer objParametriServer);
        Task<DataTable> LeggiVarietaFilteredAsync(string piva, int culCod, int vegCod, DataTable dtCentriVisibili, AgronicaCoreParametriServer objParametriServer);
        Task<DataTable> LeggiCompletaAsync(int vegCod, int gruCod, AgronicaCoreParametriServer objParametriServer);
    }
}
