using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Services.RiepilogoRaccolti
{
    /// <summary>
    /// Contratto del servizio per il calcolo della tabella di riepilogo raccolti CO₂.
    /// </summary>
    public interface IRiepilogoRaccoltiService
    {
        /// <summary>
        /// Restituisce la tabella di riepilogo dei raccolti per tutte le aziende figlie della filiera,
        /// filtrata per la coltura e l'anno selezionati. Sono incluse solo le operazioni
        /// con tutti gli esercizi chiusi (<c>Flag_Esercizio_Chiuso = "1"</c>).
        /// </summary>
        /// <param name="request">Parametri di input: P.IVA filiera, codice coltura e anno.</param>
        /// <param name="objParametriServer">Parametri server correnti (connessione, utente, cultura).</param>
        /// <returns>La tabella di riepilogo con le righe aggregate per esercizio chiuso.</returns>
        Task<RiepilogoRaccoltiResult> GetRiepilogoAsync(
            RiepilogoRaccoltiRequest request,
            AgronicaCoreParametriServer objParametriServer);
    }
}
