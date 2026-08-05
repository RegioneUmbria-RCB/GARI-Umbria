using AgronicaNetCore.Base.Models;
using AgronicaNetCore.OperazioniZoo.DAL.DataLayer.PesateAutomaticheDataMars.Models;
using AgronicaNetCore.OperazioniZoo.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Text;

namespace AgronicaNetCore.OperazioniZoo.DAL.DataLayer.PesateAutomaticheDataMars.ZooAnimaliDatamars;

/// <summary>
/// Implementazione del repository per il lookup anagrafica animali su ZOO_ANIMALI.
/// <para>Riferimento spec: DS05-BL ValidazioneAnagraficaAnimaliMappingStalla — Persistenze Coinvolte,
/// ZOO_ANIMALI (SELECT); DS02-BL Step 1 Lookup Anagrafica.</para>
/// </summary>
public sealed class ZooAnimaliDatamarsRepository : BaseDALOperazioniZoo, IZooAnimaliDatamarsRepository
{
    public ZooAnimaliDatamarsRepository(IServiceProvider provider, IStringLocalizer<Messages> localizer)
        : base(provider, localizer)
    {
    }

    /// <inheritdoc/>
    public async Task<AnimaleInfo?> GetAnimaleByMatricolaAsync(
        string matricola,
        string piva,
        AgronicaCoreParametriServer objParametriServer)
    {
        var stbQuery = new StringBuilder();
        var parSql = new Dictionary<string, object>();
        Dictionary<string, Dictionary<Type, List<object>>> parSqlIn = new();

        stbQuery.AppendLine("SELECT TOP 1 Cod_Progetto, Validita_Inizio, Validita_Fine");
        stbQuery.AppendLine("FROM ZOO_ANIMALI (NOLOCK)");
        stbQuery.AppendLine("WHERE Matricola = @matricola");
        stbQuery.AppendLine("    AND Piva = @piva");
        parSql.Add("@matricola", matricola);
        parSql.Add("@piva", piva);

        try
        {
            var dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parSql, parSqlIn);

            if (dt.Rows.Count == 0)
                return null;

            var row = dt.Rows[0];
            return new AnimaleInfo
            {
                CodProgetto = Convert.ToInt32(row["Cod_Progetto"]),
                ValiditaInizio = Convert.ToDateTime(row["Validita_Inizio"]),
                ValiditaFine = Convert.ToDateTime(row["Validita_Fine"])
            };
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
    }
}
