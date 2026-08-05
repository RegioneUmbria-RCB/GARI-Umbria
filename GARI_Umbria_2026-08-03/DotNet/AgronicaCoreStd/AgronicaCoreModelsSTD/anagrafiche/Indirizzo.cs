using AgronicaCoreModelsSTD.Gis;
using AgronicaCoreModelsSTD.metaschema;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    public class Indirizzo
    {

        /// <summary>
        /// Cod_indirizzo su db
        /// </summary>
        public int codice { get; set; }

        public Istat istatComune { get; set; }

        public string via { get; set; }
        public string frazione { get; set; }
        public string cap { get; set; }
        public CodiciNazioniISO3166 stato { get; set; }
        public string note { get; set; }
        public LatLng geolocation { get; set; }
        public bool flag_cancellazione { get; set; }


        public Indirizzo()
        {
            flag_cancellazione = false;
            this.geolocation = new LatLng(0, 0);
        }

        public Indirizzo(int codice)
        {
            this.codice = codice;
            flag_cancellazione = false;
            this.geolocation = new LatLng(0, 0);
        }

        public Indirizzo(int codice, Istat istatComune, string via, string frazione, string cap, CodiciNazioniISO3166 stato, string note, bool flag_cancellazione) : this(codice)
        {
            this.istatComune = istatComune;
            this.via = via;
            this.frazione = frazione;
            this.cap = cap;
            this.stato = stato;
            this.note = note;
            this.flag_cancellazione = flag_cancellazione;
            this.geolocation = new LatLng(0, 0);
        }
        
        public Indirizzo(int codice, Istat istatComune, string via, string frazione, string cap, CodiciNazioniISO3166 stato, string note, bool flag_cancellazione, decimal lat, decimal lng) : this(codice)
        {
            this.istatComune = istatComune;
            this.via = via;
            this.frazione = frazione;
            this.cap = cap;
            this.stato = stato;
            this.note = note;
            this.flag_cancellazione = flag_cancellazione;
            this.geolocation = new LatLng(lat, lng);
        }
    }
}
