

Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreModelliPrevisionaliBIZ
Imports System.Globalization

Public MustInherit Class AbsUniCatt_Mais_Modello

    Protected MustInherit Class AbsDatiModello

        Public DataOra As DateTime
        Public Temp As Decimal
        Public UmRel As Decimal
        Public Prec As Decimal
        Public aw As Decimal

        Public Sub New(dm As MeteoDSSItem, _aw As Decimal)
            DataOra = dm.DataOra
            Temp = dm.Temp
            UmRel = dm.UmRel
            Prec = dm.Prec
            aw = _aw
        End Sub
        Public MustOverride Function Probability() As Decimal
        Public MustOverride Function Index() As Decimal
        Public MustOverride Function Clone() As AbsDatiModello
        Public Overridable Sub Accumulate(dm As AbsDatiModello)
            Temp += dm.Temp
            UmRel += dm.UmRel
            Prec += dm.Prec
        End Sub
        Public Overridable Sub Aggregate(cnt As Decimal)
            Temp /= cnt
            UmRel /= cnt
        End Sub
    End Class

    Protected Class Par4
        Public ReadOnly a As Decimal
        Public ReadOnly b As Decimal
        Public ReadOnly c As Decimal
        Public ReadOnly d As Decimal
        Public Sub New(ByVal _a_ As Decimal, ByVal _b_ As Decimal, Optional ByVal _c_ As Decimal = 0, Optional ByVal _d_ As Decimal = 0)
            a = _a_
            b = _b_
            c = _c_
            d = _d_
        End Sub
        Public Function Fun1(ByVal x As Decimal) As Decimal
            Return Math.Pow(a * Math.Pow(x, b) * (1D - x), c)
        End Function
    End Class

    Private Class AggregatorHelper
        Private _dm_aggr As AbsDatiModello
        Private _cnt As Integer
        Private Sub _init(dm As AbsDatiModello)
            _dm_aggr = dm.Clone()
            _cnt = 1
        End Sub
        Public Sub New()
            _dm_aggr = Nothing
            _cnt = 0
        End Sub
        Public Sub New(dm As AbsDatiModello)
            _init(dm)
        End Sub
        Public Function Process(dm As AbsDatiModello) As AbsDatiModello

            If _dm_aggr Is Nothing Then

                _init(dm)

                Return Nothing
            End If

            If _dm_aggr.DataOra.DayOfYear = dm.DataOra.DayOfYear Then

                _dm_aggr.Accumulate(dm)
                _cnt += 1

                Return Nothing
            End If

            _dm_aggr.Aggregate(_cnt)

            Dim ret_aggr = _dm_aggr

            _init(dm)

            Return ret_aggr
        End Function
        Public Function Current() As AbsDatiModello

            If _dm_aggr Is Nothing Then

                Return Nothing
            End If

            _dm_aggr.Aggregate(_cnt)

            Return _dm_aggr
        End Function
    End Class

    Private ReadOnly _datiModello As List(Of AbsDatiModello)

    Public Sub New()
        _datiModello = New List(Of AbsDatiModello)
    End Sub
    Public Function IsEmpty() As Boolean
        Return Not _datiModello.Any()
    End Function
    Public Sub Add(dm As MeteoDSSItem, aw As Decimal)
        _datiModello.Add(Create(dm, aw))
    End Sub

    Protected MustOverride Function Create(dm As MeteoDSSItem, aw As Decimal) As AbsDatiModello

    Public Function GeneraOutput(mycotox_params As ML_Params) As cRisultatoModello

        Dim rismod As New cRisultatoModello

        Dim output As New OutputModello

        output.aggiungiColonna("Data", GetType(Date), "Data", "dd/MM/yyyy")
        output.aggiungiColonna("Temperatura", GetType(Decimal), Gias.Temperatura & " (°C)", "0.00")
        output.aggiungiColonna("Precipitazioni", GetType(Decimal), Gias.Pioggia & " (mm)", "0.00")
        output.aggiungiColonna("UmiditaRelativa", GetType(Decimal), Gias.UmiditaRelativa & " (%)", "0.00")
        'output.aggiungiColonna("Bagnatura", GetType(Boolean), Gias.BagnaturaFogliare & " (" & Gias.Si & "/" & Gias.No & ")", Gias.Si & "|" & Gias.No)
        output.aggiungiColonna("aw", GetType(Decimal), "Aw", "0.00")

        AggiungiColonneOutput(output)

        output.aggiungiColonna("Probability", GetType(Decimal), "Probabilità", "0.00")
        output.aggiungiColonna("Prob_Ind", "")._hidden = True
        Dim indProb As New OutputIndicator("Prob_Ind", OutputIndicator.enum_IndicatorType.Colors)
        indProb.addStopValue(0.2, New OutputIndicator.RGB(0, 128, 0))               'verde
        indProb.addStopValue(0.4, New OutputIndicator.RGB(192, 255, 0))             'verdino
        indProb.addStopValue(0.6, New OutputIndicator.RGB(255, 240, 0))             'giallo
        indProb.addStopValue(0.8, New OutputIndicator.RGB(255, 128, 0))             'arancio
        indProb.addStopValue(Decimal.MaxValue, New OutputIndicator.RGB(240, 0, 0))  'rosso
        indProb.setFieldVal("Probability")

        Dim aggrHlp As New AggregatorHelper
        Dim dm_gg As AbsDatiModello
        Dim model_index As Decimal

        For idx = 0 To _datiModello.Count

            If idx < _datiModello.Count Then

                dm_gg = aggrHlp.Process(_datiModello(idx))
            Else

                dm_gg = aggrHlp.Current()
            End If

            If dm_gg IsNot Nothing Then

                output.AddField(dm_gg.DataOra)
                output.AddField(dm_gg.Temp)
                output.AddField(dm_gg.Prec)
                output.AddField(dm_gg.UmRel)
                output.AddField(dm_gg.aw)

                RiempiColonneOutput(dm_gg, output)

                Dim probability As Decimal = dm_gg.Probability()

                output.AddField(probability)
                output.AddField(indProb.colorForVal(probability))

                output.Commit()

                model_index = dm_gg.Index()
            End If
        Next

        rismod.Modello_Tabella1 = output.Output({indProb}.ToList, indProb.PlotBands(0.2))



        If mycotox_params IsNot Nothing Then

            mycotox_params.Model_Index = model_index

            Dim computer As Compute_ML = GetCompute_ML(mycotox_params)

            Dim val = computer.Compute()

            Dim msg = "<div style='display:flex; justify-content:center; font-size:larger; color:#d2691e;'>"
            msg &= "<div style='display:flex; gap:7px; border:1px solid #fff8dc; padding:3px 6px; border-radius:5px; background-color:#fff8dc99;'>"
            msg &= "<div>Previsione presenza micotossine</div>"
            msg &= "<div style='font-weight:bold;'>" & val.ToString("0.00") & "</div>"
            msg &= "</div>"
            msg &= "</div>"

            rismod.Modello_WarningMsg = msg
        End If

        Return rismod
    End Function

    Protected MustOverride Sub AggiungiColonneOutput(output As OutputModello)
    Protected MustOverride Sub RiempiColonneOutput(dm_gg As AbsDatiModello, om As OutputModello)

    Public Sub GeneraIndicatore_Vecchio(ByRef indic As cRisultatoModelloIndicatori.Indicatore, ByVal dataFine As Date)

        Dim last_d_gg As AbsDatiModello = _datiModello.Last

        indic.Fill(last_d_gg.Probability(), 1, {0.35, 0.7}, last_d_gg.DataOra.Date, dataFine)

    End Sub

    Public Sub GeneraIndicatore(risElab As RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione)

        Dim last_idx As Integer = _datiModello.Count - 1

        Dim aggrHlp As New AggregatorHelper(_datiModello(last_idx))
        Dim dm_gg As AbsDatiModello = Nothing

        While dm_gg Is Nothing

            last_idx -= 1
            If last_idx >= 0 Then

                dm_gg = aggrHlp.Process(_datiModello(last_idx))
            Else

                dm_gg = aggrHlp.Current()
            End If
        End While

        Dim prob As Decimal = dm_gg.Probability()

