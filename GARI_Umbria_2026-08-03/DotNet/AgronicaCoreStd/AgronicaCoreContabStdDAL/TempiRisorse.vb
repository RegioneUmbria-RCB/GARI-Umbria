Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports AgronicaCoreModelloSTD
Imports Microsoft.EntityFrameworkCore

Public Class TempiRisorse_R

    Public Function LeggiPerCancellazione(dbContext As GiasDbContext, Piva As String) As List(Of Integer)

        Dim rval As List(Of Integer) = (
            From r In dbContext.APP_CDG_Generale
            Where (Piva = "" OrElse r.Piva = Piva) AndAlso r.Bozza = 0
            Select r.Id_Cdg_Generale
        ).ToList()

        Return rval

    End Function

    Public Function LeggiBozze(dbContext As GiasDbContext, Piva As String) As List(Of Integer)

        Dim listaBozze As List(Of Integer) = (
            From r In dbContext.APP_CDG_Generale
            Where (Piva = "" OrElse r.Piva = Piva) AndAlso r.Bozza = 1
            Select r.Id_Cdg_Generale
        ).ToList()

        Dim listaBozze2 As List(Of Integer) = (
            From r In dbContext.APP_Riferimenti_Interventi_Cdg
            Join op In dbContext.APP_Ricette_Operazioni On r.Ricetta_Cod Equals op.Ricetta_Cod And r.Ricetta_Operazione_Cod Equals op.Ricetta_Operazione_Cod
            Where (Piva = "" OrElse r.Piva = Piva) AndAlso op.Bozza = 1
            Select r.Id_Cdg_Generale_Rif
        ).ToList()

        Return listaBozze.Concat(listaBozze2).Distinct().ToList()

    End Function

    Public Function Leggi(dbContext As GiasDbContext, Id_Cdg_Generale As Integer) As List(Of APP_CDG_Generale)

        Dim rval As List(Of APP_CDG_Generale) = (
            From r In dbContext.APP_CDG_Generale
            Where Id_Cdg_Generale = 0 OrElse r.Id_Cdg_Generale = Id_Cdg_Generale
            Select r
            Order By r.Data_Inserimento Descending
        ).ToList()

        Return rval
    End Function

    Public Function Leggi(dbContext As GiasDbContext, Piva As String) As List(Of APP_CDG_Generale)

        Dim rval As List(Of APP_CDG_Generale) = (
            From r In dbContext.APP_CDG_Generale
            Where Piva = "" OrElse r.Piva = Piva
            Select r
            Order By r.Data_Inserimento Descending
        ).ToList()

        Return rval
    End Function

End Class

Public Class TempiRisorse_W

    Public Sub ImpostaBozzaSuAttivita(dbContext As GiasDbContext, ByVal Id_Cdg_Generale As Integer, ByVal Bozza As Integer)
        Dim letturaAttivita As New TempiRisorse_R
        Dim lista_attivita = letturaAttivita.Leggi(dbContext, Id_Cdg_Generale)
        For Each attivita In lista_attivita
            attivita.Bozza = Bozza
            dbContext.APP_CDG_Generale.Update(attivita)
            dbContext.SaveChanges()
        Next
    End Sub

    Public Sub Cancella(dbContext As GiasDbContext, Id_Cdg_Generale As Integer)
        dbContext.Database.ExecuteSqlCommand("DELETE FROM [APP_CDG_Generale] WHERE Id_Cdg_Generale = {0}", Id_Cdg_Generale)
    End Sub

    Public Sub Cancella(dbContext As GiasDbContext, Piva As String)
        dbContext.Database.ExecuteSqlCommand("DELETE FROM [APP_CDG_Generale] WHERE Piva = {0}", Piva)
    End Sub

End Class
