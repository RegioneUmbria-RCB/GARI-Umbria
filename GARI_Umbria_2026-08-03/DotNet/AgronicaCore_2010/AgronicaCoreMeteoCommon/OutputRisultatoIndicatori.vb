Imports System.Xml.Serialization
Imports AgronicaCoreModelliPrevisionaliBIZ

Public Class OutputRisultatoIndicatori
    Public Class OutputIndicatore

        Public ChiaveRichiesta As Integer
        Public Parametri As RisultatoElaborazioneIndicatori.Indicatore.ParametriElaborazione
        Public Risultato As RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione
        Public Stazione As String
        Public Modello As String
        Public Specie As String
        Public Avversita As String
        Public DescrParametri As String
        Public AuxData As Object

        Public Sub New()

        End Sub
        Public Sub New(ByVal indic As RisultatoElaborazioneIndicatori.Indicatore)
            Parametri = indic.Parametri
            Risultato = indic.Risultato
        End Sub
    End Class

    <XmlElement([Namespace]:="http://www.outputrisultatoindicatori.com")>
    Public Stato As RisultatoElaborazioneIndicatori.Stato_Elaborazione

    Public Indicatori As List(Of OutputIndicatore)

    Public Sub New()

    End Sub
    Public Sub New(ByVal rei As RisultatoElaborazioneIndicatori)
        Stato = rei.Stato
        Indicatori = New List(Of OutputIndicatore)
        For Each indic In rei.Indicatori
            Indicatori.Add(New OutputIndicatore(indic))
        Next
    End Sub
End Class

