using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SuperServer.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Dynamic;
using System.Text;

namespace AgronicaNetCore.SuperServer.DAL.Autenticazione
{
    public class Autenticazione : BaseDALSuperServer, IAutenticazione
    {
        public Autenticazione(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer) { }

        public async Task<bool> AggiornaUtentiTokenJWTAsync(string idToken, AggiornaUtentiTokenJWTCampi campi, AgronicaCoreParametriSuperServer objParametriSuperServer)
        {

            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            bool result = false;

            stbQuery.AppendLine(" UPDATE   Utenti_Token_JWT  ");
            stbQuery.AppendLine(" SET      AuthCookie = @authCookie, ");
            stbQuery.AppendLine("          Validita_Fine_AccessToken = @scadenzaToken, ");
            stbQuery.AppendLine("          BearerToken = @bearerToken ");
            stbQuery.AppendLine(" WHERE    IdToken = @idToken  ");

            var parametri = new ExpandoObject();

            try
            {
                parametri.TryAdd("@authCookie", campi.AuthCookie);
                parametri.TryAdd("@scadenzaToken", campi.ScadenzaToken);
                parametri.TryAdd("@bearerToken", campi.BearerToken);
                parametri.TryAdd("@idToken", idToken);

                result = await GetDataProvider(objParametriSuperServer).Execute_WriteAsync(stbQuery.ToString(), parametri);
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
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            DataTable result;

            stbQuery.AppendLine("SELECT    IdDB, objP_Server, objP_Utenti, objP_SuperServer");
            stbQuery.AppendLine("FROM      Utenti_Token_JWT");
            stbQuery.AppendLine("WHERE     IdToken = @IdToken");

            parametriSql.Add("@IdToken", IdToken);
            try
            {
                result = await GetDataProvider(objParametriSuperServer).ExecuteReadAsync(stbQuery.ToString(), parametriSql);
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
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            DataTable result;

            stbQuery.AppendLine("SELECT    objP_Server, objP_Utenti, objP_SuperServer");
            stbQuery.AppendLine("FROM      Utenti_Token_JWT");
            stbQuery.AppendLine("WHERE     IdToken = @IdToken");

            parametriSql.Add("@IdToken", IdToken);
            try
            {
                result = GetDataProvider(objParametriSuperServer).ExecuteRead(stbQuery.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriSuperServer, ex);
                throw;
            }
            return result;
        }

        public async Task<DataTable> LeggiConRefreshTokenAsync(string refreshToken, AgronicaCoreParametriSuperServer objParametriSuperServer)
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            DataTable result;

            stbQuery.AppendLine("SELECT    *");
            stbQuery.AppendLine("FROM      Utenti_Token_JWT");
            stbQuery.AppendLine("WHERE     refreshToken = @refreshToken");

            parametriSql.Add("@refreshToken", refreshToken);
            try
            {
                result = await GetDataProvider(objParametriSuperServer).ExecuteReadAsync(stbQuery.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriSuperServer, ex);
                throw;
            }
            return result;
        }

        public async Task<bool> ScriviAsync(string tokenId, string objPSuperServer, string objPServer, string objPUtenti, string codiceFiscale, string coreWSBaseUrl, string username, string pivaSuperUser, string versioneApp, string refreshToken, DateTime dataCreazione, DateTime dataFineValidita, int idDb, AgronicaCoreParametriSuperServer objParametriSuperServer)
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            bool result = false;

            stbQuery.AppendLine("INSERT INTO Utenti_Token_JWT (idToken, ");
            stbQuery.AppendLine("                           objP_SuperServer, ");
            stbQuery.AppendLine("                           objP_Server, ");
            stbQuery.AppendLine("                           objP_Utenti, ");
            stbQuery.AppendLine("                           Codice_Fiscale, ");
            stbQuery.AppendLine("                           CoreWSBaseURL, ");
            stbQuery.AppendLine("                           Username, ");
            stbQuery.AppendLine("                           PivaSuperUser, ");
            stbQuery.AppendLine("                           VersioneApp, ");
            stbQuery.AppendLine("                           refreshToken, ");
            stbQuery.AppendLine("                           Inviato, ");
            stbQuery.AppendLine("                           Data_Creazione, ");
            stbQuery.AppendLine("                           Data_Modifica, ");
            stbQuery.AppendLine("                           Validita_Inizio, ");
            stbQuery.AppendLine("                           Validita_Fine, ");
            stbQuery.AppendLine("                           IdDB )");

            stbQuery.AppendLine("VALUES (");
            stbQuery.AppendLine("    @tokenId, ");
            stbQuery.AppendLine("    @objPSuperServer, ");
            stbQuery.AppendLine("    @objPServer, ");
            stbQuery.AppendLine("    @objPUtenti, ");
            stbQuery.AppendLine("    @codiceFiscale, ");
            stbQuery.AppendLine("    @coreWSBaseUrl, ");
            stbQuery.AppendLine("    @username, ");
            stbQuery.AppendLine("    @pivaSuperUser, ");
            stbQuery.AppendLine("    @versioneApp, ");
            stbQuery.AppendLine("    @refreshToken, ");
            stbQuery.AppendLine("    0, ");
            stbQuery.AppendLine("    @dataCreazione, ");
            stbQuery.AppendLine("    @dataCreazione, ");
            stbQuery.AppendLine("    @dataCreazione, ");
            stbQuery.AppendLine("    @dataFineValidita, ");
            stbQuery.AppendLine("    @idDb ");
            stbQuery.AppendLine(")");

            var parametri = new ExpandoObject();

            try
            {
                parametri.TryAdd("@tokenId", tokenId);
                parametri.TryAdd("@objPSuperServer", objPSuperServer);
                parametri.TryAdd("@objPServer", objPServer);
                parametri.TryAdd("@objPUtenti", objPUtenti);
                parametri.TryAdd("@codiceFiscale", codiceFiscale);
                parametri.TryAdd("@coreWSBaseUrl", coreWSBaseUrl);
                parametri.TryAdd("@username", username);
                parametri.TryAdd("@pivaSuperUser", pivaSuperUser);
                parametri.TryAdd("@versioneApp", versioneApp);
                parametri.TryAdd("@refreshToken", refreshToken);
                parametri.TryAdd("@dataCreazione", dataCreazione);
                parametri.TryAdd("@dataFineValidita", dataFineValidita);
                parametri.TryAdd("@idDb", idDb);

                result = await GetDataProvider(objParametriSuperServer).Execute_WriteAsync(stbQuery.ToString(), parametri);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriSuperServer, ex);
                throw;
            }

            return result;
        }
    }

    public class AggiornaUtentiTokenJWTChiavi
    {
        public AggiornaUtentiTokenJWTChiavi(string pivaSuperUser, string username, int idDB)
        {
            PivaSuperUser = pivaSuperUser;
            Username = username;
            IdDB = idDB;
        }

        public string PivaSuperUser { get; }
        public string Username { get; }
        public int IdDB { get; }
    }

    public class AggiornaUtentiTokenJWTCampi
    {
        public AggiornaUtentiTokenJWTCampi(string authCookie, string bearerToken, DateTime scadenzaToken)
        {
            AuthCookie = authCookie;
            BearerToken = bearerToken;
            ScadenzaToken = scadenzaToken;
        }

        public string AuthCookie { get; }
        public string BearerToken { get; }
        public DateTime ScadenzaToken { get; }
    }
}
