using AgronicaNetCore.Anagrafe.DAL.Resources;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Base.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Text;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.Indirizzi
{
    /// <summary>
    /// Resolves geographic information (Provincia, Regione, Nazione) for a batch
    /// of appezzamenti by traversing the address registry tables.
    /// </summary>
    /// <remarks>
    /// <c>Appezzamento → AppezzamentixIndirizzi → Indirizzi
    ///   → Lista_Province (pro_cod_istat+com_cod_istat)
    ///   → Lista_Regioni  (REG)
    ///   → ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166 (Stato_Country)</c>.
    /// </remarks>
    public class Indirizzi : BaseDALAnagrafe, IIndirizzi
    {
        private readonly TempChiaviMassivo _tempChiaviMassivo;

        /// <inheritdoc/>
        public Indirizzi(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer,
            bool securityBypass = false
        )
            : base(provider, localizer, securityBypass)
        {
            _tempChiaviMassivo = _serviceProvider.GetRequiredService<TempChiaviMassivo>();
        }

        /// <inheritdoc/>
        public async Task<DataTable> LeggiIndirizzoxAppezzamentoAsync(
            IEnumerable<AppezzamentoGeoKey> chiavi,
            AgronicaCoreParametriServer objParametriServer
        )
        {
            if (chiavi is null)
                throw new ArgumentNullException(nameof(chiavi));
            var chiaviList = chiavi.Distinct().ToList();
            if (!chiaviList.Any())
                return new DataTable();

            var parametriSql = new Dictionary<string, object>();
            var strSql = new StringBuilder();

            // Step 1   — load keys into a temp table for a single batch join.
            strSql.AppendLine("IF OBJECT_ID('tempdb..#AppGeoChiavi') IS NOT NULL DROP TABLE #AppGeoChiavi;");
            strSql.AppendLine("CREATE TABLE #AppGeoChiavi (PIVA nvarchar(25) COLLATE DATABASE_DEFAULT, SA_COD int, APPEZZA int);");

            foreach (var (idx, key) in chiaviList.Select((k, i) => (i, k)))
            {
                strSql.AppendLine($"INSERT INTO #AppGeoChiavi VALUES (@p{idx}, @sc{idx}, @ap{idx});");
                parametriSql[$"@p{idx}"] = key.Piva;
                parametriSql[$"@sc{idx}"] = key.SaCod;
                parametriSql[$"@ap{idx}"] = key.Appezza;
            }

            strSql.AppendLine("SELECT");
            strSql.AppendLine("    CHI.PIVA,");
            strSql.AppendLine("    CHI.SA_COD,");
            strSql.AppendLine("    CHI.APPEZZA,");
            // LP.PROVINCIA contiene il nome della provincia (es. 'Ferrara').
            // IND.pro_cod è vuoto in molti casi; IND.pro_cod_istat è il codice ISTAT corretto.
            strSql.AppendLine("    ISNULL(LP.PROVINCIA, '')     AS Provincia,");
            // Step 2   — Provincia → Regione
            strSql.AppendLine("    LR.REG                       AS REG,");
            strSql.AppendLine("    ISNULL(LR.Regione_Des, '')   AS Regione,");
            // Step 3   — Regione → Nazione
            strSql.AppendLine("    ISNULL(LS.Codice, '')        AS Nazione");
            strSql.AppendLine("FROM #AppGeoChiavi CHI");
            strSql.AppendLine("LEFT JOIN Appezzamento           APP  ON APP.PIVA    = CHI.PIVA   AND APP.SA_COD  = CHI.SA_COD  AND APP.APPEZZA = CHI.APPEZZA");
            strSql.AppendLine("LEFT JOIN AppezzamentixIndirizzi AXI  ON AXI.PIVA    = APP.PIVA   AND AXI.sa_cod = APP.SA_COD  AND AXI.appezza = APP.APPEZZA");
            strSql.AppendLine("LEFT JOIN Indirizzi              IND  ON IND.cod_indirizzo = AXI.cod_indirizzo");
            // Step 3.1 — Chiave: solo su Prov = pro_cod_istat.
            // Lista_Province.Com è un codice interno di capoluogo, diverso da com_cod_istat in Indirizzi,
            // quindi il join su Com non è affidabile. Si usa una subquery con GROUP BY per garantire
            // una sola riga per provincia (REG e PROVINCIA sono identici per tutti i comuni della stessa provincia).
            strSql.AppendLine("LEFT JOIN (SELECT Prov, MAX(REG) AS REG, MAX(PROVINCIA) AS PROVINCIA FROM Lista_Province GROUP BY Prov) LP");
            strSql.AppendLine("                                        ON LP.Prov = IND.pro_cod_istat");
            // Step 4   — Lista_Regioni.REG = Lista_Province.REG
            strSql.AppendLine("LEFT JOIN Lista_Regioni          LR   ON LR.REG  = LP.REG");
            // Step 5   — Lista_Stati.Codice = Lista_Regioni.Stato_Country
            strSql.AppendLine("LEFT JOIN ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166 LS ON LS.Codice = LR.Stato_Country;");
            strSql.AppendLine("DROP TABLE #AppGeoChiavi;");

            try
            {
                return await GetDataProvider(objParametriServer)
                    .ExecuteReadAsync(strSql.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<DataTable> LeggiIndirizziImpresaAsync(
            List<string> chiaviImprese,
            List<int> tipoIndirizzo,
            bool indirizzoCompleto,
            AgronicaCoreParametriServer objParametriServer
        )
        {
            bool usaTempTable = chiaviImprese.Count > 1;
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            var parSqlIn = new Dictionary<string, Dictionary<Type, List<object>>>();

            if (usaTempTable)
                _tempChiaviMassivo.CreaTabellaTemp_FiltroPiva(chiaviImprese, objParametriServer);

            stbQuery.AppendLine(
                " SELECT ImpresexIndirizzi.PIVA, ImpresexIndirizzi.cod_indirizzo, ImpresexIndirizzi.Tipo_Indirizzo"
            );

            StringBuilder? stbIndirizzoJoins = null;
            if (indirizzoCompleto)
            {
                var (selColumns, joins) = AppendJoinIndirizzoCompleto(
                    "ImpresexIndirizzi.cod_indirizzo"
                );
                stbQuery.Append(selColumns);
                stbIndirizzoJoins = joins;
            }

            stbQuery.AppendLine(" FROM ImpresexIndirizzi");

            if (usaTempTable)
                stbQuery.AppendLine(" JOIN #TempPiva temp ON temp.Piva = ImpresexIndirizzi.Piva");

            if (stbIndirizzoJoins != null)
                stbQuery.Append(stbIndirizzoJoins);

            stbQuery.AppendLine(" WHERE ImpresexIndirizzi.Validita_Inizio <= @dtFine");
            stbQuery.AppendLine(" AND ImpresexIndirizzi.Validita_Fine >= @dtInizio");

            parametriSql.Add("@dtFine", objParametriServer.FinestraTemporaleFine);
            parametriSql.Add("@dtInizio", objParametriServer.FinestraTemporaleInizio);

            if (
                !usaTempTable
                && chiaviImprese.Count == 1
                && !string.IsNullOrWhiteSpace(chiaviImprese[0])
            )
            {
                stbQuery.AppendLine(" AND ImpresexIndirizzi.PIVA = @piva");
                parametriSql.Add("@piva", chiaviImprese[0].Trim());
            }

            if (tipoIndirizzo.Count > 0)
            {
                stbQuery.AppendLine(" AND ImpresexIndirizzi.Tipo_Indirizzo IN (@tipoIndirizzo)");
                parSqlIn.Add("@tipoIndirizzo", FormatClauseIn(tipoIndirizzo));
            }

            try
            {
                return await GetDataProvider(objParametriServer)
                    .ExecuteReadAsync(stbQuery.ToString(), parametriSql, parSqlIn);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
            finally
            {
                if (usaTempTable)
                    _tempChiaviMassivo.EliminaTabellaTemp_FiltroPiva(objParametriServer);
            }
        }

        /// <summary>
        /// Builds the additional SELECT columns and LEFT OUTER JOINs needed when reading
        /// a complete address record (Indirizzi + ISTAT + Lista_Province + Lista_Regioni + Lista_Stati).
        /// </summary>
        /// <param name="codIndirizzoRef">
        /// Fully-qualified column reference for the driving table's cod_indirizzo
        /// (e.g. <c>"ImpresexIndirizzi.cod_indirizzo"</c>).
        /// </param>
        /// <returns>
        /// A tuple with the extra SELECT fragment and the JOIN fragment,
        /// ready to be appended to the query builder at the appropriate positions.
        /// </returns>
        private static (
            StringBuilder SelectColumns,
            StringBuilder JoinClauses
        ) AppendJoinIndirizzoCompleto(string codIndirizzoRef)
        {
            var selectColumns = new StringBuilder();
            var joinClauses = new StringBuilder();

            selectColumns.AppendLine(
                "      , Indirizzi.ind_des, Indirizzi.frz_des, Indirizzi.CAP, Indirizzi.stato, Indirizzi.note, Indirizzi.pro_cod_istat, Indirizzi.com_cod_istat"
            );
            selectColumns.AppendLine(
                "      , Indirizzi.inviato, Indirizzi.datainvio, Indirizzi.Data_Creazione, Indirizzi.Data_Modifica, Indirizzi.Username_Creazione, Indirizzi.Username_Modifica, Indirizzi.Validita_Inizio, Indirizzi.Validita_Fine"
            );
            selectColumns.AppendLine(
                "      , Indirizzi.Validazione, Indirizzi.Data_Validazione, Indirizzi.UserName_Validazione, Indirizzi.Codice_Lingua, Indirizzi.Codice_Alternativo"
            );
            selectColumns.AppendLine("      , ISNULL(ISTAT.LOCALITA, '') AS com_des");
            selectColumns.AppendLine("      , ISNULL(ISTAT.COMUNI_PROV, '') AS pro_cod");
            selectColumns.AppendLine("      , ISNULL(Lista_Province.PROVINCIA, '') AS pro_des");
            selectColumns.AppendLine("      , ISNULL(Lista_Province.REG, '') AS reg");
            selectColumns.AppendLine("      , ISNULL(Lista_Regioni.Regione_Des, '') AS Regione");
            selectColumns.AppendLine("      , ISNULL(Lista_Stati.Descrizione, '') AS Stato");
            selectColumns.AppendLine("      , ISNULL(Lista_Stati.Gestione_Gerarchia_Geografica, 0) AS Gestione_Gerarchia_Geografica");

            joinClauses.AppendLine(
                $" LEFT OUTER JOIN Indirizzi ON Indirizzi.Cod_Indirizzo = {codIndirizzoRef}"
            );
            joinClauses.AppendLine(
                " LEFT OUTER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM"
            );
            joinClauses.AppendLine(
                " LEFT OUTER JOIN Lista_Province ON Lista_Province.Sigla = ISTAT.COMUNI_PROV"
            );
            joinClauses.AppendLine(
                " LEFT OUTER JOIN Lista_Regioni ON Lista_Regioni.REG = Lista_Province.REG"
            );
            joinClauses.AppendLine(
                " LEFT OUTER JOIN ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166 Lista_Stati ON Lista_Stati.Codice = Lista_Regioni.Stato_Country"
            );

            return (selectColumns, joinClauses);
        }

        public async Task<DataTable> LeggiIndirizziCentroAsync(
            List<(string, int)> chiaviCentro,
            List<int> tipoIndirizzo,
                        bool indirizzoCompleto,
AgronicaCoreParametriServer objParametriServer
        )
        {
            bool usaTempTable = chiaviCentro.Count > 1;

            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            var parSqlIn = new Dictionary<string, Dictionary<Type, List<object>>>();

            if (usaTempTable)
                await _tempChiaviMassivo.CreaTabellaTemp_FiltroCentro(chiaviCentro, objParametriServer);

            stbQuery.AppendLine(
                " SELECT CentrixIndirizzi.PIVA, CentrixIndirizzi.sa_cod, CentrixIndirizzi.Tipo_Indirizzo, CentrixIndirizzi.COD_INDIRIZZO"
            );

            StringBuilder? stbCentroIndirizzoJoins = null;
            if (indirizzoCompleto)
            {
                var (selColumns, joins) = AppendJoinIndirizzoCompleto(
                    "CentrixIndirizzi.cod_indirizzo"
                );
                stbQuery.Append(selColumns);
                stbCentroIndirizzoJoins = joins;
            }

            stbQuery.AppendLine(" FROM CentrixIndirizzi");
            if (usaTempTable)
                stbQuery.AppendLine(
                    " JOIN #TempCentro temp ON temp.Piva = CentrixIndirizzi.Piva AND temp.sa_cod = CentrixIndirizzi.sa_cod"
                );

            if (stbCentroIndirizzoJoins != null)
                stbQuery.Append(stbCentroIndirizzoJoins);

            stbQuery.AppendLine(" WHERE CentrixIndirizzi.Validita_inizio < @dtFine");
            stbQuery.AppendLine(" AND CentrixIndirizzi.Validita_Fine > @dtInizio");

            parametriSql.Add("@dtFine", objParametriServer.FinestraTemporaleFine);
            parametriSql.Add("@dtInizio", objParametriServer.FinestraTemporaleInizio);

            if (!usaTempTable && chiaviCentro.Count == 1)
            {
                if (!string.IsNullOrWhiteSpace(chiaviCentro[0].Item1))
                {
                    stbQuery.AppendLine(" AND CentrixIndirizzi.PIVA = @piva");
                    parametriSql.Add("@piva", chiaviCentro[0].Item1.Trim());
                }

                if (chiaviCentro[0].Item2 != 0)
                {
                    stbQuery.AppendLine(" AND CentrixIndirizzi.sa_cod = @sa_cod");
                    parametriSql.Add("@sa_cod", chiaviCentro[0].Item2);
                }
            }

            if (tipoIndirizzo != null && tipoIndirizzo.Count > 0)
            {
                stbQuery.AppendLine(" AND CentrixIndirizzi.Tipo_Indirizzo IN (@tipoIndirizzo)");
                parSqlIn.Add("@tipoIndirizzo", FormatClauseIn(tipoIndirizzo));
            }

            try
            {
                return await GetDataProvider(objParametriServer)
                    .ExecuteReadAsync(stbQuery.ToString(), parametriSql, parSqlIn);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
            finally
            {
                if (usaTempTable)
                    await _tempChiaviMassivo.EliminaTabellaTemp_FiltroCentro(objParametriServer);
            }
        }

        public async Task<DataTable> LeggiIndirizziContattiAsync(
            List<(string, int)> chiaviContatto,
            List<int> tipoIndirizzo,
                        bool indirizzoCompleto,
AgronicaCoreParametriServer objParametriServer
        )
        {
            bool usaTempTable = chiaviContatto.Count > 1;
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            Dictionary<string, Dictionary<Type, List<object>>> parSqlIn = new();

            if (usaTempTable)
                _tempChiaviMassivo.CreaTabellaTemp_FiltroContatto(
                    chiaviContatto,
                    objParametriServer
                );

            stbQuery.AppendLine(
                " SELECT ContattiXIndirizzi.piva, ContattiXIndirizzi.cod_contatto, ContattiXIndirizzi.Tipo_Indirizzo, ContattiXIndirizzi.COD_INDIRIZZO"
            );

            StringBuilder? stbContattiIndirizzoJoins = null;
            if (indirizzoCompleto)
            {
                var (selColumns, joins) = AppendJoinIndirizzoCompleto(
                    "ContattiXIndirizzi.cod_indirizzo"
                );
                stbQuery.Append(selColumns);
                stbContattiIndirizzoJoins = joins;
            }

            stbQuery.AppendLine(" FROM Contatti_Codici");
            if (usaTempTable)
                stbQuery.AppendLine(
                    " JOIN #TempContatto temp ON temp.Piva = Contatti_Codici.Piva AND temp.Cod_Contatto = Contatti_Codici.Cod_Contatto"
                );

            if (indirizzoCompleto)
                stbQuery.AppendLine(
                    " LEFT OUTER JOIN ContattiXIndirizzi ON ContattiXIndirizzi.PIVA = Contatti_Codici.PIVA AND ContattiXIndirizzi.Cod_Contatto = Contatti_Codici.cod_contatto"
                );

            if (stbContattiIndirizzoJoins != null)
                stbQuery.Append(stbContattiIndirizzoJoins);

            stbQuery.AppendLine(" WHERE ContattiXIndirizzi.Validita_Inizio <= @dtFine");
            stbQuery.AppendLine(" AND ContattiXIndirizzi.Validita_Fine >= @dtInizio");

            parametriSql.Add("@dtFine", objParametriServer.FinestraTemporaleFine);
            parametriSql.Add("@dtInizio", objParametriServer.FinestraTemporaleInizio);

            if (!usaTempTable && chiaviContatto.Count == 1)
            {
                if (!string.IsNullOrWhiteSpace(chiaviContatto[0].Item1))
                {
                    stbQuery.AppendLine(" AND ContattiXIndirizzi.PIVA = @piva");
                    parametriSql.Add("@piva", chiaviContatto[0].Item1.Trim());
                }

                if (chiaviContatto[0].Item2 != 0)
                {
                    stbQuery.AppendLine(" AND ContattiXIndirizzi.Cod_Contatto = @codContatto");
                    parametriSql.Add("@codContatto", chiaviContatto[0].Item2);
                }
            }

            if (tipoIndirizzo != null && tipoIndirizzo.Count > 0)
            {
                stbQuery.AppendLine(" AND ContattiXIndirizzi.Tipo_Indirizzo IN (@tipoIndirizzo)");
                parSqlIn.Add("@tipoIndirizzo", FormatClauseIn(tipoIndirizzo));
            }

            try
            {
                return await GetDataProvider(objParametriServer)
                    .ExecuteReadAsync(stbQuery.ToString(), parametriSql, parSqlIn);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<DataTable> LeggiIndirizziAppezzamentiAsync(
            List<(string, int, int)> chiaviAppezzamento,
            List<int> tipoIndirizzo,
                        bool indirizzoCompleto,
AgronicaCoreParametriServer objParametriServer
        )
        {
            bool usaTempTable = chiaviAppezzamento.Count > 1;

            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            Dictionary<string, Dictionary<Type, List<object>>> parSqlIn = new();

            if (usaTempTable)
                await _tempChiaviMassivo.CreaTabellaTemp_FiltroAppezzamenti(
                    chiaviAppezzamento,
                    objParametriServer
                );

            stbQuery.AppendLine(
                " SELECT AppezzamentixIndirizzi.piva, AppezzamentixIndirizzi.sa_cod, AppezzamentixIndirizzi.appezza, AppezzamentixIndirizzi.tipo_indirizzo, AppezzamentixIndirizzi.cod_indirizzo"
            );

            StringBuilder? stbAppezzIndirizzoJoins = null;
            if (indirizzoCompleto)
            {
                var (selColumns, joins) = AppendJoinIndirizzoCompleto(
                    "AppezzamentixIndirizzi.cod_indirizzo"
                );
                stbQuery.Append(selColumns);
                stbAppezzIndirizzoJoins = joins;
            }

            stbQuery.AppendLine(" FROM AppezzamentixIndirizzi");
            if (usaTempTable)
                stbQuery.AppendLine(
                    " JOIN #TempAppezzamento temp ON temp.Piva = AppezzamentixIndirizzi.Piva AND temp.sa_cod = AppezzamentixIndirizzi.sa_cod AND temp.appezza = AppezzamentixIndirizzi.appezza"
                );

            if (stbAppezzIndirizzoJoins != null)
                stbQuery.Append(stbAppezzIndirizzoJoins);

            stbQuery.AppendLine(" WHERE AppezzamentixIndirizzi.Validita_inizio < @dtFine");
            stbQuery.AppendLine(" AND AppezzamentixIndirizzi.Validita_Fine > @dtInizio");

            parametriSql.Add("@dtFine", objParametriServer.FinestraTemporaleFine);
            parametriSql.Add("@dtInizio", objParametriServer.FinestraTemporaleInizio);

            if (!usaTempTable && chiaviAppezzamento.Count == 1)
            {
                if (!string.IsNullOrWhiteSpace(chiaviAppezzamento[0].Item1))
                {
                    stbQuery.AppendLine(" AND PIVA = @piva");
                    parametriSql.Add("@piva", chiaviAppezzamento[0].Item1.Trim());
                }

                if (chiaviAppezzamento[0].Item2 != 0)
                {
                    stbQuery.AppendLine(" AND sa_cod = @sa_cod");
                    parametriSql.Add("@sa_cod", chiaviAppezzamento[0].Item2);
                }

                if (chiaviAppezzamento[0].Item3 != 0)
                {
                    stbQuery.AppendLine(" AND appezza = @appezza");
                    parametriSql.Add("@appezza", chiaviAppezzamento[0].Item3);
                }
            }

            if (tipoIndirizzo.Count > 0)
            {
                stbQuery.AppendLine(
                    " AND AppezzamentixIndirizzi.tipo_indirizzo IN (@tipoIndirizzo)"
                );
                parSqlIn.Add("@tipoIndirizzo", FormatClauseIn(tipoIndirizzo));
            }

            try
            {
                return await GetDataProvider(objParametriServer)
                    .ExecuteReadAsync(stbQuery.ToString(), parametriSql, parSqlIn);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
            finally
            {
                if (usaTempTable)
                    await _tempChiaviMassivo.EliminaTabellaTemp_FiltroAppezzamenti(objParametriServer);
            }
        }
    }
}
