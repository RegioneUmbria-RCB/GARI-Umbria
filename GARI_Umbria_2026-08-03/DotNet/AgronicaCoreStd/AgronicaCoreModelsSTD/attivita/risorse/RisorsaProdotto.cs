using System.Collections.Generic;
using AgronicaCoreModelsSTD.attivita.dettagli;
using System;
using AgronicaCoreModelsSTD.metaschema;
using Newtonsoft.Json;

namespace AgronicaCoreModelsSTD.attivita.risorse
{
    /// <summary>
    /// 1 record in tabella Movimenti_Dettagli
    /// </summary>
    public class RisorsaProdotto : Risorsa
    {
        public Prodotto prodotto { get; set; }
        
        public List<RilevamentoDiMagazzino> MagazziniMovimentazioni { get; set; }

        /// <summary>
        /// Movimenti_dettagli.Udm_Cod_Extra (10= qta totale; 11= dose)
        /// </summary>
        public int flagDoseQuantitaTotale { get; set; }  // ex doseQuantitaTotale

        /// <summary>
        /// Movimenti_dettagli.mezzo_det (0= dose/hl; 1= dose/ha)
        /// </summary>
        public int flagTipoDose { get; set; }  // ex tipoMezzo

        /// <summary>
        /// Movimenti_dettagli.qta 
        /// </summary> 
        public decimal doseHaReale { get; set; }
        
        /// <summary>
        /// Movimenti_dettagli.Qta_Extra 
        /// </summary>
        public decimal doseHlReale { get; set; }
        
        /// <summary>
        /// Movimenti_dettagli.Qta_Extra_Totale 
        /// </summary>
        public decimal quantitaTotaleReale { get; set; } // ex doseTotaleReale

        /// <summary>
        /// Movimenti_dettagli.udm_cod (espressa sempre in Kg o l)
        /// </summary>
        public UnitaDiMisura unitaDiMisura { get; set; }
        
        /// <summary>
        /// Movimenti_dettagli.extra_int
        /// </summary>
        public UnitaDiMisura unitaDiMisuraIndicata { get; set; }

        public RisorsaProdotto()
        {
            classType = costanti.ClassType.RisorsaProdotto;
        }

        public RisorsaProdotto Clona()
        {
            try
            {

                string output = JsonConvert.SerializeObject(this);
                RisorsaProdotto deserializedObject = JsonConvert.DeserializeObject<RisorsaProdotto>(output);
                return deserializedObject;
            }
            catch (Exception ex)
            {
                throw new Exception("RisorsaProdotto.Clona: " + ex.Message);
            }

        }

        public List<string> GetLotti()
        {
            List<string> lotti = new List<string>();
            if (MagazziniMovimentazioni != null)
            {
                foreach (var magazzino in MagazziniMovimentazioni)
                {
                    if (!string.IsNullOrEmpty(magazzino.Lotto) && !lotti.Contains(magazzino.Lotto))
                    {
                        lotti.Add(magazzino.Lotto);
                    }
                }
            }
            return lotti;
        }
    }
}
