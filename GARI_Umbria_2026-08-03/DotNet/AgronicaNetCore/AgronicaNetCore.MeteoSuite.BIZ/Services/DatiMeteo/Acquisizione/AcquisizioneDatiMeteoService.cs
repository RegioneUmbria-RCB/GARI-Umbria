using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Base.DataLayer.Security;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MeteoSuite.BIZ.Resources;
using AgronicaNetCore.MeteoSuite.BIZ.Services.DatiMeteo.Acquisizione;
using AgronicaNetCore.MeteoSuite.BIZ.Services.DatiMeteo.Exceptions;
using AgronicaNetCore.MeteoSuite.BIZ.Services.Stazioni.Exceptions;
using AgronicaNetCore.MeteoSuite.DAL.DataLayer.DatiMeteo.Models;
using AgronicaNetCore.MeteoSuite.DAL.DataLayer.Stazioni;
using InData.Engine.MeteoSuite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using OutData.Engine.MeteoSuite;
using System.Globalization;
using System.Net.Http.Headers;

namespace AgronicaNetCore.MeteoSuite.BIZ.Services.Engine.DatiMeteo.Acquisizione
{
    public class AcquisizioneDatiMeteoService : BaseMeteoSuiteBIZService, IAcquisizioneDatiMeteoService
    {
        private const string ChiaveUrlEngine = "urlEngine_MeteoSuite";
        private const string ChiaveApiKey = "apiKeyEngine_MeteoSuite";
        private const string ChiaveTenantName = "tenantNameEngine_MeteoSuite";

        private const int TimeoutSeconds = 60;
        private const string HypermeteoProvider = "HYPERMETEO";

        private readonly ISecurityLayerDAL _securityLayerDal;
        private readonly HttpClient _httpClient;
        private readonly IMeteoSuiteQueryDeviceService _openqueryService;
        private readonly IMeteoSuiteOnboardingDevicesService _onboardingService;
        private readonly IValidazioneDatiMeteoService _validationService;
        private readonly ITbRerStazioniDAL _rerStazioniDALService;
        private readonly ITbRerQuadrantiDAL _rerQuadrantiDALService;

        // Riferimento sensori orari
        private const string LeafWetnessSensorCodeH = "LEAFWET_HOURLY";
        private const string PrecipitationSensorCodeH = "PREC_HOURLY";
        private const string HumiditySensorCodeH = "RH2M_HOURLY";
        private const string MoistureSensorCodeH = "MOISTURE_HOURLY";
        private const string SolarRadiationSensorCodeH = "SSWTOT_HOURLY";
        private const string TemperatureSensorCodeH = "TC2M_HOURLY";
        private const string WindDirectionSensorCodeH = "WDIR10M_HOURLY";
        private const string WindSpeedSensorCodeH = "WSPD10M_HOURLY";
        // Riferimento sensori giornalieri
        private const string ET0SensorCodeD = "ET0-DAILY";
        private const string FrostProbabilityCodeD = "ET0-DAILY";
        private const string GustSensorCodeD = "GUST-DAILY";
        private const string HailProbabilitySensorCodeD = "HAIL-DAILY";
        private const string HeavyRainProbabilitySensorCodeD = "HEAVY-PREC-PROB-DAILY";
        private const string HightTemperatureProbabilitySensorCodeD = "HIGH-TC2M-PROB-DAILY";
        private const string LeafWetnessSensorCodeD = "LEAFWET-DAILY";
        private const string PrecipitationSensorCodeD = "PREC-DAILY";
        private const string HumidityMaxSensorCodeD = "RH2M-MAX-DAILY";
        private const string HumidityMinSensorCodeD = "RH2M-MIN-DAILY";
        private const string HumidityMeanSensorCodeD = "RH2M-MEAN-DAILY";
        private const string MoistureSensorCodeD = "MOISTURE-DAILY";
        private const string SolarRadiationSensorCodeD = "SSWTOT-DAILY";
        private const string TemperatureMaxSensorCodeD = "TMAX-DAILY";
        private const string TemperatureMinSensorCodeD = "TMIN-DAILY";
        private const string TemperatureMeanSensorCodeD = "TMEAN-DAILY";
        private const string WindDirectionSensorCodeD = "WDIR10M-DAILY";
        private const string WindSpeedSensorCodeD = "WSPD10M-DAILY";

        private static readonly string[] SensoriOrari = {
            LeafWetnessSensorCodeH,
            PrecipitationSensorCodeH,
            HumiditySensorCodeH,
            SolarRadiationSensorCodeH,
            TemperatureSensorCodeH,
            WindDirectionSensorCodeH,
            WindSpeedSensorCodeH
        };
        private static readonly string[] SensoriGiornalieri = {
            ET0SensorCodeD,
            FrostProbabilityCodeD,
            GustSensorCodeD,
            HailProbabilitySensorCodeD,
            HeavyRainProbabilitySensorCodeD,
            HightTemperatureProbabilitySensorCodeD,
            HightTemperatureProbabilitySensorCodeD,
            LeafWetnessSensorCodeD,
            PrecipitationSensorCodeD,
            HumidityMaxSensorCodeD,
            HumidityMinSensorCodeD,
            HumidityMeanSensorCodeD,
            SolarRadiationSensorCodeD,
            TemperatureMaxSensorCodeD,
            TemperatureMinSensorCodeD,
            TemperatureMeanSensorCodeD,
            WindDirectionSensorCodeD,
            WindSpeedSensorCodeD,
        };

        /// <summary>
        /// Initializes a new instance of the AcquisizioneDatiMeteorologiciIbridaService.
        /// </summary>
        /// <param name="provider">The service provider for dependency resolution</param>
        /// <param name="localizer">The string localizer for messages</param>
        public AcquisizioneDatiMeteoService(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer)
            : base(provider, localizer)
        {
            _securityLayerDal = provider.GetRequiredService<ISecurityLayerDAL>();
            _httpClient = provider.GetRequiredService<HttpClient>();
            _openqueryService = provider.GetRequiredService<IMeteoSuiteQueryDeviceService>();
            _onboardingService = provider.GetRequiredService<IMeteoSuiteOnboardingDevicesService>();
            _validationService = provider.GetRequiredService<IValidazioneDatiMeteoService>();
            _rerStazioniDALService = provider.GetRequiredService<ITbRerStazioniDAL>();
            _rerQuadrantiDALService = provider.GetRequiredService<ITbRerQuadrantiDAL>();
        }

