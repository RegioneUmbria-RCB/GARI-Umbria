using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Gis.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Dynamic;
using System.Text;
using AgronicaCoreDTOStd.InData.Gis;
using AgronicaNetCore.Base.Constants;
using Exception = System.Exception;

namespace AgronicaNetCore.Gis.DAL.DataLayer.Gis
{
    public class Gis : BaseDALGis, IGis
    {
        public Gis(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
        }

        public async Task<DataTable> LeggiGIS_ProcessingAlgorithms_Cleaning_AlgorithmAsync(
            AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            DataTable result;

            stbQuery.AppendLine(" SELECT     Algoritmo");
            stbQuery.AppendLine(" FROM       GIS_ProcessingAlgorithms_Cleaning_Algorithm");
            stbQuery.AppendLine(" WHERE      Validita_Inizio <= @dtFine");
            stbQuery.AppendLine(" AND        Validita_Fine >= @dtInizio");

            if (objParametriServer.FlagVisibilita == AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati)
                stbQuery.AppendLine(" AND        Inviato >= 0 ");
            else if (objParametriServer.FlagVisibilita ==
                     AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati)
                stbQuery.AppendLine(" AND        Inviato = -1 ");
            else if (objParametriServer.FlagVisibilita == AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti)
            {
                //nessun filtro da aggiungere
            }
            else
                throw new Exception("Parametro non corretto nella query (FlagVisibilita)");

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

        public async Task<bool> UpdateLayerTranslation(string layerCod, string tipologiaLayerCod, string linguaCod,
            string traduzione, AgronicaCoreParametriServer objParametriServer)
        {
            const string query = @"
                MERGE [GIS_LayerElementiGrafici_Anagrafica_XLingue] AS target
                USING (
                    SELECT @Layer_Cod AS LayerElementiGrafici_Cod, @Lingua_Cod AS Lingua_Cod, @TipologiaLayer_Cod AS TipologiaLayer_Cod, @Traduzione AS LayerElementiGrafici_Des
                ) AS source
                ON target.LayerElementiGrafici_Cod = source.LayerElementiGrafici_Cod and target.Lingua_Cod = source.Lingua_Cod and target.TipologiaLayer_Cod = source.TipologiaLayer_Cod
                
                WHEN MATCHED THEN
                UPDATE SET 
                       [LayerElementiGrafici_Des] = source.LayerElementiGrafici_Des
                       ,[inviato] = @inviato
                       ,[datainvio] = @datainvio
                       ,[Data_Modifica] = @Data_Modifica
                       ,[Username_Modifica] = @Username_Modifica
                
                WHEN NOT MATCHED THEN
                INSERT
                       ([Lingua_Cod]
                       ,[LayerElementiGrafici_Cod]
                       ,[TipologiaLayer_Cod]
                       ,[LayerElementiGrafici_Des]
                       ,[inviato] 
                       ,[datainvio]
                       ,[Data_Creazione]
                       ,[Data_Modifica]
                       ,[Username_Creazione]
                       ,[Username_Modifica]
                       ,[Validita_Inizio]
                       ,[Validita_Fine])
                 VALUES
                       (source.Lingua_Cod
                       ,source.LayerElementiGrafici_Cod
                       ,source.TipologiaLayer_Cod
                       ,source.LayerElementiGrafici_Des
                       ,@inviato
                       ,@datainvio
                       ,@Data_Creazione
                       ,@Data_Modifica
                       ,@Username_Creazione
                       ,@Username_Modifica
                       ,@Validita_Inizio
                       ,@Validita_Fine);
            ";

            try
            {
                var parametri = new ExpandoObject();
                parametri.TryAdd("@Layer_Cod", layerCod);
                parametri.TryAdd("@Lingua_Cod", linguaCod);
                parametri.TryAdd("@TipologiaLayer_Cod", tipologiaLayerCod);
                parametri.TryAdd("@Traduzione", traduzione);
                parametri.TryAdd("@inviato", 0);
                parametri.TryAdd("@datainvio", DBNull.Value);
                parametri.TryAdd("@Data_Creazione", DateTime.Now);
                parametri.TryAdd("@Data_Modifica", DateTime.Now);
                parametri.TryAdd("@Username_Creazione", objParametriServer.UsernameOperazione);
                parametri.TryAdd("@Username_Modifica", objParametriServer.UsernameOperazione);
                parametri.TryAdd("@Validita_Inizio", CostantiPersonalizzate.AGRODATAINIZIO_DATE);
                parametri.TryAdd("@Validita_Fine", CostantiPersonalizzate.AGRODATAFINE_DATE);

                return await GetDataProvider(objParametriServer).Execute_WriteAsync(query, parametri);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<bool> UpdateLayerLabelTranslation(string layerCod, string tipologiaLayerCod,
            string tipologiaLayerStructCod, string linguaCod, string traduzione,
            AgronicaCoreParametriServer objParametriServer)
        {
            const string query = @"
                MERGE [GIS_LayerElementiGrafici_Anagrafica_DataStruct_XLingue] AS target
                USING (
                    SELECT @Layer_Cod AS LayerElementiGrafici_Cod, @Lingua_Cod AS Lingua_Cod, @TipologiaLayer_Cod AS TipologiaLayer_Cod, 
                           @TipologiaLayer_struct_cod AS TipologiaLayer_struct_cod, @Traduzione AS LayerElementiGrafici_Etichetta
                ) AS source
                ON target.LayerElementiGrafici_Cod = source.LayerElementiGrafici_Cod and target.Lingua_Cod = source.Lingua_Cod 
                    and target.TipologiaLayer_Cod = source.TipologiaLayer_Cod and target.TipologiaLayer_struct_cod = source.TipologiaLayer_struct_cod
                
                WHEN MATCHED THEN
                UPDATE SET 
                       [LayerElementiGrafici_Etichetta] = source.LayerElementiGrafici_Etichetta
                       ,[inviato] = @inviato
                       ,[datainvio] = @datainvio
                       ,[Data_Modifica] = @Data_Modifica
                       ,[Username_Modifica] = @Username_Modifica
                
                WHEN NOT MATCHED THEN
                INSERT
                       ([Lingua_Cod]
                       ,[LayerElementiGrafici_Cod]
                       ,[TipologiaLayer_Cod]
                       ,[TipologiaLayer_struct_cod]
                       ,[LayerElementiGrafici_Etichetta]
                       ,[inviato] 
                       ,[datainvio]
                       ,[Data_Creazione]
                       ,[Data_Modifica]
                       ,[Username_Creazione]
                       ,[Username_Modifica]
                       ,[Validita_Inizio]
                       ,[Validita_Fine])
                 VALUES
                       (source.Lingua_Cod
                       ,source.LayerElementiGrafici_Cod
                       ,source.TipologiaLayer_Cod
                       ,source.TipologiaLayer_struct_cod
                       ,source.LayerElementiGrafici_Etichetta
                       ,@inviato
                       ,@datainvio
                       ,@Data_Creazione
                       ,@Data_Modifica
                       ,@Username_Creazione
                       ,@Username_Modifica
                       ,@Validita_Inizio
                       ,@Validita_Fine);
            ";

            try
            {
                var parametri = new ExpandoObject();
                parametri.TryAdd("@Layer_Cod", layerCod);
                parametri.TryAdd("@Lingua_Cod", linguaCod);
                parametri.TryAdd("@TipologiaLayer_Cod", tipologiaLayerCod);
                parametri.TryAdd("@TipologiaLayer_struct_cod", tipologiaLayerStructCod);
                parametri.TryAdd("@Traduzione", traduzione);
                parametri.TryAdd("@inviato", 0);
                parametri.TryAdd("@datainvio", DBNull.Value);
                parametri.TryAdd("@Data_Creazione", DateTime.Now);
                parametri.TryAdd("@Data_Modifica", DateTime.Now);
                parametri.TryAdd("@Username_Creazione", objParametriServer.UsernameOperazione);
                parametri.TryAdd("@Username_Modifica", objParametriServer.UsernameOperazione);
                parametri.TryAdd("@Validita_Inizio", CostantiPersonalizzate.AGRODATAINIZIO_DATE);
                parametri.TryAdd("@Validita_Fine", CostantiPersonalizzate.AGRODATAFINE_DATE);

                return await GetDataProvider(objParametriServer).Execute_WriteAsync(query, parametri);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }
    }
}