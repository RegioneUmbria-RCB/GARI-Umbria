using AgronicaCoreModelsSTD.attivita.dettagli;
using AgronicaCoreModelsSTD.metaschema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.attivita.risorse
{
    [JsonConverter(typeof(RisorsaClassConverter))]
    public class Risorsa
    {
        // public int Id { get; set; }
        public string classType { get; set; }
        
    }

    public class RisorsaClassConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Risorsa);
        }

        public override bool CanWrite { get { return false; } }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if ((reader != null) && (reader.TokenType == JsonToken.Null)) { return null; }
            JObject jo = JObject.Load(reader);
            string classType = (string)jo.SelectToken("classType");

            object item;

            if (classType.Equals(costanti.ClassType.DettaglioRilievo))
            {
                item = new DettaglioRilievo();
            }
            else if (classType.Equals(costanti.ClassType.DettaglioTrattamento))
            {
                item = new DettaglioTrattamento();
            }
            else if (classType.Equals(costanti.ClassType.DettaglioFertilizzazione))
            {
                item = new DettaglioFertilizzazione();
            }
            else if (classType.Equals(costanti.ClassType.DettaglioSemina))
            {
                item = new DettaglioSemina();
            }
            else if (classType.Equals(costanti.ClassType.DettaglioRaccolta))
            {
                item = new DettaglioRaccolta();
            }
            else if (classType.Equals(costanti.ClassType.DettaglioIrrigazione))
            {
                item = new DettaglioIrrigazione();
            }
            else if (classType.Equals(costanti.ClassType.RisorsaAcqua))
            {
                item = new RisorsaAcqua();
            }
            else if (classType.Equals(costanti.ClassType.RisorsaProdotto))
            {
                item = new RisorsaProdotto();
            }
            else if (classType.Equals(costanti.ClassType.RisorsaPersona))
            {
                item = new RisorsaPersona();
            }
            else if (classType.Equals(costanti.ClassType.RisorsaMacchina))
            {
                item = new RisorsaMacchina();
            }
            else if (classType.Equals(costanti.ClassType.RisorsaSpecie))
            {
                item = new RisorsaSpecie();
            }
            else if (classType.Equals(costanti.ClassType.RisorsaDestinazioneUso))
            {
                item = new RisorsaDestinazioneUso();
            }
            else if (classType.Equals(costanti.ClassType.RisorsaAssegnatarioVisita))
            {
                item = new RisorsaAssegnatarioVisita();
            }
            else if (classType.Equals(costanti.ClassType.RisorsaZootecnica))
            {
                item = new RisorsaZootecnica();
            }
            else if (classType.Equals(costanti.ClassType.RisorsaRegistrazione))
            {
                item = new RisorsaRegistrazione();
            }
            else if (classType.Equals(costanti.ClassType.RisorsaCausale))
            {
                item = new RisorsaCausale();
            }
            else if (classType.Equals(costanti.ClassType.DettaglioRegistroSomministrazioni))
            {
                item = new DettaglioRegistroSomministrazioni();
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
