using AgronicaCoreModelsSTD.attivita.risorse;
using System.Collections.Generic;
using System;
using AgronicaCoreModelsSTD.metaschema;
using Newtonsoft.Json;

namespace AgronicaCoreModelsSTD.attivita.dettagli
{

    public class DettaglioFertilizzazione : RisorsaProdotto
    {
        public decimal N { get; set; }
        public decimal P { get; set; }
        public decimal K { get; set; }
        public decimal Cu { get; set; }
        public decimal Mg { get; set; }

        public decimal efficienza { get; set; }

        public Effluente effluente { get; set; }

        public TipoFertilizzante tipoFertilizzante { get; set; }

        public List<TipologiaFertilizzante> tipologieFertilizzante { get; set; }

        public bool N_Ponderato { get; set; }
        public bool P_Ponderato { get; set; }
        public bool K_Ponderato { get; set; }
        public bool Cu_Ponderato { get; set; }


        public DettaglioFertilizzazione()
        {
            classType = costanti.ClassType.DettaglioFertilizzazione;
        }

        public new DettaglioFertilizzazione Clona()
        {
            try
            {

                string output = JsonConvert.SerializeObject(this);
                DettaglioFertilizzazione deserializedObject = JsonConvert.DeserializeObject<DettaglioFertilizzazione>(output);
                return deserializedObject;
            }
            catch (Exception ex)
            {
                throw new Exception("DettaglioFertilizzazione.Clona: " + ex.Message);
            }

        }

    }
}
