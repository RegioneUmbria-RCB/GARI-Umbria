using System.Data;
using AgronicaCoreModelsSTD.Gis;
using AgronicaNetCore.APP.BIZ.Common;
using AgronicaNetCore.APP.BIZ.Exceptions;
using AgronicaNetCore.APP.BIZ.Resources;
using AgronicaNetCore.APP.BIZ.Services.DatiAzienda.AreaTipologie;
using AgronicaNetCore.APP.BIZ.Services.DatiAzienda.AttivitaCDG;
using AgronicaNetCore.APP.BIZ.Services.DatiAzienda.AttivitaxCentriAziendali;
using AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Campi;
using AgronicaNetCore.APP.BIZ.Services.DatiAzienda.CentriAziendali;
using AgronicaNetCore.APP.BIZ.Services.DatiAzienda.CodificaProdotti;
using AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Contatti;
using AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Impostazioni;
using AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Imprese;
using AgronicaNetCore.APP.BIZ.Services.DatiAzienda.LavorazioniAttivitaCDG;
using AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Magazzini;
using AgronicaNetCore.APP.BIZ.Services.DatiAzienda.MisureAvversitaAnagrafiche;
using AgronicaNetCore.APP.BIZ.Services.DatiAzienda.MisureIndiciMaturitaAnagrafiche;
using AgronicaNetCore.APP.BIZ.Services.DatiAzienda.OperazioniCausali;
using AgronicaNetCore.APP.BIZ.Services.DatiAzienda.ParcoMacchine;
using AgronicaNetCore.APP.BIZ.Services.DatiAzienda.PianoColturale;
using AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Prodotti;
using AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Progetti;
using AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Squadre;
using AgronicaNetCore.APP.BIZ.Services.ImpostazioniApp;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiImpostazioni;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Sincronizzazione.AcquisizioneDatiTabelle;

