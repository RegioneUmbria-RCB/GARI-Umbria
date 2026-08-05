using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreModelsSTD.attivita.centri_di_costo;
using AgronicaCoreModelsSTD.attivita.dettagli;
using AgronicaCoreModelsSTD.attivita.risorse;
using AgronicaCoreModelsSTD.metaschema.avversita;
using AgronicaCoreModelsSTD.metaschema.utilizzi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgronicaCoreAPI.converters
{
    public class AttivitaConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(Job).IsAssignableFrom(objectType) 
                || typeof(CentroDiCosto).IsAssignableFrom(objectType) 
                || typeof(Risorsa).IsAssignableFrom(objectType)
                || typeof(AvversitaGruppo).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader,
            Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            string classType = (string)jo["classType"];

            object item;

            if (classType.Equals("Lavorazione"))
            {
                item = new Lavorazione();
            }
            else if (classType.Equals("AttivitaCDG"))
            {
                item = new AttivitaCDG();
            }
            else if (classType.Equals("Progetto"))
            {
                item = new Progetto();
            }
            else if (classType.Equals("Macchina"))
            {
                item = new Macchina();
            }
            else if (classType.Equals("EsercizioCDC"))
            {
                item = new EsercizioCDC();
            }
            else if (classType.Equals("EsercizioRilievoCDC"))
            {
                item = new EsercizioRilievoCDC();
            }
            else if (classType.Equals("DettaglioRilievo"))
            {
                item = new DettaglioRilievo();
            }
            else if (classType.Equals("DettaglioTrattamento"))
            {
                item = new DettaglioTrattamento();
            }
            else if (classType.Equals("DettaglioFertilizzazione"))
            {
                item = new DettaglioFertilizzazione();
            }
            else if (classType.Equals("RisorsaAcqua"))
            {
                item = new RisorsaAcqua();
            }
            else if (classType.Equals("RisorsaProdotto"))
            {
                item = new RisorsaProdotto();
            }
            else if (classType.Equals("RisorsaPersona"))
            {
                item = new RisorsaPersona();
            }
            else if (classType.Equals("RisorsaMacchina"))
            {
                item = new RisorsaMacchina();
            }
            else if (classType.Equals("Avversita"))
            {
                item = new Avversita(0);
            }
            else if (classType.Equals("GruppoAvversita"))
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
