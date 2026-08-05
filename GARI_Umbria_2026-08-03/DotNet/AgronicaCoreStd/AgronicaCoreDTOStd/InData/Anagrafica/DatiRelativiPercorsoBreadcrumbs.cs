using InData.Anagrafica;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Anagrafica
{
    public class DatiRelativiPercorsoBreadcrumbs
    {
        public BreadcrumbParamsDto agenda { get; set; } 
        public int tipoPagina { get; set; }
    }
    public class BreadcrumbParamsDto
    {
        public string Piva { get; set; }
        public int Sa_Cod { get; set; }
        public int Campo_Cod { get; set; }
    }
    
}
