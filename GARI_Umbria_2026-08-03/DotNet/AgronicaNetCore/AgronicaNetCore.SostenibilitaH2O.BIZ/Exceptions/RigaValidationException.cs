namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Exceptions
{
    /// <summary>
    /// Eccezione interna non-bloccante: segnala che una singola riga del batch non è conforme
    /// allo schema atteso. La riga viene saltata e l'elaborazione del batch prosegue.
    /// Riferimento spec: DS08-BL PersistenzaRisultatiPerColture — ValidationException.
    /// </summary>
    public class RigaValidationException : Exception
    {
        /// <summary>Indice della riga nel batch che ha causato la violazione.</summary>
        public int RigaIndex { get; }

        /// <summary>Campo o gruppo di campi che ha causato la violazione.</summary>
        public string Campo { get; }

        /// <param name="rigaIndex">Indice della riga non valida.</param>
        /// <param name="campo">Nome del campo o della chiave candidata non valida.</param>
        /// <param name="message">Descrizione della violazione.</param>
        public RigaValidationException(int rigaIndex, string campo, string message)
            : base(message)
        {
            RigaIndex = rigaIndex;
            Campo = campo;
        }
    }
}
