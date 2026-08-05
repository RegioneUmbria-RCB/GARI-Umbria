

Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelliPrevisionaliBIZ
Imports AgronicaCoreVarieBIZ


Public Class Agronomica30_TicchiolaturaMelo
    Inherits AbstractModello

    Private Class Calc_Helper

        Public Shared Function OreBagnaturaStensvand(ByVal x As Decimal) As Decimal
            'funzione stensvand (tmed ore bagnate evento infettivo)
            'ore minime di bagnatura perchè parta infezione
            'input x= Temp media calcolata da ora inizio ora fine evento solo ore BAGNATE LW1
            Dim a As Decimal = 45.9152082699904
            Dim b As Decimal = -5.70034515947508
            Dim c As Decimal = 0.0921545042765034
            Dim d As Decimal = 0.0255175407822107
            Dim e As Decimal = -0.00162824323232037
            Dim f As Decimal = 0.0000291609778237452
            'a + b * x + c * x ^ 2 + d * x ^ 3 + e * x ^ 4 + f * x ^ 5
            Return (((((f * x) + e) * x + d) * x + c) * x + b) * x + a
        End Function
        Public Shared Function S1(ByVal Tmed As Decimal, ByVal Heve As Decimal) As Decimal
            Return 1D / (1D + Math.Exp(2.999D - 0.067D * Tmed * Heve))
        End Function
        Public Shared Function S2(ByVal Tmed As Decimal, ByVal Heve As Decimal) As Decimal
            Dim a0 As Decimal = -2.97
            Dim a1 As Decimal = 0.4297
            Dim a2 As Decimal = -0.0061
            Dim b0 As Decimal = 0.416
            Dim b1 As Decimal = -0.0031
            Dim b2 As Decimal = -0.000245
            If (Tmed <= 20) Then
                a0 = 5.23
                a1 = -0.1226
                a2 = 0.0014
                b0 = 0.093
                b1 = 0.0112
                b2 = -0.000122
            End If
            Return 1D / (1D + Math.Exp(((a2 * Tmed + a1) * Tmed + a0) - ((b2 * Tmed + b1) * Tmed + b0) * Heve))
        End Function
        Public Shared Function S3(ByVal Tmed As Decimal, ByVal Heve As Decimal) As Decimal
            Dim a0 As Decimal = -2.13
            Dim a1 As Decimal = 0.5302
            Dim a2 As Decimal = -0.00913
            Dim b0 As Decimal = 0.405
            Dim b1 As Decimal = 0.00079
            Dim b2 As Decimal = -0.000347
            If Tmed <= 20 Then
                a0 = 6.33
                a1 = -0.0647
                a2 = -0.000317
                b0 = 0.111
                b1 = 0.0124
                b2 = -0.000181
            End If
            Return 1D / (1D + Math.Exp(((a2 * Tmed + a1) * Tmed + a0) - ((b2 * Tmed + b1) * Tmed + b0) * Heve))
        End Function
        Public Shared Function MOR1(ByVal Hdry As Decimal) As Decimal
            Return 0.263D * (1 - Math.Pow(0.97315D, Hdry))
        End Function
        Public Shared Function MOR2(ByVal Tdry As Decimal, ByVal Hdry As Decimal, ByVal RHdry As Decimal) As Decimal
            Return (-1.538D + (0.253D - 0.00694D * Tdry) * Tdry) * (1D - Math.Pow(0.977D, Hdry)) * (0.0108D * RHdry - 0.08D)
        End Function
        Public Shared Function MOR3(ByVal Tdry As Decimal, ByVal Hdry As Decimal) As Decimal
            Return (0.0028D * Hdry) * (-1.27D + (0.326D - 0.0102D * Tdry) * Tdry)
        End Function
    End Class

    Private Class Fenologia_Helper
        Private ReadOnly var_type As Decimal
        Public Sub New(ByVal vtype As Decimal)
            var_type = vtype
        End Sub
        Public Function Foglie_cluster(ByVal DDsum As Decimal) As Decimal
            'calcolo numero di foglie_rosetta in funzione di DD5
            '=7,8248*(1-(1-0,0008^(1-1,6516))*EXP(-0,0507*SE(dd<24,2193;0;dd-24,2193)))^(1/(1-1,6516))
            'per pink 24.2193+ 0  ovvero a =0
            'per fuji a = 24  ovvero 24.2193+24
            'valori provati il 24/03
            'rispetto al modello originale si parte da 1/1 ma DD5 in febb è basso
            Dim a As Decimal = 0 'per pink 24 per fuji
            Dim b As Decimal = Math.Max(0, DDsum - 24.2193 + a)
            Return 7.8248D * Math.Pow((1D - (1D - Math.Pow(0.0008D, (1D - 1.6516D))) * Math.Exp(-0.0507D * b)), (1D / (1D - 1.6516D)))
        End Function
        Public Function Foglie_leshoot(ByVal DDsum As Decimal) As Decimal
            'calcolo numero foglie_getto
            '=23,0231*(1-EXP(-0,0025*SE(dd<110,3238;0;dd-110,3238))^(0,838))
            Dim b As Decimal = Math.Max(0, DDsum - (110.3238D + var_type))
            Return 23.0231D * Math.Pow(1D - Math.Exp(-0.0025D * b), 0.838D)
        End Function
        Public Function Cm2_vege_leafKL(ByVal DDsum As Decimal) As Decimal
            'input DD5 out tessuto tot cm2 getti
            'calcolo cm2 tessuto sensibile getti rami prova 05/12/2016 ok
            If DDsum < 40 Then
                Return 0
            End If
            Dim a As Decimal = 296.802044
            Dim b As Decimal = 349.675858
            Dim c As Decimal = -6.47449
            Dim x As Decimal = Math.Min(450, DDsum)
            Return a / (1D + Math.Pow((x / b), c)) + 2D
        End Function
        Public Function Cm2_fruit_leaf1(ByVal DDsum As Decimal) As Decimal
            'calcolo cm2 tessuto totale fiore - prova 05/12/2016 ok
            'input dd5 output cm2 delle foglie rosetta fruit_leaf
            Dim DD5temp As Decimal = Math.Max(0, DDsum - (24.2193D + var_type))
            Return (7.8D * Math.Pow((1D - (1D - Math.Pow(0.008D, (1D - 1.69D))) * Math.Exp(-0.055D * DD5temp)), (1D / (1D - 1.68D)))) / 0.16D
        End Function
        Public Function Cm2_vege_leaf_SENS(ByVal DDsum As Decimal) As Decimal
            ' da DD5 a getti mod 29/11/2016
            If DDsum < 100 Then
                Return 0
            End If
            Dim a As Decimal = 0.866834 '1.4909
            Dim b As Decimal = -0.000995351 '-0.0026
            Dim c As Decimal = -0.00573799 '-0.005689
            Dim d As Decimal = 0.00000829955 '.000008167
            Dim ris = (a + b * DDsum) / (1D + (c + d * DDsum) * DDsum) - 1D
            Return Math.Max(ris, 0)
        End Function
        Public Function Cm2_fruit_leaf1_SENS(ByVal DDsum As Decimal) As Decimal
            'in- DD5 -out cm2_fruit_leaf1_SENS
            'tessuto sensibile cm2 rosetta fiore in f del totale
            'prova 05/12/2016 ok
            If DDsum < 40 Then
                Return 0
            End If
            'FN_cm2_fruit_leaf1_SENS = IIf(a > 0, a, 0)
            'Model: y=exp(a+b/x+cln(x))
            'Coefficient Data:
            Dim a As Decimal = 45.9925071919
            Dim b As Decimal = -943.842463801
            Dim c As Decimal = -7.21247465266
            Dim d As Decimal = Math.Exp(a + b / DDsum + c * Math.Log(DDsum))
            Return Math.Max(d, 0)
        End Function
    End Class

    Private Class DD5_Helper
        Private mSum As Decimal
        Private ReadOnly mDiv As Decimal
        Public Sub New(ByVal div As Decimal)
            mSum = 0
            mDiv = div
        End Sub
        Public Function Inc(ByVal tmin As Decimal, ByVal tmax As Decimal) As Decimal
            mSum += Value(tmin, tmax) / mDiv
            Return mSum
        End Function
        Public Shared Function Value(ByVal tmin As Decimal, ByVal tmax As Decimal) As Decimal
            Return Math.Max(0, ((tmax + tmin) / 2D) - 5D)
        End Function
    End Class

    Private Class DatoOrario
        Public ReadOnly Meteo As MeteoDSSItem
        Public ReadOnly LW As Integer
        Public ReadOnly YN As Integer
        Public PAT1 As Decimal
        Public PAT2 As Decimal
        Public PAT3 As Decimal
        Public somma_cm2_tess_sensibileVI As Decimal
        Public ProbInf As Decimal
        Public RischioInf As Decimal
        Public Sub New(ByVal src As MeteoDSSItem, ByVal _lw As Integer)
            Meteo = src
            LW = _lw
            YN = 0
            'YN è una stima della bagnatura con vpd p lw
            Dim vpd As Decimal = Meteo.VPD()
            If vpd = 0 OrElse LW > 0 OrElse Meteo.Prec > 0 Then

                YN = 1
            Else

                Dim U1b As Decimal = 0.3075D * Math.Pow(vpd, -0.3713D) * 100D
                If U1b > 19 Then
                    YN = 1
                End If
            End If

            PAT1 = 0
            PAT2 = 0
            PAT3 = 0
            somma_cm2_tess_sensibileVI = 0
            ProbInf = 0
            RischioInf = 0
        End Sub
    End Class

    Private Class DatoOrarioEvento
        Public Meteo As MeteoDSSItem
        Public LW As Integer
        Public Heve As Integer
        Public Hdry As Integer
        Public Tdry As Decimal
        Public RHdry As Decimal
        Public Tmed As Decimal
        Public MOR1 As Decimal
        Public MOR2 As Decimal
        Public MOR3 As Decimal
        Public Sub New(ByVal src As DatoOrario)
            Meteo = src.Meteo
            LW = src.LW
            Heve = src.LW
            Hdry = 0
        End Sub
        Public ReadOnly Property S1 As Decimal
            Get
                Return Calc_Helper.S1(Tmed, Heve)
            End Get
        End Property
        Public ReadOnly Property S2 As Decimal
            Get
                Return Calc_Helper.S2(Tmed, Heve)
            End Get
        End Property
        Public ReadOnly Property S3 As Decimal
            Get
                Return Calc_Helper.S3(Tmed, Heve)
            End Get
        End Property
        Public ReadOnly Property S1_disp As Decimal
            Get
                Return S1 - S2
            End Get
        End Property
        Public ReadOnly Property S2_disp As Decimal
            Get
                Return S2 - S3
            End Get
        End Property
        Public ReadOnly Property S3_disp As Decimal
            Get
                Return S3
            End Get
        End Property
        Public ReadOnly Property S1_INF As Decimal
            Get
                Return S1_disp * (1D - MOR1)
            End Get
        End Property
        Public ReadOnly Property S2_INF As Decimal
            Get
                Return S2_disp * (1D - MOR2)
            End Get
        End Property
        Public ReadOnly Property S3_INF As Decimal
            Get
                Return S3_disp * (1D - MOR3)
            End Get
        End Property
    End Class

    Private Class EventoInf
        Public ReadOnly Inizio As DateTime
        Public ReadOnly Fine As DateTime
        Public ReadOnly PAT3 As Decimal
        Public ReadOnly DeltaPAT_Dinamico As Decimal
        Public ReadOnly Tessuto_sensibile As Decimal
        Public ReadOnly Periodo_inf_incr As DateTime
        Public ReadOnly DeltaInc As Decimal
        Public ReadOnly LWcorr As Decimal
        Public ReadOnly Dur_min_bagn As Decimal
        Public ReadOnly DatiOrari As LinkedList(Of DatoOrarioEvento)
        Public Sub New(ByVal nodo_Rilascio As LinkedListNode(Of DatoOrario), ByVal nodo_FineEvInf As LinkedListNode(Of DatoOrario), ByVal lastPAT3 As Decimal)

            'aggiungo evento sia infettivo che no a collezione
            '- data e ora rilascio
            '- data
            '- memorizzo PATest evento
            '- per ogni evento memorizzo tsens

            Inizio = nodo_Rilascio.Value.Meteo.DataOra
            Fine = DateTime.MinValue
            Dim datafine As DateTime = DateTime.MaxValue
            If nodo_FineEvInf IsNot Nothing Then
                Fine = nodo_FineEvInf.Value.Meteo.DataOra
                datafine = nodo_FineEvInf.Value.Meteo.DataOra
            End If
            PAT3 = nodo_Rilascio.Value.PAT3
            DeltaPAT_Dinamico = nodo_Rilascio.Value.PAT3 - lastPAT3

            Tessuto_sensibile = nodo_Rilascio.Value.somma_cm2_tess_sensibileVI
            Periodo_inf_incr = DateTime.MinValue
            DatiOrari = New LinkedList(Of DatoOrarioEvento)

            'calcolo delta incubazione partendo da inizio evento
            'rilascio valido causante infezione
            'cerco la data in cui deltaINC raggiunge 1
            'Dim delta_INC As Decimal = 0
            DeltaInc = 0
            Dim denom As Decimal

            Dim nodo As LinkedListNode(Of DatoOrario) = nodo_Rilascio

            While nodo IsNot Nothing AndAlso (nodo.Value.Meteo.DataOra <= datafine OrElse DeltaInc < 1)

                If nodo.Value.Meteo.DataOra <= datafine Then

                    DatiOrari.AddLast(New DatoOrarioEvento(nodo.Value))
                End If

                If DeltaInc < 1 Then

                    denom = 8

                    If nodo.Value.Meteo.Temp <= 18 Then

                        denom = (26.4D - 1.0286D * nodo.Value.Meteo.Temp)
                    End If

                    DeltaInc += (1D / denom) / 24D

                    If DeltaInc >= 1 Then

                        Periodo_inf_incr = nodo.Value.Meteo.DataOra
                        DeltaInc = 1
                    End If
                End If

                nodo = nodo.Next
            End While

            'calcoli Heve-Hdry
            Dim curr = DatiOrari.First
            While curr IsNot Nothing
                If curr.Value.LW = 0 Then
                    'conto quanti 0 ci sono da qui
                    Dim conto0 = 1
                    Dim curr1 = curr.Next
                    While curr1 IsNot Nothing AndAlso curr1.Value.LW = 0
                        conto0 += 1
                        curr1 = curr1.Next
                    End While

                    While curr IsNot Nothing AndAlso curr.Value.LW = 0
                        If conto0 < 4 Then
                            curr.Value.Heve = 1
                        Else
                            curr.Value.Heve = 0
                            'metto valori in Hdry
                            If curr.Previous IsNot Nothing Then
                                curr.Value.Hdry = curr.Previous.Value.Hdry + 1
                            Else
                                curr.Value.Hdry = 1
                            End If
                        End If
                        curr = curr.Next
                    End While

                Else

                    curr = curr.Next
                End If
            End While

            'calcoli Tdry, RHdry, Tmedia ore bagnate ed S1 S2 S3- proporzione ascospore rilasciate nell'ora
            Dim sumT0 As Decimal = 0
            Dim cnt0 As Decimal = 0
            Dim sumTdry As Decimal = 0
            Dim sumRHdry As Decimal = 0
            Dim cntdry As Decimal = 0

            Dim Stensvand_sumLW As Decimal = 0
            Dim Stensvand_sumT_LW1 As Decimal = 0
            'controllo con funzione Stensvand se la durata di ore bagnate son sufficenti per inf (conto ore bagnate, calcolo tmed)

            curr = DatiOrari.First
            While curr IsNot Nothing

                Stensvand_sumLW += curr.Value.LW
                Stensvand_sumT_LW1 += curr.Value.Meteo.Temp * curr.Value.LW

                If curr.Previous IsNot Nothing Then
                    'trasformo Heve in ore sequenziali
                    curr.Value.Heve += curr.Previous.Value.Heve
                End If

                'calcoli mortalità ascospore parto da ora asciutta
                'fine calcoli a fine intervallo asciutto
                'la prima ora è sempre bagnata

                If curr.Value.Hdry = 0 Then
                    'medie solo se LW>0
                    sumT0 += curr.Value.Meteo.Temp
                    cnt0 += 1

                    If curr.Previous IsNot Nothing Then
                        'no bagnatura no mortalita
                        curr.Value.MOR1 = curr.Previous.Value.MOR1
                        curr.Value.MOR2 = curr.Previous.Value.MOR2
                        curr.Value.MOR3 = curr.Previous.Value.MOR3
                    End If

                Else
                    'Tdry ed RHdry

                    sumTdry += curr.Value.Meteo.Temp
                    sumRHdry += curr.Value.Meteo.UmRel
                    cntdry += 1

                    curr.Value.Tdry = sumTdry / cntdry
                    curr.Value.RHdry = sumRHdry / cntdry

                    curr.Value.MOR1 = Calc_Helper.MOR1(curr.Value.Hdry)
                    curr.Value.MOR2 = Calc_Helper.MOR2(curr.Value.Tdry, curr.Value.Hdry, curr.Value.RHdry)
                    curr.Value.MOR3 = Calc_Helper.MOR3(curr.Value.Tdry, curr.Value.Hdry)
                End If

                If cnt0 > 0 Then

                    curr.Value.Tmed = sumT0 / cnt0
                End If

                curr = curr.Next
            End While

            'durata al posto di mills considero solo ore bagnatura rossi no ?
            'dur_min_bagn = 90.96 * mediaT ^ -0.96 QUESTA è UNA STIMA DELLA DURATA QUANDO NON HO ANCORA LA FINE DELL EVENTO
            'dur = Round((miacoll.Item(miacoll.Count).fine - miacoll.Item(miacoll.Count).inizio) * 24, 0) + 1
            LWcorr = Stensvand_sumLW
            If Stensvand_sumLW > 0 Then
                Dur_min_bagn = Calc_Helper.OreBagnaturaStensvand(Stensvand_sumT_LW1 / Stensvand_sumLW)
            Else
                Dur_min_bagn = 0
            End If
        End Sub
        Public ReadOnly Property DeltaPAT_DinamicoNorm As Decimal
            Get
                Return Math.Min(DeltaPAT_Dinamico / 0.33D, 1) 'per sicurezza perchè .33 e arbitrario
            End Get
        End Property
        Public ReadOnly Property INF As Decimal
            Get
                Return DatiOrari.Last.Value.S3_INF
            End Get
        End Property
        Public ReadOnly Property RISK As Decimal
            Get
                Dim dPAT_DinamicoNorm As Decimal = DeltaPAT_DinamicoNorm
                Dim newRISK As Decimal = ((INF * 2D) + (dPAT_DinamicoNorm * 3D) + Tessuto_sensibile) / 6D
                Return Math.Min(1D, newRISK * (1D + ((LWcorr - Dur_min_bagn) / LWcorr) * dPAT_DinamicoNorm))
            End Get
        End Property
    End Class



    Private _fenologia As Fenologia_Helper
    Private _datiOrari As LinkedList(Of DatoOrario)
    Private _collez As List(Of EventoInf)
    Private _eventoInCorso As EventoInf
    Private _inizioCiclo As Date
    Private _fineCiclo As Date
    Private _progressBefore As Decimal

    Public Sub New(ByVal datiMeteo As MeteoReadOnlyList)
        MyBase.New(datiMeteo)

        _fenologia = New Fenologia_Helper(0)
        'vtype = 0  -> varietà precoce (Gala, Golden, Delicious)
        'vtype = 24 -> varietà tardiva (Fuji, gruppo Pink)

        _datiOrari = New LinkedList(Of DatoOrario)
        _collez = New List(Of EventoInf)
        _eventoInCorso = Nothing
        _inizioCiclo = DateTime.MinValue
        _fineCiclo = DateTime.MinValue
        _progressBefore = 0
    End Sub


    Protected Overrides Function _elaboraModello() As cRisultatoModello

        If Not Calcola() Then

            Return Nothing
        End If

        Dim risModello As New cRisultatoModello

        If _inizioCiclo > DateTime.MinValue Then

            Dim msg As String = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_TicchiolaturaMelo_dataInizioCicloMalattia_ & _inizioCiclo.ToShortDateString()
            If _fineCiclo > DateTime.MinValue Then
                msg &= " - " & My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_TicchiolaturaMelo_dataFineCicloMalattia_ & _fineCiclo.ToShortDateString()
            End If

            risModello.Modello_WarningMsg = "<div style='text-align: center; font-size: larger;'>" & msg & "</div>"
        End If

        'risModello.Modello_Tabella1 = OutputDatiOrari()
        'risModello.Modello_Tabella1 = OutputDEBUG(mEventi(0).DatiOrari)
        risModello.Modello_Tabella1 = OutputDatiGG()
        ' *****************************************************************************************
        ' *** DEBUG *******************************************************************************
        ' *****************************************************************************************
        risModello.Modello_Tabella2 = OutputCollezioni()
        'risModello.Modello_Tabella2 = OutputCollezioniNT()
        ' *****************************************************************************************
        ' *****************************************************************************************
        ' *****************************************************************************************
        risModello.Modello_TabellaMeteo = OutputMeteo()

        Return risModello
    End Function

    Protected Overrides Function _elaboraIndicatore_vecchio(dataInizio As Date, dataFine As Date) As cRisultatoModelloIndicatori.Indicatore

        Dim risIndic As New cRisultatoModelloIndicatori.Indicatore("Ticchiolatura", dataInizio, dataFine)

        If Calcola() Then

            If _fineCiclo > DateTime.MinValue Then

                _errore = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_TicchiolaturaMelo_raggiuntaFineCicloAvversita
                risIndic.Status = cRisultatoModelloIndicatori.Indicatore.enum_Status.Status_Error
                risIndic.StatusMsg = _errore

            Else

                Dim perc As Decimal = _datiOrari.Last.Value.ProbInf * 100D
                risIndic.AuxMsg = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.ProbabilitaDiInfezione_ & perc.ToString("0") & "%"

                risIndic.Fill(_datiOrari.Last.Value.RischioInf, 1, {0.3, 0.6}, _datiOrari.Last.Value.Meteo.DataOra, dataFine)
            End If
        Else

            risIndic.Status = cRisultatoModelloIndicatori.Indicatore.enum_Status.Status_Error
            risIndic.StatusMsg = _errore
        End If

        Return risIndic
    End Function

    Protected Overrides Function _elaboraIndicatore(ByVal risElab As RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione) As RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione

        If Calcola() Then

            If _fineCiclo > DateTime.MinValue Then

                _errore = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_TicchiolaturaMelo_raggiuntaFineCicloAvversita
                risElab.Errore(_errore)
            Else

                If _inizioCiclo > DateTime.MinValue Then

                    Dim perc As Decimal = _datiOrari.Last.Value.ProbInf * 100D
                    risElab.AuxMsg = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.ProbabilitaDiInfezione_ & perc.ToString("0") & "%"

                    risElab.Fill(_datiOrari.Last.Value.RischioInf, 1, {0.3, 0.6})
                Else

                    'WIDGET BARRA DI CARICAMENTO
                    'La barra di caricamento dovrebbe iniziare ad incrementarsi a partire dal 1° Febbraio come (sT/10)*100 
                    '(quando sT >= 10 gli pseudoteci sono maturi, ha inizio il ciclo di malattia ed il widget si accende).

                    risElab.InProgress(_progressBefore, My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_TicchiolaturaMelo_nonRaggiuntoInizioCicloAvversita)
                End If
            End If
        Else

            risElab.Errore(_errore)
        End If

        Return risElab
    End Function

    Private Class CalcST95_Helper
        Private _sT As Decimal
        Private _min_sT As Decimal
        Private _max_sT As Decimal
        Private ReadOnly _qPrec24hh As Queue(Of DatoOrario)
        Public Sub New()
            _min_sT = 5 '6.3 (07/10/2020)
            _max_sT = 10
            _sT = _min_sT
            _qPrec24hh = New Queue(Of DatoOrario)
        End Sub
        Public ReadOnly Property Progress As Decimal
            Get
                Return (_sT - _min_sT) / (_max_sT - _min_sT) * 100D
            End Get
        End Property
        Public Function Calc(ByVal d_o As DatoOrario) As Boolean
            If d_o.Meteo.DataOra.DayOfYear <= 31 Then
                Return False
            End If

            'dal 1 febbraio inizio a calcolare st
            _qPrec24hh.Enqueue(d_o)

            'incremento giornaliero maturazione pseudoteci (deltast)
            If _qPrec24hh.Count = 24 Then

                _sT += _delta_sT()

            ElseIf _qPrec24hh.Count > 24 Then

                _qPrec24hh.Dequeue()

                _sT += _delta_sT() / 24D
            End If

            Return (_sT >= _max_sT)
        End Function
        Private Function _delta_sT() As Decimal

            Dim deltast As Decimal = 0

            Dim sumT As Decimal = 0
            Dim sumYN As Decimal = 0
            Dim sumP As Decimal = 0
            Dim sumRH As Decimal = 0
            For Each q In _qPrec24hh
                sumT += q.Meteo.Temp
                sumYN += q.YN
                sumP += q.Meteo.Prec
                If q.Meteo.UmRel > 85 Then
                    sumRH += 1
                End If
            Next
            sumT /= 24D

            If sumT >= 0 AndAlso (sumRH >= 8 OrElse sumP > 0.25 OrElse sumYN >= 8) Then
                deltast = 0.0031D + sumT * (0.0546D - (0.00175D * sumT))
            End If

            Return deltast
        End Function
    End Class

    Private Class CalcPAT_Helper
        Private _STLW As Decimal
        Private _precPAT3 As Decimal
        Private ReadOnly _qPrec24hh As Queue(Of DatoOrario)
        Public Sub New()
            _STLW = 0
            _precPAT3 = 0
            _qPrec24hh = New Queue(Of DatoOrario)
        End Sub
        Public Sub Calc(ByVal flagST95 As Boolean, ByVal curr_do As DatoOrario)

            _qPrec24hh.Enqueue(curr_do)
            If _qPrec24hh.Count = 25 Then
                _qPrec24hh.Dequeue()
            End If

            If Not flagST95 Then
                Return
            End If

            If _precPAT3 >= 0.998 Then

                curr_do.PAT1 = 1
                curr_do.PAT2 = 1
                curr_do.PAT3 = 1

                Return
            End If

            'sicuramente sono passate più di 24 ore da inizio anno

            Dim sumBagn As Decimal = 0
            Dim sumTemp As Decimal = 0
            For Each q In _qPrec24hh
                sumBagn += q.LW 'q.YN
                'If q.YN > 0 AndAlso q.Meteo.Temp > 0 Then
                If q.LW > 0 AndAlso q.Meteo.Temp > 0 Then
                    sumTemp += q.Meteo.Temp
                End If
            Next

            If sumBagn > 0 Then
                Dim TYN = sumTemp / sumBagn 'modifca 08_05_2020SB temp*YN divido per 24 perchè formule si rif a dati gg
                _STLW += TYN / 24D 'Somma di TYN
            End If

            'calcolo potenziale RILASCIO ascospore dell'ora
            curr_do.PAT1 = 1D / (1D + Math.Exp(5.41D - 0.035D * _STLW)) 'interno
            curr_do.PAT2 = 1D / (1D + Math.Exp(8.27D - 0.035D * _STLW)) 'esterno
            curr_do.PAT3 = 1D / (1D + Math.Exp(6.89D - 0.035D * _STLW)) 'intermedio

            _precPAT3 = curr_do.PAT3
        End Sub
    End Class

    Private Function Calcola() As Boolean

        Dim result As Boolean = True

        Try

            Dim nodoPAT016 As LinkedListNode(Of DatoOrario) = Nothing

            Dim flagST95 As Boolean = False
            Dim calcST95 As New CalcST95_Helper
            Dim calcPAT As New CalcPAT_Helper
            Dim dd5 As New DD5_Helper(24)
            Dim qTemp As New Queue(Of Decimal)

            Dim sogliaUR As Decimal = 90 '90 se stazioni meteo esterne ed 85 se quadranti ER
            If _datiMeteo.TipoSorgente = enum_Meteo_Tiposorgente.Gias_RER_Quadranti OrElse _datiMeteo.TipoSorgente = enum_Meteo_Tiposorgente.Gias_RER Then
                sogliaUR = 85
            End If
            Dim hasLW As Boolean = _datiMeteo.SensoreBagnatura
            Dim lw As Integer

            Dim d_o As DatoOrario

            For Each dm In _datiMeteo

                lw = If(hasLW, dm.Bagn, dm.BagnEffettiva(sogliaUR))
                d_o = New DatoOrario(dm, lw)
                _datiOrari.AddLast(d_o)

                If Not flagST95 Then

                    flagST95 = calcST95.Calc(d_o)

                    If flagST95 Then

                        _inizioCiclo = d_o.Meteo.DataOra
                    Else

                        _progressBefore = calcST95.Progress
                    End If
                End If

                calcPAT.Calc(flagST95, d_o)

                If nodoPAT016 Is Nothing AndAlso Math.Round(d_o.PAT1, 3) >= 0.016 Then
                    nodoPAT016 = _datiOrari.Last 'da qui inizierò a vedere se piove
                End If

                qTemp.Enqueue(d_o.Meteo.Temp)
                If qTemp.Count = 24 Then
                    'calcoli sviluppo tessuti suscettibili
                    'calcolo dopo le prime 24 ore perchè le funzioni sono giornaliere
                    'provo a fare la media mobile delle 24 ore precedenti

                    Dim ddsum As Decimal = dd5.Inc(qTemp.Min(), qTemp.Max())

                    Dim getto_shoot_sens As Decimal = _fenologia.Cm2_vege_leaf_SENS(ddsum)  'Getto shoot sensibile
                    Dim TT_rosetta_sens As Decimal = _fenologia.Cm2_fruit_leaf1_SENS(ddsum) 'rosetta cluster sensibile
                    Dim getto_shoot As Decimal = _fenologia.Cm2_vege_leafKL(ddsum)          'getto cm2 superficie fogliare
                    Dim TT_rosetta As Decimal = _fenologia.Cm2_fruit_leaf1(ddsum)           'fiore cm2 superficie fogliare
                    Dim tot_tessuto_sens_su_totcm2 As Decimal = (getto_shoot_sens + TT_rosetta_sens) / (getto_shoot + TT_rosetta)
                    'max 91.80 cm2
                    'tess_sens valore indice ts base max
                    'per sicurezza se > 100 pongo a 100
                    d_o.somma_cm2_tess_sensibileVI = Math.Max(0, Math.Min(100, (tot_tessuto_sens_su_totcm2 / 91.8D) * 100D))

                    qTemp.Dequeue()
                End If
            Next

            If nodoPAT016 IsNot Nothing Then

                '*********************************************************************
                ' DA VERIFICARE LA LOGICA....
                ' i controlli temporali sull'evento vengono effettuati anche nel costruttore dell'evento
                ' forse qui sono superflui
                '*********************************************************************
                'Inizio a calcolare da PAT016
                Dim nodoPioggia As LinkedListNode(Of DatoOrario) = CercaPioggia(nodoPAT016)
                Dim nodoPioggiaPrec As LinkedListNode(Of DatoOrario) = Nothing
                Dim nodoRilascio As LinkedListNode(Of DatoOrario) = Nothing
                Dim nodoRilascioPrec As LinkedListNode(Of DatoOrario) = Nothing
                Dim nodoRilascioPrec_1 As LinkedListNode(Of DatoOrario) = Nothing

                Dim lastEvento_DataRilascio As DateTime = DateTime.MinValue
                Dim lastEvento_PAT3 As Decimal = 0

                While nodoPioggia IsNot Nothing AndAlso nodoPioggia.Value.PAT2 <= 0.999

                    'Tra un evento piovoso ed il successivo devono esserci più di 4 ore di interruzione di pioggia
                    If nodoRilascioPrec Is Nothing OrElse DateDiff(DateInterval.Hour, nodoRilascioPrec.Value.Meteo.DataOra, nodoPioggia.Value.Meteo.DataOra) > 4 Then

                        nodoRilascioPrec = nodoPioggia

                        'inf lo stesso giorno prendo solo la prima
                        If nodoPioggiaPrec Is Nothing OrElse nodoPioggia.Value.Meteo.DataOra.DayOfYear <> nodoPioggiaPrec.Value.Meteo.DataOra.DayOfYear Then

                            nodoRilascioPrec_1 = nodoRilascio

                            nodoRilascio = Rilascio(nodoPioggia, If(_collez.Count > 0, _collez.Last.PAT3, 0))

                            If nodoRilascio IsNot Nothing Then

                                Dim stesso_gg_rilascio As Boolean = False
                                If _collez.Count > 0 Then
                                    stesso_gg_rilascio = _collez.Last.Inizio.DayOfYear = nodoRilascio.Value.Meteo.DataOra.DayOfYear
                                End If

                                If Not stesso_gg_rilascio Then

                                    _eventoInCorso = Nothing

                                    Dim nodoFineEvInf As LinkedListNode(Of DatoOrario) = FineEventoInf(nodoRilascio, nodoRilascioPrec_1)

                                    If nodoFineEvInf IsNot Nothing Then

                                        'controllo che distino non meno di 5 ore tra inizio rilascio prec e inizio rilascio attuale
                                        If DateDiff(DateInterval.Hour, lastEvento_DataRilascio, nodoRilascio.Value.Meteo.DataOra) > 5 Then

                                            Dim ev As New EventoInf(nodoRilascio, nodoFineEvInf, lastEvento_PAT3)

                                            _collez.Add(ev)

                                            lastEvento_DataRilascio = ev.Inizio
                                            lastEvento_PAT3 = ev.PAT3
                                        End If
                                    Else

                                        _eventoInCorso = New EventoInf(nodoRilascio, Nothing, lastEvento_PAT3)
                                    End If
                                End If
                            End If
                        End If
                    End If

                    nodoPioggiaPrec = nodoPioggia
                    nodoPioggia = CercaPioggia(nodoPioggia.Next)
                End While

                If nodoPioggia IsNot Nothing Then
                    If nodoPioggia.Value.PAT2 > 0.999 Then
                        _fineCiclo = nodoPioggia.Value.Meteo.DataOra
                    End If
                End If
            End If

        Catch ex As Exception

            result = False

            'uso questa funzione per ottenere il Messaggio..:
            _errore = Gias.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return result
    End Function

    Private Function CercaPioggia(ByVal nodoPartenza As LinkedListNode(Of DatoOrario)) As LinkedListNode(Of DatoOrario)
        'Description: Prove valuto VPD per decidere se la pioggia<=2.5 fa partire rilascio
        'cerco nella colonna pioggia del mio range stabilisco se lunghe bagn si devono considerare pioggia
        'ritorno riga con ora di pioggia

        Dim nodoPioggia As LinkedListNode(Of DatoOrario) = Nothing

        Dim LIMITE_media_VPD As Decimal = 0.9 'prima 0.75 'DA CAMBIARE PER PROVE

        While nodoPioggia Is Nothing AndAlso nodoPartenza IsNot Nothing

            Dim pioggia As Decimal = nodoPartenza.Value.Meteo.Prec
            'i sensori misurano, come primo step 0.2, 0.23 o 0.25 

            'sopra lo 0.25 è pioggia...
            If pioggia > 0.25 Then

                nodoPioggia = nodoPartenza
            Else

                If pioggia > 0 Then
                    '0.2, 0.23 o 0.25 

                    Dim pioggiaPrec As Decimal = nodoPartenza.Previous.Value.Meteo.Prec
                    If pioggiaPrec > 0 Then
                        'continuo il loop
                    Else

                        Dim SumLW As Decimal = 0
                        Dim media_5_VPD As Decimal = 0
                        Dim cnt As Integer = 1
                        Dim nodo As LinkedListNode(Of DatoOrario) = nodoPartenza.Previous
                        While nodo IsNot Nothing AndAlso cnt <= 5
                            SumLW += nodo.Value.LW
                            media_5_VPD += nodo.Value.Meteo.VPD()
                            cnt += 1
                            nodo = nodo.Previous
                        End While

                        media_5_VPD /= 5D
                        If media_5_VPD <= LIMITE_media_VPD Then
                            'nebbia... continuo il loop
                        Else

                            If LIMITE_media_VPD < media_5_VPD AndAlso media_5_VPD <= 2 AndAlso SumLW = 5 Then

                                nodoPioggia = nodoPartenza
                            End If
                        End If
                    End If
                End If
            End If

            nodoPartenza = nodoPartenza.Next
        End While

        Return nodoPioggia
    End Function

    Private Function Rilascio(ByVal nodoPioggia As LinkedListNode(Of DatoOrario), ByVal LastEventoPAT3 As Decimal) As LinkedListNode(Of DatoOrario)
        'vedo se cè bagnatura interruzione ecc
        'potenzialmente da qui potrebbe partire rilascio e infezione
        'calcolo ora per ora t ipotetico di rilascio (Temp media da inizio ad ora)
        '1 vedo se ora pioggia cade di notte
        '2 vedo valore PAT
        '3 vedo valore deltaPAT
        '4 determino se partenza di giorno o di notte
        '5 calcolo durata evento infettivo: bagnato asciutto bagnato
        'cerco ora (idx) partenza evento
        'ritorno -1 se non c'e evento altrimenti ritorno la idx evento

        Dim curr = nodoPioggia.Value
        ' 1 PAT < 0.8 -> ev giorno
        ' 2 PAT > 0.8 -> ev notte
        ' 3 SRA >= 0.25 -> ev notte
        Dim PAT3 As Decimal = curr.PAT3 ' primo rilascio
        Dim SRA As Decimal = PAT3 - LastEventoPAT3
        Dim ril_GG_notte As Integer = If(SRA >= 0.28, 3, If(PAT3 < 0.8, 1, If(PAT3 >= 0.99, 3, 2)))  '0.23
        'ril_GG_notte > 1 then rilascio immediato anche di notte
        'controllo se cade di giorno/notte
        If (ril_GG_notte > 1) OrElse (7 <= curr.Meteo.DataOra.Hour AndAlso curr.Meteo.DataOra.Hour <= 18) Then
            'giorno tra le 7 e le 18
            'se PAT>=0.8 oppure SR>=0.3 allora rilascio di notte
            Return nodoPioggia
        End If

        Dim nodoRilascio As LinkedListNode(Of DatoOrario) = nodoPioggia
        Dim hh As Integer = 0
        While nodoRilascio IsNot Nothing AndAlso hh <= 14 ' bastano 13 ore perchè 24?

            If nodoRilascio.Value.LW = 1 And nodoRilascio.Value.Meteo.DataOra.Hour = 7 Then
                'bagnatura alle 7
                Return nodoRilascio
            Else
                'no bagnatura alle 7
            End If
            hh += 1
            nodoRilascio = nodoRilascio.Next
        End While

        Return Nothing
    End Function

    Private Class RangeLWHelper
        Private ore_interruzione_bagnatura As Integer
        Private ore_bagnatura As Integer
        Private cambio As Integer
        Private LW_prec As Integer
        Public ReadOnly Property OreInterruzioneBagnatura As Integer
            Get
                Return ore_interruzione_bagnatura
            End Get
        End Property
        Public ReadOnly Property OreBagnatura As Integer
            Get
                Return ore_bagnatura
            End Get
        End Property
        Public Sub New(ByVal lw As Integer)
            ore_interruzione_bagnatura = 0
            ore_bagnatura = 0
            cambio = 0
            LW_prec = lw
        End Sub
        Public Sub Check(ByVal lw As Integer)

            If LW_prec <> lw Then cambio += 1

            ore_bagnatura += lw 'bagnatura
            ore_interruzione_bagnatura += If(lw = 0, 1, 0)
            LW_prec = lw

            If cambio = 2 AndAlso ore_interruzione_bagnatura <= 3 Then
                'annullo condizioni verifica interv
                ore_interruzione_bagnatura = 0
                cambio = 0
            End If
        End Sub
        Public Sub Inc(ByVal lw As Integer)
            ore_bagnatura += lw 'bagnatura
            ore_interruzione_bagnatura += If(lw = 0, 1, 0)
        End Sub
        Public Sub Reset(ByVal lw As Integer)
            ore_interruzione_bagnatura = 0
            ore_bagnatura = 0
            cambio = 0
            LW_prec = lw
        End Sub
    End Class

    Private Function FineEventoInf(ByVal nodoRilascio As LinkedListNode(Of DatoOrario), ByVal nodoFineEventoPrec As LinkedListNode(Of DatoOrario)) As LinkedListNode(Of DatoOrario)

        Dim somma_TMP As Decimal = 0 'aggiunto da SB
        Dim somma_TMP_utile As Decimal = 0
        Dim somma_TMP_dry As Decimal = 0 'aggiunto da SB

        Dim dur As Decimal = 0
        Dim dur_min_bagn As Decimal

        Dim new_index_widget As Decimal = 0
        Dim index_widget_Less1 As Decimal
        Dim prima_ora As Boolean = False
        Dim MOR3 As Decimal = 0
        Dim MOR3_bagnatura As Decimal
        Dim fine_dt As DateTime 'Dim fine_index As Integer
        Dim allarme_interruzione As Decimal
        Dim raggiunto1prima As Boolean = False

        Dim S3 As Decimal
        Dim S3_interruzione As Decimal

        Dim PAT3_evento_prec As Decimal = 0
        If nodoFineEventoPrec IsNot Nothing Then
            PAT3_evento_prec = nodoFineEventoPrec.Value.PAT3
        End If

        Dim PAT3_dinamico As Decimal = nodoRilascio.Value.PAT3
        Dim delta_PAT_dinamico As Decimal = PAT3_dinamico - PAT3_evento_prec
        Dim delta_PAT_dinamico_norm As Decimal = delta_PAT_dinamico / 0.33D

        Dim lw_helper As New RangeLWHelper(1)  'sicuramente LW=1

        Dim d_o As DatoOrario

        'Partenza pioggia (incluse le prime ore di interruzione)
        Dim nodoCurr As LinkedListNode(Of DatoOrario) = nodoRilascio
        Dim nodoFineCiclo As LinkedListNode(Of DatoOrario) = Nothing

        While nodoFineCiclo Is Nothing AndAlso nodoCurr IsNot Nothing

            '**************************************************************************************
            ''SubEvento Modifica SB
            'If nodoCurr > nodoRilascio + 4 AndAlso nodoCurr - 4 >= 0 AndAlso
            '    mListaOrari(nodoCurr).Pioggia > 0 AndAlso
            '    mListaOrari(nodoCurr - 1).Pioggia = 0 AndAlso
            '    mListaOrari(nodoCurr - 2).Pioggia = 0 AndAlso
            '    mListaOrari(nodoCurr - 3).Pioggia = 0 AndAlso
            '    mListaOrari(nodoCurr - 4).Pioggia = 0 Then

            '    Dim dur_min_bagn_sub_evento As Decimal = Calc.OreBagnaturaStensvand(mListaOrari(nodoCurr).Temp)
            '    Dim allarme_sub_evento As Decimal = 1D / dur_min_bagn_sub_evento
            '    If (allarme_sub_evento > new_index_widget) Then
            '        'Controllareeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
            '        Dim riga_fine_ev_inf As Integer = FineEventoInf(nodoCurr, nodoRilascio)
            '    End If
            'End If
            '**************************************************************************************

            d_o = nodoCurr.Value

            lw_helper.Check(d_o.LW)

            dur += 1

            If lw_helper.OreInterruzioneBagnatura = 1 Then somma_TMP_utile = somma_TMP
            If lw_helper.OreInterruzioneBagnatura <= 4 Then somma_TMP += d_o.Meteo.Temp

            If lw_helper.OreInterruzioneBagnatura < 3 Then

                S3 = Calc_Helper.S3(somma_TMP / dur, dur)

                If lw_helper.OreInterruzioneBagnatura = 0 Then
                    S3_interruzione = S3
                End If
            End If

            If lw_helper.OreInterruzioneBagnatura = 1 Then ' qui deve restare > 0
                If prima_ora Then somma_TMP_dry = 0
                prima_ora = True
            End If

            If lw_helper.OreInterruzioneBagnatura >= 1 Then

                somma_TMP_dry += d_o.Meteo.Temp
                Dim h_dry As Decimal = lw_helper.OreInterruzioneBagnatura

                MOR3 = Calc_Helper.MOR3(somma_TMP_dry / h_dry, h_dry)
            End If

            If d_o.PAT3 <= 0.9998 Then

                dur_min_bagn = Calc_Helper.OreBagnaturaStensvand(somma_TMP / dur)

                new_index_widget = Math.Min(1D, dur / Math.Round(dur_min_bagn, 1))

                '****************************************************************
                'Modifica SB 06/04/2020
                If new_index_widget < 1 Then
                    index_widget_Less1 = new_index_widget
                Else
                    If d_o.LW = 1 Then
                        raggiunto1prima = True
                    Else
                        If Not raggiunto1prima Then
                            If index_widget_Less1 >= 0.95 Then
                                new_index_widget = index_widget_Less1
                            Else
                                new_index_widget = 0.95
                            End If
                        End If
                    End If
                End If
                '****************************************************************

                'Probabilità di infezione
                d_o.ProbInf = new_index_widget
                'Rischio infettivo
                d_o.RischioInf = ((delta_PAT_dinamico_norm * 3D) + d_o.somma_cm2_tess_sensibileVI + ((S3 * (1D - MOR3)) * 2D)) / 6D
            End If

            fine_dt = d_o.Meteo.DataOra
            allarme_interruzione = new_index_widget 'modifica SB

            If lw_helper.OreInterruzioneBagnatura > 3 Then 'devo calcolare la succ bagn
                nodoFineCiclo = nodoCurr.Previous.Previous.Previous 'mi posiziono su 0 
            End If

            nodoCurr = nodoCurr.Next
        End While

        If nodoFineCiclo Is Nothing Then
            Return Nothing
        End If


        'secondo evento bagnatura - sono nell'intervallo LW=0 con almeno 4 ore
        Dim Hdry As Decimal = lw_helper.OreInterruzioneBagnatura

        lw_helper.Reset(0)

        Dim fineCiclo_4 As DateTime = nodoFineCiclo.Value.Meteo.DataOra.AddHours(4)

        nodoCurr = nodoFineCiclo
        nodoFineCiclo = Nothing

        While nodoFineCiclo Is Nothing AndAlso nodoCurr IsNot Nothing

            d_o = nodoCurr.Value

            'da qui ci sono almeno 4ore con LW=0
            lw_helper.Inc(d_o.LW)

            If d_o.Meteo.DataOra >= fineCiclo_4 Then
                somma_TMP_dry += d_o.Meteo.Temp
                Hdry += 1
            End If

            If lw_helper.OreBagnatura > 0 Then ' significa che lw=1

                nodoFineCiclo = nodoCurr
                MOR3_bagnatura = MOR3
            Else

                MOR3 = Calc_Helper.MOR3(somma_TMP_dry / Hdry, Hdry)
                S3 = S3_interruzione * (1D - MOR3) ' modificato S3_finale

                If d_o.LW < 1 AndAlso d_o.Meteo.DataOra > fine_dt AndAlso d_o.PAT3 <= 0.9998 Then

                    'Probabilità di infezione
                    d_o.ProbInf = allarme_interruzione
                    'Rischio infettivo
                    d_o.RischioInf = ((delta_PAT_dinamico_norm * 3D) + d_o.somma_cm2_tess_sensibileVI + (S3 * 2D)) / 6D
                End If
            End If

            nodoCurr = nodoCurr.Next
        End While

        If nodoFineCiclo Is Nothing Then
            Return Nothing
        End If

        lw_helper.Reset(1)

        dur -= 4
        somma_TMP = somma_TMP_utile

        nodoCurr = nodoFineCiclo
        nodoFineCiclo = Nothing

        While nodoFineCiclo Is Nothing AndAlso nodoCurr IsNot Nothing

            d_o = nodoCurr.Value

            lw_helper.Check(d_o.LW)

            dur += 1
            somma_TMP += d_o.Meteo.Temp

            If d_o.Meteo.DataOra > fine_dt AndAlso d_o.PAT3 <= 0.9998 Then

                dur_min_bagn = Calc_Helper.OreBagnaturaStensvand(somma_TMP / dur)

                new_index_widget = Math.Min(1D, dur / Math.Round(dur_min_bagn, 1))

                '****************************************************************
                'Modifica SB 06/04/2020
                If new_index_widget < 1 Then
                    index_widget_Less1 = new_index_widget
                Else
                    If d_o.LW = 1 Then
                        raggiunto1prima = True
                    Else
                        If Not raggiunto1prima Then
                            If index_widget_Less1 >= 0.95 Then
                                new_index_widget = index_widget_Less1
                            Else
                                new_index_widget = 0.95
                            End If
                        End If
                    End If
                End If
                '****************************************************************

                S3 = Calc_Helper.S3(somma_TMP / dur, dur) * (1D - MOR3_bagnatura)

                'Probabilità di infezione
                d_o.ProbInf = new_index_widget
                'Rischio infettivo
                d_o.RischioInf = ((delta_PAT_dinamico_norm * 3D) + d_o.somma_cm2_tess_sensibileVI + (S3 * 2D)) / 3D
            End If

            If lw_helper.OreInterruzioneBagnatura > 3 Then
                nodoFineCiclo = nodoCurr.Previous.Previous.Previous.Previous 'idx - 4 'posizione ultimo LW1
            End If

            nodoCurr = nodoCurr.Next
        End While

        Return nodoFineCiclo
    End Function

    Private Class Output_gg
        Public Giorno As DateTime
        Public Tmin As Decimal
        Public Tmax As Decimal
        Public Tmed As Decimal
        Public PAT1 As Decimal
        Public PAT2 As Decimal
        Public PATest As Decimal
        Public Pioggia As Decimal
        Public SUM_LW As Decimal
        Public PATEventoPrecedente As Decimal
    End Class

    Private Function OutputDatiGG() As String

        Dim lista_gg As New List(Of Output_gg)
        Dim d_gg As Output_gg = Nothing
        Dim hh_cnt As Decimal = 0

        Dim iEvento As Integer = 0
        Dim currEvento As EventoInf = Nothing
        If _collez.Any() Then
            currEvento = _collez(iEvento)
        End If
        Dim PATEventoPrecedente As Decimal = 0

        Dim nodo = _datiOrari.First
        While nodo IsNot Nothing

            If currEvento IsNot Nothing AndAlso nodo.Value().Meteo.DataOra > currEvento.Inizio Then
                PATEventoPrecedente = currEvento.PAT3
                iEvento += 1
                If iEvento < _collez.Count Then
                    currEvento = _collez(iEvento)
                End If
            End If

            If d_gg Is Nothing OrElse d_gg.Giorno.DayOfYear <> nodo.Value.Meteo.DataOra.DayOfYear Then

                If d_gg IsNot Nothing Then

                    d_gg.Tmed /= hh_cnt
                    lista_gg.Add(d_gg)
                End If

                d_gg = New Output_gg With {
                    .Giorno = nodo.Value.Meteo.DataOra.Date,
                    .Tmin = nodo.Value.Meteo.Temp,
                    .Tmax = nodo.Value.Meteo.Temp,
                    .Tmed = nodo.Value.Meteo.Temp,
                    .PAT1 = nodo.Value.PAT1,
                    .PAT2 = nodo.Value.PAT2,
                    .PATest = nodo.Value.PAT3,
                    .Pioggia = nodo.Value.Meteo.Prec,
                    .SUM_LW = nodo.Value.LW,
                    .PATEventoPrecedente = PATEventoPrecedente
                }
                hh_cnt = 1
            Else

                With d_gg
                    .Tmin = Math.Min(.Tmin, nodo.Value.Meteo.Temp)
                    .Tmax = Math.Max(.Tmax, nodo.Value.Meteo.Temp)
                    .Tmed += nodo.Value.Meteo.Temp
                    .PAT1 = Math.Max(.PAT1, nodo.Value.PAT1)
                    .PAT2 = Math.Max(.PAT2, nodo.Value.PAT2)
                    .PATest = Math.Max(.PATest, nodo.Value.PAT3)
                    .Pioggia += nodo.Value.Meteo.Prec
                    .SUM_LW += nodo.Value.LW
                    .PATEventoPrecedente = PATEventoPrecedente
                End With
                hh_cnt += 1

            End If

            nodo = nodo.Next
        End While

        If d_gg IsNot Nothing Then

            d_gg.Tmed /= hh_cnt
            lista_gg.Add(d_gg)
        End If

        'i18n Tabella ancora in lavorazione
        Dim output As New OutputModello
        output.aggiungiColonna("Data", GetType(Date), Gias.Data, "dd/MM/yyyy")
        output.aggiungiColonna("tmin", GetType(Decimal), "Tmin", "0.00")
        output.aggiungiColonna("tmax", GetType(Decimal), "Tmax", "0.00")
        output.aggiungiColonna("tmed", GetType(Decimal), "Tmed", "0.00")
        output.aggiungiColonna("PAT1", GetType(Decimal), "PAT1", "0.00")
        output.aggiungiColonna("PAT2", GetType(Decimal), "PAT2", "0.00")
        output.aggiungiColonna("PATest", GetType(Decimal), "PATest", "0.00")
        output.aggiungiColonna("DeltaPATDinamico", GetType(Decimal), "Delta PAT Dinamico", "0.00")
        output.aggiungiColonna("pioggia", GetType(Decimal), Gias.Pioggia, "0.00")
        output.aggiungiColonna("SUM_LW", GetType(Integer), "SUM_LW", "0")
        output.aggiungiColonna("DD5", GetType(Decimal), "DD5", "0.00")
        output.aggiungiColonna("cluster_rosetta", GetType(Decimal), "cluster_rosetta", "0.00")
        output.aggiungiColonna("getto_foglie", GetType(Decimal), "getto_foglie", "0.00")
        output.aggiungiColonna("getto_shoot", GetType(Decimal), "getto_shoot", "0.00")
        output.aggiungiColonna("TT_rosetta", GetType(Decimal), "TT_rosetta", "0.00")
        output.aggiungiColonna("getto_shoot_sens", GetType(Decimal), "TSensibile getto", "0.00")
        output.aggiungiColonna("TT_rosetta_sens", GetType(Decimal), "TSensibile rosetta", "0.00")

        For Each o_gg In lista_gg

            Dim dd5 As Decimal = DD5_Helper.Value(o_gg.Tmin, o_gg.Tmax)

            output.AddField(o_gg.Giorno)
            output.AddField(o_gg.Tmin)
            output.AddField(o_gg.Tmax)
            output.AddField(o_gg.Tmed)
            output.AddField(o_gg.PAT1)
            output.AddField(o_gg.PAT2)
            output.AddField(o_gg.PATest)
            If _fineCiclo > Date.MinValue AndAlso o_gg.Giorno > _fineCiclo Then
                output.AddField(0)
            Else
                output.AddField((o_gg.PATest - o_gg.PATEventoPrecedente) * 100D)
            End If
            output.AddField(o_gg.Pioggia)
            output.AddField(o_gg.SUM_LW)
            output.AddField(dd5)
            output.AddField(_fenologia.Foglie_cluster(dd5))         'cluster_rosetta
            output.AddField(_fenologia.Foglie_leshoot(dd5))         'getto_foglie
            output.AddField(_fenologia.Cm2_vege_leafKL(dd5))        'getto cm2 superficie fogliare
            output.AddField(_fenologia.Cm2_fruit_leaf1(dd5))        'fiore cm2 superficie fogliare
            output.AddField(_fenologia.Cm2_vege_leaf_SENS(dd5))     'Getto shoot sensibile
            output.AddField(_fenologia.Cm2_fruit_leaf1_SENS(dd5))   'rosetta cluster sensibile

            output.Commit()
        Next

        Return output.Output()
    End Function

    Private Function OutputCollezioni() As String
        'i18n Pat, dose potenziale ascospore
        Dim output As New OutputModello
        output.aggiungiColonna("Valido", GetType(Integer), "Valido", "")._hidden = True
        output.aggiungiColonna("DataInizio", GetType(Date), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_TicchiolaturaMelo_inizioEvento, "dd/MM/yyyy HH")
        output.aggiungiColonna("DataFine", GetType(Date), Gias.DataFine, "dd/MM/yyyy HH", True)
        output.aggiungiColonna("Lwcorr", GetType(Decimal), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_TicchiolaturaMelo_oreBagnatura, "0")
        output.aggiungiColonna("dur_min_bagn", GetType(Decimal), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_TicchiolaturaMelo_minBagnatura, "0.00")
        output.aggiungiColonna("PAT3", GetType(Decimal), "PAT", "0.00")
        output.aggiungiColonna("deltaPAT", GetType(Decimal), "DeltaPAT (%)", "0.00")
        Dim deltapat_ind = output.aggiungiIndicatore("deltaPAT_IND", {0.25, 0.5, System.Decimal.MaxValue}, OutputIndicator.enum_IndicatorType.Numbers).setFieldVal("deltaPAT")
        output.aggiungiColonna("INF", GetType(Decimal), "INF", "0.00")
        Dim inf_ind = output.aggiungiIndicatore("INF_IND", {0.25, 0.5, System.Decimal.MaxValue}, OutputIndicator.enum_IndicatorType.Numbers).setFieldVal("INF")
        output.aggiungiColonna("Tsens", GetType(Decimal), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_TicchiolaturaMelo_indiceTSens, "0.00")
        Dim tsens_ind = output.aggiungiIndicatore("Tsens_IND", {0.25, 0.5, System.Decimal.MaxValue}, OutputIndicator.enum_IndicatorType.Numbers).setFieldVal("Tsens")
        output.aggiungiColonna("RISK", GetType(Decimal), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.IndiceDiRischio.ToUpper(), "0.00")
        Dim risk_ind = output.aggiungiIndicatore("RISK_IND", {0.3, 0.6, System.Decimal.MaxValue}, OutputIndicator.enum_IndicatorType.Colors).setFieldVal("RISK")
        'Dim group As String = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_TicchiolaturaMelo_periodoComparsaSintomiSulleFoglie
        'output.aggiungiColonna("Periodo_inf_inizio", GetType(Date), Gias.DataInizio, "dd/MM/yyyy", True)._gruppoColonne = group
        'output.aggiungiColonna("Periodo_inf_fine", GetType(Date), Gias.DataFine, "dd/MM/yyyy", True)._gruppoColonne = group
        'output.aggiungiColonna("Periodo_inf_durata", GetType(Integer), "durata", "0")._gruppoColonne = group
        output.aggiungiColonna("Perc_Inc", "% Incubazione / Comparsa sintomi su foglie a partire da")

        Dim lastCol As Integer = 14
        Dim cols(lastCol) As Object

        For Each ev In _collez

            For c = 0 To lastCol
                cols(c) = DBNull.Value
            Next

            Dim valido As Boolean = ev.LWcorr >= ev.Dur_min_bagn

            cols(0) = If(valido, 1, 0)
            cols(1) = ev.Inizio
            cols(2) = ev.Fine
            cols(3) = ev.LWcorr
            cols(4) = ev.Dur_min_bagn
            cols(5) = ev.PAT3
            cols(6) = ev.DeltaPAT_Dinamico * 100D
            cols(7) = deltapat_ind.colorForVal(ev.DeltaPAT_DinamicoNorm)

            If valido Then

                cols(8) = ev.INF
                cols(9) = inf_ind.colorForVal(ev.INF)
                cols(10) = ev.Tessuto_sensibile
                cols(11) = tsens_ind.colorForVal(ev.Tessuto_sensibile)
                cols(12) = ev.RISK
                cols(13) = risk_ind.colorForVal(ev.RISK)

                If ev.DeltaInc < 1 Then

                    cols(14) = ev.DeltaInc.ToString("0%")
                Else

                    cols(14) = ev.Periodo_inf_incr.ToString("dd/MM/yyyy")
                End If
            End If

            For c = 0 To lastCol
                output.AddField(cols(c))
            Next

            output.Commit()
        Next

        If _eventoInCorso IsNot Nothing Then

            For c = 0 To lastCol
                cols(c) = DBNull.Value
            Next

            cols(0) = 0
            cols(1) = _eventoInCorso.Inizio
            'cols(2) = DBNull.Value   '_eventoInCorso.Fine
            cols(3) = _eventoInCorso.LWcorr
            cols(4) = _eventoInCorso.Dur_min_bagn
            cols(5) = _eventoInCorso.PAT3
            cols(6) = _eventoInCorso.DeltaPAT_Dinamico * 100D
            cols(7) = deltapat_ind.colorForVal(_eventoInCorso.DeltaPAT_DinamicoNorm)

            For c = 0 To lastCol
                output.AddField(cols(c))
            Next

            output.Commit()
        End If

        Return output.Output({deltapat_ind, inf_ind, tsens_ind, risk_ind}.ToList())
    End Function

    Private Function OutputCollezioniNT() As String

        Dim grid As New OutputModelloNT.Grid

        grid.AddColumn(Of Integer)("Valido", "Valido").
            Hide()

        grid.AddColumn(Of Date)("DataInizio", My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_TicchiolaturaMelo_inizioEvento).
            Format("dd/MM/yyyy HH").
            Tooltip("Data inizio rilascio ascospore dopo pioggia utile.")

        grid.AddColumn(Of Date)("DataFine", Gias.DataFine).Format("dd/MM/yyyy HH").
            Nullable().
            Tooltip("Data di fine rilascio dipendente dalle condizioni meteo.")

        grid.AddColumn(Of Decimal)("Lwcorr", My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_TicchiolaturaMelo_oreBagnatura).
            Format("0").
            Tooltip("Ore di bagnatura durante evento di rilascio.")

        grid.AddColumn(Of Decimal)("dur_min_bagn", My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_TicchiolaturaMelo_minBagnatura).
            Format("0.00").
            Tooltip("Ore di bagnatura necessarie per causare infezione (Stensvand).")

        grid.AddColumn(Of Decimal)("PAT3", "PAT").
            Format("0.00").
            Tooltip("Potenziale di ascospore mature rilasciabili nella stagione (0÷1).")

        grid.AddColumn(Of Decimal)("deltaPAT", "DeltaPAT (%)").
            Format("0.00").
            Tooltip("Rappresenta la proporzione di ascospore rilasciate nell'aria durante l'evento di rilascio.")

        Dim deltapat_ind = grid.AddIndicator("deltaPAT_IND", {0.25, 0.5, System.Decimal.MaxValue}, OutputIndicator.enum_IndicatorType.Numbers).
            setFieldVal("deltaPAT")

        grid.AddColumn(Of Decimal)("INF", "INF").
            Format("0.00").
            Tooltip("Valore indice (0÷1): Infettività delle ascospore depositate. Indica la quantità di ascospore rilasciate che penetrano nei tessuti vegetali.")

        Dim inf_ind = grid.AddIndicator("INF_IND", {0.25, 0.5, System.Decimal.MaxValue}, OutputIndicator.enum_IndicatorType.Numbers).
            setFieldVal("INF")

        grid.AddColumn(Of Decimal)("Tsens", My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_TicchiolaturaMelo_indiceTSens).
            Format("0.00").
            Tooltip("Valore indice (0÷1): Indica la presenza di tessuti vegetali sensibili alla malattia. I tessuti vegetali sensibili sono quelli giovani della rosetta fiorale e dei getti.")

        Dim tsens_ind = grid.AddIndicator("Tsens_IND", {0.25, 0.5, System.Decimal.MaxValue}, OutputIndicator.enum_IndicatorType.Numbers).
            setFieldVal("Tsens")

        grid.AddColumn(Of Decimal)("RISK", My.Resources.AgronicaCoreModelliPrevisionaliBIZ.IndiceDiRischio.ToUpper()).
            Format("0.00").
            Tooltip("Indice di rischio globale, dipendente da entità rilascio, infettività, presenza di tessuti sensibili.")

        Dim risk_ind = grid.AddIndicator("RISK_IND", {0.3, 0.6, System.Decimal.MaxValue}, OutputIndicator.enum_IndicatorType.Colors).
            setFieldVal("RISK")

        grid.AddColumn(Of String)("Perc_Inc", "% Incubazione / Comparsa sintomi su foglie a partire da").
            Tooltip("Indica la % di incubazione o la data di fine periodo di incubazione (inizio comparsa sintomi su foglie).")

        For Each ev In _collez

            Dim row = grid.AddRow

            Dim valido As Boolean = ev.LWcorr >= ev.Dur_min_bagn

            row(0) = If(valido, 1, 0)
            row(1) = ev.Inizio
            row(2) = ev.Fine
            row(3) = ev.LWcorr
            row(4) = ev.Dur_min_bagn
            row(5) = ev.PAT3
            row(6) = ev.DeltaPAT_Dinamico * 100D
            row(7) = deltapat_ind.colorForVal(ev.DeltaPAT_DinamicoNorm)

            If valido Then

                row(8) = ev.INF
                row(9) = inf_ind.colorForVal(ev.INF)
                row(10) = ev.Tessuto_sensibile
                row(11) = tsens_ind.colorForVal(ev.Tessuto_sensibile)
                row(12) = ev.RISK
                row(13) = risk_ind.colorForVal(ev.RISK)

                If ev.DeltaInc < 1 Then

                    row(14) = ev.DeltaInc.ToString("0%")
                Else

                    row(14) = ev.Periodo_inf_incr.ToString("dd/MM/yyyy")
                End If
            End If
        Next

        If _eventoInCorso IsNot Nothing Then

            Dim row = grid.AddRow

            row(0) = 0
            row(1) = _eventoInCorso.Inizio
            'row(2) = DBNull.Value   '_eventoInCorso.Fine
            row(3) = _eventoInCorso.LWcorr
            row(4) = _eventoInCorso.Dur_min_bagn
            row(5) = _eventoInCorso.PAT3
            row(6) = _eventoInCorso.DeltaPAT_Dinamico * 100D
            row(7) = deltapat_ind.colorForVal(_eventoInCorso.DeltaPAT_DinamicoNorm)
        End If

        Return grid.ToJSONString({deltapat_ind, inf_ind, tsens_ind, risk_ind}.ToList())
    End Function

    Private Function OutputDatiOrari() As String
        'i18n Funzione non usata
        Dim output As New OutputModello
        output.aggiungiColonna("DataOra", GetType(Date), "DataOra", "dd/MM/yyyy HH")
        output.aggiungiColonna("tmed", GetType(Decimal), "T_Med", "0.00")
        output.aggiungiColonna("UR", GetType(Decimal), "UR", "0.00")
        output.aggiungiColonna("Pioggia", GetType(Decimal), "Pioggia", "0.00")
        output.aggiungiColonna("LW", GetType(Integer), "LW", "0")
        output.aggiungiColonna("PAT1", GetType(Decimal), "PAT1", "0.000")
        output.aggiungiColonna("PAT2", GetType(Decimal), "PAT2", "0.000")
        output.aggiungiColonna("PAT3", GetType(Decimal), "PAT3", "0.000")
        output.aggiungiColonna("somma_cm2_tess_sensibileVI", GetType(Decimal), "somma_cm2_tess_sensibileVI", "0.000")
        output.aggiungiColonna("ProbInf", GetType(Decimal), "ProbInf", "0.000")
        output.aggiungiColonna("RischioInf", GetType(Decimal), "RischioInf", "0.000")

        Dim nodo = _datiOrari.First
        While nodo IsNot Nothing

            output.AddField(nodo.Value.Meteo.DataOra)
            output.AddField(nodo.Value.Meteo.Temp)
            output.AddField(nodo.Value.Meteo.UmRel)
            output.AddField(nodo.Value.Meteo.Prec)
            output.AddField(nodo.Value.LW)
            output.AddField(nodo.Value.PAT1)
            output.AddField(nodo.Value.PAT2)
            output.AddField(nodo.Value.PAT3)
            output.AddField(nodo.Value.somma_cm2_tess_sensibileVI)
            output.AddField(nodo.Value.ProbInf)
            output.AddField(nodo.Value.RischioInf)

            output.Commit()

            nodo = nodo.Next
        End While

        Return output.Output()
    End Function

    Private Function OutputMeteo() As String

        Dim output As New OutputModello
        'i18n
        output.aggiungiColonna("DataOra", GetType(DateTime), Gias.DataOra, "dd/MM/yyyy HH")
        output.aggiungiColonna("Temp", GetType(Decimal), "Temperatura", "0.00")
        output.aggiungiColonna("UR", GetType(Decimal), "Umidità relativa", "0.00")
        output.aggiungiColonna("Pioggia", GetType(Decimal), Gias.Pioggia, "0.00")
        output.aggiungiColonna("Bagn", GetType(Integer), "Bagnatura", "0")

        For Each dm In _datiMeteo

            output.AddField(dm.DataOra)
            output.AddField(dm.Temp)
            output.AddField(dm.UmRel)
            output.AddField(dm.Prec)
            output.AddField(dm.Bagn)

            output.Commit()
        Next

        Return output.Output()
    End Function

    Private Function OutputDEBUG(ByRef DatiEvento As LinkedList(Of DatoOrarioEvento)) As String

        Dim output As New OutputModello
        output.aggiungiColonna("DataOra", GetType(Date), "DataOra", "dd/MM/yyyy HH")
        output.aggiungiColonna("Temp", GetType(Decimal), "Temp", "0.00")
        output.aggiungiColonna("UR", GetType(Decimal), "UR", "0.00")
        output.aggiungiColonna("Pioggia", GetType(Decimal), "Pioggia", "0.00")
        output.aggiungiColonna("LW", GetType(Integer), "LW", "0")
        output.aggiungiColonna("Heve", GetType(Integer), "Heve", "0")
        output.aggiungiColonna("Hdry", GetType(Integer), "Hdry", "0")
        output.aggiungiColonna("Tdry", GetType(Decimal), "Tdry", "0.00")
        output.aggiungiColonna("RHdry", GetType(Decimal), "RHdry", "0.00")
        output.aggiungiColonna("T_med", GetType(Decimal), "T_med", "0.00")
        output.aggiungiColonna("S1", GetType(Decimal), "S1", "0.000000")
        output.aggiungiColonna("S2", GetType(Decimal), "S2", "0.000000")
        output.aggiungiColonna("S3", GetType(Decimal), "S3", "0.000000")
        output.aggiungiColonna("MOR1", GetType(Decimal), "MOR1", "0.000000")
        output.aggiungiColonna("MOR2", GetType(Decimal), "MOR2", "0.000000")
        output.aggiungiColonna("MOR3", GetType(Decimal), "MOR3", "0.000000")

        Dim nodo = DatiEvento.First
        While nodo IsNot Nothing

            output.AddField(nodo.Value.Meteo.DataOra)
            output.AddField(nodo.Value.Meteo.Temp)
            output.AddField(nodo.Value.Meteo.UmRel)
            output.AddField(nodo.Value.Meteo.Prec)
            output.AddField(nodo.Value.LW)
            output.AddField(nodo.Value.Heve)
            output.AddField(nodo.Value.Hdry)
            output.AddField(nodo.Value.Tdry)
            output.AddField(nodo.Value.RHdry)
            output.AddField(nodo.Value.Tmed)
            output.AddField(nodo.Value.S1)
            output.AddField(nodo.Value.S2)
            output.AddField(nodo.Value.S3)
            output.AddField(nodo.Value.MOR1)
            output.AddField(nodo.Value.MOR2)
            output.AddField(nodo.Value.MOR3)

            output.Commit()

            nodo = nodo.Next
        End While

        Return output.Output()
    End Function
