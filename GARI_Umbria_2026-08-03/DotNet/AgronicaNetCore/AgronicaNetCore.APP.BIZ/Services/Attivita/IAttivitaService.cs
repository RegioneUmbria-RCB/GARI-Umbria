using AgronicaCoreModelsSTD.attivita;
using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.APP.BIZ.Services.Attivita
{
    public interface IAttivitaService
    {
        Task<ArchivioAttivitaCampagna> LeggiAgendeEBrogliacciPerAppAsync_OLD(string piva, DateTime dataRiferimento, DateTime dataUltimaSincro, bool soloImpiantiAttivi, AgronicaCoreParametriTriple tripleParams, string bearerToken, string urlCoreWs);
        Task<ArchivioAttivitaCampagna> LeggiBrogliacciPerAppAsync(string piva, DateTime dataRiferimento, DateTime dataUltimaSincro, bool soloImpiantiAttivi, AgronicaCoreParametriTriple tripleParams, string bearerToken, string urlCoreWs);
    }
}
