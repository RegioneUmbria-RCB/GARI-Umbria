using AgronicaNetCore.Base.Models;
using AgronicaNetCore.OperazioniZoo.DAL.DataLayer.PesateAutomaticheDataMars.Models;
using AgronicaNetCore.OperazioniZoo.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Text;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.OperazioniZoo.DAL.DataLayer.PesateAutomaticheDataMars.StallaMappingDatamars;

/// <summary>
/// Implementazione del repository per il mapping FarmID Datamars → stalla GIAS.
/// <para>
/// Il mapping sfrutta la join tra <c>Stalla</c> e <c>Fabbricati_codici</c>
/// (id_cod=1366, val_cod=FarmID) sulla chiave <c>STA_NUM = fabbricato_cod</c>.
/// </para>
/// <para>Riferimento spec: DS05-BL ValidazioneAnagraficaAnimaliMappingStalla — Persistenze Coinvolte,
/// Configurazione Mapping FarmID-Stalla (SELECT).</para>
/// </summary>
public sealed class StallaMappingDatamarsRepository : BaseDALOperazioniZoo, IStallaMappingDatamarsRepository
{
   
    public StallaMappingDatamarsRepository(IServiceProvider provider, IStringLocalizer<Messages> localizer)
        : base(provider, localizer)
    {
    }

    /// <inheritdoc/>
    public async Task<StallaInfo?> GetStallaInfoByFarmIdAsync(
        string farmId,
        AgronicaCoreParametriServer objParametriServer)
    {
        var stbQuery = new StringBuilder();
        var parSql = new Dictionary<string, object>();
        Dictionary<string, Dictionary<Type, List<object>>> parSqlIn = new();

        stbQuery.AppendLine("SELECT TOP 1 s.Piva, s.sa_cod, s.STA_NUM");
        stbQuery.AppendLine("FROM Stalla s (NOLOCK)");
        stbQuery.AppendLine("    INNER JOIN Fabbricati_codici fc (NOLOCK)");
        stbQuery.AppendLine("        ON s.Piva = fc.Piva");
        stbQuery.AppendLine("        AND s.sa_cod = fc.sa_cod");
        stbQuery.AppendLine("        AND s.STA_NUM = fc.fabbricato_cod");
        stbQuery.AppendLine("WHERE fc.id_cod = @idCod");
        stbQuery.AppendLine("    AND fc.val_cod = @farmId");
        parSql.Add("@idCod", (int) Enum_CodiciAnagrafe.Farm_ID);
        parSql.Add("@farmId", farmId);

        try
        {
            var dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parSql, parSqlIn);

            if (dt.Rows.Count == 0)
                return null;

            var row = dt.Rows[0];
            return new StallaInfo
            {
                Piva = row["Piva"]?.ToString() ?? string.Empty,
                SaCod = Convert.ToInt32(row["sa_cod"]),
                StaNum = Convert.ToInt32(row["STA_NUM"])
            };
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
    }
}
