using AgronicaCoreModelsSTD.attivita.risorse;
using AgronicaCoreModelsSTD.metaschema.avversita;
using AgronicaCoreModelsSTD.metaschema;
using System;
using System.Collections.Generic;
using System.Text;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.baseClass;
using Newtonsoft.Json;

namespace AgronicaCoreModelsSTD.attivita.dettagli
{
    public class DettaglioRegistroSomministrazioni : RisorsaProdotto
    {
        /// <summary>
        /// Numero somministrazione 
        /// </summary>
        public string codice { get; set; }

        /// <summary>
        /// Tipo di prescrizione da cui è stato generata la somministrazione, riferimenti a enum_TipoPrescrizione
        /// </summary>
        public int prescrizioneOrigine { get; set; }

        /// <summary>
        /// Numero trattamento
        /// </summary>
        public string numTrattamento { get; set; }

        /// <summary>
        /// Indica se si tratta della prima somministraziona di un gruppo (caso tipo prescrizione Da_Protocollo_GIAS)
        /// </summary>
        public bool isFirstSomm { get; set; }

        /// <summary>
        /// Indica se la somministrazione ha somministrazioni successive confermate appartenenti allo stesso gruppo (caso tipo prescrizione Da_Protocollo_GIAS)
        /// </summary>
        public bool hasSuccessiveConfermate { get; set; }
        
        /// <summary>
        /// Ricette_Zoo_Dettagli.Qta_Dose
        /// </summary>
        public float qtaDose { get; set; }
        /// <summary>
        /// Ricette_Zoo_Dettagli.Udm_Dose
        /// </summary>
        public int udmDose { get; set; }

        /// <summary>
        /// Movimenti_Dettagli.Pro_Cod da Farmaci.AIC
        /// </summary>
        public string codiceAIC { get; set; }

        /// <summary>
        /// Mov_Dettaglio_Tecnico.av_cod, Mov_Dettaglio_Tecnico.av_gru
        /// </summary>
        public AvversitaGruppo avversitaGruppo { get; set; }

        /// <summary>
        /// Movimenti_dettagli.tempocarenza
        /// </summary>
        public TempiSospensione[] sospensione { get; set; }

        /// <summary>
        /// Data inizio e fine del trattamento
        /// </summary>
        public IntervalloTemporale validita { get; set; }

        /// <summary>
        /// Data della prescrizione
        /// </summary>
        public DateTime dataPrescrizione { get; set; }

        /// <summary>
        /// Durata del trattamento in giorni
        /// </summary>
        public int durataTrattamento { get; set; }

        /// <summary>
        /// Numero del registro di scorta (da VetInfo)
        /// </summary>
        public string regSco_Numero { get; set; }

        /// <summary>
        /// Parametro considerato per l'arrotondamento del peso
        /// </summary>
        public int arrotondamentoPeso { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool massivo { get; set; }

        /// <summary>
        /// Categorie Semplificate del Farmaco (da Farmaci_Categorie_Semplificate)
        /// </summary>
        public FarmacoCategoriaSemplificata[] farmacoCatSem { get; set; }

        public DettaglioRegistroSomministrazioni()
        {
            classType = costanti.ClassType.DettaglioRegistroSomministrazioni;
        }

        public new DettaglioRegistroSomministrazioni Clona()
        {
            try
            {

                string output = JsonConvert.SerializeObject(this);
                DettaglioRegistroSomministrazioni deserializedObject = JsonConvert.DeserializeObject<DettaglioRegistroSomministrazioni>(output);
                return deserializedObject;
            }
            catch (Exception ex)
            {
                throw new Exception("DettaglioRegistroSomministrazioni.Clona: " + ex.Message);
            }

        }

    }

    public class TempiSospensione
    {
        public int tempoSospensione { get; set; }
        public BaseCodeDescr Alimento { get; set; }
    }

    public class FarmacoCategoriaSemplificata : BaseCodeDescr
    {
        public FarmacoCategoriaSemplificata(int code, string des) : base(code, des)
        {

        }
    }
}
