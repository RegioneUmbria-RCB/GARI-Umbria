Imports System.ServiceModel
Imports AgronicaCoreDataProvider

Public Class ChiamawsStruttureQry
    Inherits WSBDN

    Dim soap_Autenticazione As wsStruttureQry.SOAPAutenticazione
    Dim ws As wsStruttureQry.wsStruttureQrySoapClient

    Public Sub New(objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri, token As String, ruolo As String, valore_ruolo_codice As String)
        MyBase.New(objParametri_Server, objParametri_Utenti, token, ruolo, valore_ruolo_codice)
        initializeWebService()
    End Sub

    Public Sub New(objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri, link As String, username As String, password As String, token As String, ruolo As String, valore_ruolo_codice As String)
        MyBase.New(objParametri_Server, objParametri_Utenti, link, username, password, token, ruolo, valore_ruolo_codice)
        initializeWebService()
    End Sub

    Private Sub initializeWebService()
        serviceEnpoint &= "wsBDNInterrogazioni/wsStruttureQry.asmx"

        soap_Autenticazione = New wsStruttureQry.SOAPAutenticazione

        'If Me.username <> "" AndAlso Me.password <> "" Then
        '    soap_Autenticazione.username = Me.username
        '    soap_Autenticazione.password = Me.password
        'End If

        'If Me.token <> "" Then
        '    soap_Autenticazione.token = Me.token
        'End If

        ws = New wsStruttureQry.wsStruttureQrySoapClient(binding, theEndpoint)
    End Sub


    Public Function getInfo_Strutture(p_azienda_codice As String, p_allev_id_fiscale As String, p_spe_codice As String) As DataTable
        Try
            Dim a = ws.getInfo_Strutture(SOAPAutenticazione:=soap_Autenticazione,
                                         p_azienda_codice:=p_azienda_codice,
                                         p_allev_id_fiscale:=p_allev_id_fiscale,
                                         p_spe_codice:=p_spe_codice)
            Dim dt = MyBase.generateDTfromXml(a)
            Return dt
        Catch ex As ExpiredTokenBDNException
            Throw ex
        Catch ex As BDNException
            Throw New BDNException("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

End Class
