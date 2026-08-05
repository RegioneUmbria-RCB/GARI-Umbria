using AgronicaCoreModelsSTD.anagrafiche;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.FiltroRicerca.DAL.Utils.Utils
{
    public class Utils
    {
        public static readonly int columnOrder_stdGIAS = 9999;

        //Carichiamo solo movimenti dei seguenti gruppi
        public static readonly List<int> GruppoOperazioniAmmissibili = new List<int> { (int)Enum_GruppoOperazioni.Rilievi_in_Campo, 
                                                                                       (int)Enum_GruppoOperazioni.Rilievi_alla_Raccolta, 
                                                                                       (int)Enum_GruppoOperazioni.Trattamenti, 
                                                                                       (int)Enum_GruppoOperazioni.Lavorazioni };
    }

    public class Enum_DTColumns
    {
        public static readonly string[] FilterOut_Keys = new string[] {
            "piva", "sa_cod", "campo_cod", "appezza", "id_reg", "progetto_cod", "fabbricato_cod"
        };
        public static readonly string[] FilterOut = new string[] {
            "inviato", "datainvio", "username_creazione", "username_modifica",
            "validazione", "data_validazione", "username_validazione",
            "blk_flag", "blk_inizio_data",
            "blk_inizio_username", "blk_inizio_note", "blk_fine_data", "blk_fine_username", "blk_fine_note",
            "id_budget"
        };
        public static readonly string[] Skip_or_ApplyFormatDate = new string[] {
            "data_creazione", "data_modifica", "validita_inizio", "validita_fine"
        };

        /// <summary>
        /// COLONNE dbo.Imprese DA NON ESTRARRE MAI AUTOMATISMO
        /// </summary>
        public static readonly string[] FilterOut_Aziende = new string[]
        {
            //presenti nella select base
            "partitaIvaReale", "rag_soc","tipoimpresagerarchia",
            //colonne da escludere perchè non usate
            "delega", "at_prevalente", "forma_giuridica", "forma_conduzione", "sup_totale", "codesenzione",
            "codop", "documento", "dtdocumento", "dtvalidazione", "esenzionedescr", "flagaltresedi",
            "flagvalidato", "idutentevalidazione", "opdescr", "fonte", "dt_fonte", "aziendacessata",
            "aziendaiscrittacaa", "aziendapresente", "aziendavalidata", "dataiscrizionecaa",
            "datavalidazione", "datavariazioneazienda", "maxdatavariazioneibanazienda",
            "maxdatavariazionepersoneazienda", "maxdatavariazionepossessiazienda",
            //colonne codice, richiedono una descrizione
            "grupporaccolta_cod"
        };
        /// <summary>
        /// COLONNE dbo.Centri_Aziendali DA NON ESTRARRE MAI AUTOMATISMO
        /// </summary>
        public static readonly string[] FilterOut_CentriAziendali = new string[]
        {
            //presenti nella select base
            "sa_nome",
            //colonne da escludere perchè non usate
            "x", "y", "lat", "long", "zslm", "area", "ca_sipi", "at_prevalente", "forma_possesso",
            "titolopossesso", "sup_totale", "sup_bosco", "sup_tare", "sup_sau",
            "sup_prati", "sup_sau_convenzionale", "sup_sau_conversione", "sup_sau_biologico",
            "documento",
            "datadocumento", "flaglegale", "flagprincipale", "fonte", "fontedescr", "datafonte"
        };
        /// <summary>
        /// COLONNE dbo.Campi DA NON ESTRARRE MAI AUTOMATISMO
        /// </summary>
        public static readonly string[] FilterOut_Campi = new string[]
        {
            //presenti nella select base
            "campo_des",
            //colonne da escludere perchè non usate
            "gru_cod", "veg_cod", "sau_totale", "sau_biologico", "sau_conversione", "sau_convenzionale",
            "conversione_inizio", "conversione_fine", "confinirischio", "campo_tipo"
        };
        /// <summary>
        /// COLONNE dbo.Appezzamento DA NON ESTRARRE MAI AUTOMATISMO
        /// </summary>
        public static readonly string[] FilterOut_Appezzamenti = new string[]
        {
            //presenti nella select base
            "app_nome", "sup_app", "x", "y",
            //colonne da escludere perchè non usate
            "data_app", "ep_camp", "zslm", "esposiz", "pende", "ubicazione", "num_del", "clas", "sabbia", "limo", "argilla", "ph",
            "caltot", "calatt", "sostorg", "k2oass", "p2o5ass", "mg", "ntot", "um_s", "cl_dren", "falda",
            "csc", "k2oass_data", "matorg", "matorg_data", "notot_data", "notot", "p2o5ass_data",
            "suolo_codattri", "user", "app_nome", "campo_spia", "campo_spia_area",
            "cs_sipi", "campo_cod", "prossimo", "data_inizio", "data_fine",
            "via_stringa", "distbz_corpiidrici", "distbz_areerespub", "distbz_allevamenti", "distbz_vegnatnoncolt",
            "supbz_riduzione", "altrivitignipresenti"
        };
        /// <summary>
        /// COLONNE dbo.Reg_Impianti DA NON ESTRARRE MAI AUTOMATISMO
        /// </summary>
        public static readonly string[] FilterOut_Impianti = new string[]
        {
            //presenti nella select base
            "cul_cod", "sup_imp", "grva_cod_veg",
            //colonne da escludere perchè non usate
            "data_agg", "cod_resp", "cod_ente", "campo_spia", "data", "grfi_cod", "data_raccolta",
            "produzione", "resa_prevista", "resa_effettiva", "scarto", "ind_mat_cod", "ind_mat_ril", "sta_ter",
            "cop_di", "cop_df", "tra_fila", "su_fila", "p_ha", "foral_cod", "setup_cod", "port_cod", "imp_cod",
            "stru_prot", "pro_pag", "seme_q", "seme_t", "seme_p", "seme_d", "stato_residui", "tecn_cod", "denitrificazione",
            "volatilizzazione", "profonditalav", "id_campo", "su_cod", "cop_cod", "cover", "monitorato",
            "codice_fiscale_tecnico", "user", "regolamento", "finanziamento", "data_conversione", "provenienzaseme",
            "id_consociazione", "unita_vitata", "sovrainnesto_cod", "ancoraggitestata", "annoriferimento", "codfilistostegno",
            "codpalitessitura", "codpalitestata", "codstatocolt", "codtipovari", "dataprotocollo", "datarilievo",
            "destproduttiva", "destproduttivadescr", "distanzapali", "dtfine", "dtfinegestione", "dtinizio",
            "dtiniziogestione", "dtins", "dtvar", "fallanzeperc", "flaganomalia", "flagattuale", "flagcessata",
            "flagcontributo", "flagregolarizz2009", "flagricalcologis", "giacituraterreno", "idunitavitata",
            "idutenteins", "idutentevar", "numeroprotocollo", "progpoligono", "supvitatadich", "supvitatadichprcalcolo",
            "superficieserviziomq", "terrazzamenti", "tipocoltura", "tipoprocedimento", "tipounar", "tipovariazione",
            "unar", "data_inizio_portinnesto", "data_inizio_innesto", "piante_maschi_insesto", "data_inizio_produzione",
            "udm_cod_alt", "sup_alt", "data_inizio_impianto"
        };
        /// <summary>
        /// COLONNE dbo.Imprese_Progetti DA NON ESTRARRE MAI AUTOMATISMO
        /// </summary>
        public static readonly string[] FilterOut_Esercizi = new string[]
        {
            //presenti nella select base
            "progetto_nome", "progetto_des", "validita_inizio", "validita_fine",
            //colonne da escludere perchè non usate
            "cod_contratto", "cod_conto", "ricavi_previsti", "cau_progetto",
            "giudizio", "veg_cod", "grfi_cod", "data_inizio_prevista", "data_fine_prevista", "data_fioritura_prevista",
            "csprogetto_cod", "stato_impianto", "regolamento_cod", "disciplinare_cod",
            "disciplinare_pubblicoprivato", "regolamento_concimazioni_cod",
            "sup_prog", "flagsecondoraccolto", "p_ha_maschi", "mat_cod", "grupporaccolta_cod"
        };
     
        /// <summary>
        /// COLONNE dbo.Fabbricati DA NON ESTRARRE MAI AUTOMATISMO
        /// </summary>
        public static readonly string[] FilterOut_Fabbricati = new string[]
        {
            //Presenti nella select base
            "fabbricato_des",
            //colonne da escludere perchè non usate
            "indirizzo_cod", "tipo_fabbricato_cod", "prov", "com", "sezione", "foglio", "numero", "subalterno",
            "mc_convenzionale", "mc_conversione", "mc_biologico", "regolamento_cod", "titolopossesso", "conversione_inizio",
            "conversione_fine", "idoneo_costruzione", "idoneo_separazambienti", "idoneo_separazprodotti", "idoneo_condigieniche",
            "idoneo_autorizsanitaria", "idoneo_haccp", "idoneo_planimetria", "idoneo_layout", "idoneo_diagrammiflusso",
            "idoneo_cdx_m004", "idoneo_supmincoperte", "idoneo_supminscoperte", "mq_convenzionale", "mq_conversione",
            "mq_biologico", "mq_convenzionale_scoperto", "mq_conversione_scoperto", "mq_biologico_scoperto", "n_piani",
            "sup_piano", "num_autorizzazione", "data_richiesta_autorizzazione", "tipologia_utilizzo", "chkvirtuale",
            "proprietariocapi", "chkmagazzinofarmaci"
        };  
        
        /// <summary>
        /// PER AUTOMATISMO ESTRAZIONE COLONNE CHE CONTENGONO USERNAME UTENTE
        /// </summary>
        public static readonly string[] Estrai_Utente = new string[]
        {
            //dbo.Imprese    
            "Compliance_ISCC_Username"
            //dbo.dbo.Centri_Aziendali
            
            //dbo.Campi
            
            //dbo.Appezzamento
            
            //dbo.Reg_Impianti
            
            //dbo.Imprese_Progetti
            
            //dbo.Fabbricati
        };

    }

    public class Enum_PrefixCodici
    {
        public static readonly string Aziende = "IC_";
        public static readonly string CentriAziendali = "CAC_";
        public static readonly string Campi = "CC_";
        public static readonly string Appezzamenti = "AC_";
        public static readonly string Impianti = "RIC_";
        public static readonly string Esercizi = "EC_";
        public static readonly string Fabbricati = "FC_";
    }

    public class Enum_Tabelle
    {
        public const string Aziende = "Imprese";
        public const string CentriAziendali = "Centri_Aziendali";
        public const string Campi = "Campi";
        public const string Appezzamenti = "Appezzamento";
        public const string Impianti = "Reg_Impianti";
        public const string Esercizi = "Imprese_Progetti";
        public const string Fabbricati = "Fabbricati";
    }

    public class Enum_FiltroDate
    {
        public const string Maggiore = " > ";
        public const string Minore = " < ";

        public const string MaggioreUguale = " >= ";
        public const string MinoreUguale = " <= ";
    }

}

