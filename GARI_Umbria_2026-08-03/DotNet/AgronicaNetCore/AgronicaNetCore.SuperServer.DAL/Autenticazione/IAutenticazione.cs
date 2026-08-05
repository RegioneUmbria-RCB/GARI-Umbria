using AgronicaCoreDTOStd.InData.Provisioning;
using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.SuperServer.DAL.Autenticazione
{
    public interface IAutenticazione
    {

        Task<bool> AggiornaUtentiTokenJWTAsync(string idToken, AggiornaUtentiTokenJWTCampi campi, AgronicaCoreParametriSuperServer objParametriSuperServer);

        [Obsolete("Usare la versione asincrona del metodo")]
        DataTable LeggiObjParametri(string IdToken, AgronicaCoreParametriSuperServer objParametriSuperServer);

        Task<DataTable> LeggiObjParametriAsync(string IdToken, AgronicaCoreParametriSuperServer objParametriSuperServer);

        Task<DataTable> LeggiConRefreshTokenAsync(string refreshToken, AgronicaCoreParametriSuperServer objParametriSuperServer);

        Task<bool> ScriviAsync(string tokenId, string objPSuperServer, string objPServer, string objPUtenti, 
            string codiceFiscale, string coreWSBaseUrl, string username, string pivaSuperUser,
            string versioneApp, string refreshToken, DateTime dataCreazione, DateTime dataFineValidita,
            int idDb, AgronicaCoreParametriSuperServer objParametriSuperServer);
    }
}
