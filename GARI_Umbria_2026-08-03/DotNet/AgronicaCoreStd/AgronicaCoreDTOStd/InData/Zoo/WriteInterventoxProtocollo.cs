using OutData.Zoo;
using System;
using System.Data;

namespace InData.Zoo
{
    public class WriteInterventoxProtocollo
    {
        public int Id_Terapia { get; set; }
        public int Id_Intervento { get; set; }
        public int Id_Protocollo { get; set; }
        public int? Id_Protocollo_Alt { get; set; }
        public float? Qta { get; set; }
        public int? Udm { get; set; }
        public int? Durata { get; set; }
        public int? Udm_Durata { get; set; }
        public int? Intervallo { get; set; }
        public int? Udm_Intervallo { get; set; }
        public float? Scaglione_Cambio_Dose { get; set; }

        public int? inviato { get; set; }
        public DateTime? dataInvio { get; set; }
        public DateTime? Data_Creazione { get; set; }
        public DateTime? Data_Modifica { get; set; }
        public string Utente_Creazione { get; set; }
        public string Utente_Modifica { get; set; }
        public DateTime? Validita_Inizio { get; set; }
        public DateTime? Validita_Fine { get; set; }

        public WriteInterventoxProtocollo() { }

        public WriteInterventoxProtocollo(int idTerapia, int idIntervento, int idProtocollo, int idProtAlt = 0)
        {
            this.Id_Terapia = idTerapia;
            this.Id_Intervento = idIntervento;
            this.Id_Protocollo = idProtocollo;
            this.Id_Protocollo_Alt = idProtAlt;
        }

        public WriteInterventoxProtocollo(int Id_Terapia, int Id_Intervento, ProtocolloFromIntervento prot)
        {
            this.Id_Terapia = Id_Terapia;
            this.Id_Intervento = Id_Intervento;
            this.Id_Protocollo = prot.Id_Protocollo;
            this.Id_Protocollo_Alt = prot.Id_Protocollo_Alt;
        }
    }

    public class InterventoxProtocolloRow : DataRowWrapper
    {
        public InterventoxProtocolloRow(DataRow row) : base(row) { }
    }
}
