
namespace AgronicaCoreDTOStd.InData.Agea
{
    public class Plot : Plot_Key
    {

        public string plotDescription { get; set; }

        public Plot(string _islandId, string _plotId,string _pcgId, string _plantationId,string _codiBarrScheVali) : base(_islandId,_plotId, _pcgId, _plantationId, _codiBarrScheVali)
        {
            islandId = _islandId;
            plotId = _plotId;
            pcgId = _pcgId;
            plantationId = _plantationId;
            codiBarrScheVali = _codiBarrScheVali;
        }

    }

    public class Plot_Key
    {
        public string islandId { get; set; }

        public string plotId { get; set; }

        public string pcgId { get; set; }

        public string plantationId { get; set; }

        public string codiBarrScheVali { get; set; }

        public Plot_Key(string islandId, string plotId, string pcgId, string plantationId, string codiBarrScheVali)
        {
            this.islandId = islandId;
            this.plotId = plotId;
            this.pcgId = pcgId;
            this.plantationId = plantationId;
            this.codiBarrScheVali = codiBarrScheVali;
        }
    }


}
