Imports System.IO
Imports System.Net
Imports System.ServiceModel
Imports System.ServiceModel.Channels
Imports System.ServiceModel.Description
Imports System.ServiceModel.Dispatcher
Imports System.ServiceModel.Security.Tokens
Imports System.Text
Imports System.Xml
Imports System.Xml.Serialization


Public Class Importa_AGREA_WS

    Dim username As String
    Dim password As String
    Dim link As String

    Public Sub New(user As String, pwd As String, l As String)
        username = user
        password = pwd
        link = l
    End Sub

    Public Function importa_Old(cuaa As String,
                            anno As Integer,
                            ByRef fascicolo As pc.common.webservice.sop.agrea.it.ISWSResponse,
                            ByRef ErrCod As Integer,
                            ByRef ErrMsg As String) As String


        If link = "" Then
            'If Debugger.IsAttached Then
            'link = "http://agreatest.regione.emilia-romagna.it/FornituraServiziWS/services/GetPianoColturale"
            'Else
            link = "http://agreagestione.regione.emilia-romagna.it/FornituraServiziWS/services/GetPianoColturale"
            'End If
        End If


        'Dim myBinding As New WSHttpBinding()

        'myBinding.TextEncoding = ASCIIEncoding.UTF8
        'myBinding.MessageEncoding = WSMessageEncoding.Text
        'myBinding.Security.Mode = SecurityMode.Message
        'myBinding.Security.Message.ClientCredentialType = MessageCredentialType.UserName
        'myBinding.Security.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.Basic128

        'myBinding.Security.Message.NegotiateServiceCredential = False
        'myBinding.Security.Message.EstablishSecurityContext = False

        'Dim uri As New Uri(link)
        'Dim ea As New EndpointAddress(link)

        ''Dim bind = GetCustomBinding()

        'Dim abe As AsymmetricSecurityBindingElement = SecurityBindingElement.CreateMutualCertificateBindingElement(MessageSecurityVersion.WSSecurity10WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10)

        ''Dim wsAgrea As New GetPianoColturaleClient(myBinding, ea)
        'Dim wsAgrea As New GetPianoColturaleClient("AGREAWS")

        ''wsAgrea.Endpoint.Behaviors.Add(New CustomEndpointBehavior())

        'Dim credential = wsAgrea.ClientCredentials.UserName
        'credential.UserName = username
        'credential.Password = password

        'Try
        '    Dim response = wsAgrea.getPianoColturaleFull(cuaa, anno)
        'Catch ex As Exception

        'End Try

        'Try

        '    Dim a As New getPianoColturaleFullResponse

        '    a.Body = New getPianoColturaleFullResponseBody

        '    a.Body.getPianoColturaleFullReturn = New pc.common.webservice.sop.agrea.it.ISWSResponse

        '    Dim array_possessi As New pc.webservice.sop.agrea.it.ArrayOf_tns1_ISWSPossesso

        '    Dim possesso1 As New pc.common.webservice.sop.agrea.it.ISWSPossesso
        '    Dim possesso2 As New pc.common.webservice.sop.agrea.it.ISWSPossesso

        '    array_possessi.Add(possesso1)
        '    array_possessi.Add(possesso2)

        '    a.Body.getPianoColturaleFullReturn.possessi = array_possessi

        '    Dim serializer = New XmlSerializer(a.GetType)

        '    Dim writer = New StreamWriter("C:\\File.xml")
        '    serializer.Serialize(writer, a)


        'Catch ex As Exception

        'End Try

        Try

            Dim customHeader As New WebHeaderCollection

            customHeader.Add("SOAPAction", "")
            'customHeader.Add("Expect", "100-continue")
            customHeader.Add("Accept-Encoding", "gzip, deflate")
            'customHeader.Add("Connection", "Keep-Alive")
            customHeader.Add("VsDebuggerCausalityData", "uIDPowBeVBPO2wtIuuyh7axYX0YAAAAATdQBv2eVUUCNVHT1C33I35X6NQxwuSJBnlC6/dKzd1QACQAA")

            'ws_agronica
            '887cfd38785eb232a6700b29d6611290f6598658

            Dim webHelper As New AgronicaCoreWebService.Http
            Dim responseStr = webHelper.chiamaWS("<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">" &
                                                 "   <s:Header>" &
                                                        "<wsse:Security xmlns:wsse=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"">" &
                                                            "<wsse:UsernameToken>" &
                                                                "<wsse:Username>" & username & "</wsse:Username>" &
                                                                "<wsse:Password>" & password & "</wsse:Password>" &
                                                            "</wsse:UsernameToken>" &
                                                        "</wsse:Security>" &
                                                    "</s:Header>" &
                                                    "<s:Body> " &
                                                        "<getPianoColturaleFull xmlns=""http://pc.webService.sop.agrea.it"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"">" &
                                                        "<cuaa>" & cuaa & "</cuaa>" &
                                                        "<annoRiferimento>" & anno & "</annoRiferimento>" &
                                                        "</getPianoColturaleFull>" &
                                                    "</s:Body>" &
                                                 "</s:Envelope>",
                                                 "",
                                                 link,
                                                 "text/xml; charset=utf-8",
                                                 "POST",
                                                 "",
                                                 "",
                                                 customHeader)


            'Dim xDoc As New XDocument
            Dim getPianoColturaleFull As New getPianoColturaleFullResponse
            Dim soapenv As XNamespace = "http://schemas.xmlsoap.org/soap/envelope/"
            Dim xDoc As XDocument = XDocument.Parse(responseStr)
            Dim xmlns As XNamespace = "http://pc.webService.sop.agrea.it"

            Dim x_pianoColturaleFullReturn = (From xx In xDoc.Descendants(xmlns + "getPianoColturaleFullReturn") Select xx).FirstOrDefault

            getPianoColturaleFull.Body = New getPianoColturaleFullResponseBody
            fascicolo = New pc.common.webservice.sop.agrea.it.ISWSResponse
            getPianoColturaleFull.Body.getPianoColturaleFullReturn = fascicolo

            fascicolo.codRet = x_pianoColturaleFullReturn.Element(xmlns + "codRet").Value
            fascicolo.msgRet = x_pianoColturaleFullReturn.Element(xmlns + "msgRet").Value

            fascicolo.azienda = imposta_Azienda(xDoc)
            fascicolo.domanda = imposta_Domanda(xDoc)
            fascicolo.duDichCondizionalita = imposta_DichiarazioniCondizionalita(xDoc)
            fascicolo.persona = imposta_Persona(xDoc)
            fascicolo.possessi = imposta_Possessi(xDoc)
            fascicolo.vincoliCondizionalita = imposta_VincoliCondizionalita(xDoc)

            ErrCod = 0
            ErrMsg = ""

        Catch ex As Exception

            fascicolo = Nothing
            ErrCod = -101
            ErrMsg = "Errore durante il recupero delle informazioni : " & ex.Message

        End Try

        Return ""

    End Function


    Public Function importa(cuaa As String,
                            anno As Integer,
                            ByRef fascicolo As pc.common.webservice.sop.agrea.it.ISWSResponse,
                            ByRef ErrCod As Integer,
                            ByRef ErrMsg As String) As String


        If link = "" Then
            'link = "http://agreatest.regione.emilia-romagna.it/FornituraServiziWS/services/GetPianoColturale"
            'username = "ws_agronica"
            'password = "bd53edb209412f355ac6044d88b89d6cba3e980d"
            link = "http://agreagestione.regione.emilia-romagna.it/FornituraServiziWS/services/GetPianoColturale"
        End If

        Try

            Dim customHeader As New WebHeaderCollection

            customHeader.Add("SOAPAction", "")
            'customHeader.Add("Expect", "100-continue")
            customHeader.Add("Accept-Encoding", "gzip, deflate")
            'customHeader.Add("Connection", "Keep-Alive")
            customHeader.Add("VsDebuggerCausalityData", "uIDPowBeVBPO2wtIuuyh7axYX0YAAAAATdQBv2eVUUCNVHT1C33I35X6NQxwuSJBnlC6/dKzd1QACQAA")

            Dim restSharpCaller As New AgronicaCoreUtility.Http
            Dim response = restSharpCaller.chiamaWS_RestShapr_XML("<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">" &
                                                 "   <s:Header>" &
                                                        "<wsse:Security xmlns:wsse=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"">" &
                                                            "<wsse:UsernameToken>" &
                                                                "<wsse:Username>" & username & "</wsse:Username>" &
                                                                "<wsse:Password>" & password & "</wsse:Password>" &
                                                            "</wsse:UsernameToken>" &
                                                        "</wsse:Security>" &
                                                    "</s:Header>" &
                                                    "<s:Body> " &
                                                        "<getPianoColturaleFull xmlns=""http://pc.webService.sop.agrea.it"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"">" &
                                                        "<cuaa>" & cuaa & "</cuaa>" &
                                                        "<annoRiferimento>" & anno & "</annoRiferimento>" &
                                                        "</getPianoColturaleFull>" &
                                                    "</s:Body>" &
                                                 "</s:Envelope>",
                                                   "", link, "text/xml", RestSharp.Method.POST, "", "", customHeader)

            'Dim response = restSharpCaller.chiamaWS_RestShapr_XML("<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">" &
            '                                     "   <s:Header>" &
            '                                            "<wsse:Security xmlns:wsse=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"">" &
            '                                                "<wsse:UsernameToken>" &
            '                                                    "<wsse:Username>" + username + "</wsse:Username>" &
            '                                                    "<wsse:Password>" + password + "</wsse:Password>" &
            '                                                "</wsse:UsernameToken>" &
            '                                            "</wsse:Security>" &
            '                                        "</s:Header>" &
            '                                        "<s:Body> " &
            '                                            "<getPianoColturaleFullGrafico xmlns=""http://pc.webService.sop.agrea.it"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"">" &
            '                                            "<cuaa>" & cuaa & "</cuaa>" &
            '                                            "<annoRiferimento>" & anno & "</annoRiferimento>" &
            '                                            "</getPianoColturaleFullGrafico>" &
            '                                        "</s:Body>" &
            '                                     "</s:Envelope>",
            '                                       "", link, "text/xml", RestSharp.Method.POST, "", "", customHeader)

            Select Case response.StatusCode
                Case HttpStatusCode.OK
                    'Dim getPianoColturaleFull As New getPianoColturaleFullResponse
                    'Dim soapenv As XNamespace = "http://schemas.xmlsoap.org/soap/envelope/"
                    'Dim xDoc As XDocument = XDocument.Parse(response.Content)
                    'Dim xmlns As XNamespace = "http://pc.webService.sop.agrea.it"

                    'Dim x_pianoColturaleFullReturn = (From xx In xDoc.Descendants(xmlns + "getPianoColturaleFullReturn") Select xx).FirstOrDefault

                    'getPianoColturaleFull.Body = New getPianoColturaleFullResponseBody
                    'fascicolo = New pc.common.webservice.sop.agrea.it.ISWSResponse
                    'getPianoColturaleFull.Body.getPianoColturaleFullReturn = fascicolo

                    'fascicolo.codRet = x_pianoColturaleFullReturn.Element(xmlns + "codRet").Value
                    'fascicolo.msgRet = x_pianoColturaleFullReturn.Element(xmlns + "msgRet").Value

                    'fascicolo.azienda = imposta_Azienda(xDoc)
                    'fascicolo.domanda = imposta_Domanda(xDoc)
                    'fascicolo.duDichCondizionalita = imposta_DichiarazioniCondizionalita(xDoc)
                    'fascicolo.persona = imposta_Persona(xDoc)
                    'fascicolo.possessi = imposta_Possessi(xDoc)
                    'fascicolo.vincoliCondizionalita = imposta_VincoliCondizionalita(xDoc)

                    fascicolo = FascicoloDaXml(response.Content)

                    ErrCod = 0
                    ErrMsg = ""

                    If fascicolo.codRet = "014" Then
                        ErrMsg = fascicolo.msgRet
                        ErrCod = -103
                    End If


                Case HttpStatusCode.InternalServerError
                    'CUAA inesistente
                    If response.Content.Contains("CUAA inesistente") Then
                        ErrCod = -102
                        ErrMsg = "CUAA inesistente"
                    Else
                        ErrCod = -101
                        ErrMsg = response.Content
                    End If
            End Select

            'Dim xDoc As New XDocument


        Catch ex As Exception

            fascicolo = Nothing
            ErrCod = -101
            ErrMsg = "Errore durante il recupero delle informazioni : " & ex.Message

        End Try

        Return ""

    End Function

    Public Function importaGrafico(cuaa As String,
                            anno As Integer,
                            ByRef fascicolo As pc.common.webservice.sop.agrea.it.ISWSResponseGrafico,
                            ByRef ErrCod As Integer,
                            ByRef ErrMsg As String) As String


        If link = "" Then
            link = "http://agreatest.regione.emilia-romagna.it/FornituraServiziWS/services/GetPianoColturale"
            'username = "ws_agronica"
            'password = "bd53edb209412f355ac6044d88b89d6cba3e980d"
            'link = "http://agreagestione.regione.emilia-romagna.it/FornituraServiziWS/services/GetPianoColturale"
        End If

        Try

            Dim customHeader As New WebHeaderCollection

            customHeader.Add("SOAPAction", "")
            'customHeader.Add("Expect", "100-continue")
            customHeader.Add("Accept-Encoding", "gzip, deflate")
            'customHeader.Add("Connection", "Keep-Alive")
            customHeader.Add("VsDebuggerCausalityData", "uIDPowBeVBPO2wtIuuyh7axYX0YAAAAATdQBv2eVUUCNVHT1C33I35X6NQxwuSJBnlC6/dKzd1QACQAA")

            Dim restSharpCaller As New AgronicaCoreUtility.Http
            'Dim response = restSharpCaller.chiamaWS_RestShapr_XML("<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">" &
            '                                     "   <s:Header>" &
            '                                            "<wsse:Security xmlns:wsse=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"">" &
            '                                                "<wsse:UsernameToken>" &
            '                                                    "<wsse:Username>" & username & "</wsse:Username>" &
            '                                                    "<wsse:Password>" & password & "</wsse:Password>" &
            '                                                "</wsse:UsernameToken>" &
            '                                            "</wsse:Security>" &
            '                                        "</s:Header>" &
            '                                        "<s:Body> " &
            '                                            "<getPianoColturaleFull xmlns=""http://pc.webService.sop.agrea.it"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"">" &
            '                                            "<cuaa>" & cuaa & "</cuaa>" &
            '                                            "<annoRiferimento>" & anno & "</annoRiferimento>" &
            '                                            "</getPianoColturaleFull>" &
            '                                        "</s:Body>" &
            '                                     "</s:Envelope>",
            '                                       "", link, "text/xml", RestSharp.Method.POST, "", "", customHeader)

            Dim response = restSharpCaller.chiamaWS_RestShapr_XML("<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">" &
                                                 "   <s:Header>" &
                                                        "<wsse:Security xmlns:wsse=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"">" &
                                                            "<wsse:UsernameToken>" &
                                                                "<wsse:Username>" & username & "</wsse:Username>" &
                                                                "<wsse:Password>" & password & "</wsse:Password>" &
                                                            "</wsse:UsernameToken>" &
                                                        "</wsse:Security>" &
                                                    "</s:Header>" &
                                                    "<s:Body> " &
                                                        "<getPianoColturaleFullGrafico xmlns=""http://pc.webService.sop.agrea.it"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"">" &
                                                        "<cuaa>" & cuaa & "</cuaa>" &
                                                        "<annoRiferimento>" & anno & "</annoRiferimento>" &
                                                        "</getPianoColturaleFullGrafico>" &
                                                    "</s:Body>" &
                                                 "</s:Envelope>",
                                                   "", link, "text/xml", RestSharp.Method.POST, "", "", customHeader)

            Select Case response.StatusCode
                Case HttpStatusCode.OK
                    'Dim getPianoColturaleFull As New getPianoColturaleFullResponse
                    'Dim soapenv As XNamespace = "http://schemas.xmlsoap.org/soap/envelope/"
                    'Dim xDoc As XDocument = XDocument.Parse(response.Content)
                    'Dim xmlns As XNamespace = "http://pc.webService.sop.agrea.it"

                    'Dim x_pianoColturaleFullReturn = (From xx In xDoc.Descendants(xmlns + "getPianoColturaleFullReturn") Select xx).FirstOrDefault

                    'getPianoColturaleFull.Body = New getPianoColturaleFullResponseBody
                    'fascicolo = New pc.common.webservice.sop.agrea.it.ISWSResponse
                    'getPianoColturaleFull.Body.getPianoColturaleFullReturn = fascicolo

                    'fascicolo.codRet = x_pianoColturaleFullReturn.Element(xmlns + "codRet").Value
                    'fascicolo.msgRet = x_pianoColturaleFullReturn.Element(xmlns + "msgRet").Value

                    'fascicolo.azienda = imposta_Azienda(xDoc)
                    'fascicolo.domanda = imposta_Domanda(xDoc)
                    'fascicolo.duDichCondizionalita = imposta_DichiarazioniCondizionalita(xDoc)
                    'fascicolo.persona = imposta_Persona(xDoc)
                    'fascicolo.possessi = imposta_Possessi(xDoc)
                    'fascicolo.vincoliCondizionalita = imposta_VincoliCondizionalita(xDoc)

                    fascicolo = FascicoloDaXmlGrafico(response.Content)

                    ErrCod = 0
                    ErrMsg = ""

                    If fascicolo.codRet = "014" Then
                        ErrMsg = fascicolo.msgRet
                        ErrCod = -103
                    End If


                Case HttpStatusCode.InternalServerError
                    'CUAA inesistente
                    If response.Content.Contains("CUAA inesistente") Then
                        ErrCod = -102
                        ErrMsg = "CUAA inesistente"
                    Else
                        ErrCod = -101
                        ErrMsg = response.Content
                    End If
            End Select

            'Dim xDoc As New XDocument


        Catch ex As Exception

            fascicolo = Nothing
            ErrCod = -101
            ErrMsg = "Errore durante il recupero delle informazioni : " & ex.Message

        End Try

        Return ""

    End Function

    Public Function CuaaModificati(ByVal Data As Date,
                                    ByVal anno As Integer,
                                    ByRef fascicolo As pc.common.webservice.sop.agrea.it.ISWSResponseCUAAVariati,
                                    ByRef ErrCod As Integer,
                                    ByRef ErrMsg As String) As String


        If link = "" Then
            link = "http://agreatest.regione.emilia-romagna.it/FornituraServiziWS/services/GetPianoColturale"
            'username = "ws_agronica"
            'password = "bd53edb209412f355ac6044d88b89d6cba3e980d"
            'link = "http://agreagestione.regione.emilia-romagna.it/FornituraServiziWS/services/GetPianoColturale"
        End If

        Try

            Dim customHeader As New WebHeaderCollection

            customHeader.Add("SOAPAction", "")
            'customHeader.Add("Expect", "100-continue")
            customHeader.Add("Accept-Encoding", "gzip, deflate")
            'customHeader.Add("Connection", "Keep-Alive")
            customHeader.Add("VsDebuggerCausalityData", "uIDPowBeVBPO2wtIuuyh7axYX0YAAAAATdQBv2eVUUCNVHT1C33I35X6NQxwuSJBnlC6/dKzd1QACQAA")

            Dim restSharpCaller As New AgronicaCoreUtility.Http

            Dim response = restSharpCaller.chiamaWS_RestShapr_XML("<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">" &
                                                 "   <s:Header>" &
                                                        "<wsse:Security xmlns:wsse=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"">" &
                                                            "<wsse:UsernameToken>" &
                                                                "<wsse:Username>" & username & "</wsse:Username>" &
                                                                "<wsse:Password>" & password & "</wsse:Password>" &
                                                            "</wsse:UsernameToken>" &
                                                        "</wsse:Security>" &
                                                    "</s:Header>" &
                                                    "<s:Body> " &
                                                        "<getCUAAVariati xmlns=""http://pc.webService.sop.agrea.it"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"">" &
                                                        "<annoRiferimento>" & anno & "</annoRiferimento>" &
                                                        "<dataRiferimento>" & CStr(Data.Year) & "-" & CStr(Data.Month) & "-" & CStr(Data.Day) & "</dataRiferimento>" &
                                                        "</getCUAAVariati>" &
                                                    "</s:Body>" &
                                                 "</s:Envelope>",
                                                   "", link, "text/xml", RestSharp.Method.POST, "", "", customHeader)

            Select Case response.StatusCode
                Case HttpStatusCode.OK

                    fascicolo = CUAAVariatiDaXml(response.Content)

                    ErrCod = 0
                    ErrMsg = ""

                    If fascicolo.codRet = "014" Then
                        ErrMsg = fascicolo.msgRet
                        ErrCod = -103
                    End If


                Case HttpStatusCode.InternalServerError
                    'CUAA inesistente
                    If response.Content.Contains("CUAA inesistente") Then
                        ErrCod = -102
                        ErrMsg = "CUAA inesistente"
                    Else
                        ErrCod = -101
                        ErrMsg = response.Content
                    End If
            End Select

            'Dim xDoc As New XDocument


        Catch ex As Exception

            fascicolo = Nothing
            ErrCod = -101
            ErrMsg = "Errore durante il recupero delle informazioni : " & ex.Message

        End Try

        Return ""

    End Function

    Public Function FascicoloDaXml(xml As String) As pc.common.webservice.sop.agrea.it.ISWSResponse
        Dim getPianoColturaleFull As New getPianoColturaleFullResponse
        Dim soapenv As XNamespace = "http://schemas.xmlsoap.org/soap/envelope/"
        Dim xDoc As XDocument = XDocument.Parse(xml)
        Dim xmlns As XNamespace = "http://pc.webService.sop.agrea.it"

        Dim x_pianoColturaleFullReturn = (From xx In xDoc.Descendants(xmlns + "getPianoColturaleFullReturn") Select xx).FirstOrDefault

        getPianoColturaleFull.Body = New getPianoColturaleFullResponseBody
        Dim fascicolo = New pc.common.webservice.sop.agrea.it.ISWSResponse
        getPianoColturaleFull.Body.getPianoColturaleFullReturn = fascicolo

        fascicolo.codRet = x_pianoColturaleFullReturn.Element(xmlns + "codRet").Value
        fascicolo.msgRet = x_pianoColturaleFullReturn.Element(xmlns + "msgRet").Value

        fascicolo.azienda = imposta_Azienda(xDoc)
        fascicolo.domanda = imposta_Domanda(xDoc)
        fascicolo.duDichCondizionalita = imposta_DichiarazioniCondizionalita(xDoc)
        fascicolo.persona = imposta_Persona(xDoc)
        fascicolo.possessi = imposta_Possessi(xDoc)
        fascicolo.vincoliCondizionalita = imposta_VincoliCondizionalita(xDoc)
        Return fascicolo
    End Function

    Public Function FascicoloDaXmlGrafico(xml As String) As pc.common.webservice.sop.agrea.it.ISWSResponseGrafico
        Dim getPianoColturaleFull As New getPianoColturaleFullGraficoResponse
        Dim soapenv As XNamespace = "http://schemas.xmlsoap.org/soap/envelope/"
        Dim xDoc As XDocument = XDocument.Parse(xml)
        Dim xmlns As XNamespace = "http://pc.webService.sop.agrea.it"

        Dim x_pianoColturaleFullReturn = (From xx In xDoc.Descendants(xmlns + "getPianoColturaleFullGraficoReturn") Select xx).FirstOrDefault

        getPianoColturaleFull.Body = New getPianoColturaleFullGraficoResponseBody
        Dim fascicolo = New pc.common.webservice.sop.agrea.it.ISWSResponseGrafico
        getPianoColturaleFull.Body.getPianoColturaleFullGraficoReturn = fascicolo

        fascicolo.codRet = x_pianoColturaleFullReturn.Element(xmlns + "codRet").Value
        fascicolo.msgRet = x_pianoColturaleFullReturn.Element(xmlns + "msgRet").Value

        fascicolo.azienda = imposta_Azienda(xDoc)
        fascicolo.domanda = imposta_Domanda(xDoc)
        fascicolo.duDichCondizionalita = imposta_DichiarazioniCondizionalita(xDoc)
        fascicolo.persona = imposta_Persona(xDoc)
        fascicolo.possessi = imposta_Possessi(xDoc)
        fascicolo.vincoliCondizionalita = imposta_VincoliCondizionalita(xDoc)
        fascicolo.appezzamenti = imposta_Appezzamenti(xDoc)
        Return fascicolo
    End Function

    Public Function CUAAVariatiDaXml(xml As String) As pc.common.webservice.sop.agrea.it.ISWSResponseCUAAVariati
        Dim getPianoColturaleFull As New getCUAAVariatiResponse
        Dim soapenv As XNamespace = "http://schemas.xmlsoap.org/soap/envelope/"
        Dim xDoc As XDocument = XDocument.Parse(xml)
        Dim xmlns As XNamespace = "http://pc.webService.sop.agrea.it"

        Dim x_pianoColturaleFullReturn = (From xx In xDoc.Descendants(xmlns + "getCUAAVariatiReturn") Select xx).FirstOrDefault

        getPianoColturaleFull.Body = New getCUAAVariatiResponseBody
        Dim fascicolo = New pc.common.webservice.sop.agrea.it.ISWSResponseCUAAVariati
        getPianoColturaleFull.Body.getCUAAVariatiReturn = fascicolo

        fascicolo.codRet = x_pianoColturaleFullReturn.Element(xmlns + "codRet").Value
        fascicolo.msgRet = x_pianoColturaleFullReturn.Element(xmlns + "msgRet").Value

        Dim arrayCuaa As New pc.webservice.sop.agrea.it.ArrayOf_xsd_string
        Dim x_domandaArr = (From xx In xDoc.Descendants(xmlns + "cuaa") Select xx).FirstOrDefault
        If x_domandaArr IsNot Nothing Then
            Dim x_domandaS = (From xx In xDoc.Descendants(xmlns + "cuaa") Select xx)
            If x_domandaS.Count > 1 Then
                Dim first = True
                For Each x_domanda In x_domandaS

                    If Not first Then
                        arrayCuaa.Add(x_domanda.Value)
                    End If

                    first = False

                Next

            End If
        End If
        fascicolo.cuaa = arrayCuaa

        Return fascicolo
    End Function

    Private Function imposta_Azienda(doc As XDocument) As pc.common.webservice.sop.agrea.it.ISWSAzienda
        Dim azienda = New pc.common.webservice.sop.agrea.it.ISWSAzienda
        Dim xmlns As XNamespace = "http://pc.webService.sop.agrea.it"
        Dim x_azienda = (From xx In doc.Descendants(xmlns + "azienda") Select xx).FirstOrDefault

        azienda.cap = getValoreStringa(x_azienda, xmlns, "cap")
        azienda.codFormaGiuridica = getValoreStringa(x_azienda, xmlns, "codFormaGiuridica")
        azienda.codIstatCom = getValoreStringa(x_azienda, xmlns, "codIstatCom")
        azienda.codIstatProv = getValoreStringa(x_azienda, xmlns, "codIstatProv")
        azienda.cuaa = getValoreStringa(x_azienda, xmlns, "cuaa")
        azienda.descComune = getValoreStringa(x_azienda, xmlns, "descComune")
        azienda.descFormaGiuridica = getValoreStringa(x_azienda, xmlns, "descFormaGiuridica")
        azienda.descProvincia = getValoreStringa(x_azienda, xmlns, "descProvincia")
        azienda.idAzienda = getValoreInteger(x_azienda, xmlns, "idAzienda")
        azienda.indirizzo = getValoreStringa(x_azienda, xmlns, "indirizzo")
        azienda.numRea = getValoreStringa(x_azienda, xmlns, "numRea")
        azienda.partitaIva = getValoreStringa(x_azienda, xmlns, "partitaIva")
        azienda.provRea = getValoreStringa(x_azienda, xmlns, "provRea")
        azienda.ragioneSociale = getValoreStringa(x_azienda, xmlns, "ragioneSociale")

        Return azienda
    End Function

    Private Function imposta_Domanda(doc As XDocument) As pc.common.webservice.sop.agrea.it.ISWSDomanda
        Dim domanda = New pc.common.webservice.sop.agrea.it.ISWSDomanda
        Dim xmlns As XNamespace = "http://pc.webService.sop.agrea.it"
        Dim x_domanda = (From xx In doc.Descendants(xmlns + "domanda") Select xx).FirstOrDefault

        domanda.annoRiferimento = getValoreInteger(x_domanda, xmlns, "annoRiferimento")
        domanda.codSettore = getValoreStringa(x_domanda, xmlns, "codSettore")
        domanda.dataValidazione = getValoreStringa(x_domanda, xmlns, "dataValidazione")
        domanda.descCaa = getValoreStringa(x_domanda, xmlns, "descCaa")
        domanda.idCaa = getValoreInteger(x_domanda, xmlns, "idCaa")
        domanda.idDomanda = getValoreInteger(x_domanda, xmlns, "idDomanda")
        domanda.numProtocollo = getValoreInteger(x_domanda, xmlns, "numProtocollo")
        domanda.progRettifica = getValoreInteger(x_domanda, xmlns, "progRettifica")

        Return domanda
    End Function

    Private Function imposta_DichiarazioniCondizionalita(doc As XDocument) As pc.webservice.sop.agrea.it.ArrayOf_tns1_ISWSDuDichCondizionalita
        Dim arrayCond As New pc.webservice.sop.agrea.it.ArrayOf_tns1_ISWSDuDichCondizionalita
        Dim xmlns As XNamespace = "http://pc.webService.sop.agrea.it"
        Dim x_domandaArr = (From xx In doc.Descendants(xmlns + "duDichCondizionalita") Select xx).FirstOrDefault
        If x_domandaArr IsNot Nothing Then

            Dim x_domandaS = (From xx In doc.Descendants(xmlns + "duDichCondizionalita") Select xx)
            If x_domandaS.Count > 1 Then
                Dim first = True
                For Each x_domanda In x_domandaS

                    If Not first Then
                        Dim condizionalita As New pc.common.webservice.sop.agrea.it.ISWSDuDichCondizionalita

                        condizionalita.annoCondizione = getValoreInteger(x_domanda, xmlns, "annoCondizione")
                        condizionalita.campoCondizione = getValoreInteger(x_domanda, xmlns, "campoCondizione")
                        condizionalita.codCondizione = getValoreInteger(x_domanda, xmlns, "codCondizione")
                        condizionalita.codFonteDato = getValoreStringa(x_domanda, xmlns, "codFonteDato")
                        condizionalita.descrizione = getValoreStringa(x_domanda, xmlns, "descrizione")
                        condizionalita.falgDich = getValoreStringa(x_domanda, xmlns, "falgDich")
                        condizionalita.scoFonteDato = getValoreStringa(x_domanda, xmlns, "scoFonteDato")

                        arrayCond.Add(condizionalita)
                    End If

                    first = False

                Next

                Return arrayCond
            End If

        End If

        Return Nothing
    End Function

    Private Function imposta_Persona(doc As XDocument) As pc.common.webservice.sop.agrea.it.ISWSPersona
        Dim persona = New pc.common.webservice.sop.agrea.it.ISWSPersona
        Dim xmlns As XNamespace = "http://pc.webService.sop.agrea.it"
        Dim x_persona = (From xx In doc.Descendants(xmlns + "persona") Select xx).FirstOrDefault

        persona.cap = getValoreStringa(x_persona, xmlns, "cap")
        persona.codComNasc = getValoreStringa(x_persona, xmlns, "codComNasc")
        persona.codIstatCom = getValoreStringa(x_persona, xmlns, "codIstatCom")
        persona.codIstatProv = getValoreStringa(x_persona, xmlns, "codIstatProv")
        persona.codProvNasc = getValoreStringa(x_persona, xmlns, "codProvNasc")
        persona.codTipoPersona = getValoreStringa(x_persona, xmlns, "codTipoPersona")
        persona.codiceFiscale = getValoreStringa(x_persona, xmlns, "codiceFiscale")
        persona.cognome = getValoreStringa(x_persona, xmlns, "cognome")
        persona.dataNascita = getValoreStringa(x_persona, xmlns, "dataNascita")
        persona.email = getValoreStringa(x_persona, xmlns, "email")
        persona.frazione = getValoreStringa(x_persona, xmlns, "frazione")
        persona.indirizzo = getValoreStringa(x_persona, xmlns, "indirizzo")
        persona.nome = getValoreStringa(x_persona, xmlns, "nome")
        persona.numTelefonico = getValoreStringa(x_persona, xmlns, "numTelefonico")
        persona.scoTipoPersona = getValoreStringa(x_persona, xmlns, "scoTipoPersona")
        persona.sesso = getValoreStringa(x_persona, xmlns, "sesso")

        Return persona
    End Function

    Private Function imposta_Possessi(doc As XDocument) As pc.webservice.sop.agrea.it.ArrayOf_tns1_ISWSPossesso
        Dim arrayPossessi As New pc.webservice.sop.agrea.it.ArrayOf_tns1_ISWSPossesso
        Dim xmlns As XNamespace = "http://pc.webService.sop.agrea.it"
        Dim x_domandaArr = (From xx In doc.Descendants(xmlns + "possessi") Select xx).FirstOrDefault
        If x_domandaArr IsNot Nothing Then

            Dim x_domandaS = (From xx In doc.Descendants(xmlns + "possessi") Select xx)
            If x_domandaS.Count > 1 Then
                Dim first = True
                For Each x_domanda In x_domandaS

                    If Not first Then
                        Dim possesso As New pc.common.webservice.sop.agrea.it.ISWSPossesso

                        possesso.codBioProdInt = getValoreStringa(x_domanda, xmlns, "codBioProdInt")
                        possesso.codCasoParticolare = getValoreStringa(x_domanda, xmlns, "codCasoParticolare")
                        possesso.codCom = getValoreStringa(x_domanda, xmlns, "codCom")
                        possesso.codIrrigazione = getValoreStringa(x_domanda, xmlns, "codIrrigazione")
                        possesso.codPossesso = getValoreStringa(x_domanda, xmlns, "codPossesso")
                        possesso.codProv = getValoreStringa(x_domanda, xmlns, "codProv")
                        possesso.codTipoIrrigazione = getValoreStringa(x_domanda, xmlns, "codTipoIrrigazione")
                        possesso.dataFinePoss = getValoreStringa(x_domanda, xmlns, "dataFinePoss")
                        possesso.dataInizioPoss = getValoreStringa(x_domanda, xmlns, "dataInizioPoss")
                        possesso.descComune = getValoreStringa(x_domanda, xmlns, "descComune")
                        possesso.descProvincia = getValoreStringa(x_domanda, xmlns, "descProvincia")
                        possesso.descrizioneBioProdInt = getValoreStringa(x_domanda, xmlns, "descrizioneBioProdInt")
                        possesso.descrizioneConduzione = getValoreStringa(x_domanda, xmlns, "descrizioneConduzione")
                        possesso.flagIrrigabilita = getValoreStringa(x_domanda, xmlns, "flagIrrigabilita")
                        possesso.flagIrrigabilitaPC = getValoreStringa(x_domanda, xmlns, "flagIrrigabilitaPC")
                        possesso.flagSecondoRaccolto = getValoreStringa(x_domanda, xmlns, "flagSecondoRaccolto")
                        possesso.foglio = getValoreStringa(x_domanda, xmlns, "foglio")
                        possesso.particella = getValoreStringa(x_domanda, xmlns, "particella")
                        possesso.scoBioProdInt = getValoreStringa(x_domanda, xmlns, "scoBioProdInt")
                        possesso.scoIrrigazione = getValoreStringa(x_domanda, xmlns, "scoIrrigazione")
                        possesso.scoTipoIrrigazione = getValoreStringa(x_domanda, xmlns, "scoTipoIrrigazione")
                        possesso.sezione = getValoreStringa(x_domanda, xmlns, "sezione")
                        possesso.siglaProvincia = getValoreStringa(x_domanda, xmlns, "siglaProvincia")
                        possesso.subalterno = getValoreStringa(x_domanda, xmlns, "subalterno")
                        possesso.supCatastale = getValoreDouble(x_domanda, xmlns, "supCatastale")
                        possesso.supCondotta = getValoreDouble(x_domanda, xmlns, "supCondotta")
                        possesso.supDisponibile = getValoreDouble(x_domanda, xmlns, "supDisponibile")
                        possesso.supPossesso = getValoreDouble(x_domanda, xmlns, "supPossesso")

                        possesso.vincoliBufferZoneFasceTampone = imposta_VincoliBufferZone(x_domanda)
                        possesso.macrousi = imposta_Macrousi(x_domanda)
                        possesso.codZone = imposta_Zone(x_domanda)

                        arrayPossessi.Add(possesso)
                    End If

                    first = False

                Next

                Return arrayPossessi
            End If

        End If

        Return Nothing
    End Function

    Private Function imposta_VincoliCondizionalita(doc As XDocument) As pc.webservice.sop.agrea.it.ArrayOf_tns1_ISWSVincoliCondizionalita
        Dim arrayVincCond As New pc.webservice.sop.agrea.it.ArrayOf_tns1_ISWSVincoliCondizionalita
        Dim xmlns As XNamespace = "http://pc.webService.sop.agrea.it"
        Dim x_domandaArr = (From xx In doc.Descendants(xmlns + "vincoliCondizionalita") Select xx).FirstOrDefault
        If x_domandaArr IsNot Nothing Then

            Dim x_domandaS = (From xx In doc.Descendants(xmlns + "vincoliCondizionalita") Select xx)
            If x_domandaS.Count > 1 Then
                Dim first = True
                For Each x_domanda In x_domandaS

                    If Not first Then
                        Dim condizionalita As New pc.common.webservice.sop.agrea.it.ISWSVincoliCondizionalita

                        condizionalita.annoAtto = getValoreInteger(x_domanda, xmlns, "annoAtto")
                        condizionalita.campoAtto = getValoreInteger(x_domanda, xmlns, "campoAtto")
                        condizionalita.campoCondizione = getValoreInteger(x_domanda, xmlns, "campoCondizione")
                        condizionalita.codAtto = getValoreInteger(x_domanda, xmlns, "codAtto")
                        condizionalita.codCondizione = getValoreInteger(x_domanda, xmlns, "codCondizione")
                        condizionalita.descrizione = getValoreStringa(x_domanda, xmlns, "descrizione")

                        arrayVincCond.Add(condizionalita)
                    End If

                    first = False

                Next

                Return arrayVincCond
            End If

        End If

        Return Nothing
    End Function

    Private Function imposta_Appezzamenti(doc As XDocument) As pc.webservice.sop.agrea.it.ArrayOf_tns1_ISWSAppezzamento
        Dim arrayApp As New pc.webservice.sop.agrea.it.ArrayOf_tns1_ISWSAppezzamento
        Dim xmlns As XNamespace = "http://pc.webService.sop.agrea.it"
        Dim x_domandaArr = (From xx In doc.Descendants(xmlns + "appezzamenti") Select xx).FirstOrDefault
        If x_domandaArr IsNot Nothing Then

            Dim x_domandaS = (From xx In doc.Descendants(xmlns + "appezzamenti") Select xx)
            If x_domandaS.Count > 1 Then
                Dim first = True
                For Each x_domanda In x_domandaS

                    If Not first Then
                        Dim appezzamento As New pc.common.webservice.sop.agrea.it.ISWSAppezzamento

                        appezzamento.codColtura = getValoreStringa(x_domanda, xmlns, "codColtura")
                        appezzamento.codVarieta = getValoreStringa(x_domanda, xmlns, "codVarieta")
                        appezzamento.descColtura = getValoreStringa(x_domanda, xmlns, "descColtura")
                        appezzamento.descVarieta = getValoreStringa(x_domanda, xmlns, "descVarieta")
                        appezzamento.idAppezzamento = getValoreStringa(x_domanda, xmlns, "idAppezzamento")
                        appezzamento.idIsola = getValoreStringa(x_domanda, xmlns, "idIsola")
                        appezzamento.supAppezzamentoHa = getValoreDouble(x_domanda, xmlns, "supAppezzamentoHa")
                        appezzamento.wkt = getValoreStringa(x_domanda, xmlns, "wkt")

                        arrayApp.Add(appezzamento)
                    End If

                    first = False

                Next

                Return arrayApp
            End If

        End If

        Return Nothing
    End Function

    Private Function imposta_Zone(doc As XElement) As pc.webservice.sop.agrea.it.ArrayOf_xsd_string
        Dim arrayZone As New pc.webservice.sop.agrea.it.ArrayOf_xsd_string
        Dim xmlns As XNamespace = "http://pc.webService.sop.agrea.it"
        Dim x_domandaArr = (From xx In doc.Descendants(xmlns + "codZone") Select xx).FirstOrDefault
        If x_domandaArr IsNot Nothing Then

            Dim x_domandaS = (From xx In doc.Descendants(xmlns + "codZone") Select xx)
            If x_domandaS.Count > 1 Then
                Dim first = True
                For Each x_domanda In x_domandaS

                    If Not first Then

                        arrayZone.Add(x_domanda.Value)

                    End If

                    first = False

                Next

                Return arrayZone
            End If

        End If

        Return Nothing
    End Function

    Private Function imposta_VincoliBufferZone(doc As XElement) As pc.webservice.sop.agrea.it.ArrayOf_tns1_ISWSVincoliBufferZoneFasceTampone
        Dim arrayZone As New pc.webservice.sop.agrea.it.ArrayOf_tns1_ISWSVincoliBufferZoneFasceTampone
        Dim xmlns As XNamespace = "http://pc.webService.sop.agrea.it"
        Dim x_domandaArr = (From xx In doc.Descendants(xmlns + "vincoliBufferZoneFasceTampone") Select xx).FirstOrDefault
        If x_domandaArr IsNot Nothing Then

            Dim x_domandaS = (From xx In doc.Descendants(xmlns + "vincoliBufferZoneFasceTampone") Select xx)
            If x_domandaS.Count > 1 Then
                Dim first = True
                For Each x_domanda In x_domandaS

                    If Not first Then

                        Dim zona = New pc.common.webservice.sop.agrea.it.ISWSVincoliBufferZoneFasceTampone

                        zona.areaPart = getValoreDouble(x_domanda, xmlns, "areaPart")
                        zona.codice = getValoreStringa(x_domanda, xmlns, "codice")
                        zona.codNazionale = getValoreStringa(x_domanda, xmlns, "codNazionale")
                        zona.dataIniziVal = getValoreStringa(x_domanda, xmlns, "dataIniziVal")
                        zona.descrFascia = getValoreStringa(x_domanda, xmlns, "descrFascia")
                        zona.istatc = getValoreStringa(x_domanda, xmlns, "istatc")
                        zona.istatp = getValoreStringa(x_domanda, xmlns, "istatp")
                        zona.ladiId = getValoreInteger(x_domanda, xmlns, "ladiId")
                        zona.percInte = getValoreDouble(x_domanda, xmlns, "percInte")
                        zona.sezioneCensuria = getValoreStringa(x_domanda, xmlns, "sezioneCensuria")
                        zona.supeInte = getValoreDouble(x_domanda, xmlns, "supeInte")
                        zona.tipoInte = getValoreStringa(x_domanda, xmlns, "tipoInte")

                        arrayZone.Add(zona)

                    End If

                    first = False

                Next

                Return arrayZone
            End If

        End If

        Return Nothing
    End Function

    Private Function imposta_Macrousi(doc As XElement) As pc.webservice.sop.agrea.it.ArrayOf_tns1_ISWSMacrouso
        Dim arrayMacrousi As New pc.webservice.sop.agrea.it.ArrayOf_tns1_ISWSMacrouso
        Dim xmlns As XNamespace = "http://pc.webService.sop.agrea.it"
        Dim x_domandaArr = (From xx In doc.Descendants(xmlns + "macrousi") Select xx).FirstOrDefault
        If x_domandaArr IsNot Nothing Then

            Dim x_domandaS = (From xx In doc.Descendants(xmlns + "macrousi") Select xx)
            If x_domandaS.Count > 1 Then
                Dim first = True
                For Each x_domanda In x_domandaS

                    If Not first Then

                        Dim macrouso = New pc.common.webservice.sop.agrea.it.ISWSMacrouso

                        macrouso.codMacrouso = getValoreStringa(x_domanda, xmlns, "codMacrouso")
                        macrouso.descMacrouso = getValoreStringa(x_domanda, xmlns, "descMacrouso")
                        macrouso.supMacrouso = getValoreDouble(x_domanda, xmlns, "supMacrouso")
                        macrouso.utilizzi = imposta_Utilizzi(x_domanda)

                        arrayMacrousi.Add(macrouso)

                    End If

                    first = False

                Next

                Return arrayMacrousi
            End If

        End If

        Return Nothing
    End Function


    Private Function imposta_UnitaArboreeVite(doc As XElement) As pc.webservice.sop.agrea.it.ArrayOf_tns1_ISWSUnitaArboreeVite
        Dim arrayUnita As New pc.webservice.sop.agrea.it.ArrayOf_tns1_ISWSUnitaArboreeVite
        Dim xmlns As XNamespace = "http://pc.webService.sop.agrea.it"
        Dim x_domandaArr = (From xx In doc.Descendants(xmlns + "unitaArboreeVite") Select xx).FirstOrDefault
        If x_domandaArr IsNot Nothing Then

            Dim x_domandaS = (From xx In doc.Descendants(xmlns + "unitaArboreeVite") Select xx)
            If x_domandaS.Count > 1 Then
                Dim first = True
                For Each x_domanda In x_domandaS

                    If Not first Then

                        Dim unita = New pc.common.webservice.sop.agrea.it.ISWSUnitaArboreeVite

                        unita.annoImpianto = getValoreInteger(x_domanda, xmlns, "annoImpianto")
                        unita.codVarieta = getValoreStringa(x_domanda, xmlns, "codVarieta")
                        unita.faseAllevamento = getValoreInteger(x_domanda, xmlns, "faseAllevamento")
                        unita.formaAllevamento = getValoreStringa(x_domanda, xmlns, "formaAllevamento")
                        unita.sestoImpiantoSuFila = getValoreInteger(x_domanda, xmlns, "sestoImpiantoSuFila")
                        unita.sestoImpiantoTraFile = getValoreInteger(x_domanda, xmlns, "sestoImpiantoTraFile")
                        unita.superficieUtilizzata = getValoreDouble(x_domanda, xmlns, "superficieUtilizzata")
                        unita.numeroPiante = getValoreInteger(x_domanda, xmlns, "numeroPiante")


                        arrayUnita.Add(unita)

                    End If

                    first = False

                Next

                Return arrayUnita
            End If

        End If

        Return Nothing
    End Function

    Private Function imposta_UnitaArboree(doc As XElement) As pc.webservice.sop.agrea.it.ArrayOf_tns1_ISWSUnitaArboree
        Dim arrayUnita As New pc.webservice.sop.agrea.it.ArrayOf_tns1_ISWSUnitaArboree
        Dim xmlns As XNamespace = "http://pc.webService.sop.agrea.it"
        Dim x_domandaArr = (From xx In doc.Descendants(xmlns + "unitaArboree") Select xx).FirstOrDefault
        If x_domandaArr IsNot Nothing Then

            Dim x_domandaS = (From xx In doc.Descendants(xmlns + "unitaArboree") Select xx)
            If x_domandaS.Count > 1 Then
                Dim first = True
                For Each x_domanda In x_domandaS

                    If Not first Then

                        Dim unita = New pc.common.webservice.sop.agrea.it.ISWSUnitaArboree

                        unita.annoImpianto = getValoreInteger(x_domanda, xmlns, "annoImpianto")
                        unita.codProtezione = getValoreStringa(x_domanda, xmlns, "codProtezione")
                        unita.descrProtezione = getValoreStringa(x_domanda, xmlns, "descrProtezione")
                        unita.faseAllevamento = getValoreInteger(x_domanda, xmlns, "faseAllevamento")
                        unita.numeroPiante = getValoreInteger(x_domanda, xmlns, "numeroPiante")
                        unita.scoProtezione = getValoreStringa(x_domanda, xmlns, "scoProtezione")

                        arrayUnita.Add(unita)

                    End If

                    first = False

                Next

                Return arrayUnita
            End If

        End If

        Return Nothing
    End Function

    Private Function imposta_Utilizzi(doc As XElement) As pc.webservice.sop.agrea.it.ArrayOf_tns1_ISWSUtilizzo
        Dim arrayUtilizzi As New pc.webservice.sop.agrea.it.ArrayOf_tns1_ISWSUtilizzo
        Dim xmlns As XNamespace = "http://pc.webService.sop.agrea.it"
        Dim x_domandaArr = (From xx In doc.Descendants(xmlns + "utilizzi") Select xx).FirstOrDefault
        If x_domandaArr IsNot Nothing Then

            Dim x_domandaS = (From xx In doc.Descendants(xmlns + "utilizzi") Select xx)
            If x_domandaS.Count > 1 Then
                Dim first = True
                For Each x_domanda In x_domandaS

                    If Not first Then

                        Dim utilizzo = New pc.common.webservice.sop.agrea.it.ISWSUtilizzo

                        utilizzo.codAttivitaMinima = getValoreStringa(x_domanda, xmlns, "codAttivitaMinima")
                        utilizzo.codColtura = getValoreStringa(x_domanda, xmlns, "codColtura")
                        utilizzo.codEpocaSemina = getValoreStringa(x_domanda, xmlns, "codEpocaSemina")
                        utilizzo.codificaColt = getValoreStringa(x_domanda, xmlns, "codificaColt")
                        utilizzo.codVarieta = getValoreStringa(x_domanda, xmlns, "codVarieta")
                        utilizzo.dataFineDestinazione = getValoreStringa(x_domanda, xmlns, "dataFineDestinazione")
                        utilizzo.dataInizioDestinazione = getValoreStringa(x_domanda, xmlns, "dataInizioDestinazione")
                        utilizzo.descColtura = getValoreStringa(x_domanda, xmlns, "descColtura")
                        utilizzo.descVarieta = getValoreStringa(x_domanda, xmlns, "descVarieta")
                        utilizzo.flagPrimoRaccolto = getValoreStringa(x_domanda, xmlns, "flagPrimoRaccolto")
                        utilizzo.progrRiga = getValoreInteger(x_domanda, xmlns, "progrRiga")
                        utilizzo.scoAttivitaMinima = getValoreStringa(x_domanda, xmlns, "scoAttivitaMinima")
                        utilizzo.scoEpocaSemina = getValoreStringa(x_domanda, xmlns, "scoEpocaSemina")
                        utilizzo.supUtilizzo = getValoreDouble(x_domanda, xmlns, "supUtilizzo")

                        utilizzo.unitaArboree = imposta_UnitaArboree(x_domanda)
                        utilizzo.unitaArboreeVite = imposta_UnitaArboreeVite(x_domanda)

                        arrayUtilizzi.Add(utilizzo)

                    End If

                    first = False

                Next

                Return arrayUtilizzi
            End If

        End If

        Return Nothing
    End Function

    Private Function getValoreStringa(element As XElement, ns As XNamespace, tagName As String) As String
        Dim strRet = Nothing

        If element.Element(ns + tagName) IsNot Nothing AndAlso element.Element(ns + tagName).Value IsNot Nothing Then
            strRet = element.Element(ns + tagName).Value
        End If

        Return strRet
    End Function

    Private Function getValoreInteger(element As XElement, ns As XNamespace, tagName As String) As Integer
        Dim strRet = 0

        If element.Element(ns + tagName) IsNot Nothing AndAlso element.Element(ns + tagName).Value IsNot Nothing AndAlso IsNumeric(element.Element(ns + tagName).Value) Then
            strRet = CInt(element.Element(ns + tagName).Value)
        End If

        Return strRet
    End Function

    Private Function getValoreDouble(element As XElement, ns As XNamespace, tagName As String) As Double
        Dim strRet As Double = 0

        If element.Element(ns + tagName).Value IsNot Nothing AndAlso IsNumeric(element.Element(ns + tagName).Value) Then
            Dim s As String = element.Element(ns + tagName).Value
            strRet = CDbl(s.Replace(".", ","))
        End If

        Return strRet
    End Function

    Private Function GetCustomBinding() As System.ServiceModel.Channels.Binding
        Dim asbe = New AsymmetricSecurityBindingElement()
        'asbe.MessageSecurityVersion = MessageSecurityVersion.WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12

        asbe.InitiatorTokenParameters = New System.ServiceModel.Security.Tokens.X509SecurityTokenParameters With {
            .InclusionMode = SecurityTokenInclusionMode.Never
        }

        asbe.RecipientTokenParameters = New System.ServiceModel.Security.Tokens.X509SecurityTokenParameters With {
            .InclusionMode = SecurityTokenInclusionMode.Never
        }

        'asbe.MessageProtectionOrder = System.ServiceModel.Security.MessageProtectionOrder.SignBeforeEncrypt

        asbe.SecurityHeaderLayout = SecurityHeaderLayout.Strict
        asbe.EnableUnsecuredResponse = True
        asbe.IncludeTimestamp = False
        asbe.SetKeyDerivation(False)
        'asbe.DefaultAlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.Basic128Rsa15
        asbe.EndpointSupportingTokenParameters.Signed.Add(New UserNameSecurityTokenParameters())
        asbe.EndpointSupportingTokenParameters.Signed.Add(New X509SecurityTokenParameters())

        Dim myBinding = New CustomBinding()
        myBinding.Elements.Add(asbe)
        myBinding.Elements.Add(New TextMessageEncodingBindingElement(MessageVersion.Soap11, Encoding.UTF8))

        Dim httpsBindingElement = New HttpTransportBindingElement()

        myBinding.Elements.Add(httpsBindingElement)

        Return myBinding
    End Function

End Class