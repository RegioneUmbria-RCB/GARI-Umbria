namespace AgronicaNetCore.APP.BIZ.Common;

/// <summary>
/// Centralized configuration for all APP.BIZ services.
/// Bind to appsettings via the existing section <c>"SincroWeb2App"</c>
/// (e.g. <c>ParametriAggiuntivi.settings.json</c>).
/// Values that are absent, null, or &lt;= 0 fall back to the coded default.
/// </summary>
public sealed class SincroWeb2AppSettings
{
    public const string SectionName = "SincroWeb2AppSettings";

    private const int DefaultTimeoutMs = 100;

    private int _timeoutMs = DefaultTimeoutMs;

    /// <summary>
    /// Universal slow-operation warning threshold in milliseconds applied to all
    /// timed operations in APP.BIZ (timestamp comparison, semaphore query, JSON
    /// aggregation, validation, etc.).
    /// Values that are absent, null, or &lt;= 0 fall back to the default of 100 ms.
    /// </summary>
    public int TimeoutMs
    {
        get => _timeoutMs;
        set => _timeoutMs = value > 0 ? value : DefaultTimeoutMs;
    }

    private const long DefaultMaxPayloadBytes = 500L * 1024 * 1024;
    private long _maxPayloadBytes = DefaultMaxPayloadBytes;

    /// <summary>
    /// Maximum allowed aggregated payload size in bytes before a warning is logged.
    /// Values that are absent, null, or &lt;= 0 fall back to the default of 500 MB.
    /// </summary>
    public long MaxPayloadBytes
    {
        get => _maxPayloadBytes;
        set => _maxPayloadBytes = value > 0 ? value : DefaultMaxPayloadBytes;
    }

    private const long DefaultStreamingThresholdBytes = 100L * 1024 * 1024;
    private long _streamingThresholdBytes = DefaultStreamingThresholdBytes;

    /// <summary>
    /// Payload size threshold in bytes above which a streaming-size warning is logged.
    /// Values that are absent, null, or &lt;= 0 fall back to the default of 100 MB.
    /// </summary>
    public long StreamingThresholdBytes
    {
        get => _streamingThresholdBytes;
        set => _streamingThresholdBytes = value > 0 ? value : DefaultStreamingThresholdBytes;
    }

    private const int DefaultMaxDeadlockRetry = 1;
    private int _maxDeadlockRetry = DefaultMaxDeadlockRetry;

    /// <summary>
    /// Maximum number of deadlock retry attempts before giving up.
    /// Values that are absent, null, or &lt; 0 fall back to the default of 1.
    /// </summary>
    public int MaxDeadlockRetry
    {
        get => _maxDeadlockRetry;
        set => _maxDeadlockRetry = value >= 0 ? value : DefaultMaxDeadlockRetry;
    }

    private const int DefaultDeadlockRetryDelaySeconds = 5;
    private int _deadlockRetryDelaySeconds = DefaultDeadlockRetryDelaySeconds;

    /// <summary>
    /// Delay in seconds between deadlock retry attempts.
    /// Values that are absent, null, or &lt;= 0 fall back to the default of 5 s.
    /// </summary>
    public int DeadlockRetryDelaySeconds
    {
        get => _deadlockRetryDelaySeconds;
        set => _deadlockRetryDelaySeconds = value > 0 ? value : DefaultDeadlockRetryDelaySeconds;
    }

    private const int DefaultTransactionTimeoutMs = 100000;
    private int _transactionTimeoutMs = DefaultTransactionTimeoutMs;

    /// <summary>
    /// Maximum duration in milliseconds for the SERIALIZABLE transaction before it is cancelled.
    /// Values that are absent, null, or &lt;= 0 fall back to the default of 100 000 ms (100 s).
    /// </summary>
    public int TransactionTimeoutMs
    {
        get => _transactionTimeoutMs;
        set => _transactionTimeoutMs = value > 0 ? value : DefaultTransactionTimeoutMs;
    }

    private const int DefaultLetturaTabellaMaxRetry = 2;
    private int _letturaTabellaMaxRetry = DefaultLetturaTabellaMaxRetry;

    /// <summary>
    /// Maximum number of retry attempts for reading a common table before giving up.
    /// Values that are absent, null, or &lt; 1 fall back to the default of 2.
    /// </summary>
    public int LetturaTabellaMaxRetry
    {
        get => _letturaTabellaMaxRetry;
        set => _letturaTabellaMaxRetry = value >= 1 ? value : DefaultLetturaTabellaMaxRetry;
    }

    private const int DefaultLetturaTabellaRetryDelaySeconds = 10;
    private int _letturaTabellaRetryDelaySeconds = DefaultLetturaTabellaRetryDelaySeconds;

    /// <summary>
    /// Base delay in seconds between read retry attempts (multiplied by attempt number).
    /// Values that are absent, null, or &lt;= 0 fall back to the default of 10 s.
    /// </summary>
    public int LetturaTabellaRetryDelaySeconds
    {
        get => _letturaTabellaRetryDelaySeconds;
        set => _letturaTabellaRetryDelaySeconds = value > 0 ? value : DefaultLetturaTabellaRetryDelaySeconds;
    }

    private const int DefaultClockSkewToleranceMinutes = 1;
    private int _clockSkewToleranceMinutes = DefaultClockSkewToleranceMinutes;

    /// <summary>
    /// Tolerance window in minutes for client clock skew / manipulation detection.
    /// Values that are absent, null, or &lt;= 0 fall back to the default of 1 minute.
    /// </summary>
    public int ClockSkewToleranceMinutes
    {
        get => _clockSkewToleranceMinutes;
        set => _clockSkewToleranceMinutes = value > 0 ? value : DefaultClockSkewToleranceMinutes;
    }

    private const int DefaultParametroUtenteWarningLengthChars = 10_000;
    private int _parametroUtenteWarningLengthChars = DefaultParametroUtenteWarningLengthChars;

    /// <summary>
    /// Parameter string length above which a warning is logged during user parameter update.
    /// Values that are absent, null, or &lt;= 0 fall back to the default of 10 000 chars.
    /// </summary>
    public int ParametroUtenteWarningLengthChars
    {
        get => _parametroUtenteWarningLengthChars;
        set => _parametroUtenteWarningLengthChars = value > 0 ? value : DefaultParametroUtenteWarningLengthChars;
    }

    private const long DefaultSerializzazioneWarningThresholdBytes = 200L * 1024 * 1024;
    private long _serializzazioneWarningThresholdBytes = DefaultSerializzazioneWarningThresholdBytes;

    /// <summary>
    /// JSON serialisation output size in bytes above which a warning is logged.
    /// Values that are absent, null, or &lt;= 0 fall back to the default of 200 MB.
    /// </summary>
    public long SerializzazioneWarningThresholdBytes
    {
        get => _serializzazioneWarningThresholdBytes;
        set => _serializzazioneWarningThresholdBytes = value > 0 ? value : DefaultSerializzazioneWarningThresholdBytes;
    }

    private const int DefaultSerializzazioneStreamingThreshold = 10_000;
    private int _serializzazioneStreamingThreshold = DefaultSerializzazioneStreamingThreshold;

    /// <summary>
    /// Entity collection size (record count) above which streaming JSON serialisation is used.
    /// Values that are absent, null, or &lt;= 0 fall back to the default of 10 000 records.
    /// </summary>
    public int SerializzazioneStreamingThreshold
    {
        get => _serializzazioneStreamingThreshold;
        set => _serializzazioneStreamingThreshold = value > 0 ? value : DefaultSerializzazioneStreamingThreshold;
    }
}
