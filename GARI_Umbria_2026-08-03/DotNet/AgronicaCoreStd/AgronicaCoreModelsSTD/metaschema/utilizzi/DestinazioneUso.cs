using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.metaschema.utilizzi
{
    public class DestinazioneUso : UtilizzoTerreno
    {
        public DestinazioneUso(int codice) : base(codice) 
        {
            classType = costanti.ClassType.DestinazioneUso;
            descrizione = "";
        }

        public DestinazioneUso() : base()
        {
            classType = costanti.ClassType.DestinazioneUso;
        }
    }
}
