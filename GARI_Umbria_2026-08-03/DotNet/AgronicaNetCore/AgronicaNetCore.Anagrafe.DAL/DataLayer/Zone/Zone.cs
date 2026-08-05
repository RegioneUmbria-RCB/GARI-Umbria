using AgronicaNetCore.Anagrafe.DAL.Resources;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Text;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.Zone
{
    public class Zone : BaseDALAnagrafe, IZone
    {
        public Zone(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer) { }

        public async Task<DataTable> LeggiZoneAsync(int Zona_Cod, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            DataTable result;// = new DataTable();

            var filtraPerZona_Cod = Zona_Cod != 0;

            stbQuery.AppendLine(" SELECT    Descrizione, Zona_Cod ");
            stbQuery.AppendLine(" FROM      Zone ");
            stbQuery.AppendLine(" WHERE     1=1 ");

            if (filtraPerZona_Cod)
            {
                stbQuery.AppendLine(" AND       Zona_Cod = @zonaCod ");
                parametriSql.Add("@zonaCod", Zona_Cod);
            }

            if (objParametriServer.FlagVisibilita == AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati)
                stbQuery.AppendLine(" AND       Zone.Inviato >= 0 ");
            else if (objParametriServer.FlagVisibilita == AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati)
                stbQuery.AppendLine(" AND       Zone.Inviato = -1 ");
            else if (objParametriServer.FlagVisibilita == AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti)
            {
                //nessun filtro da aggiungere
            }
            else
                throw new Exception("Parametro non corretto nella query (FlagVisibilita)");

            stbQuery.AppendLine(" ORDER BY  Descrizione");

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
    }
}
