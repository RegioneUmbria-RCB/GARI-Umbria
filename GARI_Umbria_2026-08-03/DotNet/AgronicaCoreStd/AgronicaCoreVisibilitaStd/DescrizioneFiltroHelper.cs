using System;

namespace AgronicaCoreVisibilitaStd
{
    /// <summary>
    /// Normalizza il frammento SQL memorizzato in utenti_profili.Descrizione_2
    /// prima di essere concatenato alla WHERE della query di visibilit&#224;.
    /// Replica la logica di Filtrone.CreaStringaQueryPerDTFiltrone (righe 590-602).
    /// </summary>
    public static class DescrizioneFiltroHelper
    {
        /// <summary>
        /// Restituisce il filtro con prefisso "AND " quando manca.
        /// - null / whitespace -&gt; stringa vuota
        /// - gi&#224; inizia con "AND" (case-insensitive) -&gt; passato invariato (trim)
        /// - altrimenti: "AND " + filtro
        /// </summary>
        public static string Normalizza(string descrizione2)
        {
            if (string.IsNullOrWhiteSpace(descrizione2))
                return string.Empty;

            var trimmed = descrizione2.Trim();

            if (trimmed.StartsWith("AND", StringComparison.OrdinalIgnoreCase))
                return trimmed;

            return "AND " + trimmed;
        }
    }
}
