
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.metaschema;
using AgronicaCoreModelsSTD.metaschema.utilizzi;

namespace AgronicaCoreDTOStd.InData.Metaschema
{
    public class LeggiEfficienza
    {
        public TipoFertilizzante tipoFertilizzante { get; set; }
        public Epoca epoca { get; set; }
        public Disciplinare disciplinare { get; set; }
        public Effluente effluente { get; set; }
        public TipoAllevamento tipoAllevamento { get; set; }
        public int valoreDose { get; set; }
        public ClasseTessitura[] classiTessitura { get; set; }
        public Specie specie { get; set; }
    }
}