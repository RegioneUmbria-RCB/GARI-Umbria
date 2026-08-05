using AgronicaCoreModelsSTD.attivita.dettagli;
using AgronicaCoreModelsSTD.metaschema;
using AgronicaCoreModelsSTD.metaschema.avversita;
using AgronicaCoreModelsSTD.metaschema.utilizzi;

namespace AgronicaCoreDTOStd.InData.AgronicaChatGPT
{
    public class CheckTrattamento
    {
        public DettaglioTrattamento dettaglioTrattamento {  get; set; }

        public AvversitaGruppo avversitaGruppo { get; set; }

        public UtilizzoTerreno utilizzoTerreno { get; set; }

        public Disciplinare disciplinare { get; set; }
    }
}
