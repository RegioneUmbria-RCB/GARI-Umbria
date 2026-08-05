using System.Collections.Generic;
using AgronicaCoreModelsSTD.attivita.dettagli;
using AgronicaCoreModelsSTD.metaschema;
using Newtonsoft.Json;
using System;

namespace AgronicaCoreModelsSTD.attivita.risorse
{
    /// <summary>
    /// 1 record in tabella Movimenti_Dettagli
    /// </summary>
    public class RisorsaRegistrazione : Risorsa
    {
        public Prodotto prodotto { get; set; }
        public List<RilevamentoDiMagazzino> MagazziniMovimentazioni { get; set; }

        /// <summary>
        /// Movimenti_dettagli.qta 
        /// </summary> 
        public decimal qta { get; set; }
        
        /// <summary>
        /// Movimenti_dettagli.udm_cod
        /// </summary>
        public UnitaDiMisura unitaDiMisura { get; set; }

        /// <summary>
        /// Movimenti_dettagli.pendente
        /// </summary>
        public Causale causale { get; set; }

        /// <summary>
        /// Movimenti_dettagli_tecnici eventualmente associati al movimento_dettagli
        /// </summary>
        public DettaglioRegistrazione dettaglioRegistrazione { get; set; }

        public RisorsaRegistrazione()
        {
            classType = costanti.ClassType.RisorsaRegistrazione;
        }

        public RisorsaRegistrazione Clona()
        {
            try
            {

                string output = JsonConvert.SerializeObject(this);
                RisorsaRegistrazione deserializedObject = JsonConvert.DeserializeObject<RisorsaRegistrazione>(output);
                return deserializedObject;
            }
            catch (Exception ex)
            {
                throw new Exception("RisorsaRegistrazione.Clona: " + ex.Message);
            }

        }

    }
}
