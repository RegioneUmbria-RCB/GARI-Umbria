using System.Data;
using System.Text;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MetaSchema.DAL.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.MacchineCaratteristiche;

public class MacchineCaratteristiche : BaseDALMetaschema, IMacchineCaratteristiche
{
    public MacchineCaratteristiche(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
    {
    }

    public async Task<DataTable> LeggiAsync(int macCarCod, string classCode, string filtroAggiuntivo, string orderBy,
        AgronicaCoreParametriServer objParametriServer)
    {
        var stbQuery = new StringBuilder();
        var sqlParams = new Dictionary<string, object>();
        stbQuery.AppendLine(" SELECT MacchinexCaratteristiche.*, Macchine_Caratteristiche.*  ");
        stbQuery.AppendLine(
            " FROM MacchinexCaratteristiche INNER JOIN Macchine_Caratteristiche ON MacchinexCaratteristiche.Mac_Car_Cod = Macchine_Caratteristiche.Mac_Car_Cod ");
        stbQuery.AppendLine(" WHERE 1 = 1");

        if (macCarCod != 0)
        {
            stbQuery.AppendLine(" AND MacchinexCaratteristiche.Mac_Car_Cod = @macCarCod  ");
            sqlParams.TryAdd("@macCarCod", macCarCod);
        }

        if (!string.IsNullOrEmpty(classCode))
        {
            stbQuery.AppendLine(" AND MacchinexCaratteristiche.Class_Code = @classCode ");
            sqlParams.TryAdd("@classCode", classCode);
        }

        if (filtroAggiuntivo != "")
        {
            stbQuery.AppendLine(" AND " + filtroAggiuntivo);
        }


        switch (objParametriServer.FlagVisibilita)
        {
            case AgronicaCoreParametri.enumVisibilita.visibilita_SoloNonInviati:
                stbQuery.AppendLine(" AND   MacchinexCaratteristiche.Inviato = 0 ");
                break;
            case AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati:
                stbQuery.AppendLine(" AND   MacchinexCaratteristiche.Inviato =-1 ");
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