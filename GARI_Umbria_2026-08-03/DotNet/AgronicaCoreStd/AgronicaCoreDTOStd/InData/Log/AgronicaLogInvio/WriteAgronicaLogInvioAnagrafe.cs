using System;
using System.Collections.Generic;
using System.Text;

namespace InData.Log.AgronicaLogInvio
{
    /// <summary>
    /// Modello DTO per la scrittura di un nuovo record nella tabella di collegamento
    /// Agronica_Log_Invio_Anagrafe.
    /// </summary>
    public class WriteAgronicaLogInvioAnagrafe
    {
        /// <summary>
        /// L'ID della chiamata API (foreign key verso Agronica_Log_Invio_Chiamate).
        /// Verrà popolato dal servizio BIZ dopo aver creato il record 'Chiamate'.
        /// </summary>
        public int ID_Log_Invio { get; set; }

        /// <summary>
        /// Il codice che identifica il flusso di esportazione (es. 200 per Antares Materie Prime).
        /// </summary>
        public int Tipo_Esportazione { get; set; }

        /// <summary>
        /// Il tipo di anagrafica a cui si riferisce il log (es. "Materie_Prime", "ParcoMacchine").
        /// </summary>
        public string Tipo { get; set; }

        /// <summary>
        /// La chiave composita testuale dell'anagrafica (es. "PIVA_SACOD_...").
        /// </summary>
        public string Chiave { get; set; }

        /// <summary>
        /// La Partita IVA dell'azienda di riferimento.
        /// </summary>
        public string Piva { get; set; }

        /// <summary>
        /// Il codice del sito aziendale.
        /// </summary>
        public int Sa_Cod { get; set; }

        /// <summary>
        /// La chiave specifica della Materia Prima.
        /// </summary>
        public int? Mat_Cod { get; set; }

        // --- Altri campi presenti nella funzione VB, da aggiungere se necessari ---

        /// <summary>
        /// Codice del progetto.
        /// </summary>
        public int? Progetto_Cod { get; set; }

        /// <summary>
        /// Codice dell'appezzamento.
        /// </summary>
        public int? Appezza { get; set; }

        /// <summary>
        /// ID della registrazione.
        /// </summary>
        public int? Id_Reg { get; set; }

        /// <summary>
        /// Codice della macchina (se applicabile).
        /// </summary>
        public int? Mac_Cod { get; set; }

        // Aggiungi qui altre proprietà se la tabella Agronica_Log_Invio_Anagrafe ne ha bisogno

        public WriteAgronicaLogInvioAnagrafe()
        {
            // Inizializza i valori di default per evitare problemi con i null
            Tipo = string.Empty;
            Chiave = string.Empty;
            Piva = string.Empty;
        }
    }
}
