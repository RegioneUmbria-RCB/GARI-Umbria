Imports AgronicaCoreAnagrafeStdDAL
Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports Microsoft.EntityFrameworkCore.Storage


Public Class IndiciMaturita
    Inherits BaseBiz


    Public Sub New(dbContext As GiasDbContext)
        MyBase.New(dbContext)
    End Sub


    ''' <summary>
    ''' legge una lista di indici maturità filtrata per specie
    ''' </summary>
    ''' <param name="dbContext"></param>    
    ''' <returns></returns>
    Public Function EstraiListaIndiciMaturitaFiltrataPerSpecie(ByVal Veg_Cod As Integer) As List(Of AgronicaCoreModelloSTD.MisuraXindiciMaturita)

        Dim leggi As New IndiciMaturita_R()
        Return leggi.EstraiListaIndiciMaturitaFiltrataPerSpecie(dbContext, Veg_Cod, 0, 0)

    End Function

    Public Sub ScriviIndiciMaturita(listIndiciMaturita As List(Of APP_IndiciMaturita), cancella As Boolean)

        Dim count As Integer = 0
        Dim commit As Boolean = False
        Dim numIndiciMaturita As Integer = listIndiciMaturita.Count
        Dim commitCount As Integer = 100
        Dim xScrittura = New IndiciMaturita_W()

        If cancella Then
            xScrittura.Cancella(dbContext, Nothing)
        End If

        For Each IndiciMaturita In listIndiciMaturita
            count += 1
            commit = count Mod commitCount = 0 OrElse count = numIndiciMaturita
            xScrittura.Scrivi(dbContext, IndiciMaturita, commit)
        Next

    End Sub

    Public Sub CancellaIndiciMaturita(IndiciMaturita As APP_IndiciMaturita)

        Dim xScrittura = New IndiciMaturita_W()
        xScrittura.Cancella(dbContext, IndiciMaturita)

    End Sub
End Class
