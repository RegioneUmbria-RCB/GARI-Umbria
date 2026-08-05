using AgronicaCoreModelsSTD.metaschema.utilizzi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgronicaCoreAPI.converters
{
    public class UtilizzoTerrenoConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(UtilizzoTerreno).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader,
            Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                JObject jo = JObject.Load(reader);
                string classType = (string)jo["classType"];

                object item;

                if (classType.Equals("Varieta"))
                {
                    item = new Varieta();
                }
                else if (classType.Equals("DestinazioneUso"))
                {
                    item = new DestinazioneUso();
                }
                else if (classType.Equals("Specie"))
                {
                    item = new Specie();
                }
                else
                {
                    return serializer.Deserialize(reader, objectType);
                }

                serializer.Populate(jo.CreateReader(), item);

                return item;
            }
            catch(Exception ex)
            {
                return null;
            }

        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer,
            object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