#If False Then
classe 1: da 0.00 a 0.20 – Verde 
classe 2: da 0.21 a 0.40 – verdino
classe 3: da 0.41 a 0.60 - giallo
classe 4: da 0.61 a 0.80 – arancione
classe 5: da 0.81 a 1.00 – rosso 
#End If

        Dim bands As New List(Of RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione.Band) From {
            New RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione.Band With {
                  .Value = 0.2,
                  .Color = "#008000"    'verde
                  },
            New RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione.Band With {
                  .Value = 0.4,
                  .Color = "#c0ff00"    'verdino
                  },
            New RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione.Band With {
                  .Value = 0.6,
                  .Color = "#fff000"    'giallo
                  },
            New RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione.Band With {
                  .Value = 0.8,
                  .Color = "#ff8000"    'arancio
                  },
            New RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione.Band With {
                  .Value = Decimal.MaxValue,
                  .Color = "#f00000"    'rosso
                  }
        }

        risElab.FillCustom(prob, 1, bands)

        RiempiIndicatore(dm_gg, risElab.OutputGridValues)

        Dim outProb As New RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione.OutputGridValue("prob", "Probabilità")
        outProb.SetValue(Of Decimal)(prob, "0.00")

        risElab.OutputGridValues.Add(outProb)
    End Sub

    Protected MustOverride Sub RiempiIndicatore(dm_gg As AbsDatiModello, ogv As List(Of RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione.OutputGridValue))
    Protected MustOverride Function GetCompute_ML(mycotox_params As ML_Params) As Compute_ML
End Class



Public Class UniCatt_Mais_Modello_AFLA
    Inherits AbsUniCatt_Mais_Modello

    Protected Class DatiModello_AFLA
        Inherits AbsDatiModello

        Public Inf_SPOR As Decimal
        Public Inf_DISP As Decimal
        Public Inf_GER As Decimal
        Public Inf_INF As Decimal
        Public Inf_INFcum As Decimal
        Public Mycotox_GROW_T As Decimal
        Public Mycotox_GROW_aw As Decimal
        Public Mycotox_AFL_T As Decimal
        Public Mycotox_AFL_aw As Decimal
        Public Mycotox_AF_index As Decimal

        Public Sub New(dm As MeteoDSSItem, aw As Decimal)
            MyBase.New(dm, aw)
        End Sub
        Public Overrides Function Probability() As Decimal
            If Mycotox_AF_index > 0 Then
                Return 1D / (1D + Math.Exp(-(-2.727D + 0.001D * Mycotox_AF_index)))
            End If
            Return 0
        End Function
        Public Overrides Function Index() As Decimal
            Return Mycotox_AF_index
        End Function
        Public Overrides Function Clone() As AbsDatiModello
            Return DirectCast(Me.MemberwiseClone(), DatiModello_AFLA)
        End Function
        Public Overrides Sub Accumulate(dm As AbsDatiModello)
            MyBase.Accumulate(dm)

            Dim afla = DirectCast(dm, DatiModello_AFLA)

            Inf_SPOR += afla.Inf_SPOR
            Inf_DISP += afla.Inf_DISP
            Inf_GER += afla.Inf_GER
            Inf_INF += afla.Inf_INF
            Inf_INFcum += afla.Inf_INFcum
            Mycotox_GROW_T += afla.Mycotox_GROW_T
            Mycotox_GROW_aw += afla.Mycotox_GROW_aw
            Mycotox_AFL_T += afla.Mycotox_AFL_T
            Mycotox_AFL_aw += afla.Mycotox_AFL_aw
            Mycotox_AF_index += afla.Mycotox_AF_index
        End Sub
        Public Overrides Sub Aggregate(cnt As Decimal)
            MyBase.Aggregate(cnt)

            Mycotox_AFL_aw /= cnt
            Mycotox_AF_index /= cnt
        End Sub
    End Class

    Private ReadOnly parSPOR As Par4
    Private ReadOnly parGER As Par4
    Private ReadOnly parGROW_T As Par4
    Private ReadOnly parGROW_aw As Par4
    Private ReadOnly parAFL_T As Par4
    Private ReadOnly Q24H As Queue(Of Decimal)
    Private PrevINFcum As Decimal
    Private PrevAF_index As Decimal

    Public Sub New()
        MyBase.New()

        parSPOR = New Par4(5.28, 2.05, 0.98)
        parGER = New Par4(18.53, 5.91, 0.41)
        parGROW_T = New Par4(5.98, 1.7, 1.43)
        parGROW_aw = New Par4(27.37, -30.08, 1.12)
        parAFL_T = New Par4(4.84, 1.32, 5.59)

        Q24H = New Queue(Of Decimal)

        For i = 1 To 24
            Q24H.Enqueue(0)
        Next
        PrevINFcum = 0
        PrevAF_index = 0
    End Sub

    Protected Overrides Function Create(dm As MeteoDSSItem, aw As Decimal) As AbsDatiModello
        Dim afla = New DatiModello_AFLA(dm, aw)

        afla.Inf_SPOR = parSPOR.Fun1(Math.Max(0, (dm.Temp - 5D) / (45D - 5D)))
        afla.Inf_DISP = 0
        If dm.Prec = 0 AndAlso dm.UmRel < 80 Then
            afla.Inf_DISP = afla.Inf_SPOR
        End If
        Dim Inf_fGER As Decimal = parGER.Fun1(Math.Max(0, (dm.Temp - 10D) / (47D - 10D)))

        Q24H.Enqueue(afla.Inf_DISP)
        Q24H.Dequeue()

        afla.Inf_GER = 0
        afla.Inf_INF = 0
        If dm.Prec > 0 OrElse dm.Bagn > 0 OrElse dm.UmRel / 100D >= (0.0004 * dm.Temp - 0.0261) * dm.Temp + 1.2469 Then
            afla.Inf_GER = Inf_fGER
            afla.Inf_INF = (Q24H.Sum() / 24D) * afla.Inf_GER
        End If

        afla.Inf_INFcum = PrevINFcum
        If aw > 0.95 Then
            afla.Inf_INFcum += afla.Inf_INF
        End If
        PrevINFcum = afla.Inf_INFcum

        afla.Mycotox_GROW_T = parGROW_T.Fun1(Math.Max(0, (dm.Temp - 5D) / 43D))
        afla.Mycotox_GROW_aw = 0
        If aw >= 0.75 Then
            afla.Mycotox_GROW_aw = parGROW_aw.c / (1D + Math.Exp(parGROW_aw.a + parGROW_aw.b * aw))
        End If
        Dim Mycotox_GROW As Decimal = afla.Mycotox_GROW_T * afla.Mycotox_GROW_aw

        afla.Mycotox_AFL_T = parAFL_T.Fun1(Math.Max(0, (dm.Temp - 5D) / (42D - 5D)))
        afla.Mycotox_AFL_aw = (177.1D * aw - 327.23D) * aw + 151.37D

        Dim Mycotox_AFLA As Decimal = afla.Mycotox_AFL_T * afla.Mycotox_AFL_aw

        Dim Mycotox_MYC As Decimal = afla.Inf_INFcum * Mycotox_GROW * Mycotox_AFLA

        afla.Mycotox_AF_index = Mycotox_MYC + PrevAF_index
        PrevAF_index = afla.Mycotox_AF_index

        Return afla
    End Function

    Protected Overrides Sub AggiungiColonneOutput(output As OutputModello)
        'output.aggiungiColonna("aw", GetType(Decimal), "Aw", "0.00")
        output.aggiungiColonna("af_index", GetType(Decimal), "AF index", "0.00")
    End Sub

    Protected Overrides Sub RiempiColonneOutput(dm_gg As AbsDatiModello, om As OutputModello)
        Dim dm_afla = DirectCast(dm_gg, DatiModello_AFLA)

        'om.AddField(dm_afla.Mycotox_AFL_aw)
        om.AddField(dm_afla.Mycotox_AF_index)
    End Sub

    Protected Overrides Sub RiempiIndicatore(dm_gg As AbsDatiModello, ogv As List(Of RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione.OutputGridValue))
        Dim dm_afla = DirectCast(dm_gg, DatiModello_AFLA)

        Dim out_aw As New RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione.OutputGridValue("aw", "Aw")
        'Dim out_af_index As New RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione.OutputGridValue("af_index", "AF index")
        Dim out_af_index As New RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione.OutputGridValue("index", "Index")

        out_aw.SetValue(Of Decimal)(dm_afla.Mycotox_AFL_aw, "0.00")
        out_af_index.SetValue(Of Decimal)(dm_afla.Mycotox_AF_index, "0.00")

        ogv.Add(out_aw)
        ogv.Add(out_af_index)
    End Sub

    Protected Overrides Function GetCompute_ML(mycotox_params As ML_Params) As Compute_ML
        Return New Compute_ML_AFLA(mycotox_params)
    End Function
