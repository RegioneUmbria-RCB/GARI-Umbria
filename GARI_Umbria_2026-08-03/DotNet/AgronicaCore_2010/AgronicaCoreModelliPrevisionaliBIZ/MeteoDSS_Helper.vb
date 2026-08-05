
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreMeteoDAL
Imports Newtonsoft.Json


Public Class MeteoDSS_Helper

    Public Enum Enum_MeteoStatus
        StatusNone = 0
        StatusInformation = 1
        StatusWarning = 2
        StatusError = 3
    End Enum

    Private ReadOnly _objParametri As AgronicaCoreParametri
    Private _datiMeteo As MeteoReadOnlyList
    Private _lat As Decimal?
    Private _lng As Decimal?
    Private _timezone As String
    Private _status As Enum_MeteoStatus
    Private _statusMsg As String
    Private _elencoPiogge As PioggeReadOnlyList

    Public ReadOnly Property DatiMeteo As MeteoReadOnlyList
        Get
            Return _datiMeteo
        End Get
    End Property

    Public ReadOnly Property Lat As Decimal
        Get
            If _lat.HasValue Then
                Return _lat.Value
            End If
            Return 44.168704206487483
        End Get
    End Property

    Public ReadOnly Property Lng As Decimal
        Get
            If _lng.HasValue Then
                Return _lng.Value
            End If
            Return 12.268798020116076
        End Get
    End Property

    Public ReadOnly Property Timezone As String
        Get
            If Not String.IsNullOrEmpty(_timezone) Then
                Return _timezone
            End If
            Return "W. Europe Standard Time"
        End Get
    End Property

    Public ReadOnly Property Status As Enum_MeteoStatus
        Get
            Return _status
        End Get
    End Property

    Public ReadOnly Property StatusMsg As String
        Get
            Return _statusMsg
        End Get
    End Property

    Public ReadOnly Property ElencoPiogge As PioggeReadOnlyList
        Get
            If _elencoPiogge Is Nothing Then

                Dim pioggeList As New List(Of InfoPioggia)

                If _datiMeteo IsNot Nothing AndAlso _datiMeteo.Any() Then

                    Dim isNew As Boolean = True
                    Dim inizioEvento As DateTime
                    Dim cumulato_mm As Decimal
                    Dim durata_hh As Integer

                    For Each dm In _datiMeteo.Reverse()

                        If dm.Forecast Then

                            Continue For
                        End If

                        If dm.Prec Then

                            If isNew Then

                                inizioEvento = dm.DataOra
                                cumulato_mm = dm.Prec
                                durata_hh = 1
                                isNew = False
                            Else

                                inizioEvento = dm.DataOra
                                durata_hh += 1
                                cumulato_mm += dm.Prec
                            End If
                        Else
                            If Not isNew Then

                                pioggeList.Add(New InfoPioggia(inizioEvento, cumulato_mm, durata_hh))

                                isNew = True
                            End If
                        End If
                    Next
                End If

                _elencoPiogge = New PioggeReadOnlyList(pioggeList)
            End If

            Return _elencoPiogge
        End Get
    End Property


    Public Sub New(objParametri As AgronicaCoreParametri)
        _objParametri = objParametri
        _datiMeteo = Nothing
        _lat = Nothing
        _lng = Nothing
        _timezone = ""
        _status = Enum_MeteoStatus.StatusNone
        _statusMsg = ""
        _elencoPiogge = Nothing
    End Sub

    Public Function Leggi(params As ParametriMeteoDSS) As Boolean

        _datiMeteo = Nothing
        _lat = Nothing
        _lng = Nothing
        _timezone = ""
        _status = Enum_MeteoStatus.StatusNone
        _statusMsg = ""
        _elencoPiogge = Nothing

        '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        '+++ DEBUG ONLY +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        '_datiMeteo = New MeteoReadOnlyList(DebugReadXLS(), params.Tipo_Sorgente, True)
        'Return True
        '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        Dim Meteo As New AgronicaCoreMeteoBiz.InterfacciaMeteoNT(_objParametri)

        Dim ris = Meteo.LeggiMeteoPerDSS(params)

        If ris Is Nothing Then
            SetError(String.Format(
                     My.Resources.AgronicaCoreModelliPrevisionaliBIZ.ModelliPrevisionaliCalcolatore_mancanzaMisurePerDatiMeteoSorgente,
                     "<br/>",
                     params.DataInizio.ToString("d MMM yyyy"),
                     params.DataFine.ToString("d MMM yyyy")
                     ))
            Return False
        End If

        If ris.TblMeteo Is Nothing OrElse ris.TblMeteo.Rows.Count = 0 Then
            SetError(String.Format(
                     My.Resources.AgronicaCoreModelliPrevisionaliBIZ.ModelliPrevisionaliCalcolatore_mancanzaDatiMeteoSorgente,
                     params.DataInizio.ToString("d MMM yyyy"),
                     params.DataFine.ToString("d MMM yyyy")
                     ))
            Return False
        End If

        Dim jsonSensors = JsonConvert.SerializeObject(ris.TblSensori)
        Dim sensorList = JsonConvert.DeserializeObject(Of List(Of SensorRow))(jsonSensors)

        Dim missing = sensorList.Where(Function(s) Not s.Found).Select(Function(s) s.Sensor.ToUpper())

        'obbligatori per tutti i modelli
        If missing.Intersect({"TEMP", "PREC", "RELHUM"}).Any() Then
            SetError(My.Resources.AgronicaCoreModelliPrevisionaliBIZ.ModelliPrevisionaliCalcolatore_mancanzaMisurePerDatiMeteo)
            Return False
        End If

        Dim missingBagn As Boolean = missing.Contains("LW")

        Dim jsonMeteo = JsonConvert.SerializeObject(ris.TblMeteo)
        Dim meteoList = JsonConvert.DeserializeObject(Of List(Of MeteoRow))(jsonMeteo)

        Dim emptyItem As New MeteoDSSItem
        Dim meteoItems As New List(Of MeteoDSSItem)

        For Each mrow In meteoList
            meteoItems.Add(emptyItem.Clone(mrow))
        Next

        _datiMeteo = New MeteoReadOnlyList(meteoItems, params.Sorgente, Not missingBagn)

        If ris.TblInfo IsNot Nothing AndAlso ris.TblInfo.Rows.Count > 0 Then

            Dim infoRow = ris.TblInfo.Rows(0)

            If Not IsDBNull(infoRow("Lat")) AndAlso Not IsDBNull(infoRow("Lng")) Then

                _lat = Convert.ToDecimal(infoRow("Lat"), Globalization.CultureInfo.InvariantCulture)
                _lng = Convert.ToDecimal(infoRow("Lng"), Globalization.CultureInfo.InvariantCulture)
                _timezone = infoRow("Timezone").ToString
            End If
        End If

        If params.Forecast Then

            If meteoItems.Last.Forecast Then

                SetInformation($"Dati meteo previsionali al {meteoItems.Last.DataOra:d MMM yyyy}")
            Else

                SetWarning($"Dati meteo previsionali non acquisiti (stazione non geolocalizzata)")
            End If

            params.DataFine = meteoItems.Last.DataOra.Date
        Else

            If DateDiff(DateInterval.Hour, meteoItems.Last.DataOra, params.DataFine) > 24 Then
                SetWarning(String.Format(My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Indicatore_DatiMeteoFinoAl, meteoItems.Last.DataOra.ToShortDateString()))
            End If
        End If

        Return True
    End Function

    Private Sub SetInformation(m As String)
        SetStatus(Enum_MeteoStatus.StatusInformation, m)
    End Sub

    Private Sub SetWarning(m As String)
        SetStatus(Enum_MeteoStatus.StatusWarning, m)
    End Sub

    Private Sub SetError(m As String)
        SetStatus(Enum_MeteoStatus.StatusError, m)
    End Sub

    Private Sub SetStatus(s As Enum_MeteoStatus, m As String)
        _status = s
        _statusMsg = m
    End Sub


    Private Function DebugReadXLS() As List(Of MeteoDSSItem)

        'Leggo i dati meteo da un excel... (solo debug per verifica funzionamento modello)

        Dim xls_file As String = "C:\DEBUG_MODELLI\DEBUG_RACCA_FRUMENTO.xlsx"
        Dim provider As String = "Microsoft.ACE.OLEDB.12.0"

        Dim strXlsConn As String = $"PROVIDER={provider}; Data Source={xls_file};Extended Properties=""EXCEL 8.0;HDR=YES; IMEX=1"";"
        Dim objXlsConn As New OleDb.OleDbConnection(strXlsConn)

        objXlsConn.Open()

        Dim atable = objXlsConn.GetOleDbSchemaTable(OleDb.OleDbSchemaGuid.Tables, Nothing)
        Dim sheetname = atable.Rows(0)("table_name").ToString
        Dim excelcomm = New OleDb.OleDbCommand("select * from [" + sheetname + "]", objXlsConn)
        Dim adexcel = New OleDb.OleDbDataAdapter(excelcomm)
        Dim ds As New DataSet
        adexcel.Fill(ds)

        objXlsConn.Close()

        Dim tblMeteo As New List(Of MeteoRow)

        Dim table = ds.Tables(0)
        For Each r In table.Rows

            If Not IsDBNull(r(0)) Then

                If IsNumeric(r(0)) Then

                    'Dim arrdt = r(0).split(" ")
                    'Dim dt As DateTime = CDate(arrdt(0))
                    'dt = dt.AddHours(CInt(arrdt(1).Substring(0, 2)))
                    Dim dt As DateTime = CDate(r(1))
                    'dt = dt.AddHours(CInt(r(1)))

                    'Dim dt0 As DateTime = r(0)

                    'Dim anno As Integer = r(1)
                    'Dim mese As Integer = r(2)
                    'Dim giorno As Integer = r(3)
                    'Dim ora As Integer = r(1)
                    'Dim dt As New DateTime(dt0.Year, dt0.Month, dt0.Day, ora, 0, 0)


                    'Dim tmpR = _tblMeteo.NewRow()

                    'tmpR("DataOra") = dt
                    'tmpR("Temp") = CDec(r(1))
                    'tmpR("UmRel") = CDec(r(2))
                    'tmpR("Prec") = CDec(r(3))
                    'tmpR("Bagn") = CDec(r(4))

                    '_tblMeteo.Rows.Add(tmpR)

                    tblMeteo.Add(New MeteoRow With {
                                 .DataOra = dt,
                                 .Temp = CDec(r(3)),
                                 .RelHum = CDec(r(4)),
                                 .Prec = CDec(r(5)),
                                 .LW = CDec(r(6))
                                 })

                End If
            End If
        Next

        Dim emptyItem As New MeteoDSSItem
        Dim meteoItems As New List(Of MeteoDSSItem)

        For Each mrow In tblMeteo
            meteoItems.Add(emptyItem.Clone(mrow))
        Next

        Return meteoItems
    End Function

