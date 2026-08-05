using System;
using System.Data;

namespace InData.Zoo
{
    public class WriteZooAnimali
    {
        public string Piva { get; set; }
        public int? Sa_Cod { get; set; }
        public int Cod_Progetto { get; set; }

        public string Sesso { get; set; }
        public string Matricola { get; set; }
        public int? Gen_Cod { get; set; }
        public int? Spe_Cod { get; set; }
        public int? IPro_Cod { get; set; }
        public int? Raz_Cod { get; set; }

        public string Nome { get; set; }
        public string Collare { get; set; }
        public string Nome_Aia { get; set; }
        public string Matricola_Aia { get; set; }
        public DateTime? Dat_Nascita { get; set; }
        public string Prov_Nascita { get; set; }
        public string Stato_Nascita { get; set; }
        public string Aua_Azi_Nascita { get; set; }
        public string Ausl_Azi_Nascita { get; set; }
        public string Mat_Padre { get; set; }
        public string Mat_Madre { get; set; }
        public string Cf_Proprietario { get; set; }
        public string Cf_Detentore { get; set; }
        public int? Presente { get; set; }
        public int? Cat_Cod { get; set; }
        public double? Peso { get; set; }
        public DateTime? Data_Pesa { get; set; }
        public int? Metodo_Produzione { get; set; }
        public int? Regolamento_Cod { get; set; }
        public DateTime? Conversione_Inizio { get; set; }
        public DateTime? Conversione_Fine { get; set; }
        public int? Chk_Batteria { get; set; }
        public string CodZootecnica { get; set; }
        public string DescrZootecnica { get; set; }
        public string Fonte { get; set; }
        public string Id_Utente { get; set; }
        public DateTime? Dt_Variazione { get; set; }
        public int? Tipo_Cod { get; set; }
        public string Cf_Fornitore { get; set; }
        public string Lotto_Fornitore { get; set; }
        public string Progetto { get; set; }
        public int? Cod_Progetto_Padre { get; set; }
        public int? Cod_Progetto_Madre { get; set; }
        public int? Id_Capo_BDN { get; set; }
        public string Certificato { get; set; }
        public string Modello4_Ingresso { get; set; }
        public string Modello4_Uscita { get; set; }
        public int? Razza_Madre { get; set; }
        public int? Razza_Padre { get; set; }
        public string Modello4_Ingresso_Numero { get; set; }
        public string Modello4_Uscita_Numero { get; set; }
        public string Modello4_Ingresso_Prenotazione { get; set; }
        public string Modello4_Uscita_Prenotazione { get; set; }
        public string Codice_Azienda_Uscita { get; set; }
        public DateTime? Data_Documento_Ingresso { get; set; }
        public DateTime? Data_Documento_Uscita { get; set; }
        public string N_Bolla_Fornitore { get; set; }
        public string N_Bolla_Uscita { get; set; }
        public int? Causale_Morte { get; set; }
        public string Fornitore_Provenienza { get; set; }
        public string Codice_Azienda_Fornitore { get; set; }
        public DateTime? Data_Ddt_Ingresso { get; set; }
        public DateTime? Data_Ddt_Uscita { get; set; }
        public string Stalla_Svezzamento { get; set; }
        public int? Id_Patologia { get; set; }
        public string Note { get; set; }
        public int? Validato { get; set; }
        public string Anomalie_Note { get; set; }
        public double? Incremento_Teorico { get; set; }

        public int? Inviato { get; set; }
        public DateTime? Data_Invio { get; set; }

        public DateTime Validita_Inizio { get; set; }
        public DateTime Validita_Fine { get; set; }

        public WriteZooAnimali() { }
    }

    public class ZooAnimaliRow : DataRowWrapper
    {
        public ZooAnimaliRow(DataRow row) : base(row) { }
    }
}
