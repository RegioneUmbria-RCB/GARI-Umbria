using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AgronicaCoreModelsSTD.attivita
{
     public class AttivitaConParametriAggiuntivi: Attivita
    {
        public List<Parametri_Aggiuntivi_Attivita> parametri_aggiuntivi { get; set; }

        public AttivitaConParametriAggiuntivi()
        {
            parametri_aggiuntivi = new List<Parametri_Aggiuntivi_Attivita>();
        }

        public static AttivitaConParametriAggiuntivi Create(Attivita attivita, List<Parametri_Aggiuntivi_Attivita> parametriAggiuntivi)
        {
            try
            {

                string output = JsonConvert.SerializeObject(attivita);
                AttivitaConParametriAggiuntivi deserializedObject = JsonConvert.DeserializeObject<AttivitaConParametriAggiuntivi>(output);

                deserializedObject.parametri_aggiuntivi = parametriAggiuntivi;

                return deserializedObject;
            }
            catch (Exception ex)
            {
                throw new Exception("AttivitaConParametriAggiuntivi.Create: " + ex.Message);
            }            
        }
    }

}
