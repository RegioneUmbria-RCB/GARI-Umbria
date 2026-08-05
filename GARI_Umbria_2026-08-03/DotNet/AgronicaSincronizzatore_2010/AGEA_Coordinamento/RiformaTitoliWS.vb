Imports AgronicaCoreDataProvider

Public Class RiformaTitoliWS

    Dim objParametri_Server As AgronicaCoreParametri
    Dim objParametri_Utenti As AgronicaCoreParametri

    Dim username As String
    Dim password As String
    Dim url As String

    Dim ws As RiformaTitoli.RiformaTitoli

    Public Sub New(objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri)
        Me.objParametri_Server = objParametri_Server
        Me.objParametri_Utenti = objParametri_Utenti

        configura(username, password, url)

    End Sub

    Public Sub New(username As String, password As String, url As String)

        Me.username = username
        Me.password = password
        Me.url = url

        configura(username, password, url)

    End Sub

    Private Sub configura(username As String, password As String, url As String)
        ws = New RiformaTitoli.RiformaTitoli

        ws.Url = url

        Dim soap_Autenticazione = New RiformaTitoli.SOAPAutenticazione

        soap_Autenticazione.username = username
        soap_Autenticazione.password = password

        ws.SOAPAutenticazioneValue = soap_Autenticazione
    End Sub


    Public Function getTitoli(cuaa As String, campagna As String)
        Dim cuaaObj As New RiformaTitoli.CUAA
        cuaaObj.Item = cuaa
        cuaaObj.ItemElementName = RiformaTitoli.ItemChoiceType.CodiceFiscalePersonaFisica
        Dim Input As New RiformaTitoli.InputTitoliProduttore
        Input.campagna = campagna
        Input.CUAA = cuaaObj
        Return ws.TitoliProduttore(Input)
    End Function

End Class
