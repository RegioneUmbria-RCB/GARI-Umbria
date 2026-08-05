namespace InData.Zoo
{
    public class UpdatePrescrizione
    {
        public string Piva { get; set; }
        public int SaCod { get; set; }

        public int IdRicetta { get; set; }
        public int IdAgenda { get; set; }
        public int IdMov { get; set; }
        public int IdDettaglio { get; set; }

        public int? NumSomm { get; set; }
        public int? IntSomm { get; set; }
        public float? QtaDose { get; set; }
        public int? UdmDose { get; set; }
        public int? ArrotondamentoPeso { get; set; }
        public bool? Massivo { get; set; }
    }
}
