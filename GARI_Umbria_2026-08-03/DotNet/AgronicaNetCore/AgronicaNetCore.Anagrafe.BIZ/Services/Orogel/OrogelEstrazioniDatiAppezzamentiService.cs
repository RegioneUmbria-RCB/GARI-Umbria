using System.Data;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaNetCore.Anagrafe.BIZ.Exceptions.Orogel;
using AgronicaNetCore.Anagrafe.BIZ.Resources;
using AgronicaNetCore.Anagrafe.BIZ.Services.Orogel.Models;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.CodiciAnagrafe;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Fabbricati;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.GerarchiaImprese;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Orogel;
using AgronicaNetCore.Anagrafe.DAL.HelpersSTD;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.ContributiColtivazioni;
using Microsoft.Extensions.Localization;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.Anagrafe.BIZ.Services.Orogel
{
    /// <summary>
    /// DS08-BL: Orchestrates the extraction of paginated appezzamento data for FS004.
    /// Coordinates DS04-BL (single-key keyset cursor on <c>Progetto_Cod</c>) and
    /// <see cref="IAppezzamentoOrogelDAL"/> (INNER JOIN query execution with lookup LEFT JOINs).
    /// </summary>
    public class OrogelEstrazioniDatiAppezzamentiService
        : BaseServiceAnagrafeBIZ,
            IOrogelEstrazioniDatiAppezzamentiService
    {
        /// <summary>
        /// DS08-BL §Filtro paginazione singola chiave: Keyset on <c>Progetto_Cod</c> (integer).
        /// DS04-BL fallback for null/first page → -1.
        /// DS04-BL ToParamName("Progetto_Cod") → "@LastProgettoCod".
        /// </summary>
        private static readonly IReadOnlyList<KeysetColonnaDefinizione> ColonneOrdinamento = new[]
        {
            new KeysetColonnaDefinizione("Progetto_Cod", KeysetTipoDato.Integer),
        };

        /// <summary>
        /// DS08-BL: Initializes <see cref="OrogelEstrazioniDatiAppezzamentiService"/>.
        /// </summary>
        private readonly IOrogelPaginazioneKeysetService _paginazioneKeysetService;
        private readonly IAppezzamentoOrogelDAL _appezzamentoOrogelDAL;
        private readonly ICodiciAnagrafe _codiciAnagrafe;
        private readonly IFabbricati _fabbricati;
        private readonly IContributiColtivazioni _contributiColtivazioniDAL;
        private readonly IGerarchiaImprese _gerarchiaImprese;

        public OrogelEstrazioniDatiAppezzamentiService(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer,
            IOrogelPaginazioneKeysetService paginazioneKeysetService,
            IAppezzamentoOrogelDAL appezzamentoOrogelDAL,
            ICodiciAnagrafe codiciAnagrafe,
            IFabbricati fabbricati,
            IContributiColtivazioni contributiColtivazioniDAL,
            IGerarchiaImprese gerarchiaImprese
        )
            : base(provider, localizer)
        {
            _paginazioneKeysetService = paginazioneKeysetService;
            _appezzamentoOrogelDAL = appezzamentoOrogelDAL;
            _codiciAnagrafe = codiciAnagrafe;
            _fabbricati = fabbricati;
            _contributiColtivazioniDAL = contributiColtivazioniDAL;
            _gerarchiaImprese = gerarchiaImprese;
        }

        /// <inheritdoc/>
        public async Task<EstrazioniDatiAppezzamentiResult> EstraiAppezzamentiAsync(
            IReadOnlyList<string> codiciAzienda,
            int pageSize,
            string? nextKey,
            AgronicaCoreParametriTriple objParametriTriple
        )
        {
            // DS08-BL §Paginazione: decode cursor via DS04-BL.
            // FiltroWherePaginazione is not forwarded — DAL hardcodes ese.Progetto_Cod > @LastProgettoCod.
            KeysetFiltroResult filtro = _paginazioneKeysetService.DecodificaNextKey(
                nextKey,
                pageSize,
                ColonneOrdinamento
            );

            DataTable dt;
            try
            {
                await OpenConnectionAsync(
                    objParametriTriple.ObjParametriServer,
                    OpenTransaction: false,
                    IsolationLevel.ReadUncommitted
                );
                try
                {
                    dt = await _appezzamentoOrogelDAL.LeggiAppezzamentiPaginatiAsync(
                        pageSize,
                        filtro.ParSqlPaginazione,
                        codiciAzienda,
                        objParametriTriple.ObjParametriServer
                    );
                }
                catch (Exception ex) when (OrogelBizHelper.IsConnectionError(ex))
                {
                    throw new DatabaseConnectionException(
                        "Impossibile connettersi al database archivio per l'estrazione appezzamenti.",
                        ex
                    );
                }
                catch (Exception ex)
                {
                    throw new QueryExecutionException(
                        "Errore durante l'esecuzione della query di estrazione appezzamenti.",
                        ex
                    );
                }

                if (dt.Rows.Count == 0)
                    return new EstrazioniDatiAppezzamentiResult(
                        new KeysetMetadatiPaginazione(pageSize, null),
                        Array.Empty<AppezzamentoItem>()
                    );

                DataTable dtCodiciEsercizi = await _codiciAnagrafe.LeggiCodiciEserciziAsync(
                    dt.Rows.Cast<DataRow>()
                        .Select(r =>
                            (
                                r["codice_azienda"]?.ToString() ?? string.Empty,
                                Convert.ToInt32(r["codice_centro"]),
                                Convert.ToInt32(r["codice_appezzamento"]),
                                Convert.ToInt32(r["codice_impianto"]),
                                Convert.ToInt32(r["codice_esercizio"])
                            )
                        )
                        .Distinct()
                        .ToList(),
                    new List<int>
                    {
                    (int)Enum_CodiciAnagrafe.Tecnico,
                    (int)Enum_CodiciAnagrafe.Contributi,
                    (int)Enum_CodiciAnagrafe.Codice_Certificazione,
                    (int)Enum_CodiciAnagrafe.Codice_Certificazione_Prodotto,
                    (int)Enum_CodiciAnagrafe.Capitolato_Privato,
                    (int)Enum_CodiciAnagrafe.Codice_Residuo,
                    (int)Enum_CodiciAnagrafe.Organismo_Referente,
                    (int)Enum_CodiciAnagrafe.Magazzino_Conferimento,
                    },
                    objParametriTriple.ObjParametriServer,
                    escludiCodiciCliente: true
                );

                var codiciEsercizi =
                    new Dictionary<(string, int, int, int, int), List<CodiciAnagrafeValori>>();
                foreach (DataRow codRow in dtCodiciEsercizi.Rows)
                {
                    var key = (
                        codRow["piva"]?.ToString() ?? string.Empty,
                        Convert.ToInt32(codRow["sa_cod"]),
                        Convert.ToInt32(codRow["appezza"]),
                        Convert.ToInt32(codRow["id_reg"]),
                        Convert.ToInt32(codRow["progetto_cod"])
                    );
                    if (!codiciEsercizi.ContainsKey(key))
                        codiciEsercizi[key] = new List<CodiciAnagrafeValori>();
                    codiciEsercizi[key].Add(CodiciAnagrafeMapper.MapSTDFromRow(codRow));
                }

                DataTable dtCodiciImpianti = await _codiciAnagrafe.LeggiCodiciImpiantiAsync(
                    dt.Rows.Cast<DataRow>()
                        .Select(r =>
                            (
                                r["codice_azienda"]?.ToString() ?? string.Empty,
                                Convert.ToInt32(r["codice_centro"]),
                                Convert.ToInt32(r["codice_appezzamento"]),
                                Convert.ToInt32(r["codice_impianto"])
                            )
                        )
                        .Distinct()
                        .ToList(),
                    new List<int>
                    {
                    (int)Enum_CodiciAnagrafe.Impianto_TraFila_Maschio,
                    (int)Enum_CodiciAnagrafe.Impianto_TraFila_Femmina,
                    (int)Enum_CodiciAnagrafe.Dettaglio_Specie_Personalizzato,
                    },
                    objParametriTriple.ObjParametriServer,
                    escludiCodiciCliente: true
                );

                var codiciImpianti =
                    new Dictionary<(string, int, int, int), List<CodiciAnagrafeValori>>();
                foreach (DataRow impRow in dtCodiciImpianti.Rows)
                {
                    var impKey = (
                        impRow["piva"]?.ToString() ?? string.Empty,
                        Convert.ToInt32(impRow["sa_cod"]),
                        Convert.ToInt32(impRow["appezza"]),
                        Convert.ToInt32(impRow["id_reg"])
                    );
                    if (!codiciImpianti.ContainsKey(impKey))
                        codiciImpianti[impKey] = new List<CodiciAnagrafeValori>();
                    codiciImpianti[impKey].Add(CodiciAnagrafeMapper.MapSTDFromRow(impRow));
                }

                var contributiCodici = codiciEsercizi
                    .Values.SelectMany(list =>
                        list.Where(c => c.codiceAnagrafe.codice == (int)Enum_CodiciAnagrafe.Contributi)
                            .SelectMany(c =>
                                (c.valore ?? string.Empty).Split(
                                    '|',
                                    StringSplitOptions.RemoveEmptyEntries
                                )
                            )
                    )
                    .Distinct()
                    .ToList();

                var contributiDescrizioni = new Dictionary<string, string>();
                if (contributiCodici.Count > 0)
                {
                    DataTable dtContributi = await _contributiColtivazioniDAL.LeggiDescrizioniAsync(
                        contributiCodici,
                        objParametriTriple.ObjParametriServer
                    );
                    foreach (DataRow cRow in dtContributi.Rows)
                    {
                        string cod = cRow["Contributo_Cod"]?.ToString() ?? string.Empty;
                        contributiDescrizioni[cod] = cRow["Contributo_Des"]?.ToString() ?? string.Empty;
                    }
                }

                var fabbricatiKeys = new List<(string, int, int)>();
                foreach (var codiciList in codiciEsercizi.Values)
                {
                    foreach (
                        var c in codiciList.Where(c =>
                            c.codiceAnagrafe.codice == (int)Enum_CodiciAnagrafe.Magazzino_Conferimento
                        )
                    )
                    {
                        var p = c.valore?.Split('|');
                        if (
                            p?.Length == 3
                            && int.TryParse(p[1], out int mSaCod)
                            && int.TryParse(p[0], out int mFabbCod)
                        )
                        {
                            fabbricatiKeys.Add((p[2], mSaCod, mFabbCod));
                        }
                    }
                }
                fabbricatiKeys = fabbricatiKeys.Distinct().ToList();

                var fabbricatiDescrizioni = new Dictionary<(string, int, int), string>();
                if (fabbricatiKeys.Count > 0)
                {
                    DataTable dtFabbricati = await _fabbricati.LeggiDescrizioneAsync(
                        fabbricatiKeys,
                        objParametriTriple.ObjParametriServer,
                        estraiAzienda: true
                    );
                    foreach (DataRow fRow in dtFabbricati.Rows)
                    {
                        var fKey = (
                            fRow["PIVA"]?.ToString() ?? string.Empty,
                            Convert.ToInt32(fRow["Sa_Cod"]),
                            Convert.ToInt32(fRow["Fabbricato_Cod"])
                        );
                        string fabbDes = fRow["Fabbricato_Des"]?.ToString() ?? string.Empty;
                        string ragSoc = fRow["Rag_Soc"]?.ToString() ?? string.Empty;
                        fabbricatiDescrizioni[fKey] = string.IsNullOrEmpty(ragSoc)
                            ? fabbDes
                            : $"{fabbDes} ({ragSoc})";
                    }
                }

                // DS08-BL §organismoReferente: per ogni Organismo_Referente distinto
                // risale la gerarchia per determinare PartitaIvaFornitore e PartitaIvaAziendaLivello1.
                // FornitoreOverride = null → usa il codice_azienda dell'esercizio (se stesso).
                var gerarchieFornitori = new Dictionary<
                    string,
                    (string? FornitoreOverride, string? PrimoLivello)
                >(StringComparer.OrdinalIgnoreCase);
                var organismoReferentiDistinti = codiciEsercizi
                    .Values.SelectMany(list =>
                        list.Where(c =>
                                c.codiceAnagrafe.codice == (int)Enum_CodiciAnagrafe.Organismo_Referente
                            )
                            .Select(c => c.valore ?? string.Empty)
                    )
                    .Where(v => !string.IsNullOrEmpty(v))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                if (organismoReferentiDistinti.Count > 0)
                {
                    foreach (string organismoReferente in organismoReferentiDistinti)
                    {
                        DataTable dtAncestors = await _gerarchiaImprese.LeggiPadriRicorsivaAsync(
                            organismoReferente,
                            objParametriTriple.ObjParametriServer
                        );
                        if (dtAncestors.Rows.Count <= 1)
                            gerarchieFornitori[organismoReferente] = (null, organismoReferente);
                        else
                            gerarchieFornitori[organismoReferente] = (
                                organismoReferente,
                                dtAncestors.Rows[dtAncestors.Rows.Count - 2].Field<string>("Padre")
                            );
                    }
                }

                List<AppezzamentoItem> data = MapToItems(
                    dt,
                    codiciEsercizi,
                    codiciImpianti,
                    fabbricatiDescrizioni,
                    contributiDescrizioni,
                    gerarchieFornitori
                );
                // DS08-BL §Costruzione Metadati: last record Progetto_Cod as next cursor.
                // Key must match ColonneOrdinamento NomeColonna ("Progetto_Cod");
                // DataTable alias for that column is "codice_esercizio".
                DataRow ultimaRiga = dt.Rows[dt.Rows.Count - 1];
                var ultimoRecord = new Dictionary<string, object?>
                {
                    ["Progetto_Cod"] = ultimaRiga["codice_esercizio"],
                };

                KeysetMetadatiPaginazione metadata = _paginazioneKeysetService.GeneraMetadati(
                    pageSize,
                    dt.Rows.Count,
                    ColonneOrdinamento,
                    ultimoRecord
                );
                return new EstrazioniDatiAppezzamentiResult(metadata, data);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriTriple.ObjParametriServer, ex);
                CloseTransaction(objParametriTriple.ObjParametriServer, Rollback: true);
                throw;
            }
            finally
            {
                CloseConnection(objParametriTriple.ObjParametriServer);
            }
        }

        /// <remarks>
        /// DS08-BL §Output data[]: Maps DataTable rows to <see cref="AppezzamentoItem"/> records.
        /// Optional fields are mapped to null when DBNull or empty string is returned.
        /// </remarks>
        private static List<AppezzamentoItem> MapToItems(
            DataTable dt,
            Dictionary<(string, int, int, int, int), List<CodiciAnagrafeValori>> codiciEsercizi,
            Dictionary<(string, int, int, int), List<CodiciAnagrafeValori>> codiciImpianti,
            Dictionary<(string, int, int), string> fabbricatiDescrizioni,
            Dictionary<string, string> contributiDescrizioni,
            Dictionary<string, (string? FornitoreOverride, string? PrimoLivello)> gerarchieFornitori
        )
        {
            var items = new List<AppezzamentoItem>(dt.Rows.Count);
            foreach (DataRow row in dt.Rows)
            {
                var esercizioKey = (
                    row["codice_azienda"]?.ToString() ?? string.Empty,
                    Convert.ToInt32(row["codice_centro"]),
                    Convert.ToInt32(row["codice_appezzamento"]),
                    Convert.ToInt32(row["codice_impianto"]),
                    Convert.ToInt32(row["codice_esercizio"])
                );
                codiciEsercizi.TryGetValue(esercizioKey, out List<CodiciAnagrafeValori>? codiciRow);
                string GetValoreDaEsercizioCodice(int idCod) =>
                    codiciRow?.FirstOrDefault(c => c.codiceAnagrafe.codice == idCod)?.valore
                    ?? string.Empty;

                // DS08-BL §organismoReferente: se vuoto, fornitore e aziendaPrimoLivello sono null.
                // Altrimenti si usa la gerarchia pre-computata.
                string organismoReferenteCod = GetValoreDaEsercizioCodice(
                    (int)Enum_CodiciAnagrafe.Organismo_Referente
                );
                string? partitaIvaFornitore = null;
                string? partitaIvaAziendaLivello1 = null;
                if (
                    !string.IsNullOrEmpty(organismoReferenteCod)
                    && gerarchieFornitori.TryGetValue(organismoReferenteCod, out var gerarchia)
                )
                {
                    partitaIvaFornitore =
                        gerarchia.FornitoreOverride ?? (row["codice_azienda"]?.ToString());
                    partitaIvaAziendaLivello1 = gerarchia.PrimoLivello;
                }

                var impiantoKey = (
                    row["codice_azienda"]?.ToString() ?? string.Empty,
                    Convert.ToInt32(row["codice_centro"]),
                    Convert.ToInt32(row["codice_appezzamento"]),
                    Convert.ToInt32(row["codice_impianto"])
                );
                codiciImpianti.TryGetValue(
                    impiantoKey,
                    out List<CodiciAnagrafeValori>? codiciImpRow
                );
                string GetValoreDaImpiantoCodice(int idCod) =>
                    codiciImpRow?.FirstOrDefault(c => c.codiceAnagrafe.codice == idCod)?.valore
                    ?? string.Empty;

                string magazzinoConferimentoCodice = GetValoreDaEsercizioCodice(
                    (int)Enum_CodiciAnagrafe.Magazzino_Conferimento
                );
                string magazzinoConferimentoDescrizione = string.Empty;
                if (!string.IsNullOrEmpty(magazzinoConferimentoCodice))
                {
                    var fabbParts = magazzinoConferimentoCodice.Split('|');
                    if (
                        fabbParts.Length == 3
                        && int.TryParse(fabbParts[1], out int mSaCod)
                        && int.TryParse(fabbParts[0], out int mFabbCod)
                    )
                    {
                        fabbricatiDescrizioni.TryGetValue(
                            (fabbParts[2], mSaCod, mFabbCod),
                            out string? mDes
                        );
                        magazzinoConferimentoDescrizione = mDes ?? string.Empty;
                    }
                }

                items.Add(
                    new AppezzamentoItem(
                        CodiceAzienda: row["codice_azienda"]?.ToString() ?? string.Empty,
                        CodiceCentro: row["codice_centro"]?.ToString() ?? string.Empty,
                        CodiceAppezzamento: row["codice_appezzamento"]?.ToString() ?? string.Empty,
                        CodiceImpianto: row["codice_impianto"]?.ToString() ?? string.Empty,
                        CodiceEsercizio: row["codice_esercizio"]?.ToString() ?? string.Empty,
                        EsercizioInizioValidita: Convert.ToDateTime(row["data_inizio_esercizio"]),
                        EsercizioFineValidita: row["data_fine_esercizio"] is DBNull
                            ? default
                            : Convert.ToDateTime(row["data_fine_esercizio"]),
                        VincoloImpiantoCodice: row["vincolo_impianto_codice"]?.ToString() ?? string.Empty,
                        VincoloImpiantoDescrizione: row["vincolo_impianto_descrizione"]?.ToString()
                            ?? string.Empty,
                        StatoEsercizioCodice: row["stato_impianto_codice"]?.ToString()
                            ?? string.Empty,
                        StatoEsercizioDescrizione: row["stato_impianto_descrizione"]?.ToString()
                            ?? string.Empty,
                        ProduzionePrevista: row["produzione_prevista_kg_ha"] is DBNull
                            ? 0m
                            : Convert.ToDecimal(row["produzione_prevista_kg_ha"]),
                        ProduzioneTotalePrevista: row["produzione_prevista_kg_tot"] is DBNull
                            ? 0m
                            : Convert.ToDecimal(row["produzione_prevista_kg_tot"]),
                        FlagSecondoRaccolto: row["flagSecondoRaccolto"] is not DBNull
                            ? Convert.ToInt32(row["flagSecondoRaccolto"])
                            : 0,
                        RaccoltaPrevista: row["data_raccolta_prevista"] is DBNull
                            ? null
                            : Convert.ToDateTime(row["data_raccolta_prevista"]),
                        SpecieVegetaleCodice: row["specie_vegetale_codice"]?.ToString()
                            ?? string.Empty,
                        SpecieVegetaleDescrizione: row["specie_vegetale_descrizione"]?.ToString()
                            ?? string.Empty,
                        VarietaCodice: row["varieta_codice"]?.ToString() ?? string.Empty,
                        VarietaDescrizione: row["varieta_descrizione"]?.ToString() ?? string.Empty,
                        RaggruppamentoVarietaleCodice: row["raggruppamento_varietale_codice"]
                            ?.ToString() ?? string.Empty,
                        RaggruppamentoVarietaleDescrizione: row[
                            "raggruppamento_varietale_descrizione"
                        ]
                            ?.ToString() ?? string.Empty,
                        GruppoVegetaleCodice: row["codice_gruppo_vegetale"]?.ToString()
                            ?? string.Empty,
                        FinalitaProduttivaCodice: row["finalita_produttiva_codice"]?.ToString()
                            ?? string.Empty,
                        FinalitaProduttivaDescrizione: row["finalita_produttiva_descrizione"]?.ToString()
                            ?? string.Empty,
                        ContributoCodice: GetValoreDaEsercizioCodice(
                            (int)Enum_CodiciAnagrafe.Contributi
                        ).Replace('|', '_'),
                        ContributoDescrizione: string.Join(
                            "_",
                            (GetValoreDaEsercizioCodice((int)Enum_CodiciAnagrafe.Contributi))
                                .Split('|', StringSplitOptions.RemoveEmptyEntries)
                                .Select(cod => contributiDescrizioni.GetValueOrDefault(cod, cod))
                        ),
                        FormaAllevamentoCodice: row["forma_allevamento_codice"]?.ToString()
                            ?? string.Empty,
                        PortinnestoCodice: row["portinnesto_codice"] is DBNull
                            ? 0
                            : Convert.ToInt32(row["portinnesto_codice"]),
                        ImpiantoInizioValidita: Convert.ToDateTime(row["anno_impianto"]),
                        DataAbbattimento: row["data_abbattimento"] is DBNull
                            ? null
                            : Convert.ToDateTime(row["data_abbattimento"]),
                        Superficie: row["superficie_impianto_ha"] is DBNull
                            ? 0m
                            : Convert.ToDecimal(row["superficie_impianto_ha"]),
                        NumeroPianteProduzione: row["numero_piante_ha"] is DBNull
                            || row["superficie_impianto_ha"] is DBNull
                            ? 0m
                            : Convert.ToDecimal(row["numero_piante_ha"])
                                * Convert.ToDecimal(row["superficie_impianto_ha"]),
                        DistanzaTraFileM: decimal.TryParse(
                            GetValoreDaImpiantoCodice(
                                    (int)Enum_CodiciAnagrafe.Impianto_TraFila_Femmina
                                )
                                .Replace(',', '.'),
                            System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture,
                            out decimal _distanzaTraFile
                        )
                            ? _distanzaTraFile
                            : 0m,
                        DistanzaSuFilaM: decimal.TryParse(
                            GetValoreDaImpiantoCodice(
                                    (int)Enum_CodiciAnagrafe.Impianto_TraFila_Maschio
                                )
                                .Replace(',', '.'),
                            System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture,
                            out decimal _distanzaSuFila
                        )
                            ? _distanzaSuFila
                            : 0m,
                        FlagIrrigazione: row["flag_irrigazione"] is DBNull
                            ? 0
                            : Convert.ToInt32(row["flag_irrigazione"]),
                        FlagSerra: row["flag_serra"] is DBNull
                            ? 0
                            : Convert.ToInt32(row["flag_serra"]),
                        NumeroAppezzamento: row["numero_appezzamento"]?.ToString() ?? string.Empty,
                        PartitaIvaFornitore: partitaIvaFornitore ?? string.Empty,
                        // DS08-BL §partita_iva_conferimento: the Organismo_Referente code (id_cod=1074) stored
                        // directly in Imprese_Progetti_Codici — not the GerarchiaImprese parent.
                        PartitaIvaConferimento: organismoReferenteCod,
                        TecnicoResponsabile: row["tecnico_responsabile"]?.ToString()
                            ?? string.Empty,
                        TecnicoCf: row["tecnico_cf"]?.ToString() ?? string.Empty,
                        MagazzinoConferimentoCodice: magazzinoConferimentoCodice,
                        MagazzinoConferimentoDescrizione: magazzinoConferimentoDescrizione,
                        PartitaIvaAziendaLivello1: partitaIvaAziendaLivello1 ?? string.Empty,
                        FlagFruizioneContributi: string.IsNullOrEmpty(
                            GetValoreDaEsercizioCodice((int)Enum_CodiciAnagrafe.Contributi)
                        )
                            ? 0
                            : 1,
                        KgConferiti: row["kg_conferiti"] is DBNull
                            ? 0m
                            : Convert.ToDecimal(row["kg_conferiti"]),
                        CertificazioneAziendaleCodice: GetValoreDaEsercizioCodice(
                            (int)Enum_CodiciAnagrafe.Codice_Certificazione
                        ),
                        CertificazioneProdottoCodice: GetValoreDaEsercizioCodice(
                            (int)Enum_CodiciAnagrafe.Codice_Certificazione_Prodotto
                        ),
                        DataUltimoRilievoProducuzionePrevista: row[
                            "data_ultimo_rilievo_produzione_prevista"
                        ] is DBNull
                            ? null
                            : Convert.ToDateTime(row["data_ultimo_rilievo_produzione_prevista"]),
                        ResaUltimoRilievoProduzionePrevistaKgHa: row[
                            "resa_ultimo_rilievo_produzione_prevista_kg_ha"
                        ] is DBNull
                            ? null
                            : Convert.ToDecimal(
                                row["resa_ultimo_rilievo_produzione_prevista_kg_ha"]
                            ),
                        SuperficieAbbattuta: row["superficieAbbattuta"] is DBNull
                            ? 0m
                            : Convert.ToDecimal(row["superficieAbbattuta"]),
                        PercentualeMoria: row["danni_rilevati_numero"] is DBNull
                            ? (decimal?)null
                            : Convert.ToDecimal(row["danni_rilevati_numero"]),
                        ProduzioneStimataEffettiva: row["stima_produzione_kg_tot"] is DBNull
                            ? 0m
                            : Convert.ToDecimal(row["stima_produzione_kg_tot"]),
                        DataRilievoDanni: row["data_rilievo_danni"] is DBNull
                            ? null
                            : Convert.ToDateTime(row["data_rilievo_danni"]),
                        ResaUltimoRilievoProduzionePrevistaKgTot: row[
                            "resa_ultimo_rilievo_produzione_prevista_kg_tot"
                        ] is DBNull
                            ? null
                            : Convert.ToDecimal(
                                row["resa_ultimo_rilievo_produzione_prevista_kg_tot"]
                            ),
                        CertificazioneCommercialeCodice: GetValoreDaEsercizioCodice(
                            (int)Enum_CodiciAnagrafe.Capitolato_Privato
                        ),
                        ResiduoCodice: GetValoreDaEsercizioCodice(
                            (int)Enum_CodiciAnagrafe.Codice_Residuo
                        ),
                        StatoProduzione: row["stato_impianto_codice"] is not DBNull
                            && new[] { 102, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 17 }.Contains(
                                Convert.ToInt32(row["stato_impianto_codice"])
                            )
                            ? 2
                            : 1,
                        NoteDanni: row["danni_rilevati_descrizione"]?.ToString() ?? string.Empty,
                        SpecieWmsCodice: row["specie_wms_codice"]?.ToString() ?? string.Empty,
                        VarietaWmsCodice: row["varieta_wms_codice"]?.ToString() ?? string.Empty,
                        CodiceStagionalitaWms: GetValoreDaImpiantoCodice(
                            (int)Enum_CodiciAnagrafe.Dettaglio_Specie_Personalizzato
                        )
                    )
                );
            }
            return items;
        }
    }
}
