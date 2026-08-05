Imports System.IO
Imports System.Text
Imports System.Xml
Imports System.Xml.Xsl

Public Class XsltUtils

    Public Function trasformaDatoPercorsoFile(inputXML As String, filexslt As String) As String
        Dim fileXsltContent As String =
            My.Computer.FileSystem.ReadAllText(filexslt)

        Return trasforma(inputXML, fileXsltContent)
    End Function

    Public Function trasforma(inputXMLToTransform As String, xsltString As String) As String

        Dim xslt As XslCompiledTransform = New XslCompiledTransform()
        xslt.Load(XmlReader.Create(New StringReader(xsltString), New XmlReaderSettings()))
        Dim sb As StringBuilder = New StringBuilder()
        Dim writerSettings As XmlWriterSettings = New XmlWriterSettings()
        writerSettings.Encoding = Encoding.UTF8
        writerSettings.Indent = True
        writerSettings.OmitXmlDeclaration = True
        xslt.Transform(XmlReader.Create(New StringReader(inputXMLToTransform)), XmlWriter.Create(sb, writerSettings))
        Dim xml As String = "<?xml version=""1.0"" encoding=""UTF-8""?>" & vbCrLf & sb.ToString()
        Return xml

    End Function

End Class
