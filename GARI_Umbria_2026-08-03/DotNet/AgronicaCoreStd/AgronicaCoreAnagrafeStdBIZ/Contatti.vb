Imports AgronicaCoreAnagrafeStdDAL
Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models

Public Class Contatti
    Inherits BaseBiz

    Public Sub New(dbContext As GiasDbContext)
        MyBase.New(dbContext)
    End Sub


    Public Function EstraiListaContatti(Piva As String, ByVal Sa_Cod As Integer) As List(Of APP_Contatti)

        Dim leggi As New Contatti_R()
        Return leggi.EstraiListaContatti(dbContext, Piva, Sa_Cod)

    End Function

    Public Function LeggiContatti(Piva As String) As List(Of APP_Contatti)

        Dim xLettura = New Contatti_R()
        Return xLettura.Leggi(dbContext, Piva, 0)

    End Function

    Public Function LeggiContattoBadge(NrBadge As String) As APP_Contatti

        Dim xLettura = New Contatti_R()
        Return xLettura.LeggiBadge(dbContext, NrBadge)

    End Function

    Public Sub ScriviContatti(listContatti As List(Of APP_Contatti), piva As String, cancella As Boolean)

        Dim count As Integer = 0
        Dim commit As Boolean = False
        Dim numContatti As Integer = listContatti.Count
        Dim commitCount As Integer = 100
        Dim xScrittura = New Contatti_W()

        If cancella Then
            CancellaContatti(piva)
        End If

        For Each contatto In listContatti
            count += 1
            commit = count Mod commitCount = 0 OrElse count = numContatti
            xScrittura.Scrivi(dbContext, contatto, commit)
        Next

    End Sub

    Public Sub CancellaContatti(contatto As APP_Contatti)

        Dim xScrittura = New Contatti_W()
        xScrittura.Cancella(dbContext, contatto, Nothing)

    End Sub

    Public Sub CancellaContatti(piva As String)

        Dim xScrittura = New Contatti_W()
        xScrittura.Cancella(dbContext, Nothing, piva)

    End Sub
End Class
