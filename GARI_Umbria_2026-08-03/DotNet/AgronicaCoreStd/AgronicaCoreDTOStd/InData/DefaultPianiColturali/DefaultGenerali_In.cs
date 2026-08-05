namespace InData.DefaultPianiColturali
{
    public class DefaultGenerali_In
    {
        public string Piva { get; set; }
        public DefaultGeneraliData_In[] Data { get; set; }
    }

    public class DefaultGeneraliData_In
    {
        public int VegCod { get; set; }
        public int CulCod { get; set; }
        public int UnitaCalore { get; set; }
        public int Gg { get; set; }
        public double Resa { get; set; }
        public double DosiHa { get; set; }
        public double DosiHaUdm { get; set; }
        public double Cal1 { get; set; }
        public double Cal2 { get; set; }
        public double Cal3 { get; set; }
        public double Cal4 { get; set; }
        public double Cal5 { get; set; }
        public double Cal6 { get; set; }
    }
}