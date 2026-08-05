Imports System.Xml.Serialization

''' <summary>
''' Classe per importazione lavorazione standard da XML
''' </summary>

Public Class EsitoLavorazione
    <XmlIgnore>
    Public VersioneCorrente As Integer = VersioneEsitoLavorazione.VersioneBase 'Ultima versione XML
    <XmlAttribute>
    Public Versione As Integer 'Versione effettivamente letta da XML
    Public IdLavorazione As String
    Public Lotto As String
    Public DataInizio As String
    Public OraInizio As String
    Public DataFine As String
    Public OraFine As String
    Public Dettagli As New List(Of DettaglioLavorazione)
End Class

Public Class DettaglioLavorazione
    Public Calibro As String
    Public Qualita As String
    Public Peso As String
    Public Numero As String
    Public UnitaMisura As String
    Public Tara As String
End Class

Public Enum VersioneEsitoLavorazione
    VersioneBase = 1
End Enum

