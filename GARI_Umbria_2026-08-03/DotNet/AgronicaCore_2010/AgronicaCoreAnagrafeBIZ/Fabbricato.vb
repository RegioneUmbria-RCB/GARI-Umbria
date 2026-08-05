Imports System.Linq
Imports System.Transactions
Imports System.Xml
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.exceptions
Imports Newtonsoft.Json

Public Class Fabbricato_R
    Inherits AgronicaCoreDataProvider.LogProvider

    Public Function Fabbricato_Leggi(ByVal Piva As String,
                                     ByVal Sa_Cod As Integer,
                                     ByVal Fabbricato_Cod As Integer,
                                     ByVal ForDelete As Boolean,
                                     ByRef objParametri As AgronicaCoreParametri,
                                     Optional ByVal TipoG2G As Integer = 0
                                     ) As String

        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.Fabbricato_R.Fabbricato_Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod = 0
        '   Fabbricato_Cod = 0
        '====================================================================================

        Dim messaggioErrore As String = ""

        Dim iFabbricati As Integer
        Dim iIndirizzi As Integer
        Dim iStalle As Integer
        Dim iStallaCaratteristiche As Integer
        Dim iFabbricatiImpostazioni As Integer
        Dim iFabbricatixCodici As Integer

        Dim RisultatoFunzione As String = String.Empty

        Dim XmlDoc As XmlDocument

        '---------------------------

        Dim XmlDatiFabbricati As XmlElement
        Dim XmlFabbricato As XmlElement
        Dim XmlIndirizzo As XmlElement
        Dim XmlStalla As XmlElement
        Dim XmlDatiFabbricati_Impostazioni As XmlElement
        Dim XmlFabbricato_Impostazione As XmlElement
        Dim XmlCodice As XmlElement
        Dim XmlCaratteristica As XmlElement

        Dim ObjFabbricati As AgronicaCoreAnagrafeDAL.Fabbricati_R
        Dim objFabbricatixIndirizzi As AgronicaCoreAnagrafeDAL.Indirizzi_Read
        Dim ObjStalla As AgronicaCoreAnagrafeDAL.Stalla_R
        'Dim ObjVasca As object
        Dim objFabbricatixCodici As AgronicaCoreAnagrafeDAL.Fabbricati_Codici_R
        Dim ObjStallaxCaratteristiche As AgronicaCoreAnagrafeDAL.Stalla_Caratteristiche_R
        Dim ObjFabbricati_Impostazioni As AgronicaCoreAnagrafeDAL.Fabbricati_Impostaz_R

        Dim DtFabbricati As DataTable
        Dim DtIndirizzi As DataTable
        Dim DtStalla As DataTable
        Dim DtFabbricatixCodici As DataTable
        Dim DtStallaxCaratteristiche As DataTable
        Dim DtFabbricati_Impostazioni As DataTable

        Dim bFabbricato As Boolean               'Booleano per esistenza fabbricato in xml

        '------------------------------
        Dim FlagConnessioneLocale As Boolean = False


        Try

            '------------------------------
            'Verifico se è stata impostata una connessione
            If IsNothing(objParametri.objConnessione) Then
                FlagConnessioneLocale = True
                objParametri.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
            End If


            '------------------------------


            '//////////////////////////////
            '//////////////////////////////
            '//////////////////////////////


            '----- < Documento XML > -----
            'XmlDoc = CreateObject("Msxml2.DOMDocument.4.0")
            XmlDoc = New XmlDocument

            XmlDatiFabbricati = XmlDoc.CreateElement("DatiFabbricati")

            bFabbricato = False

            'Mi procuro un elenco dei Fabbricati associati al Centro Aziendale
            'all'interno della finestra temporale selezionata

            ObjFabbricati = New AgronicaCoreAnagrafeDAL.Fabbricati_R

            'Mi procuro il recordset richiesto
            DtFabbricati = ObjFabbricati.Leggi(CStr(Piva),
                                               CInt(Sa_Cod),
                                               CInt(Fabbricato_Cod),
                                               enumSelezioneVariabile.Selezione_TabellaCompleta,
                                               "",
                                               "",
                                               objParametri,
                                               TipoG2G)

            'Se ottengo almeno un risultato, creo la struttura XML
            If DtFabbricati.Rows.Count > 0 Then

                bFabbricato = True

                'Effettuo un ciclo sui Fabbricati

                For iFabbricati = 0 To DtFabbricati.Rows.Count - 1

                    '----- < FABBRICATO > -----
                    XmlFabbricato = XmlDoc.CreateElement("Fabbricato")

                    With XmlFabbricato
                        .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                        .SetAttribute("piva", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("PIVA")))
                        .SetAttribute("sa_cod", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("Sa_Cod")))
                        .SetAttribute("fabbricato_cod", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("fabbricato_cod")))
                        .SetAttribute("fabbricato_des", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("fabbricato_des")))
                        .SetAttribute("indirizzo_cod", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("indirizzo_cod")))
                        .SetAttribute("tipo_fabbricato_cod", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("tipo_fabbricato_cod")))
                        .SetAttribute("prov", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("prov")))
                        .SetAttribute("com", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("com")))
                        .SetAttribute("sezione", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("sezione")))
                        .SetAttribute("foglio", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("foglio")))
                        .SetAttribute("numero", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("numero")))
                        .SetAttribute("subalterno", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("subalterno")))
                        .SetAttribute("mc_convenzionale", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("mc_convenzionale")))
                        .SetAttribute("mc_conversione", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("mc_conversione")))
                        .SetAttribute("mc_biologico", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("mc_biologico")))
                        .SetAttribute("regolamento_cod", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("regolamento_cod")))
                        .SetAttribute("titolopossesso", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("titolopossesso")))
                        .SetAttribute("conversione_inizio", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("conversione_inizio")))
                        .SetAttribute("conversione_fine", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("conversione_fine")))
                        .SetAttribute("idoneo_costruzione", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("idoneo_costruzione")))
                        .SetAttribute("idoneo_separazambienti", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("idoneo_separazambienti")))
                        .SetAttribute("idoneo_separazprodotti", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("idoneo_separazprodotti")))
                        .SetAttribute("idoneo_condigieniche", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("idoneo_condigieniche")))
                        .SetAttribute("idoneo_autorizsanitaria", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("idoneo_autorizsanitaria")))
                        .SetAttribute("idoneo_haccp", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("idoneo_haccp")))
                        .SetAttribute("idoneo_planimetria", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("idoneo_planimetria")))
                        .SetAttribute("idoneo_layout", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("idoneo_layout")))
                        .SetAttribute("idoneo_diagrammiflusso", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("idoneo_diagrammiflusso")))
                        .SetAttribute("idoneo_cdx_m004", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("idoneo_cdx_m004")))
                        .SetAttribute("idoneo_supmincoperte", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("idoneo_supmincoperte")))
                        .SetAttribute("idoneo_supminscoperte", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("idoneo_supminscoperte")))

                        .SetAttribute("mq_convenzionale", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("mq_convenzionale")))
                        .SetAttribute("mq_conversione", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("mq_conversione")))
                        .SetAttribute("mq_biologico", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("mq_biologico")))
                        .SetAttribute("mq_convenzionale_scoperto", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("mq_convenzionale_scoperto")))
                        .SetAttribute("mq_conversione_scoperto", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("mq_conversione_scoperto")))
                        .SetAttribute("mq_biologico_scoperto", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("mq_biologico_scoperto")))
                        .SetAttribute("n_piani", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("n_piani")))
                        .SetAttribute("sup_piano", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("sup_piano")))

                        .SetAttribute("num_autorizzazione", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("Num_Autorizzazione")))
                        .SetAttribute("data_richiesta_autorizzazione", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("Data_Richiesta_Autorizzazione")))
                        .SetAttribute("tipologia_utilizzo", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("Tipologia_Utilizzo")))
                        .SetAttribute("chkvirtuale", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("ChkVirtuale")))

                        .SetAttribute("proprietario_capi", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("ProprietarioCapi")))
                        .SetAttribute("chkmagazzinofarmaci", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("ChkMagazzinoFarmaci")))
                        .SetAttribute("codice_bdn", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("CodiceBDN")))

                        .SetAttribute("data_creazione", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("data_creazione")))
                        .SetAttribute("data_modifica", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("data_modifica")))
                        .SetAttribute("username_creazione", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("username_creazione")))
                        .SetAttribute("username_modifica", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("username_modifica")))
                        .SetAttribute("validita_inizio", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("Validita_Inizio")))
                        .SetAttribute("validita_fine", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("Validita_Fine")))

                    End With


                    '#################################
                    '##########  INDIRIZZI  ##########
                    '#################################

                    objFabbricatixIndirizzi = New AgronicaCoreAnagrafeDAL.Indirizzi_Read

                    'Mi procuro il recordset richiesto
                    DtIndirizzi = objFabbricatixIndirizzi.Leggi(DtFabbricati.Rows(iFabbricati).Item("indirizzo_cod"),
                                                                enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                "",
                                                                "",
                                                                objParametri)

                    'Se ottengo almeno un risultato, creo la struttura XML
                    If DtIndirizzi.Rows.Count > 0 Then

                        'Effettuo un ciclo sugli indirizzi

                        For iIndirizzi = 0 To DtIndirizzi.Rows.Count - 1

                            '----- < INDIRIZZO > -----
                            XmlIndirizzo = XmlDoc.CreateElement("Indirizzo")

                            With XmlIndirizzo

                                .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                .SetAttribute("tipo_indirizzo", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("tipo_fabbricato_cod")))
                                .SetAttribute("cod_indirizzo", Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("indirizzo_cod")))
                                .SetAttribute("ind_des", Agro_SQL_Load(DtIndirizzi.Rows(iIndirizzi).Item("Ind_Des")))
                                .SetAttribute("frz_des", Agro_SQL_Load(DtIndirizzi.Rows(iIndirizzi).Item("Frz_Des")))
                                .SetAttribute("cap", Agro_SQL_Load(DtIndirizzi.Rows(iIndirizzi).Item("Cap")))
                                .SetAttribute("com_des", Agro_SQL_Load(DtIndirizzi.Rows(iIndirizzi).Item("Com_Des")))
                                .SetAttribute("pro_cod", Agro_SQL_Load(DtIndirizzi.Rows(iIndirizzi).Item("Pro_Cod")))
                                .SetAttribute("pro_des", Agro_SQL_Load(DtIndirizzi.Rows(iIndirizzi).Item("Pro_Des")))
                                .SetAttribute("stato", Agro_SQL_Load(DtIndirizzi.Rows(iIndirizzi).Item("Stato")))
                                .SetAttribute("note", Agro_SQL_Load(DtIndirizzi.Rows(iIndirizzi).Item("Note")))
                                .SetAttribute("pro_cod_istat", Agro_SQL_Load(DtIndirizzi.Rows(iIndirizzi).Item("Pro_Cod_Istat")))
                                .SetAttribute("com_cod_istat", Agro_SQL_Load(DtIndirizzi.Rows(iIndirizzi).Item("Com_Cod_Istat")))
                                .SetAttribute("validita_inizio", Agro_SQL_Load(DtIndirizzi.Rows(iIndirizzi).Item("Validita_Inizio")))
                                .SetAttribute("validita_fine", Agro_SQL_Load(DtIndirizzi.Rows(iIndirizzi).Item("Validita_Fine")))

                            End With

                            XmlFabbricato.AppendChild(XmlIndirizzo)
                            '----- < / INDIRIZZO > -----

                        Next

                    End If

                    XmlIndirizzo = Nothing
                    DtIndirizzi.Dispose()
                    DtIndirizzi = Nothing
                    objFabbricatixIndirizzi = Nothing




                    '#################################
                    '##########  STALLA  #############
                    '#################################

                    'Mi procuro un elenco dei data dell'eventuale stalla

                    ObjStalla = New AgronicaCoreAnagrafeDAL.Stalla_R

                    'Mi procuro il recordset richiesto
                    DtStalla = ObjStalla.Leggi(CStr(Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("PIVA"))),
                                               CInt(Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("Sa_Cod"))),
                                               CInt(Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("Fabbricato_Cod"))),
                                               enumSelezioneVariabile.Selezione_TabellaCompleta,
                                               "", "",
                                               objParametri)

                    'Se ottengo almeno un risultato, creo la struttura XML
                    If DtStalla.Rows.Count > 0 Then

                        'Effettuo un ciclo sulle stalle

                        For iStalle = 0 To DtStalla.Rows.Count - 1

                            '----- < STALLA > -----
                            XmlStalla = XmlDoc.CreateElement("Stalla")

                            With XmlStalla
                                .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                .SetAttribute("piva", Agro_SQL_Load(DtStalla.Rows(iStalle).Item("piva")))
                                .SetAttribute("sa_cod", Agro_SQL_Load(DtStalla.Rows(iStalle).Item("sa_cod")))
                                .SetAttribute("sta_num", Agro_SQL_Load(DtStalla.Rows(iStalle).Item("sta_num")))
                                .SetAttribute("sta_des", Agro_SQL_Load(DtStalla.Rows(iStalle).Item("sta_des")))
                                .SetAttribute("ausl_cod", Agro_SQL_Load(DtStalla.Rows(iStalle).Item("ausl_cod")))
                                .SetAttribute("dat_costr", Agro_SQL_Load(DtStalla.Rows(iStalle).Item("dat_costr")))
                                .SetAttribute("dat_chiu", Agro_SQL_Load(DtStalla.Rows(iStalle).Item("dat_chiu")))
                                .SetAttribute("cod_fabb", Agro_SQL_Load(DtStalla.Rows(iStalle).Item("cod_fabb")))
                                .SetAttribute("gen_cod", Agro_SQL_Load(DtStalla.Rows(iStalle).Item("gen_cod")))
                                .SetAttribute("spe_cod", Agro_SQL_Load(DtStalla.Rows(iStalle).Item("spe_cod")))
                                .SetAttribute("ipro_cod", Agro_SQL_Load(DtStalla.Rows(iStalle).Item("ipro_cod")))
                                .SetAttribute("x", Agro_SQL_Load(DtStalla.Rows(iStalle).Item("x")))
                                .SetAttribute("y", Agro_SQL_Load(DtStalla.Rows(iStalle).Item("y")))
                                .SetAttribute("validita_inizio", Agro_SQL_Load(DtStalla.Rows(iStalle).Item("validita_inizio")))
                                .SetAttribute("validita_fine", Agro_SQL_Load(DtStalla.Rows(iStalle).Item("validita_fine")))
                                .SetAttribute("latitudine", Agro_SQL_Load(DtStalla.Rows(iStalle).Item("latitudine")))
                                .SetAttribute("longitudine", Agro_SQL_Load(DtStalla.Rows(iStalle).Item("longitudine")))
                                .SetAttribute("BDN_codice_azienda", Agro_SQL_Load(DtStalla.Rows(iStalle).Item("BDN_Codice_Azienda")))
                                .SetAttribute("BDN_allev_idfiscale", Agro_SQL_Load(DtStalla.Rows(iStalle).Item("BDN_Allev_IdFiscale")))
                                .SetAttribute("dat_ult_agg", Agro_SQL_Load(DtStalla.Rows(iStalle).Item("dat_ult_agg")))
                                .SetAttribute("cuaa_proprietario", Agro_SQL_Load(DtStalla.Rows(iStalle).Item("cuaa_proprietario")))
                                .SetAttribute("denominazione_proprietario", Agro_SQL_Load(DtStalla.Rows(iStalle).Item("denominazione_proprietario")))
                                .SetAttribute("cuaa_detentore", Agro_SQL_Load(DtStalla.Rows(iStalle).Item("cuaa_detentore")))
                                .SetAttribute("denominazione_detentore", Agro_SQL_Load(DtStalla.Rows(iStalle).Item("denominazione_detentore")))
                            End With


                            '#################################################
                            '##########  CARATTERISTICHE STALLA  #############
                            '#################################################

                            'Mi procuro un elenco delle caratteristiche della stalla

                            ObjStallaxCaratteristiche = New AgronicaCoreAnagrafeDAL.Stalla_Caratteristiche_R

                            'Mi procuro il recordset richiesto
                            DtStallaxCaratteristiche = ObjStallaxCaratteristiche.Leggi(
                                                    CStr(Agro_SQL_Load(DtStalla.Rows(iStalle).Item("PIVA"))),
                                                    CInt(Agro_SQL_Load(DtStalla.Rows(iStalle).Item("Sa_Cod"))),
                                                    CInt(Agro_SQL_Load(DtStalla.Rows(iStalle).Item("sta_num"))),
                                                    CStr(Agro_SQL_Load(DtStalla.Rows(iStalle).Item("cod_fabb"))),
                                                    0,
                                                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                    "",
                                                    "",
                                                    objParametri)

                            'Se ottengo almeno un risultato, creo la struttura XML
                            If DtStallaxCaratteristiche.Rows.Count > 0 Then

                                'Effettuo un ciclo sui codici

                                For iStallaCaratteristiche = 0 To DtStallaxCaratteristiche.Rows.Count - 1

                                    '----- < Stalla_Caratteristica > -----
                                    XmlCaratteristica = XmlDoc.CreateElement("Stalla_Caratteristica")

                                    With XmlCaratteristica
                                        .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                        .SetAttribute("att_cod", Agro_SQL_Load(DtStallaxCaratteristiche.Rows(iStallaCaratteristiche).Item("att_cod")))
                                        .SetAttribute("valore", Agro_SQL_Load(DtStallaxCaratteristiche.Rows(iStallaCaratteristiche).Item("valore")))
                                        .SetAttribute("validita_inizio", Agro_SQL_Load(DtStallaxCaratteristiche.Rows(iStallaCaratteristiche).Item("validita_inizio")))
                                        .SetAttribute("validita_fine", Agro_SQL_Load(DtStallaxCaratteristiche.Rows(iStallaCaratteristiche).Item("validita_fine")))
                                    End With

                                    XmlStalla.AppendChild(XmlCaratteristica)
                                    '----- < / Stalla_Caratteristica > -----

                                Next

                            End If

                            XmlCaratteristica = Nothing
                            DtStallaxCaratteristiche.Dispose()
                            DtStallaxCaratteristiche = Nothing
                            ObjStallaxCaratteristiche = Nothing

                            '-----------------------------------------------------------

                            XmlFabbricato.AppendChild(XmlStalla)
                            '----- < / STALLA > -----

                        Next

                    End If

                    XmlStalla = Nothing
                    DtStalla.Dispose()
                    DtStalla = Nothing
                    ObjStalla = Nothing


                    '##################################################
                    '##########  FABBRICATO IMPOSTAZIONI  #############
                    '##################################################

                    ObjFabbricati_Impostazioni = New AgronicaCoreAnagrafeDAL.Fabbricati_Impostaz_R

                    'Mi procuro il recordset richiesto
                    DtFabbricati_Impostazioni = ObjFabbricati_Impostazioni.Leggi(
                                                    "",
                                                    CStr(Piva),
                                                    CInt(Sa_Cod),
                                                    CInt(Fabbricato_Cod),
                                                    0,
                                                    0,
                                                    0,
                                                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                    "",
                                                    "",
                                                    objParametri)

                    'Se ottengo almeno un risultato, creo la struttura XML
                    If DtFabbricati_Impostazioni.Rows.Count > 0 Then

                        XmlDatiFabbricati_Impostazioni = XmlDoc.CreateElement("DatiFabbricato_Impostazioni")

                        'Effettuo un ciclo

                        For iFabbricatiImpostazioni = 0 To DtFabbricati_Impostazioni.Rows.Count - 1

                            '----- < Fabbricato Impostazione > -----
                            XmlFabbricato_Impostazione = XmlDoc.CreateElement("Fabbricato_Impostazione")

                            With XmlFabbricato_Impostazione
                                .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                .SetAttribute("piva_superuser", Agro_SQL_Load(DtFabbricati_Impostazioni.Rows(iFabbricatiImpostazioni).Item("piva_superuser")))
                                .SetAttribute("piva", Agro_SQL_Load(DtFabbricati_Impostazioni.Rows(iFabbricatiImpostazioni).Item("piva")))
                                .SetAttribute("sa_cod", Agro_SQL_Load(DtFabbricati_Impostazioni.Rows(iFabbricatiImpostazioni).Item("sa_cod")))
                                .SetAttribute("fabbricato_cod", Agro_SQL_Load(DtFabbricati_Impostazioni.Rows(iFabbricatiImpostazioni).Item("fabbricato_cod")))
                                .SetAttribute("elem_cod", Agro_SQL_Load(DtFabbricati_Impostazioni.Rows(iFabbricatiImpostazioni).Item("elem_cod")))
                                .SetAttribute("gru_cod", Agro_SQL_Load(DtFabbricati_Impostazioni.Rows(iFabbricatiImpostazioni).Item("gru_cod")))
                                .SetAttribute("lav_cod", Agro_SQL_Load(DtFabbricati_Impostazioni.Rows(iFabbricatiImpostazioni).Item("lav_cod")))
                                .SetAttribute("validita_inizio", Agro_SQL_Load(DtFabbricati_Impostazioni.Rows(iFabbricatiImpostazioni).Item("Validita_Inizio")))
                                .SetAttribute("validita_fine", Agro_SQL_Load(DtFabbricati_Impostazioni.Rows(iFabbricatiImpostazioni).Item("Validita_Fine")))
                                .SetAttribute("data_creazione", Agro_SQL_Load(DtFabbricati_Impostazioni.Rows(iFabbricatiImpostazioni).Item("Data_Creazione")))
                                .SetAttribute("data_modifica", Agro_SQL_Load(DtFabbricati_Impostazioni.Rows(iFabbricatiImpostazioni).Item("Data_Modifica")))
                                .SetAttribute("username_creazione", Agro_SQL_Load(DtFabbricati_Impostazioni.Rows(iFabbricatiImpostazioni).Item("Username_Creazione")))
                                .SetAttribute("username_modifica", Agro_SQL_Load(DtFabbricati_Impostazioni.Rows(iFabbricatiImpostazioni).Item("Username_Modifica")))
                            End With

                            XmlDatiFabbricati_Impostazioni.AppendChild(XmlFabbricato_Impostazione)

                            '----- < / Fabbricato_Impostazione > -----

                            XmlFabbricato_Impostazione = Nothing

                        Next

                        XmlFabbricato.AppendChild(XmlDatiFabbricati_Impostazioni)

                    End If

                    'Impicco gli Oggetti
                    ObjFabbricati_Impostazioni = Nothing
                    DtFabbricati_Impostazioni.Dispose()
                    DtFabbricati_Impostazioni = Nothing
                    XmlDatiFabbricati_Impostazioni = Nothing


                    '#################################
                    '##########  CODICI  #############
                    '#################################

                    'Mi procuro un elenco dei codici del fabbricato

                    objFabbricatixCodici = New AgronicaCoreAnagrafeDAL.Fabbricati_Codici_R

                    'Mi procuro il recordset richiesto
                    DtFabbricatixCodici = objFabbricatixCodici.Leggi(
                                                CStr(Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("PIVA"))),
                                                CInt(Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("Sa_Cod"))),
                                                CInt(Agro_SQL_Load(DtFabbricati.Rows(iFabbricati).Item("Fabbricato_Cod"))),
                                                0,
                                                "",
                                                enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                "",
                                                "",
                                                objParametri)



                    'Se ottengo almeno un risultato, creo la struttura XML
                    If DtFabbricatixCodici.Rows.Count > 0 Then

                        'Effettuo un ciclo sui codici
                        For iFabbricatixCodici = 0 To DtFabbricatixCodici.Rows.Count - 1

                            '----- < CODICE > -----
                            XmlCodice = XmlDoc.CreateElement("CodiceFabbricato")

                            With XmlCodice
                                .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                .SetAttribute("id_cod", Agro_SQL_Load(DtFabbricatixCodici.Rows(iFabbricatixCodici).Item("id_cod")))
                                .SetAttribute("val_cod", Agro_SQL_Load(DtFabbricatixCodici.Rows(iFabbricatixCodici).Item("val_cod")))
                                .SetAttribute("descrizione", Agro_SQL_Load(DtFabbricatixCodici.Rows(iFabbricatixCodici).Item("descrizione")))
                                .SetAttribute("validita_inizio", Agro_SQL_Load(DtFabbricatixCodici.Rows(iFabbricatixCodici).Item("validita_inizio")))
                                .SetAttribute("validita_fine", Agro_SQL_Load(DtFabbricatixCodici.Rows(iFabbricatixCodici).Item("validita_fine")))
                            End With

                            XmlFabbricato.AppendChild(XmlCodice)
                            '----- < / CODICE > -----

                        Next

                    End If

                    XmlCodice = Nothing
                    DtFabbricatixCodici.Dispose()
                    DtFabbricatixCodici = Nothing
                    objFabbricatixCodici = Nothing

                    '-----------------------------------------------------------

                    XmlDatiFabbricati.AppendChild(XmlFabbricato)

                    XmlFabbricato = Nothing

                Next

                '----- < / FABBRICATI > -----

            End If

            'Elimino gli oggetti che ho creato
            DtFabbricati.Dispose()
            DtFabbricati = Nothing
            ObjFabbricati = Nothing

            '-----------------------------------------------------------

            ''''''     '#################################
            ''''''     '##########   VASCA  #############
            ''''''     '#################################
            ''''''
            ''''''     'Mi procuro un elenco dei dati dell'eventuale VASCA
            ''''''
            ''''''     'Creo l'oggetto COM+
            ''''''     Set ObjVasca = CreateObject("Agro_Contab_AD.Vasche_R")
            ''''''
            ''''''     'Mi procuro il recordset richiesto
            ''''''     Set RsVasca = ObjVasca.Leggi(CStr(Piva), _
            ''''''                              CLng(Sa_Cod), _
            ''''''                              CLng(Fabbricato_Cod), _
            ''''''                              0, _
            ''''''                              2, _
            ''''''                              objCnManager, _
            ''''''                              FinestraTemp_Inizio, _
            ''''''                              FinestraTemp_Fine, _
            ''''''                              ConnessioneAlternativa)
            ''''''
            ''''''     'Se ottengo almeno un risultato, creo la struttura XML
            ''''''     If RsVasca.State <> 0 Then
            ''''''
            ''''''          bFabbricato = True
            ''''''
            ''''''          'Effettuo un ciclo sulle stalle
            ''''''          Do While Not RsVasca.EOF
            ''''''
            ''''''             '----- < VASCA > -----
            ''''''             Set XmlVasca = XmlDoc.createElement("Vasca")
            ''''''
            ''''''             With XmlVasca
            ''''''                .setAttribute "TipoOperazioneDB", IIf(ForDelete, "3", "0")
            ''''''                .setAttribute "piva", Agro_SQL_Load(RsVasca("piva"))
            ''''''                .setAttribute "sa_cod", Agro_SQL_Load(RsVasca("sa_cod"))
            ''''''                .setAttribute "vas_cod", Agro_SQL_Load(RsVasca("vas_cod"))
            ''''''                .setAttribute "identificativo", Agro_SQL_Load(RsVasca("identificativo"))
            ''''''                .setAttribute "numero_serie", Agro_SQL_Load(RsVasca("numero_serie"))
            ''''''                .setAttribute "modello", Agro_SQL_Load(RsVasca("modello"))
            ''''''                .setAttribute "materiale_cod", Agro_SQL_Load(RsVasca("materiale_cod"))
            ''''''                .setAttribute "appoggio_cod", Agro_SQL_Load(RsVasca("appoggio_cod"))
            ''''''                .setAttribute "inclinato", Agro_SQL_Load(RsVasca("inclinato"))
            ''''''                .setAttribute "tipo_tasca", Agro_SQL_Load(RsVasca("tipo_tasca"))
            ''''''                .setAttribute "coibentata", Agro_SQL_Load(RsVasca("coibentata"))
            ''''''                .setAttribute "udm_cod_capacita", Agro_SQL_Load(RsVasca("udm_cod_capacita"))
            ''''''                .setAttribute "capacita_nominale", Agro_SQL_Load(RsVasca("capacita_nominale"))
            ''''''                .setAttribute "capacita_effettiva", Agro_SQL_Load(RsVasca("capacita_effettiva"))
            ''''''                .setAttribute "udm_cod_altezza", Agro_SQL_Load(RsVasca("udm_cod_altezza"))
            ''''''                .setAttribute "altezza_cilindro", Agro_SQL_Load(RsVasca("altezza_cilindro"))
            ''''''                .setAttribute "altezza_totale", Agro_SQL_Load(RsVasca("altezza_totale"))
            ''''''                .setAttribute "udm_cod_peso", Agro_SQL_Load(RsVasca("udm_cod_peso"))
            ''''''                .setAttribute "peso", Agro_SQL_Load(RsVasca("peso"))
            ''''''                .setAttribute "dimx", Agro_SQL_Load(RsVasca("dimx"))
            ''''''                .setAttribute "dimy", Agro_SQL_Load(RsVasca("dimy"))
            ''''''                .setAttribute "rotazione", Agro_SQL_Load(RsVasca("rotazione"))
            ''''''                .setAttribute "costo_acquisto", Agro_SQL_Load(RsVasca("costo_acquisto"))
            ''''''                .setAttribute "ammortamento", Agro_SQL_Load(RsVasca("ammortamento"))
            ''''''                .setAttribute "ultima_revisione", Agro_SQL_Load(RsVasca("ultima_revisione"))
            ''''''                .setAttribute "note", Agro_SQL_Load(RsVasca("note"))
            ''''''                .setAttribute "posx", Agro_SQL_Load(RsVasca("posx"))
            ''''''                .setAttribute "posy", Agro_SQL_Load(RsVasca("posy"))
            ''''''                .setAttribute "tipo", Agro_SQL_Load(RsVasca("tipo"))
            ''''''                .setAttribute "spessore", Agro_SQL_Load(RsVasca("spessore"))
            ''''''                .setAttribute "validita_inizio", Agro_SQL_Load(RsVasca("validita_inizio"))
            ''''''                .setAttribute "validita_fine", Agro_SQL_Load(RsVasca("validita_fine"))
            ''''''             End With
            ''''''
            ''''''             XmlDatiFabbricati.appendChild XmlVasca
            ''''''
            ''''''             Set XmlVasca = Nothing
            ''''''             '----- < / VASCA > -----
            ''''''
            ''''''             RsVasca.MoveNext
            ''''''
            ''''''          Loop
            ''''''
            ''''''          RsVasca.Close
            ''''''
            ''''''     End If
            ''''''
            ''''''     Set RsVasca = Nothing
            ''''''     Set ObjVasca = Nothing


            If bFabbricato Then

                XmlDoc.AppendChild(XmlDatiFabbricati)

                RisultatoFunzione = XmlDoc.OuterXml
                '----- < / Documento XML > -----

                XmlDatiFabbricati = Nothing
                XmlFabbricato = Nothing
                XmlDoc = Nothing

            Else

                'Altrimenti, se non risulta selezionato nessun fabbricato ...
                RisultatoFunzione = ""

            End If


            '//////////////////////////////
            '//////////////////////////////
            '//////////////////////////////


            '------------------------------

            If FlagConnessioneLocale Then
                objParametri.objConnessione.Close()
                objParametri.objConnessione.Dispose()
            End If

        Catch ex As Exception
            RisultatoFunzione = ""
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return RisultatoFunzione

    End Function

    Public Function Leggi_Fabbricati_APP(ByVal piva As String, ByVal sa_cod As Integer, ByRef objParametri_Server As AgronicaCoreParametri, Optional ByVal filtroMagazziniAPP As Boolean = True) As List(Of Fabbricato)

        ' Magazzini APP
        Dim leggi_fabbricati_codice As New Fabbricati_Codici_R
        Dim dtCodiciFabbricati = leggi_fabbricati_codice.Leggi(piva, sa_cod, 0, 0, "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

        Dim magazziniAPP As New List(Of String)
        Dim magazziniUsoTerzi As New List(Of String)
        For Each row In dtCodiciFabbricati.Rows
            If row.Item("id_cod") = enum_CodiciAnagrafe.Visibile_da_App AndAlso row.Item("val_cod") = "1" Then
                magazziniAPP.Add(row.Item("PIVA") & "_" & row.Item("sa_cod") & "_" & row.Item("Fabbricato_cod"))
            ElseIf row.Item("id_cod") = enum_CodiciAnagrafe.Fabbricato_Uso_da_Terzi AndAlso row.Item("val_cod") = "1" Then
                magazziniUsoTerzi.Add(row.Item("PIVA") & "_" & row.Item("sa_cod") & "_" & row.Item("Fabbricato_cod"))
            End If
        Next

        Dim leggiFabbricati As New Fabbricati_R
        Dim filtroAggiuntivo As String = " Fabbricati.Tipo_Fabbricato_Cod IN (20,50,120,121,122,123) "
        Dim dtFabbricati = leggiFabbricati.Leggi_3(piva, sa_cod, 0, filtroAggiuntivo, "", objParametri_Server, True)

        Dim listaMagazzini As List(Of Fabbricato) = (
            From dr In dtFabbricati.Rows
            Where Not filtroMagazziniAPP OrElse magazziniAPP.Contains(dr.Item("Piva") & "_" & dr.Item("Sa_Cod") & "_" & dr.Item("Fabbricato_cod"))
            Select New Fabbricato() With {
                .primaryKey = New Fabbricato.PK() With {
                    .codice = dr("Fabbricato_Cod"),
                    .centroAziendalePK = New CentroAziendale.PK(dr.Item("Sa_Cod"), dr.Item("Piva"))},
                .descrizione = dr("Fabbricato_Des"),
                .tipo = dr.Item("Tipo_Fabbricato_Cod"),
                .usoDaTerzi = If(magazziniUsoTerzi.Contains(dr.Item("Piva") & "_" & dr.Item("Sa_Cod") & "_" & dr.Item("Fabbricato_cod")), True, False),
                .indirizzo = If(dr("Indirizzo_Cod") = 0, Nothing, New Indirizzo(dr("Indirizzo_Cod")) With {
                    .via = dr("ind_des"),
                    .cap = dr("CAP"),
                    .frazione = dr("frz_des"),
                    .istatComune = New AgronicaCoreModelsSTD.metaschema.Istat() With {
                        .com = dr("com_cod_istat"),
                        .prov = dr("pro_cod_istat"),
                        .localita = dr("localita"),
                        .comuni_prov = dr("comuni_prov")
                    },
                    .stato = New AgronicaCoreModelsSTD.metaschema.CodiciNazioniISO3166(dr("stato")),
                    .note = If(IsDBNull(dr("note")), "", dr("note"))
            })
        }).ToList

        Return listaMagazzini

    End Function

    Public Function Leggi_Magazzini_QdC(ByVal Piva As String,
                                        ByVal Sa_Cod As Integer,
                                        ByVal Lav_Cod As Integer,
                                        ByVal Data_Operazione As Date,
                                        ByVal objParametri_Server As AgronicaCoreParametri,
                                        ByVal objParametri_Utenti As AgronicaCoreParametri) As List(Of Fabbricato)

        Dim listMagazzini As New List(Of Fabbricato)

        Dim xFiltroAggiuntivo As String = " (Fabbricati.Tipo_Fabbricato_Cod = 20 OR Fabbricati.Tipo_Fabbricato_Cod = 50 OR Fabbricati.Tipo_Fabbricato_Cod = 120 OR Fabbricati.Tipo_Fabbricato_Cod = 121 OR Fabbricati.Tipo_Fabbricato_Cod = 122 OR Fabbricati.Tipo_Fabbricato_Cod = 123) "
        xFiltroAggiuntivo += " AND (Fabbricati.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Operazione) & " AND Fabbricati.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Operazione) & ")"

        Dim DT As DataTable
        Dim DTTerzisti As DataTable
        Dim objFabbricati As New AgronicaCoreAnagrafeDAL.Fabbricati_R

        Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim Impostazione_Mag As String

        Impostazione_Mag = objUtenti.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.UTENTE_GESTIONE_MAGAZZINO_2,
                                                                                objParametri_Utenti,
                                                                                1)

        Dim Flag_GestioneMagazziniImpresaPadre As Boolean = False

        'Per ora nel nuovo QdC non è gestito il giro del magazzino dell'impresa padre
        'Select Case Lav_Cod
        '    Case LAVCOD_TRAPIANTO, LAVCOD_SEMINA, LAVCOD_SOVESCIO, LAVCOD_SOD_SEDDING
        '        Flag_GestioneMagazziniImpresaPadre = True
        'End Select

        Dim Pive_Terzisti As String = ""
        Dim Flag_GestioneMagazziniTerzisti As Boolean = False

        'Per ora nel nuovo QdC non è gestito il giro del magazzino del terzista
        'Select Case Lav_Cod

        '    Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_DISERBO, LAVCOD_CONCIA_SEME,
        '         LAVCOD_DISSECCAMENTO, LAVCOD_GEODISINFESTAZIONE, LAVCOD_TRATTAMENTO_FITOREGOLATORE,
        '         LAVCOD_DISTRIBUZIONE_CONCIME, LAVCOD_SARCHIATURA_CONCIMAZIONE, LAVCOD_DISTRIBUZIONE_AMMENDANTI,
        '         LAVCOD_CONCIMAZIONE_FOGLIARE, LAVCOD_FERTIRRIGAZIONE, LAVCOD_TRATTAMENTO_ANTIBUTTERATURA,
        '         LAVCOD_TRAPIANTO, LAVCOD_SEMINA, LAVCOD_SOVESCIO, LAVCOD_SOD_SEDDING

        '        Dim objcontatti As New AgronicaCoreAnagrafeDAL.Contatti_R
        '        Dim Dt_Terzisti As DataTable
        '        Dt_Terzisti = objcontatti.Leggi_Contatti_ImpreseGias_ByCod_Rapporto(False,
        '                                                            Piva,
        '                                                            "",
        '                                                            COD_TERZISTA,
        '                                                            "", "",
        '                                                            objParametri_Server)
        '        If Dt_Terzisti IsNot Nothing AndAlso Dt_Terzisti.Rows.Count > 0 Then
        '            For t = 0 To Dt_Terzisti.Rows.Count - 1
        '                Pive_Terzisti &= "'" & Dt_Terzisti.Rows(t).Item("cod_contatto") & "',"
        '            Next
        '            If Pive_Terzisti <> "" Then
        '                Pive_Terzisti = Left(Pive_Terzisti, Pive_Terzisti.Length - 1)
        '                Flag_GestioneMagazziniTerzisti = True
        '            End If
        '        End If

        'End Select



        If Not Flag_GestioneMagazziniImpresaPadre Then
            '=================================================================
            '============= GESTIONE SOLO MAGAZZINI IMPRESA ======================
            '=================================================================

            DT = objFabbricati.Leggi_2(Piva,
                      Sa_Cod,
                      0,
                      0,
                      xFiltroAggiuntivo,
                      " Fabbricati.Fabbricato_Des ",
                      objParametri_Server)



        Else
            '=================================================================
            '============= GESTIONE MAGAZZINI IMPRESA PADRE ======================
            '=================================================================

            Select Case Impostazione_Mag

                Case "", "0"

                    DT = objFabbricati.Leggi_2(Piva,
                        Sa_Cod,
                        0,
                        0,
                        xFiltroAggiuntivo,
                        " Fabbricati.Fabbricato_Des ",
                        objParametri_Server)

                Case Else

                    Dim Piva_Padre As String
                    Dim xFiltroAggiuntivo2 As String

                    xFiltroAggiuntivo2 = xFiltroAggiuntivo

                    If CInt(Impostazione_Mag) = 1 Then
                        Piva_Padre = objParametri_Server.PivaSuperUser
                    Else
                        'magazzini dell'impresa padre
                        Dim objGerarchia As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R
                        Dim strPive As String
                        strPive = objGerarchia.Ricava_Stringa_PivePadre(Piva,
                                                                        "",
                                                                        objParametri_Server)

                        xFiltroAggiuntivo2 += " AND ( Fabbricati.Piva IN " & strPive & ")"

                        Piva_Padre = ""
                    End If

                    DT = objFabbricati.LeggiUNIONFabbricatiImpresaPadre(Piva,
                                                                         Sa_Cod,
                                                                         0,
                                                                         Piva_Padre,
                                                                         0,
                                                                         0,
                                                                         False,
                                                                         xFiltroAggiuntivo,
                                                                         xFiltroAggiuntivo2,
                                                                         " Fabbricati.Fabbricato_Des ",
                                                                         objParametri_Server)

            End Select

        End If '_Flag_GestioneMagazziniImpresaPadre

        If Flag_GestioneMagazziniTerzisti AndAlso Pive_Terzisti <> "" Then

            Dim xFiltroAggiuntivo2 As String = ""
            DTTerzisti = objFabbricati.LeggiUNIONFabbricatiImpreseTerzisti(Piva,
                                                                     Sa_Cod,
                                                                     0,
                                                                     Pive_Terzisti,
                                                                     False,
                                                                     xFiltroAggiuntivo,
                                                                     xFiltroAggiuntivo2,
                                                                     " Fabbricati.Fabbricato_Des ",
                                                                     objParametri_Server)

            DT.Merge(DTTerzisti)

            DT = DT.DefaultView.ToTable(True, "Piva", "Sa_Cod", "Sa_Nome", "Rag_Soc", "Fabbricato_Des", "Fabbricato_Cod")

        End If

        Dim Des As String = ""
        Dim Piva_Value As String

        If Not IsNothing(DT) Then

            For i = 0 To DT.Rows.Count - 1

                Piva_Value = CStr(DT.Rows(i).Item("Piva"))

                If Not (Flag_GestioneMagazziniTerzisti AndAlso Pive_Terzisti <> "") Then

                    If Not Flag_GestioneMagazziniImpresaPadre Then

                        Des = CStr(DT.Rows(i).Item("Fabbricato_Des")) & " (" & CStr(DT.Rows(i).Item("Sa_Nome")) & ")"

                    Else


                        Select Case Impostazione_Mag
                            Case "", "0"
                                Des = CStr(DT.Rows(i).Item("Fabbricato_Des")) & " (" & CStr(DT.Rows(i).Item("Sa_Nome")) & ")"
                            Case Else
                                Des = CStr(DT.Rows(i).Item("Fabbricato_Des")) &
                                                " (" & CStr(DT.Rows(i).Item("Sa_Nome")) & ")" &
                                                " --- " & CStr(DT.Rows(i).Item("Rag_Soc"))

                        End Select

                    End If

                Else

                    Des = CStr(DT.Rows(i).Item("Fabbricato_Des")) &
                                            " (" & CStr(DT.Rows(i).Item("Sa_Nome")) & ")" &
                                            " --- " & CStr(DT.Rows(i).Item("Rag_Soc"))
                End If

                listMagazzini.Add(New Fabbricato() With
                                {
                                    .primaryKey = New Fabbricato.PK() With {
                                        .codice = DT.Rows(i).Item("Fabbricato_Cod"),
                                        .centroAziendalePK = New CentroAziendale.PK(DT.Rows(i).Item("Sa_Cod"), Piva_Value)
                                     },
                                    .descrizione = Des,
                                    .tipo = CostantiPersonalizzate.MAGAZZINO
                                })

            Next
        End If

        Return listMagazzini

    End Function


    ' -----------------------------------------------------------------------------
    ' <summary>
    'Prendo l'ultimo Magazzino in cui è stato movimentato quel prodotto in un certo periodo temporale
    ' </summary>
    ' -----------------------------------------------------------------------------
    Public Function Ultimo_Magazzino_Prodotto_Movimentato(ByVal Data_Inizio As Date,
                                                          ByVal Data_Fine As Date,
                                                          ByVal Piva As String,
                                                          ByVal Sa_Cod As Integer,
                                                          ByVal Elem_Cod As Integer,
                                                          ByVal Pro_Cod As Integer,
                                                          ByVal Mat_Cod As Integer,
                                                          ByVal objParametri_Server As AgronicaCoreParametri,
                                                          ByVal objParametri_Utenti As AgronicaCoreParametri) As Fabbricato

        Dim Magazzino As Fabbricato = Nothing

        Dim objMovimenti As New AgronicaCoreContabDAL.Movimenti_R

        Dim dtScheda As DataTable = objMovimenti.SchedaMovimentiMagazzino(Data_Inizio,
                                                                 Data_Fine,
                                                                 Piva,
                                                                 Sa_Cod,
                                                                 0,
                                                                 Elem_Cod,
                                                                 Pro_Cod,
                                                                 Mat_Cod,
                                                                 0,
                                                                 0,
                                                                 0,
                                                                 0,
                                                                 LOTTO_NONDEFINITO,
                                                                 "", "", "", "", "", "", "", "", "", "", "",
                                                                 " Movimenti.Data_Movimento DESC ",
                                                                 objParametri_Server, objParametri_Utenti)


        If Not IsNothing(dtScheda) AndAlso dtScheda.Rows.Count > 0 Then

            Magazzino = New Fabbricato With {
                        .primaryKey = New Fabbricato.PK With {
                            .centroAziendalePK = New CentroAziendale.PK With {
                                .codice = dtScheda(0)("Sa_Cod"),
                                .partitaIva = dtScheda(0)("Piva")
                            },
                            .codice = dtScheda(0)("Id_Destinazione")
                        },
                        .descrizione = dtScheda(0)("Fabbricato_Des")
                }

        End If

        Return Magazzino

    End Function

    Public Function Leggi_Fabbricato_Oggetto(ByVal Piva As String,
                                             ByVal Sa_Cod As Integer,
                                             ByVal Fabbricato_Cod As Integer,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As Fabbricato

        Dim objFabbricati_R As New AgronicaCoreAnagrafeDAL.Fabbricati_R

        Dim fabbricatoLetto As New Fabbricato()

        Dim objIndirizziBIZ_R As New Indirizzi_R

        Dim fabbricatoDataRow As DataTable = objFabbricati_R.Leggi(CStr(Piva),
                                                                   CStr(Sa_Cod),
                                                                   CStr(Fabbricato_Cod),
                                                                   enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                   "",
                                                                   "",
                                                                   objParametri)

        If fabbricatoDataRow.Rows.Count <> 0 Then

            Dim pkParticella As New AgronicaCoreModelsSTD.anagrafiche.ParticelleCatastali.PK(fabbricatoDataRow.Rows(0)("prov"), fabbricatoDataRow.Rows(0)("com"), fabbricatoDataRow.Rows(0)("sezione"), fabbricatoDataRow.Rows(0)("foglio"), fabbricatoDataRow.Rows(0)("numero"), fabbricatoDataRow.Rows(0)("subalterno"))

            'mappo tutti i campi nell'oggetto fabbricato

            fabbricatoLetto.fabbricato_cod = fabbricatoDataRow.Rows(0)("fabbricato_cod")

            Dim indirizzoPerFabbricato As Indirizzo = objIndirizziBIZ_R.Leggi_Indirizzo(fabbricatoDataRow.Rows(0)("indirizzo_cod"), objParametri)

            fabbricatoLetto.indirizzo = indirizzoPerFabbricato

            fabbricatoLetto.tipo = fabbricatoDataRow.Rows(0)("tipo_fabbricato_cod")
            fabbricatoLetto.particella = New AgronicaCoreModelsSTD.anagrafiche.ParticelleCatastali(pkParticella)
            fabbricatoLetto.validita = New IntervalloTemporale(fabbricatoDataRow.Rows(0)("validita_inizio"), fabbricatoDataRow.Rows(0)("validita_fine"))
            fabbricatoLetto.descrizione = If(fabbricatoDataRow.Rows(0)("fabbricato_des") IsNot DBNull.Value, fabbricatoDataRow.Rows(0)("fabbricato_des"), "")

        End If

        Return fabbricatoLetto

    End Function

    Public Function TrovaCodiceASLFabbricato(Piva As String, Sa_Cod As Integer, Fabbricato_Cod As Integer, ByVal objParametri_Server As AgronicaCoreParametri) As String
        Dim fabbricato_read As New AgronicaCoreAnagrafeDAL.Fabbricati_R
        Dim dtFabbricato = fabbricato_read.Leggi(Piva, Sa_Cod, Fabbricato_Cod, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
        If dtFabbricato.Rows.Count > 0 Then
            Dim indirizzo_cod = dtFabbricato.Rows(0)("Indirizzo_Cod")
            Dim objIndirizzi As New AgronicaCoreAnagrafeBIZ.Indirizzi_R
            Dim CodiceASL = objIndirizzi.CodiceAslDatoCodIndirizzo(indirizzo_cod, objParametri_Server)
            Return CodiceASL
        End If
        Return ""
    End Function

    Public Function Fabbricati_con_Uso_da_Terzi_Visibilita_Utente(ByVal objParametri_Server As AgronicaCoreParametri, ByVal objParametri_Utenti As AgronicaCoreParametri) As List(Of AgronicaCoreModelsSTD.anagrafiche.Fabbricato)
        Dim DT As DataTable

        Dim objProfilo As New AgronicaCoreUtentiDAL.Utenti_Profili_Read
        Dim DTProfilo As DataTable
        Dim Sql_Permessi As String = ""


        DTProfilo = objProfilo.Leggi(objParametri_Utenti.UtenteUsername,
                                     CInt(5),
                                     enumSelezioneVariabile.Selezione_TabellaCompleta,
                                     "", "",
                                     objParametri_Utenti)

        If DTProfilo.Rows.Count > 0 Then
            Sql_Permessi = DTProfilo.Rows(0).Item("Descrizione_2") & ""
        End If
        '----------------------------------------------------------------

        Dim Filtro_Visibilita_Utente = True
        If Sql_Permessi = "" Then
            Filtro_Visibilita_Utente = False
        End If

        Dim objFabbricati_Dal As New AgronicaCoreAnagrafeDAL.Fabbricati_R

        DT = objFabbricati_Dal.Leggi_Fabbricati_Uso_da_Terzi_Visibilita_Utente(objParametri_Server, objParametri_Utenti, Filtro_Visibilita_Utente)

        Dim listaFabbricati As List(Of AgronicaCoreModelsSTD.anagrafiche.Fabbricato) = Nothing
        If DT IsNot Nothing AndAlso DT.Rows.Count > 0 Then

            listaFabbricati = New List(Of AgronicaCoreModelsSTD.anagrafiche.Fabbricato)

            For Each row In DT.Rows
                listaFabbricati.Add(New Fabbricato With {
                        .primaryKey = New Fabbricato.PK With {
                            .centroAziendalePK = New CentroAziendale.PK With {
                                .codice = row("SA_COD"),
                                .partitaIva = row("PIVA")
                            },
                            .codice = row("Fabbricato_Cod")
                        },
                        .descrizione = row("Fabbricato_Des")
                })
            Next
        End If

        Return listaFabbricati
    End Function

    Public Function LeggiTipoCodiceFabbricato(Piva As String, Sa_Cod As Integer, Fabbricato_Cod As Integer, ByVal objParametri_Server As AgronicaCoreParametri) As Integer
        Dim fabbricato_read As New AgronicaCoreAnagrafeDAL.Fabbricati_R
        Dim dtFabbricato = fabbricato_read.Leggi(Piva, Sa_Cod, Fabbricato_Cod, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
        If dtFabbricato.Rows.Count = 1 Then
            Return dtFabbricato.Rows(0)("Tipo_Fabbricato_Cod")
        End If
        Return -1
    End Function

    Public Function PresenzaMovimentiBoxDaStalla(Piva As String, Sa_Cod As Integer, Fabbricato_Cod As Integer, ByVal objParametri_Server As AgronicaCoreParametri) As Boolean
        Dim ObjFabbricato As New AgronicaCoreAnagrafeDAL.Fabbricati_R
        Dim numeroMovimenti = ObjFabbricato.MovimentiBoxDaStalla(Piva, Sa_Cod, Fabbricato_Cod, objParametri_Server)
        If numeroMovimenti > 0 Then
            Return True
        Else
            Return False
        End If
    End Function
End Class


'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§


Public Class Fabbricato_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Fabbricato_Scrivi(ByVal DatiFabbricati As String,
                                      ByRef OUTPUT_Piva As String,
                                      ByRef OUTPUT_Sa_Cod As Integer,
                                      ByRef OUTPUT_Fabbricato_Cod As Integer,
                                      ByRef objParametri As AgronicaCoreParametri,
                                      Optional ByVal TipoG2G As Integer = 0,
                                      Optional NoteLog As String = ""
                                      ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.Fabbricato_W.Fabbricato_Scrivi()"

        '----------------------------------------------------------------------

        Dim dummy As Boolean

        Dim objSequenze As New Agro_Sequenze
        Dim objFabbricati As AgronicaCoreAnagrafeDAL.Fabbricati_W
        Dim objIndirizzi As AgronicaCoreAnagrafeDAL.Indirizzi_Write
        Dim objFabbricatoxCodici As AgronicaCoreAnagrafeDAL.Fabbricati_Codici_W

        Dim objStalla As AgronicaCoreAnagrafeDAL.Stalla_W
        Dim objStallaxCaratteristiche As AgronicaCoreAnagrafeDAL.Stalla_Caratteristiche_W
        Dim ObjStallaConfigurazioniBDN As AgronicaCoreAnagrafeDAL.Stalla_Configurazioni_BDN_W
        Dim ObjFabbricati_Impostazioni As AgronicaCoreAnagrafeDAL.Fabbricati_Impostaz_W
        Dim objVasca As AgronicaCoreContabDAL.Vasche_W
        Dim objAgronicaLogAnagrafeW As New AgronicaCoreAnagrafeDAL.AgronicaLogAnagrafe_W


        Dim xmlDoc As XmlDocument

        Dim xDatiFabbricati As XmlNodeList
        Dim xDatiFabbricato As XmlElement
        Dim xFabbricati As XmlNodeList
        Dim xFabbricato As XmlElement
        Dim xIndirizzi As XmlNodeList
        Dim xIndirizzo As XmlElement
        Dim xStalle As XmlNodeList
        Dim xStalla As XmlElement
        Dim xVasche As XmlNodeList
        Dim xVasca As XmlElement
        Dim xCodici As XmlNodeList
        Dim xCodice As XmlElement
        Dim xStallaCaratteristiche As XmlNodeList
        Dim xStallaCaratteristica As XmlElement
        Dim xStallaConfigurazioniBDN As XmlNodeList
        Dim xStallaConfigurazioneBDN As XmlElement
        Dim xDatiImpostazioni As XmlNodeList
        Dim xDatiImpostazione As XmlElement
        Dim xImpostazioni As XmlNodeList
        Dim xImpostazione As XmlElement

        Dim Cod_Fabbricato As Long
        Dim Cod_Indirizzo As Long
        Dim Cod_StallaCaratteristica As Long
        Dim Vas_Cod As Long

        Dim resLog As Boolean = False

        Dim i_DatiFabbricati As Integer
        Dim i_Fabbricato As Integer
        Dim i_Indirizzo As Integer
        Dim i_Stalla As Integer
        Dim i_Vasca As Integer
        Dim i_Codice As Integer
        Dim i_StallaCaratteristica As Integer
        Dim i_StallaConfigurazioneBDN As Integer
        Dim i_DatiImpostazioni As Integer
        Dim i_Impostazione As Integer

        Dim OpeDB_Fabbricato As String
        Dim OpeDB_Indirizzo As String
        Dim OpeDB_Stalla As String
        Dim OpeDB_Vasca As String
        Dim OpeDB_Codice As String
        Dim OpeDB_StallaCaratteristica As String
        Dim OpeDB_StallaConfigurazioneBDN As String
        Dim OpeDB_Impostazione As String

        Dim bFirst As Boolean

        '------------------------------
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim messaggioErrore As String = ""
        Dim xRisp As Boolean = True
        '------------------------------

        Try

            '------------------------------
            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                           FlagTransazioneLocale,
                                                           objParametri)
            '------------------------------

            xmlDoc = New XmlDocument
            'XmlDoc.async = False
            xmlDoc.LoadXml(DatiFabbricati)

            '------------------------------

            xDatiFabbricati = xmlDoc.GetElementsByTagName("DatiFabbricati")

            i_DatiFabbricati = 0

            Do While i_DatiFabbricati < xDatiFabbricati.Count

                'Prelevo l'i-esimo blocco di DatiFabbricati (in realtà ne esiste uno solo)
                xDatiFabbricato = xDatiFabbricati.Item(i_DatiFabbricati)

                '------------------------------

                xFabbricati = xDatiFabbricato.GetElementsByTagName("Fabbricato")

                i_Fabbricato = 0

                Do While i_Fabbricato < xFabbricati.Count

                    'Prelevo l' i-esima Codifica Fabbricato
                    xFabbricato = xFabbricati.Item(i_Fabbricato)

                    'Nota: eseguo prima l'inserimento dell'indirizzo per ottenere il Codice

                    '-------------------------------------------------------------
                    ' INDIRIZZI
                    '-------------------------------------------------------------

                    'Prelevo l'elenco degli indirizzi
                    xIndirizzi = xFabbricato.GetElementsByTagName("Indirizzo")

                    i_Indirizzo = 0

                    Do While i_Indirizzo < xIndirizzi.Count

                        'Prelevo l'i-esimo indirizzo
                        xIndirizzo = xIndirizzi.Item(i_Indirizzo)

                        'Prelevo gli attributi dell'indirizzo selezionato
                        OpeDB_Indirizzo = xIndirizzo.GetAttribute("TipoOperazioneDB")

                        objIndirizzi = New AgronicaCoreAnagrafeDAL.Indirizzi_Write

                        Cod_Indirizzo = CInt(xIndirizzo.GetAttribute("cod_indirizzo"))

                        'Verifico l'operazione richiesta
                        Select Case OpeDB_Indirizzo

                            Case "0"    'LEGGI -------------------------------------------------------


                            Case "1"    'SALVA -------------------------------------------------------

                                If Cod_Indirizzo <= 0 Then

                                    'Richiedo un nuovo codice indirizzo
                                    Cod_Indirizzo = objSequenze.NuovoId_Tabella("Indirizzi",
                                                                                CInt(xIndirizzo.GetAttribute("basecode")),
                                                                                CInt(xIndirizzo.GetAttribute("topcode")),
                                                                                objParametri)

                                    'Salvo l'indirizzo
                                    dummy = objIndirizzi.Scrivi(Cod_Indirizzo,
                                                                CStr(xIndirizzo.GetAttribute("ind_des")),
                                                                CStr(xIndirizzo.GetAttribute("frz_des")),
                                                                CStr(xIndirizzo.GetAttribute("cap")),
                                                                CStr(xIndirizzo.GetAttribute("com_des")),
                                                                CStr(xIndirizzo.GetAttribute("pro_cod")),
                                                                CStr(xIndirizzo.GetAttribute("stato")),
                                                                CStr(xIndirizzo.GetAttribute("note")),
                                                                CStr(xIndirizzo.GetAttribute("pro_cod_istat")),
                                                                CStr(xIndirizzo.GetAttribute("com_cod_istat")),
                                                                CDate(xIndirizzo.GetAttribute("validita_inizio")),
                                                                CDate(xIndirizzo.GetAttribute("validita_fine")),
                                                                objParametri)

                                Else

                                    'Esportazione in Locale

                                    'Marco Nota: l'indirizzo non viene esportato, poiché in alcune
                                    'installazioni (causa importazione dati) è lo stesso cod_indirizzo
                                    'già inserito a livello di centro aziendale --> evito la chiave duplicata

                                End If

                            Case "2"    'MODIFICA -------------------------------------------------------

                                objIndirizzi.Modifica(CInt(xIndirizzo.GetAttribute("cod_indirizzo")),
                                                      CStr(xIndirizzo.GetAttribute("ind_des")),
                                                      CStr(xIndirizzo.GetAttribute("frz_des")),
                                                      CStr(xIndirizzo.GetAttribute("cap")),
                                                      CStr(xIndirizzo.GetAttribute("com_des")),
                                                      CStr(xIndirizzo.GetAttribute("pro_cod")),
                                                      CStr(xIndirizzo.GetAttribute("stato")),
                                                      CStr(xIndirizzo.GetAttribute("note")),
                                                      CStr(xIndirizzo.GetAttribute("pro_cod_istat")),
                                                      CStr(xIndirizzo.GetAttribute("com_cod_istat")),
                                                      CDate(xIndirizzo.GetAttribute("validita_inizio")),
                                                      CDate(xIndirizzo.GetAttribute("validita_fine")),
                                                      "",
                                                      objParametri)


                            Case "3"    'ELIMINA -------------------------------------------------------

                                objIndirizzi.Cancella(CInt(xIndirizzo.GetAttribute("cod_indirizzo")),
                                                      "",
                                                      objParametri)

                        End Select

                        'Elimino l'oggetto

                        objIndirizzi = Nothing

                        'Incremento l'indice
                        i_Indirizzo += 1

                    Loop


                    '-------------------------------------------------------------
                    ' FABBRICATO
                    '-------------------------------------------------------------

                    'Prelevo gli attributi della materia prima selezionata
                    OpeDB_Fabbricato = xFabbricato.GetAttribute("TipoOperazioneDB")

                    objFabbricati = New AgronicaCoreAnagrafeDAL.Fabbricati_W

                    'Inizializzo Preventivamente il Cod_Fabbricato
                    Cod_Fabbricato = CStr(xFabbricato.GetAttribute("fabbricato_cod"))

                    'Verifico l'operazione richiesta
                    Select Case OpeDB_Fabbricato

                        Case "0"    'LEGGI -------------------------------------------------------

                            OUTPUT_Piva = CStr(xFabbricato.GetAttribute("piva"))
                            OUTPUT_Sa_Cod = CInt(xFabbricato.GetAttribute("sa_cod"))
                            OUTPUT_Fabbricato_Cod = CInt(xFabbricato.GetAttribute("fabbricato_cod"))

                        Case "1"    'SALVA -------------------------------------------------------

                            If Cod_Fabbricato <= 0 Then

                                'Richiedo un nuovo codice movimento
                                Cod_Fabbricato = objSequenze.NuovoId_xPiva_xSaCod("SeqMagazzino",
                                                                                  "Mag_Cod",
                                                                                  CStr(xFabbricato.GetAttribute("piva")),
                                                                                  CInt(xFabbricato.GetAttribute("sa_cod")),
                                                                                  CInt(xFabbricato.GetAttribute("basecode")),
                                                                                  CInt(xFabbricato.GetAttribute("topcode")),
                                                                                  objParametri)

                                OUTPUT_Piva = CStr(xFabbricato.GetAttribute("piva"))
                                OUTPUT_Sa_Cod = CInt(xFabbricato.GetAttribute("sa_cod"))
                                OUTPUT_Fabbricato_Cod = Cod_Fabbricato

                            Else

                                'Esportazione in Locale
                                OUTPUT_Piva = CStr(xFabbricato.GetAttribute("piva"))
                                OUTPUT_Sa_Cod = CInt(xFabbricato.GetAttribute("sa_cod"))
                                OUTPUT_Fabbricato_Cod = CInt(xFabbricato.GetAttribute("fabbricato_cod"))

                            End If

                            dummy = objFabbricati.Scrivi(CStr(xFabbricato.GetAttribute("piva")),
                                                         CInt(xFabbricato.GetAttribute("sa_cod")),
                                                         CInt(Cod_Fabbricato),
                                                         CStr(xFabbricato.GetAttribute("fabbricato_des")),
                                                         CInt(Cod_Indirizzo),
                                                         CInt(xFabbricato.GetAttribute("tipo_fabbricato_cod")),
                                                         CStr(xFabbricato.GetAttribute("prov")),
                                                         CStr(xFabbricato.GetAttribute("com")),
                                                         CStr(xFabbricato.GetAttribute("sezione")),
                                                         CInt(xFabbricato.GetAttribute("foglio")),
                                                         CInt(xFabbricato.GetAttribute("numero")),
                                                         CStr(xFabbricato.GetAttribute("subalterno")),
                                                         CDbl(xFabbricato.GetAttribute("mc_convenzionale")),
                                                         CDbl(xFabbricato.GetAttribute("mc_conversione")),
                                                         CDbl(xFabbricato.GetAttribute("mc_biologico")),
                                                         CInt(xFabbricato.GetAttribute("regolamento_cod")),
                                                         CInt(xFabbricato.GetAttribute("titolopossesso")),
                                                         CInt(xFabbricato.GetAttribute("idoneo_costruzione")),
                                                         CInt(xFabbricato.GetAttribute("idoneo_separazambienti")),
                                                         CInt(xFabbricato.GetAttribute("idoneo_separazprodotti")),
                                                         CInt(xFabbricato.GetAttribute("idoneo_condigieniche")),
                                                         CInt(xFabbricato.GetAttribute("idoneo_autorizsanitaria")),
                                                         CInt(xFabbricato.GetAttribute("idoneo_haccp")),
                                                         CInt(xFabbricato.GetAttribute("idoneo_planimetria")),
                                                         CInt(xFabbricato.GetAttribute("idoneo_layout")),
                                                         CInt(xFabbricato.GetAttribute("idoneo_diagrammiflusso")),
                                                         CInt(xFabbricato.GetAttribute("idoneo_cdx_m004")),
                                                         Agro_XML_GetInteger(xFabbricato, "idoneo_supmincoperte", 0),
                                                         Agro_XML_GetInteger(xFabbricato, "idoneo_supminscoperte", 0),
                                                         CDate(xFabbricato.GetAttribute("conversione_inizio")),
                                                         CDate(xFabbricato.GetAttribute("conversione_fine")),
                                                         Agro_XML_GetDecimal(xFabbricato, "mq_convenzionale", 0),
                                                         Agro_XML_GetDecimal(xFabbricato, "mq_conversione", 0),
                                                         Agro_XML_GetDecimal(xFabbricato, "mq_biologico", 0),
                                                         Agro_XML_GetDecimal(xFabbricato, "mq_convenzionale_scoperto", 0),
                                                         Agro_XML_GetDecimal(xFabbricato, "mq_conversione_scoperto", 0),
                                                         Agro_XML_GetDecimal(xFabbricato, "mq_biologico_scoperto", 0),
                                                         Agro_XML_GetInteger(xFabbricato, "n_piani", 0),
                                                         Agro_XML_GetDecimal(xFabbricato, "sup_piano", 0),
                                                         Agro_XML_GetString(xFabbricato, "num_autorizzazione", ""),
                                                         Agro_XML_GetDate(xFabbricato, "data_richiesta_autorizzazione", #1/1/1900#),
                                                         Agro_XML_GetInteger(xFabbricato, "tipologia_utilizzo", 0),
                                                         CDate(xFabbricato.GetAttribute("validita_inizio")),
                                                         CDate(xFabbricato.GetAttribute("validita_fine")),
                                                         objParametri,
                                                         Data_creazione:=Agro_XML_GetDate(xFabbricato, "data_creazione", #2/1/1900#),
                                                         Data_modifica:=Agro_XML_GetDate(xFabbricato, "data_modifica", #2/1/1900#),
                                                         username_creazione:=Agro_XML_GetString(xFabbricato, "username_creazione", ""),
                                                         username_modifica:=Agro_XML_GetString(xFabbricato, "username_modifica", ""),
                                                         ChkVirtuale:=Agro_XML_GetInteger(xFabbricato, "chkvirtuale", 0),
                                                         ProprietarioCapi:=Agro_XML_GetString(xFabbricato, "proprietario_capi", ""),
                                                         ChkMagazzinoFarmaci:=Agro_XML_GetInteger(xFabbricato, "chkmagazzinofarmaci", 0),
                                                         CodiceBDN:=Agro_XML_GetString(xFabbricato, "codice_bdn", ""))


                            '************************************************
                            '*********** INIZIO LOGGING *******************
                            '************************************************

                            'MEMORIZZO L'INSERIMENTO DELL'OPERAZIONE NELLA TABELLA DI LOG (AGRONICA LOG ANAGRAFE)
                            'è necessario avere Migra >= 505

                            resLog = objAgronicaLogAnagrafeW.Scrivi(CInt(OpeDB_Fabbricato),
                                                                    enum_TipoEntita_Des.Fabbricati,
                                                                    CStr(xFabbricato.GetAttribute("piva")),
                                                                    CStr(xFabbricato.GetAttribute("sa_cod")),
                                                                    CStr(Cod_Fabbricato),
                                                                    Nothing,
                                                                    Nothing,
                                                                    Nothing,
                                                                    NoteLog, enum_Id_Servizio.GiasOnline,
                                                                    objParametri,
                                                                    object_data:=DatiFabbricati)

                            '************************************************
                            '*********** FINE LOGGING *********************
                            '************************************************


                        Case "2"    'MODIFICA -------------------------------------------------------

                            OUTPUT_Piva = CStr(xFabbricato.GetAttribute("piva"))
                            OUTPUT_Sa_Cod = CInt(xFabbricato.GetAttribute("sa_cod"))
                            OUTPUT_Fabbricato_Cod = CInt(xFabbricato.GetAttribute("fabbricato_cod"))

                            objFabbricati.Modifica(CStr(xFabbricato.GetAttribute("piva")),
                                                   CInt(xFabbricato.GetAttribute("sa_cod")),
                                                   CInt(Cod_Fabbricato),
                                                   CStr(xFabbricato.GetAttribute("fabbricato_des")),
                                                   CInt(xFabbricato.GetAttribute("indirizzo_cod")),
                                                   CInt(xFabbricato.GetAttribute("tipo_fabbricato_cod")),
                                                   CStr(xFabbricato.GetAttribute("prov")),
                                                   CStr(xFabbricato.GetAttribute("com")),
                                                   CStr(xFabbricato.GetAttribute("sezione")),
                                                   CInt(xFabbricato.GetAttribute("foglio")),
                                                   CInt(xFabbricato.GetAttribute("numero")),
                                                   CStr(xFabbricato.GetAttribute("subalterno")),
                                                   CDbl(xFabbricato.GetAttribute("mc_convenzionale")),
                                                   CDbl(xFabbricato.GetAttribute("mc_conversione")),
                                                   CDbl(xFabbricato.GetAttribute("mc_biologico")),
                                                   CInt(xFabbricato.GetAttribute("regolamento_cod")),
                                                   CInt(xFabbricato.GetAttribute("titolopossesso")),
                                                   CInt(xFabbricato.GetAttribute("idoneo_costruzione")),
                                                   CInt(xFabbricato.GetAttribute("idoneo_separazambienti")),
                                                   CInt(xFabbricato.GetAttribute("idoneo_separazprodotti")),
                                                   CInt(xFabbricato.GetAttribute("idoneo_condigieniche")),
                                                   CInt(xFabbricato.GetAttribute("idoneo_autorizsanitaria")),
                                                   CInt(xFabbricato.GetAttribute("idoneo_haccp")),
                                                   CInt(xFabbricato.GetAttribute("idoneo_planimetria")),
                                                   CInt(xFabbricato.GetAttribute("idoneo_layout")),
                                                   CInt(xFabbricato.GetAttribute("idoneo_diagrammiflusso")),
                                                   CInt(xFabbricato.GetAttribute("idoneo_cdx_m004")),
                                                   Agro_XML_GetInteger(xFabbricato, "idoneo_supmincoperte", 0),
                                                   Agro_XML_GetInteger(xFabbricato, "idoneo_supminscoperte", 0),
                                                   CDate(xFabbricato.GetAttribute("conversione_inizio")),
                                                   CDate(xFabbricato.GetAttribute("conversione_fine")),
                                                   Agro_XML_GetDecimal(xFabbricato, "mq_convenzionale", 0),
                                                   Agro_XML_GetDecimal(xFabbricato, "mq_conversione", 0),
                                                   Agro_XML_GetDecimal(xFabbricato, "mq_biologico", 0),
                                                   Agro_XML_GetDecimal(xFabbricato, "mq_convenzionale_scoperto", 0),
                                                   Agro_XML_GetDecimal(xFabbricato, "mq_conversione_scoperto", 0),
                                                   Agro_XML_GetDecimal(xFabbricato, "mq_biologico_scoperto", 0),
                                                   Agro_XML_GetInteger(xFabbricato, "n_piani", 0),
                                                   Agro_XML_GetDecimal(xFabbricato, "sup_piano", 0),
                                                   Agro_XML_GetString(xFabbricato, "num_autorizzazione", ""),
                                                   Agro_XML_GetDate(xFabbricato, "data_richiesta_autorizzazione", #1/1/1900#),
                                                   Agro_XML_GetInteger(xFabbricato, "tipologia_utilizzo", 0),
                                                   CDate(xFabbricato.GetAttribute("validita_inizio")),
                                                   CDate(xFabbricato.GetAttribute("validita_fine")),
                                                   "",
                                                   objParametri,
                                                   Data_modifica:=Agro_XML_GetDate(xFabbricato, "data_modifica", #2/1/1900#),
                                                   username_modifica:=Agro_XML_GetString(xFabbricato, "username_modifica", ""),
                                                   ChkVirtuale:=If(Not xFabbricato.HasAttribute("chkvirtuale"), Nothing, CInt(xFabbricato.GetAttribute("chkvirtuale"))),
                                                   ProprietarioCapi:=Agro_XML_GetString(xFabbricato, "proprietario_capi", ""),
                                                   ChkMagazzinoFarmaci:=Agro_XML_GetInteger(xFabbricato, "chkmagazzinofarmaci", 0),
                                                   CodiceBDN:=Agro_XML_GetString(xFabbricato, "codice_bdn", ""))



                            '************************************************
                            '*********** INIZIO LOGGING *******************
                            '************************************************

                            'MEMORIZZO L'INSERIMENTO DELL'OPERAZIONE NELLA TABELLA DI LOG (AGRONICA LOG ANAGRAFE)
                            'è necessario avere Migra >= 505

                            resLog = objAgronicaLogAnagrafeW.Scrivi(CInt(OpeDB_Fabbricato),
                                                                    enum_TipoEntita_Des.Fabbricati,
                                                                    CStr(xFabbricato.GetAttribute("piva")),
                                                                    CStr(xFabbricato.GetAttribute("sa_cod")),
                                                                    CStr(Cod_Fabbricato),
                                                                    Nothing,
                                                                    Nothing,
                                                                    Nothing,
                                                                    NoteLog, enum_Id_Servizio.GiasOnline,
                                                                    objParametri,
                                                                    object_data:=DatiFabbricati)

                            '************************************************
                            '*********** FINE LOGGING *********************
                            '************************************************

                        Case "3" 'CANCELLAZIONE  ------------------------------------------------------

                            OUTPUT_Piva = CStr(xFabbricato.GetAttribute("piva"))
                            OUTPUT_Sa_Cod = CInt(xFabbricato.GetAttribute("sa_cod"))
                            OUTPUT_Fabbricato_Cod = CInt(xFabbricato.GetAttribute("fabbricato_cod"))

                            objFabbricati.Cancella(CStr(xFabbricato.GetAttribute("piva")),
                                                   CInt(xFabbricato.GetAttribute("sa_cod")),
                                                   CInt(Cod_Fabbricato),
                                                   "",
                                                   objParametri)

                            '************************************************
                            '*********** INIZIO LOGGING *******************
                            '************************************************

                            'MEMORIZZO L'INSERIMENTO DELL'OPERAZIONE NELLA TABELLA DI LOG (AGRONICA LOG ANAGRAFE)
                            'è necessario avere Migra >= 505

                            resLog = objAgronicaLogAnagrafeW.Scrivi(CInt(OpeDB_Fabbricato),
                                                                    enum_TipoEntita_Des.Fabbricati,
                                                                    CStr(xFabbricato.GetAttribute("piva")),
                                                                    CStr(xFabbricato.GetAttribute("sa_cod")),
                                                                    CStr(Cod_Fabbricato),
                                                                    Nothing,
                                                                    Nothing,
                                                                    Nothing,
                                                                    NoteLog, enum_Id_Servizio.GiasOnline,
                                                                    objParametri,
                                                                    object_data:=DatiFabbricati)

                            '************************************************
                            '*********** FINE LOGGING *********************
                            '************************************************

                    End Select

                    '-------------------------------------------------------------
                    ' STALLA
                    '-------------------------------------------------------------

                    'Prelevo l'elenco dei dati associati alla stalla
                    xStalle = xFabbricato.GetElementsByTagName("Stalla")

                    i_Stalla = 0

                    Do While i_Stalla < xStalle.Count

                        'Prelevo l'i-esimo codice
                        xStalla = xStalle.Item(i_Stalla)

                        'Prelevo gli attributi della stalla selezionato
                        OpeDB_Stalla = xStalla.GetAttribute("TipoOperazioneDB")

                        objStalla = New AgronicaCoreAnagrafeDAL.Stalla_W

                        'Verifico l'operazione richiesta
                        Select Case OpeDB_Stalla

                            Case "0"    'LEGGI -------------------------------------------------------

                            Case "1"    'SALVA -------------------------------------------------------

                                dummy = objStalla.Scrivi(CStr(xStalla.GetAttribute("piva")),
                                                         CInt(xStalla.GetAttribute("sa_cod")),
                                                         CInt(Cod_Fabbricato),
                                                         CStr(xStalla.GetAttribute("sta_des")),
                                                         CStr(xStalla.GetAttribute("ausl_cod")),
                                                         CDate(xStalla.GetAttribute("dat_costr")),
                                                         CDate(xStalla.GetAttribute("dat_chiu")),
                                                         CStr(xStalla.GetAttribute("cod_fabb")),
                                                         CInt(xStalla.GetAttribute("gen_cod")),
                                                         CInt(xStalla.GetAttribute("spe_cod")),
                                                         CInt(xStalla.GetAttribute("ipro_cod")),
                                                         CStr(xStalla.GetAttribute("x")),
                                                         CStr(xStalla.GetAttribute("y")),
                                                         CStr(xStalla.GetAttribute("BDN_codice_azienda")),
                                                         CStr(xStalla.GetAttribute("BDN_allev_idfiscale")),
                                                         CDate(Now),
                                                         Agro_XML_GetInteger(xStalla, "latitudine", 0),
                                                         Agro_XML_GetInteger(xStalla, "longitudine", 0),
                                                         Agro_XML_GetString(xStalla, "cuaa_proprietario", ""),
                                                         Agro_XML_GetString(xStalla, "denominazione_proprietario", ""),
                                                         Agro_XML_GetString(xStalla, "cuaa_detentore", ""),
                                                         Agro_XML_GetString(xStalla, "denominazione_detentore", ""),
                                                         CDate(xStalla.GetAttribute("validita_inizio")),
                                                         CDate(xStalla.GetAttribute("validita_fine")),
                                                         objParametri)

                            Case "2"    'MODIFICA -------------------------------------------------------

                                objStalla.Modifica(CStr(xStalla.GetAttribute("piva")),
                                                   CInt(xStalla.GetAttribute("sa_cod")),
                                                   CInt(Cod_Fabbricato),
                                                   CStr(xStalla.GetAttribute("sta_des")),
                                                   CStr(xStalla.GetAttribute("ausl_cod")),
                                                   CDate(xStalla.GetAttribute("dat_costr")),
                                                   CDate(xStalla.GetAttribute("dat_chiu")),
                                                   CStr(xStalla.GetAttribute("cod_fabb")),
                                                   CInt(xStalla.GetAttribute("gen_cod")),
                                                   CInt(xStalla.GetAttribute("spe_cod")),
                                                   CInt(xStalla.GetAttribute("ipro_cod")),
                                                   CStr(xStalla.GetAttribute("x")),
                                                   CStr(xStalla.GetAttribute("y")),
                                                   CStr(xStalla.GetAttribute("BDN_codice_azienda")),
                                                   CStr(xStalla.GetAttribute("BDN_allev_idfiscale")),
                                                   CDate(Now),
                                                   Agro_XML_GetInteger(xStalla, "latitudine", 0),
                                                   Agro_XML_GetInteger(xStalla, "longitudine", 0),
                                                   Agro_XML_GetString(xStalla, "cuaa_proprietario", ""),
                                                   Agro_XML_GetString(xStalla, "denominazione_proprietario", ""),
                                                   Agro_XML_GetString(xStalla, "cuaa_detentore", ""),
                                                   Agro_XML_GetString(xStalla, "denominazione_detentore", ""),
                                                   CDate(xStalla.GetAttribute("validita_inizio")),
                                                   CDate(xStalla.GetAttribute("validita_fine")),
                                                   "",
                                                   objParametri)

                            Case "3"    'ELIMINA -------------------------------------------------------

                                objStalla.Cancella(CStr(xStalla.GetAttribute("piva")),
                                                   CInt(xStalla.GetAttribute("sa_cod")),
                                                   CInt(Cod_Fabbricato),
                                                   "",
                                                   objParametri)

                        End Select

                        '-------------------------------------------------------------
                        '    CARATTERISTICHE STALLA 
                        '-------------------------------------------------------------


                        'Prelevo l'elenco dei codici
                        xStallaCaratteristiche = xStalla.GetElementsByTagName("Stalla_Caratteristica")

                        i_StallaCaratteristica = 0

                        Do While i_StallaCaratteristica < xStallaCaratteristiche.Count

                            'Prelevo l'i-esimo codice
                            xStallaCaratteristica = xStallaCaratteristiche.Item(i_StallaCaratteristica)

                            'Prelevo gli attributi del codice selezionato
                            OpeDB_StallaCaratteristica = xStallaCaratteristica.GetAttribute("TipoOperazioneDB")

                            objStallaxCaratteristiche = New AgronicaCoreAnagrafeDAL.Stalla_Caratteristiche_W


                            'Verifico l'operazione richiesta
                            Select Case OpeDB_StallaCaratteristica

                                Case "0"    'LEGGI -------------------------------------------------------

                                Case "1"    'SALVA -------------------------------------------------------

                                    'Se entro in modifica e inserisco un nuovo codice ->
                                    'il fabbricato_cod si trova nella stringa Xml
                                    If OpeDB_StallaCaratteristica = 2 Then
                                        Cod_StallaCaratteristica = CInt(xStalla.GetAttribute("fabbricato_cod"))
                                    End If


                                    dummy = objStallaxCaratteristiche.Scrivi(
                                                CStr(xStalla.GetAttribute("piva")),
                                                CInt(xStalla.GetAttribute("sa_cod")),
                                                CInt(xStalla.GetAttribute("sta_num")),
                                                CStr(xStalla.GetAttribute("cod_fabb")),
                                                CInt(xStallaCaratteristica.GetAttribute("att_cod")),
                                                CStr(xStallaCaratteristica.GetAttribute("valore")),
                                                    CDate(xStallaCaratteristica.GetAttribute("validita_inizio")),
                                                    CDate(xStallaCaratteristica.GetAttribute("validita_fine")),
                                                        objParametri)

                                Case "2"    'MODIFICA -------------------------------------------------------

                                    objStallaxCaratteristiche.Modifica(
                                                CStr(xStalla.GetAttribute("piva")),
                                                CInt(xStalla.GetAttribute("sa_cod")),
                                                CInt(xStalla.GetAttribute("sta_num")),
                                                CStr(xStalla.GetAttribute("cod_fabb")),
                                                CInt(xStallaCaratteristica.GetAttribute("att_cod")),
                                                CStr(xStallaCaratteristica.GetAttribute("valore")),
                                                    CDate(xStallaCaratteristica.GetAttribute("validita_inizio")),
                                                    CDate(xStallaCaratteristica.GetAttribute("validita_fine")),
                                                        "",
                                                        objParametri)

                                Case "3"    'ELIMINA -------------------------------------------------------

                                    objStallaxCaratteristiche.Cancella(CStr(xStalla.GetAttribute("piva")),
                                                                       CInt(xStalla.GetAttribute("sa_cod")),
                                                                       CInt(xStalla.GetAttribute("sta_num")),
                                                                       CStr(xStalla.GetAttribute("cod_fabb")),
                                                                       CInt(xStallaCaratteristica.GetAttribute("att_cod")),
                                                                       "",
                                                                       objParametri)

                            End Select

                            'Elimino l'oggetto
                            objStallaxCaratteristiche = Nothing

                            'Incremento l'indice
                            i_StallaCaratteristica += 1

                        Loop


                        xStallaConfigurazioniBDN = xStalla.GetElementsByTagName("Stalla_Configurazione")

                        i_StallaConfigurazioneBDN = 0

                        Do While i_StallaConfigurazioneBDN < xStallaConfigurazioniBDN.Count

                            'Prelevo l'i-esimo codice
                            xStallaConfigurazioneBDN = xStallaConfigurazioniBDN.Item(i_StallaConfigurazioneBDN)

                            'Prelevo gli attributi del codice selezionato
                            OpeDB_StallaConfigurazioneBDN = xStallaConfigurazioneBDN.GetAttribute("TipoOperazioneDB")

                            ObjStallaConfigurazioniBDN = New AgronicaCoreAnagrafeDAL.Stalla_Configurazioni_BDN_W

                            Dim id As Integer = xStallaConfigurazioneBDN.GetAttribute("id")
                            Dim cf_detentore As String = xStallaConfigurazioneBDN.GetAttribute("cf_detentore")
                            Dim cf_proprietario As String = xStallaConfigurazioneBDN.GetAttribute("cf_proprietario")
                            Dim ragsoc_detentore As String = xStallaConfigurazioneBDN.GetAttribute("ragsoc_detentore")
                            Dim ragsoc_proprietario As String = xStallaConfigurazioneBDN.GetAttribute("ragsoc_proprietario")

                            Dim validita_inizio As Date = CDate(xStallaConfigurazioneBDN.GetAttribute("validita_inizio"))
                            Dim validita_fine As Date = CDate(xStallaConfigurazioneBDN.GetAttribute("validita_fine"))

                            'Verifico l'operazione richiesta
                            Select Case OpeDB_StallaConfigurazioneBDN

                                Case "0"    'LEGGI -------------------------------------------------------

                                Case "1"    'SALVA -------------------------------------------------------

                                    'Se entro in modifica e inserisco un nuovo codice ->
                                    'il fabbricato_cod si trova nella stringa Xml
                                    If id = 0 Then
                                        Dim objSequenzaTabelle As New Agro_Sequenze
                                        id = objSequenzaTabelle.NuovoId_Tabella("Stalla_Configurazioni_BDN", 0, 2000000, objParametri)
                                    End If

                                    dummy = ObjStallaConfigurazioniBDN.Scrivi(id,
                                                OUTPUT_Piva,
                                                OUTPUT_Sa_Cod,
                                                OUTPUT_Fabbricato_Cod,
                                                cf_detentore,
                                                cf_proprietario,
                                                ragsoc_detentore,
                                                ragsoc_proprietario,
                                                validita_inizio,
                                                validita_fine,
                                                objParametri)

                                Case "2"    'MODIFICA -------------------------------------------------------

                                    'ObjStallaConfigurazioniBDN.Modifica(
                                    '            CStr(xStalla.GetAttribute("piva")),
                                    '            CInt(xStalla.GetAttribute("sa_cod")),
                                    '            CInt(xStalla.GetAttribute("sta_num")),
                                    '            CStr(xStalla.GetAttribute("cod_fabb")),
                                    '            CInt(xStallaCaratteristica.GetAttribute("att_cod")),
                                    '            CStr(xStallaCaratteristica.GetAttribute("valore")),
                                    '                CDate(xStallaCaratteristica.GetAttribute("validita_inizio")),
                                    '                CDate(xStallaCaratteristica.GetAttribute("validita_fine")),
                                    '                    "",
                                    '                    objParametri)

                                Case "3"    'ELIMINA -------------------------------------------------------

                                    ObjStallaConfigurazioniBDN.Cancella(id,
                                                                        OUTPUT_Piva,
                                                                        OUTPUT_Sa_Cod,
                                                                        OUTPUT_Fabbricato_Cod,
                                                                       "",
                                                                       objParametri)

                            End Select

                            'Elimino l'oggetto
                            ObjStallaConfigurazioniBDN = Nothing

                            'Incremento l'indice
                            i_StallaConfigurazioneBDN += 1

                        Loop

                        'Elimino l'oggetto
                        objStalla = Nothing

                        'Incremento l'indice
                        i_Stalla += 1

                    Loop

                    '-------------------------------------------------------------
                    ' CODICI
                    '-------------------------------------------------------------

                    objFabbricatoxCodici = New AgronicaCoreAnagrafeDAL.Fabbricati_Codici_W

                    ' se G2G cancello eventuali codici presenti
                    If TipoG2G <> 0 AndAlso OpeDB_Fabbricato = 2 Then
                        objFabbricatoxCodici.Cancella(CStr(xFabbricato.GetAttribute("piva")),
                                                      CInt(xFabbricato.GetAttribute("sa_cod")),
                                                      CInt(xFabbricato.GetAttribute("fabbricato_cod")),
                                                      0, "", objParametri)
                    End If

                    'Prelevo l'elenco dei codici
                    xCodici = xFabbricato.GetElementsByTagName("CodiceFabbricato")

                    i_Codice = 0

                    Do While i_Codice < xCodici.Count

                        'Prelevo l'i-esimo codice
                        xCodice = xCodici.Item(i_Codice)

                        'Prelevo gli attributi del codice selezionato (se G2G forzo inserimento)
                        OpeDB_Codice = If(TipoG2G = 0, xCodice.GetAttribute("TipoOperazioneDB"), "1")

                        'Verifico l'operazione richiesta
                        Select Case OpeDB_Codice

                            Case "0"    'LEGGI -------------------------------------------------------

                            Case "1"    'SALVA -------------------------------------------------------

                                'Se entro in modifica e inserisco un nuovo codice ->
                                'il fabbricato_cod si trova nella stringa Xml
                                If OpeDB_Fabbricato = 2 Then
                                    Cod_Fabbricato = CInt(xFabbricato.GetAttribute("fabbricato_cod"))
                                End If

                                Dim objFabbricato_R As New Fabbricati_Codici_R
                                Dim dtCodiciAnagrafe = objFabbricato_R.Leggi(CStr(xFabbricato.GetAttribute("piva")),
                                                      CInt(xFabbricato.GetAttribute("sa_cod")),
                                                      CInt(Cod_Fabbricato),
                                                      CInt(xCodice.GetAttribute("id_cod")),
                                                      CStr(xCodice.GetAttribute("val_cod")),
                                                      enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                      "", "", objParametri)

                                If Not IsNothing(dtCodiciAnagrafe) AndAlso dtCodiciAnagrafe.Rows.Count = 0 Then
                                    dummy = objFabbricatoxCodici.Scrivi(CStr(xFabbricato.GetAttribute("piva")),
                                                                        CInt(xFabbricato.GetAttribute("sa_cod")),
                                                                        CInt(Cod_Fabbricato),
                                                                        CInt(xCodice.GetAttribute("id_cod")),
                                                                        CDate(xCodice.GetAttribute("validita_inizio")),
                                                                        CDate(xCodice.GetAttribute("validita_fine")),
                                                                        CStr(xCodice.GetAttribute("val_cod")),
                                                                        objParametri)
                                End If

                            Case "2"    'MODIFICA -------------------------------------------------------

                                objFabbricatoxCodici.Modifica(CStr(xFabbricato.GetAttribute("piva")),
                                                              CInt(xFabbricato.GetAttribute("sa_cod")),
                                                              CInt(xFabbricato.GetAttribute("fabbricato_cod")),
                                                              CInt(xCodice.GetAttribute("id_cod")),
                                                              CStr(xCodice.GetAttribute("val_cod")),
                                                              CDate(xCodice.GetAttribute("validita_inizio")),
                                                              CDate(xCodice.GetAttribute("validita_fine")),
                                                              "",
                                                              objParametri)

                            Case "3"    'ELIMINA -------------------------------------------------------

                                objFabbricatoxCodici.Cancella(CStr(xFabbricato.GetAttribute("piva")),
                                                              CInt(xFabbricato.GetAttribute("sa_cod")),
                                                              CInt(xFabbricato.GetAttribute("fabbricato_cod")),
                                                              CInt(xCodice.GetAttribute("id_cod")),
                                                              "",
                                                              objParametri)

                        End Select

                        'Incremento l'indice
                        i_Codice += 1

                    Loop

                    'Elimino l'oggetto
                    objFabbricatoxCodici = Nothing

                    '-------------------------------------------------------------
                    ' FABBRICATI IMPOSTAZIONI
                    '-------------------------------------------------------------

                    'Prelevo l'elenco delle impostazioni
                    xDatiImpostazioni = xFabbricato.GetElementsByTagName("DatiFabbricato_Impostazioni")

                    i_DatiImpostazioni = 0

                    Do While i_DatiImpostazioni < xDatiImpostazioni.Count

                        ObjFabbricati_Impostazioni = New AgronicaCoreAnagrafeDAL.Fabbricati_Impostaz_W

                        'Prelevo l'i-esimo Blocco Impostazione
                        xDatiImpostazione = xDatiImpostazioni.Item(i_DatiImpostazioni)

                        'Prelevo l'elenco delle impostazioni
                        xImpostazioni = xFabbricato.GetElementsByTagName("Fabbricato_Impostazione")

                        i_Impostazione = 0

                        bFirst = True

                        Do While i_Impostazione < xImpostazioni.Count

                            'Prelevo l'i-esimo Impostazione
                            xImpostazione = xImpostazioni.Item(i_Impostazione)

                            'Prelevo gli attributi dell'impostazione selezionato
                            OpeDB_Impostazione = xImpostazione.GetAttribute("TipoOperazioneDB")


                            'Verifico l'operazione richiesta
                            Select Case OpeDB_Impostazione

                                Case "0"    'LEGGI -------------------------------------------------------

                                Case "1"    'SALVA -------------------------------------------------------

                                    'Se entro in modifica e inserisco un nuovo Impostazione ->
                                    'il fabbricato_cod si trova nella stringa Xml
                                    If OpeDB_Fabbricato = 2 Then
                                        Cod_Fabbricato = CInt(xFabbricato.GetAttribute("fabbricato_cod"))

                                        '=======================================================================================================
                                        'Cancellazione Preventiva delle Impostazioni
                                        '(la cancellazione viene fatta solo adesso perché il superuser è presente solo in questa tabella
                                        If bFirst Then

                                            ObjFabbricati_Impostazioni.Cancella(CStr(xImpostazione.GetAttribute("piva_superuser")),
                                                                                CStr(xFabbricato.GetAttribute("piva")),
                                                                                CInt(xFabbricato.GetAttribute("sa_cod")),
                                                                                Cod_Fabbricato,
                                                                                0,
                                                                                0,
                                                                                0,
                                                                                "",
                                                                                objParametri)

                                            bFirst = False

                                        End If
                                        '=======================================================================================================

                                    End If

                                    dummy = ObjFabbricati_Impostazioni.Scrivi(CStr(xImpostazione.GetAttribute("piva_superuser")),
                                                                              CStr(xFabbricato.GetAttribute("piva")),
                                                                              CInt(xFabbricato.GetAttribute("sa_cod")),
                                                                              CInt(Cod_Fabbricato),
                                                                              CInt(xImpostazione.GetAttribute("elem_cod")),
                                                                              CInt(xImpostazione.GetAttribute("gru_cod")),
                                                                              CInt(xImpostazione.GetAttribute("lav_cod")),
                                                                              CDate(xImpostazione.GetAttribute("validita_inizio")),
                                                                              CDate(xImpostazione.GetAttribute("validita_fine")),
                                                                              objParametri)

                                Case "2"    'MODIFICA -------------------------------------------------------

                                Case "3"    'ELIMINA -------------------------------------------------------

                                    ObjFabbricati_Impostazioni.Cancella(CStr(xImpostazione.GetAttribute("piva_superuser")),
                                                                        CStr(xFabbricato.GetAttribute("piva")),
                                                                        CInt(xFabbricato.GetAttribute("sa_cod")),
                                                                        CInt(xFabbricato.GetAttribute("fabbricato_cod")),
                                                                        CInt(xImpostazione.GetAttribute("elem_cod")),
                                                                        CInt(xImpostazione.GetAttribute("gru_cod")),
                                                                        CInt(xImpostazione.GetAttribute("lav_cod")),
                                                                        "",
                                                                        objParametri)

                            End Select

                            'Incremento l'indice
                            i_Impostazione += 1

                        Loop

                        'Elimino l'oggetto
                        ObjFabbricati_Impostazioni = Nothing

                        'Incremento l'indice
                        i_DatiImpostazioni += 1

                    Loop

                    'Elimino l'oggetto
                    objFabbricati = Nothing

                    'Incremento l'indice
                    i_Fabbricato += 1

                Loop

                '-------------------------------------------------------------
                ' VASCA ENOLOGICA
                '-------------------------------------------------------------

                'Prelevo l'elenco dei dati associati alla Vasca
                xVasche = xDatiFabbricato.GetElementsByTagName("Vasca")

                i_Vasca = 0

                Do While i_Vasca < xVasche.Count

                    'Prelevo l'i-esimo codice
                    xVasca = xVasche.Item(i_Vasca)

                    'Prelevo gli attributi della Vasca selezionato
                    OpeDB_Vasca = xVasca.GetAttribute("TipoOperazioneDB")

                    'Inizializzo Preventivamente il Codice Vasca
                    Vas_Cod = CStr(xVasca.GetAttribute("vas_cod"))

                    objVasca = New AgronicaCoreContabDAL.Vasche_W

                    'Verifico l'operazione richiesta
                    Select Case OpeDB_Vasca

                        Case "0"    'LEGGI -------------------------------------------------------

                        Case "1"    'SALVA -------------------------------------------------------

                            If Vas_Cod <= 0 Then

                                'Richiedo un nuovo codice vasca
                                Vas_Cod = objSequenze.NuovoId_xPiva_xSaCod("SeqMagazzino",
                                                                           "Mag_Cod",
                                                                           CStr(xVasca.GetAttribute("piva")),
                                                                           CInt(xVasca.GetAttribute("sa_cod")),
                                                                           CInt(xVasca.GetAttribute("basecode")),
                                                                           CInt(xVasca.GetAttribute("topcode")),
                                                                           objParametri)

                            Else
                                'Esportazione in Locale
                            End If

                            dummy = objVasca.Scrivi(CStr(xVasca.GetAttribute("piva")),
                                                    CInt(xVasca.GetAttribute("sa_cod")),
                                                    CInt(Vas_Cod),
                                                    CInt(xVasca.GetAttribute("piano_cod")),
                                                    CStr(xVasca.GetAttribute("identificativo")),
                                                    CStr(xVasca.GetAttribute("numero_serie")),
                                                    CStr(xVasca.GetAttribute("modello")),
                                                    CInt(xVasca.GetAttribute("materiale_cod")),
                                                    CInt(xVasca.GetAttribute("appoggio_cod")),
                                                    CInt(xVasca.GetAttribute("inclinato")),
                                                    CInt(xVasca.GetAttribute("refrigerata")),
                                                    CInt(xVasca.GetAttribute("tipo_tasca")),
                                                    CInt(xVasca.GetAttribute("coibentata")),
                                                    CInt(xVasca.GetAttribute("udm_cod_capacita")),
                                                    CDbl(xVasca.GetAttribute("capacita_nominale")),
                                                    CDbl(xVasca.GetAttribute("capacita_effettiva")),
                                                    CInt(xVasca.GetAttribute("udm_cod_altezza")),
                                                    CDbl(xVasca.GetAttribute("altezza_cilindro")),
                                                    CDbl(xVasca.GetAttribute("altezza_totale")),
                                                    CInt(xVasca.GetAttribute("udm_cod_peso")),
                                                    CDbl(xVasca.GetAttribute("peso")),
                                                    CInt(xVasca.GetAttribute("dimx")),
                                                    CInt(xVasca.GetAttribute("dimy")),
                                                    CDbl(xVasca.GetAttribute("rotazione")),
                                                    CDbl(xVasca.GetAttribute("costo_acquisto")),
                                                    CDbl(xVasca.GetAttribute("ammortamento")),
                                                    CDate(xVasca.GetAttribute("ultima_revisione")),
                                                    CStr(xVasca.GetAttribute("note")),
                                                    CStr(xVasca.GetAttribute("tipo")),
                                                    CInt(xVasca.GetAttribute("spessore")),
                                                    CInt(xVasca.GetAttribute("colore_esterno")),
                                                    CInt(xVasca.GetAttribute("posx")),
                                                    CInt(xVasca.GetAttribute("posy")),
                                                    CDate(xVasca.GetAttribute("validita_inizio")),
                                                    CDate(xVasca.GetAttribute("validita_fine")),
                                                    objParametri)

                        Case "2"    'MODIFICA -------------------------------------------------------

                            objVasca.Modifica(CStr(xVasca.GetAttribute("piva")),
                                              CInt(xVasca.GetAttribute("sa_cod")),
                                              CInt(Vas_Cod),
                                              CInt(xVasca.GetAttribute("piano_cod")),
                                              CStr(xVasca.GetAttribute("identificativo")),
                                              CStr(xVasca.GetAttribute("numero_serie")),
                                              CStr(xVasca.GetAttribute("modello")),
                                              CInt(xVasca.GetAttribute("materiale_cod")),
                                              CInt(xVasca.GetAttribute("appoggio_cod")),
                                              CInt(xVasca.GetAttribute("inclinato")),
                                              CInt(xVasca.GetAttribute("refrigerata")),
                                              CInt(xVasca.GetAttribute("tipo_tasca")),
                                              CInt(xVasca.GetAttribute("coibentata")),
                                              CInt(xVasca.GetAttribute("udm_cod_capacita")),
                                              CDbl(xVasca.GetAttribute("capacita_nominale")),
                                              CDbl(xVasca.GetAttribute("capacita_effettiva")),
                                              CInt(xVasca.GetAttribute("udm_cod_altezza")),
                                              CDbl(xVasca.GetAttribute("altezza_cilindro")),
                                              CDbl(xVasca.GetAttribute("altezza_totale")),
                                              CInt(xVasca.GetAttribute("udm_cod_peso")),
                                              CDbl(xVasca.GetAttribute("peso")),
                                              CInt(xVasca.GetAttribute("dimx")),
                                              CInt(xVasca.GetAttribute("dimy")),
                                              CDbl(xVasca.GetAttribute("rotazione")),
                                              CDbl(xVasca.GetAttribute("costo_acquisto")),
                                              CDbl(xVasca.GetAttribute("ammortamento")),
                                              CDate(xVasca.GetAttribute("ultima_revisione")),
                                              CStr(xVasca.GetAttribute("note")),
                                              CStr(xVasca.GetAttribute("tipo")),
                                              CInt(xVasca.GetAttribute("spessore")),
                                              CInt(xVasca.GetAttribute("colore_esterno")),
                                              CInt(xVasca.GetAttribute("posx")),
                                              CInt(xVasca.GetAttribute("posy")),
                                              CDate(xVasca.GetAttribute("validita_inizio")),
                                              CDate(xVasca.GetAttribute("validita_fine")),
                                              "",
                                              objParametri)

                        Case "3"    'ELIMINA -------------------------------------------------------

                            objVasca.Cancella(CStr(xVasca.GetAttribute("piva")),
                                              CInt(xVasca.GetAttribute("sa_cod")),
                                              CInt(Vas_Cod),
                                              "",
                                              objParametri)

                    End Select


                    'Elimino l'oggetto
                    objVasca = Nothing

                    'Incremento l'indice
                    i_Vasca += 1

                Loop

                '------------------------------

                'Incremento l'indice
                i_DatiFabbricati += 1

            Loop

            '------------------------------
            '------------------------------
            '------------------------------

            'Elimino tutti gli oggetti utilizzati

            objSequenze = Nothing
            xmlDoc = Nothing

            xDatiFabbricati = Nothing
            xDatiFabbricato = Nothing
            xFabbricati = Nothing
            xFabbricato = Nothing

            '------------------------------

            xRisp = True

            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

            '----------------------------------------------------------------------------

        Catch ex As Exception

            xRisp = False

            'Faccio il rollback della transazione
            If objParametri.objTransazione IsNot Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            '//////////////////////////////////////////////////////////////////////
            messaggioErrore = "(Piva=" & OUTPUT_Piva & ")" &
                              "(Sa_Cod=" & CStr(OUTPUT_Sa_Cod) & ")" &
                              "(Fabbricato_cod=" & CStr(OUTPUT_Fabbricato_Cod) & ")" &
                              " : " & ex.Message
            '//////////////////////////////////////////////////////////////////////

            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        Finally
            'Chiudo la connessione se è stata aperta in questa routine
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)
        End Try

        Return xRisp

    End Function

    Public Function ScriviMagazzini(ByRef magazzini As List(Of Fabbricato), ByRef objParametri As AgronicaCoreParametri, ByRef objParametri_Utenti As AgronicaCoreParametri) As String

        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.Fabbricato_W.ScriviMagazziniAPP()"

        Dim objSequenze As New Agro_Sequenze
        Dim objFabbricatiR As New Fabbricati_R
        Dim objFabbricatiW As New Fabbricati_W
        Dim objIndirizziR As New Indirizzi_Read
        Dim objIndirizziW As New Indirizzi_Write
        Dim objFabbricatiCodiciR As New Fabbricati_Codici_R
        Dim objFabbricatiCodiciW As New Fabbricati_Codici_W
        Dim objComuni As New AgronicaCoreMetaSchemaDAL.Istat_R
        Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_CodiciGiasPRO_R

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False
        Dim messaggioErrore As String = ""
        Dim response As New List(Of String)
        Dim BaseCode As Integer
        Dim TopCode As Integer

        Try

            Dim Progressivo_Gias = objUtenti.ProgressivoGias_from_Superuser(objParametri_Utenti)
            UtilityProvider.Calcola_BaseCode_TopCode(BaseCode, TopCode, Progressivo_Gias)

            ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, objParametri)

            For Each magazzino As Fabbricato In magazzini

                Dim codice As Integer = magazzino.primaryKey.codice
                Dim centroPK = magazzino.primaryKey.centroAziendalePK

                If magazzino.flag_cancellazione Then

                    codice = Math.Abs(codice)
                    Dim indirizzo = magazzino.indirizzo
                    If indirizzo IsNot Nothing AndAlso indirizzo.codice <> 0 Then
                        objIndirizziW.Cancella(indirizzo.codice, "", objParametri)
                    End If

                    objFabbricatiCodiciW.Cancella(centroPK.partitaIva, centroPK.codice, codice, enum_CodiciAnagrafe.Visibile_da_App, "", objParametri)
                    objFabbricatiW.Cancella(centroPK.partitaIva, centroPK.codice, codice, "", objParametri)

                ElseIf codice <= 0 Then

                    Dim codIndirizzo As Integer = 0
                    Dim indirizzo = magazzino.indirizzo

                    If indirizzo IsNot Nothing Then

                        If indirizzo.istatComune.com = "000" AndAlso Not String.IsNullOrEmpty(indirizzo.cap) Then
                            Dim istat = objComuni.Leggi("", "", "", indirizzo.cap, "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)
                            If istat IsNot Nothing AndAlso istat.Rows.Count > 0 Then
                                indirizzo.istatComune.prov = CStr(istat.Rows(0).Item("PROV"))
                                indirizzo.istatComune.com = CStr(istat.Rows(0).Item("COM"))
                                indirizzo.istatComune.localita = istat.Rows(0).Item("LOCALITA")
                                indirizzo.istatComune.comuni_prov = istat.Rows(0).Item("COMUNI_PROV")
                            End If
                        End If

                        codIndirizzo = objSequenze.NuovoId_Tabella("Indirizzi", 0, UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri)
                        objIndirizziW.Scrivi(codIndirizzo, indirizzo.via, indirizzo.frazione, indirizzo.cap,
                            indirizzo.istatComune.localita, indirizzo.istatComune.comuni_prov,
                            indirizzo.stato.codice, indirizzo.note,
                            indirizzo.istatComune.prov, indirizzo.istatComune.com,
                            AGRODATAINIZIO, AGRODATAFINE, objParametri)
                    End If

                    codice = objSequenze.NuovoId_xPiva_xSaCod("SeqMagazzino", "Mag_Cod", centroPK.partitaIva, centroPK.codice, BaseCode, TopCode, objParametri)
                    objFabbricatiW.Scrivi(centroPK.partitaIva, centroPK.codice, codice, magazzino.descrizione, codIndirizzo, magazzino.tipo,
                        "000", "000", "", 0, 0, "", 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                        AGRODATAINIZIO, AGRODATAINIZIO, 0, 0, 0, 0, 0, 0, 0, 0, "", AGRODATAINIZIO, 0,
                        AGRODATAINIZIO, AGRODATAFINE, objParametri)

                    objFabbricatiCodiciW.Scrivi(centroPK.partitaIva, centroPK.codice, codice, enum_CodiciAnagrafe.Visibile_da_App, AGRODATAINIZIO, AGRODATAFINE, "1", objParametri)

                ElseIf codice > 0 Then

                    objFabbricatiW.Modifica2(centroPK.partitaIva, centroPK.codice, codice, magazzino.descrizione, AGRODATAINIZIO, AGRODATAFINE, objParametri)

                    If magazzino.indirizzo IsNot Nothing Then

                        Dim indirizzo = magazzino.indirizzo

                        If indirizzo.istatComune.com = "000" AndAlso Not String.IsNullOrEmpty(indirizzo.cap) Then
                            Dim istat = objComuni.Leggi("", "", "", indirizzo.cap, "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)
                            If istat IsNot Nothing AndAlso istat.Rows.Count > 0 Then
                                indirizzo.istatComune.prov = CStr(istat.Rows(0).Item("PROV"))
                                indirizzo.istatComune.com = CStr(istat.Rows(0).Item("COM"))
                                indirizzo.istatComune.localita = istat.Rows(0).Item("LOCALITA")
                                indirizzo.istatComune.comuni_prov = istat.Rows(0).Item("COMUNI_PROV")
                            End If
                        End If

                        If indirizzo.codice = 0 Then
                            indirizzo.codice = objSequenze.NuovoId_Tabella("Indirizzi", 0, UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri)
                        End If

                        objIndirizziW.Modifica(indirizzo.codice, indirizzo.via, indirizzo.frazione, indirizzo.cap,
                            indirizzo.istatComune.localita, indirizzo.istatComune.comuni_prov,
                            indirizzo.stato.codice, indirizzo.note,
                            indirizzo.istatComune.prov, indirizzo.istatComune.com,
                            AGRODATAINIZIO, AGRODATAFINE, "", objParametri)

                    End If

                End If

                response.Add(codice)

            Next

            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

        Catch ex As Exception

            'Faccio il rollback della transazione
            If objParametri.objTransazione IsNot Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        Finally
            'Chiudo la connessione se è stata aperta in questa routine
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)
        End Try

        Return String.Join("|", response)

    End Function

    Public Function Scrivi_Fabbricato_Anagrafica(ByRef objFabbricato As AgronicaCoreModelsSTD.anagrafiche.Fabbricato,
                                                 ByRef objParametri_Server As AgronicaCoreParametri,
                                                 ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                 Optional NoteLog As String = NOTELOG_ANAGRAFE_NG,
                                                 Optional SistemaOrigine As Integer = -1) As Integer

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeBIZ.CentroAziendale_W.Scrivi_Fabbricato_Anagrafica()"
        Dim messaggioErrore As String = ""

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)

        Dim Piva As String = ""
        Dim Sa_Cod As Integer
        Dim Fabbricato_Cod As Integer

        Try
            Dim scopeOption As New TransactionScopeOption
            Dim transactionOptions As New TransactionOptions
            transactionOptions.IsolationLevel = IsolationLevel.ReadUncommitted
            Using scope As New TransactionScope(scopeOption, transactionOptions)
                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    Piva = objFabbricato.primaryKey.centroAziendalePK.partitaIva
                    Sa_Cod = objFabbricato.primaryKey.centroAziendalePK.codice
                    Dim Tipo_Operazione = "0" ' 0: Non fare niente - 1: Creazione nuovo Centro - 2: Modifica Centro

                    If Not objFabbricato.flag_cancellazione Then

                        If objFabbricato.primaryKey.codice = 0 Then
                            Tipo_Operazione = "1"
                        Else
                            Tipo_Operazione = "2"
                            Fabbricato_Cod = objFabbricato.primaryKey.codice
                        End If

                        Dim Fabbricato As AgronicaCoreEntityFramework_POCO.Fabbricati = Nothing

                        If Tipo_Operazione = "1" Then
                            Dim Centro = New AgronicaCoreEntityFramework_POCO.Centri_Aziendali()
                            Centro.PIVA = Piva
                            Centro.sa_cod = Sa_Cod
                            Fabbricato = AgronicaCoreAnagrafeDAL.EFFabbricati.CreateFabbricato(GiasContext, objParametri_Server, objParametri_Utenti, Centro, objParametri_Server.UsernameOperazione)
                            Piva = Centro.PIVA
                            Sa_Cod = Centro.sa_cod
                            Fabbricato_Cod = Fabbricato.Fabbricato_Cod
                        Else

                            Fabbricato = (From cent In GiasContext.Fabbricati
                                          Where cent.PIVA = Piva AndAlso
                                                cent.SA_COD = Sa_Cod AndAlso
                                                cent.Fabbricato_Cod = Fabbricato_Cod).FirstOrDefault()
                        End If

                        If Fabbricato Is Nothing Then
                            Throw New Exception("Fabbricato non trovato")
                        End If


                        Fabbricato.Fabbricato_Des = objFabbricato.descrizione
                        Fabbricato.Tipo_Fabbricato_Cod = 20
                        If objFabbricato.tipo > 0 Then
                            Fabbricato.Tipo_Fabbricato_Cod = objFabbricato.tipo
                        End If

                        Fabbricato.MC_Convenzionale = objFabbricato.volumeConvenzionale

                        If objFabbricato.validita.inizio < AGRODATAINIZIO OrElse objFabbricato.validita.inizio > AGRODATAFINE Then
                            objFabbricato.validita.inizio = AGRODATAINIZIO
                        End If
                        If objFabbricato.validita.fine < AGRODATAINIZIO OrElse objFabbricato.validita.fine > AGRODATAFINE Then
                            objFabbricato.validita.fine = AGRODATAFINE
                        End If

                        Fabbricato.Validita_Inizio = objFabbricato.validita.inizio
                        Fabbricato.Validita_Fine = objFabbricato.validita.fine

                        ' ---------- CODICI --------------- '
                        If Tipo_Operazione = "1" Then
                            Scrivi_Codice_Fabbricato(enum_CodiciAnagrafe.Visibile_da_App, "1",
                                                     GiasContext, Fabbricato, objParametri_Server, objParametri_Utenti)
                        End If


                        ' ---------- INDIRIZZO -------------- '
                        Dim indirizzoBIZ As New Indirizzi_W
                        If (objFabbricato.indirizzo IsNot Nothing) Then

                            Fabbricato.Indirizzo_Cod = indirizzoBIZ.Scrivi_Indirizzo_Fabbricato(objFabbricato.indirizzo, GiasContext, objParametri_Server, objParametri_Utenti, False)
                        End If

                        ' ---------- PARTICELLA -------------- '
                        If objFabbricato.particella IsNot Nothing AndAlso objFabbricato.particella.primaryKey IsNot Nothing Then
                            Fabbricato.PROV = objFabbricato.particella.primaryKey.Prov
                            Fabbricato.COM = objFabbricato.particella.primaryKey.Com
                            Fabbricato.SEZIONE = If(objFabbricato.particella.primaryKey.Sezione = "", 0, objFabbricato.particella.primaryKey.Sezione)
                            Fabbricato.FOGLIO = objFabbricato.particella.primaryKey.Foglio
                            Fabbricato.NUMERO = objFabbricato.particella.primaryKey.Numero
                            Fabbricato.SUBALTERNO = If(objFabbricato.particella.primaryKey.Subalterno = "", 0, objFabbricato.particella.primaryKey.Subalterno)

                            ''verifico che non esiste già la particella
                            Dim objCatastoBIZ_R As New AgronicaCoreAnagrafeBIZ.Particella_R
                            Dim objCatastoBIZ_W As New AgronicaCoreAnagrafeBIZ.Particella_W

                            Dim particellaNew As New AgronicaCoreModelsSTD.anagrafiche.CatastoCentroAziendale()
                            particellaNew.centro = New CentroAziendale.PK(Sa_Cod, Piva)
                            particellaNew.particella = objFabbricato.particella
                            particellaNew.possessiParticella = New List(Of PossessoParticella)

                            Dim particellaOld As AgronicaCoreModelsSTD.anagrafiche.CatastoCentroAziendale = objCatastoBIZ_R.Leggi_Particella_Anagrafica(Piva,
                                                                                                                                                        Sa_Cod,
                                                                                                                                                        objFabbricato.particella.primaryKey.Prov,
                                                                                                                                                        objFabbricato.particella.primaryKey.Com,
                                                                                                                                                        objFabbricato.particella.primaryKey.Sezione,
                                                                                                                                                        objFabbricato.particella.primaryKey.Foglio,
                                                                                                                                                        objFabbricato.particella.primaryKey.Numero,
                                                                                                                                                        objFabbricato.particella.primaryKey.Subalterno,
                                                                                                                                                        False,
                                                                                                                                                        False,
                                                                                                                                                        False,
                                                                                                                                                        False,
                                                                                                                                                        objParametri_Server
                                                                                                                                                        )

                            If particellaOld.centro Is Nothing Then
                                'significa che la particella o non esiste oppure non esiste nella ImpreseXParticelle, in caso contrario esiste già
                                Dim possesso As New PossessoParticella()
                                possesso.titolo_Di_Possesso = New AgronicaCoreModelsSTD.metaschema.TitoloDiPossesso
                                possesso.validita = New IntervalloTemporale(AGRODATAINIZIO, AGRODATAFINE)
                                possesso.Area = 1
                                particellaNew.possessiParticella.Add(possesso)
                                objCatastoBIZ_W.ParticellaCatasto_Scrivi(particellaNew, Nothing, objParametri_Server, objParametri_Utenti, False)
                            End If

                        End If

                        Dim DatiFabbricatoStr = ""
                        If Fabbricato IsNot Nothing Then
                            Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
                            DatiFabbricatoStr = JsonConvert.SerializeObject(objFabbricato, a)
                        End If

                        'Scrittura tabella Agronica_Log_Anagrafe
                        Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
                        Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.Fabbricati,
                                                                                                CStr(Fabbricato.PIVA), CStr(Fabbricato.SA_COD),
                                                                                                CStr(Fabbricato.Fabbricato_Cod),
                                                                                                Nothing, Nothing, Nothing,
                                                                                                Tipo_Operazione,
                                                                                                objParametri_Server, enum_Id_Servizio.GiasOnline,
                                                                                                NoteLog, DatiFabbricatoStr,
                                                                                                Origine:=SistemaOrigine)
                        GiasContext.Agronica_Log_Anagrafe.Add(log)
                        GiasContext.SaveChanges()

                    Else

                        EliminaFabbricato(objFabbricato, GiasContext, objParametri_Server, objParametri_Utenti, NoteLog:=NoteLog)

                    End If

                    GiasContext.SaveChanges()
                    scope.Complete()
                    scope.Dispose()
                End Using
            End Using
        Catch ex As GiasException
            messaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return Fabbricato_Cod

    End Function

    Public Shared Function controllo_MovimentiMagazzinoxEliminazione(piva As String,
                                                                     sa_cod As Integer,
                                                                     fabbricato_cod As Integer,
                                                                     ByRef objParametri As AgronicaCoreParametri)

        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.Fabbricato_W.controllo_MovimentiMagazzinoxEliminazione()"
        Dim messaggioErrore As String = ""
        Dim dtAgenda As New DataTable

        Dim objDestR As New AgronicaCoreContabDAL.Mov_Destinazioni_R
        Dim ObjRicette As New AgronicaCoreContabDAL.Ricette_Destinazioni_R

        Try
            dtAgenda = objDestR.Leggi(piva, sa_cod, 0, 0, 0, 0, fabbricato_cod,
                                      CostantiPersonalizzate.MAGAZZINO, enumSelezioneVariabile.Selezione_TabellaCompleta,
                                      "", "",
                                      objParametri)


            If dtAgenda.Rows.Count > 0 Then
                Return True
            End If

            Return False

        Catch ex As GiasException
            messaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return messaggioErrore
    End Function

    Private Sub EliminaFabbricato(objFabbricato As AgronicaCoreModelsSTD.anagrafiche.Fabbricato,
                                  ByRef GiasContext As Gias_DeveloperServer_Entities,
                                  ByRef objParametri_Server As AgronicaCoreParametri,
                                  ByRef objParametri_Utenti As AgronicaCoreParametri,
                                  Optional NoteLog As String = "")

        Dim Piva = objFabbricato.primaryKey.centroAziendalePK.partitaIva
        Dim Sa_Cod = objFabbricato.primaryKey.centroAziendalePK.codice
        Dim Fabbricato_Cod = objFabbricato.primaryKey.codice

        Dim PivaSuperUser = objParametri_Server.PivaSuperUser

        Dim fabbricato = (From f In GiasContext.Fabbricati
                          Where f.PIVA = Piva AndAlso
                                f.SA_COD = Sa_Cod AndAlso
                                f.Fabbricato_Cod = Fabbricato_Cod).FirstOrDefault
        If fabbricato IsNot Nothing Then
            If objFabbricato.tipo = TIPO_FABBRICATO_MAGAZZINO Then
                If controllo_MovimentiMagazzinoxEliminazione(Piva, Sa_Cod, Fabbricato_Cod, objParametri_Server) Then
                    Dim messaggioErroreAgenda As String = String.Format(Gias.ImpossibileEliminareFabbricatoXMovimentiMagazzinoAssociati, fabbricato.Fabbricato_Des)
                    Throw New GiasException(messaggioErroreAgenda)
                End If
            ElseIf objFabbricato.tipo = TIPO_FABBRICATO_STALLA Then
                Dim objFabbricato_R As New AgronicaCoreAnagrafeBIZ.Fabbricato_R
                Dim movimentiPresenti = objFabbricato_R.PresenzaMovimentiBoxDaStalla(Piva, Sa_Cod, Fabbricato_Cod, objParametri_Server)
                If movimentiPresenti Then
                    Dim messaggioErroreAgenda As String = String.Format(Gias.ImpossibileEliminareFabbricatoXMovimentiMagazzinoAssociati, fabbricato.Fabbricato_Des)
                    Throw New GiasException(messaggioErroreAgenda)
                End If
            End If

            Dim codIndirizzo = fabbricato.Indirizzo_Cod
            Dim indirizzi = (From i In GiasContext.Indirizzi Where i.cod_indirizzo = codIndirizzo).ToList

            Dim fabbricatiCodici = (From f In GiasContext.Fabbricati_Codici
                                    Where f.PIVA = Piva AndAlso
                                          f.sa_cod = Sa_Cod AndAlso
                                          f.Fabbricato_cod = Fabbricato_Cod).ToList()

            Dim fabbricatiImpostazioni = (From f In GiasContext.Fabbricati_Impostazioni
                                          Where f.Piva = Piva AndAlso
                                                  f.Sa_Cod = Sa_Cod AndAlso
                                                  f.Fabbricato_Cod = Fabbricato_Cod).ToList()

            Dim stalla = (From f In GiasContext.Stalla
                          Where f.PIVA = Piva AndAlso
                                  f.sa_cod = Sa_Cod AndAlso
                                  f.STA_NUM = Fabbricato_Cod).ToList()

            GiasContext.Fabbricati.Remove(fabbricato)
            GiasContext.Fabbricati_Codici.RemoveRange(fabbricatiCodici)
            GiasContext.Fabbricati_Impostazioni.RemoveRange(fabbricatiImpostazioni)
            GiasContext.Indirizzi.RemoveRange(indirizzi)
            GiasContext.Stalla.RemoveRange(stalla)


            GiasContext.SaveChanges()


            Dim datiFabbricatoStr = ""
            If objFabbricato IsNot Nothing Then
                Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
                datiFabbricatoStr = JsonConvert.SerializeObject(objFabbricato, a)
            End If

            'Scrittura tabella Agronica_Log_Anagrafe
            Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
            Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.Fabbricati,
                                                                                 CStr(Piva), CStr(Sa_Cod),
                                                                                 CStr(Fabbricato_Cod), "",
                                                                                 Nothing, Nothing,
                                                                                 enum_TipoOperazioneDB.Cancellazione,
                                                                                 objParametri_Server, enum_Id_Servizio.GiasOnline,
                                                                                 NoteLog, datiFabbricatoStr)
            GiasContext.Agronica_Log_Anagrafe.Add(log)
            GiasContext.SaveChanges()

        End If

    End Sub

    Public Sub Scrivi_Fabbricato_APP(ByRef fabbricato As AgronicaCoreModelsSTD.anagrafiche.Fabbricato,
                                     ByVal tipoOperazione As enum_TipoOperazioneDB,
                                     ByRef objParametri_Server As AgronicaCoreParametri,
                                     ByRef objParametri_Utenti As AgronicaCoreParametri)


        If fabbricato.validita Is Nothing Then
            fabbricato.validita = New IntervalloTemporale(AGRODATAINIZIO, AGRODATAFINE)
        End If

        ' modifico indirizzo magazzino
        If fabbricato.indirizzo IsNot Nothing AndAlso tipoOperazione = enum_TipoOperazioneDB.Modifica Then
            Dim objFabbricati As New Fabbricati_R
            Dim centroPK = fabbricato.primaryKey.centroAziendalePK
            Dim magazzino = objFabbricati.Leggi_3(centroPK.partitaIva, centroPK.codice, fabbricato.primaryKey.codice, "", "", objParametri_Server)
            If fabbricato.indirizzo.codice = 0 AndAlso magazzino.Rows.Count > 0 Then
                fabbricato.indirizzo.codice = magazzino.Rows(0)("indirizzo_cod")
            End If
        End If

        Dim codice = Scrivi_Fabbricato_Anagrafica(fabbricato, objParametri_Server, objParametri_Utenti)

        'commentata scrittura del codice enum_CodiciAnagrafe.Visibile_da_App poiché già scritta in Scrivi_Fabbricato_Anagrafica

        'If tipoOperazione = enum_TipoOperazioneDB.Scrittura Then

        '    fabbricato.primaryKey.codice = codice

        '    ' imposto flag visibile da app su magazzino
        '    Dim objFabbricatiCodiciW As New Fabbricati_Codici_W
        '    Dim centroPK = fabbricato.primaryKey.centroAziendalePK
        '    objFabbricatiCodiciW.Scrivi(centroPK.partitaIva, centroPK.codice, codice, enum_CodiciAnagrafe.Visibile_da_App, AGRODATAINIZIO, AGRODATAFINE, "1", objParametri_Server)

        'End If

    End Sub

    Public Sub Scrivi_Cella_APP(ByRef centro As AgronicaCoreModelsSTD.anagrafiche.CentroAziendale,
                                ByVal tipoOperazione As enum_TipoOperazioneDB,
                                ByRef objParametri_Server As AgronicaCoreParametri,
                                ByRef objParametri_Utenti As AgronicaCoreParametri)

        Dim base_code As Integer = 0
        Dim top_code As Integer = 2000000000

        Dim objSequenze As New Agro_Sequenze
        Dim objCaratteristiche As New Cantina_Caratter_W
        Dim objInsiemi As New Cantina_Insiemi_W
        Dim objVasche As New Cantina_Vasche_W
        Dim Piva = centro.primaryKey.partitaIva
        Dim Sa_Cod = centro.primaryKey.codice

        Dim Piano_Cod = objSequenze.NuovoId_Tabella("cantina_caratteristica", base_code, top_code, objParametri_Server)
        objCaratteristiche.ScriviPiano(Piva, Sa_Cod, Piano_Cod, "Piano " & centro.nome, objParametri_Server)

        Dim Insieme_Cod = objSequenze.NuovoId_Tabella("cantina_insiemi", base_code, top_code, objParametri_Server)
        objInsiemi.Scrivi(Piva, Sa_Cod, Piano_Cod, Insieme_Cod, "Insieme " & centro.nome, objParametri_Server)

        Dim objCodiciGias As New AgronicaCoreUtentiDAL.Utenti_CodiciGiasPRO_R
        objCodiciGias.Calcola_BaseCode_TopCode(base_code, top_code, objParametri_Utenti)
        Dim Vas_Cod = objSequenze.NuovoId_SeqMagazzino(Piva, Sa_Cod, base_code, top_code, objParametri_Server)
        Dim esito = objVasche.ScriviCella(Piva, Sa_Cod, Vas_Cod, Piano_Cod, "Cella " & centro.nome, Insieme_Cod, "", objParametri_Server)

    End Sub

    Public Sub DeleteCella(ByVal Piva As String,
                           ByVal Sa_Cod As Integer,
                           ByVal Vas_Cod As Integer,
                           ByVal objParametri_Server As AgronicaCoreParametri,
                           ByVal objParametri_Utenti As AgronicaCoreParametri,
                           Optional NoteLog As String = NOTELOG_ANAGRAFE_NG,
                           Optional SistemaOrigine As Integer = -1)

        Dim cantinaVascheW As New Cantina_Vasche_W
        cantinaVascheW.DeleteCantinaVascheEF(Piva, Sa_Cod, Vas_Cod, objParametri_Server, objParametri_Utenti, NoteLog, SistemaOrigine)

    End Sub

    Public Sub DeletePiano(ByVal Piva As String,
                           ByVal Sa_Cod As Integer,
                           ByVal pianoCod As Integer,
                           ByVal objParametri_Server As AgronicaCoreParametri,
                           ByVal objParametri_Utenti As AgronicaCoreParametri,
                           Optional NoteLog As String = NOTELOG_ANAGRAFE_NG,
                           Optional SistemaOrigine As Integer = -1)

        Dim cantinaCaratterW As New Cantina_Caratter_W
        cantinaCaratterW.DeleteCantinaCaratteristicheEF(Piva, Sa_Cod, pianoCod, objParametri_Server, objParametri_Utenti, NoteLog, SistemaOrigine)

    End Sub

    Public Sub DeleteInsieme(ByVal Piva As String,
                             ByVal Sa_Cod As Integer,
                             ByVal insiemeCod As Integer,
                             ByVal objParametri_Server As AgronicaCoreParametri,
                             ByVal objParametri_Utenti As AgronicaCoreParametri,
                             Optional NoteLog As String = NOTELOG_ANAGRAFE_NG,
                             Optional SistemaOrigine As Integer = -1)

        Dim cantinaInsiemiW As New Cantina_Insiemi_W
        cantinaInsiemiW.DeleteCantinaInsiemiEF(Piva, Sa_Cod, insiemeCod, objParametri_Server, objParametri_Utenti, NoteLog, SistemaOrigine)

    End Sub

    Public Sub DeletePareti(ByVal Piva As String,
                            ByVal Sa_Cod As Integer,
                            ByVal pareteCod As Integer,
                            ByVal objParametri_Server As AgronicaCoreParametri,
                            ByVal objParametri_Utenti As AgronicaCoreParametri,
                            Optional NoteLog As String = NOTELOG_ANAGRAFE_NG,
                            Optional SistemaOrigine As Integer = -1)

        Dim cantinaParetiW As New Cantina_Pareti_W
        cantinaParetiW.DeleteCantinaParetiEF(Piva, Sa_Cod, pareteCod, objParametri_Server, objParametri_Utenti, NoteLog, SistemaOrigine)

    End Sub


    Private Function Scrivi_Codice_Fabbricato(id_cod As Integer,
                                          val_cod As String,
                                           ByRef GiasContext As Gias_DeveloperServer_Entities,
                                           ByRef Fabbricato As AgronicaCoreEntityFramework_POCO.Fabbricati,
                                           ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                           ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim CodiciEF As AgronicaCoreEntityFramework_POCO.Fabbricati_Codici

        Dim piva = Fabbricato.PIVA
        Dim sa_cod = Fabbricato.SA_COD
        Dim fabbricato_Cod = Fabbricato.Fabbricato_Cod

        CodiciEF = (From cod In GiasContext.Fabbricati_Codici
                    Where cod.Fabbricato_cod = fabbricato_Cod AndAlso
                        cod.id_cod = id_cod AndAlso
                        piva = cod.PIVA AndAlso
                        sa_cod = cod.sa_cod
                   ).FirstOrDefault()

        If val_cod <> "" AndAlso val_cod <> "0" Then

            If CodiciEF Is Nothing Then

                CodiciEF = AgronicaCoreAnagrafeDAL.EFFabbricati.CreateFabbricato_FabbricatiCodici(GiasContext, objParametri_Server, piva, sa_cod,
                                                                                                  fabbricato_Cod, id_cod, val_cod,
                                                                                                  objParametri_Utenti.UsernameOperazione)

            End If

            CodiciEF.val_cod = val_cod
            CodiciEF.Username_Modifica = objParametri_Server.UsernameOperazione
            CodiciEF.Data_Modifica = DateTime.Now

        Else
            If CodiciEF IsNot Nothing Then

                ' funzione per cancellare il record
                GiasContext.Fabbricati_Codici.Attach(CodiciEF)
                GiasContext.Fabbricati_Codici.Remove(CodiciEF)

            End If
        End If
        GiasContext.SaveChanges()

        Return id_cod
    End Function

End Class
