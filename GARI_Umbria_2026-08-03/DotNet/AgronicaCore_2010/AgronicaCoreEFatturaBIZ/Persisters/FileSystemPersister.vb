Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports AgronicaCoreEFatturaBIZ.Entita
Imports AgronicaCoreEFatturaDAL

Namespace Persisters
    Public Class FileSystemPersister : Implements IPersister

        Private ReadOnly _fileManager As IFIleManager
        Private ReadOnly _logger As EFatturaLogger
        Public Sub New(ByVal fileManager As IFIleManager, ByVal logger As EFatturaLogger)
            _fileManager = fileManager
            _logger = logger
        End Sub

        Public Function Persist(
        ByVal fattura As IFatturaElettronica,
        ByVal fatturaType As FatturaElettronicaType,
        Optional ByVal nomeFile As String = "") As Boolean Implements IPersister.Persist

            Dim tipoBase As Type

            Try
                If fatturaType = FatturaElettronicaType.FatturaPa Then
                    tipoBase = GetType(FatturaPa.FatturaElettronicaType)
                Else
                    tipoBase = GetType(Semplificata.FatturaElettronicaType)
                End If

                Dim errori As String = String.Empty
                ScriviXML(fattura, tipoBase, nomeFile, _fileManager.OttieniPercorso(FatturaElettronicaPath.XmlGenerati), errori)

            Catch ex As Exception
                _logger.Logga("FileSystemPersister.Persist", ex.Message)
                Throw ex
            End Try

            Return True

        End Function

        Private Sub ScriviXML(ByVal fattura As IFatturaElettronica, ByVal objectType As Type,
                              ByVal nomeFile As String, ByVal outputFolder As String, ByRef errori As String)

            Dim xSer As New XmlSerializer(objectType)
            Dim objStreamWriter As New StreamWriter(Path.Combine(outputFolder, nomeFile))

            ' Giulia: 3/4/2018: Sono costretta a fare tutto questo giro per sostituire parte dell'intestazione
            Dim objStringWriter As New Utf8StringWriter()
            xSer.Serialize(objStringWriter, fattura)

            Dim xmlText As String = objStringWriter.ToString()
            xmlText = xmlText.Replace("encoding=""utf-16""", "encoding=""utf-8""")

            Dim nuovaIntestazione As String = "<ns3:DatiFattura xmlns:ns2=""http://www.w3.org/2000/09/xmldsig#"" xmlns:ns3=""http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v2.0"" versione=""DAT20"">"
            Dim reg As Regex = New Regex("<DatiFattura (.+?)>", RegexOptions.None, TimeSpan.FromSeconds(3))
            xmlText = reg.Replace(xmlText, nuovaIntestazione)
            xmlText = xmlText.Replace("</DatiFattura>", "</ns3:DatiFattura>")

            objStreamWriter.Write(xmlText)

            objStreamWriter.Close()

        End Sub

        Public Function Persist(fattura As FatturaGias, nomeFile As String, ByVal stato As StatoFattura_Gias) As Object Implements IPersister.Persist
            Throw New NotImplementedException()
        End Function

        Public Function Update(entita As AgronicaCoreEntityFramework_POCO.SDI_Log) As String Implements IPersister.Update
            Throw New NotImplementedException()
        End Function

        Public Function Persist(nomeFile As String, data() As Byte, dataSdi As Date) As Object Implements IPersister.Persist

            Try
                Dim percorso = _fileManager.OttieniPercorso(FatturaElettronicaPath.XmlCicloPassivo)

                If dataSdi > Date.MinValue Then
                    percorso &= "\" & dataSdi.Year & "\" & dataSdi.Month.ToString("00")
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

        Public Function Persist(nomeFile As String, data() As Byte) As Object Implements IPersister.Persist
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


