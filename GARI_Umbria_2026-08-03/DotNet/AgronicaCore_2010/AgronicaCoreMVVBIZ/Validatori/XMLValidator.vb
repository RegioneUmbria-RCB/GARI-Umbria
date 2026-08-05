Imports System.IO
Imports System.Reflection
Imports System.Xml
Imports System.Xml.Schema

Public Class XMLValidator

    Private _settings As XmlReaderSettings
    Private _errors As New List(Of String)
    Private ReadOnly _fileManager As IFIleManager

    Public Sub New(ByVal fileManager As IFIleManager)
        _fileManager = fileManager
    End Sub
    Public Sub Inizialize()

        Dim ass = Assembly.GetExecutingAssembly()
        Dim schemaMvv = ass.GetManifestResourceNames().Where(Function(m) m.Equals("AgronicaCoreMVVBIZ.wsmrga.xsd")).FirstOrDefault()

        Dim ss = New XmlSchemaSet()
        ss.Add("http://cooperazione.sian.it/schema/wsmrga/", XmlReader.Create(ass.GetManifestResourceStream(schemaMvv)))

        If ss.Count > 0 Then

            _settings = New XmlReaderSettings()
            _settings.ValidationType = ValidationType.Schema
            _settings.Schemas.Add(ss)
            _settings.Schemas.Compile()

            AddHandler _settings.ValidationEventHandler, AddressOf ValidationCallBack

        End If

    End Sub
    Public Function Validate(ByVal ms As MemoryStream, ByRef errori As List(Of String)) As Boolean

        Try

            _errors.Clear()
            Dim r = New XmlTextReader(ms)
            Dim reader = XmlReader.Create(r, _settings)
            While reader.Read()
            End While
            errori = _errors

        Catch ex As Exception
            Return False
        End Try

        Return Not _errors.Any()

    End Function

    Private Sub ValidationCallBack(ByVal sender As Object, ByVal args As ValidationEventArgs)
        If args.Severity = XmlSeverityType.Warning Then
        Else
            _errors.Add("Validation error: " & args.Message)
        End If
    End Sub

End Class