End Class



Public Class UniCatt_Mais_Modello_FER
    Inherits AbsUniCatt_Mais_Modello

    Protected Class DatiModello_FER
        Inherits AbsDatiModello

        Public SPO_aw As Decimal
        Public SPO_T As Decimal
        Public GER As Decimal
        Public INF As Decimal
        Public INFcum As Decimal
        Public INV_T As Decimal
        Public INV_aw As Decimal
        Public FB_T As Decimal
        Public FB_aw As Decimal
        Public FB_index As Decimal

        Public Sub New(dm As MeteoDSSItem, aw As Decimal)
            MyBase.New(dm, aw)
        End Sub
        Public Overrides Function Probability() As Decimal
            If FB_index > 0 Then
                Return 1D / (1D + Math.Exp(-(-3.412D + 0.073D * (FB_index / 1000D))))
            End If
            Return 0
        End Function
        Public Overrides Function Index() As Decimal
            Return FB_index
        End Function
        Public Overrides Function Clone() As AbsDatiModello
            Return DirectCast(MemberwiseClone(), DatiModello_FER)
        End Function
        Public Overrides Sub Accumulate(dm As AbsDatiModello)
            MyBase.Accumulate(dm)

            Dim fer = DirectCast(dm, DatiModello_FER)

            SPO_aw += fer.SPO_aw
            SPO_T += fer.SPO_T
            GER += fer.GER
            INF += fer.INF
            INFcum += fer.INFcum
            INV_T += fer.INV_T
            INV_aw += fer.INV_aw
            FB_T += fer.FB_T
            FB_aw += fer.FB_aw
            FB_index += fer.FB_index
        End Sub
        Public Overrides Sub Aggregate(cnt As Decimal)
            MyBase.Aggregate(cnt)

            FB_aw /= cnt
            FB_index /= cnt
        End Sub
    End Class

    Private ReadOnly parSPO_aw As Par4
    Private ReadOnly parSPO_T As Par4
    Private ReadOnly parINF_GROWTH As Par4
    Private ReadOnly parINV_T As Par4
    Private ReadOnly parINV_aw As Par4
    Private ReadOnly parFB_T As Par4
    Private ReadOnly parFB_aw As Par4
    Private PrevINFcum As Decimal
    Private PrevFB_index As Decimal

    Public Sub New()
        MyBase.New()

        parSPO_aw = New Par4(0.0147, -25.83)
        parSPO_T = New Par4(7.04, 2.17, 0.667)
        parINF_GROWTH = New Par4(7.38, 2.189, 0.797)
        parINV_T = New Par4(6.951, 2.079, 1.4803)
        parINV_aw = New Par4(1.2, 57.5, 59.93)
        parFB_T = New Par4(8.42, 2.631, 0.904)
        parFB_aw = New Par4(5.445, 157.2, 166.1, 230.6)

        PrevINFcum = 0
        PrevFB_index = 0
    End Sub

    Private Function Teq(ByVal t As Decimal, ByVal tmin As Decimal, ByVal tmax As Decimal) As Decimal
        If tmin <= t AndAlso t <= tmax Then
            Return (t - tmin) / (tmax - tmin)
        End If
        Return 0
    End Function

    Protected Overrides Function Create(dm As MeteoDSSItem, aw As Decimal) As AbsDatiModello
        Dim fer As New DatiModello_FER(dm, aw)

        fer.SPO_aw = 0
        fer.SPO_T = 0

        If aw >= 0.84 Then
            fer.SPO_aw = 1 - parSPO_aw.a * Math.Pow(aw, parSPO_aw.b)
        End If

        If dm.Temp > 0 Then
            fer.SPO_T = parSPO_T.Fun1(Teq(dm.Temp, 0, 40))
        End If

        Dim SPOR As Decimal = fer.SPO_aw * fer.SPO_T

        Dim fGER As Decimal = parINF_GROWTH.Fun1(Teq(dm.Temp, 5, 40))

        'GER y/n = dm.Bagn > 0
        fer.GER = fGER * If(dm.Bagn > 0, 1D, 0D)
        fer.INF = fer.GER
        fer.INFcum = PrevINFcum
        If aw > 0.95 Then
            fer.INFcum += fer.INF
        End If
        PrevINFcum = fer.INFcum

        fer.INV_T = 0
        If dm.Temp > 0 Then
            fer.INV_T = parINV_T.Fun1(Teq(dm.Temp, 0, 40))
        End If
        fer.INV_aw = Math.Min(1, parINV_aw.a / (1 + Math.Exp(parINV_aw.b - parINV_aw.c * aw)))
        fer.FB_T = parFB_T.Fun1(Teq(dm.Temp, 9, 37))
        fer.FB_aw = (Math.Exp(parFB_aw.a / (1 + Math.Exp(parFB_aw.b - parFB_aw.c * aw))) - 1) / parFB_aw.d

        Dim INV As Decimal = fer.INV_T * fer.INV_aw
        Dim FB As Decimal = fer.FB_T * fer.FB_aw
        Dim MYC_FB As Decimal = fer.INFcum * INV * FB

        fer.FB_index = MYC_FB + PrevFB_index
        PrevFB_index = fer.FB_index

        Return fer
    End Function

    Protected Overrides Sub AggiungiColonneOutput(output As OutputModello)
        'output.aggiungiColonna("aw", GetType(Decimal), "Aw", "0.00")
        output.aggiungiColonna("fb_index", GetType(Decimal), "FB index", "0.00")
    End Sub

    Protected Overrides Sub RiempiColonneOutput(dm_gg As AbsDatiModello, om As OutputModello)
        Dim dm_fer = DirectCast(dm_gg, DatiModello_FER)

        'om.AddField(dm_fer.FB_aw)
        om.AddField(dm_fer.FB_index)
    End Sub

    Protected Overrides Sub RiempiIndicatore(dm_gg As AbsDatiModello, ogv As List(Of RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione.OutputGridValue))
        Dim dm_fer = DirectCast(dm_gg, DatiModello_FER)

        Dim out_aw As New RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione.OutputGridValue("aw", "Aw")
        'Dim out_fb_index As New RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione.OutputGridValue("fb_index", "FB index")
        Dim out_fb_index As New RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione.OutputGridValue("index", "Index")

        out_aw.SetValue(Of Decimal)(dm_fer.FB_aw, "0.00")
        out_fb_index.SetValue(Of Decimal)(dm_fer.FB_index, "0.00")

        ogv.Add(out_aw)
        ogv.Add(out_fb_index)
    End Sub

    Protected Overrides Function GetCompute_ML(mycotox_params As ML_Params) As Compute_ML
        Return New Compute_ML_FUMO(mycotox_params)
    End Function
