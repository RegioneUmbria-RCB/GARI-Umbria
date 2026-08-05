using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Gis.DAL.Resources;
using AgronicaNetCore.Gis.Shared.Interfaces;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.Gis.DAL.DataLayer.GisElementiGrafici
{
    public class GisElementiGrafici : BaseDALGis, IGisElementiGrafici
    {
        public GisElementiGrafici(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider, localizer) { }

        /// <summary>
        /// Aggiorna la colonna StaticMap in GIS_ElementiGrafici per l'entità indicata.
        /// </summary>
        public async Task<bool> AggiornaStaticMapAsync(
            int entitaCod,
            byte[] staticMap,
            AgronicaCoreParametri objParametri
        )
        {
            var stb = new StringBuilder();
            stb.AppendLine(" UPDATE g");
            stb.AppendLine("    SET StaticMap = @StaticMap");
            stb.AppendLine("      , data_modifica  = GETDATE()");
            stb.AppendLine(" FROM gis_entita e");
            stb.AppendLine("     INNER JOIN gis_elementigrafici g");
            stb.AppendLine("         ON  e.PivaSuperUser = g.PivaSuperUser");
            stb.AppendLine("         AND e.entita_cod = g.Entita_Cod");
            stb.AppendLine(" WHERE e.Entita_Cod = @entitaCod");

            var dataProvider = GetDataProvider(objParametri);
            DbConnection? connection = null;

            try
            {
                return await GetDataProvider(objParametri)
                    .Execute_WriteVarBinaryAsync(
                        stb.ToString(),
                        new Dictionary<string, byte[]> { { "@StaticMap", staticMap } },
                        new Dictionary<string, object> { { "@entitaCod", entitaCod } }
                    );
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametri, ex);
                throw;
            }
            finally
            {
                dataProvider.CloseConnection(ref connection, RollbackTransaction: false);
            }
        }
    }
}
