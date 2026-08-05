Imports CrystalDecisions.Shared
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class RegistroViniFrizzanti
    Inherits System.Web.UI.Page

    '----------------------------
    '   DICHIARAZIONE REPORT
    '----------------------------
    Private rptFrizzanti As Rpt_RegistroViniFrizzanti

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


#Region " INIT FRIZZANTI "

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
        rptFrizzanti = New Rpt_RegistroViniFrizzanti
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

        'id_agenda dell'operazione di inizio frizz/spumant
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

        Dim Nome_Documento As String = "Registro_Vini_Frizzanti"
        Dim IdentificazioneDocumento As String = ""
        Dim cat_cod As enum_CategorieDocumenti

        cat_cod = enum_CategorieDocumenti.Cantina_RegistroFrizzanti



        If Not Me.IsPostBack Then


            Dim Id_Trasformazione As Integer = 0
            Dim Flag_FrizzBottiglie As Boolean

            Try
                CostituzionePartita(Id_Trasformazione, Flag_FrizzBottiglie)

            Catch ex As Exception
                Log_Errori += "- CostituzionePartita: " + vbCrLf + ex.Message + vbCrLf
            End Try

            If Id_Trasformazione = 0 Then
                Log_Errori += "Id_Trasformazione=0"
            Else

                Dim indicePerdite As Integer = 0
                Dim QtaPerdite As Double = 0

                Try
                    PerditeElaborazione_FineFrizzantatura(Id_Trasformazione, indicePerdite, QtaPerdite)

                Catch ex As Exception
                    Log_Errori += "- PerditeElaborazione_FineFrizzantatura: " + vbCrLf + ex.Message + vbCrLf
                End Try

                Try
                    Imbottigliamento(Id_Trasformazione, indicePerdite, QtaPerdite, Flag_FrizzBottiglie)

                Catch ex As Exception
                    Log_Errori += "- Imbottigliamento: " + vbCrLf + ex.Message + vbCrLf
                End Try

                Try
                    Acidificazioni(Id_Trasformazione)

                Catch ex As Exception
                    Log_Errori += "- Acidificazioni: " + vbCrLf + ex.Message + vbCrLf
                End Try

                Try
                    Dolcificazioni(Id_Trasformazione)

                Catch ex As Exception
                    Log_Errori += "- Dolcificazioni: " + vbCrLf + ex.Message + vbCrLf
                End Try

            End If 'Id_Trasformazione

            Try

                CType(rptFrizzanti.Section2.ReportObjects("TextPartita"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Qs_Partita

                If QS_StampaIntestazione = "1" Then

                    CType(rptFrizzanti.Section1.ReportObjects("TextRagSoc"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Qs_RagSoc + " " + Qs_Piva

                    rptFrizzanti.Section6.SectionFormat.EnableSuppress = True
                Else
                    rptFrizzanti.Section1.SectionFormat.EnableSuppress = True
                End If

            Catch ex As Exception
                Log_Errori += "- stampa intestazione: " + vbCrLf + ex.Message + vbCrLf
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
                objGestFile.SalvaReportPdf(rptFrizzanti, _
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
                                                 "RegistroViniFrizzanti.aspx", _
                                                 Log_Errori)


            End If
            '-----------------------------------------
        End If

        '==================================================================

        Try
            Session("Report") = rptFrizzanti
            Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server))
        Catch ex As Exception
            Log_Errori += "- Export: " + vbCrLf + ex.Message + vbCrLf
        End Try

    End Sub

    '####################################################################################################################
    Private Sub CostituzionePartita(ByRef Id_Trasformazione As Integer, _
                                    ByRef Flag_FrizzBottiglie As Boolean)

        'ByRef Cod_Contatto_Terzi As String, _
        'ByRef Rag_Soc_Terzi As String)

        Dim objAgenda As New AgronicaCoreContabDAL.Agenda_R
        Dim objStampe As New AgronicaCoreStampeDAL.RegistriCantina2
        Dim Dt As DataTable
        Dim i As Integer
        Dim objGenAnag As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Log_R
        Dim DtAnag As DataTable
        'Dim Id_Trasformazione As Integer = 0
        Dim Elem_Cod_Lav As Integer = 0
        Dim Mat_Cod_Lav As Integer = 0
        Dim Elem_Cod_Trasf As Integer = 0
        Dim Mat_Cod_Trasf As Integer = 0

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

        '' salvo la variabile che mi servirà per caricare le altre operazioni
        'viewstate("Id_Trasformazione") = Id_Trasformazione

        Try

            ' recupero il mat_cod dei Cali di Trasformazione [OGenerazioni_Anagrafe_Log]
            DtAnag = objGenAnag.Leggi_MateriaPrima(Qs_Piva, _
                                                    -1, -1, _
                                                    enum_Omni_Modulo_Generazione.Cantine, _
                                                    enum_Omni_Tipo_Generazione.CaliLavorazione, _
                                                    enum_Omni_Codice_Generazione.Tipo7_CaloTrasformazione, _
                                                    AGRODATAINIZIO, AGRODATAFINE, _
                                                    "", "", objParametri_Server)

            If Not IsNothing(DtAnag) AndAlso DtAnag.Rows.Count > 0 Then
                For i = 0 To DtAnag.Rows.Count - 1
                    Elem_Cod_Trasf = DtAnag.Rows(0).Item("Elem_Cod")
                    Mat_Cod_Trasf = DtAnag.Rows(0).Item("Mat_Cod")
                Next
            End If

            DtAnag.Dispose()
            DtAnag = Nothing

        Catch ex As Exception
            Log_Errori += "recupero il mat_cod dei Cali di Trasformazione: " + vbCrLf + ex.Message + vbCrLf
        End Try

        Try

            ' recupero il mat_cod delle Perdite di Lavorazione [OGenerazioni_Anagrafe_Log]
            DtAnag = objGenAnag.Leggi_MateriaPrima(Qs_Piva, _
                                                    -1, -1, _
                                                    enum_Omni_Modulo_Generazione.Cantine, _
                                                    enum_Omni_Tipo_Generazione.CaliLavorazione, _
                                                    enum_Omni_Codice_Generazione.Tipo7_PerditaLavorazione, _
                                                    AGRODATAINIZIO, AGRODATAFINE, _
                                                    "", "", objParametri_Server)

            If Not IsNothing(DtAnag) AndAlso DtAnag.Rows.Count > 0 Then
                For i = 0 To DtAnag.Rows.Count - 1
                    Elem_Cod_Lav = DtAnag.Rows(0).Item("Elem_Cod")
                    Mat_Cod_Lav = DtAnag.Rows(0).Item("Mat_Cod")
                Next
            End If

            objGenAnag = Nothing
            DtAnag.Dispose()
            DtAnag = Nothing

        Catch ex As Exception
            Log_Errori += "recupero il mat_cod delle Perdite di Lavorazione: " + vbCrLf + ex.Message + vbCrLf
        End Try

        Try

            ' lettura inizio frizzantatura
            Dt = objStampe.RegistroFrizzanti_CostituzionePartita(Qs_Piva, _
                                                                Qs_Id_Agenda, _
                                                                objParametri_Server)

        Catch ex As Exception
            Log_Errori += "lettura frizzantatura: " + vbCrLf + ex.Message + vbCrLf
        End Try

        Dim Dr_Mov As DataRow()

        ' seleziono gli scarichi
        Dr_Mov = Dt.Select("CAU_MOV='7350'")

        Dim DataPrec As DateTime
        Dim indiceBase As Integer = 0
        Dim indiceSpuma As Integer = 3
        Dim indice As Integer
        Dim QtaScarico As Double = 0
        Dim QtaCarico As Double = 0

        'COMPILAZIONE DEL RIQUADRO B
        'E DEL RIQUADRO 1. COSTITUZIONE DELLA PARTITA
        If Not IsNothing(Dr_Mov) AndAlso Dr_Mov.Length > 0 Then

            For i = 0 To Math.Min(Dr_Mov.Length, 5) - 1

                'modifica del 14/03/2013: non va bene, perchè nel nome della linea c'ès critto frizzante
                'e il vino ancora non è frizzante (lascio dicitura di default come scritto nel registro cartaceo)
                'If i = 0 Then
                '    'modifica del 04 marzo 2013: 
                '    'dalla nicolina veniva stampato “TOTALE VINO IN FRIZZANTATURA PER EMILIA TREBBIANO ROMAGNOLO IGP”
                '    CType(rptFrizzanti.Section2.ReportObjects("TextVinoFrizzante"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                '        "TOTALE " & CStr(Dr_Mov(i).Item("Linea_Des")).ToUpper
                'End If

                If Not IsDBNull(Dr_Mov(i).Item("Identificativo")) Then
                    indiceBase += 1
                    indice = indiceBase
                Else
                    indiceSpuma += 1
                    indice = indiceSpuma
                End If

                If Not IsDBNull(Dr_Mov(i).Item("Identificativo")) Then

                    'modifica del 4 marzo 2013: il numero recipiente di provenienza non deve coincidere con quello di elaborazione
                    'ora sono nel filtro scarico
                    'quindi sto leggendo al vasca di provenienza
                    CType(rptFrizzanti.Section2.ReportObjects("TextRecipiente" & (indice).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                        Dr_Mov(i).Item("Identificativo")
                    '' sezione B
                    'CType(rptFrizzanti.Section2.ReportObjects("TextRecipienteB" & (indice).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                    '     "AUTOCLAVE N. " & Dr_Mov(i).Item("Identificativo")
                End If

                '1. COSTITUZIONE DELLA PARTITA
                If Dr_Mov(i).Item("Data_Movimento") <> DataPrec Then

                    If Not IsDBNull(Dr_Mov(i).Item("Data_Movimento")) Then
                        CType(rptFrizzanti.Section2.ReportObjects("TextData" & (indice).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                            CDate(Dr_Mov(i).Item("Data_Movimento")).ToShortDateString
                    End If

                    DataPrec = Dr_Mov(i).Item("Data_Movimento")

                End If

                If Not IsDBNull(Dr_Mov(i).Item("Mat_Des")) Then
                    CType(rptFrizzanti.Section2.ReportObjects("TextProdottiBaseRiga" & (indice).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                        Dr_Mov(i).Item("Mat_Des")
                End If

                If Not IsDBNull(Dr_Mov(i).Item("Qta")) Then
                    CType(rptFrizzanti.Section2.ReportObjects("TextLitri" & (indice).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                         Format(Dr_Mov(i).Item("Qta"), "0.00")
                End If

                QtaScarico += Dr_Mov(i).Item("Qta")

            Next

        End If

        ' seleziono i carichi
        Dr_Mov = Dt.Select("CAU_MOV='7300'")
        Dim QtaPerdite As Double = 0
        Dim i_Autoclave As Integer = 1
        Dim linea_des As String = ""
        Dim Cod_Contatto_Terzi As String = ""
        Dim Rag_soc_Terzi As String = ""

        If Not IsNothing(Dr_Mov) AndAlso Dr_Mov.Length > 0 Then

            For i = 0 To Dr_Mov.Length - 1

                ' se non è un Calo di Trasformazione o una perdita di elaborazione
                If (Dr_Mov(i).Item("Elem_Cod") <> Elem_Cod_Trasf And Dr_Mov(i).Item("Mat_Cod") <> Mat_Cod_Trasf) And _
                   (Dr_Mov(i).Item("Elem_Cod") <> Elem_Cod_Lav And Dr_Mov(i).Item("Mat_Cod") <> Mat_Cod_Lav) Then

                    ' sezione B
                    If CStr(Dr_Mov(i).Item("Identificativo")).ToUpper <> "MAGAZZINO" Then

                        Flag_FrizzBottiglie = False

                        CType(rptFrizzanti.Section2.ReportObjects("TextRecipienteB" & (i_Autoclave).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                             "AUTOCLAVE N. " & CStr(Dr_Mov(i).Item("Identificativo"))

                    Else
                        Flag_FrizzBottiglie = True
                    End If

                    i_Autoclave += 1

                    'modifica del 14/03/2013: lascio dicitura di default come scritto nel registro cartaceo
                    'modifica del 04 marzo 2013: 
                    'dalla nicolina veniva stampato “TOTALE VINO IN FRIZZANTATURA PER EMILIA TREBBIANO ROMAGNOLO IGP”
                    'invece lei non vuole romangolo
                    'If Not IsDBNull(Dr_Mov(i).Item("Mat_Des")) Then
                    '    CType(rptFrizzanti.Section2.ReportObjects("TextVinoFrizzante"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                    '        "TOTALE " & CStr(Dr_Mov(i).Item("Mat_Des")).ToUpper
                    'End If


                    'correzione bug del 19/02/2015
                    'If Not IsDBNull(Dr_Mov(i).Item("Qta")) Then
                    '    CType(rptFrizzanti.Section2.ReportObjects("TextLitriFrizzante"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                    '        Format(Dr_Mov(i).Item("Qta"), "0.00")
                    'End If
                    QtaCarico += Dr_Mov(i).Item("Qta")

                    '02/03/2016
                    'Se Materie_prime.qta_extra >0,
                    ' allora in qta viene salvato il numero di botteigli e in qta_extra la capacità in l
                    If Dr_Mov(i).Item("Qta_Extra") > 0 Then
                        QtaCarico = QtaCarico * Dr_Mov(i).Item("Qta_Extra")
                    End If
                    'modifica del 19/02/2015:
                    ''CType(rptFrizzanti.Section2.ReportObjects("TextDesignazione"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                    ''   CStr(Dr_Mov(i).Item("Mat_Des")).ToUpper
                    'CType(rptFrizzanti.Section2.ReportObjects("TextDesignazione"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                    '        CStr(Dr_Mov(i).Item("Linea_Des")).ToUpper

                    linea_des = Dr_Mov(i).Item("Linea_Des")
                    Rag_soc_Terzi = Dr_Mov(i).Item("ContoTerzi")
                    Cod_Contatto_Terzi = Dr_Mov(i).Item("Cod_Contatto_Terzi")

                Else
                    'è un Calo di Trasformazione o una perdita di elaborazione
                    QtaPerdite += Dr_Mov(i).Item("Qta")
                End If

            Next

        End If

        CType(rptFrizzanti.Section2.ReportObjects("TextLitriFrizzante"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                     Format(QtaCarico, "0.00")

        'QtaPerdite = QtaCarico - QtaScarico
        CType(rptFrizzanti.Section2.ReportObjects("TextLitriPerdite"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                               Format(QtaPerdite, "0.00")

        CType(rptFrizzanti.Section2.ReportObjects("TextDesignazione"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                      CStr(linea_des).ToUpper

        If Rag_soc_Terzi <> "" Then
            CType(rptFrizzanti.Section2.ReportObjects("TxtContoTerzi"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                               Rag_soc_Terzi & " " & Cod_Contatto_Terzi
        End If




    End Sub

    '####################################################################################################################
    Private Sub PerditeElaborazione_FineFrizzantatura(ByVal Id_Trasformazione As Integer, _
                                                        ByRef indicePerdite As Integer, _
                                                        ByRef QtaPerdite As Double)

        Dim objStampe As New AgronicaCoreStampeDAL.RegistriCantina2
        Dim DtPerdite As DataTable
        Dim i As Integer
        Dim ElencoAgenda As String = String.Empty

        'MODIFICA DEL 29/11/13: gestita univocamente nel core, la lettura degli id_agenda e composizione stringa
        Try

            Dim filtro_friz As String = ""
            ' stbQ.Append(" AND Linee_Preparazioni.Codice_Generazione = " + Agro_SQL_SaveNum(enum_Omni_Preparazione_Cod.FineFrizzantaturaAutoclave) & " " + vbCrLf)

            filtro_friz += " Linee_Preparazioni.Modulo_Generazione = " & Agro_SQL_SaveNum(enum_Omni_Modulo_Generazione.Cantine)

            filtro_friz += " AND Linee_Preparazioni.Codice_Generazione IN (" & _
                            CStr(enum_Omni_Preparazione_Cod.FineFrizzantaturaAutoclave_RegCommercializzazione) & "," & _
                            CStr(enum_Omni_Preparazione_Cod.FineFrizzantaturaAutoclave_RegVinificazione) & "," & _
                            CStr(enum_Omni_Preparazione_Cod.FineFrizzantaturaAutoclaveMPFoVNAF_xVinoAttoADivenire_RegCommercializzazione) & "," & _
                            CStr(enum_Omni_Preparazione_Cod.FineFrizzantaturaAutoclaveMPFoVNAF_xVinoAttoADivenire_RegVinificazione) & "," & _
                            CStr(enum_Omni_Preparazione_Cod.FineFrizzantaturaAutoclaveMPFoVNAF_RegVinificazione) & "," & _
                            CStr(enum_Omni_Preparazione_Cod.FineFrizzantaturaAutoclaveVinoAttoADivenire_RegCommercializzazione) & "," & _
                            CStr(enum_Omni_Preparazione_Cod.FineFrizzantaturaAutoclaveVinoAttoADivenire_RegVinificazione) & "," & _
                            CStr(enum_Omni_Preparazione_Cod.FineFrizzantaturaBottiglia) & "," & _
                            CStr(enum_Omni_Preparazione_Cod.FineFrizzantaturaBottigliaViniQualita) & _
                            " ) "

            ElencoAgenda = objStampe.RegistroFrizzanti_ElencoIdAgenda(Qs_Piva, _
                                                                        Id_Trasformazione, _
                                                                        0, _
                                                                        filtro_friz, "", _
                                                                        objParametri_Server)



        Catch ex As Exception
            Log_Errori += "PerditeElaborazione_FineFrizzantatura, recupero id_agenda: " + vbCrLf + ex.Message + vbCrLf
        End Try


        indicePerdite = 0
        QtaPerdite = 0

        If ElencoAgenda = "" Then


        Else

            Try

                DtPerdite = objStampe.RegistroFrizzanti_LeggiPerdite_FineFrizzantatura(Qs_Piva, _
                                                                                        ElencoAgenda, _
                                                                                        "", "", _
                                                                                        objParametri_Server)

                If Not IsNothing(DtPerdite) AndAlso DtPerdite.Rows.Count > 0 Then
                    For i = 0 To DtPerdite.Rows.Count - 1

                        CType(rptFrizzanti.Section2.ReportObjects("TextDataPerdite" & (indicePerdite + 1).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                              CDate(DtPerdite.Rows(i).Item("Data_Movimento")).ToShortDateString

                        CType(rptFrizzanti.Section2.ReportObjects("TextLitriPerdite" & (indicePerdite + 1).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                              Format(DtPerdite.Rows(i).Item("Qta"), "0.00") '& " " & DtPerdite.Rows(i).Item("udm_sim")

                        QtaPerdite += DtPerdite.Rows(i).Item("Qta")

                        indicePerdite += 1

                    Next
                End If

            Catch ex As Exception
                Log_Errori += "Lettura perdite nella fase di fine frizzantatura: " + vbCrLf + ex.Message + vbCrLf
            End Try

        End If


    End Sub


    '####################################################################################################################
    Private Sub Imbottigliamento(ByVal Id_Trasformazione As Integer, _
                                ByRef indicePerdite As Integer, _
                                ByRef QtaPerdite As Double, _
                                ByVal Flag_FrizzBottiglie As Boolean)

        Dim objStampe As New AgronicaCoreStampeDAL.RegistriCantina2
        'Dim DtAgenda As DataTable
        Dim DtImbottigliamento As DataTable
        Dim ElencoAgenda As String = String.Empty
        Dim i As Integer

        'MODIFICA DEL 29/11/13: gestita univocamente nel core, la lettura degli id_agenda e composizione stringa
        Try

            Dim filtro_friz As String = ""

            filtro_friz += " Linee_Preparazioni.Modulo_Generazione = " & Agro_SQL_SaveNum(enum_Omni_Modulo_Generazione.Cantine)
            filtro_friz += " AND Linee_Preparazioni.Codice_Generazione IN (" & _
                        CStr(enum_Omni_Preparazione_Cod.Imbottigliamento) & "," & _
                        CStr(enum_Omni_Preparazione_Cod.ImbottigliamentoVinoAttoADivenire) & "," & _
                        CStr(enum_Omni_Preparazione_Cod.ImbottigliamentoInLinee) & "," & _
                        CStr(enum_Omni_Preparazione_Cod.ImbottigliamentoDaLinee) & "," & _
                        CStr(enum_Omni_Preparazione_Cod.Frizzantatura_Imbottigliamento_RegVinificazione) & "," & _
                        CStr(enum_Omni_Preparazione_Cod.Frizzantatura_Imbottigliamento_RegCommercializzazione) & "," & _
                        CStr(enum_Omni_Preparazione_Cod.FineFrizzantaturaBottiglia) & "," & _
                        CStr(enum_Omni_Preparazione_Cod.FineFrizzantaturaBottigliaViniQualita) & "," & _
                        CStr(enum_Omni_Preparazione_Cod.ImbottigliamentoInizioFrizzantaturaBottiglia_RegVinificazione) & "," & _
                        CStr(enum_Omni_Preparazione_Cod.ImbottigliamentoInizioFrizzantaturaBottiglia_RegCommercializzazione) & "," & _
                        CStr(enum_Omni_Preparazione_Cod.ImbottigliamentoInizioFrizzantaturaBottigliaViniQualita_RegVinificazione) & "," & _
                        CStr(enum_Omni_Preparazione_Cod.ImbottigliamentoInizioFrizzantaturaBottigliaViniQualita_RegCommercializzazione) & _
                        " ) "

            'queste operazioni non vanno bene perchè fa vedere la bottiglia in frizzantatura, non quella finita
            'CStr(enum_Omni_Preparazione_Cod.ImbottigliamentoInizioFrizzantaturaBottiglia_RegVinificazione) & "," & _
            'CStr(enum_Omni_Preparazione_Cod.ImbottigliamentoInizioFrizzantaturaBottiglia_RegCommercializzazione) & "," & _
            'CStr(enum_Omni_Preparazione_Cod.ImbottigliamentoInizioFrizzantaturaBottigliaViniQualita_RegVinificazione) & "," & _
            'CStr(enum_Omni_Preparazione_Cod.ImbottigliamentoInizioFrizzantaturaBottigliaViniQualita_RegCommercializzazione) & _



            ElencoAgenda = objStampe.RegistroFrizzanti_ElencoIdAgenda(Qs_Piva, _
                                                                        Id_Trasformazione, _
                                                                        0, _
                                                                        filtro_friz, "", _
                                                                        objParametri_Server)



        Catch ex As Exception
            Log_Errori += "Imbottigliamento, recupero id_agenda: " + vbCrLf + ex.Message + vbCrLf
        End Try


        'Try

        '    Dim filtro_friz As String = ""
        '    'filtro_friz = " Linee_Preparazioni.Tipo_Default IN (" & _
        '    '            CStr(enum_Omni_Codice_Generazione.Tipo10_Imbottigliamento) & _
        '    '            "," & _
        '    '            CStr(enum_Omni_Codice_Generazione.Tipo10_Frizzantatura_Imbottigliamento) & _
        '    '            " ) "

        '    filtro_friz += " Linee_Preparazioni.Modulo_Generazione = " & SQL_SaveNum(enum_Omni_Modulo_Generazione.Cantine)
        '    filtro_friz += " AND Linee_Preparazioni.Codice_Generazione IN (" & _
        '                CStr(enum_Omni_Preparazione_Cod.Imbottigliamento) & "," & _
        '                CStr(enum_Omni_Preparazione_Cod.ImbottigliamentoInLinee) & "," & _
        '                CStr(enum_Omni_Preparazione_Cod.Frizzantatura_Imbottigliamento_RegVinificazione) & "," & _
        '                CStr(enum_Omni_Preparazione_Cod.Frizzantatura_Imbottigliamento_RegCommercializzazione) & "," & _
        '                CStr(enum_Omni_Preparazione_Cod.ImbottigliamentoInizioFrizzantaturaBottiglia_RegVinificazione) & "," & _
        '                CStr(enum_Omni_Preparazione_Cod.ImbottigliamentoInizioFrizzantaturaBottiglia_RegCommercializzazione) & "," & _
        '                CStr(enum_Omni_Preparazione_Cod.ImbottigliamentoInizioFrizzantaturaBottigliaViniQualita_RegVinificazione) & "," & _
        '                CStr(enum_Omni_Preparazione_Cod.ImbottigliamentoInizioFrizzantaturaBottigliaViniQualita_RegCommercializzazione) & _
        '                " ) "

        '    ' recupero gli imbottigliamenti della partita grazie all'id_trasformazione che ho salvato nella Costituzione della Partita
        '    DtAgenda = objStampe.RegistroFrizzanti_AgendaFromTrasformazione(Qs_Piva, _
        '                                                                    Id_Trasformazione, _
        '                                                                    0, _
        '                                                                    filtro_friz, _
        '                                                                    "", _
        '                                                                    objParametri_Server)

        'Catch ex As Exception
        '    Log_Errori += "recupero gli imbottigliamenti della partita: " + vbCrLf + ex.Message + vbCrLf
        'End Try

        '' preparo la stringa che userò nella query successiva nella clausula del where
        'If Not IsNothing(DtAgenda) AndAlso DtAgenda.Rows.Count > 0 Then
        '    For i = 0 To DtAgenda.Rows.Count - 1
        '        ElencoAgenda &= IIf(ElencoAgenda <> String.Empty, ", ", "")
        '        ElencoAgenda &= DtAgenda.Rows(i).Item("Id_Agenda")
        '    Next
        'End If
        'DtAgenda.Dispose()
        'DtAgenda = Nothing

        If ElencoAgenda = "" Then
            'non è un errore, possono non esserci, non stampo nel log
            'Log_Errori += "Non sono stati trovati imbottigliamenti per quella partita." + vbCrLf + vbCrLf
        Else

            'Dim objGenAnag As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Log_R
            'Dim DtAnag As DataTable

            'Try
            '    ' recupero il mat_cod dei cali di imbottigliamento da [OGenerazioni_Anagrafe_Log]
            '    DtAnag = objGenAnag.Leggi_MateriaPrima(Qs_Piva, _
            '                                            -1, -1, _
            '                                            enum_Omni_Modulo_Generazione.Cantine, _
            '                                            enum_Omni_Tipo_Generazione.CaliLavorazione, _
            '                                            enum_Omni_Codice_Generazione.Tipo7_CaloImbottigliamento, _
            '                                            AGRODATAINIZIO, AGRODATAFINE, _
            '                                            "", "", _
            '                                            objParametri_Server)

            'Catch ex As Exception
            '    Log_Errori += "recupero il mat_cod dei cali di imbottigliamento: " + vbCrLf + ex.Message + vbCrLf
            'End Try

            'Dim Elem_Cod As Integer = 0
            'Dim Mat_Cod As Integer = 0

            'If Not IsNothing(DtAnag) AndAlso DtAnag.Rows.Count > 0 Then
            '    For i = 0 To DtAnag.Rows.Count - 1
            '        Elem_Cod = DtAnag.Rows(0).Item("Elem_Cod")
            '        Mat_Cod = DtAnag.Rows(0).Item("Mat_Cod")
            '    Next
            'End If

            'objGenAnag = Nothing
            'DtAnag.Dispose()
            'DtAnag = Nothing

            Dim QtaOrigine As Double = 0
            Dim QtaConfezionato As Double = 0

            Try
                ' Elenco degli imbottigliamenti
                DtImbottigliamento = objStampe.RegistroFrizzanti_Imbottigliamento(Qs_Piva, _
                                                                                  ElencoAgenda, _
                                                                                  objParametri_Server)

            Catch ex As Exception
                Log_Errori += "Elenco degli imbottigliamenti: " + vbCrLf + ex.Message + vbCrLf
            End Try



            Dim indiceImbott As Integer = 0

            If Not IsNothing(DtImbottigliamento) AndAlso DtImbottigliamento.Rows.Count > 0 Then

                Dim lotto As String = ""
                Dim i_Bottiglie As Integer = 1
                Dim num_bottiglie, capacita_bott As String

                'in data 31/01/2013 eliminata una riga per aumentare l'altezza delle righe
                'For i = 0 To Math.Min(DtImbottigliamento.Rows.Count, 10) - 1
                For i = 0 To Math.Min(DtImbottigliamento.Rows.Count, 8) - 1

                    '' se non è un calo di imbottigliamento
                    'If DtImbottigliamento.Rows(i).Item("Elem_Cod") <> Elem_Cod And DtImbottigliamento.Rows(i).Item("Mat_Cod") <> Mat_Cod Then
                    Select Case DtImbottigliamento.Rows(i).Item("Tipo_Generazione")

                        Case enum_Omni_Tipo_Generazione.CaliLavorazione
                            ' Perdite di imbottigliamento
                            CType(rptFrizzanti.Section2.ReportObjects("TextDataPerdite" & (indicePerdite + 1).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                                  CDate(DtImbottigliamento.Rows(i).Item("Data_Movimento")).ToShortDateString

                            CType(rptFrizzanti.Section2.ReportObjects("TextLitriPerdite" & (indicePerdite + 1).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                                  Format(DtImbottigliamento.Rows(i).Item("Qta"), "0.00")

                            QtaPerdite += DtImbottigliamento.Rows(i).Item("Qta")


                            indicePerdite += 1


                            ' se non è un calo di imbottigliamento
                            'If DtImbottigliamento.Rows(i).Item("Elem_Cod") <> Elem_Cod And DtImbottigliamento.Rows(i).Item("Mat_Cod") <> Mat_Cod Then

                        Case enum_Omni_Codice_Generazione.Tipo5_BottInFrizz020, enum_Omni_Codice_Generazione.Tipo5_BottInFrizz0375, _
                  enum_Omni_Codice_Generazione.Tipo5_BottInFrizz050, enum_Omni_Codice_Generazione.Tipo5_BottInFrizz075, _
                  enum_Omni_Codice_Generazione.Tipo5_BottInFrizz150, enum_Omni_Codice_Generazione.Tipo5_BottInFrizz300, _
                  enum_Omni_Codice_Generazione.Tipo5_BottInFrizzAtto020, enum_Omni_Codice_Generazione.Tipo5_BottInFrizzAtto0375, _
                  enum_Omni_Codice_Generazione.Tipo5_BottInFrizzAtto050, enum_Omni_Codice_Generazione.Tipo5_BottInFrizzAtto075, _
                  enum_Omni_Codice_Generazione.Tipo5_BottInFrizzAtto150, enum_Omni_Codice_Generazione.Tipo5_BottInFrizzAtto300

                            Dim debug As Boolean = True
                            'nel caso di Imbottigliamento e InizioFrizzantaturaBottiglia
                            'devo leggere solo l'imbottigliamento, non la bottiglia in frizzantatura
                            '(la bottiglia la vedrò con la fine frizzantatura in bottiglia)

                        Case Else

                            num_bottiglie = CStr(DtImbottigliamento.Rows(i).Item("Qta"))

                            capacita_bott = CStr(DtImbottigliamento.Rows(i).Item("QTA_EXTRA")) & " " & DtImbottigliamento.Rows(i).Item("udm_sim_extra")

                            CType(rptFrizzanti.Section2.ReportObjects("TextDataImb" & (indiceImbott + 1).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                                CDate(DtImbottigliamento.Rows(i).Item("Data_Movimento")).ToShortDateString

                            CType(rptFrizzanti.Section2.ReportObjects("TextNum" & (indiceImbott + 1).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                                num_bottiglie

                            CType(rptFrizzanti.Section2.ReportObjects("TextMl" & (indiceImbott + 1).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                                capacita_bott

                            If CStr(DtImbottigliamento.Rows(i).Item("Lotto")).ToLower <> "indefinito" And DtImbottigliamento.Rows(i).Item("Lotto") <> "" Then
                                lotto = " Lotto: " & DtImbottigliamento.Rows(i).Item("Lotto")
                            Else
                                lotto = ""
                            End If

                            CType(rptFrizzanti.Section2.ReportObjects("TextMatDesImb" & (indiceImbott + 1).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                                DtImbottigliamento.Rows(i).Item("Mat_Des") & lotto

                            'CASE WHEN Materie_Prime.Peso_Set>0 THEN Movimenti_dettagli.QTA_EXTRA*Movimenti_dettagli.QTA 
                            'WHEN Materie_Prime.Peso_Set=0 THEN Materie_Prime.QTA_EXTRA*Movimenti_dettagli.QTA 
                            'ELSE 0 END AS Qta_Confezionato, 
                            QtaConfezionato += DtImbottigliamento.Rows(i).Item("Qta_Confezionato")

                            'nel caso di frizzantatura in bottiglia
                            'imposto nel riquadro B le bottiglie
                            If Flag_FrizzBottiglie = True Then
                                CType(rptFrizzanti.Section2.ReportObjects("TextRecipienteB" & (i_Bottiglie).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                                "Bottiglie n. " & num_bottiglie & " x " & capacita_bott
                                i_Bottiglie += 1
                            End If

                            indiceImbott += 1

                    End Select

                    'Else
                    '        ' Perdite di imbottigliamento
                    '        CType(rptFrizzanti.Section2.ReportObjects("TextDataPerdite" & (indicePerdite + 1).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                    '              CDate(DtImbottigliamento.Rows(i).Item("Data_Movimento")).ToShortDateString

                    '        CType(rptFrizzanti.Section2.ReportObjects("TextLitriPerdite" & (indicePerdite + 1).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                    '              Format(DtImbottigliamento.Rows(i).Item("Qta"), "0.00")

                    '        QtaPerdite += DtImbottigliamento.Rows(i).Item("Qta")

                    '        indicePerdite += 1

                    'End If

                    '' quantità di vino scaricata utilizzata per calcolare le perdite
                    'QtaOrigine += DtImbottigliamento.Rows(i).Item("Qta_Origine")

                    'QtaConfezionato += DtImbottigliamento.Rows(i).Item("Qta_Confezionato")

                    'QtaPerdite = DtImbottigliamento.Rows(i).Item("Qta_Origine") - DtImbottigliamento.Rows(i).Item("Qta_Confezionato")

                    'If QtaPerdite <> 0 Then

                    '    ' Perdite di imbottigliamento
                    '    CType(rptFrizzanti.Section2.ReportObjects("TextDataPerdite" & (indicePerdite + 1).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                    '        CDate(DtImbottigliamento.Rows(i).Item("Data_Movimento")).ToShortDateString

                    '    CType(rptFrizzanti.Section2.ReportObjects("TextLitriPerdite" & (indicePerdite + 1).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                    '          Format(QtaPerdite, "0.00")

                    '    indicePerdite += 1

                    'End If

                Next
            End If

            CType(rptFrizzanti.Section2.ReportObjects("TextLitriImbTot"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                  Format(QtaConfezionato, "0.00")

            CType(rptFrizzanti.Section2.ReportObjects("TextLitriPerditeTot"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                  Format(QtaPerdite, "0.00")

            CType(rptFrizzanti.Section2.ReportObjects("TextTOTfinale"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                Format(QtaConfezionato, "0.00")

            DtImbottigliamento.Dispose()
            DtImbottigliamento = Nothing

        End If 'elenco agenda

    End Sub


    '####################################################################################################################
    ' acidificazioni e disacidificazioni TipoDefault 7
    Private Sub Acidificazioni(ByVal Id_Trasformazione As Integer)

        Dim objStampe As New AgronicaCoreStampeDAL.RegistriCantina2
        'Dim DtAgenda As DataTable
        Dim DtAcidificazioni As DataTable
        Dim ElencoAgenda As String = String.Empty
        Dim i As Integer


        'MODIFICA DEL 29/11/13: gestita univocamente nel core, la lettura degli id_agenda e composizione stringa
        Try

            Dim filtro_friz As String = ""

            filtro_friz += " Linee_Preparazioni.Modulo_Generazione = " & Agro_SQL_SaveNum(enum_Omni_Modulo_Generazione.Cantine)
            filtro_friz += " AND Linee_Preparazioni.Codice_Generazione IN (" & _
                            CStr(enum_Omni_Preparazione_Cod.AggiuntaAcidificanti) & _
                            " ) "

            ElencoAgenda = objStampe.RegistroFrizzanti_ElencoIdAgenda(Qs_Piva, _
                                                                        Id_Trasformazione, _
                                                                        0, _
                                                                        filtro_friz, "", _
                                                                        objParametri_Server)



        Catch ex As Exception
            Log_Errori += "Acidificazioni, recupero id_agenda: " + vbCrLf + ex.Message + vbCrLf
        End Try

        'Try

        '    '& "," & _
        '    'CStr(enum_Omni_Preparazione_Cod.) & "," & _
        '    'CStr(enum_Omni_Preparazione_Cod.) & _

        '    'DtAgenda = objStampe.RegistroFrizzanti_AgendaFromTrasformazione(Qs_Piva, _
        '    '                                                                Id_Trasformazione, _
        '    '                                                                enum_Omni_Codice_Generazione.Tipo10_Aggiunta_Prodotti_Enologici, _
        '    '                                                                "", "", _
        '    '                                                                objParametri_Server)

        '    DtAgenda = objStampe.RegistroFrizzanti_AgendaFromTrasformazione(Qs_Piva, _
        '                                                                    Id_Trasformazione, _
        '                                                                    0, _
        '                                                                    filtro_friz, "", _
        '                                                                    objParametri_Server)


        'Catch ex As Exception
        '    Log_Errori += "RegistroFrizzanti_AgendaFromTrasformazione: " + vbCrLf + ex.Message + vbCrLf
        'End Try

        '' preparo la stringa che userò nella query successiva nella clausula del where
        'If Not IsNothing(DtAgenda) AndAlso DtAgenda.Rows.Count > 0 Then
        '    For i = 0 To DtAgenda.Rows.Count - 1
        '        ElencoAgenda &= IIf(ElencoAgenda <> String.Empty, ", ", "")
        '        ElencoAgenda &= DtAgenda.Rows(i).Item("Id_Agenda")
        '    Next
        'End If
        'DtAgenda.Dispose()
        'DtAgenda = Nothing


        If ElencoAgenda = "" Then
            'non è un errore, possono non esserci, non stampo nel log
            'Log_Errori += "Non sono state trovate acidificazioni per quella partita." + vbCrLf + vbCrLf
        Else

            Try
                DtAcidificazioni = objStampe.RegistroFrizzanti_Acidificazioni(Qs_Piva, _
                                                                                ElencoAgenda, _
                                                                                objParametri_Server)

            Catch ex As Exception
                Log_Errori += "RegistroFrizzanti_Acidificazioni: " + vbCrLf + ex.Message + vbCrLf
            End Try

            If Not IsNothing(DtAcidificazioni) AndAlso DtAcidificazioni.Rows.Count > 0 Then
                For i = 0 To Math.Min(DtAcidificazioni.Rows.Count, 8 - 1)

                    CType(rptFrizzanti.Section2.ReportObjects("TextDataAC" & (i + 1).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                          CDate(DtAcidificazioni.Rows(i).Item("Data_Movimento")).ToShortDateString

                    CType(rptFrizzanti.Section2.ReportObjects("TextProdottoAC" & (i + 1).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                         DtAcidificazioni.Rows(i).Item("Mat_Des")

                    CType(rptFrizzanti.Section2.ReportObjects("TextQtaAC" & (i + 1).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                         Format(DtAcidificazioni.Rows(i).Item("Qta"), "0.00") & " " & DtAcidificazioni.Rows(i).Item("Udm_Sim")

                Next
            End If

            DtAcidificazioni.Dispose()
            DtAcidificazioni = Nothing

        End If

    End Sub

    '####################################################################################################################
    ' dolcificazioni TipoDefault 8
    Private Sub Dolcificazioni(ByVal Id_Trasformazione As Integer)

        Dim objStampe As New AgronicaCoreStampeDAL.RegistriCantina2
        ' Dim DtAgenda As DataTable
        Dim DtDolcificazioni As DataTable
        Dim ElencoAgenda As String = String.Empty
        Dim i As Integer

        'MODIFICA DEL 29/11/13: gestita univocamente nel core, la lettura degli id_agenda e composizione stringa
        Try

            Dim filtro_friz As String = ""

            filtro_friz += " Linee_Preparazioni.Modulo_Generazione = " & Agro_SQL_SaveNum(enum_Omni_Modulo_Generazione.Cantine)
            filtro_friz += " AND Linee_Preparazioni.Codice_Generazione IN (" & _
                            CStr(enum_Omni_Preparazione_Cod.Dolcificazione) & _
                            " ) "

            ElencoAgenda = objStampe.RegistroFrizzanti_ElencoIdAgenda(Qs_Piva, _
                                                                        Id_Trasformazione, _
                                                                        0, _
                                                                        filtro_friz, "", _
                                                                        objParametri_Server)



        Catch ex As Exception
            Log_Errori += "Dolcificazioni, recupero id_agenda: " + vbCrLf + ex.Message + vbCrLf
        End Try


        'Try

        '    Dim filtro_friz As String = ""

        '    filtro_friz += " Linee_Preparazioni.Modulo_Generazione = " & SQL_SaveNum(enum_Omni_Modulo_Generazione.Cantine)
        '    filtro_friz += " AND Linee_Preparazioni.Codice_Generazione IN (" & _
        '                    CStr(enum_Omni_Preparazione_Cod.Dolcificazione) & _
        '                    " ) "
        '    '"," & _
        '    'CStr(enum_Omni_Preparazione_Cod.) & "," & _
        '    'CStr(enum_Omni_Preparazione_Cod.) & _

        '    'DtAgenda = objStampe.RegistroFrizzanti_AgendaFromTrasformazione(Qs_Piva, _
        '    '                                                              Id_Trasformazione, _
        '    '                                                              enum_Omni_Codice_Generazione.Tipo10_ArricchimentoDolcificazione, _
        '    '                                                              "", "", _
        '    '                                                              objParametri_Server)

        '    DtAgenda = objStampe.RegistroFrizzanti_AgendaFromTrasformazione(Qs_Piva, _
        '                                                             Id_Trasformazione, _
        '                                                             0, _
        '                                                             filtro_friz, "", _
        '                                                             objParametri_Server)

        'Catch ex As Exception
        '    Log_Errori += "RegistroFrizzanti_AgendaFromTrasformazione: " + vbCrLf + ex.Message + vbCrLf
        'End Try

        '' preparo la stringa che userò nella query successiva nella clausula del where
        'If Not IsNothing(DtAgenda) AndAlso DtAgenda.Rows.Count > 0 Then
        '    For i = 0 To DtAgenda.Rows.Count - 1
        '        ElencoAgenda &= IIf(ElencoAgenda <> String.Empty, ", ", "")
        '        ElencoAgenda &= DtAgenda.Rows(i).Item("Id_Agenda")
        '    Next
        'End If
        'DtAgenda.Dispose()
        'DtAgenda = Nothing

        If ElencoAgenda = "" Then
            'non è un errore, possono non esserci, non stampo nel log
            'Log_Errori += "Non sono state trovate dolcificazioni per quella partita." + vbCrLf + vbCrLf
        Else

            Dim objGenAnag As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Log_R
            Dim DtAnag As DataTable

            Try
                ' recupero il mat_cod dell'MCR
                DtAnag = objGenAnag.Leggi(Qs_Piva, _
                                            -1, -1, _
                                            enum_Omni_Modulo_Generazione.Cantine, _
                                            enum_Omni_Tipo_Generazione.AltreMateriePrime, _
                                            enum_Omni_Codice_Generazione.Tipo9_MCR, _
                                            AGRODATAINIZIO, AGRODATAFINE, _
                                            "", "", _
                                            objParametri_Server)

            Catch ex As Exception
                Log_Errori += "recupero il mat_cod del mosto: " + vbCrLf + ex.Message + vbCrLf
            End Try

            Dim Elem_Cod As Integer = 0
            Dim Mat_Cod As Integer = 0
            If Not IsNothing(DtAnag) AndAlso DtAnag.Rows.Count > 0 Then
                For i = 0 To DtAnag.Rows.Count - 1
                    Elem_Cod = DtAnag.Rows(0).Item("Elem_Cod")
                    Mat_Cod = DtAnag.Rows(0).Item("Mat_Cod")
                Next
            End If
            objGenAnag = Nothing
            DtAnag.Dispose()
            DtAnag = Nothing

            Try
                DtDolcificazioni = objStampe.RegistroFrizzanti_Dolcificazioni(Qs_Piva, _
                                                                            ElencoAgenda, _
                                                                            Elem_Cod, _
                                                                            Mat_Cod, _
                                                                            objParametri_Server)
            Catch ex As Exception
                Log_Errori += "RegistroFrizzanti_Dolcificazioni: " + vbCrLf + ex.Message + vbCrLf
            End Try

            If Not IsNothing(DtDolcificazioni) AndAlso DtDolcificazioni.Rows.Count > 0 Then
                For i = 0 To Math.Min(DtDolcificazioni.Rows.Count, 3) - 1

                    CType(rptFrizzanti.Section2.ReportObjects("TextDataDolcificazione" & (i + 1).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                          CDate(DtDolcificazioni.Rows(i).Item("Data_Movimento")).ToShortDateString

                    CType(rptFrizzanti.Section2.ReportObjects("TextProdottoDolcificazione" & (i + 1).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                         DtDolcificazioni.Rows(i).Item("Mat_Des")

                    CType(rptFrizzanti.Section2.ReportObjects("TextQtaDolcificazione" & (i + 1).ToString), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                         Format(DtDolcificazioni.Rows(i).Item("Qta"), "0.00") & " " & DtDolcificazioni.Rows(i).Item("Udm_Sim")

                Next
            End If

            DtDolcificazioni.Dispose()
            DtDolcificazioni = Nothing

        End If

    End Sub




End Class
