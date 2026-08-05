using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AgronicaCoreModelsSTD.attivita
{
    [JsonConverter(typeof(JobClassConverter))]
    public abstract class Job
    {
        public PK primaryKey { get; set; }
        // public string classType;
        
        /// <summary>
        /// Lav_des
        /// </summary>
        public string descrizione { get; set; }

        public TipiJob tipo { get; set; }

        public TipiJob getTipo()
        {
            return tipo;
        }

        public int getCodice()
        {
            return int.Parse(primaryKey.codice);
        }

        public class PK
        {
            public string classType { get; set; }

            /// <summary>
            /// lav_cod da tabella Operazioni se Campagna (TODO: da verificare per CdG)
            /// </summary>
            public string codice { get; set; }

            public PK(string classType, string codice)
            {
                this.classType = classType;
                this.codice = codice;
            }

            public PK()
            {

            }
        }

    }

    public class JobClassConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Job);
        }

        public override bool CanWrite { get { return false; } }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try {
                if ((reader != null) && (reader.TokenType == JsonToken.Null)) { return null; }
                JObject jo = JObject.Load(reader);
                string classType = (string)jo.SelectToken("primaryKey.classType");

                object item;

                if (classType.Equals(costanti.ClassType.Lavorazione))
                {
                    item = new Lavorazione();
                }
                else if (classType.Equals(costanti.ClassType.AttivitaCDG))
                {
                    item = new AttivitaCDG();
                }
                else if (classType.Equals(costanti.ClassType.JobComposito))
                {
                    item = new JobComposito();
                }
                else if (classType.Equals(costanti.ClassType.Zootecnia))
                {
                    item = new Zootecnia();
                }
                else if (classType.Equals(costanti.ClassType.Registrazione))
                {
                    item = new Registrazione();
                }
                else
                {
                    return serializer.Deserialize(reader, objectType);
                }

                serializer.Populate(jo.CreateReader(), item);

                return item;
            }
            catch (Exception)
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
