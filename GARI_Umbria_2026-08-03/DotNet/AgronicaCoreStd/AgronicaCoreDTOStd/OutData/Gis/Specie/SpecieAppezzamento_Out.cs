namespace AgronicaCoreDTOStd.OutData.Gis.Specie
{
    /// <summary>
    /// Risultato dell'estrazione delle specie colturali degli anni precedenti per un appezzamento.
    /// </summary>
    public class SpecieAppezzamento_Out
    {
        /// <summary>Partita IVA dell'impresa.</summary>
        public string Piva { get; set; }

        /// <summary>Codice del centro aziendale.</summary>
        public int SaCod { get; set; }

        /// <summary>Codice dell'appezzamento.</summary>
        public int Appezza { get; set; }

        /// <summary>
        /// Identificativo della specie coltivata nell'anno corrente -1.
        /// null se non presente.
        /// </summary>
        public int? VegCod1 { get; set; }

        /// <summary>
        /// Identificativo della specie coltivata nell'anno corrente -2.
        /// null se non presente.
        /// </summary>
        public int? VegCod2 { get; set; }

        /// <summary>
        /// Identificativo della specie coltivata nell'anno corrente -3.
        /// null se non presente.
        /// </summary>
        public int? VegCod3 { get; set; }
    }
}
