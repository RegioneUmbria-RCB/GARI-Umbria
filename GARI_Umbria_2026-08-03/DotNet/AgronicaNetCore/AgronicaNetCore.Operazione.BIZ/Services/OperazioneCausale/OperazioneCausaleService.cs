using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Operazione.BIZ.Resources;
using AgronicaNetCore.Operazione.BIZ.Services;
using AgronicaNetCore.Operazione.DAL.DataLayer.OperazioneCausale;
using AgronicaNetCore.UtilityDB.DAL.DataLayer.Agro_Sequenze;
using InData.Operazione;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Data;

namespace AgronicaNetCore.Operazione.BIZ.Services.OperazioneCausale;

public class OperazioneCausaleService : BaseServiceOperazioneBIZ, IOperazioneCausaleService
{
    private readonly IOperazioneCausale _operazioneCausaleDal;
    private readonly IAgro_Sequence _sequenceDal;

    public OperazioneCausaleService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
    {
        _operazioneCausaleDal = provider.GetRequiredService<IOperazioneCausale>();
        _sequenceDal = provider.GetRequiredService<IAgro_Sequence>();
    }

    public async Task<DataTable> OperazioneCausale_LeggiAsync(LeggiOperazione leggiOperazione, AgronicaCoreParametriServer objParametriServer)
    {
        DataTable dt;
        try
        {
            dt = await _operazioneCausaleDal.LeggiAsync(leggiOperazione.LavCod, leggiOperazione.Data, objParametriServer);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }

        return dt;
    }

    public async Task<DataTable> Leggi_CausaleDes_From_CausaleId_Async(int id, AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriUtenti objParametriUtenti)
    {
        DataTable dt;
        try
        {
            dt = await _operazioneCausaleDal.Leggi_CausaleDes_From_CausaleId_DALAsync(id, objParametriServer);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }

        return dt;
    }

    public async Task<bool> OperazioneCausale_ScriviModificaAsync(OperazioneCausale_In body, AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriUtenti objParametriUtenti)
    {

        try
        {

            if (body.isNew)
            {
                var id = await _sequenceDal.NuovoId_TabellaAsync("Operazione_Causale", 0, 2000000000, objParametriServer);
                await _operazioneCausaleDal.ScriviAsync(id, body.Causale, body.LavCod, body.inviato, body.validitaInizio, body.validitaFine, objParametriServer);
            }
            else
                await _operazioneCausaleDal.ModificaAsync(body.Id, body.Causale, body.LavCod, body.inviato, body.validitaInizio, body.validitaFine, objParametriServer);

            return true;
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            return false;
        }
    }

    public async Task<bool> OperazioneCausale_CancellaAsync(int id, int lavCod, AgronicaCoreParametriServer objParametriServer) 
    {
        var result = await _operazioneCausaleDal.CancellaAsync(id, lavCod, objParametriServer);
        return result;
    }
}
