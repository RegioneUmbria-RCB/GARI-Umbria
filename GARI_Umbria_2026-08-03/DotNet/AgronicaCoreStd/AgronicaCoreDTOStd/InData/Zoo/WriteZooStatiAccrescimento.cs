using System;
using System.Data;

namespace InData.Zoo
{
    public class WriteZooStatiAccrescimento
    {
        public string Piva { get; set; }
        public int? Sa_Cod { get; set; }
        public int Cod_Progetto { get; set; }
        public int Gen_Cod { get; set; }
        public int Spe_Cod { get; set; }
        public int Tipo_Cod { get; set; }
        public int Stato_Cod { get; set; }
        public string Note { get; set; }

        public int? Inviato { get; set; }
        public DateTime? Data_Invio { get; set; }

        public DateTime Validita_Inizio { get; set; }
        public DateTime Validita_Fine { get; set; }

        public WriteZooStatiAccrescimento() { }
    }

    public class ZooAnimalixStatiAccrescimentoRow : DataRowWrapper
    {
        public ZooAnimalixStatiAccrescimentoRow(DataRow row) : base(row) { }
    }
}
