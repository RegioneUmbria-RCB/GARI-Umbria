using System;
using System.Collections.Generic;
using System.Text;
using AgronicaCoreModelsSTD.anagrafiche;

namespace AgronicaCoreDTOStd.InData.Zoo
{
    public class LeggiGiacenzeZoo
    {
        public Impresa impresa { get; set; }
        public CentroAziendale centro { get; set; }
        public Fabbricato stalla { get; set; }
        public SottogruppoStalla raggruppamento { get; set; }
        public int codAnimale { get; set; }
        public string matricola { get; set; }
        public DateTime data { get; set; }
        public bool bAll { get; set; }
        public bool filtraFornitori { get; set; }
        public List<int> lista_CodAnimale { get; set; }
        /// <summary>
        /// Mostra data primo giorno caricamento e giorni in stalla da quella data
        /// </summary>
        public bool mostraGGPrimoCaricamento { get; set; }
    }
}
