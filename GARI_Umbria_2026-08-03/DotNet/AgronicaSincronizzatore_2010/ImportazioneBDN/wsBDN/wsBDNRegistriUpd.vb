Imports System.ServiceModel
Imports System.Xml
Imports AgronicaCoreDataProvider

Public Class wsBDNRegistriUpd
    Inherits WSBDN

    Dim soap_Autenticazione As wsRegistriUpd.SOAPAutenticazione
    Dim ws As wsRegistriUpd.wsRegistriUpdSoapClient

    Public Sub New(objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri, token As String, ruolo As String, valore_ruolo_codice As String)
        MyBase.New(objParametri_Server, objParametri_Utenti, token, ruolo, valore_ruolo_codice)
        initializeWebService()
    End Sub

    Public Sub New(objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri, link As String, username As String, password As String, token As String, ruolo As String, valore_ruolo_codice As String)
        MyBase.New(objParametri_Server, objParametri_Utenti, link, username, password, token, ruolo, valore_ruolo_codice)
        initializeWebService()
    End Sub

    Private Sub initializeWebService()
        serviceEnpoint &= "wsModelliAccompagnamento/wsInterrogazioni.asmx"

        soap_Autenticazione = New wsRegistriUpd.SOAPAutenticazione

        If Me.username <> "" AndAlso Me.password <> "" Then
            soap_Autenticazione.username = Me.username
            soap_Autenticazione.password = Me.password
        End If
        'If Me.token <> "" Then
        '    soap_Autenticazione.token = Me.token
        'End If
        'soap_Autenticazione.token = Me.token

        ws = New wsRegistriUpd.wsRegistriUpdSoapClient(binding, theEndpoint)

    End Sub

    Public Function getListaPrenotazioniModelli(p_data_da As Date,
                                             p_data_a As Date,
                                             p_asl_codice_prov As String,
                                             p_azienda_codice_prov As String,
                                             p_tipo_dest As String,
                                             p_stato_modello As String,
                                             p_asl_codice_dest As String,
                                             p_codice_struttura_dest As String,
                                             p_regione_codice_dest As String) As DataTable
        Try




            Dim str_data_da = getStrDataFromDate(p_data_da)
            Dim str_data_a = getStrDataFromDate(p_data_a)

            Dim a = ws.Insert_Ingresso(soap_Autenticazione, "")

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
