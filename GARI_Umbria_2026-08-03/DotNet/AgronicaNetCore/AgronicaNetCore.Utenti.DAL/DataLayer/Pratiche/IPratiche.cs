using AgronicaCoreVisibilitaStd.Models;
using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.Utenti.DAL.DataLayer.Pratiche
{
    public interface IPratiche
    {
        Task<DataTable> LeggiPiveGerarchiaAsync(string descrizione2, AgronicaCoreParametriServer objParametriServer);

        Task<DataTable> LeggiCentriGerarchiaAsync(string descrizione2, AgronicaCoreParametriServer objParametriServer);

        Task<DataTable> LeggiPiveVisibiliAsync(string pivaSuperUser, List<PraticaProfilo> pratiche, AgronicaCoreParametriServer objParametriServer);

        Task<DataTable> LeggiCentriPerPiveAsync(IEnumerable<string> pive, AgronicaCoreParametriServer objParametriServer);

        /// <summary>
        /// Espande la gerarchia a partire dalle pive "capostipite" (cfr. Utenti_Profili.Descrizione_1).
        /// Replica della CTE ricorsiva di <c>Utenti_Visibilita_Appoggio.PopolaConGerarchia</c>,
        /// in forma SELECT (Piva, Sa_Cod). Ritorna DataTable vuoto per lista vuota o wildcard "###########".
        /// </summary>
        Task<DataTable> LeggiCapostipitiEspansioneAsync(IEnumerable<string> piveCapostipiti, AgronicaCoreParametriServer objParametriServer);
    }
}
