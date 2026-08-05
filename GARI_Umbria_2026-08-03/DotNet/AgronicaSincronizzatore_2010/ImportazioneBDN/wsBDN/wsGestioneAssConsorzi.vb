Imports System.ServiceModel
Imports AgronicaCoreDataProvider

Public Class ChiamawsGestioneAssConsorzi
    Inherits WSBDN

    Dim soap_Autenticazione As wsAssociazioniAziende.SOAPAutenticazione
    Dim ws As wsAssociazioniAziende.wsAssociazioniAziendeSoapClient

    Public Sub New(objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri, token As String, ruolo As String, valore_ruolo_codice As String)
        MyBase.New(objParametri_Server, objParametri_Utenti, token, ruolo, valore_ruolo_codice)
        initializeWebService()
    End Sub

    Public Sub New(objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri, link As String, username As String, password As String, token As String, ruolo As String, valore_ruolo_codice As String)
        MyBase.New(objParametri_Server, objParametri_Utenti, link, username, password, token, ruolo, valore_ruolo_codice)
        initializeWebService()
    End Sub

    Public Sub initializeWebService()
        Me.serviceEnpoint &= "wsGestioneAssConsorzi/wsAssociazioniAziende.asmx"
        Me.soap_Autenticazione = New wsAssociazioniAziende.SOAPAutenticazione
        Me.ws = New wsAssociazioniAziende.wsAssociazioniAziendeSoapClient(binding, theEndpoint)
    End Sub


    Public Function Delete_Azienda_Consorzio(strRecord As String) As DataTable
        Try
            Dim a = ws.Delete_Azienda_Consorzio(soap_Autenticazione, strRecord:=strRecord)
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

    Public Function Insert_Azienda_Consorzio(p_aztra_id As String, p_azienda_codice As String, p_asspro_codice As String) As DataTable
        Try
            Dim strRecord As String = "&lt;dsAZIENDE_TRAMITE_IUS xmlns=&quot;http://bdr.izs.it/XMLSchema/dsAZIENDE_TRAMITE_IUS.xsd&quot;&gt;&lt;PARAMETERS_LIST&gt;&lt;P_AZTRA_ID&gt;" & p_aztra_id & "&lt;/P_AZTRA_ID&gt;&lt;P_AZIENDA_CODICE&gt;" & p_azienda_codice & "&lt;/P_AZIENDA_CODICE&gt;&lt;P_ASSPRO_CODICE&gt;" & p_asspro_codice & "&lt;/P_ASSPRO_CODICE&gt;&lt;/PARAMETERS_LIST&gt;&lt;/dsAZIENDE_TRAMITE_IUS&gt;"

            'Dim a = ws.Insert_Azienda_Consorzio(soap_Autenticazione, strRecord:=strRecord)
            Dim a = SoapRequestV2(strRecord, Reflection.MethodBase.GetCurrentMethod().Name)
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

    Public Function Get_Aziende_Consorzio(p_azienda_codice As String, p_asspro_codice As String) As DataTable
        Try
            'Dim a = ws.Get_Aziende_Consorzio(soap_Autenticazione, p_azienda_codice:=p_azienda_codice, p_asspro_codice:=p_asspro_codice)
            Dim xml = SoapRequest(Reflection.MethodBase.GetCurrentMethod().Name, New Dictionary(Of String, String) From {{"p_azienda_codice", p_azienda_codice}, {"p_asspro_codice", p_asspro_codice}})
            Dim dt = MyBase.generateDTfromXml(xml)
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
