using System;
using System.Data;

namespace InData.Zoo
{
    public class WriteZooDistinte
    {
        public string Piva { get; set; }
        public int? Sa_Cod { get; set; }
        public int Cod_Animale { get; set; }
        public int Cod_Progetto { get; set; }
        public string Progetto_Des { get; set; }
        public string Progetto_Nome { get; set; }

        public string Codice_Distinta { get; set; }
        public int? Distinta_Chiusa { get; set; }

        public int? Inviato { get; set; }
        public DateTime? Data_Invio { get; set; }

        public DateTime Validita_Inizio { get; set; }
        public DateTime Validita_Fine { get; set; }

        public WriteZooDistinte() { }
    }

    public class ZooAnimaliDistinteRow : DataRowWrapper
    {
        public ZooAnimaliDistinteRow(DataRow row) : base(row) { }
    }
}
