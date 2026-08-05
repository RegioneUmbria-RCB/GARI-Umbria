using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.attivita.risorse;
using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreModelsSTD.metaschema.utilizzi;
using System;
using System.Collections.Generic;
using System.Text;
using AgronicaCoreModelsSTD.baseClass;

namespace InData.Operazione
{
    public class LeggiRilievi
    {
        public Impresa AziendaRilievi;
        public CentroAziendale CentroAziendaleRilievi;
        public BaseCodeDescr TipoRilievi;
        public UtilizzoTerreno SpecieRilievi;
        public DateTime Da;
        public DateTime A;
    }
}
