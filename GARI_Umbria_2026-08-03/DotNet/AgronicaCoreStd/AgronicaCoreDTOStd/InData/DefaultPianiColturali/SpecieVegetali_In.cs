namespace InData.SpecieVegetali
{
    public class SpecieVegetali_In
    {
        public string Piva { get; set; }
        public SpecieVegetaliData_In[] Data { get; set; }
    }

    public class SpecieVegetaliData_In
    {
        public int VegCod { get; set; }
        public string TipoMaturazione { get; set; }
        public decimal SogliaMinima { get; set; }
        public decimal ResaStabilimento { get; set; }
        public decimal PesoSgocciolato { get; set; }
    }
}