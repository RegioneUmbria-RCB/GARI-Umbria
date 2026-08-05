Imports System.IO
Imports System.ServiceModel
Imports System.Xml
Imports System.Xml.Serialization
Imports AgronicaCoreDataProvider

Public Class ChiamawsAnagraficaCapoQry
    Inherits WSBDN

    Dim soap_Autenticazione As wsAnagraficaCapoQry.SOAPAutenticazione
    Dim ws As wsAnagraficaCapoQry.wsAnagraficaCapoQrySoapClient

    Public Sub New(objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri, token As String, ruolo As String, valore_ruolo_codice As String)
        MyBase.New(objParametri_Server, objParametri_Utenti, token, ruolo, valore_ruolo_codice)
        initializeWebService()
    End Sub

    Public Sub New(objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri, link As String, username As String, password As String, token As String, ruolo As String, valore_ruolo_codice As String)
        MyBase.New(objParametri_Server, objParametri_Utenti, link, username, password, token, ruolo, valore_ruolo_codice)
        initializeWebService()
    End Sub

    Private Sub initializeWebService()
        serviceEnpoint &= "wsBDNInterrogazioni/wsAnagraficaCapoQry.asmx"
        soap_Autenticazione = New wsAnagraficaCapoQry.SOAPAutenticazione

        'If Me.username <> "" AndAlso Me.password <> "" Then
        '    soap_Autenticazione.username = Me.username
        '    soap_Autenticazione.password = Me.password
        'End If
        If (Me.token Is Nothing OrElse Me.token = "") AndAlso Me.username = "" AndAlso TokenHandler.GetInstance.BDNtoken <> "" Then
            soap_Autenticazione.token = TokenHandler.GetInstance.BDNtoken
        Else
            soap_Autenticazione.token = Me.token
        End If

        'End If

        ws = New wsAnagraficaCapoQry.wsAnagraficaCapoQrySoapClient(binding, theEndpoint)

        'wsRegistroStallaQry
    End Sub

    Public Function findCapoBovino(p_capo_id As String) As DataTable
        Try
            Dim a = ws.findCapoBovino(soap_Autenticazione, p_capo_id:=p_capo_id)
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

    Public Function FindCapoMacellato(p_capo_codice As String) As DataTable
        Try
            Dim a = ws.FindCapoMacellato(soap_Autenticazione, p_capo_codice:=p_capo_codice)
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

    Public Function getCapo(p_capo_codice As String) As DataTable
        Try
            'Dim xml = ws.getCapo(soap_Autenticazione, p_capo_codice:=p_capo_codice)
            Dim xml = SoapRequest(Reflection.MethodBase.GetCurrentMethod().Name, New Dictionary(Of String, String) From {{"p_capo_codice", p_capo_codice}})
            Dim nodoDati = (From a As XmlNode In xml.ChildNodes Where a.Name = "dati").FirstOrDefault
            If nodoDati Is Nothing Then
                Dim xml_doc1 As New Xml.XmlDocument
                xml_doc1.LoadXml(xml.OuterXml)
                Dim child_nodes1 As XmlNodeList = xml_doc1.GetElementsByTagName("dati")
                If child_nodes1.Count > 0 Then
                    nodoDati = child_nodes1(0)
                End If
            End If
            Dim reader2 As New StringReader(nodoDati.OuterXml)
            Dim dsCapi = New dsCAPI_G
            Try
                dsCapi.ReadXml(reader2)
            Catch ex As Exception

            End Try
            Dim dt = MyBase.generateDTfromXml(xml)
            Return dt
        Catch ex As ExpiredTokenBDNException
            Throw ex
        Catch ex As BDNException
            Throw New Exception("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function getCapoMacellato(p_capo_codice As String) As DataTable
        Try
            Dim a = ws.getCapoMacellato(soap_Autenticazione, p_capo_codice:=p_capo_codice)
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
    Public Function Get_Capi_Allevamento(p_allev_id As Integer,
                                         p_data_dal As String,
                                         p_data_al As String,
                                         Optional p_azienda_codice As String = "",
                                         Optional p_allev_idfiscale As String = "",
                                         Optional p_spe_codice As String = "",
                                         Optional p_storico As String = "",
                                         Optional p_cod_capo As String = "") As DataTable
        Try
            Dim str_data_da = getStrDataFromDate(p_data_dal)
            Dim str_data_a = getStrDataFromDate(p_data_al)
            Dim a = SoapRequest(
                Reflection.MethodBase.GetCurrentMethod().Name,
                New Dictionary(Of String, String) From {
                {"p_allev_id", p_allev_id},
                {"p_azienda_codice", p_azienda_codice},
                {"p_allev_idfiscale", p_allev_idfiscale},
                {"p_spe_codice", p_spe_codice},
                {"p_storico", p_storico},
                {"p_cod_capo", p_cod_capo},
                {"p_data_dal", p_data_dal},
                {"p_data_al", p_data_al}
                })
            Dim dt = MyBase.generateDTfromXml(a)
            Return dt
        Catch ex As ExpiredTokenBDNException
            Throw ex
        Catch ex As BDNException
            Throw New BDNException("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)
        Catch ex As Exception
            Return Nothing
        End Try
        'Try
        '    Dim a = ws.Get_Capi_Allevamento(soap_Autenticazione,
        '                                    p_allev_id:=p_allev_id,
        '                                    p_storico:=p_storico,
        '                                    p_cod_capo:=p_cod_capo)
        '    Dim dt = MyBase.generateDTfromXml(a)
        '    Return dt
        'Catch ex As BDNException
        '    Throw New Exception("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)
        'Catch ex As Exception
        '    Return Nothing
        'End Try
    End Function

    Public Function getDecesso(p_azienda_codice As String, p_allev_idfiscale As String, p_spe_codice As String, p_codice_capo As String) As DataTable
        Try
            'Dim a = ws.getDecesso(soap_Autenticazione, p_azienda_codice:=p_azienda_codice, p_allev_idfiscale:=p_allev_idfiscale, p_spe_codice:=p_spe_codice, p_codice_capo:=p_codice_capo)
            Dim a = SoapRequest(Reflection.MethodBase.GetCurrentMethod().Name,
                                New Dictionary(Of String, String) From {
                                {"p_azienda_codice", p_azienda_codice},
                                {"p_allev_idfiscale", p_allev_idfiscale},
                                {"p_spe_codice", p_spe_codice},
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

    Public Function getFurto(p_codice_capo As String) As DataTable
        Try
            Dim a = ws.getFurto(soap_Autenticazione,
                                  p_codice_capo:=p_codice_capo)
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

    Public Function getInfoCapiAllevamento(p_allev_id As String) As DataTable
        Try

            Dim a = SoapRequest(
                Reflection.MethodBase.GetCurrentMethod().Name,
                New Dictionary(Of String, String) From {
                {"p_allev_id", p_allev_id}
                })
            Dim dt = MyBase.generateDTfromXml(a)
            Return dt
        Catch ex As ExpiredTokenBDNException
            Throw ex
        Catch ex As BDNException
            Throw New BDNException("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)
        Catch ex As Exception
            Return Nothing
        End Try
        'Try
        '    Dim a = ws.getInfoCapiAllevamento(soap_Autenticazione,
        '                          p_allev_id:=p_allev_id)
        '    Dim dt = MyBase.generateDTfromXml(a)
        '    Return dt
        'Catch ex As BDNException
        '    Throw New Exception("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)
        'Catch ex As Exception
        '    Return Nothing
        'End Try
    End Function

    Public Function getMadri(p_allev_id As String, p_data As Date) As DataTable
        Try
            Dim a = ws.getMadri(soap_Autenticazione,
                                  p_allev_id:=p_allev_id,
                                  p_data:=p_data.Day & "/" & p_data.Month & "/" & p_data.Year)
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
