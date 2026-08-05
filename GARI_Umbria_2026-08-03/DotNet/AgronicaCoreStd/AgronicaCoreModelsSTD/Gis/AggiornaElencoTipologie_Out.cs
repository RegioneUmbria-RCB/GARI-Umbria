using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Gis
{
    public class ElencoTipologieLayer
    {
        public List<TipologiaLayer> ListaTipologieLayer { get; set; }
    }

    public class TipologiaLayer
    {
        /// <summary>
        /// Nome layer
        /// </summary>
        public string nome { get; set; }

        /// <summary>
        /// ID da db
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// Colore primario in RGB
        /// </summary>
        public string colore_1 { get; set; }

        /// <summary>
        /// Colore secondario in RGB
        /// </summary>
        public string colore_2 { get; set; }

        /// <summary>
        /// Numero di intervalli in cui viene divisa la scala dei colori, calcolata a partire dal colore primario e secondario
        /// </summary>
        public string varianza { get; set; }

        /// <summary>
        /// Trasparenza : 0 = Trasparente, 1 = Pieno
        /// </summary>
        public string trasparenza { get; set; }

        /// <summary>
        /// Indica se la descrizione associata al layer è visibile : 1 = Visibile, 0 = Nascosta
        /// </summary>
        public string MostraDescrizioneAssociata { get; set; }

        /// <summary>
        /// Indica quale layer è in primo piano : Valore più alto = Primo piano
        /// </summary>
        public string zindex { get; set; }

        /// <summary>
        /// Nome file icona 16x16
        /// </summary>
        public string icona16 { get; set; }

        /// <summary>
        /// Nome file icona 32x32
        /// </summary>
        public string icona32 { get; set; }

        /// <summary>
        /// Tipo di nodo albero anagrafico
        /// </summary>
        public string TipoNodoAlberoAnagrafe { get; set; }

        /// <summary>
        /// Elenco tipologie tile
        /// </summary>
        public List<TipologiaTile> tiles { get; set; }

        /// <summary>
        /// Indica se il layer è visibile : 1 = Visibile, 0 = Nascosto
        /// </summary>
        public string flagvisibile { get; set; }

        /// <summary>
        /// Indica se il layer è attivo : 1 = Attivo, 0 = Non attivo
        /// </summary>
        public string flagattivo { get; set; }

        /// <summary>
        /// Indica se la descrizione associata al layer è da raggruppare : 1 = Sì, 0 = No
        /// </summary>
        public string RaggruppaDescrizioneAssociata { get; set; }

        //--------------------------------------------------------------------------------
        // Dati aggiuntivi letti da anagrafica layer
        //--------------------------------------------------------------------------------

        /// <summary>
        /// Indica se il layer consente l'inserimento : 1 = Sì, 0 = No
        /// </summary>
        public string FlagInserimento { get; set; }

        /// <summary>
        /// Indica se il layer consente la modifica : 1 = Sì, 0 = No
        /// </summary>
        public string FlagModifica { get; set; }

        /// <summary>
        /// Indica se il layer consente la cancellazione : 1 = Sì, 0 = No
        /// </summary>
        public string FlagCancellazione { get; set; }

        /// <summary>
        /// Indica se il layer consente la gestione degli attributi : 1 = Sì, 0 = No
        /// </summary>
        public string FlagInformazioni { get; set; }

        /// <summary>
        /// Indica se l'utente ha i permessi per amministrare il Layer : 1 = Sì, 0 = No (solo sui Layer custom >= 100000
        /// </summary>
        public string FlagAmministrazione { get; set; }

        /// <summary>
        /// Indica il tipo di feature del layer :
        /// 1 = Point,
        /// 2 = MultiPoint,
        /// 3 = LineString,
        /// 4 = MultiLineString,
        /// 5 = Polygon,
        /// 6 = MultiPolygon
        /// </summary>
        public string FeatureTypeId { get; set; }

        /// <summary>
        /// Elenco delle traduzioni del nome del layer
        /// </summary>
        public List<Gis_Traduzione> Traduzioni { get; set; }

    }

    public class TipologiaTile
    {
        public string nome { get; set; }
        public string id { get; set; }
        public string colore_primario { get; set; }
        public string colore_secondario { get; set; }
        public string varianza { get; set; }
        public string v_min { get; set; }
        public string v_max { get; set; }
        public string tilelayerpadre { get; set; }
        public List<TipologiaLabel> tilelabels { get; set; }
    }
    

}