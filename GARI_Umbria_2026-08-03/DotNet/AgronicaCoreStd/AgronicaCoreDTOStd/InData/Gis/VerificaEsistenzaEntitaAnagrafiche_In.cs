namespace AgronicaCoreDTOStd.InData.Gis
{
    /// <summary>
    /// Parametri per la verifica dell'esistenza di appezzamenti ed imprese
    /// </summary>
    public class VerificaEsistenzaEntitaAnagrafiche_In
    {

        /// <summary>
        /// Partita Iva Impianto
        /// </summary>
        /// <example>012345678911</example>
        public string Piva { get; set; }

        /// <summary>
        /// Sa_Cod Impianto
        /// </summary>
        /// <example>1</example>
        public int Sa_Cod { get; set; }

        /// <summary>
        /// Appezza Impianto
        /// </summary>
        /// <example>1</example>
        public int Appezza { get; set; }

        /// <summary>
        /// Id impianto
        /// </summary>
        /// <example>1</example>
        public int Id_Reg { get; set; }
    }
}