        public async Task<AcquisizioneDatiMeteoResponse> AcquisisciDatiMeteoOrariAsync(
            AcquisizioneDatiMeteoRequest request, 
            AgronicaCoreParametriServer objParametriServer, 
            AgronicaCoreParametriSuperServer objParametriSuperServer, 
            CancellationToken cancellationToken = default)
        {
            return await AcquisisciDatiMeteoAsync(true, request, objParametriServer, objParametriSuperServer, cancellationToken);
        }

        public async Task<AcquisizioneDatiMeteoResponse> AcquisisciDatiMeteoGiornalieriAsync(
            AcquisizioneDatiMeteoRequest request, 
            AgronicaCoreParametriServer objParametriServer, 
            AgronicaCoreParametriSuperServer objParametriSuperServer, 
            CancellationToken cancellationToken = default)
        {
            return await AcquisisciDatiMeteoAsync(false, request, objParametriServer, objParametriSuperServer, cancellationToken);
        }

        private async Task<AcquisizioneDatiMeteoResponse> AcquisisciDatiMeteoAsync(
            bool datiOrari,
            AcquisizioneDatiMeteoRequest request,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default)
        {
            if (request is null) throw new ArgumentNullException(nameof(request));
            if (objParametriServer is null) throw new ArgumentNullException(nameof(objParametriServer));
            if (objParametriSuperServer is null) throw new ArgumentNullException(nameof(objParametriSuperServer));

            string[] sensori = datiOrari ? SensoriOrari : SensoriGiornalieri;

            var meteoStation = await GetMeteoStation(request, objParametriServer, objParametriSuperServer, cancellationToken);

            if (meteoStation == null)
            {
                throw new MeteoStationNotFoundException();
            }

            if (string.IsNullOrEmpty(request.StationCode) && meteoStation != null)
            {
                request.StationCode = meteoStation.providerReference;
            }

            var resultDatiMeteo = await LeggiDatiMeteoDaOpenQuery(request, sensori, objParametriServer, objParametriSuperServer, cancellationToken);

            List<MeteoSuiteQueryWeatherDataPointDto>? datiTemperaturaOrari = null;
            if (!datiOrari && (request.SogliaTermica.HasValue || request.SogliaFabbisognoFreddo.HasValue))
            {
                datiTemperaturaOrari = await LeggiDatiMeteoDaOpenQuery(request, new string[] { TemperatureSensorCodeH }, objParametriServer, objParametriSuperServer, cancellationToken);
            }

            DateTime lastUpdate = resultDatiMeteo != null && resultDatiMeteo.Count > 0
                ? resultDatiMeteo.Max(x => x.PointInTime)
                : DateTime.MinValue;

            AcquisizioneDatiMeteoResponse result = new AcquisizioneDatiMeteoResponse()
            {
                Stazione = meteoStation?.description,
                UltimoAggiornamento = lastUpdate,
                Dati = new List<MeteoDataPuntuale>(),
                Sensori = new List<MeteoDataSensore>(),
                Riepilogo = new List<MeteoDataRiepilogoDato>(),
                RiepilogoSensori = new List<MeteoDataRiepilogoDatoSensore>()
            };

            if (resultDatiMeteo != null)
            {
                foreach (var datoMeteo in resultDatiMeteo)
                {
                    var datoMeteoPuntuale = new MeteoDataPuntuale()
                    {
                        DataOra = datoMeteo.PointInTime,
                        ValoriSensori = new List<MeteoDataPuntualeSensore>()
                    };

                    foreach (var datoMeteoSensore in datoMeteo.Sensors)
                    {
                        datoMeteoPuntuale.ValoriSensori.Add(new MeteoDataPuntualeSensore()
                        {
                            Sensore = datoMeteoSensore.Name,
                            Valore = datoMeteoSensore.Value
                        });

                        AddSensorToList(meteoStation, datoMeteoSensore.Name, result);

                        if ((request.SogliaTermica.HasValue || request.SogliaFabbisognoFreddo.HasValue) &&
                            (datoMeteoSensore.Name.Equals(TemperatureSensorCodeH) || datoMeteoSensore.Name.Equals(TemperatureMeanSensorCodeD)))
                        {
                            var valoreTemp = datiOrari
                                ? datoMeteoSensore.Value
                                : datiTemperaturaOrari?.Where(x => x.PointInTime.Date == datoMeteo.PointInTime.Date).Sum(y => y.Sensors.Where(z => z.Name.Equals(TemperatureSensorCodeH)).Select(v => v.Value).FirstOrDefault());
                            
                            if (request.SogliaTermica.HasValue)
                            {
                                float sogliaTermicaValue = 0;
                                if (datiOrari)
                                {
                                    sogliaTermicaValue = (datoMeteoSensore.Value > (float)request.SogliaTermica.Value) 
                                        ? (datoMeteoSensore.Value - (float)request.SogliaTermica.Value) 
                                        : 0;
                                }
                                else if (datiTemperaturaOrari != null)
                                {
                                    sogliaTermicaValue = datiTemperaturaOrari
                                        .Where(x => x.PointInTime.Date == datoMeteo.PointInTime.Date)
                                        .Sum(x => x.Sensors
                                            .Where(y => y.Name.Equals(TemperatureSensorCodeH) && y.Value > (float)request.SogliaTermica.Value)
                                            .Select(v => v.Value - (float)request.SogliaTermica.Value)
                                            .FirstOrDefault());

                                }

                                datoMeteoPuntuale.ValoriSensori.Add(new MeteoDataPuntualeSensore()
                                {
                                    Sensore = "t_sum",
                                    Valore = sogliaTermicaValue
                                });
                            }

                            if (request.SogliaFabbisognoFreddo.HasValue && !datiOrari)
                            {
                                float sogliaFabbFreddoValue = 0;
                                if (datiTemperaturaOrari != null)
                                {
                                    sogliaFabbFreddoValue = datiTemperaturaOrari
                                            .Where(x => x.PointInTime.Date == datoMeteo.PointInTime.Date)
                                            .Sum(x => x.Sensors
                                                .Where(y => y.Name.Equals(TemperatureSensorCodeH) && y.Value < (float)request.SogliaFabbisognoFreddo.Value)
                                                .Select(v => 1)
                                                .FirstOrDefault());
                                }

                                datoMeteoPuntuale.ValoriSensori.Add(new MeteoDataPuntualeSensore()
                                {
                                    Sensore = "t_cnt_freddo",
                                    Valore = sogliaFabbFreddoValue
                                });
                            }
                        }
                    }

                    result.Dati.Add(datoMeteoPuntuale);
                }
            }

            if (request.SogliaTermica.HasValue || (request.SogliaFabbisognoFreddo.HasValue && !datiOrari))
            {
                var tSens = datiOrari
                    ? result.Sensori.Where(x => x.TipoSensore == "TC2M").FirstOrDefault()
                    : result.Sensori.Where(x => x.TipoSensore == "TMEAN").FirstOrDefault();

                if (tSens != null && request.SogliaTermica.HasValue)
                {
                    result.Sensori.Add(new MeteoDataSensore()
                    {
                        SensoreId = tSens.SensoreId,
                        Sensore = "SD_" + tSens.SensoreId + "_t_sum",
                        Etichetta = tSens.Etichetta,
                        TipoSensore = tSens.TipoSensore,
                        UM = tSens.UM,
                        FunAggreg = "t_sum",
                        ProviderReference = "t_sum"
                    });
                }

                if (tSens != null && request.SogliaFabbisognoFreddo.HasValue && !datiOrari)
                {
                    result.Sensori.Add(new MeteoDataSensore()
                    {
                        SensoreId = tSens.SensoreId,
                        Sensore = "SD_" + tSens.SensoreId + "_t_cnt_freddo",
                        Etichetta = tSens.Etichetta,
                        TipoSensore = tSens.TipoSensore,
                        UM = tSens.UM,
                        FunAggreg = "t_cnt_freddo",
                        ProviderReference = "t_cnt_freddo"
                    });
                }
            }

            foreach (var sensore in result.Sensori)
            {
                float riepilogoValue = 0;
                int count = 0;
                float lastValue = 0;
                DateTime lastDateTime = DateTime.MinValue;

                string funAggreg = sensore.FunAggreg.ToLower();

                foreach (var dato in result.Dati)
                {
                    var datosensore = dato.ValoriSensori.Where(x => x.Sensore.Equals(sensore.ProviderReference)).FirstOrDefault();
                    if (datosensore != null)
                    {
                        switch (funAggreg)
                        {
                            case "avg":
                            case "avg_vec":
                            case "sum":
                            case "t_sum":
                            case "t_cnt_freddo":
                                riepilogoValue += datosensore.Valore;
                                break;
                            case "max":
                                riepilogoValue = Math.Max(riepilogoValue, datosensore.Valore);
                                break;
                            case "min":
                                riepilogoValue = Math.Min(riepilogoValue, datosensore.Valore);
                                break;
                        }
                        count++;
                        lastValue = datosensore.Valore;
                        lastDateTime = dato.DataOra;
                    }
                }
                if (funAggreg.Equals("avg") || funAggreg.Equals("avg_vec"))
                {
                    riepilogoValue = riepilogoValue / count;
                }

                if (sensore.TipoSensore.Equals("TMEAN") || sensore.TipoSensore.Equals("TC2M") || sensore.TipoSensore.Equals("PREC"))
                {
                    result.Riepilogo.Add(new MeteoDataRiepilogoDato()
                    {
                        Sensore = sensore.Etichetta,
                        FunAggreg = funAggreg,
                        Valore = riepilogoValue,
                        UM = sensore.UM
                    });
                }

                result.RiepilogoSensori.Add(new MeteoDataRiepilogoDatoSensore()
                {
                    Sensore = sensore.Sensore,
                    Etichetta = sensore.Etichetta,
                    UM = sensore.UM,
                    DataOra = lastDateTime,
                    Val_Last = lastValue,
                    Val_Avg = funAggreg.ToLower().Equals("avg") ? riepilogoValue : null,
                    Val_Min = funAggreg.ToLower().Equals("min") ? riepilogoValue : null,
                    Val_Max = funAggreg.ToLower().Equals("max") ? riepilogoValue : null,
                    Val_Sum = funAggreg.ToLower().Equals("sum") ? riepilogoValue : null,
                    Val_Dir = funAggreg.ToLower().Equals("avg_vec") ? riepilogoValue : null,
                });
            }

            return result;
        }

