using AgronicaNetCore.Anagrafe.BIZ.Services.Orogel.Models;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Anagrafe.BIZ.Services.Orogel
{
    public interface IOrogelMultiAnnoService
    {
        Task<OrogelRoutingResult> ValidaEInstradaArchivioAsync(
            int anno,
            AgronicaCoreParametriServer objParametriServer
        );
    }
}
