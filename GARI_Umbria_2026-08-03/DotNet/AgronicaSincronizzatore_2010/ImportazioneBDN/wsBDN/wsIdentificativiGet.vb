Imports System.ServiceModel
Imports AgronicaCoreDataProvider

Public Class ChiamawsIdentificativiGet
    Inherits WSBDN

    Dim soap_Autenticazione As wsIdentificativiGet.SOAPAutenticazione
    Dim ws As wsIdentificativiGet.wsIdentificativiGetSoapClient

    Public Sub New(objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri, token As String, ruolo As String, valore_ruolo_codice As String)
        MyBase.New(objParametri_Server, objParametri_Utenti, token, ruolo, valore_ruolo_codice)
        initializeWebService()
    End Sub

    Public Sub New(objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri, link As String, username As String, password As String, token As String, ruolo As String, valore_ruolo_codice As String)
        MyBase.New(objParametri_Server, objParametri_Utenti, link, username, password, token, ruolo, valore_ruolo_codice)
        initializeWebService()
    End Sub


    Private Sub initializeWebService()
        serviceEnpoint &= "wsBDNInterrogazioni/wsIdentificativiGet.asmx"

        soap_Autenticazione = New wsIdentificativiGet.SOAPAutenticazione

        'If Me.username <> "" AndAlso Me.password <> "" Then
        '    soap_Autenticazione.username = Me.username
        '    soap_Autenticazione.password = Me.password
        'End If
        'If Me.token <> "" Then
        '    soap_Autenticazione.token = Me.token
        'End If

        ws = New wsIdentificativiGet.wsIdentificativiGetSoapClient(binding, theEndpoint)

    End Sub

    Public Function DownloadAziendeUSL() As String
        Try
            Dim a
            Return a.OuterXml
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

End Class