End Class


Public Class MeteoRow
    Public DataOra As DateTime
    Public Temp As Decimal?
    Public Prec As Decimal?
    Public RelHum As Decimal?
    Public LW As Decimal?
    Public Forecast As Boolean
End Class


Public Class SensorRow
    Public Sensor As String
    Public Found As Boolean
End Class



Public Class MeteoDSSItem

    Private Class ReadOnlyValues
        Public _temp As Decimal
        Public _prec As Decimal
        Public _umRel As Decimal
        Public _bagn As Decimal

        Public Sub New()
            _temp = 0
            _prec = 0
            _umRel = 0
            _bagn = 0
        End Sub

        Public Function Clone(temp As Decimal?, prec As Decimal?, umRel As Decimal?, bagn As Decimal?) As ReadOnlyValues
            Return New ReadOnlyValues With {
                ._temp = If(temp, _temp),
                ._prec = If(prec, _prec),
                ._umRel = If(umRel, _umRel),
                ._bagn = If(bagn, _bagn)
            }
        End Function
    End Class

    Private _dataOra As DateTime
    Private _values As ReadOnlyValues
    Private _forecast As Boolean

    Public ReadOnly Property DataOra As DateTime
        Get
            Return _dataOra
        End Get
    End Property

    Public ReadOnly Property Temp As Decimal
        Get
            Return _values._temp
        End Get
    End Property

    Public ReadOnly Property Prec As Decimal
        Get
            Return _values._prec
        End Get
    End Property

    Public ReadOnly Property UmRel As Decimal
        Get
            Return _values._umRel
        End Get
    End Property

    Public ReadOnly Property Bagn As Decimal
        Get
            Return _values._bagn
        End Get
    End Property

    Public ReadOnly Property Forecast As Boolean
        Get
            Return _forecast
        End Get
    End Property

    Public Function VPD() As Decimal
        Return VPD(_values._umRel, _values._temp)
    End Function

    Public Function BagnEffettiva(sogliaUR As Decimal) As Decimal
        Dim result As Decimal = 0
        If _values._bagn > 0 OrElse _values._umRel >= sogliaUR OrElse _values._prec > 0 Then
            result = 1
        End If
        Return result
    End Function

    Public Sub New()
        _dataOra = Date.MinValue
        _values = New ReadOnlyValues
        _forecast = False
    End Sub

    Public Function Clone(mrow As MeteoRow) As MeteoDSSItem
        Return New MeteoDSSItem With {
            ._dataOra = mrow.DataOra,
            ._values = Me._values.Clone(mrow.Temp, mrow.Prec, mrow.RelHum, mrow.LW),
            ._forecast = mrow.Forecast
        }
    End Function

    Public Shared Function VPD(UR As Decimal, T As Decimal) As Decimal
        Return (1D - UR / 100D) * (6.11D * Math.Exp((17.47D * T) / (239D + T)))
    End Function