End Class





'Public Function Ticchiolatura(ByVal datiMeteo As List(Of AgronicaCoreMeteoBiz.InterfacciaMeteoNT.MeteoDSS)) As rispostaStandard(Of cRisultatoModello)

'    Dim r As New rispostaStandard(Of cRisultatoModello) With {
'        .RispostaStringa = New cRisultatoModello(),
'        .RispostaOK = False,
'        .Errore = ""
'    }

'    If CalcolaTicchiolatura(datiMeteo) Then

'        r.RispostaOK = True
'        If mInizioCiclo > DateTime.MinValue Then

'            Dim msg As String = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_TicchiolaturaMelo_dataInizioCicloMalattia_ & mInizioCiclo.ToShortDateString()
'            If mFineCiclo > DateTime.MinValue Then
'                msg &= " - " & My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_TicchiolaturaMelo_dataFineCicloMalattia_ & mFineCiclo.ToShortDateString()
'            End If

'            r.RispostaStringa.Modello_WarningMsg = "<div style='text-align: center; font-size: larger;'>" & msg & "</div>"
'        End If
'        'r.RispostaStringa.Modello_Tabella1 = OutputDatiOrari()
'        'r.RispostaStringa.Modello_Tabella1 = OutputDEBUG(mEventi(0).DatiOrari)
'        r.RispostaStringa.Modello_Tabella1 = OutputDatiGG()
'        r.RispostaStringa.Modello_Tabella2 = OutputCollezioni(True)
'    Else

