using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaCoreDTOStd.InData.ConfrontoCatasto
{
    public enum enum_TipoConfrontoCatasto
    {
        Planning_PianoColturale = 0,
        PianoColturale_Planning = 1
    }

    public class ParametriTipoConfronto
    {
        public int tipoConfronto { get; set; }
        public string partitaIva { get; set; }
        public int programmazioneCod { get; set; }
        public DateTime dataInizio { get; set; }
        public DateTime dataFine { get; set; }
        public bool showCatasto { get; set; }
        public bool showVarieta { get; set; }
    }
}
