namespace AgronicaNetCore.OperazioniZoo.DAL.DataLayer.Models
{
    public class Agenda
    {
        public int IdAgenda { get; set; }
        public string Piva { get; set; }
        public int SaCod { get; set; }
        public int StaNum { get; set; }
        public int LavCod { get; set; }
        public string LavDes { get; set; }
        
        public int BloccoFlag { get; set; }
        public DateTime BloccoData { get; set; }
        public string BloccoUsername { get; set; }

        public int Inviato { get; set; }
        public DateTime? DataInvio { get; set; }
        public DateTime DataCreazione { get; set; }
        public DateTime DataModifica { get; set; }
        public string UsernameCreazione { get; set; }
        public string UsernameModifica { get; set; }
        public DateTime ValiditaInizio { get; set; }
        public DateTime ValiditaFine { get; set; }

        public int LineaCod { get; set; }
        public int PreparazioneCod { get; set; }
        public int IdTrasformazione { get; set; }
        public int TipoAccettazione { get; set; }
        public int StatoExport { get; set; }
        public int StatoExport2 { get; set; }
        public int TipoVisibilita { get; set; }
        public int ChkCogeManuale { get; set; }
        public int IdAttivita { get; set; }
        public int Modulo { get; set; }
        public int AuditCod { get; set; }
        public int Split { get; set; }
        public int RaccoglitoreCod { get; set; }
        public int PraticaCod { get; set; }
        public string Origine { get; set; }
        public int? StatoCod { get; set; }
        public int? DaRemoto { get; set; }
    }
}
