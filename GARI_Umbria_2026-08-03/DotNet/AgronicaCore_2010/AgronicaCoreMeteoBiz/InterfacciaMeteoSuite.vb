Imports System.Web
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreMeteoDAL
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreWebService.MeteoNT
Imports InData.Engine.MeteoSuite
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports OutData.Engine.MeteoSuite

Public Class InterfacciaMeteoSuite

#Region "Classi Private"

    Public Class SorgenteMeteo
        Public sorgente_cod As Integer
        Public sorgente_des As String
    End Class

    Public Class LeggiStazioneOut
        Public Id As Integer
        Public Stazione As String
        Public Lat As Double?
        Public Lng As Double?
        Public FlagReale As Boolean
        Public RifFornitore As String
        Public Fornitore As String

        Public Class Sensore
            Public Id As Integer
            Public Sensore As String
            Public Tipo As String
            Public UM As String
            Public Id_StazioneOrigine As Integer?
            Public StazioneOrigine As String
            Public Last_Update As DateTime?
        End Class

        Public Sensori As List(Of Sensore)
    End Class

    Private Class MeteoRiepilogo
        Public Shared Function Output(ByVal dtRiepilogo As List(Of MeteoDataRiepilogoDato)) As JArray
            Dim riepilogo_periodo As New JArray

            If dtRiepilogo IsNot Nothing Then

                For Each rr In dtRiepilogo

                    Dim oriep As New JObject
                    oriep("Sensore") = rr.Sensore
                    Dim aggreg As String = rr.FunAggreg
                    Select Case aggreg
                        Case "avg"
                            aggreg = Gias.Media
                        Case "min"
                            aggreg = Gias.Minimo
                        Case "max"
                            aggreg = Gias.Massimo
                        Case "sum"
                            aggreg = Gias.Somma
                        Case "t_sum"
                            aggreg = Gias.SommaTermica
                        Case "t_cnt_freddo"
                            aggreg = Gias.CumuloFabbisognoFreddo
                    End Select
                    oriep("Aggreg") = aggreg '& " " & Gias.Periodo.ToLower()
                    oriep("Valore") = If(Not IsDBNull(rr.Valore), CDec(rr.Valore), Nothing)
                    oriep("UM") = rr.UM
                    riepilogo_periodo.Add(oriep)

                Next
            End If

            Return riepilogo_periodo
        End Function
    End Class

    Private Class MeteoOutput
        Public Dati As List(Of MeteoDataPuntuale)
        Public Sensori As List(Of MeteoDataSensore)
        Public Charts As JArray
        Public Riepilogo As List(Of MeteoDataRiepilogoDato)
        Public RiepilogoSensori As List(Of MeteoDataRiepilogoDatoSensore)
    End Class

    Private Class Periodo
        Public DataInizio As DateTime
        Public DataFine As DateTime
        Public Output As MeteoOutput
    End Class

    Private Class OutputListaPeriodi

        'Private AnnoInizio As Integer
        Private Periodi As List(Of Periodo)
        Private OutMeteo As OutputMeteo
        Private ObjCharts As JArray

        Private Sub GeneraModello()

            OutMeteo.aggiungiColonna("Data", GetType(Date), Gias.Data, "dd/MM")

            Dim DTDati As New DataTable
            DTDati.Columns.Add(New DataColumn("Mese", GetType(Integer)))
            DTDati.Columns.Add(New DataColumn("Giorno", GetType(Integer)))

            Dim s_anno As String
            Dim coltitle As String

            For Each p In Periodi
                s_anno = p.DataInizio.Year

                For Each s In p.Output.Sensori
                    coltitle = s.Etichetta
                    If Not String.IsNullOrEmpty(s.UM) Then
                        coltitle &= " (" & s.UM & ")"
                    End If

                    OutMeteo.aggiungiColonna(s.Sensore + "_" & s_anno, GetType(Decimal), coltitle, "0.00")._gruppoColonne = s_anno

                    DTDati.Columns.Add(New DataColumn(s.Sensore & "_" & s_anno, GetType(Decimal)))
                Next
            Next

            Dim dt As DateTime
            Dim m, g As Integer
            For Each p In Periodi
                s_anno = p.DataInizio.Year

                While p.Output.Dati.Count > 0
                    Dim src = p.Output.Dati(0)
                    dt = src.DataOra
                    m = dt.Month
                    g = dt.Day

                    Dim dst_row = DTDati.Select("Mese = " & m & " AND Giorno = " & g).ToList.FirstOrDefault
                    If dst_row Is Nothing Then
                        dst_row = DTDati.NewRow()
                        dst_row("Mese") = m
                        dst_row("Giorno") = g
                        DTDati.Rows.Add(dst_row)
                    End If

                    For Each sr In p.Output.Sensori
                        dst_row(sr.Sensore & "_" & s_anno) = src.ValoriSensori.Where(Function(x) x.Sensore.Equals(sr.ProviderReference)).Select(Function(x) x.Valore).FirstOrDefault()
                    Next

                    p.Output.Dati.RemoveAt(0)
                End While
            Next

            DTDati.DefaultView.Sort = "Mese ASC, Giorno ASC"
            DTDati = DTDati.DefaultView.ToTable()

            For Each d In DTDati.Rows

                m = d("Mese")
                g = d("Giorno")

                If m <> 2 Or g <> 29 Then

                    OutMeteo.AddField(New Date(1999, m, g))

                    For Each p In Periodi

                        s_anno = p.DataInizio.Year

                        For Each sr In p.Output.Sensori

                            OutMeteo.AddField(d(sr.Sensore & "_" & s_anno))

                        Next
                    Next

                    OutMeteo.Commit()

                End If

            Next

        End Sub

        Private Function GeneraSerie(ByVal s_src As JObject, ByVal anno As String) As JObject

            Dim s_dst = s_src.DeepClone()

            If s_src("field") IsNot Nothing Then
                s_dst("field") = s_src("field").ToString & "_" & anno
            End If
            If s_src("fromField") IsNot Nothing Then
                s_dst("fromField") = s_src("fromField").ToString & "_" & anno
            End If
            If s_src("toField") IsNot Nothing Then
                s_dst("toField") = s_src("toField").ToString & "_" & anno
            End If

            s_dst("name") = s_src("name").ToString & " [" & anno & "]"

            Return s_dst
        End Function

        Private Sub GeneraChart(ByVal src_axis As JArray, ByVal src_series As JArray)

            Dim title As String = ""
            For Each p In Periodi
                If Not String.IsNullOrEmpty(title) Then
                    title &= " - "
                End If
                title &= p.DataInizio.Year.ToString
            Next

            Dim horizAxis As New JObject()
            horizAxis("field") = "Data"
            horizAxis("title") = title

            If src_axis.Count > 1 Then

                For Each a In src_axis

                    Dim newChart As New JObject
                    Dim newSeries As New JArray

                    For Each p In Periodi

                        Dim s_anno As String = p.DataInizio.Year

                        For Each s In src_series

                            If s("axis").ToString = a("name").ToString Then

                                Dim s1 = GeneraSerie(s, s_anno)

                                s1("axis").Parent.Remove()

                                newSeries.Add(s1)

                            End If

                        Next

                    Next

                    If newSeries.Count > 0 Then

                        a("name").Parent.Remove()

                        newChart("horizAxis") = horizAxis
                        newChart("series") = newSeries
                        newChart("axis") = a

                        ObjCharts.Add(newChart)

                    End If

                Next

            Else

                Dim newSeries As New JArray

                For Each p In Periodi

                    Dim s_anno As String = p.DataInizio.Year

                    For Each s In src_series

                        newSeries.Add(GeneraSerie(s, s_anno))

                    Next

                Next

                Dim newChart As New JObject

                newChart("horizAxis") = horizAxis
                newChart("series") = newSeries
                newChart("axis") = src_axis

                ObjCharts.Add(newChart)

            End If

        End Sub

        Public Sub New(ByVal l_of_p As List(Of Periodo))

            'AnnoInizio = l_of_p(0).DataInizio.Year

            Periodi = New List(Of Periodo)

            For Each p In l_of_p

                If p.Output IsNot Nothing Then

                    Periodi.Add(p)

                End If

            Next

            OutMeteo = New OutputMeteo
            ObjCharts = New JArray

        End Sub

        Public Function OutputModello() As String
            Return OutMeteo.Output()
        End Function
        Public Function OutputCharts() As String
            Return ObjCharts.ToString()
        End Function
        Public Function OutputRiepilogo() As String

            Dim riepilogo As New JArray
            Dim flagPrimo As Boolean = True

            For Each per In Periodi

                Dim anno As String = per.DataInizio.Year.ToString()

                Dim oRiep = MeteoRiepilogo.Output(per.Output.Riepilogo)

                For Each elem In oRiep

                    Dim found As Boolean = False

                    If Not flagPrimo Then

                        Dim idx As Integer = 0
                        While Not found AndAlso idx < riepilogo.Count

                            Dim out_elem = riepilogo(idx)
                            If out_elem("Sensore").ToString = elem("Sensore").ToString _
                                AndAlso out_elem("Aggreg").ToString = elem("Aggreg").ToString _
                                AndAlso out_elem("UM").ToString = elem("UM").ToString Then

                                out_elem("Valore_" & anno) = elem("Valore")
                                found = True
                            End If

                            idx += 1
                        End While
                    End If

                    If Not found Then
                        Dim new_elem As New JObject
                        For Each prop As JProperty In elem.Children

                            Dim old_prop As String = prop.Name
                            Dim new_prop As String = old_prop
                            If new_prop = "Valore" Then
                                new_prop = "Valore_" & anno
                            End If
                            new_elem(new_prop) = elem(old_prop)
                        Next

                        riepilogo.Add(new_elem)
                    End If
                Next

                flagPrimo = False
            Next

            Return riepilogo.ToString()
        End Function

        Public Function GeneraOutput() As Boolean

            If Periodi.Count = 0 Then
                Return False
            End If
            'i18n
            GeneraModello()

            Dim charts = Periodi(0).Output.Charts

            For Each ch In charts

                Dim axis As JArray = ch("axis")
                Dim series As JArray = ch("series")

                GeneraChart(axis, series)

            Next

            Return True
        End Function
    End Class

    Private Class RisultatoMeteoRiepilogoStazione
        Public Property Descrizione As String
        Public Property UltimoAggiornamento As String
        Public Property Meteo As RisultatoMeteo
    End Class

    Private Class RisultatoMeteoMonitoraggioSuoloStazione
        Public Property Descrizione As String
        Public Property SogliaInf As Decimal
        Public Property SogliaSup As Decimal
        Public Property AlertSerie As String
        Public Property Meteo As RisultatoMeteo
    End Class

    Private Class alert_item
        Public Data As Date
        Public minValue As Decimal
        Public maxValue As Decimal
        Public Function alertObj(ByVal soglia_inf As Decimal, ByVal soglia_sup As Decimal) As JObject

            Dim value As Integer = 0
            Dim color As String = "#4CAF50"

            If minValue > maxValue Then
                ' per la data non ci sono valori per nessun sensore
                value = -1
                color = "#D3D3D3"

            Else

                If minValue < soglia_inf Then
                    value = 2
                    color = "#F44836"
                Else
                    If maxValue > soglia_sup Then
                        value = 1
                        color = "#FDD835"
                    End If
                End If
            End If

            Return New JObject(New JProperty("data", Data), New JProperty("value", value), New JProperty("color", color))
        End Function
    End Class

    Private Class PioggiaGG
        Public DataOra As DateTime
        Public Prec As Decimal
        Public Temp As Decimal
        Public TempMin As Decimal
        Public TempMax As Decimal
        Public UmRel As Decimal
    End Class


    Private Class SerieData
        Public ReadOnly Gruppo As Integer
        Public ReadOnly IsDerived As Boolean
        Public ReadOnly TipoSensore As String
        Public ReadOnly IdSensore As Integer
        Private _nomeSensore As String
        Private ReadOnly _nomeSensore_Postfix As String
        'Private ReadOnly _misura As String
        Private ReadOnly _um As String
        Private ReadOnly _fieldKey As String
        Private ReadOnly _serieType As String
        Private _serieColor As String
        Private ReadOnly _serieOpacity As Decimal
        Private ReadOnly _funAggreg As String
        Private ReadOnly _serieFields As JObject

        Private ReadOnly _derived As List(Of SerieData)

        Public Sub New(r As MeteoDataSensore)

            _fieldKey = "field"
            Dim fieldValue As String = r.Sensore
            'Dim tmparr As String() = fieldValue.Split("_")

            Gruppo = -1
            IsDerived = fieldValue.StartsWith("SD")
            TipoSensore = r.TipoSensore.ToString
            'IdSensore = CInt(tmparr(tmparr.Length - 1))
            IdSensore = r.SensoreId
            _nomeSensore = r.Etichetta
            '_misura = r("Misura").ToString
            _um = r.UM
            _funAggreg = r.FunAggreg

            _serieType = "line"
            _serieColor = ""
            _serieOpacity = 1
            _nomeSensore_Postfix = ""

            'Dim strConfig As String = r("Configurazione").ToString
            'If Not String.IsNullOrEmpty(strConfig) Then

            '    Dim objConfig = JObject.Parse(strConfig)

            '    If objConfig("gruppo") IsNot Nothing Then
            '        Gruppo = CInt(objConfig("gruppo"))
            '    End If

            '    If objConfig("serie") IsNot Nothing Then
            '        _serieType = objConfig("serie").ToString
            '    End If

            '    If objConfig("colore") IsNot Nothing Then
            '        _serieColor = objConfig("colore")
            '    End If

            '    If objConfig("opacity") IsNot Nothing Then
            '        _serieOpacity = Convert.ToDecimal(objConfig("opacity"), CultureInfo.InvariantCulture)
            '    End If

            '    If objConfig("sensor_postfix") IsNot Nothing Then
            '        _nomeSensore_Postfix = objConfig("sensor_postfix").ToString
            '    End If

            '    If objConfig("fieldProp") IsNot Nothing Then
            '        _fieldKey = objConfig("fieldProp").ToString
            '    End If
            'End If

            _serieFields = New JObject(New JProperty(_fieldKey, fieldValue))
            _derived = New List(Of SerieData)

        End Sub

        Public Sub AddDerived(sdata As SerieData)

            'verifico che non ci sia gia una serie derivata con lo stesso tipo, ma con FieldKey diverso
            Dim ider As Integer = 0
            Dim merged As Boolean = False
            While Not merged And ider < _derived.Count

                Dim der = _derived(ider)

                If String.Compare(der._serieType, sdata._serieType, True) = 0 Then

                    If String.Compare(der._fieldKey, sdata._fieldKey, False) <> 0 Then

                        der._serieFields.Add(sdata._fieldKey, sdata._serieFields(sdata._fieldKey))
                        merged = True
                    End If
                End If

                ider += 1
            End While

            If Not merged Then

                If Not String.IsNullOrEmpty(sdata._nomeSensore_Postfix) Then
                    sdata._nomeSensore = _nomeSensore + sdata._nomeSensore_Postfix
                End If

                _derived.Add(sdata)
            End If
        End Sub

        Public Function IsSimilar(other As SerieData) As Boolean

            If String.Compare(_serieType, other._serieType, True) <> 0 Then
                Return False
            End If

            If String.IsNullOrEmpty(_serieColor) OrElse String.IsNullOrEmpty(other._serieColor) Then
                Return False
            End If

            Return String.Compare(_serieColor, other._serieColor, True) = 0
        End Function

        Public Sub FillList(list As List(Of JObject), forceColor As String, axisDict As Dictionary(Of String, String))

            If Not String.IsNullOrEmpty(forceColor) Then
                _serieColor = forceColor
            End If

            list.Add(_getSerie(axisDict))

            For Each der In _derived
                If String.IsNullOrEmpty(der._serieColor) Then
                    der._serieColor = _serieColor
                End If

                list.Add(der._getSerie(axisDict))
            Next
        End Sub

        Private Function _getSerie(axisDict As Dictionary(Of String, String)) As JObject

            Dim axis_name As String = "T_" & TipoSensore.ToString.PadLeft(3, "0")
            Dim axis_title As String = _nomeSensore

            'If Not String.IsNullOrEmpty(_misura) Then
            '    axis_name = "M_" & _misura.Replace(" ", "_")
            '    axis_title = _misura
            'End If

            Dim u_m As String = ""
            If Not String.IsNullOrEmpty(_um) Then
                u_m = " (" & _um & ")"
            End If

            If Not axisDict.ContainsKey(axis_name) Then
                axisDict.Add(axis_name, axis_title & u_m)
            End If

            Dim serie = New JObject(
                New JProperty("name", _nomeSensore & u_m),
                New JProperty("type", _serieType),
                New JProperty("tipo_sensore", TipoSensore),
                New JProperty("FunAggreg", _funAggreg)
                )

            serie.Merge(_serieFields)

            If Not String.IsNullOrEmpty(_serieColor) Then
                serie.Add("color", _serieColor)
            End If

            If _serieOpacity < 1 Then
                serie.Add("opacity", _serieOpacity)
            End If

            serie.Add("axis", axis_name)

            Return serie
        End Function
    End Class

    Private Class MeteoChart
        Private ReadOnly _serieList As List(Of SerieData)
        Public Sub New()
            _serieList = New List(Of SerieData)
        End Sub
        Public Sub Add(sdata As SerieData)
            _serieList.Add(sdata)
        End Sub
        Public Function AddDerived(sdata As SerieData) As Boolean

            Dim master = _serieList.Where(Function(s) s.IdSensore = sdata.IdSensore).FirstOrDefault()

            If master Is Nothing Then
                Return False
            End If

            master.AddDerived(sdata)

            Return True
        End Function

        Public Function MakeChart(isHourly As Boolean) As JObject

            Dim defColors As String() = {"#5DA5DA", "#FAA43A", "#60BD68", "#F17CB0", "#B2912F", "#B276B2", "#DECF3F", "#F15854"}
            Dim maxColor As Integer = defColors.Count
            Dim idxColor As Integer = 0

            'Controllo preventivo che non ci siano 2 serie dello stesso tipo con lo stesso colore...
            Dim iSer As Integer = 0
            Dim haveSimilar As Boolean = False
            While Not haveSimilar AndAlso iSer < _serieList.Count

                Dim jSer As Integer = iSer + 1
                While Not haveSimilar AndAlso jSer < _serieList.Count

                    haveSimilar = _serieList(iSer).IsSimilar(_serieList(jSer))

                    jSer += 1
                End While

                iSer += 1
            End While

            Dim seriesList As New List(Of JObject)
            Dim axisDict As New Dictionary(Of String, String) 'Name, Title
            Dim forceColor As String = ""

            For Each ser In _serieList
                If haveSimilar Then
                    forceColor = defColors(idxColor)
                    idxColor = (idxColor + 1) Mod maxColor
                End If
                ser.FillList(seriesList, forceColor, axisDict)
            Next

            Dim arrSeries As New JArray
            Dim arrAxis As New JArray
            Dim singleAxes As Boolean = axisDict.Count = 1

            For Each s In seriesList
                If singleAxes Then
                    s.Remove("axis")
                End If
                arrSeries.Add(s)
            Next

            For Each kvp In axisDict
                Dim a As New JObject
                If Not singleAxes Then
                    a.Add("name", kvp.Key)
                End If
                a.Add("title", New JObject(New JProperty("text", kvp.Value)))

                arrAxis.Add(a)
            Next

            Dim objChart As New JObject

            If isHourly Then

                objChart.Add("horizAxis", New JObject(New JProperty("field", "DataOra"), New JProperty("baseUnit", "hours")))
            Else

                objChart.Add("horizAxis", "DataOra")
            End If

            objChart.Add("series", arrSeries)
            objChart.Add("axis", arrAxis)

            Return objChart
        End Function
    End Class


