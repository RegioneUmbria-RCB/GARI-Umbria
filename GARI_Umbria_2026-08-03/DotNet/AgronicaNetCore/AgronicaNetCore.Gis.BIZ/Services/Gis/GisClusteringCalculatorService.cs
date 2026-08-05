using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Gis.DAL.DataLayer.Gis;
using AgronicaNetCore.Gis.BIZ.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using AgronicaCoreDTOStd.InData.Gis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using InData.Gis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace AgronicaNetCore.Gis.BIZ.Services.Gis
{
    public class GisClusteringCalculatorService : BaseServiceGisBIZ, IGisClusteringCalculatorService
    {
        private const int ChunkSize = 2_000;
        private readonly IGisClustering _gisClusteringDal;
        public GisClusteringCalculatorService(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider, localizer)
        {
            _gisClusteringDal = _serviceProvider.GetRequiredService<IGisClustering>();
        }

        public async Task<bool> StartJob_SaveCentroidsAsync(string pivaSuperUser, int? layerCod, int? elementoGraficoCod, int? batchSize, AgronicaCoreParametriServer objParametriServer)
        {

            objParametriServer.LogFileName="GIS_CalcoloCentridi";   // Specifico un file di log dedicato.
            try
            {
                LogInformation($"Inizio elaborazione di un batch di calcolo centroidi. BatchSize={batchSize ?? 0}", objParametriServer);

                // Legge un singolo "batch" di record da processare dal dal.
                DataTable recordsToProcessTable = await _gisClusteringDal.GetClusteringRecordsToProcessAsync(
                    pivaSuperUser, layerCod, elementoGraficoCod, batchSize, objParametriServer);

                if (recordsToProcessTable == null || recordsToProcessTable.Rows.Count == 0)
                {
                    LogInformation("Nessun record da processare trovato in questo batch. Il lavoro è terminato.", objParametriServer);
                    return true;
                }

                // Mappa il dt in una lista di oggetti.
                var recordsList = new List<ClusteringRecordToProcess>();
                foreach (DataRow row in recordsToProcessTable.Rows)
                {
                    recordsList.Add(new ClusteringRecordToProcess
                    {
                        PivaSuperUser = row["PivaSuperUser"].ToString(),
                        LayerElementoGrafico_Cod = Convert.ToInt32(row["LayerElementoGrafico_Cod"]),
                        ElementoGrafico_Cod = Convert.ToInt32(row["ElementoGrafico_Cod"])
                    });
                }

                LogInformation($"Trovati {recordsList.Count} record in questo batch di cui calcolare il centroide.", objParametriServer);

                var sw = new Stopwatch();
                sw.Start();
                // Processa i record di questo batch.
                foreach (var recordsByLayer in recordsList.GroupBy(x => x.LayerElementoGrafico_Cod))
                {
                    foreach (var record in recordsByLayer.Chunk(ChunkSize))
                    {
                        var elementoGraficoCods = record.Select(x => x.ElementoGrafico_Cod).ToArray();
                        try
                        {
                            if (!await SaveCentroidAsync(pivaSuperUser, recordsByLayer.Key, elementoGraficoCods, objParametriServer))
                                LogWarning($"Calcolo del centroide fallito per {pivaSuperUser}/{elementoGraficoCods}", objParametriServer);
                        }
                        catch (Exception ex)
                        {
                            LogError($"Errore durante il calcolo del centroide per {pivaSuperUser}/{elementoGraficoCods}.", objParametriServer, ex);
                        }
                    }
                }

                sw.Stop();
                LogInformation($"Elaborazione del batch di {recordsList.Count} record terminata in {sw.ElapsedMilliseconds} ms.", objParametriServer);
                
                return true;
            }
            catch (Exception ex)
            {
                LogError("Errore nel job di calcolo centroidi.", objParametriServer, ex);
                return false;
                throw;
            }
        }

        #region Metodi Privati

     
        private async Task<bool> SaveCentroidAsync(string pivaSuperUser, int layerCod, int[] elementoGraficoCods, AgronicaCoreParametriServer objParametriServer)
        {
            try
            {
                // Legge la geometria WKT del poligono 
                (int, string)[] poligonoWkts = await _gisClusteringDal.GetElementoGraficoWktsAsync(pivaSuperUser, elementoGraficoCods, objParametriServer);
                var options = new ParallelOptions
                {
                    MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount / 2)
                };

                var result = new ConcurrentBag<(int, string)>();
                Parallel.ForEach(poligonoWkts, options, data =>
                {
                    var (elementoGraficoCod, poligonoWkt) = data;
                    if (string.IsNullOrWhiteSpace(poligonoWkt))
                    {
                        LogWarning(
                            $"Geometria WKT non trovata o vuota per {pivaSuperUser}/{elementoGraficoCod}, impossibile calcolare il centroide.",
                            objParametriServer);
                        return;
                    }

                    // Chiama il metodo per il calcolo matematico.
                    string centroideWkt = CaculateCentroid(poligonoWkt);
                    if (string.IsNullOrWhiteSpace(centroideWkt))
                    {
                        LogWarning(
                            $"Calcolo del centroide fallito per {pivaSuperUser}/{elementoGraficoCod}. Probabile WKT malformato.",
                            objParametriServer);
                        return;
                    }

                    //Se il calcolo ha successo,
                    // questo metodo aggiorna la riga, salvando il WKT e impostando lo Stato a 1 (calacolato).
                    result.Add((elementoGraficoCod, centroideWkt));
                });

                await _gisClusteringDal.UpdateClusteringRecordsAsync(pivaSuperUser, layerCod, result.ToArray(), objParametriServer.UsernameOperazione, objParametriServer);
                return true;
            }
            catch (Exception ex)
            {
                LogError("Errore");
                return false;
            }
        }

        /// Metodo che esegue solo il calcolo matematico del centroide
        /// <returns>La stringa WKT del punto centroide, o null in caso di errore.</returns>
        private string CaculateCentroid(string wktEntity)
        {
            
            var wkt = wktEntity.Trim();
            string? result;
            if (wkt.StartsWith("POLYGON", StringComparison.OrdinalIgnoreCase))
            {
                var inner = wkt[(wkt.IndexOf("((", StringComparison.Ordinal) + 2)..];
                inner = inner[..inner.IndexOf("))", StringComparison.Ordinal)];

                var firstCoord = inner.Split(',')[0].Trim();

                result = FormatAsPoint(firstCoord);
            }
            else if (wkt.StartsWith("MULTIPOLYGON", StringComparison.OrdinalIgnoreCase))
            {
                var mp = wkt[(wkt.IndexOf("((", StringComparison.Ordinal) + 1)..].Trim();
                if (mp.StartsWith("(("))
                {
                    mp = mp[2..];
                }

                var inner = mp[..mp.IndexOf("))", StringComparison.Ordinal)];

                var firstCoord = inner.Split(',')[0].Trim();

                result = FormatAsPoint(firstCoord);
            }
            else
            {
                LogWarning($"Tipo di geometria non supportato per il calcolo del centroide, WKT: {wktEntity}");
                result = null;
            }

            return result;

            string FormatAsPoint(string xy)
            {
                // xy is something like "10 20"
                var parts = xy.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length < 2)
                    return null;

                return $"POINT ({parts[0]} {parts[1]})";
            }
        }

        #endregion
    }
    
    
}
