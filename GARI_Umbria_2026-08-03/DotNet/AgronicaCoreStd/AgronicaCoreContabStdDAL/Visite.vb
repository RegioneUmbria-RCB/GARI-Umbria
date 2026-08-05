Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports Microsoft.EntityFrameworkCore

Public Class Visite_R

    Public Function Leggi(dbContext As GiasDbContext, Visita_Cod As Integer) As List(Of APP_Visite)

        Dim rval As List(Of APP_Visite) = (
            From v In dbContext.APP_Visite
            Where Visita_Cod = 0 OrElse v.Visita_Cod = Visita_Cod
            Select v
            Order By v.Data_Visita Descending
        ).ToList()

        Return rval

    End Function

    Public Function LeggiPerRicaricoDati(dbContext As GiasDbContext, piva As String) As List(Of APP_Visite)

        Dim rval As List(Of APP_Visite) = (
            From r In dbContext.APP_Visite
            Where r.Piva = piva
            Select r
        ).ToList()

        Return rval
    End Function

End Class

Public Class Visite_W

    Public Sub CancellaDaImpresa(dbContext As GiasDbContext, ByVal piva As String)
        dbContext.Database.ExecuteSqlCommand("DELETE FROM [APP_Visite] WHERE Piva={0}", piva)
    End Sub

    Public Sub CancellaDaVisitaCod(dbContext As GiasDbContext, Visita_Cod As Integer)
        dbContext.Database.ExecuteSqlCommand("DELETE FROM [APP_Visite] WHERE Visita_Cod = {0}", Visita_Cod)
    End Sub

    Public Sub CancellaDocumentoVisita(dbContext As GiasDbContext, Visita_Cod As Integer)
        dbContext.Database.ExecuteSqlCommand("DELETE FROM [APP_Documenti] WHERE Visita_Cod = {0}", Visita_Cod)
    End Sub

End Class