'        r.Errore = mErrore
'    End If

'    Return r
'End Function

'Public Function Ticchiolatura_Indicatore(ByVal datiMeteo As List(Of AgronicaCoreMeteoBiz.InterfacciaMeteoNT.MeteoDSS), ByVal dataInizio As DateTime, ByVal dataFine As DateTime) As rispostaStandard(Of cRisultatoModelloIndicatori.Indicatore)

'    Dim r As New rispostaStandard(Of cRisultatoModelloIndicatori.Indicatore)
'    r.RispostaStringa = New cRisultatoModelloIndicatori.Indicatore("Ticchiolatura", dataInizio, dataFine)
'    r.RispostaOK = False
'    r.Errore = ""

'    If CalcolaTicchiolatura(datiMeteo) Then

'        r.RispostaOK = True

'        If mFineCiclo > DateTime.MinValue Then

'            r.Errore = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_TicchiolaturaMelo_raggiuntaFineCicloAvversita
'            r.RispostaStringa.Status = cRisultatoModelloIndicatori.Indicatore.enum_Status.Status_Error
'            r.RispostaStringa.StatusMsg = r.Errore

'        Else

'            Dim perc As Decimal = mDatiOrari.Last.Value.ProbInf * 100D
'            r.RispostaStringa.AuxMsg = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.ProbabilitaDiInfezione_ & perc.ToString("###") & "%"

