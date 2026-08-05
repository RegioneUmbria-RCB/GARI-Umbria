Imports System.IO
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports System.Web.Services
Imports Newtonsoft.Json
Imports AgronicaCoreEFatturaBIZ
Imports System.Collections.ObjectModel
Imports AgronicaCoreEFatturaDAL
Imports AgronicaCoreVarieDAL
Imports AgronicaCoreUtility

Public Class FattElettronica
    Inherits System.Web.UI.Page

#Region "Proprietà"
#End Region

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()
    End Sub

    Dim objParametri_Server, objParametri_Utenti, objParametri_Super_Server As AgronicaCoreParametri
    Public objparametri_server_string, objparametri_utenti_string As String
    Dim objParametriAgenda As ParametriAgenda

    Private Sub inizializzoObjParametri()
        objParametriAgenda = New ParametriAgenda
        '---
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objParametri_Super_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Super_Server"))

        objparametri_server_string = AgronicaCoreDataProvider.Utility.convertOBJparametritoString(objParametri_Server)
        '---
    End Sub


#Region "Caricamento"

    Private Sub caricaControlli()
    End Sub

#End Region

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        hdPiva.Value = Stringa_Decodifica(CStr(Request.QueryString("p")), AgroKey_EncoderDecoder, Server)

        inizializzoObjParametri()

        If objParametriAgenda.Piva <> "" Then
            hdPiva.Value = objParametriAgenda.Piva
        End If

        If Not IsPostBack Then
            inizializzoParametriPagina()
        End If

        'Controllo se l'utente ha i permessi per accedere
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim UtenteAbilitatoLettura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                            Session("ASG_Utente_Username"),
                                            Session("ASG_IdServizio"),
                                            enum_Security_Attivita.Contabilita_FattElettronica,
                                            enum_Security_Operazione.Lettura,
                                            Date.Now,
                                            "",
                                            objParametri_Utenti)

        Dim UtenteAbilitatoScrittura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                           Session("ASG_Utente_Username"),
                                           Session("ASG_IdServizio"),
                                           enum_Security_Attivita.Contabilita_FattElettronica,
                                           enum_Security_Operazione.Modifica,
                                           Date.Now,
                                           "",
                                           objParametri_Utenti)

        'Imposto le variabili di ponte con il client
        hf_UtenteAbilitatoLettura.Value = UtenteAbilitatoLettura
        hf_UtenteAbilitatoScrittura.Value = UtenteAbilitatoScrittura

        If Not UtenteAbilitatoLettura Then
            Response.Redirect("~/index.aspx")
        End If

        If Not UtenteAbilitatoScrittura Then

        End If

        If Not Page.IsPostBack Then
            caricaControlli()
        End If

        Master().Lbl_Titolo.Text = "FATTURAZIONE ELETTRONICA"

    End Sub

    Private Sub inizializzoParametriPagina()

        ' forza titolo pagina se non impostato
        If Master.flag_MenuBS_2017 AndAlso HttpContext.Current.Session.Item("ASG_MenuBS_2017") Is Nothing Then
            MenuBS_2017.salvaTitoloSezioneConGestioneRedirect(89)
        End If

    End Sub

    Public Shadows ReadOnly Property Master() As AgroAgenda_2010.AgendaBootstrap
        Get
            Return CType(MyBase.Master, AgroAgenda_2010.AgendaBootstrap)
        End Get
    End Property

