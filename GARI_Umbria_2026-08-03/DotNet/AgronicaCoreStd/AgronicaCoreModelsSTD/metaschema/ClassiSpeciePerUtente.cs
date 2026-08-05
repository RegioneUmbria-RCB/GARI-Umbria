using AgronicaCoreModelsSTD.profilazione;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.metaschema
{
    public class ClassiSpeciePerUtente : IUtente
    {
        public string UserName { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public IEnumerable<int> id_specie { get; set; } = new List<int>();
    }
}
