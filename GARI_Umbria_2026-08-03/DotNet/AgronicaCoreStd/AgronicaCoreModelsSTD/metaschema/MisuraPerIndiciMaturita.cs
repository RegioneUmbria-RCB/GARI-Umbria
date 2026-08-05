using System.Collections.Generic;

namespace AgronicaCoreModelsSTD.metaschema
{
    public class ListaMisuraPerIndiciMaturita
    {
        public List<MisuraPerIndiciMaturita> ListaMisure { get; set; }
    }
    public class MisuraPerIndiciMaturita
    {
        public string Codice { get; set; }
        public string Descrizione { get; set; }
    }
}
