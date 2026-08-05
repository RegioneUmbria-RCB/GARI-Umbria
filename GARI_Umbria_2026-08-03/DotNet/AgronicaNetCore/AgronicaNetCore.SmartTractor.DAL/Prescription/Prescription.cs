using System.Data;
using System.Text;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SmartTractor.DAL.DataLayer;
using AgronicaNetCore.SmartTractor.DAL.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.SmartTractor.DAL.Attivita;

/// <summary>
/// Data Access Layer for reading activity data from legacy Ricette tables.
/// Source tables: RICETTE_DETTAGLI, RICETTE_DESTINAZIONI.
///
/// Method Conventions:
/// - SELECT methods use ExecuteReadAsync() with Dictionary&lt;string, object&gt; parameters.
///
/// Referenced in Design Specification: DS03-BLb - Lettura Dati AttivitÃ  (FASE 4, FASE 5)
/// </summary>
public class Prescription : BaseDALSmartTractor, IPrescription
{
    public Prescription(IServiceProvider provider, IStringLocalizer<Messages> localizer)
        : base(provider, localizer)
    {
    }

    public async Task<DataTable> GetPrescriptionTypeAsync(int ricettaOperazioneCod, AgronicaCoreParametriServer serverParams)
    {
        var stbQuery = new StringBuilder();
        var parSql = new Dictionary<string, object>();

        stbQuery.AppendLine("SELECT ");
        stbQuery.AppendLine("    Ricetta_SuperUser");
        stbQuery.AppendLine("   , Ricetta_Operazione_Cod");
        stbQuery.AppendLine("   , Lav_Cod");
        stbQuery.AppendLine("   , Validita_Inizio");
        stbQuery.AppendLine("   , Validita_Fine");
        stbQuery.AppendLine("   , W_Anagrafica_Stati_Cod");
        stbQuery.AppendLine("FROM Ricette_Operazioni");
        stbQuery.AppendLine("WHERE 1 = 1");
        stbQuery.AppendLine("    AND Ricetta_Operazione_Cod = @ricettaOperazioneCod");
        stbQuery.AppendLine("   AND Ricetta_SuperUser = @pivaSuperUser");
        //stbQuery.AppendLine(" AND W_Anagrafica_Stati_Cod = 300");

        parSql.Add("@ricettaOperazioneCod", ricettaOperazioneCod);
        parSql.Add("@pivaSuperUser", serverParams.PivaSuperUser);

        try
        {
            return await GetDataProvider(serverParams).ExecuteReadAsync(stbQuery.ToString(), parSql);
        }
        catch (Exception ex)
        {
            LogError($"Error reading Ricette_Operazioni for Ricetta_Operazione_Cod={ricettaOperazioneCod}", serverParams, ex);
            throw;
        }
    }

    public async Task<DataTable> GetPrescriptionDetailsAsync(int ricettaOperazioneCod, int? elemCod, AgronicaCoreParametriServer serverParams)
    {
        var stbQuery = new StringBuilder();
        var parSql = new Dictionary< string, object>();

        stbQuery.AppendLine("SELECT");
        stbQuery.AppendLine("    Ricetta_SuperUser");
        stbQuery.AppendLine("    , Ricetta_Operazione_Cod");
        stbQuery.AppendLine("    , Ricetta_Dettaglio_Cod");
        stbQuery.AppendLine("    , Elem_Cod");
        stbQuery.AppendLine("    , Mat_Cod");
        stbQuery.AppendLine("    , Pro_Cod");
        stbQuery.AppendLine("    , Udm_Cod");
        stbQuery.AppendLine("    , Qta");
        stbQuery.AppendLine("    , Extra_Int");
        stbQuery.AppendLine("FROM Ricette_Dettagli");
        stbQuery.AppendLine("WHERE 1 = 1");
        stbQuery.AppendLine("    AND Ricetta_Operazione_Cod = @ricettaOperazioneCod");
        stbQuery.AppendLine("    AND Ricetta_SuperUser = @pivaSuperUser");

        if (elemCod != null)
        {
            stbQuery.AppendLine("    AND Elem_Cod = @elemCod");
            parSql.Add("@elemCod", elemCod);
        }

        parSql.Add("@ricettaOperazioneCod", ricettaOperazioneCod);
        parSql.Add("@pivaSuperUser", serverParams.PivaSuperUser);

        try
        {
            return await GetDataProvider(serverParams).ExecuteReadAsync(stbQuery.ToString(), parSql);
        }
        catch (Exception ex)
        {
            LogError($"Error reading Ricette_Dettagli for Ricetta_Operazione_Cod={ricettaOperazioneCod} and Ricetta_SuperUser={serverParams.PivaSuperUser}", serverParams, ex);
            throw;
        }
    }

    public async Task<DataTable> GetPrescriptionDestinationsAsync(int ricettaOperazioneCod, int? tipoDestinazione, AgronicaCoreParametriServer serverParams)
    {
        var stbQuery = new StringBuilder();
        var parSql = new Dictionary<string, object>();

        stbQuery.AppendLine("SELECT");
        stbQuery.AppendLine("    Ricetta_SuperUser");
        stbQuery.AppendLine("    , Ricetta_Operazione_Cod");
        stbQuery.AppendLine("    , Ricetta_Destinazione_Cod");
        stbQuery.AppendLine("    , Piva");
        stbQuery.AppendLine("    , Sa_Cod");
        stbQuery.AppendLine("    , Appezza");
        stbQuery.AppendLine("    , Id_Reg");
        stbQuery.AppendLine("    , Qta");
        stbQuery.AppendLine("    , Tipo_Destinazione");
        stbQuery.AppendLine("FROM Ricette_Destinazioni");
        stbQuery.AppendLine("WHERE 1 = 1");
        stbQuery.AppendLine("    AND Ricetta_Operazione_Cod = @ricettaOperazioneCod");
        stbQuery.AppendLine("    AND Ricetta_SuperUser = @pivaSuperUser");

        if (tipoDestinazione != null)
        {
            stbQuery.AppendLine("    AND Tipo_Destinazione = @tipoDest");
            parSql.Add("@tipoDest", tipoDestinazione);
        }

        parSql.Add("@ricettaOperazioneCod", ricettaOperazioneCod);
        parSql.Add("@pivaSuperUser", serverParams.PivaSuperUser);

        try
        {
            return await GetDataProvider(serverParams).ExecuteReadAsync(stbQuery.ToString(), parSql);
        }
        catch (Exception ex)
        {
            LogError($"Error reading Ricette_Dettagli for Ricetta_Operazione_Cod={ricettaOperazioneCod} and Ricetta_SuperUser={serverParams.PivaSuperUser}", serverParams, ex);
            throw;
        }
    }
}
