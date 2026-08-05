
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelliPrevisionaliBIZ
Imports AgronicaCoreVarieBIZ



Public Class Agronomica30_MaculaturaPero
    Inherits AbstractModello

    Private Class queue_sum
        Private _num As Integer
        Private _queue As Queue(Of Decimal)
        Public Sub New(ByVal n As Integer)
            _num = n
            _queue = New Queue(Of Decimal)
        End Sub
        Public Function Enqueue(ByVal v As Decimal) As Decimal
            _queue.Enqueue(v)
            If _queue.Count() > _num Then
                _queue.Dequeue()
            End If
            If _queue.Count() = 3 Then
                Return _queue.Sum()
            End If
            Return 0
        End Function
    End Class

    Private MustInherit Class Dato_gg_abs
        Public Data As DateTime
        Public Temp As Decimal
        Public Prec As Decimal
        Public UmRel As Decimal
        Public LW As Decimal
        Public Sub New(ByVal dt As DateTime, ByVal d As MeteoDSSItem)
            Data = dt
            Temp = d.Temp
            Prec = d.Prec
            UmRel = d.UmRel
            LW = d.BagnEffettiva(85)
        End Sub
        Public Sub aggiungi(ByVal d As MeteoDSSItem)
            Temp += d.Temp
            Prec += d.Prec
            UmRel += d.UmRel
            LW += d.BagnEffettiva(85)
        End Sub
        Public Sub finalizza(ByVal hh_cnt As Decimal, ByVal queue3gg As queue_sum)
            Temp /= hh_cnt
            UmRel /= hh_cnt
            OR_finalizza(queue3gg)
        End Sub
        Protected MustOverride Sub OR_finalizza(ByVal queue3gg As queue_sum)
    End Class

    Private Class Dato_gg : Inherits Dato_gg_abs
        Public BSPspor As Decimal
        Public BSPspor3gg As Decimal
        Public Sub New(ByVal dt As DateTime, ByVal d As MeteoDSSItem)
            MyBase.New(dt, d)
        End Sub
        Protected Overrides Sub OR_finalizza(ByVal queue3gg As queue_sum)

            Dim VPD As Decimal = MeteoDSSItem.VPD(UmRel, Temp)
            Dim condU As Integer = If(UmRel >= 80, 1, 0)
            Dim condP As Integer = If(Prec > 0, 1, 0)
            Dim condB As Integer = If(LW > 10, 1, 0)
            Dim condV As Integer = If(VPD <= 5, 1, 0)
            Dim condT As Decimal = If(15 <= Temp And Temp <= 25, 1, 0)
            'Modifica 20 Giugno 2019 Candolo (riunione)
            'Dim condT As Decimal = If(13 <= Temp And Temp <= 25, 1, 0)

            Dim condizioni As Integer = condU + condP + condB + condV

            Dim ft As Decimal = 0
            If Temp > 3 Then
                Dim teq As Decimal = (Temp - 3D) / 30D
                ft = Math.Pow(7.765D * Math.Pow(teq, 2.38D) * (1D - teq), 6.824D)
                '((7.765 * ((temp - 3) / 30) ^ 2.38 * (1 - (temp - 3) / 30)) ^ 6.824)
            End If

            Dim seasonV As Decimal = Season(Data)

            BSPspor = condizioni * (condT + ft) * seasonV
            BSPspor3gg = queue3gg.Enqueue(BSPspor)

        End Sub
        Private Function Season(ByVal dt As DateTime) As Decimal
            Dim mar_1 = New Date(dt.Year, 3, 1)
            'coefficienti dal 01 marzo
            Dim doy = dt.DayOfYear - mar_1.DayOfYear
            If doy < 0 Then
                Return 0
            End If
            'gcandolo 22/01/2017
            'popolo season con coefficenti
            'considero max 184 giorni
            Dim arr() As Decimal = {0.000, 0.000, 0.000, 0.000, 0.000, 0.000, 0.000, 0.000, 0.000,
                0.01, 0.01, 0.01,
                0.03, 0.03, 0.03, 0.03, 0.03, 0.03, 0.03, 0.03, 0.03, 0.03, 0.03, 0.03, 0.03, 0.03, 0.03, 0.03, 0.03, 0.03, 0.03, 0.03, 0.03,
                0.04, 0.04, 0.04, 0.04, 0.04, 0.04, 0.04, 0.04, 0.04, 0.04,
                0.07, 0.08, 0.09, 0.1, 0.11, 0.11, 0.11, 0.11, 0.11, 0.11, 0.11, 0.11, 0.11, 0.11, 0.11, 0.11, 0.11, 0.11,
                0.143, 0.143, 0.186, 0.214, 0.23, 0.23, 0.23, 0.23, 0.23, 0.23, 0.23, 0.23, 0.23, 0.23, 0.23,
                0.26, 0.26, 0.27, 0.3, 0.3, 0.3, 0.3, 0.257, 0.214, 0.15, 0.15, 0.15, 0.15, 0.15, 0.15, 0.15, 0.15, 0.15, 0.15,
                0.17, 0.17, 0.214, 0.214, 0.23, 0.27, 0.27, 0.27, 0.27, 0.27, 0.27, 0.24, 0.24, 0.24, 0.24, 0.24, 0.24, 0.2, 0.2, 0.2, 0.2, 0.243,
                0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271,
                0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271,
                0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271, 0.271}

            Dim val As Decimal = 0.271
            If doy < arr.Length Then
                val = arr(doy)
            End If

            Return val
        End Function
    End Class

    Private Class Dato_gg_8 : Inherits Dato_gg
        Public BSPcast As Decimal
        Public BSPcast3gg As Decimal
        Public Sub New(ByVal dt As DateTime, ByVal d As MeteoDSSItem)
            MyBase.New(dt, d)
        End Sub
        Protected Overrides Sub OR_finalizza(ByVal queue3gg As queue_sum)
            Dim x As Decimal = -1.70962D + 0.0289D * Temp + 0.04943D * LW + 0.00868D * Temp * LW - 0.002362 * Math.Pow(LW, 2) - 0.000238 * LW * Math.Pow(Temp, 2)
            Dim apo As Decimal = Math.Pow(10, x)
            BSPcast = apo / 3.7942D
            BSPcast3gg = queue3gg.Enqueue(BSPcast)
        End Sub
    End Class

    Private Class elem
        Public d_gg As Dato_gg
        Public d_gg_8 As Dato_gg_8
        Public Function GlobalRisk() As Decimal
            Dim mat_risk(,) As Decimal = {{0, 0, 1}, {0, 1, 2}, {1, 2, 2}}
            Return mat_risk(CodificaRisk(d_gg_8.BSPcast3gg), CodificaRisk(d_gg.BSPspor3gg))
        End Function
        Private Function CodificaRisk(ByVal bsp As Decimal) As Integer

            'Author     : gcandolo
            'Description:'codifico il valore immesso in rischio  1 o 2 o 3
            '             per spore e rischio, tengo scala uguale
            '             input BSPcast3 oppure BSPspor3 output 0-1-2
            'Date       : 22 / 1 / 2017


            Dim risk As Integer = 2
            'TANTE SPORE/risk colore rosso
            If bsp < 0.25 Then
                'NON CI SONO SPORE/risk colore verde
                risk = 0
            Else
                If bsp < 0.39 Then
                    'POCHE SPORE/risk colore giallo
                    risk = 1
                End If
            End If

            Return risk
        End Function
    End Class

    Private _lista As List(Of elem)

    Public Sub New(ByVal datiMeteo As MeteoReadOnlyList)
        MyBase.New(datiMeteo)

        _lista = New List(Of elem)
    End Sub

    Protected Overrides Function _elaboraModello() As cRisultatoModello

        If Not CalcolaMaculatura() Then

            Return Nothing
        End If

        Return New cRisultatoModello With {.Modello_Tabella1 = OutputDatiGG()}
    End Function

    Protected Overrides Function _elaboraIndicatore_vecchio(dataInizio As Date, dataFine As Date) As cRisultatoModelloIndicatori.Indicatore

        Dim risIndic As New cRisultatoModelloIndicatori.Indicatore("Maculatura", dataInizio, dataFine)

        If CalcolaMaculatura() Then
            Dim _value As Decimal = 0

            If _lista.Count > 0 Then

                _value = _lista.Last().GlobalRisk()
                'valore sempre 0, 1, 2 con 0 -> verde, 1 -> giallo, 2 -> rosso
            Else

                risIndic.AuxMsg = ""
            End If

            risIndic.Fill(_value, 2.5, {0.5, 1.5}, _lista.Last().d_gg.Data, dataFine)

        Else

            risIndic.Status = cRisultatoModelloIndicatori.Indicatore.enum_Status.Status_Error
            risIndic.StatusMsg = _errore
        End If

        Return risIndic
    End Function

    Protected Overrides Function _elaboraIndicatore(ByVal risElab As RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione) As RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione

        If CalcolaMaculatura() Then
            Dim _value As Decimal = 0

            If _lista.Count > 0 Then

                _value = _lista.Last().GlobalRisk()
                'valore sempre 0, 1, 2 con 0 -> verde, 1 -> giallo, 2 -> rosso
            Else

                risElab.AuxMsg = ""
            End If

            risElab.Fill(_value, 2.5, {0.5, 1.5})
        Else

            risElab.Errore(_errore)
        End If

        Return risElab
    End Function

    'Public Function Maculatura(ByVal datiMeteo As List(Of AgronicaCoreMeteoBiz.InterfacciaMeteoNT.MeteoDSS)) As rispostaStandard(Of cRisultatoModello)

    '    Dim r As New rispostaStandard(Of cRisultatoModello)
    '    r.RispostaStringa = New cRisultatoModello()
    '    r.RispostaOK = False
    '    r.Errore = ""

    '    If CalcolaMaculatura(datiMeteo) Then

    '        r.RispostaOK = True
    '        r.RispostaStringa.Modello_Tabella1 = OutputDatiGG()

    '    Else

    '        r.Errore = mErrore

    '    End If

    '    Return r

    'End Function

    'Public Function Maculatura_Indicatore(ByVal datiMeteo As List(Of AgronicaCoreMeteoBiz.InterfacciaMeteoNT.MeteoDSS), ByVal dataInizio As DateTime, ByVal dataFine As DateTime) As rispostaStandard(Of cRisultatoModelloIndicatori.Indicatore)

    '    Dim r As New rispostaStandard(Of cRisultatoModelloIndicatori.Indicatore)
    '    r.RispostaStringa = New cRisultatoModelloIndicatori.Indicatore("Maculatura", dataInizio, dataFine)
    '    r.RispostaOK = False
    '    r.Errore = ""

    '    If CalcolaMaculatura(datiMeteo) Then

    '        r.RispostaOK = True

    '        Dim _value As Decimal = 0

    '        If lista.Count > 0 Then

    '            _value = lista.Last().GlobalRisk()
    '            'valore sempre 0, 1, 2 con 0 -> verde, 1 -> giallo, 2 -> rosso
    '        Else

    '            r.RispostaStringa.AuxMsg = ""
    '        End If

    '        r.RispostaStringa.Fill(_value, 2.5, {0.5, 1.5}, lista.Last().d_gg.Data, dataFine)

    '    Else

    '        r.Errore = mErrore
    '        r.RispostaStringa.Status = cRisultatoModelloIndicatori.Indicatore.enum_Status.Status_Error
    '        r.RispostaStringa.StatusMsg = mErrore

    '    End If

    '    Return r

    'End Function

    Private Function CalcolaMaculatura() As Boolean

        '------------------------------------------------------------------------------------------
        'TABELLA ModelliPrevisionali (GIAS_Server_MATRICE) CAMPO ModDescrHtml
        '------------------------------------------------------------------------------------------
        '<div style = "line-height: 1.5em;" >
        '    <p>La maculatura del pero (Brown spot) è una delle infezioni più gravi per le pere in Europa.</p>
        '    <p>Il modello calcola:</p>
        '    <ol style = "line-height: 2em;" >
        '        <li>
        '            Un indice chiamato BSPspor <b>(Brown Spot <u>Prediction for sporulation</u>)</b> giornaliero. Prevede la possibile emissione di spore da parte del patogeno ed un indice chiamato BSPspor3gg somma degli ultimi 3 giorni di BSPspor.
        '        </li>
        '        <li>
        '            Un inidice chiamato BSPcast <b>(Brown Spot Pear <u>foreCASTing system</u>)</b>. Prevede se si sono verificate delle condizioni di bagnatura e di temperatura idonee all'infezione.<br><b>BSPcast</b> è un modello di tipo empirico in grado di determinare quando le condizioni ambientali risultano favorevoli per le infezioni di pero da parte di <i>S. vesicarium</i> e per lo sviluppo della malattia ed un indice chiamato BSPcast3gg somma mobile degli ultimi 3 giorni di BSPcast.
        '        </li>
        '        <li>
        '            Indice rischio globale: Indice combinato tra BSPspor e BSPcast, tiene conto sia delle condizioni che influenzano la sporulazione sia delle condizioni climatiche favorevoli all'infezione.
        '        </li>
        '    </ol>
        '    <p>I calcoli convenzionalmente iniziano dal 1 aprile di ogni anno.</p>
        '</div>
        '------------------------------------------------------------------------------------------

        Dim result As Boolean = True

        Try

            'NOTA SUI DATI METEO
            'Per il calcolo del BSPcast si considera uno shift dei dati meteo di 8 ore

            Dim dataInizio As DateTime = _datiMeteo(0).DataOra
            Dim doyInizio As Integer = (New Date(dataInizio.Year, 4, 1)).DayOfYear

            Dim idx As Integer = 0
            While idx < _datiMeteo.Count AndAlso _datiMeteo(idx).DataOra.DayOfYear < doyInizio
                idx += 1
            End While

            If idx < _datiMeteo.Count() Then

                Dim listaGG As New List(Of Dato_gg)
                Dim listaGG_8 As New List(Of Dato_gg_8)

                Dim hh_cnt As Decimal = 0
                Dim dgg As Dato_gg = Nothing
                Dim hh_cnt_8 As Decimal = 0
                Dim dgg_8 As Dato_gg_8 = Nothing
                Dim queue_BSPspor As New queue_sum(3)
                Dim queue_BSPcast As New queue_sum(3)

                While idx < _datiMeteo.Count

                    Dim m_do = _datiMeteo(idx)

                    If dgg Is Nothing OrElse m_do.DataOra.DayOfYear <> dgg.Data.DayOfYear Then

                        If dgg IsNot Nothing Then
                            dgg.finalizza(hh_cnt, queue_BSPspor)
                            listaGG.Add(dgg)
                        End If

                        dgg = New Dato_gg(m_do.DataOra, m_do)
                        hh_cnt = 1

                    Else

                        dgg.aggiungi(m_do)
                        hh_cnt += 1

                    End If

                    Dim data_8 As DateTime = m_do.DataOra.AddHours(-8)
                    If data_8 >= dataInizio Then
                        If dgg_8 Is Nothing OrElse data_8.DayOfYear <> dgg_8.Data.DayOfYear Then

                            If dgg_8 IsNot Nothing Then
                                dgg_8.finalizza(hh_cnt_8, queue_BSPcast)
                                listaGG_8.Add(dgg_8)
                            End If

                            dgg_8 = New Dato_gg_8(data_8, m_do)
                            hh_cnt_8 = 1

                        Else

                            dgg_8.aggiungi(m_do)
                            hh_cnt_8 += 1

                        End If

                    End If

                    idx += 1
                End While

                If dgg IsNot Nothing Then
                    dgg.finalizza(hh_cnt, queue_BSPspor)
                    listaGG.Add(dgg)
                End If

                If dgg_8 IsNot Nothing Then
                    dgg_8.finalizza(hh_cnt_8, queue_BSPcast)
                    listaGG_8.Add(dgg_8)
                End If

                dgg = Nothing
                dgg_8 = Nothing
                Dim doy As Integer
                Dim doy_8 As Integer
                While listaGG.Count > 0 AndAlso listaGG_8.Count > 0
                    If dgg Is Nothing Then
                        dgg = listaGG.First()
                        listaGG.RemoveAt(0)
                    End If
                    If dgg_8 Is Nothing Then
                        dgg_8 = listaGG_8.First()
                        listaGG_8.RemoveAt(0)
                    End If
                    doy = dgg.Data.DayOfYear
                    doy_8 = dgg_8.Data.DayOfYear
                    If doy = doy_8 Then
                        _lista.Add(New elem With {.d_gg = dgg, .d_gg_8 = dgg_8})
                        dgg = Nothing
                        dgg_8 = Nothing
                    Else
                        If doy < doy_8 Then
                            dgg = Nothing
                        Else
                            dgg_8 = Nothing
                        End If
                    End If
                End While

            Else

                result = False
                _errore = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_MaculaturaPero_ilModelloPrevisionaleParteDalPrimoAprile

            End If

        Catch ex As Exception

            result = False

            'uso questa funzione per ottenere il Messaggio..:
            _errore = Gias.ErroreDuranteOperazione_ & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return result

    End Function

    Private Function OutputDatiGG() As String
        'i18n
        Dim output As New OutputModello
        output.aggiungiColonna("Data", GetType(Date), Gias.Data, "dd/MM/yyyy")
        output.aggiungiColonna("Temp", GetType(Decimal), Gias.Temperatura & " (°C)", "0.00")
        output.aggiungiColonna("Prec", GetType(Decimal), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_MaculaturaPero_precipitazioni, "0.00")
        output.aggiungiColonna("UmRel", GetType(Decimal), Gias.UmiditaRelativa & " (%)", "0")
        output.aggiungiColonna("LW", GetType(Decimal), Gias.BagnaturaFogliare & " (" & Gias.Ore.ToLower() & ")", "0")
        output.aggiungiColonna("BSPspor", GetType(Decimal), "BSPspor", "0.00")
        output.aggiungiColonna("BSPspor3gg", GetType(Decimal), "BSPspor3gg", "0.00")
        Dim BSPspor_ind = output.aggiungiIndicatore("BSPspor_IND", {0.25, 0.39, System.Decimal.MaxValue}, OutputIndicator.enum_IndicatorType.Numbers).setFieldVal("BSPspor3gg")
        output.aggiungiColonna("BSPcast", GetType(Decimal), "BSPcast", "0.00")
        output.aggiungiColonna("BSPcast3gg", GetType(Decimal), "BSPcast3gg", "0.00")
        Dim BSPcast_ind = output.aggiungiIndicatore("BSPcast_IND", {0.25, 0.39, System.Decimal.MaxValue}, OutputIndicator.enum_IndicatorType.Numbers).setFieldVal("BSPcast3gg")
        output.aggiungiColonna("Global_risk", GetType(Decimal), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_MaculaturaPero_indiceRischioGlobale, "0")
        Dim Global_risk_ind = output.aggiungiIndicatore("Global_risk_IND", {1, 2, System.Decimal.MaxValue}, OutputIndicator.enum_IndicatorType.Colors).setFieldVal("Global_risk").setFieldHidden(True)

        For Each elem In _lista

            output.AddField(elem.d_gg.Data)
            output.AddField(elem.d_gg.Temp)
            output.AddField(elem.d_gg.Prec)
            output.AddField(elem.d_gg.UmRel)
            output.AddField(elem.d_gg.LW)
            output.AddField(elem.d_gg.BSPspor)
            output.AddField(elem.d_gg.BSPspor3gg)
            output.AddField(BSPspor_ind.colorForVal(elem.d_gg.BSPspor3gg))
            output.AddField(elem.d_gg_8.BSPcast)
            output.AddField(elem.d_gg_8.BSPcast3gg)
            output.AddField(BSPcast_ind.colorForVal(elem.d_gg_8.BSPcast3gg))
            Dim risk As Decimal = elem.GlobalRisk()
            output.AddField(risk)
            output.AddField(Global_risk_ind.colorForVal(risk))

            output.Commit()

        Next

        Return output.Output({BSPspor_ind, BSPcast_ind, Global_risk_ind}.ToList(), BSPspor_ind.PlotBands())
    End Function

End Class
