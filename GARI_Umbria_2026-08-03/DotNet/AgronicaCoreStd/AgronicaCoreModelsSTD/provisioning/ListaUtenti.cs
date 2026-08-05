using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.provisioning
{
    /// <summary>
    /// Classe lista dati base utente
    /// </summary>
    public class ListaUtenti
    {
        /// <summary>
        /// Lista dati base utente
        /// </summary>
        public List<DatiBaseUtente> ListaDatiBaseUtente { get; set; }
    }

    /// <summary>
    /// Dati base utente
    /// </summary>
    public class DatiBaseUtente

    {
        /// <summary>
        /// Username utente
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Cognome utente
        /// </summary>
        public string Cognome { get; set; }
        /// <summary>
        /// Nome utente
        /// </summary>
        public string Nome { get; set; }
        /// <summary>
        /// Ragiona sociale utente
        /// </summary>
        public string RagioneSociale { get; set; }
        /// <summary>
        /// Indica se l'utente è un'azienda o una persona: 1 = Azienda, 2 = Persona.
        /// </summary>
        public int Flag_Azienda_Persona { get; set; }
        /// <summary>
        /// Descrizione composta in funzione del flag azienda/persona
        /// </summary>
        public string Descrizione { get; set; }
    }
}
