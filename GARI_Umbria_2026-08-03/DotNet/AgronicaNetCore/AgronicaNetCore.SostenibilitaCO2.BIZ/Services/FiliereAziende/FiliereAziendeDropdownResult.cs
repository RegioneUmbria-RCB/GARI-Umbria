namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Services.FiliereAziende
{
    /// <summary>
    /// Single item in the filiere dropdown list.
    /// </summary>
    /// <remarks>DS-15 — response model for <c>GET v1/sostenibilita/filiere-aziende</c>.</remarks>
    public class FilieraDropdownItem
    {
        /// <summary>P.IVA identificativo del nodo.</summary>
        public string Piva { get; set; } = string.Empty;

        /// <summary>Ragione sociale del nodo.</summary>
        public string RagioneSociale { get; set; } = string.Empty;

        /// <summary>
        /// P.IVA del nodo padre diretto.
        /// Vuota per i nodi di livello 2 (radice della filiera).
        /// </summary>
        public string Padre { get; set; } = string.Empty;
    }

    /// <summary>
    /// Response payload for the filiere dropdown endpoint.
    /// </summary>
    /// <remarks>DS-15 — response model for <c>GET v1/sostenibilita/filiere-aziende</c>.</remarks>
    public class FiliereAziendeDropdownResult
    {
        /// <summary>Lista di filiere visibili all'utente.</summary>
        public List<FilieraDropdownItem> Filiere { get; set; } = new();
    }
}
