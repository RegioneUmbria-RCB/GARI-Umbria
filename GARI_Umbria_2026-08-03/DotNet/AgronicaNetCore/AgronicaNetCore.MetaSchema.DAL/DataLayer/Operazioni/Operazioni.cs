using AgronicaCoreDTOStd.InData.Metaschema;
using AgronicaCoreDTOStd.OutData.FiltroRicerca;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MetaSchema.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Text;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.Operazioni
{
    public class Operazioni : BaseDALMetaschema, IOperazioni
    {
        public Operazioni(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
        }

        public async Task<DtConVisibilita_OUT> Operazioni_GestioneFiltroUtente_LeggiAsync(LeggiOperazioni_IN leggiOperazioni, DataTable utentiImpostazioniDt, AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriUtenti objParametriUtenti)
        {
            DtConVisibilita_OUT result = new DtConVisibilita_OUT();

            result.VisibilitaApplicata = utentiImpostazioniDt.Rows.Count > 0;

            if (utentiImpostazioniDt.Rows.Count > 0)
            {
                result.DataTable = await LeggiOperazioniApplicandoLimitiDiVisibilitaAsync(utentiImpostazioniDt, leggiOperazioni, objParametriUtenti, objParametriServer);
            }
            else
            {
                result.DataTable = await LeggiAsync(leggiOperazioni, objParametriServer);
            }

            return result;
        }

        public async Task<DataTable> LeggiOperazioniPerTipoAsync(string tipoGruppoOperazione, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();

            stbQuery.AppendLine(
                " SELECT  Operazioni.LAV_COD, coalesce ( operazioni_XLingue.lav_des, Operazioni.LAV_DES) as LAV_DES, Operazioni.P, Operazioni.Validita_Inizio, Operazioni.Validita_Fine, Operazioni.GRU_OP,GruppoOperazioni.GRU_COD, ");
            stbQuery.AppendLine("         GruppoOperazioni.GRU_DES, GruppoOperazioni.Tipo, GruppoOperazioni.ATT_COD ");
            stbQuery.AppendLine(" FROM    Operazioni ");
            stbQuery.AppendLine(" INNER JOIN GruppoOperazioni ON Operazioni.GRU_OP = GruppoOperazioni.GRU_COD ");
            stbQuery.AppendLine(
                " LEFT JOIN operazioni_XLingue ON Operazioni.LAV_COD = operazioni_XLingue.LAV_COD AND operazioni_XLingue.Lingua_COD = @linguaCod ");
            stbQuery.AppendLine(" WHERE   Operazioni.Validita_Inizio < @dtFine");
            stbQuery.AppendLine(" AND     Operazioni.Validita_Fine > @dtInizio ");

            sqlParams.Add("@linguaCod", objParametriServer.Lingua_Cod);
            sqlParams.Add("@dtFine", objParametriServer.FinestraTemporaleFine);
            sqlParams.Add("@dtInizio", objParametriServer.FinestraTemporaleInizio);

            if (tipoGruppoOperazione != "0")
            {
                stbQuery.AppendLine(" AND GruppoOperazioni.Tipo = @tipo ");
                sqlParams.Add("@tipo", tipoGruppoOperazione);
            }

            stbQuery.AppendLine(
                " ORDER BY GruppoOperazioni.GRU_DES, coalesce(operazioni_XLingue.lav_des, Operazioni.LAV_DES) ");

            try
            {
                return await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), sqlParams);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        private async Task<DataTable> LeggiOperazioniApplicandoLimitiDiVisibilitaAsync(DataTable utentiImpostazioniLivelloGruCodDt,
            LeggiOperazioni_IN leggiOperazioni, AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer)
        {
            DataTable result = new DataTable();

            var applicaFiltroSuPivaSuperUser = !string.IsNullOrEmpty(objParametriUtenti.PivaSuperUser);
            var applicaFiltroSuUtenteUsername = !string.IsNullOrEmpty(objParametriUtenti.UtenteUsername);
            var applicaFiltroSuGruppoOperazioni = (leggiOperazioni.GruppoOperazioni ??= new()).Any();
            var applicaFiltroSuOperazioni = (leggiOperazioni.Operazioni ??= new()).Any();

            var primoElemento = true;

            //itero su tutti i gru_cod che l'utente può vedere
            foreach (DataRow row in utentiImpostazioniLivelloGruCodDt.Rows)
            {
                var gruCod = (int)row["ID_0"];

                var stbQuery = new StringBuilder();
                var parametriSql = new Dictionary<string, object>();
                var parametriSqlIn = new Dictionary<string, Dictionary<Type, List<object>>>();

                DataTable intermediateResult;

                //per ogni gru_cod devo capire se l'utente può vedere tutto il gru_cod o solo parte di quel gru_cod

                stbQuery.AppendLine(" IF           (");
                stbQuery.AppendLine(" SELECT       COUNT(*)");
                stbQuery.AppendLine(" FROM         Operazioni");
                stbQuery.AppendLine(" INNER JOIN   GruppoOperazioni");
                stbQuery.AppendLine(" ON           Operazioni.GRU_OP = GruppoOperazioni.GRU_COD");
                stbQuery.AppendLine(" INNER JOIN   Utenti_Impostazioni_FiltroMono");
                stbQuery.AppendLine(" ON           Operazioni.LAV_COD = Utenti_Impostazioni_FiltroMono.ID_0");
                stbQuery.AppendLine(" INNER JOIN   Utenti_Impostazioni");
                stbQuery.AppendLine(" ON           Utenti_Impostazioni.Piva_SuperUser = Utenti_Impostazioni_FiltroMono.Piva_SuperUser");
                stbQuery.AppendLine(" AND          Utenti_Impostazioni.UserName = Utenti_Impostazioni_FiltroMono.UserName");
                stbQuery.AppendLine(" AND          Utenti_Impostazioni.Impostazione_Cod = Utenti_Impostazioni_FiltroMono.Impostazione_Cod");
                stbQuery.AppendLine(" WHERE        Operazioni.Validita_Inizio <= @dtFine");
                stbQuery.AppendLine(" AND          Operazioni.Validita_Fine >= @dtInizio");
                stbQuery.AppendLine(" AND          Utenti_Impostazioni_FiltroMono.Impostazione_Cod = @impostazioneCod");
                stbQuery.AppendLine(" AND          Operazioni.GRU_OP = @gruCod");

                if (applicaFiltroSuPivaSuperUser)
                    stbQuery.AppendLine(" AND          Utenti_Impostazioni_FiltroMono.Piva_SuperUser = @pivaSuperUser");

                if (applicaFiltroSuUtenteUsername)
                    stbQuery.AppendLine(" AND          Utenti_Impostazioni_FiltroMono.Username = @username");

                stbQuery.AppendLine("              ) > 0");

                stbQuery.AppendLine("     SELECT       Operazioni.LAV_COD, Operazioni.LAV_DES, Operazioni.GRU_OP, GruppoOperazioni.GRU_COD, GruppoOperazioni.GRU_DES");
                stbQuery.AppendLine("     FROM         Operazioni");
                stbQuery.AppendLine("     INNER JOIN   GruppoOperazioni");
                stbQuery.AppendLine("     ON           Operazioni.GRU_OP = GruppoOperazioni.GRU_COD");
                stbQuery.AppendLine("     INNER JOIN   Utenti_Impostazioni_FiltroMono");
                stbQuery.AppendLine("     ON           Operazioni.LAV_COD = Utenti_Impostazioni_FiltroMono.ID_0");
                stbQuery.AppendLine("     INNER JOIN   Utenti_Impostazioni");
                stbQuery.AppendLine("     ON           Utenti_Impostazioni.Piva_SuperUser = Utenti_Impostazioni_FiltroMono.Piva_SuperUser");
                stbQuery.AppendLine("     AND          Utenti_Impostazioni.UserName = Utenti_Impostazioni_FiltroMono.UserName");
                stbQuery.AppendLine("     AND          Utenti_Impostazioni.Impostazione_Cod = Utenti_Impostazioni_FiltroMono.Impostazione_Cod");
                stbQuery.AppendLine("     WHERE        Operazioni.Validita_Inizio <= @dtFine");
                stbQuery.AppendLine("     AND          Operazioni.Validita_Fine >= @dtInizio");
                stbQuery.AppendLine("     AND          Utenti_Impostazioni_FiltroMono.Impostazione_Cod = @impostazioneCod");
                stbQuery.AppendLine("     AND          Operazioni.GRU_OP = @gruCod");
                stbQuery.AppendLine($"     AND          ({CostantiPersonalizzate.STR_OP_NON_GESTITE})");

                if (applicaFiltroSuPivaSuperUser)
                    stbQuery.AppendLine("     AND          Utenti_Impostazioni_FiltroMono.Piva_SuperUser = @pivaSuperUser");

                if (applicaFiltroSuUtenteUsername)
                    stbQuery.AppendLine("     AND          Utenti_Impostazioni_FiltroMono.Username = @username");

                if (applicaFiltroSuOperazioni)
                    stbQuery.AppendLine("     AND          Operazioni.LAV_COD IN (@parLavCod)");


                if (applicaFiltroSuGruppoOperazioni)
                    stbQuery.AppendLine("     AND          GruppoOperazioni.Gru_Cod IN (@parGruCod)");


                stbQuery.AppendLine(" ELSE");
                stbQuery.AppendLine("     SELECT       Operazioni.LAV_COD, Operazioni.LAV_DES, Operazioni.GRU_OP, GruppoOperazioni.GRU_COD, GruppoOperazioni.GRU_DES");
                stbQuery.AppendLine("     FROM         Operazioni");
                stbQuery.AppendLine("     INNER JOIN   GruppoOperazioni");
                stbQuery.AppendLine("     ON           Operazioni.GRU_OP = GruppoOperazioni.GRU_COD");
                stbQuery.AppendLine("     WHERE        Operazioni.Validita_Inizio <= @dtFine");
                stbQuery.AppendLine("     AND          Operazioni.Validita_Fine >= @dtInizio");
                stbQuery.AppendLine("     AND          Operazioni.GRU_OP = @gruCod");
                stbQuery.AppendLine($"     AND          ({CostantiPersonalizzate.STR_OP_NON_GESTITE})");

                if (applicaFiltroSuOperazioni)
                    stbQuery.AppendLine("     AND          Operazioni.LAV_COD IN (@parLavCod)");


                if (applicaFiltroSuGruppoOperazioni)
                    stbQuery.AppendLine("     AND          GruppoOperazioni.Gru_Cod IN (@parGruCod)");

                parametriSql.Add("@impostazioneCod", TipiEnumerativi.Enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_LAVORAZIONI);
                parametriSql.Add("@gruCod", gruCod);
                parametriSql.Add("@dtInizio", objParametriUtenti.FinestraTemporaleInizio);
                parametriSql.Add("@dtFine", objParametriUtenti.FinestraTemporaleFine);


                if (applicaFiltroSuPivaSuperUser)
                    parametriSql.Add("@pivaSuperUser", objParametriUtenti.PivaSuperUser);

                if (applicaFiltroSuUtenteUsername)
                    parametriSql.Add("@username", objParametriUtenti.UtenteUsername);

                if (applicaFiltroSuGruppoOperazioni)
                    parametriSqlIn.Add("@parGruCod", FormatClauseIn(leggiOperazioni.GruppoOperazioni));


                if (applicaFiltroSuOperazioni)
                    parametriSqlIn.Add("@parLavCod", FormatClauseIn(leggiOperazioni.Operazioni));

                try
                {
                    intermediateResult = await GetDataProvider(objParametriUtenti).ExecuteReadAsync(stbQuery.ToString(), parametriSql, parametriSqlIn);
                }
                catch (Exception ex)
                {
                    LogError(ex.Message, objParametriServer, ex);
                    throw;
                }

                if (primoElemento)
                {
                    result = intermediateResult.Clone();
                    primoElemento = false;
                }

                foreach (DataRow intermediateResultRow in intermediateResult.Rows)
                {
                    result.ImportRow(intermediateResultRow);
                }
            }

            return result;
        }

        private async Task<DataTable> LeggiAsync(LeggiOperazioni_IN leggiOperazioni, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            DataTable result;
            try
            {
                stbQuery.AppendLine(" SELECT      Operazioni.LAV_COD, Operazioni.LAV_DES, Operazioni.GRU_OP, ");
                stbQuery.AppendLine("             GruppoOperazioni.GRU_COD, GruppoOperazioni.GRU_DES ");
                stbQuery.AppendLine(" FROM        Operazioni");
                stbQuery.AppendLine(" INNER JOIN  GruppoOperazioni");
                stbQuery.AppendLine(" ON          Operazioni.GRU_OP = GruppoOperazioni.GRU_COD");
                stbQuery.AppendLine(" WHERE       Operazioni.Validita_Inizio <= @dtFine");
                stbQuery.AppendLine(" AND         Operazioni.Validita_Fine >= @dtInizio");

                if (leggiOperazioni?.GruppoOperazioni?.Any() == true)
                    stbQuery.AppendLine(" AND         GruppoOperazioni.Gru_Cod IN (" + string.Join(',', leggiOperazioni.GruppoOperazioni) + ")");

                if (leggiOperazioni?.Operazioni?.Any() == true)
                    stbQuery.AppendLine(" AND         Operazioni.LAV_COD IN (" + string.Join(',', leggiOperazioni.Operazioni) + ")");

                stbQuery.AppendLine($" AND         ({CostantiPersonalizzate.STR_OP_NON_GESTITE})");

                parametriSql.Add("@dtInizio", objParametriServer.FinestraTemporaleInizio);
                parametriSql.Add("@dtFine", objParametriServer.FinestraTemporaleFine);


                result = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            return result;
        }
    }
}