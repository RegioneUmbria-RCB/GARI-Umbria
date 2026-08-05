using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AgronicaCoreDTOStd.InData.Zoo
{
    public class LeggiGiacenzeZooDto
    {
        [Required] public string Piva { get; set; }
        public int CodCentro { get; set; }
        public int CodStalla { get; set; }
        public int CodRaggruppamento { get; set; }
        public int CodAnimale { get; set; }
        /// <summary>
        /// non utilizzata nella BL
        /// </summary>
        public string Matricola { get; set; }
        public DateTime DataGiacenza { get; set; }
        public bool Istantanea { get; set; }
        public bool MostraPesate { get; set; }
        [DefaultValue(true)]
        public bool FiltraFornitori { get; set; }
        /// <summary>
        /// Possibile ridondanza con CodAnimale
        /// </summary>
        public List<int> ListaCodAnimali { get; set; } 
        /// <summary>
        /// Mostra data primo giorno caricamento e giorni in stalla da quella data
        /// </summary>
        public bool MostraGGPrimoCaricamento { get; set; }
        public string CFproprietario { get; set; }
    }
}
