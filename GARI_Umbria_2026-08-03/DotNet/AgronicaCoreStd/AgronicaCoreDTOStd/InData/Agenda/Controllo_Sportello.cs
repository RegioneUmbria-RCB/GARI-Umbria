using System;

namespace AgronicaCoreDTOStd.InData.Agenda
{
    public class Controllo_Sportello
    {
        public string Piva { get; set; }

        public int Servizio_Cod { get; set; }

        public DateTime Data_Riferimento { get; set; }

        public Boolean SportelloAperto { get; set; }

        public DateTime Data_Min { get; set; }

        public DateTime Data_Max { get; set; }

    }
}