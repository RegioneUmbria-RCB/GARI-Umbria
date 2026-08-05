using AgronicaNetCore.Base.DataLayer.Security;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MeteoSuite.BIZ.Resources;
using AgronicaNetCore.MeteoSuite.BIZ.Services.DatiMeteo.Acquisizione;
using AgronicaNetCore.MeteoSuite.BIZ.Services.Engine;
using AgronicaNetCore.MeteoSuite.DAL.DataLayer.Stazioni;
using InData.Engine.MeteoSuite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using OutData.Engine.MeteoSuite;
using System.Data;
using System.Globalization;

namespace AgronicaNetCore.MeteoSuite.BIZ.Services.Stazioni
{
    public class AnagraficaStazioniMeteoService : BaseMeteoSuiteBIZService, IAnagraficaStazioniMeteoService
    {
        private const string HypermeteoProvider = "HYPERMETEO";

        private readonly IMeteoSuiteQueryDeviceService _openqueryLetturaStazioniService;
        private readonly IMeteoSuiteOnboardingDevicesService _onboardingStazioniService;
        private readonly ITbRerStazioniDAL _rerStazioniDALService;
        private readonly ITbRerQuadrantiDAL _rerQuadrantiDALService;
        private readonly IVisibilitaStazioniAliasDAL _aliasStazioniDALService;

        private readonly ISecurityLayerDAL _securityLayerDal;
        private readonly HttpClient _httpClient;
        private readonly IValidazioneDatiMeteoService _validationService;

        public AnagraficaStazioniMeteoService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _openqueryLetturaStazioniService = provider.GetRequiredService<IMeteoSuiteQueryDeviceService>();
            _onboardingStazioniService = provider.GetRequiredService<IMeteoSuiteOnboardingDevicesService>();
            _rerStazioniDALService = provider.GetRequiredService<ITbRerStazioniDAL>();
            _rerQuadrantiDALService = provider.GetRequiredService<ITbRerQuadrantiDAL>();
            _aliasStazioniDALService = provider.GetRequiredService<IVisibilitaStazioniAliasDAL>();

            _securityLayerDal = provider.GetRequiredService<ISecurityLayerDAL>();
            _httpClient = provider.GetRequiredService<HttpClient>();
            _validationService = provider.GetRequiredService<IValidazioneDatiMeteoService>();
        }

        public async Task<List<StazioneMeteoDto>> LeggiStazioniMeteoAutorizzateAsync(
            string Piva,
            int Tipo,
            decimal? Longitude, decimal? Latitude,
            int offset, int limit,
            AgronicaCoreParametriServer objParametriServer, 
            AgronicaCoreParametriSuperServer objParametriSuperServer, CancellationToken cancellationToken = default)
        {
            var response = new List<StazioneMeteoDto>();

            switch (Tipo)
            {
                case (int)enum_TipoSorgenteMeteo.GIAS_STAZIONI_REGIONE_ER:

                    var dtRerStazioni = await _rerStazioniDALService.LeggiElencoStazioniAsync(objParametriServer);
                    if (dtRerStazioni != null)
                    {
                        foreach (DataRow dr in dtRerStazioni.Rows)
                        {
                            response.Add(MapRerStazioneDtToResultDto(dr, Longitude, Latitude));
                        }
                    }

                    break;

                case (int)enum_TipoSorgenteMeteo.GIAS_QUADRANTI_REGIONE_ER:

                    var dtRerQuadranti = await _rerQuadrantiDALService.LeggiElencoQuadrantiAsync(objParametriServer);
                    if (dtRerQuadranti != null)
                    {
                        foreach (DataRow dr in dtRerQuadranti.Rows)
                        {
                            response.Add(MapRerStazioneDtToResultDto(dr, Longitude, Latitude));
                        }
                    }

                    break;

                case (int)enum_TipoSorgenteMeteo.PUBBLICHE:

                    if (Longitude.HasValue && Longitude.Value != 0 && Latitude.HasValue && Latitude.Value != 0)
                    {
                        var stationDTO = await _openqueryLetturaStazioniService.LeggiStazionePerPosizioneAsync(HypermeteoProvider, Longitude.Value, Latitude.Value, Piva, objParametriServer, objParametriSuperServer, cancellationToken);
                        if (stationDTO != null)
                        {
                            var onbStationDTO = await _onboardingStazioniService.LeggiStazionePerProviderCodeAsynch(Piva, stationDTO.code, objParametriServer, objParametriSuperServer, cancellationToken);
                            if (onbStationDTO != null)
                            {
                                response.Add(MapOnbordingDtoToResultDto(onbStationDTO, Longitude, Latitude));
                            }
                        }
                    }

                    var dtAlias = await _aliasStazioniDALService.LeggiElencoAsync(Piva, objParametriServer);
                    if (dtAlias != null && dtAlias.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtAlias.Rows.Count; i++)
                        {
                            DataRow dr = dtAlias.Rows[i];
                            int Id_Stazione = (int)dr["Id_Stazione"];
                            var onbStationDTO = await _onboardingStazioniService.LeggiStazionePerIdAsynch(Piva, Id_Stazione, objParametriServer, objParametriSuperServer, cancellationToken);

                            if (onbStationDTO != null)
                            {
                                response.Add(MapOnbordingDtoToResultDto(onbStationDTO, Longitude, Latitude));
                            }
                        }
                    }

                    break;

                case (int)enum_TipoSorgenteMeteo.AZIENDALI:
                case (int)enum_TipoSorgenteMeteo.RETI_PARTNER:
                default:

                    var onboardingResponse = await _onboardingStazioniService.LeggiStazioniAutorizzateAsynch(Piva, null, MeteoStationVisibility.Private, offset, limit, objParametriServer, objParametriSuperServer);

                    if (onboardingResponse != null && onboardingResponse.devices != null)
                    {
                        foreach (var device in onboardingResponse.devices)
                        {
                            if (Tipo == (int)enum_TipoSorgenteMeteo.RETI_PARTNER || device.isOwner)
                            {
                                response.Add(MapOnbordingDtoToResultDto(device, Longitude, Latitude));
                            }
                        }
                    }

                    break;
            }

