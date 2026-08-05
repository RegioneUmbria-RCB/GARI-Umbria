namespace InData.DefaultPianiColturali
{
    public class DistintaProduzione_In
    {
        public string Piva { get; set; }
        public int VegCod { get; set; }
        public DistintaProduzioneItem[] Data { get; set; }
    }

    public class DistintaProduzioneItem
    {
        public int GruCod { get; set; }
        public int VegCod { get; set; }
        public int CulCod { get; set; }
        public DistintaProduzioneValue[] Values { get; set; }
    }

    public class DistintaProduzioneValue
    {
        public int NCiclo { get; set; }
        public string DataSemina { get; set; }
        public string DataFioritura { get; set; }
        public string DataRaccolta { get; set; }
    }
}