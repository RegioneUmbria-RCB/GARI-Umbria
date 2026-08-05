using AgronicaCoreModelsSTD.baseClass;
using AgronicaCoreModelsSTD.metaschema;
using AgronicaCoreModelsSTD.metaschema.avversita;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AgronicaCoreModelsSTD.attivita.dettagli
{
    public class AvversitaTrappole: BaseCodeDescr
    {
        public AvversitaGruppo avversitaGruppo { get; set; }

        public UnitaDiMisura unitaDiMisura { get; set; }

        public List<UtilizzoAvversitaTrappole> utilizzi { get; set; }

        public new AvversitaTrappole Clona()
        {
            try
            {

                string output = JsonConvert.SerializeObject(this);
                AvversitaTrappole deserializedObject = JsonConvert.DeserializeObject<AvversitaTrappole>(output);
                return deserializedObject;
            }
            catch (Exception ex)
            {
                throw new Exception("AvversitaTrappole.Clona: " + ex.Message);
            }

        }

        public AvversitaTrappole(int code, string description) : base(code,description)
        {
            utilizzi = new List<UtilizzoAvversitaTrappole>();
        }
    }
}
