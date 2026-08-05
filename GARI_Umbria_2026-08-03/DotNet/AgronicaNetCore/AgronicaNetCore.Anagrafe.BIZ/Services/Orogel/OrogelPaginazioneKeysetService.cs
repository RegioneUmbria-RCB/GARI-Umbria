using System.Text;
using AgronicaNetCore.Anagrafe.BIZ.Exceptions.Orogel;
using AgronicaNetCore.Anagrafe.BIZ.Resources;
using AgronicaNetCore.Anagrafe.BIZ.Services.Orogel.Models;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.Anagrafe.BIZ.Services.Orogel
{
    /// <summary>
    /// DS04-BL: Pure in-memory implementation of <see cref="IOrogelPaginazioneKeysetService"/>.
    /// Implements cursor-based (Keyset) Pagination for all Orogel BI APIs (FS001–FS005).
    /// No database interaction — generates and decodes <c>§</c>-delimited cursors.
    /// </summary>
    public class OrogelPaginazioneKeysetService
        : BaseServiceAnagrafeBIZ,
            IOrogelPaginazioneKeysetService
    {
        private const int MaxPageSize = 1000;
        private const char KeySeparator = '§';
        private const string StringFallback = "";
        private const int IntegerFallback = -1;

        /// <summary>
        /// DS04-BL: Initializes <see cref="OrogelPaginazioneKeysetService"/>.
        /// </summary>
        public OrogelPaginazioneKeysetService(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer
        )
            : base(provider, localizer) { }

        /// <inheritdoc/>
        public KeysetFiltroResult DecodificaNextKey(
            string? nextKey,
            int pageSize,
            IReadOnlyList<KeysetColonnaDefinizione> colonneOrdinamento
        )
        {
            if (pageSize > MaxPageSize)
                throw new PaginationSizeExceededException(pageSize);

            if (string.IsNullOrEmpty(nextKey))
                return BuildFiltro(
                    colonneOrdinamento,
                    BuildDefaultValues(colonneOrdinamento),
                    nextKeyDecodificato: null
                );

            return DecodificaCursor(nextKey, colonneOrdinamento);
        }

        /// <inheritdoc/>
        public KeysetMetadatiPaginazione GeneraMetadati(
            int pageSize,
            int recordEstratti,
            IReadOnlyList<KeysetColonnaDefinizione> colonneOrdinamento,
            IReadOnlyDictionary<string, object?> ultimoRecord
        )
        {
            if (recordEstratti < pageSize)
                return new KeysetMetadatiPaginazione(pageSize, null);

            string[] keyParts = colonneOrdinamento
                .Select(c => EstraiValoreChiave(ultimoRecord, c))
                .ToArray();

            string nuovoNextKey = string.Join(KeySeparator, keyParts);
            return new KeysetMetadatiPaginazione(pageSize, nuovoNextKey);
        }

        /// <remarks>DS04-BL §Prima Pagina: Builds default values used as cursor for first page.</remarks>
        private static Dictionary<string, object> BuildDefaultValues(
            IReadOnlyList<KeysetColonnaDefinizione> colonneOrdinamento
        )
        {
            return colonneOrdinamento.ToDictionary(
                c => c.NomeColonna,
                c =>
                    (object)(
                        c.TipoDato == KeysetTipoDato.Integer ? IntegerFallback : StringFallback
                    )
            );
        }

        /// <remarks>DS04-BL §Pagine Successive: Splits and parses the incoming cursor.</remarks>
        private static KeysetFiltroResult DecodificaCursor(
            string nextKey,
            IReadOnlyList<KeysetColonnaDefinizione> colonneOrdinamento
        )
        {
            string[] parts = nextKey.Split(KeySeparator);

            if (parts.Length != colonneOrdinamento.Count)
                throw new InvalidNextKeyException(nextKey, colonneOrdinamento.Count, parts.Length);

            var decodedValues = new Dictionary<string, object>(colonneOrdinamento.Count);
            for (int i = 0; i < colonneOrdinamento.Count; i++)
            {
                KeysetColonnaDefinizione colonna = colonneOrdinamento[i];
                decodedValues[colonna.NomeColonna] = ParseValue(parts[i], colonna, nextKey);
            }

            IReadOnlyDictionary<string, object> decoded = decodedValues;
            return BuildFiltro(colonneOrdinamento, decodedValues, decoded);
        }

        /// <remarks>DS04-BL §Output: Builds WHERE fragment and SQL parameters from resolved values.
        /// First page (nextKeyDecodificato == null): returns empty FiltroWherePaginazione and empty
        /// ParSqlPaginazione so DALs skip the keyset WHERE entirely.</remarks>
        private static KeysetFiltroResult BuildFiltro(
            IReadOnlyList<KeysetColonnaDefinizione> colonneOrdinamento,
            Dictionary<string, object> valori,
            IReadOnlyDictionary<string, object>? nextKeyDecodificato
        )
        {
            if (nextKeyDecodificato == null)
                return new KeysetFiltroResult(string.Empty, new Dictionary<string, object>(), null);

            var parSql = new Dictionary<string, object>(colonneOrdinamento.Count);
            foreach (KeysetColonnaDefinizione colonna in colonneOrdinamento)
                parSql[ToParamName(colonna.NomeColonna)] = valori[colonna.NomeColonna];

            string where = BuildWhereCondition(colonneOrdinamento);

            return new KeysetFiltroResult(where, parSql, nextKeyDecodificato);
        }

        /// <remarks>
        /// DS04-BL §Pagine Successive logica a cascata: Generates SQL WHERE using cascading OR.
        /// Single key  → <c>PIVA &gt; @LastPiva</c>
        /// Composite   → <c>(A &gt; @LastA) OR (A = @LastA AND B &gt; @LastB)</c>
        /// </remarks>
        private static string BuildWhereCondition(
            IReadOnlyList<KeysetColonnaDefinizione> colonneOrdinamento
        )
        {
            if (colonneOrdinamento.Count == 1)
            {
                string col = colonneOrdinamento[0].NomeColonna;
                return $"{col} > {ToParamName(col)}";
            }

            var conditions = new List<string>(colonneOrdinamento.Count);
            for (int depth = 0; depth < colonneOrdinamento.Count; depth++)
            {
                var sb = new StringBuilder("(");
                for (int i = 0; i < depth; i++)
                {
                    string equalCol = colonneOrdinamento[i].NomeColonna;
                    sb.Append($"{equalCol} = {ToParamName(equalCol)} AND ");
                }
                string greaterCol = colonneOrdinamento[depth].NomeColonna;
                sb.Append($"{greaterCol} > {ToParamName(greaterCol)})");
                conditions.Add(sb.ToString());
            }
            return string.Join(" OR ", conditions);
        }

        /// <remarks>DS04-BL §Fallback per null: Applies type-specific fallback for null record values.</remarks>
        private static string EstraiValoreChiave(
            IReadOnlyDictionary<string, object?> record,
            KeysetColonnaDefinizione colonna
        )
        {
            if (!record.TryGetValue(colonna.NomeColonna, out object? value) || value is null)
                return colonna.TipoDato == KeysetTipoDato.Integer
                    ? IntegerFallback.ToString()
                    : StringFallback;

            return value.ToString()!;
        }

        private static object ParseValue(
            string rawValue,
            KeysetColonnaDefinizione colonna,
            string nextKey
        )
        {
            if (colonna.TipoDato == KeysetTipoDato.Integer)
            {
                if (!int.TryParse(rawValue, out int intValue))
                    throw new InvalidNextKeyValuesException(
                        nextKey,
                        colonna.NomeColonna,
                        rawValue,
                        "integer"
                    );
                return intValue;
            }
            return rawValue;
        }

        /// <summary>
        /// DS04-BL §parSqlPaginazione: Converts a SQL column name to a <c>@LastXxx</c> parameter name.
        /// Each underscore-delimited segment is title-cased.
        /// Example: <c>PIVA</c> → <c>@LastPiva</c>, <c>sa_cod</c> → <c>@LastSaCod</c>.
        /// </summary>
        private static string ToParamName(string columnName)
        {
            string[] parts = columnName.Split('_', StringSplitOptions.RemoveEmptyEntries);
            string pascalName = string.Concat(
                parts.Select(p => char.ToUpperInvariant(p[0]) + p[1..].ToLowerInvariant())
            );
            return "@Last" + pascalName;
        }
    }
}
