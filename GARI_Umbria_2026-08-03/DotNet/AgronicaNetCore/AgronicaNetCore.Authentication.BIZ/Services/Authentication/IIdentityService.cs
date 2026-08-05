using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AgronicaCoreDTOStd.Identity;

namespace AgronicaNetCore.Authentication.BIZ.Services.Authentication
{
    public interface IIdentityService
    {
        Task<AuthenticationCheckResult> IsAuthorizedAsync(ClaimsIdentity? identity, IHeaderDictionary headers);
    }
}