End Class



Public Class UniCatt_Mais
    Inherits AbstractModello

    Private ReadOnly _mod_cod As enum_ModelliPrevisionali
    Private ReadOnly _dtEmergence As Date
    Private ReadOnly GDD_T_base_10 As Decimal
    Private ReadOnly _mycotox_params As ML_Params

    Public Sub New(ByVal datiMeteo As MeteoReadOnlyList, ByVal Mod_Cod As Integer, ByVal parametriAggiuntivi As String)
        MyBase.New(datiMeteo)

        _mod_cod = Mod_Cod

        GDD_T_base_10 = 750

        Dim lettore As New LettoreParametri(parametriAggiuntivi)

        Dim dtEmergence As Date = lettore.DateOrDefault("DataEmergenza", Date.MinValue)
        Dim dtEmergence_gg As Integer = lettore.IntOrDefault("DataEmergenza_gg", 0)

        _dtEmergence = _data_from_doy(dtEmergence, dtEmergence_gg, 4, 15) '15 Aprile

        _mycotox_params = Nothing

        Dim mycotox_params As JObject = lettore.GetObject("previsione_micotox")

        If mycotox_params IsNot Nothing Then

            Dim data_semina_gg As Integer = mycotox_params("data_semina_gg")
            Dim data_raccolta_gg As Integer = mycotox_params("data_raccolta_gg")

            Dim data_semina As Date = _data_from_doy(Date.MinValue, data_semina_gg, 1, 1)
            Dim data_raccolta As Date = _data_from_doy(Date.MinValue, data_raccolta_gg, 1, 1)

            Dim cinfo = DateTimeFormatInfo.CurrentInfo

            Dim sowing_week = cinfo.Calendar.GetWeekOfYear(data_semina, cinfo.CalendarWeekRule, cinfo.FirstDayOfWeek)
            Dim harvest_week = cinfo.Calendar.GetWeekOfYear(data_raccolta, cinfo.CalendarWeekRule, cinfo.FirstDayOfWeek)

            Dim sowing_week_idx As Integer = 0
            Select Case sowing_week
                Case 10 To 12
                    sowing_week_idx = 1
                Case 13, 14
                    sowing_week_idx = 2
                Case 15, 16
                    sowing_week_idx = 3
                Case Is >= 17
                    sowing_week_idx = 4
            End Select

            Dim harvest_week_idx As Integer = 0
            Select Case harvest_week
                Case 32 To 35
                    harvest_week_idx = 1
                Case 36, 37
                    harvest_week_idx = 2
                Case 38, 39
                    harvest_week_idx = 3
                Case Is >= 40
                    harvest_week_idx = 4
            End Select

            _mycotox_params = New ML_Params With {
                .FAO_class = mycotox_params("classe_fao"),
                .Sowing_Week = sowing_week_idx,
                .Harvest_Week = harvest_week_idx,
                .Preceding_Crop = mycotox_params("prev_crop"),
                .ECB_Severity = mycotox_params("danno_piralide"),
                .Umidita_Cariosside = mycotox_params("umidita_cariosside"),
                .Giorni_Campo = data_raccolta_gg - data_semina_gg
            }
        End If
    End Sub

    Public Shared Function EsponiParams(ByVal params As String) As String

        Dim lettore As New LettoreParametri(params)

        Dim str_dt As String = lettore.OutputFromDate("DataEmergenza", "Data emergenza: ")

        If String.IsNullOrEmpty(str_dt) Then

            str_dt = lettore.OutputFromDOY("DataEmergenza_gg", "Data emergenza: ")
        End If

        Return str_dt
    End Function

    Protected Overrides Function _elaboraModello() As cRisultatoModello

        Dim risModello As cRisultatoModello = Nothing

        Try

            Dim modello = CreaModello()

            If modello IsNot Nothing Then

                risModello = modello.GeneraOutput(_mycotox_params)
            End If

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            _errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

            risModello = Nothing
        End Try

        Return risModello
    End Function

    Protected Overrides Function _elaboraIndicatore_vecchio(dataInizio As Date, dataFine As Date) As cRisultatoModelloIndicatori.Indicatore

        Dim risIndic As New cRisultatoModelloIndicatori.Indicatore("", dataInizio, dataFine)

        Dim flagOK As Boolean = False

        Try

            Dim modello = CreaModello()

            If modello IsNot Nothing Then

                modello.GeneraIndicatore_Vecchio(risIndic, dataFine)

                flagOK = True
            End If

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            _errore = "Errore durante l'operazione: " & vbCrLf &
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

            Dim modello = CreaModello()

            If modello IsNot Nothing Then

                modello.GeneraIndicatore(risElab)

                flagOK = True
            End If

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            _errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        If Not flagOK Then

            risElab.Errore(_errore)
        End If

        Return risElab
    End Function

    Private Function CreaModello() As AbsUniCatt_Mais_Modello

        If Not _datiMeteo.Any Then
            Return Nothing
        End If

        Dim idx As Integer = 0
        While idx < _datiMeteo.Count AndAlso _datiMeteo(idx).DataOra.Date < _dtEmergence
            idx += 1
        End While

        If idx >= _datiMeteo.Count Then

            _errore = "Non è stata raggiunta la data di emergenza."

            Return Nothing
        End If

        Dim modello As AbsUniCatt_Mais_Modello

        Select Case _mod_cod

            Case enum_ModelliPrevisionali.UniCatt_Mais_AFLA

                modello = New UniCatt_Mais_Modello_AFLA()

            Case enum_ModelliPrevisionali.UniCatt_Mais_FER

                modello = New UniCatt_Mais_Modello_FER()

            Case Else

                Return Nothing

        End Select

        Dim T_base10 As Decimal
        Dim Tsum As Decimal = 0
        Dim Tsum_aw As Decimal = 0
        Dim aw As Decimal

        While idx < _datiMeteo.Count

            Dim dm = _datiMeteo(idx)

            T_base10 = Math.Max(0, dm.Temp - 10D) / 24D
            Tsum += T_base10

            If Tsum > GDD_T_base_10 Then

                Tsum_aw += (dm.Temp / 24D)

                aw = 1 - Math.Exp(-15.269D * Math.Exp(-0.121D * Tsum_aw / 100D))

                modello.Add(dm, aw)
            End If

            idx += 1
        End While

        If modello.IsEmpty() Then

            _errore = "Non è stata raggiunta la fase necessaria per lo sviluppo dell'avversità."

            Return Nothing
        End If

        Return modello
    End Function
End Class



'**************************************************************************************************
'Machine Learning *********************************************************************************
'**************************************************************************************************



