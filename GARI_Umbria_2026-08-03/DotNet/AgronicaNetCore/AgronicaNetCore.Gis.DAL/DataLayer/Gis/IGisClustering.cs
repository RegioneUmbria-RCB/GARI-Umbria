using AgronicaNetCore.Base.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Gis.DAL.DataLayer.Gis
{
    
    public interface IGisClustering
    {

        /// Legge la geometria in formato WKT di unElementoGrafico dalla tabella principale GIS_ElementiGrafici.
        /// <returns>Una stringa contenente la geometria WKT, o null se non trovata.</returns>
        Task<(int, string)[]> GetElementoGraficoWktsAsync(string pivaSuperUser, int[] elementoGraficoCods, AgronicaCoreParametriServer objParametriServer);

        /// Aggiorna un record nella tabella di clustering (GIS_ElementiGrafici_Clustering) con il centroide calcolato.
        /// Imposta lo Stato a 1 (Calcolato).
        /// <returns>True se l'aggiornamento ha avuto successo.</returns>
        Task<bool> UpdateClusteringRecordAsync(string pivaSuperUser, int layerCod, int elementoGraficoCod, string centroideWkt, string username, AgronicaCoreParametriServer objParametriServer);

        /// Aggiorna i record nella tabella di clustering (GIS_ElementiGrafici_Clustering) con i centroidi calcolati.
        /// Imposta lo Stato a 1 (Calcolato).
        /// <returns>True se l'aggiornamento ha avuto successo.</returns>
        Task<bool> UpdateClusteringRecordsAsync(string pivaSuperUser, int layerCod, (int, string)[] elements, string username, AgronicaCoreParametriServer objParametriServer);

        /// Recupera dal database un elenco di tutti i record di clustering che devono ancora essere processati (Stato = 0).
        /// <returns>Un DataTable contenente le chiavi primarie dei record da processare.</returns>
        Task<DataTable> GetClusteringRecordsToProcessAsync(string pivaSuperUser, int? layerCod, int? elementoGraficoCod, int? batchSize, AgronicaCoreParametriServer objParametriServer);
    }
}