        private void AddSensorToList(MeteoSuiteOnboardingDeviceDto? meteoStation,
                                     string chiaveSensore, 
                                     AcquisizioneDatiMeteoResponse result)
        {
            if (meteoStation != null && meteoStation.sensors != null)
            {
                var stationSensor = meteoStation.sensors.Where(x => x.providerReference.Equals(chiaveSensore)).FirstOrDefault();
                if (stationSensor != null && !result.Sensori.Any(x => x.SensoreId == stationSensor.id))
                {
                    string sensPrefix = (stationSensor.sensorType.EndsWith("MIN") || stationSensor.sensorType.EndsWith("MAX")) ? "SD_" : "S_";
                    result.Sensori.Add(new MeteoDataSensore()
                    {
                        SensoreId = stationSensor.id,
                        Sensore = sensPrefix + stationSensor.id,
                        Etichetta = stationSensor.description,
                        TipoSensore = stationSensor.sensorType,
                        UM = stationSensor.uom,
                        FunAggreg = stationSensor.aggregationFunc,
                        ProviderReference = stationSensor.providerReference
                    });
                }
            }
        }

        public async Task<AcquisizioneDatiMeteoPioggeOrariResponse> AcquisisciDatiMeteoPioggeOrariAsync(
            AcquisizioneDatiMeteoRequest request,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default)
        {
            string[] sensori = sensori = new string[] 
            { 
                LeafWetnessSensorCodeH, 
                PrecipitationSensorCodeH, 
                HumiditySensorCodeH, 
                TemperatureSensorCodeH 
            };
            
            var meteoStation = await GetMeteoStation(request, objParametriServer, objParametriSuperServer, cancellationToken);

            if (string.IsNullOrEmpty(request.StationCode) && meteoStation != null)
            {
                request.StationCode = meteoStation.providerReference;
            }

            var resultDatiMeteo = await LeggiDatiMeteoDaOpenQuery(request, sensori, objParametriServer, objParametriSuperServer, cancellationToken);

            var result = new AcquisizioneDatiMeteoPioggeOrariResponse()
            {
                MeteoData = new List<DatiMeteoPioggeOrariDto>(),
                Sensors = new DatiMeteoPioggiaSensori()
                {
                    SensoreBagnaturaFogliare = new DatiMeteoPioggiaSensore(),
                    SensorePrecipiatazioni = new DatiMeteoPioggiaSensore(),
                    SensoreTemperatura = new DatiMeteoPioggiaSensore(),
                    SensoreUmiditaRelativa = new DatiMeteoPioggiaSensore(),
                    SensoriUmiditaTerreno = new List<DatiMeteoPioggiaSensore>()
                }
            };

            foreach (var datoMeteo in resultDatiMeteo)
            {
                var datoPiogge = new DatiMeteoPioggeOrariDto();
                datoPiogge.DataOra = datoMeteo.PointInTime;
                datoPiogge.UmiditaTerreno = new List<DatiMeteoPioggeUmiditaTerreno>();
                DatiMeteoPioggiaSensore? sensore;

                foreach (var datoMeteoSensore in datoMeteo.Sensors)
                {
                    sensore = null;

                    switch (datoMeteoSensore.Name)
                    {
                        case LeafWetnessSensorCodeH:
                            datoPiogge.BagnaturaFogliare = datoMeteoSensore.Value;
                            sensore = result.Sensors.SensoreBagnaturaFogliare;
                            break;

                        case PrecipitationSensorCodeH:
                            datoPiogge.Precipiatazioni = datoMeteoSensore.Value;
                            sensore = result.Sensors.SensorePrecipiatazioni;
                            break;

                        case HumiditySensorCodeH:
                            datoPiogge.UmiditaRelativa = datoMeteoSensore.Value;
                            sensore = result.Sensors.SensoreUmiditaRelativa;
                            break;

                        //case MoistureSensorCodeH:
                        //    datoPiogge.UmiditaTerreno = datoMeteoSensore.Value;
                        //    sensore = result.Sensors.SensoreUmiditaTerreno;
                        //    break;

                        case TemperatureSensorCodeH:
                            datoPiogge.Temperatura = datoMeteoSensore.Value;
                            sensore = result.Sensors.SensoreTemperatura;
                            break;

                        default:
                            if (datoMeteoSensore.Name.StartsWith("MOISTURE"))
                            {
                                DatiMeteoPioggeUmiditaTerreno? datoUT = datoPiogge.UmiditaTerreno.Where(x => x.Sensore.Equals(datoMeteoSensore.Name)).FirstOrDefault();
                                if (datoUT == null)
                                {
                                    datoUT = new DatiMeteoPioggeUmiditaTerreno();
                                    datoUT.Sensore = datoMeteoSensore.Name;
                                    datoPiogge.UmiditaTerreno.Add(datoUT);
                                }
                                datoUT.Valore = datoMeteoSensore.Value;

                                sensore = result.Sensors.SensoriUmiditaTerreno.Where(x => x.Sensore.Equals(datoMeteoSensore.Name)).FirstOrDefault();
                                if (sensore == null)
                                {
                                    sensore = new DatiMeteoPioggiaSensore();
                                    result.Sensors.SensoriUmiditaTerreno.Add(sensore);
                                }
                            }
                            break;
                    }

                    if (sensore != null && string.IsNullOrEmpty(sensore.Sensore) && meteoStation != null && meteoStation.sensors != null)
                    {
                        var stationSensor = meteoStation.sensors.Where(x => x.providerReference.Equals(datoMeteoSensore.Name)).FirstOrDefault();
                        if (stationSensor != null)
                        {
                            sensore.Sensore = stationSensor.id.ToString();
                            sensore.Etichetta = stationSensor.description;
                            sensore.TipoSensore = stationSensor.sensorType;
                            sensore.UM = stationSensor.uom;
                            sensore.FunAggreg = stationSensor.aggregationFunc;
                        }
                    }
                }

                result.MeteoData.Add(datoPiogge);
            }

            return result;
        }

