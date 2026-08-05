using AgronicaCoreDTOStd.InData.Anagrafica;
using AgronicaNetCore.Anagrafe.BIZ.Resources;
using AgronicaNetCore.Anagrafe.BIZ.Services.AlberoGerarchiaImprese.Exceptions;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.GerarchiaImprese;
using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiDettagli;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiProfili;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Data;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.Anagrafe.BIZ.Services.AlberoGerarchiaImprese
{
    public class AlberoGerarchiaImpreseFASTService : BaseServiceAnagrafeBIZ, IAlberoGerarchiaImpreseFASTService
    {
        private IUtentiDettagli _utentiDettagli;
        private IUtentiProfili _utentiProfili;
        private IGerarchiaImprese _gerarchiaImprese;

        public AlberoGerarchiaImpreseFASTService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _utentiDettagli = _serviceProvider.GetRequiredService<IUtentiDettagli>();
            _utentiProfili = _serviceProvider.GetRequiredService<IUtentiProfili>();
            _gerarchiaImprese = _serviceProvider.GetRequiredService<IGerarchiaImprese>();
        }

        public async Task<KendoHierarchicalDataSource> GetNodesGearchiaObjAsync(AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriUtenti objParametriUtenti)
        {

            var livelloImpresa = new List<AjaxTreeNodeFASTJsonObject>();
            var utentiDettagliDT = await _utentiDettagli.LeggiAsync(5, objParametriUtenti, objParametriServer);

            string testo;
            int tipoUtente;

            if (utentiDettagliDT.Rows.Count > 0)
            {
                var primaRiga = utentiDettagliDT.Rows[0];
                tipoUtente = (int)primaRiga["Flag_Azienda_Persona"];
                if (tipoUtente == 1)
                    testo = (primaRiga["Rag_Soc"] as string)!;
                else
                    testo = string.Format("{0} {1}", primaRiga["Cognome"], primaRiga["Nome"]);
            }
            else
            {
                testo = objParametriServer.UtenteUsername;
            }

            var text = string.Format("{0}{1}", CostantiPersonalizzate.AgroPrefix_Utente, testo);
            AjaxTreeNodeFASTJsonObject radice = new AjaxTreeNodeFASTJsonObject(text, TipoNodo.Utente, children: livelloImpresa);

            radice.State.Opened = false;

            var username = objParametriUtenti.UtenteUsername;
            bool filtroVisibilitaUtente = !await _utentiProfili.VisibilitaTotale(
                username, objParametriUtenti, objParametriServer, (int)Enum_Id_Servizio.GiasOnline);
            DataTable gerarchieDt;

            try
            {
                gerarchieDt = await _gerarchiaImprese.LeggixGerarchiaAlberoImprese_VisibilitaAsync(filtroVisibilitaUtente, objParametriServer);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            var results = new List<AjaxTreeNodeFASTJsonObject>();
            var pivaPadre = string.Empty;

            RestituisciNodoImpresa(results, gerarchieDt, pivaPadre);
            radice.Children!.AddRange(results);

            var listaRadice = new List<AjaxTreeNodeFASTJsonObject>();
            listaRadice.Add(radice);

            var kendoHierarchical = new KendoHierarchicalDataSource();
            AjaxTreeNodeJsonObjectConverter.AjaxTreeNodeJsonObject_KendoHierarchical(listaRadice, kendoHierarchical);

            return kendoHierarchical;
        }


        /// <summary>
        /// Recupera il cono di visibilità organizzativo di un Utente specifico interrogando il database GIAS.
        /// </summary>
        /// <remarks>
        /// Design Specification DS01-BL: RecuperoConoVisibilitaUtente - Descrizione e Regole di Business.
        /// Passi:
        ///   1. Validazione input (IdUtente non nullo/vuoto, max 256 caratteri).
        ///   2. Lettura <c>Utenti_Profili</c> via <c>IUtentiProfili.ReadAsync</c> per determinare visibilità totale o filtrata.
        ///   3. Query <c>GerarchiaImprese</c> con filtro opzionale su <c>Utenti_Visibilita_Appoggio</c>.
        ///   4. Mapping del result set a <see cref="ConoVisibilitaResponseDto"/> raggruppando per PivaPadre.
        /// </remarks>
        public async Task<ConoVisibilitaResponseDto> GetConoVisibilitaUtenteAsync(
            string idUtente,
            int timeoutMs,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriUtenti objParametriUtenti)
        {
            // 1. Validazione input — DS01-BL: Regole di Business, Validazione Input
            if (string.IsNullOrWhiteSpace(idUtente) || idUtente.Length > 256)
                throw new InvalidUserException(idUtente);

            // 2. Verifica esistenza utente e determinazione visibilità — DS01-BL: Logica di Visibilità
            objParametriUtenti.UtenteUsername = idUtente;
            var profiloDt = await _utentiProfili.ReadAsync(objParametriUtenti, objParametriServer, 5);

            if (profiloDt == null || profiloDt.Rows.Count == 0)
                throw new UserNotFoundException(idUtente);

            var username = objParametriUtenti.UtenteUsername;
            bool applicaFiltroVisibilita = !await _utentiProfili.VisibilitaTotale(
                username, objParametriUtenti, objParametriServer, (int)Enum_Id_Servizio.GiasOnline);

            // 3. Query con timeout — DS01-BL: Timeout Query
            using var cts = new CancellationTokenSource(timeoutMs);
            DataTable gerarchiaDt;
            try
            {
                var queryTask = _gerarchiaImprese.LeggiConoVisibilitaAsync(idUtente, applicaFiltroVisibilita, objParametriServer);
                gerarchiaDt = await queryTask.WaitAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                throw new QueryTimeoutException(timeoutMs);
            }

            // 4. Mapping — DS01-BL: Raggruppamento per Filiera e Array Vuoto se Nessuna Visibilità
            try
            {
                return MapToConoVisibilita(gerarchiaDt);
            }
            catch (Exception ex) when (ex is not QueryTimeoutException and not InvalidUserException and not UserNotFoundException)
            {
                throw new DataMappingException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Raggruppa le righe del result set SQL per <c>PivaPadre</c> e costruisce il <see cref="ConoVisibilitaResponseDto"/>.
        /// </summary>
        /// <remarks>
        /// Design Specification DS01-BL: RecuperoConoVisibilitaUtente - Regole di Business, Raggruppamento per Filiera.
        /// </remarks>
        private static ConoVisibilitaResponseDto MapToConoVisibilita(DataTable dt)
        {
            var parentOf = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var capofiliere = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var visibili = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var cuaaMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (DataRow row in dt.Rows)
            {
                string pivaPadre = row.Field<string>("PivaPadre")!;
                string pivaFiglio = row.Field<string>("PivaFiglio")!;
                int livello = Convert.ToInt32(row["Livello"]);
                int foglia = Convert.ToInt32(row["Foglia"]);

                cuaaMap[pivaPadre] = row.Field<string>("CuaaPadre") ?? pivaPadre;
                cuaaMap[pivaFiglio] = row.Field<string>("CuaaFiglio") ?? pivaFiglio;

                parentOf[pivaFiglio] = pivaPadre;
                visibili.Add(pivaFiglio);

                // Regola: La vera filiera è a Livello 2 e Foglia 0
                if (livello == 2 && foglia == 0) capofiliere.Add(pivaFiglio);
            }

            var filiereDict = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
            foreach (var cap in capofiliere)
            {
                // Creiamo il contenitore vuoto, MA NON AGGIUNGIAMO PIU' LA CAPOFILIERA A SE STESSA!
                filiereDict[cap] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                filiereDict[cap].Add(cap);
            }

            foreach (var piva in visibili)
            {
                if (capofiliere.Contains(piva)) continue; // Saltiamo le capofiliere, non vanno nei figli

                string root = piva;
                while (parentOf.ContainsKey(root) && !capofiliere.Contains(root)) root = parentOf[root];

                if (capofiliere.Contains(root)) filiereDict[root].Add(piva);
            }

            var filiere = new List<FilieraDto>();
            foreach (var kvp in filiereDict)
            {
                // La filiera si mostra se è fine a se stessa (e visibile) OPPURE se ha figli visibili
                if (visibili.Contains(kvp.Key) || kvp.Value.Count > 0)
                {
                    filiere.Add(new FilieraDto
                    {
                        IdFiliera = cuaaMap.GetValueOrDefault(kvp.Key, kvp.Key),
                        IdAziende = kvp.Value.Select(p => cuaaMap.GetValueOrDefault(p, p)).ToList()
                    });
                }
            }

            return new ConoVisibilitaResponseDto { Filiere = filiere };
        }

        /// <summary>
        /// Recupera la struttura organizzativa completa di tutte le Filiere e Aziende.
        /// </summary>
        /// <remarks>
        /// Design Specification DS02-BL: RecuperoStrutturaFiliereTotale - Descrizione e Regole di Business.
        /// Passi:
        ///   1. Esecuzione in sequenza delle due query (struttura filiere + admin) con timeout comune.
        ///   2. Validazione della gerarchia (cicli via DFS).
        ///   3. Mapping a StrutturaFiliereResponseDto con ordinamento deterministico.
        ///   4. Associazione admin per PivaFiliera.
        /// </remarks>
        public async Task<StrutturaFiliereResponseDto> GetStrutturaFiliereAsync(
            int timeoutMs,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriUtenti objParametriUtenti,
            AgronicaCoreParametriSuperServer objParametriSuperServer)
        {
            // 1. Query con timeout — DS02-BL: Timeout Query
            using var cts = new CancellationTokenSource(timeoutMs);
            DataTable strutturaDt;
            Dictionary<string, FilieraAdminDto> adminLookup;

            try
            {
                strutturaDt = await _gerarchiaImprese.LeggiStrutturaFiliereAsync(objParametriServer)
                    .WaitAsync(cts.Token);
                adminLookup = await _gerarchiaImprese.LeggiAdminFiliereAsync(objParametriUtenti)
                    .WaitAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                throw new QueryTimeoutException(timeoutMs);
            }

            // 2. Validazione gerarchia — DS02-BL: Validazione Gerarchia
            ValidaGerarchia(strutturaDt);

            // 3 + 4. Mapping, ordinamento, admin
            try
            {
                return MapToStrutturaFiliere(strutturaDt, adminLookup);
            }
            catch (Exception ex) when (ex is not QueryTimeoutException and not HierarchyValidationException)
            {
                throw new DataMappingException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Valida l'assenza di cicli nella gerarchia usando DFS.
        /// </summary>
        /// <remarks>
        /// Design Specification DS02-BL: RecuperoStrutturaFiliereTotale - Regole di Business,
        /// Validazione Gerarchia. Rileva cicli (es. A→B→C→A) ispezionando il grafo padre→figlio
        /// costruito dai soli rows con Livello &gt; 0.
        /// </remarks>
        private static void ValidaGerarchia(DataTable dt)
        {
            // Costruiamo grafo diretto PivaAzienda -> PivaPadreDirecto per rilevare cicli.
            // Con la nuova query (senza UNION ALL) ogni riga ha sempre un PivaPadreDirecto valido.
            var parentOf = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (DataRow row in dt.Rows)
            {
                var pivaAzienda = row.Field<string>("PivaAzienda")!;
                var pivaPadre = row.Field<string>("PivaPadreDirecto")!;
                parentOf[pivaAzienda] = pivaPadre;
            }

            var visitati = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var stack = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var nodo in parentOf.Keys)
            {
                if (!visitati.Contains(nodo))
                    DfsRilevaciCiclo(nodo, parentOf, visitati, stack);
            }
        }

        private static void DfsRilevaciCiclo(
            string nodo,
            Dictionary<string, string> parentOf,
            HashSet<string> visitati,
            HashSet<string> stack)
        {
            visitati.Add(nodo);
            stack.Add(nodo);

            if (parentOf.TryGetValue(nodo, out var padre))
            {
                if (stack.Contains(padre))
                    throw new HierarchyValidationException(
                        $"Ciclo rilevato nella gerarchia: il nodo '{padre}' è raggiunto in modo circolare da '{nodo}'.");
                if (!visitati.Contains(padre))
                    DfsRilevaciCiclo(padre, parentOf, visitati, stack);
            }

            stack.Remove(nodo);
        }

        /// <summary>
        /// Costruisce la lookup <c>PivaFiliera → FilieraAdminDto</c> dagli admin recuperati.
        /// L'associazione admin→filiera avviene tramite <c>PIVA</c> in <c>Utenti_Dettagli</c>.
        /// </summary>
        /// <remarks>
        /// Design Specification DS02-BL: RecuperoStrutturaFiliereTotale - Regole di Business,
        /// Mapping Admin di Filiera.
        /// </remarks>
        private static Dictionary<string, FilieraAdminDto> BuildAdminLookup(DataTable adminDt)
        {
            var lookup = new Dictionary<string, FilieraAdminDto>(StringComparer.OrdinalIgnoreCase);
            foreach (DataRow row in adminDt.Rows)
            {
                var pivaFiliera = row.Field<string>("PivaFiliera");
                if (string.IsNullOrEmpty(pivaFiliera)) continue;

                // La prima corrispondenza vince (al massimo un admin per filiera per spec)
                if (!lookup.ContainsKey(pivaFiliera))
                {
                    lookup[pivaFiliera] = new FilieraAdminDto
                    {
                        IdUtente = row.Field<string>("UserName")!,
                        NomeUtente = $"{row.Field<string>("Cognome")} {row.Field<string>("Nome")}".Trim(),
                        EmailUtente = row.Field<string>("Email") ?? string.Empty
                    };
                }
            }
            return lookup;
        }

        /// <summary>
        /// Raggruppa le righe del result set per capofiliera, costruisce le
        /// <see cref="FilieraCompletaDto"/> e applica l'ordinamento deterministico.
        /// </summary>
        /// <remarks>
        /// Design Specification DS02-BL: RecuperoStrutturaFiliereTotale - Regole di Business:
        /// Ordinamento Deterministico (id_filiera ASC, livello_gerarchia ASC, id_azienda ASC),
        /// Livello Gerarchia Zero per Capofiliera.
        /// Le righe con Livello=0 sono le Capofiliere (PivaPadreDirecto = null).
        /// Per row con Livello&gt;0, la capofiliera di appartenenza è determinata risalendo
        /// la catena di padri diretti fino a un nodo radice.
        /// </remarks>
        private static StrutturaFiliereResponseDto MapToStrutturaFiliere(
            DataTable strutturaDt,
            Dictionary<string, FilieraAdminDto> adminLookup)
        {
            var filiereDictionary = new Dictionary<string, FilieraCompletaDto>(StringComparer.OrdinalIgnoreCase);
            var parentOf = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (DataRow row in strutturaDt.Rows)
            {
                var figlio = row.Field<string>("PivaAzienda");
                var padre = row.Field<string>("PivaPadreDirecto");
                if (!string.IsNullOrEmpty(figlio) && !string.IsNullOrEmpty(padre)) parentOf[figlio] = padre;
            }

            // Passo 1 — le Capofiliere sono a Livello 2 e Foglia 0
            foreach (DataRow row in strutturaDt.Rows)
            {
                int livello = Convert.ToInt32(row["Livello"]);
                int foglia = Convert.ToInt32(row["Foglia"]);
                string piva = row.Field<string>("PivaAzienda")!;

                if (livello == 2 && foglia == 0 && !filiereDictionary.ContainsKey(piva))
                {
                    // Emula il "LIKE '%piva%'" cercando la PIVA all'interno di Descrizione_2
                    var admin = adminLookup.FirstOrDefault(kvp => kvp.Key != null && kvp.Key.Contains(piva)).Value;
                    string cuaaAzienda = row.Field<string>("CuaaAzienda") ?? piva;
                    var filiera = new FilieraCompletaDto { IdFiliera = cuaaAzienda, NomeFiliera = row.Field<string>("NomeAzienda")!, FilieraAdmin = admin };

                    // RIMOSSO IL BLOCCO: La filiera non viene più inserita nell'elenco delle sue stesse aziende!

                    filiereDictionary[piva] = filiera;
                }
            }

            // Passo 2 — assegna ogni azienda figlia alla propria Capofiliera
            foreach (DataRow row in strutturaDt.Rows)
            {
                string piva = row.Field<string>("PivaAzienda")!;
                int livello = Convert.ToInt32(row["Livello"]);

                //if (livello < 2 || filiereDictionary.ContainsKey(piva)) continue;

                string root = piva;
                while (parentOf.ContainsKey(root) && !filiereDictionary.ContainsKey(root)) root = parentOf[root];

                if (filiereDictionary.TryGetValue(root, out var filiera))
                {
                    string cuaaAzienda = row.Field<string>("CuaaAzienda") ?? piva;
                    if (!filiera.Aziende.Any(a => string.Equals(a.IdAzienda, cuaaAzienda, StringComparison.OrdinalIgnoreCase)))
                    {
                        string cuaaPadreDirecto = row.Field<string>("CuaaPadreDirecto") ?? string.Empty;
                        if (livello < 2 || filiereDictionary.ContainsKey(piva)) cuaaPadreDirecto = string.Empty;
                        filiera.Aziende.Add(new AziendaDto
                        {
                            IdAzienda = cuaaAzienda,
                            RagioneSociale = row.Field<string>("NomeAzienda")!,
                            Sede = new SedeDto { Stato = row.Field<string>("Stato_ISO"), Regione = row.Field<string>("Regione"), Citta = row.Field<string>("Citta") },
                            Gerarchia = new GerarchiaDto { IdAziendaParent = cuaaPadreDirecto, LivelloGerarchia = livello - 2 }
                        });
                    }
                }
            }

            // Passo 3 — ordinamento deterministico
            var filiereOrdinate = filiereDictionary.Values.OrderBy(f => f.IdFiliera, StringComparer.OrdinalIgnoreCase).ToList();
            foreach (var f in filiereOrdinate) f.Aziende = f.Aziende.OrderBy(a => a.Gerarchia.LivelloGerarchia).ThenBy(a => a.IdAzienda, StringComparer.OrdinalIgnoreCase).ToList();

            return new StrutturaFiliereResponseDto { Filiere = filiereOrdinate };
        }

        private void RestituisciNodoImpresa(List<AjaxTreeNodeFASTJsonObject> results, DataTable gerarchieDt, string pivaPadre)
        {
            int tipoImpresaGerarchia;
            var xPiva = string.Empty;
            var xRag_Soc = string.Empty;
            var xCUAA = string.Empty;
            int? certificatiBloccati = null;

            var DR = gerarchieDt.Select(string.Format("Padre = '{0}'", pivaPadre));
            if (pivaPadre == string.Empty && !DR.Any())
            {
                var padriDt = gerarchieDt.DefaultView.ToTable(true, "Padre");
                foreach (DataRow row in padriDt.Rows)
                {
                    var padre = (row[0] as string)!;
                    var eFiglio = gerarchieDt.Select(string.Format("Piva = '{0}'", padre));

                    if (eFiglio.Length == 0)
                        RestituisciNodoImpresa(results, gerarchieDt, padre);
                }
                return;
            }

            foreach (var row in DR)
            {
                bool foglia;

                if ((short)row["Foglia"] == 1)
                    foglia = true;
                else
                {
                    foglia = false;
                    var piva = row["Piva"] as string;
                    if (!string.IsNullOrEmpty(piva))
                    {
                        var padreDR = gerarchieDt.Select(string.Format("Padre = '{0}'", piva));
                        if (padreDR.Length == 0)
                        {
                            continue;
                        }
                    }
                }

                xPiva = (row["Piva"] as string)!;
                xRag_Soc = (row["Rag_Soc"] as string)!;
                xCUAA = (row["CUAA"] as string)!;

                if (row["Blk_Flag"] is not null)
                    certificatiBloccati = (int)row["Blk_Flag"];

                tipoImpresaGerarchia = (int)row["TipoImpresaGerarchia"];

                if (certificatiBloccati == -1)
                    xRag_Soc = "--SOSPESA--" + xRag_Soc;

                var tipoImpresa = GetTipoImpresa(tipoImpresaGerarchia);

                var livello2 = new List<AjaxTreeNodeFASTJsonObject>();
                if (foglia)
                    livello2 = null;
                else
                    RestituisciNodoImpresa(livello2, gerarchieDt, xPiva);

                AjaxTreeNodeFASTJsonObject impresa;
                if (livello2 is null)
                    impresa = new AjaxTreeNodeFASTJsonObject(xRag_Soc, tipoImpresa, xPiva, pivaPadre, xCUAA);
                else
                    impresa = new AjaxTreeNodeFASTJsonObject(xRag_Soc, tipoImpresa, xPiva, pivaPadre, xCUAA, livello2);

                if (pivaPadre == string.Empty)
                    impresa.State.Opened = true;
                else
                    impresa.State.Opened = false;

                impresa.State.Selected = false;
                results.Add(impresa);
            }
        }

        private TipoNodo GetTipoImpresa(int tipoImpresaGerarchia)
        {
            if (tipoImpresaGerarchia == 1)
                return TipoNodo.Impresa;
            else if (tipoImpresaGerarchia == 2)
                return TipoNodo.x_Cooperativa;
            else if (tipoImpresaGerarchia == 3)
                return TipoNodo.x_Consorzio;
            else if (tipoImpresaGerarchia == 4)
                return TipoNodo.x_OP;
            else
                return TipoNodo.Utente;
        }


    }

    public enum TipoNodo
    {
        Indefinito = 0,
        Utente = 1,
        Impresa = 2,
        x_Cooperativa = 38,
        x_Consorzio = 39,
        x_OP = 40,
    }
}
