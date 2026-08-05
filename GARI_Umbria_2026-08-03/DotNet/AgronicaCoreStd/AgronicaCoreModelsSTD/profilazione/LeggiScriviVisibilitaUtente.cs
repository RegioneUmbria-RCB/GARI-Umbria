using AgronicaCoreModelsSTD.anagrafiche;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.profilazione
{

    public class LeggiScriviVisibilitaUtente
    {
        public UtenteDTO Utente { get; set; }
        public List<ImpresaDto> AziendeVisibili { get; set; } = new List<ImpresaDto>();
        public bool Sovrascrivi { get; set; }
    }

    public class LeggiScriviVisibilitaUtenti
    {
        public List<UtenteDTO> Utenti { get; set; }
        public List<ImpresaDto> AziendeVisibili { get; set; } = new List<ImpresaDto>();
        /// <summary>
        /// Usato in scrittura per determinare se sovrascrivere la visibilità
        /// degli utenti o limitarsi ad aggiungere le imprese alla visibilità
        /// già esistente, in lettura per determinare se leggere i dettagli
        /// degli utenti (nome e cognome) o limitarsi a caricarne la visibilità.
        /// </summary>
        public bool Sovrascrivi { get; set; }
        public bool LeggiDatiImprese { get; set; }
    }

    public class RimuoviVisibilitaUtente
    {
        public UtenteDTO Utente { get; set; }
        public string Piva { get; set; }
    }
}
