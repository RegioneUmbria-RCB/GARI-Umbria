Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.UtilityDatabaseExtension

Imports AgronicaCoreDTOStd.SmartTractors_HubIoT
Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider
Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports Agronica.Helpers.OAuth2
Imports AgronicaCoreModello
Imports AgronicaCoreModelsSTD.Gis

Imports NetTopologySuite.IO.Esri.Shapefiles
Imports NetTopologySuite.Features
Imports AgronicaCoreVarieBIZ

Public Class HubIoT_Helper
    Inherits LogProvider
    Implements IDisposable

    Private _objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    Private _objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
    Private _exportPath As String
    Private _pathLog As String
    Private _fileLog As String
    Private client As New HttpClient()

    Public Sub New(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri, ByVal pathLog As String, ByVal fileLog As String)
        _objParametri_Server = objParametri_Server
        _objParametri_Utenti = objParametri_Utenti
        _pathLog = pathLog
        _fileLog = fileLog

        Dim cfgSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        Dim dtr = cfgSiti.Leggi(0, "GestioneEsportazioni_Repository", "", "", _objParametri_Server)
        If dtr.Rows.Count > 0 Then
            _exportPath = System.IO.Path.Combine(dtr.Rows(0)("Valore"), "HubIoT_Export")
            If Not Directory.Exists(_exportPath) Then
                Directory.CreateDirectory(_exportPath)
            End If
        Else
            Dim customLOGParams As New CustomLOGParams With {
                .LogDescrizioneUtente = _objParametri_Server.UtenteUsername,
                .LogDirectory = _pathLog,
                .LogFileName = _fileLog
            }
            Scrivi_LOG(_objParametri_Server,
                       "AgronicaPrecisionFarming.HubIoT_Helper constructor",
                        "Chiave configurazione siti GestioneEsportazioni_Repository non trovata. Impossibile proseguire",
                       CustomLOGParams:=customLOGParams)

            Throw New Exception("Chiave configurazione siti GestioneEsportazioni_Repository non trovata. Impossibile proseguire")
        End If


    End Sub

    Public Function GetParametriConnessione(ByVal Piva As String) As ConnectionConfig
        Dim obj As ConnectionConfig
        Try
            Dim reader As New AgronicaCoreHubIoTDAL.HubIoT_ParametriConnessioni_R

            Dim dtPar = reader.Leggi(_objParametri_Server.PivaSuperUser, Piva, "", "", _objParametri_Server)
            If dtPar.Rows.Count <= 0 Then

                Throw New Exception(String.Format("Nessun parametro di connessione trovato per la partita iva {0}", Piva))
            Else
                obj = New ConnectionConfig() With {
                        .PivaSuperUser = dtPar.Rows(0)("PivaSuperUser").ToString(),
                        .piva = dtPar.Rows(0)("Piva").ToString(),
                        .pars = JsonConvert.DeserializeObject(Of Parameters)(dtPar.Rows(0)("Parametri").ToString())
                    }
            End If
        Catch ex As Exception
            obj = Nothing
            Dim customLOGParams As New CustomLOGParams With {
                .LogDescrizioneUtente = _objParametri_Server.UtenteUsername,
                .LogDirectory = _pathLog,
                .LogFileName = _fileLog
            }
            Scrivi_LOG(_objParametri_Server,
                       "AgronicaPrecisionFarming.HubIoT_Helper.GetParametriConnessione",
                       $"{ex.Message}{vbCrLf}StackTrace: {ex.StackTrace}",
                       CustomLOGParams:=customLOGParams)

            Throw New Exception(ex.Message, ex)
        End Try
        Return obj
    End Function

    Public Sub SaveParametriConnessione(ByVal data As ConnectionConfig)
        Try
            Dim writer As New AgronicaCoreHubIoTDAL.HubIoT_ParametriConnessioni_W
            Dim reader As New AgronicaCoreHubIoTDAL.HubIoT_ParametriConnessioni_R

            Dim dtPar = reader.Leggi(data.PivaSuperUser, data.piva, "", "", _objParametri_Server)
            If dtPar.Rows.Count <= 0 Then
                writer.Scrivi(data.PivaSuperUser, data.piva, JsonConvert.SerializeObject(data), AGRODATAINIZIO, AGRODATAFINE, _objParametri_Server, DateTime.Now, DateTime.Now, _objParametri_Server.UtenteUsername, _objParametri_Server.UtenteUsername)
            Else
                writer.Modifica(data.PivaSuperUser, data.piva, JsonConvert.SerializeObject(data), AGRODATAINIZIO, AGRODATAFINE, _objParametri_Server)
            End If

        Catch ex As Exception
            Dim customLOGParams As New CustomLOGParams With {
                .LogDescrizioneUtente = _objParametri_Server.UtenteUsername,
                .LogDirectory = _pathLog,
                .LogFileName = _fileLog
            }
            Scrivi_LOG(_objParametri_Server,
                       "AgronicaPrecisionFarming.HubIoT_Helper.SaveParametriConnessione",
                       $"{ex.Message}{vbCrLf}StackTrace: {ex.StackTrace}",
                       CustomLOGParams:=customLOGParams)
            Throw New Exception(ex.Message, ex)
        End Try
    End Sub

    Public Function GetConfigurazione(ByVal Piva As String) As Settings
        Dim obj As Settings
        Try
            Dim reader As New AgronicaCoreHubIoTDAL.HubIoT_Settings_R

            Dim dtPar = reader.Leggi(_objParametri_Server.PivaSuperUser, Piva, "", "", _objParametri_Server)
            If dtPar.Rows.Count <= 0 Then
                dtPar = reader.Leggi(_objParametri_Server.PivaSuperUser, "-1", "", "", _objParametri_Server)
            End If

            If dtPar Is Nothing OrElse dtPar.Rows.Count <= 0 Then
                Throw New Exception(String.Format("Nessun parametro di connessione trovato. Operazione interrotta"))
            Else
                obj = JsonConvert.DeserializeObject(Of Settings)(dtPar.Rows(0)("Parametri").ToString())

            End If
        Catch ex As Exception
            obj = Nothing
            Dim customLOGParams As New CustomLOGParams With {
                .LogDescrizioneUtente = _objParametri_Server.UtenteUsername,
                .LogDirectory = _pathLog,
                .LogFileName = _fileLog
            }
            Scrivi_LOG(_objParametri_Server,
                       "AgronicaPrecisionFarming.HubIoT_Helper.GetConfigurazione",
                       $"{ex.Message}{vbCrLf}StackTrace: {ex.StackTrace}",
                       CustomLOGParams:=customLOGParams)
            Throw New Exception(ex.Message, ex)
        End Try
        Return obj
    End Function

    Public Sub SaveConfigurazione(ByVal PivaSuperUser As String,
                                  ByVal Piva As String,
                                  ByVal data As Settings)
        Try
            Dim writer As New AgronicaCoreHubIoTDAL.HubIoT_Settings_W
            Dim reader As New AgronicaCoreHubIoTDAL.HubIoT_Settings_R

            Dim dtPar = reader.Leggi(PivaSuperUser, Piva, "", "", _objParametri_Server)
            If dtPar.Rows.Count <= 0 Then
                writer.Scrivi(PivaSuperUser, Piva, JsonConvert.SerializeObject(data), AGRODATAINIZIO, AGRODATAFINE, _objParametri_Server, DateTime.Now, DateTime.Now, _objParametri_Server.UtenteUsername, _objParametri_Server.UtenteUsername)
            Else
                writer.Modifica(PivaSuperUser, Piva, JsonConvert.SerializeObject(data), AGRODATAINIZIO, AGRODATAFINE, "", _objParametri_Server)
            End If

        Catch ex As Exception
            Dim customLOGParams As New CustomLOGParams With {
                .LogDescrizioneUtente = _objParametri_Server.UtenteUsername,
                .LogDirectory = _pathLog,
                .LogFileName = _fileLog
            }
            Scrivi_LOG(_objParametri_Server,
                       "AgronicaPrecisionFarming.HubIoT_Helper.SaveConfigurazione",
                       $"{ex.Message}{vbCrLf}StackTrace: {ex.StackTrace}",
                       CustomLOGParams:=customLOGParams)
            Throw New Exception(ex.Message, ex)
        End Try
    End Sub

    Public Function GetElencoOrgIDPerConfigurazione(ByVal PivaSuperUser As String,
                                                    ByVal Piva As String) As List(Of OrganizationIdentity)
        Try

        Catch ex As Exception
            Dim customLOGParams As New CustomLOGParams With {
                .LogDescrizioneUtente = _objParametri_Server.UtenteUsername,
                .LogDirectory = _pathLog,
                .LogFileName = _fileLog
            }
            Scrivi_LOG(_objParametri_Server,
                       "AgronicaPrecisionFarming.HubIoT_Helper.GetElencoOrgIDPerConfigurazione",
                       $"{ex.Message}{vbCrLf}StackTrace: {ex.StackTrace}",
                       CustomLOGParams:=customLOGParams)
            Throw New Exception(ex.Message, ex)
        End Try
    End Function

    Public Function GetWorkOrderDetail(ByVal piva As String,
                                       ByVal VIN As String,
                                       ByVal IdDocumento As Integer,
                                       ByVal IdOperazioneDocumento As Integer,
                                       ByVal orgIDs As List(Of OrganizationIdentity)) As List(Of WorkOrder)



        'If IdRicetta = 0 Then
        '    Throw New Exception("specificare id Ricetta")
        'End If
        'If IdRicettaOperazione = 0 Then
        '    Throw New Exception("specificare id Ricetta Operazione")
        'End If

        Dim obj As New List(Of WorkOrder)
        Try
            Dim reader As New AgronicaCoreHubIoTBIZ.WorkOrderKey_R

            Dim wo_reader As New AgronicaCoreHubIoTDAL.HubIoT_WorkOrderData_R

            Dim keyList = reader.LeggiElencoWorkOrderDaInviare(piva, VIN, IdDocumento, IdOperazioneDocumento, _objParametri_Server)

            Dim idList = keyList.Select(Of String)(Function(x) x.workerOrderId.ToString()).ToList()

            AggiornaStatoWorkOrderKey(idList, TipiEnumerativi.enum_HubIoT_StatoElaborazione.Elaborazione)

            For Each key In keyList
                If key.Mac_Cod <> 0 Then
                    Select Case key.Entita_Origine
                        Case TipiEnumerativi.enum_HubIoT_EntitaOrigine.Ricetta
                            Dim wkList = wo_reader.GetWorkerOrderDetail_RicetteOperazioni(key.PivaSuperUser, key.Piva, key.Id_documento, key.id_operazione_documento, "", "", _objParametri_Server).ToExpandoObject().ToList()
                            For Each wk In wkList
                                Dim mess_err As String = ""
                                If CheckExecutableRowByRule(wk, IIf(key.RegolaElaborazione <> 0, key.RegolaElaborazione, GetWorkOrderRulesByEntitySource(key.Entita_Origine, wk("Lav_Cod_Esterno"))), mess_err) = True Then
                                    CreateWorkOrderRecordFromRicetta(obj, key.workerOrderId.ToString(), wk, orgIDs, key.RegolaElaborazione)
                                Else
                                    ''scrivere log
                                    Dim l As New List(Of String)
                                    l.Add(key.workerOrderId.ToString())
                                    AggiornaStatoWorkOrderKey(l, TipiEnumerativi.enum_HubIoT_StatoElaborazione.Inserito)
                                End If
                            Next
                        Case TipiEnumerativi.enum_HubIoT_EntitaOrigine.Operazione_Pianificata
                            Dim wkList = wo_reader.GetWorkerOrderDetail_OperazioniPianificate(key.PivaSuperUser, key.Piva, key.Id_documento, key.id_operazione_documento, "", "", _objParametri_Server).ToExpandoObject().ToList()
                            For Each wk In wkList
                                Dim mess_err As String = ""
                                If CheckExecutableRowByRule(wk, IIf(key.RegolaElaborazione <> 0, key.RegolaElaborazione, GetWorkOrderRulesByEntitySource(key.Entita_Origine, wk("Lav_Cod_Esterno"))), mess_err) = True Then
                                    CreateWorkOrderRecordFromOperazionePianificata(obj, key.workerOrderId.ToString(), wk, orgIDs, key.RegolaElaborazione)
                                Else
                                    ''scrivere log
                                    Dim l As New List(Of String)
                                    l.Add(key.workerOrderId.ToString())
                                    AggiornaStatoWorkOrderKey(l, 0)
                                End If
                            Next
                        Case TipiEnumerativi.enum_HubIoT_EntitaOrigine.Consiglio_Irriguo
                            'to be soon
                    End Select
                End If
            Next

        Catch ex As Exception
            obj = Nothing
            Dim customLOGParams As New CustomLOGParams With {
                .LogDescrizioneUtente = _objParametri_Server.UtenteUsername,
                .LogDirectory = _pathLog,
                .LogFileName = _fileLog
            }
            Scrivi_LOG(_objParametri_Server,
                       "AgronicaPrecisionFarming.HubIoT_Helper.GetWorkOrderDetail",
                       $"{ex.Message}{vbCrLf}StackTrace: {ex.StackTrace}",
                       CustomLOGParams:=customLOGParams)
            Throw New Exception(ex.Message, ex)
        End Try
        Return obj
    End Function

    Public Sub AggiornaStatoWorkOrderKey(ByVal idList As List(Of String), ByVal stato As Integer)
        Try
            Dim writer As New AgronicaCoreHubIoTBIZ.WorkOrderKey_W
            writer.AggiornaStatoWorkOrderKey(idList, stato, _objParametri_Server)
        Catch ex As Exception
            Dim customLOGParams As New CustomLOGParams With {
                .LogDescrizioneUtente = _objParametri_Server.UtenteUsername,
                .LogDirectory = _pathLog,
                .LogFileName = _fileLog
            }
            Scrivi_LOG(_objParametri_Server,
                       "AgronicaPrecisionFarming.HubIoT_Helper.AggiornaStatoWorkOrderKey",
                       $"{ex.Message}{vbCrLf}StackTrace: {ex.StackTrace}",
                       CustomLOGParams:=customLOGParams)
            Throw New Exception(ex.Message, ex)
        End Try

    End Sub

    Public Shared Function ScriviWorkOrderKeyDaRicettaOperazione(ByVal elenco_ricette As List(Of RicettaOperazione2WorkOrderKey),
                                                                 ByRef objParametri_Server As AgronicaCoreParametri,
                                                                 ByRef objParametri_Utenti As AgronicaCoreParametri) As RispostaStandard
        Dim r As New RispostaStandard
        Try
            Dim writer As New AgronicaCoreHubIoTBIZ.WorkOrderKey_W
            For Each ricetta In elenco_ricette
                If writer.ScriviWorkOrderKeyDaRicetteOperazioni(ricetta.Ricetta, ricetta.RicettaOperazione, ricetta.MacCod, objParametri_Server) = False Then
                    Throw New Exception(String.Format("Errore Integer scrittura workorderkey da ricetta ({0}) \ ricetta operazione ({1}) \ Macchina ({3})", ricetta.Ricetta.ToString(), ricetta.RicettaOperazione.ToString(), ricetta.MacCod.ToString()))
                End If
            Next
            r.RispostaOK = True
            r.RispostaStringa = "Ricette inviate a SmartTractor"
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        End Try
        Return r
    End Function

    Public Shared Function LeggiParametriConnessioni(ByVal elencoPiva As List(Of ParametriConnessioni_In),
                                                     ByRef objParametri_Server As AgronicaCoreParametri,
                                                     ByRef objParametri_Utenti As AgronicaCoreParametri) As rispostaStandard(Of ParametriConnessioni_Out)
        Dim ret As New rispostaStandard(Of ParametriConnessioni_Out)
        Try
            Dim reader As New AgronicaCoreHubIoTDAL.HubIoT_ParametriConnessioni_R
            Dim dtPar As DataTable
            If elencoPiva.Count <= 0 OrElse elencoPiva Is Nothing Then
                dtPar = reader.Leggi(objParametri_Server.PivaSuperUser, "", "", "", objParametri_Server)
            Else
                Dim lst As New List(Of String)
                For Each piva In elencoPiva
                    lst.Add(piva.piva)
                Next
                dtPar = reader.LeggiElenco(objParametri_Server.PivaSuperUser, lst, "", "", objParametri_Server)
            End If
            ret.RispostaStringa = New ParametriConnessioni_Out
            For Each par In dtPar.Rows
                ret.RispostaStringa.pars.Add(New ConnectionConfig() With {
                                                    .PivaSuperUser = par("PivaSuperUser").ToString(),
                                                    .piva = par("Piva").ToString(),
                                                    .pars = JsonConvert.DeserializeObject(Of Parameters)(par("Parametri").ToString())
                                                })
            Next
            ret.RispostaOK = True
        Catch ex As Exception
            ret = Nothing
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Public Shared Function SalvaParametriConnessioni(ByVal elencoConnessionni As List(Of SalvaParametriConnessioni),
                                                     ByRef objParametri_Server As AgronicaCoreParametri,
                                                     ByRef objParametri_Utenti As AgronicaCoreParametri) As RispostaStandard
        Dim ret As New RispostaStandard
        Try
            Dim writer As New AgronicaCoreHubIoTDAL.HubIoT_ParametriConnessioni_W
            Dim reader As New AgronicaCoreHubIoTDAL.HubIoT_ParametriConnessioni_R
            For Each conn In elencoConnessionni
                If conn.delete = True Then
                    writer.Cancella(conn.pars.PivaSuperUser, conn.pars.piva, objParametri_Server)
                Else
                    Dim dtPar = reader.Leggi(conn.pars.PivaSuperUser, conn.pars.piva, "", "", objParametri_Server)
                    If dtPar.Rows.Count <= 0 Then
                        writer.Scrivi(conn.pars.PivaSuperUser, conn.pars.piva, JsonConvert.SerializeObject(conn.pars.pars), AGRODATAINIZIO, AGRODATAFINE, objParametri_Server, DateTime.Now, DateTime.Now, objParametri_Server.UtenteUsername, objParametri_Server.UtenteUsername)
                    Else
                        writer.Modifica(conn.pars.PivaSuperUser, conn.pars.piva, JsonConvert.SerializeObject(conn.pars.pars), AGRODATAINIZIO, AGRODATAFINE, objParametri_Server)
                    End If
                End If
            Next
            ret.RispostaOK = True
            ret.RispostaStringa = "Parametri connessioni salvati correttamente"
        Catch ex As Exception
            ret = Nothing
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Public Shared Function GetLogInvioRicettaOperazione(ByVal elenco_ricette As List(Of RicettaOperazione2WorkOrderKey),
                                                                 ByRef objParametri_Server As AgronicaCoreParametri,
                                                                 ByRef objParametri_Utenti As AgronicaCoreParametri) As RispostaStandard
        Dim r As New RispostaStandard
        Try
            Dim reader As New AgronicaCoreHubIoTDAL.HubIoT_LogInvio_R

        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        End Try
        Return r
    End Function

