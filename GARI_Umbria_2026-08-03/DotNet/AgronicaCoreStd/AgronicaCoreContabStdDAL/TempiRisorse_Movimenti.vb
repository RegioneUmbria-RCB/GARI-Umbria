Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports Microsoft.EntityFrameworkCore

Public Class TempiRisorse_Movimenti_R

    Public Function Leggi(dbContext As GiasDbContext, Id_Cdg_Generale As Integer) As List(Of APP_CDG_Movimenti)

        Dim rval As List(Of APP_CDG_Movimenti) = (
            From r In dbContext.APP_CDG_Movimenti
            Where Id_Cdg_Generale = 0 OrElse r.Id_Cdg_Generale = Id_Cdg_Generale
            Select r
            Order By r.Data_Ora_Inizio Descending
        ).ToList()

        Return rval
    End Function

    Public Function Leggi(dbContext As GiasDbContext, Piva As String) As List(Of APP_CDG_Movimenti)

        Dim rval As List(Of APP_CDG_Movimenti) = (
            From r In dbContext.APP_CDG_Movimenti
            Where Piva = "" OrElse r.Piva = Piva
            Select r
            Order By r.Data_Ora_Inizio Descending
        ).ToList()

        Return rval
    End Function

End Class

Public Class TempiRisorse_Movimenti_W

    Public Sub Cancella(dbContext As GiasDbContext, Id_Cdg_Generale As Integer)
        dbContext.Database.ExecuteSqlCommand("DELETE FROM [APP_CDG_Movimenti] WHERE Id_Cdg_Generale = {0}", Id_Cdg_Generale)
    End Sub

    Public Sub Cancella(dbContext As GiasDbContext, Piva As String)
        dbContext.Database.ExecuteSqlCommand("DELETE FROM [APP_CDG_Movimenti] WHERE Piva = {0}", Piva)
    End Sub

End Class