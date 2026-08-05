namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Services.ColtureAziende
{
    /// <summary>
    /// Single crop species entry in the colture dropdown list.
    /// </summary>
    /// <remarks>DS-16 — response model for <c>GET v1/sostenibilita/colture-aziende</c>.</remarks>
    public class ColturaDropdownItem
    {
        /// <summary>Codice specie vegetale (Veg_Cod).</summary>
        public int VegCod { get; set; }

        /// <summary>Descrizione specie vegetale (Veg_Des).</summary>
        public string VegDes { get; set; } = string.Empty;
    }

    /// <summary>
    /// Response payload for the colture dropdown endpoint.
    /// </summary>
    /// <remarks>DS-16 — response model for <c>GET v1/sostenibilita/colture-aziende</c>.</remarks>
    public class ColtureAziendeDropdownResult
    {
        /// <summary>Lista di colture disponibili per le filiere selezionate.</summary>
        public List<ColturaDropdownItem> Colture { get; set; } = new();
    }
}
