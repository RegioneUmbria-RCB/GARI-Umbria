Imports System.IO
Imports System.Net
Imports System.ServiceModel
Imports System.Threading
Imports System.Xml
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreModelsSTD.exceptions

Public MustInherit Class WSBDN

    Protected objParametri_Server As AgronicaCoreParametri
    Protected objParametri_Utenti As AgronicaCoreParametri

    Protected serviceEnpoint As String
    Protected username As String
    Protected password As String
    Protected ruolo As String
    Protected valore_ruolo_codice As String
    Protected token As String

    Protected binding As BasicHttpBinding
    Protected theEndpoint As EndpointAddress

    Private tokenHandler As TokenHandler

    Private objLog As AgronicaCoreDataProvider.LogProvider
    Private customLOGParams As CustomLOGParams
    Private logDirectory As String
    'Private logFileName As String

    Private log As Boolean = False

    Private cache As Hashtable

    Protected Sub New(objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri, token As String, ruolo As String, valore_ruolo_codice As String)
        Me.objParametri_Server = objParametri_Server
        Me.objParametri_Utenti = objParametri_Utenti

        Dim confSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        Dim configurazioneBDNVetInfo = confSiti.leggiConfigurazioneBDNVetInfo(objParametri_Server)
        If configurazioneBDNVetInfo IsNot Nothing Then
            log = configurazioneBDNVetInfo.logMsg
            Me.serviceEnpoint = configurazioneBDNVetInfo.BDN.link
            'Me.serviceEnpoint = "http://bdrtest.izs.it/"
            Me.username = configurazioneBDNVetInfo.BDN.username
            Me.password = configurazioneBDNVetInfo.BDN.password
        Else
            Throw New GiasException("Collegamento con la BDN non configurato correttamente, contattare l'assistenza")
        End If
        'End If
        'End If
        tokenHandler = TokenHandler.GetInstance
        Me.ruolo = ruolo
        Me.valore_ruolo_codice = valore_ruolo_codice
        Me.token = token
        If Me.token Is Nothing OrElse Me.token = "" Then
            Me.token = tokenHandler.BDNtoken
        End If
        'If Debugger.IsAttached Then
        '    If serviceEnpoint = "" Then
        '        serviceEnpoint = "http://bdrizsam.izs.it/wsBDNInterrogazioni/wsRegistroStallaQry.asmx"
        '    End If

        '    If username = "" Then
        '        username = "wsinalca_MAC"
        '    End If

        '    If password = "" Then
        '        password = "inalc@2022"
        '    End If
        'End If
        '    If password = "" Then
        '        password = "inalc@2022"
        '    End If
        'End If

        If serviceEnpoint = "" Then
            Throw New GiasException("Endpoint servizio Integrazione BDN non configurato correttamente")
        End If

        If (username = "" OrElse password = "") AndAlso token = "" Then
            Throw New GiasException("Credenziali servizio Integrazione BDN non configurate correttamente")
        End If

        If token <> "" AndAlso Not serviceEnpoint.EndsWith("pub/") Then
            serviceEnpoint &= "pub/"
        End If


        binding = New BasicHttpBinding With {
                .Name = "BDN",
                .MaxReceivedMessageSize = Integer.MaxValue
        }
        CType(binding, BasicHttpBinding).Security.Mode = BasicHttpSecurityMode.None

        theEndpoint = New EndpointAddress(New Uri(serviceEnpoint))

        If ServicePointManager.SecurityProtocol <> SecurityProtocolType.Tls12 Then
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
        End If

#If DEBUG Then
        System.Net.ServicePointManager.ServerCertificateValidationCallback = AddressOf AcceptAllCertifications
#End If

        binding = New BasicHttpBinding With {
            .Name = "SolutionDOC_HubSoap",
            .MaxReceivedMessageSize = Integer.MaxValue
        }
        binding.Security.Mode = BasicHttpSecurityMode.None
        theEndpoint = New EndpointAddress(New Uri(serviceEnpoint))

        binding = New BasicHttpBinding With {
            .Name = "SolutionDOC_HubSoap",
            .MaxReceivedMessageSize = Integer.MaxValue
        }
        binding.Security.Mode = BasicHttpSecurityMode.None
        theEndpoint = New EndpointAddress(New Uri(serviceEnpoint))

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 Or SecurityProtocolType.Tls13
#If DEBUG Then
        System.Net.ServicePointManager.ServerCertificateValidationCallback = AddressOf AcceptAllCertifications
