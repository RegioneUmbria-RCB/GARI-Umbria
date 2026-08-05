namespace AgronicaNetCore.APP.BIZ.Services.DatiComuni.PreparazioneDatiJob.AggregazioneDatiComuni;

/// <summary>
/// Result returned by <see cref="IAggregazioneDatiComuniService.EseguiAggregazioneAsync"/>.
/// Ref: DS14-API – Risposte: 200 e 206.
/// </summary>
public sealed class AggregazioneDatiComuniResult
{
    /// <summary><c>true</c> if all tables were processed and persisted successfully.</summary>
    public bool IsSuccess { get; }

    /// <summary>Human-readable summary suitable for <c>rispostaStringa</c> or <c>errore</c> fields.</summary>
    public string Message { get; }

    /// <summary>Total number of tables read during this cycle.</summary>
    public int TabelleElaborate { get; }

    /// <summary>Number of tables whose JSON changed and were written to the preparation table.</summary>
    public int TabelleConVariazioni { get; }

    /// <summary>Name of the table that caused the failure, or <c>null</c> on success.</summary>
    public string? FailedTable { get; }

    /// <summary>Raw exception message on failure, or <c>null</c> on success.</summary>
    public string? ErrorMessage { get; }

    /// <summary>Sum of JSON byte sizes for all successfully serialised tables.</summary>
    public long DimensioneTotaleByte { get; }

    private AggregazioneDatiComuniResult(
        bool isSuccess,
        string message,
        int tabelleElaborate,
        int tabelleConVariazioni,
        long dimensioneTotaleByte,
        string? failedTable,
        string? errorMessage)
    {
        IsSuccess = isSuccess;
        Message = message;
        TabelleElaborate = tabelleElaborate;
        TabelleConVariazioni = tabelleConVariazioni;
        DimensioneTotaleByte = dimensioneTotaleByte;
        FailedTable = failedTable;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static AggregazioneDatiComuniResult Success(
        int tabelleElaborate,
        int tabelleConVariazioni,
        long dimensioneTotaleByte)
    {
        int senzaVariazioni = tabelleElaborate - tabelleConVariazioni;
        string message =
            $"{tabelleElaborate} tabelle elaborate, {tabelleConVariazioni} con variazioni, {senzaVariazioni} senza variazioni";

        return new AggregazioneDatiComuniResult(
            isSuccess: true,
            message: message,
            tabelleElaborate: tabelleElaborate,
            tabelleConVariazioni: tabelleConVariazioni,
            dimensioneTotaleByte: dimensioneTotaleByte,
            failedTable: null,
            errorMessage: null);
    }

    /// <summary>
    /// Creates a failure result with rollback information.
    /// </summary>
    public static AggregazioneDatiComuniResult Failure(
        int tabelleElaborate,
        string failedTable,
        string errorMessage)
    {
        string message =
            $"Aggregazione fallita: lettura {failedTable} non riuscita dopo 2 tentativi. Nessun dato è stato persistito (rollback totale).";

        return new AggregazioneDatiComuniResult(
            isSuccess: false,
            message: message,
            tabelleElaborate: tabelleElaborate,
            tabelleConVariazioni: 0,
            dimensioneTotaleByte: 0,
            failedTable: failedTable,
            errorMessage: errorMessage);
    }
}
