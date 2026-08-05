using AgronicaCoreModelsSTD.anagrafiche;
using System;
using System.Collections.Generic;
using System.Text;

namespace InData.Anagrafica
{
    public class AppezzamentoXParcoMacchine
    {
        public string piva { get; set; }
        public int saCod { get; set; }
        public int appezza { get; set; }
        public int macCod { get; set; }
        public string classCode { get; set; }
        public IntervalloTemporale validity { get; set; }

        public AppezzamentoXParcoMacchine(
            string piva = "",
            int saCod = 0,
            int appezza = 0,
            int macCod = 0,
            string classcode = "",
            DateTime? startV = null,
            DateTime? endV = null
            )
        {
            this.piva = piva;
            this.saCod = saCod;
            this.appezza = appezza;
            this.macCod = macCod;
            this.classCode = classcode;
            this.validity = new IntervalloTemporale(
                startV.GetValueOrDefault(new DateTime(1900, 1, 1)),
                endV.GetValueOrDefault(new DateTime(2100, 12, 31))
                );
        }

        public AppezzamentoXParcoMacchine()
        {
            this.piva = "";
            this.saCod = 0;
            this.appezza = 0;
            this.macCod = 0;
            this.classCode = "";
            this.validity = new IntervalloTemporale(new DateTime(1900, 1, 1), new DateTime(2100, 12, 31));
        }
    }
}
