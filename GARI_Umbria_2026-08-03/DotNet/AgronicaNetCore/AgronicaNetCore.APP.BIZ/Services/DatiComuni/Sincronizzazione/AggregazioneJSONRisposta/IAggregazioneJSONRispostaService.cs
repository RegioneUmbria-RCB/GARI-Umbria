using AgronicaNetCore.APP.BIZ.Exceptions;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.APP.BIZ.Services.DatiComuni.Sincronizzazione.AggregazioneJSONRisposta;

/// <summary>
/// Reads consolidated JSON payloads from <c>app_preparazione_daticomuni_web2app</c>
/// for the tables requested by the client and aggregates them into a single structured
/// JSON payload ready for API delivery.
/// Ref: DS09-BL – Nome: AggregazioneJSONRisposta.
/// </summary>
public interface IAggregazioneJSONRispostaService
{
    /// <summary>
    /// Reads <c>json_content</c> for every table in <paramref name="tabelleRichieste"/>,
    /// validates each value, assembles the aggregated payload
    /// <c>{ "tabella1": {...}, "tabella2": {...} }</c> and — when requested — applies
    /// gzip compression.
    /// Ref: DS09-BL – Descrizione; Regole di Business.
    /// </summary>
    /// <param name="tabelleRichieste">
    /// Names of the common tables to include in the payload (≤ 36).
    /// </param>
    /// <param name="abilitaCompressione">
    /// When <c>true</c> the payload is also gzip-compressed and the compressed bytes are
    /// included in the result. The timeout budget increases to 500 ms.
    /// Ref: DS09-BL – Input: abilita_compressione.
    /// </param>
    /// <param name="objParametriServer">Server context used for data-provider resolution.</param>
    /// <exception cref="JsonAggregationException">
    /// Thrown when one or more <c>json_content</c> values are not valid JSON.
    /// Ref: DS09-BL – Eccezioni: JsonAggregationException.
    /// </exception>
    /// <exception cref="PayloadSizeExceededException">
    /// Thrown when the uncompressed payload exceeds the configured maximum size.
    /// Ref: DS09-BL – Eccezioni: PayloadSizeExceededException.
    /// </exception>
    /// <exception cref="AggregationTimeoutException">
    /// Thrown when the operation exceeds 300 ms (or 500 ms with compression).
    /// Ref: DS09-BL – Eccezioni: AggregationTimeoutException.
    /// </exception>
    Task<AggregazioneJSONRispostaResult> AggregaAsync(
        List<string> tabelleRichieste,
        FiltriRichiesti filtriRichiesti,
        bool abilitaCompressione,
        AgronicaCoreParametriServer objParametriServer);
}
