Imports System.Net
Imports System.ServiceModel
Imports AgronicaCoreDataProvider

Public Class ChiamawsTerritorioQry
    Inherits WSBDN

    Dim soap_Autenticazione As wsTerritorioQry.SOAPAutenticazione
    Dim ws As wsTerritorioQry.wsTerritorioQrySoapClient

    Public Sub New(objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri, token As String, ruolo As String, valore_ruolo_codice As String)
        MyBase.New(objParametri_Server, objParametri_Utenti, token, ruolo, valore_ruolo_codice)
        initializeWebService()
    End Sub

    Public Sub New(objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri, link As String, username As String, password As String, token As String, ruolo As String, valore_ruolo_codice As String)
        MyBase.New(objParametri_Server, objParametri_Utenti, link, username, password, token, ruolo, valore_ruolo_codice)
        initializeWebService()
    End Sub

    Private Sub initializeWebService()
        serviceEnpoint &= "wsBDNInterrogazioni/wsTerritorioQry.asmx"
        soap_Autenticazione = New wsTerritorioQry.SOAPAutenticazione

        'If Me.username <> "" AndAlso Me.password <> "" Then
        '    soap_Autenticazione.username = Me.username
        '    soap_Autenticazione.password = Me.password
        'End If
        'If Me.token <> "" Then
        '    soap_Autenticazione.token = Me.token
        'End If

        ws = New wsTerritorioQry.wsTerritorioQrySoapClient(binding, theEndpoint)
    End Sub

    Public Function DownloadAziendeUSL() As DataTable
        Try
            Dim seBK = ServicePointManager.SecurityProtocol
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

            Dim a = Me.SoapRequest(Reflection.MethodBase.GetCurrentMethod().Name, Nothing)
            ServicePointManager.SecurityProtocol = seBK

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

    Public Function DownloadComuni() As DataTable
        Try
            Dim seBK = ServicePointManager.SecurityProtocol
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

            Dim a = Me.SoapRequest(Reflection.MethodBase.GetCurrentMethod().Name, Nothing)
            ServicePointManager.SecurityProtocol = seBK

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

    Public Function DownloadDistretti() As DataTable
        Try
            Dim seBK = ServicePointManager.SecurityProtocol
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

            Dim a = Me.SoapRequest(Reflection.MethodBase.GetCurrentMethod().Name, Nothing)
            ServicePointManager.SecurityProtocol = seBK

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

    Public Function DownloadProvince() As DataTable
        Try
            Dim seBK = ServicePointManager.SecurityProtocol
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

            Dim a = Me.SoapRequest(Reflection.MethodBase.GetCurrentMethod().Name, Nothing)
            ServicePointManager.SecurityProtocol = seBK

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

    Public Function DownloadRegioni() As DataTable
        Try
            Dim seBK = ServicePointManager.SecurityProtocol
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

            Dim a = Me.SoapRequest(Reflection.MethodBase.GetCurrentMethod().Name, Nothing)
            ServicePointManager.SecurityProtocol = seBK

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

    Public Function DownloadStati() As DataTable
        Try
            Dim seBK = ServicePointManager.SecurityProtocol
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

            Dim a = Me.SoapRequest(Reflection.MethodBase.GetCurrentMethod().Name, Nothing)
            ServicePointManager.SecurityProtocol = seBK

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

    Public Function DownloadCategorie() As DataTable
        Try
            Dim seBK = ServicePointManager.SecurityProtocol
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

            Dim a = Me.SoapRequest(Reflection.MethodBase.GetCurrentMethod().Name, Nothing)
            ServicePointManager.SecurityProtocol = seBK

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

    Public Function DownloadAssociazioni() As DataTable
        Try
            Dim seBK = ServicePointManager.SecurityProtocol
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

            Dim a = Me.SoapRequest(Reflection.MethodBase.GetCurrentMethod().Name, Nothing)
            ServicePointManager.SecurityProtocol = seBK

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
