using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Gis
{
    public class SementiMappaturaLibera_SpecieVegPermessa_In
    {
        public int Veg_Cod { get; set; }
        public int Finalita { get; set; }
        public string SementiMappaturaLibera { get; set; }

        //Al momento (compresa la funzione originale) data_inizio e data_fine non vengono usate mai (per la funzione in cui compare questa classe)
        //public DateTime Data_Inizio { get; set; }
        //public DateTime Data_Fine { get; set; }
    }
}