        public async Task<AcquisizioneDatiMeteoPioggeGiornalieriResponse> AcquisisciDatiMeteoPioggeGiornalieriAsync(
            AcquisizioneDatiMeteoRequest request,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            bool leggiOrePiogge = false,
            CancellationToken cancellationToken = default)
        {
            string[] sensori = leggiOrePiogge 
                ? new string[] 
                {
                    LeafWetnessSensorCodeD, 
                    PrecipitationSensorCodeH, 
                    HumidityMeanSensorCodeD, 
                    HumidityMinSensorCodeD, 
                    HumidityMaxSensorCodeD,
                    MoistureSensorCodeD,
                    TemperatureMeanSensorCodeD, 
                    TemperatureMinSensorCodeD, 
                    TemperatureMaxSensorCodeD 
                }
                : new string[]
                {
                    LeafWetnessSensorCodeD,
                    PrecipitationSensorCodeD,
                    HumidityMeanSensorCodeD,
                    HumidityMinSensorCodeD,
                    HumidityMaxSensorCodeD,
                    MoistureSensorCodeD,
                    TemperatureMeanSensorCodeD,
                    TemperatureMinSensorCodeD,
                    TemperatureMaxSensorCodeD
                };

            var meteoStation = await GetMeteoStation(request, objParametriServer, objParametriSuperServer, cancellationToken);

            if (string.IsNullOrEmpty(request.StationCode) && meteoStation != null)
            {
                request.StationCode = meteoStation.providerReference;
            }

            var resultDatiMeteo = await LeggiDatiMeteoDaOpenQuery(request, sensori, objParametriServer, objParametriSuperServer, cancellationToken);

            var result = new AcquisizioneDatiMeteoPioggeGiornalieriResponse()
            {
                MeteoData = new List<DatiMeteoPioggeGiornalieriDto>(),
                Sensors = new DatiMeteoPioggiaSensori()
                {
                    SensoreBagnaturaFogliare = new DatiMeteoPioggiaSensore(),
                    SensorePrecipiatazioni = new DatiMeteoPioggiaSensore(),
                    SensoreTemperatura = new DatiMeteoPioggiaSensore(),
                    SensoreUmiditaRelativa = new DatiMeteoPioggiaSensore(),
                    SensoriUmiditaTerreno = new List<DatiMeteoPioggiaSensore>()
                }
            };

            Dictionary<DateTime, float> dictSumPrecH = new Dictionary<DateTime, float>();
            Dictionary<DateTime, int> dictOrePioggia = new Dictionary<DateTime, int>();

            foreach (var datoMeteo in resultDatiMeteo)
            {
                var datoPiogge = new DatiMeteoPioggeGiornalieriDto();
                datoPiogge.DataOra = datoMeteo.PointInTime;
                datoPiogge.UmiditaTerreno = new List<DatiMeteoPioggeUmiditaTerreno>();
                DatiMeteoPioggiaSensore? sensore;
                bool datoGLetto = false;

                foreach (var datoMeteoSensore in datoMeteo.Sensors)
                {
                    sensore = null;

                    switch (datoMeteoSensore.Name)
                    {
                        case LeafWetnessSensorCodeD:
                            datoPiogge.BagnaturaFogliare = datoMeteoSensore.Value;
                            sensore = result.Sensors.SensoreBagnaturaFogliare;
                            datoGLetto = true;
                            break;

                        case PrecipitationSensorCodeD:
                            datoPiogge.PrecipiatazioniCumulate = datoMeteoSensore.Value;
                            sensore = result.Sensors.SensorePrecipiatazioni;
                            datoGLetto = true;
                            break;

                        case PrecipitationSensorCodeH:
                            if (datoMeteoSensore.Value > 0)
                            {
                                if (dictSumPrecH.ContainsKey(datoMeteo.PointInTime.Date))
                                {
                                    dictSumPrecH[datoMeteo.PointInTime.Date] += datoMeteoSensore.Value;
                                    dictOrePioggia[datoMeteo.PointInTime.Date] += 1;
                                }
                                else
                                {
                                    dictSumPrecH[datoMeteo.PointInTime.Date] = datoMeteoSensore.Value;
                                    dictOrePioggia[datoMeteo.PointInTime.Date] = 1;
                                }
                            }
                            break;

                        case HumidityMinSensorCodeD:
                            datoPiogge.UmiditaRelativaMin = datoMeteoSensore.Value;
                            sensore = result.Sensors.SensoreUmiditaRelativa;
                            datoGLetto = true;
                            break;

                        case HumidityMaxSensorCodeD:
                            datoPiogge.UmiditaRelativaMax = datoMeteoSensore.Value;
                            sensore = result.Sensors.SensoreUmiditaRelativa;
                            datoGLetto = true;
                            break;

                        case HumidityMeanSensorCodeD:
                            datoPiogge.UmiditaRelativaMedia = datoMeteoSensore.Value;
                            sensore = result.Sensors.SensoreUmiditaRelativa;
                            datoGLetto = true;
                            break;

                        case TemperatureMinSensorCodeD:
                            datoPiogge.TemperaturaMin = datoMeteoSensore.Value;
                            sensore = result.Sensors.SensoreTemperatura;
                            datoGLetto = true;
                            break;

                        //case MoistureSensorCodeD:
                        //    datoPiogge.UmiditaTerreno = datoMeteoSensore.Value;
                        //    sensore = result.Sensors.SensoreUmiditaTerreno;
                        //    datoGLetto = true;
                        //    break;

                        case TemperatureMaxSensorCodeD:
                            datoPiogge.TemperaturaMax = datoMeteoSensore.Value;
                            sensore = result.Sensors.SensoreTemperatura;
                            datoGLetto = true;
                            break;

                        case TemperatureMeanSensorCodeD:
                            datoPiogge.TemperaturaMedia = datoMeteoSensore.Value;
                            sensore = result.Sensors.SensoreTemperatura;
                            datoGLetto = true;
                            break;

                        default:
                            if (datoMeteoSensore.Name.StartsWith("MOISTURE"))
                            {
                                DatiMeteoPioggeUmiditaTerreno? datoUT = datoPiogge.UmiditaTerreno.Where(x => x.Sensore.Equals(datoMeteoSensore.Name)).FirstOrDefault();
                                if (datoUT == null)
                                {
                                    datoUT = new DatiMeteoPioggeUmiditaTerreno();
                                    datoPiogge.UmiditaTerreno.Add(datoUT);
                                }
                                datoUT.Valore = datoMeteoSensore.Value;

                                sensore = result.Sensors.SensoriUmiditaTerreno.Where(x => x.Sensore.Equals(datoMeteoSensore.Name)).FirstOrDefault();
                                if (sensore == null)
                                {
                                    sensore = new DatiMeteoPioggiaSensore();
                                    result.Sensors.SensoriUmiditaTerreno.Add(sensore);
                                }
                            }
                            break;
                    }

                    if (sensore != null && string.IsNullOrEmpty(sensore.Sensore) && meteoStation != null && meteoStation.sensors != null)
                    {
                        var stationSensor = meteoStation.sensors.Where(x => x.providerReference.Equals(datoMeteoSensore.Name)).FirstOrDefault();
                        if (stationSensor != null)
                        {
                            sensore.Sensore = stationSensor.id.ToString();
                            sensore.Etichetta = stationSensor.description;
                            sensore.TipoSensore = stationSensor.sensorType;
                            sensore.UM = stationSensor.uom;
                            sensore.FunAggreg = stationSensor.aggregationFunc;
                        }
                    }
                }

                if (datoGLetto)
                {
                    result.MeteoData.Add(datoPiogge);
                }
            }

            if (leggiOrePiogge)
            {
                foreach (var meteoEntry in result.MeteoData)
                {
                    if (dictSumPrecH.ContainsKey(meteoEntry.DataOra.Date))
                    {
                        meteoEntry.PrecipiatazioniCumulate = dictSumPrecH[meteoEntry.DataOra.Date];
                        meteoEntry.OrePioggia = dictOrePioggia[meteoEntry.DataOra.Date];
                    }
                    else
                    {
                        meteoEntry.PrecipiatazioniCumulate = 0;
                        meteoEntry.OrePioggia = 0;
                    }
                }
            }

            return result;
        }

