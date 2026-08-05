namespace AgronicaNetCore.OperazioniZoo.BIZ.Services.PesateAutomaticheDataMars.Exceptions;

/// <summary>
/// Lanciata quando tutti i tentativi di retry verso l'API Datamars sono esauriti.
/// <para>Riferimento spec: DS10-BL GestioneRetryEsponentialeApiDatamars — Eccezioni.</para>
/// </summary>
public sealed class ApiRetryExhaustedException : Exception
{
    public int Tentativi { get; }
    public int? UltimoStatusHttp { get; }

    public ApiRetryExhaustedException(int tentativi, int? ultimoStatusHttp, string? endpoint, Exception? innerException = null)
        : base($"API Datamars non raggiungibile dopo {tentativi} tentativi. Endpoint: '{endpoint}'. Ultimo HTTP status: {ultimoStatusHttp?.ToString() ?? "N/A"}.", innerException)
    {
        Tentativi = tentativi;
        UltimoStatusHttp = ultimoStatusHttp;
    }
}

/// <summary>
/// Lanciata quando il payload JSON ricevuto dall'API Datamars non è valido o non è parsabile.
/// <para>Riferimento spec: DS09-BL ValidazionePayloadJsonDatamars — Eccezioni (JsonSyntaxException);
/// DS01-BL AcquisizionePesateDatamarsAPI — Eccezioni (JsonParsingException).</para>
/// </summary>
public sealed class JsonParsingException : Exception
{
    public JsonParsingException(string idSessione, string message, Exception? innerException = null)
        : base($"Payload JSON non valido per la sessione '{idSessione}': {message}", innerException)
    {
    }
}

/// <summary>
/// Lanciata (e catturata per idempotenza) quando un id_sessione è già presente in STAGING_PESATE
/// con flag_validazione diverso da ERRORE_TOTALE.
/// <para>Riferimento spec: DS01-BL AcquisizionePesateDatamarsAPI — Regole di Business (Deduplicazione);
/// Eccezioni (SessionAlreadyProcessedException).</para>
/// </summary>
public sealed class SessionAlreadyProcessedException : Exception
{
    public string IdSessione { get; }

    public SessionAlreadyProcessedException(string idSessione)
        : base($"La sessione '{idSessione}' è già presente in STAGING_PESATE con stato valido. Skip idempotente.")
    {
        IdSessione = idSessione;
    }
}

/// <summary>
/// Lanciata quando il peso di una pesata è fuori dal range valido (≤ 0).
/// Non blocca la sessione: la pesata viene scartata e conteggiata in record_errori.
/// <para>Riferimento spec: DS01-BL AcquisizionePesateDatamarsAPI — Eccezioni (WeightValidationException);
/// DS09-BL — Eccezioni (InvalidWeightException).</para>
/// </summary>
public sealed class InvalidWeightException : Exception
{
    public decimal PesoRicevuto { get; }

    public InvalidWeightException(string idSessione, int pesataIndex, decimal pesoRicevuto)
        : base($"Sessione '{idSessione}', pesata[{pesataIndex}]: peso {pesoRicevuto} non valido (deve essere > 0).")
    {
        PesoRicevuto = pesoRicevuto;
    }
}

/// <summary>
/// Lanciata quando il LID (Lifetime Identifier) di una pesata è null o mancante.
/// La singola pesata viene scartata ma la sessione continua.
/// <para>Riferimento spec: DS01-BL AcquisizionePesateDatamarsAPI — Eccezioni (LIDMissingException).</para>
/// </summary>
public sealed class LIDMissingException : Exception
{
    public LIDMissingException(string idSessione, int pesataIndex)
        : base($"Sessione '{idSessione}', pesata[{pesataIndex}]: LID (lifetimeIdentifierTag) obbligatorio mancante.")
    {
    }
}

/// <summary>
/// Lanciata quando il timestamp di una pesata non è ISO 8601 valido o è nel futuro.
/// La singola pesata viene flaggata come non valida.
/// <para>Riferimento spec: DS09-BL ValidazionePayloadJsonDatamars — Eccezioni (InvalidTimestampException).</para>
/// </summary>
public sealed class InvalidTimestampException : Exception
{
    public InvalidTimestampException(string idSessione, int pesataIndex, string timestampRicevuto)
        : base($"Sessione '{idSessione}', pesata[{pesataIndex}]: timestamp '{timestampRicevuto}' non è ISO 8601 valido o è nel futuro.")
    {
    }
}

