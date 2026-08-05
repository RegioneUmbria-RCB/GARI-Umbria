using System.Data;
using System.Text;
using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Widgets.DAL.DataLayer.WidgetsIndici;

public class WidgetIndici : DAL_Base, IWidgetIndici
{
    public WidgetIndici(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public async Task<DataTable?> GetWidgetKpiAsync(uint year, string piva, AgronicaCoreParametriServer objParametriServer)
    {
        var stbQuery = new StringBuilder();
        var sqlParams = new Dictionary<string, object>();
        var dataInizio = new DateTime((int)year, 1, 1);

        stbQuery.AppendLine("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;");
        stbQuery.AppendLine("SELECT TOP 1");
        stbQuery.AppendLine("    rai.Validita_Inizio");
        stbQuery.AppendLine("    ,rai.Validita_Fine");
        stbQuery.AppendLine("    ,ai.IndiciProduttivitaAi_COD");

        stbQuery.AppendLine("    ,ai.Produttivita");
        stbQuery.AppendLine("    ,minVal.minProduttivita");
        stbQuery.AppendLine("    ,maxVal.maxProduttivita");
        stbQuery.AppendLine(
            "    ,case when (maxVal.MaxProduttivita =0 and minVal.minProduttivita=0 ) then 0 else ROUND ( ( ( (ai.Produttivita - minVal.minProduttivita) / (maxVal.MaxProduttivita - minVal.minProduttivita) ) * 5) + 1, 0) end AS indiceProduttivitaNN");

        stbQuery.AppendLine("    ,ai.PLV");
        stbQuery.AppendLine("    ,minVal.minPlv");
        stbQuery.AppendLine("    ,maxVal.maxPlv");
        stbQuery.AppendLine(
            "    ,case when (maxVal.maxPlv =0 and minVal.minPlv=0 ) then 0 else ROUND ( ( ( (ai.Plv - minVal.minPlv) / (maxVal.maxPlv - minVal.minPlv) ) * 5) + 1, 0) end AS indicePlvNN");

        stbQuery.AppendLine("    ,ai.IndiceCO2");
        stbQuery.AppendLine("    ,minVal.minIndiceCO2");
        stbQuery.AppendLine("    ,maxVal.maxIndiceCO2");
        stbQuery.AppendLine(
            "    ,case when (maxVal.maxIndiceCO2 =0 and minVal.minIndiceCO2=0 ) then 0 else ROUND ( ( ( (ai.IndiceCO2 - minVal.minIndiceCO2) / (maxVal.maxIndiceCO2 - minVal.minIndiceCO2) ) * 5) + 1, 0) end AS indiceCO2NN");

        stbQuery.AppendLine("    ,ai.IndiceErosione");
        stbQuery.AppendLine("    ,minVal.minIndiceErosione");
        stbQuery.AppendLine("    ,maxVal.maxIndiceErosione");
        stbQuery.AppendLine(
            "    ,case when (maxVal.maxIndiceErosione =0 and minVal.minIndiceErosione=0 ) then 0 else ROUND ( ( ( (ai.IndiceErosione - minVal.minIndiceErosione) / (maxVal.maxIndiceErosione- minVal.minIndiceErosione) ) * 5) + 1, 0) end AS indiceErosioneNN");

        stbQuery.AppendLine("    ,ai.IndiceRischioMeteoAggregato AS IndiceRischioMeteoAggregato");
        stbQuery.AppendLine("    ,minVal.minIndiceRischioMeteoAggregato");
        stbQuery.AppendLine("    ,maxVal.maxIndiceRischioMeteoAggregato");
        stbQuery.AppendLine(
            "    ,case when (maxVal.maxIndiceRischioMeteoAggregato =0 and minVal.minIndiceRischioMeteoAggregato=0 ) then 0 else ROUND ( ai.IndiceRischioMeteoAggregato, 0) end AS indiceRischioMeteoAggregatoNN");

        stbQuery.AppendLine("FROM Reg_Impianti_XIndiciProduttivitaAi rai");
        stbQuery.AppendLine("    INNER JOIN IndiciProduttivitaAi ai");
        stbQuery.AppendLine("        ON ai.IndiciProduttivitaAi_COD = rai.IndiciProduttivitaAi_COD");
        stbQuery.AppendLine("    INNER JOIN (");
        stbQuery.AppendLine("        SELECT TOP 1");
        stbQuery.AppendLine("            Produttivita AS maxProduttivita");
        stbQuery.AppendLine("            ,Plv AS maxPlv");
        stbQuery.AppendLine("            ,IndiceCO2 AS maxIndiceCO2");
        stbQuery.AppendLine("            ,IndiceErosione AS maxIndiceErosione");
        stbQuery.AppendLine("            ,IndiceRischioMeteoAggregato AS maxIndiceRischioMeteoAggregato");

        stbQuery.AppendLine("        FROM Reg_Impianti_XIndiciProduttivitaAi rai");
        stbQuery.AppendLine("            INNER JOIN IndiciProduttivitaAi ai");
        stbQuery.AppendLine("                ON ai.IndiciProduttivitaAi_COD = rai.IndiciProduttivitaAi_COD");
        stbQuery.AppendLine("        WHERE ai.Validita_Inizio = @dataInizio");
        stbQuery.AppendLine("            AND rai.piva = '-32'");
        stbQuery.AppendLine("    ) maxVal");
        stbQuery.AppendLine("        ON 1 = 1");

        stbQuery.AppendLine("    INNER JOIN (");
        stbQuery.AppendLine("        SELECT TOP 1");
        stbQuery.AppendLine("            Produttivita AS minProduttivita");
        stbQuery.AppendLine("            ,Plv AS minPlv");
        stbQuery.AppendLine("            ,IndiceCO2 AS minIndiceCO2");
        stbQuery.AppendLine("            ,IndiceErosione AS minIndiceErosione");
        stbQuery.AppendLine("            ,IndiceRischioMeteoAggregato AS minIndiceRischioMeteoAggregato");

        stbQuery.AppendLine("        FROM Reg_Impianti_XIndiciProduttivitaAi rai");
        stbQuery.AppendLine("            INNER JOIN IndiciProduttivitaAi ai");
        stbQuery.AppendLine("                ON ai.IndiciProduttivitaAi_COD = rai.IndiciProduttivitaAi_COD");
        stbQuery.AppendLine("        WHERE ai.Validita_Inizio = @dataInizio");
        stbQuery.AppendLine("            AND rai.piva = '-31'");
        stbQuery.AppendLine("    ) minVal");
        stbQuery.AppendLine("        ON 1 = 1");

        stbQuery.AppendLine("WHERE rai.piva = @piva");
        stbQuery.AppendLine("    AND ai.validita_Inizio = @dataInizio");
        stbQuery.AppendLine("    AND ai.IndiciProduttivitaAi_COD > 0");
        stbQuery.AppendLine("");

        sqlParams.Add("@dataInizio", dataInizio);
        sqlParams.Add("@piva", piva);

        try
        {
            return await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), sqlParams);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            return null;
        }
    }

    public async Task<DataTable> ReadDatiImpresaIndiceProduttivita(string piva, AgronicaCoreParametriServer objParametriServer)
    {
        var stbQuery = new StringBuilder();
        var sqlParams = new Dictionary<string, object>();

        stbQuery.AppendLine("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;");
        stbQuery.AppendLine("SELECT i.PIVA,");
        stbQuery.AppendLine("    i.rag_soc,");
        stbQuery.AppendLine("    ic.val_cod AS cuaa,");
        stbQuery.AppendLine("    lp.REG AS regioneCod,");
        stbQuery.AppendLine("    lr.Regione_Des AS regioneDes");
        stbQuery.AppendLine("FROM Imprese i");
        stbQuery.AppendLine("    LEFT JOIN Imprese_Codici ic ON ic.PIVA = i.PIVA");
        stbQuery.AppendLine("        AND ic.id_cod = @idCod");
        stbQuery.AppendLine("    INNER JOIN ImpresexIndirizzi ixi ON ixi.PIVA = i.PIVA");
        stbQuery.AppendLine("    INNER JOIN Indirizzi ind ON ind.cod_indirizzo = ixi.cod_indirizzo");
        stbQuery.AppendLine("    INNER JOIN Lista_Province lp ON lp.PROV = ind.pro_cod_istat");
        stbQuery.AppendLine("    INNER JOIN Lista_Regioni lr ON lr.REG = lp.REG");
        stbQuery.AppendLine("WHERE i.PIVA = @piva");
        stbQuery.AppendLine("");

        sqlParams.Add("@idCod", TipiEnumerativi.Enum_CodiciAnagrafe.CodiceCUAA);
        sqlParams.Add("@piva", piva);

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

    public async Task<DataTable> ReadSpecificIndiciProduttivitaAsync(string piva, int year, int? vegCod,
        AgronicaCoreParametriServer objParametriServer)
    {
        var stbQuery = new StringBuilder();
        var sqlParams = new Dictionary<string, object>();

        var dataInizio = new DateTime(year, 1, 1);
        var dataFine = new DateTime(year, 12, 31);

        stbQuery.AppendLine("");
        stbQuery.AppendLine("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;");
        stbQuery.AppendLine("SELECT TOP 1 ");
        stbQuery.AppendLine("      0 as Cul_Cod");
        stbQuery.AppendLine("    , '' as Cul_Des");
        stbQuery.AppendLine("    , 0 as Veg_Cod");
        stbQuery.AppendLine("    , '' as Veg_Des");
        stbQuery.AppendLine("    ,i.Sup_Imp");
        stbQuery.AppendLine("    ,ind.IndiciProduttivitaAi_Cod");
        stbQuery.AppendLine("    ,ind.Produttivita");
        stbQuery.AppendLine("    ,ind.PLV");
        stbQuery.AppendLine("    ,ind.IndiceErosione");
        stbQuery.AppendLine("    ,ind.IndiceCO2");
        stbQuery.AppendLine("    ,ind.IndiceRischioMeteoAggregato as IndiceRischioMeteoAggregato");
        stbQuery.AppendLine("    ,ind.IndiceRischioGelata as IndiceRischioGelata");
        stbQuery.AppendLine("    ,ind.IndiceRischioVentoForte as IndiceRischioVentoForte");
        stbQuery.AppendLine("    ,ind.IndiceRischioSiccita as IndiceRischioSiccita");
        stbQuery.AppendLine("    ,ind.IndiceRischioGrandine as IndiceRischioGrandine");
        stbQuery.AppendLine("    ,ind.IndiceRischioAllagamento as IndiceRischioAllagamento");
        stbQuery.AppendLine("FROM IndiciProduttivitaAi AS ind");
        stbQuery.AppendLine(
            "    INNER JOIN Reg_Impianti_XIndiciProduttivitaAi ixi ON ixi.IndiciProduttivitaAi_COD = ind.IndiciProduttivitaAi_COD");
        stbQuery.AppendLine("    INNER JOIN Reg_Impianti i ON i.PIVA = ixi.piva");
        stbQuery.AppendLine("        AND i.APPEZZA = ixi.appezza");
        stbQuery.AppendLine("        AND i.SA_COD= ixi.sa_cod");
        stbQuery.AppendLine("        AND i.ID_REG = ixi.id_reg");
        stbQuery.AppendLine("WHERE ind.Validita_Inizio <= @dataFine");
        stbQuery.AppendLine("    AND ind.Validita_Fine >= @dataInizio");
        stbQuery.AppendLine("    AND i.PIVA = @piva");
        stbQuery.AppendLine("");

        sqlParams.Add("@dataInizio", dataInizio);
        sqlParams.Add("@dataFine", dataFine);
        sqlParams.Add("@piva", piva);

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

    public async Task<DataTable> ReadGeneralIndiciProduttivitaAsync(string piva, int year,
        AgronicaCoreParametriServer objParametriServer)
    {
        var stbQuery = new StringBuilder();
        var sqlParams = new Dictionary<string, object>();

        var dataInizio = new DateTime(year, 1, 1);
        var dataFine = new DateTime(year, 12, 31);

        stbQuery.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;");
        stbQuery.AppendLine("SELECT");
        stbQuery.AppendLine("    -8 as IndiciProduttivitaAi_COD ");
        stbQuery.AppendLine("    , ai.reg_Produttivita as Produttivita");
        stbQuery.AppendLine("    , ai.reg_PLV as PLV");
        stbQuery.AppendLine("    , ai.reg_IndiceErosione as IndiceErosione");
        stbQuery.AppendLine("    , ai.reg_IndiceCO2 as IndiceCO2");
        stbQuery.AppendLine("    , ai.reg_IndiceRischioMeteoAggregato as IndiceRischioMeteoAggregato");
        stbQuery.AppendLine("    , ai.reg_IndiceRischioGelata as IndiceRischioGelata");
        stbQuery.AppendLine("    , ai.reg_IndiceRischioVentoForte as IndiceRischioVentoForte");
        stbQuery.AppendLine("    , ai.reg_IndiceRischioSiccita as IndiceRischioSiccita");
        stbQuery.AppendLine("    , ai.reg_IndiceRischioGrandine as IndiceRischioGrandine");
        stbQuery.AppendLine("    , ai.reg_IndiceRischioAllagamento as IndiceRischioAllagamento");
        stbQuery.AppendLine("FROM Reg_Impianti_XIndiciProduttivitaAi ixi");
        stbQuery.AppendLine(
            "    INNER JOIN  IndiciProduttivitaAi ai ON ixi.IndiciProduttivitaAi_COD = ai.IndiciProduttivitaAi_COD");
        stbQuery.AppendLine("        AND (ixi.piva = @piva)");
        stbQuery.AppendLine("WHERE ai.Validita_Inizio <= @dataFine");
        stbQuery.AppendLine("    AND ai.Validita_Fine >= @dataFine");

        stbQuery.AppendLine("UNION ALL");

        stbQuery.AppendLine("SELECT");
        stbQuery.AppendLine("    -30 as IndiciProduttivitaAi_COD ");
        stbQuery.AppendLine("    , ai.port_Produttivita as Produttivita");
        stbQuery.AppendLine("    , ai.port_PLV as PLV");
        stbQuery.AppendLine("    , ai.port_IndiceErosione as IndiceErosione");
        stbQuery.AppendLine("    , ai.port_IndiceCO2 as IndiceCO2");
        stbQuery.AppendLine("    , ai.port_IndiceRischioMeteoAggregato as IndiceRischioMeteoAggregato");
        stbQuery.AppendLine("    , ai.port_IndiceRischioGelata as IndiceRischioGelata");
        stbQuery.AppendLine("    , ai.port_IndiceRischioVentoForte as IndiceRischioVentoForte");
        stbQuery.AppendLine("    , ai.port_IndiceRischioSiccita as IndiceRischioSiccita");
        stbQuery.AppendLine("    , ai.port_IndiceRischioGrandine as IndiceRischioGrandine");
        stbQuery.AppendLine("    , ai.port_IndiceRischioAllagamento as IndiceRischioAllagamento");
        stbQuery.AppendLine("FROM Reg_Impianti_XIndiciProduttivitaAi ixi");
        stbQuery.AppendLine(
            "    INNER JOIN  IndiciProduttivitaAi ai ON ixi.IndiciProduttivitaAi_COD = ai.IndiciProduttivitaAi_COD");
        stbQuery.AppendLine("        AND (ixi.piva = @piva)");
        stbQuery.AppendLine("WHERE ai.Validita_Inizio <= @dataFine");
        stbQuery.AppendLine("    AND ai.Validita_Fine >= @dataFine");

        sqlParams.Add("@dataInizio", dataInizio);
        sqlParams.Add("@dataFine", dataFine);
        sqlParams.Add("@piva", piva);

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

    public async Task<DataTable> ReadAvailableYearsAsync(string piva, AgronicaCoreParametriServer objParametriServer)
    {
        var stbQuery = new StringBuilder();
        var sqlParams = new Dictionary<string, object>();

        stbQuery.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;");
        stbQuery.AppendLine("SELECT DISTINCT");
        stbQuery.AppendLine("    i.Validita_Inizio");
        stbQuery.AppendLine("    ,i.Validita_Fine");
        stbQuery.AppendLine("FROM IndiciProduttivitaAi i");
        stbQuery.AppendLine(
            "    INNER JOIN Reg_Impianti_XIndiciProduttivitaAi ixi ON ixi.IndiciProduttivitaAi_COD = i.IndiciProduttivitaAi_COD");
        stbQuery.AppendLine("WHERE i.IndiciProduttivitaAi_COD > 0 ");
        stbQuery.AppendLine("    AND ixi.piva = @piva");
        stbQuery.AppendLine("");

        sqlParams.Add("@piva", piva);

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