using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Gis
{
    public class SalvaNuovoMultiPoint_Out
    {
        public bool Esito { get; set; }
        public List<Obj_SalvaGrafica> Lista_obj_SalvaGrafica { get; set; }
    }
}