#Region "script services Fatturazione Elettronica"

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiConfigurazione(ByVal piva As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim dataAttivazione = FattElettronica_DAL.LeggiDataAttivazione(piva, objParametri_Server)

            If dataAttivazione = AGRODATAFINE Then
                r.RispostaStringa = ""
            Else

                Dim risposta As New StringBuilder
                risposta.AppendLine("<div class='row'><div class='col-xs-12'>")

                ' fatturazione attiva
                Dim servizio = enum_Tipi_Servizi_Background.EFattura_Invio_XML_Attivi
                objParametri_Super_Server = FattElettronica_DAL.LeggiConnessioneAgroGSB(objParametri_Server, objParametri_Super_Server)
                Dim configurazione = FattElettronica_DAL.LeggiConfigurazioneServizio(piva, servizio, objParametri_Server, objParametri_Super_Server, False)

                If Not IsNothing(configurazione) Then
                    risposta.AppendLine("<div class='btn btn-warning' id='btn_genera' onclick='GenerazioneXML();'><i class='fa fa-download'></i>Genera XML</div>")
                    risposta.AppendLine("<div class='btn btn-danger' id='btn_invia' onclick='InvioXMLAttivi();'><i class='fa fa-cloud-upload'></i>Invia Fatture</div>")
                    risposta.AppendLine("<div class='btn btn-info' id='btn_esiti' onclick='VerificaEsiti();'><i class='fa fa-check'></i>Leggi Esiti</div>")
                Else
                    risposta.AppendLine("<div class='btn btn-warning' id='btn_genera' onclick='GenerazioneXML();'><i class='fa fa-download'></i>Genera XML</div>")
                    risposta.AppendLine("<div class='btn btn-danger' id='btn_esporta' onclick='EsportazioneXML();'><i class='fa fa-file-archive-o'></i>Esporta XML</div>")
                    risposta.AppendLine("<div class='btn btn-info' id='btn_rigenera' onclick='RigenerazioneXML();'><i class='fa fa-refresh'></i>Rigenera XML</div>")
                End If

                ' fatturazione passiva
                servizio = enum_Tipi_Servizi_Background.EFattura_Ricevi_XML_Passivi
                configurazione = FattElettronica_DAL.LeggiConfigurazioneServizio(piva, servizio, objParametri_Server, objParametri_Super_Server, False)

                If Not IsNothing(configurazione) Then
                    risposta.AppendLine("<div class='btn btn-success' id='btn_ricevi' onclick='RiceviXMLPassivi();'><i class='fa fa-cloud-download'></i>Ricevi Fatture</div>")
                    risposta.AppendLine("<div class='btn btn-info' id='btn_scarica' onclick='ScaricaXMLPassivi();'><i class='fa fa-file-archive-o'></i>Scarica Fatture</div>")
                End If

                risposta.AppendLine("</div></div>")

                r.RispostaStringa = risposta.ToString

            End If

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ElencoImprese() As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim listaImprese = FattElettronica_DAL.LeggiImprese(objParametri_Server)
            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            r.RispostaStringa = JsonConvert.SerializeObject(listaImprese, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ElencoFatture(ByVal piva As String, ByVal dataDal As String, ByVal dataAl As String, ByVal tipoDoc As String, ByVal filtroLav As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            If String.IsNullOrEmpty(piva) Then

                r.Errore = "Azienda non disponibile"
                r.RispostaOK = False

            Else

                Dim servizio = enum_Tipi_Servizi_Background.EFattura_Invio_XML_Attivi
                Dim configurazione = FattElettronica_DAL.LeggiConfigurazioneServizio(piva, servizio, objParametri_Server, objParametri_Super_Server)
                Dim listaFatture = FattElettronica_DAL.LeggiFatture(piva, CDate(dataDal), CDate(dataAl), tipoDoc, filtroLav, configurazione IsNot Nothing, objParametri_Server, objParametri_Utenti)
                Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
                r.RispostaStringa = JsonConvert.SerializeObject(listaFatture, Formatting.None, serializerSettings)
                r.RispostaOK = True

            End If

        Catch ex As Exception
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LogFattura(ByVal piva As String, ByVal idAgenda As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim listaFattureLog = FattElettronica_DAL.LeggiFattureLog(piva, idAgenda, objParametri_Server)
            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            r.RispostaStringa = JsonConvert.SerializeObject(listaFattureLog, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function FattElettronicaService(ByVal piva As String, ByVal servizio As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            If String.IsNullOrEmpty(piva) Then

                r.Errore = "Azienda non disponibile"
                r.RispostaOK = False

            Else

                Dim dataAttivazione = FattElettronica_DAL.LeggiDataAttivazione(piva, objParametri_Server)

                If dataAttivazione = AGRODATAFINE Then
                    r.Errore = "Servizio non disponibile"
                    r.RispostaOK = False
                Else

                    Dim configurazione = FattElettronica_DAL.LeggiConfigurazioneServizio(piva, servizio, objParametri_Server, objParametri_Super_Server)

                    If configurazione Is Nothing Then
                        r.Errore = "Configurazione servizio non disponibile"
                        r.RispostaOK = False
                    Else

                        Dim fattService As New FatturaElettronicaService(objParametri_Server,
                                                                         objParametri_Utenti,
                                                                         objParametri_Super_Server,
                                                                         configurazione)

                        Dim errori = fattService.Esegui()

                        If errori.Count > 0 Then
                            Dim risposta As String = "Procedura eseguita con errori:<br><br>"
                            For Each errore In errori
                                risposta &= errore & "<br>"
                            Next
                            r.RispostaStringa = risposta
                        Else
                            Select Case servizio
                                Case enum_Tipi_Servizi_Background.EFattura_Ricevi_XML_Passivi
                                    Dim ultimo_scarico = FattElettronica_DAL.LeggiUltimoScarico(piva, objParametri_Server)
                                    r.RispostaStringa = "Procedura eseguita correttamente" & ultimo_scarico
                                Case Else
                                    r.RispostaStringa = "Procedura eseguita correttamente"
                            End Select
                        End If

                        r.RispostaOK = True

                    End If

                End If

            End If

        Catch ex As Exception
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function GenerazioneXML(ByVal piva As String) As RispostaStandard
        Return FattElettronicaService(piva, enum_Tipi_Servizi_Background.EFattura_Generazione_XML)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function InvioXMLAttivi(ByVal piva As String) As RispostaStandard
        Return FattElettronicaService(piva, enum_Tipi_Servizi_Background.EFattura_Invio_XML_Attivi)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function VerificaEsiti(ByVal piva As String) As RispostaStandard
        Return FattElettronicaService(piva, enum_Tipi_Servizi_Background.EFattura_Verifica_Esiti)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function RiceviXMLPassivi(ByVal piva As String) As RispostaStandard
        Return FattElettronicaService(piva, enum_Tipi_Servizi_Background.EFattura_Ricevi_XML_Passivi)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function EsportazioneXML(ByVal piva As String, ByVal files As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            If String.IsNullOrEmpty(piva) Then

                r.Errore = "Azienda non disponibile"
                r.RispostaOK = False

            Else

                Dim servizio = enum_Tipi_Servizi_Background.EFattura_Generazione_XML

                Dim configurazione = FattElettronica_DAL.LeggiConfigurazioneServizio(piva, servizio, objParametri_Server, objParametri_Super_Server)
                If configurazione Is Nothing Then

                    r.Errore = "Configurazione servizio non disponibile"
                    r.RispostaOK = False

                Else

                    Dim fattService = New FatturaElettronicaService(configurazione)
                    Dim parametriExtra = fattService.ParametriExtra()

                    Dim fileManager As New FileManager(configurazione.DirectoryFileEsportazioni, configurazione.DirectoryLOG)
                    fileManager.Initialize()
                    Dim pathXML = fileManager.OttieniPercorso(FatturaElettronicaPath.XmlGenerati)

                    Dim pathZIP = fileManager.OttieniPercorso(FatturaElettronicaPath.ZipCicloAttivo)
                    If parametriExtra.PercorsoScaricoXml IsNot Nothing AndAlso
                            Not String.IsNullOrEmpty(parametriExtra.PercorsoScaricoXml) Then
                        pathZIP = parametriExtra.PercorsoScaricoXml
                    End If

                    Dim fileZip = Path.Combine(pathZIP, "ExportXML.zip")

                    Dim listaFiles As List(Of String) = JsonConvert.DeserializeObject(Of List(Of String))(files)

                    If listaFiles.Count > 0 Then

                        Dim listaFileXML As New List(Of String)
                        Dim listaLogSDI As New List(Of Integer)
                        For Each item In listaFiles
                            Dim items() As String = item.Split("|")
                            listaLogSDI.Add(CInt(items(0)))
                            listaFileXML.Add(Path.Combine(pathXML, items(1)))
                        Next

                        If parametriExtra.ZipXml Then

                            ' creazione file zip
                            Dim zipFullPath = fileManager.CreaFileZip(fileZip, listaFileXML)
                            If zipFullPath Is Nothing Then
                                r.RispostaStringa = "Problema durante la creazione del file ZIP"
                            Else
                                Dim objAllegato As New Varie.objAllegato
                                objAllegato.NomeFile = Path.GetFileName(zipFullPath)
                                objAllegato.Estensione = Path.GetExtension(zipFullPath).Substring(1)
                                objAllegato.File = My.Computer.FileSystem.ReadAllBytes(zipFullPath)

                                FattElettronica_DAL.AggiornaFattureLog(piva, listaLogSDI, StatoFattura_Gias.XMLEsportato, objParametri_Server)
                                r.RispostaStringa = "Procedura eseguita correttamente<br><br>File XML esportati: " & listaFileXML.Count
                                r.ParametroDue_stringa = JsonConvert.SerializeObject(objAllegato, Formatting.None)
                                r.ParametroDue = True
                            End If

                        Else

                            ' copia di ogni singolo file
                            For Each sourceFile As String In listaFileXML
                                FileSystemHelper.CopiaFile(sourceFile, pathZIP)
                            Next
                            FattElettronica_DAL.AggiornaFattureLog(piva, listaLogSDI, StatoFattura_Gias.XMLEsportato, objParametri_Server)
                            r.RispostaStringa = "Procedura eseguita correttamente<br><br>File XML esportati: " & listaFileXML.Count
                        End If

                    Else
                        r.RispostaStringa = "Nessun file XML da esportare"
                        r.ParametroDue = False
                    End If

                    r.RispostaOK = True

                End If

            End If

        Catch ex As Exception
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function RigenerazioneXML(ByVal piva As String, ByVal files As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            If String.IsNullOrEmpty(piva) Then

                r.Errore = "Azienda non disponibile"
                r.RispostaOK = False

            Else

                Dim servizio = enum_Tipi_Servizi_Background.EFattura_Generazione_XML
                Dim configurazione = FattElettronica_DAL.LeggiConfigurazioneServizio(piva, servizio, objParametri_Server, objParametri_Super_Server)

                If configurazione Is Nothing Then

                    r.Errore = "Configurazione servizio non disponibile"
                    r.RispostaOK = False

                Else

                    Dim listaFiles As List(Of String) = JsonConvert.DeserializeObject(Of List(Of String))(files)

                    If listaFiles.Count > 0 Then

                        Dim listaFileXML As New List(Of String)
                        Dim listaLogSDI As New List(Of Integer)
                        For Each item In listaFiles
                            Dim items() As String = item.Split("|")
                            listaLogSDI.Add(CInt(items(0)))
                            listaFileXML.Add(items(1))
                        Next

                        FattElettronica_DAL.AggiornaFattureLog(piva, listaLogSDI, StatoFattura_Gias.XMLDaRigenerare, objParametri_Server)
                        r.RispostaStringa = "Procedura eseguita correttamente<br><br>File XML da rigenerare: " & listaFileXML.Count
                    Else
                        r.RispostaStringa = "Nessun file XML da rigenerare"
                        r.ParametroDue = False
                    End If

                    r.RispostaOK = True

                End If

            End If

        Catch ex As Exception
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ScaricaXMLPassivi(ByVal piva As String, ByVal dataDal As String, ByVal dataAl As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            If String.IsNullOrEmpty(piva) Then

                r.Errore = "Azienda non disponibile"
                r.RispostaOK = False

            Else

                Dim servizio = enum_Tipi_Servizi_Background.EFattura_Ricevi_XML_Passivi
                Dim configurazione = FattElettronica_DAL.LeggiConfigurazioneServizio(piva, servizio, objParametri_Server, objParametri_Super_Server)

                If configurazione Is Nothing Then

                    r.Errore = "Configurazione servizio non disponibile"
                    r.RispostaOK = False

                Else

                    Dim fileManager As New FileManager(configurazione.DirectoryFileEsportazioni, configurazione.DirectoryLOG)
                    fileManager.Initialize()
                    Dim pathXML = fileManager.OttieniPercorso(FatturaElettronicaPath.XmlCicloPassivo)
                    Dim pathZip = fileManager.OttieniPercorso(FatturaElettronicaPath.ZipCicloPassivo)
                    Dim fileZip = Path.Combine(pathZip, "ExportXMLPassivi.zip")
                    Dim listaFileXML As New List(Of String)
                    Dim annoDal = CDate(dataDal).Year
                    Dim annoAl = CDate(dataAl).Year

                    If annoAl > annoDal Then
                        fileZip = Path.Combine(pathZip, "ExportXMLPassivi_" & annoDal & "_" & annoAl & ".zip")
                    Else
                        fileZip = Path.Combine(pathZip, "ExportXMLPassivi_" & annoAl & ".zip")
                    End If

                    For anno As Integer = annoDal To annoAl
                        Dim pathXMLanno = Path.Combine(pathXML, anno)
                        If Directory.Exists(pathXMLanno) Then
                            Dim dirInfo = New DirectoryInfo(pathXML)
                            Dim files_p7m = dirInfo.GetFiles("*.p7m", SearchOption.AllDirectories)
                            Dim files_xml = dirInfo.GetFiles("*.xml", SearchOption.AllDirectories)
                            Dim listaFiles = files_p7m.Concat(files_xml).ToList()
                            If listaFiles.Count > 0 Then
                                For Each item In listaFiles
                                    listaFileXML.Add(item.FullName)
                                Next
                            End If
                        End If
                    Next

                    If listaFileXML.Count > 0 Then
                        Dim zipFullPath = fileManager.CreaFileZip(fileZip, listaFileXML)

                        Dim objAllegato As New Varie.objAllegato
                        objAllegato.NomeFile = Path.GetFileName(zipFullPath)
                        objAllegato.Estensione = Path.GetExtension(zipFullPath).Substring(1)
                        objAllegato.File = My.Computer.FileSystem.ReadAllBytes(zipFullPath)

                        r.RispostaStringa = "Procedura eseguita correttamente<br><br>Num. documenti scaricati: " & listaFileXML.Count
                        r.ParametroDue_stringa = JsonConvert.SerializeObject(objAllegato, Formatting.None)
                        r.ParametroDue = True
                    Else
                        r.RispostaStringa = "Nessun file presente"
                        r.ParametroDue = False
                    End If

                    r.RispostaOK = True

                End If

            End If

        Catch ex As Exception
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r
    End Function

    Private Sub FattElettronica_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete

    End Sub

#End Region

End Class