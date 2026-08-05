Imports System.IO
Imports System.Text
Imports System.Xml.Serialization
Imports AgronicaCoreMVVBIZ.Integrazione.SIAN.MVV

Public Class MemoryPersister

    'Private ReadOnly _logger As EFatturaLogger
    Public Sub New()
        '_logger = logger
    End Sub
    Public Function ToMemoryStream(ByVal mvv As MVVSiRPVInput) As MemoryStream

        Dim nomeRoutine As String = "MemoryPersister.ToMemoryStream"

        Try

            Dim xSer As New XmlSerializer(mvv.GetType)

            ' Giulia: 3/4/2018: Sono costretta a fare tutto questo giro per sostituire parte dell'intestazione
            Dim objStringWriter As New Utf8StringWriter()
            xSer.Serialize(objStringWriter, mvv)

            Dim xmlText As String = objStringWriter.ToString()
            xmlText = xmlText.Replace("encoding=""utf-16""", "encoding=""utf-8""")
            xmlText = xmlText.Replace(" xmlns=""http://cooperazione.sian.it/schema/wsmrga/""", "")

            Return New MemoryStream(ASCIIEncoding.UTF8.GetBytes(xmlText))

        Catch ex As Exception
            '_logger.Logga(nomeRoutine, ex)
            Throw ex
        End Try

    End Function

    Public NotInheritable Class Utf8StringWriter
        Inherits StringWriter

        Public Overrides ReadOnly Property Encoding As Encoding
            Get
                Return Encoding.UTF8
            End Get
        End Property
    End Class

End Class
