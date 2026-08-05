using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AgronicaCoreVisibilitaStd
{
    /// <summary>
    /// Estrae la lista di pive "capostipite" dal campo <c>Utenti_Profili.Descrizione_1</c>.
    /// Replica la logica di <c>AgronicaCoreUtentiBIZ.Utenti_Visibilita.LeggiPiveCapostipiti(String)</c>.
    /// </summary>
    public static class CapostipitiHelper
    {
        private static readonly Regex _rxPive = new Regex(
            "\"(([a-z0-9A-Z#]){11,})\"+",
            RegexOptions.Compiled,
            TimeSpan.FromSeconds(3));

        /// <summary>
        /// Marker wildcard usato nel legacy per indicare "visibilita totale o nulla":
        /// se presente tra le pive estratte, il chiamante non deve inserire alcun record in UVA.
        /// </summary>
        public const string WildcardPiva = "###########";

        /// <summary>
        /// Estrae le pive (token quotati di lunghezza &gt;= 11 di alfanumerici o '#') dalla stringa XML
        /// contenuta in <c>Utenti_Profili.Descrizione_1</c>.
        /// </summary>
        public static List<string> ExtractPive(string descrizione1)
        {
            if (string.IsNullOrEmpty(descrizione1))
                return new List<string>();

            return _rxPive.Matches(descrizione1)
                .Cast<Match>()
                .Select(m => m.Value.Replace("\"", string.Empty))
                .ToList();
        }

        /// <summary>
        /// Regola legacy: lista vuota o contenente il wildcard <see cref="WildcardPiva"/> equivale a
        /// "visibilita totale o nulla" e il chiamante non deve inserire nulla in UVA.
        /// </summary>
        public static bool IsVisibilitaTotaleONulla(IEnumerable<string> pive)
        {
            if (pive == null) return true;
            var any = false;
            foreach (var p in pive)
            {
                any = true;
                if (string.Equals(p, WildcardPiva, StringComparison.Ordinal))
                    return true;
            }
            return !any;
        }
    }
}
