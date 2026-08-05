using System;
using System.Collections.Generic;
using System.Text;

namespace InData.Zoo
{
    public class SpostamentoGruppi
    {
        public string Codice { get; set; }
        public string Piva { get; set; }
        public int SaCod { get; set; }
        public int StaNum { get; set; }
        public DateTime Inizio { get; set; }
        public List<Gruppo> GruppiOrigine { get; set; }
        public Gruppo GruppoDestinazione { get; set; }
    }

    public class Gruppo
    {
        public int CodiceGruppo { get; set; }
        public string DescrizioneGruppo { get; set; }
    }
}
