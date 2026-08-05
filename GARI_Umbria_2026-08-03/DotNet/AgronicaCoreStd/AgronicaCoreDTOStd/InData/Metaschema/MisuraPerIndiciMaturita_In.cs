using AgronicaCoreModelsSTD.metaschema;
using System.Collections.Generic;

namespace InData.Metaschema
{
    public class MisuraPerIndiciMaturita_In
    {
        public List<MisuraPerIndiciMaturitaAnagrafica> AnagraficaInsert { get; set; }
        public List<MisuraPerIndiciMaturitaAnagrafica> AnagraficaUpdate { get; set; }
        public List<MisuraPerIndiciMaturitaAnagrafica> AnagraficaDelete { get; set; }
    }
}
