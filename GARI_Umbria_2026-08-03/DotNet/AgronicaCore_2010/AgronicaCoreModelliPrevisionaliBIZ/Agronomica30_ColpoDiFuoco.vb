
Imports System.Web.Script.Serialization
Imports AgronicaCoreDataProvider.My.Resources
Imports Newtonsoft.Json.Linq

Public Class Agronomica30_ColpoDiFuoco
    Inherits AbstractModello

    Private Class CalcHelper
        Private ReadOnly _q As Queue(Of Decimal)
        Private ReadOnly _max_hh As Integer
        Public ReadOnly Property CumH As Integer
            Get
                Return _max_hh
            End Get
        End Property
        Public Sub New(ByVal gg_cum As Integer)
            _max_hh = gg_cum * 24
            _q = New Queue(Of Decimal)
        End Sub
        Public Function AddAndSum(ByVal temp As Decimal) As Decimal

            Dim cum As Decimal = 0

            _q.Enqueue(_computeTRV(temp))

            If _q.Count > _max_hh Then

                _q.Dequeue()
                cum = _q.Sum()
            End If

            Return cum
        End Function

        Private Function _computeTRV(ByVal t As Decimal) As Decimal

            'TMP_ore_down = floor(TMP_ore * 2) / 2;    
            'if(TMP_ore_down <= 10 || TMP_ore_down >= 35.5)
            '        TRV = 0;
            'else
            '    ind_TRV = find(TMP_ore_down == TRV_table(: , 1));
            '    TRV = TRV_table(ind_TRV , 2);
            'end

            Dim tbl As Decimal() = {0,
            0.05, 0.1, 0.15, 0.2, 0.22, 0.25, 0.3, 0.35, 0.4, 0.45,
            0.5, 0.6, 0.7, 0.9, 1.0, 1.12, 1.25, 1.6, 2.1, 2.7,
            3.2, 3.9, 4.75, 5.6, 7.0, 8.9, 11.5, 14.7, 17.1, 20.3,
            23.0, 26.0, 29.0, 32.0, 34.5, 37.5, 40.5, 44.0, 46.5, 48.2,
            50.0, 51.0, 52.0, 52.0, 51.0, 50.0, 45.0, 35.0, 20.0, 10.0,
            0}

            Dim t_05 As Decimal = Math.Floor(t * 2D) / 2D

            Dim i_tbl As Integer = Math.Min(Math.Max(0, (t_05 - 10D) * 2D), tbl.Length - 1)

            Return tbl(i_tbl)
        End Function
    End Class

    Private Class DatoOrario
        Public DataOra As DateTime
        Public Temp As Decimal
        Public Pioggia As Decimal
        Public TRV As Decimal
    End Class

    Private ReadOnly _calcHelper As CalcHelper
    Private _datiOrari As List(Of DatoOrario)
    Private ReadOnly _dtEmergenza As Date

    Public Sub New(ByVal datiMeteo As MeteoReadOnlyList, ByVal Veg_Cod As Integer)
        MyBase.New(datiMeteo)

        Select Case Veg_Cod
            Case 40 'Melo
                _calcHelper = New CalcHelper(4)
            Case 48 'Pero
                _calcHelper = New CalcHelper(5)
            Case Else
                _calcHelper = Nothing
        End Select

        _datiOrari = Nothing

        'Dim lettore As New LettoreParametri("")

        Dim dtEmergenza As Date = Date.MinValue 'lettore.DateOrDefault("", Date.MinValue)
        Dim dtEmergenza_gg As Integer = 0 'lettore.IntOrDefault("", 0)

        _dtEmergenza = _data_from_doy(dtEmergenza, dtEmergenza_gg, 3, 15) '15 Marzo
    End Sub

    Protected Overrides Function _elaboraModello() As cRisultatoModello

        If Not Calcola() Then

            Return Nothing
        End If

        'Dim risModello As New cRisultatoModello

        'If mDataSopraSoglia_12 > Date.MinValue Then

        '    Dim msg As String = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_Pom_MISP_IPI_dataInizioRischioIPISopraSoglia_ & mDataSopraSoglia_12.ToShortDateString()

        '    risModello.Modello_WarningMsg = "<div style='text-align: center; font-size: larger;'>" & msg & "</div>"
        'End If

        'risModello.Modello_Tabella1 = Output_IPI()
        'risModello.Modello_Tabella2 = Output_MISP()

        'Return risModello


        Dim output As New OutputModello
        output.aggiungiColonna("DataOra", GetType(DateTime), Gias.DataOra, "dd/MM/yyyy HH\""h\""")
        output.aggiungiColonna("Temp", GetType(Decimal), Gias.Temperatura & " (°C)", "0.00")
        output.aggiungiColonna("Prec", GetType(Decimal), Gias.Pioggia & " (mm)", "0.00")
        output.aggiungiColonna("TRV_" + _calcHelper.CumH.ToString, GetType(Decimal), "Indice di rischio TRV", "0.00")
        output.aggiungiColonna("Rischio1", GetType(Decimal), "Rischio caso 1", "0.00")
        Dim indic1 = output.aggiungiIndicatore("Rischio1_IND", {400, 800, Decimal.MaxValue}, OutputIndicator.enum_IndicatorType.Colors).setFieldVal("Rischio1").setFieldHidden(True)
        output.aggiungiColonna("Rischio2", GetType(Decimal), "Rischio caso 2", "0.00")
        Dim indic2 = output.aggiungiIndicatore("Rischio2_IND", {150, 350, Decimal.MaxValue}, OutputIndicator.enum_IndicatorType.Colors).setFieldVal("Rischio2").setFieldHidden(True)
        output.aggiungiColonna("Rischio3", GetType(Decimal), "Rischio caso 3", "0.00")
        Dim indic3 = output.aggiungiIndicatore("Rischio3_IND", {80, 200, Decimal.MaxValue}, OutputIndicator.enum_IndicatorType.Colors).setFieldVal("Rischio3").setFieldHidden(True)

        For Each d_o In _datiOrari

            output.AddField(d_o.DataOra)
            output.AddField(d_o.Temp)
            output.AddField(d_o.Pioggia)
            output.AddField(d_o.TRV)
            output.AddField(d_o.TRV)
            output.AddField(indic1.colorForVal(d_o.TRV))
            output.AddField(d_o.TRV)
            output.AddField(indic2.colorForVal(d_o.TRV))
            output.AddField(d_o.TRV)
            output.AddField(indic3.colorForVal(d_o.TRV))

            output.Commit()
        Next

        Dim tbl As JObject = JObject.Parse(output.Output({indic1, indic2, indic3}.ToList()))

        Dim jss = New JavaScriptSerializer()
        Dim obj_pb1 As JObject = JObject.Parse(jss.Serialize(indic1.PlotBands()))
        Dim obj_pb2 As JObject = JObject.Parse(jss.Serialize(indic2.PlotBands()))
        Dim obj_pb3 As JObject = JObject.Parse(jss.Serialize(indic3.PlotBands()))
        tbl.Add("plotBands", New JArray From {obj_pb1, obj_pb2, obj_pb3})

        Return New cRisultatoModello With {.Modello_Tabella1 = tbl.ToString()}
    End Function

    Protected Overrides Function _elaboraIndicatore_vecchio(dataInizio As Date, dataFine As Date) As cRisultatoModelloIndicatori.Indicatore
        Throw New NotImplementedException()
    End Function

    Protected Overrides Function _elaboraIndicatore(risElab As RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione) As RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione

        If Calcola() Then

            risElab.Fill(_datiOrari.Last().TRV, 350, {80, 200})
            'Dim indic1 = output.aggiungiIndicatore("Rischio1_IND", {400, 800, Decimal.MaxValue}, OutputIndicator.enum_IndicatorType.Colors).setFieldVal("Rischio1").setFieldHidden(True)
            'Dim indic2 = output.aggiungiIndicatore("Rischio2_IND", {150, 350, Decimal.MaxValue}, OutputIndicator.enum_IndicatorType.Colors).setFieldVal("Rischio2").setFieldHidden(True)
            'Dim indic3 = output.aggiungiIndicatore("Rischio3_IND", {80, 200, Decimal.MaxValue}, OutputIndicator.enum_IndicatorType.Colors).setFieldVal("Rischio3").setFieldHidden(True)
        Else

            risElab.Errore(_errore)
        End If

        Return risElab
    End Function

    Private Function Calcola() As Boolean
        Dim result As Boolean = True

        If _calcHelper Is Nothing Then

            _errore = "Modello non disponibile"
            Return False
        End If

        Try
            _datiOrari = New List(Of DatoOrario)

            Dim doyFioriAperti = _dtEmergenza.Date.DayOfYear

            Dim idxCalc As Integer = 0
            'N.B.: Inizio a calcolare dalla data fiori aperti (15 marzo)
            While idxCalc < _datiMeteo.Count AndAlso _datiMeteo(idxCalc).DataOra.DayOfYear < doyFioriAperti
                idxCalc += 1
            End While

            While idxCalc < _datiMeteo.Count

                Dim dm = _datiMeteo(idxCalc)

                Dim d_o As New DatoOrario With {
                    .DataOra = dm.DataOra,
                    .Temp = dm.Temp,
                    .Pioggia = dm.Prec,
                    .TRV = _calcHelper.AddAndSum(dm.Temp)
                }

                _datiOrari.Add(d_o)

                idxCalc += 1
            End While

            If Not _datiOrari.Any() Then

                result = False
                _errore = "NO fiori aperti" 'My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_Pom_MISP_IPI_nonRaggiuntaDataEmergenzaOTrapiantoDellaColtura
            End If

        Catch ex As Exception

            result = False

            'uso questa funzione per ottenere il Messaggio..:
            _errore = Gias.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return result
    End Function
End Class

