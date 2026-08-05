using AgronicaCoreModelsSTD.attivita.risorse;
using AgronicaCoreModelsSTD.metaschema.utilizzi;
using System;
using Newtonsoft.Json;

namespace AgronicaCoreModelsSTD.attivita.dettagli
{
    public class DettaglioSemina : RisorsaProdotto
    {
        public Varieta varieta { get; set; }
        public string codArticolo { get; set; }
        public int regolamento { get; set; }

        public DettaglioSemina()
        {
            classType = costanti.ClassType.DettaglioSemina;
        }
        public new DettaglioSemina Clona()
        {
            try
            {

                string output = JsonConvert.SerializeObject(this);
                DettaglioSemina deserializedObject = JsonConvert.DeserializeObject<DettaglioSemina>(output);
                return deserializedObject;
            }
            catch (Exception ex)
            {
                throw new Exception("DettaglioSemina.Clona: " + ex.Message);
            }

        }
    }
}
