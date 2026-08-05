using AgronicaCoreDTOStd.InData.Anagrafica;
using AgronicaNetCore.Anagrafe.DAL.Resources;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Text;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.GerarchiaImprese
{
    public class GerarchiaImprese : BaseDALAnagrafe, IGerarchiaImprese
    {
        public GerarchiaImprese(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer) { }

        /// <inheritdoc/>
        public async Task<DataTable> LeggixGerarchiaAlberoImprese_VisibilitaAsync(bool applicaVisibilita, AgronicaCoreParametriServer objParametriServer, int? livello = null)
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            DataTable dt;

            stbQuery.AppendLine(" SELECT DISTINCT a.Padre, a.rag_soc, a.PIVA, ISNULL(IC.Val_Cod, '') AS CUAA, a.Foglia, a.Validazione, a.TipoImpresaGerarchia, a.Blk_Flag, a.validita_fine ");
            stbQuery.AppendLine(" FROM");
            stbQuery.AppendLine(" ( ");
            stbQuery.AppendLine("   SELECT      GerarchiaImprese.Padre, Imprese.rag_soc, Imprese.PIVA, GerarchiaImprese.Foglia, Imprese.Validazione, Imprese.TipoImpresaGerarchia , Imprese.Blk_Flag, Imprese.validita_fine  ");
            stbQuery.AppendLine("   FROM        GerarchiaImprese (NOLOCK) ");
            stbQuery.AppendLine("   INNER JOIN  Imprese (NOLOCK) ");
            stbQuery.AppendLine("   ON          GerarchiaImprese.Figlio = Imprese.PIVA   ");

            if (applicaVisibilita)
            {
                stbQuery.AppendLine("   LEFT JOIN   Utenti_Visibilita_Appoggio uvap (NOLOCK)  ");
                stbQuery.AppendLine("   ON          GerarchiaImprese.Padre = uvap.Piva");
                stbQuery.AppendLine("   AND         uvap.Entita_Cod = 1 ");
                stbQuery.AppendLine("   AND         uvap.sa_cod = 0  ");
                stbQuery.AppendLine("   AND         uvap.Username = @utenteUsername ");

                stbQuery.AppendLine("   LEFT JOIN   Utenti_Visibilita_Appoggio uvaf (NOLOCK)  ");
                stbQuery.AppendLine("   ON          GerarchiaImprese.Figlio = uvaf.Piva");
                stbQuery.AppendLine("   AND         uvaf.Entita_Cod = 1 ");
                stbQuery.AppendLine("   AND         uvaf.sa_cod = 0  ");
                stbQuery.AppendLine("   AND         uvaf.Username = @utenteUsername ");

                parametriSql.Add("@utenteUsername", objParametriServer.UtenteUsername);
            }
            if (livello.HasValue)
            {
                parametriSql.Add("@livello", livello.Value);
            }

            stbQuery.AppendLine("   WHERE       1 = 1");

            if (applicaVisibilita)
            {
                stbQuery.AppendLine("   AND NOT     (uvap.Piva IS NULL AND uvaf.Piva is NULL) ");
            }

            // DS-15 Modifiche.1 — filtro opzionale per livello gerarchico
            if (livello.HasValue)
            {
                stbQuery.AppendLine("   AND         GerarchiaImprese.Livello = @livello ");
            }

            stbQuery.AppendLine("     ) as a ");

            stbQuery.AppendLine("     LEFT JOIN Imprese_Codici IC ON IC.Piva = a.Piva AND IC.id_cod = " + (int)Enum_CodiciAnagrafe.CodiceCUAA + " ");

            stbQuery.AppendLine(" ORDER BY        a.rag_soc ");

            try
            {
                dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            return dt;
        }

        public async Task<DataTable> LeggiGerarchiaImpreseFiglieAsync(string piva, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            parametriSql.TryAdd("piva", piva);
            parametriSql.TryAdd("dataValiditaInizio", objParametriServer.FinestraTemporaleInizio);
            parametriSql.TryAdd("dataValiditaFine", objParametriServer.FinestraTemporaleFine);

            DataTable dt;

            stbQuery.AppendLine("SELECT *");
            stbQuery.AppendLine("FROM GerarchiaImprese");
            stbQuery.AppendLine("WHERE Padre = @piva");
            stbQuery.AppendLine("AND Validita_inizio < @dataValiditaFine");
            stbQuery.AppendLine("AND Validita_Fine > @dataValiditaInizio");

            try
            {
                dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            return dt;
        }

        public async Task<List<string>> LeggiElencoGerarchiaImpreseFiglieAsync(string piva, AgronicaCoreParametriServer objParametriServer)
        {
            List<string> result = new List<string>();
            result.Add(piva);

            DataTable dt = await LeggiGerarchiaImpreseFiglieAsync(piva,objParametriServer);
            if (dt != null && dt.Rows.Count > 0)
            {
                result.AddRange(dt.AsEnumerable().Select(r => r.Field<string>("Figlio")!).ToArray());

                var padri = dt.AsEnumerable()
                    .Where(r => r.Field<Int16>("Foglia") == 0)
                    .Select(r => r.Field<string>("Figlio")!)
                    .ToArray();

                foreach (string padre in padri)
                    result.AddRange(await LeggiElencoGerarchiaImpreseFiglieAsync(padre, objParametriServer));
            }
            
            return result
                .Distinct()
                .ToList();  
        }

        /// <summary>
        /// Recupera il cono di visibilità di un Utente specifico interrogando la gerarchia aziendale GIAS.
        /// </summary>
        /// <param name="idUtente">Username dell'Utente di cui recuperare la visibilità.</param>
        /// <param name="applicaFiltroVisibilita">
        /// <c>true</c> per filtrare tramite JOIN su <c>Utenti_Visibilita_Appoggio</c>;
        /// <c>false</c> per visibilità totale (nessun filtro applicato).
        /// </param>
        /// <param name="objParametriServer">Parametri di connessione al DB Server operativo.</param>
        /// <remarks>
        /// Design Specification DS01-BL: RecuperoConoVisibilitaUtente - Persistenze Coinvolte.
        /// Query logica: SELECT padre.PIVA as PivaPadre, figlio.PIVA as PivaFiglio, Livello, Foglia
        /// FROM GerarchiaImprese JOIN Imprese figlio ON Figlio=figlio.PIVA JOIN Imprese padre ON Padre=padre.PIVA
        /// [JOIN Utenti_Visibilita_Appoggio uva ON figlio.PIVA=uva.Piva WHERE uva.Entita_Cod=1 AND uva.Username=@idUtente]
        /// ORDER BY Livello, figlio.rag_soc
        /// </remarks>
        public async Task<DataTable> LeggiConoVisibilitaAsync(string idUtente, bool applicaFiltroVisibilita, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            DataTable dt;

            stbQuery.AppendLine("SELECT");
            stbQuery.AppendLine("    padre.PIVA                               AS PivaPadre,");
            stbQuery.AppendLine("    figlio.PIVA                              AS PivaFiglio,");
            stbQuery.AppendLine("    ISNULL(ic_padre.Val_Cod,  padre.PIVA)   AS CuaaPadre,");
            stbQuery.AppendLine("    ISNULL(ic_figlio.Val_Cod, figlio.PIVA)  AS CuaaFiglio,");
            stbQuery.AppendLine("    gi.Livello,");
            stbQuery.AppendLine("    gi.Foglia");
            stbQuery.AppendLine("FROM   GerarchiaImprese gi (NOLOCK)");
            stbQuery.AppendLine("JOIN   Imprese figlio (NOLOCK) ON gi.Figlio = figlio.PIVA");
            stbQuery.AppendLine("JOIN   Imprese padre  (NOLOCK) ON gi.Padre  = padre.PIVA");
            stbQuery.AppendLine($"LEFT JOIN Imprese_Codici ic_figlio (NOLOCK) ON ic_figlio.Piva = figlio.PIVA AND ic_figlio.id_cod = {(int)Enum_CodiciAnagrafe.CodiceCUAA}");
            stbQuery.AppendLine($"LEFT JOIN Imprese_Codici ic_padre  (NOLOCK) ON ic_padre.Piva  = padre.PIVA  AND ic_padre.id_cod  = {(int)Enum_CodiciAnagrafe.CodiceCUAA}");

            if (applicaFiltroVisibilita)
            {
                stbQuery.AppendLine("JOIN   Utenti_Visibilita_Appoggio uva (NOLOCK)");
                stbQuery.AppendLine("       ON  figlio.PIVA = uva.Piva");
                stbQuery.AppendLine("       AND uva.Entita_Cod = 1");
                stbQuery.AppendLine("       AND uva.Username   = @idUtente");

                parametriSql.Add("@idUtente", idUtente);
            }

            stbQuery.AppendLine("ORDER BY gi.Livello, figlio.rag_soc;");

            try
            {
                dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            return dt;
        }

        /// <summary>
        /// Recupera la struttura organizzativa completa di tutte le Filiere e Aziende con sede,
        /// senza filtri di autorizzazione. Usa una UNION ALL per includere anche le Capofiliere
        /// (radici dell'albero) come righe con <c>Livello = 0</c> e padre nullo.
        /// </summary>
        /// <remarks>
        /// Design Specification DS02-BL: RecuperoStrutturaFiliereTotale - Persistenze Coinvolte.
        /// </remarks>
        public async Task<DataTable> LeggiStrutturaFiliereAsync(AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();

            stbQuery.AppendLine("SELECT");
            stbQuery.AppendLine("    padre.PIVA                               AS PivaPadreDirecto,");
            stbQuery.AppendLine("    padre.rag_soc                            AS NomePadreDirecto,");
            stbQuery.AppendLine("    figlio.PIVA                              AS PivaAzienda,");
            stbQuery.AppendLine("    figlio.rag_soc                           AS NomeAzienda,");
            stbQuery.AppendLine("    ISNULL(ic_figlio.Val_Cod, figlio.PIVA)   AS CuaaAzienda,");
            stbQuery.AppendLine("    ISNULL(ic_padre.Val_Cod,  padre.PIVA)    AS CuaaPadreDirecto,");
            stbQuery.AppendLine("    gi.Livello,");
            stbQuery.AppendLine("    gi.Foglia,");
            stbQuery.AppendLine("    stato.Codice     AS Stato_ISO,");
            stbQuery.AppendLine("    lr.Regione_Des   AS Regione,");
            stbQuery.AppendLine("    ist.LOCALITA     AS Citta");
            stbQuery.AppendLine("FROM   GerarchiaImprese gi (NOLOCK)");
            stbQuery.AppendLine("JOIN   Imprese figlio (NOLOCK) ON gi.Figlio = figlio.PIVA");
            stbQuery.AppendLine("JOIN   Imprese padre  (NOLOCK) ON gi.Padre  = padre.PIVA");
            stbQuery.AppendLine($"LEFT JOIN Imprese_Codici ic_figlio (NOLOCK) ON ic_figlio.Piva = figlio.PIVA AND ic_figlio.id_cod = {(int)Enum_CodiciAnagrafe.CodiceCUAA}");
            stbQuery.AppendLine($"LEFT JOIN Imprese_Codici ic_padre  (NOLOCK) ON ic_padre.Piva  = padre.PIVA  AND ic_padre.id_cod  = {(int)Enum_CodiciAnagrafe.CodiceCUAA}");
            stbQuery.AppendLine("LEFT JOIN ImpresexIndirizzi ix (NOLOCK) ON figlio.PIVA = ix.PIVA");
            stbQuery.AppendLine("LEFT JOIN Indirizzi ind (NOLOCK) ON ix.cod_indirizzo = ind.cod_indirizzo");
            stbQuery.AppendLine("LEFT JOIN ISTAT ist (NOLOCK) ON ind.pro_cod_istat = ist.PROV AND ind.com_cod_istat = ist.COM");
            stbQuery.AppendLine("LEFT JOIN Lista_Province lp (NOLOCK) ON ist.PROV = lp.PROV");
            stbQuery.AppendLine("LEFT JOIN Lista_Regioni lr (NOLOCK) ON lp.REG = lr.REG");
            stbQuery.AppendLine("LEFT JOIN ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166 stato (NOLOCK) ON ist.Stato_Country = stato.Codice");
            stbQuery.AppendLine("ORDER BY gi.Livello, padre.rag_soc, figlio.rag_soc;");

            try
            {
                return await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), new Dictionary<string, object>());
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<DataTable> LeggiFilieraAdminAsync(AgronicaCoreParametriUtenti objParametriUtenti)
        {
            var stbQuery = new StringBuilder();
            DataTable dt;

            stbQuery.AppendLine("SELECT");
            stbQuery.AppendLine("    u.UserName,");
            stbQuery.AppendLine("    ud.Cognome,");
            stbQuery.AppendLine("    ud.Nome,");
            stbQuery.AppendLine("    ud.Email,");
            stbQuery.AppendLine("    ud.PIVA AS PivaFiliera");
            stbQuery.AppendLine("FROM   Utenti u (NOLOCK)");
            stbQuery.AppendLine("JOIN   Utenti_Dettagli ud (NOLOCK) ON u.UserName = ud.UserName");
            stbQuery.AppendLine("JOIN   Utenti_Tipologie ut (NOLOCK) ON u.Tipologia_Cod = ut.Tipologia_Cod");
            stbQuery.AppendLine("WHERE  ut.Tipologia_Des LIKE '%filiera admin%';");

            try
            {
                dt = await GetDataProvider(objParametriUtenti).ExecuteReadAsync(stbQuery.ToString(), new Dictionary<string, object>());
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriUtenti, ex);
                throw;
            }

            return dt;
        }

        public async Task<Dictionary<string, FilieraAdminDto>> LeggiAdminFiliereAsync(AgronicaCoreParametriUtenti objParametriUtenti)
        {
            var stbQuery = new StringBuilder();

            stbQuery.AppendLine("SELECT");
            stbQuery.AppendLine("    Utenti.UserName,");
            stbQuery.AppendLine("    Utenti_Dettagli.Cognome,");
            stbQuery.AppendLine("    Utenti_Dettagli.Nome,");
            stbQuery.AppendLine("    Utenti_Dettagli.Email,");
            stbQuery.AppendLine("    Utenti_Profili.Descrizione_2 AS IdFiliera");
            stbQuery.AppendLine("FROM Utenti (NOLOCK)");
            stbQuery.AppendLine("JOIN Utenti_Dettagli (NOLOCK) ON Utenti.UserName = Utenti_Dettagli.UserName");
            stbQuery.AppendLine("JOIN Utenti_Tipologie (NOLOCK) ON Utenti.Tipologia_Cod = Utenti_Tipologie.Tipologia_Cod");
            stbQuery.AppendLine("JOIN Utenti_Profili (NOLOCK) ON Utenti.UserName = Utenti_Profili.Utente AND Utenti_Profili.Id_Servizio = 5");
            stbQuery.AppendLine("WHERE Utenti_Tipologie.Tipologia_Des LIKE '%filiera admin%'");

            var adminDict = new Dictionary<string, FilieraAdminDto>(StringComparer.OrdinalIgnoreCase);

            try
            {
                var dt = await GetDataProvider(objParametriUtenti).ExecuteReadAsync(stbQuery.ToString(), new Dictionary<string, object>());

                foreach (DataRow row in dt.Rows)
                {
                    var idFiliera = row.Field<string>("IdFiliera");

                    if (!string.IsNullOrEmpty(idFiliera))
                    {
                        string key = idFiliera.Trim();

                        if (!adminDict.ContainsKey(key))
                        {
                            adminDict.Add(key, new FilieraAdminDto
                            {
                                IdUtente = row.Field<string>("UserName") ?? string.Empty,
                                NomeUtente = $"{row.Field<string>("Cognome")} {row.Field<string>("Nome")}".Trim(),
                                EmailUtente = row.Field<string>("Email") ?? string.Empty
                            });
                        }
                    }
                }

                return adminDict;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriUtenti, ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<DataTable> LeggiPadriRicorsivaAsync(
            string figlio,
            AgronicaCoreParametriServer objParametriServer)
        {
            if (string.IsNullOrWhiteSpace(figlio))
                throw new ArgumentException("figlio non può essere vuoto.");

            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>
            {
                ["@figlio"]   = figlio,
                ["@dtFine"]   = objParametriServer.FinestraTemporaleFine,
                ["@dtInizio"] = objParametriServer.FinestraTemporaleInizio
            };

            stbQuery.AppendLine("WITH Antenati AS (");
            stbQuery.AppendLine("    SELECT Figlio, Padre, Livello");
            stbQuery.AppendLine("    FROM GerarchiaImprese");
            stbQuery.AppendLine("    WHERE Figlio = @figlio");
            stbQuery.AppendLine("    AND GerarchiaImprese.Validita_inizio <= @dtFine");
            stbQuery.AppendLine("    AND GerarchiaImprese.Validita_Fine   >= @dtInizio");
            stbQuery.AppendLine("    AND GerarchiaImprese.Inviato >= 0");
            stbQuery.AppendLine("    UNION ALL");
            stbQuery.AppendLine("    SELECT g.Figlio, g.Padre, g.Livello");
            stbQuery.AppendLine("    FROM GerarchiaImprese g");
            stbQuery.AppendLine("    JOIN Antenati a ON a.Padre = g.Figlio AND g.Padre != ''");
            stbQuery.AppendLine(")");
            stbQuery.AppendLine("SELECT Padre, Figlio, Livello FROM Antenati;");

            try
            {
                return await GetDataProvider(objParametriServer)
                    .ExecuteReadAsync(stbQuery.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<DataTable> LeggiTuttaGerarchiaAsync(AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();

            stbQuery.AppendLine("SELECT");
            stbQuery.AppendLine("    gi.Padre    AS Padre,");
            stbQuery.AppendLine("    gi.Figlio   AS PIVA,");
            stbQuery.AppendLine("    imp.rag_soc AS rag_soc,");
            stbQuery.AppendLine("    gi.Foglia   AS Foglia");
            stbQuery.AppendLine("FROM  GerarchiaImprese gi (NOLOCK)");
            stbQuery.AppendLine("JOIN  Imprese imp (NOLOCK) ON imp.PIVA = gi.Figlio;");

            try
            {
                return await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), new Dictionary<string, object>());
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<string?> LeggiPadreGerarchiaPerUtenteAsync(
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriUtenti objParametriUtenti)
        {
            // Step 1: recupera il gruppo utente dal DB Utenti
            const string gruppoSql = "SELECT TOP 1 Gruppi_Utente_Cod FROM Utenti_xGruppi_Utente WHERE username = @username";
            var gruppoParams = new Dictionary<string, object> { { "@username", objParametriServer.UtenteUsername } };

            DataTable dtGruppo;
            try
            {
                dtGruppo = await GetDataProvider(objParametriUtenti).ExecuteReadAsync(gruppoSql, gruppoParams);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            if (dtGruppo.Rows.Count == 0) return null;
            int gruppoUtenteCode = Convert.ToInt32(dtGruppo.Rows[0]["Gruppi_Utente_Cod"]);

            // Step 2: recupera la PIVA del nodo padre configurato per il gruppo nel servizio GiasAPP (Id_Servizio=100)
            const int idServizioGiasAPP = 100;
            var stbPadre = new StringBuilder();
            stbPadre.AppendLine("SELECT DISTINCT a.Piva");
            stbPadre.AppendLine("FROM GerarchiaImprese_Tipologia_NodiApplicati a");
            stbPadre.AppendLine("   INNER JOIN GerarchiaImprese_Tipologia b ON a.TipologiaGerarchie_Cod = b.TipologiaGerarchie_Cod");
            stbPadre.AppendLine("   INNER JOIN GerarchiaImprese_Tipologia_Dettagli c ON a.TipologiaGerarchie_Cod = c.TipologiaGerarchie_Cod");
            stbPadre.AppendLine("   INNER JOIN GerarchiaImprese_Tipologia_Dettagli_XGruppi_Utente d ON c.TipologiaGerarchie_Dettagli_Livello_Cod = d.TipologiaGerarchie_Dettagli_Livello_Cod");
            stbPadre.AppendLine("WHERE d.Gruppi_Utente_Cod = @gruppo AND d.Id_Servizio = @idServizio");

            var padreParams = new Dictionary<string, object>
            {
                { "@gruppo", gruppoUtenteCode },
                { "@idServizio", idServizioGiasAPP }
            };

            DataTable dtPadre;
            try
            {
                dtPadre = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbPadre.ToString(), padreParams);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            return dtPadre.Rows.Count > 0 ? dtPadre.Rows[0]["Piva"]?.ToString() : null;
        }

        /// <inheritdoc/>
        public async Task<DataRow?> LeggiImpreseBasePivaAsync(string piva, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("SELECT Imprese.Piva, Imprese.Rag_Soc, Imprese.TipoImpresaGerarchia,");
            stbQuery.AppendLine("       GI.Padre, GI.Livello, GI.Foglia");
            stbQuery.AppendLine("FROM Imprese");
            stbQuery.AppendLine("LEFT JOIN GerarchiaImprese GI ON GI.Figlio = Imprese.Piva");
            stbQuery.AppendLine("WHERE Imprese.Piva = @piva");

            var parSql = new Dictionary<string, object> { { "@piva", piva } };
            DataTable dt;
            try
            {
                dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }
    }
}
