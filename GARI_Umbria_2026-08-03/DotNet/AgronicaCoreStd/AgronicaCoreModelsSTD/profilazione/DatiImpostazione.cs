using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.profilazione
{
    public class ImpostazioneBase : BaseCodeDescrStr
    {
        public String Valore { get; set; }
        public String TipoCampo { get; set; }
        public String Note { get; set; }
        public String Username { get; set; }
    }

}
