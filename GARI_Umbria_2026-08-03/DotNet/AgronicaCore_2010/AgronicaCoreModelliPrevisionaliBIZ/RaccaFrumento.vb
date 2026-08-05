
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports Newtonsoft.Json


Public Class RaccaFrumento
    Inherits AbstractModello

    Public Shared Function Contains(ModCod As enum_ModelliPrevisionali) As Boolean
        Return ModCod = enum_ModelliPrevisionali.RaccaFrumento_Fusariosi OrElse
            ModCod = enum_ModelliPrevisionali.RaccaFrumento_RuggineBruna OrElse
            ModCod = enum_ModelliPrevisionali.RaccaFrumento_RuggineGialla OrElse
            ModCod = enum_ModelliPrevisionali.RaccaFrumento_RuggineNera OrElse
            ModCod = enum_ModelliPrevisionali.RaccaFrumento_Stagonosporiosi OrElse
            ModCod = enum_ModelliPrevisionali.RaccaFrumento_Septoria OrElse
            ModCod = enum_ModelliPrevisionali.RaccaFrumento_Fusariosi OrElse
            ModCod = enum_ModelliPrevisionali.RaccaFrumento_FusariosiSpiga OrElse
            ModCod = enum_ModelliPrevisionali.RaccaFrumento_Fusariosi2 OrElse
            ModCod = enum_ModelliPrevisionali.RaccaFrumento_Fusariosi3 OrElse
            ModCod = enum_ModelliPrevisionali.RaccaFrumento_MarciumeRosa
    End Function


    Private Class ParametriCalcolo
        Public DataSemina As DateTime?
        Public DoYSemina As Integer?
        Public DataRaccolta As DateTime?
        Public DoYRaccolta As Integer?
        Public ResistenzaVarietale As Integer?

        Public ReadOnly Property SeminaAsString As String
            Get
                Return DateOrDoY(DataSemina, DoYSemina)
            End Get
        End Property

        Public ReadOnly Property RaccoltaAsString As String
            Get
                Return DateOrDoY(DataRaccolta, DoYRaccolta)
            End Get
        End Property

        Private Function DateOrDoY(dt As DateTime?, doy As Integer?) As String

            If Not dt.HasValue Then

                If doy.HasValue Then

                    dt = New Date(1999, 1, 1) '1999 Anno non bisestile
                    dt = dt.Value.AddDays(doy - 1)
                End If
            End If

            Return If(dt.HasValue, $"{dt.Value:d MMM}", "N/A")
        End Function

        Public Sub DateFromDoY(startDate As DateTime, endDate As DateTime)

            If DataSemina.HasValue AndAlso DataRaccolta.HasValue Then

                Return
            End If

            If Not DoYSemina.HasValue OrElse Not DoYRaccolta.HasValue Then

                Return
            End If

            Dim dt As New Date(1999, 1, 1) '1999 Anno non bisestile
            Dim semina = dt.AddDays(DoYSemina.Value - 1)
            Dim raccolta = dt.AddDays(DoYRaccolta.Value - 1)

            'DataSemina nell'anno di elaborazione 
            semina = New Date(startDate.Year, semina.Month, semina.Day)
            raccolta = New Date(startDate.Year, raccolta.Month, raccolta.Day)
            If raccolta < semina Then
                '...e DataRaccolta nell'anno successivo
                raccolta = raccolta.AddYears(1)
            End If

            If semina > endDate Then
                'Semina nell'anno precedente
                semina = semina.AddYears(-1)
                raccolta = raccolta.AddYears(-1)
            End If

            DataSemina = semina
            DataRaccolta = raccolta
        End Sub

    End Class

    Public Class RisultatoVerifica
        Public Result As Boolean
        Public Message As String

        Public Sub New()
            Result = True
            Message = ""
        End Sub

        Public Shared Function Errore(msg As String) As RisultatoVerifica
            Return New RisultatoVerifica With {.Result = False, .Message = msg}
        End Function

    End Class


    Private Shared Function VerificaParametriModello(params As ParametriCalcolo) As RisultatoVerifica

        If params.DoYSemina.HasValue AndAlso params.DoYRaccolta.HasValue Then

            Return New RisultatoVerifica()
        End If

        If params.DataSemina Is Nothing OrElse params.DataRaccolta Is Nothing Then

            Return RisultatoVerifica.Errore("Data semina o data raccolta non impostate")
        End If

        If params.DataRaccolta.Value <= params.DataSemina.Value Then

            Return RisultatoVerifica.Errore("Data semina posteriore a data raccolta")
        End If

        If DateDiff(DateInterval.Day, params.DataSemina.Value, params.DataRaccolta.Value) > 365 Then

            Return RisultatoVerifica.Errore("Periodo tra data semina e data raccolta non valido")
        End If

        Return New RisultatoVerifica()
    End Function

    Public Shared Function VerificaParametri(pcm As ParametriCalcoloModello) As RisultatoVerifica

        Dim params = JsonConvert.DeserializeObject(Of ParametriCalcolo)(pcm.Modello.Params)

        Dim ris = VerificaParametriModello(params)

        If Not ris.Result Then

            Return ris
        End If

        params.DateFromDoY(pcm.Meteo.DataInizio, pcm.Meteo.DataFine)

        If params.DataSemina.Value >= pcm.Meteo.DataFine Then

            Return RisultatoVerifica.Errore("Data semina posteriore al periodo elaborazione")
        End If

        pcm.Meteo.DataInizio = params.DataSemina

        If Not params.ResistenzaVarietale.HasValue Then
            params.ResistenzaVarietale = 0
            pcm.Modello.Params = JsonConvert.SerializeObject(params)
        End If

        Return New RisultatoVerifica()
    End Function

    Public Shared Sub AggiustaParametri(pcm As ParametriCalcoloModello)

        Dim params = JsonConvert.DeserializeObject(Of ParametriCalcolo)(pcm.Modello.Params)

        If Not (
            (params.DataSemina.HasValue AndAlso params.DataRaccolta.HasValue) OrElse
            (params.DoYSemina.HasValue AndAlso params.DoYRaccolta.HasValue)
            ) Then

            'defaults???
            params.DoYSemina = 274 '1 Ottobre
            params.DoYRaccolta = 182 '1 Luglio
        End If

        If Not (params.DoYSemina.HasValue AndAlso params.DoYRaccolta.HasValue) Then
            params.DoYSemina = params.DataSemina.Value.DayOfYear - 1
            params.DoYRaccolta = params.DataRaccolta.Value.DayOfYear - 1
            params.DataSemina = Nothing
            params.DataRaccolta = Nothing
        End If

        params.DateFromDoY(pcm.Meteo.DataInizio, pcm.Meteo.DataFine)

        If params.DataSemina.Value < pcm.Meteo.DataInizio Then

            pcm.Meteo.DataInizio = params.DataSemina.Value
        End If

        pcm.Modello.Params = JsonConvert.SerializeObject(params)
    End Sub

    Public Shared Function EsponiParams(params As String) As String

        Dim objParams = JsonConvert.DeserializeObject(Of ParametriCalcolo)(params)

        Dim risver = VerificaParametriModello(objParams)

        If Not risver.Result Then

            Return risver.Message
        End If

        Dim resvar As String = "Sconosciuta"
        Select Case objParams.ResistenzaVarietale
            Case 0 : resvar = "Suscettibile"
            Case 1 : resvar = "Medio resistente"
            Case 2 : resvar = "Resistente"
        End Select

        Return $"Semina: {objParams.SeminaAsString} - Raccolta: {objParams.RaccoltaAsString} - Resistenza varietale: {resvar}"
    End Function


    Private ReadOnly CondMeteo As RaccaCondMeteo
    Private ReadOnly INF As InfProb
    Private ReadOnly LAT As Latency
    Private ReadOnly MaxRate As Decimal
    Private ReadOnly SogliaGiallo As Decimal
    Private ReadOnly SogliaRosso As Decimal
    Private ReadOnly DataSemina As DateTime
    Private ReadOnly DataRaccolta As DateTime
    Private ReadOnly CoeffResistenzaVarietale As Decimal


    Public Sub New(datiMeteo As MeteoReadOnlyList, Mod_Cod As Integer, Veg_Cod As Integer, Av_Cod As Integer, parametriAggiuntivi As String)
        MyBase.New(datiMeteo)

        'mNomeModello = "Sconosciuto"

        Dim params = JsonConvert.DeserializeObject(Of ParametriCalcolo)(parametriAggiuntivi)

        Dim ris = VerificaParametriModello(params)
        If Not ris.Result Then

            Throw New Exception("Errore in parametri calcolo modello")
        End If

        DataSemina = params.DataSemina
        DataRaccolta = params.DataRaccolta

        Select Case params.ResistenzaVarietale
            Case 0
                CoeffResistenzaVarietale = 0.9 'Suscettibile      
            Case 1
                CoeffResistenzaVarietale = 0.5 'Medio resistente   
            Case 2
                CoeffResistenzaVarietale = 0.0 'Resistente         
            Case Else
                CoeffResistenzaVarietale = 0.0
        End Select

        CondMeteo = New RaccaCondMeteo With {
            .CalcoloPeriodi_DatiMisurati = False,
            .CalcoloTempPeriodo_BagnNormale = True,
            .SogliaPioggiaCorrBagn = 0.2,
            .SogliaURCorrBagn = 90,
            .SogliaPioggiaPerBagn = 0.2
        }

        INF = Nothing
        LAT = Nothing

        SogliaGiallo = 0.3
        SogliaRosso = 0.5

        Select Case Mod_Cod

            Case enum_ModelliPrevisionali.RaccaFrumento_RuggineBruna
                'Puccinia triticina (Ruggine bruna)
                INF = New InfProb With {
                    .Yopt = 0.871,
                    .Tmin = 2.212,
                    .Topt = 18.43,
                    .Tmax = 28.95,
                    .Tn = 0.758,
                    .LWa = 1.34,
                    .LWb = 0.252,
                    .LWc = 5.268,
                    .LWy0 = 0.002
                }
                LAT = New Latency With {
                    .a = 52.936,
                    .b = -5.565,
                    .c = 0.158
                }
                MaxRate = 0.886

            Case enum_ModelliPrevisionali.RaccaFrumento_RuggineGialla
                'Puccinia striiformis (Ruggine gialla)
                INF = New InfProb With {
                    .Yopt = 0.938,
                    .Tmin = 1.243,
                    .Topt = 10.41,
                    .Tmax = 22.96,
                    .Tn = 1.416,
                    .LWa = 0.996,
                    .LWb = 0.373,
                    .LWc = 12.18,
                    .LWy0 = 0.011
                }
                LAT = New Latency With {
                    .a = 29.081,
                    .b = -4.822,
                    .c = 0.227
                }
                MaxRate = 0.997

            Case enum_ModelliPrevisionali.RaccaFrumento_RuggineNera
                'Puccinia graminis (Ruggine nera)
                INF = New InfProb With {
                    .Yopt = 0.898,
                    .Tmin = 15.0,
                    .Topt = 22.55,
                    .Tmax = 30.0,
                    .Tn = 0.798,
                    .LWa = 1.005,
                    .LWb = 0.447,
                    .LWc = 45.16,
                    .LWy0 = 0.003
                }
                LAT = New Latency With {
                    .a = 205.63,
                    .b = -17.661,
                    .c = 0.392
                }
                MaxRate = 0.714

            Case enum_ModelliPrevisionali.RaccaFrumento_Stagonosporiosi
                'Stagonospora nodorum (Stagonosporiosi)
                INF = New InfProb With {
                    .Yopt = 0.875,
                    .Tmin = 6.662,
                    .Topt = 19.09,
                    .Tmax = 28.77,
                    .Tn = 0.713,
                    .LWa = 0.95,
                    .LWb = 0.461,
                    .LWc = 283.3,
                    .LWy0 = 0.043
                }
                LAT = New Latency With {
                    .a = 106.91,
                    .b = -10.52,
                    .c = 0.288
                }
                MaxRate = 0.711

            Case enum_ModelliPrevisionali.RaccaFrumento_Septoria
                'Zymoseptoria tritici (Septoria)
                CondMeteo.CalcoloTempPeriodo_BagnNormale = False
                INF = New InfProb With {
                    .Yopt = 0.904,
                    .Tmin = 6.593,
                    .Topt = 19.78,
                    .Tmax = 30.75,
                    .Tn = 0.667,
                    .LWa = 1.012,
                    .LWb = 0.37,
                    .LWc = 19170.0,
                    .LWy0 = 0.016
                }
                LAT = New Latency With {
                    .a = 145.21,
                    .b = -12.439,
                    .c = 0.319
                }
                MaxRate = 0.064

            Case enum_ModelliPrevisionali.RaccaFrumento_Fusariosi
                'Fusarium culmorum(Fusariosi)
                INF = New InfProb With {
                    .Yopt = 0.79,
                    .Tmin = 7.59,
                    .Topt = 19.66,
                    .Tmax = 31.41,
                    .Tn = 0.798,
                    .LWa = 0.988,
                    .LWb = 0.5,
                    .LWc = 16960.0,
                    .LWy0 = 0.043
                }
                LAT = New Latency With {
                    .a = 76.005,
                    .b = -7.777,
                    .c = 0.207
                }
                MaxRate = 0.646

            Case enum_ModelliPrevisionali.RaccaFrumento_FusariosiSpiga
                'Fusarium graminearum (Fusariosi della spiga)
                INF = New InfProb With {
                    .Yopt = 0.771,
                    .Tmin = 6.863,
                    .Topt = 23.6,
                    .Tmax = 35.95,
                    .Tn = 0.96,
                    .LWa = 0.954,
                    .LWb = 0.278,
                    .LWc = 821.8,
                    .LWy0 = 0.04
                }
                LAT = New Latency With {
                    .a = 89.461,
                    .b = -7.58,
                    .c = 0.167
                }
                MaxRate = 0.167

            Case enum_ModelliPrevisionali.RaccaFrumento_Fusariosi2
                'Fusarium avenaceum (Fusariosi)
                INF = New InfProb With {
                    .Yopt = 0.796,
                    .Tmin = 10.17,
                    .Topt = 22.79,
                    .Tmax = 27.41,
                    .Tn = 0.284,
                    .LWa = 0.956,
                    .LWb = 0.279,
                    .LWc = 92.4,
                    .LWy0 = 0.058
                }
                LAT = New Latency With {
                    .a = 114.03,
                    .b = -10.336,
                    .c = 0.244
                }
                MaxRate = 0.333

            Case enum_ModelliPrevisionali.RaccaFrumento_Fusariosi3
                'Fusarium poae (Fusariosi)
                INF = New InfProb With {
                    .Yopt = 0.771,
                    .Tmin = 6.863,
                    .Topt = 23.6,
                    .Tmax = 35.95,
                    .Tn = 0.96,
                    .LWa = 0.952,
                    .LWb = 0.369,
                    .LWc = 400.0,
                    .LWy0 = 0.071
                }
                LAT = New Latency With {
                    .a = 89.461,
                    .b = -7.58,
                    .c = 0.167
                }
                MaxRate = 0.429

            Case enum_ModelliPrevisionali.RaccaFrumento_MarciumeRosa
                'Microdochium nivale (Marciume rosa invernale)
                INF = New InfProb With {
                    .Yopt = 0.804,
                    .Tmin = 0.712,
                    .Topt = 14.41,
                    .Tmax = 20.0,
                    .Tn = 0.483,
                    .LWa = 0.954,
                    .LWb = 0.278,
                    .LWc = 821.8,
                    .LWy0 = 0.04
                }
                LAT = New Latency With {
                    .a = 61.926,
                    .b = -9.13,
                    .c = 0.351
                }
                MaxRate = 0.569812704

        End Select

        If INF Is Nothing Then
            Throw New Exception("Modello sconosciuto")
        End If

    End Sub


    Protected Overrides Function _elaboraModello() As cRisultatoModello

        Dim risModello As New cRisultatoModello

        Try

            Dim elencoGG = Calcola()

            Dim output As New OutputModello

            output.aggiungiColonna("Data", GetType(DateTime), Gias.Data, "dd/MM/yyyy")
            output.aggiungiColonna("Temperatura", GetType(Decimal), Gias.Temperatura, "0.000")
            output.aggiungiColonna("UmiditaRelativa", GetType(Decimal), Gias.UmiditaRelativa, "0.000")
            output.aggiungiColonna("Pioggia", GetType(Decimal), Gias.Pioggia, "0.000")
            output.aggiungiColonna("Bagnatura", GetType(Integer), Gias.BagnaturaFogliare & " (" & Gias.Ore & ")", "0")
            output.aggiungiColonna("Bagnatura_mm", GetType(Decimal), Gias.BagnaturaFogliare & " (mm)", "0.000")
            output.aggiungiColonna("LAI", GetType(Decimal), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.RaccaModelloStd_indiceAreaFogliareRel, "0.000")
            output.aggiungiColonna("INF_Prob", GetType(Decimal), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.ProbabilitaDiInfezione_, "0.000")
            output.aggiungiColonna("IndiceRischio", GetType(Decimal), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.IndiceDiRischio, "0.000")
            output.aggiungiColonna("PeriodoLatenza", GetType(Decimal), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.RaccaModelloStd_periodoDiLatenza, "0")
            output.aggiungiColonna("IndiceRischioMediato", GetType(Decimal), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.RaccaModelloStd_indiceDiRischioMediatoSullaLatenza, "0.000")

            Dim indic As OutputIndicator = output.aggiungiIndicatore("IndiceRischioMediato_IND", {SogliaGiallo, SogliaRosso, System.Decimal.MaxValue}, OutputIndicator.enum_IndicatorType.Colors).setFieldVal("IndiceRischioMediato")

            For Each d_gg In elencoGG

                output.AddField(d_gg.Giorno)
                output.AddField(d_gg.Temp)
                output.AddField(d_gg.UmRel)
                output.AddField(d_gg.Prec)
                output.AddField(d_gg.Bagn_ore)
                output.AddField(d_gg.Bagn_mm)
                output.AddField(d_gg.LAI)
                output.AddField(d_gg.INF_Prob)
                output.AddField(d_gg.Indice_rischio)
                output.AddField(d_gg.Latent_day)
                output.AddField(d_gg.Indice_rischio_media_latenza)

                output.AddField(indic.colorForVal(d_gg.Indice_rischio_media_latenza))

                output.Commit()
            Next

            risModello.Modello_Tabella1 = output.Output({indic}.ToList(), indic.PlotBands())

            'risModello.Modello_Disclaimer =
            '    "<div style=""border: 1px solid #ccc; border-radius: 5px; padding: 15px; background-color: rgb(247,247,247);"">" &
            '    "<div style=""margin-bottom: 10px;"">" &
            '    "<span>" &
            '    String.Format(My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Racca_modelloXOttenutoAnalizzandoDatiScientificiPubblicati, mNomeModello) &
            '    My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Racca_ilModelloAnalizzaLeCondizioniMeteorologicheFavorevoliPerUnPatogeno &
            '    "</span>" &
            '    "</div>" &
            '    "<div>" &
            '    "<span style=""font-weight: bold;"">" & Gias.Attenzione & " </span>" &
            '    "<span style=""font-style: italic;"">" &
            '    My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Racca_senzaInoculoAncheInCondizioniFavorevoliLaMalattiaNonSiSviluppa &
            '    "</span> " &
            '    "<span style=""font-style: italic;"">" &
            '    My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Racca_ilModelloSiIntendeComeSupportoDSS &
            '    "</span>" &
            '    "</div>" &
            '    "</div>"

        Catch ex As Exception

            risModello = Nothing

            'uso questa funzione per ottenere il Messaggio..:
            _errore = Gias.ErroreDuranteOperazione_ & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return risModello
    End Function

    Protected Overrides Function _elaboraIndicatore_vecchio(dataInizio As Date, dataFine As Date) As cRisultatoModelloIndicatori.Indicatore
        Throw New NotImplementedException()
    End Function

    Protected Overrides Function _elaboraIndicatore(risElab As RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione) As RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione

        Dim flagOK As Boolean = False

        Try

            Dim elencoGG = Calcola()

            If elencoGG IsNot Nothing AndAlso elencoGG.Count > 0 Then

                Dim last_d_gg = elencoGG.Last()

                risElab.Fill(last_d_gg.Indice_rischio_media_latenza, 1, {SogliaGiallo, SogliaRosso})

                flagOK = True
            Else

                _errore = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Racca_modelloNonDisponibilePerIlPeriodoSelezionato
            End If

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            _errore = Gias.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        If Not flagOK Then

            risElab.Errore(_errore)
        End If

        Return risElab
    End Function


    Private Function Calcola() As List(Of DatoGiornaliero)

        Dim DatiOrari As New List(Of DatoOrario)

        Dim dto As DatoOrario
        Dim prec_dto As DatoOrario = Nothing

        For Each dm In _datiMeteo

            Dim _inf As InfProb = If(dm.DataOra.Date >= DataSemina.Date, INF, Nothing)

            dto = New DatoOrario(dm, CondMeteo, _inf, prec_dto)

            DatiOrari.Add(dto)

            prec_dto = dto
        Next

        Dim BiofixHost As Integer = DataSemina.Date.DayOfYear - 1
        Dim CropSeason As Integer = DateDiff(DateInterval.Day, DataSemina.Date, DataRaccolta.Date) - 2
        Dim gg As Integer = BiofixHost + 1
        Dim gg_rel As Decimal = (gg - BiofixHost + 1) / CropSeason
        Dim curr_dt As DateTime = DataSemina.Date
        Dim calc_lai As New LAI()
        Dim LAI As New SortedList(Of DateTime, Decimal)

        While gg_rel < 1

            LAI.Add(curr_dt, calc_lai.Calc(gg_rel))

            curr_dt = curr_dt.AddDays(1)
            gg += 1
            gg_rel = (gg - BiofixHost + 1) / CropSeason
        End While

        Dim DatiGiorno As New List(Of DatoGiornaliero)

        Dim d_gg As DatoGiornaliero = Nothing
        Dim vLAI As Decimal
        Dim hh As Decimal

        For Each d_hh In DatiOrari

            If d_gg Is Nothing OrElse d_gg.Giorno.DayOfYear <> d_hh.Meteo.DataOra.DayOfYear Then

                If d_gg IsNot Nothing Then

                    d_gg.Temp /= hh
                    d_gg.UmRel /= hh

                    Dim INF_Prob_CV As Decimal = d_gg.INF_Prob / MaxRate
                    INF_Prob_CV += INF_Prob_CV * CoeffResistenzaVarietale
                    If INF_Prob_CV > 1 Then
                        INF_Prob_CV = 0.9
                    End If

                    Dim Latent_day_CV As Decimal = LAT.Calc(INF, d_gg.Temp)
                    Dim Latent_rate_CV As Decimal = If(Latent_day_CV > 0, 1 / Latent_day_CV, 0)
                    Latent_rate_CV += Latent_rate_CV * CoeffResistenzaVarietale
                    If Latent_rate_CV > 1 Then
                        Latent_rate_CV = 0.9
                    End If
                    Latent_day_CV = If(Latent_rate_CV > 0, Math.Truncate(1 / Latent_rate_CV), 0)

                    d_gg.INF_Prob = INF_Prob_CV
                    d_gg.Latent_day = Latent_day_CV
                    d_gg.Latent_rate = Latent_rate_CV
                    d_gg.Indice_rischio = d_gg.INF_Prob + d_gg.Latent_rate - (d_gg.INF_Prob * d_gg.Latent_rate)

                    DatiGiorno.Add(d_gg)

                    d_gg.Indice_rischio_media_7_gg = MovingAvg(DatiGiorno, 7)
                    d_gg.Indice_rischio_media_latenza = MovingAvg(DatiGiorno, d_gg.Latent_day + 2)
                End If

                vLAI = 0
                LAI.TryGetValue(d_hh.Meteo.DataOra.Date, vLAI)

                d_gg = New DatoGiornaliero With {
                    .Giorno = d_hh.Meteo.DataOra.Date,
                    .Temp = d_hh.Meteo.Temp,
                    .UmRel = d_hh.Meteo.UmRel,
                    .Prec = d_hh.Meteo.Prec,
                    .Bagn_ore = d_hh.Bagnatura_x_modello,
                    .Bagn_mm = d_hh.Bagnatura_mm,
                    .LAI = vLAI,
                    .INF_Prob = d_hh.INF
                }
                hh = 1

            Else
                d_gg.Temp += d_hh.Meteo.Temp
                d_gg.UmRel += d_hh.Meteo.UmRel
                d_gg.Prec += d_hh.Meteo.Prec
                d_gg.Bagn_ore += d_hh.Bagnatura_x_modello
                d_gg.Bagn_mm += d_hh.Bagnatura_mm
                If d_hh.INF > d_gg.INF_Prob Then
                    d_gg.INF_Prob = d_hh.INF
                End If

                hh += 1
            End If
        Next

        Return DatiGiorno
    End Function


    Private Function MovingAvg(DatiGiorno As List(Of DatoGiornaliero), count As Integer) As Decimal

        Dim sum As Decimal = 0

        If DatiGiorno.Count >= count Then

            sum = DatiGiorno.GetRange(DatiGiorno.Count - count, count).Sum(Function(x) x.Indice_rischio)
        End If

        Return sum / count
    End Function



    Public Class LAI
        Private ReadOnly CGRYMax As Decimal
        Private ReadOnly CGRMin As Decimal
        Private ReadOnly CGROpt As Decimal
        Private ReadOnly CGRMax As Decimal
        Private ReadOnly CGRn As Decimal

        Public Sub New()
            CGRYMax = 1
            CGRMin = 0
            CGROpt = 0.6998
            CGRMax = 1
            CGRn = 0.6323
        End Sub

        Public Function Calc(gg_rel As Decimal) As Decimal
            '=SE(E(AB220>0;AB220<=1);CGRYMax * (((AB220 -CGRMin)/(CGROpt-CGRMin)) ^ ( CGRn * (CGROpt-CGRMin)/(CGRMax-CGROpt))) *(((CGRMax -AB220)/(CGRMax- CGROpt))^CGRn);0)
            Return CGRYMax * (((gg_rel - CGRMin) / (CGROpt - CGRMin)) ^ (CGRn * (CGROpt - CGRMin) / (CGRMax - CGROpt))) * (((CGRMax - gg_rel) / (CGRMax - CGROpt)) ^ CGRn)
        End Function
    End Class


    Private Class InfProb
        Public Yopt As Decimal
        Public Tmin As Decimal
        Public Topt As Decimal
        Public Tmax As Decimal
        Public Tn As Decimal
        Public LWa As Decimal
        Public LWb As Decimal
        Public LWc As Decimal
        Public LWy0 As Decimal

        Public Function Calc(t As Decimal, lw As Decimal) As Decimal
            '=SE(E(AA249>=BiofixDisease;U249>INFTmin;U249<INFTmax); (INFLWy0+INFLWa*(1-EXP(-INFLWb*S249))^INFLWc)* (((U249 -INFTmin)/(INFTopt-INFTmin)) ^ ( INFTn * ( INFTopt- INFTmin)/( INFTmax- INFTopt))) *((( INFTmax - U249)/( INFTmax-  INFTopt))^ INFTn);0)
            Dim result As Decimal = 0
            If Tmin < t AndAlso t < Tmax Then
                result = (LWy0 + LWa * (1.0 - Math.Exp(-LWb * lw)) ^ LWc) * (((t - Tmin) / (Topt - Tmin)) ^ (Tn * (Topt - Tmin) / (Tmax - Topt))) * (((Tmax - t) / (Tmax - Topt)) ^ Tn)
            End If
            Return result
        End Function
    End Class


    Private Class Latency
        Public a As Decimal
        Public b As Decimal
        Public c As Decimal

        Public Function Calc(infP As InfProb, t As Decimal) As Decimal
            Dim result As Decimal = 0
            If infP.Tmin < t AndAlso t < infP.Tmax Then

                result = a + b * t + c * t ^ 2
            End If

            Return Math.Truncate(result)
        End Function
    End Class


    Private Class DatoOrario
        Public ReadOnly Meteo As MeteoDSSItem
        Public ReadOnly VP As Decimal
        Public ReadOnly VP_sat As Decimal
        Public ReadOnly VPD As Decimal
        Public ReadOnly Bagnatura_mm As Decimal
        Public ReadOnly Bagnatura As Decimal
        Public ReadOnly Bagnatura_x_modello As Decimal
        Public ReadOnly Bagnatura_corretta As Decimal
        Public ReadOnly Calcolo_mm_bagnatura As Decimal
        Public ReadOnly Calcolo_periodi_secchi As Integer
        Public ReadOnly Periodo_bagnatura_senza_pioggia As Integer
        Public ReadOnly Periodo_bagnatura_dopo_pioggia As Integer
        Public ReadOnly Bagnatura_periodo As Integer 'Bagnatura normale / Bagnatura con pioggia a seconda di CondMeteo.CalcoloTempPeriodo_BagnNormale
        Public ReadOnly Somma_termica_periodo_bagnatura As Decimal
        Public ReadOnly Temp_media_periodo_bagnatura As Decimal
        Public ReadOnly INF As Decimal

        Public Sub New(dm As MeteoDSSItem, CondMeteo As RaccaCondMeteo, infP As InfProb, prec_dto As DatoOrario)

            Meteo = dm
            VP = MeteoMath.VP(Meteo.Temp, Meteo.UmRel)
            VP_sat = MeteoMath.VP(Meteo.Temp, 100)
            VPD = VP_sat - VP
            Bagnatura_mm = If(Meteo.Prec > 0, 0, 0.18D * Math.Exp(-(VPD * 6.895D) / 0.17D) + 0.0315D)
            Bagnatura = 1D / (1D + Math.Exp(-(1.35567823583444 - (63.69881694837 * VPD))))
            Bagnatura = If(Bagnatura > 0.5, 1, 0)
            Bagnatura_x_modello = If(CondMeteo.CalcoloPeriodi_DatiMisurati, Meteo.Bagn, Bagnatura)
            Bagnatura_corretta = If(
                Meteo.Prec >= CondMeteo.SogliaPioggiaCorrBagn OrElse Meteo.UmRel >= CondMeteo.SogliaURCorrBagn,
                1,
                Bagnatura_x_modello)
            Calcolo_mm_bagnatura = If(Bagnatura_corretta > 0, Bagnatura_mm, 0)
            Calcolo_periodi_secchi = 0
            Periodo_bagnatura_senza_pioggia = 0
            Periodo_bagnatura_dopo_pioggia = 0

            If prec_dto IsNot Nothing Then

                If Bagnatura_corretta > 0 Then

                    Periodo_bagnatura_senza_pioggia = prec_dto.Periodo_bagnatura_senza_pioggia + 1

                    If prec_dto.Periodo_bagnatura_dopo_pioggia + prec_dto.Meteo.Prec >= CondMeteo.SogliaPioggiaPerBagn Then

                        Periodo_bagnatura_dopo_pioggia = prec_dto.Periodo_bagnatura_dopo_pioggia + 1
                    End If
                Else

                    Calcolo_periodi_secchi = prec_dto.Calcolo_periodi_secchi + 1
                End If
            End If
            Bagnatura_periodo = If(CondMeteo.CalcoloTempPeriodo_BagnNormale, Periodo_bagnatura_senza_pioggia, Periodo_bagnatura_dopo_pioggia)
            Somma_termica_periodo_bagnatura = 0
            If prec_dto IsNot Nothing Then

                If CondMeteo.CalcoloTempPeriodo_BagnNormale Then

                    If Periodo_bagnatura_senza_pioggia > 0 Then

                        Somma_termica_periodo_bagnatura = prec_dto.Somma_termica_periodo_bagnatura + Meteo.Temp
                    End If
                Else

                    If Periodo_bagnatura_dopo_pioggia > 0 Then

                        Somma_termica_periodo_bagnatura = prec_dto.Somma_termica_periodo_bagnatura + Meteo.Temp
                    End If
                End If
            End If
            Temp_media_periodo_bagnatura = If(Bagnatura_periodo > 0, Somma_termica_periodo_bagnatura / Bagnatura_periodo, 0)

            INF = If(infP Is Nothing, 0, infP.Calc(Temp_media_periodo_bagnatura, Bagnatura_periodo))
        End Sub
    End Class


    Public Class DatoGiornaliero
        Public Giorno As DateTime
        Public Temp As Decimal
        Public UmRel As Decimal
        Public Prec As Decimal
        Public Bagn_ore As Decimal
        Public Bagn_mm As Decimal
        Public LAI As Decimal
        Public INF_Prob As Decimal
        Public Latent_day As Decimal
        Public Latent_rate As Decimal
        Public Indice_rischio As Decimal
        Public Indice_rischio_media_7_gg As Decimal
        Public Indice_rischio_media_latenza As Decimal
    End Class


End Class

