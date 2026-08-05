using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Gis
{
    public class LeggiImpostazioniAvanzateLayer
    {
        /// <summary>
        /// ID del layer selezionato
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Indica se il layer è bloccato
        /// </summary>
        public string Bloccato { get; set; }

        /// <summary>
        /// Nome layer
        /// </summary>
        public string NomeLayer { get; set; }

        /// <summary>
        /// Tipologia di oggetto GIS
        /// </summary>
        public string TipoGIS { get; set; }

        /// <summary>
        /// Lista di attributi del layer selezionato
        /// </summary>
        public List<AttributoLayer> ListaAttributiLayer { get; set; }
    }

    public class AttributoLayer
    {

        /// <summary>
        /// Nome dell'attributo
        /// </summary>
        public string NomeAttributo { get; set; }

        /// <summary>
        /// Tipo del dato dell'attributo
        /// </summary>
        public string TipoDato { get; set; }

        /// <summary>
        /// Progressivo della tabella DataStruct
        /// </summary>
        public string ProgressivoDataStruct { get; set; }

        /// <summary>
        /// Flag CampoChiave per l'attributo
        /// </summary>
        public string CampoChiave { get; set; }

        /// <summary>
        /// Indica se l'attributo è attivo
        /// </summary>
        public string TemaAttivo { get; set; }

        /// <summary>
        /// Indica se l'etichetta è visibile
        /// </summary>
        public bool EtichettaVisibile { get; set; }

        /// <summary>
        /// Elenco delle traduzioni del nome dell'etichetta del layer
        /// </summary>
        public List<Gis_Traduzione> Traduzioni { get; set; }
    }
}
