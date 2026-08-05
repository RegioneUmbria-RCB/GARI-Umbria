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
    public class MisureDanniRaccoltaApp : BaseServiceAppBIZ
    {
        private readonly ILoggingService _loggingService;
        private readonly ChiamaWebService _wsClient;
        private readonly ISecurityLayerDAL _securityLayerDal;

        public MisureDanniRaccoltaApp(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer
        )
            : base(provider, localizer)
        {
            _loggingService = _serviceProvider.GetRequiredService<ILoggingService>();
            _wsClient = _serviceProvider.GetRequiredService<ChiamaWebService>();
            _securityLayerDal = _serviceProvider.GetRequiredService<ISecurityLayerDAL>();
        }

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

            var input = new MisuraXDanniRaccoltaInput
            {
                Personalizzate = personalizzate,
                PivaSuperuser = parameters.ObjParametriTriple.ObjParametriServer.PivaSuperUser,
                FiltraSpecie = false,
                Url = url + "/MisureXDanniRaccolta",
                EstraiPersonalizzatePerAPP = personalizzate
            };

            var json = await _wsClient.ChiamaWebServiceAsync(
                input.Url,
                input,
                parameters.BearerToken
            );

            var output =
                JsonConvert.DeserializeObject<MisuraXDanniRaccoltaOutput>(json)
                ?? throw new DeserializationException("Risposta nulla da MisureXDanniRaccolta WS");

            if (!string.IsNullOrEmpty(output.MessaggioErrore))
                throw new WebServiceException(
                    $"Errore WS MisureXDanniRaccolta: {output.MessaggioErrore}"
                );

            if (personalizzate)
                return output
                    .ListaMisureXDanniRaccolta.Select(
                        m => new MisuraDanniRaccoltaPersonalizzataEntity
                        {
                            specieCod = m.VegCod,
                            codice = m.DrCod,
                            udmCod = m.UdmCod,
                            descrizione = m.DrDes,
                            udmDescrizione = m.UdmDes,
                            udmSimbolo = m.UdmSim,
                            flag_visibile = m.Visibile,
                            pivasuperuser = m.Pivasuperuser,
                        }
                    )
                    .ToList();
            else
                return output
                    .ListaMisureXDanniRaccolta.Select(m => new MisuraDanniRaccoltaEntity
                    {
                        specieCod = m.VegCod,
                        codice = m.DrCod,
                        udmCod = m.UdmCod,
                        descrizione = m.DrDes,
                        udmDescrizione = m.UdmDes,
                        udmSimbolo = m.UdmSim,
                        flag_visibile = m.Visibile,
                    })
                    .ToList();
        }
    }
}

