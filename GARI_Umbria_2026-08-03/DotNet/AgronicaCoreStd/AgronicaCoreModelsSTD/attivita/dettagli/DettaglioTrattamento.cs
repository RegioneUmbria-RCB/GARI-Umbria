using AgronicaCoreModelsSTD.attivita.risorse;
using AgronicaCoreModelsSTD.metaschema.avversita;
using AgronicaCoreModelsSTD.metaschema;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AgronicaCoreModelsSTD.attivita.dettagli
{
    public class DettaglioTrattamento : RisorsaProdotto
    {
        /// <summary>
        /// Mov_Dettaglio_Tecnico.av_cod, Mov_Dettaglio_Tecnico.av_gru
        /// </summary>
        public AvversitaGruppo avversitaGruppo { get; set; }

        /// <summary>
        /// Movimenti_dettagli.PrincipiAttivi (pa_cod1§titolo1|pa_cod2§titolo2 ...), Movimenti_dettagli.PrincipiAttiviPesi (pa_cod1§peso1|pa_cod2§peso2 ...)
        /// </summary>
        public List<PrincipioAttivo> principiAttivi { get; set; }

        /// <summary>
        /// Movimenti_dettagli.tempocarenza
        /// </summary>
        public int tempoCarenza { get; set; }
        /// <summary>
        /// Movimenti_dettagli.Buffer (BufferMin|BufferMax)
        /// </summary>
        public BufferZone bufferzone { get; set; }

        /// <summary>
        /// Movimenti_dettagli.Extra_Str (for_veg_cod = codice tabella matrice FormulatixSpecieVegetali)
        /// </summary>
        public int dettaglioProdotto { get; set; }

        /// <summary>
        /// Movimenti_dettagli.DoseEtichetta, DoseEtichetta_Value
        /// </summary>
        public List<DoseEtichetta> dosiEtichetta { get; set; }

        /// <summary>
        /// Mov_Dettaglio_Tecnico.Soglia_Cod, Mov_Dettaglio_Tecnico.Soglia_Des, Mov_Dettaglio_Tecnico.Soglia_Quantita
        /// </summary>
        public Soglia soglia { get; set; }

        public string descrizionePrecedente { get; set; }
        public DateTime dataSmaltimentoScorte { get; set; }
        public string classificazioni { get; set; }
        public string inRevisione { get; set; }
        public DateTime dataAttoNormativo { get; set; }
        public int formulatiXAllegatiNormative_IDRiga { get; set; }
        public int tipoFormulato { get; set; }
        public string epocheBlocchi { get; set; }
        public string dettagliDose { get; set; }
        public string protezione { get; set; }
        public string modalitaImpiego { get; set; }
        public Tipo_Polverulento polverulento { get; set; }
        public bool isImpollinatore { get; set; }
        public int durataFeromone { get; set; }
        public DateTime scadenzaFeromone { get; set; }
        public List<QuantitaSuImpianto> quantitaSuImpianti { get; set; }
        public enum_Ripartizione_Trappole ripartizioneTrappole { get; set; }

        public DettaglioTrattamento()
        {
            classType = costanti.ClassType.DettaglioTrattamento;
        }

        public new DettaglioTrattamento Clona()
        {
            try
            {

                string output = JsonConvert.SerializeObject(this);
                DettaglioTrattamento deserializedObject = JsonConvert.DeserializeObject<DettaglioTrattamento>(output);
                return deserializedObject;
            }
            catch (Exception ex)
            {
                throw new Exception("DettaglioTrattamento.Clona: " + ex.Message);
            }

        }
    }

    public enum Tipo_Polverulento
    {
        NonPolverulento = 0,
        Polverulento = 1
    }

    public enum enum_Ripartizione_Trappole
    {
        Manuale = 1,
        Automatica = 2
    }

}
