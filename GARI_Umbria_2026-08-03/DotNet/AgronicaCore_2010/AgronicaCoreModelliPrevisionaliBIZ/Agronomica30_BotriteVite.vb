Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelliPrevisionaliBIZ
Imports AgronicaCoreVarieBIZ

Public Class Agronomica30_BotriteVite
    Inherits AbstractModello

    Private Class clDatoGG
        Public data As DateTime
        Public Tmin As Decimal
        Public Tmax As Decimal
        Public Tmed As Decimal
        Public UR As Decimal
        Public Pioggia As Decimal
        Public LW As Decimal
        Public NHHsum As Decimal
        Public BBCH_rip As Decimal
        Public BBCH_veg As Decimal
        Public CISO As Decimal
        Public SEV1 As Decimal
        Public SEV2 As Decimal
        Public SEV3 As Decimal
        Public Indicatore As Decimal
        Public Sub New(d_h As MeteoDSSItem)
            data = d_h.DataOra
            Tmin = d_h.Temp
            Tmax = d_h.Temp
            Tmed = d_h.Temp
            UR = d_h.UmRel
            Pioggia = d_h.Prec
            LW = d_h.BagnEffettiva(85)
        End Sub
        Public Function Finestra() As Integer
            If 53 <= BBCH_rip AndAlso BBCH_rip <= 73 Then
                Return 1
            ElseIf 79 <= BBCH_rip AndAlso BBCH_rip <= 89 Then
                Return 2
            End If
            Return 0
        End Function
    End Class

    Private Class modello
        Private Function MYGROW(ByVal teq As Decimal, ByVal LW As Decimal) As Decimal
            'tasso crescita micelio
            'We therefore assumed that residues contain sufficient humidity for mycelial growth
            'in the hours of the day when rain, R0.2 mm or wetness duration, WD30 min or RH (daily
            'average relative humidity)90% (i.e., moist hours); therefore, Mf = number of moist hours/24
            Dim Mf As Decimal = LW / 24D
            '=(3.78*T_eq^0.9*(1-T_eq))^0.475*Mf
            Return Math.Pow((3.78D * Math.Pow(teq, 0.9D) * (1D - teq)), 0.475D) * Mf
        End Function
        Private Function SPOR(ByVal teq As Decimal, ByVal UR As Decimal) As Decimal
            'calcolo tasso produzione spore
            If teq = 0 Then
                Return 0
            End If
            Dim res As Decimal = Math.Pow((3.7D * Math.Pow(teq, 0.9D) * (1D - teq)), 10.49D) * (-3.595D + 0.097D * UR - 0.0005 * Math.Pow(UR, 2))
            Return Math.Max(0, Math.Min(res, 1)) 'potrebbe avere qualche millesimo in piu
            '>>> GABRIELE <<< se UR < ~50 il valore restituito è negativo... da verificare se è possibile oppure Math.Max(res, 0)
        End Function
        Public Function INF1(ByVal teq As Decimal, ByVal LW As Decimal, ByVal SUS As Decimal) As Decimal
            'In the first infection window (stages 53 to 73), the model calculates an infection rate on
            'inflorescences and young clusters (INF1) as:
            Return Math.Pow(3.56D * Math.Pow(teq, 0.99D) * (1D - teq), 0.71D) / (1D + Math.Exp(1.85D - 0.19D * LW)) * SUS
        End Function
        Private Function SUS1(ByVal GS As Decimal) As Decimal
            'SUS1 = relative susceptibility of the inflorescences and young clusters
            'GS = growth stage of the plant based on the stages of the scale of Lorenz et al.
            Dim GS_100 As Decimal = GS / 100D
            Dim ris As Decimal = ((((-379.09D * GS_100) + 671.25) * GS_100) - 390.33D) * GS_100 + 75.209D
            Return Math.Max(ris, 0)
        End Function
        Private Function INF2(ByVal teq As Decimal, ByVal LW As Decimal, ByVal SUS As Decimal) As Decimal
            'In the second infection window (stages 79 to 89), the model calculates two infection rates on
            'ripening berries: one for conidial infection (INF2) and another for berry-to-berry infection
            'Infection rate for conidial infection is calculated as follows
            Return Math.Pow((6.416D * Math.Pow(teq, 1.292D) * (1D - teq)), 0.469D) * Math.Exp(-2.3D * Math.Exp(-0.048D * LW)) * SUS
        End Function
        Private Function SUS2(ByVal GS As Decimal) As Decimal
            'SUS2 = relative susceptibility of the ripening berries to B. cinerea
            'conidia infection, and GS = growth stage of the plant based on the scale of Lorenz
            Dim ris As Decimal = 5.0E-17 * Math.Exp(0.4219D * GS)
            Return Math.Max(ris, 0)
        End Function
        Private Function INF3(ByVal teq As Decimal, ByVal UR As Decimal, ByVal SUS As Decimal) As Decimal
            'Infection rate for berry-to-berry infection during the second infection window is calculated as follows
            Return Math.Pow((7.75D * Math.Pow(teq, 2.14D) * (1D - teq)), 0.469D) / (1D + Math.Exp(35.36D - 40.26D * UR / 100D)) * SUS
        End Function
        Private Function SUS3(ByVal GS As Decimal) As Decimal
            'SUS3 = relative susceptibility of the ripening berries to B. cinerea mycelium infection,
            'GS = growth stage of the plant based on the scale of Lorenz
            Dim ris As Decimal = 0.0546D * GS - 3.87D
            Return Math.Min(Math.Max(ris, 0), 1)
        End Function

        Private _CISO_7gg As Queue(Of Decimal)
        Private _sumSEV1 As Decimal
        Private _sumSEV2 As Decimal
        Private _sumSEV3 As Decimal

        Private _indicatore As Decimal
        Private _last_indicatore As Decimal

        Public Sub New()
            _CISO_7gg = New Queue(Of Decimal)
            _sumSEV1 = 0
            _sumSEV2 = 0
            _sumSEV3 = 0
            _indicatore = -1
        End Sub
        Public Sub calcola(ByRef dgg As clDatoGG)

            'stabilisco finestra in base allo stadio BBCH per il calcolo di teq
            '0=fuori finestraBBCH  1   2
            Dim finestra As Integer = dgg.Finestra()
            If finestra <> 0 Then

                Dim tmin As Decimal = 0
                Dim tmax As Decimal = If(finestra = 1, 35, 40)
                'Teq = temperature equivalent in the form
                'Teq = (T — Tmin) / (Tmax — Tmin);
                'with T = daily average temperature (°C);
                'Tmin = minimum temperature for mycelial growth or sporulation (0°C);
                'Tmax = maximum temperature for mycelial growth (40°C) or sporulation (35°C);

                Dim teq As Decimal = (dgg.Tmed - tmin) / (tmax - tmin)
                Dim micelio As Decimal = MYGROW(teq, dgg.LW)
                Dim sporulazione As Decimal = SPOR(teq, dgg.UR)
                _CISO_7gg.Enqueue(micelio * sporulazione)
                If _CISO_7gg.Count() > 7 Then
                    _CISO_7gg.Dequeue()
                End If
                'If pStack.Count <> 7 Then
                '    CISO = 0
                'End If
                Dim CISO_def As Decimal = _CISO_7gg.Sum() / 7D

                dgg.CISO = sporulazione

                If finestra = 1 Then
                    _sumSEV1 += CISO_def * INF1(teq, dgg.LW, SUS1(dgg.BBCH_rip))
                    dgg.SEV1 = _sumSEV1
                Else
                    _sumSEV2 += CISO_def * INF2(teq, dgg.LW, SUS2(dgg.BBCH_rip))
                    _sumSEV3 += micelio * INF3(teq, dgg.UR, SUS3(dgg.BBCH_rip))
                    dgg.SEV2 = _sumSEV2
                    dgg.SEV3 = _sumSEV3
                End If

            End If

            Dim a As Decimal = 0
            'Dim b As Decimal = 1.5
            '*** 12 Giugno 2019 *** Modifica suggerita da Candolo (mail del 11 Giugno 2019)
            Dim b As Decimal = 1

            If dgg.BBCH_rip < 53 OrElse 93 <= dgg.BBCH_rip Then
                _indicatore = -1
            Else
                If 53 <= dgg.BBCH_rip AndAlso dgg.BBCH_rip <= 73 Then
                    _last_indicatore = a + b * (Math.Log(dgg.SEV1 + 1D))
                    _indicatore = _last_indicatore
                ElseIf 79 <= dgg.BBCH_rip AndAlso dgg.BBCH_rip <= 89 Then
                    _indicatore = _last_indicatore + a + b * (Math.Log(dgg.SEV2 + dgg.SEV3 + 1D))
                End If
            End If

            dgg.Indicatore = _indicatore

        End Sub
    End Class

    'Private MustInherit Class varieta
    '    Private _Tc_opt As Decimal
    '    Private _Tc_min As Decimal
    '    Private _Tc_max As Decimal
    '    Protected _NHHsum As Decimal
    '    Private _BBCH_rip As Decimal
    '    Private _BBCH_veg As Decimal
    '    Protected Sub New(ByVal tc_opt As Decimal, ByVal tc_min As Decimal, ByVal tc_max As Decimal)
    '        _Tc_opt = tc_opt
    '        _Tc_min = tc_min
    '        _Tc_max = tc_max
    '        _NHHsum = 0
    '        _BBCH_rip = 0
    '        _BBCH_veg = 0
    '    End Sub
    '    Protected MustOverride Function calc_BBCH_rip() As Decimal
    '    Protected MustOverride Function calc_BBCH_veg() As Decimal
    '    Public Sub NHH_BBCH(ByVal temp As Decimal)
    '        Dim NHH As Decimal = 0
    '        If _Tc_min <= temp And temp <= _Tc_max Then
    '            Dim alpha As Decimal = Math.Log(2 / Math.Log((_Tc_max - _Tc_min) / (_Tc_opt - _Tc_min)))
    '            NHH = (2D * Math.Pow((temp - _Tc_min), alpha) * Math.Pow((_Tc_opt - _Tc_min), alpha) - Math.Pow((temp - _Tc_min), (2D * alpha))) / Math.Pow((_Tc_opt - _Tc_min), (2D * alpha))
    '            NHH = Math.Max(NHH, 0)
    '            _NHHsum += NHH
    '        End If
    '        _BBCH_rip = 0
    '        _BBCH_veg = 0
    '        If _NHHsum > 0 Then
    '            _BBCH_rip = calc_BBCH_rip()
    '            _BBCH_rip = Math.Round(_BBCH_rip)
    '            '_BBCH_rip = If(_BBCH_rip < 50 Or _BBCH_rip > 93, 0, _BBCH_rip)
    '            _BBCH_veg = calc_BBCH_veg()
    '            _BBCH_veg = If(_BBCH_veg > 0, _BBCH_veg, 0)
    '        End If
    '    End Sub
    '    Public ReadOnly Property NHHsum As Decimal
    '        Get
    '            Return _NHHsum
    '        End Get
    '    End Property
    '    Public ReadOnly Property BBCH_rip As Decimal
    '        Get
    '            Return _BBCH_rip
    '        End Get
    '    End Property
    '    Public ReadOnly Property BBCH_veg As Decimal
    '        Get
    '            Return _BBCH_veg
    '        End Get
    '    End Property
    'End Class
    'Private Class Chardonnay
    '    Inherits varieta
    '    Public Sub New()
    '        MyBase.New(26, 12, 33)
    '    End Sub
    '    Protected Overrides Function calc_BBCH_rip() As Decimal
    '        'Species tcmin tcopt tcmax NHH-BBCH equation
    '        'Vitis vinifera cv. Chardonnay (reproductive scale)RRRRRRRRRRRRRRRRRRRRR
    '        '12 25 33 BBCH = 8 E(-12) NHHsum4 – 2 E(-8) NHHsum3 – E(-5) NHHsum2 +0.0432 NHHsum + 43.736
    '        Return ((((0.000000000007D * _NHHsum) - 0.00000002D) * _NHHsum - 0.000001D) * _NHHsum + 0.0432D) * _NHHsum + 43.736D
    '    End Function
    '    Protected Overrides Function calc_BBCH_veg() As Decimal
    '        'Species tcmin tcopt tcmax NHH-BBCH equation
    '        'Vitis vinifera cv. Chardonnay (vegetative scale)VVVVVVVVVVVVVVVVVVVVVVVVVVVV
    '        '12 25 33 BBCH = 12.133 Ln(NHHsum) - 48.537
    '        Return 12.133D * Math.Log(_NHHsum) - 48.537D
    '    End Function
    'End Class
    'Private Class CabernetSauvignon
    '    Inherits varieta
    '    Public Sub New()
    '        MyBase.New(26, 12, 33)
    '    End Sub
    '    Protected Overrides Function calc_BBCH_rip() As Decimal
    '        'Species tcmin tcopt tcmax NHH-BBCH equation
    '        'Vitis vinifera cv. Cabernet S. (reproductive scale)RRRRRRRRRRRRRRRRRRRR
    '        '12 25 33 BBCH = 7 E(-12) NHHsum4 – 2 E(-8) NHHsum3 – 2 E(-6) NHHsum2 +0.0488 NHHsum + 42.642
    '        Return ((((0.000000000007D * _NHHsum) - 0.00000002D) * _NHHsum - 0.000002D) * _NHHsum + 0.0488D) * _NHHsum + 42.642D
    '    End Function
    '    Protected Overrides Function calc_BBCH_veg() As Decimal
    '        'Vitis vinifera cv. Cabernet S. (vegetative scale)VVVVVVVVVVVVVVVVVVVVVVVVVVV
    '        '12 25 33 BBCH = 12.667 Ln(NHHsum) - 53.051
    '        Return 12.667D * Math.Log(_NHHsum) - 53.061D
    '    End Function
    'End Class

    Private _datiModello As List(Of clDatoGG)

    Public Sub New(ByVal datiMeteo As MeteoReadOnlyList)
        MyBase.New(datiMeteo)

        _datiModello = New List(Of clDatoGG)
    End Sub

    Protected Overrides Function _elaboraModello() As cRisultatoModello

        If Not CalcolaBotrite() Then
            Return Nothing
        End If

        Return New cRisultatoModello With {
            .Modello_Tabella1 = outputModello(False),
            .Modello_Tabella2 = outputModello(True)
        }
    End Function

    Protected Overrides Function _elaboraIndicatore_vecchio(dataInizio As Date, dataFine As Date) As cRisultatoModelloIndicatori.Indicatore

        Dim risIndic As New cRisultatoModelloIndicatori.Indicatore("Botrite", dataInizio, dataFine)

        If CalcolaBotrite() Then

            'se indicatore < 0 sono in un periodo in cui il modello non è disponibile....
            Dim _value As Decimal = _datiModello.Last().Indicatore
            If _value < 0 Then
                _value = 0
                'risIndic.AuxMsg = "Stadio di sviluppo senza rischio infettivo"
            End If

            'risIndic.Fill(_value, 2, {0.5, 1.14}, mDatiModello.Last().data, DataFine)
            '*** 12 Giugno 2019 *** Modifica suggerita da Candolo (mail del 11 Giugno 2019)
            risIndic.Fill(_value, 2.5, {1.38, 1.72}, _datiModello.Last().data, dataFine)
        Else

            risIndic.Status = cRisultatoModelloIndicatori.Indicatore.enum_Status.Status_Error
            risIndic.StatusMsg = _errore
        End If

        Return risIndic
    End Function

    Protected Overrides Function _elaboraIndicatore(ByVal risElab As RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione) As RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione

        If CalcolaBotrite() Then

            'se indicatore < 0 sono in un periodo in cui il modello non è disponibile....
            Dim _value As Decimal = _datiModello.Last().Indicatore
            If _value < 0 Then
                _value = 0
                'risIndic.AuxMsg = "Stadio di sviluppo senza rischio infettivo"
            End If

            'risIndic.Fill(_value, 2, {0.5, 1.14}, mDatiModello.Last().data, DataFine)
            '*** 12 Giugno 2019 *** Modifica suggerita da Candolo (mail del 11 Giugno 2019)
            risElab.Fill(_value, 2.5, {1.38, 1.72})
        Else

            risElab.Errore(_errore)
        End If

        Return risElab
    End Function

    'Public Function Botrite(ByVal datiMeteo As List(Of AgronicaCoreMeteoBiz.InterfacciaMeteoNT.MeteoDSS)) As rispostaStandard(Of cRisultatoModello)

    '    Dim r As New rispostaStandard(Of cRisultatoModello)
    '    r.RispostaStringa = New cRisultatoModello()
    '    r.RispostaOK = False
    '    r.Errore = ""

    '    If CalcolaBotrite(datiMeteo) Then

    '        r.RispostaOK = True
    '        r.RispostaStringa.Modello_Tabella1 = outputModello(False)
    '        r.RispostaStringa.Modello_Tabella2 = outputModello(True)

    '    Else

    '        r.Errore = mErrore

    '    End If

    '    Return r

    'End Function

    'Public Function Botrite_Indicatore(ByVal datiMeteo As List(Of AgronicaCoreMeteoBiz.InterfacciaMeteoNT.MeteoDSS), ByVal DataInizio As DateTime, ByVal DataFine As DateTime) As rispostaStandard(Of cRisultatoModelloIndicatori.Indicatore)

    '    Dim r As New rispostaStandard(Of cRisultatoModelloIndicatori.Indicatore)
    '    r.RispostaStringa = New cRisultatoModelloIndicatori.Indicatore("Botrite", DataInizio, DataFine)
    '    r.RispostaOK = False
    '    r.Errore = ""

    '    If CalcolaBotrite(datiMeteo) Then

    '        'se indicatore < 0 sono in un periodo in cui il modello non è disponibile....
    '        r.RispostaOK = True
    '        Dim _value As Decimal = mDatiModello.Last().Indicatore
    '        If _value < 0 Then
    '            _value = 0
    '            'r.RispostaStringa.AuxMsg = "Stadio di sviluppo senza rischio infettivo"
    '        End If

    '        'r.RispostaStringa.Fill(_value, 2, {0.5, 1.14}, mDatiModello.Last().data, DataFine)
    '        '*** 12 Giugno 2019 *** Modifica suggerita da Candolo (mail del 11 Giugno 2019)
    '        r.RispostaStringa.Fill(_value, 2.5, {1.38, 1.72}, mDatiModello.Last().data, DataFine)

    '    Else

    '        r.Errore = mErrore
    '        r.RispostaStringa.Status = cRisultatoModelloIndicatori.Indicatore.enum_Status.Status_Error
    '        r.RispostaStringa.StatusMsg = mErrore

    '    End If

    '    Return r

    'End Function

    Private Function CalcolaBotrite() As Boolean

        Dim result As Boolean = True

        Try

            Dim mdl As New modello()



            Dim BBCH As New Agronomica30_BBCH(12, 33, 26, Agronomica30_BBCH.VarietaVite.CabernetSauvignon)

            Dim hh_cnt As Integer
            Dim d_gg As clDatoGG = Nothing

            For Each mdo In _datiMeteo

                BBCH.Calc(mdo.Temp)

                If d_gg Is Nothing OrElse mdo.DataOra.DayOfYear <> d_gg.data.DayOfYear Then

                    If d_gg IsNot Nothing Then
                        d_gg.Tmed = d_gg.Tmed / hh_cnt
                        d_gg.UR = d_gg.UR / hh_cnt
                        mdl.calcola(d_gg)
                        _datiModello.Add(d_gg)
                    End If

                    d_gg = New clDatoGG(mdo)
                    d_gg.NHHsum = BBCH.NHHsum
                    d_gg.BBCH_rip = BBCH.BBCH_rip
                    d_gg.BBCH_veg = BBCH.BBCH_veg
                    hh_cnt = 1

                Else
                    d_gg.Tmin = Math.Min(d_gg.Tmin, mdo.Temp)
                    d_gg.Tmax = Math.Max(d_gg.Tmax, mdo.Temp)
                    d_gg.Tmed += mdo.Temp
                    d_gg.UR += mdo.UmRel
                    d_gg.Pioggia += mdo.Prec
                    d_gg.LW += mdo.BagnEffettiva(85)
                    d_gg.NHHsum = Math.Max(d_gg.NHHsum, BBCH.NHHsum)
                    d_gg.BBCH_rip = Math.Max(d_gg.BBCH_rip, BBCH.BBCH_rip)
                    d_gg.BBCH_veg = Math.Max(d_gg.BBCH_veg, BBCH.BBCH_veg)
                    hh_cnt += 1
                End If

            Next

            If d_gg IsNot Nothing Then
                d_gg.Tmed = d_gg.Tmed / hh_cnt
                d_gg.UR = d_gg.UR / hh_cnt
                mdl.calcola(d_gg)
                _datiModello.Add(d_gg)

                If d_gg.BBCH_rip < 53 Then

                    result = False
                    _errore = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.NonRaggiuntaLaFaseFenologicaNecessariaAlloSviluppoDellaAvversita
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

    Private Function outputModello(ByVal bCompleto As Boolean) As String

        Dim output As New OutputModello
        output.aggiungiColonna("Data", GetType(Date), Gias.Data, "dd/MM/yyyy")
        output.aggiungiColonna("tmed", GetType(Decimal), Gias.TemperaturaMedia & " (°C)", "0.00")
        output.aggiungiColonna("UR", GetType(Decimal), Gias.UmiditaRelativaMedia & "(%)", "0.00")
        output.aggiungiColonna("Pioggia", GetType(Decimal), Gias.Pioggia & " (mm)", "0.00")
        output.aggiungiColonna("LW", GetType(Integer), Gias.BagnaturaFogliare & " (ore si)", "0") 'i18n

        Dim indSEV1 As OutputIndicator = Nothing
        Dim indSEV2 As OutputIndicator = Nothing
        Dim indSEV3 As OutputIndicator = Nothing
        Dim indSEVtot As OutputIndicator = Nothing
        Dim indSEV As OutputIndicator = Nothing

        If bCompleto Then

            output.aggiungiColonna("CISO", GetType(Decimal), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_BotriteVite_abbondanzaRelativaDeiConidi & " (%)", "0\\%", True)
            output.aggiungiColonna("SEV1", GetType(Decimal), String.Format(My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_BotriteVite_severitàInfezioneFx, 1) & " (%)", "0.00")
            indSEV1 = output.aggiungiIndicatore("SEV1_IND", {0.94, 1.81, 2.46, System.Decimal.MaxValue}, OutputIndicator.enum_IndicatorType.Numbers).setFieldVal("SEV1")
            output.aggiungiColonna("SEV2", GetType(Decimal), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_BotriteVite_daConidi, "0.00")._gruppoColonne = String.Format(My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_BotriteVite_severitàInfezioneFx, 2) & " (%)"
            indSEV2 = output.aggiungiIndicatore("SEV2_IND", {0.15, 0.2, 0.28, System.Decimal.MaxValue}, OutputIndicator.enum_IndicatorType.Numbers).setFieldVal("SEV2")
            output.aggiungiColonna("SEV3", GetType(Decimal), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_BotriteVite_daMicelioAcino, "0.00")._gruppoColonne = String.Format(My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_BotriteVite_severitàInfezioneFx, 2) & " (%)"
            indSEV3 = output.aggiungiIndicatore("SEV3_IND", {0.34, 0.37, 0.76, System.Decimal.MaxValue}, OutputIndicator.enum_IndicatorType.Numbers).setFieldVal("SEV3")
            output.aggiungiColonna("SEVtot", GetType(Decimal), Gias.Somma, "0.00")._gruppoColonne = String.Format(My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_BotriteVite_severitàInfezioneFx, 2) & " (%)"
            indSEVtot = output.aggiungiIndicatore("SEVtot_IND", {0.49, 0.57, 1.04, System.Decimal.MaxValue}, OutputIndicator.enum_IndicatorType.Numbers).setFieldVal("SEVtot")

        Else

            output.aggiungiColonna("SEV", GetType(Decimal), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_BotriteVite_rischioInfettivo, "0.00")
            'indSEV = output.aggiungiIndicatore("SEV_IND", {0.5, 1.14, System.Decimal.MaxValue}).setFieldVal("SEV")
            '*** 12 Giugno 2019 *** Modifica suggerita da Candolo (mail del 11 Giugno 2019)
            indSEV = output.aggiungiIndicatore("SEV_IND", {1.38, 1.72, System.Decimal.MaxValue}, OutputIndicator.enum_IndicatorType.Colors).setFieldVal("SEV")
            output.aggiungiColonna("Finestra", GetType(Integer), "Finestra", "0")._hidden = True

        End If

        output.aggiungiColonna("BBCH", GetType(Decimal), "BBCH", "0")._hidden = True
        output.aggiungiColonna("BBCH_marker", GetType(Integer), "", "0")._hidden = True

        Dim fin As Integer
        Dim sev As Decimal
        Dim bbchToMark As List(Of Integer) = {13, 55, 62, 77, 83, Integer.MaxValue}.ToList()

        For Each d In _datiModello

            output.AddField(d.data)
            output.AddField(d.Tmed)
            output.AddField(d.UR)
            output.AddField(d.Pioggia)
            output.AddField(d.LW)

            fin = d.Finestra()

            If bCompleto Then

                If fin <> 0 Then

                    output.AddField(d.CISO * 100)
                    If fin = 1 Then
                        output.AddField(d.SEV1)
                        output.AddField(indSEV1.colorForVal(d.SEV1))
                        output.AddField(DBNull.Value)
                        output.AddField(DBNull.Value)
                        output.AddField(DBNull.Value)
                        output.AddField(DBNull.Value)
                        output.AddField(DBNull.Value)
                        output.AddField(DBNull.Value)
                    ElseIf fin = 2 Then
                        output.AddField(DBNull.Value)
                        output.AddField(DBNull.Value)
                        output.AddField(d.SEV2)
                        output.AddField(indSEV2.colorForVal(d.SEV2))
                        output.AddField(d.SEV3)
                        output.AddField(indSEV3.colorForVal(d.SEV3))
                        sev = d.SEV2 + d.SEV3
                        output.AddField(sev)
                        output.AddField(indSEVtot.colorForVal(sev))
                    End If

                Else
                    output.AddField(DBNull.Value)
                    output.AddField(DBNull.Value)
                    output.AddField(DBNull.Value)
                    output.AddField(DBNull.Value)
                    output.AddField(DBNull.Value)
                    output.AddField(DBNull.Value)
                    output.AddField(DBNull.Value)
                    output.AddField(DBNull.Value)
                    output.AddField(DBNull.Value)
                End If

            Else

                If d.Indicatore < 0 Then
                    output.AddField(DBNull.Value)
                    output.AddField(DBNull.Value)
                Else
                    output.AddField(d.Indicatore)
                    output.AddField(indSEV.colorForVal(d.Indicatore))
                End If

                output.AddField(fin)

            End If

            output.AddField(d.BBCH_rip)
            While d.BBCH_rip > bbchToMark(0)
                bbchToMark.RemoveAt(0)
            End While
            If d.BBCH_rip = bbchToMark(0) Then
                output.AddField(1)
                bbchToMark.RemoveAt(0)
            Else
                output.AddField(DBNull.Value)
            End If

            output.Commit()

        Next

        Dim indicators As New List(Of OutputIndicator)
        If indSEV1 IsNot Nothing Then indicators.Add(indSEV1)
        If indSEV2 IsNot Nothing Then indicators.Add(indSEV2)
        If indSEV3 IsNot Nothing Then indicators.Add(indSEV3)
        If indSEVtot IsNot Nothing Then indicators.Add(indSEVtot)
        If indSEV IsNot Nothing Then indicators.Add(indSEV)

        Return output.Output(indicators)
    End Function
End Class