'            r.RispostaStringa.Fill(mDatiOrari.Last.Value.RischioInf, 1, {0.25, 0.5}, mDatiOrari.Last.Value.DataOra, dataFine)
'        End If
'    Else

'        r.Errore = mErrore
'        r.RispostaStringa.Status = cRisultatoModelloIndicatori.Indicatore.enum_Status.Status_Error
'        r.RispostaStringa.StatusMsg = mErrore

'    End If

'    Return r
'End Function

#If False Then



    Private Function FineEventoInf(ByVal idx_rilascio As Integer) As Integer
        'verifico lunghezza evento infettivo: bagnato asciutto bagnato
        'cerco 3 range bagnato asciutto bagnato

        Dim idx As Integer = idx_rilascio

        Dim LW_prec As Integer
        Dim LW As Integer
        Dim cambio As Integer
        Dim ore_interruzione_bagnatura As Integer
        Dim flag As Boolean

        For Each bagnato In {True, False, True}

            LW_prec = 1 'sicuramente LW=1 per intervallo bagnato
            cambio = 0
            ore_interruzione_bagnatura = 0
            flag = False

            While idx < ListaOrari.Count And Not flag

                LW = ListaOrari(idx).Meteo.LW

                If bagnato Then
                    'intervallo bagnato
                    cambio += If(LW_prec <> LW, 1, 0)
                    ore_interruzione_bagnatura += If(LW = 0, 1, 0)
                    LW_prec = LW
                    If cambio = 2 And ore_interruzione_bagnatura <= 3 Then
                        'annullo condizioni verifica interv
                        ore_interruzione_bagnatura = 0
                        cambio = 0
                    End If
                    flag = ore_interruzione_bagnatura > 3
                Else
                    'intervallo asciutto
                    flag = LW > 0 ' esco se bagnato
                End If

                If Not flag Then
                    idx += 1
                End If

            End While

            If Not flag Then
                Return -1
            End If

            If bagnato Then
                'se ho cercato il bagnato devo tornare indietro
                idx -= 3
            End If

        Next

        Return idx - 1
    End Function



    Private Sub CreaCollezioni(ByVal PAT016_idx As Integer)

        Dim d_o As dato_orario
        'ora inizio a calcolare da PAT016_idx
        'memorizzo ultima pg dopo pat
        Dim idx_pioggiaprec As Integer = Cerco_pioggia(PAT016_idx) - 25 'inizio per sicurezza
        Dim idx_pioggia As Integer = 1
        Dim idx_rilascoprec As Integer = 0

        For x = PAT016_idx To ListaOrari.Count - 1

            d_o = ListaOrari(x)

            'cerco pioggia
            If d_o.PAT2 > 0.999 Then
                Exit For 'fine ascospore
            End If
            idx_pioggia = Cerco_pioggia(idx_pioggiaprec + 1)
            If idx_pioggia < 0 Then
                Exit For 'fine dati
            End If
            'Tra un evento piovoso ed il successivo devono esserci più di 4 ore di interruzione di pioggia
            If idx_pioggia - idx_rilascoprec >= 5 Then
                idx_rilascoprec = idx_pioggia
                'inf lo stesso giorno prendo solo la prima
                If ListaOrari(idx_pioggia).DataOra.DayOfYear <> ListaOrari(idx_pioggiaprec).DataOra.DayOfYear Then
                    'vedo se cè bagnatura interruzione ecc
                    'potenzialmente da qui potrebbe partire rilascio e infezione
                    'calcolo ora per ora t ipotetico di rilascio (Temp media da inizio ad ora)
                    '1 vedo se ora pioggia cade di notte
                    '2 vedo valore PAT
                    '3 vedo valore deltaPAT
                    '4 determino se partenza di giorno o di notte
                    '5 calcolo durata evento infettivo: bagnato asciutto bagnato
                    'cerco ora (idx) partenza evento
                    Dim idx_rilascio As Integer = 0
                    'ritorno 0 se non c'e evento altrimenti ritorno la riga evento
                    ' 1 PAT<0.8 -> ev giorno
                    ' 2 PAt>0.8 -> ev notte
                    ' 3 SRA>=0.25 -> ev notte
                    Dim valorePAT3 As Decimal = ListaOrari(idx_pioggia).PAT3 ' primo rilascio
                    Dim SRA As Decimal = valorePAT3
                    If ListaColl.Count > 0 Then
                        SRA -= ListaColl(ListaColl.Count - 1).PAT3
                    End If
                    Dim ril_GG_notte As Integer = If(SRA >= 0.23, 3, If(valorePAT3 < 0.8, 1, If(valorePAT3 >= 0.99, 3, 2)))
                    If ril_GG_notte > 1 Then
                        'rilascio immediato anche di notte
                        idx_rilascio = idx_pioggia
                    Else
                        'controllo se cade di giorno/notte
                        If (ListaOrari(idx_pioggia).DataOra.Hour >= 7) And (ListaOrari(idx_pioggia).DataOra.Hour <= 18) Then
                            'giorno tra le 7 e le 18
                            'se PAT>=0.8 oppure SR>=0.3 allora rilascio di notte
                            idx_rilascio = idx_pioggia
                        Else
                            'controllo che ci sia bagnatura al mattino alle 7
                            For u = idx_pioggia To idx_pioggia + 24
                                If ListaOrari(u).LW = 1 And ListaOrari(u).DataOra.Hour = 7 Then
                                    'bagnatura alle 7
                                    idx_rilascio = u
                                Else
                                    'no bagnatura alle 7
                                    Exit For
                                End If
                            Next
                        End If
                    End If

                    If idx_rilascio > 0 Then
                        'cerco ora fine evento
                        Dim idx_fine_ev_inf As Integer = Fine_evento_inf(idx_rilascio)
                        ' se non è finito esco
                        If idx_fine_ev_inf >= 0 Then
                            'aggiungo evento sia infettivo che no a collezione
                            aggiungi_evento(idx_rilascio, idx_fine_ev_inf)
                        End If
                    End If
                End If

            End If

            idx_pioggiaprec = idx_pioggia

        Next

    End Sub



