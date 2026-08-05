using AgronicaNetCore.Agenda.DAL.DataLayer.Agenda;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Impresa;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SostenibilitaCO2.BIZ.Exceptions;
using AgronicaNetCore.SostenibilitaCO2.BIZ.Models;
using AgronicaNetCore.SostenibilitaCO2.DAL.DataLayer.Codifiche;
using AgronicaNetCore.SostenibilitaCO2.DAL.DataLayer.AnalisiTerreno;
using AgronicaNetCore.SostenibilitaCO2.DAL.DataLayer.RegImpiantiCodici;
using AgronicaNetCore.Gis.DAL.DataLayer.Gis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Serilog.Context;
using System.Data;
using System.Globalization;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiImpostazioni;
using AgronicaCoreDTOStd.InData;

namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Services.AssemblyPayloadCo2
{
    /// <summary>
    /// Implementazione della business logic di assembly del payload M4 (DS03-BL + DS04-BL).
    /// Costruisce l'intero payload M4: root, aziende[], appezzamenti[], impianti[] e operazioni[].
    /// <para>
    /// Fase DS03: root (codice/tipo raggruppamento) e aziende[] (id_azienda, campagna, centroide, consumi).
    /// Fase DS04: appezzamenti[], impianti[] e operazioni colturali (fertilizzanti, agrofarmaci, sementi, raccolte).
    /// </para>
    /// Riferimento spec: DS03-BL AssemblyPayloadM4FilieraAzienda, DS04-BL AssemblyPayloadM4AppezzamentoImpianto,
    /// DS04.2-BL Flusso chiamate.
    /// </summary>
    public class AssemblyPayloadCo2FilieraAziendaService
        : BaseServiceSostenibilitaCO2Biz, IAssemblyPayloadCo2FilieraAziendaService
    {
        private const string DefaultNazione = "IT";
        private const string TipoRaggruppamentoFisso = "FILIERA-COLTURA-ANNO";

        private readonly IUtentiImpostazioni _utentiImpostazioni;

        public AssemblyPayloadCo2FilieraAziendaService(
            IServiceProvider provider,
            IStringLocalizer<Resources.Messages> localizer)
            : base(provider, localizer)
        {
            _utentiImpostazioni = _serviceProvider.GetRequiredService<IUtentiImpostazioni>();
        }

        /// <inheritdoc/>
        public async Task<PayloadCo2Root> AssembleAsync(
            AssemblyPayloadCo2Input input,
            AgronicaCoreParametriUtenti objParametriUtenti,
            AgronicaCoreParametriServer objParametriServer)
        {
            ArgumentNullException.ThrowIfNull(input);
            ArgumentNullException.ThrowIfNull(input.Perimetro);
            ArgumentNullException.ThrowIfNull(input.Carburanti);
            ArgumentNullException.ThrowIfNull(input.Energia);
            if (input.Esercizi is null || input.Esercizi.Count == 0)
                throw new ArgumentException("Specificare almeno un codice esercizio.", nameof(input));

            var logPath = Path.Combine(
                objParametriServer.LogDirectory,
                Path.GetFileNameWithoutExtension(objParametriServer.LogFileName));

            using (LogContext.PushProperty("LogPath", logPath))
            {
                LogInformation(
                    "AssemblyPayloadSostenibilitaCo2 avviato Filiera: {Filiera}, Anno: {Anno}, Modalita: {Modalita}, Aziende: {Count}",
                    objParametriServer, null, input.Perimetro.Filiera, input.Perimetro.Anno, input.Perimetro.Modalita, input.Perimetro.Aziende.Count);
            }

            // ------------------------------------------------------------
            // FASE DS03 --> Root + Aziende (id, campagna, centroide, consumi)
            // ------------------------------------------------------------

            var imprese = _serviceProvider.GetRequiredService<IImpresa>();

            var cuaaFiliera = await imprese.CuaaFromPivaAsync(input.Perimetro.Filiera, objParametriServer);
            if (string.IsNullOrWhiteSpace(cuaaFiliera))
                throw new DataNotFoundException($"CUAA non trovato per la filiera PIVA '{input.Perimetro.Filiera}'.");

            var payload = new PayloadCo2Root
            {
                codice_raggruppamento = BuildCodiceRaggruppamento(input, cuaaFiliera),
                tipo_raggruppamento = TipoRaggruppamentoFisso,
                aziende = new List<AziendaPayload>()
            };

            //var aziendeDAL = _serviceProvider.GetRequiredService<IAziendeDAL>();

            foreach (var piva in input.Perimetro.Aziende)
            {
                var cuaaAzienda = await imprese.CuaaFromPivaAsync(piva, objParametriServer);
                if (string.IsNullOrWhiteSpace(cuaaAzienda))
                    throw new DataNotFoundException($"CUAA non trovato per la PIVA '{piva}'.");

                var (lat, lon) = await imprese.GetCoordinateCentroideAsync(piva, objParametriServer);
                var centroideAzienda = $"POINT({lon.ToString(System.Globalization.CultureInfo.InvariantCulture)} {lat.ToString(System.Globalization.CultureInfo.InvariantCulture)})";
                var nazioneAzienda = await imprese.GetNazioneAsync(piva, objParametriServer);

                if (lat == 0m && lon == 0m)
                {
                    using (LogContext.PushProperty("LogPath", logPath))
                    {
                        LogWarning(
                            "Centroide non disponibile per PIVA {Piva}: coordinate (0, 0). Usato POINT(0 0).",
                            objParametriServer, new CentroideInvalidoException(piva), piva);
                    }
                }

                if (string.IsNullOrWhiteSpace(nazioneAzienda))
                {
                    nazioneAzienda = DefaultNazione;

                    using (LogContext.PushProperty("LogPath", logPath))
                    {
                        LogWarning(
                            "Nazione non disponibile per PIVA {Piva}. Usato default {NazioneDefault}.",
                            objParametriServer, new NazioneNonTrovataException(piva, nazioneAzienda), piva, nazioneAzienda);
                    }
                }

                payload.aziende.Add(new AziendaPayload
                {
                    id_azienda = cuaaAzienda,
                    piva_azienda = piva,
                    campagna = input.Perimetro.Anno,
                    centroide = centroideAzienda,
                    nazione = nazioneAzienda,
                    consumi = BuildConsumiPayload(piva, input.Carburanti, input.Energia),
                    appezzamenti = new List<AppezzamentoPayload>()
                });
            }

            // ------------------------------------------------------------
            // FASE DS04 --> Appezzamenti, Impianti, Operazioni colturali
            // ------------------------------------------------------------

            var agenda = _serviceProvider.GetRequiredService<IAgenda>();
            var agendaCO2DAL = _serviceProvider.GetRequiredService<ICodificheDAL>();
            var analisiTerrenoDAL = _serviceProvider.GetRequiredService<IAnalisiTerrenoDAL>();
            var regImpiantiCodiciDAL = _serviceProvider.GetRequiredService<IRegImpiantiCodiciDAL>();
            var gisClusterDAL = _serviceProvider.GetRequiredService<IGisClusterConfig>();

            var lavCodFertilizzanti = new List<int>(Common.OperazioniFertilizzazione);
            var lavCodTrattamenti = new List<int>(Common.OperazioniTrattamento);
            var lavCodSemina = new List<int>(Common.OperazioniSeminaTrapianto);
            var lavCodRaccolta = new List<int>(Common.OperazioniRaccolta);
            var lavCodTutteOperazioni = new List<int> { 0 };

            var dataInizioEFine = await _utentiImpostazioni.CropYearAsync(new DateTime(input.Perimetro.Anno, 1, 1), objParametriUtenti, objParametriServer);

            var dataInizioAnnataAgraria = dataInizioEFine.DataInizio;
            var dataFineAnnataAgraria = dataInizioEFine.DataFine;

            // Lettura completa operazioni: nessun filtro lav_cod (0 = tutti i valori).
            var dtImpiantiAll = await ChiamaLeggiImpiantiAsync(agenda, input.Esercizi, lavCodTutteOperazioni, objParametriServer);

            // Filtra: solo righe con Data_Movimento compresa nell'anno di campagna
            //var dtImpiantiFert = FiltraPerDataMovimento(dtImpiantiFertAll, dataInizioAnnataAgraria, dataFineAnnataAgraria);
            //var dtImpiantiTratt = FiltraPerDataMovimento(dtImpiantiTrattAll, dataInizioAnnataAgraria, dataFineAnnataAgraria);
            //var dtImpiantiSem = FiltraPerDataMovimento(dtImpiantiSemAll, dataInizioAnnataAgraria, dataFineAnnataAgraria);
            //var dtImpiantiRacc = FiltraPerDataMovimento(dtImpiantiRaccAll, dataInizioAnnataAgraria, dataFineAnnataAgraria);

            var dtProdottiAll = await ChiamaLeggiProdottiAsync(agenda, input.Esercizi, lavCodTutteOperazioni, false, objParametriServer);

            // Decodifica Udm_Cod ? Udm_Cod_Esterno (Codifica_UnitaMisura_SistemiEsterni, necessaria per SementePayload.UnitaDiMisura)
            var udmCodDistinti = dtProdottiAll
                .AsEnumerable()
                .Where(r => r["Udm_Cod"] != DBNull.Value)
                .Select(r => Convert.ToInt32(r["Udm_Cod"]))
                .Distinct()
                .ToList();
            var udmEsternoByCode = await agendaCO2DAL.GetUdmCodEsternoByUdmCodAsync(udmCodDistinti, objParametriServer);

            // tipo_operazione, data_operazione, resa_prevista



            var tuttiImpianti = dtImpiantiAll;
            var tuttiProdotti = dtProdottiAll;

            // Raccoglie i lavCod distinti presenti in tuttiImpianti ed esegue UNA sola query DB
            var lavCodDistinti = tuttiImpianti
                .AsEnumerable()
                .Select(r => Convert.ToInt32(r["Lav_Cod"]))
                .Distinct()
                .ToList();

            var tipoOperazioneByLavCod = await agendaCO2DAL.GetTipoOperazioneByLavCodAsync(
              lavCodDistinti, objParametriServer);

            var prodottiByIdAgenda = tuttiProdotti
                .AsEnumerable()
                .GroupBy(r => Convert.ToInt32(r["Id_Agenda"]))
                .ToDictionary(g => g.Key, g => g.ToList());

            var appezzamentiPerAzienda = tuttiImpianti
                .AsEnumerable()
                .GroupBy(r => r["Piva"]?.ToString() ?? string.Empty);

            //var aziende = tuttiImpianti
            //    .AsEnumerable()
            //    .Select(r => r["Piva"]?.ToString() ?? string.Empty)
            //    .Distinct()
            //    .ToList();

            foreach (var gruppoAzienda in appezzamentiPerAzienda)
            {
                var piva = gruppoAzienda.Key;
                var aziendaPayload = payload.aziende
                    .FirstOrDefault(a => string.Equals(a.piva_azienda, piva, StringComparison.OrdinalIgnoreCase));

                if (aziendaPayload is null)
                {
                    using (LogContext.PushProperty("LogPath", logPath))
                    {
                        LogWarning("Azienda {Piva} nei dati impianti ma non nel payload DS03. Ignorata.", objParametriServer, null, piva);
                    }
                    continue;
                }

                var appezzamentiPayload = new List<AppezzamentoPayload>();

                var gruppiAppezzamento = gruppoAzienda
                    .GroupBy(r => $"{r["Piva"]}|{r["Sa_Cod"]}|{r["Appezza"]}");

                foreach (var gruppoApp in gruppiAppezzamento)
                {
                    var primeRigaApp = gruppoApp.First();
                    var saCod = primeRigaApp["Sa_Cod"] != DBNull.Value ? Convert.ToInt32(primeRigaApp["Sa_Cod"]) : 0;
                    var idAppezzamento = primeRigaApp["Appezza"] != DBNull.Value ? Convert.ToInt32(primeRigaApp["Appezza"]) : -1;
                    var area_ha = primeRigaApp["SUP_APP"] != DBNull.Value ? Convert.ToDecimal(primeRigaApp["SUP_APP"]) : (decimal?)null;

                    var poligono = await gisClusterDAL.GetPoligonoWktAsync(piva, saCod, idAppezzamento, objParametriServer)
                        ?? "POLYGON EMPTY";
                    var centroide = await gisClusterDAL.GetFirstPointCentroideWktAsync(poligono);
                    //var centroide = await gisClusterDAL.GetCentroideWktAsync(piva, saCod, idAppezzamento, objParametriServer)
                    //    ?? "POINT(0 0)";

                    if (centroide == "POINT(0 0)")
                    {
                        using (LogContext.PushProperty("LogPath", logPath))
                        {
                            LogWarning(
                                "Appezzamento {Id}: centroide GIS non disponibile. Usato POINT(0 0).",
                                objParametriServer, null, $"{piva}|{saCod}|{idAppezzamento}");
                        }
                    }

                    var soEntity = await analisiTerrenoDAL.GetSostanzaOrganicaAsync(piva, saCod, idAppezzamento, objParametriServer);

                    var appPayload = new AppezzamentoPayload
                    {
                        id_appezzamento = BuildIdAppezzamento(piva, saCod.ToString(), idAppezzamento.ToString()),
                        centroide = centroide,
                        perc_sostanza_organica = soEntity?.SostanzaOrganica,
                        anno_campagna = input.Perimetro.Anno,
                        area_ha = area_ha
                    };

                    var gruppiImpianto = gruppoApp
                        .GroupBy(r => $"{r["Piva"]}|{r["Sa_Cod"]}|{r["Appezza"]}|{r["Id_Destinazione"]}|{r["progetto_cod"]}");

                    foreach (var gruppoImp in gruppiImpianto)
                    {
                        var primeRigaImp = gruppoImp.First();
                        var idReg = primeRigaImp["Id_Destinazione"]?.ToString() ?? string.Empty;
                        var progettoCod = primeRigaImp["progetto_cod"]?.ToString() ?? string.Empty;
                        var idImpianto = BuildIdImpianto(piva, saCod.ToString(), idAppezzamento.ToString(), idReg, progettoCod);

                        var supImpValue = primeRigaImp["sup_imp"];
                        decimal areaHa = 0m;

                        if (supImpValue != null && supImpValue != DBNull.Value)
                        {
                            try
                            {
                                areaHa = Convert.ToDecimal(supImpValue);
                            }
                            catch
                            {
                                areaHa = 0m;
                            }
                        }

                        //if (!decimal.TryParse(primeRigaImp["sup_imp"]?.ToString(),
                        //NumberStyles.Number, CultureInfo.InvariantCulture, out var areaHa)
                        //|| areaHa <= 0m)

                        if (areaHa <= 0m)
                        {
                            using (LogContext.PushProperty("LogPath", logPath))
                            {
                                LogWarning(
                                    "Impianto {Id} omesso: area_ha assente o zero.", objParametriServer, new DataInconsistencyException(idImpianto), idImpianto);
                            }
                            continue;
                        }

                        // Data inizio effettiva = max(dataInizioAnnataAgraria, validita_inizio_esercizio)
                        var dataInizioEsercizio = primeRigaImp["validita_inizio_esercizio"] != DBNull.Value && primeRigaImp["validita_inizio_esercizio"] != null
                            ? Convert.ToDateTime(primeRigaImp["validita_inizio_esercizio"])
                            : DateTime.MinValue;

                        var dataFineEsercizio = primeRigaImp["validita_fine_esercizio"] != DBNull.Value && primeRigaImp["validita_fine_esercizio"] != null
                            ? Convert.ToDateTime(primeRigaImp["validita_fine_esercizio"])
                            : DateTime.MaxValue;

                        var dataInizioEffettiva = dataInizioEsercizio > dataInizioAnnataAgraria
                            ? dataInizioEsercizio
                            : dataInizioAnnataAgraria;

                        var dataInizioImpianto = primeRigaImp["validita_inizio_impianto"] != DBNull.Value
                            && DateTime.TryParse(primeRigaImp["validita_inizio_impianto"]?.ToString(), out var validitaInizioImpianto)
                                ? validitaInizioImpianto
                                : (DateTime?)null;

                        var progCodInt = int.TryParse(progettoCod, out var pc) ? pc : 0;
                        var esercizioCorrente = input.Esercizi.FirstOrDefault(e => e.id_esercizio == progCodInt);

                        var idRegInt = int.TryParse(idReg, out var parsedIdReg) ? parsedIdReg : 0;
                        int? annoImpianto = null;
                        if (idRegInt > 0)
                        {
                            annoImpianto = await regImpiantiCodiciDAL.GetAnnoImpiantoAsync(
                                piva, saCod, idAppezzamento, idRegInt, objParametriServer);

                            if (annoImpianto is null && dataInizioImpianto.HasValue)
                            {
                                annoImpianto = dataInizioImpianto.Value.Year;
                            }

                            if (annoImpianto is null)
                            {
                                using (LogContext.PushProperty("LogPath", logPath))
                                {
                                    LogWarning(
                                        "Anno impianto non trovato in Reg_Impianti_Codici e validita_inizio non disponibile per impianto {Id}. Campo omesso dal payload.",
                                        objParametriServer, null, idImpianto);
                                }
                            }
                        }

                        var culCod = primeRigaImp["Cul_Cod"] != DBNull.Value ? Convert.ToInt32(primeRigaImp["Cul_Cod"]) : 0;
                        string? idFinalita = null;
                        string? idDestinazioneUso = null;

                        if (culCod != 0)
                        {
                            var grfiCod = primeRigaImp.Table.Columns.Contains("grfi_cod") && primeRigaImp["grfi_cod"] != DBNull.Value
                                ? Convert.ToInt32(primeRigaImp["grfi_cod"])
                                : 0;

                            if (grfiCod > 0)
                                idFinalita = grfiCod.ToString();
                        }
                        else if (idRegInt > 0)
                        {
                            var idDestinazioneUsoCod = await regImpiantiCodiciDAL.GetIdDestinazioneUsoAsync(
                                piva, saCod, idAppezzamento, idRegInt, objParametriServer);

                            if (idDestinazioneUsoCod.HasValue)
                                idDestinazioneUso = idDestinazioneUsoCod.Value.ToString();
                        }

                        var impPayload = new ImpiantoPayload
                        {
                            id_impianto = idImpianto,
                            id_coltura = primeRigaImp["Veg_Cod"]?.ToString() ?? string.Empty,
                            tipo_id_coltura = "Profitosan",
                            area_ha = areaHa,
                            anno_impianto = annoImpianto,
                            id_finalita = idFinalita,
                            id_destinazione_uso = idDestinazioneUso,
                            resa_prevista_kg_ha = Convert.ToDecimal(primeRigaImp["Produzione_Prevista"]),
                            data_inizio_ciclo = DateOnly.FromDateTime(dataInizioEsercizio),
                            data_fine_ciclo = DateOnly.FromDateTime(dataFineEsercizio),
                            Cul_Cod = culCod,
                            Stato = esercizioCorrente?.nazione,
                            Regione = esercizioCorrente?.istat_reg
                        };

                        var idAgendeImpianto = gruppoImp
                            .Select(r => Convert.ToInt32(r["Id_Agenda"]))
                            .Distinct()
                            .ToList();

                        foreach (var idAgenda in idAgendeImpianto)
                        {
                            if (idAgenda == 0)
                                continue;

                            var supOp = 0m;
                            var rigaImp = gruppoImp.FirstOrDefault(r => Convert.ToInt32(r["Id_Agenda"]) == idAgenda);
                            if (rigaImp is not null)
                                //decimal.TryParse(rigaImp["sup_trattata"]?.ToString(), NumberStyles.Number, CultureInfo.InvariantCulture, out supOp);
                                supOp = rigaImp["sup_trattata"] != DBNull.Value ? Convert.ToDecimal(rigaImp["sup_trattata"]) : 0m;
                          
                            // Recupera il Lav_Cod dalla riga e cerca il tipo operazione nel dizionario
                            var lavCodRiga = rigaImp is not null ? Convert.ToInt32(rigaImp["Lav_Cod"]) : 0;
                            if (!tipoOperazioneByLavCod.TryGetValue(lavCodRiga, out var tipoOp)
                                || string.IsNullOrWhiteSpace(tipoOp))
                            {
                                using (LogContext.PushProperty("LogPath", logPath))
                                {
                                    LogWarning(
                                        "Operazione {IdAgenda} esclusa: Lav_Cod {LavCod} senza mapping in Codifica_Operazioni_SistemiEsterni.",
                                        objParametriServer, null, idAgenda, lavCodRiga);
                                }
                                continue;
                            }

                            var isOperazioneDettaglio = lavCodFertilizzanti.Contains(lavCodRiga)
                                                       || lavCodTrattamenti.Contains(lavCodRiga)
                                                       || lavCodSemina.Contains(lavCodRiga)
                                                       || lavCodRaccolta.Contains(lavCodRiga);

                            // Data_Movimento e' gia' disponibile nella DataRow di LeggiImpianti
                            var dataOp = rigaImp is not null && rigaImp["Data_Movimento"] != DBNull.Value ? Convert.ToDateTime(rigaImp["Data_Movimento"]) : default;

                            // Esclude operazioni con Data_Movimento antecedente alla data inizio effettiva
                            if (rigaImp is not null && rigaImp["Data_Movimento"] != DBNull.Value && Convert.ToDateTime(rigaImp["Data_Movimento"]) < dataInizioEffettiva)
                            {
                                using (LogContext.PushProperty("LogPath", logPath))
                                {
                                LogDebug(
                                    "Operazione {IdAgenda} ignorata: Data_Movimento {DataMovimento:d} < inizio effettivo esercizio {DataInizioEffettiva:d}.",
                                    objParametriServer, null, idAgenda, Convert.ToDateTime(rigaImp["Data_Movimento"]), dataInizioEffettiva);
                                }
                                continue;
                            }

                            var opPayload = new OperazionePayload
                            {
                                id_operazione = idAgenda.ToString(),
                                tipo_operazione = tipoOp,
                                data_operazione = DateOnly.FromDateTime(dataOp),
                                superficie_operazione_ha = isOperazioneDettaglio ? supOp : null
                            };

                            if (isOperazioneDettaglio && prodottiByIdAgenda.TryGetValue(idAgenda, out var righeProdotti))
                            {
                                foreach (var rp in righeProdotti)
                                {
                                    var elemCod = rp.Table.Columns.Contains("Elem_Cod")
                                        ? Convert.ToInt32(rp["Elem_Cod"])
                                        : 0;
                                    var qtaDett = rp.Table.Columns.Contains("Qta_Dett") && rp["Qta_Dett"] != DBNull.Value
                                        ? Convert.ToDecimal(rp["Qta_Dett"])
                                        : 0m;

                                    int udmCodRp;

                                    switch (elemCod)
                                    {
                                        case ELEM_COD.FERTILIZZANTI:
                                            if (!lavCodFertilizzanti.Contains(lavCodRiga))
                                                break;

                                            // Se l'UdM e' volume (litri), si applica conversione 1 litro ? 1 Kg (fattore 1:1).
                                            // Per altre UdM non di volume la quantita viene usata invariata.
                                            var origineFertilizzante = string.Empty;
                                            if (lavCodRiga == LAV_COD.LAVCOD_DISTRIBUZIONE_AMMENDANTI)
                                            {
                                                origineFertilizzante = "FERT_ORIGIN_2";
                                            }
                                            else
                                            {
                                                origineFertilizzante = "FERT_ORIGIN_1";
                                            }
                                            opPayload.fertilizzanti.Add(BuildFertilizzante(rp, qtaDett, origineFertilizzante));
                                            break;
                                        case ELEM_COD.FORMULATI:
                                            if (!lavCodTrattamenti.Contains(lavCodRiga))
                                                break;

                                            // Se l'UdM e' volume (litri), si applica conversione 1 litro ? 1 Kg (fattore 1:1).
                                            // Per altre UdM non di volume la quantita viene usata invariata.
                                            opPayload.agrofarmaci.Add(new AgrofarmacоPayload
                                            {
                                                codice_agrofarmaco = rp["Pro_Cod"]?.ToString() ?? string.Empty,
                                                quantita_kg = qtaDett,
                                                principi_attivi = ParsePrincipiAttivi(rp["PrincipiAttivi"]?.ToString())
                                            });
                                            break;
                                        case ELEM_COD.SEMENTI:
                                            if (!lavCodSemina.Contains(lavCodRiga))
                                                break;

                                            // Sementi accettate solo se l'UdM ha transcodifica esterna
                                            // in Codifica_UnitaMisura_SistemiEsterni (Sistema_Cod = 10).
                                            udmCodRp = rp.Table.Columns.Contains("Udm_Cod") && rp["Udm_Cod"] != DBNull.Value
                                                ? Convert.ToInt32(rp["Udm_Cod"]) : 0;

                                            if (!udmEsternoByCode.TryGetValue(udmCodRp, out var udmEsterno)
                                                || string.IsNullOrWhiteSpace(udmEsterno))
                                                break;

                                            opPayload.sementi.Add(new SementePayload
                                            {
                                                codice_semente = rp.Table.Columns.Contains("Veg_Cod") && rp["Veg_Cod"] != DBNull.Value
                                                                    ? rp["Veg_Cod"].ToString() : string.Empty,
                                                quantita = qtaDett,
                                                uom = udmEsterno
                                            });
                                            break;
                                        case ELEM_COD.TRASFORMATI_VEGETALI:
                                            if (!lavCodRaccolta.Contains(lavCodRiga))
                                                break;

                                            // Prodotti raccolti accettati solo se l'UdM sorgente = massa (Kg): altrimenti omessi.

                                            udmCodRp = rp.Table.Columns.Contains("Udm_Cod") && rp["Udm_Cod"] != DBNull.Value
                                                ? Convert.ToInt32(rp["Udm_Cod"]) : 0;

                                            if (udmCodRp != (int)TipiEnumerativi.Enum_UnitaMisura.KG)
                                                break;

                                            // Propaga Elem_Cod e Mat_Cod sull'impianto per la persistenza nel lookup colture
                                            if (impPayload.Elem_Cod == null && rp.Table.Columns.Contains("Elem_Cod") && rp["Elem_Cod"] != DBNull.Value)
                                                impPayload.Elem_Cod = Convert.ToInt32(rp["Elem_Cod"]);

                                            if (impPayload.Mat_Cod == null && rp.Table.Columns.Contains("Mat_Cod") && rp["Mat_Cod"] != DBNull.Value)
                                                impPayload.Mat_Cod = Convert.ToInt32(rp["Mat_Cod"]);

                                            if (impPayload.Lotto == null && rp.Table.Columns.Contains("Lotto") && rp["Lotto"] != DBNull.Value)
                                                impPayload.Lotto = rp["Lotto"]?.ToString();

                                            opPayload.prodotti_raccolti.Add(new ProdottoRaccoltоPayload
                                            {
                                                quantita_kg = qtaDett,
                                                specie = rp.Table.Columns.Contains("Veg_Cod") && rp["Veg_Cod"] != DBNull.Value && Convert.ToInt32(rp["Veg_Cod"]) != 0
                                                    ? rp["Veg_Cod"].ToString()
                                                    : null
                                            });
                                            break;
                                    }
                                }
                            }

                            impPayload.operazioni.Add(opPayload);
                        }

                        appPayload.impianti.Add(impPayload);
                    }

                    if (appPayload.impianti.Count == 0)
                        throw new IncompletePerimeterException(idAppezzamento.ToString());

                    appezzamentiPayload.Add(appPayload);
                }

                aziendaPayload.appezzamenti = appezzamentiPayload;
            }

            ValidatePayloadStructure(payload);

            using (LogContext.PushProperty("LogPath", logPath))
            {
                LogInformation(
                    "AssemblyPayloadSostenibilitaCo2 completato Aziende: {Count}", objParametriServer, null, payload.aziende.Count);
            }

            return payload;
        }

        // --------------------------------------------------------------------------
        // Helpers --> DS03: codice raggruppamento, centroide azienda, consumi
        // --------------------------------------------------------------------------

        private static string BuildCodiceRaggruppamento(AssemblyPayloadCo2Input input, string cuaaFiliera)
        {
            if (input.Perimetro.Modalita.Equals("Colture", StringComparison.OrdinalIgnoreCase))
            {
                var coltura = input.Perimetro.Colture.Count == 1 ? input.Perimetro.Colture[0] : "+";
                return $"{cuaaFiliera}|{coltura}|{input.Perimetro.Anno}";
            }
            return $"{cuaaFiliera}|*|{input.Perimetro.Anno}";
        }

        private static ConsumiPayload BuildConsumiPayload(string piva, List<CarburanteConsumo> carburanti, List<EnergiaConsumo> energia)
        {
            var carburantiPayload = carburanti
                .Where(c => c.Azienda.Equals(piva, StringComparison.OrdinalIgnoreCase))
                .Select(c => new CarburanteAltroPayload
                {
                    tipo_carburante = MappingCarburante(c.TipoCarburante),
                    quantita_carburante = c.Quantita,
                    unita_di_misura_carburante = MappingUdmCarburante(c.TipoCarburante)
                })
                .ToList();

            var elettricitaPayload = energia
                .Where(e => e.Azienda.Equals(piva, StringComparison.OrdinalIgnoreCase))
                .Select(e => new ElettricaPayload
                {
                    data_inizio = DateOnly.FromDateTime(e.DataInizio),
                    data_fine = DateOnly.FromDateTime(e.DataFine),
                    consumo_kwh = e.ConsumoKwh,
                    perc_rinnovabili = e.PercentualeRinnovabili
                })
                .ToList();

            return new ConsumiPayload
            {
                carburanti_altro = carburantiPayload,
                elettricita = elettricitaPayload
            };
        }

        private static void ValidatePayloadStructure(PayloadCo2Root payload)
        {
            if (string.IsNullOrWhiteSpace(payload.codice_raggruppamento))
                throw new PayloadStructureException(
                    "Campo obbligatorio 'codice_raggruppamento' è nullo o vuoto dopo l'assembly.");

            if (string.IsNullOrWhiteSpace(payload.tipo_raggruppamento))
                throw new PayloadStructureException(
                    "Campo obbligatorio 'tipo_raggruppamento' è nullo o vuoto dopo l'assembly.");

            if (payload.aziende.Count == 0)
                throw new PayloadStructureException(
                    "Il payload SostenibilitaCo2 non contiene alcuna azienda. Il perimetro non ha prodotto risultati validi.");

            foreach (var azienda in payload.aziende)
            {
                if (string.IsNullOrWhiteSpace(azienda.piva_azienda))
                    throw new PayloadStructureException(
                        "Campo obbligatorio 'piva_azienda' nullo o vuoto in almeno un'azienda del payload.");
            }
        }

        // --------------------------------------------------------------------------
        // Helpers --> DS04: chiamate DAL Agenda
        // --------------------------------------------------------------------------

        private static Task<DataTable> ChiamaLeggiImpiantiAsync(
            IAgenda agenda,
            List<Esercizio> esercizi,
            List<int> lavCods,
            AgronicaCoreParametriServer objParametriServer)
            => agenda.LeggiImpiantiAsync(
                Id_Agenda: new List<int>(),
                objParametriServer: objParametriServer,
                createTempEsercizi: false,
                leggiFlagEsercizioChiuso: false,
                leggiIndirizzoAppezzamento: false,
                leggiGIS: true,
                Id_Esercizi: esercizi?.Select(e => e.id_esercizio).ToList(),
                Lav_cod: lavCods);

        private static Task<DataTable> ChiamaLeggiProdottiAsync(
            IAgenda agenda,
            List<Esercizio> esercizi,
            List<int> lavCods,
            bool leggiMateriePrime,
            AgronicaCoreParametriServer objParametriServer)
            => agenda.LeggiProdottiAsync(
                Id_Agenda: new List<int>(),
                leggiMateriePrime: leggiMateriePrime,
                leggiEsercizi: true,
                objParametriServer: objParametriServer,
                leggiFlagEsercizioChiuso: false,
                Id_Esercizi: esercizi?.Select(e => e.id_esercizio).ToList(),
                Lav_cod: lavCods);

        // --------------------------------------------------------------------------
        // Helpers --> DS04: costruzione oggetti payload
        // --------------------------------------------------------------------------

        private static string BuildIdAppezzamento(string piva, string saCod, string appezza)
            => $"{piva}|{saCod}|{appezza}";

        private static string BuildIdImpianto(string piva, string saCod, string appezza, string idReg, string progettoCod)
            => $"{piva}|{saCod}|{appezza}|{idReg}|{progettoCod}";

        private static FertilizzantePayload BuildFertilizzante(DataRow rp, decimal qtaDett, string origineFertilizzante)
        {
            var fert = new FertilizzantePayload { quantita_kg = qtaDett, origine_fertilizzante = origineFertilizzante };

            void AddMolecola(string nome, string colonna)
            {
                if (!rp.Table.Columns.Contains(colonna)) return;
                var rawVal = rp[colonna];
                if (rawVal == null || rawVal == DBNull.Value) return;
                decimal val;
                try { val = Convert.ToDecimal(rawVal, CultureInfo.InvariantCulture); }
                catch { return; }
                if (val == 0m) return;
                fert.molecole.Add(new MolecolaPayload { molecola = nome, titolo = val });
            }

            AddMolecola("N", "n");
            AddMolecola("P", "p");
            AddMolecola("K", "k");
            AddMolecola("Mg", "mg");
            AddMolecola("Cu", "cu");
            return fert;
        }

        /// <summary>
        /// Parsa il campo <c>Movimenti_dettagli.PrincipiAttivi</c> nel formato <c>cod§titolo|cod§titolo|...</c>
        /// e restituisce la lista di <see cref="PrincipioAttivoPayload"/>.
        /// </summary>
        private static List<PrincipioAttivoPayload> ParsePrincipiAttivi(string? raw)
        {
            var result = new List<PrincipioAttivoPayload>();
            if (string.IsNullOrWhiteSpace(raw))
                return result;

            foreach (var entry in raw.Split('|', StringSplitOptions.RemoveEmptyEntries))
            {
                var parts = entry.Split('§');
                if (parts.Length != 2) continue;
                if (!int.TryParse(parts[0].Trim(), out var codice)) continue;
                if (!decimal.TryParse(parts[1].Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var perc)) continue;
                result.Add(new PrincipioAttivoPayload { codice_principio_attivo = codice.ToString(), perc_principio_attivo = perc });
            }

            return result;
        }

        // --------------------------------------------------------------------------
        // Helpers --> merge DataTable e conversioni
        // --------------------------------------------------------------------------

        private static DataTable MergeDataTables(params DataTable[] tables)
        {
            var result = tables[0].Clone();
            foreach (var dt in tables)
            {
                foreach (DataRow row in dt.Rows)
                    result.ImportRow(row);
            }
            return result;
        }

        /// <summary>
        /// Restituisce una nuova <see cref="DataTable"/> contenente solo le righe
        /// il cui campo <c>Data_Movimento</c> è compreso nell'intervallo
        /// [<paramref name="dataInizio"/>, <paramref name="dataFine"/>] (estremi inclusi).
        /// Righe con valore null o DBNull vengono escluse.
        /// </summary>
        private static DataTable FiltraPerDataMovimento(DataTable source, DateTime dataInizio, DateTime dataFine)
        {
            var result = source.Clone(); // preserva la struttura delle colonne

            foreach (DataRow row in source.Rows)
            {
                var val = row["Data_Movimento"];
                if (val == null || val == DBNull.Value)
                    continue;

                var dataMovimento = Convert.ToDateTime(val);
                if (dataMovimento >= dataInizio && dataMovimento <= dataFine)
                    result.ImportRow(row);
            }

            return result;
        }

        private static string MappingCarburante(string tipoCarburante)
        {
            return tipoCarburante.ToUpper() switch
            {
                "GASOLIO AGRICOLO" => "5",
                "GASOLIO" => "5",
                "BENZINA" => "6",
                "BENZINA AGRICOLA" => "6",
                "GPL" => "2",
                "GAS METANO" => "3",
                "KEROSENE" => "16",
                "GASOLIO SERRA" => "5",
                _ => "5"
            };
        }

        private static string MappingUdmCarburante(string tipoCarburante)
        {
            return tipoCarburante.ToUpper() switch
            {
                "GASOLIO AGRICOLO" => "L",
                "GASOLIO" => "L",
                "BENZINA" => "L",
                "BENZINA AGRICOLA" => "L",
                "GPL" => "L",
                "GAS METANO" => "m3",
                "KEROSENE" => "L",
                "GASOLIO SERRA" => "L",
                _ => "L"
            };
        }
    }
}

