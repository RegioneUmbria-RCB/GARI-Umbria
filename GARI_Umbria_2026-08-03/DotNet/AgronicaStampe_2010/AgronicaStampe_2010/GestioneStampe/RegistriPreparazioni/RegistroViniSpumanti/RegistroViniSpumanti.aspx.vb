Imports CrystalDecisions.Shared
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza

Public Class RegistroViniSpumanti
    Inherits System.Web.UI.Page

    '----------------------------
    '   DICHIARAZIONE REPORT
    '----------------------------
    Private rptSpumanti As Rpt_RegistroViniSpumanti

    '----- Gestione Querystring
    Dim Qs_Piva As String
    Dim Qs_RagSoc As String
    Dim Qs_Sa_Cod As String
    Dim Qs_Id_Agenda As String
    Dim Qs_Id_Mov As String
    Dim QS_Report As String  
    Dim QS_StampaIntestazione As String
    Dim Qs_Partita As String

    Dim Log_Errori As String = ""
    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri


#Region " INIT SPUMANTI "

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

        'istanzio gli oggetti report
        rptSpumanti = New Rpt_RegistroViniSpumanti
    End Sub

#End Region

    '#######################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'Faccio scadere subito la pagina memorizzata nella cache
        Response.Expires = 0

        '#################################################################################
        '#####  Recupero dati dalla QueryString 
        '#################################################################################

        Qs_Piva = Stringa_Decodifica(CStr(Request.QueryString("p")), _
                                    AgroKey_EncoderDecoder, _
                                    Server)

        Qs_RagSoc = Stringa_Decodifica(CStr(Request.QueryString("rs")), _
                                        AgroKey_EncoderDecoder, _
                                        Server)

        Qs_Sa_Cod = Stringa_Decodifica(Request.QueryString("s").ToString, _
                                       AgroKey_EncoderDecoder, _
                                       Server)

        QS_Report = Stringa_Decodifica(Request.QueryString("rp").ToString, _
                                       AgroKey_EncoderDecoder, _
                                       Server)

        Qs_Id_Agenda = Stringa_Decodifica(Request.QueryString("id_agenda").ToString, _
                                       AgroKey_EncoderDecoder, _
                                       Server)

        Qs_Id_Mov = Stringa_Decodifica(Request.QueryString("id_mov").ToString, _
                                       AgroKey_EncoderDecoder, _
                                       Server)

        Qs_Partita = Stringa_Decodifica(Request.QueryString("partita").ToString, _
                                       AgroKey_EncoderDecoder, _
                                       Server)

        QS_StampaIntestazione = Stringa_Decodifica(Request.QueryString("si").ToString, _
                                                   AgroKey_EncoderDecoder, _
                                                   Server)


        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))




        '#################################################################################
        '#####  Genero il report
        '#################################################################################


        Dim Nome_Documento As String = "Registro_Vini_Spumanti"
        Dim IdentificazioneDocumento As String = ""
        Dim cat_cod As enum_CategorieDocumenti

        cat_cod = enum_CategorieDocumenti.Cantina_RegistroSpumanti

        If Not Me.IsPostBack Then


            Dim objStampe As New AgronicaCoreStampeDAL.RegistriCantina2
            Dim Id_Trasformazione As Integer = 0
            Dim Flag_SpumBottiglie As Boolean
            Dim Trasformazione_Des As String = ""
            ' Dim Msg_Errori As String = ""
            Dim DT_Note As DataTable
            Dim DT_Perdite As DataTable

            DT_Note = Note_CaricaDT()
            DT_Perdite = Perdite_CaricaDT()

            Try
                _1_CostituzionePartita(objStampe, DT_Note, Log_Errori, Id_Trasformazione, Flag_SpumBottiglie, Trasformazione_Des)

            Catch ex As Exception
                Log_Errori += "- CostituzionePartita: " + vbCrLf + ex.Message + vbCrLf
            End Try

            If Id_Trasformazione = 0 Then
                Log_Errori += "Id_Trasformazione=0"
            Else

                Dim indicePerdite As Integer = 0
                Dim QtaPerdite As Double = 0

                Try
                    _2_Acidificazioni(objStampe, DT_Note, Log_Errori, Id_Trasformazione, Trasformazione_Des)

                Catch ex As Exception
                    Log_Errori += "- Acidificazioni: " + vbCrLf + ex.Message + vbCrLf
                End Try

                Try
                    _3a_AggiuntaSciroppoDosaggio(objStampe, DT_Note, Log_Errori, Id_Trasformazione)

                Catch ex As Exception
                    Log_Errori += "- AggiuntaSciroppoDosaggio: " + vbCrLf + ex.Message + vbCrLf
                End Try

                'Try
                '    _3b_Arricchimenti(objStampe, DT_Note, Log_Errori, Id_Trasformazione, Trasformazione_Des)

                'Catch ex As Exception
                '    Log_Errori += "- Arricchimenti: " + vbCrLf + ex.Message + vbCrLf
                'End Try

                Try
                    _4_PerditeElaborazione(objStampe, DT_Note, DT_Perdite, Log_Errori, Id_Trasformazione)

                Catch ex As Exception
                    Log_Errori += "- PerditeElaborazione: " + vbCrLf + ex.Message + vbCrLf
                End Try

                Try

                    _5_Travasi(objStampe, DT_Note, DT_Perdite, Log_Errori, Id_Trasformazione)

                Catch ex As Exception
                    Log_Errori += "- Travasi: " + vbCrLf + ex.Message + vbCrLf
                End Try


                Try
                    'indicePerdite, QtaPerdite,
                    _6_Imbottigliamento(objStampe, DT_Note, DT_Perdite, Log_Errori, Id_Trasformazione, Flag_SpumBottiglie)

                Catch ex As Exception
                    Log_Errori += "- Imbottigliamento: " + vbCrLf + ex.Message + vbCrLf
                End Try


                Try
                    Visualizza_PerditeElaborazione(DT_Perdite, Log_Errori)

                Catch ex As Exception
                    Log_Errori += "- Visualizza_PerditeElaborazione: " + vbCrLf + ex.Message + vbCrLf
                End Try

                Try
                    Visualizza_Note(DT_Note, Log_Errori)

                Catch ex As Exception
                    Log_Errori += "- Visualizza_Note: " + vbCrLf + ex.Message + vbCrLf
                End Try

            End If

            Try

                '-------------------------------------
                '---------- INTESTAZIONE ------------
                '-------------------------------------
                CType(rptSpumanti.Section2.ReportObjects("TextPartita"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Qs_Partita

                If QS_StampaIntestazione = "1" Then
                    CType(rptSpumanti.Section1.ReportObjects("TextRagSoc"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Qs_RagSoc + " " + Qs_Piva
                    rptSpumanti.Section6.SectionFormat.EnableSuppress = True
                Else
                    rptSpumanti.Section1.SectionFormat.EnableSuppress = True
                End If

            Catch ex As Exception
                Log_Errori += "- stampa intestazione: " + vbCrLf + ex.Message + vbCrLf
            End Try

            Try

                If Log_Errori <> "" Then

                    CType(rptSpumanti.Section2.ReportObjects("TextMsgErrori"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Log_Errori

                End If

            Catch ex As Exception
                Log_Errori += "- Gestione errori: " + vbCrLf + ex.Message + vbCrLf
            End Try


            '-----------------------------------------
            '---- Gestione Salvataggio PDF -----------  
            '-----------------------------------------
            Try

                Dim Data_Inizio_Allegati, Data_Fine_Allegati As Date

                Data_Inizio_Allegati = AGRODATAINIZIO
                Data_Fine_Allegati = AGRODATAFINE
                IdentificazioneDocumento += "_" + Format(Data_Inizio_Allegati, "yyyy_MM_dd") + "_" + Format(Data_Fine_Allegati, "yyyy_MM_dd")


                ' leggo la sottocartella da CategorieDocumenti
                Dim Sottocartella As String
                Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
                Sottocartella = objCatDoc.Sottocartella(cat_cod, "", "", objParametri_Server)

                ' salvo il report in formato PDF
                Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
                objGestFile.SalvaReportPdf(rptSpumanti, _
                                           cat_cod, _
                                           Sottocartella, _
                                           Nome_Documento + "_p" & Qs_Piva + "_" + IdentificazioneDocumento + ".pdf", _
                                           objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)

                Dim objAllegati As New AgronicaCoreScadenziario_BIZ.Allegati
                Dim AllegatiDocumentiCod As Integer

                AllegatiDocumentiCod = objAllegati.SalvaAllegato(Qs_Piva, _
                                                                 cat_cod, _
                                                                 Nome_Documento, _
                                                                 Nome_Documento + "_p" & Qs_Piva + "_" + IdentificazioneDocumento + ".pdf", _
                                                                 Sottocartella, _
                                                                 "", "", "", "", _
                                                                 Data_Inizio_Allegati, _
                                                                 Data_Fine_Allegati, _
                                                                 objParametri_Server)


            Catch ex As Exception
                Log_Errori += "- Gestione allegati: " + vbCrLf + ex.Message + vbCrLf
            End Try


            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            Dim Nome_File As String

            If Log_Errori <> "" Then

                Log_Errori = Nome_Documento + ", " + vbCrLf + _
                                "Piva = " + CStr(Qs_Piva) + ", " + vbCrLf + _
                                "Rag_Soc = " + CStr(Qs_RagSoc) + ", " + vbCrLf + _
                                "Sa_Cod = " + CStr(Qs_Sa_Cod) + ", " + vbCrLf + _
                                "Id_Agenda = " + CStr(Qs_Id_Agenda) + ", " + vbCrLf + _
                                "Id_Mov = " + CStr(Qs_Id_Mov) + ", " + vbCrLf + _
                                "Partita = " + CStr(Qs_Partita) + " " + vbCrLf + _
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
                                                 "RegistroVinificazione.aspx", _
                                                 Log_Errori)



            End If
            '-----------------------------------------
        End If

        '==================================================================

        Try
            Session("Report") = rptSpumanti
            Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server))
        Catch ex As Exception
            Log_Errori += "- Export: " + vbCrLf + ex.Message + vbCrLf
        End Try



    End Sub

    '####################################################################################################################
    Private Sub _1_CostituzionePartita(ByRef objStampe As AgronicaCoreStampeDAL.RegistriCantina2, _
                                    ByRef DT_Note As DataTable, _
                                    ByRef Msg_Errori As String, _
                                    ByRef Id_Trasformazione As Integer, _
                                    ByRef Flag_SpumBottiglie As Boolean, _
                                    ByRef Trasformazione_Des As String)

        'ByRef Cod_Contatto_Terzi As String, _
        'ByRef Rag_Soc_Terzi As String)

        Dim objAgenda As New AgronicaCoreContabDAL.Agenda_R
        Dim Dt As DataTable
        Dim i As Integer
        Dim objGenAnag As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Log_R
        Dim DtAnag As DataTable
        'Dim Id_Trasformazione As Integer = 0
        'Dim Elem_Cod_Lav As Integer = 0
        'Dim Mat_Cod_Lav As Integer = 0
        'Dim Elem_Cod_Trasf As Integer = 0
        'Dim Mat_Cod_Trasf As Integer = 0

        Id_Trasformazione = 0

        Try

            ' recupero l'id_trasformazione dall'agenda
            ' mi serve per recuperare tutte le operazioni di agenda di quella linea produttiva 

            Dt = objAgenda.Leggi(Qs_Piva, _
                                    0, _
                                    Qs_Id_Agenda, _
                                    0, _
                                    AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                    "", "", objParametri_Server)

            If Not IsNothing(Dt) AndAlso Dt.Rows.Count > 0 Then
                Id_Trasformazione = Dt.Rows(0).Item("Id_Trasformazione")
            End If

            Dt.Dispose()
            Dt = Nothing

        Catch ex As Exception
            Log_Errori += "recupero l'id_trasformazione dall'agenda: " + vbCrLf + ex.Message + vbCrLf
        End Try


        Try

            ' lettura inizio spumantizzazione
            Dt = objStampe.RegistroFrizzanti_CostituzionePartita(Qs_Piva, _
                                                                Qs_Id_Agenda, _
                                                                objParametri_Server)

        Catch ex As Exception
            Log_Errori += "lettura CostituzionePartita: " + vbCrLf + ex.Message + vbCrLf
        End Try

        Dim Dr_Mov As DataRow()

        ' seleziono gli scarichi
        Dr_Mov = Dt.Select("CAU_MOV='" & CAU_SCARICO & "'")

        'Dim DataPrec As DateTime

        Dim i_Base_min As Integer = 1
        Dim i_Base_max As Integer = 3
        ' Dim i_Arr_min As Integer = 4
        'Dim i_Arr_max As Integer = 5
        Dim i_Scir_Min As Integer = 6
        Dim i_Scir_Max As Integer = 7

        Dim QtaScarico As Double = 0
        Dim QtaCarico As Double = 0

        '   Const NumRigheCostPartita As Integer = 7

        Dim Modello, registro As String
        Dim Qta As Double
        Dim mat_des As String
        'Dim Trasformazione_Des As String = ""

        Dim CantineHLP As New AgronicaCoreContabHLP.Cantine

        'COMPILAZIONE DEL RIQUADRO B
        'E DEL RIQUADRO 1. COSTITUZIONE DELLA PARTITA
        If Not IsNothing(Dr_Mov) AndAlso Dr_Mov.Length > 0 Then

            'For i = 0 To Math.Min(Dr_Mov.Length, NumRigheCostPartita) - 1
            For i = 0 To Dr_Mov.Length - 1

                Modello = Dr_Mov(i).Item("Modello")

                registro = CantineHLP.Registro_from_ModelloLineaFrizSpum(Modello)

                Qta = Dr_Mov(i).Item("Qta")
                QtaScarico += Qta

                Trasformazione_Des = Dr_Mov(i).Item("Trasformazione_Des")


                Select Case Dr_Mov(i).Item("Tipo_Generazione")

                    Case enum_Omni_Tipo_Generazione.SemilavoratiMateriePrime, _
                    enum_Omni_Tipo_Generazione.ProdottoFinito

                        If i_Base_min <= i_Base_max Then

                            If Not IsDBNull(Dr_Mov(i).Item("Data_Movimento")) Then
                                CType(rptSpumanti.Section2.ReportObjects("TextData" & (i_Base_min).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                                    CDate(Dr_Mov(i).Item("Data_Movimento")).ToShortDateString
                            End If

                            CType(rptSpumanti.Section2.ReportObjects("TextRecipiente" & (i_Base_min).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                                    Dr_Mov(i).Item("Identificativo")

                            CType(rptSpumanti.Section2.ReportObjects("TextTipo" & (i_Base_min).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                                                        registro

                            If Not IsDBNull(Dr_Mov(i).Item("Mat_Des")) Then
                                CType(rptSpumanti.Section2.ReportObjects("TextProdottiBaseRiga" & (i_Base_min).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                                    Dr_Mov(i).Item("Mat_Des")
                            End If

                            CType(rptSpumanti.Section2.ReportObjects("TextLitri" & (i_Base_min).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                                 Format(Qta, "0.00")

                            i_Base_min += 1

                        Else
                            Msg_Errori += "1. Cost Partita: superato num max di righe 'prodotti di base'" & vbCrLf
                        End If


                    Case enum_Omni_Tipo_Generazione.AltreMateriePrime

                        If i_Scir_Min <= i_Scir_Max Then

                            mat_des = Dr_Mov(i).Item("Mat_Des")

                            If CStr(Dr_Mov(i).Item("Lotto")).ToLower <> "indefinito" And _
                                        CStr(Dr_Mov(i).Item("Lotto")).ToLower <> Trasformazione_Des.ToLower Then
                                mat_des += " " & Dr_Mov(i).Item("Lotto")
                            End If

                            If Not IsDBNull(Dr_Mov(i).Item("Data_Movimento")) Then
                                CType(rptSpumanti.Section2.ReportObjects("TextData" & (i_Scir_Min).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                                    CDate(Dr_Mov(i).Item("Data_Movimento")).ToShortDateString
                            End If

                            CType(rptSpumanti.Section2.ReportObjects("TextRecipiente" & (i_Scir_Min).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                               Dr_Mov(i).Item("Identificativo")

                            If Not IsDBNull(Dr_Mov(i).Item("Mat_Des")) Then
                                CType(rptSpumanti.Section2.ReportObjects("TextProdottiBaseRiga" & (i_Scir_Min).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                                    mat_des
                            End If

                            CType(rptSpumanti.Section2.ReportObjects("TextLitri" & (i_Scir_Min).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                                Format(Qta, "0.00")

                            i_Scir_Min += 1

                        Else
                            Msg_Errori += "1. Cost Partita: superato num max di righe 'aggiunta sciroppo zuccherino'" & vbCrLf
                        End If


                End Select

                'If Not IsDBNull(Dr_Mov(i).Item("Identificativo")) Then
                '    indiceBase += 1
                '    indice = indiceBase
                'Else
                '    indiceSpuma += 1
                '    indice = indiceSpuma
                'End If

                'If Not IsDBNull(Dr_Mov(i).Item("Identificativo")) Then

                'modifica del 4 marzo 2013: il numero recipiente di provenienza non deve coincidere con quello di elaborazione
                'ora sono nel filtro scarico
                'quindi sto leggendo al vasca di provenienza
                'CType(rptSpumanti.Section2.ReportObjects("TextRecipiente" & (indice).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                '    Dr_Mov(i).Item("Identificativo")
                ''End If

                ''1. COSTITUZIONE DELLA PARTITA
                'If Dr_Mov(i).Item("Data_Movimento") <> DataPrec Then

                '    If Not IsDBNull(Dr_Mov(i).Item("Data_Movimento")) Then
                '        CType(rptSpumanti.Section2.ReportObjects("TextData" & (indice).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                '            CDate(Dr_Mov(i).Item("Data_Movimento")).ToShortDateString
                '    End If

                '    DataPrec = Dr_Mov(i).Item("Data_Movimento")

                'End If

                'If Not IsDBNull(Dr_Mov(i).Item("Mat_Des")) Then
                '    CType(rptSpumanti.Section2.ReportObjects("TextProdottiBaseRiga" & (indice).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                '        Dr_Mov(i).Item("Mat_Des")
                'End If

                'If Not IsDBNull(Dr_Mov(i).Item("Qta")) Then
                '    CType(rptSpumanti.Section2.ReportObjects("TextLitri" & (indice).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                '         Format(Dr_Mov(i).Item("Qta"), "0.00")
                'End If

                ' QtaScarico += Dr_Mov(i).Item("Qta")

            Next

        End If

        ' seleziono i carichi
        Dr_Mov = Dt.Select("CAU_MOV='" & CAU_CARICO & "'")


        Dim QtaPerdite As Double = 0
        Dim i_Autoclave As Integer = 1
        Dim i_Autoclave_Max = 6
        Dim linea_des As String = ""
        Dim Cod_Contatto_Terzi As String = ""
        Dim Rag_soc_Terzi As String = ""

        If Not IsNothing(Dr_Mov) AndAlso Dr_Mov.Length > 0 Then

            For i = 0 To Dr_Mov.Length - 1

                Qta = Dr_Mov(i).Item("Qta")

                linea_des = Dr_Mov(i).Item("Linea_Des")
                Rag_soc_Terzi = Dr_Mov(i).Item("ContoTerzi")
                Cod_Contatto_Terzi = Dr_Mov(i).Item("Cod_Contatto_Terzi")

                Select Case Dr_Mov(i).Item("Tipo_Generazione")

                    Case enum_Omni_Tipo_Generazione.SemilavoratiMateriePrime, _
                        enum_Omni_Tipo_Generazione.ProdottoFinito

                        QtaCarico += Qta

                        If i_Autoclave <= i_Autoclave_Max Then

                            ' 
                            If CStr(Dr_Mov(i).Item("Identificativo")).ToUpper <> "MAGAZZINO" Then

                                Flag_SpumBottiglie = False

                                CType(rptSpumanti.Section2.ReportObjects("TextRecipienteB" & (i_Autoclave).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                                     "AUTOCLAVE N. " & CStr(Dr_Mov(i).Item("Identificativo"))

                            Else
                                Flag_SpumBottiglie = True
                            End If

                            i_Autoclave += 1

                        Else
                            Msg_Errori += "B. Recipienti: superato num max di righe." & vbCrLf
                        End If


                    Case enum_Omni_Tipo_Generazione.CaliLavorazione

                        QtaPerdite += Qta


                End Select

                '' se non è un Calo di Trasformazione o una perdita di elaborazione
                'If (Dr_Mov(i).Item("Elem_Cod") <> Elem_Cod_Trasf And Dr_Mov(i).Item("Mat_Cod") <> Mat_Cod_Trasf) And _
                '   (Dr_Mov(i).Item("Elem_Cod") <> Elem_Cod_Lav And Dr_Mov(i).Item("Mat_Cod") <> Mat_Cod_Lav) Then

                '    ' sezione B
                '    If CStr(Dr_Mov(i).Item("Identificativo")).ToUpper <> "MAGAZZINO" Then

                '        Flag_SpumBottiglie = False

                '        CType(rptSpumanti.Section2.ReportObjects("TextRecipienteB" & (i_Autoclave).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                '             "AUTOCLAVE N. " & CStr(Dr_Mov(i).Item("Identificativo"))

                '    Else
                '        Flag_SpumBottiglie = True
                '    End If

                '    i_Autoclave += 1

                '    'correzione bug del 19/02/2015
                '    'If Not IsDBNull(Dr_Mov(i).Item("Qta")) Then
                '    '    CType(rptSpumanti.Section2.ReportObjects("TextLitriFrizzante"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                '    '        Format(Dr_Mov(i).Item("Qta"), "0.00")
                '    'End If
                '    QtaCarico += Dr_Mov(i).Item("Qta")

                '    'modifica del 19/02/2015:
                '    ''CType(rptSpumanti.Section2.ReportObjects("TextDesignazione"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                '    ''   CStr(Dr_Mov(i).Item("Mat_Des")).ToUpper
                '    'CType(rptSpumanti.Section2.ReportObjects("TextDesignazione"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                '    '        CStr(Dr_Mov(i).Item("Linea_Des")).ToUpper

                '    linea_des = Dr_Mov(i).Item("Linea_Des")
                '    Rag_soc_Terzi = Dr_Mov(i).Item("ContoTerzi")
                '    Cod_Contatto_Terzi = Dr_Mov(i).Item("Cod_Contatto_Terzi")

                'Else
                '    'è un Calo di Trasformazione o una perdita di elaborazione
                '    QtaPerdite += Dr_Mov(i).Item("Qta")
                'End If

            Next 'carichi

        End If

        CType(rptSpumanti.Section2.ReportObjects("TextLitriSpumante"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                     Format(QtaCarico, "0.00")

        CType(rptSpumanti.Section2.ReportObjects("TextLitriPerdite"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                               Format(QtaPerdite, "0.00")

        CType(rptSpumanti.Section2.ReportObjects("TextDesignazione"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                      CStr(linea_des).ToUpper

        If Rag_soc_Terzi <> "" Then
            CType(rptSpumanti.Section2.ReportObjects("TxtContoTerzi"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                               Rag_soc_Terzi
            CType(rptSpumanti.Section2.ReportObjects("TxtContoTerziPiva"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                                Cod_Contatto_Terzi
        End If

        If Trasformazione_Des <> "" Then
            If IsNumeric(Left(Trasformazione_Des, 2)) Then
                CType(rptSpumanti.Section2.ReportObjects("TextAnnata"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                                       Left(Trasformazione_Des, 2)
            End If

        End If

        'era all'inizio

        '' salvo la variabile che mi servirà per caricare le altre operazioni
        'viewstate("Id_Trasformazione") = Id_Trasformazione

        'Try

        '    ' recupero il mat_cod dei Cali di Trasformazione [OGenerazioni_Anagrafe_Log]
        '    DtAnag = objGenAnag.Leggi_MateriaPrima(Qs_Piva, _
        '                                            -1, -1, _
        '                                            enum_Omni_Modulo_Generazione.Cantine, _
        '                                            enum_Omni_Tipo_Generazione.CaliLavorazione, _
        '                                            enum_Omni_Codice_Generazione.Tipo7_CaloTrasformazione, _
        '                                            AGRODATAINIZIO, AGRODATAFINE, _
        '                                            "", "", objParametri_Server)

        '    If Not IsNothing(DtAnag) AndAlso DtAnag.Rows.Count > 0 Then
        '        For i = 0 To DtAnag.Rows.Count - 1
        '            Elem_Cod_Trasf = DtAnag.Rows(0).Item("Elem_Cod")
        '            Mat_Cod_Trasf = DtAnag.Rows(0).Item("Mat_Cod")
        '        Next
        '    End If

        '    DtAnag.Dispose()
        '    DtAnag = Nothing

        'Catch ex As Exception
        '    Log_Errori += "recupero il mat_cod dei Cali di Trasformazione: " + vbCrLf + ex.Message + vbCrLf
        'End Try

        'Try

        '    ' recupero il mat_cod delle Perdite di Lavorazione [OGenerazioni_Anagrafe_Log]
        '    DtAnag = objGenAnag.Leggi_MateriaPrima(Qs_Piva, _
        '                                            -1, -1, _
        '                                            enum_Omni_Modulo_Generazione.Cantine, _
        '                                            enum_Omni_Tipo_Generazione.CaliLavorazione, _
        '                                            enum_Omni_Codice_Generazione.Tipo7_PerditaLavorazione, _
        '                                            AGRODATAINIZIO, AGRODATAFINE, _
        '                                            "", "", objParametri_Server)

        '    If Not IsNothing(DtAnag) AndAlso DtAnag.Rows.Count > 0 Then
        '        For i = 0 To DtAnag.Rows.Count - 1
        '            Elem_Cod_Lav = DtAnag.Rows(0).Item("Elem_Cod")
        '            Mat_Cod_Lav = DtAnag.Rows(0).Item("Mat_Cod")
        '        Next
        '    End If

        '    objGenAnag = Nothing
        '    DtAnag.Dispose()
        '    DtAnag = Nothing

        'Catch ex As Exception
        '    Log_Errori += "recupero il mat_cod delle Perdite di Lavorazione: " + vbCrLf + ex.Message + vbCrLf
        'End Try



    End Sub

    '####################################################################################################################
    Private Sub _4_PerditeElaborazione(ByRef objStampe As AgronicaCoreStampeDAL.RegistriCantina2, _
                                    ByRef DT_Note As DataTable, _
                                    ByRef DT_Perdite As DataTable, _
                                    ByRef Msg_Errori As String, _
                                    ByVal Id_Trasformazione As Integer)


        Dim DtPerdelab As DataTable
        Dim filtro As String = ""
        Dim i As Integer

        Try

            filtro += " ( " & _
                     " (Linee_Preparazioni.Codice_Generazione IN (" & _
                   CStr(enum_Omni_Preparazione_Cod.FineSpumantizzazioneAutoclave_RegCommercializzazione) & "," & _
                   CStr(enum_Omni_Preparazione_Cod.FineSpumantizzazioneAutoclave_RegVinificazione) & "," & _
                   CStr(enum_Omni_Preparazione_Cod.FineSpumantizzazioneAutoclaveMPFoVNAF_xVinoAttoADivenire_RegCommercializzazione) & "," & _
                   CStr(enum_Omni_Preparazione_Cod.FineSpumantizzazioneAutoclaveMPFoVNAF_xVinoAttoADivenire_RegVinificazione) & "," & _
                   CStr(enum_Omni_Preparazione_Cod.FineSpumantizzazioneAutoclaveMPFoVNAF_RegVinificazione) & "," & _
                   CStr(enum_Omni_Preparazione_Cod.FineSpumantizzazioneAutoclave_VinoAttoA_RegCommercializzazione) & "," & _
                   CStr(enum_Omni_Preparazione_Cod.FineSpumantizzazioneAutoclave_VinoAttoA_RegVinificazione) & "," & _
                   CStr(enum_Omni_Preparazione_Cod.FineSpumantizzazioneBottiglia) & "," & _
                   CStr(enum_Omni_Preparazione_Cod.FineSpumantizzazioneBottigliavinoAtto) & _
                   " ) ) " & _
                   " OR " & _
                   " Linee_Preparazioni.Tipo_Default = " & CStr(enum_Omni_TipoDefault_Preparazioni.RilevamentoCaliPerdite) & " " & _
                    " ) "

            'CStr(enum_Omni_Preparazione_Cod.ClassificazioneVinoAttoFineSpumantizzazioneInAutoclave_RegCommercializzazione) & "," & _
            'CStr(enum_Omni_Preparazione_Cod.ClassificazioneVinoAttoFineSpumantizzazioneInAutoclave_RegVinificazione) & "," & _


            DtPerdelab = objStampe.PerditeElaborazione(Qs_Piva, _
                                                        Id_Trasformazione, _
                                                        filtro, "", _
                                                        objParametri_Server)
        Catch ex As Exception
            Log_Errori += "lettura PerditeElaborazione: " + vbCrLf + ex.Message + vbCrLf
        End Try


        Dim desc, Data, preparazione_des As String

        If Not IsNothing(DtPerdelab) AndAlso DtPerdelab.Rows.Count > 0 Then

            For i = 0 To DtPerdelab.Rows.Count - 1

                Data = CDate(DtPerdelab.Rows(i).Item("Data_Movimento")).ToShortDateString

                preparazione_des = DtPerdelab.Rows(i).Item("preparazione_des")

                desc = Data & " " & preparazione_des

                Note_Inserisci_RigaDT(DT_Note, _
                                        DtPerdelab.Rows(i).Item("id_agenda"), _
                                        Data, _
                                        desc)

                Perdite_Inserisci_RigaDT(DT_Perdite, _
                                  DtPerdelab.Rows(i).Item("id_agenda"), _
                                  Data, _
                                  desc, _
                                  DtPerdelab.Rows(i).Item("qta"))


            Next

        End If




        'Dim DtPerdite As DataTable
        'Dim i As Integer
        'Dim ElencoAgenda As String = String.Empty

        ''MODIFICA DEL 29/11/13: gestita univocamente nel core, la lettura degli id_agenda e composizione stringa
        'Try

        '    Dim filtro_friz As String = ""

        '    filtro_friz += " Linee_Preparazioni.Modulo_Generazione = " & SQL_SaveNum(enum_Omni_Modulo_Generazione.Cantine)

        '    filtro_friz += " AND Linee_Preparazioni.Codice_Generazione IN (" & _
        '                    CStr(enum_Omni_Preparazione_Cod.FineSpumantizzazioneAutoclave_RegCommercializzazione) & "," & _
        '                    CStr(enum_Omni_Preparazione_Cod.FineSpumantizzazioneAutoclave_RegVinificazione) & "," & _
        '                    CStr(enum_Omni_Preparazione_Cod.FineSpumantizzazioneAutoclaveMPFoVNAF_xVinoAttoADivenire_RegCommercializzazione) & "," & _
        '                    CStr(enum_Omni_Preparazione_Cod.FineSpumantizzazioneAutoclaveMPFoVNAF_xVinoAttoADivenire_RegVinificazione) & "," & _
        '                    CStr(enum_Omni_Preparazione_Cod.FineSpumantizzazioneAutoclaveMPFoVNAF_RegVinificazione) & "," & _
        '                    CStr(enum_Omni_Preparazione_Cod.FineSpumantizzazioneAutoclave_VinoAttoA_RegCommercializzazione) & "," & _
        '                    CStr(enum_Omni_Preparazione_Cod.FineSpumantizzazioneAutoclave_VinoAttoA_RegVinificazione) & "," & _
        '                    CStr(enum_Omni_Preparazione_Cod.FineSpumantizzazioneBottiglia) & "," & _
        '                    CStr(enum_Omni_Preparazione_Cod.FineSpumantizzazioneBottigliavinoAtto) & _
        '                    " ) "

        '    ElencoAgenda = objStampe.RegistroFrizzanti_ElencoIdAgenda(Qs_Piva, _
        '                                                                Id_Trasformazione, _
        '                                                                0, _
        '                                                                filtro_friz, "", _
        '                                                                objParametri_Server)



        'Catch ex As Exception
        '    Log_Errori += "PerditeElaborazione_FineSpumantizzazione, recupero id_agenda: " + vbCrLf + ex.Message + vbCrLf
        'End Try


        'indicePerdite = 0
        'QtaPerdite = 0

        'If ElencoAgenda = "" Then


        'Else

        '    Try

        '        DtPerdite = objStampe.RegistroFrizzanti_LeggiPerdite_FineFrizzantatura(Qs_Piva, _
        '                                                                                ElencoAgenda, _
        '                                                                                "", "", _
        '                                                                                objParametri_Server)

        '        If Not IsNothing(DtPerdite) AndAlso DtPerdite.Rows.Count > 0 Then
        '            For i = 0 To DtPerdite.Rows.Count - 1

        '                CType(rptSpumanti.Section2.ReportObjects("TextDataPerdite" & (indicePerdite + 1).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
        '                      CDate(DtPerdite.Rows(i).Item("Data_Movimento")).ToShortDateString

        '                CType(rptSpumanti.Section2.ReportObjects("TextLitriPerdite" & (indicePerdite + 1).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
        '                      Format(DtPerdite.Rows(i).Item("Qta"), "0.00") '& " " & DtPerdite.Rows(i).Item("udm_sim")

        '                QtaPerdite += DtPerdite.Rows(i).Item("Qta")

        '                indicePerdite += 1

        '            Next
        '        End If

        '    Catch ex As Exception
        '        Log_Errori += "Lettura perdite nella fase di fine Spumantizzazione: " + vbCrLf + ex.Message + vbCrLf
        '    End Try

        'End If


    End Sub


    '####################################################################################################################
    Private Sub _6_Imbottigliamento(ByRef objStampe As AgronicaCoreStampeDAL.RegistriCantina2, _
                                ByRef DT_Note As DataTable, _
                                ByRef DT_Perdite As DataTable, _
                                ByRef Msg_Errori As String, _
                                ByVal Id_Trasformazione As Integer, _
                                ByVal Flag_SpumBottiglie As Boolean)

        '          ByRef indicePerdite As Integer, _
        'ByRef QtaPerdite As Double, _

        Dim DtImbottigliamento As DataTable
        Dim i As Integer


        Try

            Dim filtro As String = ""

            filtro += " Linee_Preparazioni.Codice_Generazione IN (" & _
                          CStr(enum_Omni_Preparazione_Cod.Imbottigliamento) & "," & _
                          CStr(enum_Omni_Preparazione_Cod.ImbottigliamentoVinoAttoADivenire) & "," & _
                          CStr(enum_Omni_Preparazione_Cod.ImbottigliamentoSenzaEtichettatura) & "," & _
                          CStr(enum_Omni_Preparazione_Cod.ImbottigliamentoSenzaEtichettaturaAttoADivenire) & "," & _
                          CStr(enum_Omni_Preparazione_Cod.ImbottigliamentoInLinee) & "," & _
                          CStr(enum_Omni_Preparazione_Cod.ImbottigliamentoDaLinee) & "," & _
                          CStr(enum_Omni_Preparazione_Cod.ImbottigliamentoInizioSpumantizzazioneBottiglia_RegVinificazione) & "," & _
                          CStr(enum_Omni_Preparazione_Cod.ImbottigliamentoInizioSpumantizzazioneBottiglia_RegCommercializzazione) & "," & _
                          CStr(enum_Omni_Preparazione_Cod.ImbottigliamentoInizioSpumantizzazioneBottigliaViniQualita_RegVinificazione) & "," & _
                          CStr(enum_Omni_Preparazione_Cod.ImbottigliamentoInizioSpumantizzazioneBottigliaViniQualita_RegCommercializzazione) & _
                          " ) "

            DtImbottigliamento = objStampe.Imbottigliamento(Qs_Piva, _
                                                Id_Trasformazione, _
                                                filtro, _
                                                objParametri_Server)


        Catch ex As Exception
            Log_Errori += "lettura Imbottigliamento: " + vbCrLf + ex.Message + vbCrLf
        End Try


        'Dim indiceImbott As Integer = 0


        If Not IsNothing(DtImbottigliamento) AndAlso DtImbottigliamento.Rows.Count > 0 Then

            Dim lotto As String = ""
            Dim i_imb_min As Integer = 1
            Dim i_imb_max As Integer = 8
            Dim num_bottiglie, capacita_bott As String
            'Dim qta As Double
            Dim i_recipiente As Integer = 1
            '    Dim QtaOrigine As Double = 0
            Dim QtaConfezionato As Double = 0

            'For i = 0 To Math.Min(DtImbottigliamento.Rows.Count, 8) - 1
            For i = 0 To DtImbottigliamento.Rows.Count - 1

                Select Case DtImbottigliamento.Rows(i).Item("Tipo_Generazione")

                    Case enum_Omni_Tipo_Generazione.ProdottoFinito

                        If i_imb_min <= i_imb_max Then

                            num_bottiglie = CStr(DtImbottigliamento.Rows(i).Item("Qta"))

                            capacita_bott = CStr(DtImbottigliamento.Rows(i).Item("QTA_EXTRA")) & " " & DtImbottigliamento.Rows(i).Item("udm_sim_extra")

                            CType(rptSpumanti.Section2.ReportObjects("TextDataImb" & (i_imb_min).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                                CDate(DtImbottigliamento.Rows(i).Item("Data_Movimento")).ToShortDateString

                            CType(rptSpumanti.Section2.ReportObjects("TextNum" & (i_imb_min).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                                num_bottiglie

                            CType(rptSpumanti.Section2.ReportObjects("TextMl" & (i_imb_min).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                                capacita_bott

                            If CStr(DtImbottigliamento.Rows(i).Item("Lotto")).ToLower <> "indefinito" And DtImbottigliamento.Rows(i).Item("Lotto") <> "" Then
                                lotto = " Lotto: " & DtImbottigliamento.Rows(i).Item("Lotto")
                            Else
                                lotto = ""
                            End If

                            CType(rptSpumanti.Section2.ReportObjects("TextMatDesImb" & (i_imb_min).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                                DtImbottigliamento.Rows(i).Item("Mat_Des") & lotto

                            'CASE WHEN Materie_Prime.Peso_Set>0 THEN Movimenti_dettagli.QTA_EXTRA*Movimenti_dettagli.QTA 
                            'WHEN Materie_Prime.Peso_Set=0 THEN Materie_Prime.QTA_EXTRA*Movimenti_dettagli.QTA 
                            'ELSE 0 END AS Qta_Confezionato, 
                            QtaConfezionato += DtImbottigliamento.Rows(i).Item("Qta_Confezionato")

                            'nel caso di Spumantizzazione in bottiglia
                            'imposto nel riquadro B le bottiglie
                            If Flag_SpumBottiglie = True Then
                                CType(rptSpumanti.Section2.ReportObjects("TextRecipienteB" & (i_recipiente).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                                "Bottiglie n. " & num_bottiglie & " x " & capacita_bott
                                i_recipiente += 1
                            End If

                            i_imb_min += 1

                        Else
                            Msg_Errori += "Imbottigliamenti: superato num max di righe." & vbCrLf
                        End If


                    Case enum_Omni_Tipo_Generazione.CaliLavorazione

                        'QtaPerdite += qta

                        Perdite_Inserisci_RigaDT(DT_Perdite, _
                                          DtImbottigliamento.Rows(i).Item("id_agenda"), _
                                          DtImbottigliamento.Rows(i).Item("Data_Movimento").ToShortDateString, _
                                          "Imbottigliamento", _
                                          DtImbottigliamento.Rows(i).Item("qta"))


                End Select


            Next

            CType(rptSpumanti.Section2.ReportObjects("TextLitriImbTot"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                  Format(QtaConfezionato, "0.00")

            CType(rptSpumanti.Section2.ReportObjects("TextTOTfinale"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                Format(QtaConfezionato, "0.00")

        End If


        'Dim DtAgenda As DataTable
        'Dim ElencoAgenda As String = String.Empty


        ''MODIFICA DEL 29/11/13: gestita univocamente nel core, la lettura degli id_agenda e composizione stringa
        'Try

        '    Dim filtro_friz As String = ""

        '    filtro_friz += " Linee_Preparazioni.Modulo_Generazione = " & SQL_SaveNum(enum_Omni_Modulo_Generazione.Cantine)
        '    filtro_friz += " AND Linee_Preparazioni.Codice_Generazione IN (" & _
        '                CStr(enum_Omni_Preparazione_Cod.Imbottigliamento) & "," & _
        '                CStr(enum_Omni_Preparazione_Cod.ImbottigliamentoInLinee) & "," & _
        '                CStr(enum_Omni_Preparazione_Cod.ImbottigliamentoDaLinee) & "," & _
        '                CStr(enum_Omni_Preparazione_Cod.ImbottigliamentoInizioSpumantizzazioneBottiglia_RegVinificazione) & "," & _
        '                CStr(enum_Omni_Preparazione_Cod.ImbottigliamentoInizioSpumantizzazioneBottiglia_RegCommercializzazione) & "," & _
        '                CStr(enum_Omni_Preparazione_Cod.ImbottigliamentoInizioSpumantizzazioneBottigliaViniQualita_RegVinificazione) & "," & _
        '                CStr(enum_Omni_Preparazione_Cod.ImbottigliamentoInizioSpumantizzazioneBottigliaViniQualita_RegCommercializzazione) & _
        '                " ) "


        '    ElencoAgenda = objStampe.RegistroFrizzanti_ElencoIdAgenda(Qs_Piva, _
        '                                                                Id_Trasformazione, _
        '                                                                0, _
        '                                                                filtro_friz, "", _
        '                                                                objParametri_Server)



        'Catch ex As Exception
        '    Log_Errori += "Imbottigliamento, recupero id_agenda: " + vbCrLf + ex.Message + vbCrLf
        'End Try


        'If ElencoAgenda = "" Then
        '    'non è un errore, possono non esserci, non stampo nel log
        '    'Log_Errori += "Non sono stati trovati imbottigliamenti per quella partita." + vbCrLf + vbCrLf
        'Else

        '    Dim objGenAnag As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Log_R
        '    Dim DtAnag As DataTable

        '    Try
        '        ' recupero il mat_cod dei cali di imbottigliamento da [OGenerazioni_Anagrafe_Log]
        '        DtAnag = objGenAnag.Leggi_MateriaPrima(Qs_Piva, _
        '                                                -1, -1, _
        '                                                enum_Omni_Modulo_Generazione.Cantine, _
        '                                                enum_Omni_Tipo_Generazione.CaliLavorazione, _
        '                                                enum_Omni_Codice_Generazione.Tipo7_CaloImbottigliamento, _
        '                                                AGRODATAINIZIO, AGRODATAFINE, _
        '                                                "", "", _
        '                                                objParametri_Server)

        '    Catch ex As Exception
        '        Log_Errori += "recupero il mat_cod dei cali di imbottigliamento: " + vbCrLf + ex.Message + vbCrLf
        '    End Try

        '    Dim Elem_Cod As Integer = 0
        '    Dim Mat_Cod As Integer = 0

        '    If Not IsNothing(DtAnag) AndAlso DtAnag.Rows.Count > 0 Then
        '        For i = 0 To DtAnag.Rows.Count - 1
        '            Elem_Cod = DtAnag.Rows(0).Item("Elem_Cod")
        '            Mat_Cod = DtAnag.Rows(0).Item("Mat_Cod")
        '        Next
        '    End If

        '    objGenAnag = Nothing
        '    DtAnag.Dispose()
        '    DtAnag = Nothing

        '    Dim QtaOrigine As Double = 0
        '    Dim QtaConfezionato As Double = 0

        '    Try
        '        ' Elenco degli imbottigliamenti
        '        DtImbottigliamento = objStampe.RegistroFrizzanti_Imbottigliamento(Qs_Piva, _
        '                                                                          ElencoAgenda, _
        '                                                                          objParametri_Server)

        '    Catch ex As Exception
        '        Log_Errori += "Elenco degli imbottigliamenti: " + vbCrLf + ex.Message + vbCrLf
        '    End Try



        '    Dim indiceImbott As Integer = 0

        '    If Not IsNothing(DtImbottigliamento) AndAlso DtImbottigliamento.Rows.Count > 0 Then

        '        Dim lotto As String = ""
        '        Dim i_Bottiglie As Integer = 1
        '        Dim num_bottiglie, capacita_bott As String

        '        'in data 31/01/2013 eliminata una riga per aumentare l'altezza delle righe
        '        'For i = 0 To Math.Min(DtImbottigliamento.Rows.Count, 10) - 1
        '        For i = 0 To Math.Min(DtImbottigliamento.Rows.Count, 8) - 1

        '            ' se non è un calo di imbottigliamento
        '            If DtImbottigliamento.Rows(i).Item("Elem_Cod") <> Elem_Cod And DtImbottigliamento.Rows(i).Item("Mat_Cod") <> Mat_Cod Then

        '                num_bottiglie = CStr(DtImbottigliamento.Rows(i).Item("Qta"))

        '                capacita_bott = CStr(DtImbottigliamento.Rows(i).Item("QTA_EXTRA")) & " " & DtImbottigliamento.Rows(i).Item("udm_sim_extra")

        '                CType(rptSpumanti.Section2.ReportObjects("TextDataImb" & (indiceImbott + 1).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
        '                    CDate(DtImbottigliamento.Rows(i).Item("Data_Movimento")).ToShortDateString

        '                CType(rptSpumanti.Section2.ReportObjects("TextNum" & (indiceImbott + 1).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
        '                    num_bottiglie

        '                CType(rptSpumanti.Section2.ReportObjects("TextMl" & (indiceImbott + 1).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
        '                    capacita_bott

        '                If CStr(DtImbottigliamento.Rows(i).Item("Lotto")).ToLower <> "indefinito" And DtImbottigliamento.Rows(i).Item("Lotto") <> "" Then
        '                    lotto = " Lotto: " & DtImbottigliamento.Rows(i).Item("Lotto")
        '                Else
        '                    lotto = ""
        '                End If

        '                CType(rptSpumanti.Section2.ReportObjects("TextMatDesImb" & (indiceImbott + 1).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
        '                    DtImbottigliamento.Rows(i).Item("Mat_Des") & lotto

        '                'CASE WHEN Materie_Prime.Peso_Set>0 THEN Movimenti_dettagli.QTA_EXTRA*Movimenti_dettagli.QTA 
        '                'WHEN Materie_Prime.Peso_Set=0 THEN Materie_Prime.QTA_EXTRA*Movimenti_dettagli.QTA 
        '                'ELSE 0 END AS Qta_Confezionato, 
        '                QtaConfezionato += DtImbottigliamento.Rows(i).Item("Qta_Confezionato")

        '                'nel caso di Spumantizzazione in bottiglia
        '                'imposto nel riquadro B le bottiglie
        '                If Flag_SpumBottiglie = True Then
        '                    CType(rptSpumanti.Section2.ReportObjects("TextRecipienteB" & (i_Bottiglie).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
        '                    "Bottiglie n. " & num_bottiglie & " x " & capacita_bott
        '                    i_Bottiglie += 1
        '                End If

        '                indiceImbott += 1

        '            Else
        '                ' Perdite di imbottigliamento
        '                CType(rptSpumanti.Section2.ReportObjects("TextDataPerdite" & (indicePerdite + 1).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
        '                      CDate(DtImbottigliamento.Rows(i).Item("Data_Movimento")).ToShortDateString

        '                CType(rptSpumanti.Section2.ReportObjects("TextLitriPerdite" & (indicePerdite + 1).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
        '                      Format(DtImbottigliamento.Rows(i).Item("Qta"), "0.00")

        '                QtaPerdite += DtImbottigliamento.Rows(i).Item("Qta")

        '                indicePerdite += 1

        '            End If

        '            '' quantità di vino scaricata utilizzata per calcolare le perdite
        '            'QtaOrigine += DtImbottigliamento.Rows(i).Item("Qta_Origine")

        '            'QtaConfezionato += DtImbottigliamento.Rows(i).Item("Qta_Confezionato")

        '            'QtaPerdite = DtImbottigliamento.Rows(i).Item("Qta_Origine") - DtImbottigliamento.Rows(i).Item("Qta_Confezionato")

        '            'If QtaPerdite <> 0 Then

        '            '    ' Perdite di imbottigliamento
        '            '    CType(rptSpumanti.Section2.ReportObjects("TextDataPerdite" & (indicePerdite + 1).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
        '            '        CDate(DtImbottigliamento.Rows(i).Item("Data_Movimento")).ToShortDateString

        '            '    CType(rptSpumanti.Section2.ReportObjects("TextLitriPerdite" & (indicePerdite + 1).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
        '            '          Format(QtaPerdite, "0.00")

        '            '    indicePerdite += 1

        '            'End If

        '        Next

        '    End If

        '    CType(rptSpumanti.Section2.ReportObjects("TextLitriImbTot"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
        '          Format(QtaConfezionato, "0.00")

        '    CType(rptSpumanti.Section2.ReportObjects("TextLitriPerditeTot"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
        '          Format(QtaPerdite, "0.00")

        '    CType(rptSpumanti.Section2.ReportObjects("TextTOTfinale"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
        '        Format(QtaConfezionato, "0.00")

        '    DtImbottigliamento.Dispose()
        '    DtImbottigliamento = Nothing

        'End If 'elenco agenda

    End Sub


    '####################################################################################################################
    ' acidificazioni e disacidificazioni TipoDefault 7
    Private Sub _2_Acidificazioni(ByRef objStampe As AgronicaCoreStampeDAL.RegistriCantina2, _
                                ByRef DT_Note As DataTable, _
                                ByRef Msg_Errori As String, _
                                ByVal Id_Trasformazione As Integer, _
                                ByRef Trasformazione_Des As String)

        Dim DtAcidificazioni As DataTable
        Dim i As Integer


        Try

            Dim filtro As String = ""

            filtro += " Linee_Preparazioni.Codice_Generazione IN (" & _
                            CStr(enum_Omni_Preparazione_Cod.AggiuntaAcidificanti) & _
                            " ) "

            DtAcidificazioni = objStampe.Acidificazioni(Qs_Piva, _
                                                            Id_Trasformazione, _
                                                            filtro, _
                                                            objParametri_Server)

        Catch ex As Exception
            Log_Errori += "lettura Acidificazioni: " + vbCrLf + ex.Message + vbCrLf
        End Try

        Dim i_acid_min As Integer = 1
        Dim i_acid_max As Integer = 6
        Dim mat_des As String

        If Not IsNothing(DtAcidificazioni) AndAlso DtAcidificazioni.Rows.Count > 0 Then

            'For i = 0 To Math.Min(DtAcidificazioni.Rows.Count, 8 - 1)
            For i = 0 To DtAcidificazioni.Rows.Count - 1

                If i_acid_min <= i_acid_max Then

                    mat_des = DtAcidificazioni.Rows(i).Item("Mat_Des")

                    If CStr(DtAcidificazioni.Rows(i).Item("Lotto")).ToLower <> "indefinito" And _
                        CStr(DtAcidificazioni.Rows(i).Item("Lotto")).ToLower <> Trasformazione_Des.ToLower Then
                        mat_des += " " & DtAcidificazioni.Rows(i).Item("Lotto")
                    End If

                    CType(rptSpumanti.Section2.ReportObjects("TextDataAC" & (i_acid_min).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                          CDate(DtAcidificazioni.Rows(i).Item("Data_Movimento")).ToShortDateString

                    CType(rptSpumanti.Section2.ReportObjects("TextProdottoAC" & (i_acid_min).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                         mat_des

                    CType(rptSpumanti.Section2.ReportObjects("TextQtaAC" & (i_acid_min).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                         Format(DtAcidificazioni.Rows(i).Item("Qta"), "0.00") '& " " & DtAcidificazioni.Rows(i).Item("Udm_Sim")

                    i_acid_min += 1

                Else
                    Msg_Errori += "Acidificazioni: superato num max di righe." & vbCrLf
                End If

            Next

        End If


        'Dim DtAgenda As DataTable
        'Dim ElencoAgenda As String = String.Empty
        ''MODIFICA DEL 29/11/13: gestita univocamente nel core, la lettura degli id_agenda e composizione stringa
        'Try

        '    Dim filtro_friz As String = ""

        '    filtro_friz += " Linee_Preparazioni.Modulo_Generazione = " & SQL_SaveNum(enum_Omni_Modulo_Generazione.Cantine)
        '    filtro_friz += " AND Linee_Preparazioni.Codice_Generazione IN (" & _
        '                    CStr(enum_Omni_Preparazione_Cod.AggiuntaAcidificanti) & _
        '                    " ) "

        '    ElencoAgenda = objStampe.RegistroFrizzanti_ElencoIdAgenda(Qs_Piva, _
        '                                                                Id_Trasformazione, _
        '                                                                0, _
        '                                                                filtro_friz, "", _
        '                                                                objParametri_Server)



        'Catch ex As Exception
        '    Log_Errori += "Acidificazioni, recupero id_agenda: " + vbCrLf + ex.Message + vbCrLf
        'End Try

        'If ElencoAgenda = "" Then
        '    'non è un errore, possono non esserci, non stampo nel log
        '    'Log_Errori += "Non sono state trovate acidificazioni per quella partita." + vbCrLf + vbCrLf
        'Else

        '    Try
        '        DtAcidificazioni = objStampe.RegistroFrizzanti_Acidificazioni(Qs_Piva, _
        '                                                                        ElencoAgenda, _
        '                                                                        objParametri_Server)

        '    Catch ex As Exception
        '        Log_Errori += "RegistroFrizzanti_Acidificazioni: " + vbCrLf + ex.Message + vbCrLf
        '    End Try

        '    If Not IsNothing(DtAcidificazioni) AndAlso DtAcidificazioni.Rows.Count > 0 Then
        '        For i = 0 To Math.Min(DtAcidificazioni.Rows.Count, 8 - 1)

        '            CType(rptSpumanti.Section2.ReportObjects("TextDataAC" & (i + 1).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
        '                  CDate(DtAcidificazioni.Rows(i).Item("Data_Movimento")).ToShortDateString

        '            CType(rptSpumanti.Section2.ReportObjects("TextProdottoAC" & (i + 1).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
        '                 DtAcidificazioni.Rows(i).Item("Mat_Des")

        '            CType(rptSpumanti.Section2.ReportObjects("TextQtaAC" & (i + 1).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
        '                 Format(DtAcidificazioni.Rows(i).Item("Qta"), "0.00") & " " & DtAcidificazioni.Rows(i).Item("Udm_Sim")

        '        Next
        '    End If

        '    DtAcidificazioni.Dispose()
        '    DtAcidificazioni = Nothing

        'End If

    End Sub

    '####################################################################################################################
    Private Sub _3a_AggiuntaSciroppoDosaggio(ByRef objStampe As AgronicaCoreStampeDAL.RegistriCantina2, _
                                            ByRef DT_Note As DataTable, _
                                            ByRef Msg_Errori As String, _
                                            ByVal Id_Trasformazione As Integer)

        Dim DtDolcificazioni As DataTable
        Dim filtro As String = ""
        Dim i As Integer


        Try

            'filtro += " Linee_Preparazioni.Codice_Generazione IN (" & _
            '                CStr(enum_Omni_Preparazione_Cod.Dolcificazione) & ", " & _
            '                CStr(enum_Omni_Preparazione_Cod.AggiuntaSciroppoDosaggio) & ", " & _
            '                CStr(enum_Omni_Preparazione_Cod.ArricchimentoConSaccarosio) & ", " & _
            '                CStr(enum_Omni_Preparazione_Cod.ArricchimentoConMCR) & " " & _
            '                " ) "

            filtro += " Linee_Preparazioni.Codice_Generazione IN (" & _
                            CStr(enum_Omni_Preparazione_Cod.Dolcificazione) & ", " & _
                            CStr(enum_Omni_Preparazione_Cod.AggiuntaSciroppoDosaggio) & " " & _
                            " ) "

            'filtro += " Linee_Preparazioni.Codice_Generazione IN (" & _
            '            CStr(enum_Omni_Preparazione_Cod.AggiuntaSciroppoDosaggio) & " " & _
            '            " ) "

            DtDolcificazioni = objStampe.Dolcificazioni_Arricchimenti(Qs_Piva, _
                                                                 Id_Trasformazione, _
                                                                 filtro, _
                                                                 objParametri_Server)
        Catch ex As Exception
            Log_Errori += "lettura AggiuntaSciroppoDosaggio: " + vbCrLf + ex.Message + vbCrLf
        End Try


        Dim i_sciroppo_min = 1
        Dim i_sciroppo_max = 10
        Dim desc, Data, preparazione_des As String

        If Not IsNothing(DtDolcificazioni) AndAlso DtDolcificazioni.Rows.Count > 0 Then

            For i = 0 To DtDolcificazioni.Rows.Count - 1

                Data = CDate(DtDolcificazioni.Rows(i).Item("Data_Movimento")).ToShortDateString

                preparazione_des = DtDolcificazioni.Rows(i).Item("preparazione_des")

                desc = Data & " " & preparazione_des

                Note_Inserisci_RigaDT(DT_Note, _
                                        DtDolcificazioni.Rows(i).Item("id_agenda"), _
                                        Data, _
                                        desc)

                If i_sciroppo_min <= i_sciroppo_max Then

                    CType(rptSpumanti.Section2.ReportObjects("TextDataSciroppo" & (i_sciroppo_min).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                          Data

                    'CType(rptSpumanti.Section2.ReportObjects("TextProdottoDolcificazione" & (i + 1).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                    '     DtDolcificazioni.Rows(i).Item("Mat_Des")

                    CType(rptSpumanti.Section2.ReportObjects("TextLitriSciroppo" & (i_sciroppo_min).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                         Format(DtDolcificazioni.Rows(i).Item("Qta"), "0.00") '& " " & DtDolcificazioni.Rows(i).Item("Udm_Sim")

                    i_sciroppo_min += 1

                Else
                    Msg_Errori += "AggiuntaSciroppoDosaggio: superato num max di righe." & vbCrLf
                End If

            Next

        End If


        ' Dim ElencoAgenda As String = String.Empty
        ' Dim DtAgenda As DataTable

        ''MODIFICA DEL 29/11/13: gestita univocamente nel core, la lettura degli id_agenda e composizione stringa
        'Try


        '    filtro += " Linee_Preparazioni.Modulo_Generazione = " & SQL_SaveNum(enum_Omni_Modulo_Generazione.Cantine)
        '    filtro += " AND Linee_Preparazioni.Codice_Generazione IN (" & _
        '                    CStr(enum_Omni_Preparazione_Cod.Dolcificazione) & ", " & _
        '                    CStr(enum_Omni_Preparazione_Cod.AggiuntaSciroppoZuccherino) & ", " & _
        '                    CStr(enum_Omni_Preparazione_Cod.ArricchimentoConSaccarosio) & ", " & _
        '                    CStr(enum_Omni_Preparazione_Cod.ArricchimentoConMCR) & " " & _
        '                    " ) "

        '    ElencoAgenda = objStampe.RegistroFrizzanti_ElencoIdAgenda(Qs_Piva, _
        '                                                                Id_Trasformazione, _
        '                                                                0, _
        '                                                                filtro, "", _
        '                                                                objParametri_Server)



        'Catch ex As Exception
        '    Log_Errori += "Dolcificazioni, recupero id_agenda: " + vbCrLf + ex.Message + vbCrLf
        'End Try


        'If ElencoAgenda = "" Then
        '    'non è un errore, possono non esserci, non stampo nel log
        '    'Log_Errori += "Non sono state trovate dolcificazioni per quella partita." + vbCrLf + vbCrLf
        'Else

        'Dim objGenAnag As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Log_R
        'Dim DtAnag As DataTable

        'Try
        '    ' recupero il mat_cod dell'MCR
        '    DtAnag = objGenAnag.Leggi(Qs_Piva, _
        '                                -1, -1, _
        '                                enum_Omni_Modulo_Generazione.Cantine, _
        '                                enum_Omni_Tipo_Generazione.AltreMateriePrime, _
        '                                enum_Omni_Codice_Generazione.Tipo9_MCR, _
        '                                AGRODATAINIZIO, AGRODATAFINE, _
        '                                "", "", _
        '                                objParametri_Server)

        'Catch ex As Exception
        '    Log_Errori += "recupero il mat_cod del mosto: " + vbCrLf + ex.Message + vbCrLf
        'End Try

        'Dim Elem_Cod As Integer = 0
        'Dim Mat_Cod As Integer = 0
        'If Not IsNothing(DtAnag) AndAlso DtAnag.Rows.Count > 0 Then
        '    For i = 0 To DtAnag.Rows.Count - 1
        '        Elem_Cod = DtAnag.Rows(0).Item("Elem_Cod")
        '        Mat_Cod = DtAnag.Rows(0).Item("Mat_Cod")
        '    Next
        'End If

        ' End If


    End Sub


    '####################################################################################################################
    Private Sub _3b_Arricchimenti(ByRef objStampe As AgronicaCoreStampeDAL.RegistriCantina2, _
                                            ByRef DT_Note As DataTable, _
                                            ByRef Msg_Errori As String, _
                                            ByVal Id_Trasformazione As Integer, _
                                            ByRef Trasformazione_Des As String)

        Dim DtDolcificazioni As DataTable
        Dim filtro As String = ""
        Dim i As Integer


        Try

            filtro += " Linee_Preparazioni.Codice_Generazione IN (" & _
                            CStr(enum_Omni_Preparazione_Cod.ArricchimentoConSaccarosio) & ", " & _
                            CStr(enum_Omni_Preparazione_Cod.ArricchimentoConMCR) & " " & _
                            " ) "


            DtDolcificazioni = objStampe.Dolcificazioni_Arricchimenti(Qs_Piva, _
                                                                 Id_Trasformazione, _
                                                                 filtro, _
                                                                 objParametri_Server)
        Catch ex As Exception
            Log_Errori += "lettura Arricchimenti: " + vbCrLf + ex.Message + vbCrLf
        End Try


        Dim i_Arr_min As Integer = 4
        Dim i_Arr_max As Integer = 5
        Dim desc, Data, mat_des As String
        Dim Qta As Double

        If Not IsNothing(DtDolcificazioni) AndAlso DtDolcificazioni.Rows.Count > 0 Then

            For i = 0 To DtDolcificazioni.Rows.Count - 1

                'Select Case DtDolcificazioni.Rows(i).Item("Tipo_Generazione")

                '    Case enum_Omni_Tipo_Generazione.AltreMateriePrime

                        If i_Arr_min <= i_Arr_max Then

                            mat_des = DtDolcificazioni.Rows(i).Item("Mat_Des")

                            Qta = DtDolcificazioni.Rows(i).Item("Qta")

                            If CStr(DtDolcificazioni.Rows(i).Item("Lotto")).ToLower <> "indefinito" And _
                                        CStr(DtDolcificazioni.Rows(i).Item("Lotto")).ToLower <> Trasformazione_Des.ToLower Then
                                mat_des += " " & DtDolcificazioni.Rows(i).Item("Lotto")
                            End If

                            If Not IsDBNull(DtDolcificazioni.Rows(i).Item("Data_Movimento")) Then
                                CType(rptSpumanti.Section2.ReportObjects("TextData" & (i_Arr_min).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                                    CDate(DtDolcificazioni.Rows(i).Item("Data_Movimento")).ToShortDateString
                            End If

                            CType(rptSpumanti.Section2.ReportObjects("TextRecipiente" & (i_Arr_min).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                               DtDolcificazioni.Rows(i).Item("Identificativo")

                            If Not IsDBNull(DtDolcificazioni.Rows(i).Item("Mat_Des")) Then
                                CType(rptSpumanti.Section2.ReportObjects("TextProdottiBaseRiga" & (i_Arr_min).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                                    mat_des
                            End If

                            CType(rptSpumanti.Section2.ReportObjects("TextLitri" & (i_Arr_min).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                                Format(Qta, "0.00")

                            i_Arr_min += 1

                        Else
                            Msg_Errori += "1. Cost Partita: superato num max di righe 'arricchimento speciale'" & vbCrLf
                        End If


                ' End Select

            Next

        End If



    End Sub


    '####################################################################################################################
    Private Sub _5_Travasi(ByRef objStampe As AgronicaCoreStampeDAL.RegistriCantina2, _
                                            ByRef DT_Note As DataTable, _
                                            ByRef DT_Perdite As DataTable, _
                                            ByRef Msg_Errori As String, _
                                            ByVal Id_Trasformazione As Integer)

        Dim DtTravasi As DataTable
        Dim filtro As String = ""
        Dim i As Integer

        Try

            filtro += " Linee_Preparazioni.Tipo_Default = " & CStr(enum_Omni_TipoDefault_Preparazioni.Travasi) & " "

            DtTravasi = objStampe.Travasi(Qs_Piva, _
                                                Id_Trasformazione, _
                                                filtro, _
                                                objParametri_Server)
        Catch ex As Exception
            Log_Errori += "lettura Travasi: " + vbCrLf + ex.Message + vbCrLf
        End Try


        Dim desc, Data, preparazione_des As String

        If Not IsNothing(DtTravasi) AndAlso DtTravasi.Rows.Count > 0 Then

            For i = 0 To DtTravasi.Rows.Count - 1

                Data = CDate(DtTravasi.Rows(i).Item("Data_Movimento")).ToShortDateString

                preparazione_des = DtTravasi.Rows(i).Item("preparazione_des")

                desc = Data & " " & preparazione_des

                Note_Inserisci_RigaDT(DT_Note, _
                                        DtTravasi.Rows(i).Item("id_agenda"), _
                                        Data, _
                                        desc)

                Perdite_Inserisci_RigaDT(DT_Perdite, _
                                  DtTravasi.Rows(i).Item("id_agenda"), _
                                  Data, _
                                  desc, _
                                  DtTravasi.Rows(i).Item("qta"))


            Next

        End If


    End Sub

    '####################################################################################################################
    Private Sub Visualizza_PerditeElaborazione(ByRef DTPerdite As DataTable, _
                                                ByRef Msg_Errori As String)

        Dim i As Integer

        Dim i_perdite_min = 1
        Dim i_perdite_max = 10
        Dim QtaPerdite As Double = 0

        DTPerdite.TableName = "perdite"

        Dim DvPerdite As New DataView
        DvPerdite.Table = DTPerdite
        DvPerdite.Sort = "Data ASC"


        If Not IsNothing(DvPerdite) AndAlso DvPerdite.Count > 0 Then

            For i = 0 To DvPerdite.Count - 1

                If i_perdite_min <= i_perdite_max Then

                    CType(rptSpumanti.Section2.ReportObjects("TextDataPerdite" & (i_perdite_min).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                      CDate(DvPerdite.Item(i).Item("Data")).ToShortDateString

                    CType(rptSpumanti.Section2.ReportObjects("TextLitriPerdite" & (i_perdite_min).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                          Format(DvPerdite.Item(i).Item("Qta"), "0.00") '& " " & DvPerdite.item(i).Item("udm_sim")

                    QtaPerdite += DvPerdite.Item(i).Item("Qta")

                    i_perdite_min += 1

                Else
                    Msg_Errori += "Perdite: superato num max di righe." & vbCrLf
                End If

            Next

            CType(rptSpumanti.Section2.ReportObjects("TextLitriPerditeTot"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                  Format(QtaPerdite, "0.00")

        End If

    End Sub


    '####################################################################################################################
    Private Sub Visualizza_Note(ByRef DT_Note As DataTable, _
                                ByRef Msg_Errori As String)

        Dim i As Integer

        Dim i_note_min = 1
        Dim i_note_max = 10

        DT_Note.TableName = "note"

        Dim DvNote As New DataView
        DvNote.Table = DT_Note
        DvNote.Sort = "Data ASC"


        If Not IsNothing(DvNote) AndAlso DvNote.Count > 0 Then

            For i = 0 To DvNote.Count - 1

                If i_note_min <= i_note_max Then

                    CType(rptSpumanti.Section2.ReportObjects("TxtNote" & (i_note_min).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                          CStr(DvNote.Item(i).Item("Descrizione"))

                    i_note_min += 1

                Else
                    Msg_Errori += "Note: superato num max di righe." & vbCrLf
                End If

            Next

        End If

    End Sub

    '########################################################################################
    Private Function Note_CaricaDT() As DataTable

        '----- Definizione delle variabili
        Dim Dt As New DataTable

        '----- Definisco la struttura del DataTable
        Dt.Columns.Add(New DataColumn("Id_Agenda", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Data", GetType(Date)))
        Dt.Columns.Add(New DataColumn("Descrizione", GetType(String)))

        Return Dt

    End Function

    '########################################################################################
    Public Sub Note_Inserisci_RigaDT(ByRef Dt As DataTable, _
                                    ByVal Id_Agenda As Integer, _
                                    ByVal Data As Date, _
                                    ByVal Descrizione As String _
                                    )

        Dim Dr As DataRow
        Dim flag_esiste As Boolean = False
        Dim i As Integer

        For i = 0 To Dt.Rows.Count - 1
            If Dt.Rows(i).Item("Id_Agenda") = Id_Agenda Then
                flag_esiste = True
            End If
        Next

        If flag_esiste = False Then

            Dr = Dt.NewRow

            Dr.Item("Id_Agenda") = Id_Agenda
            Dr.Item("Data") = Data
            Dr.Item("Descrizione") = Descrizione

            Dt.Rows.Add(Dr)

        End If

    End Sub

    '########################################################################################
    Private Function Perdite_CaricaDT() As DataTable

        '----- Definizione delle variabili
        Dim Dt As New DataTable

        '----- Definisco la struttura del DataTable
        Dt.Columns.Add(New DataColumn("Id_Agenda", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Data", GetType(Date)))
        Dt.Columns.Add(New DataColumn("Descrizione", GetType(String)))
        Dt.Columns.Add(New DataColumn("Qta", GetType(Double)))

        Return Dt

    End Function

    '########################################################################################
    Public Sub Perdite_Inserisci_RigaDT(ByRef Dt As DataTable, _
                                        ByVal Id_Agenda As Integer, _
                                        ByVal Data As Date, _
                                        ByVal Descrizione As String, _
                                        ByVal Qta As Double _
                                        )

        Dim i As Integer
        Dim flag_esiste As Boolean = False

        For i = 0 To Dt.Rows.Count - 1
            If Dt.Rows(i).Item("Id_Agenda") = Id_Agenda Then
                flag_esiste = True
            End If
        Next

        If flag_esiste = False Then

            Dim Dr As DataRow

            Dr = Dt.NewRow

            Dr.Item("Id_Agenda") = Id_Agenda
            Dr.Item("Data") = Data
            Dr.Item("Descrizione") = Descrizione
            Dr.Item("Qta") = Qta

            Dt.Rows.Add(Dr)

        End If

    End Sub



    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class
