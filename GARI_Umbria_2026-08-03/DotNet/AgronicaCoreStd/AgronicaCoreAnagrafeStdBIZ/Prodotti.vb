
Imports AgronicaCoreDataProviderSTD.TipiEnumerativi
Imports AgronicaCoreDataProviderSTD.CostantiPersonalizzate
Imports AgronicaCoreAnagrafeStdDAL
Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports AgronicaCoreModelloSTD

Public Class Prodotti
    Inherits BaseBiz

    Public Sub New(dbContext As GiasDbContext)
        MyBase.New(dbContext)
    End Sub

    Public Function LeggiProdotti(Piva As String, Elem_Cod As Integer) As List(Of APP_Prodotti)

        Dim xLettura = New Prodotti_R()
        Return xLettura.Leggi(dbContext, Piva, Elem_Cod)

    End Function


    ''' <summary>
    ''' Dato il tipo di Operazione e la data di riferimento restituisce lista di prodotti
    ''' </summary>
    ''' <param name="Elem_cod"></param>
    ''' <param name="Prodotto_Cod"></param>
    ''' <returns></returns>
    Public Function GiacenzeProdottiLeggi(Elem_cod As Integer, Prodotto_Cod As Integer) As List(Of AgronicaCoreModelloSTD.RilevamentoDiMagazzino)

        Dim leggi As New Prodotti_R()
        Return leggi.GiacenzeProdottiLeggi(dbContext, Elem_cod, Prodotto_Cod)

    End Function

    ''' <summary>
    ''' Dato il tipo di Operazione e la data di riferimento restituisce lista di prodotti
    ''' </summary>
    ''' <param name="Lav_Cod"></param>
    ''' <param name="piva"></param>
    ''' <param name="sa_cod"></param>
    ''' <param name="DataRiferimento"></param>
    ''' <param name="Magazzino"></param>
    ''' <param name="ProdottiMagazzino"></param>
    ''' <returns></returns>
    Public Function LeggiProdottiPerOperazione(Lav_Cod As Integer, piva As String, sa_cod As Integer, DataRiferimento As DateTime, Magazzino As Integer, FiltroProdottiMagazzino As String) As List(Of AgronicaCoreModelloSTD.RilevamentoDiMagazzino)

        Dim xLettura = New Prodotti_R()

        Dim ListaElemCod As List(Of Integer) = CategorieMagazzino.LeggiCategorieMagazzinoDatoLavCod(Lav_Cod)
        Dim ListaElemCodProdotti As New List(Of Integer)
        Dim ListaElemCodMagazzino As New List(Of Integer)
        Dim ListaElemCodGiacenza As New List(Of Integer)

        ' imposta categorie per filtro su prodotti
        If Not String.IsNullOrEmpty(FiltroProdottiMagazzino) Then
            Dim filtro_categorie = FiltroProdottiMagazzino.Split(CChar("|"))
            For Each filtro In filtro_categorie
                Dim items As String() = filtro.Split(CChar("_"))
                If items.Length > 1 AndAlso ListaElemCod.Contains(CInt(items(0))) Then
                    If CInt(items(1)) = 0 Then
                        ListaElemCodProdotti.Add(CInt(items(0)))
                        ListaElemCodMagazzino.Add(CInt(items(0)))
                    ElseIf CInt(items(1)) = 1 Then
                        ListaElemCodMagazzino.Add(CInt(items(0)))
                    ElseIf CInt(items(1)) = 2 Then
                        ListaElemCodGiacenza.Add(CInt(items(0)))
                    End If
                End If
            Next
        End If

        Dim AggiungiNpkInDescrizione As Boolean = (ListaElemCod.Contains(enum_CategorieMagazzino.FERTILIZZANTI))

        Dim listaProdotti As New List(Of AgronicaCoreModelloSTD.RilevamentoDiMagazzino)
        Dim listaProdottiMagazzino As New List(Of AgronicaCoreModelloSTD.RilevamentoDiMagazzino)
        Dim listaProdottiGiacenza As New List(Of AgronicaCoreModelloSTD.RilevamentoDiMagazzino)

        ' lista prodotti in anagrafica
        If ListaElemCodProdotti.Count > 0 Then
            Dim prodotti As List(Of APP_Prodotti) = xLettura.LeggiProdotti(dbContext, piva, ListaElemCodProdotti)
            listaProdotti = xLettura.LeggiProdottiPerOperazione(prodotti, AggiungiNpkInDescrizione)
        End If

        ' lista prodotti movimentati
        If ListaElemCodMagazzino.Count > 0 Then
            listaProdottiMagazzino = xLettura.GiacenzeProdottiLeggi(dbContext, piva, sa_cod, Magazzino, ListaElemCodMagazzino, AggiungiNpkInDescrizione, False)

        End If

        ' lista prodotti con giacenza positiva
        If ListaElemCodGiacenza.Count > 0 Then
            listaProdottiGiacenza = xLettura.GiacenzeProdottiLeggi(dbContext, piva, sa_cod, Magazzino, ListaElemCodGiacenza, AggiungiNpkInDescrizione, True)

        End If

        ' rimuovo dalla lista prodotti quelli a magazzino o in giacenza (per evitare ambiguità)
        If listaProdotti.Count > 0 Then
            If listaProdottiMagazzino.Count > 0 Then
                Dim prodottiMagazzino As List(Of Integer) = (From p In listaProdottiMagazzino Select p.Prodotto.Prodotto_Cod).ToList
                listaProdotti = (From p In listaProdotti Where Not prodottiMagazzino.Contains(p.Prodotto.Prodotto_Cod) Select p).ToList()
            End If
            If listaProdottiGiacenza.Count > 0 Then
                Dim prodottiGiacenza As List(Of Integer) = (From p In listaProdottiGiacenza Select p.Prodotto.Prodotto_Cod).ToList
                listaProdotti = (From p In listaProdotti Where Not prodottiGiacenza.Contains(p.Prodotto.Prodotto_Cod) Select p).ToList()
            End If
        End If

        Return listaProdotti.Concat(listaProdottiMagazzino).Concat(listaProdottiGiacenza).OrderBy(Function(prod) prod.Descrizione).ToList()

    End Function

    Public Function ScriviProdottoOperazione(piva As String, rdm As RilevamentoDiMagazzino) As Boolean

        Dim xLettura As New Prodotti_R()
        If xLettura.Leggi(dbContext, piva, rdm.Prodotto.Elem_Cod, rdm.Prodotto.Prodotto_Cod) Is Nothing Then
            Dim xScrittura As New Prodotti_W()
            Dim prodotto As New APP_Prodotti With {
                .Piva = If(rdm.Prodotto.Prodotto_Cod > 0, "", piva),
                .Elem_Cod = rdm.Prodotto.Elem_Cod,
                .Prodotto_Cod = rdm.Prodotto.Prodotto_Cod,
                .Prodotto_Des = rdm.Prodotto.Prodotto_Des,
                .Udm_Cod = rdm.Prodotto.Udm.udm_cod,
                .N = rdm.N,
                .K2O = rdm.K2O,
                .P2O5 = rdm.P2O5,
                .Cu = rdm.Cu
            }
            xScrittura.Scrivi(dbContext, prodotto, True)
            Return True
        End If

        Return False

    End Function

    Public Sub ScriviProdotti(listProdotti As List(Of APP_Prodotti), piva As String, cancella As Boolean)

        Dim count As Integer = 0
        Dim commit As Boolean = False
        Dim numProdotti As Integer = listProdotti.Count
        Dim commitCount As Integer = 100
        Dim xScrittura = New Prodotti_W()

        If cancella Then
            xScrittura.Cancella(dbContext, Nothing, piva)
        End If

        For Each prodotto In listProdotti

            ' forzo partita iva
            prodotto.Piva = piva

            count += 1
            commit = count Mod commitCount = 0 OrElse count = numProdotti
            xScrittura.Scrivi(dbContext, prodotto, commit)
        Next

    End Sub

    Public Sub CancellaProdotti(piva As String)

        Dim xScrittura = New Prodotti_W()
        xScrittura.Cancella(dbContext, Nothing, piva)

    End Sub

    Public Sub CancellaProdotto(prodotto As APP_Prodotti)

        Dim xScrittura = New Prodotti_W()
        xScrittura.Cancella(dbContext, prodotto, Nothing)

    End Sub

End Class
