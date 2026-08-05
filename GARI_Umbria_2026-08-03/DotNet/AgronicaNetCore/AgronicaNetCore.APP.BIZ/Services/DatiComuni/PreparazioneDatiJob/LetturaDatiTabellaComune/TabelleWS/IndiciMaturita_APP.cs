using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiComuni;
using AgronicaNetCore.APP.BIZ.Common.WebServiceDTO.Input;
using AgronicaNetCore.APP.BIZ.Common.WebServiceDTO.Output;
using AgronicaNetCore.APP.BIZ.Exceptions;
using AgronicaNetCore.APP.BIZ.Resources;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.DataLayer.Security;
using AgronicaNetCore.Base.Exceptions;
using AgronicaNetCore.Base.Utility;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.TabelleComuni_APP;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using AgronicaNetCore.Base.Base;
using Newtonsoft.Json;

namespace AgronicaNetCore.APP.BIZ.Services.LetturaDatiTabellaComune.TabelleWS
{
    public class IndiciMaturitaApp : BaseServiceAppBIZ, ILetturaTabellaComuneApp
    {
        private readonly ILoggingService _loggingService;
        private readonly ChiamaWebService _wsClient;
        private readonly ISecurityLayerDAL _securityLayerDal;

        private Task<(object indici, object misure, object specieVegetali)>? _cachedFalse;
        private Task<(object indici, object misure, object specieVegetali)>? _cachedTrue;

        public IndiciMaturitaApp(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer
        )
            : base(provider, localizer)
        {
            _loggingService = _serviceProvider.GetRequiredService<ILoggingService>();
            _wsClient = _serviceProvider.GetRequiredService<ChiamaWebService>();
            _securityLayerDal = _serviceProvider.GetRequiredService<ISecurityLayerDAL>();
        }

        // ILetturaTabellaComuneAPP — TabellaComune.IndiciMaturita (personalizzate=false, restituisce indici)
        public async Task<object> LeggiAsync(LetturaTabellaComuneAppParameters parameters)
        {
            var (indici, _, _) = await GetOrFetchAsync(false, parameters);
            return indici;
        }

        // Usato dalle classi wrapper per leggere la slice giusta con la personalizzazione corretta
        internal Task<(object indici, object misure, object specieVegetali)> GetOrFetchAsync(
            bool personalizzate,
            LetturaTabellaComuneAppParameters parameters
        ) =>
            personalizzate
                ? _cachedTrue ??= LeggiInternalAsync(true, parameters)
                : _cachedFalse ??= LeggiInternalAsync(false, parameters);

        private async Task<(
            object indici,
            object misure,
            object specieVegetali
        )> LeggiInternalAsync(bool personalizzate, LetturaTabellaComuneAppParameters parameters)
        {
            var url = await _securityLayerDal.LeggiConfigurazioneSitiScalareAsync(
                "GiasOnline_WS_SpecieVegetali_IAgroAPI_SpecieVegetali",
                parameters.ObjParametriTriple.ObjParametriServer,
                parameters.ObjParametriTriple.ObjParametriSuperServer
            );

            var input = new IndiciMaturitaInput
            {
                TipoTestata = 0,
                Personalizzate = personalizzate,
                PivaSuperuser = parameters.ObjParametriTriple.ObjParametriServer.PivaSuperUser,
                LinguaCod = parameters.ObjParametriTriple.ObjParametriServer.Lingua_Cod,
                FiltraSpecie = false,
                Url = url + "/IndiciMaturita",
                EstraiPersonalizzatePerAPP = personalizzate
            };

            var json = await _wsClient.ChiamaWebServiceAsync(
                input.Url,
                input,
                parameters.BearerToken
            );

            var output =
                JsonConvert.DeserializeObject<IndiciMaturitaOutput>(json)
                ?? throw new DeserializationException("Risposta nulla da IndiciMaturita WS");

            if (!string.IsNullOrEmpty(output.MessaggioErrore))
                throw new WebServiceException(
                    $"Errore WS IndiciMaturita: {output.MessaggioErrore}"
                );

            var lista = output.ListaIndiciMaturita;

            if (personalizzate)
            {
                var indici = lista
                    .DistinctBy(m => (m.IndMatCod, m.Pivasuperuser))
                    .Select(m => new IndiciMaturitaPersonalizzataEntity
                    {
                        codice = m.IndMatCod,
                        descrizione = m.IndMatDes,
                        lavCod = m.LavCod,
                        dataAggiornamento = DateTime.Parse(CostantiPersonalizzate.AGRODATAINIZIO),
                        pivasuperuser = m.Pivasuperuser,
                    })
                    .ToList();

                var misure = lista
                    .DistinctBy(m => (m.IndMatCod, m.UdmCod, m.Pivasuperuser))
                    .Select(m => new MisuraIndiciMaturitaPersonalizzataEntity
                    {
                        indiceMaturitaCod = m.IndMatCod,
                        udmCod = m.UdmCod,
                        dataAggiornamento = DateTime.Parse( CostantiPersonalizzate.AGRODATAINIZIO),
                        pivasuperuser = m.Pivasuperuser,
                    })
                    .ToList();

                var specieVegetali = lista
                    .DistinctBy(m => (m.IndMatCod, m.VegCod, m.Pivasuperuser))
                    .Select(m => new IndiciMaturitaSpecieVegetaliPersonalizzataEntity
                    {
                        indiceMaturitaCod = m.IndMatCod,
                        specieCod = m.VegCod,
                        REG_COD = m.RegCod,
                        classe = m.Classe,
                        raccolta = m.FlagRaccolta != 0,
                        dataAggiornamento = DateTime.Parse(CostantiPersonalizzate.AGRODATAINIZIO),
                        pivasuperuser = m.Pivasuperuser,
                    })
                    .ToList();

                return (indici, misure, specieVegetali);
            }
            else
            {
                var indici = lista
                    .DistinctBy(m => m.IndMatCod)
                    .Select(m => new IndiciMaturitaEntity
                    {
                        codice = m.IndMatCod,
                        descrizione = m.IndMatDes,
                        lavCod = m.LavCod,
                        dataAggiornamento = DateTime.Parse(CostantiPersonalizzate.AGRODATAINIZIO),
                    })
                    .ToList();

                var misure = lista
                    .DistinctBy(m => (m.IndMatCod, m.UdmCod))
                    .Select(m => new MisuraIndiciMaturitaEntity
                    {
                        indiceMaturitaCod = m.IndMatCod,
                        udmCod = m.UdmCod,
                        dataAggiornamento = DateTime.Parse(CostantiPersonalizzate.AGRODATAINIZIO)
                    })
                    .ToList();

                var specieVegetali = lista
                    .DistinctBy(m => (m.IndMatCod, m.VegCod))
                    .Select(m => new IndiciMaturitaSpecieVegetaliEntity
                    {
                        indiceMaturitaCod = m.IndMatCod,
                        specieCod = m.VegCod,
                        REG_COD = m.RegCod,
                        classe = m.Classe,
                        raccolta = m.FlagRaccolta != 0,
                        dataAggiornamento = DateTime.Parse(CostantiPersonalizzate.AGRODATAINIZIO),
                    })
                    .ToList();

                return (indici, misure, specieVegetali);
            }
        }
    }
}

