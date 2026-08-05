namespace AgronicaCoreDTOStd.InData.Gis
{
    /// <summary>
    /// Intervallo di date in cui effettuare la ricerca delle posizioni rilevate
    /// </summary>
    public class LettureTecniciInCampo_In
    {

        /// <summary>
        /// Data Inizio
        /// </summary>
        /// <example>2023-01-02T23:00:00.000Z</example>
        public string DataInizio { get; set; }

        /// <summary>
        /// Data Fine
        /// </summary>
        /// <example>2023-01-02T23:00:00.000Z</example>
        public string DataFine { get; set; }

        /// <summary>
        /// Flag utente corrente
        /// </summary>
        /// <example>1</example>
        public bool Utente_corrente { get; set; }

    }
}