Public Class ML_Params

    Public FAO_class As Integer
    '1: 200–300
    '2: 400
    '3: 500
    '4: 600–700

    Public Preceding_Crop As Integer
    '1: arable crops
    '2: small grain
    '3: maize

    Public Sowing_Week As Integer
    '1: 10–12
    '2: 13–14
    '3: 15–16
    '4: 17+

    Public Harvest_Week As Integer
    '1: 32–35
    '2: 36–37
    '3: 38–39
    '4: 40+

    Public ECB_Severity As Integer
    '1: No/Minor-damage
    '2: Medium damage
    '3: Severe damage

    Public Umidita_Cariosside As Decimal
    Public Giorni_Campo As Integer
    Public Model_Index As Decimal
End Class

Public MustInherit Class Compute_ML

    Protected Class avg_std_values
        Public INDEX_AVG As Decimal 'media indice del modello
        Public INDEX_STD As Decimal 'deviazione standard indice del modello
        Public DAY_AVG As Decimal   'media giorni in campo
        Public DAY_STD As Decimal   'deviazione standard giorni in campo
        Public KER_AVG As Decimal   'media umidità della cariosside
        Public KER_STD As Decimal   'deviazione standard umidità della cariosside
    End Class

    Protected _hidden_layer(,) As Decimal
    Protected _output_layer(,) As Decimal
    Protected _input_array(8) As Decimal

    Protected Sub New(par As ML_Params, avg_std As avg_std_values)

        _input_array(5) = (par.Umidita_Cariosside - avg_std.KER_AVG) / avg_std.KER_STD
        _input_array(6) = (par.Giorni_Campo - avg_std.DAY_AVG) / avg_std.DAY_STD
        _input_array(7) = (par.Model_Index - avg_std.INDEX_AVG) / avg_std.INDEX_STD
        'Aggiungere un ultimo 1 alla fine dell'input è necessario ai fini della rete neurale, questo valore si chiama Bias ed è sempre 1.
        _input_array(8) = 1
    End Sub

    Public Function Compute() As Decimal

        Dim H_L = weighted_sum(_input_array, _hidden_layer) 'Calcolo del nuovo input

        Dim len_H_L As Integer = UBound(H_L)

        Dim Activated_H_L(len_H_L + 1) As Decimal

        'Applicazione della trasformazione per il passaggio dall'input originale al primo strato della rete
        For i = 0 To len_H_L
            Activated_H_L(i) = ReLU(H_L(i))
        Next
        'Aggiungo un 1 alla fine del nuovo array di input. Aggiungere un ultimo 1 alla fine dell'input è necessario ai fini della rete neurale, questo valore si chiama Bias ed è sempre 1.
        Activated_H_L(len_H_L + 1) = 1

        'Calcolo dell'output finale
        Dim O_L = weighted_sum(Activated_H_L, _output_layer)

#If False Then
            
    if probability:
        return (Sigmoid(O_L[0]), Sigmoid(O_L[0], probability = True))    
    else:
        return Sigmoid(O_L[0])

#End If
        Return Sigmoid(O_L(0), True)
    End Function

    Private Function weighted_sum(input_variables As Decimal(), weights As Decimal(,)) As Decimal()
        ' La somma pesata serve per calcolare l'attivazione dei nodi della rete neurale
        ' Per ogni 'N' del dizionario sopra dichiarato bisogna moltiplicare ogni input[i] per ogni relativo peso hidden_layer[i] tutti i risultati vanno poi sommati. (Somma = SUM(input_array[j] * hidden_layer[i][j]))
        ' Per il primo strato della rete neurale bisogna poi applicare la funzione ReLU, l'output della funzione sarà quindi il nuovo input per il passaggio successivo (il nuovo array di input avrà quindi 80 elementi nel modello AFLA, e 50 elementi nel modello FUMO)
        ' Lo stesso principio della somma pesata deve essere poi applicato. Quindi somma pesata -> funzione Sigmoid, che da due risultati, uno espresso in percentuale (0.0 - 1.0) e uno di classificazione (0 o 1 in cui 0 significa al di sotto dei limiti di legge e 1 significa al di sopra dei limiti di legge)

        Dim outer_cnt As Integer = UBound(weights, 1)
        Dim inner_cnt As Integer = UBound(input_variables)

        Dim new_input(outer_cnt) As Decimal

        Dim w_s As Decimal

        For outer_idx = 0 To outer_cnt

            w_s = 0

            For inner_idx = 0 To inner_cnt

                w_s += input_variables(inner_idx) * weights(outer_idx, inner_idx)
            Next

            new_input(outer_idx) = w_s
        Next

        Return new_input
    End Function

    Private Function ReLU(x As Decimal) As Decimal
        Return Math.Max(0D, x)
    End Function

    Private Function Sigmoid(x As Decimal, probability As Boolean) As Decimal

        Dim res As Decimal = 1D / (1D + Math.Exp(-x))

        If probability Then

            Return res
        End If

        If res > 0.5 Then

            Return 1
        End If

        Return 0
    End Function
End Class

