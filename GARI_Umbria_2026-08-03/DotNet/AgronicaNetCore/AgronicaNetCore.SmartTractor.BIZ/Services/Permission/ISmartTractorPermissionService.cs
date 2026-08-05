using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SmartTractor.BIZ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.SmartTractor.BIZ.Services.Permission
{
    public interface ISmartTractorPermissionService
    {
        /// <summary>
        /// Verifies if a user has permission for a Smart Tractor operation.
        /// </summary>
        /// <param name="request">The permission request.</param>
        /// <returns>The permission response.</returns>
        Task<SmartTractorPermissionResponse> VerifySendPermissionAsync(AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer, bool forceRefresh);
    }
}