#End If


        Me.cache = New Hashtable()

        initializeLog()
    End Sub

    Protected Sub New(objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri, link As String, username As String, password As String, token As String, ruolo As String, valore_ruolo_codice As String)
        Me.objParametri_Server = objParametri_Server
        Me.objParametri_Utenti = objParametri_Utenti
        Me.serviceEnpoint = link
        Me.username = username
        Me.password = password
        'tokenHandler = TokenHandler.GetInstance
        Me.token = token
        Me.ruolo = ruolo
        Me.valore_ruolo_codice = valore_ruolo_codice

        Me.cache = New Hashtable()

        initializeLog()
    End Sub

    Sub initializeLog()
        objLog = New LogProvider
        logDirectory = Me.objParametri_Server.LogDirectory & "\BDN\"
        'logFileName = "chiamateBDN.txt"

        customLOGParams = New CustomLOGParams With {
            .LogDescrizioneUtente = objParametri_Server.LogDescrizioneUtente,
            .LogDirectory = logDirectory,
            .LogFileName = "chiamateBDN.txt"
        }

    End Sub


    Protected Function generateDTfromXml(xml As XmlNode) As DataTable
        Dim dt As New DataTable
        Dim nodoDati = (From a As XmlNode In xml.ChildNodes Where a.Name = "dati").FirstOrDefault

        If nodoDati Is Nothing Then
            Dim xml_doc1 As New Xml.XmlDocument
            xml_doc1.LoadXml(xml.OuterXml)
            Dim child_nodes1 As XmlNodeList = xml_doc1.GetElementsByTagName("dati")
            If child_nodes1.Count > 0 Then
                nodoDati = child_nodes1(0)
            End If
        End If

        If nodoDati IsNot Nothing AndAlso
            nodoDati.HasChildNodes AndAlso
            nodoDati.ChildNodes.Count > 0 AndAlso
            nodoDati.ChildNodes(0).HasChildNodes Then
            Dim righe = (From ri As XmlNode In nodoDati.ChildNodes(0).ChildNodes()).ToList
            Dim i = 0
            For Each riga As XmlNode In righe
                If riga.OuterXml Is Nothing Then
                    Continue For
                End If
                If riga.OuterXml.Trim = "" Then
                    Continue For
                End If
                If i = 0 Then
                    If riga.HasChildNodes Then
                        For Each col As XmlNode In riga.ChildNodes
                            If col.Name <> "#whitespace" Then
                                dt.Columns.Add(New DataColumn(col.Name, GetType(String)))
                            End If
                        Next
                    End If
                End If
                Dim dr = dt.NewRow

                For Each col As XmlNode In riga.ChildNodes
                    If col.Name <> "#whitespace" Then
                        If Not dt.Columns.Contains(col.Name) Then
                            dt.Columns.Add(New DataColumn(col.Name, GetType(String)))
                        End If
                        dr(col.Name) = col.InnerText
                    End If
                Next

                dt.Rows.Add(dr)
                i = i + 1
            Next
            Return dt
        End If
        Dim nodoErroreInfo = (From a As XmlNode In xml.ChildNodes Where a.Name = "error_info").FirstOrDefault
        If nodoErroreInfo IsNot Nothing AndAlso
            nodoErroreInfo.HasChildNodes AndAlso
            nodoErroreInfo.ChildNodes.Count > 0 AndAlso
            nodoErroreInfo.ChildNodes(0).HasChildNodes Then
            Dim nodoErrore = (From a As XmlNode In nodoErroreInfo.ChildNodes Where a.Name = "error").FirstOrDefault

            Dim erroreDes = (From a As XmlNode In nodoErrore.ChildNodes Where a.Name = "des").FirstOrDefault()
            If erroreDes IsNot Nothing Then
                If erroreDes.InnerText <> "" Then
                    If erroreDes.InnerText.Contains("TOKEN SCADUTO") Then
                        Throw New ExpiredTokenBDNException(erroreDes.InnerText)
                    End If
                    Throw New BDNException(erroreDes.InnerText)
                End If
            End If
        End If

        Dim xml_doc As New Xml.XmlDocument
        xml_doc.LoadXml(xml.OuterXml)
        Dim child_nodes As XmlNodeList = xml_doc.GetElementsByTagName("error")
        If child_nodes IsNot Nothing AndAlso child_nodes.Count > 0 Then
            Dim errorDesNode = (From a As XmlNode In child_nodes(0).ChildNodes Where a.Name = "des").FirstOrDefault
            If errorDesNode IsNot Nothing AndAlso errorDesNode.InnerText <> "" AndAlso Not errorDesNode.InnerText.Contains("TOKEN SCADUTO") Then
                Throw New BDNException(errorDesNode.InnerText)
            End If

            If errorDesNode IsNot Nothing AndAlso errorDesNode.InnerText <> "" AndAlso errorDesNode.InnerText.Contains("TOKEN SCADUTO") Then
                Throw New ExpiredTokenBDNException(errorDesNode.InnerText)
            End If
        End If

    End Function

    Protected Function generateDSfromXml(xml As XmlNode) As DataSet
        Dim ds As New DataSet
        Dim nodoDati = (From a As XmlNode In xml.ChildNodes Where a.Name = "dati").FirstOrDefault

        If nodoDati Is Nothing Then
            Dim xml_doc1 As New Xml.XmlDocument
            xml_doc1.LoadXml(xml.OuterXml)
            Dim child_nodes1 As XmlNodeList = xml_doc1.GetElementsByTagName("dati")
            If child_nodes1.Count > 0 Then
                nodoDati = child_nodes1(0)
            End If
        End If

        If nodoDati IsNot Nothing AndAlso
            nodoDati.HasChildNodes AndAlso
            nodoDati.ChildNodes.Count > 0 Then

            For Each ch As XmlNode In nodoDati.ChildNodes
                Dim dt As New DataTable

                If ch.HasChildNodes AndAlso ch.ChildNodes.Count > 0 Then
                    Dim righe = (From ri As XmlNode In ch.ChildNodes()).ToList
                    Dim i = 0
                    For Each riga As XmlNode In righe
                        If riga.OuterXml Is Nothing Then
                            Continue For
                        End If
                        If riga.OuterXml.Trim = "" Then
                            Continue For
                        End If
                        If i = 0 Then
                            If riga.HasChildNodes Then
                                For Each col As XmlNode In riga.ChildNodes
                                    If col.Name <> "#whitespace" Then
                                        dt.Columns.Add(New DataColumn(col.Name, GetType(String)))
                                    End If
                                Next
                            End If
                        End If
                        Dim dr = dt.NewRow

                        For Each col As XmlNode In riga.ChildNodes
                            If col.Name <> "#whitespace" Then
                                If Not dt.Columns.Contains(col.Name) Then
                                    dt.Columns.Add(New DataColumn(col.Name, GetType(String)))
                                End If
                                dr(col.Name) = col.InnerText
                            End If
                        Next

                        dt.Rows.Add(dr)
                        i = i + 1
                    Next

                    dt.TableName = righe(0).Name
                    ds.Tables.Add(dt)
                End If

            Next

            Return ds
        End If
        Dim nodoErroreInfo = (From a As XmlNode In xml.ChildNodes Where a.Name = "error_info").FirstOrDefault
        If nodoErroreInfo IsNot Nothing AndAlso
            nodoErroreInfo.HasChildNodes AndAlso
            nodoErroreInfo.ChildNodes.Count > 0 AndAlso
            nodoErroreInfo.ChildNodes(0).HasChildNodes Then
            Dim nodoErrore = (From a As XmlNode In nodoErroreInfo.ChildNodes Where a.Name = "error").FirstOrDefault

            Dim erroreDes = (From a As XmlNode In nodoErrore.ChildNodes Where a.Name = "des").FirstOrDefault()
            If erroreDes IsNot Nothing Then
                If erroreDes.InnerText <> "" Then
                    Throw New BDNException(erroreDes.InnerText)
                End If
            End If
        End If

        Dim xml_doc As New Xml.XmlDocument
        xml_doc.LoadXml(xml.OuterXml)
        Dim child_nodes As XmlNodeList = xml_doc.GetElementsByTagName("error")
        If child_nodes IsNot Nothing AndAlso child_nodes.Count > 0 Then
            Dim errorDesNode = (From a As XmlNode In child_nodes(0).ChildNodes Where a.Name = "des").FirstOrDefault
            If errorDesNode IsNot Nothing AndAlso errorDesNode.InnerText <> "" Then
                Throw New BDNException(errorDesNode.InnerText)
            End If
        End If

    End Function

    Protected Function getStrDataFromDate(data As Date) As String
        Dim strData = ""
        If data = AGRODATAINIZIO Then
            Return strData
        End If
        If data = AGRODATAFINE Then
            Return strData
        End If
        strData = data.Day.ToString("D2") & "/" & data.Month.ToString("D2") & "/" & data.Year
        Return strData
    End Function

    Private Function getSoapBody(soapAction As String, params As Dictionary(Of String, String)) As String
        Dim openTag = "<" & soapAction & " xmlns=""http://bdr.izs.it/webservices"">"
        Dim closeTag = "</" & soapAction & ">"
        Dim strParams As String = ""
        If params IsNot Nothing Then
            For Each param In params
                Dim strParam = "<" & param.Key & ">"
                strParam &= param.Value
                strParam &= "</" & param.Key & ">"
                strParams &= strParam
            Next
        End If
        Return openTag & strParams & closeTag
    End Function

    Private Sub getSoapBodyV2(ByVal soapAction As String,
                              ByVal params As Dictionary(Of String, String))


    End Sub

    Protected Function SoapRequest(soapAction As String, params As Dictionary(Of String, String), Optional ByRef cacheable As Boolean = False) As XmlNode
        Dim customHeader As New WebHeaderCollection
        Dim SOAPBodyContent = getSoapBody(soapAction, params)
        'customHeader.Add("SOAPAction", "http://bdr.izs.it/webservices/getPrenotazioneModello")
        'customHeader.Add("Accept-Encoding", "gzip, deflate")

        Dim SOAPAutenticazione As String = ""
        If Me.token <> "" AndAlso Me.ruolo <> "" AndAlso Me.valore_ruolo_codice <> "" Then
            SOAPAutenticazione = "<SOAPAutenticazione xmlns=""http://bdr.izs.it/webservices"">
                                                          <ruolo_codice>" & Me.ruolo & "</ruolo_codice>
                                                          <valore_ruolo_codice>" & Me.valore_ruolo_codice & "</valore_ruolo_codice>
                                                          <token>" & Me.token & "</token>
                                  </SOAPAutenticazione>"
        Else
            SOAPAutenticazione = "<SOAPAutenticazione xmlns=""http://bdr.izs.it/webservices"">
                                                          <username>" & Me.username & "</username>
                                                          <password>" & Me.password & "</password>
                                                        </SOAPAutenticazione>"
        End If

        Dim SOAPHeader As String = "<soap12:Header>" & SOAPAutenticazione & "</soap12:Header>"

        Dim SOAPBoby As String = "<soap12:Body>" & SOAPBodyContent & "</soap12:Body>"

        Dim cacheKey = Me.serviceEnpoint & "_" & SOAPBoby

        Dim soapRequest_str As String = "<?xml version=""1.0"" encoding=""utf-8""?>
                                                    <soap12:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap12=""http://www.w3.org/2003/05/soap-envelope"">
                                                      " & SOAPHeader & "
                                                      " & SOAPBoby & "
                                                    </soap12:Envelope>"
        Dim webHelper As New AgronicaCoreWebService.Http

        If log Then
            objLog.Scrivi_LOG(objParametri_Server,
                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Chiamata:" & vbCrLf & "URL:" & Me.serviceEnpoint & vbCrLf & soapRequest_str,
                          CustomLOGParams:=customLOGParams)
        End If


        Dim seBK = ServicePointManager.SecurityProtocol
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
        Dim responseStr1 = ""
        Try
            If cacheable AndAlso cache.ContainsKey(cacheKey) Then
                responseStr1 = cache(cacheKey)
                Exit Try
            End If

            responseStr1 = webHelper.chiamaWS(soapRequest_str,
                                              "",
                                              Me.serviceEnpoint,
                                              "text/xml; charset=utf-8",
                                              "POST",
                                              "",
                                              "",
                                              customHeader)

            If responseStr1.Contains("TOKEN SCADUTO") Then

                Thread.Sleep(200)
                If log Then
                    objLog.Scrivi_LOG(objParametri_Server,
                                      System.Reflection.MethodBase.GetCurrentMethod().Name,
                                      "TOKEN SCADUTO, RIPROVO",
                                      CustomLOGParams:=customLOGParams)
                End If

                responseStr1 = webHelper.chiamaWS(soapRequest_str,
                                              "",
                                              Me.serviceEnpoint,
                                              "text/xml; charset=utf-8",
                                              "POST",
                                              "",
                                              "",
                                              customHeader)
            End If

            If cacheable Then
                cache(cacheKey) = responseStr1
            End If

        Catch ex As Exception

            objLog.Scrivi_LOG(objParametri_Server,
                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Errore chiamata:" & vbCrLf & Me.serviceEnpoint & " " & soapAction & ":" & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True),
                          CustomLOGParams:=customLOGParams)
        End Try


        ServicePointManager.SecurityProtocol = seBK
        If log Then
            objLog.Scrivi_LOG(objParametri_Server,
                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Risposta:" & vbCrLf & responseStr1,
                          CustomLOGParams:=customLOGParams)
        End If

        Dim xmlNode = XmlStringToXmlNode(responseStr1)

        Return xmlNode

    End Function

    ''' <summary>
    ''' Diversa versione di SoapRequest, 
    ''' richiede una stringa xml ricavata da l'Escape dell'oggetto xml 
    ''' </summary>
    ''' <param name="xmlStr"></param>
    ''' <param name="SoapAction"></param>
    ''' <returns></returns>
    Protected Function SoapRequestV2(ByVal xmlStr As String,
                                     ByVal SoapAction As String) As XmlNode

        Dim customHeader As New WebHeaderCollection
        'Dim SOAPBodyContent = getSoapBoby(SoapAction, params)
        customHeader.Add("SOAPAction", SoapAction)
        customHeader.Add("Accept-Encoding", "gzip, deflate")

        Dim SOAPAutenticazione As String = ""
        If Me.token <> "" AndAlso Me.ruolo <> "" AndAlso Me.valore_ruolo_codice <> "" Then
            SOAPAutenticazione = "<SOAPAutenticazione xmlns=""http://bdr.izs.it/webservices"">
                                      <ruolo_codice>" & Me.ruolo & "</ruolo_codice>
                                      <valore_ruolo_codice>" & Me.valore_ruolo_codice & "</valore_ruolo_codice>
                                      <token>" & Me.token & "</token>
                                  </SOAPAutenticazione>"
        Else
            SOAPAutenticazione = "<SOAPAutenticazione xmlns=""http://bdr.izs.it/webservices"">
                                      <username>" & Me.username & "</username>
                                      <password>" & Me.password & "</password>
                                  </SOAPAutenticazione>"
        End If

        Dim SOAPHeader As String = "<soap12:Header>
                                        " & SOAPAutenticazione & "
                                    </soap12:Header>"

        Dim SoapBody As String = "<soap12:Body><" & SoapAction & " xmlns=""http://bdr.izs.it/webservices"">
                                     <strRecord>
                                         " & xmlStr & "
                                     </strRecord>
                                  </" & SoapAction & "></soap12:Body>"

        Dim soapRequest_str As String = "<?xml version=""1.0"" encoding=""utf-8""?>
                                         <soap12:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap12=""http://www.w3.org/2003/05/soap-envelope"">
                                            " & SOAPHeader & "
                                            " & SoapBody & "
                                         </soap12:Envelope>"
        If log Then
            objLog.Scrivi_LOG(objParametri_Server,
                              System.Reflection.MethodBase.GetCurrentMethod().Name,
                              "Link:" & vbCrLf & Me.serviceEnpoint,
                              CustomLOGParams:=customLOGParams)

            objLog.Scrivi_LOG(objParametri_Server,
                              System.Reflection.MethodBase.GetCurrentMethod().Name,
                              "Chiamata:" & vbCrLf & soapRequest_str,
                              CustomLOGParams:=customLOGParams)
        End If


        Dim seBK = ServicePointManager.SecurityProtocol
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

        Dim webHelper As New AgronicaCoreWebService.Http
        Dim responseStr1 As String = ""

        Try
            responseStr1 = webHelper.chiamaWS(soapRequest_str,
                                                        "", Me.serviceEnpoint,
                                                        "application/soap+xml; charset=utf-8",
                                                        "POST", "", "",
                                                        customHeader)

            If responseStr1.Contains("TOKEN SCADUTO") Then
                responseStr1 = webHelper.chiamaWS(soapRequest_str,
                                                        "", Me.serviceEnpoint,
                                                        "application/soap+xml; charset=utf-8",
                                                        "POST", "", "",
                                                        customHeader)
            End If

        Catch ex As Exception
            objLog.Scrivi_LOG(objParametri_Server,
                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Errore chiamata:" & vbCrLf & Me.serviceEnpoint & " " & SoapAction & ":" & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True),
                          CustomLOGParams:=customLOGParams)
        End Try

        ServicePointManager.SecurityProtocol = seBK

        If log Then
            objLog.Scrivi_LOG(objParametri_Server,
                              System.Reflection.MethodBase.GetCurrentMethod().Name,
                              "Risposta:" & vbCrLf & responseStr1,
                              CustomLOGParams:=customLOGParams)
        End If


        Dim xmlNode As XmlNode = XmlStringToXmlNode(responseStr1)

        Return xmlNode

    End Function

    Protected Function Utf16Encode(ByVal unicode_code_point)
        If (unicode_code_point >= 0 And unicode_code_point <= &HD7FF&) Or (unicode_code_point >= &HE000& And unicode_code_point <= &HFFFF&) Then
            Return ChrW(unicode_code_point)
        Else
            unicode_code_point = unicode_code_point - &H10000&
            Return ChrW(&HD800 Or (unicode_code_point \ &H400&)) & ChrW(&HDC00 Or (unicode_code_point And &H3FF&))
        End If
    End Function

    Public Shared Function XmlStringToXmlNode(xmlInputString As String) As XmlNode
        If String.IsNullOrEmpty(xmlInputString.Trim()) Then
            Throw New ArgumentNullException("xmlInputString")
        End If
        Dim xd = New XmlDocument()
        Using sr = New StringReader(xmlInputString)
            xd.Load(sr)
        End Using
        Return xd
    End Function

    Protected Function AcceptAllCertifications(ByVal sender As Object, ByVal certification As System.Security.Cryptography.X509Certificates.X509Certificate, ByVal chain As System.Security.Cryptography.X509Certificates.X509Chain, ByVal sslPolicyErrors As System.Net.Security.SslPolicyErrors) As Boolean

        Return True

    End Function

End Class


Public Class BDNException
    Inherits Exception

    Public Sub New()
        MyBase.New()
    End Sub
    Public Sub New(ByVal message As String)
        MyBase.New(message)
    End Sub
    Public Sub New(ByVal message As String, ByVal e As Exception)
        MyBase.New(message, e)
    End Sub

End Class

Public Class ExpiredTokenBDNException
    Inherits Exception

    Public Sub New()
        MyBase.New()
    End Sub
    Public Sub New(ByVal message As String)
        MyBase.New(message)
    End Sub
    Public Sub New(ByVal message As String, ByVal e As Exception)
        MyBase.New(message, e)
    End Sub

End Class

Public Class TokenBDNException
    Inherits Exception

    Public Sub New()
        MyBase.New()
    End Sub
    Public Sub New(ByVal message As String)
        MyBase.New(message)
    End Sub
    Public Sub New(ByVal message As String, ByVal e As Exception)
        MyBase.New(message, e)
    End Sub

End Class