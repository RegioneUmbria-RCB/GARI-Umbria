Imports AgronicaCoreAnagrafeStdDAL
Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models

Public Class Magazzini
    Inherits BaseBiz
    Public Sub New(dbContext As GiasDbContext)
        MyBase.New(dbContext)
    End Sub

    Public Function EstraiListaMagazzini(Piva As String, Sa_Cod As Integer) As List(Of AgronicaCoreModelloSTD.Fabbricato)

        Dim xLettura As New Magazzini_R()
        Return xLettura.EstraiListaMagazzini(dbContext, Piva, Sa_Cod)

    End Function

    Public Sub ScriviMagazzini(listMagazzini As List(Of APP_Magazzini), piva As String, cancella As Boolean)

        Dim count As Integer = 0
        Dim commit As Boolean = False
        Dim numMagazzini As Integer = listMagazzini.Count
        Dim commitCount As Integer = 100
        Dim xScrittura = New Magazzini_W()

        If cancella Then
            xScrittura.Cancella(dbContext, Nothing, piva)
        End If

        For Each magazzino In listMagazzini
            count += 1
            commit = count Mod commitCount = 0 OrElse count = numMagazzini
            xScrittura.Scrivi(dbContext, magazzino, commit)
        Next

    End Sub

End Class
