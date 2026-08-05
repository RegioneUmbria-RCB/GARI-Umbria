Imports CrystalDecisions.Shared
Imports CrystalDecisions.CrystalReports.Engine
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreAuditBIZ

Partial Public Class GlobalGap_RappVerificaIspettiva
    Inherits System.Web.UI.Page

    Private rptStampa As Rpt_RappVerificaIspettiva

    '----- Gestione Querystring
    Dim Qs_Piva As String
    Dim Qs_Piva_SuperUser As String
    Dim Qs_AuditCod As String
    Dim Qs_AuditData As Date
    Dim Qs_RegolamentoCod As String
    Dim Qs_TipVar As String = "0"
    Dim Qs_TipVar_Spe As String = "0"

    'oggetto objparametri x server e utenti
    'Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri

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

        rptStampa = New Rpt_RappVerificaIspettiva

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

        'inizializzazione oggetti objParametri_Utenti e objParametri_Server
        'objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

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

        If Not IsNothing(Request.QueryString("gv")) Then

            Qs_TipVar = ObjAgronicaDataProvider.Stringa_Decodifica(Request.QueryString("gv").ToString,
                                                   ObjAgronicaDataProviderCostanti.AgroKey_EncoderDecoder,
                                                   Server)
        End If

        If Not IsNothing(Request.QueryString("gvf")) Then

            Qs_TipVar_Spe = ObjAgronicaDataProvider.Stringa_Decodifica(Request.QueryString("gvf").ToString,
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

            strPath = PathFileTemporanei & "\GlobalGapVerificaIspettiva_" & Session.SessionID.ToString & Date.Now.ToFileTimeUtc.ToString & ".pdf"

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

        '------------imposto la finestra temporale in modo che carichi solo i dati validi nella data di verifica--
        objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(Qs_AuditData, Qs_AuditData)




        'CType(rptStampa.Section2.ReportObjects("TextAuditCod"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "N°: " & Qs_AuditCod
        'CType(rptStampa.Section2.ReportObjects("TextData"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Data: " & Qs_AuditData


        'Dim DLL_AD As New AccessoDB_Condizionalita.AccessoDati(objparametri_server)

        Dim DataSet_VI As New DS_RappVerificaIspettiva

        'Dim DataSet_DT_RiepilogoRapportoDataTable As DS_RappVerificaIspettiva.DT_RiepilogoRapportoDataTable
        Dim DataSet_DT_RiepilogoRapportoRow As DS_RappVerificaIspettiva.DT_RiepilogoRapportoRow
        'una sola riga 
        DataSet_DT_RiepilogoRapportoRow = DataSet_VI.DT_RiepilogoRapporto.NewDT_RiepilogoRapportoRow

        Try
            Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig()
            'Dim bmpF As System.Drawing.Bitmap = New System.Drawing.Bitmap("C:\AgroSorgenti - Copia\AgronicaAudit\AgronicaGlobalGap\AB_Immagini\Logo\Logo_Agronica.bmp")
            If objWebConfig.Path_Directory_Loghi_Cliente <> "" Then
                Dim path As String = objWebConfig.Path_Directory_Loghi_Cliente & "Logo_Orizzontale_Standard.bmp"
                Dim bmpF As System.Drawing.Bitmap = New System.Drawing.Bitmap(path)
                ' metto l'immagine nella colonna blob
                Dim c As New System.Drawing.ImageConverter
                DataSet_DT_RiepilogoRapportoRow.Item("Blob_Logo") = c.ConvertTo(bmpF, GetType(Byte()))
            End If

        Catch ex As Exception

        End Try


        'Dim DataSet_NonConformitaDataTable As DS_RappVerificaIspettiva.NonConformitaDataTable
        Dim DataSet_NonConformitaRow As DS_RappVerificaIspettiva.NonConformitaRow
        'righe aggiunte dopo


        Dim AAImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
        Dim AAImpreseCodici As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        Dim AACentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
        Dim AARegImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim AADatatableUtility As New AgronicaCoreUtility.DatatableUtility
        Dim Validita_Inizio As Date = CDate("01/01/1900")
        Dim Validita_Fine As Date = CDate("31/12/2100")
        'Dim ObjAudit_Disposizioni As New AgronicaCoreAuditDAL.Audit_Disposizioni_R

        'leggo tutte le risposte
        Dim auditAgronica As New AuditAgronicaWS(objParametri_Server)
        Dim strErr As String = String.Empty
        Dim ObjAudit_Risposte As New AgronicaCoreAuditDAL.Audit_Risposte_R
        Dim codici = auditAgronica.LeggiCodici(enum_AuditPuaTipo.Audit_GlobalGap, Qs_RegolamentoCod, 0, 0, "")
        Dim Dt_risp As DataTable = ObjAudit_Risposte.AuditRisposte_Leggi_Punteggio_Senza_Codici(enum_AuditPuaTipo.Audit_GlobalGap,
                                                                                   Qs_RegolamentoCod, Qs_AuditCod, objParametri_Server.PivaSuperUser, 0, "", "", objParametri_Server)
        Dim temp As AuditCodiciModel

        For Each risp In Dt_risp.Rows

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

        'righe risp Non Conformi
        Dim DR_risp_NC As DataRow() = Dt_risp.Select(" Valore = '0' ")

            'righe risp Maggiori OK
            Dim DR_risp_Mag_C As DataRow() = Dt_risp.Select(" Punteggio = '3'   and Valore = '1' ")
        Dim N_risp_Mag_C As Integer = DR_risp_Mag_C.Length

        'righe risp Maggiori Non Conformi
        Dim DR_risp_Mag_NC As DataRow() = Dt_risp.Select(" Punteggio = '3'  and Valore = '0' ")
        Dim N_risp_Mag_NC As Integer = DR_risp_Mag_NC.Length

        Dim N_risp_Mag_CeNC As Integer = N_risp_Mag_C + N_risp_Mag_NC

        'righe risp Maggiori Non Applicabili
        Dim DR_risp_Mag_NA As DataRow() = Dt_risp.Select(" Punteggio = '3'  and Valore = '-1' ")
        Dim N_risp_Mag_NA As Integer = DR_risp_Mag_NA.Length

        'totale
        Dim N_risp_Mag As Integer = N_risp_Mag_C + N_risp_Mag_NC + N_risp_Mag_NA

        'righe risp Minori OK
        Dim DR_risp_Min_C As DataRow() = Dt_risp.Select(" Punteggio = '2'   and Valore = '1' ")
        Dim N_risp_Min_C As Integer = DR_risp_Min_C.Length

        'righe risp Minori Non Conformi
        Dim DR_risp_Min_NC As DataRow() = Dt_risp.Select(" Punteggio = '2'  and Valore = '0' ")
        Dim N_risp_Min_NC As Integer = DR_risp_Min_NC.Length

        Dim N_risp_Min_CeNC As Integer = N_risp_Min_C + N_risp_Min_NC

        'righe risp Maggiori Non Applicabili
        Dim DR_risp_Min_NA As DataRow() = Dt_risp.Select(" Punteggio = '2'  and Valore = '-1' ")
        Dim N_risp_Min_NA As Integer = DR_risp_Min_NA.Length

        'totale
        Dim N_risp_Min As Integer = N_risp_Min_C + N_risp_Min_NC + N_risp_Min_NA

        'righe risp Raccomandazioni OK
        Dim DR_risp_Rac_C As DataRow() = Dt_risp.Select(" Punteggio = '1'   and Valore = '1' ")
        Dim N_risp_Rac_C As Integer = DR_risp_Rac_C.Length

        'righe risp Raccomandazioni Non Conformi
        Dim DR_risp_Rac_NC As DataRow() = Dt_risp.Select(" Punteggio = '1'  and Valore = '0' ")
        Dim N_risp_Rac_NC As Integer = DR_risp_Rac_NC.Length

        Dim N_risp_Rac_CeNC As Integer = N_risp_Rac_C + N_risp_Rac_NC

        'righe risp Maggiori Non Applicabili
        Dim DR_risp_Rac_NA As DataRow() = Dt_risp.Select(" Punteggio = '1'  and Valore = '-1' ")
        Dim N_risp_Rac_NA As Integer = DR_risp_Rac_NA.Length

        'totale
        Dim N_risp_Rac As Integer = N_risp_Rac_C + N_risp_Rac_NC + N_risp_Rac_NA


        Dim i As Integer
        Dim tempList As List(Of AuditDisposizioniModel)
        Dim dispNome As String = ""
        For i = 0 To DR_risp_NC.Length - 1

            ' nel report ci vanno le Raccomandazioni (punteggio=1)
            'If DR_risp_NC(i).Item("Punteggio") <> 1 Then

            DataSet_NonConformitaRow = DataSet_VI.NonConformita.NewNonConformitaRow

            Dim ptoUno As String = CInt(Mid(DR_risp_NC(i).Item("Punto_Numero"), 1, 2))
            Dim ptoDue As String = CInt(Mid(DR_risp_NC(i).Item("Punto_Numero"), 3, 2))
            Dim ptoTre As String = CInt(Mid(DR_risp_NC(i).Item("Punto_Numero"), 5, 2))

            tempList = auditAgronica.LeggiDisposizioni(enum_AuditPuaTipo.Audit_GlobalGap, Qs_RegolamentoCod, 0, DR_risp_NC(i).Item("Disp_Cod"))
            dispNome = If(tempList IsNot Nothing AndAlso tempList.Count > 0, tempList(0).Disp_Nome, "")
            'ObjAudit_Disposizioni.DispNome_From_DispCod(enum_AuditPuaTipo.Audit_GlobalGap, Qs_RegolamentoCod, DR_risp_NC(i).Item("Disp_Cod"), objParametri_Server)

            DataSet_NonConformitaRow.cod_nc = dispNome & " " &
                                                           ptoUno &
                                                           IIf(ptoDue <> "0", "." & ptoDue, "") &
                                                           IIf(ptoTre <> "0", "." & ptoTre, "")

            Select Case DR_risp_NC(i).Item("Punteggio")
                Case 1
                    DataSet_NonConformitaRow.Valore_nc = "Raccomandaz."
                Case 2
                    DataSet_NonConformitaRow.Valore_nc = "Minore"
                Case 3
                    DataSet_NonConformitaRow.Valore_nc = "Maggiore"
            End Select

            DataSet_VI.NonConformita.AddNonConformitaRow(DataSet_NonConformitaRow)

            'End If

        Next


        'campo data verifica
        DataSet_DT_RiepilogoRapportoRow.Data_V = Qs_AuditData

        'campo AzAgricola
        DataSet_DT_RiepilogoRapportoRow.Az_Agricola = AAImprese.RagSoc_from_Piva(Qs_Piva, objParametri_Server)

        'campo Cod_GlobalGap
        DataSet_DT_RiepilogoRapportoRow.Cod_GlobalGap = Qs_AuditCod

        'valorizzo campo codice globalgap e codice GGN
        DataSet_DT_RiepilogoRapportoRow.Cod_GlobalGap2 = AAImpreseCodici.Leggi_Codice_from_Imprese_Codici(Qs_Piva, enum_CodiciAnagrafe.Codice_GlobalGap2, objParametri_Server)
        DataSet_DT_RiepilogoRapportoRow.Cod_GGN = AAImpreseCodici.Leggi_Codice_from_Imprese_Codici(Qs_Piva, enum_CodiciAnagrafe.Codice_GlobalGap, objParametri_Server)

        'Casadei 13/04/2026 aggiunta selezione della coltura relativa alla checklist
        Dim leggiAudit As New AgronicaCoreAuditDAL.Audit_R
        Dim dtAudit As DataTable = leggiAudit.Leggi(Qs_AuditCod, 4, Qs_RegolamentoCod,
                                                    Qs_Piva, Validita_Inizio, Validita_Fine,
                                                    "", "", objParametri_Server)

        'campo colture
        Dim FiltroAggiuntivo As String = ""
        Dim Veg_cod As Integer = 0
        If (dtAudit.Rows.Count > 0) Then
            If Not IsDBNull(dtAudit.Rows(0).Item("Riferimento_Cod")) AndAlso dtAudit.Rows(0).Item("Riferimento_Cod") <> "" Then
                FiltroAggiuntivo = " SpecieVegetali.Veg_Cod in (" & CStr(dtAudit.Rows(0).Item("Riferimento_Cod")).Split("_").Last & ") "
                Veg_cod = CInt(CStr(dtAudit.Rows(0).Item("Riferimento_Cod")).Split("_").Last)
            End If
        End If
        Dim DT_ImpiantiSpecie As DataTable

        'campo superficie coltivata
        Dim SAU_Totale As Double 'sup appezzamenti
        SAU_Totale = AARegImpianti.LeggiSuperfici(Qs_Piva, 0, 0, Veg_cod, 0, 0, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
        DataSet_DT_RiepilogoRapportoRow.Sup_Tot_Coltivata = SAU_Totale

        If Qs_TipVar = "1" Then

            DT_ImpiantiSpecie = AARegImpianti.Leggi_Distinct_Solo_SpecieGruppoVarietale(Qs_Piva, 0, 0, 0, Veg_cod, 0,
                                                                            FiltroAggiuntivo,
                                                                            " SpecieVegetali.veg_des, SpecieVegetali.veg_cod ,  GruppoVarietale.grva_des", objParametri_Server)

        Else

            DT_ImpiantiSpecie = AARegImpianti.Leggi_SpecieVarieta(Qs_Piva, 0, 0, 0, Veg_cod, 0, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                            FiltroAggiuntivo,
                                                                            "", objParametri_Server)


        End If


        Dim elencoColture As String = ""

        If Qs_TipVar = "1" Then

            If Not IsNothing(DT_ImpiantiSpecie) AndAlso DT_ImpiantiSpecie.Rows.Count > 0 Then
                i = 0
                Dim veg_cod_attuale = DT_ImpiantiSpecie.Rows(i).Item("veg_cod")

                elencoColture = DT_ImpiantiSpecie.Rows(i).Item("Veg_Des")
                If Not IsDBNull(DT_ImpiantiSpecie.Rows(i).Item("Grva_Des")) AndAlso DT_ImpiantiSpecie.Rows(i).Item("Grva_Cod") <> 0 Then

                    If Qs_TipVar_Spe <> "0" AndAlso DT_ImpiantiSpecie.Rows(i).Item("Veg_Cod") <> Qs_TipVar_Spe Then
                        elencoColture = DT_ImpiantiSpecie.Rows(i).Item("Veg_Des") & ""
                        elencoColture = elencoColture & " ("
                    Else
                        elencoColture = DT_ImpiantiSpecie.Rows(i).Item("Veg_Des") & ""
                        elencoColture = elencoColture & " (" & DT_ImpiantiSpecie.Rows(i).Item("Grva_Des") & ""
                    End If

                Else
                    elencoColture = DT_ImpiantiSpecie.Rows(i).Item("Veg_Des") & ""
                    elencoColture = elencoColture & " ("
                End If
                'elencoColture = elencoColture

                For i = 1 To DT_ImpiantiSpecie.Rows.Count - 1

                    If veg_cod_attuale = DT_ImpiantiSpecie.Rows(i).Item("veg_cod") Then

                        If Not IsDBNull(DT_ImpiantiSpecie.Rows(i).Item("Grva_Des")) AndAlso DT_ImpiantiSpecie.Rows(i).Item("Grva_Cod") <> 0 Then
                            If Qs_TipVar_Spe <> "0" AndAlso DT_ImpiantiSpecie.Rows(i).Item("Veg_Cod") <> Qs_TipVar_Spe Then
                                'elencoColture = elencoColture
                            Else
                                elencoColture = elencoColture & " , " & DT_ImpiantiSpecie.Rows(i).Item("Grva_Des")
                            End If
                        Else
                            'elencoColture = elencoColture
                        End If



                    Else

                        If Not IsDBNull(DT_ImpiantiSpecie.Rows(i).Item("Grva_Des")) AndAlso DT_ImpiantiSpecie.Rows(i).Item("Grva_Cod") <> 0 Then

                            If Qs_TipVar_Spe <> "0" AndAlso DT_ImpiantiSpecie.Rows(i).Item("Veg_Cod") <> Qs_TipVar_Spe Then
                                elencoColture = elencoColture & ") , " & DT_ImpiantiSpecie.Rows(i).Item("Veg_Des") & ""
                                elencoColture = elencoColture & " ("
                            Else
                                elencoColture = elencoColture & ") , " & DT_ImpiantiSpecie.Rows(i).Item("Veg_Des") & ""
                                elencoColture = elencoColture & " (" & DT_ImpiantiSpecie.Rows(i).Item("Grva_Des") & ""
                            End If

                        Else
                            elencoColture = elencoColture & ") , " & DT_ImpiantiSpecie.Rows(i).Item("Veg_Des") & ""
                            elencoColture = elencoColture & " ("
                        End If

                    End If

                    veg_cod_attuale = DT_ImpiantiSpecie.Rows(i).Item("veg_cod")

                Next
                elencoColture = elencoColture & ") ."
                elencoColture = elencoColture.Replace("()", "")
            Else
                elencoColture = "Nessuna Coltura"
            End If

        Else

            Dim DT_Specie As DataTable
            DT_Specie = AADatatableUtility.SelectDistinct("Specie", DT_ImpiantiSpecie, "Veg_Des", True)
            If Not IsNothing(DT_Specie) AndAlso DT_Specie.Rows.Count > 0 Then
                i = 0
                elencoColture = DT_Specie.Rows(i).Item("Veg_Des")
                For i = 1 To DT_Specie.Rows.Count - 1
                    elencoColture = elencoColture & " , " & DT_Specie.Rows(i).Item("Veg_Des")
                Next
                elencoColture = elencoColture & " ."
            Else
                elencoColture = "Nessuna Coltura"
            End If
        End If






        DataSet_DT_RiepilogoRapportoRow.Colture = elencoColture


        'campo centri
        Dim DT_Centri As DataTable = AACentri.LeggiPerImpianti_Coltivazioni(Qs_Piva, 0, 0, 0, 0, 0, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
        Dim elencoCentri As String = ""
        Dim indirizzo As String = ""
        If Not IsNothing(DT_Centri) AndAlso DT_Centri.Rows.Count > 0 Then
            i = 0
            Dim DtIndirizzo As DataTable = AACentri.Leggi(DT_Centri.Rows(i).Item("Piva"), _
                                                          DT_Centri.Rows(i).Item("sa_cod"), _
                                                          AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, _
                                                          "", "", objParametri_Server)
            indirizzo = DtIndirizzo.Rows(0).Item("ind_des") & " , " & _
                        DtIndirizzo.Rows(0).Item("com_des") & " , " & _
                        DtIndirizzo.Rows(0).Item("pro_cod") & ""
            elencoCentri = DT_Centri.Rows(i).Item("sa_nome") & " , " & indirizzo
            For i = 1 To DT_Centri.Rows.Count - 1
                DtIndirizzo = AACentri.Leggi(DT_Centri.Rows(i).Item("Piva"), _
                                              DT_Centri.Rows(i).Item("sa_cod"), _
                                              AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, _
                                              "", "", objParametri_Server)
                indirizzo = DtIndirizzo.Rows(0).Item("ind_des") & " , " & _
                            DtIndirizzo.Rows(0).Item("com_des") & " , " & _
                            DtIndirizzo.Rows(0).Item("pro_cod") & ""
                elencoCentri = elencoCentri & " ;  " & vbCrLf & DT_Centri.Rows(i).Item("sa_nome") & " , " & indirizzo
                elencoCentri = elencoCentri & " ."
            Next
        Else
            elencoCentri = "Nessun Centro"
        End If
        DataSet_DT_RiepilogoRapportoRow.CentriAziendali = elencoCentri

        'campo standard global
        'Dim objAudit_Regolamenti As New AgronicaCoreAuditDAL.Audit_Regolamenti_R

        'Dim dt_reg As DataTable = objAudit_Regolamenti.AuditRegolamenti_Leggi(enum_AuditPuaTipo.Audit_GlobalGap, Qs_RegolamentoCod, objParametri_Server)

        Dim regolamenti = auditAgronica.LeggiRegolamenti(enum_AuditPuaTipo.Audit_GlobalGap)
        DataSet_DT_RiepilogoRapportoRow.StandardGlobal = regolamenti.Where(Function(x) x.Regolamento_Cod = Qs_RegolamentoCod).Select(Function(x) x.Regolamento_Des).FirstOrDefault()

        'campo status produttore
        DataSet_DT_RiepilogoRapportoRow.Status_Produttore = "0"

        'campo status prodotto
        DataSet_DT_RiepilogoRapportoRow.Status_Prodotto = "0"

        'campi req maggiori
        Dim pma As Double = 100 * CDbl(N_risp_Mag_C / N_risp_Mag_CeNC)
        If Double.IsNaN(pma) Then
            pma = 0
        End If
        DataSet_DT_RiepilogoRapportoRow.ReqMaggiori = CStr(pma)

        DataSet_DT_RiepilogoRapportoRow.Mag_Tot_CNC = N_risp_Mag_CeNC
        DataSet_DT_RiepilogoRapportoRow.Mag_C = N_risp_Mag_C
        DataSet_DT_RiepilogoRapportoRow.Mag_NC = N_risp_Mag_NC
        DataSet_DT_RiepilogoRapportoRow.Mag_NA = N_risp_Mag_NA
        DataSet_DT_RiepilogoRapportoRow.Mag_Tot = N_risp_Mag

        'campi req minori
        Dim pmi As Double = 100 * CDbl(N_risp_Min_C / N_risp_Min_CeNC)
        If Double.IsNaN(pmi) Then
            pmi = 0
        End If
        DataSet_DT_RiepilogoRapportoRow.ReqMinori = CStr(pmi)

        DataSet_DT_RiepilogoRapportoRow.Min_Tot_CNC = N_risp_Min_CeNC
        DataSet_DT_RiepilogoRapportoRow.Min_C = N_risp_Min_C
        DataSet_DT_RiepilogoRapportoRow.Min_NC = N_risp_Min_NC
        DataSet_DT_RiepilogoRapportoRow.Min_NA = N_risp_Min_NA
        DataSet_DT_RiepilogoRapportoRow.Min_Tot = N_risp_Min

        'campi raccomandaz
        Dim pra As Double = 100 * CDbl(N_risp_Rac_C / N_risp_Rac_CeNC)
        If Double.IsNaN(pra) Then
            pra = 0
        End If
        DataSet_DT_RiepilogoRapportoRow.Raccomandazioni = CStr(pra)

        DataSet_DT_RiepilogoRapportoRow.Rac_Tot_CNC = N_risp_Rac_CeNC
        DataSet_DT_RiepilogoRapportoRow.Rac_C = N_risp_Rac_C
        DataSet_DT_RiepilogoRapportoRow.Rac_NC = N_risp_Rac_NC
        DataSet_DT_RiepilogoRapportoRow.Rac_NA = N_risp_Rac_NA
        DataSet_DT_RiepilogoRapportoRow.Rac_Tot = N_risp_Rac

        'campo esito verifica
        Dim conforme As Boolean = False
        DataSet_DT_RiepilogoRapportoRow.Esito = "NON CONFORME"
        If N_risp_Mag_NC = 0 AndAlso pmi >= 95 Then
            conforme = True
            DataSet_DT_RiepilogoRapportoRow.Esito = "CONFORME"
        End If


        'campo ispettore
        DataSet_DT_RiepilogoRapportoRow.Ispettore = "0"

        'campo produttore
        DataSet_DT_RiepilogoRapportoRow.Produttore = "0"




        DataSet_VI.DT_RiepilogoRapporto.AddDT_RiepilogoRapportoRow(DataSet_DT_RiepilogoRapportoRow)

        rptStampa.SetDataSource(DataSet_VI)

        '-----------------------------reimposto la finestra temporale con i valori corretti------------
        objParametri_Server.ResettaFinestra()

    End Sub




End Class