        private async Task<MeteoSuiteOnboardingDeviceDto?> GetMeteoStation(
            AcquisizioneDatiMeteoRequest request,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(request.Piva))
            {
                return null;
            }

            if ((request.StationId != null && request.StationId! > 0) || !string.IsNullOrEmpty(request.StationCode))
            {
                if (request.StationId != null && request.StationId! > 0)
                {
                    int stationId = request.StationId.Value;

                    if (request.TipoStazione == (int)enum_TipoSorgenteMeteo.GIAS_STAZIONI_REGIONE_ER || 
                        request.TipoStazione == (int)enum_TipoSorgenteMeteo.GIAS_QUADRANTI_REGIONE_ER)
                    {
                        var dtRER = request.TipoStazione == (int)enum_TipoSorgenteMeteo.GIAS_STAZIONI_REGIONE_ER
                            ? await _rerStazioniDALService.LeggiStazionePerIdAsync(stationId, objParametriServer)
                            : await _rerQuadrantiDALService.LeggiQuadrantePerIdAsync(stationId, objParametriServer);
                        
                        if (dtRER == null || dtRER.Rows.Count == 0)
                        {
                            return null;
                        }

                        var drRER = dtRER.Rows[0];
                        decimal lat = 0;
                        decimal lng = 0;
                        decimal.TryParse((string)drRER["Lat"], NumberStyles.Any, CultureInfo.InvariantCulture, out lat);
                        decimal.TryParse((string)drRER["Lng"], NumberStyles.Any, CultureInfo.InvariantCulture, out lng);
                        string provider = !string.IsNullOrWhiteSpace(request.Provider) ? request.Provider : HypermeteoProvider;

                        var qryMeteoStation = _openqueryService.LeggiStazionePerPosizioneAsync(provider, lng, lat, request.Piva, objParametriServer, objParametriSuperServer, cancellationToken);
                        if (qryMeteoStation == null) 
                        { 
                            return null; 
                        }

                        stationId = qryMeteoStation.Id;
                    }
                    
                    return await _onboardingService.LeggiStazionePerIdAsynch(request.Piva, stationId, objParametriServer, objParametriSuperServer, cancellationToken);
                }
                else
                {
                    string? stationCode = request.StationCode;
                    return await _onboardingService.LeggiStazionePerProviderCodeAsynch(request.Piva, stationCode, objParametriServer, objParametriSuperServer, cancellationToken);
                }
            }
            else if (request.Coordinates != null)
            {
                var qryMeteoStation = await _openqueryService.LeggiStazionePerPosizioneAsync(HypermeteoProvider, request.Coordinates.Longitude, request.Coordinates.Latitude, request.Piva, objParametriServer, objParametriSuperServer, cancellationToken);
                if (qryMeteoStation != null)
                {
                    string? stationCode = qryMeteoStation.code;
                    return await _onboardingService.LeggiStazionePerProviderCodeAsynch(request.Piva, stationCode, objParametriServer, objParametriSuperServer, cancellationToken);
                }
            }

