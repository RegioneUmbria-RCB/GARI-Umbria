Imports AgronicaCoreDataProvider
Imports AgronicaCoreModelsSTD.exceptions
Imports Newtonsoft.Json
Imports System.Net.Http
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Web

Public Class GestoreCache

    Public Function PulisciCacheSync(httpClient As HttpClient, objParametri_Server As AgronicaCoreParametri) As RispostaStandard
        Dim r As New RispostaStandard
        r.RispostaOK = True

        Try
            Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim clearCahecheEndPoints As List(Of String) = objConfSiti.getCaheServersUrls(objParametri_Server)
            Dim erroriGias As New List(Of ErroreGias)

            For Each endpoint As String In clearCahecheEndPoints
                Try
                    Dim response As New HttpResponseMessage
                    Dim content As String = "{" & vbCrLf & "}"
                    If endpoint.Contains("PulisciCacheEFactory") Then
                        content = "{" + $"objP_server: '{JsonConvert.SerializeObject(HttpContext.Current.Session("ASG_objParametri_Server"))}'" + "}"
                    End If

                    response = httpClient.PostAsync(endpoint, New StringContent(content, Nothing, "application/json")).Result

                    If (response.IsSuccessStatusCode) Then
                        r.RispostaStringaCustom &= "Chiamata POST: " & endpoint & " eseguita correttamente<br>"
                    Else
                        response = httpClient.GetAsync(endpoint).Result
                        If (response.IsSuccessStatusCode) Then
                            r.RispostaStringaCustom &= "Chiamata GET: " & endpoint & " eseguita correttamente<br>"
                        End If
                    End If

                    response.EnsureSuccessStatusCode()
                Catch ex As Exception

                    r.RispostaOK = False
                    r.ErroriGias.Add(New ErroreGias With {
                                        .messaggio = "Errore durante l'operazione su " & endpoint & "<br>" & ex.Message & "<br>"
                                     }
                    )

                End Try
            Next
        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function

    Public Async Function PulisciCache(httpClient As HttpClient, objParametri_Server As AgronicaCoreParametri) As Task(Of RispostaStandard)
        Dim r As New RispostaStandard With {.RispostaOK = True}

        Try
            Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim clearCahecheEndPoints As List(Of String) = objConfSiti.getCaheServersUrls(objParametri_Server)

            Dim taskList = clearCahecheEndPoints _
                .Select(Function(ep) ElaboraEndpointAsync(httpClient, ep)) _
                .ToArray()

            ' aspetta che finiscano tutte
            Dim esiti = Await Task.WhenAll(taskList).ConfigureAwait(False)

            For Each e In esiti
                If e.Ok Then
                    r.RispostaStringaCustom &= $"Chiamata {e.Endpoint} ok<br>"
                Else
                    r.ErroriGias.Add(New ErroreGias With {.messaggio = $"Errore su {e.Endpoint}: {e.Messaggio}<br>"})
                End If
            Next


        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " &
            AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function


    Public Async Function PulisciCachePermessi(httpClient As HttpClient,
                                           objParametri_Server As AgronicaCoreParametri) As Task(Of RispostaStandard)

        Dim r As New RispostaStandard With {.RispostaOK = True}

        Try
            Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim clearCahecheEndPoints As List(Of String) = objConfSiti.getCaheServersUrlsMetodo(objParametri_Server)

            Dim taskList = clearCahecheEndPoints _
                .Select(Function(ep) ElaboraEndpointMetodoAsync("Utenti_Permessi_R", httpClient, ep)) _
                .ToArray()

            ' aspetta che finiscano tutte
            Dim esiti = Await Task.WhenAll(taskList).ConfigureAwait(False)

            For Each e In esiti
                If e.Ok Then
                    r.RispostaStringaCustom &= $"Chiamata {e.Endpoint} ok<br>"
                Else
                    r.ErroriGias.Add(New ErroreGias With {.messaggio = $"Errore su {e.Endpoint}: {e.Messaggio}<br>"})
                    r.RispostaOK = False
                End If
            Next


        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " &
            AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function

    Private Class EsitoEndpoint
        Public Property Endpoint As String
        Public Property Ok As Boolean
        Public Property Messaggio As String
    End Class

    Private Async Function ElaboraEndpointAsync(httpClient As HttpClient,
                                             endpoint As String) As Task(Of EsitoEndpoint)
        Dim esito As New EsitoEndpoint With {.Endpoint = endpoint}

        Try
            Dim cts As New CancellationTokenSource()

            Dim content = New StringContent("{ }", System.Text.Encoding.UTF8, "application/json")
            ' *** Modifica 1: Aggiungi .ConfigureAwait(False) ***
            Dim httpTask = httpClient.PostAsync(endpoint, content, cts.Token)
            Dim delayTask = Task.Delay(TimeSpan.FromSeconds(10))

            ' *** Modifica 2: Aggiungi .ConfigureAwait(False) ***
            Dim finished = Await Task.WhenAny(httpTask, delayTask).ConfigureAwait(False)

            If finished Is delayTask Then
                ' annullo la http perché ha sforato
                cts.Cancel()
                Throw New TimeoutException($"POST {endpoint} ha superato il timeout di {10} secondi.")
            End If

            ' *** Modifica 3: Aggiungi .ConfigureAwait(False) ***
            Dim response = Await httpTask.ConfigureAwait(False)

            If response.IsSuccessStatusCode Then
                esito.Ok = True
                esito.Messaggio = "POST ok"
            Else
                ' *** Modifica 4: Aggiungi .ConfigureAwait(False) ***
                Dim getResponse = Await httpClient.GetAsync(endpoint).ConfigureAwait(False)
                getResponse.EnsureSuccessStatusCode()
                esito.Ok = True
                esito.Messaggio = "GET ok (fallback)"
            End If

        Catch ex As Exception
            esito.Ok = False
            esito.Messaggio = ex.Message
        End Try

        Return esito
    End Function

    Private Async Function ElaboraEndpointMetodoAsync(nomeMetodo As String,
                                                     httpClient As HttpClient,
                                                     endpoint As String) As Task(Of EsitoEndpoint)
        Dim esito As New EsitoEndpoint With {.Endpoint = endpoint}

        Try
            Dim cts As New CancellationTokenSource()

            Dim contentJson = "{ ""NomeMetodo"": """ + nomeMetodo + """ }"

            Dim content = New StringContent(contentJson, System.Text.Encoding.UTF8, "application/json")
            ' *** Modifica 1: Aggiungi .ConfigureAwait(False) ***
            Dim httpTask = httpClient.PostAsync(endpoint, content, cts.Token)
            Dim delayTask = Task.Delay(TimeSpan.FromSeconds(30))

            ' *** Modifica 2: Aggiungi .ConfigureAwait(False) ***
            Dim finished = Await Task.WhenAny(httpTask, delayTask).ConfigureAwait(False)

            If finished Is delayTask Then
                ' annullo la http perché ha sforato
                cts.Cancel()
                Throw New TimeoutException($"POST {endpoint} ha superato il timeout di {30} secondi.")
            End If

            ' *** Modifica 3: Aggiungi .ConfigureAwait(False) ***
            Dim response = Await httpTask.ConfigureAwait(False)

            If response.IsSuccessStatusCode Then
                esito.Ok = True
                esito.Messaggio = "POST ok"
            Else
                ' *** Modifica 4: Aggiungi .ConfigureAwait(False) ***
                Dim getResponse = Await httpClient.GetAsync(endpoint).ConfigureAwait(False)
                getResponse.EnsureSuccessStatusCode()
                esito.Ok = True
                esito.Messaggio = "GET ok (fallback)"
            End If

        Catch ex As Exception
            esito.Ok = False
            esito.Messaggio = ex.Message
        End Try

        Return esito
    End Function


    Public Function PulisciCachePermessiSync(httpClient As HttpClient, objParametri_Server As AgronicaCoreParametri) As RispostaStandard
        Dim r As New RispostaStandard
        r.RispostaOK = True

        Try
            Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim clearCahecheEndPoints As List(Of String) = objConfSiti.getCaheServersUrlsPermessi(objParametri_Server)
            Dim erroriGias As New List(Of ErroreGias)

            For Each endpoint As String In clearCahecheEndPoints
                Try
                    If endpoint.Contains("Permessi") Then

                        Dim response As New HttpResponseMessage
                        Dim content As String = "{" & vbCrLf & "}"

                        response = httpClient.PostAsync(endpoint, New StringContent(content, Nothing, "application/json")).Result

                        If (response.IsSuccessStatusCode) Then
                            r.RispostaStringaCustom &= "Chiamata POST: " & endpoint & " eseguita correttamente<br>"
                        Else
                            response = httpClient.GetAsync(endpoint).Result
                            If (response.IsSuccessStatusCode) Then
                                r.RispostaStringaCustom &= "Chiamata GET: " & endpoint & " eseguita correttamente<br>"
                            End If
                        End If

                        response.EnsureSuccessStatusCode()

                    End If
                Catch ex As Exception

                    r.RispostaOK = False
                    r.ErroriGias.Add(New ErroreGias With {
                                        .messaggio = "Errore durante l'operazione su " & endpoint & "<br>" & ex.Message & "<br>"
                                     }
                    )

                End Try
            Next
        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function

    Public Function PulisciCacheImpostazioniSync(httpClient As HttpClient, objParametri_Server As AgronicaCoreParametri) As RispostaStandard
        Dim r As New RispostaStandard
        r.RispostaOK = True

        Try
            Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim clearCahecheEndPoints As List(Of String) = objConfSiti.getCaheServersUrlsImpostazioni(objParametri_Server)
            Dim erroriGias As New List(Of ErroreGias)

            For Each endpoint As String In clearCahecheEndPoints
                Try
                    If endpoint.Contains("Impostazioni") Then

                        Dim response As New HttpResponseMessage
                        Dim content As String = "{" & vbCrLf & "}"

                        response = httpClient.PostAsync(endpoint, New StringContent(content, Nothing, "application/json")).Result

                        If (response.IsSuccessStatusCode) Then
                            r.RispostaStringaCustom &= "Chiamata POST: " & endpoint & " eseguita correttamente<br>"
                        Else
                            response = httpClient.GetAsync(endpoint).Result
                            If (response.IsSuccessStatusCode) Then
                                r.RispostaStringaCustom &= "Chiamata GET: " & endpoint & " eseguita correttamente<br>"
                            End If
                        End If

                        response.EnsureSuccessStatusCode()

                    End If
                Catch ex As Exception

                    r.RispostaOK = False
                    r.ErroriGias.Add(New ErroreGias With {
                                        .messaggio = "Errore durante l'operazione su " & endpoint & "<br>" & ex.Message & "<br>"
                                     }
                    )

                End Try
            Next
        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function

    Public Async Function PulisciCacheImpostazioni(httpClient As HttpClient, objParametri_Server As AgronicaCoreParametri) As Task(Of RispostaStandard)
        Dim r As New RispostaStandard With {.RispostaOK = True}

        Try
            Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim clearCahecheEndPoints As List(Of String) = objConfSiti.getCaheServersUrlsMetodo(objParametri_Server)

            Dim taskList As List(Of Task(Of EsitoEndpoint)) = clearCahecheEndPoints _
                .Select(Function(ep) ElaboraEndpointMetodoAsync("Imprese_Impostazioni_R", httpClient, ep)) _
                .ToList()

            Dim taskList1 As List(Of Task(Of EsitoEndpoint)) = clearCahecheEndPoints _
                .Select(Function(ep) ElaboraEndpointMetodoAsync("Utenti_Impostazioni_Read", httpClient, ep)) _
                .ToList()

            taskList.AddRange(taskList1)

            ' aspetta che finiscano tutte
            Dim esiti = Await Task.WhenAll(taskList).ConfigureAwait(False)

            For Each e In esiti
                If e.Ok Then
                    r.RispostaStringaCustom &= $"Chiamata {e.Endpoint} ok<br>"
                Else
                    r.ErroriGias.Add(New ErroreGias With {.messaggio = $"Errore su {e.Endpoint}: {e.Messaggio}<br>"})
                    r.RispostaOK = False
                End If
            Next


        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " &
            AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function

End Class
