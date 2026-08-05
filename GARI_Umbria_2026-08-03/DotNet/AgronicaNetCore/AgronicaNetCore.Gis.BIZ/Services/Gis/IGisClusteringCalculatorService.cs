using AgronicaNetCore.Base.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Gis.BIZ.Services.Gis
{
    public interface IGisClusteringCalculatorService
    {
        /// Avvia il processo per calcolare i centroidi per tutti gli elementi con (Stato = 0).
        Task<bool> StartJob_SaveCentroidsAsync(string pivaSuperUser, int? layerCod, int? elementoGraficoCod, int? batchSize, AgronicaCoreParametriServer objParametriServer);
    }
}