/// <summary>
/// Implements DS04-BL data-acquisition: routes each table marked for sync to the correct
/// BIZ service and returns the aggregated company dataset.
/// Any service error propagates immediately as a global exception (fail-fast).
/// Ref: DS04-BL – Nome: AcquisizioneDatiTabelle.
/// </summary>
public sealed class AcquisizioneDatiTabelle_APPService
    : BaseServiceAppBIZ,
        IAcquisizioneDatiTabelle_APPService
{
    private readonly IImprese_APPService _imprese;
    private readonly ICentriAziendali_APPService _centriAziendali;
    private readonly ICampi_APPService _campi;
    private readonly IMagazzini_APPService _magazzini;
    private readonly IPianoColturale_APPService _pianoColturale;
    private readonly IContatti_APPService _contatti;
    private readonly ISquadre_APPService _squadre;
    private readonly IParcoMacchine_APPService _parcoMacchine;
    private readonly IProgetti_APPService _progetti;
    private readonly IImpostazioni_APPService _impostazioni;
    private readonly IAttivitaCDG_APPService _attivitaCDG;
    private readonly ILavorazioniAttivitaCDG_APPService _lavorazioniAttivitaCDG;
    private readonly IAreaTipologie_APPService _areaTipologie;
    private readonly ICodificaProdotti_APPService _codificaProdotti;
    private readonly IMisureAvversitaAnagrafiche_APPService _misureAvversitaAnagrafiche;
    private readonly IMisureIndiciMaturitaAnagrafiche_APPService _misureIndiciMaturitaAnagrafiche;
    private readonly IOperazioniCausaliAPP_Service _operazioniCausali;
    private readonly IProdotti_APPService _prodotti;
    private readonly IAttivitaxCentriAziendali_APPService _attivitaxCentriAziendali;
    private readonly IUtentiImpostazioni _utentiImpostazioni;

    public AcquisizioneDatiTabelle_APPService(
        IServiceProvider provider,
        IStringLocalizer<Messages> localizer,
        IImprese_APPService imprese,
        ICentriAziendali_APPService centriAziendali,
        ICampi_APPService campi,
        IMagazzini_APPService magazzini,
        IPianoColturale_APPService pianoColturale,
        IContatti_APPService contatti,
        ISquadre_APPService squadre,
        IParcoMacchine_APPService parcoMacchine,
        IProgetti_APPService progetti,
        IImpostazioni_APPService impostazioni,
        IAttivitaCDG_APPService attivitaCDG,
        ILavorazioniAttivitaCDG_APPService lavorazioniAttivitaCDG,
        IAreaTipologie_APPService areaTipologie,
        ICodificaProdotti_APPService codificaProdotti,
        IMisureAvversitaAnagrafiche_APPService misureAvversitaAnagrafiche,
        IMisureIndiciMaturitaAnagrafiche_APPService misureIndiciMaturitaAnagrafiche,
        IOperazioniCausaliAPP_Service operazioniCausali,
        IProdotti_APPService prodotti,
        IAttivitaxCentriAziendali_APPService attivitaxCentriAziendali,
        IUtentiImpostazioni utentiImpostazioni
    )
        : base(provider, localizer)
    {
        _imprese = imprese;
        _centriAziendali = centriAziendali;
        _campi = campi;
        _magazzini = magazzini;
        _pianoColturale = pianoColturale;
        _contatti = contatti;
        _squadre = squadre;
        _parcoMacchine = parcoMacchine;
        _progetti = progetti;
        _impostazioni = impostazioni;
        _attivitaCDG = attivitaCDG;
        _lavorazioniAttivitaCDG = lavorazioniAttivitaCDG;
        _areaTipologie = areaTipologie;
        _codificaProdotti = codificaProdotti;
        _misureAvversitaAnagrafiche = misureAvversitaAnagrafiche;
        _misureIndiciMaturitaAnagrafiche = misureIndiciMaturitaAnagrafiche;
        _operazioniCausali = operazioniCausali;
        _prodotti = prodotti;
        _attivitaxCentriAziendali = attivitaxCentriAziendali;
        _utentiImpostazioni = utentiImpostazioni;
    }

    /// <inheritdoc/>
    public async Task<AcquisizioneDatiTabelle_Result> AcquisisciDatiAsync(
        AcquisizioneDatiTabelle_Input input
    )
    {
        ArgumentNullException.ThrowIfNull(input);

        if (string.IsNullOrWhiteSpace(input.Request.Piva))
        {
            throw new InvalidInputException(
                "DS04-BL: il campo 'piva' è obbligatorio. Ref: DS04-BL – Eccezioni: InvalidAziendaException."
            );
        }

        var result = new AcquisizioneDatiTabelle_Result();
        var piva = input.Request.Piva;
        var parametriServer = input.ObjParametriServer;
        var parametriUtenti = input.ObjParametriUtenti;
        var parametriSuperServer = input.ObjParametriSuperServer;

        try
        {
            await OpenConnectionAsync(
                parametriServer,
                OpenTransaction: false,
                IsolationLevel.ReadUncommitted
            );

            var _jsonImpostazioniApp =
                await _utentiImpostazioni.ImpostazioneValore1_from_ImpostazioneCod(
                    Enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_IMPOSTAZIONI,
                    (int)ModalitaLeggiImpostazioniUtente.SuperUser,
                    parametriUtenti,
                    parametriServer
                );
            ImpostazioniAppModel impostazioniApp = !string.IsNullOrEmpty(_jsonImpostazioniApp)
                ? JsonConvert.DeserializeObject<ImpostazioniAppModel>(_jsonImpostazioniApp)
                    ?? new ImpostazioniAppModel()
                : new ImpostazioniAppModel();

            bool modalitaDemetra = input.ModalitaDemetra;

            // ── 1. Azienda ────────────────────────────────────────────────────────
            // Ref: DS04-BL – Routing BIZ service: Azienda → Imprese_APPService.
            if (DeveSincronizzare(input, DatiAziendaTabelleNomi.Azienda))
                result.Azienda = await _imprese.LeggiImpresaAsync(piva, parametriServer);

            // ── 2. CentriAziendali ────────────────────────────────────────────────
            // Ref: DS04-BL – Routing BIZ service: CentriAziendali → ICentriAziendali_APPService.
            if (DeveSincronizzare(input, DatiAziendaTabelleNomi.CentriAziendali))
            {
                result.CentriAziendali = await _centriAziendali.LeggiCentriAziendaliAsync(
                    piva,
                    parametriServer
                );
                result.CentriAziendali = NullIfEmpty(result.CentriAziendali);
            }

            // ── 2b. CentriAziendaliAttivitaCDG ────────────────────────────────────
            if (DeveSincronizzare(input, DatiAziendaTabelleNomi.CentriAziendaliAttivitaCDG))
            {
                result.CentriAziendaliAttivitaCDG =
                    await _attivitaxCentriAziendali.LeggiAttivitaxCentriAziendaliAsync(
                        piva,
                        parametriServer
                    );
                result.CentriAziendaliAttivitaCDG = NullIfEmpty(result.CentriAziendaliAttivitaCDG);
            }

            // ── 9. Progetti ───────────────────────────────────────────────────────
            // Ref: DS04-BL – Routing BIZ service: Progetti → IProgetti_APPService.
            if (DeveSincronizzare(input, DatiAziendaTabelleNomi.Progetti))
            {
                result.Progetti = await _progetti.LeggiProgettiAsync(piva, parametriServer);
                result.Progetti = NullIfEmpty(result.Progetti);
            }

            bool leggiPianoColturaleCompleto = input.Request.AnagPianoColturale;
            // ── 3. PianoColturale ─────────────────────────────────────────────────
            // Ref: DS04-BL – Routing BIZ service: PianoColturale → IPianoColturale_APPService.
            if (DeveSincronizzare(input, DatiAziendaTabelleNomi.PianoColturale))
                result.PianoColturale = await _pianoColturale.LeggiPianoColturaleAsync(
                    piva,
                    data: DateTime.Today,
                    leggiCompleto: leggiPianoColturaleCompleto,
                    parametriUtenti,
                    parametriServer,
                    parametriSuperServer,
                    impostazioniApp,
                    modalitaDemetra
                );

            // ── 4. Campi ──────────────────────────────────────────────────────────
            // Ref: DS04-BL – Routing BIZ service: Campi → ICampi_APPService.
            if (DeveSincronizzare(input, DatiAziendaTabelleNomi.Campi))
            {
                result.Campi = await _campi.LeggiCampiAsync(piva, parametriServer);
                result.Campi = NullIfEmpty(result.Campi);
            }

            // ── 5. Magazzini ──────────────────────────────────────────────────────
            // Ref: DS04-BL – Routing BIZ service: Magazzini → IMagazzini_APPService.
            if (DeveSincronizzare(input, DatiAziendaTabelleNomi.Magazzini))
            {
                result.Magazzini = await _magazzini.LeggiMagazziniAsync(piva, parametriServer);
                result.Magazzini = NullIfEmpty(result.Magazzini);
            }

            if (DeveSincronizzare(input, DatiAziendaTabelleNomi.Squadre))
            {
                result.Squadre = await _squadre.LeggiAsync(piva, parametriServer, parametriUtenti);
                result.Squadre = NullIfEmpty(result.Squadre);
            }

            // ── 6. Contatti, RisorseUmane, Fornitori, RisorseFornitori, ContattiStazioniMeteo ─
            // A single IContatti_APPService call is made with the union of required flags to avoid
            // redundant I/O, then the result is distributed to each table.
            // Ref: DS04-BL – Routing BIZ service: Contatti/RisorseUmane/Fornitori/RisorseFornitori/
            //               ContattiStazioniMeteo → IContatti_APPService.
            bool syncContatti =
                DeveSincronizzare(input, DatiAziendaTabelleNomi.Contatti)
                || DeveSincronizzare(input, DatiAziendaTabelleNomi.RisorseUmane);

            bool syncFornitori =
                DeveSincronizzare(input, DatiAziendaTabelleNomi.Fornitori)
                || DeveSincronizzare(input, DatiAziendaTabelleNomi.RisorseFornitori);

            bool syncFornitoriMeteo =
                DeveSincronizzare(input, DatiAziendaTabelleNomi.ContattiStazioniMeteo)
                && impostazioniApp.StazioniMeteo;

            if (syncContatti || syncFornitori || syncFornitoriMeteo)
            {
                var contattiResult = await _contatti.LeggiAsync(
                    new Contatti_APPRequest
                    {
                        Piva = piva,
                        IncludeContatti = syncContatti,
                        IncludeFornitori = syncFornitori,
                        IncludeFornitoriMeteo = syncFornitoriMeteo,
                        ModalitaDemetra = modalitaDemetra,
                    },
                    parametriServer
                );

                if (syncContatti)
                {
                    result.Contatti = contattiResult.Contatti;
                    result.RisorseUmane = contattiResult.RisorseUmane;
                    result.Contatti = NullIfEmpty(result.Contatti);
                    result.RisorseUmane = NullIfEmpty(result.RisorseUmane);
                }

                if (syncFornitori)
                {
                    result.Fornitori = contattiResult.Fornitori;
                    result.RisorseFornitori = contattiResult.RisorseFornitori;
                }

                if (syncFornitoriMeteo)
                {
                    result.Fornitori.AddRange(contattiResult.FornitoriMeteo);
                    result.RisorseFornitori.AddRange(contattiResult.RisorseFornitoriMeteo);
                }

                if (syncFornitori || syncFornitoriMeteo)
                {
                    result.Fornitori = NullIfEmpty(result.Fornitori);
                    result.RisorseFornitori = NullIfEmpty(result.RisorseFornitori);
                }
            }

            // ── 7. ParcoMacchine ──────────────────────────────────────────────────
            // Ref: DS04-BL – Routing BIZ service: ParcoMacchine → IParcoMacchine_APPService.
            if (DeveSincronizzare(input, DatiAziendaTabelleNomi.ParcoMacchine))
            {
                result.ParcoMacchine = await _parcoMacchine.LeggiParcoMacchineAsync(
                    piva,
                    parametriServer
                );
                result.ParcoMacchine = NullIfEmpty(result.ParcoMacchine);
            }

            // ── 10. ImpreseImpostazioni ───────────────────────────────────────────
            // Ref: DS04-BL – Routing BIZ service: ImpreseImpostazioni → IImpostazioni_APPService.
            if (DeveSincronizzare(input, DatiAziendaTabelleNomi.ImpreseImpostazioni))
            {
                result.ImpreseImpostazioni = await _impostazioni.LeggiImpostazioniAsync(
                    piva,
                    parametriServer
                );
                result.ImpreseImpostazioni = NullIfEmpty(result.ImpreseImpostazioni);
            }

            // ── 11. AttivitaCDG ───────────────────────────────────────────────────
            // Ref: DS04-BL – Routing BIZ service: AttivitaCDG → IAttivitaCDG_APPService.
            if (DeveSincronizzare(input, DatiAziendaTabelleNomi.AttivitaCDG))
            {
                result.AttivitaCDG = await _attivitaCDG.LeggiAttivitaCDGAsync(
                    parametriServer
                );
                result.AttivitaCDG = NullIfEmpty(result.AttivitaCDG);
            }

            // ── 12. LavorazioniAttivitaCDG ────────────────────────────────────────
            // Ref: DS04-BL – Routing BIZ service: LavorazioniAttivitaCDG → ILavorazioniAttivitaCDG_APPService.
            if (DeveSincronizzare(input, DatiAziendaTabelleNomi.LavorazioniAttivitaCDG))
            {
                result.LavorazioniAttivitaCDG =
                    await _lavorazioniAttivitaCDG.LeggiLavorazioniAttivitaCDGAsync(
                        piva,
                        parametriServer
                    );
                result.LavorazioniAttivitaCDG = NullIfEmpty(result.LavorazioniAttivitaCDG);
            }

            // ── 13. AreaTipologie + Tipologie ─────────────────────────────────────
            // Ref: DS04-BL – Routing BIZ service: AreaeTipologie → IAreaTipologie_APPService
            //               (single call returns both areeTipologie and tipologie).
            bool syncAreaTipologie = DeveSincronizzare(input, DatiAziendaTabelleNomi.AreaTipologie);
            bool syncTipologie = DeveSincronizzare(input, DatiAziendaTabelleNomi.Tipologie);
            if (syncAreaTipologie || syncTipologie)
            {
                var (aree, tipologie) = await _areaTipologie.LeggiTipologieAsync(parametriServer);
                if (syncAreaTipologie)
                {
                    result.AreaTipologie = aree;
                    result.AreaTipologie = NullIfEmpty(result.AreaTipologie);
                }

                if (syncTipologie)
                {
                    result.Tipologie = tipologie;
                    result.Tipologie = NullIfEmpty(result.Tipologie);
                }
            }

            // ── 14. CodificaProdotti ──────────────────────────────────────────────
            // Ref: DS04-BL – Routing BIZ service: CodificaProdotti → ICodificaProdotti_APPService.
            if (DeveSincronizzare(input, DatiAziendaTabelleNomi.CodificaProdotti))
            {
                result.CodificaProdotti = await _codificaProdotti.LeggiCodificaProdottiAsync(
                    parametriServer
                );
                result.CodificaProdotti = NullIfEmpty(result.CodificaProdotti);
            }

            // ── 15. MisureAvversitaAnagrafiche ────────────────────────────────────
            // Ref: DS04-BL – Routing BIZ service: MisureAvversitaAnagrafiche →
            //               IMisureAvversitaAnagrafiche_APPService (global data, no piva filter).
            if (DeveSincronizzare(input, DatiAziendaTabelleNomi.MisureAvversitaAnagrafiche))
            {
                result.MisureAvversitaAnagrafiche =
                    await _misureAvversitaAnagrafiche.LeggiMisureAvversitaAnagraficheAsync(
                        parametriServer
                    );
                result.MisureAvversitaAnagrafiche = NullIfEmpty(result.MisureAvversitaAnagrafiche);
            }

            // ── 16. MisureIndiciMaturitaAnagrafiche ───────────────────────────────
            // Ref: DS04-BL – Routing BIZ service: MisureIndiciMaturitaAnagrafiche →
            //               IMisureIndiciMaturitaAnagrafiche_APPService (global data, no piva filter).
            if (DeveSincronizzare(input, DatiAziendaTabelleNomi.MisureIndiciMaturitaAnagrafiche))
            {
                result.MisureIndiciMaturitaAnagrafiche =
                    await _misureIndiciMaturitaAnagrafiche.LeggiMisureIndiciMaturitaAnagraficheAsync(
                        parametriServer
                    );
                result.MisureIndiciMaturitaAnagrafiche = NullIfEmpty(result.MisureIndiciMaturitaAnagrafiche);
            }

            // ── 17. OperazioneCausale ─────────────────────────────────────────────
            // Ref: DS04-BL – Routing BIZ service: OperazioneCausale →
            //               AgronicaNetCore.Operazione.BIZ … OperazioneCausale_LeggiAsync(piva).
            // The piva-based method defined in the spec does not exist in the current implementation.
            // result.OperazioneCausale remains empty until the service is available.
            if (DeveSincronizzare(input, DatiAziendaTabelleNomi.OperazioneCausale))
            {
                result.OperazioneCausale = await _operazioniCausali.LeggiAsync(parametriServer);
                result.OperazioneCausale = NullIfEmpty(result.OperazioneCausale);
            }

            // ── 8. Prodotti ───────────────────────────────────────────────────────
            if (DeveSincronizzare(input, DatiAziendaTabelleNomi.ProdottiSementi))
                //verificare se tira su pubblici e privati insieme
                result.Prodotti.AddRange(
                    await _prodotti.LeggiProdottiAPPAsync(
                        piva,
                        ELEM_COD.SEMENTI,
                        "",
                        "",
                        !input.PermessiUtente.PianoColturale,
                        leggiNonMovimentati: DeveSincronizzare(
                            input,
                            DatiAziendaTabelleNomi.ProdottiSementiNonMovimentati
                        ),
                        parametriUtenti,
                        parametriServer,
                        impostazioniApp,
                        modalitaDemetra
                    )
                );

            if (DeveSincronizzare(input, DatiAziendaTabelleNomi.ProdottiTrasformatiVegetali))
                //verificare se tira su pubblici e privati insieme
                result.Prodotti.AddRange(
                    await _prodotti.LeggiProdottiAPPAsync(
                        piva,
                        ELEM_COD.TRASFORMATI_VEGETALI,
                        "",
                        "",
                        !input.PermessiUtente.PianoColturale,
                        leggiNonMovimentati: DeveSincronizzare(
                            input,
                            DatiAziendaTabelleNomi.ProdottiTrasformatiVegetaliNonMovimentati
                        ),
                        parametriUtenti,
                        parametriServer,
                        impostazioniApp,
                        modalitaDemetra
                    )
                );

            if (DeveSincronizzare(input, DatiAziendaTabelleNomi.ProdottiFormulati))
                result.Prodotti.AddRange(
                    await _prodotti.LeggiProdottiAPPAsync(
                        piva,
                        ELEM_COD.FORMULATI,
                        input.Request.SpecieProdotti,
                        input.Request.Nazione, //passare tramite parametro
                        !input.PermessiUtente.PianoColturale,
                        leggiNonMovimentati: DeveSincronizzare(
                            input,
                            DatiAziendaTabelleNomi.ProdottiFormulatiNonMovimentati
                        ),
                        parametriUtenti,
                        parametriServer,
                        impostazioniApp,
                        modalitaDemetra
                    )
                );

            if (DeveSincronizzare(input, DatiAziendaTabelleNomi.ProdottiFertilizzanti))
                //verificare se tira su pubblici e privati insieme
                result.Prodotti.AddRange(
                    await _prodotti.LeggiProdottiAPPAsync(
                        piva,
                        ELEM_COD.FERTILIZZANTI,
                        "", //passare tramite parametro
                        input.Request.Nazione,
                        !input.PermessiUtente.PianoColturale,
                        leggiNonMovimentati: DeveSincronizzare(
                            input,
                            DatiAziendaTabelleNomi.ProdottiFertilizzantiNonMovimentati
                        ),
                        parametriUtenti,
                        parametriServer,
                        impostazioniApp,
                        modalitaDemetra
                    )
                );

            if (DeveSincronizzare(input, DatiAziendaTabelleNomi.ProdottiInsetti))
                //verificare se tira su pubblici e privati insieme
                result.Prodotti.AddRange(
                    await _prodotti.LeggiProdottiAPPAsync(
                        piva,
                        ELEM_COD.INSETTI,
                        "", //passare tramite parametro
                        "",
                        !input.PermessiUtente.PianoColturale,
                        leggiNonMovimentati: false,
                        parametriUtenti,
                        parametriServer,
                        impostazioniApp,
                        modalitaDemetra
                    )
                );

            bool syncProdotti =
                DeveSincronizzare(input, DatiAziendaTabelleNomi.ProdottiSementi)
                || DeveSincronizzare(input, DatiAziendaTabelleNomi.ProdottiTrasformatiVegetali)
                || DeveSincronizzare(input, DatiAziendaTabelleNomi.ProdottiFormulati)
                || DeveSincronizzare(input, DatiAziendaTabelleNomi.ProdottiFertilizzanti)
                || DeveSincronizzare(input, DatiAziendaTabelleNomi.ProdottiInsetti);

            if (syncProdotti)
            {
                result.Prodotti = NullIfEmpty(result.Prodotti);
            }
        }
        catch (Exception ex)
        {
            LogError(ex.Message, parametriServer, ex);
            throw;
        }
        finally
        {
            CloseConnection(parametriServer);
        }
        return result;
    }

    // Returns true when the table is present in the input dictionary and marked for sync.
    // Tables absent from the dictionary are treated as not-to-sync.
    // Ref: DS04-BL – Regole di Business: Tabelle non sincronizzate.
    private static bool DeveSincronizzare(AcquisizioneDatiTabelle_Input input, string nomeTabella)
    {
        return input.TabellePerSincronizzazione.TryGetValue(nomeTabella, out var decisione)
            && decisione.Sincronizzare;
    }

    private static List<T>? NullIfEmpty<T>(List<T>? values)
    {
        return values is { Count: > 0 } ? values : null;
    }
}
