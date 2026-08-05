Imports AgronicaCoreDataProvider.My.Resources

Public Class BetaCoProB_Cercosporiosi
    Inherits AbstractModello

    Private Class DatoHH
        Public ReadOnly DataOra As DateTime
        Public ReadOnly UR As Decimal
        Public ReadOnly T As Decimal
        Public ReadOnly UR_corr As Decimal
        Public ReadOnly T_corr As Decimal

        Public Sub New(d As MeteoDSSItem)
            DataOra = d.DataOra
            UR = d.UmRel
            T = d.Temp
            UR_corr = (d.UmRel * 0.9096D) + 13.664D
            T_corr = (d.Temp * 1.146D) - 3.4121D
        End Sub
    End Class

    Private Class DatoGG
        Public DataOra As DateTime
        Public UR_avg As Decimal
        Public T_avg As Decimal
        Public N_ore As Integer
        Public TCorr_avg As Decimal
        Public IG As Integer
        Public Inf_2GG As Integer
        Public IG_Cum As Integer
    End Class

    Private ReadOnly _dtPartenza As DateTime
    Private ReadOnly _sogliaMinUR As Decimal
    Private ReadOnly _datiHH As List(Of DatoHH)
    Private ReadOnly _datiGG As List(Of DatoGG)

    Public Sub New(ByVal datiMeteo As MeteoReadOnlyList, ByVal parametriAggiuntivi As String)
        MyBase.New(datiMeteo)

        Dim lettore As New LettoreParametri(parametriAggiuntivi)

        Dim dtPartenza_gg As Integer = lettore.IntOrDefault("DataPartenza_gg", 0)

        _dtPartenza = _data_from_doy(DateTime.MinValue, dtPartenza_gg, 5, 20) '20 maggio

        _dtPartenza = _dtPartenza.AddHours(-12) 'le 12:00 del 19 maggio

        _sogliaMinUR = 76

        _datiHH = New List(Of DatoHH)
        _datiGG = New List(Of DatoGG)
    End Sub


    Public Shared Function EsponiParams(ByVal params As String) As String

        Dim lettore As New LettoreParametri(params)

        Return lettore.OutputFromDOY("DataPartenza_gg", "Data partenza: ")
    End Function


    Protected Overrides Function _elaboraModello() As cRisultatoModello

        If Not Calcola() Then

            Return Nothing
        End If

        Dim output As New OutputModello
        output.aggiungiColonna("Data", GetType(DateTime), Gias.Data, "dd/MM/yyyy")
        output.aggiungiColonna("UmiditaRelativa", GetType(Decimal), Gias.UmiditaRelativa & " (%)", "0.00")
        output.aggiungiColonna("Temperatura", GetType(Decimal), Gias.Temperatura & " (°C)", "0.00")
        output.aggiungiColonna("IG", GetType(Integer), "IG", "0")
        output.aggiungiColonna("Inf_2GG", GetType(Integer), "2 GG Tot Inf", "0")
        output.aggiungiColonna("IG_Cum", GetType(Integer), "Andamento", "0")
        Dim indicator = output.aggiungiIndicatore("Inf_2GG_IND", {4, 7, Decimal.MaxValue}, OutputIndicator.enum_IndicatorType.Colors).setFieldVal("Inf_2GG")

        For Each dgg In _datiGG

            output.AddField(dgg.DataOra)
            output.AddField(dgg.UR_avg)
            output.AddField(dgg.T_avg)
            output.AddField(dgg.IG)
            output.AddField(dgg.Inf_2GG)
            output.AddField(dgg.IG_Cum)
            output.AddField(indicator.colorForVal(dgg.Inf_2GG))

            output.Commit()
        Next

        Dim warning = "<div style='text-align:center; font-size:larger; padding:6px 3px; border-radius:5px; color:#fff; background-color:#428bca; font-weight:bold;'>" &
        "Il modello è in fase di prototipo e pertanto quest'anno per le indicazioni di difesa dalla cercospora " &
        "siete pregati di seguire i consigli di COPROB (bollettini ed SMS) e rivolgervi al vostro tecnico di riferimento." &
        "</div>"

        Return New cRisultatoModello With {.Modello_WarningMsg = warning, .Modello_Tabella1 = output.Output({indicator}.ToList())}
    End Function

    Protected Overrides Function _elaboraIndicatore_vecchio(ByVal dataInizio As DateTime, ByVal dataFine As DateTime) As cRisultatoModelloIndicatori.Indicatore
        Throw New NotImplementedException()
    End Function

    Protected Overrides Function _elaboraIndicatore(ByVal risElab As RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione) As RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione

        If Calcola() Then

            If _datiGG.Any Then
                risElab.Fill(_datiGG.Last().Inf_2GG, 14, {4, 7})
            Else
                risElab.Errore("Non è stata raggiunta la data di partenza.")
            End If
        Else

            risElab.Errore(_errore)
        End If

        Return risElab
    End Function

    Private Class runningGG
        Private ReadOnly _tabellaIG As Decimal(,)
        Private _sumT As Decimal
        Private _sumUR As Decimal
        Private _nOre As Integer
        Private _sumTCorr As Decimal
        Private _nOreSopraSoglia As Integer
        Private _lastGG As DatoGG
        Public Sub New()
            _tabellaIG = {
                {14.99, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1},
                {15.55, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 2, 2},
                {16.11, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 2, 2, 3, 3, 4},
                {16.66, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 2, 2, 2, 3, 3, 4, 4, 4, 5},
                {17.22, 0, 0, 0, 0, 0, 0, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 4, 4, 4, 5, 5, 5},
                {17.77, 0, 0, 0, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 6, 6, 6},
                {18.33, 0, 0, 0, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 6, 6, 6},
                {18.88, 0, 0, 0, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 6, 6, 6},
                {19.44, 0, 0, 0, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 6, 6, 6},
                {19.99, 0, 0, 0, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 6, 6, 6},
                {20.55, 0, 0, 0, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 6, 6, 6},
                {21.1, 0, 0, 1, 1, 1, 1, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 6, 6, 6, 6},
                {21.66, 0, 1, 1, 1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 6, 6, 6, 6, 6},
                {22.21, 1, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 4, 4, 4, 4, 4, 5, 5, 6, 6, 6, 6, 6, 7},
                {22.77, 1, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 4, 4, 4, 4, 4, 5, 5, 6, 6, 6, 6, 6, 7},
                {23.32, 1, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 4, 4, 4, 4, 4, 5, 5, 6, 6, 6, 6, 6, 7},
                {23.88, 1, 1, 1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 6, 6, 6, 6, 7, 7},
                {24.43, 1, 1, 1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 4, 5, 5, 5, 6, 6, 6, 7, 7, 7, 7},
                {24.99, 1, 1, 1, 1, 2, 2, 2, 3, 3, 4, 4, 4, 5, 5, 6, 6, 6, 7, 7, 7, 7, 7, 7, 7},
                {25.54, 1, 1, 1, 2, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 6, 7, 7, 7, 7, 7, 7, 7, 7, 7},
                {26.1, 1, 1, 2, 2, 3, 3, 4, 5, 5, 6, 6, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7},
                {26.65, 1, 1, 2, 2, 3, 4, 4, 5, 6, 6, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7},
                {27.21, 1, 1, 2, 2, 3, 4, 4, 5, 6, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7},
                {27.76, 1, 1, 2, 2, 3, 4, 4, 5, 6, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7},
                {28.32, 1, 1, 2, 2, 3, 4, 4, 5, 6, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7},
                {28.87, 1, 1, 2, 2, 3, 4, 4, 5, 6, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7},
                {29.43, 1, 1, 2, 2, 3, 4, 4, 5, 6, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7},
                {29.98, 1, 1, 2, 2, 3, 4, 4, 5, 6, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7},
                {30.54, 1, 1, 2, 2, 3, 4, 4, 5, 6, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7},
                {31.09, 1, 1, 2, 2, 3, 4, 4, 5, 6, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7},
                {31.65, 1, 1, 2, 2, 3, 4, 4, 5, 6, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7},
                {32.2, 1, 1, 2, 2, 3, 4, 4, 5, 6, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7},
                {32.76, 1, 1, 2, 2, 3, 4, 4, 5, 6, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7},
                {33.31, 1, 1, 2, 2, 3, 4, 4, 5, 6, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7},
                {33.87, 1, 1, 2, 2, 3, 4, 4, 5, 6, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7},
                {34.42, 1, 1, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3}
            }
            _sumT = 0
            _sumUR = 0
            _nOre = 0
            _sumTCorr = 0
            _nOreSopraSoglia = 0
            _lastGG = Nothing
        End Sub
        Public Sub add(d_hh As DatoHH, sogliaUR As Decimal)
            _sumT += d_hh.T
            _sumUR += d_hh.UR
            _nOre += 1
            If d_hh.UR_corr >= sogliaUR Then
                _sumTCorr += d_hh.T_corr
                _nOreSopraSoglia += 1
            End If
        End Sub
        Public Function flush(dt As DateTime) As DatoGG

            Dim t_med As Decimal = 0
            Dim ig As Integer = 0
            If _nOreSopraSoglia > 0 Then

                t_med = _sumTCorr / _nOreSopraSoglia

                Dim coord_Y As Integer = 1
                If 15 <= t_med Then
                    coord_Y = Math.Round(((t_med - 14.99) / 0.56) + 1.5)
                End If

                If t_med < 34.4 Then

                    'Dim col As Integer = 35
                    'While col >= 0
                    '    If t_med <= tabellaIG(col, 0) Then
                    '        ig = tabellaIG(col, coord_Y)
                    '    End If
                    '    col -= 1
                    'End While
                    ig = _tabellaIG(coord_Y - 1, _nOreSopraSoglia)
                End If
            End If

            Dim dGG = New DatoGG With {
                .DataOra = dt,
                .UR_avg = _sumUR / _nOre,
                .T_avg = _sumT / _nOre,
                .N_ore = _nOreSopraSoglia,
                .TCorr_avg = t_med,
                .IG = ig
            }

            If _lastGG IsNot Nothing Then

                dGG.Inf_2GG = _lastGG.IG + dGG.IG
                dGG.IG_Cum = _lastGG.IG_Cum + dGG.IG
            End If

            _lastGG = dGG

            _sumT = 0
            _sumUR = 0
            _nOre = 0
            _sumTCorr = 0
            _nOreSopraSoglia = 0

            Return dGG
        End Function
    End Class

    Private Function Calcola() As Boolean

        Dim result As Boolean = True

        Try

            Dim next_gg As DateTime = _dtPartenza.AddHours(23)
            Dim d_hh As DatoHH

            Dim runner As New runningGG()

            For Each d In _datiMeteo

                If d.DataOra >= _dtPartenza Then

                    d_hh = New DatoHH(d)

                    _datiHH.Add(d_hh)

                    If d_hh.DataOra > next_gg Then

                        _datiGG.Add(runner.flush(next_gg))

                        While next_gg < d_hh.DataOra
                            next_gg = next_gg.AddHours(24)
                        End While
                    End If

                    runner.add(d_hh, _sogliaMinUR)
                End If
            Next

            'dovrei considerare anche l'ultimo giorno della serie...
            'potrei avere dati incompleti che "falsano" il rischio 
            'occorre stabilire un criterio

        Catch ex As Exception

            result = False

            'uso questa funzione per ottenere il Messaggio..:
            _errore = Gias.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return result
    End Function
End Class