#End If



#If False Then

Public Class Agronomica30_TicchiolaturaMelo

    Private Class Calc
        Public Shared Function OreBagnaturaStensvand(ByVal x As Decimal) As Decimal
            'funzione stensvand (tmed ore bagnate evento infettivo)
            'ore minime di bagnatura perchè parta infezione
            'input x= Temp media calcolata da ora inizio ora fine evento solo ore BAGNATE LW1
            Dim a As Decimal = 45.9152082699904
            Dim b As Decimal = -5.70034515947508
            Dim c As Decimal = 0.0921545042765034
            Dim d As Decimal = 0.0255175407822107
            Dim e As Decimal = -0.00162824323232037
            Dim f As Decimal = 0.0000291609778237452
            'a + b * x + c * x ^ 2 + d * x ^ 3 + e * x ^ 4 + f * x ^ 5
            Return (((((f * x) + e) * x + d) * x + c) * x + b) * x + a
        End Function
        Public Shared Function S1(ByVal Tmed As Decimal, ByVal Heve As Decimal) As Decimal
            Return 1D / (1D + Math.Exp(2.999D - 0.067D * Tmed * Heve))
        End Function
        Public Shared Function S2(ByVal Tmed As Decimal, ByVal Heve As Decimal) As Decimal
            Dim a0 As Decimal = -2.97
            Dim a1 As Decimal = 0.4297
            Dim a2 As Decimal = -0.0061
            Dim b0 As Decimal = 0.416
            Dim b1 As Decimal = -0.0031
            Dim b2 As Decimal = -0.000245
            If (Tmed <= 20) Then
                a0 = 5.23
                a1 = -0.1226
                a2 = 0.0014
                b0 = 0.093
                b1 = 0.0112
                b2 = -0.000122
            End If
            Return 1D / (1D + Math.Exp(((a2 * Tmed + a1) * Tmed + a0) - ((b2 * Tmed + b1) * Tmed + b0) * Heve))
        End Function
        Public Shared Function S3(ByVal Tmed As Decimal, ByVal Heve As Decimal) As Decimal
            Dim a0 As Decimal = -2.13
            Dim a1 As Decimal = 0.5302
            Dim a2 As Decimal = -0.00913
            Dim b0 As Decimal = 0.405
            Dim b1 As Decimal = 0.00079
            Dim b2 As Decimal = -0.000347
            If Tmed <= 20 Then
                a0 = 6.33
                a1 = -0.0647
                a2 = -0.000317
                b0 = 0.111
                b1 = 0.0124
                b2 = -0.000181
            End If
            Return 1D / (1D + Math.Exp(((a2 * Tmed + a1) * Tmed + a0) - ((b2 * Tmed + b1) * Tmed + b0) * Heve))
        End Function
        Public Shared Function MOR1(ByVal Hdry As Decimal) As Decimal
            Return 0.263D * (1 - Math.Pow(0.97315D, Hdry))
        End Function
        Public Shared Function MOR2(ByVal Tdry As Decimal, ByVal Hdry As Decimal, ByVal RHdry As Decimal) As Decimal
            Return (-1.538D + (0.253D - 0.00694D * Tdry) * Tdry) * (1D - Math.Pow(0.977D, Hdry)) * (0.0108D * RHdry - 0.08D)
        End Function
        Public Shared Function MOR3(ByVal Tdry As Decimal, ByVal Hdry As Decimal) As Decimal
            Return (0.0028D * Hdry) * (-1.27D + (0.326D - 0.0102D * Tdry) * Tdry)
        End Function
    End Class

    Private Class CalcSens
        Private ReadOnly var_type As Decimal
        Public Sub New(ByVal vtype As Decimal)
            var_type = vtype
        End Sub
        Public Function Foglie_cluster(ByVal DDsum As Decimal) As Decimal
            'calcolo numero di foglie_rosetta in funzione di DD5
            '=7,8248*(1-(1-0,0008^(1-1,6516))*EXP(-0,0507*SE(dd<24,2193;0;dd-24,2193)))^(1/(1-1,6516))
            'per pink 24.2193+ 0  ovvero a =0
            'per fuji a = 24  ovvero 24.2193+24
            'valori provati il 24/03
            'rispetto al modello originale si parte da 1/1 ma DD5 in febb è basso
            Dim a As Decimal = 0 'per pink 24 per fuji
            Dim b As Decimal = Math.Max(0, DDsum - 24.2193 + a)
            Return 7.8248D * Math.Pow((1D - (1D - Math.Pow(0.0008D, (1D - 1.6516D))) * Math.Exp(-0.0507D * b)), (1D / (1D - 1.6516D)))
        End Function
        Public Function Foglie_leshoot(ByVal DDsum As Decimal) As Decimal
            'calcolo numero foglie_getto
            '=23,0231*(1-EXP(-0,0025*SE(dd<110,3238;0;dd-110,3238))^(0,838))
            Dim b As Decimal = Math.Max(0, DDsum - (110.3238D + var_type))
            Return 23.0231D * Math.Pow(1D - Math.Exp(-0.0025D * b), 0.838D)
        End Function
        Public Function Cm2_vege_leafKL(ByVal DDsum As Decimal) As Decimal
            'input DD5 out tessuto tot cm2 getti
            'calcolo cm2 tessuto sensibile getti rami prova 05/12/2016 ok
            If DDsum < 40 Then
                Return 0
            End If
            Dim a As Decimal = 296.802044
            Dim b As Decimal = 349.675858
            Dim c As Decimal = -6.47449
            Dim x As Decimal = Math.Min(450, DDsum)
            Return a / (1D + Math.Pow((x / b), c)) + 2D
        End Function
        Public Function Cm2_fruit_leaf1(ByVal DDsum As Decimal) As Decimal
            'calcolo cm2 tessuto totale fiore - prova 05/12/2016 ok
            'input dd5 output cm2 delle foglie rosetta fruit_leaf
            Dim DD5temp As Decimal = Math.Max(0, DDsum - (24.2193D + var_type))
            Return (7.8D * Math.Pow((1D - (1D - Math.Pow(0.008D, (1D - 1.69D))) * Math.Exp(-0.055D * DD5temp)), (1D / (1D - 1.68D)))) / 0.16D
        End Function
        Public Function Cm2_vege_leaf_SENS(ByVal DDsum As Decimal) As Decimal
            ' da DD5 a getti mod 29/11/2016
            If DDsum < 100 Then
                Return 0
            End If
            Dim a As Decimal = 0.866834 '1.4909
            Dim b As Decimal = -0.000995351 '-0.0026
            Dim c As Decimal = -0.00573799 '-0.005689
            Dim d As Decimal = 0.00000829955 '.000008167
            Dim ris = (a + b * DDsum) / (1D + (c + d * DDsum) * DDsum) - 1D
            Return Math.Max(ris, 0)
        End Function
        Public Function Cm2_fruit_leaf1_SENS(ByVal DDsum As Decimal) As Decimal
            'in- DD5 -out cm2_fruit_leaf1_SENS
            'tessuto sensibile cm2 rosetta fiore in f del totale
            'prova 05/12/2016 ok
            If DDsum < 40 Then
                Return 0
            End If
            'FN_cm2_fruit_leaf1_SENS = IIf(a > 0, a, 0)
            'Model: y=exp(a+b/x+cln(x))
            'Coefficient Data:
            Dim a As Decimal = 45.9925071919
            Dim b As Decimal = -943.842463801
            Dim c As Decimal = -7.21247465266
            Dim d As Decimal = Math.Exp(a + b / DDsum + c * Math.Log(DDsum))
            Return Math.Max(d, 0)
        End Function
        Public Function Vindice_tess_sens(ByVal DDsum As Decimal) As Decimal

            Dim getto_shoot As Decimal = Cm2_vege_leafKL(DDsum) 'getto cm2 superficie fogliare
            Dim TT_rosetta As Decimal = Cm2_fruit_leaf1(DDsum) ' fiore cm2 superficie fogliare
            Dim getto_shoot_sens As Decimal = Cm2_vege_leaf_SENS(DDsum) 'Getto shoot sensibile
            Dim TT_rosetta_sens As Decimal = Cm2_fruit_leaf1_SENS(DDsum) 'rosetta cluster sensibile
            Dim tot_tessuto_sens_su_totcm2 As Decimal = (getto_shoot_sens + TT_rosetta_sens) / (getto_shoot + TT_rosetta) '(somma rosSENS+gettiSENS)/tot cm2
            'max 91.80 cm2
            'tess_sens valore indice ts base max
            'per sicurezza se >100 pongo a 100
            If tot_tessuto_sens_su_totcm2 <= 0 Then
                Return 0
            End If
            Return Math.Min(100, (tot_tessuto_sens_su_totcm2 / 91.8D) * 100D)

        End Function
    End Class

    Private Class DD5
        Private mSum As Decimal
        Private ReadOnly mDiv As Decimal
        Public Sub New(ByVal div As Decimal)
            mSum = 0
            mDiv = div
        End Sub
        Public Function Inc(ByVal tmin As Decimal, ByVal tmax As Decimal)
            mSum += Math.Max(0, ((tmax + tmin) / 2D) - 5D) / mDiv
            Return mSum
        End Function
    End Class

    Private Class DatoMeteo
        Public DataOra As DateTime
        Public Temp As Decimal
        Public UmRel As Decimal
        Public Pioggia As Decimal
        Public LW As Integer
        Public Sub New(ByVal src As AgronicaCoreMeteoBiz.InterfacciaMeteoNT.MeteoDSS)
            DataOra = src.DataOra
            Temp = src.Temp
            UmRel = src.UmRel
            Pioggia = src.Prec
            LW = If(src.Bagn > 0 Or src.Prec > 0, 1, 0)
        End Sub
        Public Sub New(ByVal src As DatoMeteo)
            DataOra = src.DataOra
            Temp = src.Temp
            UmRel = src.UmRel
            Pioggia = src.Pioggia
            LW = src.LW
        End Sub
        Public Function VPD() As Decimal  'vapour pressure deficit
            Return (1D - (UmRel / 100D)) * (6.11D * Math.Exp((17.47D * Temp) / (239D + Temp)))
        End Function
        Public Function YN() As Integer
            'YN è una stima della bagnatura con vpd p lw

            Dim _vpd As Decimal = VPD()

            If _vpd = 0 OrElse LW > 0 OrElse Pioggia > 0 Then
                Return 1
            End If

            Dim U1b As Decimal = 0.3075D * Math.Pow(_vpd, -0.3713D) * 100D
            If U1b > 19 Then
                Return 1
            End If

            Return 0
        End Function
    End Class

    Private Class DatoOrario
        Inherits DatoMeteo

        Public PAT1 As Decimal
        Public PAT2 As Decimal
        Public PAT3 As Decimal
        Public somma_cm2_tess_sensibileVI As Decimal
        Public ProbInf As Decimal
        Public RischioInf As Decimal
        Public Sub New(ByVal src As AgronicaCoreMeteoBiz.InterfacciaMeteoNT.MeteoDSS)
            MyBase.New(src)

            PAT1 = 0
            PAT2 = 0
            PAT3 = 0
            somma_cm2_tess_sensibileVI = 0
            ProbInf = 0
            RischioInf = 0
        End Sub
    End Class

    Private Class DatoOrarioColl
        Inherits DatoMeteo

        Public Heve As Integer
        Public Hdry As Integer
        Public Tdry As Decimal
        Public RHdry As Decimal
        Public Tmed As Decimal
        Public MOR1 As Decimal
        Public MOR2 As Decimal
        Public MOR3 As Decimal
        Public Sub New(ByVal src As DatoOrario)
            MyBase.New(src)

            Heve = src.LW
            Hdry = 0
        End Sub
        Public Function S1() As Decimal
            Return Calc.S1(Tmed, Heve)
        End Function
        Public Function S2() As Decimal
            Return Calc.S2(Tmed, Heve)
        End Function
        Public Function S3() As Decimal
            Return Calc.S3(Tmed, Heve)
        End Function
        Public Function S1_disp() As Decimal
            Return S1() - S2()
        End Function
        Public Function S2_disp() As Decimal
            Return S2() - S3()
        End Function
        Public Function S3_disp() As Decimal
            Return S3()
        End Function
        Public Function S1_INF() As Decimal
            Return S1_disp() * (1D - MOR1)
        End Function
        Public Function S2_INF() As Decimal
            Return S2_disp() * (1D - MOR2)
        End Function
        Public Function S3_INF() As Decimal
            Return S3_disp() * (1D - MOR3)
        End Function
    End Class

    Private Class EventoInf
        Public Cond_5_Ore As Boolean
        Public Cond_Durata_Stensvad As Boolean
        Public Cond_Stessa_Data As Boolean
        Public Inizio As DateTime
        Public Fine As DateTime
        Public LWcorr As Decimal
        Public Dur_min_bagn As Decimal
        Public PAT3 As Decimal
        Public DeltaPAT3 As Decimal
        Public Tessuto_sensibile As Decimal
        Public Periodo_inf_incr As DateTime

        Public ListaOrari As List(Of DatoOrarioColl)

        Public Sub New(ByVal lista_orari As List(Of DatoOrario), ByVal idx_rilascio As Integer, ByVal idx_fine_ev_inf As Integer, ByVal last_evento As EventoInf)

            Inizio = lista_orari(idx_rilascio).DataOra ' data e ora rilascio
            Fine = lista_orari(idx_fine_ev_inf).DataOra 'data
            PAT3 = lista_orari(idx_rilascio).PAT3 'memorizzo PATest evento
            DeltaPAT3 = PAT3
            Tessuto_sensibile = lista_orari(idx_rilascio).somma_cm2_tess_sensibileVI 'per ogni evento memorizzo tsens

            ListaOrari = New List(Of DatoOrarioColl)

            'controllo con funzione Stensvand se la durata di ore bagnate son sufficenti per inf
            'conto ore bagnate
            'calcolo tmed
            Dim sommaLW As Decimal = 0
            Dim sommaT_LW1 As Decimal = 0
            For u = idx_rilascio To idx_fine_ev_inf

                Dim d_o = lista_orari(u)

                ListaOrari.Add(New DatoOrarioColl(d_o))

                sommaLW += d_o.LW
                sommaT_LW1 += d_o.Temp * d_o.LW
            Next
            LWcorr = sommaLW
            'durata al posto di mills considero solo ore bagnatura rossi no
            'dur_min_bagn_old = 90.96 * mediaT ^ -0.96
            Dur_min_bagn = Calc.OreBagnaturaStensvand(sommaT_LW1 / sommaLW)

            Cond_5_Ore = True
            Cond_Durata_Stensvad = sommaLW >= Dur_min_bagn
            Cond_Stessa_Data = False

            If last_evento IsNot Nothing Then

                DeltaPAT3 -= last_evento.PAT3

                'controllo che distino non meno di 5 ore tra inizio rilascio prec e inizio rilascio attuale
                Cond_5_Ore = DateDiff(DateInterval.Hour, last_evento.Inizio, Inizio) > 5
                'controllo se il gg dell'evento n = eventon-1
                Cond_Stessa_Data = Inizio.DayOfYear = last_evento.Inizio.DayOfYear  'stessa data !
            End If

            'durata al posto di mills considero solo ore bagnatura rossi no ?
            'dur_min_bagn = 90.96 * mediaT ^ -0.96 QUESTA è UNA STIMA DELLA DURATA QUANDO NON HO ANCORA LA FINE DELL EVENTO
            'dur = Round((miacoll.Item(miacoll.Count).fine - miacoll.Item(miacoll.Count).inizio) * 24, 0) + 1

            Periodo_inf_incr = DateTime.MinValue

            'calcolo delta incubazione partendo da inizio evento
            'rilascio valido causante infezione
            'cerco la data in cui deltaINC raggiunge 1
            Dim idx As Integer = idx_rilascio
            Dim delta_INC As Decimal = 0
            While idx < lista_orari.Count And delta_INC < 1
                Dim temp As Decimal = lista_orari(idx).Temp
                'deltaINC += 1 / (26.4 - 1.0286 * temp) / 24   ' VECCHIO CALCOLO
                'mettere se temp>18 allora ((1/8)/24) altrimenti 1 / (26.4 - 1.0286 * temp) / 24 variazione del 11/112/2017
                delta_INC += If(temp > 18, (1D / 8D) / 24D, 1D / (26.4D - 1.0286D * temp) / 24D)
                If delta_INC >= 1 Then
                    Periodo_inf_incr = lista_orari(idx).DataOra
                End If
                idx += 1
            End While

        End Sub
        Public Sub CalcolaListaOrari()
            'calcoli Heve-Hdry
            Dim conto0 As Integer
            Dim idx As Integer = 0
            Dim idx1 As Integer
            While idx < ListaOrari.Count
                If ListaOrari(idx).LW = 0 Then
                    'conto quanti 0 ci sono da qui
                    conto0 = 1
                    idx1 = idx + 1
                    While idx1 < ListaOrari.Count AndAlso ListaOrari(idx1).LW = 0
                        conto0 += 1
                        idx1 += 1
                    End While

                    While idx < idx1
                        If conto0 < 4 Then
                            ListaOrari(idx).Heve = 1
                        Else
                            ListaOrari(idx).Heve = 0
                            'metto valori in Hdry
                            ListaOrari(idx).Hdry = ListaOrari(idx - 1).Hdry + 1
                        End If
                        idx += 1
                    End While

                Else
                    idx += 1
                End If

            End While

            Dim doc As DatoOrarioColl
            Dim sumT0 As Decimal = 0
            Dim cnt0 As Decimal = 0
            Dim sumTdry As Decimal = 0
            Dim sumRHdry As Decimal = 0
            Dim cntdry As Decimal = 0

            'calcoli Tdry, RHdry, Tmedia ore bagnate ed S1 S2 S3- proporzione ascospore rilasciate nell'ora
            For idx = 0 To ListaOrari.Count - 1

                doc = ListaOrari(idx)

                If idx > 0 Then
                    'trasformo Heve in ore sequenziali
                    doc.Heve += ListaOrari(idx - 1).Heve
                End If

                'calcoli mortalità ascospore parto da ora asciutta
                'fine calcoli a fine intervallo asciutto
                'la prima ora è sempre bagnata

                If doc.Hdry = 0 Then
                    'medie solo se LW>0
                    sumT0 += doc.Temp
                    cnt0 += 1

                    If idx > 0 Then
                        'no bagnatura no mortalita
                        doc.MOR1 = ListaOrari(idx - 1).MOR1
                        doc.MOR2 = ListaOrari(idx - 1).MOR2
                        doc.MOR3 = ListaOrari(idx - 1).MOR3
                    End If

                Else
                    'Tdry ed RHdry

                    sumTdry += doc.Temp
                    sumRHdry += doc.UmRel
                    cntdry += 1

                    doc.Tdry = sumTdry / cntdry
                    doc.RHdry = sumRHdry / cntdry

                    doc.MOR1 = Calc.MOR1(doc.Hdry)
                    doc.MOR2 = Calc.MOR2(doc.Tdry, doc.Hdry, doc.RHdry)
                    doc.MOR3 = Calc.MOR3(doc.Tdry, doc.Hdry)
                End If

                doc.Tmed = sumT0 / cnt0
            Next

        End Sub
        Public Function INF() As Decimal
            Return ListaOrari(ListaOrari.Count - 1).S3_INF
        End Function
        Public Function RISK() As Decimal
            Dim VI33_deltaPAT As Decimal = Math.Min(DeltaPAT3 / 0.33D, 1) 'per sicurezza perchè .33 e arbitrario
            Return (INF() + VI33_deltaPAT + Tessuto_sensibile) / 3D
        End Function
    End Class



    Private mCalcSens As CalcSens

    Private mListaOrari As List(Of DatoOrario)
    Private mListaEventi As List(Of EventoInf)

    Private mInizioCiclo As Date
    Private mFineCiclo As Date
    Private mErrore As String

    Public Sub New()
        mCalcSens = New CalcSens(0)
        'vtype = 0  -> varietà precoce (Gala, Golden, Delicious)
        'vtype = 24 -> varietà tardiva (Fuji, gruppo Pink)

        mListaOrari = New List(Of DatoOrario)
        mListaEventi = New List(Of EventoInf)

        mInizioCiclo = DateTime.MinValue
        mFineCiclo = DateTime.MinValue

        mErrore = ""
    End Sub

    Private Function CercoPioggia(ByVal partenza As Integer) As Integer
        'Description: Prove valuto VPD per decidere se la pioggia<=2.5 fa partire rilascio
        'cerco nella colonna pioggia del mio range stabilisco se lunghe bagn si devono considerare pioggia
        'ritorno riga con ora di pioggia

        Dim LIMITE_media_VPD As Decimal = 0.9 ' prima 0.75 'DA CAMBIARE PER PROVE

        Dim idxPioggia = -1
        While idxPioggia < 0 AndAlso partenza < mListaOrari.Count

            Dim pioggia As Decimal = mListaOrari(partenza).Pioggia
            'i sensori misurano, come primo step 0.2, 0.23 o 0.25 

            'sopra lo 0.25 è pioggia...
            If pioggia > 0.25 Then

                idxPioggia = partenza

            Else

                If pioggia > 0 Then
                    '0.2, 0.23 o 0.25 

                    Dim pioggiaPrec As Decimal = mListaOrari(partenza - 1).Pioggia
                    If pioggiaPrec > 0 Then
                        'continuo il loop
                    Else

                        Dim SumLW As Decimal = 0
                        Dim media_5_V_PD As Decimal = 0
                        For x = partenza - 5 To partenza - 1
                            SumLW += mListaOrari(x).LW
                            media_5_V_PD += mListaOrari(x).VPD
                        Next
                        media_5_V_PD /= 5D
                        If media_5_V_PD <= LIMITE_media_VPD Then
                            'nebbia... continuo il loop
                        Else

                            If (LIMITE_media_VPD < media_5_V_PD AndAlso media_5_V_PD < 2) AndAlso SumLW = 5 Then

                                idxPioggia = partenza

                            End If
                        End If
                    End If
                End If
            End If

            partenza += 1
        End While

        Return idxPioggia
    End Function

    Private Function Rilascio(ByVal idx_pioggia As Integer) As Integer
        'vedo se cè bagnatura interruzione ecc
        'potenzialmente da qui potrebbe partire rilascio e infezione
        'calcolo ora per ora t ipotetico di rilascio (Temp media da inizio ad ora)
        '1 vedo se ora pioggia cade di notte
        '2 vedo valore PAT
        '3 vedo valore deltaPAT
        '4 determino se partenza di giorno o di notte
        '5 calcolo durata evento infettivo: bagnato asciutto bagnato
        'cerco ora (idx) partenza evento
        'ritorno -1 se non c'e evento altrimenti ritorno la idx evento

        Dim curr = mListaOrari(idx_pioggia)
        ' 1 PAT < 0.8 -> ev giorno
        ' 2 PAT > 0.8 -> ev notte
        ' 3 SRA >= 0.25 -> ev notte
        Dim PAT3 As Decimal = curr.PAT3 ' primo rilascio
        Dim SRA As Decimal = PAT3
        If mListaEventi.Count > 0 Then
            SRA -= mListaEventi.Last.PAT3
        End If
        Dim ril_GG_notte As Integer = If(SRA >= 0.23, 3, If(PAT3 < 0.8, 1, If(PAT3 >= 0.99, 3, 2)))
        'ril_GG_notte > 1 then rilascio immediato anche di notte
        'controllo se cade di giorno/notte
        If (ril_GG_notte > 1) OrElse (7 <= curr.DataOra.Hour AndAlso curr.DataOra.Hour <= 18) Then
            'giorno tra le 7 e le 18
            'se PAT>=0.8 oppure SR>=0.3 allora rilascio di notte
            Return idx_pioggia
        End If

        Dim idx_rilascio As Integer = idx_pioggia
        While idx_rilascio < mListaOrari.Count AndAlso idx_rilascio <= idx_pioggia + 14 ' bastano 13 ore perchè 24?

            If mListaOrari(idx_rilascio).LW = 1 And mListaOrari(idx_rilascio).DataOra.Hour = 7 Then
                'bagnatura alle 7
                Return idx_rilascio
            Else
                'no bagnatura alle 7
            End If

            idx_rilascio += 1
        End While

        Return -1
    End Function

    Private Class ChkLW

        Private ore_interruzione_bagnatura As Integer
        Private ore_bagnatura As Integer
        Private cambio As Integer
        Private LW_prec As Integer = 1 'sicuramente LW=1
        Public ReadOnly Property OreInterruzioneBagnatura As Integer
            Get
                Return ore_interruzione_bagnatura
            End Get
        End Property
        Public ReadOnly Property OreBagnatura As Integer
            Get
                Return ore_bagnatura
            End Get
        End Property
        Public Sub New(ByVal lw As Integer)
            ore_interruzione_bagnatura = 0
            ore_bagnatura = 0
            cambio = 0
            LW_prec = lw
        End Sub
        Public Sub Check(ByVal lw As Integer)

            If LW_prec <> lw Then cambio += 1

            ore_bagnatura += lw 'bagnatura
            ore_interruzione_bagnatura += If(lw = 0, 1, 0)
            LW_prec = lw

            If cambio = 2 AndAlso ore_interruzione_bagnatura <= 3 Then
                'annullo condizioni verifica interv
                ore_interruzione_bagnatura = 0
                cambio = 0
            End If
        End Sub
        Public Sub Inc(ByVal lw As Integer)
            ore_bagnatura += lw 'bagnatura
            ore_interruzione_bagnatura += If(lw = 0, 1, 0)
        End Sub
        Public Sub Reset(ByVal lw As Integer)
            ore_interruzione_bagnatura = 0
            ore_bagnatura = 0
            cambio = 0
            LW_prec = lw
        End Sub
    End Class

    Private Function FineEventoInf(ByVal partenza_ora_pioggia As Integer, ByVal riga_fine_evento_prec As Integer) As Integer

        Dim somma_TMP As Decimal = 0 'aggiunto da SB
        Dim somma_TMP_utile As Decimal = 0
        Dim somma_TMP_dry As Decimal = 0 'aggiunto da SB

        Dim dur As Decimal = 0
        Dim dur_min_bagn As Decimal

        Dim new_index_widget As Decimal = 0
        Dim prima_ora As Boolean = False
        Dim MOR3 As Decimal = 0
        Dim MOR3_bagnatura As Decimal
        Dim fine_index As Integer
        Dim allarme_interruzione As Decimal

        Dim Hdry As Decimal
        Dim S3 As Decimal
        Dim S3_interruzione As Decimal

        Dim PAT3_evento_prec As Decimal = 0
        If riga_fine_evento_prec >= 0 Then
            PAT3_evento_prec = mListaOrari(riga_fine_evento_prec).PAT3
        End If

        Dim PAT3_dinamico As Decimal = mListaOrari(partenza_ora_pioggia).PAT3
        Dim delta_PAT_dinamico As Decimal = PAT3_dinamico - PAT3_evento_prec
        Dim delta_PAT_dinamico_norm As Decimal = delta_PAT_dinamico / 0.33

        Dim checker As New ChkLW(1)  'sicuramente LW=1

        Dim d_o As DatoOrario

        'Partenza pioggia (incluse le prime ore di interruzione)
        Dim idx As Integer = partenza_ora_pioggia
        Dim idx_fine_ciclo As Integer = -1

        While idx_fine_ciclo < 0 AndAlso idx < mListaOrari.Count

            '**************************************************************************************
            ''SubEvento Modifica SB
            'If idx > partenza_ora_pioggia + 4 AndAlso idx - 4 >= 0 AndAlso
            '    mListaOrari(idx).Pioggia > 0 AndAlso
            '    mListaOrari(idx - 1).Pioggia = 0 AndAlso
            '    mListaOrari(idx - 2).Pioggia = 0 AndAlso
            '    mListaOrari(idx - 3).Pioggia = 0 AndAlso
            '    mListaOrari(idx - 4).Pioggia = 0 Then

            '    Dim dur_min_bagn_sub_evento As Decimal = Calc.OreBagnaturaStensvand(mListaOrari(idx).Temp)
            '    Dim allarme_sub_evento As Decimal = 1D / dur_min_bagn_sub_evento
            '    If (allarme_sub_evento > new_index_widget) Then
            '        'Controllareeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
            '        Dim riga_fine_ev_inf As Integer = FineEventoInf(idx, partenza_ora_pioggia)
            '    End If
            'End If
            '**************************************************************************************

            d_o = mListaOrari(idx)

            checker.Check(d_o.LW)

            dur += 1

            If checker.OreInterruzioneBagnatura = 1 Then somma_TMP_utile = somma_TMP
            If checker.OreInterruzioneBagnatura <= 4 Then somma_TMP += d_o.Temp

            If checker.OreInterruzioneBagnatura < 3 Then

                S3 = Calc.S3(somma_TMP / dur, dur)

                If checker.OreInterruzioneBagnatura = 0 Then
                    S3_interruzione = S3
                End If
            End If

            If checker.OreInterruzioneBagnatura = 1 Then ' qui deve restare > 0
                If prima_ora Then somma_TMP_dry = 0
                prima_ora = True
            End If

            If checker.OreInterruzioneBagnatura >= 1 Then

                Hdry = checker.OreInterruzioneBagnatura

                somma_TMP_dry += d_o.Temp

                MOR3 = Calc.MOR3(somma_TMP_dry / Hdry, Hdry)

            End If

            If d_o.PAT3 <= 0.9998 Then

                dur_min_bagn = Calc.OreBagnaturaStensvand(somma_TMP / dur)

                new_index_widget = Math.Min(1D, dur / Math.Round(dur_min_bagn, 1))

                'Probabilità di infezione
                d_o.ProbInf = new_index_widget
                'Rischio infettivo
                d_o.RischioInf = (delta_PAT_dinamico_norm + d_o.somma_cm2_tess_sensibileVI + (S3 * (1D - MOR3))) / 3D
                'd_o.DeltaPATDinamico = delta_PAT_dinamico 

            End If

            fine_index = idx 'modifica SB
            allarme_interruzione = new_index_widget 'modifica SB

            If checker.OreInterruzioneBagnatura > 3 Then 'devo calcolare la succ bagn
                idx_fine_ciclo = idx - 3 'mi posiz su 0
            End If

            idx += 1
        End While

        If idx_fine_ciclo < 0 Then
            Return -1
        End If


        'secondo evento bagnatura - sono nell'intervallo LW=0 con almeno 4 ore
        Hdry = checker.OreInterruzioneBagnatura

        checker.Reset(0)

        Dim idx_fine_ciclo_4 As Integer = idx_fine_ciclo + 4

        idx = idx_fine_ciclo
        idx_fine_ciclo = -1

        While idx_fine_ciclo < 0 AndAlso idx < mListaOrari.Count

            d_o = mListaOrari(idx)

            'da qui ci sono almeno 4ore con LW=0
            checker.Inc(d_o.LW)

            If idx >= idx_fine_ciclo_4 Then
                somma_TMP_dry += d_o.Temp
                Hdry += 1
            End If

            If checker.OreBagnatura > 0 Then ' significa che lw=1

                idx_fine_ciclo = idx
                MOR3_bagnatura = MOR3

            Else

                MOR3 = Calc.MOR3(somma_TMP_dry / Hdry, Hdry)
                S3 = S3_interruzione * (1D - MOR3) ' modificato S3_finale

                If d_o.LW < 1 AndAlso idx > fine_index AndAlso d_o.PAT3 <= 0.9998 Then

                    'Probabilità di infezione
                    d_o.ProbInf = allarme_interruzione
                    'Rischio infettivo
                    d_o.RischioInf = (delta_PAT_dinamico_norm + d_o.somma_cm2_tess_sensibileVI + S3) / 3D
                    'd_o.DeltaPATDinamico = delta_PAT_dinamico 

                End If
            End If

            idx += 1
        End While

        If idx_fine_ciclo < 0 Then
            Return -1
        End If


        checker.Reset(1)

        dur -= 4
        somma_TMP = somma_TMP_utile

        idx = idx_fine_ciclo
        idx_fine_ciclo = -1

        While idx_fine_ciclo < 0 AndAlso idx < mListaOrari.Count

            d_o = mListaOrari(idx)

            checker.Check(d_o.LW)

            dur += 1
            somma_TMP += d_o.Temp

            If idx > fine_index AndAlso d_o.PAT3 <= 0.9998 Then

                dur_min_bagn = Calc.OreBagnaturaStensvand(somma_TMP / dur)

                new_index_widget = Math.Min(1D, dur / Math.Round(dur_min_bagn, 1))

                PAT3_dinamico = d_o.PAT3
                delta_PAT_dinamico = PAT3_dinamico - PAT3_evento_prec
                delta_PAT_dinamico_norm = delta_PAT_dinamico / 0.33D

                S3 = Calc.S3(somma_TMP / dur, dur) * (1D - MOR3_bagnatura)

                'Probabilità di infezione
                d_o.ProbInf = new_index_widget
                'Rischio infettivo
                d_o.RischioInf = (delta_PAT_dinamico_norm + d_o.somma_cm2_tess_sensibileVI + S3) / 3D
                'd_o.DeltaPATDinamico = delta_PAT_dinamico 

            End If

            If checker.OreInterruzioneBagnatura > 3 Then
                'devo calcolare la succ bagn
                idx_fine_ciclo = idx - 4 'posizione ultimo LW1
            End If

            idx += 1
        End While

        Return idx_fine_ciclo
    End Function

    Public Function Ticchiolatura(ByVal datiMeteo As List(Of AgronicaCoreMeteoBiz.InterfacciaMeteoNT.MeteoDSS)) As rispostaStandard(Of cRisultatoModello)

        Dim r As New rispostaStandard(Of cRisultatoModello) With {
            .RispostaStringa = New cRisultatoModello(),
            .RispostaOK = False,
            .Errore = ""
        }

        If CalcolaTicchiolatura(datiMeteo) Then

            r.RispostaOK = True
            If mInizioCiclo > DateTime.MinValue Then

                Dim msg As String = "Data inizio ciclo malattia: " & mInizioCiclo.ToShortDateString()
                If mFineCiclo > DateTime.MinValue Then
                    msg &= " - Data fine ciclo malattia: " & mFineCiclo.ToShortDateString()
                End If

                r.RispostaStringa.Modello_WarningMsg = "<div style='text-align: center; font-size: larger;'>" & msg & "</div>"
            End If
            'r.RispostaStringa.Modello_Tabella1 = OutputDatiOrari()
            r.RispostaStringa.Modello_Tabella1 = OutputCollezioni(True)
            'r.RispostaStringa.Modello_Tabella = OutputDEBUG(listaColl(1).ListaOrari)
            'r.RispostaStringa.Modello_Tabella = OutputDEBUG(listaColl(7).ListaOrari)
            'r.RispostaStringa.Modello_Tabella1 = OutputDatiGG()

        Else

            r.Errore = mErrore

        End If

        'DEBUG *************************************************************************************************
        'Ticchiolatura_Indicatore(dataInizio, dataFine, ModelliPrevisionali_InputData_Meteo, objParametri_server)
        '*******************************************************************************************************

        Return r
    End Function

    Public Function Ticchiolatura_Indicatore(ByVal datiMeteo As List(Of AgronicaCoreMeteoBiz.InterfacciaMeteoNT.MeteoDSS), ByVal dataInizio As DateTime, ByVal dataFine As DateTime) As rispostaStandard(Of cRisultatoModelloIndicatori.Indicatore)

        Dim r As New rispostaStandard(Of cRisultatoModelloIndicatori.Indicatore)
        r.RispostaStringa = New cRisultatoModelloIndicatori.Indicatore("Ticchiolatura", dataInizio, dataFine)
        r.RispostaOK = False
        r.Errore = ""

        If CalcolaTicchiolatura(datiMeteo) Then

            r.RispostaOK = True

            Dim risk As Double = -1

            For Each ev In mListaEventi

                If ev.Cond_5_Ore And ev.Cond_Durata_Stensvad Then

                    If ev.Periodo_inf_incr > DateTime.MinValue Then

                        Dim periodo_inizio As DateTime = ev.Periodo_inf_incr.Date
                        Dim durata As Integer = Math.Round((DateDiff(DateInterval.Hour, ev.Inizio, ev.Fine) + 36D) / 24D)
                        Dim periodo_fine As DateTime = ev.Periodo_inf_incr.AddDays(durata)

                        If periodo_inizio <= dataFine AndAlso dataFine <= periodo_fine Then

                            risk = Math.Max(risk, ev.RISK())

                        End If
                    End If
                End If
            Next

            If risk < 0 Then
                risk = 0
                r.RispostaStringa.AuxMsg = "Nessun evento infettivo rilevato"
            End If

            r.RispostaStringa.Fill(risk, 1, {0.33, 0.66}, dataFine, dataFine)

        Else

            r.Errore = mErrore
            r.RispostaStringa.Status = cRisultatoModelloIndicatori.Indicatore.enum_Status.Status_Error
            r.RispostaStringa.StatusMsg = mErrore

        End If

        Return r
    End Function

    Private Function CreaDatiOrari(ByVal datiMeteo As List(Of AgronicaCoreMeteoBiz.InterfacciaMeteoNT.MeteoDSS)) As Integer

        Dim ST95 As Boolean = False
        Dim sT As Decimal = 6.3
        Dim STLW As Decimal = 0
        Dim idx_PAT016 As Integer = -1
        Dim q_bagn24 As New Queue(Of Decimal)
        Dim q_temp As New Queue(Of Decimal)
        Dim dd5sum As New DD5(24)

        Dim d_o As DatoOrario

        For Each d In datiMeteo

            d_o = New DatoOrario(d)

            'dal 1 febbraio inizio a calcolare st
            If d_o.DataOra.DayOfYear > 31 And Not ST95 Then
                'incremento giornaliero maturazione pseudoteci (deltast)
                If d_o.Temp >= 0 And (d_o.UmRel > 70 Or d_o.Pioggia > 0) Then 'Or YN > 8 Then
                    sT += (0.0031D + (0.0546D - 0.00175D * d_o.Temp) * d_o.Temp) / 24D
                End If
                ST95 = sT >= 9.5

                If ST95 Then
                    mInizioCiclo = d.DataOra
                End If
            End If

            q_bagn24.Enqueue(If(ST95, d_o.LW, d_o.YN))
            If q_bagn24.Count = 25 Then
                q_bagn24.Dequeue()
            End If

            If ST95 Then
                If mListaOrari.Count > 0 AndAlso mListaOrari.Last.PAT3 > 0.9995 Then

                    d_o.PAT1 = 1
                    d_o.PAT2 = 1
                    d_o.PAT3 = 1

                Else

                    'sicuramente sono passate più di 24 ore da inizio anno
                    Dim bagn_24ore_prec As Decimal = q_bagn24.Sum()
                    Dim TYN As Decimal = If(d_o.Temp > 0, d_o.Temp, 0) * bagn_24ore_prec / 24D ' temp*YN divido per 24 perchè formule si rif a dati gg
                    STLW += TYN / 24D 'Somma di TYN

                    'calcolo potenziale RILASCIO ascospore dell'ora

                    d_o.PAT1 = 1D / (1D + Math.Exp(5.41D - 0.035D * STLW)) 'interno
                    d_o.PAT2 = 1D / (1D + Math.Exp(8.27D - 0.035D * STLW)) 'esterno
                    d_o.PAT3 = 1D / (1D + Math.Exp(6.89D - 0.035D * STLW)) 'intermedio

                    If idx_PAT016 < 0 AndAlso Math.Round(d_o.PAT1, 3) >= 0.016 Then
                        idx_PAT016 = mListaOrari.Count ' da qui inizierò a vedere se piove
                    End If
                End If
            End If

            q_temp.Enqueue(d_o.Temp)
            If q_temp.Count = 24 Then
                'calcoli sviluppo tessuti suscettibili
                'calcolo dopo le prime 24 ore perchè le funzioni sono giornaliere
                'provo a fare la media mobile delle 24 ore precedenti
                d_o.somma_cm2_tess_sensibileVI = mCalcSens.Vindice_tess_sens(dd5sum.Inc(q_temp.Min(), q_temp.Max()))

                q_temp.Dequeue()
            End If

            mListaOrari.Add(d_o)

        Next

        Return idx_PAT016
    End Function

    Private Sub CreaCollezioni(ByVal idx_PAT016 As Integer)

        '*********************************************************************
        ' DA VERIFICARE LA LOGICA....
        ' i controlli temporali sull'evento vengono effettuati anche nel costruttore dell'evento
        ' forse qui sono superflui
        '*********************************************************************
        'ora inizio a calcolare da PAT016_idx
        Dim idx_pioggia As Integer = CercoPioggia(idx_PAT016)
        Dim idx_pioggiaprec As Integer = -1
        Dim idx_rilascio As Integer = -1
        Dim idx_rilascioprec As Integer = -1
        Dim idx_rilascioprec_1 As Integer = -1

        While idx_pioggia >= 0 AndAlso mListaOrari(idx_pioggia).PAT2 <= 0.999

            'Tra un evento piovoso ed il successivo devono esserci più di 4 ore di interruzione di pioggia
            If idx_rilascioprec < 0 OrElse idx_pioggia - idx_rilascioprec > 4 Then

                idx_rilascioprec = idx_pioggia

                'inf lo stesso giorno prendo solo la prima
                If idx_pioggiaprec < 0 OrElse mListaOrari(idx_pioggia).DataOra.DayOfYear <> mListaOrari(idx_pioggiaprec).DataOra.DayOfYear Then

                    idx_rilascioprec_1 = idx_rilascio
                    idx_rilascio = Rilascio(idx_pioggia)
                    If idx_rilascio >= 0 Then

                        Dim stesso_gg_rilascio As Boolean = False
                        If mListaEventi.Count > 0 Then
                            stesso_gg_rilascio = mListaEventi.Last.Inizio.DayOfYear = mListaOrari(idx_rilascio).DataOra.DayOfYear
                        End If

                        If Not stesso_gg_rilascio Then

                            Dim idx_fine_ev_inf As Integer = FineEventoInf(idx_rilascio, idx_rilascioprec_1)
                            If idx_fine_ev_inf >= 0 Then

                                'aggiungo evento sia infettivo che no a collezione
                                Dim evento As New EventoInf(mListaOrari, idx_rilascio, idx_fine_ev_inf, If(mListaEventi.Count > 0, mListaEventi(mListaEventi.Count - 1), Nothing))

                                If evento.Cond_5_Ore Then ' se non disto 5 ore non aggiungo nulla
                                    mListaEventi.Add(evento)
                                    evento.CalcolaListaOrari()
                                End If

                            End If
                        End If
                    End If
                End If
            End If

            idx_pioggiaprec = idx_pioggia
            idx_pioggia = CercoPioggia(idx_pioggia + 1)

        End While

        If idx_pioggia >= 0 Then
            If mListaOrari(idx_pioggia).PAT2 > 0.999 Then
                mFineCiclo = mListaOrari(idx_pioggia).DataOra
            End If
        End If

    End Sub

    Private Function CalcolaTicchiolatura(ByVal datiMeteo As List(Of AgronicaCoreMeteoBiz.InterfacciaMeteoNT.MeteoDSS)) As Boolean

        Dim result As Boolean = True

        Try

            Dim idx_PAT016 As Integer = CreaDatiOrari(datiMeteo)

            If idx_PAT016 >= 0 Then

                CreaCollezioni(idx_PAT016)

            Else

                result = False
                mErrore = "Non è stato raggiunto l'inizio del ciclo dell'avversità."

            End If

        Catch ex As Exception

            result = False

            'uso questa funzione per ottenere il Messaggio..:
            mErrore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return result
    End Function

    Private Class DatoGG
        Private Giorno As DateTime
        Private Tmin As Decimal
        Private Tmax As Decimal
        Private Tmed As Decimal
        Private PAT1 As Decimal
        Private PAT2 As Decimal
        Private PATest As Decimal
        Private Pioggia As Decimal
        Private SUM_LW As Decimal
        Private hh_cnt As Decimal
        Public Sub New(ByVal src As DatoOrario)
            Giorno = src.DataOra.Date
            Tmin = src.Temp
            Tmax = src.Temp
            Tmed = src.Temp
            PAT1 = src.PAT1
            PAT2 = src.PAT2
            PATest = src.PAT3
            Pioggia = src.Pioggia
            SUM_LW = src.LW
            hh_cnt = 1
        End Sub
        Public Sub AddHH(ByVal src As DatoOrario)
            Tmin = Math.Min(Tmin, src.Temp)
            Tmax = Math.Max(Tmax, src.Temp)
            Tmed += src.Temp
            PAT1 = Math.Max(PAT1, src.PAT1)
            PAT2 = Math.Max(PAT2, src.PAT2)
            PATest = Math.Max(PATest, src.PAT3)
            Pioggia += src.Pioggia
            SUM_LW += src.LW
            hh_cnt += 1
        End Sub
        Public Function NeedFlush(ByVal dt As DateTime) As Boolean
            Return Giorno.DayOfYear <> dt.DayOfYear
        End Function
        Public Sub Flush(ByVal calc As CalcSens, ByVal output As OutputModello)

            Dim dd5sum As New DD5(1)

            Tmed /= hh_cnt
            Dim DD5 As Decimal = dd5sum.Inc(Tmin, Tmax)

            Dim cluster_rosetta As Decimal = calc.Foglie_cluster(DD5)          'foglie
            Dim getto_foglie As Decimal = calc.Foglie_leshoot(DD5)             'foglie
            Dim getto_shoot As Decimal = calc.Cm2_vege_leafKL(DD5)             'getto cm2 superficie fogliare
            Dim TT_rosetta As Decimal = calc.Cm2_fruit_leaf1(DD5)              'fiore cm2 superficie fogliare
            Dim getto_shoot_sens As Decimal = calc.Cm2_vege_leaf_SENS(DD5)     'Getto shoot sensibile
            Dim TT_rosetta_sens As Decimal = calc.Cm2_fruit_leaf1_SENS(DD5)    'rosetta cluster sensibile

            output.AddField(Giorno)
            output.AddField(Tmin)
            output.AddField(Tmax)
            output.AddField(Tmed)
            output.AddField(PAT1)
            output.AddField(PAT2)
            output.AddField(PATest)
            output.AddField(Pioggia)
            output.AddField(SUM_LW)
            output.AddField(DD5)
            output.AddField(cluster_rosetta)
            output.AddField(getto_foglie)
            output.AddField(getto_shoot)
            output.AddField(TT_rosetta)
            output.AddField(getto_shoot_sens)
            output.AddField(TT_rosetta_sens)

            output.Commit()
        End Sub
    End Class

    Private Function OutputDatiGG() As String

        Dim output As New OutputModello
        output.aggiungiColonna("Data", GetType(Date), "Data", "dd/MM/yyyy")
        output.aggiungiColonna("tmin", GetType(Decimal), "Tmin", "0.00")
        output.aggiungiColonna("tmax", GetType(Decimal), "Tmax", "0.00")
        output.aggiungiColonna("tmed", GetType(Decimal), "Tmed", "0.00")
        output.aggiungiColonna("PAT1", GetType(Decimal), "PAT1", "0.00")
        output.aggiungiColonna("PAT2", GetType(Decimal), "PAT2", "0.00")
        output.aggiungiColonna("PATest", GetType(Decimal), "PATest", "0.00")
        output.aggiungiColonna("pioggia", GetType(Decimal), "Pioggia", "0.00")
        output.aggiungiColonna("SUM_LW", GetType(Integer), "SUM_LW", "0")
        output.aggiungiColonna("DD5", GetType(Decimal), "DD5", "0.00")
        output.aggiungiColonna("cluster_rosetta", GetType(Decimal), "cluster_rosetta", "0.00")
        output.aggiungiColonna("getto_foglie", GetType(Decimal), "getto_foglie", "0.00")
        output.aggiungiColonna("getto_shoot", GetType(Decimal), "getto_shoot", "0.00")
        output.aggiungiColonna("TT_rosetta", GetType(Decimal), "TT_rosetta", "0.00")
        output.aggiungiColonna("getto_shoot_sens", GetType(Decimal), "TSensibile getto", "0.00")
        output.aggiungiColonna("TT_rosetta_sens", GetType(Decimal), "TSensibile rosetta", "0.00")

        Dim dgg As DatoGG = Nothing

        For Each d In mListaOrari

            If dgg Is Nothing OrElse dgg.NeedFlush(d.DataOra) Then

                If dgg IsNot Nothing Then

                    dgg.Flush(mCalcSens, output)
                End If

                dgg = New DatoGG(d)
            Else

                dgg.AddHH(d)
            End If
        Next

        If dgg IsNot Nothing Then
            dgg.Flush(mCalcSens, output)
        End If

        Return output.Output()
    End Function

    Private Function OutputCollezioni(ByVal bFilter As Boolean) As String

        Dim output As New OutputModello
        'output.aggiungiColonna("Valido", GetType(String), "Valido", "")
        output.aggiungiColonna("DataInizio", GetType(Date), "Inizio evento", "dd/MM/yyyy HH")
        output.aggiungiColonna("DataFine", GetType(Date), "Data fine", "dd/MM/yyyy HH")
        output.aggiungiColonna("Lwcorr", GetType(Decimal), "Ore bagnatura", "0")
        output.aggiungiColonna("dur_min_bagn", GetType(Decimal), "Min bagnatura", "0.00")
        output.aggiungiColonna("PAT3", GetType(Decimal), "PAT", "0.00")
        output.aggiungiColonna("deltaPAT", GetType(Decimal), "DeltaPAT VI", "0.00")
        Dim deltapat_ind = output.aggiungiIndicatore("deltaPAT_IND", {0.33, 0.66, System.Decimal.MaxValue}, OutputIndicator.enum_IndicatorType.Numbers).setFieldVal("deltaPAT")
        output.aggiungiColonna("INF", GetType(Decimal), "INF", "0.00")
        Dim inf_ind = output.aggiungiIndicatore("INF_IND", {0.33, 0.66, System.Decimal.MaxValue}, OutputIndicator.enum_IndicatorType.Numbers).setFieldVal("INF")
        output.aggiungiColonna("Tsens", GetType(Decimal), "Indice t. sens.", "0.00")
        Dim tsens_ind = output.aggiungiIndicatore("Tsens_IND", {0.33, 0.66, System.Decimal.MaxValue}, OutputIndicator.enum_IndicatorType.Numbers).setFieldVal("Tsens")
        output.aggiungiColonna("RISK", GetType(Decimal), "INDICE RISCHIO", "0.00")
        Dim risk_ind = output.aggiungiIndicatore("RISK_IND", {0.33, 0.66, System.Decimal.MaxValue}, OutputIndicator.enum_IndicatorType.Colors).setFieldVal("RISK")
        Dim group As String = "Periodo comparsa sintomi sulle foglie"
        output.aggiungiColonna("Periodo_inf_inizio", GetType(Date), "Inizio", "dd/MM/yyyy", True)._gruppoColonne = group
        output.aggiungiColonna("Periodo_inf_fine", GetType(Date), "Fine", "dd/MM/yyyy", True)._gruppoColonne = group
        'output.aggiungiColonna("Periodo_inf_durata", GetType(Integer), "durata", "0")._gruppoColonne = group

        For Each ev In mListaEventi

            If Not bFilter OrElse (ev.Cond_5_Ore And ev.Cond_Durata_Stensvad) Then

                'vallist.Add(If(co.Cond_5_ore, "1", "0") + If(co.Cond_Durata_Stensvad, "1", "0") + If(co.Cond_Stessa_Data, "1", "0"))
                output.AddField(ev.Inizio)
                output.AddField(ev.Fine)
                output.AddField(ev.LWcorr)
                output.AddField(ev.Dur_min_bagn)
                output.AddField(ev.PAT3)

                Dim valDeltaPAT3 As Decimal = ev.DeltaPAT3
                If bFilter Then
                    valDeltaPAT3 /= 0.33D
                    valDeltaPAT3 = Math.Min(valDeltaPAT3, 1) 'per sicurezza perchè .33 e arbitrario
                End If

                output.AddField(valDeltaPAT3)
                output.AddField(deltapat_ind.colorForVal(valDeltaPAT3))
                output.AddField(ev.INF())
                output.AddField(inf_ind.colorForVal(ev.INF()))
                output.AddField(ev.Tessuto_sensibile)
                output.AddField(tsens_ind.colorForVal(ev.Tessuto_sensibile))
                output.AddField(ev.RISK())
                output.AddField(risk_ind.colorForVal(ev.RISK()))
                If ev.Periodo_inf_incr > DateTime.MinValue Then
                    Dim durata As Integer = Math.Round((DateDiff(DateInterval.Hour, ev.Inizio, ev.Fine) + 36D) / 24D)
                    output.AddField(ev.Periodo_inf_incr)
                    output.AddField(ev.Periodo_inf_incr.AddDays(durata))
                    'output.AddField(durata)
                Else
                    output.AddField(DBNull.Value)
                    output.AddField(DBNull.Value)
                    'output.AddField(DBNull.Value)
                End If

                output.Commit()

            End If

        Next

        Return output.Output({deltapat_ind, inf_ind, tsens_ind, risk_ind}.ToList())
    End Function

    Private Function OutputDatiOrari() As String

        Dim output As New OutputModello
        output.aggiungiColonna("DataOra", GetType(Date), "DataOra", "dd/MM/yyyy HH")
        output.aggiungiColonna("tmed", GetType(Decimal), "T_Med", "0.00")
        output.aggiungiColonna("UR", GetType(Decimal), "UR", "0.00")
        output.aggiungiColonna("Pioggia", GetType(Decimal), "Pioggia", "0.00")
        output.aggiungiColonna("LW", GetType(Integer), "LW", "0")
        output.aggiungiColonna("PAT1", GetType(Decimal), "PAT1", "0.000")
        output.aggiungiColonna("PAT2", GetType(Decimal), "PAT2", "0.000")
        output.aggiungiColonna("PAT3", GetType(Decimal), "PAT3", "0.000")
        output.aggiungiColonna("somma_cm2_tess_sensibileVI", GetType(Decimal), "somma_cm2_tess_sensibileVI", "0.000")
        output.aggiungiColonna("ProbInf", GetType(Decimal), "ProbInf", "0.000")
        output.aggiungiColonna("RischioInf", GetType(Decimal), "RischioInf", "0.000")

        For Each d In mListaOrari

            output.AddField(d.DataOra)
            output.AddField(d.Temp)
            output.AddField(d.UmRel)
            output.AddField(d.Pioggia)
            output.AddField(d.LW)
            output.AddField(d.PAT1)
            output.AddField(d.PAT2)
            output.AddField(d.PAT3)
            output.AddField(d.somma_cm2_tess_sensibileVI)
            output.AddField(d.ProbInf)
            output.AddField(d.RischioInf)

            output.Commit()

        Next

        Return output.Output()
    End Function

    Private Function OutputDEBUG(ByRef DatiModello As List(Of DatoOrarioColl)) As String

        Dim output As New OutputModello
        output.aggiungiColonna("DataOra", GetType(Date), "DataOra", "dd/MM/yyyy HH")
        output.aggiungiColonna("Temp", GetType(Decimal), "Temp", "0.00")
        output.aggiungiColonna("UR", GetType(Decimal), "UR", "0.00")
        output.aggiungiColonna("Pioggia", GetType(Decimal), "Pioggia", "0.00")
        output.aggiungiColonna("LW", GetType(Integer), "LW", "0")
        output.aggiungiColonna("Heve", GetType(Integer), "Heve", "0")
        output.aggiungiColonna("Hdry", GetType(Integer), "Hdry", "0")
        output.aggiungiColonna("Tdry", GetType(Decimal), "Tdry", "0.00")
        output.aggiungiColonna("RHdry", GetType(Decimal), "RHdry", "0.00")
        output.aggiungiColonna("T_med", GetType(Decimal), "T_med", "0.00")
        output.aggiungiColonna("S1", GetType(Decimal), "S1", "0.000000")
        output.aggiungiColonna("S2", GetType(Decimal), "S2", "0.000000")
        output.aggiungiColonna("S3", GetType(Decimal), "S3", "0.000000")
        output.aggiungiColonna("MOR1", GetType(Decimal), "MOR1", "0.000000")
        output.aggiungiColonna("MOR2", GetType(Decimal), "MOR2", "0.000000")
        output.aggiungiColonna("MOR3", GetType(Decimal), "MOR3", "0.000000")

        For Each d In DatiModello

            output.AddField(d.DataOra)
            output.AddField(d.Temp)
            output.AddField(d.UmRel)
            output.AddField(d.Pioggia)
            output.AddField(d.LW)
            output.AddField(d.Heve)
            output.AddField(d.Hdry)
            output.AddField(d.Tdry)
            output.AddField(d.RHdry)
            output.AddField(d.Tmed)
            output.AddField(d.S1)
            output.AddField(d.S2)
            output.AddField(d.S3)
            output.AddField(d.MOR1)
            output.AddField(d.MOR2)
            output.AddField(d.MOR3)

            output.Commit()

        Next

        Return output.Output()
    End Function

End Class

#End If
