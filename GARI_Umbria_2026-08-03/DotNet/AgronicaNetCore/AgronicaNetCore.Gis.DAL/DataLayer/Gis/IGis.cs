using AgronicaNetCore.Base.Models;
using System.Data;
using AgronicaCoreDTOStd.InData.Gis;

namespace AgronicaNetCore.Gis.DAL.DataLayer.Gis
{
    public interface IGis
    {
        Task<DataTable> LeggiGIS_ProcessingAlgorithms_Cleaning_AlgorithmAsync(
            AgronicaCoreParametriServer objParametriServer);

        public Task<bool> UpdateLayerTranslation(string layerCod, string tipologiaLayerCod, string linguaCod,
            string traduzione, AgronicaCoreParametriServer objParametriServer);

        public Task<bool> UpdateLayerLabelTranslation(string layerCod, string tipologiaLayerCod,
            string tipologiaLayerStructCod, string linguaCod, string traduzione,
            AgronicaCoreParametriServer objParametriServer);
    }
}