namespace AgronicaNetCore.DSSNutrizione.DAL.DataLayer.Engine.ConsiglioNutrizione.Models
{

    /// <summary>
    /// Input per l'inserimento di una riga in Consigli_Nutrizione_Engine.
    /// Riferimento: DS04-BL Richiesta Consiglio Nutrizione Appezzamenti — Persistenze Coinvolte (Scrittura).
    /// </summary>
    public sealed class InsertConsiglioNutrizioneInput
    {
        public string  Piva                    { get; init; } = string.Empty;
        public int     SaCod                   { get; init; }
        public int     Appezza                 { get; init; }
        public int     IdReg                   { get; init; }
        public DateTime DataConsiglio          { get; init; }
        public DateTime DataSemina { get; init; }
        public string  Elemento                { get; init; } = string.Empty;
        public double? FabbisognoMinimo         { get; init; }
        public double? FabbisognoMassimo        { get; init; }
        public double? DoseConsigliataMiniima   { get; init; }
        public double? DoseConsigliataMassima   { get; init; }
        public double? QuantitativoPresente     { get; init; }
        public double? QuantitativoMinimoResiduo  { get; init; }
        public double? QuantitativoMassimoResiduo { get; init; }
        public string? Messaggi                { get; init; }
        public string  UsernameCreazione       { get; init; } = string.Empty;
    }

    /// <summary>
    /// Input per l'inserimento di una riga in Input_Consigli_Nutrizione_Engine.
    /// Riferimento: DS04-BL Richiesta Consiglio Nutrizione Appezzamenti — Persistenze Coinvolte (Scrittura).
    /// </summary>
    public sealed class InsertInputConsiglioNutrizioneInput
    {
        public int     IdConsiglio          { get; init; }
        public int?    IdAgenda             { get; init; }
        public int?    IdMov               { get; init; }
        public int?    IdMovDet            { get; init; }
        public string? AnalisiSuperUser    { get; init; }
        public int?    AnalisiTestataCod   { get; init; }
        public int?    AnalisiDettaglioCod { get; init; }
        public int?    AnalisiParametroCod { get; init; }
        public string  UsernameCreazione   { get; init; } = string.Empty;
    }
}
