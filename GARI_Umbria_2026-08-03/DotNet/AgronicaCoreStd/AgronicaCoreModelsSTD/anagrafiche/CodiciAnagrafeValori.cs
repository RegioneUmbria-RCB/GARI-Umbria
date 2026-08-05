using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    public class CodiciAnagrafeValori
    {
        public CodiceAnagrafe codiceAnagrafe { get; set; }

        public string valore { get; set; }

        public IntervalloTemporale validita { get; set; }

        public CodiciAnagrafeValori()
        {
        }
    }
}
