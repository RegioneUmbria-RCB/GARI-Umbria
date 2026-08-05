Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ


Partial Class Impianto_Edit_OLD
    Inherits System.Web.UI.Page

#Region "Default"
    Private Const DEFAULT_SPECIE As String = ""
    Private Const DEFAULT_CODICITERRENO As String = ""
    Private Const DEFAULT_IMPIRRIGAZIONE As Integer = -1
    Private Const DEFAULT_PROVENIENZA_SEME As Integer = 0
    Private Const DEFAULT_SEMINA_TRAPIANTO As String = "-1"
    Private Const DEFAULT_DETT_VARIETA_PERSONALIZZATO As String = ""
    Private Const DEFAULT_DISCIPLINARE As String = ""
    Private Const DEFAULT_CAPITOLATO_PRIVATO As String = ""
    Private Const DEFAULT_ORG_REFERENTE As String = ""
    Private Const DEFAULT_REGOLAMENT_CONC As Integer = 0
    Private Const DEFAULT_REGOLAMENTO As Integer = 1
    Private Const DEFAULT_FINALITA As Integer = -1
    Private Const DEFAULT_CULTIVAR As Integer = 0
    Private Const DEFAULT_GRVA As Integer = 0
    Private Const DEFAULT_PORTTINNESTO As Integer = -1
    Private Const DEFAULT_FORMA_ALLEVAMENTO As Integer = -1
    Private Const DEFAULT_COPERTURA As Integer = -1

#End Region

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Dim objParametriAgenda As ParametriAgenda

    Public Operazione As Integer
    Dim Operazione_Contatti As Integer

    ''----- Gestione Querystring
    'Dim Qs_Key As String
    'Dim Qs_Operazione As String
    Dim Qs_Piva As String
    'Dim Qs_PivaPadre As String
    Dim Qs_PivaNuova As String
    'Dim Qs_PaginaRitorno As String

    Public permessi As PermessiUtente

    Dim xPiva As String
    Dim xSa_Cod As String
    Dim xAppezza As String
    Dim xId_Imp As String
    Dim xCampo_Cod As String

    Dim xValiditaInizio As Date
    Dim xValiditaFine As Date

    Dim BaseCode As Integer
    Dim TopCode As Integer

    Dim Id_Consociazione As Integer

    Public jsImpianti As String
    Public jsCodici As String
    Public jsParticelle As String

