using AgronicaCoreModelsSTD.analisi;
using AgronicaCoreModelsSTD.metaschema;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    public class RifCentroAziendale : RifImpresa
    {
        public int saCod { get; set; }

        // Costruttore vuoto per consentire deserializzazioni della classe
        public RifCentroAziendale() : base()
        {
            saCod = 0;
        }

        public RifCentroAziendale(string _piva, int _saCod) : base(_piva)
        {
            saCod = _saCod;
        }

        public String getCentroAziendaleFullId(string separator = "_")
        {
            return $"{partitaIva}{separator}{saCod}";
        }
    }

    public class CentroAziendale 
    {
        // public int Id { get; set; }
        //public int codice { get; set; }
        //public Impresa impresa { get; set; }
        public PK primaryKey { get; set; }
        public string nome { get; set; }

        public TipologiaSede tipologia { get; set; }
        public TitoloDiPossesso titolo_Di_Possesso { get; set; }
        public decimal lat { get; set; }
        public decimal lng { get; set; }

        public List<Appezzamento> appezzamenti { get; set; }
        public List<Fabbricato> fabbricati { get; set; }

        public List<CatastoCentroAziendale> catastoCentroAziendale { get; set; }
        public List<CodiciAnagrafeValori> codici { get; set; }

        public List<IndirizzoAssociato> indirizzi { get; set; }

        public List<RubricaVoci> rubricaVoci { get; set; }

        public IntervalloTemporale validita { get; set; }

        public List<OrientamentoTecnicoEconomico> orientamentoTecnicoEconomico { get; set; }

        public BioTipoAttivita bioTipoAttivita { get; set; }

        public BioOrganismoDiControllo bioOrganismoDiControllo { get; set; }

        public CentroAziendaleEsternoCollegato centroAziendaleEsternoCollegato { get; set; }

        public bool flag_cancellazione { get; set; }

        public CentroAziendale(PK primaryKey)
        {
            this.primaryKey = primaryKey;
            this.flag_cancellazione = false;
        }
        public class PK
        {
            public int codice { get; set; }
            public string partitaIva { get; set; }

            public PK(int codice, string partitaIva)
            {
                this.codice = codice;
                this.partitaIva = partitaIva;
            }

            public PK()
            {

            }
        }

        public CentroAziendale()
        {

        }

        public RifCentroAziendale getRiferimento()
        {
            return new RifCentroAziendale(this.primaryKey.partitaIva, this.primaryKey.codice);
        }
    }
}
