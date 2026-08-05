Imports AgronicaCoreAuditBIZ
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared

Partial Public Class GlobalGap_RapportoNC
    Inherits System.Web.UI.Page

    Private rptStampa As Rpt_RapportoNC


    '----- Gestione Querystring
    Dim Qs_Piva As String
    Dim Qs_Piva_SuperUser As String
    Dim Qs_AuditCod As String
    Dim Qs_AuditData As Date
    Dim Qs_RegolamentoCod As String

#Region " Codice generato da Progettazione Web Form "

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

        rptStampa = New Rpt_RapportoNC

    End Sub

#End Region


    '###########################################################################################
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load



        'Tolgo la pagina dalla cache
        Response.Expires = 0

        'Se vengono selezionati molti dati , la pagina va in timeout ...
        'Allungo il timeout dai 180 secondi di default (3 minuti) a 900 secondi (15 minuti)
        Server.ScriptTimeout = 900


        '##############################################################
        '#####  Verifico Credenziali di Accesso  ######################
        '##############################################################

        '----- Verifico che l'utente sia autenticato

        If Session("ASG_objParametri_Server") Is Nothing Then
            Response.Redirect("~/Custom500.aspx")
        End If

        '----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina

        Dim UtenteAbilitato As Boolean
        Dim strDummy As String = ""     'controllo accesso negato.....


        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        UtenteAbilitato = objPermessi.Controlla_Permessi_Utente(
                    Session("ASG_Utente_Username"),
                    Session("ASG_IdServizio"),
                    enum_Security_Attivita.Gest_CartellaAziendale_GlobalGap,
                    enum_Security_Operazione.Lettura,
                    Date.Now,
                    "",
                    objParametri_Utenti)


        If UtenteAbilitato = False Then
            Response.Redirect("../../Messaggi/AccessoNegato.htm")
        End If


        '##############################################################
        '#####  QUERYSTRING   #########################################
        '##############################################################
        Dim ObjAgronicaDataProvider As New AgronicaCoreDataProvider.Sicurezza
        Dim ObjAgronicaDataProviderCostanti As New AgronicaCoreDataProvider.CostantiPersonalizzate

        Qs_Piva = ObjAgronicaDataProvider.Stringa_Decodifica(Request.QueryString("p").ToString,
                                     ObjAgronicaDataProviderCostanti.AgroKey_EncoderDecoder,
                                     Server)


        Qs_AuditCod = ObjAgronicaDataProvider.Stringa_Decodifica(Request.QueryString("c").ToString,
                                         ObjAgronicaDataProviderCostanti.AgroKey_EncoderDecoder,
                                         Server)

        Qs_AuditData = CDate(ObjAgronicaDataProvider.Stringa_Decodifica(Request.QueryString("d").ToString,
                                                ObjAgronicaDataProviderCostanti.AgroKey_EncoderDecoder,
                                                Server))

        If Not IsNothing(Request.QueryString("r")) Then

            Qs_RegolamentoCod = ObjAgronicaDataProvider.Stringa_Decodifica(Request.QueryString("r").ToString,
                                                   ObjAgronicaDataProviderCostanti.AgroKey_EncoderDecoder,
                                                   Server)
        End If

        Qs_Piva_SuperUser = CStr(Session("ASG_SuperUser_Codfiscale"))



        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        'Dim DT_Intestazione As DataTable

        Dim CrystalReportViewer1 As CrystalDecisions.Web.CrystalReportViewer

        Dim Log As String = ""
        Dim strPath As String
        Dim PathFileTemporanei As String


        If Not Me.IsPostBack Then

            CrystalReportViewer1 = New CrystalDecisions.Web.CrystalReportViewer

            CrystalReportViewer1.Style.Add("LEFT", "-275px")
            CrystalReportViewer1.Style.Add("TOP", "0px")
            CrystalReportViewer1.Style.Add("POSITION", "Absolute")
            Me.FindControl("Form1").Controls.Add(CrystalReportViewer1)

            'carico i dati nel datatable 
            Dim ciSonoDati As Boolean = Carica_Report()

            If Not ciSonoDati Then
                'se non ci sono dati per il report esco
                Call AgroMsgBox("Non ci sono rapporti di non conformità da stampare.", Page, , False)
                Exit Sub
            End If

            '---------------------------------------------------
            '---------------------------------------------------
            '---------------------------------------------------
            'faccio il databind col visualizzatore dei reports...
            CrystalReportViewer1.ReportSource = rptStampa
            CrystalReportViewer1.DataBind()

            ' Dichiara le variabili e restituisce le opzioni di esportazione.
            Dim exportOpts As New ExportOptions
            Dim diskOpts As New DiskFileDestinationOptions
            exportOpts = rptStampa.ExportOptions

            ' Imposta il formato di esportazione.
            exportOpts.ExportFormatType = ExportFormatType.PortableDocFormat
            exportOpts.ExportDestinationType = ExportDestinationType.DiskFile

            ' Imposta le opzioni relative al file del disco.
            ' If ConfigurationManager.AppSettings("PathFileTemporanei").ToString = "" Then
            PathFileTemporanei = Server.MapPath("../../File_Temporanei")
            'Else
            'PathFileTemporanei = ConfigurationManager.AppSettings("PathFileTemporanei").ToString
            ' End If

            strPath = PathFileTemporanei & "\RapportoNC_" & Session.SessionID.ToString & Date.Now.ToFileTimeUtc.ToString & ".pdf"

            diskOpts.DiskFileName = strPath
            exportOpts.DestinationOptions = diskOpts

            'Imposto la stampa orizzontale
            'rptStampa.PrintOptions.PaperOrientation = PaperOrientation.Landscape

            ' Esportazione del report.
            rptStampa.Export()

            'End Select

        End If


        ' Con il seguente codice il file pdf viene scritto 
        '  nel browser del client.
        Response.ClearContent()
        Response.ClearHeaders()
        Response.ContentType = "application/pdf"
        Response.WriteFile(strPath)
        Response.Flush()
        Response.Close()

        ' il file esportato viene eliminato dal disco
        System.IO.File.Delete(strPath)

    End Sub

    '################################################################################
    Private Function Carica_Report() As Boolean

        CType(rptStampa.Section2.ReportObjects("TextAuditCod"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "NON CONFORMITA' N°: " & Qs_AuditCod
        CType(rptStampa.Section2.ReportObjects("TextData"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "DATA: " & Qs_AuditData

        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))


        Dim objDS As New DS_RapportoNC
        Dim dr As DS_RapportoNC.DT_RapportoNCRow
        Dim ObjAnagrafe As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        Dim ObjAudit_Risposte As New AgronicaCoreAuditDAL.Audit_Risposte_R
        'Dim ObjAudit_Disposizioni As New AgronicaCoreAuditDAL.Audit_Disposizioni_R

        Dim Validita_Inizio As Date = CDate("01/01/1900")
        Dim Validita_Fine As Date = CDate("31/12/2100")

        Dim strErr As String = String.Empty
        Dim Dt As DataTable

        'Dt = ObjAudit_Risposte.AuditRisposte_Leggi_Punteggio(enum_AuditPuaTipo.Audit_GlobalGap, Qs_RegolamentoCod, Qs_AuditCod,
        'objParametri_Server.PivaSuperUser, 0, "", "", objParametri_Server, Validita_Inizio, Validita_Fine,, , "0")

        Dim auditAgronica As New AuditAgronicaWS(objParametri_Server)
        Dim codici = auditAgronica.LeggiCodici(enum_AuditPuaTipo.Audit_GlobalGap, Qs_RegolamentoCod, 0, 0, "")
        Dt = ObjAudit_Risposte.AuditRisposte_Leggi_Punteggio_Senza_Codici(enum_AuditPuaTipo.Audit_GlobalGap,
                                                                                   Qs_RegolamentoCod, Qs_AuditCod, objParametri_Server.PivaSuperUser,
                                                                                   0, "", "", objParametri_Server, Validita_Inizio, Validita_Fine, , "0")
        Dim temp As AuditCodiciModel

        For Each risp In Dt.Rows

            temp = codici.Where(Function(x) x.Disp_Cod = risp.Item("Disp_Cod") And
                                    x.Punto_Numero = risp.Item("Punto_Numero") And
                                    x.Audit_Tipo = enum_AuditPuaTipo.Audit_GlobalGap And
                                    x.Regolamento_Cod = Qs_RegolamentoCod).FirstOrDefault()

            If temp IsNot Nothing Then
                risp.Item("Tipo") = CStr(temp.Tipo)
                risp.Item("Descrizione") = CStr(temp.Descrizione)
                risp.Item("Punteggio") = CInt(temp.Punteggio)
                'risp.Item("PropostaCorrettiva") = CInt(temp.PropostaCorrettiva)
            End If

        Next

        If Dt.Rows.Count = 0 Then
            'se non ci sono dati nel datatable e quindi nmon ci sono risposte di non conformità ritorno false
            Return False
        End If

        Dim logo() As Byte
        Try
            Dim objWebConfig As AgronicaCoreGestioneRichieste.AgroWebConfig = New AgronicaCoreGestioneRichieste.AgroWebConfig()
            'Dim bmpF As System.Drawing.Bitmap = New System.Drawing.Bitmap("C:\AgroSorgenti - Copia\AgronicaAudit\AgronicaGlobalGap\AB_Immagini\Logo\Logo_Agronica.bmp")
            If objWebConfig.Path_Directory_Loghi_Cliente <> "" Then
                Dim path As String = objWebConfig.Path_Directory_Loghi_Cliente & "Logo_Orizzontale_Standard.bmp"
                Dim bmpF As System.Drawing.Bitmap = New System.Drawing.Bitmap(path)
                ' metto l'immagine nella colonna blob
                Dim c As New System.Drawing.ImageConverter
                logo = c.ConvertTo(bmpF, GetType(Byte()))
                ' DataSet_DT_RiepilogoRapportoRow.Item("Blob_Logo") = c.ConvertTo(bmpF, GetType(Byte()))
            End If

        Catch ex As Exception

        End Try

        Dim i As Integer
        For i = 0 To Dt.Rows.Count - 1

            ' nel report non ci vanno le Raccomandazioni (punteggio=1)
            If Dt.Rows(i).Item("Punteggio") <> 1 Then

                dr = objDS.DT_RapportoNC.NewDT_RapportoNCRow

                Try
                    If Not IsNothing(logo) Then
                        dr.Item("Blob_Logo") = logo
                    End If
                Catch ex As Exception

                End Try

                Dim ptoUno As String = CInt(Mid(Dt.Rows(i).Item("Punto_Numero"), 1, 2))
                Dim ptoDue As String = CInt(Mid(Dt.Rows(i).Item("Punto_Numero"), 3, 2))
                Dim ptoTre As String = CInt(Mid(Dt.Rows(i).Item("Punto_Numero"), 5, 2))


                'dr.Punto_Numero = ObjAudit_Disposizioni.DispNome_From_DispCod(enum_AuditPuaTipo.Audit_GlobalGap, Qs_RegolamentoCod, Dt.Rows(i).Item("Disp_Cod"), objParametri_Server) & " " &
                '                                               ptoUno &
                '                                               IIf(ptoDue <> "0", "." & ptoDue, "") &
                '                                               IIf(ptoTre <> "0", "." & ptoTre, "")

                dr.Punto_Numero = auditAgronica.LeggiDisposizioni(enum_AuditPuaTipo.Audit_GlobalGap, CInt(Qs_RegolamentoCod), 0, Dt.Rows(i).Item("Disp_Cod")).FirstOrDefault.Disp_Nome & " " &
                                                               ptoUno &
                                                               IIf(ptoDue <> "0", "." & ptoDue, "") &
                                                               IIf(ptoTre <> "0", "." & ptoTre, "")

                Select Case Dt.Rows(i).Item("Punteggio")
                    Case 2
                        dr.Punteggio = "Minore"
                    Case 3
                        dr.Punteggio = "Maggiore"
                End Select
                If IsDBNull(Dt.Rows(i).Item("PropostaCorrettiva")) Then
                    dr.PropostaCorrettiva = "-------------------"
                Else
                    dr.PropostaCorrettiva = Dt.Rows(i).Item("PropostaCorrettiva")
                End If

                objDS.DT_RapportoNC.AddDT_RapportoNCRow(dr)

            End If

            rptStampa.SetDataSource(objDS)
        Next

        '-------------------------------------------------
        Return True
    End Function


    '################################################################
    Private Sub AgroMsgBox(ByVal Testo As String,
                          ByRef objPage As System.Web.UI.Page,
                          Optional ByVal NomeForm As String = "FORM1",
                          Optional ByVal FlagMaster As Boolean = True)

        '----- Formatto il testo di ingresso in modo che non crei problemi ...

        'Elimino il carattere \ e lo sostituisco con \\
        Testo = Replace(Testo, "\", "\\")

        'Elimino il carattere (o coppia di caratteri) VBCRLF che crea problemi ...
        Testo = Replace(Testo, vbCrLf, Chr(13))

        'Elimino i doppi apici sostituendoli con due singoli apostrofi chr(96)
        Testo = Replace(Testo, Chr(34), Chr(96) & Chr(96))

        'Elimino i singoli apici sostituendoli con un apostrofo chr(96)
        Testo = Replace(Testo, Chr(39), Chr(96))

        'Gli eventuali Chr(13) li sostituisco con l'equivalente JS
        'attenzione! va messo dopo la sostituzione di \ con \\ !!!
        Testo = Replace(Testo, Chr(13), "\r")

        Select Case FlagMaster

            Case True
                '----- Faccio apparire un msgbox aggiungendo il controllo alla form
                objPage.Master.FindControl(NomeForm).Controls.Add(
                    New LiteralControl(
                        "<script language='javascript'>alert('" & Testo & "');</script>"))
            Case False
                '----- Faccio apparire un msgbox aggiungendo il controllo alla form
                objPage.FindControl(NomeForm).Controls.Add(
                    New LiteralControl(
                        "<script language='javascript'>alert('" & Testo & "');</script>"))
        End Select

    End Sub


End Class