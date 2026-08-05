using AgronicaCoreDTOStd.Identity;
using AgronicaNetCore.APP.BIZ.Services.Engine.BancheDati.Prodotti.Models;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.APP.BIZ.Services.Carichi;

namespace AgronicaNetCore.APP.BIZ.Services.Engine.BancheDati.Prodotti
{
    /// <summary>
    /// Servizio per il recupero dei prodotti (fertilizzanti e fitofarmaci) dall'engine Banche Dati.
    /// </summary>
    public interface IProdottiBancheDatiEngineService
    {
        /// <summary>
        /// Restituisce true se l'engine Banche Dati è raggiungibile: prima controlla le chiavi
        /// di configurazione (UrlEngineBancheDati / ApiKeyEngineBancheDati), poi in cascata
        /// interroga il DB tramite RecuperaConfigurazioneEngineAsync.
        /// Restituisce false solo se nessuna delle due sorgenti produce valori validi.
        /// </summary>
        Task<bool> IsEngineConfiguratoAsync(
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer);

        /// <summary>
        /// Recupera l'elenco dei fertilizzanti dall'engine Banche Dati secondo i filtri specificati.
        /// </summary>
        Task<IReadOnlyList<FertilizzanteEngineDto>> GetFertilizzantiAsync(
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            RicercaFertilizzantiFiltriEngineDto filtri,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Recupera l'elenco dei fitofarmaci (FS024) dall'engine Banche Dati secondo i filtri specificati.
        /// </summary>
        Task<IReadOnlyList<FormulatoEngineDto>> GetFormulatiAsync(
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            RicercaFitofarmaciFilterEngineDto filtri,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Recupera l'elenco dei prodotti usando l'api legacy dei core ws.
        /// </summary>
        Task<List<ProdottoEntity>> GetProdottiAsync(string piva, int categoria, string filtro, int  specie, string codici, string stato, int lav_cod, 
            ObjParametri objParametri, AgronicaCoreParametriTriple tripleParams, string bearerToken, string coreWsUrl);

    }
}
