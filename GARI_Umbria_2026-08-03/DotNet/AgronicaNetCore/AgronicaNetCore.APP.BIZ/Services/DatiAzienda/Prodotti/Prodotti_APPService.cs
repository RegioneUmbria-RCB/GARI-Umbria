using System.Data;
using AgronicaCoreDTOStd.InData;
using AgronicaCoreModelsSTD.attivita.dettagli;
using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App;
using AgronicaCoreModelsSTD.metaschema;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.CodificaProdotti;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Fabbricati;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Impianti;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.MateriePrime;
using AgronicaNetCore.APP.BIZ.Resources;
using AgronicaNetCore.APP.BIZ.Services.ImpostazioniApp;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Magazzino.DAL.DataLayer;
using AgronicaNetCore.Magazzino.DAL.DataLayer.CategorieMagazzino;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.Fertilizzanti;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.Formulati;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiImpostazioni;
using Microsoft.Extensions.Localization;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Prodotti
{
    public class Prodotti_APPService : BaseServiceAppBIZ, IProdotti_APPService
    {
        private readonly IMateriePrime_APP _materiePrimeApp;
        private readonly ICodificaProdotti_APP _codificaProdottiApp;
        private readonly IUtentiImpostazioni _utentiImpostazioni;
        private readonly IMagazzino _magazzino;
        private readonly IFabbricati_APP _magazzinoAPP;
        private readonly IImpianti _impianti;
        private readonly ICategorieMagazzino _categorieMagazzino;
        private readonly IFormulati _formulati;
        private readonly IFertilizzanti _fertilizzanti;

        // Cache a livello di istanza per evitare letture ripetute sulle stesse chiamate
        private bool leggiImpostazioneJoinCacPivaSuperUser = true;
        private string _cachedImpostazioneJoinCacPivaSuperUser = string.Empty;

        private bool leggiDtCacCodificaProdotti = true;
        private DataTable _cachedDtCacCodificaProdotti = new();

        private bool leggiMagazziniAPP = true;
        private List<(string Piva, int SaCod, int FabbricatoCod)> _cachedMagazziniAPP = new();

        private bool leggiJoinSpecieUtilizzate = true;
        private List<int> _cachedJoinSpecieUtilizzate = new();

        private DataTable? _dtCategorieMagazzino; // Cache a livello di istanza per categorie magazzino

        public Prodotti_APPService(
            IMateriePrime_APP materiePrime,
            ICodificaProdotti_APP codificaProdotti,
            IUtentiImpostazioni utentiImpostazioni,
            IMagazzino magazzino,
            IFabbricati_APP fabbricati,
            IImpianti impianti,
            ICategorieMagazzino categorieMagazzino,
            IFormulati formulati,
            IFertilizzanti fertilizzanti,
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer
        )
            : base(provider, localizer)
        {
            _materiePrimeApp = materiePrime;
            _codificaProdottiApp = codificaProdotti;
            _utentiImpostazioni = utentiImpostazioni;
            _magazzino = magazzino;
            _impianti = impianti;
            _magazzinoAPP = fabbricati;
            _categorieMagazzino = categorieMagazzino;
            _formulati = formulati;
            _fertilizzanti = fertilizzanti;
        }

        public async Task<List<ProdottoEntity>> LeggiProdottiAPPAsync(
            string piva,
            int elemCod,
            string specieProdotti,
            string statoCod,
            bool limitazioniPianoColturale,
            bool leggiNonMovimentati,
            AgronicaCoreParametriUtenti objParametriUtenti,
            AgronicaCoreParametriServer objParametriServer,
            ImpostazioniAppModel? impostazioniApp = null,
            bool modalitaDemetra = false
        )
        {
            try
            {
                if (string.IsNullOrEmpty(statoCod))
                    statoCod = "IT";

                DataTable dtProdotti = await ElencoProdottiAsync(
                    piva,
                    elemCod,
                    specieProdotti,
                    statoCod,
                    limitazioniPianoColturale,
                    leggiNonMovimentati,
                    objParametriUtenti,
                    objParametriServer,
                    impostazioniApp,
                    modalitaDemetra
                );

                var result = new List<ProdottoEntity>();
                var seen = new HashSet<(int codice, int elemCod)>();

                foreach (DataRow row in dtProdotti.Rows)
                {
                    int codice = row.Field<int?>("Prodotto_Cod") ?? 0;
                    int elem = row.Field<int?>("Elem_Cod") ?? 0;

                    if (!seen.Add((codice, elem)))
                        continue;

                    var item = new ProdottoEntity
                    {
                        codice = codice,
                        elemCod = elem,
                        unitaDiMisuraCod = row.Field<int?>("Udm_Cod") ?? 0,
                        nomeComune = row.Field<string>("NomeComune") ?? string.Empty,
                        descrizione = row.Field<string>("Prodotto_Des") ?? string.Empty,
                        specieCod = row.Field<int?>("Specie_Cod") ?? 0,
                        N = (double)(row.Field<decimal?>("N") ?? 0m),
                        P2O5 = (double)(row.Field<decimal?>("P2O5") ?? 0m),
                        K20 = (double)(row.Field<decimal?>("K2O") ?? 0m),
                        Cu = (double)(row.Field<decimal?>("Cu") ?? 0m),
                    };

                    result.Add(item);
                }

                return result;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        /// <summary>
        /// Corrisponde a ElencoProdotti nel codice VB legacy.
        /// Versione semplificata per contesto APP (xGiasApp=true, soloInGiacenza=false, Cau_Mov=CAU_CARICO).
        /// </summary>
        private async Task<DataTable> ElencoProdottiAsync(
            string piva,
            int elemCod,
            string specieProdotti,
            string statoCod,
            bool limitazioniPianoColturale,
            bool leggiNonMovimentati,
            AgronicaCoreParametriUtenti objParametriUtenti,
            AgronicaCoreParametriServer objParametriServer,
            ImpostazioniAppModel? impostazioniApp = null,
            bool modalitaDemetra = false
        )
        {
            var dataMovimento = DateTime.Now;

            List<string> spevarList = NormalizzaSpecieVarieta(specieProdotti);

            if (
                (limitazioniPianoColturale || modalitaDemetra)
                && leggiJoinSpecieUtilizzate == true
            )
            {
                // Leggo le specie utilizzate in piano colturale (cache a livello di istanza)
                DateTime? dataInizioSpecie = null;
                int sincroAnni = impostazioniApp?.SincroAnni ?? 0;
                if (sincroAnni > 0)
                {
                    int baseYear = DateTime.Today.Month < 11 ? DateTime.Today.Year - 1 : DateTime.Today.Year;
                    dataInizioSpecie = new DateTime(baseYear - sincroAnni, 11, 1, 0, 0, 0, DateTimeKind.Local);
                }

                DataTable specie = await _impianti.LeggiColturexPivaAsync(
                    new List<string>() { piva },
                    objParametriServer,
                    dataInizioSpecie,
                    soloAttiviOggi: dataInizioSpecie == null,
                    modalitaDemetra
                );
                _cachedJoinSpecieUtilizzate =
                    specie.AsEnumerable().Select(r => r.Field<int>("Veg_Cod")).Distinct().ToList()
                    ?? new();
                leggiJoinSpecieUtilizzate = false;
            }

            if (leggiImpostazioneJoinCacPivaSuperUser)
            {
                // Leggo impostazione (cached per istanza)
                _cachedImpostazioneJoinCacPivaSuperUser ??=
                    await _utentiImpostazioni.ImpostazioneValore1_from_ImpostazioneCod(
                        Enum_Impostazioni_Utenti.SUPERUSER_JoinCacPivaSuperUser,
                        (int)ModalitaLeggiImpostazioniUtente.SuperUser,
                        objParametriUtenti,
                        objParametriServer
                    );
                leggiImpostazioneJoinCacPivaSuperUser = false;
            }

            // Leggo CAC codifica prodotti aziendali (cached per istanza)
            // Se impostazioneJoinCacPivaSuperUser = "1" → decodifica per SuperUser (leggo senza piva)
            // Se piva è vuota e impostazione = "0" → non si mostra il codice prodotto cliente
            bool joinCacPerSuperUser = _cachedImpostazioneJoinCacPivaSuperUser == "1";
            if (
                leggiDtCacCodificaProdotti
                && (!string.IsNullOrWhiteSpace(piva) || joinCacPerSuperUser)
            )
            {
                string pivaDaUsare = joinCacPerSuperUser ? string.Empty : piva;
                _cachedDtCacCodificaProdotti = await _codificaProdottiApp.ReadAsync(
                    pivaDaUsare,
                    Enum_Tipo_CAC_Codifica_ProdottiAziendali.NonDefinito,
                    0,
                    objParametriServer,
                    soloMappati: true
                );
                leggiDtCacCodificaProdotti = false;
            }

            if (leggiMagazziniAPP)
            {
                DataTable dtMagazziniAPP = await _magazzinoAPP.ReadAsync(piva, objParametriServer);
                _cachedMagazziniAPP =
                    dtMagazziniAPP
                        .AsEnumerable()
                        .Select(r =>
                            (
                                Piva: r.Field<string>("piva") ?? string.Empty,
                                SaCod: r.Field<int?>("SA_COD") ?? 0,
                                FabbricatoCod: r.Field<int?>("Fabbricato_Cod") ?? 0
                            )
                        )
                        .ToList()
                    ?? new();
                leggiMagazziniAPP = false;
            }

            var dtProdotti = CreaDtProdotti();

            switch (elemCod)
            {
                case ELEM_COD.SEMENTI:
                case ELEM_COD.TRASFORMATI_VEGETALI:
                    await ProdottiMovimentatiAsync(
                        elemCod,
                        piva,
                        dataMovimento,
                        objParametriServer,
                        objParametriUtenti,
                        dtProdotti
                    );
                    if (leggiNonMovimentati)
                        await MateriePrimePubblicheAsync(
                            elemCod,
                            piva,
                            objParametriServer,
                            dtProdotti
                        );
                    break;

                case ELEM_COD.FERTILIZZANTI:
                case ELEM_COD.FORMULATI:
                case ELEM_COD.INSETTI:
                    List<int> vegCodList = spevarList
                        .Select(s => int.Parse(s.Split('|')[0]))
                        .ToList();
                    if (vegCodList.Count == 0 && limitazioniPianoColturale)
                        vegCodList = _cachedJoinSpecieUtilizzate;

                    if (leggiNonMovimentati)
                        await CercaProdottiBancheDatiAsync(
                            elemCod,
                            dtProdotti,
                            objParametriServer,
                            statoCod,
                            vegCodList: spevarList.Select(s => int.Parse(s.Split('|')[0])).ToList()
                        );
                    await ProdottiMovimentatiAsync(
                        elemCod,
                        piva,
                        dataMovimento,
                        objParametriServer,
                        objParametriUtenti,
                        dtProdotti
                    );
                    break;
            }

            return dtProdotti;
        }

        /// <summary>
        /// Corrisponde al ramo CAU_CARICO di CaricaListControl.Materie_Prime del legacy.
        /// Legge l'anagrafica materie prime e popola dtProdotti. xGiasApp=true, LeggiAlias=false.
        /// </summary>
        private async Task MateriePrimePubblicheAsync(
            int elemCod,
            string piva,
            AgronicaCoreParametriServer objParametriServer,
            DataTable dtProdotti
        )
        {
            DataTable dtMateriePrime = await _materiePrimeApp.ReadAnagraficaAsync(
                piva,
                elemCod,
                _cachedJoinSpecieUtilizzate ?? new(),
                objParametriServer
            );

            foreach (DataRow row in dtMateriePrime.Rows)
            {
                int matCod = row.Field<int?>("Mat_Cod") ?? 0;
                string matDes = row.Field<string>("Mat_Des") ?? string.Empty;
                string codArticolo = row.Field<string>("Cod_Articolo") ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(codArticolo))
                    matDes += $" (Cod. Articolo: {codArticolo})";

                var dr = dtProdotti.NewRow();
                dr["Elem_Cod"] = elemCod;
                dr["Prodotto_Cod"] = -matCod;
                dr["Prodotto_Des"] = matDes;
                dr["NomeComune"] = row.Field<string>("NomeComune") ?? string.Empty;
                dr["Udm_Cod"] = row.Field<int?>("Udm_Cod") ?? 0;
                dr["Extra_Str"] = row.Field<string>("Extra_Str") ?? string.Empty;
                dtProdotti.Rows.Add(dr);
            }
        }

        private async Task ProdottiMovimentatiAsync(
            int elemCod,
            string piva,
            DateTime dataMovimento,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriUtenti objParametriUtenti,
            DataTable dtProdotti
        )
        {
            DataTable prodottiInGiacenza = await _magazzino.SchedaGiacenzeMagazzino(
                dataMovimento,
                piva,
                Sa_Cod: 0,
                Id_Destinazione: 0,
                elemCod,
                Pro_Cod: 0,
                Mat_Cod: 0,
                Cal_Cod: 0,
                Cod_Progetto: COSTANTI_GENERALI.CODPROGETTO_NONDEFINITO,
                Fase_Cod: 0,
                Udm_Cod: 0,
                Lotto: COSTANTI_GENERALI.LOTTO_NONDEFINITO,
                Flag_QtaNoZero: false,
                xFiltroAggiuntivo: null,
                xFiltroAggiuntivo_1: null,
                xFiltroAggiuntivo_2: null,
                xFiltroAggiuntivo_3: null,
                xFiltroAggiuntivo_4: null,
                xFiltroAggiuntivo_5: null,
                xFiltroAggiuntivo_6: null,
                xFiltroAggiuntivo_7: null,
                xFiltroAggiuntivo_8: null,
                xFiltroAggiuntivo_9: null,
                xFiltroAggiuntivo_10: null,
                xFiltroAggiuntivo_12: null,
                xOrderBy: string.Empty,
                objParametriServer,
                objParametriUtenti,
                joinTempMagazzini: _cachedMagazziniAPP ?? new(),
                joinTempSpecie: _cachedJoinSpecieUtilizzate ?? new()
            );

            // Per i fertilizzanti, carica i macro-elementi (N/P2O5/K2O/Cu) in un'unica query
            Dictionary<int, DataRow> macroElementiLookup = new();
            if (elemCod == ELEM_COD.FERTILIZZANTI && prodottiInGiacenza.Rows.Count > 0)
            {
                var ferCodList = prodottiInGiacenza
                    .AsEnumerable()
                    .Select(r => r.Field<int?>("Pro_Cod") ?? 0)
                    .Where(c => c != 0)
                    .Distinct()
                    .ToList();

                if (ferCodList.Count > 0)
                {
                    DataTable dtMacro = await _fertilizzanti.LeggiMacroElementiAsync(
                        ferCodList,
                        objParametriServer
                    );
                    macroElementiLookup = dtMacro
                        .AsEnumerable()
                        .ToDictionary(r => r.Field<int>("Fer_Cod"));
                }
            }

            var seenProdotti = new HashSet<int>();

            foreach (DataRow row in prodottiInGiacenza.Rows)
            {
                int prodottoCod =
                    elemCod == ELEM_COD.FERTILIZZANTI
                    || elemCod == ELEM_COD.FORMULATI
                    || elemCod == ELEM_COD.INSETTI
                        ? row.Field<int?>("Pro_Cod") ?? 0
                        : -(row.Field<int?>("Mat_Cod") ?? 0);

                if (!seenProdotti.Add(prodottoCod))
                    continue;

                var dr = dtProdotti.NewRow();

                if (
                    elemCod == ELEM_COD.FERTILIZZANTI
                    || elemCod == ELEM_COD.FORMULATI
                    || elemCod == ELEM_COD.INSETTI
                )
                {
                    dr["Prodotto_Cod"] = prodottoCod;
                    dr["Prodotto_Des"] =
                        $"{row.Field<string>("Descrizione_Prodotto") ?? string.Empty} ({row.Field<int?>("Pro_Cod")})";

                    var cacRow = _cachedDtCacCodificaProdotti
                        ?.AsEnumerable()
                        .FirstOrDefault(r =>
                            r.Field<int?>("Elem_Cod") == elemCod
                            && r.Field<int?>("Codice_GIAS") == row.Field<int?>("Pro_Cod")
                        );
                    string codArticolo = cacRow?.Field<string>("Cod_Articolo") ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(codArticolo))
                        dr["Prodotto_Des"] += $" (Cod. Articolo: {codArticolo})";
                }
                else
                {
                    //Il codice è impostato con il segno negativo, non so perchè :(
                    //Prodotti.asmx/ElencoProdotti() -->  clc.Materie_Prime
                    dr["Prodotto_Cod"] = prodottoCod;
                    dr["Prodotto_Des"] = row.Field<string>("Descrizione_Prodotto") ?? string.Empty;

                    string codArticolo = row.Field<string>("Cod_Articolo") ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(codArticolo))
                        dr["Prodotto_Des"] += $" (Cod. Articolo: {codArticolo})";
                }


                dr["Elem_Cod"] = elemCod;
                dr["NomeComune"] = row.Field<string>("NomeComune") ?? string.Empty;
                dr["Udm_Cod"] = row.Field<int?>("Udm_Cod") ?? 0;
                dr["Extra_Str"] = row.Field<string>("Extra_Str") ?? string.Empty;

                if (elemCod == ELEM_COD.FERTILIZZANTI)
                {
                    macroElementiLookup.TryGetValue(prodottoCod, out var macroRow);
                    dr["N"] =
                        macroRow == null || macroRow["N"] == DBNull.Value
                            ? 0m
                            : Convert.ToDecimal(macroRow["N"]);
                    dr["P2O5"] =
                        macroRow == null || macroRow["P2O5"] == DBNull.Value
                            ? 0m
                            : Convert.ToDecimal(macroRow["P2O5"]);
                    dr["K2O"] =
                        macroRow == null || macroRow["K2O"] == DBNull.Value
                            ? 0m
                            : Convert.ToDecimal(macroRow["K2O"]);
                    dr["Cu"] =
                        macroRow == null || macroRow["Cu"] == DBNull.Value
                            ? 0m
                            : Convert.ToDecimal(macroRow["Cu"]);
                }

                if (elemCod == ELEM_COD.FORMULATI)
                {
                    if (prodottiInGiacenza.Columns.Contains("IsTrappolaFormulato"))
                        dr["IsTrappolaFormulato"] =
                            bool.TryParse(
                                row["IsTrappolaFormulato"]?.ToString(),
                                out var isTrappola
                            ) && isTrappola;
                }

                if (elemCod == ELEM_COD.TRASFORMATI_VEGETALI)
                    dr["Specie_Cod"] = row.Field<int>("Veg_Cod");

                dtProdotti.Rows.Add(dr);
            }
        }

        /// <summary>
        /// Normalizza le coppie specie/varietà  da usare come filtro. Solo Elenco_Specie è popolato.
        /// Porta la logica VB di NormalizzaSpecieVarieta (ramo varieta_Array.Length == 0).
        /// </summary>
        /// <param name="elencoPiante">Codici specie separati da | (es. "3|10")</param>
        /// <returns>Lista di coppie "vegCod|0"</returns>
        private static DataTable CreaDtProdotti()
        {
            var dt = new DataTable();
            dt.Columns.Add(new DataColumn("Elem_Cod", typeof(int)));
            dt.Columns.Add(new DataColumn("Prodotto_Cod", typeof(int)));
            dt.Columns.Add(new DataColumn("Prodotto_Des", typeof(string)));
            dt.Columns.Add(new DataColumn("NomeComune", typeof(string)));
            dt.Columns.Add(new DataColumn("N", typeof(decimal)) { DefaultValue = 0m });
            dt.Columns.Add(new DataColumn("P2O5", typeof(decimal)) { DefaultValue = 0m });
            dt.Columns.Add(new DataColumn("K2O", typeof(decimal)) { DefaultValue = 0m });
            dt.Columns.Add(new DataColumn("Cu", typeof(decimal)) { DefaultValue = 0m });
            dt.Columns.Add(new DataColumn("Uso", typeof(int)) { DefaultValue = 0 });
            dt.Columns.Add(new DataColumn("Specie_Cod", typeof(int)) { DefaultValue = 0 });
            dt.Columns.Add(new DataColumn("Udm_Cod", typeof(int)));
            dt.Columns.Add(new DataColumn("Extra_Str", typeof(string)));
            dt.Columns.Add(
                new DataColumn("IsTrappolaFormulato", typeof(bool)) { DefaultValue = false }
            );
            return dt;
        }

        private static List<string> NormalizzaSpecieVarieta(string elencoPiante)
        {
            if (string.IsNullOrEmpty(elencoPiante))
                return new List<string>();

            return elencoPiante
                .Split('|', StringSplitOptions.RemoveEmptyEntries)
                .Select(spe => spe + "|0")
                .ToList();
        }

        /// <summary>
        /// Corrisponde al ramo CAU_CARICO di CercaProdottiBancheDati del legacy.
        /// Versione semplificata: xGiasApp=true, cauMov=CAU_CARICO, gruppiMerce/trasferimento non gestiti.
        /// </summary>
        private async Task CercaProdottiBancheDatiAsync(
            int elemCod,
            DataTable dtProdotti,
            AgronicaCoreParametriServer objParametriServer,
            string statoCod,
            List<int> vegCodList
        )
        {
            DataTable dtRisultato;
            if (elemCod == ELEM_COD.FORMULATI)
            {
                dtRisultato = await _formulati.LeggiAsync(
                    -(int)Enum_TipoFormulato.InstallazioneTrappoleCattureMassa,
                    statoCod,
                    vegCodList,
                    objParametriServer,
                    leggiAPP: true
                );
            }
            else
            {
                // tipoRichiesto=0: tutti i fertilizzanti; validità filtrate su oggi
                dtRisultato = await _fertilizzanti.LeggiAsync(
                    ferCod: 0,
                    ferDes: string.Empty,
                    tipoRichiesto: 0,
                    regolamentoCod: 0,
                    validitaInizio: DateTime.Today,
                    validitaFine: DateTime.Today,
                    includiTipologia: false,
                    statoCod: statoCod,
                    objParametriServer
                );
            }

            _dtCategorieMagazzino ??= await _categorieMagazzino.LeggiAsync(
                0,
                CAU_MOV.CAU_MAGAZZINO,
                false,
                objParametriServer
            );

            string wNomecomune =
                _dtCategorieMagazzino
                    .AsEnumerable()
                    .FirstOrDefault(cat => cat.Field<int?>("Elem_Cod") == elemCod)
                    ?.Field<string>("NomeComune")
                ?? string.Empty;

            foreach (DataRow elem in dtRisultato.Rows)
            {
                var drProdotti = dtProdotti.NewRow();

                if (elemCod == ELEM_COD.FERTILIZZANTI)
                {
                    var ferCod = elem.Field<int?>("Fer_Cod") ?? 0;
                    var cacRow = _cachedDtCacCodificaProdotti
                        ?.AsEnumerable()
                        .FirstOrDefault(r =>
                            r.Field<int?>("Elem_Cod") == elemCod
                            && r.Field<int?>("Codice_GIAS") == ferCod
                        );
                    string codArticolo = cacRow?.Field<string>("Cod_Articolo") ?? string.Empty;

                    drProdotti["Prodotto_Cod"] = ferCod;
                    // Flag_VisualizzaProCod=true ma cauMov=CAU_CARICO â†’ codice prodotto non accodato alla descrizione
                    string xProDes = elem.Field<string>("Fer_Des") ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(codArticolo))
                        xProDes += $" (Cod. Articolo: {codArticolo})";
                    drProdotti["Prodotto_Des"] = xProDes;
                    drProdotti["N"] = elem.Field<decimal?>("N") ?? 0m;
                    drProdotti["P2O5"] = elem.Field<decimal?>("P2O5") ?? 0m;
                    drProdotti["K2O"] = elem.Field<decimal?>("K2O") ?? 0m;
                    drProdotti["Cu"] = elem.Field<decimal?>("Cu") ?? 0m;
                }
                else if (elemCod == ELEM_COD.FORMULATI)
                {
                    var frCod = elem.Field<int?>("Fr_Cod") ?? 0;
                    var cacRow = _cachedDtCacCodificaProdotti
                        ?.AsEnumerable()
                        .FirstOrDefault(r =>
                            r.Field<int?>("Elem_Cod") == elemCod
                            && r.Field<int?>("Codice_GIAS") == frCod
                        );
                    string codArticolo = cacRow?.Field<string>("Cod_Articolo") ?? string.Empty;

                    drProdotti["Prodotto_Cod"] = frCod;
                    string xProDesFr = elem.Field<string>("Fr_Des") ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(codArticolo))
                        xProDesFr += $" (Cod. Articolo: {codArticolo})";
                    drProdotti["Prodotto_Des"] = xProDesFr;

                    if (dtRisultato.Columns.Contains("IsTrappolaFormulato"))
                        drProdotti["IsTrappolaFormulato"] =
                            bool.TryParse(
                                elem["IsTrappolaFormulato"]?.ToString(),
                                out var _isTrappola
                            ) && _isTrappola;
                }
                else
                {
                    continue;
                }

                drProdotti["Elem_Cod"] = elemCod;
                drProdotti["NomeComune"] = wNomecomune;
                dtProdotti.Rows.Add(drProdotti);
            }
        }
    }
}
