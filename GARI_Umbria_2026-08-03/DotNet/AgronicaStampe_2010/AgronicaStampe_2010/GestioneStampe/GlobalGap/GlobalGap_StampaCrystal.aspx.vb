Imports AgronicaCoreAuditBIZ
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework_POCO
Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared


Partial Public Class GlobalGap_StampaCrystal
    Inherits System.Web.UI.Page

    Private rptStampa As Rpt_GlobalGap


    '----- Gestione Querystring
    Dim Qs_Piva As String
    Dim Qs_Piva_SuperUser As String
    Dim Qs_AuditCod As String
    Dim Qs_AuditData As Date
    Dim Qs_ElencoDisp As String
    Dim Qs_RegolamentoCod As String
    Dim Qs_ProfiloCod As String




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

        rptStampa = New Rpt_GlobalGap

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

        Qs_ElencoDisp = ObjAgronicaDataProvider.Stringa_Decodifica(Request.QueryString("e").ToString,
                                           ObjAgronicaDataProviderCostanti.AgroKey_EncoderDecoder,
                                           Server)

        If Not IsNothing(Request.QueryString("r")) Then

            Qs_RegolamentoCod = ObjAgronicaDataProvider.Stringa_Decodifica(Request.QueryString("r").ToString,
                                                   ObjAgronicaDataProviderCostanti.AgroKey_EncoderDecoder,
                                                   Server)
        End If

        If Not IsNothing(Request.QueryString("pc")) Then

            Qs_ProfiloCod = ObjAgronicaDataProvider.Stringa_Decodifica(Request.QueryString("pc").ToString,
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
            Carica_Report()

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

            strPath = PathFileTemporanei & "\CheckListGG_" & Session.SessionID.ToString & Date.Now.ToFileTimeUtc.ToString & ".pdf"

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
    Private Sub Carica_Report()
        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))

        'Dim DLL_AD As New AccessoDB_Condizionalita.AccessoDati(objparametri_server)
        ' ''Dim objDS_Condi As New DS_Cond
        ' ''Dim dr As DS_Condi.DT_CondiRow
        Dim ObjAnagrafe As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        Dim ObjAudit As New AgronicaCoreAuditDAL.Audit_R

        '**********************************************************************************************************
        '**  Intestazione      ************************************************************************************
        '**********************************************************************************************************

        Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
        CType(rptStampa.Section1.ReportObjects("TextRagSoc"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = objImprese.RagSoc_from_Piva(Qs_Piva, objParametri_Server)

        Dim strCuaa As String = ObjAnagrafe.CUAA_From_Piva(Qs_Piva, objParametri_Server)
        CType(rptStampa.Section1.ReportObjects("TextCUAA"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = IIf(strCuaa <> String.Empty, "CUAA: " & strCuaa, String.Empty)

        Dim strIndirizzo As String = ObjAnagrafe.Indirizzo_From_Piva(Qs_Piva, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, objParametri_Server)
        CType(rptStampa.Section1.ReportObjects("TextIndirizzo"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = IIf(strIndirizzo <> String.Empty, "Indirizzo: " & strIndirizzo, String.Empty)

        Dim strErr As String = String.Empty

        Dim DataCompilazione As String = String.Empty
        Dim UsernameCompilazione As String = String.Empty
        Dim Stato As String = String.Empty
        Dim Note As String = String.Empty
        Dim Validita_Inizio As Date = CDate("01/01/1900")
        Dim Validita_Fine As Date = CDate("31/12/2100")
        If Qs_AuditCod <> "0" Then

            Dim Dt As DataTable
            Dt = ObjAudit.LeggiAudit2(Qs_AuditCod, enum_AuditPuaTipo.Audit_GlobalGap,
                                     Qs_RegolamentoCod,
                                     Qs_Piva,
                                     Validita_Inizio,
                                     Validita_Fine,
                                     "", "",
                                     objParametri_Server)


            If Dt.Rows.Count > 0 Then
                DataCompilazione = CDate(Dt.Rows(0).Item("Validita_Inizio")).ToShortDateString
                UsernameCompilazione = Dt.Rows(0).Item("Username_Modifica")
                If Not IsDBNull(Dt.Rows(0).Item("Stato_Des")) Then
                    Stato = Dt.Rows(0).Item("Stato_Des")
                End If
                Note = Dt.Rows(0).Item("Note")
            End If

            If UsernameCompilazione <> String.Empty Then

                Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Read

                ObjUtenti.Nome_From_CF(UsernameCompilazione,
                                       UsernameCompilazione,
                                       objParametri_Utenti)

                If UsernameCompilazione = "" Then
                    UsernameCompilazione = "______________________"
                End If

            End If
            Dt.Dispose()
            Dt = Nothing


        Else

            UsernameCompilazione = "__________________"

            DataCompilazione = "__________________"

            Stato = "__________________"

        End If

        CType(rptStampa.Section1.ReportObjects("TextTecnico"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Tecnico: " & UsernameCompilazione
        CType(rptStampa.Section1.ReportObjects("TextData"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Data: " & DataCompilazione
        CType(rptStampa.Section1.ReportObjects("TextStato"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Stato: " & Stato
        CType(rptStampa.SectionNote.ReportObjects("TextNote"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Note

        '' '' ''-------------------------------------------------
        '' '' '' creo la riga della superpiva
        ' '' ''dr = objDS_Condi.DT_Condi.NewDT_CondiRow

        ' '' ''dr.SuperPiva = Qs_Piva_SuperUser

        ' '' ''objDS_Condi.DT_Condi.AddDT_CondiRow(dr)

        ' '' ''rptStampa.SetDataSource(objDS_Condi)
        '' '' ''-------------------------------------------------

        'DLL_AD = Nothing


        Dim vetDisp As String() = Qs_ElencoDisp.Split(",")

        Dim bVisualizzaTabellaPunteggi As Boolean = False

        Dim objDS_CheckList As New DS_CheckList
        Dim objDS_Punteggio As New DS_Punteggio
        Dim objDS_Profilazione As New DS_Profilazione

        Dim bProfilazione As Boolean = False
        Dim bDisposizioni As Boolean = False
        Dim bPunteggio As Boolean = False
        Dim bNote As Boolean = False

        If Note <> String.Empty Then
            bNote = True
        End If

        Dim strRis As String = String.Empty

        ' Recupero le disposizioni selezionate
        Dim disp As String
        For Each disp In vetDisp
            If disp.Trim() <> "" Then

                ' tabella dei punteggi
                If disp.Trim() = "P" Then

                    Carica_Punteggio(objDS_Punteggio, strRis)
                    CType(rptStampa.Section1.ReportObjects("TextRisultato"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = strRis
                    bPunteggio = True

                ElseIf disp.Trim = "F" Then

                    Carica_Profilazione(objDS_Profilazione)
                    bProfilazione = True

                Else
                    Carica_Disposizione(disp, objDS_CheckList)

                    If Not IsNothing(disp) AndAlso disp.Length > 0 Then
                        rptStampa.OpenSubreport("Rpt_CheckList.rpt").SetDataSource(objDS_CheckList)
                        bDisposizioni = True
                    End If

                End If

            End If

        Next disp   ' ciclo sul vettore delle disposizioni

        ' elimino le sezioni non selezionate
        If Not bDisposizioni Then
            rptStampa.Section3.SectionFormat.EnableSuppress = True
        End If

        If Not bPunteggio Then
            rptStampa.DetailSection2.SectionFormat.EnableSuppress = True
            rptStampa.DetailSection3.SectionFormat.EnableSuppress = True
        End If

        If Not bProfilazione Then
            rptStampa.DetailSection1.SectionFormat.EnableSuppress = True
        End If

        ' '' ''If Not bNote Then
        ' '' ''    rptStampa.SectionNote.SectionFormat.EnableSuppress = True
        ' '' ''End If

        objDS_CheckList.Dispose()
        objDS_Punteggio.Dispose()
        objDS_Profilazione.Dispose()


    End Sub


    '################################################################################
    Private Sub Carica_Disposizione(ByVal disp As Integer, ByRef objDs_CheckList As DS_CheckList)
        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        'Dim DLL_AD As New AccessoDB_Condizionalita.AccessoDati(objparametri_server)

        Dim dr As DS_CheckList.DT_CheckListRow

        Dim list_Disp As List(Of AuditDisposizioniModel)
        Dim list_Sezioni As List(Of AuditSezioniModel)
        Dim DT_Codici As DataTable
        Dim codici As List(Of AuditCodiciModel)
        ' Dim FiltroParte As Integer
        Dim bAggiungiSezione As Boolean

        Dim strErr As String = String.Empty
        Dim auditAgronica As New AuditAgronicaWS(objParametri_Server)
        '' se il FiltroParte = 0 allora stampo entrambe le parti della check-list (elementi di verifica e calcolo riduzione)
        '' se Qs_AuditCod=0 vuol dire che voglio stampare la check vuota e devo stampare sia la prima che la seconda parte
        ''FiltroParte = 0
        'If FiltroParte = 0 AndAlso Qs_AuditCod <> "0" AndAlso DLL_AD.EsitoDisposizione(Session("ASG_SuperUser_CodFiscale"), enum_AuditPuaTipo.Audit_Condizionalita, _
        '                                                     CInt(Qs_RegolamentoCod), _
        '                                                     CInt(Qs_AuditCod), CInt(disp), _
        '                                                     enum_AuditSezioni.AuditSezioni_ElementiVerifica, _
        '                                                     enum_AuditSezioni.AuditSezioni_Deroghe) Then
        '    FiltroParte = 1
        'End If

        list_Disp = auditAgronica.LeggiDisposizioni(enum_AuditPuaTipo.Audit_GlobalGap, CInt(Qs_RegolamentoCod), 0, CInt(disp))
        'Dim ObjAudit_Disposizioni As New AgronicaCoreAuditDAL.Audit_Disposizioni_R
        'DT_Disp = ObjAudit_Disposizioni.Leggi(enum_AuditPuaTipo.Audit_GlobalGap,
        '                                     CInt(Qs_RegolamentoCod),
        '                                     0,
        '                                     CInt(disp), "", "", "", objParametri_Server)

        'Dim ObjAudit_Sezioni As New AgronicaCoreAuditDAL.Audit_Sezioni_R
        list_Sezioni = auditAgronica.LeggiSezioni(enum_AuditPuaTipo.Audit_GlobalGap, CInt(Qs_RegolamentoCod), 0, 0, #1/1/1900#)
        'ObjAudit_Sezioni.Leggi(enum_AuditPuaTipo.Audit_GlobalGap, CInt(Qs_RegolamentoCod), 0, 0, #1/1/1900#, strErr, "", "", objParametri_Server,, AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti)

        ' ciclo sulle sezioni ordinate per Ordine
        For Each sezione In list_Sezioni

            bAggiungiSezione = True

            codici = auditAgronica.LeggiCodici(enum_AuditPuaTipo.Audit_GlobalGap, Qs_RegolamentoCod, CInt(disp), sezione.Sezione_Cod, "")

            'Dim Riga As HtmlTableRow
            'Dim Cella As HtmlTableCell
            'Dim Cella_dx As HtmlTableCell

            ' ciclo sui codici per creare i controlli
            For Each codice In codici

                Dim ObjAudit_Risposte As New AgronicaCoreAuditDAL.Audit_Risposte_R
                DT_Codici = ObjAudit_Risposte.LeggiDaCodiciGG(CInt(Qs_AuditCod),
                                                              enum_AuditPuaTipo.Audit_GlobalGap,
                                                              CInt(Qs_RegolamentoCod), CInt(disp),
                                                              codice.Punto_Numero, objParametri_Server)

                ' creo la riga della sezione
                dr = objDs_CheckList.DT_CheckList.NewDT_CheckListRow

                dr.Disp_Cod = disp
                dr.Disp_Nome = list_Disp.First.Disp_Nome & " - " & list_Disp.First.Descrizione
                dr.Sezione_Cod = sezione.Sezione_Cod
                dr.Sezione_Des = sezione.Sezione_Des

                dr.Punto_Numero = list_Disp.First.Disp_Nome & " "

                If codice.Punto_Numero <> String.Empty Then

                    If CInt(Mid(codice.Punto_Numero, 1, 2)) <> 0 Then
                        dr.Punto_Numero &= CInt(Mid(codice.Punto_Numero, 1, 2)).ToString

                        If CInt(Mid(codice.Punto_Numero, 3, 2)) <> 0 Then
                            dr.Punto_Numero &= "." & CInt(Mid(codice.Punto_Numero, 3, 2)).ToString

                            If CInt(Mid(codice.Punto_Numero, 5, 2)) <> 0 Then
                                dr.Punto_Numero &= "." & CInt(Mid(codice.Punto_Numero, 5, 2)).ToString

                            End If
                        End If
                    End If


                End If
                ' dr.Punto_Numero = DT_Codici.Rows(j).Item("Punto_Numero")
                dr.Descrizione = codice.Descrizione
                dr.PropostaCorrettiva = If(DT_Codici.Rows.Count > 0, DT_Codici.Rows(0).Item("ValPropostaCorrettiva"), "")

                Select Case codice.Punteggio
                    Case 1
                        dr.Punteggio = "Raccom."
                    Case 2
                        dr.Punteggio = "Minore"
                    Case 3
                        dr.Punteggio = "Maggiore"
                    Case Else
                        dr.Punteggio = ""
                End Select


                If codice.Nota = 0 Then

                    Select Case codice.Tipo

                        Case "sn", "sna"

                            If DT_Codici.Rows.Count > 0 AndAlso
                                Not IsDBNull(DT_Codici.Rows(0).Item("Valore")) Then
                                Select Case DT_Codici.Rows(0).Item("Valore")
                                    Case "0"
                                        'dr.Valore = "¡Si ¤No ¡Non app."
                                        dr.Valore = "No"
                                    Case "1"
                                        ' dr.Valore = "¤Si ¡No ¡Non app."
                                        dr.Valore = "Sì"
                                    Case "-1"
                                        'dr.Valore = "¡Si ¡No ¤Non app."
                                        dr.Valore = "N.a."
                                    Case Else
                                        dr.Valore = "Si   No   N.a."
                                End Select
                            Else
                                dr.Valore = "Si   No   N.a."
                            End If

                            'Crea_RadioButtonList(Cella, CLng(DT_Codici.Rows(j).Item("Disp_Cod")).ToString & "_" & DT_Codici.Rows(j).Item("Punto_Numero"), DT_Codici.Rows(j).Item("Valore").ToString, bSolaLettura)

                        Case "c", "cs"

                            If DT_Codici.Rows.Count > 0 AndAlso
                                Not IsDBNull(DT_Codici.Rows(0).Item("Valore")) Then
                                If DT_Codici.Rows(0).Item("Valore") = "1" Then
                                    'dr.Valore = "þ"
                                    dr.Valore = "Sì"
                                Else
                                    'dr.Valore = "¨"
                                    dr.Valore = "No"
                                End If
                            Else
                                dr.Valore = "Si   No"
                            End If
                            '   Crea_CheckBox(Cella, CLng(DT_Codici.Rows(j).Item("Disp_Cod")) & "_" & DT_Codici.Rows(j).Item("Punto_Numero"), "", DT_Codici.Rows(j).Item("Valore").ToString, bSolaLettura)

                            ''Case "cs"

                            ''    Crea_CheckBox(Cella, CLng(DT_Codici.Rows(j).Item("Disp_Cod")) & "_" & DT_Codici.Rows(j).Item("Punto_Numero"), "", DT_Codici.Rows(j).Item("Valore").ToString, bSolaLettura)


                        Case "n"

                            dr.Valore = "Note"
                            If DT_Codici.Rows.Count > 0 AndAlso
                                Not IsDBNull(DT_Codici.Rows(0).Item("Valore")) Then
                                dr.Descrizione = DT_Codici.Rows(0).Item("Valore").ToString
                            Else
                                dr.Descrizione = ""
                            End If


                        Case Else
                            dr.Valore = ""

                    End Select

                Else
                    dr.Valore = "----"

                End If

                ' pulizia dei tag html
                If Not IsDBNull(dr.Valore) Then
                    dr.Valore = dr.Valore.Replace("<i>", "")
                    dr.Valore = dr.Valore.Replace("</i>", "")
                    dr.Valore = dr.Valore.Replace("<b>", "")
                    dr.Valore = dr.Valore.Replace("</b>", "")
                    dr.Valore = dr.Valore.Replace("<br>", "")
                End If

                If Not IsDBNull(dr.Descrizione) Then
                    dr.Descrizione = dr.Descrizione.Replace("<i>", "")
                    dr.Descrizione = dr.Descrizione.Replace("</i>", "")
                    dr.Descrizione = dr.Descrizione.Replace("<b>", "")
                    dr.Descrizione = dr.Descrizione.Replace("</b>", "")
                    dr.Descrizione = dr.Descrizione.Replace("<br>", "")
                End If


                objDs_CheckList.DT_CheckList.AddDT_CheckListRow(dr)


                DT_Codici.Dispose()
                DT_Codici = Nothing

            Next    ' ciclo su Codici

        Next        ' ciclo su Sezioni

        list_Sezioni.Clear()
        list_Sezioni = Nothing

        list_Disp.Clear()
        list_Disp = Nothing

    End Sub


    '##################################################################################################
    Private Sub Carica_Profilazione(ByRef objDs_Profilazione As DS_Profilazione)

        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        'Dim DLL_AD As New AccessoDB_Condizionalita.AccessoDati(objparametri_server)
        Dim Dt As DataTable
        Dim listaDomandeInterviste As List(Of AuditDomandeIntervisteModel)
        Dim DtProfilazioni As DataTable
        Dim strErr As String = String.Empty

        Dim IntervistaCod As Integer = 0

        Dim Dr As DS_Profilazione.DT_ProfilazioneRow

        'Se il profilo_cod|intervista_cod  mi è passato come parametro non lo cerco, altrimenti lo ricerco
        If IsNumeric(Qs_ProfiloCod) Then
            'Prendo quello che mi è stato passato come parametro
            IntervistaCod = CInt(Qs_ProfiloCod)

        Else

            Dim Data As Date = Now.Date
            If IsDate(Qs_AuditData) Then
                Data = CDate(Qs_AuditData)

                ' recupero il codice della profilazione da caricare
                Dim ObjAudit_Interviste As New AgronicaCoreAuditDAL.Audit_Interviste_R
                DtProfilazioni = ObjAudit_Interviste.Leggi(enum_AuditPuaTipo.Audit_GlobalGap,
                                                              CInt(Qs_RegolamentoCod),
                                                              0,
                                                              Qs_Piva,
                                                              Data,
                                                              Data,
                                                              "",
                                                              "",
                                                              objParametri_Server)

                If DtProfilazioni.Rows.Count > 0 Then
                    IntervistaCod = DtProfilazioni.Rows(0).Item("Intervista_Cod")
                End If

                ' dealloco la memoria
                If Not IsNothing(DtProfilazioni) Then
                    DtProfilazioni.Dispose()
                    DtProfilazioni = Nothing
                End If

            End If

        End If




        ' leggo le domande dell'intervista. se intervistacod<>0, legge anche le risposte
        'Dim ObjAudit_DomandeInterviste As New AgronicaCoreAuditDAL.Audit_Domande_Interviste_R
        'Dt = ObjAudit_DomandeInterviste.Leggi(enum_AuditPuaTipo.Audit_GlobalGap,
        'CInt(Qs_RegolamentoCod),
        '                                         IntervistaCod,
        '                                         "", "", objParametri_Server)

        Dim auditAgronica As New AuditAgronicaWS(objParametri_Server)
        listaDomandeInterviste = auditAgronica.LeggiDomandeInterviste(enum_AuditPuaTipo.Audit_GlobalGap, CInt(Qs_RegolamentoCod))

        Dim ObjAudit_RisposteInterviste As New AgronicaCoreAuditDAL.Audit_Risposte_Interviste_R


        For Each domandaIntervista In listaDomandeInterviste

            DT = ObjAudit_RisposteInterviste.LeggiRisposte(enum_AuditPuaTipo.Audit_GlobalGap,
                                                 CInt(Qs_RegolamentoCod),
                                                 IntervistaCod,
                                                 domandaIntervista.Domanda_Cod, "", "", objParametri_Server)

            ' creo la riga della sezione
            Dr = objDs_Profilazione.DT_Profilazione.NewDT_ProfilazioneRow

            Dr.Domanda_Cod = domandaIntervista.Domanda_Cod
            Dr.Domanda_Des = domandaIntervista.Domanda_Des.ToString.Replace("<b>", "").Replace("</b>", "")
            Dr.Domanda_Des = domandaIntervista.Domanda_Des.ToString.Replace("<i>", "").Replace("</i>", "")

            ' controllo se c'è la colonna valore, non c'è quando non c'è una profilazione attiva nella data
            If IntervistaCod <> 0 Then
                If Dt.Rows.Count > 0 AndAlso
                        Not IsDBNull(Dt.Rows(0).Item("Valore")) Then
                    If Dt.Rows(0).Item("Valore") = "1" Then
                        Dr.Valore = "þ"
                    Else
                        Dr.Valore = "¨"
                    End If
                Else
                    Dr.Valore = "¨"
                End If
            Else
                Dr.Valore = "¨"
            End If


            ' aggiungo la riga
            objDs_Profilazione.DT_Profilazione.AddDT_ProfilazioneRow(Dr)

        Next

        ' deallocazioni
        If Not IsNothing(Dt) Then
            Dt.Dispose()
            Dt = Nothing
        End If

        listaDomandeInterviste.Clear()
        listaDomandeInterviste = Nothing

        rptStampa.OpenSubreport("Rpt_Profilazione.rpt").SetDataSource(objDs_Profilazione)


    End Sub


    '##################################################################################################
    Private Sub Carica_Punteggio(ByRef objDs_Punteggio As DS_Punteggio, _
                                 ByRef strRisultato As String)

        'Dim DT_Disp As DataTable
        Dim DT_Punteggi As DataTable
        Dim DT_Risposte As DataTable
        Dim Dr As DS_Punteggio.DT_PunteggioRow
        Dim listaDisposizioni As List(Of AuditDisposizioniModel)

        Dim strErr As String = ""
        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        ' istanzio la dll dell'accesso ai dati
        ' Dim DLL_AD As New AccessoDB_Condizionalita.AccessoDati(objparametri_server)

        Dim i, j, k As Integer


        Dim Maggiori_TOT As Integer = 0
        Dim Maggiori_SI As Integer = 0
        Dim Minori_TOT As Integer = 0
        Dim Minori_SI As Integer = 0
        Dim Raccom_TOT As Integer = 0
        Dim Raccom_SI As Integer = 0

        Dim TOT_Maggiori_TOT As Integer = 0
        Dim TOT_Maggiori_SI As Integer = 0
        Dim TOT_Minori_TOT As Integer = 0
        Dim TOT_Minori_SI As Integer = 0
        Dim TOT_Raccom_TOT As Integer = 0
        Dim TOT_Raccom_SI As Integer = 0


        ' leggo le disposizioni del campo
        Dim auditAgronica As New AuditAgronicaWS(objParametri_Server)
        listaDisposizioni = auditAgronica.LeggiDisposizioni(enum_AuditPuaTipo.Audit_GlobalGap, CInt(Qs_RegolamentoCod), 0, 0)
        'Dim ObjAudit_Disposizioni As New AgronicaCoreAuditDAL.Audit_Disposizioni_R
        'DT_Disp = ObjAudit_Disposizioni.Leggi(enum_AuditPuaTipo.Audit_GlobalGap,
        '                                     CInt(Qs_RegolamentoCod),
        '                                     0,
        '                                     0, "", "", "", objParametri_Server)

        Dim codici = auditAgronica.LeggiCodici(enum_AuditPuaTipo.Audit_GlobalGap, Qs_RegolamentoCod, 0, 0, "")

        For Each disposizione In listaDisposizioni

            Dim vetParametri(5) As String

            vetParametri(0) = Session("ASG_SuperUser_CodFiscale")
            vetParametri(1) = Qs_Piva
            vetParametri(2) = CInt(disposizione.Disp_Cod)
            vetParametri(3) = #1/1/1900#
            vetParametri(4) = CStr(enum_AuditPuaTipo.Audit_GlobalGap)
            vetParametri(5) = CStr(Qs_RegolamentoCod)

            If Qs_AuditData <> #1/1/1900# Then
                vetParametri(3) = Qs_AuditData
            End If

            If IsNumeric(Qs_ProfiloCod) Then
                Array.Resize(vetParametri, 7)
                vetParametri(6) = CInt(Qs_ProfiloCod)
            End If

            'Dim DLL_Disposizioni As New Condizionalita.Disposizioni(objParametri_Server)

            ' controllo se la disposizione è attiva
            'If CallByName(DLL_Disposizioni, "Attivazione", CallType.Method, vetParametri) Then


            Dim ObjAudit_Risposte As New AgronicaCoreAuditDAL.Audit_Risposte_R

            DT_Risposte = ObjAudit_Risposte.Leggi(Qs_AuditCod, enum_AuditPuaTipo.Audit_GlobalGap,
                                                          Qs_RegolamentoCod,
                                                          CInt(disposizione.Disp_Cod),
                                                          "01/01/1900",
                                                          "31/12/2100",
                                                          "", "",
                                                          objParametri_Server)

            Maggiori_TOT = 0
            Maggiori_SI = 0
            Minori_TOT = 0
            Minori_SI = 0
            Raccom_TOT = 0
            Raccom_SI = 0

            'Audit_Risposte.Disp_Cod = Audit_Codici.Disp_Cod And Audit_Risposte.Punto_Numero = Audit_Codici.Punto_Numero And " &
            '              " Audit_Risposte.Regolamento_Cod = Audit_Codici.Regolamento_Cod AND Audit_Risposte.Audit_Tipo = Audit_Codici.Audit_Tipo

            For Each risposta In DT_Risposte.Rows
                Dim codice = codici.FirstOrDefault(Function(c) c.Punto_Numero = risposta.Item("Punto_Numero") AndAlso c.Disp_Cod = risposta.Item("Disp_Cod") AndAlso
                                                   c.Regolamento_Cod = risposta.Item("Regolamento_Cod") AndAlso c.Audit_Tipo = risposta.Item("Audit_Tipo"))
                If Not IsNothing(codice) Then
                    Select Case codice.Punteggio
                        Case 1
                            Raccom_TOT += 1
                            If risposta.Item("Valore") = "1" Then
                                Raccom_SI += 1
                            End If
                        Case 2
                            Minori_TOT += 1
                            If risposta.Item("Valore") = "1" Then
                                Minori_SI += 1
                            End If
                        Case 3
                            Maggiori_TOT += 1
                            If risposta.Item("Valore") = "1" Then
                                Maggiori_SI += 1
                            End If
                    End Select
                End If
            Next


            'For k = 0 To DT_Punteggi.Rows.Count - 1

            '    Select Case DT_Punteggi.Rows(k).Item("Punteggio")
            '        Case "Maggiori_TOT"
            '            Maggiori_TOT = DT_Punteggi.Rows(k).Item("nRighe")
            '        Case "Maggiori_SI"
            '            Maggiori_SI = DT_Punteggi.Rows(k).Item("nRighe")

            '        Case "Minori_TOT"
            '            Minori_TOT = DT_Punteggi.Rows(k).Item("nRighe")
            '        Case "Minori_SI"
            '            Minori_SI = DT_Punteggi.Rows(k).Item("nRighe")

            '        Case "Raccom_TOT"
            '            Raccom_TOT = DT_Punteggi.Rows(k).Item("nRighe")
            '        Case "Raccom_SI"
            '            Raccom_SI = DT_Punteggi.Rows(k).Item("nRighe")
            '    End Select


            'Next


            ' creo la riga della sezione
            Dr = objDs_Punteggio.DT_Punteggio.NewDT_PunteggioRow

            Dr.Modulo = disposizione.Disp_Nome
            Dr.Maggiori = CStr(Maggiori_SI) & "/" & CStr(Maggiori_TOT)
            Dr.Minori = CStr(Minori_SI) & "/" & CStr(Minori_TOT)
            Dr.Raccomandazioni = CStr(Raccom_SI) & "/" & CStr(Raccom_TOT)


            ' aggiungo la riga
            objDs_Punteggio.DT_Punteggio.AddDT_PunteggioRow(Dr)

            TOT_Maggiori_TOT += Maggiori_TOT
            TOT_Maggiori_SI += Maggiori_SI
            TOT_Minori_TOT += Minori_TOT
            TOT_Minori_SI += Minori_SI
            TOT_Raccom_TOT += Raccom_TOT
            TOT_Raccom_SI += Raccom_SI



            'End If

        Next    ' ciclo sulle disposizioni del campo di condizionalità

        ' creo la riga della sezione
        Dr = objDs_Punteggio.DT_Punteggio.NewDT_PunteggioRow

        Dr.Modulo = "TOTALE"
        Dr.Maggiori = CStr(TOT_Maggiori_SI) & "/" & CStr(TOT_Maggiori_TOT)
        Dr.Minori = CStr(TOT_Minori_SI) & "/" & CStr(TOT_Minori_TOT)
        Dr.Raccomandazioni = CStr(TOT_Raccom_SI) & "/" & CStr(TOT_Raccom_TOT)

        ' aggiungo la riga
        objDs_Punteggio.DT_Punteggio.AddDT_PunteggioRow(Dr)


        Dim percMaggiori As Double = If(Double.IsNaN(TOT_Maggiori_SI * 100 / TOT_Maggiori_TOT), 0, TOT_Maggiori_SI * 100 / TOT_Maggiori_TOT)
        Dim percMinori As Double = If(Double.IsNaN(TOT_Minori_SI * 100 / TOT_Minori_TOT), 0, TOT_Minori_SI * 100 / TOT_Minori_TOT)
        Dim percRaccom As Double = If(Double.IsNaN(TOT_Raccom_SI * 100 / TOT_Raccom_TOT), 0, TOT_Raccom_SI * 100 / TOT_Raccom_TOT)

        ' creo la riga della sezione
        Dr = objDs_Punteggio.DT_Punteggio.NewDT_PunteggioRow

        Dr.Modulo = "TOTALE"
        Dr.Maggiori = Format(percMaggiori, "0.00")
        Dr.Minori = Format(percMinori, "0.00")
        Dr.Raccomandazioni = Format(percRaccom, "0.00")

        ' aggiungo la riga
        objDs_Punteggio.DT_Punteggio.AddDT_PunteggioRow(Dr)


        Dim strSiNo As String = String.Empty

        If percMaggiori < 100 Or percMinori < 95 Then
            strSiNo = "NON"
        End If


        strRisultato = "L'azienda " & strSiNo & " risulta conforme e quindi " & strSiNo & " certificabile in quanto adempie al " & _
                      Format(percMaggiori, "0.00") & "% dei requisiti Maggiori e al " & _
                      Format(percMinori, "0.00") & "% dei requisiti Minori"



        ' Riga del punteggio totale

        'DLL_AD = Nothing

        rptStampa.OpenSubreport("Rpt_Punteggio.rpt").SetDataSource(objDs_Punteggio)

    End Sub

End Class