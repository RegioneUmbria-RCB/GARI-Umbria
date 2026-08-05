namespace AgronicaNetCore.APP.BIZ.Services.ValidazioneParametriUtente;

/// <summary>
/// Well-known reason codes returned in <see cref="ValidazioneParametriUtenteDatiComuniResult.Motivo"/>.
/// Ref: DS07-BL – Output: motivo.
/// </summary>
internal static class MotivoValidazione
{
    internal const string PrimoAccesso = "primo_accesso";
    internal const string ParamRichiamoDiverso = "param_richiamo_diverso";
    internal const string ParamPermessiDiverso = "param_permessi_diverso";
    internal const string FiltriVisibilitaDiversi = "filtri_visibilita_diversi";
    internal const string ParametriCorrispondenti = "parametri_corrispondenti";
}
