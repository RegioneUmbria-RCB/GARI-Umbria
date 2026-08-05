
Imports AgronicaCoreAnagrafeStdDAL
Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports Microsoft.EntityFrameworkCore.Storage


Public Class MisuraXAvversita
    Inherits BaseBiz


    Public Sub New(dbContext As GiasDbContext)
        MyBase.New(dbContext)
    End Sub

    Public Function EstraiListaMisuraXAvversita(ByVal veg_cod As Integer) As List(Of AgronicaCoreModelloSTD.MisuraXAvversita)

        Dim leggi As New MisuraXAvversita_R
        Dim rval As New List(Of AgronicaCoreModelloSTD.MisuraXAvversita)
        rval = leggi.Leggi(dbContext, veg_cod, 0, 0, 0)

        Return rval

    End Function

    Public Sub ScriviMisuraXAvversita(listMisuraXAvversita As List(Of APP_MisuraxAvversita), cancella As Boolean)

        Dim count As Integer = 0
        Dim commit As Boolean = False
        Dim numMisuraXAvversita As Integer = listMisuraXAvversita.Count
        Dim commitCount As Integer = 100
        Dim xScrittura = New MisuraXAvversita_W()

        If cancella Then
            xScrittura.Cancella(dbContext, Nothing)
        End If

        For Each MisuraXAvversita In listMisuraXAvversita
            count += 1
            commit = count Mod commitCount = 0 OrElse count = numMisuraXAvversita
            xScrittura.Scrivi(dbContext, MisuraXAvversita, commit)
        Next

    End Sub

    Public Sub CancellaMisuraXAvversita(MisuraXAvversita As APP_MisuraxAvversita)

        Dim xScrittura = New MisuraXAvversita_W()
        xScrittura.Cancella(dbContext, MisuraXAvversita)

    End Sub
End Class
