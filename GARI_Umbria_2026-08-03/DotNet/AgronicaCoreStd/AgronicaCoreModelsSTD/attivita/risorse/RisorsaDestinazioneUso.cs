using AgronicaCoreModelsSTD.metaschema.utilizzi;

namespace AgronicaCoreModelsSTD.attivita.risorse
{
    public class RisorsaDestinazioneUso : Risorsa
    {
        public DestinazioneUso destinazioneUso { get; set; }

        public RisorsaDestinazioneUso()
        {
            classType = costanti.ClassType.RisorsaDestinazioneUso;
        }

    }
}