#Region "OldVersion"

    '##########################################################################################################################################
    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

        ' Pulisco la Sessione
        'HttpContext.Current.Session("dt_Padri") = Nothing
        'HttpContext.Current.Session("dt_Codici") = Nothing

        ' Pulisco il valore per visualizzare la lista completa di impianti nel menu principale
        objParametriAgenda.Sa_Cod = 0
        objParametriAgenda.Appezza = 0

        Dim TargetUrl As String
        TargetUrl = Master.TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAnagrafica, objParametriAgenda)

        Response.Redirect(TargetUrl)

    End Sub

    Private Sub Impianto_Edit_OLD_Init(sender As Object, e As System.EventArgs) Handles Me.Init

        ' VAnni: 2/10/2017: a seguito di AgroMaseterPage
        'AddHandler CType(Page.Master.FindControl("ImgBtn_AnnullaTutto"), ImageButton).Click, AddressOf Me.AnnullaTutto
        AddHandler CType(Me.Master, AgendaBootstrap).ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto

    End Sub


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        permessi = New PermessiUtente()

        Master.flag_pag_Anagrafica = True
        '----- Verifico che l'utente sia autenticato
        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("~/Custom500.aspx")
        End If

        '---
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        '---

        objParametriAgenda = New ParametriAgenda
        xPiva = objParametriAgenda.Piva
        xSa_Cod = objParametriAgenda.Sa_Cod
        xAppezza = objParametriAgenda.Appezza

        Select Case objParametriAgenda.Tipo_Operazione
            Case enum_TipoOperazioneDB.Scrittura
                Master.Lbl_Titolo.Text = "Creazione Nuovo Impianto"
                'objParametriAgenda.Sa_Cod = 0
                xId_Imp = 0
            Case enum_TipoOperazioneDB.Lettura
                Master.Lbl_Titolo.Text = "Lettura Impianto"
                xId_Imp = objParametriAgenda.Id_Imp
            Case enum_TipoOperazioneDB.Modifica
                Master.Lbl_Titolo.Text = "Modifica Impianto"
                xId_Imp = objParametriAgenda.Id_Imp
        End Select

        Operazione = objParametriAgenda.Tipo_Operazione

        HttpContext.Current.Session("operazione") = Operazione

        'Call ChiaveAlbero_Decodifica_PartitaIVA(Qs_Key, Qs_PivaPadre)


        '==================================
        '======= VERIFICA PERMESSI ========
        '==================================

        Dim UtenteAbilitato_Lettura As Boolean = False
        Dim UtenteAbilitato_Modifica As Boolean = False
        UtenteAbilitato_Lettura = permessi.getPermesso(enum_Security_Attivita.Anagrafica_CentroAziendale).Lettura
        UtenteAbilitato_Modifica = permessi.getPermesso(enum_Security_Attivita.Anagrafica_CentroAziendale).Scrittura
        Session("UtenteAbilitato_Modifica") = UtenteAbilitato_Modifica
        Session("UtenteAbilitato_Lettura") = UtenteAbilitato_Lettura


        If UtenteAbilitato_Modifica = False Then
            'Non ho i permessi per modificare l'impianto
            Response.Redirect("../MenuAnagrafica/Menubs_anagrafica.aspx")
            Exit Sub
        End If


        ' Inserisco i dati nelle label in testata
        'Centro Aziendale
        Dim objCentriAz As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
        LblCentro.Text = objCentriAz.SaNome_from_SaCod(xPiva, xSa_Cod, objParametri_Server)

        'Appezzamento
        Dim objAppezza As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
        LblAppezza.Text = objAppezza.AppezzamentoNome_from_Appezza(xPiva, xSa_Cod, xAppezza, objParametri_Server)

        ' Campo
        Dim Dt_Appezzamento As New DataTable
        Dt_Appezzamento = objAppezza.Recupera_Dati_Appezzamento(xPiva, xSa_Cod, xAppezza, "", "", objParametri_Server)
        If Not Dt_Appezzamento Is Nothing AndAlso Dt_Appezzamento.Rows.Count > 0 Then
            LblCampo.Text = Dt_Appezzamento.Rows(0).Item("Campo_Des")

            If Dt_Appezzamento.Rows(0).Item("Campo_Cod") <> 0 Then
                xCampo_Cod = CStr(Dt_Appezzamento.Rows(0).Item("Campo_Cod"))

                Dim Dt_Centro As New DataTable

                Dim objImpresaR As New AgronicaCoreAnagrafeDAL.Imprese_Read
                Dt_Centro = objImpresaR.Recupera_Dati_CentroAziendale(xPiva, xSa_Cod, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

                If Not Dt_Centro Is Nothing AndAlso Dt_Centro.Rows.Count > 0 Then
                    lbl_centro_data_inizio.Text = Dt_Centro.Rows(0).Item("Validita_Inizio")
                    lbl_centro_data_fine.Text = Dt_Centro.Rows(0).Item("Validita_Fine")
                End If

            Else
                xCampo_Cod = "0"
            End If

            InizioAppezzamento.Value = Dt_Appezzamento.Rows(0).Item("Validita_Inizio")
            FineAppezzamento.Value = Dt_Appezzamento.Rows(0).Item("Validita_Fine")

            lbl_appezza_data_inizio.Text = Dt_Appezzamento.Rows(0).Item("Validita_Inizio")
            lbl_appezza_data_fine.Text = Dt_Appezzamento.Rows(0).Item("Validita_Fine")

        End If





        AgronicaCoreUtility.CaricaListControl.SpecieVegetale_Optimize_x_PDC_Modificata(Cmb_Specie,
                                                                            True, "SELEZIONA", DEFAULT_SPECIE, objParametri_Server)
        AgronicaCoreUtility.CaricaListControl.CodiciTerreno(Cmb_CodiciTerreno, True, "SELEZIONA", DEFAULT_CODICITERRENO, "", "", objParametri_Server)
        AgronicaCoreUtility.CaricaListControl.CaricaCombo_ImpIrrigazione(Cmb_ImpIrrigazione, True, "SELEZIONA", CStr(DEFAULT_IMPIRRIGAZIONE), 0, "", "", "", objParametri_Server)
        AgronicaCoreUtility.CaricaListControl.CaricaCombo_ProvenienzaSeme(Cmb_ProvenienzaSeme, True, "SELEZIONA", CStr(DEFAULT_PROVENIENZA_SEME))
        AgronicaCoreUtility.CaricaListControl.CaricaCombo_SeminaTrapianto(Cmb_SeminaTrapianto, True, "SELEZIONA", CStr(DEFAULT_SEMINA_TRAPIANTO))
        AgronicaCoreUtility.CaricaListControl.DettaglioPersonalizzato_Impianto(Cmb_DettaglioVarietaPersonalizzato, True, "SELEZIONA", DEFAULT_DETT_VARIETA_PERSONALIZZATO, "", "", objParametri_Server)

        Cmb_Stato.Items.Add(New ListItem("Impianto in Produzione",
                                       "102"))

        Dim objCaricaCombo As New AgronicaCoreDpiBIZ.CaricaListControl

        objCaricaCombo.Disciplinari_Elenco_Metaschema(Cmb_Disciplinare,
                                           True, "SELEZIONA", DEFAULT_DISCIPLINARE,
                                           objParametri_Server,
                                           0,
                                           True,
                                           False,
                                                New AgronicaCoreGestioneRichieste.AgroWebConfig)

        AgronicaCoreUtility.CaricaListControl.Capitolato_Privato(Cmb_CapitolatoPrivato,
                                                                       True,
                                                                       "SELEZIONA",
                                                                       DEFAULT_CAPITOLATO_PRIVATO,
                                                                       "",
                                                                       0,
                                                                       2,
                                                                       "", "",
                                                                       objParametri_Server)
        AgronicaCoreUtility.CaricaListControl.OrganismiReferenti(Cmb_OrganismoReferente, True, "SELEZIONA", DEFAULT_ORG_REFERENTE, xPiva, "", objParametri_Server)

        Dim objReg As New AgronicaCoreMetaSchemaDAL.PUA_Regolamenti_R
        Dim i_reg As Integer = 0
        Dim DtReg As DataTable

        'Reg Concimazione
        Cmb_RegolamentoConc.Items.Add(New ListItem("Nessuno", CStr(DEFAULT_REGOLAMENT_CONC)))
        DtReg = objReg.Leggi(0,
                              AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                              "", "",
                              objParametri_Server)
        If DtReg.Rows.Count > 0 Then
            For i_reg = 0 To DtReg.Rows.Count - 1
                Cmb_RegolamentoConc.Items.Add(New ListItem(DtReg.Rows(i_reg).Item("Regolamento_DES") & " Calcolato",
                           -DtReg.Rows(i_reg).Item("Regolamento_Cod")))
                Cmb_RegolamentoConc.Items.Add(New ListItem(DtReg.Rows(i_reg).Item("Regolamento_DES") & " Dose Standard",
                            DtReg.Rows(i_reg).Item("Regolamento_Cod")))
            Next
        End If


        'carico la combo dei codici
        Dim StrCodiciImpianto As String
        Dim objcodAn As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R
        StrCodiciImpianto = objcodAn.Filtro_Codici_Anagrafe(4, 3, 2, objParametri_Server)
        'Elimino il codice Titolo Possesso, Metodo Produzione, Magazzino Conferimento,
        'Organismo Referente, Capitolato Privato e Dettaglio Specie Personalizzato 
        'perchè già presenti
        StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " + CStr(enum_CodiciAnagrafe.TitoloPossesso), "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " + CStr(enum_CodiciAnagrafe.TitoloPossesso) + " Or ", "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " + CStr(enum_CodiciAnagrafe.MetodoDiProduzione), "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " + CStr(enum_CodiciAnagrafe.MetodoDiProduzione) + " Or ", "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " + CStr(enum_CodiciAnagrafe.Organismo_Referente), "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " + CStr(enum_CodiciAnagrafe.Organismo_Referente) + " Or ", "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " + CStr(enum_CodiciAnagrafe.Capitolato_Privato), "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " + CStr(enum_CodiciAnagrafe.Capitolato_Privato) + " Or ", "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " + CStr(enum_CodiciAnagrafe.Dettaglio_Specie_Personalizzato), "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " + CStr(enum_CodiciAnagrafe.Dettaglio_Specie_Personalizzato) + " Or ", "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " + CStr(enum_CodiciAnagrafe.Magazzino_Conferimento), "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " + CStr(enum_CodiciAnagrafe.Magazzino_Conferimento) + " Or ", "")

        AgronicaCoreUtility.CaricaListControl.Codici(CType(CmbCodice, ListControl),
                                                     True, "", "",
                                                     0, "",
                                                     StrCodiciImpianto, "", objParametri_Server)

        'metto la stinga restituita dal componente nel formato "cod,cod,cod.."
        'per utilizzarla quando devo caricare i dati
        StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = ", ",")
        StrCodiciImpianto = Right(StrCodiciImpianto, StrCodiciImpianto.Length - 9)











        If Not Page.IsPostBack Then

            '    '==========================================
            '    '===== Pagina caricata per la prima volta
            '    '==========================================

            HttpContext.Current.Session("Dt_Progetti") = Nothing
            HttpContext.Current.Session("dt_Codici_Ana") = Nothing
            HttpContext.Current.Session("DT_Particelle") = Nothing


        Else

            '==========================================
            '===== Pagina ricaricata in POSTBACK
            '==========================================

            Exit Sub

        End If


        AgronicaCoreUtility.CaricaListControl.CaricaCombo_ParticelleCatastali_xAppezzamento(Cmb_Particella, xPiva, xSa_Cod, xAppezza, objParametri_Server)

        '##############################################################
        '#####  Se sono in MODIFICA carico i dati  ####################
        '##############################################################

        If Operazione = enum_TipoOperazioneDB.Scrittura Then

            Dim Dt As New DataTable
            Dim Dt2 As New DataTable
            Dim Dt3 As New DataTable

            jsImpianti = DT_to_Json_Progetti(Dt)
            HttpContext.Current.Session("Dt_Progetti") = Dt

            Dt2.Columns.Add(New DataColumn("Id_Cod", GetType(Integer)))
            Dt2.Columns.Add(New DataColumn("Descrizione", GetType(String)))
            Dt2.Columns.Add(New DataColumn("Val_Cod", GetType(String)))
            Dt2.Columns.Add(New DataColumn("Validita_Inizio", GetType(String)))
            Dt2.Columns.Add(New DataColumn("Validita_Fine", GetType(String)))
            jsCodici = DT_to_Json_Codici(Dt2)
            HttpContext.Current.Session("dt_Codici_Ana") = Dt2


            Dt3.Columns.Add(New DataColumn("Nome", GetType(String)))
            Dt3.Columns.Add(New DataColumn("Val_Cod", GetType(String)))
            jsParticelle = DT_to_Json_Particelle(Dt3)
            HttpContext.Current.Session("DT_Particelle") = Dt3


        ElseIf Operazione = enum_TipoOperazioneDB.Modifica Or Operazione = enum_TipoOperazioneDB.Lettura Then

            'LblSubAppezza.Text = "0,68"
            'TxtSuperficie.Text = "0,68"
            'TxtSuperficie2.Text = "0,68"
            'Txt_Germinabilita.Text = "100"

            'Carico i Progetti/Distinte ordinati dal in anno decrescente in modo da avere l'ultima distinta in alto
            Dim objProgetto As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
            Dim Dt As DataTable

            Dt = objProgetto.Leggi(CStr(xPiva),
                                0,
                                CInt(9100),
                                0,
                                CInt(xSa_Cod),
                                CInt(xAppezza),
                                CInt(xId_Imp),
                                0, 0,
                                enumSelezioneVariabile.Selezione_TabellaCompleta,
                                "",
                                " Imprese_Progetti.Validita_Inizio DESC ",
                                objParametri_Server)
            jsImpianti = DT_to_Json_Progetti(Dt)
            HttpContext.Current.Session("Dt_Progetti") = Dt


            ' @Paolo
            ' Riempio la distinta dell'ultimo impianto 

            Dim objParametriAgenda As New ParametriAgenda
            Dim Dt_Progetti As DataTable

            'Mi procuro il recordset richiesto
            Dt_Progetti = objProgetto.Leggi(
                                            CStr(objParametriAgenda.Piva),
                                            CInt(Dt.Rows(0).Item("Progetto_Cod")),
                                            "",
                                            0,
                                            CInt(objParametriAgenda.Sa_Cod),
                                            CInt(objParametriAgenda.Appezza),
                                            CInt(objParametriAgenda.Id_Imp),
                                            0,
                                            0,
                                            AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                            "",
                                            "",
                                            objParametri_Server)

            If Not IsNothing(Dt_Progetti) Then

                Txt_ValiditaInizio_Distinta.Text = Dt.Rows(0).Item("Validita_Inizio")
                Txt_ValiditaFine_Distinta.Text = Dt.Rows(0).Item("Validita_Fine")
                TxtPianteImpianto2.Text = Dt_Progetti.Rows(0).Item("p_ha")
                Txt_semina_prevista.Text = Dt_Progetti.Rows(0).Item("data_inizio_prevista")
                Txt_fioritura_prevista.Text = Dt_Progetti.Rows(0).Item("data_fioritura_prevista")
                Txt_raccolta_prevista.Text = Dt_Progetti.Rows(0).Item("data_fine_prevista")
                Txt_ResaPrevista.Text = Dt_Progetti.Rows(0).Item("giudizio")
                Txt_Resa1.Text = Dt_Progetti.Rows(0).Item("produzione_prevista")
                Cmb_Disciplinare.SelectedValue = Dt_Progetti.Rows(0).Item("disciplinare_cod") & "/1"
                Cmb_Regolamento.SelectedValue = Dt_Progetti.Rows(0).Item("regolamento_cod")
                Cmb_RegolamentoConc.SelectedValue = Dt_Progetti.Rows(0).Item("regolamento_concimazioni_cod")
                Cmb_Stato.SelectedValue = Dt_Progetti.Rows(0).Item("stato_impianto")
                Txt_Lotto.Text = Dt_Progetti.Rows(0).Item("Progetto_Nome")

                '' @Paolo
                '' Leggo le particelle legate al progetto


                'Dim FiltroLetturaCodici As String = ""
                'Dim Id_Cod As Integer
                'Dim Val_Cod As String = ""
                ''Dim jsParticelle As String

                'Dim StringaXML As String
                ''variabili per spacchettamento stringa xml
                'Dim XmlDoc As New System.Xml.XmlDocument
                'Dim XML_DatiProgetto As System.Xml.XmlElement
                'Dim XML_Progetto As System.Xml.XmlElement
                ''Dim XML_Fase As System.Xml.XmlElement
                ''Dim XMLs_Fase As System.Xml.XmlNodeList
                ''Dim XML_DatiCodici As System.Xml.XmlElement
                ''Dim XML_CodiceImpianto As System.Xml.XmlElement
                ''Dim XMLs_CodiceImpianto As System.Xml.XmlNodeList
                'Dim XML_DatiParticelle As System.Xml.XmlElement
                'Dim XML_Particella As System.Xml.XmlElement
                'Dim XMLs_Particelle As System.Xml.XmlNodeList

                '' Create new DataTable instance.
                'Dim tbl_Particelle As New DataTable

                'tbl_Particelle.Columns.Add(New DataColumn("Nome", GetType(String)))
                'tbl_Particelle.Columns.Add(New DataColumn("Val_Cod", GetType(String)))



                'Dim objProgetto2 As New AgronicaCoreAnagrafeBIZ.Progetto_R
                'StringaXML = objProgetto2.Impresa_Progetti_Leggi( _
                '                                    CStr(objParametriAgenda.Piva), _
                '                                    CInt(Dt.Rows(0).Item("Progetto_Cod")), _
                '                                    "", _
                '                                    0, _
                '                                    CInt(objParametriAgenda.Sa_Cod), _
                '                                    CInt(objParametriAgenda.Appezza), _
                '                                    CInt(objParametriAgenda.Id_Imp), _
                '                                    0, 0, _
                '                                    False, _
                '                                    objParametri_Server)

                'XmlDoc.LoadXml(StringaXML)

                ''----- Tag DatiProgetto

                'XML_DatiProgetto = XmlDoc.SelectSingleNode("DatiProgetto")

                ''----- Tag Progetto

                'XML_Progetto = XML_DatiProgetto.SelectSingleNode("Progetto")

                ''--------------------------------------
                ''--- Particelle x Progetto
                ''--------------------------------------

                'Dim StrParticella As String = ""
                'Dim StrParticelle As String = ""
                'Dim Prov As String
                'Dim Com As String
                'Dim Sezione As String
                'Dim Foglio As Integer
                'Dim Numero As Integer
                'Dim Subalterno As String
                'Dim StrCodProvincia As String
                'Dim StrCodComune As String
                'Dim StrSezione As String
                'Dim StrFoglio As String
                'Dim StrNumero As String
                'Dim StrSubalterno As String
                'Dim Testo As String
                'Dim Valore As String

                'Dim BaseCode As Integer
                'Dim TopCode As Integer

                'Call Calcola_BaseCode_TopCode(BaseCode, _
                '                          TopCode, _
                '                          HttpContext.Current.Session("ASG_ProgressivoGIAS"))

                'XML_DatiParticelle = XML_Progetto.SelectSingleNode("DatiParticellexProgetto")



                'If Not XML_DatiParticelle Is Nothing AndAlso XML_DatiParticelle.HasChildNodes Then

                '    '----- Tag Particella  (multiplo)

                '    'Recupero la collezione dei nodi
                '    XMLs_Particelle = XML_DatiParticelle.GetElementsByTagName("ParticellaxProgetto")

                '    If XMLs_Particelle.Count > 0 Then

                '        StrParticelle = ""




                '        For i = 0 To XMLs_Particelle.Count - 1

                '            XML_Particella = XMLs_Particelle.Item(i)

                '            Prov = XML_Particella.GetAttribute("prov")
                '            Com = XML_Particella.GetAttribute("com")
                '            Sezione = XML_Particella.GetAttribute("sezione")
                '            Foglio = XML_Particella.GetAttribute("foglio")
                '            Numero = XML_Particella.GetAttribute("numero")
                '            Subalterno = XML_Particella.GetAttribute("subalterno")

                '            Id_Cod = CInt(XML_Particella.GetAttribute("id_cod"))
                '            Val_Cod = XML_Particella.GetAttribute("val_cod")



                '            'Genero l'XML del singolo nodo
                '            Call XML_ParticellaxProgetto(enum_CodificaDecodifica.Codifica,
                '                StrParticella,
                '                enum_TipoOperazioneDB.Cancellazione,
                '                Prov,
                '                Com,
                '                Sezione,
                '                Foglio,
                '                Numero,
                '                Subalterno,
                '                Id_Cod,
                '                Val_Cod,
                '                CDate(XML_Particella.GetAttribute("validita_inizio")),
                '                CDate(XML_Particella.GetAttribute("validita_fine")),
                '                BaseCode,
                '                TopCode)

                '            tbl_Particelle.Rows.Add(StrParticella, Val_Cod)

                '        Next

                '    End If

                'End If



                'HttpContext.Current.Session("DT_Particelle") = tbl_Particelle

                'jsParticelle = DT_to_Json_Particelle(tbl_Particelle)




            End If

            '''''''''''''''''''''''''''''''''''''''''


            Dim Dt2 As New DataTable

            Dim objRegImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
            Dim DtRegImpianti As DataTable

            DtRegImpianti = objRegImpianti.Leggi(xPiva, xSa_Cod, xAppezza, xId_Imp, enumSelezioneVariabile.Selezione_JoinCompleta, "", "", objParametri_Server)

            If Not DtRegImpianti Is Nothing AndAlso DtRegImpianti.Rows.Count > 0 Then

                Dim CulCod As Integer
                Dim Veg_Cod As Integer
                Dim Gru_Cod As Integer
                ''---------------------
                ''riciclo resa_prevista e resa_corretta impianto per apofruit
                'Try
                '    If Not IsDBNull(DtRegImpianti.Fields("Resa_Prevista")) Then
                '        If Not IsNothing(DtRegImpianti.Fields("Resa_Prevista").Value) Then
                '            If IsNumeric(DtRegImpianti.Fields("Resa_Prevista").Value) Then
                '                Me.Txt_Resa1.Text = DtRegImpianti.Fields("Resa_Prevista").Value
                '            End If
                '        End If
                '    End If
                '    If Not IsDBNull(DtRegImpianti.Fields("Resa_Effettiva")) Then
                '        If Not IsNothing(DtRegImpianti.Fields("Resa_Effettiva").Value) Then
                '            If IsNumeric(DtRegImpianti.Fields("Resa_Effettiva").Value) Then
                '                Me.Txt_Resa2.Text = DtRegImpianti.Fields("Resa_Effettiva").Value
                '            End If
                '        End If
                '    End If


                'Catch ex As Exception
                'End Try

                Id_Consociazione = CInt(DtRegImpianti.Rows(0).Item("Id_Consociazione"))


                If DtRegImpianti.Rows(0).Item("sup_imp") <> "0" Then
                    TxtSuperficie.Text = DtRegImpianti.Rows(0).Item("sup_imp")
                    LblSubAppezza.Text = DtRegImpianti.Rows(0).Item("sup_imp")
                End If


                CulCod = DtRegImpianti.Rows(0).Item("Cul_cod")

                ' Controllo se è un terreno nudo
                If CulCod = 0 Then

                    '===== TERRENO NUDO =========================

                    'Attivo i controlli opportuni
                    'Call xx_TerrenoNudo_On()

                    'Attivo il check del terreno nudo
                    ChkTerrenoNudo.Checked = True

                Else

                    'Disattivo il check del terreno nudo
                    ChkTerrenoNudo.Checked = False

                    Veg_Cod = DtRegImpianti.Rows(0).Item("Veg_Cod")
                    Gru_Cod = DtRegImpianti.Rows(0).Item("Gru_Cod")

                    'Imposto le informazioni
                    TxtValiditaInizio.Text = DtRegImpianti.Rows(0).Item("Validita_Inizio")
                    TxtValiditaFine.Text = DtRegImpianti.Rows(0).Item("Validita_Fine")

                    '
                    If DtRegImpianti.Rows(0).Item("cover") = 0 Then
                        ChkCoverCrops.Checked = False
                    Else
                        ChkCoverCrops.Checked = True
                    End If
                    '
                    If DtRegImpianti.Rows(0).Item("monitorato") = 0 Then
                        ChkMonitorato.Checked = False
                    Else
                        ChkMonitorato.Checked = True
                    End If
                    '
                    TxtCopDataInizio.Text = DtRegImpianti.Rows(0).Item("cop_di").ToString
                    TxtCopDataFine.Text = DtRegImpianti.Rows(0).Item("cop_df").ToString

                    'Specie Vegetale
                    Cmb_Specie.SelectedIndex =
                        Cmb_Specie.Items.IndexOf(Cmb_Specie.Items.FindByValue(
                        Veg_Cod))


                    'Se la specie vegetale è 
                    Select Case Veg_Cod

                        Case 6  'la barbabietola da zucchero occorre il sesto impianto

                            'Semina/Trapianto
                            Cmb_SeminaTrapianto.SelectedIndex =
                                Cmb_SeminaTrapianto.Items.IndexOf(Cmb_SeminaTrapianto.Items.FindByValue(
                                DtRegImpianti.Rows(0).Item("setup_cod").ToString))

                        'Conduzione Terreno Tra Fila
                        'Cmb_ConduzioneTraFila.SelectedIndex = _
                        '    Cmb_ConduzioneTraFila.Items.IndexOf(Cmb_ConduzioneTraFila.Items.FindByValue( _
                        '    RsImpianti.Fields("tecn_cod").Value.ToString))


                        Case 46  'Se patata attivo il pannello tuberi

                            'Pannello_Tuberi.Visible = True

                    End Select


                    '
                    'Finalita
                    AgronicaCoreUtility.CaricaListControl.Finalita(Cmb_Finalita, True, "SELEZIONA", CStr(DEFAULT_FINALITA), Veg_Cod, 0, "", "", "",
                                                                         objParametri_Server)

                    Cmb_Finalita.SelectedIndex =
                    Cmb_Finalita.Items.IndexOf(Cmb_Finalita.Items.FindByValue(
                    DtRegImpianti.Rows(0).Item("Grfi_cod")))
                    '
                    'Cultivar
                    AgronicaCoreUtility.CaricaListControl.Cultivar(Cmb_Cultivar, True, "SELEZIONA", CStr(DEFAULT_CULTIVAR), Veg_Cod, 0, "", True, 0, 0, "", "",
                                                                         objParametri_Server,
                                                                         HttpContext.Current.Session("ASG_objParametri_Utenti"))

                    Cmb_Cultivar.SelectedIndex =
                    Cmb_Cultivar.Items.IndexOf(Cmb_Cultivar.Items.FindByValue(
                    DtRegImpianti.Rows(0).Item("cul_cod")))
                    '
                    'Gruppo Varietale
                    AgronicaCoreUtility.CaricaListControl.GruppoVarietalexSpecie(Cmb_TipologiaVarietale, True, "SELEZIONA", CStr(DEFAULT_GRVA), Veg_Cod, "", "",
                                                                         objParametri_Server)

                    Cmb_TipologiaVarietale.SelectedIndex =
                    Cmb_TipologiaVarietale.Items.IndexOf(Cmb_TipologiaVarietale.Items.FindByValue(
                    DtRegImpianti.Rows(0).Item("grva_cod_veg")))


                    Select Case DtRegImpianti.Rows(0).Item("grva_cod_veg")
                        Case Is < 0 'Ibrido
                            ChkVarietaIbrida.Checked = True
                        Case Is > 0 'Non Ibrido
                            ChkVarietaIbrida.Checked = False
                        Case Else
                            ChkVarietaIbrida.Checked = False
                    End Select


                    'Portinnesto
                    AgronicaCoreUtility.CaricaListControl.CaricaCombo_Portinnesto(Cmb_Portinnesto, True, "SELEZIONA", CStr(DEFAULT_PORTTINNESTO), Veg_Cod, 0, "", "", "",
                                                                     objParametri_Server)
                    Cmb_Portinnesto.SelectedIndex =
                    Cmb_Portinnesto.Items.IndexOf(Cmb_Portinnesto.Items.FindByValue(
                    DtRegImpianti.Rows(0).Item("port_cod").ToString))
                    '
                    'Forma Allevamento
                    AgronicaCoreUtility.CaricaListControl.CaricaCombo_FormaAllevamento(Cmb_FormaAllevamento, True, "SELEZIONA", CStr(DEFAULT_FORMA_ALLEVAMENTO), Veg_Cod, 0, "", "", "",
                                                                     objParametri_Server)
                    Cmb_FormaAllevamento.SelectedIndex =
                    Cmb_FormaAllevamento.Items.IndexOf(Cmb_FormaAllevamento.Items.FindByValue(
                    DtRegImpianti.Rows(0).Item("foral_cod").ToString))

                    'Impianto Irrigazione
                    Cmb_ImpIrrigazione.SelectedIndex =
                        Cmb_ImpIrrigazione.Items.IndexOf(Cmb_ImpIrrigazione.Items.FindByValue(
                        DtRegImpianti.Rows(0).Item("Imp_Cod2").ToString))
                    '
                    'Provenienza Seme
                    Cmb_ProvenienzaSeme.SelectedIndex =
                        Cmb_ProvenienzaSeme.Items.IndexOf(Cmb_ProvenienzaSeme.Items.FindByValue(
                        DtRegImpianti.Rows(0).Item("ProvenienzaSeme").ToString))
                    '
                    'Copertura
                    AgronicaCoreUtility.CaricaListControl.Copertura(Cmb_Copertura, True, "SELEZIONA", CStr(DEFAULT_COPERTURA), Gru_Cod, 0, "", "", "", objParametri_Server)

                    Cmb_Copertura.SelectedIndex =
                    Cmb_Copertura.Items.IndexOf(Cmb_Copertura.Items.FindByValue(
                    DtRegImpianti.Rows(0).Item("cop_cod").ToString))

                    ' Unità vitata
                    Dim Unita_Vitata As String = ""
                    If Not IsNothing(DtRegImpianti.Rows(0).Item("Unita_Vitata")) Then
                        Unita_Vitata = DtRegImpianti.Rows(0).Item("Unita_Vitata")
                    End If
                    If IsNumeric(Unita_Vitata) AndAlso CInt(Unita_Vitata) <> 0 Then
                        TXT_UnitaVitata.Text = Unita_Vitata
                    Else
                        TXT_UnitaVitata.Text = ""
                    End If


                    '--------------------------------------------------------------------------------------------------------------------------------------------


                    'Imposto le informazioni
                    TxtValiditaInizio.Text = DtRegImpianti.Rows(0).Item("Validita_Inizio")
                    TxtValiditaFine.Text = DtRegImpianti.Rows(0).Item("Validita_Fine")

                    'controllo se la data inizio e fine validità è uguale ai valori di default allora
                    'imposto le txt vuote..
                    If Me.TxtValiditaInizio.Text = "01/01/1900" Then
                        Me.TxtValiditaInizio.Text = ""
                    End If

                    If Me.TxtValiditaFine.Text = "31/12/2100" Then
                        Me.TxtValiditaFine.Text = ""
                    End If


                    Cmb_CodiciTerreno.SelectedIndex =
                                        Cmb_CodiciTerreno.Items.IndexOf(Cmb_CodiciTerreno.Items.FindByValue(
                                            xId_Imp.ToString))




                End If


                '------------------------------------------------------------------------------------------------------
                '--------- Carico i codici associati all'impianto
                '------------------------------------------------------------------------------------------------------

                Dim ErrMSG As String

                Dim Connessione As OleDb.OleDbConnection
                Dim Transazione As OleDb.OleDbTransaction

                Dim objCodici As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R  'Object 'Agro_Anagrafe_AD.Reg_Impianti_Codici_R
                Dim DtCodici As DataTable
                'Dim Dr() As DataRow
                Dim i As Integer

                Dim Id_Cod As Integer
                Dim Val_Cod As String
                Dim StrCodice As String
                Dim StrCodici2 As String = ""
                Dim CodiceAnagrafeDes As String
                Dim FiltroLetturaCodici As String = ""

                'modifica del 27/09/2012: in modifica dell'impianto si perdevano i codici
                'in cui vengono salvate le precessioni colturali

                'Non devono essere cancellati i codici specie-varietà cliente!!!
                FiltroLetturaCodici += " ( " &
                                            " Reg_Impianti_Codici.Id_Cod NOT IN ( " &
                                            CStr(enum_CodiciAnagrafe.Codice_Specie_Agea) & ", " &
                                            CStr(enum_CodiciAnagrafe.Codice_Cultivar_Agea) & ", " &
                                            CStr(enum_CodiciAnagrafe.Coltura_Precedente_1) & ", " &
                                            CStr(enum_CodiciAnagrafe.Coltura_Precedente_2) & ", " &
                                            CStr(enum_CodiciAnagrafe.Coltura_Precedente_3) & ", " &
                                            CStr(enum_CodiciAnagrafe.Coltura_Precedente_4) & " " &
                                            "   ) " &
                                            " )"

                '--------------------------------------------------
                'filtro i codici relativi all'impianto...
                'FILTRO i codici CHIAVE DEI CLIENTI!!!!!!!!!!!!!!!
                '--------------------------------------------------
                'aggiungo al filtro anche l'esclusione dei codici clienti
                '(veniva fatto dopo la select sul dt, ma così escludo già in partenza)
                FiltroLetturaCodici += " AND Reg_Impianti_Codici.progetto_cod = 0  " &
                                       " AND (Reg_Impianti_Codici.id_cod < 2000 OR Reg_Impianti_Codici.id_cod >= 3000 ) "


                DtCodici = objCodici.Leggi(CStr(xPiva),
                                           CInt(xSa_Cod),
                                           CInt(xAppezza),
                                           CInt(xId_Imp),
                                            "",
                                           CInt(Id_Cod),
                                           CStr(Val_Cod),
                                           AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                            FiltroLetturaCodici,
                                           "",
                                           objParametri_Server)

                StrCodici2 = ""


                If Not DtCodici Is Nothing AndAlso DtCodici.Rows.Count > 0 Then



                    Dim Dr As DataRow

                    If (IsNothing(HttpContext.Current.Session("dt_Codici_Ana"))) Then
                        Dt2.Columns.Add(New DataColumn("Id_Cod", GetType(Integer)))
                        Dt2.Columns.Add(New DataColumn("Descrizione", GetType(String)))
                        Dt2.Columns.Add(New DataColumn("Val_Cod", GetType(String)))
                        Dt2.Columns.Add(New DataColumn("Validita_Inizio", GetType(String)))
                        Dt2.Columns.Add(New DataColumn("Validita_Fine", GetType(String)))
                    Else
                        Dt2 = HttpContext.Current.Session("dt_Codici_Ana")
                    End If

                    ''--------------------------------------------------
                    ''filtro i codici relativi all'impianto...
                    ''FILTRO i codici CHIAVE DEI CLIENTI!!!!!!!!!!!!!!!
                    ''--------------------------------------------------
                    'Dr = DtCodici.Select("(progetto_cod = 0 AND id_cod < 2000) OR (progetto_cod = 0 AND id_cod >= 3000)")

                    'If Not Dr Is Nothing AndAlso Dr.Length > 0 Then

                    'For i = 0 To Dr.Length - 1
                    For i = 0 To DtCodici.Rows.Count - 1

                        'If (Not IsDBNull(Dr(i).Item("val_cod"))) Then
                        If (Not IsDBNull(DtCodici.Rows(i).Item("val_cod"))) Then

                            Id_Cod = DtCodici.Rows(i).Item("id_cod")
                            Val_Cod = DtCodici.Rows(i).Item("val_cod")
                            CodiceAnagrafeDes = DtCodici.Rows(i).Item("descrizione")

                            'Genero l'XML del singolo nodo
                            Call XML_Codice(enum_CodificaDecodifica.Codifica,
                                            StrCodice,
                                            enum_TipoOperazioneDB.Cancellazione,
                                            Id_Cod,
                                            Val_Cod,
                                            CDate("01/01/1900"),
                                            CDate("31/12/2100"),
                                            BaseCode,
                                            TopCode,
                                            "Impianto")

                            'Inserisco l'XML nella stringa complessiva
                            StrCodici2 = StrCodici2 & StrCodice

                            Select Case Id_Cod

                            'Case enum_CodiciAnagrafe.Impianto_Ibrido
                            '    If Val_Cod = 0 And RsImpianti.Fields("grva_cod_veg").Value >= 0 Then
                            '        Me.ChkVarietaIbrida.Checked = False
                            '        Me.LblMaschio.Visible = False
                            '        Me.LblFemmina.Visible = False
                            '        Me.Pannello_Maschio.Visible = True
                            '        Me.Pannello_Femmina.Visible = False
                            '    Else
                            '        Me.ChkVarietaIbrida.Checked = True
                            '        Me.LblMaschio.Visible = True
                            '        Me.LblFemmina.Visible = True
                            '        Me.Pannello_Maschio.Visible = True
                            '        Me.Pannello_Femmina.Visible = True
                            '    End If

                                Case enum_CodiciAnagrafe.Impianto_CodiceB_Maschio
                                    Me.Txt_CodBMBDBT_M.Text = Val_Cod

                                Case enum_CodiciAnagrafe.Impianto_CodiceB_Femmina
                                    Me.Txt_CodBMBDBT_F.Text = Val_Cod

                                Case enum_CodiciAnagrafe.Impianto_Genetica_Maschio
                                    Me.Txt_Genetica_M.Text = Val_Cod

                                Case enum_CodiciAnagrafe.Impianto_Genetica_Femmina
                                    Me.Txt_Genetica_F.Text = Val_Cod

                                Case enum_CodiciAnagrafe.Impianto_OffType_Maschio
                                    Me.Txt_OffType_M.Text = Val_Cod

                                Case enum_CodiciAnagrafe.Impianto_OffType_Femmina
                                    Me.Txt_OffType_F.Text = Val_Cod

                                Case enum_CodiciAnagrafe.Impianto_TraFila_Maschio
                                    Me.Txt_DistanzaTraFila_M.Text = Val_Cod

                                Case enum_CodiciAnagrafe.Impianto_TraFila_Femmina
                                    Me.Txt_DistanzaTraFila_F.Text = Val_Cod

                                Case enum_CodiciAnagrafe.Impianto_SuFila_Maschio
                                    Me.Txt_DistanzaSuFila_M.Text = Val_Cod

                                Case enum_CodiciAnagrafe.Impianto_SuFila_Femmina
                                    Me.Txt_DistanzaSuFila_F.Text = Val_Cod

                                Case enum_CodiciAnagrafe.Impianto_Interbina
                                    If Val_Cod <> "0" Then
                                        Me.ChkFilaBinata.Checked = True
                                        Me.Txt_Interbina.Enabled = True
                                        Me.Txt_Interbina.Text = Val_Cod
                                    End If

                                Case enum_CodiciAnagrafe.Impianto_Germinabilita
                                    Me.Txt_Germinabilita.Text = Val_Cod

                                Case 3000 To 3999

                                    Me.Cmb_CodiciTerreno.SelectedIndex =
                                        Cmb_CodiciTerreno.Items.IndexOf(Cmb_CodiciTerreno.Items.FindByValue(
                                            Id_Cod.ToString))

                                'Case enum_CodiciAnagrafe.Impianto_Taglio_Tuberi_Patate

                                '    Me.Cmb_TagliatoTuberi.SelectedIndex = _
                                '        Cmb_TagliatoTuberi.Items.IndexOf(Cmb_TagliatoTuberi.Items.FindByValue( _
                                '                                         Val_Cod))

                                'Case enum_CodiciAnagrafe.Impianto_Parti_Tuberi_Patate

                                '    Txt_PartiTuberi.Text = Val_Cod

                                Case enum_CodiciAnagrafe.Dettaglio_Specie_Personalizzato

                                    Me.Cmb_DettaglioVarietaPersonalizzato.SelectedIndex =
                                        Cmb_DettaglioVarietaPersonalizzato.Items.IndexOf(Cmb_DettaglioVarietaPersonalizzato.Items.FindByValue(
                                            Val_Cod))

                                Case Else

                                    'se nella stringa contenente i codici restituita dal componente è presente..
                                    If InStr(StrCodiciImpianto, Id_Cod.ToString) <> 0 Then

                                        'ListCodici.Items.Add(New ListItem(CodiceAnagrafeDes & " = " & Val_Cod, _
                                        '                                  Id_Cod.ToString))

                                        'Creo una nuova riga
                                        Dr = Dt2.NewRow


                                        Dr.Item("Id_Cod") = Id_Cod
                                        'Dr.Item("Descrizione") = objCodiceAnagrafeR.CodiceAnagrafeDes_from_CodiceAnagrafeCod(codice_id, HttpContext.Current.Session("ASG_objParametri_Server"))
                                        Dr.Item("Descrizione") = CodiceAnagrafeDes
                                        Dr.Item("Val_Cod") = Val_Cod

                                        Dr.Item("Validita_Inizio") = DtCodici.Rows(i).Item("Validita_Inizio")

                                        Dr.Item("Validita_Fine") = DtCodici.Rows(i).Item("Validita_Fine")

                                        'Associo alla tabella la nuova riga creata
                                        Dt2.Rows.Add(Dr)


                                    End If

                            End Select

                        End If

                    Next


                    'End If

                    HttpContext.Current.Session("dt_Codici_Ana") = Dt2



                    DtCodici = Nothing

                End If 'DtCodici

                Session("StrXmlCodiciAttuali") = StrCodici2


            Else

                '????? TO CHECK if is for creation 
                Cmb_Specie.SelectedIndex =
                    Cmb_Specie.Items.IndexOf(Cmb_Specie.Items.FindByValue(
                    HttpContext.Current.Session("cmb_specie")))
                Cmb_Finalita.SelectedIndex =
                    Cmb_Finalita.Items.IndexOf(Cmb_Finalita.Items.FindByValue(
                    HttpContext.Current.Session("cmb_finalita")))


                Id_Consociazione = 0

                'Salva_Distinta()

            End If 'RegImpianti


            jsCodici = DT_to_Json_Codici(Dt2)

            '--------------------------------------------------------------------------------------------------------------------------------------------
            'Controllo, se sono già state registrate Operazioni d'Agenda
            'che le date delle operazioni nn siano esterne alle date scelte x l'impianto 
            '--------------------------------------------------------------------------------------------------------------------------------------------

            Dim Validita_Inizio_Agenda As Date
            Dim Validita_Fine_Agenda As Date

            Dim MovimentiPresenti As Boolean

            Dim Hash_IdAgenda_ToFlag As New Hashtable
            MovimentiPresenti = False

            If Operazione = enum_TipoOperazioneDB.Modifica Then

                Dim ObjAgenda As New AgronicaCoreContabDAL.Mov_Destinazioni_R  'New Agro_Contab_AD.Mov_Destinazioni_R
                Dim DTAgenda As DataTable
                Dim Id_Agenda As Integer

                'ricavo il recordset dei movimenti di produzione associati all'impianto
                DTAgenda = ObjAgenda.LeggiCronologiaMovimenti(xPiva,
                                                            xSa_Cod,
                                                            xAppezza,
                                                            xId_Imp,
                                                              AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                            "",
                                                            "",
                                                            objParametri_Server)


                ObjAgenda = Nothing

                If DTAgenda.Rows.Count > 0 Then

                    MovimentiPresenti = True

                    Validita_Inizio_Agenda = DTAgenda.Rows(0).Item("Data_Movimento")

                    Validita_Fine_Agenda = DTAgenda.Rows(DTAgenda.Rows.Count - 1).Item("Data_Movimento")

                    '(24/07/2015) eliminato perchè spostato nel core di scrittura
                    ''se ho modificato la superficie, mi salvo gli id_agenda delle operazioni da flaggare
                    'If viewstate("FlagSupModificata") = True Then
                    '    Dim i As Integer
                    '    For i = 0 To DTAgenda.Rows.Count - 1
                    '        Id_Agenda = DTAgenda.Rows(i).Item("Id_agenda")
                    '        If Not Hash_IdAgenda_ToFlag.Contains(Id_Agenda) Then
                    '            Hash_IdAgenda_ToFlag.Add(Id_Agenda, Qs_Piva)
                    '        End If
                    '    Next
                    'End If

                End If

                If MovimentiPresenti = True Then

                    'If Validita_Inizio_Agenda < Validita_Inizio Then
                    '    MessaggioErrore += "   - Non è possibile inserire l'impianto nell'intervallo scelto, " & vbCrLf & _
                    '                    "     poichè sono state registrate Operazioni d'Agenda dal " & CDate(Validita_Inizio_Agenda).ToShortDateString & " al " & CDate(Validita_Fine_Agenda).ToShortDateString & vbCrLf
                    'End If

                    'If Validita_Fine_Agenda > Validita_Fine Then
                    '    MessaggioErrore += "   - Non è possibile inserire l'impianto nell'intervallo scelto, " & vbCrLf & _
                    '                    "     poichè sono state registrate Operazioni d'Agenda dal " & CDate(Validita_Inizio_Agenda).ToShortDateString & " al " & CDate(Validita_Fine_Agenda).ToShortDateString & vbCrLf
                    'End If

                    ChkTerrenoNudo.Enabled = False
                    Cmb_Specie.Enabled = False
                    Cmb_Finalita.Enabled = False

                End If

            End If


            CmbCodice.Enabled = False
            TxtCodiceValore.Enabled = False

            Cmb_Particella.Enabled = False
            TxtParticellaValore.Enabled = False


        End If


        If Operazione = enum_TipoOperazioneDB.Lettura Then
            ChkTerrenoNudo.Enabled = False
            Cmb_CodiciTerreno.Enabled = False
            Cmb_Specie.Enabled = False
            Cmb_Finalita.Enabled = False
            Cmb_Cultivar.Enabled = False
            TxtValiditaInizio.Enabled = False
            TxtValiditaFine.Enabled = False
            TxtSuperficie.Enabled = False

            Cmb_TipologiaVarietale.Enabled = False
            ChkConsociazione.Enabled = False
            ChkCoverCrops.Enabled = False
            ChkMonitorato.Enabled = False
            Cmb_ImpIrrigazione.Enabled = False
            Cmb_Portinnesto.Enabled = False
            Cmb_SeminaTrapianto.Enabled = False
            Cmb_ProvenienzaSeme.Enabled = False
            TxtSuperficie2.Enabled = False
            Txt_DistanzaSuFila_M.Enabled = False
            Txt_DistanzaTraFila_M.Enabled = False
            ChkFilaBinata.Enabled = False
            Txt_Interbina.Enabled = False
            Txt_Germinabilita.Enabled = False
            TxtPianteHa.Enabled = False
            TxtPianteImpianto.Enabled = False
            Cmb_Copertura.Enabled = False
            Cmb_Finalita.Enabled = False
            Cmb_Cultivar.Enabled = False
            TxtCopDataInizio.Enabled = False
            TxtCopDataFine.Enabled = False
            Cmb_DettaglioVarietaPersonalizzato.Enabled = False
            ChkVarietaIbrida.Enabled = False
            Txt_CodBMBDBT_M.Enabled = False
            Txt_Genetica_M.Enabled = False
            Txt_OffType_M.Enabled = False
            Txt_CodBMBDBT_F.Enabled = False
            Txt_Genetica_F.Enabled = False
            Txt_OffType_F.Enabled = False
            Txt_DistanzaSuFila_F.Enabled = False
            Txt_DistanzaTraFila_F.Enabled = False
            btn_nuova_distinta.Visible = False

        End If

    End Sub

    Private Function Verifica_Esistenza_Impianti_Consociati(ByRef pId_Consociazione As Integer) As Boolean

        Dim objRegImp As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim DtRegImp As DataTable

        Dim InizioImpianto As Date

        If IsDate(TxtValiditaInizio.Text) Then
            InizioImpianto = CDate(TxtValiditaInizio.Text)
        Else
            InizioImpianto = AGRODATAINIZIO
        End If

        'Verifico se ci sono già altri impianti attivi e consociati nell'appezzamento
        DtRegImp = objRegImp.Leggi(xPiva,
                                   xSa_Cod,
                                   xAppezza,
                                   0,
                                   enumSelezioneVariabile.Selezione_TabellaCompleta,
                                   " Validita_Fine >= " +
                                   AgronicaCoreDataProvider.UtilityProvider.Agro_SQL_SaveDate(InizioImpianto) +
                                   " AND Id_Consociazione <> 0",
                                   "", objParametri_Server)

        If Not DtRegImp Is Nothing AndAlso DtRegImp.Rows.Count > 0 Then

            Id_Consociazione = CInt(DtRegImp.Rows(0).Item("Id_Consociazione"))
            Return True

        Else

            Id_Consociazione = 0
            Return False

        End If

    End Function


    '########################################################################################
    Private Sub ImgBtn_Cancella_Distinta_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Cancella_Distinta.Click

        'Dim Connessione As OleDb.OleDbConnection
        '  Dim Transazione As OleDb.OleDbTransaction
        Dim ErrMSG As String

        Dim StringaXmlCancellazione As String
        Dim BoolDummy As Boolean = False


        Try

            Dim objProgetto_W As New AgronicaCoreAnagrafeBIZ.Progetto_W
            Dim objProgetto_R As New AgronicaCoreAnagrafeBIZ.Progetto_R




            objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(AGRODATAINIZIO, AGRODATAFINE)
            StringaXmlCancellazione = objProgetto_R.Impresa_Progetti_Leggi(
                                              CStr(xPiva),
                                              CInt(HttpContext.Current.Session("progetto_cod")),
                                              "",
                                              0,
                                              0,
                                              0,
                                              0,
                                              0, 0,
                                              True,
                                              objParametri_Server)


            ' APRO LA TRANSAZIONE
            '   Transazione = Connessione.BeginTransaction()

            'BoolDummy = objProgetto_W.Impresa_Progetto_Scrivi( _
            '                                    CStr(StringaXmlCancellazione), _
            '                                    Nothing, _
            '                                    Nothing, _
            '                                    CStr(Session("ASG_SuperUser_CodFiscale")), _
            '                                    CStr(Session("ASG_Utente_CodFiscale")), _
            '                                    #1/1/1900#, _
            '                                    #12/31/2100#, _
            '                                    1, 0, _
            '                                    Connessione, _
            '                                    Transazione, _
            '                                    Nothing, _
            '                                    CStr(Session("ASG_AgronicaCore_DirectoryLOG")), _
            '                                    NomeFileLog_AgronicaCore, _
            '                                    CStr(Session("ASG_Utente_CodFiscale")))

            Dim outPiva As String
            Dim outCod As Integer

            BoolDummy = objProgetto_W.Impresa_Progetto_Scrivi(
                                            CStr(StringaXmlCancellazione),
                                               outPiva,
                                                outCod,
                                               objParametri_Server)

            objParametri_Server.ResettaFinestra()


            'objProgetto_W = Nothing

            '=================================
            '======== TRANSAZIONE OK =========
            ' Transazione.Commit()
            ' Connessione.Close()

            ' Transazione = Nothing
            '  Connessione = Nothing
            '=================================

            BoolDummy = True
            HttpContext.Current.Session("progetto_cod") = ""

        Catch ex As Exception

            '------------------------------------------------
            'Si e' verificata una eccezione !!!!!!
            '------------------------------------------------

            'Faccio il rollback della transazione
            'If Not Transazione Is Nothing Then
            '    Transazione.Rollback()
            '    Transazione = Nothing
            'End If

            ''Chiudo la connessione se è apertta
            'If (Not Connessione Is Nothing) Then
            '    Connessione.Close()
            '    Connessione = Nothing
            'End If


            'Messaggio di errore
            ErrMSG = "Si e' verificato un'errore : " &
                        Chr(13) &
                        ex.Message.ToString()

            'Visualizzo il messaggio di errore
            Call Messaggi.AgroMsgBox(ErrMSG, Page)

            '------------------------------------------------

        End Try
        ''Ricarico la pagina
        'If BoolDummy = True Then
        '    Response.Redirect("Edit_Impianto_2.aspx?P=" & Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) & _
        '                      "&o=" & Stringa_Codifica(Qs_Operazione, AgroKey_EncoderDecoder, Server) & _
        '                      "&k=" & Stringa_Codifica(Qs_Key, AgroKey_EncoderDecoder, Server))
        'End If

    End Sub

    '########################################################################################
    Private Sub ImgBtn_SalvaTDistinta_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_SalvaDistinta.Click

        Salva_Distinta()

    End Sub


    '########################################################################################
    Private Sub Salva_Distinta()

        Dim ErrMSG As String

        Dim i As Integer
        Dim Messaggio As String
        Dim StringaDistinta As String
        Dim strDummy As String
        Dim BooDummy As Boolean


        Dim ValiditaInizio_Impianto As Date
        Dim ValiditaFine_Impianto As Date


        Dim contr As AgronicaControlli_2010.SalvataggioWebControl = New AgronicaControlli_2010.SalvataggioWebControl


        '--- Controllo NOME lotto

        'If Me.Txt_Lotto.Text = "" Then
        '    AgroMsgBox("Inserire il Lotto!", Page)
        '    Exit Sub
        'End If

        ''--- Controllo Organismo Referente

        'If Esiste_Obbligo_SalvataggioOrganismoReferente(Server, Session, Page) = True Then

        '    If Not IsNothing(Me.Cmb_OrganismoReferente.SelectedItem) Then
        '        If Me.Cmb_OrganismoReferente.SelectedItem.Text = "" Then
        '            AgroMsgBox("E' obbligatorio impostare l'Organismo Referente!", Page)
        '            Exit Sub
        '        End If
        '    Else
        '        AgroMsgBox("E' obbligatorio impostare l'Organismo Referente!", Page)
        '        Exit Sub
        '    End If

        'End If


        '--- Controllo date

        If Not IsDate(Txt_ValiditaInizio_Distinta.Text) Then
            xValiditaInizio = AGRODATAINIZIO
        Else
            xValiditaInizio = CDate(Txt_ValiditaInizio_Distinta.Text)
        End If

        If Not IsDate(Txt_ValiditaFine_Distinta.Text) Then
            xValiditaFine = AGRODATAFINE
        Else
            xValiditaFine = CDate(Txt_ValiditaFine_Distinta.Text)
        End If

        'If Not IsDate(TxtValiditaInizio.Text) Then
        '    ValiditaInizio_Impianto = AGRODATAINIZIO
        'Else
        '    ValiditaInizio_Impianto = CDate(TxtValiditaInizio.Text)
        'End If

        'If Not IsDate(TxtValiditaFine.Text) Then
        '    ValiditaFine_Impianto = AGRODATAFINE
        'Else
        '    ValiditaFine_Impianto = CDate(TxtValiditaFine.Text)
        'End If


        'If Operazione = enum_TipoOperazioneDB.Scrittura Then

        '    'If xValiditaInizio < ValiditaInizio_Impianto Then
        '    '    xValiditaInizio = ValiditaInizio_Impianto
        '    'End If
        '    'If xValiditaFine > ValiditaFine_Impianto Then
        '    '    xValiditaFine = ValiditaFine_Impianto
        '    'End If
        '    'If xValiditaInizio < ValiditaInizio_Appezzamento Then
        '    '    xValiditaInizio = ValiditaInizio_Appezzamento
        '    'End If
        '    'If xValiditaFine > ValiditaFine_Appezzamento Then
        '    '    xValiditaFine = ValiditaFine_Appezzamento
        '    'End If

        'Else
        '    'If xValiditaInizio < ValiditaInizio_Impianto Then
        '    '    AgroMsgBox("La data di inizio esercizio non può essere antecedente alla data di inizio Impianto!", Page)
        '    '    Exit Sub
        '    'End If

        '    'If xValiditaFine > ValiditaFine_Impianto Then
        '    '    AgroMsgBox("La data di fine esercizio non può essere posteriore alla data di fine Impianto!", Page)
        '    '    Exit Sub
        '    'End If

        '    'If xValiditaInizio < CDate(InizioAppezzamento.Value) Then
        '    '    AgroMsgBox("La data di inizio esercizio non può essere antecedente alla data di inizio Appezzamento (" & InizioAppezzamento.Value & ")!", Page)
        '    '    Exit Sub
        '    'End If

        '    'If xValiditaFine > CDate(FineAppezzamento.Value) Then
        '    '    AgroMsgBox("La data di fine esercizio non può essere posteriore alla data di fine Appezzamento (" & FineAppezzamento.Value & ")!", Page)
        '    '    Exit Sub
        '    'End If

        '    ''controllo validita date
        '    'If Esistono_Distinte_Su_Impianto(Server, Session, Page, _
        '    '                                 Me.Lbl_Piva.Text, _
        '    '                                 CInt(Me.Lbl_SaCod.Text), _
        '    '                                 CInt(Me.Lbl_Appezza.Text), _
        '    '                                 CInt(Me.Lbl_IdReg.Text), _
        '    '                                 CInt(Me.Lbl_ProgettoCod.Text), _
        '    '                                 xValiditaInizio, _
        '    '                                 xValiditaFine) = True Then
        '    '    AgroMsgBox("Nell'intervallo di tempo impostato esiste già un esercizio!", Page)
        '    '    Exit Sub
        '    'End If

        'End If



        '--- Apporti massimi di macroelementi


        If InStr(TxtN.Text, ".") <> 0 Then
            TxtN.Text = Replace(TxtN.Text, ".", ",")
        End If

        If InStr(TxtP2O5.Text, ".") <> 0 Then
            TxtP2O5.Text = Replace(TxtP2O5.Text, ".", ",")
        End If

        If InStr(TxtK2O.Text, ".") <> 0 Then
            TxtK2O.Text = Replace(TxtK2O.Text, ".", ",")
        End If

        If InStr(TxtMgO.Text, ".") <> 0 Then
            TxtMgO.Text = Replace(TxtMgO.Text, ".", ",")
        End If




        ''controllo i parametri inseriti
        'For i = 0 To DataGridFasi.Items.Count - 1

        '    If CType(DataGridFasi.Items(i).FindControl("TxtBudget"), TextBox).Text = "" Then

        '        CType(DataGridFasi.Items(i).FindControl("TxtBudget"), TextBox).Text = "0,00"
        '    Else

        '        If Not IsNumeric(CType(DataGridFasi.Items(i).FindControl("TxtBudget"), TextBox).Text) Then
        '            Messaggio = "Il Budget deve essere un valore numerico."
        '            AgroMsgBox(Messaggio, Page)
        '            Exit Sub
        '        End If

        '        If InStr(CType(DataGridFasi.Items(i).FindControl("TxtBudget"), TextBox).Text, ".") Then
        '            CType(DataGridFasi.Items(i).FindControl("TxtBudget"), TextBox).Text = Replace( _
        '            CType(DataGridFasi.Items(i).FindControl("TxtBudget"), TextBox).Text, ".", ",")
        '        End If

        '    End If

        'Next


        ''controllo i parametri inseriti nel DataGridLavorati
        'For i = 0 To DataGridLavorati.Items.Count - 1

        '    If CType(DataGridLavorati.Items(i).FindControl("TxtBudget"), TextBox).Text = "" Then

        '        CType(DataGridLavorati.Items(i).FindControl("TxtBudget"), TextBox).Text = "0,00"
        '    Else

        '        If Not IsNumeric(CType(DataGridLavorati.Items(i).FindControl("TxtBudget"), TextBox).Text) Then
        '            Messaggio = "Il Budget deve essere un valore numerico."
        '            AgroMsgBox(Messaggio, Page)
        '            Exit Sub
        '        End If

        '        If InStr(CType(DataGridLavorati.Items(i).FindControl("TxtBudget"), TextBox).Text, ".") Then
        '            CType(DataGridLavorati.Items(i).FindControl("TxtBudget"), TextBox).Text = Replace( _
        '            CType(DataGridLavorati.Items(i).FindControl("TxtBudget"), TextBox).Text, ".", ",")
        '        End If

        '    End If

        'Next

        'controllo i valori inseriti nelle textbox
        If Txt_ResaPrevista.Text = "" Then

            Txt_ResaPrevista.Text = "0,00"
        Else

            If InStr(Txt_ResaPrevista.Text, ".") Then
                Txt_ResaPrevista.Text = Replace(Txt_ResaPrevista.Text, ".", ",")
            End If

        End If

        '*** ???
        ''controllo i valori inseriti nelle textbox
        'If Txt_RicaviPrevisti.Text = "" Then

        '    Txt_RicaviPrevisti.Text = "0,00"
        'Else

        '    If InStr(Txt_RicaviPrevisti.Text, ".") Then
        '        Txt_RicaviPrevisti.Text = Replace(Txt_RicaviPrevisti.Text, ".", ",")
        '    End If

        'End If

        'controllo i valori inseriti nelle textbox
        If Me.TxtPianteHa.Text = "" Then
            TxtPianteHa.Text = "0"
        Else

            If InStr(TxtPianteHa.Text, ".") Then
                TxtPianteHa.Text = Replace(TxtPianteHa.Text, ".", ",")
            End If
        End If




        Dim Flag_Redirect As Boolean = False
        Dim Flag_SalvaTutto As Boolean = False

        'apro la connessione e transazione 
        If objParametri_Server.objTransazione Is Nothing Then
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)
        Else
            Flag_SalvaTutto = True
        End If

        Try

            ''--------------------------------------------------------------------------------------
            ''se sono in modifica cancello prima tutti CODICI - PARTICELLE associati alla distinta
            'If Me.Lbl_ProgettoCod.Text <> "0" Then

            '    Dim objCodici_W As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_W

            '    objCodici_W.CancellaxProgetto(CStr(Me.Lbl_Piva.Text), _
            '                                CInt(Me.Lbl_SaCod.Text), _
            '                                CInt(Me.Lbl_Appezza.Text), _
            '                                CInt(Me.Lbl_IdReg.Text), _
            '                                CInt(Me.Lbl_ProgettoCod.Text), _
            '                                CInt(0), _
            '                                "", _
            '                                objParametri_Server)


            '    objCodici_W = Nothing

            '    Dim objParticelle_W As New AgronicaCoreAnagrafeDAL.ProgettixParticelle_W

            '    objParticelle_W.CancellaxProgetto(CStr(Me.Lbl_Piva.Text), _
            '                                        CInt(Me.Lbl_SaCod.Text), _
            '                                        CInt(Me.Lbl_Appezza.Text), _
            '                                        CInt(Me.Lbl_IdReg.Text), _
            '                                        CInt(Me.Lbl_ProgettoCod.Text), _
            '                                        "", "", "", 0, 0, "", _
            '                                        "", _
            '                                        objParametri_Server)

            '    objParticelle_W = Nothing

            'End If


            Dim objProgetto As New AgronicaCoreAnagrafeBIZ.Progetto_W

            '===============================================
            '=========== XML DISTINTA ==================
            '===============================================
            StringaDistinta = Xml_GeneraStringoneDistinta()


            '==============================================
            '=========== SCRITTURA DISTINTA ================
            '===============================================
            BooDummy = objProgetto.Impresa_Progetto_Scrivi(
                                                CStr(StringaDistinta),
                                                Nothing,
                                                Nothing,
                                                objParametri_Server)

            objProgetto = Nothing

            'chiudo la transazione
            If Flag_SalvaTutto = False Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server)
                'chiudo la connessione 
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)
            End If
            '=================================

            'Attiva_Pannello(enum_Pannello.Pannello_Generale, False)
            'Me.ImgBtn_Modifica_Distinta.Visible = True
            'Me.ImgBtn_Salva_Distinta.Visible = False



            Flag_Redirect = True
        Catch exc As Exception

            '------------------------------------------------
            'Si e' verificata una eccezione !!!!!!
            '------------------------------------------------

            'Faccio il rollback della transazione
            'If Not Transazione Is Nothing Then
            '    Transazione.Rollback()
            '    Transazione = Nothing
            'End If

            ''Chiudo la connessione se è apertta
            'If (Not Connessione Is Nothing) Then
            '    Connessione.Close()
            '    Connessione = Nothing
            'End If

            'chiudo la transazione con il rollback
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            'chiudo la connessione
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)

            'Messaggio di errore
            Messaggio = "Si e' verificato un'errore : " &
                        Chr(13) &
                        exc.Message.ToString()

            'Visualizzo il messaggio di errore
            Call Messaggi.AgroMsgBox(Messaggio, Page)

            '------------------------------------------------

        End Try


        'If Flag_SalvaTutto = False Then
        '    If Flag_Redirect = True Then
        '        If Qs_Operazione <> enum_TipoOperazioneDB.Scrittura Then

        '            'Ricarico la pagina
        '            Response.Redirect("Edit_Impianto_2.aspx?P=" & Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) & _
        '                              "&o=" & Stringa_Codifica(Qs_Operazione, AgroKey_EncoderDecoder, Server) & _
        '                              "&k=" & Stringa_Codifica(Qs_Key, AgroKey_EncoderDecoder, Server))

        '        End If
        '    End If
        'End If

        If Flag_SalvaTutto = False Then
            If Flag_Redirect = True Then

                'Ricarico la pagina
                Response.Redirect("Impianto_Edit.aspx")
            End If
        End If


    End Sub


    '###################################################################################################
    Private Function Xml_GeneraStringoneDistinta() As String

        Dim i, j, Indice As Integer
        Dim Start As Integer

        'Dim xPiva As String
        'Dim xSa_Cod As Integer
        'Dim xAppezza As Integer
        'Dim xId_Imp As Integer

        'Dim xValiditaInizio As Date
        'Dim xValiditaFine As Date
        Dim ValiditaInizio_Impianto As Date
        Dim ValiditaFine_Impianto As Date
        Dim xInizioPrevisto As Date
        Dim xFinePrevista As Date
        Dim xRicaviPrevisti As Double
        Dim xProduzionePrevista As Double
        Dim Data_Semina_Prevista, Data_Fioritura_Prevista, Data_Raccolta_Prevista As Date

        Dim str_Progetto As String
        Dim str_ProgettoFase As String
        Dim str_PrezzoUnitario As String
        Dim strDescrizioneProgetto As String
        Dim strNomeProgetto As String

        Dim str_DatiCodici As String
        Dim str_CodiceImpianto As String

        Dim str_DatiParticelle As String
        Dim str_Particella As String

        Dim Id_Cod As Integer
        Dim Val_Cod As String

        Dim Valore As String
        Dim Prov As String
        Dim Com As String
        Dim Sezione As String
        Dim Foglio As Integer
        Dim Numero As Integer
        Dim Subalterno As String
        Dim ArrayParticella() As String

        Dim Cod_Progetto As Integer

        Dim P_Ha As Double

        Dim Operazione As enum_TipoOperazioneDB

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlDoc2 As New System.Xml.XmlDocument
        Dim XML_DatiProgetto As System.Xml.XmlElement
        Dim XML_Progetto As System.Xml.XmlElement
        Dim XML_Progetto_Fase As System.Xml.XmlElement
        Dim XML_DatiCodici As System.Xml.XmlElement
        Dim XML_CodiceImpianto As System.Xml.XmlElement
        Dim XMLs_CodiceImpianto As System.Xml.XmlNodeList
        Dim XML_DatiParticelle As System.Xml.XmlElement
        Dim XML_Particella As System.Xml.XmlElement
        Dim XMLs_Particelle As System.Xml.XmlNodeList

        'recupero dal viewstate le chiavi dell'impianto selezionato

        'xPiva = Me.Lbl_Piva.Text
        'xSa_Cod = CInt(Me.Lbl_SaCod.Text)
        'xAppezza = CInt(Me.Lbl_Appezza.Text
        'xId_Imp = CInt(Me.Lbl_IdReg.Text)
        'Cod_Progetto = CInt(Me.Lbl_ProgettoCod.Text)

        If IsNothing(HttpContext.Current.Session("Distinta_corrente")) Then
            Cod_Progetto = 0
        Else
            Dim dt As New DataTable

            dt = HttpContext.Current.Session("Distinta_corrente")
            Cod_Progetto = dt.Rows(0).Item("Progetto_Cod")

        End If


        '------------------------------------------------
        '----- Calcolo i valori di BaseCode e TopCode
        '------------------------------------------------

        Call Calcola_BaseCode_TopCode(BaseCode,
                                      TopCode,
                                      Session("ASG_ProgressivoGIAS"))

        'controllo date...
        'se nn sono state impostate x la distinta..gli assegno quelle dell'impianto!!

        If IsDate(Me.Txt_ValiditaInizio_Distinta.Text) = True Then
            xValiditaInizio = Me.Txt_ValiditaInizio_Distinta.Text
        Else
            xValiditaInizio = AGRODATAINIZIO
        End If

        If IsDate(Me.Txt_ValiditaFine_Distinta.Text) = True Then
            xValiditaFine = Me.Txt_ValiditaFine_Distinta.Text
        Else
            xValiditaFine = AGRODATAFINE
        End If

        If Not IsDate(TxtValiditaInizio.Text) Then
            ValiditaInizio_Impianto = AGRODATAINIZIO
        Else
            ValiditaInizio_Impianto = CDate(TxtValiditaInizio.Text)
        End If

        If Not IsDate(TxtValiditaFine.Text) Then
            ValiditaFine_Impianto = AGRODATAFINE
        Else
            ValiditaFine_Impianto = CDate(TxtValiditaFine.Text)
        End If



        If (IsDate(Me.TxtValiditaInizio.Text) = True) And (Not IsDate(Me.Txt_ValiditaInizio_Distinta.Text) = True) Then
            xValiditaInizio = CDate(Me.TxtValiditaInizio.Text)
        End If
        If (IsDate(Me.TxtValiditaFine.Text) = True) And (Not IsDate(Me.Txt_ValiditaFine_Distinta.Text) = True) Then
            xValiditaFine = CDate(Me.TxtValiditaFine.Text)
        End If


        ''se nn sono state impostate x la distinta..gli assegno quelle dell'impianto!!
        'If Me.Txt_RicaviPrevisti.Text <> "" Then
        '    xRicaviPrevisti = Me.Txt_RicaviPrevisti.Text
        'Else
        '    xRicaviPrevisti = 0
        'End If

        ' Paolo: In attesa di specifiche
        xRicaviPrevisti = 0


        If Me.Txt_ResaPrevista.Text <> "" Then
            xProduzionePrevista = Me.Txt_ResaPrevista.Text
        Else
            xProduzionePrevista = 0
        End If


        If Me.Txt_Lotto.Text = "" Then

            If Me.ChkTerrenoNudo.Checked = False Then

                strNomeProgetto = Me.LblAppezza.Text & " - " &
                                         Cmb_Specie.SelectedItem.Text & " - " &
                                         Cmb_Cultivar.SelectedItem.Text
            Else
                strNomeProgetto = Me.LblAppezza.Text & " - Terreno Nudo"

            End If

        Else
            strNomeProgetto = Me.Txt_Lotto.Text
        End If


        If Me.ChkTerrenoNudo.Checked = False Then

            strDescrizioneProgetto = "Esercizio: " &
                                        Me.LblAppezza.Text & " - " &
                                        Cmb_Specie.SelectedItem.Text & " - " &
                                        Cmb_Cultivar.SelectedItem.Text
        Else
            strDescrizioneProgetto = "Esercizio: " &
                                      Me.LblAppezza.Text & " - Terreno Nudo"

        End If


        'If Me.Lbl_ProgettoCod.Text = "0" Then
        '    Operazione = enum_TipoOperazioneDB.Scrittura
        'Else
        '    Operazione = enum_TipoOperazioneDB.Modifica
        'End If
        If HttpContext.Current.Session("Operazione_Distinta") <> "" Then

            Operazione = HttpContext.Current.Session("Operazione_Distinta")

        End If


        If Me.TxtPianteHa.Text <> "" Then
            P_Ha = Me.TxtPianteHa.Text
        Else
            P_Ha = 0
        End If


        If Me.Txt_semina_prevista.Text <> "" Then
            Data_Semina_Prevista = Me.Txt_semina_prevista.Text
        Else
            Data_Semina_Prevista = AGRODATAINIZIO
        End If

        If Me.Txt_fioritura_prevista.Text <> "" Then
            Data_Fioritura_Prevista = Me.Txt_fioritura_prevista.Text
        Else
            Data_Fioritura_Prevista = AGRODATAINIZIO
        End If

        If Me.Txt_raccolta_prevista.Text <> "" Then
            Data_Raccolta_Prevista = Me.Txt_raccolta_prevista.Text
        Else
            Data_Raccolta_Prevista = AGRODATAFINE
        End If

        Dim Disciplinare_Cod As Integer = 1
        Dim Disciplinare_PubblicoPrivato As Integer = 0



        Dim Regolamento As Integer
        Dim Stato As Integer
        Dim Disciplinare As Integer
        Dim Reg_Concimazioni As Integer

        If HttpContext.Current.Session("Cmb_Regolamento") = "" Then

            'If Cmb_Regolamento.SelectedItem.Value = "" Then
            Regolamento = 1
        Else
            Regolamento = HttpContext.Current.Session("Cmb_Regolamento")
        End If


        If HttpContext.Current.Session("Cmb_Stato") = "" Then

            'If Cmb_Regolamento.SelectedItem.Value = "" Then
            Stato = 1
        Else
            Stato = HttpContext.Current.Session("Cmb_Stato")
        End If


        If HttpContext.Current.Session("Cmb_Disciplinare") <> "" Then
            Disciplinare = HttpContext.Current.Session("Cmb_Disciplinare")
            Disciplinare_Cod = Split(Disciplinare, "/")(0)
            Disciplinare_PubblicoPrivato = Split(Disciplinare, "/")(1)
        End If

        If HttpContext.Current.Session("Cmb_RegolamentoConc") = "" Then

            'If Cmb_Regolamento.SelectedItem.Value = "" Then
            Reg_Concimazioni = 0
        Else
            Reg_Concimazioni = HttpContext.Current.Session("Cmb_RegolamentoConc")
        End If




        Dim objXML As New AgronicaCoreXML.AnagrafeXML

        objXML.Xml_ProgettoPerImpianto(enum_CodificaDecodifica.Codifica,
                                                       str_Progetto,
                                                        CInt(Operazione),
                                                xPiva,
                                                xSa_Cod,
                                                Cod_Progetto,
                                                strNomeProgetto,
                                                strDescrizioneProgetto,
                                                CAU_PROGETTO_PRODUZIONE,
                                                0,
                                                0,
                                                Data_Semina_Prevista,
                                                Data_Raccolta_Prevista,
                                                "",
                                                xAppezza,
                                                xId_Imp,
                                                0,
                                                0,
                                                Stato,
                                                Regolamento,
                                                Disciplinare_Cod,
                                                Reg_Concimazioni,
                                                0,
                                                xRicaviPrevisti,
                                                xProduzionePrevista,
                                                CDate(xValiditaInizio),
                                                CDate(xValiditaFine),
                                                BaseCode,
                                                TopCode)


        XmlDoc.LoadXml(str_Progetto)

        XML_DatiProgetto = XmlDoc.SelectSingleNode("DatiProgetto")

        XML_Progetto = XML_DatiProgetto.SelectSingleNode("Progetto")

        '------------------------------------------------
        '----- Costruisco la stringa XML dei CODICI
        '------------------------------------------------

        XML_DatiCodici = XmlDoc.CreateElement("DatiCodici")

        XML_Progetto.AppendChild(XML_DatiCodici)

        str_DatiCodici = ""

        '----- Leggo gli i dati nei controlli e per ognuno genero un nodo 

        For j = 0 To 3

            Val_Cod = ""

            Select Case j

                Case 0
                    If Me.TxtN.Text <> "" Then
                        Id_Cod = enum_CodiciAnagrafe.Impianto_LimiteN
                        Val_Cod = Me.TxtN.Text
                    End If

                Case 1
                    If Me.TxtP2O5.Text <> "" Then
                        Id_Cod = enum_CodiciAnagrafe.Impianto_LimiteP
                        Val_Cod = Me.TxtP2O5.Text
                    End If

                Case 2
                    If Me.TxtK2O.Text <> "" Then
                        Id_Cod = enum_CodiciAnagrafe.Impianto_LimiteK
                        Val_Cod = Me.TxtK2O.Text
                    End If

                Case 3
                    If Me.TxtMgO.Text <> "" Then
                        Id_Cod = enum_CodiciAnagrafe.Impianto_LimiteMg
                        Val_Cod = Me.TxtMgO.Text
                    End If

            End Select

            If Val_Cod <> "" Then

                'Genero l'XML del singolo nodo solo se i codici sono valorizzati
                Call XML_Codice(enum_CodificaDecodifica.Codifica,
                                str_CodiceImpianto,
                                enum_TipoOperazioneDB.Scrittura,
                                Id_Cod,
                                Val_Cod,
                                CDate(xValiditaInizio),
                                CDate(xValiditaFine),
                                BaseCode,
                                TopCode,
                                "Impianto")

                'Inserisco l'XML nella stringa complessiva
                str_DatiCodici = str_DatiCodici & str_CodiceImpianto

            End If

        Next



        Dim Organismo As String

        If HttpContext.Current.Session("Cmb_OrganismoReferente") = "" Then

            'If Cmb_Regolamento.SelectedItem.Value = "" Then
            Organismo = ""
        Else
            Organismo = HttpContext.Current.Session("Cmb_OrganismoReferente")
        End If


        'Genero l'XML del nodo Organismo referente
        If Organismo <> "" Then

            Id_Cod = enum_CodiciAnagrafe.Organismo_Referente
            Val_Cod = Organismo

            'Genero l'XML del singolo nodo
            Call XML_Codice(enum_CodificaDecodifica.Codifica,
                            str_CodiceImpianto,
                            enum_TipoOperazioneDB.Scrittura,
                            Id_Cod,
                            Val_Cod,
                            CDate(xValiditaInizio),
                            CDate(xValiditaFine),
                            BaseCode,
                            TopCode,
                            "Impianto")

            'Inserisco l'XML nella stringa complessiva
            str_DatiCodici = str_DatiCodici & str_CodiceImpianto

        End If


        Dim Magazzino As String

        If HttpContext.Current.Session("Cmb_MagazzinoConferimento") = "" Then

            'If Cmb_Regolamento.SelectedItem.Value = "" Then
            Magazzino = ""
        Else
            Magazzino = HttpContext.Current.Session("Cmb_MagazzinoConferimento")
        End If


        'Genero l'XML del nodo magazzino conferimento
        If Magazzino <> "" Then

            Id_Cod = enum_CodiciAnagrafe.Magazzino_Conferimento
            Val_Cod = Magazzino

            'Genero l'XML del singolo nodo
            Call XML_Codice(enum_CodificaDecodifica.Codifica,
                            str_CodiceImpianto,
                            enum_TipoOperazioneDB.Scrittura,
                            Id_Cod,
                            Val_Cod,
                            CDate(xValiditaInizio),
                            CDate(xValiditaFine),
                            BaseCode,
                            TopCode,
                            "Impianto")

            'Inserisco l'XML nella stringa complessiva
            str_DatiCodici = str_DatiCodici & str_CodiceImpianto

        End If


        Dim Capitolato As String

        If HttpContext.Current.Session("Cmb_CapitolatoPrivato") = "" Then

            'If Cmb_Regolamento.SelectedItem.Value = "" Then
            Capitolato = ""
        Else
            Capitolato = HttpContext.Current.Session("Cmb_CapitolatoPrivato")
        End If


        'Genero l'XML del nodo capitolato privato
        If Capitolato <> "" Then

            Id_Cod = enum_CodiciAnagrafe.Capitolato_Privato
            Val_Cod = Capitolato

            'Genero l'XML del singolo nodo
            Call XML_Codice(enum_CodificaDecodifica.Codifica,
                            str_CodiceImpianto,
                            enum_TipoOperazioneDB.Scrittura,
                            Id_Cod,
                            Val_Cod,
                            CDate(xValiditaInizio),
                            CDate(xValiditaFine),
                            BaseCode,
                            TopCode,
                            "Impianto")

            'Inserisco l'XML nella stringa complessiva
            str_DatiCodici = str_DatiCodici & str_CodiceImpianto

        End If

        ''Genero l'XML dei nodi Altri codici identificativi dell'impianto
        'For Indice = 0 To ListCodici.Items.Count - 1

        '    'Recupero le informazioni
        '    Id_Cod = CInt(ListCodici.Items(Indice).Value)
        '    Start = InStr(ListCodici.Items(Indice).Text, " =", )
        '    Val_Cod = Right(ListCodici.Items(Indice).Text, ListCodici.Items(Indice).Text.Length - Start - 2)

        '    'Genero l'XML del singolo nodo
        '    Call XML_Codice(enum_CodificaDecodifica.Codifica, _
        '                    str_CodiceImpianto, _
        '                    enum_TipoOperazioneDB.Scrittura, _
        '                    Id_Cod, _
        '                    Val_Cod, _
        '                    CDate(xValiditaInizio), _
        '                    CDate(xValiditaFine), _
        '                    BaseCode, _
        '                    TopCode, _
        '                    "Impianto")

        '    'Inserisco l'XML nella stringa complessiva
        '    str_DatiCodici = str_DatiCodici & str_CodiceImpianto

        'Next


        XML_DatiCodici.InnerXml = str_DatiCodici


        '------------------------------------------------
        '----- Costruisco la stringa XML delle particelle
        '------------------------------------------------

        XML_DatiParticelle = XmlDoc.CreateElement("DatiParticellexProgetto")

        XML_Progetto.AppendChild(XML_DatiParticelle)

        str_DatiParticelle = ""

        ''----- Leggo gli i dati nei controlli e per ognuno genero un nodo 

        'For Indice = 0 To ListParticelle.Items.Count - 1

        '    'Recupero le informazioni
        '    Id_Cod = enum_CodiciAnagrafe.CodiceRigaRiferimentoQuadroP

        '    Start = InStr(ListParticelle.Items(Indice).Text, " =", )
        '    Val_Cod = Right(ListParticelle.Items(Indice).Text, ListParticelle.Items(Indice).Text.Length - Start - 2)

        '    Valore = ListParticelle.Items(Indice).Value
        '    ArrayParticella = Split(Valore, "£")

        '    Prov = ArrayParticella(0)
        '    Com = ArrayParticella(1)
        '    Sezione = ArrayParticella(2)
        '    Foglio = CInt(ArrayParticella(3))
        '    Numero = CInt(ArrayParticella(4))
        '    Subalterno = ArrayParticella(5)

        '    'Genero l'XML del singolo nodo
        '    Call XML_ParticellaxProgetto(enum_CodificaDecodifica.Codifica, _
        '                                str_Particella, _
        '                                enum_TipoOperazioneDB.Scrittura, _
        '                                Prov, _
        '                                Com, _
        '                                Sezione, _
        '                                Foglio, _
        '                                Numero, _
        '                                Subalterno, _
        '                                Id_Cod, _
        '                                Val_Cod, _
        '                                CDate(xValiditaInizio), _
        '                                CDate(xValiditaFine), _
        '                                BaseCode, _
        '                                TopCode)

        '    'Inserisco l'XML nella stringa complessiva
        '    str_DatiParticelle = str_DatiParticelle & str_Particella

        'Next


        XML_DatiParticelle.InnerXml = str_DatiParticelle



        ''------------------------------------------------
        ''----- Costruisco la stringa XML delle FASI
        ''------------------------------------------------

        'For i = 0 To DataGridFasi.Items.Count - 1

        '    str_ProgettoFase += GeneraBloccoProgettoFase(CInt(Operazione), _
        '                                                xPiva, _
        '                                                Cod_Progetto, _
        '                                                CInt(DataGridFasi.Items(i).Cells(2).Text), _
        '                                                DataGridFasi.Items(i).Cells(3).Text, _
        '                                                CInt(DataGridFasi.Items(i).Cells(0).Text), _
        '                                                CInt(DataGridFasi.Items(i).Cells(4).Text), , , , _
        '                                                CInt(CType(DataGridFasi.Items(i).FindControl("TxtBudget"), TextBox).Text), _
        '                                                , , _
        '                                                BaseCode, _
        '                                                TopCode)

        'Next

        'For i = 0 To DataGridLavorati.Items.Count - 1

        '    str_PrezzoUnitario += Xml_PrezzoUnitario(2, _
        '                                            xPiva, _
        '                                            Cod_Progetto, _
        '                                            CInt(DataGridLavorati.Items(i).Cells(3).Text), _
        '                                            CInt(DataGridLavorati.Items(i).Cells(13).Text), _
        '                                            CInt(DataGridLavorati.Items(i).Cells(14).Text), _
        '                                            CInt(DataGridLavorati.Items(i).Cells(0).Text), _
        '                                            CInt(DataGridLavorati.Items(i).Cells(1).Text), _
        '                                            CInt(DataGridLavorati.Items(i).Cells(2).Text), _
        '                                            CInt(DataGridLavorati.Items(i).Cells(5).Text), _
        '                                            CInt(DataGridLavorati.Items(i).Cells(6).Text), _
        '                                            Replace(DataGridLavorati.Items(i).Cells(8).Text, "&nbsp;", ""), _
        '                                            CDbl(CType(DataGridLavorati.Items(i).FindControl("TxtBudget"), TextBox).Text), _
        '                                            BaseCode, _
        '                                            TopCode)


        'Next

        XML_Progetto.InnerXml = str_ProgettoFase & str_PrezzoUnitario & XML_DatiCodici.OuterXml & XML_DatiParticelle.OuterXml

        XmlDoc.AppendChild(XML_DatiProgetto)

        Return XmlDoc.OuterXml


    End Function

    '########################################################################################
    Private Sub ImgBtn_SalvaTutto_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_SalvaTutto.Click

        Salva_Tutto()

    End Sub


    Private Sub Salva_Tutto()

        'Dim Connessione As OleDb.OleDbConnection
        'Dim Transazione As OleDb.OleDbTransaction
        Dim ErrMSG As String

        Dim StrImpianto As String
        Dim StrXmlInserisci As String
        Dim StrXmlCancella As String


        Dim TipoOperazioneDB As enum_TipoOperazioneDB
        'Dim Piva As String
        'Dim Sa_Cod As Integer
        Dim Campo_Cod As Integer
        'Dim Appezza As Integer
        'Dim Id_Imp As Integer
        Dim Sup_Imp As Double
        Dim Cod_Resp As Integer
        Dim Cod_Ente As Integer
        Dim Campo_Spia As Integer
        Dim Data As Date
        Dim Cul_Cod As Integer
        Dim Grva_Cod_Veg As Integer
        Dim Data_Raccolta As String
        Dim Produzione As Integer
        Dim Scarto As Integer
        Dim Ind_Mat_Cod As Integer
        Dim Ind_Mat_Ril As String
        Dim Sta_Ter As String
        Dim Cop_DI As String
        Dim Cop_DF As String
        Dim P_HA As Double
        Dim P_IMPIANTO As Double
        'Dim ResaPrevista_HA As Double
        Dim ResaPrevista_IMPIANTO As Double = 0
        Dim Setup_Cod As String = ""
        Dim Port_Cod As Integer
        Dim Stru_Prot As Integer
        Dim Pro_Pag As Integer
        Dim Seme_Q As Integer
        Dim Seme_T As Integer
        Dim Seme_P As Integer
        Dim Seme_D As Integer
        Dim Stato_Residui As String
        Dim Denitrificazione As Integer
        Dim Volatilizzazione As Integer
        Dim ProfonditaLav As Integer
        Dim Cover As Integer
        Dim Monitorato As Integer
        Dim Codice_Ficale_Tecnico As String
        Dim Data_Conversione As String
        Dim Grfi_Cod As Integer
        Dim Imp_Cod As Integer
        Dim Regolamento As Integer
        Dim Finanziamento As Integer = 0
        Dim Su_Cod As Integer
        Dim Cop_Cod As Integer
        Dim Foral_Cod As Integer
        Dim Tecn_Cod As Integer
        Dim Validita_Inizio As Date
        Dim Validita_Fine As Date

        Dim ProvenienzaSeme As Integer

        Dim StrCodice As String
        Dim StrCodici As String

        Dim Id_Cod As Integer
        Dim Val_Cod As String

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlDoc2 As New System.Xml.XmlDocument
        Dim XmlDatiReg_Impianti As System.Xml.XmlElement
        Dim XmlImpianto As System.Xml.XmlElement
        Dim XmlImpianto2 As System.Xml.XmlElement
        Dim XmlCodice As System.Xml.XmlElement
        Dim XmlDatiCodici As System.Xml.XmlElement
        Dim XmlDatiCodici2 As System.Xml.XmlElement

        Dim objImpianto As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_W   'Object 'Agro_Anagrafe.Reg_Impianto_W
        Dim BoolDummy As String
        Dim Id_Reg_New As Integer

        Dim j As Integer
        Dim Indice As Integer
        Dim Start As Integer



        '------------------------------------------------
        '----- Verifico l'operazione richiesta (Inserimento / Modifica / Cancellazione)
        '------------------------------------------------

        'Recupero l'operazione richiesta, dalla querystring
        'Operazione = Operazione = objParametriAgenda.Tipo_Operazione


        '------------------------------------------------
        '----- Verifico la correttezza delle informazioni inserite
        '------------------------------------------------

        Dim contr As New AgronicaControlli_2010.SalvataggioWebControl


        '------------------------------------------------
        '--- Dati impianto


        'If Me.ChkTerrenoNudo.Checked = False Then
        '    'If Me.Cmb_GruppoVegetale.SelectedIndex = 0 Then
        '    '    MessaggioErrore += "Selezionare il Gruppo Vegetale!!" & vbCrLf
        '    'Else
        '    If Me.Cmb_Specie.SelectedIndex = 0 Then
        '        MessaggioErrore += "Selezionare la Specie Vegetale!!" & vbCrLf
        '    Else
        '        If Me.Cmb_Cultivar.SelectedIndex = 0 Then
        '            MessaggioErrore += "Selezionare la Varietà!!" & vbCrLf
        '        Else
        '            If Me.Cmb_Finalita.SelectedIndex = 0 Then
        '                MessaggioErrore += "Selezionare la Finalità!!" & vbCrLf
        '            End If
        '        End If
        '    End If
        '    'End If
        'Else
        ''controllo che sia stata selezionata la destinzaione d'uso
        'If Cmb_CodiciTerreno.SelectedIndex = 0 Then
        '    MessaggioErrore += "Selezionare la destinzaione d'uso associata al terreno nudo!!" & vbCrLf
        'End If
        'End If


        ''------------------------------------------------
        ''--- Date di validita

        'If Not IsDate(TxtValiditaInizio.Text) Then
        '    MessaggioErrore += "La Data di Inizio Validità è obbligatoria!!" & vbCrLf
        'End If

        If Not IsDate(TxtValiditaInizio.Text) Then
            Validita_Inizio = AGRODATAINIZIO
        Else
            Validita_Inizio = CDate(TxtValiditaInizio.Text)
        End If

        If Not IsDate(TxtValiditaFine.Text) Then
            Validita_Fine = AGRODATAFINE
        Else
            Validita_Fine = CDate(TxtValiditaFine.Text)
        End If

        If Validita_Fine < Validita_Inizio Then
            contr.AggiungiMessaggioErrore("   - La fine dell'impianto non puo' precedere l'inizio!!")
        End If

        If Validita_Inizio < CDate(InizioAppezzamento.Value) Then
            contr.AggiungiMessaggioErrore("   - L'inizio dell'impianto non puo' precedere la creazione dell'Appezzamento. (" & InizioAppezzamento.Value & ")")
        End If

        If Validita_Fine > CDate(FineAppezzamento.Value) Then
            contr.AggiungiMessaggioErrore("   - La fine dell'impianto non puo' seguire la cessazione dell'Appezzamento. (" & FineAppezzamento.Value & ")")
        End If

        Dim Data_Fine_Distinta As Date

        If Not IsDate(Me.Txt_ValiditaFine_Distinta.Text) Then
            Data_Fine_Distinta = AGRODATAFINE
        Else
            Data_Fine_Distinta = CDate(Me.Txt_ValiditaFine_Distinta.Text)
        End If

        If Data_Fine_Distinta > Validita_Fine Then
            contr.AggiungiMessaggioErrore("La data di fine Impianto non può essere antecedente alla data fine dell'Esercizio!")
        End If

        'Se l'impianto è consociato...
        If ChkConsociazione.Checked = True Then


            'Recupero l'Id_Consociazione eventualmente già esistente
            Verifica_Esistenza_Impianti_Consociati(Id_Consociazione)

            'Se è il primo impianto consociato devo calcolare il nuovo id_consociazione
            If Id_Consociazione = 0 Then

                Dim objSeq As New AgronicaCoreDataProvider.Agro_Sequenze

                Id_Consociazione = objSeq.NuovoId_Tabella("Reg_Impianti_Consociazioni",
                                                            0,
                                                            2000000000,
                                                            objParametri_Server)
            End If

        End If


        '--------------------------------------------------------------------------------------------------------------------------------------------
        'Controllo, se sono già state registrate Operazioni d'Agenda
        'che le date delle operazioni nn siano esterne alle date scelte x l'impianto 
        '--------------------------------------------------------------------------------------------------------------------------------------------

        Dim Validita_Inizio_Agenda As Date
        Dim Validita_Fine_Agenda As Date

        Dim MovimentiPresenti As Boolean

        Dim Hash_IdAgenda_ToFlag As New Hashtable
        MovimentiPresenti = False

        If Operazione = enum_TipoOperazioneDB.Modifica Then

            Dim ObjAgenda As New AgronicaCoreContabDAL.Mov_Destinazioni_R  'New Agro_Contab_AD.Mov_Destinazioni_R
            Dim DTAgenda As DataTable
            Dim Id_Agenda As Integer

            'ricavo il recordset dei movimenti di produzione associati all'impianto
            DTAgenda = ObjAgenda.LeggiCronologiaMovimenti(xPiva,
                                                        xSa_Cod,
                                                        xAppezza,
                                                        xId_Imp,
                                                          AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                        "",
                                                        "",
                                                        objParametri_Server)


            ObjAgenda = Nothing

            If DTAgenda.Rows.Count > 0 Then

                MovimentiPresenti = True

                Validita_Inizio_Agenda = DTAgenda.Rows(0).Item("Data_Movimento")

                Validita_Fine_Agenda = DTAgenda.Rows(DTAgenda.Rows.Count - 1).Item("Data_Movimento")

                '(24/07/2015) eliminato perchè spostato nel core di scrittura
                ''se ho modificato la superficie, mi salvo gli id_agenda delle operazioni da flaggare
                'If viewstate("FlagSupModificata") = True Then
                '    Dim i As Integer
                '    For i = 0 To DTAgenda.Rows.Count - 1
                '        Id_Agenda = DTAgenda.Rows(i).Item("Id_agenda")
                '        If Not Hash_IdAgenda_ToFlag.Contains(Id_Agenda) Then
                '            Hash_IdAgenda_ToFlag.Add(Id_Agenda, Qs_Piva)
                '        End If
                '    Next
                'End If

            End If

            If MovimentiPresenti = True Then

                If Validita_Inizio_Agenda < Validita_Inizio Then
                    contr.AggiungiMessaggioErrore("   - Non è possibile inserire l'impianto nell'intervallo scelto, " +
                                    "     poichè sono state registrate Operazioni d'Agenda dal " & CDate(Validita_Inizio_Agenda).ToShortDateString & " al " & CDate(Validita_Fine_Agenda).ToShortDateString)
                End If

                If Validita_Fine_Agenda > Validita_Fine Then
                    contr.AggiungiMessaggioErrore("   - Non è possibile inserire l'impianto nell'intervallo scelto, " & vbCrLf &
                                    "     poichè sono state registrate Operazioni d'Agenda dal " & CDate(Validita_Inizio_Agenda).ToShortDateString & " al " & CDate(Validita_Fine_Agenda).ToShortDateString)
                End If

            End If

        End If

        ''------------------------------------------------
        ''--- Superficie

        'If Me.TxtSuperficie.Text = "0" Or Me.TxtSuperficie.Text = "" Then
        '    contr.AggiungiMessaggioErrore(" La superficie non può essere nulla!!")
        'Else
        '    If Not IsNumeric(Me.TxtSuperficie.Text) Then
        '        contr.AggiungiMessaggioErrore("La superficie deve essere un numero!")
        '    End If
        '    If InStr(TxtSuperficie.Text, ".") <> 0 Then
        '        TxtSuperficie.Text = Replace(TxtSuperficie.Text, ".", ",")
        '    End If
        'End If



        ''------------------------------------------------------------------------
        'If MovimentiPresenti = True Then

        '    If ViewState("Cambiata_Sup") = True Then

        '        Dim Testo As String

        '        Testo = "Attenzione! E' variata la Superficie dell'impianto!" & vbCrLf &
        '                "Poichè a quest'ultimo risultano essere associati movimenti d'agenda" & vbCrLf &
        '                "sarà NECESSARIO aprire ogni singolo intervento e risalvarlo" & vbCrLf &
        '                "al fine di consentire un coerente ricalcolo delle dosi!" & vbCrLf & vbCrLf &
        '                "Si desidera MODIFICARE comunque la superficie?"

        '        Testo = Server.UrlEncode(Testo)

        '        Dim Stringa As String = "<script language='vbscript'> " &
        '                                " a = window.showModalDialog(" & Chr(34) & "../AA_Script/Controlli/AgroSiNo/AgroSiNo.aspx?des=" & Testo & Chr(34) & "," & Chr(34) & Chr(34) & "," & Chr(34) & "dialogWidth:260px;dialogHeight:350px;status:no; center:yes;edge:raised; help:no;" & Chr(34) & ") " &
        '                                vbCrLf & " document.all(" & Chr(34) & "SI_NO" & Chr(34) & ").value = a" &
        '                                vbCrLf & " document.getElementById(" & Chr(34) & "Form1" & Chr(34) & ").submit()" &
        '                                "</script>"

        '        Me.FindControl("Form1").Controls.Add(New LiteralControl(Stringa))

        '        ViewState("Cambiata_Sup") = False

        '        Exit Sub

        '    End If

        'End If



        ''------------------------------------------------
        ''--- Sesto Impianto

        'If Me.Txt_DistanzaTraFila_M.Text <> "" Then
        '    If Not IsNumeric(Me.Txt_DistanzaTraFila_M.Text) Then
        '        AgroMsgBox("La distanza tra fila deve essere un numero!", Page)
        '        Exit Sub
        '    End If
        'End If
        'If InStr(Txt_DistanzaTraFila_M.Text, ".") <> 0 Then
        '    Txt_DistanzaTraFila_M.Text = Replace(Txt_DistanzaTraFila_M.Text, ".", ",")
        'End If

        'If Me.Txt_DistanzaTraFila_F.Text <> "" Then
        '    If Not IsNumeric(Me.Txt_DistanzaTraFila_F.Text) Then
        '        AgroMsgBox("La distanza tra fila deve essere un numero!", Page)
        '        Exit Sub
        '    End If
        'End If
        'If InStr(Txt_DistanzaTraFila_F.Text, ".") <> 0 Then
        '    Txt_DistanzaTraFila_F.Text = Replace(Txt_DistanzaTraFila_F.Text, ".", ",")
        'End If

        'If Me.Txt_DistanzaSuFila_M.Text <> "" Then
        '    If Not IsNumeric(Me.Txt_DistanzaSuFila_M.Text) Then
        '        AgroMsgBox("La distanza su fila deve essere un numero!", Page)
        '        Exit Sub
        '    End If
        'End If
        'If InStr(Txt_DistanzaSuFila_M.Text, ".") <> 0 Then
        '    Txt_DistanzaSuFila_M.Text = Replace(Txt_DistanzaSuFila_M.Text, ".", ",")
        'End If

        'If Me.Txt_DistanzaSuFila_F.Text <> "" Then
        '    If Not IsNumeric(Me.Txt_DistanzaSuFila_F.Text) Then
        '        AgroMsgBox("La distanza su fila deve essere un numero!", Page)
        '        Exit Sub
        '    End If
        'End If
        'If InStr(Txt_DistanzaSuFila_F.Text, ".") <> 0 Then
        '    Txt_DistanzaSuFila_F.Text = Replace(Txt_DistanzaSuFila_F.Text, ".", ",")
        'End If

        'If Me.Txt_Interbina.Text <> "" Then
        '    If Not IsNumeric(Me.Txt_Interbina.Text) Then
        '        AgroMsgBox("La distanza tra due file binate deve essere un numero!", Page)
        '        Exit Sub
        '    End If
        'End If
        'If InStr(Txt_Interbina.Text, ".") <> 0 Then
        '    Txt_Interbina.Text = Replace(Txt_Interbina.Text, ".", ",")
        'End If

        'If Me.Txt_Germinabilita.Text <> "" Then
        '    If Not IsNumeric(Me.Txt_Germinabilita.Text) Then
        '        AgroMsgBox("La germinabilità deve essere un numero!", Page)
        '        Exit Sub
        '    End If
        'End If
        'If InStr(Txt_Germinabilita.Text, ".") <> 0 Then
        '    Txt_Germinabilita.Text = Replace(Txt_Germinabilita.Text, ".", ",")
        'End If


        ''--- Controllo Organismo Referente
        'If Esiste_Obbligo_SalvataggioOrganismoReferente(Server, Session, Page) = True Then

        '    If Not IsNothing(Me.Cmb_OrganismoReferente.SelectedItem) Then
        '        If Me.Cmb_OrganismoReferente.SelectedItem.Text = "" Then
        '            MessaggioErrore += "E' obbligatorio impostare l'Organismo Referente nell'esercizio selezionata!" & vbCrLf
        '        End If
        '    Else
        '        MessaggioErrore += "E' obbligatorio impostare l'Organismo Referente nell'esercizio selezionata!" & vbCrLf
        '    End If
        'End If


        ''--- RIEPILOGO

        'Dim Messaggio As String

        'If MessaggioErrore <> "" Then

        '    Messaggio = ""
        '    Messaggio += "Sono stati rilevati i seguenti errori : " & vbCrLf
        '    Messaggio += "" & vbCrLf
        '    Messaggio += MessaggioErrore
        '    Messaggio += "" & vbCrLf
        '    Messaggio += "Ritentare il salvataggio dopo la correzione ..."

        '    AgroMsgBox(Messaggio, Page)

        '    Exit Sub

        'End If


        '------------------------------------------------
        '----- Calcolo i valori di BaseCode e TopCode
        '------------------------------------------------

        Call Calcola_BaseCode_TopCode(BaseCode,
                                      TopCode,
                                      Session("ASG_ProgressivoGIAS"))

        '------------------------------------------------
        '----- Costruisco la stringa XML dei CODICI
        '------------------------------------------------

        'Azzero la stringa complessiva dei codici
        StrCodici = ""


        '----- Leggo gli i dati nei controlli e per ognuno genero un nodo 

        TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura
        '#NotInMyName Simone
        For j = 0 To 14

            Val_Cod = ""

            Select Case j

                Case 0
                    If Me.ChkVarietaIbrida.Checked = False Then
                        Id_Cod = enum_CodiciAnagrafe.Impianto_Ibrido
                        Val_Cod = "0"
                    Else
                        Id_Cod = enum_CodiciAnagrafe.Impianto_Ibrido
                        Val_Cod = "1"
                    End If

                Case 1
                    If Me.Txt_CodBMBDBT_M.Text <> "" Then
                        Id_Cod = enum_CodiciAnagrafe.Impianto_CodiceB_Maschio
                        Val_Cod = Me.Txt_CodBMBDBT_M.Text
                    End If

                Case 2
                    If Me.ChkVarietaIbrida.Checked = True Then
                        Id_Cod = enum_CodiciAnagrafe.Impianto_CodiceB_Femmina
                        Val_Cod = Me.Txt_CodBMBDBT_F.Text
                    Else
                        Id_Cod = enum_CodiciAnagrafe.Impianto_CodiceB_Femmina
                        Val_Cod = ""
                    End If

                Case 3
                    If Me.Txt_Genetica_M.Text <> "" Then
                        Id_Cod = enum_CodiciAnagrafe.Impianto_Genetica_Maschio
                        Val_Cod = Me.Txt_Genetica_M.Text
                    End If

                Case 4
                    If Me.ChkVarietaIbrida.Checked = True Then
                        Id_Cod = enum_CodiciAnagrafe.Impianto_Genetica_Femmina
                        Val_Cod = Me.Txt_Genetica_F.Text
                    Else
                        Id_Cod = enum_CodiciAnagrafe.Impianto_Genetica_Femmina
                        Val_Cod = ""
                    End If

                Case 5
                    If Me.Txt_OffType_M.Text <> "" Then
                        Id_Cod = enum_CodiciAnagrafe.Impianto_OffType_Maschio
                        Val_Cod = Me.Txt_OffType_M.Text
                    End If

                Case 6
                    If Me.ChkVarietaIbrida.Checked = True Then
                        Id_Cod = enum_CodiciAnagrafe.Impianto_OffType_Femmina
                        Val_Cod = Me.Txt_OffType_F.Text
                    Else
                        Id_Cod = enum_CodiciAnagrafe.Impianto_OffType_Femmina
                        Val_Cod = ""
                    End If

                Case 7
                    If Me.Txt_DistanzaSuFila_M.Text <> "" Then
                        Id_Cod = enum_CodiciAnagrafe.Impianto_SuFila_Maschio
                        Val_Cod = Me.Txt_DistanzaSuFila_M.Text
                    End If

                Case 8
                    If Me.ChkVarietaIbrida.Checked = True Then
                        Id_Cod = enum_CodiciAnagrafe.Impianto_SuFila_Femmina
                        Val_Cod = Me.Txt_DistanzaSuFila_F.Text
                    Else
                        Id_Cod = enum_CodiciAnagrafe.Impianto_SuFila_Femmina
                        Val_Cod = ""
                    End If

                Case 9
                    If Me.Txt_DistanzaTraFila_M.Text <> "" Then
                        Id_Cod = enum_CodiciAnagrafe.Impianto_TraFila_Maschio
                        Val_Cod = Me.Txt_DistanzaTraFila_M.Text
                    End If

                Case 10

                    If Me.ChkVarietaIbrida.Checked = True Then
                        Id_Cod = enum_CodiciAnagrafe.Impianto_TraFila_Femmina
                        Val_Cod = Me.Txt_DistanzaTraFila_F.Text
                    Else
                        Id_Cod = enum_CodiciAnagrafe.Impianto_TraFila_Femmina
                        Val_Cod = ""
                    End If

                Case 11

                    If Me.Txt_Interbina.Text <> "" Then
                        Id_Cod = enum_CodiciAnagrafe.Impianto_Interbina
                        Val_Cod = Me.Txt_Interbina.Text
                    End If

                Case 12

                    If Me.Txt_Germinabilita.Text <> "" Then
                        Id_Cod = enum_CodiciAnagrafe.Impianto_Germinabilita
                        Val_Cod = Me.Txt_Germinabilita.Text
                    End If

                    'Case 13

                    '    If Txt_PartiTuberi.Text <> "" Then
                    '        Id_Cod = enum_CodiciAnagrafe.Impianto_Parti_Tuberi_Patate
                    '        Val_Cod = Txt_PartiTuberi.Text
                    '    End If

                    'Case 14

                    '    If Cmb_TagliatoTuberi.SelectedValue.ToString <> "" Then
                    '        Id_Cod = enum_CodiciAnagrafe.Impianto_Taglio_Tuberi_Patate
                    '        Val_Cod = Cmb_TagliatoTuberi.SelectedValue.ToString
                    '    End If

            End Select

            If Val_Cod <> "" Then

                'Genero l'XML del singolo nodo solo se i codici sono valorizzati
                Call XML_Codice(enum_CodificaDecodifica.Codifica,
                                StrCodice,
                                TipoOperazioneDB,
                                Id_Cod,
                                Val_Cod,
                                CDate(Validita_Inizio),
                                CDate(Validita_Fine),
                                BaseCode,
                                TopCode,
                                "Impianto")

                'Inserisco l'XML nella stringa complessiva
                StrCodici = StrCodici & StrCodice

            End If

        Next


        'Genero l'XML del nodo codice terreno (terreno nudo destinazione d'uso)

        If ChkTerrenoNudo.Checked = True And Cmb_CodiciTerreno.SelectedValue <> "" Then
            Id_Cod = Cmb_CodiciTerreno.SelectedValue
            Val_Cod = ""

            'Genero l'XML del singolo nodo
            Call XML_Codice(enum_CodificaDecodifica.Codifica,
                            StrCodice,
                            TipoOperazioneDB,
                            Id_Cod,
                            Val_Cod,
                            CDate(Validita_Inizio),
                            CDate(Validita_Fine),
                            BaseCode,
                            TopCode,
                            "Impianto")

            'Inserisco l'XML nella stringa complessiva
            StrCodici = StrCodici & StrCodice

        End If

        'Genero l'XML del nodo gruppo varietale
        If Me.Cmb_DettaglioVarietaPersonalizzato.SelectedItem.Value <> "0" Then


            Id_Cod = enum_CodiciAnagrafe.Dettaglio_Specie_Personalizzato
            Val_Cod = Me.Cmb_DettaglioVarietaPersonalizzato.SelectedItem.Value

            'Genero l'XML del singolo nodo
            Call XML_Codice(enum_CodificaDecodifica.Codifica,
                            StrCodice,
                            TipoOperazioneDB,
                            Id_Cod,
                            Val_Cod,
                            CDate(Validita_Inizio),
                            CDate(Validita_Fine),
                            BaseCode,
                            TopCode,
                            "Impianto")

            'Inserisco l'XML nella stringa complessiva
            StrCodici = StrCodici & StrCodice

        End If

        '------------------------------------------------
        '----- Costruisco la stringa XML dell'IMPIANTO
        '------------------------------------------------

        '---------------------
        'riciclo resa_prevista e resa_corretta impianto per apofruit
        Dim resa_prevista As Double
        Dim resa_effettiva As Double = 0

        contr.Controlla(Txt_Resa1, resa_prevista, 0)
        contr.Controlla(Txt_Resa2, resa_effettiva, 0)

        '---------------------



        'Prelevo le informazioni immediate
        TipoOperazioneDB = Operazione
        'Piva = Me.Lbl_Piva.Text
        'Sa_Cod = Lbl_SaCod.Text
        'Campo_Cod = Lbl_CampoCod.Text
        'Appezza = Lbl_Appezza.Text
        'Id_Imp = Lbl_IdReg.Text
        Sup_Imp = TxtSuperficie.Text
        '
        Cod_Resp = 0
        Cod_Ente = 0
        Campo_Spia = 0
        Data = CDate(TxtValiditaInizio.Text)
        '        '
        Data_Raccolta = "0"
        Produzione = "0"


        'se vuoto lo pongo a 0
        'If TxtResaEff.Text = "" Then
        '    TxtResaEff.Text = "0"
        'End If

        '
        Scarto = 0
        Ind_Mat_Cod = 0
        Ind_Mat_Ril = "0"
        Sta_Ter = ""
        '
        Dim Cop_DI_data As Date
        contr.Controlla(TxtCopDataInizio, Cop_DI_data, AGRODATAINIZIO)
        Cop_DI = CStr(Cop_DI_data)
        '


        Dim Cop_DF_data As Date
        contr.Controlla(TxtCopDataFine, Cop_DF_data, AGRODATAFINE)
        Cop_DF = CStr(Cop_DF_data)

        '
        P_HA = "0"
        'ResaPrevista_HA = "0"

        '
        Stru_Prot = 0
        Pro_Pag = 0
        Seme_Q = 0
        Seme_T = 0
        Seme_P = 0
        Seme_D = 0
        Stato_Residui = ""
        Denitrificazione = 0
        Volatilizzazione = 0
        ProfonditaLav = 0
        Codice_Ficale_Tecnico = ""
        Data_Conversione = "0"


        'identifico il Codice_Ficale_Tecnico
        Dim xUtenteCorrente As String = CType(objParametri_Server, AgronicaCoreDataProvider.AgronicaCoreParametri).UtenteUsername
        Dim objUtentiDAL As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R
        Codice_Ficale_Tecnico = objUtentiDAL.Leggi_IdentificativoGruppoUtenti_Singolo(xUtenteCorrente, objParametri_Utenti)



        '
        If ChkCoverCrops.Checked = True Then
            Cover = 1
        Else
            Cover = 0
        End If
        '
        If ChkMonitorato.Checked = True Then
            Monitorato = 1
        Else
            Monitorato = 0
        End If
        '
        '
        '========== COMBOBOX ==========
        '

        'TODO una volta terminati i test unire tutti i seguenti IF

        If Me.ChkTerrenoNudo.Checked = False Then
            'If Cmb_Finalita.Enabled = True Then
            'nb patch finchè non viene sistemato il db -> alcune specie vegetali non hanno i riferimenti 
            'sul database....

            contr.Controlla(Cmb_Finalita, Grfi_Cod, DEFAULT_FINALITA)

        End If


        '
        '
        '
        If Me.ChkTerrenoNudo.Checked = False Then
            'If Cmb_Cultivar.Enabled = True Then
            'nb patch finchè non viene sistemato il db -> alcune specie vegetali non hanno i riferimenti 
            'sul database....

            contr.Controlla(Cmb_Cultivar, Cul_Cod, DEFAULT_CULTIVAR)

            'If Cmb_Cultivar.SelectedValue = "" Then
            '    'If IsNothing(Cmb_Cultivar.SelectedItem) Then
            '    AgroMsgBox("Non esistono VARIETA' correlate alla specie selezionata. " & vbCrLf &
            '                "Non è stato possibile salvare ..., " & vbCrLf &
            '                "Prendere nota della Specie Vegetale e contattare il referente!",
            '                Page,, True)

            '    Cul_Cod = 0


            'Else
            '    Cul_Cod = Cmb_Cultivar.SelectedValue
            'End If

        End If
        '

        If Me.ChkTerrenoNudo.Checked = False Then
            'If Cmb_TipologiaVarietale.Enabled = True Then
            'nb patch finchè non viene sistemato il db -> alcune specie vegetali non hanno i riferimenti 
            'sul database....

            contr.Controlla(Cmb_TipologiaVarietale, Grva_Cod_Veg, DEFAULT_GRVA)

            'If Me.Cmb_TipologiaVarietale.SelectedValue = "" Then

            '    'If IsNothing(Cmb_TipologiaVarietale.SelectedItem) Then
            '    'AgroMsgBox("Non esistono Tipologie Varietali correlate alla specie selezionata. " & vbCrLf & _
            '    '            "Non è stato possibile salvare ..., " & vbCrLf & _
            '    '            "Prendere nota della Specie Vegetale e contattare l'amministratore!", _
            '    '            Page)

            '    Grva_Cod_Veg = 0

            'Else
            '    Grva_Cod_Veg = Me.Cmb_TipologiaVarietale.SelectedValue
            'End If

        End If
        '
        '
        '
        If Cmb_ImpIrrigazione.Enabled = True Then

            contr.Controlla(Cmb_ImpIrrigazione, Imp_Cod, DEFAULT_IMPIRRIGAZIONE)

            'If Me.Cmb_ImpIrrigazione.SelectedValue = "0" Or Me.Cmb_ImpIrrigazione.SelectedValue = "" Then
            '    'If Cmb_ImpIrrigazione.SelectedItem.Value = "0" Or Cmb_ImpIrrigazione.SelectedItem.Value = "" Then
            '    Imp_Cod = -1
            'Else
            '    Imp_Cod = CInt(Me.Cmb_ImpIrrigazione.SelectedValue)
            'End If
        Else
            Imp_Cod = DEFAULT_IMPIRRIGAZIONE
        End If
        '
        If Cmb_Regolamento.Enabled = True Then

            contr.Controlla(Cmb_Regolamento, Regolamento, DEFAULT_REGOLAMENTO)

            'If Me.Cmb_Regolamento.SelectedValue = "" Then

            '    'If Cmb_Regolamento.SelectedItem.Value = "" Then
            '    Regolamento = 1
            'Else
            '    Regolamento = Me.Cmb_Regolamento.SelectedValue
            'End If
        Else
            Regolamento = DEFAULT_REGOLAMENTO
        End If
        '
        '
        '
        If Cmb_ProvenienzaSeme.Enabled = True Then

            contr.Controlla(Cmb_ProvenienzaSeme, ProvenienzaSeme, DEFAULT_PROVENIENZA_SEME)

            'If Cmb_ProvenienzaSeme.SelectedValue = "" Then
            '    'If Cmb_ProvenienzaSeme.SelectedItem.Value = "" Then
            '    ProvenienzaSeme = 0
            'Else
            '    ProvenienzaSeme = Cmb_ProvenienzaSeme.SelectedValue
            'End If
        Else
            ProvenienzaSeme = DEFAULT_PROVENIENZA_SEME
        End If
        '
        '
        '
        'If Cmb_Disciplinare.Enabled = True Then
        '    If Cmb_Disciplinare.SelectedItem.Value = "0" Then
        '        Finanziamento = 0
        '    Else
        '        Finanziamento = Cmb_Disciplinare.SelectedItem.Value
        '    End If
        'Else
        '    Finanziamento = 0
        'End If
        '
        '
        '
        'If Cmb_ConduzioneSuFila.Enabled = True Then
        '    If Cmb_ConduzioneSuFila.SelectedItem.Value = "" Then
        '        Su_Cod = -1
        '    Else
        '        Su_Cod = Cmb_ConduzioneSuFila.SelectedItem.Value
        '    End If
        'Else
        '    Su_Cod = -1
        'End If
        '
        '
        '
        If Cmb_Copertura.Enabled = True Then

            contr.Controlla(Cmb_Copertura, Cop_Cod, DEFAULT_COPERTURA)

            'If Me.Cmb_Copertura.SelectedValue = "" Then
            '    Cop_Cod = -1
            'Else
            '    Cop_Cod = Me.Cmb_Copertura.SelectedValue
            'End If
        Else
            Cop_Cod = DEFAULT_COPERTURA
        End If
        '
        '
        '
        If Cmb_FormaAllevamento.Enabled = True Then
            'nb patch finchè non viene sistemato il db -> alcune specie vegetali non hanno i riferimenti 
            'sul database....
            'If IsNothing(Cmb_FormaAllevamento.SelectedItem) Then
            '    AgroMsgBox("Non esistono FORME di ALLEVAMENTO correlate alla specie selezionata. " & vbCrLf & _
            '                "Non è stato possibile salvare ..., " & vbCrLf & _
            '                "Prendere nota della Specie Vegetale e contattare l'amministratore!", _
            '                Page)
            '    Exit Sub
            'ElseIf Cmb_FormaAllevamento.SelectedItem.Value = "" Then

            contr.Controlla(Cmb_FormaAllevamento, Foral_Cod, DEFAULT_FORMA_ALLEVAMENTO)

            'If Me.Cmb_FormaAllevamento.SelectedValue = "" Then
            '    Foral_Cod = -1
            'Else
            '    Foral_Cod = Cmb_FormaAllevamento.SelectedValue
            '    'HttpContext.Current.Session("Cmb_FormaAllevamento")


            'End If
        Else
            Foral_Cod = DEFAULT_FORMA_ALLEVAMENTO
        End If
        '
        '
        '
        'If Cmb_ConduzioneTraFila.Enabled = True Then
        '    If Cmb_ConduzioneTraFila.SelectedItem.Value = "" Then
        '        Tecn_Cod = -1
        '    Else
        '        Tecn_Cod = Cmb_ConduzioneTraFila.SelectedItem.Value
        '    End If
        'Else
        '    Tecn_Cod = -1
        'End If
        '
        '
        '
        If Cmb_SeminaTrapianto.Enabled = True Then

            contr.Controlla(Cmb_SeminaTrapianto, Setup_Cod, DEFAULT_SEMINA_TRAPIANTO)

            'If Me.Cmb_SeminaTrapianto.SelectedValue = "" Then
            '    'If Cmb_SeminaTrapianto.SelectedItem.Value = "" Then
            '    Setup_Cod = -1
            'Else
            '    Setup_Cod = Me.Cmb_SeminaTrapianto.SelectedValue
            'End If
        Else
            Setup_Cod = DEFAULT_SEMINA_TRAPIANTO
        End If
        '
        '
        '
        If Cmb_Portinnesto.Enabled = True Then
            'nb patch finchè non viene sistemato il db -> alcune specie vegetali non hanno i riferimenti 
            'sul database....
            'If IsNothing(Cmb_Portinnesto.SelectedItem) Then
            '    AgroMsgBox("Non esistono PORTINNESTI correlate alla specie selezionata. " & vbCrLf & _
            '                "Non è stato possibile salvare ..., " & vbCrLf & _
            '                "Prendere nota della Specie Vegetale e contattare l'amministratore!", _
            '                Page)
            '    Exit Sub
            'ElseIf Cmb_Portinnesto.SelectedItem.Value = "" Then

            contr.Controlla(Cmb_Portinnesto, Port_Cod, DEFAULT_PORTTINNESTO)

            'If Me.Cmb_Portinnesto.SelectedValue = "" Then
            '    Port_Cod = DEFAULT_PORTTINNESTO
            'Else
            '    Port_Cod = Me.Cmb_Portinnesto.SelectedValue
            'End If
        Else
            Port_Cod = DEFAULT_PORTTINNESTO
        End If

        Dim unita_vitata_num As Double
        contr.Controlla(TXT_UnitaVitata, unita_vitata_num, 0)

        Dim Unita_Vitata As String = CStr(unita_vitata_num)


        ' Se sono stati inseriti messaggi nel controllo significa che un dato non andava bene
        If contr.Messaggi <> "" Then
            Messaggi.AgroMsgBox("Dati non corretti: " + contr.Messaggi, Page)
            Return
        End If

        'Genero la stringa XML
        Call XML_Impianto(enum_CodificaDecodifica.Codifica,
                            StrImpianto,
                            TipoOperazioneDB,
                            xPiva,
                            xSa_Cod,
                            xCampo_Cod,
                            xAppezza,
                            xId_Imp,
                            Sup_Imp,
                            Cod_Resp,
                            Cod_Ente,
                            Campo_Spia,
                            Data,
                            Cul_Cod,
                            Grva_Cod_Veg,
                            Data_Raccolta,
                           resa_prevista,
                           resa_effettiva,
                            Scarto,
                            Ind_Mat_Cod,
                            Ind_Mat_Ril,
                            Sta_Ter,
                            Cop_DI,
                            Cop_DF,
                            0,
                            0,
                            0,
                            Setup_Cod,
                            Port_Cod,
                            Stru_Prot,
                            Pro_Pag,
                            Seme_Q,
                            Seme_T,
                            Seme_P,
                            Seme_D,
                            Stato_Residui,
                            Denitrificazione,
                            Volatilizzazione,
                            ProfonditaLav,
                            Cover,
                            Monitorato,
                            Codice_Ficale_Tecnico,
                            Data_Conversione,
                            Grfi_Cod,
                            Imp_Cod,
                            Regolamento,
                            Finanziamento,
                            Su_Cod,
                            Cop_Cod,
                            Foral_Cod,
                            Tecn_Cod,
                            ProvenienzaSeme,
                            Validita_Inizio,
                            Validita_Fine,
                            BaseCode,
                            TopCode,
                            Id_Consociazione,
                            Unita_Vitata)



        '------------------------------------------------
        '----- Costruisco la stringa XML complessiva di inserimento
        '------------------------------------------------

        'Creo il nodo "DatiReg_Impianti"
        XmlDatiReg_Impianti = XmlDoc.CreateElement("DatiReg_Impianti")

        'Inserisco il nodo "Reg_Impianto"
        XmlDatiReg_Impianti.InnerXml = StrImpianto

        'Rendo l'albero figlio del documento
        XmlDoc.AppendChild(XmlDatiReg_Impianti)

        'Seleziono il nodo "Reg_Impianto"
        XmlImpianto = XmlDoc.SelectSingleNode("//Reg_Impianto")

        'Creo il nodo "DatiCodici"
        XmlDatiCodici = XmlDoc.CreateElement("DatiCodici")

        'Inserisco gli elementi "Codice" come figli del nodo "DatiCodici"
        XmlDatiCodici.InnerXml = StrCodici

        'Rendo "DatiCodici" figlio del nodo "Reg_Impianto"
        XmlImpianto.AppendChild(XmlDatiCodici)

        'Estraggo la stringa XML complessiva
        StrXmlInserisci = XmlDoc.InnerXml

        '------------------------------------------------
        '----- Costruisco la stringa XML complessiva di cancellazione
        '------------------------------------------------

        If Operazione = enum_TipoOperazioneDB.Modifica Then

            If Session("StrXmlCodiciAttuali").ToString <> "" Then

                'Creo il nodo "DatiReg_Impianti"
                XmlDatiReg_Impianti = XmlDoc2.CreateElement("DatiReg_Impianti")

                'Inserisco il nodo "Impianto"
                XmlDatiReg_Impianti.InnerXml = StrImpianto

                'Rendo l'albero figlio del documento
                XmlDoc2.AppendChild(XmlDatiReg_Impianti)

                'Seleziono il nodo "Reg_Impianto"
                XmlImpianto2 = XmlDoc2.SelectSingleNode("//Reg_Impianto")

                'Rendo il centro in lettura
                XmlImpianto2.SetAttribute("TipoOperazioneDB", CStr(enum_TipoOperazioneDB.Lettura))

                'Creo il nodo "DatiCodici"
                XmlDatiCodici2 = XmlDoc2.CreateElement("DatiCodici")

                'Inserisco gli elementi "Codice" come figli del nodo "DatiCodici"
                XmlDatiCodici2.InnerXml = Session("StrXmlCodiciAttuali")

                'Rendo "DatiCodici" figlio del nodo "Reg_Impianto"
                XmlImpianto2.AppendChild(XmlDatiCodici2)

                'Estraggo la stringa XML complessiva
                StrXmlCancella = XmlDoc2.InnerXml

            End If

        End If


        'Distruggo gli oggetti
        XmlDatiReg_Impianti = Nothing
        XmlImpianto = Nothing
        XmlDoc = Nothing
        XmlDoc2 = Nothing


        '=======================
        '===  Aggiornamento  ===
        '=======================

        Dim ok As Boolean = False

        'apro la connessione e transazione 
        AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)

        Try

            '(24/07/2015) eliminato perchè spostato nel core di scrittura
            '------------------------------------------------
            '----- sono in MODIFICA
            '---- è stata MODIFICATA LA SUPERFICIE
            '---- ci sono operazioni registrate
            '---- l'utente ha dato conferma della modifica
            '-----> flaggo le operazioni di agenda
            '------------------------------------------------
            'If Operazione = enum_TipoOperazioneDB.Modifica And _
            '    viewstate("FlagSupModificata") = True And _
            '    MovimentiPresenti = True Then
            '    Dim objAgenda As New AgronicaCoreContabDAL.Agenda_W
            '    Dim Flag_UpdateFlagOK As Boolean
            '    Flag_UpdateFlagOK = objAgenda.Flagga_OpColturali_DaVerificare(objParametri_Server, Hash_IdAgenda_ToFlag)
            'End If



            '------------------------------------------------
            '----- Se sono in MODIFICA cancello i codici attuali
            '------------------------------------------------

            If Operazione = enum_TipoOperazioneDB.Modifica Then

                If Session("StrXmlCodiciAttuali") <> "" Then

                    BoolDummy = objImpianto.Reg_Impianto_Scrivi(
                                 CStr(StrXmlCancella),
                                 Nothing,
                                 Nothing,
                                 Nothing,
                                 Nothing,
                                   "",
                                   objParametri_Server)

                End If

            End If

            '------------------------------------------------
            '----- Modifico o Inserisco l'IMPIANTO
            '------------------------------------------------

            BoolDummy = objImpianto.Reg_Impianto_Scrivi(
                      CStr(StrXmlInserisci),
                      Nothing,
                      Nothing,
                      Nothing,
                      Id_Reg_New,
                      "",
                      objParametri_Server)

            objImpianto = Nothing

            '------------------------------------------------
            '----- Se sono in INSERIMENTO creo una distinta
            '------------------------------------------------

            If Operazione = enum_TipoOperazioneDB.Scrittura Then

                'TO CHECK

                'Creo già la nuova distinta o devo fare qui??

                'Me.Lbl_IdReg.Text = Id_Reg_New

                ' Me.ImgBtn_Salva_Distinta_Click(Me, Nothing)
                'Salva_Distinta()

            End If


            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server)
            'chiudo la connessione
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)

            'Codifico la partita IVA
            'Piva = Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server)

            ok = True
            '------------------------------------------------


        Catch exc As Exception

            '------------------------------------------------
            'Si e' verificata una eccezione !!!!!!
            '------------------------------------------------

            'Messaggio di errore
            ErrMSG = exc.Message.ToString()

            'chiudo la transazione con il rollback
            If Not objParametri_Server.objConnessione Is Nothing Then
                If Not objParametri_Server.objTransazione Is Nothing Then
                    'chiudo transazione
                    AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
                End If
                'chiudo la connessione
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)
            End If
            'Se la transazione ha avuto esito negativo allora ...
            Messaggi.AgroMsgBox("Si è verificato un errore durante la fase di salvataggio : " & vbCrLf & ErrMSG, Page)
            'AgroMsgBox("Si è verificato un errore durante la fase di salvataggio : " & vbCrLf & ErrMSG, Page,, True)
            Return


            '------------------------------------------------

        End Try


        '============================
        '===  Fine Aggiornamento  ===
        '============================

        If ok = True Then
            'Ritorno alla pagina AlberoImprese
            'Response.Redirect("AlberoImprese.aspx?P=" & xPiva)

            Dim Messaggio As String
            Messaggio = "NUOVO IMPIANTO salvato con successo." & vbCrLf & vbCrLf

            Dim TargetUrl As String
            Select Case tipo_salva.Value

                Case 1
                    Page_Load(Nothing, EventArgs.Empty)
                    clear_form()
                    Messaggio += "Ricaricata la pagina per un ulteriore inserimento" & vbCrLf
                    Messaggi.AgroMsgBox(Messaggio, Page, , UpdatePanel_script, , True)
                Case 2
                    Messaggio += "Salvataggio temporaneo avvenuto con successo" & vbCrLf
                    Messaggi.AgroMsgBox(Messaggio, Page, , UpdatePanel_script, , True)
                Case Else
                    TargetUrl = Master.TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAnagrafica, objParametriAgenda)
                    Response.Redirect(TargetUrl)
            End Select


        End If

    End Sub

    Private Sub clear_form()

        ChkTerrenoNudo.Checked = False
        Cmb_CodiciTerreno.ClearSelection()
        Cmb_Specie.ClearSelection()
        Cmb_Finalita.ClearSelection()
        Cmb_Cultivar.ClearSelection()
        TxtValiditaInizio.Text = ""
        TxtValiditaFine.Text = ""
        TxtSuperficie.Text = ""

        Cmb_TipologiaVarietale.ClearSelection()
        ChkConsociazione.Checked = False
        ChkCoverCrops.Checked = False
        ChkMonitorato.Checked = False
        Cmb_ImpIrrigazione.ClearSelection()
        Cmb_Portinnesto.ClearSelection()
        Cmb_SeminaTrapianto.ClearSelection()
        Cmb_ProvenienzaSeme.ClearSelection()
        TxtSuperficie2.Text = ""
        Txt_DistanzaSuFila_M.Text = ""
        Txt_DistanzaTraFila_M.Text = ""
        ChkFilaBinata.Checked = False
        Txt_Interbina.Text = ""
        Txt_Germinabilita.Text = ""
        TxtPianteHa.Text = ""
        TxtPianteImpianto.Text = ""
        Cmb_Copertura.ClearSelection()


        Cmb_Finalita.ClearSelection()
        Cmb_Cultivar.ClearSelection()
        TxtCopDataInizio.Text = ""
        TxtCopDataFine.Text = ""
        Cmb_DettaglioVarietaPersonalizzato.ClearSelection()
        ChkVarietaIbrida.Checked = False
        Txt_CodBMBDBT_M.Text = ""
        Txt_Genetica_M.Text = ""
        Txt_OffType_M.Text = ""
        Txt_CodBMBDBT_F.Text = ""
        Txt_Genetica_F.Text = ""
        Txt_OffType_F.Text = ""
        Txt_DistanzaSuFila_F.Text = ""
        Txt_DistanzaTraFila_F.Text = ""

    End Sub

#End Region

#Region "NewVersion"


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Esistono_Impianti_Su_Appezzamenti(ByVal data_inizio As String, ByVal data_fine As String) As RispostaStandard
        Dim r As New RispostaStandard With {
            .RispostaOK = True,
            .RispostaStringa = "ok"
        }

        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
        Dim objParametriAgenda As New ParametriAgenda

        If objParametriAgenda.Tipo_Operazione <> enum_TipoOperazioneDB.Modifica Then

            objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")
            objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(data_inizio, data_fine)

            Dim DTImpianto As DataTable

            'Creo gli oggetti COM+
            Dim objImpianto As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read  'New Agro_Anagrafe_AD.Reg_Impianti_Read
            'objImpianto = objServer....CreateCANCELLATOObject("Agro_Anagrafe_AD.Reg_Impianti_Read")

            'Recupero le info dell'impianto

            DTImpianto = objImpianto.Leggi(
                                        CStr(objParametriAgenda.Piva),
                                        CInt(objParametriAgenda.Sa_Cod),
                                        CInt(objParametriAgenda.Appezza),
                                        0,
                                         AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                         "ID_REG <> " & 0,
                                         "", objParametri_Server)

            'Distruggo l'oggetto COM+
            objImpianto = Nothing

            If DTImpianto.Rows.Count > 0 Then
                r.RispostaOK = False
                r.Errore = "Esiste già un impianto nell'appezzamento con le date selezionate."

            End If

            objParametri_Server.ResettaFinestra()

        End If

        Return r

    End Function



    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Aggiungi_Codice(ByVal codice As String, ByVal valore As String, ByVal codice_id As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim Dt As New DataTable
        Dim Dr As DataRow
        Dim Contatore As Integer
        Dim flag As Boolean = True



        If (IsNothing(HttpContext.Current.Session("dt_Codici_Ana"))) Then
            Dt.Columns.Add(New DataColumn("Id_Cod", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("Descrizione", GetType(String)))
            Dt.Columns.Add(New DataColumn("Val_Cod", GetType(String)))
            'Dt.Columns.Add(New DataColumn("Validita_Inizio", GetType(String)))
            'Dt.Columns.Add(New DataColumn("Validita_Fine", GetType(String)))
        Else
            Dt = HttpContext.Current.Session("dt_Codici_Ana")
        End If


        'Contatore = 1
        'Dim i As Integer
        Dim objCodiceAnagrafeR As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R

        'For i = 0 To Dt.Rows.Count - 1
        '    Contatore = Contatore + 1
        'Next




        'Controllo se esiste già la voce che si vuole inserire
        For i = 0 To Dt.Rows.Count - 1

            If (Dt.Rows(i).Item("Id_Cod") = codice_id) Then
                flag = False
            End If
        Next


        If (flag) Then

            'Creo una nuova riga
            Dr = Dt.NewRow

            'Definisco i valori

            'Dr.Item("Contatore") = Contatore


            Dr.Item("Id_Cod") = CInt(codice_id)
            'Dr.Item("Descrizione") = objCodiceAnagrafeR.CodiceAnagrafeDes_from_CodiceAnagrafeCod(codice_id, HttpContext.Current.Session("ASG_objParametri_Server"))
            Dr.Item("Descrizione") = codice
            Dr.Item("Val_Cod") = valore

            'Dr.Item("Validita_Inizio") = xValiditaInizio

            'Dr.Item("Validita_Fine") = xValiditaFine

            'Associo alla tabella la nuova riga creata
            Dt.Rows.Add(Dr)
            HttpContext.Current.Session("dt_Codici_Ana") = Dt
            Dim str_Risposta = DT_to_Json_Codici(Dt)
            r.RispostaOK = True
            r.RispostaStringa = str_Risposta
        Else
            r.RispostaOK = False
            r.Errore = "Codice già inserito"
        End If

        Return r

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Aggiungi_Particella(ByVal nome As String, ByVal valore As String, ByVal codice_id As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim Dt As New DataTable
        Dim Dr As DataRow
        Dim Contatore As Integer
        Dim flag As Boolean = True



        If (IsNothing(HttpContext.Current.Session("DT_Particelle"))) Then
            Dt.Columns.Add(New DataColumn("Id_Cod", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("Nome", GetType(String)))
            Dt.Columns.Add(New DataColumn("Val_Cod", GetType(String)))
            'Dt.Columns.Add(New DataColumn("Validita_Inizio", GetType(String)))
            'Dt.Columns.Add(New DataColumn("Validita_Fine", GetType(String)))
        Else
            Dt = HttpContext.Current.Session("dt_Codici_Ana")
        End If


        'Contatore = 1
        'Dim i As Integer
        Dim objCodiceAnagrafeR As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R

        'For i = 0 To Dt.Rows.Count - 1
        '    Contatore = Contatore + 1
        'Next




        'Controllo se esiste già la voce che si vuole inserire
        For i = 0 To Dt.Rows.Count - 1

            If (Dt.Rows(i).Item("Id_Cod") = codice_id) Then
                flag = False
            End If
        Next


        If (flag) Then

            'Creo una nuova riga
            Dr = Dt.NewRow

            'Definisco i valori

            'Dr.Item("Contatore") = Contatore


            Dr.Item("Id_Cod") = codice_id
            'Dr.Item("Descrizione") = objCodiceAnagrafeR.CodiceAnagrafeDes_from_CodiceAnagrafeCod(codice_id, HttpContext.Current.Session("ASG_objParametri_Server"))
            Dr.Item("Nome") = nome
            Dr.Item("Val_Cod") = valore

            'Dr.Item("Validita_Inizio") = xValiditaInizio

            'Dr.Item("Validita_Fine") = xValiditaFine

            'Associo alla tabella la nuova riga creata
            Dt.Rows.Add(Dr)
            HttpContext.Current.Session("DT_Particelle") = Dt
            Dim str_Risposta = DT_to_Json_Particelle(Dt)
            r.RispostaOK = True
            r.RispostaStringa = str_Risposta
        Else
            r.RispostaOK = False
            r.Errore = "Codice già inserito"
        End If

        Return r

    End Function



    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_Cmb_CapitolatoPrivato(ByVal valore As String)

        HttpContext.Current.Session("Cmb_CapitolatoPrivato") = valore

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_Cmb_MagazzinoConferimento(ByVal valore As String)

        HttpContext.Current.Session("Cmb_MagazzinoConferimento") = valore

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_Cmb_OrganismoReferente(ByVal valore As String)

        HttpContext.Current.Session("Cmb_OrganismoReferente") = valore

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_Cmb_RegolamentoConc(ByVal valore As String)

        HttpContext.Current.Session("Cmb_RegolamentoConc") = valore

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_Cmb_Stato(ByVal valore As String)

        HttpContext.Current.Session("Cmb_Stato") = valore

    End Function

    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_Cmb_Regolamento(ByVal valore As String)

        HttpContext.Current.Session("Cmb_Regolamento") = valore

    End Function

    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_Cmb_Specie(ByVal valore As String)

        HttpContext.Current.Session("Cmb_Specie") = valore

    End Function

    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_Cmb_Cultivar(ByVal valore As String)

        HttpContext.Current.Session("Cmb_Cultivar") = valore

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_Cmb_Finalita(ByVal valore As String)

        HttpContext.Current.Session("Cmb_Finalita") = valore

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_Cmb_TipologiaVarietale(ByVal valore As String)

        HttpContext.Current.Session("Cmb_TipologiaVarietale") = valore

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_Cmb_Copertura(ByVal valore As String)

        HttpContext.Current.Session("Cmb_Copertura") = valore

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_Cmb_FormaAllevamento(ByVal valore As String)

        HttpContext.Current.Session("Cmb_FormaAllevamento") = valore

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_Cmb_Portinnesto(ByVal valore As String)

        HttpContext.Current.Session("Cmb_Portinnesto") = valore

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_Cmb_DettaglioVarietaPersonalizzato(ByVal valore As String)

        HttpContext.Current.Session("Cmb_DettaglioVarietaPersonalizzato") = valore

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_Cmb_CodiciTerreno(ByVal valore As String)

        HttpContext.Current.Session("Cmb_CodiciTerreno") = valore

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_Cmb_ImpIrrigazione(ByVal valore As String)

        HttpContext.Current.Session("Cmb_ImpIrrigazione") = valore

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_Cmb_ProvenienzaSeme(ByVal valore As String)

        HttpContext.Current.Session("Cmb_ProvenienzaSeme") = valore

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_Cmb_SeminaTrapianto(ByVal valore As String)

        HttpContext.Current.Session("Cmb_SeminaTrapianto") = valore

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Select_Cultivar(ByVal parametro As String) As String

        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim cmb_cultivar As New DropDownList

        'AgronicaCoreUtility.CaricaListControl.ImpreseConFiltroUtente(cmb_cultivar, True, "SELEZIONA", "", _
        '                                                                 " Imprese.rag_soc LIKE '%" & parametro & "%'", " ORDER BY Rag_Soc asc", _
        '                                                                 HttpContext.Current.Session("ASG_objParametri_Server"), _
        '                                                                 HttpContext.Current.Session("ASG_objParametri_Utenti"))
        AgronicaCoreUtility.CaricaListControl.Cultivar(cmb_cultivar, True, "SELEZIONA", "", parametro, 0, "", True, 0, 0, "", "",
                                                                         objParametri_Server,
                                                                         HttpContext.Current.Session("ASG_objParametri_Utenti"))

        Dim rval As String = ""
        For Each itm As ListItem In cmb_cultivar.Items
            rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
        Next

        'Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        'HttpContext.Current.Session("prova") = "caio"

        Return rval

    End Function

    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Select_Finalita(ByVal parametro As String) As String

        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim cmb_finalita As New DropDownList

        AgronicaCoreUtility.CaricaListControl.Finalita(cmb_finalita, True, "SELEZIONA", "", parametro, 0, "", "", "",
                                                                         objParametri_Server)

        Dim rval As String = ""
        For Each itm As ListItem In cmb_finalita.Items
            rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
        Next


        Return rval

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Select_TipologiaVarietale(ByVal parametro As String) As String
        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim cmb_tv As New DropDownList

        AgronicaCoreUtility.CaricaListControl.GruppoVarietalexSpecie(cmb_tv, True, "SELEZIONA", "", parametro, "", "",
                                                                         objParametri_Server)

        Dim rval As String = ""
        For Each itm As ListItem In cmb_tv.Items
            rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
        Next


        Return rval

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Select_Allevamenti(ByVal parametro As String) As String

        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))


        Dim cmb_all As New DropDownList

        AgronicaCoreUtility.CaricaListControl.CaricaCombo_FormaAllevamento(cmb_all, True, "SELEZIONA", "", parametro, 0, "", "", "",
                                                                     objParametri_Server)
        'AgronicaCoreUtility.CaricaListControl.GruppoVarietalexSpecie(cmb_tv, True, "SELEZIONA", "", parametro, "", "", _
        '                                                                 HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim rval As String = ""
        For Each itm As ListItem In cmb_all.Items
            rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
        Next


        Return rval

    End Function



    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Select_Portinnesto(ByVal parametro As String) As String
        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim cmb_inn As New DropDownList

        AgronicaCoreUtility.CaricaListControl.CaricaCombo_Portinnesto(cmb_inn, True, "SELEZIONA", "", parametro, 0, "", "", "",
                                                                     objParametri_Server)
        'AgronicaCoreUtility.CaricaListControl.GruppoVarietalexSpecie(cmb_tv, True, "SELEZIONA", "", parametro, "", "", _
        '                                                                 HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim rval As String = ""
        For Each itm As ListItem In cmb_inn.Items
            rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
        Next


        Return rval

    End Function



    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Select_Copertura(ByVal parametro As String) As String
        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim cmb_cop As New DropDownList

        'AgronicaCoreUtility.CaricaListControl.CaricaCombo_Portinnesto(cmb_inn, True, "SELEZIONA", "", parametro, 0, "", "", "", _
        '                                                             HttpContext.Current.Session("ASG_objParametri_Server"))
        AgronicaCoreUtility.CaricaListControl.Copertura(cmb_cop, True, "SELEZIONA", "", parametro, 0, "", "", "", objParametri_Server)


        Dim rval As String = ""
        For Each itm As ListItem In cmb_cop.Items
            rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
        Next


        Return rval

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Select_Regolamento(ByVal parametro As String) As String

        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim cmb_reg As New DropDownList

        If parametro = "" Then
            AgronicaCoreUtility.CaricaListControl.Regolamento(CType(cmb_reg, ListControl),
                                                               False, "", "",
                                                               "", "", objParametri_Server)
        Else

            AgronicaCoreUtility.CaricaListControl.Regolamento(CType(cmb_reg, ListControl),
                                                             False, "", "",
                                                             " Reg_Cod = 4", "", objParametri_Server)
        End If


        Dim rval As String = ""
        For Each itm As ListItem In cmb_reg.Items
            rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
        Next


        Return rval

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Select_MagazzinoConferimento() As String
        'Public Shared Function Carica_Select_MagazzinoConferimento(ByVal parametro As String) As String

        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim cmb_cop As New DropDownList

        'AgronicaCoreUtility.CaricaListControl.MagazzinoConferimento_OrgReferente(
        '                                cmb_cop,
        '                                    True, "", "",
        '                                parametro,
        '                                objParametri_Server)

        AgronicaCoreUtility.CaricaListControl.MagazzinoConferimento_OrgReferente(
                                                     cmb_cop,
                                                    True, "", "",
                                                     "",
                                                     objParametri_Server)

        'AgronicaCoreUtility.CaricaListControl.Copertura(cmb_cop, True, "SELEZIONA", "", parametro, 0, "", "", "", HttpContext.Current.Session("ASG_objParametri_Server"))


        Dim rval As String = ""
        For Each itm As ListItem In cmb_cop.Items
            rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
        Next


        Return rval

    End Function




    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function EliminaProgetto(ByVal progetto_cod As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim Dt As DataTable
        Dt = HttpContext.Current.Session("Dt_Progetti")

        'Controllo se esiste già la voce che si vuole inserire
        For i = 0 To Dt.Rows.Count - 1
            If (Dt.Rows(i).Item("Progetto_Cod") = progetto_cod) Then
                Dt.Rows.RemoveAt(i)
                Exit For
            End If
        Next
        HttpContext.Current.Session("progetto_cod") = progetto_cod

        HttpContext.Current.Session("Dt_Progetti") = Dt
        Dim str_Risposta = DT_to_Json_Progetti(Dt)
        r.RispostaOK = True
        r.RispostaStringa = str_Risposta
        Return r
    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function DT_to_Json_Progetti(ByVal dt As DataTable) As String

        Dim objParametriAgenda As Integer
        objParametriAgenda = HttpContext.Current.Session("operazione")

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        'aggiungo i pulsanti per modifica ed eliminazione
        Dim c = New ColonneNome("Progetto_Cod", "Tool", "string")

        Dim listaBtn = New List(Of btnAzioni)

        If objParametriAgenda <> enum_TipoOperazioneDB.Lettura Then
            listaBtn.Add(New btnAzioni("fa-info info_elem", "InfoProgetti(this);"))
            listaBtn.Add(New btnAzioni("fa-trash-o del_elem", "EliminaProgetti(this);"))

        End If



        Dim tool As New ToolStandard(listaBtn)
        tool.cssColonna = "colmodmovimenti"
        c._FormatoParticolare = tool.toString()
        c._Filtrabile = False
        c._ColonnaDiSelezione = True
        l.Add(c)

        Dim cn As New ColonneNome("anno_validita", "Anno", "string")
        l.Add(cn)

        cn = New ColonneNome("Progetto_Nome", "Nome Progetto", "string")
        l.Add(cn)

        'cn = New ColonneNome(dt.Columns("Validita_Inizio"), "V_I")
        cn = New ColonneNome("Validita_Inizio", "Data Inizio", "date")
        l.Add(cn)

        cn = New ColonneNome("Validita_Fine", "Data Fine", "date")
        l.Add(cn)



        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable(dt, l)
        Return risp
    End Function


    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function DT_to_Json_Codici(ByVal dt As DataTable) As String

        Dim objParametriAgenda As Integer
        objParametriAgenda = HttpContext.Current.Session("operazione")

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        'aggiungo i pulsanti per modifica ed eliminazione
        Dim c = New ColonneNome("Id_Cod", "Tool", "string")

        Dim listaBtn = New List(Of btnAzioni)

        If objParametriAgenda <> enum_TipoOperazioneDB.Lettura Then
            listaBtn.Add(New btnAzioni("fa-pencil-square-o edit_elem", "ModificaCodice(this);"))
            listaBtn.Add(New btnAzioni("fa-trash-o del_elem", "EliminaCodice(this);"))
        End If

        Dim tool As New ToolStandard(listaBtn)
        tool.cssColonna = "colmodmovimenti"
        c._FormatoParticolare = tool.toString()
        c._Filtrabile = False
        c._ColonnaDiSelezione = True
        l.Add(c)


        Dim cn As New ColonneNome("descrizione", "Codice", "string")
        l.Add(cn)

        cn = New ColonneNome("Val_Cod", "Valore", "string")
        l.Add(cn)

        'cn = New ColonneNome("Validita_Inizio", "Dal", "string")
        'l.Add(cn)

        'cn = New ColonneNome("Validita_Fine", "Al", "string")
        'l.Add(cn)



        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable(dt, l)
        Return risp
    End Function


    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function DT_to_Json_Particelle(ByVal dt As DataTable) As String

        Dim objParametriAgenda As Integer
        objParametriAgenda = HttpContext.Current.Session("operazione")

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        'aggiungo i pulsanti per modifica ed eliminazione
        Dim c = New ColonneNome("Id_Cod", "Tool", "string")

        Dim listaBtn = New List(Of btnAzioni)

        If objParametriAgenda <> enum_TipoOperazioneDB.Lettura Then
            listaBtn.Add(New btnAzioni("fa-pencil-square-o edit_elem", "ModificaCodice(this);"))
            listaBtn.Add(New btnAzioni("fa-trash-o del_elem", "EliminaCodice(this);"))
        End If

        Dim tool As New ToolStandard(listaBtn)
        tool.cssColonna = "colmodmovimenti"
        c._FormatoParticolare = tool.toString()
        c._Filtrabile = False
        c._ColonnaDiSelezione = True
        l.Add(c)


        Dim cn As New ColonneNome("Nome", "Nome", "string")
        l.Add(cn)

        cn = New ColonneNome("Val_Cod", "Valore", "string")
        l.Add(cn)

        'cn = New ColonneNome("Validita_Inizio", "Dal", "string")
        'l.Add(cn)

        'cn = New ColonneNome("Validita_Fine", "Al", "string")
        'l.Add(cn)



        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable(dt, l)
        Return risp
    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Controlla_Date_Distinta_Su_Storico(ByVal data_inizio As String, ByVal data_fine As String, ByVal modalita As Integer, ByVal chiave As Integer) As String


        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim objProgetto As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
        Dim Dt As DataTable

        Dim d_inizio As Date
        Dim d_fine As Date

        Dim filtro_agg As String

        If modalita = "2" Then

            filtro_agg = "Progetto_Cod !=" & chiave
        Else
            filtro_agg = ""

        End If

        d_inizio = Date.Parse(data_inizio)
        d_fine = Date.Parse(data_fine)

        Dim objParametriAgenda As New ParametriAgenda

        Dt = objProgetto.Leggi(CStr(objParametriAgenda.Piva),
                            0,
                            CInt(9100),
                            0,
                            CInt(objParametriAgenda.Sa_Cod),
                            CInt(objParametriAgenda.Appezza),
                            CInt(objParametriAgenda.Id_Imp),
                            0, 0,
                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                            filtro_agg,
                            " Imprese_Progetti.Validita_Inizio DESC ",
                            objParametri_Server)


        Dim risp As String
        Dim flag As Boolean = True

        ' Ciclo su tutte le Distinte e controllo se ci sono sovrapposizioni
        For i = 0 To Dt.Rows.Count - 1

            If (d_inizio >= Dt.Rows(i).Item("Validita_Inizio") And d_inizio <= Dt.Rows(i).Item("Validita_Fine")) Then
                flag = False
                Exit For
            End If

            If (d_fine >= Dt.Rows(i).Item("Validita_Inizio") And d_fine <= Dt.Rows(i).Item("Validita_Fine")) Then
                flag = False
                Exit For
            End If

            ' Se la nuova Distinta ne contiene interamente un'altra
            If (d_inizio < Dt.Rows(i).Item("Validita_Inizio") And d_fine > Dt.Rows(i).Item("Validita_Fine")) Then
                flag = False
                Exit For
            End If

        Next

        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        'Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        'Dim risp As String = AgronicaCoreDataProvider.JSON_DataTable.Create_rows(Dt, JSON_DataTable.getListaColonneFromDT(Dt))

        If flag Then
            risp = "ok"
        Else
            risp = ""
        End If

        Return risp
    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Esiste_Obbligo_SalvataggioOrganismoReferente() As String

        Dim Dt As DataTable
        Dim x_Cod As Integer

        Dim objUtente As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

        Dt = objUtente.Leggi(enum_Impostazioni_Utenti.SUPERUSER_COD_ORG_REFERENTE_OBBLIGATORIO,
                                2,
                                enumSelezioneVariabile.Selezione_TabellaCompleta,
                                 "", "", HttpContext.Current.Session("ASG_objParametri_Utenti")
                             )


        Dim ret As String

        If Not IsNothing(Dt) Then

            If Dt.Rows.Count <> 0 Then
                If Dt.Rows(0).Item("Impostazione_Valore_1") = 1 Then
                    'Return True
                    ret = ""
                Else
                    'Return False
                    ret = "ok"
                End If
            Else
                'Return False
                ret = "ok"
            End If
        Else
            'Return False
            ret = "ok"
        End If


    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Trova_Impianto(ByVal progetto_cod As String) As String

        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))


        Dim objProgetto As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
        Dim Dt As DataTable

        Dim objParametriAgenda As New ParametriAgenda

        Dt = objProgetto.Leggi(CStr(objParametriAgenda.Piva),
                            progetto_cod,
                            CInt(9100),
                            0,
                            CInt(objParametriAgenda.Sa_Cod),
                            CInt(objParametriAgenda.Appezza),
                            CInt(objParametriAgenda.Id_Imp),
                            0, 0,
                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                            "",
                            " Imprese_Progetti.Validita_Inizio DESC ",
                            objParametri_Server)

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = AgronicaCoreDataProvider.JSON_DataTable.Create_rows(Dt, JSON_DataTable.getListaColonneFromDT(Dt))

        Riempi_Tab_Codici(progetto_cod)

        Return risp
    End Function

    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Riempi_Tab_Codici(ByVal progetto_cod As String) As String()

        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))


        Dim FiltroLetturaCodici As String = ""
        Dim Id_Cod As Integer
        Dim Val_Cod As String = ""
        Dim objParametriAgenda As New ParametriAgenda
        Dim jsCodici(4) As String

        FiltroLetturaCodici += " Reg_Impianti_Codici.progetto_cod = " + progetto_cod + " "

        Dim DT_codici As New DataTable
        Dim objCodici As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R
        DT_codici = objCodici.Leggi(CStr(objParametriAgenda.Piva),
                                           CInt(objParametriAgenda.Sa_Cod),
                                           CInt(objParametriAgenda.Appezza),
                                           CInt(objParametriAgenda.Id_Imp),
                                           "",
                                           CInt(Id_Cod),
                                           CStr(Val_Cod),
                                           AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta,
                                            FiltroLetturaCodici,
                                           "",
                                           objParametri_Server)

        For i = DT_codici.Rows.Count - 1 To 0 Step -1

            Select Case DT_codici.Rows(i).Item("descrizione")

                Case "Capitolato Privato"
                    jsCodici(1) = DT_codici.Rows(i).Item("val_cod")
                    DT_codici.Rows.RemoveAt(i)

                Case "Organismo Referente"
                    jsCodici(2) = DT_codici.Rows(i).Item("val_cod")
                    DT_codici.Rows.RemoveAt(i)

                Case "Magazzino Conferimento"
                    jsCodici(3) = DT_codici.Rows(i).Item("val_cod")
                    DT_codici.Rows.RemoveAt(i)

            End Select


        Next

        HttpContext.Current.Session("Operazione_Distinta") = "2"

        jsCodici(0) = DT_to_Json_Codici(DT_codici)

        Return jsCodici
    End Function

    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Riempi_Tab_Particelle(ByVal progetto_cod As String) As String
        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim FiltroLetturaCodici As String = ""
        Dim Id_Cod As Integer
        Dim Val_Cod As String = ""
        Dim objParametriAgenda As New ParametriAgenda
        Dim jsParticelle As String

        Dim StringaXML As String
        'variabili per spacchettamento stringa xml
        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XML_DatiProgetto As System.Xml.XmlElement
        Dim XML_Progetto As System.Xml.XmlElement
        'Dim XML_Fase As System.Xml.XmlElement
        'Dim XMLs_Fase As System.Xml.XmlNodeList
        'Dim XML_DatiCodici As System.Xml.XmlElement
        'Dim XML_CodiceImpianto As System.Xml.XmlElement
        'Dim XMLs_CodiceImpianto As System.Xml.XmlNodeList
        Dim XML_DatiParticelle As System.Xml.XmlElement
        Dim XML_Particella As System.Xml.XmlElement
        Dim XMLs_Particelle As System.Xml.XmlNodeList

        ' Create new DataTable instance.
        Dim tbl_Particelle As New DataTable

        tbl_Particelle.Columns.Add("Nome", GetType(String))
        tbl_Particelle.Columns.Add("Val_Cod", GetType(String))


        If progetto_cod <> 0 Then

            Dim objProgetto As New AgronicaCoreAnagrafeBIZ.Progetto_R
            StringaXML = objProgetto.Impresa_Progetti_Leggi(
                                                CStr(objParametriAgenda.Piva),
                                                CInt(progetto_cod),
                                                "",
                                                0,
                                                CInt(objParametriAgenda.Sa_Cod),
                                                CInt(objParametriAgenda.Appezza),
                                                CInt(objParametriAgenda.Id_Imp),
                                                0, 0,
                                                False,
                                                objParametri_Server)

            XmlDoc.LoadXml(StringaXML)

            '----- Tag DatiProgetto

            XML_DatiProgetto = XmlDoc.SelectSingleNode("DatiProgetto")

            '----- Tag Progetto

            XML_Progetto = XML_DatiProgetto.SelectSingleNode("Progetto")

            '--------------------------------------
            '--- Particelle x Progetto
            '--------------------------------------

            Dim StrParticella As String = ""
            Dim StrParticelle As String = ""
            Dim Prov As String
            Dim Com As String
            Dim Sezione As String
            Dim Foglio As Integer
            Dim Numero As Integer
            Dim Subalterno As String
            Dim StrCodProvincia As String
            Dim StrCodComune As String
            Dim StrSezione As String
            Dim StrFoglio As String
            Dim StrNumero As String
            Dim StrSubalterno As String
            Dim Testo As String
            Dim Valore As String

            Dim BaseCode As Integer
            Dim TopCode As Integer

            Call Calcola_BaseCode_TopCode(BaseCode,
                                      TopCode,
                                      HttpContext.Current.Session("ASG_ProgressivoGIAS"))

            XML_DatiParticelle = XML_Progetto.SelectSingleNode("DatiParticellexProgetto")



            If Not XML_DatiParticelle Is Nothing Then

                If XML_DatiParticelle.HasChildNodes Then

                    '----- Tag Particella  (multiplo)

                    'Recupero la collezione dei nodi
                    XMLs_Particelle = XML_DatiParticelle.GetElementsByTagName("ParticellaxProgetto")

                    If XMLs_Particelle.Count > 0 Then

                        StrParticelle = ""




                        For i = 0 To XMLs_Particelle.Count - 1

                            XML_Particella = XMLs_Particelle.Item(i)

                            Prov = XML_Particella.GetAttribute("prov")
                            Com = XML_Particella.GetAttribute("com")
                            Sezione = XML_Particella.GetAttribute("sezione")
                            Foglio = XML_Particella.GetAttribute("foglio")
                            Numero = XML_Particella.GetAttribute("numero")
                            Subalterno = XML_Particella.GetAttribute("subalterno")

                            Id_Cod = CInt(XML_Particella.GetAttribute("id_cod"))
                            Val_Cod = XML_Particella.GetAttribute("val_cod")



                            'Genero l'XML del singolo nodo
                            Call XML_ParticellaxProgetto(enum_CodificaDecodifica.Codifica,
                                StrParticella,
                                enum_TipoOperazioneDB.Cancellazione,
                                Prov,
                                Com,
                                Sezione,
                                Foglio,
                                Numero,
                                Subalterno,
                                Id_Cod,
                                Val_Cod,
                                CDate(XML_Particella.GetAttribute("validita_inizio")),
                                CDate(XML_Particella.GetAttribute("validita_fine")),
                                BaseCode,
                                TopCode)

                            tbl_Particelle.Rows.Add(StrParticella, Val_Cod)

                            ''Inserisco l'XML nella stringa complessiva
                            'StrParticelle = StrParticelle & StrParticella

                            'Select Case Id_Cod

                            '    Case enum_CodiciAnagrafe.CodiceRigaRiferimentoQuadroP

                            '        StrCodProvincia = Left("_" & Prov & "_", 10)
                            '        StrCodComune = Left("_" & Com & "_", 10)

                            '        If Sezione = "0" Then
                            '            StrSezione = Left("_", 6)
                            '        Else
                            '            StrSezione = Left("_" & Sezione & "_", 6)
                            '        End If

                            '        StrFoglio = Left("_" & Foglio & "_", 7)
                            '        StrNumero = Left("_" & Numero & "_", 7)

                            '        If Subalterno = "0" Then
                            '            StrSubalterno = Left("_", 7)
                            '        Else
                            '            StrSubalterno = Left("_" & Subalterno & "_", 7)
                            '        End If

                            '        Testo = StrCodProvincia & " : " & _
                            '          StrCodComune & " : " & _
                            '          StrSezione & " : " & _
                            '          StrFoglio & " : " & _
                            '          StrNumero & " / " & _
                            '          StrSubalterno

                            '        Valore = "" & Prov & _
                            '          "£" & Com & _
                            '          "£" & Sezione & _
                            '          "£" & Foglio & _
                            '          "£" & Numero & _
                            '          "£" & Subalterno

                            '        'Me.ListParticelle.Items.Add(New ListItem(Testo & " = " & Val_Cod, _
                            '        '     Valore))

                            'End Select

                        Next

                    End If

                End If

            End If

        End If

        HttpContext.Current.Session("DT_Particelle") = tbl_Particelle

        jsParticelle = DT_to_Json_Particelle(tbl_Particelle)

        Return jsParticelle
    End Function


    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Riempi_Info_Impianto(ByVal progetto_cod As String) As String()

        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim objParametriAgenda As New ParametriAgenda
        Dim Dt_Progetti As DataTable
        Dim jsInfo(10) As String

        'Creo l'oggetto COM+
        Dim objImpresa_Progetti = New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R

        'Mi procuro il recordset richiesto
        Dt_Progetti = objImpresa_Progetti.Leggi(
                                        CStr(objParametriAgenda.Piva),
                                        CInt(progetto_cod),
                                        "",
                                        0,
                                        CInt(objParametriAgenda.Sa_Cod),
                                        CInt(objParametriAgenda.Appezza),
                                        CInt(objParametriAgenda.Id_Imp),
                                        0,
                                        0,
                                        AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                        "",
                                        "",
                                        objParametri_Server)

        If Not IsNothing(Dt_Progetti) Then
            ' N° piante/impianto
            jsInfo(0) = Dt_Progetti.Rows(0).Item("p_ha")
            ' Data Semina prevista ?
            jsInfo(1) = Dt_Progetti.Rows(0).Item("data_inizio_prevista")
            ' Data Fioritura prevista
            jsInfo(2) = Dt_Progetti.Rows(0).Item("data_fioritura_prevista")
            ' Data Raccolta prevista ?
            jsInfo(3) = Dt_Progetti.Rows(0).Item("data_fine_prevista")
            ' Resa [Kg/Ha] ?
            jsInfo(4) = Dt_Progetti.Rows(0).Item("giudizio")
            ' Resa prevista
            jsInfo(5) = Dt_Progetti.Rows(0).Item("produzione_prevista")
            ' Disciplinare
            jsInfo(6) = Dt_Progetti.Rows(0).Item("disciplinare_cod")
            ' Regolamento
            jsInfo(7) = Dt_Progetti.Rows(0).Item("regolamento_cod")
            ' Regolamento concimazioni
            jsInfo(8) = Dt_Progetti.Rows(0).Item("regolamento_concimazioni_cod")
            ' Stato Impianto
            jsInfo(9) = Dt_Progetti.Rows(0).Item("stato_impianto")
            ' Nome Progetto
            jsInfo(10) = Dt_Progetti.Rows(0).Item("Progetto_Nome")

        End If


        ' Salvo nella Session la Distinta CORRENTE
        HttpContext.Current.Session("Distinta_corrente") = Dt_Progetti

        Return jsInfo
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Svuota_Distinta()

        HttpContext.Current.Session("Distinta_corrente") = Nothing
        HttpContext.Current.Session("Operazione_Distinta") = "1"

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Select_StatoImpianto(ByVal specie As String, ByVal finalita As String, ByVal regolamento As String) As String
        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim cmb_finalita As New DropDownList

        'Dim objStato As New AgronicaCoreUtility.CaricaListControl
        CaricaListControl.FasiClicloColturale(cmb_finalita,
                                     True, "Impianto in Produzione", "102",
                                     specie,
                                     finalita,
                                     Math.Abs(CInt(regolamento)),
                                     "",
                                     "", objParametri_Server)

        Dim rval As String = ""
        For Each itm As ListItem In cmb_finalita.Items
            rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
        Next


        Return rval

    End Function

#End Region


End Class