            return null;
        }

        private async Task<List<MeteoSuiteQueryWeatherDataPointDto>> LeggiDatiMeteoDaOpenQuery(
            AcquisizioneDatiMeteoRequest request,
            string[] sensori,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Validate request has a valid data source
                if (!request.HasValidDataSource())
                {
                    throw new MeteoMissingInputDataException(
                        "Neither station code nor valid coordinates provided. Unable to retrieve meteorological data.");
                }

                var (urlEngine, apiKey, tenantName) = await _securityLayerDal.RecuperaConfigurazioneEngineAsync(ChiaveUrlEngine, ChiaveApiKey, ChiaveTenantName, objParametriServer, objParametriSuperServer);

                if (string.IsNullOrWhiteSpace(urlEngine)) throw new ArgumentNullException(nameof(urlEngine));
                if (string.IsNullOrWhiteSpace(apiKey)) throw new ArgumentNullException(nameof(apiKey));
                if (string.IsNullOrWhiteSpace(tenantName)) throw new ArgumentNullException(nameof(tenantName));

                List<MeteoSuiteQueryWeatherDataPointDto> result;

                if (!string.IsNullOrWhiteSpace(request.StationCode))
                {
                    result = await _openqueryService.LeggiDatiMeteoPerCodiceAsync(
                        request.StationCode, 
                        request.DataInizio, request.DataFine, 
                        request.Piva,
                        sensori,
                        objParametriServer, objParametriSuperServer);
                }
                else if (request.Coordinates is not null)
                {
                    // Format coordinates with invariant culture to ensure proper decimal formatting
                    string latStr = request.Coordinates.Latitude.ToString(CultureInfo.InvariantCulture);
                    string lonStr = request.Coordinates.Longitude.ToString(CultureInfo.InvariantCulture);
                    string provider = !string.IsNullOrWhiteSpace(request.Provider) ? request.Provider : HypermeteoProvider;

                    result = await _openqueryService.LeggiDatiMeteoPerPosizioneAsync(
                        provider,
                        request.Coordinates.Longitude,
                        request.Coordinates.Latitude,
                        request.DataInizio, request.DataFine,
                        sensori,
                        objParametriServer, objParametriSuperServer);
                }
                else
                {
                    throw new ArgumentNullException(nameof(request.Coordinates));
                }

                if (result != null)
                {
                    if (request.IntervalloG != null)
                    {
                        DateTime lastTime = result.Max(x => x.PointInTime);
                        result = result.Where(x => x.PointInTime >= lastTime.AddDays(-request.IntervalloG.Value)).ToList();
                    }
                    else if (request.IntervalloH != null)
                    {
                        DateTime lastTime = result.Max(x => x.PointInTime);
                        result = result.Where(x => x.PointInTime >= lastTime.AddHours(-request.IntervalloH.Value)).ToList();
                    }
                }

                return result;
            }
            catch (MeteoMissingInputDataException ex)
            {
                //response.Status = "ERRORE";
                LogMeteoAcquisition(request, "ERRORE", ex.Message);
                throw;
            }
            catch (MeteoDevicesTimeoutException ex)
            {
                //response.Status = "ERRORE";
                LogMeteoAcquisition(request, "ERRORE", $"API Timeout: {ex.Message}");
                throw;
            }
            catch (MeteoUnauthorizedException ex)
            {
                //response.Status = "ERRORE";
                LogMeteoAcquisition(request, "ERRORE", $"API Unauthorized: {ex.Message}");
                throw;
            }
            catch (MeteoStationNotFoundException ex)
            {
                //response.Status = "ERRORE";
                LogMeteoAcquisition(request, "ERRORE", $"Station Not Found: {ex.Message}");
                throw;
            }
            catch (MeteoInsuccessResponseException ex)
            {
                //response.Status = "ERRORE";
                LogMeteoAcquisition(request, "ERRORE", $"API Error: {ex.Message}");
                throw;
            }
            catch (MeteoInvalidResponseException ex)
            {
                //response.Status = "ERRORE";
                LogMeteoAcquisition(request, "ERRORE", $"Invalid Response: {ex.Message}");
                throw;
            }
            catch (MeteoEmptyResultDataException ex)
            {
                //response.Status = "ERRORE";
                LogMeteoAcquisition(request, "ERRORE", $"Invalid Response: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                //response.Status = "ERRORE";
                LogMeteoAcquisition(request, "ERRORE", $"Unexpected error: {ex.Message}");
                throw;
            }
        }

