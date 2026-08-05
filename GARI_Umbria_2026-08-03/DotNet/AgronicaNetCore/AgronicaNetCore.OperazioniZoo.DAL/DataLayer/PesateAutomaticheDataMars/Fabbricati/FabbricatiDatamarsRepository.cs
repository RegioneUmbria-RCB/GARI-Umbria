using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.OperazioniZoo.DAL.DataLayer;
using AgronicaNetCore.OperazioniZoo.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Text;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.OperazioniZoo.DAL.DataLayer.PesateAutomaticheDataMars.Fabbricati;

/// <summary>
/// Implementazione del repository per la lettura dei FarmID Datamars.
/// Esegue la query di sistema sui fabbricati con codice identificativo <c>id_cod = 1366</c>
/// e tipo <c>tipo_fabbricato_cod = 178</c>.
/// <para>Riferimento spec: DS01-BL AcquisizionePesateDatamarsAPI — Step 1 Estrazione FarmID,
/// query SQL specificata nella sezione "Integrazione DataProvider".</para>
/// </summary>
public sealed class FabbricatiDatamarsRepository : BaseDALOperazioniZoo, IFabbricatiDatamarsRepository
{
    public FabbricatiDatamarsRepository(IServiceProvider provider, IStringLocalizer<Messages> localizer)
        : base(provider, localizer)
    {
    }

    /// <inheritdoc/>
    public async Task<List<string>> GetFarmIdsAsync(AgronicaCoreParametriServer objParametriServer)
    {
        var stbQuery = new StringBuilder();
        var parSql = new Dictionary<string, object>();
        Dictionary<string, Dictionary<Type, List<object>>> parSqlIn = new();

        stbQuery.AppendLine("SELECT Val_cod AS FARMID");
        stbQuery.AppendLine("FROM Fabbricati f");
        stbQuery.AppendLine("    INNER JOIN Fabbricati_codici fc");
        stbQuery.AppendLine("        ON f.Piva = fc.Piva");
        stbQuery.AppendLine("        AND f.sa_cod = fc.sa_cod");
        stbQuery.AppendLine("        AND f.fabbricato_cod = fc.fabbricato_cod");
        stbQuery.AppendLine("WHERE fc.id_cod = @idCod");
        stbQuery.AppendLine("    AND f.tipo_fabbricato_cod = @tipoFabbricatoCod");
        parSql.Add("@idCod", (int) Enum_CodiciAnagrafe.Farm_ID);
        parSql.Add("@tipoFabbricatoCod", TIPO_FABBRICATI.TIPO_FABBRICATO_STALLA);

        try
        {
            var dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parSql, parSqlIn);
            return dt.AsEnumerable()
                .Select(row => row["FARMID"]?.ToString() ?? string.Empty)
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .ToList();
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
    }
}
