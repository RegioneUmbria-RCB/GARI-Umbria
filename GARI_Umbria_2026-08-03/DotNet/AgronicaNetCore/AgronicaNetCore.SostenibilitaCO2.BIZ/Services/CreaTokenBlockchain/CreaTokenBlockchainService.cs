using AgronicaCoreDTOStd.InData;
using AgronicaCoreModelsSTD.provisioning;
using AgronicaCoreVarieBizSTD;
using AgronicaNetCore.Base.DataLayer.Security;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Base.Utility;
using AgronicaNetCore.SostenibilitaCO2.BIZ.Exceptions;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Impresa;
using AgronicaNetCore.Gis.DAL.DataLayer.Gis;
using AgronicaNetCore.SostenibilitaCO2.DAL.DataLayer.Lookup_Sost_CO2_Aziendale_Chiavi;
using InData.FoodMetaVerse;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using OutData.FoodMetaverse;
using System.Data;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Services.CreaTokenBlockchain
{
    /// <summary>
    /// Costruisce il payload M5 e lo invia al servizio Blockchain FMP per la creazione del Token CO2.
    /// <list type="number">
    ///   <item>Valida i campi obbligatori del payload M4 in input.</item>
    ///   <item>Legge metadati di contesto da <c>Lookup_Sost_CO2_Aziendale_Chiavi</c> (filiera, anno, data_invocazione).</item>
    ///   <item>Query su <c>Aziende</c> per Ragione Sociale, Città, Regione, Stato ISO.</item>
    ///   <item>Query GIS per WKT poligono + EPSG di ogni appezzamento.</item>
    ///   <item>Assembla il <see cref="SostenibilitaCO2CarbonDataRequest"/> e lo invia in POST al servizio Blockchain.</item>
    ///   <item>Restituisce la risposta del servizio Blockchain.</item>
    /// </list>
    /// Riferimento spec: DS10-BL CreaTokenBlockchain.
    /// </summary>
    public class CreaTokenBlockchainService : BaseServiceSostenibilitaCO2Biz, ICreaTokenBlockchainService
    {
        private const int    TimeoutSecondi         = 30;

        private readonly HttpClient _httpClient;
        private readonly ISecurityLayerDAL _securityLayerDal;
        private readonly ILookup_Sost_CO2_Aziendale_Chiavi _chiaviDal;
        private readonly IImpresa _impresaMetadataBlockchain;
        private readonly IGIS_ElementiGrafici _gisPoligonoDal;

        /// <summary>Initializes a new instance, resolving dependencies from the DI container.</summary>
        public CreaTokenBlockchainService(
            IServiceProvider provider,
            IStringLocalizer<Resources.Messages> localizer)
            : base(provider, localizer)
        {
            _httpClient         = provider.GetRequiredService<HttpClient>();
            _securityLayerDal   = provider.GetRequiredService<ISecurityLayerDAL>();
            _chiaviDal          = provider.GetRequiredService<ILookup_Sost_CO2_Aziendale_Chiavi>();
            _impresaMetadataBlockchain = provider.GetRequiredService<IImpresa>();
            _gisPoligonoDal     = provider.GetRequiredService<IGIS_ElementiGrafici>();
        }

        /// <inheritdoc/>
        public async Task<RispostaStandard> CreateTokenAsync(
            SostenibilitaCO2CreaTokenBlockchainRequest request,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer)
        {
            ArgumentNullException.ThrowIfNull(request);
            if (string.IsNullOrWhiteSpace(request.Azienda))
                throw new ArgumentException("Campo 'Azienda' obbligatorio.", nameof(request));
            if (string.IsNullOrWhiteSpace(request.Filiera))
                throw new ArgumentException("Campo 'Filiera' obbligatorio.", nameof(request));
            if (string.IsNullOrWhiteSpace(request.IdInvocazione))
                throw new ArgumentException("Campo 'IdInvocazione' obbligatorio.", nameof(request));
            ArgumentNullException.ThrowIfNull(request.PayloadLookupSostenibilitaCO2);

            var cuaaAzienda = await _impresaMetadataBlockchain.CuaaFromPivaAsync(request.Azienda, objParametriServer);
            if (string.IsNullOrWhiteSpace(cuaaAzienda))
                throw new DataNotFoundException($"CUAA non trovato per l'azienda PIVA '{request.Azienda}'.");

            var cuaaFiliera = await _impresaMetadataBlockchain.CuaaFromPivaAsync(request.Filiera, objParametriServer);
            if (string.IsNullOrWhiteSpace(cuaaFiliera))
                throw new DataNotFoundException($"CUAA non trovato per la filiera PIVA '{request.Filiera}'.");

            LogInformation(
                "CreaTokenBlockchain avviato — Azienda: {Azienda}, IdInvocazione: {IdInvocazione}",
                objParametriServer, null, request.Azienda, request.IdInvocazione);

            // ── 1: Validate M4 response mandatory fields ──────────────────────────────
            var lookupAziendale = UtilityAgronica.FromJsonStringToType<SostenibilitaCO2LookupAziendaleResponse>(request.PayloadLookupSostenibilitaCO2);
            if (lookupAziendale == null)
                throw new ArgumentException("Campo 'PayloadLookupSostenibilitaCO2' non è nel formato corretto.", nameof(request));

            // ── 2: Locate azienda in M4 response ─────────────────────────────────────
            var respAzienda = lookupAziendale.Aziende.FirstOrDefault(a =>
                string.Equals(a.IdAzienda, cuaaAzienda, StringComparison.OrdinalIgnoreCase));

            if (respAzienda is null)
                throw new PayloadStructureException(
                    $"Azienda '{request.Azienda}' non trovata nella risposta M4 per id_invocazione='{request.IdInvocazione}'.");

            // ── 3: Read lookup chiavi for context (filiera, anno, data_invocazione) ───
            var chiaviTable = await _chiaviDal.ReadAsync(
                new GetSostCO2Aziendale {
                    Azienda = cuaaAzienda,
                    Filiera = cuaaFiliera,
                    IdInvocazione = request.IdInvocazione 
                },
                objParametriServer);

            if (chiaviTable.Rows.Count == 0)
                throw new DataNotFoundException(request.IdInvocazione);

            var chiaviRow = chiaviTable.Rows[0];
            var anno = ReadColumnInt(chiaviRow, "anno");
            var dataInvocazione = ReadColumn(chiaviRow, "data_invocazione") ?? string.Empty;

            if (!anno.HasValue || anno.Value <= 0)
                throw new PayloadStructureException(
                    $"Campagna non indicata impossibile definire il periodo d'esercizio per id_invocazione='{request.IdInvocazione}'.");

            lookupAziendale.StartDate = new DateTime(anno!.Value, 1, 1).ToString("yyyy-MM-dd");
            lookupAziendale.EndDate = new DateTime(anno!.Value, 12, 31).ToString("yyyy-MM-dd");

            // ── 4: Read azienda metadata from Aziende table ───────────────────────────
            var aziendaTable = await _impresaMetadataBlockchain.LeggiMetadatiImpresaBlockchainAsync(request.Azienda, objParametriServer);
            if (aziendaTable.Rows.Count == 0)
                throw new DataNotFoundException(request.Azienda);

            var aziendaRow = aziendaTable.Rows[0];

            var ragioneSociale = ReadColumn(aziendaRow, "RagioneSociale");
            if (string.IsNullOrWhiteSpace(ragioneSociale))
                throw new DataNotFoundException(request.Azienda, "RagioneSociale");
            
            var citta = ReadColumn(aziendaRow, "Citta");
            if (string.IsNullOrWhiteSpace(citta))
                throw new DataNotFoundException(request.Azienda, "Città");

            var regione = ReadColumn(aziendaRow, "Regione");
            if (string.IsNullOrWhiteSpace(regione) )
                throw new DataNotFoundException(request.Azienda, "Regione");
            
            var statoIso = ReadColumn(aziendaRow, "Stato_ISO3");
            if (string.IsNullOrWhiteSpace(statoIso))
                throw new DataNotFoundException(request.Azienda, "Stato_ISO3");

            // ── 5: Build token code (Regola 1) ────────────────────────────────────────
            var code = string.Join("-", cuaaFiliera, cuaaAzienda, anno, dataInvocazione, request.IdInvocazione);

            // ── 6: Compute total area and build fields[] (Regole 7-8) ─────────────────
            var totalAreaHa = respAzienda.Appezzamenti
                .Sum(a => ParseDecimal(a.AreaHa, cifreDecimali: 4));

            decimal totalVarSocSoilBiogenicCarbon = 0;
            var fields = new List<SostenibilitaCO2CarbonDataField>(respAzienda.Appezzamenti.Count);
            foreach (var appResp in respAzienda.Appezzamenti)
            {
                if (!int.TryParse(appResp.IdAppezzamento, out var appezzaCode))
                    throw new PayloadStructureException(
                        $"Campo 'id_appezzamento' non parsabile come intero: '{appResp.IdAppezzamento}'.");

                totalVarSocSoilBiogenicCarbon += ParseDecimal(appResp.VarSocSoilBiogenicCarbon, 
                    exceptionMessage: $"Campo 'var_soc_soil_biogenic_carbon' dell'appezzamento non valido o assente per azienda '{request.Azienda}'.");

                var (wkt, epsg) = await _gisPoligonoDal.GetPoligonoConEpsgAsync(
                    request.Azienda, appezzaCode, objParametriServer);

                // Missing geometry is a blocking error — no fallback (DS10-BL spec).
                if (string.IsNullOrWhiteSpace(wkt))
                    throw new MissingGeometryException(appResp.IdAppezzamento);

                decimal areaParziale = ParseDecimal(appResp.AreaHa, cifreDecimali: 4);
                if (fields.Count == (respAzienda.Appezzamenti.Count - 1))
                    areaParziale = totalAreaHa - fields.Sum(f => f.AreaHa);

                fields.Add(new SostenibilitaCO2CarbonDataField
                {
                    Code = appResp.IdAppezzamento,
                    Wkt = wkt,
                    Epsg = epsg,
                    AreaHa = areaParziale,
                    VarSocSoilBiogenicCarbon = ParseDecimal(appResp.VarSocSoilBiogenicCarbon)
                });
            }

            // ── 7: Assemble SostenibilitaCO2CarbonDataRequest (M5 payload) ───────────
            var payloadM5 = new SostenibilitaCO2CarbonDataRequest
            {
                Code = code,
                FarmAdminId = cuaaFiliera,
                StartDate = lookupAziendale.StartDate,
                EndDate = lookupAziendale.EndDate,
                VarSocSoilBiogenicCarbon = totalVarSocSoilBiogenicCarbon,
                Farm = new SostenibilitaCO2CarbonDataFarm
                {
                    Code = cuaaAzienda,
                    Name = ragioneSociale,
                    City = citta,
                    Region = regione,
                    State = statoIso,
                    AreaHa = totalAreaHa,
                    Fields = fields
                }
            };
            var jsonPayload = UtilityAgronica.ToJson(payloadM5);

            LogInformation(
                SanitizeLogMessage($"CreaTokenBlockchain — invio a Blockchain. Azienda: {request.Azienda}, Fields: {fields.Count}"),
                objParametriServer);

            // ── 8: Read blockchain URL/ApiKey from Configurazione_Siti ────────────────
            var (urlNetCoreApi, tokenUsername, urlBlockchain) = await RecuperaConfigurazioneEngineAsync(objParametriServer, objParametriSuperServer);

            // ── 9: POST to blockchain and return response ─────────────────────────────
            var tokenBlockchain = await SendToBlockchainAsync(urlBlockchain, tokenUsername, jsonPayload);

            var risposta = new RispostaStandard()
            {
                RispostaOK = true,
                RispostaStringa = tokenBlockchain
            };

            LogInformation(
                "CreaTokenBlockchain completato — Azienda: {Azienda}, TokenCode: {Code}",
                objParametriServer, null, request.Azienda, tokenBlockchain);

            return risposta;
        }

        // ── Private helpers ───────────────────────────────────────────────────────────
        private async Task<string> SendToBlockchainAsync(string urlBlockchain, string apiKey, string jsonPayload)
        {
            string? result = null!;
            try
            {
                long timeout = TimeoutSecondi;
                string url = $"{urlBlockchain}/api/carbon-data/submit";
                //_httpClient.Timeout = new TimeSpan(timeout);

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
                if (!string.IsNullOrWhiteSpace(apiKey))
                {
                    request.Headers.Add("x-api-key", apiKey);
                    request.Headers.Add("x-client-module", "M1/M4");
                    request.Headers.Add("x-request-id", Guid.NewGuid().ToString());
                }
                request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(TimeoutSecondi));
                {
                    HttpResponseMessage response = await _httpClient!.SendAsync(request);
                    string content = await response.Content.ReadAsStringAsync();

                    switch (response.StatusCode)
                    {
                        case System.Net.HttpStatusCode.OK:
                        case System.Net.HttpStatusCode.Accepted:
                            var validResponse = UtilityAgronica.FromJsonStringToType<SubmitBlockchainResponse>(content);
                            result = validResponse!.data.acknowledgmentId;
                            break;

                        case System.Net.HttpStatusCode.BadRequest:
                            var errorResponse = UtilityAgronica.FromJsonStringToType<SubmitBlockchainResponseError>(content);
                            throw new HttpRequestException(
                                $"{errorResponse!.error.message} => {errorResponse!.error.details.field} - {errorResponse!.error.details.reason}",
                                null,
                                System.Net.HttpStatusCode.BadRequest);

                        case System.Net.HttpStatusCode.TooManyRequests:
                            throw new HttpRequestException(
                                $"Il traffico verso il server Blockchain è saturo",
                                null,
                                System.Net.HttpStatusCode.TooManyRequests);

                        default:
                            var internalErrorResponse = UtilityAgronica.FromJsonStringToType<SubmitBlockchainResponseError>(content);
                            throw new HttpRequestException(
                                internalErrorResponse!.error.message,
                                null,
                                response.StatusCode);
                    }
                }
            }
            catch
            {
                throw;
            }
            return result;
        }

        private static string? ReadColumn(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName))
                return null;
            return row[columnName] == DBNull.Value ? null : row[columnName]?.ToString();
        }

        private static int? ReadColumnInt(DataRow row, string columnName)
        {
            int result = 0;
            string? value = ReadColumn(row, columnName);
            if (string.IsNullOrWhiteSpace(value))
                return null;
            if (!int.TryParse(value, out result))
                return null;
            return result;
        }

        private static decimal ParseDecimal(string? value, int cifreDecimali = -1, string exceptionMessage = null!)
        {
            decimal result = 0;
            if (string.IsNullOrWhiteSpace(exceptionMessage))
            {
                if (!decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out result))
                    result = 0;
            }
            else
            {
                if (!decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out result))
                    throw new PayloadStructureException(exceptionMessage);
            }

            if (cifreDecimali > -1)
                result = Math.Round((decimal)result, cifreDecimali);

            return result;
        }

        private async Task<(string urlNetCoreApi, string tokenUsername, string urlBlockchain)> RecuperaConfigurazioneEngineAsync(
            AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriSuperServer objParametriSuperServer)
        {
            string urlNetCoreApi = await _securityLayerDal.LeggiConfigurazioneSitiScalareAsync("GiasOnline_Core_API", objParametriServer, objParametriSuperServer);
            if (string.IsNullOrWhiteSpace(urlNetCoreApi))
                throw new Exception($"La chiave di configurazione 'GiasOnline_Core_API' non è stata trovata nel sistema di configurazione.");
            urlNetCoreApi = UtilityAgronica.AggiustaUrl(_securityLayerDal, urlNetCoreApi, objParametriServer, objParametriSuperServer);

            string tokenUsername = await _securityLayerDal.LeggiConfigurazioneSitiScalareAsync("FMP_M5_Blockchain_Apikey", objParametriServer, objParametriSuperServer);
            if (string.IsNullOrWhiteSpace(tokenUsername))
                throw new Exception($"La chiave di configurazione 'FMP_M5_Blockchain_Apikey' non è stata trovata nel sistema di configurazione.");

            string urlBlockchain = await _securityLayerDal.LeggiConfigurazioneSitiScalareAsync("FMP_M5_Blockchain_UrlSubmit", objParametriServer, objParametriSuperServer);
            if (string.IsNullOrWhiteSpace(urlBlockchain))
                throw new Exception($"La chiave di configurazione 'FMP_M5_Blockchain_UrlSubmit' non è stata trovata nel sistema di configurazione.");
            urlBlockchain = UtilityAgronica.AggiustaUrl(_securityLayerDal, urlBlockchain, objParametriServer, objParametriSuperServer);

            return (urlNetCoreApi, tokenUsername, urlBlockchain);
        }
    }
}
