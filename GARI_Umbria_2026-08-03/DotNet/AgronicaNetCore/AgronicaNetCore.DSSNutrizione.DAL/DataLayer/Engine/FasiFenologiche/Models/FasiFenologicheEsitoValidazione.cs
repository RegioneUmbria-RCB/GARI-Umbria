using System.Collections.Generic;

namespace AgronicaNetCore.DSSNutrizione.DAL.DataLayer.Engine.FasiFenologiche.Models
{
    /// <summary>
    /// Esito della validazione dei parametri della richiesta fasi fenologiche.
    /// </summary>
    /// <remarks>
    /// Design Specification: ValidazioneParametriRichiestaFasiFenologiche - Output.
    /// </remarks>
    public sealed class FasiFenologicheEsitoValidazione
    {
        public bool ValidazioneEsito { get; init; }
        public IReadOnlyList<FasiFenologicheErroreValidazione> ErroriValidazione { get; init; } = new List<FasiFenologicheErroreValidazione>();
    }
}
