using System.Data;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MetaSchema.BIZ.Resources;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.GruppiOperazione;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.MetaSchema.BIZ.Services.GruppiOperazione;

public class GruppiOperazioneService : BaseServiceMetaschemaBIZ, IGruppiOperazioneService
{
    private readonly IGruppiOperazione _gruppiOperazioneDal;

    public GruppiOperazioneService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
    {
        _gruppiOperazioneDal = provider.GetRequiredService<IGruppiOperazione>();
    }

    public async Task<DataTable> GruppiOperazione_LeggiAsync(AgronicaCoreParametriServer objParametriServer)
    {
        DataTable dt;
        try
        {
            dt = await _gruppiOperazioneDal.GruppiOperazione_LeggiAsync(objParametriServer);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }

        return dt;
    }
}