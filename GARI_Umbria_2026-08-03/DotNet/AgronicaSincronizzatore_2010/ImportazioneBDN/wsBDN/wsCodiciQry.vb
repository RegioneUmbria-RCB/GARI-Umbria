Imports System.ServiceModel
Imports AgronicaCoreDataProvider

Public Class ChiamawsCodiciQry
    Inherits WSBDN

    Dim soap_Autenticazione As wsCodiciQry.SOAPAutenticazione
    Dim ws As wsCodiciQry.wsCodiciQrySoapClient

    Public Sub New(objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri, token As String, ruolo As String, valore_ruolo_codice As String)
        MyBase.New(objParametri_Server, objParametri_Utenti, token, ruolo, valore_ruolo_codice)
        initializeWebService()
    End Sub

    Public Sub New(objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri, link As String, username As String, password As String, token As String, ruolo As String, valore_ruolo_codice As String)
        MyBase.New(objParametri_Server, objParametri_Utenti, link, username, password, token, ruolo, valore_ruolo_codice)
        initializeWebService()
    End Sub


    Private Sub initializeWebService()
        serviceEnpoint &= "wsBDNInterrogazioni/wsCodiciQry.asmx"

        soap_Autenticazione = New wsCodiciQry.SOAPAutenticazione

        If Me.username <> "" AndAlso Me.password <> "" Then
            soap_Autenticazione.username = Me.username
            soap_Autenticazione.password = Me.password
        End If
        'If Me.token <> "" Then
        '    soap_Autenticazione.token = Me.token
        'End If

        ws = New wsCodiciQry.wsCodiciQrySoapClient(binding, theEndpoint)
    End Sub

    Public Function DownloadAccessori() As DataTable
        Try
            Dim a = ws.DownloadAccessori(soap_Autenticazione)
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

    Public Function DownloadCausaliMorte() As DataTable
        Try
            Dim a = ws.DownloadCausaliMorte(soap_Autenticazione)
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

    Public Function DownloadClassificazioni() As DataTable
        Try
            Dim a = ws.DownloadClassificazioni(soap_Autenticazione)
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

    Public Function DownloadCriteriControlli() As DataTable
        Try
            Dim a = ws.DownloadCriteriControlli(soap_Autenticazione)
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
    Public Function DownloadEsitiTest() As DataTable
        Try
            Dim a = ws.DownloadEsitiTest(soap_Autenticazione)
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


    Public Function DownloadGruppiSpecie() As DataTable
        Try
            Dim a = ws.DownloadGruppiSpecie(soap_Autenticazione)
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

    Public Function DownloadLibriGenealogici() As DataTable
        Try
            Dim a = ws.DownloadLibriGenealogici(soap_Autenticazione)
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

    Public Function DownloadMalattie_QualificheSanitarie() As DataTable
        Try
            Dim a = SoapRequest(Reflection.MethodBase.GetCurrentMethod().Name, Nothing)
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

    Public Function DownloadTipi_Qualifiche_Sanitarie() As DataTable
        Try
            Dim a = SoapRequest(Reflection.MethodBase.GetCurrentMethod().Name, Nothing)
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

    Public Function DownloadTipologiaRisultatoTest() As DataTable
        Try
            Dim a = SoapRequest(Reflection.MethodBase.GetCurrentMethod().Name, Nothing)
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

    Public Function DownloadMotiviIngresso() As DataTable
        Try
            Dim a = ws.DownloadMotiviIngresso(soap_Autenticazione)
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

    Public Function DownloadMotiviUscita() As DataTable
        Try
            Dim a = ws.DownloadMotiviUscita(soap_Autenticazione)
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
    Public Function DownloadOrientamentiProduttivi() As DataTable
        Try
            Dim a = ws.DownloadOrientamentiProduttivi(soap_Autenticazione)
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

    Public Function DownloadProduttoriMarche() As DataTable
        Try
            Dim a = ws.DownloadProduttoriMarche(soap_Autenticazione)
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

    Public Function DownloadRazze() As DataTable
        Try
            Dim a = ws.DownloadRazze(soap_Autenticazione)
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

    Public Function DownloadSpecie() As DataTable
        Try
            Dim a = ws.DownloadSpecie(soap_Autenticazione)
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

    Public Function DownloadSuiTipiCategorie() As DataTable
        Try
            Dim a = ws.DownloadSuiTipiCategorie(soap_Autenticazione)
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

    Public Function DownloadTipiAllevamento() As DataTable
        Try
            Dim a = ws.DownloadTipiAllevamento(soap_Autenticazione)
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

    Public Function DownloadTipiCodice() As DataTable
        Try
            Dim a = ws.DownloadTipiCodice(soap_Autenticazione)
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

    Public Function DownloadTipiIrregolarita() As DataTable
        Try
            Dim a = ws.DownloadTipiIrregolarita(soap_Autenticazione)
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

    Public Function DownloadTipiPassaporti() As DataTable
        Try
            Dim a = ws.DownloadTipiPassaporti(soap_Autenticazione)
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

    Public Function DownloadTipiProduzione() As DataTable
        Try
            Dim a = ws.DownloadTipiProduzione(soap_Autenticazione)
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

    Public Function DownloadTipiProvenienza() As DataTable
        Try
            Dim a = ws.DownloadTipiProvenienza(soap_Autenticazione)
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

    Public Function DownloadTipiStatoCapo() As DataTable
        Try
            Dim a = ws.DownloadTipiStatoCapo(soap_Autenticazione)
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

    Public Function DownloadTipi_Malattie() As DataTable
        Try
            Dim a = SoapRequest(Reflection.MethodBase.GetCurrentMethod().Name, Nothing)
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

    Public Function DownloadTipiTipologieProduttive() As DataTable
        Try
            Dim a = ws.DownloadTipiTipologieProduttive(soap_Autenticazione)
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
