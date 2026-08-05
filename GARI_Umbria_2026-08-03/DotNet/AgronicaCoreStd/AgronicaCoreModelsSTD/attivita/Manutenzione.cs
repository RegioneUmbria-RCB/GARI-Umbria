using AgronicaCoreModelsSTD.anagrafiche;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.attivita
{
    public class Manutenzione
    {
        public int codice;
        public ParcoMacchine macchina;
        public TipoManutenzione type;
        public DateTime dataIntervento;
        public string fornitore;
        public string descrizione;
        public string pezziSostituiti;
        public decimal oreManodopera;
        public decimal importoRicambiNoIva;
        public decimal importoManodoperaNoIva;
        public List<RisorseUmane> risorseUmane;
        public string guid;
        public bool cancellato;
    }

    public enum TipoManutenzione
    {
        MANUTENZIONE = 0,
        ROTTURA = 1,
        USURA = 2,
        TARATURA = 3,
        LAVAGGIO = 4,
        ALTRO = 5,
        UKNOWN = 100
    }
}
