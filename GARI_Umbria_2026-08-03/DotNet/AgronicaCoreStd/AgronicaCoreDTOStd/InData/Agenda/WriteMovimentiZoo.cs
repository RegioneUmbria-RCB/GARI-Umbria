using System;
using System.Data;

namespace InData.Agenda
{
    public class WriteMovimentiZoo
    {
        public string Piva { get; set; }
        public int? Sa_Cod { get; set; }
        public int Id_Agenda { get; set; }
        public int Id_Mov { get; set; }

        public int? Raz_Cod { get; set; }
        public string Nazione_Cod { get; set; }
        public string Progetto { get; set; }
        public string Certificato { get; set; }
        public DateTime? Data_Documento_Ingresso { get; set; }
        public string Fornitore_Provenienza { get; set; }
        public string Fornitore_Fatturazione { get; set; }
        public string Lotto_Fornitore { get; set; }
        public string N_Bolla_Fornitore { get; set; }
        public DateTime? Data_DDT_Ingresso { get; set; }
        public string Modello4_Ingresso { get; set; }
        public string Modello4_Ingresso_Numero { get; set; }
        public double? Kg_Pagati { get; set; }
        public double? Kg_Arrivo { get; set; }
        public double? Kg_Pagati_Medio { get; set; }
        public double? Kg_Arrivo_Medio { get; set; }
        public double? Calo_Tot { get; set; }
        public double? Calo_Medio { get; set; }
        public double? Calo_Perc { get; set; }
        public double? Incremento_Teorico { get; set; }
        public double? Costo_Totale { get; set; }
        public double? Costo_Unitario { get; set; }
        public string Pres_Numero { get; set; }
        public string PresRiga_Numero { get; set; }
        public int? Tipo_Calcolo_Peso { get; set; }
        public double? Kg_Partenza { get; set; }
        public double? Coeff_Calo_Peso { get; set; }
        public int? Stato_Trattamento { get; set; }
        public int? Tipo_Trattamento { get; set; }
        public double? Kg_Aggiuntivi { get; set; }
        public string Note { get; set; }

        public int? Inviato { get; set; }
        public DateTime? Data_Invio { get; set; }

        public DateTime Validita_Inizio { get; set; }
        public DateTime Validita_Fine { get; set; }

        public WriteMovimentiZoo() { }

        public WriteMovimentiZoo(string piva, int idAgenda, int idMov)
        {
            Piva = piva;
            Id_Agenda = idAgenda;
            Id_Mov = idMov;
        }
    }

    public class MovimentiZooRow : DataRowWrapper
    {
        public MovimentiZooRow(DataRow row) : base(row) { }
    }
}