#Region "Chiamate ad HubIoT"

    Public Function getOAuth2Metadata(ByVal wellKnownUri As String) As OAuth2_Metadata
        Dim ret As OAuth2_Metadata
        If wellKnownUri = "" Then
            Throw New Exception("Link di lettura metadati oauth2 non valorizzato. impossibile proseguire")
        End If

        Try
            Dim request As New HttpRequestMessage(HttpMethod.Get, wellKnownUri)
            Dim resp As HttpResponseMessage = client.SendAsync(request).GetAwaiter().GetResult()
            If resp.StatusCode = HttpStatusCode.OK Then
                Dim r = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult()
                ret = JsonConvert.DeserializeObject(Of OAuth2_Metadata)(r)
            Else
                Throw New Exception("[Error] getOAuth2Metadata - Response: " + resp.StatusCode.ToString() + " ")
            End If
        Catch ex As Exception
            ret = Nothing
            Dim customLOGParams As New CustomLOGParams With {
                .LogDescrizioneUtente = _objParametri_Server.UtenteUsername,
                .LogDirectory = _pathLog,
                .LogFileName = _fileLog
            }
            Scrivi_LOG(_objParametri_Server,
                       "AgronicaPrecisionFarming.HubIoT_Helper.getOAuth2Metadata",
                       $"{ex.Message}{vbCrLf}StackTrace: {ex.StackTrace}",
                       CustomLOGParams:=customLOGParams)
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Public Function getAccessToken(ByVal uri As String, ByVal cfg As ConnectionParameters) As OAuth2Token
        Dim ret As OAuth2Token

        If cfg Is Nothing Then
            Throw New Exception("Parametri di connessione non valorizzati")
        End If
        If cfg.client_id = "" OrElse cfg.client_secret = "" Then
            Throw New Exception("Client_ID o Client_Secret non valorizzati. Verificare parametri di connessione")
        End If

        If cfg.grant_type = "password" Then
            If cfg.username = "" OrElse cfg.password = "" Then
                Throw New Exception("Specificare username e password.")
            End If
        End If

        Try

            Dim HttpQueryParams = New Dictionary(Of String, String)

            HttpQueryParams.Add("grant_type", cfg.grant_type)
            HttpQueryParams.Add("client_id", cfg.client_id)
            HttpQueryParams.Add("client_secret", cfg.client_secret)

            If cfg.grant_type = "password" Then
                HttpQueryParams.Add("username", cfg.username)
                HttpQueryParams.Add("password", cfg.password)
            End If

            If cfg.scope <> "" Then
                HttpQueryParams.Add("scope", cfg.scope)
            End If

            Dim request As New HttpRequestMessage(HttpMethod.Post, uri)
            request.Content = New FormUrlEncodedContent(HttpQueryParams)

            Dim resp As HttpResponseMessage = client.SendAsync(request).GetAwaiter().GetResult()
            If resp.StatusCode = HttpStatusCode.OK Then
                Dim r = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult()
                ret = JsonConvert.DeserializeObject(Of OAuth2Token)(r)
            Else
                Dim r = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult()
                Dim err = JsonConvert.DeserializeObject(Of OAuth2Error)(r)

                Throw New Exception("[Error] GetAccessToken - Response: " + resp.StatusCode.ToString() + " - error: " + err.error_s + " / " + err.error_description)
            End If
        Catch ex As Exception
            ret = Nothing
            Dim customLOGParams As New CustomLOGParams With {
                .LogDescrizioneUtente = _objParametri_Server.UtenteUsername,
                .LogDirectory = _pathLog,
                .LogFileName = _fileLog
            }
            Scrivi_LOG(_objParametri_Server,
                       "AgronicaPrecisionFarming.HubIoT_Helper.getAccessToken",
                       $"{ex.Message}{vbCrLf}StackTrace: {ex.StackTrace}",
                       CustomLOGParams:=customLOGParams)
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Public Function getUserToken(ByVal uri As String, ByVal token As OAuth2Token) As List(Of UserToken)
        Dim ret As List(Of UserToken)
        If token Is Nothing Then
            Throw New Exception("Token non valorizzato. Impossibile proseguire")
        End If

        Try
            client.DefaultRequestHeaders.Remove("Authorization")
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.access_token}")
            Dim uriReuqest = uri + "/User/getUserToken"

            Dim request As New HttpRequestMessage(HttpMethod.Get, uriReuqest)

            Dim resp As HttpResponseMessage = client.SendAsync(request).GetAwaiter().GetResult()
            If resp.StatusCode = HttpStatusCode.OK Then
                Dim r = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult()
                ret = JsonConvert.DeserializeObject(Of List(Of UserToken))(r)

            Else
                Throw New Exception("[Error] getUserToken - Response: " + resp.StatusCode.ToString())
            End If
        Catch ex As Exception
            ret = Nothing
            Dim customLOGParams As New CustomLOGParams With {
                .LogDescrizioneUtente = _objParametri_Server.UtenteUsername,
                .LogDirectory = _pathLog,
                .LogFileName = _fileLog
            }
            Scrivi_LOG(_objParametri_Server,
                       "AgronicaPrecisionFarming.HubIoT_Helper.getUserToken",
                       $"{ex.Message}{vbCrLf}StackTrace: {ex.StackTrace}",
                       CustomLOGParams:=customLOGParams)
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Public Function SendPrescriptionFile(ByVal uri As String,
                                    ByVal token As OAuth2Token,
                                    ByVal platform_token As UserToken,
                                    ByVal workorder As WorkOrder,
                                    ByVal piva As String) As Boolean
        Dim ret As Boolean = False
        Try
            Dim logInvio As New AgronicaCoreHubIoTDAL.HubIoT_LogInvio_W

            client.DefaultRequestHeaders.Remove("Authorization")
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.access_token}")

            Dim uriReuqest = uri + "/Files/SendPrescriptionFile"

            Dim queryStringParams As List(Of String) = New List(Of String)
            queryStringParams.Add("orgId=" + workorder.orgId + "")
            queryStringParams.Add("vin=" + workorder.vin + "")
            queryStringParams.Add("workOrderId=" + workorder.workOrderId + "")
            queryStringParams.Add("keyFileName=" + workorder.keyFileName + "")

            Using formData As New MultipartFormDataContent("------------" + Guid.NewGuid().ToString())
                Select Case workorder.rule
                    Case TipiEnumerativi.enum_HubIoT_RegoleElaborazione.None
                        'to be
                    Case TipiEnumerativi.enum_HubIoT_RegoleElaborazione.InviaPoligonoSenzaShape
                        'to be
                    Case TipiEnumerativi.enum_HubIoT_RegoleElaborazione.InviaPoligonoSuShape

                    Case TipiEnumerativi.enum_HubIoT_RegoleElaborazione.InviaMappaDiPrescrizione, TipiEnumerativi.enum_HubIoT_RegoleElaborazione.InviaMappaDiPrescrizioneConLineaGuida
                        Select Case workorder.content.prescriptionMap.pType
                            Case PrescriptionMapType.Undefined
                                Throw New Exception("Tipo mappa di prescrizione non definito. Impossibile proseguire")
                            Case PrescriptionMapType.XML, PrescriptionMapType.JSON
                                Dim fpath = CreateZIPPrescriptionMap(piva, workorder.content.prescriptionMap.pType, workorder.content.prescriptionMap.content, workorder.keyFileName, workorder.content.setup, workorder.rule)
                                Using fs As FileStream = File.OpenRead(fpath)
                                    Using sc As New StreamContent(fs)
                                        Using fc As New ByteArrayContent(sc.ReadAsByteArrayAsync().GetAwaiter().GetResult())
                                            fc.Headers.ContentType = New Headers.MediaTypeHeaderValue("application/zip")
                                            formData.Add(fc, "file", Path.GetFileName(fpath))

                                            Dim req As New HttpRequestMessage()
                                            req.Method = HttpMethod.Post
                                            req.RequestUri = New Uri(uriReuqest + "?" + String.Join("&", queryStringParams))
                                            req.Headers.Add("platform_token", platform_token.token.Replace("\\u002B", ""))
                                            req.Content = formData

                                            logInvio.Scrivi(workorder.workOrderId, fpath, DateTime.Now, "", "", AGRODATAINIZIO, "", _objParametri_Server)

                                            Using resp As HttpResponseMessage = client.SendAsync(req).GetAwaiter().GetResult()
                                                If resp.StatusCode <> HttpStatusCode.OK Then
                                                    Dim msgRet As String = "[Error] SendPrescriptionFile - Response: " + resp.StatusCode.ToString() + vbCrLf + "Message: " + resp.Content.ReadAsStringAsync().GetAwaiter().GetResult()
                                                    logInvio.Modifica(workorder.workOrderId, "", AGRODATAINIZIO, msgRet, "", AGRODATAINIZIO, "", "", _objParametri_Server)
                                                    Throw New Exception(msgRet)
                                                Else
                                                    logInvio.Modifica(workorder.workOrderId, "", AGRODATAINIZIO, "[OK] Invio eseguito correttamente", "", AGRODATAINIZIO, "", "", _objParametri_Server)
                                                    ret = True
                                                End If
                                            End Using
                                        End Using
                                    End Using
                                End Using
                        End Select
                End Select
            End Using

        Catch ex As Exception
            ret = False
            Dim customLOGParams As New CustomLOGParams With {
                .LogDescrizioneUtente = _objParametri_Server.UtenteUsername,
                .LogDirectory = _pathLog,
                .LogFileName = _fileLog
            }
            Scrivi_LOG(_objParametri_Server,
                       "AgronicaPrecisionFarming.HubIoT_Helper.SendPrescriptionFile",
                       $"{ex.Message}{vbCrLf}StackTrace: {ex.StackTrace}",
                       CustomLOGParams:=customLOGParams)
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

