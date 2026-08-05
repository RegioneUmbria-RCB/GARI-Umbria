using System.Data;
using System.Text;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MetaSchema.DAL.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.ParcoMacchine;

public class ParcoMacchine : BaseDALMetaschema, IParcoMacchine
{

    public ParcoMacchine(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer) { }

    public async Task<DataTable> ParcoMacchine_LeggiAsync(string piva, string visibilityFilter, string orderByField,
        AgronicaCoreParametriServer objParametriServer)
    {
        var stbQuery = new StringBuilder();
        var sqlParams = new Dictionary<string, object>();

        stbQuery.AppendLine(" SELECT Parco_Macchine.* , Imprese.Rag_Soc as Referente, D.Ditta_Des ");
        stbQuery.AppendLine(" FROM   Parco_Macchine LEFT OUTER JOIN Imprese ON Parco_Macchine.Piva = Imprese.Piva ");
        stbQuery.AppendLine(" INNER  JOIN UtentiXImprese ON Parco_Macchine.Piva = UtentiXImprese.PIVA ");
        stbQuery.AppendLine(" LEFT JOIN Ditte D ON Parco_Macchine.Ditta_Cod = D.Ditta_Cod ");
        stbQuery.AppendLine(" WHERE  Parco_Macchine.Validita_inizio < @dtFine ");
        stbQuery.AppendLine(" AND    Parco_Macchine.Validita_Fine > @dtInizio ");
        stbQuery.AppendLine(" AND    UtentiXImprese.[USER] = @piva ");
        stbQuery.AppendLine($" AND   {visibilityFilter} ");

        sqlParams.Add("@dtFine", objParametriServer.FinestraTemporaleFine);
        sqlParams.Add("@dtInizio", objParametriServer.FinestraTemporaleInizio);
        sqlParams.Add("@piva", objParametriServer.PivaSuperUser.Trim());

        if (string.IsNullOrEmpty(orderByField))
        {
            stbQuery.AppendLine("ORDER BY Parco_Macchine.Piva ASC ");
        }
        else
        {
            stbQuery.AppendLine($"ORDER BY {orderByField} ");
        }

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

    public async Task<string?> LeggiDesFromMacCodAsync(string piva, int macCod, DataTable dtCentriVisibili, AgronicaCoreParametriServer objParametriServer)
    {
        var dt = await LeggiParcoMacchinexSuperUserAsync(piva, macCod, 0, true, dtCentriVisibili, objParametriServer);
        if (dt.Rows.Count <= 0)
        {
            return null;
        }

        return dt.Rows[0]["Mac_Des"].Equals("")
            ? $"{dt.Rows[0]["Ditta_Des"]} {dt.Rows[0]["Modello"]}"
            : dt.Rows[0]["Mac_Des"].ToString();
    }

    public async Task<DataTable> LeggiParcoMacchinexSuperUserAsync(string piva,
                                                                   int macCod,
                                                                   int macCodOrigine,
                                                                   bool ancheImportati,
                                                                   string filtroAggiuntivo,
                                                                   string orderBy,
                                                                   DataTable dtCentriVisibili,
                                                                   AgronicaCoreParametriServer objParametriServer)
    {
        var stbQuery = new StringBuilder();
        var sqlParams = new Dictionary<string, object>();
        stbQuery.AppendLine(" SELECT Parco_Macchine.* , Imprese.Rag_Soc as Referente ");
        stbQuery.AppendLine(" FROM   Parco_Macchine LEFT OUTER JOIN Imprese ON Parco_Macchine.Piva = Imprese.Piva ");
        stbQuery.AppendLine(" INNER  JOIN UtentiXImprese ON Parco_Macchine.Piva = UtentiXImprese.PIVA ");
        stbQuery.AppendLine(" WHERE  Parco_Macchine.Validita_inizio <= @dtFine ");
        stbQuery.AppendLine(" AND    Parco_Macchine.Validita_Fine >= @dtInizio ");
        stbQuery.AppendLine(" AND    UtentiXImprese.[USER] = @pivaSuperUser ");

        sqlParams.Add("@dtFine", objParametriServer.FinestraTemporaleFine);
        sqlParams.Add("@dtInizio", objParametriServer.FinestraTemporaleInizio);
        sqlParams.Add("@pivaSuperUser", objParametriServer.PivaSuperUser.Trim());

        if (macCod == 0)
        {
            // Lettura delle Macchine Visibili dall'Impresa
            stbQuery.AppendLine(" AND    ( (Parco_Macchine.Sa_Cod = -1) ");

            var filtroCentri = "";
            if (dtCentriVisibili is { Rows.Count: > 0 })
            {
                foreach (DataRow row in dtCentriVisibili.Rows)
                {
                    filtroCentri +=
                        $" (Parco_Macchine.Piva = '{row["piva"]}' AND Parco_Macchine.Sa_Cod = {row["sa_cod"]}) OR ";
                }

                if (filtroCentri != "")
                {
                    stbQuery.AppendLine(
                        $" OR ({filtroCentri[..^3]} OR (Parco_Macchine.Piva = '{piva}' AND Parco_Macchine.Sa_Cod = 0) ) ");
                }
            }

            // aziendali
            if (string.IsNullOrEmpty(filtroCentri))
            {
                stbQuery.AppendLine("          OR  (Parco_Macchine.Piva = @piva)  ");
                sqlParams.Add("@piva", piva);
            }

            stbQuery.AppendLine("        ) ");
        }
        else
        {
            // Lettura Mirata
            stbQuery.AppendLine(" AND Parco_Macchine.Mac_Cod = @macCod   ");
            sqlParams.Add("@macCod", macCod);
        }

        if (macCodOrigine != 0)
        {
            stbQuery.AppendLine(" AND  Parco_Macchine.Mac_Cod_Origine = @macCodOrigine   ");
            sqlParams.Add("@macCodOrigine", macCodOrigine);
        }

        if (ancheImportati)
        {
            stbQuery.AppendLine(" AND  Parco_Macchine.Mac_Cod_Origine = 0 ");
        }

        if (!string.IsNullOrEmpty(filtroAggiuntivo))
        {
            stbQuery.AppendLine(" AND " + filtroAggiuntivo);
        }

        switch (objParametriServer.FlagVisibilita)
        {
            case AgronicaCoreParametri.enumVisibilita.visibilita_SoloNonInviati:
                stbQuery.AppendLine(" AND   Parco_Macchine.Inviato = 0 ");
                break;
            case AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati:
                stbQuery.AppendLine(" AND   Parco_Macchine.Inviato =-1 ");
                break;
            case AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti:
                break;
            default:
                throw new Exception("Parametro non corretto nella query (FlagVisibilita)");
        }

        if (orderBy != "")
        {
            stbQuery.AppendLine(" ORDER BY " + orderBy);
        }
        else
        {
            stbQuery.AppendLine(" ORDER BY Parco_Macchine.Piva ASC ");
        }


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

    private async Task<DataTable> LeggiParcoMacchinexSuperUserAsync(string piva, int macCod, int macCodOrigine, bool ancheImportati, DataTable dtCentriVisibili,
        AgronicaCoreParametriServer objParametriServer)
    {
        var stbQuery = new StringBuilder();
        var sqlParams = new Dictionary<string, object>();

        stbQuery.AppendLine(" SELECT Parco_Macchine.* , Imprese.Rag_Soc as Referente, Ditte.Ditta_Des, Macchine.* ");
        stbQuery.AppendLine(
            ", ISNULL((select class_desc from macchine where CLASS_CODE =substring (Parco_Macchine.Class_Code, 1 , 2) and Len(Class_Code)=2),'') as tipo_desc   ");
        stbQuery.AppendLine(
            ", ISNULL((select class_desc from macchine where CLASS_CODE =substring (Parco_Macchine.Class_Code, 1 , 5) and Len(Class_Code)=5),'') as dettaglio_1_desc  ");
        stbQuery.AppendLine(
            ", ISNULL((select class_desc from macchine where CLASS_CODE =substring (Parco_Macchine.Class_Code, 1 , 7) and Len(Class_Code)=7 ),'') as dettaglio_2_desc ");
        stbQuery.AppendLine(" FROM Parco_Macchine (NOLOCK) LEFT OUTER JOIN ");
        stbQuery.AppendLine("   Imprese (NOLOCK) ON Parco_Macchine.Piva = Imprese.PIVA LEFT OUTER JOIN ");
        stbQuery.AppendLine("   Ditte (NOLOCK) ON Parco_Macchine.Ditta_Cod = Ditte.Ditta_Cod LEFT OUTER JOIN ");
        stbQuery.AppendLine("   UtentiXImprese (NOLOCK) ON Parco_Macchine.Piva = UtentiXImprese.PIVA INNER JOIN ");
        stbQuery.AppendLine("   Macchine (NOLOCK) ON Parco_Macchine.Class_Code = Macchine.CLASS_CODE ");
        stbQuery.AppendLine(" WHERE  Parco_Macchine.Validita_inizio <= @dtFine ");
        stbQuery.AppendLine(" AND    Parco_Macchine.Validita_Fine >= @dtInizio ");
        stbQuery.AppendLine(" AND    UtentiXImprese.[USER] = @pivaSuperUser");
        stbQuery.AppendLine(" AND    Parco_Macchine.Cod_Contatto = ''  ");

        sqlParams.Add("@dtFine", objParametriServer.FinestraTemporaleFine);
        sqlParams.Add("@dtInizio", objParametriServer.FinestraTemporaleInizio);
        sqlParams.Add("@pivaSuperUser", objParametriServer.PivaSuperUser.Trim());

        if (macCod == 0)
        {
            // Lettura delle Macchine Visibili dall'Impresa

            stbQuery.AppendLine(" AND    ( (Parco_Macchine.Sa_Cod = -1) ");

            var filtroCentri = "";
            if (dtCentriVisibili != null && dtCentriVisibili.Rows.Count > 0)
            {
                for (var i = 0; i < dtCentriVisibili.Rows.Count - 1; i++)
                {
                    filtroCentri += " (Parco_Macchine.Piva = '" + dtCentriVisibili.Rows[i]["piva"] +
                                    "' AND Parco_Macchine.Sa_Cod = " + dtCentriVisibili.Rows[i]["sa_cod"] + ") OR ";
                }

                if (filtroCentri != "")
                {
                    stbQuery.AppendLine(
                        " OR (@filtroCentri OR (Parco_Macchine.Piva = @piva AND Parco_Macchine.Sa_Cod = 0) ) ");
                    sqlParams.TryAdd("@filtroCentri", filtroCentri[..^3]);
                    sqlParams.TryAdd("@piva", piva);
                }
            }

            if (filtroCentri == "")
            {
                stbQuery.AppendLine("          OR  (Parco_Macchine.Piva = @piva)  ");
                sqlParams.TryAdd("@piva", piva);
            }

            stbQuery.AppendLine("        ) ");
        }
        else
        {
            stbQuery.AppendLine(" AND Parco_Macchine.Mac_Cod = @macCod   ");
            sqlParams.TryAdd("@macCod", macCod);
        }

        if (macCodOrigine != 0)
        {
            stbQuery.AppendLine(" AND  Parco_Macchine.Mac_Cod_Origine = @macCodOrigine   ");
            sqlParams.TryAdd("@macCodOrigine", macCodOrigine);
        }

        if (ancheImportati)
        {
            stbQuery.AppendLine(" AND  Parco_Macchine.Mac_Cod_Origine = 0 ");
        }


        switch (objParametriServer.FlagVisibilita)
        {
            case AgronicaCoreParametri.enumVisibilita.visibilita_SoloNonInviati:
                stbQuery.AppendLine(" AND   Parco_Macchine.Inviato = 0 ");
                break;
            case AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati:
                stbQuery.AppendLine(" AND   Parco_Macchine.Inviato =-1 ");
                break;
            case AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti:
                break;
            default:
                throw new Exception("Parametro non corretto nella query (FlagVisibilita)");
        }

        stbQuery.AppendLine(" ORDER BY Parco_Macchine.Piva ASC ");

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
}