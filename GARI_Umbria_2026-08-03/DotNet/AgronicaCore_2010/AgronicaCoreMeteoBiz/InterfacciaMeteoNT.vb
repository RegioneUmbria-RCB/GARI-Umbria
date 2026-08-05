
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreMeteoDAL
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class InterfacciaMeteoNT

    Private ObjParametri As AgronicaCoreParametri

    Public Sub New(ObjParametri_Server As AgronicaCoreParametri)
        ObjParametri = ObjParametri_Server
    End Sub



    Public Class LeggiStazioneIn
        Public Id_Stazione As Integer
        Public NeedSensors As Boolean
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

    Public Function LeggiStazione(inParam As LeggiStazioneIn) As LeggiStazioneOut

        Dim meteoDAL As New MeteoNT

        Dim ds As DataSet = meteoDAL.LeggiStazione(inParam.Id_Stazione, inParam.NeedSensors, ObjParametri)

        If ds Is Nothing Then

            Return Nothing
        End If

        Dim StazioneDT As DataTable = ds.Tables.Item(0)

        If StazioneDT Is Nothing OrElse StazioneDT.Rows.Count = 0 Then

            Return Nothing
        End If

        Dim StazioneJson = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(StazioneDT, Formatting.None))

        Dim StazOut = JsonConvert.DeserializeObject(Of LeggiStazioneOut)(JsonConvert.SerializeObject(StazioneJson(0)))

        If inParam.NeedSensors Then

            Dim SensoriDT As DataTable = ds.Tables.Item(1)

            StazOut.Sensori = JsonConvert.DeserializeObject(Of List(Of LeggiStazioneOut.Sensore))(JsonConvert.SerializeObject(SensoriDT, Formatting.None))
        End If

        Return StazOut
    End Function



    Public Class SorgenteMeteo
        Public sorgente_cod As Integer
        Public sorgente_des As String
    End Class

    Public Function ElencoSorgentiMeteo(PIVA_Superuser As String) As List(Of SorgenteMeteo)

        Dim sorgenti As New List(Of SorgenteMeteo)

        Dim meteoDAL As New MeteoNT

        Dim dt = meteoDAL.LeggiSorgentiMeteo(PIVA_Superuser, ObjParametri)

        Dim json = JsonConvert.SerializeObject(dt)

        Dim obj = New With {.Id = 0, .Descrizione = ""}
        Dim list = {obj}.ToList()
        list = JsonConvert.DeserializeAnonymousType(json, list)

        For Each e In list
            sorgenti.Add(New SorgenteMeteo With {.sorgente_cod = e.Id, .sorgente_des = e.Descrizione})
        Next

        Return sorgenti
    End Function



    Public Function ElencoStazioniConDistanza(ByVal objParam As JObject) As DataTable

        Dim lat As Double = 0
        Dim lng As Double = 0
        If objParam("lat") IsNot Nothing Then
            lat = CDbl(objParam("lat").ToString)
        End If
        If objParam("lng") IsNot Nothing Then
            lng = CDbl(objParam("lng").ToString)
        End If

        Dim TipoSorgente As enum_Meteo_Tiposorgente
        If objParam("tiposorgente") IsNot Nothing Then
            TipoSorgente = objParam("tiposorgente").ToString
        Else
            TipoSorgente = enum_Meteo_Tiposorgente.RetiPartner
        End If

        Dim Piva_SuperUser As String = ""
        If objParam("piva_superuser") IsNot Nothing Then
            Piva_SuperUser = objParam("piva_superuser").ToString
        End If

        Dim Piva As String = ""
        If objParam("piva") IsNot Nothing Then
            Piva = objParam("piva").ToString
        End If

        Dim meteoDAL As New AgronicaCoreMeteoDAL.MeteoNT
        Dim dtStazioni As DataTable = meteoDAL.LeggiStazioniConDistanza(TipoSorgente, lat, lng, Piva_SuperUser, Piva, ObjParametri)
        '    Utente_Username_client_GIAS,

        Return dtStazioni

    End Function

    Public Function ElencoQuadrantiConDistanza(ByVal objParams As JObject) As DataTable

        Dim utm_x As Integer = CInt(objParams("UTM_X"))
        Dim utm_y As Integer = CInt(objParams("UTM_Y"))
        Dim limiteDistanza As Integer = 0
        If objParams("LimiteDistanza") IsNot Nothing Then
            limiteDistanza = CInt(objParams("LimiteDistanza"))
        End If

        Dim meteoDAL As New AgronicaCoreMeteoDAL.MeteoNT
        Dim dtQuadranti As DataTable = meteoDAL.LeggiQuadrantiConDistanza(utm_x, utm_y, limiteDistanza, ObjParametri)
        Return dtQuadranti

    End Function





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





    Public Class MeteoOutput
        Public Dati As DataTable
        Public Sensori As DataTable
        Public Charts As JArray
        Public Riepilogo As DataTable
        Public RiepilogoSensori As DataTable
    End Class





    Public Function LeggiMeteo(TipoSorgente As enum_Meteo_Tiposorgente,
                               IdStazione As Integer,
                               DataInizio As DateTime,
                               DataFine As DateTime,
                               granularita As MeteoNT.enum_GranularitaDati,
                               SogliaTermica As Decimal,
                               SogliaFabbisognoFreddo As Decimal) As MeteoOutput

        If granularita = MeteoNT.enum_GranularitaDati.Giornalieri OrElse granularita = MeteoNT.enum_GranularitaDati.Orari Then

            'DataInizio ->  ora 00:00:00.000
            'DataFine   ->  ora 23:59:59.000
            DataInizio = New Date(DataInizio.Year, DataInizio.Month, DataInizio.Day, 0, 0, 0, 0)
            DataFine = New Date(DataFine.Year, DataFine.Month, DataFine.Day, 23, 59, 59, 0)
        End If

        Dim meteoDAL As New MeteoNT

        Dim ds As DataSet = meteoDAL.LeggiDati(TipoSorgente, IdStazione, DataInizio, DataFine, granularita, SogliaTermica, SogliaFabbisognoFreddo, ObjParametri)

        Return DataSet2MeteoOutput(ds, granularita = MeteoNT.enum_GranularitaDati.Orari)
    End Function

    Public Function LeggiMeteo_V2(pmeteo As ParametriMeteo) As MeteoOutput

        Dim meteoDAL As New MeteoNT

        If pmeteo.GranularitaDati = Enum_TimeGranularity.Daily OrElse pmeteo.GranularitaDati = Enum_TimeGranularity.Hourly Then
            pmeteo.StartPeriod = pmeteo.StartPeriod.Date
            pmeteo.EndPeriod = pmeteo.EndPeriod.Date.AddDays(1).AddSeconds(-1)
        End If

        Dim ds As DataSet = meteoDAL.LeggiDati_V2(pmeteo, ObjParametri)

        Return DataSet2MeteoOutput(ds, pmeteo.GranularitaDati = Enum_TimeGranularity.Hourly)
    End Function


    Private Function DataSet2MeteoOutput(ds As DataSet, isHourly As Boolean) As MeteoOutput

        If ds Is Nothing Then
            Return Nothing
        End If

        Dim datimeteo As DataTable = ds.Tables("DatiMeteo_NT")
        Dim dt_sensori As DataTable = ds.Tables("DatiMeteo_NT1")
        Dim riepilogo As DataTable = ds.Tables("DatiMeteo_NT2")
        Dim riepilogoSensori As DataTable = ds.Tables("DatiMeteo_NT3")

        If datimeteo Is Nothing OrElse datimeteo.Rows.Count = 0 Then
            Return Nothing
        End If

        Dim mOut As New MeteoOutput With {
            .Dati = datimeteo,
            .Sensori = dt_sensori,
            .Charts = New JArray,
            .Riepilogo = riepilogo,
            .RiepilogoSensori = riepilogoSensori
        }

        Dim ChartList As New List(Of MeteoChart)
        Dim grouped_dict As New Dictionary(Of Integer, MeteoChart)
        Dim ungrouped_dict As New Dictionary(Of Integer, List(Of SerieData))
        Dim derived As New List(Of SerieData)

        For Each s In dt_sensori.Rows

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

            If kvp.Key <> 0 Then
                'tiposensore non generico: lo stesso chart per tutti i sensori

                Dim chart As New MeteoChart
                For Each sdr In kvp.Value
                    chart.Add(sdr)
                Next
                ChartList.Add(chart)
            Else

                'Un chart per ogni sensore
                For Each sdr In kvp.Value
                    Dim chart As New MeteoChart
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
            mOut.Charts.Add(chart.MakeChart(isHourly))
        Next

        Return mOut
    End Function



    Public Class MeteoOutputStazione
        Public Stazione As RisultatoMeteoRiepilogo.Stazione
        Public Meteo As MeteoOutput
        Sub New()
            Stazione = New RisultatoMeteoRiepilogo.Stazione
        End Sub
    End Class


    Public Function LeggiMeteoStazione(piva_superuser As String, piva As String, TipoSorgente As enum_Meteo_Tiposorgente, IdStazione As Integer, NumOre As Integer) As MeteoOutputStazione

        Dim meteoDAL As New MeteoNT

        Dim dtInfo = meteoDAL.LeggiInfoStazione(piva_superuser, piva, TipoSorgente, IdStazione, ObjParametri)

        If dtInfo Is Nothing OrElse dtInfo.Rows.Count = 0 Then

            Return Nothing
        End If

        Dim mos As New MeteoOutputStazione

        mos.Stazione.Descrizione = dtInfo.Rows(0)("Stazione_Des").ToString
        mos.Stazione.UltimoAggiornamento = CDate(dtInfo.Rows(0)("UltimoAggiornamento")).ToString("o")

        Dim DataFine As DateTime = CDate(dtInfo.Rows(0)("UltimoAggiornamento"))
        Dim DataInizio As DateTime = DataFine.AddHours(-1 * NumOre)
        Dim Oggi As DateTime = Date.Now().Date
        If DataInizio > Oggi Then
            'Ho il dato con previsione > NumOre
            DataInizio = Oggi
        End If
        DataInizio = New DateTime(DataInizio.Year, DataInizio.Month, DataInizio.Day, DataInizio.Hour, 0, 0)

        Dim mout = LeggiMeteo(TipoSorgente, IdStazione, DataInizio, DataFine, MeteoNT.enum_GranularitaDati.Sorgente, 0, 0)

        If mout Is Nothing Then

            Return Nothing
        End If

        Dim baseUnit As String = "minutes"

        If TipoSorgente = enum_Meteo_Tiposorgente.Gias_RER OrElse
            TipoSorgente = enum_Meteo_Tiposorgente.Gias_RER_Quadranti OrElse
            TipoSorgente = enum_Meteo_Tiposorgente.Pubbliche Then

            baseUnit = "hours"
        End If

        For Each ch In mout.Charts
            Dim horizAxis As New JObject
            horizAxis("field") = ch("horizAxis")
            horizAxis("baseUnit") = baseUnit
            ch("horizAxis") = horizAxis
        Next

        mos.Meteo = mout

        Return mos
    End Function


    Public Class MeteoOutputStazione2
        Public Stazione As RisultatoMeteoMonitoraggioSuolo.Stazione
        Public Meteo As MeteoOutput
        Sub New()
            Stazione = New RisultatoMeteoMonitoraggioSuolo.Stazione
        End Sub
    End Class


    Public Function LeggiMeteoStazione2(piva_superuser As String, piva As String, TipoSorgente As enum_Meteo_Tiposorgente, IdStazione As Integer, NumGG As Integer) As MeteoOutputStazione2

        Dim meteoDAL As New MeteoNT

        Dim dtInfo = meteoDAL.LeggiInfoStazione(piva_superuser, piva, TipoSorgente, IdStazione, ObjParametri)

        If dtInfo Is Nothing OrElse dtInfo.Rows.Count = 0 Then

            Return Nothing
        End If

        Dim Oggi As DateTime = Date.Now().Date
        Dim DataFine As DateTime = CDate(dtInfo.Rows(0)("UltimoAggiornamento"))
        If DataFine > Oggi Then
            'Ho il dato con previsione
            DataFine = Oggi
        End If
        Dim DataInizio As DateTime = DataFine.AddDays(-1 * NumGG).Date

        Dim mos As New MeteoOutputStazione2

        mos.Stazione.Descrizione = dtInfo.Rows(0)("Stazione_Des").ToString

        Dim mout = LeggiMeteo(TipoSorgente, IdStazione, DataInizio, DataFine, MeteoNT.enum_GranularitaDati.Orari, 0, 0)

        If mout Is Nothing Then

            Return Nothing
        End If

        mos.Meteo = mout

        Return mos
    End Function





    Public Class RisultatoLetturaMeteoDSS
        Public TblMeteo As DataTable
        Public TblSensori As DataTable
        Public TblInfo As DataTable
    End Class

    Public Function LeggiMeteoPerDSS(params As ParametriMeteoDSS) As RisultatoLetturaMeteoDSS

        params.DataInizio = params.DataInizio.Date
        params.DataFine = params.DataFine.Date.AddDays(1).AddSeconds(-1)

        Dim meteoDAL As New MeteoNT

        Dim ds As DataSet = meteoDAL.LeggiDatiPerDSS(params, ObjParametri)

        If ds Is Nothing Then
            Return Nothing
        End If

        Return New RisultatoLetturaMeteoDSS With {
            .TblMeteo = ds.Tables("DatiMeteo_NT"),
            .TblSensori = ds.Tables("DatiMeteo_NT1"),
            .TblInfo = ds.Tables("DatiMeteo_NT2")
        }
    End Function

    Public Class PioggiaGG
        Public DataOra As DateTime
        Public Prec As Decimal
        Public Temp As Decimal
        Public TempMin As Decimal
        Public TempMax As Decimal
        Public UmRel As Decimal
        Sub New(ByVal dr As DataRow, ByVal colonne As List(Of String))

            DataOra = Convert.ToDateTime(dr("DataOra"))

            Dim val As Decimal

            For Each col In colonne

                val = If(IsDBNull(dr(col)), 0, Convert.ToDecimal(dr(col)))

                CallByName(Me, col, CallType.Set, val)

            Next
        End Sub

    End Class

    Public Function LeggiMeteoPerPiogge(ByVal TipoSorgente As enum_Meteo_Tiposorgente, ByVal IdStazione As Integer, ByVal DataInizio As DateTime, ByVal DataFine As DateTime) As List(Of PioggiaGG)

        DataInizio = New Date(DataInizio.Year, DataInizio.Month, DataInizio.Day, 0, 0, 0, 0)
        DataFine = New Date(DataFine.Year, DataFine.Month, DataFine.Day, 23, 59, 59, 0)

        Dim meteoDAL As New MeteoNT

        Dim datimeteo As DataTable = meteoDAL.LeggiDatiPerPiogge(TipoSorgente, IdStazione, DataInizio, DataFine, ObjParametri)

        If datimeteo Is Nothing OrElse datimeteo.Rows.Count = 0 Then
            Return Nothing
        End If

        If Not datimeteo.Columns.Contains("Prec") Then
            Return Nothing
        End If

        Dim colonne As New List(Of String)
        For Each col In datimeteo.Columns
            If col.DataType() <> System.Type.GetType("System.DateTime") Then
                colonne.Add(col.ToString)
            End If
        Next

        Dim elencoGG As New List(Of PioggiaGG)

        For Each row In datimeteo.Rows
            elencoGG.Add(New PioggiaGG(row, colonne))
        Next

        Return elencoGG
    End Function

    Public Function LeggiMeteoPerIrrigazione(ByVal TipoSorgente As enum_Meteo_Tiposorgente, ByVal IdStazione As Integer, ByVal DataInizio As DateTime, ByVal DataFine As DateTime) As JObject

        DataInizio = New Date(DataInizio.Year, DataInizio.Month, DataInizio.Day, 0, 0, 0, 0)
        DataFine = New Date(DataFine.Year, DataFine.Month, DataFine.Day, 23, 59, 59, 0)

        Dim meteoDAL As New MeteoNT

        Dim dm_set As DataSet = meteoDAL.LeggiDatiPerIrrigazione(TipoSorgente, IdStazione, DataInizio, DataFine, ObjParametri)

        If dm_set Is Nothing Then
            Return Nothing
        End If

        Dim datimeteo As DataTable = dm_set.Tables().Item(0)
        If datimeteo Is Nothing OrElse datimeteo.Rows.Count = 0 Then
            Return Nothing
        End If

        If Not datimeteo.Columns.Contains("RainMM") Then
            Return Nothing
        End If

        Dim dt_cols As DataTable = dm_set.Tables().Item(1)

        Dim resObj As New JObject(New JProperty("model"), New JProperty("data"))

        Dim model As New JArray
        model.Add(New JObject(New JProperty("field", "Date"), New JProperty("label", "Date")))
        For Each col In dt_cols.Rows
            model.Add(New JObject(New JProperty("field", col("col").ToString), New JProperty("label", col("eti").ToString)))
        Next

        Dim data As New JArray

        For Each dm In datimeteo.Rows

            Dim d As New JObject

            For Each dc In datimeteo.Columns

                If dc.DataType() = System.Type.GetType("System.DateTime") Then

                    d(dc.ToString) = Convert.ToDateTime(dm(dc.ToString))

                Else

                    If Not IsDBNull(dm(dc.ToString)) Then

                        d(dc.ToString) = Convert.ToDecimal(dm(dc.ToString))
                    End If
                End If
            Next

            data.Add(d)
        Next

        resObj("model") = model
        resObj("data") = data

        Return resObj

        'Dim colDict As New Dictionary(Of String, String)
        'For Each col In datimeteo.Columns
        '    If col.DataType() <> System.Type.GetType("System.DateTime") Then
        '        Dim colName As String = col.ToString
        '        If Not colName.StartsWith("UT_") Then
        '            colDict.Add(colName, colName)
        '        Else
        '            If Not colDict.ContainsKey("UmTerr") Then
        '                colDict.Add("UmTerr", colName)
        '            End If
        '        End If
        '    End If
        'Next
