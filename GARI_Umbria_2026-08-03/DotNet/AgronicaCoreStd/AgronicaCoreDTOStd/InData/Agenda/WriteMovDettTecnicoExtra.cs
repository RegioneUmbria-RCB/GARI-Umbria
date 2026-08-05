using System;
using System.Data;

namespace InData.Agenda
{
    public class WriteMovDettTecnicoExtra
    {
        public string Piva { get; set; }
        public int? Sa_Cod { get; set; }
        public int Id_Agenda { get; set; }
        public int Id_Mov { get; set; }
        public int Id_Mov_Det { get; set; }
        public int Id_Reg_Det { get; set; }

        public string Regione { get; set; }
        public string Asl { get; set; }
        public string Serie { get; set; }
        public string Numero { get; set; }
        public int? Mac_Cod { get; set; }
        public int? Cod_RisUm { get; set; }
        public string Trasportatore { get; set; }
        public string Mezzo_Trasporto { get; set; }
        public string Targa { get; set; }
        public string N_Immatricolazione { get; set; }
        public string N_Immatricolazione_Rimorchio { get; set; }
        public string N_Autorizzazione_Trasporto { get; set; }
        public DateTime? Data_Rilascio_Autorizzazione { get; set; }
        public double? Peso { get; set; }
        public int? Codice_Prodotto { get; set; }
        public int? Colore { get; set; }
        public string Zona_Viticola { get; set; }
        public int? Manipolazioni { get; set; }
        public string Precisazioni { get; set; }
        public string Annotazioni { get; set; }
        public int? Num_Contenitori { get; set; }
        public string Marche_Contenitori { get; set; }
        public string Des_Contenitori { get; set; }
        public string Tipo_Documento { get; set; }
        public int? Id_Cod_Autorita { get; set; }
        public string Luogo_Partenza { get; set; }
        public string Luogo_Consegna { get; set; }
        public DateTime? Data_Spedizione { get; set; }
        public string Indicazioni_Complementari { get; set; }
        public double? Titolo_Alcol { get; set; }
        public string Codice_NC { get; set; }
        public string Num_Riferimento { get; set; }
        public DateTime? Data_Dichiarazione { get; set; }
        public string Garanzia { get; set; }
        public string Certificati { get; set; }
        public string Durata_Viaggio { get; set; }
        public double? Peso_Lordo { get; set; }
        public int? Num_Colli { get; set; }
        public int? Contenitore_Cod { get; set; }
        public int? Imballaggio_Cod { get; set; }
        public int? Agente_Cod { get; set; }
        public double? Provvigione { get; set; }
        public int? Tipo_Trasporto { get; set; }
        public int? Unita_Trasporto { get; set; }
        public string Codice_Alternativo { get; set; }
        public int? Id_Gestione_Vettore { get; set; }
        public int? Ritenuta_Acconto_Cod { get; set; }
        public double? Ritenuta_Acconto { get; set; }
        public int? Enasarco_Cod { get; set; }
        public double? Enasarco { get; set; }
        public int? ACCDAA_Cod_RisUm_Destinatario { get; set; }
        public int? ACCDAA_Cod_RisUm_Destinazione { get; set; }
        public int? ACCDAA_Cod_IndirizzoRisUm_Destinatario { get; set; }
        public int? ACCDAA_Cod_IndirizzoRisUm_Destinazione { get; set; }
        public int? CapoArea_Cod { get; set; }
        public double? Provvigione_CapoArea { get; set; }
        public double? Provvigione_Pagata_Agente { get; set; }
        public double? Provvigione_Pagata_CapoArea { get; set; }
        public string N_Doc_Cliente { get; set; }
        public DateTime? Data_Doc_Cliente { get; set; }
        public string N_Nota_Fattura { get; set; }
        public DateTime? Data_Nota_Fattura { get; set; }
        public string N_Nota_DDT { get; set; }
        public string N_Nota_Riga_DDT { get; set; }
        public DateTime? Data_Nota_DDT { get; set; }
        public int? Causale_Fattura { get; set; }
        public string N_Doc_Ente { get; set; }
        public int? Anno_Doc_Ente { get; set; }
        public int? Num_Conf_Riscontrate { get; set; }
        public int? Num_Colli_Riscontrati { get; set; }
        public int? Num_Imballi_Riscontrati { get; set; }
        public double? Peso_Netto_Riscontrato { get; set; }
        public double? Peso_Lordo_Riscontrato { get; set; }
        public double? Tara_Unit_Conf_Riscontrata { get; set; }
        public double? Tara_Unit_Collo_Riscontrata { get; set; }
        public double? Tara_Unit_Imballo_Riscontrata { get; set; }
        public int? Distanza_Trasporto_Udm { get; set; }
        public double? Distanza_Trasporto { get; set; }

        public int? Inviato { get; set; }
        public DateTime? Data_Invio { get; set; }

        public DateTime Validita_Inizio { get; set; }
        public DateTime Validita_Fine { get; set; }

        public WriteMovDettTecnicoExtra() { }
    }

    public class MovDettTecnicoExtraRow : DataRowWrapper
    {
        public MovDettTecnicoExtraRow(DataRow row) : base(row) { }
    }
}
