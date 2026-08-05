using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace InData.Agenda
{
    public class WriteMovimenti
    {
        public string Piva { get; set; }
        public int? Sa_Cod { get; set; }
        public int Id_Agenda { get; set; }
        public int Id_Mov { get; set; }
        public int Cau_Mov { get; set; }
        public string Mov_Desc { get; set; }
        public DateTime Data_Movimento { get; set; }
        
        public int? Cod_RisUm { get; set; }
        public DateTime? Scadenza { get; set; }
        public int? Doc_Numero { get; set; }
        public int? Num_Protocollo { get; set; }
        public int? Cod_IndirizzoRisUm { get; set; }
        public int? Cod_Destinazione { get; set; }
        public int? Cod_IndirizzoDestinazione { get; set; }
        public int? Mezzo { get; set; }
        public int? Cod_Vettore { get; set; }
        public int? Cod_IndirizzoVettore { get; set; }
        public int? Causale_Trasporto { get; set; }
        public string Aspetto { get; set; }
        public float? Peso { get; set; }
        public DateTime? Ora { get; set; }
        public int? Colli { get; set; }
        public string Extra_Str { get; set; }
        public int? Extra_Int { get; set; }
        public DateTime? Extra_Date { get; set; }
        public int? Tipo_Sconto { get; set; }
        public string Doc_Numero_Des { get; set; }
        public string Natura_Beni { get; set; }
        public int? Tara_Veicolo { get; set; }
        public int? Tara_Imballi { get; set; }
        public int? Tipo_Peso { get; set; }
        public int? Modalita { get; set; }
        public string Username_Note { get; set; }
        public DateTime? Scadenza_Extra { get; set; }
        public string Doc_Numero_Sin { get; set; }
        public int? Progr_Protocollo { get; set; }
        public int? Progr_Registrazione { get; set; }
        public DateTime? Data_Registrazione { get; set; }
        public int? ChkLayOut_Bypass_Fatturato { get; set; }
        public int? ChkLayOut_Join_Prodotti { get; set; }
        public int? Cod_RisUm_Altro { get; set; }
        public int? ChkLayOut_Peso { get; set; }
        public int? ChkLayOut_Prezzo { get; set; }
        public int? ChkFiltro_Varietale { get; set; }
        public int? Disciplinare_PubblicoPrivato { get; set; }
        public int? Sezionale_Cod { get; set; }
        public int? Causale_Trasporto_Cod { get; set; }
        public int? ChkLayOut_Litri { get; set; }
        public int? Cod_RisUm_Aggiuntivo { get; set; }
        public int? Cod_Indirizzo_Aggiuntivo { get; set; }
        public int? ChkLayOut_Riscontrato { get; set; }
        public string Doc_Numero_Visualizzato { get; set; }
        public int? TipoDocumento { get; set; }
        public string Cod_Macchina_Lav { get; set; }
        public DateTime? OraFine { get; set; }
        public int? Modalita_Applicazione { get; set; }

        public int? Inviato { get; set; }
        public DateTime? Data_Invio { get; set; }

        public DateTime Validita_Inizio { get; set; }
        public DateTime Validita_Fine { get; set; }

        public WriteMovimenti() { }

        public WriteMovimenti(string piva, int idAgenda, int idMov)
        {
            Piva = piva;
            Id_Agenda = idAgenda;
            Id_Mov = idMov;
        }
    }

    public class MovimentiRow : DataRowWrapper
    {
        public MovimentiRow(DataRow row) : base(row) { }
    }
}
