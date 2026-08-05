using AgronicaCoreDTOStd.Identity;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Utenti.DAL.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using anagrafiche = AgronicaCoreModelsSTD.anagrafiche;
using profilazione = AgronicaCoreModelsSTD.profilazione;

namespace AgronicaNetCore.Utenti.DAL.DataLayer.Utenti
{
    public class Utenti : BaseDALUtenti, IUtenti
    {
        public Utenti(IServiceProvider provider, IStringLocalizer<Messages> localizer, bool securityBypass = false) : base(provider, localizer, securityBypass)
        {
        }

        public async Task<DataTable> LeggiAsync(AgronicaCoreParametriUtenti objParametriUtenti)
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            DataTable result = null!;

            stbQuery.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;");
            stbQuery.AppendLine(" SELECT        * ");
            stbQuery.AppendLine(" FROM          Utenti (NOLOCK)");
            stbQuery.AppendLine(" ORDER BY      Utenti.UserName ");

            try
            {
                result = await GetDataProvider(objParametriUtenti).ExecuteReadAsync(stbQuery.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriUtenti, ex);
                throw;
            }

            return result;
        }

        public async Task<DataTable> LeggiUtenteAsync(string username, AgronicaCoreParametriUtenti objParametriUtenti)
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            DataTable result = null!;

            stbQuery.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;");
            stbQuery.AppendLine(" SELECT        * ");
            stbQuery.AppendLine(" FROM          Utenti (NOLOCK)");
            stbQuery.AppendLine(" WHERE         UserName = @username");

            parametriSql.Add("@username", username);

            try
            {
                result = await GetDataProvider(objParametriUtenti).ExecuteReadAsync(stbQuery.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriUtenti, ex);
                throw;
            }

