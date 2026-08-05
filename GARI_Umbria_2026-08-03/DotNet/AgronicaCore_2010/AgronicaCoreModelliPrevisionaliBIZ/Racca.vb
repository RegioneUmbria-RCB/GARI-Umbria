
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports Newtonsoft.Json.Linq


Public Class RaccaCondMeteo
    Public CalcoloPeriodi_DatiMisurati As Boolean 'True -> Dati misurati, False-> Dati simulati VPD
    Public CalcoloTempPeriodo_BagnNormale As Boolean 'True -> Bagnatura normale, False -> Bagnatura con pioggia
    Public SogliaPioggiaCorrBagn As Decimal
    Public SogliaURCorrBagn As Decimal
    Public SogliaPioggiaPerBagn As Decimal
End Class


Public Class MeteoMath
    Public Shared Function VP(t As Decimal, ur As Decimal)
        Dim v0 As Decimal = (ur - 122.2900152) / 58.04521029
        Dim v1 As Decimal = (t - 138.8452957) / 43.69924278
        Return 14.50792883 * Math.Exp(-0.5 * ((v0 * v0) + (v1 * v1)))
    End Function
End Class



Public MustInherit Class RaccaDatoOrario
    Public ReadOnly Meteo As MeteoDSSItem
    Public ReadOnly BagnCorretta As Integer
    Public ReadOnly Bagn_mm As Decimal
    Public ReadOnly CalcoloPeriodiSecchi As Integer
    Public ReadOnly BagnSenzaPioggia As Integer
    Public ReadOnly BagnDopoPioggia As Integer
    Public ReadOnly SommaTermicaPerBagn As Decimal
    Public ReadOnly TempMediaPerBagn As Decimal
    Public SommaTermicaPerLAI As Decimal
    Public LAI As Decimal

    Public Sub New(ByVal dm As MeteoDSSItem, ByVal cond As RaccaCondMeteo, ByVal prec As RaccaDatoOrario)

        Meteo = dm

        'Dati misurati o Dati simulati VPD
        Dim VP_Sat As Decimal = MeteoMath.VP(Meteo.Temp, 100)
        Dim VP As Decimal = MeteoMath.VP(Meteo.Temp, Meteo.UmRel)
        Dim VPD As Decimal = VP_Sat - VP

        If Meteo.Prec >= cond.SogliaPioggiaCorrBagn OrElse Meteo.UmRel >= cond.SogliaURCorrBagn Then
            BagnCorretta = 1
        Else
            If cond.CalcoloPeriodi_DatiMisurati Then
                BagnCorretta = dm.Bagn
            Else
                'VP_Bagn
                Dim val As Decimal = 1D / (1D + Math.Exp(-(1.35567823583444 - (63.69881694837 * VPD))))
                BagnCorretta = If(val > 0.5, 1, 0)
            End If
        End If

        Bagn_mm = 0
        If BagnCorretta = 1 Then
            If Meteo.Prec = 0 Then
                Bagn_mm = 0.18D * Math.Exp(-(VPD * 6.895D) / 0.17D) + 0.0315D
            End If
        End If
        CalcoloPeriodiSecchi = If(BagnCorretta = 0, 1, 0)
        BagnSenzaPioggia = BagnCorretta
        BagnDopoPioggia = BagnCorretta
        SommaTermicaPerBagn = 0
        TempMediaPerBagn = 0
        SommaTermicaPerLAI = 0
        LAI = 0

        If prec Is Nothing Then
            Return
        End If

        'Calcolo periodi secchi -> (il primo record: se BagnaturaCorretta() = 0 1 altrimeti 0
        '                           i successivi: se BagnaturaCorretta() = 0 valore prec. + 1 altrimenti 0)
        'Periodo bagnatura senza pioggia -> (il primo record: BagnaturaCorretta()
        '                                   i successivi: se BagnaturaCorretta() = 1 il precedente + BagnaturaCorretta() altrimenti 0)
        'Periodo bagnatura dopo pioggia (il primo record: BagnaturaCorretta()
        '                               i successivi SE(E(N5=1;R4+F4>='Parametri Modello'!$B$7);R4+N5;0)
        If BagnCorretta = 0 Then
            CalcoloPeriodiSecchi = prec.CalcoloPeriodiSecchi + 1
        End If

        If BagnCorretta = 1 Then
            BagnSenzaPioggia = prec.BagnSenzaPioggia + 1
        End If

        If BagnCorretta = 1 AndAlso (prec.BagnDopoPioggia + prec.Meteo.Prec) >= cond.SogliaPioggiaPerBagn Then
            BagnDopoPioggia = prec.BagnDopoPioggia + BagnCorretta
        Else
            BagnDopoPioggia = 0
        End If

        Dim bagnNormale = BagnSenzaPioggia 'Bagnatura normale
        If Not cond.CalcoloTempPeriodo_BagnNormale Then
            'Bagnatura con pioggia
            bagnNormale = BagnDopoPioggia
        End If
        'R5=SE(E($S$2="Bagnatura normale";"Q5>0);T4+D5;SE(E($S$2="Bagnatura con pioggia";R5>0);T4+D5;0))
        'If (mCalcoloTempPeriodo_BagnNormale AndAlso d_o.BagnSenzaPioggia > 0) OrElse (Not mCalcoloTempPeriodo_BagnNormale AndAlso d_o.BagnDopoPioggia > 0) Then
        If bagnNormale > 0 Then
            SommaTermicaPerBagn = prec.SommaTermicaPerBagn + Meteo.Temp
            TempMediaPerBagn = SommaTermicaPerBagn / bagnNormale
        End If
    End Sub
End Class


#Region "Funzioni LAI"

Public MustInherit Class RaccaFunzioneLAI

    Protected ReadOnly calc_gg As Integer
    Protected ReadOnly calc_gg_varieta As Integer
    Private ReadOnly a As Decimal
    Private ReadOnly b As Decimal
    Private ReadOnly x0 As Decimal
    Private ReadOnly y0 As Decimal
    Private ReadOnly yMax As Decimal

    Public Sub New(ByVal Veg_Cod As Integer, ByVal gg As Integer, ByVal gg_var As Integer)

        calc_gg = gg 'C12
        calc_gg_varieta = gg_var

        Select Case Veg_Cod

            Case 52 'Pomodoro
                a = 4.3119 'C17
                b = 0.6829 'C18
                x0 = 798.0776 'C19
                y0 = 0 'C20
                yMax = 4.3119 'C21

            Case 6 'Barbabietola da zucchero
                a = 5.2 'C17
                b = 0.8 'C18
                x0 = 1767 'C19
                y0 = 0 'C20
                yMax = 5.2 'C21

            Case 46 'Patata
                a = 4.1269 'C17
                b = 0.3478 'C18
                x0 = 938.9242 'C19
                y0 = 0 'C20
                yMax = 4.1269 'C21

            Case 58 'Soia
                a = 6.9895 'C18
                b = 0.4768 'C19
                x0 = 0.4869 'C20
                y0 = 0 'C21
                yMax = 6.9895 'C22

            Case 55 'Riso
                a = 3.1201 'C18
                b = 0.2844 'C19
                x0 = 0.7059 'C20
                y0 = 0.3797 'C21
                yMax = 3.4998 'C22

            Case 38 'Mais
                a = 2.831 'C17
                b = 0.5178 'C18
                x0 = 0.7878 'C19
                y0 = 0 'C20
                yMax = 4.11 'C21

            Case 44 'Olivo
                a = 1 'C17
                b = 1 'C18
                x0 = 1 'C19
                y0 = 1 'C20
                yMax = 1 'C21

            Case Else
                a = 1 'C17
                b = 1 'C18
                x0 = 1 'C19
                y0 = 1 'C20
                yMax = 1 'C21

        End Select
    End Sub

    Protected Function Calc(ByVal val As Decimal, ByVal factor As Decimal) As Decimal
        Dim x As Decimal = Math.Log(val / x0) / b
        Return (y0 + a * Math.Exp(-0.5 * x * x) / factor) / yMax
    End Function

    Public MustOverride Sub Calcola(ByVal giuliano As Integer, ByRef d_o As RaccaDatoOrario, ByVal d_o_prec As RaccaDatoOrario)
End Class

Public Class RaccaFunzioneLAI_Std
    Inherits RaccaFunzioneLAI
    Public Sub New(ByVal Veg_Cod As Integer, ByVal gg As Integer, ByVal gg_var As Integer)
        MyBase.New(Veg_Cod, gg, gg_var)
    End Sub
    Public Overrides Sub Calcola(ByVal giuliano As Integer, ByRef d_o As RaccaDatoOrario, ByVal d_o_prec As RaccaDatoOrario)
        If giuliano > calc_gg Then
            If d_o_prec IsNot Nothing Then
                d_o.SommaTermicaPerLAI = d_o_prec.SommaTermicaPerLAI + (d_o.Meteo.Temp / 24D)
            End If

            '=SE(AA5>0;('Parametri Modello'!$C$17*EXP(-0,5*(LN(AA5/'Parametri Modello'!$C$19)/'Parametri Modello'!$C$18)^2))/'Parametri Modello'!$C$21;0)
            If d_o.SommaTermicaPerLAI > 0 Then
                d_o.LAI = Calc(d_o.SommaTermicaPerLAI, 1)
            End If
        End If
    End Sub
End Class

Public Class RaccaFunzioneLAI_SoiaSclerotinia
    Inherits RaccaFunzioneLAI
    Public Sub New(ByVal Veg_Cod As Integer, ByVal gg As Integer, ByVal gg_var As Integer)
        MyBase.New(Veg_Cod, gg, gg_var)
    End Sub
    Public Overrides Sub Calcola(giuliano As Integer, ByRef d_o As RaccaDatoOrario, d_o_prec As RaccaDatoOrario)

        If giuliano > calc_gg Then

            d_o.SommaTermicaPerLAI = Math.Min(1D, CDec((giuliano - (calc_gg + 1)) + 1) / CDec(calc_gg_varieta))

            If d_o.SommaTermicaPerLAI < 1 Then

                d_o.LAI = Calc(d_o.SommaTermicaPerLAI, 1)
            End If
        End If
    End Sub
End Class

Public Class RaccaFunzioneLAI_RisoBrusone
    Inherits RaccaFunzioneLAI
    Public Sub New(ByVal Veg_Cod As Integer, ByVal gg As Integer, ByVal gg_var As Integer)
        MyBase.New(Veg_Cod, gg, gg_var)
    End Sub
    Public Overrides Sub Calcola(giuliano As Integer, ByRef d_o As RaccaDatoOrario, d_o_prec As RaccaDatoOrario)
        If giuliano > calc_gg Then

            d_o.SommaTermicaPerLAI = Math.Min(1D, CDec(giuliano - calc_gg) / CDec(calc_gg_varieta))

            If d_o.SommaTermicaPerLAI < 1 Then

                d_o.LAI = Calc(d_o.SommaTermicaPerLAI, 1)
            End If
        End If
    End Sub
End Class

Public Class RaccaFunzioneLAI_Mais
    Inherits RaccaFunzioneLAI
    Public Sub New(ByVal Veg_Cod As Integer, ByVal gg As Integer, ByVal gg_var As Integer)
        MyBase.New(Veg_Cod, gg, gg_var)
    End Sub
    Public Overrides Sub Calcola(giuliano As Integer, ByRef d_o As RaccaDatoOrario, d_o_prec As RaccaDatoOrario)
        If giuliano > calc_gg Then

            Dim x As Decimal = CDec(giuliano - calc_gg) / CDec(calc_gg_varieta)
            'LAI(rel.)=(y0 + a * exp(-0.5 * (ln(x/x0)/b)^2)/x)) / yMax
            d_o.LAI = Calc(x, x)
        End If
    End Sub