End Class



Public Class MeteoReadOnlyList
    Implements IEnumerable(Of MeteoDSSItem)

    Private ReadOnly _underlyingList As IList(Of MeteoDSSItem)
    Public ReadOnly Property TipoSorgente As enum_Meteo_Tiposorgente
    Public ReadOnly Property SensoreBagnatura As Boolean

    Public Function GetEnumerator() As IEnumerator(Of MeteoDSSItem) Implements IEnumerable(Of MeteoDSSItem).GetEnumerator
        Return _underlyingList.GetEnumerator()
    End Function

    Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Return _underlyingList.GetEnumerator()
    End Function

    Public Sub New(InitialList As IList(Of MeteoDSSItem), TipoSorgente As enum_Meteo_Tiposorgente, SensoreBagnatura As Boolean)
        _underlyingList = InitialList
        Me.TipoSorgente = TipoSorgente
        Me.SensoreBagnatura = SensoreBagnatura
    End Sub

    Public Function FindDateGE(ByVal dt As DateTime) As Integer

        'Return Array.FindIndex(Of DatoMeteo)(UnderlyingList.ToArray, Function(x)
        '                                                                 Return x.DataOra >= dt
        '                                                             End Function)

        'la lista è ordinata per data... ricerca binaria?

        Dim max_lim As Integer = _underlyingList.Count
        Dim left As Integer = 0
        Dim right As Integer = max_lim
        Dim mid As Integer

        While left < right
            mid = Math.Floor((left + right) / 2)
            If _underlyingList(mid).DataOra < dt Then
                left = mid + 1
            Else
                right = mid
            End If
        End While

        If right >= max_lim Then
            Return -1
        End If

        Return right
    End Function
