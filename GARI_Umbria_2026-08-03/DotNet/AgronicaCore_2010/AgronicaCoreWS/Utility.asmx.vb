Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreContabObject
Imports System.IO
Imports System.Text
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider.DataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports Microsoft.Win32
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreXML.XML_Stampe
Imports AgronicaCoreDTOStd.InData.Utility
Imports InData.Utility
Imports AgronicaCoreModelsSTD.utente
Imports AgronicaCoreModelsSTD.exceptions

Public Class ModuliGias
    Public Modulo_Cantine As Boolean
    Public Modulo_FreshFood As Boolean
    Public Modulo_Tabacco As Boolean
    Public Modulo_Zoo As Boolean
End Class

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class UtilityWS
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function getProgressivoGias(ByRef objParametri_utenti As String) As Integer
        Dim aB As JObject = JsonConvert.DeserializeObject(objParametri_utenti)
        Dim objPUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri = aB.ToObject(Of AgronicaCoreDataProvider.AgronicaCoreParametri)()

        Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_CodiciGiasPRO_R
        Dim dt As DataTable = objUtenti.Leggi("", "", objPUtenti)
        Return dt.Rows(0).Item("ProgressivoGIAS")
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function getLinkProfitosan(ByVal objP_super_server As String,
                                        ByVal objP_server As String,
                                        ByVal objP_utenti As String) As rispostaStandard(Of String)

        Dim r As New rispostaStandard(Of String)

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig(objParametri_Super_Server, objParametri_Server, False)

            r.RispostaStringa = AgronicaCoreGestioneRichieste.profitosan.getLink2023(True, objWebConfig, "",
                                                                                                    objParametri_Server, objParametri_Utenti)
            r.RispostaOK = True
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r

    End Function

    '############################################################################ 

    'Metodi normali

    Public Function WarmUpEF(ByRef errore As String,
                             ByRef evenlog As EventLog,
                             ByVal stringaConnessione As String,
                             ByRef valoreRitorno As String,
                             Optional ByVal nomeDB As String = "",
                             Optional ByVal logEventi As Boolean = True) As Boolean
        Dim result = False

        Dim NomeRoutine As String = "WarmUpEF"
        Dim MessaggioErrore As String = ""
        Dim gefutils As New Gias_EF_Utility
        Dim GiasContext As Gias_DeveloperServer_Entities

        Try

            If Not String.IsNullOrEmpty(stringaConnessione) Then

                Dim EFConnString As String = gefutils.GetEntityConnectionString(stringaConnessione)

                GiasContext = New Gias_DeveloperServer_Entities(EFConnString)

                Dim ver = (From v In GiasContext.Versione_Database
                           Order By v.Data_Modifica Descending
                           Select v).FirstOrDefault

                valoreRitorno = "PRE=[" & ver.Username_Modifica & "-" & ver.Data_Modifica.ToString & "]"

                'ver.Username_Modifica = ver.Username_Modifica & " MOD=[" & nomeDB & "-" & Date.Now & "]"

                'GiasContext.SaveChanges()

                GiasContext = Nothing

                Return True
            Else
                Throw New ConfigurationErrorsException("La stringa di connessione per il Warm Up dell'Entity Framework è vuota. verificare il web.config del Core WS")
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            If Not IsNothing(ex.InnerException) Then
                MessaggioErrore = MessaggioErrore & " [" & ex.InnerException.Message & "]"
            End If
            errore &= "[" & NomeRoutine & "] : " & MessaggioErrore & vbCrLf
            LogEvent(evenlog, "[" & NomeRoutine & "] : " & MessaggioErrore, EventLogEntryType.Error, scriviLogEventi:=logEventi)
            GiasContext = Nothing
            Return False
        End Try

        Return result

    End Function

    Public Function GeneraListaStringheConnessione(ByRef errore As String,
                                                   ByRef eventLog As EventLog,
                                                   Optional ByVal logEventi As Boolean = True
                                                   ) As List(Of String)
        Const nomeRoutine = "GeneraListaStringheConnessione"
        Dim messaggioErrore As String = ""
        Dim objConnectionString As String = ""
        Dim objConnectionStringSuper As String = ""
        Dim listDB As New List(Of String)

        Try
            'recupero tutte le info dal web.config
            Dim provider As String = ConfigurationManager.AppSettings("Super_Server_Provider")
            Dim password As String = ConfigurationManager.AppSettings("Super_Server_Password")
            Dim userId As String = ConfigurationManager.AppSettings("Super_Server_UserId")
            Dim server As String = ConfigurationManager.AppSettings("Super_Server_Server")
            Dim initialCatalog As String = ConfigurationManager.AppSettings("Gias_DB")
            Dim superServerDB As String = ConfigurationManager.AppSettings("Super_Server_DB")

            If IsNothing(provider) OrElse String.IsNullOrEmpty(provider) Then
                Throw New ConfigurationErrorsException("Provider non specificato in web.config Core_WS")
            End If

            If IsNothing(password) OrElse String.IsNullOrEmpty(password) Then
                Throw New ConfigurationErrorsException("Password non specificata in web.config Core_WS")
            End If

            If IsNothing(userId) OrElse String.IsNullOrEmpty(userId) Then
                Throw New ConfigurationErrorsException("UserId non specificato in web.config Core_WS")
            End If

            If IsNothing(server) OrElse String.IsNullOrEmpty(server) Then
                Throw New ConfigurationErrorsException("Server non specificato in web.config Core_WS")
            End If

            If IsNothing(initialCatalog) OrElse String.IsNullOrEmpty(initialCatalog) Then
                'Throw New ConfigurationErrorsException("InitialCatalog non specificato in web.config Core_WS")

                'se non ho impostato un nome db, leggo da super server e carico tutto
                If IsNothing(superServerDB) OrElse String.IsNullOrEmpty(superServerDB) Then
                    Throw New ConfigurationErrorsException("InitialCatalog e Super_Server_DB non specificati in web.config Core_WS; impostare almeno uno dei due.")
                End If

                objConnectionStringSuper = "Provider=" & provider.ToString &
                    ";Server=" & server.ToString &
                    ";Initial Catalog=" & superServerDB &
                    ";User ID=" & userId.ToString &
                    ";Password=" & password.ToString &
                    ";Persist Security Info=True" &
                    ";Extended Properties=;"


                Dim objParametri_Super_Server As New AgronicaCoreParametri() With {.StringaConnessione = objConnectionStringSuper}


                Dim objConn As New Connessioni
                listDB = objConn.Recupera_Lista_StringheConnessione(ID_DB:=0,
                                                                    TipoDB:=TipiEnumerativi.enum_Tipo_DB.GIAS_SERVER,
                                                                    Server:=server.ToString,
                                                                    DB:="",
                                                                    Provider:="",
                                                                    UserId:="",
                                                                    Password:="",
                                                                    PivaSuperUser:="",
                                                                    Note:="",
                                                                    Progressivo:=0,
                                                                    Descrizione:="",
                                                                    xFiltroAggiuntivo:="",
                                                                    xOrderBy:=" DB ASC",
                                                                    objParametri:=objParametri_Super_Server)
            Else

                objConnectionString = "Provider=" & provider.ToString &
                    ";Server=" & server.ToString &
                    ";Initial Catalog=" & initialCatalog.ToString &
                    ";User ID=" & userId.ToString &
                    ";Password=" & password.ToString &
                    ";Persist Security Info=True" &
                    ";Extended Properties=;"

                listDB.Add(objConnectionString)

            End If



        Catch ex As Exception
            messaggioErrore = ex.Message
            If Not IsNothing(ex.InnerException) Then
                messaggioErrore = messaggioErrore & " [" & ex.InnerException.Message & "]"
            End If
            errore &= "[" & nomeRoutine & "] : " & messaggioErrore & vbCrLf
            LogEvent(eventLog, "[" & nomeRoutine & "] : " & messaggioErrore, EventLogEntryType.Error, scriviLogEventi:=logEventi)
            Return Nothing
        End Try

        Return listDB

    End Function


    Public Sub LogEvent(ByRef log As EventLog,
                        ByVal msg As String,
                        ByVal tipoMessaggio As EventLogEntryType,
                        Optional ByVal eventID As Integer = 0,
                        Optional ByVal scriviLogEventi As Boolean = False)

        If scriviLogEventi = True Then

            If log Is Nothing Then

                'If Not EventLog.SourceExists("Core-WS") Then
                '    EventLog.CreateEventSource("Core-WS", "Core-WS")
                'End If

                log = New EventLog With {.Source = "Core-WS",
                    .Log = "Core WS",
                    .EnableRaisingEvents = True}

                'log = CreateEventLogReg("Core-WS.SLog", "Core-WS")

            End If

            Try
                log.WriteEntry(msg, tipoMessaggio, eventID)
            Catch ex As Exception
                'questo è per impedire che il servizio di importazione si blocchi se il log degli eventi è pieno
            End Try

        End If

    End Sub


    Public Function CreateEventLogReg(ByVal eventLogName As String, ByVal sourceName As String) As EventLog

        'Dim eventLogName As String = "Hoksoft"
        'Dim sourceName As String = "MediaManager"
        Dim hoksoftLog As EventLog
        hoksoftLog = New EventLog With {
            .Log = eventLogName
        }

        ' set default event source (to be same as event log name) if not passed in
        If (sourceName Is Nothing) OrElse (sourceName.Trim().Length = 0) Then
            sourceName = eventLogName
        End If

        hoksoftLog.Source = sourceName

        ' Extra Raw event data can be added (later) if needed
        Dim rawEventData As Byte() = Encoding.ASCII.GetBytes("")

        ''' Check whether the Event Source exists. It is possible that this may
        ''' raise a security exception if the current process account doesn't
        ''' have permissions for all sub-keys under 
        ''' HKEY_LOCAL_MACHINE\System\CurrentControlSet\Services\EventLog

        ' Check whether registry key for source exists

        Dim keyName As String = Convert.ToString("SYSTEM\CurrentControlSet\Services\EventLog\") & eventLogName

        Dim rkEventSource As RegistryKey = Registry.LocalMachine.OpenSubKey(Convert.ToString(keyName & Convert.ToString("\")) & sourceName)

        ' Check whether key exists
        If rkEventSource Is Nothing Then
            ''' Key does not exist. Create key which represents source
            Registry.LocalMachine.CreateSubKey(Convert.ToString(keyName & Convert.ToString("\")) & sourceName)
        End If

        ''' Now validate that the .NET Event Message File, EventMessageFile.dll (which correctly
        ''' formats the content in a Log Message) is set for the event source
        Dim eventMessageFile As Object = rkEventSource.GetValue("EventMessageFile")

        ''' If the event Source Message File is not set, then set the Event Source message file.
        If eventMessageFile Is Nothing Then
            ''' Source Event File Doesn't exist - determine .NET framework location,
            ''' for Event Messages file.
            Dim dotNetFrameworkSettings As RegistryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\.NetFramework\")

            If dotNetFrameworkSettings IsNot Nothing Then

                Dim dotNetInstallRoot As Object = dotNetFrameworkSettings.GetValue("InstallRoot", Nothing, RegistryValueOptions.None)

                If dotNetInstallRoot IsNot Nothing Then
                    Dim eventMessageFileLocation As String = dotNetInstallRoot.ToString() & "v" & System.Environment.Version.Major.ToString() & "." & System.Environment.Version.Minor.ToString() & "." & System.Environment.Version.Build.ToString() & "\EventLogMessages.dll"

                    ''' Validate File exists
                    If System.IO.File.Exists(eventMessageFileLocation) Then
                        ''' The Event Message File exists in the anticipated location on the
                        ''' machine. Set this value for the new Event Source

                        ' Re-open the key as writable
                        rkEventSource = Registry.LocalMachine.OpenSubKey(Convert.ToString(keyName & Convert.ToString("\")) & sourceName, True)

                        ' Set the "EventMessageFile" property
                        rkEventSource.SetValue("EventMessageFile", eventMessageFileLocation, RegistryValueKind.[String])
                    End If
                End If
            End If

            dotNetFrameworkSettings.Close()
        End If

        rkEventSource.Close()

        ''' Log the message
        'hoksoftLog.WriteEntry(logMessage, Type, eventId, 0, rawEventData)

        Return hoksoftLog

    End Function


    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function warmUpEFGiasNG(ByVal objP_super_server As String,
                                 ByVal objP_server As String,
                                 ByVal objP_utenti As String) As rispostaStandard(Of String)
        Dim r As New rispostaStandard(Of String)

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)
            Dim errore As String = ""
            Dim eventLog As New EventLog
            Dim valoreRitorno As String = ""
            WarmUpEF(errore, eventLog, objParametri_Server.StringaConnessione, valoreRitorno)
            r.RispostaOK = True
            r.RispostaStringa = valoreRitorno

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function


    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function warmUpGiasNG_NG(InData As Object) As rispostaStandard(Of warmUpGiasNG_Response)

        Dim r As New rispostaStandard(Of warmUpGiasNG_Response)
        InData = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData))
        Dim unid As String = InData.InData

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If
        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_super_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If
        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect
            r.RispostaStringa = gestioneRedirect.warmUpGiasNG(objParametri_Super_Server, objParametri_Server, objParametri_Utenti, unid)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function warmUpGiasNG(ByVal objP_super_server As String,
                                 ByVal objP_server As String,
                                 ByVal objP_utenti As String,
                                 ByVal unid As String) As rispostaStandard(Of warmUpGiasNG_Response)
        Dim r As New rispostaStandard(Of warmUpGiasNG_Response)

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect
            r.RispostaStringa = gestioneRedirect.warmUpGiasNG(objParametri_Super_Server, objParametri_Server, objParametri_Utenti, unid)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function warmUpGiasNG_NG(ByVal InData As CoreWS_Generic(Of String)) As rispostaStandard(Of warmUpGiasNG_Response)
        Dim r As New rispostaStandard(Of warmUpGiasNG_Response)

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If InData.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect
            r.RispostaStringa = gestioneRedirect.warmUpGiasNG(objParametri_Super_Server, objParametri_Server, objParametri_Utenti, InData.InData)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function getModuli(InData As Object) As rispostaStandard(Of ModuliGias)
        Dim r As New rispostaStandard(Of ModuliGias)

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim iData As CoreWS_Generic(Of String) = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData), a)

        If iData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If iData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If iData.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If

        Dim m As New ModuliGias With {
            .Modulo_Cantine = False,
            .Modulo_FreshFood = False,
            .Modulo_Tabacco = False,
            .Modulo_Zoo = False
        }

        Try
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

            Dim elencoModuli = ""
            Dim leggiAnagrafeLog As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R
            Dim dtAnagrafeLog = leggiAnagrafeLog.Leggi(iData.InData, 0, 0, 0, 0, -1, -1, -1, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server)

            If Not dtAnagrafeLog Is Nothing AndAlso dtAnagrafeLog.Rows.Count > 0 Then
                For Each rAnagrafeLog In dtAnagrafeLog.Rows
                    Select Case CInt(rAnagrafeLog.Item("Modulo_Generazione"))
                        Case 1
                            m.Modulo_Cantine = True
                        Case 2
                            m.Modulo_FreshFood = True
                        Case 3
                            m.Modulo_Tabacco = True
                        Case 4

                        Case 5
                            m.Modulo_Zoo = True
                    End Select

                Next
            End If

            r.RispostaOK = True
            r.RispostaStringa = m

        Catch ex As Exception

            r.RispostaOK = False
            r.RispostaStringa = m
            'uso questa funzione per ottenere il Messaggio..:
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function


    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function getUtente(ByVal objP_super_server As String,
                                 ByVal objP_server As String,
                                 ByVal objP_utenti As String) As rispostaStandard(Of Utente)
        Dim r As New rispostaStandard(Of Utente)

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            '' Chiamo la verifica sulla validità dei permessi utente e della licenza gias (la stessa che viene chiamata nella 
            '' AgroMasterPage dell'Agenda 
            Dim auK As New AgronicaCoreUtentiDAL.AutenticaUtente
            Dim validitaUtentePermessiLicenza = auK.Verifica_Validita_Permessi_E_Chiave_Licenza(objParametri_Utenti)

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect
            Dim u As Utente = gestioneRedirect.Get_Utente_Permessi(objParametri_Utenti)
            If Not IsNothing(u) Then
                u.ValiditaUtentePermessiLicenza = validitaUtentePermessiLicenza
            End If

            r.RispostaStringa = u

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Get_Imprese_Impostazioni_NG(InData As CoreWS_Generic(Of Get_Imprese_Impostazioni)) As rispostaStandard(Of List(Of AgronicaCoreGestioneRichieste.Imprese_Impostazioni))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreGestioneRichieste.Imprese_Impostazioni))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If InData.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect
            r.RispostaStringa = gestioneRedirect.Get_Imprese_Impostazioni(InData.InData.Piva, InData.InData.Sa_Cod, objParametri_Server)

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r
    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Get_Imprese_Impostazioni(ByVal objP_super_server As String,
                                             ByVal objP_server As String,
                                             ByVal objP_utenti As String,
                                             ByVal Piva As String,
                                             ByVal Sa_Cod As Integer) As rispostaStandard(Of List(Of AgronicaCoreGestioneRichieste.Imprese_Impostazioni))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreGestioneRichieste.Imprese_Impostazioni))

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect
            r.RispostaStringa = gestioneRedirect.Get_Imprese_Impostazioni(Piva, Sa_Cod, objParametri_Server)

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function GestioneStampe_NG(InData As CoreWS_Generic(Of GestioneStampe)) As RispostaStandard
        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If InData.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
            HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti
            HttpContext.Current.Session("ASG_objParametri_Super_Server") = objParametri_Super_Server

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect

            Dim VariabiliInSessione As New VariabiliInSessione_NG
            VariabiliInSessione = gestioneRedirect.GetVariabiliInSessioneNG(InData.InData.VariabiliInSessione)

            Dim ParametriAggiuntivi As New List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString)
            ParametriAggiuntivi = GetParametriAggiuntiviNG(InData.InData.ParametriAggiuntivi)

            Dim objAgendaNG As New AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG
            objAgendaNG = GetParametriObjParametriAgendaNG(InData.InData.InData)

            r.RispostaStringa = gestioneRedirect.GestioneStampe(objParametri_Super_Server,
                                               objParametri_Server,
                                               objParametri_Utenti,
                                               InData.InData.report,
                                               VariabiliInSessione,
                                               objAgendaNG,
                                               ParametriAggiuntivi)
            r.RispostaOK = True


        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function GestioneStampe(ByVal objP_super_server As String,
                                        ByVal objP_server As String,
                                        ByVal objP_utenti As String,
                                        ByVal report As Integer,
                                        ByVal VariabiliInSessione As VariabiliInSessione_NG,
                                        ByVal InData As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG,
                                        ByVal ParametriAggiuntivi As List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString)) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
            HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti
            HttpContext.Current.Session("ASG_objParametri_Super_Server") = objParametri_Super_Server

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect

            r.RispostaStringa = gestioneRedirect.GestioneStampe(objParametri_Super_Server,
                                               objParametri_Server,
                                               objParametri_Utenti,
                                               report,
                                               VariabiliInSessione,
                                               InData,
                                               ParametriAggiuntivi)
            r.RispostaOK = True


        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function PassaggioSitoAgenda_NG(InData As CoreWS_Generic(Of PassaggioSitoAgenda)) As RispostaStandard
        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If InData.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        Try

            'AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG

            Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

            Dim iData As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG = JsonConvert.DeserializeObject(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG)(JsonConvert.SerializeObject(InData.InData.InData), a)

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
            HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti
            HttpContext.Current.Session("ASG_objParametri_Super_Server") = objParametri_Super_Server

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect

            Dim VariabiliInSessione As New VariabiliInSessione_NG
            VariabiliInSessione = gestioneRedirect.GetVariabiliInSessioneNG(InData.InData.VariabiliInSessione)

            Dim ParametriAggiuntivi As New List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString)
            ParametriAggiuntivi = GetParametriAggiuntiviNG(InData.InData.ParametriAggiuntivi)

            r.RispostaOK = True
            r.RispostaStringa = gestioneRedirect.PassaggioSitoAgenda(objParametri_Super_Server,
                                                                     objParametri_Server,
                                                                     objParametri_Utenti,
                                                                     VariabiliInSessione,
                                                                     iData,
                                                                     ParametriAggiuntivi,
                                                                     InData.InData.AggiungiSoloParametriAggiuntivi,
                                                                     InData.InData.IDSezione)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function PassaggioSitoAgenda(ByVal objP_super_server As String,
                                        ByVal objP_server As String,
                                        ByVal objP_utenti As String,
                                        ByVal VariabiliInSessione As VariabiliInSessione_NG,
                                        ByVal InData As Object,
                                        ByVal ParametriAggiuntivi As List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString),
                                        ByVal AggiungiSoloParametriAggiuntivi As Boolean,
                                        ByVal IDSezione As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        Try

            'AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG

            Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

            Dim iData As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG = JsonConvert.DeserializeObject(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG)(JsonConvert.SerializeObject(InData), a)

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
            HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti
            HttpContext.Current.Session("ASG_objParametri_Super_Server") = objParametri_Super_Server

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect

            r.RispostaOK = True
            r.RispostaStringa = gestioneRedirect.PassaggioSitoAgenda(objParametri_Super_Server,
                                                                     objParametri_Server,
                                                                     objParametri_Utenti,
                                                                     VariabiliInSessione,
                                                                     iData,
                                                                     ParametriAggiuntivi,
                                                                     AggiungiSoloParametriAggiuntivi,
                                                                     IDSezione)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function PassaggioSitoPianoConcimazione_NG(InData As CoreWS_Generic(Of PassaggioSitoPianoConcimazione)) As RispostaStandard
        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If InData.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        Try

            'AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG

            Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

            Dim iData As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG = JsonConvert.DeserializeObject(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG)(JsonConvert.SerializeObject(InData.InData.InData), a)

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
            HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti
            HttpContext.Current.Session("ASG_objParametri_Super_Server") = objParametri_Super_Server

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect

            Dim VariabiliInSessione As New VariabiliInSessione_NG
            VariabiliInSessione = gestioneRedirect.GetVariabiliInSessioneNG(InData.InData.VariabiliInSessione)

            Dim ParametriAggiuntivi As New List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString)
            ParametriAggiuntivi = GetParametriAggiuntiviNG(InData.InData.ParametriAggiuntivi)

            'Dim objAgendaNG As New AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG
            'objAgendaNG = GetParametriObjParametriAgendaNG(InData.InData.InData)


            r.RispostaOK = True
            r.RispostaStringa = gestioneRedirect.PassaggioSitoPianoConcimazione(objParametri_Super_Server,
                                                                     objParametri_Server,
                                                                     objParametri_Utenti,
                                                                     VariabiliInSessione,
                                                                     iData,
                                                                     ParametriAggiuntivi,
                                                                     InData.InData.AggiungiSoloParametriAggiuntivi,
                                                                     InData.InData.IDSezione)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function PassaggioSitoPianoConcimazione(ByVal objP_super_server As String,
                                        ByVal objP_server As String,
                                        ByVal objP_utenti As String,
                                        ByVal VariabiliInSessione As VariabiliInSessione_NG,
                                        ByVal InData As Object,
                                        ByVal ParametriAggiuntivi As List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString),
                                        ByVal AggiungiSoloParametriAggiuntivi As Boolean,
                                        ByVal IDSezione As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        Try

            'AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG

            Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

            Dim iData As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG = JsonConvert.DeserializeObject(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG)(JsonConvert.SerializeObject(InData), a)

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
            HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti
            HttpContext.Current.Session("ASG_objParametri_Super_Server") = objParametri_Super_Server

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect

            r.RispostaOK = True
            r.RispostaStringa = gestioneRedirect.PassaggioSitoPianoConcimazione(objParametri_Super_Server,
                                                                     objParametri_Server,
                                                                     objParametri_Utenti,
                                                                     VariabiliInSessione,
                                                                     iData,
                                                                     ParametriAggiuntivi,
                                                                     AggiungiSoloParametriAggiuntivi,
                                                                     IDSezione)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function PassaggioSitoSincronizzatore_NG(InData As CoreWS_Generic(Of InDataUtility)) As RispostaStandard
        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If InData.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
            HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti
            HttpContext.Current.Session("ASG_objParametri_Super_Server") = objParametri_Super_Server

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect

            Dim VariabiliInSessione As New VariabiliInSessione_NG
            VariabiliInSessione = gestioneRedirect.GetVariabiliInSessioneNG(InData.InData.VariabiliInSessione)

            Dim ParametriAggiuntivi As New List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString)
            ParametriAggiuntivi = GetParametriAggiuntiviNG(InData.InData.ParametriAggiuntivi)

            Dim objAgendaNG As New AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG
            objAgendaNG = GetParametriObjParametriAgendaNG(InData.InData.InData)


            r.RispostaOK = True
            r.RispostaStringa = gestioneRedirect.PassaggioSitoSincronizzatore(objParametri_Super_Server,
                                                                     objParametri_Server,
                                                                     objParametri_Utenti,
                                                                     VariabiliInSessione,
                                                                     objAgendaNG,
                                                                     ParametriAggiuntivi,
                                                                     InData.InData.AggiungiSoloParametriAggiuntivi,
                                                                     InData.InData.IDSezione)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function PassaggioSitoSincronizzatore(ByVal objP_super_server As String,
                                        ByVal objP_server As String,
                                        ByVal objP_utenti As String,
                                        ByVal VariabiliInSessione As VariabiliInSessione_NG,
                                        ByVal InData As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG,
                                        ByVal ParametriAggiuntivi As List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString),
                                        ByVal AggiungiSoloParametriAggiuntivi As Boolean,
                                        ByVal IDSezione As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
            HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti
            HttpContext.Current.Session("ASG_objParametri_Super_Server") = objParametri_Super_Server

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect

            r.RispostaOK = True
            r.RispostaStringa = gestioneRedirect.PassaggioSitoSincronizzatore(objParametri_Super_Server,
                                                                     objParametri_Server,
                                                                     objParametri_Utenti,
                                                                     VariabiliInSessione,
                                                                     InData,
                                                                     ParametriAggiuntivi,
                                                                     AggiungiSoloParametriAggiuntivi,
                                                                     IDSezione)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function PassaggioSitoAnalisi_NG(InData As CoreWS_Generic(Of InDataUtility)) As RispostaStandard
        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If InData.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
            HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti
            HttpContext.Current.Session("ASG_objParametri_Super_Server") = objParametri_Super_Server

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect

            Dim VariabiliInSessione As New VariabiliInSessione_NG
            VariabiliInSessione = gestioneRedirect.GetVariabiliInSessioneNG(InData.InData.VariabiliInSessione)

            Dim ParametriAggiuntivi As New List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString)
            ParametriAggiuntivi = GetParametriAggiuntiviNG(InData.InData.ParametriAggiuntivi)

            Dim objAgendaNG As New AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG
            objAgendaNG = GetParametriObjParametriAgendaNG(InData.InData.InData)

            r.RispostaOK = True
            r.RispostaStringa = gestioneRedirect.PassaggioSitoAnalisi(objParametri_Super_Server,
                                                                     objParametri_Server,
                                                                     objParametri_Utenti,
                                                                     VariabiliInSessione,
                                                                     objAgendaNG,
                                                                     ParametriAggiuntivi,
                                                                     InData.InData.AggiungiSoloParametriAggiuntivi, InData.InData.IDSezione)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function PassaggioSitoAnalisi(ByVal objP_super_server As String,
                                        ByVal objP_server As String,
                                        ByVal objP_utenti As String,
                                        ByVal VariabiliInSessione As VariabiliInSessione_NG,
                                        ByVal InData As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG,
                                        ByVal ParametriAggiuntivi As List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString),
                                        ByVal AggiungiSoloParametriAggiuntivi As Boolean,
                                        ByVal IDSezione As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
            HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti
            HttpContext.Current.Session("ASG_objParametri_Super_Server") = objParametri_Super_Server

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect

            r.RispostaOK = True
            r.RispostaStringa = gestioneRedirect.PassaggioSitoAnalisi(objParametri_Super_Server,
                                                                     objParametri_Server,
                                                                     objParametri_Utenti,
                                                                     VariabiliInSessione,
                                                                     InData,
                                                                     ParametriAggiuntivi,
                                                                     AggiungiSoloParametriAggiuntivi, IDSezione)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function PassaggioSitoPua_NG(InData As CoreWS_Generic(Of InDataUtility)) As RispostaStandard
        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If InData.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
            HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti
            HttpContext.Current.Session("ASG_objParametri_Super_Server") = objParametri_Super_Server

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect

            Dim VariabiliInSessione As New VariabiliInSessione_NG
            VariabiliInSessione = gestioneRedirect.GetVariabiliInSessioneNG(InData.InData.VariabiliInSessione)

            Dim ParametriAggiuntivi As New List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString)
            ParametriAggiuntivi = GetParametriAggiuntiviNG(InData.InData.ParametriAggiuntivi)

            Dim objAgendaNG As New AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG
            objAgendaNG = GetParametriObjParametriAgendaNG(InData.InData.InData)

            r.RispostaOK = True
            r.RispostaStringa = gestioneRedirect.PassaggioSitoPua(objParametri_Super_Server,
                                                                     objParametri_Server,
                                                                     objParametri_Utenti,
                                                                     VariabiliInSessione,
                                                                     objAgendaNG,
                                                                     ParametriAggiuntivi,
                                                                      InData.InData.AggiungiSoloParametriAggiuntivi)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function PassaggioSitoPua(ByVal objP_super_server As String,
                                        ByVal objP_server As String,
                                        ByVal objP_utenti As String,
                                        ByVal VariabiliInSessione As VariabiliInSessione_NG,
                                        ByVal InData As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG,
                                        ByVal ParametriAggiuntivi As List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString),
                                        ByVal AggiungiSoloParametriAggiuntivi As Boolean,
                                        ByVal IDSezione As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
            HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti
            HttpContext.Current.Session("ASG_objParametri_Super_Server") = objParametri_Super_Server

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect

            r.RispostaOK = True
            r.RispostaStringa = gestioneRedirect.PassaggioSitoPua(objParametri_Super_Server,
                                                                     objParametri_Server,
                                                                     objParametri_Utenti,
                                                                     VariabiliInSessione,
                                                                     InData,
                                                                     ParametriAggiuntivi,
                                                                     AggiungiSoloParametriAggiuntivi)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function PassaggioSitoPlanning_NG(InData As CoreWS_Generic(Of InDataUtility)) As RispostaStandard
        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If InData.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
            HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti
            HttpContext.Current.Session("ASG_objParametri_Super_Server") = objParametri_Super_Server

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect
            Dim VariabiliInSessione As New VariabiliInSessione_NG
            VariabiliInSessione = gestioneRedirect.GetVariabiliInSessioneNG(InData.InData.VariabiliInSessione)

            Dim ParametriAggiuntivi As New List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString)
            ParametriAggiuntivi = GetParametriAggiuntiviNG(InData.InData.ParametriAggiuntivi)

            Dim objAgendaNG As New AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG
            objAgendaNG = GetParametriObjParametriAgendaNG(InData.InData.InData)

            r.RispostaOK = True
            r.RispostaStringa = gestioneRedirect.PassaggioSitoPlanning(objParametri_Super_Server,
                                                                     objParametri_Server,
                                                                     objParametri_Utenti,
                                                                     VariabiliInSessione,
                                                                     objAgendaNG,
                                                                     ParametriAggiuntivi,
                                                                     InData.InData.AggiungiSoloParametriAggiuntivi,
                                                                     InData.InData.IDSezione)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function PassaggioSitoPlanning(ByVal objP_super_server As String,
                                        ByVal objP_server As String,
                                        ByVal objP_utenti As String,
                                        ByVal VariabiliInSessione As VariabiliInSessione_NG,
                                        ByVal InData As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG,
                                        ByVal ParametriAggiuntivi As List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString),
                                        ByVal AggiungiSoloParametriAggiuntivi As Boolean,
                                        ByVal IDSezione As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
            HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti
            HttpContext.Current.Session("ASG_objParametri_Super_Server") = objParametri_Super_Server

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect

            r.RispostaOK = True
            r.RispostaStringa = gestioneRedirect.PassaggioSitoPlanning(objParametri_Super_Server,
                                                                     objParametri_Server,
                                                                     objParametri_Utenti,
                                                                     VariabiliInSessione,
                                                                     InData,
                                                                     ParametriAggiuntivi,
                                                                     AggiungiSoloParametriAggiuntivi,
                                                                     IDSezione)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function PassaggioSitoGiasOnline_NG(InData As CoreWS_Generic(Of InDataUtility)) As RispostaStandard
        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If InData.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
            HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti
            HttpContext.Current.Session("ASG_objParametri_Super_Server") = objParametri_Super_Server

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect

            Dim VariabiliInSessione As New VariabiliInSessione_NG
            VariabiliInSessione = gestioneRedirect.GetVariabiliInSessioneNG(InData.InData.VariabiliInSessione)

            Dim ParametriAggiuntivi As New List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString)
            ParametriAggiuntivi = GetParametriAggiuntiviNG(InData.InData.ParametriAggiuntivi)

            Dim objAgendaNG As New AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG
            objAgendaNG = GetParametriObjParametriAgendaNG(InData.InData.InData)

            r.RispostaOK = True
            r.RispostaStringa = gestioneRedirect.PassaggioSitoGiasOnline(objParametri_Super_Server,
                                                                     objParametri_Server,
                                                                     objParametri_Utenti,
                                                                     VariabiliInSessione,
                                                                     objAgendaNG,
                                                                     ParametriAggiuntivi,
                                                                     InData.InData.AggiungiSoloParametriAggiuntivi)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function PassaggioSitoGiasOnline(ByVal objP_super_server As String,
                                        ByVal objP_server As String,
                                        ByVal objP_utenti As String,
                                        ByVal VariabiliInSessione As VariabiliInSessione_NG,
                                        ByVal InData As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG,
                                        ByVal ParametriAggiuntivi As List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString),
                                        ByVal AggiungiSoloParametriAggiuntivi As Boolean,
                                        ByVal IDSezione As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
            HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti
            HttpContext.Current.Session("ASG_objParametri_Super_Server") = objParametri_Super_Server

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect

            r.RispostaOK = True
            r.RispostaStringa = gestioneRedirect.PassaggioSitoGiasOnline(objParametri_Super_Server,
                                                                     objParametri_Server,
                                                                     objParametri_Utenti,
                                                                     VariabiliInSessione,
                                                                     InData,
                                                                     ParametriAggiuntivi,
                                                                     AggiungiSoloParametriAggiuntivi)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function PassaggioSitoStampe_2010_NG(InData As CoreWS_Generic(Of InDataUtility)) As RispostaStandard
        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If InData.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
            HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti
            HttpContext.Current.Session("ASG_objParametri_Super_Server") = objParametri_Super_Server

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect

            Dim VariabiliInSessione As New VariabiliInSessione_NG
            VariabiliInSessione = gestioneRedirect.GetVariabiliInSessioneNG(InData.InData.VariabiliInSessione)

            Dim ParametriAggiuntivi As New List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString)
            ParametriAggiuntivi = GetParametriAggiuntiviNG(InData.InData.ParametriAggiuntivi)

            Dim objAgendaNG As New AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG
            objAgendaNG = GetParametriObjParametriAgendaNG(InData.InData.InData)

            r.RispostaOK = True
            r.RispostaStringa = gestioneRedirect.PassaggioSitoStampe_2010(objParametri_Super_Server,
                                                                     objParametri_Server,
                                                                     objParametri_Utenti,
                                                                     VariabiliInSessione,
                                                                     objAgendaNG,
                                                                     ParametriAggiuntivi,
                                                                     InData.InData.AggiungiSoloParametriAggiuntivi, InData.InData.IDSezione)

        Catch ex As GiasException

            r.RispostaOK = False
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}
            r.Errore = ""
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function PassaggioSitoStampe_2010(ByVal objP_super_server As String,
                                        ByVal objP_server As String,
                                        ByVal objP_utenti As String,
                                        ByVal VariabiliInSessione As VariabiliInSessione_NG,
                                        ByVal InData As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG,
                                        ByVal ParametriAggiuntivi As List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString),
                                        ByVal AggiungiSoloParametriAggiuntivi As Boolean,
                                        ByVal IDSezione As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
            HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti
            HttpContext.Current.Session("ASG_objParametri_Super_Server") = objParametri_Super_Server

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect

            r.RispostaOK = True
            r.RispostaStringa = gestioneRedirect.PassaggioSitoStampe_2010(objParametri_Super_Server,
                                                                     objParametri_Server,
                                                                     objParametri_Utenti,
                                                                     VariabiliInSessione,
                                                                     InData,
                                                                     ParametriAggiuntivi,
                                                                     AggiungiSoloParametriAggiuntivi, IDSezione)
        Catch ex As GiasException

            r.RispostaOK = False
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}
            r.Errore = ""
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function PassaggioSitoProfilazione_NG(InData As CoreWS_Generic(Of InDataUtility)) As RispostaStandard
        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If InData.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
            HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti
            HttpContext.Current.Session("ASG_objParametri_Super_Server") = objParametri_Super_Server

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect

            Dim VariabiliInSessione As New VariabiliInSessione_NG
            VariabiliInSessione = gestioneRedirect.GetVariabiliInSessioneNG(InData.InData.VariabiliInSessione)

            Dim ParametriAggiuntivi As New List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString)
            ParametriAggiuntivi = GetParametriAggiuntiviNG(InData.InData.ParametriAggiuntivi)

            Dim objAgendaNG As New AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG
            objAgendaNG = GetParametriObjParametriAgendaNG(InData.InData.InData)

            r.RispostaOK = True
            r.RispostaStringa = gestioneRedirect.PassaggioSitoProfilazione(objParametri_Super_Server,
                                                                     objParametri_Server,
                                                                     objParametri_Utenti,
                                                                     VariabiliInSessione,
                                                                     objAgendaNG,
                                                                     ParametriAggiuntivi,
                                                                     InData.InData.AggiungiSoloParametriAggiuntivi)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function PassaggioSitoProfilazione(ByVal objP_super_server As String,
                                        ByVal objP_server As String,
                                        ByVal objP_utenti As String,
                                        ByVal VariabiliInSessione As VariabiliInSessione_NG,
                                        ByVal InData As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG,
                                        ByVal ParametriAggiuntivi As List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString),
                                        ByVal AggiungiSoloParametriAggiuntivi As Boolean,
                                        ByVal IDSezione As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
            HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti
            HttpContext.Current.Session("ASG_objParametri_Super_Server") = objParametri_Super_Server

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect

            r.RispostaOK = True
            r.RispostaStringa = gestioneRedirect.PassaggioSitoProfilazione(objParametri_Super_Server,
                                                                     objParametri_Server,
                                                                     objParametri_Utenti,
                                                                     VariabiliInSessione,
                                                                     InData,
                                                                     ParametriAggiuntivi,
                                                                     AggiungiSoloParametriAggiuntivi)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function PassaggioSitoAudit_NG(InData As CoreWS_Generic(Of InDataUtility)) As RispostaStandard
        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If InData.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
            HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti
            HttpContext.Current.Session("ASG_objParametri_Super_Server") = objParametri_Super_Server

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect

            Dim VariabiliInSessione As New VariabiliInSessione_NG
            VariabiliInSessione = gestioneRedirect.GetVariabiliInSessioneNG(InData.InData.VariabiliInSessione)

            Dim ParametriAggiuntivi As New List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString)
            ParametriAggiuntivi = GetParametriAggiuntiviNG(InData.InData.ParametriAggiuntivi)

            Dim objAgendaNG As New AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG
            objAgendaNG = GetParametriObjParametriAgendaNG(InData.InData.InData)

            r.RispostaOK = True
            r.RispostaStringa = gestioneRedirect.PassaggioSitoAudit(objParametri_Super_Server,
                                                                     objParametri_Server,
                                                                     objParametri_Utenti,
                                                                     VariabiliInSessione,
                                                                     objAgendaNG,
                                                                     ParametriAggiuntivi,
                                                                     InData.InData.AggiungiSoloParametriAggiuntivi, InData.InData.IDSezione)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function PassaggioSitoAgronicaCheckCOOP_NG(InData As CoreWS_Generic(Of InDataUtility)) As RispostaStandard
        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If InData.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
            HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti
            HttpContext.Current.Session("ASG_objParametri_Super_Server") = objParametri_Super_Server

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect

            Dim VariabiliInSessione As New VariabiliInSessione_NG
            VariabiliInSessione = gestioneRedirect.GetVariabiliInSessioneNG(InData.InData.VariabiliInSessione)

            Dim ParametriAggiuntivi As New List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString)
            ParametriAggiuntivi = GetParametriAggiuntiviNG(InData.InData.ParametriAggiuntivi)

            Dim objAgendaNG As New AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG
            objAgendaNG = GetParametriObjParametriAgendaNG(InData.InData.InData)

            r.RispostaOK = True
            r.RispostaStringa = gestioneRedirect.PassaggioSitoAgronicaAuditSicurezzaGlobalCoop(objParametri_Super_Server,
                                                                     objParametri_Server,
                                                                     objParametri_Utenti,
                                                                     VariabiliInSessione,
                                                                     objAgendaNG,
                                                                     ParametriAggiuntivi,
                                                                     InData.InData.AggiungiSoloParametriAggiuntivi, InData.InData.IDSezione)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function PassaggioSitoAgronicaLabQualita_NG(InData As CoreWS_Generic(Of InDataUtility)) As RispostaStandard
        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If InData.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
            HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti
            HttpContext.Current.Session("ASG_objParametri_Super_Server") = objParametri_Super_Server

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect

            Dim VariabiliInSessione As New VariabiliInSessione_NG
            VariabiliInSessione = gestioneRedirect.GetVariabiliInSessioneNG(InData.InData.VariabiliInSessione)

            Dim ParametriAggiuntivi As New List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString)
            ParametriAggiuntivi = GetParametriAggiuntiviNG(InData.InData.ParametriAggiuntivi)

            Dim objAgendaNG As New AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG
            objAgendaNG = GetParametriObjParametriAgendaNG(InData.InData.InData)

            r.RispostaOK = True
            r.RispostaStringa = gestioneRedirect.PassaggioSitoLabQualita(objParametri_Super_Server,
                                                                     objParametri_Server,
                                                                     objParametri_Utenti,
                                                                     VariabiliInSessione,
                                                                     objAgendaNG,
                                                                     ParametriAggiuntivi,
                                                                     InData.InData.AggiungiSoloParametriAggiuntivi, InData.InData.IDSezione)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function PassaggioSitoAudit(ByVal objP_super_server As String,
                                        ByVal objP_server As String,
                                        ByVal objP_utenti As String,
                                        ByVal VariabiliInSessione As VariabiliInSessione_NG,
                                        ByVal InData As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG,
                                        ByVal ParametriAggiuntivi As List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString),
                                        ByVal AggiungiSoloParametriAggiuntivi As Boolean,
                                        ByVal IDSezione As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
            HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti
            HttpContext.Current.Session("ASG_objParametri_Super_Server") = objParametri_Super_Server

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect

            r.RispostaOK = True
            r.RispostaStringa = gestioneRedirect.PassaggioSitoAudit(objParametri_Super_Server,
                                                                     objParametri_Server,
                                                                     objParametri_Utenti,
                                                                     VariabiliInSessione,
                                                                     InData,
                                                                     ParametriAggiuntivi,
                                                                     AggiungiSoloParametriAggiuntivi, IDSezione)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function PassaggioSitoPianiCampionamento_NG(InData As CoreWS_Generic(Of InDataUtility)) As RispostaStandard
        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If InData.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
            HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti
            HttpContext.Current.Session("ASG_objParametri_Super_Server") = objParametri_Super_Server

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect
            Dim VariabiliInSessione As New VariabiliInSessione_NG
            VariabiliInSessione = gestioneRedirect.GetVariabiliInSessioneNG(InData.InData.VariabiliInSessione)

            Dim ParametriAggiuntivi As New List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString)
            ParametriAggiuntivi = GetParametriAggiuntiviNG(InData.InData.ParametriAggiuntivi)

            Dim objAgendaNG As New AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG
            objAgendaNG = GetParametriObjParametriAgendaNG(InData.InData.InData)
            r.RispostaOK = True
            r.RispostaStringa = gestioneRedirect.PassaggioSitoPianiCampionamento(objParametri_Super_Server,
                                                                     objParametri_Server,
                                                                     objParametri_Utenti,
                                                                     VariabiliInSessione,
                                                                     objAgendaNG,
                                                                     ParametriAggiuntivi,
                                                                     InData.InData.AggiungiSoloParametriAggiuntivi,
                                                                     InData.InData.IDSezione)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function PassaggioSitoPianiCampionamento(ByVal objP_super_server As String,
                                        ByVal objP_server As String,
                                        ByVal objP_utenti As String,
                                        ByVal VariabiliInSessione As VariabiliInSessione_NG,
                                        ByVal InData As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG,
                                        ByVal ParametriAggiuntivi As List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString),
                                        ByVal AggiungiSoloParametriAggiuntivi As Boolean,
                                        ByVal IDSezione As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
            HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti
            HttpContext.Current.Session("ASG_objParametri_Super_Server") = objParametri_Super_Server

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect

            r.RispostaOK = True
            r.RispostaStringa = gestioneRedirect.PassaggioSitoPianiCampionamento(objParametri_Super_Server,
                                                                     objParametri_Server,
                                                                     objParametri_Utenti,
                                                                     VariabiliInSessione,
                                                                     InData,
                                                                     ParametriAggiuntivi,
                                                                     AggiungiSoloParametriAggiuntivi,
                                                                     IDSezione)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function PassaggioSitoPianiSemina_NG(InData As CoreWS_Generic(Of InDataUtility)) As RispostaStandard
        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If InData.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
            HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti
            HttpContext.Current.Session("ASG_objParametri_Super_Server") = objParametri_Super_Server

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect
            Dim VariabiliInSessione As New VariabiliInSessione_NG
            VariabiliInSessione = gestioneRedirect.GetVariabiliInSessioneNG(InData.InData.VariabiliInSessione)

            Dim ParametriAggiuntivi As New List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString)
            ParametriAggiuntivi = GetParametriAggiuntiviNG(InData.InData.ParametriAggiuntivi)

            Dim objAgendaNG As New AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG
            objAgendaNG = GetParametriObjParametriAgendaNG(InData.InData.InData)


            r.RispostaOK = True
            r.RispostaStringa = gestioneRedirect.PassaggioSitoPianiSemina(objParametri_Super_Server,
                                                                     objParametri_Server,
                                                                     objParametri_Utenti,
                                                                     VariabiliInSessione,
                                                                     objAgendaNG,
                                                                     ParametriAggiuntivi,
                                                                     InData.InData.AggiungiSoloParametriAggiuntivi)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function PassaggioSitoPianiSemina(ByVal objP_super_server As String,
                                        ByVal objP_server As String,
                                        ByVal objP_utenti As String,
                                        ByVal VariabiliInSessione As VariabiliInSessione_NG,
                                        ByVal InData As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG,
                                        ByVal ParametriAggiuntivi As List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString),
                                        ByVal AggiungiSoloParametriAggiuntivi As Boolean,
                                        ByVal IDSezione As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
            HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti
            HttpContext.Current.Session("ASG_objParametri_Super_Server") = objParametri_Super_Server

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect

            r.RispostaOK = True
            r.RispostaStringa = gestioneRedirect.PassaggioSitoPianiSemina(objParametri_Super_Server,
                                                                     objParametri_Server,
                                                                     objParametri_Utenti,
                                                                     VariabiliInSessione,
                                                                     InData,
                                                                     ParametriAggiuntivi,
                                                                     AggiungiSoloParametriAggiuntivi)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function PassaggioSitoAgronicaUma_NG(InData As CoreWS_Generic(Of InDataUtility)) As RispostaStandard
        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If InData.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
            HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti
            HttpContext.Current.Session("ASG_objParametri_Super_Server") = objParametri_Super_Server

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect

            Dim VariabiliInSessione = gestioneRedirect.GetVariabiliInSessioneNG(InData.InData.VariabiliInSessione)

            Dim ParametriAggiuntivi As New List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString)
            ParametriAggiuntivi = GetParametriAggiuntiviNG(InData.InData.ParametriAggiuntivi)

            Dim objAgendaNG As New AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG
            objAgendaNG = GetParametriObjParametriAgendaNG(InData.InData.InData)

            r.RispostaOK = True
            r.RispostaStringa = gestioneRedirect.PassaggioSitoAgronicaUma(objParametri_Super_Server,
                                                                     objParametri_Server,
                                                                     objParametri_Utenti,
                                                                     VariabiliInSessione,
                                                                     objAgendaNG,
                                                                     ParametriAggiuntivi,
                                                                     InData.InData.AggiungiSoloParametriAggiuntivi, InData.InData.IDSezione)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function PassaggioSitoAgronicaUma(ByVal objP_super_server As String,
                                        ByVal objP_server As String,
                                        ByVal objP_utenti As String,
                                        ByVal VariabiliInSessione As VariabiliInSessione_NG,
                                        ByVal InData As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG,
                                        ByVal ParametriAggiuntivi As List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString),
                                        ByVal AggiungiSoloParametriAggiuntivi As Boolean,
                                        ByVal IDSezione As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
            HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti
            HttpContext.Current.Session("ASG_objParametri_Super_Server") = objParametri_Super_Server

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect

            r.RispostaOK = True
            r.RispostaStringa = gestioneRedirect.PassaggioSitoAgronicaUma(objParametri_Super_Server,
                                                                     objParametri_Server,
                                                                     objParametri_Utenti,
                                                                     VariabiliInSessione,
                                                                     InData,
                                                                     ParametriAggiuntivi,
                                                                     AggiungiSoloParametriAggiuntivi, IDSezione)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Redirect_NG(InData As CoreWS_Generic(Of InDataUtility)) As RispostaStandard
        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If InData.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
            HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti
            HttpContext.Current.Session("ASG_objParametri_Super_Server") = objParametri_Super_Server

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect
            Dim VariabiliInSessione As New VariabiliInSessione_NG
            VariabiliInSessione = gestioneRedirect.GetVariabiliInSessioneNG(InData.InData.VariabiliInSessione)
            Dim objAgendaNG As New AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG
            objAgendaNG = GetParametriObjParametriAgendaNG(InData.InData.InData)

            r.RispostaOK = True
            r.RispostaStringa = gestioneRedirect.GestioneRedirectSitoAgenda(objParametri_Super_Server,
                                                                     objParametri_Server,
                                                                     objParametri_Utenti,
                                                                     VariabiliInSessione,
                                                                     objAgendaNG)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function PassaggioSitoAgronicaDomandaIrrigua(ByVal objP_super_server As String,
                                        ByVal objP_server As String,
                                        ByVal objP_utenti As String,
                                        ByVal VariabiliInSessione As VariabiliInSessione_NG,
                                        ByVal InData As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG,
                                        ByVal ParametriAggiuntivi As List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString),
                                        ByVal AggiungiSoloParametriAggiuntivi As Boolean,
                                        ByVal IDSezione As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
            HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti
            HttpContext.Current.Session("ASG_objParametri_Super_Server") = objParametri_Super_Server

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect

            r.RispostaOK = True
            r.RispostaStringa = gestioneRedirect.PassaggioSitoAgronicaDomandaIrrigua(objParametri_Super_Server,
                                                                     objParametri_Server,
                                                                     objParametri_Utenti,
                                                                     VariabiliInSessione,
                                                                     InData,
                                                                     ParametriAggiuntivi,
                                                                     AggiungiSoloParametriAggiuntivi, IDSezione)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function PassaggioSitoAgronicaDomandaIrrigua_NG(InData As CoreWS_Generic(Of InDataUtility)) As RispostaStandard
        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If InData.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
            HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti
            HttpContext.Current.Session("ASG_objParametri_Super_Server") = objParametri_Super_Server

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect

            Dim VariabiliInSessione = gestioneRedirect.GetVariabiliInSessioneNG(InData.InData.VariabiliInSessione)

            Dim ParametriAggiuntivi As New List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString)
            ParametriAggiuntivi = GetParametriAggiuntiviNG(InData.InData.ParametriAggiuntivi)

            Dim objAgendaNG As New AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG
            objAgendaNG = GetParametriObjParametriAgendaNG(InData.InData.InData)

            r.RispostaOK = True
            r.RispostaStringa = gestioneRedirect.PassaggioSitoAgronicaDomandaIrrigua(objParametri_Super_Server,
                                                                     objParametri_Server,
                                                                     objParametri_Utenti,
                                                                     VariabiliInSessione,
                                                                     objAgendaNG,
                                                                     ParametriAggiuntivi,
                                                                     InData.InData.AggiungiSoloParametriAggiuntivi, InData.InData.IDSezione)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function PassaggioSitoGiasOnlineVecchio_NG(InData As CoreWS_Generic(Of InDataUtility)) As RispostaStandard
        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If InData.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
            HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti
            HttpContext.Current.Session("ASG_objParametri_Super_Server") = objParametri_Super_Server

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect

            Dim VariabiliInSessione = gestioneRedirect.GetVariabiliInSessioneNG(InData.InData.VariabiliInSessione)

            Dim ParametriAggiuntivi As New List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString)
            ParametriAggiuntivi = GetParametriAggiuntiviNG(InData.InData.ParametriAggiuntivi)

            Dim objAgendaNG As New AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG
            objAgendaNG = GetParametriObjParametriAgendaNG(InData.InData.InData)

            r.RispostaOK = True
            r.RispostaStringa = gestioneRedirect.PassaggioSitoGiasOnlineVecchio(objParametri_Super_Server,
                                                                     objParametri_Server,
                                                                     objParametri_Utenti,
                                                                     VariabiliInSessione,
                                                                     objAgendaNG,
                                                                     ParametriAggiuntivi,
                                                                     InData.InData.AggiungiSoloParametriAggiuntivi, InData.InData.IDSezione)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Redirect(ByVal objP_super_server As String,
                                        ByVal objP_server As String,
                                        ByVal objP_utenti As String,
                                        ByVal VariabiliInSessione As VariabiliInSessione_NG,
                                        ByVal InData As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
            HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti
            HttpContext.Current.Session("ASG_objParametri_Super_Server") = objParametri_Super_Server

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect

            r.RispostaOK = True
            r.RispostaStringa = gestioneRedirect.GestioneRedirectSitoAgenda(objParametri_Super_Server,
                                                                     objParametri_Server,
                                                                     objParametri_Utenti,
                                                                     VariabiliInSessione,
                                                                     InData)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function LeggiConfigurazioneSiti(ByVal objP_super_server As String,
                                            ByVal objP_server As String,
                                            ByVal objP_utenti As String) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim objConfigurazioneSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim dt = objConfigurazioneSiti.Leggi_ServerESuperServer2(0, "", "", "", objParametri_Server, objParametri_Super_Server, True)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(dt, serializerSettings)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function LogoutGias(ByVal InData As Object) As rispostaStandard(Of String)

        Dim r As New rispostaStandard(Of String)

        Try

            Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
            Dim params As CoreWS_Generic(Of List(Of String)) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of List(Of String)))(JsonConvert.SerializeObject(InData), a)

            If Not IsNothing(params.InData) AndAlso params.InData.Count > 0 Then
                For Each c In params.InData
                    Dim cookie = HttpContext.Current.Response.Cookies.Get(c)
                    If Not IsNothing(cookie) Then
                        HttpContext.Current.Response.Cookies.Remove(c)
                    End If
                Next
            End If

            r.RispostaOK = True
            r.RispostaStringa = "OK"

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r


    End Function


End Class