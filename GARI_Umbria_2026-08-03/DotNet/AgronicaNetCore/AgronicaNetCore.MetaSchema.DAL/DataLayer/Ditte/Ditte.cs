using System.Data;
using System.Text;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MetaSchema.DAL.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.Ditte;

public class Ditte : BaseDALMetaschema, IDitte
{
    public Ditte(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
    {
    }

    public async Task<DataTable> LeggiAsync(int dittaCod, string tipo, string filtroAggiuntivo, string orderBy,
        AgronicaCoreParametriServer objParametriServer)
    {
        var stbQuery = new StringBuilder();
        var sqlParams = new Dictionary<string, object>();
        stbQuery.AppendLine(" SELECT * ");
        stbQuery.AppendLine(" FROM  Ditte ");
        stbQuery.AppendLine(" WHERE Validita_Inizio < @dtFine ");
        stbQuery.AppendLine(" AND   Validita_Fine > @dtInizio ");
        
        sqlParams.Add("@dtFine", objParametriServer.FinestraTemporaleFine);
        sqlParams.Add("@dtInizio", objParametriServer.FinestraTemporaleInizio);

        if (dittaCod != 0)
        {
            stbQuery.AppendLine(" AND Ditta_Cod = @dittaCod  ");
            sqlParams.TryAdd("@dittaCod", dittaCod);
        }

        if (!string.IsNullOrEmpty(tipo))
        {
            stbQuery.AppendLine(" AND Tipo = @tipo ");
            sqlParams.TryAdd("@tipo", tipo);
        }

        if (filtroAggiuntivo != "")
        {
            stbQuery.AppendLine(" AND " + filtroAggiuntivo);
        }


        switch (objParametriServer.FlagVisibilita)
        {
            case AgronicaCoreParametri.enumVisibilita.visibilita_SoloNonInviati:
                stbQuery.AppendLine(" AND   Inviato = 0 ");
                break;
            case AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati:
                stbQuery.AppendLine(" AND   Inviato =-1 ");
                break;
            case AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti:
                break;
            default:
                throw new Exception("Parametro non corretto nella query (FlagVisibilita)");
        }

        if (!string.IsNullOrEmpty(orderBy))
        {
            stbQuery.AppendLine(" ORDER BY " + orderBy);
        }
        else
        {
            stbQuery.AppendLine(" ORDER BY Ditta_Des");
        }

        try
        {
            return await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), sqlParams);
        }
        catch
            (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
    }
}