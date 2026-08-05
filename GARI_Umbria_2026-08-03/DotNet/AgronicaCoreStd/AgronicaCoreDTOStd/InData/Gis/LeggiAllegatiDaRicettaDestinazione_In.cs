namespace AgronicaCoreDTOStd.InData.Gis
{
    /// <summary>
    /// Parametri per la lettura degli allegati da Ricetta Destinazione
    /// </summary>
    public class LeggiAllegatiDaRicettaDestinazione_In
    {

        /// <summary>
        /// campo Piva della tabella Alert_Entita
        /// </summary>
        /// <example>012345678911</example>
        public string Piva { get; set; }

        /// <summary>
        /// campo Sa_Cod della tabella Alert_Entita
        /// </summary>
        /// <example>1</example>
        public int Sa_Cod { get; set; }

        /// <summary>
        /// campo Appezza della tabella Alert_Entita
        /// </summary>
        /// <example>1</example>
        public int Appezza { get; set; }

        /// <summary>
        /// Id impianto
        /// </summary>
        /// <example>1</example>
        public int Id_Imp { get; set; }

        /// <summary>
        /// Codice Ricetta Destinazione
        /// </summary>
        /// <example>1</example>
        public int Ricetta_Operazione_Cod { get; set; }

        /// <summary>
        /// campo Allegati_Documenti_CatCod della tabella Allegati_Documenti
        /// </summary>
        /// <example>1</example>
        public int Allegati_Documenti_CatCod { get; set; }
    }
}