        public async Task<AcquisizioneDatiMeteoPioggeResponse> AcquisisciDatiMeteorologiciPioggeAsync(
            AcquisizioneDatiMeteoRequest request,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default)
        {
            if (request is null) throw new ArgumentNullException(nameof(request));
            if (objParametriServer is null) throw new ArgumentNullException(nameof(objParametriServer));
            if (objParametriSuperServer is null) throw new ArgumentNullException(nameof(objParametriSuperServer));

            var response = new AcquisizioneDatiMeteoPioggeResponse();

            try
            {
                // Validate request has a valid data source
                if (!request.HasValidDataSource())
                {
                    throw new MeteoMissingInputDataException(
                        "Neither station code nor valid coordinates provided. Unable to retrieve meteorological data.");
                }

                var (urlEngine, apiKey, tenantName) = await _securityLayerDal.RecuperaConfigurazioneEngineAsync(ChiaveUrlEngine, ChiaveApiKey, ChiaveTenantName, objParametriServer, objParametriSuperServer);

                if (string.IsNullOrWhiteSpace(urlEngine)) throw new ArgumentNullException(nameof(urlEngine));
                if (string.IsNullOrWhiteSpace(apiKey)) throw new ArgumentNullException(nameof(apiKey));
                if (string.IsNullOrWhiteSpace(tenantName)) throw new ArgumentNullException(nameof(tenantName));

                string fullUrl = urlEngine.TrimEnd('/') + "/meteo-suite/" + tenantName.TrimStart('/').TrimEnd('/') + "/public/v1";
                if (!string.IsNullOrWhiteSpace(request.StationCode))
                {
                    fullUrl += $"/normalized/datapoints/devices/{request.StationCode}";
                    fullUrl += $"?from={Uri.EscapeDataString(request.DataInizio)}&to={Uri.EscapeDataString(request.DataFine)}";
                }
                else if (request.Coordinates is not null)
                {
                    // Format coordinates with invariant culture to ensure proper decimal formatting
                    string latStr = request.Coordinates.Latitude.ToString(CultureInfo.InvariantCulture);
                    string lonStr = request.Coordinates.Longitude.ToString(CultureInfo.InvariantCulture);
                    string provider = !string.IsNullOrWhiteSpace(request.Provider) ? request.Provider : HypermeteoProvider;
                    fullUrl += $"/normalized/datapoints?provider={provider}&lon={lonStr}&lat={latStr}";
                    fullUrl += $"&from={Uri.EscapeDataString(request.DataInizio)}&to={Uri.EscapeDataString(request.DataFine)}";
                }
                else
                {
                    throw new ArgumentNullException(nameof(request.Coordinates));
                }

                var sensorReadings = await EseguiChiamataHttpAsync(
                    request,
                    fullUrl,
                    apiKey,
                    TimeoutSeconds,
                    cancellationToken);

                // Aggregate sensor readings into meteorological data points
                var meteoData = AggregateMeteoData(sensorReadings);
                response.MeteoData = new List<DAL.DataLayer.DatiMeteo.Models.MeteoRainDataH>();

                // Validate the aggregated data
                List<ValidazioneDatiMeteoResult> validationResults = new List<ValidazioneDatiMeteoResult>();
                foreach (var meteoDataEntry in meteoData)
                {
                    var meteoDataValidationResult = _validationService.ValidateMeteoRainDataH(meteoDataEntry);
                    validationResults.Add(meteoDataValidationResult);

                    if (meteoDataValidationResult.IsValid)
                        response.MeteoData.Add(meteoDataEntry);
                }

                // Determine overall status
                response.Status = _validationService.DetermineOverallRainDataStatus(response.MeteoData, validationResults);

                // Log successful acquisition
                LogMeteoAcquisition(request, response.Status, null);

                return response;
            }
            catch (MeteoMissingInputDataException ex)
            {
                response.Status = "ERRORE";
                LogMeteoAcquisition(request, "ERRORE", ex.Message);
                throw;
            }
            catch (MeteoDevicesTimeoutException ex)
            {
                response.Status = "ERRORE";
                LogMeteoAcquisition(request, "ERRORE", $"API Timeout: {ex.Message}");
                throw;
            }
            catch (MeteoUnauthorizedException ex)
            {
                response.Status = "ERRORE";
                LogMeteoAcquisition(request, "ERRORE", $"API Unauthorized: {ex.Message}");
                throw;
            }
            catch (MeteoStationNotFoundException ex)
            {
                response.Status = "ERRORE";
                LogMeteoAcquisition(request, "ERRORE", $"Station Not Found: {ex.Message}");
                throw;
            }
            catch (MeteoInsuccessResponseException ex)
            {
                response.Status = "ERRORE";
                LogMeteoAcquisition(request, "ERRORE", $"API Error: {ex.Message}");
                throw;
            }
            catch (MeteoInvalidResponseException ex)
            {
                response.Status = "ERRORE";
                LogMeteoAcquisition(request, "ERRORE", $"Invalid Response: {ex.Message}");
                throw;
            }
            catch (MeteoEmptyResultDataException ex)
            {
                response.Status = "ERRORE";
                LogMeteoAcquisition(request, "ERRORE", $"Invalid Response: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                response.Status = "ERRORE";
                LogMeteoAcquisition(request, "ERRORE", $"Unexpected error: {ex.Message}");
                throw;
            }
        }

        private async Task<List<MeteoSensorsDataPoint>> EseguiChiamataHttpAsync(
            AcquisizioneDatiMeteoRequest parametri,
            string requestUrl,
            string apiKey,
            int timeoutSecondi = 30,
            CancellationToken cancellationToken = default)
        {
            using HttpRequestMessage request = new(HttpMethod.Get, requestUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("APIKEY", apiKey);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using CancellationTokenSource timeoutCts = new(TimeSpan.FromSeconds(timeoutSecondi));
            using CancellationTokenSource linkedCts =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, linkedCts.Token);
            }
            catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
            {
                throw new MeteoTimeoutException(timeoutSecondi);
            }

            string responseBody = await response.Content.ReadAsStringAsync(CancellationToken.None);
            
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new MeteoUnauthorizedException("API key is invalid or expired. HTTP 401 Unauthorized received from Meteo Suite API");
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new MeteoStationNotFoundException("The requested meteorological station or coordinates were not found. HTTP 404 Not Found received from Meteo Suite API");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new MeteoInsuccessResponseException(response.StatusCode, responseBody);
            }

