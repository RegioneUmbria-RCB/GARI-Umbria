using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Gis
{
    /// <summary>
    /// Parametri salvataggio layer personalizzato con attributi
    /// </summary>
    public class SalvaEntitaConAttributi_In
    {
        /// <summary>
        /// Codice entità da modificare.
        /// Se passato = 0, trattasi di entità da inserire.
        /// </summary>
        public int EntitaCod { get; set; }
        /// <summary>
        /// Partita iva
        /// </summary>
        public string Piva { get; set; }
        /// <summary>
        /// Centro aziendale
        /// </summary>
        public int SaCod { get; set; }
        /// <summary>
        /// Codice layer
        /// </summary>
        public int LayerElementiGraficiCod { get; set; }
        /// <summary>
        /// Elenco attributi in formato compresso
        /// </summary>
        public string ElementoGraficoDes { get; set; } 
        
        /// <summary>
        /// Elenco attributi (questo va a sostituire il formato "chiave1§valore1|chiave2§valore2"
        /// </summary>
        public AttributoEntita_In[] Attributi { get; set; }
        /// <summary>
        /// Dati cartografici
        /// </summary>
        public string Cartografia{ get; set; }
        /// <summary>
        /// Indica se la cartografica è stata acquisita tramite GPS: 1 = Sì, 0 = No.
        /// </summary>
        public int FlagGps { get; set; }
        /// <summary>
        /// Codice analisi campione.
        /// Significativo solo in caso di inserimento.
        /// </summary>
        public int AnalisiCampioneCod { get; set; }
        /// <summary>
        /// Codice fabbricato.
        /// Significativo solo in caso di inserimento.
        /// </summary>
        public int FabbricatoCod { get; set; }
    }
}


public class AttributoEntita_In
{
    public int TipologiaLayerDataCod { get; set; }
    public int TipologiaLayerstructcod { get; set; }
    public string Valore { get; set; }
}