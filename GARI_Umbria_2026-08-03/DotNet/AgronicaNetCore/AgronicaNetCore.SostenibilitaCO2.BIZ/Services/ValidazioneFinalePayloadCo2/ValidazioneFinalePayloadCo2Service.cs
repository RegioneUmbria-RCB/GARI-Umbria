using AgronicaNetCore.SostenibilitaCO2.BIZ.Exceptions;
using AgronicaNetCore.SostenibilitaCO2.BIZ.Models;
using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Services.ValidazioneFinalePayloadCo2
{
    /// <summary>
    /// Implementazione della business logic di validazione finale del payload M4 (DS06-BL).
    /// Esegue validazione strutturale e semantica in-memory: campi obbligatori, cardinalità,
    /// range numerici, formato WKT centroidi e serializzabilità JSON.
    /// Nessun accesso a database — operazione puramente in-memory.
    /// Riferimento spec: DS06-BL ValidazioneFinalePayloadM4.
    /// </summary>
    public class ValidazioneFinalePayloadCo2Service : BaseServiceSostenibilitaCO2Biz, IValidazioneFinalePayloadCo2Service
    {
        // ----------------------------------------------------------------
        // Costanti spec DS06-BL — livelli gerarchici
        // ----------------------------------------------------------------
        private const string LivelloFiliera = "filiera";
        private const string LivelloAzienda = "azienda";
        private const string LivelloAppezzamento = "appezzamento";
        private const string LivelloImpianto = "impianto";
        private const string LivelloOperazione = "operazione";

        // ----------------------------------------------------------------
        // Costanti spec DS06-BL — tipi di errore
        // ----------------------------------------------------------------
        private const string TipoFieldNull = "FIELD_NULL";
        private const string TipoOutOfRange = "OUT_OF_RANGE";
        private const string TipoInvalidFormat = "INVALID_FORMAT";
        private const string TipoCardinality = "CARDINALITY_VIOLATION";
        private const string TipoDataInconsistency = "DATA_INCONSISTENCY";

        /// <summary>
        /// Regex per validazione WKT POINT: <c>POINT(longitude latitude)</c>.
        /// Gruppo 1 = longitudine, Gruppo 2 = latitudine.
        /// Riferimento spec: DS06-BL Regola 11.
        /// </summary>
        private static readonly Regex WktPointRegex = new(
            @"^POINT\((-?\d+(?:\.\d+)?)\s+(-?\d+(?:\.\d+)?)\)$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromSeconds(3));

        public ValidazioneFinalePayloadCo2Service(
            IServiceProvider provider,
            IStringLocalizer<Resources.Messages> localizer)
            : base(provider, localizer)
        {
        }

        /// <inheritdoc/>
        public ValidazioneFinalePayloadCo2Output Validate(PayloadCo2Root payload)
        {
            ArgumentNullException.ThrowIfNull(payload);

            // Regola 12: serializzabilità JSON — errore tecnico, non di validazione
            try
            {
                _ = JsonSerializer.Serialize(payload);
            }
            catch (Exception ex)
            {
                throw new SerializationException(
                    "Il payload SostenibilitaCo2 non è serializzabile a JSON valido. Richiede debug della fase di assembly.",
                    ex);
            }

            var errors = new List<ValidazioneErroreCo2>();

            ValidateTopLevel(payload, errors);

            // Regola 6: cardinalità aziende >= 1
            if (payload.aziende == null || payload.aziende.Count == 0)
            {
                errors.Add(MakeError(LivelloFiliera, TipoCardinality,
                    "$.aziende",
                    "L'array 'aziende' deve contenere almeno un elemento.",
                    payload.aziende?.Count.ToString() ?? "null"));
            }
            else
            {
                for (int i = 0; i < payload.aziende.Count; i++)
                    ValidateAzienda(payload.aziende[i], i, errors);
            }

            // Regola 13: unicità ID — non bloccante, solo warning log
            CheckIdUniqueness(payload);

            var isValid = errors.Count == 0;

            LogInformation(
                "Validazione finale payload SostenibilitaCo2 completata — Esito: {Esito}, Errori bloccanti: {Count}",
                null, null, isValid, errors.Count);

            return new ValidazioneFinalePayloadCo2Output
            {
                ValidazioneEsito = isValid,
                ErroriValidazione = errors,
                PayloadValidato = isValid ? payload : null
            };
        }

        // ----------------------------------------------------------------
        // Regola 1 — Campi obbligatori top-level
        // ----------------------------------------------------------------

        private void ValidateTopLevel(PayloadCo2Root payload, List<ValidazioneErroreCo2> errors)
        {
            if (string.IsNullOrWhiteSpace(payload.codice_raggruppamento))
                errors.Add(MakeError(LivelloFiliera, TipoFieldNull,
                    "$.codice_raggruppamento",
                    "Campo 'codice_raggruppamento' obbligatorio non valorizzato.",
                    payload.codice_raggruppamento));

            if (string.IsNullOrWhiteSpace(payload.tipo_raggruppamento))
                errors.Add(MakeError(LivelloFiliera, TipoFieldNull,
                    "$.tipo_raggruppamento",
                    "Campo 'tipo_raggruppamento' obbligatorio non valorizzato.",
                    payload.tipo_raggruppamento));
        }

        // ----------------------------------------------------------------
        // Regola 2 — Campi obbligatori Azienda + Regola 7 cardinalità appezzamenti
        // ----------------------------------------------------------------

        private void ValidateAzienda(AziendaPayload az, int azIdx, List<ValidazioneErroreCo2> errors)
        {
            var basePath = $"$.aziende[{azIdx}]";

            if (string.IsNullOrWhiteSpace(az.id_azienda))
                errors.Add(MakeError(LivelloAzienda, TipoFieldNull,
                    $"{basePath}.id_azienda",
                    "Campo 'id_azienda' obbligatorio non valorizzato.",
                    az.id_azienda));

            if (az.campagna <= 0)
                errors.Add(MakeError(LivelloAzienda, TipoFieldNull,
                    $"{basePath}.campagna",
                    "Campo 'campagna' obbligatorio non valorizzato (valore deve essere > 0).",
                    az.campagna.ToString()));

            ValidateCentroide(az.centroide, LivelloAzienda, $"{basePath}.centroide", errors);

            ValidateConsumi(az.consumi, basePath, errors);

            // Regola 7: cardinalità appezzamenti >= 1
            if (az.appezzamenti == null || az.appezzamenti.Count == 0)
            {
                errors.Add(MakeError(LivelloAzienda, TipoCardinality,
                    $"{basePath}.appezzamenti",
                    $"L'azienda '{az.id_azienda}' deve avere almeno un appezzamento.",
                    az.appezzamenti?.Count.ToString() ?? "null"));
            }
            else
            {
                for (int i = 0; i < az.appezzamenti.Count; i++)
                    ValidateAppezzamento(az.appezzamenti[i], azIdx, i, errors);
            }
        }

        private void ValidateConsumi(ConsumiPayload? consumi, string parentPath, List<ValidazioneErroreCo2> errors)
        {
            if (consumi == null)
            {
                errors.Add(MakeError(LivelloAzienda, TipoFieldNull,
                    $"{parentPath}.consumi",
                    "Campo 'consumi' obbligatorio è null.",
                    null));
                return;
            }

            if (consumi.carburanti_altro == null)
            {
                errors.Add(MakeError(LivelloAzienda, TipoFieldNull,
                    $"{parentPath}.consumi.carburanti_altro",
                    "Campo 'consumi.carburanti_altro' non deve essere null.",
                    null));
            }
            else
            {
                for (int i = 0; i < consumi.carburanti_altro.Count; i++)
                    ValidateCarburante(consumi.carburanti_altro[i], $"{parentPath}.consumi.carburanti_altro[{i}]", errors);
            }

            if (consumi.elettricita == null)
            {
                errors.Add(MakeError(LivelloAzienda, TipoFieldNull,
                    $"{parentPath}.consumi.elettricita",
                    "Campo 'consumi.elettricita' non deve essere null.",
                    null));
            }
            else
            {
                for (int i = 0; i < consumi.elettricita.Count; i++)
                    ValidateElettrica(consumi.elettricita[i], $"{parentPath}.consumi.elettricita[{i}]", errors);
            }
        }

        private void ValidateCarburante(CarburanteAltroPayload c, string path, List<ValidazioneErroreCo2> errors)
        {
            if (string.IsNullOrWhiteSpace(c.tipo_carburante))
                errors.Add(MakeError(LivelloAzienda, TipoFieldNull,
                    $"{path}.tipo_carburante",
                    "Campo 'tipo_carburante' obbligatorio non valorizzato.",
                    c.tipo_carburante));

            if (c.quantita_carburante < 0)
                errors.Add(MakeError(LivelloAzienda, TipoOutOfRange,
                    $"{path}.quantita_carburante",
                    "Campo 'quantita_carburante' deve essere >= 0.",
                    c.quantita_carburante.ToString(CultureInfo.InvariantCulture)));

            if (string.IsNullOrWhiteSpace(c.unita_di_misura_carburante))
                errors.Add(MakeError(LivelloAzienda, TipoFieldNull,
                    $"{path}.unita_di_misura_carburante",
                    "Campo 'unita_di_misura_carburante' obbligatorio non valorizzato.",
                    c.unita_di_misura_carburante));
        }

        private void ValidateElettrica(ElettricaPayload e, string path, List<ValidazioneErroreCo2> errors)
        {
            if (e.consumo_kwh < 0)
                errors.Add(MakeError(LivelloAzienda, TipoOutOfRange,
                    $"{path}.consumo_kwh",
                    "Campo 'consumo_kwh' deve essere >= 0.",
                    e.consumo_kwh.ToString(CultureInfo.InvariantCulture)));

            if (e.perc_rinnovabili < 0)
                errors.Add(MakeError(LivelloAzienda, TipoOutOfRange,
                    $"{path}.%_rinnovabili",
                    "Campo '%_rinnovabili' deve essere >= 0.",
                    e.perc_rinnovabili.ToString(CultureInfo.InvariantCulture)));
        }

        // ----------------------------------------------------------------
        // Regola 3 — Campi obbligatori Appezzamento + Regola 8 cardinalità impianti
        // ----------------------------------------------------------------

        private void ValidateAppezzamento(AppezzamentoPayload app, int azIdx, int appIdx, List<ValidazioneErroreCo2> errors)
        {
            var basePath = $"$.aziende[{azIdx}].appezzamenti[{appIdx}]";

            if (string.IsNullOrWhiteSpace(app.id_appezzamento))
                errors.Add(MakeError(LivelloAppezzamento, TipoFieldNull,
                    $"{basePath}.id_appezzamento",
                    "Campo 'id_appezzamento' obbligatorio non valorizzato.",
                    app.id_appezzamento));

            ValidateCentroide(app.centroide, LivelloAppezzamento, $"{basePath}.centroide_appezzamento", errors);

            if (app.anno_campagna <= 0)
                errors.Add(MakeError(LivelloAppezzamento, TipoFieldNull,
                    $"{basePath}.anno_campagna",
                    "Campo 'anno_campagna' obbligatorio non valorizzato (valore deve essere > 0).",
                    app.anno_campagna.ToString()));

            // %_sostanza_organica è opzionale; se presente deve essere >= 0
            if (app.perc_sostanza_organica.HasValue && app.perc_sostanza_organica.Value < 0)
                errors.Add(MakeError(LivelloAppezzamento, TipoOutOfRange,
                    $"{basePath}.%_sostanza_organica",
                    "Campo '%_sostanza_organica' deve essere >= 0 se valorizzato.",
                    app.perc_sostanza_organica.Value.ToString(CultureInfo.InvariantCulture)));

            // Regola 8: cardinalità impianti >= 1
            if (app.impianti == null || app.impianti.Count == 0)
            {
                errors.Add(MakeError(LivelloAppezzamento, TipoCardinality,
                    $"{basePath}.impianti",
                    $"L'appezzamento '{app.id_appezzamento}' deve avere almeno un impianto.",
                    app.impianti?.Count.ToString() ?? "null"));
            }
            else
            {
                for (int i = 0; i < app.impianti.Count; i++)
                    ValidateImpianto(app.impianti[i], azIdx, appIdx, i, errors);
            }
        }

        // ----------------------------------------------------------------
        // Regola 4 — Campi obbligatori Impianto + Regola 9 cardinalità operazioni (warning)
        // ----------------------------------------------------------------

        private void ValidateImpianto(ImpiantoPayload imp, int azIdx, int appIdx, int impIdx, List<ValidazioneErroreCo2> errors)
        {
            var basePath = $"$.aziende[{azIdx}].appezzamenti[{appIdx}].impianti[{impIdx}]";

            if (string.IsNullOrWhiteSpace(imp.id_impianto))
                errors.Add(MakeError(LivelloImpianto, TipoFieldNull,
                    $"{basePath}.id_impianto",
                    "Campo 'id_impianto' obbligatorio non valorizzato.",
                    imp.id_impianto));

            if (string.IsNullOrWhiteSpace(imp.id_coltura))
                errors.Add(MakeError(LivelloImpianto, TipoFieldNull,
                    $"{basePath}.id_coltura",
                    "Campo 'id_coltura' obbligatorio non valorizzato.",
                    imp.id_coltura));

            if (string.IsNullOrWhiteSpace(imp.tipo_id_coltura))
                errors.Add(MakeError(LivelloImpianto, TipoFieldNull,
                    $"{basePath}.tipo_id_coltura",
                    "Campo 'tipo_id_coltura' obbligatorio non valorizzato.",
                    imp.tipo_id_coltura));

            if (imp.area_ha < 0)
                errors.Add(MakeError(LivelloImpianto, TipoOutOfRange,
                    $"{basePath}.area_ha",
                    "Campo 'area_ha' deve essere >= 0.",
                    imp.area_ha.ToString(CultureInfo.InvariantCulture)));

            // resa_prevista è opzionale; se presente deve essere >= 0
            if (imp.resa_prevista_kg_ha.HasValue && imp.resa_prevista_kg_ha.Value < 0)
                errors.Add(MakeError(LivelloImpianto, TipoOutOfRange,
                    $"{basePath}.resa_prevista",
                    "Campo 'resa_prevista' deve essere >= 0 se valorizzato.",
                    imp.resa_prevista_kg_ha.Value.ToString(CultureInfo.InvariantCulture)));

            if (!string.IsNullOrWhiteSpace(imp.id_finalita) && !string.IsNullOrWhiteSpace(imp.id_destinazione_uso))
                errors.Add(MakeError(LivelloImpianto, TipoDataInconsistency,
                    basePath,
                    "I campi 'id_finalita' e 'id_destinazione_uso' sono mutuamente esclusivi.",
                    $"id_finalita={imp.id_finalita}; id_destinazione_uso={imp.id_destinazione_uso}"));

            if (imp.data_inizio_ciclo == default)
                errors.Add(MakeError(LivelloImpianto, TipoFieldNull,
                    $"{basePath}.data_inizio_ciclo",
                    "Campo 'data_inizio_ciclo' obbligatorio non valorizzato.",
                    null));

            if (imp.data_fine_ciclo == default)
                errors.Add(MakeError(LivelloImpianto, TipoFieldNull,
                    $"{basePath}.data_fine_ciclo",
                    "Campo 'data_fine_ciclo' obbligatorio non valorizzato.",
                    null));

            // Regola 9: cardinalità operazioni — non bloccante, solo warning log
            if (imp.operazioni == null || imp.operazioni.Count == 0)
            {
                LogWarning(
                    "CARDINALITY_WARNING — Impianto '{IdImpianto}' (path: {Path}) non ha operazioni colturali associate.",
                    null, null, imp.id_impianto, basePath);
            }
            else
            {
                for (int i = 0; i < imp.operazioni.Count; i++)
                    ValidateOperazione(imp.operazioni[i], azIdx, appIdx, impIdx, i, errors);
            }
        }

        // ----------------------------------------------------------------
        // Regola 5 — Campi obbligatori Operazione + Regola 10 numeri >= 0
        // ----------------------------------------------------------------

        private void ValidateOperazione(OperazionePayload op, int azIdx, int appIdx, int impIdx, int opIdx, List<ValidazioneErroreCo2> errors)
        {
            var basePath = $"$.aziende[{azIdx}].appezzamenti[{appIdx}].impianti[{impIdx}].operazioni[{opIdx}]";

            if (int.Parse(op.id_operazione) <= 0)
                errors.Add(MakeError(LivelloOperazione, TipoFieldNull,
                    $"{basePath}.id_operazione",
                    "Campo 'id_operazione' obbligatorio non valorizzato (valore deve essere > 0).",
                    op.id_operazione.ToString()));

            //if (string.IsNullOrWhiteSpace(op.TipoOperazione))
            //    errors.Add(MakeError(LivelloOperazione, TipoFieldNull,
            //        $"{basePath}.tipo_operazione",
            //        "Campo 'tipo_operazione' obbligatorio non valorizzato.",
            //        op.TipoOperazione));

            if (op.data_operazione == default)
                errors.Add(MakeError(LivelloOperazione, TipoFieldNull,
                    $"{basePath}.data_operazione",
                    "Campo 'data_operazione' obbligatorio non valorizzato.",
                    null));

            if (op.superficie_operazione_ha.HasValue && op.superficie_operazione_ha.Value < 0)
                errors.Add(MakeError(LivelloOperazione, TipoOutOfRange,
                    $"{basePath}.superficie_operazione",
                    "Campo 'superficie_operazione' deve essere >= 0.",
                    op.superficie_operazione_ha.Value.ToString(CultureInfo.InvariantCulture)));

            // Array opzionali — se presenti, validare gli elementi (Regola 10)
            if (op.fertilizzanti != null)
                for (int i = 0; i < op.fertilizzanti.Count; i++)
                    ValidateFertilizzante(op.fertilizzanti[i], $"{basePath}.fertilizzanti[{i}]", errors);

            if (op.agrofarmaci != null)
                for (int i = 0; i < op.agrofarmaci.Count; i++)
                    ValidateAgrofarmaco(op.agrofarmaci[i], $"{basePath}.agrofarmaci[{i}]", errors);

            if (op.sementi != null)
                for (int i = 0; i < op.sementi.Count; i++)
                    ValidateSemente(op.sementi[i], $"{basePath}.sementi[{i}]", errors);

            if (op.prodotti_raccolti != null)
                for (int i = 0; i < op.prodotti_raccolti.Count; i++)
                    ValidateProdottoRaccolto(op.prodotti_raccolti[i], $"{basePath}.prodotti_raccolti[{i}]", errors);
        }

        private void ValidateFertilizzante(FertilizzantePayload f, string path, List<ValidazioneErroreCo2> errors)
        {
            if (f.quantita_kg < 0)
                errors.Add(MakeError(LivelloOperazione, TipoOutOfRange,
                    $"{path}.quantita_kg",
                    "Campo 'quantita_kg' del fertilizzante deve essere >= 0.",
                    f.quantita_kg.ToString(CultureInfo.InvariantCulture)));

            if (f.molecole != null)
                for (int i = 0; i < f.molecole.Count; i++)
                    ValidateMolecola(f.molecole[i], $"{path}.molecole[{i}]", errors);
        }

        private void ValidateMolecola(MolecolaPayload m, string path, List<ValidazioneErroreCo2> errors)
        {
            if (string.IsNullOrWhiteSpace(m.molecola))
                errors.Add(MakeError(LivelloOperazione, TipoFieldNull,
                    $"{path}.molecola",
                    "Campo 'molecola' obbligatorio non valorizzato.",
                    m.molecola));

            if (m.titolo < 0)
                errors.Add(MakeError(LivelloOperazione, TipoOutOfRange,
                    $"{path}.titolo",
                    "Campo 'titolo' della molecola deve essere >= 0.",
                    m.titolo.ToString(CultureInfo.InvariantCulture)));
        }

        private void ValidateAgrofarmaco(AgrofarmacоPayload a, string path, List<ValidazioneErroreCo2> errors)
        {
            if (string.IsNullOrWhiteSpace(a.codice_agrofarmaco))
                errors.Add(MakeError(LivelloOperazione, TipoFieldNull,
                    $"{path}.cod_agrofarmaco",
                    "Campo 'cod_agrofarmaco' obbligatorio non valorizzato.",
                    a.codice_agrofarmaco));

            if (a.quantita_kg < 0)
                errors.Add(MakeError(LivelloOperazione, TipoOutOfRange,
                    $"{path}.quantita",
                    "Campo 'quantita' dell'agrofarmaco deve essere >= 0.",
                    a.quantita_kg.ToString(CultureInfo.InvariantCulture)));

            if (a.principi_attivi != null)
                for (int i = 0; i < a.principi_attivi.Count; i++)
                    ValidatePrincipioAttivo(a.principi_attivi[i], $"{path}.principi_attivi[{i}]", errors);
        }

        private void ValidatePrincipioAttivo(PrincipioAttivoPayload pa, string path, List<ValidazioneErroreCo2> errors)
        {
            if (pa.perc_principio_attivo < 0)
                errors.Add(MakeError(LivelloOperazione, TipoOutOfRange,
                    $"{path}.perc_principio_attivo",
                    "Campo 'perc_principio_attivo' deve essere >= 0.",
                    pa.perc_principio_attivo.ToString(CultureInfo.InvariantCulture)));
        }

        private void ValidateSemente(SementePayload s, string path, List<ValidazioneErroreCo2> errors)
        {
            if (string.IsNullOrWhiteSpace(s.codice_semente))
                errors.Add(MakeError(LivelloOperazione, TipoFieldNull,
                    $"{path}.cod_semente",
                    "Campo 'cod_semente' obbligatorio non valorizzato.",
                    s.codice_semente));

            if (s.quantita < 0)
                errors.Add(MakeError(LivelloOperazione, TipoOutOfRange,
                    $"{path}.quantita",
                    "Campo 'quantita' della semente deve essere >= 0.",
                    s.quantita.ToString(CultureInfo.InvariantCulture)));

            if (string.IsNullOrWhiteSpace(s.uom))
                errors.Add(MakeError(LivelloOperazione, TipoFieldNull,
                    $"{path}.unita_di_misura",
                    "Campo 'unita_di_misura' della semente obbligatorio non valorizzato.",
                    s.uom));
        }

        private void ValidateProdottoRaccolto(ProdottoRaccoltоPayload pr, string path, List<ValidazioneErroreCo2> errors)
        {
            if (pr.quantita_kg < 0)
                errors.Add(MakeError(LivelloOperazione, TipoOutOfRange,
                    $"{path}.quantita_kg",
                    "Campo 'quantita_kg' del prodotto raccolto deve essere >= 0.",
                    pr.quantita_kg.ToString(CultureInfo.InvariantCulture)));
        }

        // ----------------------------------------------------------------
        // Regola 11 — Validazione WKT POINT e range coordinate
        // ----------------------------------------------------------------

        private void ValidateCentroide(string centroide, string livello, string path, List<ValidazioneErroreCo2> errors)
        {
            if (string.IsNullOrWhiteSpace(centroide))
            {
                errors.Add(MakeError(livello, TipoFieldNull, path,
                    "Campo centroide obbligatorio non valorizzato.",
                    centroide));
                return;
            }

            var match = WktPointRegex.Match(centroide);
            if (!match.Success)
            {
                errors.Add(MakeError(livello, TipoInvalidFormat, path,
                    "Formato WKT non valido. Atteso: POINT(longitude latitude).",
                    centroide));
                return;
            }

            if (double.TryParse(match.Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double lon)
                && (lon < -180 || lon > 180))
            {
                errors.Add(MakeError(livello, TipoOutOfRange, path,
                    $"Longitudine '{lon}' fuori range valido [-180, 180].",
                    match.Groups[1].Value));
            }

            if (double.TryParse(match.Groups[2].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double lat)
                && (lat < -90 || lat > 90))
            {
                errors.Add(MakeError(livello, TipoOutOfRange, path,
                    $"Latitudine '{lat}' fuori range valido [-90, 90].",
                    match.Groups[2].Value));
            }
        }

        // ----------------------------------------------------------------
        // Regola 13 — Unicità ID (warning, non bloccante)
        // ----------------------------------------------------------------

        /// <summary>
        /// Verifica l'unicità degli ID concatenati nel payload.
        /// In caso di duplicati emette solo un warning log (non bloccante).
        /// Riferimento spec: DS06-BL Regola 13.
        /// </summary>
        private void CheckIdUniqueness(PayloadCo2Root payload)
        {
            if (payload.aziende == null) return;

            var appezzamentoIds = new HashSet<string>(StringComparer.Ordinal);
            var impiantoIds = new HashSet<string>(StringComparer.Ordinal);
            var operazioneIds = new HashSet<string>(StringComparer.Ordinal);

            foreach (var az in payload.aziende)
            {
                if (az.appezzamenti == null) continue;

                foreach (var app in az.appezzamenti)
                {
                    if (!string.IsNullOrWhiteSpace(app.id_appezzamento) && !appezzamentoIds.Add(app.id_appezzamento))
                        LogWarning(
                            "ID_UNIQUENESS_WARNING — 'id_appezzamento' duplicato nel payload: '{Id}'.",
                            null, null, app.id_appezzamento);

                    if (app.impianti == null) continue;

                    foreach (var imp in app.impianti)
                    {
                        if (!string.IsNullOrWhiteSpace(imp.id_impianto) && !impiantoIds.Add(imp.id_impianto))
                            LogWarning(
                                "ID_UNIQUENESS_WARNING — 'id_impianto' duplicato nel payload: '{Id}'.",
                                null, null, imp.id_impianto);

                        if (imp.operazioni == null) continue;

                        foreach (var op in imp.operazioni)
                        {
                            if (int.Parse(op.id_operazione) > 0 && !operazioneIds.Add(op.id_operazione))
                                LogWarning(
                                    "ID_UNIQUENESS_WARNING — 'id_operazione' duplicato nel payload: {Id}.",
                                    null, null, op.id_operazione);
                        }
                    }
                }
            }
        }

        // ----------------------------------------------------------------
        // Factory helper
        // ----------------------------------------------------------------

        private static ValidazioneErroreCo2 MakeError(
            string livello,
            string tipoErrore,
            string pathJson,
            string messaggio,
            string? valoreAttuale) =>
            new()
            {
                Livello = livello,
                TipoErrore = tipoErrore,
                PathJson = pathJson,
                Messaggio = messaggio,
                ValoreAttuale = valoreAttuale
            };
    }
}
