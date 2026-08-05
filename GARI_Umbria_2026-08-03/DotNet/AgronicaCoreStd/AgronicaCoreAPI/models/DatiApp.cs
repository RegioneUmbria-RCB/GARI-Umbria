using AgronicaCoreModelsSTD.attivita;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgronicaCoreAPI.models
{
    public class DatiApp
    {
        public List<Attivita> attivita = new List<Attivita>();
        public List<Attivita> rilievi = new List<Attivita>();
        public List<Attivita> visite = new List<Attivita>();
        public List<Attivita> attivitaPianificate = new List<Attivita>();
        public List<AgronicaCoreModelloSTD.DocumentoPerScarico> documenti = new List<AgronicaCoreModelloSTD.DocumentoPerScarico>();
        public List<MovimentoDiMagazzino> movimenti = new List<MovimentoDiMagazzino>();
        public List<Manutenzione> manutenzioni = new List<Manutenzione>();
        public List<Acquisto> acquisti = new List<Acquisto>();
    }
}
