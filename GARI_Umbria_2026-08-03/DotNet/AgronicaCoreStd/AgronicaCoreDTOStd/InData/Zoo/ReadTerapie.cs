using System;

namespace InData.Zoo
{
    public class ReadTerapie
    {
        public int? Id_Terapia { get; set; }
        public string Piva { get; set; }
        public int? Sa_Cod { get; set; }
        public int? Sta_Num { get; set; }
        public DateTime? Data { get; set; }

        public ReadTerapie() {}

        public ReadTerapie(int idTerapia) => this.Id_Terapia = idTerapia;

        public ReadTerapie(string piva, int saCod, int staNum, DateTime data)
        {
            this.Piva = piva;
            this.Sa_Cod = saCod;
            this.Sta_Num = staNum;
            this.Data = data;
        }
    }
}
