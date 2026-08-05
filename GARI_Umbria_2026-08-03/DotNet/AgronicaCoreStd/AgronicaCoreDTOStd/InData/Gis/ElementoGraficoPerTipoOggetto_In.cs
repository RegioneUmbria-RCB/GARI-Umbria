namespace AgronicaCoreDTOStd.InData.Gis
{
    /// <summary>
    /// Parametri per l'aggiornamento dei collegamenti tra elemento grafico e tipologia oggetto GIS
    /// </summary>
    public class ElementoGraficoPerTipoOggetto_In
    {
        /// <summary>
        /// Codice elemento grafico
        /// </summary>
        /// <example>1</example>
        public int LayerElementiGrafici_Cod { get; set; }

        /// <summary>
        /// Codice tipo oggetto GIS
        /// </summary>
        /// <example>1234</example>
        public int GIS_TipoOggetto_Cod { get; set; }

    }
}
