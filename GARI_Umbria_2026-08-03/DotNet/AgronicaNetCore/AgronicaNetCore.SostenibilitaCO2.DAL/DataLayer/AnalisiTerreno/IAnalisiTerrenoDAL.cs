using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.SostenibilitaCO2.DAL.DataLayer.AnalisiTerreno
{
    /// <summary>
    /// Contratto per l'accesso alla tabella <c>Analisi_Terreno</c> per il recupero
    /// del parametro Sostanza Organica (<c>SO</c>) nel contesto del calcolo CO2.
    /// Riferimento spec: DS04-BL AssemblyPayloadM4AppezzamentoImpianto — Persistenze, Tabella <c>Analisi_Terreno</c>, Regola 3.
    /// </summary>
    public interface IAnalisiTerrenoDAL
    {
        /// <summary>
        /// Recupera il valore percentuale di Sostanza Organica (<c>SO</c>) per un appezzamento,
        /// selezionando l'analisi più recente disponibile.
        /// </summary>
        /// <param name="piva">Partita IVA dell'azienda.</param>
        /// <param name="saCod">Codice azienda satellite.</param>
        /// <param name="appezza">Codice appezzamento.</param>
        /// <param name="objParametriServer">Parametri di connessione al database GIAS.</param>
        /// <returns>
        /// <see cref="AnalisiTerrenoEntity"/> con <c>SostanzaOrganica</c> valorizzato se disponibile;
        /// <c>null</c> se nessun valore SO è presente per l'appezzamento (campo opzionale nel payload).
        /// </returns>
        Task<AnalisiTerrenoEntity?> GetSostanzaOrganicaAsync(string piva, int saCod, int appezza, AgronicaCoreParametriServer objParametriServer);
    }
}
