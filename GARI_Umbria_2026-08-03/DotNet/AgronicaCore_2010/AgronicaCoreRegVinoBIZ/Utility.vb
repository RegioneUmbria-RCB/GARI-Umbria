Imports System.IO
Imports System.Xml.Serialization
Imports System.Text
Imports System.Xml
Imports System.Security.Cryptography.X509Certificates

Public Class Utility

    Public Shared Function getIntestazione(ByVal username As String, ByVal password As String, ByVal operCode As String) As XDocument
        Dim intestazione = "<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:soap=""http://cooperazione.sian.it/schema/SoapAutenticazione"" xmlns:wsm=""http://cooperazione.sian.it/schema/wsmrga/"">" &
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

    Public Shared Function getCodOper(ByVal codOperCF As String, ByVal codOperPersonaFisica As Boolean) As CUAA
        Dim cuaa As CUAA = New CUAA
        If codOperCF.Length = 16 Then
            cuaa.ItemElementName = ItemChoiceType.PersonaFisica
        Else
            cuaa.ItemElementName = ItemChoiceType.PersonaGiuridica
        End If
        cuaa.Item = codOperCF
        Return cuaa
    End Function

    Public Shared Sub cleanXmlSend(ByRef xml As XDocument)

        Dim listaNS As New List(Of String)
        listaNS.Add("http://schemas.xmlsoap.org/soap/envelope/")
        listaNS.Add("http://cooperazione.sian.it/schema/SoapAutenticazione")
        listaNS.Add("http://cooperazione.sian.it/schema/wsmrga/")
        AgronicaGIS2012.Commons.xmlHelper.RemoveNamespace2(xml, listaNS)

    End Sub

    'Public Shared Function getBodyRequest(ByRef objToSerialize As Object) As String
    '    Dim x As New XmlSerializer(objToSerialize.GetType())
    '    Dim wsm As XNamespace = "http://schemas.xmlsoap.org/soap/envelope/"
    '    Dim objStreamWriter As New StringWriter()

    '    Dim nsSerializer = New XmlSerializerNamespaces()
    '    nsSerializer.Add("wsm", "http://cooperazione.sian.it/schema/wsmrga/")


    '    Dim sb As New StringBuilder()
    '    Dim settings As New XmlWriterSettings
    '    settings.OmitXmlDeclaration = True
    '    settings.NamespaceHandling = NamespaceHandling.OmitDuplicates

    '    Dim writer = XmlWriter.Create(sb, settings)

    '    x.Serialize(writer, objToSerialize, nsSerializer)

    '    Dim resBody As String = sb.ToString '.Replace("xmlns:wsm=""http://cooperazione.sian.it/schema/wsmrga/""", " ")

    '    'Dim xd As XElement
    '    'xd = XElement.Parse(resBody)
    '    Return resBody
    'End Function

    'Public Shared Function addBody(ByVal xml As XDocument, ByVal bodyRequest As String) As XDocument

    '    Dim xDtmp As XElement = XElement.Parse(bodyRequest)

    '    xml.Descendants(getSoapenv() + "Body").FirstOrDefault.Add(xDtmp)
    '    Return xml
    'End Function

    'Shared Function getObjectFromResponse(response As String, ByRef obj As Object) As Object
    '    'Dim responseWithouHeader
    '    Dim xDoc As XDocument = XDocument.Parse(response)
    '    Dim soapenv As XNamespace = "http://schemas.xmlsoap.org/soap/envelope/"
    '    Dim bodys = xDoc.Descendants(soapenv + "Body")
    '    Dim body = bodys(0).Elements()(0).ToString
    '    Dim x As New XmlSerializer(obj.GetType())
    '    obj = x.Deserialize(GenerateStreamFromString(body))
    '    Return obj
    'End Function

    'Shared Function getObjectFromXml(response As String, ByRef obj As Object) As Object
    '    'Dim responseWithouHeader
    '    Dim xDoc As XDocument = XDocument.Parse(response)
    '    Dim x As New XmlSerializer(obj.GetType())
    '    obj = x.Deserialize(GenerateStreamFromString(xDoc.ToString))
    '    Return obj
    'End Function

    'Shared Function getObjectFromXmlOperation(response As String, ByRef obj As Object) As Object
    '    'Dim responseWithouHeader
    '    Dim xDoc As XDocument = XDocument.Parse(response)
    '    cleanXmlSend(xDoc)
    '    Dim x As New XmlSerializer(obj.GetType())
    '    obj = x.Deserialize(GenerateStreamFromString(xDoc.ToString))
    '    Return obj
    'End Function

    'Shared Function GenerateStreamFromString(ByVal value As String) As MemoryStream
    '    Return New MemoryStream(Encoding.UTF8.GetBytes(value))
    'End Function

