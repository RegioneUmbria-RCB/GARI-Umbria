using AgronicaNetCore.Base.Models;
using OutData.Zoo.DataMars;
using AgronicaNetCore.OperazioniZoo.DAL.DataLayer.PesateAutomaticheDataMars.AgendaPesatura;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.OperazioniZoo.BIZ.Services.PesateAutomaticheDataMars.PrevenzioneDuplicati;

/// <summary>
/// Implementazione del controllo duplicati per operazioni di pesatura su agenda.
/// <para>Riferimento spec: DS06-BL PrevenzioneDuplicatiOperazioniAgenda — Descrizione,
/// Regole di Business, Persistenze Coinvolte.</para>
/// </summary>
public sealed class PrevenzioneDuplicatiOperazioniAgendaService : BaseServiceOperazioniZooBIZ, IPrevenzioneDuplicatiOperazioniAgendaService
{
    private readonly IAgendaPesaturaRepository _agendaPesaturaRepository;

    public PrevenzioneDuplicatiOperazioniAgendaService(
        IServiceProvider provider,
        IStringLocalizer<Resources.Messages> localizer)
        : base(provider, localizer)
    {
        _agendaPesaturaRepository = provider.GetRequiredService<IAgendaPesaturaRepository>();
    }

    /// <inheritdoc/>
    public async Task<DuplicatoCheckResult> CheckDuplicatoPerDataAsync(
        string piva,
        int saCod,
        int staNum,
        IReadOnlyList<int> codProgettiAnimali,
        DateTime dataPesata,
        AgronicaCoreParametriServer objParametriServer)
    {
        try
        {
            var count = await _agendaPesaturaRepository.CheckDuplicatoPesaturaPerDataAsync(
                piva, saCod, staNum, codProgettiAnimali, dataPesata, objParametriServer);

            return new DuplicatoCheckResult
            {
                IsDuplicate = count > 0,
                NumDuplicatiFound = count
            };
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
    }
}
