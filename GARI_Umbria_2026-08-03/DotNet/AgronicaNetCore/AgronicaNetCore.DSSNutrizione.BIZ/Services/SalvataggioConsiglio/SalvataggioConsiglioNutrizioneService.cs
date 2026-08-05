using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.DSSNutrizione.BIZ.Resources;
using AgronicaNetCore.DSSNutrizione.BIZ.Services.Engine.ConsiglioNutrizione.Models;
using AgronicaNetCore.DSSNutrizione.BIZ.Services.SalvataggioConsiglio.Models;
using AgronicaNetCore.DSSNutrizione.BIZ.Services.WidgetNutrizione.Models;
using AgronicaNetCore.DSSNutrizione.DAL.DataLayer.Engine.ConsiglioNutrizione;
using AgronicaNetCore.DSSNutrizione.DAL.DataLayer.Engine.ConsiglioNutrizione.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.DSSNutrizione.BIZ.Services.SalvataggioConsiglio
{
    /// <summary>
    /// Implementazione BIZ per il salvataggio esplicito del consiglio nutrizionale.
    /// Verifica l'assenza di duplicati per (PIVA, SA_COD, APPEZZA, ID_REG, Data_Consiglio)
    /// e inserisce un record per ogni elemento NPK in Consigli_Nutrizione_Engine.
    /// </summary>
    /// <remarks>
    /// Design Specification: DS07-BL Salvataggio Consiglio Nutrizione — Descrizione e Regole di Business.
    /// DS16-API POST /v1/dss/nutrizione/consigli/salva — Dipendenze Business Logic.
    /// </remarks>
    public sealed class SalvataggioConsiglioNutrizioneService : BaseDSSNutrizioneBIZService, ISalvataggioConsiglioNutrizioneService
    {
        private readonly IConsiglioNutrizioneDAL _consiglioNutrizioneDAL;

        public SalvataggioConsiglioNutrizioneService(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer)
            : base(provider, localizer)
        {
            _consiglioNutrizioneDAL = provider.GetRequiredService<IConsiglioNutrizioneDAL>();
        }

        /// <inheritdoc/>
        public async Task<SalvataggioConsiglioNutrizioneResponse> EseguiAsync(
            AggregazioneConsiglioNutrizioneResult aggregazione,
            bool controllaDuplicati,
            string username,
            AgronicaCoreParametriServer objParametriServer,
            CancellationToken cancellationToken = default)
        {
            if (aggregazione is null)
                throw new ArgumentNullException(nameof(aggregazione));
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username obbligatorio.", nameof(username));
            if (objParametriServer is null)
                throw new ArgumentNullException(nameof(objParametriServer));

            cancellationToken.ThrowIfCancellationRequested();

            var appId = aggregazione.Appezzamento;

            // DS07-BL — Data_Consiglio dalla risposta aggregata; default a oggi se assente o non valida.
            DateTime dataConsiglio = DateTime.TryParse(
                aggregazione.ConsiglioNutrizione.DataConsiglio, out var dt)
                ? dt
                : DateTime.UtcNow.Date;

            // DS07-BL — Regole di Business: verifica duplicati solo se il flag ControllaDuplicati è true.
            if (controllaDuplicati)
            {
                bool exists = await _consiglioNutrizioneDAL.ExistsConsiglioNutrizioneAsync(
                    appId.Piva, appId.SaCod, appId.Appezza, appId.IdReg, CostantiPersonalizzate.AGRODATAINIZIO_DATE, objParametriServer);

                if (exists)
                {
                    return new SalvataggioConsiglioNutrizioneResponse
                    {
                        SalvataggioId   = 0,
                        Esito           = "DUPLICATE",
                        DataSalvataggio = DateTime.UtcNow.ToString("O"),
                        Messaggio       = _localizer["ConsiglioGiaSalvato"]
                    };
                }
            }

            // DS07-BL — Inserimento transazionale: un record per ogni elemento NPK.
            int firstInsertedId = 0;
            foreach (var elemento in aggregazione.ConsiglioNutrizione.Elementi)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var input = new InsertConsiglioNutrizioneInput
                {
                    Piva                       = appId.Piva,
                    SaCod                      = appId.SaCod,
                    Appezza                    = appId.Appezza,
                    IdReg                      = appId.IdReg,
                    DataConsiglio              = dataConsiglio,
                    DataSemina                 = dataConsiglio,   // non persista nel DB (vedere SQL), usato solo per il modello
                    Elemento                   = elemento.Elemento,
                    FabbisognoMinimo           = elemento.FabbisognoMinimo,
                    FabbisognoMassimo          = elemento.FabbisognoMassimo,
                    DoseConsigliataMiniima     = elemento.DoseConsigliataMinima,
                    DoseConsigliataMassima     = elemento.DoseConsigliataMassima,
                    QuantitativoPresente       = elemento.QuantitativoPresente,
                    QuantitativoMinimoResiduo  = elemento.QuantitativoMinimoResiduo,
                    QuantitativoMassimoResiduo = elemento.QuantitativoMassimoResiduo,
                    Messaggi                   = elemento.Message,
                    UsernameCreazione          = username
                };

                int id = await _consiglioNutrizioneDAL.InsertConsiglioNutrizioneAsync(input, objParametriServer);
                if (firstInsertedId == 0)
                    firstInsertedId = id;
            }

            return new SalvataggioConsiglioNutrizioneResponse
            {
                SalvataggioId   = firstInsertedId,
                Esito           = "SUCCESS",
                DataSalvataggio = DateTime.UtcNow.ToString("O"),
                Messaggio       = _localizer["ConsiglioSalvatoConSuccesso"]
            };
        }

        /// <inheritdoc/>
        public async Task<int?> EseguiAsync(
            RichiestaConsiglioNutrizioneInput input,
            EsitoConsiglioNutrizione esitoEngine,
            string usernameRichiedente,
            AgronicaCoreParametriServer objParametriServer,
            CancellationToken cancellationToken = default)
        {
            if (input is null)          throw new ArgumentNullException(nameof(input));
            if (esitoEngine is null)    throw new ArgumentNullException(nameof(esitoEngine));
            if (string.IsNullOrWhiteSpace(usernameRichiedente))
                throw new ArgumentException("Username obbligatorio.", nameof(usernameRichiedente));
            if (objParametriServer is null) throw new ArgumentNullException(nameof(objParametriServer));

            if (esitoEngine.Results?.Outcome?.Consigli is null || esitoEngine.Results.Outcome.Consigli.Count == 0)
                return null;

            cancellationToken.ThrowIfCancellationRequested();

            int? primoConsiglioId = null;

            var messaggi = esitoEngine.Results.Outcome.Messaggi is { Count: > 0 } msgs
                ? string.Join("; ", msgs)
                : null;

            bool connectionOpened = false;
            try
            {
                await OpenConnectionAsync(objParametriServer, OpenTransaction: true);
                connectionOpened = true;

                foreach (var elemento in esitoEngine.Results.Outcome.Consigli)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var consiglioInput = new InsertConsiglioNutrizioneInput
                    {
                        Piva                       = input.DatiImpianto.Piva,
                        SaCod                      = input.DatiImpianto.SaCod,
                        Appezza                    = input.DatiImpianto.Appezza,
                        IdReg                      = input.DatiImpianto.IdReg,
                        DataConsiglio              = input.DataConsiglio,
                        DataSemina                 = input.DataSemina,
                        Elemento                   = elemento.Elemento,
                        FabbisognoMinimo           = elemento.FabbisognoMinimo,
                        FabbisognoMassimo          = elemento.FabbisognoMassimo,
                        DoseConsigliataMiniima     = elemento.DoseConsigliataMinima,
                        DoseConsigliataMassima     = elemento.DoseConsigliataMassima,
                        QuantitativoPresente       = elemento.QuantitativoGiaPresente,
                        QuantitativoMinimoResiduo  = elemento.QuantitativoMinimoResiduo,
                        QuantitativoMassimoResiduo = elemento.QuantitativoMassimoResiduo,
                        Messaggi                   = messaggi,
                        UsernameCreazione          = usernameRichiedente
                    };

                    var consiglioId = await _consiglioNutrizioneDAL.InsertConsiglioNutrizioneAsync(consiglioInput, objParametriServer);
                    primoConsiglioId ??= consiglioId;

                    // Audit: fase fenologica (se disponibile)
                    if (input.DatiFaseFenologica is not null)
                    {
                        await _consiglioNutrizioneDAL.InsertInputConsiglioNutrizioneAsync(new InsertInputConsiglioNutrizioneInput
                        {
                            IdConsiglio       = consiglioId,
                            IdAgenda          = input.DatiFaseFenologica.IdAgenda,
                            IdMov             = input.DatiFaseFenologica.IdMov,
                            IdMovDet          = input.DatiFaseFenologica.IdMovDet,
                            UsernameCreazione = usernameRichiedente
                        }, objParametriServer);
                    }

                    // Audit: analisi terreno (se disponibile)
                    if (input.DatiAnalisiTerreno is not null)
                    {
                        await _consiglioNutrizioneDAL.InsertInputConsiglioNutrizioneAsync(new InsertInputConsiglioNutrizioneInput
                        {
                            IdConsiglio          = consiglioId,
                            AnalisiSuperUser     = input.DatiAnalisiTerreno.AnalisiSuperUser,
                            AnalisiTestataCod    = input.DatiAnalisiTerreno.AnalisiTestataCod,
                            AnalisiDettaglioCod  = input.DatiAnalisiTerreno.AnalisiDettaglioCod,
                            AnalisiParametroCod  = input.DatiAnalisiTerreno.AnalisiParametroCod,
                            UsernameCreazione    = usernameRichiedente
                        }, objParametriServer);
                    }

                    // Audit: fertilizzazioni precedenti
                    foreach (var fertilizzazione in input.DatiFertilizzazioniPrecedenti)
                    {
                        await _consiglioNutrizioneDAL.InsertInputConsiglioNutrizioneAsync(new InsertInputConsiglioNutrizioneInput
                        {
                            IdConsiglio       = consiglioId,
                            IdAgenda          = fertilizzazione.IdAgenda,
                            IdMov             = fertilizzazione.IdMov,
                            IdMovDet          = fertilizzazione.IdMovDet,
                            UsernameCreazione = usernameRichiedente
                        }, objParametriServer);
                    }
                }

                CloseTransaction(objParametriServer, Rollback: false);
                CloseConnection(objParametriServer);
                connectionOpened = false;
            }
            catch (Exception)
            {
                if (connectionOpened)
                    CloseTransaction(objParametriServer, Rollback: true);
                throw;
            }

            return primoConsiglioId;
        }
    }
}
