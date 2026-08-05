Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports Microsoft.EntityFrameworkCore

Public Class RicetteXnote_R


    Public Function LeggiPerRicaricoDati(dbContext As GiasDbContext, piva As String) As List(Of APP_RicettexNote)

        Dim rval As List(Of APP_RicettexNote) = (
            From r In dbContext.APP_Ricette
            Join op In dbContext.APP_RicettexNote
                    On op.Ricetta_Cod Equals r.ricetta_cod
            Where op.Ricetta_Operazione_Cod <= 0 AndAlso r.piva = piva
            Select op
        ).ToList()

        Return rval

    End Function

End Class


Public Class RicetteXnote_W


    Public Sub CancellaDataImpresa(dbContext As GiasDbContext, ByVal piva As String)

        dbContext.Database.ExecuteSqlCommand("DELETE FROM [APP_RicetteXNote] WHERE Ricetta_COD in (select Ricetta_Cod from APP_Ricette WHERE Piva={0})", piva)

    End Sub

    Public Sub CancellaDaRicettaOperazioneCod(dbContext As GiasDbContext, ricettaOperazioneCod As Integer)
        dbContext.Database.ExecuteSqlCommand("DELETE FROM [APP_RicetteXNote] WHERE Ricetta_Operazione_COD = {0}", ricettaOperazioneCod)
    End Sub
End Class
