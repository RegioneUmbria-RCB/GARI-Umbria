using AgronicaCoreModelsSTD.anagrafiche;

namespace AgronicaCoreModelsSTD.attivita.risorse
{
    public class RisorsaAssegnatarioVisita : Risorsa
    {
        public RisorseUmane risorsaUmana { get; set; }

        public RisorsaAssegnatarioVisita()
        {
            classType = costanti.ClassType.RisorsaAssegnatarioVisita;
        }
    }
}
