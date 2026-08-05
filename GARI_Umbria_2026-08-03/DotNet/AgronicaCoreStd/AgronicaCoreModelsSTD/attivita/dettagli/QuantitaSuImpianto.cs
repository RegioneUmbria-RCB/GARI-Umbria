using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.attivita.centri_di_costo;
using AgronicaCoreModelsSTD.attivita.risorse;
using AgronicaCoreModelsSTD.metaschema.utilizzi;
using Newtonsoft.Json;
using System;

namespace AgronicaCoreModelsSTD.attivita.dettagli
{
    public class QuantitaSuImpianto
    {
        public decimal Qta { get; set; }
        public EsercizioCDC esercizioCDC { get; set; }
        public Fabbricato Magazzino { get; set; }
        public string Lotto { get; set; }
        public Prodotto Prodotto { get; set; }

        public QuantitaSuImpianto()
        {
            Lotto = "";
        }

        public QuantitaSuImpianto Clone()
        {
            try
            {
                string output = JsonConvert.SerializeObject(this);
                QuantitaSuImpianto deserializedObject = JsonConvert.DeserializeObject<QuantitaSuImpianto>(output);
                return deserializedObject;
            }
            catch (Exception ex)
            {
                throw new Exception("QuantitaSuImpianto.Clona: " + ex.Message);
            }
        }
    }
}
