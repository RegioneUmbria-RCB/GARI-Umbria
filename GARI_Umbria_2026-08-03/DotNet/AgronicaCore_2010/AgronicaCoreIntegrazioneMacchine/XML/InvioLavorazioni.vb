Imports System.Xml.Serialization

''' <summary>
''' Classe per invio ordine lavorazione standard in formato XML
''' </summary>

Public Class InvioOrdineLavorazione
    <XmlAttribute>
    Public Versione As Integer = VersioneInvioOrdineLavorazione.VersioneBase
    Public IdLavorazione As String
    Public Data As String
    Public DescrizioneProdotto As String
    Public DescrizioneFornitore As String
    Public TotaleQtaIngresso As String
End Class

Public Enum VersioneInvioOrdineLavorazione
    VersioneBase = 1
End Enum

''' <summary>
''' Classe per invio primo ingresso lavorazione standard in formato XML
''' </summary>

Public Class InvioPrimoIngresso
    <XmlAttribute>
    Public Versione As Integer = VersioneInvioPrimoIngresso.VersioneBase
    Public IdLavorazione As String
End Class

Public Enum VersioneInvioPrimoIngresso
    VersioneBase = 1
End Enum