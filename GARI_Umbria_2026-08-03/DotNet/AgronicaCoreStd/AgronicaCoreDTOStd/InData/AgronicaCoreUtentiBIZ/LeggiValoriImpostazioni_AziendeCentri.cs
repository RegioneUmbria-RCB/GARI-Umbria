using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.AgronicaCoreUtentiBIZ
{
    public class LeggiValoriImpostazioni_AziendeCentri
    {
        /// <summary>
        /// Lista contenente i codici delle impostazioni da leggere
        /// </summary>
       public List<int>  Impostazioni {get;set;}
       public List<ImpresaDto> Imprese { get; set; }

    }

    public class LeggiImpostazioniAziendaCentro
    {
        /// <summary>
        /// Lista contenente i codici delle impostazioni da leggere
        /// </summary>
        public List<int> Impostazioni { get; set; }
        public ImpresaDto ImpresaCentro { get; set; }
    }
}
