Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class AgriBio_Costanti

    Public Const AgriBio_CategoriaAttivita_Produttore As String = "CO001"
    Public Const AgriBio_CategoriaAttivita_Preparatore As String = "CO002"
    Public Const AgriBio_CategoriaAttivita_Importatore As String = "CO003"

    Public Const AgriBio_TipoNotifica_Prima As String = "NO001"
    Public Const AgriBio_TipoNotifica_Variazione As String = "NO002"
    Public Const AgriBio_TipoNotifica_Recesso As String = "NO003"

    Public Const AgriBio_StatoOperatore_Attivo As String = "SO001"
    Public Const AgriBio_StatoOperatore_Cessato As String = "SO002"
    Public Const AgriBio_StatoOperatore_Sospesa As String = "SO003"

    Public Const AgriBio_CategoriaProduzioni_Vegetali As String = "CP001"
    Public Const AgriBio_CategoriaProduzioni_Alimentari As String = "CP003"
    Public Const AgriBio_CategoriaProduzioni_Zootecniche As String = "CP002"

    Public Const AgriBio_MetodoProduzione_Convenzionale As String = "MP001"
    Public Const AgriBio_MetodoProduzione_Conversione As String = "MP002"
    Public Const AgriBio_MetodoProduzione_Biologico As String = "MP003"

    Public Const AgriBio_TipologiaColtura_ArboreaPura As String = "TC001"
    Public Const AgriBio_TipologiaColtura_ArboreaConsociata As String = "TC002"
    Public Const AgriBio_TipologiaColtura_ErbaceaPura As String = "TC003"
    Public Const AgriBio_TipologiaColtura_ErbaceaConsociata As String = "TC004"
    Public Const AgriBio_TipologiaColtura_Promiscua As String = "TC005"

    Public Const AgriBio_PossessoStruttura_Proprieta As String = "PS001"
    Public Const AgriBio_PossessoStruttura_Affitto As String = "PS002"
    Public Const AgriBio_PossessoStruttura_Colonia As String = "PS003"
    Public Const AgriBio_PossessoStruttura_Mezzadria As String = "PS004"
    Public Const AgriBio_PossessoStruttura_Comodato As String = "PS005"
    Public Const AgriBio_PossessoStruttura_Usufrutto As String = "PS006"
    Public Const AgriBio_PossessoStruttura_Concessione As String = "PS007"
    Public Const AgriBio_PossessoStruttura_Enfiteusi As String = "PS008"
    Public Const AgriBio_PossessoStruttura_Altro As String = "PS009"

    Public Const AgriBio_TipoStruttura_AdUsoAbitativo As String = "TS001"
    Public Const AgriBio_TipoStruttura_Magazzini As String = "TS002"
    Public Const AgriBio_TipoStruttura_Sili As String = "TS003"
    Public Const AgriBio_TipoStruttura_CelleXveg As String = "TS004"
    Public Const AgriBio_TipoStruttura_CelleXZoo As String = "TS005"
    Public Const AgriBio_TipoStruttura_AltriLocali As String = "TS006"
    Public Const AgriBio_TipoStruttura_ImpiantiXUva As String = "TS007"
    Public Const AgriBio_TipoStruttura_ImpiantiXOlive As String = "TS008"
    Public Const AgriBio_TipoStruttura_RicoveroAnimali As String = "TS010"
    Public Const AgriBio_TipoStruttura_FienileMagazzino As String = "TS011"
    Public Const AgriBio_TipoStruttura_Stalla As String = "TS012"
    Public Const AgriBio_TipoStruttura_ND As String = "TS013"

    Public Const AgriBio_OrientamentoProduttivo_Cerialicolo As String = "OP001"
    Public Const AgriBio_OrientamentoProduttivo_Orticolo As String = "OP002"
    Public Const AgriBio_OrientamentoProduttivo_Frutticolo As String = "OP003"
    Public Const AgriBio_OrientamentoProduttivo_Viticolo As String = "OP004"
    Public Const AgriBio_OrientamentoProduttivo_Olivicolo As String = "OP005"
    Public Const AgriBio_OrientamentoProduttivo_FloricoloVivaistico As String = "OP006"
    Public Const AgriBio_OrientamentoProduttivo_ColtureIndustriali As String = "OP007"
    Public Const AgriBio_OrientamentoProduttivo_Foraggero As String = "OP008"
    Public Const AgriBio_OrientamentoProduttivo_SupConnesseAllev As String = "OP009"
    Public Const AgriBio_OrientamentoProduttivo_Altro As String = "OP010"

    '    --FP	FORMA POSSESSO
    Public Const AgriBio_FormaPossesso_Proprieta_SiCond As String = "FP001"
    Public Const AgriBio_FormaPossesso_Proprieta_NoCond As String = "FP021"
    Public Const AgriBio_FormaPossesso_Affitto_SiCond As String = "FP002"
    Public Const AgriBio_FormaPossesso_Affitto_NoCond As String = "FP017"
    Public Const AgriBio_FormaPossesso_Comodato_SiCond As String = "FP018"
    Public Const AgriBio_FormaPossesso_Comodato_NoCond As String = "FP011"
    Public Const AgriBio_FormaPossesso_Comodato_AltreForme As String = "FP004"
    'FP001	PROPRIETA' CON CONDUZIONE
    'FP021	PROPRIETA' SENZA CONDUZIONE
    'FP002	AFFITTO CON CONDUZIONE
    'FP017	AFFITTO SENZA CONDUZIONE
    'FP018	COMODATO SENZA CONDUZIONE
    'FP011	COMODATO CON CONDUZIONE
    'FP003	MEZZADRIA CON CONDUZIONE
    'FP005	COMPARTECIPAZIONE SENZA CONDUZIONE
    'FP006	VENDITA DI ERBE SENZA CONDUZIONE
    'FP007	COLTIVAZIONI INTERCALARI SENZA CONDUZIONE
    'FP008	USUCAPIONE CON CONDUZIONE
    'FP009	USUFRUTTO CON CONDUZIONE
    'FP010	USO CON CONDUZIONE
    'FP012	SOCCIDA CON PASCOLO O SENZA PASCOLO CON CONDUZIONE
    'FP013	PARTECIPANZA, COMUNALIA CON CONDUZIONE
    'FP014	COMPARTECIPAZIONE CON CONDUZIONE
    'FP015	VENDITA DI ERBE CON CONDUZIONE
    'FP016	COLTIVAZIONI INTERCALARI CON CONDUZIONE
    'FP019	MEZZADRIA SENZA CONDUZIONE
    'FP020	PARTECIPANZA, COMUNALIA SENZA CONDUZIONE
    'FP022	SOCCIDA CON PASCOLO O SENZA PASCOLO SENZA CONDUZIONE
    'FP023	USO SENZA CONDUZIONE
    'FP024	USUCAPIONE SENZA CONDUZIONE
    'FP025	USUFRUTTO SENZA CONDUZIONE
    'FP026	PROCURA CON CONDUZIONE
    'FP027	PROCURA SENZA CONDUZIONE
    'FP028	IRREPERIBILITA' CON CONDUZIONE
    'FP029	IRREPERIBILITA' SENZA CONDUZIONE
    'FP030	ASSENZA (EX. ART. 49 C.C.) CON CONDUZIONE
    'FP031	ASSENZA (EX. ART. 49 C.C.) SENZA CONDUZIONE
    'FP032	MORTE PRESUNTA (EX. ART. 58 C.C.) CON CONDUZIONE
    'FP033	MORTE PRESUNTA (EX. ART. 58 C.C.) SENZA CONDUZIONE
    'FP034	LIVELLO CON CONDUZIONE
    'FP035	LIVELLO SENZA CONDUZIONE

    '    --TV	TIPO PRODUZIONE VEGETALE
    'INDIRIZZO PRODUTTIVO
    Public Const AgriBio_TipoProdVeg_Cerealicolo As String = "TV001"
    Public Const AgriBio_TipoProdVeg_Orticolo As String = "TV002"
    Public Const AgriBio_TipoProdVeg_ColtIndustriali As String = "TV003"
    Public Const AgriBio_TipoProdVeg_Frutticolo As String = "TV004"
    Public Const AgriBio_TipoProdVeg_Vitivinicolo As String = "TV005"
    Public Const AgriBio_TipoProdVeg_Olivicolo As String = "TV006"
    Public Const AgriBio_TipoProdVeg_Foraggero As String = "TV007"
    Public Const AgriBio_TipoProdVeg_VivSementiero As String = "TV008"
    'STRUTTURE STOCCAGGIO
    Public Const AgriBio_TipoProdVeg_Magazzini As String = "TV010"
    Public Const AgriBio_TipoProdVeg_Sili As String = "TV011"
    Public Const AgriBio_TipoProdVeg_CelleFrigorifere As String = "TV012"
    Public Const AgriBio_TipoProdVeg_ImpiantiPreparazioni As String = "TV013"
    'INFORMAZIONI RIEPILOGATIVE
    Public Const AgriBio_TipoProdVeg_Cereali As String = "TV014"
    Public Const AgriBio_TipoProdVeg_ColtIndustriali2 As String = "TV015"
    Public Const AgriBio_TipoProdVeg_Ortofrutticoli As String = "TV016"
    Public Const AgriBio_TipoProdVeg_CompVitivinicolo As String = "TV017"
    Public Const AgriBio_TipoProdVeg_CompOleicolo As String = "TV018"
    Public Const AgriBio_TipoProdVeg_CompVivSementiero As String = "TV019"
    Public Const AgriBio_TipoProdVeg_ProdottiSpontanei As String = "TV020"

    '    --SV	SUB TIPO PRODUZIONE VEGETALE
    '-------INDIRIZZO PRODUTTIVO---------------
    Public Const AgriBio_SubTipoProdVeg_Cerealicolo_Riso As String = "SV001"
    Public Const AgriBio_SubTipoProdVeg_Cerealicolo_FrumentoDuro As String = "SV002"
    Public Const AgriBio_SubTipoProdVeg_Cerealicolo_FrumentoTenero As String = "SV003"
    Public Const AgriBio_SubTipoProdVeg_Cerealicolo_Mais As String = "SV004"
    Public Const AgriBio_SubTipoProdVeg_Cerealicolo_AltriCereali As String = "SV005"

    Public Const AgriBio_SubTipoProdVeg_Orticolo_PienoCampo As String = "SV006"
    Public Const AgriBio_SubTipoProdVeg_Orticolo_colturaProtetta As String = "SV007"

    Public Const AgriBio_SubTipoProdVeg_Frutticolo_Pomacee As String = "SV008"
    Public Const AgriBio_SubTipoProdVeg_Frutticolo_Drupacee As String = "SV009"
    Public Const AgriBio_SubTipoProdVeg_Frutticolo_Agrumi As String = "SV010"
    Public Const AgriBio_SubTipoProdVeg_Frutticolo_FruttaSecca As String = "SV011"
    Public Const AgriBio_SubTipoProdVeg_Frutticolo_UvaDaTavola As String = "SV012"
    Public Const AgriBio_SubTipoProdVeg_Frutticolo_Altro As String = "SV013"

    Public Const AgriBio_SubTipoProdVeg_Vitivinicolo_DaTavola As String = "SV014"
    Public Const AgriBio_SubTipoProdVeg_Vitivinicolo_Denominazione As String = "SV015"

    Public Const AgriBio_subTipoProdVeg_Olivicolo_DaMensa As String = "SV016"
    Public Const AgriBio_subTipoProdVeg_Olivicolo_DaOlio As String = "SV017"


    '---------STRUTTURE STOCCAGGIO----------------
    Public Const AgriBio_TipoProdVeg_Magazzini_Aziendali As String = "SV018"
    Public Const AgriBio_TipoProdVeg_Magazzini_Esterni As String = "SV019"
    Public Const AgriBio_TipoProdVeg_Magazzini_MezziTecnici As String = "SV020"
    Public Const AgriBio_TipoProdVeg_Magazzini_Attrezzature As String = "SV021"
    Public Const AgriBio_TipoProdVeg_Magazzini_Vendita As String = "SV022"
    Public Const AgriBio_TipoProdVeg_Magazzini_Altro As String = "SV023"
    '-----------------------------------------
    Public Const AgriBio_TipoProdVeg_Sili_Aziendali As String = "SV024"
    Public Const AgriBio_TipoProdVeg_Sili_Esterni As String = "SV025"
    Public Const AgriBio_TipoProdVeg_Sili_StoccaggioGranaglie As String = "SV026"
    Public Const AgriBio_TipoProdVeg_Sili_StoccaggioColtIndustriale As String = "SV027"
    Public Const AgriBio_TipoProdVeg_Sili_StoccaggioMangimi As String = "SV028"
    Public Const AgriBio_TipoProdVeg_Sili_PreparazioniInsilati As String = "SV029"
    Public Const AgriBio_TipoProdVeg_Sili_Altro As String = "SV030"
    '-----------------------------------------
    Public Const AgriBio_TipoProdVeg_CelleFrigorifere_Aziendali As String = "SV031"
    Public Const AgriBio_TipoProdVeg_CelleFrigorifere_Esterni As String = "SV032"
    Public Const AgriBio_TipoProdVeg_CelleFrigorifere_ProdVeg As String = "SV033"
    Public Const AgriBio_TipoProdVeg_CelleFrigorifere_ProdZoo As String = "SV034"
    Public Const AgriBio_TipoProdVeg_CelleFrigorifere_Altro As String = "SV035"
    '-----------------------------------------
    Public Const AgriBio_TipoProdVeg_ImpiantiPreparazioni_Aziendali As String = "SV036"
    Public Const AgriBio_TipoProdVeg_ImpiantiPreparazioni_Esterni As String = "SV037"
    Public Const AgriBio_TipoProdVeg_ImpiantiPreparazioni_Altro As String = "SV038"



    '------------INFORMAZIONI RIEPILOGATIVE------------------
    Public Const AgriBio_SubTipoProdVeg_Cereali_Granella As String = "SV039"
    Public Const AgriBio_SubTipoProdVeg_Cereali_Conservazione As String = "SV040"
    Public Const AgriBio_SubTipoProdVeg_Cereali_Sfarinati As String = "SV041"
    Public Const AgriBio_SubTipoProdVeg_Cereali_Panificazione As String = "SV042"
    Public Const AgriBio_SubTipoProdVeg_Cereali_Pastificazione As String = "SV043"
    Public Const AgriBio_SubTipoProdVeg_Cereali_ProdottiForno As String = "SV044"
    Public Const AgriBio_SubTipoProdVeg_Cereali_Altro As String = "SV045"
    '-----------------------------------------
    Public Const AgriBio_SubTipoProdVeg_ColtIndustriali_Granella As String = "SV046"
    Public Const AgriBio_SubTipoProdVeg_ColtIndustriali_Sfarinati As String = "SV047"
    Public Const AgriBio_SubTipoProdVeg_ColtIndustriali_EstrazioneOlio As String = "SV048"
    Public Const AgriBio_SubTipoProdVeg_ColtIndustriali_Conservazione As String = "SV049"
    Public Const AgriBio_SubTipoProdVeg_ColtIndustriali_Confezionamento As String = "SV050"
    Public Const AgriBio_SubTipoProdVeg_ColtIndustriali_Altro As String = "SV051"
    '-------------------------
    Public Const AgriBio_SubTipoProdVeg_Ortofrutticoli_Freschi As String = "SV052"
    Public Const AgriBio_SubTipoProdVeg_Ortofrutticoli_ConserveVegetali As String = "SV053"
    Public Const AgriBio_SubTipoProdVeg_Ortofrutticoli_Conservazione As String = "SV054"
    Public Const AgriBio_SubTipoProdVeg_Ortofrutticoli_Confezionamento As String = "SV055"
    '--------------------------
    Public Const AgriBio_SubTipoProdVeg_Vitivinicolo_Vinificazione As String = "SV056"
    Public Const AgriBio_SubTipoProdVeg_Vitivinicolo_Mostificazione As String = "SV057"
    Public Const AgriBio_SubTipoProdVeg_Vitivinicolo_Imbottigliamento As String = "SV058"
    '---------------------------
    Public Const AgriBio_SubTipoProdVeg_Oleolicolo_ConserveVegetali As String = "SV059"
    Public Const AgriBio_SubTipoProdVeg_Oleolicolo_EstrazioneOlio As String = "SV060"
    Public Const AgriBio_SubTipoProdVeg_Oleolicolo_Imbottigliamento As String = "SV061"
    '-------------------
    Public Const AgriBio_SubTipoProdVeg_VivSementiero_Semi As String = "SV062"
    Public Const AgriBio_SubTipoProdVeg_VivSementiero_Orticolo As String = "SV063"
    Public Const AgriBio_SubTipoProdVeg_VivSementiero_Astoni As String = "SV064"
    Public Const AgriBio_SubTipoProdVeg_VivSementiero_Barbatelle As String = "SV065"
    Public Const AgriBio_SubTipoProdVeg_VivSementiero_Altro As String = "SV066"
    Public Const AgriBio_SubTipoProdVeg_VivSementiero_PianteOfficinali As String = "SV067"
    '---------------------------
    'SV068	COLT. INDUSTRIALE '???????????

    '    --TZ	TIPO PRODUZIONE ZOOTECNICA
    'TZ001	NO DESCRIZIONE
    'TZ002	CARNE
    'TZ003	LATTE
    'TZ004	UOVA
    'TZ005	PRODOTTI DELL'APICOLTURA
    'TZ006	ALTRO
    Public Const AgriBio_TipoProdZoo_NoDesc As String = "TZ001"
    Public Const AgriBio_TipoProdZoo_Carne As String = "TZ002"
    Public Const AgriBio_TipoProdZoo_Latte As String = "TZ003"
    Public Const AgriBio_TipoProdZoo_Uova As String = "TZ004"
    Public Const AgriBio_TipoProdZoo_Apicoltura As String = "TZ005"
    Public Const AgriBio_TipoProdZoo_Altro As String = "TZ006"

    '    --SZ	SUB TIPO PRODUZIONE ZOOTECNICA
    ''informazioni riepilogative delle attività connesse alle prod zootecniche
    'SZ011	CARNE FRESCA
    'SZ012	DERIVATI DELLA CARNE
    'SZ013	MACELLAZIONE
    'SZ014	CONSERVAZIONE
    'SZ015	SEZIONAMENTO
    'SZ016	PRODOTTI DI SALUMERIA
    'SZ017	CONFEZIONAMENTO
    Public Const AgriBio_SubTipoProdZoo_Carne_CarneFresca As String = "SZ011"
    Public Const AgriBio_SubTipoProdZoo_Carne_DerivatiCarne As String = "SZ012"
    Public Const AgriBio_SubTipoProdZoo_Carne_Macellazione As String = "SZ013"
    Public Const AgriBio_SubTipoProdZoo_Carne_Conservazione As String = "SZ014"
    Public Const AgriBio_SubTipoProdZoo_Carne_Sezionamento As String = "SZ015"
    Public Const AgriBio_SubTipoProdZoo_Carne_Salumeria As String = "SZ016"
    Public Const AgriBio_SubTipoProdZoo_Carne_Confezionamento As String = "SZ017"

    'SZ018	LATTE ALIMENTARE
    'SZ019	CASEIFICAZIONE
    'SZ020	BURRO
    'SZ021	YOGURT
    'SZ022	ALTRI DERIVATI DEL LATTE
    'SZ023	CONFEZIONAMENTO
    'SZ024	ALTRO
    Public Const AgriBio_SubTipoProdZoo_Latte_LatteAlimentare As String = "SZ018"
    Public Const AgriBio_SubTipoProdZoo_Latte_Caseificazione As String = "SZ019"
    Public Const AgriBio_SubTipoProdZoo_Latte_Burro As String = "SZ020"
    Public Const AgriBio_SubTipoProdZoo_Latte_Yogurt As String = "SZ021"
    Public Const AgriBio_SubTipoProdZoo_Latte_DerivatiLatte As String = "SZ022"
    Public Const AgriBio_SubTipoProdZoo_Latte_Confezionamento As String = "SZ023"
    Public Const AgriBio_SubTipoProdZoo_Latte_Altro As String = "SZ024"

    'SZ025	CONFEZIONAMENTO
    'SZ026	ALTRO
    Public Const AgriBio_SubTipoProdZoo_Uova_Confezionamento As String = "SZ025"
    Public Const AgriBio_SubTipoProdZoo_Uova_Altro As String = "SZ026"

    'SZ027	CONFEZIONAMENTO
    'SZ028	ALTRO
    Public Const AgriBio_SubTipoProdZoo_Apicoltura_Confezionamento As String = "SZ027"
    Public Const AgriBio_SubTipoProdZoo_Apicoltura_Altro As String = "SZ028"

    '    --SZ	SUB TIPO PRODUZIONE ZOOTECNICA
    'informazioni riepilogative delle attività di allevamento
    'SZ001	BOVINI
    'SZ002	BUFALINI
    'SZ003	OVINI
    'SZ004	CAPRINI
    'SZ005	EQUINI
    'SZ006	SUINI
    'SZ007	SUINI DA INGRASSO
    'SZ008	AVICOLI
    'SZ009	API
    'SZ010	ALTRO
    Public Const AgriBio_SubTipoProdZoo_AA_Bovini As String = "SZ001"
    Public Const AgriBio_SubTipoProdZoo_AA_Bufalini As String = "SZ002"
    Public Const AgriBio_SubTipoProdZoo_AA_Ovini As String = "SZ003"
    Public Const AgriBio_SubTipoProdZoo_AA_Caprini As String = "SZ004"
    Public Const AgriBio_SubTipoProdZoo_AA_Equini As String = "SZ005"
    Public Const AgriBio_SubTipoProdZoo_AA_Suini As String = "SZ006"
    Public Const AgriBio_SubTipoProdZoo_AA_SuiniIngrasso As String = "SZ007"
    Public Const AgriBio_SubTipoProdZoo_AA_Avicoli As String = "SZ008"
    Public Const AgriBio_SubTipoProdZoo_AA_Api As String = "SZ009"
    Public Const AgriBio_SubTipoProdZoo_AA_Altro As String = "SZ010"

    '--PA	TIPO STRUTTURA PREPARAZIONE ALIMENTARE
    'PA001	SILI
    'PA002	CELLE FRIGORIFERE
    'PA003	MAGAZZINI
    Public Const AgriBio_TipoStrutturaPrepAlim_Sili As String = "PA001"
    Public Const AgriBio_TipoStrutturaPrepAlim_CelleFrigorifere As String = "PA002"
    Public Const AgriBio_TipoStrutturaPrepAlim_Magazzini As String = "PA003"

    '--SA	SUB TIPO STRUTTURA PREPARAZIONE ALIMENTARE
    'SA001	STOCCAGGIO CEREALI
    'SA002	STOCCAGGIO PROTEOLEAGINOSE
    'SA003	ALTRO
    Public Const AgriBio_SubTipoStrutturaPrepAlim_Sili_StoccaggioCereali As String = "SA001"
    Public Const AgriBio_SubTipoStrutturaPrepAlim_Sili_StoccaggioProteoleaginose As String = "SA002"
    Public Const AgriBio_SubTipoStrutturaPrepAlim_Sili_Altro As String = "SA003"
    'SA004	PRODUZIONI VEGETALI
    'SA005	PRODUZIONI ZOOTECNICHE
    'SA006	ALTRO
    Public Const AgriBio_SubTipoStrutturaPrepAlim_CelleFrigorifere_ProdVegetali As String = "SA004"
    Public Const AgriBio_SubTipoStrutturaPrepAlim_CelleFrigorifere_ProdZoo As String = "SA005"
    Public Const AgriBio_SubTipoStrutturaPrepAlim_CelleFrigorifere_Altro As String = "SA006"
    'SA007	_
    Public Const AgriBio_SubTipoStrutturaPrepAlim_Magazzini_ As String = "SA007"

    '    --TP	TIPOLOGIA DI COMMERCIALIZZAZIONE PER PREPARAZIONE
    'TP001	INGROSSO
    'TP002	DETTAGLIO
    'TP003	G.D.O.
    'TP004	DISTRIBUZIONE A MARCHIO
    'TP005	ALTRO
    Public Const AgriBio_TipoCommercio_Ingrosso As String = "TP001"
    Public Const AgriBio_TipoCommercio_Dettaglio As String = "TP002"
    Public Const AgriBio_TipoCommercio_GDO As String = "TP003"
    Public Const AgriBio_TipoCommercio_SistribuzioneMarchio As String = "TP004"
    Public Const AgriBio_TipoCommercio_Altro As String = "TP005"

    '--TL	TIPO PRODUZIONE ALIMENTARE
    'TL001	DA MAT. PRIMA VEGETALE
    'TL002	DA MAT. PRIMA ANIMALE
    'TL003	INDUSTRIA DOLCIARIA
    'TL004	MANGIMI
    'TL006	ALTRO
    'TL007	ALTRE FASI - CONSERVAZIONE
    'TL008	ALTRE FASI - CONDIZIONAMENTO
    'TL009	ALTRE FASI - IMMAGAZZINAMENTO
    'TL010	ALTRE FASI - ETICHETTATURA
    'TL011	ALTRE FASI - CONFEZIONAMENTO
    Public Const AgriBio_TipoProdAlim_MPVegetale As String = "TL001"
    Public Const AgriBio_TipoProdAlim_MPAnimale As String = "TL002"
    Public Const AgriBio_TipoProdAlim_IndustriaDolciaria As String = "TL003"
    Public Const AgriBio_TipoProdAlim_Mangimi As String = "TL004"
    Public Const AgriBio_TipoProdAlim_Altro As String = "TL006"
    Public Const AgriBio_TipoProdAlim_Conservazione As String = "TL007"
    Public Const AgriBio_TipoProdAlim_Condizionamento As String = "TL008"
    Public Const AgriBio_TipoProdAlim_Immagazzinamento As String = "TL009"
    Public Const AgriBio_TipoProdAlim_Etichettatura As String = "TL010"
    Public Const AgriBio_TipoProdAlim_Confezionamento As String = "TL011"

    '    --SL	SUB TIPO PRODUZIONE ALIMENTARE
    'SL001	MOLITURA E AFFINI
    'SL002	FIOCCATURA E TOSTATURE
    'SL003	PASTIFICAZIONE E PANIFICAZIONE
    'SL004	CONSERVE VEGETALI
    'SL005	INTEGRATORI ALIMENTARI
    'SL006	ESTRAZIONE DI OLIO E GRASSI
    'SL007	VINIFICAZIONE
    'SL008	LIQUORI E/O DISTILLATI
    'SL009	IMBOTTIGLIAMENTO
    Public Const AgriBio_subTipoProdAlim_Molitura As String = "SL001"
    Public Const AgriBio_subTipoProdAlim_Fioccatura As String = "SL002"
    Public Const AgriBio_subTipoProdAlim_Pastificazione As String = "SL003"
    Public Const AgriBio_subTipoProdAlim_ConserveVegetali As String = "SL004"
    Public Const AgriBio_subTipoProdAlim_Integratori As String = "SL005"
    Public Const AgriBio_subTipoProdAlim_EstrazioneOlio As String = "SL006"
    Public Const AgriBio_subTipoProdAlim_Vinificazione As String = "SL007"
    Public Const AgriBio_subTipoProdAlim_LiquoriDistillati As String = "SL008"
    Public Const AgriBio_subTipoProdAlim_Imbottigliamento As String = "SL009"
    'SL010	PRODOTTI ERBORISTICI
    'SL011	MACELLAZIONE
    'SL012	SEZIONAMENTO
    'SL013	DERIVATI DELLA CARNE
    'SL014	CONSERVE ANIMALI
    'SL015	PRODOTTI DI SALUMERIA
    'SL016	LATTE ALIMENTARI
    'SL017	CASEIFICAZIONE
    'SL018	BURRO
    'SL019	YOGURT
    Public Const AgriBio_subTipoProdAlim_ProdErboristici As String = "SL010"
    Public Const AgriBio_subTipoProdAlim_Macellazione As String = "SL011"
    Public Const AgriBio_subTipoProdAlim_Sezionamento As String = "SL012"
    Public Const AgriBio_subTipoProdAlim_DerivatiCarne As String = "SL013"
    Public Const AgriBio_subTipoProdAlim_ConserveAnimali As String = "SL014"
    Public Const AgriBio_subTipoProdAlim_ProdSalumeria As String = "SL015"
    Public Const AgriBio_subTipoProdAlim_Latte As String = "SL016"
    Public Const AgriBio_subTipoProdAlim_Caseificazioni As String = "SL017"
    Public Const AgriBio_subTipoProdAlim_Burro As String = "SL018"
    Public Const AgriBio_subTipoProdAlim_Yogurt As String = "SL019"
    'SL020	ALTRO
    'SL021	PRODOTTI DA FORNO
    'SL022	ALTRI PRODOTTI
    'SL024	ALTRO
    'SL025	ALTRO
    'SL026	PRODOTTI SURGELATI
    'SL027	ORTOFRUTTICOLI FRESCHI/SECCHI
    'SL028	PORZIONATURA
    'SL029	UOVA E DERIVATI
    Public Const AgriBio_subTipoProdAlim_Altro As String = "SL020"
    Public Const AgriBio_subTipoProdAlim_ProdottiForno As String = "SL021"
    Public Const AgriBio_subTipoProdAlim_AltriProdotto As String = "SL022"
    Public Const AgriBio_subTipoProdAlim_Altro1 As String = "SL024"
    Public Const AgriBio_subTipoProdAlim_Altro2 As String = "SL025"
    Public Const AgriBio_subTipoProdAlim_ProdSurgelati As String = "SL026"
    Public Const AgriBio_subTipoProdAlim_Ortofrutticoli As String = "SL027"
    Public Const AgriBio_subTipoProdAlim_Porzionatura As String = "SL028"
    Public Const AgriBio_subTipoProdAlim_UovaDerivati As String = "SL029"


    '--TA	TIPO PREPARAZIONE ALIMENTARE
    'TA001	MAT. PRIMA VEGETALE
    'TA002	MOLITURA E AFFINI
    'TA003	FIOCCATURA E TOSTATURE
    'TA004	PASTIFICAZIONE
    'TA005	CONSERVE VEGETALI
    'TA006	INTEGRATORI ALIMENTARI
    'TA007	ESTRAZIONE DI OLIO
    'TA008	VINIFICAZIONE
    'TA009	LIQUORI E DISTILLATI
    'TA010	IMBOTTIGLIAMENTO
    'TA011	PRODOTTI ERBORISTICI
    'TA012	MAT. PRIMA ANIMALE
    'TA013	MACELLAZIONE
    'TA014	SEZIONAMENTO
    'TA015	DERIVATI DELLA CARNE
    'TA016	CONSERVE ANIMALI
    'TA017	PRODOTTI DI SALUMERIA
    'TA018	LATTE ALIMENTARE
    'TA019	CASEIFICAZIONE
    'TA020	BURRO
    'TA021	YOGURT
    'TA022	ALTRO PROD. ANIMALI
    'TA023	INDUSTRIA DOLCIARIA
    'TA024	PRODOTTI DA FORNO
    'TA025	ALTRI PROD.DOLCIARI
    'TA026	MANGIMI
    'TA027	ETICHETTATURA
    'TA028	ALTRO
    'TA029	IMMAGAZZINAMENTO
    'TA030	ORTOFRUTTICOLI FRESCHI/SECCCHI
    'TA031	PRODOTTI SURGELATI
    'TA032	PORZIONATURA
    'TA033	UOVA E DERIVATI
    Public Const AgriBio_TipoPrep_MPVegetale As String = "TA001"
    Public Const AgriBio_TipoPrep_Molitura As String = "TA002"
    Public Const AgriBio_TipoPrep_Fioccatura As String = "TA003"
    Public Const AgriBio_TipoPrep_Pastificazione As String = "TA004"
    Public Const AgriBio_TipoPrep_ConserveVegetali As String = "TA005"
    Public Const AgriBio_TipoPrep_Integratori As String = "TA006"
    Public Const AgriBio_TipoPrep_EstrazioneOlio As String = "TA007"
    Public Const AgriBio_TipoPrep_Vinificazione As String = "TA008"
    Public Const AgriBio_TipoPrep_LiquoriDistillati As String = "TA009"
    Public Const AgriBio_TipoPrep_Imbottigliamento As String = "TA010"
    Public Const AgriBio_TipoPrep_ProdErboristici As String = "TA011"
    Public Const AgriBio_TipoPrep_MPAnimale As String = "TA012"
    Public Const AgriBio_TipoPrep_Macellazione As String = "TA013"
    Public Const AgriBio_TipoPrep_Sezionamento As String = "TA014"
    Public Const AgriBio_TipoPrep_DerivatiCarne As String = "TA015"
    Public Const AgriBio_TipoPrep_ConserveAnimali As String = "TA016"
    Public Const AgriBio_TipoPrep_ProdSalumeria As String = "TA017"
    Public Const AgriBio_TipoPrep_Latte As String = "TA018"
    Public Const AgriBio_TipoPrep_Caseificazioni As String = "TA019"
    Public Const AgriBio_TipoPrep_Burro As String = "TA020"
    Public Const AgriBio_TipoPrep_Yogurt As String = "TA021"
    Public Const AgriBio_TipoPrep_AltroProdAnimali As String = "TA022"
    Public Const AgriBio_TipoPrep_IndDolciaria As String = "TA023"
    Public Const AgriBio_TipoPrep_ProdottiForno As String = "TA024"
    Public Const AgriBio_TipoPrep_AltriProdDolciari As String = "TA025"
    Public Const AgriBio_TipoPrep_Mangimi As String = "TA026"
    Public Const AgriBio_TipoPrep_Etichettatura As String = "TA027"
    Public Const AgriBio_TipoPrep_Altro As String = "TA028"
    Public Const AgriBio_TipoPrep_Immagazzinamento As String = "TA029"
    Public Const AgriBio_TipoPrep_Ortofrutticoli As String = "TA030"
    Public Const AgriBio_TipoPrep_ProdSurgelati As String = "TA031"
    Public Const AgriBio_TipoPrep_Porzionatura As String = "TA032"
    Public Const AgriBio_TipoPrep_UovaDerivati As String = "TA033"

    '    --AF	ALTRE FASI DI PREPARAZIONE ALIMENTARE
    'AF001	IMMAGAZZINAMENTO
    'AF002	CONSERVAZIONE
    'AF003	CONDIZIONAMENTO
    'AF004	CONFEZIONAMENTO
    'AF005	ETICHETTATURA
    'AF006	ALTRO
    Public Const AgriBio_AltreFasiPrepAlim_Immagazzinamento As String = "AF001"
    Public Const AgriBio_AltreFasiPrepAlim_Conservazione As String = "AF002"
    Public Const AgriBio_AltreFasiPrepAlim_Condizionamento As String = "AF003"
    Public Const AgriBio_AltreFasiPrepAlim_Confezionamento As String = "AF004"
    Public Const AgriBio_AltreFasiPrepAlim_Etichettatura As String = "AF005"
    Public Const AgriBio_AltreFasiPrepAlim_Altro As String = "AF006"

    '    --UM	UNITA MISURA
    'UM001	LITRI
    'UM002	TONE
    'UM003	METRI
    'UM004	KM
    'UM005	ORE
    'UM006	KG
    'UM007	MCUBI
    'UM008	QUINTALE
    'UM009	PEZZI
    'UM010	ALTRO
    'UM011	KG/GIORNO
    'UM012	Q.LI/GIORNO
    'UM013	UNITA/GIORNO
    'UM014	ETTOLITRI
    'UM015	UNITA/ORA
    'UM016	Q.LI/ORA
    'UM017	LITRI/ORA
    'UM018	KG/ORA
    'UM019	MQ
    'UM020	UNITA'
    Public Const AgriBio_UnitaMisura_Litri As String = "UM001"
    Public Const AgriBio_UnitaMisura_Tone As String = "UM002"
    Public Const AgriBio_UnitaMisura_Metri As String = "UM003"
    Public Const AgriBio_UnitaMisura_Km As String = "UM004"
    Public Const AgriBio_UnitaMisura_Ore As String = "UM005"
    Public Const AgriBio_UnitaMisura_Kg As String = "UM006"
    Public Const AgriBio_UnitaMisura_MetriCubi As String = "UM007"
    Public Const AgriBio_UnitaMisura_Quintali As String = "UM008"
    Public Const AgriBio_UnitaMisura_Pezzi As String = "UM009"
    Public Const AgriBio_UnitaMisura_Altro As String = "UM010"
    Public Const AgriBio_UnitaMisura_Kg_Giorno As String = "UM011"
    Public Const AgriBio_UnitaMisura_Qli_Giorno As String = "UM012"
    Public Const AgriBio_UnitaMisura_Unita_Giorno As String = "UM013"
    Public Const AgriBio_UnitaMisura_Ettolitri As String = "UM014"
    Public Const AgriBio_UnitaMisura_Unita_Ora As String = "UM015"
    Public Const AgriBio_UnitaMisura_Qli_Ora As String = "UM016"
    Public Const AgriBio_UnitaMisura_Litri_Ora As String = "UM017"
    Public Const AgriBio_UnitaMisura_Kg_Ora As String = "UM018"
    Public Const AgriBio_UnitaMisura_MetriQuadri As String = "UM019"
    Public Const AgriBio_UnitaMisura_Unita As String = "UM020"


    Public Const AgriBio_Consistenze_AltriAllevamenti As String = "ALT000"
    Public Const AgriBio_Consistenze_CoturniciDaRiproduzione As String = "ALT001"
    Public Const AgriBio_Consistenze_Coturnici As String = "ALT002"
    Public Const AgriBio_Consistenze_AltriVolatili As String = "ALT003"
    Public Const AgriBio_Consistenze_LepriVisoniNutrieCincilla As String = "ALT004"
    Public Const AgriBio_Consistenze_Volpi As String = "ALT005"
    Public Const AgriBio_Consistenze_PesciCrostaceiMolluschiDaRiproduzione As String = "ALT006"
    Public Const AgriBio_Consistenze_PesciCrostaceiMolluschiDaConsumo As String = "ALT007"
    Public Const AgriBio_Consistenze_CinghialiCervi As String = "ALT008"
    Public Const AgriBio_Consistenze_DainiCaprioliMufloni As String = "ALT009"

    Public Const AgriBio_Consistenze_SuiniRiproduzione_ScrofeGestazione As String = "SUI001"
    Public Const AgriBio_Consistenze_SuiniRiproduzione_ScrofeParto As String = "SUI002"
    Public Const AgriBio_Consistenze_SuiniRiproduzione_Verri As String = "SUI003"
    Public Const AgriBio_Consistenze_SuiniRiproduzione_Lattonzoli As String = "SUI004"
    Public Const AgriBio_Consistenze_SuiniRiproduzione_Scrofette As String = "SUI005"


    ' le superfici sono espresse in m2
    Public Const CoeffConversione As Decimal = 10000

    '##########################################
    Public Function TipoFabbricatoCod_from_tipoStruttura(ByVal tipoStruttura As String) As Integer

        Dim TipoFabbricatoCod As Integer = 0

        'Const AgriBio_TipoStruttura_AdUsoAbitativo As String = "TS001"
        'Const AgriBio_TipoStruttura_Magazzini As String = "TS002"
        'Const AgriBio_TipoStruttura_Sili As String = "TS003"
        'Const AgriBio_TipoStruttura_CelleXveg As String = "TS004"
        'Const AgriBio_TipoStruttura_CelleXZoo As String = "TS005"
        'Const AgriBio_TipoStruttura_AltriLocali As String = "TS006"
        'Const AgriBio_TipoStruttura_ImpiantiXUva As String = "TS007"
        'Const AgriBio_TipoStruttura_ImpiantiXOlive As String = "TS008"
        'Const AgriBio_TipoStruttura_RicoveroAnimali As String = "TS010"
        'Const AgriBio_TipoStruttura_FienileMagazzino As String = "TS011"
        'Const AgriBio_TipoStruttura_Stalla As String = "TS012"
        'Const AgriBio_TipoStruttura_ND As String = "TS013"

        Select Case tipoStruttura

            Case AgriBio_TipoStruttura_AdUsoAbitativo
                TipoFabbricatoCod = enum_FabbricatiTipi.FabbricatoUsoAbitativo

            Case AgriBio_TipoStruttura_Magazzini
                TipoFabbricatoCod = enum_FabbricatiTipi.MagazzinoAziendale

            Case AgriBio_TipoStruttura_Sili
                TipoFabbricatoCod = enum_FabbricatiTipi.SiloAziendale

            Case AgriBio_TipoStruttura_CelleXveg
                TipoFabbricatoCod = enum_FabbricatiTipi.CellaFrigoriferaProdVegetali

            Case AgriBio_TipoStruttura_CelleXZoo
                TipoFabbricatoCod = enum_FabbricatiTipi.CellaFrigoriferaProdZoo

            Case AgriBio_TipoStruttura_AltriLocali
                TipoFabbricatoCod = enum_FabbricatiTipi.Altro

            Case AgriBio_TipoStruttura_ImpiantiXUva
                TipoFabbricatoCod = enum_FabbricatiTipi.ImpiantoPrepAlimentariLavUva

            Case AgriBio_TipoStruttura_ImpiantiXOlive
                TipoFabbricatoCod = enum_FabbricatiTipi.ImpiantoPrepAlimentariLavOlive

            Case AgriBio_TipoStruttura_RicoveroAnimali
                TipoFabbricatoCod = enum_FabbricatiTipi.RicoveroAnimali

            Case AgriBio_TipoStruttura_FienileMagazzino
                TipoFabbricatoCod = enum_FabbricatiTipi.FienileAziendale

            Case AgriBio_TipoStruttura_Stalla
                TipoFabbricatoCod = enum_FabbricatiTipi.RicoveroAnimaliBox

            Case AgriBio_TipoStruttura_ND
                TipoFabbricatoCod = enum_FabbricatiTipi.Altro

        End Select

        Return TipoFabbricatoCod

    End Function


    '##########################################
    Public Function TitoloPossesso_from_codPossessoStruttura(ByVal codPossesso As String) As Integer

        Dim TitoloPossesso As Integer = 0

        'Const AgriBio_PossessoStruttura_Proprieta As String = "PS001"
        'Const AgriBio_PossessoStruttura_Affitto As String = "PS002"
        'Const AgriBio_PossessoStruttura_Colonia As String = "PS003"
        'Const AgriBio_PossessoStruttura_Mezzadria As String = "PS004"
        'Const AgriBio_PossessoStruttura_Comodato As String = "PS005"
        'Const AgriBio_PossessoStruttura_Usufrutto As String = "PS006"
        'Const AgriBio_PossessoStruttura_Concessione As String = "PS007"
        'Const AgriBio_PossessoStruttura_Enfiteusi As String = "PS008"
        'Const AgriBio_PossessoStruttura_Altro As String = "PS009"

        Select Case codPossesso

            Case AgriBio_PossessoStruttura_Proprieta
                TitoloPossesso = enum_TitoloPossesso.Proprieta

            Case AgriBio_PossessoStruttura_Affitto
                TitoloPossesso = enum_TitoloPossesso.AffittoContratto

            Case AgriBio_PossessoStruttura_Colonia
                TitoloPossesso = enum_TitoloPossesso.Altro

            Case AgriBio_PossessoStruttura_Mezzadria
                TitoloPossesso = enum_TitoloPossesso.Altro

            Case AgriBio_PossessoStruttura_Comodato
                TitoloPossesso = enum_TitoloPossesso.Comodato

            Case AgriBio_PossessoStruttura_Usufrutto
                TitoloPossesso = enum_TitoloPossesso.Altro

            Case AgriBio_PossessoStruttura_Concessione
                TitoloPossesso = enum_TitoloPossesso.Altro

            Case AgriBio_PossessoStruttura_Enfiteusi
                TitoloPossesso = enum_TitoloPossesso.Altro

            Case AgriBio_PossessoStruttura_Altro
                TitoloPossesso = enum_TitoloPossesso.Altro

        End Select

        Return TitoloPossesso

    End Function

    '####################################################################
    Public Function TitoloPossesso_from_formaPossessoCatasto(ByVal formaPossesso As String) As Integer

        Dim TitoloPossesso As Integer = 0

        '    --FP	FORMA POSSESSO
        'Public Const AgriBio_FormaPossesso_Proprieta_SiCond As String = "FP001"
        'Public Const AgriBio_FormaPossesso_Proprieta_NoCond As String = "FP021"
        'Public Const AgriBio_FormaPossesso_Affitto_SiCond As String = "FP002"
        'Public Const AgriBio_FormaPossesso_Affitto_NoCond As String = "FP017"
        'Public Const AgriBio_FormaPossesso_Comodato_SiCond As String = "FP018"
        'Public Const AgriBio_FormaPossesso_Comodato_NoCond As String = "FP011"

        Select Case formaPossesso

            Case AgriBio_FormaPossesso_Proprieta_SiCond, AgriBio_FormaPossesso_Proprieta_NoCond
                TitoloPossesso = enum_TitoloPossesso.Proprieta

            Case AgriBio_FormaPossesso_Affitto_SiCond, AgriBio_FormaPossesso_Affitto_NoCond
                TitoloPossesso = enum_TitoloPossesso.AffittoContratto

            Case AgriBio_FormaPossesso_Comodato_SiCond, AgriBio_FormaPossesso_Comodato_NoCond
                TitoloPossesso = enum_TitoloPossesso.Comodato

            Case Else
                TitoloPossesso = enum_TitoloPossesso.Altro

        End Select

        Return TitoloPossesso

    End Function

    '####################################################################
    Public Function formaPossessoCatasto_from_TitoloPossesso(ByVal TitoloPossesso As String) As String

        Dim formaPossessoCatasto As String = ""

        '    --FP	FORMA POSSESSO
        'Public Const AgriBio_FormaPossesso_Proprieta_SiCond As String = "FP001"
        'Public Const AgriBio_FormaPossesso_Proprieta_NoCond As String = "FP021"
        'Public Const AgriBio_FormaPossesso_Affitto_SiCond As String = "FP002"
        'Public Const AgriBio_FormaPossesso_Affitto_NoCond As String = "FP017"
        'Public Const AgriBio_FormaPossesso_Comodato_SiCond As String = "FP018"
        'Public Const AgriBio_FormaPossesso_Comodato_NoCond As String = "FP011"

        Select Case TitoloPossesso

            Case enum_TitoloPossesso.Proprieta
                formaPossessoCatasto = AgriBio_FormaPossesso_Proprieta_SiCond

            Case enum_TitoloPossesso.AffittoContratto
                formaPossessoCatasto = AgriBio_FormaPossesso_Affitto_SiCond

            Case enum_TitoloPossesso.Comodato
                formaPossessoCatasto = AgriBio_FormaPossesso_Comodato_SiCond

            Case Else
                formaPossessoCatasto = AgriBio_FormaPossesso_Comodato_AltreForme

        End Select

        Return formaPossessoCatasto

    End Function

    '##########################################
    Public Function MetodoProduzioneCod_from_tipoProduz(ByVal tipoProduz As String) As Integer

        Dim MetodoProduzioneCod As Integer = 0

        'Const AgriBio_MetodoProduzione_Convenzionale As String = "MP001"
        'Const AgriBio_MetodoProduzione_Conversione As String = "MP002"
        'Const AgriBio_MetodoProduzione_Biologico As String = "MP003"

        Select Case tipoProduz

            Case AgriBio_MetodoProduzione_Convenzionale
                MetodoProduzioneCod = enum_MetodoProduzione.Integrato

            Case AgriBio_MetodoProduzione_Conversione
                MetodoProduzioneCod = enum_MetodoProduzione.InConversione

            Case AgriBio_MetodoProduzione_Biologico
                MetodoProduzioneCod = enum_MetodoProduzione.Biologico

        End Select

        Return MetodoProduzioneCod

    End Function

    '##########################################
    Public Function TipologiaColturaCod_from_tipoColtura(ByVal tipoColtura As String) As Integer

        Dim TipologiaColturaCod As Integer = 0

        'Const AgriBio_TipologiaColtura_ArboreaPura As String = "TC001"
        'Const AgriBio_TipologiaColtura_ArboreaConsociata As String = "TC002"
        'Const AgriBio_TipologiaColtura_ErbaceaPura As String = "TC003"
        'Const AgriBio_TipologiaColtura_ErbaceaConsociata As String = "TC004"
        'Const AgriBio_TipologiaColtura_Promiscua As String = "TC005"

        Select Case tipoColtura

            Case AgriBio_TipologiaColtura_ArboreaPura
                TipologiaColturaCod = 1

            Case AgriBio_TipologiaColtura_ArboreaConsociata
                TipologiaColturaCod = 2

            Case AgriBio_TipologiaColtura_ErbaceaPura
                TipologiaColturaCod = 3

            Case AgriBio_TipologiaColtura_ErbaceaConsociata
                TipologiaColturaCod = 4

            Case AgriBio_TipologiaColtura_Promiscua
                TipologiaColturaCod = 5

        End Select

        Return TipologiaColturaCod

    End Function

    '##########################################
    Public Function OrientamentoProduttivoCod_from_codOrientamento(ByVal codOrientamento As String) As Integer

        Dim OrientamentoProduttivoCod As Integer = 0

        'Const AgriBio_OrientamentoProduttivo_Cerialicolo As String = "OP001"
        ' Const AgriBio_OrientamentoProduttivo_Orticolo As String = "OP002"
        'Const AgriBio_OrientamentoProduttivo_Frutticolo As String = "OP003"
        'Const AgriBio_OrientamentoProduttivo_Viticolo As String = "OP004"
        'Const AgriBio_OrientamentoProduttivo_Olivicolo As String = "OP005"
        'Const AgriBio_OrientamentoProduttivo_FloricoloVivaistico As String = "OP006"
        'Const AgriBio_OrientamentoProduttivo_ColtureIndustriali As String = "OP007"
        'Const AgriBio_OrientamentoProduttivo_Foraggero As String = "OP008"
        'Const AgriBio_OrientamentoProduttivo_SupConnesseAllev As String = "OP009"
        'Const AgriBio_OrientamentoProduttivo_Altro As String = "OP010"

        Select Case codOrientamento

            Case AgriBio_OrientamentoProduttivo_Cerialicolo
                OrientamentoProduttivoCod = 10

            Case AgriBio_OrientamentoProduttivo_Orticolo
                OrientamentoProduttivoCod = 20

            Case AgriBio_OrientamentoProduttivo_Frutticolo
                OrientamentoProduttivoCod = 30

            Case AgriBio_OrientamentoProduttivo_Viticolo
                OrientamentoProduttivoCod = 40

            Case AgriBio_OrientamentoProduttivo_Olivicolo
                OrientamentoProduttivoCod = 50

            Case AgriBio_OrientamentoProduttivo_FloricoloVivaistico
                OrientamentoProduttivoCod = 60

            Case AgriBio_OrientamentoProduttivo_ColtureIndustriali
                OrientamentoProduttivoCod = 70

            Case AgriBio_OrientamentoProduttivo_Foraggero
                OrientamentoProduttivoCod = 80

            Case AgriBio_OrientamentoProduttivo_SupConnesseAllev
                OrientamentoProduttivoCod = 90

            Case AgriBio_OrientamentoProduttivo_Altro
                OrientamentoProduttivoCod = 99

        End Select

        Return OrientamentoProduttivoCod

    End Function

    '####################################################
    Public Sub StrutturaStoccaggio_from_tiposubtipoProduz(ByVal tipoProduz As String, _
                                                            ByVal subtipoProduz As String, _
                                                            ByVal bio As String, _
                                                            ByVal conv As String, _
                                                            ByVal descrizione As String, _
                                                            ByRef S_Magazzini_Aziendali As Integer, _
                                                            ByRef S_Magazzini_Esterni As Integer, _
                                                            ByRef S_Magazzini_xMezziTecnici As Integer, _
                                                            ByRef S_Magazzini_xAttrezzature As Integer, _
                                                            ByRef S_Magazzini_xVenditaProdotti As Integer, _
                                                            ByRef S_Magazzini_Altro As Integer, _
                                                            ByRef S_Magazzini_Altro_Des As String, _
                                                            ByRef S_Sili_Aziendali As Integer, _
                                                            ByRef S_Sili_Esterni As Integer, _
                                                            ByRef S_Sili_StockGranaglie As Integer, _
                                                            ByRef S_Sili_StockColtureIndustriali As Integer, _
                                                            ByRef S_Sili_StockMangimi As Integer, _
                                                            ByRef S_Sili_PreparazioneInsilati As Integer, _
                                                            ByRef S_Sili_Altro As Integer, _
                                                            ByRef S_Sili_Altro_Des As String, _
                                                            ByRef S_Celle_Aziendali As Integer, _
                                                            ByRef S_Celle_Esterne As Integer, _
                                                            ByRef S_Celle_ProdVegetali As Integer, _
                                                            ByRef S_Celle_ProdZootecniche As Integer, _
                                                            ByRef S_Celle_Altro As Integer, _
                                                            ByRef S_Celle_Altro_Des As String, _
                                                            ByRef S_Impianti_Aziendali As Integer, _
                                                            ByRef S_Impianti_Esterni As Integer, _
                                                            ByRef S_Impianti_Altro As Integer, _
                                                            ByRef S_Impianti_Altro_Des As String)

        ''    --TV	TIPO PRODUZIONE VEGETALE
        ''STRUTTURE STOCCAGGIO
        'Public Const AgriBio_TipoProdVeg_Magazzini As String = "TV010"
        'Public Const AgriBio_TipoProdVeg_Sili As String = "TV011"
        'Public Const AgriBio_TipoProdVeg_CelleFrigorifere As String = "TV012"
        'Public Const AgriBio_TipoProdVeg_ImpiantiPreparazioni As String = "TV013"

        Dim objSezA_R As New AgronicaCoreBiologicoDAL.BIO_Notifica_SezA_Informazioni_R

        Select Case tipoProduz

            Case AgriBio_TipoProdVeg_Magazzini

                Select Case subtipoProduz

                    Case AgriBio_TipoProdVeg_Magazzini_Aziendali
                        S_Magazzini_Aziendali = objSezA_R.StrutturaStoccaggioValore_from_BioConv(bio, conv)
                    Case AgriBio_TipoProdVeg_Magazzini_Esterni
                        S_Magazzini_Esterni = objSezA_R.StrutturaStoccaggioValore_from_BioConv(bio, conv)
                    Case AgriBio_TipoProdVeg_Magazzini_MezziTecnici
                        S_Magazzini_xMezziTecnici = objSezA_R.StrutturaStoccaggioValore_from_BioConv(bio, conv)
                    Case AgriBio_TipoProdVeg_Magazzini_Attrezzature
                        S_Magazzini_xAttrezzature = objSezA_R.StrutturaStoccaggioValore_from_BioConv(bio, conv)
                    Case AgriBio_TipoProdVeg_Magazzini_Vendita
                        S_Magazzini_xVenditaProdotti = objSezA_R.StrutturaStoccaggioValore_from_BioConv(bio, conv)
                    Case AgriBio_TipoProdVeg_Magazzini_Altro
                        S_Magazzini_Altro = objSezA_R.StrutturaStoccaggioValore_from_BioConv(bio, conv)
                        S_Magazzini_Altro_Des = descrizione
                End Select
                '------------------------

            Case AgriBio_TipoProdVeg_Sili
                Select Case subtipoProduz

                    Case AgriBio_TipoProdVeg_Sili_Aziendali
                        S_Sili_Aziendali = objSezA_R.StrutturaStoccaggioValore_from_BioConv(bio, conv)
                    Case AgriBio_TipoProdVeg_Sili_Esterni
                        S_Sili_Esterni = objSezA_R.StrutturaStoccaggioValore_from_BioConv(bio, conv)
                    Case AgriBio_TipoProdVeg_Sili_StoccaggioGranaglie
                        S_Sili_StockGranaglie = objSezA_R.StrutturaStoccaggioValore_from_BioConv(bio, conv)
                    Case AgriBio_TipoProdVeg_Sili_StoccaggioColtIndustriale
                        S_Sili_StockColtureIndustriali = objSezA_R.StrutturaStoccaggioValore_from_BioConv(bio, conv)
                    Case AgriBio_TipoProdVeg_Sili_StoccaggioMangimi
                        S_Sili_StockMangimi = objSezA_R.StrutturaStoccaggioValore_from_BioConv(bio, conv)
                    Case AgriBio_TipoProdVeg_Sili_PreparazioniInsilati
                        S_Sili_PreparazioneInsilati = objSezA_R.StrutturaStoccaggioValore_from_BioConv(bio, conv)
                    Case AgriBio_TipoProdVeg_Sili_Altro
                        S_Sili_Altro = objSezA_R.StrutturaStoccaggioValore_from_BioConv(bio, conv)
                        S_Sili_Altro_Des = descrizione

                End Select
                '------------------------

            Case AgriBio_TipoProdVeg_CelleFrigorifere

                Select Case subtipoProduz
                    Case AgriBio_TipoProdVeg_CelleFrigorifere_Aziendali
                        S_Celle_Aziendali = objSezA_R.StrutturaStoccaggioValore_from_BioConv(bio, conv)
                    Case AgriBio_TipoProdVeg_CelleFrigorifere_Esterni
                        S_Celle_Esterne = objSezA_R.StrutturaStoccaggioValore_from_BioConv(bio, conv)
                    Case AgriBio_TipoProdVeg_CelleFrigorifere_ProdVeg
                        S_Celle_ProdVegetali = objSezA_R.StrutturaStoccaggioValore_from_BioConv(bio, conv)
                    Case AgriBio_TipoProdVeg_CelleFrigorifere_ProdZoo
                        S_Celle_ProdZootecniche = objSezA_R.StrutturaStoccaggioValore_from_BioConv(bio, conv)
                    Case AgriBio_TipoProdVeg_CelleFrigorifere_Altro
                        S_Celle_Altro = objSezA_R.StrutturaStoccaggioValore_from_BioConv(bio, conv)
                        S_Celle_Altro_Des = descrizione
                End Select
                '------------------------

            Case AgriBio_TipoProdVeg_ImpiantiPreparazioni

                Select Case subtipoProduz
                    Case AgriBio_TipoProdVeg_ImpiantiPreparazioni_Aziendali
                        S_Impianti_Aziendali = objSezA_R.StrutturaStoccaggioValore_from_BioConv(bio, conv)
                    Case AgriBio_TipoProdVeg_ImpiantiPreparazioni_Esterni
                        S_Impianti_Esterni = objSezA_R.StrutturaStoccaggioValore_from_BioConv(bio, conv)
                    Case AgriBio_TipoProdVeg_ImpiantiPreparazioni_Altro
                        S_Impianti_Altro = objSezA_R.StrutturaStoccaggioValore_from_BioConv(bio, conv)
                        S_Impianti_Altro_Des = descrizione
                End Select
                '------------------------

        End Select


    End Sub

    '####################################################
    Public Sub UBAAttivitaAllevamento_from_subTipoProduz(ByVal subtipoProduz As String, _
                                                           ByVal descrizione As String, _
                                                           ByVal biologico As Decimal, _
                                                           ByVal convenzionale As Decimal, _
                                                             ByRef AA10_UBABiologico As Decimal, _
                                                            ByRef AA10_UBAConvenzionale As Decimal, _
                                                            ByRef AA20_UBABiologico As Decimal, _
                                                            ByRef AA20_UBAConvenzionale As Decimal, _
                                                            ByRef AA30_UBABiologico As Decimal, _
                                                            ByRef AA30_UBAConvenzionale As Decimal, _
                                                            ByRef AA40_UBABiologico As Decimal, _
                                                            ByRef AA40_UBAConvenzionale As Decimal, _
                                                            ByRef AA50_UBABiologico As Decimal, _
                                                            ByRef AA50_UBAConvenzionale As Decimal, _
                                                            ByRef AA60_UBABiologico As Decimal, _
                                                            ByRef AA60_UBAConvenzionale As Decimal, _
                                                            ByRef AA60_IPRiproduzione As Integer, _
                                                            ByRef AA61_UBABiologico As Decimal, _
                                                            ByRef AA61_UBAConvenzionale As Decimal, _
                                                            ByRef AA70_UBABiologico As Decimal, _
                                                            ByRef AA70_UBAConvenzionale As Decimal, _
                                                            ByRef AA80_UBABiologico As Integer, _
                                                            ByRef AA80_UBAConvenzionale As Integer, _
                                                            ByRef AA90_UBABiologico As Decimal, _
                                                            ByRef AA90_UBAConvenzionale As Decimal, _
                                                            ByRef AA90_AltroDes As String)

        'Public Const AgriBio_SubTipoProdZoo_AA_Bovini As String = "SZ001"
        'Public Const AgriBio_SubTipoProdZoo_AA_Bufalini As String = "SZ002"
        'Public Const AgriBio_SubTipoProdZoo_AA_Ovini As String = "SZ003"
        'Public Const AgriBio_SubTipoProdZoo_AA_Caprini As String = "SZ004"
        'Public Const AgriBio_SubTipoProdZoo_AA_Equini As String = "SZ005"
        'Public Const AgriBio_SubTipoProdZoo_AA_Suini As String = "SZ006"
        'Public Const AgriBio_SubTipoProdZoo_AA_SuiniIngrasso As String = "SZ007"
        'Public Const AgriBio_SubTipoProdZoo_AA_Avicoli As String = "SZ008"
        'Public Const AgriBio_SubTipoProdZoo_AA_Api As String = "SZ009"
        'Public Const AgriBio_SubTipoProdZoo_AA_Altro As String = "SZ010"

        Select Case subtipoProduz

            Case AgriBio_SubTipoProdZoo_AA_Bovini
                AA10_UBABiologico = biologico
                AA10_UBAConvenzionale = convenzionale

            Case AgriBio_SubTipoProdZoo_AA_Bufalini
                AA20_UBABiologico = biologico
                AA20_UBAConvenzionale = convenzionale

            Case AgriBio_SubTipoProdZoo_AA_Ovini
                AA30_UBABiologico = biologico
                AA30_UBAConvenzionale = convenzionale

            Case AgriBio_SubTipoProdZoo_AA_Caprini
                AA40_UBABiologico = biologico
                AA40_UBAConvenzionale = convenzionale

            Case AgriBio_SubTipoProdZoo_AA_Equini
                AA50_UBABiologico = biologico
                AA50_UBAConvenzionale = convenzionale

            Case AgriBio_SubTipoProdZoo_AA_Suini
                AA60_UBABiologico = biologico
                AA60_UBAConvenzionale = convenzionale

            Case AgriBio_SubTipoProdZoo_AA_SuiniIngrasso
                AA61_UBABiologico = biologico
                AA61_UBAConvenzionale = convenzionale

            Case AgriBio_SubTipoProdZoo_AA_Avicoli
                AA70_UBABiologico = biologico
                AA70_UBAConvenzionale = convenzionale

            Case AgriBio_SubTipoProdZoo_AA_Api
                AA80_UBABiologico = biologico
                AA80_UBAConvenzionale = convenzionale

            Case AgriBio_SubTipoProdZoo_AA_Altro
                AA90_UBABiologico = biologico
                AA90_UBAConvenzionale = convenzionale
                AA90_AltroDes = descrizione

        End Select

    End Sub

    '####################################################
    Private Function Check_from_SiNo(ByVal SiNo As String) As Integer

        If SiNo.ToUpper = "S" Then
            Return 1
        Else
            Return 0
        End If

    End Function

    '####################################################
    'non si sa bene com'è salvata l'info nell'xml,
    'modificare se necessario
    Private Function CheckAltro_from_SiNo(ByVal SiNo As String, _
                                          ByVal descrizione As String, _
                                          ByRef Str_Altro As String _
                                          ) As Integer

        'If SiNo.ToUpper = "S" Then
        '    Return 1
        'Else
        '    Return 0
        'End If

        If descrizione <> "" Then
            Str_Altro = descrizione
            Return 1
        Else
            Return 0
        End If

    End Function

    '####################################################
    Public Sub IPAttivitaAllevamento_from_subTipoProduz(ByVal subtipoProduz As String, _
                                                           ByVal descrizione As String, _
                                                           ByVal carne As String, _
                                                           ByVal latte As String, _
                                                           ByVal riproduzione As String, _
                                                           ByVal prodaltro As String, _
                                                            ByRef AA10_IPCarne As Integer, _
                                                            ByRef AA10_IPLatte As Integer, _
                                                            ByRef AA10_IPRiproduzione As Integer, _
                                                            ByRef AA10_IPAltro As Integer, _
                                                            ByRef AA10_IPAltroDes As String, _
                                                            ByRef AA20_IPCarne As Integer, _
                                                            ByRef AA20_IPLatte As Integer, _
                                                            ByRef AA20_IPRiproduzione As Integer, _
                                                            ByRef AA20_IPAltro As Integer, _
                                                            ByRef AA20_IPAltroDes As String, _
                                                            ByRef AA30_IPCarne As Integer, _
                                                            ByRef AA30_IPLatte As Integer, _
                                                            ByRef AA30_IPRiproduzione As Integer, _
                                                            ByRef AA30_IPAltro As Integer, _
                                                            ByRef AA30_IPAltroDes As String, _
                                                            ByRef AA40_IPCarne As Integer, _
                                                            ByRef AA40_IPLatte As Integer, _
                                                            ByRef AA40_IPRiproduzione As Integer, _
                                                            ByRef AA40_IPAltro As Integer, _
                                                            ByRef AA40_IPAltroDes As String, _
                                                            ByRef AA50_IPCarne As Integer, _
                                                            ByRef AA50_IPRiproduzione As Integer, _
                                                            ByRef AA50_IPAltro As Integer, _
                                                            ByRef AA50_IPAltroDes As String, _
                                                            ByRef AA60_IPRiproduzione As Integer, _
                                                            ByRef AA60_IPAltro As Integer, _
                                                            ByRef AA60_IPAltroDes As String, _
                                                            ByRef AA61_IPCarne As Integer, _
                                                            ByRef AA70_IPCarne As Integer, _
                                                            ByRef AA70_IPUova As Integer, _
                                                            ByRef AA70_IPRiproduzione As Integer, _
                                                            ByRef AA70_IPAltro As Integer, _
                                                            ByRef AA70_IPAltroDes As String, _
                                                            ByRef AA80_IPMiele As Integer, _
                                                            ByRef AA80_IPPReale As Integer, _
                                                            ByRef AA80_IPCera As Integer, _
                                                            ByRef AA80_IPAltro As Integer, _
                                                            ByRef AA80_IPAltroDes As String, _
                                                            ByRef AA90_IPAltroDes As String)



        'Public Const AgriBio_SubTipoProdZoo_AA_Bovini As String = "SZ001"
        'Public Const AgriBio_SubTipoProdZoo_AA_Bufalini As String = "SZ002"
        'Public Const AgriBio_SubTipoProdZoo_AA_Ovini As String = "SZ003"
        'Public Const AgriBio_SubTipoProdZoo_AA_Caprini As String = "SZ004"
        'Public Const AgriBio_SubTipoProdZoo_AA_Equini As String = "SZ005"
        'Public Const AgriBio_SubTipoProdZoo_AA_Suini As String = "SZ006"
        'Public Const AgriBio_SubTipoProdZoo_AA_SuiniIngrasso As String = "SZ007"
        'Public Const AgriBio_SubTipoProdZoo_AA_Avicoli As String = "SZ008"
        'Public Const AgriBio_SubTipoProdZoo_AA_Api As String = "SZ009"
        'Public Const AgriBio_SubTipoProdZoo_AA_Altro As String = "SZ010"

        Select Case subtipoProduz

            Case AgriBio_SubTipoProdZoo_AA_Bovini
                AA10_IPCarne = Check_from_SiNo(carne)
                AA10_IPLatte = Check_from_SiNo(latte)
                AA10_IPRiproduzione = Check_from_SiNo(riproduzione)
                AA10_IPAltro = CheckAltro_from_SiNo("", prodaltro, AA10_IPAltroDes) '???????????

            Case AgriBio_SubTipoProdZoo_AA_Bufalini
                AA20_IPCarne = Check_from_SiNo(carne)
                AA20_IPLatte = Check_from_SiNo(latte)
                AA20_IPRiproduzione = Check_from_SiNo(riproduzione)
                AA20_IPAltro = CheckAltro_from_SiNo("", prodaltro, AA10_IPAltroDes) '???????????

            Case AgriBio_SubTipoProdZoo_AA_Ovini
                AA30_IPCarne = Check_from_SiNo(carne)
                AA30_IPLatte = Check_from_SiNo(latte)
                AA30_IPRiproduzione = Check_from_SiNo(riproduzione)
                AA30_IPAltro = CheckAltro_from_SiNo("", prodaltro, AA10_IPAltroDes) '???????????

            Case AgriBio_SubTipoProdZoo_AA_Caprini
                AA40_IPCarne = Check_from_SiNo(carne)
                AA40_IPLatte = Check_from_SiNo(latte)
                AA40_IPRiproduzione = Check_from_SiNo(riproduzione)
                AA40_IPAltro = CheckAltro_from_SiNo("", prodaltro, AA10_IPAltroDes) '???????????

            Case AgriBio_SubTipoProdZoo_AA_Equini
                AA50_IPCarne = Check_from_SiNo(carne)
                AA50_IPRiproduzione = Check_from_SiNo(riproduzione)
                AA50_IPAltro = CheckAltro_from_SiNo("", prodaltro, AA10_IPAltroDes) '???????????

            Case AgriBio_SubTipoProdZoo_AA_Suini
                AA60_IPRiproduzione = Check_from_SiNo(riproduzione)
                AA60_IPAltro = CheckAltro_from_SiNo("", prodaltro, AA10_IPAltroDes) '???????????

            Case AgriBio_SubTipoProdZoo_AA_SuiniIngrasso
                AA61_IPCarne = Check_from_SiNo(carne)

            Case AgriBio_SubTipoProdZoo_AA_Avicoli
                AA70_IPCarne = Check_from_SiNo(carne)
                AA70_IPUova = 0 '?????????
                AA70_IPRiproduzione = Check_from_SiNo(riproduzione)
                AA70_IPAltro = CheckAltro_from_SiNo("", prodaltro, AA10_IPAltroDes) '???????????

            Case AgriBio_SubTipoProdZoo_AA_Api
                AA80_IPMiele = 0 '?????????
                AA80_IPPReale = 0 '?????????
                AA80_IPCera = 0 '?????????
                AA80_IPAltro = CheckAltro_from_SiNo("", prodaltro, AA10_IPAltroDes) '???????????

            Case AgriBio_SubTipoProdZoo_AA_Altro
                AA90_IPAltroDes = descrizione

        End Select

    End Sub


    '####################################################
    Public Sub IndirizzoProduttivo_from_tipoProduz(ByVal tipoProduz As String, _
                                                   ByVal descrizione As String, _
                                                   ByRef IP_Cerealicolo As Integer, _
                                                    ByRef IP_Orticolo As Integer, _
                                                    ByRef IP_ColtureIndustriali As Integer, _
                                                    ByRef IP_Frutticolo As Integer, _
                                                    ByRef IP_Vitivinicolo As Integer, _
                                                    ByRef IP_Olivicolo As Integer, _
                                                    ByRef IP_Foraggero As Integer, _
                                                    ByRef IP_VivaisticoSementiero As Integer, _
                                                    ByRef IP_Altro As Integer, _
                                                    ByRef IP_Altro_Des As String)

        ''    --TV	TIPO PRODUZIONE VEGETALE
        ''INDIRIZZO PRODUTTIVO
        'Public Const AgriBio_TipoProdVeg_Cerealicolo As String = "TV001"
        'Public Const AgriBio_TipoProdVeg_Orticolo As String = "TV002"
        'Public Const AgriBio_TipoProdVeg_ColtIndustriali As String = "TV003"
        'Public Const AgriBio_TipoProdVeg_Frutticolo As String = "TV004"
        'Public Const AgriBio_TipoProdVeg_Vitivinicolo As String = "TV005"
        'Public Const AgriBio_TipoProdVeg_Olivicolo As String = "TV006"
        'Public Const AgriBio_TipoProdVeg_Foraggero As String = "TV007"
        'Public Const AgriBio_TipoProdVeg_VivSementiero As String = "TV008"
        ''STRUTTURE STOCCAGGIO
        'Public Const AgriBio_TipoProdVeg_Magazzini As String = "TV010"
        'Public Const AgriBio_TipoProdVeg_Sili As String = "TV011"
        'Public Const AgriBio_TipoProdVeg_CelleFrigorifere As String = "TV012"
        'Public Const AgriBio_TipoProdVeg_ImpiantiPreparazioni As String = "TV013"

        Select Case tipoProduz

            Case AgriBio_TipoProdVeg_Cerealicolo
                IP_Cerealicolo = 1

            Case AgriBio_TipoProdVeg_Orticolo
                IP_Orticolo = 1

            Case AgriBio_TipoProdVeg_ColtIndustriali
                IP_ColtureIndustriali = 1

            Case AgriBio_TipoProdVeg_Frutticolo
                IP_Frutticolo = 1

            Case AgriBio_TipoProdVeg_Vitivinicolo
                IP_Vitivinicolo = 1

            Case AgriBio_TipoProdVeg_Olivicolo
                IP_Olivicolo = 1

            Case AgriBio_TipoProdVeg_Foraggero
                IP_Foraggero = 1

            Case AgriBio_TipoProdVeg_VivSementiero
                IP_VivaisticoSementiero = 1

            Case Else
                IP_Altro = 1
                IP_Altro_Des = descrizione

        End Select

    End Sub

    '###############################################################################################
    Public Sub SubIndirizzoProduttivo_from_tiposubtipoProduz(ByVal tipoProduz As String, _
                                                                      ByVal subtipoProduz As String, _
                                                                      ByVal bio As String, _
                                                                      ByVal conv As String, _
                                                                      ByVal descrizione As String, _
                                                                       ByRef IP_Cerealicolo_Riso As Integer, _
                                                                        ByRef IP_Cerealicolo_GranoDuro As Integer, _
                                                                        ByRef IP_Cerealicolo_GranoTenero As Integer, _
                                                                        ByRef IP_Cerealicolo_Mais As Integer, _
                                                                        ByRef IP_Cerealicolo_AltriCereali As Integer, _
                                                                        ByRef IP_Orticolo_PienoCampo As Integer, _
                                                                        ByRef IP_Orticolo_ColturaProtetta As Integer, _
                                                                       ByRef IP_Frutticolo_Pomacee As Integer, _
                                                                        ByRef IP_Frutticolo_Drupacee As Integer, _
                                                                        ByRef IP_Frutticolo_Agrumi As Integer, _
                                                                        ByRef IP_Frutticolo_FruttaSecca As Integer, _
                                                                        ByRef IP_Frutticolo_UvaTavola As Integer, _
                                                                        ByRef IP_Frutticolo_Altro As Integer, _
                                                                        ByRef IP_Frutticolo_Altro_Des As String, _
                                                                        ByRef IP_Vitivinicolo_DaTavola As Integer, _
                                                                        ByRef IP_Vitivinicolo_Denominazione As Integer, _
                                                                        ByRef IP_Olivicolo_DaMensa As Integer, _
                                                                        ByRef IP_Olivicolo_DaOlio As Integer)

        Dim objSezA_R As New AgronicaCoreBiologicoDAL.BIO_Notifica_SezA_Informazioni_R


        Select Case tipoProduz

            Case AgriBio_TipoProdVeg_Cerealicolo

                Select Case subtipoProduz
                    Case AgriBio_SubTipoProdVeg_Cerealicolo_Riso
                        IP_Cerealicolo_Riso = objSezA_R.IndirizzoProduttivoValore_from_BioConv(bio, conv)
                    Case AgriBio_SubTipoProdVeg_Cerealicolo_FrumentoDuro
                        IP_Cerealicolo_GranoDuro = objSezA_R.IndirizzoProduttivoValore_from_BioConv(bio, conv)
                    Case AgriBio_SubTipoProdVeg_Cerealicolo_FrumentoTenero
                        IP_Cerealicolo_GranoTenero = objSezA_R.IndirizzoProduttivoValore_from_BioConv(bio, conv)
                    Case AgriBio_SubTipoProdVeg_Cerealicolo_Mais
                        IP_Cerealicolo_Mais = objSezA_R.IndirizzoProduttivoValore_from_BioConv(bio, conv)
                    Case AgriBio_SubTipoProdVeg_Cerealicolo_AltriCereali
                        IP_Cerealicolo_AltriCereali = objSezA_R.IndirizzoProduttivoValore_from_BioConv(bio, conv)
                End Select
                '------------------------

            Case AgriBio_TipoProdVeg_Orticolo
                Select Case subtipoProduz
                    Case AgriBio_SubTipoProdVeg_Orticolo_PienoCampo
                        IP_Orticolo_PienoCampo = objSezA_R.IndirizzoProduttivoValore_from_BioConv(bio, conv)
                    Case AgriBio_SubTipoProdVeg_Orticolo_colturaProtetta
                        IP_Orticolo_ColturaProtetta = objSezA_R.IndirizzoProduttivoValore_from_BioConv(bio, conv)
                End Select
                '------------------------

            Case AgriBio_TipoProdVeg_Frutticolo

                Select Case subtipoProduz
                    Case AgriBio_SubTipoProdVeg_Frutticolo_Pomacee
                        IP_Frutticolo_Pomacee = objSezA_R.IndirizzoProduttivoValore_from_BioConv(bio, conv)
                    Case AgriBio_SubTipoProdVeg_Frutticolo_Drupacee
                        IP_Frutticolo_Drupacee = objSezA_R.IndirizzoProduttivoValore_from_BioConv(bio, conv)
                    Case AgriBio_SubTipoProdVeg_Frutticolo_Agrumi
                        IP_Frutticolo_Agrumi = objSezA_R.IndirizzoProduttivoValore_from_BioConv(bio, conv)
                    Case AgriBio_SubTipoProdVeg_Frutticolo_FruttaSecca
                        IP_Frutticolo_FruttaSecca = objSezA_R.IndirizzoProduttivoValore_from_BioConv(bio, conv)
                    Case AgriBio_SubTipoProdVeg_Frutticolo_UvaDaTavola
                        IP_Frutticolo_UvaTavola = objSezA_R.IndirizzoProduttivoValore_from_BioConv(bio, conv)
                    Case AgriBio_SubTipoProdVeg_Frutticolo_Altro
                        IP_Frutticolo_Altro = objSezA_R.IndirizzoProduttivoValore_from_BioConv(bio, conv)
                        IP_Frutticolo_Altro_Des = descrizione
                End Select
                '------------------------

            Case AgriBio_TipoProdVeg_Vitivinicolo

                Select Case subtipoProduz
                    Case AgriBio_SubTipoProdVeg_Vitivinicolo_DaTavola
                        IP_Vitivinicolo_DaTavola = objSezA_R.IndirizzoProduttivoValore_from_BioConv(bio, conv)
                    Case AgriBio_SubTipoProdVeg_Vitivinicolo_Denominazione
                        IP_Vitivinicolo_Denominazione = objSezA_R.IndirizzoProduttivoValore_from_BioConv(bio, conv)
                End Select
                '------------------------

            Case AgriBio_TipoProdVeg_Olivicolo

                Select Case subtipoProduz
                    Case AgriBio_subTipoProdVeg_Olivicolo_DaMensa
                        IP_Olivicolo_DaMensa = objSezA_R.IndirizzoProduttivoValore_from_BioConv(bio, conv)
                    Case AgriBio_subTipoProdVeg_Olivicolo_DaOlio
                        IP_Olivicolo_DaOlio = objSezA_R.IndirizzoProduttivoValore_from_BioConv(bio, conv)
                End Select
                '------------------------

        End Select

    End Sub

    '###############################################################################################
    Public Sub InfoRiepProdVeg_from_tipoProduz_subtipoProduz(ByVal tipoProduz As String, _
                                                               ByVal subtipoProduz As String, _
                                                               ByRef IR_CEREALI As Integer, _
                                                               ByRef IR_Cereali_Granella As Integer, _
                                                                ByRef IR_Cereali_Conservazione As Integer, _
                                                                ByRef IR_Cereali_Sfarinati As Integer, _
                                                                ByRef IR_Cereali_Pastificazione As Integer, _
                                                                ByRef IR_Cereali_Panificazione As Integer, _
                                                                ByRef IR_Cereali_ProdottiDaForno As Integer, _
                                                                ByRef IR_Cereali_AltriProdotti As Integer, _
                                                                ByRef IR_COLTUREIND As Integer, _
                                                                ByRef IR_ColtureInd_Granella As Integer, _
                                                                ByRef IR_ColtureInd_Sfarinati As Integer, _
                                                                ByRef IR_ColtureIndi_EstrazioneOlio As Integer, _
                                                                ByRef IR_ColtureInd_Conservazione As Integer, _
                                                                ByRef IR_ColtureInd_Confezionamento As Integer, _
                                                                ByRef IR_ColtureInd_AltriProdotti As Integer, _
                                                                ByRef IR_ORTOFRUTTICOLI As Integer, _
                                                                ByRef IR_Ortofrutticoli_Freschi As Integer, _
                                                                ByRef IR_Ortofrutticoli_ConserveVegetali As Integer, _
                                                                ByRef IR_Ortofrutticoli_Conservazione As Integer, _
                                                                ByRef IR_Ortofrutticoli_Confezionamento As Integer, _
                                                                ByRef IR_VITIVINICOLO As Integer, _
                                                                ByRef IR_Vitivinicolo_Vinificazione As Integer, _
                                                                ByRef IR_Vitivinicolo_Mostificazione As Integer, _
                                                                ByRef IR_Vitivinicolo_Imbottigliamento As Integer, _
                                                                ByRef IR_OLEICOLO As Integer, _
                                                                ByRef IR_Oleicolo_ConserveVegetali As Integer, _
                                                                ByRef IR_Oleicolo_EstrazioneOlio As Integer, _
                                                                ByRef IR_Oleicolo_Imbottigliamento As Integer, _
                                                                ByRef IR_VIVSEMENTIERO As Integer, _
                                                                ByRef IR_VivSementiero_Semi As Integer, _
                                                                ByRef IR_VivSementiero_OrticoleTrapianto As Integer, _
                                                                ByRef IR_VivSementiero_Astoni As Integer, _
                                                                ByRef IR_VivSementiero_Barbatelle As Integer, _
                                                                ByRef IR_VivSementiero_Altro As Integer, _
                                                                ByRef IR_PIANTE_OFFICINALI As Integer, _
                                                                ByRef IR_PRODOTTI_SPONTANEI As Integer)


        ''    --TV	TIPO PRODUZIONE VEGETALE
        ''INFORMAZIONI RIEPILOGATIVE
        'Public Const AgriBio_TipoProdVeg_Cereali As String = "TV014"
        'Public Const AgriBio_TipoProdVeg_ColtIndustriali2 As String = "TV015"
        'Public Const AgriBio_TipoProdVeg_Ortofrutticoli As String = "TV016"
        'Public Const AgriBio_TipoProdVeg_CompVitivinicolo As String = "TV017"
        'Public Const AgriBio_TipoProdVeg_CompOleicolo As String = "TV018"
        'Public Const AgriBio_TipoProdVeg_CompVivSementiero As String = "TV019"
        'Public Const AgriBio_TipoProdVeg_ProdottiSpontanei As String = "TV020"

        Select Case tipoProduz

            Case AgriBio_TipoProdVeg_Cereali
                IR_CEREALI = 1

            Case AgriBio_TipoProdVeg_ColtIndustriali2
                IR_COLTUREIND = 1

            Case AgriBio_TipoProdVeg_Ortofrutticoli
                IR_ORTOFRUTTICOLI = 1

            Case AgriBio_TipoProdVeg_CompVitivinicolo
                IR_VITIVINICOLO = 1

            Case AgriBio_TipoProdVeg_CompOleicolo
                IR_OLEICOLO = 1

            Case AgriBio_TipoProdVeg_CompVivSementiero
                IR_VIVSEMENTIERO = 1

            Case AgriBio_TipoProdVeg_ProdottiSpontanei
                IR_PRODOTTI_SPONTANEI = 1

        End Select

        Select Case subtipoProduz

            '------------INFORMAZIONI RIEPILOGATIVE------------------
            Case AgriBio_SubTipoProdVeg_Cereali_Granella
                IR_Cereali_Granella = 1
            Case AgriBio_SubTipoProdVeg_Cereali_Conservazione
                IR_Cereali_Conservazione = 1
            Case AgriBio_SubTipoProdVeg_Cereali_Sfarinati
                IR_Cereali_Sfarinati = 1
            Case AgriBio_SubTipoProdVeg_Cereali_Panificazione
                IR_Cereali_Pastificazione = 1
            Case AgriBio_SubTipoProdVeg_Cereali_Pastificazione
                IR_Cereali_Panificazione = 1
            Case AgriBio_SubTipoProdVeg_Cereali_ProdottiForno
                IR_Cereali_ProdottiDaForno = 1
            Case AgriBio_SubTipoProdVeg_Cereali_Altro
                IR_Cereali_AltriProdotti = 1
                '-----------------------------------------
            Case AgriBio_SubTipoProdVeg_ColtIndustriali_Granella
                IR_ColtureInd_Granella = 1
            Case AgriBio_SubTipoProdVeg_ColtIndustriali_Sfarinati
                IR_ColtureInd_Sfarinati = 1
            Case AgriBio_SubTipoProdVeg_ColtIndustriali_EstrazioneOlio
                IR_ColtureIndi_EstrazioneOlio = 1
            Case AgriBio_SubTipoProdVeg_ColtIndustriali_Conservazione
                IR_ColtureInd_Conservazione = 1
            Case AgriBio_SubTipoProdVeg_ColtIndustriali_Confezionamento
                IR_ColtureInd_Confezionamento = 1
            Case AgriBio_SubTipoProdVeg_ColtIndustriali_Altro
                IR_ColtureInd_AltriProdotti = 1
                '-------------------------
            Case AgriBio_SubTipoProdVeg_Ortofrutticoli_Freschi
                IR_Ortofrutticoli_Freschi = 1
            Case AgriBio_SubTipoProdVeg_Ortofrutticoli_ConserveVegetali
                IR_Ortofrutticoli_ConserveVegetali = 1
            Case AgriBio_SubTipoProdVeg_Ortofrutticoli_Conservazione
                IR_Ortofrutticoli_Conservazione = 1
            Case AgriBio_SubTipoProdVeg_Ortofrutticoli_Confezionamento
                IR_Ortofrutticoli_Confezionamento = 1
                '--------------------------
            Case AgriBio_SubTipoProdVeg_Vitivinicolo_Vinificazione
                IR_Vitivinicolo_Vinificazione = 1
            Case AgriBio_SubTipoProdVeg_Vitivinicolo_Mostificazione
                IR_Vitivinicolo_Mostificazione = 1
            Case AgriBio_SubTipoProdVeg_Vitivinicolo_Imbottigliamento
                IR_Vitivinicolo_Imbottigliamento = 1
                '---------------------------
            Case AgriBio_SubTipoProdVeg_Oleolicolo_ConserveVegetali
                IR_Oleicolo_ConserveVegetali = 1
            Case AgriBio_SubTipoProdVeg_Oleolicolo_EstrazioneOlio
                IR_Oleicolo_EstrazioneOlio = 1
            Case AgriBio_SubTipoProdVeg_Oleolicolo_Imbottigliamento
                IR_Oleicolo_Imbottigliamento = 1
                '-------------------
            Case AgriBio_SubTipoProdVeg_VivSementiero_Semi
                IR_VivSementiero_Semi = 1
            Case AgriBio_SubTipoProdVeg_VivSementiero_Orticolo
                IR_VivSementiero_OrticoleTrapianto = 1
            Case AgriBio_SubTipoProdVeg_VivSementiero_Astoni
                IR_VivSementiero_Astoni = 1
            Case AgriBio_SubTipoProdVeg_VivSementiero_Barbatelle
                IR_VivSementiero_Barbatelle = 1
            Case AgriBio_SubTipoProdVeg_VivSementiero_Altro
                IR_VivSementiero_Altro = 1
            Case AgriBio_SubTipoProdVeg_VivSementiero_PianteOfficinali
                IR_PIANTE_OFFICINALI = 1

        End Select




    End Sub

    '###############################################################################################
    Public Sub InfoRiepProdZoo_from_tipoProduz_subtipoProduz(ByVal tipoProduz As String, _
                                                               ByVal subtipoProduz As String, _
                                                               ByVal descrizione As String, _
                                                                ByRef PZ_CARNE As Integer, _
                                                                ByRef PZ_Carne_CarneFresca As Integer, _
                                                                ByRef PZ_Carne_DerivatiCarne As Integer, _
                                                                ByRef PZ_Carne_Macellazione As Integer, _
                                                                ByRef PZ_Carne_Conservazione As Integer, _
                                                                ByRef PZ_Carne_Sezionamento As Integer, _
                                                                ByRef PZ_Carne_ProdottiSalumeria As Integer, _
                                                                ByRef PZ_Carne_Confezionamento As Integer, _
                                                                ByRef PZ_LATTE As Integer, _
                                                                ByRef PZ_Latte_LatteAlimentare As Integer, _
                                                                ByRef PZ_Latte_Caseificazione As Integer, _
                                                                ByRef PZ_Latte_Burro As Integer, _
                                                                ByRef PZ_Latte_Yogurt As Integer, _
                                                                ByRef PZ_Latte_AltriDerivatiLatte As Integer, _
                                                                ByRef PZ_Latte_Confezionamento As Integer, _
                                                                ByRef PZ_Latte_Altro As Integer, _
                                                                ByRef PZ_Latte_AltroDes As String, _
                                                                ByRef PZ_UOVA As Integer, _
                                                                ByRef PZ_Uova_Confezionamento As Integer, _
                                                                ByRef PZ_Uova_Altro As Integer, _
                                                                ByRef PZ_Uova_AltroDes As String, _
                                                                ByRef PZ_PRODOTTIAPICOLTURA As Integer, _
                                                                ByRef PZ_ProdottiApicoltura_Confezionamento As Integer, _
                                                                ByRef PZ_ALTRO As Integer, _
                                                                ByRef PZ_Altro_AltroDes As String)

        '    --TZ	TIPO PRODUZIONE ZOOTECNICA
        'Public Const AgriBio_TipoProdZoo_NoDesc As String = "TZ001"
        'Public Const AgriBio_TipoProdZoo_Carne As String = "TZ002"
        'Public Const AgriBio_TipoProdZoo_Latte As String = "TZ003"
        'Public Const AgriBio_TipoProdZoo_Uova As String = "TZ004"
        'Public Const AgriBio_TipoProdZoo_Apicoltura As String = "TZ005"
        'Public Const AgriBio_TipoProdZoo_Altro As String = "TZ006"

        Select Case tipoProduz

            Case AgriBio_TipoProdZoo_NoDesc '????????

            Case AgriBio_TipoProdZoo_Carne
                PZ_CARNE = 1

            Case AgriBio_TipoProdZoo_Latte
                PZ_LATTE = 1

            Case AgriBio_TipoProdZoo_Uova
                PZ_UOVA = 1

            Case AgriBio_TipoProdZoo_Apicoltura
                PZ_PRODOTTIAPICOLTURA = 1

            Case AgriBio_TipoProdZoo_Altro
                PZ_ALTRO = 1
                PZ_Altro_AltroDes = descrizione

        End Select

        '    --SZ	SUB TIPO PRODUZIONE ZOOTECNICA
        ''informazioni riepilogative delle attività connesse alle prod zootecniche
        Select Case subtipoProduz

            Case AgriBio_SubTipoProdZoo_Carne_CarneFresca
                PZ_Carne_CarneFresca = 1
            Case AgriBio_SubTipoProdZoo_Carne_DerivatiCarne
                PZ_Carne_DerivatiCarne = 1
            Case AgriBio_SubTipoProdZoo_Carne_Macellazione
                PZ_Carne_Macellazione = 1
            Case AgriBio_SubTipoProdZoo_Carne_Conservazione
                PZ_Carne_Conservazione = 1
            Case AgriBio_SubTipoProdZoo_Carne_Sezionamento
                PZ_Carne_Sezionamento = 1
            Case AgriBio_SubTipoProdZoo_Carne_Salumeria
                PZ_Carne_ProdottiSalumeria = 1
            Case AgriBio_SubTipoProdZoo_Carne_Confezionamento
                PZ_Carne_Confezionamento = 1
                ''------------
            Case AgriBio_SubTipoProdZoo_Latte_LatteAlimentare
                PZ_Latte_LatteAlimentare = 1
            Case AgriBio_SubTipoProdZoo_Latte_Caseificazione
                PZ_Latte_Caseificazione = 1
            Case AgriBio_SubTipoProdZoo_Latte_Burro
                PZ_Latte_Burro = 1
            Case AgriBio_SubTipoProdZoo_Latte_Yogurt
                PZ_Latte_Yogurt = 1
            Case AgriBio_SubTipoProdZoo_Latte_DerivatiLatte
                PZ_Latte_AltriDerivatiLatte = 1
            Case AgriBio_SubTipoProdZoo_Latte_Confezionamento
                PZ_Latte_Confezionamento = 1
            Case AgriBio_SubTipoProdZoo_Latte_Altro
                PZ_Latte_Altro = 1
                PZ_Latte_AltroDes = descrizione
                ''---------------
            Case AgriBio_SubTipoProdZoo_Uova_Confezionamento
                PZ_Uova_Confezionamento = 1

            Case AgriBio_SubTipoProdZoo_Uova_Altro
                PZ_Uova_Altro = 1
                PZ_Uova_AltroDes = descrizione
                ''---------------------
            Case AgriBio_SubTipoProdZoo_Apicoltura_Confezionamento
                PZ_PRODOTTIAPICOLTURA = 1
                PZ_ProdottiApicoltura_Confezionamento = descrizione
                ''---------------------
            Case AgriBio_SubTipoProdZoo_Apicoltura_Altro
                PZ_ALTRO = 1
                PZ_Altro_AltroDes = descrizione

        End Select

    End Sub

    '###############################################################################################
    Public Sub StrutturaStoccaggio_from_codStruttura_subcodStruttura(ByVal codStruttura As String, _
                                                               ByVal subcodStruttura As String, _
                                                               ByVal descrizione As String, _
                                                               ByVal metricubi As Decimal, _
                                                               ByRef STRUTTURE_SILI_mc As Decimal, _
                                                                ByRef STRUTTURE_SILI_StoccaggioCereali As Integer, _
                                                                ByRef STRUTTURE_SILI_StoccaggioProteoleaginose As Integer, _
                                                                ByRef STRUTTURE_SILI_Altro_Des As String, _
                                                                ByRef STRUTTURE_SILI_Altro As Integer, _
                                                                ByRef STRUTTURE_CELLE_mc As Decimal, _
                                                                ByRef STRUTTURE_CELLE_Vegetali As Integer, _
                                                                ByRef STRUTTURE_CELLE_Zootecnici As Integer, _
                                                                ByRef STRUTTURE_CELLE_Altro_Des As String, _
                                                                ByRef STRUTTURE_CELLE_Altro As Integer)

        '--PA	TIPO STRUTTURA PREPARAZIONE ALIMENTARE
        'PA001	SILI
        'PA002	CELLE FRIGORIFERE
        'PA003	MAGAZZINI
        Select Case codStruttura
            Case AgriBio_TipoStrutturaPrepAlim_Sili
                STRUTTURE_SILI_mc = metricubi
            Case AgriBio_TipoStrutturaPrepAlim_CelleFrigorifere
                STRUTTURE_CELLE_mc = metricubi
            Case AgriBio_TipoStrutturaPrepAlim_Magazzini
        End Select

        '--SA	SUB TIPO STRUTTURA PREPARAZIONE ALIMENTARE
        'SA001	STOCCAGGIO CEREALI
        'SA002	STOCCAGGIO PROTEOLEAGINOSE
        'SA003	ALTRO
        'SA004	PRODUZIONI VEGETALI
        'SA005	PRODUZIONI ZOOTECNICHE
        'SA006	ALTRO
        'SA007	_

        Select Case subcodStruttura

            Case AgriBio_SubTipoStrutturaPrepAlim_Sili_StoccaggioCereali
                STRUTTURE_SILI_StoccaggioCereali = 1

            Case AgriBio_SubTipoStrutturaPrepAlim_Sili_StoccaggioProteoleaginose
                STRUTTURE_SILI_StoccaggioProteoleaginose = 1

            Case AgriBio_SubTipoStrutturaPrepAlim_Sili_Altro
                STRUTTURE_SILI_Altro = 1
                STRUTTURE_SILI_Altro_Des = descrizione

            Case AgriBio_SubTipoStrutturaPrepAlim_CelleFrigorifere_ProdVegetali
                STRUTTURE_CELLE_Vegetali = 1

            Case AgriBio_SubTipoStrutturaPrepAlim_CelleFrigorifere_ProdZoo
                STRUTTURE_CELLE_Zootecnici = 1

            Case AgriBio_SubTipoStrutturaPrepAlim_CelleFrigorifere_Altro
                STRUTTURE_CELLE_Altro = 1
                STRUTTURE_CELLE_Altro_Des = descrizione

            Case AgriBio_SubTipoStrutturaPrepAlim_Magazzini_

        End Select

    End Sub

    '###############################################################################################
    Public Sub UtilizzoStrutturaStoccaggio_from_flagUtilizzo(ByVal flagUtilizzoDedicato As String, _
                                                                ByRef STRUTTURE_UTILIZZO_Dedicato As Integer, _
                                                                ByRef STRUTTURE_UTILIZZO_Misto As Integer)

        If flagUtilizzoDedicato.ToUpper = "S" Then
            STRUTTURE_UTILIZZO_Dedicato = 1
        Else
            STRUTTURE_UTILIZZO_Misto = 1
        End If

    End Sub

    '###############################################################################################
    Public Sub TipoCommercializzazione_from_codTipologia(ByVal codTipologia As String, _
                                                          ByVal descrizione As String, _
                                                             ByRef COMMERCIO_Ingrosso As Integer, _
                                                        ByRef COMMERCIO_Dettagliante As Integer, _
                                                        ByRef COMMERCIO_GDO As Integer, _
                                                        ByRef COMMERCIO_Altro As Integer, _
                                                        ByRef COMMERCIO_Amarchio As Integer, _
                                                        ByRef COMMERCIO_AMARCHIO_Des As String)

        '    --TP	TIPOLOGIA DI COMMERCIALIZZAZIONE PER PREPARAZIONE
        'TP001	INGROSSO
        'TP002	DETTAGLIO
        'TP003	G.D.O.
        'TP004	DISTRIBUZIONE A MARCHIO
        'TP005	ALTRO

        Select Case codTipologia

            Case AgriBio_TipoCommercio_Ingrosso
                COMMERCIO_Ingrosso = 1

            Case AgriBio_TipoCommercio_Dettaglio
                COMMERCIO_Dettagliante = 1

            Case AgriBio_TipoCommercio_GDO
                COMMERCIO_GDO = 1

            Case AgriBio_TipoCommercio_SistribuzioneMarchio
                COMMERCIO_Amarchio = 1
                COMMERCIO_AMARCHIO_Des = descrizione

            Case AgriBio_TipoCommercio_Altro
                COMMERCIO_Altro = 1

        End Select

    End Sub

    '###############################################################################################
    Public Sub TipoCommercializzazioneBioConvPeriodicaContinuativa_from_flag(ByVal flagBioPeriodica As String, _
                                                                             ByVal flagConvPeriodica As String, _
                                                                             ByVal flagBioContinuativa As String, _
                                                                             ByVal flagConvContinuativa As String, _
                                                                              ByRef COMMERCIO_PERIODICA_Bio As Integer, _
                                                                           ByRef COMMERCIO_PERIODICA_Conv As Integer, _
                                                                            ByRef COMMERCIO_CONTINUATIVA_Bio As Integer, _
                                                                            ByRef COMMERCIO_CONTINUATIVA_Conv As Integer)

        COMMERCIO_PERIODICA_Bio = Check_from_SiNo(flagBioPeriodica)
        COMMERCIO_PERIODICA_Conv = Check_from_SiNo(flagConvPeriodica)
        COMMERCIO_CONTINUATIVA_Bio = Check_from_SiNo(flagBioContinuativa)
        COMMERCIO_CONTINUATIVA_Conv = Check_from_SiNo(flagConvContinuativa)

    End Sub

    '###############################################################################################
    Public Sub PreparazioneBioConvPeriodicaContinuativa_from_flag(ByVal flagBioPeriodica As String, _
                                                                    ByVal flagConvPeriodica As String, _
                                                                    ByVal flagBioContinuativa As String, _
                                                                    ByVal flagConvContinuativa As String, _
                                                                    ByRef CaratteristichePA_Periodica_Bio As Integer, _
                                                                    ByRef CaratteristichePA_Continuativa_Bio As Integer, _
                                                                    ByRef CaratteristichePA_Periodica_Conv As Integer, _
                                                                    ByRef CaratteristichePA_Continuativa_Conv As Integer, _
                                                                    ByRef CaratteristichePA_Flag_Periodica_Continuativa As Integer)

        CaratteristichePA_Periodica_Bio = Check_from_SiNo(flagBioPeriodica)
        CaratteristichePA_Periodica_Conv = Check_from_SiNo(flagConvPeriodica)
        CaratteristichePA_Continuativa_Bio = Check_from_SiNo(flagBioContinuativa)
        CaratteristichePA_Continuativa_Conv = Check_from_SiNo(flagConvContinuativa)

        'vecchio flag, viene valorizzato sempre a 1 ora
        CaratteristichePA_Flag_Periodica_Continuativa = 1

    End Sub

    '###############################################################################################
    Public Sub CapacitaLavorativaStoccaggio_from_tipoPreparaz( _
                                                     ByVal tipoPreparaz As String, _
                                                    ByVal capacitaLavoro As String, _
                                                    ByVal idUnimisLav As String, _
                                                    ByVal capacitaStoccaggio As String, _
                                                    ByVal idUnimisStocc As String, _
                                                    ByRef CaratteristichePA_TipoDes As String, _
                                                    ByRef CaratteristichePA_CapacitaLavoro As Decimal, _
                                                    ByRef CaratteristichePA_CapacitaStoccaggio As Decimal, _
                                                    ByRef CaratteristichePA_CapacitaLavoro_UdmCod As Integer, _
                                                    ByRef CaratteristichePA_CapacitaLavoroTempo_UdmCod As Integer, _
                                                    ByRef CaratteristichePA_CapacitaStoccaggio_UdmCod As Integer)

        If capacitaLavoro <> "" Then
            CaratteristichePA_CapacitaLavoro = capacitaLavoro
        End If
        If capacitaStoccaggio <> "" Then
            CaratteristichePA_CapacitaStoccaggio = capacitaStoccaggio
        End If

        If idUnimisLav <> "" Then
            CaratteristichePA_CapacitaLavoro_UdmCod = UdmCod_from_idUnimis(idUnimisLav, CaratteristichePA_CapacitaLavoroTempo_UdmCod)
        End If

        If idUnimisStocc <> "" Then
            CaratteristichePA_CapacitaStoccaggio_UdmCod = UdmCod_from_idUnimis(idUnimisStocc, Nothing)
        End If

    End Sub

    '###############################################################################################
    Public Function UdmCod_from_idUnimis(ByVal idUnimis As String, _
                                 ByRef Udm_Cod_Tempo As Integer) As Integer

        Dim Udm_Cod As Integer = 0

        Select Case idUnimis

            Case AgriBio_UnitaMisura_Litri
                Udm_Cod = enum_UnitaMisura.Litri

            Case AgriBio_UnitaMisura_Tone
                Udm_Cod = enum_UnitaMisura.Tonnellate

            Case AgriBio_UnitaMisura_Metri
                Udm_Cod = enum_UnitaMisura.Metri

            Case AgriBio_UnitaMisura_Ore
                Udm_Cod = enum_UnitaMisura.Ore

            Case AgriBio_UnitaMisura_Kg
                Udm_Cod = enum_UnitaMisura.KG

            Case AgriBio_UnitaMisura_MetriCubi
                Udm_Cod = enum_UnitaMisura.Metri_Cubi

            Case AgriBio_UnitaMisura_MetriQuadri
                Udm_Cod = enum_UnitaMisura.MetriQuadri

            Case AgriBio_UnitaMisura_Quintali
                Udm_Cod = enum_UnitaMisura.Quintali

            Case AgriBio_UnitaMisura_Ettolitri
                Udm_Cod = enum_UnitaMisura.Ettolitro

                '-----------------------

            Case AgriBio_UnitaMisura_Unita_Ora
                Udm_Cod_Tempo = enum_UnitaMisura.Ore
                Udm_Cod = 0

            Case AgriBio_UnitaMisura_Qli_Ora
                Udm_Cod = enum_UnitaMisura.Quintali
                Udm_Cod_Tempo = enum_UnitaMisura.Ore

            Case AgriBio_UnitaMisura_Litri_Ora
                Udm_Cod = enum_UnitaMisura.Litri
                Udm_Cod_Tempo = enum_UnitaMisura.Ore

            Case AgriBio_UnitaMisura_Kg_Ora
                Udm_Cod = enum_UnitaMisura.KG
                Udm_Cod_Tempo = enum_UnitaMisura.Ore

            Case AgriBio_UnitaMisura_Kg_Giorno
                Udm_Cod = enum_UnitaMisura.KG
                Udm_Cod_Tempo = enum_UnitaMisura.Giorni

            Case AgriBio_UnitaMisura_Qli_Giorno
                Udm_Cod = enum_UnitaMisura.Quintali
                Udm_Cod_Tempo = enum_UnitaMisura.Giorni

            Case AgriBio_UnitaMisura_Unita_Giorno
                Udm_Cod_Tempo = enum_UnitaMisura.Giorni
                Udm_Cod = 0
                '----------------------

            Case AgriBio_UnitaMisura_Unita
                Udm_Cod = 0

            Case AgriBio_UnitaMisura_Km
                Udm_Cod = 0

            Case AgriBio_UnitaMisura_Pezzi
                Udm_Cod = 0

            Case AgriBio_UnitaMisura_Altro
                Udm_Cod = 0

        End Select

        Return Udm_Cod

    End Function

    '###############################################################################################
    Public Sub TipoPreparazioneDes_from_tipoPreparaz(ByVal tipoPreparaz As String, _
                                                   ByRef CaratteristichePA_TipoDes As String)

        Select Case tipoPreparaz

            Case AgriBio_TipoPrep_MPVegetale
                CaratteristichePA_TipoDes = "DA MAT PRIMA VEGETALE"

            Case AgriBio_TipoPrep_Molitura
                CaratteristichePA_TipoDes = "Molitura e affini"

            Case AgriBio_TipoPrep_Fioccatura
                CaratteristichePA_TipoDes = "Fioccatura e tostature"

            Case AgriBio_TipoPrep_Pastificazione
                CaratteristichePA_TipoDes = "Pastificazione"

            Case AgriBio_TipoPrep_ConserveVegetali
                CaratteristichePA_TipoDes = "Conserve vegetali"

            Case AgriBio_TipoPrep_Integratori
                CaratteristichePA_TipoDes = "Integratori alimentari"

            Case AgriBio_TipoPrep_EstrazioneOlio
                CaratteristichePA_TipoDes = "Estrazione di olio"

            Case AgriBio_TipoPrep_Vinificazione
                CaratteristichePA_TipoDes = "Vinificazione"

            Case AgriBio_TipoPrep_LiquoriDistillati
                CaratteristichePA_TipoDes = "Liquori e distillati"

            Case AgriBio_TipoPrep_Imbottigliamento
                CaratteristichePA_TipoDes = "Imbottigliamento"

            Case AgriBio_TipoPrep_ProdErboristici
                CaratteristichePA_TipoDes = "Prodotti erboristici"

            Case AgriBio_TipoPrep_MPAnimale
                CaratteristichePA_TipoDes = "DA MAT. PRIMA ANIMALE"

            Case AgriBio_TipoPrep_Macellazione
                CaratteristichePA_TipoDes = "Macellazione"

            Case AgriBio_TipoPrep_Sezionamento
                CaratteristichePA_TipoDes = "Sezionamento"

            Case AgriBio_TipoPrep_DerivatiCarne
                CaratteristichePA_TipoDes = "Derivati della carne"

            Case AgriBio_TipoPrep_ConserveAnimali
                CaratteristichePA_TipoDes = "Conserve animali"

            Case AgriBio_TipoPrep_ProdSalumeria
                CaratteristichePA_TipoDes = "Prodotti di salumeria"

            Case AgriBio_TipoPrep_Latte
                CaratteristichePA_TipoDes = "Latte alimentare"

            Case AgriBio_TipoPrep_Caseificazioni
                CaratteristichePA_TipoDes = "Caseificazione"

            Case AgriBio_TipoPrep_Burro
                CaratteristichePA_TipoDes = "Burro"

            Case AgriBio_TipoPrep_Yogurt
                CaratteristichePA_TipoDes = "Yogurt"

            Case AgriBio_TipoPrep_AltroProdAnimali
                CaratteristichePA_TipoDes = "Altri prodotti animali"

            Case AgriBio_TipoPrep_IndDolciaria
                CaratteristichePA_TipoDes = "INDUSTRIA DOLCIARIA"

            Case AgriBio_TipoPrep_ProdottiForno
                CaratteristichePA_TipoDes = "Prodotti da forno"

            Case AgriBio_TipoPrep_AltriProdDolciari
                CaratteristichePA_TipoDes = "Altri prodotti dolciari"

            Case AgriBio_TipoPrep_Mangimi
                CaratteristichePA_TipoDes = "Mangimi"

            Case AgriBio_TipoPrep_Etichettatura
                CaratteristichePA_TipoDes = "ETICHETTATURA"

            Case AgriBio_TipoPrep_Altro
                CaratteristichePA_TipoDes = "Altro"

            Case AgriBio_TipoPrep_Immagazzinamento
                CaratteristichePA_TipoDes = "IMMAGAZZINAMENTO"

            Case AgriBio_TipoPrep_Ortofrutticoli
                CaratteristichePA_TipoDes = "Ortofrutticoli freschi/secchi"

            Case AgriBio_TipoPrep_ProdSurgelati
                CaratteristichePA_TipoDes = "Prodotti surgelati"

            Case AgriBio_TipoPrep_Porzionatura
                CaratteristichePA_TipoDes = "Porzionatura"

            Case AgriBio_TipoPrep_UovaDerivati
                CaratteristichePA_TipoDes = "Uova e derivati"


        End Select

    End Sub

    '###############################################################################################
    Public Sub TipologiaAttivitaPrep_from_tipoProduz_subtipoProduz(ByVal tipoProduz As String, _
                                                               ByVal subtipoProduz As String, _
                                                               ByVal cproprio As String, _
                                                               ByVal cterzi As String, _
                                                               ByVal comm As String, _
                                                               ByVal descrizione As String, _
                                                                ByRef PA_VEGETALE_Cp As Integer, _
                                                                ByRef PA_VEGETALE_Ct As Integer, _
                                                                ByRef PA_VEGETALE_Cm As Integer, _
                                                                    ByRef PA_Vegetale_Ortofrutticoli_Cp As Integer, _
                                                                    ByRef PA_Vegetale_Ortofrutticoli_Ct As Integer, _
                                                                    ByRef PA_Vegetale_Ortofrutticoli_Cm As Integer, _
                                                                    ByRef PA_Vegetale_Molinatura_Cp As Integer, _
                                                                    ByRef PA_Vegetale_Molinatura_Ct As Integer, _
                                                                      ByRef PA_Vegetale_Molinatura_Cm As Integer, _
                                                                    ByRef PA_Vegetale_Fioccatura_Cp As Integer, _
                                                                    ByRef PA_Vegetale_Fioccatura_Ct As Integer, _
                                                                    ByRef PA_Vegetale_Fioccatura_Cm As Integer, _
                                                                    ByRef PA_Vegetale_Pastificazione_Cp As Integer, _
                                                                    ByRef PA_Vegetale_Pastificazione_Ct As Integer, _
                                                                    ByRef PA_Vegetale_Pastificazione_Cm As Integer, _
                                                                    ByRef PA_Vegetale_Surgelati_Cp As Integer, _
                                                                    ByRef PA_Vegetale_Surgelati_Ct As Integer, _
                                                                    ByRef PA_Vegetale_Surgelati_Cm As Integer, _
                                                                    ByRef PA_Vegetale_Conserve_Cp As Integer, _
                                                                    ByRef PA_Vegetale_Conserve_Ct As Integer, _
                                                                    ByRef PA_Vegetale_Conserve_Cm As Integer, _
                                                                    ByRef PA_Vegetale_IntegratoriAlimentari_Cp As Integer, _
                                                                    ByRef PA_Vegetale_IntegratoriAlimentari_Ct As Integer, _
                                                                    ByRef PA_Vegetale_IntegratoriAlimentari_Cm As Integer, _
                                                                    ByRef PA_Vegetale_EstrazioneOlio_Cp As Integer, _
                                                                    ByRef PA_Vegetale_EstrazioneOlio_Ct As Integer, _
                                                                    ByRef PA_Vegetale_EstrazioneOlio_Cm As Integer, _
                                                                    ByRef PA_Vegetale_Vinificazione_Cp As Integer, _
                                                                    ByRef PA_Vegetale_Vinificazione_Ct As Integer, _
                                                                    ByRef PA_Vegetale_Vinificazione_Cm As Integer, _
                                                                    ByRef PA_Vegetale_Liquori_Cp As Integer, _
                                                                    ByRef PA_Vegetale_Liquori_Ct As Integer, _
                                                                    ByRef PA_Vegetale_Liquori_Cm As Integer, _
                                                                    ByRef PA_Vegetale_Imbottigliamento_Cp As Integer, _
                                                                     ByRef PA_Vegetale_Imbottigliamento_Ct As Integer, _
                                                                    ByRef PA_Vegetale_Imbottigliamento_Cm As Integer, _
                                                                    ByRef PA_Vegetale_ProdottiErboristici_Cp As Integer, _
                                                                    ByRef PA_Vegetale_ProdottiErboristici_Ct As Integer, _
                                                                    ByRef PA_Vegetale_ProdottiErboristici_Cm As Integer, _
                                                                    ByRef PA_Vegetale_AltroDes_Cp As Integer, _
                                                                    ByRef PA_Vegetale_AltroDes_Ct As Integer, _
                                                                    ByRef PA_Vegetale_AltroDes_Cm As Integer, _
                                                                    ByRef PA_Vegetale_AltroDes As String, _
                                                                    ByRef PA_ANIMALE_Cp As Integer, _
                                                                    ByRef PA_ANIMALE_Ct As Integer, _
                                                                    ByRef PA_ANIMALE_Cm As Integer, _
                                                                    ByRef PA_Animale_Porzionatura_Cp As Integer, _
                                                                    ByRef PA_Animale_Porzionatura_Ct As Integer, _
                                                                    ByRef PA_Animale_Porzionatura_Cm As Integer, _
                                                                    ByRef PA_Animale_Macellazione_Cp As Integer, _
                                                                    ByRef PA_Animale_Macellazione_Ct As Integer, _
                                                                    ByRef PA_Animale_Macellazione_Cm As Integer, _
                                                                    ByRef PA_Animale_Sezionamento_Cp As Integer, _
                                                                    ByRef PA_Animale_Sezionamento_Ct As Integer, _
                                                                    ByRef PA_Animale_Sezionamento_Cm As Integer, _
                                                                    ByRef PA_Animale_DerivatiCarne_Cp As Integer, _
                                                                    ByRef PA_Animale_DerivatiCarne_Ct As Integer, _
                                                                    ByRef PA_Animale_DerivatiCarne_Cm As Integer, _
                                                                    ByRef PA_Animale_ConserveAnimali_Cp As Integer, _
                                                                    ByRef PA_Animale_ConserveAnimali_Ct As Integer, _
                                                                    ByRef PA_Animale_ConserveAnimali_Cm As Integer, _
                                                                    ByRef PA_Animale_ProdottiSalumeria_Cp As Integer, _
                                                                    ByRef PA_Animale_ProdottiSalumeria_Ct As Integer, _
                                                                    ByRef PA_Animale_ProdottiSalumeria_Cm As Integer, _
                                                                    ByRef PA_Animale_LatteAlimentare_Cp As Integer, _
                                                                    ByRef PA_Animale_LatteAlimentare_Ct As Integer, _
                                                                    ByRef PA_Animale_LatteAlimentare_Cm As Integer, _
                                                                    ByRef PA_Animale_Caseificazione_Cp As Integer, _
                                                                    ByRef PA_Animale_Caseificazione_Ct As Integer, _
                                                                    ByRef PA_Animale_Caseificazione_Cm As Integer, _
                                                                    ByRef PA_Animale_Burro_Cp As Integer, _
                                                                    ByRef PA_Animale_Burro_Ct As Integer, _
                                                                    ByRef PA_Animale_Burro_Cm As Integer, _
                                                                    ByRef PA_Animale_Yogurt_Cm As Integer, _
                                                                    ByRef PA_Animale_Yogurt_Cp As Integer, _
                                                                    ByRef PA_Animale_Yogurt_Ct As Integer, _
                                                                    ByRef PA_Animale_Uova_Cp As Integer, _
                                                                    ByRef PA_Animale_Uova_Ct As Integer, _
                                                                    ByRef PA_Animale_Uova_Cm As Integer, _
                                                                    ByRef PA_Animale_Altro_Cp As Integer, _
                                                                    ByRef PA_Animale_Altro_Ct As Integer, _
                                                                    ByRef PA_Animale_Altro_Cm As Integer, _
                                                                    ByRef PA_Animale_AltroDes As String, _
                                                                    ByRef PA_INDUSTRIADOLCIARIA_Cp As Integer, _
                                                                    ByRef PA_INDUSTRIADOLCIARIA_Ct As Integer, _
                                                                    ByRef PA_IndustriaDolciaria_DaForno_Cp As Integer, _
                                                                    ByRef PA_IndustriaDolciaria_DaForno_Ct As Integer, _
                                                                    ByRef PA_IndustriaDolciaria_DaForno_Cm As Integer, _
                                                                    ByRef PA_IndustriaDolciaria_ProdottiDolciari_Cp As Integer, _
                                                                    ByRef PA_IndustriaDolciaria_ProdottiDolciari_Ct As Integer, _
                                                                    ByRef PA_IndustriaDolciaria_AltriProdotti_Cp As Integer, _
                                                                    ByRef PA_IndustriaDolciaria_AltriProdotti_Ct As Integer, _
                                                                    ByRef PA_IndustriaDolciaria_AltriProdotti_Cm As Integer, _
                                                                    ByRef PA_MANGIMI_Cp As Integer, _
                                                                    ByRef PA_MANGIMI_Ct As Integer, _
                                                                    ByRef PA_MANGIMI_Cm As Integer, _
                                                                    ByRef PA_Mangimi_AltroDes_Cp As Integer, _
                                                                    ByRef PA_Mangimi_AltroDes_Ct As Integer, _
                                                                    ByRef PA_Mangimi_AltroDes_Cm As Integer, _
                                                                     ByRef PA_Mangimi_AltroDes As String, _
                                                                    ByRef PA_IMMAGAZZINAMENTO_Cp As Integer, _
                                                                    ByRef PA_IMMAGAZZINAMENTO_Ct As Integer, _
                                                                    ByRef PA_CONSERVAZIONE_Cp As Integer, _
                                                                    ByRef PA_CONSERVAZIONE_Ct As Integer, _
                                                                    ByRef PA_CONDIZIONAMENTO_Cp As Integer, _
                                                                    ByRef PA_CONDIZIONAMENTO_Ct As Integer, _
                                                                    ByRef PA_CONFEZIONAMENTO_Cp As Integer, _
                                                                    ByRef PA_CONFEZIONAMENTO_Ct As Integer, _
                                                                    ByRef PA_ETICHETTATURA_Cp As Integer, _
                                                                    ByRef PA_ETICHETTATURA_Ct As Integer, _
                                                                    ByRef PA_ALTRO_Cp As Integer, _
                                                                    ByRef PA_ALTRO_Ct As Integer, _
                                                                    ByRef PA_ALTRODes As String)

        Select Case tipoProduz

            Case AgriBio_TipoProdAlim_MPVegetale
                PA_VEGETALE_Cp = Check_from_SiNo(cproprio)
                PA_VEGETALE_Ct = Check_from_SiNo(cterzi)
                PA_VEGETALE_Cm = Check_from_SiNo(comm)

            Case AgriBio_TipoProdAlim_MPAnimale
                PA_ANIMALE_Cp = Check_from_SiNo(cproprio)
                PA_ANIMALE_Ct = Check_from_SiNo(cterzi)
                PA_ANIMALE_Cm = Check_from_SiNo(comm)

            Case AgriBio_TipoProdAlim_IndustriaDolciaria
                PA_INDUSTRIADOLCIARIA_Cp = Check_from_SiNo(cproprio)
                PA_INDUSTRIADOLCIARIA_Ct = Check_from_SiNo(cterzi)

            Case AgriBio_TipoProdAlim_Mangimi
                PA_MANGIMI_Cp = Check_from_SiNo(cproprio)
                PA_MANGIMI_Ct = Check_from_SiNo(cterzi)
                PA_MANGIMI_Cm = Check_from_SiNo(comm)

            Case AgriBio_TipoProdAlim_Altro
                PA_Mangimi_AltroDes_Cp = Check_from_SiNo(cproprio)
                PA_Mangimi_AltroDes_Ct = Check_from_SiNo(cterzi)
                PA_Mangimi_AltroDes_Cm = Check_from_SiNo(comm)
                PA_Mangimi_AltroDes = descrizione

            Case AgriBio_TipoProdAlim_Conservazione
                PA_CONSERVAZIONE_Cp = Check_from_SiNo(cproprio)
                PA_CONSERVAZIONE_Ct = Check_from_SiNo(cterzi)

            Case AgriBio_TipoProdAlim_Condizionamento
                PA_CONDIZIONAMENTO_Cp = Check_from_SiNo(cproprio)
                PA_CONDIZIONAMENTO_Ct = Check_from_SiNo(cterzi)

            Case AgriBio_TipoProdAlim_Immagazzinamento
                PA_IMMAGAZZINAMENTO_Cp = Check_from_SiNo(cproprio)
                PA_IMMAGAZZINAMENTO_Ct = Check_from_SiNo(cterzi)

            Case AgriBio_TipoProdAlim_Etichettatura
                PA_ETICHETTATURA_Cp = Check_from_SiNo(cproprio)
                PA_ETICHETTATURA_Ct = Check_from_SiNo(cterzi)

            Case AgriBio_TipoProdAlim_Confezionamento
                PA_CONFEZIONAMENTO_Cp = Check_from_SiNo(cproprio)
                PA_CONFEZIONAMENTO_Ct = Check_from_SiNo(cterzi)

        End Select


        Select Case subtipoProduz

            Case AgriBio_subTipoProdAlim_Molitura
                PA_Vegetale_Molinatura_Cp = Check_from_SiNo(cproprio)
                PA_Vegetale_Molinatura_Ct = Check_from_SiNo(cterzi)
                PA_Vegetale_Molinatura_Cm = Check_from_SiNo(comm)

            Case AgriBio_subTipoProdAlim_Fioccatura
                PA_Vegetale_Fioccatura_Cp = Check_from_SiNo(cproprio)
                PA_Vegetale_Fioccatura_Ct = Check_from_SiNo(cterzi)
                PA_Vegetale_Fioccatura_Cm = Check_from_SiNo(comm)

            Case AgriBio_subTipoProdAlim_Pastificazione
                PA_Vegetale_Pastificazione_Cp = Check_from_SiNo(cproprio)
                PA_Vegetale_Pastificazione_Ct = Check_from_SiNo(cterzi)
                PA_Vegetale_Pastificazione_Cm = Check_from_SiNo(comm)

            Case AgriBio_subTipoProdAlim_ConserveVegetali
                PA_Vegetale_Conserve_Cp = Check_from_SiNo(cproprio)
                PA_Vegetale_Conserve_Ct = Check_from_SiNo(cterzi)
                PA_Vegetale_Conserve_Cm = Check_from_SiNo(comm)

            Case AgriBio_subTipoProdAlim_Integratori
                PA_Vegetale_IntegratoriAlimentari_Cp = Check_from_SiNo(cproprio)
                PA_Vegetale_IntegratoriAlimentari_Ct = Check_from_SiNo(cterzi)
                PA_Vegetale_IntegratoriAlimentari_Cm = Check_from_SiNo(comm)

            Case AgriBio_subTipoProdAlim_EstrazioneOlio
                PA_Vegetale_EstrazioneOlio_Cp = Check_from_SiNo(cproprio)
                PA_Vegetale_EstrazioneOlio_Ct = Check_from_SiNo(cterzi)
                PA_Vegetale_EstrazioneOlio_Cm = Check_from_SiNo(comm)

            Case AgriBio_subTipoProdAlim_Vinificazione
                PA_Vegetale_Vinificazione_Cp = Check_from_SiNo(cproprio)
                PA_Vegetale_Vinificazione_Ct = Check_from_SiNo(cterzi)
                PA_Vegetale_Vinificazione_Cm = Check_from_SiNo(comm)

            Case AgriBio_subTipoProdAlim_LiquoriDistillati
                PA_Vegetale_Liquori_Cp = Check_from_SiNo(cproprio)
                PA_Vegetale_Liquori_Ct = Check_from_SiNo(cterzi)
                PA_Vegetale_Liquori_Cm = Check_from_SiNo(comm)

            Case AgriBio_subTipoProdAlim_Imbottigliamento
                PA_Vegetale_Imbottigliamento_Cp = Check_from_SiNo(cproprio)
                PA_Vegetale_Imbottigliamento_Ct = Check_from_SiNo(cterzi)
                PA_Vegetale_Imbottigliamento_Cm = Check_from_SiNo(comm)

            Case AgriBio_subTipoProdAlim_ProdErboristici
                PA_Vegetale_ProdottiErboristici_Cp = Check_from_SiNo(cproprio)
                PA_Vegetale_ProdottiErboristici_Ct = Check_from_SiNo(cterzi)
                PA_Vegetale_ProdottiErboristici_Cm = Check_from_SiNo(comm)

            Case AgriBio_subTipoProdAlim_Macellazione
                PA_Animale_Macellazione_Cp = Check_from_SiNo(cproprio)
                PA_Animale_Macellazione_Ct = Check_from_SiNo(cterzi)
                PA_Animale_Macellazione_Cm = Check_from_SiNo(comm)

            Case AgriBio_subTipoProdAlim_Sezionamento
                PA_Animale_Sezionamento_Cp = Check_from_SiNo(cproprio)
                PA_Animale_Sezionamento_Ct = Check_from_SiNo(cterzi)
                PA_Animale_Sezionamento_Cm = Check_from_SiNo(comm)

            Case AgriBio_subTipoProdAlim_DerivatiCarne
                PA_Animale_DerivatiCarne_Cp = Check_from_SiNo(cproprio)
                PA_Animale_DerivatiCarne_Ct = Check_from_SiNo(cterzi)
                PA_Animale_DerivatiCarne_Cm = Check_from_SiNo(comm)

            Case AgriBio_subTipoProdAlim_ConserveAnimali
                PA_Animale_ConserveAnimali_Cp = Check_from_SiNo(cproprio)
                PA_Animale_ConserveAnimali_Ct = Check_from_SiNo(cterzi)
                PA_Animale_ConserveAnimali_Cm = Check_from_SiNo(comm)

            Case AgriBio_subTipoProdAlim_ProdSalumeria
                PA_Animale_ProdottiSalumeria_Cp = Check_from_SiNo(cproprio)
                PA_Animale_ProdottiSalumeria_Ct = Check_from_SiNo(cterzi)
                PA_Animale_ProdottiSalumeria_Cm = Check_from_SiNo(comm)

            Case AgriBio_subTipoProdAlim_Latte
                PA_Animale_LatteAlimentare_Cp = Check_from_SiNo(cproprio)
                PA_Animale_LatteAlimentare_Ct = Check_from_SiNo(cterzi)
                PA_Animale_LatteAlimentare_Cm = Check_from_SiNo(comm)

            Case AgriBio_subTipoProdAlim_Caseificazioni
                PA_Animale_Caseificazione_Cp = Check_from_SiNo(cproprio)
                PA_Animale_Caseificazione_Ct = Check_from_SiNo(cterzi)
                PA_Animale_Caseificazione_Cm = Check_from_SiNo(comm)

            Case AgriBio_subTipoProdAlim_Burro
                PA_Animale_Burro_Cp = Check_from_SiNo(cproprio)
                PA_Animale_Burro_Ct = Check_from_SiNo(cterzi)
                PA_Animale_Burro_Cm = Check_from_SiNo(comm)

            Case AgriBio_subTipoProdAlim_Yogurt
                PA_Animale_Yogurt_Cp = Check_from_SiNo(cproprio)
                PA_Animale_Yogurt_Ct = Check_from_SiNo(cterzi)
                PA_Animale_Yogurt_Cm = Check_from_SiNo(comm)

            Case AgriBio_subTipoProdAlim_Altro
                PA_Vegetale_AltroDes_Cp = Check_from_SiNo(cproprio)
                PA_Vegetale_AltroDes_Ct = Check_from_SiNo(cterzi)
                PA_Vegetale_AltroDes_Cm = Check_from_SiNo(comm)
                PA_Vegetale_AltroDes = descrizione

            Case AgriBio_subTipoProdAlim_ProdottiForno
                PA_IndustriaDolciaria_DaForno_Cp = Check_from_SiNo(cproprio)
                PA_IndustriaDolciaria_DaForno_Ct = Check_from_SiNo(cterzi)
                PA_IndustriaDolciaria_DaForno_Cm = Check_from_SiNo(comm)

            Case AgriBio_subTipoProdAlim_AltriProdotto
                PA_IndustriaDolciaria_AltriProdotti_Cp = Check_from_SiNo(cproprio) '?
                PA_IndustriaDolciaria_AltriProdotti_Ct = Check_from_SiNo(cterzi) '?
                PA_IndustriaDolciaria_AltriProdotti_Cm = Check_from_SiNo(comm) '?

            Case AgriBio_subTipoProdAlim_Altro1, AgriBio_subTipoProdAlim_Altro2
                PA_Animale_Altro_Cp = Check_from_SiNo(cproprio)
                PA_Animale_Altro_Ct = Check_from_SiNo(cterzi)
                PA_Animale_Altro_Cm = Check_from_SiNo(comm)
                PA_Animale_AltroDes = descrizione

            Case AgriBio_subTipoProdAlim_Altro2
                'PA_Vegetale_Molinatura_Cp = Check_from_SiNo(cproprio)
                'PA_Vegetale_Molinatura_Ct = Check_from_SiNo(cterzi)
                'PA_Vegetale_Molinatura_Cm = Check_from_SiNo(comm)

            Case AgriBio_subTipoProdAlim_ProdSurgelati
                PA_Vegetale_Surgelati_Cp = Check_from_SiNo(cproprio)
                PA_Vegetale_Surgelati_Ct = Check_from_SiNo(cterzi)
                PA_Vegetale_Surgelati_Cm = Check_from_SiNo(comm)

            Case AgriBio_subTipoProdAlim_Ortofrutticoli
                PA_Vegetale_Ortofrutticoli_Cp = Check_from_SiNo(cproprio)
                PA_Vegetale_Ortofrutticoli_Ct = Check_from_SiNo(cterzi)
                PA_Vegetale_Ortofrutticoli_Cm = Check_from_SiNo(comm)

            Case AgriBio_subTipoProdAlim_Porzionatura
                PA_Animale_Porzionatura_Cp = Check_from_SiNo(cproprio)
                PA_Animale_Porzionatura_Ct = Check_from_SiNo(cterzi)
                PA_Animale_Porzionatura_Cm = Check_from_SiNo(comm)

            Case AgriBio_subTipoProdAlim_UovaDerivati
                PA_Animale_Uova_Cp = Check_from_SiNo(cproprio)
                PA_Animale_Uova_Ct = Check_from_SiNo(cterzi)
                PA_Animale_Uova_Cm = Check_from_SiNo(comm)

        End Select

    End Sub

    '#########################################################
    Public Sub AttivitaControllata_from_codAttivita(ByVal codAttivita As String, _
                                                   ByRef Organismo_Flag_Produttore As Integer, _
                                                  ByRef Organismo_Flag_Preparatore As Integer, _
                                                  ByRef Organismo_Flag_Importatore As Integer)

        'Public Const AgriBio_CategoriaProduzioni_Vegetali As String = "CP001"
        'Public Const AgriBio_CategoriaProduzioni_Alimentari As String = "CP003"
        'Public Const AgriBio_CategoriaProduzioni_Zootecniche As String = "CP002"

        Select Case codAttivita
            Case AgriBio_CategoriaProduzioni_Vegetali
                Organismo_Flag_Produttore = 1
            Case AgriBio_CategoriaProduzioni_Alimentari
                Organismo_Flag_Preparatore = 1
            Case AgriBio_CategoriaProduzioni_Zootecniche
                Organismo_Flag_Importatore = 1
        End Select


    End Sub

    '####################################################
    Public Sub UBAAttivitaAllevamento_from_Consistenza(ByVal codZootecnica As String, _
                                                           ByVal tipoProduzione As String, _
                                                           ByVal num_capi As Integer, _
                                                             ByRef AA10_UBABiologico As Decimal, _
                                                            ByRef AA10_UBAConvenzionale As Decimal, _
                                                            ByRef AA20_UBABiologico As Decimal, _
                                                            ByRef AA20_UBAConvenzionale As Decimal, _
                                                            ByRef AA30_UBABiologico As Decimal, _
                                                            ByRef AA30_UBAConvenzionale As Decimal, _
                                                            ByRef AA40_UBABiologico As Decimal, _
                                                            ByRef AA40_UBAConvenzionale As Decimal, _
                                                            ByRef AA50_UBABiologico As Decimal, _
                                                            ByRef AA50_UBAConvenzionale As Decimal, _
                                                            ByRef AA60_UBABiologico As Decimal, _
                                                            ByRef AA60_UBAConvenzionale As Decimal, _
                                                            ByRef AA60_IPRiproduzione As Integer, _
                                                            ByRef AA61_UBABiologico As Decimal, _
                                                            ByRef AA61_UBAConvenzionale As Decimal, _
                                                            ByRef AA70_UBABiologico As Decimal, _
                                                            ByRef AA70_UBAConvenzionale As Decimal, _
                                                            ByRef AA80_UBABiologico As Integer, _
                                                            ByRef AA80_UBAConvenzionale As Integer, _
                                                            ByRef AA90_UBABiologico As Decimal, _
                                                            ByRef AA90_UBAConvenzionale As Decimal, _
                                                            ByRef AA90_AltroDes As String, _
                                                            ByRef B_AA_TotaleUBA As Decimal, _
                                                            ByRef B_AA_TotaleFamiglie As Decimal)


        Select Case Left(codZootecnica, 3).ToUpper

            Case "BOV"
                B_AA_TotaleUBA += num_capi
                Select Case tipoProduzione
                    Case AgriBio_MetodoProduzione_Biologico
                        AA10_UBABiologico += num_capi
                    Case AgriBio_MetodoProduzione_Convenzionale
                        AA10_UBAConvenzionale += num_capi
                End Select

            Case "BUF"
                B_AA_TotaleUBA += num_capi
                Select Case tipoProduzione
                    Case AgriBio_MetodoProduzione_Biologico
                        AA20_UBABiologico += num_capi
                    Case AgriBio_MetodoProduzione_Convenzionale
                        AA20_UBAConvenzionale += num_capi
                End Select

            Case "OVI"
                B_AA_TotaleUBA += num_capi
                Select Case tipoProduzione
                    Case AgriBio_MetodoProduzione_Biologico
                        AA30_UBABiologico += num_capi
                    Case AgriBio_MetodoProduzione_Convenzionale
                        AA30_UBAConvenzionale += num_capi
                End Select

            Case "CAP"
                B_AA_TotaleUBA += num_capi
                Select Case tipoProduzione
                    Case AgriBio_MetodoProduzione_Biologico
                        AA40_UBABiologico += num_capi
                    Case AgriBio_MetodoProduzione_Convenzionale
                        AA40_UBAConvenzionale += num_capi
                End Select

            Case "EQU"
                B_AA_TotaleUBA += num_capi
                Select Case tipoProduzione
                    Case AgriBio_MetodoProduzione_Biologico
                        AA50_UBABiologico += num_capi
                    Case AgriBio_MetodoProduzione_Convenzionale
                        AA50_UBAConvenzionale += num_capi
                End Select

            Case "SUI"
                B_AA_TotaleUBA += num_capi

                Select Case codZootecnica

                    Case AgriBio_Consistenze_SuiniRiproduzione_ScrofeGestazione, AgriBio_Consistenze_SuiniRiproduzione_ScrofeParto, _
                       AgriBio_Consistenze_SuiniRiproduzione_Verri, AgriBio_Consistenze_SuiniRiproduzione_Lattonzoli, _
                        AgriBio_Consistenze_SuiniRiproduzione_Scrofette

                        Select Case tipoProduzione
                            Case AgriBio_MetodoProduzione_Biologico
                                AA60_UBABiologico += num_capi
                            Case AgriBio_MetodoProduzione_Convenzionale
                                AA60_UBAConvenzionale += num_capi
                        End Select

                    Case Else
                        Select Case tipoProduzione
                            Case AgriBio_MetodoProduzione_Biologico
                                AA61_UBABiologico += num_capi
                            Case AgriBio_MetodoProduzione_Convenzionale
                                AA61_UBAConvenzionale += num_capi
                        End Select

                End Select

            Case "AVI"
                B_AA_TotaleUBA += num_capi
                Select Case tipoProduzione
                    Case AgriBio_MetodoProduzione_Biologico
                        AA70_UBABiologico += num_capi
                    Case AgriBio_MetodoProduzione_Convenzionale
                        AA70_UBAConvenzionale += num_capi
                End Select

            Case "API"
                B_AA_TotaleFamiglie += num_capi
                Select Case tipoProduzione
                    Case AgriBio_MetodoProduzione_Biologico
                        AA80_UBABiologico += num_capi
                    Case AgriBio_MetodoProduzione_Convenzionale
                        AA80_UBAConvenzionale += num_capi
                End Select

            Case "ALT"
                B_AA_TotaleUBA += num_capi
                Select Case tipoProduzione
                    Case AgriBio_MetodoProduzione_Biologico
                        AA90_UBABiologico += num_capi
                    Case AgriBio_MetodoProduzione_Convenzionale
                        AA90_UBAConvenzionale += num_capi
                End Select
                AA90_AltroDes = DescrizioneAltro_from_codZootecnica(codZootecnica)

        End Select

    End Sub

    '#########################################################
    Public Function DescrizioneAltro_from_codZootecnica(ByVal codZootecnica As String) As String

        Dim desc_altro As String = ""

        Select Case codZootecnica

            Case AgriBio_Consistenze_AltriAllevamenti
                desc_altro = "Altri Allevamenti"

            Case AgriBio_Consistenze_CoturniciDaRiproduzione
                desc_altro = "Coturnici Da Riproduzione"

            Case AgriBio_Consistenze_Coturnici
                desc_altro = "Coturnici"

            Case AgriBio_Consistenze_AltriVolatili
                desc_altro = "Altri Volatili"

            Case AgriBio_Consistenze_LepriVisoniNutrieCincilla
                desc_altro = "Lepri Visoni Nutrie Cincilla"

            Case AgriBio_Consistenze_Volpi
                desc_altro = "Volpi"

            Case AgriBio_Consistenze_PesciCrostaceiMolluschiDaRiproduzione
                desc_altro = "Pesci Crostacei Molluschi Da Riproduzione"

            Case AgriBio_Consistenze_PesciCrostaceiMolluschiDaConsumo
                desc_altro = "Pesci Crostacei Molluschi Da Consumo"

            Case AgriBio_Consistenze_CinghialiCervi
                desc_altro = "Cinghiali Cervi"

            Case AgriBio_Consistenze_DainiCaprioliMufloni
                desc_altro = "Daini Caprioli Mufloni"

        End Select

        Return desc_altro

    End Function

    '####################################################
    Public Sub IPAttivitaAllevamento_from_Consistenza(ByVal codZootecnica As String, _
                                                      ByVal DT_AgriBio_Consistenze As DataTable, _
                                                            ByRef AA10_IPCarne As Integer, _
                                                            ByRef AA10_IPLatte As Integer, _
                                                            ByRef AA10_IPRiproduzione As Integer, _
                                                            ByRef AA10_IPAltro As Integer, _
                                                            ByRef AA10_IPAltroDes As String, _
                                                            ByRef AA20_IPCarne As Integer, _
                                                            ByRef AA20_IPLatte As Integer, _
                                                            ByRef AA20_IPRiproduzione As Integer, _
                                                            ByRef AA20_IPAltro As Integer, _
                                                            ByRef AA20_IPAltroDes As String, _
                                                            ByRef AA30_IPCarne As Integer, _
                                                            ByRef AA30_IPLatte As Integer, _
                                                            ByRef AA30_IPRiproduzione As Integer, _
                                                            ByRef AA30_IPAltro As Integer, _
                                                            ByRef AA30_IPAltroDes As String, _
                                                            ByRef AA40_IPCarne As Integer, _
                                                            ByRef AA40_IPLatte As Integer, _
                                                            ByRef AA40_IPRiproduzione As Integer, _
                                                            ByRef AA40_IPAltro As Integer, _
                                                            ByRef AA40_IPAltroDes As String, _
                                                            ByRef AA50_IPCarne As Integer, _
                                                            ByRef AA50_IPRiproduzione As Integer, _
                                                            ByRef AA50_IPAltro As Integer, _
                                                            ByRef AA50_IPAltroDes As String, _
                                                            ByRef AA60_IPRiproduzione As Integer, _
                                                            ByRef AA60_IPAltro As Integer, _
                                                            ByRef AA60_IPAltroDes As String, _
                                                            ByRef AA61_IPCarne As Integer, _
                                                            ByRef AA70_IPCarne As Integer, _
                                                            ByRef AA70_IPUova As Integer, _
                                                            ByRef AA70_IPRiproduzione As Integer, _
                                                            ByRef AA70_IPAltro As Integer, _
                                                            ByRef AA70_IPAltroDes As String, _
                                                            ByRef AA80_IPMiele As Integer, _
                                                            ByRef AA80_IPPReale As Integer, _
                                                            ByRef AA80_IPCera As Integer, _
                                                            ByRef AA80_IPAltro As Integer, _
                                                            ByRef AA80_IPAltroDes As String, _
                                                            ByRef AA90_IPAltroDes As String)

        Dim Dr() As DataRow
        Dr = DT_AgriBio_Consistenze.Select(" Cod_AgriBio = '" + codZootecnica + "'")

        Select Case Left(codZootecnica, 3).ToUpper

            Case "BOV"
                AA10_IPCarne = Dr(0).Item("Chk_Carne")
                AA10_IPLatte = Dr(0).Item("Chk_Latte")
                AA10_IPRiproduzione = Dr(0).Item("Chk_Riproduzione")
                AA10_IPAltro = Dr(0).Item("Chk_Altro")
                AA10_IPAltroDes = ""

            Case "BUF"
                AA20_IPCarne = Dr(0).Item("Chk_Carne")
                AA20_IPLatte = Dr(0).Item("Chk_Latte")
                AA20_IPRiproduzione = Dr(0).Item("Chk_Riproduzione")
                AA20_IPAltro = Dr(0).Item("Chk_Altro")
                AA20_IPAltroDes = ""

            Case "OVI"
                AA30_IPCarne = Dr(0).Item("Chk_Carne")
                AA30_IPLatte = Dr(0).Item("Chk_Latte")
                AA30_IPRiproduzione = Dr(0).Item("Chk_Riproduzione")
                AA30_IPAltro = Dr(0).Item("Chk_Altro")
                AA30_IPAltroDes = ""

            Case "CAP"
                AA40_IPCarne = Dr(0).Item("Chk_Carne")
                AA40_IPLatte = Dr(0).Item("Chk_Latte")
                AA40_IPRiproduzione = Dr(0).Item("Chk_Riproduzione")
                AA40_IPAltro = Dr(0).Item("Chk_Altro")
                AA40_IPAltroDes = ""

            Case "EQU"
                AA50_IPCarne = Dr(0).Item("Chk_Carne")
                AA50_IPRiproduzione = Dr(0).Item("Chk_Riproduzione")
                AA50_IPAltro = Dr(0).Item("Chk_Altro")
                AA50_IPAltroDes = ""

            Case "SUI"

                Select Case codZootecnica

                    Case AgriBio_Consistenze_SuiniRiproduzione_ScrofeGestazione, AgriBio_Consistenze_SuiniRiproduzione_ScrofeParto, _
                       AgriBio_Consistenze_SuiniRiproduzione_Verri, AgriBio_Consistenze_SuiniRiproduzione_Lattonzoli, _
                        AgriBio_Consistenze_SuiniRiproduzione_Scrofette

                        AA60_IPRiproduzione = Dr(0).Item("Chk_Riproduzione")
                        AA60_IPAltro = Dr(0).Item("Chk_Altro")
                        AA60_IPAltroDes = ""

                    Case Else
                        AA61_IPCarne = Dr(0).Item("Chk_Carne")

                End Select

            Case "AVI"
                AA70_IPCarne = Dr(0).Item("Chk_Carne")
                AA70_IPUova = Dr(0).Item("Chk_Uova")
                AA70_IPRiproduzione = Dr(0).Item("Chk_Riproduzione")
                AA70_IPAltro = Dr(0).Item("Chk_Altro")
                AA70_IPAltroDes = ""

            Case "API"
                AA80_IPMiele = Dr(0).Item("Chk_Miele")
                AA80_IPPReale = Dr(0).Item("Chk_PappaReale")
                AA80_IPCera = Dr(0).Item("Chk_Cera")
                AA80_IPAltro = Dr(0).Item("Chk_Altro")
                AA80_IPAltroDes = ""

            Case "ALT"
                AA90_IPAltroDes = ""


        End Select

    End Sub

    'Public Const AgriBio_Consistenze_SuiniRiproduzione_ScrofeGestazione As String = "SUI001"
    'Public Const AgriBio_Consistenze_SuiniRiproduzione_ScrofeParto As String = "SUI002"
    'Public Const AgriBio_Consistenze_SuiniRiproduzione_Verri As String = "SUI003"
    'Public Const AgriBio_Consistenze_SuiniRiproduzione_Lattonzoli As String = "SUI004"
    'Public Const AgriBio_Consistenze_SuiniRiproduzione_Scrofette As String = "SUI005"

End Class
