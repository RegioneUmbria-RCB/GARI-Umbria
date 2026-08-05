using System.Data;
using System.Text;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Widgets.DAL.Resources;
using Microsoft.Extensions.Localization;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.Widgets.DAL.DataLayer.WidgetsDocumentale.WidgetsDocumentale
{
    public class WidgetsDocumentale : BaseDALWidgets, IWidgetsDocumentale
    {
        public WidgetsDocumentale(IServiceProvider serviceProvider, IStringLocalizer<Messages> localizer) : base(serviceProvider, localizer) { }

        public async Task<DataTable?> GetDocumentRecapAsync(DateTime timeStart, bool useWorkflow, bool userVisibility, AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriUtenti objParametriUtenti)
        {
            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();
            DataTable? result;
            try
            {
                string UserDBName = GetDBName(objParametriUtenti);

                bool bAuthxUser = false; //We'll need to apply filters to the visibility
                Int16 iHierarchyLevel = 0; //Livello Max della tabella GerarchiaImprese (in modo da costruire un filtro dinamico)

                //Superuser won't need any filter
                bool isSuperUSer = objParametriServer.UtenteUsername.ToLower() == objParametriServer.SuperUserUsername.ToLower() ? true : false;

                if (!isSuperUSer)
                {
                    DataTable dt_Auths = await GetCategTipologiaDocumentiXUtentiAsync(objParametriServer);
                    if (dt_Auths.Rows.Count > 0)
                    {
                        bAuthxUser = true;

                        DataTable dt_Hierarchy = await GetHierarchyxChildAsync(objParametriServer);
                        if (dt_Hierarchy.Rows.Count > 0)
                        {
                            iHierarchyLevel = (Int16)dt_Hierarchy.Rows[0]["Livello"];
                        }
                    }
                }

                if (userVisibility)
                {
                    stbQuery.AppendLine(" SELECT ");
                    stbQuery.AppendLine("   Piva ");
                    stbQuery.AppendLine(" INTO #UtentiVisibilitaAppoggioImprese_TT ");
                    stbQuery.AppendLine(" FROM Utenti_Visibilita_Appoggio ");
                    stbQuery.AppendLine($" WHERE Entita_Cod = {(int)Enum_TipoEntita.Impresa} ");
                    stbQuery.AppendLine(" AND Username = @UserUsername ");
                    stbQuery.AppendLine("");
                }

                stbQuery.AppendLine(" SELECT UserName_Upload, _at.ID_Area, _ae.PIVA, COUNT(1) AS count_considered ");
                stbQuery.AppendLine(" INTO #Considered_TT ");
                stbQuery.AppendLine(" FROM Allegati_Documenti _ad ");
                stbQuery.AppendLine(" JOIN Alert_entita _ae ON _ae.Allegati_Documenti_Cod = _ad.Allegati_Documenti_Cod ");
                stbQuery.AppendLine(" JOIN Alert_Elenco _ael ON _ael.ID_Alert_Entita = _ae.ID_Alert_Entita  ");
                stbQuery.AppendLine(" JOIN Alert_Tipologia _at ON _at.ID_Tipologia = _ael.ID_Tipologia ");

                if (useWorkflow)
                {
                    stbQuery.AppendLine(" LEFT JOIN pratiche_Stati_Attuali psa ON _ad.pratica_Cod = psa.pratica_Cod ");
                    stbQuery.AppendLine(" WHERE Stato_Cod <> @workflow_daValidare ");
                }
                else
                {
                    stbQuery.AppendLine(" WHERE Validazione_Flag <> @daValidare ");
                }

                stbQuery.AppendLine(" AND _ad.Data_Upload >= @timeStart ");
                stbQuery.AppendLine(" GROUP BY _ae.PIVA, UserName_Upload, _at.ID_Area ");

                stbQuery.AppendLine("");

                stbQuery.AppendLine(" SELECT UserName_Upload, _at.ID_Area, _ae.PIVA, COUNT(1) AS count_to_consider ");
                stbQuery.AppendLine(" INTO #ToConsider_TT ");
                stbQuery.AppendLine(" FROM Allegati_Documenti _ad ");
                stbQuery.AppendLine(" JOIN Alert_entita _ae ON _ae.Allegati_Documenti_Cod = _ad.Allegati_Documenti_Cod ");
                stbQuery.AppendLine(" JOIN Alert_Elenco _ael ON _ael.ID_Alert_Entita = _ae.ID_Alert_Entita ");
                stbQuery.AppendLine(" JOIN Alert_Tipologia _at ON _at.ID_Tipologia = _ael.ID_Tipologia ");

                if (useWorkflow)
                {
                    stbQuery.AppendLine(" LEFT JOIN pratiche_Stati_Attuali psa ON _ad.pratica_Cod = psa.pratica_Cod ");
                    stbQuery.AppendLine(" WHERE Stato_Cod = @workflow_daValidare ");
                }
                else
                {
                    stbQuery.AppendLine(" WHERE Validazione_Flag = @daValidare ");
                }
                stbQuery.AppendLine(" AND _ad.Data_Upload >= @timeStart ");
                stbQuery.AppendLine(" GROUP BY _ae.PIVA, UserName_Upload, _at.ID_Area ");

                stbQuery.AppendLine("");

                stbQuery.AppendLine(" SELECT ");
                stbQuery.AppendLine("       ae.PIVA ");
                stbQuery.AppendLine("     , ad.UserName_Upload ");
                stbQuery.AppendLine("     , COUNT(1) AS count_document ");
                stbQuery.AppendLine("     , aa.ID_Area ");
                stbQuery.AppendLine("     , ISNULL(count_considered, 0) AS count_considered ");
                stbQuery.AppendLine("     , ISNULL(count_to_consider, 0) AS count_to_consider ");
                stbQuery.AppendLine(" INTO #Result_TT ");
                stbQuery.AppendLine(" FROM Alert_entita ae ");
                stbQuery.AppendLine(" JOIN Alert_Elenco ael ON ael.ID_Alert_Entita = ae.ID_Alert_Entita ");
                stbQuery.AppendLine(" JOIN Allegati_Documenti ad ON ad.Allegati_Documenti_Cod = ae.Allegati_Documenti_Cod ");
                stbQuery.AppendLine(" JOIN Alert_Tipologia at ON at.ID_Tipologia = ael.ID_Tipologia ");
                stbQuery.AppendLine(" JOIN Alert_Area aa ON aa.ID_Area = at.ID_Area ");
                stbQuery.AppendLine(" LEFT JOIN #ToConsider_TT toConsider ON toConsider.UserName_Upload = ad.UserName_Upload AND toConsider.ID_Area = aa.ID_Area AND toConsider.PIVA = ae.Piva ");
                stbQuery.AppendLine(" LEFT JOIN #Considered_TT Considered ON Considered.UserName_Upload = ad.UserName_Upload AND Considered.ID_Area = aa.ID_Area AND Considered.PIVA = ae.Piva  ");

                if (userVisibility)
                    stbQuery.AppendLine(" JOIN #UtentiVisibilitaAppoggioImprese_TT UtentiVisibilitaAppoggioImprese_TT ON UtentiVisibilitaAppoggioImprese_TT.Piva = ae.Piva ");

                if (bAuthxUser)
                {
                    //Sono stati inseriti dei record nella tabella CategTipologiaDocumentiXUtenti,
                    //l'utente deve per forza essere autorizzato ad utilizzare quella Categoria oppure quella specifica Tipologia


                    //'1. Controllo per Piva puntuale
                    stbQuery.AppendLine(" LEFT OUTER JOIN CategTipologiaDocumentiXUtenti permessi ON ");
                    stbQuery.AppendLine("     permessi.PivaSuperUser = at.PivaSuperUser ");
                    stbQuery.AppendLine(" AND permessi.ID_Categoria = at.ID_area ");
                    stbQuery.AppendLine(" AND (permessi.ID_Tipologia = at.ID_Tipologia OR permessi.ID_Tipologia = 0) ");
                    stbQuery.AppendLine(" AND permessi.Autorizzato IN (1,2) ");
                    stbQuery.AppendLine(" AND permessi.Username = @UserUsername ");
                    stbQuery.AppendLine(" AND permessi.Piva = ae.Piva ");

                    if (iHierarchyLevel > 1)
                    {
                        //2. Controllo per Piva Padre
                        stbQuery.AppendLine(" LEFT OUTER JOIN CategTipologiaDocumentiXUtenti permessi_padre ON ");
                        stbQuery.AppendLine("     permessi_padre.PivaSuperUser = at.PivaSuperUser ");
                        stbQuery.AppendLine(" AND permessi_padre.ID_Categoria = at.ID_area ");
                        stbQuery.AppendLine(" AND (permessi_padre.ID_Tipologia = at.ID_Tipologia OR permessi_padre.ID_Tipologia = 0) ");
                        stbQuery.AppendLine(" AND permessi_padre.Autorizzato IN (1,2) ");
                        stbQuery.AppendLine(" AND permessi_padre.Username = @UserUsername ");
                        stbQuery.AppendLine(" AND permessi_padre.Piva IN (SELECT Padre FROM GerarchiaImprese WHERE Figlio = ae.Piva) ");
                    }

                    if (iHierarchyLevel > 2)
                    {
                        //3. Controllo per Piva Nonno
                        stbQuery.AppendLine(" LEFT OUTER JOIN CategTipologiaDocumentiXUtenti permessi_nonno ON ");
                        stbQuery.AppendLine("     permessi_nonno.PivaSuperUser = at.PivaSuperUser ");
                        stbQuery.AppendLine(" AND permessi_nonno.ID_Categoria = at.ID_area ");
                        stbQuery.AppendLine(" AND (permessi_nonno.ID_Tipologia = at.ID_Tipologia OR permessi_nonno.ID_Tipologia = 0) ");
                        stbQuery.AppendLine(" AND permessi_nonno.Autorizzato IN (1,2) ");
                        stbQuery.AppendLine(" AND permessi_nonno.Username = @UserUsername ");
                        stbQuery.AppendLine(" AND permessi_nonno.Piva IN ( ");
                        stbQuery.AppendLine("       SELECT Padre FROM GerarchiaImprese WHERE Figlio IN ( ");
                        stbQuery.AppendLine("           SELECT Padre FROM GerarchiaImprese GerarchiaImpresePadre WHERE Figlio = ae.Piva ");
                        stbQuery.AppendLine("       ) ");
                        stbQuery.AppendLine(" ) ");
                    }

                    if (iHierarchyLevel > 3)
                    {
                        //4. Controllo per Piva Bis-Nonno
                        stbQuery.AppendLine(" LEFT OUTER JOIN CategTipologiaDocumentiXUtenti permessi_bis_nonno ON ");
                        stbQuery.AppendLine("     permessi_bis_nonno.PivaSuperUser = at.PivaSuperUser ");
                        stbQuery.AppendLine(" AND permessi_bis_nonno.ID_Categoria = at.ID_area ");
                        stbQuery.AppendLine(" AND (permessi_bis_nonno.ID_Tipologia = at.ID_Tipologia OR permessi_bis_nonno.ID_Tipologia = 0) ");
                        stbQuery.AppendLine(" AND permessi_bis_nonno.Autorizzato IN (1,2) ");
                        stbQuery.AppendLine(" AND permessi_bis_nonno.Username = @UserUsername ");
                        stbQuery.AppendLine(" AND permessi_bis_nonno.Piva IN ( ");
                        stbQuery.AppendLine("   SELECT Padre FROM GerarchiaImprese GerarchiaImpreseBisNonno WHERE Figlio IN ( ");
                        stbQuery.AppendLine("       SELECT Padre FROM GerarchiaImprese GerarchiaImpreseNonno2 WHERE Figlio IN ( ");
                        stbQuery.AppendLine("           SELECT Padre FROM GerarchiaImprese GerarchiaImpresePadre2 WHERE Figlio = ae.Piva ");
                        stbQuery.AppendLine("       ) ");
                        stbQuery.AppendLine("   ) ");
                        stbQuery.AppendLine(" ) ");
                    }

                    if (iHierarchyLevel > 4)
                    {
                        //5. Controllo per Piva Tris-Nonno
                        stbQuery.AppendLine(" LEFT OUTER JOIN CategTipologiaDocumentiXUtenti permessi_tris_nonno ON ");
                        stbQuery.AppendLine("     permessi_tris_nonno.PivaSuperUser = at.PivaSuperUser ");
                        stbQuery.AppendLine(" AND permessi_tris_nonno.ID_Categoria = at.ID_area ");
                        stbQuery.AppendLine(" AND (permessi_tris_nonno.ID_Tipologia = at.ID_Tipologia OR permessi_tris_nonno.ID_Tipologia = 0) ");
                        stbQuery.AppendLine(" AND permessi_tris_nonno.Autorizzato IN (1,2) ");
                        stbQuery.AppendLine(" AND permessi_tris_nonno.Username = @UserUsername ");
                        stbQuery.AppendLine(" AND permessi_tris_nonno.Piva IN (");
                        stbQuery.AppendLine("   SELECT Padre FROM GerarchiaImprese GerarchiaImpreseTrisNonno WHERE Figlio IN ( ");
                        stbQuery.AppendLine("       SELECT Padre FROM GerarchiaImprese GerarchiaImpreseBisNonno3 WHERE Figlio IN ( ");
                        stbQuery.AppendLine("           SELECT Padre FROM GerarchiaImprese GerarchiaImpreseNonno3 WHERE Figlio IN ( ");
                        stbQuery.AppendLine("               SELECT Padre FROM GerarchiaImprese GerarchiaImpresePadre3 WHERE Figlio = ae.Piva ");
                        stbQuery.AppendLine("           ) ");
                        stbQuery.AppendLine("       ) ");
                        stbQuery.AppendLine("   ) ");
                        stbQuery.AppendLine(" ) ");
                    }

                    if (iHierarchyLevel > 5)
                    {
                        //6. Controllo per Piva Quad-Nonno

                        stbQuery.AppendLine(" LEFT OUTER JOIN CategTipologiaDocumentiXUtenti permessi_Quad_nonno ON ");
                        stbQuery.AppendLine("     permessi_Quad_nonno.PivaSuperUser = at.PivaSuperUser ");
                        stbQuery.AppendLine(" AND permessi_Quad_nonno.ID_Categoria = at.ID_area ");
                        stbQuery.AppendLine(" AND (permessi_Quad_nonno.ID_Tipologia = at.ID_Tipologia OR permessi_Quad_nonno.ID_Tipologia = 0) ");
                        stbQuery.AppendLine(" AND permessi_Quad_nonno.Autorizzato IN (1,2) ");
                        stbQuery.AppendLine(" AND permessi_Quad_nonno.Username = @UserUsername ");
                        stbQuery.AppendLine(" AND permessi_Quad_nonno.Piva IN ");
                        stbQuery.AppendLine("       SELECT Padre FROM GerarchiaImprese GerarchiaImpreseQuadNonno WHERE Figlio IN ( ");
                        stbQuery.AppendLine("           SELECT Padre FROM GerarchiaImprese GerarchiaImpreseTrisNonno4 WHERE Figlio IN ( ");
                        stbQuery.AppendLine("               SELECT Padre FROM GerarchiaImprese GerarchiaImpreseBisNonno4 WHERE Figlio IN ( ");
                        stbQuery.AppendLine("                   SELECT Padre FROM GerarchiaImprese GerarchiaImpreseNonno4 WHERE Figlio IN ( ");
                        stbQuery.AppendLine("                       SELECT Padre FROM GerarchiaImprese GerarchiaImpresePadre4 WHERE Figlio = ae.Piva) ");
                        stbQuery.AppendLine("                   ) ");
                        stbQuery.AppendLine("               ) ");
                        stbQuery.AppendLine("           ) ");
                        stbQuery.AppendLine("       ) ");
                        stbQuery.AppendLine(" ) ");
                    }
                }


                stbQuery.AppendLine(" WHERE 1 = 1 ");
                stbQuery.AppendLine(" AND ae.PivaSuperUser = @PivaSuperUser ");
                stbQuery.AppendLine(" AND ad.Data_Upload >= @timeStart ");
                stbQuery.AppendLine(" AND ID_Elenco NOT IN (SELECT ID_Elenco FROM Alert_Elenco_x_Utente WHERE Alert_Elenco_x_Utente.ID_Elenco = ael.ID_Elenco AND non_mostrare = -1 AND Alert_Elenco_x_Utente.Username = @UsernameOperazione) ");


                if (bAuthxUser)
                {
                    //Condizione almeno un autorizzazione valida
                    stbQuery.AppendLine(" AND ((ISNULL(permessi.Autorizzato, 0) ");

                    if (iHierarchyLevel > 1)
                        stbQuery.AppendLine("     + ISNULL(permessi_padre.Autorizzato, 0) ");

                    if (iHierarchyLevel > 2)
                        stbQuery.AppendLine("     + ISNULL(permessi_nonno.Autorizzato, 0) ");

                    if (iHierarchyLevel > 3)
                        stbQuery.AppendLine("     + ISNULL(permessi_bis_nonno.Autorizzato, 0) ");

                    if (iHierarchyLevel > 4)
                        stbQuery.AppendLine("     + ISNULL(permessi_tris_nonno.Autorizzato, 0) ");

                    if (iHierarchyLevel > 5)
                        stbQuery.AppendLine("     + ISNULL(permessi_quad_nonno.Autorizzato, 0) ");
                    stbQuery.AppendLine(" ) > 0) ");
                }

                stbQuery.AppendLine(" GROUP BY ae.PIVA, ad.UserName_Upload, aa.ID_Area, count_considered, count_to_consider ");

                stbQuery.AppendLine("");

                stbQuery.AppendLine(" SELECT ");
                stbQuery.AppendLine("     Imprese.rag_soc AS company ");
                stbQuery.AppendLine("   , (LTRIM(RTRIM(ud.cognome + ' ' + ud.nome + ' ' + ud.Rag_Soc))) AS user_upload ");
                stbQuery.AppendLine("   , aa.Nome AS category ");
                stbQuery.AppendLine("   , count_document ");
                stbQuery.AppendLine("   , count_considered ");
                stbQuery.AppendLine("   , count_to_consider ");
                stbQuery.AppendLine(" FROM #Result_TT Result ");

                stbQuery.AppendLine(" JOIN Imprese ON Imprese.PIVA = Result.Piva ");
                stbQuery.AppendLine(" JOIN [" + UserDBName + "].[dbo].Utenti_dettagli ud ON ud.UserName = Result.UserName_Upload ");
                stbQuery.AppendLine(" JOIN Alert_Area aa ON aa.ID_Area = Result.ID_Area ");

                stbQuery.AppendLine(" ORDER BY Imprese.rag_soc ");

                stbQuery.AppendLine("");

                if (userVisibility) stbQuery.AppendLine(" DROP TABLE #UtentiVisibilitaAppoggioImprese_TT ");
                stbQuery.AppendLine(" DROP TABLE #Considered_TT ");
                stbQuery.AppendLine(" DROP TABLE #ToConsider_TT ");
                stbQuery.AppendLine(" DROP TABLE #Result_TT ");

                parSql.Add("@workflow_daValidare", 400);
                parSql.Add("@daValidare", 0);
                parSql.Add("@timeStart", timeStart);

                parSql.Add("@PivaSuperUser", objParametriServer.PivaSuperUser);
                parSql.Add("@UserUsername", objParametriServer.UtenteUsername);
                parSql.Add("@UsernameOperazione", objParametriServer.UsernameOperazione);

                result = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parSql);

            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                result = null;
            }
            return result;
        }

        private string GetDBName(AgronicaCoreParametri objParametri)
        {

            string UserDBName = "";

            string[] dummy = objParametri.StringaConnessione.Split(';');

            //[0] Provider = SQLOLEDB;
            //[1] Server = ;
            //[2] Initial Catalog = DB_Name;
            //[3] User Id = ;
            //[4] Password = ;

            if (dummy.Length > 0)
            {
                UserDBName = dummy[2].Split("=")[1];
            }

            return UserDBName;

        }

        private async Task<DataTable> GetCategTipologiaDocumentiXUtentiAsync(AgronicaCoreParametriServer objParametriServer)
        {
            var parSql = new Dictionary<string, object>();
            DataTable result;
            var stbQuery = new StringBuilder();
            try
            {
                stbQuery.AppendLine("  SELECT * ");
                stbQuery.AppendLine("  FROM CategTipologiaDocumentiXUtenti ");
                stbQuery.AppendLine("  WHERE PivaSuperUser = @PivaSuperUser ");

                parSql.Add("@PivaSuperUser", objParametriServer.PivaSuperUser);

                result = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
            return result;
        }

        private async Task<DataTable> GetHierarchyxChildAsync(AgronicaCoreParametriServer objParametriServer)
        {
            var parSql = new Dictionary<string, object>();
            DataTable result;
            var stbQuery = new StringBuilder();
            try
            {
                stbQuery.AppendLine("  SELECT GerarchiaImprese.* ");
                stbQuery.AppendLine("  FROM  GerarchiaImprese ");
                stbQuery.AppendLine("  INNER JOIN UtentiXImprese ON GerarchiaImprese.Figlio = UtentiXImprese.PIVA ");
                stbQuery.AppendLine("  WHERE UtentiXImprese.[USER] = @PivaSuperUser ");
                stbQuery.AppendLine("  ORDER BY Livello DESC ");

                parSql.Add("@PivaSuperUser", objParametriServer.PivaSuperUser);

                result = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parSql);
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
