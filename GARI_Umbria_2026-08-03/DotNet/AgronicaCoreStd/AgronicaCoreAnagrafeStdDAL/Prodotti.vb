Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports Microsoft.EntityFrameworkCore

Public Class Prodotti_R

    Public Function Leggi(dbContext As GiasDbContext, Piva As String, Elem_Cod As Integer) As List(Of APP_Prodotti)

        Return dbContext.APP_Prodotti.Where(
            Function(item) (
                item.Piva Is Nothing OrElse item.Piva = "" OrElse item.Piva = Piva) _
                AndAlso (Elem_Cod = 0 OrElse item.Elem_Cod = Elem_Cod)
        ).ToList()

    End Function


    Public Function Leggi(dbContext As GiasDbContext, Piva As String, Elem_Cod As Integer, prodotto_Cod As Integer) As AgronicaCoreModelloSTD.Prodotto

        Dim rval As AgronicaCoreModelloSTD.Prodotto = (
            From appPro In dbContext.APP_Prodotti
            Where ((String.IsNullOrEmpty(appPro.Piva) AndAlso appPro.Prodotto_Cod > 0) OrElse appPro.Piva = Piva) AndAlso
                appPro.Elem_Cod = Elem_Cod AndAlso
                appPro.Prodotto_Cod = prodotto_Cod
            Select New AgronicaCoreModelloSTD.Prodotto With {
                .Elem_Cod = appPro.Elem_Cod,
                .Prodotto_Cod = appPro.Prodotto_Cod,
                .Prodotto_Des = appPro.Prodotto_Des
                }).FirstOrDefault

        Return rval

    End Function

    Public Function GiacenzeProdottiLeggi(dbContext As GiasDbContext, piva As String, sa_Cod As Integer, Fabbricato_Cod As Integer, listaElemCodFiltro As List(Of Integer), AggiungiNpkInDescrizione As Boolean, GiacenzaPositiva As Boolean) As List(Of AgronicaCoreModelloSTD.RilevamentoDiMagazzino)

        Dim rval As List(Of AgronicaCoreModelloSTD.RilevamentoDiMagazzino) =
                      (From p In dbContext.APP_Prodotti
                       Join gg In dbContext.APP_Prodotti_Giacenze
                               On p.Piva Equals gg.Piva _
                               And p.Elem_Cod Equals gg.Elem_Cod _
                               And p.Prodotto_Cod Equals gg.Prodotto_Cod
                       Join udm1 In dbContext.APP_CategorieXUnitaMisura
                               On udm1.Udm_Cod Equals gg.Udm_Cod
                       Where gg.Sa_Cod = sa_Cod AndAlso
                             gg.Piva = piva AndAlso
                             (Fabbricato_Cod = 0 OrElse gg.Fabbricato_Cod = Fabbricato_Cod) AndAlso
                             listaElemCodFiltro.Contains(p.Elem_Cod) AndAlso
                             (Not GiacenzaPositiva OrElse gg.Giacenza > 0)
                       Select New AgronicaCoreModelloSTD.RilevamentoDiMagazzino With {
                          .Prodotto = New AgronicaCoreModelloSTD.Prodotto With {
                              .Elem_Cod = p.Elem_Cod,
                              .Prodotto_Cod = p.Prodotto_Cod,
                              .Prodotto_Des = If(AggiungiNpkInDescrizione, p.Prodotto_Des & " (N:" & DesNum(p.N) & " P:" & DesNum(p.P2O5) & " K:" & DesNum(p.K2O) & " CU:" & DesNum(p.Cu) & ") [" & gg.Fabbricato_Des & " (" & DesNum(gg.Giacenza) & " " & udm1.Udm_des & ")]", p.Prodotto_Des & "[" & gg.Fabbricato_Des & " " & gg.Lotto & " (" & DesNum(gg.Giacenza) & " " & udm1.Udm_des & ")]"),
                              .Udm = New AgronicaCoreModelloSTD.UnitaMisura With {
                                  .udm_cod = gg.Udm_Cod
                                  }
                       },
                       .Descrizione = If(AggiungiNpkInDescrizione, p.Prodotto_Des & " (N:" & DesNum(p.N) & " P:" & DesNum(p.P2O5) & " K:" & DesNum(p.K2O) & " CU:" & DesNum(p.Cu) & ") [" & gg.Fabbricato_Des & " (" & DesNum(gg.Giacenza) & " " & udm1.Udm_des & ")]", p.Prodotto_Des & "[" & gg.Fabbricato_Des & " " & gg.Lotto & " (" & DesNum(gg.Giacenza) & " " & udm1.Udm_des & ")]"),
                       .N = p.N,
                       .P2O5 = p.P2O5,
                       .K2O = p.K2O,
                       .Cu = p.Cu,
                       .Magazzino = New AgronicaCoreModelloSTD.Fabbricato With {
                           .Piva = gg.Piva,
                           .Sa_Cod = gg.Sa_Cod,
                           .Fabbricato_Cod = gg.Fabbricato_Cod,
                           .Fabbricato_Des = gg.Fabbricato_Des,
                           .Tipo_Destinazione = gg.Tipo_Destinazione
                           },
                       .Lotto = gg.Lotto,
                       .Cal_Cod = gg.Cal_Cod,
                       .Cod_Progetto = gg.Cod_Progetto,
                       .Qta = gg.Giacenza,
                       .TipoRilevamento = AgronicaCoreDataProviderSTD.TipiEnumerativi.enum_RilevamentoMagazzinoTipo.giacenza,
                       .udm = New AgronicaCoreModelloSTD.UnitaMisura With {
                       .udm_cod = gg.Udm_Cod
                      }
                  }).ToList()

        Return rval
    End Function


    Private Function DesNum(ByVal d As Decimal?) As String

        Return AgronicaCoreUtilityStd.Numeri.ArrotondamentoStringaOut(d)

    End Function

    Public Function GiacenzeProdottiLeggi(dbContext As GiasDbContext) As List(Of AgronicaCoreModelloSTD.RilevamentoDiMagazzino)

    End Function

    Public Function GiacenzeProdottiLeggi(dbContext As GiasDbContext, Elem_cod As Integer, Prodotto_Cod As Integer) As List(Of AgronicaCoreModelloSTD.RilevamentoDiMagazzino)

        Dim rval As List(Of AgronicaCoreModelloSTD.RilevamentoDiMagazzino) =
            (From p In dbContext.APP_Prodotti
             Join gg In dbContext.APP_Prodotti_Giacenze
                     On p.Elem_Cod Equals gg.Elem_Cod _
                     And p.Prodotto_Cod Equals gg.Prodotto_Cod
             Where p.Elem_Cod = Elem_cod AndAlso
                   p.Prodotto_Cod = Prodotto_Cod
             Select New AgronicaCoreModelloSTD.RilevamentoDiMagazzino With {
                .Prodotto = New AgronicaCoreModelloSTD.Prodotto With {
                    .Elem_Cod = p.Elem_Cod,
                    .Prodotto_Cod = p.Prodotto_Cod
             },
            .Magazzino = New AgronicaCoreModelloSTD.Fabbricato With {
                 .Piva = gg.Piva,
                 .Sa_Cod = gg.Sa_Cod,
                 .Fabbricato_Des = gg.Fabbricato_Des,
                 .Tipo_Destinazione = gg.Tipo_Destinazione
                 },
            .udm = New AgronicaCoreModelloSTD.UnitaMisura With {
            .udm_cod = gg.Udm_Cod
            },
                 .Qta = gg.Giacenza,
                 .TipoRilevamento = AgronicaCoreDataProviderSTD.TipiEnumerativi.enum_RilevamentoMagazzinoTipo.giacenza
        }).ToList()

        Return rval

    End Function

    Public Function LeggiProdotti(dbContext As GiasDbContext, piva As String, Elem_Cod As List(Of Integer)) As List(Of APP_Prodotti)

        Dim rval As List(Of APP_Prodotti) = (
          From i In dbContext.APP_Prodotti
          Where Elem_Cod.Contains(i.Elem_Cod) AndAlso
                ((String.IsNullOrEmpty(i.Piva) AndAlso i.Prodotto_Cod > 0) OrElse i.Piva = piva)
          ).Distinct().OrderBy(Function(f) f.Prodotto_Des).ToList()

        Return rval

    End Function

    Public Function LeggiProdottiPerOperazione(Prodotti As List(Of APP_Prodotti), AggiungiNpkInDescrizione As Boolean) As List(Of AgronicaCoreModelloSTD.RilevamentoDiMagazzino)

        Dim listaProdotti = (From o In Prodotti
                             Select New AgronicaCoreModelloSTD.RilevamentoDiMagazzino With {
                                .Prodotto = New AgronicaCoreModelloSTD.Prodotto With {
                                    .Elem_Cod = o.Elem_Cod,
                                    .Prodotto_Cod = o.Prodotto_Cod,
                                    .Prodotto_Des = If(AggiungiNpkInDescrizione, o.Prodotto_Des & " (N:" & DesNum(o.N) & " P:" & DesNum(o.P2O5) & " K:" & DesNum(o.K2O) & " CU:" & DesNum(o.Cu) & ")", o.Prodotto_Des),
                                    .Udm = New AgronicaCoreModelloSTD.UnitaMisura With {.udm_cod = o.Udm_Cod}
                                    },
                                .udm = New AgronicaCoreModelloSTD.UnitaMisura With {.udm_cod = o.Udm_Cod},
                                .Descrizione = If(AggiungiNpkInDescrizione, o.Prodotto_Des & " (N:" & DesNum(o.N) & " P:" & DesNum(o.P2O5) & " K:" & DesNum(o.K2O) & " CU:" & DesNum(o.Cu) & ")", o.Prodotto_Des),
                                .N = o.N,
                                .P2O5 = o.P2O5,
                                .K2O = o.K2O,
                                .Cu = o.Cu
                        }).ToList()

        Return listaProdotti

    End Function

End Class


Public Class Prodotti_W

    Public Sub Scrivi(dbContext As GiasDbContext, prodotto As APP_Prodotti, commit As Boolean)

        dbContext.APP_Prodotti.Add(prodotto)

        If commit Then
            dbContext.SaveChanges()
        End If

    End Sub

    Public Sub Cancella(dbContext As GiasDbContext, prodotto As APP_Prodotti, piva As String)

        If prodotto Is Nothing Then
            If String.IsNullOrEmpty(piva) Then
                dbContext.Database.ExecuteSqlCommandAsync("DELETE FROM [APP_Prodotti] WHERE Piva IS NULL")
            Else
                dbContext.Database.ExecuteSqlCommandAsync("DELETE FROM [APP_Prodotti] WHERE Piva={0}", piva)
            End If
        Else
            dbContext.APP_Prodotti.Remove(prodotto)
            dbContext.SaveChanges()
        End If

    End Sub

End Class


