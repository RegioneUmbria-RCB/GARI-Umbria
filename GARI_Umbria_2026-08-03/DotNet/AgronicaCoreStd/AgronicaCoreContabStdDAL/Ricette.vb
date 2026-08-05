Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports Microsoft.EntityFrameworkCore

Public Class Ricette_R


    Public Function LeggiPerRicaricoDati(dbContext As GiasDbContext, piva As String) As List(Of APP_Ricette)

        Dim rval As List(Of APP_Ricette) = (
            From r In dbContext.APP_Ricette
            Where r.ricetta_cod <= 0 AndAlso r.piva = piva
            Select r
        ).ToList()

        Return rval
    End Function

    Public Function Leggi(dbContext As GiasDbContext, ricetta_cod As Integer) As List(Of APP_Ricette)

        Dim rval As List(Of APP_Ricette) = (
            From r In dbContext.APP_Ricette
            Where ricetta_cod = 0 OrElse r.ricetta_cod = ricetta_cod
            Select r
        ).ToList()

        Return rval
    End Function

End Class


Public Class Ricette_W
    Public Sub CancellaDataImpresa(dbContext As GiasDbContext, ByVal piva As String)
        dbContext.Database.ExecuteSqlCommand("DELETE FROM [APP_Ricette] WHERE Piva={0}", piva)
    End Sub

    Public Sub CancellaDaRicettaCod(dbContext As GiasDbContext, ricettaCod As Integer)
        dbContext.Database.ExecuteSqlCommand("DELETE FROM [APP_Ricette] WHERE Ricetta_COD = {0}", ricettaCod)
    End Sub

    Public Sub AggiornaRicettaBozza(dbContext As GiasDbContext, ricettaOperazioneCod As Integer, bozza As Integer)
        dbContext.Database.ExecuteSqlCommand("UPDATE [APP_Ricette_Operazioni] SET Bozza = {1} WHERE Ricetta_Operazione_Cod = {0}", ricettaOperazioneCod, bozza)
    End Sub

End Class