End Class

Public Class RaccaFunzioneLAI_OlivoAntracnosi
    Inherits RaccaFunzioneLAI
    Public Sub New(ByVal Veg_Cod As Integer, ByVal gg As Integer, ByVal gg_var As Integer)
        MyBase.New(Veg_Cod, gg, gg_var)
    End Sub
    Public Overrides Sub Calcola(giuliano As Integer, ByRef d_o As RaccaDatoOrario, d_o_prec As RaccaDatoOrario)
        d_o.LAI = 1
    End Sub
End Class

#End Region


Public Class RaccaMath

    Public Class FunzioneBetaHau
        Public y As Decimal
        Public min As Decimal
        Public max As Decimal
        Public opt As Decimal
        Public n As Decimal
        Public Function Calc(ByVal x As Decimal) As Decimal
            If x >= min AndAlso x <= max Then
                Return y * Math.Pow((x - min) / (opt - min), (n * (opt - min) / (max - opt))) * Math.Pow((max - x) / (max - opt), n)
            End If
            Return 0
        End Function
    End Class

    Public Class FunzioneChapman
        Public b As Decimal
        Public c As Decimal
        Public Function Calc(ByVal x As Decimal) As Decimal
            Return Math.Pow((1D - Math.Exp(-b * x)), c)
        End Function
    End Class

End Class


Public MustInherit Class RaccaModello

    Public Class ParametriIndicatori
        Public ReadOnly ScaleMax As Decimal
        Public ReadOnly BandsValues As Decimal()
        Public Sub New(ByVal smax As Decimal, ByVal bvals As Decimal())
            ScaleMax = smax
            BandsValues = bvals
        End Sub
    End Class

    Protected ReadOnly mCondMeteo As RaccaCondMeteo
    Protected mParametriIndicatori As ParametriIndicatori

    Public Property Indicatori As ParametriIndicatori
        Get
            Return mParametriIndicatori
        End Get
        Set(value As ParametriIndicatori)
            mParametriIndicatori = value
        End Set
    End Property

    Public Sub New(ByVal condMeteo As RaccaCondMeteo)
        mCondMeteo = condMeteo
        mParametriIndicatori = Nothing
    End Sub

    Public MustInherit Class DatoGG
        Public Data As DateTime
        Public Temp As Decimal
        Public UR As Decimal
        Public Pioggia As Decimal
        Public Bagn_ore As Integer
        Public Bagn_mm As Decimal
        Public LAI As Decimal
        Public MustOverride Sub Output(ByRef output As OutputModello, ByRef indic As OutputIndicator)
        Public MustOverride Function Indicatore() As Decimal
    End Class

    Protected MustInherit Class RunningGG
        Protected hh As Decimal
        Public Sub New()
            hh = 0
        End Sub
        Protected Sub AbsCreate(ByRef d_gg As DatoGG, ByVal d_hh As RaccaDatoOrario)
            d_gg.Data = d_hh.Meteo.DataOra.Date
            d_gg.Temp = d_hh.Meteo.Temp
            d_gg.UR = d_hh.Meteo.UmRel
            d_gg.Pioggia = d_hh.Meteo.Prec
            d_gg.Bagn_ore = d_hh.BagnCorretta
            d_gg.Bagn_mm = d_hh.Bagn_mm
            d_gg.LAI = d_hh.LAI

            hh = 1
        End Sub
        Protected Sub AbsAdd(ByRef d_gg As DatoGG, ByVal d_hh As RaccaDatoOrario)
            d_gg.Temp += d_hh.Meteo.Temp
            d_gg.UR += d_hh.Meteo.UmRel
            d_gg.Pioggia += d_hh.Meteo.Prec
            d_gg.Bagn_ore += d_hh.BagnCorretta
            d_gg.Bagn_mm += d_hh.Bagn_mm
            d_gg.LAI = Math.Max(d_gg.LAI, d_hh.LAI)

            hh += 1
        End Sub
        Protected Sub AbsFlush(ByRef d_gg As DatoGG)
            d_gg.Temp /= hh
            d_gg.UR /= hh
        End Sub
    End Class

    Protected MustOverride Function CalcolaDatiOrari(ByVal datiMeteo As MeteoReadOnlyList, ByVal funLAI As RaccaFunzioneLAI) As List(Of RaccaDatoOrario)
    Protected MustOverride Function CalcolaDatiGiornalieri(ByVal elencoOrari As List(Of RaccaDatoOrario)) As List(Of DatoGG)
    Public Function Calcola(ByVal datiMeteo As MeteoReadOnlyList, ByVal funLAI As RaccaFunzioneLAI) As List(Of DatoGG)

        Dim dati_orari As List(Of RaccaDatoOrario) = CalcolaDatiOrari(datiMeteo, funLAI)

        Return CalcolaDatiGiornalieri(dati_orari)
    End Function

    Protected Class GiornoGiuliano
        Private mEndOfYear As DateTime
        Private mGiorno As Integer
        Private mCumGiorno As Integer
        Public ReadOnly Property Giorno As Integer
            Get
                Return mGiorno
            End Get
        End Property
        Public ReadOnly Property CumGiorno As Integer
            Get
                Return mCumGiorno
            End Get
        End Property
        Public Sub New(ByVal dt As DateTime)
            mEndOfYear = New DateTime(dt.Year, 12, 31)
            mGiorno = dt.DayOfYear
            mCumGiorno = mGiorno
        End Sub
        Public Sub SetDate(ByVal dt As DateTime)
            mGiorno = dt.DayOfYear
            If dt > mEndOfYear Then
                mCumGiorno = mEndOfYear.DayOfYear + mGiorno
            Else
                mCumGiorno = mGiorno
            End If
        End Sub
    End Class

    Protected MustOverride Function CreaOutput(ByRef output As OutputModello) As OutputIndicator

    Public Function GeneraOutput(ByVal elencoGG As List(Of DatoGG)) As String

        Dim output As New OutputModello

        Dim indic = CreaOutput(output)

        For Each d_gg In elencoGG

            d_gg.Output(output, indic)

            output.Commit()
        Next

        If indic IsNot Nothing Then
            Return output.Output({indic}.ToList(), indic.PlotBands())
        End If

        Return output.Output()
    End Function

End Class


Public Class DatoOrarioStd
    Inherits RaccaDatoOrario

    Public INF_T As Decimal
    Public INF_BN As Decimal
    Public Les_T As Decimal
    Public Les_UR As Decimal
    Public PeriodoLatenza As Decimal

    Public Sub New(ByVal dm As MeteoDSSItem, ByVal cond As RaccaCondMeteo, ByVal prec As RaccaDatoOrario)
        MyBase.New(dm, cond, prec)
        INF_T = 0
        INF_BN = 0
        Les_T = 0
        Les_UR = 0
        PeriodoLatenza = 0
    End Sub
    Public ReadOnly Property INF_Prob As Decimal
        Get
            Return INF_T * INF_BN
        End Get
    End Property
    Public Property CrescitaLesioni As Decimal
        Get
            Return Les_T * Les_UR
        End Get
        Set(value As Decimal)
            Les_T = value
            Les_UR = 1 'Les_T * Les_UR deve essere uguale a value
        End Set
    End Property
    Public ReadOnly Property IndiceRischio
        Get
            Return LAI * INF_Prob * CrescitaLesioni
        End Get
    End Property
End Class



Public MustInherit Class ProbInfFactory
    Public calc_gg As Integer
    Public calc_T As RaccaMath.FunzioneBetaHau
    Public calc_BN As RaccaMath.FunzioneChapman

    Public Sub New()
        calc_T = New RaccaMath.FunzioneBetaHau
        calc_BN = New RaccaMath.FunzioneChapman
    End Sub
    Public Sub Calcola(ByVal gg As Integer, ByRef d_o As DatoOrarioStd, ByVal cm As RaccaCondMeteo)
        If gg < calc_gg Then
            Return
        End If
        ExCalcola(d_o, cm)
    End Sub
    Protected MustOverride Sub ExCalcola(ByRef d_o As DatoOrarioStd, ByVal cm As RaccaCondMeteo)
End Class

Public Class ProbInfFactory_Std
    Inherits ProbInfFactory
    Protected Overrides Sub ExCalcola(ByRef d_o As DatoOrarioStd, ByVal cm As RaccaCondMeteo)
        'AC4=SE(E(A4>='Parametri Modello'!$C$25;U4>='Parametri Modello'!$C$33;U4<='Parametri Modello'!$C$35);  'Parametri Modello'!$C$32 * (((U4 - 'Parametri Modello'!$C$33) / ('Parametri Modello'!$C$34 - 'Parametri Modello'!$C$33)) ^ ('Parametri Modello'!$C$36 * (('Parametri Modello'!$C$34 - 'Parametri Modello'!$C$33) / ('Parametri Modello'!$C$35 - 'Parametri Modello'!$C$34)))) * ((('Parametri Modello'!$C$35 - U4) / ('Parametri Modello'!$C$35 - 'Parametri Modello'!$C$34))) ^ 'Parametri Modello'!$C$36;0)
        d_o.INF_T = calc_T.Calc(d_o.TempMediaPerBagn)
        'AD4=SE('Algoritmo dati orari'!A4>='Parametri Modello'!$C$25; (1-EXP(-'Parametri Modello'!$C$37*'Algoritmo dati orari'!Q4))^'Parametri Modello'!$C$38;0)
        d_o.INF_BN = calc_BN.Calc(d_o.BagnSenzaPioggia)
    End Sub
End Class

Public Class ProbInfFactory_PomodoroOidio
    Inherits ProbInfFactory
    Protected Overrides Sub ExCalcola(ByRef d_o As DatoOrarioStd, ByVal cm As RaccaCondMeteo)
        If d_o.CalcoloPeriodiSecchi <= 0 Then
            Return
        End If
        'AC4=SE(E(P4>0;A4>='Parametri Modello'!$C$25;D4>='Parametri Modello'!$C$33;D4<='Parametri Modello'!$C$35);  'Parametri Modello'!$C$32 * (((D4 - 'Parametri Modello'!$C$33) / ('Parametri Modello'!$C$34 - 'Parametri Modello'!$C$33)) ^ ('Parametri Modello'!$C$36 * (('Parametri Modello'!$C$34 - 'Parametri Modello'!$C$33) / ('Parametri Modello'!$C$35 - 'Parametri Modello'!$C$34)))) * ((('Parametri Modello'!$C$35 - D4) / ('Parametri Modello'!$C$35 - 'Parametri Modello'!$C$34))) ^ 'Parametri Modello'!$C$36;0)
        d_o.INF_T = calc_T.Calc(d_o.Meteo.Temp)
        'AD4=SE(E(P4>0;'Algoritmo dati orari'!A4>='Parametri Modello'!$C$25); (1-EXP(-'Parametri Modello'!$C$37*'Algoritmo dati orari'!E4))^'Parametri Modello'!$C$38;0)
        d_o.INF_BN = calc_BN.Calc(d_o.Meteo.UmRel)
    End Sub
End Class

