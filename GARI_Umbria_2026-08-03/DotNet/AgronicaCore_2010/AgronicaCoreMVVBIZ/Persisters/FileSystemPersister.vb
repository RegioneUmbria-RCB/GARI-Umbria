Imports System.IO
Imports System.Text
Imports System.Xml.Serialization
Imports AgronicaCoreMVVBIZ.Integrazione.SIAN.MVV

Namespace Persisters
    Public Class FileSystemPersister

        Private ReadOnly _fileManager As IFIleManager
        Private ReadOnly _logger As MVVLogger

        Public Sub New(ByVal fileManager As IFIleManager, ByVal logger As MVVLogger)
            _fileManager = fileManager
            _logger = logger
        End Sub

        Public Function Persist(ByVal mvv As MVVSiRPVInput, Optional ByVal nomeFile As String = "") As Boolean

            Try
                Dim errori As String = String.Empty
                ScriviXML(mvv, mvv.GetType, nomeFile, _fileManager.OttieniPercorso(MVV_E_Path.XmlGenerati), errori)

            Catch ex As Exception
                _logger.Logga("FileSystemPersister.Persist", ex.Message)
                Throw ex
            End Try

            Return True

        End Function

        Private Sub ScriviXML(ByVal mvv As MVVSiRPVInput, ByVal objectType As Type,
                              ByVal nomeFile As String, ByVal outputFolder As String, ByRef errori As String)

            Dim xSer As New XmlSerializer(objectType)
            Dim objStreamWriter As New StreamWriter(Path.Combine(outputFolder, nomeFile))

            ' Giulia: 3/4/2018: Sono costretta a fare tutto questo giro per sostituire parte dell'intestazione
            Dim objStringWriter As New Utf8StringWriter()
            xSer.Serialize(objStringWriter, mvv)

            Dim xmlText As String = objStringWriter.ToString()
            xmlText = xmlText.Replace("encoding=""utf-16""", "encoding=""utf-8""")

            'Dim nuovaIntestazione As String = "<ns3:DatiFattura xmlns:ns2=""http://www.w3.org/2000/09/xmldsig#"" xmlns:ns3=""http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v2.0"" versione=""DAT20"">"
            'Dim reg As Regex = New Regex("<DatiFattura (.+?)>")
            'xmlText = reg.Replace(xmlText, nuovaIntestazione)
            'xmlText = xmlText.Replace("</DatiFattura>", "</ns3:DatiFattura>")

            objStreamWriter.Write(xmlText)

            objStreamWriter.Close()

        End Sub

        Public Sub SpostaXML(ByVal nomeFile As String)

            _fileManager.SpostaXMLInSpediti(nomeFile)

        End Sub

        Public Sub LeggiPDF(ByVal nomeFile As String, ByRef data() As Byte)

            Dim percorso = _fileManager.OttieniPercorso(MVV_E_Path.PdfRicevuti)
            Dim fullFilePath = Path.Combine(percorso, nomeFile)
            If File.Exists(fullFilePath) Then
                data = File.ReadAllBytes(fullFilePath)
            End If

        End Sub


        Public Function Persist(nomeFile As String, data() As Byte, dataMvv As Date) As Object

            Try
                Dim percorso = _fileManager.OttieniPercorso(MVV_E_Path.PdfRicevuti)

                If dataMvv > Date.MinValue Then
                    percorso &= "\" & dataMvv.Year & "\" & dataMvv.Month.ToString("00")
                End If

                If Not Directory.Exists(percorso) Then
                    Directory.CreateDirectory(percorso)
                End If

                Dim fullFilePath = Path.Combine(percorso, nomeFile)

                File.WriteAllBytes(fullFilePath, data)
            Catch ex As Exception
                _logger.Logga("FileSystemPersister.Persist", ex)
                Return False
            End Try

            Return True

        End Function

        Public Function Persist(nomeFile As String, data() As Byte) As Object
            Return Persist(nomeFile, data, Date.MinValue)
        End Function

    End Class

    Public NotInheritable Class Utf8StringWriter
        Inherits StringWriter

        Public Overrides ReadOnly Property Encoding As Encoding
            Get
                Return Encoding.UTF8
            End Get
        End Property
    End Class

End Namespace


