using System;

namespace AgronicaCoreDTOStd.OutData.Pratiche
{
    /// <summary>
    /// Pratica filtrata arricchita con i dati del catalogo (Servizi_Pratiche).
    /// Usata nella response GET del profilo utente filtri pratiche.
    /// </summary>
    public class PraticaFiltrataDettaglio_OUT
    {
        /// <summary>Codice servizio (Servizi.Servizio_Cod).</summary>
        public int Servizio_Cod { get; set; }

        /// <summary>Descrizione del servizio dal catalogo.</summary>
        public string ServizioDescrizione { get; set; } = string.Empty;

        /// <summary>Se true, applica anche il filtro sulla validità temporale della pratica.</summary>
        public bool ConsideraValiditaTemporale { get; set; }

        /// <summary>Data inizio validità del servizio nel catalogo.</summary>
        public DateTime? DataValiditaInizio { get; set; }

        /// <summary>Data fine validità del servizio nel catalogo.</summary>
        public DateTime? DataValiditaFine { get; set; }

        /// <summary>True se il servizio è attualmente valido (Validita_Fine >= oggi).</summary>
        public bool IsValida { get; set; }
    }
}
