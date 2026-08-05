Imports AgronicaCoreAnagrafeStdDAL
Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models

Public Class AttivitaOperazioni
    Inherits BaseBiz

    Public Sub New(dbContext As GiasDbContext)
        MyBase.New(dbContext)
    End Sub

    Public Function LeggiAttivitaOperazioni(Lav_Cod As Integer) As List(Of APP_AttivitaXOperazioni)

        Dim xLettura = New AttivitaXOperazioni_R()
        Return xLettura.Leggi(dbContext, Lav_Cod)

    End Function

    Public Sub ScriviAttivitaOperazioni(listAttivitaOperazioni As List(Of APP_AttivitaXOperazioni), cancella As Boolean)

        Dim count As Integer = 0
        Dim commit As Boolean = False
        Dim numAttivitaOperazioni As Integer = listAttivitaOperazioni.Count
        Dim commitCount As Integer = 100
        Dim xScrittura = New AttivitaXOperazioni_W()

        If cancella Then
            xScrittura.Cancella(dbContext, Nothing)
        End If

        For Each attivita In listAttivitaOperazioni
            count += 1
            commit = count Mod commitCount = 0 OrElse count = numAttivitaOperazioni
            xScrittura.Scrivi(dbContext, attivita, commit)
        Next

    End Sub

    Public Sub CancellaAttivita(attivita As APP_AttivitaXOperazioni)

        Dim xScrittura = New AttivitaXOperazioni_W()
        xScrittura.Cancella(dbContext, attivita)

    End Sub

End Class
