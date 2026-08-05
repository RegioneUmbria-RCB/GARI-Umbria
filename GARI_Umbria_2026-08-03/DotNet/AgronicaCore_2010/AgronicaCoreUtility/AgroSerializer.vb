Imports System.Xml
Imports System.Xml.Serialization
Imports System.IO
Imports System.Text
Imports System.Runtime.Serialization

Public Class AgroSerializer

    Public Shared Function GeneraXSD(Of T)(value As T) As String

        Dim schemas = New XmlSchemas()
        Dim exporter = New XmlSchemaExporter(schemas)
        Dim mapping = New XmlReflectionImporter().ImportTypeMapping(GetType(T))
        exporter.ExportTypeMapping(mapping)
        Dim schemaWriter = New StringWriter()
        For Each Xschema In schemas
            Xschema.Write(schemaWriter)
        Next
        Return schemaWriter.ToString()

    End Function

    Public Shared Function SerializzaQuesto(Of T)(value As T) As String

        If value Is Nothing Then
            Return Nothing
        End If

        Dim serializer As New XmlSerializer(GetType(T))

        Dim settings As New XmlWriterSettings()
        settings.Encoding = New UnicodeEncoding(False, False)
        ' no BOM in a .NET string
        settings.Indent = False
        settings.OmitXmlDeclaration = False

        Using textWriter As New StringWriter()
            Using xmlWriter__1 As XmlWriter = XmlWriter.Create(textWriter, settings)
                serializer.Serialize(xmlWriter__1, value)
            End Using
            Return textWriter.ToString()
        End Using
    End Function

    ''' <summary>
    ''' Serializes an object to Xml as a string.
    ''' </summary>
    ''' <typeparam name="T">Datatype T.</typeparam>
    ''' <param name="ToSerialize">Object of type T to be serialized.</param>
    ''' <returns>Xml string of serialized type T object.</returns>
    Public Shared Function Serialize_Use_DataContractSerializer_ToXmlString(Of T)(ToSerialize As T) As String
        Using memStm As New MemoryStream()
            Dim serializer = New DataContractSerializer(GetType(T))
            serializer.WriteObject(memStm, ToSerialize)

            memStm.Seek(0, SeekOrigin.Begin)

            Using streamReader = New StreamReader(memStm)
                Dim result As String = streamReader.ReadToEnd()
                Return result
            End Using
        End Using
    End Function

    ''' <summary>
    ''' Deserializes Xml string of type T.
    ''' </summary>
    ''' <typeparam name="T">Datatype T.</typeparam>
    ''' <param name="XmlString">Input Xml string from which to read.</param>
    ''' <returns>Returns rehydrated object of type T.</returns>
    Public Shared Function Deserialize_Use_DataContractSerializer_XmlString(Of T)(XmlString As String) As T
        Dim tempObject As T = Nothing

        Using memoryStream As New MemoryStream(StringToUTF8ByteArray(XmlString))

            Dim xs As New DataContractSerializer(GetType(T))
            Dim xmlTextWriter As New XmlTextWriter(memoryStream, Encoding.UTF8)

            tempObject = DirectCast(xs.ReadObject(memoryStream), T)
        End Using

        Return tempObject
    End Function



    ' Convert Array to String
    Public Shared Function UTF8ByteArrayToString(ArrBytes As [Byte]()) As [String]
        Return New UTF8Encoding().GetString(ArrBytes)
    End Function
    ' Convert String to Array
    Public Shared Function StringToUTF8ByteArray(XmlString As [String]) As [Byte]()
        Return New UTF8Encoding().GetBytes(XmlString)
    End Function

End Class
