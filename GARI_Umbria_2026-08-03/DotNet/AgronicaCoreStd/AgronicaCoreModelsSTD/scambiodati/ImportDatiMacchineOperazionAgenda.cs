using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.scambiodati
{
    public class ImportDatiMacchineOperazionAgenda
    {
        public int ID { get; set; }
        public int status { get; set; }
        public string pivasuperuser { get; set; }
        public string piva { get; set; }
        public int cod_operazione { get; set; }
        public int cod_impianto { get; set; }
        public int id_agenda { get; set; }
        public string payload { get; set; }

    }
}