            return result;
        }

        public async Task<bool> EsisteUtenteCuaaAsync(string cuaa, AgronicaCoreParametriServer objParametriServer)
        {
            bool result = false;

            try
            {
                DataTable DT = await LeggiCodiceCuaaAsync(cuaa, objParametriServer);
                result = (DT != null);
                result = result && (DT!.Rows.Count > 0);
                result = result && (!string.IsNullOrEmpty(DT!.Rows[0].Field<string>("PIVA")));
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            return result;
        }

        public async Task<DataTable> LeggiCodiceCuaaAsync(string cuaa, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            DataTable result = null!;

            stbQuery.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;");
            stbQuery.AppendLine(" SELECT TOP(1) * ");
            stbQuery.AppendLine(" FROM Imprese_Codici (NOLOCK)");
            stbQuery.AppendLine(" WHERE id_cod = 1010");
            stbQuery.AppendLine(" AND val_cod = @cuaa");
            stbQuery.AppendLine(" AND Validita_Inizio <= @validitaFine");
            stbQuery.AppendLine(" AND Validita_Fine >= @validitaInizio");

            parametriSql.Add("@cuaa", cuaa);
            parametriSql.Add("@validitaInizio", objParametriServer.FinestraTemporaleInizio);
            parametriSql.Add("@validitaFine", objParametriServer.FinestraTemporaleFine);

            try
            {
                result = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            return result;
        }

        public async Task<string> LeggiPivaByCuaaAsync(string cuaa, AgronicaCoreParametriServer objParametriServer)
        {
            if (string.IsNullOrEmpty(cuaa))
                return null!;

            // Deve esistere il codice CUAA
            bool isEsisteCuaaUtente = await EsisteUtenteCuaaAsync(cuaa, objParametriServer);
            if (!isEsisteCuaaUtente)
                return null!;

            string piva = null!;
            DataTable cuaaUtente = await LeggiCodiceCuaaAsync(cuaa, objParametriServer);
            if (cuaaUtente == null || cuaaUtente.Rows.Count < 1)
                return null!;

            return cuaaUtente.Rows[0].Field<string>("PIVA")!;
        }

        public async Task<string> LeggiNumeroTesseraByCuaaAsync(string username, string cuaa, AgronicaCoreParametriServer objParametriServer)
        {
            if (string.IsNullOrEmpty(username))
                return null!;

            if (string.IsNullOrEmpty(cuaa))
                return null!;

            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            string result = null!;

            stbQuery.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;");
            stbQuery.AppendLine(" SELECT TOP(1) uva.Piva NumeroTessera, * ");
            stbQuery.AppendLine(" FROM [Imprese_Codici] ic WITH(NOLOCK)");
            stbQuery.AppendLine(" JOIN [Utenti_Visibilita_Appoggio] uva WITH(NOLOCK)");
            stbQuery.AppendLine(" ON ic.PIVA = uva.Piva");
            stbQuery.AppendLine(" WHERE ic.id_cod = 1010");
            stbQuery.AppendLine(" AND ic.val_cod = @cuaa");
            stbQuery.AppendLine(" AND uva.Username = @username");
            stbQuery.AppendLine(" AND ic.Validita_Inizio <= @validitaFine");
            stbQuery.AppendLine(" AND ic.Validita_Fine >= @validitaInizio");
            stbQuery.AppendLine(" AND uva.Entita_Cod = 1");


            parametriSql.Add("@username", username);
            parametriSql.Add("@cuaa", cuaa);
            parametriSql.Add("@validitaInizio", objParametriServer.FinestraTemporaleInizio);
            parametriSql.Add("@validitaFine", objParametriServer.FinestraTemporaleFine);

            try
            {
                DataTable DT = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parametriSql);
                if (DT != null && DT.Rows.Count > 0)
                    result = DT.Rows[0].Field<string>("NumeroTessera")!;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            return result;
        }

        public async Task<profilazione.LeggiScriviVisibilitaUtente> MapUserToAgronicaUtente(string pivaSuperUser, string piva, string username,
            UtenteColdiretti datiColdiretti, int profiloDefault, AgronicaCoreParametriServer objParametriServer)
        {
            profilazione.LeggiScriviVisibilitaUtente profiloUtente = new profilazione.LeggiScriviVisibilitaUtente()
            {
                Utente = new profilazione.UtenteDTO()
                {
                    piva = "",
                    Piva_SuperUser = pivaSuperUser,
                    UserName = username,
                    Tipologia = new profilazione.TipologiaUtente() { codice = profiloDefault },
                    username_commerciale = "UT.PDS.DEMETRA",
                    Email = "",
                    Password = "",
                    Nome = datiColdiretti.Nome(),
                    Cognome = datiColdiretti.Cognome(),
                    Rag_Soc = datiColdiretti.RagioneSociale(),
                    codice_fiscale = datiColdiretti.data.SGL_CODFIS,
                    flag_azienda_persona = datiColdiretti.IsPersonaGiuridica
                },
                AziendeVisibili = new List<anagrafiche.ImpresaDto>() { new anagrafiche.ImpresaDto { piva = piva, Sa_Cod = 0, Sa_Nome = "" } }
            };
            return await Task.FromResult(profiloUtente);
        }

        public async Task<DataTable> LeggiUtentiInScadenzaPasswordAsync(int giorniScadenza, int giorniPreavviso, AgronicaCoreParametriUtenti objParametriUtenti)
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            DataTable result = null!;

            stbQuery.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;");
            stbQuery.AppendLine(" SELECT u.UserName, u.DataUltimaModificaPassword, u.Lingua_cod,");
            stbQuery.AppendLine("        ud.Cognome, ud.Nome, ud.Email,");
            stbQuery.AppendLine("        DATEDIFF(day, GETDATE(), DATEADD(day, @giorniScadenza, u.DataUltimaModificaPassword)) AS GiorniRimanenti,");
            stbQuery.AppendLine("        DATEADD(day, @giorniScadenza, u.DataUltimaModificaPassword) AS DataScadenza");
            stbQuery.AppendLine(" FROM   Utenti u (NOLOCK)");
            stbQuery.AppendLine(" JOIN   Utenti_Dettagli ud (NOLOCK) ON ud.UserName = u.UserName");
            stbQuery.AppendLine(" WHERE  u.DataUltimaModificaPassword IS NOT NULL");
            stbQuery.AppendLine("        AND NULLIF(LTRIM(RTRIM(ud.Email)), '') IS NOT NULL");
            stbQuery.AppendLine("        AND DATEDIFF(day, GETDATE(), DATEADD(day, @giorniScadenza, u.DataUltimaModificaPassword)) BETWEEN 0 AND @giorniPreavviso");

            parametriSql.Add("@giorniScadenza", giorniScadenza);
            parametriSql.Add("@giorniPreavviso", giorniPreavviso);

            try
            {
                result = await GetDataProvider(objParametriUtenti).ExecuteReadAsync(stbQuery.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriUtenti, ex);
                throw;
            }

            return result;
        }
    }
}
