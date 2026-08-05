using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreModelsSTD.metaschema;
using AgronicaCoreModelsSTD.metaschema.avversita;
using AgronicaCoreModelsSTD.metaschema.utilizzi;
using System;

namespace InData.Anagrafica
{
    public class LeggiProdotti
    {
        public Impresa impresa { get; set; }
        public CentroAziendale centroAziendale { get; set; }
        public Attivita.Tipo_Attivita tipoAttivita { get; set; }

        public Attivita.Stati statoAttivita { get; set; }

        public Lavorazione lavorazione { get; set; }

        public Impianto[] impianti { get; set; }
        public MovimentoDiMagazzino[] prodottiDaTrattare { get; set; }

        //public UtilizzoTerreno utilizzoTerreno { get; set; }
        public Specie specie { get; set; }
        public Varieta varieta { get; set; }
        public Regolamenti regolamento { get; set; }
        public GruppoFinalita finalita { get; set; }


        public Disciplinare disciplinare { get; set; }
        
        public Epoca epocaDPI { get; set; }

        public AvversitaGruppo avversitaGruppo { get; set; }

        public string filtroPerDescrizione { get; set; }

        public DateTime data { get; set; }
        
        public bool escludiGiacenzeZero { get; set; }

        public bool magazziniAgenzie { get; set; }

        public bool magazziniEsterni { get; set; }

        public Pua pua { get; set; } 

    }
}
