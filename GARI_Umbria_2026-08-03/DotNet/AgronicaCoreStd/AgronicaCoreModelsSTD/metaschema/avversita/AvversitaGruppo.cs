using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreModelsSTD.attivita.dettagli;
using AgronicaCoreModelsSTD.baseClass;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace AgronicaCoreModelsSTD.metaschema.avversita
{
    [JsonConverter(typeof(AvversitaGruppoClassConverter))]
    public abstract class AvversitaGruppo : BaseCodeDescr
    {
        public string classType { get; set; }

        public int For_Veg_Av_Cod { get; set; }
        public int formulatiXAllegatiNormative_IDRiga { get; set; }
        public DateTime dataSmaltimentoScorte { get; set; }

        //Movimento_Dettaglio_Tecnico.Sigla_av
        public string abbreviazione { get; set; }

        public List<RilevamentoDiMagazzino> MagazziniMovimentazioni { get; set; }

        public AvversitaGruppo(int code) : base(code, "") {
            MagazziniMovimentazioni = new List<RilevamentoDiMagazzino>();
        }

        public AvversitaGruppo() : base() {
            MagazziniMovimentazioni = new List<RilevamentoDiMagazzino>();
        }

        public new AvversitaGruppo Clona()
        {
            try
            {

                string output = JsonConvert.SerializeObject(this);
                AvversitaGruppo deserializedObject = JsonConvert.DeserializeObject<AvversitaGruppo>(output);
                return deserializedObject;
            }
            catch (Exception ex)
            {
                throw new Exception("AvversitaGruppo.Clona: " + ex.Message);
            }

        }
    }

    public class AvversitaGruppoClassConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(AvversitaGruppo);
        }

        public override bool CanWrite { get { return false; } }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                if ((reader != null) && (reader.TokenType == JsonToken.Null)) { return null; }
                JObject jo = JObject.Load(reader);
                string classType = (string)jo.SelectToken("classType");

                object item;

                if (classType.Equals(costanti.ClassType.Avversita))
                {
                    item = new Avversita(0);
                }
                else if (classType.Equals(costanti.ClassType.GruppoAvversita))
                {
                    item = new GruppoAvversita(0);
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
