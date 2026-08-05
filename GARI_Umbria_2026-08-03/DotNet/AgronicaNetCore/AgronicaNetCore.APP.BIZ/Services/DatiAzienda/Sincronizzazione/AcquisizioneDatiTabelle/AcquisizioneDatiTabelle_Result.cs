using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App;
using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiAzienda;
using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiComuni;
using PianoColturaleEntity = AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiAzienda.DatiAziendaEntity.PianoColturaleEntity;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Sincronizzazione.AcquisizioneDatiTabelle;

/// <summary>
/// DS04-BL result: aggregated company data acquired from all BIZ services.
/// Each property is populated when the corresponding table was marked for synchronisation;
/// otherwise it retains its default empty value.
/// Ref: DS04-BL – Output.
/// </summary>
public sealed class AcquisizioneDatiTabelle_Result
{
    // ── Anagrafe ─────────────────────────────────────────────────────────────

    /// <summary>Ref: DS04-BL – Output: azienda.</summary>
    public ImpresaEntity? Azienda { get; set; }

    /// <summary>Ref: DS04-BL – Output: centriAziendali.</summary>
    public List<CentroAziendaleEntity> CentriAziendali { get; set; } = new();

    /// <summary>Ref: DS04-BL – Output: pianoColturale.</summary>
    public PianoColturaleEntity PianoColturale { get; set; } = new();

    /// <summary>Ref: DS04-BL – Output: campi.</summary>
    public List<CampoEntity> Campi { get; set; } = new();

    /// <summary>Ref: DS04-BL – Output: magazzini.</summary>
    public List<FabbricatoEntity> Magazzini { get; set; } = new();

    // ── Contatti ─────────────────────────────────────────────────────────────

    /// <summary>Ref: DS04-BL – Output: contatti.</summary>
    public List<ContattoEntity> Contatti { get; set; } = new();

    /// <summary>Ref: DS04-BL – Output: risorseUmane.</summary>
    public List<RisorseUmaneEntity> RisorseUmane { get; set; } = new();

    /// <summary>Ref: DS08-BL section 4.1.1 – Output: squadre.</summary>
    public List<SquadreEntity> Squadre { get; set; } = new();

    /// <summary>Ref: DS04-BL – Output: fornitori.</summary>
    public List<ContattoEntity> Fornitori { get; set; } = new();

    /// <summary>Ref: DS04-BL – Output: risorseFornitori.</summary>
    public List<RisorseUmaneEntity> RisorseFornitori { get; set; } = new();

    // ── Macchine / Prodotti ───────────────────────────────────────────────────

    /// <summary>Ref: DS04-BL – Output: parcoMacchine.</summary>
    public List<ParcoMacchineEntity> ParcoMacchine { get; set; } = new();

    /// <summary>
    /// Prodotti aziendali. Routing not yet defined in framework.
    /// Ref: DS04-BL – Output: prodotti.
    /// </summary>
    public List<ProdottoEntity> Prodotti { get; set; } = new();

    // ── Progetti / CDG ────────────────────────────────────────────────────────

    /// <summary>Ref: DS04-BL – Output: progetti.</summary>
    public List<ProgettoEntity> Progetti { get; set; } = new();

    /// <summary>Ref: DS04-BL – Output: centriAziendaliAttivitaCDG.</summary>
    public List<CentroAziendaleAttivitaCDGEntity> CentriAziendaliAttivitaCDG { get; set; } = new();

    /// <summary>Ref: DS04-BL – Output: impreseImpostazioni.</summary>
    public List<ImpostazioneEntity> ImpreseImpostazioni { get; set; } = new();

    /// <summary>Ref: DS04-BL – Output: attivitaCDG.</summary>
    public List<AttivitaCDGEntity> AttivitaCDG { get; set; } = new();

    /// <summary>Ref: DS04-BL – Output: lavorazioniAttivitaCDG.</summary>
    public List<LavorazioneAttivitaCDGEntity> LavorazioniAttivitaCDG { get; set; } = new();

    // ── Tipologie ─────────────────────────────────────────────────────────────

    /// <summary>Ref: DS04-BL – Output: areeTipologie.</summary>
    public List<AreaTipologieDocumentoEntity> AreaTipologie { get; set; } = new();

    /// <summary>Ref: DS04-BL – Output: tipologie.</summary>
    public List<TipologiaDocumentoEntity> Tipologie { get; set; } = new();

    // ── Codifiche / Misure ────────────────────────────────────────────────────

    /// <summary>Ref: DS04-BL – Output: codificaProdotti.</summary>
    public List<CodificaProdottoEntity> CodificaProdotti { get; set; } = new();

    /// <summary>Ref: DS04-BL – Output: misureAvversitaAnagrafiche.</summary>
    public List<MisuraAvversitaAnagraficheEntity> MisureAvversitaAnagrafiche { get; set; } = new();

    /// <summary>Ref: DS04-BL – Output: misureIndiciMaturitaAnagrafiche.</summary>
    public List<MisuraIndiciMaturitaAnagraficheEntity> MisureIndiciMaturitaAnagrafiche { get; set; } = new();

    /// <summary>
    /// Operation causals.
    /// NOTE: DS04-BL references a piva-based OperazioneCausale service
    /// (AgronicaNetCore.Operazione.BIZ.Services.OperazioneCausale.IOperazioneCausaleService)
    /// that has not yet been implemented. This collection is always empty pending that service.
    /// Ref: DS04-BL – Output: operazioneCausale.
    /// </summary>
    public List<OperazioneCausaleEntity> OperazioneCausale { get; set; } = new();
}
