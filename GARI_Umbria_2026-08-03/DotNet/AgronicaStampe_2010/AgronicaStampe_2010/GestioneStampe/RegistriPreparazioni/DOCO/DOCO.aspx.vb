Imports CrystalDecisions.Shared
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza


Public Class DOCO1
    Inherits System.Web.UI.Page

    Private rptDoco_Layout As Rpt_DOCO
    Private rptDoco_NoLayout As Rpt_DOCO_NoLayout

    '----- Gestione Querystring
    Dim Piva As String
    Dim Lav_Cod As Integer
    Dim Id_Agenda As Integer
    Dim Opt_Layout As Integer
    Dim Stampa_Intestazione As Integer

    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Utenti As New AgronicaCoreDataProvider.AgronicaCoreParametri


#Region " DOCO "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
        'istanzio l'oggetto report
        rptDoco_Layout = New Rpt_DOCO
        rptDoco_NoLayout = New Rpt_DOCO_NoLayout
    End Sub

#End Region


    '##############################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'Faccio scadere subito la pagina memorizzata nella cache
        Response.Expires = 0

        '#################################################################################
        '#####  Recupero dati dalla QueryString 
        '#################################################################################

        'Lettura Parametri Query String
        Piva = Stringa_Decodifica(CStr(Request.QueryString("p")), _
                                    AgroKey_EncoderDecoder, _
                                    Server)

        Id_Agenda = CInt(Stringa_Decodifica(CStr(Request.QueryString("i")), _
                                    AgroKey_EncoderDecoder, _
                                    Server))

        Lav_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("l")), _
                                    AgroKey_EncoderDecoder, _
                                    Server))

        Opt_Layout = CInt(Stringa_Decodifica(CStr(Request.QueryString("ol")), _
                                    AgroKey_EncoderDecoder, _
                                    Server))

        Stampa_Intestazione = CInt(Stringa_Decodifica(CStr(Request.QueryString("si")), _
                                  AgroKey_EncoderDecoder, _
                                  Server))

        'PrintToPrinter = Stringa_Decodifica(CStr(Request.QueryString("r")), _
        '                            AgroKey_EncoderDecoder, _
        '                            Server)

        'PrintName = Stringa_Decodifica(CStr(Request.QueryString("a")), _
        '                            AgroKey_EncoderDecoder, _
        '                            Server)

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))


        '#################################################################################
        '#####  Genero il report
        '#################################################################################



        Dim Nome_Documento As String = "DOCO"
        Dim Log_Errori As String
        Dim ObjReport As Object

        If Not Me.IsPostBack Then

            '--------------------------------------------
            ' DICHIARAZIONE  E INIZIALIZZAZIONE DATASET
            '--------------------------------------------
            Dim DS As New DS_DOCO

            Try

                Log_Errori = ""

                Dim Flag_StampaIntestazione As Boolean
                Dim Flag_StampaContenuto As Boolean

                If Id_Agenda <> 0 Then
                    'stampa del DOCO con i dati
                    Flag_StampaContenuto = True
                Else
                    'stampa del DOCO vuoto, solo con intestazione
                    Flag_StampaContenuto = False
                End If

                If Stampa_Intestazione = 1 Then
                    Flag_StampaIntestazione = True
                Else
                    Flag_StampaIntestazione = False
                End If


                Stampa_DOCO(Opt_Layout, DS, Log_Errori, Flag_StampaIntestazione, Flag_StampaContenuto)

                Select Case Opt_Layout
                    Case 1
                        ObjReport = rptDoco_Layout
                    Case 2
                        ObjReport = rptDoco_NoLayout
                End Select


            Catch exc As Exception
                Log_Errori += "- PageLoad: " + vbCrLf + exc.Message + vbCrLf
            End Try



            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            Dim Nome_File As String

            If Log_Errori <> "" Then

                Log_Errori = Nome_Documento + ", " + vbCrLf + _
                                        "Piva = " + CStr(Piva) + ", " + vbCrLf + _
                                        "Id_agenda = " + CStr(Id_Agenda) + " " + vbCrLf + _
                                        vbCrLf + vbCrLf + vbCrLf + vbCrLf + _
                                        Log_Errori

                Nome_File = "Log_Errori_" + Nome_Documento + "_" + CStr(Session("ASG_Utente_Username"))

                'GestioneFile_CreaCartellaNelPathWebConfig("Path_LogErrori_StampeEsportazioni", "Stampe_Cantine", Path_Errore, Str_Errore_Path)
                'If Str_Errore_Path = "" And Path_Errore <> "" Then
                '    GestioneFile_CreaScriviFileConRicercaNome(Log_Errori, Path_Errore, Nome_File, Str_Errore_Path, )
                'End If

                Dim objLog As New GestioneLogStampe
                objLog.Gestione_LogErrori_Stampe(objParametri_Server, _
                                                 "Stampe_Cantine", _
                                                 Nome_File, _
                                                 Session("ASG_Utente_Username"), _
                                                 "DOCO.aspx", _
                                                 Log_Errori)



            End If
            '-----------------------------------------
        End If

        '==================================================================

        Try
            Session("Report") = ObjReport
            Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server))
        Catch ex As Exception
            Log_Errori += "- Export: " + vbCrLf + ex.Message + vbCrLf
        End Try


    End Sub


    '#####################################################################################################
    Private Sub Stampa_DOCO(ByVal Opt_Layout As Integer, _
                                    ByRef DS As DS_DOCO, _
                                    ByRef Log_Errori As String, _
                                    ByVal Flag_StampaIntestazione As Boolean, _
                                    ByVal Flag_StampaContenuto As Boolean)


        Dim objContabHLP As New AgronicaCoreContabHLP.Contabilita
        Dim objContab As New AgronicaCoreContabDAL.Contabilita_R
        Dim objContabParcoMacchine As New AgronicaCoreContabBIZ.Parco_Macchine_R


        Dim DT As DataTable
        Dim i As Integer

        Dim Dr_Intestazione As DS_DOCO.IntestazioneRow
        Dim Dr_Dettagli As DS_DOCO.DettagliRow

        '------------------
        'Impresa

        Dim Indirizzo_Impresa As String
        Dim x_RagSoc_Impresa As String = ""
        Dim x_CodContatto_Impresa As String = ""
        Dim x_CodiceFiscale_Impresa As String = ""
        Dim x_IndDes_Impresa As String = ""
        Dim x_FrzDes_Impresa As String = ""
        Dim x_Cap_Impresa As String = ""
        Dim x_Comune_Impresa As String = ""
        Dim x_Provincia_Impresa As String = ""
        Dim x_Numero As String = ""
        Dim x_Stato_Impresa As String = ""

        Dim x_RegImprese As String = ""
        Dim x_Provincia_RegImprese As String = ""
        Dim x_REA As String = ""
        Dim x_ISO As String = ""
        Dim x_AlboCoop As String = ""
        Dim x_CapitaleSociale As String = ""
        Dim x_Telefono As String = ""
        Dim x_Fax As String = ""
        Dim x_Cell As String = ""
        Dim x_Email As String = ""
        Dim x_SitoWeb As String = ""

        Dim x_Fabbricato_Des() As String
        Dim x_IndDes_Fabbricato() As String
        Dim x_FrzDes_Fabbricato() As String
        Dim x_Cap_Fabbricato() As String
        Dim x_Comune_Fabbricato() As String
        Dim x_Provincia_Fabbricato() As String

        '------------------

        Dim x_IndDes_Ministero As String = ""
        Dim x_FrzDes_Ministero As String = ""
        Dim x_Cap_Ministero As String = ""
        Dim x_Comune_Ministero As String = ""
        Dim x_Provincia_Ministero As String = ""

        '------------------

        Dim Targa As String = ""
        Dim Mac_Des As String = ""
        Dim N_Immatricolazione As String = ""
        Dim N_Immatricolazione_Rimorchio As String = ""
        Dim N_Autorizzazione_Trasporto As String = ""
        Dim Data_Rilascio_Autorizzazione As String = ""
        Dim PesoTara As Double = 0

        Dim x_Descrizione As String = ""
        Dim x_Tipo As String = ""
        Dim Flag_RaggruppaDettagli As Boolean = False

        Dim Tot_ImponibileNetto As Double = 0
        Dim Tot_Iva As Double = 0
        Dim Tot_Qta As Double = 0

        Dim Memo_Udm_Sim As String = ""
        Dim Memo_Udm_Cod As Integer = 0
        Dim Memo_Iva_Perc As Integer = 0

        Dim x_Imponibile As Double
        Dim x_Imponibile_Netto As Double
        Dim x_Sconto As Double
        Dim x_Aliquota As String
        Dim x_IVA As Double
        Dim x_Importo As Double

        Dim Colore As String

        Dim Nome_Categoria As String = ""
        '------------------

        Dr_Intestazione = DS.Intestazione.NewIntestazioneRow

        Try

            '################################################################
            '####################### INTESTAZIONE ###########################
            '################################################################

            Leggi_Intestazione_Impresa(objParametri_Server, _
                                            CInt(Session("ASG_ProgressivoGIAS")), _
                                        True, _
                                        False, _
                                        Piva, _
                                        Log_Errori, _
                                        x_RagSoc_Impresa, _
                                        x_CodContatto_Impresa, _
                                        x_CodiceFiscale_Impresa, _
                                        x_IndDes_Impresa, _
                                        x_FrzDes_Impresa, _
                                        x_Cap_Impresa, _
                                        x_Comune_Impresa, _
                                        x_Provincia_Impresa, _
                                        x_RegImprese, _
                                        x_Provincia_RegImprese, _
                                        x_REA, _
                                        x_ISO, _
                                        x_AlboCoop, _
                                        x_CapitaleSociale, _
                                        x_Telefono, _
                                        x_Fax, _
                                        x_Cell, _
                                        x_Email, _
                                        x_SitoWeb, _
                                        x_Fabbricato_Des, _
                                        x_IndDes_Fabbricato, _
                                        x_FrzDes_Fabbricato, _
                                        x_Cap_Fabbricato, _
                                        x_Comune_Fabbricato, _
                                        x_Provincia_Fabbricato, _
                                        x_IndDes_Ministero, _
                                        x_FrzDes_Ministero, _
                                        x_Cap_Ministero, _
                                        x_Comune_Ministero, _
                                        x_Provincia_Ministero, _
                                        Nothing, _
                                        Nothing, _
                                        Nothing, _
                                        Nothing, _
                                        Nothing, _
                                        Nothing, _
                                        Nothing, _
                                        Nothing, _
                                        x_Stato_Impresa)

            If Flag_StampaIntestazione = True Then

                Dr_Intestazione.RagSoc_Impresa = x_RagSoc_Impresa
                Dr_Intestazione.CodContatto_Impresa = x_CodContatto_Impresa
                Dr_Intestazione.CodiceFiscale_Impresa = "Cod. Fisc.: " + x_CodiceFiscale_Impresa
                Dr_Intestazione.Piva_Impresa = "P.IVA: " + Piva
                Dr_Intestazione.IndDes_Impresa = "Via " + x_IndDes_Impresa
                Dr_Intestazione.FrzDes_Impresa = x_FrzDes_Impresa
                Dr_Intestazione.Cap_Impresa = x_Cap_Impresa
                Dr_Intestazione.Comune_Impresa = x_Comune_Impresa
                Dr_Intestazione.Provincia_Impresa = x_Provincia_Impresa
                Dr_Intestazione.RegImprese = x_RegImprese
                Dr_Intestazione.REA = "Cod. REA:" + x_REA
                Dr_Intestazione.ISO = x_ISO
                Dr_Intestazione.AlboCoop = x_AlboCoop
                Dr_Intestazione.Telefono = x_Telefono
                Dr_Intestazione.Fax = x_Fax
                Dr_Intestazione.Email = x_Email
                Dr_Intestazione.SitoWeb = x_SitoWeb

            End If

            If Flag_StampaContenuto = True Then
                Dr_Intestazione.IndDes_Ministero = x_IndDes_Ministero
                Dr_Intestazione.FrzDes_Ministero = x_FrzDes_Ministero
                Dr_Intestazione.Cap_Ministero = x_Cap_Ministero
                Dr_Intestazione.Comune_Ministero = x_Comune_Ministero
                Dr_Intestazione.Provincia_Ministero = x_Provincia_Ministero
            End If


        Catch ex As Exception
            Log_Errori += "- Lettura dei dati dell'intestazione dell'impresa: " + vbCrLf + ex.Message + vbCrLf
        End Try


        '####################################################

        If Flag_StampaContenuto = True Then

            Dim Zona_Viticola As String

            Try

                Dim objUtentiImp As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

                Zona_Viticola = objUtentiImp.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.SUPERUSER_COD_ZONA_VITICOLA, _
                                                                                       objParametri_Utenti, _
                                                                                       2)

                Dim objDOCO As New AgronicaCoreStampeDAL.DocCantina

                'DT = NewCom_DOCO_Leggi(Server, Session, Page, _
                '                        Piva, _
                '                        Id_Agenda, _
                '                        "", _
                '                        "")

                DT = objDOCO.DOCO_Leggi(Piva, _
                                      Id_Agenda, _
                                      "", _
                                      "", _
                                      objParametri_Server)


            Catch ex As Exception
                Log_Errori += "- Lettura dei dati: " + vbCrLf + ex.Message + vbCrLf + vbCrLf
            End Try

            If Not IsNothing(DT) AndAlso DT.Rows.Count <> 0 Then

                For i = 0 To DT.Rows.Count - 1

                    Try

                        '-----------------------------
                        Dr_Dettagli = DS.Dettagli.NewDettagliRow
                        '-----------------------------

                        Try

                            If i = 0 Then

                                'uso extra_str 2 e 3 per copiare la ragione sociale e il telefono nel report
                                Dr_Intestazione.Extra_Str_2 = x_RagSoc_Impresa
                                Dr_Intestazione.Extra_Str_3 = x_Telefono

                                With DT.Rows(i)

                                    Dr_Intestazione.Totale_Fattura = Format(Arrotonda_2Decimali(CDbl(.Item("Num_Protocollo"))), "#,###,##0.00")

                                    If .Item("Cod_Destinazione") = 0 Then
                                        '-------------------------------------------------------
                                        '--------------- DESTINATARIO = CLIENTE ----------------
                                        '-------------------------------------------------------

                                        Dr_Intestazione.Cod_RisUm = .Item("Cod_RisUm")
                                        Dr_Intestazione.Cod_IndirizzoRisUm = .Item("Cod_IndirizzoRisUm")
                                        Dr_Intestazione.Cod_Destinazione = .Item("Cod_Destinazione")
                                        Dr_Intestazione.Cod_IndirizzoDestinazione = .Item("Cod_IndirizzoDestinazione")

                                        '-------------------------------------------------------
                                        '--------------- DESTINATARIO --------------------------

                                        Dr_Intestazione.RagSoc_Cliente = CStr(.Item("Rag_Soc_Cliente")) + CStr(.Item("Nome_Cliente")) + " " + CStr(.Item("Cognome_Cliente"))
                                        Dr_Intestazione.CodContatto_Cliente = .Item("Cod_Contatto_Cliente")

                                        Dr_Intestazione.Piva_Cliente = ""
                                        Dr_Intestazione.CodiceFiscale_Cliente = ""
                                        Ricava_Piva_Codicefiscale(.Item("id_cf_Cliente"), _
                                                                  .Item("Cod_Contatto_Cliente"), _
                                                                    .Item("Codice_Fiscale_Cliente"), _
                                                                    0,
                                                                    Dr_Intestazione.Piva_Cliente, _
                                                                    Dr_Intestazione.CodiceFiscale_Cliente, _
                                                                    Nothing)

                                        Dr_Intestazione.IndDes_Cliente = .Item("Ind_Des_Cliente")
                                        Dr_Intestazione.FrzDes_Cliente = .Item("Frz_Des_Cliente")
                                        Dr_Intestazione.Cap_Cliente = .Item("Cap_Cliente")
                                        Select Case .Item("id_cf_Cliente")
                                            Case 2
                                                'Dr_Intestazione.Indirizzo_Cliente = CStr(.Item("Ind_Des_Cliente")) + " " + CStr(.Item("Frz_Des_Cliente")) + " " + CStr(.Item("Stato_Cliente"))
                                                Dr_Intestazione.Indirizzo_Cliente = CStr(.Item("Frz_Des_Cliente")) + " " + CStr(.Item("Stato_Cliente"))
                                                Dr_Intestazione.Piva_CF_Cliente = .Item("Cod_Contatto_Cliente")
                                            Case Else
                                                Dr_Intestazione.Comune_Cliente = .Item("Localita_Cliente")
                                                Dr_Intestazione.Provincia_Cliente = .Item("Comuni_Prov_Cliente")
                                                'Dr_Intestazione.Indirizzo_Cliente = CStr(.Item("Ind_Des_Cliente")) + " " + CStr(.Item("Localita_Cliente")) + " " + CStr(.Item("Comuni_Prov_Cliente"))
                                                Dr_Intestazione.Indirizzo_Cliente = CStr(.Item("Localita_Cliente")) + " " + CStr(.Item("Comuni_Prov_Cliente"))
                                                If .Item("id_cf_Cliente") = 0 Then
                                                    Dr_Intestazione.Piva_CF_Cliente = Dr_Intestazione.CodiceFiscale_Cliente
                                                Else
                                                    Dr_Intestazione.Piva_CF_Cliente = Dr_Intestazione.Piva_Cliente
                                                End If
                                        End Select
                                        Dr_Intestazione.Stato_Cliente = .Item("Stato_Cliente")
                                        '--------------- DESTINATARIO -------------------------------
                                        '-------------------------------------------------------

                                    Else
                                        '-------------------------------------------------------
                                        '-------------- DESTINATARIO <> CLIENTE ----------------
                                        '------------ DESTINATARIO = DEST DIVERSO --------------
                                        '------------- ACQUIRENTE = CLIENTE --------------------
                                        '-------------------------------------------------------

                                        Dr_Intestazione.Cod_RisUm = .Item("Cod_RisUm")
                                        Dr_Intestazione.Cod_IndirizzoRisUm = .Item("Cod_IndirizzoRisUm")
                                        Dr_Intestazione.Cod_Destinazione = .Item("Cod_Destinazione")
                                        Dr_Intestazione.Cod_IndirizzoDestinazione = .Item("Cod_IndirizzoDestinazione")

                                        '-------------------------------------------------------
                                        '--------------- DESTINATARIO --------------------------

                                        Dr_Intestazione.RagSoc_Cliente = CStr(.Item("Rag_Soc_DestDiv")) + CStr(.Item("Nome_DestDiv")) + " " + CStr(.Item("Cognome_DestDiv"))
                                        Dr_Intestazione.CodContatto_Cliente = .Item("Cod_Contatto_DestDiv")

                                        Dr_Intestazione.Piva_Cliente = ""
                                        Dr_Intestazione.CodiceFiscale_Cliente = ""
                                        Ricava_Piva_Codicefiscale(.Item("id_cf_DestDiv"), _
                                                                  .Item("Cod_Contatto_DestDiv"), _
                                                                    .Item("Codice_Fiscale_DestDiv"), _
                                                                    0,
                                                                    Dr_Intestazione.Piva_Cliente, _
                                                                    Dr_Intestazione.CodiceFiscale_Cliente, _
                                                                    Nothing)

                                        Dr_Intestazione.IndDes_Cliente = .Item("Ind_Des_DestDiv")
                                        Dr_Intestazione.FrzDes_Cliente = .Item("Frz_Des_DestDiv")
                                        Dr_Intestazione.Cap_Cliente = .Item("Cap_DestDiv")
                                        Select Case .Item("id_cf_DestDiv")
                                            Case 2
                                                Dr_Intestazione.Indirizzo_Cliente = CStr(.Item("Frz_Des_DestDiv")) + " " + CStr(.Item("Stato_DestDiv"))
                                                Dr_Intestazione.Piva_CF_Cliente = .Item("Cod_Contatto_DestDiv")
                                            Case Else
                                                Dr_Intestazione.Comune_Cliente = .Item("Localita_DestDiv")
                                                Dr_Intestazione.Provincia_Cliente = .Item("Comuni_Prov_DestDiv")
                                                Dr_Intestazione.Indirizzo_Cliente = CStr(.Item("Localita_DestDiv")) + " (" + CStr(.Item("Comuni_Prov_DestDiv")) + ")"
                                                If .Item("id_cf_DestDiv") = 0 Then
                                                    Dr_Intestazione.Piva_CF_Cliente = Dr_Intestazione.CodiceFiscale_Cliente
                                                Else
                                                    Dr_Intestazione.Piva_CF_Cliente = Dr_Intestazione.Piva_Cliente
                                                End If
                                        End Select
                                        Dr_Intestazione.Stato_Cliente = .Item("Stato_DestDiv")
                                        '--------------- DESTINATARIO -------------------------------
                                        '-------------------------------------------------------


                                        '-------------------------------------------------------
                                        '--------------- ACQUIRENTE -------------------------------
                                        Dr_Intestazione.RagSoc_DestDiv = CStr(.Item("Rag_Soc_Cliente")) + CStr(.Item("Nome_Cliente")) + " " + CStr(.Item("Cognome_Cliente"))
                                        Dr_Intestazione.CodContatto_DestDiv = .Item("Cod_Contatto_Cliente")

                                        Dr_Intestazione.Piva_DestDiv = ""
                                        Dr_Intestazione.CodiceFiscale_DestDiv = ""
                                        Ricava_Piva_Codicefiscale(.Item("id_cf_Cliente"), _
                                                                  .Item("Cod_Contatto_Cliente"), _
                                                                    .Item("Codice_Fiscale_Cliente"), _
                                                                    0,
                                                                    Dr_Intestazione.Piva_DestDiv, _
                                                                    Dr_Intestazione.CodiceFiscale_DestDiv, _
                                                                    Nothing)

                                        Dr_Intestazione.IndDes_DestDiv = .Item("Ind_Des_Cliente")
                                        Dr_Intestazione.FrzDes_DestDiv = .Item("Frz_Des_Cliente")
                                        Dr_Intestazione.Cap_DestDiv = .Item("Cap_Cliente")
                                        Select Case .Item("id_cf_Cliente")
                                            Case 2
                                                Dr_Intestazione.Indirizzo_DestDiv = CStr(.Item("Frz_Des_Cliente")) + " " + CStr(.Item("Stato_Cliente"))
                                                Dr_Intestazione.Piva_CF_DestDiv = .Item("Cod_Contatto_Cliente")
                                            Case Else
                                                Dr_Intestazione.Comune_DestDiv = .Item("Localita_Cliente")
                                                Dr_Intestazione.Provincia_DestDiv = .Item("Comuni_prov_Cliente")
                                                Dr_Intestazione.Indirizzo_DestDiv = CStr(.Item("Localita_Cliente")) + " (" + CStr(.Item("Comuni_Prov_Cliente")) + ")"
                                                If .Item("id_cf_Cliente") = 0 Then
                                                    Dr_Intestazione.Piva_CF_DestDiv = Dr_Intestazione.CodiceFiscale_DestDiv
                                                Else
                                                    Dr_Intestazione.Piva_CF_DestDiv = Dr_Intestazione.Piva_DestDiv
                                                End If
                                        End Select
                                        Dr_Intestazione.Stato_DestDiv = .Item("Stato_Cliente")
                                        '--------------- ACQUIRENTE -------------------------------
                                        '-------------------------------------------------------

                                    End If


                                    '-------------------------------------------------------
                                    '--------------- VETTORE -------------------------------
                                    Dr_Intestazione.Mezzo = .Item("Mezzo")

                                    Select Case CInt(.Item("Mezzo"))
                                        Case 0
                                            'CType(RptDOCO.Section2.ReportObjects("XMittente"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "X"
                                            CType(rptDoco_Layout.Section2.ReportObjects("XMittente"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "X"
                                            CType(rptDoco_NoLayout.Section2.ReportObjects("XMittente"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "X"
                                        Case 1
                                            'CType(RptDOCO.Section2.ReportObjects("XDestinatario"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "X"
                                            CType(rptDoco_Layout.Section2.ReportObjects("XDestinatario"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "X"
                                            CType(rptDoco_NoLayout.Section2.ReportObjects("XDestinatario"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "X"
                                        Case 2
                                            'CType(RptDOCO.Section2.ReportObjects("XVettore"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "X"
                                            CType(rptDoco_Layout.Section2.ReportObjects("XVettore"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "X"
                                            CType(rptDoco_NoLayout.Section2.ReportObjects("XVettore"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "X"

                                            Dr_Intestazione.Cod_Vettore = .Item("Cod_Vettore")

                                            If .Item("Cod_Vettore") <> 0 Then

                                                Dr_Intestazione.Cod_IndirizzoVettore = .Item("Cod_IndirizzoVettore")
                                                Dr_Intestazione.RagSoc_Vettore = CStr(.Item("Rag_Soc_Vettore")) + CStr(.Item("Nome_Vettore")) + " " + CStr(.Item("Cognome_Vettore"))
                                                Dr_Intestazione.CodContatto_Vettore = .Item("Cod_Contatto_Vettore")

                                                Dr_Intestazione.Piva_Vettore = ""
                                                Dr_Intestazione.CodiceFiscale_Vettore = ""
                                                Ricava_Piva_Codicefiscale(.Item("id_cf_Vettore"), _
                                                                          .Item("Cod_Contatto_Vettore"), _
                                                                           .Item("Codice_Fiscale_Vettore"), _
                                                                           0,
                                                                          Dr_Intestazione.Piva_Vettore, _
                                                                          Dr_Intestazione.CodiceFiscale_Vettore, _
                                                                          Nothing)

                                                Dr_Intestazione.IndDes_Vettore = .Item("Ind_Des_Vettore")
                                                Dr_Intestazione.FrzDes_Vettore = .Item("Frz_Des_Vettore")
                                                Dr_Intestazione.Cap_Vettore = .Item("Cap_Vettore")
                                                Select Case .Item("id_cf_Vettore")
                                                    Case 2
                                                        Dr_Intestazione.Indirizzo_Vettore = CStr(.Item("Frz_Des_Vettore")) + " " + CStr(.Item("Stato_Vettore"))
                                                        Dr_Intestazione.Piva_CF_Vettore = .Item("Cod_Contatto_vettore")
                                                    Case Else
                                                        Dr_Intestazione.Comune_Vettore = .Item("Localita_Vettore")
                                                        Dr_Intestazione.Provincia_Vettore = .Item("Comuni_prov_Vettore")
                                                        Dr_Intestazione.Indirizzo_Vettore = CStr(.Item("Localita_Vettore")) + " (" + CStr(.Item("Comuni_Prov_Vettore")) + ")"
                                                        If .Item("id_cf_Vettore") = 0 Then
                                                            Dr_Intestazione.Piva_CF_Vettore = Dr_Intestazione.CodiceFiscale_Vettore
                                                        Else
                                                            Dr_Intestazione.Piva_CF_Vettore = Dr_Intestazione.Piva_Vettore
                                                        End If
                                                End Select
                                                Dr_Intestazione.Stato_Vettore = .Item("Stato_Vettore")
                                            End If
                                    End Select
                                    '--------------- VETTORE -------------------------------
                                    '-------------------------------------------------------

                                    Dr_Intestazione.Data_Redazione = .Item("Data_Movimento")
                                    Dr_Intestazione.Data_Firma = .Item("Data_Movimento")
                                    Dr_Intestazione.Data_Spedizione = .Item("Data_Spedizione")
                                    Dr_Intestazione.Data_Trasporto = .Item("Data_Spedizione")
                                    Dr_Intestazione.Ora_Trasporto = CDate(.Item("Ora")).ToShortTimeString
                                    Dr_Intestazione.Doc_Numero_sin = .Item("Doc_Numero_sin")
                                    Dr_Intestazione.Doc_Numero = .Item("Doc_Numero")
                                    Dr_Intestazione.Doc_Numero_Des = .Item("Doc_Numero_Des")
                                    'Dr_Intestazione.Numerazione_Interna = .Item("")
                                    'Dr_Intestazione.Numero_Fattura = .Item("")
                                    Dr_Intestazione.Luogo_Firma = .Item("Luogo_Partenza")
                                    Dr_Intestazione.Luogo_Partenza = .Item("Luogo_Partenza")
                                    Dr_Intestazione.Luogo_Consegna = .Item("Luogo_Consegna")
                                    Dr_Intestazione.Causale_Trasporto = .Item("Causale_Trasporto")
                                    Dr_Intestazione.Aspetto_Beni = .Item("Aspetto")
                                    Dr_Intestazione.Descrizione = .Item("Mov_Desc")
                                    Dr_Intestazione.Tipo_Peso = .Item("Tipo_Peso")
                                    Dr_Intestazione.Peso = .Item("Peso")
                                    Dr_Intestazione.Colli = .Item("Colli")
                                    Dr_Intestazione.Scadenza = .Item("Scadenza")
                                    Dr_Intestazione.Totale_Importo = .Item("Num_Protocollo")

                                    Dr_Intestazione.Tipo_Documento = .Item("Tipo_Documento")

                                    Select Case CStr(.Item("Tipo_Documento")).ToUpper
                                        Case "MOD A"
                                            'CType(RptDOCO.Section2.ReportObjects("XModA"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "X"
                                            CType(rptDoco_Layout.Section2.ReportObjects("XModA"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "X"
                                            CType(rptDoco_NoLayout.Section2.ReportObjects("XModA"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "X"
                                        Case "MOD B"
                                            'CType(RptDOCO.Section2.ReportObjects("XModB"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "X"
                                            CType(rptDoco_Layout.Section2.ReportObjects("XModB"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "X"
                                            CType(rptDoco_NoLayout.Section2.ReportObjects("XModB"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "X"
                                        Case "MOD C"
                                            'CType(RptDOCO.Section2.ReportObjects("XModC"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "X"
                                            CType(rptDoco_Layout.Section2.ReportObjects("XModC"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "X"
                                            CType(rptDoco_NoLayout.Section2.ReportObjects("XModC"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "X"
                                        Case "MOD D"
                                            'CType(RptDOCO.Section2.ReportObjects("XModD"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "X"
                                            CType(rptDoco_Layout.Section2.ReportObjects("XModD"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "X"
                                            CType(rptDoco_NoLayout.Section2.ReportObjects("XModD"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "X"
                                    End Select

                                    Dr_Intestazione.Indicazioni_Complementari = .Item("Indicazioni_Complementari")
                                    Dr_Intestazione.NomeCognome_Legale = CStr(.Item("Rag_Soc_Legale")) + CStr(.Item("Nome_Legale")) + " " + CStr(.Item("Cognome_Legale"))
                                    'Dr_Intestazione.Note = .Item("")

                                    If .Item("Mac_Cod") = 0 Then
                                        Dr_Intestazione.Mac_Cod = 0
                                        Dr_Intestazione.Targa = .Item("Targa_2")
                                        Dr_Intestazione.N_Immatricolazione = .Item("N_Immatricolazione_2")
                                        Dr_Intestazione.N_Immatricolazione_Rimorchio = .Item("N_Immatricolazione_Rimorchio_2")
                                        Dr_Intestazione.N_Autorizzazione = .Item("N_Autorizzazione_Trasporto_2")
                                        Dr_Intestazione.Data_Autorizzazione = .Item("Data_Rilascio_Autorizzazione_2")
                                        Dr_Intestazione.Peso_Tara = .Item("Peso_2")
                                        Dr_Intestazione.Extra_Str_1 = .Item("Mezzo_Trasporto")
                                    Else

                                        objContabParcoMacchine.Leggi_DettaglioMacchina(objParametri_Server, _
                                                                Piva, _
                                                                .Item("Mac_Cod"), _
                                                                Targa, _
                                                                Mac_Des, _
                                                                N_Immatricolazione, _
                                                                N_Immatricolazione_Rimorchio, _
                                                                N_Autorizzazione_Trasporto, _
                                                                Data_Rilascio_Autorizzazione, _
                                                                PesoTara)



                                        Dr_Intestazione.Mac_Cod = .Item("Mac_Cod")
                                        Dr_Intestazione.Targa = Targa
                                        Dr_Intestazione.N_Immatricolazione = N_Immatricolazione
                                        Dr_Intestazione.N_Immatricolazione_Rimorchio = N_Immatricolazione_Rimorchio
                                        Dr_Intestazione.N_Autorizzazione = N_Autorizzazione_Trasporto
                                        Dr_Intestazione.Data_Autorizzazione = Data_Rilascio_Autorizzazione
                                        Dr_Intestazione.Peso_Tara = PesoTara
                                        Dr_Intestazione.Extra_Str_1 = Mac_Des
                                    End If


                                End With

                            End If 'primo giro


                        Catch ex As Exception
                            Log_Errori += "- Gestione dell'intestazione: " + vbCrLf + ex.Message + vbCrLf + vbCrLf
                        End Try

                        '=======================================================================

                        With DT.Rows(i)

                            'se x_ChkLayOut_Hide=1 non si vuole visualizzare il dettaglio
                            If .Item("ChkLayOut_Hide") <> 1 Then

                                'il dettaglio è visibile

                                'se si vuole visualizzare i dettagli così come sono
                                If .Item("ChkLayout_Join_Prodotti") <> 1 Then

                                    '---------------------------------------------
                                    '------ VISUALIZZAZIONE DETTAGLI NORMALE -----
                                    '---------------------------------------------

                                    Dr_Dettagli.Contatore = i
                                    Dr_Dettagli.Codice = .Item("Codice_Prodotto")
                                    Dr_Dettagli.Colore = .Item("Colore")
                                    Select Case .Item("Colore")
                                        Case 0
                                            Dr_Dettagli.Codice += "B"
                                            Colore = "Bianco"
                                        Case 1
                                            Dr_Dettagli.Codice += "R"
                                            colore = "Rosso"
                                        Case 2
                                            Dr_Dettagli.Codice += "S"
                                            Colore = "Rosato"
                                    End Select

                                    Dr_Dettagli.N_Contenitori = .Item("Num_Contenitori")
                                    Dr_Dettagli.Marche = .Item("Marche_Contenitori")
                                    Dr_Dettagli.Numeri = .Item("Num_Contenitori")
                                    Dr_Dettagli.Num_Colli = .Item("Num_Colli")
                                    Dr_Dettagli.Natura_Colli = .Item("Des_Contenitori")
                                    Dr_Dettagli.Zona_Viticola = Zona_Viticola
                                    Dr_Dettagli.Manipolazioni = .Item("Manipolazioni")
                                    Dr_Dettagli.Titolo_Alcol = .Item("Titolo_Alcol")

                                    Dr_Dettagli.Elem_Cod = .Item("Elem_Cod")
                                    Dr_Dettagli.Pro_Cod = .Item("Pro_Cod")
                                    Dr_Dettagli.Mat_Cod = .Item("Mat_Cod")
                                    Dr_Dettagli.Cod_Progetto = .Item("Cod_Progetto")
                                    Dr_Dettagli.Fase_Cod = .Item("Fase_Cod")
                                    Dr_Dettagli.Lotto = .Item("Lotto")
                                    Dr_Dettagli.Cal_Cod = .Item("Cal_Cod")

                                    If .Item("ChkLayOut_Join_Prodotti") = 1 Then
                                        Flag_RaggruppaDettagli = True
                                    End If


                                    x_Descrizione = objContab.LeggiProdottoStampeContab(objParametri_Server, _
                                                        objParametri_Utenti, _
                                                        Nome_Categoria, _
                                                        Nothing, _
                                                        Nothing, _
                                                        Piva, _
                                                        .Item("Elem_Cod"), _
                                                        .Item("Pro_Cod"), _
                                                        .Item("Mat_Cod"), _
                                                        .Item("Cod_Progetto"), _
                                                        0, _
                                                        "", _
                                                        0, _
                                                        False, _
                                                        , , _
                                                        Flag_RaggruppaDettagli)



                                    'Ricava_Tipo_Descrizione(Server, Session, Page, _
                                    '                        objParametri_Server, _
                                    '                        objParametri_Utenti, _
                                    '                      x_Descrizione, _
                                    '                      x_Tipo, _
                                    '                      Piva, _
                                    '                      .Item("Elem_Cod"), _
                                    '                      .Item("Pro_Cod"), _
                                    '                      .Item("Mat_Cod"), _
                                    '                      .Item("Cod_Progetto"), _
                                    '                      .Item("Fase_Cod"), _
                                    '                      .Item("Lotto"), _
                                    '                      .Item("Cal_Cod"), _
                                    '                      .Item("Mov_Det_des"), _
                                    '                      .Item("Data_Movimento"), _
                                    '                      False, _
                                    '                      Flag_RaggruppaDettagli, _
                                    '                      False)

                                    Dr_Dettagli.Indicazioni_Supplementari = ""

                                    If CStr(.Item("Marche_Contenitori")) <> "" Then
                                        Dr_Dettagli.Indicazioni_Supplementari += " n." + CStr(.Item("Num_Contenitori")) + " " + CStr(.Item("Marche_Contenitori")) + " - "
                                    End If

                                    If CStr(.Item("Des_Contenitori")) <> "" Then
                                        Dr_Dettagli.Indicazioni_Supplementari += " n." + CStr(.Item("Num_Colli")) + " " + CStr(.Item("Des_Contenitori")) + " - "
                                    End If

                                    Dr_Dettagli.Indicazioni_Supplementari += " n." + CStr(.Item("Qta")) + " bottiglie "
                                    Dr_Dettagli.Indicazioni_Supplementari += x_Descrizione
                                    Dr_Dettagli.Indicazioni_Supplementari += " - Vino " + Colore
                                    Dr_Dettagli.Indicazioni_Supplementari += " - " + CStr(.Item("Titolo_Alcol")) + "%"

                                    'Dr_Dettagli.Indicazioni_Supplementari = x_Descrizione

                                    Dr_Dettagli.Udm_Cod = .Item("Udm_Cod")
                                    Dr_Dettagli.Udm_Des = .Item("Udm_Des")
                                    Dr_Dettagli.Udm_Sim = .Item("Udm_Sim")
                                    Dr_Dettagli.Qta = .Item("Qta")
                                    Dr_Dettagli.Udm_Cod_Extra = .Item("Udm_Cod_Extra")
                                    Dr_Dettagli.Udm_Des_Extra = ""
                                    Dr_Dettagli.Udm_Sim_Extra = ""
                                    If .Item("Udm_Cod_Extra") <> 0 Then

                                        ' Dr_Dettagli.Udm_Des_Extra = UdmDes_from_UdmCod(Server, Session, Page, .Item("Udm_Cod_Extra"), Dr_Dettagli.Udm_Sim_Extra)

                                        Dim udmcore As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R
                                        Dr_Dettagli.Udm_Des_Extra = udmcore.UdmDes_from_UdmCod(.Item("Udm_Cod_Extra"), Dr_Dettagli.Udm_Sim_Extra, objParametri_Server)

                                    End If
                                    Dr_Dettagli.Qta_Extra = .Item("Qta_Extra")
                                    'Dr_Dettagli.Qta_Extra_Totale=

                                    x_Imponibile = objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, CDbl(.Item("Imponibile")))
                                    x_Imponibile_Netto = objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, CDbl(.Item("Imponibile_Netto")))

                                    x_Sconto = x_Imponibile_Netto - x_Imponibile

                                    'x_Aliquota = objContabHLP.Aliquota_from_CodIVA(.Item("Cod_Iva"))
                                    x_Aliquota = .Item("Sigla_Iva")
                                    x_IVA = objContabHLP.Leggi_IVA_PositivaNegativa(Lav_Cod, CDbl(.Item("Iva")))

                                    x_Importo = x_Imponibile_Netto + x_IVA

                                    'Dr_Dettagli.Prezzo_Unitario=
                                    'Dr_Dettagli.Prezzo_Unitario_Netto=
                                    Dr_Dettagli.Sconto_Percentuale = .Item("Sconto")
                                    Dr_Dettagli.Sconto = Format(Arrotonda_2Decimali(x_Sconto), "#,###,##0.00")

                                    Dr_Dettagli.Imponibile = Format(Arrotonda_2Decimali(x_Imponibile), "#,###,##0.00")
                                    Dr_Dettagli.Imponibile_Netto = Format(Arrotonda_2Decimali(x_Imponibile_Netto), "#,###,##0.00")
                                    'Dr_Dettagli.Prezzo_Effettivo=
                                    Dr_Dettagli.Iva_Percentuale = x_Aliquota
                                    'Dr_Dettagli.Iva_Indetraibile_Perc=
                                    Dr_Dettagli.Iva = Format(Arrotonda_2Decimali(x_IVA), "#,###,##0.00")

                                    Dr_Dettagli.Importo = Format(Arrotonda_2Decimali(x_Importo), "#,###,##0.00")

                                    If i = 0 Then
                                        Memo_Iva_Perc = .Item("Cod_Iva")
                                        Memo_Udm_Cod = .Item("Udm_Cod")
                                        Memo_Udm_Sim = .Item("Udm_Sim")
                                        Tot_Qta += .Item("Qta")
                                    Else
                                        If .Item("Cod_Iva") <> Memo_Iva_Perc Then
                                            Log_Errori += "- Lettura del dettaglio " + CStr(i + 1) + ": " + vbCrLf + _
                                                            "Il codice dell'Iva è diverso da quello del primo dettaglio!" + vbCrLf + vbCrLf
                                        End If
                                        If .Item("Udm_Cod") <> Memo_Udm_Cod Then
                                            Log_Errori += "- Lettura del dettaglio " + CStr(i + 1) + ": " + vbCrLf + _
                                                            "L'unità di misura è diversa da quella del primo dettaglio!" + vbCrLf + vbCrLf
                                        Else
                                            Tot_Qta += .Item("Qta")
                                        End If
                                    End If

                                    'Dr_Dettagli.Cod_Conto"				type="xs:int" minOccurs="0" />
                                    'Dr_Dettagli.Conto"					type="xs:string" minOccurs="0" />
                                    'Dr_Dettagli.Anno"						type="xs:string" minOccurs="0" />
                                    'Dr_Dettagli.Ric_Cod"					type="xs:int" minOccurs="0" />
                                    'Dr_Dettagli.Tara"						type="xs:string" minOccurs="0" />
                                    'Dr_Dettagli.Variazione"				type="xs:string" minOccurs="0" />
                                    'Dr_Dettagli.ChkLayOut_Hide"			type="xs:int" minOccurs="0" />
                                    'Dr_Dettagli.Listino_Cod"				type="xs:int" minOccurs="0" />
                                    'Dr_Dettagli.Listino_Des"				type="xs:string" minOccurs="0" />
                                    'Dr_Dettagli.Extra_Int"				type="xs:int" minOccurs="0" />
                                    'Dr_Dettagli.Extra_Date"				type="xs:string" minOccurs="0" />
                                    'Dr_Dettagli.Extra_Str"				type="xs:string" minOccurs="0" />
                                    'Dr_Dettagli.Extra_Str_1"				type="xs:string" minOccurs="0" />
                                    'Dr_Dettagli.Extra_Str_2"				type="xs:string" minOccurs="0" />
                                    'Dr_Dettagli.Extra_Str_3"				type="xs:string" minOccurs="0" />
                                    'Dr_Dettagli.Sa_Cod"					type="xs:int" minOccurs="0" />
                                    'Dr_Dettagli.Id_Mov"					type="xs:int" minOccurs="0" />
                                    'Dr_Dettagli.Id_Mov_Det"				type="xs:int" minOccurs="0" />

                                    Tot_ImponibileNetto += CDbl(.Item("Imponibile_Netto"))

                                Else
                                    '------------------------------------
                                    '------ RAGGRUPPAMENTO DETTAGLI -----
                                    '------------------------------------
                                    Log_Errori += "Raggruppamento dei dettagli non gestito!" + vbCrLf + vbCrLf
                                End If

                            Else
                                'il dettaglio non deve essere visualzizato
                            End If

                        End With

                        '-----------------------------
                        DS.Dettagli.Rows.Add(Dr_Dettagli)
                        '-----------------------------

                    Catch ex As Exception
                        Log_Errori += "- Gestione del dettaglio " + CStr(i + 1) + ": " + vbCrLf + ex.Message + vbCrLf + vbCrLf
                    End Try

                Next

                '=======================================================================

                Dr_Intestazione.Imponibile_Totale = Format(Arrotonda_2Decimali(Tot_ImponibileNetto), "#,###,##0.00")
                Dr_Intestazione.Iva_Percentuale = Memo_Iva_Perc
                Dr_Intestazione.Iva_Totale = Format(Arrotonda_2Decimali(Dr_Intestazione.Totale_Fattura - Tot_ImponibileNetto), "#,###,##0.00")

                'Dr_Intestazione.UdmCod_Totale = .Item("Stato_Vettore")
                'Dr_Intestazione.UdmDes_Totale = .Item("Stato_Vettore")
                Dr_Intestazione.UdmSim_Totale = Memo_Udm_Sim
                Dr_Intestazione.Qta_Totale = Tot_Qta
                'Dr_Intestazione.Qta_Totale_Cifre = .Item("Stato_Vettore")
            Else
                '-----------------------------
                Dr_Dettagli = DS.Dettagli.NewDettagliRow
                Dr_Dettagli.Contatore = 1
                DS.Dettagli.Rows.Add(Dr_Dettagli)
                '-----------------------------
            End If

        Else
            '-----------------------------
            Dr_Dettagli = DS.Dettagli.NewDettagliRow
            Dr_Dettagli.Contatore = 1
            DS.Dettagli.Rows.Add(Dr_Dettagli)
            '-----------------------------
        End If 'Flag_SoloIntestazione

        '####################################################

        DS.Intestazione.Rows.Add(Dr_Intestazione)

        '####################################################


        Try

            'imposto il datset sul report
            'RptDOCO.SetDataSource(DS)
            rptDoco_Layout.SetDataSource(DS)
            rptDoco_NoLayout.SetDataSource(DS)


        Catch ex As Exception
            Log_Errori += "- Aggancio dataset: " + vbCrLf + ex.Message + vbCrLf + vbCrLf
        End Try


        'Return RptDOCO


    End Sub



End Class
