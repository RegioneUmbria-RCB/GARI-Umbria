using System;
using System.Collections.Generic;
using AgronicaCoreModelsSTD.attivita;

namespace AgronicaCoreDTOStd.InData.Metaschema
{
    public class LeggiOperazioni
    {
        public string[] gruppiOperazioni { get; set; }
        public int[] lista_Lav_Cod { get; set; }
        public Boolean FiltraImpostazioniUtente { get; set; }
        public Boolean Visualizza_Solo_Operazioni_Preferite { get; set; }
        public Attivita.Tipo_Attivita tipo_Attivita { get; set; }
        public Attivita.Tipo_Ricetta tipo_Ricetta { get; set; }
        public Attivita.Stati stato { get; set; }
    }

    public class LeggiOperazioni_IN
    {
        public List<int> GruppoOperazioni { get; set; }
        public List<int> Operazioni { get; set; }
    }
}
