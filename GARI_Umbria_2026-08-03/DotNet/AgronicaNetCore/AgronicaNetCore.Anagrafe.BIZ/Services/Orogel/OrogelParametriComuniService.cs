using System.Text.RegularExpressions;
using AgronicaNetCore.Anagrafe.BIZ.Exceptions.Orogel;
using AgronicaNetCore.Anagrafe.BIZ.Resources;
using AgronicaNetCore.Anagrafe.BIZ.Services.Orogel.Models;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.Anagrafe.BIZ.Services.Orogel
{
    /// <summary>
    /// DS03-BL: Pure in-memory implementation of <see cref="IOrogelParametriComuniService"/>.
    /// No database interaction — validates and normalizes <c>pageSize</c>, <c>codiciAzienda</c>,
    /// and <c>anno</c> shared across all Orogel BI APIs (FS001–FS005).
    /// </summary>
    public class OrogelParametriComuniService
        : BaseServiceAnagrafeBIZ,
            IOrogelParametriComuniService
    {
        private const int DefaultPageSize = 100;
        private const int MaxPageSize = 1000;
        private const int MinAnno = 2000;

        /// <remarks>DS03-BL §Regole codiciAzienda: alphanumeric, length 1–20.</remarks>
        private static readonly Regex CodiceAziendaRegex = new(
            @"^[a-zA-Z0-9]{1,20}$",
            RegexOptions.Compiled,
            TimeSpan.FromSeconds(3)
        );

        /// <summary>
        /// DS03-BL: Initializes <see cref="OrogelParametriComuniService"/>.
        /// </summary>
        public OrogelParametriComuniService(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer
        )
            : base(provider, localizer) { }

        /// <inheritdoc/>
        public ParametriComuniNormalizzatiResult NormalizzaParametriComuni(
            int anno,
            int? pageSize,
            IEnumerable<string>? codiciAzienda
        )
        {
            int annoConfermato = ValidaAnno(anno);
            int pageSizeNormalizzato = NormalizzaPageSize(pageSize);
            IReadOnlyList<string> codiciNormalizzati = NormalizzaCodiciAzienda(codiciAzienda);

            return new ParametriComuniNormalizzatiResult(
                annoConfermato,
                pageSizeNormalizzato,
                codiciNormalizzati
            );
        }

        /// <remarks>DS03-BL §Regole anno: 4-digit YYYY, range [2000, current year].</remarks>
        private static int ValidaAnno(int anno)
        {
            if (anno < 1000 || anno > 9999)
                throw new InvalidAnnoCohesionException(
                    anno,
                    "deve essere nel formato YYYY (4 cifre)"
                );

            int currentYear = DateTime.Now.Year;
            if (anno < MinAnno || anno > currentYear)
                throw new InvalidAnnoCohesionException(
                    anno,
                    $"deve essere compreso tra {MinAnno} e {currentYear}"
                );

            return anno;
        }

        /// <remarks>
        /// DS03-BL §Regole pageSize: null → 100; &lt; 1 → exception; &gt; 1000 → silently clamped to 1000.
        /// </remarks>
        private static int NormalizzaPageSize(int? pageSize)
        {
            if (pageSize is null)
                return DefaultPageSize;

            if (pageSize.Value < 1)
                throw new InvalidPageSizeException(pageSize.Value);

            return Math.Min(pageSize.Value, MaxPageSize);
        }

        /// <remarks>
        /// DS03-BL §Regole codiciAzienda: parse comma-separated, trim, uppercase, de-duplicate.
        /// No DB validation — SQL IN clause handles non-existent codes natively.
        /// Throws <see cref="InvalidCodiciAziendaFormatException"/> on invalid element format.
        /// </remarks>
        private static IReadOnlyList<string> NormalizzaCodiciAzienda(
            IEnumerable<string>? codiciAzienda
        )
        {
            if (codiciAzienda is null)
                return Array.Empty<string>();

            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (string codice in codiciAzienda)
            {
                foreach (string part in codice.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    string normalized = part.Trim().ToUpperInvariant();

                    if (string.IsNullOrEmpty(normalized))
                        continue;

                    if (!CodiceAziendaRegex.IsMatch(normalized))
                        throw new InvalidCodiciAziendaFormatException(normalized);

                    result.Add(normalized);
                }
            }

            return result.ToList().AsReadOnly();
        }
    }
}
