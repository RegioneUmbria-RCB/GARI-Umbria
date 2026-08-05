using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    public class CatastoEsercizio
    {

        public ParticelleCatastali particella { get; set; }

        public CodiciAnagrafeValori codice { get; set; }
        public bool flag_cancellazione { get; set; }

        public CatastoEsercizio(ParticelleCatastali particella, CodiciAnagrafeValori codice)
        {
            this.particella = particella;
            this.codice = codice;
            this.flag_cancellazione = false;
        }

        public CatastoEsercizio()
        {

        }
    }
}
