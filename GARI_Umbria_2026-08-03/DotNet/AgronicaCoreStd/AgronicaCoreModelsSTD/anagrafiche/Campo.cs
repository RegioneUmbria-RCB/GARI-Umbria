using System;
using System.Collections.Generic;
using System.Text;
using AgronicaCoreModelsSTD.analisi;
using AgronicaCoreModelsSTD.metaschema.utilizzi;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    public class Campo 
    {

        public string descrizione { get; set; }
        public IntervalloTemporale validita { get; set; }
        public string campo_Codice { get; set; }
        public int orientamento_Colturale { get; set; }

        public Specie specie { get; set; }

        public bool serra { get; set; }

        public List<CodiciAnagrafeValori> codici { get; set; }

        public List<CatastoCampo> catastoCampo { get; set; }

        public List<AppezzamentoCampo> appezzamentoCampo { get; set; }

        /// <summary>
        /// dato cartografico associato
        /// </summary>
        public string cartografia { get; set; }

        /// <summary>
        /// flag identificativo se l'appezzamento è disegnato tramite tramite il gps
        /// </summary>
        public bool flag_gps { get; set; }

        //public List<Codici> Codici { get; set; }
        //public List<Catasto_Campo> Catasto { get; set; }

        public PK primaryKey { get; set; }

        public bool flag_cancellazione { get; set; }

        public Campo (PK primaryKey)
        {
            this.primaryKey = primaryKey;
            this.flag_cancellazione = false;
        }

        public Campo()
        {
            flag_cancellazione = false;
        }

        public class PK
        {
            public CentroAziendale.PK centroAziendalePK { get; set; }

            public int codice { get; set; }

            public PK(int codice,  CentroAziendale.PK centroAziendalePK)
            {
                this.codice = codice;
                this.centroAziendalePK = centroAziendalePK;
            }

            public PK()
            {

            }

        }
    }


}
