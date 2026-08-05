Imports AgronicaCoreAnagrafeStdDAL
Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models

Public Class AttivitaCentriAziendali
    Inherits BaseBiz

    Public Sub New(dbContext As GiasDbContext)
        MyBase.New(dbContext)
    End Sub

    Public Function LeggiAttivitaCentriAziendali(Piva As String, Sa_Cod As Integer) As List(Of APP_AttivitaXCentri_Aziendali)

        Dim xLettura = New AttivitaXCentriAziendali_R()
        Return xLettura.Leggi(dbContext, Piva, Sa_Cod)

    End Function

    Public Sub ScriviAttivitaCentriAziendali(listAttivitaCentriAziendali As List(Of APP_AttivitaXCentri_Aziendali), piva As String, cancella As Boolean)

        Dim count As Integer = 0
        Dim commit As Boolean = False
        Dim numAttivitaCentriAziendali As Integer = listAttivitaCentriAziendali.Count
        Dim commitCount As Integer = 100
        Dim xScrittura = New AttivitaXCentriAziendali_W()

        If cancella Then
            xScrittura.Cancella(dbContext, Nothing, piva)
        End If

        For Each attivita In listAttivitaCentriAziendali
            ' forzo partita iva
            attivita.Piva = piva
            count += 1
            commit = count Mod commitCount = 0 OrElse count = numAttivitaCentriAziendali
            xScrittura.Scrivi(dbContext, attivita, commit)
        Next

    End Sub

    Public Sub CancellaAttivitaCentriAziendali(attivita As APP_AttivitaXCentri_Aziendali)

        Dim xScrittura = New AttivitaXCentriAziendali_W()
        xScrittura.Cancella(dbContext, attivita, Nothing)

    End Sub
End Class
