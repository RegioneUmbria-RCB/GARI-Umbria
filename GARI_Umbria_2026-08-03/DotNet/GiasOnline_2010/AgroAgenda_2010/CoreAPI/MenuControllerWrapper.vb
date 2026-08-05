Imports System.Net
Imports AgronicaCoreDTOStd.InData.Menu
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.Menu
Imports AgronicaCoreModelsSTD.profilazione
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json

Public Interface IControllerWrapper


End Interface

Public Class MenuControllerWrapper : Inherits BaseControllerWrapper
    Implements IControllerWrapper

    Private Const ROUTENAME As String = "/Menu"

    Public Sub New()
        MyBase.New()
    End Sub


    Public Function AlberoMenu() As rispostaStandard(Of AlberoMenu)

        Dim endpointUrl As String = _linkCoreAPI + ROUTENAME + "/AlberoMenu"
        Dim rs As rispostaStandard(Of AlberoMenu) = Nothing
        Dim resp = MyBase.MakeRestCall(endpointUrl, "application/json", RestSharp.Method.GET)

        If Not resp.IsSuccessful Then
            Throw New Exception(resp.ErrorMessage)
        End If

        If resp.IsSuccessful AndAlso resp.ResponseStatus = RestSharp.ResponseStatus.Completed AndAlso resp.StatusCode = HttpStatusCode.OK Then
            rs = JsonConvert.DeserializeObject(Of rispostaStandard(Of AlberoMenu))(resp.Content)
        End If

        Return rs

    End Function

    Public Function informazioniUtente() As rispostaStandard(Of Utente)

        Dim endpointUrl As String = _linkCoreAPI + ROUTENAME + "/informazioniUtente"
        Dim rs As rispostaStandard(Of Utente) = Nothing
        Dim resp = MyBase.MakeRestCall(endpointUrl, "application/json", RestSharp.Method.GET)

        If Not resp.IsSuccessful Then
            Throw New Exception(resp.ErrorMessage)
        End If

        If resp.IsSuccessful AndAlso resp.ResponseStatus = RestSharp.ResponseStatus.Completed AndAlso resp.StatusCode = HttpStatusCode.OK Then
            rs = JsonConvert.DeserializeObject(Of rispostaStandard(Of Utente))(resp.Content)
        End If

        Return rs

    End Function

    Public Function AggiornaPreferiti(idSezioni As List(Of Integer)) As HttpStatusCode

        Dim endpointUrl As String = _linkCoreAPI + ROUTENAME + "/aggiornaPreferiti"

        Dim body = New preferiti_in With {.preferiti = New List(Of Integer)(idSezioni)}

        Dim resp = MyBase.MakeRestCall(endpointUrl, "application/json", RestSharp.Method.POST, body)

        If Not resp.IsSuccessful Then
            Throw New Exception(resp.ErrorMessage)
        End If

        Return resp.StatusCode

    End Function

    Public Function AggiornaAttivitaNavigazioneAziende(piva As String) As Object

        Dim endpointUrl As String = _linkCoreAPI + ROUTENAME + "/aggiornaAttivitaNavigazioneAziende"

        Dim body = New attivitaNavigazioneAziende_in With
                        {
                            .piva = piva
                        }

        Dim resp = MyBase.MakeRestCall(endpointUrl, "application/json", RestSharp.Method.POST, body)

        If Not resp.IsSuccessful Then
            Throw New Exception(resp.ErrorMessage)
        End If

        Return resp.StatusCode

    End Function

    Public Function RicercaAzienda(stringaRicerca As String) As rispostaStandard(Of List(Of Impresa))

        Dim endpointUrl As String = _linkCoreAPI + ROUTENAME + "/ricercaAzienda?stringaRicerca=" & stringaRicerca
        Dim rs As rispostaStandard(Of List(Of Impresa)) = Nothing
        Dim resp = MyBase.MakeRestCall(endpointUrl, "application/json", RestSharp.Method.GET)

        If Not resp.IsSuccessful Then
            Throw New Exception(resp.ErrorMessage)
        End If

        If resp.IsSuccessful AndAlso resp.ResponseStatus = RestSharp.ResponseStatus.Completed AndAlso resp.StatusCode = HttpStatusCode.OK Then
            rs = JsonConvert.DeserializeObject(Of rispostaStandard(Of List(Of Impresa)))(resp.Content)
        End If

        Return rs

    End Function

    Public Function OttieniUltimeAziendeSelezionate() As rispostaStandard(Of List(Of Impresa))

        Dim endpointUrl As String = _linkCoreAPI + ROUTENAME + "/ottieniUltimeAziendeSelezionate"
        Dim rs As rispostaStandard(Of List(Of Impresa)) = Nothing
        Dim resp = MyBase.MakeRestCall(endpointUrl, "application/json", RestSharp.Method.GET)

        If Not resp.IsSuccessful Then
            Throw New Exception(resp.ErrorMessage)
        End If

        If resp.IsSuccessful AndAlso resp.ResponseStatus = RestSharp.ResponseStatus.Completed AndAlso resp.StatusCode = HttpStatusCode.OK Then
            rs = JsonConvert.DeserializeObject(Of rispostaStandard(Of List(Of Impresa)))(resp.Content)
        End If

        Return rs

    End Function

End Class
