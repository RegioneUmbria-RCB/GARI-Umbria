
Imports AgronicaCoreDataProvider
Imports AgronicaCoreModelliPrevisionaliBIZ
Imports AgronicaCoreVarieBIZ

Public Class TicchiolaturaMelo
    Inherits AbstractModello

    Private Class dati_modello
        Public DataOra As DateTime
        Public Infezione As Double
        Public Giorno As DateTime
        Public PercEvasione As Double
        Public GiornoEvasione As DateTime
        Public OreBagnatura As Integer
        Public MmPioggia As Single
        Public Temperatura As Single

        Public ReadOnly Property Data As DateTime
            Get
                Return DataOra.Date
            End Get
        End Property

        Public Sub New(ByVal dt As DateTime)
            DataOra = dt
            Infezione = 0
            Giorno = New Date(1, 1, 1)
            PercEvasione = 0
            GiornoEvasione = New Date(1, 1, 1)
            OreBagnatura = 0
            MmPioggia = 0
            Temperatura = 0
        End Sub
    End Class

    Private DatiModello As List(Of dati_modello)

    Private Function fInfezione(ByVal T As Double) As Double
        fInfezione = -0.058 + 0.02053 * T - 0.000558 * (T ^ 2)
    End Function
    Private Function fEvasione(ByVal T As Double) As Double
        fEvasione = 0.0003541 + 0.0002501 * T - 0.000000805 * (T ^ 2)
    End Function

    Private Function PioggiaInfettante(ByVal TipoModello As Integer, ByVal datiMeteo As MeteoReadOnlyList, ByVal current As Integer) As Boolean

        Dim d_hh As MeteoDSSItem = datiMeteo(current)

        Dim OreTotali As Integer = 0
        Dim OreBagnatura As Integer = 0
        Dim OreInterruzione As Integer = 0
        Dim Temperatura As Double = 0
        Dim Temperatura_Int As Double = 0
        Dim Pioggia As Double = 0
        Dim Pioggia_Int As Double = 0
        Dim Infez As Double = 0
        Dim Evas As Double = 0
        Dim Evas_Int As Double = 0

        'Dim Giorno As Integer
        'Dim GiornoFinale As Integer
        'Dim OraFinale As Integer
        'Dim Ora As Integer

        'Giorno = d_hh.Data.DayOfYear
        'Ora = d_hh.ora
        'GiornoFinale = Giorno
        'OraFinale = Ora

        Dim Termina As Boolean = False
        ' Continua ad esaminare le ore di bagnatura finchè non raggiungo la data finale
        ' oppura finchè l'infezione non supera la soglia 1

        Dim idx As Integer = current

        'Se non uso il modello di Mills devo modificare l'ora di inizio
        If TipoModello > 1 Then
            Select Case d_hh.DataOra.Hour
                Case 20 To 23
                    'vado alle 7 del giorno dopo
                    Dim domani As Integer = d_hh.DataOra.DayOfYear() + 1
                    idx += 1
                    While idx < datiMeteo.Count AndAlso datiMeteo(idx).DataOra.DayOfYear <= domani AndAlso datiMeteo(idx).DataOra.Hour < 7
                        idx += 1
                    End While
                Case 0 To 6
                    'vado alle 7 del giorno
                    idx += 1
                    While idx < datiMeteo.Count AndAlso datiMeteo(idx).DataOra.Hour < 7
                        idx += 1
                    End While
            End Select
        End If

        'Cerca il buco di bagnatura di almeno 4 ore consecutive
        While idx <= datiMeteo.Count And (Not Termina)

            If datiMeteo(idx).Bagn <> 0 Then

                OreTotali += 1
                OreBagnatura += 1
                Infez += fInfezione(datiMeteo(idx).Temp)
                Temperatura += datiMeteo(idx).Temp
                Pioggia += datiMeteo(idx).Prec
                Evas += fEvasione(datiMeteo(idx).Temp)

                If OreInterruzione <> 0 Then

                    OreBagnatura += OreInterruzione
                    OreTotali += OreInterruzione
                    Temperatura += Temperatura_Int
                    Pioggia += Pioggia_Int
                    Evas += Evas_Int

                End If

                Temperatura_Int = 0
                Pioggia_Int = 0
                Evas_Int = 0
                OreInterruzione = 0
                'GiornoFinale = Giorno
                'OraFinale = Ora

            Else

                OreInterruzione += 1
                Temperatura_Int += datiMeteo(idx).Temp
                Pioggia_Int += datiMeteo(idx).Prec
                Evas_Int += fEvasione(datiMeteo(idx).Temp)

                'se ci sono state 1 o 2 ore di interruzione consecutive procedo con il calcolo
                If OreInterruzione < 3 Then

                    Infez += fInfezione(datiMeteo(idx).Temp)

                ElseIf OreInterruzione = 4 Then

                    'se ci sono state 4 ore di interruzione consecutive fermo il calcolo
                    Termina = True
                    OreInterruzione = 0

                End If

            End If

            idx += 1

        End While

        If OreInterruzione <> 0 Then

            OreBagnatura += OreInterruzione
            OreTotali += OreInterruzione
            Temperatura += Temperatura_Int
            Pioggia += Pioggia_Int
            Evas += Evas_Int

        End If

        Dim d_m As dati_modello = Nothing

        If Infez >= 1.0# Then

            d_m = New dati_modello(d_hh.DataOra)

            d_m.Infezione = Infez
            '    If Giorno <= Fine Then
            '        d_m.Giorno = GiornoFinale
            '    Else
            '        d_m.Giorno = Fine
            '    End If
            d_m.OreBagnatura = OreBagnatura
            d_m.MmPioggia = Pioggia
            d_m.Temperatura = Temperatura / OreTotali
            'Cerco la data di presunta evasione
            'Do While Evas < 1 And GiornoFinale <= Fine
            '    Evas += fEvasione(Meteo.Orari(GiornoFinale, OraFinale).Temp)
            '    IncrementaGiorno(GiornoFinale, OraFinale)
            'Loop
            d_m.PercEvasione = Math.Min(1, Evas)
            'd_m.GiornoEvasione = GiornoFinale

            'ElseIf GiornoFinale >= Fine Then
            '    'Memorizzo i valori di infezione nel caso sia giunto al limite dei dati meteo

            '    d_m = New dati_modello(d_hh.Data_Agg.AddHours(d_hh.ora))

            '    d_m.Infezione = Infez
            '    d_m.Giorno = Fine
            '    d_m.OreBagnatura = OreBagnatura
            '    d_m.MmPioggia = SommaPioggia
            '    d_m.Temperatura = Temperatura / OreTotali
            '    d_m.PercEvasione = 0#
            '    d_m.GiornoEvasione = GiornoFinale

        End If

        If d_m IsNot Nothing Then
            DatiModello.Add(d_m)
            Return True
        End If

        Return False
    End Function

    Private _alg_cod As Integer
    Private _mmPioggia As Decimal

    Private Function LeggiParametro(DT As DataTable, Parametro As String, valDefault As String) As String
        Dim rval As String = (From r In DT.AsEnumerable
                              Where r("ParametroNome") = Parametro
                              Select CStr(r("ParametroValore"))).FirstOrDefault

        If Not String.IsNullOrEmpty(rval) Then
            Return rval
        Else
            Return valDefault
        End If
    End Function

    Public Sub New(ByVal datiMeteo As MeteoReadOnlyList, ByVal Mod_Cod As Integer, ByVal Alg_Cod As Integer, ByVal objParametri_server As AgronicaCoreParametri)
        MyBase.New(datiMeteo)

        DatiModello = New List(Of dati_modello)

        _alg_cod = Alg_Cod
        Dim Av_Cod As Integer = 63
        Dim Veg_Cod As Integer = 40

        Dim xLeggi As New AgronicaCoreModelliPrevisionaliDAL.ParametriModelli_R
        Dim dt As DataTable = xLeggi.Leggi(Av_Cod, Veg_Cod, Mod_Cod, "", "", objParametri_server)

        If dt.Rows.Count = 0 Then
            'R.Errore &= "Errore: non trovo i parametri di calcolo, uso i valori di default"
            _mmPioggia = 0.2
        Else
            _mmPioggia = LeggiParametro(dt, "MmPioggia", 0.2)
        End If
    End Sub

    Protected Overrides Function _elaboraModello() As cRisultatoModello

        Dim risModello As cRisultatoModello = Nothing

        Try

            Dim hh As Integer = 0
            Dim d_hh As MeteoDSSItem

            While hh < _datiMeteo.Count

                d_hh = _datiMeteo(hh)
                If d_hh.Prec >= _mmPioggia Then

                    If PioggiaInfettante(_alg_cod, _datiMeteo, hh) Then

                        'vado al giorno dopo
                        Dim this_doy As Integer = d_hh.DataOra.DayOfYear()
                        hh += 1
                        While hh < _datiMeteo.Count AndAlso _datiMeteo(hh).DataOra.DayOfYear = this_doy
                            hh += 1
                        End While

                    Else
                        hh += 1
                    End If
                Else
                    hh += 1
                End If
            End While

            Dim output As New OutputModello

            output.aggiungiColonna("InizioPioggia", GetType(Date), "Inizio pioggia", "dd/MM/yyyy")
            output.aggiungiColonna("mmPioggia", GetType(Decimal), "Pioggia (mm)", "0.0")
            output.aggiungiColonna("OreBagnatura", GetType(Decimal), "Ore bagnatura", "0")
            output.aggiungiColonna("Temperatura", GetType(Decimal), "Temperatura", "0.00")
            output.aggiungiColonna("Infezione", "Infezione")
            output.aggiungiColonna("DataInfezione", GetType(Date), "Data infezione", "dd/MM/yyyy")
            output.aggiungiColonna("Evasione", "Evasione")
            'output.aggiungiColonna("PercEvasione", GetType(Decimal), "Evasione %", "0.00\%")
            'output.aggiungiColonna("GiornoEvasione", GetType(Date), "Giorno evasione", "dd/MM/yyyy")

            For Each d_m In DatiModello

                output.AddField(d_m.Data)
                output.AddField(d_m.MmPioggia)
                output.AddField(d_m.OreBagnatura)
                output.AddField(d_m.Temperatura)

                If d_m.Infezione > 1.8 Then
                    output.AddField("Grave")
                ElseIf d_m.Infezione > 1.4 Then
                    output.AddField("Media")
                ElseIf d_m.Infezione > 1 Then
                    output.AddField("Leggera")
                Else
                    output.AddField(Format(d_m.Infezione, "0"))
                End If

                'Dim lGiornoInfezione As DateTime = GiulianoToDate(tmpEvasione.Giorno, Year(DataInizio))
                'Dr("DataInfezione") = lGiornoInfezione
                output.AddField(d_m.Giorno.Date)

                If d_m.PercEvasione < 1 Then
                    output.AddField(Format(d_m.PercEvasione, "0%"))
                Else
                    output.AddField(d_m.GiornoEvasione.ToShortDateString)
                End If

                'output.AddField(d_m.PercEvasione * 100)
                'output.AddField(d_m.GiornoEvasione)

                output.Commit()
            Next

            risModello = New cRisultatoModello With {
                .Modello_Tabella1 = output.Output()
            }

        Catch ex As Exception

            risModello = Nothing

            'uso questa funzione per ottenere il Messaggio..:
            _errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return risModello
    End Function

    Protected Overrides Function _elaboraIndicatore_vecchio(dataInizio As Date, dataFine As Date) As cRisultatoModelloIndicatori.Indicatore
        Throw New NotImplementedException()
    End Function

    Protected Overrides Function _elaboraIndicatore(ByVal risElab As RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione) As RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione
        Throw New NotImplementedException()
    End Function

    'Public Function Ticchiolatura_Melo(ByVal Algoritmo As String, ByVal objParametri_server As AgronicaCoreParametri) As rispostaStandard(Of cRisultatoModello)

    '    Dim r As New rispostaStandard(Of cRisultatoModello)
    '    r.RispostaStringa = New cRisultatoModello()

    '    Try
    '        'TipoModello = Algoritmo
    '        Dim Classe As Integer = 14
    '        Dim Av_Cod As Integer = 63
    '        Dim Veg_Cod As Integer = 40

    '        Dim param As New ParametriModello(Av_Cod, Veg_Cod, Classe, Algoritmo, objParametri_server, r)

    '        Dim hh As Integer = 0
    '        Dim d_hh As AgronicaCoreMeteoBiz.InterfacciaMeteoNT.MeteoDSS
    '        While hh < datiMeteo.Count

    '            d_hh = datiMeteo(hh)
    '            If d_hh.Prec >= param.MmPioggia Then

    '                If PioggiaInfettante(Algoritmo, datiMeteo, hh) Then

    '                    'vado al giorno dopo
    '                    Dim this_doy As Integer = d_hh.DataOra.DayOfYear()
    '                    hh += 1
    '                    While hh < datiMeteo.Count AndAlso datiMeteo(hh).DataOra.DayOfYear = this_doy
    '                        hh += 1
    '                    End While

    '                Else
    '                    hh += 1
    '                End If
    '            Else
    '                hh += 1
    '            End If

    '        End While

    '        Dim output As New OutputModello

    '        output.aggiungiColonna("InizioPioggia", GetType(Date), "Inizio pioggia", "dd/MM/yyyy")
    '        output.aggiungiColonna("mmPioggia", GetType(Decimal), "Pioggia (mm)", "0.0")
    '        output.aggiungiColonna("OreBagnatura", GetType(Decimal), "Ore bagnatura", "0")
    '        output.aggiungiColonna("Temperatura", GetType(Decimal), "Temperatura", "0.00")
    '        output.aggiungiColonna("Infezione", "Infezione")
    '        output.aggiungiColonna("DataInfezione", GetType(Date), "Data infezione", "dd/MM/yyyy")
    '        output.aggiungiColonna("Evasione", "Evasione")
    '        'output.aggiungiColonna("PercEvasione", GetType(Decimal), "Evasione %", "0.00\%")
    '        'output.aggiungiColonna("GiornoEvasione", GetType(Date), "Giorno evasione", "dd/MM/yyyy")

    '        For Each d_m In DatiModello

    '            output.AddField(d_m.Data)
    '            output.AddField(d_m.MmPioggia)
    '            output.AddField(d_m.OreBagnatura)
    '            output.AddField(d_m.Temperatura)

    '            If d_m.Infezione > 1.8 Then
    '                output.AddField("Grave")
    '            ElseIf d_m.Infezione > 1.4 Then
    '                output.AddField("Media")
    '            ElseIf d_m.Infezione > 1 Then
    '                output.AddField("Leggera")
    '            Else
    '                output.AddField(Format(d_m.Infezione, "0"))
    '            End If

    '            'Dim lGiornoInfezione As DateTime = GiulianoToDate(tmpEvasione.Giorno, Year(DataInizio))
    '            'Dr("DataInfezione") = lGiornoInfezione
    '            output.AddField(d_m.Giorno.Date)

    '            If d_m.PercEvasione < 1 Then
    '                output.AddField(Format(d_m.PercEvasione, "0%"))
    '            Else
    '                output.AddField(d_m.GiornoEvasione.ToShortDateString)
    '            End If

    '            'output.AddField(d_m.PercEvasione * 100)
    '            'output.AddField(d_m.GiornoEvasione)

    '            output.Commit()

    '        Next

    '        r.RispostaOK = True
    '        r.RispostaStringa.Modello_Tabella1 = output.Output()

    '    Catch ex As Exception

    '        r.RispostaOK = False

    '        'uso questa funzione per ottenere il Messaggio..:
    '        r.Errore = "Errore durante l'operazione: " & vbCrLf &
    '            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

    '    End Try

    '    Return r

    'End Function
