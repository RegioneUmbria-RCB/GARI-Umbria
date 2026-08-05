using System;

namespace InData.Log.AgronicaLogInvio
{

    /// <summary>
    /// Modello DTO per l'aggiornamento di un record esistente in Agronica_Log_Invio_Chiamate.
    /// Corrisponde alla logica della funzione VB "Update_Log_Invio_Chiamate".
    /// </summary>
    public class UpdateAgronicaLogInvioChiamate
    {
        public int ID { get; set; }
        public int Tipo_Esportazione { get; set; }
        public string Dati_Inviati { get; set; }
        public string Tipo_Operazione { get; set; }
        public string Esito { get; set; }
        public string Dati_Ricevuti { get; set; }
        public DateTime? Data_Invio { get; set; }
    }

}
