using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SostenibilitaH2O.BIZ.Exceptions;
using AgronicaNetCore.SostenibilitaH2O.DAL.DataLayer.BenchmarkWaterFootprint;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Services.LookupBenchmarkWaterFootprint
{
    /// <summary>
    /// Implementazione della business logic per il recupero del Green-Blue Water Footprint (DS06-BL).
    /// <para>
    /// Flusso:
    /// <list type="number">
    /// <item><description>Valida <c>paeseIsoAlpha3</c> (esattamente 3 caratteri alfabetici).</description></item>
    /// <item><description>Trascodifica <c>vegCodGias</c> → <c>productCodeFaostat</c> via <c>CAC_Codifica_Veg_Cod</c>.</description></item>
    /// <item><description>Interroga <c>waterfootprint</c> con filtro su regione (se fornita).</description></item>
    /// <item><description>Se nessun risultato con regione, riprova con <c>CNTRY-average</c>.</description></item>
    /// <item><description>Se ancora nessun risultato, solleva <see cref="BenchmarkNotFoundException"/> (bloccante).</description></item>
    /// </list>
    /// </para>
    /// Riferimento spec: DS06-BL LookupBenchmarkWaterFootprint.
    /// </summary>
    public class LookupBenchmarkWaterFootprintService
        : BaseServiceSostenibilitaH2OBiz, ILookupBenchmarkWaterFootprintService
    {
        private readonly ILogger<LookupBenchmarkWaterFootprintService> _logger;

        /// <summary>Numero di caratteri attesi per un codice ISO Alpha-3.</summary>
        private const int IsoAlpha3Length = 3;

        /// <inheritdoc cref="BaseServiceSostenibilitaH2OBiz"/>
        public LookupBenchmarkWaterFootprintService(
            ILogger<LookupBenchmarkWaterFootprintService> logger,
            IServiceProvider provider,
            IStringLocalizer<Resources.Messages> localizer)
            : base(provider, localizer)
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<decimal> GetWaterFootprintAsync(
            int vegCodGias,
            string paeseIsoAlpha3,
            string? regionCode,
            AgronicaCoreParametriServer objParametriServer)
        {
            ArgumentNullException.ThrowIfNull(objParametriServer);
            if (string.IsNullOrWhiteSpace(paeseIsoAlpha3))
                throw new ArgumentException("Il codice paese non può essere vuoto.", nameof(paeseIsoAlpha3));

            var benchmarkDal = _serviceProvider.GetRequiredService<IBenchmarkWaterFootprintDAL>();

            // ── Step 0: conversione Alpha-2 → Alpha-3 (se necessario) ─────────────
            if (paeseIsoAlpha3.Length == 2 && paeseIsoAlpha3.All(char.IsLetter))
            {
                string alpha2 = paeseIsoAlpha3;
                string? converted;
                try
                {
                    converted = await benchmarkDal.GetIsoAlpha3FromAlpha2Async(alpha2, objParametriServer);
                }
                catch (Exception ex)
                {
                    throw new DatabaseQueryException(
                        $"Errore durante la conversione del codice paese Alpha-2 '{alpha2}' in Alpha-3.", ex);
                }

                if (string.IsNullOrWhiteSpace(converted))
                    throw new InvalidCountryCodeException(
                        $"Il codice paese '{alpha2}' non è stato trovato in ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166. " +
                        "Impossibile ricavare il codice ISO Alpha-3.");

                paeseIsoAlpha3 = converted;
                _logger.LogInformation(
                    "Conversione paese Alpha-2 → Alpha-3: '{Alpha2}' → '{Alpha3}'.", alpha2, paeseIsoAlpha3);
            }

            // ── Step 1: validazione codice paese ISO Alpha-3 ──────────────────────
            if (paeseIsoAlpha3.Length != IsoAlpha3Length
                || !paeseIsoAlpha3.All(char.IsLetter))
            {
                throw new InvalidCountryCodeException(
                    $"Il codice paese '{paeseIsoAlpha3}' non è un codice ISO Alpha-3 valido. " +
                    "Deve essere esattamente 3 caratteri alfabetici (es. 'ITA').");
            }

            // ── Step 2: trascodifica Veg_Cod GIAS → product_code_faostat ─────────
            string? productCodeFaostat;
            try
            {
                productCodeFaostat = await benchmarkDal.GetProductCodeFaostatAsync(
                    vegCodGias, objParametriServer);
            }
            catch (Exception ex) when (ex is not InvalidCountryCodeException)
            {
                throw new DatabaseQueryException(
                    $"Errore durante la trascodifica del codice varietà GIAS {vegCodGias} → FAOSTAT.", ex);
            }

            if (string.IsNullOrWhiteSpace(productCodeFaostat))
            {
                throw new MalformedVarietaException(
                    $"Nessun codice FAOSTAT trovato per il codice varietà GIAS {vegCodGias} " +
                    "nella tabella CAC_Codifica_Veg_Cod.");
            }

            _logger.LogInformation(
                "Trascodifica completata: VegCodGias={VegCodGias} → ProductCodeFaostat={ProductCode}.",
                vegCodGias, productCodeFaostat);

            // ── Step 3: lookup waterfootprint ───────────────────────────
            decimal? wfValue;
            try
            {
                wfValue = await benchmarkDal.GetWaterFootprintAsync(
                    productCodeFaostat, paeseIsoAlpha3, regionCode, objParametriServer);
            }
            catch (Exception ex) when (ex is not InvalidCountryCodeException
                                               and not MalformedVarietaException)
            {
                throw new DatabaseQueryException(
                    $"Errore durante il recupero del benchmark WF per " +
                    $"productCode={productCodeFaostat}, paese={paeseIsoAlpha3}.", ex);
            }

            if (wfValue is null)
            {
                throw new BenchmarkNotFoundException(
                    $"Nessun valore Green+Blue WF trovato in waterfootprint per " +
                    $"productCode='{productCodeFaostat}', paese='{paeseIsoAlpha3}' " +
                    "(incluso fallback CNTRY-average). Il calcolo del bilancio idrico non può proseguire.");
            }

            return Math.Round(wfValue.Value, 2);
        }
    }
}
