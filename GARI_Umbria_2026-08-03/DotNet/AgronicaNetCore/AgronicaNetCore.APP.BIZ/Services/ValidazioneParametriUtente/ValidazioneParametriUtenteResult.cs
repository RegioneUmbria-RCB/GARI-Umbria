using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.UtentiImpostazioni;

namespace AgronicaNetCore.APP.BIZ.Services.ValidazioneParametriUtente;

/// <summary>
/// Result returned by <see cref="IValidazioneParametriUtenteService.ValidaParametriDatiComuniAsync"/>.
/// Ref: DS07-BL – Output.
/// </summary>
public sealed class ValidazioneParametriUtenteDatiComuniResult
{
    /// <summary>
    /// True when parameters or filters differ from stored values, or on first access.
    /// False when all comparisons match.
    /// Ref: DS07-BL – Output: richiedi_dataset_completo.
    /// </summary>
    public bool RichiediDatasetCompleto { get; }

    /// <summary>
    /// Reason code. One of the values defined in <see cref="MotivoValidazione"/>.
    /// Ref: DS07-BL – Output: motivo.
    /// </summary>
    public string Motivo { get; }

    /// <summary>
    /// Normalized JSON of the current user permissions. Always present.
    /// Ref: DS07-BL – Output: param_permessi_estratti.
    /// </summary>
    public string ParamPermessiEstrattiJson { get; }

    public PermessiUtenteSincronizzazioneEntity ParamPermessiEstratti { get; }
    public ParamVisibilitaUtente ParamVisibilitaEstratti { get; }

    /// <summary>
    /// Normalized JSON of the current user visibility filters. Always present.
    /// Ref: DS07-BL – Output: param_visibilita_estratti.
    /// </summary>
    public string ParamVisibilitaEstrattiJson { get; }

    /// <summary>
    /// Milliseconds spent in the validation process.
    /// Ref: DS07-BL – Output: validation_time_ms.
    /// </summary>
    public long ValidationTimeMs { get; }

    public ValidazioneParametriUtenteDatiComuniResult(
        bool richiediDatasetCompleto,
        string motivo,
        string paramPermessiEstrattiJson,
        string paramVisibilitaEstrattiJson,
        PermessiUtenteSincronizzazioneEntity paramPermessiEstratti,
        ParamVisibilitaUtente paramVisibilitaEstratti,
        long validationTimeMs
    )
    {
        RichiediDatasetCompleto = richiediDatasetCompleto;
        Motivo = motivo;
        ParamPermessiEstrattiJson = paramPermessiEstrattiJson;
        ParamVisibilitaEstrattiJson = paramVisibilitaEstrattiJson;
        ParamPermessiEstratti = paramPermessiEstratti;
        ParamVisibilitaEstratti = paramVisibilitaEstratti;
        ValidationTimeMs = validationTimeMs;
    }

    /// <summary>
    /// Result returned by <see cref="IValidazioneParametriUtenteService.ValidaParametriDatiAziendaAsync"/>.
    /// Ref: DS01-BL – Output.
    /// </summary>
    public sealed class ValidazioneParametriUtenteDatiAziendaResult
    {
        /// <summary>
        /// True when parameters or filters differ from stored values, or on first access.
        /// False when all comparisons match.
        /// Ref: DS01-BL – Output: richiedi_dataset_completo.
        /// </summary>
        public bool RichiediDatasetCompleto { get; }

        /// <summary>
        /// Reason code. One of the values defined in <see cref="MotivoValidazione"/>.
        /// Ref: DS01-BL – Output: motivo.
        /// </summary>
        public string Motivo { get; }

        /// <summary>
        /// Normalized JSON of the current user permissions. Always present.
        /// Ref: DS01-BL – Output: param_permessi_estratti.
        /// </summary>
        public string ParamPermessiEstrattiJson { get; }

        public PermessiUtenteSincronizzazioneEntity ParamPermessiEstratti { get; }
        public ParamVisibilitaUtente ParamVisibilitaEstratti { get; }

        public ParamVisibilitaAziendeUtente ParamVisibilitaAziendeEstratti { get; }

        /// <summary>
        /// Normalized JSON of the current user visibility filters. Always present.
        /// Ref: DS01-BL – Output: param_visibilita_estratti.
        /// </summary>
        public string ParamVisibilitaEstrattiJson { get; }

        /// <summary>
        /// Normalized JSON of the current company visibility filters. Always present.
        /// Ref: DS01-BL – Output: param_visibilita_aziente_estratti.
        /// </summary>
        public string ParamVisibilitaAziendeEstrattiJson { get; }

        /// <summary>
        /// Milliseconds spent in the validation process.
        /// Ref: DS01-BL – Output: validation_time_ms.
        /// </summary>
        public long ValidationTimeMs { get; }

        public ValidazioneParametriUtenteDatiAziendaResult(
            bool richiediDatasetCompleto,
            string motivo,
            string paramPermessiEstrattiJson,
            string paramVisibilitaEstrattiJson,
            string paramVisibilitaAziendeUtenteEstrattiJson,
            PermessiUtenteSincronizzazioneEntity paramPermessiEstratti,
            ParamVisibilitaUtente paramVisibilitaEstratti,
            ParamVisibilitaAziendeUtente paramVisibilitaAziendeUtenteEstratti,
            long validationTimeMs
        )
        {
            RichiediDatasetCompleto = richiediDatasetCompleto;
            Motivo = motivo;
            ParamPermessiEstrattiJson = paramPermessiEstrattiJson;
            ParamVisibilitaEstrattiJson = paramVisibilitaEstrattiJson;
            ParamVisibilitaAziendeEstrattiJson = paramVisibilitaAziendeUtenteEstrattiJson;
            ParamPermessiEstratti = paramPermessiEstratti;
            ParamVisibilitaEstratti = paramVisibilitaEstratti;
            ParamVisibilitaAziendeEstratti = paramVisibilitaAziendeUtenteEstratti;
            ValidationTimeMs = validationTimeMs;
        }
    }
}
