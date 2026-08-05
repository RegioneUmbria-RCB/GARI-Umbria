using AgronicaCoreModelsSTD.metaschema.utilizzi;

namespace AgronicaCoreModelsSTD.attivita.risorse
{
    public class RisorsaSpecie: Risorsa
    {
        public Specie specie { get; set; }

        public RisorsaSpecie()
        {
            classType = costanti.ClassType.RisorsaSpecie;
        }

    }
}
