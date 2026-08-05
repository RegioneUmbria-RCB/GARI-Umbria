using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreEntityFrameworkSTD_POCO.Models
{
    public class APP_CategorieXUnitaMisura: APP_Agronica_Entity
    {

        public int Elem_Cod { get; set; }
        public int Udm_Cod { get; set; }
        public string NomeComune { get; set; }
        public string Udm_des { get; set; }
        public string Udm_Sim { get; set; }


    }
}