Public Class ProbInfFactory_BietolaOidio
    Inherits ProbInfFactory
    Protected Overrides Sub ExCalcola(ByRef d_o As DatoOrarioStd, ByVal cm As RaccaCondMeteo)
        If d_o.CalcoloPeriodiSecchi <= 0 Then
            Return
        End If
        'AC4=SE(E(P4>0;A4>='Parametri Modello'!$C$25;D4>='Parametri Modello'!$C$33;D4<='Parametri Modello'!$C$35);  'Parametri Modello'!$C$32 * (((D4 - 'Parametri Modello'!$C$33) / ('Parametri Modello'!$C$34 - 'Parametri Modello'!$C$33)) ^ ('Parametri Modello'!$C$36 * (('Parametri Modello'!$C$34 - 'Parametri Modello'!$C$33) / ('Parametri Modello'!$C$35 - 'Parametri Modello'!$C$34)))) * ((('Parametri Modello'!$C$35 - D4) / ('Parametri Modello'!$C$35 - 'Parametri Modello'!$C$34))) ^ 'Parametri Modello'!$C$36;0)
        d_o.INF_T = calc_T.Calc(d_o.Meteo.Temp)
        'AD4=SE(E(P4>0;'Algoritmo dati orari'!A4>='Parametri Modello'!$C$25); (1-EXP(-'Parametri Modello'!$C$37*'Algoritmo dati orari'!E4))^'Parametri Modello'!$C$38;0)
        d_o.INF_BN = calc_BN.Calc(d_o.Meteo.UmRel)
    End Sub
End Class

Public Class ProbInfFactory_BietolaCercospora
    Inherits ProbInfFactory
    Protected Overrides Sub ExCalcola(ByRef d_o As DatoOrarioStd, ByVal cm As RaccaCondMeteo)
        Dim bagnNormale = d_o.BagnSenzaPioggia 'Bagnatura normale
        If Not cm.CalcoloTempPeriodo_BagnNormale Then
            'Bagnatura con pioggia
            bagnNormale = d_o.BagnDopoPioggia
        End If
        If bagnNormale > 0 Then
            '=SE(E(S4>0;A4>='Parametri Modello'!$C$25;U4>='Parametri Modello'!$C$33;U4<='Parametri Modello'!$C$35);  'Parametri Modello'!$C$32 * (((U4 - 'Parametri Modello'!$C$33) / ('Parametri Modello'!$C$34 - 'Parametri Modello'!$C$33)) ^ ('Parametri Modello'!$C$36 * (('Parametri Modello'!$C$34 - 'Parametri Modello'!$C$33) / ('Parametri Modello'!$C$35 - 'Parametri Modello'!$C$34)))) * ((('Parametri Modello'!$C$35 - U4) / ('Parametri Modello'!$C$35 - 'Parametri Modello'!$C$34))) ^ 'Parametri Modello'!$C$36;0)
            d_o.INF_T = calc_T.Calc(d_o.TempMediaPerBagn)
            '=SE(E(S4>0;'Algoritmo dati orari'!A4>='Parametri Modello'!$C$25); (1-EXP(-'Parametri Modello'!$C$37*'Algoritmo dati orari'!S4))^'Parametri Modello'!$C$38;0)
            d_o.INF_BN = calc_BN.Calc(bagnNormale)
        End If
    End Sub
End Class



Public MustInherit Class LesioniFactory
    Public calc_T As RaccaMath.FunzioneBetaHau
    Public calc_BN As RaccaMath.FunzioneChapman
    Public Sub New()
        calc_T = New RaccaMath.FunzioneBetaHau
        calc_BN = New RaccaMath.FunzioneChapman
    End Sub
    Public Sub Calcola(ByRef d_o As DatoOrarioStd, ByRef d_o_prec As DatoOrarioStd)
        If d_o_prec Is Nothing Then
            Return
        End If
        ExCalcola(d_o, d_o_prec)
    End Sub
    Protected MustOverride Sub ExCalcola(ByRef d_o As DatoOrarioStd, ByRef d_o_prec As DatoOrarioStd)
End Class

Public Class LesioniFactory_Std
    Inherits LesioniFactory
    Protected Overrides Sub ExCalcola(ByRef d_o As DatoOrarioStd, ByRef d_o_prec As DatoOrarioStd)
        'AF5=SE(E((AF4+AE5)>0;D5>='Parametri Modello'!$C$48;D5<='Parametri Modello'!$C$50); 'Parametri Modello'!$C$47* (((D5 - 'Parametri Modello'!$C$48)/('Parametri Modello'!$C$49-'Parametri Modello'!$C$48)) ^ ( 'Parametri Modello'!$C$51 * ('Parametri Modello'!$C$49-'Parametri Modello'!$C$48)/('Parametri Modello'!$C$50-'Parametri Modello'!$C$49))) *((('Parametri Modello'!$C$50 -D5)/('Parametri Modello'!$C$50 - 'Parametri Modello'!$C$49))^'Parametri Modello'!$C$51);0)
        If d_o_prec.CrescitaLesioni + d_o.INF_Prob > 0 Then
            d_o.CrescitaLesioni = calc_T.Calc(d_o.Meteo.Temp)
        End If
    End Sub
End Class

Public Class LesioniFactory_PomodoroAlternaria
    Inherits LesioniFactory
    Protected Overrides Sub ExCalcola(ByRef d_o As DatoOrarioStd, ByRef d_o_prec As DatoOrarioStd)
        'AF5=SE(E((AF4+AE5)>0;D5>='Parametri Modello'!$C$50;D5<='Parametri Modello'!$C$52); 'Parametri Modello'!$C$49 * (((D5 - 'Parametri Modello'!$C$50) / ('Parametri Modello'!$C$51 - 'Parametri Modello'!$C$50)) ^ ('Parametri Modello'!$C$53 * (('Parametri Modello'!$C$51 - 'Parametri Modello'!$C$50) / ('Parametri Modello'!$C$52 - 'Parametri Modello'!$C$51)))) * ((('Parametri Modello'!$C$52 - D5) / ('Parametri Modello'!$C$52 - 'Parametri Modello'!$C$51))) ^ 'Parametri Modello'!$C$53;0)
        If d_o_prec.Les_T + d_o.INF_Prob > 0 Then
            d_o.Les_T = calc_T.Calc(d_o.Meteo.Temp)
        End If
        If d_o.Les_T > 0 Then
            '=SE(AF5>0;(1-EXP(-'Parametri Modello'!$C$54*E5))^'Parametri Modello'!$C$55;0)
            d_o.Les_UR = calc_BN.Calc(d_o.Meteo.UmRel)
        End If
    End Sub
End Class

Public Class LesioniFactory_BietolaCercospora
    Inherits LesioniFactory
    Protected Overrides Sub ExCalcola(ByRef d_o As DatoOrarioStd, ByRef d_o_prec As DatoOrarioStd)
        d_o.CrescitaLesioni = calc_T.y
    End Sub
End Class

Public Class LesioniFactory_PatataAlternaria
    Inherits LesioniFactory
    Protected Overrides Sub ExCalcola(ByRef d_o As DatoOrarioStd, ByRef d_o_prec As DatoOrarioStd)
        'AF5=SE(E((AF4+AE5)>0;D5>='Parametri Modello'!$C$50;D5<='Parametri Modello'!$C$52); 'Parametri Modello'!$C$49 * (((D5 - 'Parametri Modello'!$C$50) / ('Parametri Modello'!$C$51 - 'Parametri Modello'!$C$50)) ^ ('Parametri Modello'!$C$53 * (('Parametri Modello'!$C$51 - 'Parametri Modello'!$C$50) / ('Parametri Modello'!$C$52 - 'Parametri Modello'!$C$51)))) * ((('Parametri Modello'!$C$52 - D5) / ('Parametri Modello'!$C$52 - 'Parametri Modello'!$C$51))) ^ 'Parametri Modello'!$C$53;0)
        If d_o_prec.Les_T + d_o.INF_Prob > 0 Then
            d_o.Les_T = calc_T.Calc(d_o.Meteo.Temp)
        End If
        If d_o.Les_T > 0 Then
            '=SE(AF5>0;(1-EXP(-'Parametri Modello'!$C$54*E5))^'Parametri Modello'!$C$55;0)
            d_o.Les_UR = calc_BN.Calc(d_o.Meteo.UmRel)
        End If
    End Sub
End Class



Public Class LatenzaFactory
    Public ReadOnly calc_ComeMedia As Boolean 'True -> latenza gg come media latenze hh, False -> latenza gg come max latenze hh
    Public ReadOnly calc_T As RaccaMath.FunzioneBetaHau
    Public Sub New(ByVal comeMedia As Boolean)
        calc_ComeMedia = comeMedia
        calc_T = New RaccaMath.FunzioneBetaHau
    End Sub
End Class



