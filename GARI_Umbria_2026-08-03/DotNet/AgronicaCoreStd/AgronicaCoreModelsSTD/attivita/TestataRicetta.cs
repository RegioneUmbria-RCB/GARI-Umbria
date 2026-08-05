using System;
using AgronicaCoreModelsSTD.metaschema;

namespace AgronicaCoreModelsSTD.attivita
{
    public class TestataRicetta
    {
        public int Ricetta_Cod { get; set; }

        /// <summary>
        /// Valorizzato solamente se siamo in una Ricetta di tipo PUA
        /// </summary>
        public Pua pua { get; set; }

        public string Ricetta_Des { get; set; }
        public string Ricetta_Des_Long { get; set; }
        public string Ricetta_Numero { get; set; }
        public string Note { get; set; }
        public DateTime Data_Da { get; set; }
        public DateTime Data_A { get; set; }
    }
}
