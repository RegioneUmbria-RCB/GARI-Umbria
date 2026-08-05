using System.Data;
using System.Text;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MetaSchema.BIZ.Resources;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.Contatti;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiVisibilitaAppoggio;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.MetaSchema.BIZ.Services.Contatti;

public class ContattiService : BaseServiceMetaschemaBIZ, IContattiService
{
    private readonly IContatti _contattiDal;
    private readonly IUtentiVisibilitaAppoggio _utentiVisibilitaAppoggioDal;

    public ContattiService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
    {
        _contattiDal = provider.GetRequiredService<IContatti>();
        _utentiVisibilitaAppoggioDal = provider.GetRequiredService<IUtentiVisibilitaAppoggio>();
    }

    public async Task<DataTable> Contatti_LeggiAsync(string piva, AgronicaCoreParametriServer objParametriServer,
        AgronicaCoreParametriUtenti objParametriUtenti)
    {
        DataTable dt;
        try
        {
            var visibilityFilter = await GetVisibilityFilterAsync(piva, objParametriServer);
            dt = await _contattiDal.LeggiAsync(visibilityFilter, objParametriServer);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }

        return dt;
    }

    private async Task<string> GetVisibilityFilterAsync(string? piva, AgronicaCoreParametriServer objParametriServer)
    {
        if (string.IsNullOrEmpty(piva))
        {
            return " AND    (C.Sa_Cod = -1)  ";
        }

        var strSql = new StringBuilder();
        strSql.AppendLine(" AND    ( (C.Sa_Cod = -1) ");

        var filtroCentri = "";
        var dtCentriVisibili = await _utentiVisibilitaAppoggioDal.ReadAsync((int)TipiEnumerativi.Enum_TipoEntita.Centro, objParametriServer, piva: piva);
        if (dtCentriVisibili?.Rows.Count > 0)
        {
            foreach (DataRow row in dtCentriVisibili.Rows)
            {
                filtroCentri +=
                    $" (C.Piva = '{row["piva"]}' AND C.Sa_Cod = {row["sa_cod"]}) OR ";
            }

            if (filtroCentri != "")
            {
                strSql.AppendLine(
                    $" OR ({filtroCentri[..^3]} OR (C.Piva = '{piva}' AND C.Sa_Cod = 0) ) ");
            }
        }

        if (filtroCentri == "")
        {
            strSql.AppendLine($"          OR  (C.Piva = '{piva}')  ");
        }

        strSql.AppendLine("        ) ");
        return strSql.ToString();
    }
}