Public Class Compute_ML_AFLA
    Inherits Compute_ML

    Public Sub New(par As ML_Params)
        MyBase.New(par, New avg_std_values With {
                   .INDEX_AVG = 2903.86704485488,
                   .INDEX_STD = 2228.67904159283,
                   .DAY_AVG = 158.042216358839,
                   .DAY_STD = 16.6789894554392,
                   .KER_AVG = 20.65514511873,
                   .KER_STD = 3.55857912129
                   })

        _input_array(0) = par.Preceding_Crop
        _input_array(1) = par.Sowing_Week
        _input_array(2) = par.Harvest_Week
        _input_array(3) = par.ECB_Severity
        _input_array(4) = par.FAO_class

        _hidden_layer = {
            {-0.147017897, -0.126294676, 0.192909478, 0.355770345, -0.15558692, -0.423809746, -0.810948195, -0.02862928, 0.093430568},
            {0.191642776, 0.056152425, -0.084137791, -0.174399074, -0.33207317, 0.508592075, -0.389932115, -0.176854426, 0.243650921},
            {0.25064614, -0.093917546, -0.094038746, -0.383129828, -0.265460289, -0.305031489, 0.033967634, 0.469397864, 0.428357915},
            {-0.104442309, 0.074340018, 0.354578671, -0.390467799, -0.275313027, 0.452968344, 0.008317784, 0.285100031, -0.101374419},
            {-0.123502391, 0.056574339, 0.465035663, 0.022988247, -0.152687621, 0.622012569, -0.222602057, 0.145200312, -0.147581523},
            {-0.11862946, -0.11338296, 0.318965865, -0.213079555, 0.022225608, -0.65995368, 0.311571532, 0.584790277, -0.28246471},
            {-0.01854134, 0.104611762, -0.299821227, 0.248755973, 0.165502755, -0.287458092, 0.100476366, -0.174707266, 0.018319203},
            {0.271099258, -0.413889252, -0.261824695, -0.243428356, 0.197103536, -0.476205244, 0.164163467, 0.434771063, -0.044683801},
            {0.277513255, 0.102676541, 0.155066631, -0.247220439, -0.131416255, 0.052434487, 0.30710602, 0.145677092, 0.137225089},
            {0.108850999, 0.410145406, -0.273926876, -0.427300608, -0.00989933, -0.267157839, 0.263128628, 0.439219748, -0.153025145},
            {0.283207804, 0.014133428, -0.140639763, -0.500402529, -0.133128792, -0.335568349, -0.050435536, 0.54881339, 0.171225565},
            {-0.005036837, -0.027992877, -0.297664905, 0.231828231, 0.257319901, 0.158333751, -0.057817099, -0.177903259, 0.144871354},
            {-0.245820649, -0.374381486, 0.119732675, 0.568908357, -0.457506945, -0.462375973, 0.475073827, -0.695281283, -0.115585389},
            {0.263940544, 0.084454861, -0.300606807, -0.032381305, 0.162260448, 0.085339468, -0.154025309, -0.049239122, -0.113404511},
            {-0.192653288, -0.03505141, 0.301212571, 0.223001038, 0.13618583, -0.181798035, 0.332379513, 0.536664474, 0.239638737},
            {-7.7102E-56, -7.1419E-60, -8.71862E-62, -1.0769E-55, 1.01265E-61, -2.91448E-63, -3.02998E-64, -7.53921E-66, -0.230547618},
            {-0.078401056, 0.054123197, -0.028101859, 0.017706998, -0.090422982, -0.128603885, -0.071321532, -0.165970542, -0.005717331},
            {0.221868769, -0.27459064, 0.212098415, 0.023430248, 0.01201222, -0.589615786, 0.060997717, 0.434782878, -0.067498948},
            {0.174871531, 0.035419578, 0.071371593, 0.161677015, 0.071200984, -0.060532846, -0.121090987, 0.089538453, 0.026018546},
            {0.233008599, -0.433610207, -0.141601527, -0.417593637, -0.013478419, -0.034128551, -0.018073931, 0.335001534, 0.290453707},
            {0.326684977, 0.216570451, -0.323776449, -0.084500648, -0.167630963, -0.034828867, -0.063754106, -0.298998293, -0.261639591},
            {0.094211658, -0.077787522, 0.153450006, -0.23842565, -0.205548062, 0.48744921, 0.1700841, 0.206299212, 0.171167637},
            {0.018929304, 0.253787048, 0.180035143, -0.482488513, -0.189136512, -0.055905848, 0.024967035, 0.111561932, -0.429888565},
            {0.100869735, 0.329133147, -0.292358161, 0.223726227, -0.280563041, -0.175884524, 0.13201386, -0.216176004, -0.070765855},
            {-0.221885204, -0.197468978, 0.119978525, 0.080808282, 0.271438877, 0.171457802, -0.14581796, 0.064765746, -0.146534513},
            {-0.100016827, -0.470300689, 0.175440317, 0.09529407, -0.070893397, 0.068851147, 0.380951064, -0.701009323, 0.257843661},
            {-0.410053829, 0.330252092, 0.083584831, -0.189986699, -0.131311848, 0.092330749, 0.432104292, -0.153335039, 0.104659271},
            {0.295517287, 0.089728679, 0.023276876, 0.145420102, -0.090079096, -0.034690886, -0.682430044, -0.278041593, -0.143213808},
            {0.101488524, -0.416998951, -0.230709691, -0.193778529, 0.376228719, 0.610326525, -0.327706096, -0.255014397, -0.110730911},
            {-0.216449119, 0.379137234, 0.034361233, -0.122480389, 0.235525678, -0.15751033, 0.006757772, 0.132089687, -0.038412553},
            {-0.233394017, -0.084066759, -0.025541512, 0.359242698, 0.190867489, -0.18312541, -0.292090315, 0.488925171, -0.060220733},
            {0.17989344, 0.038732984, -0.037186995, 0.404129447, -0.262711963, 0.261936548, -0.360948405, -0.041508287, -0.070958491},
            {-0.07734637, 0.090106641, -0.057125553, -0.113730438, -0.000691398, -0.115847237, 0.091099149, 0.054318284, 0.035722055},
            {-0.140666849, -0.286571981, 0.338786716, 0.469819833, -0.004141945, 0.186022692, 0.063665055, -0.26919525, -0.121388869},
            {-0.261522851, 0.189000435, 0.344101123, -0.5255097, 0.115113173, 0.304487265, -0.021530241, 0.019668254, 0.212210873},
            {0.035222842, 0.091917274, 0.037516833, 0.317734036, 0.071786372, -0.181753087, 0.094598968, 0.13973836, 0.035774178},
            {-0.076825849, 0.363230329, -0.287769401, 0.134241702, -0.052193737, 0.048005088, -0.195512981, -0.050788547, 0.10258596},
            {0.15935377, -0.022670335, -0.565162333, 0.317158894, -0.33905077, -0.239912286, -0.146136006, 0.061591574, -0.063990096},
            {0.323685677, 0.311192824, -0.271025844, 0.111840963, -0.177365204, 0.165538508, 0.01830726, 0.28311171, -0.216818216},
            {0.121109852, -0.031911501, -0.065426344, 0.155156906, 0.266379778, 0.087345586, 0.200605945, 0.356032545, 0.024668},
            {-1.09192E-69, 2.05275E-64, -1.49361E-66, -1.92753E-65, -4.73187E-60, -9.20456E-70, 2.1248E-78, -1.72529E-59, -0.049417035},
            {0.023032875, -0.109023936, 0.097743827, 0.209730677, 0.250714701, 0.403346694, -0.146966216, -0.139482542, -0.070365277},
            {0.224753912, 0.232561382, -0.146762317, -0.010534058, 0.079645227, 0.189123499, -0.35005551, 0.494626858, -0.164187624},
            {-0.4803275, -0.150508462, -0.00885326, 0.015501423, 0.075939038, -0.401991354, 0.061097168, -0.193743757, 0.470089688},
            {-0.128408795, -0.037522101, -0.438972155, 0.061747471, 0.238478918, -0.290527064, -0.799043469, 0.204114212, -0.16309395},
            {0.166696696, 0.054928336, -0.157179228, -0.443331335, -0.074381931, -0.088356485, -0.230876493, 0.099178283, -0.045318245},
            {-0.087387148, -0.358661779, -0.029629464, 0.314719675, 0.264783324, 0.420643687, -0.090624388, 0.066000628, -0.068573781},
            {-0.430693879, -0.268308385, 0.211329061, -0.019146412, -0.03192772, 0.900460942, 0.136828276, -0.105882741, 0.148773909},
            {-0.133727533, 0.223299889, 0.064222552, 0.077660574, 0.187183589, 0.155791625, -0.202235419, -0.111471288, 0.18779116},
            {0.009034876, -0.062483032, 0.148117707, 0.025433226, 0.160419036, 0.959296879, 0.321337681, -0.100710997, -0.303073143},
            {0.004507724, 0.20291988, 0.294124957, 0.01227374, 0.077356898, -0.250752855, -0.257428089, -0.143117155, 0.141185137},
            {-1.92479E-76, -1.89971E-75, -1.91166E-61, -7.39472E-58, -1.52107E-67, 7.19003E-53, -1.61175E-68, -3.53911E-68, -0.245872381},
            {0.291711599, -0.275314802, 0.127574549, 0.097051643, 0.182416474, -0.150403961, 0.094249711, 0.370611387, 0.144264252},
            {-1.97834E-56, -9.54269E-66, -8.52076E-64, -3.5563E-60, 5.24009E-70, -1.38517E-56, -1.18518E-64, -1.03231E-57, -0.05000714},
            {-0.018072049, 0.055741382, -0.161673293, -0.087911177, -0.035658843, 0.142867602, -0.158844117, -0.167182988, -0.024822334},
            {-0.271660664, 0.086948336, 0.190648695, -0.277786542, -0.38498411, -0.581710186, -0.057057489, 0.323674787, 0.013129236},
            {0.12180342, 0.035170528, 0.032374843, -0.332150905, 0.259854665, 0.417290136, -0.073066593, 0.059497466, -0.037992859},
            {-0.066276101, -0.032163552, 0.060851582, 0.229107622, 0.313747004, -0.198627103, 0.323767748, 0.273616785, -0.109561546},
            {-0.048639887, 0.221793605, 0.082898151, 0.329565666, -0.048118395, -0.025976296, 0.079534601, -0.1753963, -0.04065355},
            {0.079513649, -0.060732804, -0.00684843, 0.277069628, 0.166579218, 0.610178942, 0.359632235, 0.577898575, 0.076589736},
            {-0.096145273, 0.111556127, 0.020169183, 0.211716122, 0.229897933, 0.004311198, -0.403833958, -0.004737735, 0.066992595},
            {-0.257423904, -0.172587204, 0.249738372, 0.305707925, 0.066389416, 0.161019072, 0.021861247, -0.129215264, -0.092728658},
            {0.05522926, -0.030383014, -0.378801167, -0.121276176, 0.435653227, 0.044089403, 0.141353002, 0.206664191, -0.314289861},
            {-0.298915731, 0.342411063, 0.338117159, -0.497034923, -0.163573716, 0.240349251, 0.199205896, 0.137114455, -0.139627014},
            {-0.132413956, 0.095994859, -0.059026891, -0.059001635, 0.252510632, -0.265472158, 0.350656279, 0.527844559, -0.003270792},
            {0.019256229, -0.000995542, 0.213482768, -0.157340695, 0.253029927, 0.058953203, -0.245256818, 0.355941859, -0.12015571},
            {0.119954649, -0.20416662, 0.300593869, -0.355267966, 0.213160769, -0.245678823, 0.087566011, 0.276159897, -0.081135205},
            {-3.11421E-56, 1.39778E-55, -2.2297E-61, -1.52466E-54, -8.08349E-59, -4.11986E-66, 5.15078E-60, 5.96883E-66, -0.226571054},
            {0.23761884, -0.030820834, 0.193158078, 0.044643597, 0.066965429, 0.330868897, 0.051145714, -0.056413822, -0.183071818},
            {-0.170896716, 0.129994126, 0.082549396, -0.349172539, 0.148715185, -0.042118843, -0.200088043, -0.22002794, 0.305937167},
            {0.34660769, -0.070994493, -0.121113568, 0.312962592, -0.440911796, -0.061786794, 0.326611391, -0.597067597, -0.367593001},
            {-0.022062195, 0.247023831, -0.083268002, -0.486026446, -0.310913132, 0.633565332, -0.476837011, -0.599853388, -0.035358256},
            {0.410717308, -0.414070415, -0.196199595, 0.026980516, -0.36364175, 0.067848741, 0.228327267, 0.078052138, 0.304603896},
            {0.213106433, 0.157185587, -0.330095603, -0.206579614, 0.236774327, 0.271007224, -0.183949847, 0.042811396, 0.083787929},
            {0.479317662, -0.558930947, -0.45799933, 0.262802677, -0.361782838, 0.21457363, 0.208346749, -0.017103511, 0.153853458},
            {-0.160915028, 0.134129443, -0.196586379, -0.236852418, 0.308204034, 0.541705159, -0.623373235, 0.630324373, -0.219028819},
            {-0.012833636, -0.009801407, -0.296965862, 0.100091249, -0.147128587, 0.601182027, -0.465640097, -0.867255534, 0.109710477},
            {-0.260663973, -0.137829795, -0.009044434, -0.163772258, -0.122924035, 0.545435057, -0.320127669, -0.994837001, 0.42112819},
            {-0.115345804, 0.054095887, -0.028291257, 0.085399985, -0.095959773, 0.181768491, 0.009382479, -0.145155672, -0.090816111},
            {-0.356827984, -0.703618509, 0.249854598, 0.397198525, -0.079416988, 0.307374138, 0.094039565, 0.497269291, 0.12436776}
        }

        _output_layer = {
            {0.929667122, 0.922457989, -0.885424998, 1.219977717, -0.674175261, 1.299591755, 0.419919964, 1.253283624, 0.23738464, -0.88234911, -0.62323756, -0.195116191, -1.887123576, -0.215098025, -0.20872055, 1.77E-66, 0.178647274, -0.296630166, -0.162391011, -1.990540397, -0.774836027, 0.59853383, -2.018922158, -0.456593844, 0.316811244, -1.199953932, 1.044600304, 0.336796151, 1.42369596, 0.160554708, -0.596842967, 0.257470855, -0.28102662, -0.506033014, -0.719497241, 0.222896146, -0.334164327, -1.716318359, -0.577472102, -0.138098286, 2.09E-57, -0.136356571, -0.229890536, -1.855508649, 1.011372506, 0.867000566, 0.636245329, -1.294238742, 0.278920338, 0.623276299, 0.150631092, -2.74E-62, -0.277589684, -1.7E-58, -0.449072397, -1.772053793, -0.291486866, -0.104943457, 0.290328813, 0.432186146, -0.186646722, -0.378842471, 1.082943945, 1.091586352, -0.470039773, -0.209737813, -0.431389354, -1.08E-54, -0.063172277, -0.238854853, 0.990437892, -1.361474389, 1.189280915, -0.456769424, 1.875324979, 0.849822429, -1.223920175, -1.721655834, -0.34158942, -1.537613773, -0.299539111}
        }
    End Sub
