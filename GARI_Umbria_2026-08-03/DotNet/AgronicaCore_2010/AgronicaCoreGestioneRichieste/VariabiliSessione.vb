
Imports System.Web
''' -----------------------------------------------------------------------------
''' Project	 : AgronicaCoreGestioneRichieste
''' Class	 : AgroWebConfig
''' 
''' -----------------------------------------------------------------------------
''' <summary>
''' RICORDA: quando si aggiunge una nuova chiave al webconfig ricordarsi di aggiungere all'interno della classe: 
''' **** La variabile privata
''' **** La corrispondente proprietà Pubblica
''' **** Aggiungere il parametro nella CARICA
''' **** Aggiungere il parametro nella LeggiXML 
''' **** Aggiungere il parametro nella GeneraXML
''' **** Modificare il costruttore che legge il Web config
''' </summary>
''' <remarks>
''' </remarks>
''' -----------------------------------------------------------------------------
Public Class VariabiliSessione

    Private _ASG_IdServizio As String
    Private _ASG_PathFileINI As String
    Private _ASG_Connessione_Server As String
    Private _ASG_Connessione_Utenti As String
    Private _ASG_Connessione_LOG As String
    Private _ASG_StringaConnessione_Server As String
    Private _ASG_StringaConnessione_Utenti As String
    Private _ASG_ProgressivoGIAS As String
    Private _ASG_Utente_Username As String
    Private _ASG_Utente_Password As String
    Private _ASG_Utente_CodFiscale As String
    Private _ASG_Utente_Username_Crypt As String
    Private _ASG_Utente_Password_Crypt As String
    Private _ASG_SuperUser_Username As String
    Private _ASG_SuperUser_Password As String
    Private _ASG_SuperUser_CodFiscale As String
    Private _ASG_SuperUser_Username_Crypt As String
    Private _ASG_SuperUser_Password_Crypt As String
    Private _ASG_FinestraTemporale_Inizio As String
    Private _ASG_FinestraTemporale_Fine As String
    Private _AgronicaCore_Flag_CancellazioneLogica As String
    Private _AgronicaCore_Flag_Visibilita As String
    Private _AgronicaCore_DirectoryLOG As String
    Private _AgronicaCore_NomeFileLOG As String
    Private _Collegamento_Fito As String
    Private _Collegamento_Dpi As String

    Private _ASG_Super_Server_PivaSuperUser As String
    Private _ASG_StringaConnessione_Super_Server As String
    Private _Sessione As Boolean

    '








    ''' <summary>
    ''' Crea l'oggetto a partire dalla sessione
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        _Sessione = True
        Carica()
    End Sub


    ''' <summary>
    ''' Crea l'oggetto senza usare la Sessione e passando obj AgronicaCoreParametri 
    ''' perchè appunto non può utilizzare la Sessione
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New(ByVal flagSessione As Boolean,
                   ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                   ByVal objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                   ByVal objParametri_SuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri)
        _Sessione = flagSessione
        Carica(objParametri_Server,
               objParametri_Utenti,
               objParametri_SuperServer)
    End Sub

    ''' <summary>
    ''' crea l'oggetto a partire dalla sessione
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub Carica()
        If _Sessione Then
            Dim objParametriAppoggioServer As AgronicaCoreDataProvider.AgronicaCoreParametri

            objParametriAppoggioServer = HttpContext.Current.Session("ASG_objParametri_Server")


            Me.Collegamento_Dpi = HttpContext.Current.Session("Collegamento_DPI")
            Me.Collegamento_Fito = HttpContext.Current.Session("Collegamento_Fito")
            Me.ASG_IdServizio = HttpContext.Current.Session("ASG_IdServizio")
            Me.ASG_PathFileINI = HttpContext.Current.Session("ASG_PathFileINI")
            Me.ASG_Connessione_Server = HttpContext.Current.Session("ASG_Connessione_Server")
            Me.ASG_Connessione_Utenti = HttpContext.Current.Session("ASG_Connessione_Utenti")
            Me.ASG_Connessione_LOG = HttpContext.Current.Session("ASG_Connessione_LOG")
            Me.ASG_StringaConnessione_Server = HttpContext.Current.Session("ASG_StringaConnessione_Server")
            Me.ASG_StringaConnessione_Utenti = HttpContext.Current.Session("ASG_StringaConnessione_Utenti")
            Me.ASG_ProgressivoGIAS = HttpContext.Current.Session("ASG_ProgressivoGIAS")
            Me.ASG_Utente_Username = HttpContext.Current.Session("ASG_Utente_Username")
            Me.ASG_Utente_Password = HttpContext.Current.Session("ASG_Utente_Password")
            Me.ASG_Utente_CodFiscale = HttpContext.Current.Session("ASG_Utente_CodFiscale")
            Me.ASG_Utente_Username_Crypt = HttpContext.Current.Session("ASG_Utente_Username_Crypt")
            Me.ASG_Utente_Password_Crypt = HttpContext.Current.Session("ASG_Utente_Password_Crypt")

            If Not objParametriAppoggioServer Is Nothing Then
                Me.ASG_SuperUser_Username = objParametriAppoggioServer.SuperUserUsername
            Else
                Me.ASG_SuperUser_Username = HttpContext.Current.Session("ASG_SuperUser_Username")
            End If

            Me.ASG_SuperUser_Password = HttpContext.Current.Session("ASG_SuperUser_Password")
            Me.ASG_SuperUser_CodFiscale = HttpContext.Current.Session("ASG_SuperUser_CodFiscale")
            Me.ASG_SuperUser_Username_Crypt = HttpContext.Current.Session("ASG_SuperUser_Username_Crypt")
            Me.ASG_SuperUser_Password_Crypt = HttpContext.Current.Session("ASG_SuperUser_Password_Crypt")
            Me.ASG_FinestraTemporale_Inizio = HttpContext.Current.Session("ASG_FinestraTemporale_Inizio")
            Me.ASG_FinestraTemporale_Fine = HttpContext.Current.Session("ASG_FinestraTemporale_Fine")
            Me.AgronicaCore_Flag_CancellazioneLogica = HttpContext.Current.Session("ASG_AgronicaCore_Flag_CancellazioneLogica")
            Me.AgronicaCore_Flag_Visibilita = HttpContext.Current.Session("ASG_AgronicaCore_Flag_Visibilita")
            Me.AgronicaCore_DirectoryLOG = HttpContext.Current.Session("ASG_AgronicaCore_DirectoryLOG")
            If Not IsNothing(objParametriAppoggioServer) Then
                Me.AgronicaCore_NomeFileLOG = objParametriAppoggioServer.LogFileName
            Else
                Me.AgronicaCore_NomeFileLOG = "AgronicaCoreLOG.txt"
            End If

            Me.ASG_Super_Server_PivaSuperUser = ""
            If Not IsNothing(HttpContext.Current.Session("ASG_Super_Server_PivaSuperUser")) Then
                Me.ASG_Super_Server_PivaSuperUser = HttpContext.Current.Session("ASG_Super_Server_PivaSuperUser")
            End If

            Me.ASG_StringaConnessione_Super_Server = ""
            If Not IsNothing(HttpContext.Current.Session("ASG_StringaConnessione_Super_Server")) Then
                Me.ASG_StringaConnessione_Super_Server = HttpContext.Current.Session("ASG_StringaConnessione_Super_Server")
            End If

        End If
    End Sub

    Private Sub Carica(ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                       ByVal objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                       ByVal objParametri_SuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri)
        If Not _Sessione Then
            'TODO_GIS: Da aggiungere i campi non presenti negli objParametri passati
            Me.Collegamento_Dpi = ""
            Me.Collegamento_Fito = ""
            Me.ASG_IdServizio = ""
            Me.ASG_PathFileINI = ""
            Me.ASG_Connessione_Server = ""
            Me.ASG_Connessione_Utenti = ""
            Me.ASG_Connessione_LOG = ""
            Me.ASG_StringaConnessione_Server = objParametri_Server.StringaConnessione
            Me.ASG_StringaConnessione_Utenti = objParametri_Utenti.StringaConnessione
            Me.ASG_ProgressivoGIAS = ""
            Me.ASG_Utente_Username = objParametri_Utenti.UtenteUsername
            Me.ASG_Utente_Password = ""
            Me.ASG_Utente_CodFiscale = objParametri_Utenti.UtenteCodFiscale
            Me.ASG_Utente_Username_Crypt = ""
            Me.ASG_Utente_Password_Crypt = ""
            Me.ASG_SuperUser_Username = objParametri_SuperServer.SuperUserUsername
            Me.ASG_SuperUser_Password = ""
            Me.ASG_SuperUser_CodFiscale = objParametri_SuperServer.PivaSuperUser
            Me.ASG_SuperUser_Username_Crypt = ""
            Me.ASG_SuperUser_Password_Crypt = ""
            Me.ASG_FinestraTemporale_Inizio = objParametri_Server.FinestraTemporaleInizio
            Me.ASG_FinestraTemporale_Fine = objParametri_Server.FinestraTemporaleFine
            Me.AgronicaCore_Flag_CancellazioneLogica = objParametri_Server.FlagCancellazioneLogica
            Me.AgronicaCore_Flag_Visibilita = objParametri_Server.FlagVisibilita
            Me.AgronicaCore_DirectoryLOG = objParametri_Server.LogDirectory

            If Not IsNothing(objParametri_Server) Then
                Me.AgronicaCore_NomeFileLOG = objParametri_Server.LogFileName
            Else
                Me.AgronicaCore_NomeFileLOG = "AgronicaCoreLOG.txt"
            End If

            Me.ASG_Super_Server_PivaSuperUser = objParametri_SuperServer.PivaSuperUser
            Me.ASG_StringaConnessione_Super_Server = objParametri_SuperServer.StringaConnessione
        Else
            Carica()
        End If
    End Sub

    Public Sub Salva()
        If _Sessione Then
            HttpContext.Current.Session("VariabiliSessione") = Me
        End If
    End Sub

    Public Sub Leggi()
        If _Sessione Then
            Dim obj As New VariabiliSessione
            obj = HttpContext.Current.Session("VariabiliSessione")

            If Not IsNothing(obj) Then
                _ASG_IdServizio = obj._ASG_IdServizio
                _ASG_PathFileINI = obj._ASG_PathFileINI
                _ASG_Connessione_Server = obj._ASG_Connessione_Server
                _ASG_Connessione_Utenti = obj._ASG_Connessione_Utenti
                _ASG_Connessione_LOG = obj._ASG_Connessione_LOG
                _ASG_StringaConnessione_Server = obj._ASG_StringaConnessione_Server
                _ASG_StringaConnessione_Utenti = obj._ASG_StringaConnessione_Utenti
                _ASG_ProgressivoGIAS = obj._ASG_ProgressivoGIAS
                _ASG_Utente_Username = obj._ASG_Utente_Username
                _ASG_Utente_Password = obj._ASG_Utente_Password
                _ASG_Utente_CodFiscale = obj._ASG_Utente_CodFiscale
                _ASG_Utente_Username_Crypt = obj._ASG_Utente_Username_Crypt
                _ASG_Utente_Password_Crypt = obj._ASG_Utente_Password_Crypt
                _ASG_SuperUser_Username = obj._ASG_SuperUser_Username
                _ASG_SuperUser_Password = obj._ASG_SuperUser_Password
                _ASG_SuperUser_CodFiscale = obj._ASG_SuperUser_CodFiscale
                _ASG_SuperUser_Username_Crypt = obj._ASG_SuperUser_Username_Crypt
                _ASG_SuperUser_Password_Crypt = obj._ASG_SuperUser_Password_Crypt
                _ASG_FinestraTemporale_Inizio = obj._ASG_FinestraTemporale_Inizio
                _ASG_FinestraTemporale_Fine = obj._ASG_FinestraTemporale_Fine
                _AgronicaCore_Flag_CancellazioneLogica = obj._AgronicaCore_Flag_CancellazioneLogica
                _AgronicaCore_Flag_Visibilita = obj._AgronicaCore_Flag_Visibilita
                _AgronicaCore_DirectoryLOG = obj._AgronicaCore_DirectoryLOG
                _AgronicaCore_NomeFileLOG = obj._AgronicaCore_NomeFileLOG
                _Collegamento_Fito = obj._Collegamento_Fito
                _Collegamento_Dpi = obj._Collegamento_Dpi
                _ASG_Super_Server_PivaSuperUser = obj._ASG_Super_Server_PivaSuperUser
                _ASG_StringaConnessione_Super_Server = obj._ASG_StringaConnessione_Super_Server

            End If
        End If
    End Sub

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Creo l'oggetto partendo dall'xml ricevuto
    ''' </summary>
    ''' <param name="XML">XML contenente il nodo AgroWebConfig</param>
    ''' <remarks>
    ''' </remarks>
    ''' -----------------------------------------------------------------------------
    Public Sub New(ByVal XML As String)
        _Sessione = True
        LeggiXML(XML)
        Carica()
    End Sub

    Private Sub LeggiXML(ByVal STR As String)
        Dim XmlDoc As New System.Xml.XmlDocument
        XmlDoc.LoadXml(STR)

        Dim XML_VariabiliSessione As System.Xml.XmlElement
        XML_VariabiliSessione = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("VariabiliSessione")


        'objSession("ASG_AgronicaCore_Flag_CancellazioneLogica")

        HttpContext.Current.Session("ASG_AgronicaCore_Flag_Visibilita") = XML_VariabiliSessione.GetAttribute(LCase("AgronicaCore_Flag_Visibilita"))
        HttpContext.Current.Session("ASG_AgronicaCore_Flag_CancellazioneLogica") = XML_VariabiliSessione.GetAttribute(LCase("AgronicaCore_Flag_CancellazioneLogica"))

        HttpContext.Current.Session("ASG_IdServizio") = XML_VariabiliSessione.GetAttribute("id_servizio")
        HttpContext.Current.Session("ASG_PathFileINI") = XML_VariabiliSessione.GetAttribute(LCase("PathFileINI"))
        HttpContext.Current.Session("ASG_Connessione_Server") = XML_VariabiliSessione.GetAttribute(LCase("Cn_Server"))
        HttpContext.Current.Session("ASG_Connessione_Utenti") = XML_VariabiliSessione.GetAttribute(LCase("Cn_Utenti"))
        HttpContext.Current.Session("ASG_Connessione_LOG") = XML_VariabiliSessione.GetAttribute(LCase("Cn_LogAccessi"))

        If XML_VariabiliSessione.HasAttribute(LCase("stringa_cn_server")) = False Then
            Try
                Dim objAgronicaCore As New AgronicaCoreDataProvider.DataProvider
                HttpContext.Current.Session("ASG_StringaConnessione_Server") = objAgronicaCore.FindConnessione_Su_Ini_O_Superserver(XML_VariabiliSessione.GetAttribute(LCase("PathFileINI")), XML_VariabiliSessione.GetAttribute(LCase("Cn_Server")))
                objAgronicaCore = Nothing
            Catch ex As Exception
                HttpContext.Current.Session("ASG_StringaConnessione_Server") = ""
            End Try
        Else
            If XML_VariabiliSessione.GetAttribute(LCase("stringa_cn_server")) = "" Then
                Try
                    Dim objAgronicaCore As New AgronicaCoreDataProvider.DataProvider
                    HttpContext.Current.Session("ASG_StringaConnessione_Server") = objAgronicaCore.FindConnessione_Su_Ini_O_Superserver(XML_VariabiliSessione.GetAttribute(LCase("PathFileINI")), XML_VariabiliSessione.GetAttribute(LCase("Cn_Server")))
                    objAgronicaCore = Nothing
                Catch ex As Exception
                    HttpContext.Current.Session("ASG_StringaConnessione_Server") = ""
                End Try
            Else
                HttpContext.Current.Session("ASG_StringaConnessione_Server") = XML_VariabiliSessione.GetAttribute(LCase("stringa_cn_server"))
            End If
        End If
        If XML_VariabiliSessione.HasAttribute(LCase("stringa_cn_utenti")) = False Then
            Try
                Dim objAgronicaCore As New AgronicaCoreDataProvider.DataProvider
                HttpContext.Current.Session("ASG_StringaConnessione_Utenti") = objAgronicaCore.FindConnessione_Su_Ini_O_Superserver(XML_VariabiliSessione.GetAttribute(LCase("PathFileINI")), XML_VariabiliSessione.GetAttribute(LCase("Cn_Utenti")))
                objAgronicaCore = Nothing
            Catch ex As Exception
                HttpContext.Current.Session("ASG_StringaConnessione_Utenti") = ""
            End Try
        Else
            If XML_VariabiliSessione.GetAttribute(LCase("stringa_cn_utenti")) = "" Then
                Try
                    Dim objAgronicaCore As New AgronicaCoreDataProvider.DataProvider
                    HttpContext.Current.Session("ASG_StringaConnessione_Utenti") = objAgronicaCore.FindConnessione_Su_Ini_O_Superserver(XML_VariabiliSessione.GetAttribute(LCase("PathFileINI")), XML_VariabiliSessione.GetAttribute(LCase("Cn_Utenti")))
                    objAgronicaCore = Nothing
                Catch ex As Exception
                    HttpContext.Current.Session("ASG_StringaConnessione_Utenti") = ""
                End Try
            Else
                HttpContext.Current.Session("ASG_StringaConnessione_Utenti") = XML_VariabiliSessione.GetAttribute(LCase("stringa_cn_utenti"))
            End If
        End If

        HttpContext.Current.Session("ASG_ProgressivoGIAS") = XML_VariabiliSessione.GetAttribute("progressivo_gias")

        HttpContext.Current.Session("ASG_Utente_Username") = XML_VariabiliSessione.GetAttribute("utente_usr")
        HttpContext.Current.Session("ASG_Utente_Password") = XML_VariabiliSessione.GetAttribute("utente_pwd")
        HttpContext.Current.Session("ASG_Utente_CodFiscale") = XML_VariabiliSessione.GetAttribute(LCase("Utente_CodFiscale"))
        HttpContext.Current.Session("ASG_Utente_Username_Crypt") = XML_VariabiliSessione.GetAttribute(LCase("Utente_Usr_Crypt"))
        HttpContext.Current.Session("ASG_Utente_Password_Crypt") = XML_VariabiliSessione.GetAttribute(LCase("Utente_Pwd_Crypt"))

        HttpContext.Current.Session("ASG_SuperUser_Username") = XML_VariabiliSessione.GetAttribute("superuser_usr")
        HttpContext.Current.Session("ASG_SuperUser_Password") = XML_VariabiliSessione.GetAttribute("superuser_pwd")
        HttpContext.Current.Session("ASG_SuperUser_CodFiscale") = XML_VariabiliSessione.GetAttribute("superuser_piva")
        HttpContext.Current.Session("ASG_SuperUser_Username_Crypt") = XML_VariabiliSessione.GetAttribute(LCase("SuperUser_Usr_Crypt"))
        HttpContext.Current.Session("ASG_SuperUser_Password_Crypt") = XML_VariabiliSessione.GetAttribute(LCase("SuperUser_Pwd_Crypt"))

        If XML_VariabiliSessione.HasAttribute(LCase("FinestraTemporale_Inizio")) = False Then
            HttpContext.Current.Session("ASG_FinestraTemporale_Inizio") = CDate("01/01/1900")
        Else
            If XML_VariabiliSessione.GetAttribute(LCase("FinestraTemporale_Inizio")) = "" Then
                HttpContext.Current.Session("ASG_FinestraTemporale_Inizio") = CDate("01/01/1900")
            Else
                HttpContext.Current.Session("ASG_FinestraTemporale_Inizio") = XML_VariabiliSessione.GetAttribute(LCase("FinestraTemporale_Inizio"))
            End If
        End If

        If XML_VariabiliSessione.HasAttribute(LCase("FinestraTemporale_Fine")) = False Then
            HttpContext.Current.Session("ASG_FinestraTemporale_Fine") = CDate("31/12/2100")
        Else
            If XML_VariabiliSessione.GetAttribute(LCase("FinestraTemporale_Fine")) = "" Then
                HttpContext.Current.Session("ASG_FinestraTemporale_Fine") = CDate("31/12/2100")
            Else
                HttpContext.Current.Session("ASG_FinestraTemporale_Fine") = XML_VariabiliSessione.GetAttribute(LCase("FinestraTemporale_Fine"))
            End If
        End If

        If XML_VariabiliSessione.HasAttribute(LCase("AgronicaCore_Flag_CancellazioneLogica")) = False Then
            HttpContext.Current.Session("AgronicaCore_Flag_CancellazioneLogica") = "0"
        Else
            If XML_VariabiliSessione.GetAttribute(LCase("AgronicaCore_Flag_CancellazioneLogica")) = "" Then
                HttpContext.Current.Session("AgronicaCore_Flag_CancellazioneLogica") = "0"
            Else
                HttpContext.Current.Session("AgronicaCore_Flag_CancellazioneLogica") = XML_VariabiliSessione.GetAttribute(LCase("AgronicaCore_Flag_CancellazioneLogica"))
            End If
        End If

        If XML_VariabiliSessione.HasAttribute(LCase("AgronicaCore_Flag_Visibilita")) = False Then
            HttpContext.Current.Session("AgronicaCore_Flag_Visibilita") = "1"
        Else
            If XML_VariabiliSessione.GetAttribute(LCase("AgronicaCore_Flag_Visibilita")) = "" Then
                HttpContext.Current.Session("AgronicaCore_Flag_Visibilita") = "1"
            Else
                HttpContext.Current.Session("AgronicaCore_Flag_Visibilita") = XML_VariabiliSessione.GetAttribute(LCase("AgronicaCore_Flag_Visibilita"))
            End If
        End If

        If XML_VariabiliSessione.HasAttribute(LCase("AgronicaCore_DirectoryLOG")) = False Then
            HttpContext.Current.Session("AgronicaCore_DirectoryLOG") = "C:\GIASLAN\Log"
        Else
            If XML_VariabiliSessione.GetAttribute(LCase("AgronicaCore_DirectoryLOG")) = "" Then
                HttpContext.Current.Session("AgronicaCore_DirectoryLOG") = "C:\GIASLAN\Log"
            Else
                HttpContext.Current.Session("AgronicaCore_DirectoryLOG") = XML_VariabiliSessione.GetAttribute(LCase("AgronicaCore_DirectoryLOG"))
            End If
        End If

        If XML_VariabiliSessione.HasAttribute(LCase("AgronicaCore_NomeFileLOG")) = False Then
            HttpContext.Current.Session("AgronicaCore_NomeFileLOG") = "AgronicaCore_LOG.txt"
        Else
            If XML_VariabiliSessione.GetAttribute(LCase("AgronicaCore_NomeFileLOG")) = "" Then
                HttpContext.Current.Session("AgronicaCore_NomeFileLOG") = "AgronicaCore_LOG.txt"
            Else
                HttpContext.Current.Session("AgronicaCore_NomeFileLOG") = XML_VariabiliSessione.GetAttribute(LCase("AgronicaCore_NomeFileLOG"))
            End If
        End If

        If XML_VariabiliSessione.HasAttribute(LCase("Collegamento_Fito")) = False Then
            HttpContext.Current.Session("Collegamento_Fito") = "True"
        Else
            If XML_VariabiliSessione.GetAttribute(LCase("Collegamento_Fito")) = "" Then
                HttpContext.Current.Session("Collegamento_Fito") = "True"
            Else
                HttpContext.Current.Session("Collegamento_Fito") = XML_VariabiliSessione.GetAttribute("collegamento_fito")
            End If
        End If

        If XML_VariabiliSessione.HasAttribute(LCase("Collegamento_DPI")) = False Then
            HttpContext.Current.Session("Collegamento_DPI") = "True"
        Else
            If XML_VariabiliSessione.GetAttribute(LCase("Collegamento_DPI")) = "" Then
                HttpContext.Current.Session("Collegamento_DPI") = "True"
            Else
                HttpContext.Current.Session("Collegamento_DPI") = XML_VariabiliSessione.GetAttribute("collegamento_dpi")
            End If
        End If



        If XML_VariabiliSessione.HasAttribute(LCase("ASG_Super_Server_PivaSuperUser")) = False Then
            HttpContext.Current.Session("ASG_Super_Server_PivaSuperUser") = ""
        Else
            HttpContext.Current.Session("ASG_Super_Server_PivaSuperUser") = XML_VariabiliSessione.GetAttribute(LCase("ASG_Super_Server_PivaSuperUser"))
        End If


        If XML_VariabiliSessione.HasAttribute(LCase("ASG_StringaConnessione_Super_Server")) = False Then
            HttpContext.Current.Session("ASG_StringaConnessione_Super_Server") = ""
        Else
            HttpContext.Current.Session("ASG_StringaConnessione_Super_Server") = XML_VariabiliSessione.GetAttribute(LCase("ASG_StringaConnessione_Super_Server"))
        End If




    End Sub


    ''' <summary>
    ''' Genera il nodo VariabiliSessione con tutti gli attributi della sessione
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GeneraXML() As String
        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlTxt As System.Xml.XmlElement

        XmlTxt = XmlDoc.CreateElement("VariabiliSessione")

        XmlTxt.SetAttribute(LCase("collegamento_dpi"), CStr(Collegamento_Dpi))
        XmlTxt.SetAttribute(LCase("collegamento_fito"), CStr(Collegamento_Fito))
        XmlTxt.SetAttribute(LCase("id_servizio"), CStr(ASG_IdServizio))
        XmlTxt.SetAttribute(LCase("PathFileINI"), CStr(ASG_PathFileINI))
        XmlTxt.SetAttribute(LCase("Cn_Server"), CStr(ASG_Connessione_Server))
        XmlTxt.SetAttribute(LCase("Cn_Utenti"), CStr(ASG_Connessione_Utenti))
        XmlTxt.SetAttribute(LCase("Cn_LogAccessi"), CStr(ASG_Connessione_LOG))
        XmlTxt.SetAttribute(LCase("stringa_cn_server"), CStr(ASG_StringaConnessione_Server))
        XmlTxt.SetAttribute(LCase("stringa_cn_utenti"), CStr(ASG_StringaConnessione_Utenti))
        XmlTxt.SetAttribute(LCase("progressivo_gias"), CStr(ASG_ProgressivoGIAS))
        XmlTxt.SetAttribute(LCase("utente_usr"), CStr(ASG_Utente_Username))
        XmlTxt.SetAttribute(LCase("utente_pwd"), CStr(ASG_Utente_Password))
        XmlTxt.SetAttribute(LCase("Utente_CodFiscale"), CStr(ASG_Utente_CodFiscale))
        XmlTxt.SetAttribute(LCase("Utente_Usr_Crypt"), CStr(ASG_Utente_Username_Crypt))
        XmlTxt.SetAttribute(LCase("Utente_Pwd_Crypt"), CStr(ASG_Utente_Password_Crypt))
        XmlTxt.SetAttribute(LCase("superuser_usr"), CStr(ASG_SuperUser_Username))
        XmlTxt.SetAttribute(LCase("superuser_pwd"), CStr(ASG_SuperUser_Password))
        XmlTxt.SetAttribute(LCase("superuser_piva"), CStr(ASG_SuperUser_CodFiscale))
        XmlTxt.SetAttribute(LCase("SuperUser_Usr_Crypt"), CStr(ASG_SuperUser_Username_Crypt))
        XmlTxt.SetAttribute(LCase("SuperUser_Pwd_Crypt"), CStr(ASG_SuperUser_Password_Crypt))
        XmlTxt.SetAttribute(LCase("FinestraTemporale_Inizio"), CStr(ASG_FinestraTemporale_Inizio))
        XmlTxt.SetAttribute(LCase("FinestraTemporale_Fine"), CStr(ASG_FinestraTemporale_Fine))
        XmlTxt.SetAttribute(LCase("AgronicaCore_Flag_CancellazioneLogica"), CStr(AgronicaCore_Flag_CancellazioneLogica))
        XmlTxt.SetAttribute(LCase("AgronicaCore_Flag_Visibilita"), CStr(AgronicaCore_Flag_Visibilita))
        XmlTxt.SetAttribute(LCase("AgronicaCore_DirectoryLOG"), CStr(AgronicaCore_DirectoryLOG))
        XmlTxt.SetAttribute(LCase("AgronicaCore_NomeFileLOG"), CStr(AgronicaCore_NomeFileLOG))

        XmlDoc.AppendChild(XmlTxt)
        Dim str As String
        str = XmlDoc.InnerXml
        Return str
    End Function
    Public Property AgronicaCore_DirectoryLOG() As String
        Get
            Return _AgronicaCore_DirectoryLOG
        End Get
        Set(ByVal value As String)
            _AgronicaCore_DirectoryLOG = value
        End Set
    End Property

    Public Property Collegamento_Fito() As String
        Get
            Return _Collegamento_Fito
        End Get
        Set(ByVal value As String)
            _Collegamento_Fito = value
        End Set
    End Property
    Public Property Collegamento_Dpi() As String
        Get
            Return _Collegamento_Dpi
        End Get
        Set(ByVal value As String)
            _Collegamento_Dpi = value
        End Set
    End Property

    Public Property AgronicaCore_Flag_CancellazioneLogica() As String
        Get
            Return _AgronicaCore_Flag_CancellazioneLogica
        End Get
        Set(ByVal value As String)
            _AgronicaCore_Flag_CancellazioneLogica = value
        End Set
    End Property
    Public Property AgronicaCore_Flag_Visibilita() As String
        Get
            Return _AgronicaCore_Flag_Visibilita
        End Get
        Set(ByVal value As String)
            _AgronicaCore_Flag_Visibilita = value
        End Set
    End Property
    Public Property AgronicaCore_NomeFileLOG() As String
        Get
            Return _AgronicaCore_NomeFileLOG
        End Get
        Set(ByVal value As String)
            _AgronicaCore_NomeFileLOG = value
        End Set
    End Property
    Public Property ASG_Connessione_LOG() As String
        Get
            Return _ASG_Connessione_LOG
        End Get
        Set(ByVal value As String)
            _ASG_Connessione_LOG = value
        End Set
    End Property
    Public Property ASG_Connessione_Server() As String
        Get
            Return _ASG_Connessione_Server
        End Get
        Set(ByVal value As String)
            _ASG_Connessione_Server = value
        End Set
    End Property
    Public Property ASG_Connessione_Utenti() As String
        Get
            Return _ASG_Connessione_Utenti
        End Get
        Set(ByVal value As String)
            _ASG_Connessione_Utenti = value
        End Set
    End Property
    Public Property ASG_FinestraTemporale_Fine() As String
        Get
            Return _ASG_FinestraTemporale_Fine
        End Get
        Set(ByVal value As String)
            _ASG_FinestraTemporale_Fine = value
        End Set
    End Property
    Public Property ASG_FinestraTemporale_Inizio() As String
        Get
            Return _ASG_FinestraTemporale_Inizio
        End Get
        Set(ByVal value As String)
            _ASG_FinestraTemporale_Inizio = value
        End Set
    End Property
    Public Property ASG_IdServizio() As String
        Get
            Return _ASG_IdServizio
        End Get
        Set(ByVal value As String)
            _ASG_IdServizio = value
        End Set
    End Property
    Public Property ASG_PathFileINI() As String
        Get
            Return _ASG_PathFileINI
        End Get
        Set(ByVal value As String)
            _ASG_PathFileINI = value
        End Set
    End Property
    Public Property ASG_ProgressivoGIAS() As String
        Get
            Return _ASG_ProgressivoGIAS
        End Get
        Set(ByVal value As String)
            _ASG_ProgressivoGIAS = value
        End Set
    End Property
    Public Property ASG_StringaConnessione_Server() As String
        Get
            Return _ASG_StringaConnessione_Server
        End Get
        Set(ByVal value As String)
            _ASG_StringaConnessione_Server = value
        End Set
    End Property
    Public Property ASG_StringaConnessione_Utenti() As String
        Get
            Return _ASG_StringaConnessione_Utenti
        End Get
        Set(ByVal value As String)
            _ASG_StringaConnessione_Utenti = value
        End Set
    End Property
    Public Property ASG_SuperUser_CodFiscale() As String
        Get
            Return _ASG_SuperUser_CodFiscale
        End Get
        Set(ByVal value As String)
            _ASG_SuperUser_CodFiscale = value
        End Set
    End Property
    Public Property ASG_SuperUser_Password() As String
        Get
            Return _ASG_SuperUser_Password
        End Get
        Set(ByVal value As String)
            _ASG_SuperUser_Password = value
        End Set
    End Property
    Public Property ASG_SuperUser_Password_Crypt() As String
        Get
            Return _ASG_SuperUser_Password_Crypt
        End Get
        Set(ByVal value As String)
            _ASG_SuperUser_Password_Crypt = value
        End Set
    End Property
    Public Property ASG_SuperUser_Username() As String
        Get
            Return _ASG_SuperUser_Username
        End Get
        Set(ByVal value As String)
            _ASG_SuperUser_Username = value
        End Set
    End Property
    Public Property ASG_SuperUser_Username_Crypt() As String
        Get
            Return _ASG_SuperUser_Username_Crypt
        End Get
        Set(ByVal value As String)
            _ASG_SuperUser_Username_Crypt = value
        End Set
    End Property
    Public Property ASG_Utente_CodFiscale() As String
        Get
            Return _ASG_Utente_CodFiscale
        End Get
        Set(ByVal value As String)
            _ASG_Utente_CodFiscale = value
        End Set
    End Property
    Public Property ASG_Utente_Password() As String
        Get
            Return _ASG_Utente_Password
        End Get
        Set(ByVal value As String)
            _ASG_Utente_Password = value
        End Set
    End Property
    Public Property ASG_Utente_Password_Crypt() As String
        Get
            Return _ASG_Utente_Password_Crypt
        End Get
        Set(ByVal value As String)
            _ASG_Utente_Password_Crypt = value
        End Set
    End Property
    Public Property ASG_Utente_Username() As String
        Get
            Return _ASG_Utente_Username
        End Get
        Set(ByVal value As String)
            _ASG_Utente_Username = value
        End Set
    End Property
    Public Property ASG_Utente_Username_Crypt() As String
        Get
            Return _ASG_Utente_Username_Crypt
        End Get
        Set(ByVal value As String)
            _ASG_Utente_Username_Crypt = value
        End Set
    End Property

    Public Property ASG_Super_Server_PivaSuperUser() As String
        Get
            Return _ASG_Super_Server_PivaSuperUser
        End Get
        Set(ByVal value As String)
            _ASG_Super_Server_PivaSuperUser = value
        End Set
    End Property

    Public Property ASG_StringaConnessione_Super_Server() As String
        Get
            Return _ASG_StringaConnessione_Super_Server
        End Get
        Set(ByVal value As String)
            _ASG_StringaConnessione_Super_Server = value
        End Set
    End Property

End Class
