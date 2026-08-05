Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports Microsoft.EntityFrameworkCore

Public Class Visite_Destinazioni_R
    Public Function Leggi(dbContext As GiasDbContext, Visita_Cod As Integer) As List(Of APP_Visite_Destinazioni)

        Dim rval As List(Of APP_Visite_Destinazioni) = (
            From v In dbContext.APP_Visite_Destinazioni
            Where v.Visita_Cod = Visita_Cod
            Select v
        ).ToList()

        Return rval

    End Function

    Public Function LeggiPerRicaricoDati(dbContext As GiasDbContext, piva As String) As List(Of APP_Visite_Destinazioni)

        Dim rval As List(Of APP_Visite_Destinazioni) = (
            From v In dbContext.APP_Visite
            Join vd In dbContext.APP_Visite_Destinazioni
                    On vd.Visita_Cod Equals v.Visita_Cod
            Where v.Piva = piva
            Select vd
        ).ToList()

        Return rval

    End Function

End Class

Public Class Visite_Destinazioni_W
    Public Sub CancellaDaImpresa(dbContext As GiasDbContext, ByVal piva As String)
        dbContext.Database.ExecuteSqlCommand("DELETE FROM [APP_Visite_Destinazioni] WHERE Visita_Cod in (select Visita_Cod from APP_Visite WHERE Piva={0})", piva)
    End Sub

    Public Sub CancellaDaVisitaCod(dbContext As GiasDbContext, Visita_Cod As Integer)
        dbContext.Database.ExecuteSqlCommand("DELETE FROM [APP_Visite_Destinazioni] WHERE Visita_Cod = {0}", Visita_Cod)
    End Sub
End Class
