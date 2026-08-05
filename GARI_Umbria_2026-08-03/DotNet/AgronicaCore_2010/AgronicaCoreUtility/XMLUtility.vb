Imports System.Xml.Serialization
Imports System.IO
Imports System.Xml
Imports System.Text


Imports System.Runtime.Serialization

Public Class XMLUtility
    'Prefisso e Namespace da aggiungere eventualmente ai tag del body
    Public Shared Function getBodyRequest(ByRef objToSerialize As Object, _
                                          Optional ByRef prefix As String = Nothing, _
                                          Optional ByRef ns As String = Nothing, _
                                           Optional ByRef types As Type() = Nothing) As String
        Dim x As XmlSerializer
        If types Is Nothing Then
            x = New XmlSerializer(objToSerialize.GetType())
        Else
            x = New XmlSerializer(objToSerialize.GetType(), types)
        End If
        Dim objStreamWriter As New StringWriter()

        Dim nsSerializer = New XmlSerializerNamespaces()
        'nsSerializer.Add("wsm", "http://cooperazione.sian.it/schema/wsmrga/")
        If (prefix IsNot Nothing) And (ns IsNot Nothing) Then
            nsSerializer.Add(prefix, ns)
        End If

        Dim sb As New StringBuilder()
        Dim settings As New XmlWriterSettings
        settings.OmitXmlDeclaration = True
        settings.NamespaceHandling = NamespaceHandling.OmitDuplicates

        Dim writer = XmlWriter.Create(sb, settings)

        x.Serialize(writer, objToSerialize, nsSerializer)

        Dim resBody As String = sb.ToString '.Replace("xmlns:wsm=""http://cooperazione.sian.it/schema/wsmrga/""", " ")

        'Dim xd As XElement
        'xd = XElement.Parse(resBody)
        Return resBody
    End Function

    Public Shared Function addBody(ByVal xml As XDocument, _
                                   ByVal bodyRequest As String, _
                                   Optional ByRef ns As XNamespace = Nothing) As XDocument

        Dim xDtmp As XElement = XElement.Parse(bodyRequest)
        If ns Is Nothing Then
            xml.Descendants("Body").FirstOrDefault.Add(xDtmp)
        Else
            xml.Descendants(ns + "Body").FirstOrDefault.Add(xDtmp)
        End If

        Return xml
    End Function

    Shared Function getObjectFromResponse(response As String, _
                                          ByRef obj As Object, _
                                          Optional ByRef ns As XNamespace = Nothing, _
                                          Optional ExtractBody As Boolean = True) As Object
        'Dim responseWithouHeader
        Dim xDoc As XDocument = XDocument.Parse(response)
        Dim body As String = ""
        Dim x As New XmlSerializer(obj.GetType)
        If ExtractBody Then
            If ns Is Nothing Then
                Dim bodys = xDoc.Descendants("Body")
                body = bodys(0).Elements()(0).ToString
            Else
                Dim bodys = xDoc.Descendants(ns + "Body")
                body = bodys(0).Elements()(0).ToString
            End If
            obj = x.Deserialize(GenerateStreamFromString(body))
        Else
            obj = x.Deserialize(GenerateStreamFromString(xDoc.ToString))
        End If
        Return obj
    End Function

    Shared Function getStringFromObject(ByRef obj As Object) As String
        Dim x As New XmlSerializer(obj.GetType)
        Dim s As New StringWriter()
        x.Serialize(s, obj)
        Return s.ToString
    End Function

    Shared Function getObjectFromXml(response As String, ByRef obj As Object) As Object
        Dim xDoc As XDocument = XDocument.Parse(response)
        Dim x As New XmlSerializer(obj.GetType())
        obj = x.Deserialize(GenerateStreamFromString(xDoc.ToString))
        Return obj
    End Function

    Shared Function GenerateStreamFromString(ByVal value As String) As MemoryStream
        Return New MemoryStream(Encoding.UTF8.GetBytes(value))
    End Function



    Public Shared Function ToDataTable(ByVal element As XElement) As DataTable
        Dim ds As DataSet = New DataSet()
        Dim rawXml As String = element.ToString()
        ds.ReadXml(New StringReader(rawXml))
        Return ds.Tables(0)
    End Function


    Public Shared Function XmlToDataTable(ByVal elements As IEnumerable(Of XElement)) As DataTable
        Return ToDataTable(New XElement("Root", elements))
    End Function

    ''' <summary>
    ''' Serializza un oggetto di tipo T in una stringa
    ''' </summary>
    ''' <typeparam name="T">Tipo di oggetto</typeparam>
    ''' <param name="o">Oggetto da serializzare</param>
    ''' <returns></returns>
    Public Shared Function SerializzaOggetto(Of T)(ByVal objectToSerialize As T, ByVal NomeElementoRadice As String) As String

        Using memStm As MemoryStream = New MemoryStream()
            Dim serializer As DataContractSerializer
            If NomeElementoRadice = "" Then
                serializer = New DataContractSerializer(GetType(T))
            Else
                serializer = New DataContractSerializer(GetType(T), NomeElementoRadice, "")
            End If

            serializer.WriteObject(memStm, objectToSerialize)
            memStm.Seek(0, SeekOrigin.Begin)

            Using streamReader = New StreamReader(memStm)
                Dim result As String = streamReader.ReadToEnd()
                Return result
            End Using
        End Using

    End Function

    ''' <summary>
    ''' Deserializza l'oggetto
    ''' </summary>
    ''' <typeparam name="T">Tipo da deserializzare</typeparam>
    ''' <param name="xml"></param>
    ''' <returns></returns>
    Public Shared Function DeserializzaOggetto(Of T)(ByVal xml As String, ByVal NomeElementoRadice As String) As T
        Dim result = Nothing
        Dim serializer As DataContractSerializer
        If NomeElementoRadice = "" Then
            serializer = New DataContractSerializer(GetType(T))
        Else
            serializer = New DataContractSerializer(GetType(T), NomeElementoRadice, "")
        End If

        Using stream = New MemoryStream()
            Dim writer = New StreamWriter(stream)
            writer.Write(xml)
            writer.Flush()
            stream.Position = 0
            result = CType(serializer.ReadObject(stream), T)
        End Using

        Return result
    End Function

    ''' <summary>
    ''' Serializza l'oggetto passato su stringa (in UTF8)
    ''' </summary>
    ''' <typeparam name="T">Tipo da serializzare</typeparam>
    ''' <param name="serializableObject">Oggetto da serializzare</param>
    ''' <param name="xmlSettings">Opzionale - Settings (Default: Indent = True, Encoding = New UTF8Encoding(False))</param>
    ''' <returns>L'oggetto xml serializzato su string</returns>
    Public Shared Function SerializeToString(Of T)(ByVal serializableObject As T,
                                                   Optional ByVal xmlSettings As XmlWriterSettings = Nothing
                                                   ) As String

        Dim xmlSerializer = New XmlSerializer(serializableObject.GetType())

        If xmlSettings Is Nothing Then
            xmlSettings = New XmlWriterSettings() With {
                .Encoding = New UTF8Encoding(False),
                .Indent = True
            }
        End If

        Using ms = New MemoryStream()
            Using xw = XmlWriter.Create(ms, xmlSettings)
                xmlSerializer.Serialize(xw, serializableObject)
                Return Encoding.UTF8.GetString(ms.ToArray())
            End Using
        End Using
    End Function


    Private Shared Function CreateOverrider(Of T)() As XmlSerializer
        Dim xOver As XmlAttributeOverrides = New XmlAttributeOverrides()
        Dim attrs As XmlAttributes = New XmlAttributes()
        attrs.XmlIgnore = True
        xOver.Add(GetType(T), "ChangeTracker", attrs)
        Dim xSer As XmlSerializer = New XmlSerializer(GetType(T), xOver)
        Return xSer
    End Function


    Public Shared Function RimuoviNameSpaceDaXElement(ByVal root As XElement) As XElement
        Return New XElement(root.Name.LocalName, (From n In root.Nodes() Select (If((TypeOf n Is XElement), RimuoviNameSpaceDaXElement(TryCast(n, XElement)), n))), If((root.HasAttributes), (From a In root.Attributes() Where (Not a.IsNamespaceDeclaration) Select New XAttribute(a.Name.LocalName, a.Value)), Nothing))
    End Function

End Class
