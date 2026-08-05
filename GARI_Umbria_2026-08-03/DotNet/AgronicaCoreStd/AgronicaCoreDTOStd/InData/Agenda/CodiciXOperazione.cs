using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.attivita;

namespace AgronicaCoreDTOStd.InData.Agenda
{
    public class CodiciXOperazione
    { 

        public string CodiceAttivita { get; set; }

        /// <summary>
        /// Ricette_Operazioni.Ricetta_Cod
        /// </summary>
        public string CodiceRicetta { get; set; }

        /// <summary>
        /// Ricette_Operazioni.Ricetta_Operazione_Cod
        /// </summary>
        public string CodiceOperazioneRicetta { get; set; }

        public Lavorazione Operazione { get; set; }

        public AssociazionePK Associazione_PK { get; set; }

        public CentroAziendale Centro_Aziendale { get; set; }

        public string APP_RicettaOperazione_ID { get; set; }

        public string CodiceAttivitaVisita { get; set; }

        public int CodicePUA { get; set; }

    }
}