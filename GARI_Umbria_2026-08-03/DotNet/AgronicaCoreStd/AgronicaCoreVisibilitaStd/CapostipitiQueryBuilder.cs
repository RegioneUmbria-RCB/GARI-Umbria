using AgronicaCoreVisibilitaStd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AgronicaCoreVisibilitaStd
{
    /// <summary>
    /// Costruisce la query di espansione gerarchia a partire dalle pive "capostipite":
    /// replica la CTE ricorsiva di <c>AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio.PopolaConGerarchia</c>,
    /// ma in forma SELECT (non INSERT) cosi' il chiamante puo' raccogliere il DataTable
    /// e passarlo al writer UVA o combinatore.
    /// </summary>
    public static class CapostipitiQueryBuilder
    {
        /// <summary>
        /// Restituisce il SELECT DISTINCT (Piva, Sa_Cod) espandendo la gerarchia a partire da <paramref name="pive"/>.
        /// Ritorna <c>null</c> se la lista e' vuota oppure contiene il wildcard
        /// (<see cref="CapostipitiHelper.WildcardPiva"/>) — caso legacy "visibilita totale o nulla".
        /// La colonna Sa_Cod vale 0 per le righe derivate dalla sola gerarchia, oppure il valore reale
        /// di <c>Centri_Aziendali.sa_cod</c> quando presente.
        /// </summary>
        public static BuildPraticheResult BuildQueryEspansioneGerarchia(IEnumerable<string> pive)
        {
            if (pive == null)
                return null;

            var lista = pive
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Select(p => p.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (lista.Count == 0) return null;
            if (lista.Contains(CapostipitiHelper.WildcardPiva, StringComparer.Ordinal)) return null;

            var parameters = new Dictionary<string, object>();
            var placeholders = new List<string>(lista.Count);
            for (int i = 0; i < lista.Count; i++)
            {
                var name = "@pivaCapo_" + i;
                placeholders.Add(name);
                parameters[name] = lista[i];
            }

            var stb = new StringBuilder();
            stb.AppendLine(" WITH imprese_figlie AS ( ");
            stb.AppendLine("   SELECT padre, figlio, livello, cast(livello as int) as depth ");
            stb.AppendLine("   FROM GerarchiaImprese WITH (NOLOCK) ");
            stb.AppendLine("   WHERE figlio IN (" + string.Join(", ", placeholders) + ") ");
            stb.AppendLine("   UNION ALL ");
            stb.AppendLine("   SELECT f.padre, f.figlio, f.livello, p.depth + 1 as depth ");
            stb.AppendLine("   FROM GerarchiaImprese f WITH (NOLOCK) ");
            stb.AppendLine("   INNER JOIN imprese_figlie p ON f.Padre = p.Figlio ");
            stb.AppendLine("   WHERE f.livello = p.depth + 1 ");
            stb.AppendLine(" ) ");
            stb.AppendLine(" SELECT DISTINCT Piva, Sa_Cod ");
            stb.AppendLine(" FROM ( ");
            stb.AppendLine("   SELECT imprese_figlie.Figlio as Piva, ");
            stb.AppendLine("          isnull(Centri_Aziendali.sa_cod, 0) as Sa_Cod ");
            stb.AppendLine("   FROM imprese_figlie ");
            stb.AppendLine("   LEFT JOIN Centri_Aziendali WITH (NOLOCK) ON Centri_Aziendali.PIVA = imprese_figlie.Figlio ");
            stb.AppendLine("   UNION ALL ");
            stb.AppendLine("   SELECT a.Figlio as Piva, 0 as Sa_Cod ");
            stb.AppendLine("   FROM imprese_figlie a ");
            stb.AppendLine(" ) m ");

            return new BuildPraticheResult(stb.ToString(), parameters);
        }
    }
}