            return response;
        }

        public async Task<StazioneMeteoDto?> LeggiStazioneMeteoPerIdAsync(
            string Piva,
            int Tipo,
            int IdStazione,
            decimal? Longitude, decimal? Latitude,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default)
        {
            switch (Tipo)
            {
                case (int)enum_TipoSorgenteMeteo.GIAS_STAZIONI_REGIONE_ER:

                    var dtRerStazioni = await _rerStazioniDALService.LeggiStazionePerIdAsync(IdStazione, objParametriServer);
                    if (dtRerStazioni != null && dtRerStazioni.Rows.Count > 0)
                    {
                        return MapRerStazioneDtToResultDto(dtRerStazioni.Rows[0], Longitude, Latitude);
                    }

                    break;

                case (int)enum_TipoSorgenteMeteo.GIAS_QUADRANTI_REGIONE_ER:

                    var dtRerQuadranti = await _rerQuadrantiDALService.LeggiQuadrantePerIdAsync(IdStazione, objParametriServer);
                    if (dtRerQuadranti != null && dtRerQuadranti.Rows.Count > 0)
                    {
                        return MapRerStazioneDtToResultDto(dtRerQuadranti.Rows[0], Longitude, Latitude);
                    }

                    break;

                case (int)enum_TipoSorgenteMeteo.AZIENDALI:
                case (int)enum_TipoSorgenteMeteo.RETI_PARTNER:
                case (int)enum_TipoSorgenteMeteo.PUBBLICHE:
                default:

                    var response = await _onboardingStazioniService.LeggiStazionePerIdAsynch(Piva, IdStazione, objParametriServer, objParametriSuperServer, cancellationToken);

                    if (response != null)
                    {
                        return MapOnbordingDtoToResultDto(response, Longitude, Latitude);
                    }

                    break;
            }

            return null;
        }

        public async Task<StazioneMeteoDto?> LeggiStazioneMeteoPerPosizioneAsync(
            string Piva,
            int Tipo,
            decimal Longitude, decimal Latitude, 
            AgronicaCoreParametriServer objParametriServer, 
            AgronicaCoreParametriSuperServer objParametriSuperServer, 
            CancellationToken cancellationToken = default)
        {
            switch (Tipo)
            {
                case (int)enum_TipoSorgenteMeteo.GIAS_STAZIONI_REGIONE_ER:

                    var dtRerStazioni = await _rerStazioniDALService.LeggiElencoStazioniAsync(objParametriServer);

                    return GetNearestRERStazioneQuadrante(Longitude, Latitude, dtRerStazioni);

                case (int)enum_TipoSorgenteMeteo.GIAS_QUADRANTI_REGIONE_ER:

                    var dtRerQuadranti = await _rerQuadrantiDALService.LeggiElencoQuadrantiAsync(objParametriServer);
                    
                    return GetNearestRERStazioneQuadrante(Longitude, Latitude, dtRerQuadranti);

                case (int)enum_TipoSorgenteMeteo.AZIENDALI:
                case (int)enum_TipoSorgenteMeteo.RETI_PARTNER:
                case (int)enum_TipoSorgenteMeteo.PUBBLICHE:
                default:

                    var stationDTO = await _openqueryLetturaStazioniService.LeggiStazionePerPosizioneAsync(HypermeteoProvider, Longitude, Latitude, Piva, objParametriServer, objParametriSuperServer, cancellationToken);
                    if (stationDTO == null)
                    {
                        return null;
                    }

                    var onbStationDTO = await _onboardingStazioniService.LeggiStazionePerProviderCodeAsynch(Piva, stationDTO.code, objParametriServer, objParametriSuperServer);
                    if (onbStationDTO == null)
                    {
                        return null;
                    }

                    return MapOnbordingDtoToResultDto(onbStationDTO, Longitude, Latitude);
            }           
        }

        private StazioneMeteoDto? GetNearestRERStazioneQuadrante(decimal Longitude, decimal Latitude, DataTable? dt)
        {
            if (dt != null)
            {
                decimal lat = 0;
                decimal lng = 0;
                double dist;
                double minDist = double.MaxValue;
                DataRow? minDistDR = null;

                foreach (DataRow dr in dt.Rows)
                {
                    if (dr.IsNull("Lat") || dr.IsNull("Lng"))
                    {
                        continue;
                    }

                    decimal.TryParse((string)dr["Lat"], NumberStyles.Any, CultureInfo.InvariantCulture, out lat);
                    decimal.TryParse((string)dr["Lng"], NumberStyles.Any, CultureInfo.InvariantCulture, out lng);
                    dist = Math.Sqrt((double)((lng - Longitude) * (lng - Longitude)) + (double)((lat - Latitude) * (lat - Latitude)));

                    if (dist < minDist)
                    {
                        minDist = dist;
                        minDistDR = dr;
                    }
                }

                if (minDistDR != null)
                {
                    return MapRerStazioneDtToResultDto(minDistDR, Longitude, Latitude);
                }
            }

            return null;
        }

        public async Task<StazioneMeteoDto?> AggiornaOCreaStazioneMeteoAsync(
            AggiornaStazioneMeteoRequest request, 
            AgronicaCoreParametriServer objParametriServer, 
            AgronicaCoreParametriSuperServer objParametriSuperServer, 
            CancellationToken cancellationToken = default)
        {
            var response = await _onboardingStazioniService.AggiornaOCreaStazioneAsynch(request, objParametriServer, objParametriSuperServer, cancellationToken);

            if (response == null)
            {
                return null;
            }

            return MapOnbordingDtoToResultDto(response, null, null);
        }

        public async Task<bool> EliminaStazioneMeteoAsync(
            string Piva,
            int IdStazione,
            AgronicaCoreParametriServer objParametriServer, 
            AgronicaCoreParametriSuperServer objParametriSuperServer, 
            CancellationToken cancellationToken = default)
        {
            var response = await _onboardingStazioniService.EliminaStazioneAsynch(Piva, IdStazione, objParametriServer, objParametriSuperServer, cancellationToken);

            return response;
        }

        private StazioneMeteoDto MapOnbordingDtoToResultDto(MeteoSuiteOnboardingDeviceDto dto, decimal? longitude, decimal? latitude)
        {
            var respStation = new StazioneMeteoDto()
            {
                Id_Stazione = dto.id,
                Nome_Stazione = dto.description,
                Lng = dto.longitude,
                Lat = dto.latitude,
                Fornitore = dto.provider,
                RifFornitore = dto.providerReference,
                FlagReale = dto.flIsReal,
                Proprietario = dto.isOwner,
                Distanza = null,
                Sensors = new List<StazioneMeteoSensoreDto>()
            };

            if (longitude.HasValue && longitude.Value != 0 && latitude.HasValue && latitude.Value != 0)
            {
                decimal dx = longitude.Value - dto.longitude;
                decimal dy = latitude.Value - dto.latitude;
                respStation.Distanza = (decimal?)Math.Sqrt((double)(dx * dx + dy * dy));
            }

            foreach (var sens in dto.sensors)
            {
                respStation.Sensors.Add(new StazioneMeteoSensoreDto()
                {
                    Id_Sensore = sens.id,
                    Description = sens.description,
                    SensorTypeId = sens.sensorTypeId,
                    SensorType = sens.sensorType,
                    MeasureUnit = sens.uom,
                    AggregationFunc = sens.aggregationFunc
                });
            }

            return respStation;
        }

        private StazioneMeteoDto MapRerStazioneDtToResultDto(DataRow dr, decimal? longitude, decimal? latitude)
        {
            decimal lat = 0;
            decimal lng = 0;
            if (!dr.IsNull("Lat"))
                decimal.TryParse((string)dr["Lat"], NumberStyles.Any, CultureInfo.InvariantCulture, out lat);
            if (!dr.IsNull("Lng"))
                decimal.TryParse((string)dr["Lng"], NumberStyles.Any, CultureInfo.InvariantCulture, out lng);

            var respStation = new StazioneMeteoDto()
            {
                Id_Stazione = (int)dr["ID"],
                Nome_Stazione = (string)dr["Descrizione"],
                Lng = lng,
                Lat = lat,
                Fornitore = (string)dr["Fornitore"],
                RifFornitore = (string)dr["RifFornitore"],
                FlagReale = true,
                Sensors = new List<StazioneMeteoSensoreDto>()
            };

            if (longitude.HasValue && longitude.Value != 0 && latitude.HasValue && latitude.Value != 0 && lat != 0 && lng != 0)
            {
                respStation.Distanza = DistanzaHaversineInM(longitude.Value, lng, latitude.Value, lat);
            }

            return respStation;
        }

        private decimal DistanzaHaversineInM(decimal Long1, decimal Long2, decimal Lat1, decimal Lat2)
        {
            const double conv2Rad = 0.01745329251; //(Math.PI / 180)

            //double result = Math.Acos(Math.Cos((double)(90 - Lat1) * conv2Rad * Math.Cos((double)(90 - Lat2) * conv2Rad) + Math.Sin((double)(90 - Lat1) * conv2Rad) * Math.Sin((double)(90 - Lat2) * conv2Rad) * Math.Cos((double)(Long1 - Long2) * conv2Rad)) * 6371;

            //return result;

            double lat1Rad = (double)Lat1 * conv2Rad;
            double lat2Rad = (double)Lat1 * conv2Rad;
            double lon1Rad = (double)Long1 * conv2Rad;
            double lon2Rad = (double)Long2 * conv2Rad;

            const double r = 6378100; // meters

            var sdlat = Math.Sin((lat2Rad - lat1Rad) / 2);
            var sdlon = Math.Sin((lon2Rad - lon1Rad) / 2);
            var q = sdlat * sdlat + Math.Cos(lat1Rad) * Math.Cos(lat2Rad) * sdlon * sdlon;
            var d = 2 * r * Math.Asin(Math.Sqrt(q));

            return (decimal)d;
        }

        public async Task<List<StazioneMeteoAliasDto>> LeggiElencoAlias(string Piva, AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriSuperServer objParametriSuperServer, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(Piva)) throw new ArgumentNullException(nameof(Piva));
            if (objParametriServer is null) throw new ArgumentNullException(nameof(objParametriServer));
            if (objParametriSuperServer is null) throw new ArgumentNullException(nameof(objParametriSuperServer));

            DataTable? dtAlias = await _aliasStazioniDALService.LeggiElencoAsync(Piva, objParametriServer);

            List<StazioneMeteoAliasDto> result = new List<StazioneMeteoAliasDto>();

            if (dtAlias != null)
            {
                foreach (DataRow drAlias in dtAlias.Rows) 
                {
                    int Id_Stazione = (int)drAlias["Id_Stazione"];
                    decimal Latitude = 0;
                    decimal Longitude = 0;

                    var onbStationDTO = await _onboardingStazioniService.LeggiStazionePerIdAsynch(Piva, Id_Stazione, objParametriServer, objParametriSuperServer);
                    if (onbStationDTO != null)
                    {
                        Latitude = onbStationDTO.latitude;
                        Longitude = onbStationDTO.longitude;
                    }

                    result.Add(new StazioneMeteoAliasDto()
                    {
                        Id = Id_Stazione,
                        Name = (string)drAlias["Alias"],
                        Lat = Latitude,
                        Lng = Longitude
                    });
                }
            }

            return result;
        }

        public async Task<StazioneMeteoAliasNewDto?> LeggiStazioneMeteoXAlias(string Piva, decimal Longitude, decimal Latitude, AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriSuperServer objParametriSuperServer, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(Piva)) throw new ArgumentNullException(nameof(Piva));
            if (objParametriServer is null) throw new ArgumentNullException(nameof(objParametriServer));
            if (objParametriSuperServer is null) throw new ArgumentNullException(nameof(objParametriSuperServer));

            var stationDTO = await _openqueryLetturaStazioniService.LeggiStazionePerPosizioneAsync(HypermeteoProvider, Longitude, Latitude, Piva, objParametriServer, objParametriSuperServer);
            if (stationDTO == null) 
            {
                return null;
            }
            
            var onbStationDTO = await _onboardingStazioniService.LeggiStazionePerProviderCodeAsynch(Piva, stationDTO.code, objParametriServer, objParametriSuperServer);
            if (onbStationDTO == null)
            {
                return null;
            }

            StazioneMeteoAliasNewDto result = new StazioneMeteoAliasNewDto()
            {
                Id = onbStationDTO.id,
                Name = onbStationDTO.description,
                Lat = onbStationDTO.latitude,
                Lng = onbStationDTO.longitude,
                Fornitore = onbStationDTO.provider,
                Dist = 0,
                FlagNew = true,
                AliasName = null
            };
            double xdist = (double)(onbStationDTO.longitude - Longitude);
            double ydist = (double)(onbStationDTO.latitude - Latitude);
            result.Dist = Math.Sqrt(xdist * xdist + ydist * ydist);

            var dtAlias = await _aliasStazioniDALService.LeggiPerStazioneAsync(Piva, onbStationDTO.id, objParametriServer);
            if (dtAlias != null && dtAlias.Rows.Count > 0)
            {
                DataRow drAlias = dtAlias.Rows[0];
                result.AliasName = (string?)drAlias["Alias"];
                result.FlagNew = false;
            }

            return result;
        }

        public async Task<StazioneMeteoAliasDto?> AggiornaAlias(string Piva, int IdStazione, string NomeAlias, AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriSuperServer objParametriSuperServer, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(Piva)) throw new ArgumentNullException(nameof(Piva));
            if (IdStazione <= 0) throw new ArgumentNullException(nameof(IdStazione));
            if (objParametriServer is null) throw new ArgumentNullException(nameof(objParametriServer));
            if (objParametriSuperServer is null) throw new ArgumentNullException(nameof(objParametriSuperServer));

            var onbStationDTO = await _onboardingStazioniService.LeggiStazionePerIdAsynch(Piva, IdStazione, objParametriServer, objParametriSuperServer);
            if (onbStationDTO == null)
            {
                return null;
            }

            StazioneMeteoAliasDto? result = null;

            var dtAlias = await _aliasStazioniDALService.UpsertAsync(Piva, IdStazione, NomeAlias, objParametriServer);
            if (dtAlias != null && dtAlias.Rows.Count > 0)
            {
                DataRow drAlias = dtAlias.Rows[0];
                result = new StazioneMeteoAliasDto()
                {
                    Id = onbStationDTO.id,
                    Name = (string)drAlias["Alias"],
                    Lat = onbStationDTO.latitude,
                    Lng = onbStationDTO.longitude
                };
            }

            return result;
        }

        public async Task<bool> EliminaAlias(string Piva, int IdStazione, AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriSuperServer objParametriSuperServer, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(Piva)) throw new ArgumentNullException(nameof(Piva));
            if (IdStazione <= 0) throw new ArgumentNullException(nameof(IdStazione));
            if (objParametriServer is null) throw new ArgumentNullException(nameof(objParametriServer));
            if (objParametriSuperServer is null) throw new ArgumentNullException(nameof(objParametriSuperServer));

            return await _aliasStazioniDALService.DeleteAsync(Piva, IdStazione, objParametriServer);
        }
    }
}
