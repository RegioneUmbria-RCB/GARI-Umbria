Public Class Utility

    Public Shared Function getIntestazione(ByVal username As String, ByVal password As String, ByVal operCode As String) As XDocument

        '  Vanni, 25/10/2016 15:49:38: verificare se deve essere inviato anche un servizio.
        '"            <nomeServizio>" & operCode & "</nomeServizio>" &

        Dim intestazione = "<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:soap=""http://cooperazione.sian.it/schema/SoapAutenticazione"" xmlns:xs=""http://www.w3.org/2001/XMLSchema"" xmlns=""http://cooperazione.sian.it/schema/interscambio/"" >" &
                "<soapenv:Header>" &
                "        <soap:SOAPAutenticazione>" &
                "            <!--Credenziali rilasciate dal Ministero all'azienda-->" &
                "            <username>" & username & "</username>" &
                "            <password>" & password & "</password>" &
                "            <nomeServizio>" & operCode & "</nomeServizio>" &
                "        </soap:SOAPAutenticazione>" &
                "    </soapenv:Header> " &
                "<soapenv:Body></soapenv:Body>" &
                "</soapenv:Envelope>"
        Dim xD As XDocument = XDocument.Parse(intestazione)
        Return xD
    End Function

    Public Shared Function getSoapenv() As XNamespace
        Dim soapenv As XNamespace = "http://schemas.xmlsoap.org/soap/envelope/"
        Return soapenv
    End Function

    Public Shared Function getSoap() As XNamespace
        Dim soap As XNamespace = "http://cooperazione.sian.it/schema/SoapAutenticazione"
        Return soap
    End Function

    Public Shared Function getWsm() As XNamespace
        Dim wsm As XNamespace = "http://cooperazione.sian.it/schema/wsmrga/"
        Return wsm
    End Function



    Public Shared Sub cleanXmlSend(ByRef xml As XDocument)

        Dim listaNS As New List(Of String)
        listaNS.Add("http://schemas.xmlsoap.org/soap/envelope/")
        listaNS.Add("http://cooperazione.sian.it/schema/SoapAutenticazione")
        listaNS.Add("http://cooperazione.sian.it/schema/wsmrga/")
        AgronicaGIS2012.Commons.xmlHelper.RemoveNamespace2(xml, listaNS)

    End Sub

End Class
