namespace AgronicaNetCore.Anagrafe.BIZ.Services.AlberoGerarchiaImprese.Exceptions
{
    /// <summary>
    /// Eccezione lanciata quando l'<c>id_utente</c> fornito è nullo, vuoto
    /// o supera la lunghezza massima consentita di 256 caratteri.
    /// </summary>
    /// <remarks>
    /// Design Specification DS01-BL: RecuperoConoVisibilitaUtente - Eccezioni - InvalidUserException.
    /// </remarks>
    public sealed class InvalidUserException : Exception
    {
        /// <param name="idUtente">Il valore non valido ricevuto in input.</param>
        public InvalidUserException(string? idUtente)
            : base($"Il parametro 'id_utente' non è valido: '{idUtente}'. Deve essere non nullo, non vuoto e al massimo 256 caratteri.")
        {
            IdUtente = idUtente;
        }

        /// <summary>Il valore di <c>id_utente</c> che ha causato l'eccezione.</summary>
        public string? IdUtente { get; }
    }

    /// <summary>
    /// Eccezione lanciata quando lo <c>Username</c> fornito non è presente
    /// in alcuna tabella del database GIAS (<c>Utenti_Profili</c>).
    /// </summary>
    /// <remarks>
    /// Design Specification DS01-BL: RecuperoConoVisibilitaUtente - Eccezioni - UserNotFoundException.
    /// </remarks>
    public sealed class UserNotFoundException : Exception
    {
        /// <param name="idUtente">L'Username che non è stato trovato.</param>
        public UserNotFoundException(string idUtente)
            : base($"L'utente con id_utente='{idUtente}' non è stato trovato nel sistema.")
        {
            IdUtente = idUtente;
        }

        /// <summary>L'<c>id_utente</c> che non è stato trovato.</summary>
        public string IdUtente { get; }
    }

    /// <summary>
    /// Eccezione lanciata quando la query SQL sul database GIAS supera
    /// il timeout configurato (default: 500 ms).
    /// </summary>
    /// <remarks>
    /// Design Specification DS01-BL: RecuperoConoVisibilitaUtente - Eccezioni - QueryTimeoutException.
    /// </remarks>
    public sealed class QueryTimeoutException : Exception
    {
        /// <param name="timeoutMs">Il timeout in millisecondi che è stato superato.</param>
        public QueryTimeoutException(int timeoutMs)
            : base($"La query SQL ha superato il timeout di {timeoutMs} ms.")
        {
            TimeoutMs = timeoutMs;
        }

        /// <summary>Il timeout in millisecondi che è stato superato.</summary>
        public int TimeoutMs { get; }
    }

    /// <summary>
    /// Eccezione lanciata quando il mapping dei dati SQL alla struttura JSON fallisce,
    /// ad esempio per campi obbligatori assenti nel result set.
    /// </summary>
    /// <remarks>
    /// Design Specification DS01-BL: RecuperoConoVisibilitaUtente - Eccezioni - DataMappingException.
    /// </remarks>
    public sealed class DataMappingException : Exception
    {
        /// <param name="motivo">Descrizione del motivo del fallimento del mapping.</param>
        /// <param name="inner">Eccezione originale, se presente.</param>
        public DataMappingException(string motivo, Exception? inner = null)
            : base($"Errore durante il mapping dei dati SQL alla struttura JSON: {motivo}", inner)
        {
            Motivo = motivo;
        }

        /// <summary>Descrizione del motivo del fallimento del mapping.</summary>
        public string Motivo { get; }
    }

    /// <summary>
    /// Eccezione lanciata quando la gerarchia aziendale contiene cicli, riferimenti a parent
    /// inesistenti, o altri problemi di integrità strutturale.
    /// </summary>
    /// <remarks>
    /// Design Specification DS02-BL: RecuperoStrutturaFiliereTotale - Eccezioni - HierarchyValidationException.
    /// </remarks>
    public sealed class HierarchyValidationException : Exception
    {
        /// <param name="motivo">Descrizione del problema di integrità rilevato.</param>
        public HierarchyValidationException(string motivo)
            : base($"Validazione gerarchia aziendale fallita: {motivo}")
        {
            Motivo = motivo;
        }

        /// <summary>Descrizione del problema di integrità rilevato nella gerarchia.</summary>
        public string Motivo { get; }
    }

    /// <summary>
    /// Eccezione lanciata quando i dati recuperati dal database sono incoerenti
    /// (es. azienda referenziata che non esiste, dati di geolocalizzazione anomali).
    /// </summary>
    /// <remarks>
    /// Design Specification DS02-BL: RecuperoStrutturaFiliereTotale - Eccezioni - DataInconsistencyException.
    /// </remarks>
    public sealed class DataInconsistencyException : Exception
    {
        /// <param name="motivo">Descrizione dell'incoerenza rilevata.</param>
        /// <param name="inner">Eccezione originale, se presente.</param>
        public DataInconsistencyException(string motivo, Exception? inner = null)
            : base($"Incoerenza nei dati recuperati dal database: {motivo}", inner)
        {
            Motivo = motivo;
        }

        /// <summary>Descrizione dell'incoerenza rilevata.</summary>
        public string Motivo { get; }
    }

    /// <summary>
    /// Eccezione lanciata quando si verifica un errore nell'identificazione
    /// o nel mapping dell'admin di filiera.
    /// </summary>
    /// <remarks>
    /// Design Specification DS02-BL: RecuperoStrutturaFiliereTotale - Eccezioni - AdminAssignmentException.
    /// </remarks>
    public sealed class AdminAssignmentException : Exception
    {
        /// <param name="motivo">Descrizione del problema riscontrato nel mapping admin.</param>
        /// <param name="inner">Eccezione originale, se presente.</param>
        public AdminAssignmentException(string motivo, Exception? inner = null)
            : base($"Errore nell'identificazione dell'admin di filiera: {motivo}", inner)
        {
            Motivo = motivo;
        }

        /// <summary>Descrizione del problema riscontrato nel mapping admin.</summary>
        public string Motivo { get; }
    }
}
