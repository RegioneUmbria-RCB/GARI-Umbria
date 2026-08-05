Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreDTOStd.InData.IoT

Public Class InterfacciaIOT
    Private ObjParametri As AgronicaCoreParametri

    Public Sub New(ObjParametri_Server As AgronicaCoreParametri)
        ObjParametri = ObjParametri_Server
    End Sub



    'Public Class LeggiDispositiviIn
    '    Public Id_Dispositivo As Integer
    '    Public NeedSensors As Boolean
    'End Class

    'Public Class LeggiDispositivoOut
    '    Public Id As Integer
    '    Public Dispositivo As String
    '    Public Lat As Double?
    '    Public Lng As Double?
    '    Public FlagReale As Boolean
    '    Public RifFornitore As String
    '    Public Fornitore As String

    '    Public Class Sensore
    '        Public Id As Integer
    '        Public Sensore As String
    '        Public Tipo As String
    '        Public UM As String
    '        Public Id_DispositivoOrigine As Integer?
    '        Public DispositivoOrigine As String
    '        Public Last_Update As DateTime?
    '    End Class

    '    Public Sensori As List(Of Sensore)
    'End Class

    Public Function LeggiDispositivo(inParam As LeggiDispositivoIn) As LeggiDispositivoOut

        Dim iotDAL As New AgronicaCoreIOTDAL.IOT

        Dim ds As DataSet = iotDAL.LeggiDispositivo(inParam.id_dispositivo, inParam.needSensors, ObjParametri)

        If ds Is Nothing Then

            Return Nothing
        End If

        Dim StazioneDT As DataTable = ds.Tables.Item(0)

        If StazioneDT Is Nothing OrElse StazioneDT.Rows.Count = 0 Then

            Return Nothing
        End If

        Dim StazioneJson = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(StazioneDT, Formatting.None))

        Dim StazOut = JsonConvert.DeserializeObject(Of LeggiDispositivoOut)(JsonConvert.SerializeObject(StazioneJson(0)))

        If inParam.needSensors Then

            Dim SensoriDT As DataTable = ds.Tables.Item(1)

            StazOut.Sensori = JsonConvert.DeserializeObject(Of List(Of SensoreObj))(JsonConvert.SerializeObject(SensoriDT, Formatting.None))
        End If

        Return StazOut
    End Function

    Public Function LeggiElencoTipiSensorePerDispositivo(ByVal id_dispositivo As Integer) As DataTable
        Dim iotDAL As New AgronicaCoreIOTDAL.IOT

        Dim dtElenco As DataTable = Nothing

        dtElenco = iotDAL.LeggiTipoSensoriPerDispositivo(id_dispositivo, ObjParametri)

        If dtElenco Is Nothing OrElse dtElenco.Rows.Count = 0 Then

            Return Nothing
        End If

        Return dtElenco
    End Function

    Public Function LeggiDispositiviAutorizzati(piva_superuser As String, piva As String, tipo_sorgente As Integer, lat As Decimal, lng As Decimal) As DataTable

        Dim iotDAL As New AgronicaCoreIOTDAL.IOT

        Dim coord As AgronicaCoreIOTDAL.IOT.LatLng = Nothing

        If Math.Abs(lat) > 0.001 AndAlso Math.Abs(lng) > 0.001 Then

            coord = New AgronicaCoreIOTDAL.IOT.LatLng With {
                .Lat = lat,
                .Lng = lng
            }
        End If

        Dim dtAnag = iotDAL.LeggiDispositiviAutorizzati(piva_superuser, piva, tipo_sorgente, coord, ObjParametri)

        If dtAnag IsNot Nothing AndAlso dtAnag.Rows.Count > 0 Then

            Return dtAnag
        End If

        Return Nothing
    End Function
    Public Function LeggiDati(TipoSorgente As enum_IoT_Tiposorgente,
                               IdDispositivo As Integer,
                               DataInizio As DateTime,
                               DataFine As DateTime,
                               granularita As AgronicaCoreIOTDAL.IOT.enum_GranularitaDati,
                               SogliaTermica As Decimal,
                               SogliaFabbisognoFreddo As Decimal) As IOT_Output

        If granularita = AgronicaCoreIOTDAL.IOT.enum_GranularitaDati.Giornalieri OrElse granularita = AgronicaCoreIOTDAL.IOT.enum_GranularitaDati.Orari Then

            'DataInizio ->  ora 00:00:00.000
            'DataFine   ->  ora 23:59:59.000
            DataInizio = New Date(DataInizio.Year, DataInizio.Month, DataInizio.Day, 0, 0, 0, 0)
            DataFine = New Date(DataFine.Year, DataFine.Month, DataFine.Day, 23, 59, 59, 0)
        End If

        Dim iotDAL As New AgronicaCoreIOTDAL.IOT

        Dim ds As DataSet = iotDAL.LeggiDati(TipoSorgente, IdDispositivo, DataInizio, DataFine, granularita, SogliaTermica, SogliaFabbisognoFreddo, ObjParametri)

        If ds Is Nothing Then
            Return Nothing
        End If

        Dim datiiot As DataTable = ds.Tables("Dati_IOT")
        Dim dt_sensori As DataTable = ds.Tables("Dati_IOT1")
        Dim riepilogo As DataTable = ds.Tables("Dati_IOT2")
        Dim riepilogoSensori As DataTable = ds.Tables("Dati_IOT3")

        If datiiot Is Nothing OrElse datiiot.Rows.Count = 0 Then
            Return Nothing
        End If

        Dim mOut As New IOT_Output With {
            .Dati = datiiot,
            .Sensori = dt_sensori,
            .Charts = New JArray,
            .Riepilogo = riepilogo,
            .RiepilogoSensori = riepilogoSensori
        }

        Dim ChartList As New List(Of IoT_Chart)
        Dim grouped_dict As New Dictionary(Of Integer, IoT_Chart)
        Dim ungrouped_dict As New Dictionary(Of Integer, List(Of SerieData))
        Dim derived As New List(Of SerieData)

        For Each s In dt_sensori.Rows

            Dim sdr As New SerieData(s)
            Dim gruppo = sdr.Gruppo

            If gruppo > 0 Then

                If grouped_dict.ContainsKey(gruppo) Then

                    grouped_dict(gruppo).Add(sdr)
                Else

                    Dim chart As New IoT_Chart
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

            If kvp.Key <> 0 Then
                'tiposensore non generico: lo stesso chart per tutti i sensori

                Dim chart As New IoT_Chart
                For Each sdr In kvp.Value
                    chart.Add(sdr)
                Next
                ChartList.Add(chart)
            Else

                'Un chart per ogni sensore
                For Each sdr In kvp.Value
                    Dim chart As New IoT_Chart
                    chart.Add(sdr)
                    ChartList.Add(chart)
                Next
            End If
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
            mOut.Charts.Add(chart.MakeChart(granularita))
        Next

        Return mOut
    End Function


    Private Class IoT_Chart
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

        Public Function MakeChart(granularita As AgronicaCoreIOTDAL.IOT.enum_GranularitaDati) As JObject

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

            If granularita = AgronicaCoreIOTDAL.IOT.enum_GranularitaDati.Orari Then

                objChart.Add("horizAxis", New JObject(New JProperty("field", "DataOra"), New JProperty("baseUnit", "hours")))
            Else

                objChart.Add("horizAxis", "DataOra")
            End If

            objChart.Add("series", arrSeries)
            objChart.Add("axis", arrAxis)

            Return objChart
        End Function
    End Class

    Private Class SerieData
        Public ReadOnly Gruppo As Integer
        Public ReadOnly IsDerived As Boolean
        Public ReadOnly TipoSensore As Integer
        Public ReadOnly IdSensore As Integer
        Private _nomeSensore As String
        Private ReadOnly _nomeSensore_Postfix As String
        Private ReadOnly _misura As String
        Private ReadOnly _um As String
        Private ReadOnly _fieldKey As String
        Private ReadOnly _serieType As String
        Private _serieColor As String
        Private ReadOnly _serieOpacity As Decimal
        Private ReadOnly _funAggreg As String
        Private ReadOnly _serieFields As JObject

        Private ReadOnly _derived As List(Of SerieData)

        Public Sub New(r As DataRow)

            _fieldKey = "field"
            Dim fieldValue As String = r("NomeColonna").ToString
            Dim tmparr As String() = fieldValue.Split("_")

            Gruppo = -1
            IsDerived = fieldValue.StartsWith("SD")
            TipoSensore = CInt(r("TipoSensore").ToString)
            IdSensore = CInt(tmparr(tmparr.Length - 1))
            _nomeSensore = r("Sensore").ToString
            _misura = r("Misura").ToString
            _um = r("UM").ToString
            _funAggreg = r("FunAggreg").ToString

            _serieType = "line"
            _serieColor = ""
            _serieOpacity = 1
            _nomeSensore_Postfix = ""

            Dim strConfig As String = r("Configurazione").ToString
            If Not String.IsNullOrEmpty(strConfig) Then

                Dim objConfig = JObject.Parse(strConfig)

                If objConfig("gruppo") IsNot Nothing Then
                    Gruppo = CInt(objConfig("gruppo"))
                End If

                If objConfig("serie") IsNot Nothing Then
                    _serieType = objConfig("serie").ToString
                End If

                If objConfig("colore") IsNot Nothing Then
                    _serieColor = objConfig("colore")
                End If

                If objConfig("opacity") IsNot Nothing Then
                    _serieOpacity = Convert.ToDecimal(objConfig("opacity"), Globalization.CultureInfo.InvariantCulture)
                End If

                If objConfig("sensor_postfix") IsNot Nothing Then
                    _nomeSensore_Postfix = objConfig("sensor_postfix").ToString
                End If

                If objConfig("fieldProp") IsNot Nothing Then
                    _fieldKey = objConfig("fieldProp").ToString
                End If
            End If

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

            If Not String.IsNullOrEmpty(_misura) Then
                axis_name = "M_" & _misura.Replace(" ", "_")
                axis_title = _misura
            End If

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
End Class
