Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports Microsoft.EntityFrameworkCore

Public Class TempiRisorse_Riferimenti_R

    Public Function Leggi(dbContext As GiasDbContext, ricetta_cod As Integer) As List(Of APP_Riferimenti_Interventi_Cdg)

        Dim rval As List(Of APP_Riferimenti_Interventi_Cdg) = (
            From r In dbContext.APP_Riferimenti_Interventi_Cdg
            Join i In dbContext.APP_Ricette On r.Ricetta_Cod Equals i.ricetta_cod
            Where ricetta_cod = 0 OrElse r.Ricetta_Cod = ricetta_cod
            Select r
        ).ToList()

        Return rval
    End Function

    Public Function Leggi(dbContext As GiasDbContext, Piva As String) As List(Of APP_Riferimenti_Interventi_Cdg)

        Dim rval As List(Of APP_Riferimenti_Interventi_Cdg) = (
            From r In dbContext.APP_Riferimenti_Interventi_Cdg
            Join i In dbContext.APP_Ricette On r.Ricetta_Cod Equals i.ricetta_cod
            Where Piva = "" OrElse r.Piva = Piva
            Select r
        ).ToList()

        Return rval
    End Function

End Class

Public Class TempiRisorse_Riferimenti_W

    Public Sub Cancella(dbContext As GiasDbContext, Id_Cdg_Generale As Integer)
        dbContext.Database.ExecuteSqlCommand("DELETE FROM [APP_Riferimenti_Interventi_Cdg] WHERE Id_Cdg_Generale_Rif = {0}", Id_Cdg_Generale)
    End Sub

    Public Sub Cancella(dbContext As GiasDbContext, Piva As String)
        Dim gestione_bozze = ""
        dbContext.Database.ExecuteSqlCommand("DELETE FROM [APP_Riferimenti_Interventi_Cdg] WHERE Piva = {0}", Piva)
    End Sub

End Class