using AgronicaCoreModelsSTD.metaschema;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    public class IndirizzoAssociato
    {

        public Indirizzo indirizzo { get; set; }

        public int tipo_Indirizzo { get; set; }
        public bool flag_cancellazione { get; set; }

        public IndirizzoAssociato()
        {
            flag_cancellazione = false;
        }

        public IndirizzoAssociato(CodiciNazioniISO3166 stato)
        {
            flag_cancellazione = false;
            indirizzo = new Indirizzo(0);
            indirizzo.cap = "00000";
            indirizzo.frazione = "";
            indirizzo.note = "";
            indirizzo.via = "";
            indirizzo.istatComune = new Istat();
            indirizzo.istatComune.com = "000";
            indirizzo.istatComune.prov = "000";
            indirizzo.istatComune.reg = "000";
            indirizzo.stato = stato;
        }

    }
}
