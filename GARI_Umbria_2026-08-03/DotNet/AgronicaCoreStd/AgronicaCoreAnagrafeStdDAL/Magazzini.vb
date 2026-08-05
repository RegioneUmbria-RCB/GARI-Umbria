Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports AgronicaCoreModelloSTD
Imports Microsoft.EntityFrameworkCore

Public Class Magazzini_R

    Public Function Leggi(dbContext As GiasDbContext) As List(Of APP_Magazzini)

        Return dbContext.APP_Magazzini.ToList()

    End Function

    Public Function LeggiMagazzino(
        dbContext As GiasDbContext,
        Piva As String,
        Sa_Cod As Integer,
        Tipo_Destinazione As Integer,
        Fabbricato_Cod As Integer
     ) As APP_Magazzini

        Dim rval As APP_Magazzini = (
            From p In dbContext.APP_Magazzini
            Where p.Piva = Piva AndAlso
                  p.Sa_Cod = Sa_Cod AndAlso
                  p.Tipo_Destinazione = Tipo_Destinazione AndAlso
                  p.Id_Destinazione = Fabbricato_Cod
            ).FirstOrDefault

        Return rval

    End Function

    Public Function EstraiListaMagazzini(dbcontext As GiasDbContext, Piva As String, ByVal Sa_Cod As Integer) As List(Of AgronicaCoreModelloSTD.Fabbricato)

        Dim rval As List(Of Fabbricato) = (
              From m In dbcontext.APP_Magazzini
              Where m.Piva = Piva AndAlso m.Sa_Cod = Sa_Cod
              Select New Fabbricato With {
                  .Piva = m.Piva,
                  .Sa_Cod = m.Sa_Cod,
                  .Fabbricato_Des = m.Ubic_Des,
                  .Fabbricato_Cod = m.Id_Destinazione,
                  .Tipo_Destinazione = m.Tipo_Destinazione
              }
          ).Distinct().OrderBy(Function(f) f.Fabbricato_Des).ToList()

        Return rval

    End Function


End Class

Public Class Magazzini_W

    Public Sub Scrivi(dbContext As GiasDbContext, magazzino As APP_Magazzini, commit As Boolean)

        dbContext.APP_Magazzini.Add(magazzino)

        If commit Then
            dbContext.SaveChanges()
        End If

    End Sub

    Public Sub Cancella(dbContext As GiasDbContext, magazzino As APP_Magazzini, piva As String)

        If magazzino Is Nothing Then
            dbContext.Database.ExecuteSqlCommandAsync("DELETE FROM [APP_Magazzini] WHERE piva={0}", piva)
        Else
            dbContext.APP_Magazzini.Remove(magazzino)
            dbContext.SaveChanges()
        End If

    End Sub

End Class