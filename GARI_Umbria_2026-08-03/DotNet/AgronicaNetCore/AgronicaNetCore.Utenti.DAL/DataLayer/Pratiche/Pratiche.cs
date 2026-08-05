using AgronicaCoreVisibilitaStd;
using AgronicaCoreVisibilitaStd.Models;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Utenti.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Data;

namespace AgronicaNetCore.Utenti.DAL.DataLayer.Pratiche
{
    public class Pratiche : BaseDALUtenti, IPratiche
    {
        public Pratiche(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer) { }

        public Task<DataTable> LeggiPiveGerarchiaAsync(string descrizione2, AgronicaCoreParametriServer objParametriServer)
        {
            // UvaLean: allineato al flow legacy di login (InizializzaTabellaUtentiVisibilitaAppoggio_GUID_NoTransaction)
            var sql = VisibilitaQueryBuilder.BuildQueryImprese(descrizione2, ModalitaQuery.UvaLean);
            return ExecuteVisibilitaAsync(sql, objParametriServer);
        }

        public Task<DataTable> LeggiCentriGerarchiaAsync(string descrizione2, AgronicaCoreParametriServer objParametriServer)
        {
            var sql = VisibilitaQueryBuilder.BuildQueryCentri(descrizione2, ModalitaQuery.UvaLean);
            return ExecuteVisibilitaAsync(sql, objParametriServer);
        }

        public async Task<DataTable> LeggiPiveVisibiliAsync(string pivaSuperUser, List<PraticaProfilo> pratiche, AgronicaCoreParametriServer objParametriServer)
        {
            var build = VisibilitaQueryBuilder.BuildQueryPratiche(pratiche);
            if (build == null)
                return new DataTable();

            build.Parametri["@pivaSuperUser"] = pivaSuperUser;

            try
            {
                return await GetDataProvider(objParametriServer).ExecuteReadAsync(build.Sql, build.Parametri);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<DataTable> LeggiCapostipitiEspansioneAsync(IEnumerable<string> piveCapostipiti, AgronicaCoreParametriServer objParametriServer)
        {
            var build = CapostipitiQueryBuilder.BuildQueryEspansioneGerarchia(piveCapostipiti);
            if (build == null)
                return new DataTable();

            try
            {
                return await GetDataProvider(objParametriServer).ExecuteReadAsync(build.Sql, build.Parametri);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<DataTable> LeggiCentriPerPiveAsync(IEnumerable<string> pive, AgronicaCoreParametriServer objParametriServer)
        {
            var lista = (pive ?? Enumerable.Empty<string>())
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Select(p => p.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (lista.Count == 0)
                return new DataTable();

            var parameters = new Dictionary<string, object>
            {
                ["@finestraInizio"] = objParametriServer.FinestraTemporaleInizio,
                ["@finestraFine"] = objParametriServer.FinestraTemporaleFine
            };

            var placeholders = new List<string>(lista.Count);
            for (int i = 0; i < lista.Count; i++)
            {
                var name = "@pivaCentri_" + i;
                placeholders.Add(name);
                parameters[name] = lista[i];
            }

            var sql =
                "SELECT Centri_Aziendali.PIVA, Centri_Aziendali.sa_cod " +
                "FROM Centri_Aziendali (NOLOCK) " +
                "WHERE Centri_Aziendali.PIVA IN (" + string.Join(", ", placeholders) + ") " +
                "AND Centri_Aziendali.Validita_Inizio <= @finestraFine " +
                "AND Centri_Aziendali.Validita_Fine >= @finestraInizio";

            try
            {
                return await GetDataProvider(objParametriServer).ExecuteReadAsync(sql, parameters);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        private async Task<DataTable> ExecuteVisibilitaAsync(string sql, AgronicaCoreParametriServer objParametriServer)
        {
            var parameters = new Dictionary<string, object>
            {
                ["@pivaSuperUser"] = objParametriServer.PivaSuperUser,
                ["@finestraInizio"] = objParametriServer.FinestraTemporaleInizio,
                ["@finestraFine"] = objParametriServer.FinestraTemporaleFine
            };

            try
            {
                return await GetDataProvider(objParametriServer).ExecuteReadAsync(sql, parameters);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }
    }
}
