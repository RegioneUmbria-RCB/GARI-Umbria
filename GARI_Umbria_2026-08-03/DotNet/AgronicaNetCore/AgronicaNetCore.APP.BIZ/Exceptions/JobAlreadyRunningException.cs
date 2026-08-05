namespace AgronicaNetCore.APP.BIZ.Exceptions;

/// <summary>
/// Thrown when a job cannot start because an active semaphore record
/// (flag_aggiornamento_in_corso = 1) already exists in <c>app_semaforo_daticomuni_web2app</c>.
/// Ref: DS06-BL – Eccezioni: JobAlreadyRunningException.
/// </summary>
public class JobAlreadyRunningException : Exception
{
    public JobAlreadyRunningException(string message) : base(message) { }
    public JobAlreadyRunningException(string message, Exception innerException) : base(message, innerException) { }
}
