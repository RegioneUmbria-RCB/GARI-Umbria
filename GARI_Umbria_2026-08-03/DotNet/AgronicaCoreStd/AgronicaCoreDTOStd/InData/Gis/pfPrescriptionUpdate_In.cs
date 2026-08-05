using AgronicaCoreModelsSTD.Gis;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Gis
{
    public class pfPrescriptionUpdate_In
    {
        public int allegati_documenti_cod { get; set; }
        public string jsonMap { get; set; }
        
        public string appIdRateRef { get; set; }

        public pfPrescriptionUpdate_In() { }

        public pfPrescriptionUpdate_In(int allegati_documenti_cod, string jsonMap)
        {
            this.allegati_documenti_cod = allegati_documenti_cod;
            this.jsonMap = jsonMap;
        }
    }
}