#Region "Enum e Costanti"

    Public Enum StatoGIAS
        creata = 18000001
        valida_per_invio = 18000002
        non_valida_per_invio = 18000003
        autorizzata_per_invio = 18000004
        invio_in_corso = 18000005
        invio_effettuato_correttamente = 18000006
        invio_non_riuscito = 18000007
        errori_rilevati_dal_Sian = 18000008
        valida_nel_sian = 18000009
        in_fase_di_verifica = 18000010
        eliminata_nel_sian = 18000011
        Aggiornamento_Prodotto = 18000012
        Prodotto_Aggiornato = 18000013
    End Enum

    'METODI
    'Asincroni
    Public Const SoggSiRPV = "SoggSiRPV"
    Public Const GetSoggSiRPV = "GetSoggSiRPV"
    Public Const CancSoggSiRPV = "CancSoggSiRPV"
    Public Const GetCancSoggSiRPV = "GetCancSoggSiRPV"

    Public Const VasiSiRPV = "VasiSiRPV"
    Public Const GetVasiSiRPV = "GetVasiSiRPV"
    Public Const CancVasiSiRPV = "CancVasiSiRPV"
    Public Const GetCancVasiSiRPV = "GetCancVasiSiRPV"

    Public Const VigneSiRPV = "VigneSiRPV"
    Public Const GetVigneSiRPV = "GetVigneSiRPV"
    Public Const CancVigneSiRPV = "CancVigneSiRPV"
    Public Const GetCancVigneSiRPV = "GetCancVigneSiRPV"

    Public Const OperSiRPV = "OperSiRPV"
    Public Const GetOperSiRPV = "GetOperSiRPV"
    Public Const CancOperSiRPV = "CancOperSiRPV"
    Public Const GetCancOperSiRPV = "GetCancOperSiRPV"

    Public Const ProdSiRPV = "ProdSiRPV"
    Public Const GetProdSiRPV = "GetProdSiRPV"
    Public Const CancProdSiRPV = "CancProdSiRPV"
    Public Const GetCancProdSiRPV = "GetCancProdSiRPV"

    Public Const CancAzieSiRPVInput = "CancAzieSiRPVInput"

    'Sincroni
    Public Const VisSoggSiRPV = "VisSoggSiRPV"
    Public Const VisOperSiRPV = "VisOperSiRPV"
    Public Const VisVasiSiRPV = "VisVasiSiRPV"
    Public Const VisVigneSiRPV = "VisVigneSiRPV"
    Public Const VisProdSiRPV = "VisProdSiRPV"
    Public Const VisGiacSiRPV = "VisGiacSiRPV"

    'OPERAZIONI
    Public Const ARMC = "ARMC"
    Public Const GIIN = "GIIN"
    Public Const CASD = "CASD"
    Public Const USSD = "USSD"
    Public Const IMBO = "IMBO"
    Public Const ACID = "ACID"
    Public Const PIGI = "PIGI"
    Public Const SVIN = "SVIN"
    Public Const DOLC = "DOLC"
    Public Const TAGL = "TAGL"
    Public Const CERT = "CERT"
    Public Const DENT = "DENT"
    Public Const AUCO = "AUCO"
    Public Const PERD = "PERD"
    Public Const SFEC = "SFEC"
    Public Const AVLT = "AVLT"
    Public Const SPGS = "SPGS"
    Public Const SPAB = "SPAB"
    Public Const SCDS = "SCDS"
    Public Const ELMC = "ELMC"
    Public Const FRGS = "FRGS"
    Public Const SCZC = "SCZC"
    Public Const LIEL = "LIEL"
    Public Const EVAL = "EVAL"
    Public Const FRAB = "FRAB"
    Public Const AARD = "AARD"
    Public Const DISA = "DISA"
    Public Const BABS = "BABS"
    Public Const ETIC = "ETIC"
    Public Const SUPE = "SUPE"
    Public Const DERI = "DERI"
    Public Const DIST = "DIST"
    Public Const APRT = "APRT"
    Public Const TRSO = "TRSO"
    Public Const ACET = "ACET"

    Public Const DataOperazione = "data_operazione"
    Public Const NumOperazione = "num_operazione"
    Public Const NumGiustificativo = "num_giustificativo"
    Public Const DataGiustificativo = "data_giustificativo"
    Public Const EsoneroDeroga = "deroga"
    Public Const CodRecipiente = "codRecipiente"
    Public Const CodCategoria = "categoria"
    Public Const Quantita = "qta"
    Public Const AttoCert = "certificato"
    Public Const CodClassificazione = "classificazione"
    Public Const PraticheEnologiche = "praticaenologica"
    Public Const CodStatoFisico = "statofisico"
    Public Const CodZonaViticola = "zonaviticola"
    Public Const CodColore = "colore"
    Public Const TitoloAlcolPot = "TitoloAlcolPot"
    Public Const Biologico = "biologico"
    Public Const Provenienza = "provenienza"
    Public Const TitoloAlcolTot = "TitoloAlcolTot"
    Public Const VolNominale = "VolNominale"
    Public Const NumConf = "NumConf"

    Public Const CertificateFile_Test = "C:\registri.sian.it.crt"
    'Public Const CertificateFile_Test = "C:\cooperazione.sian.it.crt"
    Public Const CertificateFile = "C:\cooperazione.sian.it.crt"
    'Public Const CertificateFile = "C:\registri.sian.it.crt"
    'Public Const WebServiceAsyncAddress_Test = "https://registri.sian.it/wsTOAST/services/wsRegVinoAsync"
    'Public Const WebServiceAsyncAddress = "https://cooperazione.sian.it/wsTOAST/services/wsRegVinoAsync"
    'Public Const WebServiceSyncAddress_Test = "https://registri.sian.it/wsTOAST/services/wsRegVino"
    'Public Const WebServiceSyncAddress = "https://cooperazione.sian.it/wsTOAST/services/wsRegVino"

    Public Const TipoRitornoErrore = "Errore"
    Public Const TipoRitornoInfo = "Info"
    Public Const TipoRitornoWarning = "Warning"

    Public Const separator = "|"
    Public Const wsmNamespace = "wsm"

#End Region

End Class
