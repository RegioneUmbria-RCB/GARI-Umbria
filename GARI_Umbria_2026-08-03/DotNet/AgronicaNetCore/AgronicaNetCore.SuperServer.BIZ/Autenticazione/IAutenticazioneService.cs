using AgronicaCoreDTOStd.InData.Provisioning;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SuperServer.DAL.Autenticazione;
using System.Data;

namespace AgronicaNetCore.SuperServer.BIZ.Autenticazione
{
    public interface IAutenticazioneService
    {
        Task<bool> AggiornaUtentiTokenJWTAsync(string idToken, AggiornaUtentiTokenJWTCampi campi, AgronicaCoreParametriSuperServer objParametriSuperServer);

        Task<CreaTokenJWT_In> RefreshTokenAsync(RefreshTokenJWT_In refreshTokenIN, AgronicaCoreParametriSuperServer objParametriSuperServer);
        [Obsolete("Usare la versione asincrona del metodo")]
        DataTable LeggiObjParametri(string IdToken, AgronicaCoreParametriSuperServer objParametriSuperServer);
        Task<DataTable> LeggiObjParametriAsync(string IdToken, AgronicaCoreParametriSuperServer objParametriSuperServer);
    }
}
