using AgronicaCoreDTOStd.InData.Metaschema;
using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiComuni;
using AgronicaCoreModelsSTD.metaschema;
using AgronicaCoreModelsSTD.metaschema.utilizzi;
using AgronicaNetCore.APP.BIZ.Exceptions;
using AgronicaNetCore.APP.BIZ.Resources;
using AgronicaNetCore.Base.DataLayer.Security;
using AgronicaNetCore.Base.Utility;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.TabelleComuni_APP;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using AgronicaNetCore.Base.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AgronicaNetCore.APP.BIZ.Services.LetturaDatiTabellaComune.TabelleWS
{
    public class DisciplinariApp : BaseServiceAppBIZ, ILetturaTabellaComuneApp
    {
        private readonly ILoggingService _loggingService;
        private readonly ChiamaCoreWS _wsClient;
        private readonly ISecurityLayerDAL _securityLayerDal;

        public DisciplinariApp(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer
        )
            : base(provider, localizer)
        {
            _loggingService = _serviceProvider.GetRequiredService<ILoggingService>();
            _wsClient = _serviceProvider.GetRequiredService<ChiamaCoreWS>();
            _securityLayerDal = _serviceProvider.GetRequiredService<ISecurityLayerDAL>();
        }

        private async Task<object> LeggiAsync(LetturaTabellaComuneAppParameters parameters)
        {
            var url = parameters.coreWsUrl + "/AgronicaCoreDPI/DPI.asmx/CaricaComboDisciplinare_Modello";

            var input = new LeggiDisciplinari
            {
                specie = new Specie(0),
                data = new DateTime(),
                privato = true,
            };

            var json = await _wsClient.ChiamaCoreWSAsync(
                url,
                input,
                parameters.ObjParametriTriple,
                parameters.BearerToken
            );

            // Il servizio ASMX/ScriptService wrappa la risposta in {"d": { ... }}
            // RispostaStandard(Of T) VB non è direttamente usabile come generic C#:
            // si usa JObject per navigare l'envelope e deserializzare RispostaStringa.
            JObject jObj = JObject.Parse(json);
            JToken d = jObj["d"]
                ?? throw new DeserializationException("Risposta nulla da Disciplinari WS");

            bool rispostaOk = d["RispostaOK"]?.Value<bool>() ?? false;
            if (!rispostaOk)
                throw new DeserializationException(
                    $"Errore WS Disciplinari: {d["Errore"]?.Value<string>()}"
                );

            List<Disciplinare> output = d["RispostaStringa"]?.ToObject<List<Disciplinare>>()
                ?? throw new DeserializationException("RispostaStringa nulla da Disciplinari WS");

            return output
                .Select(d => new DisciplinareEntity
                {
                    codice = d.codice,
                    descrizione = d.descrizione,
                    regConcimazioneCod = d.regolamentoConcimazione?.codice ?? 0,
                    regConcimazioneDesc = d.regolamentoConcimazione?.descrizione,
                    disciplinarePubblicoPrivato = d.disciplinarePubblicoPrivato,
                    flagProtetto = d.flagProtetto,
                    idTr = d.idTr,
                    inizioValidita = d.validita?.inizio ?? DateTime.MinValue,
                    fineValidita = d.validita?.fine ?? DateTime.MaxValue,
                })
                .ToList();
        }

        Task<object> ILetturaTabellaComuneApp.LeggiAsync(
            LetturaTabellaComuneAppParameters parameters
        )
        {
            return LeggiAsync(parameters);
        }
    }
}

