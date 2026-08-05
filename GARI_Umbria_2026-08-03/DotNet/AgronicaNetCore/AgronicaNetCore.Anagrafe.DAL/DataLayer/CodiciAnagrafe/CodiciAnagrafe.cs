using System.Data;
using System.Text;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaNetCore.Anagrafe.DAL.Resources;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Base.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.CodiciAnagrafe
{
    public class CodiciAnagrafe : BaseDALAnagrafe, ICodiciAnagrafe
    {
        private readonly TempChiaviMassivo _tempChiaviMassivo;

        private readonly string _impreseCodici = "Imprese_Codici";
        private readonly string _centriAziendaliCodici = "Centri_Aziendali_Codici";
        private readonly string _tabellaCampiCodici = "Campi_Codici";
        private readonly string _tabellaAppezzamentiCodici = "Appezzamento_Codici";
        private readonly string _tabellaRegImpiantiCodici = "Reg_Impianti_Codici";
        private readonly string _tabellaFabbricatiCodici = "Fabbricati_Codici";

        public CodiciAnagrafe(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider, localizer)
        {
            _tempChiaviMassivo = _serviceProvider.GetRequiredService<TempChiaviMassivo>();
        }

        public async Task<DataTable> LeggiCodiciUsatixEntitaDestinazioniDUsoAsync(
            AgronicaCoreParametriServer objParametriServer
        )
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            DataTable dt;

            stbQuery.AppendLine(" SELECT   Codice, Descrizione");
            stbQuery.AppendLine(" FROM     Codici_Anagrafe ");
            stbQuery.AppendLine(" WHERE    Validita_Inizio <= @dtFine");
            stbQuery.AppendLine(" AND      Validita_Fine >= @dtInizio");
            stbQuery.AppendLine(" AND      Codice >= 3000");
            stbQuery.AppendLine(" AND      Codice < 4000");

            parametriSql.Add("@dtFine", objParametriServer.FinestraTemporaleFine);
            parametriSql.Add("@dtInizio", objParametriServer.FinestraTemporaleInizio);

            try
            {
                dt = await GetDataProvider(objParametriServer)
                    .ExecuteReadAsync(stbQuery.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            return dt;
        }

        public async Task<List<CodiceAnagrafeBase>> LeggiCodiciUsatixEntitaUsatixEntitaAsync(
            int entitaLetturaCodici,
            int idBudget,
            AgronicaCoreParametriServer objParametriServer
        )
        {
            //Aziende, Centri e Fabbricati NON esistono in modalità budget

            if (entitaLetturaCodici == (int)Enum_Entita_FiltroRicerca.Azienda)
            {
                return await LeggiCodiciUsatixEntitaAsync(_impreseCodici, 0, objParametriServer);
            }
            else if (entitaLetturaCodici == (int)Enum_Entita_FiltroRicerca.CentroAziendale)
            {
                return await LeggiCodiciUsatixEntitaAsync(
                    _centriAziendaliCodici,
                    0,
                    objParametriServer
                );
            }
            else if (entitaLetturaCodici == (int)Enum_Entita_FiltroRicerca.Campo)
            {
                return await LeggiCodiciUsatixEntitaAsync(
                    _tabellaCampiCodici,
                    idBudget,
                    objParametriServer
                );
            }
            else if (entitaLetturaCodici == (int)Enum_Entita_FiltroRicerca.Appezzamento)
            {
                return await LeggiCodiciUsatixEntitaAsync(
                    _tabellaAppezzamentiCodici,
                    idBudget,
                    objParametriServer,
                    EscludiCodici: new List<int> { (int)Enum_CodiciAnagrafe.MetodoDiProduzione }
                );
            }
            else if (entitaLetturaCodici == (int)Enum_Entita_FiltroRicerca.Impianto)
            {
                return await LeggiCodiciUsatixEntitaAsync(
                    _tabellaRegImpiantiCodici,
                    idBudget,
                    objParametriServer,
                    isImpianto: true
                );
            }
            else if (entitaLetturaCodici == (int)Enum_Entita_FiltroRicerca.Esercizio)
            {
                return await LeggiCodiciUsatixEntitaAsync(
                    _tabellaRegImpiantiCodici,
                    idBudget,
                    objParametriServer,
                    isEsercizio: true
                );
            }
            else if (entitaLetturaCodici == (int)Enum_Entita_FiltroRicerca.Fabbricato)
            {
                return await LeggiCodiciUsatixEntitaAsync(
                    _tabellaFabbricatiCodici,
                    0,
                    objParametriServer
                );
            }
            else
                throw new NotImplementedException();
        }

        /// <summary>
        /// Effettua la lettura dei codici (NON RENDERE PUBBLICO, sarebbe suscettibile al sql injection)
        /// </summary>
        /// <param name="objParametriServer"></param>
        /// <param name="tabellaCodice"></param>
        /// <param name="EscludiCodici"></param>
        /// <param name="isImpianto"></param>
        /// <param name="isEsercizio"></param>
        /// <returns></returns>
        private async Task<List<CodiceAnagrafeBase>> LeggiCodiciUsatixEntitaAsync(
            string tabellaCodice,
            int idBudget,
            AgronicaCoreParametriServer objParametriServer,
            bool isImpianto = false,
            bool isEsercizio = false,
            List<int> EscludiCodici = null
        )
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            Dictionary<string, Dictionary<Type, List<object>>> parSqlIn = new();

            DataTable dt;

            if (idBudget > 0)
                tabellaCodice = "Budget_" + tabellaCodice;

            stbQuery.AppendLine(" SELECT DISTINCT Codice, Descrizione, TipoControllo_Cod");
            stbQuery.AppendLine(" FROM Codici_Anagrafe WITH (NOLOCK) ");
            stbQuery.AppendLine($" JOIN {tabellaCodice} tabellaCodice WITH (NOLOCK)");
            stbQuery.AppendLine(" ON tabellaCodice.id_cod = Codici_Anagrafe.codice");

            if (idBudget > 0)
            {
                stbQuery.AppendLine(" AND tabellaCodice.Id_Budget = @idBudget");
                parametriSql.Add("@idBudget", idBudget);
            }

            stbQuery.AppendLine(" WHERE 1 = 1 ");

            if (CodiciSkip.Length > 0)
            {
                stbQuery.AppendLine(" AND Codice NOT IN (@codiciSkip) ");
                parSqlIn.Add("@codiciSkip", FormatClauseIn(CodiciSkip.ToList()));
            }

            if (EscludiCodici != null && EscludiCodici.Count > 0)
            {
                stbQuery.AppendLine(" AND Codice NOT IN (@codiciEsclusi) ");
                parSqlIn.Add("@codiciEsclusi", FormatClauseIn(EscludiCodici));
            }

            //Escludo destinazioni d'uso (codice <3000 or codice >= 4000) e i codici cliente (codice <2000)
            stbQuery.AppendLine(" AND (Codice < 2000 OR Codice >= 4000) ");
            stbQuery.AppendLine(" AND (Codice <= 10000) ");

            if (isImpianto)
                stbQuery.AppendLine(" AND Progetto_Cod = 0");
            else if (isEsercizio)
                stbQuery.AppendLine(" AND Progetto_Cod <> 0");

            try
            {
                dt = await GetDataProvider(objParametriServer)
                    .ExecuteReadAsync(stbQuery.ToString(), parametriSql, parSqlIn);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            var codiciAnagrafeBase = new List<CodiceAnagrafeBase>();

            foreach (DataRow row in dt.Rows)
            {
                var codiceAnagrafe = new CodiceAnagrafeBase(
                    (int)row["Codice"],
                    (string)row["Descrizione"]
                )
                {
                    TipoControllo_Cod = (int)row["TipoControllo_Cod"],
                };
                codiciAnagrafeBase.Add(codiceAnagrafe);
            }

            return codiciAnagrafeBase;
        }

        public async Task<DataTable> LeggiCodiciImpresaAsync(
            List<string> chiaviImprese,
            List<int> idCod,
            AgronicaCoreParametriServer objParametriServer,
            List<int>? codiciDaEscludere = null,
            bool escludiCodiciCliente = false
        )
        {
            bool usaTempTable = chiaviImprese.Count > 1;
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            var parSqlIn = new Dictionary<string, Dictionary<Type, List<object>>>();

            if (usaTempTable)
                await _tempChiaviMassivo.CreaTabellaTemp_FiltroPiva(chiaviImprese, objParametriServer);

            stbQuery.AppendLine(" SELECT Imprese_Codici.Piva, id_cod, val_cod, Codici_Anagrafe.descrizione");
            stbQuery.AppendLine("      , Imprese_Codici.Validita_Inizio, Imprese_Codici.Validita_Fine");
            stbQuery.AppendLine(" FROM Imprese_Codici");
            if (usaTempTable)
                stbQuery.AppendLine(" JOIN #TempPiva temp ON temp.Piva = Imprese_Codici.Piva");
            stbQuery.AppendLine(" INNER JOIN Codici_Anagrafe ON Codici_Anagrafe.Codice = Imprese_Codici.id_cod");
            stbQuery.AppendLine(" WHERE Imprese_Codici.Validita_Inizio <= @dtFine");
            stbQuery.AppendLine(" AND Imprese_Codici.Validita_Fine >= @dtInizio");

            parametriSql.Add("@dtFine", objParametriServer.FinestraTemporaleFine);
            parametriSql.Add("@dtInizio", objParametriServer.FinestraTemporaleInizio);

            if (
                !usaTempTable
                && chiaviImprese.Count == 1
                && !string.IsNullOrWhiteSpace(chiaviImprese[0])
            )
            {
                stbQuery.AppendLine(" AND Imprese_Codici.PIVA = @piva");
                parametriSql.Add("@piva", chiaviImprese[0].Trim());
            }

            if (idCod != null && idCod.Count > 0)
            {
                stbQuery.AppendLine(" AND Imprese_Codici.id_cod IN (@idCod)");
                parSqlIn.Add("@idCod", FormatClauseIn(idCod));
            }

            AppendFiltriCodici(stbQuery, codiciDaEscludere, escludiCodiciCliente, parSqlIn);

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
                   await _tempChiaviMassivo.EliminaTabellaTemp_FiltroPiva(objParametriServer);
            }
        }

        public async Task<DataTable> LeggiCodiciCentroAsync(
            List<(string, int)> chiaviCentro,
            List<int> idCod,
            AgronicaCoreParametriServer objParametriServer,
            List<int>? codiciDaEscludere = null,
            bool escludiCodiciCliente = false
        )
        {
            bool usaTempTable = chiaviCentro.Count > 1;

            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            var parSqlIn = new Dictionary<string, Dictionary<Type, List<object>>>();

            if (usaTempTable)
                await _tempChiaviMassivo.CreaTabellaTemp_FiltroCentro(chiaviCentro, objParametriServer);

            stbQuery.AppendLine(" SELECT Centri_Aziendali_Codici.PIVA, Centri_Aziendali_Codici.sa_cod, id_cod, val_cod, Codici_Anagrafe.descrizione");
            stbQuery.AppendLine("      , Centri_Aziendali_Codici.Validita_Inizio, Centri_Aziendali_Codici.Validita_Fine");
            stbQuery.AppendLine(" FROM Centri_Aziendali_Codici");
            if (usaTempTable)
                stbQuery.AppendLine(
                    " JOIN #TempCentro temp ON temp.Piva = Centri_Aziendali_Codici.Piva AND temp.sa_cod = Centri_Aziendali_Codici.sa_cod"
                );
            stbQuery.AppendLine(" INNER JOIN Codici_Anagrafe ON Codici_Anagrafe.Codice = Centri_Aziendali_Codici.id_cod");
            stbQuery.AppendLine(" WHERE Centri_Aziendali_Codici.Validita_inizio < @dtFine");
            stbQuery.AppendLine(" AND Centri_Aziendali_Codici.Validita_Fine > @dtInizio");

            parametriSql.Add("@dtFine", objParametriServer.FinestraTemporaleFine);
            parametriSql.Add("@dtInizio", objParametriServer.FinestraTemporaleInizio);

            if (!usaTempTable && chiaviCentro.Count == 1)
            {
                if (!string.IsNullOrWhiteSpace(chiaviCentro[0].Item1))
                {
                    stbQuery.AppendLine(" AND Centri_Aziendali_Codici.PIVA = @piva");
                    parametriSql.Add("@piva", chiaviCentro[0].Item1.Trim());
                }

                if (chiaviCentro[0].Item2 != 0)
                {
                    stbQuery.AppendLine(" AND Centri_Aziendali_Codici.sa_cod = @sa_cod");
                    parametriSql.Add("@sa_cod", chiaviCentro[0].Item2);
                }
            }

            if (idCod != null && idCod.Count > 0)
            {
                stbQuery.AppendLine(" AND Centri_Aziendali_Codici.id_cod IN (@idCod)");
                parSqlIn.Add("@idCod", FormatClauseIn(idCod));
            }

            AppendFiltriCodici(stbQuery, codiciDaEscludere, escludiCodiciCliente, parSqlIn);

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

        public async Task<DataTable> LeggiCodiciContattiAsync(
            List<(string, int)> chiaviContatto,
            List<int> idCod,
            AgronicaCoreParametriServer objParametriServer,
            List<int>? codiciDaEscludere = null,
            bool escludiCodiciCliente = false
        )
        {
            bool usaTempTable = chiaviContatto.Count > 1;
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            Dictionary<string, Dictionary<Type, List<object>>> parSqlIn = new();

            if (usaTempTable)
               await _tempChiaviMassivo.CreaTabellaTemp_FiltroContatto(
                    chiaviContatto,
                    objParametriServer
                );

            stbQuery.AppendLine(" SELECT  Contatti_Codici.piva, Contatti_Codici.cod_contatto, id_cod, val_cod, Codici_Anagrafe.descrizione");
            stbQuery.AppendLine("       , Contatti_Codici.Validita_Inizio, Contatti_Codici.Validita_Fine");
            stbQuery.AppendLine(" FROM Contatti_Codici");
            if (usaTempTable)
                stbQuery.AppendLine(
                    " JOIN #TempContatto temp ON temp.Piva = Contatti_Codici.Piva AND temp.Cod_Contatto = Contatti_Codici.Cod_Contatto"
                );
            stbQuery.AppendLine(" INNER JOIN Codici_Anagrafe ON Codici_Anagrafe.Codice = Contatti_Codici.id_cod");
            stbQuery.AppendLine(" WHERE Contatti_Codici.Validita_Inizio <= @dtFine");
            stbQuery.AppendLine(" AND Contatti_Codici.Validita_Fine >= @dtInizio");

            parametriSql.Add("@dtFine", objParametriServer.FinestraTemporaleFine);
            parametriSql.Add("@dtInizio", objParametriServer.FinestraTemporaleInizio);

            if (!usaTempTable && chiaviContatto.Count == 1)
            {
                if (!string.IsNullOrWhiteSpace(chiaviContatto[0].Item1))
                {
                    stbQuery.AppendLine(" AND Contatti_Codici.PIVA = @piva");
                    parametriSql.Add("@piva", chiaviContatto[0].Item1.Trim());
                }

                if (chiaviContatto[0].Item2 != 0)
                {
                    stbQuery.AppendLine(" AND Contatti_Codici.Cod_Contatto = @codContatto");
                    parametriSql.Add("@codContatto", chiaviContatto[0].Item2);
                }
            }

            if (idCod != null && idCod.Count > 0)
            {
                stbQuery.AppendLine(" AND Contatti_Codici.id_cod IN (@idCod)");
                parSqlIn.Add("@idCod", FormatClauseIn(idCod));
            }

            AppendFiltriCodici(stbQuery, codiciDaEscludere, escludiCodiciCliente, parSqlIn);

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
                    await _tempChiaviMassivo.EliminaTabellaTemp_FiltroContatto(objParametriServer);
            }
        }

        public async Task<DataTable> LeggiCodiciAppezzamentiAsync(
            List<(string, int, int)> chiaviAppezzamento,
            List<int> idCod,
            AgronicaCoreParametriServer objParametriServer,
            List<int>? codiciDaEscludere = null,
            bool escludiCodiciCliente = false
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

            stbQuery.AppendLine(" SELECT Appezzamento_Codici.piva, Appezzamento_Codici.sa_cod, Appezzamento_Codici.appezza, id_cod, val_cod, Codici_Anagrafe.descrizione");
            stbQuery.AppendLine("      , Appezzamento_Codici.validita_inizio, Appezzamento_Codici.validita_fine");
            stbQuery.AppendLine(" FROM Appezzamento_Codici");
            if (usaTempTable)
                stbQuery.AppendLine(
                    " JOIN #TempAppezzamento temp ON temp.Piva = Appezzamento_Codici.Piva AND temp.sa_cod = Appezzamento_Codici.sa_cod AND temp.appezza = Appezzamento_Codici.appezza"
                );
            stbQuery.AppendLine(" INNER JOIN Codici_Anagrafe ON Codici_Anagrafe.Codice = Appezzamento_Codici.id_cod");
            stbQuery.AppendLine(" WHERE Appezzamento_Codici.Validita_inizio < @dtFine");
            stbQuery.AppendLine(" AND Appezzamento_Codici.Validita_Fine > @dtInizio");

            parametriSql.Add("@dtFine", objParametriServer.FinestraTemporaleFine);
            parametriSql.Add("@dtInizio", objParametriServer.FinestraTemporaleInizio);

            if (!usaTempTable && chiaviAppezzamento.Count == 1)
            {
                if (!string.IsNullOrWhiteSpace(chiaviAppezzamento[0].Item1))
                {
                    stbQuery.AppendLine(" AND Appezzamento_Codici.PIVA = @piva");
                    parametriSql.Add("@piva", chiaviAppezzamento[0].Item1.Trim());
                }

                if (chiaviAppezzamento[0].Item2 != 0)
                {
                    stbQuery.AppendLine(" AND Appezzamento_Codici.sa_cod = @sa_cod");
                    parametriSql.Add("@sa_cod", chiaviAppezzamento[0].Item2);
                }

                if (chiaviAppezzamento[0].Item3 != 0)
                {
                    stbQuery.AppendLine(" AND Appezzamento_Codici.appezza = @appezza");
                    parametriSql.Add("@appezza", chiaviAppezzamento[0].Item3);
                }
            }

            if (idCod != null && idCod.Count > 0)
            {
                stbQuery.AppendLine(" AND Appezzamento_Codici.id_cod IN (@idCod)");
                parSqlIn.Add("@idCod", FormatClauseIn(idCod));
            }

            AppendFiltriCodici(stbQuery, codiciDaEscludere, escludiCodiciCliente, parSqlIn);

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

        public async Task<DataTable> LeggiCodiciImpiantiAsync(
            List<(string, int, int, int)> chiaviImpianto,
            List<int> idCod,
            AgronicaCoreParametriServer objParametriServer,
            List<int>? codiciDaEscludere = null,
            bool escludiCodiciCliente = false
        )
        {
            bool usaTempTable = chiaviImpianto.Count > 1;

            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            Dictionary<string, Dictionary<Type, List<object>>> parSqlIn = new();

            if (usaTempTable)
                await _tempChiaviMassivo.CreaTabellaTemp_FiltroImpianti(
                    chiaviImpianto,
                    objParametriServer
                );

            stbQuery.AppendLine(" SELECT Reg_Impianti_Codici.piva, Reg_Impianti_Codici.sa_cod, Reg_Impianti_Codici.appezza, Reg_Impianti_Codici.id_reg,  Reg_Impianti_Codici.id_cod, val_cod, Codici_Anagrafe.descrizione");
            stbQuery.AppendLine("      , Reg_Impianti_Codici.validita_inizio, Reg_Impianti_Codici.validita_fine");
            stbQuery.AppendLine(" FROM Reg_Impianti_Codici");
            if (usaTempTable)
                stbQuery.AppendLine(
                    " JOIN #TempImpianto temp ON temp.Piva = Reg_Impianti_Codici.Piva AND temp.sa_cod = Reg_Impianti_Codici.sa_cod AND temp.appezza = Reg_Impianti_Codici.appezza AND temp.Id_Reg = Reg_Impianti_Codici.id_reg"
                );
            stbQuery.AppendLine(" INNER JOIN Codici_Anagrafe ON Codici_Anagrafe.Codice = Reg_Impianti_Codici.id_cod");
            stbQuery.AppendLine(" WHERE Reg_Impianti_Codici.Validita_inizio < @dtFine");
            stbQuery.AppendLine(" AND Reg_Impianti_Codici.Validita_Fine > @dtInizio");
            stbQuery.AppendLine(" AND Reg_Impianti_Codici.progetto_cod = 0");

            parametriSql.Add("@dtFine", objParametriServer.FinestraTemporaleFine);
            parametriSql.Add("@dtInizio", objParametriServer.FinestraTemporaleInizio);

            if (!usaTempTable && chiaviImpianto.Count == 1)
            {
                if (!string.IsNullOrWhiteSpace(chiaviImpianto[0].Item1))
                {
                    stbQuery.AppendLine(" AND Reg_Impianti_Codici.PIVA = @piva");
                    parametriSql.Add("@piva", chiaviImpianto[0].Item1.Trim());
                }

                if (chiaviImpianto[0].Item2 != 0)
                {
                    stbQuery.AppendLine(" AND Reg_Impianti_Codici.sa_cod = @sa_cod");
                    parametriSql.Add("@sa_cod", chiaviImpianto[0].Item2);
                }

                if (chiaviImpianto[0].Item3 != 0)
                {
                    stbQuery.AppendLine(" AND Reg_Impianti_Codici.appezza = @appezza");
                    parametriSql.Add("@appezza", chiaviImpianto[0].Item3);
                }

                if (chiaviImpianto[0].Item4 != 0)
                {
                    stbQuery.AppendLine(" AND Reg_Impianti_Codici.id_reg = @id_reg");
                    parametriSql.Add("@id_reg", chiaviImpianto[0].Item4);
                }
            }

            if (idCod != null && idCod.Count > 0)
            {
                stbQuery.AppendLine(" AND Reg_Impianti_Codici.id_cod IN (@idCod)");
                parSqlIn.Add("@idCod", FormatClauseIn(idCod));
            }

            AppendFiltriCodici(stbQuery, codiciDaEscludere, escludiCodiciCliente, parSqlIn);

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
                    await _tempChiaviMassivo.EliminaTabellaTemp_FiltroImpianti(objParametriServer);
            }
        }

        public async Task<DataTable> LeggiCodiciDestinazioniUsoAsync(List<(string, int, int, int)> chiaviImpianto, AgronicaCoreParametriServer objParametriServer)
        {
            var sb = new StringBuilder();

            var parametriSql = new Dictionary<string, object>
            {
                { "@idCodMinValue", 3000 },
                { "@idCodMaxValue", 3999 }
            };

            var useTempTable = chiaviImpianto.Count > 10; // Scegli se usare la tabella temporanea in base al numero di chiavi

            if (useTempTable)
            {
                await _tempChiaviMassivo.CreaTabellaTemp_FiltroImpianti(
                    chiaviImpianto,
                    objParametriServer
                );
            }

            sb.AppendLine(" SELECT DISTINCT ");
            sb.AppendLine("       ric.Id_Cod, ric.Val_Cod, ca.Descrizione, ");
            sb.AppendLine("       ric.Piva, ric.Sa_Cod, ric.Appezza, ric.Id_Reg ");
            sb.AppendLine(" FROM ");
            sb.AppendLine("       Reg_Impianti_Codici ric ");
            sb.AppendLine(" JOIN ");
            sb.AppendLine("       Codici_Anagrafe ca ");
            sb.AppendLine("       ON ric.Id_Cod = ca.Codice ");
            if (useTempTable)
            {
                sb.AppendLine(" JOIN ");
                sb.AppendLine("     #TempImpianto temp");
                sb.AppendLine("     ON temp.Piva = ric.Piva ");
                sb.AppendLine("     AND temp.sa_cod = ric.sa_cod ");
                sb.AppendLine("     AND temp.appezza = ric.appezza ");
                sb.AppendLine("     AND temp.Id_Reg = ric.id_reg");
            }
            sb.AppendLine(" WHERE ");
            sb.AppendLine("       ric.progetto_cod = 0 ");
            sb.AppendLine("       AND ric.Id_Cod >= @idCodMinValue ");
            sb.AppendLine("       AND ric.Id_Cod <= @idCodMinValue ");

            if (!useTempTable)
            {
                sb.AppendLine("     AND (");
                for (int i = 0; i < chiaviImpianto.Count; i++)
                {
                    var chiaveImpianto = chiaviImpianto[i];
                    sb.AppendLine($"     (ric.Piva = @piva{i} AND ric.sa_cod = @saCod{i} AND ric.appezza = @appezza{i} AND ric.Id_Reg = @idReg{i}) ");
                    if (i < chiaviImpianto.Count - 1)
                    {
                        sb.AppendLine("     OR ");
                    }
                    parametriSql.Add($"piva{i}", chiaveImpianto.Item1);
                    parametriSql.Add($"saCod{i}", chiaveImpianto.Item2);
                    parametriSql.Add($"appezza{i}", chiaveImpianto.Item3);
                    parametriSql.Add($"idReg{i}", chiaveImpianto.Item4);
                }

                sb.AppendLine("         )");
            }

            try
            {
                return await GetDataProvider(objParametriServer).ExecuteReadAsync(sb.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
            finally
            {
                if (useTempTable)
                {
                    await _tempChiaviMassivo.EliminaTabellaTemp_FiltroImpianti(objParametriServer);
                }
            }
        }

        public async Task<DataTable> LeggiCodiciEserciziAsync(
            List<(string, int, int, int, int)> chiaviEsercizio,
            List<int> idCod,
            AgronicaCoreParametriServer objParametriServer,
            List<int>? codiciDaEscludere = null,
            bool escludiCodiciCliente = false
        )
        {
            bool usaTempTable = chiaviEsercizio.Count > 1;

            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            Dictionary<string, Dictionary<Type, List<object>>> parSqlIn = new();

            if (usaTempTable)
              await  _tempChiaviMassivo.CreaTabellaTemp_FiltroEsercizi(
                    chiaviEsercizio,
                    objParametriServer
                );

            stbQuery.AppendLine(
                " SELECT Reg_Impianti_Codici.piva, Reg_Impianti_Codici.sa_cod, Reg_Impianti_Codici.appezza, Reg_Impianti_Codici.id_reg, Reg_Impianti_Codici.progetto_cod, id_cod, val_cod, Codici_Anagrafe.descrizione"
            );
            stbQuery.AppendLine("      , Reg_Impianti_Codici.validita_inizio, Reg_Impianti_Codici.validita_fine");
            stbQuery.AppendLine(" FROM Reg_Impianti_Codici");
            if (usaTempTable)
                stbQuery.AppendLine(
                     " JOIN #TempEsercizio temp ON temp.Piva = Reg_Impianti_Codici.Piva AND temp.sa_cod = Reg_Impianti_Codici.sa_cod AND temp.appezza = Reg_Impianti_Codici.appezza AND temp.Id_Reg = Reg_Impianti_Codici.id_reg AND temp.Progetto_Cod = Reg_Impianti_Codici.progetto_cod"
                );
            stbQuery.AppendLine(" INNER JOIN Codici_Anagrafe ON Codici_Anagrafe.Codice = Reg_Impianti_Codici.id_cod");
            stbQuery.AppendLine(" WHERE Reg_Impianti_Codici.Validita_inizio < @dtFine");
            stbQuery.AppendLine(" AND Reg_Impianti_Codici.Validita_Fine > @dtInizio");
            stbQuery.AppendLine(" AND Reg_Impianti_Codici.progetto_cod <> 0");

            parametriSql.Add("@dtFine", objParametriServer.FinestraTemporaleFine);
            parametriSql.Add("@dtInizio", objParametriServer.FinestraTemporaleInizio);

            if (!usaTempTable && chiaviEsercizio.Count == 1)
            {
                if (!string.IsNullOrWhiteSpace(chiaviEsercizio[0].Item1))
                {
                    stbQuery.AppendLine(" AND Reg_Impianti_Codici.PIVA = @piva");
                    parametriSql.Add("@piva", chiaviEsercizio[0].Item1.Trim());
                }

                if (chiaviEsercizio[0].Item2 != 0)
                {
                    stbQuery.AppendLine(" AND Reg_Impianti_Codici.sa_cod = @sa_cod");
                    parametriSql.Add("@sa_cod", chiaviEsercizio[0].Item2);
                }

                if (chiaviEsercizio[0].Item3 != 0)
                {
                    stbQuery.AppendLine(" AND Reg_Impianti_Codici.appezza = @appezza");
                    parametriSql.Add("@appezza", chiaviEsercizio[0].Item3);
                }

                if (chiaviEsercizio[0].Item4 != 0)
                {
                    stbQuery.AppendLine(" AND Reg_Impianti_Codici.id_reg = @id_reg");
                    parametriSql.Add("@id_reg", chiaviEsercizio[0].Item4);
                }

                if (chiaviEsercizio[0].Item5 != 0)
                {
                    stbQuery.AppendLine(" AND Reg_Impianti_Codici.progetto_cod = @progetto_cod");
                    parametriSql.Add("@progetto_cod", chiaviEsercizio[0].Item5);
                }
            }

            if (idCod != null && idCod.Count > 0)
            {
                stbQuery.AppendLine(" AND Reg_Impianti_Codici.id_cod IN (@idCod)");
                parSqlIn.Add("@idCod", FormatClauseIn(idCod));
            }

            AppendFiltriCodici(stbQuery, codiciDaEscludere, escludiCodiciCliente, parSqlIn);

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
                    await _tempChiaviMassivo.EliminaTabellaTemp_FiltroEsercizi(objParametriServer);
            }
        }

        public async Task<DataTable> LeggiCodiciFabbricatiAsync(
    List<(string, int, int)> chiaviFabbricato,
    List<int> idCod,
    AgronicaCoreParametriServer objParametriServer,
    List<int>? codiciDaEscludere = null,
    bool escludiCodiciCliente = false
)
        {
            bool usaTempTable = chiaviFabbricato.Count > 1;

            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            Dictionary<string, Dictionary<Type, List<object>>> parSqlIn = new();

            if (usaTempTable)
                await _tempChiaviMassivo.CreaTabellaTemp_FiltroFabbricati(
                    chiaviFabbricato,
                    objParametriServer
                );

            stbQuery.AppendLine(" SELECT fabbricati_codici.piva, fabbricati_codici.sa_cod, fabbricati_codici.fabbricato_cod, id_cod, val_cod, Codici_Anagrafe.descrizione");
            stbQuery.AppendLine("      , fabbricati_codici.validita_inizio, fabbricati_codici.validita_fine");
            stbQuery.AppendLine(" FROM fabbricati_codici");
            if (usaTempTable)
                stbQuery.AppendLine(
                    " JOIN #TempFabbricato temp ON temp.Piva = fabbricati_codici.Piva AND temp.sa_cod = fabbricati_codici.sa_cod AND temp.fabbricato_cod = fabbricati_codici.fabbricato_cod"
                );
            stbQuery.AppendLine(" INNER JOIN Codici_Anagrafe ON Codici_Anagrafe.Codice = fabbricati_codici.id_cod");
            stbQuery.AppendLine(" WHERE fabbricati_codici.Validita_inizio < @dtFine");
            stbQuery.AppendLine(" AND fabbricati_codici.Validita_Fine > @dtInizio");

            parametriSql.Add("@dtFine", objParametriServer.FinestraTemporaleFine);
            parametriSql.Add("@dtInizio", objParametriServer.FinestraTemporaleInizio);

            if (!usaTempTable && chiaviFabbricato.Count == 1)
            {
                if (!string.IsNullOrWhiteSpace(chiaviFabbricato[0].Item1))
                {
                    stbQuery.AppendLine(" AND fabbricati_codici.piva = @piva");
                    parametriSql.Add("@piva", chiaviFabbricato[0].Item1.Trim());
                }

                if (chiaviFabbricato[0].Item2 != 0)
                {
                    stbQuery.AppendLine(" AND fabbricati_codici.sa_cod = @sa_cod");
                    parametriSql.Add("@sa_cod", chiaviFabbricato[0].Item2);
                }

                if (chiaviFabbricato[0].Item3 != 0)
                {
                    stbQuery.AppendLine(" AND fabbricati_codici.fabbricato_cod = @fabbricatoCod");
                    parametriSql.Add("@fabbricatoCod", chiaviFabbricato[0].Item3);
                }
            }

            if (idCod != null && idCod.Count > 0)
            {
                stbQuery.AppendLine(" AND fabbricati_codici.id_cod IN (@idCod)");
                parSqlIn.Add("@idCod", FormatClauseIn(idCod));
            }

            AppendFiltriCodici(stbQuery, codiciDaEscludere, escludiCodiciCliente, parSqlIn);

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
                    await _tempChiaviMassivo.EliminaTabellaTemp_FiltroFabbricati(objParametriServer);
            }
        }


        private void AppendFiltriCodici(
            StringBuilder stbQuery,
            List<int>? codiciDaEscludere,
            bool escludiCodiciCliente,
            Dictionary<string, Dictionary<Type, List<object>>> parSqlIn
        )
        {
            if (codiciDaEscludere is { Count: > 0 })
            {
                stbQuery.AppendLine(" AND id_cod NOT IN (@parCodiciSkip)");
                parSqlIn.Add("@parCodiciSkip", FormatClauseIn(codiciDaEscludere));
            }

            if (escludiCodiciCliente)
                stbQuery.AppendLine(" AND (id_cod < 2000 OR id_cod >= 3000)");
        }
    }
}
