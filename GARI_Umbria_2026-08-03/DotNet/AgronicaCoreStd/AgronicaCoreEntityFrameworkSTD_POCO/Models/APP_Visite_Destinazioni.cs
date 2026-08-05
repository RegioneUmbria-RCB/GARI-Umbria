using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreEntityFrameworkSTD_POCO.Models
{
    public class APP_Visite_Destinazioni : APP_Agronica_Entity
    {

        public int Visita_Cod { get; set; }
        public int Visita_Destinazione_Cod { get; set; }
        public string Piva { get; set; }
        public int Sa_Cod { get; set; }
        public int Appezza { get; set; }
        public int Id_Reg { get; set; }
        public decimal Qta { get; set; }
        public decimal Qta2 { get; set; }
        public int Tipo_Destinazione { get; set; }
        public decimal QuotaDistribuzione { get; set; }
    }
}
