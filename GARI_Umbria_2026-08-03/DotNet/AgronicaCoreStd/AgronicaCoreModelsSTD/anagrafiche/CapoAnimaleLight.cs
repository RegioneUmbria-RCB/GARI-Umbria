using System;
using System.Collections.Generic;
using System.Text;
using AgronicaCoreModelsSTD.metaschema;
using AgronicaCoreModelsSTD.metaschema.utilizzi;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    /// <summary>
    /// Classe CapoAnimale con solo i campi chiave
    /// </summary>
    public class CapoAnimaleLight
    {
        #region Properties
        /// <summary>
        /// 
        /// </summary>
        public string partitaIva { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int codice { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string matricola { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string nome { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string sesso { get; set; }

        /// <summary>
        /// Se impostato a true, l'elemento è da cancellare 
        /// </summary>
        public bool flagCancellazione { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime dataNascita { get; set; }

        /// <summary>
        /// (= GEN_COD)
        /// </summary>
        public Genere genere { get; set; }

        /// <summary>
        /// Specie Animale (= SPE_COD)
        /// </summary>
        public Specie specie { get; set; }
        
        /// <summary>
        /// Razza Animale (= RAZ_COD)
        /// </summary>
        public Razza razza { get; set; }

        /// <summary>
        /// Validità inizio/fine del capo
        /// </summary>
        public IntervalloTemporale validita { get; set; }

        /// <summary>
        /// Lista degli esercizi sull'Animale
        /// </summary>
        public List<EsercizioCapoAnimale> esercizi { get; set; }

        /// <summary>
        /// Lista delle anomalie dell'Animale
        /// </summary>
        public List<AnomalieCapoAnimale> anomalie { get; set; }

        /// <summary>
        /// Note Anomalie
        /// </summary>
        public string anomalieNote { get; set; }

        /// <summary>
        /// Data di fine sospensione carne dell'Animale
        /// </summary>
        public DateTime fineSospensioneCarne { get; set; }
        
        /// <summary>
        /// Data di fine sospensione latte dell'Animale
        /// </summary>
        public DateTime fineSospensioneLatte { get; set; }

        /// <summary>
        /// Peso stimato dell'Animale
        /// </summary>
        public double pesoStimato { get; set; }

        /// <summary>
        /// Codice fiscale proprietario
        /// </summary>
        public string cfProprietario { get; set; }

        /// <summary>
        /// Indica se il capo è sotto antibiotico
        /// </summary>
        public bool antibioticoInCorso { get; set; }

        /// <summary>
        /// Indica se il capo è sotto antinfiammatorio
        /// </summary>
        public bool antinfiammatorioInCorso { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        public CapoAnimaleLight()
        {
            this.flagCancellazione = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="piva"></param>
        /// <param name="codProgetto"></param>
        /// <param name="matricola"></param>
        public CapoAnimaleLight(string piva, int codProgetto, string matricola)
        {
            this.partitaIva = piva;
            this.codice = codProgetto;
            this.matricola = matricola; 
            this.flagCancellazione = false;
        }
        #endregion
    }
}
