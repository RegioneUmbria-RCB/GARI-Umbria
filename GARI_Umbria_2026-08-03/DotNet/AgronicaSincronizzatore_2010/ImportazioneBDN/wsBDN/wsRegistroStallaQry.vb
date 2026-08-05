Imports System.IO
Imports System.ServiceModel
Imports System.Xml
Imports AgronicaCoreDataProvider

Public Class ChiamawsRegistroStallaQry
    Inherits WSBDN

    Dim soap_Autenticazione As wsRegistroStallaQry.SOAPAutenticazione
    Dim ws As wsRegistroStallaQry.wsRegistroStallaQrySoapClient
    Public Sub New(objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri, token As String, ruolo As String, valore_ruolo_codice As String)
        MyBase.New(objParametri_Server, objParametri_Utenti, token, ruolo, valore_ruolo_codice)
        initializeWebService()
    End Sub

    Public Sub New(objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri, link As String, username As String, password As String, token As String, ruolo As String, valore_ruolo_codice As String)
        MyBase.New(objParametri_Server, objParametri_Utenti, link, username, password, token, ruolo, valore_ruolo_codice)
        initializeWebService()
    End Sub

    Private Sub initializeWebService()
        serviceEnpoint &= "wsBDNInterrogazioni/wsRegistroStallaQry.asmx"
        soap_Autenticazione = New wsRegistroStallaQry.SOAPAutenticazione

        'If Me.username <> "" AndAlso Me.password <> "" Then
        '    soap_Autenticazione.username = Me.username
        '    soap_Autenticazione.password = Me.password
        'End If
        'If Me.token <> "" Then
        '    soap_Autenticazione.token = Me.token
        'End If


        ws = New wsRegistroStallaQry.wsRegistroStallaQrySoapClient(binding, theEndpoint)
    End Sub

    Public Function getMovimentazioniCapo(p_capo_codice As String) As DataTable
        Try
            'Dim xml = ws.getMovimentazioniCapo(soap_Autenticazione, codice_capo)
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
            Dim dsMovimentazioni = New dsMOVIMENTAZIONI_G
            Try
                dsMovimentazioni.ReadXml(reader2)
            Catch ex As Exception

            End Try
            Dim dt = MyBase.generateDTfromXml(xml)
            Return dsMovimentazioni.Tables(0)

        Catch ex As ExpiredTokenBDNException
            Throw ex
        Catch ex As BDNException
            Throw New BDNException("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function getRegistriStalla(codice_capo As String, azienda_codice As String, allev_idFiscale As String, spe_codice As String, dt_ingresso As Date) As DataTable
        Try
            'Dim a = ws.getRegistriStalla(soap_Autenticazione, codice_capo, azienda_codice, allev_idFiscale, spe_codice, dt_ingresso)
            Dim a = SoapRequest(Reflection.MethodBase.GetCurrentMethod().Name,
                                New Dictionary(Of String, String) From {
                                {"p_capo_codice", codice_capo},
                                {"p_azienda_codice", azienda_codice},
                                {"p_allev_idfiscale", allev_idFiscale},
                                {"p_spe_codice", spe_codice},
                                {"p_dt_ingresso", dt_ingresso.ToString("yyyy-MM-dd")}
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
    End Function

    Public Function Capi_In_Stalla(p_allev_id As String, Optional p_ordinamento As String = "MATRICOLA") As DataTable
        Try
            'Dim a = ws.Capi_In_Stalla(soap_Autenticazione, allev_id, Ordinamento)
            'Dim dt = MyBase.generateDTfromXml(a)
            'Return dt
            Dim a = SoapRequest(Reflection.MethodBase.GetCurrentMethod().Name, New Dictionary(Of String, String) From {{"p_allev_id", p_allev_id}, {"p_ordinamento", p_ordinamento}})
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
