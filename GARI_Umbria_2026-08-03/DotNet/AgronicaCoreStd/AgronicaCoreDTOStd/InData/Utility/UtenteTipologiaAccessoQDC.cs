using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Utility
{
    public class UtenteTipologiaAccessoQDC
    {
        public string piva { get; set; }

        public DateTime data { get; set; }
    }

    public class ConfigurazioneControlloServizioQDC<T>
    {
        public int tipo { get; set; }
        public T pars { get; set; }
    }

    public class ParametriControlloServizioQDCColdiretti
    {
        public int codiceServizio { get; set; }
        public List<int> statiAmmessi { get; set; }
    }
}

