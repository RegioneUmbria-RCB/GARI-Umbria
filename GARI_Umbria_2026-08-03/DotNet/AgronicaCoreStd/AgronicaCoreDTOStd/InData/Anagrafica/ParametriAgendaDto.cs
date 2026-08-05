
using AgronicaCoreDTOStd.Identity;
using InData.Agenda;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace InData.Anagrafica
{
    public class ParametriAgendaDto
    {
        private WrappedParametriAgenda _WParametriAgenda { get; set; }
        private bool _UsaSession { get; set; }
        public int SitoOrigine { get; set; }
        public int PaginaSitoOrigine { get; set; }
        public string Cau_Mov { get; set; } 
        public string Mac_Cod { get; set; }
        public string Lav_Des { get; set; }
        public string SaNome { get; set; }
        public string RagSoc { get; set; }
        public List<Movimento> Movimenti { get; set; }


        public bool TornaASitoOrigine { get; set; }

        public string TrappolaUso { get; set; }

        public int PaginaDestinazioneFiltrino { get; set; }

        public int SitoDestinazioneFiltrino { get; set; }


        public string QueryStringFiltrino { get; set; }


        public string WS_Fitofarmaci_AgroWS_Fitofarmaci { get; set; }


        public string WS_Disciplinari_AgroWS_Disciplinari { get; set; }


        public DateTime Data { get; set; }


        public string Piva { get; set; }


        public string Sa_Cod { get; set; }


        public string Appezza { get; set; }


        public string Id_Imp { get; set; }


        public string Campo_Cod { get; set; }


        public string TipoOperazioneAgenda { get; set; }


        public enum_Tipo_Operazione_Agenda_Target TargetOperazione { get; set; }


        public enum_TipoRicetta TipoRicetta { get; set; }

        public int Programmazione_Cod { get; set; }


        public string TipoOperazioneColturale { get; set; }


        public string GruppoOperazioneColturale { get; set; }


        public string Des_lib { get; set; }


        public string Lav_Cod { get; set; }


        public string Elem_Cod { get; set; }


        public string Id_Agenda { get; set; }

        public string Raccoglitore_Cod { get; set; }

        public string Tipo_Operazione { get; set; }


        public string Veg_Cod { get; set; }


        public string Cul_Cod { get; set; }


        public string Fabbricato { get; set; }


        public string Disciplinare { get; set; }


        public string FiltroRicerca { get; set; }


        public List<ImpiantoDTO> Impianti { get; set; }

        public List<Nota> Note { get; set; }


        public bool OperazioneMulticentro { get; set; }

        public ParametriInstallazioneTrappole InstallazioneTrappola { get; set; }


        public List<rilievoAvv> Rilievi { get; set; }



        public List<Particella> Particelle { get; set; }


        public List<int> ClassiTessitura { get; set; }


        public string Lotto { get; set; }

        public string Cod_Contatto { get; set; }


        public string Piva_Origine { get; set; }


        public int Qualifica_Cod { get; set; }


        public int Tariffa_Cod { get; set; }


        public string LinkAgronicaAgenda2010 { get; set; }


        public string Raggruppamento_Cod { get; set; }


        public string Cod_Progetto { get; set; }


        public string StrGenericaXlinkGiasOnline { get; set; }

        public string RedirectUrl { get; set; }

    }
    public enum enum_Tipo_Operazione_Agenda_Target
    {
        Reale = 1,
        Planning = 2
    }
    public enum enum_Tipo_Operazione_Agenda
    {
        QuadernoDiCampagna = 1,
        Ricetta = 2,
        RicettaBrogliaccio = 3
    }

    public enum enum_TipoRicetta
    {
        Non_Filtrare = -1,
        Standard = 0,
        Costi = 1,
        PUA = 2,
        Budget_Globale = 3,
        Budget_Utente = 4,
        Standard_Destinazioni = 5,
        PianoDistribuzioneConcimi = 6,
        ControlloDiGestione = 7,
        Standard_Destinazioni_Planning = 8,
        PianoDistribuzionePua = 9,
        RichiestaUMA = 10, 
        Zoo = 11
    }
    class WrappedParametriAgenda
    {
        private enum_Tipo_Operazione_Agenda _TipoOperazioneAgenda { get; set; }
        private enum_Tipo_Operazione_Agenda_Target _TargetOperazione { get; set; }
        private enum_TipoRicetta _TipoRicetta { get; set; }

        private int _programmazione_Cod { get; set; }

        private bool _TornaASitoOrigine { get; set; }
        private int _SitoOrigine { get; set; }
        private int _PaginaSitoOrigine { get; set; }

        private int _Mac_Cod { get; set; }
        private DateTime _Data { get; set; }
        private string _RagSoc { get; set; }
        private string _SaNome { get; set; }
        private string _Piva { get; set; }
        private string _Sa_Cod { get; set; }
        private string _Lav_Cod { get; set; }
        private string _Lav_Des { get; set; }
        private string _Cau_Mov { get; set; }
        private string _Elem_Cod { get; set; }
        private string _Id_Agenda { get; set; }
        private string _Raccoglitore_Cod { get; set; }
        private string _Tipo_Operazione { get; set; }
        private string _Veg_Cod { get; set; }
        private string _Cul_Cod { get; set; }
        private string _Fabbricato { get; set; }
        private string _Disciplinare { get; set; }
        private string _FiltroRicerca { get; set; }
        private bool _OperazioneMulticentro { get; set; }
        private string _WS_Disciplinari_AgroWS_Disciplinari { get; set; }
        private string _WS_Fitofarmaci_AgroWS_Fitofarmaci { get; set; }

        private string _TipoOperazioneColturale { get; set; }
        private string _GruppoOperazioneColturale { get; set; }

        private List<ImpiantoDTO> _Impianti { get; set; }
        private List<CapoAnimale> _CapiAnimali { get; set; }
        private IList<Nota> _Note { get; set; }
        private List<Movimento> _Movimenti { get; set; }

        private string _TrappolaUso { get; set; }
        private ParametriInstallazioneTrappole _InstallazioneTrappola { get; set; }
        private List<rilievoAvv> _Rilievi { get; set; }

        private string _QueryStringFiltrino { get; set; }
        private int _PaginaDestinazioneFiltrino { get; set; }
        private int _SitoDestinazioneFiltrino { get; set; }

        private string _Des_Lib { get; set; }

        private string _Lotto { get; set; }

        private List<Particella> _Particelle { get; set; }
        private List<int> _ClassiTessitura { get; set; }

        private string _Appezza { get; set; }
        private string _Id_Imp { get; set; }

        private string _Campo_Cod { get; set; }
        private string _Cod_Contatto { get; set; }

        private string _Piva_Origine { get; set; }

        private int _Qualifica_Cod { get; set; }
        private int _Tariffa_Cod { get; set; }

        private string _LinkAgronicaAgenda2010 { get; set; }
        private int _Raggruppamento_Cod { get; set; }
        private int _Cod_Progetto { get; set; }
        private string _StrGenericaxlinkGiasOnline { get; set; }
        private string _redirectUrl { get; set; }

    }
    public class rilievoAvv
    {
        private string _mov_destinazioni_graphickey { get; set; }
        private int _av_cod { get; set; }
        private int _udm_cod { get; set; }
        private string _valore { get; set; }
        private string _lav_cod { get; set; }
        //private AgronicaCoreModello.ParametriAgenda_Temp.Impianto _impianto = new AgronicaCoreModello.ParametriAgenda_Temp.Impianto();
    }
    public class Particella
    {
        private int _Part_Cod { get; set; }
        private string _Provincia { get; set; }
        private string _Comune { get; set; }
        private string _Sezione { get; set; }
        private int _Foglio { get; set; }
        private int _Numero { get; set; }
        private string _Subalterno { get; set; }
    }
    public class ParametriInstallazioneTrappole
    {

        // parametri per installazione trappole
        private int _NumeroTrappole;
        private int _CodiceProdotto;
        private int _CodiceDitta;
        private decimal _Freatimetro;
        private string _SiglaAv;
        private int _AvCod;
        private int _NumeroInneschi;
        // lista degli appezzamenti coinvolti nell'operazione agenda
        private Hashtable _Appezzamenti_X_NumTrappole;
        // elenco trappele (numero-nome personalizzato)
        private Hashtable _Trappole_X_Nome;
        // Coppia trappola - appezzamento che la contiene
        private Hashtable _Trappole_X_Appezzamento;
        // Coppia innesco - trappola che la contiene che la contiene
        private Hashtable _Trappole_X_NumInneschi;
    }

    public class Nota
    {
        public int Id_Agenda { get; set; }

        public int Nota_Cod { get; set; }

        public DateTime Data_Creazione { get; set; }

        public DateTime Data_Modifica { get; set; }

        public string Username_Creazione { get; set; }

        public string Username_Modifica { get; set; }
    }

    public class Movimento
    {
        public List<Movimento_Dettaglio_Tecnico> Movimenti_Dettagli_Tecnici;
        public List<Movimento_Dettaglio_Tecnico_Extra> Movimenti_Dettagli_Tecnici_Extra;
        public List<Movimento_Dettaglio> Movimenti_Dettagli;
        public List<Pagamento> Pagamenti;

        public int Lav_Cod { get; set; }

        public string Piva { get; set; }

        public int Sa_Cod { get; set; }

        public int Id_Agenda { get; set; }

        public int Id_Mov { get; set; }

        public int Cod_Risum { get; set; }

        public string Cau_Mov { get; set; }

        public string Mov_Desc { get; set; }

        public DateTime Data { get; set; }

        public DateTime Scadenza { get; set; }

        public decimal Doc_Numero { get; set; }

        public int Num_Protocollo { get; set; }

        public decimal Num_Protocollo_Decimal { get; set; }

        public int Cod_IndirizzoRisUm { get; set; }

        public int Cod_Destinazione { get; set; }

        public int Cod_IndirizzoDestinazione { get; set; }

        public int TipoDocumento { get; set; }

        public int Mezzo { get; set; }

        public int Cod_Vettore { get; set; }

        public int Cod_IndirizzoVettore { get; set; }

        public string Causale_Trasporto { get; set; }

        public string Aspetto { get; set; }

        public decimal Peso { get; set; }

        public DateTime Ora { get; set; }
        public DateTime OraFine { get; set; }

        public int Colli { get; set; }

        public string Extra_Str { get; set; }

        public int Extra_Int { get; set; }

        public DateTime Extra_Date { get; set; }

        public int Tipo_Sconto { get; set; }

        public string Doc_Numero_Des { get; set; }

        public string Natura_Beni { get; set; }

        public decimal Tara_Veicolo { get; set; }

        public decimal Tara_Imballi { get; set; }

        public int Tipo_Peso { get; set; }

        public int Modalita { get; set; }
        public int Modalita_Applicazione { get; set; }

        public string Username_Note { get; set; }

        public DateTime Scadenza_Extra { get; set; }

        public string Doc_Numero_Sin { get; set; }

        public int Progr_Protocollo { get; set; }

        public int Progr_Registrazione { get; set; }

        public DateTime Data_Registrazione { get; set; }

        public int ChkLayOut_Bypass_Fatturato { get; set; }

        public int ChkLayOut_Join_Prodotti { get; set; }

        public int Cod_RisUm_Altro { get; set; }

        public int ChkLayOut_Peso { get; set; }

        public int ChkLayOut_Prezzo { get; set; }

        public int ChkFiltro_Varietale { get; set; }

        public int Disciplinare_PubblicoPrivato { get; set; }

        public int Sezionale_Cod { get; set; }

        public int Causale_Trasporto_Cod { get; set; }

        public int ChkLayOut_Litri { get; set; }

        public int Cod_RisUm_Aggiuntivo { get; set; }

        public int Cod_Indirizzo_Aggiuntivo { get; set; }

        public int ChkLayOut_Riscontrato { get; set; }

        public string Doc_Numero_Visualizzato { get; set; }

        public string Cod_Macchina_Lav { get; set; }

        public DateTime Data_Creazione { get; set; }

        public DateTime Data_Modifica { get; set; }

        public string Username_Creazione { get; set; }

        public string Username_Modifica { get; set; }

        public int TopCode { get; set; }
        public int BaseCode { get; set; }

        public WriteMovimenti ToWriteMovimenti()
        {
            WriteMovimenti wobj = new WriteMovimenti();
            wobj.Piva = Piva;
            wobj.Sa_Cod = Sa_Cod;
            wobj.Id_Agenda = Id_Agenda;
            wobj.Id_Mov = Id_Mov;
            wobj.Cod_RisUm = Cod_Risum;
            wobj.Cau_Mov = Cau_Mov != null ? int.Parse(Cau_Mov) : 0;
            wobj.Mov_Desc = Mov_Desc;
            wobj.Data_Movimento = Data;
            wobj.Scadenza = Scadenza;
            wobj.Scadenza_Extra = Scadenza_Extra;
            wobj.Doc_Numero = (int)Doc_Numero;
            wobj.Num_Protocollo = Num_Protocollo;
            wobj.Cod_IndirizzoRisUm = Cod_IndirizzoRisUm;
            wobj.Cod_Destinazione = Cod_Destinazione;
            wobj.Cod_IndirizzoDestinazione = Cod_IndirizzoDestinazione;
            wobj.Mezzo = Mezzo;
            wobj.Cod_Vettore = Cod_Vettore;
            wobj.Cod_IndirizzoVettore = Cod_IndirizzoVettore;
            wobj.Causale_Trasporto = Causale_Trasporto != null ? int.Parse(Causale_Trasporto) : 0;
            wobj.Aspetto = Aspetto;
            wobj.Peso = (float)Peso;
            wobj.Ora = Ora;
            wobj.Colli = Colli;
            wobj.Tipo_Sconto = Tipo_Sconto;
            wobj.Extra_Str = Extra_Str;
            wobj.Extra_Int = Extra_Int;
            wobj.Extra_Date = Extra_Date;
            wobj.Doc_Numero_Sin = Doc_Numero_Sin;
            wobj.Doc_Numero_Des = Doc_Numero_Des;
            wobj.Natura_Beni = Natura_Beni;
            wobj.Tara_Veicolo = (int)Tara_Veicolo;
            wobj.Tara_Imballi = (int)Tara_Imballi;
            wobj.Tipo_Peso = Tipo_Peso;
            wobj.Modalita = Modalita;
            wobj.Username_Note = Username_Note;
            wobj.Progr_Protocollo = Progr_Protocollo;
            wobj.Progr_Registrazione = Progr_Registrazione;
            wobj.Data_Registrazione = Data_Registrazione;
            wobj.ChkLayOut_Bypass_Fatturato = ChkLayOut_Bypass_Fatturato;
            wobj.ChkLayOut_Join_Prodotti = ChkLayOut_Join_Prodotti;
            wobj.Validita_Inizio = Data;
            wobj.Disciplinare_PubblicoPrivato = Disciplinare_PubblicoPrivato;
            wobj.Sezionale_Cod = Sezionale_Cod;
            wobj.Causale_Trasporto_Cod = Causale_Trasporto_Cod;
            wobj.Cod_RisUm_Altro = Cod_RisUm_Altro;
            wobj.ChkLayOut_Peso = ChkLayOut_Peso;
            wobj.ChkLayOut_Prezzo = ChkLayOut_Prezzo;
            wobj.ChkFiltro_Varietale = ChkFiltro_Varietale;
            wobj.ChkLayOut_Litri = ChkLayOut_Litri;
            wobj.Cod_RisUm_Aggiuntivo = Cod_RisUm_Aggiuntivo;
            wobj.Cod_Indirizzo_Aggiuntivo = Cod_Indirizzo_Aggiuntivo;
            wobj.ChkLayOut_Riscontrato = ChkLayOut_Riscontrato;
            wobj.Doc_Numero_Visualizzato = Doc_Numero_Visualizzato;
            wobj.Cod_Macchina_Lav = Cod_Macchina_Lav;
            wobj.TipoDocumento = TipoDocumento;
            wobj.OraFine = OraFine;
            wobj.Modalita_Applicazione = Modalita_Applicazione;
            return wobj;
        }
    }

    public class Movimento_Dettaglio_Tecnico
    {
        public string Inn2_data { get; set; }

        public int Nitrati { get; set; }

        public int Parziale { get; set; }

        public int Piezo1 { get; set; }

        public int Piezo2 { get; set; }

        public int Piezo3 { get; set; }

        public int Piezo4 { get; set; }

        public int Soglia_Cod { get; set; }

        public decimal Soglia_Quantita { get; set; }

        public string Soglia_Des { get; set; }

        public int ff_classe { get; set; }

        public string Piva { get; set; }

        public int Sa_Cod { get; set; }

        public int Id_Agenda { get; set; }

        public int Id_Mov { get; set; }

        public int Id_Mov_Det { get; set; }

        public int Id_Reg_Dettaglio { get; set; }

        public DateTime Data { get; set; }

        public decimal Qta_Ril { get; set; }

        public int Id_Insetto { get; set; }

        public int Av_Cod { get; set; }

        public int Av_Gru { get; set; }

        public decimal N { get; set; }

        public decimal P { get; set; }

        public decimal K { get; set; }

        public decimal M { get; set; }

        public decimal Cu { get; set; }

        public decimal Efficienza { get; set; }

        public int TopCode { get; set; }

        public int BaseCode { get; set; }

        public int Trap_num { get; set; }

        public int Ditta_cod { get; set; }

        public decimal Dose { get; set; }

        public decimal Freatimetro { get; set; }

        public DateTime Inn1_data { get; set; }

        public string Sigla_av { get; set; }

        public string ExtraStr { get; set; }
        public int Extra_Int { get; set; }
        public DateTime Extra_Date { get; set; }

        public int dett_cod { get; set; }

        public string Data_Ril { get; set; }

        public DateTime Data_Creazione { get; set; }

        public DateTime Data_Modifica { get; set; }

        public string Username_Creazione { get; set; }

        public string Username_Modifica { get; set; }
    }
    public class Pagamento
    {
        public string Piva { get; set; }

        public int Sa_Cod { get; set; }

        public int Id_Agenda { get; set; }

        public int Id_Mov { get; set; }

        public int Cod_Pagamento { get; set; }

        public decimal Importo { get; set; }

        public decimal Percentuale { get; set; }

        public DateTime Data_Pagamento { get; set; }

        public string Note { get; set; }

        public string Cau_Risorsa { get; set; }

        public int Cod_Liquidita_Dare { get; set; }

        public int Cod_Liquidita_Avere { get; set; }

        public string Cau_Pagamento { get; set; }

        public int Extra_Int { get; set; }

        public string Extra_Str { get; set; }

        public DateTime Extra_Date { get; set; }

        public DateTime Validita_Inizio { get; set; }

        public DateTime Validita_Fine { get; set; }

        public int Cod_Conto_Dare { get; set; }

        public int Cod_Conto_Avere { get; set; }

        public int Cod_Conto_Pat_Dare { get; set; }

        public int Cod_Conto_Pat_Avere { get; set; }

        public int Tipo_Dare { get; set; }

        public int Tipo_Avere { get; set; }

        public int Tipo_Cod_Dare { get; set; }

        public int Tipo_Cod_Avere { get; set; }

        public int Anno { get; set; }

        public int Ric_Cod { get; set; }

        public int Ric_Cod_Pat { get; set; }

        public int Previsto_Avvenuto { get; set; }

        public int cbi_causale { get; set; }

        public int ChkDataScadenza_Manuale { get; set; }

        public DateTime DataScadenza_Manuale { get; set; }

        public DateTime Data_Creazione { get; set; }

        public DateTime Data_Modifica { get; set; }

        public string Username_Creazione { get; set; }

        public string Username_Modifica { get; set; }

        public int TopCode { get; set; }

        public int BaseCode { get; set; }
    }

    public class Movimento_Dettaglio_Tecnico_Extra
    {
        public int TopCode { get; set; }

        public int BaseCode { get; set; }

        public string Piva { get; set; }

        public int Sa_Cod { get; set; }

        public int Id_Agenda { get; set; }

        public int Id_Mov { get; set; }

        public int Id_Mov_Det { get; set; }

        public int Id_Reg_Dettaglio { get; set; }

        public string Regione { get; set; }

        public string ASL { get; set; }

        public string Serie { get; set; }

        public string Numero { get; set; }

        public int Mac_Cod { get; set; }

        public int Cod_RisUm { get; set; }

        public string Trasportatore { get; set; }

        public string Mezzo_Trasporto { get; set; }

        public string Targa { get; set; }

        public string N_Immatricolazione { get; set; }

        public string N_Immatricolazione_Rimorchio { get; set; }

        public string N_Autorizzazione_Trasporto { get; set; }

        public DateTime Data_Rilascio_Autorizzazione { get; set; }

        public decimal Peso { get; set; }

        public int Codice_Prodotto { get; set; }

        public int Colore { get; set; }

        public string Zona_Viticola { get; set; }

        public int Manipolazioni { get; set; }

        public string Precisazioni { get; set; }

        public string Annotazioni { get; set; }

        public int Num_Contenitori { get; set; }

        public string Marche_Contenitori { get; set; }

        public string Des_Contenitori { get; set; }

        public string Tipo_Documento { get; set; }

        public int Id_Cod_Autorita { get; set; }

        public string Luogo_Partenza { get; set; }

        public string Luogo_Consegna { get; set; }

        public DateTime Data_Spedizione { get; set; }

        public string Indicazioni_Complementari { get; set; }

        public decimal Titolo_Alcol { get; set; }

        public string Codice_NC { get; set; }

        public string Num_Riferimento { get; set; }

        public DateTime Data_Dichiarazione { get; set; }

        public string Garanzia { get; set; }

        public string Certificati { get; set; }

        public string Durata_Viaggio { get; set; }

        public decimal Peso_Lordo { get; set; }

        public int Num_Colli { get; set; }

        public int Contenitore_Cod { get; set; }

        public int Imballaggio_Cod { get; set; }

        public int Agente_Cod { get; set; }

        public decimal Provvigione { get; set; }

        public int Tipo_Trasporto { get; set; }

        public int Unita_Trasporto { get; set; }

        public string Codice_Alternativo { get; set; }

        public int Id_Gestione_Vettore { get; set; }

        public int Ritenuta_Acconto_Cod { get; set; }

        public decimal Ritenuta_Acconto { get; set; }

        public int Enasarco_Cod { get; set; }

        public decimal Enasarco { get; set; }

        public int ACCDAA_Cod_Risum_Destinatario { get; set; }

        public int ACCDAA_Cod_Risum_Destinazione { get; set; }

        public int ACCDAA_Cod_IndirizzoRisum_Destinatario { get; set; }

        public int ACCDAA_Cod_IndirizzoRisum_Destinazione { get; set; }

        public int CapoArea_Cod { get; set; }

        public decimal Provvigione_CapoArea { get; set; }

        public decimal Provvigione_Pagata_Agente { get; set; }

        public decimal Provvigione_Pagata_CapoArea { get; set; }

        public string N_Doc_Cliente { get; set; }

        public DateTime Data_Doc_Cliente { get; set; }

        public string N_Doc_Ente { get; set; }

        public int Anno_Doc_Ente { get; set; }

        public int Num_Conf_Riscontrate { get; set; }

        public int Num_Colli_Riscontrati { get; set; }

        public int Num_Imballi_Riscontrati { get; set; }

        public decimal Peso_Netto_Riscontrato { get; set; }

        public decimal Peso_Lordo_Riscontrato { get; set; }

        public decimal Tara_Unit_Conf_Riscontrata { get; set; }

        public decimal Tara_Unit_Collo_Riscontrata { get; set; }

        public decimal Tara_Unit_Imballo_Riscontrata { get; set; }

        public string N_Nota_Fattura { get; set; }

        public DateTime Data_Nota_Fattura { get; set; }

        public string N_Nota_DDT { get; set; }

        public string N_Nota_Riga_DDT { get; set; }

        public DateTime Data_Nota_DDT { get; set; }

        public int Causale_Fattura { get; set; }
        
        public DateTime Validita_Inizio { get; set; }

        public DateTime Validita_Fine { get; set; }

        public DateTime Data_Creazione { get; set; }

        public DateTime Data_Modifica { get; set; }

        public string Username_Creazione { get; set; }

        public string Username_Modifica { get; set; }
    }
    public class Movimento_Dettaglio
    {
        public int Lav_Cod { get; set; }
        public string Cau_Mov { get; set; }

        public string Piva { get; set; }
        public int Sa_Cod { get; set; }
        public int Id_Agenda { get; set; }
        public int Id_Mov { get; set; }
        public int Id_Mov_Det { get; set; }

        public DateTime Data { get; set; }

        public int Elem_Cod { get; set; }

        public int Pro_Cod { get; set; }

        public int Mat_Cod { get; set; }

        public string Mov_Det_Des { get; set; }

        public int Udm_Cod { get; set; }

        public decimal Qta { get; set; }

        public int Cod_Iva { get; set; }

        public decimal Sconto { get; set; }

        public decimal Prezzo_Unitario { get; set; }

        public int Cod_Conto { get; set; }

        public int Cod_Progetto { get; set; }

        public int Fase_Cod { get; set; }

        public int Contabilizzato { get; set; }

        public int Pendente { get; set; }

        public DateTime Validita_Inizio { get; set; }

        public DateTime Validita_Fine { get; set; }

        public int Cal_Cod { get; set; }

        public string Extra_Str { get; set; }

        public int Extra_Int { get; set; }

        public DateTime Extra_Date { get; set; }

        public int Anno { get; set; }

        public int Ric_Cod { get; set; }

        public decimal Imponibile { get; set; }

        public decimal Iva { get; set; }

        public string Lotto { get; set; }

        public int Jolly_Int { get; set; }

        public decimal Imponibile_Netto { get; set; }

        public decimal Prezzo_Unitario_Netto { get; set; }

        public int Udm_Cod_Extra { get; set; }

        public decimal Qta_Extra { get; set; }

        public decimal Prezzo_Effettivo { get; set; }

        public int ChkIva_Manuale { get; set; }

        public int Cod_IvaIndetraibile { get; set; }

        public decimal Qta_Extra_Totale { get; set; }

        public decimal Tara { get; set; }

        public int ChkLayOut_Hide { get; set; }

        public decimal Variazione { get; set; }

        public int Listino_Cod { get; set; }

        public decimal Sconto_Listino { get; set; }

        public int Sconto_Modalita { get; set; }

        public int Mat_Cod_Alias { get; set; }

        public int Mezzo_Det { get; set; }

        public string Sconto_Testo { get; set; }

        public int Ric_Cod_Pat { get; set; }

        public int Cod_Conto_Pat { get; set; }

        public int TempoCarenza { get; set; }

        public string DoseEtichetta { get; set; }

        public int Turno_Cod { get; set; }

        public int ID_Attivita { get; set; }

        public int Veg_Cod { get; set; }

        public decimal Iva_Indetraibile { get; set; }

        public decimal Iva_Indetraibile_Perc { get; set; }

        public string PrincipiAttivi { get; set; }
        public string PrincipiAttiviPesi { get; set; }
        public string Buffer { get; set; }

        public string CLassiTossicologiche { get; set; }

        public string DoseEtichetta_Value { get; set; }

        public int Iva_Deto_Cod { get; set; }

        public decimal Qta_Dettaglio1 { get; set; }

        public decimal Qta_Dettaglio2 { get; set; }

        public int Dettagli_Blocco_Flag { get; set; }

        public string Dettagli_Blocco_Username { get; set; }

        public DateTime Dettagli_Blocco_Data { get; set; }

        public int Qualifica_Cod { get; set; }

        public int Tariffa_Cod { get; set; }

        public int Ordine_Det { get; set; }

        public int Deroga_Cod { get; set; }

        public int Prezzo_Livello { get; set; }

        public string Rif_Esterno { get; set; }
        public string Rif_Esterno_2 { get; set; }

        public decimal Importo { get; set; }

        public DateTime Data_Creazione { get; set; }

        public DateTime Data_Modifica { get; set; }

        public string Username_Creazione { get; set; }

        public string Username_Modifica { get; set; }

        public int TopCode { get; set; }

        public int BaseCode { get; set; }

        public string PrincipiAttiviPercAbb { get; set; }

        public List<Movimento_Dettaglio_Tecnico> Movimenti_Dettagli_Tecnici { get; set; }

        public List<Movimento_Dettaglio_Tecnico_Extra> Movimenti_Dettagli_Tecnici_Extra { get; set; }

        public List<Movimento_Dettaglio_Conferimento> Movimenti_Dettagli_Conferimento { get; set; }

        public List<Movimento_Destinazione> Movimenti_Destinazioni { get; set; }

        public List<Movimento_Dettaglio> Movimenti_Dettagli_Riferiti { get; set; }

        public List<Movimento_Dettaglio_Riferimento> Movimenti_Dettagli_Riferimenti { get; set; }

        public List<Materia_Prima_Campionatura> Materie_Prime_Campionature { get; set; }

        public int Polverulento { get; set; }

        public WriteMovDettagli ToWriteMovDettagli()
        {
            WriteMovDettagli wobj = new WriteMovDettagli();
            wobj.Piva = Piva;
            wobj.Sa_Cod = Sa_Cod;
            wobj.Id_Agenda = Id_Agenda;
            wobj.Id_Mov = Id_Mov;
            wobj.Id_Mov_Det = Id_Mov_Det;
            wobj.Elem_Cod = Elem_Cod;
            wobj.Pro_Cod = Pro_Cod;
            wobj.Mat_Cod = Mat_Cod;
            wobj.Mov_Det_Des = Mov_Det_Des;
            wobj.Qta = (double)Qta;
            wobj.Udm_Cod = Udm_Cod;
            wobj.Cod_Iva = Cod_Iva;
            wobj.Jolly_Int = Jolly_Int;
            wobj.Sconto = (double)Sconto;
            wobj.Prezzo_Unitario = (double)Prezzo_Unitario;
            wobj.Prezzo_Unitario_Netto = (double)Prezzo_Unitario_Netto;
            wobj.Cal_Cod = Cal_Cod;
            wobj.Cod_Progetto = Cod_Progetto;
            wobj.Fase_Cod = Fase_Cod;
            wobj.Extra_Int = Extra_Int;
            wobj.Extra_Str = Extra_Str;
            wobj.Extra_Date = Extra_Date;
            wobj.Ric_Cod = Ric_Cod;
            wobj.Anno = Anno;
            wobj.Imponibile = (double)Imponibile;
            wobj.Imponibile_Netto = (double)Imponibile_Netto;
            wobj.Iva = (double)Iva;
            wobj.Listino_Cod = Listino_Cod;
            wobj.Contabilizzato = Contabilizzato;
            wobj.Pendente = Pendente;
            wobj.Lotto = Lotto;
            wobj.UDM_COD_EXTRA = Udm_Cod_Extra;
            wobj.Prezzo_Effettivo = (double)Prezzo_Effettivo;
            wobj.Variazione = (double)Variazione;
            wobj.Tara = (double)Tara;
            wobj.ChkLayOut_Hide = ChkLayOut_Hide;
            wobj.ChkIva_Manuale = ChkIva_Manuale;
            wobj.Cod_IvaIndetraibile = Cod_IvaIndetraibile;
            wobj.TempoCarenza = TempoCarenza;
            wobj.DoseEtichetta = DoseEtichetta;
            wobj.Turno_Cod = Turno_Cod;
            wobj.ID_Attivita = ID_Attivita;
            wobj.PrincipiAttivi = PrincipiAttivi;
            wobj.ClassiTossicologiche = CLassiTossicologiche;
            wobj.DoseEtichetta_Value = DoseEtichetta_Value;
            wobj.Validita_Inizio = Validita_Inizio;
            wobj.Validita_Fine = Validita_Fine;
            wobj.Mat_Cod_Alias = Mat_Cod_Alias;
            wobj.Qta_Dettaglio1 = (double?)Qta_Dettaglio1;
            wobj.Qta_Dettaglio2 = (double?)Qta_Dettaglio2;
            wobj.Sconto_Listino = (double?)Sconto_Listino;
            wobj.Sconto_Modalita = Sconto_Modalita;
            wobj.Sconto_Testo = Sconto_Testo;
            wobj.Qualifica_Cod = Qualifica_Cod;
            wobj.Tariffa_Cod = Tariffa_Cod;
            wobj.Mezzo_Det = Mezzo_Det;
            wobj.Ric_Cod_Pat = Ric_Cod_Pat;
            wobj.Cod_Conto_Pat = Cod_Conto_Pat;
            wobj.Iva_Indetraibile = (double?)Iva_Indetraibile;
            wobj.Iva_Indetraibile_Perc = (double?)Iva_Indetraibile_Perc;
            wobj.Iva_Deto_Cod = Iva_Deto_Cod;
            wobj.Dettagli_Blocco_Flag = Dettagli_Blocco_Flag;
            wobj.Dettagli_Blocco_Username = Dettagli_Blocco_Username;
            wobj.Dettagli_Blocco_Data = Dettagli_Blocco_Data;
            wobj.Ordine_Det = Ordine_Det;
            wobj.Deroga_Cod = Deroga_Cod;
            wobj.Prezzo_Livello = Prezzo_Livello;
            wobj.PrincipiAttiviPesi = PrincipiAttiviPesi;
            wobj.Buffer = Buffer;
            wobj.Rif_Esterno = Rif_Esterno;
            wobj.Rif_Esterno_2 = Rif_Esterno_2;
            wobj.PrincipiAttiviPercAbb = PrincipiAttiviPercAbb;
            wobj.Polverulento = Polverulento;
            return wobj;
        }
    }
    public class Materia_Prima_Campionatura
    {
        public int Progressivo { get; set; }

        public string Tipo { get; set; }

        public int Tipo_Cod { get; set; }

        public int Udm_Cod { get; set; }

        public string Val_Cod { get; set; }

        public string Descrizione { get; set; }

        public DateTime Validita_Inizio { get; set; }

        public DateTime Validita_Fine { get; set; }

        public int Progressivo_Origine { get; set; }

        public string Piva_SuperUser_Origine { get; set; }

        public decimal Peso_Campione { get; set; }

        public int ChkStima { get; set; }

        public int ChkTara_Campionatura { get; set; }

        public decimal Tara_Campionatura { get; set; }

        public DateTime Data_Creazione { get; set; }

        public DateTime Data_Modifica { get; set; }

        public string Username_Creazione { get; set; }

        public string Username_Modifica { get; set; }

        public int TopCode { get; set; }

        public int BaseCode { get; set; }
    }
    public class Movimento_Destinazione
    {
        public int GisTipoEntita_cod { get; set; }
        public int GisLayerCod { get; set; }
        public string GisWkt { get; set; }

        public string GisWktSistemaRiferimento { get; set; }

        public string GisWktGps { get; set; }

        public string Piva { get; set; }

        public int Sa_Cod { get; set; }

        public int Id_Agenda { get; set; }

        public int Id_Mov { get; set; }

        public int Id_Mov_Det { get; set; }

        public int Appezza { get; set; }

        public int Id_Destinazione { get; set; }

        public int Progetto_Cod { get; set; }

        public int Tipo { get; set; }

        public DateTime Data { get; set; }

        public decimal Qta { get; set; }

        public decimal Qta2 { get; set; }

        public int Tipo_Scorta { get; set; }

        public decimal Scorta_Min { get; set; }

        public string mov_destinazioni_graphickey { get; set; }

        public decimal Qta_Dest1 { get; set; }

        public decimal Qta_Dest2 { get; set; }

        public decimal QuotaDistribuzione { get; set; }

        public string parametroGenerico { get; set; }

        public int Programmazione_Entita_Cod { get; set; }

        public DateTime Data_Creazione { get; set; }

        public DateTime Data_Modifica { get; set; }

        public string Username_Creazione { get; set; }

        public string Username_Modifica { get; set; }

        public int TopCode { get; set; }

        public int BaseCode { get; set; }

        public decimal Sup_Riduzione_BufferZone { get; set; }

        public decimal Perc_Riduzione_Deriva { get; set; }

        public WriteMovDestinazioni ToWriteMovDestinazioni()
        {
            WriteMovDestinazioni wobj = new WriteMovDestinazioni();
            wobj.Piva = Piva;
            wobj.Sa_Cod = Sa_Cod;
            wobj.Id_Agenda = Id_Agenda;
            wobj.Id_Mov = Id_Mov;
            wobj.Id_Mov_Det = Id_Mov_Det;
            wobj.Appezza = Appezza;
            wobj.Id_Destinazione = Id_Destinazione;
            wobj.Tipo_Destinazione = Tipo;
            wobj.Qta = (double?)Qta;
            wobj.Qta2 = (double?)Qta2;
            wobj.Tipo_Scorta = Tipo_Scorta;
            wobj.Scorta_Min = (double?)Scorta_Min;
            wobj.Mov_Destinazioni_GraphicKey = mov_destinazioni_graphickey;
            wobj.QuotaDistribuzione = (double)QuotaDistribuzione;
            wobj.Validita_Inizio = this.Data;
            wobj.Qta_Dest1 = (double?)Qta_Dest1;
            wobj.Qta_Dest2 = (double?)Qta_Dest2;
            wobj.Sup_Riduzione_BufferZone = (double?)Sup_Riduzione_BufferZone;
            wobj.Perc_Riduzione_Deriva = (double?)Perc_Riduzione_Deriva;
            return wobj;
        }
    }

    public class Movimento_Dettaglio_Riferimento
    {
        public string Piva { get; set; }

        public int Sa_Cod { get; set; }

        public int Id_Agenda { get; set; }

        public int Id_Mov { get; set; }

        public int Id_Mov_Det { get; set; }

        public int Lav_Cod { get; set; }

        public string Cau_Mov { get; set; }

        public string Piva_Rif { get; set; }

        public int Sa_Cod_Rif { get; set; }

        public int Id_Agenda_Rif { get; set; }

        public int Id_Mov_Rif { get; set; }

        public int Id_Mov_Det_Rif { get; set; }

        public int Lav_Cod_Rif { get; set; }

        public string Cau_Mov_Rif { get; set; }

        public decimal Qta { get; set; }

        public DateTime Data_Creazione { get; set; }

        public DateTime Data_Modifica { get; set; }

        public string Username_Creazione { get; set; }

        public string Username_Modifica { get; set; }

        public int TopCode { get; set; }

        public int BaseCode { get; set; }

        public int Preserva_Legame { get; set; }

        public int Tipo_Associazione { get; set; }
    }

    public class Movimento_Dettaglio_Conferimento
    {
        public int TopCode { get; set; }
        public int BaseCode { get; set; }

        public string Piva { get; set; }
        public int Sa_Cod { get; set; }
        public int Id_Agenda { get; set; }
        public int Id_Mov { get; set; }
        public int Id_Mov_Det { get; set; }
        public int Id_Reg_Dettaglio { get; set; }

        public string Tagliando_Pesa { get; set; }
        public decimal Premio_Complessivo { get; set; }
        public decimal Prezzo_Unitario_Finale { get; set; }
        public int Cod_Varieta { get; set; }
        public string Desc_Appezzamenti { get; set; }

        public DateTime Validita_Inizio { get; set; }
        public DateTime Validita_Fine { get; set; }

        public DateTime Data_Creazione { get; set; }
        public DateTime Data_Modifica { get; set; }

        public string Username_Creazione { get; set; }
        public string Username_Modifica { get; set; }
    }

    public class ImpiantoDTO
    {
        public enum_Tipo_Operazione_Agenda_Target Reale_Or_Planning;

        public int Programmazione_Entita_cod;

        public string Piva;
        public int Sa_Cod;
        public int Appezza;
        public int Campo_Cod;
        public int ID_Reg;

        public string App_Nome;
        public string Campo_Des;

        public int Progetto_Cod;
        public decimal Sup_Imp;
        public DateTime Validita_Inizio_Distinta;
        public DateTime Validita_Fine_Distinta;
        public string Cul_Des;
        public DateTime Data_Raccolta;
        public DateTime Data_Raccolta_Prevista;
        public DateTime Data_Fioritura;
        public DateTime Data_Fioritura_Prevista;

        public int Veg_Cod;
        public int Cul_Cod;
        public string Rag_Soc;
        public string Sa_Nome;
        public string Veg_Des;

        public int Cop_Cod;
        public int Grfi_Cod;
        public DateTime Validita_Inizio_Appezzamento;
        public DateTime Validita_Fine_Appezzamento;

        public DateTime Validita_Inizio;
        public DateTime Validita_Fine;

        private int _Udm_Cod;
        private int _Mat_cod;

        private decimal _Qta;
        private decimal _Qta2;

        public decimal _QuotaDistribuzione;

        public string Codici_Anagrafe_Des;

        public string GisWkt = "";
        public string GisWktGps = "";
        public string GisWktSistemaRiferimento = "";
        public int GisTipoEntita_cod = 0;
        public int GisLayerCod = 0;

        public decimal DistBZ_CorpiIdrici;
        public decimal DistBZ_AreeResPub;
        public decimal DistBZ_Allevamenti;
        public decimal DistBZ_VegNatNonColt;
        public decimal SupBZ_Riduzione; // capezzagna
        private decimal _Sup_Riduzione_BufferZone;
        private decimal _Perc_Riduzione_Deriva;

        public List<Particella> ListaParticelle;
        public List<int> ListaClassiTessitura;

        public int Stato_Impianto;
    }
    public class CapoAnimale
    {
        public int Cod_Animale;
    }


}