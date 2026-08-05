using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.profilazione
{
    /// <summary>
    /// Definisce la rappresentazione di impostazione facendo riferimento ai campi
    /// delle tabelle SQL Guida_Impostazioni_Valori e Guida_Impostazioni.
    /// </summary>
    public class Impostazione
    {
        public string Data_Creazione { get; set; }
        public string Data_Modifica{ get; set; }
        public string Data_Invio{ get; set; }
    
        public string Impostazione_AziendaCentro{ get; set; }
        public string Impostazione_AziendaCentroSpecie{ get; set; }
    
        public int Impostazione_Cod{ get; set; }
        public string Impostazione_Des{ get; set; }
        public string Impostazione_SuperUser{ get; set; }
    
        public int Inviato{ get; set; }
        public string Note{ get; set; }
        public int Ordine{ get; set; }
        public string Tipo_Campo{ get; set; }
    
        public int Sezione_Cod{ get; set; }
        public string Sezione_Des{ get; set; }
        public int SottoSezione_Cod{ get; set; }
        public string SottoSezione_Des{ get; set; }
        public int Livello_Cod{ get; set; }
        public string Livello_Des{ get; set; }
    
        public string Username_Creazione{ get; set; }
        public string Username_Modifica{ get; set; }
    
        public string Validita_Inizio{ get; set; }
        public string Validita_Fine{ get; set; }

        public string utente_interessato{ get; set; }
        public string codice{ get; set; }
        public string descrizione{ get; set; }
        public string value{ get; set; }


    }
}
