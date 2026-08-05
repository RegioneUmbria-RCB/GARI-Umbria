
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreModelliPrevisionaliBIZ
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json.Linq

Public Class Agronomica30_Pom_MISP_IPI
    Inherits AbstractModello

    '**********************************************************************************************
    'MISP (Main Infection and Sporulation Period)
    '**********************************************************************************************
    'Il modello MISP individua i periodi infettivi.
    'GRAFICO
    'Il rosso appare quando, in una finestra di 24 ore, si verificano condizioni di pioggia, umidità relativa e temperatura favorevoli. 
    'Il colore arancio indica condizioni favorevoli; quello rosso condizioni molto favorevoli. 
    'I diamanti sono vuoti se IPIcum<15.
    '**********************************************************************************************

    '**********************************************************************************************
    'IPI (Indice Potenziale Infettivo)
    '**********************************************************************************************
    'la prima fase, a basso rischio (sfondo verde), comprende il periodo in cui IPIcum<10;
    'la seconda fase, a rischio medio-basso (sfondo giallo), comprende il periodo in cui IPIcum<15;
    'la terza fase, a rischio medio-alto (sfondo arancione), comprende il periodo in cui IPIcum>15;
    'la quarta fase, a rischio alto (sfondo rosso), comprende il periodo successivo al momento in cui IPIcum>15 e IPI7>2,55.
    '**********************************************************************************************

    Private Class Infez
        Public DataInizio As DateTime
        Public DataFine As DateTime
        Public ValoriDH As List(Of Decimal)
        Public NumInfez24h As Integer
        Public Sub New(ByVal inizio As DateTime)
            DataInizio = inizio
            DataFine = Date.MinValue
            ValoriDH = New List(Of Decimal)
            NumInfez24h = 0
        End Sub
    End Class

    Private Class DatoGG
        Public Data As DateTime
        Public Tmin As Decimal
        Public Tmax As Decimal
        Public Tmed As Decimal
        Public UR As Decimal
        Public Pioggia As Decimal
        Public IPI As Decimal
        Public IPI_Cum_gg As Decimal
        Public IPI_7_gg As Decimal
    End Class

    Private Class RunningGG
        Private hh As Decimal
        Private pioggia_prec As Decimal
        Private IN_cumulato As Decimal
        Private ReadOnly stack7 As Queue(Of Decimal)

        Public Sub New()
            hh = 0
            pioggia_prec = 0
            IN_cumulato = 0
            stack7 = New Queue(Of Decimal)
        End Sub
        Public Function Create(ByVal d_hh As MeteoDSSItem) As DatoGG
            Dim d_gg As New DatoGG With {
                .Data = d_hh.DataOra.Date,
                .Tmin = d_hh.Temp,
                .Tmax = d_hh.Temp,
                .Tmed = d_hh.Temp,
                .UR = d_hh.UmRel,
                .Pioggia = d_hh.Prec,
                .IPI = 0,
                .IPI_Cum_gg = 0,
                .IPI_7_gg = 0
            }
            hh = 1
            Return d_gg
        End Function
        Public Sub Add(ByRef d_gg As DatoGG, ByVal d_hh As MeteoDSSItem)
            d_gg.Tmin = Math.Min(d_hh.Temp, d_gg.Tmin)
            d_gg.Tmax = Math.Max(d_hh.Temp, d_gg.Tmax)
            d_gg.Tmed += d_hh.Temp
            d_gg.UR += d_hh.UmRel
            d_gg.Pioggia += d_hh.Prec

            hh += 1
        End Sub
        Public Function Flush(ByVal d_gg As DatoGG) As DatoGG

            d_gg.Tmed /= hh
            d_gg.UR /= hh

            Dim pioggia_cum2gg As Decimal = d_gg.Pioggia + pioggia_prec
            pioggia_prec = d_gg.Pioggia

            Dim IPI As Decimal = 0

            If d_gg.Tmin > 7 Then

                Dim INDUR As Decimal = (-69.994545 + 1.502 * d_gg.UR - 0.007818 * Math.Pow(d_gg.UR, 2)) / 2
                Dim INPGTM As Decimal = 0
                Dim Ind_pioggia_cum2gg = Math.Min(0.00667 + 0.194405 * pioggia_cum2gg + 0.0002239 * Math.Pow(pioggia_cum2gg, 2), 3)

                Dim correz As Decimal = 1
                If d_gg.Tmin <= 13 Then
                    correz = (0.35 + 0.05 * d_gg.Tmin)
                End If

                Dim Ind_temp_media As Decimal = (-2.19247D + 0.259906D * d_gg.Tmed - 0.000139 * Math.Pow(d_gg.Tmed, 3) - 6.095832 * Math.Pow(10, -6) * Math.Pow(d_gg.Tmed, 4)) * correz
                Ind_temp_media = Math.Min(Math.Max(0, Ind_temp_media), 1)

                INDUR = Math.Min(Math.Max(0, INDUR), 2)
                If d_gg.Pioggia > 0 Then
                    INPGTM = Ind_temp_media * Ind_pioggia_cum2gg
                End If

                IPI = Math.Max(INPGTM, Ind_temp_media * INDUR)

            End If

            IN_cumulato += IPI
            stack7.Enqueue(IPI)
            If stack7.Count > 7 Then
                stack7.Dequeue()
            End If

            d_gg.IPI = IPI
            d_gg.IPI_Cum_gg = IN_cumulato
            d_gg.IPI_7_gg = stack7.Sum()

            Return d_gg
        End Function
    End Class

    Private ReadOnly _dtTrapianto As Date
    Private ReadOnly _colInfez As List(Of Infez)
    Private ReadOnly _elencoIPIgg As List(Of DatoGG)
    Private _MISP_probability As Decimal
    Private _dataSopraSoglia_12 As Date
    Private _dataSopraSoglia_15 As Date

    Public Sub New(ByVal datiMeteo As MeteoReadOnlyList, ByVal parametriAggiuntivi As String)
        MyBase.New(datiMeteo)

        _colInfez = New List(Of Infez)
        _elencoIPIgg = New List(Of DatoGG)
        _MISP_probability = 0
        _dataSopraSoglia_12 = Date.MinValue
        _dataSopraSoglia_15 = Date.MinValue

        Dim lettore As New LettoreParametri(parametriAggiuntivi)

        Dim dtTrapianto As Date = lettore.DateOrDefault("DataTrapianto", Date.MinValue)
        Dim dtTrapianto_gg As Integer = lettore.IntOrDefault("DataTrapianto_gg", 0)

        _dtTrapianto = _data_from_doy(dtTrapianto, dtTrapianto_gg, 4, 1) '1 Aprile
    End Sub

    Public Shared Function EsponiParams(ByVal params As String) As String

        Dim lettore As New LettoreParametri(params)

        Dim str_dt As String = lettore.OutputFromDate("DataTrapianto", "Data trapianto: ")

        If String.IsNullOrEmpty(str_dt) Then

            str_dt = lettore.OutputFromDOY("DataTrapianto_gg", "Data trapianto: ")
        End If

        Return str_dt
    End Function

    Protected Overrides Function _elaboraModello() As cRisultatoModello

        If Not Calcola() Then

            Return Nothing
        End If

        Dim risModello As New cRisultatoModello

        If _dataSopraSoglia_12 > Date.MinValue Then

            Dim msg As String = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_Pom_MISP_IPI_dataInizioRischioIPISopraSoglia_ & _dataSopraSoglia_12.ToShortDateString()

            risModello.Modello_WarningMsg = "<div style='text-align: center; font-size: larger;'>" & msg & "</div>"
        End If

        risModello.Modello_Tabella1 = Output_IPI()
        risModello.Modello_Tabella2 = Output_MISP()

        Return risModello
    End Function

    Protected Overrides Function _elaboraIndicatore_vecchio(dataInizio As Date, dataFine As Date) As cRisultatoModelloIndicatori.Indicatore

        Dim risIndic As New cRisultatoModelloIndicatori.Indicatore("MISP/IPI", dataInizio, dataFine)

        If Calcola() Then

            If _elencoIPIgg.Any() Then

                Dim d_gg = _elencoIPIgg.Last()

                If d_gg.IPI_Cum_gg >= 12 Then

                    If d_gg.IPI_Cum_gg >= 15 Then

                        risIndic.AuxMsg = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.ProbabilitaDiInfezione_ & Math.Round(_MISP_probability).ToString("0") & "%"
                    End If

                    risIndic.Fill(d_gg.IPI_7_gg, 5, {1.4, 2.55}, d_gg.Data, dataFine)
                Else

                    _errore = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_Pom_MISP_IPI_nonRaggiuntoIlPeriodoDiProbabileSviluppoAvversita
                    risIndic.Status = cRisultatoModelloIndicatori.Indicatore.enum_Status.Status_Error
                    risIndic.StatusMsg = _errore
                End If
            Else

                _errore = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_Pom_MISP_IPI_nonRaggiuntoIlPeriodoDiProbabileSviluppoAvversita
                risIndic.Status = cRisultatoModelloIndicatori.Indicatore.enum_Status.Status_Error
                risIndic.StatusMsg = _errore
            End If
        Else

            risIndic.Status = cRisultatoModelloIndicatori.Indicatore.enum_Status.Status_Error
            risIndic.StatusMsg = _errore
        End If

        Return risIndic
    End Function

    Protected Overrides Function _elaboraIndicatore(ByVal risElab As RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione) As RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione

        If Calcola() Then

            If _elencoIPIgg.Any() Then

                Dim d_gg = _elencoIPIgg.Last()

                If d_gg.IPI_Cum_gg >= 12 Then

                    If d_gg.IPI_Cum_gg >= 15 Then

                        risElab.AuxMsg = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.ProbabilitaDiInfezione_ & _MISP_probability.ToString("0") & "%"
                    End If

                    risElab.Fill(d_gg.IPI_7_gg, 5, {1.4, 2.55})
                Else

                    'WIDGET BARRA DI CARICAMENTO
                    'La barra di caricamento per il widget spento dovrebbe essere calcolata come (IPIcum/12)*100
                    'dove IPIcum è il valore giornaliero di IPI cumulato e 12 il valore di soglia 
                    'dopo il quale lo sviluppo della malattia diventa probabile ed il widget si accende

                    risElab.InProgress(d_gg.IPI_Cum_gg / 12D * 100D, My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_Pom_MISP_IPI_nonRaggiuntoIlPeriodoDiProbabileSviluppoAvversita)
                End If
            Else

                _errore = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_Pom_MISP_IPI_nonRaggiuntoIlPeriodoDiProbabileSviluppoAvversita
                risElab.Errore(_errore)
            End If
        Else

            risElab.Errore(_errore)
        End If

        Return risElab
    End Function

    Private Function Calcola() As Boolean
        Dim result As Boolean = True

        Try
            Dim doyTrapianto = _dtTrapianto.Date.DayOfYear

            Dim idxCalc As Integer = 0
            'N.B.: Inizio a calcolare dalla data trapianto
            While idxCalc < _datiMeteo.Count AndAlso _datiMeteo(idxCalc).DataOra.DayOfYear < doyTrapianto
                idxCalc += 1
            End While

            Dim runner As New RunningGG
            Dim d_gg As DatoGG = Nothing

            Dim data_ultimo_calcolo As DateTime = Date.MinValue

            Dim idxPrev24 As Integer = idxCalc

            While idxCalc < _datiMeteo.Count

                Dim dm = _datiMeteo(idxCalc)

                'creao l'elenco dei dati IPI giornalieri
                If d_gg Is Nothing OrElse d_gg.Data.DayOfYear <> dm.DataOra.DayOfYear Then

                    If d_gg IsNot Nothing Then

                        _elencoIPIgg.Add(runner.Flush(d_gg))

                        If _dataSopraSoglia_12 = Date.MinValue AndAlso _elencoIPIgg.Last.IPI_Cum_gg >= 12 Then
                            _dataSopraSoglia_12 = _elencoIPIgg.Last.Data
                        End If
                        If _dataSopraSoglia_15 = Date.MinValue AndAlso _elencoIPIgg.Last.IPI_Cum_gg >= 15 Then
                            _dataSopraSoglia_15 = _elencoIPIgg.Last.Data
                        End If
                    End If

                    d_gg = runner.Create(dm)
                Else

                    runner.Add(d_gg, dm)
                End If

                If DateDiff(DateInterval.Hour, _datiMeteo(idxPrev24).DataOra, _datiMeteo(idxCalc).DataOra) >= 23 Then
                    Calcola_MISP(idxPrev24, idxCalc, data_ultimo_calcolo)
                    idxPrev24 += 1
                End If

                idxCalc += 1
            End While

            If d_gg IsNot Nothing Then

                _elencoIPIgg.Add(runner.Flush(d_gg))

                If _dataSopraSoglia_12 = Date.MinValue AndAlso _elencoIPIgg.Last.IPI_Cum_gg >= 12 Then
                    _dataSopraSoglia_12 = _elencoIPIgg.Last.Data
                End If
                If _dataSopraSoglia_15 = Date.MinValue AndAlso _elencoIPIgg.Last.IPI_Cum_gg >= 15 Then
                    _dataSopraSoglia_15 = _elencoIPIgg.Last.Data
                End If
            Else

                result = False
                _errore = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_Pom_MISP_IPI_nonRaggiuntaDataEmergenzaOTrapiantoDellaColtura
            End If

        Catch ex As Exception

            result = False

            'uso questa funzione per ottenere il Messaggio..:
            _errore = Gias.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return result
    End Function

    Private Sub Calcola_MISP(ByVal idx_prev As Integer, ByVal idx_curr As Integer, ByRef data_ultimo_calcolo As DateTime)

        'Si considerano le ore come differenza tra data ora elemento oppure come differenza indici elemento?
        'Se non mancano dati non cambia nulla (distanza tra gli elementi sempre 1 ora), mentre se ci sono buchi orari...

        'Da qui cerco la condizione PT6
        Dim contaPG As Integer = 0
        Dim idx_PT6 As Integer = idx_prev

        While idx_PT6 < _datiMeteo.Count AndAlso contaPG < 6 AndAlso idx_PT6 <= idx_curr
            Dim d = _datiMeteo(idx_PT6)
            'If d.Prec > 0 AndAlso d.Temp >= 7 Then 'sostituito da (mail del 19/11/2021)
            If d.Prec > 0 AndAlso d.Temp >= 10 Then
                contaPG += 1
            End If
            idx_PT6 += 1
        End While

        If contaPG < 6 Then

            _MISP_probability = (contaPG / 12D) * 100D
            Return
        End If

        idx_PT6 -= 1

        'hh_dopo_PT6_nell_intervallo_24 < 6
        If 24 - (idx_PT6 - idx_prev) < 6 Then

            _MISP_probability = 50 '(hh_dopo_PT6_nell_intervallo_24 / 12D) * 100D
            Return
        End If

        Dim idx = idx_PT6 + 1
        Dim sumUR90 As Integer = 0

        While idx <= idx_curr AndAlso sumUR90 < 6
            If _datiMeteo(idx).UmRel >= 90 Then
                sumUR90 += 1
            Else
                sumUR90 = 0 'le ore con UR >= 90 devono essere consecutive (07/10/2020)
            End If
            idx += 1
        End While

        If sumUR90 < 6 Then

            _MISP_probability = ((6 + sumUR90) / 12D) * 100D
            Return
        End If

        _MISP_probability = 100

        If _colInfez.Any AndAlso _colInfez.Last.DataInizio.Date = _datiMeteo(idx_curr).DataOra.Date Then
            'se sono nello stesso gg dell'incubazione precedente non graficizzo ma tengo conto dell'incubazione che avverrà
            'conto solo le infezioni dello stesso gg senza segnarle se non c'e =1 inf
            _colInfez.Last.NumInfez24h += 1
            Return
        End If

        'avanzo di 7 ore
        idx = idx_PT6 + 7

        If idx >= _datiMeteo.Count Then
            Return
        End If

        Dim oInfez As Infez

        If _colInfez.Any AndAlso _colInfez.Last.DataInizio.Date = _datiMeteo(idx).DataOra.Date Then

            oInfez = _colInfez.Last

        Else

            oInfez = New Infez(_datiMeteo(idx).DataOra)

            _colInfez.Add(oInfez)
        End If

        'FN_incub_Schroedter
        Dim sumGD As Decimal = 0
        Dim idx1 = idx
        Dim EndWhile As Boolean = False

        While idx1 < _datiMeteo.Count AndAlso Not EndWhile

            If data_ultimo_calcolo.Date = _datiMeteo(idx1).DataOra.Date Then

                EndWhile = True
            Else

                sumGD += Math.Max(0, _datiMeteo(idx1).Temp - 7)

                oInfez.ValoriDH.Add(sumGD / 1543D)

                If sumGD >= 1543 Then

                    oInfez.DataFine = _datiMeteo(idx1).DataOra

                    data_ultimo_calcolo = _datiMeteo(idx).DataOra.Date

                    EndWhile = True
                End If
            End If

            idx1 += 1
        End While
    End Sub

    Private Function Output_MISP() As String

        Dim output As New OutputModello
        output.aggiungiColonna("NumInf", GetType(Integer), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_Pom_MISP_IPI_eventoInfettivo, "0")
        output.aggiungiColonna("DataInizioInf", GetType(Date), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_Pom_MISP_IPI_dataInizioInfezione, "dd/MM/yyyy HH")
        output.aggiungiColonna("DataFineInc", GetType(Date), Gias.DataInizio, "dd/MM/yyyy", True, "")._gruppoColonne = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_Pom_MISP_IPI_periodoComparsaSintomi
        'output.aggiungiColonna("V_inf_sintomi", GetType(Date), "Inizio", "dd/MM/yyyy")
        output.aggiungiColonna("V_sup_sintomi", GetType(Date), Gias.DataFine, "dd/MM/yyyy", True, "")._gruppoColonne = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_Pom_MISP_IPI_periodoComparsaSintomi
        'output.aggiungiColonna("ComparsaSintomi", GetType(Decimal), "Comparsa probabile sintomi (gg)", "0.00")
        output.aggiungiColonna("IPI_7_gg", GetType(Decimal), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_Pom_MISP_IPI_setteGiorni, "0.00")
        Dim indic_IPI_7_gg = output.aggiungiIndicatore("IPI_7_gg_ind", {1.4, 2.55, System.Decimal.MaxValue}, OutputIndicator.enum_IndicatorType.Colors).setFieldVal("IPI_7_gg")
        'output.aggiungiColonna("DataOra", GetType(Date), "Data", "dd/MM/yyyy HH")._hidden = True
        'output.aggiungiColonna("Valore", GetType(Decimal), "Valore", "0.000000")._hidden = True

        Dim num As Decimal = 0

        Dim idxIPI As Integer = 0

        For Each oInf In _colInfez

            Dim d_gg As DatoGG = Nothing
            If idxIPI < _elencoIPIgg.Count Then
                While idxIPI < _elencoIPIgg.Count AndAlso _elencoIPIgg(idxIPI).Data < oInf.DataInizio.Date
                    idxIPI += 1
                End While

                If idxIPI < _elencoIPIgg.Count AndAlso _elencoIPIgg(idxIPI).Data.DayOfYear = oInf.DataInizio.DayOfYear Then

                    d_gg = _elencoIPIgg(idxIPI)
                End If
            End If

            If d_gg IsNot Nothing Then

                If d_gg.IPI_Cum_gg >= 15 Then

                    num += 1

                    output.AddField(num)
                    output.AddField(oInf.DataInizio)

                    If oInf.DataFine > Date.MinValue Then

                        output.AddField(oInf.DataFine)
                        'Dim V_inf_sintomi = oInf.DataFine.Date.AddHours(-30)
                        Dim V_sup_sintomi = oInf.DataFine.Date.AddDays(2 + 2 / num)
                        'comparsa probabile sintomi in gg = V_sup_sintomi - V_inf_sintomi
                        'Dim hh As Decimal = DateDiff(DateInterval.Hour, V_inf_sintomi, V_sup_sintomi)

                        'output.AddField(V_inf_sintomi)
                        output.AddField(V_sup_sintomi)

                    Else

                        output.AddField(DBNull.Value)
                        output.AddField(DBNull.Value)
                    End If

                    output.AddField(d_gg.IPI_7_gg)
                    output.AddField(indic_IPI_7_gg.colorForVal(d_gg.IPI_7_gg))

                    output.Commit()
                End If
            End If
        Next

        Return output.Output({indic_IPI_7_gg}.ToList())
    End Function

    Private Function Output_IPI() As String
        'i18n Indice Potenziale Infettivo, Main Infection and Sporulation Period
        Dim output As New OutputModello
        output.aggiungiColonna("Data", GetType(Date), Gias.Data, "dd/MM/yyyy")
        output.aggiungiColonna("Tmin", GetType(Decimal), Gias.TemperaturaAbbr & " Min", "0.00")
        output.aggiungiColonna("Tmax", GetType(Decimal), Gias.TemperaturaAbbr & " Max", "0.00")
        output.aggiungiColonna("Tmed", GetType(Decimal), Gias.TemperaturaAbbr & " Med", "0.00")
        output.aggiungiColonna("UR", GetType(Decimal), Gias.UmiditaRelativa, "0.00")
        output.aggiungiColonna("Pioggia", GetType(Decimal), Gias.Pioggia, "0.00")
        output.aggiungiColonna("IPI", GetType(Decimal), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_Pom_MISP_IPI, "0.00")
        output.aggiungiColonna("IPI_Cum_gg", GetType(Decimal), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_Pom_MISP_IPI_cumulato, "0.00")
        output.aggiungiColonna("IPI_7_gg", GetType(Decimal), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_Pom_MISP_IPI_setteGiorni, "0.00")
        Dim indic_IPI_7_gg = output.aggiungiIndicatore("IPI_7_gg_ind", {1.4, 2.55, System.Decimal.MaxValue}, OutputIndicator.enum_IndicatorType.Colors).setFieldVal("IPI_7_gg")
        output.aggiungiColonna("Infez", GetType(Integer), "", "0")._hidden = True
        output.aggiungiColonna("NumInfez", GetType(Integer), "", "0")._hidden = True

        Dim iCol As Integer = 0
        Dim NumInfez As Integer = 0

        For Each d_gg In _elencoIPIgg

            If d_gg.IPI_Cum_gg >= 12 Then

                output.AddField(d_gg.Data)
                output.AddField(d_gg.Tmin)
                output.AddField(d_gg.Tmax)
                output.AddField(d_gg.Tmed)
                output.AddField(d_gg.UR)
                output.AddField(d_gg.Pioggia)
                output.AddField(d_gg.IPI)
                output.AddField(d_gg.IPI_Cum_gg)
                output.AddField(d_gg.IPI_7_gg)
                output.AddField(indic_IPI_7_gg.colorForVal(d_gg.IPI_7_gg))

                Dim newInfez As Boolean = False

                If d_gg.IPI_Cum_gg >= 15 Then

                    If iCol < _colInfez.Count Then

                        While iCol < _colInfez.Count AndAlso _colInfez(iCol).DataInizio < d_gg.Data
                            iCol += 1
                        End While

                        If iCol < _colInfez.Count AndAlso _colInfez(iCol).DataInizio.DayOfYear = d_gg.Data.DayOfYear Then

                            newInfez = True
                            NumInfez += 1
                        End If
                    End If

                End If

                If newInfez Then

                    output.AddField(1)
                    output.AddField(NumInfez)
                Else

                    output.AddField(DBNull.Value)
                    output.AddField(DBNull.Value)
                End If

                output.Commit()
            End If
        Next

        Return output.Output({indic_IPI_7_gg}.ToList())
    End Function
End Class