End Class





#If False Then

Public Class TicchiolaturaMelo

    Public Infezione(0 To 370, 0 To 23) As Boolean
    Public Evasione(0 To 370, 0 To 23) As EvasioneTicchiolatura

    Private MmPioggia As Double
    Private InizioPeriodo As Integer
    Private FinePeriodo As Integer
    Private TipoModello As Integer
    Private NumeroIncubazioni As Integer

    Public Sub New()

    End Sub

    Public Function Ticchiolatura_Melo(ByVal datiMeteo As List(Of AgronicaCoreMeteoBiz.InterfacciaMeteoNT.MeteoDSS),
                                       ByVal Algoritmo As String,
                                       ByVal DataInizio As DateTime,
                                       ByVal objParametri_server As AgronicaCoreParametri) As rispostaStandard(Of cRisultatoModello)

        Dim r As New rispostaStandard(Of cRisultatoModello)
        r.RispostaStringa = New cRisultatoModello()





        '>>> GABRIELE <<<
        r.RispostaOK = False
        r.Errore = "Errore durante l'operazione: "
        Return r





        Try

            TipoModello = Algoritmo
            MmPioggia = 0.2

            Dim Av_Cod As Integer = 63
            Dim Veg_Cod As Integer = 40
            Dim Classe As Integer = 14

            Dim xLEggi As New AgronicaCoreModelliPrevisionaliDAL.ParametriModelli_R
            Dim dtp As DataTable = xLEggi.Leggi(Av_Cod, Veg_Cod, Classe, "", "", objParametri_server)

            If dtp.Rows.Count = 0 Then

                'r.Errore &= "Errore: non trovo i parametri di calcolo, uso i valori di default"

            Else

                Dim rval As String = (From p In dtp.AsEnumerable
                                      Where p("ParametroNome") = "MmPioggia"
                                      Select CStr(p("ParametroValore"))).FirstOrDefault

                If Not String.IsNullOrEmpty(rval) Then
                    MmPioggia = rval
                End If

            End If

            'Dim xg As Integer
            'Dim yr As Integer

            'InizioPeriodo = DateToGiuliano(DataInizio)

            'FinePeriodo = DateToGiuliano(Meteo.UltimaDataOrari)

            'NumeroIncubazioni = 0

            'For xg = 0 To 370
            '    For yr = 0 To 23
            '        Infezione(xg, yr) = False
            '        Evasione(xg, yr).PercEvasione = 0#
            '        Evasione(xg, yr).GiornoEvasione = -1
            '        Evasione(xg, yr).Giorno = -1
            '        Evasione(xg, yr).Infezione = 0#
            '        Evasione(xg, yr).OreBagnatura = 0
            '        Evasione(xg, yr).MmPioggia = 0
            '        Evasione(xg, yr).Temperatura = 0
            '    Next
            'Next

            'xg = InizioPeriodo
            'yr = 0
            'Do While xg < FinePeriodo
            '    If Meteo.Orari(xg, yr).Prec >= MmPioggia Then
            '        Infezione(xg, yr) = PioggiaInfettante(xg, yr, FinePeriodo)
            '        If Infezione(xg, yr) Then
            '            xg = xg + 1
            '            yr = 0
            '        Else
            '            IncrementaGiorno(xg, yr)

            '        End If
            '    Else
            '        IncrementaGiorno(xg, yr)
            '        Infezione(xg, yr) = False
            '    End If
            'Loop

            ''calcola e memorizza il n° e le date di infezione
            'For xg = InizioPeriodo To FinePeriodo
            '    For yr = 0 To 23
            '        If Infezione(xg, yr) Then
            '            NumeroIncubazioni = NumeroIncubazioni + 1
            '        End If
            '    Next
            'Next


            Dim DT As DataTable = TabellaTicchiolaturaMelo(DataInizio)

            r.RispostaOK = True
            r.RispostaStringa.Modello_Tabella1 = JSON_TabellaTicchiolaturaMelo(DT, DataInizio)

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r
    End Function


    Private Function TabellaTicchiolaturaMelo(DataInizio As Date) As DataTable

        '***GABRIELE***
        'Dim i As Long
        'Dim j As Long
        'Dim Riga As Long

        'Dim DT As New DataTable
        'DT.Columns.Add(New DataColumn("InizioPioggia", GetType(Date)))
        'DT.Columns.Add(New DataColumn("mmPioggia", GetType(Decimal)))
        'DT.Columns.Add(New DataColumn("OreBagnatura", GetType(Decimal)))
        'DT.Columns.Add(New DataColumn("Temperatura", GetType(Decimal)))
        'DT.Columns.Add(New DataColumn("Infezione", GetType(String)))
        'DT.Columns.Add(New DataColumn("DataInfezione", GetType(Date)))
        'DT.Columns.Add(New DataColumn("Evasione", GetType(String)))
        ''DT.Columns.Add(New DataColumn("PercEvasione", GetType(Decimal)))
        ''DT.Columns.Add(New DataColumn("GiornoEvasione", GetType(Date)))

        'Riga = 0
        'For i = DateToGiuliano(DataInizio) To FinePeriodo
        '    For j = 0 To 23
        '        If Infezione(i, j) Then
        '            Riga = Riga + 1

        '            Dim Dr As DataRow = DT.NewRow

        '            Dr("InizioPioggia") = Meteo.Orari(i, j).Data


        '            Dim tmpEvasione As EvasioneTicchiolatura = Evasione(i, j)

        '            Dr("mmPioggia") = tmpEvasione.MmPioggia
        '            Dr("OreBagnatura") = tmpEvasione.OreBagnatura
        '            Dr("Temperatura") = tmpEvasione.Temperatura

        '            If tmpEvasione.Infezione > 1.8 Then
        '                Dr("Infezione") = "Grave"

        '            ElseIf tmpEvasione.Infezione > 1.4 Then
        '                Dr("Infezione") = "Media"

        '            ElseIf tmpEvasione.Infezione > 1 Then
        '                Dr("Infezione") = "Leggera"

        '            Else
        '                Dr("Infezione") = Format(tmpEvasione.Infezione, "##0")

        '            End If

        '            Dim lGiornoInfezione As DateTime = GiulianoToDate(tmpEvasione.Giorno, Year(DataInizio))
        '            Dr("DataInfezione") = lGiornoInfezione

        '            If tmpEvasione.PercEvasione < 1 Then
        '                Dr("Evasione") = Format(tmpEvasione.PercEvasione, "0%")
        '            Else
        '                Dr("Evasione") = GiulianoToDate(tmpEvasione.GiornoEvasione, Year(DataInizio)).ToShortDateString
        '            End If

        '            'Dr("PercEvasione") = tmpEvasione.PercEvasione * 100
        '            'Dim lGiornoEvasione As DateTime = GiulianoToDate(tmpEvasione.GiornoEvasione, Year(DataInizio))
        '            'Dr("GiornoEvasione") = lGiornoEvasione

        '            DT.Rows.Add(Dr)

        '        End If
        '    Next
        'Next

        'Return DT

    End Function

    Private Function PioggiaInfettante(ByVal xg As Integer, ByVal yr As Integer, ByVal Fine As Integer) As Boolean

        Dim Giorno As Integer = xg
        Dim GiornoFinale As Integer = xg
        Dim OraFinale As Integer = yr
        Dim Ora As Integer = yr
        Dim OreBagnatura As Integer = 0
        Dim OreInterruzione As Integer = 0
        Dim SommaPioggia As Double = 0
        Dim Infez As Double = 0
        Dim Evas As Double = 0
        Dim Termina As Boolean = False
        Dim OreTotali As Integer = 0
        Dim Temperatura As Double = 0
        Dim IntTemp As Double = 0
        Dim IntPiog As Double = 0
        Dim IntEvas As Double = 0

        ' Continua ad esaminare le ore di bagnatura finchè non raggiungo la data finale
        ' oppura finchè l'infezione non supera la soglia 1

        'Se non uso il modello di Mills devo modificare l'ora di inizio
        If TipoModello > 1 Then
            Select Case Ora
                Case 20 To 23
                    Giorno = Giorno + 1
                    Ora = 7
                Case 0 To 6
                    Ora = 7
            End Select
        End If

        'Cerca il buco di bagnatura di almeno 4 ore consecutive
        Do While Giorno <= Fine And (Not Termina)
            If Meteo.Orari(Giorno, Ora).Bagnat <> 0 Then
                OreTotali = OreTotali + 1
                OreBagnatura = OreBagnatura + 1
                Infez = Infez + fInfezione(Meteo.Orari(Giorno, Ora).Temp)
                Temperatura = Temperatura + Meteo.Orari(Giorno, Ora).Temp
                SommaPioggia = SommaPioggia + Meteo.Orari(Giorno, Ora).Prec
                Evas = Evas + fEvasione(Meteo.Orari(Giorno, Ora).Temp)
                If OreInterruzione <> 0 Then
                    OreBagnatura = OreBagnatura + OreInterruzione
                    OreTotali = OreTotali + OreInterruzione
                    Temperatura = Temperatura + IntTemp
                    SommaPioggia = SommaPioggia + IntPiog
                    Evas = Evas + IntEvas
                End If
                IntTemp = 0
                IntPiog = 0
                IntEvas = 0
                OreInterruzione = 0
                GiornoFinale = Giorno
                OraFinale = Ora
            Else
                OreInterruzione = OreInterruzione + 1
                IntTemp = IntTemp + Meteo.Orari(Giorno, Ora).Temp
                IntPiog = IntPiog + Meteo.Orari(Giorno, Ora).Prec
                IntEvas = IntEvas + fEvasione(Meteo.Orari(Giorno, Ora).Temp)
                'se ci sono state 1 o 2 ore di interruzione consecutive procedo con il calcolo
                If OreInterruzione < 3 Then
                    Infez = Infez + fInfezione(Meteo.Orari(Giorno, Ora).Temp)
                ElseIf OreInterruzione = 4 Then
                    'se ci sono state 4 ore di interruzione consecutive fermo il calcolo
                    Termina = True
                    OreInterruzione = 0
                End If
            End If
            IncrementaGiorno(Giorno, Ora)
        Loop

        If OreInterruzione <> 0 Then
            OreBagnatura = OreBagnatura + OreInterruzione
            OreTotali = OreTotali + OreInterruzione
            Temperatura = Temperatura + IntTemp
            SommaPioggia = SommaPioggia + IntPiog
            Evas = Evas + IntEvas
        End If

        Dim result As Boolean = True

        If Infez >= 1.0# Then

            Evasione(xg, yr).Infezione = Infez
            If Giorno <= Fine Then
                Evasione(xg, yr).Giorno = GiornoFinale
            Else
                Evasione(xg, yr).Giorno = Fine
            End If
            Evasione(xg, yr).OreBagnatura = OreBagnatura
            Evasione(xg, yr).MmPioggia = SommaPioggia
            Evasione(xg, yr).Temperatura = Temperatura / OreTotali

            'Cerco la data di presunta evasione
            Do While Evas < 1 And GiornoFinale <= Fine
                Evas = Evas + fEvasione(Meteo.Orari(GiornoFinale, OraFinale).Temp)
                IncrementaGiorno(GiornoFinale, OraFinale)
            Loop

            If Evas > 1 Then
                Evasione(xg, yr).PercEvasione = 1
            Else
                Evasione(xg, yr).PercEvasione = Evas
            End If

            Evasione(xg, yr).GiornoEvasione = GiornoFinale

        ElseIf GiornoFinale >= Fine Then

            'Memorizzo i valori di infezione nel caso sia giunto al limite dei dati meteo

            Evasione(xg, yr).Infezione = Infez
            Evasione(xg, yr).Giorno = Fine
            Evasione(xg, yr).OreBagnatura = OreBagnatura
            Evasione(xg, yr).MmPioggia = SommaPioggia
            Evasione(xg, yr).Temperatura = Temperatura / OreTotali
            Evasione(xg, yr).PercEvasione = 0#
            Evasione(xg, yr).GiornoEvasione = GiornoFinale

        Else

            result = False

        End If

        Return result
    End Function

    Private Function fInfezione(ByVal T As Double) As Double
        fInfezione = -0.058 + 0.02053 * T - 0.000558 * (T ^ 2)
    End Function
    Private Function fEvasione(ByVal T As Double) As Double
        fEvasione = 0.0003541 + 0.0002501 * T - 0.000000805 * (T ^ 2)
    End Function


    Private Function JSON_TabellaTicchiolaturaMelo(ByVal DT As DataTable, DataInizio As Date) As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome

        ' VAnni: 15/2/2017: todo: verificare tutte le chiavi commentate (es: tariffa_cod per costi SBTF..)

        c = New ColonneNome("InizioPioggia", "InizioPioggia", "date")
        c._FormatoParticolare = "#=kendo.toString(InizioPioggia, 'dd/MM/yyyy')#"
        c._Filtrabile = False
        l.Add(c)

        c = New ColonneNome("mmPioggia", "mmPioggia", "number")
        c._formatNr = "0.0"
        c._css = "allineadestra"
        c._cssHeader = "allineadestra"
        c._Filtrabile = False
        l.Add(c)

        c = New ColonneNome("OreBagnatura", "OreBagnatura", "number")
        c._formatNr = "0"
        c._css = "allineadestra"
        c._cssHeader = "allineadestra"
        c._Filtrabile = False
        l.Add(c)

        c = New ColonneNome("Temperatura", "Temperatura", "number")
        c._formatNr = "0.00"
        c._css = "allineadestra"
        c._cssHeader = "allineadestra"
        c._Filtrabile = False
        l.Add(c)

        c = New ColonneNome("Infezione", "Infezione", "string")
        l.Add(c)

        c = New ColonneNome("DataInfezione", "DataInfezione", "date")
        c._FormatoParticolare = "#=kendo.toString(DataInfezione, 'dd/MM/yyyy')#"
        c._Filtrabile = False
        l.Add(c)

        c = New ColonneNome("Evasione", "Evasione", "string")
        c._Filtrabile = False
        l.Add(c)

        'c = New ColonneNome("PercEvasione", "PercEvasione", "numeric")
        'c._formatNr = "0.00"
        'c._css = "allineadestra"
        'c._cssHeader = "allineadestra"
        'l.Add(c)

        'c = New ColonneNome("GiornoEvasione", "GiornoEvasione", "date")
        'c._FormatoParticolare = "#=kendo.toString(GiornoEvasione, 'dd/MM/yyyy')#"
        'c._Filtrabile = False
        'l.Add(c)

        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        js.Editabile_Deafault = False
        Dim risp As String = js.JSON_DataTable_Kendo(DT, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaDiscesa)

        Return risp

    End Function

End Class



Public Structure EvasioneTicchiolatura

    Public Infezione As Double
    Public Giorno As Integer
    Public PercEvasione As Double
    Public GiornoEvasione As Integer
    Public OreBagnatura As Integer
    Public MmPioggia As Single
    Public Temperatura As Single
    Public Tmin As Single
End Structure

#End If
