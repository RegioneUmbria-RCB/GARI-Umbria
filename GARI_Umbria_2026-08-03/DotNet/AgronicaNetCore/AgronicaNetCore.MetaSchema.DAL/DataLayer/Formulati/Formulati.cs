using System.Data;
using System.Text;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MetaSchema.DAL.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.Formulati;

public class Formulati : BaseDALMetaschema, IFormulati
{
    public Formulati(IServiceProvider provider, IStringLocalizer<Messages> localizer)
        : base(provider, localizer) { }

    public async Task<DataTable> LeggiAsync(
        int tipoRichiesto,
        string statoCod,
        List<int> vegCodList,
        AgronicaCoreParametriServer objParametriServer,
        bool leggiAPP = false
    )
    {
        var sqlParams = new Dictionary<string, object>();
        Dictionary<string, Dictionary<Type, List<object>>> parSqlIn = new();
        var stb = new StringBuilder();

        stb.AppendLine(" SELECT DISTINCT ");

        if (leggiAPP)
        {
            stb.AppendLine(" Formulati.Fr_Cod,");
            stb.AppendLine(" Formulati.Fr_Des,");
        }
        else
        {
            stb.AppendLine(" Formulati.*,");
            stb.AppendLine(" FormulatiXPeriodoSospensione.DataSospensioneDA,");
            stb.AppendLine(" FormulatiXPeriodoSospensione.DataSospensioneA,");
            stb.AppendLine(" ISNULL(udm_a.UDM_COD, 0) AS Udm_Cod_A,");
            stb.AppendLine(" ISNULL(udm_i.UDM_COD, 0) AS Udm_Cod_I,");
        }

        // IsTrappolaFormulato: solo se tipo è numerico (filtra per CLASS_COD trappole)
        if (tipoRichiesto != 0)
        {
            stb.AppendLine("        CASE");
            stb.AppendLine(
                "            WHEN FormulatixClassificazioni.CLASS_COD IN (602, 613, 617) THEN 'true'"
            );
            stb.AppendLine("            ELSE 'false'");
            stb.AppendLine("        END AS IsTrappolaFormulato");
        }
        else
        {
            stb.AppendLine("        'false' AS IsTrappolaFormulato");
        }

        stb.AppendLine(" FROM Formulati");

        if (!string.IsNullOrEmpty(statoCod))
        {
            stb.AppendLine(" INNER JOIN FormulatixAmbitoEstero");
            stb.AppendLine("     ON Formulati.Fr_Cod = FormulatixAmbitoEstero.Fr_Cod");
            stb.AppendLine("     AND FormulatixAmbitoEstero.Stato_Cod = @statoCod");
            sqlParams.Add("@statoCod", statoCod);
        }

        if (!leggiAPP)
        {
            stb.AppendLine(" LEFT JOIN FormulatiXPeriodoSospensione");
            stb.AppendLine("     ON Formulati.Fr_Cod = FormulatiXPeriodoSospensione.Fr_Cod");

            // Udm_Cod_A: prima UDM valida tra le dosi su avversità
            stb.AppendLine(" OUTER APPLY (");
            stb.AppendLine("     SELECT TOP 1 d.UDM_COD");
            stb.AppendLine("     FROM FormulatixSpeciexAvversita a");
            stb.AppendLine(
                "     INNER JOIN FormulatixSpeciexAvversitaxDosi d ON a.For_Veg_Av_Cod = d.For_Veg_Av_Cod"
            );
            stb.AppendLine("     INNER JOIN UnitaMisura u ON d.UDM_COD = u.UDM_COD");
            stb.AppendLine("     WHERE a.Fr_Cod = Formulati.Fr_Cod");
            stb.AppendLine(" ) AS udm_a");

            // Udm_Cod_I: prima UDM valida tra le dosi su infestanti
            stb.AppendLine(" OUTER APPLY (");
            stb.AppendLine("     SELECT TOP 1 d.UDM_COD");
            stb.AppendLine("     FROM FormulatixSpeciexInfestanti i");
            stb.AppendLine(
                "     INNER JOIN FormulatixSpeciexInfestantixDosi d ON i.For_Veg_Av_Cod = d.For_Veg_Av_Cod"
            );
            stb.AppendLine("     INNER JOIN UnitaMisura u ON d.UDM_COD = u.UDM_COD");
            stb.AppendLine("     WHERE i.Fr_Cod = Formulati.Fr_Cod");
            stb.AppendLine(" ) AS udm_i");
        }

        if (tipoRichiesto != 0)
        {
            stb.AppendLine(" INNER JOIN FormulatixClassificazioni");
            stb.AppendLine("     ON Formulati.Fr_Cod = FormulatixClassificazioni.For_Cod");
            sqlParams.Add("@tipo", tipoRichiesto);
        }

        if (vegCodList?.Count > 0)
        {
            stb.AppendLine(" INNER JOIN FormulatixSpecieVegetalixNormative");
            stb.AppendLine("     ON Formulati.Fr_Cod = FormulatixSpecieVegetalixNormative.Fr_Cod");
        }

        stb.AppendLine(" WHERE 1 = 1");

        if (vegCodList?.Count > 0)
        {
            stb.AppendLine("   AND (");
            stb.AppendLine("         FormulatixSpecieVegetalixNormative.Veg_Cod IN (@vegCodList)");
            // Geodisinfestanti: terreno senza coltura (codice fisso 5000336)
            stb.AppendLine("      OR FormulatixSpecieVegetalixNormative.Veg_Cod = 5000336");
            stb.AppendLine("      OR FormulatixSpecieVegetalixNormative.Grsp_Cod IN (");
            stb.AppendLine("             SELECT Grsp_Cod FROM GruppoColturaleXSpecieVegetali");
            stb.AppendLine(
                "             WHERE GruppoColturaleXSpecieVegetali.Veg_Cod IN (@vegCodList)"
            );
            stb.AppendLine("         )");
            stb.AppendLine("       )");
            parSqlIn.Add("@vegCodList", FormatClauseIn(vegCodList));
        }

        stb.AppendLine(" ORDER BY Fr_Des");

        try
        {
            return await GetDataProvider(objParametriServer)
                .ExecuteReadAsync(stb.ToString(), sqlParams, parSqlIn);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
    }
}
