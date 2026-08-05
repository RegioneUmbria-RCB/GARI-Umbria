using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiAzienda;
using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.UtentiImpostazioni;
using AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Sincronizzazione.DecisioneModalitaSincronizzazione;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Sincronizzazione.AcquisizioneDatiTabelle;

/// <summary>
/// Input for DS04-BL: the company identifier, the per-table sync decisions produced by DS03,
/// and the calling context parameters required by the downstream BIZ services.
/// Ref: DS04-BL – Input.
/// </summary>
public sealed class AcquisizioneDatiTabelle_Input
{
    /// <summary>
    /// Per-table sync decisions produced by DS03.
    /// Only entries with <see cref="DecisioneTabellaSincronizzazioneResult.Sincronizzare"/> = true
    /// will trigger a BIZ service call.
    /// Ref: DS04-BL – Input: tabelle_per_sincronizzazione.
    /// </summary>
    public IReadOnlyDictionary<string, DecisioneTabellaSincronizzazioneResult> TabellePerSincronizzazione { get; }

    /// <summary>Server connection and tenant parameters required by all downstream services.</summary>
    public AgronicaCoreParametriServer ObjParametriServer { get; }

    /// <summary>User context parameters required by services such as PianoColturale.</summary>
    public AgronicaCoreParametriUtenti ObjParametriUtenti { get; }

    /// <summary>Super-server parameters required by services such as PianoColturale.</summary>
    public AgronicaCoreParametriSuperServer ObjParametriSuperServer { get; }

    public PermessiUtenteSincronizzazioneEntity PermessiUtente { get; }
    public DatiAziendaFromFlutterRequest Request { get; }
    public bool ModalitaDemetra { get; }

    public AcquisizioneDatiTabelle_Input(
        IReadOnlyDictionary<string, DecisioneTabellaSincronizzazioneResult> tabellePerSincronizzazione,
        PermessiUtenteSincronizzazioneEntity permessiUtente,
        DatiAziendaFromFlutterRequest request,
        AgronicaCoreParametriServer objParametriServer,
        AgronicaCoreParametriUtenti objParametriUtenti,
        AgronicaCoreParametriSuperServer objParametriSuperServer,
        bool modalitaDemetra)
    {
        PermessiUtente = permessiUtente;
        Request = request;
        TabellePerSincronizzazione = tabellePerSincronizzazione;
        ObjParametriServer = objParametriServer;
        ObjParametriUtenti = objParametriUtenti;
        ObjParametriSuperServer = objParametriSuperServer;
        ModalitaDemetra = modalitaDemetra;
    }
}
