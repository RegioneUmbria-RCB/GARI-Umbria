using AgronicaNetCore.Base.Models;
using System.Data;
using AgronicaCoreDTOStd.InData.Gis;

namespace AgronicaNetCore.Gis.BIZ.Services.Gis
{
    public interface IGisService
    {
        Task<DataTable> LeggiGIS_ProcessingAlgorithms_Cleaning_AlgorithmAsync(AgronicaCoreParametriServer objParametriServer);
        Task<bool> UpdateLayerTranslations(TraduzioniLayer_In input, AgronicaCoreParametriServer objParametriServer);
        Task<bool> UpdateLayerLabelTranslations(TraduzioniLayerLabel_In input, AgronicaCoreParametriServer objParametriServer);
    }
}
