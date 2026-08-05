namespace AgronicaCoreDTOStd.InData.Gis
{
    /// <summary>
    /// Parametri per l'aggiornamento dei collegamenti tra elemento grafico e tipologia oggetto GIS
    /// </summary>
    public class AggiornaElementoGraficoPerTipoOggetto_In : ElementoGraficoPerTipoOggetto_In
    {

        /// <summary>
        /// Codice tipo oggetto GIS precedente
        /// </summary>
        /// <example>1234</example>
        public int GIS_TipoOggetto_Cod_Prev { get; set; }

    }
}
