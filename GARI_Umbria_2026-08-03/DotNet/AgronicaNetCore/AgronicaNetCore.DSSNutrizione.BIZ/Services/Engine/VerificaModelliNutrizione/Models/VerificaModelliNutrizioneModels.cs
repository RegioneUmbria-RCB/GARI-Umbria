namespace AgronicaNetCore.DSSNutrizione.BIZ.Services.Engine.VerificaModelliNutrizione.Models
{
    /// <summary>
    /// Coppia (specie vegetale, varietà) per la verifica della disponibilità dei modelli nutrizionali.
    /// Riferimento: DS03-BL Verifica Disponibilità Modelli Nutrizione.
    /// </summary>
    public sealed class CoppiaSpecieVarieta
    {
        public int     Veg_Cod     { get; init; }
        public int?    Cul_Cod { get; init; }
    }

    /// <summary>
    /// Singolo modello di calcolo nutrizionale disponibile per una coppia (specie, varietà).
    /// Riferimento: DS13-API POST /v1/dss/nutrizione/modelli/verifica — Response 200.modelli.
    /// </summary>
    public sealed class ModelloNutrizioneDto
    {
        public string Id       { get; init; } = string.Empty;
        public string DetailId { get; init; } = string.Empty;
        public string Code     { get; init; } = string.Empty;
    }

    /// <summary>
    /// Stato della disponibilità dei modelli per una coppia (specie, varietà).
    /// Riferimento: DS13-API POST /v1/dss/nutrizione/modelli/verifica — Response 200.status.
    /// </summary>
    public enum StatoModelloNutrizione
    {
        /// <summary>Almeno un modello disponibile per la coppia.</summary>
        Available,
        /// <summary>Nessun modello disponibile per la coppia.</summary>
        NotAvailable,
        /// <summary>L'engine non ha risposto per questa coppia (fallimento isolato).</summary>
        Error
    }

    /// <summary>
    /// Risultato per una singola coppia (specie vegetale, varietà) con i modelli disponibili.
    /// Riferimento: DS03-BL Verifica Disponibilità Modelli Nutrizione — Output.modelli_per_specie_varieta.
    /// DS13-API POST /v1/dss/nutrizione/modelli/verifica — Response 200.modelli_per_specie_varieta.
    /// </summary>
    public sealed class ModelliPerSpecieVarietaDto
    {
        public int     Veg_Cod     { get; init; }
        public int?    Cul_Cod { get; init; }
        public int     NumModelli { get; init; }
        public IReadOnlyList<ModelloNutrizioneDto> Modelli { get; init; } = Array.Empty<ModelloNutrizioneDto>();
        public StatoModelloNutrizione Status { get; init; }
    }

    /// <summary>
    /// Risultato aggregato della verifica disponibilità modelli di calcolo nutrizionale.
    /// Riferimento: DS13-API POST /v1/dss/nutrizione/modelli/verifica — Response 200.
    /// DS03-BL Verifica Disponibilità Modelli Nutrizione — Output.
    /// </summary>
    public sealed class VerificaModelliNutrizioneResult
    {
        public IReadOnlyList<ModelliPerSpecieVarietaDto> ModelliPerSpecieVarieta { get; init; }
            = Array.Empty<ModelliPerSpecieVarietaDto>();

        /// <summary>Timestamp dell'esecuzione della verifica in formato ISO 8601.</summary>
        public string TimestampVerifica { get; init; } = string.Empty;
    }
}
