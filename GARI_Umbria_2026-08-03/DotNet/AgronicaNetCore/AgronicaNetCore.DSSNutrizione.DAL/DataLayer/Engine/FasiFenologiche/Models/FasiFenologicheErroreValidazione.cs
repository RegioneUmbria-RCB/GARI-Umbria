namespace AgronicaNetCore.DSSNutrizione.DAL.DataLayer.Engine.FasiFenologiche.Models
{
    /// <summary>
    /// Errore di validazione associato a un campo della richiesta fasi fenologiche.
    /// </summary>
    /// <remarks>
    /// Design Specification: ValidazioneParametriRichiestaFasiFenologiche - Output.
    /// </remarks>
    public sealed class FasiFenologicheErroreValidazione
    {
        public string Campo { get; init; } = string.Empty;
        public string Messaggio { get; init; } = string.Empty;
    }
}