End Class

Public Class Compute_ML_FUMO
    Inherits Compute_ML

    Public Sub New(par As ML_Params)
        MyBase.New(par, New avg_std_values With {
                   .INDEX_AVG = 215691.91148347,
                   .INDEX_STD = 578880.42594047,
                   .DAY_AVG = 156.43555555556,
                   .DAY_STD = 16.0553333867201,
                   .KER_AVG = 20.4,
                   .KER_STD = 3.67570450001474
                   })

        _input_array(0) = par.FAO_class
        _input_array(1) = par.Preceding_Crop
        _input_array(2) = par.Sowing_Week
        _input_array(3) = par.Harvest_Week
        _input_array(4) = par.ECB_Severity

        _hidden_layer = {
            {0.0001984, -0.0000975602, -0.000619002, 0.00082271, 0.000600001, 0.000367115, -0.000524394, -0.000174012, -0.068627828},
            {-0.000589445, 0.01368647, 0.007472333, -0.001211749, 0.007831792, 0.00685243, 0.002074086, 0.009597125, 0.592336399},
            {0.05926816, -0.071537516, 0.107374507, 0.188571793, -0.063335455, 0.054781491, -0.011415007, -0.06889116, -0.422913779},
            {0.114988302, 0.108303484, 0.255024165, 0.314323348, -0.16263996, 0.193628523, -0.087432328, -0.122958978, -1.08428568},
            {0.000571269, 0.004503416, 0.003381525, 0.000927679, 0.004436553, -0.0000999376, 0.000709311, 0.00292245, 0.248482289},
            {-0.317440443, -0.231108959, 0.1774414, 0.643044518, -0.349712422, -0.580252454, 0.152657228, -0.363815379, -0.186378993},
            {0.003829319, -0.002592663, 0.010988106, 0.00504867, -0.00823684, 0.00987357, -0.002262932, 0.00079206, -0.042942042},
            {0.594289322, 0.167310053, -0.097335104, -0.215554622, -0.097206689, 0.475067078, -0.242104646, -0.371460086, -0.501961721},
            {0.682217275, -0.172214751, -0.613367578, -0.361452736, -0.398457106, -0.233810012, 0.254700204, 0.089158923, 0.715113827},
            {0.063721301, -0.61555116, 0.248192367, 0.164138974, -0.096399327, -0.148456078, -0.810465626, 0.374561383, 0.440188285},
            {0.063291446, -0.045398245, -0.048107483, -0.033409422, 0.062775598, 0.039399156, 0.021283753, -0.029203388, 0.077970214},
            {-0.026282056, -0.38226245, 0.272149912, 0.061476078, -0.484496189, -0.324945922, -0.350522804, -0.009123199, 0.097566853},
            {-0.04973625, 0.062385635, 0.023960806, 0.10082851, -0.041270862, -0.0000614798, -0.007395498, -0.014228715, -0.247521649},
            {-0.00668084, 0.001970935, -0.002003588, -0.007685694, -0.00038879, -0.003088417, -0.006422051, -0.000697833, -0.214010522},
            {0.000952285, 0.000262659, -0.000764312, -0.00068777, 0.000990764, 0.000632886, -0.000103892, -0.001073813, -0.203293999},
            {-0.000367942, -0.002375524, 0.000515041, -0.000393152, -0.001085639, -0.000566827, 0.000884415, -0.000362142, -0.148737046},
            {0.000584519, -0.000775811, -0.001497945, 0.00104812, 0.0005496, 0.000758655, 0.001963291, -0.00078029, -0.211515077},
            {0.158478257, 0.183128273, -0.014013989, -0.024947975, 0.222200919, 0.610666136, -0.211234069, -0.171226717, -0.571250558},
            {0.000511734, 0.016617175, 0.010092411, -0.007628289, 0.00494758, 0.004183771, -0.008725317, 0.005624762, 0.225371951},
            {0.327880893, -0.275836918, -0.209124837, -0.338423701, 0.148524084, -0.090470032, 0.00162439, -0.123825841, 0.969178768},
            {0.001507213, 0.007035658, 0.004662378, 0.000118114, 0.005361416, 0.000830446, 0.002038064, 0.0055517, 0.401354598},
            {-0.378281261, 0.58558598, -0.553665965, -0.423370218, 0.616837332, -0.603685527, -0.17285044, 0.339821701, 2.580767351},
            {-0.589635735, 0.144296607, 0.093934464, 0.238728285, 0.370231553, 0.237096186, -0.216976747, -0.15084506, 0.29332329},
            {-0.001863714, 0.010806283, -0.000251526, -0.004569432, 0.004243842, 0.002635866, -0.002425973, -0.001104477, 0.058580623},
            {-0.000360955, 0.002060528, 0.002144967, -0.001200225, 0.002250811, 0.0000351278, -0.001456918, 0.000819146, 0.036971439},
            {-0.000899976, 0.005788818, 0.001858255, -0.00043906, -0.0000127121, 0.00155711, -0.003342593, -0.000254713, 0.024067151},
            {0.000919819, 0.010506323, 0.008687905, -0.001180799, 0.007225054, 0.000730984, -0.000782829, 0.008027853, 0.451293169},
            {0.199155371, 0.105706831, -0.179680228, -0.182983681, -0.198578815, 0.047259237, 0.232958779, 0.11312404, 0.707701984},
            {0.0037986, 0.027489395, 0.020170661, -0.006740459, 0.01233396, 0.00836093, -0.008839781, 0.011575077, 0.773485159},
            {-0.016617442, 0.020663736, 0.01446405, 0.037029422, -0.015634914, -0.004211087, 0.002813305, -0.009107184, -0.107225768},
            {0.371132325, -0.105090719, -0.121689721, 0.09661776, -0.077531324, 0.198956164, 0.592691684, 0.251678945, -0.461681219},
            {-0.05433242, 0.001043081, 0.151655914, 0.294822039, -0.103990366, 0.111139386, -0.170047261, -0.049227826, -0.45442325},
            {-0.000293263, 0.000285851, 0.000352733, -0.000628997, -0.000337788, -0.000243567, 0.001169247, -0.000773202, -0.171961187},
            {0.106991947, 0.091857633, 0.270513341, -0.237844452, -0.651357947, -0.235923384, 0.07722214, -0.487079435, 0.498830942},
            {-0.083720077, -0.08739868, 0.011594104, 0.076721225, -0.07286774, -0.190920551, 0.016577806, 0.033544187, 0.11521766},
            {-0.000686767, 0.013752572, 0.008336237, -0.003947491, 0.002543983, 0.001208022, -0.002695932, 0.007317219, 0.312254597},
            {0.000675862, 0.002092725, 0.001634897, -0.00011482, 0.001124382, 0.000173288, -0.001455191, -0.001112504, -0.009638318},
            {0.381404145, -0.543475172, 0.403538806, 0.126058171, -0.330219143, 0.445527434, -0.623415295, 0.085612908, -1.228781942},
            {0.00186827, 0.018507212, 0.013714031, -0.004731907, 0.009219318, 0.004912529, -0.006858515, 0.007375793, 0.484318426},
            {-0.002874211, -0.002996242, -0.0000659968, 0.002382363, 0.00057882, -0.000697316, 0.001188155, 0.001264828, -0.016875138},
            {-0.388732942, 0.0565772, 0.329859175, 0.358419381, -0.289831164, -0.301997246, 0.005167469, -0.583050336, 0.521662611},
            {-0.076429736, 0.352432222, -0.249277493, -0.943772703, 0.409173511, -0.473226508, -0.115739993, 0.097924103, 0.605587044},
            {0.000616341, 0.004180801, 0.002988212, 0.001103991, 0.00338033, -0.000266643, 0.000688618, 0.003032293, 0.228680628},
            {-0.125655506, -0.093688532, 0.020517137, 0.096867887, -0.015603614, -0.161025504, 0.007082221, 0.035021823, 0.10617308},
            {0.02520014, 0.012385574, -0.008135132, 0.001753297, 0.005624022, -0.023796231, -0.00034178, 0.003256653, -0.065091168},
            {0.486593737, -0.05288094, -0.018902748, -0.688399056, -0.419515942, -0.87779078, 0.336308548, -0.205086345, 0.525815203},
            {-0.268853049, -0.314617915, -0.585349379, -0.506070467, 0.416979019, -0.462479051, 0.049558543, 0.003658208, 2.053026395},
            {-0.158306152, -0.238525913, 0.089550304, -0.131151897, -0.162468366, -0.234315285, -0.189298887, -0.026154685, 0.630956502},
            {0.438967798, -0.288815747, -0.116143161, -0.830240866, 0.065318973, -0.107678759, -0.520136707, -0.074783686, 1.412411887},
            {-0.369299426, -0.223114658, 1.050325522, -0.051243963, -0.44153313, 0.458313775, 0.490290228, 0.429410648, -0.432276823}
        }

        _output_layer = {
            {-0.000823258, 0.007508124, -0.237170205, -0.546825197, 0.003094444, -1.103989418, 0.01400242, -0.913115376, -1.150293698, 1.14565384, -0.098374282, -0.832636787, -0.140076493, 0.002151326, -0.0000696473, 0.001318465, 0.002461349, -0.74663445, -0.000289466, -0.595956158, 0.005402121, 1.480927361, -0.83182281, -0.00040084, -0.00025971, -0.000647877, 0.005117284, 0.461367798, 0.006579955, -0.049787917, 0.813374676, -0.385117806, 0.000730314, -0.932406835, -0.252091345, 0.002530539, -0.00077743, 1.142896562, 0.003857186, 0.002046884, -0.956220087, -1.206076188, 0.002864207, -0.247647897, 0.029318395, -1.350989738, -1.16428745, -0.495947683, -1.118941246, 1.457364218, 1.011003783}
        }
    End Sub
End Class