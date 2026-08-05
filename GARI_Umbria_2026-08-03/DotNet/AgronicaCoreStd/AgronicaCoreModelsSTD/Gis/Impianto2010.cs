using System;
using System.Collections.Generic;
using System.Text;
namespace AgronicaCoreModelsSTD.Gis
{
    /// <summary>
    /// Classe implementata per poter utilizzare AgronicaCoreGestioneRichieste.Impianti2010, 
    /// sucessivamente è necessario Serializzarla in stringa e deserializzarla nel tipo AgronicaCoreGestioneRichieste.Impianti2010
    /// </summary>
    public class Impianto2010
    {
        public string Piva { get; set; }
        public int Sa_Cod { get; set; }
        public int Appezza { get; set; }
        public int Id_Reg { get; set; }
        public int Progetto_Cod { get; set; }
        public int veg_cod { get; set; }
        public int id_cod { get; set; }
    }
}