            // HTTP 200 — parse JSON response
            List<MeteoSensorsDataPoint> sensorsData = ParseSensorsDataPoint(responseBody);

            if (sensorsData.Count == 0)
            {
                throw new MeteoEmptyResultDataException();
            }

            return sensorsData;
        }

        private static List<MeteoSensorsDataPoint> ParseSensorsDataPoint(string responseContent)
        {
            try
            {
                // Parse the response according to spec:
                // Response is an array of objects with "pointInTime" and "sensors" properties
                var dataPoints = JsonConvert.DeserializeObject<List<dynamic>>(responseContent);

                if (dataPoints == null)
                {
                    throw new MeteoInvalidResponseException("API returned null or empty response");
                }

                var sensorReadings = new List<MeteoSensorsDataPoint>();

                foreach (var dataPoint in dataPoints)
                {
                    try
                    {
                        var pointInTime = dataPoint["pointInTime"];
                        if (pointInTime == null)
                        {
                            throw new MeteoInvalidResponseException("Missing 'pointInTime' field in API response");
                        }

                        var sensorsObj = dataPoint["sensors"];
                        if (sensorsObj == null)
                        {
                            throw new MeteoInvalidResponseException("Missing 'sensors' field in API response");
                        }

                        // Convert sensors from dynamic object to dictionary
                        var sensorDict = new Dictionary<string, decimal>();
                        if (sensorsObj is Newtonsoft.Json.Linq.JObject jObj)
                        {
                            foreach (var property in jObj.Properties())
                            {
                                //NumberFormatInfo nfi = new NumberFormatInfo();
                                //nfi.NumberDecimalSeparator = ".";
                                if (decimal.TryParse(property.Value.ToString().Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
                                {
                                    sensorDict[property.Name] = value;
                                }
                            }
                        }

                        // Parse point in time
                        var pointInTimeStr = pointInTime.ToString("O");
                        if (!DateTime.TryParse(pointInTimeStr, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime parsedTime))
                        {
                            throw new MeteoInvalidResponseException($"Invalid date format in pointInTime: {pointInTime}");
                        }

                        sensorReadings.Add(new MeteoSensorsDataPoint
                        {
                            PointInTime = parsedTime,
                            Sensors = sensorDict
                        });
                    }
                    catch (Exception ex) when (!(ex is MeteoInvalidResponseException))
                    {
                        throw new MeteoInvalidResponseException("Failed to parse data point from API response", ex);
                    }
                }

                return sensorReadings;
            }
            catch (JsonException ex)
            {
                throw new MeteoInvalidResponseException("API returned malformed JSON", ex);
            }
        }

        /// <summary>
        /// Aggregates raw sensor readings into hourly meteorological data points.
        /// Maps sensor codes to their corresponding meteorological parameters.
        /// </summary>
        private List<DAL.DataLayer.DatiMeteo.Models.MeteoRainDataH> AggregateMeteoData(List<MeteoSensorsDataPoint> sensorReadings)
        {
            // Group readings by point in time (hour)
            var groupedReadings = sensorReadings
                .GroupBy(r => r.PointInTime)
                .OrderBy(g => g.Key)
                .ToList();

            var meteoDataList = new List<DAL.DataLayer.DatiMeteo.Models.MeteoRainDataH>();

            foreach (var hourGroup in groupedReadings)
            {
                // There should be only one reading per point in time according to spec
                // "non eventuali pointInTime duplicati"
                var reading = hourGroup.First();

                var meteoData = new DAL.DataLayer.DatiMeteo.Models.MeteoRainDataH
                {
                    DataOra = reading.PointInTime,
                    Temp = ExtractSensorValue(reading.Sensors, TemperatureSensorCodeH),
                    Prec = ExtractSensorValue(reading.Sensors, PrecipitationSensorCodeH),
                    RelHum = ExtractSensorValue(reading.Sensors, HumiditySensorCodeH),
                    Lw = ExtractSensorValue(reading.Sensors, LeafWetnessSensorCodeH)
                };

                meteoDataList.Add(meteoData);
            }

            return meteoDataList;
        }

        /// <summary>
        /// Extracts a sensor value from the sensor dictionary, returning null if not present.
        /// </summary>
        private decimal? ExtractSensorValue(Dictionary<string, decimal> sensors, string sensorCode)
        {
            if (sensors != null && sensors.TryGetValue(sensorCode, out var value))
            {
                return value;
            }
            return null;
        }

        /// <summary>
        /// Logs meteorological data acquisition activity for audit trail.
        /// Referenced in DS02-BL_: "Log acquisizione (scrittura): timestamp, source, validation_status per audit"
        /// </summary>
        private void LogMeteoAcquisition(
            AcquisizioneDatiMeteoRequest request,
            string status,
            string? errorMessage)
        {
            string source = !string.IsNullOrWhiteSpace(request.StationCode)
                ? $"STATION:{request.StationCode}"
                : $"COORDINATES:{request.Coordinates?.Latitude},{request.Coordinates?.Longitude}";

            if (string.IsNullOrEmpty(errorMessage))
            {
                LogInformation(
                    $"Meteorological data acquisition successful. Source: {source}, Status: {status}, Period: {request.DataInizio} to {request.DataFine}");
            }
            else
            {
                LogWarning(
                    $"Meteorological data acquisition failed. Source: {source}, Status: {status}, Error: {errorMessage}");
            }
        }
    }
}
