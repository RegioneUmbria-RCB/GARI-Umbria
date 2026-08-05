using AgronicaCoreDTOStd.Identity;
using AgronicaCoreDTOStd.InData.Provisioning;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SuperServer.DAL.Autenticazione;
using OutData.Authentication;
using System.Data;

namespace AgronicaNetCore.Authentication.BIZ.Services.Authentication
{
    public interface IRequestTokenService
    {
        /// <summary>
        /// Restituisce l'intero oggetto token con tutti i dettagli (access_token, refresh_token, etc).
        /// </summary>
        Task<TokenResponse> GetTokenFromUserPwdAsync(TokenUserPwdRequest request, CancellationToken cancellationToken = default);
    }
}