/// <summary>
/// Lanciata quando manca un campo obbligatorio nel payload (es. sessionId, array pesate).
/// <para>Riferimento spec: DS09-BL ValidazionePayloadJsonDatamars — Eccezioni (MissingMandatoryFieldException).</para>
/// </summary>
public sealed class MissingMandatoryFieldException : Exception
{
    public string FieldName { get; }

    public MissingMandatoryFieldException(string fieldName)
        : base($"Campo obbligatorio mancante nel payload Datamars: '{fieldName}'.")
    {
        FieldName = fieldName;
    }
}

/// <summary>
/// Lanciata quando si verifica un errore durante la transazione di persistenza su database.
/// Indica che è stato eseguito il rollback completo della sessione.
/// <para>Riferimento spec: DS01-BL AcquisizionePesateDatamarsAPI — Eccezioni (DatabaseTransactionException).</para>
/// </summary>
public sealed class DatabaseTransactionException : Exception
{
    public string IdSessione { get; }

    public DatabaseTransactionException(string idSessione, Exception innerException)
        : base($"Errore transazione DB per la sessione '{idSessione}'. Rollback eseguito.", innerException)
    {
        IdSessione = idSessione;
    }
}

// ── DS02 / DS05 / DS06 Exceptions ────────────────────────────────────────────

/// <summary>
/// Lanciata quando il LID (Matricola) di una pesata non esiste in ZOO_ANIMALI.
/// La pesata non viene importata; la sessione di trasformazione continua con la pesata successiva.
/// <para>Riferimento spec: DS02-BL TrasformazionePesateStagingAgenda — Eccezioni (AnimalNotFoundException);
/// DS05-BL ValidazioneAnagraficaAnimaliMappingStalla — Eccezioni (AnimalNotFound).</para>
/// </summary>
public sealed class AnimalNotFoundException : Exception
{
    public string Lid { get; }

    public AnimalNotFoundException(string lid)
        : base($"Animale con LID='{lid}' non trovato in ZOO_ANIMALI.")
    {
        Lid = lid;
    }
}

/// <summary>
/// Lanciata quando la data della pesata cade fuori dalla finestra di validità dell'animale
/// (Validita_Fine &lt; dataPesata o dataPesata &lt; Validita_Inizio).
/// La pesata non viene importata; la sessione di trasformazione continua.
/// <para>Riferimento spec: DS02-BL — Eccezioni (AnimalDisattivException);
/// DS05-BL — Eccezioni (AnimalInactiveException).</para>
/// </summary>
public sealed class AnimalInactiveException : Exception
{
    public string Lid { get; }
    public DateTime PesataDate { get; }
    public DateTime ValiditaFine { get; }

    public AnimalInactiveException(string lid, DateTime pesataDate, DateTime validitaFine)
        : base($"Animale LID='{lid}' risulta inattivo alla data di pesata {pesataDate:O}. ValiditaFine={validitaFine:O}.")
    {
        Lid = lid;
        PesataDate = pesataDate;
        ValiditaFine = validitaFine;
    }
}

/// <summary>
/// Lanciata quando il FarmID della sessione non è mappato a nessuna stalla GIAS.
/// L'errore è bloccante per l'intera sessione di trasformazione (non solo per la singola pesata).
/// <para>Riferimento spec: DS02-BL — Eccezioni (StallaMappingException);
/// DS05-BL — Eccezioni (StallaMappingNotFoundException).</para>
/// </summary>
public sealed class StallaMappingException : Exception
{
    public string FarmId { get; }

    public StallaMappingException(string farmId)
        : base($"FarmID='{farmId}' non è mappato a nessuna stalla GIAS in Fabbricati_codici (id_cod=1366).")
    {
        FarmId = farmId;
    }
}

/// <summary>
/// Lanciata quando esiste già un'operazione di pesatura per lo stesso animale e la stessa data.
/// La pesata duplicata non viene importata; la sessione di trasformazione continua.
/// <para>Riferimento spec: DS02-BL — Eccezioni (DuplicateWeightException);
/// DS06-BL PrevenzioneDuplicatiOperazioniAgenda — Regole di Business.</para>
/// </summary>
public sealed class DuplicateWeightException : Exception
{
    public int CodProgetto { get; }
    public DateTime DataPesata { get; }

    public DuplicateWeightException(int codProgetto, DateTime dataPesata)
        : base($"Operazione di pesatura già esistente per Cod_Progetto={codProgetto} in data {dataPesata:yyyy-MM-dd}.")
    {
        CodProgetto = codProgetto;
        DataPesata = dataPesata;
    }
}
