Imports AgronicaCoreAnagrafeStdDAL
Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models

Public Class CategorieXUnitaMisura
    Inherits BaseBiz

    Public Sub New(dbContext As GiasDbContext)
        MyBase.New(dbContext)
    End Sub

    Public Sub ScriviCategoriaPerUnitaMisura(listaCategorieXUnitaMisura As List(Of APP_CategorieXUnitaMisura), cancella As Boolean)

        Dim count As Integer = 0
        Dim commit As Boolean = False
        Dim numCateg As Integer = listaCategorieXUnitaMisura.Count
        Dim commitCount As Integer = 100
        Dim xScrittura = New CategorieXUnitaMisura_W()

        If cancella Then
            xScrittura.Cancella(dbContext, Nothing)
        End If

        For Each categoria_um In listaCategorieXUnitaMisura
            count += 1
            commit = count Mod commitCount = 0 OrElse count = numCateg
            xScrittura.Scrivi(dbContext, categoria_um, commit)
        Next

    End Sub

End Class
