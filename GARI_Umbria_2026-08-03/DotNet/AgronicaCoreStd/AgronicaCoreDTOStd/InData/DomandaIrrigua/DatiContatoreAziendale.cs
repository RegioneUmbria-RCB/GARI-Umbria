using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.DomandaIrrigua
{
    public class DatiContatoreAziendale
    {
        public string piva { get; set; }
        public int anno { get; set; }
        public List<DatiAnagraficaContatori> elencoContatori { get; set; }
        public DatiAzienda datiAzienda { get; set; }
        public List<LettureContatore> elencoLetture { get; set; }
    }

    public class LettureContatore
    {
        public int id { get; set; }
        public int id_contatore { get; set; }
        public DateTime datalettura { get; set; }
        public decimal valore { get; set; }
    }

    public class DatiAnagraficaContatori
    {
        public int id_contatore { get; set; }
        public string matricola { get; set; }
        public string descrizione { get; set; }
        public DateTime ValidoDal { get; set; }
        public DateTime ValidoAl { get; set; }
    }
}
