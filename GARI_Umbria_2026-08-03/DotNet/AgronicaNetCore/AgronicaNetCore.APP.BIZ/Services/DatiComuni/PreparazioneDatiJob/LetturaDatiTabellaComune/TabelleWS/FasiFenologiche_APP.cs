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
    public class FasiFenologicheApp : BaseServiceAppBIZ
    {
        private readonly ILoggingService _loggingService;
        private readonly ChiamaWebService _wsClient;
        private readonly ISecurityLayerDAL _securityLayerDal;

        public FasiFenologicheApp(
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

            var input = new FasiFenologicheInput
            {
                Personalizzate = personalizzate,
                PivaSuperuser = parameters.ObjParametriTriple.ObjParametriServer.PivaSuperUser,
                LinguaCod = parameters.ObjParametriTriple.ObjParametriServer.Lingua_Cod,
                Url = url + "/FasiFenologiche",
                EstraiPersonalizzatePerAPP = personalizzate
            };

            var json = await _wsClient.ChiamaWebServiceAsync(
                input.Url,
                input,
                parameters.BearerToken
            );

            var output =
                JsonConvert.DeserializeObject<FasiFenologicheOutput>(json)
                ?? throw new DeserializationException("Risposta nulla da FasiFenologiche WS");

            if (!string.IsNullOrEmpty(output.MessaggioErrore))
                throw new WebServiceException(
                    $"Errore WS FasiFenologiche: {output.MessaggioErrore}"
                );

            if (personalizzate)
            {
                // Prima chiamata: pivasuperuser "normale"
                var listaPersonalizzate = output
                    .ListaFasiFenologiche.Select(
                        m => new SpecieVegetaliStadiCrescitaPersonalizzataEntity
                        {
                            codice = m.CodSs,
                            specieCod = m.VegCod,
                            ID_BBCH = m.IdBbch,
                            descrizione = $"{m.Descrizione} (BBCH {m.Stadio})",
                            codiceFF = m.FfCod,
                            visibile = m.Visibile != 0,
                            fioritura = m.Fioritura != 0,
                            pivasuperuser = m.Pivasuperuser,
                        }
                    )
                    .ToList();

                // Seconda chiamata: personalizzate=false, perchè chi non ha le personalizzazioni deve vedere tutte quelle non personalizzate (lo gestisce il webservice)
                var inputDefault = new FasiFenologicheInput
                {
                    Personalizzate = false,
                    LinguaCod = parameters.ObjParametriTriple.ObjParametriServer.Lingua_Cod,
                    Url = url + "/FasiFenologiche",
                    EstraiPersonalizzatePerAPP = true
                };
                var jsonDefault = await _wsClient.ChiamaWebServiceAsync(
                    inputDefault.Url,
                    inputDefault,
                    parameters.BearerToken
                );
                var outputDefault =
                    JsonConvert.DeserializeObject<FasiFenologicheOutput>(jsonDefault)
                    ?? throw new DeserializationException("Risposta nulla da FasiFenologiche WS (default)");
                if (!string.IsNullOrEmpty(outputDefault.MessaggioErrore))
                    throw new WebServiceException(
                        $"Errore WS FasiFenologiche (default): {outputDefault.MessaggioErrore}"
                    );
                var listaDefault = outputDefault
                    .ListaFasiFenologiche.Select(
                        m => new SpecieVegetaliStadiCrescitaPersonalizzataEntity
                        {
                            codice = m.CodSs,
                            specieCod = m.VegCod,
                            ID_BBCH = m.IdBbch,
                            descrizione = $"{m.Descrizione} (BBCH {m.Stadio})",
                            codiceFF = m.FfCod,
                            visibile = m.Visibile != 0,
                            fioritura = m.Fioritura != 0,
                            pivasuperuser = m.Pivasuperuser,
                        }
                    )
                    .ToList();
                // Accoda i risultati
                listaPersonalizzate.AddRange(listaDefault);
                return listaPersonalizzate;
            }
            else
            {
                return output
                    .ListaFasiFenologiche.Select(m => new SpecieVegetaliStadiCrescitaEntity
                    {
                        codice = m.CodSs,
                        specieCod = m.VegCod,
                        ID_BBCH = m.IdBbch,
                        descrizione = $"{m.Descrizione} (BBCH {m.Stadio})",
                        codiceFF = m.FfCod,
                        visibile = m.Visibile != 0,
                        fioritura = m.Fioritura != 0,
                    })
                    .ToList();
            }
        }
    }
}

