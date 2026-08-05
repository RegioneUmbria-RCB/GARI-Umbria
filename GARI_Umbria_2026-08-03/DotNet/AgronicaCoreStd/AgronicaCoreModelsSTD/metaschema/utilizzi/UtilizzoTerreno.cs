using AgronicaCoreModelsSTD.baseClass;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AgronicaCoreModelsSTD.metaschema.utilizzi
{
    [JsonConverter(typeof(UtilizzoTerrenoClassConverter))]
    public abstract class UtilizzoTerreno : BaseCodeDescr
    {
        public string classType { get; set; }

        public UtilizzoTerreno() : base() { }

        public UtilizzoTerreno(int codice) : base(codice, "") { }
    }

    public class UtilizzoTerrenoClassConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(UtilizzoTerreno);
        }

        public override bool CanWrite { get { return false; } }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                if ((reader!= null) && (reader.TokenType == JsonToken.Null)){ return null; }

                JObject jo = JObject.Load(reader);
                string classType = (string)jo["classType"];

                if (classType == null)
                    return null;

                object item;

                if (classType.Equals(costanti.ClassType.Varieta))
                {
                    item = new Varieta();
                }
                else if (classType.Equals(costanti.ClassType.DestinazioneUso))
                {
                    item = new DestinazioneUso();
                }
                else if (classType.Equals(costanti.ClassType.Specie))
                {
                    item = new Specie();
                }
                else
                {
                    return serializer.Deserialize(reader, objectType);
                }

                serializer.Populate(jo.CreateReader(), item);

                return item;
            }catch(Exception)
            {
                return null;
            }
            
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

}
