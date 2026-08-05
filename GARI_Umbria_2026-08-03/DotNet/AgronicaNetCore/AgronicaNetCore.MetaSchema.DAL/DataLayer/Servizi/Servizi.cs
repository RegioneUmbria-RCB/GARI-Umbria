using AgronicaCoreDTOStd.InData.Metaschema;
using AgronicaCoreDTOStd.InData.Pratiche;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MetaSchema.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Text;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.Servizi
{
    public class Servizi : BaseDALMetaschema, IServizi
    {
        public Servizi(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer) { }

        public async Task<DataTable> LeggiServiziEffettivamenteUsatiAsync(LeggiServizi_IN leggiServizi_IN, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            DataTable result;

            stbQuery.AppendLine(" SELECT     DISTINCT s.Servizio_Cod, s.Servizio_Des");
            stbQuery.AppendLine(" FROM       Servizi s");
            stbQuery.AppendLine(" JOIN       Pratiche p");
            stbQuery.AppendLine(" ON         s.Servizio_Cod = p.Servizio_Cod");
            stbQuery.AppendLine(" WHERE      s.Validita_Inizio <= @dtFine");
            stbQuery.AppendLine(" AND        s.Validita_Fine >= @dtInizio");

            if (objParametriServer.FlagVisibilita == AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati)
                stbQuery.AppendLine(" AND        s.Inviato >= 0 ");
            else if (objParametriServer.FlagVisibilita == AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati)
                stbQuery.AppendLine(" AND        s.Inviato = -1 ");
            else if (objParametriServer.FlagVisibilita == AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti)
            {
                //nessun filtro da aggiungere
            }
            else
                throw new Exception("Parametro non corretto nella query (FlagVisibilita)");

            if (leggiServizi_IN.Servizio_Cod != 0)
            {
                stbQuery.AppendLine(" AND        s.Servizio_Cod = @servizioCod");
                parametriSql.Add("@servizioCod", leggiServizi_IN.Servizio_Cod);
            }

            if (!string.IsNullOrEmpty(leggiServizi_IN.Servizio_Des))
            {
                stbQuery.AppendLine(" AND        s.Servizio_Des LIKE CONCAT('%', @servizioDes, '%')");
                parametriSql.Add("@servizioDes", leggiServizi_IN.Servizio_Des);
            }

            stbQuery.AppendLine(" ORDER BY   s.Servizio_Cod");

            parametriSql.Add("@dtInizio", objParametriServer.FinestraTemporaleInizio);
            parametriSql.Add("@dtFine", objParametriServer.FinestraTemporaleFine);

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

        public async Task<DataTable> LeggiServizi_StatiAsync(LeggiServiziStati_IN leggiServiziStati_IN, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            DataTable result;

            stbQuery.AppendLine(" SELECT * FROM ( ");

            stbQuery.AppendLine("   SELECT     WAnagraficaStati.WAnagraficaStati_Cod AS Stato_Cod, WAnagraficaStati.WAnagraficaStati_Des AS Stato_Des, WTransizioniDiStatoConfigurazione.Servizio_Cod ");
            stbQuery.AppendLine("   FROM       WTransizioniDiStatoConfigurazione ");
            stbQuery.AppendLine("   JOIN       WAnagraficaStati ON WTransizioniDiStatoConfigurazione.Stato_Origine_Cod = WAnagraficaStati.WAnagraficaStati_Cod ");

            stbQuery.AppendLine("   UNION ");

            stbQuery.AppendLine("   SELECT     WAnagraficaStati.WAnagraficaStati_Cod AS Stato_Cod, WAnagraficaStati.WAnagraficaStati_Des AS Stato_Des, WTransizioniDiStatoConfigurazione.Servizio_Cod ");
            stbQuery.AppendLine("   FROM       WTransizioniDiStatoConfigurazione ");
            stbQuery.AppendLine("   JOIN       WAnagraficaStati ON WTransizioniDiStatoConfigurazione.Stato_Destinazione_cod = WAnagraficaStati.WAnagraficaStati_Cod ");
       
            stbQuery.AppendLine(" ) X ");

            stbQuery.AppendLine(" WHERE      1 = 1 ");

            if (leggiServiziStati_IN.Stato_Cod != 0)
            {
                stbQuery.AppendLine(" AND        Stato_Cod = @statoCod");
                parametriSql.Add("@statoCod", leggiServiziStati_IN.Stato_Cod);
            }

            if (leggiServiziStati_IN.Servizio_Cod != 0)
            {
                stbQuery.AppendLine(" AND        Servizio_Cod = @servizioCod");
                parametriSql.Add("@servizioCod", leggiServiziStati_IN.Servizio_Cod);
            }

            if (!string.IsNullOrEmpty(leggiServiziStati_IN.Stato_Des))
            {
                stbQuery.AppendLine(" AND        Stato_Des LIKE CONCAT('%', @statoDes, '%')");
                parametriSql.Add("@statoDes", leggiServiziStati_IN.Stato_Des);
            }

            stbQuery.AppendLine(" ORDER BY        Stato_Cod");

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

        /// <inheritdoc/>
        public async Task<DataTable> LeggiCatalogoServiziAsync(LeggiCatalogoServizi_IN leggiCatalogoServizi_IN, AgronicaCoreParametriServer objParametriServer)
        {
            var parametriSql = new Dictionary<string, object>();
            var stbQuery = new StringBuilder();

            var offset = (leggiCatalogoServizi_IN.Page - 1) * leggiCatalogoServizi_IN.PageSize;
            var sortDirection = string.Equals(leggiCatalogoServizi_IN.Sort, "DESC", StringComparison.OrdinalIgnoreCase)
                ? "DESC"
                : "ASC";

            stbQuery.AppendLine("SELECT  Servizio_Cod,");
            stbQuery.AppendLine("        Servizio_Des,");
            stbQuery.AppendLine("        Validita_Inizio,");
            stbQuery.AppendLine("        Validita_Fine,");
            stbQuery.AppendLine("        CASE WHEN Validita_Fine >= CAST(GETDATE() AS DATE) THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS IsValida,");
            stbQuery.AppendLine("        CASE WHEN Inviato = 0 THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS IsAttiva,");
            stbQuery.AppendLine("        COUNT(*) OVER() AS TotalCount");
            stbQuery.AppendLine("FROM    Servizi WITH (NOLOCK)");
            stbQuery.AppendLine("WHERE   Inviato = 0");

            if (!leggiCatalogoServizi_IN.IncludeExpired)
            {
                stbQuery.AppendLine("AND     Validita_Fine >= CAST(GETDATE() AS DATE)");
            }

            if (!string.IsNullOrWhiteSpace(leggiCatalogoServizi_IN.Filter))
            {
                stbQuery.AppendLine("AND     Servizio_Des LIKE CONCAT('%', @filter, '%')");
                parametriSql.Add("@filter", leggiCatalogoServizi_IN.Filter);
            }

            stbQuery.AppendLine($"ORDER BY Servizio_Cod {sortDirection}");
            stbQuery.AppendLine("OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY");

            parametriSql.Add("@offset", offset);
            parametriSql.Add("@pageSize", leggiCatalogoServizi_IN.PageSize);

            try
            {
                return await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }
    }
}
