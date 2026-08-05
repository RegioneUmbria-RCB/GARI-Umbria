using AgronicaCoreDTOStd.InData.Budget;
using AgronicaCoreModelsSTD.profilazione;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Metaschema
{
    public class ScriviGruppoUtentexTransizioniStato
    {
        public GruppoUtente Gruppo { get; set; }
        public List<TransizioneDiStato> Transizioni { get; set; }
        public enum_TipoOperazioneDB Operazione { get; set; }
    }

    public class ScriviGruppoUtente
    {
        public GruppoUtente Gruppo { get; set; }
        public enum_TipoOperazioneDB Operazione { get; set; }
    }
}
