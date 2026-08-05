using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    public class GruppoRaccolta: baseClass.BaseCodeDescr
    {
        public IntervalloTemporale validita { get; set; }

        public GruppoRaccolta(int code, string descr): base(code, descr)
        {
        }

        public GruppoRaccolta() { }
    }
}
