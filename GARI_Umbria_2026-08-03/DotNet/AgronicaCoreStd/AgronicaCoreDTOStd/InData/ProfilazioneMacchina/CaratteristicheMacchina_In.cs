using System.Collections.Generic;

namespace InData.ProfilazioneMacchina
{
    public class CaratteristicheMacchina_In
    {
        public CaratteristicheMacchinaValue[] Caratteristiche { get; set; }
        public int IdProfilazione { get; set; }
    }

    public class CaratteristicheMacchinaValue
    {
        public int MacCod { get; set; }
        public int MacCarCod { get; set; }
        public string Valore { get; set; }
    }
}