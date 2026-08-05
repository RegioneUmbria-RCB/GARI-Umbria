Imports System.IO
Imports System.Web.Services
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.Sicurezza
Imports System.Xml
Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreStampeDAL
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreVarieBIZ

Imports AgronicaCoreStampeDAL.FF_GestoreConfigStampa

Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.CrystalReports


Public Class StampaPassaporto
    Inherits System.Web.UI.Page


    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()

    End Sub


    Private Const LabelSeparator = "|"c
    Private Const LabelPrintSeparator = ","c


#Region "Script Services"

    <WebMethod(EnableSession:=True)> _
    Public Shared Function Stampanti(ByVal piva As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()


        Try

            'Inserire il codice QUI..


            r.RispostaOK = True
            r.RispostaStringa = Stampanti(piva, objParametri_Server)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & _
                       AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)



        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)> _
    Public Shared Function Lingue(ByVal piva As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()


        Try

            'Inserire il codice QUI..


            r.RispostaOK = True
            r.RispostaStringa = bindLingua2(objParametri_Server)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & _
                       AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)



        End Try

        Return r
    End Function


    <WebMethod(EnableSession:=True)> _
    Public Shared Function Anteprima_o_Stampa(ByVal ConfigurazioneDaStampare As String, ByVal piva As String, ByVal VivaiPassaporti_Operazione_cod As String, ByVal StampaDiretta As Boolean) As Etichette_RispostaStandard
        Dim r As New Etichette_RispostaStandard

        VivaiPassaporti_Operazione_cod = VivaiPassaporti_Operazione_cod.Replace("&#92;", "\")

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()


        Try

            'Inserire il codice QUI..
            r = ReportEPassa(ConfigurazioneDaStampare, piva, VivaiPassaporti_Operazione_cod, StampaDiretta, objParametri_Server)

            r.RispostaOK = True
            r.IsLink = True
            r.RispostaStringa = "Ok."

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & _
                       AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)



        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)> _
    Public Shared Function RegistroPassaportiLettura(ByVal piva As String, ByVal operazioniCod As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()


        Try

            Dim permessi As New PermessiUtente
            If Not permessi.getPermesso(TipiEnumerativi.enum_Security_Attivita.VivaiAttivitaAdempimenti).Scrittura Then
                Throw New Exception("Non si dispone dei permessi per operare sulle attività vivaistiche")
            End If

            Dim letturaRegistro As New AgronicaCoreContabDAL.VivaiPassaporti_Operazioni_R
            Dim dt As DataTable
            If operazioniCod = "-1" Then
                dt = letturaRegistro.LeggiPerStampaLibera("PP", "", "", objParametri_Server)
            Else
                dt = letturaRegistro.Leggi(piva, AGRODATAINIZIO, AGRODATAFINE, " op.ws_VivaiPassaporti_Operazione_cod in (" & operazioniCod & ")", "", objParametri_Server)
            End If



            r.RispostaOK = True
            r.RispostaStringa = AgronicaCoreContabBIZ.ws_VivaiPassaportiOperazioniBiz.CaricaGriglia_RegistroPassaporti_xJSON(dt, enum_PassaportiVivaiTipoGriglia.Stampa)

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                       AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r
    End Function



#End Region 'script services



    Public piva As String
    Public operazioniCod As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Me.Master.flag_MostraHeader = False
        Me.Master.flag_MostraFooter = False

        piva = Stringa_Decodifica(Request.QueryString("p"), AgroKey_EncoderDecoder, Server)
        hf_Piva.Value = piva
        operazioniCod = Stringa_Decodifica(Request.QueryString("OperazioneCod"), AgroKey_EncoderDecoder, Server)
        hf_Operazioni_Cod.Value = operazioniCod

    End Sub

    Private Shared Function getReport() As CrystalDecisions.CrystalReports.Engine.ReportDocument

        Dim pFile As String = HttpContext.Current.Server.MapPath("~/GestioneStampe/Magazzino/Vivaismo/")
        pFile = AgronicaCoreUtility.FileSystemHelper.AggiungiSlashSeNonEsiste(pFile)

        If My.Computer.FileSystem.FileExists(pFile & "PassaportoVivaismo.rpt") Then
            Dim crHlp As New CrystalHelper
            Return crHlp.getReportDaFile(pFile, "PassaportoVivaismo.rpt")
        End If


    End Function


    Private Shared Function ReportEPassa(ByVal ConfigurazioneDaStampare As String, ByVal piva As String, ByVal filtrooperazioni As String, ByVal StampaDiretta As Boolean, ByVal objParametri_server As AgronicaCoreParametri) As Etichette_RispostaStandard


        If Not StampaDiretta Then
            Return ReportEPassa_SingolaAnteprima(piva, filtrooperazioni, StampaDiretta, objParametri_server)
            Exit Function
        End If
        Dim RStandard As New Etichette_RispostaStandard

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(HttpContext.Current.Session("ASG_objParametri_Server")) Then
            RStandard.Sessione = False
            Return RStandard
        End If

        Dim ListaReportDaPassare As New List(Of ReportDaPassare)
        For Each copia In ConfigurazioneDaStampare.TrimEnd("|").Split("|")

            Dim numero_copie As Integer
            Dim lingua_cod As Integer
            Dim layout_cod As Integer
            Dim stampante_cod As Integer
            Dim Stampante_NomePerStampa As String

            getLayoutLinguaStampante(copia, layout_cod, stampante_cod, Stampante_NomePerStampa, lingua_cod, numero_copie, filtrooperazioni)

            Dim altezza, larghezza As Integer
            Dim rpt As ReportDocument
            rpt = ReportEPassa_StampaMassiva(
                piva,
                filtrooperazioni,
                "-1",
                "",
                layout_cod,
                lingua_cod,
                StampaDiretta,
                altezza,
                larghezza,
                numero_copie,
                objParametri_server
            )

            ListaReportDaPassare.Add( _
                New ReportDaPassare With { _
                        .Codice = AgronicaCoreUtility.FileSystemHelper.NomeFileUnivoco(".rpt"), _
                        .ilReportDaPassare = rpt, _
                        .Numero_Copie = numero_copie, _
                        .PrintName = Stampante_NomePerStampa, _
                        .Altezza = altezza, _
                        .Larghezza = larghezza _
                    }
                )


        Next

        HttpContext.Current.Session("ListaReportDaPassare") = ListaReportDaPassare

        RStandard.Lista_FF_Stampa_Dettagli_Cod = ""

        Dim lPageToOpen As String = GetLPageToOpen()

        RStandard.UrlLink = lPageToOpen

        Return RStandard

    End Function

    Private Shared Function ReportEPassa_StampaMassiva( _
        ByVal piva As String,
        ByVal FiltroOperazioni As String,
        ByVal InfoNumeroCopie As String, _
        ByVal Nome_Per_Stampa As String, _
        ByVal Layouyt_cod As Integer, _
        ByVal Lingua_Cod As Integer, _
        ByVal stampaDiretta As Boolean, _
        ByRef Altezza As Integer, _
        ByRef Larghezza As Integer, _
        ByVal numero_copie As Integer, _
        ByVal objParametri_server As AgronicaCoreParametri _
    ) As Engine.ReportDocument

        Dim rpt As New Engine.ReportDocument
        Dim coreLettura As New AgronicaCoreContabDAL.VivaiPassaporti_Operazioni_R
        Dim coreLetturaTraduzioni As New AgronicaCoreStampeDAL.Traduzioni_Stampe_R

        rpt = GetRpt(coreLettura, piva, FiltroOperazioni, objParametri_server, coreLetturaTraduzioni)

        Return rpt
    End Function

    Public Shared Sub getLayoutLinguaStampante(ByVal ConfigurazioneDaStampare As String, ByRef Layout_Cod As Integer, ByRef Stampante_cod As String, ByRef Stampante_Nome_PerStampa As String, ByRef lingua_cod As Integer, ByRef Numero_copie As Integer, ByRef filtrooperazioni As String)

        Dim mpp() As String = ConfigurazioneDaStampare.Split(LabelPrintSeparator)

        Layout_Cod = 0
        Stampante_cod = mpp(0)
        Stampante_Nome_PerStampa = mpp(1) 'TODO
        lingua_cod = mpp(2)
        Numero_copie = mpp(3)
        filtrooperazioni = mpp(4)

    End Sub

    Private Shared Function ReportEPassa_SingolaAnteprima(ByVal piva As String, ByVal filtroOperazioni As String, ByVal StampaDiretta As Boolean, ByVal ObjParametri_server As AgronicaCoreParametri) As Etichette_RispostaStandard

        Dim rpt As New Engine.ReportDocument
        Dim coreLettura As New AgronicaCoreContabDAL.VivaiPassaporti_Operazioni_R
        Dim coreLetturaTraduzioni As New AgronicaCoreStampeDAL.Traduzioni_Stampe_R

        rpt = GetRpt(coreLettura, piva, filtroOperazioni, ObjParametri_server, coreLetturaTraduzioni)

        HttpContext.Current.Session.Remove("ListaReportDaPassare")
        HttpContext.Current.Session("Report") = rpt


        Dim QS_PrintName As String
        Dim QS_Numero_Copie As Integer
        Dim QS_Str_Flag_Fascicola As String
        Dim QS_Start_Page As Integer
        Dim QS_End_Page As Integer

        Dim qS_cript As String


        If StampaDiretta Then

            QS_PrintName = "Stampante_NomePerStampa" 'TODO: impostare il nome per la stampa
            QS_Numero_Copie = 1
            QS_Start_Page = 0
            QS_End_Page = 0
            QS_Str_Flag_Fascicola = "False"

            qS_cript = _
                "&PrintServizioWindows=" & Stringa_Codifica("1", AgroKey_EncoderDecoder, HttpContext.Current.Server) & _
                "&QS_PrintName=" & Stringa_Codifica(QS_PrintName, AgroKey_EncoderDecoder, HttpContext.Current.Server) & _
                "&QS_Numero_Copie=" & Stringa_Codifica(QS_Numero_Copie, AgroKey_EncoderDecoder, HttpContext.Current.Server) & _
                "&QS_Start_Page=" & Stringa_Codifica(QS_Start_Page, AgroKey_EncoderDecoder, HttpContext.Current.Server) & _
                "&QS_End_Page=" & Stringa_Codifica(QS_End_Page, AgroKey_EncoderDecoder, HttpContext.Current.Server) & _
                "&QS_Str_Flag_Fascicola=" & Stringa_Codifica(QS_Str_Flag_Fascicola, AgroKey_EncoderDecoder, HttpContext.Current.Server)

        End If


        Dim lPageToOpen As String = GetLPageToOpen()

        Dim rval As New Etichette_RispostaStandard
        rval.UrlLink = lPageToOpen & qS_cript

        Return rval
    End Function

    Private Shared Function GetRpt(coreLettura As VivaiPassaporti_Operazioni_R, piva As String, filtroOperazioni As String, ObjParametri_server As AgronicaCoreParametri, coreLetturaTraduzioni As Traduzioni_Stampe_R) As ReportDocument
        Dim dtRpt As DataTable
        Dim rpt As ReportDocument
        Dim dtRptTraduzioni As DataTable

        dtRpt = coreLettura.Leggi(piva, AGRODATAINIZIO, AGRODATAFINE, " op.ws_VivaiPassaporti_Operazione_cod in (" & filtroOperazioni & ")", "", ObjParametri_server)
        Dim lng As String = "IT"
        If dtRpt.Rows.Count > 0 Then
            lng = dtRpt(0)("PaeseDiOrigine")
        End If

        Dim GestisciBarcode As New AgronicaCoreStampeDAL.FF_Etichette_R

        'ora hardcoded, poi si vedrà.
        Dim cfg As New FF_BarcodeType_Barcode
        cfg.SeparatoreCampo = vbCrLf
        cfg.SeparatoreChiaveVaore = "- "
        cfg.ListaMappaDescrizioni.Add(New FF_DB_Rimappatura With {.NomeCampoDt = "Rag_Soc_RUOP", .NomeCampoDescrizione = "ROUP"})
        cfg.ListaMappaDescrizioni.Add(New FF_DB_Rimappatura With {.NomeCampoDt = "SitoRagioneSociale", .NomeCampoDescrizione = "Sito"})
        cfg.ListaMappaDescrizioni.Add(New FF_DB_Rimappatura With {.NomeCampoDt = "SitoCodice", .NomeCampoDescrizione = "Codice Sito"})
        cfg.ListaMappaDescrizioni.Add(New FF_DB_Rimappatura With {.NomeCampoDt = "DDT_in_Numero", .NomeCampoDescrizione = "N. DDT"})
        cfg.ListaMappaDescrizioni.Add(New FF_DB_Rimappatura With {.NomeCampoDt = "DataMovimento", .NomeCampoDescrizione = "Data"})
        cfg.ListaMappaDescrizioni.Add(New FF_DB_Rimappatura With {.NomeCampoDt = "Origine", .NomeCampoDescrizione = "Origine"})
        cfg.ListaMappaDescrizioni.Add(New FF_DB_Rimappatura With {.NomeCampoDt = "Destinazione", .NomeCampoDescrizione = "Destinazione"})
        cfg.ListaMappaDescrizioni.Add(New FF_DB_Rimappatura With {.NomeCampoDt = "Calibro", .NomeCampoDescrizione = "Calibro"})
        cfg.ListaMappaDescrizioni.Add(New FF_DB_Rimappatura With {.NomeCampoDt = "Coltivatore_RagioneSociale", .NomeCampoDescrizione = "Azienda di Installazione"})
        GestisciBarcode.QRCodeCommom(dtRpt, "QRBlob", "", cfg)

        'Dim cfg1 As New FF_BarcodeType With {
        '    .Code128 = New List(Of FF_BarcodeType_Barcode)
        '}

        'cfg1.Code128.Add(New FF_BarcodeType_Barcode With {
        '    .CampoDa = "Lotto_1",
        '    .CampoA = "CodBin"
        '})

        'dtRpt.Columns.Add(New DataColumn("CodBin", GetType(System.String)))
        'GestisciBarcode.BarCodeCommonCode128("", "", "", "B", dtRpt, cfg1)

        dtRptTraduzioni = coreLetturaTraduzioni.Leggi(
            "", enum_CodificaStampe.PassaportoMaterialeVivaistico, lng, "1", -1, "", "", ObjParametri_server)

        rpt = getReport()



        rpt.SetDataSource(dtRpt)

        If dtRptTraduzioni.Rows.Count = 1 Then
            rpt.SetParameterValue("DicituraPassaportoFitosanitario", dtRptTraduzioni.Rows(0)("TxtReport_Descrizione"))
        Else
            rpt.SetParameterValue("DicituraPassaportoFitosanitario", "Passaporto delle Piante")
        End If
        Return rpt
    End Function


    Private Shared Function GetLPageToOpen() As String
        Dim vAppUrl As String() = HttpContext.Current.Request.Url.ToString.Split("/")
        'Dim vBase As String = vAppUrl(0) & "//" & vAppUrl(2) & "/" & vAppUrl(3)
        'Dim lPageToOpen As String = vBase & "/GestioneStampe/VisualizzatoreReport.aspx?anteprima=" & Stringa_Codifica("1", AgroKey_EncoderDecoder, HttpContext.Current.Server)
        Dim vBase As String = vAppUrl(0) & "//" & HttpContext.Current.Request.Url.Authority & HttpContext.Current.Request.ApplicationPath
        Dim lPageToOpen As String = vBase & "/GestioneStampe/VisualizzatoreReport.aspx?anteprima=" & Stringa_Codifica("1", AgroKey_EncoderDecoder, HttpContext.Current.Server)
        Return lPageToOpen
    End Function


#Region "Lookup caselle a discesa"


    Private Shared Function stampanti(piva As String, ByVal objParametri_server As AgronicaCoreParametri) As String
        Dim xLeggiP As New AgronicaCoreStampeDAL.FF_Stampanti_R
        Dim dtP As DataTable = xLeggiP.Leggi( _
            "", _
            "", _
            objParametri_server _
            )

        Return GetjSonFromDT(dtP, "FF_Stampanti_Cod,Nome_Per_Stampa")

    End Function
    Private Shared Function bindLingua2(ByVal objParametri_server As AgronicaCoreParametri) As String

        Dim dtRpt As DataTable
        Dim coreLettura As New AgronicaCoreStampeDAL.FF_Etichette_R


        dtRpt = _
            coreLettura.LeggiLinguEtichette("", " Lingua_Default Desc, LE1.Nome Asc  ", objParametri_server)

        Return GetjSonFromDT(dtRpt, "Lingua_Cod,Lingua_Des")

    End Function
#End Region

#Region "funzioni di utilità"

    Private Shared Function GetjSonFromDT(ByVal dt As DataTable, ByVal cfg As String) As String

        Dim rval As String = ""
        Dim listaObj As New List(Of String)
        Dim resultArray As New List(Of String)
        For Each dRow As DataRow In dt.Rows
            For Each singleCfg In cfg.Split(",")
                listaObj.Add("""" & singleCfg & """: """ & dRow(singleCfg) & """")
            Next

            resultArray.Add("{ " & String.Join(",", listaObj) & " }")

        Next


        rval = "[" & String.Join(",", resultArray) & "]"

        Return rval


    End Function



#End Region

#Region "Classe di appoggio"



    Public Class Etichette_RispostaStandard
        Inherits RispostaStandard

        Public IsLink As Boolean

        Public UrlLink As String
        Public Lista_FF_Stampa_Dettagli_Cod As String

    End Class




#End Region
End Class