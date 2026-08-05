Imports System.Text
Imports System.Web
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports Newtonsoft.Json

<CachedDataProviderAttribute("Configurazione_Siti_R")>
Public Class Configurazione_Siti_R
    Inherits AgronicaCoreDataProvider.CachedDataProvider

    Public Function Assistenza(objParametri_Super_Server As AgronicaCoreParametri, objParametri_Server As AgronicaCoreParametri) As String

        Dim sAssistenza As String = Leggi_Valore(0, "Assistenza", "", "", objParametri_Super_Server)

        If String.IsNullOrEmpty(sAssistenza) Then

            If Not objParametri_Server Is Nothing Then
                sAssistenza = Leggi_Valore(0, "Assistenza", "", "", objParametri_Server)
            End If

            If String.IsNullOrEmpty(sAssistenza) Then
                sAssistenza = " <a href=""mailto:assistenza@agronica.it"">assistenza@agronica.it</a>  "
            End If

        End If

        Return sAssistenza

    End Function

    <Cacheable(True)>
    Public Function Leggi(
                            ByRef objParametri As AgronicaCoreParametri,
                            ByVal strSql As String
                          ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreVarieDAL.Configurazione_Siti_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable

        Try

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function LeggiNonCacheable(ByVal Chiave As String,
                                      ByVal xFiltroAggiuntivo As String,
                                      ByRef objParametri As AgronicaCoreParametri) As DataTable
        Dim nomeRoutine As String = "AgronicaCoreVarieDAL.Configurazione_Siti_R.LeggiNonCacheable()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
            strSql.AppendLine(" SELECT * ")
            strSql.AppendLine(" FROM  Configurazione_Siti ")
            strSql.AppendLine(" WHERE 1=1  ")

            If Chiave <> "" Then
                strSql.AppendLine(" AND Chiave ='" & Agro_SQL_SaveText(Chiave) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    <Cacheable(True)>
    Public Function Leggi(ByVal Sito_Cod_NONUSARE As Integer,
                          ByVal Chiave As String,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional ByVal soloChiaviVisibiliLatoClient As Boolean = False
                          ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreVarieDAL.Configurazione_Siti_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
            strSql.AppendLine(" SELECT * ")
            strSql.AppendLine(" FROM  Configurazione_Siti WITH(NOLOCK)")
            strSql.AppendLine(" WHERE 1=1  ")
            If soloChiaviVisibiliLatoClient Then
                Dim soloVisibilitaLatoClient As Int16 = 1
                strSql.AppendLine(" AND Visibilita = " & Agro_SQL_SaveNum(soloVisibilitaLatoClient))
            End If

            'If Agro_SQL_SaveText(objParametri.PivaSuperUser) <> "" Then
            '    strSql.AppendLine(" AND PivaSuperUser ='" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            'End If
            'If Sito_Cod <> 0 Then
            '    strSql.AppendLine(" AND Sito_Cod =" & Agro_SQL_SaveNum(Sito_Cod) & " ")
            'End If

            If Chiave <> "" Then
                strSql.AppendLine(" AND Chiave ='" & Agro_SQL_SaveText(Chiave) & "' ")
            End If

            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY  Chiave ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------



        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    'Salvatore Zammataro 05/10/2022: creata tale funzione per permettere di leggere i parametri della tabella Configurazione_Siti, sia da server che super_server
    <Cacheable(True)>
    Public Function Leggi_ServerESuperServer(
        ByVal Sito_Cod_NONUSARE As Integer,
        ByVal Chiave As String,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri_server As AgronicaCoreParametri,
        ByRef objParametri_superServer As AgronicaCoreParametri
    ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreVarieDAL.Configurazione_Siti_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine("SELECT")
            strSql.AppendLine("")
            strSql.AppendLine("    COALESCE (s.PivaSuperUser, ss.PivaSuperUser COLLATE SQL_Latin1_General_CP850_CI_AS) as PivaSuperUser,")
            strSql.AppendLine("    COALESCE (s.Sito_Cod, ss.Sito_Cod) as Sito_Cod,")
            strSql.AppendLine("    COALESCE (s.Chiave, ss.Chiave COLLATE SQL_Latin1_General_CP850_CI_AS) as Chiave,")
            strSql.AppendLine("    COALESCE (s.Valore, ss.Valore COLLATE SQL_Latin1_General_CP850_CI_AS) as Valore")
            strSql.AppendLine("")
            strSql.AppendLine("FROM Configurazione_Siti s")
            strSql.AppendLine("FULL OUTER JOIN " & objParametri_superServer.Recupera_NomeDB() & ".dbo.configurazione_Siti ss ")
            strSql.AppendLine("ON s.Chiave = ss.Chiave COLLATE SQL_Latin1_General_CP850_CI_AS")
            strSql.AppendLine(" WHERE 1=1  ")

            'If Agro_SQL_SaveText(objParametri.PivaSuperUser) <> "" Then
            '    strSql.AppendLine(" AND PivaSuperUser ='" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            'End If
            'If Sito_Cod <> 0 Then
            '    strSql.AppendLine(" AND Sito_Cod =" & Agro_SQL_SaveNum(Sito_Cod) & " ")
            'End If

            If Chiave <> "" Then
                strSql.AppendLine(" AND Chiave ='" & Agro_SQL_SaveText(Chiave) & "' ")
            End If

            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_server))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_server))
            Else
                strSql.AppendLine(" ORDER BY  Chiave ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_server, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    <Cacheable(True)>
    Public Function Leggi_ServerESuperServer2(
        ByVal Sito_Cod_NONUSARE As Integer,
        ByVal Chiave As String,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri_server As AgronicaCoreParametri,
        ByRef objParametri_superServer As AgronicaCoreParametri,
        ByVal soloChiaviVisibiliLatoClient As Boolean
    ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreVarieDAL.Configurazione_Siti_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            Dim dtServer = Leggi(0, "", "", "", objParametri_server, soloChiaviVisibiliLatoClient)
            Dim dtSuperServer = Leggi(0, "", "", "", objParametri_superServer, soloChiaviVisibiliLatoClient)

            Dim listChiaviServer As List(Of String) = (From r In dtServer.Rows Select CStr(r("Chiave"))).ToList()
            Dim listChiaviSuperServer As List(Of String) = (From r In dtSuperServer.Rows Select CStr(r("Chiave"))).ToList()
            Dim listChiaviSoloSuperServer As New List(Of String)
            For Each el In listChiaviSuperServer
                If Not listChiaviServer.Contains(el) Then
                    listChiaviSoloSuperServer.Add(el)
                End If
            Next

            Dim dtFinale = dtServer.Copy

            For Each row In dtSuperServer.Rows
                Dim chiaveCC = row("chiave")
                If listChiaviSoloSuperServer.Contains(chiaveCC) Then
                    Dim rowAdd = dtFinale.NewRow
                    rowAdd("PivaSuperUser") = row("PivaSuperUser")
                    rowAdd("Sito_Cod") = row("Sito_Cod")
                    rowAdd("Chiave") = row("Chiave")
                    rowAdd("Valore") = row("Valore")
                    dtFinale.Rows.Add(rowAdd)
                End If

            Next

            Return dtFinale

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Ottiene_Lav_Cod_Ricettabili(objP_Server As AgronicaCoreParametri,
                                                Optional isFromNG As Boolean = False) As List(Of String)

        Dim StrLavCodRicettabili = CostantiPersonalizzate.STR_OP_RICETTABILI

        If isFromNG Then
            StrLavCodRicettabili += "," & STR_OP_RICETTABILI_NG
        End If

        'Escludo le Ricette di Irrigazione se la chiave IrrigazioneBS è false
        'perchè le Irrigazioni vecchie non le gestivano le ricette
        Dim ListLavCodRicettabili As List(Of String) = StrLavCodRicettabili.Split(",").ToList()

        If Not IsNothing(ListLavCodRicettabili) AndAlso ListLavCodRicettabili.Count > 0 Then
            Dim dtIrrigazione As DataTable = Leggi(0, "IrrigazioneBS", " valore = 'true' ", "", objP_Server)

            If IsNothing(dtIrrigazione) OrElse dtIrrigazione.Rows.Count = 0 Then
                ListLavCodRicettabili = ListLavCodRicettabili.Where(Function(value) value <> "1").ToList()
            End If

        End If

        StrLavCodRicettabili = String.Join(",", ListLavCodRicettabili.ToArray())

        Return ListLavCodRicettabili
    End Function

    '###############################################################################
    Public Function Recupera_Valore_ByChiave(ByVal Sito_Cod As Integer,
                                             ByVal Chiave As String,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As String

        Dim valore As String = ""
        Dim dt As DataTable

        dt = Leggi(Sito_Cod,
                   Chiave,
                   "", "",
                   objParametri)

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            valore = dt.Rows(0).Item("Valore")
        End If

        Return valore

    End Function

    Function Leggi_Valore(ByVal Sito_Cod As Integer,
                          ByVal Chiave As String,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As String

        Dim valore As String = ""
        Dim dt As DataTable = Leggi(Sito_Cod,
                                    Chiave,
                                    xFiltroAggiuntivo,
                                    xOrderBy,
                                    objParametri)
        If dt.Rows.Count > 1 Then
            Throw New Exception("Troppi record selezionati")
        End If
        If dt.Rows.Count = 1 Then
            valore = dt.Rows(0).Item("Valore")
        End If

        Return valore

    End Function

    Function Leggi_Valore_ServerESuperServer(ByVal Sito_Cod As Integer,
                          ByVal Chiave As String,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametriServer As AgronicaCoreParametri,
                          ByRef objParametriSuperServer As AgronicaCoreParametri
                          ) As String

        Dim result = Leggi_Valore(Sito_Cod, Chiave, xFiltroAggiuntivo, xOrderBy, objParametriServer)
        If Not String.IsNullOrWhiteSpace(result) Then
            Return result
        End If
        
        Return Leggi_Valore(Sito_Cod, Chiave, xFiltroAggiuntivo, xOrderBy, objParametriSuperServer)

    End Function

    Function Leggi_ValoreNonCacheable(
                          ByVal Chiave As String,
                          ByVal xFiltroAggiuntivo As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As String

        Dim valore As String = ""
        Dim dt As DataTable = LeggiNonCacheable(Chiave, xFiltroAggiuntivo, objParametri)

        If dt.Rows.Count > 1 Then
            Throw New Exception("Troppi record selezionati")
        End If
        If dt.Rows.Count = 1 Then
            valore = dt.Rows(0).Item("Valore")
        End If

        Return valore

    End Function

    Public Function LeggiValoreAsBooleanType(chiave As String, objParametri_Server As AgronicaCoreParametri) As Boolean
        Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim gotoMenuAgendaNG = objConfSiti.Leggi_Valore(0, chiave, "", "", objParametri_Server)

        If gotoMenuAgendaNG = "" Then
            Return False
        End If

        Return Boolean.Parse(gotoMenuAgendaNG)
    End Function

    'Legge un valore di una chiave in formato JSON e ne deserializza il valore nell'oggetto passato come parametro
    'La chiave viene dedotta dalla classe passata a partire dal 5° carattere (dopo "VCS_")
    Function Leggi_Valore_JSON(Of T)(ByRef objParametri As AgronicaCoreParametri) As T

        Dim valore As String = String.Empty

        Dim tipo = GetType(T)

        Dim chiave = tipo.Name.Substring(4)

        valore = Leggi_Valore(0, chiave, "", "", objParametri)

        If String.IsNullOrEmpty(valore) Then
            Select Case tipo.Name
                Case "VCS_Lavorazioni"
                    Dim objNew As New VCS_Lavorazioni
                    valore = JsonConvert.SerializeObject(objNew)
                Case "VCS_OrdiniLavorazione"
                    Dim objNew As New VCS_OrdiniLavorazione
                    valore = JsonConvert.SerializeObject(objNew)
                Case "VCS_ContrattiAffitto"
                    Dim objNew As New VCS_ContrattiAffitto
                    valore = JsonConvert.SerializeObject(objNew)
                Case "VCS_TestUMA"
                    Dim objNew As New VCS_TestUMA
                    valore = JsonConvert.SerializeObject(objNew)
                Case Else
                    Throw New Exception("Classe valore JSON non gestita: " & tipo.Name)
            End Select
        End If

        If Not String.IsNullOrEmpty(valore) Then
            Return JsonConvert.DeserializeObject(Of T)(valore)
        End If

    End Function

    'legge dal config_siti le chiavi che servono per la stampa massiva dei report tramite servizio windows AgroWinSrvc_PrintUtility_2015
    'o stampa diretta dal crystal reports
    Public Sub Leggi_Config_AgroWinSrvcPrintUtility(ByRef objParametri_Server As AgronicaCoreParametri,
                                                  ByRef Mode_AgroWinSrvc_PrintUtility_PtP As Integer,
                                                  ByRef PathComandi_AgroWinSrvc_PrintUtility As String,
                                                  ByRef PathDati_AgroWinSrvc_PrintUtility As String)

        '0 chiamata al servizio windows
        '1 funzione print to printer
        Mode_AgroWinSrvc_PrintUtility_PtP = 1
        'di solito c:\giaslan\AgroWinSrvc_PrintUtility_2015\WorkingDir\Comandi
        PathComandi_AgroWinSrvc_PrintUtility = ""
        'di solito: c:\giaslan\AgroWinSrvc_PrintUtility_2015\WorkingDir\Dati
        PathDati_AgroWinSrvc_PrintUtility = ""

        Try

            Dim DTConfigSiti As DataTable
            Dim FiltroAgg As String = " Chiave IN ('Mode_AgroWinSrvc_PrintUtility_PtP', 'PathComandi_AgroWinSrvc_PrintUtility', 'PathDati_AgroWinSrvc_PrintUtility') "

            DTConfigSiti = Leggi(0, "", FiltroAgg, "", objParametri_Server)

            If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 Then
                For Each Dr In DTConfigSiti.Rows
                    Select Case Dr("Chiave")
                        Case "Mode_AgroWinSrvc_PrintUtility_PtP"
                            Mode_AgroWinSrvc_PrintUtility_PtP = Dr("Valore")
                        Case "PathComandi_AgroWinSrvc_PrintUtility"
                            PathComandi_AgroWinSrvc_PrintUtility = Dr("Valore")
                        Case "PathDati_AgroWinSrvc_PrintUtility"
                            PathDati_AgroWinSrvc_PrintUtility = Dr("Valore")
                    End Select
                Next
            End If

        Catch ex As Exception
            Throw New Exception("Leggi_Config_AgroWinSrvcPrintUtility: " & ex.Message)
        End Try

    End Sub

    Public Function leggiConfigurazioneBDNVetInfo(ByRef objParametri_Server As AgronicaCoreParametri) As Configurazione_BDN_VetInfo
        Dim DTConfigSiti As DataTable
        DTConfigSiti = Leggi(0, "BDNVetInfo_Conf", "", "", objParametri_Server)
        Dim retVal As Configurazione_BDN_VetInfo
        If DTConfigSiti IsNot Nothing AndAlso DTConfigSiti.Rows.Count > 0 Then
            Dim val = CStr(DTConfigSiti.Rows(0)("valore"))
            If val <> "" Then
                retVal = JsonConvert.DeserializeObject(Of Configurazione_BDN_VetInfo)(val)
            End If
        End If
        Return retVal
    End Function

    Public Function leggiConfigurazioneNogmo(ByRef objParametri_Server As AgronicaCoreParametri) As Configurazione_NOGMO
        Dim DTConfigSiti As DataTable
        DTConfigSiti = Leggi(0, "NOGMO_Conf", "", "", objParametri_Server)
        Dim retVal As Configurazione_NOGMO
        If DTConfigSiti IsNot Nothing AndAlso DTConfigSiti.Rows.Count > 0 Then
            Dim val = CStr(DTConfigSiti.Rows(0)("valore"))
            If val <> "" Then
                retVal = JsonConvert.DeserializeObject(Of Configurazione_NOGMO)(val)
            End If
        End If
        Return retVal
    End Function


    Public Function getCaheServersUrls(objParametri_Server As AgronicaCoreParametri) As List(Of String)

        ConfigurazioneAjaxFactory.Reset(objParametri_Server)

        Dim urls = New List(Of String)

        Dim LanToWebSiteBasePath = Leggi_Valore(0, "LanToWebSiteBasePath", "", "", objParametri_Server)
        Dim jsonServers = Leggi_Valore(0, "ServerAddresses", "", "", objParametri_Server)
        Dim servers As String() = {}
        If (Not String.IsNullOrWhiteSpace(jsonServers)) Then
            servers = JsonConvert.DeserializeObject(Of String())(jsonServers)
        End If


        Dim chiavi = New List(Of String) From
                    {
                        "'GiasOnline_WS_Core_AgroWS_Core'",
                        "'LinkAgronicaStampe_2010'",
                        "'LinkAgronicaProfilazione'",
                        "'LinkAgronicaPlanning'",
                        "'LinkAgronicaAnalisi_2010'",
                        "'LinkAgronicaPianiCampionamento'",
                        "'LinkAgronicaPianiSemina'",
                        "'LinkAgronicaPua'",
                        "'LinkAgronicaAudit'",
                        "'LinkAgronicaSincronizzatore'",
                        "'LinkPianoConcimazione_2017'",
                        "'LinkAgronicaUMA'",
                        "'LinkAgronicaDomandaIrrigua'",
                        "'LinkAgronicaAgenda2010'"
                    }

        Dim xfiltroAggiuntivo As String = " chiave in (" + String.Join(",", chiavi) + ") "
        Dim dt As DataTable = Leggi(0, "", xfiltroAggiuntivo, "", objParametri_Server)
        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            For Each row As DataRow In dt.Rows
                Dim url As String = ""

                Dim endpointPulisciCache As String = "Cache/CacheManager.asmx/PulisciCache"
                url = PurificaUrl(LanToWebSiteBasePath, row("valore").ToString.Trim, endpointPulisciCache)
                If Not url.ToLower.Contains("/Cache/CacheManager.asmx/PulisciCache".ToLower) Then
                    If url.EndsWith("/") Then
                        url = url & endpointPulisciCache
                    Else
                        url = url & "/" & endpointPulisciCache
                    End If
                End If
                urls.Add(url)

                Dim endpointPulisciCacheEFactory As String = "Cache/CacheManager.asmx/PulisciCacheEFactory"
                url = PurificaUrl(LanToWebSiteBasePath, row("valore").ToString.Trim, endpointPulisciCacheEFactory)
                If Not url.ToLower.Contains("/Cache/CacheManager.asmx/PulisciCacheEFactory".ToLower) Then
                    If url.EndsWith("/") Then
                        url = url & endpointPulisciCacheEFactory
                    Else
                        url = url & "/" & endpointPulisciCacheEFactory
                    End If
                End If
                urls.Add(url)
            Next
        End If

        Dim remoteServerUrls = New List(Of String)
        For Each server As String In servers
            For Each url As String In urls
                Dim uri As Uri = Nothing
                If Uri.TryCreate(url, UriKind.Absolute, uri) Then
                    remoteServerUrls.Add($"{server.TrimEnd("/")}/{uri.PathAndQuery.TrimStart("/")}")
                End If
            Next
        Next
        urls.AddRange(remoteServerUrls)

        Return urls

    End Function

    Public Function getCaheServersUrlsPermessi(objParametri_Server As AgronicaCoreParametri) As List(Of String)

        ConfigurazioneAjaxFactory.Reset(objParametri_Server)

        Dim urls = New List(Of String)

        Dim LanToWebSiteBasePath = Leggi_Valore(0, "LanToWebSiteBasePath", "", "", objParametri_Server)
        Dim jsonServers = Leggi_Valore(0, "ServerAddresses", "", "", objParametri_Server)
        Dim servers As String() = {}
        If (Not String.IsNullOrWhiteSpace(jsonServers)) Then
            servers = JsonConvert.DeserializeObject(Of String())(jsonServers)
        End If


        Dim chiavi = New List(Of String) From
                    {
                        "'GiasOnline_WS_Core_AgroWS_Core'",
                        "'LinkAgronicaStampe_2010'",
                        "'LinkAgronicaProfilazione'",
                        "'LinkAgronicaPlanning'",
                        "'LinkAgronicaAnalisi_2010'",
                        "'LinkAgronicaPianiCampionamento'",
                        "'LinkAgronicaPianiSemina'",
                        "'LinkAgronicaPua'",
                        "'LinkAgronicaAudit'",
                        "'LinkAgronicaSincronizzatore'",
                        "'LinkPianoConcimazione_2017'",
                        "'LinkAgronicaUMA'",
                        "'LinkAgronicaDomandaIrrigua'",
                        "'LinkAgronicaAgenda2010'"
                    }

        Dim xfiltroAggiuntivo As String = " chiave in (" + String.Join(",", chiavi) + ") "
        Dim dt As DataTable = Leggi(0, "", xfiltroAggiuntivo, "", objParametri_Server)
        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            For Each row As DataRow In dt.Rows
                Dim url As String = ""

                Dim endpointPulisciCachePermessi As String = "Cache/CacheManager.asmx/PulisciCachePermessi"
                url = PurificaUrl(LanToWebSiteBasePath, row("valore").ToString.Trim, endpointPulisciCachePermessi)
                If Not url.ToLower.Contains("/Cache/CacheManager.asmx/PulisciCachePermessi".ToLower) Then
                    If url.EndsWith("/") Then
                        url = url & endpointPulisciCachePermessi
                    Else
                        url = url & "/" & endpointPulisciCachePermessi
                    End If
                End If
                urls.Add(url)
            Next
        End If

        Dim remoteServerUrls = New List(Of String)
        For Each server As String In servers
            For Each url As String In urls
                Dim uri As Uri = Nothing
                If Uri.TryCreate(url, UriKind.Absolute, uri) Then
                    remoteServerUrls.Add($"{server.TrimEnd("/")}/{uri.PathAndQuery.TrimStart("/")}")
                End If
            Next
        Next
        urls.AddRange(remoteServerUrls)

        Return urls

    End Function

    Public Function getCaheServersUrlsImpostazioni(objParametri_Server As AgronicaCoreParametri) As List(Of String)

        ConfigurazioneAjaxFactory.Reset(objParametri_Server)

        Dim urls = New List(Of String)

        Dim LanToWebSiteBasePath = Leggi_Valore(0, "LanToWebSiteBasePath", "", "", objParametri_Server)
        Dim jsonServers = Leggi_Valore(0, "ServerAddresses", "", "", objParametri_Server)
        Dim servers As String() = {}
        If (Not String.IsNullOrWhiteSpace(jsonServers)) Then
            servers = JsonConvert.DeserializeObject(Of String())(jsonServers)
        End If


        Dim chiavi = New List(Of String) From
                    {
                        "'GiasOnline_WS_Core_AgroWS_Core'",
                        "'LinkAgronicaStampe_2010'",
                        "'LinkAgronicaProfilazione'",
                        "'LinkAgronicaPlanning'",
                        "'LinkAgronicaAnalisi_2010'",
                        "'LinkAgronicaPianiCampionamento'",
                        "'LinkAgronicaPianiSemina'",
                        "'LinkAgronicaPua'",
                        "'LinkAgronicaAudit'",
                        "'LinkAgronicaSincronizzatore'",
                        "'LinkPianoConcimazione_2017'",
                        "'LinkAgronicaUMA'",
                        "'LinkAgronicaDomandaIrrigua'",
                        "'LinkAgronicaAgenda2010'"
                    }

        Dim xfiltroAggiuntivo As String = " chiave in (" + String.Join(",", chiavi) + ") "
        Dim dt As DataTable = Leggi(0, "", xfiltroAggiuntivo, "", objParametri_Server)
        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            For Each row As DataRow In dt.Rows
                Dim url As String = ""

                Dim endpointPulisciCacheImpostazioni As String = "Cache/CacheManager.asmx/PulisciCacheImpostazioni"
                url = PurificaUrl(LanToWebSiteBasePath, row("valore").ToString.Trim, endpointPulisciCacheImpostazioni)
                If Not url.ToLower.Contains("/Cache/CacheManager.asmx/PulisciCacheImpostazioni".ToLower) Then
                    If url.EndsWith("/") Then
                        url = url & endpointPulisciCacheImpostazioni
                    Else
                        url = url & "/" & endpointPulisciCacheImpostazioni
                    End If
                End If
                urls.Add(url)
            Next
        End If

        Dim remoteServerUrls = New List(Of String)
        For Each server As String In servers
            For Each url As String In urls
                Dim uri As Uri = Nothing
                If Uri.TryCreate(url, UriKind.Absolute, uri) Then
                    remoteServerUrls.Add($"{server.TrimEnd("/")}/{uri.PathAndQuery.TrimStart("/")}")
                End If
            Next
        Next
        urls.AddRange(remoteServerUrls)

        Return urls

    End Function

    Public Function getCaheServersUrlsMetodo(objParametri_Server As AgronicaCoreParametri) As List(Of String)

        ConfigurazioneAjaxFactory.Reset(objParametri_Server)

        Dim urls = New List(Of String)

        Dim LanToWebSiteBasePath = Leggi_Valore(0, "LanToWebSiteBasePath", "", "", objParametri_Server)
        Dim jsonServers = Leggi_Valore(0, "ServerAddresses", "", "", objParametri_Server)
        Dim servers As String() = {}
        If (Not String.IsNullOrWhiteSpace(jsonServers)) Then
            servers = JsonConvert.DeserializeObject(Of String())(jsonServers)
        End If


        Dim chiavi = New List(Of String) From
                    {
                        "'GiasOnline_WS_Core_AgroWS_Core'",
                        "'LinkAgronicaStampe_2010'",
                        "'LinkAgronicaProfilazione'",
                        "'LinkAgronicaPlanning'",
                        "'LinkAgronicaAnalisi_2010'",
                        "'LinkAgronicaPianiCampionamento'",
                        "'LinkAgronicaPianiSemina'",
                        "'LinkAgronicaPua'",
                        "'LinkAgronicaAudit'",
                        "'LinkAgronicaSincronizzatore'",
                        "'LinkPianoConcimazione_2017'",
                        "'LinkAgronicaUMA'",
                        "'LinkAgronicaDomandaIrrigua'",
                        "'LinkAgronicaAgenda2010'"
                    }

        Dim xfiltroAggiuntivo As String = " chiave in (" + String.Join(",", chiavi) + ") "
        Dim dt As DataTable = Leggi(0, "", xfiltroAggiuntivo, "", objParametri_Server)
        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            For Each row As DataRow In dt.Rows
                Dim url As String = ""

                Dim endpointPulisciCacheMetodo As String = "Cache/CacheManager.asmx/PulisciCacheMetodo"
                url = PurificaUrl(LanToWebSiteBasePath, row("valore").ToString.Trim, endpointPulisciCacheMetodo)
                If Not url.ToLower.Contains("/Cache/CacheManager.asmx/PulisciCacheMetodo".ToLower) Then
                    If url.EndsWith("/") Then
                        url = url & endpointPulisciCacheMetodo
                    Else
                        url = url & "/" & endpointPulisciCacheMetodo
                    End If
                End If
                urls.Add(url)
            Next
        End If

        Dim remoteServerUrls = New List(Of String)
        For Each server As String In servers
            For Each url As String In urls
                Dim uri As Uri = Nothing
                If Uri.TryCreate(url, UriKind.Absolute, uri) Then
                    remoteServerUrls.Add($"{server.TrimEnd("/")}/{uri.PathAndQuery.TrimStart("/")}")
                End If
            Next
        Next
        urls.AddRange(remoteServerUrls)

        Return urls

    End Function

    Private Function PurificaUrl(basePath As String, url As String, endpoint As String) As String

        If String.IsNullOrEmpty(url) Then
            Return ""
        End If

        url = url.Replace("GestioneRichieste.aspx", endpoint)
        If url.ToLower.StartsWith("http") Then
            Return url
        Else
            Return String.Format("{0}{1}", basePath, url)
        End If

    End Function

End Class

<CachedDataProviderAttribute("Configurazione_Siti_W", "Configurazione_Siti_R")>
Public Class Configurazione_Siti_W
    Inherits AgronicaCoreDataProvider.CachedDataProvider


    '##############################################################################################
    <ClearCache>
    Public Function AggiornaConfigurazione(ByVal sito_cod As Integer,
                                           ByVal Chiave As String,
                                           ByVal Valore As String,
                                           ByRef objParametri As AgronicaCoreParametri,
                                           Optional ByVal Data_creazione As Date = #2/1/1900#,
                                           Optional ByVal Data_modifica As Date = #2/1/1900#,
                                           Optional ByVal username_creazione As String = "",
                                           Optional ByVal username_modifica As String = ""
                                           ) As Boolean

        Dim nomeRoutine As String = "Scrivi()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If


            '---------------------------------------------
            stb.Length = 0
            stb.AppendLine(" UPDATE Configurazione_Siti ")

            stb.AppendLine(" SET Valore = '" & Agro_SQL_SaveText(Valore) & "' ")
            'stb.AppendLine(" , Data_Modifica = " & Agro_SQL_SaveDateTime(Data_modifica) & " ")
            'stb.AppendLine(" , Username_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' ")

            stb.AppendLine(" WHERE Chiave = '" & Agro_SQL_SaveText(Chiave) & "' ")
            stb.AppendLine(" AND PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            stb.AppendLine(" AND Sito_Cod = " & Agro_SQL_SaveNum(sito_cod))


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return xRisp

    End Function

    <ClearCache>
    Public Function Scrivi(ByVal Sito_Cod As Integer,
                           ByVal Chiave As String, Valore As String,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean

        Dim nomeRoutine As String = "Scrivi()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            stb.Length = 0
            stb.AppendLine(" INSERT Configurazione_Siti ")

            stb.AppendLine("              (")
            stb.AppendLine("         PivaSuperUser , ")
            stb.AppendLine("         Sito_Cod, ")
            stb.AppendLine("         Chiave, ")
            stb.AppendLine("             Valore ")
            stb.AppendLine("              ) ")

            stb.AppendLine(" VALUES ( ")

            stb.AppendLine("           '" & objParametri.PivaSuperUser & "' ")
            stb.AppendLine("         ,  " & Agro_SQL_SaveNum(Sito_Cod) & " ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(Chiave) & "' ")
            stb.AppendLine("		 , '" & Agro_SQL_SaveText(Valore) & "' ")

            stb.Append(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------



        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return xRisp

    End Function

    <ClearCache>
    Public Function AggiornaValori(ByVal chiavi As String(), ByVal valori As String(), ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim nomeRoutine = NameOf(AggiornaValori)

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim risposta As Boolean = False

        Try
            stb.Length = 0

            stb.AppendLine(" UPDATE Configurazione_Siti")
            stb.AppendLine(" SET    Valore = CASE ")

            For i = 0 To chiavi.Length - 1
                Dim chiave As String = chiavi(i)
                Dim valore As String = valori(i)

                stb.AppendLine("    WHEN   Chiave = '" & Agro_SQL_SaveText(chiave) & "' THEN '" & Agro_SQL_SaveText(valore) & "' ")
            Next
            stb.AppendLine("    ELSE   Valore END")
            stb.AppendLine(" WHERE  Chiave IN (")

            For i = 0 To chiavi.Length - 1
                Dim chiave As String = chiavi(i)

                If (i <> (chiavi.Length - 1)) Then
                    stb.AppendLine(" '" & Agro_SQL_SaveText(chiave) & "', ")
                Else
                    stb.AppendLine(" '" & Agro_SQL_SaveText(chiave) & "' )")
                End If
            Next

            risposta = EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            risposta = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return risposta

    End Function

End Class

'Classi VCS (Valori Configurazione Siti) che rappresentano il formato JSON del VALORE contenuto nelle rispettive CHIAVI

''' <summary>
''' Classe lettura valori configurazione siti per chiave Lavorazioni
''' </summary>
Public Class VCS_Lavorazioni

    ''' <value>
    ''' Controllo univocità identificativo lavorazione.
    ''' 0=Nessun controllo
    ''' 1=Univoco considerando solo lavorazioni aperte (bloccante)
    ''' 2=Univoco assoluto (bloccante)
    ''' </value>
    Public CtrUnivIdenLav As Integer = 0

End Class

''' <summary>
''' Classe lettura valori configurazione siti per chiave OrdiniLavorazione
''' </summary>
Public Class VCS_OrdiniLavorazione

    ''' <value>
    ''' Controllo univocità identificativo lavorazione.
    ''' 0=Nessun controllo
    ''' 1=Univoco considerando solo ordini aperti (bloccante)
    ''' 2=Univoco assoluto (bloccante)
    ''' </value>
    Public CtrUnivIdenLav As Integer = 0

    ''' <value>
    ''' Controllo legame con lavorazioni.
    ''' 0=Nessun controllo
    ''' 1=Singolo considerando solo lavorazioni aperte (warning)
    ''' 2=Singolo assoluto (warning)
    ''' </value>
    Public CtrLegameLav As Integer = 0

End Class

''' <summary>
''' Classe lettura valori configurazione siti per chiave ContrattiAffitto
''' </summary>
Public Class VCS_ContrattiAffitto

    ''' <value>
    ''' Scrittura log in fase di aggiornamento imprese per particelle
    ''' 0=No
    ''' 1=Sì
    ''' </value>
    Public ScriviLogAggiornaImpreseParticelle As Integer = 0

End Class

''' <summary>
''' Classe lettura valori configurazione siti per chiave TestUMA
''' </summary>
Public Class VCS_TestUMA

    ''' <value>
    ''' Modalità test UMA abilitata
    ''' 0=No
    ''' 1=Sì
    ''' </value>
    Public Abilitato As Integer = 0

    ''' <value>
    ''' Mese per simulazione passaggio di stato richieste
    ''' 0=No
    ''' 1=Sì
    ''' </value>
    Public Mese As Integer = 0

    ''' <value>
    ''' Log calcolo capi allevabili
    ''' 0=No
    ''' 1=Sì
    ''' </value>
    Public LogCalcoloCapiAllevabili As Integer = 0

    ''' <value>
    ''' Log caricamento elenco pratiche
    ''' 0=No
    ''' 1=Sì
    ''' </value>
    Public LogCaricaElencoPratiche As Integer = 0

    ''' <value>
    ''' Setta isolation level in caricamento elenco pratiche
    ''' 0=No
    ''' 1=ReadUncommitted
    ''' </value>
    Public SetIsolationLevelElencoPratiche As Integer = 0

    ''' <value>
    ''' Indica se usare le tabelle temporanee al posto delle CTE
    ''' 0=No
    ''' 1=Sì
    ''' </value>
    Public UsaTempTableElencoPratiche As Integer = 0

    ''' <value>
    ''' Indica se disabilitare il controllo bloccante sui trasferimenti nel caso il destinatario abbia già una rendicontazione approvata
    ''' 0=No
    ''' 1=Sì
    ''' </value>
    Public DisabCtrBloccTrasfRendApprovata As Integer = 0

End Class

Public Class Configurazione_BDN_VetInfo
    Public BDN As Configurazione_BDN
    Public VetInfo As Configurazione_VetInfo
    Public logMsg As Boolean
End Class

Public Class Configurazione_BDN
    Public link As String
    Public username As String
    Public password As String
    Public clientId As String
    Public authorizeLink As String
    Public authorizePath As String
    Public Scope As String
End Class

Public Class Configurazione_VetInfo
    Public link As String
    Public clientId As String
    Public clientSecret As String
    Public authorizeLink As String
    Public authorizePath As String
    Public Scope As String
End Class


Public Class Configurazione_NOGMO
    Public link As String
    Public username As String
    Public password As String
    Public token As String
    Public addEndPoint As String
    Public checkEndPoint As String
    Public exporterID As Integer
End Class