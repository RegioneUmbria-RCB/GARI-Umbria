Imports AgronicaCoreEntityFrameworkSTD_POCO.Models

Public Class TempiRisorse_Movimenti

    Public Property Id_CDG_Movimenti As Integer

    Public Property Attivita As Attivita
    Public Property Progetto As APP_Imputazioni_Fasi
    Public Property Impianto As APP_Reg_Impianti

    Public Property DataOraInizio As DateTime
    Public Property DataOraFine As DateTime
    Public Property Qta As Decimal

    Public Property RisorsaAziendale As Risorsa_Aziendale

    Public ReadOnly Property InizioAttivita() As String
        Get
            Dim DataOra As String = ""
            If DataOraInizio > DateTime.MinValue Then
                DataOra = "Inizio: " & DataOraInizio.ToString("dd/MM/yyyy - HH:mm")
            End If
            Return DataOra
        End Get
    End Property

    Public ReadOnly Property FineAttivita() As String
        Get
            Dim DataOra As String = ""
            If DataOraFine > DateTime.MinValue Then
                DataOra = "Fine: " & DataOraFine.ToString("dd/MM/yyyy - HH:mm")
            End If
            Return DataOra
        End Get
    End Property

End Class
