using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.DSSNutrizione.DAL.DataLayer.BilancioAzoto
{
    /// <summary>
    /// Interfaccia DAL per il caricamento degli eventi della timeline del bilancio azoto.
    /// Restituisce eventi BBCH_CHANGE (da operazioni QDCA Lav_Cod=79) e ANALISI_TERRENO
    /// (da Analisi_EntitaxTestata / Analisi_Testata) nell'arco di validità dell'impianto.
    /// Riferimento: DS09-BL — Persistenze Coinvolte.
    /// </summary>
    public interface IBilancioAzotoDAL
    {
        /// <summary>
        /// Restituisce tutti gli eventi BBCH_CHANGE e ANALISI_TERRENO della timeline
        /// nell'arco di validità dell'impianto (Imprese_Progetti.Validita_Inizio / Validita_Fine),
        /// ordinati cronologicamente per data crescente.
        /// Le colonne Validita_Inizio e Validita_Fine sono incluse in ogni riga per consentire
        /// al servizio BIZ di determinare la finestra temporale senza una query separata.
        /// Riferimento: DS09-BL — Persistenze Coinvolte; Regole di Business (Timeline copre la validità dell'impianto).
        /// </summary>
        /// <param name="piva">Partita IVA dell'azienda (obbligatoria).</param>
        /// <param name="saCod">Codice centro aziendale.</param>
        /// <param name="appezza">Codice appezzamento.</param>
        /// <param name="idReg">ID registro impianto.</param>
        /// <param name="progettoCod">Codice progetto (Imprese_Progetti.Progetto_Cod).</param>
        /// <param name="objParametriServer">Parametri server per la connessione al database.</param>
        /// <returns>
        /// DataTable con colonne: Validita_Inizio, Validita_Fine, Tipo_Evento, Data_Evento,
        /// BBCH_Cod, Id_Agenda, Id_Mov, Id_Mov_Det, Analisi_Testata_Cod, Analisi_SuperUser,
        /// Sabbia_Percentuale, Limo_Percentuale, Argilla_Percentuale, N_Totale.
        /// </returns>
        Task<DataTable> LeggiEventiTimelineAsync(
            string piva,
            int saCod,
            int appezza,
            int idReg,
            int progettoCod,
            AgronicaCoreParametriServer objParametriServer);
    }
}
