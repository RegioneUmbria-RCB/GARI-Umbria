Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports AgronicaCoreEFatturaBIZ.Entita
Imports AgronicaCoreEFatturaBIZ.Persisters
Imports AgronicaCoreEFatturaDAL
Public Class MemoryPersister

    Private ReadOnly _logger As EFatturaLogger
    Public Sub New(ByVal logger As EFatturaLogger)
        _logger = logger
    End Sub
    Public Function ToMemoryStream(ByVal fattura As IFatturaElettronica, ByVal fatturaType As FatturaElettronicaType) As MemoryStream

        Dim nomeRoutine As String = "MemoryPersister.ToMemoryStream"

        Try

            Dim tipoBase As Type

            If fatturaType = FatturaElettronicaType.FatturaPa Then
                tipoBase = GetType(FatturaPa.FatturaElettronicaType)
            Else
                tipoBase = GetType(Semplificata.FatturaElettronicaType)
            End If

            Dim xSer As New XmlSerializer(tipoBase)

            ' Giulia: 3/4/2018: Sono costretta a fare tutto questo giro per sostituire parte dell'intestazione
            Dim objStringWriter As New Utf8StringWriter()
            xSer.Serialize(objStringWriter, fattura)

            Dim xmlText As String = objStringWriter.ToString()
            xmlText = xmlText.Replace("encoding=""utf-16""", "encoding=""utf-8""")

            Dim nuovaIntestazione As String = "<ns3:DatiFattura xmlns:ns2=""http://www.w3.org/2000/09/xmldsig#"" xmlns:ns3=""http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v2.0"" versione=""DAT20"">"
            Dim reg As Regex = New Regex("<DatiFattura (.+?)>", RegexOptions.None, TimeSpan.FromSeconds(3))
            xmlText = reg.Replace(xmlText, nuovaIntestazione)
            xmlText = xmlText.Replace("</DatiFattura>", "</ns3:DatiFattura>")

            Return New MemoryStream(ASCIIEncoding.UTF8.GetBytes(xmlText))

        Catch ex As Exception
            _logger.Logga(nomeRoutine, ex)
            Throw ex
        End Try

    End Function

End Class
