using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.attivita.risorse;
using AgronicaCoreModelsSTD.metaschema;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.attivita
{
    public class RilevamentoDiMagazzino
    {
        // public int Id { get; set; }

        public Prodotto Prodotto { get; set; }

        public Fabbricato Magazzino { get; set; }

        public Fabbricato Agenzia { get; set; }

        /// <summary>
        /// Quantità scaricata (Sempre kg o l): In scarico in AGENDA si salva un record in Mov_Destinazione.QTA (Tipo_Destinazione = 20) ed anche in Movimenti_Dettagli_QTA (stessa quantità)
        /// </summary>
        public decimal Qta { get; set; }

        /// <summary>
        /// Utilizzata al momento come Giacenza Totale nella visualizzazione delle giacenze dei prodotti (in questo scenario Qta è la Giacenza alla data)
        /// </summary>
        public decimal QtaTot { get; set; }

        /// <summary>
        /// Lotto del prodotto
        /// </summary>
        public string Lotto { get; set; }

        /// <summary>
        /// FEDE VERIFICA
        /// </summary>
        public int Cal_Cod { get; set; }

        /// <summary>
        /// FEDE VERIFICA
        /// </summary>
        public int Cod_Progetto { get; set; }

        public UnitaDiMisura udm { get; set; }

        public RilevamentoMagazzinoTipo TipoRilevamento { get; set; }

        public string Descrizione { get; set; }

        public decimal? N { get; set; }

        public decimal? P2O5 { get; set; }

        public decimal? K2O { get; set; }

        public decimal? Cu { get; set; }

        /// <summary>
        /// Movimenti_dettagli.qta (suddiviso per magazzino, in base alla qta, espressa rispetto alla udm indicata)
        /// </summary>
        public decimal doseHaIndicata { get; set; }
        /// <summary>
        /// Movimenti_dettagli.Qta_Extra (suddiviso per magazzino, in base alla qta, espressa rispetto alla udm indicata)
        /// </summary>
        public decimal doseHlIndicata { get; set; }

        public List<Attivita> registrazioniCollegate { get; set; }

        public RilevamentoDiMagazzino()
        {

        }

        public enum RilevamentoMagazzinoTipo
        {
            carico = 1,
            scarico = -1,
            giacenza = 0,
            prodotto = 2
        }

        public new RilevamentoDiMagazzino Clona()
        {
            try
            {

                string output = JsonConvert.SerializeObject(this);
                RilevamentoDiMagazzino deserializedObject = JsonConvert.DeserializeObject<RilevamentoDiMagazzino>(output);
                return deserializedObject;
            }
            catch (Exception ex)
            {
                throw new Exception("RilevamentoDiMagazzino.Clona: " + ex.Message);
            }

        }
    }
}
