using System;

namespace AgronicaCoreDTOStd.InData.Agenda
{
    public class Ricetta_Operazione
    {

        public int ricetta_cod { get; set; }

        public int ricetta_operazione_cod { get; set; }

        public DateTime data { get; set; }

        public int in_uso { get; set; }

        public string app_ricetta_operazione_id { get; set; }
    }
}