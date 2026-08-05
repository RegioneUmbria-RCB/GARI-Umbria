using AgronicaCoreModelsSTD.attivita.centri_di_costo;
using AgronicaCoreModelsSTD.attivita.risorse;
using AgronicaCoreModelsSTD.metaschema.utilizzi;
using System;
using System.Collections.Generic;

namespace AgronicaCoreModelsSTD.attivita.dettagli
{
    public class DettaglioRaccolta : RisorsaProdotto
    {
        public Specie specie { get; set; } //TODO_DT: verificare con Carlo se è usata
        public string codArticolo { get; set; } //TODO_DT: verificare con Carlo se è usata
        public int regolamento { get; set; } //TODO_DT: verificare con Carlo se è usata

        public int culCod { get; set; } //TODO_DT: verificare con Carlo se è usata
        public Varieta varieta { get; set; } //TODO_DT: verificare con Carlo se è usata

        public GruppoFinalita finalita { get; set; }

        public int calCod { get; set; }

        public List<QuantitaSuImpianto> QuantitaSuImpianti { get; set; }

        /// <summary>
        /// Raccoglie le opzioni usate per la raccolta.
        /// </summary>
        public Opzioni_Raccolta Opzioni_Raccolta { get; set; }

        public DateTime dataIngresso { get; set; }

        public DettaglioRaccolta()
        {
            classType = costanti.ClassType.DettaglioRaccolta;
            Opzioni_Raccolta = new Opzioni_Raccolta();
        }
    }
}