#End Region

    Private _objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    Private _objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri

    Private _URL_NetCoreDataExchange_API As String
    Private _AccessToken As String
    Private _CallNetCoreUtility As CallNetCore

    Public Sub New(objParametriServer As AgronicaCoreParametri, objParametriSuperServer As AgronicaCoreParametri)

        _objParametri_Server = objParametriServer
        _objParametri_Super_Server = objParametriSuperServer
    End Sub

#Region "Lettura Dati Meteo"

    Public Function DatiMeteoElabora(ByVal objParams As JObject) As String

        If Not ControllaUtilizzoNuovoEngineMeteoSuite() Then
            Dim objMeteo As New AgronicaCoreWebService.MeteoNT
            Return objMeteo.DatiMeteoElabora(objParams, _objParametri_Server)
        End If

        Dim r As New rispostaStandard(Of RisultatoMeteo) With {
            .RispostaStringa = New RisultatoMeteo()
        }

        Try
            Dim Piva As String = _objParametri_Server.PivaSuperUser
            Dim TipoSorgente As Integer = CInt(objParams("TipoSorgente"))
            Dim Sorgente As Integer = CInt(objParams("Sorgente"))
            Dim DataInizio As DateTime = Convert.ToDateTime(objParams("DataInizio").ToString)
            Dim DataFine As DateTime = Convert.ToDateTime(objParams("DataFine").ToString)
            Dim FrequenzaDati As String = objParams("FrequenzaDati").ToString()
            Dim SogliaTemp As Decimal = Convert.ToDecimal(objParams("SogliaTemp"))
            Dim SogliaFreddo As Decimal = 7
            If objParams("SogliaFreddo") IsNot Nothing Then
                SogliaFreddo = CDec(objParams("SogliaFreddo"))
            End If
            Dim CfrStorico As String = objParams("CfrStorico").ToString()
            Dim LinguaCodiceISO As String = objParams("LinguaCodiceISO").ToString()

            Dim _periodi As New List(Of Periodo) From {
                New Periodo With {
                    .DataInizio = DataInizio,
                    .DataFine = DataFine,
                    .Output = New MeteoOutput
                }
            }

            If Not String.IsNullOrEmpty(CfrStorico) Then
                Dim CfrAnni As Integer() = Array.ConvertAll(CfrStorico.Split("|"), Function(s) Convert.ToInt32(s.Trim()))
                If CfrAnni.Count > 0 Then
                    Dim anno As Integer = DataInizio.Year
                    If anno <> DataFine.Year Then
                        'Messaggio errore -> Intervallo a cavallo di due anni... (altrimenti cosa devo fare?)
                    End If

                    For Each a In CfrAnni
                        If a <> anno Then
                            _periodi.Add(New Periodo With {
                                     .DataInizio = New Date(a, DataInizio.Month, DataInizio.Day),
                                     .DataFine = New Date(a, DataFine.Month, DataFine.Day),
                                     .Output = New MeteoOutput
                                     })
                        End If
                    Next

                    If _periodi.Count = 1 Then
                        'Non ho nulla da confrontare...
                    End If
                End If
            End If

            Dim datiOrari As Boolean = Not FrequenzaDati = "G"

            For Each periodo In _periodi

                Dim response = ChiamataNetCore_LeggiDatiMeteo(datiOrari, Piva, TipoSorgente, Sorgente, DataInizio, DataFine, Nothing, Nothing, SogliaTemp, SogliaFreddo)

                periodo.Output = DatiMeteo2MeteoOutput(response.RispostaStringa, datiOrari)
            Next

            If _periodi.Count = 1 Then

                r.RispostaStringa = ConvertiOutput(_periodi(0).Output, Not datiOrari)
                r.RispostaOK = True

            Else

                Dim olp As New OutputListaPeriodi(_periodi)

                If olp.GeneraOutput() Then
                    r.RispostaStringa.Meteo_Table = olp.OutputModello()
                    r.RispostaStringa.Meteo_Charts = olp.OutputCharts()
                    r.RispostaStringa.Meteo_RiepilogoPeriodo = olp.OutputRiepilogo()
                    r.RispostaStringa.Meteo_RiepilogoSensori = ""
                    r.RispostaOK = True
                Else

                    r.RispostaOK = False
                    r.Errore = "Dati Meteo Non Presenti Nei Periodi"
                End If

            End If

        Catch ex As Exception

            r.RispostaOK = False

            r.Errore = Gias.ErroreDuranteOperazione_ & vbCrLf &
                Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return JsonConvert.SerializeObject(r)

    End Function

    Private Function DatiMeteo2MeteoOutput(datiMeteoResponse As AcquisizioneDatiMeteoResponse, isHourly As Boolean) As MeteoOutput

        If datiMeteoResponse Is Nothing Then
            Return Nothing
        End If

        If datiMeteoResponse.Dati Is Nothing OrElse datiMeteoResponse.Dati.Count = 0 Then
            Return Nothing
        End If

        Dim mOut As New MeteoOutput
        mOut.Dati = datiMeteoResponse.Dati
        mOut.Sensori = datiMeteoResponse.Sensori
        mOut.Charts = New JArray
        mOut.Riepilogo = datiMeteoResponse.Riepilogo
        mOut.RiepilogoSensori = datiMeteoResponse.RiepilogoSensori

        Dim ChartList As New List(Of MeteoChart)
        Dim grouped_dict As New Dictionary(Of Integer, MeteoChart)
        Dim ungrouped_dict As New Dictionary(Of String, List(Of SerieData))
        Dim derived As New List(Of SerieData)

        For Each s In datiMeteoResponse.Sensori

            Dim sdr As New SerieData(s)
            Dim gruppo = sdr.Gruppo

            If gruppo > 0 Then

                If grouped_dict.ContainsKey(gruppo) Then

                    grouped_dict(gruppo).Add(sdr)
                Else

                    Dim chart As New MeteoChart
                    chart.Add(sdr)
                    ChartList.Add(chart)
                    grouped_dict.Add(gruppo, chart)
                End If
            Else

                If sdr.IsDerived Then
                    'Appoggio la configurazione in un elenco provvisorio per poi raggrupparla in seguito insieme al suo master...

                    derived.Add(sdr)
                Else

                    Dim tiposensore = sdr.TipoSensore

                    If ungrouped_dict.ContainsKey(tiposensore) Then

                        ungrouped_dict(tiposensore).Add(sdr)
                    Else

                        ungrouped_dict.Add(tiposensore, New List(Of SerieData) From {sdr})
                    End If
                End If
            End If
        Next

        'Assegno lo stesso grafico a sensori dello stesso tipo (non Generici)
        For Each kvp In ungrouped_dict

            'If kvp.Key <> 0 Then
            'tiposensore non generico: lo stesso chart per tutti i sensori

            Dim chart As New MeteoChart
                For Each sdr In kvp.Value
                    chart.Add(sdr)
                Next
                ChartList.Add(chart)
            'Else

            '    'Un chart per ogni sensore
            '    For Each sdr In kvp.Value
            '        Dim chart As New MeteoChart
            '        chart.Add(sdr)
            '        ChartList.Add(chart)
            '    Next
            'End If
        Next

        'Per tutti i sensori derivati non già raggruppati...
        For Each sdr In derived
            Dim added As Boolean = False
            Dim ichart As Integer = 0
            While Not added AndAlso ichart < ChartList.Count

                added = ChartList(ichart).AddDerived(sdr)

                ichart += 1
            End While
        Next

        For Each chart In ChartList
            mOut.Charts.Add(chart.MakeChart(isHourly))
        Next

        Return mOut
    End Function

    Public Function DatiMeteoElaboraRiepilogo(ByVal objParams As JObject) As String

        If Not ControllaUtilizzoNuovoEngineMeteoSuite() Then
            Dim objMeteo As New AgronicaCoreWebService.MeteoNT
            Return objMeteo.DatiMeteoElaboraRiepilogo(objParams, _objParametri_Server)
        End If

        Dim r As New rispostaStandard(Of RisultatoMeteoRiepilogo.Stazione)
        r.RispostaStringa = New RisultatoMeteoRiepilogo.Stazione()

        Try

            Dim piva As String = objParams("Piva")
            Dim tipoSorgente As Integer = CInt(objParams("TipoSorgente"))
            Dim sorgente As Integer = CInt(objParams("Sorgente"))
            Dim numOre As Integer = 24
            If objParams("NumOre") IsNot Nothing Then
                'Se NumOre è di tipo Decimal il punto decimale crea problemi di conversione
                numOre = Convert.ToDecimal(objParams("NumOre"))
            End If

            Dim numDD = CInt(numOre / 24) + 1

            Dim response = ChiamataNetCore_LeggiDatiMeteo(True, piva, tipoSorgente, sorgente, Nothing, Nothing, numDD, Nothing, Nothing, Nothing)

            If Not response.RispostaStringa Is Nothing Then

                Dim meteoResponse As AcquisizioneDatiMeteoResponse = response.RispostaStringa
                Dim meteoOutput As New MeteoOutput
                meteoOutput.Dati = meteoResponse.Dati

                r.RispostaStringa.Descrizione = meteoResponse.Stazione
                r.RispostaStringa.UltimoAggiornamento = meteoResponse.UltimoAggiornamento
                r.RispostaStringa.Meteo = ConvertiOutput(meteoOutput, False)
                r.RispostaStringa.Meteo.Meteo_RiepilogoSensori = JsonConvert.SerializeObject(meteoResponse.RiepilogoSensori, New JsonSerializerSettings With {.NullValueHandling = NullValueHandling.Ignore})

                r.RispostaOK = True
            Else

                r.RispostaOK = False
            End If

        Catch ex As Exception

            r.RispostaOK = False

            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return JsonConvert.SerializeObject(r)

    End Function

    Public Function DatiMeteoElaboraMonitoraggioSuolo(ByVal objParams As JObject) As String

        If Not ControllaUtilizzoNuovoEngineMeteoSuite() Then
            Dim objMeteo As New AgronicaCoreWebService.MeteoNT
            Return objMeteo.DatiMeteoElaboraMonitoraggioSuolo(objParams, _objParametri_Server)
        End If

        Dim r As New rispostaStandard(Of RisultatoMeteoMonitoraggioSuolo.Stazione)
        r.RispostaStringa = New RisultatoMeteoMonitoraggioSuolo.Stazione()

        Try

            Dim piva As String = objParams("Piva")
            Dim tipoSorgente As Integer = CInt(objParams("TipoSorgente"))
            Dim sorgente As Integer = CInt(objParams("Sorgente"))

            Dim soglia_inf As Decimal = 0
            Dim soglia_sup As Decimal = 0
            Dim num_gg As Integer = 7

            If objParams("Parametri") IsNot Nothing Then
                Dim par = objParams("Parametri")
                If par("periodo_gg") IsNot Nothing Then
                    num_gg = Convert.ToDecimal(par("periodo_gg"))
                End If
                If par("soglia_inf") IsNot Nothing Then
                    soglia_inf = Convert.ToDecimal(par("soglia_inf"))
                End If
                If par("soglia_sup") IsNot Nothing Then
                    soglia_sup = Convert.ToDecimal(par("soglia_sup"))
                End If
            End If

            Dim response = ChiamataNetCore_LeggiDatiMeteo(True, piva, tipoSorgente, sorgente, Nothing, Nothing, num_gg, Nothing, Nothing, Nothing)

            Dim meteoResponse As AcquisizioneDatiMeteoResponse = response.RispostaStringa

            If Not meteoResponse Is Nothing Then

                Dim meteoOutput As New MeteoOutput
                meteoOutput.Dati = response.RispostaStringa.Dati

                r.RispostaStringa.SogliaInf = soglia_inf
                r.RispostaStringa.SogliaSup = soglia_sup
                r.RispostaStringa.AlertSerie = OutputAlert(meteoOutput, soglia_inf, soglia_sup)

                Dim risMeteo = ConvertiOutput(meteoOutput, False)

                Dim ch_arr = JArray.Parse(risMeteo.Meteo_Charts)

                Dim flag_soglie As Boolean = False

                For Each ch As JObject In ch_arr

                    Dim axis As JArray = ch("axis")
                    Dim series As JArray = ch("series")

                    If axis.Count = 1 Then
                        'unica asse verticale

                        Dim ser As JObject = series(0)
                        'tutte le serie hanno la stessa asse verticale

                        If CInt(ser("tipo_sensore")) = 15 Then

                            Dim axis_with_bands As JObject = New JObject(New JProperty("axes", axis))

                            Dim bands As New JArray
                            bands.Add(New JObject(New JProperty("StopValue", soglia_inf),
                                              New JProperty("Color", "rgb(240, 0, 0)"),
                                              New JProperty("Opacity", 0.2)))
                            bands.Add(New JObject(New JProperty("StopValue", soglia_sup),
                                              New JProperty("Color", "rgb(0, 128, 0)"),
                                              New JProperty("Opacity", 0.2)))
                            bands.Add(New JObject(New JProperty("StopValue", Integer.MaxValue),
                                              New JProperty("Color", "rgb(255, 240, 0)"),
                                              New JProperty("Opacity", 0.2)))

                            Dim oBands = New JObject(New JProperty("Bands", bands))

                            axis_with_bands.Add(New JProperty("bands", oBands))

                            ch("axis") = axis_with_bands

                            flag_soglie = True
                        End If
                    End If
                Next

                If flag_soglie Then

                    risMeteo.Meteo_Charts = ch_arr.ToString
                End If

                r.RispostaStringa.Descrizione = meteoResponse.Stazione
                r.RispostaStringa.Meteo = risMeteo
                r.RispostaStringa.Meteo.Meteo_RiepilogoSensori = JsonConvert.SerializeObject(meteoOutput.RiepilogoSensori, New JsonSerializerSettings With {.NullValueHandling = NullValueHandling.Ignore})

                r.RispostaOK = True

            Else
                r.RispostaOK = False
            End If

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                    AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return JsonConvert.SerializeObject(r)

    End Function

    Public Function DatiMeteoElaboraPiogge(ByVal objParams As JObject) As String

        If Not ControllaUtilizzoNuovoEngineMeteoSuite() Then
            Dim objMeteo As New AgronicaCoreWebService.MeteoNT
            Return objMeteo.DatiMeteoElaboraPiogge(objParams, _objParametri_Server)
        End If

        Dim r As New rispostaStandard(Of String)
        r.RispostaStringa = ""

        Try

            Dim piva As String = _objParametri_Server.PivaSuperUser
            Dim tipoSorgente As Integer = CInt(objParams("TipoSorgente"))
            Dim sorgente As Integer = CInt(objParams("Sorgente"))
            Dim dataInizio As DateTime = Convert.ToDateTime(objParams("DataInizio").ToString)
            Dim dataFine As DateTime = Convert.ToDateTime(objParams("DataFine").ToString)

            Dim response = ChiamataNetCore_LeggiDatiMeteoPioggiaGiornalieri(piva, tipoSorgente, sorgente, dataInizio, dataFine, False)

            If Not response.RispostaStringa Is Nothing Then

                Dim meteoDataPiogge As List(Of DatiMeteoPioggeGiornalieriDto) = response.RispostaStringa.MeteoData

                Dim piogge = New List(Of PioggiaGG)

                For Each datoMeteoPiogge In meteoDataPiogge
                    piogge.Add(New PioggiaGG() With {
                       .DataOra = datoMeteoPiogge.DataOra,
                       .Prec = datoMeteoPiogge.PrecipiatazioniCumulate,
                       .TempMin = datoMeteoPiogge.TemperaturaMin,
                       .TempMax = datoMeteoPiogge.TemperaturaMax,
                       .Temp = datoMeteoPiogge.TemperaturaMedia,
                       .UmRel = datoMeteoPiogge.UmiditaRelativaMedia})
                Next

                r.RispostaStringa = JsonConvert.SerializeObject(piogge)
                r.RispostaOK = True
            Else

                r.RispostaOK = False
                r.Errore = "Non sono presenti dati meteo nel periodo " & dataInizio.ToString("d MMMM yyyy") & " - " & dataFine.ToString("d MMMM yyyy")
            End If

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return JsonConvert.SerializeObject(r)

    End Function

    Public Function DatiMeteoElaboraIrrigazione(ByVal objParams As JObject) As String

        If Not ControllaUtilizzoNuovoEngineMeteoSuite() Then
            Dim objMeteo As New AgronicaCoreWebService.MeteoNT
            Return objMeteo.DatiMeteoElaboraIrrigazione(objParams, _objParametri_Server)
        End If

        Dim r As New rispostaStandard(Of String)
        r.RispostaStringa = ""

        Try

            Dim piva As String = _objParametri_Server.PivaSuperUser
            Dim tipoSorgente As Integer = CInt(objParams("TipoSorgente"))
            Dim sorgente As Integer = CInt(objParams("Sorgente"))
            Dim dataInizio As DateTime = Convert.ToDateTime(objParams("DataInizio").ToString)
            Dim dataFine As DateTime = Convert.ToDateTime(objParams("DataFine").ToString)

            Dim response = ChiamataNetCore_LeggiDatiMeteoPioggiaGiornalieri(piva, tipoSorgente, sorgente, dataInizio, dataFine, True)

            If Not response.RispostaStringa Is Nothing Then

                Dim meteoDataPiogge As List(Of DatiMeteoPioggeGiornalieriDto) = response.RispostaStringa.MeteoData
                Dim meteoSensori As DatiMeteoPioggiaSensori = response.RispostaStringa.Sensors

                Dim data As New JArray

                For Each datoMeteoPiogge In meteoDataPiogge
                    Dim d As New JObject
                    d("Date") = Convert.ToDateTime(datoMeteoPiogge.DataOra).Date
                    d("Tmed") = Convert.ToDecimal(datoMeteoPiogge.TemperaturaMedia.GetValueOrDefault())
                    d("Tmin") = Convert.ToDecimal(datoMeteoPiogge.TemperaturaMin.GetValueOrDefault())
                    d("Tmax") = Convert.ToDecimal(datoMeteoPiogge.TemperaturaMax.GetValueOrDefault())
                    d("RainMM") = Convert.ToDecimal(datoMeteoPiogge.PrecipiatazioniCumulate.GetValueOrDefault())
                    d("RainH") = Convert.ToDecimal(datoMeteoPiogge.OrePioggia.GetValueOrDefault())
                    If Not meteoSensori.SensoriUmiditaTerreno Is Nothing Then
                        For Each SensoreMeteoUT In meteoSensori.SensoriUmiditaTerreno
                            Dim utValue As Single? = If(Not meteoSensori.SensoriUmiditaTerreno Is Nothing, datoMeteoPiogge.UmiditaTerreno.Where(Function(x) x.Sensore.Equals(SensoreMeteoUT.Sensore)).FirstOrDefault()?.Valore, Nothing)
                            d("UT_" + SensoreMeteoUT.Sensore) = Convert.ToDecimal(utValue)
                        Next
                    End If
                    data.Add(d)
                Next

                Dim model As New JArray
                model.Add(New JObject(New JProperty("field", "Date"), New JProperty("label", "Date")))
                model.Add(New JObject(New JProperty("field", "Tmed"), New JProperty("label", meteoSensori.SensoreTemperatura.Etichetta)))
                model.Add(New JObject(New JProperty("field", "Tmin"), New JProperty("label", meteoSensori.SensoreTemperatura.Etichetta)))
                model.Add(New JObject(New JProperty("field", "Tmax"), New JProperty("label", meteoSensori.SensoreTemperatura.Etichetta)))
                model.Add(New JObject(New JProperty("field", "RainMM"), New JProperty("label", meteoSensori.SensorePrecipiatazioni.Etichetta)))
                model.Add(New JObject(New JProperty("field", "RainH"), New JProperty("label", meteoSensori.SensorePrecipiatazioni.Etichetta)))
                If Not meteoSensori.SensoriUmiditaTerreno Is Nothing Then
                    For Each SensoreMeteoUT In meteoSensori.SensoriUmiditaTerreno
                        model.Add(New JObject(New JProperty("field", "UT_" + SensoreMeteoUT.Sensore), New JProperty("label", SensoreMeteoUT.Etichetta)))
                    Next
                End If

                Dim resObj As New JObject(New JProperty("model"), New JProperty("data"))
                resObj("model") = model
                resObj("data") = data

                r.RispostaStringa = resObj.ToString(Newtonsoft.Json.Formatting.None)
                r.RispostaOK = True
            Else

                r.RispostaOK = False
                r.Errore = "Non sono presenti dati meteo nel periodo " & dataInizio.ToString("d MMMM yyyy") & " - " & dataFine.ToString("d MMMM yyyy")
            End If

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return JsonConvert.SerializeObject(r)

    End Function

    Private Function ConvertiOutput(meteo_out As MeteoOutput, isDaily As Boolean) As RisultatoMeteo

        If meteo_out Is Nothing Then
            Return New RisultatoMeteo
        End If

        Dim om As New OutputMeteo

        If isDaily Then
            om.aggiungiColonna("DataOra", GetType(Date), Gias.Data, "dd/MM/yyyy")
        Else
            om.aggiungiColonna("DataOra", GetType(DateTime), Gias.DataOra, "dd/MM/yyyy HH\""h\""")
        End If

        Dim coltitle As String
        For Each sens In meteo_out.Sensori

            coltitle = sens.Etichetta
            If Not String.IsNullOrEmpty(sens.UM) Then
                coltitle &= " (" & sens.UM & ")"
            End If

            om.aggiungiColonna(sens.Sensore.ToString, GetType(Decimal), coltitle, "0.00")
        Next

        For Each dato_meteo In meteo_out.Dati

            om.AddField(dato_meteo.DataOra)

            For Each sens In meteo_out.Sensori
                om.AddField(dato_meteo.ValoriSensori.Where(Function(x) x.Sensore.Equals(sens.ProviderReference)).Select(Function(x) x.Valore).FirstOrDefault())
            Next

            om.Commit()
        Next

        Return New RisultatoMeteo With {
            .Meteo_Table = om.Output(),
            .Meteo_Charts = meteo_out.Charts.ToString(),
            .Meteo_RiepilogoPeriodo = MeteoRiepilogo.Output(meteo_out.Riepilogo).ToString(),
            .Meteo_RiepilogoSensori = JsonConvert.SerializeObject(meteo_out.RiepilogoSensori, New JsonSerializerSettings With {.NullValueHandling = NullValueHandling.Ignore})
        }
    End Function

    Private Function OutputAlert(ByVal mout As MeteoOutput, ByVal soglia_inf As Decimal, ByVal soglia_sup As Decimal) As String

        Dim arrAlert As New JArray

        Dim sensoriUmTerr = mout.Sensori.Where(Function(x) x.TipoSensore = "")

        If sensoriUmTerr.Any() Then

            Dim currItem As alert_item = Nothing

            For Each dm In mout.Dati
                Dim minVal As Decimal = Decimal.MaxValue
                Dim maxVal As Decimal = Decimal.MinValue

                For Each sut In sensoriUmTerr
                    Dim oUmTerr = dm.ValoriSensori.Where(Function(x) x.Sensore.Equals(sut.Sensore)).FirstOrDefault()

                    If Not IsDBNull(oUmTerr) Then
                        Dim umterr As Decimal = CDec(oUmTerr.Valore)
                        minVal = Math.Min(minVal, umterr)
                        maxVal = Math.Max(maxVal, umterr)
                    End If
                Next

                Dim dt As Date = CDate(dm.DataOra).Date()

                If currItem Is Nothing OrElse currItem.Data.DayOfYear() <> dt.DayOfYear() Then

                    If currItem IsNot Nothing Then

                        arrAlert.Add(currItem.alertObj(soglia_inf, soglia_sup))
                    End If

                    currItem = New alert_item With {.Data = dt, .minValue = minVal, .maxValue = maxVal}

                Else

                    currItem.minValue = Math.Min(currItem.minValue, minVal)
                    currItem.maxValue = Math.Max(currItem.maxValue, maxVal)
                End If
            Next

            If currItem IsNot Nothing Then
                arrAlert.Add(currItem.alertObj(soglia_inf, soglia_sup))
            End If
        End If

        Return arrAlert.ToString(Newtonsoft.Json.Formatting.None)
    End Function


    Private Function ChiamataNetCore_LeggiDatiMeteo(ByVal DatiOrari As Boolean, ByVal Piva As String, ByVal Tipo_Sorgente As Integer, ByVal Stazione_Cod As Integer,
                                                    ByVal DataInizio As DateTime?, ByVal DataFine As DateTime?,
                                                    ByVal IntervalloGiorni As Integer?, ByVal IntervalloOre As Integer?,
                                                    ByVal SogliaTermica As Decimal?, ByVal SogliaFreddo As Decimal?) As rispostaStandard(Of AcquisizioneDatiMeteoResponse)

        Dim tipo As Integer = MapTipoSorgenteGiasToMeteoSuite(Tipo_Sorgente)

        Dim endpoint As String = If(DatiOrari, $"DatiMeteo/LeggiDatiMeteoOrari", $"DatiMeteo/LeggiDatiMeteoGiornalieri")
        endpoint = endpoint + $"?Piva={Piva}&TipoSorgente={tipo.ToString()}&IdStazione={Stazione_Cod.ToString()}"

        If DataInizio.HasValue And DataFine.HasValue Then
            Dim strDataInizio As String = DataInizio.Value.ToString("o")
            Dim strDataFine As String = DataFine.Value.ToString("o")

            endpoint += $"&DataInizio={strDataInizio}&DataFine={strDataFine}"
        End If

        If Not IntervalloGiorni Is Nothing Then
            endpoint += $"&IntervalloG={IntervalloGiorni.ToString()}"
        End If

        If Not IntervalloOre Is Nothing Then
            endpoint += $"&IntervalloH={IntervalloOre.ToString()}"
        End If

        If Not SogliaTermica Is Nothing Then
            endpoint += $"&SogliaTermica={SogliaTermica.ToString().Replace(",", ".")}"
        End If

        If Not SogliaFreddo Is Nothing Then
            endpoint += $"&SogliaFreddo={SogliaFreddo.ToString().Replace(",", ".")}"
        End If

        Dim apiResponse = ChiamataNetCoreDataExchangeGet(endpoint)

        Dim response = JsonConvert.DeserializeObject(Of rispostaStandard(Of AcquisizioneDatiMeteoResponse))(apiResponse)

        Return response
    End Function

    Private Function ChiamataNetCore_LeggiDatiMeteoPioggiaGiornalieri(ByVal Piva As String, ByVal Tipo_Sorgente As Integer, ByVal Stazione_Cod As Integer,
                                                                      ByVal DataInizio As DateTime, ByVal DataFine As DateTime,
                                                                      ByVal LeggiOrePioggia As Boolean) As rispostaStandard(Of AcquisizioneDatiMeteoPioggeGiornalieriResponse)

        Dim tipo As Integer = MapTipoSorgenteGiasToMeteoSuite(Tipo_Sorgente)

        Dim endpoint As String = $"DatiMeteo/LeggiDatiMeteoPioggeGiornalieri?Piva={Piva}&TipoSorgente={tipo.ToString()}&IdStazione={Stazione_Cod.ToString()}"

        Dim strDataInizio As String = DataInizio.ToString("o")
        Dim strDataFine As String = DataFine.ToString("o")
        endpoint += $"&DataInizio={strDataInizio}&DataFine={strDataFine}&LeggiOrePioggia={LeggiOrePioggia.ToString()}"

        Dim apiResponse = ChiamataNetCoreDataExchangeGet(endpoint)

        Dim response = JsonConvert.DeserializeObject(Of rispostaStandard(Of AcquisizioneDatiMeteoPioggeGiornalieriResponse))(apiResponse)

        Return response
    End Function

#End Region

#Region "Anagrafica Stazioni"

    Public Function ElencoSorgentiMeteo(objParams As JObject) As String

        If Not ControllaUtilizzoNuovoEngineMeteoSuite() Then
            Dim objMeteo As New AgronicaCoreWebService.MeteoNT
            Return objMeteo.ElencoSorgentiMeteo(objParams, _objParametri_Server)
        End If

        Dim r As New RispostaStandard
        Try

            Dim objMeteoDAL_R As New MeteoSuite_R
            Dim dtSorgenti = objMeteoDAL_R.LeggiSorgentiMeteo(_objParametri_Server)

            Dim sorgentiMeteo As New List(Of SorgenteMeteo)

            For Each dr In dtSorgenti.Rows
                sorgentiMeteo.Add(New SorgenteMeteo With {.sorgente_cod = dr("id"), .sorgente_des = dr("Descrizione")})
            Next

            Dim sorgenti = JArray.FromObject(sorgentiMeteo)

            If objParams("RilievoPiogge") IsNot Nothing Then
                Dim flag As Boolean = CBool(objParams("RilievoPiogge"))
                If flag Then
                    sorgenti.Add(New JObject(New JProperty("sorgente_cod", 999), New JProperty("sorgente_des", "Stazioni irrigazione preferite")))
                End If
            End If

            r.RispostaStringa = sorgenti.ToString()
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return JsonConvert.SerializeObject(r)

    End Function

    Public Function LeggiAnagraficaStazioni(ByVal objParams As JObject) As String

        If Not ControllaUtilizzoNuovoEngineMeteoSuite() Then
            Dim objMeteo As New AgronicaCoreWebService.MeteoNT
            Return objMeteo.LeggiAnagraficaStazioni(objParams, _objParametri_Server)
        End If

        Dim r As New RispostaStandard
        Try

            Dim PIVA As String = objParams("PIVA")
            Dim Tipo_Sorgente As Integer = enum_Meteo_Tiposorgente.RetiPartner

            Dim response = ChiamataNetCore_LeggiStazioniAutorizzate(PIVA, Tipo_Sorgente, Nothing, Nothing)

            Dim stazArr As New JArray

            If Not response.RispostaStringa Is Nothing Then

                stazArr = ArrayAnagStazioni(response.RispostaStringa)

            End If

            r.RispostaStringa = stazArr.ToString() 'resultList.ToString()
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return JsonConvert.SerializeObject(r)

    End Function

    Public Function AggiornaAnagraficaStazione(ByVal objParams As JObject) As String

        If Not ControllaUtilizzoNuovoEngineMeteoSuite() Then
            Dim objMeteo As New AgronicaCoreWebService.MeteoNT
            Return objMeteo.AggiornaAnagraficaStazione(objParams, _objParametri_Server)
        End If

        Dim r As New RispostaStandard
        Try

            Dim PIVA As String = objParams("PIVA")
            Dim objAnag As JObject = objParams("Anagrafica")
            Dim IdStazione As Integer = CInt(objAnag("Staz_Id"))
            Dim NomeStazione As String = objAnag("Staz_Nome").ToString()
            Dim Latitude As Decimal? = Nothing
            Dim Longitude As Decimal? = Nothing
            If objAnag("Staz_Geo") IsNot Nothing Then
                Latitude = Convert.ToDecimal(objAnag("Staz_Geo")("Lat"))
                Longitude = Convert.ToDecimal(objAnag("Staz_Geo")("Lng"))
            End If

            Dim dictSensori As New Dictionary(Of Integer, String)
            Dim arrSensori As JArray = objAnag("Sensori")
            For Each objSens In arrSensori
                dictSensori.Add(objSens("Id_Sensore"), objSens("Etichetta"))
            Next

            Dim response = ChiamataNetCore_AggiornaStazione(PIVA, IdStazione, NomeStazione, Latitude, Longitude, dictSensori)

            If Not response.RispostaOK Then
                Throw New Exception("Meteo Suite meteo stations update error: " + response.Errore)
            End If

            If Not response.RispostaStringa Is Nothing Then

                Dim resultStazione As StazioneMeteoDto = response.RispostaStringa

                Dim stazArr As JArray = ArrayAnagStazioni(New List(Of StazioneMeteoDto) From {resultStazione})

                r.RispostaStringa = stazArr(0).ToString()
                r.RispostaOK = True

            Else

                r.RispostaOK = False
            End If

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return JsonConvert.SerializeObject(r)

    End Function

    Private Function ArrayAnagStazioni(ByVal stazList As List(Of StazioneMeteoDto)) As JArray

        Dim stazArr As New JArray

        If stazList IsNot Nothing Then

            Dim staz_id As Integer = 0

            Dim stazObj As JObject = Nothing
            Dim sensArr As JArray = Nothing

            For Each stazione In stazList

                stazObj = New JObject

                stazObj("Staz_Id") = stazione.Id_Stazione
                stazObj("Staz_Nome") = stazione.Nome_Stazione
                If Not stazione.Lat = 0 AndAlso Not stazione.Lng = 0 Then
                    Dim geoObj As New JObject
                    geoObj("Lat") = stazione.Lat
                    geoObj("Lng") = stazione.Lng
                    stazObj("Staz_Geo") = geoObj
                End If
                stazObj("Fornitore") = stazione.Fornitore
                stazObj("RifFornitore") = stazione.RifFornitore
                stazObj("Proprietario") = stazione.Proprietario
                stazObj("FlagReale") = stazione.FlagReale

                sensArr = New JArray
                stazObj("Sensori") = sensArr

                For Each sens In stazione.Sensors
                    Dim sensObj As New JObject
                    sensObj("Id_Sensore") = sens.Id_Sensore
                    sensObj("Sensore") = sens.Description
                    sensObj("Tipo_Sensore") = sens.SensorType
                    sensObj("UM") = sens.MeasureUnit

                    Dim objOutputConfig As New JObject
                    If Not String.IsNullOrEmpty(sens.OutputConfig) Then
                        objOutputConfig = JObject.Parse(sens.OutputConfig)
                    End If
                    sensObj("OutputConfig") = objOutputConfig

                    sensObj("Id_StazioneOrigine") = sens.Id_StazioneOrigine
                    sensObj("StazioneOrigine") = sens.StazioneOrigine

                    If sens.Last_Update IsNot Nothing AndAlso Not IsDBNull(sens.Last_Update) Then
                        sensObj("Last_Update") = sens.Last_Update
                    End If

                    sensArr.Add(sensObj)
                Next

                If stazObj IsNot Nothing Then
                    stazArr.Add(stazObj)
                End If
            Next

        End If

        Return stazArr
    End Function

    Public Function VerificaEliminaStazione(ByVal objParams As JObject) As String

        If Not ControllaUtilizzoNuovoEngineMeteoSuite() Then
            Dim objMeteo As New AgronicaCoreWebService.MeteoNT
            Return objMeteo.VerificaEliminaStazione(objParams, _objParametri_Server)
        End If

        Dim r As New RispostaStandard
        Try

            Dim PIVA As String = objParams("PIVA").ToString()
            Dim Stazione_Cod As Integer = CInt(objParams("Stazione_Cod"))

            Dim response = ChiamataNetCore_EliminaStazione(PIVA, Stazione_Cod)

            Dim iError As Integer
            Dim message As String

            If response.RispostaOK Then

                iError = 0
                message = ""

            Else

                iError = 1
                message = response.Errore

            End If

            r.RispostaStringa = (New JObject(New JProperty("Error", iError), New JProperty("Message", message))).ToString(Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return JsonConvert.SerializeObject(r)

    End Function

    Public Function LeggiStazioniXSorgente(ByVal objParams As JObject) As String

        If Not ControllaUtilizzoNuovoEngineMeteoSuite() Then
            Dim objMeteo As New AgronicaCoreWebService.MeteoNT
            Return objMeteo.LeggiStazioniXSorgente(objParams, _objParametri_Server)
        End If

        Dim r As New RispostaStandard
        Try

            Dim PIVA As String = objParams("PIVA")
            Dim TipoSorgente As Integer = objParams("TipoSorgente")

            If TipoSorgente = 999 Then
                AddElencoStazioniDaIrriga(objParams)
            End If

            Dim lat As Decimal = 0
            Dim lng As Decimal = 0
            If objParams("DistanzaDa") IsNot Nothing Then
                lat = Convert.ToDecimal(objParams("DistanzaDa")("lat"))
                lng = Convert.ToDecimal(objParams("DistanzaDa")("lng"))
            End If

            Dim jElencoStazioni As New JArray
            If objParams("ElencoStazioni") IsNot Nothing Then
                jElencoStazioni = JArray.Parse(objParams("ElencoStazioni").ToString)
            Else
                jElencoStazioni.Add(New JObject(New JProperty("tipo_sorgente", TipoSorgente)))
            End If

            'Dim stazArr As JArray = LeggiStazioniDaElenco(PIVA, lat, lng, jElencoStazioni)

            Dim stazArr As New JArray

            For Each jsc In jElencoStazioni

                Dim tipo_sorgente As Integer = jsc("tipo_sorgente")
                Dim elenco_stazioni As List(Of Integer) = Nothing

                If jsc("elenco_stazioni") IsNot Nothing Then
                    elenco_stazioni = New List(Of Integer)

                    For Each c In jsc("elenco_stazioni")
                        elenco_stazioni.Add(CInt(c("stazione_cod")))
                    Next
                End If

                Dim response = ChiamataNetCore_LeggiStazioniAutorizzate(PIVA, tipo_sorgente, lat, lng)

                If Not response.RispostaStringa Is Nothing Then

                    For Each stazione In response.RispostaStringa

                        If Not elenco_stazioni Is Nothing Then
                            If Not elenco_stazioni.Contains(stazione.Id_Stazione) Then
                                Continue For
                            End If
                        End If

                        Dim stazObj As New JObject

                        stazObj("tipo_sorgente") = tipo_sorgente
                        stazObj("id_stazione") = stazione.Id_Stazione
                        stazObj("nome_stazione") = stazione.Nome_Stazione
                        stazObj("fornitore") = stazione.Fornitore
                        stazObj("rif_fornitore") = stazione.RifFornitore
                        stazObj("flag_reale") = stazione.FlagReale

                        If Not stazione.Lat = 0 AndAlso Not stazione.Lng = 0 Then
                            Dim latlng As New JObject
                            latlng("lat") = stazione.Lat
                            latlng("lng") = stazione.Lng
                            stazObj("geo") = latlng
                        End If

                        If Not stazione.Last_Update Is Nothing Then
                            stazObj("ultimo_aggiornamento") = Convert.ToDateTime(stazione.Last_Update)
                        End If

                        If Not stazione.Distanza Is Nothing Then
                            stazObj("distanza") = stazione.Distanza
                        End If

                        stazArr.Add(stazObj)
                    Next

                End If
            Next

            r.RispostaStringa = stazArr.ToString()
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return JsonConvert.SerializeObject(r)

    End Function

    Private Sub AddElencoStazioniDaIrriga(ByVal objParams As JObject)

        Dim piva As String = objParams("PIVA")

        Dim objSxI As New AgronicaCoreMeteoDAL.DSS_Stazioni_X_Irriga_Reader
        Dim filtroAggiuntivo As AgronicaCoreMeteoDAL.DSS_Stazioni_X_Irriga_Reader.Filter = Nothing

        If objParams("CodiceCentro") IsNot Nothing Then
            Try
                Dim sa_cod As Integer = CInt(objParams("CodiceCentro"))
                If sa_cod > 0 Then
                    filtroAggiuntivo = New AgronicaCoreMeteoDAL.DSS_Stazioni_X_Irriga_Reader.Filter With {
                        .CodiceCentro = sa_cod,
                        .TipoSorgente = -1,
                        .StazioneCod = ""
                    }
                End If
            Catch ex As Exception
            End Try
        End If

        Dim dt_SxI = objSxI.Leggi(piva, filtroAggiuntivo, _objParametri_Server)
        Dim dictSorgente As New Dictionary(Of Integer, List(Of Integer))

        For Each _si In dt_SxI.Rows
            Dim _tipoSorgente As Integer = _si("Tipo_Sorgente")
            Dim _stazioneCod As Integer = _si("Stazione_Cod")

            If Not dictSorgente.ContainsKey(_tipoSorgente) Then
                dictSorgente.Add(_tipoSorgente, New List(Of Integer))
            End If

            If Not dictSorgente(_tipoSorgente).Contains(_stazioneCod) Then
                dictSorgente(_tipoSorgente).Add(_stazioneCod)
            End If
        Next

        Dim j_stazioni As New JArray
        Dim j_staz As JArray
        For Each kvp In dictSorgente
            j_staz = New JArray

            For Each s In kvp.Value
                j_staz.Add(New JObject(New JProperty("stazione_cod", s)))
            Next

            j_stazioni.Add(New JObject(New JProperty("tipo_sorgente", kvp.Key), New JProperty("elenco_stazioni", j_staz)))
        Next

        objParams.Add(New JProperty("ElencoStazioni", j_stazioni))
    End Sub

    Public Function LeggiStazione(stazione_cod As Integer, needSensors As Boolean) As String

        If Not ControllaUtilizzoNuovoEngineMeteoSuite() Then
            Dim objMeteo As New AgronicaCoreWebService.MeteoNT
            Return objMeteo.LeggiStazione(stazione_cod, needSensors, _objParametri_Server)
        End If

        Dim r As New RispostaStandard
        Try
            Dim piva As String = _objParametri_Server.PivaSuperUser
            Dim Tipo_Sorgente As Integer = enum_Meteo_Tiposorgente.RetiPartner

            Dim response = ChiamataNetCore_LeggiStazionePerId(piva, Tipo_Sorgente, stazione_cod, Nothing, Nothing)

            Dim StazioneOut As LeggiStazioneOut = Nothing

            If Not response.RispostaStringa Is Nothing Then
                Dim stazione As StazioneMeteoDto = response.RispostaStringa

                StazioneOut = New LeggiStazioneOut With {
                    .Id = stazione.Id_Stazione,
                    .Stazione = stazione.Nome_Stazione,
                    .Lat = stazione.Lat,
                    .Lng = stazione.Lng,
                    .FlagReale = stazione.FlagReale,
                    .RifFornitore = stazione.RifFornitore,
                    .Fornitore = stazione.Fornitore,
                    .Sensori = Nothing
                }

                If needSensors Then
                    StazioneOut.Sensori = New List(Of LeggiStazioneOut.Sensore)

                    For Each resultSensore In stazione.Sensors
                        StazioneOut.Sensori.Add(New LeggiStazioneOut.Sensore With {
                            .Id = resultSensore.Id_Sensore,
                            .Sensore = resultSensore.Description,
                            .Tipo = resultSensore.SensorType,
                            .UM = resultSensore.MeasureUnit,
                            .Id_StazioneOrigine = resultSensore.Id_StazioneOrigine,
                            .StazioneOrigine = resultSensore.StazioneOrigine,
                            .Last_Update = resultSensore.Last_Update})
                    Next
                End If
            End If

            r.RispostaStringa = JsonConvert.SerializeObject(StazioneOut)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return JsonConvert.SerializeObject(r)

    End Function

    Public Function LeggiElencoStazioni(ByVal objParams As JObject) As String

        If Not ControllaUtilizzoNuovoEngineMeteoSuite() Then
            Dim objMeteo As New AgronicaCoreWebService.MeteoNT
            Return objMeteo.LeggiElencoStazioni(objParams, _objParametri_Server)
        End If

        Dim r As New RispostaStandard
        Try

            Dim PIVA As String = objParams("PIVA")
            Dim jElencoStazioni As JArray = objParams("ElencoStazioni")

            'Dim stazArr As JArray = LeggiStazioniDaElenco(PIVA, Nothing, Nothing, jElencoStazioni)

            'Dim stazArr As New JArray

            For Each jsc In jElencoStazioni

                Dim tipo_sorgente As Integer = jsc("tipo_sorgente")
                Dim elenco_stazioni As List(Of Integer) = Nothing

                If jsc("elenco_stazioni") IsNot Nothing Then
                    elenco_stazioni = New List(Of Integer)

                    For Each c In jsc("elenco_stazioni")
                        elenco_stazioni.Add(CInt(c("stazione_cod")))
                    Next
                End If

                Dim result As List(Of StazioneMeteoDto) = Nothing

                If tipo_sorgente = enum_Meteo_Tiposorgente.Pubbliche Then

                    result = New List(Of StazioneMeteoDto)

                    For Each staz_id In elenco_stazioni

                        Dim risp = ChiamataNetCore_LeggiStazionePerId(PIVA, tipo_sorgente, staz_id, Nothing, Nothing)

                        If Not risp.RispostaStringa Is Nothing Then
                            result.Add(risp.RispostaStringa)
                        End If
                    Next
                Else

                    Dim response = ChiamataNetCore_LeggiStazioniAutorizzate(PIVA, tipo_sorgente, Nothing, Nothing)

                    result = response.RispostaStringa
                End If

                If Not result Is Nothing Then

                    Dim sorg = CInt(jsc("tipo_sorgente"))

                    For Each c In jsc("elenco_stazioni")

                        Dim staz As Integer = CInt(c("stazione_cod"))

                        Dim stazione = result.Where(Function(x) x.Id_Stazione = staz).FirstOrDefault()

                        If Not stazione Is Nothing Then

                            c("stazione_nome") = stazione.Nome_Stazione
                            c("rif_fornitore") = stazione.RifFornitore
                            c("flag_reale") = stazione.FlagReale
                            c("sensori") = String.Join("§", stazione.Sensors.Select(Function(x) x.Description))
                        End If
                    Next

                End If
            Next

            r.RispostaStringa = jElencoStazioni.ToString()
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return JsonConvert.SerializeObject(r)

    End Function

    Public Function StazioniDaLatLng(ByVal objParams As JObject) As String

        If Not ControllaUtilizzoNuovoEngineMeteoSuite() Then
            Dim objMeteo As New AgronicaCoreWebService.MeteoNT
            Return objMeteo.StazioniDaLatLng(objParams, _objParametri_Server)
        End If

        Dim r As New RispostaStandard
        Try

            Dim PIVA As String = objParams("PIVA")
            Dim jsonElencoPosizioni = objParams("ElencoPosizioni").ToString()
            Dim ElencoPosizioni = JsonConvert.DeserializeObject(Of List(Of AgronicaCoreMeteoDAL.MeteoNT.IdLatLng))(jsonElencoPosizioni)

            Dim Tipo_Sorgente As Integer = enum_Meteo_Tiposorgente.Pubbliche

            Dim ResultElenco As New List(Of AgronicaCoreMeteoDAL.MeteoNT.IdLatLng)

            For Each Posizione In ElencoPosizioni

                Dim response = ChiamataNetCore_LeggiStazionePerPosizione(PIVA, Tipo_Sorgente, Posizione.Lat, Posizione.Lng)

                If Not response.RispostaStringa Is Nothing Then

                    Dim stazione As StazioneMeteoDto = response.RispostaStringa
                    ResultElenco.Add(New AgronicaCoreMeteoDAL.MeteoNT.IdLatLng With {
                              .Id = Posizione.Id,
                              .Lat = stazione.Lat,
                              .Lng = stazione.Lng,
                              .Id_Stazione = stazione.Id_Stazione})
                End If
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(ResultElenco)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return JsonConvert.SerializeObject(r)

    End Function

    Public Function ElencoStazioniConDistanza(ByVal Piva_SuperUser As String,
                                              ByVal Doorkey As String,
                                              ByVal Utente_Username_client_GIAS As String,
                                              ByVal Username As String,
                                              ByVal Password As String,
                                              ByVal PivaVisibilita_ClientGIASImpostata As String,
                                              ByVal Lat As Double,
                                              ByVal Lng As Double,
                                              ByVal TipoSorgente As Integer,
                                              Optional ByVal pGiasOnline_WS_Meteo_Meteo As String = "") As DataTable

        If Not ControllaUtilizzoNuovoEngineMeteoSuite() Then
            Dim objMeteo As New AgronicaCoreWebService.MeteoNT
            Return objMeteo.ElencoStazioniConDistanza(Piva_SuperUser, Doorkey, Utente_Username_client_GIAS, Username, Password,
                                                      PivaVisibilita_ClientGIASImpostata, _objParametri_Server, Lat, Lng, TipoSorgente,
                                                      pGiasOnline_WS_Meteo_Meteo)
        End If

        Dim DT_Stazioni As DataTable = Nothing
        Try
            Dim response = ChiamataNetCore_LeggiStazioniAutorizzate(PivaVisibilita_ClientGIASImpostata, TipoSorgente, Lat, Lng)

            If Not response.RispostaStringa Is Nothing Then

                DT_Stazioni = New DataTable

                DT_Stazioni.Columns.Add(New DataColumn("id", GetType(Integer)))
                DT_Stazioni.Columns.Add(New DataColumn("descrizione", GetType(String)))
                DT_Stazioni.Columns.Add(New DataColumn("fornitore", GetType(String)))
                DT_Stazioni.Columns.Add(New DataColumn("rif_fornitore", GetType(String)))
                DT_Stazioni.Columns.Add(New DataColumn("data_ultimo_agg", GetType(Date)))
                DT_Stazioni.Columns.Add(New DataColumn("distanza", GetType(Decimal)))

                Dim row As DataRow

                Dim elenco As List(Of StazioneMeteoDto) = response.RispostaStringa

                For Each elem In elenco

                    row = DT_Stazioni.NewRow

                    row("id") = elem.Id_Stazione
                    row("descrizione") = elem.Nome_Stazione
                    row("fornitore") = elem.Fornitore
                    row("rif_fornitore") = elem.RifFornitore

                    If elem.Last_Update IsNot Nothing Then
                        row("data_ultimo_agg") = elem.Last_Update
                    Else
                        row("data_ultimo_agg") = DBNull.Value
                    End If

                    If elem.Distanza IsNot Nothing Then
                        row("distanza") = elem.Distanza
                    Else
                        row("distanza") = DBNull.Value
                    End If

                    DT_Stazioni.Rows.Add(row)

                Next

                DT_Stazioni.DefaultView.Sort = "distanza ASC"
                DT_Stazioni = DT_Stazioni.DefaultView.ToTable

            End If

        Catch ex As Exception

        End Try

        Return DT_Stazioni
    End Function

    Public Function LeggiTipoSensoriPerStazione(ByVal objParams As JObject) As String

        If Not ControllaUtilizzoNuovoEngineMeteoSuite() Then
            Dim objMeteo As New AgronicaCoreWebService.MeteoNT
            Return objMeteo.LeggiTipoSensoriPerStazione(objParams, _objParametri_Server)
        End If

        Dim r As New RispostaStandard
        Try

            Dim PIVA As String = _objParametri_Server.PivaSuperUser
            Dim id_stazione As Integer = CInt(objParams("id_stazione"))
            Dim Tipo_Sorgente As Integer = enum_Meteo_Tiposorgente.RetiPartner

            Dim response = ChiamataNetCore_LeggiStazionePerId(PIVA, Tipo_Sorgente, id_stazione, Nothing, Nothing)

            Dim ar As New JArray

            If Not response.RispostaStringa Is Nothing Then

                For Each sensor In response.RispostaStringa.Sensors
                    ar.Add(New JObject(
                       New JProperty("id_sensore", sensor.Id_Sensore),
                       New JProperty("Id_tipoSensore", sensor.SensorTypeId),
                       New JProperty("tipo", sensor.SensorType),
                       New JProperty("um", sensor.MeasureUnit)
                      ))
                Next
            End If

            r.RispostaOK = True
            r.RispostaStringa = ar.ToString()

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return JsonConvert.SerializeObject(r)

    End Function


    Private Function ChiamataNetCore_LeggiStazioniAutorizzate(ByVal Piva As String, ByVal Tipo_Sorgente As Integer, ByVal Latitude As Decimal?, ByVal Longitude As Decimal?) As rispostaStandard(Of List(Of StazioneMeteoDto))

        Dim tipo As Integer = MapTipoSorgenteGiasToMeteoSuite(Tipo_Sorgente)
        Dim endpoint As String = $"AnagraficaStazioni/LeggiStazioniMeteoAutorizzate?Piva={Piva}&TipoSorgente={tipo.ToString()}"

        If Not Latitude Is Nothing And Not Longitude Is Nothing Then
            Dim strLat As String = Latitude.ToString().Replace(",", ".")
            Dim strLng As String = Longitude.ToString().Replace(",", ".")
            endpoint = endpoint + $"&Longitude={strLng}&Latitude={strLat}"
        End If

        Dim apiResponse = ChiamataNetCoreDataExchangeGet(endpoint)

        Dim response = JsonConvert.DeserializeObject(Of rispostaStandard(Of List(Of StazioneMeteoDto)))(apiResponse)

        Return response
    End Function

    Private Function ChiamataNetCore_LeggiStazionePerId(ByVal Piva As String, ByVal Tipo_Sorgente As Integer, ByVal Stazione_Cod As Integer, ByVal Latitude As Decimal?, ByVal Longitude As Decimal?) As rispostaStandard(Of StazioneMeteoDto)

        Dim tipo As Integer = MapTipoSorgenteGiasToMeteoSuite(Tipo_Sorgente)
        Dim endpoint As String = $"AnagraficaStazioni/LeggiStazioneMeteoPerId?Piva={Piva}&TipoSorgente={tipo.ToString()}&IdStazione={Stazione_Cod.ToString()}"

        If Not Latitude Is Nothing And Not Longitude Is Nothing Then
            Dim strLat As String = Latitude.ToString().Replace(",", ".")
            Dim strLng As String = Longitude.ToString().Replace(",", ".")
            endpoint = endpoint + $"&Longitude={strLng}&Latitude={strLat}"
        End If

        Dim apiResponse = ChiamataNetCoreDataExchangeGet(endpoint)

        Dim response = JsonConvert.DeserializeObject(Of rispostaStandard(Of StazioneMeteoDto))(apiResponse)

        Return response
    End Function

    Private Function ChiamataNetCore_LeggiStazionePerPosizione(ByVal Piva As String, ByVal Tipo_Sorgente As Integer, ByVal Latitude As Decimal, ByVal Longitude As Decimal) As rispostaStandard(Of StazioneMeteoDto)

        Dim tipo As Integer = MapTipoSorgenteGiasToMeteoSuite(Tipo_Sorgente)
        Dim strLat As String = Latitude.ToString().Replace(",", ".")
        Dim strLng As String = Longitude.ToString().Replace(",", ".")
        Dim endpoint As String = $"AnagraficaStazioni/LeggiStazioneMeteoPerPosizione?Piva={Piva}&TipoSorgente={Tipo_Sorgente.ToString()}&Longitude={strLng}&Latitude={strLat}"

        Dim apiResponse = ChiamataNetCoreDataExchangeGet(endpoint)

        Dim response = JsonConvert.DeserializeObject(Of rispostaStandard(Of StazioneMeteoDto))(apiResponse)

        Return response
    End Function

    Private Function ChiamataNetCore_AggiornaStazione(ByVal Piva As String, ByVal Stazione_Cod As Integer, ByVal Stazione_Nome As String, ByVal Latitude As Decimal?, ByVal Longitude As Decimal?, ByVal Sensori As Dictionary(Of Integer, String)) As rispostaStandard(Of StazioneMeteoDto)

        Dim request As New AggiornaStazioneMeteoRequest With {
            .PIVASuperUser = _objParametri_Server.PivaSuperUser,
            .PIVA = Piva,
            .IdStazione = Stazione_Cod,
            .Descrizione = Stazione_Nome,
            .Latitude = Latitude,
            .Longitude = Longitude,
            .Sensori = New List(Of AggiornaStazioneMeteoRequest.Sensore)
            }

        If Not Sensori Is Nothing Then
            For Each sensore In Sensori
                request.Sensori.Add(New AggiornaStazioneMeteoRequest.Sensore() With {
                .Id = sensore.Key,
                .Descrizione = sensore.Value
                })
            Next
        End If

        Dim payload = JsonConvert.SerializeObject(request)
        Dim apiResponse = ChiamataNetCoreDataExchangePost("AnagraficaStazioni/AggiornaStazioneMeteoProprietaria", payload)

        Dim response = JsonConvert.DeserializeObject(Of rispostaStandard(Of StazioneMeteoDto))(apiResponse)

        Return response
    End Function

    Private Function ChiamataNetCore_EliminaStazione(ByVal Piva As String, ByVal Stazione_Cod As Integer) As RispostaStandard

        Return ChiamataNetCoreDataExchangeDelete($"AnagraficaStazioni/EliminaStazioneMeteoProprietaria?Piva={Piva}&IdStazione={Stazione_Cod}")

    End Function

#End Region

#Region "Gestione Alias"

    Public Function LeggiElencoAlias(piva_superuser As String, piva As String) As List(Of StazioneAlias)

        If Not ControllaUtilizzoNuovoEngineMeteoSuite() Then
            Dim objMeteo As New AgronicaCoreWebService.MeteoNT
            Return objMeteo.LeggiElencoAlias(piva_superuser, piva, _objParametri_Server)
        End If

        Dim apiResponse = ChiamataNetCoreDataExchangeGet($"AliasStazioni/LeggiElencoAliasStazioni?Piva={piva}")

        Dim response = JsonConvert.DeserializeObject(Of rispostaStandard(Of List(Of StazioneMeteoAliasDto)))(apiResponse)

        Dim resultAlias As New List(Of StazioneAlias)

        For Each resAlias In response.RispostaStringa
            resultAlias.Add(New StazioneAlias With {
                            .Id = resAlias.Id,
                            .Name = resAlias.Name,
                            .Lat = resAlias.Lat,
                            .Lng = resAlias.Lng})

        Next

        Return resultAlias

    End Function

    Public Function LeggiStazioneXAlias(piva_superuser As String, piva As String, lat As Decimal, lng As Decimal) As StazioneAliasNew

        If Not ControllaUtilizzoNuovoEngineMeteoSuite() Then
            Dim objMeteo As New AgronicaCoreWebService.MeteoNT
            Return objMeteo.LeggiStazioneXAlias(piva_superuser, piva, lat, lng, _objParametri_Server)
        End If

        Dim result As StazioneAliasNew = Nothing

        Dim strLat As String = lat.ToString().Replace(",", ".")
        Dim strLng As String = lng.ToString().Replace(",", ".")
        Dim apiResponse = ChiamataNetCoreDataExchangeGet($"AliasStazioni/LeggiStazioneMeteoPerAlias?Piva={piva}&Longitude={strLng}&Latitude={strLat}")

        Dim response = JsonConvert.DeserializeObject(Of rispostaStandard(Of StazioneMeteoAliasNewDto))(apiResponse)

        Dim responseData As StazioneMeteoAliasNewDto = response.RispostaStringa
        If Not responseData Is Nothing Then
            result = New StazioneAliasNew With {
                .Id = responseData.Id,
                .Name = responseData.Name,
                .Lat = responseData.Lat,
                .Lng = responseData.Lng,
                .Fornitore = responseData.Fornitore,
                .Dist = responseData.Dist,
                .FlagNew = responseData.FlagNew,
                .AliasName = responseData.AliasName
                }
        End If

        Return result
    End Function

    Public Function UpsertAlias(piva_superuser As String, piva As String, id As Integer, strAlias As String) As StazioneAlias

        If Not ControllaUtilizzoNuovoEngineMeteoSuite() Then
            Dim objMeteo As New AgronicaCoreWebService.MeteoNT
            Return objMeteo.UpsertAlias(piva_superuser, piva, id, strAlias, _objParametri_Server)
        End If

        Dim result As StazioneAliasNew = Nothing

        Dim request As New AggiornaAliasStazioneMeteoRequest With {
            .PIVA = piva,
            .IdStazione = id.ToString(),
            .NomeAlias = strAlias
        }

        Dim payload = JsonConvert.SerializeObject(request)
        Dim apiResponse = ChiamataNetCoreDataExchangePost($"AliasStazioni/CreaOdAggiornaAliasStazione", payload)

        Dim response = JsonConvert.DeserializeObject(Of rispostaStandard(Of StazioneMeteoAliasDto))(apiResponse)

        Dim responseData As StazioneMeteoAliasDto = response.RispostaStringa
        If Not responseData Is Nothing Then
            result = New StazioneAliasNew With {
                .Id = responseData.Id,
                .Name = responseData.Name,
                .Lat = responseData.Lat,
                .Lng = responseData.Lng
                }
        End If

        Return result
    End Function

    Public Function DeleteAlias(piva_superuser As String, piva As String, id As Integer) As Boolean

        If Not ControllaUtilizzoNuovoEngineMeteoSuite() Then
            Dim objMeteo As New AgronicaCoreWebService.MeteoNT
            Return objMeteo.DeleteAlias(piva_superuser, piva, id, _objParametri_Server)
        End If
        Dim result As StazioneAliasNew = Nothing

        Dim apiResponse = ChiamataNetCoreDataExchangeDelete($"AliasStazioni/EliminaAliasStazione?Piva={piva}&IdStazione={id.ToString()}")

        Dim response = JsonConvert.DeserializeObject(Of RispostaStandard)(apiResponse)

        If Not response.RispostaOK Then
            Return False
        End If

        Return True
    End Function

#End Region



    Private Function ChiamataNetCoreDataExchangeGet(endpoint As String) As Object

        CheckNetCoreUtility()

        Dim urlCompleto As String = _URL_NetCoreDataExchange_API +
            If(_URL_NetCoreDataExchange_API.EndsWith("/"), "MeteoSuite/", "/MeteoSuite/") + endpoint

        Return _CallNetCoreUtility.CallNetCoreGet(urlCompleto, _AccessToken, _objParametri_Server)

    End Function

    Private Function ChiamataNetCoreDataExchangePost(endpoint As String, payLoad As String) As Object

        CheckNetCoreUtility()

        Dim urlCompleto As String = _URL_NetCoreDataExchange_API +
            If(_URL_NetCoreDataExchange_API.EndsWith("/"), "MeteoSuite/", "/MeteoSuite/") + endpoint

        Dim content As New System.Net.Http.StringContent(payLoad, Text.Encoding.UTF8, "application/json")

        Return _CallNetCoreUtility.CallNetCorePost(urlCompleto, _AccessToken, payLoad, _objParametri_Server)

    End Function

    Private Function ChiamataNetCoreDataExchangeDelete(endpoint As String) As Object

        CheckNetCoreUtility()

        Dim urlCompleto As String = _URL_NetCoreDataExchange_API +
            If(_URL_NetCoreDataExchange_API.EndsWith("/"), "MeteoSuite/", "/MeteoSuite/") + endpoint

        Return _CallNetCoreUtility.CallNetCoreDelete(urlCompleto, _AccessToken, _objParametri_Server)

    End Function

    Private Sub CheckNetCoreUtility()

        If String.IsNullOrEmpty(_URL_NetCoreDataExchange_API) Then
            _URL_NetCoreDataExchange_API = LeggiUrlNetCoreDataExchangeAPI()
        End If

        If _AccessToken Is Nothing Then
            Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
            Dim objUtentiToken As New Utenti_Token_R
            _AccessToken = objUtentiToken.Leggi_Token("", objParametri_Super_Server)
        End If

        If _CallNetCoreUtility Is Nothing Then
            _CallNetCoreUtility = New CallNetCore()
        End If

    End Sub

    Private Function LeggiUrlNetCoreDataExchangeAPI() As String

        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim url As String = objConfigSiti.Leggi_Valore(0, "GiasOnline_NetCoreDataExchange_API", "", "", _objParametri_Server)

        Dim callNetCore As New CallNetCore
        callNetCore.AggiustaUrl(url, _objParametri_Server)

        Return url
    End Function

    Private Function MapTipoSorgenteGiasToMeteoSuite(ByVal TipoSorgenteGias As Integer) As Integer

        Select Case TipoSorgenteGias
            Case enum_Meteo_Tiposorgente.Aziendali
                Return InData.Engine.MeteoSuite.enum_TipoSorgenteMeteo.AZIENDALI
            Case enum_Meteo_Tiposorgente.RetiPartner
                Return InData.Engine.MeteoSuite.enum_TipoSorgenteMeteo.RETI_PARTNER
            Case enum_Meteo_Tiposorgente.Pubbliche
                Return InData.Engine.MeteoSuite.enum_TipoSorgenteMeteo.PUBBLICHE
            Case enum_Meteo_Tiposorgente.Gias_RER
                Return InData.Engine.MeteoSuite.enum_TipoSorgenteMeteo.GIAS_STAZIONI_REGIONE_ER
            Case enum_Meteo_Tiposorgente.Gias_RER_Quadranti
                Return InData.Engine.MeteoSuite.enum_TipoSorgenteMeteo.GIAS_QUADRANTI_REGIONE_ER
        End Select

        Return InData.Engine.MeteoSuite.enum_TipoSorgenteMeteo.AZIENDALI

    End Function

    Public Function ControllaUtilizzoNuovoEngineMeteoSuite() As Boolean

        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim useNewMeteoEngineValue As String = objConfigSiti.Leggi_Valore(0, "isActiveEngine_MeteoSuite", "", "", _objParametri_Server)

        Return useNewMeteoEngineValue = "1"
    End Function

End Class
