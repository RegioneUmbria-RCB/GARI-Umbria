using System;
using System.Numerics;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.metaschema.utilizzi;
using static AgronicaCoreModelsSTD.anagrafiche.CentroAziendale;

namespace InData.Agenda
{
    /// <summary>
    /// Classe contenente i parametri di input per la ricerca delle statistiche del quaderno per la dashboard
    /// </summary>
    public class LeggiAgendaStatistiche
    {
        /// <summary>
        /// Lista delle P.IVA delle aziende da includere nella ricerca, se vuoto le seleziona tutte
        /// </summary>
         public string[] Piva_aziende { get; set; }

        /// <summary>
        /// Lista delle chiavi primarie dei centri aziendali da coinvoglere nella ricerca (sa_cod e P.IVA)
        /// </summary>
        public PK[] Centri_aziendali { get; set; }

        /// <summary>
        /// Lista di specie vegetali che saranno incluse nella ricerca, se vuoto le seleziona tutte
        /// </summary>
        public Specie[] Specie_vegetale { get; set; }

        /// <summary>
        /// Data meno recente del periodo in cui verrà effettuata la ricerca
        /// </summary>
        public DateTime Data_inizio_operazione { get; set; } = new DateTime(1900, 1, 1);

        /// <summary>
        /// Data più recente del periodo in cui verrà effettuata la ricerca
        /// </summary>
        public DateTime Data_fine_operazione { get; set; } = new DateTime(2100, 12, 31);

        /// <summary>
        /// Data meno recente del periodo in cui verrà effettuata la ricerca
        /// </summary>
        public DateTime Data_inizio_impianto { get; set; } = new DateTime(1900, 1, 1);

        /// <summary>
        /// Data più recente del periodo in cui verrà effettuata la ricerca
        /// </summary>
        public DateTime Data_fine_impianto { get; set; } = new DateTime(2100, 12, 31);

        /// <summary>
        /// Giorni di differenza rispetto ad oggi che compongono il periodo su cui sarà effettuata la ricerca
        /// </summary>
        public int Periodo_da_data_Impianto { get; set; }

        /// <summary>
        /// Giorni di differenza rispetto ad oggi che compongono il periodo su cui sarà effettuata la ricerca
        /// </summary>
        public int Periodo_da_data_operazione { get; set; }

        /// <summary>
        /// Tipi di operazioni da includere nella ricerca, se vuoto li seleziona tutti
        /// </summary>
        public int[] Tipo_operazioni { get; set; }

        /// <summary>
        /// Operazioni da includere nella ricerca, se vuoto le seleziona tutte
        /// </summary>
        public int[] Operazioni { get; set; }

        /// <summary>
        /// Nazioni da includere nella ricerca, se vuoto le seleziona tutte
        /// </summary>
        public string[] Nazioni { get; set; }

        /// <summary>
        /// Regioni da includere nella ricerca, se vuoto le seleziona tutte
        /// </summary>
        public string[] Regioni { get; set; }

        /// <summary>
        /// Province da includere nella ricerca, se vuoto le seleziona tutte
        /// </summary>
        public string[] Province { get; set; }

        /// <summary>
        /// Comuni da includere nella ricerca, se vuoto le seleziona tutte
        /// </summary>
        public string[] Comuni { get; set; }

        /// <summary>
        /// Filtro per visualizzare o no gli impianti nel risultato finale
        /// </summary>
        public Boolean Impianti { get; set; }

        /// <summary>
        /// Filtro per visualizzare o no i prodotti nel risultato finale
        /// </summary>
        public Boolean Prodotti { get; set; }

        /// <summary>
        /// Filtro per visualizzare o no le operazioni colturali nel risultato finale
        /// </summary>
        public Boolean OperazioniColt { get; set; }

        public int Estrazione { get; set; }

        /// <summary>
        /// Filtro per visualizzare o no i dati tecnici nel risultato finale
        /// </summary>
        public Boolean Dati_tecnici { get; set; }

        /// <summary>
        /// Lista delle P.IVA delle aziende refernti su cui filtrare, se vuoto le seleziona tutte
        /// </summary>
        public string[] Aziende_referenti { get; set; }

        public Tuple<string, string>[] parametriAggiuntivi { get; set; }

    }
}

