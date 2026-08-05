using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Gis
{
    /// <summary>
    /// Classe implementata per poter utilizzare AgronicaCoreGestioneRichieste.ParametriConcimazione2017, 
    /// sucessivamente è necessario Serializzarla in stringa e deserializzarla nel tipo AgronicaCoreGestioneRichieste.ParametriConcimazione2017
    /// </summary>
    public class ObjParams_Concimazione2017
    {
        /// <summary>
        /// Assolutamente da lasciare come prima property 
        /// poichè gestisce la sessione nella classe a lvl inferiore
        /// </summary>
        public bool Sessione { get; set; }
        public int Tipo_Concimazione { get; set; }
        public string Piva { get; set; }
        public int Sa_Cod { get; set; }
        public int PianoConcimazione_Testata_Cod { get; set; }
        public int Pagina_Richiesta { get; set; }   //tipo enumerativo enum_ParametriConcimazione_2008
        public int Tipo_Operazione { get; set; }   //Inserimento 1 - Visualizzazione 2
        public int SitoOrigine { get; set; }  
        public int Pagina_SitoOrigine { get; set; }  
        public int Veg_Cod { get; set; }  
        public ImpiantoGis[] ListaImpianti { get; set; }
    }
}