End Class



Public Class InfoPioggia
    Private ReadOnly _inizioEvento As DateTime
    Private ReadOnly _cumulato_mm As Decimal
    Private ReadOnly _durata_hh As Integer

    Public ReadOnly Property InizioEvento As DateTime
        Get
            Return _inizioEvento
        End Get
    End Property

    Public ReadOnly Property Cumulato_mm As Decimal
        Get
            Return _cumulato_mm
        End Get
    End Property

    Public ReadOnly Property Durata_hh As Integer
        Get
            Return _durata_hh
        End Get
    End Property

    Public Sub New(inizioEvento As DateTime, cumulato_mm As Decimal, durata_hh As Integer)
        _inizioEvento = inizioEvento
        _cumulato_mm = cumulato_mm
        _durata_hh = durata_hh
    End Sub
End Class


Public Class PioggeReadOnlyList
    Implements IEnumerable(Of InfoPioggia)

    Private ReadOnly _underlyingList As IList(Of InfoPioggia)

    Public Sub New(InitialList As IList(Of InfoPioggia))
        _underlyingList = InitialList
    End Sub

    Public Function GetEnumerator() As IEnumerator(Of InfoPioggia) Implements IEnumerable(Of InfoPioggia).GetEnumerator
        Return _underlyingList.GetEnumerator()
    End Function

    Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Return _underlyingList.GetEnumerator()
    End Function
End Class
