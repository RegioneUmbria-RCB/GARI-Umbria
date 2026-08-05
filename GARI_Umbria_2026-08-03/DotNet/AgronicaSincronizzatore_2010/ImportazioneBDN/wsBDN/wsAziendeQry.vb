Imports System.IO
Imports System.Net
Imports System.ServiceModel
Imports System.Xml
Imports System.Xml.Serialization
Imports AgronicaCoreDataProvider

Public Class ChiamawsAziendeQry
    Inherits WSBDN

    Dim soap_Autenticazione As wsAziendeQry.SOAPAutenticazione
    Dim soap_Autenticazione_token As PwsAziendeQry.SOAPAutenticazione
    Dim ws As wsAziendeQry.wsAziendeQrySoapClient
    Dim wsToken As PwsAziendeQry.wsAziendeQrySoapClient

    Public Sub New(objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri, token As String, ruolo As String, valore_ruolo_codice As String)
        MyBase.New(objParametri_Server, objParametri_Utenti, token, ruolo, valore_ruolo_codice)
        initializeWebService()
    End Sub

    Public Sub New(objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri, link As String, username As String, password As String, token As String, ruolo As String, valore_ruolo_codice As String)
        MyBase.New(objParametri_Server, objParametri_Utenti, link, username, password, token, ruolo, valore_ruolo_codice)
        initializeWebService()
    End Sub

    Private Sub initializeWebService()
        serviceEnpoint &= "wsBDNInterrogazioni/wsAziendeQry.asmx"

        'If Me.username <> "" AndAlso Me.password <> "" Then
        '    soap_Autenticazione = New wsAziendeQry.SOAPAutenticazione
        '    CType(soap_Autenticazione, wsAziendeQry.SOAPAutenticazione).username = Me.username
        '    CType(soap_Autenticazione, wsAziendeQry.SOAPAutenticazione).password = Me.password
        'End If
        'If Me.token <> "" Then
        '    soap_Autenticazione_token = New PwsAziendeQry.SOAPAutenticazione
        '    CType(soap_Autenticazione_token, PwsAziendeQry.SOAPAutenticazione).token = Me.token
        '    CType(soap_Autenticazione_token, PwsAziendeQry.SOAPAutenticazione).valore_ruolo_codice = "DET"
        '    CType(soap_Autenticazione_token, PwsAziendeQry.SOAPAutenticazione).ruolo_codice = "DET"
        'End If

        ws = New wsAziendeQry.wsAziendeQrySoapClient(binding, theEndpoint)
    End Sub

    Public Function getAzienda(p_azienda_codice As String) As DataTable
        Try
            If soap_Autenticazione IsNot Nothing Then
                'Dim a = ws.getAzienda(soap_Autenticazione, p_azienda_codice)
                Dim a = SoapRequest(Reflection.MethodBase.GetCurrentMethod().Name, New Dictionary(Of String, String) From {{"p_azienda_codice", p_azienda_codice}})
                Dim dt = MyBase.generateDTfromXml(a)
                Return dt
            Else
                Dim a = SoapRequest(Reflection.MethodBase.GetCurrentMethod().Name, New Dictionary(Of String, String) From {{"p_azienda_codice", p_azienda_codice}})
                Dim dt = MyBase.generateDTfromXml(a)
                Return dt
            End If
        Catch ex As ExpiredTokenBDNException
            Throw ex
        Catch ex As BDNException
            Throw New BDNException("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function getInfoAzienda(p_azienda_id As String) As DataTable
        Try
            'If soap_Autenticazione IsNot Nothing Then
            '    Dim a = ws.getInfoAzienda(soap_Autenticazione, p_azienda_codice)
            '    Dim dt = MyBase.generateDTfromXml(a)
            '    Return dt
            'Else
            '    Dim a = SoapRequest(Reflection.MethodBase.GetCurrentMethod().Name, New Dictionary(Of String, String) From {{"p_azienda_codice", p_azienda_codice}})
            '    Dim dt = MyBase.generateDTfromXml(a)
            '    Return dt
            'End If
            Dim a = SoapRequest(Reflection.MethodBase.GetCurrentMethod().Name, New Dictionary(Of String, String) From {{"p_azienda_id", p_azienda_id}})
            Dim dt = MyBase.generateDTfromXml(a)
            Return dt
        Catch ex As ExpiredTokenBDNException
            Throw ex
        Catch ex As BDNException
            Throw New Exception("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function


    Public Function Estrazione_Allevamenti(p_azienda_codice As String, strDelega As String) As DataTable
        Try
            'If soap_Autenticazione IsNot Nothing Then
            '    Dim a = ws.Estrazione_Allevamenti(soap_Autenticazione, strDelega:=strDelega)
            '    Dim dt = MyBase.generateDTfromXml(a)
            '    Return dt
            'Else
            '    Dim a = SoapRequest(Reflection.MethodBase.GetCurrentMethod().Name, New Dictionary(Of String, String) From {{"p_azienda_codice", p_azienda_codice}, {"strDelega", strDelega}})
            '    Dim dt = MyBase.generateDTfromXml(a)
            '    Return dt
            'End If

            Dim a = SoapRequest(Reflection.MethodBase.GetCurrentMethod().Name, New Dictionary(Of String, String) From {{"p_azienda_codice", p_azienda_codice}, {"strDelega", strDelega}})
            Dim dt = MyBase.generateDTfromXml(a)
            Return dt
        Catch ex As ExpiredTokenBDNException
            Throw ex
        Catch ex As BDNException
            Throw New Exception("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function FindAllevamento(p_azienda_codice As String, p_denominazione As String, p_specie_codice As String) As DataTable
        Try
            'If soap_Autenticazione IsNot Nothing Then
            '    Dim a = ws.FindAllevamento(soap_Autenticazione, p_azienda_codice:=p_azienda_codice, p_denominazione:=p_denominazione, p_specie_codice:=p_specie_codice)
            '    Dim dt = MyBase.generateDTfromXml(a)
            '    Return dt
            'Else
            '    Dim a = SoapRequest(Reflection.MethodBase.GetCurrentMethod().Name, New Dictionary(Of String, String) From {
            '                             {"p_azienda_codice", p_azienda_codice},
            '                             {"p_denominazione", p_denominazione},
            '                             {"p_specie_codice", p_specie_codice}
            '                             })
            '    Dim dt = MyBase.generateDTfromXml(a)
            '    Return dt
            'End If

            Dim a = SoapRequest(Reflection.MethodBase.GetCurrentMethod().Name, New Dictionary(Of String, String) From {
                                         {"p_azienda_codice", p_azienda_codice},
                                         {"p_denominazione", p_denominazione},
                                         {"p_specie_codice", p_specie_codice}
                                         })
            Dim dt = MyBase.generateDTfromXml(a)
            Return dt
        Catch ex As ExpiredTokenBDNException
            Throw ex
        Catch ex As BDNException
            Throw New Exception("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function getAllevamento(p_azienda_codice As String, p_allev_idfiscale As String, p_spe_codice As String) As DataTable
        Try
            'If soap_Autenticazione IsNot Nothing Then
            '    Dim a = ws.getAllevamento(soap_Autenticazione, p_azienda_codice:=p_azienda_codice, p_allev_idfiscale:=p_allev_idfiscale, p_spe_codice:=p_spe_codice)
            '    Dim dt = MyBase.generateDTfromXml(a)
            '    Return dt
            'Else
            '    Dim a = SoapRequest(Reflection.MethodBase.GetCurrentMethod().Name, New Dictionary(Of String, String) From {
            '                             {"p_azienda_codice", p_azienda_codice},
            '                             {"p_allev_idfiscale", p_allev_idfiscale},
            '                             {"p_spe_codice", p_spe_codice}
            '                             })
            '    Dim dt = MyBase.generateDTfromXml(a)
            '    Return dt
            'End If

            Dim a = SoapRequest(Reflection.MethodBase.GetCurrentMethod().Name, New Dictionary(Of String, String) From {
                                         {"p_azienda_codice", p_azienda_codice},
                                         {"p_allev_idfiscale", p_allev_idfiscale},
                                         {"p_spe_codice", p_spe_codice}
                                         })
            Dim dt = MyBase.generateDTfromXml(a)
            Return dt
        Catch ex As ExpiredTokenBDNException
            Throw ex
        Catch ex As BDNException
            Throw New Exception("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function


    Public Function GetSoccidari(p_azienda_codice As String, p_allev_idfiscale As String, p_spe_codice As String, p_storico As String) As DataTable
        Try
            'If soap_Autenticazione IsNot Nothing Then
            '    Dim a = ws.GetSoccidari(soap_Autenticazione, p_azienda_codice, p_allev_idfiscale, p_spe_codice, p_storico)
            '    Dim dt = MyBase.generateDTfromXml(a)
            '    Return dt
            'Else
            '    Dim a = SoapRequest(Reflection.MethodBase.GetCurrentMethod().Name, New Dictionary(Of String, String) From {
            '                             {"p_azienda_codice", p_azienda_codice},
            '                             {"p_allev_idfiscale", p_allev_idfiscale},
            '                             {"p_spe_codice", p_spe_codice},
            '                             {"p_storico", p_storico}
            '                             })
            '    Dim dt = MyBase.generateDTfromXml(a)
            '    Return dt
            'End If

            Dim a = SoapRequest(Reflection.MethodBase.GetCurrentMethod().Name, New Dictionary(Of String, String) From {
                                         {"p_azienda_codice", p_azienda_codice},
                                         {"p_allev_idfiscale", p_allev_idfiscale},
                                         {"p_spe_codice", p_spe_codice},
                                         {"p_storico", p_storico}
                                         })
            Dim dt = MyBase.generateDTfromXml(a)
            Return dt
        Catch ex As ExpiredTokenBDNException
            Throw ex
        Catch ex As BDNException
            Throw New Exception("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function Get_ListaAllevamenti(p_codice_capo As String) As DataTable
        Try
            'If soap_Autenticazione IsNot Nothing Then
            '    Dim a = ws.Get_ListaAllevamenti(soap_Autenticazione, p_codice_capo:=p_codice_capo)
            '    Dim dt = MyBase.generateDTfromXml(a)
            '    Return dt
            'Else
            '    Dim a = SoapRequest(Reflection.MethodBase.GetCurrentMethod().Name, New Dictionary(Of String, String) From {
            '                             {"p_codice_capo", p_codice_capo}
            '                             })
            '    Dim dt = MyBase.generateDTfromXml(a)
            '    Return dt
            'End If

            Dim a = SoapRequest(Reflection.MethodBase.GetCurrentMethod().Name, New Dictionary(Of String, String) From {
                                         {"p_codice_capo", p_codice_capo}
                                         })
            Dim dt = MyBase.generateDTfromXml(a)
            Return dt
        Catch ex As ExpiredTokenBDNException
            Throw ex
        Catch ex As BDNException
            Throw New Exception("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

End Class
