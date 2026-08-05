Imports CrystalDecisions.Shared
Imports CrystalDecisions.CrystalReports.Engine

Imports System.Xml

Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreStampeDAL
Imports AgronicaCoreUtility.Gestione_Eccezioni

Public Class VisualizzatoreReport
    Inherits System.Web.UI.Page

    Dim Qs_Anteprima As String
    Dim Qs_FilePDF As String
    Dim Qs_NomeFilePDF As String = ""
    Dim Qs_DirectPrint As String
    Dim Qs_ForzaAnteprima As Boolean

    'MS L'oggetto ReportDocument viene creato a questo livello per poterlo distruggere al page_unload altrimenti rimane allocato
    Dim RPT As ReportDocument
    Dim rptTempSuDisco As Boolean

    Private Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init

        If Page.Request.QueryString("tmpReportPath") Is Nothing Then
            rptTempSuDisco = False
        Else
            rptTempSuDisco = True
        End If

        Dim oReportDaPassare As List(Of ReportDaPassare) = Session("ListaReportDaPassare")

        If Not IsNothing(oReportDaPassare) Then
            For Each iRptDaPassare In oReportDaPassare

                If iRptDaPassare.Numero_Copie < 0 Then

                    For i As Integer = 1 To Math.Abs(iRptDaPassare.Numero_Copie)

                        System.Threading.Thread.CurrentThread.Sleep(1000)

                        Stampa_SalvandoFileSuServizioWindows( _
                                iRptDaPassare.ilReportDaPassare, _
                                iRptDaPassare.PrintName, _
                                1, _
                                "False", _
                                0, _
                                0, _
                                iRptDaPassare.Larghezza, _
                                iRptDaPassare.Altezza, _
                                False _
                            )
                    Next

                    fCloseAndDispose(iRptDaPassare.ilReportDaPassare)

                Else
                    Stampa_SalvandoFileSuServizioWindows( _
                        iRptDaPassare.ilReportDaPassare, _
                        iRptDaPassare.PrintName, _
                        iRptDaPassare.Numero_Copie, _
                        "False", _
                        0, _
                        0, _
                        iRptDaPassare.Larghezza, _
                        iRptDaPassare.Altezza, _
                        True _
                    )
                End If

               
            Next

            MessaggiServizioWindows.Visible = True
            Exit Sub
        End If

        'MS Spostato a livello di class per poterlo distruggere al page_unload:  Dim RPT As ReportDocument

        'stampa diretta se DirectPrint = 1
        If Not IsNothing(Request.QueryString("PrintServizioWindows")) Then

            Qs_DirectPrint = Stringa_Decodifica(Request.QueryString("PrintServizioWindows").ToString, AgroKey_EncoderDecoder, Server)
            Dim QS_PrintName As String = Stringa_Decodifica(Request.QueryString("QS_PrintName"), AgroKey_EncoderDecoder, Server)
            Dim QS_Numero_Copie As Integer = Stringa_Decodifica(Request.QueryString("QS_Numero_Copie"), AgroKey_EncoderDecoder, Server)
            Dim QS_Str_Flag_Fascicola As String = Stringa_Decodifica(Request.QueryString("QS_Str_Flag_Fascicola"), AgroKey_EncoderDecoder, Server)
            Dim QS_Start_Page As Integer = Stringa_Decodifica(Request.QueryString("QS_Start_Page"), AgroKey_EncoderDecoder, Server)
            Dim QS_End_Page As Integer = Stringa_Decodifica(Request.QueryString("QS_End_Page"), AgroKey_EncoderDecoder, Server)

            '@@MS TODO Gestire questo caso. Non si puà passare report referenziato da session ma va usato rpt temporaneo su disco
            RPT = Session("Report")
            Stampa_SalvandoFileSuServizioWindows(RPT,
                                                 QS_PrintName,
                                                 QS_Numero_Copie,
                                                 QS_Str_Flag_Fascicola,
                                                 QS_Start_Page,
                                                 QS_End_Page,
                                                 0,
                                                 0,
                                                 True)

            MessaggiServizioWindows.Visible = True
            Exit Sub

        End If


        If Not IsNothing(Request.QueryString("anteprima")) Then
            Qs_Anteprima = Stringa_Decodifica(Request.QueryString("anteprima").ToString, AgroKey_EncoderDecoder, Server)
        Else
            Qs_Anteprima = "0"
        End If

        'Questo è il percorso completo del file pdf
        If Not IsNothing(Request.QueryString("pdf")) Then
            Qs_FilePDF = Stringa_Decodifica(Request.QueryString("pdf").ToString, AgroKey_EncoderDecoder, Server)
        Else
            Qs_FilePDF = ""
        End If

        'Questo è il solo nome del file pdf (in modo che il visualizzatore pdf di chrome quando si scarica mostra il nome reale)
        If Not IsNothing(Request.QueryString("NomePdf")) Then
            Qs_NomeFilePDF = Stringa_Decodifica(Request.QueryString("NomePdf").ToString, AgroKey_EncoderDecoder, Server)
            'L'estensione pdf viene aggiunta automaticamente, quindi se il nome file arriva già con l'estensione, allora la devo togliere
            If Qs_NomeFilePDF.EndsWith(".pdf") OrElse Qs_NomeFilePDF.EndsWith(".PDF") Then
                Qs_NomeFilePDF = Qs_NomeFilePDF.Substring(0, (Qs_NomeFilePDF.Length - 4))
            End If
        Else
            Qs_NomeFilePDF = ""
        End If

        If Not IsNothing(Request.QueryString("ForzaAnteprima")) Then
            Qs_ForzaAnteprima = Stringa_Decodifica(Request.QueryString("ForzaAnteprima").ToString, AgroKey_EncoderDecoder, Server)
        Else
            Qs_ForzaAnteprima = False
        End If

        ' nascondo l'albero dei gruppi
        CrystalReportViewer1.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None

        CrystalReportViewer1.DisplayToolbar = True
        CrystalReportViewer1.HasExportButton = True
        CrystalReportViewer1.HasPrintButton = True
        CrystalReportViewer1.HasSearchButton = True

        'MS TODO 29/02/2016 Temporaneamente per garantire la correzione progressiva dei report, manteniamo la vecchia gestione con riferimento al report 
        'passato in session. Una volta convertiti tutti i report alla nuova gestione con rpt temporaneo manteniamo solo il nuovo giro.

        'MS Per discriminare nuova e vecchia gestione verifico se è stato passato i path del report temporaneo in QueryString
        If rptTempSuDisco Then
            'MS Nuovo giro con report temporaneo caricato da disco
            Try
                Dim tmpReportPath As String = Stringa_Decodifica(Page.Request.QueryString("tmpReportPath").ToString, AgroKey_EncoderDecoder, Server)
                RPT = New ReportDocument
                RPT.Load(tmpReportPath)
            Catch ex As Exception
                Throw (New Exception("Errore nel caricamento report temporaneo da VisualizzatoreReport: " & MessaggioCompletoDataEccezione(ex, True), ex))
            End Try
        Else
            'MS Vecchio giro con riferimento a report in session che crea il problema perchè non rilascia la memoria
            RPT = Session("Report")
        End If

        'Controllo se voglio mostrare direttamente il PDF
        Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim DtImpostazioni As DataTable = objUtenti.Leggi_Utente_Poi_SuperUser(enum_Impostazioni_Utenti.UTENTE_COD_MODALITA_STAMPA, 1, _
                             AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                             "", "", Session("ASG_objParametri_Utenti"))

        If Not IsNothing(DtImpostazioni) AndAlso DtImpostazioni.Rows.Count > 0 AndAlso DtImpostazioni.Rows(0).Item("Impostazione_Valore_1") = enum_Modalita_Stampa.MostraPDF Then
            Qs_Anteprima = "0"
        End If

        '30/07/2019: aggiunta possibilità di forzatura apertura anteprima crystal
        '(serviva dal filtro stampe contab)
        If Qs_ForzaAnteprima = True Then
            Qs_Anteprima = "1"
        End If

        'faccio il databind col visualizzatore dei reports...
        CrystalReportViewer1.ReportSource = RPT
        CrystalReportViewer1.DataBind()

        If Qs_Anteprima = "0" AndAlso Qs_FilePDF <> "" Then

            'Controllo che Qs_FilePDF contenga parte del percorso allegati std GIAS per evitare tentativi di download esterni
            Dim objAgroWeb = New AgronicaCoreGestioneRichieste.AgroWebConfig
            Dim PathAllegati As String
            If objAgroWeb.GestioneAllegati_Repository = String.Empty Then
                PathAllegati = "C:\GIASLAN\AgronicaStampe_Allegati\" 'default
            Else
                PathAllegati = objAgroWeb.GestioneAllegati_Repository
            End If

            If Not (Qs_FilePDF.ToLower().Contains(PathAllegati.ToLower()) OrElse Qs_FilePDF.ToLower().Contains(CrystalHelper.getCartellaReportTemporanei().ToLower())) Then
                Throw New Exception("File non trovato.")
            End If

            ' Con il seguente codice il file pdf viene scritto nel browser del client.
            Response.ClearContent()
                Response.ClearHeaders()
                Response.ContentType = "application/pdf"
                '  Giulia, 29/11/2017 15:52:08: salvando il file viene impostato il nome esatto del file, anche se la testata del viewer rimane VisualizzatoreReport.aspx
                Response.AppendHeader("Content-Disposition", "inline; filename=""" & IO.Path.GetFileName(Qs_FilePDF) & """")
                Response.WriteFile(Qs_FilePDF)
                Response.Flush()
                Response.Close()

            ElseIf Qs_Anteprima = "0" AndAlso Qs_FilePDF = "" Then
                RPT.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Me.Response, False, Qs_NomeFilePDF)
        End If

    End Sub

    Private Sub Page_Unload(sender As Object, e As System.EventArgs) Handles Me.Unload

        'MS Solo se si sta usando il nuovo giro con rpt su disco, in uscita distruggo il report per evitare che rimanga appeso in memoria.
        'Non si deve cancellare il report temporaneo su disco perché possono essere fatte postback es. export.
        '@@TODO Una volta corretti tutti i report non sarà più necessaria la condizione.
        If rptTempSuDisco Then
            RPT.Close()
            RPT.Dispose()
            RPT = Nothing
            GC.Collect()
        End If

    End Sub


    Private Function Stampa_SalvandoFileSuServizioWindows( _
        ByVal rpt As ReportDocument, _
        ByVal QS_PrintName As String, _
        ByVal QS_Numero_Copie As Integer, _
        ByVal QS_Str_Flag_Fascicola As String, _
        ByVal QS_Start_Page As Integer, _
        ByVal QS_End_Page As Integer, _
        ByVal QS_Larghezza As Integer, _
        ByVal QS_Altezza As Integer, _
        ByVal CloseAndDispose As Boolean _
        )

        ' Dichiara le variabili e restituisce le opzioni di esportazione.
        Dim exportOpts As ExportOptions
        Dim diskOpts As New DiskFileDestinationOptions
        exportOpts = rpt.ExportOptions

        ' Imposta il formato di esportazione.
        exportOpts.ExportFormatType = ExportFormatType.CrystalReport
        exportOpts.ExportDestinationType = ExportDestinationType.DiskFile

        ' Imposta le opzioni relative al file del disco.
        Dim strPath, PathDatiServizioWinStampa, PathComandiServizioWinStampa, NomeFileRpt As String

        'percorso dati
        Dim objAgroWebconfig As New AgronicaCoreGestioneRichieste.AgroWebConfig

        If Not IsNothing(objAgroWebconfig.PathDati_AgroWinSrvc_PrintUtility) Then
            PathDatiServizioWinStampa = objAgroWebconfig.PathDati_AgroWinSrvc_PrintUtility
        Else
            PathDatiServizioWinStampa = ""
        End If

        If Not IsNothing(objAgroWebconfig.PathComandi_AgroWinSrvc_PrintUtility) Then
            PathComandiServizioWinStampa = objAgroWebconfig.PathComandi_AgroWinSrvc_PrintUtility
        Else
            PathComandiServizioWinStampa = ""
        End If

        If Not PathDatiServizioWinStampa.EndsWith("\") Then PathDatiServizioWinStampa &= "\"

        NomeFileRpt = AgronicaCoreUtility.FileSystemHelper.NomeFileUnivoco("rpt")

        strPath = PathDatiServizioWinStampa & NomeFileRpt

        diskOpts.DiskFileName = strPath
        exportOpts.DestinationOptions = diskOpts

        ' Esportazione del report.
        rpt.Export()

        Dim risp As Boolean
        Dim Log_Errori As String = ""

        risp = StampaToServizioUtility(Request,
                                       rpt,
                                       Log_Errori,
                                       QS_PrintName,
                                       NomeFileRpt,
                                       PathComandiServizioWinStampa,
                                       PathDatiServizioWinStampa,
                                       QS_Numero_Copie,
                                       QS_Str_Flag_Fascicola,
                                       QS_Start_Page,
                                       QS_End_Page,
                                       QS_Altezza,
                                       QS_Larghezza,
                                       CloseAndDispose)

    End Function


    '####################################################################
    'Invia il file da stampare al servizio di utility di Net-Agree con un comando di stampa associato
    Public Function StampaToServizioUtility( _
                    ByRef Request As HttpRequest, _
                    ByVal rpt As ReportDocument, _
                    ByRef MsgErrore As String, _
                    ByVal NomeStampante As String, _
                    ByVal NomeFileDati As String, _
                    ByVal PathComandiServizioWinStampa As String, _
                    ByVal PathDatiServizioWinStampa As String, _
                    ByVal NumeroCopie As Integer, _
                    ByVal Str_Flag_Fascicola As String, _
                    ByVal Start_Page As Integer, _
                    ByVal End_Page As Integer, _
                    ByVal Altezza As Integer, _
                    ByVal Larghezza As Integer, _
                    ByVal CloseAndDispose As Boolean _
                ) As Boolean

        Dim ip As String = Request.UserHostAddress()

        Dim NomeFileComandi As String
        'Dim NomeFileDati As String
        Dim IstanteCorrente As Date
        Dim Ok As Boolean

        IstanteCorrente = Now

        '-----------------------------------------------------------
        'Salvo il report nella directory dei dati del servizio di utility

        Try

            If NomeFileDati = "" Then
                NomeFileDati = Year(IstanteCorrente) & Right("0" & Month(IstanteCorrente), 2) & Right("0" & Day(IstanteCorrente), 2) & _
                                Right("0" & Hour(IstanteCorrente), 2) & Right("0" & Minute(IstanteCorrente), 2) & Right("0" & Second(IstanteCorrente), 2) & Right("00" & IstanteCorrente.Millisecond, 3)

                NomeFileDati &= "_"
                NomeFileDati &= HttpContext.Current.Session.SessionID
                NomeFileDati &= ".rpt"
            End If

            'rpt.SaveAs(PathDatiServizioWinStampa & NomeFileDati, True)
            rpt.SaveAs(PathDatiServizioWinStampa & NomeFileDati, CrystalDecisions.[Shared].ReportFileFormat.VSNetFileFormat)

            If CloseAndDispose Then
                fCloseAndDispose(rpt)
            End If



        Catch ex As Exception
            Ok = False
            MsgErrore = "Impossibile completare l'operazione. Errore in fase di salvataggio del report nella directory dei dati del servizio di stampa. " & ex.Message
        End Try
        '-----------------------------------------------------------

        '-----------------------------
        'Creo e salvo il file dei comandi
        Try

            'Creo il file dei comandi
            Dim XmlDocComandi As XmlDocument
            XmlDocComandi = File_Xml_Comandi( _
                NomeFileDati, _
                NomeStampante, _
                NumeroCopie, _
                Str_Flag_Fascicola, _
                Start_Page, _
                End_Page, _
                Altezza, _
                Larghezza _
            )

            NomeFileComandi = NomeFileDati.Replace(".rpt", ".xml").Replace(".pdf", ".xml")

            XmlDocComandi.Save(AgronicaCoreUtility.FileSystemHelper.AggiungiSlashSeNonEsiste(PathComandiServizioWinStampa) & NomeFileComandi)


        Catch ex As Exception
            Ok = False
            MsgErrore = "Impossibile completare l'operazione. Errore in fase di creazione o salvataggio del file dei comandi di stampa. " & ex.Message
        End Try
        '-----------------------------------------------------------

    End Function


    Private Function fCloseAndDispose(ByVal rpt As ReportDocument)
        rpt.Close()

        rpt.Dispose()

        rpt = Nothing

        FileSystem.ChDir("C:\")

        GC.Collect()
    End Function

    '####################################################################
    Private Function File_Xml_Comandi( _
                ByVal NomeFileDati As String, _
                ByVal NomeStampante As String, _
                ByVal NumeroCopie As Integer, _
                ByVal Str_Flag_Fascicola As String, _
                ByVal Start_Page As Integer, _
                ByVal End_Page As Integer, _
                ByVal Altezza As Integer, _
                ByVal Larghezza As Integer _
        ) As XmlDocument

        Dim XmlDocComandi As XmlDocument
        Dim NodoRoot, Nodo, NodoDocumento As XmlElement

        XmlDocComandi = New XmlDocument

        'radice
        NodoRoot = XmlDocComandi.CreateElement("dataroot")
        XmlDocComandi.AppendChild(NodoRoot)

        'comando
        Nodo = XmlDocComandi.CreateElement("comando")

        Nodo.InnerText = "PRINT"
        NodoRoot.AppendChild(Nodo)

        'documento da stampare
        NodoDocumento = XmlDocComandi.CreateElement("documento")
        NodoRoot.AppendChild(NodoDocumento)

        'parametri documento da stampare
        '. formato
        Nodo = XmlDocComandi.CreateElement("formato")
        Nodo.InnerText = "CrystalReports"
        NodoDocumento.AppendChild(Nodo)

        '. file
        Nodo = XmlDocComandi.CreateElement("file")
        Nodo.InnerText = NomeFileDati
        NodoDocumento.AppendChild(Nodo)

        '. stampante
        Nodo = XmlDocComandi.CreateElement("stampante")
        Nodo.InnerText = NomeStampante
        NodoDocumento.AppendChild(Nodo)

        '. numeroCopie
        Nodo = XmlDocComandi.CreateElement("numeroCopie")
        Nodo.InnerText = CStr(NumeroCopie)
        NodoDocumento.AppendChild(Nodo)

        '. collated
        Nodo = XmlDocComandi.CreateElement("collated")
        Nodo.InnerText = Str_Flag_Fascicola
        NodoDocumento.AppendChild(Nodo)

        '. startPage
        Nodo = XmlDocComandi.CreateElement("startPage")
        Nodo.InnerText = CStr(Start_Page)
        NodoDocumento.AppendChild(Nodo)

        '. endPage
        Nodo = XmlDocComandi.CreateElement("endPage")
        Nodo.InnerText = CStr(End_Page)
        NodoDocumento.AppendChild(Nodo)

        '. Altezza
        Nodo = XmlDocComandi.CreateElement("altezza")
        Nodo.InnerText = CStr(Altezza)
        NodoDocumento.AppendChild(Nodo)

        '. endPage
        Nodo = XmlDocComandi.CreateElement("larghezza")
        Nodo.InnerText = CStr(Larghezza)
        NodoDocumento.AppendChild(Nodo)

        Return XmlDocComandi

    End Function

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

End Class