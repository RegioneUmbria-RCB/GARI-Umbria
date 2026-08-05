Imports System.ComponentModel
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDTOStd.InData.IsAlive
Imports AgronicaCoreModelsSTD.IsAlive
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieDAL

<Script.Services.ScriptService()>
<WebService(Namespace:="http://tempuri.org/")>
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class IsAlive
    Inherits System.Web.Services.WebService

    Private Const LanToWebSiteBasePath_KEY As String = "LanToWebSiteBasePath"
    Private Const ServerAddresses_KEY As String = "ServerAddresses"

    Private Shared lanToWebSiteBasePath As String
    Private Shared ipServers As List(Of String)
    Private Shared objP_Server As AgronicaCoreParametri
    Private Shared objP_Super_Server As AgronicaCoreParametri
    Private Shared ReadOnly sitesMapper As New Dictionary(Of String, Tuple(Of String, String))

    Shared Sub New()
        ipServers = New List(Of String)

        sitesMapper.Add("Agenda", New Tuple(Of String, String)("LinkAgronicaAgenda2010", "/AgronicaAgenda_2010/IsAlive/IsAlive.asmx/IsAlive"))
        sitesMapper.Add("Analisi", New Tuple(Of String, String)("LinkAgronicaAnalisi_2010", "/AgronicaAnalisi_2010/IsAlive/IsAlive.asmx/IsAlive"))
        sitesMapper.Add("PianoConcimazione", New Tuple(Of String, String)("LinkPianoConcimazione_2017", "/PianoConcimazione_2017/IsAlive/IsAlive.asmx/IsAlive"))
        sitesMapper.Add("PUA", New Tuple(Of String, String)("LinkAgronicaPua", "/AgronicaPUA/IsAlive/IsAlive.asmx/IsAlive"))
        sitesMapper.Add("Stampe", New Tuple(Of String, String)("LinkAgronicaStampe_2010", "/AgronicaStampe_2010/IsAlive/IsAlive.asmx/IsAlive"))
        sitesMapper.Add("WebApiProfilatore", New Tuple(Of String, String)("", "/AgronicaWebApiProfilatore/IsAlive/IsAlive.asmx/IsAlive"))
        sitesMapper.Add("GiasBase", New Tuple(Of String, String)("LinkGiasBase", "/GiasBase/Default.aspx"))
        sitesMapper.Add("GiasNG", New Tuple(Of String, String)("LinkAgronicaGiasNG", "/GiasNG/"))
        sitesMapper.Add("Ws_Importa_GIAS", New Tuple(Of String, String)("Sincro_LinkWSImportaGIAS", "/WS_Importa_GIAS/IsAlive/IsAlive.asmx/IsAlive"))
        sitesMapper.Add("Audit", New Tuple(Of String, String)("LinkAgronicaAudit", "/AgronicaAudit/IsAlive/IsAlive.asmx/IsAlive"))
        sitesMapper.Add("PianiCampionamento", New Tuple(Of String, String)("LinkAgronicaPianiCampionamento", "/AgronicaPianiCampionamento/IsAlive/IsAlive.asmx/IsAlive"))
        sitesMapper.Add("Planning", New Tuple(Of String, String)("LinkAgronicaPlanning", "/AgronicaPlanning/IsAlive/IsAlive.asmx/IsAlive"))
        sitesMapper.Add("Sincronizzatore", New Tuple(Of String, String)("LinkAgronicaSincronizzatore", "/AgronicaSincronizzatoreWeb/IsAlive/IsAlive.asmx/IsAlive"))
        sitesMapper.Add("NetCoreApi", New Tuple(Of String, String)("GiasOnline_NetCore_API", "/AgronicaNetCoreAPI/IsAlive"))
        sitesMapper.Add("HubAgea", New Tuple(Of String, String)("LinkAPIHubAgea", "/api/health/is-alive"))
        sitesMapper.Add("NetCoreDataExchange", New Tuple(Of String, String)("GiasOnline_NetCoreDataExchange_API", "/IsAlive/IsAlive"))
        sitesMapper.Add("QdCACompliance", New Tuple(Of String, String)("GiasOnline_QdCACompliance_API", "/IsAlive/IsAlive"))
        sitesMapper.Add("SupportAPI", New Tuple(Of String, String)("LinkSupportAPI", "/api/health/is-alive"))
    End Sub

    Private Function GetConnectionString(ByVal input As String) As String
        Dim result As String = ""
        Dim pattern As String = "Server=([^;]+)|Initial Catalog=([^;]+)"
        Dim matches = Regex.Matches(input, pattern, RegexOptions.None, TimeSpan.FromSeconds(3))

        Dim serverValue As String = ""
        Dim catalogValue As String = ""

        For Each match In matches
            If match.Groups(1).Success Then
                serverValue = match.Groups(1).Value
            ElseIf match.Groups(2).Success Then
                catalogValue = match.Groups(2).Value
            End If
        Next

        If serverValue <> "" AndAlso catalogValue <> "" Then
            result = "Server=" & serverValue & "; Initial Catalog=" & catalogValue
        ElseIf serverValue <> "" Then
            result = "Server=" & serverValue
        ElseIf catalogValue <> "" Then
            result = "Initial Catalog=" & catalogValue
        End If

        Return result

    End Function

    ''' <summary>
    ''' WarmUp delle tabelle su EF
    ''' </summary>
    ''' <param name="objParametri_Server"></param>
    ''' <returns></returns>
    Private Function WarmUpEF(ByRef objParametri_Server As AgronicaCoreParametri) As String
        Dim err As String = ""

        Try
            Dim gefutils As New Gias_EF_Utility
            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)

            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
                Dim imprese = From im In GiasContext.Imprese Select im
            End Using
        Catch ex As Exception
            err = " Sistema di accesso ai dati basato su EntityFramework non attivo."
        End Try

        Return err

    End Function

    ''' <summary>
    ''' Legge il valore di una chiave da Configurazione_Siti.
    ''' Se non trova il valore coi parametri server, riprova coi parametri super server.
    ''' </summary>
    ''' <param name="key"></param>
    ''' <param name="configSiti_R"></param>
    ''' <param name="objP_Server"></param>
    ''' <param name="objP_Super_Server"></param>
    ''' <returns></returns>
    Private Function ReadValFromKey_ConfigSiti(ByVal key As String,
                                               ByRef configSiti_R As Configurazione_Siti_R,
                                               ByRef objP_Server As AgronicaCoreParametri,
                                               ByRef objP_Super_Server As AgronicaCoreParametri)
        If configSiti_R Is Nothing Then configSiti_R = New Configurazione_Siti_R

        Dim val As String = configSiti_R.Leggi_Valore(0, key, "", "", objP_Server)
        If String.IsNullOrEmpty(val) Then
            val = configSiti_R.Leggi_Valore(0, key, "", "", objP_Super_Server)
        End If

        Return val
    End Function

    Private Function AggiungiSlashSeNonEsiste(ByVal Percorso As String) As String
        If String.IsNullOrEmpty(Percorso) Then
            Return String.Empty
        End If

        If Not Percorso.ToLowerInvariant.Contains("http") AndAlso
            Not Percorso.StartsWith("/") Then
            Percorso = "/" & Percorso
        End If

        If Not Percorso.EndsWith("/") Then
            Return Percorso & "/"
        Else
            Return Percorso
        End If
    End Function

    Private Function PurificaUrl(siteKey As String, basePath As String, url As String) As String
        If Not siteKey.Equals("GiasBase") AndAlso Not siteKey.Equals("GiasNG") AndAlso url.ToLower().EndsWith("gestionerichieste.aspx") Then
            url = url.Replace("GestioneRichieste.aspx", "IsAlive/IsAlive.asmx/IsAlive")
        ElseIf siteKey.Equals("GiasNG") AndAlso url.ToLower().EndsWith("gestionerichieste") Then
            url = url.Replace("GestioneRichieste", "")
        ElseIf siteKey.Equals("GiasBase") Then
            url = AggiungiSlashSeNonEsiste(url)
        ElseIf siteKey.Equals("NetCoreApi") AndAlso
            Not url.Contains("/IsAlive?InData=") Then
            url &= "/IsAlive?InData=" & siteKey
        ElseIf siteKey.Equals("HubAgea") AndAlso Not url.Contains("/api/health/is-alive") Then
            url &= "/api/health/is-alive"
        ElseIf siteKey.Equals("Ws_Importa_GIAS") AndAlso url.ToLower().EndsWith("importaws.asmx") Then
            url = url.Replace("ImportaWS.asmx", "IsAlive/IsAlive.asmx/IsAlive")
        ElseIf siteKey.Equals("NetCoreDataExchange") AndAlso
            Not url.Contains("/IsAlive?InData=") Then
            url &= "/IsAlive?InData=" & siteKey
        ElseIf siteKey.Equals("QdCACompliance") AndAlso
            Not url.Contains("/IsAlive?InData=") Then
            url &= "/IsAlive/IsAlive?InData=" & siteKey
        ElseIf siteKey.Equals("SupportAPI") AndAlso Not url.Contains("/api/health/is-alive") Then
            url &= "/api/health/is-alive"
        End If

        If url.ToLower.StartsWith("http") Then
            Return url
        Else
            Return String.Format("{0}{1}", basePath, url)
        End If
    End Function

    Private Function ReadSiteByKey(siteKey As String) As ReachableSiteIN
        Dim reachableSite As New ReachableSiteIN(siteKey)
        Dim configSiti_R As New Configurazione_Siti_R

        Dim siteVal As String
        Dim tupleKeyLink As Tuple(Of String, String) =
            If(sitesMapper.ContainsKey(siteKey), sitesMapper(siteKey), New Tuple(Of String, String)("", ""))

        If tupleKeyLink.Item1 <> "" Then
            siteVal = ReadValFromKey_ConfigSiti(tupleKeyLink.Item1, configSiti_R, objP_Server, objP_Super_Server)

            ' Ritenta con la siteKey
            If String.IsNullOrEmpty(siteVal) Then
                siteVal = ReadValFromKey_ConfigSiti(siteKey, configSiti_R, objP_Server, objP_Super_Server)
            End If
        ElseIf tupleKeyLink.Item2 <> "" Then
            siteVal = tupleKeyLink.Item2
        Else
            siteVal = ReadValFromKey_ConfigSiti(siteKey, configSiti_R, objP_Server, objP_Super_Server)
        End If
        reachableSite.urls.Add(PurificaUrl(siteKey, lanToWebSiteBasePath, siteVal))

        For Each ipServer In ipServers
            Dim uri As Uri = Nothing
            If Uri.TryCreate(reachableSite.urls.First, UriKind.Absolute, uri) Then
                siteVal = $"{ipServer.TrimEnd("/")}/{uri.PathAndQuery.TrimStart("/")}"
            End If

            reachableSite.urls.Add(PurificaUrl(siteKey, lanToWebSiteBasePath, siteVal))
        Next

        ' Solo per TEST 
        'reachableSite.urls.Clear()
        'reachableSite.urls.AddRange(ipServers)

        Return reachableSite

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ReachableDB(InData As Object) As rispostaStandard(Of List(Of ReachableDBsOUT))
        Dim r As New rispostaStandard(Of List(Of ReachableDBsOUT))
        r.RispostaStringa = New List(Of ReachableDBsOUT)

        Dim objReq = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData))

        Try
            'lettura db su Super_Server
            Dim objParametri_Super_Server As AgronicaCoreParametri =
            Utility.convertStringtoOBJparametri(objReq.objP.objP_super_server)
            Dim connessioni_R As New AgronicaCoreDataProvider.Connessioni
            Dim respSuperServer = connessioni_R.IsAlive(objParametri_Super_Server)

            If respSuperServer Then
                r.RispostaStringa.Add(New ReachableDBsOUT("Super_Server", GetConnectionString(objParametri_Super_Server.StringaConnessione), "", Nothing, True))
            Else
                r.RispostaStringa.Add(New ReachableDBsOUT("Super_Server", GetConnectionString(objParametri_Super_Server.StringaConnessione), Gias.DB_SuperServer_Unreachable, Nothing, False))
            End If

            'se Super_Server andato a buon fine, legge db su Server
            Dim objParametri_Server As AgronicaCoreParametri =
                Utility.convertStringtoOBJparametri(objReq.objP.objP_server)
            Dim imprese_R As New AgronicaCoreAnagrafeBIZ.Impresa_R
            Dim respServer As RispostaStandard = imprese_R.IsAlive(objParametri_Server)

            'Warm up di EF
            Dim msgEF As String = ""
            If respServer.RispostaOK Then
                'se DB server up, warmup di EF
                msgEF = WarmUpEF(objParametri_Server)

                If msgEF <> "" Then
                    r.RispostaStringa.Add(New ReachableDBsOUT("Server", GetConnectionString(objParametri_Server.StringaConnessione), "", msgEF, False))
                Else
                    r.RispostaStringa.Add(New ReachableDBsOUT("Server", GetConnectionString(objParametri_Server.StringaConnessione), "", Nothing, True))
                End If
            Else
                r.RispostaStringa.Add(New ReachableDBsOUT("Server", GetConnectionString(objParametri_Server.StringaConnessione), Gias.DB_Server_Unreachable, Nothing, False))
            End If

            'se Server e EF andati a buon fine, legge db su Utenti
            Dim objParametri_Utenti As AgronicaCoreParametri =
                Utility.convertStringtoOBJparametri(objReq.objP.objP_utenti)
            Dim utenti_R As New AgronicaCoreUtentiBIZ.Utenti
            Dim respUtenti As RispostaStandard = utenti_R.IsAlive(objParametri_Utenti)

            If respUtenti.RispostaOK Then
                r.RispostaStringa.Add(New ReachableDBsOUT("Utenti", GetConnectionString(objParametri_Utenti.StringaConnessione), "", Nothing, True))
            Else
                r.RispostaStringa.Add(New ReachableDBsOUT("Utenti", GetConnectionString(objParametri_Utenti.StringaConnessione), Gias.DB_Utenti_Unreachable, Nothing, False))
            End If

            r.RispostaOK = True
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ReachableSites(InData As Object) As rispostaStandard(Of List(Of ReachableSiteIN))
        Dim r As New rispostaStandard(Of List(Of ReachableSiteIN))

        Dim objReq = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of List(Of String)))(JsonConvert.SerializeObject(InData))
        objP_Server = Utility.convertStringtoOBJparametri(objReq.objP.objP_server)
        objP_Super_Server = Utility.convertStringtoOBJparametri(objReq.objP.objP_super_server)

        Dim sites As List(Of String) = objReq.InData

        Dim configSiti_R As New Configurazione_Siti_R
        lanToWebSiteBasePath = ReadValFromKey_ConfigSiti(LanToWebSiteBasePath_KEY, configSiti_R, objP_Server, objP_Super_Server)

        Dim jsonServers = ReadValFromKey_ConfigSiti(ServerAddresses_KEY, configSiti_R, objP_Server, objP_Super_Server)
        If Not String.IsNullOrWhiteSpace(jsonServers) Then ipServers = JsonConvert.DeserializeObject(Of List(Of String))(jsonServers)

        Try
            r.RispostaStringa = New List(Of ReachableSiteIN)
            r.RispostaOK = True

            If sites.Count = 0 Then Return r

            sites.Where(Function(siteKey) Not IsNothing(siteKey) AndAlso Not siteKey.Equals("")).ToList.
                ForEach(Sub(siteKey)
                            If Not r.RispostaStringa.Any(Function(site As ReachableSiteIN) site.key.Equals(siteKey)) Then
                                r.RispostaStringa.Add(ReadSiteByKey(siteKey))
                            End If
                        End Sub)
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

End Class