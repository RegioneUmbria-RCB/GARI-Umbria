using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AgronicaCoreModelsSTD.attivita.centri_di_costo
{
    [JsonConverter(typeof(CentroDiCostoClassConverter))]
    public class CentroDiCosto
    {
        // public int Id { get; set; }
        public string classType { get; set; }
        public CodeType codice { get; set; }
        public Tipo tipo { get; set; }

        public CentroDiCosto()
        {

        }
    
        public class CodeType
        {
            public int intValue { get; set; }
            public string stringValue { get; set; }
            public Type type { get; set; }
            public CodeType()
            {
            }
            public CodeType(int value)
            {
                intValue = value;
                type = Type.INT;
            }
            public CodeType(string value)
            {
                stringValue = value;
                type = Type.STRING;
            }
            public enum Type
            {
                INT,
                STRING
            }
        }
    }

    public enum Tipo
        {
            Macchina,
            Esercizio,
            Progetto,
            CapoAnimale,
            ProdottoDaTrattare
        }

    public class CentroDiCostoClassConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(CentroDiCosto);
        }

        public override bool CanWrite { get { return false; } }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if ((reader != null) && (reader.TokenType == JsonToken.Null)) { return null; }
            JObject jo = JObject.Load(reader);
            string classType = (string)jo.SelectToken("classType");

            object item;

            if (classType.Equals(costanti.ClassType.Progetto))
            {
                item = new Progetto();
            }
            else if (classType.Equals(costanti.ClassType.Macchina))
            {
                item = new Macchina();
            }
            else if (classType.Equals(costanti.ClassType.EsercizioCDC))
            {
                item = new EsercizioCDC();
            }
            else if (classType.Equals(costanti.ClassType.EsercizioRilievoCDC))
            {
                item = new EsercizioRilievoCDC();
            }
            else if (classType.Equals(costanti.ClassType.CapoAnimaleCDC))
            {
                item = new CapoAnimaleCDC();
            }
            else if (classType.Equals(costanti.ClassType.ProdottoDaTrattareCDC))
            {
                item = new ProdottoDaTrattareCDC();
            }
            else
            {
                return serializer.Deserialize(reader, objectType);
            }

            serializer.Populate(jo.CreateReader(), item);

            return item;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

}
