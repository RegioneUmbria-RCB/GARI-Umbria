Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelliPrevisionaliBIZ
Imports AgronicaCoreVarieBIZ

Public Class Agronomica30_BatteriosiKiwi_PSA
    Inherits AbstractModello

    Private Class DatoHH
        Public DataOra As DateTime
        Public Temp As Decimal
        Public Prec As Decimal
        Public UR As Decimal
        Public Bagn As Boolean
        Public M_Orario As Decimal
        Public Risk_Index As Decimal
    End Class

    Private _datiHH As List(Of DatoHH)

    Public Sub New(datiMeteo As MeteoReadOnlyList)
        MyBase.New(datiMeteo)

        _datiHH = New List(Of DatoHH)
    End Sub


    Protected Overrides Function _elaboraModello() As cRisultatoModello

        If Not CalcolaPSA() Then

            Return Nothing
        End If

        Dim output As New OutputModello
        output.aggiungiColonna("DataOra", GetType(DateTime), Gias.DataOra, "dd/MM/yyyy HH\""h\""")
        output.aggiungiColonna("Temperatura", GetType(Decimal), Gias.Temperatura & " (°C)", "0.00")
        output.aggiungiColonna("Precipitazione", GetType(Decimal), Gias.Pioggia & " (mm)", "0.00")
        output.aggiungiColonna("UmiditaRelativa", GetType(Decimal), Gias.UmiditaRelativa & " (%)", "0.00")
        output.aggiungiColonna("Bagnatura", GetType(Boolean), Gias.BagnaturaFogliare & " (" & Gias.Si & "/" & Gias.No & ")", Gias.Si & "|" & Gias.No)
        output.aggiungiColonna("M_Orario", GetType(Decimal), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_BatteriosiKiwi_PSA_mOrario, "0.00")
        output.aggiungiColonna("Risk_Index", GetType(Decimal), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_BatteriosiKiwi_PSA_indiceDiRischioSommaMobileOre, "0.0", True)
        Dim indicator = output.aggiungiIndicatore("Risk_Index_IND", {20, 40, Decimal.MaxValue}, OutputIndicator.enum_IndicatorType.Colors).setFieldVal("Risk_Index")

        For Each dhh In _datiHH

            output.AddField(dhh.DataOra)
            output.AddField(dhh.Temp)
            output.AddField(dhh.Prec)
            output.AddField(dhh.UR)
            output.AddField(dhh.Bagn)
            output.AddField(dhh.M_Orario)
            If dhh.Risk_Index >= 0 Then
                output.AddField(dhh.Risk_Index)
                output.AddField(indicator.colorForVal(dhh.Risk_Index))
            End If

            output.Commit()
        Next

        Return New cRisultatoModello With {.Modello_Tabella1 = output.Output({indicator}.ToList(), indicator.PlotBands())}
    End Function

    Protected Overrides Function _elaboraIndicatore_vecchio(ByVal dataInizio As DateTime, ByVal dataFine As DateTime) As cRisultatoModelloIndicatori.Indicatore

        Dim risIndic As New cRisultatoModelloIndicatori.Indicatore("PSA", dataInizio, dataFine)

        If CalcolaPSA() Then

            risIndic.Fill(_datiHH.Last().Risk_Index, 80, {20, 40}, _datiHH.Last().DataOra, dataFine)
        Else

            risIndic.Status = cRisultatoModelloIndicatori.Indicatore.enum_Status.Status_Error
            risIndic.StatusMsg = _errore
        End If

        Return risIndic
    End Function

    Protected Overrides Function _elaboraIndicatore(ByVal risElab As RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione) As RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione

        If CalcolaPSA() Then

            risElab.Fill(_datiHH.Last().Risk_Index, 80, {20, 40})
        Else

            risElab.Errore(_errore)
        End If

        Return risElab
    End Function


    'Public Function PSA(ByVal datiMeteo As List(Of AgronicaCoreMeteoBiz.InterfacciaMeteoNT.MeteoDSS)) As rispostaStandard(Of cRisultatoModello)

    '    Dim r As New rispostaStandard(Of cRisultatoModello)
    '    r.RispostaStringa = New cRisultatoModello()
    '    r.RispostaOK = False
    '    r.Errore = ""

    '    If CalcolaPSA(datiMeteo) Then

    '        r.RispostaOK = True

    '        Dim output As New OutputModello
    '        output.aggiungiColonna("DataOra", GetType(DateTime), Gias.DataOra, "dd/MM/yyyy HH\""h\""")
    '        output.aggiungiColonna("Temperatura", GetType(Decimal), Gias.Temperatura & " (°C)", "0.00")
    '        output.aggiungiColonna("Precipitazione", GetType(Decimal), Gias.Pioggia & " (mm)", "0.00")
    '        output.aggiungiColonna("UmiditaRelativa", GetType(Decimal), Gias.UmiditaRelativa & " (%)", "0.00")
    '        output.aggiungiColonna("Bagnatura", GetType(Boolean), Gias.BagnaturaFogliare & " (" & Gias.Si & "/" & Gias.No & ")", Gias.Si & "|" & Gias.No)
    '        output.aggiungiColonna("M_Orario", GetType(Decimal), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_BatteriosiKiwi_PSA_mOrario, "0.00")
    '        output.aggiungiColonna("Risk_Index", GetType(Decimal), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_BatteriosiKiwi_PSA_indiceDiRischioSommaMobileOre, "0.0", True)
    '        Dim indicator = output.aggiungiIndicatore("Risk_Index_IND", {20, 40, Decimal.MaxValue}, OutputIndicator.enum_IndicatorType.Colors).setFieldVal("Risk_Index")

    '        For Each dhh In _datiHH

    '            output.AddField(dhh.DataOra)
    '            output.AddField(dhh.Temp)
    '            output.AddField(dhh.Prec)
    '            output.AddField(dhh.UR)
    '            output.AddField(dhh.Bagn)
    '            output.AddField(dhh.M_Orario)
    '            If dhh.Risk_Index >= 0 Then
    '                output.AddField(dhh.Risk_Index)
    '                output.AddField(indicator.colorForVal(dhh.Risk_Index))
    '            End If

    '            output.Commit()

    '        Next

    '        r.RispostaStringa.Modello_Tabella1 = output.Output({indicator}.ToList(), indicator.PlotBands())

    '    Else

    '        r.Errore = _errore

    '    End If

    '    Return r

    'End Function

    'Public Function PSA_Indicatore(ByVal datiMeteo As List(Of AgronicaCoreMeteoBiz.InterfacciaMeteoNT.MeteoDSS), ByVal dataInizio As DateTime, ByVal dataFine As DateTime) As rispostaStandard(Of cRisultatoModelloIndicatori.Indicatore)

    '    Dim r As New rispostaStandard(Of cRisultatoModelloIndicatori.Indicatore)
    '    r.RispostaStringa = New cRisultatoModelloIndicatori.Indicatore("PSA", dataInizio, dataFine)
    '    r.RispostaOK = False
    '    r.Errore = ""

    '    If CalcolaPSA(datiMeteo) Then

    '        r.RispostaOK = True
    '        r.RispostaStringa.Fill(_datiHH.Last().Risk_Index, 80, {20, 40}, _datiHH.Last().DataOra, dataFine)

    '    Else

    '        r.Errore = _errore
    '        r.RispostaStringa.Status = cRisultatoModelloIndicatori.Indicatore.enum_Status.Status_Error
    '        r.RispostaStringa.StatusMsg = _errore

    '    End If

    '    Return r

    'End Function

    Private Function CalcolaPSA() As Boolean

        Dim result As Boolean = True

        Try

            Dim riskIndex72 As New Queue(Of Decimal)

            Dim d_hh As DatoHH

            For Each d In _datiMeteo

                d_hh = New DatoHH With {
                    .DataOra = d.DataOra,
                    .Temp = d.Temp,
                    .Prec = d.Prec,
                    .UR = d.UmRel,
                    .Bagn = d.BagnEffettiva(80) > 0,
                    .M_Orario = 0
                }

                If d_hh.Bagn Then
                    d_hh.M_Orario = 0.247
                    Dim t As Decimal = d.Temp
                    For Each c In {0.0541D, 0.00201D, -0.00011D, -0.000003D}
                        d_hh.M_Orario += c * t
                        t *= d.Temp
                    Next
                End If

                riskIndex72.Enqueue(d_hh.M_Orario)
                If riskIndex72.Count() > 72 Then
                    riskIndex72.Dequeue()
                End If
                If riskIndex72.Count() = 72 Then
                    d_hh.Risk_Index = riskIndex72.Sum()
                Else
                    d_hh.Risk_Index = -1
                End If

                _datiHH.Add(d_hh)

            Next

        Catch ex As Exception

            result = False

            'uso questa funzione per ottenere il Messaggio..:
            _errore = Gias.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return result
    End Function

End Class
