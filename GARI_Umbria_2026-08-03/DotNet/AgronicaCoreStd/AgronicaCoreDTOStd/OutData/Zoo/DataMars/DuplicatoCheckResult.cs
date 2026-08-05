namespace OutData.Zoo.DataMars
{
    /// <summary>
    /// Risultato del controllo duplicati operazioni agenda.
    /// <para>Riferimento spec: DS06-BL PrevenzioneDuplicatiOperazioniAgenda â€” Output.</para>
    /// </summary>
    public sealed class DuplicatoCheckResult
    {
        /// <summary>True se esiste giÃ  almeno un'operazione di pesatura identica.</summary>
        public bool IsDuplicate { get; set; }

        /// <summary>Numero totale di operazioni duplicate trovate.</summary>
        public int NumDuplicatiFound { get; set; }
    }
}
