using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Gis
{
    public class ElencoAlgoritmi
    {
        public List<AlgoritmoProiezione> Algoritmi { get; set; }
    }

    public class AlgoritmoProiezione
    {
        public int Algoritmo_Cod { get; set; }
        public string Algoritmo_Des { get; set; }
        public List<ParametriAlgoritmoProiezione> Parametri { get; set; }
    }

    public class ParametriAlgoritmoProiezione
    {
        public int Parametro_Cod { get; set; }
        public string Parametro_Des { get; set; }
        public int tipo_Parametro { get; set; }
    }
}
