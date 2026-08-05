
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelliPrevisionaliBIZ
Imports AgronicaCoreVarieBIZ

Public Class Agronomica30_OidioVite
    Inherits AbstractModello

    Private Class Dato_G
        Public data As Date
        Public tmed As Decimal
        Public UR As Decimal
        Public pioggia As Decimal
        Public LW As Integer
        Public sumDD10 As Decimal
        Public PAR As Decimal
        Public BBCH As Decimal
        Public INF As INF_Primario
    End Class

    Private Delegate Sub MeteoAggregatorCallbak(d_g As Dato_G)

    Private Class MeteoAggregator

        Private _dst_elem As Dato_G
        Private _cnt As Integer

        Private ReadOnly _bbch As Agronomica30_BBCH

        Private ReadOnly _soglia_bb0 As Decimal
        Private ReadOnly _soglia_3fg As Decimal
        Private _somma_nhh As Decimal
        Private _data_bb0 As DateTime
        Private _data_3fg As DateTime
        Private _somma_dd10 As Decimal
        Private _somma_dd10_ora0 As Decimal
        Private _flag_ora0 As Boolean

        Private ReadOnly _callback As MeteoAggregatorCallbak

        Public ReadOnly Property Data_bb0 As DateTime
            Get
                Return _data_bb0
            End Get
        End Property

        Public ReadOnly Property Data_3fg As DateTime
            Get
                Return _data_3fg
            End Get
        End Property

        Public ReadOnly Property Somma_dd10_Ora0 As Decimal
            Get
                Return _somma_dd10_ora0
            End Get
        End Property

        Public Sub New(soglia_bb0 As Decimal, soglia_3fg As Decimal, callback As MeteoAggregatorCallbak)

            _dst_elem = Nothing

            _bbch = New Agronomica30_BBCH(12, 33, 26, Agronomica30_BBCH.VarietaVite.CabernetSauvignon)

            _soglia_bb0 = soglia_bb0
            _soglia_3fg = soglia_3fg
            _somma_nhh = 0
            _data_bb0 = Date.MinValue
            _data_3fg = Date.MinValue

            _somma_dd10 = 0
            _somma_dd10_ora0 = 0
            _flag_ora0 = False

            _callback = callback
        End Sub

        Public Sub Process(srcList As MeteoReadOnlyList)

            For Each src_elem In srcList

                _compute_nhh(src_elem)

                _bbch.Calc(src_elem.Temp)

                If _dst_elem IsNot Nothing Then

                    If src_elem.DataOra.DayOfYear = _dst_elem.data.DayOfYear Then

                        _dst_elem.tmed += src_elem.Temp
                        _dst_elem.UR += src_elem.UmRel
                        _dst_elem.pioggia += src_elem.Prec
                        _dst_elem.LW += src_elem.BagnEffettiva(85)
                        _dst_elem.BBCH = Math.Max(_dst_elem.BBCH, _bbch.BBCH_rip)

                        _cnt += 1
                    Else

                        _finalizza()
                        _callback(_dst_elem)

                        _dst_elem = Nothing
                    End If
                End If

                If _dst_elem Is Nothing Then

                    _dst_elem = New Dato_G With {
                            .data = src_elem.DataOra.Date,
                            .tmed = src_elem.Temp,
                            .UR = src_elem.UmRel,
                            .pioggia = src_elem.Prec,
                            .LW = src_elem.BagnEffettiva(85),
                            .sumDD10 = 0,
                            .PAR = 0,
                            .BBCH = _bbch.BBCH_rip,
                            .INF = Nothing
                        }
                    _cnt = 1
                End If
            Next

            If _dst_elem IsNot Nothing Then

                _finalizza()
                _callback(_dst_elem)
            End If
        End Sub

        Private Sub _finalizza()

            _dst_elem.tmed /= _cnt
            _dst_elem.UR /= _cnt

            _somma_dd10 += Math.Max(_dst_elem.tmed - 10, 0) 'somma gradi ora

            _dst_elem.sumDD10 = _somma_dd10

            If Not _flag_ora0 AndAlso _data_bb0 > Date.MinValue Then

                If _dst_elem.data.Date > _data_bb0.Date Then

                    _somma_dd10_ora0 = _somma_dd10
                    _flag_ora0 = True
                End If
            End If
        End Sub

        Private Sub _compute_nhh(src_elem As MeteoDSSItem)

            'calcolo della data in cui si raggiunge la soglia per rottura gemme (bb0) e/o 3 foglie (3fg)

            If _data_bb0 > Date.MinValue AndAlso _data_3fg > Date.MinValue Then

                Return
            End If

            Dim nhh As Decimal = 0

            Dim Tc_max As Decimal = 33 'cambio del 09/05/2017
            Dim Tc_min As Decimal = 10
            Dim Tc_opt As Decimal = 25
            Dim t As Decimal = src_elem.Temp

            If Tc_min <= t AndAlso t <= Tc_max Then

                Dim alpha As Decimal = Math.Log(2 / Math.Log((Tc_max - Tc_min) / (Tc_opt - Tc_min)))
                nhh = (2D * Math.Pow((t - Tc_min), alpha) * Math.Pow((Tc_opt - Tc_min), alpha) - Math.Pow((t - Tc_min), (2D * alpha))) / Math.Pow((Tc_opt - Tc_min), (2D * alpha))
            End If

            _somma_nhh += Math.Max(nhh, 0)

            If _data_bb0 <= Date.MinValue AndAlso _somma_nhh >= _soglia_bb0 Then

                _data_bb0 = src_elem.DataOra
            End If

            If _data_3fg <= Date.MinValue AndAlso _somma_nhh >= _soglia_3fg Then

                _data_3fg = src_elem.DataOra
            End If
        End Sub
    End Class

    Private Class INF_Primario
        Public deltaPAR As Decimal
        Public ADR As Decimal
        Public INF As Decimal
        Public COLONIA As Decimal
        Public latenza As Decimal
        Public data_fine_latenza As DateTime
        Public sporul As Decimal
        Public data_fine_sporul As DateTime
    End Class

    Private Class Sporulazione_G
        Public data As Date
        Public sec_index As Decimal
        Public index2 As Decimal
        Public indicatore As Decimal
    End Class

    Private Delegate Sub SporAggregatorCallback(spor_g As Sporulazione_G)

    Private Class SporAggregator

        Private ReadOnly _callback As SporAggregatorCallback
        Private ReadOnly _somma3gg As Queue(Of Decimal)
        Private _curr_spor_g As Sporulazione_G

        Public Sub New(callback As SporAggregatorCallback)
            _callback = callback
            _somma3gg = New Queue(Of Decimal)
            _curr_spor_g = Nothing
        End Sub

        Public Sub Add(dt As Date, index As Decimal, tot_colonie_index As Decimal)

            If _curr_spor_g IsNot Nothing Then

                If dt.Date = _curr_spor_g.data Then

                    _curr_spor_g.sec_index += tot_colonie_index
                    _curr_spor_g.index2 = Math.Max(_curr_spor_g.index2, index)
                Else

                    Close()
                End If
            End If

            If _curr_spor_g Is Nothing Then

                _curr_spor_g = New Sporulazione_G With {
                    .data = dt,
                    .sec_index = tot_colonie_index,
                    .index2 = index
                }
            End If
        End Sub

        Public Sub Close()

            If _curr_spor_g Is Nothing Then

                Return
            End If

            _somma3gg.Enqueue(_curr_spor_g.sec_index)

            If _somma3gg.Count() > 3 Then

                _somma3gg.Dequeue()
            End If

            If _somma3gg.Count() = 3 Then

                _curr_spor_g.indicatore = _somma3gg.Sum()
            Else

                _curr_spor_g.indicatore = 0
            End If

            _callback(_curr_spor_g)

            _curr_spor_g = Nothing
        End Sub
    End Class

    Private ReadOnly Lista_G As LinkedList(Of Dato_G)
    Private ReadOnly Sporulazioni_G As List(Of Sporulazione_G)

    Public Sub New(ByVal datiMeteo As MeteoReadOnlyList)
        MyBase.New(datiMeteo)

        Lista_G = New LinkedList(Of Dato_G)
        Sporulazioni_G = New List(Of Sporulazione_G)
    End Sub

    Protected Overrides Function _elaboraModello() As cRisultatoModello

        If Not CalcolaOidio() Then

            Return Nothing
        End If

        Return New cRisultatoModello With {
            .Modello_Tabella1 = OUT_Collezioni(),
            .Modello_Tabella2 = OUT_Sporulazioni()
        }
    End Function

    Protected Overrides Function _elaboraIndicatore_vecchio(dataInizio As Date, dataFine As Date) As cRisultatoModelloIndicatori.Indicatore

        Dim risIndic As New cRisultatoModelloIndicatori.Indicatore("Oidio", dataInizio, dataFine)

        If CalcolaOidio() Then

            If Sporulazioni_G.Count > 0 Then
                risIndic.Fill(Sporulazioni_G.Last().indicatore, 40, {8, 24}, Sporulazioni_G.Last().data, dataFine)
            Else
                'Nessun evento infettivo... Cosa uso come indicatore/messaggio?
                risIndic.Fill(0, 40, {8, 24}, dataFine, dataFine)
            End If
        Else

            risIndic.Status = cRisultatoModelloIndicatori.Indicatore.enum_Status.Status_Error
            risIndic.StatusMsg = _errore
        End If

        Return risIndic
    End Function

    Protected Overrides Function _elaboraIndicatore(ByVal risElab As RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione) As RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione

        If CalcolaOidio() Then

            If Sporulazioni_G.Count > 0 Then
                risElab.Fill(Sporulazioni_G.Last().indicatore, 40, {8, 24})
            Else
                'Nessun evento infettivo... Cosa uso come indicatore/messaggio?
                risElab.Fill(0, 40, {8, 24})
            End If
        Else

            risElab.Errore(_errore)
        End If

        Return risElab
    End Function

    Private Function CalcolaOidio() As Boolean

        Dim result As Boolean = True

        Try

            'Calcolo i dati giornalieri aggregando i dati orari

            Dim meteo_aggr As New MeteoAggregator(102.5, 170.6, Sub(elem As Dato_G) Lista_G.AddLast(elem))

            meteo_aggr.Process(_datiMeteo)

            'Se raggiungo data rottura gemme
            If meteo_aggr.Data_bb0 > Date.MinValue Then

                Dim data_bb0 = meteo_aggr.Data_bb0
                Dim data_3fg = meteo_aggr.Data_3fg
                Dim somma_dd10_ora0 = meteo_aggr.Somma_dd10_Ora0

                'Creo collezione di eventi infettivi

                Dim collezioni As New List(Of Dato_G)

                Dim node = Lista_G.First
                Dim precedentePAR As Decimal

                While node IsNot Nothing

                    Dim d_g = node.Value

                    'Proporzione ascospore pronte per il rilascio
                    d_g.PAR = Math.Exp(-1.97D * Math.Exp(-1.91D * (d_g.sumDD10 - somma_dd10_ora0) / 100D))

                    If d_g.data > data_bb0 Then

                        If CreaRilascio(node, precedentePAR) Then

                            collezioni.Add(d_g)

                            precedentePAR = d_g.PAR
                        End If
                    Else

                        If d_g.data = data_bb0.Date Then

                            precedentePAR = d_g.PAR
                        End If
                    End If

                    node = node.Next
                End While

                If collezioni.Count > 0 Then

                    'Calcolo le sporulazioni orarie ed aggrego per ottenere le giornaliere

                    Dim idx As Integer = _datiMeteo.FindDateGE(collezioni.First.data)

                    Dim data_fine_latenza As DateTime = collezioni.First.INF.data_fine_latenza

                    If data_fine_latenza > Date.MinValue Then

                        data_fine_latenza = data_fine_latenza.AddHours(12)
                    Else

                        data_fine_latenza = Date.MaxValue
                    End If

                    Dim spor_h_index As Decimal
                    Dim spor_h_tot_colonie_index As Decimal
                    Dim vIndexCum As Decimal = 0
                    Dim data_partenza As DateTime
                    Dim data_arrivo As DateTime

                    Dim aggr As New SporAggregator(Sub(spor_g) Sporulazioni_G.Add(spor_g))

                    While idx < _datiMeteo.Count

                        Dim dm = _datiMeteo(idx)

                        spor_h_index = 0
                        spor_h_tot_colonie_index = 0

                        If dm.DataOra >= data_fine_latenza Then

                            Dim vIndex As Decimal = Index(dm, data_3fg)

                            vIndexCum += vIndex
                            spor_h_index = vIndexCum

                            For Each d_inf In collezioni

                                If d_inf.INF.data_fine_latenza > Date.MinValue Then

                                    data_partenza = d_inf.INF.data_fine_latenza.AddHours(12) 'DOMANDA: 12 ore dopo oppure alle 00 del giorno dopo?

                                    If dm.DataOra >= data_partenza Then

                                        data_arrivo = Date.MaxValue

                                        If d_inf.INF.data_fine_sporul > Date.MinValue Then

                                            data_arrivo = d_inf.INF.data_fine_sporul
                                        End If

                                        If dm.DataOra <= data_arrivo Then

                                            spor_h_tot_colonie_index += (vIndex * d_inf.INF.COLONIA)
                                        End If
                                    Else

                                        Exit For
                                    End If
                                Else

                                    'se non ho fine latenza significa che non sporula quindi non posso calcolare
                                    Exit For
                                End If
                            Next
                        End If

                        aggr.Add(dm.DataOra.Date, spor_h_index, spor_h_tot_colonie_index)

                        idx += 1
                    End While

                    aggr.Close()
                End If
            Else

                result = False
                _errore = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.NonRaggiuntaLaFaseFenologicaNecessariaAlloSviluppoDellaAvversita
            End If

        Catch ex As Exception

            result = False

            'uso questa funzione per ottenere il Messaggio..:
            _errore = Gias.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return result
    End Function

    Private Function CreaRilascio(node As LinkedListNode(Of Dato_G), precedentePAR As Decimal) As Boolean

        Dim d_g = node.Value

        'Scarto se Pioggia <= 2 And temp <= 4 And temp >= 30 OR data_doybb0 < 0 OR PAR > 0.99 per rilascio infettante
        'Se PAR > 0.999 considero esauriti i rilasci primari
        If d_g.pioggia <= 2 OrElse (d_g.tmed <= 4 OrElse 30 <= d_g.tmed) OrElse d_g.PAR > 0.999 Then

            Return False
        End If

        Dim infez = New INF_Primario With {
            .deltaPAR = d_g.PAR - precedentePAR,
            .ADR = 0,
            .INF = 0,
            .latenza = 0,
            .data_fine_latenza = Date.MinValue,
            .sporul = 0,
            .data_fine_sporul = Date.MinValue
        }

        d_g.INF = infez

        'tasso infezione in funzione di VPD e temp
        Dim Teq = _Teq(d_g.tmed, 5, 31) 'bug 28 ?

        If Teq > 0 Then

            'vapour pressure deficit 
            Dim VPD As Decimal = MeteoDSSItem.VPD(d_g.UR, d_g.tmed)

            infez.INF = Math.Pow((7.391D * Math.Pow(Teq, 2.403D) * (1 - Teq)), 0.892) * Math.Exp(-0.221D * VPD)
        End If

        'Tasso rilascio ascospore
        'si rilascia se piove e se la temp è compresa tra 4 e 30
        If d_g.pioggia >= 2 AndAlso 4 <= d_g.tmed AndAlso d_g.tmed <= 30 Then

            infez.ADR = 1D - 0.969D * Math.Exp(-0.00039D * Math.Pow(d_g.tmed, 2) * d_g.LW)
        End If

        'Acospore sulle foglie AOL_foglie
        'AOL_foglie = deltaPAR * ADR

        'fine calcoli infezioni PRIMARIE colonia finale (la moltiplico X 100)
        infez.COLONIA = infez.ADR * infez.deltaPAR * infez.INF * 100D

        'calcoli per latenza e sporulazione
        'dalla data di uscita colonia devo calcolare data fine latenza e poi la data di fine sporulazione

        'Fine latenza
        Dim n = node
        While n IsNot Nothing And infez.latenza < 1

            infez.latenza += 1D / (0.067D * Math.Pow(n.Value.tmed, 2) - 3.244D * n.Value.tmed + 44.7D)

            If infez.latenza >= 1 Then

                infez.latenza = 1
                infez.data_fine_latenza = n.Value.data.AddDays(-1)
            Else

                n = n.Next
            End If

        End While

        If infez.latenza = 1 Then

            'Fine sporulazione
            Dim a As Decimal = 41.7746907571
            Dim b As Decimal = -0.082804161295

            While n IsNot Nothing AndAlso infez.sporul < 1

                infez.sporul += (1D / (a * Math.Exp(b * n.Value.tmed)))

                If infez.sporul >= 1 Then

                    infez.sporul = 1
                    infez.data_fine_sporul = n.Value.data
                End If

                n = n.Next
            End While
        End If

        Return True
    End Function

    Private Function _Teq(t As Decimal, tmin As Decimal, tmax As Decimal) As Decimal

        'temperatura equivalente

        If tmin <= t AndAlso t <= tmax Then

            Return (t - tmin) / (tmax - tmin)
        End If

        Return 0
    End Function

    Private Function Index(dm As MeteoDSSItem, data3_fg_distese As DateTime) As Decimal

        'Indice combinato (Index) di adeguatezza delle condizioni ambientali per le infezioni secondarie di oidio

        Dim LW = dm.BagnEffettiva(85)

        If dm.UmRel > 95 OrElse dm.Prec > 0 OrElse LW > 0 Then 'Moist (umido)

            Return 0
        End If

        'numero di giorni trascorsi dallo stadio di 3 foglie distese
        Dim diff_gg As Decimal = DateDiff(DateInterval.Day, data3_fg_distese.Date, dm.DataOra.Date)

        If diff_gg <= 0 Then

            Return 0
        End If

        Dim Teq_spor As Decimal = _Teq(dm.Temp, 14, 32)
        Dim Teq_infs As Decimal = _Teq(dm.Temp, 5, 33)

        'Tasso di sporulazione
        Dim SPOR As Decimal = Math.Pow((4.42D * Math.Pow(Teq_spor, 1.121D) * (1D - Teq_spor)), 1.9661D)

        'Infezioni secondarie
        'L'indice di infezione esprime l'efficienza relativa di infezione secondaria calcolata sulla base delle condizioni 
        'di temperatura, umidità relativa, pioggia e durata della bagnatura.
        'L'indice è pari a zero quando le condizioni ambientali non rendono possibile l'infezione e cresce con il verificarsi di condizioni via via più favorevoli.
        Dim INFs As Decimal = Math.Pow((2.20054D * Math.Pow(Teq_infs, 1.055D) * (1D - Teq_infs)), 0.35)

        'Indice di crescita del fungo nei tessuti dell'ospite
        Dim GER As Decimal = Math.Exp(-0.0151D * Math.Pow((dm.Temp - 24.43D), 2) - 0.000617D * Math.Pow((dm.UmRel - 82.188D), 2))
        Dim GRO As Decimal = Math.Pow((7.915D * Math.Pow(GER, 3.172D) * (1D - GER)), 0.311D)

        Dim HOST = ((Math.Pow(diff_gg, 3) * 0.0002D) + (176.6 * Math.Sqrt(diff_gg)) - (114.9D * Math.Log(diff_gg)) - (14D * diff_gg) - 66D) / 100D
        HOST = Math.Max(0, HOST)

        Return SPOR * INFs * GRO * HOST
    End Function

    Private Function OUT_Sporulazioni() As String

        Dim output As New OutputModello
        output.aggiungiColonna("Data", GetType(Date), Gias.Data, "dd/MM/yyyy")
        output.aggiungiColonna("sec_index", GetType(Decimal), "Index secondarie", "0.00")._hidden = True
        output.aggiungiColonna("index2", GetType(Decimal), "Index 2", "0.00")._hidden = True
        output.aggiungiColonna("somma_index_3gg", GetType(Decimal), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_OidioVite_sommaMobileIndexTreGiorni, "0.00")

        Dim indicator = output.aggiungiIndicatore("somma_index_3gg_IND", {8, 24, System.Decimal.MaxValue}, OutputIndicator.enum_IndicatorType.Colors).setFieldVal("somma_index_3gg")

        For Each sp_gg In Sporulazioni_G

            output.AddField(sp_gg.data)
            output.AddField(sp_gg.sec_index)
            output.AddField(sp_gg.index2)
            output.AddField(sp_gg.indicatore)
            output.AddField(indicator.colorForVal(sp_gg.indicatore))

            output.Commit()

        Next

        Return output.Output({indicator}.ToList())

    End Function

    Private Function OUT_Collezioni() As String
        'i18n da tradurre: num
        Dim output As New OutputModello
        output.aggiungiColonna("Data", GetType(Date), Gias.Data, "dd/MM/yyyy")
        output.aggiungiColonna("pioggia", GetType(Decimal), Gias.Pioggia & " (mm)", "0.00")
        output.aggiungiColonna("PAR", GetType(Decimal), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_OidioVite_proporzioneAscosporeRilasciabili & " (%)", "0\\%")
        output.aggiungiColonna("filtro", GetType(Boolean), "filtro", "")._hidden = True
        output.aggiungiColonna("deltaPAR", GetType(Decimal), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_OidioVite_intensitàCoorteRilasciata & " (num 0-1)", "0.00", True)
        output.aggiungiColonna("AOL_foglie", GetType(Decimal), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_OidioVite_ascosporePresentiSulleFoglie & " (num 0-1)", "0.00")
        output.aggiungiColonna("INF", GetType(Decimal), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_OidioVite_infettività & " (num 0-1)", "0.00")
        output.aggiungiColonna("COLONIA", GetType(Decimal), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_OidioVite_ascosporeFormantiColonieSulleFoglie & " (num)", "0.00")

        Dim indicatorNum = output.aggiungiIndicatore("COLONIA_IND", {0.5, 1.5, System.Decimal.MaxValue}, OutputIndicator.enum_IndicatorType.Numbers).setFieldVal("COLONIA")
        output.aggiungiColonna("COLONIA2", GetType(Decimal), "", "")._hidden = True
        Dim indicatorClr = output.aggiungiIndicatore("COLONIA2_IND", {0.5, 1.5, System.Decimal.MaxValue}, OutputIndicator.enum_IndicatorType.Colors).setFieldVal("COLONIA2")
        output.aggiungiColonna("fine_latenza", "% " & My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_OidioVite_latenzaDataFine)
        output.aggiungiColonna("fine_sporulazione", "% " & My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_OidioVite_sporulazioneDataFine)

        Dim node = Lista_G.First

        While node IsNot Nothing

            Dim d_g = node.Value

            output.AddField(d_g.data)
            output.AddField(d_g.pioggia)
            output.AddField(d_g.PAR * 100D)

            If d_g.INF IsNot Nothing Then

                output.AddField(True)

                output.AddField(d_g.INF.deltaPAR)
                output.AddField(d_g.INF.deltaPAR * d_g.INF.ADR)
                output.AddField(d_g.INF.INF)
                output.AddField(d_g.INF.COLONIA)
                output.AddField(indicatorNum.colorForVal(d_g.INF.COLONIA))
                output.AddField(d_g.INF.COLONIA)
                output.AddField(indicatorClr.colorForVal(d_g.INF.COLONIA))
                If d_g.INF.data_fine_latenza > Date.MinValue Then
                    output.AddField(d_g.INF.data_fine_latenza.ToString("dd/MM/yyyy"))
                Else
                    output.AddField(d_g.INF.latenza.ToString("0%"))
                End If
                If d_g.INF.data_fine_sporul > Date.MinValue Then
                    output.AddField(d_g.INF.data_fine_sporul.ToString("dd/MM/yyyy"))
                Else
                    output.AddField(d_g.INF.sporul.ToString("0%"))
                End If
            Else

                output.AddField(False)
            End If

            output.Commit()

            node = node.Next
        End While

        Return output.Output({indicatorNum, indicatorClr}.ToList())
    End Function
End Class

