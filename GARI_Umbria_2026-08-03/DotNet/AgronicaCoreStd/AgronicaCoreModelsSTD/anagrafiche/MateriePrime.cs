using AgronicaCoreModelsSTD.baseClass;
using AgronicaCoreModelsSTD.documenti;
using AgronicaCoreModelsSTD.metaschema;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    public class MateriePrime
    {
        public string partitaIva { get; set; }

        public CentroAziendale.PK centroPK { get; set; }

        public int categoria_prodotto { get; set; }

        public int codice { get; set; }

        public string cod_articolo { get; set; }

        public string descrizione { get; set; }

        public bool visibilitaPubblica { get; set; }

        public IntervalloTemporale validita { get; set; }

        public DateTime data_Creazione { get; set; }

        public DateTime data_Modifica { get; set; }

        // Per evitare errori di serializzazione se questa classe viene usata altrove, soluzione temporanea 
        [System.Xml.Serialization.XmlIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string UltimoEsito { get; set; }

        [System.Xml.Serialization.XmlIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public int IdUltimaChiamata { get; set; }

    }
}