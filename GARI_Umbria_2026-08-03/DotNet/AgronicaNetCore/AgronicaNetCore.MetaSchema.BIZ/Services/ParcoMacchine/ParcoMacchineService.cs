using System.Data;
using System.Text;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MetaSchema.BIZ.Resources;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.ParcoMacchine;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiVisibilitaAppoggio;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.MetaSchema.BIZ.Services.ParcoMacchine;

public class ParcoMacchineService : BaseServiceMetaschemaBIZ, IParcoMacchineService
{
    private readonly IParcoMacchine _parcoMacchineDal;
    private readonly IUtentiVisibilitaAppoggio _utentiVisibilitaAppoggioDal;

    public ParcoMacchineService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
    {
        _parcoMacchineDal = provider.GetRequiredService<IParcoMacchine>();
        _utentiVisibilitaAppoggioDal = provider.GetRequiredService<IUtentiVisibilitaAppoggio>();
    }

    public async Task<DataTable> ParcoMacchine_LeggiAsync(string piva, AgronicaCoreParametriServer objParametriServer,
        AgronicaCoreParametriUtenti objParametriUtenti)
    {
        DataTable dt;
        try
        {
            var visibilityFilter = await GetVisibilityFilterAsync(piva, objParametriServer);
            dt = await _parcoMacchineDal.ParcoMacchine_LeggiAsync(piva, visibilityFilter, "mac_des", objParametriServer);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }

        return dt;
    }

    private async Task<string> GetVisibilityFilterAsync(string piva, AgronicaCoreParametriServer objParametriServer)
    {
        var strSql = new StringBuilder();
        strSql.AppendLine(" ( (Parco_Macchine.Sa_Cod = -1) ");

        var filtroCentri = "";
        var dtCentriVisibili = await _utentiVisibilitaAppoggioDal.ReadAsync((int)TipiEnumerativi.Enum_TipoEntita.Centro, objParametriServer, piva: piva);
        if (dtCentriVisibili?.Rows.Count > 0)
        {
            foreach (DataRow row in dtCentriVisibili.Rows)
            {
                filtroCentri +=
                    $" (Parco_Macchine.Piva = '{row["piva"]}' AND Parco_Macchine.Sa_Cod = {row["sa_cod"]}) OR ";
            }

            if (filtroCentri != "")
            {
                strSql.AppendLine(
                    $" OR ({filtroCentri[..^3]} OR (Parco_Macchine.Piva = '{piva}' AND Parco_Macchine.Sa_Cod = 0) ) ");
            }
        }

        if (filtroCentri == "")
        {
            strSql.AppendLine($"          OR  (Parco_Macchine.Piva = '{piva}')  ");
        }

        strSql.AppendLine("        ) ");
        return strSql.ToString();
    }
}