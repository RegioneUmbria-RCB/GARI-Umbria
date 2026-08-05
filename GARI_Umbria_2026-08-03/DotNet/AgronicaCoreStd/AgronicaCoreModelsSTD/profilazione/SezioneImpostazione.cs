using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.profilazione
{
    /// <summary>
    /// Definisce una sezione nella gerarchia di sezioni impostazioni definita 
    /// dalle tabelle Guida_Impostazioni e Guida_Impostazioni_Sezioni.
    /// </summary>
    public class SezioneImpostazione : BaseCodeDescr
    {
        public bool espandibile { get; set; }
        public int flagLivello { get; set; }
        public List<SezioneImpostazione> children { get; set; }

        public SezioneImpostazione(int code, string descr) : base(code, descr)
        {
            this.children = new List<SezioneImpostazione>();
        }
    }
}
