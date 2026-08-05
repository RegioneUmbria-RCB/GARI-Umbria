using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SmartTractor.BIZ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.SmartTractor.BIZ.Services
{
    public interface ISmartTractorService
    {
        Task<IReadOnlyList<GestoreInvioResponse>> SendPrescriptionAsync(
            int ricettaOperazioneCod,
            AgronicaCoreParametriServer serverParams,
            AgronicaCoreParametriSuperServer superServerParams,
            AgronicaCoreParametriUtenti userParams,
            CancellationToken cancellationToken = default);
    }
}
