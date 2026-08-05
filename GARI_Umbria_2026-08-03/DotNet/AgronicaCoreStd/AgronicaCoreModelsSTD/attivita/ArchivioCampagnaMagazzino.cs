using System.Collections.Generic;

namespace AgronicaCoreModelsSTD.attivita
{
    public class ArchivioCampagnaMagazzino
    {
        public ArchivioAttivitaCampagna ArchivioAttivitaCampagna { get; set; } = new ArchivioAttivitaCampagna();
        public List<MovimentoDiMagazzino> MovimentiMagazzino { get; set; } = new List<MovimentoDiMagazzino>();
        public List<Acquisto> Acquisti { get; set; } = new List<Acquisto>();
        public List<RilevamentoDiMagazzinoEntityApp> Giacenze { get; set; } = new List<RilevamentoDiMagazzinoEntityApp>();
        public string DataUltimaSincro { get; set; }
    }
}