#If False Then

            Public Class IrriGG
        Public Giorno As DateTime
        Public Tmed As Decimal?
        Public Tmin As Decimal?
        Public Tmax As Decimal?
        Public RainMM As Decimal?
        Public RainH As Integer?
        Public UmTerr As Decimal?
        Sub New(ByVal dr As DataRow, ByVal colDict As Dictionary(Of String, String))

            Giorno = Convert.ToDateTime(dr("Date"))

            Dim val As Decimal

            For Each col In colDict
                If Not IsDBNull(dr(col.Value)) Then
                    val = Convert.ToDecimal(dr(col.Value))
                    CallByName(Me, col.Key, CallType.Set, val)
                End If
            Next
        End Sub

    End Class

#End If
        'Dim elencoGG As New List(Of IrriGG)

        'For Each row In datimeteo.Rows
        '    elencoGG.Add(New IrriGG(row, colDict))
        'Next

        'Return elencoGG
    End Function

    Public Function LeggiAnagraficaStazioni(ByVal piva_superuser As String, ByVal piva As String, ByVal id_stazione As Integer) As DataTable

        Dim meteoDAL As New MeteoNT

        Dim dtAnag = meteoDAL.LeggiAnagraficaStazioni(piva_superuser, piva, id_stazione, ObjParametri)

        If dtAnag Is Nothing OrElse dtAnag.Rows.Count = 0 Then

            Return Nothing

        End If

        Return dtAnag
    End Function

    Public Function AggiornaAnagraficaStazione(ByVal piva_superuser As String, ByVal piva As String, ByVal objAnag As JObject) As DataTable

        Dim anagStaz As New MeteoNT.AnagStazione With {
            .Id = CInt(objAnag("Staz_Id")),
            .Nome = objAnag("Staz_Nome").ToString
        }
        If objAnag("Staz_Geo") IsNot Nothing Then
            anagStaz.Lat = Convert.ToDecimal(objAnag("Staz_Geo")("Lat"))
            anagStaz.Lng = Convert.ToDecimal(objAnag("Staz_Geo")("Lng"))
        End If
        Dim arrSensori As JArray = objAnag("Sensori")

        For Each objSens In arrSensori

            Dim outputConfig As String = ""
            Dim objConfig = objSens("OutputConfig")
            If objConfig.Type <> JTokenType.Null AndAlso objConfig.HasValues Then
                outputConfig = objConfig.ToString(Formatting.None)
            End If

            anagStaz.Sensori.Add(New MeteoNT.AnagStazione.Sensore With {
                                 .Id = objSens("Id_Sensore"),
                                 .Etichetta = objSens("Etichetta"),
                                 .OutputConfig = outputConfig
                                 })
        Next
        If objAnag("Staz_Note_Visibilita") IsNot Nothing Then
            anagStaz.NoteVisibilita = objAnag("Staz_Note_Visibilita").ToString
        End If

        Dim meteoDAL As New MeteoNT

        Dim id_stazione As Integer = meteoDAL.AggiornaAnagraficaStazione(piva_superuser, piva, anagStaz, ObjParametri)

        If id_stazione = 0 Then

            Return Nothing
        End If

        Return LeggiAnagraficaStazioni(piva_superuser, piva, id_stazione)
    End Function

    Public Function LeggiVisibilitaStazione(ByVal Id_Stazione As Integer) As DataTable

        Dim meteoDAL As New MeteoNT

        Dim dt = meteoDAL.LeggiVisibilitaStazione(Id_Stazione, ObjParametri)

        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then

            Return dt
        End If

        Return Nothing
    End Function

    Public Function EliminaStazione(ByVal Id_Stazione As Integer) As Boolean

        Dim meteoDAL As New MeteoNT

        Return meteoDAL.EliminaStazione(Id_Stazione, ObjParametri)
    End Function

    Public Function LeggiStazioniAutorizzate(piva_superuser As String, piva As String, tipo_sorgente As Integer, lat As Decimal, lng As Decimal) As DataTable

        Dim meteoDAL As New MeteoNT

        Dim coord As MeteoNT.LatLng = Nothing

        If Math.Abs(lat) > 0.001 AndAlso Math.Abs(lng) > 0.001 Then

            coord = New MeteoNT.LatLng With {
                .Lat = lat,
                .Lng = lng
            }
        End If

        Dim dtAnag = meteoDAL.LeggiStazioniAutorizzate(piva_superuser, piva, tipo_sorgente, coord, ObjParametri)

        If dtAnag IsNot Nothing AndAlso dtAnag.Rows.Count > 0 Then

            Return dtAnag
        End If

        Return Nothing
    End Function

    Public Class StazCodes
        Public TipoSorgente As enum_Meteo_Tiposorgente
        Public CodiciStazione As List(Of Integer)
        Public Sub New(ByVal _tiposorgente As enum_Meteo_Tiposorgente)
            TipoSorgente = _tiposorgente
            CodiciStazione = New List(Of Integer)
        End Sub
    End Class
    Public Function LeggiElencoStazioni(ByVal piva_superuser As String, ByVal piva As String, ByVal staz_codes As List(Of StazCodes)) As DataTable

        Dim meteoDAL As New MeteoNT

        Dim dtElenco As DataTable = Nothing

        For Each sc In staz_codes

            Dim stazXtipo = meteoDAL.LeggiElencoStazioni(piva_superuser, piva, sc.TipoSorgente, sc.CodiciStazione.ToArray, True, "§", ObjParametri)

            If dtElenco Is Nothing Then

                dtElenco = stazXtipo
            Else

                dtElenco.Merge(stazXtipo, True)
            End If
        Next

        If dtElenco Is Nothing OrElse dtElenco.Rows.Count = 0 Then

            Return Nothing
        End If

        Return dtElenco
    End Function


    Public Function StazioniDaLatLng(lista As List(Of MeteoNT.IdLatLng)) As List(Of MeteoNT.IdLatLng)

        Dim meteoDAL As New MeteoNT

        Dim outList = meteoDAL.StazioniPubblicheDaLatLng(lista, ObjParametri)

        Return outList
    End Function

    Public Function LeggiElencoTipiSensorePerStazione(ByVal id_stazione As Integer) As DataTable
        Dim meteoDAL As New MeteoNT

        Dim dtElenco As DataTable = Nothing

        dtElenco = meteoDAL.LeggiTipoSensoriPerStazione(id_stazione, ObjParametri)

        If dtElenco Is Nothing OrElse dtElenco.Rows.Count = 0 Then

            Return Nothing
        End If

        Return dtElenco
    End Function





    Public Function LeggiElencoAlias(piva_superuser As String, piva As String) As DataTable

        Dim meteoDAL As New MeteoNT

        Dim dtElenco As DataTable = meteoDAL.LeggiElencoAlias(piva_superuser, piva, ObjParametri)

        Return dtElenco
    End Function



    Public Function LeggiStazioneXAlias(piva_superuser As String, piva As String, lat As Decimal, lng As Decimal) As DataTable

        Dim meteoDAL As New MeteoNT

        Dim dtElenco As DataTable = meteoDAL.LeggiStazioneXAlias(piva_superuser, piva, lat, lng, ObjParametri)

        Return dtElenco
    End Function


    Public Function UpsertAlias(piva_superuser As String, piva As String, id As Integer, strAlias As String) As DataTable

        Dim meteoDAL As New MeteoNT

        Dim dtElenco As DataTable = meteoDAL.UpsertAlias(piva_superuser, piva, id, strAlias, ObjParametri)

        Return dtElenco
    End Function


    Public Function DeleteAlias(piva_superuser As String, piva As String, id As Integer) As DataTable

        Dim meteoDAL As New MeteoNT

        Dim dtElenco As DataTable = meteoDAL.DeleteAlias(piva_superuser, piva, id, ObjParametri)

        Return dtElenco
    End Function


End Class
