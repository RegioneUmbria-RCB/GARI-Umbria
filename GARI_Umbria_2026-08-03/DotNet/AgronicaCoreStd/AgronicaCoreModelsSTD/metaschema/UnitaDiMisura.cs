using AgronicaCoreModelsSTD.baseClass;
using Newtonsoft.Json;
using System;

namespace AgronicaCoreModelsSTD.metaschema
{
    public class UnitaDiMisura : BaseCodeDescr
    {

        public string simbolo { get; set; }

        public BaseCodeDescr tipoControllo { get; set; }

        public UnitaDiMisura(int codice) : base(codice, "")
        {
            this.tipoControllo = new BaseCodeDescr(0, "");
            this.simbolo = "";
        }

        public UnitaDiMisura() : base(-1, "")
        {
        }
        public UnitaDiMisura Clona()
        {
            try
            {

                string output = JsonConvert.SerializeObject(this);
                UnitaDiMisura deserializedObject = JsonConvert.DeserializeObject<UnitaDiMisura>(output);
                return deserializedObject;
            }
            catch (Exception ex)
            {
                throw new Exception("UnitaDiMisura.Clona: " + ex.Message);
            }

        }
    }

}
