using AgronicaCoreDTOStd.InData.Provisioning;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SuperServer.BIZ.Resources;
using AgronicaNetCore.SuperServer.DAL.Autenticazione;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Data;

namespace AgronicaNetCore.SuperServer.BIZ.Autenticazione
{
    public class AutenticazioneService : BaseServiceSuperServerBIZ, IAutenticazioneService
    {
        private readonly IAutenticazione _autenticazione;

        public AutenticazioneService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _autenticazione = _serviceProvider.GetRequiredService<IAutenticazione>();
        }

        public async Task<bool> AggiornaUtentiTokenJWTAsync(string idToken, AggiornaUtentiTokenJWTCampi campi, AgronicaCoreParametriSuperServer objParametriSuperServer)
        {
            bool result;
            try
            {
                result = await _autenticazione.AggiornaUtentiTokenJWTAsync(idToken, campi, objParametriSuperServer);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriSuperServer, ex);
                throw;
            }
            return result;
        }

        [Obsolete("Usare la versione asincrona del metodo")]
        public DataTable LeggiObjParametri(string IdToken, AgronicaCoreParametriSuperServer objParametriSuperServer)
        {
            DataTable result;
            try
            {
                result = _autenticazione.LeggiObjParametri(IdToken, objParametriSuperServer);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriSuperServer, ex);
                throw;
            }
            return result;
        }

        public async Task<DataTable> LeggiObjParametriAsync(string IdToken, AgronicaCoreParametriSuperServer objParametriSuperServer)
        {
            DataTable result;
            try
            {
                result = await _autenticazione.LeggiObjParametriAsync(IdToken, objParametriSuperServer);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriSuperServer, ex);
                throw;
            }
            return result;
        }

        public async Task<CreaTokenJWT_In> RefreshTokenAsync(RefreshTokenJWT_In refreshTokenIN, AgronicaCoreParametriSuperServer objParametriSuperServer)
        {
            DataTable oldRefreshTokenDT;
            try
            {
                oldRefreshTokenDT = await _autenticazione.LeggiConRefreshTokenAsync(refreshTokenIN.oldRefreshToken, objParametriSuperServer);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriSuperServer, ex);
                throw;
            }
            if (oldRefreshTokenDT.Rows.Count == 0)
                throw new Exception("Vecchio token non presente");

            var oldRefreshRoken = oldRefreshTokenDT.Rows[0];

            CreaTokenJWT_In newTokenJWT = new CreaTokenJWT_In();
            newTokenJWT.objP_super_server = (string)oldRefreshRoken["objP_SuperServer"];
            newTokenJWT.objP_server = (string)oldRefreshRoken["objP_server"];
            newTokenJWT.objP_utenti = (string)oldRefreshRoken["objP_utenti"];
            newTokenJWT.codiceFiscale = (string)oldRefreshRoken["Codice_Fiscale"];
            newTokenJWT.coreWSBaseURL = (string)oldRefreshRoken["CoreWSBaseURL"];
            newTokenJWT.username = (string)oldRefreshRoken["Username"];
            newTokenJWT.pivaSuperUser = (string)oldRefreshRoken["PivaSuperUser"];
            newTokenJWT.versioneApp = (string)oldRefreshRoken["VersioneApp"];
            newTokenJWT.IdDB = (int)oldRefreshRoken["IdDB"];

            bool rowInserted;
            try
            {
                rowInserted = await _autenticazione.ScriviAsync(refreshTokenIN.newTokenID,
                    newTokenJWT.objP_super_server,
                    newTokenJWT.objP_server,
                    newTokenJWT.objP_utenti,
                    newTokenJWT.codiceFiscale,
                    newTokenJWT.coreWSBaseURL,
                    newTokenJWT.username,
                    newTokenJWT.pivaSuperUser,
                    newTokenJWT.versioneApp,
                    refreshTokenIN.newRefreshToken,
                    refreshTokenIN.dataCreazione,
                    refreshTokenIN.dataFineValidita,
                    newTokenJWT.IdDB,
                    objParametriSuperServer
                    );
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriSuperServer, ex);
                throw;
            }
            
            if (rowInserted)
            {
                newTokenJWT.idToken = refreshTokenIN.newTokenID;
                newTokenJWT.refreshToken = refreshTokenIN.newRefreshToken;
                newTokenJWT.dataCreazione = refreshTokenIN.dataCreazione;
                newTokenJWT.dataFineValidita = refreshTokenIN.dataFineValidita;
            }

            return newTokenJWT;
        }
    }
}
