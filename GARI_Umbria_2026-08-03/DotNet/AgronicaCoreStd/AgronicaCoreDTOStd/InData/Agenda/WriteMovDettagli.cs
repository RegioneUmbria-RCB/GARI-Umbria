using System;
using System.Data;

namespace InData.Agenda
{
    public class WriteMovDettagli
    {
        public string Piva { get; set; }
        public int? Sa_Cod { get; set; }
        public int Id_Agenda { get; set; }
        public int Id_Mov { get; set; }
        public int Id_Mov_Det { get; set; }
        public int? Elem_Cod { get; set; }
        public int? Pro_Cod { get; set; }
        public int? Mat_Cod { get; set; }
        public string Mov_Det_Des { get; set; }
        public int? Udm_Cod { get; set; }
        public double? Qta { get; set; }

        public int? Cod_Iva { get; set; }
        public double? Sconto { get; set; }
        public double? Prezzo_Unitario { get; set; }
        public int? Cod_Conto { get; set; }
        public int? Cod_Progetto { get; set; }
        public int? Fase_Cod { get; set; }
        public int? Contabilizzato { get; set; }
        public int? Pendente { get; set; }
        public int? Cal_Cod { get; set; }
        public string Extra_Str { get; set; }
        public int? Extra_Int { get; set; }
        public DateTime? Extra_Date { get; set; }
        public int? Anno { get; set; }
        public int? Ric_Cod { get; set; }
        public double? Imponibile { get; set; }
        public double? Iva { get; set; }
        public string Lotto { get; set; }
        public int? Jolly_Int { get; set; }
        public double? Imponibile_Netto { get; set; }
        public double? Prezzo_Unitario_Netto { get; set; }
        public int? UDM_COD_EXTRA { get; set; }
        public double? QTA_EXTRA { get; set; }
        public double? Prezzo_Effettivo { get; set; }
        public int? ChkIva_Manuale { get; set; }
        public int? Cod_IvaIndetraibile { get; set; }
        public double? Qta_Extra_Totale { get; set; }
        public double? Tara { get; set; }
        public int? ChkLayOut_Hide { get; set; }
        public double? Variazione { get; set; }
        public int? Listino_Cod { get; set; }
        public double? Sconto_Listino { get; set; }
        public int? Sconto_Modalita { get; set; }
        public int? Mat_Cod_Alias { get; set; }
        public int? Mezzo_Det { get; set; }
        public string Sconto_Testo { get; set; }
        public int? Ric_Cod_Pat { get; set; }
        public int? Cod_Conto_Pat { get; set; }
        public int? TempoCarenza { get; set; }
        public string DoseEtichetta { get; set; }
        public int? Turno_Cod { get; set; }
        public int? ID_Attivita { get; set; }
        public int? Dettaglio_VegCod { get; set; }
        public double? Iva_Indetraibile { get; set; }
        public double? Iva_Indetraibile_Perc { get; set; }
        public string PrincipiAttivi { get; set; }
        public string ClassiTossicologiche { get; set; }
        public string DoseEtichetta_Value { get; set; }
        public double? Iva_Deto_Cod { get; set; }
        public double? Qta_Dettaglio1 { get; set; }
        public double? Qta_Dettaglio2 { get; set; }
        public int? Dettagli_Blocco_Flag { get; set; }
        public string Dettagli_Blocco_Username { get; set; }
        public DateTime? Dettagli_Blocco_Data { get; set; }
        public int? Qualifica_Cod { get; set; }
        public int? Tariffa_Cod { get; set; }
        public int? Ordine_Det { get; set; }
        public int? Deroga_Cod { get; set; }
        public int? Prezzo_Livello { get; set; }
        public string PrincipiAttiviPesi { get; set; }
        public string Buffer { get; set; }
        public string Rif_Esterno { get; set; }
        public string Rif_Esterno_2 { get; set; }
        public string PrincipiAttiviPercAbb { get; set; }
        public int? Polverulento { get; set; }
        public int? Dettaglio_IdCod { get; set; }
        public int? Dettaglio_GenCod { get; set; }
        public int? Dettaglio_SpeCod { get; set; }
        public int? Dettaglio_IProCod { get; set; }
        public int? Id_Mov_Esterno { get; set; }
        public string RegSco_Numero { get; set; }

        public int? Inviato { get; set; }
        public DateTime? Data_Invio { get; set; }

        public DateTime Validita_Inizio { get; set; }
        public DateTime Validita_Fine { get; set; }

        public WriteMovDettagli() { }

        public WriteMovDettagli(string piva, int idAgenda, int idMov, int idMovDet)
        {
            Piva = piva;
            Id_Agenda = idAgenda;
            Id_Mov = idMov;
            Id_Mov_Det = idMovDet;
        }
    }

    public class MovDettagliRow : DataRowWrapper
    {
        public MovDettagliRow(DataRow row) : base(row) { }
    }
}
