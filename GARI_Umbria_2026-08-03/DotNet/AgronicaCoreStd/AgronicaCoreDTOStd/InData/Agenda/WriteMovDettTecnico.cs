using System;
using System.Data;

namespace InData.Agenda
{
    public class WriteMovDettTecnico
    {
        public string Piva { get; set; }
        public int? Sa_Cod { get; set; }
        public int Id_Agenda { get; set; }
        public int Id_Mov { get; set; }
        public int Id_Mov_Det { get; set; }
        public int Id_Reg_Dettaglio { get; set; }
        
        public double? Qta_Ril { get; set; }
        public DateTime? Data_Ril { get; set; }
        public int? Ditta_Cod { get; set; }
        public int? Dett_Cod { get; set; }
        public int? Id_Insetto { get; set; }
        public int? FF_Classe { get; set; }
        public double? Dose { get; set; }
        public double? Mg { get; set; }
        public double? N { get; set; }
        public double? K { get; set; }
        public double? P { get; set; }
        public int? Parziale { get; set; }
        public int? Nitrati { get; set; }
        public double? Freatimetro { get; set; }
        public double? Piezo1 { get; set; }
        public double? Piezo2 { get; set; }
        public double? Piezo3 { get; set; }
        public double? Piezo4 { get; set; }
        public string Sigla_AV { get; set; }
        public int? Trap_Num { get; set; }
        public DateTime? Inn1_Data { get; set; }
        public DateTime? Inn2_Data { get; set; }
        public DateTime? Inn3_Data { get; set; }
        public DateTime? Inn4_Data { get; set; }
        public int? Av_Cod { get; set; }
        public int? Av_Gru { get; set; }
        public string Lotto { get; set; }
        public string Extra_Str { get; set; }
        public DateTime? Extra_Date { get; set; }
        public int? Extra_Int { get; set; }
        public int? Soglia_Cod { get; set; }
        public string Soglia_Des { get; set; }
        public double? Soglia_Quantita { get; set; }
        public double? Efficienza { get; set; }
        public double? Cu { get; set; }

        public int? Inviato { get; set; }
        public DateTime? Data_Invio { get; set; }

        public DateTime Validita_Inizio { get; set; }
        public DateTime Validita_Fine { get; set; }

        public WriteMovDettTecnico() { }

        public WriteMovDettTecnico(string piva, int idAgenda, int idMov, int idMovDet, int idRegDet)
        {
            Piva = piva;
            Id_Agenda = idAgenda;
            Id_Mov = idMov;
            Id_Mov_Det = idMovDet;
            Id_Reg_Dettaglio = idRegDet;
        }
    }

    public class MovDettTecnicoRow : DataRowWrapper
    {
        public MovDettTecnicoRow(DataRow row) : base(row) { }
    }
}
