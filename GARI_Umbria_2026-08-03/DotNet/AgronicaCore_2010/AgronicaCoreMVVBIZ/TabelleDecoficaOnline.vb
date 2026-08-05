Imports AgronicaCoreDataProvider
Imports AgronicaCoreMVVCommon
Imports AgronicaCoreMVVDal

Public Class TabelleDecoficaOnline

    Private ReadOnly _objParametriServer As AgronicaCoreParametri

    Private ReadOnly _biologico As New Dictionary(Of String, String) From
        {
            {"1", "Biologico"},
            {"2", "Biologico in Conversione"}
        }

    Private ReadOnly _unitaMisura As New Dictionary(Of String, String) From
       {
           {"l", "litri"},
           {"hl", "ettolitri"},
           {"kg", "kilogrammi"},
           {"ton", "tonnellate"}
       }

    Private ReadOnly _statiFisici As New Dictionary(Of String, String) From
        {
            {"1", "Sfuso"},
            {"2", "Imbottigliato/confezionato"},
            {"3", "Imbottigliato senza etichetta"}
        }

    Private ReadOnly _tenoreZuccheri As New Dictionary(Of String, String) From
        {
            {"1", "Spumanti - dosaggio zero"},
            {"2", "Spumanti - extra brut"},
            {"3", "Spumanti - brut"},
            {"4", "Spumanti - extra dry"},
            {"5", "Spumanti - secco, asciutto, dry"},
            {"6", "Spumanti - demi-sec, abboccato"},
            {"7", "Spumanti - dolce"},
            {"11", "Vini - secco, asciutto"},
            {"12", "Vini - demi-sec, abboccato "},
            {"13", "Vini - amabile"},
            {"14", "Vini - dolce"},
            {"21", "Vini Frizzanti - secco"},
            {"22", "Vini Frizzanti - semisecco - abboccato"},
            {"23", "Vini Frizzanti - ambile"},
            {"24", "Vini Frizzanti - dolce"},
            {"31", "Vini Liquorosi - secco"},
            {"32", "Vini Liquorosi - semisecco - amabile"},
            {"33", "Vini Liquorosi - dolce"}
        }

    Private ReadOnly _statiMembri As IEnumerable(Of Object)
    Private ReadOnly _tuttiGliStati As IEnumerable(Of Object)

    Private ReadOnly _unitaTrasporto As IEnumerable(Of Object)

    Private ReadOnly _elencoTipiProdotto As New List(Of TipoProdottoSianMatrice) From
        {
            New TipoProdottoSianMatrice With {.Codice = "01", .Descrizione = "Uve",
                                    .CodiceGenerazione = New List(Of Integer) From {50, 326, 131, 165},
                                    .ElencoCategorie = New List(Of Integer) From {0},
                                    .ElencoClassificazione = New List(Of String) From {"A, B, C, D, E, G, I, L"}
                                    },
            New TipoProdottoSianMatrice With {
                                    .Codice = "02",
                                    .Descrizione = "Sfuso DOC/IGP",
                                    .CodiceGenerazione = New List(Of Integer) From {55, 433, 780, 54, 700, 702, 720, 716, 699, 781, 719, 718},
                                    .ElencoCategorie = New List(Of Integer) From {19, 20, 21, 731, 732, 734, 735},
                                    .ElencoClassificazione = New List(Of String) From {"A, B, C, D"}
                                    },
            New TipoProdottoSianMatrice With {
                                    .Codice = "03",
                                    .Descrizione = "Sfuso generico/varietale",
                                    .CodiceGenerazione = New List(Of Integer) From {55, 433, 720, 716, 699, 719, 718, 781},
                                    .ElencoCategorie = New List(Of Integer) From {163, 22, 733, 736, 737},
                                    .ElencoClassificazione = New List(Of String) From {"E, F, G, H, I, L"}
                                    },
            New TipoProdottoSianMatrice With {
                                    .Codice = "04",
                                    .Descrizione = "Imbottigliato DOC/IGP",
                                    .CodiceGenerazione = New List(Of Integer) From {770, 728, 303, 113, 86, 87, 352, 88, 723, 695, 124, 348, 437, 614, 345, 126, 127, 128, 129, 130, 338, 339, 340, 341, 342, 343, 729, 331, 332, 333, 334, 335, 336, 480, 638},
                                    .ElencoCategorie = New List(Of Integer) From {19, 20, 21, 731, 732, 734, 735},
                                    .ElencoClassificazione = New List(Of String) From {"A,B, C, D, R, S"}
                                    },
            New TipoProdottoSianMatrice With {
                                    .Codice = "05",
                                    .Descrizione = "Imbottigliato generico/varietale",
                                    .CodiceGenerazione = New List(Of Integer) From {770, 728, 303, 113, 86, 87, 352, 88, 723, 695, 124, 348, 437, 614, 29, 331, 332, 333, 334, 335, 336, 480, 638, 729},
                                    .ElencoCategorie = New List(Of Integer) From {163, 22, 733, 736, 737},
                                    .ElencoClassificazione = New List(Of String) From {"E, F, G, H, I, L, Q"}
                                    },
            New TipoProdottoSianMatrice With {
                                    .Codice = "06",
                                    .Descrizione = "Altri Prodotti",
                                    .CodiceGenerazione = New List(Of Integer) From {51, 677, 722, 775, 721, 778, 776, 52, 53, 434, 427, 99, 766, 767, 768, 98, 100},
                                    .ElencoCategorie = New List(Of Integer) From {0},
                                    .ElencoClassificazione = New List(Of String) From {"A, B, C, D, E, G, I, L"}
                                    }
        }

    Private ReadOnly _praticheEno As New Dictionary(Of String, String) From
        {
            {"00", "il prodotto non è stato oggetto di pratiche enologiche"},
            {"01", "il prodotto è stato arricchito"},
            {"02", "il prodotto è stato acidificato"},
            {"03", "il prodotto è stato disacidificato"},
            {"04", "il prodotto è stato dolcificato"},
            {"05", "il prodotto è stato oggetto di un'aggiunta di alcole"},
            {"06", "al prodotto è stato aggiunto un prodotto originario di un'unità geografica diversa da quella indicata nella designazione"},
            {"07", "al prodotto è stato aggiunto un prodotto proveniente da una varietà di vite diversa da quella indicata nella designazione"},
            {"08", "al prodotto è stato aggiunto un prodotto raccolto nel corso di un anno diverso da quello indicato nella designazione"},
            {"09", "il prodotto è stato elaborato utilizzando pezzi di legno di quercia"},
            {"10", "il prodotto è stato elaborato con l'impiego sperimentale di una nuova pratica enologica"},
            {"11", "il tenore alcolico del prodotto è stato corretto"},
            {"12", "trattamento con ferrocianuro"},
            {"GG", "Denaturazione con cloruro di sodio"},
            {"HH", "Denaturazione con litiocloruro"},
            {"II", "Denaturazione Fecce per uso agronomico con Solfato ferroso"},
            {"LL", "Invecchiamento In legno"},
            {"MM", "Diluizione del vino per la produzione di aceto"},
            {"NN", "Diluizione dell'aceto"},
            {"OO", "Decolorazione dell'aceto"},
            {"PP", "Aromatizzazione dell'aceto "},
            {"QQ", "Trattamento Materie Prime per la Distillazione"},
            {"SB", "Sboccatura Vini Spumanti"}
        }

    Private ReadOnly _causaliTRasporto As New Dictionary(Of String, String) From
        {
            {"Vendita/Invio a Conto lavoro".ToLower, "nn"},
            {"Vendita".ToLower, "aa"},
            {"Conto Lavorazione".ToLower, "bb"},
            {"Reso Conto Lavoro".ToLower, "cc"},
            {"Reso Conto Lavoro-Vendita".ToLower, "dd"},
            {"Reso Conto Lavoro - invio a Conto lavoro".ToLower, "ee"},
            {"Trasferimento tra depositi aziendali".ToLower, "ff"},
            {"Conferimento".ToLower, "gg"},
            {"Campionatura".ToLower, "hh"},
            {"Omaggio".ToLower, "ii"},
            {"Trasferimento prodotti denaturati o non conformi".ToLower, "ll"},
            {"Vendita a soggetto senza codice ICQRF".ToLower, "mm"}
        }


    Sub New(ByVal objParametriServer As AgronicaCoreParametri)
        _objParametriServer = objParametriServer
        Dim dal = New MVVElettronico_R(objParametriServer)

        Dim dt As DataTable = dal.CaricaStatiMembri()
        If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then

            _statiMembri = (From s In dt.AsEnumerable() Select New With
                            {
                                .Codice = If(s.Item("Codice") Is DBNull.Value, "", s.Item("Codice")),
                                .Descrizione = If(s.Item("Descrizione") Is DBNull.Value, "", s.Item("Descrizione")),
                                .Codice_NUmerico = If(s.Item("Codice_NUmerico") Is DBNull.Value, "", s.Item("Codice_NUmerico"))
                            }).ToList()

        End If

        dt = dal.CaricaPaesiTerzi()
        If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then

            _tuttiGliStati = (From s In dt.AsEnumerable() Select New With
                            {
                                .Codice = If(s.Item("Codice") Is DBNull.Value, "", s.Item("Codice")),
                                .Descrizione = If(s.Item("Descrizione") Is DBNull.Value, "", s.Item("Descrizione")),
                                .Codice_NUmerico = If(s.Item("Codice_NUmerico") Is DBNull.Value, "", s.Item("Codice_NUmerico"))
                            }).ToList()

        End If

        dt = dal.CaricaUnitaTrasporto()
        If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then

            _unitaTrasporto = (From s In dt.AsEnumerable() Select New With
                            {
                                .Codice = If(s.Item("Codice") Is DBNull.Value, "", s.Item("Codice")),
                                .Descrizione = If(s.Item("Descrizione") Is DBNull.Value, "", s.Item("Descrizione")),
                                .Sigla = If(s.Item("Sigla") Is DBNull.Value, "", s.Item("Sigla"))
                            }).ToList()

        End If

        dt = dal.CaricaMatriceProdottiSian
        If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then

            _elencoTipiProdotto.Clear()

            For Each s As DataRow In dt.Rows

                Dim ps = New TipoProdottoSianMatrice With
                {
                    .Codice = If(s.Item("Codice") Is DBNull.Value, "", s.Item("Codice")),
                    .Descrizione = If(s.Item("Descrizione") Is DBNull.Value, "", s.Item("Descrizione"))
                }
                If s.Item("CodiceGenerazione") Is DBNull.Value OrElse String.IsNullOrEmpty(s.Item("CodiceGenerazione")) Then
                    ps.CodiceGenerazione = New List(Of Integer) From {0}
                Else
                    Dim codiciGen = s.Item("CodiceGenerazione").ToString().Split(",").ToList()
                    ps.CodiceGenerazione = New List(Of Integer)
                    For Each c As String In codiciGen
                        ps.CodiceGenerazione.Add(CInt(c))
                    Next
                End If
                If s.Item("ElencoCategorie") Is DBNull.Value OrElse String.IsNullOrEmpty(s.Item("ElencoCategorie")) Then
                    ps.ElencoCategorie = New List(Of Integer) From {0}
                Else
                    Dim codiciCat = s.Item("ElencoCategorie").ToString().Split(",").ToList()
                    ps.ElencoCategorie = New List(Of Integer)
                    For Each c As String In codiciCat
                        ps.ElencoCategorie.Add(CInt(c))
                    Next
                End If
                If s.Item("Classificazione") Is DBNull.Value OrElse String.IsNullOrEmpty(s.Item("Classificazione")) Then
                    ps.ElencoClassificazione = New List(Of String)
                Else
                    Dim codiciClassificazione = s.Item("Classificazione").ToString().Split(",").ToList()
                    ps.ElencoClassificazione = New List(Of String)
                    For Each c As String In codiciClassificazione
                        ps.ElencoClassificazione.Add(c.Trim)
                    Next
                End If

                _elencoTipiProdotto.Add(ps)
            Next

        End If

        dt = dal.CausaliTrasportoSian
        If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then

            For Each s As DataRow In dt.Rows

                Dim valore = If(s.Item("Causale_Trasporto_Sigla") Is DBNull.Value, "", s.Item("Causale_Trasporto_Sigla"))
                Dim chiave = If(s.Item("Causale_Trasporto_Des") Is DBNull.Value, "", s.Item("Causale_Trasporto_Des")).ToString.ToLower

                If Not String.IsNullOrEmpty(chiave) AndAlso Not _causaliTRasporto.ContainsKey(chiave) Then
                    _causaliTRasporto.Add(chiave, valore)
                End If
            Next

        End If

    End Sub

    Public Function DecodificaCausaleTrasporto(ByVal descrizione As String) As String

        If _causaliTRasporto.ContainsKey(descrizione.ToLower.Trim) Then
            Return _causaliTRasporto(descrizione.ToLower.Trim)
        End If
        Return String.Empty

    End Function

    Public Function DecodificaCodiceTipoProdotto(ByVal genCod As Integer, ByVal categoriaCod As Integer, ByVal classificazioneCod As String) As String

        If genCod = 0 Then
            Return String.Empty
        End If

        Dim trovati As List(Of TipoProdottoSianMatrice) = New List(Of TipoProdottoSianMatrice)
        For Each tp As TipoProdottoSianMatrice In _elencoTipiProdotto
            If tp.ElencoCategorie.Contains(categoriaCod) AndAlso tp.CodiceGenerazione.Contains(genCod) AndAlso
                tp.ElencoClassificazione.Contains(classificazioneCod) Then
                trovati.Add(tp)
            End If
        Next

        If Not trovati.Any Then
            Return String.Empty
        End If

        Return trovati.FirstOrDefault().Codice

    End Function

    Public Function DecodificaUnitaTrasporto(ByVal codice As Integer) As String

        If codice = -1 Then
            Return String.Empty
        End If

        Dim ut = _unitaTrasporto.FirstOrDefault(Function(u) u.Codice = codice)
        If Not ut Is Nothing Then
            Return ut.Sigla
        End If

        Return String.Empty

    End Function

    Public Function DecodificaStato(ByVal stato As String) As String

        Dim sm = _tuttiGliStati.FirstOrDefault(Function(s) s.Codice.ToString.ToLower = stato.ToLower)
        If Not sm Is Nothing Then
            Return sm.Codice_NUmerico
        End If

        sm = _tuttiGliStati.FirstOrDefault(Function(s) s.Descrizione.ToString.ToLower = stato.ToLower)
        If Not sm Is Nothing Then
            Return sm.Codice_NUmerico
        End If

        Return String.Empty

    End Function

    Public Function StatoMembro(ByVal codice As String) As Boolean
        Dim sm = _statiMembri.FirstOrDefault(Function(s) s.Codice_NUmerico = codice)
        If Not sm Is Nothing Then
            Return True
        End If
        Return False

    End Function

    Public Function DecodificaBiologico(ByVal codiceBio As String) As String

        If _biologico.ContainsKey(codiceBio) Then
            Return codiceBio
        End If
        Return String.Empty

    End Function

    Public Function DecodificaPraticaEno(ByVal codice As String) As String

        If _praticheEno.ContainsKey(codice) Then
            Return codice
        End If
        Return String.Empty

    End Function

    Public Function DecodificaStatoFisico(ByVal codiceSF As String) As String

        If _statiFisici.ContainsKey(codiceSF) Then
            Return codiceSF
        End If
        Return String.Empty

    End Function

    Public Function DecodificaTenoreZucchero(ByVal codTenoreZucchero As String) As String
        If _tenoreZuccheri.ContainsKey(codTenoreZucchero) Then
            Return codTenoreZucchero
        End If
        Return String.Empty
    End Function

End Class
