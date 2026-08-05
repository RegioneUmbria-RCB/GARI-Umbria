using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.documenti;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.attivita
{
    public class Acquisto
    {
        public DateTime data { get; set; }
        public string codice { get; set; }
        public CentroAziendale centroAziendale { get; set; }
        public RisorseUmane fornitore { get; set; }
        public DateTime dataDoc { get; set; }
        public string numDoc { get; set; }
        public List<MovimentoDiMagazzino> movimenti { get; set; }
        public List<Documento> documenti { get; set; }
        public string note { get; set; }
        public string guid { get; set; }
        public string versione { get; set; }
        public string origine { get; set; }
        public bool definitivo { get; set; }
        public bool cancellato { get; set; }
    }
}
