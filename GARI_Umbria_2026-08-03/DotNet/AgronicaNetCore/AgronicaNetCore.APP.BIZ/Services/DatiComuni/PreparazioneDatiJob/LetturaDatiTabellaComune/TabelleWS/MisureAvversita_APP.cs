using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiComuni;
using AgronicaNetCore.APP.BIZ.Common.WebServiceDTO.Input;
using AgronicaNetCore.APP.BIZ.Common.WebServiceDTO.Output;
using AgronicaNetCore.APP.BIZ.Exceptions;
using AgronicaNetCore.APP.BIZ.Resources;
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
    public class MisureAvversitaApp : BaseServiceAppBIZ, ILetturaTabellaComuneApp
    {
        private readonly ILoggingService _loggingService;
        private readonly ChiamaWebService _wsClient;
        private readonly ISecurityLayerDAL _securityLayerDal;

        public MisureAvversitaApp(
            IServiceProvider provider,
            ChiamaWebService wsClient,
            IStringLocalizer<Messages> localizer
        )
            : base(provider, localizer)
        {
            _loggingService = _serviceProvider.GetRequiredService<ILoggingService>();
            _wsClient = wsClient;
            _securityLayerDal = _serviceProvider.GetRequiredService<ISecurityLayerDAL>();
        }

        // ILetturaTabellaComuneAPP — TabellaComune.MisureAvversita (personalizzate=false)
        public async Task<object> LeggiAsync(LetturaTabellaComuneAppParameters parameters) =>
            await LeggiAsync(false, parameters);

        internal async Task<object> LeggiAsync(
            bool personalizzate,
            LetturaTabellaComuneAppParameters parameters
        )
        {
            var url = await _securityLayerDal.LeggiConfigurazioneSitiScalareAsync(
                "GiasOnline_WS_SpecieVegetali_IAgroAPI_SpecieVegetali",
                parameters.ObjParametriTriple.ObjParametriServer,
                parameters.ObjParametriTriple.ObjParametriSuperServer
            );

            var input = new MisuraXAvversitaInput
            {
                VegCod = 0,
                DpiCod = 0,
                IdRcdpi = 0,
                TipoTestata = 0,
                DpiPubblicoPrivato = 1,
                LinguaCod = parameters.ObjParametriTriple.ObjParametriServer.Lingua_Cod,
                Personalizzate = personalizzate,
                PivaSuperuser = parameters.ObjParametriTriple.ObjParametriServer.PivaSuperUser,
                Url = url + "/MisureXAvversita",
                EstraiPersonalizzatePerAPP = personalizzate
            };

            var json = await _wsClient.ChiamaWebServiceAsync(
                input.Url,
                input,
                parameters.BearerToken
            );

            var output =
                JsonConvert.DeserializeObject<MisuraXAvversitaOutput>(json)
                ?? throw new DeserializationException("Risposta nulla da MisureXAvversita WS");

            if (!string.IsNullOrEmpty(output.MessaggioErrore))
                throw new WebServiceException(
                    $"Errore WS MisureXAvversita: {output.MessaggioErrore}"
                );

            if (personalizzate)
                return output
                    .ListaMisureXAvversita.Select(m => new MisuraAvversitaPersonalizzataEntity
                    {
                        codice = m.Cod,
                        udmCod = m.UdmCod,
                        avversitaCod = m.AvCod,
                        specieCod = m.VegCod,
                        fondamentale = 1,
                        ff_Cod = m.FfCod,
                        ordine = m.Ordine,
                        gruppoAvversitaCod = m.AvGru,
                        pivasuperuser = m.Pivasuperuser,
                    })
                    .ToList();

            return output
                .ListaMisureXAvversita.Select(m => new MisuraAvversitaEntity
                {
                    codice = m.Cod,
                    udmCod = m.UdmCod,
                    avversitaCod = m.AvCod,
                    specieCod = m.VegCod,
                    fondamentale = 1,
                    ff_Cod = m.FfCod,
                    ordine = m.Ordine,
                    gruppoAvversitaCod = m.AvGru,
                })
                .ToList();
        }
    }
}