#End Region

#Region "internal"
    Private Sub CreateWorkOrderRecordFromRicetta(ByRef objlist As List(Of WorkOrder),
                                                 ByVal key As String,
                                                 ByVal wk As IDictionary(Of String, Object),
                                                 ByVal orgIdList As List(Of OrganizationIdentity),
                                                 ByVal RegolaElaborazione As TipiEnumerativi.enum_HubIoT_RegoleElaborazione)
        Dim newObj As New WorkOrder
        newObj.Platform = IIf(wk("Platform") Is DBNull.Value, 0, wk("Platform"))
        newObj.orgId = GetorgIDByPlatformDestination(IIf(wk("Platform") Is DBNull.Value, 0, wk("Platform")), wk("Piva"), orgIdList)
        newObj.workOrderId = key
        newObj.vin = wk("VIN")
        newObj.keyFileName = key
        newObj.rule = RegolaElaborazione

        Dim content As New PrescriptionContent
        content.prescriptionMap = GetPrescriptionMapData(wk("Ricetta_Operazione_Cod"), wk("Piva"), wk("Sa_Cod"), wk("Appezza"), wk("Id_Reg"), RegolaElaborazione)

        Dim setup As New SetupPrescription
        setup.workOrderDate = CType(wk("Validita_Inizio"), DateTime).ToString("yyyy-MM-ddTHH:mm:ssZ")
        setup.operationType = wk("CodiceOperazione")
        setup.rateName = "CAP_N+"
        setup.rateUnit = GetRateUnit(TipiEnumerativi.enum_HubIoT_EntitaOrigine.Ricetta, wk("Udm_Cod"), wk("Udm_Cod_Extra"), wk("Mezzo_Det"))     '"Ha",
        setup.client = New Client() With {
                            .id = wk("Piva"),
                            .name = CheckStringLength(wk("rag_soc").ToString(), 32)
                        }
        setup.farm = New Farm(setup.client) With {
                            .id = wk("Sa_Cod"),
                            .name = CheckStringLength(wk("Sa_nome").ToString(), 32)
                        }
        setup.field = New Field(setup.client, setup.farm) With {
                            .id = wk("Piva").ToString() + "|" + wk("Sa_Cod").ToString() + "|" + wk("Appezza").ToString() + "|" + wk("Id_Reg").ToString(),
                            .name = CheckStringLength(wk("FieldName").ToString(), 32)
                        }
        setup.boundary = New Boundary(setup.field) With {
                                    .active = True,
                                    .archived = False,
                                    .irrigated = False,
                                    .id = wk("Piva").ToString() + "|" + wk("Entita_Cod").ToString(),
                                    .name = "Boundary - " + wk("FieldName"),
                                    .sourceType = "External",
                                    .multipolygons = GetBoundaryPolygon(wk("Piva").ToString(), wk("Sa_Cod"), wk("Appezza"), wk("Id_Reg"))
                                    }
        setup.chemical = IIf(setup.operationType = "Application" AndAlso wk("CategoriaMagazzinoProdotti") = 191,
                                    New Chemical() With {
                                        .id = wk("CategoriaMagazzinoProdotti").ToString() + "|" + wk("Mat_Cod").ToString(),
                                        .name = GetProDesc(wk("CategoriaMagazzinoProdotti"), wk("Mat_Cod")),   ' "descrizione prodotto da prendere".Substring(1, 32),
                                        .companyName = "---",
                                        .materialClassification = GetMaterialClasification(wk("Udm_Cod")),  ' "DRY",  'LIQUID, DRY, GAS
                                        .type = GetChemicalType(wk("Mat_Cod")),   'ADDITIVE, FUNGICIDE, INSECTICIDE, HERBICIDE, GROWTH_REGULATOR, NITROGEN_STABILIZER
                                        .carrier = False
                                            },
                                    Nothing
                                    )
        setup.fertilizer = IIf(setup.operationType = "Application" AndAlso wk("CategoriaMagazzinoProdotti") = 3,
                                    New Fertilizer() With {
                                        .id = wk("CategoriaMagazzinoProdotti").ToString() + "|" + wk("Mat_Cod").ToString(),
                                        .name = GetProDesc(wk("CategoriaMagazzinoProdotti"), wk("Mat_Cod")),    ' "descrizione prodotto da prendere".Substring(1, 32),
                                        .companyName = "---",
                                        .materialClassification = GetMaterialClasification(wk("Udm_Cod")),  ' "DRY",  'LIQUID, DRY, GAS 
                                        .type = GetFertilizerType(wk("Mat_Cod")),
                                        .carrier = False
                                            },
                                    Nothing
                                    )
        setup.guidanceLine = Nothing
        setup.tankMix = Nothing
        setup.variety = IIf(setup.operationType = "Seeding" AndAlso wk("CategoriaMagazzinoProdotti") = 10,
                                    New Variety() With {
                                        .id = wk("CategoriaMagazzinoProdotti").ToString() + "|" + wk("Mat_Cod").ToString(),
                                        .name = GetProDesc(wk("CategoriaMagazzinoProdotti"), wk("Mat_Cod")),  '"descrizione prodotto da prendere".Substring(1, 32),
                                        .companyName = "---",
                                        .cropName = GetExternalCodificaSpecieVegetale(wk("Veg_Cod"), wk("Cul_Cod"))      'da recuperare da tabella di metaschema
                                            },
                                    Nothing
                                    )

        content.setup = setup
        newObj.content = content
        objlist.Add(newObj)

    End Sub

    Private Sub CreateWorkOrderRecordFromOperazionePianificata(ByRef objlist As List(Of WorkOrder),
                                                               ByVal key As String,
                                                               ByVal wk As IDictionary(Of String, Object),
                                                               ByVal orgIdList As List(Of OrganizationIdentity),
                                                               ByVal RegolaElaborazione As TipiEnumerativi.enum_HubIoT_RegoleElaborazione)
        objlist.Add(New WorkOrder() With {
            .Platform = IIf(wk("Platform") Is DBNull.Value, 0, wk("Platform")),
            .orgId = GetorgIDByPlatformDestination(IIf(wk("Platform") Is DBNull.Value, 0, wk("Platform")), wk("piva"), orgIdList),
            .workOrderId = key,
            .vin = wk("VIN"),
            .keyFileName = key,
            .rule = RegolaElaborazione,
            .content = New PrescriptionContent() With {
                .prescriptionMap = New PrescriptionMap() With {.pType = PrescriptionMapType.Undefined, .content = ""},                           'GetXmlPrescriptionMap(wk("Ricetta_Operazione_Cod"), wk("PIVA"), wk("sa_Cod"), wk("appezza"), wk("id_reg"), RegolaElaborazione),
                .setup = New SetupPrescription() With {
                    .workOrderDate = CType(wk("Validita_Inizio"), DateTime).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    .operationType = wk("CodiceOperazione"),
                    .rateName = "RATE",
                    .rateUnit = GetRateUnit(TipiEnumerativi.enum_HubIoT_EntitaOrigine.Operazione_Pianificata, wk("Uom_Cod"), wk("Udm_Cod_Extra"), wk("Mezzo_Det")), ' "Ha",
                    .client = New Client() With {
                            .id = wk("PIVA"),
                            .name = CheckStringLength(wk("rag_soc").ToString(), 32)
                        },
                    .farm = New Farm(wk("PIVA")) With {
                            .id = wk("sa_cod"),
                            .name = CheckStringLength(wk("Sa_nome").ToString(), 32)
                        },
                    .field = New Field(wk("PIVA"), wk("sa_cod")) With {
                            .id = wk("PIVA").ToString() + "|" + wk("sa_cod").ToString() + "|" + wk("Appezza").ToString() + "|" + wk("id_reg").ToString(),
                            .name = CheckStringLength(wk("FieldName").ToString(), 32)
                        },
                    .boundary = New Boundary(.field) With {
                                    .active = True,
                                    .archived = False,
                                    .irrigated = False,
                                    .id = wk("PIVA").ToString() + "|" + wk("Entita_Cod").ToString(),
                                    .name = "Boundary - " + wk("FieldName"),
                                    .sourceType = "External",
                                    .multipolygons = GetBoundaryPolygon(wk("PIVA").ToString(), wk("sa_cod"), wk("Appezza"), wk("id_reg"))
                                    },
                    .chemical = IIf(.operationType = "Application" AndAlso wk("Elem_Cod") = 191,
                                    New Chemical() With {
                                        .id = wk("CategoriaMagazzinoProdotti").ToString() + "|" + wk("Mat_Cod").ToString(),
                                        .name = GetProDesc(wk("elem_cod"), wk("mat_cod")),   ' "descrizione prodotto da prendere".Substring(1, 32),
                                        .companyName = "---",
                                        .materialClassification = GetMaterialClasification(wk("Uom_Cod")),  ' "DRY",  'LIQUID, DRY, GAS
                                        .type = GetChemicalType(wk("mat_cod")),   'ADDITIVE, FUNGICIDE, INSECTICIDE, HERBICIDE, GROWTH_REGULATOR, NITROGEN_STABILIZER
                                        .carrier = False
                                            },
                                    Nothing
                                    ),
                    .fertilizer = IIf(.operationType = "Application" AndAlso wk("Elem_Cod") = 3,
                                    New Fertilizer() With {
                                        .id = wk("CategoriaMagazzinoProdotti").ToString() + "|" + wk("Mat_Cod").ToString(),
                                        .name = GetProDesc(wk("elem_cod"), wk("mat_cod")),    ' "descrizione prodotto da prendere".Substring(1, 32),
                                        .companyName = "---",
                                        .materialClassification = GetMaterialClasification(wk("Uom_Cod")),  ' "DRY",  'LIQUID, DRY, GAS 
                                        .type = GetFertilizerType(wk("mat_cod")),
                                        .carrier = False
                                            },
                                    Nothing
                                    ),
                    .guidanceLine = Nothing,
                    .tankMix = Nothing,
                    .variety = IIf(.operationType = "Seeding" AndAlso wk("Elem_Cod") = 10,
                                    New Variety() With {
                                        .id = wk("CategoriaMagazzinoProdotti").ToString() + "|" + wk("Mat_Cod").ToString(),
                                        .name = GetProDesc(wk("elem_cod"), wk("mat_cod")),  '"descrizione prodotto da prendere".Substring(1, 32),
                                        .companyName = "---",
                                        .cropName = GetExternalCodificaSpecieVegetale(wk("Veg_Cod"), wk("Cul_Cod"))      'da recuperare da tabella di metaschema
                                            },
                                    Nothing
                                    )
                            }
                }
            }
        )
    End Sub
    Private Sub CreateWorkOrderRecordFromConsiglioIrriguo(ByRef objlist As List(Of WorkOrder),
                                                          ByVal key As String,
                                                          ByVal wk As IDictionary(Of String, Object),
                                                          ByVal orgIdList As List(Of OrganizationIdentity),
                                                          ByVal RegolaElaborazione As TipiEnumerativi.enum_HubIoT_RegoleElaborazione)
        Throw New NotImplementedException()
    End Sub

    Private Function CheckStringLength(ByVal oriString As String, ByVal len As Integer) As String
        Dim ret As String = oriString
        Try
            If len > 0 Then
                If oriString.Length > len Then
                    ret = oriString.Substring(1, len)
                End If
            End If
        Catch ex As Exception
            ret = ""
            Dim customLOGParams As New CustomLOGParams With {
                .LogDescrizioneUtente = _objParametri_Server.UtenteUsername,
                .LogDirectory = _pathLog,
                .LogFileName = _fileLog
            }
            Scrivi_LOG(_objParametri_Server,
                       "AgronicaPrecisionFarming.HubIoT_Helper.CheckStringLength",
                       $"{ex.Message}{vbCrLf}StackTrace: {ex.StackTrace}",
                       CustomLOGParams:=customLOGParams)
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Private Function GetRateUnit(ByVal EntitaOrigine As Integer,
                                 ByVal Udm_Cod As Integer,
                                 ByVal Modalita_Distribuzione As Integer,
                                 ByVal Mezzo As Integer) As String
        Dim ret As String = ""
        Try
            Dim convR As New AgronicaCoreHubIoTDAL.HubIoT_UnitaMisuraConversione_R
            Dim res = convR.Leggi(0, EntitaOrigine, Udm_Cod, Modalita_Distribuzione, Mezzo, "", "", _objParametri_Server)
            If res.Rows.Count > 0 Then
                ret = res.Rows(0)("RateUnit").ToString()
            End If
        Catch ex As Exception
            ret = ""
            Dim customLOGParams As New CustomLOGParams With {
                .LogDescrizioneUtente = _objParametri_Server.UtenteUsername,
                .LogDirectory = _pathLog,
                .LogFileName = _fileLog
            }
            Scrivi_LOG(_objParametri_Server,
                       "AgronicaPrecisionFarming.HubIoT_Helper.GetRateUnit",
                       $"{ex.Message}{vbCrLf}StackTrace: {ex.StackTrace}",
                       CustomLOGParams:=customLOGParams)
        End Try
        Return ret
    End Function

    Private Function GetMaterialClasification(ByVal uomcod As Integer) As String
        Dim ret As String = "DRY"
        Try

        Catch ex As Exception
            ret = ""
            Dim customLOGParams As New CustomLOGParams With {
                .LogDescrizioneUtente = _objParametri_Server.UtenteUsername,
                .LogDirectory = _pathLog,
                .LogFileName = _fileLog
            }
            Scrivi_LOG(_objParametri_Server,
                       "AgronicaPrecisionFarming.HubIoT_Helper.GetMaterialClasification",
                       $"{ex.Message}{vbCrLf}StackTrace: {ex.StackTrace}",
                       CustomLOGParams:=customLOGParams)
        End Try
        Return ret
    End Function

    Private Function GetChemicalType(ByVal fer_cod As Integer) As String
        Dim ret As String = "ADDITIVE"
        Try
            Dim formR As New AgronicaCoreMetaSchemaDAL.FormulatixClassifica_R
            Dim resDT = formR.Leggi(fer_cod, 0, AGRODATAINIZIO, AGRODATAFINE, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", _objParametri_Server)
            For Each row In resDT.Rows
                If Not row("Class_Des").ToString.ToLower.Contains("non usare") Then
                    If row("Class_Des").ToString.ToLower.Contains("insetti") Then
                        ret = "INSECTICIDE"
                        Exit For
                    End If
                    If row("Class_Des").ToString.ToLower.Contains("fungi") Then
                        ret = "FUNGICIDE"
                        Exit For
                    End If
                    If row("Class_Des").ToString.ToLower.Contains("diserbante") Then
                        ret = "HERBICIDE"
                        Exit For
                    End If
                    If row("Class_Des").ToString.ToLower.Contains("regolatore") Then
                        ret = "GROWTH_REGULATOR"
                        Exit For
                    End If
                    If row("Class_Des").ToString.ToLower.Contains("azoto") Then
                        ret = "NITROGEN_STABILIZER"
                        Exit For
                    End If
                End If

            Next

        Catch ex As Exception
            ret = "ADDITIVE"
            Dim customLOGParams As New CustomLOGParams With {
                .LogDescrizioneUtente = _objParametri_Server.UtenteUsername,
                .LogDirectory = _pathLog,
                .LogFileName = _fileLog
            }
            Scrivi_LOG(_objParametri_Server,
                       "AgronicaPrecisionFarming.HubIoT_Helper.GetChemicalType",
                       $"{ex.Message}{vbCrLf}StackTrace: {ex.StackTrace}",
                       CustomLOGParams:=customLOGParams)
        End Try
        Return ret
    End Function

    Private Function GetFertilizerType(ByVal pro_cod As Integer) As String
        Dim ret As String = "FERTILIZER"
        Try
            Dim objFertilizzantiXTipologie As New AgronicaCoreMetaSchemaDAL.FertilizzantixTipologie_R
            Dim DT = objFertilizzantiXTipologie.Leggi(0, 0, pro_cod, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", _objParametri_Server)

            If DT.Rows.Count > 0 Then
                For Each row As DataRow In DT.Rows
                    Select Case row.Item("TP_COD")
                        Case 6, 7, 8
                            ret = "MANURE"
                            Exit For
                        Case Else
                            ret = "FERTILIZER"
                    End Select
                Next
            End If

        Catch ex As Exception
            ret = "FERTILIZER"
            Dim customLOGParams As New CustomLOGParams With {
                .LogDescrizioneUtente = _objParametri_Server.UtenteUsername,
                .LogDirectory = _pathLog,
                .LogFileName = _fileLog
            }
            Scrivi_LOG(_objParametri_Server,
                       "AgronicaPrecisionFarming.HubIoT_Helper.GetFertilizerType",
                       $"{ex.Message}{vbCrLf}StackTrace: {ex.StackTrace}",
                       CustomLOGParams:=customLOGParams)
        End Try
        Return ret
    End Function

    Private Function GetExternalCodificaSpecieVegetale(ByVal Veg_Cod As Integer,
                                                       ByVal Cul_Cod As Integer) As String
        Dim ret As String = ""
        Try
            Dim reader As New AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_SistemiEsterni_R

            '1- cerco per specie e varietà
            '2- cerco per specie
            '3- no crop (default)

            Dim dtret = reader.Leggi(0, 3, "", "", Veg_Cod, Cul_Cod, 0, 0, 0, 0, 0, 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, "", "", _objParametri_Server)
            If dtret.Rows.Count <= 0 Then
                dtret = reader.Leggi(0, 3, "", "", Veg_Cod, 0, 0, 0, 0, 0, 0, 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, "", "", _objParametri_Server)
                If dtret.Rows.Count <= 0 Then
                    dtret = reader.Leggi(0, 3, "", "", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, "", "", _objParametri_Server)
                    If dtret.Rows.Count <= 0 Then
                        Throw New Exception("Nessuna codifica esterna trovata per la specie vegetale.")
                    End If
                End If
            End If
            If dtret.Rows(0)("Cul_Des_Esterno").ToString() = "" Then
                ret = dtret.Rows(0)("Veg_Des_Esterno").ToString()
            Else
                ret = dtret.Rows(0)("Cul_Des_Esterno").ToString()
            End If

        Catch ex As Exception
            ret = ""
            Dim customLOGParams As New CustomLOGParams With {
                .LogDescrizioneUtente = _objParametri_Server.UtenteUsername,
                .LogDirectory = _pathLog,
                .LogFileName = _fileLog
            }
            Scrivi_LOG(_objParametri_Server,
                       "AgronicaPrecisionFarming.HubIoT_Helper.GetExternalCodificaSpecieVegetale",
                       $"{ex.Message}{vbCrLf}StackTrace: {ex.StackTrace}",
                       CustomLOGParams:=customLOGParams)
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Private Function GetorgIDByPlatformDestination(ByVal platform As TipiEnumerativi.enum_HubIoTPlatformDestination,
                                                   ByVal piva As String,
                                                   ByVal orgIdList As List(Of OrganizationIdentity)) As String

        Dim orgIdRec = orgIdList.Where(Function(x) x.piva = piva AndAlso x.platform = platform).FirstOrDefault()
        If orgIdRec Is Nothing Then
            Throw New Exception($"OrgID non trovato per la piva {piva} e platform {platform.ToString()}")
        End If

        Return IIf(orgIdRec.orgIdType = OrganizationIdentityMode.CustomerId, orgIdRec.piva, orgIdRec.orgID)

    End Function
    Private Function GetBoundaryPolygon(ByVal piva As String,
                                        ByVal sa_cod As Integer,
                                        ByVal appezza As Integer,
                                        ByVal id_reg As Integer) As List(Of Polygon)
        Dim ret As List(Of Polygon) = Nothing
        Try
            Dim gisElemGrafici As New AgronicaCoreGisDAL.GIS_ElementiGrafici_R

            Dim dtElem = gisElemGrafici.LeggiWKTDaChiaveImpianto(piva, sa_cod, appezza, id_reg, "", "", _objParametri_Server)
            If dtElem.Rows.Count <= 0 Then
                Throw New Exception("Nessun record dato relativo al poligono dell'impianto trovato, impossibile proseguire")
            End If

            Dim wkts As String = IIf(dtElem.Rows(0)("wkt") Is DBNull.Value, "", dtElem.Rows(0)("wkt"))
            If wkts = "" Then
                Throw New Exception("Stringa WKT del poligono impianto non valorizzata, impossibile proseguire")
            End If

            Dim points As New List(Of Point)

            Dim geojson = New SqlSpatialConverter(Of WktParser)(wkts).ToGeoJson()

            Select Case geojson.type
                Case FeatureType.Point.ToString()
                    Dim p As Double() = CType(geojson.coordinates, Double())
                    points.Add(New Point() With {
                            .lat = p(0),
                            .lon = p(1)
                           })

                Case FeatureType.MultiPoint.ToString()
                    Dim p As Double()() = CType(geojson.coordinates, Double()())
                    For Each subp In p
                        points.Add(New Point() With {
                            .lat = subp(0),
                            .lon = subp(1)
                           })
                    Next

                Case FeatureType.LineString.ToString()
                    Dim p As Double()() = CType(geojson.coordinates, Double()())
                    For Each subp In p
                        points.Add(New Point() With {
                            .lat = subp(0),
                            .lon = subp(1)
                           })
                    Next
                Case FeatureType.MultiLineString.ToString()
                    Dim p As Double()()() = CType(geojson.coordinates, Double()()())
                    For Each subp1 In p
                        For Each subp11 In subp1
                            points.Add(New Point() With {
                                            .lat = subp11(0),
                                            .lon = subp11(1)
                                           })
                        Next
                    Next
                Case FeatureType.Polygon.ToString()
                    Dim p As Double()()() = CType(geojson.coordinates, Double()()())
                    For Each subp1 In p
                        For Each subp11 In subp1
                            points.Add(New Point() With {
                                            .lat = subp11(0),
                                            .lon = subp11(1)
                                           })
                        Next
                    Next
                Case FeatureType.MultiPolygon.ToString()
                    Dim p As Double()()()() = CType(geojson.coordinates, Double()()()())
                    For Each subp1 In p
                        For Each subp11 In subp1
                            For Each subp111 In subp11
                                points.Add(New Point() With {
                                            .lat = subp111(0),
                                            .lon = subp111(1)
                                           })
                            Next
                        Next
                    Next
            End Select

            Dim rings = New List(Of Ring)
            rings.Add(New Ring() With {
                        .points = points
                      })


            ret = New List(Of Polygon)
            ret.Add(New Polygon() With {
                    .rings = rings
                })

        Catch ex As Exception
            ret = Nothing
            Dim customLOGParams As New CustomLOGParams With {
                .LogDescrizioneUtente = _objParametri_Server.UtenteUsername,
                .LogDirectory = _pathLog,
                .LogFileName = _fileLog
            }
            Scrivi_LOG(_objParametri_Server,
                       "AgronicaPrecisionFarming.HubIoT_Helper.GetBoundaryPolygon",
                       $"{ex.Message}{vbCrLf}StackTrace: {ex.StackTrace}",
                       CustomLOGParams:=customLOGParams)
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Private Function AggiornaFlagInvioHubIot(ByVal IdRicetta As Integer,
                                             ByVal IdRicettaOperazione As Integer,
                                             ByVal Flag As Integer) As Boolean
        Dim ret As Boolean = False
        Try
            Dim rOpeW As New AgronicaCoreHubIoTDAL.HubIoT_WorkOrderData_W

            Return rOpeW.Aggiorna_RicettaOperazione_FlagInvio_HubIot(IdRicetta, IdRicettaOperazione, Flag, "", _objParametri_Server)

        Catch ex As Exception
            ret = False
            Dim customLOGParams As New CustomLOGParams With {
                .LogDescrizioneUtente = _objParametri_Server.UtenteUsername,
                .LogDirectory = _pathLog,
                .LogFileName = _fileLog
            }
            Scrivi_LOG(_objParametri_Server,
                       "AgronicaPrecisionFarming.HubIoT_Helper.AggiornaFlagInvioHubIot",
                       $"{ex.Message}{vbCrLf}StackTrace: {ex.StackTrace}",
                       CustomLOGParams:=customLOGParams)
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Private Function GetPrescriptionMapData(ByVal IdRicettaOperazione As Integer,
                                          ByVal piva As String,
                                          ByVal sa_Cod As Integer,
                                          ByVal appezza As Integer,
                                          ByVal id_reg As Integer,
                                          ByVal RegolaElaborazione As Integer) As PrescriptionMap
        Dim ret As New PrescriptionMap()
        Try
            Select Case RegolaElaborazione
                Case TipiEnumerativi.enum_HubIoT_RegoleElaborazione.None
                    ret = Nothing
                Case TipiEnumerativi.enum_HubIoT_RegoleElaborazione.InviaPoligonoSenzaShape
                    ret = Nothing     'qui non serve, viene generato da hubiot tramite l'oggetto boundary
                Case TipiEnumerativi.enum_HubIoT_RegoleElaborazione.InviaPoligonoSuShape
                    'to be soon
                    Dim gis_elem As New AgronicaCoreGisDAL.GIS_ElementiGrafici_R
                    Dim dt = gis_elem.LeggiWKTDaChiaveImpianto(piva, sa_Cod, appezza, id_reg, "", "", _objParametri_Server)
                    If dt.Rows.Count > 0 Then
                        ret.pType = PrescriptionMapType.Undefined
                        ret.content = dt.Rows(0)("wkt").ToString()
                    End If
                Case TipiEnumerativi.enum_HubIoT_RegoleElaborazione.InviaMappaDiPrescrizione
                    Dim allegDoc_Reader As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_R

                    Dim doclist = allegDoc_Reader.LeggiAllegatiDaRicettaDestinazione(_objParametri_Server.PivaSuperUser, piva, sa_Cod, appezza, id_reg, IdRicettaOperazione, 75, enumSelezioneVariabile.Selezione_TabellaCompleta, "", _objParametri_Server, True)

                    If doclist.Rows.Count <= 0 Then
                        Throw New Exception("Nessuna mappa di prescrizione per le destinazione delle ricetta operazione " + IdRicettaOperazione + ToString())
                    End If

                    If doclist.Rows(0)("allegatiDocumentiText") IsNot DBNull.Value OrElse doclist.Rows(0)("allegatiDocumentiText") <> "" Then
                        ret.pType = PrescriptionMapType.JSON
                        ret.content = doclist.Rows(0)("allegatiDocumentiText").ToString()
                    Else
                        ret.pType = PrescriptionMapType.XML
                        ret.content = doclist.Rows(0)("allegatiDocumentiXML").ToString()
                    End If
                Case TipiEnumerativi.enum_HubIoT_RegoleElaborazione.InviaMappaDiPrescrizioneConLineaGuida
                    'to be soon
                    ret = Nothing
            End Select

            Return ret
        Catch ex As Exception
            ret = Nothing
            Dim customLOGParams As New CustomLOGParams With {
                .LogDescrizioneUtente = _objParametri_Server.UtenteUsername,
                .LogDirectory = _pathLog,
                .LogFileName = _fileLog
            }
            Scrivi_LOG(_objParametri_Server,
                       "AgronicaPrecisionFarming.HubIoT_Helper.GetPrescriptionMapData",
                       $"{ex.Message}{vbCrLf}StackTrace: {ex.StackTrace}",
                       CustomLOGParams:=customLOGParams)
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Private Function CreateZIPPrescriptionMap(ByVal piva As String,
                                              ByVal source As Integer,
                                              ByVal data As String,
                                              ByVal filename As String,
                                              ByVal setup As SetupPrescription,
                                              ByVal RegolaElaborazione As TipiEnumerativi.enum_HubIoT_RegoleElaborazione
                                              ) As String
        Dim ret As String = ""
        Try
            'Dim filename As String = Guid.NewGuid().ToString()                           'IdRicetta.ToString() + "_" + IdRicettaOperazione.ToString() + "_" + VIN

            Select Case RegolaElaborazione
                Case TipiEnumerativi.enum_HubIoT_RegoleElaborazione.InviaPoligonoSenzaShape

                Case TipiEnumerativi.enum_HubIoT_RegoleElaborazione.InviaPoligonoSuShape
                    'ret = CreatePolygonShapeZIP(data, filename, setup)
                Case TipiEnumerativi.enum_HubIoT_RegoleElaborazione.InviaMappaDiPrescrizione
                    ret = CreatePrescriptionZIP(piva, source, data, filename, setup)
                Case TipiEnumerativi.enum_HubIoT_RegoleElaborazione.InviaMappaDiPrescrizioneConLineaGuida
                    ret = "-1"
                Case Else
                    ret = ""
            End Select
        Catch ex As Exception
            ret = ""
            Dim customLOGParams As New CustomLOGParams With {
                .LogDescrizioneUtente = _objParametri_Server.UtenteUsername,
                .LogDirectory = _pathLog,
                .LogFileName = _fileLog
            }
            Scrivi_LOG(_objParametri_Server,
                       "AgronicaPrecisionFarming.HubIoT_Helper.CreateZIPPrescriptionMap",
                       $"{ex.Message}{vbCrLf}StackTrace: {ex.StackTrace}",
                       CustomLOGParams:=customLOGParams)
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Private Function CreatePrescriptionZIP(ByVal piva As String,
                                           ByVal source As Integer,
                                           ByVal data As String,
                                           ByVal filename As String,
                                           ByVal setup As SetupPrescription) As String
        Dim returnPath = ""
        Try
            Dim subdir As String = Path.Combine(_exportPath, filename)
            If Not Directory.Exists(subdir) Then
                Directory.CreateDirectory(subdir)
            End If

            Select Case source
                Case 1
                    'sorgente xml
                    Dim obj As New AgronicaSHPWrapper.AgronicaGis2012ToShapeVarie
                    obj.EsportaShpDatoXml(Path.Combine(subdir, filename + ".shp"), data, False, True, RemapPrescriptionSetting(GetConfigurazione(piva).mappaturaDatiMappaPrescrizione))
                Case 2
                    'sorgente json
                    Dim geojson As GeoJson_New(Of AgronicaCoreModelsSTD.Gis.GeoJSONAgroGisProp) = JsonConvert.DeserializeObject(Of GeoJson_New(Of AgronicaCoreModelsSTD.Gis.GeoJSONAgroGisProp))(data)

                    'metodo puntuale per la definizione e scrittura dello shape file con formattazione attributi
                    Dim fldDef = New NetTopologySuite.IO.Esri.Dbf.Fields.DbfNumericDoubleField("CAP_N+", 12, 3)
                    Dim writerOpt = New Writers.ShapefileWriterOptions(NetTopologySuite.IO.Esri.ShapeType.Polygon, fldDef)
                    writerOpt.Projection = getWGS84Projection()

                    Dim wrt = NetTopologySuite.IO.Esri.Shapefile.OpenWrite(Path.Combine(subdir, filename + ".shp"), writerOpt)
                    For Each obj In geojson.geoJsonCaricato.features.Where(Function(x) x.geometry.type.Equals(FeatureType.Polygon.ToString())).ToList()
                        fldDef.Value = GetRateFromGeoJsonProperties(obj.properties.AppIdRate)("CAP_N+")
                        wrt.Write(New Feature(GetPolygonFromCoordinates(obj.geometry.coordinates), Nothing))

                    Next
                    wrt.Dispose()

                    'metodo generico senza particolari configurazioni a livello di attributi
                    'se si deve formattare in modo particolare lo shape file (attributi, shapetype specifici , etc...) vedere il codice sopra

                    'Dim features As New List(Of Feature)
                    'For Each obj In geojson.geoJsonCaricato.features.Where(Function(x) x.geometry.type.Equals(FeatureType.Polygon.ToString())).ToList()
                    '    features.Add(New Feature(GetPolygonFromCoordinates(obj.geometry.coordinates),
                    '                 New AttributesTable(GetRateFromGeoJsonProperties(obj.properties.AppIdRate))))
                    'Next
                    'NetTopologySuite.IO.Esri.Shapefile.WriteAllFeatures(features, Path.Combine(subdir, filename + ".shp"), getWGS84Projection())

                Case Else
                    Throw New Exception("Tipologia sorgente dati per mappa di prescrizione non mappata. Operazione interrotta")
            End Select

            '1 - scrivi file setupprescription.json
            If setup IsNot Nothing Then
                File.WriteAllText(Path.Combine(subdir, "prescriptionsetup.json"), JsonConvert.SerializeObject(setup))
            Else
                Throw New Exception("Prescription setup not found.")
            End If

            '2- zip
            If File.Exists(Path.Combine(_exportPath, filename + ".zip")) Then
                File.Delete(Path.Combine(_exportPath, filename + ".zip"))
            End If
            AgronicaCoreUtility.AgroZip.ZipAFolder(Path.Combine(_exportPath, filename + ".zip"), subdir)

            If File.Exists(Path.Combine(_exportPath, filename + ".zip")) Then
                returnPath = Path.Combine(_exportPath, filename + ".zip")
            Else
                returnPath = ""
                Throw New Exception("File zip di prescrizione non generato. impossibile proseguire")
            End If
        Catch ex As Exception
            returnPath = ""
            Dim customLOGParams As New CustomLOGParams With {
                .LogDescrizioneUtente = _objParametri_Server.UtenteUsername,
                .LogDirectory = _pathLog,
                .LogFileName = _fileLog
            }
            Scrivi_LOG(_objParametri_Server,
                       "AgronicaPrecisionFarming.HubIoT_Helper.CreatePrescriptionZIP",
                       $"{ex.Message}{vbCrLf}StackTrace: {ex.StackTrace}",
                       CustomLOGParams:=customLOGParams)
            Throw New Exception(ex.Message, ex)
        End Try
        Return returnPath
    End Function

    Private Function getWGS84Projection() As String
        Return "GEOGCS[""GCS_WGS_1984"",DATUM[""D_WGS_1984"",SPHEROID[""WGS_1984"",6378137.0,298.257223563]],PRIMEM[""Greenwich"",0.0],UNIT[""Degree"",0.0174532925199433]]"
    End Function

    Private Function GetPolygonFromCoordinates(ByVal coords As Object) As NetTopologySuite.Geometries.Polygon
        Dim polygon As NetTopologySuite.Geometries.Polygon = Nothing
        Try

            Dim shell As NetTopologySuite.Geometries.LinearRing = Nothing
            Dim holes As List(Of NetTopologySuite.Geometries.LinearRing) = Nothing
            Dim coordinate As Double()()() = CType(coords, Double()()())

            Dim i As Integer = 0
            For i = 0 To coordinate.Length - 1
                If i = 0 Then
                    shell = GetCoordinatesArrayAsLinearRing(coordinate(i))
                Else
                    If holes Is Nothing Then
                        holes = New List(Of NetTopologySuite.Geometries.LinearRing)
                    End If
                    holes.Add(GetCoordinatesArrayAsLinearRing(coordinate(i)))
                End If
            Next
            If holes Is Nothing Then
                polygon = New NetTopologySuite.Geometries.Polygon(shell)
            Else
                polygon = New NetTopologySuite.Geometries.Polygon(shell, holes.ToArray())
            End If

        Catch ex As Exception
            polygon = Nothing
            Dim customLOGParams As New CustomLOGParams With {
                .LogDescrizioneUtente = _objParametri_Server.UtenteUsername,
                .LogDirectory = _pathLog,
                .LogFileName = _fileLog
            }
            Scrivi_LOG(_objParametri_Server,
                       "AgronicaPrecisionFarming.HubIoT_Helper.GetPolygonFromCoordinates",
                       $"{ex.Message}{vbCrLf}StackTrace: {ex.StackTrace}",
                       CustomLOGParams:=customLOGParams)
            Throw New Exception(ex.Message, ex)
        End Try
        Return polygon
    End Function

    Private Function GetRateFromGeoJsonProperties(ByVal propValue As String) As Dictionary(Of String, Object)
        Dim ret As New Dictionary(Of String, Object)
        Try
            Dim first = propValue.Split("|")
            Dim second = first(0).Split("§")
            ret.Add(second(0), Math.Round(CType(second(1).Replace(".", Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator), Double), 3))
        Catch ex As Exception
            ret = Nothing
            Dim customLOGParams As New CustomLOGParams With {
                .LogDescrizioneUtente = _objParametri_Server.UtenteUsername,
                .LogDirectory = _pathLog,
                .LogFileName = _fileLog
            }
            Scrivi_LOG(_objParametri_Server,
                       "AgronicaPrecisionFarming.HubIoT_Helper.GetRateFromGeoJsonProperties",
                       $"{ex.Message}{vbCrLf}StackTrace: {ex.StackTrace}",
                       CustomLOGParams:=customLOGParams)
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Private Function GetCoordinatesArrayAsLinearRing(ByVal points As Double()()) As NetTopologySuite.Geometries.LinearRing
        Dim ret As NetTopologySuite.Geometries.LinearRing
        Try
            Dim pList As New List(Of NetTopologySuite.Geometries.Coordinate)
            For Each p In points
                pList.Add(New NetTopologySuite.Geometries.Coordinate(p(0), p(1)))
            Next
            ret = New NetTopologySuite.Geometries.LinearRing(pList.ToArray())
        Catch ex As Exception
            ret = Nothing
            Dim customLOGParams As New CustomLOGParams With {
                .LogDescrizioneUtente = _objParametri_Server.UtenteUsername,
                .LogDirectory = _pathLog,
                .LogFileName = _fileLog
            }
            Scrivi_LOG(_objParametri_Server,
                       "AgronicaPrecisionFarming.HubIoT_Helper.GetCoordinatesArrayAsLinearRing",
                       $"{ex.Message}{vbCrLf}StackTrace: {ex.StackTrace}",
                       CustomLOGParams:=customLOGParams)
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Private Function CreatePolygonShapeZIP(ByVal data As String,
                                           ByVal filename As String,
                                           ByVal setup As SetupPrescription) As String
        Dim returnPath = ""
        Try
            Dim subdir As String = Directory.Exists(Path.Combine(_exportPath, filename))
            If Not Directory.Exists(subdir) Then
                Directory.CreateDirectory(subdir)
            End If

            '0 - scrivi file setupprescription.json
            If setup IsNot Nothing Then
                File.WriteAllText(Path.Combine(subdir, "prescriptionsetup.json"), JsonConvert.SerializeObject(setup))
            Else
                Throw New Exception("Prescription setup not found.")
            End If

            '1- in xmldata ho il WKT del poligono -> tramite il plugin gis2esrishapefile genero il pacchetto di files (shp,shx,prj,dbf)
            ' N.B.: almeno per ora si considera questa opzione valida solo per tillage , a cui non è associato il concetto di prescrizione per cui i valori di RATE sono fittizzi e fissi a 1
            Dim _esriGenerator As New Gis2EsriShapeFile("RATE")
            Dim attr As New AttributeRecord()
            attr.AddColumn("RATE", AttributeFieldType._Double, 1, 19, 3)
            _esriGenerator.Add(data, attr)
            _esriGenerator.ExportToFile(subdir, filename)

            '2- zip
            AgronicaCoreUtility.AgroZip.ZipAFolder(Path.Combine(_exportPath, filename + ".zip"), subdir)

            If File.Exists(Path.Combine(_exportPath, filename + ".zip")) Then
                returnPath = Path.Combine(_exportPath, filename + ".zip")
            Else
                returnPath = ""
                Throw New Exception("File zip di prescrizione non generato. impossibile proseguire")
            End If
        Catch ex As Exception
            returnPath = ""
            Dim customLOGParams As New CustomLOGParams With {
                .LogDescrizioneUtente = _objParametri_Server.UtenteUsername,
                .LogDirectory = _pathLog,
                .LogFileName = _fileLog
            }
            Scrivi_LOG(_objParametri_Server,
                       "AgronicaPrecisionFarming.HubIoT_Helper.CreatePolygonShapeZIP",
                       $"{ex.Message}{vbCrLf}StackTrace: {ex.StackTrace}",
                       CustomLOGParams:=customLOGParams)
            Throw New Exception(ex.Message, ex)
        End Try
        Return returnPath
    End Function

    Private Function GetProDesc(ByVal elem_cod As Integer, ByVal cod As Integer) As String
        Dim ret As String = ""
        Try
            Dim artReader As New AgronicaCoreMetaSchemaDAL.Categorie_Magazzino_R

            ret = artReader.ProDes_from_ProCod(elem_cod, cod, _objParametri_Server)
            If ret.Length > 32 Then
                ret = ret.Substring(1, 32)
            End If

        Catch ex As Exception
            ret = ""
            Dim customLOGParams As New CustomLOGParams With {
                .LogDescrizioneUtente = _objParametri_Server.UtenteUsername,
                .LogDirectory = _pathLog,
                .LogFileName = _fileLog
            }
            Scrivi_LOG(_objParametri_Server,
                       "AgronicaPrecisionFarming.HubIoT_Helper.GetProDesc",
                       $"{ex.Message}{vbCrLf}StackTrace: {ex.StackTrace}",
                       CustomLOGParams:=customLOGParams)
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Private Function CheckExecutableRowByRule(ByVal row As IDictionary(Of String, Object), ByVal Rule As Integer, ByRef mess_error As String) As Boolean
        Try
            If row("VIN") Is DBNull.Value OrElse row("VIN") = "" Then
                mess_error = "VIN non valorizzato."
                Return False
            End If
            If row("CodiceOperazione") Is DBNull.Value OrElse row("CodiceOperazione") = "" Then
                mess_error = "Categoria lavorazione non valorizzata. Impossibile indentificare il tipo di lavorazione che la macchina deve eseguire"
                Return False
            End If

            Select Case Rule
                Case TipiEnumerativi.enum_HubIoT_RegoleElaborazione.InviaPoligonoSenzaShape
                    If row("WKTPoligono") Is DBNull.Value OrElse row("WKTPoligono") = "" Then
                        mess_error = "Poligono non valorizzato."
                        Return False
                    End If
                Case TipiEnumerativi.enum_HubIoT_RegoleElaborazione.InviaPoligonoSuShape
                    If row("WKTPoligono") Is DBNull.Value OrElse row("WKTPoligono") = "" Then
                        mess_error = "Poligono non valorizzato."
                        Return False
                    End If
                Case TipiEnumerativi.enum_HubIoT_RegoleElaborazione.InviaMappaDiPrescrizione
                    If row("WKTPoligono") Is DBNull.Value OrElse row("WKTPoligono") = "" Then
                        mess_error = "Poligono non valorizzato."
                        Return False
                    End If
                    If (row("allegatiDocumentiXML") Is DBNull.Value OrElse row("allegatiDocumentiXML") = "") AndAlso (row("allegatiDocumentiText") Is DBNull.Value OrElse row("allegatiDocumentiText") = "") Then
                        mess_error = "Mappa di prescrizione non trovata."
                        Return False
                    End If
                Case TipiEnumerativi.enum_HubIoT_RegoleElaborazione.InviaMappaDiPrescrizioneConLineaGuida
                    If row("WKTPoligono") Is DBNull.Value OrElse row("WKTPoligono") = "" Then
                        mess_error = "Poligono non valorizzato."
                        Return False
                    End If
                    If (row("allegatiDocumentiXML") Is DBNull.Value OrElse row("allegatiDocumentiXML") = "") AndAlso (row("allegatiDocumentiText") Is DBNull.Value OrElse row("allegatiDocumentiText") = "") Then
                        mess_error = "Mappa di prescrizione non trovata."
                        Return False
                    End If
            End Select
        Catch ex As Exception
            Dim customLOGParams As New CustomLOGParams With {
                .LogDescrizioneUtente = _objParametri_Server.UtenteUsername,
                .LogDirectory = _pathLog,
                .LogFileName = _fileLog
            }
            Scrivi_LOG(_objParametri_Server,
                       "AgronicaPrecisionFarming.HubIoT_Helper.CheckExecutableRowByRule",
                       $"{ex.Message}{vbCrLf}StackTrace: {ex.StackTrace}",
                       CustomLOGParams:=customLOGParams)
            Return False
        End Try
        Return True
    End Function

    Private Function GetWorkOrderRulesByEntitySource(ByVal Entita_Origine As Integer,
                                                     ByVal Categoria_Lavorazione As Integer
                                                     ) As Integer
        Dim ret As Integer = -1
        Try
            Dim reader As New AgronicaCoreHubIoTDAL.HubIoT_RegoleXEntita_R
            Dim dt = reader.Leggi(Entita_Origine, Categoria_Lavorazione, "", "", _objParametri_Server)
            If dt.Rows.Count > 0 Then
                ret = dt.Rows(0)("Regola_Elaborazione")
            End If
        Catch ex As Exception
            ret = -1
            Dim customLOGParams As New CustomLOGParams With {
                .LogDescrizioneUtente = _objParametri_Server.UtenteUsername,
                .LogDirectory = _pathLog,
                .LogFileName = _fileLog
            }
            Scrivi_LOG(_objParametri_Server,
                       "AgronicaPrecisionFarming.HubIoT_Helper.GetWorkOrderRulesByEntitySource",
                       $"{ex.Message}{vbCrLf}StackTrace: {ex.StackTrace}",
                       CustomLOGParams:=customLOGParams)
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Private Function RemapPrescriptionSetting(ByVal dbfcols As List(Of ElencoColonneDBFPrescriptionMap)) As List(Of AgronicaSHPWrapper.DBFDataModel_MappaturaDati)
        Dim ret As List(Of AgronicaSHPWrapper.DBFDataModel_MappaturaDati)
        Try
            For Each col In dbfcols
                ret.Add(New AgronicaSHPWrapper.DBFDataModel_MappaturaDati() With {
                            .CampoDaRimappare = col.CampoDaRimappare,
                            .NuovoCampo = col.NuovoCampo,
                            .Operazione = col.Operazione,
                            .ValoreDefault = col.ValoreDefault
                        })
            Next
        Catch ex As Exception
            ret = Nothing
            Dim customLOGParams As New CustomLOGParams With {
                .LogDescrizioneUtente = _objParametri_Server.UtenteUsername,
                .LogDirectory = _pathLog,
                .LogFileName = _fileLog
            }
            Scrivi_LOG(_objParametri_Server,
                       "AgronicaPrecisionFarming.HubIoT_Helper.RemapPrescriptionSetting",
                       $"{ex.Message}{vbCrLf}StackTrace: {ex.StackTrace}",
                       CustomLOGParams:=customLOGParams)
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Public Sub Dispose() Implements IDisposable.Dispose
        _objParametri_Server = Nothing
        _objParametri_Server = Nothing
    End Sub
#End Region


#Region "NetTopologySuite"
    Public Sub Test_NTS()
        Dim features As New List(Of Feature)
        Dim pointList As New List(Of NetTopologySuite.Geometries.Coordinate)

        pointList.Add(New NetTopologySuite.Geometries.Coordinate(11.9703901852, 44.8771886405))
        pointList.Add(New NetTopologySuite.Geometries.Coordinate(11.9703994438, 44.8773684406))
        pointList.Add(New NetTopologySuite.Geometries.Coordinate(11.9706523212, 44.8773618573))
        pointList.Add(New NetTopologySuite.Geometries.Coordinate(11.9706430618, 44.8771820572))
        pointList.Add(New NetTopologySuite.Geometries.Coordinate(11.9703901852, 44.8771886405))

        Dim polygon As New NetTopologySuite.Geometries.Polygon(New NetTopologySuite.Geometries.LinearRing(pointList.ToArray()))
        Dim attrTable As New AttributesTable()
        attrTable.Add("RATE", 100)

        features.Add(New Feature(polygon, attrTable))

        pointList = New List(Of NetTopologySuite.Geometries.Coordinate)

        pointList.Add(New NetTopologySuite.Geometries.Coordinate(11.9706430618, 44.8771820572))
        pointList.Add(New NetTopologySuite.Geometries.Coordinate(11.9706523212, 44.8773618573))
        pointList.Add(New NetTopologySuite.Geometries.Coordinate(11.9709051986, 44.8773552734))
        pointList.Add(New NetTopologySuite.Geometries.Coordinate(11.9708959384, 44.8771754734))
        pointList.Add(New NetTopologySuite.Geometries.Coordinate(11.9706430618, 44.8771820572))

        polygon = New NetTopologySuite.Geometries.Polygon(New NetTopologySuite.Geometries.LinearRing(pointList.ToArray()))
        attrTable = New AttributesTable()
        attrTable.Add("RATE", 200)

        features.Add(New Feature(polygon, attrTable))

        pointList = New List(Of NetTopologySuite.Geometries.Coordinate)

        pointList.Add(New NetTopologySuite.Geometries.Coordinate(11.9708959384, 44.877175473))
        pointList.Add(New NetTopologySuite.Geometries.Coordinate(11.9709051986, 44.877355273))
        pointList.Add(New NetTopologySuite.Geometries.Coordinate(11.9711580758, 44.877348689))
        pointList.Add(New NetTopologySuite.Geometries.Coordinate(11.9711488149, 44.877168889))
        pointList.Add(New NetTopologySuite.Geometries.Coordinate(11.9708959384, 44.877175473))

        polygon = New NetTopologySuite.Geometries.Polygon(New NetTopologySuite.Geometries.LinearRing(pointList.ToArray()))
        attrTable = New AttributesTable()
        attrTable.Add("RATE", 300)

        features.Add(New Feature(polygon, attrTable))


        NetTopologySuite.IO.Esri.Shapefile.WriteAllFeatures(features, $"C:\GIASLAN\File_Esportazioni\EsportazionePF\test_nettopologysuite.shp", "WGS84")


    End Sub
#End Region
End Class
