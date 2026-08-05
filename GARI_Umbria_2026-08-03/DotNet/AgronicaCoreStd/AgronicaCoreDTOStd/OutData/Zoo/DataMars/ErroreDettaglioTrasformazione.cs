namespace OutData.Zoo.DataMars
{
    /// <summary>
    /// Singolo errore durante la trasformazione di una pesata dallo staging all'agenda.
    /// <para>Riferimento spec: DS02-BL TrasformazionePesateStagingAgenda â€” Output (Details.ErroriDettagli).</para>
    /// </summary>
    public sealed class ErroreDettaglioTrasformazione
    {
        /// <summary>ID della sessione staging da cui proviene la pesata.</summary>
        public string IdSessione { get; set; } = string.Empty;

        /// <summary>LID (Matricola) dell'animale che ha generato l'errore.</summary>
        public string Lid { get; set; } = string.Empty;

        /// <summary>Tipo di eccezione (es. "AnimalNotFoundException").</summary>
        public string TipoErrore { get; set; } = string.Empty;

        /// <summary>Messaggio descrittivo dell'errore.</summary>
        public string Messaggio { get; set; } = string.Empty;
    }
}
