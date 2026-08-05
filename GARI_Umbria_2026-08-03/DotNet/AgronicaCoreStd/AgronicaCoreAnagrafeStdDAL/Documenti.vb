Imports AgronicaCoreAnagrafeStdDAL
Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports Microsoft.EntityFrameworkCore

Public Class Documenti_R

    Public Function Leggi(dbContext As GiasDbContext, Documento_Cod As Integer, Optional Visita_Cod As Integer = 0) As List(Of APP_Documenti)

        Dim rval As List(Of APP_Documenti) = (
            From d In dbContext.APP_Documenti
            Where (Documento_Cod = 0 OrElse d.Documento_Cod = Documento_Cod) AndAlso d.Visita_Cod = Visita_Cod
            Select d
            Order By d.Descrizione
        ).ToList()

        Return rval

    End Function

    Public Function LeggiPerRicaricoDati(dbContext As GiasDbContext, piva As String, Optional Visita As Boolean = False) As List(Of APP_Documenti)

        Dim rval As List(Of APP_Documenti) = (
            From d In dbContext.APP_Documenti
            Where (piva = "" OrElse d.Piva = piva) AndAlso If(Visita, d.Visita_Cod <> 0, d.Visita_Cod = 0)
            Select d
        ).ToList()

        Return rval

    End Function

End Class

Public Class Documenti_W

    Public Sub Scrivi(dbContext As GiasDbContext, documento As APP_Documenti, commit As Boolean)

        dbContext.APP_Documenti.Add(documento)

        If commit Then
            dbContext.SaveChanges()
        End If

    End Sub

    Public Sub Cancella(dbContext As GiasDbContext, Documento_Cod As Integer)
        dbContext.Database.ExecuteSqlCommand("DELETE FROM [APP_Documenti] WHERE Documento_Cod = {0}", Documento_Cod)
    End Sub

End Class