Public Class RaccaModelloStd
    Inherits RaccaModello

    Protected ReadOnly mPInf As ProbInfFactory
    Protected ReadOnly mLesioni As LesioniFactory
    Protected ReadOnly mLatenza As LatenzaFactory

    Public Sub New(ByVal condMeteo As RaccaCondMeteo, ByVal pInf As ProbInfFactory, ByVal les As LesioniFactory, ByVal lat As LatenzaFactory)
        MyBase.New(condMeteo)
        mPInf = pInf
        mLesioni = les
        mLatenza = lat
    End Sub

    Protected Overrides Function CalcolaDatiOrari(ByVal datiMeteo As MeteoReadOnlyList, ByVal funLAI As RaccaFunzioneLAI) As List(Of RaccaDatoOrario)
        Dim elencoOrari As New List(Of RaccaDatoOrario)

        Dim d_o As DatoOrarioStd
        Dim d_o_prec As DatoOrarioStd = Nothing
        Dim d_o_prec2 As DatoOrarioStd = Nothing

        Dim gGiuliano As GiornoGiuliano = Nothing

        For Each d In datiMeteo

            d_o = New DatoOrarioStd(d, mCondMeteo, d_o_prec)

            If gGiuliano IsNot Nothing Then
                gGiuliano.SetDate(d.DataOra)
            Else
                gGiuliano = New GiornoGiuliano(d.DataOra)
            End If

            funLAI.Calcola(gGiuliano.CumGiorno, d_o, d_o_prec)

            mPInf.Calcola(gGiuliano.Giorno, d_o, mCondMeteo)

            mLesioni.Calcola(d_o, d_o_prec)

            If d_o_prec2 IsNot Nothing Then
                If d_o_prec2.PeriodoLatenza + d_o.INF_Prob > 0 Then
                    'si notino gli indici delle righe della formula...
                    'ho bisogno di avere anche un valore della riga successiva a quella che devo valorizzare (AA6)
                    'AJ5 = SE(E((AJ4+AE6)>0;D5>'Parametri Modello'!$C$61;D5<'Parametri Modello'!$C$63);'Parametri Modello'!$C$60* (((D5 - 'Parametri Modello'!$C$61)/('Parametri Modello'!$C$62-'Parametri Modello'!$C$61)) ^ ( 'Parametri Modello'!$C$64 * ('Parametri Modello'!$C$62-'Parametri Modello'!$C$61)/('Parametri Modello'!$C$63-'Parametri Modello'!$C$62))) *((('Parametri Modello'!$C$63 - D5)/('Parametri Modello'!$C$63 - 'Parametri Modello'!$C$62))^'Parametri Modello'!$C$64);0)
                    d_o_prec.PeriodoLatenza = mLatenza.calc_T.Calc(d_o_prec.Meteo.Temp)
                End If
            End If

            elencoOrari.Add(d_o)

            d_o_prec2 = d_o_prec
            d_o_prec = d_o
        Next

        Return elencoOrari
    End Function

    Protected Class DatoGGStd
        Inherits DatoGG

        Public INF_T As Decimal
        Public INF_BN As Decimal
        Public INF_Prob As Decimal
        Public CrescitaLesioni As Decimal
        Public IndiceRischio As Decimal
        Public PeriodoLatenza As Decimal
        Public IndiceRischioMediato As Decimal
        Public Overrides Sub Output(ByRef output As OutputModello, ByRef indic As OutputIndicator)
            output.AddField(Data)
            output.AddField(Temp)
            output.AddField(UR)
            output.AddField(Pioggia)
            output.AddField(Bagn_ore)
            output.AddField(Bagn_mm)
            output.AddField(LAI)
            'output.AddField(INF_T)
            'output.AddField(INF_BN)
            output.AddField(INF_Prob)
            output.AddField(CrescitaLesioni)
            output.AddField(IndiceRischio)
            output.AddField(PeriodoLatenza)
            output.AddField(IndiceRischioMediato)
            If indic IsNot Nothing Then
                output.AddField(indic.colorForVal(IndiceRischioMediato))
            End If
        End Sub
        Public Overrides Function Indicatore() As Decimal
            Return IndiceRischioMediato
        End Function
    End Class

    Private Class RunningGGStd
        Inherits RunningGG

        Private ReadOnly calcoloLatenzaComeMedia As Boolean
        Private sum_periodolatenza As Decimal
        Private max_periodolatenza As Decimal
        Public Sub New(ByVal latenzaComeMedia As Boolean)
            MyBase.New()
            calcoloLatenzaComeMedia = latenzaComeMedia
        End Sub
        Public Function Create(ByVal d_hh As DatoOrarioStd) As DatoGG

            Dim d_gg = New DatoGGStd

            AbsCreate(d_gg, d_hh)

            d_gg.INF_T = d_hh.INF_T
            d_gg.INF_BN = d_hh.INF_BN
            d_gg.INF_Prob = d_hh.INF_Prob
            d_gg.CrescitaLesioni = d_hh.CrescitaLesioni
            d_gg.IndiceRischio = d_hh.IndiceRischio
            d_gg.IndiceRischioMediato = 0

            sum_periodolatenza = d_hh.PeriodoLatenza
            max_periodolatenza = d_hh.PeriodoLatenza
            Return d_gg
        End Function
        Public Sub Add(ByRef d_gg As DatoGGStd, ByVal d_hh As DatoOrarioStd)

            AbsAdd(d_gg, d_hh)

            d_gg.INF_T = Math.Max(d_gg.INF_T, d_hh.INF_T)
            d_gg.INF_BN = Math.Max(d_gg.INF_BN, d_hh.INF_BN)
            d_gg.INF_Prob = Math.Max(d_gg.INF_Prob, d_hh.INF_Prob)
            d_gg.CrescitaLesioni = Math.Max(d_gg.CrescitaLesioni, d_hh.CrescitaLesioni)
            d_gg.IndiceRischio = Math.Max(d_gg.IndiceRischio, d_hh.IndiceRischio)

            sum_periodolatenza += d_hh.PeriodoLatenza
            max_periodolatenza = Math.Max(max_periodolatenza, d_hh.PeriodoLatenza)
        End Sub
        Public Function Flush(ByVal d_gg As DatoGGStd) As DatoGGStd

            AbsFlush(d_gg)

            If sum_periodolatenza > 0 Then
                If calcoloLatenzaComeMedia Then
                    d_gg.PeriodoLatenza = Math.Round(1D / (sum_periodolatenza / hh))
                Else
                    d_gg.PeriodoLatenza = Math.Round(1D / max_periodolatenza)
                End If
            End If
            Return d_gg
        End Function
    End Class

    Protected Overrides Function CalcolaDatiGiornalieri(ByVal elencoOrari As List(Of RaccaDatoOrario)) As List(Of DatoGG)
        Dim elencoGG As New List(Of DatoGG)

        Dim d_gg As DatoGGStd = Nothing
        Dim runner As New RunningGGStd(mLatenza.calc_ComeMedia)

        For Each d_hh In elencoOrari

            If d_gg Is Nothing OrElse d_gg.Data.DayOfYear <> d_hh.Meteo.DataOra.DayOfYear Then

                If d_gg IsNot Nothing Then

                    elencoGG.Add(runner.Flush(d_gg))
                End If

                d_gg = runner.Create(d_hh)
            Else

                runner.Add(d_gg, d_hh)
            End If

        Next

        If d_gg IsNot Nothing Then

            elencoGG.Add(runner.Flush(d_gg))
        End If

        Dim idx As Integer = 0
        While idx < elencoGG.Count

            d_gg = elencoGG(idx)

            If d_gg.PeriodoLatenza > 0 Then

                'Calcolo l'indice di rischio mediato sulla latenza
                'N.B.   Calcolando l'indice di rischio mediato in questo modo
                '       avrò sempre il valore per l'ultimo giorno della serie (l'indicatore) vicino allo 0
                '       perchè la sommatoria considera il futuro
                'Dim last As Integer = Math.Min(dati_gg.Count - 1, idx + d_gg.PeriodoLatenza)
                'While last >= idx
                '    d_gg.IndiceRischioMediato += dati_gg(last).IndiceRischio
                '    last -= 1
                'End While

                Dim prev As Integer = Math.Max(0, idx - d_gg.PeriodoLatenza - 1)
                While prev <= idx
                    Dim prev_gg As DatoGGStd = elencoGG(prev)
                    d_gg.IndiceRischioMediato += prev_gg.IndiceRischio
                    prev += 1
                End While

                d_gg.IndiceRischioMediato /= d_gg.PeriodoLatenza
            End If

            idx += 1
        End While

        Return elencoGG
    End Function

    Protected Overrides Function CreaOutput(ByRef output As OutputModello) As OutputIndicator

        output.aggiungiColonna("Data", GetType(DateTime), Gias.Data, "dd/MM/yyyy")
        output.aggiungiColonna("Temperatura", GetType(Decimal), Gias.Temperatura, "0.000")
        output.aggiungiColonna("UmiditaRelativa", GetType(Decimal), Gias.UmiditaRelativa, "0.000")
        output.aggiungiColonna("Pioggia", GetType(Decimal), Gias.Pioggia, "0.000")
        output.aggiungiColonna("Bagnatura", GetType(Integer), Gias.BagnaturaFogliare & " (" & Gias.Ore & ")", "0")
        output.aggiungiColonna("Bagnatura_mm", GetType(Decimal), Gias.BagnaturaFogliare & " (mm)", "0.000")
        output.aggiungiColonna("LAI", GetType(Decimal), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.RaccaModelloStd_indiceAreaFogliareRel, "0.000")
        'output.aggiungiColonna("INF_T", GetType(Decimal), "INF_T", "0.000")
        'output.aggiungiColonna("INF_BN", GetType(Decimal), "INF_BN", "0.000")
        output.aggiungiColonna("INF_Prob", GetType(Decimal), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.ProbabilitaDiInfezione_, "0.000")
        output.aggiungiColonna("CrescitaLesioni", GetType(Decimal), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.RaccaModelloStd_crescitaDelleLesioni, "0.000")
        output.aggiungiColonna("IndiceRischio", GetType(Decimal), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.IndiceDiRischio, "0.000")
        output.aggiungiColonna("PeriodoLatenza", GetType(Decimal), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.RaccaModelloStd_periodoDiLatenza, "0")
        output.aggiungiColonna("IndiceRischioMediato", GetType(Decimal), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.RaccaModelloStd_indiceDiRischioMediatoSullaLatenza, "0.000")

        Dim indic As OutputIndicator = Nothing

        If mParametriIndicatori IsNot Nothing Then

            Dim limiti = mParametriIndicatori.BandsValues.ToList()
            limiti.Add(System.Decimal.MaxValue)
            indic = output.aggiungiIndicatore("Indic", limiti.ToArray(), OutputIndicator.enum_IndicatorType.Colors).setFieldVal("IndiceRischioMediato")

        End If

        Return indic
    End Function

End Class

'**************************************************************************************************

#Region "Modelli Micotox"

Public Class RaccaModelloMaisMicotox
    Inherits RaccaModello

    Private Const MicotoxNum As Integer = 5 '0..5 -> 6 elementi

    Private Class Micotox_PInf
        Public CalcPInf_T As RaccaMath.FunzioneBetaHau
        Public CalcPInf_BN As RaccaMath.FunzioneChapman
        Public AW1 As Decimal
        Public Micotox As String
        Public Sub New(ByVal Tmin As Decimal, ByVal Topt As Decimal, ByVal Tmax As Decimal, ByVal Ty As Decimal, ByVal Tn As Decimal, ByVal BNb As Decimal, ByVal BNc As Decimal, ByVal _aw1 As Decimal, ByVal str As String)
            CalcPInf_T = New RaccaMath.FunzioneBetaHau
            CalcPInf_BN = New RaccaMath.FunzioneChapman

            CalcPInf_T.min = Tmin
            CalcPInf_T.opt = Topt
            CalcPInf_T.max = Tmax
            CalcPInf_T.y = Ty
            CalcPInf_T.n = Tn
            CalcPInf_BN.b = BNb
            CalcPInf_BN.c = BNc

            AW1 = _aw1

            Micotox = str
        End Sub
    End Class

    Private Class ParamMaturazione
        Public TemperaturaBaseGDD As Decimal
        Public Giuliano_SumGDD As Integer
        Public LB_FaseMaturazione_SumGDD As Decimal
        Public UB_FaseMaturazione_SumGDD As Decimal
        Public TDissecGran_a As Decimal
        Public TDissecGran_b As Decimal
        Public UmGranellaR6 As Decimal
    End Class

    Private mCalcPInf_giuliano As Integer
    Private mCalcPInf(MicotoxNum) As Micotox_PInf
    Private mParamMaturazione As ParamMaturazione

    Public Sub New(ByVal condMeteo As RaccaCondMeteo)
        MyBase.New(condMeteo)

        mParamMaturazione = New ParamMaturazione With {
            .TemperaturaBaseGDD = 6,
            .Giuliano_SumGDD = 105, 'mCalcLAI_giuliano,
            .LB_FaseMaturazione_SumGDD = 681.93,
            .UB_FaseMaturazione_SumGDD = 1343.35,
            .TDissecGran_a = -0.7133,
            .TDissecGran_b = 0.0202,
            .UmGranellaR6 = 80
        }

        mCalcPInf_giuliano = 105

        mCalcPInf = {
            New Micotox_PInf(10.8301, 35.0047, 41.796, 1, 0.9291, 2.2514, 1.5878, 85, "Aspergillus flavus, Aspergillus parasiticus"),
            New Micotox_PInf(3.5, 24.9636, 34.5991, 1, 1.3849, 0.1791, 1.9837, 95, "Fusarium graminearum, Fusarium culmorum"),
            New Micotox_PInf(0.01176, 25.03, 35.59, 1, 1.679, 0.1796, 1.986, 95, "Fusarium sporotrichioides"),
            New Micotox_PInf(10.4289, 30.041, 35.1844, 1, 1.6153, 0.1774, 1.9707, 91, "Fusarium verticillioides (moniliforme), Fusarium proliferatum"),
            New Micotox_PInf(0, 36.09, 39.78, 1, 1.296, 0.1821, 2.056, 83, "Aspergillus ochraces"),
            New Micotox_PInf(0, 23.02, 31.4, 1, 1.652, 0.1789, 1.981, 83, "Penicillium verrucosum")
        }

        'mParametriIndicatori.ScaleMax = 0.5
        'mParametriIndicatori.BandsValues = {0.15, 0.3}
    End Sub

    Protected Class DatoOrarioMicotox
        Inherits RaccaDatoOrario

        Public Class DatoInf
            Public INF_T As Decimal
            Public INF_BN As Decimal
            Public AW_Period As Decimal
            Public Sub New()
                INF_T = 0
                INF_BN = 0
                AW_Period = 0
            End Sub
            Public ReadOnly Property INF_Prob As Decimal
                Get
                    Return INF_T * INF_BN
                End Get
            End Property
        End Class

        Public INF(MicotoxNum) As DatoInf
        Public Sub New(ByVal dm As MeteoDSSItem, ByVal cond As RaccaCondMeteo, ByVal prec As RaccaDatoOrario)
            MyBase.New(dm, cond, prec)

            For idx = 0 To MicotoxNum
                INF(idx) = New DatoInf
            Next
        End Sub
    End Class

    Protected Overrides Function CalcolaDatiOrari(ByVal datiMeteo As MeteoReadOnlyList, ByVal funLAI As RaccaFunzioneLAI) As List(Of RaccaDatoOrario)
        Dim elencoOrari As New List(Of RaccaDatoOrario)

        Dim d_o As DatoOrarioMicotox
        Dim d_o_prec As DatoOrarioMicotox = Nothing

        Dim gGiuliano As GiornoGiuliano = Nothing

        For Each d In datiMeteo

            d_o = New DatoOrarioMicotox(d, mCondMeteo, d_o_prec)

            For idx = 0 To MicotoxNum
                d_o.INF(idx).AW_Period = If(d_o.Meteo.UmRel >= mCalcPInf(idx).AW1, 1, 0)
            Next

            If gGiuliano IsNot Nothing Then
                gGiuliano.SetDate(d.DataOra)
            Else
                gGiuliano = New GiornoGiuliano(d.DataOra)
            End If

            funLAI.Calcola(gGiuliano.CumGiorno, d_o, d_o_prec)

            If gGiuliano.Giorno >= mCalcPInf_giuliano Then
                For idx = 0 To MicotoxNum
                    d_o.INF(idx).INF_T = mCalcPInf(idx).CalcPInf_T.Calc(d_o.TempMediaPerBagn)
                    d_o.INF(idx).INF_BN = mCalcPInf(idx).CalcPInf_BN.Calc(d_o.BagnSenzaPioggia)
                Next
            End If

            elencoOrari.Add(d_o)

            d_o_prec = d_o
        Next

        Return elencoOrari
    End Function

    Protected Class DatoGGMicotox
        Inherits DatoGG

        Public Class DatoInf
            Public INF_Prob As Decimal
            Public AW_Period As Decimal
            Public IndRischioCum As Decimal
            Public IndRischioMed5GG As Decimal
        End Class

        Public GDD As Decimal
        Public SumGDD As Decimal
        Public TassoDissec As Decimal
        Public UmGran As Decimal
        Public INF(MicotoxNum) As DatoInf

        Public Overrides Sub Output(ByRef output As OutputModello, ByRef indic As OutputIndicator)
            output.AddField(Data)
            output.AddField(Temp)
            output.AddField(UR)
            output.AddField(Pioggia)
            output.AddField(Bagn_ore)
            output.AddField(Bagn_mm)
            output.AddField(LAI)
            output.AddField(GDD)
            output.AddField(SumGDD)
            output.AddField(TassoDissec)
            output.AddField(UmGran)

            Dim sum5gg As Decimal = INF(0).IndRischioMed5GG
            Dim max5gg As Decimal = INF(0).IndRischioMed5GG
            For idx = 1 To MicotoxNum
                sum5gg += INF(idx).IndRischioMed5GG
                max5gg = Math.Max(max5gg, INF(idx).IndRischioMed5GG)
            Next

            output.AddField(sum5gg / CDec(MicotoxNum))
            output.AddField(max5gg)

            'output.AddField(indic.colorForVal(IndiceRischioMediato))
        End Sub

        Public Overrides Function Indicatore() As Decimal
            Return 0
        End Function
    End Class
    Private Class RunningGGMicotox
        Inherits RunningGG

        Private pMaturazione As ParamMaturazione
        Private GGiuliano As GiornoGiuliano
        Private tmin As Decimal
        Private tmax As Decimal
        Private d_gg_prec As DatoGGMicotox
        Private stack5gg(MicotoxNum) As Queue(Of Decimal)

        Public Sub New(ByVal pm As ParamMaturazione)
            MyBase.New()

            pMaturazione = pm

            d_gg_prec = Nothing
            For idx = 0 To MicotoxNum
                stack5gg(idx) = New Queue(Of Decimal)
            Next

            GGiuliano = Nothing
        End Sub
        Public Function Create(ByVal d_hh As DatoOrarioMicotox) As DatoGG

            Dim d_gg = New DatoGGMicotox

            AbsCreate(d_gg, d_hh)

            tmin = d_hh.Meteo.Temp
            tmax = d_hh.Meteo.Temp

            For idx = 0 To MicotoxNum
                d_gg.INF(idx) = New DatoGGMicotox.DatoInf With {
                    .INF_Prob = d_hh.INF(idx).INF_Prob,
                    .AW_Period = d_hh.INF(idx).AW_Period
                }
            Next

            Return d_gg
        End Function
        Public Sub Add(ByRef d_gg As DatoGGMicotox, ByVal d_hh As DatoOrarioMicotox)

            AbsAdd(d_gg, d_hh)

            tmin = Math.Min(tmin, d_hh.Meteo.Temp)
            tmax = Math.Max(tmax, d_hh.Meteo.Temp)

            For idx = 0 To MicotoxNum
                d_gg.INF(idx).INF_Prob = Math.Max(d_gg.INF(idx).INF_Prob, d_hh.INF(idx).INF_Prob)
                d_gg.INF(idx).AW_Period += d_hh.INF(idx).AW_Period
                d_gg.INF(idx).IndRischioCum = 0
                d_gg.INF(idx).IndRischioMed5GG = 0
            Next
        End Sub
        Public Function Flush(ByVal d_gg As DatoGGMicotox) As DatoGGMicotox

            AbsFlush(d_gg)

            d_gg.GDD = Math.Max(0, (tmax + tmin) / 2 - pMaturazione.TemperaturaBaseGDD)

            If GGiuliano IsNot Nothing Then
                GGiuliano.SetDate(d_gg.Data)
            Else
                GGiuliano = New GiornoGiuliano(d_gg.Data)
            End If

            If GGiuliano.CumGiorno >= pMaturazione.Giuliano_SumGDD Then
                d_gg.SumGDD = d_gg_prec.SumGDD + d_gg.GDD
            Else
                d_gg.SumGDD = 0
            End If

            d_gg.TassoDissec = pMaturazione.TDissecGran_a + pMaturazione.TDissecGran_b * (d_gg.Temp * 1.8D + 32D)

            If d_gg_prec IsNot Nothing Then
                d_gg.UmGran = d_gg_prec.UmGran
            Else
                d_gg.UmGran = pMaturazione.UmGranellaR6
            End If

            If pMaturazione.LB_FaseMaturazione_SumGDD <= d_gg.SumGDD AndAlso d_gg.SumGDD <= pMaturazione.UB_FaseMaturazione_SumGDD Then
                d_gg.UmGran -= d_gg.TassoDissec
            End If

            For idx = 0 To MicotoxNum

                d_gg.INF(idx).AW_Period /= hh

                If d_gg_prec IsNot Nothing Then
                    d_gg.INF(idx).IndRischioCum = d_gg_prec.INF(idx).IndRischioCum + d_gg.INF(idx).INF_Prob
                End If

                stack5gg(idx).Enqueue(d_gg.INF(idx).INF_Prob)
                If stack5gg(idx).Count() > 5 Then
                    stack5gg(idx).Dequeue()
                End If
                If stack5gg(idx).Count() = 5 Then
                    d_gg.INF(idx).IndRischioMed5GG = stack5gg(idx).Sum() / 5D
                End If

            Next

            d_gg_prec = d_gg

            Return d_gg
        End Function
    End Class

    Protected Overrides Function CalcolaDatiGiornalieri(ByVal elencoOrari As List(Of RaccaDatoOrario)) As List(Of DatoGG)
        Dim elencoGG As New List(Of DatoGG)

        Dim d_gg As DatoGGMicotox = Nothing
        Dim runner As New RunningGGMicotox(mParamMaturazione)

        For Each d_hh In elencoOrari

            If d_gg Is Nothing OrElse d_gg.Data.DayOfYear <> d_hh.Meteo.DataOra.DayOfYear Then

                If d_gg IsNot Nothing Then

                    elencoGG.Add(runner.Flush(d_gg))
                End If

                d_gg = runner.Create(d_hh)
            Else

                runner.Add(d_gg, d_hh)
            End If

        Next

        If d_gg IsNot Nothing Then

            elencoGG.Add(runner.Flush(d_gg))
        End If

        Return elencoGG
    End Function

    Protected Overrides Function CreaOutput(ByRef output As OutputModello) As OutputIndicator
        'i18n Growning Degrees Days
        output.aggiungiColonna("Data", GetType(DateTime), Gias.Data, "dd/MM/yyyy")
        output.aggiungiColonna("Temperatura", GetType(Decimal), Gias.Temperatura, "0.000")
        output.aggiungiColonna("UmiditaRelativa", GetType(Decimal), Gias.UmiditaRelativa, "0.000")
        output.aggiungiColonna("Pioggia", GetType(Decimal), Gias.Pioggia, "0.000")
        output.aggiungiColonna("Bagnatura", GetType(Integer), Gias.BagnaturaFogliare & " (" & Gias.Ore & ")", "0")
        output.aggiungiColonna("Bagnatura_mm", GetType(Decimal), Gias.BagnaturaFogliare & " (mm)", "0.000")
        output.aggiungiColonna("LAI", GetType(Decimal), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.RaccaModelloStd_indiceAreaFogliareRel, "0.000")
        output.aggiungiColonna("GDD", GetType(Decimal), "GDD", "0.000")
        output.aggiungiColonna("SommaGDD", GetType(Decimal), "Somma GDD", "0.000")
        output.aggiungiColonna("TassoDissec", GetType(Decimal), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.RaccaModelloMaisMicotox_tassoDiDisseccamento, "0.000")
        output.aggiungiColonna("UmGranella", GetType(Decimal), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.RaccaModelloMaisMicotox_umiditaGranella & " R2-R6", "0.000")
        output.aggiungiColonna("Media5gg", GetType(Decimal), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.RaccaModelloMaisMicotox_mediaIndiceRischioCinqueGiorni, "0.000")
        output.aggiungiColonna("Max5gg", GetType(Decimal), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.RaccaModelloMaisMicotox_maxIndiceRischioCinqueGiorni, "0.000")

        'Dim limiti = mParametriIndicatori.BandsValues.ToList()
        'limiti.Add(System.Decimal.MaxValue)
        'Dim indic As OutputIndicator = output.aggiungiIndicatore("Indic", limiti.ToArray(), OutputIndicator.enum_IndicatorType.Colors).setFieldVal("IndiceRischioMediato")

        'Return indic
        Return Nothing
    End Function

End Class

#End Region

'**************************************************************************************************

Public Class Racca
    Inherits AbstractModello

    Private ReadOnly mFunLAI As RaccaFunzioneLAI
    Private ReadOnly mModello As RaccaModello
    Private ReadOnly mNomeModello As String

    Public Sub New(ByVal datiMeteo As MeteoReadOnlyList, ByVal Mod_Cod As Integer, ByVal Veg_Cod As Integer, ByVal Av_Cod As Integer, ByVal parametriAggiuntivi As String)
        MyBase.New(datiMeteo)

        mFunLAI = Nothing
        mModello = Nothing
        mNomeModello = "Sconosciuto"

        Dim objParams = JObject.Parse(parametriAggiuntivi)

        Dim condMeteo As New RaccaCondMeteo With {
            .CalcoloPeriodi_DatiMisurati = False,
            .CalcoloTempPeriodo_BagnNormale = False,
            .SogliaPioggiaCorrBagn = 0.2,
            .SogliaURCorrBagn = 90,
            .SogliaPioggiaPerBagn = 0.2
        }

        Dim parIndicatori As RaccaModello.ParametriIndicatori = Nothing

        If Mod_Cod = enum_ModelliPrevisionali.Racca_MaisMicotox Then

            'calcLAI_gg = 105
            'CalcLAI_ClasseMaturazione = 100 
            mFunLAI = New RaccaFunzioneLAI_Mais(Veg_Cod, 105, 100)
            mModello = New RaccaModelloMaisMicotox(condMeteo)
            mNomeModello = "" '"Micotox del mais"

        Else

            Dim pInf As ProbInfFactory = Nothing
            Dim les As LesioniFactory = Nothing
            Dim lat As LatenzaFactory = Nothing

            parIndicatori = New RaccaModello.ParametriIndicatori(1, {0.1, 0.2, 0.5})

            Select Case Mod_Cod

                Case enum_ModelliPrevisionali.Racca_PeroPom

                    mFunLAI = New RaccaFunzioneLAI_Std(Veg_Cod, 91, 0)

                    condMeteo.CalcoloTempPeriodo_BagnNormale = True

                    pInf = New ProbInfFactory_Std
                    pInf.calc_gg = 91 'C25
                    pInf.calc_T.min = 10 'C33
                    pInf.calc_T.opt = 17.563 'C34
                    pInf.calc_T.max = 30 'C35
                    pInf.calc_T.y = 1 'C32
                    pInf.calc_T.n = 1.64 'C36
                    pInf.calc_BN.b = 0.202 'C37
                    pInf.calc_BN.c = 13.48 'C38

                    les = New LesioniFactory_Std
                    les.calc_T.y = 1 'C47
                    les.calc_T.min = 0 'C48
                    les.calc_T.max = 26 'C50
                    les.calc_T.opt = 20.748 'C49
                    les.calc_T.n = 0.462 'C51

                    lat = New LatenzaFactory(True)
                    lat.calc_T.y = 0.242 'C60
                    lat.calc_T.min = 10 'C61
                    lat.calc_T.max = 30 'C63
                    lat.calc_T.opt = 21.457 'C62
                    lat.calc_T.n = 0.488 'C64

                    parIndicatori = New RaccaModello.ParametriIndicatori(0.5, {0.05, 0.15})

                    mNomeModello = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Racca_peronosporaDelPomodoro

                Case enum_ModelliPrevisionali.Racca_AlterPom

                    mFunLAI = New RaccaFunzioneLAI_Std(Veg_Cod, 91, 0)

                    condMeteo.CalcoloTempPeriodo_BagnNormale = True

                    pInf = New ProbInfFactory_Std
                    pInf.calc_gg = 91 'C25
                    pInf.calc_T.min = 10 'C33
                    pInf.calc_T.opt = 23.93 'C34
                    pInf.calc_T.max = 35 'C35
                    pInf.calc_T.y = 1 'C32
                    pInf.calc_T.n = 0.244 'C36
                    pInf.calc_BN.b = 0.12 'C37
                    pInf.calc_BN.c = 2.019 'C38

                    les = New LesioniFactory_PomodoroAlternaria
                    les.calc_T.y = 1 'C49
                    les.calc_T.min = 10 'C50
                    les.calc_T.max = 36 'C52
                    les.calc_T.opt = 28.421 'C51
                    les.calc_T.n = 0.599 'C53
                    les.calc_BN.b = 0.062 'C54
                    les.calc_BN.c = 22.786 'C55

                    lat = New LatenzaFactory(False) '??? nel file excel è così... è una svista?
                    lat.calc_T.y = 0.148 'C63
                    lat.calc_T.min = 10 'C64
                    lat.calc_T.max = 35 'C66
                    lat.calc_T.opt = 23.19 'C65
                    lat.calc_T.n = 0.391 'C67

                    parIndicatori = New RaccaModello.ParametriIndicatori(0.5, {0.03, 0.06})

                    mNomeModello = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Racca_alternariaDelPomodoro

                Case enum_ModelliPrevisionali.Racca_OidioPom

                    mFunLAI = New RaccaFunzioneLAI_Std(Veg_Cod, 91, 0)

                    condMeteo.CalcoloTempPeriodo_BagnNormale = True

                    pInf = New ProbInfFactory_PomodoroOidio
                    pInf.calc_gg = 91 'C25
                    pInf.calc_T.min = 0 'C33
                    pInf.calc_T.opt = 24.5409 'C34
                    pInf.calc_T.max = 35.1688 'C35
                    pInf.calc_T.y = 1 'C32
                    pInf.calc_T.n = 1.0772 'C36
                    pInf.calc_BN.b = 0.0438 'C37
                    pInf.calc_BN.c = 1.1773 'C38

                    les = New LesioniFactory_Std
                    les.calc_T.y = 1 'C47
                    les.calc_T.min = 15.963 'C48
                    les.calc_T.max = 28.006 'C50
                    les.calc_T.opt = 23.959 'C49
                    les.calc_T.n = 0.898 'C51

                    lat = New LatenzaFactory(True)
                    lat.calc_T.y = 0.145 'C60
                    lat.calc_T.min = 5 'C61
                    lat.calc_T.max = 35 'C63
                    lat.calc_T.opt = 21.082 'C62
                    lat.calc_T.n = 2.923 'C64

                    parIndicatori = New RaccaModello.ParametriIndicatori(1.5, {0.3, 0.5})

                    mNomeModello = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Racca_oidioDelPomodoro

                Case enum_ModelliPrevisionali.Racca_BotriPom

                    mFunLAI = New RaccaFunzioneLAI_Std(Veg_Cod, 91, 0)

                    pInf = New ProbInfFactory_Std
                    pInf.calc_gg = 91 'C25
                    pInf.calc_T.min = 5 'C33
                    pInf.calc_T.opt = 22.5924 'C34
                    pInf.calc_T.max = 35 'C35
                    pInf.calc_T.y = 1 'C32
                    pInf.calc_T.n = 0.1619 'C36
                    pInf.calc_BN.b = 0.4741 'C37
                    pInf.calc_BN.c = 1.9924 'C38

                    les = New LesioniFactory_Std
                    les.calc_T.y = 1 'C47
                    les.calc_T.min = 0 'C48
                    les.calc_T.max = 35 'C50
                    les.calc_T.opt = 19.845 'C49
                    les.calc_T.n = 1.684 'C51

                    lat = New LatenzaFactory(True)
                    lat.calc_T.y = 0.66 'C60
                    lat.calc_T.min = 0 'C61
                    lat.calc_T.max = 30.88 'C63
                    lat.calc_T.opt = 22.32 'C62
                    lat.calc_T.n = 0.528 'C64

                    parIndicatori = New RaccaModello.ParametriIndicatori(1.5, {0.4, 0.5})

                    mNomeModello = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Racca_botriteDelPomodoro

                Case enum_ModelliPrevisionali.Racca_PeroBiet

                    mFunLAI = New RaccaFunzioneLAI_Std(Veg_Cod, 213, 0)

                    condMeteo.CalcoloTempPeriodo_BagnNormale = True

                    pInf = New ProbInfFactory_Std
                    pInf.calc_gg = 213 'C25
                    pInf.calc_T.min = 3.2492 'C33
                    pInf.calc_T.opt = 8 'C34
                    pInf.calc_T.max = 36 'C35
                    pInf.calc_T.y = 1 'C32
                    pInf.calc_T.n = 12.165 'C36
                    pInf.calc_BN.b = 0.4524 'C37
                    pInf.calc_BN.c = 8.4223 'C38

                    les = New LesioniFactory_Std
                    les.calc_T.y = 1 'C47
                    les.calc_T.min = 0 'C48
                    les.calc_T.max = 30 'C50
                    les.calc_T.opt = 10 'C49
                    les.calc_T.n = 21.3466 'C51

                    lat = New LatenzaFactory(True)
                    lat.calc_T.y = 0.1677 'C60
                    lat.calc_T.min = 0 'C61
                    lat.calc_T.max = 25 'C63
                    lat.calc_T.opt = 14.4297 'C62
                    lat.calc_T.n = 3.0865 'C64

                    parIndicatori = New RaccaModello.ParametriIndicatori(0.5, {0.05, 0.15})

                    mNomeModello = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Racca_peronosporaDellaBietola

                Case enum_ModelliPrevisionali.Racca_OidioBiet

                    mFunLAI = New RaccaFunzioneLAI_Std(Veg_Cod, 60, 0)

                    pInf = New ProbInfFactory_BietolaOidio
                    pInf.calc_gg = 60 'C25
                    pInf.calc_T.min = 0 'C33
                    pInf.calc_T.opt = 21.117 'C34
                    pInf.calc_T.max = 40 'C35
                    pInf.calc_T.y = 1 'C32
                    pInf.calc_T.n = 3.6377 'C36
                    pInf.calc_BN.b = 0.0139 'C37
                    pInf.calc_BN.c = 0.2098 'C38

                    les = New LesioniFactory_Std
                    les.calc_T.y = 1 'C47
                    les.calc_T.min = 0 'C48
                    les.calc_T.max = 40 'C50
                    les.calc_T.opt = 22.9944 'C49
                    les.calc_T.n = 3.9477 'C51

                    lat = New LatenzaFactory(True)
                    lat.calc_T.y = 0.25 'C60
                    lat.calc_T.min = 0 'C61
                    lat.calc_T.max = 40 'C63
                    lat.calc_T.opt = 24.8568 'C62
                    lat.calc_T.n = 2.8458 'C64

                    parIndicatori = New RaccaModello.ParametriIndicatori(1.5, {0.3, 0.5})

                    mNomeModello = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Racca_oidioDellaBietola

                Case enum_ModelliPrevisionali.Racca_CercoBiet

                    mFunLAI = New RaccaFunzioneLAI_Std(Veg_Cod, 60, 0)

                    pInf = New ProbInfFactory_BietolaCercospora
                    pInf.calc_gg = 60 'C25
                    pInf.calc_T.min = 1.822 'C33
                    pInf.calc_T.opt = 14.696 'C34
                    pInf.calc_T.max = 35 'C35
                    pInf.calc_T.y = 1 'C32
                    pInf.calc_T.n = 1.5325 'C36
                    pInf.calc_BN.b = 1.3505 'C37
                    pInf.calc_BN.c = 23.7469 'C38

                    les = New LesioniFactory_BietolaCercospora
                    'la crescita delle lesioni non viene considerata in quanto troppo piccole
                    les.calc_T.y = 1 'C47
                    les.calc_T.min = 0 'C48
                    les.calc_T.max = 0 'C50
                    les.calc_T.opt = 0 'C49
                    les.calc_T.n = 0 'C51

                    lat = New LatenzaFactory(True)
                    lat.calc_T.y = 0.1428 'C60
                    lat.calc_T.min = 5 'C61
                    lat.calc_T.max = 40 'C63
                    lat.calc_T.opt = 30.0829 'C62
                    lat.calc_T.n = 0.3878 'C64

                    parIndicatori = New RaccaModello.ParametriIndicatori(1.5, {0.25, 0.5})

                    mNomeModello = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Racca_cercosporaDellaBietola

                Case enum_ModelliPrevisionali.Racca_PeroPat

                    mFunLAI = New RaccaFunzioneLAI_Std(Veg_Cod, 91, 0)

                    condMeteo.CalcoloTempPeriodo_BagnNormale = True

                    pInf = New ProbInfFactory_Std
                    pInf.calc_gg = 91 'C25
                    pInf.calc_T.min = 10 'C33
                    pInf.calc_T.opt = 17.563 'C34
                    pInf.calc_T.max = 30 'C35
                    pInf.calc_T.y = 1 'C32
                    pInf.calc_T.n = 1.64 'C36
                    pInf.calc_BN.b = 0.202 'C37
                    pInf.calc_BN.c = 13.48 'C38

                    les = New LesioniFactory_Std
                    les.calc_T.y = 1 'C47
                    les.calc_T.min = 0 'C48
                    les.calc_T.max = 26 'C50
                    les.calc_T.opt = 20.748 'C49
                    les.calc_T.n = 0.462 'C51

                    lat = New LatenzaFactory(True)
                    lat.calc_T.y = 0.242 'C60
                    lat.calc_T.min = 10 'C61
                    lat.calc_T.max = 30 'C63
                    lat.calc_T.opt = 21.457 'C62
                    lat.calc_T.n = 0.488 'C64

                    parIndicatori = New RaccaModello.ParametriIndicatori(0.5, {0.05, 0.15})

                    mNomeModello = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Racca_peronosporaDellaPatata

                Case enum_ModelliPrevisionali.Racca_AlterPat

                    mFunLAI = New RaccaFunzioneLAI_Std(Veg_Cod, 91, 0)

                    condMeteo.CalcoloTempPeriodo_BagnNormale = True

                    pInf = New ProbInfFactory_Std
                    pInf.calc_gg = 60 'C25
                    pInf.calc_T.min = 10 'C33
                    pInf.calc_T.opt = 23.93 'C34
                    pInf.calc_T.max = 35 'C35
                    pInf.calc_T.y = 1 'C32
                    pInf.calc_T.n = 0.244 'C36
                    pInf.calc_BN.b = 0.12 'C37
                    pInf.calc_BN.c = 2.019 'C38

                    les = New LesioniFactory_PatataAlternaria
                    les.calc_T.y = 1 'C47
                    les.calc_T.min = 10 'C48
                    les.calc_T.max = 36 'C50
                    les.calc_T.opt = 28.42 'C49
                    les.calc_T.n = 0.599 'C51
                    les.calc_BN.b = 0.062
                    les.calc_BN.c = 22.786

                    lat = New LatenzaFactory(True)
                    lat.calc_T.y = 0.148 'C60
                    lat.calc_T.min = 10 'C61
                    lat.calc_T.max = 35 'C63
                    lat.calc_T.opt = 23.19 'C62
                    lat.calc_T.n = 0.391 'C64

                    parIndicatori = New RaccaModello.ParametriIndicatori(0.5, {0.02, 0.04})

                    mNomeModello = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Racca_alternariaDellaPatata

                Case enum_ModelliPrevisionali.Racca_ScleroSoia

                    Dim calcLAI_gg As Integer = 120
                    Dim calcLAI_gg_varieta As Integer = 125
                    'C13 Varietà (precoce/media/tardiva)
                    'precoce -> 108
                    'media -> 125
                    'tardiva -> 146

                    If objParams("Varieta") IsNot Nothing Then

                        Select Case objParams("Varieta").ToString.ToLower()
                            Case "precoce"
                                calcLAI_gg_varieta = 108
                            Case "media"
                                calcLAI_gg_varieta = 125
                            Case "tardiva"
                                calcLAI_gg_varieta = 146
                        End Select

                    End If

                    mFunLAI = New RaccaFunzioneLAI_SoiaSclerotinia(Veg_Cod, calcLAI_gg, calcLAI_gg_varieta)

                    condMeteo.CalcoloTempPeriodo_BagnNormale = True

                    pInf = New ProbInfFactory_Std
                    pInf.calc_gg = 120 'C26
                    pInf.calc_T.min = 7.1847 'C34
                    pInf.calc_T.opt = 25.0232 'C35
                    pInf.calc_T.max = 29.6635 'C36
                    pInf.calc_T.y = 1 'C33
                    pInf.calc_T.n = 0.9211 'C37
                    pInf.calc_BN.b = 0.058 'C38
                    pInf.calc_BN.c = 2.6519 'C39

                    les = New LesioniFactory_Std
                    les.calc_T.y = 1 'C48
                    les.calc_T.min = 2.843 'C49
                    les.calc_T.max = 35.915 'C51
                    les.calc_T.opt = 21.2169 'C50
                    les.calc_T.n = 2.037 'C52

                    lat = New LatenzaFactory(True)
                    lat.calc_T.y = 0.1 'C61
                    lat.calc_T.min = 5 'C62
                    lat.calc_T.max = 30.4 'C64
                    lat.calc_T.opt = 25.02 'C63
                    lat.calc_T.n = 0.2 'C65

                    parIndicatori = New RaccaModello.ParametriIndicatori(0.2, {0.01, 0.025})

                    mNomeModello = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Racca_sclerotiniaDellaSoia

                Case enum_ModelliPrevisionali.Racca_BrusoneRiso

                    'LAI_giuliano = 7 'C12 
                    'LAI_giuliano = 280 'C12 
                    Dim calcLAI_gg As Integer = 7
                    Dim calcLAI_gg_varieta As Integer = 112

                    Dim lettore As New LettoreParametri(parametriAggiuntivi)
                    Dim dtSemina_gg As Integer = lettore.IntOrDefault("DataSemina_gg", 0)
                    If dtSemina_gg = 0 Then

                        Dim dtSemina As Date = lettore.DateOrDefault("DataSemina", Date.MinValue)
                        If dtSemina > Date.MinValue Then

                            calcLAI_gg = dtSemina.DayOfYear
                        End If
                    Else

                        calcLAI_gg = dtSemina_gg
                    End If

                    If objParams("Semina") IsNot Nothing Then

                        If objParams("Semina").ToString.ToLower() = "autunnale" Then
                            calcLAI_gg = 280
                        End If
                    End If

                    'C13 Varietà 
                    'precoce -> 101
                    'media -> 112
                    'tardiva -> 125
                    Dim var As String = lettore.StringOrDefault("Varieta", "")
                    Select Case var.ToLower()
                        Case "precoce"
                            calcLAI_gg_varieta = 101
                        Case "media"
                            calcLAI_gg_varieta = 112
                        Case "tardiva"
                            calcLAI_gg_varieta = 125
                    End Select

                    mFunLAI = New RaccaFunzioneLAI_RisoBrusone(Veg_Cod, calcLAI_gg, calcLAI_gg_varieta)

                    condMeteo.CalcoloTempPeriodo_BagnNormale = True

                    pInf = New ProbInfFactory_Std
                    pInf.calc_gg = 14 'C26
                    pInf.calc_T.min = 8 'C34
                    pInf.calc_T.opt = 27.2287 'C35
                    pInf.calc_T.max = 36.0001 'C36
                    pInf.calc_T.y = 1 'C33
                    pInf.calc_T.n = 0.7788 'C37
                    pInf.calc_BN.b = 0.791 'C38
                    pInf.calc_BN.c = 5.1995 'C39

                    les = New LesioniFactory_Std
                    les.calc_T.y = 1 'C48
                    les.calc_T.min = 8 'C49
                    les.calc_T.max = 34.0457 'C51
                    les.calc_T.opt = 27.9907 'C50
                    les.calc_T.n = 0.6751 'C52

                    lat = New LatenzaFactory(True)
                    lat.calc_T.y = 0.25 'C61
                    lat.calc_T.min = 8 'C62
                    lat.calc_T.max = 34.4463 'C64
                    lat.calc_T.opt = 25.3326 'C63
                    lat.calc_T.n = 1.1447 'C65

                    parIndicatori = New RaccaModello.ParametriIndicatori(0.2, {0.01, 0.025})

                    mNomeModello = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Racca_brusoneDelRiso

                Case enum_ModelliPrevisionali.Racca_ElmintosporiosiMais

                    mFunLAI = New RaccaFunzioneLAI_Mais(Veg_Cod, 105, 90)

                    pInf = New ProbInfFactory_Std
                    pInf.calc_gg = 105 'C26
                    pInf.calc_T.min = 9.566 'C34
                    pInf.calc_T.opt = 15.1902 'C35
                    pInf.calc_T.max = 34.4267 'C36
                    pInf.calc_T.y = 1 'C33
                    pInf.calc_T.n = 1.0378 'C37
                    pInf.calc_BN.b = 0.9226 'C38
                    pInf.calc_BN.c = 2.6385 'C39

                    les = New LesioniFactory_Std
                    les.calc_T.y = 1 'C48
                    les.calc_T.min = 0 'C49
                    les.calc_T.max = 35 'C51
                    les.calc_T.opt = 22.8435 'C50
                    les.calc_T.n = 2.9721 'C52

                    lat = New LatenzaFactory(True)
                    lat.calc_T.y = 0.264 'C61
                    lat.calc_T.min = 9 'C62
                    lat.calc_T.max = 36.0874 'C64
                    lat.calc_T.opt = 15.5317 'C63
                    lat.calc_T.n = 1.2933 'C65

                    parIndicatori = New RaccaModello.ParametriIndicatori(0.5, {0.15, 0.3})

                    mNomeModello = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Racca_elmintosporiosiDelMais

                Case enum_ModelliPrevisionali.Racca_BipolarisMaidis

                    mFunLAI = New RaccaFunzioneLAI_Mais(Veg_Cod, 105, 90)

                    pInf = New ProbInfFactory_Std
                    pInf.calc_gg = 105 'C26
                    pInf.calc_T.min = 12.8923 'C34
                    pInf.calc_T.opt = 31.2441 'C35
                    pInf.calc_T.max = 37.0628 'C36
                    pInf.calc_T.y = 1 'C33
                    pInf.calc_T.n = 1.1198 'C37
                    pInf.calc_BN.b = 0.3528 'C38
                    pInf.calc_BN.c = 2.6828 'C39

                    les = New LesioniFactory_Std
                    les.calc_T.y = 1 'C48
                    les.calc_T.min = 9.9 'C49
                    les.calc_T.max = 38.3162 'C51
                    les.calc_T.opt = 29.625 'C50
                    les.calc_T.n = 1.7421 'C52

                    lat = New LatenzaFactory(True)
                    lat.calc_T.y = 0.329 'C61
                    lat.calc_T.min = 0 'C62
                    lat.calc_T.max = 37.2015 'C64
                    lat.calc_T.opt = 31.5328 'C63
                    lat.calc_T.n = 1.0202 'C65

                    parIndicatori = New RaccaModello.ParametriIndicatori(0.1, {0.0015, 0.003})

                    mNomeModello = "Bipolaris maidis"

                Case enum_ModelliPrevisionali.Racca_AntracnosiOlivo

                    mFunLAI = New RaccaFunzioneLAI_OlivoAntracnosi(Veg_Cod, 0, 0)

                    pInf = New ProbInfFactory_Std
                    pInf.calc_gg = 105 'C26
                    pInf.calc_T.min = 7.6615 'C34
                    pInf.calc_T.opt = 24.9771 'C35
                    pInf.calc_T.max = 34.4834 'C36
                    pInf.calc_T.y = 1.0137 'C33
                    pInf.calc_T.n = 0.9491 'C37
                    pInf.calc_BN.b = 0.8607 'C38
                    pInf.calc_BN.c = 2.4643 'C39

                    les = New LesioniFactory_Std
                    les.calc_T.y = 1 'C48
                    les.calc_T.min = 5 'C49
                    les.calc_T.max = 35 'C51
                    les.calc_T.opt = 17.2298 'C50
                    les.calc_T.n = 2.3341 'C52

                    lat = New LatenzaFactory(False)
                    lat.calc_T.y = 0.125 'C61
                    lat.calc_T.min = 2.1507 'C62
                    lat.calc_T.max = 38 'C64
                    lat.calc_T.opt = 29.3724 'C63
                    lat.calc_T.n = 0.4295 'C65

                    parIndicatori = New RaccaModello.ParametriIndicatori(0.5, {0.15, 0.3})

                    mNomeModello = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Racca_antracnosiDellOlivo

            End Select

            If pInf IsNot Nothing AndAlso les IsNot Nothing AndAlso lat IsNot Nothing Then
                mModello = New RaccaModelloStd(condMeteo, pInf, les, lat)
            End If

        End If

        'VEDI QUI (***) PER EVENTUALE IMPLEMENTAZIONE
        'If mModello IsNot Nothing Then
        '    mModello.LeggiParametri(Mod_Cod, Veg_Cod, Av_Cod, objParametri_server)
        'End If

        If mModello Is Nothing Then
            Throw New Exception("Modello sconosciuto")
        End If

        If parIndicatori IsNot Nothing Then
            mModello.Indicatori = parIndicatori
        End If

    End Sub

    Public Shared Function EsponiParams(ByVal mod_cod As Integer, ByVal params As String) As String

        If mod_cod = enum_ModelliPrevisionali.Racca_BrusoneRiso Then

            Dim lettore As New LettoreParametri(params)

            Dim str_params(1) As String
            str_params(0) = lettore.OutputFromDate("DataSemina", "Data semina: ")
            If String.IsNullOrEmpty(str_params(0)) Then

                str_params(0) = lettore.OutputFromDOY("DataSemina_gg", "Data semina: ")
            End If

            str_params(1) = lettore.StringOrDefault("Varieta", "")
            If Not String.IsNullOrEmpty(str_params(1)) Then

                str_params(1) = "Varietà: " & str_params(1)
            End If

            Return String.Join(" ", str_params.Where(Function(s) Not String.IsNullOrEmpty(s)))
        End If

        Return ""
    End Function

    Protected Overrides Function _elaboraModello() As cRisultatoModello

        Dim risModello As New cRisultatoModello

        Try

            Dim elencoGG = mModello.Calcola(_datiMeteo, mFunLAI)

            risModello.Modello_Tabella1 = mModello.GeneraOutput(elencoGG)

            risModello.Modello_Disclaimer =
                "<div style=""border: 1px solid #ccc; border-radius: 5px; padding: 15px; background-color: rgb(247,247,247);"">" &
                "<div style=""margin-bottom: 10px;"">" &
                "<span>" &
                String.Format(My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Racca_modelloXOttenutoAnalizzandoDatiScientificiPubblicati, mNomeModello) &
                My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Racca_ilModelloAnalizzaLeCondizioniMeteorologicheFavorevoliPerUnPatogeno &
                "</span>" &
                "</div>" &
                "<div>" &
                "<span style=""font-weight: bold;"">" & Gias.Attenzione & " </span>" &
                "<span style=""font-style: italic;"">" &
                My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Racca_senzaInoculoAncheInCondizioniFavorevoliLaMalattiaNonSiSviluppa &
                "</span> " &
                "<span style=""font-style: italic;"">" &
                My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Racca_ilModelloSiIntendeComeSupportoDSS &
                "</span>" &
                "</div>" &
                "</div>"

        Catch ex As Exception

            risModello = Nothing

            'uso questa funzione per ottenere il Messaggio..:
            _errore = Gias.ErroreDuranteOperazione_ & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return risModello
    End Function

    Protected Overrides Function _elaboraIndicatore_vecchio(dataInizio As Date, dataFine As Date) As cRisultatoModelloIndicatori.Indicatore

        Dim risIndic As New cRisultatoModelloIndicatori.Indicatore("", dataInizio, dataFine)

        Dim flagOK As Boolean = False

        Try

            Dim elencoGG = mModello.Calcola(_datiMeteo, mFunLAI)

            If elencoGG IsNot Nothing AndAlso elencoGG.Count > 0 Then

                Dim last_d_gg = elencoGG.Last()

                risIndic.Fill(last_d_gg.Indicatore(), mModello.Indicatori.ScaleMax, mModello.Indicatori.BandsValues, last_d_gg.Data, dataFine)

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

            risIndic.Status = cRisultatoModelloIndicatori.Indicatore.enum_Status.Status_Error
            risIndic.StatusMsg = _errore
        End If

        Return risIndic
    End Function

    Protected Overrides Function _elaboraIndicatore(ByVal risElab As RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione) As RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione

        Dim flagOK As Boolean = False

        Try

            Dim elencoGG = mModello.Calcola(_datiMeteo, mFunLAI)

            If elencoGG IsNot Nothing AndAlso elencoGG.Count > 0 Then

                Dim last_d_gg = elencoGG.Last()

                risElab.Fill(last_d_gg.Indicatore(), mModello.Indicatori.ScaleMax, mModello.Indicatori.BandsValues)

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

End Class





'**************************************************************************************************
' (***) RIFERIMENTO PER EVENTUALE IMPLEMENTAZIONE
'**************************************************************************************************

#If False Then

    Public Sub LeggiParametri(ByVal Mod_Cod As Integer, ByVal Veg_Cod As Integer, ByVal Av_Cod As Integer, ByVal objParametri_server As AgronicaCoreParametri)
        'Dim parMod As New parametriModello(Mod_Cod, Veg_Cod, Av_Cod, objParametri_server)
        ''Dim parMod As New parametriModello(10, 0, 87, objParametri_server)
        'If parMod.IsEmpty() Then
        '    'r.Errore &= "Errore: non trovo i parametri di calcolo, uso i valori di default"
        'Else
        '    'mCalcoloPeriodi_DatiMisurati = parMod.leggi("CalcoloPeriodi_DatiMisurati", mCalcoloPeriodi_DatiMisurati)
        '    'mCalcoloTempPeriodo_BagnNormale = parMod.leggi("CalcoloTempPeriodo_BagnNormale", mCalcoloTempPeriodo_BagnNormale)
        '    'mCalcoloLatenzaComeMedia = parMod.leggi("CalcoloLatenzaComeMedia", mCalcoloLatenzaComeMedia)
        '    'mSogliaPioggiaCorrBagn = parMod.leggi("SogliaPioggiaCorrBagn", mSogliaPioggiaCorrBagn)
        '    'mSogliaURCorrBagn = parMod.leggi("SogliaURCorrBagn", mSogliaURCorrBagn)
        '    'mSogliaPioggiaPerBagn = parMod.leggi("SogliaPioggiaPerBagn", mSogliaPioggiaPerBagn)
        '    'mCalcLAI_giuliano = parMod.leggi("CalcLAI_giuliano", mCalcLAI_giuliano)
        '    'mCalcLAI_a = parMod.leggi("CalcLAI_a", mCalcLAI_a)
        '    'mCalcLAI_b = parMod.leggi("CalcLAI_b", mCalcLAI_b)
        '    'mCalcLAI_x0 = parMod.leggi("CalcLAI_x0", mCalcLAI_x0)
        '    'mCalcLAI_y0 = parMod.leggi("CalcLAI_y0", mCalcLAI_y0)
        '    'mCalcLAI_yMax = parMod.leggi("CalcLAI_yMax", mCalcLAI_yMax)
        '    'mCalcPInf_giuliano = parMod.leggi("CalcPInf_giuliano", mCalcPInf_giuliano)
        '    'mCalcPInf_T = parMod.leggi("CalcPInf_T", mCalcPInf_T)
        '    'mCalcPInf_BN = parMod.leggi("CalcPInf_BN", mCalcPInf_BN)
        '    'mCalcLesioni_T = parMod.leggi("CalcLesioni_T", mCalcLesioni_T)
        '    'mCalcLesioni_BN = parMod.leggi("CalcLesioni_BN", mCalcLesioni_BN)
        '    'mCalcLatenza_T = parMod.leggi("CalcLatenza_T", mCalcLatenza_T)
        'End If
    End Sub



    Private Class ParametriModello
        Private mDT As DataTable

        Private Function leggi(Of T)(ByVal parNome As String, ByVal defValue As T) As T

            Dim rval As Object = (From r In mDT.AsEnumerable
                                  Where r("ParametroNome") = parNome
                                  Select r("ParametroValore")).FirstOrDefault()

            If rval Is Nothing Then
                Return defValue
            End If

            Dim rvalT As T = defValue
            Try

                'Convert.ToBoolean(rval)
                'Convert.ToInt32(rval)
                'Convert.ToDecimal(rval, New Globalization.CultureInfo("en-US"))  'Globalization.CultureInfo.InvariantCulture

            Catch ex As Exception
                rvalT = defValue
            End Try

            Return rvalT

        End Function
        Public Sub New(ByVal Mod_Cod As Integer, ByVal Veg_Cod As Integer, ByVal Av_Cod As Integer, ByVal objParametri_server As AgronicaCoreParametri)
            Dim reader As New AgronicaCoreModelliPrevisionaliDAL.ParametriModelli_R
            mDT = reader.Leggi(Av_Cod, Veg_Cod, Mod_Cod, "", "", objParametri_server)
        End Sub
        Public Function IsEmpty() As Boolean
            Return mDT.Rows.Count = 0
        End Function
        Public Function leggi(ByVal parametro As String, ByVal defValue As Boolean) As Boolean
            Return leggi(Of Boolean)(parametro, defValue)
        End Function
        Public Function leggi(ByVal parametro As String, ByVal defValue As Integer) As Integer
            Return leggi(Of Integer)(parametro, defValue)
        End Function
        Public Function leggi(ByVal parametro As String, ByVal defValue As Decimal) As Decimal
            Return leggi(Of Decimal)(parametro, defValue)
        End Function
        Public Function leggi(ByVal parametro As String, ByVal valDefault As RaccaUtility.FunzioneBetaHau) As RaccaUtility.FunzioneBetaHau
            'da implementare
            Return valDefault
        End Function
        Public Function leggi(ByVal parametro As String, ByVal valDefault As RaccaUtility.FunzioneChapman) As RaccaUtility.FunzioneChapman
            'da implementare
            Return valDefault
        End Function
    End Class
#End If