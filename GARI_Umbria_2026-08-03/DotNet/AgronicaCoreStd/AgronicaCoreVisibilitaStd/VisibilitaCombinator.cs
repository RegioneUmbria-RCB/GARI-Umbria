using System;
using System.Collections.Generic;
using System.Linq;

namespace AgronicaCoreVisibilitaStd
{
    public static class VisibilitaCombinator
    {
        public static HashSet<string> Combina(
            IEnumerable<string> gerarchiaPive,
            IEnumerable<string> praticheVisibiliPive,
            string operatore)
        {
            var gerarchia = ToPivaSet(gerarchiaPive);
            var pratiche = ToPivaSet(praticheVisibiliPive);

            if (IsOr(operatore))
            {
                gerarchia.UnionWith(pratiche);
                return gerarchia;
            }

            gerarchia.IntersectWith(pratiche);
            return gerarchia;
        }

        /// <summary>
        /// Combina i centri aziendali secondo l'operatore.
        ///
        /// OR: union dei centri di gerarchia con i centri delle pive-pratiche-only
        /// (passati dal caller che li recupera via query extra su Centri_Aziendali).
        /// Se <paramref name="praticheCentri"/> &#232; null/empty in OR, vengono restituiti
        /// solo i centri di gerarchia (degrada correttamente quando non ci sono
        /// pive-pratiche-only nuove rispetto a gerarchia).
        ///
        /// AND: intersezione; un centro resta visibile solo se la sua piva
        /// &#232; in <paramref name="praticheVisibiliPive"/>.
        /// </summary>
        public static HashSet<Tuple<string, int>> CombinaCentri(
            IEnumerable<Tuple<string, int>> gerarchiaCentri,
            IEnumerable<Tuple<string, int>> praticheCentri,
            IEnumerable<string> praticheVisibiliPive,
            string operatore)
        {
            var gerarchia = ToCentriSet(gerarchiaCentri);
            var pratiche = ToPivaSet(praticheVisibiliPive);

            if (IsOr(operatore))
            {
                var extra = ToCentriSet(praticheCentri);
                gerarchia.UnionWith(extra);
                return gerarchia;
            }

            return new HashSet<Tuple<string, int>>(gerarchia.Where(c => pratiche.Contains(c.Item1)));
        }

        private static bool IsOr(string operatore)
        {
            return string.Equals(operatore, "OR", StringComparison.OrdinalIgnoreCase);
        }

        private static HashSet<string> ToPivaSet(IEnumerable<string> pive)
        {
            return new HashSet<string>(
                (pive ?? Enumerable.Empty<string>())
                    .Where(p => !string.IsNullOrWhiteSpace(p))
                    .Select(p => p.Trim()),
                StringComparer.OrdinalIgnoreCase);
        }

        private static HashSet<Tuple<string, int>> ToCentriSet(IEnumerable<Tuple<string, int>> centri)
        {
            return new HashSet<Tuple<string, int>>(
                (centri ?? Enumerable.Empty<Tuple<string, int>>())
                    .Where(c => c != null && !string.IsNullOrWhiteSpace(c.Item1))
                    .Select(c => Tuple.Create(c.Item1.Trim(), c.Item2)));
        }
    }
}
