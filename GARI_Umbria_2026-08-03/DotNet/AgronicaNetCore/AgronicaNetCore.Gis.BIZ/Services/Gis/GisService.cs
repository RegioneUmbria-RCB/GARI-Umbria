using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Gis.DAL.DataLayer.Gis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Data;
using AgronicaCoreDTOStd.InData.Gis;
using AgronicaNetCore.Gis.BIZ.Resources;

namespace AgronicaNetCore.Gis.BIZ.Services.Gis
{
    public class GisService : BaseServiceGisBIZ, IGisService
    {
        private readonly IGis _gisDAL;

        public GisService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _gisDAL = _serviceProvider.GetRequiredService<IGis>();            
        }

        public async Task<DataTable> LeggiGIS_ProcessingAlgorithms_Cleaning_AlgorithmAsync(AgronicaCoreParametriServer objParametriServer)
        {
            DataTable dt;
            try
            {
                dt = await _gisDAL.LeggiGIS_ProcessingAlgorithms_Cleaning_AlgorithmAsync(objParametriServer);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
            return dt;
        }

        public async Task<bool> UpdateLayerTranslations(TraduzioniLayer_In input, AgronicaCoreParametriServer objParametriServer)
        {
            try
            {
                foreach (var traduzione in input.Traduzioni)
                {
                    if (!await _gisDAL.UpdateLayerTranslation(
                            input.Layer_Cod,
                            input.TipologiaLayer_Cod,
                            traduzione.Lingua_Cod,
                            traduzione.Traduzione,
                            objParametriServer))
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }
        

        public async Task<bool> UpdateLayerLabelTranslations(TraduzioniLayerLabel_In input, AgronicaCoreParametriServer objParametriServer)
        {
            try
            {
                foreach (var traduzione in input.Traduzioni)
                {
                    if (!await _gisDAL.UpdateLayerLabelTranslation(
                            input.Layer_Cod,
                            input.TipologiaLayer_Cod,
                            input.TipologiaLayer_struct_Cod,
                            traduzione.Lingua_Cod,
                            traduzione.Traduzione,
                            objParametriServer))
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }
    }
}
