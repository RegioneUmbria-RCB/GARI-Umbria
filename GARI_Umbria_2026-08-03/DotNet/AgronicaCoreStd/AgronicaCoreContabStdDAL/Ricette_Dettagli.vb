Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports Microsoft.EntityFrameworkCore

Public Class Ricette_Dettagli_R


    Public Function LeggiPerRicaricoDati(dbContext As GiasDbContext, piva As String) As List(Of APP_Ricette_Dettagli)

        Dim rval As List(Of APP_Ricette_Dettagli) = (
            From r In dbContext.APP_Ricette
            Join op In dbContext.APP_Ricette_Dettagli
                    On op.Ricetta_Cod Equals r.ricetta_cod
            Where op.Ricetta_Operazione_Cod <= 0 AndAlso r.piva = piva
            Select op
        ).ToList()

        Return rval

    End Function


    Public Function Leggi(dbContext As GiasDbContext, ricetta_cod As Integer, ricetta_Operazione_cod As Integer) As List(Of APP_Ricette_Dettagli)

        Dim rval As List(Of APP_Ricette_Dettagli) = (
            From r In dbContext.APP_Ricette_Dettagli
            Where r.Ricetta_Cod = ricetta_cod AndAlso
                  r.Ricetta_Operazione_Cod = ricetta_Operazione_cod
            Select r
        ).ToList()

        Return rval
    End Function


End Class


Public Class Ricette_Dettagli_W




    Public Sub CancellaDataImpresa(dbContext As GiasDbContext, ByVal piva As String)

        dbContext.Database.ExecuteSqlCommand("DELETE FROM [APP_Ricette_Dettagli] WHERE Ricetta_COD in (select Ricetta_Cod from APP_Ricette WHERE Piva={0})", piva)

    End Sub

    Public Sub CancellaDaRicettaOperazioneCod(dbContext As GiasDbContext, ricettaOperazioneCod As Integer)
        dbContext.Database.ExecuteSqlCommand("DELETE FROM [APP_Ricette_Dettagli] WHERE Ricetta_Operazione_COD = {0}", ricettaOperazioneCod)
    End Sub
End Class
