namespace AgronicaCoreDTOStd.InData.Gis
{
    /// <summary>
    /// Parametri di filtro in lettura della lista dei layer e dei tiles
    /// </summary>
    public class LeggiImpostazioniAvanzateLayer_In
    {
        /// <summary>
        /// ID Layer desiderato
        /// </summary>
        /// <example>furno</example>
        public string IdLayer { get; set; }
    }
}
