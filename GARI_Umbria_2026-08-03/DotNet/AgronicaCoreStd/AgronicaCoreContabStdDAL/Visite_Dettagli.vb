Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports Microsoft.EntityFrameworkCore

Public Class Visite_Dettagli_R

    Public Function Leggi(dbContext As GiasDbContext, Visita_Cod As Integer) As List(Of APP_Visite_Dettagli)

        Dim rval As List(Of APP_Visite_Dettagli) = (
            From v In dbContext.APP_Visite_Dettagli
            Where v.Visita_Cod = Visita_Cod
            Select v
        ).ToList()

        Return rval

    End Function

    Public Function LeggiPerRicaricoDati(dbContext As GiasDbContext, piva As String) As List(Of APP_Visite_Dettagli)

        Dim rval As List(Of APP_Visite_Dettagli) = (
            From v In dbContext.APP_Visite
            Join vd In dbContext.APP_Visite_Dettagli
                    On vd.Visita_Cod Equals v.Visita_Cod
            Where v.Piva = piva
            Select vd
        ).ToList()

        Return rval

    End Function

End Class

Public Class Visite_Dettagli_W
    Public Sub CancellaDaImpresa(dbContext As GiasDbContext, ByVal piva As String)
        dbContext.Database.ExecuteSqlCommand("DELETE FROM [APP_Visite_Dettagli] WHERE Visita_Cod in (select Visita_Cod from APP_Visite WHERE Piva={0})", piva)
    End Sub

    Public Sub CancellaDaVisitaCod(dbContext As GiasDbContext, Visita_Cod As Integer)
        dbContext.Database.ExecuteSqlCommand("DELETE FROM [APP_Visite_Dettagli] WHERE Visita_Cod = {0}", Visita_Cod)
    End Sub
End Class
