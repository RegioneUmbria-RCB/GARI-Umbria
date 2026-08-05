using AgronicaNetCore.Agenda.DAL.DataLayer.Agenda;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Gis.DAL.DataLayer.Gis;
using AgronicaNetCore.RischiMeteo.BIZ.Exceptions;
using AgronicaNetCore.RischiMeteo.BIZ.Models;
using AgronicaNetCore.RischiMeteo.BIZ.Models.RischiMeteo.Request;
using AgronicaNetCore.RischiMeteo.DAL.DataLayer.GisGeometria;
using AgronicaNetCore.RischiMeteo.DAL.DataLayer.Impianti;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Data;

namespace AgronicaNetCore.RischiMeteo.BIZ.Services.AssemblyPayloadRischiMeteo
{
    /// <summary>
    /// Implementa la costruzione automatica dei payload JSON per l'Engine Rischi Meteoclimatici (M2).
    /// Per ogni Esercizio selezionato, integra dati GIAS (Agenda, GIS, Impianti) e assembla
    /// il payload conforme alla specifica API M2.
    /// I fallimenti per singolo Esercizio sono isolati: gli altri payload vengono comunque costruiti.
    /// Riferimento spec: DS02-BL CostruttoPayloadM2.
    /// </summary>
    public class AssemblyPayloadRischiMeteoService
        : BaseServiceRischiMeteoBiz, IAssemblyPayloadRischiMeteoService
    {
        private const string CodificaFissa = "profitosan";
        private const string EpsgFisso     = "4326";
        private const int GiorniFinestraFine = 7;

        /// <summary>
        /// Inizializza il servizio e registra il logger.
        /// Le dipendenze DAL vengono risolte lazily all'interno di <see cref="AssembleAsync"/>.
        /// </summary>
        public AssemblyPayloadRischiMeteoService(
            IServiceProvider provider,
            IStringLocalizer<Resources.Messages> localizer)
            : base(provider, localizer)
        {
        }

        /// <inheritdoc/>
        public async Task<CostruzionePayloadRischiMeteoOutput> AssembleAsync(
            CostruzionePayloadRischiMeteoInput input,
            AgronicaCoreParametriServer objParametriServer)
        {
            ArgumentNullException.ThrowIfNull(input);

            if (input.EserciziSelezionati.Count == 0)
                throw new ArgumentException("Specificare almeno un Esercizio.", nameof(input));

            var agenda          = _serviceProvider.GetRequiredService<IAgenda>();
            var gisClusterDAL = _serviceProvider.GetRequiredService<IGisClusterConfig>();
            var impiantiDAL     = _serviceProvider.GetRequiredService<IImpiantiRischiMeteoDAL>();
            LogInformation(
                "AssemblyPayloadRischiMeteo avviato – Filiera: {IdFiliera}, Esercizi: {Count}",
                objParametriServer, null, input.IdFiliera, input.EserciziSelezionati.Count);

            // ----------------------------------------------------------------
            // Recupera operazioni di Semina/Trapianto per tutti gli Esercizi
            // in un'unica chiamata, usando i Lav_Cod semina standard.
            // ----------------------------------------------------------------
            var idEsercizi   = input.EserciziSelezionati.Select(e => e.id_esercizio).Distinct().ToList();
            var lavCodSemina = new List<int>(Common.OperazioniSeminaTrapianto);

            DataTable dtSemina;
            try
            {
                dtSemina = await agenda.LeggiImpiantiAsync(
                    Id_Agenda:                 new List<int>(),
                    objParametriServer:        objParametriServer,
                    createTempEsercizi:        false,
                    leggiFlagEsercizioChiuso:  false,
                    leggiIndirizzoAppezzamento: false,
                    leggiGIS:                  false,
                    Id_Esercizi:               idEsercizi,
                    Lav_cod:                   lavCodSemina);
            }
            catch (Exception ex)
            {
                LogError(
                    "LeggiImpiantiAsync (semina) fallita per Filiera {IdFiliera}.", objParametriServer, ex, input.IdFiliera);
                throw;
            }

            // Raggruppa le righe di semina per Progetto_Cod
            var seminePerEsercizio = dtSemina.AsEnumerable()
                .GroupBy(r => Convert.ToInt32(r["progetto_cod"]))
                .ToDictionary(g => g.Key, g => g.ToList());


            // ----------------------------------------------------------------
            // Costruzione payload per ogni Esercizio (indipendente, fail-safe)
            // ----------------------------------------------------------------
            var payloads = new List<PayloadRischiMeteoRisultato>();
            var errori   = new List<ErroreCostruzioneRischiMeteo>();

            foreach (var esercizio in input.EserciziSelezionati)
            {
                var avvisi = new List<string>();
                try
                {
                    seminePerEsercizio.TryGetValue(esercizio.id_esercizio, out var righeSemina);
                    //datiBasePerEsercizio.TryGetValue(esercizio.id_esercizio, out var righeBase);

                    var payload = await BuildPayloadAsync(
                        esercizio, righeSemina,
                        gisClusterDAL, impiantiDAL, avvisi, objParametriServer);

                    payloads.Add(new PayloadRischiMeteoRisultato
                    {
                        Esercizio = esercizio,
                        Payload   = payload,
                        Avvisi    = avvisi
                    });
                }
                catch (InvalidWktFormatException ex)
                {
                    LogError("WKT non valido per l'Esercizio {Id}.", objParametriServer, ex, esercizio.id_esercizio);
                    errori.Add(BuildErrore(esercizio, nameof(InvalidWktFormatException), ex.Message));
                }
                catch (MissingMandatoryFieldException ex)
                {
                    LogError("Campo obbligatorio mancante per l'Esercizio {Id}.", objParametriServer, ex, esercizio.id_esercizio);
                    errori.Add(BuildErrore(esercizio, nameof(MissingMandatoryFieldException), ex.Message));
                }
                catch (DatabaseQueryException ex)
                {
                    LogError("Query GIS fallita per l'Esercizio {Id}.", objParametriServer, ex, esercizio.id_esercizio);
                    errori.Add(BuildErrore(esercizio, nameof(DatabaseQueryException), ex.Message));
                }
                catch (Exception ex)
                {
                    LogError("Costruzione payload fallita per l'Esercizio {Id}.", objParametriServer, ex, esercizio.id_esercizio);
                    errori.Add(BuildErrore(esercizio, nameof(PayloadConstructionException), ex.Message));
                }
            }

            LogInformation(
                "AssemblyPayloadRischiMeteo completato – Successi: {Ok}, Errori: {Err}",
                objParametriServer, null, payloads.Count, errori.Count);

            return new CostruzionePayloadRischiMeteoOutput
            {
                Payloads       = payloads,
                Errori         = errori,
                TotaleEsercizi = input.EserciziSelezionati.Count
            };
        }

        // ────────────────────────────────────────────────────────────────────
        // Costruzione payload per un singolo Esercizio
        // ────────────────────────────────────────────────────────────────────

        private async Task<AssessRiskRequest> BuildPayloadAsync(
            EsercizioRischiMeteoInput esercizio,
            List<DataRow>? righeSemina,
            IGisClusterConfig gisClusterConfigDAL,
            IImpiantiRischiMeteoDAL impiantiDAL,
            List<string> avvisi,
            AgronicaCoreParametriServer objParametriServer)
        {
            // Seleziona la riga di semina più recente (Data_Movimento MAX)
            var rigaSemina = righeSemina?
                .Where(r => r["Data_Movimento"] != DBNull.Value)
                .OrderBy(r => Convert.ToDateTime(r["Data_Movimento"]))
                .FirstOrDefault();

            int? vegCod = esercizio.cod_specie;
            int? culCod = esercizio.cod_varieta;

            // Fallback DAL Impianti se non disponibile da Agenda
            if (vegCod is null && esercizio.id_impianto > 0)
            {
                ImpiantoRischiMeteoEntity? impiantoFallback;
                try
                {
                    impiantoFallback = await impiantiDAL.GetImpiantoAsync(
                        esercizio.piva_azienda, esercizio.sa_cod, esercizio.id_appezzamento,
                        esercizio.id_impianto, objParametriServer);
                }
                catch (Exception ex)
                {
                    throw new DatabaseQueryException(
                        esercizio.id_esercizio.ToString(), "Fallback query Reg_Impianti fallita.", ex);
                }

                vegCod = impiantoFallback?.VegCod;
                culCod = impiantoFallback?.CulCod;
            }

            if (vegCod is null)
                throw new MissingMandatoryFieldException("codiceSpecie", esercizio.id_esercizio.ToString());

            // Normalizza: Veg_Cod "0" emesso da ISNULL(sv.Veg_Cod,0) → null
            if (vegCod == 0) vegCod = null;
            if (vegCod is null)
                throw new MissingMandatoryFieldException("codiceSpecie", esercizio.id_esercizio.ToString());

            if (culCod == 0) culCod = null;

            // ---- Data semina/trapianto ----
            DateTime? dataSemina = null;
            if (rigaSemina is not null && rigaSemina["Data_Movimento"] != DBNull.Value)
                dataSemina = Convert.ToDateTime(rigaSemina["Data_Movimento"]);

            // ---- validita_inizio_esercizio (fallback per finestraEventi.start) ----
            DateTime? validitaInizio = DateTime.Parse(esercizio.data_inizio_esercizio);

            // ---- finestraEventi ----
            var (startStr, endStr, dateWarning) = CalcolaFinestraEventi(
                dataSemina, validitaInizio, esercizio.id_esercizio.ToString());
            if (dateWarning is not null)
                avvisi.Add(dateWarning);

            // ---- GIS geometry ----
            string? poligonoWkt;
            string? centroideWkt;
            try
            {
                poligonoWkt  = await gisClusterConfigDAL.GetPoligonoWktAsync(
                    esercizio.piva_azienda, esercizio.sa_cod, esercizio.id_appezzamento, objParametriServer);
                //centroideWkt = await gisGeometriaDAL.GetCentroideWktAsync(
                //           esercizio.piva_azienda, esercizio.sa_cod, esercizio.id_appezzamento, objParametriServer);
                centroideWkt = await gisClusterConfigDAL.GetFirstPointCentroideWktAsync(poligonoWkt);
            }
            catch (Exception ex)
            {
                throw new DatabaseQueryException(
                    esercizio.id_esercizio.ToString(), "Query GIS fallita.", ex);
            }

            if (poligonoWkt is null)
            {
                avvisi.Add("Poligono non disponibile per l'appezzamento.");
                centroideWkt = null;
            }
            else
            {
                ValidaWkt("poligonoWkt", poligonoWkt, "POLYGON");
                if (centroideWkt is not null)
                    ValidaWkt("centroideWkt", centroideWkt, "POINT");
            }

            return new AssessRiskRequest
            {
                finestra_eventi = new FinestraEventiRequest
                {
                    start = startStr,
                    end   = endStr
                },
                coltura = new ColturaRequest
                {
                    codifica       = CodificaFissa,
                    codice_specie   = vegCod,
                    codice_cultivar = culCod
                },
                geo_impianto = new GeoImpiantoRequest
                {
                    poligono_wkt  = poligonoWkt,
                    centroide_wkt = centroideWkt,
                    epsg         = EpsgFisso
                },
                data_semina_o_trapianto   = dataSemina?.ToString("yyyy-MM-dd"),
                osservazione_fenologica = null,
                seleziona_avversita     = new List<string>(),
                dettaglio_eventi        = false
            };
        }

        // ────────────────────────────────────────────────────────────────────
        // Helpers – finestraEventi
        // ────────────────────────────────────────────────────────────────────

        private static (string start, string end, string? warning) CalcolaFinestraEventi(
            DateTime? dataSemina,
            DateTime? validitaInizio,
            string idEsercizio)
        {
                    // Mese e giorno di inizio dell'Annata Agraria italiana (1 novembre dell'anno precedente).
        const int MeseInizioAnnataAgraria = 11;
        const int GiornoInizioAnnataAgraria = 11;
        DateTime startDate;

            var sogliaCorrenteAnno = new DateTime(DateTime.Now.Year, MeseInizioAnnataAgraria, GiornoInizioAnnataAgraria);
            var inizioAnnataAgraria = DateTime.Now >= sogliaCorrenteAnno
                ? sogliaCorrenteAnno
                : sogliaCorrenteAnno.AddYears(-1);
     

            if (dataSemina.HasValue)
            {
                startDate = dataSemina.Value.Date;
            }
            else
            {
                startDate = validitaInizio.HasValue
                    ? (validitaInizio.Value.Date > inizioAnnataAgraria
                        ? validitaInizio.Value.Date
                        : inizioAnnataAgraria)
                    : inizioAnnataAgraria;
            }

            var endDate = DateTime.UtcNow.Date.AddDays(GiorniFinestraFine);
            string? warning = null;

            if (startDate > endDate)
            {
                (startDate, endDate) = (endDate, startDate);
                warning = $"[Esercizio {idEsercizio}] finestraEventi.start > end: date scambiate.";
            }

            return (startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"), warning);
        }

        // ────────────────────────────────────────────────────────────────────
        // Helpers – validazione WKT
        // ────────────────────────────────────────────────────────────────────

        private static void ValidaWkt(string campo, string wkt, string prefissoAtteso)
        {
            if (!wkt.TrimStart().StartsWith(prefissoAtteso, StringComparison.OrdinalIgnoreCase))
                throw new InvalidWktFormatException(campo, wkt);
        }

        // ────────────────────────────────────────────────────────────────────
        // Helpers – estrazione dati da DataRow
        // ────────────────────────────────────────────────────────────────────

        private static int? Estrai(DataRow? row, string colonna)
        {
            if (row is null) return null;
            if (!row.Table.Columns.Contains(colonna)) return null;
            return row[colonna] == DBNull.Value ? null : Convert.ToInt32(row[colonna]);
        }

        private static ErroreCostruzioneRischiMeteo BuildErrore(
            EsercizioRischiMeteoInput esercizio, string tipoErrore, string messaggio)
            => new()
            {
                Esercizio  = esercizio,
                TipoErrore = tipoErrore,
                Messaggio  = messaggio
            };
    }
}
