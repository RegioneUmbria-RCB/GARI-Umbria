Imports System.Data
Imports System.Xml
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreUtility

Public Class DocumentoContabileGenerico
    Inherits System.Web.UI.Page

    Dim objParametriAgenda As ParametriAgenda_2010

    'Colonne del datagrid prodotti
    Const COL_CHIAVE As Integer = 0
    Const COL_CODICE As Integer = 1
    Const COL_PENDENTE As Integer = 2
    Const COL_ALLEGATO As Integer = 3
    Const COL_TIPO As Integer = 4

    Const COL_ELEM_COD As Integer = 5
    Const COL_PRO_COD As Integer = 6
    Const COL_MAT_COD As Integer = 7
    Const COL_COD_PROGETTO As Integer = 8
    Const COL_FASE_COD As Integer = 9
    Const COL_LOTTO As Integer = 10
    Const COL_CAL_COD As Integer = 11
    Const COL_UDM_COD As Integer = 12

    Const COL_UDM_COD_EXTRA As Integer = 13
    Const COL_QTA_EXTRA As Integer = 14
    Const COL_PREZZO_EFFETTIVO As Integer = 15

    Const COL_DESCR As Integer = 16
    Const COL_UDM_DES As Integer = 17
    Const COL_QTA As Integer = 18

    Const COL_Prezzo_Unitario As Integer = 19
    Const COL_Prezzo_Unitario_Netto As Integer = 20
    Const COL_Cod_Variazione As Integer = 21
    Const COL_Variazione_Perc As Integer = 22
    Const COL_Variazione As Integer = 23
    Const COL_Imponibile As Integer = 24
    Const COL_Imponibile_Netto As Integer = 25
    Const COL_Cod_Iva As Integer = 26
    Const COL_Aliquota As Integer = 27
    Const COL_IVA As Integer = 28
    Const COL_Importo As Integer = 29
    Const COL_Anno As Integer = 30
    Const COL_Ric_Cod As Integer = 31
    Const COL_Cod_Conto As Integer = 32
    Const COL_Conto As Integer = 33
    Const COL_Piva_Destinazione As Integer = 34
    Const COL_Sa_Cod_Destinazione As Integer = 35
    Const COL_Id_Agenda_Destinazione As Integer = 36
    Const COL_Id_Mov_Destinazione As Integer = 37
    Const COL_Id_Mov_Det_Destinazione As Integer = 38
    Const COL_Id_Destinazione As Integer = 39
    Const COL_Destinazione As Integer = 40
    Const COL_Piva_Provenienza As Integer = 41
    Const COL_Sa_Cod_Provenienza As Integer = 42
    Const COL_Id_Agenda_Provenienza As Integer = 43
    Const COL_Id_Mov_Provenienza As Integer = 44
    Const COL_Id_Mov_Det_Provenienza As Integer = 45
    Const COL_Id_Provenienza As Integer = 46
    Const COL_Provenienza As Integer = 47
    Const COL_Xml_Destinazione As Integer = 48
    Const COL_Xml_Provenienza As Integer = 49


    Const COL_MODIFICA As Integer = 50
    Const COL_ELIMINA As Integer = 51


    'indici della toolbar
    Const T_PRODOTTO As Integer = 0
    Const T_ZOO As Integer = 1
    Const T_ALLEGATO As Integer = 3
    Const T_ELIMINA As Integer = 5
    Const T_STAMPA As Integer = 6

    Const str_COL_PIVA_DEST As String = "Piva_Destinazione"
    'Const str_COL_SACOD_DEST As String = "Sa_Cod_Destinazione"
    'Const str_COL_ID_DEST As String = "Id_Destinazione"
    'Const str_COL_DESTINAZIONE As String = "Destinazione"
    'Const str_COL_XML_DEST As String = "Xml_Destinazione"
    Const str_COL_ID_AGENDA_DEST As String = "Id_Agenda_Destinazione"
    Const str_COL_ID_MOV_DEST As String = "Id_Mov_Destinazione"
    Const str_COL_ID_MOV_DET_DEST As String = "Id_Mov_Det_Destinazione"

    Const str_COL_PIVA_PROV As String = "Piva_Provenienza"
    'Const str_COL_SACOD_PROV As String = "Sa_Cod_Provenienza"
    'Const str_COL_ID_PROV As String = "Id_Provenienza"
    'Const str_COL_PROVENIENZA As String = "Provenienza"
    'Const str_COL_XML_PROV As String = "Xml_Provenienza"
    Const str_COL_ID_AGENDA_PROV As String = "Id_Agenda_Provenienza"
    Const str_COL_ID_MOV_PROV As String = "Id_Mov_Provenienza"
    Const str_COL_ID_MOV_DET_PROV As String = "Id_Mov_det_Provenienza"


    '----- Gestione della pagina transazionale
    Dim EseguitaOperazione As Boolean
    Dim PremutoAnnulla As Boolean

    'Dim UtenteAbilitatoModifica As Boolean

    '----- Gestione Querystring
    Dim Qs_Piva As String
    Dim Qs_SaCod As Integer
    Dim Qs_DataSelezionata As String
    Dim Qs_IdAgenda As Integer
    Dim Qs_Operazione As Integer
    Dim Qs_PagRitorno As String
    Dim QS_LavCod As Integer
    Dim Qs_Rag_Soc As String
    Dim Qs_Servizio_Cod As Integer
    'Dim Qs_TipoFattura As Integer


    '----- Variabili per il recupero dati dai controlli TESTATA
    Dim x_XML_Pagamenti As String

    Dim x_Des_Lib As String
    Dim x_Cau_Mov As String
    Dim x_Cau_Mov_Magazzino As String

    Dim x_Mov_Desc_Contabile As String
    Dim x_Mov_Desc_Magazzino As String
    Dim x_Extra_Str As String
    Dim x_Data_Emissione As String
    Dim x_Data_Registrazione As Date
    Dim x_Scadenza As Date
    Dim x_Extra_Date As Date

    Dim x_Num_Protocollo As Decimal
    Dim x_Doc_Numero_Sin As String
    Dim x_Doc_Numero As Decimal
    Dim x_Doc_Numero_Des As String
    Dim x_Tipo_Sconto As Integer
    Dim x_Extra_Int As Integer
    Dim x_Peso As Integer

    Dim frm_Aspetto As String
    Dim frm_Consegna As String
    Dim frm_CausaleTrasporto As String
    Dim frm_NaturaBeni As String
    Dim frm_Ora As String
    Dim frm_Colli As Integer
    Dim frm_TaraVeicolo As Decimal



    Dim x_Cod_RisUm As Integer
    Dim x_Cod_IndirizzoRisUm As Integer
    Dim x_Cod_Destinazione As Integer
    Dim x_Cod_IndirizzoDestinazione As Integer
    Dim x_Mezzo As Integer
    Dim x_Cod_Vettore As Integer
    Dim x_Cod_IndirizzoVettore As Integer
    'Dim frm_Cod_RisUm_Extra As Integer = 0

    'Dim frm_Mac_Cod As Integer = 0
    Dim frm_PesoTara As Decimal = 0
    Dim frm_Targa As String
    Dim frm_Immatricolazione As String
    Dim frm_ImmatrRimorchio As String
    Dim frm_Autorizzizzazione As String
    Dim frm_Data_Autorizzazione As Date

    '----- Variabili per il recupero dati dai controlli DETTAGLI

    Dim x_Pendente As Integer
    Dim x_Allegato As String
    Dim x_Tipo As String

    Dim x_Elem_Cod As Integer
    Dim x_Pro_Cod As Integer
    Dim x_Mat_Cod As Integer
    Dim x_Cod_Progetto As Integer
    Dim x_Fase_Cod As Integer
    Dim x_Lotto As String
    Dim x_Cal_Cod As Integer
    Dim x_Udm_Cod As Integer

    Dim x_Udm_Cod_Extra As Integer
    Dim x_Qta_Extra As Decimal
    Dim x_Prezzo_Effettivo As Decimal

    Dim x_Descrizione As String
    Dim x_Mov_Det_Des As String
    Dim x_Udm_Sim As String
    Dim x_Udm_Des As String
    Dim x_Quantita As Decimal

    Dim x_Prezzo_Unitario As Decimal
    Dim x_Prezzo_Unitario_Netto As Decimal
    Dim x_Sconto_Perc As Decimal
    Dim x_Sconto As Decimal
    Dim x_Imponibile As Decimal
    Dim x_Imponibile_Netto As Decimal
    Dim x_Cod_IVA As Integer
    Dim x_Aliquota As String
    Dim x_IVA As Decimal
    Dim x_Anno As Integer
    Dim x_Ric_Cod As Integer
    Dim x_Cod_Conto As Integer
    Dim x_Conto As String

    Dim x_N As Decimal
    Dim x_P2O5 As Decimal
    Dim x_K2O As Decimal
    Dim x_Cu As Decimal
    Dim x_Regolamento_Cod_Ferti As Integer

    '----- Variabili

    Dim x_Piva_Dest As String
    Dim x_SaCod_Dest As String
    Dim x_IdAgenda_Dest As Integer
    Dim x_IdMov_Dest As Integer
    Dim x_IdMovDet_Dest As Integer
    Dim x_Id_Destinazione As Integer
    Dim x_Destinazione As String
    Dim x_XML_Destinazione As String

    Dim x_Piva_Prov As String
    Dim x_SaCod_Prov As String
    Dim x_IdAgenda_Prov As Integer
    Dim x_IdMov_Prov As Integer
    Dim x_IdMovDet_Prov As Integer
    Dim x_Id_Provenienza As Integer
    Dim x_Provenienza As String
    Dim x_XML_Provenienza As String

    Dim x_BaseCode As Integer
    Dim x_TopCode As Integer

    'Variabili utilizzate nei riferimenti
    Dim r_Piva As String
    Dim r_Sa_Cod As Integer = 0
    Dim r_Id_Destinazione As Integer = 0
    Dim r_Lav_Cod As Integer
    Dim r_Cau_Mov As String
    Dim r_Descrizione As String
    Dim r_Cau_Mov_Rif As String
    Dim r_DataOperazione As String
    Dim r_Consegna As String
    Dim r_Ora As String
    Dim r_NumFattura As String
    Dim r_NumFattura_Sin As String
    Dim r_NumFattura_Des As String
    Dim r_BaseCode As Integer = 0
    Dim r_TopCode As Integer = 0

    Dim v_Tot_Imponibile_Lordo As Decimal
    Dim v_Tot_Variazioni As Decimal
    Dim v_Tot_Imponibile_Netto As Decimal
    Dim v_Tot_Iva As Decimal
    Dim v_Tot_Importo As Decimal

    Dim v_Flag_AncheImportati As Boolean = False

    Dim objParametri_Server As New AgronicaCoreParametri
    Dim objParametri_Utenti As New AgronicaCoreParametri


    Private Sub GestistiSessioneScaduta()
        Response.Redirect("../../default.aspx")
    End Sub




    Private Sub fine_salvataggio(ByVal TipoSalvataggio As Integer, ByVal upPanel As UpdatePanel, ByVal isPopUp As Boolean)

        Dim strJS As New StringBuilder

        Select Case TipoSalvataggio
            Case 1

                If isPopUp Then
                    If upPanel Is Nothing Then
                        strJS.AppendLine("<script> ")
                    End If

                    strJS.AppendLine("$(document).ready(function () { ")
                    strJS.AppendLine("      window.close(); ")
                    strJS.AppendLine(" });")
                    If upPanel Is Nothing Then
                        strJS.AppendLine("</script> ")
                    End If
                Else
                    AnnullaTutto(Nothing, Nothing)
                End If

            Case 2
                DocumentoContabileGenerico_Disposed()

                'todo

                Dim OpUtil As New AgronicaCoreModello.Utility_Operazioni
                Dim objParametriAgenda As New ParametriAgenda
                objParametriAgenda.Piva = Qs_Piva
                objParametriAgenda.RagSoc = Qs_Rag_Soc
                objParametriAgenda.Lav_Cod = QS_LavCod

                Dim TargetUrl As String = OpUtil.LinkPagina_from_LavCod_NEW(QS_LavCod, objParametriAgenda)

                If upPanel Is Nothing Then
                    strJS.AppendLine("<script> ")
                End If

                strJS.AppendLine("$(document).ready(function () { ")
                strJS.AppendLine("      window.location = '" & TargetUrl & "'; ")
                strJS.AppendLine(" });")
                If upPanel Is Nothing Then
                    strJS.AppendLine("</script> ")
                End If

        End Select


        If upPanel IsNot Nothing Then
            ScriptManager.RegisterStartupScript( _
                upPanel, _
                upPanel.GetType(), _
                    String.Format("jQuery_{0}", upPanel.ClientID), strJS.ToString, True)
        Else
            Me.Controls.Add(New LiteralControl(strJS.ToString))
        End If

    End Sub

    Private Sub DocumentoContabileGenerico_Disposed()

    End Sub

    Public Master_Operazione As Agenda
    Private Sub DocumentoContabileGenerico_Init(sender As Object, e As System.EventArgs) Handles Me.Init

        Master_Operazione = CType(Page.Master, Agenda)
        AddHandler CType(Page.Master.FindControl("ImgBtn_AnnullaTutto"), ImageButton).Click, AddressOf Me.AnnullaTutto


    End Sub


    Private Sub CaricaComboXddt()

        Select Case QS_LavCod

            Case LAVCOD_BOLLA_RICEVUTA
                CaricaCombo_Causale("CONTO ACQUISTO")

            Case LAVCOD_BOLLA_EMESSA
                CaricaCombo_Causale("CONTO VENDITA")

            Case LAVCOD_FATTURA_RICEVUTA, LAVCOD_FATTURA_EMESSA

                If Me.Rbl_TipoFattura.SelectedValue = "1" Then
                    CaricaCombo_Causale("CONTO VENDITA")
                End If

            Case LAVCOD_NOTA_ACCREDITO_EMESSA, LAVCOD_NOTA_ACCREDITO_RICEVUTA

                CaricaCombo_Causale("MERCE RESA")

        End Select

        CaricaCombo_Aspetto()

    End Sub

    Private Sub AnnullaTutto(sender As Object, ByVal e As System.EventArgs)

        PremutoAnnulla = True

        AAA_GestioneUscitaPagina()
        If CInt(Qs_PagRitorno) = enum_PagineAgenda_2010.Menu Then
            Response.Redirect("../Menu/Menu.aspx")
        End If

        If CInt(Qs_PagRitorno) = enum_PagineAgenda_2010.Menu_BS Then
            Response.Redirect("../Menu/MenuBS_Agenda_Nuovo.aspx")
        End If

    End Sub

    '######################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        InizializzaScripts()

        'Allungo il timeout dai 180 secondi di default (3 minuti) a 900 secondi (15 minuti)
        Server.ScriptTimeout = 900

        Dim Messaggio As String = ""

        Try


            '##############################################################
            '#####  Verifico Credenziali di Accesso  ######################
            '##############################################################

            '----- Verifico che l'utente sia autenticato

            If Session("ASG_Utente_Username") = "" Then
                GestistiSessioneScaduta()
            End If

            objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
            objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))

            '##############################################################
            '###################  QUERY STRING  ###########################
            '##############################################################
            If Not IsNothing(Request.QueryString("o")) Then

                Qs_Operazione = CInt(Stringa_Decodifica(Request.QueryString("o").ToString, _
                                         AgroKey_EncoderDecoder, _
                                         Server))
            Else
                Qs_Operazione = CStr(0)
            End If
            'If InStr(Request.QueryString.ToString, "&orig=") <> 0 Then

            '    QS_PaginaRitorno = Stringa_Decodifica(Request.QueryString("orig").ToString, _
            '                           AgroKey_EncoderDecoder, _
            '                           Server)
            'Else
            '    'QS_PaginaRitorno = "../MenuContabilita.aspx?p=" & Request.QueryString("p").ToString & "&d=" & Request.QueryString("d").ToString
            '    QS_PaginaRitorno = ""
            'End If

            'pagina chiamante --> per gestire il tipo di uscita dalla pagina:
            'fare un redirect o chiuderla (perché aperta in modal dialog)
            'usa enum_PagineGiasOnline
            If Not IsNothing(Request.QueryString("orig")) Then

                Qs_PagRitorno = Stringa_Decodifica(Request.QueryString("orig").ToString, _
                              AgroKey_EncoderDecoder, Server)

            Else
                Qs_PagRitorno = CStr(0)
            End If

            If Not IsNothing(Request.QueryString("p")) Then
                Qs_Piva = Stringa_Decodifica(Request.QueryString("p").ToString, _
                                            AgroKey_EncoderDecoder, Server)
            Else
                Qs_Piva = CStr(0)
            End If


            If InStr(Request.QueryString.ToString, "&s=") <> 0 Then

                Qs_SaCod = CInt(Stringa_Decodifica(Request.QueryString("s").ToString, _
                                        AgroKey_EncoderDecoder, _
                                        Server))
            Else
                Qs_SaCod = 0
            End If

            If InStr(Request.QueryString.ToString, "&i=") <> 0 Then

                Qs_IdAgenda = CInt(Stringa_Decodifica(Request.QueryString("i").ToString, _
                                         AgroKey_EncoderDecoder, Server))
            Else
                Qs_IdAgenda = 0
            End If

            If Not IsNothing(Request.QueryString("l")) Then
                QS_LavCod = CInt(Stringa_Decodifica(Request.QueryString("l").ToString, _
                                           AgroKey_EncoderDecoder, Server))

            Else
                QS_LavCod = 0
            End If

            If InStr(Request.QueryString.ToString, "&d=") <> 0 Then

                Qs_DataSelezionata = Stringa_Decodifica(Request.QueryString("d").ToString, _
                                       AgroKey_EncoderDecoder, _
                                       Server)
            Else
                Qs_DataSelezionata = CStr(Date.Today)
            End If


            If Not IsNothing(Request.QueryString("rs")) Then
                Qs_Rag_Soc = Stringa_Decodifica(Request.QueryString("rs").ToString,
                           AgroKey_EncoderDecoder,
                           Server)
            Else
                Qs_Rag_Soc = CStr(Date.Today)
            End If

            If Not IsNothing(Request.QueryString("sc")) Then
                Qs_Servizio_Cod = Stringa_Decodifica(Request.QueryString("sc").ToString,
                           AgroKey_EncoderDecoder,
                           Server)
            Else
                Qs_Servizio_Cod = enum_Servizi.Quaderno_Campagna_Caa
            End If

            Dim datamin As Date = AGRODATAINIZIO
            Dim dataMax As Date = AGRODATAFINE
            Dim sportelloAperto As Boolean = True
            Dim objPratiche As New AgronicaCoreProfilazioneBIZ.Pratiche_R
            objPratiche.Data_Sportello_Da_Servizio(Qs_Piva,
                                                       enum_Servizi.Quaderno_Campagna_Caa, 'Qs_Servizio_Cod,
                                                       DateTime.Now,
                                                       sportelloAperto,
                                                       datamin,
                                                       dataMax,
                                                       objParametri_Server,
                                                       objParametri_Utenti)

            objPratiche.Sportello_ChiamataSecondaria_SeNessunCambiamento(Qs_Piva,
                                                                        enum_Servizi.QuadernoCampagnaBio,
                                                                        DateAndTime.Now,
                                                                        True,
                                                                        AGRODATAINIZIO,
                                                                        AGRODATAFINE,
                                                                        sportelloAperto,
                                                                        datamin,
                                                                        dataMax,
                                                                        objParametri_Server,
                                                                        objParametri_Utenti)

            If sportelloAperto Then
                Me.ValiditaInizioSportello.Value = datamin
                Me.ValiditaFineSportello.Value = dataMax
            Else
                Me.ValiditaInizioSportello.Value = ""
                Me.ValiditaFineSportello.Value = ""
            End If



            'If Not String.IsNullOrEmpty(Request.QueryString("tf")) Then
            '    Qs_TipoFattura = CInt(Stringa_Decodifica(Request.QueryString("tf").ToString, _
            '                              AgroKey_EncoderDecoder, _
            '                              Server))
            'End If


            ImpostazioniDocumentoDatoLAV_COD()



            '##############################################################
            '#####  Verifico se sono in Post-Back  ########################
            '##############################################################

            If Not Page.IsPostBack Then

                '==========================================
                '===== Pagina caricata per la prima volta
                '==========================================

                caricaImpostazioniUtente()

                Me.Rbl_TipoFattura.SelectedValue = "0"
                CaricaComboXddt()

            Else

                '==========================================
                '===== Pagina ricaricata in POSTBACK
                '==========================================


                '//////////////////////////////////////////////////////
                '            RITORNO DALLA FORM PRODOTTO
                '\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

                'ritorno dalla FormProdotto: devo inserire i prodotti nel dettaglio
                Select Case Me.InsProdotto.Value

                    Case "0", "", "undefined" 'NO
                        'niente

                    Case Else 'SI
                        '  Case "999" 'SI

                        Dim Unid As String

                        Unid = Me.InsProdotto.Value

                        SelezionaTab(upTabs_1, 1)

                        Try

                            Dim Dt As DataTable
                            Dim Dt_Prodotti, Dt_IVA As DataTable
                            Dim Dr_Prodotti As DataRow
                            Dim hash_iva As Hashtable
                            Dim i As Integer
                            Dim Errore, Chiave_Oggetto, Stringa_Parametri_Base, Stringa_Parametri_Rif, Data, Username As String
                            Dim Id_Riga, Tipo_Operazione, Flag_Errore As Integer

                            ViewState("UtenteAbilitatoModifica") = True

                            Dt = NewCom_Web_ComunicazionePagine_Leggi(objParametri_Server, Session, Page, Unid)

                            If Not IsNothing(Dt) Then

                                If Dt.Rows.Count <> 0 Then

                                    If IsNothing(Session("DT_Prodotti_nel_Doc")) Then
                                        CaricaGriglia_Dettagli()
                                        Dt_Prodotti = Session("DT_Prodotti_nel_Doc")
                                    Else
                                        Dt_Prodotti = Session("DT_Prodotti_nel_Doc")
                                    End If

                                    If IsNothing(Session("DT_IVA_nel_Doc")) Then
                                        hash_iva = New Hashtable
                                    Else
                                        Dt_IVA = Session("DT_IVA_nel_Doc")
                                        hash_iva = Ricava_HashIVA_da_DtIVA(Dt_IVA)
                                    End If


                                    Dim XmlDoc As New XmlDocument


                                    For i = 0 To Dt.Rows.Count - 1

                                        Unid = Dt.Rows(i).Item("Unid")
                                        Errore = Dt.Rows(i).Item("Errore")
                                        Chiave_Oggetto = Dt.Rows(i).Item("Chiave_Oggetto")
                                        Stringa_Parametri_Base = Dt.Rows(i).Item("Stringa_Parametri_Base")
                                        Stringa_Parametri_Rif = Dt.Rows(i).Item("Stringa_Parametri_Rif")
                                        Data = Dt.Rows(i).Item("Data")
                                        Username = Dt.Rows(i).Item("Username")

                                        Id_Riga = Dt.Rows(i).Item("Id_Riga")
                                        Tipo_Operazione = Dt.Rows(i).Item("Tipo_Operazione")
                                        Flag_Errore = Dt.Rows(i).Item("Flag_Errore")

                                        'se sono in modifica
                                        If Tipo_Operazione = enum_TipoOperazioneDB.Modifica Then

                                            'CANCELLO QUEL PRODOTTO

                                            'Trovo la riga da cancellare tramite la chiave
                                            Dr_Prodotti = Dt_Prodotti.Rows.Find(Chiave_Oggetto)

                                            'Elimino la riga
                                            Dr_Prodotti.Delete()

                                            Session("DT_Prodotti_nel_Doc") = Dt_Prodotti

                                        End If

                                        Try

                                            'devo impostare il cau_mov perché viene utilizzato
                                            'da XML_Leggi_MovimentoDettaglio
                                            Select Case QS_LavCod
                                                Case LAVCOD_FATTURA_EMESSA, LAVCOD_BOLLA_EMESSA, LAVCOD_NOTA_ACCREDITO_RICEVUTA
                                                    x_Cau_Mov = CAU_SCARICO

                                                Case LAVCOD_FATTURA_RICEVUTA, LAVCOD_BOLLA_RICEVUTA, LAVCOD_NOTA_ACCREDITO_EMESSA
                                                    x_Cau_Mov = CAU_CARICO
                                            End Select

                                            'leggo il dettaglio e lo inserisco in griglia
                                            XML_Leggi_MovimentoDettaglio(XmlDoc, Stringa_Parametri_Base, Stringa_Parametri_Rif, hash_iva, False, CDate(Me.Txt_DataEmissione.Text), Nothing)

                                        Catch ex As Exception

                                            Messaggi.AgroMsgBox("Errore durante la lettura dei dati ricevuti dalla pagina di gestione del magazzino.", Page)

                                        End Try

                                    Next 'x ogni riga scritta nella tabella di comunicazione

                                    'Associo il DataTable con la DataGrid
                                    Me.DataGrid_Prodotti.DataSource = Dt_Prodotti
                                    Me.DataGrid_Prodotti.DataBind()

                                    Call CaricaGriglia_IVA(hash_iva)

                                    Call Ricava_RiepilogoDocumento_da_DtProdotti(Dt_Prodotti)

                                    Call Visualizza_RiepilogoDocumento()

                                Else
                                    Messaggi.AgroMsgBox("Non sono arrivati i dati dalla pagina di gestione del magazzino.", Page)
                                End If
                            Else
                                Messaggi.AgroMsgBox("Errore durante la ricezione dei dati dalla pagina di gestione del magazzino.", Page)
                            End If

                            Me.InsProdotto.Value = "0"

                        Catch ex As Exception

                            Messaggi.AgroMsgBox("Errore durante la ricezione dei dati dalla pagina di gestione del magazzino: " & vbCrLf & ex.Message, Page)

                        Finally

                            Try

                                NewCom_Web_ComunicazionePagine_Cancella(objParametri_Server, Session, Page, Unid)

                            Catch ex As Exception

                            End Try

                        End Try

                        Ricava_PesoNettoTotale()

                End Select


                '//////////////////////////////////////////////////////
                '       RITORNO DAL FILTRO MOV CONTABILI
                '\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

                'ritorno dal FiltroMovimentiContabili
                Select Case Me.Input_AllegaDoc.Value

                    Case "0", "", "undefined" 'NO
                        'niente

                    Case Else 'SI


                        Dim Unid As String

                        Unid = Me.Input_AllegaDoc.Value

                        SelezionaTab(upTabs_1, 1)

                        Try

                            Dim Dt As DataTable
                            Dim Dt_Prodotti, Dt_IVA As DataTable
                            'Dim Dr_Prodotti As DataRow
                            Dim hash_iva As Hashtable
                            Dim i As Integer
                            Dim Errore, Chiave_Oggetto, Stringa_Parametri_Base, Stringa_Parametri_Rif, Data, Username As String
                            Dim Id_Riga, Tipo_Operazione, Flag_Errore As Integer

                            ViewState("UtenteAbilitatoModifica") = True

                            Dt = NewCom_Web_ComunicazionePagine_Leggi(objParametri_Server, Session, Page, Unid)

                            If Not IsNothing(Dt) Then

                                If Dt.Rows.Count <> 0 Then

                                    If IsNothing(Session("DT_Prodotti_nel_Doc")) Then
                                        CaricaGriglia_Dettagli()
                                        Dt_Prodotti = Session("DT_Prodotti_nel_Doc")
                                    Else
                                        Dt_Prodotti = Session("DT_Prodotti_nel_Doc")
                                    End If

                                    If IsNothing(Session("DT_IVA_nel_Doc")) Then
                                        hash_iva = New Hashtable
                                    Else
                                        Dt_IVA = Session("DT_IVA_nel_Doc")
                                        hash_iva = Ricava_HashIVA_da_DtIVA(Dt_IVA)
                                    End If


                                    Dim XmlDoc As New XmlDocument


                                    For i = 0 To Dt.Rows.Count - 1

                                        Unid = Dt.Rows(i).Item("Unid")
                                        Errore = Dt.Rows(i).Item("Errore")
                                        Chiave_Oggetto = Dt.Rows(i).Item("Chiave_Oggetto")
                                        Stringa_Parametri_Base = Dt.Rows(i).Item("Stringa_Parametri_Base")
                                        Stringa_Parametri_Rif = Dt.Rows(i).Item("Stringa_Parametri_Rif")
                                        Data = Dt.Rows(i).Item("Data")
                                        Username = Dt.Rows(i).Item("Username")

                                        Id_Riga = Dt.Rows(i).Item("Id_Riga")
                                        Tipo_Operazione = Dt.Rows(i).Item("Tipo_Operazione")
                                        Flag_Errore = Dt.Rows(i).Item("Flag_Errore")

                                        ''se sono in modifica
                                        'If Tipo_Operazione = enum_TipoOperazioneDB.Modifica Then

                                        '    'CANCELLO QUEL PRODOTTO

                                        '    'Trovo la riga da cancellare tramite la chiave
                                        '    Dr_Prodotti = Dt_Prodotti.Rows.Find(Chiave_Oggetto)

                                        '    'Elimino la riga
                                        '    Dr_Prodotti.Delete()

                                        '    Session("DT_Prodotti_nel_Doc") = Dt_Prodotti

                                        'End If

                                        Try

                                            'devo impostare il cau_mov perchè viene utilizzato
                                            'da XML_Leggi_MovimentoDettaglio

                                            Select Case QS_LavCod
                                                Case LAVCOD_FATTURA_EMESSA, LAVCOD_BOLLA_EMESSA, LAVCOD_NOTA_ACCREDITO_RICEVUTA
                                                    x_Cau_Mov = CAU_SCARICO

                                                Case LAVCOD_FATTURA_RICEVUTA, LAVCOD_BOLLA_RICEVUTA, LAVCOD_NOTA_ACCREDITO_EMESSA
                                                    x_Cau_Mov = CAU_CARICO
                                            End Select

                                            'leggo il dettaglio e lo inserisco in griglia
                                            XML_Leggi_MovimentoDettaglio(XmlDoc, Stringa_Parametri_Base, Stringa_Parametri_Rif, hash_iva, True, CDate(Me.Txt_DataEmissione.Text), Nothing)

                                        Catch ex As Exception

                                            Messaggi.AgroMsgBox("Errore durante la lettura dei dati ricevuti dalla pagina di filtro movimenti contabili.", Page)

                                        End Try

                                    Next 'x ogni riga scritta nella tabella di comunicazione


                                    'Associo il DataTable con la DataGrid
                                    Me.DataGrid_Prodotti.DataSource = Dt_Prodotti
                                    Me.DataGrid_Prodotti.DataBind()

                                    Call CaricaGriglia_IVA(hash_iva)

                                    Call Ricava_RiepilogoDocumento_da_DtProdotti(Dt_Prodotti)

                                    Call Visualizza_RiepilogoDocumento()

                                Else
                                    Messaggi.AgroMsgBox("Non sono arrivati i dati dalla pagina di filtro movimenti contabili.", Page)
                                End If
                            Else
                                Messaggi.AgroMsgBox("Errore durante la ricezione dei dati dalla pagina di filtro movimenti contabili.", Page)
                            End If

                            Me.Input_AllegaDoc.Value = "0"

                        Catch ex As Exception

                            Messaggi.AgroMsgBox("Errore durante la ricezione dei dati dalla pagina di filtro movimenti contabili: " & vbCrLf & ex.Message, Page)

                        Finally

                            Try

                                NewCom_Web_ComunicazionePagine_Cancella(objParametri_Server, Session, Page, Unid)

                            Catch ex As Exception

                            End Try

                        End Try


                        Ricava_PesoNettoTotale()

                End Select


                Exit Sub


            End If


            '##############################################################
            '#####################  PERMESSI  #############################
            '##############################################################

            '----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina

            Dim UtenteAbilitato As Boolean
            Dim acUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

            Select Case CInt(Qs_Operazione)

                Case enum_TipoOperazioneDB.Lettura


                    UtenteAbilitato = acUtenti.Controlla_Permessi_Utente( _
                                       Session("ASG_Utente_Username"), _
                                       Session("ASG_IdServizio"), _
                                       enum_Security_Attivita.Gest_Contabilita, _
                                       enum_Security_Operazione.Lettura, _
                                       Now, "", objParametri_Utenti _
                                       )

                    ViewState("UtenteAbilitatoModifica") = False

                Case enum_TipoOperazioneDB.Scrittura, _
                    enum_TipoOperazioneDB.Modifica, _
                    enum_TipoOperazioneDB.Cancellazione

                    UtenteAbilitato = acUtenti.Controlla_Permessi_Utente( _
                                       Session("ASG_Utente_Username"), _
                                       Session("ASG_IdServizio"), _
                                       enum_Security_Attivita.Gest_Contabilita, _
                                       enum_Security_Operazione.Modifica, _
                                       Now, "", _
                                       objParametri_Utenti _
                                      )

                    ViewState("UtenteAbilitatoModifica") = True

            End Select



            '----- Se l'utente non ha il permesso per visualizzare la pagina ... 

            If Not UtenteAbilitato Then

                ''NOTA BENE
                ''Pagina Transazionale :
                ''Per poter uscire dalla pagina bisogna chiudere la transazione

                ''Chiedo il blocco della transazione
                'System.EnterpriseServices.ContextUtil.SetAbort()

                'Assegno il flag di uscita dalla pagina
                PremutoAnnulla = True

                AAA_GestioneUscitaPagina()

                'Esco dalla routine di Page_Load
                Exit Sub

            End If


            '##############################################################
            '#####  Inizializzo i controlli  ##############################
            '##############################################################

            'Inizializzo le variabili per il controllo delle transazioni
            EseguitaOperazione = False
            PremutoAnnulla = False

            'todo
            'Me.Txt_RagioneSociale.Text = Qs_Rag_Soc

            Call CaricaCombo_TipoSconto(Server, Session, Page, Me.Cmb_TipoSconto)

            If CInt(Qs_Operazione) = enum_TipoOperazioneDB.Lettura Then
                v_Flag_AncheImportati = True
            End If


            '=========================================================
            '=========================================================

            '@Paolo:
            ' Controllo se la chiamata viene da Gias (Querystring definita)

            If Qs_Piva <> "" OrElse Qs_IdAgenda <> 0 Then
                'If Qs_Piva <> 0 And Qs_IdAgenda <> 0 Then

                Select Case CInt(Qs_Operazione)

                    Case enum_TipoOperazioneDB.Scrittura

                        If Session("AgendaScarico") Is Nothing Then
                            Imposta_Intestazione_Default()
                        Else
                            Dim AgendaScarico As Operazione_Agenda = Session("AgendaScarico")

                            Ripristina_Dati_nei_Controlli_Da_AgendaSessione(AgendaScarico)

                            Dim Doc_Numero As Decimal = 0
                            Dim Doc_Numero_Sin As String = ""
                            Dim Doc_Numero_Des As String = ""

                            RicavaNumDocumentoEmesso(objParametri_Server, Session, Page, _
                                                    Doc_Numero_Sin, _
                                                    Doc_Numero, _
                                                    Doc_Numero_Des, _
                                                    Qs_Piva, _
                                                    CDate(Me.Txt_DataEmissione.Text).Year, _
                                                    QS_LavCod, _
                                                    CAU_REGISTRAZIONI, _
                                                    0)


                            Me.Txt_NumFattura.Text = Doc_Numero + 1

                        End If

                        '=========================================================

                    Case enum_TipoOperazioneDB.Modifica

                        ImgBtn_Salva_Nuovo.Visible = False

                        Call Ripristina_Dati_nei_Controlli()

                        '=========================================================

                    Case enum_TipoOperazioneDB.Lettura

                        ImgBtn_Salva.Visible = False
                        ImgBtn_Salva_Nuovo.Visible = False

                        Call Ripristina_Dati_nei_Controlli()

                        '=========================================================

                End Select

                Call Imposta_Pannelli("T")

                Call Imposta_Toolbar()


            Else ' Chiamata proveniente da SAMRT


                ' @Paolo:
                ' Gestione proveniente da Smart
                objParametriAgenda = New ParametriAgenda_2010
                objParametriAgenda.Leggi()

                Qs_Piva = objParametriAgenda.Piva
                Qs_SaCod = objParametriAgenda.Sa_Cod
                Qs_IdAgenda = objParametriAgenda.Id_Agenda

                QS_LavCod = objParametriAgenda.Lavorazione

                CaricaComboXddt()

                Select Case objParametriAgenda.Lavorazione

                    Case LAVCOD_BOLLA_EMESSA '1031

                        Select Case CInt(objParametriAgenda.Operazione)

                            Case enum_TipoOperazioneDB.Scrittura

                                If Session("AgendaScarico") Is Nothing Then
                                    Imposta_Intestazione_Default()
                                Else
                                    Dim AgendaScarico As Operazione_Agenda = Session("AgendaScarico")

                                    Ripristina_Dati_nei_Controlli_Da_AgendaSessione(AgendaScarico)

                                    Dim Doc_Numero As Decimal = 0
                                    Dim Doc_Numero_Sin As String = ""
                                    Dim Doc_Numero_Des As String = ""

                                    RicavaNumDocumentoEmesso(objParametri_Server, Session, Page,
                                                            Doc_Numero_Sin,
                                                            Doc_Numero,
                                                            Doc_Numero_Des,
                                                            Qs_Piva,
                                                            CDate(Me.Txt_DataEmissione.Text).Year,
                                                            QS_LavCod,
                                                            CAU_REGISTRAZIONI,
                                                            0)


                                    Me.Txt_NumFattura.Text = Doc_Numero + 1

                                End If

                                '=========================================================

                            Case enum_TipoOperazioneDB.Modifica

                                ImgBtn_Salva_Nuovo.Visible = False

                                Call Ripristina_Dati_nei_Controlli()

                                '=========================================================

                            Case enum_TipoOperazioneDB.Lettura

                                ImgBtn_Salva.Visible = False
                                ImgBtn_Salva_Nuovo.Visible = False

                                Call Ripristina_Dati_nei_Controlli()

                                '=========================================================

                        End Select

                    Case LAVCOD_BOLLA_RICEVUTA '1025

                        Select Case CInt(objParametriAgenda.Operazione)

                            Case enum_TipoOperazioneDB.Scrittura

                                If Session("AgendaScarico") Is Nothing Then
                                    Imposta_Intestazione_Default()
                                Else
                                    Dim AgendaScarico As Operazione_Agenda = Session("AgendaScarico")

                                    Ripristina_Dati_nei_Controlli_Da_AgendaSessione(AgendaScarico)

                                    Dim Doc_Numero As Decimal = 0
                                    Dim Doc_Numero_Sin As String = ""
                                    Dim Doc_Numero_Des As String = ""

                                    RicavaNumDocumentoEmesso(objParametri_Server, Session, Page,
                                                            Doc_Numero_Sin,
                                                            Doc_Numero,
                                                            Doc_Numero_Des,
                                                            Qs_Piva,
                                                            CDate(Me.Txt_DataEmissione.Text).Year,
                                                            QS_LavCod,
                                                            CAU_REGISTRAZIONI,
                                                            0)


                                    Me.Txt_NumFattura.Text = Doc_Numero + 1

                                End If

                                '=========================================================

                            Case enum_TipoOperazioneDB.Modifica

                                ImgBtn_Salva_Nuovo.Visible = False

                                Call Ripristina_Dati_nei_Controlli()

                                '=========================================================

                            Case enum_TipoOperazioneDB.Lettura

                                ImgBtn_Salva.Visible = False
                                ImgBtn_Salva_Nuovo.Visible = False

                                Call Ripristina_Dati_nei_Controlli()

                                '=========================================================

                        End Select

                    Case LAVCOD_FATTURA_EMESSA '1001

                        Select Case CInt(objParametriAgenda.Operazione)

                            Case enum_TipoOperazioneDB.Scrittura

                                If Session("AgendaScarico") Is Nothing Then
                                    Imposta_Intestazione_Default()
                                Else
                                    Dim AgendaScarico As Operazione_Agenda = Session("AgendaScarico")

                                    Ripristina_Dati_nei_Controlli_Da_AgendaSessione(AgendaScarico)

                                    Dim Doc_Numero As Decimal = 0
                                    Dim Doc_Numero_Sin As String = ""
                                    Dim Doc_Numero_Des As String = ""

                                    RicavaNumDocumentoEmesso(objParametri_Server, Session, Page,
                                                            Doc_Numero_Sin,
                                                            Doc_Numero,
                                                            Doc_Numero_Des,
                                                            Qs_Piva,
                                                            CDate(Me.Txt_DataEmissione.Text).Year,
                                                            QS_LavCod,
                                                            CAU_REGISTRAZIONI,
                                                            0)


                                    Me.Txt_NumFattura.Text = Doc_Numero + 1

                                End If

                                '=========================================================

                            Case enum_TipoOperazioneDB.Modifica

                                ImgBtn_Salva_Nuovo.Visible = False

                                Call Ripristina_Dati_nei_Controlli()

                                '=========================================================

                            Case enum_TipoOperazioneDB.Lettura

                                ImgBtn_Salva.Visible = False
                                ImgBtn_Salva_Nuovo.Visible = False

                                Call Ripristina_Dati_nei_Controlli()

                                '=========================================================

                        End Select

                    Case LAVCOD_FATTURA_RICEVUTA '1000

                        Select Case CInt(objParametriAgenda.Operazione)

                            Case enum_TipoOperazioneDB.Scrittura

                                If Session("AgendaScarico") Is Nothing Then
                                    Imposta_Intestazione_Default()
                                Else
                                    Dim AgendaScarico As Operazione_Agenda = Session("AgendaScarico")

                                    Ripristina_Dati_nei_Controlli_Da_AgendaSessione(AgendaScarico)

                                    Dim Doc_Numero As Decimal = 0
                                    Dim Doc_Numero_Sin As String = ""
                                    Dim Doc_Numero_Des As String = ""

                                    RicavaNumDocumentoEmesso(objParametri_Server, Session, Page,
                                                            Doc_Numero_Sin,
                                                            Doc_Numero,
                                                            Doc_Numero_Des,
                                                            Qs_Piva,
                                                            CDate(Me.Txt_DataEmissione.Text).Year,
                                                            QS_LavCod,
                                                            CAU_REGISTRAZIONI,
                                                            0)


                                    Me.Txt_NumFattura.Text = Doc_Numero + 1

                                End If

                                '=========================================================

                            Case enum_TipoOperazioneDB.Modifica

                                ImgBtn_Salva_Nuovo.Visible = False

                                Call Ripristina_Dati_nei_Controlli()

                                '=========================================================

                            Case enum_TipoOperazioneDB.Lettura

                                ImgBtn_Salva.Visible = False
                                ImgBtn_Salva_Nuovo.Visible = False

                                Call Ripristina_Dati_nei_Controlli()

                                '=========================================================

                        End Select

                End Select

                Call Imposta_Pannelli("T")

                Call Imposta_Toolbar()

            End If


            '04/03/2019: modifica per documento light
            'in modifica aprire la tab dettagli
            Dim objconfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim DocumentoRicevutoLight As Boolean
            Dim DocumentoRicevutoLight_str As String = objconfigSiti.Leggi_Valore(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, "DocumentoRicevutoLight", "", "", HttpContext.Current.Session("ASG_objParametri_Server"))

            If DocumentoRicevutoLight_str = "" Then
                DocumentoRicevutoLight = False
            Else
                DocumentoRicevutoLight = DocumentoRicevutoLight_str
            End If

            If DocumentoRicevutoLight AndAlso (QS_LavCod = LAVCOD_FATTURA_RICEVUTA OrElse QS_LavCod = LAVCOD_BOLLA_RICEVUTA) Then

                Dim strJS As New StringBuilder
                'If upPanel Is Nothing Then
                'strJS.AppendLine("<script> ")
                'End If
                strJS.AppendLine("$(document).ready(function () { ")
                strJS.AppendLine("  $(""#tabDettagli"").click(); ")
                strJS.AppendLine(" });")
                'If upPanel Is Nothing Then
                'strJS.AppendLine("</script> ")
                'End If
                ScriptManager.RegisterClientScriptBlock(upTabs_1, upTabs_1.GetType(),
                                     String.Format("jQuery_{0}", "tab2click"), strJS.ToString, True)

            End If

        Catch exc As Exception

            '------------------------------------------------
            'Si e' verificata una eccezione !!!!!!
            '------------------------------------------------

            Disattiva_Pulsanti()

            'Messaggio di errore
            Messaggio = "Si e' verificato un errore : " &
                        Chr(13) &
                        exc.Message.ToString()

            'Visualizzo il messaggio di errore
            Call Messaggi.AgroMsgBox(Messaggio, Page)

            '------------------------------------------------

        End Try


    End Sub


    Private Sub ImpostazioniDocumentoDatoLAV_COD()

        'Pannello_Mezzo.Enabled = False 'todo, verifica!

        If QS_LavCod = LAVCOD_BOLLA_EMESSA OrElse
            QS_LavCod = LAVCOD_BOLLA_RICEVUTA Then


            Me.DataGrid_Prodotti.Columns.Item(COL_Variazione).Visible = False
            Me.DataGrid_Prodotti.Columns.Item(COL_Variazione_Perc).Visible = False
            Me.DataGrid_Prodotti.Columns.Item(COL_Imponibile_Netto).Visible = False
            Me.DataGrid_Prodotti.Columns.Item(COL_Aliquota).Visible = False
            Me.DataGrid_Prodotti.Columns.Item(COL_IVA).Visible = False
            Me.DataGrid_Prodotti.Columns.Item(COL_Anno).Visible = False
            Me.DataGrid_Prodotti.Columns.Item(COL_Conto).Visible = False
            Me.DataGrid_Prodotti.Columns.Item(COL_ALLEGATO).Visible = False

            lbl_DataScadenza.Visible = False
            Txt_DataScadenza.Visible = False

        End If


        Dim testo As String = ""


        Select Case QS_LavCod
            Case LAVCOD_FATTURA_EMESSA, LAVCOD_BOLLA_EMESSA, LAVCOD_NOTA_ACCREDITO_EMESSA
                testo = "Emissione "
                Lbl_tabC1.InnerText = "Cessionario"
                Lbl_tabC2.InnerText = "Cessionario Diverso"
                Lbl_Contatto.Text = "Cessionario:"
                Lbl_DestinazioneDiversa.Text = "Cessionario Diverso:"
            Case LAVCOD_FATTURA_RICEVUTA, LAVCOD_BOLLA_RICEVUTA, LAVCOD_NOTA_ACCREDITO_RICEVUTA
                testo = "Ricevimento "
                Lbl_tabC1.InnerText = "Cedente"
                Lbl_tabC2.InnerText = "Cedente Diverso"
                Lbl_Contatto.Text = "Cedente:"
                Lbl_DestinazioneDiversa.Text = "Cedente Diverso:"
        End Select

        Select Case QS_LavCod

            Case LAVCOD_FATTURA_EMESSA
                testo = "Emissione Fattura"
                Lbl_tabC1.InnerText = "Cessionario"
                Me.Lbl_Contatto.Text = "Cessionario:"

            Case LAVCOD_FATTURA_RICEVUTA
                testo = "Ricevimento Fattura"
                Lbl_tabC1.InnerText = "Cedente"
                Me.Lbl_Contatto.Text = "Cedente:"

            Case LAVCOD_BOLLA_EMESSA
                testo = "Emissione "
                Lbl_tabC1.InnerText = "Cessionario"
                Me.Lbl_Contatto.Text = "Cessionario:"

            Case LAVCOD_BOLLA_RICEVUTA
                testo = "Ricevimento "
                Lbl_tabC1.InnerText = "Cedente"
                Me.Lbl_Contatto.Text = "Cedente:"

            Case LAVCOD_NOTA_ACCREDITO_RICEVUTA
                testo = "Ricevimento "
                Lbl_tabC1.InnerText = "Cedente"
                Me.Lbl_Contatto.Text = "Cedente:"

            Case LAVCOD_NOTA_ACCREDITO_EMESSA
                testo = "Emissione "
                Lbl_tabC1.InnerText = "Cessionario"
                Me.Lbl_Contatto.Text = "Cessionario:"

        End Select


        Select Case QS_LavCod

            Case LAVCOD_FATTURA_EMESSA, LAVCOD_FATTURA_RICEVUTA, LAVCOD_NOTA_ACCREDITO_EMESSA, LAVCOD_NOTA_ACCREDITO_RICEVUTA

                'ID_Zootecnico.Visible = False
                panDDT.Visible = False
                Pannello_IVA.Visible = True
                Pannello_Riepilogo.Visible = True

                Select Case QS_LavCod

                    Case LAVCOD_FATTURA_EMESSA, LAVCOD_FATTURA_RICEVUTA

                        Select Case Me.Rbl_TipoFattura.SelectedValue
                            Case 0
                                testo &= " Fattura Differita"
                                ID_Allegati.Visible = True
                                panDDT.Visible = False
                            Case 1
                                testo &= " Fattura Immediata"
                                ID_Allegati.Visible = False
                                panDDT.Visible = True
                        End Select

                    Case LAVCOD_NOTA_ACCREDITO_EMESSA, LAVCOD_NOTA_ACCREDITO_RICEVUTA
                        testo &= " Nota Accredito"
                        ID_Allegati.Visible = False
                        panDDT.Visible = True
                        Rbl_TipoFattura.Visible = False

                End Select

            Case Else

                panDDT.Visible = True
                Pannello_IVA.Visible = False
                Pannello_Riepilogo.Visible = False
                ID_Allegati.Visible = False
                Rbl_TipoFattura.Visible = False


                testo &= "Documento di Trasporto - D.P.R. 472 del 14/08/96"

        End Select

        CType(Page.Master.FindControl("Lbl_Titolo"), Label).Text = testo


    End Sub





    '########################################################################################
    'La pagina, non essendo più transazionale, non ha nè il Page Commit, nè il Page Abort
    'Utilizza AAA_GestioneUscitaPagina per la gestione di uscita dalla pagina 
    '(in caso di salvataggio o di exit)
    Private Sub AAA_GestioneUscitaPagina()

        'Verifico che l'uscita sia voluta ....
        If PremutoAnnulla OrElse EseguitaOperazione Then

            Session("DT_Prodotti_nel_Doc") = Nothing
            Session("DT_IVA_nel_Doc") = Nothing
            Session("AgendaScarico") = Nothing
            Session("ListaOpAgenda") = Nothing

            'controllo se provengo dal gias smart

            Dim link As String = ""
            Try
                Dim objParametriAgenda2 As New ParametriAgenda

                Dim sitoorigine As Enum_SiteRedirector = objParametriAgenda2.SitoOrigine
                Dim paginaOnLineRitorno As Integer = CInt(Qs_PagRitorno)

                If sitoorigine = Enum_SiteRedirector.Sito_GiasOnline_2010 AndAlso paginaOnLineRitorno = enum_PagineGiasOnline_2010.RegistazioneSmart Then
                    link = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline2010_PassandoDirettamenteIParametri(
                                           Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                           enum_PagineGiasOnline_2010.RegistazioneSmart,
                                           enum_PagineAgenda_2010.Menu, Qs_Piva, "", "", 0, "")

                    Response.Redirect(link)
                ElseIf sitoorigine = Enum_SiteRedirector.GiasNG Then

                    AgronicaCoreGestioneRichieste.MenuBS_2017_RedirectGestione.RedirectGenerico(objParametriAgenda2.Piva,
                                                                            Enum_SiteRedirector.GiasNG,
                                                                           objParametriAgenda2.PaginaSitoOrigine,
                                                                            link,
                                                                            objParametri_Server)

                    Response.Redirect(link)
                End If

            Catch ex As Exception

            End Try

            Select Case Qs_PagRitorno


                Case enum_PagineAgenda_2010.Menu

                    'Response.Redirect("../Menu/Menu.aspx")

                    Dim Path_Menu As String = "../Menu/Menu.aspx"
                    Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                    Dim DTConfigSiti As DataTable = objConfigSiti.Leggi(0, "MenuAgendaBS", "", "", objParametri_Server)

                    If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 AndAlso LCase(DTConfigSiti.Rows(0).Item("Valore")) = "true" Then
                        Path_Menu = "../Menu/MenuBS_Agenda_Nuovo.aspx"
                    End If

                    Response.Redirect(Path_Menu)

                Case enum_PagineAgenda_2010.Menu_BS


                    Response.Redirect("../Menu/MenuBS_Agenda_Nuovo.aspx")


                Case enum_PagineAgenda_2010.Pagina_GestioneMagazzini


                    Response.Redirect("../GestioneMagazzini/GestioneMagazzini.aspx")

                Case enum_PagineAgenda_2010.Pagina_GestioneMagazziniBS


                    Response.Redirect("../GestioneMagazzini/GestioneMagazziniBS.aspx")
                Case Else

                    Dim TargetURL As String
                    Dim PaginaRitorno As String

                    PaginaRitorno = PaginaAspx_from_TipoEnumPagina(CInt(Qs_PagRitorno), "../../")

                    'Costruisco il link
                    TargetURL = PaginaRitorno &
                                "?p=" &
                                Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) &
                                "&s=" &
                                Stringa_Codifica(Qs_SaCod, AgroKey_EncoderDecoder, Server) &
                                "&d=" &
                                Stringa_Codifica(Qs_DataSelezionata, AgroKey_EncoderDecoder, Server)

                    'Vado alla pagina
                    Response.Redirect(TargetURL)


            End Select

        End If

    End Sub





    '####################################################################################
    Private Sub Disattiva_Pulsanti()

        ImgBtn_Salva.Visible = False
        ImgBtn_Salva_Nuovo.Visible = False

        Me.PulsantieraDettagli.Enabled = False
        Me.PulsantieraDettagli.Visible = False

    End Sub


    '########################################################################################
    Private Sub Imposta_Intestazione_Default()

        'todo
        'Me.Txt_RagioneSociale.Text = Qs_Rag_Soc



        Me.Txt_DataEmissione.Text = CDate(Qs_DataSelezionata).ToShortDateString
        Me.Txt_DataRegistrazione.Text = CDate(Qs_DataSelezionata).ToShortDateString
        Me.TXT_DataConsegna.Text = CDate(Qs_DataSelezionata).ToShortDateString
        Me.txt_Ora.Text = "12.00"

        If CDate(Qs_DataSelezionata).Month = 12 Then
            Me.Txt_DataScadenza.Text = Right("00" + CStr(CDate(Qs_DataSelezionata).Day), 2) & "/01/" & CStr(CDate(Qs_DataSelezionata).Year + 1)
        Else
            If CDate(Qs_DataSelezionata).Day < 29 Then
                'non ci sono problemi di data
                Me.Txt_DataScadenza.Text = Right("00" + CStr(CDate(Qs_DataSelezionata).Day), 2) & "/" & Right("00" + CStr(CDate(Qs_DataSelezionata).Month + 1), 2) & "/" & CStr(CDate(Qs_DataSelezionata).Year)
            Else
                'possono esserci problemi, verifico in base al mese
                Select Case CDate(Qs_DataSelezionata).Month + 1
                    Case 11, 4, 6, 9
                        Me.Txt_DataScadenza.Text = "30/" & Right("00" + CStr(CDate(Qs_DataSelezionata).Month + 1), 2) & "/" & CStr(CDate(Qs_DataSelezionata).Year)
                    Case 2
                        Me.Txt_DataScadenza.Text = "28/" & Right("00" + CStr(CDate(Qs_DataSelezionata).Month + 1), 2) & "/" & CStr(CDate(Qs_DataSelezionata).Year)
                    Case Else
                        Me.Txt_DataScadenza.Text = Right("00" + CStr(CDate(Qs_DataSelezionata).Day), 2) & "/" & Right("00" + CStr(CDate(Qs_DataSelezionata).Month + 1), 2) & "/" & CStr(CDate(Qs_DataSelezionata).Year)
                End Select
            End If
        End If

        'Me.Txt_Ora.Text = CDate(Qs_DataSelezionata).ToShortTimeString
        'If Me.Txt_Ora.Text = "0.00" Then
        '    Me.Txt_Ora.Text = "12.00"
        'End If

        Me.Txt_PesoNetto.Text = "0"

        Me.Cmb_TipoIndirizzo_Contatto.Enabled = False

        ''SE SONO IN SCRITTURA DEVONO ESSERE DISABILITATI
        ''LI ABILITO SOLO QUANDO ATTIVO IL CHECK
        'Me.Cmb_TipoIndirizzo_DestDiverso.Enabled = False
        'Me.Cmb_DestinatarioDiverso.Enabled = False
        'Me.Txt_Cerca_Diverso.Enabled = False
        'Me.Img_btn_CercaDiverso.Enabled = False
        'Me.Txt_Cerca_Diverso2.Enabled = False
        'Me.Img_btn_CercaDiverso2.Enabled = False

        'il trasportatore di default è il mittente
        Me.Rbl_Trasporto.SelectedIndex = 0

        'il trasportatore non è il vettore, allora lo disabilito
        Me.Cmb_Vettore.Enabled = False
        Me.Cmb_TipoIndirizzo_Vettore.Enabled = False


        '--------------------------------------------------
        'SE STO EMETTENDO UNA FATTURA IMPOSTO DI DEFAULT IL NUMERO
        'Lettura nuovo numero documento contabile

        Select Case QS_LavCod

            Case LAVCOD_FATTURA_EMESSA, LAVCOD_NOTA_ACCREDITO_EMESSA,
                LAVCOD_BOLLA_EMESSA

                Dim Doc_Numero As Decimal = 0
                Dim Doc_Numero_Sin As String = ""
                Dim Doc_Numero_Des As String = ""

                RicavaNumDocumentoEmesso(objParametri_Server, Session, Page,
                                        Doc_Numero_Sin,
                                        Doc_Numero,
                                        Doc_Numero_Des,
                                        Qs_Piva,
                                        CDate(Me.Txt_DataEmissione.Text).Year,
                                        QS_LavCod,
                                        CAU_REGISTRAZIONI,
                                        0)


                Me.Txt_NumFattura.Text = Doc_Numero + 1

                Me.Txt_NumFattura.ToolTip = "Numero fattura corretto per impostazione automatica: " & Me.Txt_NumFattura.Text


            Case LAVCOD_FATTURA_RICEVUTA

                'Me.LblTitolo.Text = "Ricevimento Fattura"

        End Select

        '--------------------------------------------------

        'PROGRESSIVO PROTOCOLLO

        'chiedere se è univoco sia per ricevute che per emesse, oppure no




    End Sub


    '########################################################################################
    Private Sub Imposta_Toolbar()

        Select Case CInt(Qs_Operazione)

            Case enum_TipoOperazioneDB.Lettura

                ID_Prodotto.Enabled = False
                'ID_Zootecnico.Enabled = False
                ID_Allegati.Enabled = False
                ID_Cancella.Enabled = False
                'ID_Stampa.Enabled = True

            Case enum_TipoOperazioneDB.Scrittura,
                enum_TipoOperazioneDB.Modifica

                ID_Prodotto.Enabled = True
                'ID_Zootecnico.Enabled = True
                ID_Allegati.Enabled = True

                'If Qs_TipoFattura = 1 Then
                '    'immediata
                '    Me.ToolbarOperazioni.Items.Item(T_ALLEGATO).Enabled = False
                'Else
                '    'differita
                '    Me.ToolbarOperazioni.Items.Item(T_ALLEGATO).Enabled = True
                'End If

                ID_Cancella.Enabled = True
                'ID_Stampa.Enabled = False


        End Select

    End Sub

    '########################################################################################
    Private Sub Imposta_Pannelli(ByVal InizialiPannelliAttivi As String)

        '----- Imposto le dimensioni



        '----- Attivo i pannelli richiesti

        'Testata
        If InStr(InizialiPannelliAttivi, "T") <> 0 Then


            Select Case QS_LavCod

                Case LAVCOD_FATTURA_RICEVUTA

                    Select Case Me.Rbl_TipoFattura.SelectedValue
                        Case 1
                            'todo
                            'Me.LblTitolo.Text = "Ricevimento Fattura Immediata"
                            'Me.Pannello_Vettore.Visible = True 'todo
                            Me.DataGrid_Prodotti.Columns.Item(COL_ALLEGATO).Visible = False
                        Case 0
                            'todo
                            'Me.LblTitolo.Text = "Ricevimento Fattura Differita"
                            'Me.Pannello_Vettore.Visible = False 'todo
                            Me.DataGrid_Prodotti.Columns.Item(COL_ALLEGATO).Visible = True
                    End Select

                    Me.Lbl_Contatto.Text = "Fornitore:"

                Case LAVCOD_FATTURA_EMESSA

                    Select Case Me.Rbl_TipoFattura.SelectedValue
                        Case 1
                            'todo
                            'Me.LblTitolo.Text = "Emissione Fattura Immediata"
                            'Me.Pannello_Vettore.Visible = True 'todo
                            Me.DataGrid_Prodotti.Columns.Item(COL_ALLEGATO).Visible = False
                        Case 0
                            'todo
                            'Me.LblTitolo.Text = "Emissione Fattura Differita"
                            'Me.Pannello_Vettore.Visible = False 'todo
                            Me.DataGrid_Prodotti.Columns.Item(COL_ALLEGATO).Visible = True
                    End Select

                    Me.Lbl_Contatto.Text = "Cliente:"

                    'Case LAVCOD_BOLLA_EMESSA
                    '    Me.Lbl_Contatto.Text = "Destinatario:"

                    'Case LAVCOD_BOLLA_RICEVUTA
                    '    Me.Lbl_Contatto.Text = "Cedente:"

            End Select

        Else

            'Me.Pannello_Testata.Visible = False
            'Me.Tab_Testata.BorderColor = Color.FromArgb(Val("&H00"), Val("&H00"), Val("&HC0"))
            'Me.Tab_Testata.BackColor = Color.FromArgb(Val("&HC0"), Val("&HFF"), Val("&HFF"))
            'Me.Lbl_Testata.CssClass = "Testo_08_Blue_Bold"
            'Me.Lbl_Testata.BackColor = Color.FromArgb(Val("&HC0"), Val("&HFF"), Val("&HFF"))
            'Me.ImgBtn_Testata.BackColor = Color.FromArgb(Val("&HC0"), Val("&HFF"), Val("&HFF"))

        End If

        '=============================================================================

        'Dettagli

        '=============================================================================

        'Allegati

        '=============================================================================

        'Pagamenti

        '=============================================================================

    End Sub

    '######################################################################################################################################
    Private Sub Verifica_Controlli_ClienteFornitore(ByRef Messaggio As String)

        'MITTENTE / DESTINATARIO

        'verifica tipo operazione
        If Qs_Operazione = enum_TipoOperazioneDB.Scrittura Then

            'scrittura

            If Me.Cmb_Contatti.SelectedValue = "" Then

                Messaggio &= "Prima di inserire i dettagli è necessario completare i dati della testata del documento: " & vbCrLf & vbCrLf

                If (QS_LavCod = CStr(LAVCOD_FATTURA_RICEVUTA) OrElse QS_LavCod = CStr(LAVCOD_BOLLA_RICEVUTA)) Then 'ricevimento
                    Messaggio &= "- selezionare il fornitore." & vbCrLf
                ElseIf (QS_LavCod = CStr(LAVCOD_FATTURA_EMESSA) OrElse QS_LavCod = CStr(LAVCOD_bolla_EMESSA)) Then 'emissione
                    Messaggio &= "- selezionare il cliente." & vbCrLf
                ElseIf QS_LavCod = CStr(LAVCOD_NOTA_ACCREDITO_EMESSA) Then 'emissione
                    Messaggio &= "- selezionare il cliente." & vbCrLf
                ElseIf QS_LavCod = CStr(LAVCOD_NOTA_ACCREDITO_RICEVUTA) Then 'emissione
                    Messaggio &= "- selezionare il fornitore." & vbCrLf
                End If

            End If

        Else

            'info e modifica

            If Me.Cmb_Contatti.SelectedValue = "" Then
                Messaggio &= "Errore nella visualizzazione della testata del documento: il contatto non è selezionato." & vbCrLf & vbCrLf
            End If

        End If 'operazione

    End Sub

    '######################################################################################################################################
    Private Sub Verifica_Controlli_Testata(ByRef Messaggio As String)

        '------------------------------------------------
        '----- TESTATA
        '------------------------------------------------


        'NUMERO
        If Me.Txt_NumFattura.Text = "" Then
            Messaggio &= " E' necessario indicare il numero del documento." & vbCrLf
        Else

            If Not IsNumeric(Me.Txt_NumFattura.Text) Then
                Messaggio &= " E' necessario indicare nella parte numerica del numero del documento un valore numerico non nullo." & vbCrLf
            Else

                Dim cod_Risum As Integer = Cmb_Contatti.SelectedValue.Split("|")(0) '0
                'If QS_LavCod = LAVCOD_FATTURA_RICEVUTA OrElse
                '    QS_LavCod = LAVCOD_BOLLA_RICEVUTA OrElse
                '    QS_LavCod = LAVCOD_NOTA_ACCREDITO_RICEVUTA Then
                '    cod_Risum = Cmb_Contatti.SelectedValue.Split("|")(0)
                'End If

                Dim Str_NumFattura As String = ""
                Str_NumFattura = Me.Txt_NumFattura_Sin.Text & Me.Txt_NumFattura.Text & Me.Txt_NumFattura_Des.Text

                If (Not (CInt(Qs_Operazione) = enum_TipoOperazioneDB.Modifica AndAlso
                    Str_NumFattura = CStr(Session("Doc_Numero")))) AndAlso
                        (Not (CInt(Qs_Operazione) = enum_TipoOperazioneDB.Lettura AndAlso
                            Str_NumFattura = CStr(Session("Doc_Numero")))) Then

                    If VerificaNumDocumento(objParametri_Server, Session, Page,
                                                 Qs_Piva,
                                                 CStr(CDate(Me.Txt_DataEmissione.Text).Year),
                                                 QS_LavCod,
                                                 cod_Risum,
                                                 Me.Txt_NumFattura_Sin.Text,
                                                 CDbl(Me.Txt_NumFattura.Text),
                                                 Me.Txt_NumFattura_Des.Text,
                                                 CAU_REGISTRAZIONI) = True _
                    Then
                        Messaggio &= "Il numero del documento specificato è già stato registrato nell'anno in corso." & vbCrLf &
                                    "Se occorre inserire ulteriori articoli nel documento numero " & Me.Txt_NumFattura.Text & ", basta entrare in modifica di tale documento e aggiungere gli articoli mancanti."
                    End If


                End If

            End If

        End If


        'DATA EMISSIONE
        If Me.Txt_DataEmissione.Text = "" Then
            Messaggio &= " E' necessario indicare la data di emissione del documento." & vbCrLf
        Else
            If Not IsDate(Me.Txt_DataEmissione.Text) Then
                Messaggio &= " La data di emissione del documento deve essere una data." & vbCrLf
            End If
        End If

        ''DATA REGISTRAZIONE
        'If Me.Txt_DataRegistrazione.Text = "" Then
        '    Messaggio &= " E' necessario indicare la data di registrazione della fattura." & vbCrLf
        'End If

        'SCADENZA

        If QS_LavCod = LAVCOD_FATTURA_EMESSA OrElse
            QS_LavCod = LAVCOD_FATTURA_RICEVUTA OrElse
            QS_LavCod = LAVCOD_NOTA_ACCREDITO_RICEVUTA OrElse
            QS_LavCod = LAVCOD_NOTA_ACCREDITO_EMESSA Then
            If Me.Txt_DataScadenza.Text = "" Then
                Messaggio &= " E' necessario indicare la data di scadenza della fattura." & vbCrLf
            Else
                If Not IsDate(Me.Txt_DataScadenza.Text) Then
                    Messaggio &= " La data di scadenza deve essere una data." & vbCrLf
                End If
            End If
        Else
            If TXT_DataConsegna.Text = "" Then
                TXT_DataConsegna.Text = Txt_DataEmissione.Text
                'Messaggio &= " E' necessario indicare la data di consegna del DDT." & vbCrLf
            Else
                If Not IsDate(Me.TXT_DataConsegna.Text) Then
                    Messaggio &= " La data di consegna deve essere una data." & vbCrLf
                End If
            End If
        End If




        'PESO NETTO
        If Me.Txt_PesoNetto.Text = "" Then
            Messaggio &= " E' necessario indicare il peso netto." & vbCrLf
        Else
            If Not IsNumeric(Me.Txt_PesoNetto.Text) Then
                Messaggio &= " E' necessario indicare il peso netto con un valore numerico." & vbCrLf
            Else
                If InStr(Me.Txt_PesoNetto.Text, ".") <> 0 Then
                    Me.Txt_PesoNetto.Text = Replace(Me.Txt_PesoNetto.Text, ".", ",")
                End If
            End If
        End If


        Verifica_Controlli_ClienteFornitore(Messaggio)


        'INDIRIZZO MITTENTE / DESTINATARIO
        If Me.Txt_CodIndirizzo_Contatto.Text = "" Then

            If QS_LavCod = CStr(LAVCOD_FATTURA_RICEVUTA) OrElse QS_LavCod = CStr(LAVCOD_NOTA_ACCREDITO_RICEVUTA) Then 'ricevimento
                Messaggio &= " E' necessario selezionare l'indirizzo del fornitore." & vbCrLf
            ElseIf QS_LavCod = CStr(LAVCOD_FATTURA_EMESSA) OrElse QS_LavCod = CStr(LAVCOD_NOTA_ACCREDITO_EMESSA) Then 'emissione
                Messaggio &= " E' necessario selezionare l'indirizzo del cliente." & vbCrLf
            End If

        End If

        ''DESTINATARIO DIVERSO
        'If Me.Chk_DestinatarioDiverso.Checked Then

        '    If Me.Txt_CodContatto_DestDiverso.Text = "" Then
        '        Messaggio &= " E' necessario selezionare il destinatario diverso della fattura." & vbCrLf
        '    End If

        '    If Me.Txt_CodIndirizzo_DestDiverso.Text = "" Then
        '        Messaggio &= " E' necessario selezionare l'indirizzo del destinatario diverso della fattura." & vbCrLf
        '    End If

        'End If

        'VETTORE
        Select Case Me.Rbl_Trasporto.SelectedIndex

            Case 2 'vettore

                If Me.Txt_CodContatto_Vettore.Text = "" Then
                    Messaggio &= " E' necessario selezionare il vettore che si occupa del trasporto." & vbCrLf
                End If

                If Me.Txt_CodIndirizzo_Vettore.Text = "" Then
                    Messaggio &= " E' necessario selezionare l'indirizzo del vettore che si occupa del trasporto." & vbCrLf
                End If

        End Select


        ''PESO TARA
        If Me.Txt_PesoTara.Text <> "" Then
            If Not IsNumeric(Me.Txt_PesoTara.Text) Then
                Messaggio &= " E' necessario indicare il peso del mezzo con un valore numerico." & vbCrLf
            Else
                If InStr(Me.Txt_PesoTara.Text, ".") <> 0 Then
                    Me.Txt_PesoTara.Text = Replace(Me.Txt_PesoTara.Text, ".", ",")
                End If
            End If
        End If


    End Sub


    '######################################################################################################################################
    Private Sub Verifica_Controlli_Dettagli(ByRef Messaggio As String)

        '------------------------------------------------
        '----- DETTAGLI
        '------------------------------------------------

        'è necessario indicare almeno un prodotto
        If Me.DataGrid_Prodotti.Items.Count = 0 Then

            Messaggio &= " E' necessario specificare gli articoli della fattura nella sezione dei Dettagli." & vbCrLf
            Exit Sub

        End If

        If QS_LavCod = LAVCOD_FATTURA_RICEVUTA OrElse QS_LavCod = LAVCOD_FATTURA_EMESSA Then

            If Not IsNothing(Session("UTENTE_COD_BLOCCA_SE_DATA_FATTURA_SUPERA_ALLEGATI")) AndAlso Session("UTENTE_COD_BLOCCA_SE_DATA_FATTURA_SUPERA_ALLEGATI") Then
                Dim dt As DataTable = Session("DT_Prodotti_nel_Doc")
                For i = 0 To dt.Rows.Count - 1

                    If Not IsDBNull(dt.Rows(i).Item("ValiditaInizio")) AndAlso
                        IsDate(dt.Rows(i).Item("ValiditaInizio")) AndAlso
                        Not IsNothing(Txt_DataEmissione.Text) AndAlso
                        IsDate(Txt_DataEmissione.Text) Then

                        If CDate(dt.Rows(i).Item("ValiditaInizio")) > CDate(Txt_DataEmissione.Text) Then
                            Messaggio = " La data dell'operazione è precedente a quella del dettaglio " & i + 1 & ", in base alle impostazioni utenti il salvataggio non è consentito ." & vbCrLf
                            Exit Sub
                        End If

                    End If
                Next
            End If

        End If



    End Sub



    '##############################################################################################################
    Private Sub ImgBtn_Dettagli_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Dettagli.Click
        Tab_Dettagli()
    End Sub

    Protected Sub BTN_ChangeData_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BTN_ChangeData.Click

        If IsDate(Txt_DataEmissione.Text) Then
            Dim objParametriAgenda = New ParametriAgenda_2010
            Dim objParametriAgenda2 = New ParametriAgenda

            Dim dataMin As Date = AGRODATAINIZIO
            Dim dataMax As Date = AGRODATAFINE
            Dim SportelloAperto As Boolean = True

            Dim piva As String = If(IsNothing(objParametriAgenda.Piva), objParametriAgenda2.Piva, objParametriAgenda.Piva)
            If IsNothing(piva) Then
                piva = Qs_Piva
            End If

            Dim objPratiche As New AgronicaCoreProfilazioneBIZ.Pratiche_R
            objPratiche.Data_Sportello_Da_Servizio(piva,
                                                   Qs_Servizio_Cod,
                                                   DateTime.Now,
                                                   SportelloAperto,
                                                   dataMin,
                                                   dataMax,
                                                   objParametri_Server,
                                                   objParametri_Utenti)

            If Txt_DataEmissione.Text < dataMin Then
                Txt_DataEmissione.Text = dataMin.ToShortDateString
                Txt_DataRegistrazione.Text = dataMin.ToShortDateString
                Messaggi.AgroMsgBox("Non è possibile inserire un'operazione prima del " & dataMin.ToShortDateString & ". Sportello chiuso", Page, NomeForm:="aspnetForm", UpdatePanel:=upTabs_2)
            End If

            If Txt_DataEmissione.Text > dataMax Then
                Txt_DataEmissione.Text = dataMax.ToShortDateString
                Txt_DataRegistrazione.Text = dataMax.ToShortDateString
                Messaggi.AgroMsgBox("Non è possibile inserire un'operazione dopo il " & dataMax.ToShortDateString & ". Sportello chiuso", Page, NomeForm:="aspnetForm", UpdatePanel:=upTabs_2)
            End If

        End If

    End Sub


    Private Sub Tab_Dettagli()

        Dim Messaggio As String = ""

        Verifica_Controlli_ClienteFornitore(Messaggio)

        If Messaggio <> "" Then

            Messaggi.AgroMsgBox(Messaggio, Page, NomeForm:="aspnetForm", UpdatePanel:=upTabs_2)

        Else

            Imposta_Pannelli("D")

        End If

    End Sub


    '##############################################################################################################
    Private Sub ImgBtn_Testata_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Testata.Click

        If Me.Cmb_Contatti.SelectedValue <> "" Then

            If Me.DataGrid_Prodotti.Items.Count > 0 AndAlso
                VerificaEsistenza_CodContatto_as_PivaGIAS(objParametri_Server, Session, Page, Me.Cmb_Contatti.SelectedValue.Split("|")(0)) = True Then

                Me.Cmb_Contatti.Enabled = False

            End If

            Imposta_Pannelli("T")

        Else
            Messaggi.AgroMsgBox("Errore nella visualizzazione della testata del documento: il contatto non è selezionato." & vbCrLf, Page, NomeForm:="aspnetForm", UpdatePanel:=upTabs_1)
        End If

    End Sub


    ''##############################################################################################################
    'Private Sub ImgBtn_Allegati_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Allegati.Click

    '    'Imposta_Pannelli("A")

    '    Dim QueryString As String
    '    Dim lragioneSociale As String = ""
    '    'lragioneSociale =  Me.Txt_RagioneSociale.Text

    '    QueryString = "?p=" & Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) & _
    '                    "&rs=" & Stringa_Codifica(AgronicaCoreDataProvider.UtilityProvider.QS_SaveText(lragioneSociale), AgroKey_EncoderDecoder, Server) & _
    '                    "&i=" & Stringa_Codifica(CStr(Qs_IdAgenda), AgroKey_EncoderDecoder, Server)

    '    AgronicaCoreDataProvider.UtilityProvider.Page_NewWindow(Page, _
    '                    "../GestioneAllegati.aspx", _
    '                    QueryString, _
    '                    "Allegati", _
    '                    550, _
    '                    1000, _
    '                    0, 0, _
    '                    , , , )



    'End Sub


    '##############################################################################################################
    Private Sub Cmb_Contatti_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cmb_Contatti.SelectedIndexChanged


        '=============================================================================
        '         CLIENTE / FORNITORE
        '=============================================================================

        Dim Tipo_Indirizzo As Integer

        'non è stato selezionato nessun contatto
        If Me.Cmb_Contatti.SelectedIndex < 1 Then

            Me.Txt_CodContatto_Contatto.Text = ""
            Me.Txt_Progressivo_Contatto.Text = ""
            Me.Txt_Attivita_Contatto.Text = ""
            Me.Txt_CodiceFiscale_Contatto.Text = ""
            Me.Txt_CodIndirizzo_Contatto.Text = "0"

            Me.List_Indirizzo_Contatto.Items.Clear()
            Me.Cmb_TipoIndirizzo_Contatto.Enabled = False
            Me.Cmb_TipoIndirizzo_Contatto.Items.Clear()

            Exit Sub

        Else
            Me.Cmb_TipoIndirizzo_Contatto.Enabled = True
            If Me.Cmb_TipoIndirizzo_Contatto.Items.Count > 0 Then
                Me.Cmb_TipoIndirizzo_Contatto.SelectedIndex = 0
            End If


            If Me.Cmb_TipoIndirizzo_Contatto.SelectedValue <> "" Then
                Tipo_Indirizzo = CInt(Me.Cmb_TipoIndirizzo_Contatto.SelectedValue)
            Else
                Tipo_Indirizzo = 0
            End If

        End If

        Dim Testo_Combo As String = ""
        Carica_Contatto(Testo_Combo,
                        CInt(Split(Me.Cmb_Contatti.SelectedValue, "|")(0)),
                        CStr(Split(Me.Cmb_Contatti.SelectedValue, "|")(1)),
                        Tipo_Indirizzo,
                        0)

        'TODO, verifica!
        Cmb_TipoIndirizzo_Contatto_SelectedIndexChanged(Me, Nothing)
        '=========================================================================

    End Sub


    '####################################################################################################
    Private Sub Carica_Contatto(ByRef TestoCombo As String,
                                ByVal Cod_Risum As Integer,
                                ByRef Piva_Contatto As String,
                                ByVal Tipo_Indirizzo As Integer,
                                ByVal Cod_Indirizzo As Integer)


        '=============================================================================
        '         CLIENTE / FORNITORE
        '=============================================================================

        Me.Txt_CodContatto_Contatto.Text = ""
        Me.Txt_Progressivo_Contatto.Text = ""
        Me.Txt_Attivita_Contatto.Text = ""
        Me.Txt_CodiceFiscale_Contatto.Text = ""
        Me.Txt_CodIndirizzo_Contatto.Text = "0"

        Me.List_Indirizzo_Contatto.Items.Clear()

        Dim Dt_Contatto As DataTable
        Dim Dt_Impresa_Contatto As DataTable = Nothing
        Dim Flag_Contatto_GIAS As Boolean
        Dim Cod_Contatto As String
        Dim ID_CF As Integer

        Dim leggiContatti As New AgronicaCoreAnagrafeDAL.Contatti_R
        Dt_Contatto = leggiContatti.Leggi(
            "",
            "",
            Cod_Risum,
            0,
            False,
            False,
            0,
            0,
            False,
            0,
            ID_CF_NOFILTRO,
            0,
            "",
            True,
            0,
            0,
            0,
            0,
            0,
            AGRODATAINIZIO,
            AGRODATAFINE,
            False,
            "",
            "",
            objParametri_Server
        )



        If Not IsNothing(Dt_Contatto) Then

            If Dt_Contatto.Rows.Count <> 0 Then

                TestoCombo = CStr(Dt_Contatto.Rows(0).Item("Rag_Soc")) &
                             CStr(Dt_Contatto.Rows(0).Item("Cognome")) & " " &
                             CStr(Dt_Contatto.Rows(0).Item("Nome")) &
                             " (" & Dt_Contatto.Rows(0).Item("Rapporto_Des") & ")"


                Cod_Contatto = Dt_Contatto.Rows(0).Item("Cod_Contatto")
                Piva_Contatto = Dt_Contatto.Rows(0).Item("Piva")

                If VerificaEsistenza_PivaGIAS(objParametri_Server, Cod_Contatto) = True Then
                    Me.Chk_Contatto_GIAS.Checked = True
                    Flag_Contatto_GIAS = True
                Else
                    Me.Chk_Contatto_GIAS.Checked = False
                    Flag_Contatto_GIAS = False
                End If

                Me.Txt_CodContatto_Contatto.Text = Cod_Contatto

                Me.Txt_CodiceFiscale_Contatto.Text = Dt_Contatto.Rows(0).Item("Codice_Fiscale")

                Me.Txt_Progressivo_Contatto.Text = Dt_Contatto.Rows(0).Item("Settore_Des")

                Me.Txt_Attivita_Contatto.Text = Dt_Contatto.Rows(0).Item("Attivita_Des")

                ID_CF = Dt_Contatto.Rows(0).Item("ID_CF")

                CaricaCombo_TipoIndirizzo_Contatto(Server, Session, Page, Me.Cmb_TipoIndirizzo_Contatto, Dt_Contatto.Rows(0).Item("Cod_Contatto"), ID_CF)

                If Cod_Indirizzo <> 0 Then
                    'recupero il tipo di indirizzo dal codice indirizzo e dal codice del contatto
                    Tipo_Indirizzo = TipoIndirizzo_from_IndirizzoContatto2(objParametri_Server, Session, Page, Cod_Indirizzo, Flag_Contatto_GIAS, Dt_Impresa_Contatto, Piva_Contatto, Cod_Contatto, Cod_Risum)
                End If

                If Tipo_Indirizzo <> 0 Then
                    Me.Cmb_TipoIndirizzo_Contatto.SelectedIndex = Me.Cmb_TipoIndirizzo_Contatto.Items.IndexOf(Me.Cmb_TipoIndirizzo_Contatto.Items.FindByValue(Tipo_Indirizzo))
                Else
                    Me.Cmb_TipoIndirizzo_Contatto.SelectedIndex = 0
                    Tipo_Indirizzo = CInt(Me.Cmb_TipoIndirizzo_Contatto.SelectedValue)
                End If

                CaricaListBox_Indirizzo_2(objParametri_Server, Session, Page,
                                                Me.List_Indirizzo_Contatto,
                                                Cod_Indirizzo,
                                                Flag_Contatto_GIAS,
                                                Dt_Impresa_Contatto,
                                                Piva_Contatto,
                                                Cod_Contatto,
                                                Cod_Risum,
                                                Tipo_Indirizzo)

                Me.Txt_CodIndirizzo_Contatto.Text = Cod_Indirizzo

                If Me.Chk_Contatto_GIAS.Checked Then
                    Me.Cmb_TipoIndirizzo_Contatto.Enabled = False
                Else
                    Me.Cmb_TipoIndirizzo_Contatto.Enabled = True
                End If

            End If

        End If


    End Sub



    '####################################################################################################
    Private Sub Cmb_TipoIndirizzo_Contatto_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cmb_TipoIndirizzo_Contatto.SelectedIndexChanged

        '=============================================================================
        '   TIPO INDIRIZZO       CLIENTE / FORNITORE
        '=============================================================================

        Dim Cod_Indirizzo As Integer = 0

        CaricaListBox_Indirizzo_2(objParametri_Server, Session, Page,
                                        Me.List_Indirizzo_Contatto,
                                        Cod_Indirizzo,
                                        Me.Chk_Contatto_GIAS.Checked,
                                        Nothing,
                                        Me.Cmb_Contatti.SelectedValue.Split("|")(1),
                                        Me.Txt_CodContatto_Contatto.Text,
                                        Me.Cmb_Contatti.SelectedValue.Split("|")(0),
                                        Me.Cmb_TipoIndirizzo_Contatto.SelectedValue)

        Me.Txt_CodIndirizzo_Contatto.Text = Cod_Indirizzo

        If Me.Chk_Contatto_GIAS.Checked Then
            Me.Cmb_TipoIndirizzo_Contatto.Enabled = False
        Else
            Me.Cmb_TipoIndirizzo_Contatto.Enabled = True
        End If

    End Sub


    ''##########################################################################################################
    'Private Sub Chk_DestinatarioDiverso_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Chk_DestinatarioDiverso.CheckedChanged

    '    '=============================================================================
    '    '    CHECK      DESTINATARIO DIVERSO
    '    '=============================================================================

    '    Me.Txt_CodContatto_DestDiverso.Text = ""
    '    Me.Txt_Progressivo_DestDiverso.Text = ""
    '    Me.Txt_Attivita_DestDiverso.Text = ""
    '    Me.Txt_CodiceFiscale_DestDiverso.Text = ""
    '    Me.Txt_CodIndirizzo_DestDiverso.Text = "0"

    '    Me.List_Indirizzo_DestDiverso.Items.Clear()


    '    If Me.Chk_DestinatarioDiverso.Checked = True Then
    '        'il check è attivo

    '        Me.Cmb_DestinatarioDiverso.Enabled = True
    '        Me.Txt_Cerca_Diverso.Enabled = True
    '        Me.Img_btn_CercaDiverso.Enabled = True
    '        Me.Txt_Cerca_Diverso2.Enabled = True
    '        Me.Img_btn_CercaDiverso2.Enabled = True

    '    Else

    '        Me.Cmb_DestinatarioDiverso.Items.Clear()
    '        Me.Cmb_DestinatarioDiverso.Enabled = False
    '        Me.Txt_Cerca_Diverso.Enabled = False
    '        Me.Img_btn_CercaDiverso.Enabled = False
    '        Me.Txt_Cerca_Diverso2.Enabled = False
    '        Me.Img_btn_CercaDiverso2.Enabled = False
    '        Me.Cmb_TipoIndirizzo_DestDiverso.Enabled = False
    '        Me.Cmb_TipoIndirizzo_DestDiverso.Items.Clear()

    '        Me.List_Indirizzo_DestDiverso.Items.Clear()

    '    End If

    'End Sub



    '##################################################################################################
    Private Sub Cmb_DestinatarioDiverso_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cmb_DestinatarioDiverso.SelectedIndexChanged

        '=============================================================================
        '             DESTINATARIO DIVERSO
        '=============================================================================

        Dim Tipo_Indirizzo As Integer

        'non è stato selezionato nessun contatto
        If Me.Cmb_DestinatarioDiverso.SelectedIndex < 1 Then

            Me.Txt_CodContatto_DestDiverso.Text = ""
            Me.Txt_Progressivo_DestDiverso.Text = ""
            Me.Txt_Attivita_DestDiverso.Text = ""
            Me.Txt_CodiceFiscale_DestDiverso.Text = ""
            Me.Txt_CodIndirizzo_DestDiverso.Text = "0"

            Me.List_Indirizzo_DestDiverso.Items.Clear()
            Me.Cmb_TipoIndirizzo_DestDiverso.Enabled = False
            Me.Cmb_TipoIndirizzo_DestDiverso.Items.Clear()

            Exit Sub
        Else
            Me.Cmb_TipoIndirizzo_DestDiverso.Enabled = True
            If Cmb_TipoIndirizzo_DestDiverso.Items.Count > 0 Then
                Me.Cmb_TipoIndirizzo_DestDiverso.SelectedIndex = 0
            End If


            If Me.Cmb_TipoIndirizzo_DestDiverso.SelectedValue <> "" Then
                Tipo_Indirizzo = CInt(Me.Cmb_TipoIndirizzo_DestDiverso.SelectedValue)
            Else
                Tipo_Indirizzo = 0
            End If
        End If

        Dim TestoCombo As String = ""
        Carica_DestinatarioDiverso(TestoCombo,
                                   CInt(Split(Me.Cmb_DestinatarioDiverso.SelectedValue, "|")(0)),
                                    CStr(Split(Me.Cmb_DestinatarioDiverso.SelectedValue, "|")(1)),
                                    Tipo_Indirizzo,
                                    0)

        Cmb_TipoIndirizzo_DestDiverso_SelectedIndexChanged(Me, Nothing)

    End Sub


    '########################################################################################################################
    Private Sub Carica_DestinatarioDiverso(ByRef TestoCombo As String,
                                           ByVal Cod_Risum As Integer,
                                            ByRef Piva_Contatto As String,
                                            ByVal Tipo_Indirizzo As Integer,
                                            ByVal Cod_Indirizzo As Integer)


        '=============================================================================
        '             DESTINATARIO DIVERSO
        '=============================================================================

        Me.Txt_CodContatto_DestDiverso.Text = ""
        Me.Txt_Progressivo_DestDiverso.Text = ""
        Me.Txt_Attivita_DestDiverso.Text = ""
        Me.Txt_CodiceFiscale_DestDiverso.Text = ""
        Me.Txt_CodIndirizzo_DestDiverso.Text = "0"

        Me.List_Indirizzo_DestDiverso.Items.Clear()

        '---------

        Dim Dt_Contatto As DataTable
        Dim Dt_Impresa_Contatto As DataTable = Nothing
        Dim Flag_Contatto_GIAS As Boolean
        Dim Cod_Contatto As String
        Dim ID_CF As Integer

        Dim leggiContatti As New AgronicaCoreAnagrafeDAL.Contatti_R
        Dt_Contatto = leggiContatti.Leggi(
                       "",
                       "",
                       Cod_Risum,
                       0,
                       False,
                       False,
                       0,
                       0,
                       False,
                       0,
                       ID_CF_NOFILTRO,
                       0,
                       "",
                       True,
                       0,
                       0,
                       0,
                       0,
                       0,
                       AGRODATAINIZIO,
                       AGRODATAFINE,
                       False,
                       "",
                       "",
                       objParametri_Server
   )




        If Not IsNothing(Dt_Contatto) Then

            If Dt_Contatto.Rows.Count <> 0 Then

                TestoCombo = CStr(Dt_Contatto.Rows(0).Item("Rag_Soc")) &
                             CStr(Dt_Contatto.Rows(0).Item("Cognome")) & " " &
                             CStr(Dt_Contatto.Rows(0).Item("Nome")) &
                             " (" & Dt_Contatto.Rows(0).Item("Rapporto_Des") & ")"

                Cod_Contatto = Dt_Contatto.Rows(0).Item("Cod_Contatto")
                Piva_Contatto = Dt_Contatto.Rows(0).Item("Piva")

                If VerificaEsistenza_PivaGIAS(objParametri_Server, Cod_Contatto) = True Then
                    Me.Chk_Destinatario_GIAS.Checked = True
                    Flag_Contatto_GIAS = True
                Else
                    Me.Chk_Destinatario_GIAS.Checked = False
                    Flag_Contatto_GIAS = False
                End If

                Me.Txt_CodContatto_DestDiverso.Text = Cod_Contatto

                Me.Txt_CodiceFiscale_DestDiverso.Text = Dt_Contatto.Rows(0).Item("Codice_Fiscale")

                Me.Txt_Progressivo_DestDiverso.Text = Dt_Contatto.Rows(0).Item("Settore_Des")

                Me.Txt_Attivita_DestDiverso.Text = Dt_Contatto.Rows(0).Item("Attivita_Des")

                ID_CF = Dt_Contatto.Rows(0).Item("ID_CF")

                CaricaCombo_TipoIndirizzo_Contatto(Server, Session, Page, Me.Cmb_TipoIndirizzo_DestDiverso, Dt_Contatto.Rows(0).Item("Cod_Contatto"), id_cf)

                If Cod_Indirizzo <> 0 Then
                    'recupero il tipo di indirizzo dal codice indirizzo e dal codice del contatto
                    Tipo_Indirizzo = TipoIndirizzo_from_IndirizzoContatto2(objParametri_Server, Session, Page, Cod_Indirizzo, Flag_Contatto_GIAS, Dt_Impresa_Contatto, Piva_Contatto, Cod_Contatto, Cod_Risum)
                End If

                If Tipo_Indirizzo <> 0 Then
                    Me.Cmb_TipoIndirizzo_DestDiverso.SelectedIndex = Me.Cmb_TipoIndirizzo_DestDiverso.Items.IndexOf(Me.Cmb_TipoIndirizzo_DestDiverso.Items.FindByValue(Tipo_Indirizzo))
                Else
                    Me.Cmb_TipoIndirizzo_DestDiverso.SelectedIndex = 0
                    Tipo_Indirizzo = CInt(Me.Cmb_TipoIndirizzo_DestDiverso.SelectedValue)
                End If

                'carico già il primo indirizzo
                CaricaListBox_Indirizzo_2(objParametri_Server, Session, Page,
                                                Me.List_Indirizzo_DestDiverso,
                                                Cod_Indirizzo,
                                                Flag_Contatto_GIAS,
                                                Dt_Impresa_Contatto,
                                                Piva_Contatto,
                                                Cod_Contatto,
                                                Cod_Risum,
                                                Tipo_Indirizzo)

                Me.Txt_CodIndirizzo_DestDiverso.Text = Cod_Indirizzo

                If Me.Chk_Destinatario_GIAS.Checked Then
                    Me.Cmb_TipoIndirizzo_DestDiverso.Enabled = False
                Else
                    Me.Cmb_TipoIndirizzo_DestDiverso.Enabled = True
                End If

            End If

        End If

    End Sub



    '########################################################################################################################
    Private Sub Cmb_TipoIndirizzo_DestDiverso_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cmb_TipoIndirizzo_DestDiverso.SelectedIndexChanged

        '=============================================================================
        '    TIPO INDIRIZZO      DESTINATARIO DIVERSO
        '=============================================================================

        Dim Cod_Indirizzo As Integer = 0

        CaricaListBox_Indirizzo_2(objParametri_Server, Session, Page,
                                    Me.List_Indirizzo_DestDiverso,
                                    Cod_Indirizzo,
                                    Me.Chk_Destinatario_GIAS.Checked,
                                        Nothing,
                                        Me.Cmb_DestinatarioDiverso.SelectedValue.Split("|")(1),
                                        Me.Txt_CodContatto_DestDiverso.Text,
                                        Me.Cmb_DestinatarioDiverso.SelectedValue.Split("|")(0),
                                        Me.Cmb_TipoIndirizzo_DestDiverso.SelectedValue)


        Me.Txt_CodIndirizzo_DestDiverso.Text = Cod_Indirizzo

        If Me.Chk_Destinatario_GIAS.Checked Then
            Me.Cmb_TipoIndirizzo_DestDiverso.Enabled = False
        Else
            Me.Cmb_TipoIndirizzo_DestDiverso.Enabled = True
        End If


    End Sub




    '###################################################################
    Private Sub ImgBtn_CercaVettore_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_CercaVettore.Click

        If Rbl_Trasporto.SelectedIndex <> 2 Then
            Messaggi.AgroMsgBox("Per cercare un vettore è necessario impostare 'trasporto a cura del Vettore'", Page, "aspnetForm", upTabs_1)
        Else
            If Me.txt_CercaVettore.Text = "" Then
                Messaggi.AgroMsgBox("Scrivere parte della ragione sociale del vettore prima di avviare il filtro di ricerca.", Page, "aspnetForm", upTabs_1)
            Else
                FiltraComboVettore(Me.txt_CercaVettore.Text)
            End If
        End If

    End Sub


    '############################################################
    Private Sub FiltraComboVettore(ByVal TestoRicerca As String)

        Dim filtro As String = ""

        filtro &= " (Rapporti_Contabili.Fornitore = 1 OR Rapporti_Contabili.Terzista = 1) "


        filtro &= String.Format("AND (Contatti.rag_soc like '%{0}%' OR Contatti.nome like '%{0}%' OR Contatti.cognome like '%{0}%') ",
                                TestoRicerca)

        'carico la combo dei vettori, che devono essere dei terzisti
        CaricaCombo_Contatti(objParametri_Server, Session, Page, Me.Cmb_Vettore,
                                    Qs_Piva,
                                    CStr(Session("ASG_SuperUser_CodFiscale")),
                                    , , , , ,
                                    filtro,
                                    Me.Txt_DataEmissione.Text,
                                    Me.Txt_DataEmissione.Text,
                                    v_Flag_AncheImportati,
                                    False)

        If Me.Cmb_Vettore.Items.Count > 1 Then
            Me.Cmb_Vettore.SelectedIndex = 1
            Cmb_Vettore_SelectedIndexChanged(Me, Nothing)
        End If

    End Sub



    '##########################################################################################################
    Private Sub Rbl_Trasporto_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Rbl_Trasporto.SelectedIndexChanged

        Me.Txt_CodContatto_Vettore.Text = ""
        Me.Txt_CodiceFiscale_Vettore.Text = ""
        Me.Txt_CodIndirizzo_Vettore.Text = "0"

        Me.List_Indirizzo_Vettore.Items.Clear()


        Select Case Rbl_Trasporto.SelectedIndex

            Case 2

                'TRASPORTO A CARICO DEL VETTORE

                'abilito la combo
                Me.Cmb_Vettore.Enabled = True

                ''carico la combo dei vettori, che devono essere dei terzisti
                'CaricaCombo_Contatti(objParametri_Server, Session, Page, Me.Cmb_Vettore, _
                '                            Qs_Piva, _
                '                            CStr(Session("ASG_SuperUser_CodFiscale")), _
                '                            , , , 1, , _
                '                            "", _
                '                            Me.Txt_DataEmissione.Text, _
                '                            Me.Txt_DataEmissione.Text, _
                '                            v_Flag_AncheImportati, _
                '                            False)

            Case Else

                'VETTORE    disabilito le combo 

                Me.Cmb_Vettore.Enabled = False
                Me.Cmb_Vettore.Items.Clear()
                Me.Cmb_TipoIndirizzo_Vettore.Enabled = False
                Me.Cmb_TipoIndirizzo_Vettore.Items.Clear()

        End Select



    End Sub


    '###########################################################################################################
    Private Sub Cmb_Vettore_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cmb_Vettore.SelectedIndexChanged

        '=============================================================================
        '                 VETTORE
        '=============================================================================

        Dim Tipo_Indirizzo As Integer

        'non è stato selezionato nessun contatto
        If Me.Cmb_Vettore.SelectedIndex < 1 Then

            Me.Txt_CodContatto_Vettore.Text = ""
            Me.Txt_CodiceFiscale_Vettore.Text = ""
            Me.Txt_CodIndirizzo_Vettore.Text = "0"

            Me.List_Indirizzo_Vettore.Items.Clear()

            Me.Cmb_TipoIndirizzo_Vettore.Enabled = False
            Me.Cmb_TipoIndirizzo_Vettore.Items.Clear()

            Exit Sub

        Else
            Me.Cmb_TipoIndirizzo_Vettore.Enabled = True
            If Cmb_TipoIndirizzo_Vettore.Items.Count > 0 Then
                Me.Cmb_TipoIndirizzo_Vettore.SelectedIndex = 0
            End If


            If Me.Cmb_TipoIndirizzo_Vettore.SelectedValue <> "" Then
                Tipo_Indirizzo = CInt(Me.Cmb_TipoIndirizzo_Vettore.SelectedValue)
            Else
                Tipo_Indirizzo = 0
            End If
        End If

        Dim Testo_Combo As String = ""
        Carica_Vettore(Testo_Combo,
                       CInt(Split(Me.Cmb_Vettore.SelectedValue, "|")(0)),
                        CStr(Split(Me.Cmb_Vettore.SelectedValue, "|")(1)),
                        Tipo_Indirizzo,
                        0)


        Cmb_TipoIndirizzo_Vettore_SelectedIndexChanged(Me, Nothing)
    End Sub

    '########################################################################################################################
    Private Sub Carica_Vettore(ByRef TestoCombo As String,
                               ByVal Cod_Risum As Integer,
                                ByRef Piva_Contatto As String,
                                ByVal Tipo_Indirizzo As Integer,
                                ByVal Cod_Indirizzo As Integer
                                )


        '=============================================================================
        '                 VETTORE
        '=============================================================================

        Me.Txt_CodContatto_Vettore.Text = ""
        Me.Txt_CodiceFiscale_Vettore.Text = ""
        Me.Txt_CodIndirizzo_Vettore.Text = "0"

        Me.List_Indirizzo_Vettore.Items.Clear()

        '---------

        Dim Dt_Contatto As DataTable
        Dim Dt_Impresa_Contatto As DataTable = Nothing
        Dim Flag_Contatto_GIAS As Boolean
        Dim Cod_Contatto As String
        Dim ID_CF As Integer

        Dim leggiContatti As New AgronicaCoreAnagrafeDAL.Contatti_R
        Dt_Contatto = leggiContatti.Leggi(
            "",
            "",
            Cod_Risum,
            0,
            False,
            False,
            0,
            0,
            False,
            0,
            ID_CF_NOFILTRO,
            0,
            "",
            True,
            0,
            0,
            0,
            0,
            0,
            AGRODATAINIZIO,
            AGRODATAFINE,
            False,
            "",
            "",
            objParametri_Server
        )



        If Not IsNothing(Dt_Contatto) Then

            If Dt_Contatto.Rows.Count <> 0 Then

                TestoCombo = CStr(Dt_Contatto.Rows(0).Item("Rag_Soc")) &
                             CStr(Dt_Contatto.Rows(0).Item("Cognome")) & " " &
                             CStr(Dt_Contatto.Rows(0).Item("Nome")) &
                             " (" & Dt_Contatto.Rows(0).Item("Rapporto_Des") & ")"

                Cod_Contatto = Dt_Contatto.Rows(0).Item("Cod_Contatto")
                Piva_Contatto = Dt_Contatto.Rows(0).Item("Piva")

                If VerificaEsistenza_PivaGIAS(objParametri_Server, Cod_Contatto) = True Then
                    Me.Chk_Vettore_GIAS.Checked = True
                    Flag_Contatto_GIAS = True
                Else
                    Me.Chk_Vettore_GIAS.Checked = False
                    Flag_Contatto_GIAS = False
                End If

                Me.Txt_CodContatto_Vettore.Text = Cod_Contatto
                Me.Txt_CodiceFiscale_Vettore.Text = Dt_Contatto.Rows(0).Item("Codice_Fiscale")

                ID_CF = Dt_Contatto.Rows(0).Item("ID_CF")

                CaricaCombo_TipoIndirizzo_Contatto(Server, Session, Page, Me.Cmb_TipoIndirizzo_Vettore, Dt_Contatto.Rows(0).Item("Cod_Contatto"), id_cf)

                If Cod_Indirizzo <> 0 Then
                    'recupero il tipo di indirizzo dal codice indirizzo e dal codice del contatto
                    Tipo_Indirizzo = TipoIndirizzo_from_IndirizzoContatto2(objParametri_Server, Session, Page, Cod_Indirizzo, Flag_Contatto_GIAS, Dt_Impresa_Contatto, Piva_Contatto, Cod_Contatto, Cod_Risum)
                End If

                If Tipo_Indirizzo <> 0 Then
                    Me.Cmb_TipoIndirizzo_Vettore.SelectedIndex = Me.Cmb_TipoIndirizzo_Vettore.Items.IndexOf(Me.Cmb_TipoIndirizzo_Vettore.Items.FindByValue(Tipo_Indirizzo))
                Else
                    Me.Cmb_TipoIndirizzo_Vettore.SelectedIndex = 0
                    Tipo_Indirizzo = CInt(Me.Cmb_TipoIndirizzo_Vettore.SelectedValue)
                End If

                'carico già il primo indirizzo
                CaricaListBox_Indirizzo_2(objParametri_Server, Session, Page,
                                                Me.List_Indirizzo_Vettore,
                                                Cod_Indirizzo,
                                                Flag_Contatto_GIAS,
                                                Dt_Impresa_Contatto,
                                                Piva_Contatto,
                                                Cod_Contatto,
                                                Cod_Risum,
                                                Tipo_Indirizzo)

                Me.Txt_CodIndirizzo_Vettore.Text = Cod_Indirizzo

                If Me.Chk_Vettore_GIAS.Checked Then
                    Me.Cmb_TipoIndirizzo_Vettore.Enabled = False
                Else
                    Me.Cmb_TipoIndirizzo_Vettore.Enabled = True
                End If

            End If

        End If



    End Sub


    '##########################################################################################################
    Private Sub Cmb_TipoIndirizzo_Vettore_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cmb_TipoIndirizzo_Vettore.SelectedIndexChanged

        '=============================================================================
        '    TIPO INDIRIZZO        VETTORE
        '=============================================================================

        Dim Cod_Indirizzo As Integer = 0

        CaricaListBox_Indirizzo_2(objParametri_Server, Session, Page,
                                        Me.List_Indirizzo_Vettore,
                                        Cod_Indirizzo,
                                        Me.Chk_Vettore_GIAS.Checked,
                                        Nothing,
                                        Me.Cmb_Vettore.SelectedValue.Split("|")(1),
                                        Me.Txt_CodContatto_Vettore.Text,
                                        Me.Cmb_Vettore.SelectedValue.Split("|")(0),
                                        Me.Cmb_TipoIndirizzo_Vettore.SelectedValue)

        Me.Txt_CodIndirizzo_Vettore.Text = Cod_Indirizzo

        If Me.Chk_Vettore_GIAS.Checked Then
            Me.Cmb_TipoIndirizzo_Vettore.Enabled = False
        Else
            Me.Cmb_TipoIndirizzo_Vettore.Enabled = True
        End If

    End Sub



    '####################################################################################################################
    Private Sub Chk_ParcoMacchine_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Chk_ParcoMacchine.CheckedChanged

        'Select Case Me.Chk_ParcoMacchine.Checked

        '    Case True

        '        CaricaCombo_TargaMacchine(Server, Session, Page, Me.Cmb_Targa, Qs_Piva)

        '        Me.Txt_Targa.Visible = False
        '        Me.Cmb_Targa.Visible = True

        '        Me.Txt_Immatr.ReadOnly = True
        '        Me.Txt_ImmatrRimorchio.ReadOnly = True
        '        Me.Txt_Autorizz.ReadOnly = True
        '        Me.Txt_PesoTara.ReadOnly = True

        '    Case False

        '        Me.Txt_Targa.Visible = True
        '        Me.Cmb_Targa.Visible = False

        '        Me.Txt_Immatr.ReadOnly = False
        '        Me.Txt_ImmatrRimorchio.ReadOnly = False
        '        Me.Txt_Autorizz.ReadOnly = False
        '        Me.Txt_PesoTara.ReadOnly = False


        'End Select


    End Sub


    '####################################################################################################################
    Private Sub Cmb_Targa_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cmb_Targa.SelectedIndexChanged

        'Dim DT As DataTable
        'Dim Mac_Cod As Integer = 0
        'Dim i As Integer = 0

        'If Me.Cmb_Targa.SelectedIndex > 0 Then

        '    Mac_Cod = Me.Cmb_Targa.SelectedValue

        '    'leggi macchine
        '    DT = NewCom_ParcoMacchine_Leggi(Server, Session, Page, Qs_Piva, Mac_Cod, True, , , , , , , , , False)

        '    For i = 0 To DT.Rows.Count - 1

        '        'Impostazione Macchina
        '        Me.Txt_Immatr.Text = DT.Rows(i).Item("N_Immatricolazione")
        '        Me.Txt_ImmatrRimorchio.Text = DT.Rows(i).Item("N_Immatricolazione_Rimorchio")
        '        Me.Txt_Autorizz.Text = DT.Rows(i).Item("N_Autorizzazione_Trasporto")

        '        If DT.Rows(i).Item("Data_Rilascio_Autorizzazione") = "01/01/1900" Then
        '            Me.Txt_DataAutorizz.Text = ""
        '        Else
        '            Me.Txt_DataAutorizz.Text = DT.Rows(i).Item("Data_Rilascio_Autorizzazione")
        '        End If

        '        Me.Txt_PesoTara.Text = DT.Rows(i).Item("Peso")

        '    Next

        'End If


    End Sub


    '####################################################################################
    Private Sub Txt_DataEmissione_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Txt_DataEmissione.TextChanged


        'Select Case CInt(Qs_Operazione)

        '    Case enum_TipoOperazioneDB.Scrittura, enum_TipoOperazioneDB.Modifica

        '        Select Case QS_LavCod

        '            Case CStr(LAVCOD_FATTURA_RICEVUTA)

        '                'RICEVIMENTO FATTURA --> CARICO DI MAGAZZINO

        '                'CARICO I FORNITORI

        '                CaricaCombo_Contatti(objParametri_Server, Session, Page, Me.Cmb_Contatti, _
        '                                        Qs_Piva, _
        '                                        CStr(Session("ASG_SuperUser_CodFiscale")), _
        '                                        , _
        '                                        , _
        '                                        , _
        '                                        , _
        '                                        , _
        '                                        " AND (Rapporti_Contabili.Fornitore = 1 OR Rapporti_Contabili.Terzista = 1) ", _
        '                                        CDate(Me.Txt_DataEmissione.Text), _
        '                                        CDate(Me.Txt_DataEmissione.Text), _
        '                                        v_Flag_AncheImportati, _
        '                                        False)


        '                '==========================================================================

        '            Case CStr(LAVCOD_FATTURA_EMESSA)

        '                'EMISSIONE FATTURA --> SCARICO DI MAGAZZINO

        '                'CARICO I CLIENTI

        '                CaricaCombo_Contatti(objParametri_Server, Session, Page, Me.Cmb_Contatti, _
        '                                       Qs_Piva, _
        '                                       CStr(Session("ASG_SuperUser_CodFiscale")), _
        '                                       0, _
        '                                       0, _
        '                                       0, _
        '                                       0, _
        '                                       0, _
        '                                       " AND     (Rapporti_Contabili.Cliente = 1 OR Rapporti_Contabili.Terzista = 1 ) ", _
        '                                       CDate(Me.Txt_DataEmissione.Text), _
        '                                       CDate(Me.Txt_DataEmissione.Text), _
        '                                       v_Flag_AncheImportati, _
        '                                       False)

        '        End Select

        '        Messaggi.AgroMsgBox("Attenzione!!!" & vbCrLf & "Poiché è stata modificata la data del documento, è stato ricaricato l'elenco dei Contatti.", Page, NomeForm:="aspnetForm", UpdatePanel:=upTabs_1)

        '        '-----------

        '    Case enum_TipoOperazioneDB.Lettura

        '        'If CInt(Qs_Operazione) = enum_TipoOperazioneDB.Lettura Then
        '        '    v_Flag_AncheImportati = True
        '        'End If


        'End Select


    End Sub


    ''####################################################################################
    'Private Sub ToolbarOperazioni_ButtonClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles ToolbarOperazioni.ButtonClick

    '    Gestione_OperazioniToolbar(sender, e)


    'End Sub

    '########################################################################################
    Private Sub Gestione_OperazioniToolbar(ByVal sender As Object, ByVal e As System.EventArgs)

        Select Case sender.id.ToString.ToLower

            'Case ID_Zootecnico.ID.ToLower

            '    Messaggi.AgroMsgBox("Funzionalità disattivata temporaneamente.", Page, NomeForm:="aspnetForm", UpdatePanel:=upTabs_2)


            Case ID_Prodotto.ID.ToLower

                'Select Case Me.Rbl_TipoFattura.SelectedValue

                '    Case 0, 1  'immediata e differita
                'Messaggi.AgroMsgBox("Nelle fatture differite è necessario allegare dei Documenti di Trasporto" & _
                'vbCrLf & "(lo scarico del prodotto dal magazzino è già stato effettuato con la registrazione del D.d.T.).", Page, NomeForm:="aspnetForm", UpdatePanel:=upTabs_2)

                'Case 1 'immediata

                'If sender.id.ToString = ID_Zootecnico.ID.ToLower And QS_LavCod = LAVCOD_FATTURA_EMESSA And QS_LavCod = LAVCOD_NOTA_ACCREDITO_EMESSA Then

                '    Messaggi.AgroMsgBox("Non è possibile inserire delle Consistenze Zootecniche in una fattura/nota di accredito emessa.", Page, NomeForm:="aspnetForm", UpdatePanel:=upTabs_2)

                'Else

                Dim CaricoScarico As String = ""
                Dim Chiave, Tipo, Cod_Contatto, Ragsoc_Contatto As String
                    Dim QueryString As String

                    Select Case sender.id.ToString.ToLower
                        Case ID_Prodotto.ID.ToLower
                            Tipo = CAU_MAGAZZINO
                        Case Else
                            Tipo = CAU_ANIMALE
                    End Select

                    Albero.ChiaveAlbero_Codifica(Chiave, enum_TipoNodo.f_Magazzino, Qs_Piva, Qs_SaCod, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)

                    Select Case QS_LavCod
                        Case LAVCOD_FATTURA_RICEVUTA, LAVCOD_BOLLA_RICEVUTA, LAVCOD_NOTA_ACCREDITO_EMESSA
                            CaricoScarico = "C"
                        Case LAVCOD_FATTURA_EMESSA, LAVCOD_BOLLA_EMESSA, LAVCOD_NOTA_ACCREDITO_RICEVUTA
                            CaricoScarico = "S"
                    End Select


                    If Me.Cmb_Contatti.SelectedValue <> "" Then

                        If Me.Cmb_Contatti.SelectedValue.Split("|")(0) <> "" Then
                            If VerificaEsistenza_CodContatto_as_PivaGIAS(objParametri_Server, Session, Page,
                                        Me.Cmb_Contatti.SelectedValue.Split("|")(0)) = True Then
                                Cod_Contatto = Me.Txt_CodContatto_Contatto.Text
                                Ragsoc_Contatto = Me.Cmb_Contatti.SelectedItem.Text
                            Else
                                Cod_Contatto = ""
                                Ragsoc_Contatto = ""
                            End If
                        Else
                            Cod_Contatto = ""
                            Ragsoc_Contatto = ""
                        End If



                        Dim lTxtRagioneSociale As String = ""
                        'lTxtRagioneSociale =  Me.Txt_RagioneSociale.Text

                        Dim lModeDatoLavCod As String = "fattura"

                        If QS_LavCod = LAVCOD_BOLLA_RICEVUTA OrElse
                            QS_LavCod = LAVCOD_BOLLA_EMESSA Then
                            lModeDatoLavCod = "bolla"
                        End If

                        QueryString = "?o=" &
                         Stringa_Codifica(enum_TipoOperazioneDB.Scrittura, AgroKey_EncoderDecoder, Server) &
                         "&c=" &
                         Stringa_Codifica(CaricoScarico, AgroKey_EncoderDecoder, Server) &
                         "&k=" &
                         Stringa_Codifica(Chiave, AgroKey_EncoderDecoder, Server) &
                         "&orig=" &
                         Stringa_Codifica(enum_PagineGiasOnline.Fattura, AgroKey_EncoderDecoder, Server) &
                         "&mode=" &
                         Stringa_Codifica(lModeDatoLavCod, AgroKey_EncoderDecoder, Server) &
                         "&l=" &
                         Stringa_Codifica(QS_LavCod, AgroKey_EncoderDecoder, Server) &
                         "&d=" &
                         Stringa_Codifica(Me.Txt_DataEmissione.Text, AgroKey_EncoderDecoder, Server) &
                        "&ora=" &
                         Stringa_Codifica("12.00", AgroKey_EncoderDecoder, Server) &
                         "&tipo=" &
                         Stringa_Codifica(Tipo, AgroKey_EncoderDecoder, Server) &
                         "&codcont=" &
                         Stringa_Codifica(Cod_Contatto, AgroKey_EncoderDecoder, Server) &
                         "&ragcont=" &
                         Stringa_Codifica(AgronicaCoreDataProvider.UtilityProvider.QS_SaveText(Ragsoc_Contatto), AgroKey_EncoderDecoder, Server) &
                         "&ragsoc=" &
                         Stringa_Codifica(AgronicaCoreDataProvider.UtilityProvider.QS_SaveText(lTxtRagioneSociale), AgroKey_EncoderDecoder, Server) &
                         "&ts=" &
                         Stringa_Codifica(Me.Cmb_TipoSconto.SelectedValue, AgroKey_EncoderDecoder, Server) &
                         "&dialog=" &
                         Stringa_Codifica("true", AgroKey_EncoderDecoder, Server)


                        '  Vanni, 28/05/2014 16:51:13: vecchia chiamata..
                        ' 770, 1020, 0, 0, _
                        Dim StrWindowOpen As String = AgronicaCoreDataProvider.UtilityProvider.JqueryModalDialogScript(
                                        "../GestioneMagazzini/FormProdotto.aspx", QueryString, InsProdotto.ClientID,
                                        0, 0, 0, 0,
                                        , , , , , , NomeForm:="aspnetForm")

                        If upTabs_2 Is Nothing Then
                            Page.FindControl("aspnetForm").Controls.Add(New LiteralControl(StrWindowOpen))
                        Else
                            ScriptManager.RegisterClientScriptBlock(upTabs_2, upTabs_2.GetType(),
                                                         String.Format("jQuery_{0}", "openmodal"), StrWindowOpen, True)

                        End If

                    Else
                        Messaggi.AgroMsgBox("Non è possibile aprire la pagina, perchè non è stato selezionato il contatto nella testata del documento.", Page, NomeForm:="aspnetForm", UpdatePanel:=upTabs_2)
                    End If


                'End If

                'End Select 'tipo fattura


                '--------------------------------------------------

                'Case "ToolbarButton - ID_Zootecnico"

                '    messaggi.AgroMsgBox("Al momento non è ancora possibile caricare prodotti animali.", Page)

                '--------------------------------------------------

                    Case ID_Cancella.ID.ToLower

                Session("DT_Prodotti_nel_Doc") = Nothing
                Session("DT_IVA_nel_Doc") = Nothing

                'Associo il DataTable con la DataGrid
                Me.DataGrid_Prodotti.DataSource = Nothing
                Me.DataGrid_Prodotti.DataBind()

                Me.DataGrid_IVA.DataSource = Nothing
                Me.DataGrid_IVA.DataBind()

                Visualizza_RiepilogoDocumento()

                Me.Cmb_Contatti.Enabled = True


                '--------------------------------------------------

            Case ID_Allegati.ID.ToLower

                Select Case Me.Rbl_TipoFattura.SelectedValue

                    Case 1 'immediata
                        Messaggi.AgroMsgBox("E' consentito allegare Documenti di Trasporto solo alle Fatture Differite.", Page, NomeForm:="aspnetForm", UpdatePanel:=upTabs_2)

                    Case 0 'differita

                        Dim QueryString As String
                        Dim Cod_RisUm As Integer

                        Cod_RisUm = CInt(Me.Cmb_Contatti.SelectedValue.Split("|")(0))

                        QueryString = "?p=" & Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) &
                                          "&rs=" & Stringa_Codifica(Qs_Rag_Soc, AgroKey_EncoderDecoder, Server) &
                                          "&a=" & Stringa_Codifica(CStr(CDate(Qs_DataSelezionata).Year), AgroKey_EncoderDecoder, Server) &
                                          "&d=" & Stringa_Codifica(Qs_DataSelezionata, AgroKey_EncoderDecoder, Server) &
                                          "&mode=" & Stringa_Codifica(CStr(enum_TipoModalita.ModAssociazione), AgroKey_EncoderDecoder, Server) &
                                          "&cod_ru=" & Stringa_Codifica(CStr(Cod_RisUm), AgroKey_EncoderDecoder, Server) &
                                          "&fil=" & Stringa_Codifica("", AgroKey_EncoderDecoder, Server) &
                                          "&orig=" & Stringa_Codifica(CStr(enum_PagineGiasOnline.Fattura), AgroKey_EncoderDecoder, Server) &
                                          "&dialog=" &
                                          Stringa_Codifica("true", AgroKey_EncoderDecoder, Server) &
                                          "&lc=" & Stringa_Codifica(QS_LavCod, AgroKey_EncoderDecoder, Server)


                        '  Vanni, 28/05/2014 16:51:54: vecchia chiamata
                        '770, 1020, 0, 0, _
                        Dim StrWindowOpen As String = AgronicaCoreDataProvider.UtilityProvider.JqueryModalDialogScript(
                                        "FiltroMovContabili.aspx", QueryString, Input_AllegaDoc.ClientID,
                                        0, 0, 0, 0,
                                        , , , , , , NomeForm:="aspnetForm")

                        If upTabs_2 Is Nothing Then
                            Page.FindControl("aspnetForm").Controls.Add(New LiteralControl(StrWindowOpen))
                        Else
                            ScriptManager.RegisterClientScriptBlock(upTabs_2, upTabs_2.GetType(),
                                                         String.Format("jQuery_{0}", "openmodal"), StrWindowOpen, True)

                        End If

                End Select

                '--------------------------------------------------

                'Case ID_Stampa.ID.ToLower


                '    If Qs_Operazione = enum_TipoOperazioneDB.Lettura Then

                '        'se non ho selezionato una FATTURA emessa
                '        Select Case QS_LavCod
                '            Case LAVCOD_FATTURA_EMESSA, LAVCOD_BOLLA_EMESSA, LAVCOD_NOTA_ACCREDITO_EMESSA
                '            Case Else
                '                Messaggi.AgroMsgBox("E' possibile stampare solo le Documenti Emessi!", Page, NomeForm:="aspnetForm", UpdatePanel:=upTabs_2)
                '                Exit Sub
                '        End Select

                '        '####################################################################
                '        '#####  Costruisco la stringa xml e lancio la stampa  ###############
                '        '####################################################################

                '        Stampa_Documento(Server, objParametri_Server, Session, Page, QS_LavCod, Qs_Piva, Qs_IdAgenda, "../../", UpdatePanel:=upTabs_2)



                '    Else

                '        Messaggi.AgroMsgBox("Impossibile stampare la Fattura prima di aver salvato!", Page, NomeForm:="aspnetForm", UpdatePanel:=upTabs_2)

                '    End If

        End Select


    End Sub

    '########################################################################################
    Private Sub CaricaGriglia_IVA(ByRef Hash_IVA As Hashtable)


        '----- Definizione delle variabili
        Dim MyKeys As ICollection
        Dim Key As Object
        Dim Dt As New DataTable
        Dim Dr As DataRow
        Dim Valore As String

        '----- Definisco la struttura del DataTable

        Dt.Columns.Add(New DataColumn("Cod_IVA", GetType(String)))
        Dt.Columns.Add(New DataColumn("Aliquota", GetType(String)))
        Dt.Columns.Add(New DataColumn("Imponibile", GetType(String)))
        Dt.Columns.Add(New DataColumn("IVA", GetType(String)))
        Dt.Columns.Add(New DataColumn("Importo", GetType(String)))

        '------------------------------------------------------

        Dim DtKeys(0) As DataColumn

        'Valorizzo le celle del vettore
        DtKeys(0) = Dt.Columns("Cod_IVA")

        'Assegno il vettore delle chiavi al DataTable
        Dt.PrimaryKey = DtKeys

        '------------------------------------------------------

        If (Hash_IVA.Count <> 0) Then

            MyKeys = Hash_IVA.Keys()

            For Each Key In MyKeys

                Dr = Dt.NewRow

                Dr.Item("Cod_Iva") = CStr(Key).Split("|")(0)
                Dr.Item("Aliquota") = CStr(Key).Split("|")(1)

                Valore = CStr(Hash_IVA(Key))

                Dr.Item("Imponibile") = Format(CDbl(Valore.Split("|")(0)), "#,###,##0.00")
                Dr.Item("IVA") = Format(CDbl(Valore.Split("|")(1)), "#,###,##0.00")
                Dr.Item("Importo") = Format(CDbl(Valore.Split("|")(2)), "#,###,##0.00")

                Dt.Rows.Add(Dr)

            Next

        End If


        '----- Associo il DataTable con la DataGrid

        Me.DataGrid_IVA.Visible = True
        Me.DataGrid_IVA.DataSource = Dt
        Me.DataGrid_IVA.DataBind()

        Session("DT_IVA_nel_Doc") = Dt

        Hash_IVA = Nothing


    End Sub



    '########################################################################################
    Private Sub Riempi_Hash_IVA(ByRef hash_iva As Hashtable,
                                ByVal Cod_IVA As Integer, ByVal Aliquota As String,
                                ByVal Imponibile_Netto As Decimal,
                                ByVal IVA As Decimal)


        If IsNothing(hash_iva) Then

            hash_iva = New Hashtable

        End If

        '----------------------------------------

        '--- Report IVA
        'Dim chiave As Integer
        'dim valore as Decimal
        Dim temp_imp, temp_iva, temp_tot As Decimal

        Dim chiave As String
        Dim valore, old_valore, new_valore As String

        chiave = CStr(Cod_IVA) & "|" & Aliquota

        'se ho già inserito quel codice IVA
        If hash_iva.Contains(chiave) Then

            old_valore = hash_iva.Item(chiave)

            'recupero i valori
            temp_imp = old_valore.Split("|")(0) 'imponibile
            temp_iva = old_valore.Split("|")(1) 'iva
            temp_tot = old_valore.Split("|")(2) 'importo

            'aggiungo i nuovi 
            temp_imp += Imponibile_Netto
            temp_iva += IVA
            temp_tot += Imponibile_Netto + IVA

            new_valore = CStr(temp_imp) & "|" & CStr(temp_iva) & "|" & CStr(temp_tot)

            'memorizzo i nuovi valori per quel codice iva
            hash_iva.Item(chiave) = new_valore

        Else
            'è la prima volta che si inserisce quel codice iva
            valore = CStr(Imponibile_Netto) & "|" & CStr(IVA) & "|" & CStr(Imponibile_Netto + IVA)
            hash_iva.Add(chiave, valore)
        End If


    End Sub


    '########################################################################################
    Private Function Ricava_HashIVA_da_DtIVA(ByVal DT_IVA As DataTable) As Hashtable

        Dim i, Cod_Iva As Integer
        Dim Aliquota As String
        Dim Imponibile, IVA As Decimal

        Dim Hash_IVA As New Hashtable

        If Not IsNothing(DT_IVA) Then

            For i = 0 To DT_IVA.Rows.Count - 1

                Cod_Iva = DT_IVA.Rows(i).Item("Cod_Iva")
                Aliquota = DT_IVA.Rows(i).Item("Aliquota")
                Imponibile = DT_IVA.Rows(i).Item("Imponibile")
                IVA = DT_IVA.Rows(i).Item("IVA")

                Riempi_Hash_IVA(Hash_IVA, Cod_Iva, Aliquota, Imponibile, IVA)

            Next

        End If

        Return Hash_IVA


    End Function


    '########################################################################################
    Private Function Ricava_HashIVA_da_DtProdotti(ByVal DT_Prodotti As DataTable) As Hashtable

        Dim i, Cod_Iva As Integer
        Dim Aliquota As String
        Dim Imponibile_Netto, IVA As Decimal

        Dim Hash_IVA As New Hashtable

        If Not IsNothing(DT_Prodotti) Then

            For i = 0 To DT_Prodotti.Rows.Count - 1

                Cod_Iva = DT_Prodotti.Rows(i).Item("Cod_Iva")
                Aliquota = DT_Prodotti.Rows(i).Item("Aliquota")
                Imponibile_Netto = DT_Prodotti.Rows(i).Item("Imponibile_Netto")
                IVA = DT_Prodotti.Rows(i).Item("IVA")

                Riempi_Hash_IVA(Hash_IVA, Cod_Iva, Aliquota, Imponibile_Netto, IVA)

            Next

        End If

        Return Hash_IVA


    End Function


    '########################################################################################
    Private Sub Ricava_RiepilogoDocumento_da_DtProdotti(ByVal DT_Prodotti As DataTable)

        Dim Imponibile_Lordo, Variazione, Imponibile_Netto, IVA, Importo As Decimal
        Dim i As Integer

        v_Tot_Imponibile_Lordo = 0
        v_Tot_Variazioni = 0
        v_Tot_Imponibile_Netto = 0
        v_Tot_Iva = 0
        v_Tot_Importo = 0

        If Not IsNothing(DT_Prodotti) Then

            If DT_Prodotti.Rows.Count <> 0 Then

                For i = 0 To DT_Prodotti.Rows.Count - 1

                    Imponibile_Lordo = DT_Prodotti.Rows(i).Item("Imponibile")
                    Variazione = DT_Prodotti.Rows(i).Item("Variazione")
                    Imponibile_Netto = DT_Prodotti.Rows(i).Item("Imponibile_Netto")
                    IVA = DT_Prodotti.Rows(i).Item("IVA")
                    Importo = Imponibile_Netto + IVA

                    v_Tot_Imponibile_Lordo += Imponibile_Lordo
                    v_Tot_Variazioni += Variazione
                    v_Tot_Imponibile_Netto += Imponibile_Netto
                    v_Tot_Iva += IVA
                    v_Tot_Importo += Importo

                Next

            End If

        End If

    End Sub




    '########################################################################################
    Private Sub CaricaGriglia_Dettagli()

        '----- Definizione delle variabili

        Dim Dt As New DataTable

        '----- Definisco la struttura del DataTable

        Dt.Columns.Add(New DataColumn("Chiave", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Codice", GetType(String)))
        Dt.Columns.Add(New DataColumn("Pendente", GetType(String)))
        Dt.Columns.Add(New DataColumn("Allegato", GetType(String)))
        Dt.Columns.Add(New DataColumn("Tipo", GetType(String)))

        Dt.Columns.Add(New DataColumn("Elem_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Pro_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Mat_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Cod_Progetto", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Fase_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Lotto", GetType(String)))
        Dt.Columns.Add(New DataColumn("Cal_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Udm_Cod", GetType(Integer)))

        Dt.Columns.Add(New DataColumn("Udm_Cod_Extra", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Qta_Extra", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("Prezzo_Effettivo", GetType(Decimal)))

        Dt.Columns.Add(New DataColumn("Descrizione", GetType(String)))
        Dt.Columns.Add(New DataColumn("Udm_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Qta", GetType(Decimal)))

        Dt.Columns.Add(New DataColumn("Prezzo_Unitario", GetType(String)))
        Dt.Columns.Add(New DataColumn("Prezzo_Unitario_Netto", GetType(String)))

        Dt.Columns.Add(New DataColumn("Cod_Variazione", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("Variazione_Perc", GetType(String)))
        Dt.Columns.Add(New DataColumn("Variazione", GetType(String)))

        Dt.Columns.Add(New DataColumn("Imponibile", GetType(String)))
        Dt.Columns.Add(New DataColumn("Imponibile_Netto", GetType(String)))
        Dt.Columns.Add(New DataColumn("Cod_Iva", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Aliquota", GetType(String)))
        Dt.Columns.Add(New DataColumn("IVA", GetType(String)))
        Dt.Columns.Add(New DataColumn("Importo", GetType(String)))

        Dt.Columns.Add(New DataColumn("Anno", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Ric_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Cod_Conto", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Conto", GetType(String)))

        Dt.Columns.Add(New DataColumn("Piva_Destinazione", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sa_Cod_Destinazione", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Id_Agenda_Destinazione", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Id_Mov_Destinazione", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Id_Mov_Det_Destinazione", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Id_Destinazione", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Destinazione", GetType(String)))

        Dt.Columns.Add(New DataColumn("Piva_Provenienza", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sa_Cod_Provenienza", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Id_Agenda_Provenienza", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Id_Mov_Provenienza", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Id_Mov_Det_Provenienza", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Id_Provenienza", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Provenienza", GetType(String)))

        Dt.Columns.Add(New DataColumn("Xml_Destinazione", GetType(String)))
        Dt.Columns.Add(New DataColumn("Xml_Provenienza", GetType(String)))

        Dt.Columns.Add(New DataColumn("Dettaglio_Destinazione", GetType(Object)))
        Dt.Columns.Add(New DataColumn("Dettaglio_Provenienza", GetType(Object)))

        'per controllo data fattura
        Dt.Columns.Add(New DataColumn("ValiditaInizio", GetType(String)))

        Dt.Columns.Add(New DataColumn("N", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("P2O5", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("K2O", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("Cu", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("Regolamento_Cod_Ferti", GetType(Integer)))

        '------------------------------------------------------

        Dim DtKeys(0) As DataColumn

        'Valorizzo le celle del vettore
        DtKeys(0) = Dt.Columns("Chiave")

        'Assegno il vettore delle chiavi al DataTable
        Dt.PrimaryKey = DtKeys

        '----- Associo il DataTable con la DataGrid

        DataGrid_Prodotti.DataSource = Dt
        Me.DataGrid_Prodotti.DataBind()

        Session("DT_Prodotti_nel_Doc") = Dt

    End Sub


    '########################################################################################
    Private Sub Inserisci_MovDettaglio_inGriglia(ByVal Pendente As Integer,
                                                 ByVal Allegato As String,
                                                 ByVal Tipo As String,
                                                 ByVal Elem_Cod As Integer,
                                                 ByVal Pro_Cod As Integer,
                                                 ByVal Mat_Cod As Integer,
                                                 ByVal Cod_Progetto As Integer,
                                                 ByVal Fase_Cod As Integer,
                                                 ByVal Lotto As String,
                                                 ByVal Cal_Cod As Integer,
                                                 ByVal Udm_Cod As Integer,
                                                 ByVal Udm_Cod_Extra As Integer,
                                                 ByVal Qta_Extra As Decimal,
                                                 ByVal Prezzo_Effettivo As Decimal,
                                                 ByVal Descrizione As String,
                                                 ByVal Udm_Des As String,
                                                 ByVal Qta As Decimal,
                                                 ByVal Prezzo_Unitario As Decimal,
                                                 ByVal Prezzo_Unitario_Netto As Decimal,
                                                 ByVal Variazione_Perc As Decimal,
                                                 ByVal Variazione As Decimal,
                                                 ByVal Imponibile As Decimal,
                                                 ByVal Imponibile_Netto As Decimal,
                                                 ByVal Cod_Iva As Integer,
                                                 ByVal Aliquota As String,
                                                 ByVal IVA As Decimal,
                                                 ByVal Anno As Integer,
                                                 ByVal Ric_Cod As Integer,
                                                 ByVal Cod_Conto As Integer,
                                                 ByVal Conto As String,
                                                 ByVal Piva_Destinazione As String,
                                                 ByVal Sa_Cod_Destinazione As Integer,
                                                 ByVal Id_Agenda_Destinazione As Integer,
                                                 ByVal Id_Mov_Destinazione As Integer,
                                                 ByVal Id_Mov_Det_Destinazione As Integer,
                                                 ByVal Id_Destinazione As Integer,
                                                 ByVal Destinazione As String,
                                                 ByVal Piva_Provenienza As String,
                                                 ByVal Sa_Cod_Provenienza As Integer,
                                                 ByVal Id_Agenda_Provenienza As Integer,
                                                 ByVal Id_Mov_Provenienza As Integer,
                                                 ByVal Id_Mov_Det_Provenienza As Integer,
                                                 ByVal Id_Provenienza As Integer,
                                                 ByVal Provenienza As String,
                                                 ByVal Xml_Destinazione As String,
                                                 ByVal Xml_Provenienza As String,
                                                 ByVal Dettaglio_Destinazione As Object,
                                                 ByVal Dettaglio_Provenienza As Object,
                                                 ByVal ValiditaInizio As String,
                                                 ByVal N As Decimal,
                                                 ByVal P2O5 As Decimal,
                                                 ByVal K2O As Decimal,
                                                 ByVal Cu As Decimal,
                                                 ByVal Regolamento_Cod_Ferti As Integer)


        '----- Dimensiono le variabili
        Dim DT As DataTable
        Dim Dr As DataRow
        'Dim IndiceRiga As Integer
        Dim ElementoPresente As Boolean
        'Dim Messaggio As String

        '----- Verifico la correttezza dei dati inseriti
        'If Qta = 0 Then
        '    Messaggio = "Errore nell'inserimento del dettaglio: " & Descrizione & " nel documento: quantità nulla!"
        '    Call messaggi.AgroMsgBox(Messaggio, Page)
        '    Exit Sub
        'End If


        '----- Verifico SE IL PRODOTTO non sia gia' presente nel datatable

        'Inizializzo
        ElementoPresente = False


        DT = Session("DT_Prodotti_nel_Doc")


        If Not IsNothing(DT) Then

            ''PERMETTO L'INSERIMENTO DELLO STESSO PRODOTTO PIù VOLTE
            ''
            ''Ciclo nelle righe del datatable
            'For IndiceRiga = 0 To DT.Rows.Count - 1

            '    If DT.Rows(IndiceRiga).Item("Piva_Destinazione") = Piva_Destinazione And _
            '       DT.Rows(IndiceRiga).Item("Sa_Cod_Destinazione") = Sa_Cod_Destinazione And _
            '       DT.Rows(IndiceRiga).Item("Id_Destinazione") = Id_Destinazione And _
            '       DT.Rows(IndiceRiga).Item("Piva_Provenienza") = Piva_Provenienza And _
            '       DT.Rows(IndiceRiga).Item("Sa_Cod_Provenienza") = Sa_Cod_Provenienza And _
            '       DT.Rows(IndiceRiga).Item("Id_Provenienza") = Id_Provenienza And _
            '       DT.Rows(IndiceRiga).Item("Elem_Cod") = Elem_Cod And _
            '       DT.Rows(IndiceRiga).Item("Pro_Cod") = Pro_Cod And _
            '       DT.Rows(IndiceRiga).Item("Mat_Cod") = Mat_Cod And _
            '       DT.Rows(IndiceRiga).Item("Cod_Progetto") = Cod_Progetto And _
            '       DT.Rows(IndiceRiga).Item("Fase_Cod") = Fase_Cod And _
            '       DT.Rows(IndiceRiga).Item("Lotto") = Lotto And _
            '       DT.Rows(IndiceRiga).Item("Cal_Cod") = Cal_Cod And _
            '       DT.Rows(IndiceRiga).Item("Udm_Cod") = Udm_Cod Then

            '        Select Case DT.Rows(IndiceRiga).Item("Elem_Cod")

            '            Case ALTRI_BENI, SERVIZI

            '                If DT.Rows(IndiceRiga).Item("Descrizione") = Descrizione Then
            '                    ElementoPresente = True
            '                    Exit For
            '                Else
            '                    ElementoPresente = False
            '                End If

            '            Case Else
            '                ElementoPresente = True
            '                Exit For
            '        End Select


            '    End If

            'Next

            ''Se esiste gia' allora esco
            'If ElementoPresente = True Then
            '    Messaggio = "Non e' consentito inserire più volte lo stesso dettaglio!"
            '    Call Messaggi.AgroMsgBox(Messaggio, Page, NomeForm:="aspnetForm", UpdatePanel:=upTabs_2)
            '    Exit Sub
            'End If


            '----- Inserisco il nuovo record
            Dim i As Integer
            Dim Max As Integer = 0

            For i = 0 To DT.Rows.Count - 1
                Dr = DT.Rows(i)
                If Max < Dr.Item("Chiave") Then
                    Max = Dr.Item("Chiave")
                End If
            Next

            'Creo una nuova riga
            Dr = DT.NewRow

            'Definisco i valori
            Dr.Item("Chiave") = Max + 1

            If Pro_Cod <> 0 Then
                Dr.Item("Codice") = "Gias" & Pro_Cod
            Else
                If Mat_Cod <> 0 Then
                    Dr.Item("Codice") = "Azi" & Mat_Cod
                Else
                    If Cod_Progetto <> 0 Then
                        Dr.Item("Codice") = "Zoo" & Cod_Progetto
                    Else
                        Dr.Item("Codice") = "---"
                    End If
                End If
            End If

            Dr.Item("Pendente") = Pendente
            Dr.Item("Allegato") = Allegato
            Dr.Item("Tipo") = Tipo
            Dr.Item("Elem_Cod") = Elem_Cod
            Dr.Item("Pro_Cod") = Pro_Cod
            Dr.Item("Mat_Cod") = Mat_Cod
            Dr.Item("Cod_Progetto") = Cod_Progetto
            Dr.Item("Fase_Cod") = Fase_Cod
            Dr.Item("Lotto") = Lotto
            Dr.Item("Cal_Cod") = Cal_Cod
            Dr.Item("Udm_Cod") = Udm_Cod

            Dr.Item("Udm_Cod_Extra") = Udm_Cod_Extra
            Dr.Item("Qta_Extra") = Qta_Extra
            Dr.Item("Prezzo_Effettivo") = Prezzo_Effettivo

            Dr.Item("Descrizione") = Descrizione
            Dr.Item("Udm_Des") = Udm_Des
            Dr.Item("Qta") = Qta

            Dr.Item("Prezzo_Unitario") = Prezzo_Unitario ' Format(Prezzo_Unitario, "#,###,##0.00")
            Dr.Item("Prezzo_Unitario_Netto") = Prezzo_Unitario_Netto ' Format(Prezzo_Unitario_Netto, "#,###,##0.00")

            Dr.Item("Cod_Variazione") = Variazione_Perc
            Dr.Item("Variazione_Perc") = CStr(Variazione_Perc) & "%"
            Dr.Item("Variazione") = Format(Variazione, "#,###,##0.00")

            Dr.Item("Imponibile") = Format(Imponibile, "#,###,##0.00")
            Dr.Item("Imponibile_Netto") = Format(Imponibile_Netto, "#,###,##0.00")
            Dr.Item("Cod_Iva") = Cod_Iva
            Dr.Item("Aliquota") = Aliquota
            Dr.Item("IVA") = Format(IVA, "#,###,##0.00")
            Dr.Item("Importo") = Format(CDbl(Imponibile_Netto + IVA), "#,###,##0.00")

            Dr.Item("Anno") = Anno
            Dr.Item("Ric_Cod") = Ric_Cod
            Dr.Item("Cod_Conto") = Cod_Conto
            Dr.Item("Conto") = Conto

            Dr.Item("N") = N
            Dr.Item("P2O5") = P2O5
            Dr.Item("K2O") = K2O
            Dr.Item("Cu") = Cu
            Dr.Item("Regolamento_Cod_Ferti") = Regolamento_Cod_Ferti

            Dr.Item("Piva_Destinazione") = Piva_Destinazione
            Dr.Item("Sa_Cod_Destinazione") = Sa_Cod_Destinazione
            Dr.Item("Id_Agenda_Destinazione") = Id_Agenda_Destinazione
            Dr.Item("Id_Mov_Destinazione") = Id_Mov_Destinazione
            Dr.Item("Id_Mov_Det_Destinazione") = Id_Mov_Det_Destinazione
            Dr.Item("Id_Destinazione") = Id_Destinazione
            Dr.Item("Destinazione") = Destinazione

            Dr.Item("Piva_Provenienza") = Piva_Provenienza
            Dr.Item("Sa_Cod_Provenienza") = Sa_Cod_Provenienza
            Dr.Item("Id_Agenda_Provenienza") = Id_Agenda_Provenienza
            Dr.Item("Id_Mov_Provenienza") = Id_Mov_Provenienza
            Dr.Item("Id_Mov_Det_Provenienza") = Id_Mov_Det_Provenienza
            Dr.Item("Id_Provenienza") = Id_Provenienza
            Dr.Item("Provenienza") = Provenienza

            If Xml_Destinazione = "" Then
                If Dettaglio_Destinazione IsNot Nothing Then
                    Xml_Destinazione = CreaXMLDettaglio_Da_Dettaglio(Dettaglio_Destinazione)
                End If
            End If
            Dr.Item("Xml_Destinazione") = Xml_Destinazione

            If Xml_Provenienza = "" Then
                If Dettaglio_Provenienza IsNot Nothing Then
                    Xml_Provenienza = CreaXMLDettaglio_Da_Dettaglio(Dettaglio_Provenienza)
                End If
            End If
            Dr.Item("Xml_Provenienza") = Xml_Provenienza




            If Dettaglio_Destinazione IsNot Nothing Then
                'arrivo da lettura oggetto agenda
                Dr.Item("Dettaglio_Destinazione") = Dettaglio_Destinazione
            Else
                'arrivo dalle altre pagine (la stringa è salvata su DB)
                If x_XML_Destinazione <> "" Then
                    Dim Dettaglio As Movimento_Dettaglio = CreaDettaglio_Da_XMLDettaglio(x_XML_Destinazione)
                    Dr.Item("Dettaglio_Destinazione") = Dettaglio
                End If
            End If


            If Dettaglio_Provenienza IsNot Nothing Then
                'arrivo da lettura oggetto agenda
                Dr.Item("Dettaglio_Provenienza") = Dettaglio_Provenienza
            Else
                'arrivo dalle altre pagine (la stringa è salvata su DB)
                If Xml_Provenienza <> "" Then
                    Dim Dettaglio As Movimento_Dettaglio = CreaDettaglio_Da_XMLDettaglio(Xml_Provenienza)
                    Dr.Item("Dettaglio_Provenienza") = Dettaglio
                End If
            End If

            Dr.Item("ValiditaInizio") = ValiditaInizio

            'pulsante modifica
            'pulsante elimina


            'Associo alla tabella la nuova riga creata
            DT.Rows.Add(Dr)


            Session("DT_Prodotti_nel_Doc") = DT


        End If



    End Sub

    Private Function CreaDettaglio_Da_XMLDettaglio(ByVal str_Movimento_Dettaglio As String) As Movimento_Dettaglio

        Dim XmlDoc As New XmlDocument
        Dim XML_MovimentoDettaglio As XmlElement
        Dim XML_MovimentoDestinazione As XmlElement
        Dim XML_MovRiferimento2 As XmlElement
        Dim XML_MovimentoTecnico As XmlElement

        Dim Movimento_Dettaglio As New Movimento_Dettaglio
        Movimento_Dettaglio.Movimenti_Destinazioni = New List(Of Movimento_Destinazione)
        Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti = New List(Of Movimento_Dettaglio_Riferimento)
        Movimento_Dettaglio.Movimenti_Dettagli_Tecnici = New List(Of Movimento_Dettaglio_Tecnico)

        Dim Movimento_Destinazione As Movimento_Destinazione
        Dim Movimento_Dettaglio_Riferimento As Movimento_Dettaglio_Riferimento
        Dim Movimento_Dettaglio_Tecnico As Movimento_Dettaglio_Tecnico

        Dim objIVA As New AgronicaCoreMetaSchemaDAL.IVA_Aliquote_R

        If str_Movimento_Dettaglio <> "" Then

            XmlDoc.LoadXml(str_Movimento_Dettaglio)

            XML_MovimentoDettaglio = XmlDoc.SelectSingleNode("//Movimento_Dettaglio")

            If Not IsNothing(XML_MovimentoDettaglio) Then

                XML_MovimentoDestinazione = XML_MovimentoDettaglio.SelectSingleNode("Movimento_Destinazione")

                XML_MovRiferimento2 = XML_MovimentoDettaglio.SelectSingleNode("Movimento_Riferimento2")

                XML_MovimentoTecnico = XML_MovimentoDettaglio.SelectSingleNode("Movimento_Dettaglio_Tecnico_2")

                '------------------------
                Movimento_Dettaglio.Piva = CStr(XML_MovimentoDettaglio.GetAttribute("piva"))
                Movimento_Dettaglio.Sa_Cod = CInt(XML_MovimentoDettaglio.GetAttribute("sa_cod"))
                Movimento_Dettaglio.Id_Agenda = CInt(XML_MovimentoDettaglio.GetAttribute("id_agenda"))
                Movimento_Dettaglio.Id_Mov = CInt(XML_MovimentoDettaglio.GetAttribute("id_mov"))
                Movimento_Dettaglio.Id_Mov_Det = CInt(XML_MovimentoDettaglio.GetAttribute("id_mov_det"))



                '------------------------

                'x_Mov_Det_Des = XML_MovimentoDettaglio.GetAttribute("mov_det_des")

                'chiave del prodotto
                Movimento_Dettaglio.Elem_Cod = XML_MovimentoDettaglio.GetAttribute("elem_cod")
                Movimento_Dettaglio.Pro_Cod = XML_MovimentoDettaglio.GetAttribute("pro_cod")
                Movimento_Dettaglio.Mat_Cod = XML_MovimentoDettaglio.GetAttribute("mat_cod")
                Movimento_Dettaglio.Cod_Progetto = XML_MovimentoDettaglio.GetAttribute("cod_progetto")
                Movimento_Dettaglio.Fase_Cod = XML_MovimentoDettaglio.GetAttribute("fase_cod")
                Movimento_Dettaglio.Lotto = XML_MovimentoDettaglio.GetAttribute("lotto")
                Movimento_Dettaglio.Cal_Cod = XML_MovimentoDettaglio.GetAttribute("cal_cod")
                Movimento_Dettaglio.Udm_Cod = XML_MovimentoDettaglio.GetAttribute("udm_cod")
                Movimento_Dettaglio.Udm_Cod_Extra = XML_MovimentoDettaglio.GetAttribute("udm_cod_extra")
                Movimento_Dettaglio.Qta = XML_MovimentoDettaglio.GetAttribute("qta")
                Movimento_Dettaglio.Qta_Extra = XML_MovimentoDettaglio.GetAttribute("qta_extra")

                Movimento_Dettaglio.Prezzo_Effettivo = XML_MovimentoDettaglio.GetAttribute("prezzo_effettivo")
                Movimento_Dettaglio.Prezzo_Unitario = CDbl(XML_MovimentoDettaglio.GetAttribute("prezzo_unitario"))
                Movimento_Dettaglio.Prezzo_Unitario_Netto = CDbl(XML_MovimentoDettaglio.GetAttribute("prezzo_unitario_netto"))

                Movimento_Dettaglio.Sconto = XML_MovimentoDettaglio.GetAttribute("sconto")

                Movimento_Dettaglio.Imponibile = XML_MovimentoDettaglio.GetAttribute("imponibile")
                Movimento_Dettaglio.Imponibile_Netto = XML_MovimentoDettaglio.GetAttribute("imponibile_netto")

                Movimento_Dettaglio.Cod_Iva = XML_MovimentoDettaglio.GetAttribute("cod_iva")
                x_Aliquota = objIVA.Aliquota_from_CodIVA(x_Cod_IVA, "", objParametri_Server)
                Movimento_Dettaglio.Iva = XML_MovimentoDettaglio.GetAttribute("iva")

                Movimento_Dettaglio.Anno = XML_MovimentoDettaglio.GetAttribute("anno")
                Movimento_Dettaglio.Ric_Cod = XML_MovimentoDettaglio.GetAttribute("ric_cod")
                Movimento_Dettaglio.Cod_Conto = XML_MovimentoDettaglio.GetAttribute("cod_conto")

                Movimento_Dettaglio.Jolly_Int = XML_MovimentoDettaglio.GetAttribute("jolly_int")
                Movimento_Dettaglio.Contabilizzato = XML_MovimentoDettaglio.GetAttribute("contabilizzato")
                Movimento_Dettaglio.Pendente = XML_MovimentoDettaglio.GetAttribute("pendente")
                Movimento_Dettaglio.Extra_Str = XML_MovimentoDettaglio.GetAttribute("extra_str")
                Movimento_Dettaglio.Extra_Int = XML_MovimentoDettaglio.GetAttribute("extra_int")
                Movimento_Dettaglio.Extra_Date = XML_MovimentoDettaglio.GetAttribute("extra_date")

                'If Not IsDBNull(XML_MovimentoDettaglio.GetAttribute("validita_inizio")) AndAlso
                '    Not IsNothing(XML_MovimentoDettaglio.GetAttribute("validita_inizio")) AndAlso
                '    IsDate(XML_MovimentoDettaglio.GetAttribute("validita_inizio")) Then
                '    Movimento_Dettaglio.ValiditaInizio = XML_MovimentoDettaglio.GetAttribute("validita_inizio")
                'Else
                '    Movimento_Dettaglio.ValiditaInizio = AGRODATAINIZIO
                'End If
                Movimento_Dettaglio.Validita_Inizio = XML_MovimentoDettaglio.GetAttribute("validita_inizio")
                Movimento_Dettaglio.Validita_Fine = XML_MovimentoDettaglio.GetAttribute("validita_fine")

                Movimento_Dettaglio.BaseCode = XML_MovimentoDettaglio.GetAttribute("basecode")
                Movimento_Dettaglio.TopCode = XML_MovimentoDettaglio.GetAttribute("topcode")

                If XML_MovimentoDettaglio.HasAttribute("lav_cod") Then
                    Movimento_Dettaglio.Lav_Cod = XML_MovimentoDettaglio.GetAttribute("lav_cod")
                Else
                    Movimento_Dettaglio.Lav_Cod = QS_LavCod
                End If

                Movimento_Dettaglio.Cau_Mov = XML_MovimentoDettaglio.GetAttribute("cau_mov")


                '##########################################################

                If Not IsNothing(XML_MovimentoDestinazione) Then

                    Movimento_Destinazione = New Movimento_Destinazione

                    Movimento_Destinazione.Piva = XML_MovimentoDestinazione.GetAttribute("piva")
                    Movimento_Destinazione.Sa_Cod = XML_MovimentoDestinazione.GetAttribute("sa_cod")
                    Movimento_Destinazione.Id_Agenda = XML_MovimentoDestinazione.GetAttribute("id_agenda")
                    Movimento_Destinazione.Id_Mov = XML_MovimentoDestinazione.GetAttribute("id_mov")
                    Movimento_Destinazione.Id_Mov_Det = XML_MovimentoDestinazione.GetAttribute("id_mov_det")
                    Movimento_Destinazione.Appezza = XML_MovimentoDestinazione.GetAttribute("appezza")
                    Movimento_Destinazione.Id_Destinazione = CInt(XML_MovimentoDestinazione.GetAttribute("id_destinazione"))
                    Movimento_Destinazione.Tipo = XML_MovimentoDestinazione.GetAttribute("tipo_destinazione")
                    Movimento_Destinazione.Qta = XML_MovimentoDestinazione.GetAttribute("qta")
                    Movimento_Destinazione.Qta2 = XML_MovimentoDestinazione.GetAttribute("qta2")
                    Movimento_Destinazione.BaseCode = XML_MovimentoDestinazione.GetAttribute("basecode")
                    Movimento_Destinazione.TopCode = XML_MovimentoDestinazione.GetAttribute("topcode")

                    Movimento_Dettaglio.Movimenti_Destinazioni.Add(Movimento_Destinazione)

                End If

                '##########################################################

                If Not IsNothing(XML_MovRiferimento2) Then

                    Movimento_Dettaglio_Riferimento = New Movimento_Dettaglio_Riferimento

                    Movimento_Dettaglio_Riferimento.Piva = XML_MovRiferimento2.GetAttribute("piva")
                    Movimento_Dettaglio_Riferimento.Sa_Cod = XML_MovRiferimento2.GetAttribute("sa_cod")
                    Movimento_Dettaglio_Riferimento.Id_Agenda = XML_MovRiferimento2.GetAttribute("id_agenda")
                    Movimento_Dettaglio_Riferimento.Id_Mov = XML_MovRiferimento2.GetAttribute("id_mov")
                    Movimento_Dettaglio_Riferimento.Id_Mov_Det = XML_MovRiferimento2.GetAttribute("id_mov_det")
                    Movimento_Dettaglio_Riferimento.Lav_Cod = XML_MovRiferimento2.GetAttribute("lav_cod")
                    Movimento_Dettaglio_Riferimento.Cau_Mov = XML_MovRiferimento2.GetAttribute("cau_mov")
                    Movimento_Dettaglio_Riferimento.Piva_Rif = XML_MovRiferimento2.GetAttribute("piva_rif")
                    Movimento_Dettaglio_Riferimento.Sa_Cod_Rif = XML_MovRiferimento2.GetAttribute("sa_cod_rif")
                    Movimento_Dettaglio_Riferimento.Id_Agenda_Rif = XML_MovRiferimento2.GetAttribute("id_agenda_rif")
                    Movimento_Dettaglio_Riferimento.Id_Mov_Rif = XML_MovRiferimento2.GetAttribute("id_mov_rif")
                    Movimento_Dettaglio_Riferimento.Id_Mov_Det_Rif = XML_MovRiferimento2.GetAttribute("id_mov_det_rif")
                    Movimento_Dettaglio_Riferimento.Lav_Cod_Rif = XML_MovRiferimento2.GetAttribute("lav_cod_rif")
                    Movimento_Dettaglio_Riferimento.Cau_Mov_Rif = XML_MovRiferimento2.GetAttribute("cau_mov_rif")

                    Movimento_Dettaglio_Riferimento.Qta = XML_MovRiferimento2.GetAttribute("qta")

                    Movimento_Dettaglio_Riferimento.BaseCode = XML_MovRiferimento2.GetAttribute("basecode")
                    Movimento_Dettaglio_Riferimento.TopCode = XML_MovRiferimento2.GetAttribute("topcode")

                    Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti.Add(Movimento_Dettaglio_Riferimento)

                End If

                '##########################################################

                If Not IsNothing(XML_MovimentoTecnico) Then

                    Movimento_Dettaglio_Tecnico = New Movimento_Dettaglio_Tecnico With {
                        .Piva = XML_MovimentoTecnico.GetAttribute("piva"),
                        .Sa_Cod = XML_MovimentoTecnico.GetAttribute("sa_cod"),
                        .Id_Agenda = XML_MovimentoTecnico.GetAttribute("id_agenda"),
                        .Id_Mov = XML_MovimentoTecnico.GetAttribute("id_mov"),
                        .Id_Mov_Det = XML_MovimentoTecnico.GetAttribute("id_mov_det"),
                        .N = XML_MovimentoTecnico.GetAttribute("n"),
                        .P = XML_MovimentoTecnico.GetAttribute("p"),
                        .K = XML_MovimentoTecnico.GetAttribute("k"),
                        .Cu = XML_MovimentoTecnico.GetAttribute("cu"),
                        .Extra_Int = XML_MovimentoTecnico.GetAttribute("extra_int"),
                        .Data = XML_MovimentoTecnico.GetAttribute("validita_inizio"),
                        .BaseCode = XML_MovimentoTecnico.GetAttribute("basecode"),
                        .TopCode = XML_MovimentoTecnico.GetAttribute("topcode")
                    }

                    Movimento_Dettaglio.Movimenti_Dettagli_Tecnici.Add(Movimento_Dettaglio_Tecnico)

                End If

            End If

        End If

        Return Movimento_Dettaglio


    End Function

    Private Function CreaXMLDettaglio_Da_Dettaglio(ByVal Movimento_Dettaglio As Movimento_Dettaglio) As String

        Dim str_Movimento_Dettaglio As String = ""
        Dim XmlDoc As New XmlDocument

        Dim XmlMovimento_Dettaglio As XmlElement
        Dim XmlMov_Destinazione As XmlElement
        Dim XmlMov_Tecnico As XmlElement
        Dim XmlMov_Det_Riferimento As XmlElement
        Dim Segno As String

        Select Case Movimento_Dettaglio.Cau_Mov
            Case CAU_CARICO
                Segno = "-" 'acquisto - fattura ricevuta
            Case CAU_CONFERIMENTO
                Segno = "-" 'costo: ricevi da un socio
            Case CAU_CONFERIMENTO_DIVERSI
                Segno = "+" 'ricavo: vendi a esterni
            Case CAU_ACCETTAZIONE_BENI
                Segno = "-"             '?
            Case CAU_SCARICO
                Segno = "+" ' vendita - fattura emessa
            Case CAU_TRASFERIMENTO
                Segno = "+" 'Fittizio    ?
            Case Else
                Segno = "+" 'default
        End Select

        XmlMovimento_Dettaglio = XmlDoc.CreateElement("Movimento_Dettaglio")

        XmlMovimento_Dettaglio.SetAttribute("TipoOperazioneDB", Qs_Operazione)

        XmlMovimento_Dettaglio.SetAttribute("piva", CStr(Movimento_Dettaglio.Piva))
        XmlMovimento_Dettaglio.SetAttribute("sa_cod", CStr(Movimento_Dettaglio.Sa_Cod))
        XmlMovimento_Dettaglio.SetAttribute("id_agenda", CStr(Movimento_Dettaglio.Id_Agenda))
        XmlMovimento_Dettaglio.SetAttribute("id_mov", CStr(Movimento_Dettaglio.Id_Mov))
        XmlMovimento_Dettaglio.SetAttribute("id_mov_det", CStr(Movimento_Dettaglio.Id_Mov_Det))

        '-------------------------------------------------------------------

        'XmlMovimento_Dettaglio.SetAttribute("mov_det_des", Descrizione)

        XmlMovimento_Dettaglio.SetAttribute("elem_cod", CStr(Movimento_Dettaglio.Elem_Cod))
        XmlMovimento_Dettaglio.SetAttribute("pro_cod", CStr(Movimento_Dettaglio.Pro_Cod))
        XmlMovimento_Dettaglio.SetAttribute("mat_cod", CStr(Movimento_Dettaglio.Mat_Cod))
        XmlMovimento_Dettaglio.SetAttribute("cod_progetto", CStr(Movimento_Dettaglio.Cod_Progetto))
        XmlMovimento_Dettaglio.SetAttribute("fase_cod", CStr(Movimento_Dettaglio.Fase_Cod))
        XmlMovimento_Dettaglio.SetAttribute("lotto", Movimento_Dettaglio.Lotto)
        XmlMovimento_Dettaglio.SetAttribute("cal_cod", CStr(Movimento_Dettaglio.Cal_Cod))
        XmlMovimento_Dettaglio.SetAttribute("udm_cod", CStr(Movimento_Dettaglio.Udm_Cod))

        XmlMovimento_Dettaglio.SetAttribute("qta", CStr(Movimento_Dettaglio.Qta))

        XmlMovimento_Dettaglio.SetAttribute("udm_cod_extra", CStr(Movimento_Dettaglio.Udm_Cod_Extra))
        XmlMovimento_Dettaglio.SetAttribute("qta_extra", CStr(Movimento_Dettaglio.Qta_Extra))

        XmlMovimento_Dettaglio.SetAttribute("prezzo_unitario", CStr(Movimento_Dettaglio.Prezzo_Unitario))
        XmlMovimento_Dettaglio.SetAttribute("prezzo_unitario_netto", CStr(Movimento_Dettaglio.Prezzo_Unitario_Netto))

        'XmlMovimento_Dettaglio.SetAttribute("imponibile", CStr(Imponibile))
        'XmlMovimento_Dettaglio.SetAttribute("imponibile_netto", CStr(Imponibile_Netto))
        XmlMovimento_Dettaglio.SetAttribute("imponibile", Movimento_Dettaglio.Imponibile)
        XmlMovimento_Dettaglio.SetAttribute("imponibile_netto", Movimento_Dettaglio.Imponibile_Netto)
        XmlMovimento_Dettaglio.SetAttribute("cod_iva", CStr(Movimento_Dettaglio.Cod_Iva))
        XmlMovimento_Dettaglio.SetAttribute("iva", Movimento_Dettaglio.Iva)

        XmlMovimento_Dettaglio.SetAttribute("sconto", CStr(Movimento_Dettaglio.Sconto))
        XmlMovimento_Dettaglio.SetAttribute("prezzo_effettivo", CStr(Movimento_Dettaglio.Prezzo_Effettivo))

        '=============================================================================================
        'Attributi per l'aggiornamento Piano dei Conti
        '---------------------------------------------------------------------------------------------
        XmlMovimento_Dettaglio.SetAttribute("anno", CStr(Movimento_Dettaglio.Anno))
        XmlMovimento_Dettaglio.SetAttribute("ric_cod", CStr(Movimento_Dettaglio.Ric_Cod))
        XmlMovimento_Dettaglio.SetAttribute("cod_conto", CStr(Movimento_Dettaglio.Cod_Conto))

        '---------------------------------------------------------------------------------------------

        XmlMovimento_Dettaglio.SetAttribute("jolly_int", CStr(Movimento_Dettaglio.Jolly_Int))
        XmlMovimento_Dettaglio.SetAttribute("contabilizzato", CStr(Movimento_Dettaglio.Contabilizzato))
        XmlMovimento_Dettaglio.SetAttribute("pendente", CStr(Movimento_Dettaglio.Pendente))

        XmlMovimento_Dettaglio.SetAttribute("extra_str", Movimento_Dettaglio.Extra_Str)
        XmlMovimento_Dettaglio.SetAttribute("extra_int", CStr(Movimento_Dettaglio.Extra_Int))
        XmlMovimento_Dettaglio.SetAttribute("extra_date", CDate(Movimento_Dettaglio.Extra_Date))

        XmlMovimento_Dettaglio.SetAttribute("validita_inizio", Movimento_Dettaglio.Validita_Inizio)
        XmlMovimento_Dettaglio.SetAttribute("validita_fine", Movimento_Dettaglio.Validita_Fine)
        XmlMovimento_Dettaglio.SetAttribute("basecode", CStr(Movimento_Dettaglio.BaseCode))
        XmlMovimento_Dettaglio.SetAttribute("topcode", CStr(Movimento_Dettaglio.TopCode))

        '=============================================================================================

        XmlMovimento_Dettaglio.SetAttribute("lav_cod", Movimento_Dettaglio.Lav_Cod) 'Nota: Utile in FormFattura
        XmlMovimento_Dettaglio.SetAttribute("cau_mov", Movimento_Dettaglio.Cau_Mov)




        '--------------------------------------------------------------------------------------------------------------




        '==============================================================================================================
        '==============================================================================================================
        '======================== MOV DESTINAZIONE =================================================
        '==============================================================================================================
        '==============================================================================================================

        If Movimento_Dettaglio.Movimenti_Destinazioni IsNot Nothing AndAlso Movimento_Dettaglio.Movimenti_Destinazioni.Count > 0 Then

            XmlMov_Destinazione = XmlDoc.CreateElement("Movimento_Destinazione")

            'XmlMov_Destinazione.SetAttribute("TipoOperazioneDB", Operazione)
            XmlMov_Destinazione.SetAttribute("piva", Movimento_Dettaglio.Movimenti_Destinazioni(0).Piva)
            XmlMov_Destinazione.SetAttribute("sa_cod", Movimento_Dettaglio.Movimenti_Destinazioni(0).Sa_Cod)
            XmlMov_Destinazione.SetAttribute("id_agenda", Movimento_Dettaglio.Movimenti_Destinazioni(0).Id_Agenda) 'mId_Agenda
            XmlMov_Destinazione.SetAttribute("id_mov", Movimento_Dettaglio.Movimenti_Destinazioni(0).Id_Mov)
            XmlMov_Destinazione.SetAttribute("id_mov_det", Movimento_Dettaglio.Movimenti_Destinazioni(0).Id_Mov_Det)
            XmlMov_Destinazione.SetAttribute("appezza", Movimento_Dettaglio.Movimenti_Destinazioni(0).Appezza) 'Carico/Scarico in Magazzino
            XmlMov_Destinazione.SetAttribute("id_destinazione", Movimento_Dettaglio.Movimenti_Destinazioni(0).Id_Destinazione)
            XmlMov_Destinazione.SetAttribute("tipo_destinazione", Movimento_Dettaglio.Movimenti_Destinazioni(0).Tipo)
            XmlMov_Destinazione.SetAttribute("qta", Movimento_Dettaglio.Movimenti_Destinazioni(0).Qta)
            XmlMov_Destinazione.SetAttribute("qta2", Movimento_Dettaglio.Movimenti_Destinazioni(0).Qta2)
            XmlMov_Destinazione.SetAttribute(LCase("Tipo_Scorta"), CStr(0))
            XmlMov_Destinazione.SetAttribute(LCase("Scorta_Min"), CStr(0))
            XmlMov_Destinazione.SetAttribute("validita_inizio", Movimento_Dettaglio.Movimenti_Destinazioni(0).Data)
            XmlMov_Destinazione.SetAttribute("validita_fine", "31/12/2100")
            XmlMov_Destinazione.SetAttribute("basecode", Movimento_Dettaglio.Movimenti_Destinazioni(0).BaseCode)
            XmlMov_Destinazione.SetAttribute("topcode", Movimento_Dettaglio.Movimenti_Destinazioni(0).TopCode)

            XmlMovimento_Dettaglio.AppendChild(XmlMov_Destinazione)


        End If

        '==============================================================================================================
        '==============================================================================================================
        '======================== MOV DETTAGLIO TECNICO =================================================
        '==============================================================================================================
        '==============================================================================================================

        If Movimento_Dettaglio.Movimenti_Dettagli_Tecnici IsNot Nothing AndAlso Movimento_Dettaglio.Movimenti_Dettagli_Tecnici.Count > 0 Then

            XmlMov_Tecnico = XmlDoc.CreateElement("Movimento_Dettaglio_Tecnico_2")

            With XmlMov_Tecnico
                '.SetAttribute("TipoOperazioneDB", Operazione)
                .SetAttribute("piva", Movimento_Dettaglio.Movimenti_Dettagli_Tecnici(0).Piva)
                .SetAttribute("sa_cod", Movimento_Dettaglio.Movimenti_Dettagli_Tecnici(0).Sa_Cod)
                .SetAttribute("id_agenda", Movimento_Dettaglio.Movimenti_Dettagli_Tecnici(0).Id_Agenda)
                .SetAttribute("id_mov", Movimento_Dettaglio.Movimenti_Dettagli_Tecnici(0).Id_Mov)
                .SetAttribute("id_mov_det", Movimento_Dettaglio.Movimenti_Dettagli_Tecnici(0).Id_Mov_Det)
                .SetAttribute("n", Movimento_Dettaglio.Movimenti_Dettagli_Tecnici(0).N)
                .SetAttribute("p", Movimento_Dettaglio.Movimenti_Dettagli_Tecnici(0).P)
                .SetAttribute("k", Movimento_Dettaglio.Movimenti_Dettagli_Tecnici(0).K)
                .SetAttribute("cu", Movimento_Dettaglio.Movimenti_Dettagli_Tecnici(0).Cu)
                .SetAttribute("extra_int", Movimento_Dettaglio.Movimenti_Dettagli_Tecnici(0).Extra_Int)
                .SetAttribute("validita_inizio", Movimento_Dettaglio.Movimenti_Dettagli_Tecnici(0).Data)
                .SetAttribute("validita_fine", "31/12/2100")
                .SetAttribute("basecode", Movimento_Dettaglio.Movimenti_Dettagli_Tecnici(0).BaseCode)
                .SetAttribute("topcode", Movimento_Dettaglio.Movimenti_Dettagli_Tecnici(0).TopCode)
            End With

            XmlMovimento_Dettaglio.AppendChild(XmlMov_Tecnico)


        End If


        If Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti IsNot Nothing AndAlso Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti.Count > 0 Then

            'Se il documento è allegato i dati sulle destinazioni sono riferite all'allegato
            'Inserimento del riferimento

            XmlMov_Det_Riferimento = XmlDoc.CreateElement("Movimento_Riferimento2")
            'XmlMov_Det_Riferimento.SetAttribute("TipoOperazioneDB", Operazione)
            XmlMov_Det_Riferimento.SetAttribute("piva", Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(0).Piva)
            XmlMov_Det_Riferimento.SetAttribute("sa_cod", Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(0).Sa_Cod)
            XmlMov_Det_Riferimento.SetAttribute("lav_cod", Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(0).Lav_Cod)
            XmlMov_Det_Riferimento.SetAttribute("cau_mov", Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(0).Cau_Mov)
            XmlMov_Det_Riferimento.SetAttribute("id_agenda", Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(0).Id_Agenda) 'Da Definire
            XmlMov_Det_Riferimento.SetAttribute("id_mov", Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(0).Id_Mov) 'Da Definire
            XmlMov_Det_Riferimento.SetAttribute("id_mov_det", Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(0).Id_Mov_Det) 'Da Definire
            XmlMov_Det_Riferimento.SetAttribute("piva_rif", Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(0).Piva_Rif)
            XmlMov_Det_Riferimento.SetAttribute("sa_cod_rif", Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(0).Sa_Cod_Rif)
            XmlMov_Det_Riferimento.SetAttribute("lav_cod_rif", Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(0).Lav_Cod_Rif)
            XmlMov_Det_Riferimento.SetAttribute("cau_mov_rif", Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(0).Cau_Mov_Rif)
            XmlMov_Det_Riferimento.SetAttribute("id_agenda_rif", Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(0).Id_Agenda_Rif)
            XmlMov_Det_Riferimento.SetAttribute("id_mov_rif", Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(0).Id_Mov_Rif)
            XmlMov_Det_Riferimento.SetAttribute("id_mov_det_rif", Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(0).Id_Mov_Det_Rif)
            XmlMov_Det_Riferimento.SetAttribute("qta", Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(0).Qta)
            XmlMov_Det_Riferimento.SetAttribute("validita_inizio", Movimento_Dettaglio.Data)
            XmlMov_Det_Riferimento.SetAttribute("validita_fine", Format(AGRODATAFINE, "dd/mm/yyyy"))

            XmlMovimento_Dettaglio.AppendChild(XmlMov_Det_Riferimento)

        End If

        str_Movimento_Dettaglio = XmlMovimento_Dettaglio.OuterXml

        Return str_Movimento_Dettaglio


    End Function


    '####################################################################################################################
    Private Function Verifica_Dettaglio_Allegato(ByVal Lav_Cod_DDT As Integer,
                                                    ByVal Piva_DDT As String,
                                                    ByVal Sa_Cod_DDT As Integer,
                                                    ByVal Id_Agenda_DDT As Integer,
                                                    ByVal Id_Mov_DDT As Integer,
                                                    ByVal Id_Mov_Det_DDT As Integer) As Boolean

        Dim DT As DataTable
        Dim i As Integer
        Dim Flag_AllegatoPresente As Boolean = False

        Dim str_COL_PIVA As String = ""
        Dim str_COL_ID_AGENDA As String = ""
        Dim str_COL_ID_MOV As String = ""
        Dim str_COL_ID_MOV_DET As String = ""

        DT = Session("DT_Prodotti_nel_Doc")

        If Not IsNothing(DT) Then

            Select Case Lav_Cod_DDT

                Case LAVCOD_BOLLA_EMESSA, LAVCOD_DDT_CONTABILIZZATO_EMESSO

                    str_COL_PIVA = str_COL_PIVA_DEST
                    str_COL_ID_AGENDA = str_COL_ID_AGENDA_DEST
                    str_COL_ID_MOV = str_COL_ID_MOV_DEST
                    str_COL_ID_MOV_DET = str_COL_ID_MOV_DET_DEST

                Case LAVCOD_BOLLA_RICEVUTA

                    str_COL_PIVA = str_COL_PIVA_PROV
                    str_COL_ID_AGENDA = str_COL_ID_AGENDA_PROV
                    str_COL_ID_MOV = str_COL_ID_MOV_PROV
                    str_COL_ID_MOV_DET = str_COL_ID_MOV_DET_PROV

            End Select

            'Ciclo nelle righe del datatable
            For i = 0 To DT.Rows.Count - 1

                If DT.Rows(i).Item(str_COL_PIVA) = Piva_DDT AndAlso
                   DT.Rows(i).Item(str_COL_ID_AGENDA) = Id_Agenda_DDT AndAlso
                   DT.Rows(i).Item(str_COL_ID_MOV) = Id_Mov_DDT AndAlso
                   DT.Rows(i).Item(str_COL_ID_MOV_DET) = Id_Mov_Det_DDT Then

                    Flag_AllegatoPresente = True

                    Exit For

                End If

            Next

        End If

        Return Flag_AllegatoPresente

    End Function



    '####################################################################################################################
    Private Sub DataGrid_Prodotti_ItemCommand(ByVal source As System.Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles DataGrid_Prodotti.ItemCommand


        If ViewState("UtenteAbilitatoModifica") = True Then

            Dim IndiceRigaGriglia As Integer
            Dim Chiave As Integer

            Dim Dt As DataTable
            Dim Dr As DataRow

            'Recupero l'indice di riga del datagrid
            IndiceRigaGriglia = e.Item.ItemIndex

            Select Case e.CommandName

                Case "Cancella"

                    Chiave = CInt(DataGrid_Prodotti.Items(IndiceRigaGriglia).Cells(COL_CHIAVE).Text)

                    Dt = Session("DT_Prodotti_nel_Doc")

                    'Trovo la riga da cancellare tramite la chiave
                    Dr = Dt.Rows.Find(Chiave)

                    'Elimino la riga
                    Dr.Delete()

                    'Associo il DataTable con la DataGrid
                    Me.DataGrid_Prodotti.DataSource = Dt
                    Me.DataGrid_Prodotti.DataBind()

                    Imposta_Pannelli("DC")

                    Session("DT_Prodotti_nel_Doc") = Dt

                    If Not IsNothing(Dt) AndAlso Dt.Rows.Count = 0 Then
                        Me.Cmb_Contatti.Enabled = True
                    End If


                    '----------------------------------------

                    'Va aggiornato il riepilogo dell'IVA

                    Session("DT_IVA_nel_Doc") = Nothing

                    Dim HashIVA As New Hashtable

                    HashIVA = Ricava_HashIVA_da_DtProdotti(Dt)

                    CaricaGriglia_IVA(HashIVA)

                    'Session("DT_IVA_nel_Doc") viene aggiornato dentro alla funzione

                    '----------------------------------------

                    Ricava_RiepilogoDocumento_da_DtProdotti(Dt)

                    Call Visualizza_RiepilogoDocumento()

                    '===================================================================

                Case "Modifica"

                    Dim Tipo As String = ""
                    Dim vet_XML_To_FormProdotto(2) As String

                    Tipo = DataGrid_Prodotti.Items(IndiceRigaGriglia).Cells(COL_TIPO).Text

                    Select Case Tipo

                        Case CAU_ANIMALE

                            Messaggi.AgroMsgBox("Non è consentita la modifica di Consistenze Zootecniche." & vbCrLf &
                            "In caso di errori, cancellare la consistenza e reinserirla correttamente.", Page)


                        Case CAU_MAGAZZINO

                            Dim CaricoScarico As String = ""
                            Dim ChiaveAlbero, Cod_Contatto, Ragsoc_Contatto As String

                            Call Albero.ChiaveAlbero_Codifica(ChiaveAlbero,
                                      enum_TipoNodo.x_GiacenzeMagazzino,
                                       Qs_Piva, Qs_SaCod, , , , , , , , , , , , , , , , , , )

                            'recupero settupla che identifica il prodotto

                            Chiave = CInt(DataGrid_Prodotti.Items(IndiceRigaGriglia).Cells(COL_CHIAVE).Text)


                            Select Case QS_LavCod

                                Case LAVCOD_FATTURA_EMESSA

                                    CaricoScarico = "S"

                                    vet_XML_To_FormProdotto(0) = CStr(Chiave)
                                    'Xml dell'impresa che registra il documento
                                    vet_XML_To_FormProdotto(1) = Verifica_StringaVuota(DataGrid_Prodotti.Items(IndiceRigaGriglia).Cells(COL_Xml_Provenienza).Text())
                                    'Xml del contatto
                                    vet_XML_To_FormProdotto(2) = Verifica_StringaVuota(DataGrid_Prodotti.Items(IndiceRigaGriglia).Cells(COL_Xml_Destinazione).Text())

                                    Session("vet_XML_To_FormProdotto") = vet_XML_To_FormProdotto

                                Case LAVCOD_FATTURA_RICEVUTA

                                    CaricoScarico = "C"

                                    vet_XML_To_FormProdotto(0) = CStr(Chiave)
                                    'Xml dell'impresa che registra il documento
                                    vet_XML_To_FormProdotto(1) = Verifica_StringaVuota(DataGrid_Prodotti.Items(IndiceRigaGriglia).Cells(COL_Xml_Destinazione).Text())
                                    'Xml del contatto
                                    vet_XML_To_FormProdotto(2) = Verifica_StringaVuota(DataGrid_Prodotti.Items(IndiceRigaGriglia).Cells(COL_Xml_Provenienza).Text())

                                    Session("vet_XML_To_FormProdotto") = vet_XML_To_FormProdotto

                                Case LAVCOD_BOLLA_RICEVUTA

                                    CaricoScarico = "C"

                                    vet_XML_To_FormProdotto(0) = CStr(Chiave)
                                    'Xml dell'impresa che registra il d2ocumento
                                    vet_XML_To_FormProdotto(1) = Verifica_StringaVuota(DataGrid_Prodotti.Items(IndiceRigaGriglia).Cells(COL_Xml_Destinazione).Text())
                                    'Xml del contatto
                                    vet_XML_To_FormProdotto(2) = Verifica_StringaVuota(DataGrid_Prodotti.Items(IndiceRigaGriglia).Cells(COL_Xml_Provenienza).Text())

                                    Session("vet_XML_To_FormProdotto") = vet_XML_To_FormProdotto

                                Case LAVCOD_BOLLA_EMESSA

                                    CaricoScarico = "S"

                                    vet_XML_To_FormProdotto(0) = CStr(Chiave)
                                    'Xml dell'impresa che registra il documento
                                    vet_XML_To_FormProdotto(1) = Verifica_StringaVuota(DataGrid_Prodotti.Items(IndiceRigaGriglia).Cells(COL_Xml_Provenienza).Text())
                                    'Xml del contatto
                                    vet_XML_To_FormProdotto(2) = Verifica_StringaVuota(DataGrid_Prodotti.Items(IndiceRigaGriglia).Cells(COL_Xml_Destinazione).Text())

                                    Session("vet_XML_To_FormProdotto") = vet_XML_To_FormProdotto

                                Case LAVCOD_NOTA_ACCREDITO_RICEVUTA

                                    CaricoScarico = "S"

                                    vet_XML_To_FormProdotto(0) = CStr(Chiave)
                                    'Xml dell'impresa che registra il documento
                                    vet_XML_To_FormProdotto(1) = Verifica_StringaVuota(DataGrid_Prodotti.Items(IndiceRigaGriglia).Cells(COL_Xml_Provenienza).Text())
                                    'Xml del contatto
                                    vet_XML_To_FormProdotto(2) = Verifica_StringaVuota(DataGrid_Prodotti.Items(IndiceRigaGriglia).Cells(COL_Xml_Destinazione).Text())

                                    Session("vet_XML_To_FormProdotto") = vet_XML_To_FormProdotto

                                Case LAVCOD_NOTA_ACCREDITO_EMESSA

                                    CaricoScarico = "C"

                                    vet_XML_To_FormProdotto(0) = CStr(Chiave)
                                    'Xml dell'impresa che registra il documento
                                    vet_XML_To_FormProdotto(1) = Verifica_StringaVuota(DataGrid_Prodotti.Items(IndiceRigaGriglia).Cells(COL_Xml_Destinazione).Text())
                                    'Xml del contatto
                                    vet_XML_To_FormProdotto(2) = Verifica_StringaVuota(DataGrid_Prodotti.Items(IndiceRigaGriglia).Cells(COL_Xml_Provenienza).Text())

                                    Session("vet_XML_To_FormProdotto") = vet_XML_To_FormProdotto

                            End Select


                            '----------------------------------------

                            If VerificaEsistenza_CodContatto_as_PivaGIAS(objParametri_Server, Session, Page,
                            Me.Cmb_Contatti.SelectedValue.Split("|")(0)) = True Then
                                Cod_Contatto = Me.Txt_CodContatto_Contatto.Text
                                Ragsoc_Contatto = Me.Cmb_Contatti.SelectedItem.Text
                            Else
                                Cod_Contatto = ""
                                Ragsoc_Contatto = ""
                            End If

                            Dim lModeDatoLavCod As String = "fattura"

                            If QS_LavCod = LAVCOD_BOLLA_RICEVUTA OrElse
                                QS_LavCod = LAVCOD_BOLLA_EMESSA Then
                                lModeDatoLavCod = "bolla"
                            End If

                            Dim Id_Agenda_dettaglio As Integer = CInt(DataGrid_Prodotti.Items(IndiceRigaGriglia).Cells(COL_Id_Agenda_Destinazione).Text) 'ID_Agenda
                            'Chiave = CInt(DataGrid_Prodotti.Items(IndiceRigaGriglia).Cells(COL_CHIAVE).Text)


                            'chiamo la form prodotto 

                            Dim QueryString = "?o=" &
                             Stringa_Codifica(enum_TipoOperazioneDB.Modifica, AgroKey_EncoderDecoder, Server) &
                             "&c=" &
                             Stringa_Codifica(CaricoScarico, AgroKey_EncoderDecoder, Server) &
                             "&k=" &
                             Stringa_Codifica(ChiaveAlbero, AgroKey_EncoderDecoder, Server) &
                             "&orig=" &
                             Stringa_Codifica(enum_PagineAgenda_2010.Pagina_DocumentoContabileGenerico, AgroKey_EncoderDecoder, Server) &
                             "&mode=" &
                             Stringa_Codifica(lModeDatoLavCod, AgroKey_EncoderDecoder, Server) &
                             "&l=" &
                             Stringa_Codifica(QS_LavCod, AgroKey_EncoderDecoder, Server) &
                             "&d=" &
                             Stringa_Codifica(Me.Txt_DataEmissione.Text, AgroKey_EncoderDecoder, Server) &
                            "&ora=" &
                             Stringa_Codifica("12:00", AgroKey_EncoderDecoder, Server) &
                             "&tipo=" &
                             Stringa_Codifica(Tipo, AgroKey_EncoderDecoder, Server) &
                            "&codcont=" &
                            Stringa_Codifica(Cod_Contatto, AgroKey_EncoderDecoder, Server) &
                            "&ragcont=" &
                            Stringa_Codifica(AgronicaCoreDataProvider.UtilityProvider.QS_SaveText(Ragsoc_Contatto), AgroKey_EncoderDecoder, Server) &
                             "&ts=" &
                             Stringa_Codifica(Me.Cmb_TipoSconto.SelectedValue, AgroKey_EncoderDecoder, Server) &
                             "&dialog=" &
                             Stringa_Codifica("true", AgroKey_EncoderDecoder, Server) &
                            "&a=" & Stringa_Codifica(Id_Agenda_dettaglio, AgroKey_EncoderDecoder)


                            '  Vanni, 28/05/2014 16:52:14: vecchia chiamata
                            '770, 1020, 0, 0, _
                            Dim StrWindowOpen As String = AgronicaCoreDataProvider.UtilityProvider.JqueryModalDialogScript(
                                                       "../GestioneMagazzini/FormProdotto.aspx", QueryString, InsProdotto.ClientID,
                                            0, 0, 0, 0,
                                            , , , , , , NomeForm:="aspnetForm")

                            If upTabs_2 Is Nothing Then
                                Page.FindControl("aspnetForm").Controls.Add(New LiteralControl(StrWindowOpen))
                            Else
                                ScriptManager.RegisterClientScriptBlock(upTabs_2, upTabs_2.GetType(),
                                                             String.Format("jQuery_{0}", "openmodal"), StrWindowOpen, True)

                            End If

                    End Select

            End Select

        End If

    End Sub

    '####################################################################################################################
    Private Sub Btn_RicavaPesoNetto_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_RicavaPesoNetto.Click

        Ricava_PesoNettoTotale()

    End Sub


    '####################################################################################################################
    Private Sub Ricava_PesoNettoTotale()

        '##################################################################################################
        '###################### RICAVA IL PESO NETTO SUI DETTAGLI  ########################################
        '##################################################################################################

        Dim Qta As Decimal = 0
        Dim i As Integer


        For i = 0 To Me.DataGrid_Prodotti.Items.Count - 1

            'Controllo Udm_Cod_Extra_Unico
            If Ricava_MacroUdm(Me.DataGrid_Prodotti.Items(i).Cells(COL_UDM_COD).Text) = 2 Then

                Qta = Qta + Ricava_MacroQta(Me.DataGrid_Prodotti.Items(i).Cells(COL_UDM_COD).Text, Me.DataGrid_Prodotti.Items(i).Cells(COL_QTA).Text)

            End If

        Next

        If Qta = 0 AndAlso Me.Txt_PesoNetto.Text <> "" Then
            'LASCIO QUELLO CHE E' IMPOSTATO NELLA TXT
        Else
            Me.Txt_PesoNetto.Text = Qta
        End If



    End Sub


    '###################################################################################################################
    Private Sub Cmb_TipoSconto_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Cmb_TipoSconto.SelectedIndexChanged

        AggiornaImporto(Me.Cmb_TipoSconto.SelectedValue)

    End Sub


    '<Obsolete("XML_Leggi_Fattura Sostituita da Leggi_Fattura che usa l'AgronicaCoreModello e non l'XML")>
    'Private Sub XML_Leggi_Fattura(ByRef hash_iva As Hashtable)

    '    Dim StrAgenda As String
    '    Dim i, j As Integer
    '    Dim Dt As New DataTable

    '    Dim XmlDoc As New XmlDocument
    '    Dim XML_DatiAgenda As XmlElement
    '    Dim XML_Agenda As XmlElement
    '    Dim XML_DatiMovimenti As XmlElement
    '    Dim XML_Movimento As XmlElement
    '    Dim XMLs_Movimento As XmlNodeList
    '    Dim XML_DatiPagamenti As XmlElement
    '    Dim XML_DatiMovimentiDettagli As XmlElement
    '    Dim XML_MovimentoDettaglio As XmlElement
    '    Dim XMLs_MovimentoDettaglio As XmlNodeList


    '    ''Leggo la stringa relativa all'operazione
    '    'StrAgenda = objAgenda.Agenda_Leggi( _
    '    '                                 CStr(Qs_Piva), _
    '    '                                 CInt(Qs_SaCod), _
    '    '                                 CInt(Qs_IdAgenda), _
    '    '                                 , _
    '    '                                 , _
    '    '                                 , _
    '    '                                 , _
    '    '                                 CBool(False), _
    '    '                                 CStr(Session("ASG_Connessione_Server")))

    '    Dim objAgenda As New AgronicaCoreContabBIZ.Agenda_R

    '    'Leggo la stringa relativa all'operazione
    '    StrAgenda = objAgenda.Agenda_Leggi(
    '                                     CStr(Qs_Piva),
    '                                     CInt(Qs_SaCod),
    '                                     CInt(Qs_IdAgenda),
    '                                     0,
    '                                     False,
    '                                    objParametri_Server)

    '    If StrAgenda = "" Then
    '        Messaggi.AgroMsgBox("Errore durante la lettura dell'operazione!", Page, NomeForm:="aspnetForm", UpdatePanel:=upTabs_1)

    '    Else

    '        'Carico la stringa nel documento XML
    '        XmlDoc.LoadXml(StrAgenda)


    '        XML_DatiAgenda = XmlDoc.SelectSingleNode("DatiAgenda")

    '        XML_Agenda = XML_DatiAgenda.SelectSingleNode("Agenda")

    '        'If InStr(XML_Agenda.GetAttribute("des_lib"), ":") > 0 Then
    '        '    Des_Lib = XML_Agenda.GetAttribute("des_lib").Split(":")(1)
    '        'Else
    '        '    Des_Lib = XML_Agenda.GetAttribute("des_lib")
    '        'End If

    '        'Me.Txt_DesLib.Text = Des_Lib

    '        XML_DatiMovimenti = XML_Agenda.SelectSingleNode("DatiMovimenti")

    '        XMLs_Movimento = XML_DatiMovimenti.SelectNodes("Movimento")

    '        'XML_Movimento = XML_DatiMovimenti.SelectSingleNode("Movimento")

    '        For i = 0 To XMLs_Movimento.Count - 1

    '            XML_Movimento = XMLs_Movimento(i)


    '            x_Cau_Mov = XML_Movimento.GetAttribute("cau_mov")
    '            Select Case x_Cau_Mov


    '                '#######################################################
    '                '########### MOVIMENTO CONTABILE #######################
    '                '#######################################################
    '                Case CAU_REGISTRAZIONI

    '                    x_Data_Emissione = XML_Movimento.GetAttribute("data_movimento")
    '                    x_Data_Registrazione = XML_Movimento.GetAttribute("data_registrazione")
    '                    x_Scadenza = XML_Movimento.GetAttribute("scadenza")
    '                    x_Extra_Date = XML_Movimento.GetAttribute("extra_date") 'insolvenza

    '                    x_Extra_Str = XML_Movimento.GetAttribute("extra_str") 'note
    '                    x_Mov_Desc_Contabile = XML_Movimento.GetAttribute("mov_desc") 'descrizione

    '                    x_Num_Protocollo = XML_Movimento.GetAttribute("num_protocollo")
    '                    v_Tot_Importo = x_Num_Protocollo

    '                    x_Doc_Numero_Sin = XML_Movimento.GetAttribute("doc_numero_sin")
    '                    x_Doc_Numero = XML_Movimento.GetAttribute("doc_numero")
    '                    x_Doc_Numero_Des = XML_Movimento.GetAttribute("doc_numero_des")
    '                    Session("Doc_Numero") = x_Doc_Numero_Sin & CStr(x_Doc_Numero) & x_Doc_Numero_Des

    '                    x_Tipo_Sconto = XML_Movimento.GetAttribute("tipo_sconto")
    '                    x_Extra_Int = XML_Movimento.GetAttribute("extra_int")
    '                    x_Peso = XML_Movimento.GetAttribute("peso")

    '                    x_Cod_RisUm = XML_Movimento.GetAttribute("cod_risum")
    '                    x_Cod_IndirizzoRisUm = XML_Movimento.GetAttribute("cod_indirizzorisum")
    '                    x_Cod_Destinazione = XML_Movimento.GetAttribute("cod_destinazione")
    '                    x_Cod_IndirizzoDestinazione = XML_Movimento.GetAttribute("cod_indirizzodestinazione")
    '                    x_Mezzo = XML_Movimento.GetAttribute("mezzo")
    '                    x_Cod_Vettore = XML_Movimento.GetAttribute("cod_vettore")
    '                    x_Cod_IndirizzoVettore = XML_Movimento.GetAttribute("cod_indirizzovettore")

    '                    'vanni, verificare ..
    '                    frm_Aspetto = CStr(XML_Movimento.GetAttribute("aspetto"))
    '                    frm_CausaleTrasporto = CStr(XML_Movimento.GetAttribute("causale_trasporto"))
    '                    frm_Ora = CDate(XML_Movimento.GetAttribute("ora")).ToShortTimeString

    '                    If CStr(XML_Movimento.GetAttribute("scadenza")) = "31/12/2100" Then
    '                        frm_Consegna = ""
    '                    Else
    '                        frm_Consegna = CStr(XML_Movimento.GetAttribute("scadenza"))
    '                    End If

    '                    frm_NaturaBeni = CStr(XML_Movimento.GetAttribute("natura_beni"))

    '                    'frm_Peso = CStr(XML_Movimento.GetAttribute("peso"))

    '                    frm_Colli = CStr(XML_Movimento.GetAttribute("colli"))



    '                    ''====================================================================================
    '                    ''Prelevo le informazioni sui dettagli tecnici extra
    '                    ''------------------------------------------------------------------------------------
    '                    'XMLs_MovDettaglioTecnicoExtra = XML_Movimento.GetElementsByTagName("Movimento_Dettaglio_Tecnico_Extra")

    '                    'For z = 0 To XMLs_MovDettaglioTecnicoExtra.Count - 1

    '                    '    'Prelevo l'i-esimo Movimento Dettaglio
    '                    '    XML_MovDettaglioTecnicoExtra = XMLs_MovDettaglioTecnicoExtra.Item(z)

    '                    '    frm_Mac_Cod = XML_MovDettaglioTecnicoExtra.GetAttribute("mac_cod")
    '                    '    frm_Targa = UCase(XML_MovDettaglioTecnicoExtra.GetAttribute("targa"))
    '                    '    frm_Immatricolazione = XML_MovDettaglioTecnicoExtra.GetAttribute("n_immatricolazione")
    '                    '    frm_ImmatrRimorchio = XML_MovDettaglioTecnicoExtra.GetAttribute("n_immatricolazione_rimorchio")
    '                    '    frm_Autorizzizzazione = XML_MovDettaglioTecnicoExtra.GetAttribute("n_autorizzazione_trasporto")
    '                    '    frm_Data_Autorizzazione = IIf(XML_MovDettaglioTecnicoExtra.GetAttribute("data_rilascio_autorizzazione") = AgroDataInizio, "", XML_MovDettaglioTecnicoExtra.GetAttribute("data_rilascio_autorizzazione"))
    '                    '    frm_PesoTara = XML_MovDettaglioTecnicoExtra.GetAttribute("peso")


    '                    'Next


    '                    '=============================================================

    '                    '--------------------- PAGAMENTI --------------------------

    '                    XML_DatiPagamenti = XML_Movimento.SelectSingleNode("DatiPagamenti")

    '                    If Not IsNothing(XML_DatiPagamenti) Then
    '                        x_XML_Pagamenti = XML_DatiPagamenti.OuterXml
    '                    Else
    '                        x_XML_Pagamenti = ""
    '                    End If



    '                    '#######################################################
    '                    '########### MOVIMENTO MAGAZZINO #######################
    '                    '#######################################################
    '                Case CAU_CARICO, CAU_SCARICO

    '                    '-----------------------------------------------------------------
    '                    '------------------------- DETTAGLI ------------------------------
    '                    '-----------------------------------------------------------------
    '                    XML_DatiMovimentiDettagli = XML_Movimento.SelectSingleNode("DatiMovimenti_Dettagli")


    '                    XMLs_MovimentoDettaglio = XML_DatiMovimentiDettagli.SelectNodes("Movimento_Dettaglio")


    '                    '------------------------------------------
    '                    '----- Carica la Griglia
    '                    '------------------------------------------
    '                    CaricaGriglia_Dettagli()

    '                    '---------------------------------------------------
    '                    '----- BaseCode  +  TopCode
    '                    '---------------------------------------------------

    '                    Call Calcola_BaseCode_TopCode(x_BaseCode,
    '                                                    x_TopCode,
    '                                                    Session("ASG_ProgressivoGIAS"))


    '                    Dim XmlDoc_Dett As New XmlDocument

    '                    'se legge prima il caumov 7300 la data è "", imposto un valore
    '                    If IsNothing(x_Data_Emissione) Or x_Data_Emissione = "" Then
    '                        x_Data_Emissione = XML_Movimento.GetAttribute("data_movimento")
    '                    End If

    '                    For j = 0 To XMLs_MovimentoDettaglio.Count - 1

    '                        XML_MovimentoDettaglio = XMLs_MovimentoDettaglio(j)

    '                        XML_Leggi_MovimentoDettaglio(XmlDoc_Dett, XML_MovimentoDettaglio.OuterXml, "", hash_iva, False, x_Data_Emissione, Nothing)

    '                    Next

    '                    Dt = Session("DT_Prodotti_nel_Doc")

    '                    '----- Associo il DataTable con la DataGrid
    '                    Me.DataGrid_Prodotti.DataSource = Dt
    '                    Me.DataGrid_Prodotti.DataBind()


    '            End Select



    '        Next 'movimenti

    '    End If

    'End Sub


    '###################################################################################################################
    Private Sub Leggi_Fattura(ByRef hash_iva As Hashtable)

        Dim i, j As Integer
        Dim Dt As New DataTable

        Dim objAgenda As New Agenda_Operazione_Helper

        Dim Agenda As New Operazione_Agenda

        Agenda = objAgenda.Leggi(Qs_Piva,
                                 Qs_SaCod,
                                 Qs_IdAgenda,
                                 0,
                                 objParametri_Server)

        If IsNothing(Agenda) Then
            Messaggi.AgroMsgBox("Errore durante la lettura dell'operazione!", Page, NomeForm:="aspnetForm", UpdatePanel:=upTabs_1)
        Else

            'MOVIMENTI
            If Not IsNothing(Agenda.Movimenti) Then

                For i = 0 To Agenda.Movimenti.Count - 1

                    Select Case Agenda.Movimenti(i).Cau_Mov

                        '#######################################################
                        '########### MOVIMENTO CONTABILE #######################
                        '#######################################################
                        Case CAU_REGISTRAZIONI

                            x_Data_Emissione = Agenda.Movimenti(i).Data ' XML_Movimento.GetAttribute("data_movimento")
                            x_Data_Registrazione = Agenda.Movimenti(i).Data_Registrazione 'XML_Movimento.GetAttribute("data_registrazione")
                            x_Scadenza = Agenda.Movimenti(i).Scadenza 'XML_Movimento.GetAttribute("scadenza")
                            x_Extra_Date = Agenda.Movimenti(i).Extra_Date 'XML_Movimento.GetAttribute("extra_date") 'insolvenza

                            x_Extra_Str = Agenda.Movimenti(i).Extra_Str 'XML_Movimento.GetAttribute("extra_str") 'note
                            x_Mov_Desc_Contabile = Agenda.Movimenti(i).Mov_Desc 'XML_Movimento.GetAttribute("mov_desc") 'descrizione

                            x_Num_Protocollo = Agenda.Movimenti(i).Num_Protocollo ' XML_Movimento.GetAttribute("num_protocollo")
                            v_Tot_Importo = x_Num_Protocollo

                            x_Doc_Numero_Sin = Agenda.Movimenti(i).Doc_Numero_Sin 'XML_Movimento.GetAttribute("doc_numero_sin")
                            x_Doc_Numero = Agenda.Movimenti(i).Doc_Numero 'XML_Movimento.GetAttribute("doc_numero")
                            x_Doc_Numero_Des = Agenda.Movimenti(i).Doc_Numero_Des ' XML_Movimento.GetAttribute("doc_numero_des")
                            Session("Doc_Numero") = x_Doc_Numero_Sin & CStr(x_Doc_Numero) & x_Doc_Numero_Des

                            x_Tipo_Sconto = Agenda.Movimenti(i).Tipo_Sconto 'XML_Movimento.GetAttribute("tipo_sconto")
                            x_Extra_Int = Agenda.Movimenti(i).Extra_Int 'XML_Movimento.GetAttribute("extra_int")
                            x_Peso = Agenda.Movimenti(i).Peso 'XML_Movimento.GetAttribute("peso")

                            x_Cod_RisUm = Agenda.Movimenti(i).Cod_Risum 'XML_Movimento.GetAttribute("cod_risum")
                            x_Cod_IndirizzoRisUm = Agenda.Movimenti(i).Cod_IndirizzoRisUm ' XML_Movimento.GetAttribute("cod_indirizzorisum")
                            x_Cod_Destinazione = Agenda.Movimenti(i).Cod_Destinazione 'XML_Movimento.GetAttribute("cod_destinazione")
                            x_Cod_IndirizzoDestinazione = Agenda.Movimenti(i).Cod_IndirizzoDestinazione ' XML_Movimento.GetAttribute("cod_indirizzodestinazione")
                            x_Mezzo = Agenda.Movimenti(i).Mezzo 'XML_Movimento.GetAttribute("mezzo")
                            x_Cod_Vettore = Agenda.Movimenti(i).Cod_Vettore 'XML_Movimento.GetAttribute("cod_vettore")
                            x_Cod_IndirizzoVettore = Agenda.Movimenti(i).Cod_IndirizzoVettore 'XML_Movimento.GetAttribute("cod_indirizzovettore")

                            'vanni, verificare ..
                            frm_Aspetto = Agenda.Movimenti(i).Aspetto ' CStr(XML_Movimento.GetAttribute("aspetto"))
                            frm_CausaleTrasporto = Agenda.Movimenti(i).Causale_Trasporto 'CStr(XML_Movimento.GetAttribute("causale_trasporto"))
                            frm_Ora = CDate(Agenda.Movimenti(i).Ora).ToShortTimeString

                            If CStr(Agenda.Movimenti(i).Scadenza) = "31/12/2100" Then
                                frm_Consegna = ""
                            Else
                                frm_Consegna = CStr(Agenda.Movimenti(i).Scadenza)
                            End If

                            frm_NaturaBeni = Agenda.Movimenti(i).Natura_Beni ' CStr(XML_Movimento.GetAttribute("natura_beni"))

                            frm_Colli = Agenda.Movimenti(i).Colli ' CStr(XML_Movimento.GetAttribute("colli"))


                            ''--------------------- PAGAMENTI --------------------------
                            If Agenda.Movimenti(i).Pagamenti IsNot Nothing AndAlso Agenda.Movimenti(i).Pagamenti.Count > 0 Then
                                Session("Pagamenti") = Agenda.Movimenti(i).Pagamenti
                            End If

                            'XML_DatiPagamenti = XML_Movimento.SelectSingleNode("DatiPagamenti")

                            'If Not IsNothing(XML_DatiPagamenti) Then
                            '    x_XML_Pagamenti = XML_DatiPagamenti.OuterXml
                            'Else
                            '    x_XML_Pagamenti = ""
                            'End If



                            '#######################################################
                            '########### MOVIMENTO MAGAZZINO #######################
                            '#######################################################
                        Case CAU_CARICO, CAU_SCARICO


                            '------------------------------------------
                            '----- Carica la Griglia
                            '------------------------------------------
                            CaricaGriglia_Dettagli()

                            '---------------------------------------------------
                            '----- BaseCode  +  TopCode
                            '---------------------------------------------------

                            UtilityProvider.Calcola_BaseCode_TopCode(x_BaseCode, x_TopCode,
                                                                     Session("ASG_ProgressivoGIAS"))


                            'se legge prima il caumov 7300 la data è "", imposto un valore
                            If IsNothing(x_Data_Emissione) OrElse x_Data_Emissione = "" Then
                                x_Data_Emissione = Agenda.Movimenti(i).Data 'XML_Movimento.GetAttribute("data_movimento")
                            End If

                            '-----------------------------------------------------------------
                            '------------------------- DETTAGLI ------------------------------
                            '-----------------------------------------------------------------

                            If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli) Then

                                For j = 0 To Agenda.Movimenti(i).Movimenti_Dettagli.Count - 1
                                    Leggi_MovimentoDettaglio(Agenda.Movimenti(i).Movimenti_Dettagli(j), "", hash_iva, False, x_Data_Emissione, Nothing)
                                Next

                            End If

                            Dt = Session("DT_Prodotti_nel_Doc")

                            '----- Associo il DataTable con la DataGrid
                            Me.DataGrid_Prodotti.DataSource = Dt
                            Me.DataGrid_Prodotti.DataBind()

                    End Select

                Next

            End If

        End If


    End Sub

    '###################################################################################################################
    Private Sub Leggi_Fattura_Da_Sessione(ByVal AgendaScarico As Operazione_Agenda, ByRef hash_iva As Hashtable)

        Dim i, j As Integer
        Dim Dt As New DataTable

        Dim objAgenda As New Agenda_Operazione_Helper

        If IsNothing(AgendaScarico) Then
            Messaggi.AgroMsgBox("Errore durante la lettura dell'operazione!", Page, NomeForm:="aspnetForm", UpdatePanel:=upTabs_1)
        Else

            'MOVIMENTI
            If Not IsNothing(AgendaScarico.Movimenti) Then

                For i = 0 To AgendaScarico.Movimenti.Count - 1

                    Select Case AgendaScarico.Movimenti(i).Cau_Mov

                        '#######################################################
                        '########### MOVIMENTO CONTABILE #######################
                        '#######################################################
                        Case CAU_REGISTRAZIONI

                            x_Data_Emissione = AgendaScarico.Movimenti(i).Data ' XML_Movimento.GetAttribute("data_movimento")
                            x_Data_Registrazione = AgendaScarico.Movimenti(i).Data_Registrazione 'XML_Movimento.GetAttribute("data_registrazione")
                            x_Scadenza = AgendaScarico.Movimenti(i).Scadenza 'XML_Movimento.GetAttribute("scadenza")
                            x_Extra_Date = AgendaScarico.Movimenti(i).Extra_Date 'XML_Movimento.GetAttribute("extra_date") 'insolvenza

                            x_Extra_Str = AgendaScarico.Movimenti(i).Extra_Str 'XML_Movimento.GetAttribute("extra_str") 'note
                            x_Mov_Desc_Contabile = AgendaScarico.Movimenti(i).Mov_Desc 'XML_Movimento.GetAttribute("mov_desc") 'descrizione

                            x_Num_Protocollo = AgendaScarico.Movimenti(i).Num_Protocollo ' XML_Movimento.GetAttribute("num_protocollo")
                            v_Tot_Importo = x_Num_Protocollo

                            x_Doc_Numero_Sin = AgendaScarico.Movimenti(i).Doc_Numero_Sin 'XML_Movimento.GetAttribute("doc_numero_sin")
                            x_Doc_Numero = AgendaScarico.Movimenti(i).Doc_Numero 'XML_Movimento.GetAttribute("doc_numero")
                            x_Doc_Numero_Des = AgendaScarico.Movimenti(i).Doc_Numero_Des ' XML_Movimento.GetAttribute("doc_numero_des")
                            Session("Doc_Numero") = x_Doc_Numero_Sin & CStr(x_Doc_Numero) & x_Doc_Numero_Des

                            x_Tipo_Sconto = AgendaScarico.Movimenti(i).Tipo_Sconto 'XML_Movimento.GetAttribute("tipo_sconto")
                            x_Extra_Int = AgendaScarico.Movimenti(i).Extra_Int 'XML_Movimento.GetAttribute("extra_int")
                            x_Peso = AgendaScarico.Movimenti(i).Peso 'XML_Movimento.GetAttribute("peso")

                            x_Cod_RisUm = AgendaScarico.Movimenti(i).Cod_Risum 'XML_Movimento.GetAttribute("cod_risum")
                            x_Cod_IndirizzoRisUm = AgendaScarico.Movimenti(i).Cod_IndirizzoRisUm ' XML_Movimento.GetAttribute("cod_indirizzorisum")
                            x_Cod_Destinazione = AgendaScarico.Movimenti(i).Cod_Destinazione 'XML_Movimento.GetAttribute("cod_destinazione")
                            x_Cod_IndirizzoDestinazione = AgendaScarico.Movimenti(i).Cod_IndirizzoDestinazione ' XML_Movimento.GetAttribute("cod_indirizzodestinazione")
                            x_Mezzo = AgendaScarico.Movimenti(i).Mezzo 'XML_Movimento.GetAttribute("mezzo")
                            x_Cod_Vettore = AgendaScarico.Movimenti(i).Cod_Vettore 'XML_Movimento.GetAttribute("cod_vettore")
                            x_Cod_IndirizzoVettore = AgendaScarico.Movimenti(i).Cod_IndirizzoVettore 'XML_Movimento.GetAttribute("cod_indirizzovettore")

                            'vanni, verificare ..
                            frm_Aspetto = AgendaScarico.Movimenti(i).Aspetto ' CStr(XML_Movimento.GetAttribute("aspetto"))
                            frm_CausaleTrasporto = AgendaScarico.Movimenti(i).Causale_Trasporto 'CStr(XML_Movimento.GetAttribute("causale_trasporto"))
                            frm_Ora = CDate(AgendaScarico.Movimenti(i).Ora).ToShortTimeString

                            If CStr(AgendaScarico.Movimenti(i).Scadenza) = "31/12/2100" Then
                                frm_Consegna = ""
                            Else
                                frm_Consegna = CStr(AgendaScarico.Movimenti(i).Scadenza)
                            End If

                            frm_NaturaBeni = AgendaScarico.Movimenti(i).Natura_Beni ' CStr(XML_Movimento.GetAttribute("natura_beni"))

                            frm_Colli = AgendaScarico.Movimenti(i).Colli ' CStr(XML_Movimento.GetAttribute("colli"))


                            ''--------------------- PAGAMENTI --------------------------
                            If AgendaScarico.Movimenti(i).Pagamenti IsNot Nothing AndAlso AgendaScarico.Movimenti(i).Pagamenti.Count > 0 Then
                                Session("Pagamenti") = AgendaScarico.Movimenti(i).Pagamenti
                            End If

                            '#######################################################
                            '########### MOVIMENTO MAGAZZINO #######################
                            '#######################################################
                        Case CAU_CARICO, CAU_SCARICO


                            '------------------------------------------
                            '----- Carica la Griglia
                            '------------------------------------------
                            CaricaGriglia_Dettagli()

                            '---------------------------------------------------
                            '----- BaseCode  +  TopCode
                            '---------------------------------------------------

                            UtilityProvider.Calcola_BaseCode_TopCode(x_BaseCode, x_TopCode,
                                                                     Session("ASG_ProgressivoGIAS"))


                            'se legge prima il caumov 7300 la data è "", imposto un valore
                            If IsNothing(x_Data_Emissione) OrElse x_Data_Emissione = "" Then
                                x_Data_Emissione = AgendaScarico.Movimenti(i).Data
                            End If

                            '-----------------------------------------------------------------
                            '------------------------- DETTAGLI ------------------------------
                            '-----------------------------------------------------------------

                            If Not IsNothing(AgendaScarico.Movimenti(i).Movimenti_Dettagli) Then

                                For j = 0 To AgendaScarico.Movimenti(i).Movimenti_Dettagli.Count - 1
                                    Leggi_MovimentoDettaglio(AgendaScarico.Movimenti(i).Movimenti_Dettagli(j), "", hash_iva, False, x_Data_Emissione, Nothing)
                                Next

                            End If

                            Dt = Session("DT_Prodotti_nel_Doc")

                            '----- Associo il DataTable con la DataGrid
                            Me.DataGrid_Prodotti.DataSource = Dt
                            Me.DataGrid_Prodotti.DataBind()

                    End Select

                Next

            End If

        End If


    End Sub

    '########################################################################################
    Private Sub XML_Leggi_MovimentoDettaglio(ByRef XmlDoc As XmlDocument,
                                            ByVal str_Movimento_Dettaglio As String,
                                            ByVal str_RifMagazzino_Contatto As String,
                                            ByRef hash_iva As Hashtable,
                                            ByVal Flag_DDTAllegato As Boolean,
                                            ByVal Data_Operazione As Date,
                                            ByVal updatePandel As UpdatePanel)

        Dim objContabHLP As New AgronicaCoreContabHLP.Contabilita
        Dim objIVA As New AgronicaCoreMetaSchemaDAL.IVA_Aliquote_R

        Dim XML_MovimentoDettaglio As XmlElement
        Dim XML_MovimentoDettaglio_Rif As XmlElement
        Dim XML_MovimentoDettaglioTecnico As XmlElement
        Dim XML_MovimentoDestinazione As XmlElement
        Dim XML_MovimentoDestinazione_Rif As XmlElement
        Dim XML_MovRiferimento2 As XmlElement
        Dim XML_Allegato As XmlElement

        Dim Dt_Fabbricato As DataTable
        Dim r_Id_Destinazione As Integer
        Dim r_Destinazione As String

        Dim Piva As String
        Dim Sa_Cod As Integer
        Dim Id_Agenda As Integer
        Dim Id_Mov As Integer
        Dim Id_Mov_Det As Integer
        Dim Id_Destinazione As Integer
        Dim Destinazione As String
        Dim XML As String

        Dim Num_DDT As String

        Dim Piva_Rif As String = ""
        Dim Sa_Cod_Rif As Integer = 0
        Dim Id_Agenda_Rif As Integer = 0
        Dim Id_Mov_Rif As Integer = 0
        Dim Id_Mov_Det_Rif As Integer = 0
        Dim Id_Destinazione_Rif As Integer = 0
        Dim Lav_Cod_Rif As Integer = 0
        Dim Destinazione_Rif As String = "Nessuno"
        Dim XML_Rif As String = ""

        Dim DT_DDT As DataTable
        Dim Piva_DDT As String = ""
        Dim Sa_Cod_DDT As Integer = 0
        Dim Id_Agenda_DDT As Integer = 0
        Dim Id_Mov_DDT As Integer = 0
        Dim Id_Mov_Det_DDT As Integer = 0
        Dim Lav_Cod_DDT As Integer = 0

        'Dim str_COL_PIVA_BASE As String
        'Dim str_COL_SACOD_BASE As String
        'Dim str_COL_IDDEST_BASE As String
        'Dim str_COL_DEST_BASE As String
        'Dim str_COL_XML_BASE As String
        'Dim str_COL_ID_AGENDA_BASE As String
        'Dim str_COL_ID_MOV_BASE As String
        'Dim str_COL_ID_MOV_DET_BASE As String

        'Dim str_COL_PIVA_RIF As String
        'Dim str_COL_SACOD_RIF As String
        'Dim str_COL_IDDEST_RIF As String
        'Dim str_COL_ID_AGENDA_RIF As String
        'Dim str_COL_ID_MOV_RIF As String
        'Dim str_COL_ID_MOV_DET_RIF As String

        Dim ValiditaInizio As String

        '------------------------------------------------
        '----- Calcolo i valori di BaseCode e TopCode
        '------------------------------------------------

        UtilityProvider.Calcola_BaseCode_TopCode(x_BaseCode, x_TopCode,
                                                 Session("ASG_ProgressivoGIAS").ToString)


        If str_Movimento_Dettaglio <> "" Then

            XmlDoc.LoadXml(str_Movimento_Dettaglio)

            XML_MovimentoDettaglio = XmlDoc.SelectSingleNode("//Movimento_Dettaglio")

            If Not IsNothing(XML_MovimentoDettaglio) Then


                XML_MovimentoDestinazione = XML_MovimentoDettaglio.SelectSingleNode("Movimento_Destinazione")

                XML_MovRiferimento2 = XML_MovimentoDettaglio.SelectSingleNode("Movimento_Riferimento2")

                '------------------------

                Piva = CStr(XML_MovimentoDettaglio.GetAttribute("piva"))

                Sa_Cod = CInt(XML_MovimentoDettaglio.GetAttribute("sa_cod"))

                Id_Agenda = CInt(XML_MovimentoDettaglio.GetAttribute("id_agenda"))

                Id_Mov = CInt(XML_MovimentoDettaglio.GetAttribute("id_mov"))

                Id_Mov_Det = CInt(XML_MovimentoDettaglio.GetAttribute("id_mov_det"))

                If Not IsDBNull(XML_MovimentoDettaglio.GetAttribute("validita_inizio")) AndAlso
                    Not IsNothing(XML_MovimentoDettaglio.GetAttribute("validita_inizio")) AndAlso
                    IsDate(XML_MovimentoDettaglio.GetAttribute("validita_inizio")) Then

                    ValiditaInizio = XML_MovimentoDettaglio.GetAttribute("validita_inizio")
                Else
                    ValiditaInizio = AGRODATAINIZIO
                End If

                '------------------------

                x_Mov_Det_Des = XML_MovimentoDettaglio.GetAttribute("mov_det_des")

                'chiave del prodotto
                x_Elem_Cod = XML_MovimentoDettaglio.GetAttribute("elem_cod")
                x_Pro_Cod = XML_MovimentoDettaglio.GetAttribute("pro_cod")
                x_Mat_Cod = XML_MovimentoDettaglio.GetAttribute("mat_cod")
                x_Cod_Progetto = XML_MovimentoDettaglio.GetAttribute("cod_progetto")
                x_Fase_Cod = XML_MovimentoDettaglio.GetAttribute("fase_cod")
                x_Lotto = XML_MovimentoDettaglio.GetAttribute("lotto")
                x_Cal_Cod = XML_MovimentoDettaglio.GetAttribute("cal_cod")
                x_Udm_Cod = XML_MovimentoDettaglio.GetAttribute("udm_cod")

                If x_Udm_Cod = 0 Then
                    x_Udm_Sim = ""
                    x_Udm_Des = ""
                Else
                    Dim leggmetaschema As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R
                    x_Udm_Des = leggmetaschema.UdmDes_from_UdmCod(x_Udm_Cod, x_Udm_Sim, objParametri_Server)
                End If

                x_Udm_Cod_Extra = XML_MovimentoDettaglio.GetAttribute("udm_cod_extra")
                x_Qta_Extra = XML_MovimentoDettaglio.GetAttribute("qta_extra")
                x_Prezzo_Effettivo = XML_MovimentoDettaglio.GetAttribute("prezzo_effettivo")

                'serve per ricavare x_Descrizione
                x_Descrizione = Ricava_Tipo_Descrizione(Piva, Data_Operazione)

                x_Quantita = XML_MovimentoDettaglio.GetAttribute("qta")

                x_Prezzo_Unitario = CDbl(XML_MovimentoDettaglio.GetAttribute("prezzo_unitario"))
                x_Prezzo_Unitario_Netto = CDbl(XML_MovimentoDettaglio.GetAttribute("prezzo_unitario_netto"))

                x_Sconto_Perc = XML_MovimentoDettaglio.GetAttribute("sconto")
                x_Sconto = ((x_Prezzo_Unitario * x_Sconto_Perc) / 100) * x_Quantita

                x_Imponibile = objContabHLP.Leggi_Imponibile_PositivoNegativo(QS_LavCod, CDbl(XML_MovimentoDettaglio.GetAttribute("imponibile")))
                x_Imponibile_Netto = objContabHLP.Leggi_Imponibile_PositivoNegativo(QS_LavCod, CDbl(XML_MovimentoDettaglio.GetAttribute("imponibile_netto")))

                x_Cod_IVA = XML_MovimentoDettaglio.GetAttribute("cod_iva")
                x_Aliquota = objIVA.Aliquota_from_CodIVA(x_Cod_IVA, "", objParametri_Server)
                x_IVA = objContabHLP.Leggi_IVA_PositivaNegativa(QS_LavCod, CDbl(XML_MovimentoDettaglio.GetAttribute("iva")))

                x_Anno = XML_MovimentoDettaglio.GetAttribute("anno")
                x_Ric_Cod = XML_MovimentoDettaglio.GetAttribute("ric_cod")

                x_Cod_Conto = XML_MovimentoDettaglio.GetAttribute("cod_conto")
                If x_Cod_Conto = 0 Then
                    x_Conto = ""
                Else
                    If Not IsNothing(XML_MovimentoDettaglio.GetAttribute("conto")) AndAlso
                        XML_MovimentoDettaglio.GetAttribute("conto") <> "" Then
                        'da filtro movimenti
                        x_Conto = CStr(XML_MovimentoDettaglio.GetAttribute("conto"))
                    Else
                        'da form prodotto
                        Dim objconti As New AgronicaCoreContabDAL.PianoConti_Economici_R
                        x_Conto = objconti.ContoDescr_from_CodConto(Piva, x_Ric_Cod, x_Anno, x_Cod_Conto, True, objParametri_Server)
                    End If
                End If

                '##########################################################

                v_Tot_Imponibile_Lordo += x_Imponibile
                v_Tot_Variazioni += x_Sconto
                v_Tot_Imponibile_Netto += x_Imponibile_Netto
                v_Tot_Iva += x_IVA

                '##########################################################

                Riempi_Hash_IVA(hash_iva, x_Cod_IVA, x_Aliquota, x_Imponibile_Netto, x_IVA)

                '##########################################################

                If Not IsNothing(XML_MovimentoDestinazione) Then

                    Id_Destinazione = CInt(XML_MovimentoDestinazione.GetAttribute("id_destinazione"))


                    If Not IsNothing(XML_MovimentoDestinazione.GetAttribute("fabbricato_des")) AndAlso
                        XML_MovimentoDestinazione.GetAttribute("fabbricato_des") <> "" Then
                        Destinazione = XML_MovimentoDestinazione.GetAttribute("fabbricato_des")
                    Else
                        Destinazione = AgronicaCoreAnagrafeDAL.Fabbricati_R.FabbricatoDes_from_FabbricatoCod(Piva,
                                                                                 Sa_Cod,
                                                                                 Id_Destinazione,
                                                                                 objParametri_Server)
                    End If
                Else
                    Id_Destinazione = 0
                    Destinazione = ""
                End If

                XML_MovimentoDettaglioTecnico = XML_MovimentoDettaglio.SelectSingleNode("Movimento_Dettaglio_Tecnico_2")
                If Not IsNothing(XML_MovimentoDettaglioTecnico) Then

                    x_N = If(Not XML_MovimentoDettaglioTecnico.HasAttribute("n"), 0, XML_MovimentoDettaglioTecnico.GetAttribute("n"))
                    x_P2O5 = If(Not XML_MovimentoDettaglioTecnico.HasAttribute("p"), 0, XML_MovimentoDettaglioTecnico.GetAttribute("p"))
                    x_K2O = If(Not XML_MovimentoDettaglioTecnico.HasAttribute("k"), 0, XML_MovimentoDettaglioTecnico.GetAttribute("k"))
                    x_Cu = If(Not XML_MovimentoDettaglioTecnico.HasAttribute("cu"), 0, XML_MovimentoDettaglioTecnico.GetAttribute("cu"))
                    x_Regolamento_Cod_Ferti = If(Not XML_MovimentoDettaglioTecnico.HasAttribute("extra_int"), 0, XML_MovimentoDettaglioTecnico.GetAttribute("extra_int"))

                Else
                    x_N = 0
                    x_P2O5 = 0
                    x_K2O = 0
                    x_Cu = 0
                    x_Regolamento_Cod_Ferti = 0
                End If

                'lo sposto da qui
                'Imposta_MovDettaglio_Impresa(XML_MovimentoDettaglio, XML_MovimentoDestinazione)

                'XML = XML_MovimentoDettaglio.OuterXml


                '##############################################################

                If Flag_DDTAllegato Then

                    x_Allegato = ""
                    '/////////////////////////////////////////////////////////////
                    '           SCRITTURA AGGANCIO FATTURA - BOLLA
                    '/////////////////////////////////////////////////////////////

                    'sto arrivando dalla pagina di filtro mov contabili

                    Dim Flag_AllegatoPresente As Boolean = False
                    Dim Qta_GiaAgganciata As Decimal = 0
                    Dim Qta_Risultato As Decimal = 0
                    Dim Qta_Residua As Decimal = 0
                    Dim j As Integer

                    If Not IsNothing(XML_MovRiferimento2) Then

                        XML_Allegato = XML_MovRiferimento2.SelectSingleNode("Allegato")

                        Piva_DDT = XML_MovRiferimento2.GetAttribute("piva_rif")
                        Sa_Cod_DDT = XML_MovRiferimento2.GetAttribute("sa_cod_rif")
                        Id_Agenda_DDT = XML_MovRiferimento2.GetAttribute("id_agenda_rif")
                        Id_Mov_DDT = XML_MovRiferimento2.GetAttribute("id_mov_rif")
                        Id_Mov_Det_DDT = XML_MovRiferimento2.GetAttribute("id_mov_det_rif")
                        Lav_Cod_DDT = XML_MovRiferimento2.GetAttribute("lav_cod_rif")

                        Piva_Rif = Piva_DDT
                        Id_Agenda_Rif = Id_Agenda_DDT
                        Id_Mov_Rif = Id_Mov_DDT
                        Id_Mov_Det_Rif = Id_Mov_Det_DDT

                        If Not IsNothing(XML_Allegato) Then

                            Num_DDT = XML_Allegato.GetAttribute("doc_numero_completo")
                            x_Allegato = "DDT n." & Num_DDT

                            'altre info nel nodo allegato


                            '=========================================================================================================================================================================================
                            'Controllo Dettaglio Duplicato (se si sta tentando di allegare lo stesso dettaglio alla fattura
                            '-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                            Flag_AllegatoPresente = Verifica_Dettaglio_Allegato(Lav_Cod_DDT, Piva_DDT, Sa_Cod_DDT, Id_Agenda_DDT, Id_Mov_DDT, Id_Mov_Det_DDT)

                            If Flag_AllegatoPresente Then

                                Messaggi.AgroMsgBox(x_Descrizione & " non può essere associato poiché già allegato alla fattura.", Page, NomeForm:="aspnetForm", UpdatePanel:=upTabs_1)
                                Exit Sub

                            End If


                            '==============================================================================================================================================================================
                            'Verifico se il DDT è già stato agganciato ad altre fatture
                            '------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


                            'cerco il DDT corrispondente, se è allegato a una qualche fattura
                            Dim objRif As New AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R
                            DT_DDT = objRif.MovimentiRiferimenti_LeggiBilaterale_Completa(Piva_DDT,
                                                                                            0,
                                                                                            Id_Agenda_DDT,
                                                                                            Id_Mov_DDT,
                                                                                            Id_Mov_Det_DDT,
                                                                                            0,
                                                                                            "",
                                                                                            " AND (Lav_Cod_Rif = " & CStr(LAVCOD_FATTURA_EMESSA) & " OR Lav_Cod_Rif = " & CStr(LAVCOD_FATTURA_RICEVUTA) & " ) ",
                                                                                            "", 0, 0, 0, 0,
                                                                                            0,
                                                                                            "",
                                                                                            " AND (Lav_Cod = " & CStr(LAVCOD_FATTURA_EMESSA) & " OR Lav_Cod = " & CStr(LAVCOD_FATTURA_RICEVUTA) & " ) ",
                                                                                            "", objParametri_Server)


                            If Not IsNothing(DT_DDT) Then

                                For j = 0 To DT_DDT.Rows.Count - 1

                                    'Qta_Risultato = DT_DDT.Rows(j).Item("Qta_Risultato")
                                    Qta_Risultato = DT_DDT.Rows(j).Item("Qta")

                                    Qta_GiaAgganciata += Qta_Risultato

                                Next

                            End If

                            Qta_Residua = x_Quantita - Qta_GiaAgganciata

                            Select Case Qta_Residua

                                Case Is < 0 'la qta residua da poter utilizzare è < 0 --> errore
                                    Messaggi.AgroMsgBox("Attenzione, il DDT n." & Num_DDT & " è già stato allegato ad altre fatture per una quantità maggiore di quella del DDT!" &
                                    vbCrLf & "Verificare le altre fatture a cui è allegato.", Page)
                                    Exit Sub

                                Case Is = 0 'la qta residua è 0
                                    Messaggi.AgroMsgBox(x_Descrizione & " non può essere associato al documento poiché è già stato allegato interamente ad altre fatture.", Page)
                                    Exit Sub

                                Case Is > 0 'la qta residua da poter utilizzare è > 0

                                    'se la qta residua è < della qta della bolla
                                    '--> allora già usato parzialmente
                                    If Qta_Residua < x_Quantita Then
                                        Messaggi.AgroMsgBox(x_Descrizione & " può essere associato al documento solo parzialmente poichè è stato allegato anche ad altre fatture.", Page)
                                    End If

                                    x_Quantita = Qta_Residua



                            End Select

                        Else
                            'non può essere!!!!!
                            Messaggi.AgroMsgBox("Non sono stati trovati i dati del documento allegato!", Page)
                        End If

                        Imposta_MovDettaglio_Impresa(XML_MovimentoDettaglio, XML_MovimentoDestinazione,
                                                     XML_MovRiferimento2, XML_MovimentoDettaglioTecnico)

                        XML = XML_MovimentoDettaglio.OuterXml

                    Else
                        'non può essere!!!!!
                        Messaggi.AgroMsgBox("Non sono stati trovati i dati del documento allegato!", Page)
                    End If

                Else
                    x_Allegato = ""
                    '/////////////////////////////////////////////////////////////
                    '    1)       MOV DI RIFERIMENTO PER IL CONTATTO
                    '/////////////////////////////////////////////////////////////
                    '                        OPPURE
                    '/////////////////////////////////////////////////////////////
                    '    2)   LETTURA/MODIFICA  AGGANCIO FATTURA - BOLLA
                    '/////////////////////////////////////////////////////////////

                    'non sto arrivando dalla pagina di filtro

                    'siamo nel caso di lettura/modifica di un aggancio bolla-fattura
                    'oppure, di gestione del movimento di riferimento del contatto


                    If Not IsNothing(XML_MovRiferimento2) Then

                        'devo distinguere i due casi

                        'LAVCOD_BOLLA_EMESSA = 1031
                        'LAVCOD_BOLLA_RICEVUTA = 1025
                        'LAVCOD_FATTURA_EMESSA = 1001
                        'LAVCOD_FATTURA_RICEVUTA = 1000

                        'per farlo verifico se il lav_cod_rif è una bolla ricevuta o emessa
                        'in caso affermativo si tratta di aggancio bolle-fatture
                        'in caso negativo di tratta di movimento di carico/scarico del contatto

                        Piva_DDT = XML_MovRiferimento2.GetAttribute("piva_rif")
                        Sa_Cod_DDT = XML_MovRiferimento2.GetAttribute("sa_cod_rif")
                        Id_Agenda_DDT = XML_MovRiferimento2.GetAttribute("id_agenda_rif")
                        Id_Mov_DDT = XML_MovRiferimento2.GetAttribute("id_mov_rif")
                        Id_Mov_Det_DDT = XML_MovRiferimento2.GetAttribute("id_mov_det_rif")
                        Lav_Cod_DDT = XML_MovRiferimento2.GetAttribute("lav_cod_rif")

                        Piva_Rif = Piva_DDT
                        Id_Agenda_Rif = Id_Agenda_DDT
                        Id_Mov_Rif = Id_Mov_DDT
                        Id_Mov_Det_Rif = Id_Mov_Det_DDT

                        Select Case Lav_Cod_DDT

                            Case LAVCOD_CONFERIMENTO, LAVCOD_CONFERIMENTO_DIVERSI

                                'non ancora gestito

                                '================================================

                            Case LAVCOD_BOLLA_EMESSA, LAVCOD_DDT_CONTABILIZZATO_EMESSO, LAVCOD_BOLLA_RICEVUTA

                                '//////////////////////////////////////////////////////////////////////
                                '    2)   RIPRISTINO DATI (LETTURA/MODIFICA)  AGGANCIO FATTURA - BOLLA
                                '//////////////////////////////////////////////////////////////////////

                                'legge i dati della bolla x ricavare numero documento

                                'Dim DT_DDT As DataTable

                                'non passo l'id_mov, perché quello dell'xml è quello del movimento di magazzino,
                                'qui invece servirebbe l'id_mov contabile
                                Dim objMovcont As New AgronicaCoreContabDAL.Movimenti_R
                                DT_DDT = objMovcont.MovimentiContabili(Lav_Cod_DDT,
                                                                       Piva,
                                                                       0,
                                                                       Id_Agenda_DDT,
                                                                       0,
                                                                       0,
                                                                       CAU_REGISTRAZIONI,
                                                                       0,
                                                                       AGRODATAINIZIO,
                                                                       AGRODATAFINE,
                                                                       "XYZ",
                                                                       0,
                                                                       "XYZ",
                                                                       0,
                                                                       0,
                                                                       AGRODATAINIZIO,
                                                                       "",
                                                                       "",
                                                                       objParametri_Server)


                                x_Allegato = ""
                                x_Pendente = enum_Pendenza.DocBolla

                                If Not IsNothing(DT_DDT) Then

                                    If DT_DDT.Rows.Count <> 0 Then

                                        Num_DDT = DT_DDT.Rows(0).Item("Doc_Numero_Sin") & CStr(DT_DDT.Rows(0).Item("Doc_Numero")) & DT_DDT.Rows(0).Item("Doc_Numero_Des")
                                        x_Allegato = "DDT n." & Num_DDT

                                    Else
                                        Messaggi.AgroMsgBox("Errore nella lettura del DDT allegato.", Page)
                                    End If
                                Else
                                    Messaggi.AgroMsgBox("Errore nella lettura del DDT allegato.", Page)
                                End If

                                Imposta_MovDettaglio_Impresa(XML_MovimentoDettaglio, XML_MovimentoDestinazione,
                                                             XML_MovRiferimento2, XML_MovimentoDettaglioTecnico)

                                'Dr.Item(str_COL_XML_BASE) = XML_MovimentoDettaglio.OuterXml
                                XML = XML_MovimentoDettaglio.OuterXml

                                '================================================


                            Case LAVCOD_CARICO, LAVCOD_SCARICO 'movimenti di magazzino

                                '/////////////////////////////////////////////////////////////
                                '    1)       MOV DI RIFERIMENTO PER IL CONTATTO
                                '/////////////////////////////////////////////////////////////

                                x_Allegato = ""

                                'in questo caso non devo passare XML_MovRiferimento2 (perché si intende quello di aggancio alla bolla)
                                Imposta_MovDettaglio_Impresa(XML_MovimentoDettaglio, XML_MovimentoDestinazione,
                                                             Nothing, XML_MovimentoDettaglioTecnico)

                                'Dr.Item(str_COL_XML_BASE) = XML_MovimentoDettaglio.OuterXml
                                XML = XML_MovimentoDettaglio.OuterXml

                                Dim mDestLeggi As New AgronicaCoreContabDAL.Mov_Destinazioni_R
                                Dt_Fabbricato = mDestLeggi.Leggi(
                                                CStr(XML_MovRiferimento2.GetAttribute("piva")),
                                                CInt(XML_MovRiferimento2.GetAttribute("sa_cod")),
                                                CInt(XML_MovRiferimento2.GetAttribute("id_agenda")),
                                                CInt(XML_MovRiferimento2.GetAttribute("id_mov")),
                                                CInt(XML_MovRiferimento2.GetAttribute("id_mov_det")),
                                                0,
                                                0,
                                                0,
                                                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                "",
                                                "",
                                                objParametri_Server
                                        )


                                If Not IsNothing(Dt_Fabbricato) Then
                                    If Dt_Fabbricato.Rows.Count <> 0 Then
                                        r_Id_Destinazione = Dt_Fabbricato.Rows(0).Item("Id_Destinazione")
                                        r_Destinazione = Dt_Fabbricato.Rows(0).Item("Fabbricato_Des")
                                    End If
                                End If

                                'Dr.Item(str_COL_PIVA_RIF) = CStr(XML_MovRiferimento2.GetAttribute("piva"))
                                Piva_Rif = CStr(XML_MovRiferimento2.GetAttribute("piva"))

                                'Dr.Item(str_COL_SACOD_RIF) = CInt(XML_MovRiferimento2.GetAttribute("sa_cod"))
                                Sa_Cod_Rif = CInt(XML_MovRiferimento2.GetAttribute("sa_cod"))

                                'Dr.Item(str_COL_ID_AGENDA_RIF) = CInt(XML_MovRiferimento2.GetAttribute("id_agenda"))
                                Id_Agenda_Rif = CInt(XML_MovRiferimento2.GetAttribute("id_agenda"))

                                'Dr.Item(str_COL_ID_MOV_RIF) = CInt(XML_MovRiferimento2.GetAttribute("id_mov"))
                                Id_Mov_Rif = CInt(XML_MovRiferimento2.GetAttribute("id_mov"))

                                'Dr.Item(str_COL_ID_MOV_DET_RIF) = CInt(XML_MovRiferimento2.GetAttribute("id_mov_det"))
                                Id_Mov_Det_Rif = CInt(XML_MovRiferimento2.GetAttribute("id_mov_det"))

                                'Dr.Item(str_COL_IDDEST_RIF) = r_Id_Destinazione ' query x prelevare id dest
                                Id_Destinazione_Rif = r_Id_Destinazione ' query x prelevare id dest

                                Destinazione_Rif = r_Destinazione 'query per prelevare nome fabbricato


                                XML_MovimentoDettaglio.RemoveChild(XML_MovRiferimento2)

                                Imposta_MovDettaglio_Contatto(XML_MovimentoDettaglio,
                                                                 XML_MovimentoDettaglio_Rif,
                                                                 XML_MovimentoDestinazione_Rif,
                                                                 CStr(XML_MovRiferimento2.GetAttribute("piva")),
                                                                 CStr(XML_MovRiferimento2.GetAttribute("sa_cod")),
                                                                 r_Id_Destinazione,
                                                                 x_Cau_Mov)


                                XML_Rif = XML_MovimentoDettaglio_Rif.OuterXml

                                '================================================

                            Case Else

                                Messaggi.AgroMsgBox("Riferimento non gestito", Page)

                        End Select ' 2 casi



                    Else
                        'caso di nessun riferimento

                        Imposta_MovDettaglio_Impresa(XML_MovimentoDettaglio, XML_MovimentoDestinazione,
                                                     Nothing, XML_MovimentoDettaglioTecnico)

                        XML = XML_MovimentoDettaglio.OuterXml

                    End If 'XML_MovRiferimento2


                End If 'Flag_DDTAllegato = false


                '##########################################################

                Select Case x_Cau_Mov

                    Case CAU_CARICO

                        x_Piva_Dest = Piva
                        x_SaCod_Dest = Sa_Cod
                        x_IdAgenda_Dest = Id_Agenda
                        x_IdMov_Dest = Id_Mov
                        x_IdMovDet_Dest = Id_Mov_Det
                        x_Id_Destinazione = Id_Destinazione
                        x_Destinazione = Destinazione
                        x_XML_Destinazione = XML

                        x_Piva_Prov = Piva_Rif
                        x_SaCod_Prov = Sa_Cod_Rif
                        x_IdAgenda_Prov = Id_Agenda_Rif
                        x_IdMov_Prov = Id_Mov_Rif
                        x_IdMovDet_Prov = Id_Mov_Det_Rif
                        x_Id_Provenienza = Id_Destinazione_Rif
                        x_Provenienza = Destinazione_Rif
                        x_XML_Provenienza = XML_Rif



                    Case CAU_SCARICO

                        x_Piva_Dest = Piva_Rif
                        x_SaCod_Dest = Sa_Cod_Rif
                        x_IdAgenda_Dest = Id_Agenda_Rif
                        x_IdMov_Dest = Id_Mov_Rif
                        x_IdMovDet_Dest = Id_Mov_Det_Rif
                        x_Id_Destinazione = Id_Destinazione_Rif
                        x_Destinazione = Destinazione_Rif
                        x_XML_Destinazione = XML_Rif

                        x_Piva_Prov = Piva
                        x_SaCod_Prov = Sa_Cod
                        x_IdAgenda_Prov = Id_Agenda
                        x_IdMov_Prov = Id_Mov
                        x_IdMovDet_Prov = Id_Mov_Det
                        x_Id_Provenienza = Id_Destinazione
                        x_Provenienza = Destinazione
                        x_XML_Provenienza = XML


                End Select

                '##########################################################

                'Inserisco nel DT
                Inserisci_MovDettaglio_inGriglia(x_Pendente,
                                                x_Allegato,
                                                x_Tipo,
                                                x_Elem_Cod,
                                                x_Pro_Cod,
                                                x_Mat_Cod,
                                                x_Cod_Progetto,
                                                x_Fase_Cod,
                                                x_Lotto,
                                                x_Cal_Cod,
                                                x_Udm_Cod,
                                                x_Udm_Cod_Extra,
                                                x_Qta_Extra,
                                                x_Prezzo_Effettivo,
                                                x_Descrizione,
                                                x_Udm_Sim,
                                                x_Quantita,
                                                x_Prezzo_Unitario,
                                                x_Prezzo_Unitario_Netto,
                                                x_Sconto_Perc,
                                                x_Sconto,
                                                x_Imponibile,
                                                x_Imponibile_Netto,
                                                x_Cod_IVA,
                                                x_Aliquota,
                                                x_IVA,
                                                x_Anno,
                                                x_Ric_Cod,
                                                x_Cod_Conto,
                                                x_Conto,
                                                x_Piva_Dest,
                                                x_SaCod_Dest,
                                                x_IdAgenda_Dest,
                                                x_IdMov_Dest,
                                                x_IdMovDet_Dest,
                                                x_Id_Destinazione,
                                                x_Destinazione,
                                                x_Piva_Prov,
                                                x_SaCod_Prov,
                                                x_IdAgenda_Prov,
                                                x_IdMov_Prov,
                                                x_IdMovDet_Prov,
                                                x_Id_Provenienza,
                                                x_Provenienza,
                                                x_XML_Destinazione,
                                                x_XML_Provenienza,
                                                Nothing,
                                                Nothing,
                                                ValiditaInizio,
                                                 x_N, x_P2O5, x_K2O, x_Cu,
                                                 x_Regolamento_Cod_Ferti)

            End If

        End If 'stringa ""



    End Sub

    '########################################################################################
    Private Sub Leggi_MovimentoDettaglio(ByVal Movimento_Dettaglio As Movimento_Dettaglio,
                                         ByVal str_RifMagazzino_Contatto As String,
                                         ByRef hash_iva As Hashtable,
                                         ByVal Flag_DDTAllegato As Boolean,
                                         ByVal Data_Operazione As Date,
                                         ByVal updatePandel As UpdatePanel)

        Dim objContabHLP As New AgronicaCoreContabHLP.Contabilita
        Dim objIVA As New AgronicaCoreMetaSchemaDAL.IVA_Aliquote_R

        'Dim XML_MovimentoDettaglio As XmlElement
        'Dim XML_MovimentoDettaglio_Rif As XmlElement
        'Dim XML_MovimentoDestinazione As XmlElement
        'Dim XML_MovimentoDestinazione_Rif As XmlElement
        'Dim XML_MovRiferimento2 As XmlElement
        'Dim XML_Allegato As XmlElement

        Dim Dt_Fabbricato As DataTable
        Dim r_Id_Destinazione As Integer
        Dim r_Destinazione As String

        Dim Cau_Mov As String
        Dim Piva As String
        Dim Sa_Cod As Integer
        Dim Id_Agenda As Integer
        Dim Id_Mov As Integer
        Dim Id_Mov_Det As Integer
        Dim Id_Destinazione As Integer
        Dim Destinazione As String
        Dim XML As String
        Dim Dettaglio_Destinazione As Movimento_Dettaglio = Nothing
        Dim Dettaglio_Provenienza As Movimento_Dettaglio = Nothing
        Dim Dettaglio_Dest As New Movimento_Dettaglio
        Dim Dettaglio_Prov As New Movimento_Dettaglio

        Dim MovDettaglioTecnico As Movimento_Dettaglio_Tecnico = Nothing


        Dim Num_DDT As String = ""

        Dim Piva_Rif As String = ""
        Dim Sa_Cod_Rif As Integer = 0
        Dim Id_Agenda_Rif As Integer = 0
        Dim Id_Mov_Rif As Integer = 0
        Dim Id_Mov_Det_Rif As Integer = 0
        Dim Id_Destinazione_Rif As Integer = 0
        Dim Lav_Cod_Rif As Integer = 0
        Dim Destinazione_Rif As String = "Nessuno"
        Dim XML_Rif As String = ""

        Dim DT_DDT As DataTable
        Dim Piva_DDT As String = ""
        Dim Sa_Cod_DDT As Integer = 0
        Dim Id_Agenda_DDT As Integer = 0
        Dim Id_Mov_DDT As Integer = 0
        Dim Id_Mov_Det_DDT As Integer = 0
        Dim Lav_Cod_DDT As Integer = 0

        'Dim str_COL_XML_BASE As String

        'Dim str_COL_PIVA_RIF As String
        'Dim str_COL_SACOD_RIF As String
        'Dim str_COL_IDDEST_RIF As String
        'Dim str_COL_ID_AGENDA_RIF As String
        'Dim str_COL_ID_MOV_RIF As String
        'Dim str_COL_ID_MOV_DET_RIF As String

        Dim ValiditaInizio As String

        '------------------------------------------------
        '----- Calcolo i valori di BaseCode e TopCode
        '------------------------------------------------

        UtilityProvider.Calcola_BaseCode_TopCode(x_BaseCode, x_TopCode,
                                                 Session("ASG_ProgressivoGIAS").ToString)

        If Not IsNothing(Movimento_Dettaglio) Then


            '------------------------

            Cau_Mov = Movimento_Dettaglio.Cau_Mov

            Piva = Movimento_Dettaglio.Piva
            Sa_Cod = Movimento_Dettaglio.Sa_Cod
            Id_Agenda = Movimento_Dettaglio.Id_Agenda
            Id_Mov = Movimento_Dettaglio.Id_Mov
            Id_Mov_Det = Movimento_Dettaglio.Id_Mov_Det

            If Not IsDBNull(Movimento_Dettaglio.Validita_Inizio) AndAlso
                Not IsNothing(Movimento_Dettaglio.Validita_Inizio) AndAlso
                IsDate(Movimento_Dettaglio.Validita_Inizio) Then

                ValiditaInizio = Movimento_Dettaglio.Validita_Inizio
            Else
                ValiditaInizio = AGRODATAINIZIO
            End If

            '------------------------

            x_Mov_Det_Des = Movimento_Dettaglio.Mov_Det_Des

            'chiave del prodotto
            x_Elem_Cod = Movimento_Dettaglio.Elem_Cod
            x_Pro_Cod = Movimento_Dettaglio.Pro_Cod
            x_Mat_Cod = Movimento_Dettaglio.Mat_Cod
            x_Cod_Progetto = Movimento_Dettaglio.Cod_Progetto
            x_Fase_Cod = Movimento_Dettaglio.Fase_Cod
            x_Lotto = Movimento_Dettaglio.Lotto
            x_Cal_Cod = Movimento_Dettaglio.Cal_Cod
            x_Udm_Cod = Movimento_Dettaglio.Udm_Cod

            If x_Udm_Cod = 0 Then
                x_Udm_Sim = ""
                x_Udm_Des = ""
            Else
                Dim leggMetaschema As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R
                x_Udm_Des = leggMetaschema.UdmDes_from_UdmCod(x_Udm_Cod, x_Udm_Sim, objParametri_Server)
            End If

            x_Udm_Cod_Extra = Movimento_Dettaglio.Udm_Cod_Extra
            x_Qta_Extra = Movimento_Dettaglio.Qta_Extra
            x_Prezzo_Effettivo = Movimento_Dettaglio.Prezzo_Effettivo

            'serve per ricavare x_Descrizione
            x_Descrizione = Ricava_Tipo_Descrizione(Piva, Data_Operazione)

            x_Quantita = Movimento_Dettaglio.Qta

            x_Prezzo_Unitario = Movimento_Dettaglio.Prezzo_Unitario
            x_Prezzo_Unitario_Netto = Movimento_Dettaglio.Prezzo_Unitario_Netto

            x_Sconto_Perc = Movimento_Dettaglio.Sconto
            x_Sconto = ((x_Prezzo_Unitario * x_Sconto_Perc) / 100) * x_Quantita

            x_Imponibile = objContabHLP.Leggi_Imponibile_PositivoNegativo(QS_LavCod, Movimento_Dettaglio.Imponibile)
            x_Imponibile_Netto = objContabHLP.Leggi_Imponibile_PositivoNegativo(QS_LavCod, Movimento_Dettaglio.Imponibile_Netto)

            x_Cod_IVA = Movimento_Dettaglio.Cod_Iva
            x_Aliquota = objIVA.Aliquota_from_CodIVA(x_Cod_IVA, "", objParametri_Server)
            x_IVA = objContabHLP.Leggi_IVA_PositivaNegativa(QS_LavCod, Movimento_Dettaglio.Iva)

            x_Anno = Movimento_Dettaglio.Anno
            x_Ric_Cod = Movimento_Dettaglio.Ric_Cod

            x_Cod_Conto = Movimento_Dettaglio.Cod_Conto
            If x_Cod_Conto = 0 Then
                x_Conto = ""
            Else
                'da form prodotto
                Dim objconti As New AgronicaCoreContabDAL.PianoConti_Economici_R
                x_Conto = objconti.ContoDescr_from_CodConto(Piva, x_Ric_Cod, x_Anno, x_Cod_Conto, True, objParametri_Server)
            End If

            '##########################################################

            v_Tot_Imponibile_Lordo += x_Imponibile
            v_Tot_Variazioni += x_Sconto
            v_Tot_Imponibile_Netto += x_Imponibile_Netto
            v_Tot_Iva += x_IVA

            '##########################################################

            Riempi_Hash_IVA(hash_iva, x_Cod_IVA, x_Aliquota, x_Imponibile_Netto, x_IVA)

            '##########################################################

            If Not IsNothing(Movimento_Dettaglio.Movimenti_Destinazioni) AndAlso Movimento_Dettaglio.Movimenti_Destinazioni.Count > 0 Then

                Id_Destinazione = Movimento_Dettaglio.Movimenti_Destinazioni(0).Id_Destinazione

                Destinazione = AgronicaCoreAnagrafeDAL.Fabbricati_R.FabbricatoDes_from_FabbricatoCod(
                                                                         Piva,
                                                                         Sa_Cod,
                                                                         Id_Destinazione,
                                                                         objParametri_Server)

            Else
                Id_Destinazione = 0
                Destinazione = ""
            End If


            '##########################################################

            If Not IsNothing(Movimento_Dettaglio.Movimenti_Dettagli_Tecnici) AndAlso Movimento_Dettaglio.Movimenti_Dettagli_Tecnici.Count > 0 Then
                MovDettaglioTecnico = Movimento_Dettaglio.Movimenti_Dettagli_Tecnici(0)

                x_N = MovDettaglioTecnico.N
                x_P2O5 = MovDettaglioTecnico.P
                x_K2O = MovDettaglioTecnico.K
                x_Cu = MovDettaglioTecnico.Cu
                x_Regolamento_Cod_Ferti = MovDettaglioTecnico.Extra_Int
            Else
                x_N = 0
                x_P2O5 = 0
                x_K2O = 0
                x_Cu = 0
                x_Regolamento_Cod_Ferti = 0
            End If



            '##############################################################

            If Flag_DDTAllegato Then

                x_Allegato = ""
                '/////////////////////////////////////////////////////////////
                '           SCRITTURA AGGANCIO FATTURA - BOLLA
                '/////////////////////////////////////////////////////////////

                'sto arrivando dalla pagina di filtro mov contabili

                Dim Flag_AllegatoPresente As Boolean = False
                Dim Qta_GiaAgganciata As Decimal = 0
                Dim Qta_Risultato As Decimal = 0
                Dim Qta_Residua As Decimal = 0
                Dim j As Integer

                If Not IsNothing(Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti) AndAlso Not Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti.Count > 0 Then

                    'XML_Allegato = XML_MovRiferimento2.SelectSingleNode("Allegato")

                    Piva_DDT = Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(0).Piva_Rif
                    Sa_Cod_DDT = Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(0).Sa_Cod_Rif
                    Id_Agenda_DDT = Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(0).Id_Agenda_Rif
                    Id_Mov_DDT = Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(0).Id_Mov_Rif
                    Id_Mov_Det_DDT = Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(0).Id_Mov_Det_Rif
                    Lav_Cod_DDT = Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(0).Lav_Cod_Rif

                    Piva_Rif = Piva_DDT
                    Id_Agenda_Rif = Id_Agenda_DDT
                    Id_Mov_Rif = Id_Mov_DDT
                    Id_Mov_Det_Rif = Id_Mov_Det_DDT

                    'If Not IsNothing(XML_Allegato) Then

                    'Num_DDT = XML_Allegato.GetAttribute("doc_numero_completo")
                    x_Allegato = "DDT n." & Num_DDT

                    'altre info nel nodo allegato


                    '=========================================================================================================================================================================================
                    'Controllo Dettaglio Duplicato (se si sta tentando di allegare lo stesso dettaglio alla fattura
                    '-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                    Flag_AllegatoPresente = Verifica_Dettaglio_Allegato(Lav_Cod_DDT, Piva_DDT, Sa_Cod_DDT, Id_Agenda_DDT, Id_Mov_DDT, Id_Mov_Det_DDT)

                    If Flag_AllegatoPresente Then

                        Messaggi.AgroMsgBox(x_Descrizione & " non può essere associato poiché già allegato alla fattura.", Page, NomeForm:="aspnetForm", UpdatePanel:=upTabs_1)
                        Exit Sub

                    End If


                    '==============================================================================================================================================================================
                    'Verifico se il DDT è già stato agganciato ad altre fatture
                    '------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


                    'cerco il DDT corrispondente, se è allegato a una qualche fattura
                    Dim objRif As New AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R
                    DT_DDT = objRif.MovimentiRiferimenti_LeggiBilaterale_Completa(
                                                                                    Piva_DDT,
                                                                                    0,
                                                                                    Id_Agenda_DDT,
                                                                                    Id_Mov_DDT,
                                                                                    Id_Mov_Det_DDT,
                                                                                    0,
                                                                                    "",
                                                                                    " AND (Lav_Cod_Rif = " & CStr(LAVCOD_FATTURA_EMESSA) & " OR Lav_Cod_Rif = " & CStr(LAVCOD_FATTURA_RICEVUTA) & " ) ",
                                                                                    "", 0, 0, 0, 0,
                                                                                    0,
                                                                                    "",
                                                                                    " AND (Lav_Cod = " & CStr(LAVCOD_FATTURA_EMESSA) & " OR Lav_Cod = " & CStr(LAVCOD_FATTURA_RICEVUTA) & " ) ",
                                                                                    "", objParametri_Server)


                    If Not IsNothing(DT_DDT) Then

                        For j = 0 To DT_DDT.Rows.Count - 1

                            'Qta_Risultato = DT_DDT.Rows(j).Item("Qta_Risultato")
                            Qta_Risultato = DT_DDT.Rows(j).Item("Qta")

                            Qta_GiaAgganciata += Qta_Risultato

                        Next

                    End If

                    Qta_Residua = x_Quantita - Qta_GiaAgganciata

                    Select Case Qta_Residua

                        Case Is < 0 'la qta residua da poter utilizzare è < 0 --> errore
                            Messaggi.AgroMsgBox("Attenzione, il DDT n." & Num_DDT & " è già stato allegato ad altre fatture per una quantità maggiore di quella del DDT!" &
                            vbCrLf & "Verificare le altre fatture a cui è allegato.", Page)
                            Exit Sub

                        Case Is = 0 'la qta residua è 0
                            Messaggi.AgroMsgBox(x_Descrizione & " non può essere associato al documento poiché è già stato allegato interamente ad altre fatture.", Page)
                            Exit Sub

                        Case Is > 0 'la qta residua da poter utilizzare è > 0

                            'se la qta residua è < della qta della bolla
                            '--> allora già usato parzialmente
                            If Qta_Residua < x_Quantita Then
                                Messaggi.AgroMsgBox(x_Descrizione & " può essere associato al documento solo parzialmente poiché è stato allegato anche ad altre fatture.", Page)
                            End If

                            x_Quantita = Qta_Residua

                    End Select

                    'Else
                    '    'non può essere!!!!!
                    '    Messaggi.AgroMsgBox("Non sono stati trovati i dati del documento allegato!", Page)
                    'End If

                    Imposta_MovDettaglio_Impresa(Movimento_Dettaglio,
                                                 Movimento_Dettaglio.Movimenti_Destinazioni(0),
                                                 Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(0),
                                                 MovDettaglioTecnico)

                    'Dr.Item(str_COL_XML_BASE) = XML_MovimentoDettaglio.OuterXml
                    'XML = XML_MovimentoDettaglio.OuterXml

                    Dettaglio_Destinazione = Movimento_Dettaglio


                Else
                    'non può essere!!!!!
                    Messaggi.AgroMsgBox("Non sono stati trovati i dati del documento allegato!", Page)
                End If

            Else
                x_Allegato = ""
                '/////////////////////////////////////////////////////////////
                '    1)       MOV DI RIFERIMENTO PER IL CONTATTO
                '/////////////////////////////////////////////////////////////
                '                        OPPURE
                '/////////////////////////////////////////////////////////////
                '    2)   LETTURA/MODIFICA  AGGANCIO FATTURA - BOLLA
                '/////////////////////////////////////////////////////////////

                'non sto arrivando dalla pagina di filtro

                'siamo nel caso di lettura/modifica di un aggancio bolla-fattura
                'oppure, di gestione del movimento di riferimento del contatto


                If Not IsNothing(Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti) AndAlso Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti.Count > 0 Then

                    'devo distinguere i due casi

                    'LAVCOD_BOLLA_EMESSA = 1031
                    'LAVCOD_BOLLA_RICEVUTA = 1025
                    'LAVCOD_FATTURA_EMESSA = 1001
                    'LAVCOD_FATTURA_RICEVUTA = 1000

                    'per farlo verifico se il lav_cod_rif è una bolla ricevuta o emessa
                    'in caso affermativo si tratta di aggancio bolle-fatture
                    'in caso negativo di tratta di movimento di carico/scarico del contatto

                    Piva_DDT = Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(0).Piva_Rif 
                    Sa_Cod_DDT = Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(0).Sa_Cod_Rif
                    Id_Agenda_DDT = Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(0).Id_Agenda_Rif
                    Id_Mov_DDT = Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(0).Id_Mov_Rif
                    Id_Mov_Det_DDT = Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(0).Id_Mov_Det_Rif
                    Lav_Cod_DDT = Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(0).Lav_Cod_Rif

                    Piva_Rif = Piva_DDT
                    Id_Agenda_Rif = Id_Agenda_DDT
                    Id_Mov_Rif = Id_Mov_DDT
                    Id_Mov_Det_Rif = Id_Mov_Det_DDT

                    Select Case Lav_Cod_DDT

                        Case LAVCOD_CONFERIMENTO, LAVCOD_CONFERIMENTO_DIVERSI

                            'non ancora gestito

                            '================================================

                        Case LAVCOD_BOLLA_EMESSA, LAVCOD_DDT_CONTABILIZZATO_EMESSO, LAVCOD_BOLLA_RICEVUTA

                            '//////////////////////////////////////////////////////////////////////
                            '    2)   RIPRISTINO DATI (LETTURA/MODIFICA)  AGGANCIO FATTURA - BOLLA
                            '//////////////////////////////////////////////////////////////////////

                            'legge i dati della bolla x ricavare numero documento

                            'Dim DT_DDT As DataTable

                            'non passo l'id_mov, perché quello dell'xml è quello del movimento di magazzino,
                            'qui invece servirebbe l'id_mov contabile
                            Dim objMovcont As New AgronicaCoreContabDAL.Movimenti_R
                            DT_DDT = objMovcont.MovimentiContabili(Lav_Cod_DDT,
                                                                   Piva,
                                                                   0,
                                                                   Id_Agenda_DDT,
                                                                   0,
                                                                   0,
                                                                   CAU_REGISTRAZIONI,
                                                                   0,
                                                                   AGRODATAINIZIO,
                                                                   AGRODATAFINE,
                                                                   "XYZ",
                                                                   0,
                                                                   "XYZ",
                                                                   0,
                                                                   0,
                                                                   AGRODATAINIZIO,
                                                                   "",
                                                                   "",
                                                                   objParametri_Server)

                            x_Allegato = ""
                            x_Pendente = enum_Pendenza.DocBolla

                            If Not IsNothing(DT_DDT) Then

                                If DT_DDT.Rows.Count <> 0 Then

                                    Num_DDT = DT_DDT.Rows(0).Item("Doc_Numero_Sin") & CStr(DT_DDT.Rows(0).Item("Doc_Numero")) & DT_DDT.Rows(0).Item("Doc_Numero_Des")
                                    x_Allegato = "DDT n." & Num_DDT

                                Else
                                    Messaggi.AgroMsgBox("Errore nella lettura del DDT allegato.", Page)
                                End If
                            Else
                                Messaggi.AgroMsgBox("Errore nella lettura del DDT allegato.", Page)
                            End If

                            Imposta_MovDettaglio_Impresa(Movimento_Dettaglio,
                                                         Movimento_Dettaglio.Movimenti_Destinazioni(0),
                                                         Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(0),
                                                         MovDettaglioTecnico)

                            'Dr.Item(str_COL_XML_BASE) = XML_MovimentoDettaglio.OuterXml
                            'XML = XML_MovimentoDettaglio.OuterXml
                            Dettaglio_Destinazione = Movimento_Dettaglio
                            '================================================


                        Case LAVCOD_CARICO, LAVCOD_SCARICO 'movimenti di magazzino

                            '/////////////////////////////////////////////////////////////
                            '    1)       MOV DI RIFERIMENTO PER IL CONTATTO
                            '/////////////////////////////////////////////////////////////

                            x_Allegato = ""

                            'in questo caso non devo passare XML_MovRiferimento2 (perché si intende quello di aggancio alla bolla)
                            Imposta_MovDettaglio_Impresa(Movimento_Dettaglio,
                                                         Movimento_Dettaglio.Movimenti_Destinazioni(0),
                                                         Nothing,
                                                         MovDettaglioTecnico)

                            'Dr.Item(str_COL_XML_BASE) = XML_MovimentoDettaglio.OuterXml
                            'XML = XML_MovimentoDettaglio.OuterXml
                            Dettaglio_Destinazione = Movimento_Dettaglio

                            Dim mDestLeggi As New AgronicaCoreContabDAL.Mov_Destinazioni_R
                            Dt_Fabbricato = mDestLeggi.Leggi(
                                            Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(0).Piva,
                                            Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(0).Sa_Cod,
                                            Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(0).Id_Agenda,
                                            Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(0).Id_Mov,
                                            Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(0).Id_Mov_Det,
                                            0,
                                            0,
                                            0,
                                            AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                            "",
                                            "",
                                            objParametri_Server
                                    )


                            If Not IsNothing(Dt_Fabbricato) Then
                                If Dt_Fabbricato.Rows.Count <> 0 Then
                                    r_Id_Destinazione = Dt_Fabbricato.Rows(0).Item("Id_Destinazione")
                                    r_Destinazione = Dt_Fabbricato.Rows(0).Item("Fabbricato_Des")
                                End If
                            End If

                            Piva_Rif = Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(0).Piva

                            Sa_Cod_Rif = Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(0).Sa_Cod

                            Id_Agenda_Rif = Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(0).Id_Agenda

                            Id_Mov_Rif = Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(0).Id_Mov

                            Id_Mov_Det_Rif = Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(0).Id_Mov_Det

                            Id_Destinazione_Rif = r_Id_Destinazione ' query x prelevare id dest

                            Destinazione_Rif = r_Destinazione 'query per prelevare nome fabbricato

                            Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti.Clear()
                            'XML_MovimentoDettaglio.RemoveChild(XML_MovRiferimento2)

                            Dim Movimento_Dettaglio_Rif As New Movimento_Dettaglio
                            Dim Movimento_Destinazione_Rif As New Movimento_Destinazione

                            Imposta_MovDettaglio_Contatto(Movimento_Dettaglio,
                                                             Movimento_Dettaglio_Rif,
                                                             Movimento_Destinazione_Rif,
                                                             Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(0).Piva,
                                                             Movimento_Dettaglio.Movimenti_Dettagli_Riferimenti(0).Sa_Cod,
                                                             r_Id_Destinazione,
                                                             x_Cau_Mov)


                            'XML_Rif = XML_MovimentoDettaglio_Rif.OuterXml
                            Dettaglio_Provenienza = Movimento_Dettaglio_Rif

                            '================================================

                        Case Else

                            Messaggi.AgroMsgBox("Riferimento non gestito", Page)

                    End Select ' 2 casi



                Else
                    'caso di nessun riferimento

                    Imposta_MovDettaglio_Impresa(Movimento_Dettaglio,
                                                 Movimento_Dettaglio.Movimenti_Destinazioni(0),
                                                 Nothing,
                                                 MovDettaglioTecnico)

                    'XML = XML_MovimentoDettaglio.OuterXml
                    Dettaglio_Destinazione = Movimento_Dettaglio

                End If 'XML_MovRiferimento2


            End If 'Flag_DDTAllegato = false


            '##########################################################

            Select Case Cau_Mov

                Case CAU_CARICO

                    x_Piva_Dest = Piva
                    x_SaCod_Dest = Sa_Cod
                    x_IdAgenda_Dest = Id_Agenda
                    x_IdMov_Dest = Id_Mov
                    x_IdMovDet_Dest = Id_Mov_Det
                    x_Id_Destinazione = Id_Destinazione
                    x_Destinazione = Destinazione
                    x_XML_Destinazione = XML

                    x_Piva_Prov = Piva_Rif
                    x_SaCod_Prov = Sa_Cod_Rif
                    x_IdAgenda_Prov = Id_Agenda_Rif
                    x_IdMov_Prov = Id_Mov_Rif
                    x_IdMovDet_Prov = Id_Mov_Det_Rif
                    x_Id_Provenienza = Id_Destinazione_Rif
                    x_Provenienza = Destinazione_Rif
                    x_XML_Provenienza = XML_Rif
                    Dettaglio_Dest = Dettaglio_Destinazione
                    Dettaglio_Prov = Dettaglio_Provenienza

                Case CAU_SCARICO

                    x_Piva_Dest = Piva_Rif
                    x_SaCod_Dest = Sa_Cod_Rif
                    x_IdAgenda_Dest = Id_Agenda_Rif
                    x_IdMov_Dest = Id_Mov_Rif
                    x_IdMovDet_Dest = Id_Mov_Det_Rif
                    x_Id_Destinazione = Id_Destinazione_Rif
                    x_Destinazione = Destinazione_Rif
                    x_XML_Destinazione = XML_Rif

                    x_Piva_Prov = Piva
                    x_SaCod_Prov = Sa_Cod
                    x_IdAgenda_Prov = Id_Agenda
                    x_IdMov_Prov = Id_Mov
                    x_IdMovDet_Prov = Id_Mov_Det
                    x_Id_Provenienza = Id_Destinazione
                    x_Provenienza = Destinazione
                    x_XML_Provenienza = XML
                    Dettaglio_Dest = Dettaglio_Provenienza
                    Dettaglio_Prov = Dettaglio_Destinazione

            End Select

            '##########################################################

            'Inserisco nel DT
            Inserisci_MovDettaglio_inGriglia(x_Pendente,
                                            x_Allegato,
                                            x_Tipo,
                                            x_Elem_Cod,
                                            x_Pro_Cod,
                                            x_Mat_Cod,
                                            x_Cod_Progetto,
                                            x_Fase_Cod,
                                            x_Lotto,
                                            x_Cal_Cod,
                                            x_Udm_Cod,
                                            x_Udm_Cod_Extra,
                                            x_Qta_Extra,
                                            x_Prezzo_Effettivo,
                                            x_Descrizione,
                                            x_Udm_Sim,
                                            x_Quantita,
                                            x_Prezzo_Unitario,
                                            x_Prezzo_Unitario_Netto,
                                            x_Sconto_Perc,
                                            x_Sconto,
                                            x_Imponibile,
                                            x_Imponibile_Netto,
                                            x_Cod_IVA,
                                            x_Aliquota,
                                            x_IVA,
                                            x_Anno,
                                            x_Ric_Cod,
                                            x_Cod_Conto,
                                            x_Conto,
                                            x_Piva_Dest,
                                            x_SaCod_Dest,
                                            x_IdAgenda_Dest,
                                            x_IdMov_Dest,
                                            x_IdMovDet_Dest,
                                            x_Id_Destinazione,
                                            x_Destinazione,
                                            x_Piva_Prov,
                                            x_SaCod_Prov,
                                            x_IdAgenda_Prov,
                                            x_IdMov_Prov,
                                            x_IdMovDet_Prov,
                                            x_Id_Provenienza,
                                            x_Provenienza,
                                            x_XML_Destinazione,
                                            x_XML_Provenienza,
                                            Dettaglio_Dest,
                                            Dettaglio_Prov,
                                            ValiditaInizio,
                                             x_N, x_P2O5, x_K2O, x_Cu,
                                             x_Regolamento_Cod_Ferti)

        End If




    End Sub

    '########################################################################################
    Private Sub Imposta_MovDettaglio_Impresa(ByRef XML_MovimentoDettaglio As XmlElement,
                                             ByRef XML_MovimentoDestinazione As XmlElement,
                                             ByRef XML_Riferimento2 As XmlElement,
                                             ByRef XML_MovimentoDettaglioTecnico As XmlElement)

        'ByRef XML_MovimentoDettaglio_Rif As XmlElement, _
        'ByRef XML_MovimentoDestinazione_Rif As XmlElement, _
        'ByVal piva_rif As String, _
        'ByVal sacod_rif As Integer, _
        'ByVal iddest_rif As Integer, _
        'ByVal Cau_Mov As String)

        XML_MovimentoDettaglio.SetAttribute("TipoOperazioneDB", CStr(enum_TipoOperazioneDB.Scrittura))
        XML_MovimentoDettaglio.SetAttribute("basecode", CStr(x_BaseCode))
        XML_MovimentoDettaglio.SetAttribute("topcode", CStr(x_TopCode))
        'XML_MovimentoDettaglio.SetAttribute("sa_cod", "0")
        XML_MovimentoDettaglio.SetAttribute("id_agenda", "0")
        XML_MovimentoDettaglio.SetAttribute("id_mov", "0")
        XML_MovimentoDettaglio.SetAttribute("id_mov_det", "0")

        If Not IsNothing(XML_MovimentoDestinazione) Then
            XML_MovimentoDestinazione.SetAttribute("TipoOperazioneDB", CStr(enum_TipoOperazioneDB.Scrittura))
            XML_MovimentoDestinazione.SetAttribute("basecode", CStr(x_BaseCode))
            XML_MovimentoDestinazione.SetAttribute("topcode", CStr(x_TopCode))
            'XML_MovimentoDestinazione.SetAttribute("sa_cod", "0")
            XML_MovimentoDestinazione.SetAttribute("id_agenda", "0")
            XML_MovimentoDestinazione.SetAttribute("id_mov", "0")
            XML_MovimentoDestinazione.SetAttribute("id_mov_det", "0")
            'XML_MovimentoDestinazione.SetAttribute("id_destinazione", "0")
        End If

        If Not IsNothing(XML_Riferimento2) Then

            XML_Riferimento2.SetAttribute("TipoOperazioneDB", CStr(enum_TipoOperazioneDB.Scrittura))
            XML_Riferimento2.SetAttribute("sa_cod", "0")
            XML_Riferimento2.SetAttribute("id_agenda", "0")
            XML_Riferimento2.SetAttribute("id_mov", "0")
            XML_Riferimento2.SetAttribute("id_mov_det", "0")

            XML_Riferimento2.SetAttribute("lav_cod", QS_LavCod)
            XML_Riferimento2.SetAttribute("cau_mov", x_Cau_Mov)
            XML_Riferimento2.SetAttribute("basecode", CStr(x_BaseCode))
            XML_Riferimento2.SetAttribute("topcode", CStr(x_TopCode))

        End If

        If Not IsNothing(XML_MovimentoDettaglioTecnico) Then
            XML_MovimentoDettaglioTecnico.SetAttribute("TipoOperazioneDB", CStr(enum_TipoOperazioneDB.Scrittura))
            XML_MovimentoDettaglioTecnico.SetAttribute("basecode", CStr(x_BaseCode))
            XML_MovimentoDettaglioTecnico.SetAttribute("topcode", CStr(x_TopCode))
            'XML_MovimentoDettaglioTecnico.SetAttribute("sa_cod", "0")
            XML_MovimentoDettaglioTecnico.SetAttribute("id_agenda", "0")
            XML_MovimentoDettaglioTecnico.SetAttribute("id_mov", "0")
            XML_MovimentoDettaglioTecnico.SetAttribute("id_mov_det", "0")
        End If

    End Sub

    '########################################################################################
    Private Sub Imposta_MovDettaglio_Impresa(ByRef Movimento_Dettaglio As Movimento_Dettaglio,
                                             ByRef Movimento_Destinazione As Movimento_Destinazione,
                                             ByRef Movimento_Dettaglio_Riferimento As Movimento_Dettaglio_Riferimento,
                                             ByRef Movimento_Dettaglio_Tecnico As Movimento_Dettaglio_Tecnico)

        Movimento_Dettaglio.Id_Agenda = Qs_IdAgenda
        Movimento_Dettaglio.Id_Mov = 0
        Movimento_Dettaglio.Id_Mov_Det = 0

        'XML_MovimentoDettaglio.SetAttribute("TipoOperazioneDB", CStr(enum_TipoOperazioneDB.Scrittura))
        'XML_MovimentoDettaglio.SetAttribute("basecode", CStr(x_BaseCode))
        'XML_MovimentoDettaglio.SetAttribute("topcode", CStr(x_TopCode))
        'XML_MovimentoDettaglio.SetAttribute("id_agenda", "0")
        'XML_MovimentoDettaglio.SetAttribute("id_mov", "0")
        'XML_MovimentoDettaglio.SetAttribute("id_mov_det", "0")

        If Not IsNothing(Movimento_Destinazione) Then
            Movimento_Destinazione.Id_Agenda = Qs_IdAgenda
            Movimento_Destinazione.Id_Mov = 0
            Movimento_Destinazione.Id_Mov_Det = 0
            'XML_MovimentoDestinazione.SetAttribute("TipoOperazioneDB", CStr(enum_TipoOperazioneDB.Scrittura))
            'XML_MovimentoDestinazione.SetAttribute("basecode", CStr(x_BaseCode))
            'XML_MovimentoDestinazione.SetAttribute("topcode", CStr(x_TopCode))
            'XML_MovimentoDestinazione.SetAttribute("id_agenda", "0")
            'XML_MovimentoDestinazione.SetAttribute("id_mov", "0")
            'XML_MovimentoDestinazione.SetAttribute("id_mov_det", "0")
        End If

        If Not IsNothing(Movimento_Dettaglio_Riferimento) Then

            Movimento_Dettaglio_Riferimento.Sa_Cod = 0
            Movimento_Dettaglio_Riferimento.Id_Agenda = Qs_IdAgenda
            Movimento_Dettaglio_Riferimento.Id_Mov = 0
            Movimento_Dettaglio_Riferimento.Id_Mov_Det = 0
            Movimento_Dettaglio_Riferimento.Lav_Cod = QS_LavCod
            Movimento_Dettaglio_Riferimento.Cau_Mov = x_Cau_Mov
            'XML_Riferimento2.SetAttribute("TipoOperazioneDB", CStr(enum_TipoOperazioneDB.Scrittura))
            'XML_Riferimento2.SetAttribute("sa_cod", "0")
            'XML_Riferimento2.SetAttribute("id_agenda", "0")
            'XML_Riferimento2.SetAttribute("id_mov", "0")
            'XML_Riferimento2.SetAttribute("id_mov_det", "0")
            'XML_Riferimento2.SetAttribute("lav_cod", QS_LavCod)
            'XML_Riferimento2.SetAttribute("cau_mov", x_Cau_Mov)
            'XML_Riferimento2.SetAttribute("basecode", CStr(x_BaseCode))
            'XML_Riferimento2.SetAttribute("topcode", CStr(x_TopCode))

        End If

        If Not IsNothing(Movimento_Dettaglio_Tecnico) Then
            Movimento_Dettaglio_Tecnico.Id_Agenda = Qs_IdAgenda
            Movimento_Dettaglio_Tecnico.Id_Mov = 0
            Movimento_Dettaglio_Tecnico.Id_Mov_Det = 0
        End If

    End Sub

    '########################################################################################
    Private Sub Imposta_MovDettaglio_Contatto(ByRef XML_MovimentoDettaglio As XmlElement,
                                                ByRef XML_MovimentoDettaglio_Rif As XmlElement,
                                                ByRef XML_MovimentoDestinazione_Rif As XmlElement,
                                                ByVal piva_rif As String,
                                                ByVal sacod_rif As Integer,
                                                ByVal iddest_rif As Integer,
                                                ByVal Cau_Mov As String)

        XML_MovimentoDettaglio_Rif = XML_MovimentoDettaglio.Clone

        XML_MovimentoDettaglio_Rif.SetAttribute("TipoOperazioneDB", CStr(enum_TipoOperazioneDB.Scrittura))
        XML_MovimentoDettaglio_Rif.SetAttribute("basecode", CStr(x_BaseCode))
        XML_MovimentoDettaglio_Rif.SetAttribute("topcode", CStr(x_TopCode))

        XML_MovimentoDettaglio_Rif.SetAttribute("id_agenda", "0")
        XML_MovimentoDettaglio_Rif.SetAttribute("id_mov", "0")
        XML_MovimentoDettaglio_Rif.SetAttribute("id_mov_det", "0")

        XML_MovimentoDettaglio_Rif.SetAttribute("piva", piva_rif)
        XML_MovimentoDettaglio_Rif.SetAttribute("sa_cod", CStr(sacod_rif))
        XML_MovimentoDettaglio_Rif.SetAttribute("id_destinazione", CStr(iddest_rif))  ' query x prelevare id dest

        Select Case Cau_Mov
            Case CAU_CARICO
                XML_MovimentoDettaglio_Rif.SetAttribute("cau_mov", CAU_SCARICO)
            Case CAU_SCARICO
                XML_MovimentoDettaglio_Rif.SetAttribute("cau_mov", CAU_CARICO)
            Case Else
                XML_MovimentoDettaglio_Rif.SetAttribute("cau_mov", "")
        End Select

        XML_MovimentoDettaglio_Rif.SetAttribute("lav_cod", CStr(0))

        XML_MovimentoDestinazione_Rif = XML_MovimentoDettaglio_Rif.SelectSingleNode("Movimento_Destinazione")

        If Not IsNothing(XML_MovimentoDestinazione_Rif) Then

            XML_MovimentoDestinazione_Rif.SetAttribute("TipoOperazioneDB", CStr(enum_TipoOperazioneDB.Scrittura))
            XML_MovimentoDestinazione_Rif.SetAttribute("basecode", CStr(x_BaseCode))
            XML_MovimentoDestinazione_Rif.SetAttribute("topcode", CStr(x_TopCode))

            XML_MovimentoDestinazione_Rif.SetAttribute("id_agenda", "0")
            XML_MovimentoDestinazione_Rif.SetAttribute("id_mov", "0")
            XML_MovimentoDestinazione_Rif.SetAttribute("id_mov_det", "0")

            XML_MovimentoDestinazione_Rif.SetAttribute("piva", piva_rif)
            XML_MovimentoDestinazione_Rif.SetAttribute("sa_cod", CStr(sacod_rif))
            XML_MovimentoDestinazione_Rif.SetAttribute("id_destinazione", CStr(iddest_rif))  ' query x prelevare id dest

        End If

    End Sub

    Private Sub Imposta_MovDettaglio_Contatto(ByRef Movimento_Dettaglio As Movimento_Dettaglio,
                                             ByRef Movimento_Dettaglio_Rif As Movimento_Dettaglio,
                                             ByRef Movimento_Destinazione_Rif As Movimento_Destinazione,
                                             ByVal piva_rif As String,
                                             ByVal sacod_rif As Integer,
                                             ByVal iddest_rif As Integer,
                                             ByVal Cau_Mov As String)


        Movimento_Dettaglio_Rif = Movimento_Dettaglio

        'XML_MovimentoDettaglio_Rif.SetAttribute("TipoOperazioneDB", CStr(enum_TipoOperazioneDB.Scrittura))
        'XML_MovimentoDettaglio_Rif.SetAttribute("basecode", CStr(x_BaseCode))
        'XML_MovimentoDettaglio_Rif.SetAttribute("topcode", CStr(x_TopCode))

        Movimento_Dettaglio_Rif.Id_Agenda = 0
        Movimento_Dettaglio_Rif.Id_Mov = 0
        Movimento_Dettaglio_Rif.Id_Mov_Det = 0
        'XML_MovimentoDettaglio_Rif.SetAttribute("id_agenda", "0")
        'XML_MovimentoDettaglio_Rif.SetAttribute("id_mov", "0")
        'XML_MovimentoDettaglio_Rif.SetAttribute("id_mov_det", "0")

        Movimento_Dettaglio_Rif.Piva = piva_rif
        Movimento_Dettaglio_Rif.Sa_Cod = sacod_rif

        'XML_MovimentoDettaglio_Rif.SetAttribute("piva", piva_rif)
        'XML_MovimentoDettaglio_Rif.SetAttribute("sa_cod", CStr(sacod_rif))
        'XML_MovimentoDettaglio_Rif.SetAttribute("id_destinazione", CStr(iddest_rif))  ' query x prelevare id dest

        Select Case Cau_Mov
            Case CAU_CARICO
                Movimento_Dettaglio_Rif.Cau_Mov = CAU_SCARICO
                'XML_MovimentoDettaglio_Rif.SetAttribute("cau_mov", CAU_SCARICO)
            Case CAU_SCARICO
                Movimento_Dettaglio_Rif.Cau_Mov = CAU_CARICO
            Case Else
                Movimento_Dettaglio_Rif.Cau_Mov = ""
        End Select

        Movimento_Dettaglio_Rif.Lav_Cod = 0
        'XML_MovimentoDettaglio_Rif.SetAttribute("lav_cod", CStr(0))

        'XML_MovimentoDestinazione_Rif = XML_MovimentoDettaglio_Rif.SelectSingleNode("Movimento_Destinazione")

        'If Not IsNothing(XML_MovimentoDestinazione_Rif) Then

        If Movimento_Dettaglio_Rif.Movimenti_Destinazioni IsNot Nothing AndAlso Movimento_Dettaglio.Movimenti_Destinazioni.Count > 0 Then

            'XML_MovimentoDestinazione_Rif.SetAttribute("TipoOperazioneDB", CStr(enum_TipoOperazioneDB.Scrittura))
            'XML_MovimentoDestinazione_Rif.SetAttribute("basecode", CStr(x_BaseCode))
            'XML_MovimentoDestinazione_Rif.SetAttribute("topcode", CStr(x_TopCode))

            Movimento_Destinazione_Rif.Id_Agenda = 0
            Movimento_Destinazione_Rif.Id_Mov = 0
            Movimento_Destinazione_Rif.Id_Mov_Det = 0

            Movimento_Destinazione_Rif.Piva = piva_rif
            Movimento_Destinazione_Rif.Sa_Cod = sacod_rif

            'XML_MovimentoDestinazione_Rif.SetAttribute("id_agenda", "0")
            'XML_MovimentoDestinazione_Rif.SetAttribute("id_mov", "0")
            'XML_MovimentoDestinazione_Rif.SetAttribute("id_mov_det", "0")

            'XML_MovimentoDestinazione_Rif.SetAttribute("piva", piva_rif)
            'XML_MovimentoDestinazione_Rif.SetAttribute("sa_cod", CStr(sacod_rif))
            'XML_MovimentoDestinazione_Rif.SetAttribute("id_destinazione", CStr(iddest_rif))  ' query x prelevare id dest

        End If

    End Sub


    '##################################################################################################
    Private Function Ricava_Tipo_Descrizione(ByVal x_Piva As String,
                                            ByVal Data_Operazione As Date) As String

        Dim Nome_Categoria As String = ""

        Select Case x_Elem_Cod

            Case SERVIZI
                'non può essere!
                x_Descrizione = x_Mov_Det_Des
                '-------------------------------------

            Case ALTRI_BENI, 1

                x_Tipo = CAU_MAGAZZINO 'anche se in realtà non movimenta il magazzino

                x_Descrizione = x_Mov_Det_Des
                '-------------------------------------

            Case ZOO_CONSISTENZA 'Consistenze Animali

                x_Tipo = CAU_ANIMALE

                If x_Cod_Progetto <> 0 Then

                    x_Descrizione = LeggiProdotto(Server, Session, Page, objParametri_Server,
                                                    Nome_Categoria,
                                                    x_Piva,
                                                    x_Elem_Cod,
                                                    x_Pro_Cod,
                                                    x_Mat_Cod,
                                                    x_Cod_Progetto,
                                                    0,
                                                    "",
                                                    0,
                                                    False,
                                                    Data_Operazione,
                                                    Data_Operazione,
                                                    False)

                Else
                    x_Descrizione = "Consistenze Zootecniche: " & x_Mov_Det_Des
                End If

                '-------------------------------------
            Case SEMENTI

                x_Tipo = CAU_MAGAZZINO

                x_Descrizione = LeggiProdotto(Server, Session, Page, objParametri_Server,
                                                Nome_Categoria,
                                                x_Piva,
                                                x_Elem_Cod,
                                                x_Pro_Cod,
                                                x_Mat_Cod,
                                                x_Cod_Progetto,
                                                x_Fase_Cod,
                                                x_Lotto,
                                                x_Cal_Cod,
                                                True,
                                                AGRODATAINIZIO,
                                                AGRODATAFINE,
                                                False)


            Case Else

                x_Tipo = CAU_MAGAZZINO

                x_Descrizione = LeggiProdotto(Server, Session, Page, objParametri_Server,
                                                Nome_Categoria,
                                                x_Piva,
                                                x_Elem_Cod,
                                                x_Pro_Cod,
                                                x_Mat_Cod,
                                                x_Cod_Progetto,
                                                x_Fase_Cod,
                                                x_Lotto,
                                                x_Cal_Cod,
                                                True,
                                                Data_Operazione,
                                                Data_Operazione,
                                                False)

        End Select

        Return x_Descrizione

    End Function



    '##################################################################################################
    Private Sub AggiornaImporto(ByVal Tipo_Sconto As Integer)

        'Dim i As Integer

        'Dim DT As DataTable

        'Dim Totale As Decimal
        'Dim Iva As Decimal
        'Dim Imponibile As Decimal
        'Dim Imposta As Decimal
        'Dim Importo As Decimal
        'Dim Sconto As Decimal
        'Dim ScontoImponibileLordo As Decimal
        'Dim Riga As Integer
        'Dim bAliquota As Boolean

        'Dim Qta As Decimal
        'Dim Prezzo_Unitario As Decimal

        ''MSIva.rows = 1
        ''TxtVariazioni = Format(0, "##,###,###.00")
        ''TxtImponibile = Format(0, "##,###,###.00")
        ''TxtImponibileNetto = Format(0, "##,###,###.00")
        ''TxtImposta = Format(0, "##,###,###.00")
        ''TxtImporto = Format(0, "##,###,###.00")

        'DT = Session("DT_Prodotti_nel_Doc")


        'For i = 0 To DT.Rows.Count - 1


        '    If IsNumeric(MSDettagli.TextMatrix(i, COL_QTA)) And IsNumeric(MSDettagli.TextMatrix(i, COL_PREZZO)) Then


        '        Select Case Tipo_Sconto

        '            Case enum_TipoSconto.PrezzoUnitario

        '                MSDettagli.TextMatrix(i, COL_PREZZO_NETTO) = Format(CDbl(MSDettagli.TextMatrix(i, COL_PREZZO)) + (CDbl(MSDettagli.TextMatrix(i, COL_PREZZO) / 100) * CDbl(MSDettagli.TextMatrix(i, COL_VARIAZIONE))), "##,###,###.00######")

        '                Imponibile = CDbl(MSDettagli.TextMatrix(i, COL_PREZZO)) * CDbl(MSDettagli.TextMatrix(i, COL_QTA))
        '                MSDettagli.TextMatrix(i, COL_IMPONIBILE_LORDO) = Format(Imponibile, "##,###,###.00")

        '                Imponibile = CDbl(MSDettagli.TextMatrix(i, COL_PREZZO_NETTO)) * CDbl(MSDettagli.TextMatrix(i, COL_QTA))
        '                MSDettagli.TextMatrix(i, COL_IMPONIBILE_NETTO) = Format(Imponibile, "##,###,###.00")

        '                '                  Sconto = (CDbl(MSDettagli.TextMatrix(i, COL_PREZZO) / 100) * CDbl(MSDettagli.TextMatrix(i, COL_VARIAZIONE))) * CDbl(MSDettagli.TextMatrix(i, COL_QTA))
        '                Sconto = Format(CDbl(MSDettagli.TextMatrix(i, COL_IMPONIBILE_LORDO)) - CDbl(MSDettagli.TextMatrix(i, COL_IMPONIBILE_NETTO)), "##,###,###.00")

        '            Case enum_TipoSconto.Imponibile

        '                MSDettagli.TextMatrix(i, COL_PREZZO) = Format(MSDettagli.TextMatrix(i, COL_IMPONIBILE_LORDO) / CDbl(MSDettagli.TextMatrix(i, COL_QTA)), "##,###,###.00######")

        '                Imponibile = MSDettagli.TextMatrix(i, COL_IMPONIBILE_LORDO)
        '                Imponibile = CDbl(Imponibile) + ((CDbl(Imponibile) / 100) * CDbl(MSDettagli.TextMatrix(i, COL_VARIAZIONE)))
        '                MSDettagli.TextMatrix(i, COL_IMPONIBILE_NETTO) = Format(Imponibile, "##,###,###.00")

        '                MSDettagli.TextMatrix(i, COL_PREZZO_NETTO) = Format(CDbl(Imponibile) / CDbl(MSDettagli.TextMatrix(i, COL_QTA)), "##,###,###.00######")

        '                Sconto = Format(CDbl(MSDettagli.TextMatrix(i, COL_IMPONIBILE_LORDO)) - CDbl(MSDettagli.TextMatrix(i, COL_IMPONIBILE_NETTO)), "##,###,###.00")


        '            Case enum_TipoSconto.Totale


        '                'Calcolo del Totale Imponibile Sconti + Controllo Univocità Sconto
        '                Imponibile = 0
        '                Sconto = 0
        '                For Riga = 1 To MSDettagli.rows - 1
        '                    If CDbl(MSDettagli.TextMatrix(Riga, COL_VARIAZIONE)) <> 0 Then
        '                        Imponibile = Imponibile + CDbl(MSDettagli.TextMatrix(Riga, COL_PREZZO)) * CDbl(MSDettagli.TextMatrix(Riga, COL_QTA))
        '                        If Sconto = 0 Then
        '                            Sconto = CDbl(MSDettagli.TextMatrix(Riga, COL_VARIAZIONE))
        '                        ElseIf Sconto <> CDbl(MSDettagli.TextMatrix(Riga, COL_VARIAZIONE)) Then
        '                            MsgBox("L'applicazione dello sconto a piede fattura è consentito solo in fatture aventi un'unica percentuale di sconto applicata.")
        '                            CmbTipoSconto.ListIndex = 0
        '                            Exit Sub
        '                        End If
        '                    End If

        '                Next Riga

        '                'Sconto Complessivo
        '                ScontoImponibileLordo = Format((CDbl(Imponibile) / 100) * CDbl(MSDettagli.TextMatrix(i, COL_VARIAZIONE)), "##,###,###.00")


        '                'Calcolo dell'imponibile netto
        '                If ScontoImponibileLordo <> 0 Then
        '                    MSDettagli.TextMatrix(i, COL_IMPONIBILE_NETTO) = Format(MSDettagli.TextMatrix(i, COL_IMPONIBILE_LORDO) - (CDbl((MSDettagli.TextMatrix(i, COL_IMPONIBILE_LORDO)) * -ScontoImponibileLordo) / Imponibile), "##,###,###.00")
        '                    Sconto = Format(CDbl((MSDettagli.TextMatrix(i, COL_IMPONIBILE_LORDO) * -ScontoImponibileLordo) / Imponibile), "##,###,###.00#")
        '                Else
        '                    MSDettagli.TextMatrix(i, COL_IMPONIBILE_NETTO) = MSDettagli.TextMatrix(i, COL_IMPONIBILE_LORDO)
        '                    Sconto = 0
        '                End If

        '                MSDettagli.TextMatrix(i, COL_PREZZO_NETTO) = Format(CDbl(MSDettagli.TextMatrix(i, COL_IMPONIBILE_NETTO)) / CDbl(MSDettagli.TextMatrix(i, COL_QTA)), "##,###,###.00######")


        '        End Select


        '        MSIva.rows = MSIva.rows + 1
        '        ''MSIva.MergeRow (MSIva.rows - 1)

        '        MSIva.TextMatrix(MSIva.rows - 1, COL_CODICE) = MSDettagli.TextMatrix(i, COL_CODICE)
        '        MSIva.TextMatrix(MSIva.rows - 1, COL_DESCRIZIONE) = MSDettagli.TextMatrix(i, COL_DESCRIZIONE)
        '        MSIva.TextMatrix(MSIva.rows - 1, COL_COD_IVA) = MSDettagli.TextMatrix(i, COL_COD_IVA)

        '        Select Case CLng(MSDettagli.TextMatrix(i, COL_COD_IVA))

        '            Case 0
        '                MSIva.TextMatrix(MSIva.rows - 1, COL_DES_IVA) = "Non Ivabile"
        '                Iva = 0
        '            Case 1
        '                MSIva.TextMatrix(MSIva.rows - 1, COL_DES_IVA) = "FCI"
        '                Iva = 0
        '            Case 4
        '                MSIva.TextMatrix(MSIva.rows - 1, COL_DES_IVA) = "4%"
        '                Iva = 4
        '            Case 10
        '                MSIva.TextMatrix(MSIva.rows - 1, COL_DES_IVA) = "10%"
        '                Iva = 10
        '            Case 12
        '                MSIva.TextMatrix(MSIva.rows - 1, COL_DES_IVA) = "12%"
        '                Iva = 12
        '            Case 20
        '                MSIva.TextMatrix(MSIva.rows - 1, COL_DES_IVA) = "20%"
        '                Iva = 20
        '        End Select

        '        '            MSIva.TextMatrix(MSIva.rows - 1, COL_IVA) = MSDettagli.TextMatrix(i, COL_COD_IVA)
        '        '            'Ricerca di una riga in tabella avente la stessa aliquota
        '        '            bAliquota = False
        '        '            For Riga = 1 To MSIva.rows - 1
        '        '                If CLng(MSIva.TextMatrix(Riga, COL_COD_IVA)) = CLng(MSDettagli.TextMatrix(i, COL_COD_IVA)) Then
        '        '                   bAliquota = True
        '        '                   Exit For
        '        '                End If
        '        '            Next Riga

        '        Imponibile = Format(MSDettagli.TextMatrix(i, COL_IMPONIBILE_NETTO), "##,###,###.00")

        '        'Nota: Il calcolo viene effettuato su 3 decimali perché + esatto
        '        '            Imposta = Format(Imponibile * (Iva / 100), "##,###,###.00#")
        '        Imposta = Format(MSDettagli.TextMatrix(i, COL_IVA), "##,###,###.00#")


        '        Importo = Format(Imponibile + Imposta, "##,###,###.00#")
        '        MSDettagli.TextMatrix(i, COL_COSTOCOMPLESSIVO) = Format(Importo, "##,###,###.00")

        '        MSIva.TextMatrix(MSIva.rows - 1, COL_PREZZO) = Format(Imponibile, "##,###,###.00")
        '        MSIva.TextMatrix(MSIva.rows - 1, COL_COSTOCOMPLESSIVO) = Format(Imposta, "##,###,###.00#")
        '        MSIva.TextMatrix(MSIva.rows - 1, COL_IVA) = Format(Imposta, "##,###,###.00#")

        '        TxtImponibile.Text = Format(CDbl(TxtImponibile) + CDbl(MSDettagli.TextMatrix(i, COL_IMPONIBILE_LORDO)), "##,###,###.00")
        '        TxtImponibileNetto.Text = Format(CDbl(TxtImponibileNetto) + CDbl(MSDettagli.TextMatrix(i, COL_IMPONIBILE_NETTO)), "##,###,###.00")

        '        'Nota: Il calcolo viene effettuato su 3 decimali perché + esatto
        '        TxtImposta.Text = Format(CDbl(TxtImposta) + Imposta, "##,###,###.00#")
        '        TxtImporto.Text = Format(CDbl(TxtImporto) + CDbl(Importo), "##,###,###.00#")
        '        TxtVariazioni = Format(TxtVariazioni - Sconto, "##,###,###.00#")

        '        CmdPagamenti.enabled = IIf(CDbl(TxtImporto.Text) > 0, True, False)

        '    End If 'qta numerica e prezzo numerico

        'Next  'per ogni dettaglio

        '''Formattazione a 2 decimali
        ''TxtVariazioni = Format(TxtVariazioni, "##,###,###.00")
        ''TxtImposta.Text = Format(TxtImposta, "##,###,###.00")
        ''TxtImporto.Text = Format(TxtImporto, "##,###,###.00")


        '''Ordinamento Griglia Iva
        ''FormOrdinamentoIva()

    End Sub



    '##################################################################################################
    Private Sub Ripristina_Dati_nei_Controlli()

        Dim hash_iva As New Hashtable
        Dim Testo_Combo As String = ""

        v_Tot_Imponibile_Lordo = 0
        v_Tot_Variazioni = 0
        v_Tot_Imponibile_Netto = 0
        v_Tot_Iva = 0
        v_Tot_Importo = 0

        '=======================================================================
        ' LEGGO L'XML DELL'OPERAZIONE
        '=======================================================================

        'Call XML_Leggi_Fattura(hash_iva)
        Leggi_Fattura(hash_iva)

        '=======================================================================
        ' CARICO IL RIEPILOGO DELL'IVA
        '=======================================================================

        Call CaricaGriglia_IVA(hash_iva)


        '=======================================================================
        'CARICO NEI CONTROLLI LE INFORMAZIONI PRELEVATE DALL'XML
        '=======================================================================

        'Salvo i pagamenti nella variabile di sessione
        'perchè altrimenti ho problemi successivamente
        'quando rifaccio il replace

        If Session("Pagamenti") IsNot Nothing Then

        End If
        'Session("XML_Pagamenti") = x_XML_Pagamenti

        'x_XML_Pagamenti = x_XML_Pagamenti.Replace("<", "{")
        'x_XML_Pagamenti = x_XML_Pagamenti.Replace(">", "}")
        'Me.Txt_DatiPagamenti.Text = x_XML_Pagamenti


        Me.Txt_DataEmissione.Text = x_Data_Emissione
        Me.Txt_DataRegistrazione.Text = x_Data_Registrazione

        If x_Scadenza <> "31/12/2100" Then
            Me.Txt_DataScadenza.Text = x_Scadenza
        End If


        Me.Txt_Note.Text = x_Extra_Str
        Me.Txt_Descrizione.Text = x_Mov_Desc_Contabile

        Me.Txt_NumFattura_Sin.Text = x_Doc_Numero_Sin
        Me.Txt_NumFattura.Text = CStr(x_Doc_Numero)
        Me.Txt_NumFattura_Des.Text = x_Doc_Numero_Des




        Me.Txt_Importo.Text = Format(x_Num_Protocollo, "##,###,##0.00")

        Me.Cmb_TipoSconto.SelectedIndex =
            Me.Cmb_TipoSconto.Items.IndexOf(Me.Cmb_TipoSconto.Items.FindByValue(x_Tipo_Sconto))

        Me.Rbl_TipoFattura.SelectedValue = x_Extra_Int

        '===========================================================
        '15/10/2019: è stato gestito lo sblocco in MODIFICA del documento light per fare in modo di inserire la FATTURA DIFFERITA
        '  Me.Rbl_TipoFattura.Enabled = False 'vanni, 17/09/2012
        Dim objconfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim DocumentoRicevutoLight As Boolean
        Dim DocumentoRicevutoLight_str As String = objconfigSiti.Leggi_Valore(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, "DocumentoRicevutoLight", "", "", HttpContext.Current.Session("ASG_objParametri_Server"))
        If DocumentoRicevutoLight_str = "" Then
            DocumentoRicevutoLight = False
        Else
            DocumentoRicevutoLight = DocumentoRicevutoLight_str
        End If
        If DocumentoRicevutoLight Then
            Me.Rbl_TipoFattura.Enabled = True
        Else
            Me.Rbl_TipoFattura.Enabled = False
        End If
        '===========================================================


        ' Dopo che imposto l'Rbl_TipoFattura faccio la CaricaComboXddt
        CaricaComboXddt()   ' nicoletta 26/08/2013
        'vanni, verifica
        If DatiDDT_o_Immediata() Then

            Me.Txt_NaturaBeni.Text = frm_NaturaBeni
            Me.TXT_DataConsegna.Text = frm_Consegna
            Me.Cmb_Aspetto.SelectedIndex = Me.Cmb_Aspetto.Items.IndexOf(Me.Cmb_Aspetto.Items.FindByText(frm_Aspetto))
            Me.Cmb_Causale.SelectedIndex = Me.Cmb_Causale.Items.IndexOf(Me.Cmb_Causale.Items.FindByText(frm_CausaleTrasporto))


            Me.txt_Ora.Text = frm_Ora
            Me.Txt_NumColli.Text = frm_Colli

        End If
        ImpostazioniDocumentoDatoLAV_COD()

        ''Me.Txt_NaturaBeni.Text = frm_NaturaBeni
        ''Me.Txt_Ora.Text = frm_Ora
        ''Me.Txt_NumColli.Text = frm_Colli

        If x_Peso <> 0 Then
            Me.Txt_PesoNetto.Text = CStr(x_Peso)
        Else
            Ricava_PesoNettoTotale()
        End If

        Me.Rbl_Trasporto.SelectedIndex = x_Mezzo


        '=======================================================================
        'Carico le informazioni sul CLIENTE/FORNITORE
        '=======================================================================

        Dim Piva_Contatto As String = ""
        Dim Codice_Combo As String = ""

        Me.Cmb_TipoIndirizzo_Contatto.Enabled = True

        Carica_Contatto(Testo_Combo, x_Cod_RisUm, Piva_Contatto, 0, x_Cod_IndirizzoRisUm)

        Codice_Combo = CStr(x_Cod_RisUm) & "|" & Piva_Contatto

        Me.Cmb_Contatti.Items.Add(New ListItem(Testo_Combo, Codice_Combo))

        Me.Cmb_Contatti.SelectedIndex = Me.Cmb_Contatti.Items.IndexOf(Me.Cmb_Contatti.Items.FindByValue(Codice_Combo))

        'If VerificaEsistenza_CodContatto_as_PivaGIAS(Server, Session, Page, x_Cod_RisUm) = True Then
        '    Me.Cmb_Contatti.Enabled = False
        'Else
        '    Me.Cmb_Contatti.Enabled = True
        'End If

        'in modifica blocco sempre il contatto
        Me.Cmb_Contatti.Enabled = False

        '=======================================================================
        'Carico le informazioni sul DESTINATARIO DIVERSO
        '=======================================================================

        If x_Cod_Destinazione <> 0 Then

            'Me.Cmb_DestinatarioDiverso.Enabled = True
            'Me.Txt_Cerca_Diverso.Enabled = True
            'Me.Img_btn_CercaDiverso.Enabled = True

            'Me.Txt_Cerca_Diverso2.Enabled = True
            'Me.Img_btn_CercaDiverso2.Enabled = True
            'Me.Cmb_TipoIndirizzo_DestDiverso.Enabled = True

            Piva_Contatto = ""

            Carica_DestinatarioDiverso(Testo_Combo, x_Cod_Destinazione, Piva_Contatto, 0, x_Cod_IndirizzoDestinazione)

            ''modifica del 18/09/2012: non carico tutto
            'CaricaCombo_Contatti(Server, Session, Page, Me.Cmb_DestinatarioDiverso, _
            '                    Qs_Piva, _
            '                    CStr(Session("ASG_SuperUser_CodFiscale")), _
            '                    , _
            '                    , _
            '                    , _
            '                    , _
            '                    , _
            '                    "", _
            '                    CDate(Qs_DataSelezionata), _
            '                    CDate(Qs_DataSelezionata), _
            '                    v_Flag_AncheImportati, _
            '                    False)

            Codice_Combo = CStr(x_Cod_Destinazione) & "|" & Piva_Contatto

            Me.Cmb_DestinatarioDiverso.Items.Add(New ListItem(Testo_Combo, Codice_Combo))

            Me.Cmb_DestinatarioDiverso.SelectedIndex = Me.Cmb_DestinatarioDiverso.Items.IndexOf(Me.Cmb_DestinatarioDiverso.Items.FindByValue(Codice_Combo))

        Else

            'Me.Chk_DestinatarioDiverso.Checked = False
            'Me.Cmb_TipoIndirizzo_DestDiverso.Enabled = False
            'Me.Cmb_DestinatarioDiverso.Enabled = False
            'Me.Txt_CodIndirizzo_DestDiverso.Text = ""
            'Me.Txt_CodContatto_DestDiverso.Text = ""

        End If


        '=======================================================================
        'Carico le informazioni sul VETTORE
        '=======================================================================

        If x_Cod_Vettore <> 0 Then

            Me.Pannello_Vettore.Visible = True

            Me.Cmb_Vettore.Enabled = True
            Me.Cmb_TipoIndirizzo_Vettore.Enabled = True

            Piva_Contatto = ""

            ''modifica del 18/09/2012: non carico tutto
            Carica_Vettore(Testo_Combo, x_Cod_Vettore, Piva_Contatto, 0, x_Cod_IndirizzoVettore)

            ''carico la combo del vettore e ci carico tutti i contatti
            'CaricaCombo_Contatti(Server, Session, Page, Me.Cmb_Vettore, _
            '                        Qs_Piva, _
            '                        CStr(Session("ASG_SuperUser_CodFiscale")), _
            '                        , _
            '                        , _
            '                        , _
            '                        1, _
            '                        , _
            '                        "", _
            '                        CDate(Qs_DataSelezionata), _
            '                        CDate(Qs_DataSelezionata), _
            '                        v_Flag_AncheImportati, _
            '                        False)


            Codice_Combo = CStr(x_Cod_Vettore) & "|" & Piva_Contatto

            Me.Cmb_Vettore.Items.Add(New ListItem(Testo_Combo, Codice_Combo))

            Me.Cmb_Vettore.SelectedIndex = Me.Cmb_Vettore.Items.IndexOf(Me.Cmb_Vettore.Items.FindByValue(Codice_Combo))

        Else

            'Me.Cmb_TipoIndirizzoVettore.Enabled = False
            'Me.Cmb_Vettore.Enabled = False

            'Me.Txt_CodIndirizzoVettore.Text = ""
            'Me.Txt_CodContattoVettore.Text = ""

        End If

        ''=======================================================================
        ''Carico le informazioni sul DETTAGLIO DEL MEZZO
        ''=======================================================================

        'Select Case x_Mac_Cod

        '    Case 0

        '        Me.Chk_ParcoMacchine.Checked = False
        '        Me.Txt_Targa.Text = frm_Targa
        '        Me.Txt_Immatr.Text = frm_Immatricolazione
        '        Me.Txt_ImmatrRimorchio.Text = frm_ImmatrRimorchio
        '        Me.Txt_Autorizz.Text = frm_Autorizzizzazione
        '        Me.Txt_DataAutorizz.Text = frm_Data_Autorizzazione
        '        Me.Txt_PesoTara.Text = frm_PesoTara

        '    Case Else

        '        Me.Chk_ParcoMacchine.Checked = True
        '        Chk_ParcoMacchine_CheckedChanged(Me, Nothing)

        '        Me.Cmb_Targa.SelectedIndex = Me.Cmb_Targa.Items.IndexOf(Me.Cmb_Targa.Items.FindByValue(frm_Mac_Cod))
        '        Cmb_Targa_SelectedIndexChanged(Me, Nothing)

        'End Select

        Visualizza_RiepilogoDocumento()


    End Sub

    '##################################################################################################
    Private Sub Ripristina_Dati_nei_Controlli_Da_AgendaSessione(ByVal AgendaScarico As Operazione_Agenda)

        Dim hash_iva As New Hashtable
        Dim Testo_Combo As String = ""

        v_Tot_Imponibile_Lordo = 0
        v_Tot_Variazioni = 0
        v_Tot_Imponibile_Netto = 0
        v_Tot_Iva = 0
        v_Tot_Importo = 0

        '=======================================================================
        ' LEGGO L'XML DELL'OPERAZIONE
        '=======================================================================

        'Call XML_Leggi_Fattura(hash_iva)
        Leggi_Fattura_Da_Sessione(AgendaScarico, hash_iva)

        '=======================================================================
        ' CARICO IL RIEPILOGO DELL'IVA
        '=======================================================================

        Call CaricaGriglia_IVA(hash_iva)


        '=======================================================================
        'CARICO NEI CONTROLLI LE INFORMAZIONI PRELEVATE DALL'XML
        '=======================================================================

        Me.Txt_DataEmissione.Text = x_Data_Emissione
        Me.Txt_DataRegistrazione.Text = x_Data_Registrazione

        If x_Scadenza <> "31/12/2100" Then
            Me.Txt_DataScadenza.Text = x_Scadenza
        End If


        Me.Txt_Note.Text = x_Extra_Str
        Me.Txt_Descrizione.Text = x_Mov_Desc_Contabile

        Me.Txt_NumFattura_Sin.Text = x_Doc_Numero_Sin
        Me.Txt_NumFattura.Text = CStr(x_Doc_Numero)
        Me.Txt_NumFattura_Des.Text = x_Doc_Numero_Des

        Me.Txt_Importo.Text = Format(x_Num_Protocollo, "##,###,##0.00")

        Me.Cmb_TipoSconto.SelectedIndex =
            Me.Cmb_TipoSconto.Items.IndexOf(Me.Cmb_TipoSconto.Items.FindByValue(x_Tipo_Sconto))

        Me.Rbl_TipoFattura.SelectedValue = x_Extra_Int
        Me.Rbl_TipoFattura.Enabled = False 'vanni, 17/09/2012
        ' Dopo che imposto l'Rbl_TipoFattura faccio la CaricaComboXddt
        CaricaComboXddt()   ' nicoletta 26/08/2013
        'vanni, verifica
        If DatiDDT_o_Immediata() Then

            Me.Txt_NaturaBeni.Text = frm_NaturaBeni
            Me.TXT_DataConsegna.Text = frm_Consegna
            Me.Cmb_Aspetto.SelectedIndex = Me.Cmb_Aspetto.Items.IndexOf(Me.Cmb_Aspetto.Items.FindByText(frm_Aspetto))
            Me.Cmb_Causale.SelectedIndex = Me.Cmb_Causale.Items.IndexOf(Me.Cmb_Causale.Items.FindByText(frm_CausaleTrasporto))

            Me.txt_Ora.Text = frm_Ora
            Me.Txt_NumColli.Text = frm_Colli

        End If

        ImpostazioniDocumentoDatoLAV_COD()

        If x_Peso <> 0 Then
            Me.Txt_PesoNetto.Text = CStr(x_Peso)
        Else
            Ricava_PesoNettoTotale()
        End If

        Me.Rbl_Trasporto.SelectedIndex = x_Mezzo

        '=======================================================================
        'Carico le informazioni sul CLIENTE/FORNITORE
        '=======================================================================

        Dim Piva_Contatto As String = ""
        Dim Codice_Combo As String = ""

        Me.Cmb_TipoIndirizzo_Contatto.Enabled = True

        Carica_Contatto(Testo_Combo, x_Cod_RisUm, Piva_Contatto, 0, x_Cod_IndirizzoRisUm)

        Codice_Combo = CStr(x_Cod_RisUm) & "|" & Piva_Contatto

        Me.Cmb_Contatti.Items.Add(New ListItem(Testo_Combo, Codice_Combo))

        Me.Cmb_Contatti.SelectedIndex = Me.Cmb_Contatti.Items.IndexOf(Me.Cmb_Contatti.Items.FindByValue(Codice_Combo))

        'in modifica blocco sempre il contatto
        Me.Cmb_Contatti.Enabled = False

        '=======================================================================
        'Carico le informazioni sul DESTINATARIO DIVERSO
        '=======================================================================

        If x_Cod_Destinazione <> 0 Then

            Piva_Contatto = ""

            Carica_DestinatarioDiverso(Testo_Combo, x_Cod_Destinazione, Piva_Contatto, 0, x_Cod_IndirizzoDestinazione)

            Codice_Combo = CStr(x_Cod_Destinazione) & "|" & Piva_Contatto

            Me.Cmb_DestinatarioDiverso.Items.Add(New ListItem(Testo_Combo, Codice_Combo))

            Me.Cmb_DestinatarioDiverso.SelectedIndex = Me.Cmb_DestinatarioDiverso.Items.IndexOf(Me.Cmb_DestinatarioDiverso.Items.FindByValue(Codice_Combo))


        End If


        '=======================================================================
        'Carico le informazioni sul VETTORE
        '=======================================================================

        If x_Cod_Vettore <> 0 Then

            Me.Pannello_Vettore.Visible = True

            Me.Cmb_Vettore.Enabled = True
            Me.Cmb_TipoIndirizzo_Vettore.Enabled = True

            Piva_Contatto = ""

            ''modifica del 18/09/2012: non carico tutto
            Carica_Vettore(Testo_Combo, x_Cod_Vettore, Piva_Contatto, 0, x_Cod_IndirizzoVettore)

            Codice_Combo = CStr(x_Cod_Vettore) & "|" & Piva_Contatto

            Me.Cmb_Vettore.Items.Add(New ListItem(Testo_Combo, Codice_Combo))

            Me.Cmb_Vettore.SelectedIndex = Me.Cmb_Vettore.Items.IndexOf(Me.Cmb_Vettore.Items.FindByValue(Codice_Combo))

        End If

        Visualizza_RiepilogoDocumento()


        'disattivo pulsantiera dettagli e salva e nuovo e nascondo colonne x editare dettagli
        Disattiva_Pulsanti()
        ImgBtn_Salva.Visible = True

        Me.DataGrid_Prodotti.Columns.Item(COL_MODIFICA).Visible = False
        Me.DataGrid_Prodotti.Columns.Item(COL_ELIMINA).Visible = False


    End Sub

    Private Function DatiDDT_o_Immediata() As Boolean
        Return QS_LavCod = LAVCOD_BOLLA_EMESSA OrElse
               QS_LavCod = LAVCOD_BOLLA_RICEVUTA OrElse
               (QS_LavCod = LAVCOD_FATTURA_RICEVUTA AndAlso x_Extra_Int = 1) OrElse
               (QS_LavCod = LAVCOD_FATTURA_EMESSA AndAlso x_Extra_Int = 1)
    End Function


    '##################################################################################################
    Private Sub Visualizza_RiepilogoDocumento()

        '=======================================================================
        'Carico il riepilogo del documento
        '=======================================================================

        If v_Tot_Importo = 0 Then
            v_Tot_Importo = v_Tot_Imponibile_Netto + v_Tot_Iva
        End If

        Me.Txt_ImponibileLordo.Text = Format(v_Tot_Imponibile_Lordo, "#,###,##0.00")
        Me.Txt_Variazioni.Text = Format(v_Tot_Variazioni, "#,###,##0.00")
        Me.Txt_ImponibileNetto.Text = Format(v_Tot_Imponibile_Netto, "#,###,##0.00")
        Me.Txt_IVA.Text = Format(v_Tot_Iva, "#,###,##0.00")
        Me.Txt_Importo.Text = Format(v_Tot_Importo, "#,###,##0.00")

    End Sub


    '##################################################################################################
    Private Sub Recupera_Dati_dai_Controlli()


        Dim x_DesBollaFatt As String = ""
        Select Case QS_LavCod
            Case LAVCOD_BOLLA_EMESSA, LAVCOD_BOLLA_RICEVUTA
                x_DesBollaFatt = "Bolla"
            Case LAVCOD_FATTURA_EMESSA, LAVCOD_FATTURA_RICEVUTA
                x_DesBollaFatt = "Fattura"
            Case LAVCOD_NOTA_ACCREDITO_EMESSA, LAVCOD_NOTA_ACCREDITO_RICEVUTA
                x_DesBollaFatt = "Nota di Accredito"
        End Select

        x_Des_Lib = x_DesBollaFatt

        x_Mov_Desc_Contabile = Me.Txt_Descrizione.Text
        x_Extra_Str = Me.Txt_Note.Text

        x_Doc_Numero_Sin = Me.Txt_NumFattura_Sin.Text
        x_Doc_Numero = CDbl(Me.Txt_NumFattura.Text)
        x_Doc_Numero_Des = Me.Txt_NumFattura_Des.Text

        x_Data_Emissione = Me.Txt_DataEmissione.Text

        x_Data_Registrazione = Me.Txt_DataRegistrazione.Text

        If Me.Txt_DataScadenza.Text <> "" Then
            x_Scadenza = Me.Txt_DataScadenza.Text
        Else
            x_Scadenza = AGRODATAFINE
        End If

        x_Extra_Date = AGRODATAFINE

        Select Case QS_LavCod

            'todo, verifica
            Case LAVCOD_FATTURA_EMESSA, LAVCOD_BOLLA_EMESSA

                x_Cau_Mov_Magazzino = CAU_SCARICO

                x_Des_Lib &= " Emessa (n. " & x_Doc_Numero_Sin & CStr(x_Doc_Numero) & x_Doc_Numero_Des & " Rif: " &
                                                   Split(Me.Cmb_Contatti.SelectedItem.Text, "(")(0) & ")"

                x_Mov_Desc_Magazzino &= "Scarico di articoli relativi alla " & x_DesBollaFatt & " n. " &
                                            x_Doc_Numero_Sin & CStr(x_Doc_Numero) & x_Doc_Numero_Des & " del " & x_Data_Emissione

            Case LAVCOD_FATTURA_RICEVUTA, LAVCOD_BOLLA_RICEVUTA

                x_Cau_Mov_Magazzino = CAU_CARICO

                x_Des_Lib &= " Ricevuta (n. " & x_Doc_Numero_Sin & CStr(x_Doc_Numero) & x_Doc_Numero_Des & " Rif: " &
                                                              Split(Me.Cmb_Contatti.SelectedItem.Text, "(")(0) & ")"

                x_Mov_Desc_Magazzino &= "Carico di articoli relativi alla " & x_DesBollaFatt & " n. " &
                                            x_Doc_Numero_Sin & CStr(x_Doc_Numero) & x_Doc_Numero_Des & " del " & x_Data_Emissione

                'todo, verifica
            Case LAVCOD_NOTA_ACCREDITO_EMESSA

                x_Cau_Mov_Magazzino = CAU_CARICO

                x_Des_Lib &= " Emessa (n. " & x_Doc_Numero_Sin & CStr(x_Doc_Numero) & x_Doc_Numero_Des & " Rif: " &
                                                   Split(Me.Cmb_Contatti.SelectedItem.Text, "(")(0) & ")"

                x_Mov_Desc_Magazzino &= "Carico di articoli relativi alla " & x_DesBollaFatt & " n. " &
                                            x_Doc_Numero_Sin & CStr(x_Doc_Numero) & x_Doc_Numero_Des & " del " & x_Data_Emissione

            Case LAVCOD_NOTA_ACCREDITO_RICEVUTA

                x_Cau_Mov_Magazzino = CAU_SCARICO

                x_Des_Lib &= " Ricevuta (n. " & x_Doc_Numero_Sin & CStr(x_Doc_Numero) & x_Doc_Numero_Des & " Rif: " &
                                                   Split(Me.Cmb_Contatti.SelectedItem.Text, "(")(0) & ")"

                x_Mov_Desc_Magazzino &= "Scarico di articoli relativi alla " & x_DesBollaFatt & " n. " &
                                            x_Doc_Numero_Sin & CStr(x_Doc_Numero) & x_Doc_Numero_Des & " del " & x_Data_Emissione


        End Select

        If Me.Txt_Importo.Text <> "" Then
            x_Num_Protocollo = CDbl(Me.Txt_Importo.Text)
        Else
            x_Num_Protocollo = CDbl(0)
        End If

        x_Tipo_Sconto = Me.Cmb_TipoSconto.SelectedValue

        x_Extra_Int = Me.Rbl_TipoFattura.SelectedValue


        x_Mezzo = Me.Rbl_Trasporto.SelectedIndex

        If IsNumeric(Me.Txt_PesoNetto.Text) Then
            x_Peso = CDbl(Me.Txt_PesoNetto.Text)
        Else
            x_Peso = 0
        End If

        If DatiDDT_o_Immediata() Then

            frm_CausaleTrasporto = Me.Cmb_Causale.SelectedItem.Text
            frm_Aspetto = Me.Cmb_Aspetto.SelectedItem.Text

            If Me.Txt_NumColli.Text <> "" Then
                frm_Colli = CInt(Me.Txt_NumColli.Text)
            Else
                frm_Colli = 0I
            End If

            frm_NaturaBeni = Me.Txt_NaturaBeni.Text
            frm_Consegna = Me.TXT_DataConsegna.Text

            If Txt_PesoTara.Text <> "" Then
                frm_TaraVeicolo = CDbl(Me.Txt_PesoTara.Text)
            Else
                frm_TaraVeicolo = 0.0R
            End If

        End If


        'x_CausaleTrasporto = ""
        'x_Aspetto = ""
        'x_Colli = 0

        'CLIENTE / FORNITORE
        x_Cod_RisUm = CInt(Split(Me.Cmb_Contatti.SelectedValue, "|")(0))

        If CStr(Me.Txt_CodIndirizzo_Contatto.Text) <> "" Then
            x_Cod_IndirizzoRisUm = CInt(Me.Txt_CodIndirizzo_Contatto.Text)
        Else
            x_Cod_IndirizzoRisUm = 0
        End If

        'DESTINATARIO diverso
        If Cmb_DestinatarioDiverso IsNot Nothing AndAlso Me.Cmb_DestinatarioDiverso.SelectedValue <> "" Then

            x_Cod_Destinazione = CInt(Split(Me.Cmb_DestinatarioDiverso.SelectedItem.Value, "|")(0))

            If CStr(Me.Txt_CodIndirizzo_DestDiverso.Text) <> "" Then
                x_Cod_IndirizzoDestinazione = CInt(Me.Txt_CodIndirizzo_DestDiverso.Text)
            Else
                x_Cod_IndirizzoDestinazione = 0
            End If

        Else
            x_Cod_Destinazione = 0
            x_Cod_IndirizzoDestinazione = 0
        End If


        'VETTORE
        If x_Mezzo = 2 AndAlso Me.Cmb_Vettore.SelectedValue <> "" Then

            x_Cod_Vettore = CInt(Split(Me.Cmb_Vettore.SelectedValue, "|")(0))

            If CStr(Me.Txt_CodIndirizzo_Vettore.Text) <> "" Then
                x_Cod_IndirizzoVettore = CInt(Me.Txt_CodIndirizzo_Vettore.Text)
            Else
                x_Cod_IndirizzoVettore = 0
            End If

        Else
            x_Cod_Vettore = 0
            x_Cod_IndirizzoVettore = 0
        End If


        ''------------------------------------------
        '' DETTAGLIO MEZZO DI TRASPORTO
        'Select Case Me.Rbl_Trasporto.SelectedValue
        '    Case "Vettore"
        '        frm_Cod_RisUm_Extra = frm_CodVettore
        '    Case False
        '        frm_Cod_RisUm_Extra = 0
        'End Select

        'Select Case Me.Chk_ParcoMacchine.Checked
        '    Case True
        '        frm_Mac_Cod = Me.Cmb_Targa.SelectedValue
        '        frm_Data_Autorizzazione = CStr(Date.Today) 'perchè????
        '        frm_PesoTara = 0 'perchè?????
        '    Case False
        '        frm_Mac_Cod = 0
        '        frm_Data_Autorizzazione = Me.Txt_DataAutorizz.Text
        '        If Me.Txt_PesoTara.Text = "" Then
        '            frm_PesoTara = 0
        '        Else
        '            frm_PesoTara = Me.Txt_PesoTara.Text
        '        End If
        'End Select

        frm_PesoTara = 0
        If IsNumeric(Txt_PesoTara.Text) Then
            frm_PesoTara = CDbl(Txt_PesoTara.Text)
        End If

        frm_Data_Autorizzazione = #1/1/1900#
        If IsDate(Txt_DataAutorizz.Text) Then
            frm_Data_Autorizzazione = CDate(Txt_DataAutorizz.Text)
        End If

        frm_Targa = Me.Txt_Targa.Text
        frm_Immatricolazione = Me.Txt_Immatr.Text
        frm_ImmatrRimorchio = Me.Txt_ImmatrRimorchio.Text
        frm_Autorizzizzazione = Me.Txt_Autorizz.Text


    End Sub


    ''########################################################################################
    'Private Function XML_Genera_StringoneFinale() As String

    '    Dim Str_Xml_Pagamenti = ""
    '    'Dim Str_Xml_Mov_Dettagli = ""
    '    Dim XmlDoc, XmlTemp As XmlDocument
    '    Dim XmlDatiMovDettagli As mlElement
    '    Dim XmlDatiPagamenti, XmlDatiPagamentiTemp As XmlElement

    '    Dim XML_DatiMovTecniciExtra As XmlElement
    '    Dim XML_MovTecniciExtra As XmlElement


    '    '------------------------------------------------
    '    '----- Calcolo i valori di BaseCode e TopCode
    '    '------------------------------------------------

    '    Call Calcola_BaseCode_TopCode(x_BaseCode, _
    '                                  x_TopCode, _
    '                                  Session("ASG_ProgressivoGIAS").ToString)


    '    '------------------------------------------------
    '    '----- Recupero dai dai controlli
    '    '------------------------------------------------

    '    Recupera_Dati_dai_Controlli()

    '    'Ho salvato i pagamenti nella variabile di sessione
    '    'perchè altrimenti ho problemi 
    '    'quando rifaccio il replace

    '    'Dim Temp As String
    '    'Temp = Me.Txt_DatiPagamenti.Text

    '    'Str_Xml_Pagamenti = CStr(Temp.Replace("{", "<"))
    '    'Str_Xml_Pagamenti = CStr(Str_Xml_Pagamenti.Replace("}", ">"))

    '    'Str_Xml_Pagamenti = CStr(Session("XML_Pagamenti"))

    '    'Str_Xml_Pagamenti = ""



    '    ''------------------------------------------------
    '    ''----- creo l'xml
    '    ''------------------------------------------------

    '    'XmlDoc = New XmlDocument

    '    'If Str_Xml_Pagamenti <> "" Then

    '    '    XmlTemp = New XmlDocument

    '    '    XmlTemp.Load(Str_Xml_Pagamenti)

    '    '    XmlDatiPagamenti = XmlDoc.ImportNode(XmlDatiPagamentiTemp, True)

    '    '    XmlTemp = Nothing
    '    '    XmlDatiPagamentiTemp = Nothing

    '    'End If

    '    XmlDatiMovDettagli = XML_2_Contabilita_Fattura_Dettagli(XmlDoc, _
    '                                                            Session("DT_Prodotti_nel_Doc"), _
    '                                                            Qs_Piva, _
    '                                                            Qs_SaCod, _
    '                                                            QS_LavCod, _
    '                                                            x_Cau_Mov_Magazzino, _
    '                                                            x_Data_Emissione, _
    '                                                            x_BaseCode, _
    '                                                            x_TopCode)

    '    XML_2_Contabilita_Fattura(XmlDoc, enum_TipoOperazioneDB.Scrittura, Qs_Piva, Qs_SaCod, QS_LavCod, x_Des_Lib, x_Cau_Mov_Magazzino, x_Mov_Desc_Contabile, x_Mov_Desc_Magazzino, x_Extra_Str, x_Num_Protocollo, x_Doc_Numero_Sin, x_Doc_Numero, x_Doc_Numero_Des, x_Tipo_Sconto, x_Extra_Int, x_Peso, x_Cod_RisUm, x_Cod_IndirizzoRisUm, x_Cod_Destinazione, x_Cod_IndirizzoDestinazione, x_Mezzo, x_Cod_Vettore, x_Cod_IndirizzoVettore, x_Data_Emissione, x_Data_Registrazione, x_Scadenza, x_Extra_Date, XmlDatiPagamenti, XmlDatiMovDettagli, x_BaseCode, x_TopCode, _
    '                              AGRODATAINIZIO, frm_Colli, frm_Aspetto, frm_CausaleTrasporto, frm_NaturaBeni, 0, frm_TaraVeicolo, 0.0R, 0, "")


    '    ''TODO, verificare 
    '    ''##########################################################
    '    ''################  MOVIMENTO TECNICO EXTRA   ##############
    '    ''##########################################################

    '    'XML_DatiMovTecniciExtra = XmlDoc.CreateElement("DatiMov_Dettagli_Tecnici_Extra")

    '    'XML_MovimentoContabile.AppendChild(XML_DatiMovTecniciExtra)


    '    'XML_MovTecniciExtra = XML_2_Agenda_Mov_Dettagli_Tecnici_Extra( _
    '    '                                  enum_TipoOperazioneDB.Scrittura, _
    '    '                                  Qs_Piva, _
    '    '                                  0, _
    '    '                                  0, _
    '    '                                  0, _
    '    '                                  0, _
    '    '                                  0, _
    '    '                                  "000", _
    '    '                                  "", _
    '    '                                  "", _
    '    '                                  "", _
    '    '                                  0, _
    '    '                                  0, _
    '    '                                  "", _
    '    '                                  "", _
    '    '                                  frm_Targa, _
    '    '                                  frm_Immatricolazione, _
    '    '                                  frm_ImmatrRimorchio, _
    '    '                                  frm_Autorizzizzazione, _
    '    '                                  frm_Data_Autorizzazione, _
    '    '                                  frm_PesoTara, _
    '    '                                  x_Data_Emissione, _
    '    '                                  , _
    '    '                                  x_BaseCode, _
    '    '                                  x_TopCode, _
    '    '                                  XmlDoc)


    '    'XML_DatiMovTecniciExtra.AppendChild(XML_MovTecniciExtra)

    '    Return XmlDoc.OuterXml

    'End Function




    '##########################################################
    Private Sub Salva_Tutto(ByRef Messaggio As String)

        Try


            '---------------------------------------------------------------------------------------
            '----- Verifico l'operazione richiesta (Inserimento / Modifica / Cancellazione / Lettura)
            '---------------------------------------------------------------------------------------

            Verifica_Controlli_Testata(Messaggio)

            Verifica_Controlli_Dettagli(Messaggio)

            If Messaggio <> "" Then
                Exit Sub
            End If


            Try

                '------------------------------------------------
                '----- Genero l'XML
                '------------------------------------------------

                'StringaXmlCreazione = XML_Genera_StringoneFinale()

                Dim Agenda As Operazione_Agenda
                Agenda = CreaOggettoAgenda()

                If IsNothing(Agenda) Then
                    Throw New Exception(Resources.AgronicaAgenda_2010.NonÈStatoPossibileCreareLOperazione)
                End If

                Dim objAgendaScrivi As New Agenda_Operazione_Helper
                Dim Id_Agenda As Integer = 0

                If Qs_Operazione = enum_TipoOperazioneDB.Modifica Then

                    Dim CancellataOperazione As Boolean = False
                    CancellataOperazione = objAgendaScrivi.Cancella(Qs_Piva,
                                                                    Qs_SaCod,
                                                                    Qs_IdAgenda, False,
                                                                     objParametri_Server, logCancellazione:=False)
                End If

                Id_Agenda = objAgendaScrivi.Scrivi(Agenda, objParametri_Server)

                'SALVO LE OPERAZIONI COLLEGATE
                'E I RIFERIMENTI
                If Session("ListaOpAgenda") IsNot Nothing Then

                    Dim objRifScrivi As New Agenda_Movimenti_Dettagli_Riferimenti_Helper

                    Dim ListaOpAgenda As List(Of Operazione_Agenda)
                    ListaOpAgenda = Session("ListaOpAgenda")

                    Dim Id_Agenda_Rif As Integer

                    For i = 0 To ListaOpAgenda.Count - 1

                        Id_Agenda_Rif = objAgendaScrivi.Scrivi(ListaOpAgenda(i), objParametri_Server)

                        'scrivo i riferimenti
                        Dim RifAg As New Movimento_Dettaglio_Riferimento
                        'operazione
                        RifAg.Piva = ListaOpAgenda(i).Piva
                        RifAg.Sa_Cod = ListaOpAgenda(i).Sa_Cod
                        RifAg.Id_Agenda = Id_Agenda_Rif
                        RifAg.Lav_Cod = ListaOpAgenda(i).Lav_Cod

                        'ddt
                        RifAg.Piva_Rif = Agenda.Piva
                        RifAg.Sa_Cod_Rif = Agenda.Sa_Cod
                        RifAg.Id_Agenda_Rif = Id_Agenda
                        RifAg.Lav_Cod_Rif = Agenda.Lav_Cod

                        objRifScrivi.Scrivi(RifAg, objParametri_Server)

                        If Not IsNothing(Session("UtilizzataRicetta")) AndAlso Session("UtilizzataRicetta") Then

                            Dim ricetta_cod As String = Session("ricetta_cod")
                            Dim Ricetta_Operazione_Cod As String = Session("Ricetta_Operazione_Cod")

                            Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                            Dim Impostazione_RicetteXagenda As String = objUtenti.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_RiferimentoRicetteOperazioni,
                                                                        HttpContext.Current.Session("ASG_objParametri_Utenti"),
                                                                            1)
                            If Impostazione_RicetteXagenda <> "0" Then
                                Dim objRicetta As New AgronicaCoreContabDAL.RicettexAgenda_W
                                If Not objRicetta.Scrivi(ricetta_cod, Ricetta_Operazione_Cod, Id_Agenda_Rif, AGRODATAINIZIO, AGRODATAFINE, objParametri_Server) Then
                                    Throw New Exception(Resources.AgronicaAgenda_2010.BNonÈRiuscitoLAggancioDellaRicettaBBr)
                                End If
                            End If

                            Session("UtilizzataRicetta") = False
                            Session("ricetta_cod") = Nothing
                            Session("Ricetta_Operazione_Cod") = Nothing

                        End If

                    Next

                End If




            Catch ex As Exception
                Messaggio &= "Si è verificato un errore durante il salvataggio: " & ex.Message
            End Try


        Catch exc As Exception

            '------------------------------------------------
            'Si e' verificata una eccezione !!!!!!
            '------------------------------------------------

            'Messaggio di errore
            Messaggio &= "Si e' verificato un'errore : " &
                        Chr(13) &
                        exc.Message.ToString()

            '------------------------------------------------

        End Try

        If Messaggio = "" Then

            EseguitaOperazione = True

        Else
            'Messaggio di errore
            Messaggio = "Si e' verificato un'errore : " &
                        Chr(13) &
                        Messaggio

            'Visualizzo il messaggio di errore
            'Call Messaggi.AgroMsgBox(Messaggio, Page)

        End If



    End Sub

    '#############################################
    Private Sub FiltraComboCessionario(ByVal textBox_Ricerca As TextBox,
                                       ByVal rag_soc_0_piva_1 As Integer)

        If Not Me.Cmb_Contatti.Enabled Then
            'sono in modifica,
            'il contatto è bloccato finché ci sono dettagli
        Else

            Dim filtro As String = ""

            Dim Flag_AncheImportati As Boolean

            Select Case QS_LavCod
                Case CStr(LAVCOD_FATTURA_RICEVUTA), CStr(LAVCOD_BOLLA_RICEVUTA), CStr(LAVCOD_NOTA_ACCREDITO_RICEVUTA)
                    filtro &= "  (Rapporti_Contabili.Fornitore = 1 OR Rapporti_Contabili.Terzista = 1) "
                Case CStr(LAVCOD_FATTURA_EMESSA), CStr(LAVCOD_BOLLA_EMESSA), CStr(LAVCOD_NOTA_ACCREDITO_EMESSA)
                    filtro &= "  (Rapporti_Contabili.Cliente = 1 OR Rapporti_Contabili.Terzista = 1 ) "


            End Select

            If rag_soc_0_piva_1 = 0 Then
                filtro = String.Format("{0} AND (Contatti.rag_soc like '%{1}%' OR Contatti.nome like '%{1}%' OR Contatti.cognome like '%{1}%') ",
                                    filtro,
                                    UtilityProvider.Agro_SQL_SaveText(textBox_Ricerca.Text))
            Else
                filtro = String.Format("{0} AND Contatti.cod_contatto like '%{1}%' ",
                                    filtro,
                                    UtilityProvider.Agro_SQL_SaveText(textBox_Ricerca.Text))
            End If

            If CInt(Qs_Operazione) = enum_TipoOperazioneDB.Lettura Then
                Flag_AncheImportati = True
            End If

            CaricaCombo_Contatti(objParametri_Server, Session, Page,
                                    Me.Cmb_Contatti,
                                    Qs_Piva,
                                    CStr(Session("ASG_SuperUser_CodFiscale")),
                                    ,
                                    ,
                                    ,
                                    ,
                                    ,
                                    filtro,
                                    CDate(Qs_DataSelezionata),
                                    CDate(Qs_DataSelezionata),
                                    Flag_AncheImportati,
                                    False)

            If textBox_Ricerca.Text <> "" AndAlso Me.Cmb_Contatti.Items.Count > 1 Then
                Me.Cmb_Contatti.SelectedIndex = 1
                Cmb_Contatti_SelectedIndexChanged(Me, Nothing)
            End If

        End If

    End Sub



    '#################################################
    Private Sub FiltraComboCessionarioDiverso(ByVal textBox_Ricerca As TextBox,
                                              ByVal Rag_Soc_0_Piva_1 As Integer)

        Dim filtro As String = ""

        'Select Case QS_LavCod
        '    Case CStr(LAVCOD_BOLLA_RICEVUTA)
        '        filtro &= " (Rapporti_Contabili.Fornitore = 1 OR Rapporti_Contabili.Terzista = 1) "
        '    Case CStr(LAVCOD_BOLLA_EMESSA)
        '        filtro &= " (Rapporti_Contabili.Cliente = 1 OR Rapporti_Contabili.Terzista = 1 ) "
        '    Case CStr(LAVCOD_FATTURA_EMESSA)
        '        filtro &= " (Rapporti_Contabili.Cliente = 1 OR Rapporti_Contabili.Terzista = 1 ) "
        '    Case CStr(LAVCOD_FATTURA_RICEVUTA)
        '        filtro &= " (Rapporti_Contabili.Fornitore = 1 OR Rapporti_Contabili.Terzista = 1) "
        'End Select

        filtro &= " (Rapporti_Contabili.Fornitore = 1 OR Rapporti_Contabili.cliente = 1 OR Rapporti_Contabili.dipendente = 1 OR Rapporti_Contabili.Terzista = 1) "

        If Rag_Soc_0_Piva_1 = 0 Then
            filtro &= String.Format("AND (Contatti.rag_soc like '%{0}%' OR Contatti.nome like '%{0}%' OR Contatti.cognome like '%{0}%') ",
                                UtilityProvider.Agro_SQL_SaveText(textBox_Ricerca.Text))
        Else
            filtro &= String.Format("AND Contatti.cod_contatto like '%{0}%' ", UtilityProvider.Agro_SQL_SaveText(textBox_Ricerca.Text))
        End If


        'carico la combo dei contatti e ci carico tutti i contatti
        Call CaricaCombo_Contatti(objParametri_Server, Session, Page,
                                    Me.Cmb_DestinatarioDiverso,
                                   Qs_Piva,
                                   CStr(Session("ASG_SuperUser_CodFiscale")),
                                    , , , , , filtro,
                                    Me.Txt_DataEmissione.Text,
                                    Me.Txt_DataEmissione.Text,
                                    ,
                                    False)

        If textBox_Ricerca.Text <> "" AndAlso Me.Cmb_DestinatarioDiverso.Items.Count > 1 Then
            Me.Cmb_DestinatarioDiverso.SelectedIndex = 1
            Cmb_DestinatarioDiverso_SelectedIndexChanged(Me, Nothing)
        End If

    End Sub



    '#####################################################
    Private Sub Img_btn_CercaMittente_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles Img_btn_CercaMittente.Click

        If Me.Txt_Cerca_Mittente.Text = "" Then
            Messaggi.AgroMsgBox("Scrivere parte della ragione sociale del contatto prima di avviare il filtro di ricerca.", Page, "aspnetForm", upTabs_1)
        Else
            FiltraComboCessionario(Me.Txt_Cerca_Mittente, 0)
        End If

    End Sub

    '#################################################
    Private Sub Img_btn_CercaMittente2_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles Img_btn_CercaMittente2.Click


        If Me.Txt_Cerca_Mittente2.Text = "" Then
            Messaggi.AgroMsgBox("Scrivere la partita IVA del contatto prima di avviare il filtro di ricerca.", Page, "aspnetForm", upTabs_1)
        Else
            'If VerificaEspressioneRegolare(Txt_Cerca_Mittente2.Text, "", Input_Controllato.enum_EspressioniRegolari.RegExp_PartitaIVA) Or _
            '   Txt_Cerca_Mittente2.Text = "" Then
            'If Agro_CheckPartitaIVA(Txt_Cerca_Mittente2.Text) Or _
            '   Txt_Cerca_Mittente2.Text = "" Then
            FiltraComboCessionario(Me.Txt_Cerca_Mittente2, 1)
            'Else
            '    AgroMsgBox("La stringa non rappresenta una partita iva valida!", Page)
            'End If
            'Else
            '    AgroMsgBox("La stringa non rappresenta una partita iva valida!", Page)
            'End If
        End If

    End Sub

    '#################################################
    Private Sub Img_btn_CercaDiverso_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles Img_btn_CercaDiverso.Click

        If Me.Txt_Cerca_Diverso.Text = "" Then
            Messaggi.AgroMsgBox("Scrivere parte della ragione sociale del destinatario diverso prima di avviare il filtro di ricerca.", Page, "aspnetForm", upTabs_1)
        Else
            FiltraComboCessionarioDiverso(Txt_Cerca_Diverso, 0)
        End If
    End Sub

    '#################################################
    Private Sub Img_btn_CercaDiverso2_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles Img_btn_CercaDiverso2.Click

        If Me.Txt_Cerca_Diverso2.Text = "" Then
            Messaggi.AgroMsgBox("Scrivere la partita IVA del destinatario diverso prima di avviare il filtro di ricerca.", Page, "aspnetForm", upTabs_1)
        Else
            'If VerificaEspressioneRegolare(Txt_Cerca_Diverso2.Text, "", Input_Controllato.enum_EspressioniRegolari.RegExp_PartitaIVA) Or _
            '   Txt_Cerca_Diverso2.Text = "" Then

            'If Agro_CheckPartitaIVA(Txt_Cerca_Diverso2.Text) Or _
            '   Txt_Cerca_Diverso2.Text = "" Then

            FiltraComboCessionarioDiverso(Txt_Cerca_Diverso2, 1)

            '    Else
            'AgroMsgBox("La stringa non rappresenta una partita iva valida!", Page)
            '    End If

            'Else
            '    AgroMsgBox("La stringa non rappresenta una partita iva valida!", Page)
            'End If
        End If

    End Sub


    Private Sub imgBtn_Nuovo_Contatto_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgBtn_Nuovo_Contatto.Click

        Dim UrlTarget As String

        '----- Verifico se l'utente dispone dei permessi per creare o modificare i rapporti contabili
        Dim PermessoRappContab As Boolean
        Dim acUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

        PermessoRappContab = acUtenti.Controlla_Permessi_Utente(
                                    Session("ASG_Utente_Username"),
                                    Session("ASG_IdServizio"),
                                    enum_Security_Attivita.Anagrafica_Contatto,
                                    enum_Security_Operazione.Modifica,
                                    Now, "",
                                    objParametri_Server
                                    )


        If PermessoRappContab Then

            'UrlTarget = "../GestioneContatti/Contatti_Edit_2.aspx" & _
            '            "?k=" & Stringa_Codifica(xChiave, AgroKey_EncoderDecoder, Server) & _
            '            "&p=" & Stringa_Codifica(piva, AgroKey_EncoderDecoder, Server) & _
            '            "&o=" & Stringa_Codifica(enum_TipoOperazioneDB.Scrittura, AgroKey_EncoderDecoder, Server) & _
            '            "&f=" & Stringa_Codifica(Me.RdBtnListPersone.SelectedIndex, AgroKey_EncoderDecoder, Server) & _
            '            "&v=" & Stringa_Codifica(Me.RdBtnListTipoAlbero.SelectedIndex, AgroKey_EncoderDecoder, Server)

            Dim k As String
            Albero.ChiaveAlbero_Codifica(k, enum_TipoNodo.x_Contatti, Qs_Piva)

            UrlTarget = "../../GestioneContatti/Contatti_Edit_2.aspx" &
                        "?k=" & Stringa_Codifica(k, AgroKey_EncoderDecoder, Server) &
                        "&p=" & Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) &
                        "&o=" & Stringa_Codifica(enum_TipoOperazioneDB.Scrittura, AgroKey_EncoderDecoder, Server) &
                        "&f=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Server) &
                        "&v=" & Stringa_Codifica(1, AgroKey_EncoderDecoder, Server)

            AgronicaCoreDataProvider.UtilityProvider.Page_NewWindow(Page, UrlTarget, "", , , , , , , , , )

        End If

    End Sub


    Public Sub CaricaCombo_TipoIndirizzo_Contatto(ByRef objServer As System.Web.HttpServerUtility,
                                                    ByRef objSession As System.Web.SessionState.HttpSessionState,
                                                    ByRef objPage As System.Web.UI.Page,
                                                    ByRef Cmb As System.Web.UI.WebControls.DropDownList,
                                                    ByVal Cod_Contatto As String,
                                                    ByVal ID_CF As Integer)
        'carico gli indirizzi a seconda della persona fisica o giuridica

        Cmb.Items.Clear()

        Select Case ID_CF

            Case PERSONA_FISICA
                Cmb.Items.Add(New ListItem("Residenza", "3"))
                Cmb.Items.Add(New ListItem("Luogo di Nascita", "5"))
                Cmb.Items.Add(New ListItem("Domicilio", "2"))
                Cmb.Items.Add(New ListItem("Residenza Estiva", "4"))

            Case PERSONA_GIURIDICA, CONTATTO_ESTERO
                Cmb.Items.Add(New ListItem("Sede Operativa", "1"))
                Cmb.Items.Add(New ListItem("Sede Legale", "101"))
                Cmb.Items.Add(New ListItem("Sede Aziendale", "102"))
                Cmb.Items.Add(New ListItem("Stabilimento", "103"))

            Case Else

                If IsNumeric(Cod_Contatto) AndAlso Cod_Contatto.Length = 11 AndAlso Not Cod_Contatto.StartsWith("-") Then
                    'persona giuridica (PIVA)
                    Cmb.Items.Add(New ListItem("Sede Operativa", "1"))
                    Cmb.Items.Add(New ListItem("Sede Legale", "101"))
                    Cmb.Items.Add(New ListItem("Sede Aziendale", "102"))
                    Cmb.Items.Add(New ListItem("Stabilimento", "103"))
                Else
                    If Cod_Contatto.Length = 16 Then
                        'persona fisica (CF)
                        Cmb.Items.Add(New ListItem("Residenza", "3"))
                        Cmb.Items.Add(New ListItem("Luogo di Nascita", "5"))
                        Cmb.Items.Add(New ListItem("Domicilio", "2"))
                        Cmb.Items.Add(New ListItem("Residenza Estiva", "4"))
                    Else
                        If Cod_Contatto.StartsWith("-") Then
                            'contatto generico del giaslan
                            Cmb.Items.Add(New ListItem("Residenza", "3"))
                            Cmb.Items.Add(New ListItem("Luogo di Nascita", "5"))
                            Cmb.Items.Add(New ListItem("Domicilio", "2"))
                            Cmb.Items.Add(New ListItem("Residenza Estiva", "4"))
                        Else
                            'codice fittizio del giasonline, possono averlo sia le p.f. che le p.g.
                            Cmb.Items.Add(New ListItem("Sede Operativa", "1"))
                            Cmb.Items.Add(New ListItem("Sede Legale", "101"))
                            Cmb.Items.Add(New ListItem("Sede Aziendale", "102"))
                            Cmb.Items.Add(New ListItem("Stabilimento", "103"))
                            Cmb.Items.Add(New ListItem("Residenza", "3"))
                            Cmb.Items.Add(New ListItem("Luogo di Nascita", "5"))
                            Cmb.Items.Add(New ListItem("Domicilio", "2"))
                            Cmb.Items.Add(New ListItem("Residenza Estiva", "4"))
                        End If
                    End If

                End If

        End Select

    End Sub

    Protected Sub ID_Prodotto_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ID_Prodotto.Click
        Gestione_OperazioniToolbar(sender, e)
    End Sub

    'Protected Sub ID_Zootecnico_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ID_Zootecnico.Click
    '    Gestione_OperazioniToolbar(sender, e)
    'End Sub

    Protected Sub ID_Allegati_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ID_Allegati.Click
        Gestione_OperazioniToolbar(sender, e)
    End Sub

    Protected Sub ID_Cancella_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ID_Cancella.Click
        Gestione_OperazioniToolbar(sender, e)
    End Sub

    'Protected Sub ID_Stampa_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ID_Stampa.Click
    '    Gestione_OperazioniToolbar(sender, e)
    'End Sub

    Private Sub InizializzaScripts()

        Dim contenitore As String = "window"
        Dim scarto As String = "-30"
        Dim scartoW As String = "-60"

        Dim stb As New StringBuilder
        stb.AppendLine("$(document).ready(function () {")

        'stb.AppendLine("$('#dialog').dialog({")
        'stb.AppendLine("    autoOpen: false,")
        'stb.AppendLine("    height: $(" & contenitore & ").height()" & scarto & ",")
        'stb.AppendLine("    width: $(" & contenitore & ").width()" & scartoW & ",")
        'stb.AppendLine("    maxHeight: $(" & contenitore & ").height()" & scarto & ",")
        'stb.AppendLine("    maxWidth: $(" & contenitore & ").width()" & scartoW & ",")
        'stb.AppendLine("    minHeight: $(" & contenitore & ").height()" & scarto & ",")
        'stb.AppendLine("    minWidth: $(" & contenitore & ").width()" & scartoW & ",")
        'stb.AppendLine("    modal: true")
        'stb.AppendLine("});")

        stb.AppendLine("$('#dialog').dialog({")
        stb.AppendLine("    autoOpen: false,")
        stb.AppendLine("    height: $(" & contenitore & ").height()" & scarto & ",")
        stb.AppendLine("    width: '98%',")
        stb.AppendLine("    maxHeight: $(" & contenitore & ").height()" & scarto & ",")
        stb.AppendLine("    maxWidth: $(" & contenitore & ").width()" & scartoW & ",")
        stb.AppendLine("    modal: true")
        stb.AppendLine("});")



        stb.AppendLine("    $('.datepicker').datepicker({ ")
        stb.AppendLine("        dateFormat:  'dd/mm/yy',")
        stb.AppendLine("        disabled: false,")
        stb.AppendLine("        changeMonth: true,")
        stb.AppendLine("        changeYear: true")
        stb.AppendLine("    });")

        stb.AppendLine("      $('.Txt_DataEmissione').change(function(){$('#" & BTN_ChangeData.ClientID & "').click(); CopiaValoreInTxt_DataRegistrazione();}) ")

        stb.AppendLine(" $('#" & tabContatti.ClientID & "').tabs(); ")

        stb.AppendLine(" $('.bottone').button(); ")

        stb.AppendLine("});")
        ScriptManager.RegisterClientScriptBlock(upTabs_1, upTabs_1.GetType(),
                                         String.Format("jQuery_{0}", "datepicker"), stb.ToString, True)
    End Sub

    Private Sub Chiudi()
        Dim Str As New StringBuilder
        Str.AppendLine("$(document).ready(function () {")
        Str.AppendLine("    window.close();")
        Str.AppendLine("    });")


        ScriptManager.RegisterClientScriptBlock(upButtons, upButtons.GetType(),
                                         String.Format("jQuery_{0}", "closeme"), Str.ToString, True)
    End Sub



#Region "Per DDT"
    Private Sub CaricaCombo_Causale(ByVal lselezionato As String)

        Me.Cmb_Causale.Items.Clear()

        '03/01/2018: sui primi non viene usato il tipo enumerativo perchè è in conflitto col giaslan
        Me.Cmb_Causale.Items.Add(New ListItem("CONTO VENDITA", 0))
        Me.Cmb_Causale.Items.Add(New ListItem("OMAGGI", 1))
        Me.Cmb_Causale.Items.Add(New ListItem("CONTO VISIONE", 2))
        Me.Cmb_Causale.Items.Add(New ListItem("C/CONFERIMENTO", 3))
        Me.Cmb_Causale.Items.Add(New ListItem("CONTO ACQUISTO", 4))
        Me.Cmb_Causale.Items.Add(New ListItem("MERCE RESA", 5))
        Me.Cmb_Causale.Items.Add(New ListItem("CONTO LAVORAZIONE", enum_CausaliTrasporto.CONTO_LAVORAZIONE))
        Me.Cmb_Causale.Items.Add(New ListItem("CONTO ESSICAZIONE", enum_CausaliTrasporto.CONTO_ESSICAZIONE))
        Me.Cmb_Causale.Items.Add(New ListItem("RESO DA C/ESSICAZIONE", enum_CausaliTrasporto.RESO_DA_CONTO_ESSICAZIONE))

        Me.Cmb_Causale.SelectedIndex =
        Me.Cmb_Causale.Items.IndexOf(Me.Cmb_Causale.Items.FindByText(lselezionato))


    End Sub


    '####################################################################################################################
    Private Sub CaricaCombo_Aspetto()

        Me.Cmb_Aspetto.Items.Clear()

        Me.Cmb_Aspetto.Items.Add(New ListItem("VISIBILE", 0))
        Me.Cmb_Aspetto.Items.Add(New ListItem("NON VISIBILE", 1))

    End Sub

#End Region




    Protected Sub Rbl_TipoFattura_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Rbl_TipoFattura.SelectedIndexChanged

        CaricaComboXddt()

        ImpostazioniDocumentoDatoLAV_COD()


    End Sub


    'Private Function GetColumnIndexByName(gw As GridView, SearchColumnName As String) As Integer

    '    Dim columnIndex As Integer = 0
    '    Dim row As GridViewRow = gw.
    '    For Each cell As DataControlFieldCell In row.Cells



    '        columnIndex += 1
    '    Next
    '    Return columnIndex
    'End Function


    'Private Function GetColumnIndexByName(ByVal grid As DataGrid, ByVal name As String) As Integer

    '    For Each col As DataColumn In grid.Columns

    '        If (col.ColumnName.ToLower().Trim() = name.ToLower().Trim()) Then
    '            Return col.Ordinal
    '        End If

    '    Next

    '    Return -1
    'End Function

    'Public Function getDataGridColumnNumber(dataGridStyle As DataGridTableStyle, columnName As String) As Integer
    '    Dim columnNumber As Integer = -1

    '    For Each colStyle As DataGridColumnStyle In dataGridStyle.GridColumnStyles
    '        columnNumber = columnNumber + 1
    '        If colStyle.HeaderText.CompareTo(columnName) = 0 Then
    '            Return columnNumber
    '        End If
    '    Next
    '    columnNumber = -1
    '    Return columnNumber
    'End Function




    Private Shared Function gettimesep() As String
        Return System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.TimeSeparator
    End Function

    Private Sub caricaImpostazioniUtente()
        'leggo le eventuali IMPOSTAZIONI UTENTE
        Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim Dt_Impostazioni As New DataTable

        Dt_Impostazioni = ObjUtenti.Leggi(0,
                                          1,
                                          AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                          "",
                                          "",
                                          objParametri_Utenti)






        If Dt_Impostazioni IsNot Nothing AndAlso Dt_Impostazioni.Rows.Count > 0 Then

            For i = 0 To Dt_Impostazioni.Rows.Count - 1

                'Impostazioni sia per scrittura che modifica
                Select Case CInt(Dt_Impostazioni.Rows(i).Item("Impostazione_Cod"))


                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_SE_DATA_FATTURA_SUPERA_ALLEGATI
                        Select Case Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                            Case "0"
                                Session("UTENTE_COD_BLOCCA_SE_DATA_FATTURA_SUPERA_ALLEGATI") = False
                            Case Else
                                Session("UTENTE_COD_BLOCCA_SE_DATA_FATTURA_SUPERA_ALLEGATI") = True
                        End Select

                    Case Else



                End Select



            Next
        End If




    End Sub

    Protected Sub ImageButtonNuovoContatto_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ImageButtonNuovoContatto.Click


        Dim objParametriAgenda As New ParametriAgenda
        objParametriAgenda.Piva = Qs_Piva
        objParametriAgenda.RagSoc = Qs_Rag_Soc
        objParametriAgenda.Lav_Cod = QS_LavCod

        ApriConttatti(objParametriAgenda.Piva)


    End Sub

    Public Sub ApriConttatti(ByVal piva As String)

        Dim TipoRapp_cont As Integer

        Select Case QS_LavCod
            Case LAVCOD_FATTURA_EMESSA, LAVCOD_BOLLA_EMESSA, LAVCOD_NOTA_ACCREDITO_EMESSA
                TipoRapp_cont = COD_CLIENTE
            Case LAVCOD_FATTURA_RICEVUTA, LAVCOD_BOLLA_RICEVUTA, LAVCOD_NOTA_ACCREDITO_RICEVUTA
                TipoRapp_cont = COD_FORNITORE
        End Select


        Dim QueryString As String
        QueryString = "?o=" &
                        Stringa_Codifica(enum_TipoOperazioneDB.Scrittura, AgroKey_EncoderDecoder, Server) &
                        "&tipo_rapporto=" &
                        Stringa_Codifica(TipoRapp_cont, AgroKey_EncoderDecoder, Server) &
                        "&lav_cod=" &
                        Stringa_Codifica(QS_LavCod, AgroKey_EncoderDecoder, Server) &
                        "&piva=" &
                        Stringa_Codifica(piva, AgroKey_EncoderDecoder, Server) &
                        "&orig=" &
                        Stringa_Codifica(enum_PagineGiasOnline.Fattura, AgroKey_EncoderDecoder, Server) &
                        "&codcont=" &
                        Stringa_Codifica("0", AgroKey_EncoderDecoder, Server)


        Dim StrWindowOpen As String = AgronicaCoreDataProvider.UtilityProvider.JqueryModalDialogScript(
                "../Anagrafica/New_Contatto_Edit.aspx",
                QueryString,
                InsProdotto.ClientID,
                1250, 450, 0, 0,
                NomeForm:="aspnetForm")

        '  Vanni, 28/05/2014 16:12:46: con jquery...
        If upTabs_1 Is Nothing Then
            Page.FindControl("aspnetForm").Controls.Add(New LiteralControl(StrWindowOpen))
        Else
            ScriptManager.RegisterClientScriptBlock(upTabs_1, upTabs_1.GetType(),
                                         String.Format("jQuery_{0}", "openmodal"), StrWindowOpen, True)

        End If




    End Sub
    
    Public Sub SelezionaTab(ByVal upPanel As UpdatePanel, ByVal progressivoABaseZero As Integer)

        Dim stb As New StringBuilder

        stb.AppendLine("$(function() { ")
        stb.AppendLine("    $('#tabs').tabs({ ")
        stb.AppendLine("        active: " & progressivoABaseZero)
        stb.AppendLine("    }); ")
        stb.AppendLine("}); ")

        ScriptManager.RegisterClientScriptBlock(upPanel, upPanel.GetType(),
                                         String.Format("jQuery_{0}", "spostati"), stb.ToString, True)

    End Sub

    Private Sub BottoneNascostoContatti_Click(sender As Object, e As System.EventArgs) Handles BottoneNascostoContatti.Click
        RecuperaContatto()
    End Sub

    Sub RecuperaContatto()
        If Not IsNothing(Session("ContattoCreatoPiva")) AndAlso Session("ContattoCreatoPiva") <> "" Then
            Me.Txt_Cerca_Mittente2.Text = Session("ContattoCreatoPiva")
            FiltraComboCessionario(Me.Txt_Cerca_Mittente2, 1)
            FiltraComboCessionario(Session("ContattoCreatoPiva"), 1)
        End If
    End Sub


    Private Function CreaOggettoAgenda() As Operazione_Agenda


        Recupera_Dati_dai_Controlli()

        Dim BaseCode As Integer = 0
        Dim TopCode As Integer = 0

        '------------------------------------------------
        '----- Calcolo i valori di BaseCode e TopCode
        '------------------------------------------------
        UtilityProvider.Calcola_BaseCode_TopCode(BaseCode, TopCode, Session("ASG_ProgressivoGIAS"))

        '------------------------------------------------
        '----- AGENDA
        '------------------------------------------------
        Dim Agenda As Operazione_Agenda
        Dim Movimento As Movimento
        Dim Movimento_Dettaglio As Movimento_Dettaglio = Nothing

        Agenda = New Operazione_Agenda

        Agenda.Tipo_Operazione = Qs_Operazione
        Agenda.Id_Agenda = Qs_IdAgenda
        Agenda.Data = x_Data_Emissione
        Agenda.Piva = Qs_Piva
        Agenda.Sa_Cod = Qs_SaCod
        Agenda.Lav_Cod = QS_LavCod
        Agenda.Des_Lib = x_Des_Lib

        Agenda.BaseCode = BaseCode
        Agenda.TopCode = TopCode


        '------------------------------------------------
        '----- MOVIMENTI
        '------------------------------------------------
        Agenda.Movimenti = New List(Of Movimento)


        '------------------------------------------------
        '------------------------------------------------
        '----- MOVIMENTO CONTABILE
        '------------------------------------------------
        '------------------------------------------------
        Movimento = New Movimento

        Movimento.Id_Agenda = Qs_IdAgenda
        Movimento.Piva = Qs_Piva
        Movimento.Sa_Cod = Qs_SaCod
        Movimento.Data = x_Data_Emissione
        Movimento.Lav_Cod = QS_LavCod
        Movimento.Cau_Mov = CAU_REGISTRAZIONI
        Movimento.Mov_Desc = x_Mov_Desc_Contabile
        'Movimento.Ora = CDate("12.00").ToShortTimeString
        Movimento.Data_Registrazione = x_Data_Registrazione
        Movimento.Scadenza = x_Scadenza
        Movimento.Doc_Numero_Sin = x_Doc_Numero_Sin
        Movimento.Doc_Numero = x_Doc_Numero
        Movimento.Doc_Numero_Des = x_Doc_Numero_Des
        Movimento.Num_Protocollo = x_Num_Protocollo
        Movimento.Colli = frm_Colli
        Movimento.Peso = x_Peso
        Movimento.Aspetto = frm_Aspetto
        Movimento.Causale_Trasporto = frm_CausaleTrasporto
        Movimento.Tipo_Sconto = x_Tipo_Sconto
        Movimento.Cod_Risum = x_Cod_RisUm
        Movimento.Cod_IndirizzoRisUm = x_Cod_IndirizzoRisUm
        Movimento.Cod_Destinazione = x_Cod_Destinazione
        Movimento.Cod_IndirizzoDestinazione = x_Cod_IndirizzoDestinazione
        Movimento.Mezzo = x_Mezzo
        Movimento.Cod_Vettore = x_Cod_Vettore
        Movimento.Cod_IndirizzoVettore = x_Cod_IndirizzoVettore
        Movimento.Natura_Beni = frm_NaturaBeni
        Movimento.Modalita = 0
        Movimento.Extra_Str = x_Extra_Str
        Movimento.Extra_Int = x_Extra_Int
        Movimento.Extra_Date = x_Extra_Date

        Movimento.BaseCode = BaseCode
        Movimento.TopCode = TopCode

        Agenda.Movimenti.Add(Movimento)

        '------------------------------------------------
        '----- PAGAMENTI
        '------------------------------------------------

        If Session("Pagamenti") IsNot Nothing Then
            Dim Pagamenti As List(Of Pagamento)
            Pagamenti = Session("Pagamenti")
            '  Galassi, 29/06/2017 17.57.36: Da realizzare se il pagamento segue la data dell'agenda
            'If Qs_Operazione = enum_TipoOperazioneDB.Modifica Then
            '    For Each pagam As Pagamento In Pagamenti
            '        pagam.Data_Pagamento = Agenda.Data
            '    Next
            'End If
            Agenda.Movimenti(Agenda.Movimenti.Count - 1).Pagamenti = Pagamenti
        End If


        '------------------------------------------------
        '------------------------------------------------
        '----- MOVIMENTO MAGAZZINO
        '------------------------------------------------
        '------------------------------------------------
        Movimento = New Movimento

        Movimento.Id_Agenda = Qs_IdAgenda
        Movimento.Piva = Qs_Piva
        Movimento.Sa_Cod = Qs_SaCod
        Movimento.Data = x_Data_Emissione
        Movimento.Lav_Cod = QS_LavCod
        Movimento.Cau_Mov = x_Cau_Mov_Magazzino
        Movimento.Mov_Desc = x_Mov_Desc_Magazzino
        'Movimento.Ora = "12.00"
        Movimento.Data_Registrazione = x_Data_Registrazione
        Movimento.Scadenza = AGRODATAFINE

        Movimento.BaseCode = BaseCode
        Movimento.TopCode = TopCode

        Agenda.Movimenti.Add(Movimento)


        '------------------------------------------------
        '----- MOVIMENTI DETTAGLI
        '------------------------------------------------

        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli = New List(Of Movimento_Dettaglio)

        If Session("DT_Prodotti_nel_Doc") IsNot Nothing Then

            Dim Dt As DataTable = Session("DT_Prodotti_nel_Doc")

            For i = 0 To Dt.Rows.Count - 1

                Select Case QS_LavCod

                    Case CStr(LAVCOD_FATTURA_RICEVUTA), CStr(LAVCOD_BOLLA_RICEVUTA), CStr(LAVCOD_NOTA_ACCREDITO_EMESSA)
                        'RICEVIMENTO
                        'carico
                        'destinazione
                        Movimento_Dettaglio = Dt.Rows(i).Item("Dettaglio_Destinazione")

                    Case CStr(LAVCOD_FATTURA_EMESSA), CStr(LAVCOD_BOLLA_EMESSA), CStr(LAVCOD_NOTA_ACCREDITO_RICEVUTA)
                        'EMISSIONE
                        'scarico
                        'provenienza
                        Movimento_Dettaglio = Dt.Rows(i).Item("Dettaglio_Provenienza")

                End Select

                If Qs_Operazione = enum_TipoOperazioneDB.Modifica Then
                    Movimento_Dettaglio.Data = Agenda.Data
                End If

                Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Add(Movimento_Dettaglio)

            Next

        End If


        Return Agenda

    End Function



    Protected Sub ImgBtn_Salva_Click(sender As Object, e As ImageClickEventArgs) Handles ImgBtn_Salva.Click
        Dim Errore As String = ""

        Salva_Tutto(Errore)

        If Errore = "" Then
            fine_salvataggio(1, Nothing, False)
            AAA_GestioneUscitaPagina()
        Else
            Messaggi.AgroMsgBox(Errore, Page, NomeForm:="aspnetForm", UpdatePanel:=upTabs_2)
        End If
    End Sub

    'Protected Sub ImgBtn_Salva_Nuovo_Click(sender As Object, e As EventArgs) Handles ImgBtn_Salva_Nuovo.Click

    '    Dim Errore As String = ""

    '    Salva_Tutto(Errore)

    '    If Errore = "" Then
    '        fine_salvataggio(2, Nothing, False)
    '        Session("DT_Prodotti_nel_Doc") = Nothing
    '        Session("DT_IVA_nel_Doc") = Nothing
    '    Else
    '        Messaggi.AgroMsgBox(Errore, Page, NomeForm:="aspnetForm", UpdatePanel:=upTabs_2)
    '    End If


    'End Sub

    Protected Sub ImgBtn_Salva_Nuovo_Click(sender As Object, e As ImageClickEventArgs) Handles ImgBtn_Salva_Nuovo.Click
        Dim Errore As String = ""

        Salva_Tutto(Errore)

        If Errore = "" Then
            fine_salvataggio(2, Nothing, False)
            Session("DT_Prodotti_nel_Doc") = Nothing
            Session("DT_IVA_nel_Doc") = Nothing
        Else
            Messaggi.AgroMsgBox(Errore, Page, NomeForm:="aspnetForm", UpdatePanel:=upTabs_2)
        End If

    End Sub


End Class



