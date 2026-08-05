Public Class Ricette_Attivita

    Public Property Ricetta As Ricette_Operazioni
    Public Property Attivita As TempiRisorse
    Public Property DataRiferimento As DateTime

    ' 1=Intervento, 2=Attività, 3=Intervento/Attività
    Public ReadOnly Property Tipo() As Integer
        Get
            If Ricetta IsNot Nothing Then
                If Attivita Is Nothing Then
                    Return "1"
                Else
                    Return "3"
                End If
            End If
            Return "2"
        End Get
    End Property

    Public ReadOnly Property Colore() As String
        Get
            Dim Bozza As Boolean = False
            If Ricetta Is Nothing Then
                Bozza = Attivita.Bozza = 1
            ElseIf Attivita Is Nothing Then
                Bozza = Ricetta.Bozza = 1
            Else
                Bozza = Attivita.Bozza = 1 OrElse Ricetta.Bozza = 1
            End If
            If Not Bozza Then
                Return "LightGreen"
            End If
            Return "Default"
        End Get
    End Property

    Public ReadOnly Property Descrizione() As String
        Get
            If Ricetta IsNot Nothing Then
                If Attivita Is Nothing Then
                    Return Ricetta.Ricetta_Operazione_Des
                Else
                    Return Ricetta.Ricetta_Operazione_Des & " - " & Attivita.Attivita.Descrizione
                End If
            End If
            Return Attivita.Attivita.Descrizione
        End Get
    End Property

End Class
