using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Gis
{
    /// <summary>
    /// questa classe è una copia di AgronicaCoreModello.PrecisionModel_CFGLetturaDati
    /// poichè non era possibile utilizzarla
    /// </summary>
    public class Lista_DatiPrecision_XmlAllegati_Out
    {
        public List<int> ListaCodici_RicetteOperazioniCod_MappePrescrizione { get; set; }
        public List<int> ListaCodici_RicetteOperazioniCod_MappeProduzione { get; set; }
        public List<int> ListaCodici_AllegatiDocumentiCod_MappePrescrizione { get; set; }
        public List<int> ListaCodici_AllegatiDocumentiCod_MappeProduzione { get; set; }
    }
}
