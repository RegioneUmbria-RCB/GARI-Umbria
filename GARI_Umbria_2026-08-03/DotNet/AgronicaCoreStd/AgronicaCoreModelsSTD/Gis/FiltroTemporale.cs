using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Gis
{

    /// <summary>
    /// Classe filtro temporale
    /// </summary>   

    public class FiltroTemporale
    {

        public enum enum_TipoFiltroTemporale
        {
            ValidiAllaData = 1,
            ValidiSuAnnataAgrariaInCorso = 2,
            IntervalloTemporale = 3,
            EserciziValidiAllaData = 4
        }

        public enum enum_OperatoreFiltroTemporale
        {
            Precedente = 1,
            PrecedenteUguale = 2,
            Successivo = 3,
            SuccessivoUguale = 4
        }

        /// <summary>
        /// Indica il tipo di filtro temporale selezionato
        /// </summary>
        public enum_TipoFiltroTemporale TipoFiltroTemporale { get; set; } = enum_TipoFiltroTemporale.ValidiAllaData;

        /// <summary>
        /// Data inizio filtro temporale
        /// </summary>
        public DateTime DataInizio { get; set; }

        /// <summary>
        /// Tipo operaratore data inizio filtro temporale
        /// </summary>
        public enum_OperatoreFiltroTemporale TipoOperatoreDataInizio { get; set; } = enum_OperatoreFiltroTemporale.SuccessivoUguale;

        /// <summary>
        /// Data fine filtro temporale
        /// </summary>
        public DateTime DataFine { get; set; }

        /// <summary>
        /// Tipo operaratore data fine filtro temporale
        /// </summary>
        public enum_OperatoreFiltroTemporale TipoOperatoreDataFine { get; set; } = enum_OperatoreFiltroTemporale.PrecedenteUguale;

    }

    /// <summary>
    /// Classe filtro temporale avanzato: periodo e singola data
    /// </summary>
    public class FiltroTemporaleAvanzato
    {
        /// <summary>
        /// Filtro temporale periodo (es. IMPIANTI)
        /// </summary>
        public FiltroTemporale filtroTemporalePeriodo { get; set; }
        /// <summary>
        /// Filtro temporale singola data (es. OPERAZIONI)
        /// </summary>
        public FiltroTemporale filtroTemporaleSingolaData { get; set; }
    }

}