using System;
using System.Collections.Generic;
using System.Linq;

namespace InData.AntaresTrace
{
    public interface IOperazioneColturaleDto
    {
        int CodiceAttivita { get; set; }
        DateTime DataAttivita { get; set; }
        string DescrizioneAttivita { get; set; }
        string IdAttivita { get; set; }
        string PartitaIva { get; set; }
        int CodiceSedeAziendale { get; set; }
        List<ImpiantoDto> Impianti { get; set; }
        string SaNome { get; set; }
        int SpecieVegetale { get; set; }
        float SuperficieTrattataTotale { get; set; }
    }

    public static class IOperazioneColturaleDtoExtensions
    {
        public static List<int> ListIdAgenda(this IEnumerable<IOperazioneColturaleDto> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return source.Select(x => int.Parse(x.IdAttivita)).ToList();
        }

        public static List<int> ListIdAgenda(this IEnumerable<IOperazioneColturaleDto> source, Predicate<IOperazioneColturaleDto> filter)
        {
            if (source == null || filter == null)
                throw new ArgumentNullException(nameof(source));

            return source.Where(item => filter(item)).Select(x => int.Parse(x.IdAttivita)).ToList();
        }
    }
}