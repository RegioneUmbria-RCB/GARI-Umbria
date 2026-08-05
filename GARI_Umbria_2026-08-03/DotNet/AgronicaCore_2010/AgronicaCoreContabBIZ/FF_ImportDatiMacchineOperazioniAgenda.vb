Imports System.Transactions
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreUtility
Imports System.Net
Imports Newtonsoft.Json
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports System.Data.Entity
Imports AgronicaCoreDataProvider

Public Class FF_ImportDatiMacchineOperazioniAgenda
    Public Function ImportDatiMacchineOperazioniAgenda_ScriviModifica(ByVal DatiMacchinaOperazione As AgronicaCoreModelsSTD.scambiodati.ImportDatiMacchineOperazionAgenda,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As Boolean
        Dim nomeRoutine As String = "AgronicaCoreContabBIZ.FF_ImportDatiMacchineOperazioniAgenda.ImportDatiMacchineOperazioniAgenda_ScriviModifica()"
        Dim messaggioErrore As String = ""

        Dim username As String = If(objParametri_Utenti.UtenteUsername <> "", objParametri_Utenti.UtenteUsername, objParametri.UsernameOperazione)
        Dim ret = False

        Try
            Using scope As New TransactionScope()
                Using GiasContext As Gias_DeveloperServer_Entities = Gias_EF_Utility.CreateGiasContextConnection(objParametri.StringaConnessione)

                    Dim DatiMacchineOperazioniAgendaList = From DatiMacchineOperazioniAgenda In GiasContext.FF_ImportDatiMacchineOperazioniAgenda
                                                           Where DatiMacchineOperazioniAgenda.ID = DatiMacchinaOperazione.ID
                                                           Select DatiMacchineOperazioniAgenda

                    Dim oDatiMacchineOperazioniAgenda = DatiMacchineOperazioniAgendaList.FirstOrDefault()

                    Dim check As AgronicaCoreEntityFramework_POCO.FF_ImportDatiMacchineOperazioniAgenda = Nothing


                    If oDatiMacchineOperazioniAgenda Is Nothing Then
                        check = AgronicaCoreContabDAL.EF_FF_ImportDatiMacchineOperazioniAgenda.Internal_Scrivi_DatiMacchinaOperazioneAgenda(DatiMacchinaOperazione,
                                                                          objParametri,
                                                                          objParametri_Utenti,
                                                                          username,
                                                                          GiasContext,
                                                                          False
                                                                          )

                    Else
                        check = AgronicaCoreContabDAL.EF_FF_ImportDatiMacchineOperazioniAgenda.Internal_Modifica_DatiMacchinaOperazioneAgenda(DatiMacchinaOperazione,
                                                                          objParametri,
                                                                          objParametri_Utenti,
                                                                          username,
                                                                          GiasContext,
                                                                          False
                                                                          )
                    End If

                    scope.Complete()
                End Using
            End Using
            ret = True
        Catch wex As WebException
            Throw New WebException(wex.Message)

        Catch ex As Exception
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return ret
    End Function


    Public Function RecoverySingleTaskData(ByVal ID_Task As Integer,
                                     ByVal ApiKey As String,
                                     ByVal ApiUrlBase As String,
                                     ByVal ApiUrlResource As String,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                     ByRef objParametri_utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabBIZ.FF_ImportDatiMacchineOperazioniAgenda.RecoveryTaskData()"
        Dim messaggioErrore As String = ""

        Dim username As String = If(objParametri_utenti.UtenteUsername <> "", objParametri_utenti.UtenteUsername, objParametri.UsernameOperazione)
        Dim ret = False

        Try

            'Dim callURI As String = /v1/TaskData/" + ID_Task.ToString()
            Dim clientHttp = New Http()
            Dim HeadCust = New WebHeaderCollection
            Dim newState As Integer = -1 'errore di default

            HeadCust.Add("X-API-KEY", ApiKey)

            Dim resp = clientHttp.CallWS_RestSharp_JSON(ApiUrlBase, ApiUrlResource + ID_Task.ToString(), "GET", "", HeadCust)
            Using scope As New TransactionScope()
                Using GiasContext As Gias_DeveloperServer_Entities = Gias_EF_Utility.CreateGiasContextConnection(objParametri.StringaConnessione)
                    Dim DatiMacchineOperazioniAgendaList = From DatiMacchineOperazioniAgenda In GiasContext.FF_ImportDatiMacchineOperazioniAgenda
                                                           Where DatiMacchineOperazioniAgenda.ID = ID_Task
                                                           Select DatiMacchineOperazioniAgenda

                    Dim oDatiMacchineOperazioniAgenda = DatiMacchineOperazioniAgendaList.FirstOrDefault()
                    Dim strResp As String = JsonConvert.SerializeObject(resp.Content)


                    If resp.StatusCode = HttpStatusCode.OK Then
                        newState = 2    'elaborato
                    End If

                    Dim objData As New AgronicaCoreModelsSTD.scambiodati.ImportDatiMacchineOperazionAgenda() With {
                            .ID = ID_Task,
                            .pivasuperuser = oDatiMacchineOperazioniAgenda.pivasuperuser,
                            .piva = oDatiMacchineOperazioniAgenda.piva,
                            .cod_impianto = oDatiMacchineOperazioniAgenda.cod_impianto,
                            .cod_operazione = oDatiMacchineOperazioniAgenda.cod_operazione,
                            .id_agenda = oDatiMacchineOperazioniAgenda.id_agenda,
                            .status = newState,
                            .payload = strResp
                        }


                    If oDatiMacchineOperazioniAgenda Is Nothing Then
                        Throw New WebException("Hook per TaskData " + ID_Task.ToString() + " non trovato su GIAS, impossibile recuperare i dati per associazione tracciabilità")

                    Else
                        AgronicaCoreContabDAL.EF_FF_ImportDatiMacchineOperazioniAgenda.Internal_Modifica_DatiMacchinaOperazioneAgenda(objData,
                                                                              objParametri,
                                                                              objParametri_utenti,
                                                                              username,
                                                                              GiasContext,
                                                                              False
                                                                              )
                    End If

                    'If LinkTaskDataWithAgenda(ID_Task,
                    '                          objParametri,
                    '                          objParametri_utenti,
                    '                          GiasContext,
                    '                          False) = False Then
                    '    Throw New WebException("Errore Link TaskData con Agenda " + ID_Task.ToString())

                    'End If

                    'se passa la fase di link , lo stato diventa 3 (associato)

                    scope.Complete()

                End Using
            End Using



            'predisposizione al download dello shape file (da definire dove salvarlo)
            'If newState <> -1 And resp.StatusCode = HttpStatusCode.OK Then
            '    Dim objZIP = DownloadShapeFileFromTaskData(ID_Task, resp.Content("shapefileCosUrl"), objParametri, objParametri_utenti)
            '    If objZIP.RispostaStringa IsNot Nothing Then

            '    End If
            'End If
        Catch wex As WebException
            Throw New WebException(wex.Message)

        Catch ex As Exception
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try
    End Function

    Public Function DownloadShapeFileFromTaskData(ByVal ID_Task As Integer,
                                                  ByVal UrlBase As String,
                                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                  ByRef objParametri_utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As rispostaStandard(Of Byte())

        Dim rval As New rispostaStandard(Of Byte())
        rval.RispostaStringa = Nothing
        Try
            Dim client = New Http()
            Dim HeadCust = New WebHeaderCollection

            Dim resp = client.chiamaWS_RestShapr(Nothing, "", UrlBase, "", RestSharp.Method.GET, "application/json", "", HeadCust)
            Select Case resp.StatusCode
                Case HttpStatusCode.OK
                    rval.RispostaOK = True
                    Dim HeaderResp = resp.Headers.ToList()
                    Dim contentType = IIf(HeaderResp.Find(Function(x) x.Name = "Content-Type") IsNot Nothing, HeaderResp.Find(Function(x) x.Name = "Content-Type").Value, "").ToString()
                    rval.RispostaStringa = resp.RawBytes
                Case Else
                    Throw New WebException("Download Shape file failed: " + vbCrLf + resp.StatusCode.ToString() + " " + resp.StatusDescription + vbCrLf)
            End Select
        Catch ex As Exception
            rval.RispostaOK = False
            rval.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return rval
    End Function

    Public Function LinkTaskDataWithAgenda(ByVal ID_Task As Integer,
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                           ByRef objParametri_utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                           Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                           Optional ByVal NewTransaction As Boolean = True) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabBIZ.FF_ImportDatiMacchineOperazioniAgenda.LinkTaskDataWithAgenda()"
        Dim messaggioErrore As String = ""
        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False
        Dim ret As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri.StringaConnessione)
            bCloseContext = True
        End If

        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Try

            'leggo il record di TaskData da associare
            Dim DatiMacchineOperazioniAgendaList = From DatiMacchineOperazioniAgenda In GiasContext.FF_ImportDatiMacchineOperazioniAgenda.AsNoTracking
                                                   Where DatiMacchineOperazioniAgenda.ID = ID_Task
                                                   Select DatiMacchineOperazioniAgenda

            Dim oDatiMacchineOperazioniAgenda = DatiMacchineOperazioniAgendaList.FirstOrDefault()


            'recupero l'elenco dei record di TaskData già associati per piva\cod_operazione
            'da cui estraggo l'elenco degli ID_agenda da escludere nella ricerca dei movimenti agenda associabili
            Dim DatiMacchinaOperazioniAgendaAssociati_idAgenda = From DatiMacchineOperazioniAgenda In GiasContext.FF_ImportDatiMacchineOperazioniAgenda.AsNoTracking
                                                                 Where DatiMacchineOperazioniAgenda.piva = oDatiMacchineOperazioniAgenda.piva And
                                                                     DatiMacchineOperazioniAgenda.cod_operazione = oDatiMacchineOperazioniAgenda.cod_operazione And
                                                                     DatiMacchineOperazioniAgenda.stato > 1
                                                                 Select DatiMacchineOperazioniAgenda.id_agenda

            Dim DatiMacchinaOperazioniAgendaAssociati_idAgendaList = DatiMacchinaOperazioniAgendaAssociati_idAgenda.ToList()



            Dim objTaskData As AgronicaCoreDTOStd.SDF.TaskData =
                JsonConvert.DeserializeObject(Of AgronicaCoreDTOStd.SDF.TaskData)(oDatiMacchineOperazioniAgenda.payload)

            Dim rImpreseProgetti = New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
            Dim rFF_DatiMAcchinaOperazioniAgenda = New AgronicaCoreContabDAL.FF_ImportDatiMacchinaOperazioniAgenda_R

            'recupero la chiave dell'impianto collegato
            Dim datiImpiantoDaEsercizio = rImpreseProgetti.LeggiMinimal(oDatiMacchineOperazioniAgenda.piva,
                                                                        0,
                                                                        0,
                                                                        0,
                                                                        oDatiMacchineOperazioniAgenda.cod_impianto,
                                                                        AGRODATAINIZIO,
                                                                        AGRODATAFINE,
                                                                        "",
                                                                        "",
                                                                        objParametri)
            If datiImpiantoDaEsercizio IsNot Nothing Then
                'recupero elenco movimenti agenda per tipologia operazione su quell'impianto (recuperato dall'esercizio passato nel campo cod_impianto
                'del modello dati), con data_movimento minore o uguale alla data di chiusura del task.
                'per gestire il caso di n movimenti per la stessa operazione, alla stessa data, li ordino per id_agenda crescente (quindi li associo per ordine di inserimento su gias)

                Dim filtroAgenda As String = IIf((DatiMacchinaOperazioniAgendaAssociati_idAgendaList Is Nothing), "", "a.id_agenda not in (" & String.Join(",", DatiMacchinaOperazioniAgendaAssociati_idAgendaList) & ")")
                Dim ordinamento As String = "Data_Movimento desc, a.Id_Agenda Asc"

                Dim rowData As DataRow = datiImpiantoDaEsercizio.Rows(0)

                Dim dtMovimentiAgenda = rFF_DatiMAcchinaOperazioniAgenda.GetMovimentiAgendaDaAssociareConTaskDataSDF(rowData("Piva"),
                                                                                                                     rowData("Sa_Cod"),
                                                                                                                     rowData("Appezza"),
                                                                                                                     rowData("Id_Reg"),
                                                                                                                     oDatiMacchineOperazioniAgenda.cod_operazione,
                                                                                                                     objTaskData.taskDataEndDateTime,
                                                                                                                     objParametri,
                                                                                                                     filtroAgenda,
                                                                                                                     ordinamento)
                If dtMovimentiAgenda IsNot Nothing Then
                    If dtMovimentiAgenda.Rows.Count() > 0 Then
                        oDatiMacchineOperazioniAgenda.id_agenda = dtMovimentiAgenda.Rows(0)("id_agenda")
                        oDatiMacchineOperazioniAgenda.stato = 3
                        GiasContext.FF_ImportDatiMacchineOperazioniAgenda.Attach(oDatiMacchineOperazioniAgenda)
                        GiasContext.Entry(oDatiMacchineOperazioniAgenda).State = EntityState.Modified
                        GiasContext.SaveChanges()
                        If NewTransaction Then
                            scope.Complete()
                            scope.Dispose()
                        End If
                    End If
                End If
            End If
            ret = True
        Catch ex As Exception
            messaggioErrore = ex.Message
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

        Return ret

    End Function

    Public Function GetTaskDataToLink(ByVal PartitaIva As String,
                                      ByVal ProgettoCod As Integer,
                                      ByVal CodiceOperazione As Integer,
                                      ByVal id_agenda As Integer,
                                      ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

    End Function

    Public Sub MassiveTaskDataLink(ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing)



    End Sub

    Public Shared Function LinkTaskDataWithAgendaMov(ByVal ID_Task As Integer,
                                           ByVal id_agenda As Integer,
                                          ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                           Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                           Optional ByVal NewTransaction As Boolean = True) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabBIZ.FF_ImportDatiMacchineOperazioniAgenda.ReadDataFromTaskData()"
        Dim messaggioErrore As String = ""
        Dim bCloseContext As Boolean = False
        Dim ret As Boolean = False
        Dim scope As TransactionScope = Nothing

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If

        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Try

            Dim PayloadList = From DatiMacchineOperazioniAgenda In GiasContext.FF_ImportDatiMacchineOperazioniAgenda
                              Where DatiMacchineOperazioniAgenda.ID = ID_Task
                              Select DatiMacchineOperazioniAgenda

            Dim oDatiMacchineOperazioniAgenda = PayloadList.FirstOrDefault()

            If oDatiMacchineOperazioniAgenda IsNot Nothing Then
                oDatiMacchineOperazioniAgenda.id_agenda = id_agenda
                oDatiMacchineOperazioniAgenda.stato = 3
                GiasContext.FF_ImportDatiMacchineOperazioniAgenda.Attach(oDatiMacchineOperazioniAgenda)
                GiasContext.Entry(oDatiMacchineOperazioniAgenda).State = EntityState.Modified
                GiasContext.SaveChanges()
                scope.Complete()
                If NewTransaction Then
                    scope.Dispose()
                End If
            End If

        Catch wex As WebException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            Throw New WebException(wex.Message)

        Catch ex As Exception
            messaggioErrore = ex.Message
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

        Return ret

    End Function


    Public Function ReadDataFromTaskData(ByVal ID_Task As Integer,
                                         ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                         ByRef objParametri_utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                         Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing) As AgronicaCoreDTOStd.SDF.TaskData

        Dim nomeRoutine As String = "AgronicaCoreContabBIZ.FF_ImportDatiMacchineOperazioniAgenda.ReadDataFromTaskData()"
        Dim messaggioErrore As String = ""
        Dim bCloseContext As Boolean = False
        Dim objData As AgronicaCoreDTOStd.SDF.TaskData = Nothing

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If

        Try

            Dim PayloadList = From DatiMacchineOperazioniAgenda In GiasContext.FF_ImportDatiMacchineOperazioniAgenda
                              Where DatiMacchineOperazioniAgenda.ID = ID_Task
                              Select DatiMacchineOperazioniAgenda.payload

            Dim payl = PayloadList.FirstOrDefault()

            If payl IsNot Nothing Then
                objData = JsonConvert.DeserializeObject(Of AgronicaCoreDTOStd.SDF.TaskData)(payl)
            End If

        Catch wex As WebException
            Throw New WebException(wex.Message)

        Catch ex As Exception
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

        Return objData

    End Function


    Public Sub MassiveRecoveryTaskData(ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                       ByRef objParametri_utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                       Optional ByVal Directory_log As String = "",
                                       Optional ByVal NomeFile As String = "")

        Dim nomeRoutine As String = "AgronicaCoreContabBIZ.FF_ImportDatiMacchineOperazioniAgenda.MassiveRecoveryTaskData()"
        Dim messaggioErrore As String = ""
        Dim GiasContext As Gias_DeveloperServer_Entities = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)

        Dim oReader As New AgronicaCoreVarieDAL.Configurazione_Siti_R()
        Dim ApiUrlBase = oReader.Leggi_Valore(6, "Tracciabilita_ApiUrlBase_SDF", "", "", objParametriServer)
        Dim ApiUrlResource = oReader.Leggi_Valore(6, "Tracciabilita_ApiUrlResource_SDF", "", "", objParametriServer)
        Dim ApiKey = oReader.Leggi_Valore(6, "Tracciabilita_ApiKey_SDF", "", "", objParametriServer)

        Dim objLog As AgronicaCoreDataProvider.LogProvider = Nothing

        If Directory_log <> "" And NomeFile <> "" Then
            objLog = New AgronicaCoreDataProvider.LogProvider
        End If

        Dim customLOGParams As New CustomLOGParams With {
                .LogDescrizioneUtente = objParametriServer.LogDescrizioneUtente,
                .LogDirectory = Directory_log,
                .LogFileName = NomeFile
            }

        Try
            Dim TaskDataList = From DatiMacchineOperazioniAgenda In GiasContext.FF_ImportDatiMacchineOperazioniAgenda
                               Where DatiMacchineOperazioniAgenda.stato = 1
                               Select DatiMacchineOperazioniAgenda

            For Each task In TaskDataList
                If objLog IsNot Nothing Then
                    objLog.Scrivi_LOG(
                           objParametriServer,
                           "MassiveRecoveryTaskData",
                           "Elaborazione TaskData ID: " + task.ID.ToString(),
                           CustomLOGParams:=customLOGParams)
                End If
                RecoverySingleTaskData(task.ID, ApiKey, ApiUrlBase, ApiUrlResource, objParametriServer, objParametri_utenti)
            Next


        Catch ex As Exception
            If objLog IsNot Nothing Then
                objLog.Scrivi_LOG(
                           objParametriServer,
                           "MassiveRecoveryTaskData",
                           "Errore: " + ex.Message,
                           CustomLOGParams:=customLOGParams)
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        GiasContext.Dispose()

    End Sub

End Class
