using System.Data;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.MetaSchema.BIZ.Services.GruppiOperazione;

public interface IGruppiOperazioneService
{
    Task<DataTable> GruppiOperazione_LeggiAsync(AgronicaCoreParametriServer objParametriServer);
}