using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Gis
{
    /// <summary>
    /// Dati salvataggio nuovo elemento grafico da chiave albero
    /// </summary>
    public class SalvaNuovoElementoGraficoDaChiaveAlberoConAppezza_In
    {
        /// <summary>
        /// Chiave albero elemento grafico
        /// </summary>
        public string ChiaveAlbero { get; set; }
        /// <summary>
        /// Geometria elemento grafico
        /// </summary>
        public string HiddenPuntiNuovo { get; set; }
        /// <summary>
        /// Area elemento grafico
        /// </summary>
        public string Area { get; set; }
        /// <summary>
        /// Indicativo GPS elemento grafico
        /// </summary>
        public string FlagGps { get; set; }
        /// <summary>
        /// Tipo operazione database elemento grafico: 0 = Lettura, 1 = Scrittura, 2 = Modifica, 3 = Cancellazione, ecc.
        /// (TipiEnumerativi.enum_TipoOperazioneDB: AgronicaCoreDataProvider/AgronicaCoreDataProviderSTD)
        /// </summary>
        public int TipoOperazioneDB { get; set; }
        /// <summary>
        /// Codice entità elemento grafico
        /// </summary>
        public int EntitaCod { get; set; }
    }
}
