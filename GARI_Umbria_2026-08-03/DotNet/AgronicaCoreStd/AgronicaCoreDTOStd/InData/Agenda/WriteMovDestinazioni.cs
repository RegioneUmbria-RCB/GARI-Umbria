using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace InData.Agenda
{
    public class WriteMovDestinazioni
    {
        public string Piva { get; set; }
        public int? Sa_Cod { get; set; }
        public int Id_Agenda { get; set; }
        public int Id_Mov { get; set; }
        public int Id_Mov_Det { get; set; }
        public int Id_Destinazione { get; set; }
        public int? Appezza { get; set; }
        public int? Tipo_Destinazione { get; set; }
        public double? Qta { get; set; }
        
        public double? Qta2 { get; set; }
        public int? Tipo_Scorta { get; set; }
        public double? Scorta_Min { get; set; }
        public string Mov_Destinazioni_GraphicKey { get; set; }
        public double? Qta_Dest1 { get; set; }
        public double? Qta_Dest2 { get; set; }
        public double? Sup_Riduzione_BufferZone { get; set; }
        public double? Perc_Riduzione_Deriva { get; set; }
        public double? QuotaDistribuzione { get; set; }
        public int? Sa_Cod_Riferimento { get; set; }
        public int? Id_Destinazione_Riferimento { get; set; }
        public int? Tipo_Destinazione_Riferimento { get; set; }
        public string Extra_Str { get; set; }

        public int? Inviato { get; set; }
        public DateTime? Data_Invio { get; set; }

        public DateTime Validita_Inizio { get; set; }
        public DateTime Validita_Fine { get; set; }

        public WriteMovDestinazioni() { }

        public WriteMovDestinazioni(string piva, int idAgenda, int idMov, int idMovDet, int idDest)
        {
            Piva = piva;
            Id_Agenda = idAgenda;
            Id_Mov = idMov;
            Id_Mov_Det = idMovDet;
            Id_Destinazione = idDest;
        }
    }

    public class MovDestinazioniRow : DataRowWrapper
    {
        public MovDestinazioniRow(DataRow row) : base(row) { }
    }
}
