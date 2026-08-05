using System.Data;
using System.Text;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MetaSchema.DAL.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.GruppiOperazione;

public class GruppiOperazione : BaseDALMetaschema, IGruppiOperazione
{
    public GruppiOperazione(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
    {
    }

    public async Task<DataTable> GruppiOperazione_LeggiAsync(AgronicaCoreParametriServer objParametriServer)
    {
        var stbQuery = new StringBuilder();
        var sqlParams = new Dictionary<string, object>();

        stbQuery.AppendLine(" SELECT * ");
        stbQuery.AppendLine(" FROM  GruppoOperazioni ");
        stbQuery.AppendLine(" WHERE Validita_inizio < @dtFine ");
        stbQuery.AppendLine(" AND   Validita_Fine > @dtInizio");
        stbQuery.AppendLine(" ORDER BY Tipo ");

        sqlParams.Add("@dtFine", objParametriServer.FinestraTemporaleFine);
        sqlParams.Add("@dtInizio", objParametriServer.FinestraTemporaleInizio);

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