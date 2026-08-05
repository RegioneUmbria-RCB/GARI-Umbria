using System.Text;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.Base.Utility;
using System.Data;
using Microsoft.Extensions.DependencyInjection;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.Fabbricati
{
    public class Fabbricati : DAL_Base, IFabbricati
    {
        private readonly TempChiaviMassivo _tempChiaviMassivo;

        public Fabbricati(IServiceProvider provider, bool securityBypass = false) : base(provider, securityBypass)
        {
            _tempChiaviMassivo = provider.GetRequiredService<TempChiaviMassivo>();
        }

        public async Task<DataTable> LeggiDescrizioneAsync(
            List<(string, int, int)> chiaviFabbricato,
            AgronicaCoreParametriServer objParametriServer,
            bool estraiAzienda = false)
        {
            if (chiaviFabbricato == null || chiaviFabbricato.Count == 0)
                throw new ArgumentException("chiaviFabbricato non può essere vuota.");

            bool usaTempTable = chiaviFabbricato.Count > 1;
            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();

            if (usaTempTable)
                await _tempChiaviMassivo.CreaTabellaTemp_FiltroFabbricati(chiaviFabbricato, objParametriServer);

            stbQuery.AppendLine("SELECT f.PIVA, f.Sa_Cod, f.Fabbricato_Cod, f.Fabbricato_Des");
            if (estraiAzienda)
                stbQuery.AppendLine("    , imp.Rag_Soc");
            stbQuery.AppendLine("FROM Fabbricati f");
            if (estraiAzienda)
            {
                stbQuery.AppendLine("JOIN Imprese imp");
                stbQuery.AppendLine("    ON imp.Piva = f.PIVA");
            }
            if (usaTempTable)
            {
                stbQuery.AppendLine("JOIN #TempFabbricato temp");
                stbQuery.AppendLine("    ON temp.Piva = f.PIVA");
                stbQuery.AppendLine("    AND temp.Sa_Cod = f.Sa_Cod");
                stbQuery.AppendLine("    AND temp.Fabbricato_Cod = f.Fabbricato_Cod");
            }
            else
            {
                stbQuery.AppendLine("WHERE 1 = 1");
                if (!string.IsNullOrWhiteSpace(chiaviFabbricato[0].Item1))
                {
                    stbQuery.AppendLine("    AND f.PIVA = @piva");
                    sqlParams.Add("@piva", chiaviFabbricato[0].Item1.Trim());
                }
                if (chiaviFabbricato[0].Item2 != 0)
                {
                    stbQuery.AppendLine("    AND f.Sa_Cod = @saCod");
                    sqlParams.Add("@saCod", chiaviFabbricato[0].Item2);
                }
                if (chiaviFabbricato[0].Item3 != 0)
                {
                    stbQuery.AppendLine("    AND f.Fabbricato_Cod = @fabbricatoCod");
                    sqlParams.Add("@fabbricatoCod", chiaviFabbricato[0].Item3);
                }
            }

            try
            {
                return await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), sqlParams);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
            finally
            {
                if (usaTempTable)
                    await _tempChiaviMassivo.EliminaTabellaTemp_FiltroFabbricati(objParametriServer);
            }
        }

        public async Task<int> ReadFabbCodAsync(string Piva, int Sa_Cod, int Tipo_Fabb, AgronicaCoreParametriServer objParametriServer)
        {
            if(string.IsNullOrEmpty(Piva))
                throw new ArgumentException("Piva non può essere vuota.");
            if(Sa_Cod == 0)
                throw new ArgumentException("Sa_Cod non può essere vuoto.");

            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();
            stbQuery.AppendLine("SELECT * ")
                .AppendLine("FROM Fabbricati  ")
                .AppendLine("WHERE PIVA = @piva AND Sa_Cod = @saCod ");

            sqlParams.TryAdd("@piva", Piva);
            sqlParams.TryAdd("@saCod", Sa_Cod);

            if (Tipo_Fabb != 0)
            {
                sqlParams.TryAdd("@tipo", Tipo_Fabb);
                stbQuery.AppendLine("    AND Tipo_Fabbricato_Cod = @tipo ");
            }
            sqlParams.TryAdd("@inizio", objParametriServer.FinestraTemporaleInizio);
            sqlParams.TryAdd("@fine", objParametriServer.FinestraTemporaleFine);
            stbQuery.AppendLine("    AND Validita_Inizio <= @inizio ")
                .AppendLine("    AND Validita_Fine >= @fine ")
                .AppendLine("ORDER BY Fabbricato_Cod DESC ");

            try
            {
                var dtFabb = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), sqlParams);

                if (dtFabb.Rows.Count == 0)
                    return 0;

                int fabbCod = (int)dtFabb.Rows[0]["Fabbricato_Cod"];
                return fabbCod;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<DataTable> ReadAsync(string Piva, int Sa_Cod, int Fabbricato_Cod, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();
            stbQuery.AppendLine("SELECT * ")
                .AppendLine("FROM Fabbricati  ")
                .AppendLine("WHERE 1 = 1 ");

            if (Piva != "")
            {
                stbQuery.AppendLine(" AND PIVA = @piva ");
                sqlParams.TryAdd("@piva", Piva);
            }

            if (Sa_Cod != 0)
            {
                stbQuery.AppendLine(" AND Sa_Cod = @saCod ");
                sqlParams.TryAdd("@saCod", Sa_Cod);
            }

            if (Fabbricato_Cod != 0)
            {
                sqlParams.TryAdd("@Fabbricato_Cod", Fabbricato_Cod);
                stbQuery.AppendLine("    AND Fabbricato_Cod = @Fabbricato_Cod ");
            }
            sqlParams.TryAdd("@inizio", objParametriServer.FinestraTemporaleInizio);
            sqlParams.TryAdd("@fine", objParametriServer.FinestraTemporaleFine);
            stbQuery.AppendLine("    AND Validita_Inizio <= @inizio ")
                .AppendLine("    AND Validita_Fine >= @fine ")
                .AppendLine("ORDER BY Fabbricato_Cod DESC ");

            try
            {
                var dtFabb = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), sqlParams);
                return dtFabb;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

    }
}
