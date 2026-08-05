namespace AgronicaCoreDTOStd.OutData.Pratiche
{
    /// <summary>
    /// Messaggio di avviso non-bloccante restituito nelle response di modifica profilo pratiche.
    /// </summary>
    public class MessaggioAvviso_OUT
    {
        /// <summary>Tipo di avviso (es. 'AND_ZERO_PRATICHE', 'VALIDATION_WARNING').</summary>
        public string Tipo { get; set; } = string.Empty;

        /// <summary>Descrizione leggibile dell'avviso.</summary>
        public string Messaggio { get; set; } = string.Empty;
    }
}
