using System;
using System.Data;
using System.Collections.Generic;

namespace InData.Zoo
{
    public class WriteTerapia
    {
        public int Id_Terapia { get; set; }
        public string Piva { get; set; }
        public int Sa_Cod { get; set; }
        public int Sta_Num { get; set; }
        public string Terapia_Des { get; set; }
        public int? inviato { get; set; }
        public DateTime? dataInvio { get; set; }
        public DateTime? Data_Creazione { get; set; }
        public DateTime? Data_Modifica { get; set; }
        public string Utente_Creazione { get; set; }
        public string Utente_Modifica { get; set; }
        public DateTime? Validita_Inizio { get; set; }
        public DateTime? Validita_Fine { get; set; }

        public WriteTerapia() { }

        public WriteTerapia(int id, string piva, int saCod, int staNum, string des = "")
        {
            this.Id_Terapia = id;
            this.Terapia_Des = des;
            this.Piva = piva;
            this.Sa_Cod = saCod;
            this.Sta_Num = staNum;
        }
    }

    public class TerapieRow : DataRowWrapper
    {
        public TerapieRow(DataRow row) : base(row) { }
    }
}
