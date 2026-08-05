
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq


Public Class OutputModello

    Private ReadOnly DT As DataTable
    Private ReadOnly Cols As List(Of KendoColumn)
    Private ReadOnly Vals As List(Of Object)

    Public Sub New()
        DT = New DataTable
        Cols = New List(Of KendoColumn)
        Vals = New List(Of Object)
    End Sub

    Public Function aggiungiColonna(nomeDT As String, dataType As Type, nomeOut As String, formato As String,
                                    Optional ancheNull As Boolean = False, Optional stringaNull As String = "") As KendoColumn

        DT.Columns.Add(New DataColumn(nomeDT, dataType))

        Dim col As New KendoColumn(nomeDT, dataType, nomeOut)

        If dataType <> GetType(String) Then

            Select Case dataType

                Case GetType(Date), GetType(DateTime)

                    col._formatDataOra = formato

                    If Not ancheNull Then
                        col._template = "#=kendo.toString(" + nomeDT + ", '" + formato + "')#"
                    Else
                        col._template = "#:" + nomeDT + " != null ? kendo.toString(" + nomeDT + ", '" + formato + "') : '" + stringaNull + "'#"
                    End If


                Case GetType(Boolean)

                    If Not String.IsNullOrEmpty(formato) Then
                        Dim vero_falso = formato.Split("|")
                        Dim vero As String = vero_falso(0)
                        Dim falso As String = If(vero_falso.Count > 1, vero_falso(1), "")
                        col._template = "#=(" + nomeDT + " === true) ? '" + vero + "' : '" + falso + "'#"
                    End If
                    col._css = "allineacentro"
                    col._cssHeader = "allineacentro"


                Case Else

                    col._formatNr = formato
                    col._css = "allineadestra"
                    col._cssHeader = "allineadestra"

            End Select
        End If

        Cols.Add(col)

        Return col
    End Function

    Public Function aggiungiColonna(nomeDT As String, nomeOut As String) As KendoColumn
        Return aggiungiColonna(nomeDT, GetType(String), nomeOut, "")
    End Function

    Public Function aggiungiIndicatore(nomeDT As String, limiti() As Decimal, type As OutputIndicator.enum_IndicatorType) As OutputIndicator
        aggiungiColonna(nomeDT, "")._hidden = True
        Return OutputHelper.createIndicator(nomeDT, limiti, type)
    End Function

    Public Sub AddField(val As Object)
        If val IsNot Nothing Then
            Vals.Add(val)
        Else
            Vals.Add(DBNull.Value)
        End If
    End Sub

    Public Sub Commit()
        If Vals.Count = 0 Then
            Return
        End If
        Dim dr As DataRow = DT.NewRow
        Dim cnt As Integer = Math.Min(DT.Columns.Count, Vals.Count) - 1
        For i = 0 To cnt
            dr(i) = Vals(i)
        Next
        Vals.Clear()
        DT.Rows.Add(dr)
    End Sub

    Public Function Output(Optional indicators As List(Of OutputIndicator) = Nothing, Optional bands As PlotBands = Nothing) As String

        Dim kendoColumns As New JArray
        Dim kendoModel As New JObject
        Dim kendoRows As New JArray

        For Each col In Cols

            Dim kcol = col.GetKendoColumn()
            If kcol IsNot Nothing Then

                kendoColumns.Add(kcol)
            End If

            kendoModel.Add(col.GetKendoModel())
        Next

        For Each row As DataRow In DT.Rows

            Dim jobj As New JObject

            For Each col In Cols

                jobj.Add(col.GetKendoRowProperty(row))
            Next

            kendoRows.Add(jobj)
        Next

        Dim resultObj As New JObject From {
            {"kendo_columns", kendoColumns},
            {"kendo_model", kendoModel},
            {"kendo_rows", kendoRows}
        }

        If indicators IsNot Nothing AndAlso indicators.Count > 0 Then

            Dim arrIndicators As New JArray
            For Each oi In indicators
                arrIndicators.Add(New JObject(
                                  New JProperty("type", oi.Type),
                                  New JProperty("levels", oi.Levels),
                                  New JProperty("fieldVal", oi.FieldVal),
                                  New JProperty("fieldClr", oi.FieldClr),
                                  New JProperty("fieldHidden", oi.FieldHidden)
                                  )
                                  )
            Next

            resultObj.Add(New JProperty("indicators", arrIndicators))

            If bands IsNot Nothing Then

                resultObj.Add(New JProperty("plotBands", JObject.FromObject(bands)))
            End If
        End If


        Dim result = JsonConvert.SerializeObject(
            resultObj,
            New JsonSerializerSettings With {.DateFormatString = "yyyy-MM-ddTHH:mm:ss"})

        Return result
    End Function



    Public Class KendoColumn
        Private ReadOnly _field As String
        Private ReadOnly _tipo As String
        Private ReadOnly _title As String
        Public _template As String
        Public _hidden As Boolean
        Public _css As String
        Public _cssHeader As String
        Public _gruppoColonne As String
        Public _formatNr As String
        Public _formatDataOra As String

        Public Sub New(field As String, vbType As Type, title As String)
            _field = field
            Select Case vbType
                Case GetType(Date), GetType(DateTime)
                    _tipo = "date"
                Case GetType(String)
                    _tipo = "string"
                Case GetType(Boolean)
                    _tipo = "boolean"
                Case Else
                    _tipo = "number"
            End Select
            _title = title
            _css = ""
            _cssHeader = ""
            _hidden = False
            _formatNr = ""
            _formatDataOra = ""
            _gruppoColonne = ""
            _template = ""
        End Sub

        Public Function GetKendoColumn() As JObject

            If _hidden Then

                Return Nothing
            End If

            Dim format As String = ""

            If _tipo = "date" Then

                format = "dd/MM/yyyy"

                If Not String.IsNullOrEmpty(_formatDataOra) Then

                    format = _formatDataOra
                End If
            Else

                If _tipo = "number" Then

                    format = _formatNr
                End If
            End If

            Dim jobj As New JObject From {
                {"field", _field},
                {"title", _title}
            }

            If Not String.IsNullOrEmpty(format) Then
                jobj.Add("format", $"{{0:{format}}}".Replace("\\", "\"))
            End If

            If Not String.IsNullOrEmpty(_template) Then
                jobj.Add("template", _template)
            End If

            If Not String.IsNullOrEmpty(_cssHeader) Then
                jobj.Add("headerAttributes", New JObject(New JProperty("class", _cssHeader)))
            End If

            If Not String.IsNullOrEmpty(_css) Then
                jobj.Add("attributes", New JObject(New JProperty("class", _css)))
            End If

            If Not String.IsNullOrEmpty(_gruppoColonne) Then
                jobj.Add("gruppoColonne", _gruppoColonne)
            End If

            Return jobj
        End Function

        Public Function GetKendoModel() As JProperty
            Return New JProperty(_field, New JObject(New JProperty("type", _tipo)))
        End Function

        Public Function GetKendoRowProperty(row As DataRow) As JProperty

            If IsDBNull(row.Item(_field)) Then

                If _tipo = "string" Then

                    Return New JProperty(_field, "")
                End If

                Return New JProperty(_field, Nothing)
            End If

            Return New JProperty(_field, row.Item(_field))
        End Function
    End Class

End Class



Public Class OutputIndicator

    Public Structure RGB
        Public R As Integer
        Public G As Integer
        Public B As Integer
        Public Sub New(ByVal r_ As Integer, ByVal g_ As Integer, ByVal b_ As Integer)
            R = r_
            G = g_
            B = b_
        End Sub
        Public Function HexColor() As String
            Dim hexclr As String = "#" & Strings.Right("0" & Hex(R), 2) & Strings.Right("0" & Hex(G), 2) & Strings.Right("0" & Hex(B), 2)
            Return Strings.Left(hexclr, 7)
        End Function
        Public Shared Operator +(ByVal rgb1 As RGB, ByVal rgb2 As RGB) As RGB
            Return New RGB(rgb1.R + rgb2.R, rgb1.G + rgb2.G, rgb1.B + rgb2.B)
        End Operator
        Public Shared Operator -(ByVal rgb1 As RGB, ByVal rgb2 As RGB) As RGB
            Return New RGB(rgb1.R - rgb2.R, rgb1.G - rgb2.G, rgb1.B - rgb2.B)
        End Operator
        Public Shared Operator *(ByVal _rgb As RGB, ByVal v As Decimal) As RGB
            Return New RGB(Math.Round(_rgb.R * v), Math.Round(_rgb.G * v), Math.Round(_rgb.B * v))
        End Operator

    End Structure

    Private Class IndicatorStop
        Public Limite As Decimal
        Public Colore As RGB
    End Class

    Public Enum enum_IndicatorType
        Colors = 1
        Numbers = 2
    End Enum

    Private _Type As enum_IndicatorType
    Private Stops As List(Of IndicatorStop)
    Private _FieldClr As String
    Private _FieldVal As String
    Private _FieldHidden As Boolean

    Public ReadOnly Property Type As String
        Get
            Dim t As String = "not_available"
            Select Case _Type
                Case enum_IndicatorType.Colors
                    t = "colors"
                Case enum_IndicatorType.Numbers
                    t = "numbers"
            End Select
            Return t
        End Get
    End Property
    Public ReadOnly Property Levels As Integer
        Get
            Return Stops.Count
        End Get
    End Property
    Public ReadOnly Property FieldClr As String
        Get
            Return _FieldClr
        End Get
    End Property
    Public ReadOnly Property FieldVal As String
        Get
            Return _FieldVal
        End Get
    End Property
    Public ReadOnly Property FieldHidden As Boolean
        Get
            Return _FieldHidden
        End Get
    End Property

    Public Sub New(ByVal fClr As String, ByVal type As enum_IndicatorType)
        _Type = type
        Stops = New List(Of IndicatorStop)()
        _FieldClr = fClr
        _FieldVal = ""
        _FieldHidden = False
    End Sub
    Public Function setFieldVal(ByVal fVal As String) As OutputIndicator
        _FieldVal = fVal
        Return Me
    End Function
    Public Function setFieldHidden(ByVal hidden As Boolean) As OutputIndicator
        _FieldHidden = hidden
        Return Me
    End Function
    Public Sub addStopValue(ByVal lim As Decimal, ByVal clr As RGB)
        Stops.Add(New IndicatorStop With {.Limite = lim, .Colore = clr})
    End Sub
    Public Function colorForVal(ByVal val As Decimal) As String
        Dim i As Integer = 0
        Dim iVal = -1
        While iVal < 0 AndAlso i < Stops.Count
            If val < Stops(i).Limite Then
                iVal = i
            End If
            i += 1
        End While

        Dim retVal As String = ""

        Select Case _Type
            Case enum_IndicatorType.Colors
                If iVal >= 0 Then
                    retVal = Stops(iVal).Colore.HexColor()
                Else
                    retVal = "#FFFFFF"
                End If
            Case enum_IndicatorType.Numbers
                If iVal >= 0 Then
                    retVal = iVal + 1
                Else
                    retVal = 0
                End If
        End Select

        Return retVal
    End Function
    Public Function gradientForVal(ByVal val As Decimal) As String
        Dim i As Integer = 1
        While i < Stops.Count
            If val <= Stops(i).Limite Then
                Dim f As Decimal = (val - Stops(i - 1).Limite) / (Stops(i).Limite - Stops(i - 1).Limite)
                Return (Stops(i - 1).Colore + ((Stops(i).Colore - Stops(i - 1).Colore) * f)).HexColor()
            End If
            i += 1
        End While
        Return Stops.Last.Colore.HexColor()
    End Function

    Public Function PlotBands(Optional opacity As Decimal = 0.3) As PlotBands

        Dim bands As New PlotBands With {
            .Field = FieldVal,
            .Bands = New List(Of PlotBands.Band)
        }
        For Each s In Stops
            bands.Bands.Add(New PlotBands.Band With {.StopValue = s.Limite, .Color = s.Colore.HexColor(), .Opacity = opacity})
        Next

        Return bands
    End Function

End Class



Class OutputHelper

    Public Shared Function createIndicator(ByVal fClr As String, ByVal stops() As Decimal, ByVal type As OutputIndicator.enum_IndicatorType) As OutputIndicator

        Dim outind As New OutputIndicator(fClr, type)

        If stops.Length <= 0 Then
            outind.addStopValue(System.Decimal.MaxValue, New OutputIndicator.RGB(0, 128, 0))
        Else
            Dim colori As New List(Of OutputIndicator.RGB)
            colori.Add(New OutputIndicator.RGB(0, 128, 0)) 'verde
            colori.Add(New OutputIndicator.RGB(255, 240, 0)) 'giallo
            If stops.Length > 3 Then
                colori.Add(New OutputIndicator.RGB(255, 128, 0)) 'arancio
            End If
            If stops.Length >= 3 Then
                colori.Add(New OutputIndicator.RGB(240, 0, 0)) 'rosso
            End If
            Dim cnt As Integer = Math.Min(stops.Length, colori.Count)
            For i = 0 To cnt - 1
                outind.addStopValue(stops(i), colori(i))
            Next

        End If

        Return outind

    End Function

End Class



Public Class PlotBands
    Public Class Band
        Public Property StopValue As Decimal
        Public Property Color As String
        Public Property Opacity As Decimal
    End Class

    Public Property Field As String
    Public Property Bands As List(Of Band)

End Class












Module OutputModelloNT


    Public Class ColumnTooltip
        Private _title As String
        Private _content As String

        Public Sub New(t As String, c As String)
            _title = t
            _content = c
        End Sub

        Public Sub New(c As String)
            _title = ""
            _content = c
        End Sub

        Public ReadOnly Property Content As JProperty
            Get
                If String.IsNullOrEmpty(_title) Then

                    Return New JProperty("tooltip_data", _content)
                End If

                Return New JProperty("tooltip_datat", New JObject(New JProperty("title", _title),
                                                                    New JProperty("content", _content)))
            End Get
        End Property
    End Class


    Public MustInherit Class AbsColumn

        Protected ReadOnly _title As String
        Protected _tooltip As ColumnTooltip

        Protected Sub New(title As String)
            _title = title
            _tooltip = Nothing
        End Sub

        Public Sub Tooltip(title As String, content As String)
            _tooltip = New ColumnTooltip(title, content)
        End Sub

        Public Sub Tooltip(content As String)
            _tooltip = New ColumnTooltip(content)
        End Sub

        Public MustOverride Function DataColumns() As DataColumn()

        Public MustOverride Function KendoModel() As JArray

        Public MustOverride Function KendoColumn() As JObject

    End Class


    Public Class ColumnGroup
        Inherits AbsColumn

        Private ReadOnly _columns As List(Of Column)

        Public Sub New(title As String)
            MyBase.New(title)

            _columns = New List(Of Column)
        End Sub

        Public Function AddColumn(Of T)(field As String, title As String) As Column

            Dim col As New TypedColumn(Of T)(field, title)

            _columns.Add(col)

            Return col
        End Function

        Public Overrides Function DataColumns() As DataColumn()

            Dim cols As New List(Of DataColumn)

            For Each c In _columns

                cols.AddRange(c.DataColumns())
            Next

            Return cols.ToArray
        End Function

        Public Overrides Function KendoModel() As JArray

            Dim model As New JArray

            For Each c In _columns

                Dim m = c.KendoModel()

                For Each obj In m

                    model.Add(obj)
                Next
            Next

            Return model
        End Function

        Public Overrides Function KendoColumn() As JObject

            Dim kcol As New JObject(New JProperty("title", _title))

            If _tooltip IsNot Nothing Then

                kcol.Add(_tooltip.Content)
            End If

            Dim kcols As New JArray

            For Each c In _columns

                Dim kc = c.KendoColumn

                If kc IsNot Nothing Then

                    kcols.Add(c.KendoColumn)
                End If
            Next

            kcol.Add(New JProperty("columns", kcols))

            Return kcol
        End Function
    End Class


    Public MustInherit Class Column
        Inherits AbsColumn

        Protected ReadOnly _field As String
        Protected _nullable As Boolean
        Protected _nullString As String
        Protected _format As String
        Protected _hidden As Boolean

        Public Sub New(field As String, title As String)
            MyBase.New(title)

            _field = field
            _nullable = False
            _nullString = ""
            _format = ""
            _hidden = False
        End Sub

        Public Function Format(fmt As String) As Column
            _format = fmt
            Return Me
        End Function

        Public Function Nullable(Optional nullString As String = "") As Column
            _nullable = True
            _nullString = nullString
            Return Me
        End Function

        Public Function Hide() As Column
            _hidden = True
            Return Me
        End Function


    End Class


    Public Class TypedColumn(Of T)
        Inherits Column

        Public Sub New(field As String, title As String)
            MyBase.New(field, title)
        End Sub

        Public Overrides Function DataColumns() As DataColumn()

            Return {New DataColumn(_field, GetType(T))}
        End Function

        Public Overrides Function KendoModel() As JArray

            Dim type = "number"

            Select Case GetType(T)

                Case GetType(Date), GetType(DateTime)
                    type = "date"

                Case GetType(String)
                    type = "string"

                Case GetType(Boolean)
                    type = "boolean"

            End Select

            'Case GetType(Integer), GetType(Decimal), GetType(Double)

            Dim model As New JArray

            model.Add(New JObject(New JProperty(_field, New JObject(
                                                New JProperty("editable", False),
                                                New JProperty("type", type)))))

            Return model
        End Function

        Public Overrides Function KendoColumn() As JObject

            If _hidden Then

                Return Nothing
            End If

            Dim kcol As New JObject(New JProperty("field", _field),
                                    New JProperty("title", _title),
                                    New JProperty("filterable", False))

            Dim template As String = ""
            Dim css As String = ""

            Select Case GetType(T)

                Case GetType(String)

                    'nulla da fare


                Case GetType(Date), GetType(DateTime)


                    Dim fmtNotNull As String = "kendo.toString(kendo.parseDate(" + _field + ", 'yyyy-MM-ddTHH:mm:ss'), '" + _format + "')"

                    If Not _nullable Then

                        template = "#=" + fmtNotNull + "#"
                    Else

                        template = "#:" + _field + " != null ? " + fmtNotNull + " : '" + _nullString + "'#"
                    End If


                Case GetType(Boolean)

                    If Not String.IsNullOrEmpty(_format) Then

                        Dim vero_falso = _format.Split("|")
                        Dim vero As String = vero_falso(0)
                        Dim falso As String = If(vero_falso.Count > 1, vero_falso(1), "")

                        template = "#=(" + _field + " === true) ? '" + vero + "' : '" + falso + "'#"

                        _format = ""
                    End If

                    css = "allineacentro"


                Case Else

                    css = "allineadestra"


            End Select


            If Not String.IsNullOrEmpty(template) Then

                kcol.Add(New JProperty("template", template))
            End If

            If Not String.IsNullOrEmpty(_format) Then

                kcol.Add(New JProperty("format", "{0:" & _format & "}"))
            End If

            If Not String.IsNullOrEmpty(css) Then

                kcol.Add(New JProperty("headerAttributes", New JObject(New JProperty("class", css))))
                kcol.Add(New JProperty("attributes", New JObject(New JProperty("class", css))))
            End If

            If _tooltip IsNot Nothing Then

                kcol.Add(_tooltip.Content)
            End If

            Return kcol
        End Function
    End Class


    Public Class Grid

        Private ReadOnly _columns As List(Of AbsColumn)
        Private _table As DataTable

        Public Sub New()
            _columns = New List(Of AbsColumn)
            _table = Nothing
        End Sub

        Public Function AddGroup(title As String) As ColumnGroup

            Dim group = New ColumnGroup(title)

            _columns.Add(group)

            Return group
        End Function

        Public Function AddColumn(Of T)(field As String, title As String) As Column

            Dim col As New TypedColumn(Of T)(field, title)

            _columns.Add(col)

            Return col
        End Function

        Public Function AddIndicator(field As String, limiti() As Decimal, type As OutputIndicator.enum_IndicatorType) As OutputIndicator

            AddColumn(Of String)(field, "").Hide()

            Return OutputHelper.createIndicator(field, limiti, type)
        End Function

        Public Function AddRow() As DataRow

            If _table Is Nothing Then

                _table = New DataTable

                For Each c0 In _columns

                    _table.Columns.AddRange(c0.DataColumns())
                Next
            End If

            Dim row = _table.NewRow()

            For idx = 0 To _table.Columns.Count - 1

                row(idx) = DBNull.Value
            Next

            _table.Rows.Add(row)

            Return row
        End Function

        Public Function ToJSONString(Optional indicators As List(Of OutputIndicator) = Nothing, Optional bands As PlotBands = Nothing) As String

            Dim kendo_model As New JArray
            Dim kendo_columns As New JArray

            For Each col In _columns

                Dim m = col.KendoModel

                For Each obj In m

                    kendo_model.Add(obj)
                Next
            Next
            Return ""


            'Public Function KendoValue(row As DataRow) As JProperty

            '    Dim dbValue = row(_field)

            '    If IsDBNull(dbValue) Then

            '        Return New JProperty(_field, Nothing)
            '    End If

            '    Return New JProperty(_field, dbValue)
            'End Function

            'Dim kendo_rows As New JArray

            'For Each col In _columns

            '    kendo_model.Add(col.KendoModel)


            '    Dim kcol = col.KendoColumn

            '    If kcol IsNot Nothing Then

            '        kendo_columns.Add(kcol)
            '    End If
            'Next

            'For Each row In _table.Rows

            '    Dim kRow As New JObject

            '    For Each col In _columns

            '        kRow.Add(col.KendoValue(row))
            '    Next

            '    kendo_rows.Add(kRow)
            'Next

            'Dim data As New JObject(New JProperty("kendo_model", kendo_model),
            '                        New JProperty("kendo_columns", kendo_columns),
            '                        New JProperty("kendo_rows", kendo_rows))


            'If indicators IsNot Nothing AndAlso indicators.Count > 0 Then

            '    Dim arrIndicators As New JArray
            '    For Each oi In indicators
            '        arrIndicators.Add(New JObject(
            '                          New JProperty("type", oi.Type),
            '                          New JProperty("levels", oi.Levels),
            '                          New JProperty("fieldVal", oi.FieldVal),
            '                          New JProperty("fieldClr", oi.FieldClr),
            '                          New JProperty("fieldHidden", If(oi.FieldHidden, "true", "false"))
            '                          )
            '                          )
            '    Next

            '    data.Add(New JProperty("indicators", arrIndicators))

            '    If bands IsNot Nothing Then

            '        data.Add(New JProperty("plotBands", JObject.FromObject(bands)))
            '    End If
            'End If

            ''Dim dbg = JsonConvert.SerializeObject(data, New JavaScriptDateTimeConverter())

            'Return data.ToString
        End Function
    End Class




#If False Then

TOOLTIP


*** Agronomica30_BatteriosiKiwi_PSA ***

    kendodata.tooltip = {
        column: "M_Orario",
        content: '<div style="font-size: larger;"><span>'
            + TraduzioneMultiResx(datiMeteoResx, 'TooltipIndiceMoltiplicazioneBatterica1', 'Indice moltiplicazione batterica per ogni ora con bagnatura fogliare')
            + '</span></div><div style="margin-top: 15px; margin-bottom: 10px;"><span>'
            + TraduzioneMultiResx(datiMeteoResx, 'TooltipIndiceMoltiplicazioneBatterica2', 'Se')
            + ' <i>' + TraduzioneMultiResx(datiMeteoResx, 'TooltipIndiceMoltiplicazioneBatterica3', 'Bagnatura fogliare') + '</i></span></div>'
            + '<div style="padding-left: 1em;"><span>M = -0.000003*temp<sup>4</sup>-0.00011*temp<sup>3</sup>+0.00201*temp<sup>2</sup>+0.0541*temp+0.247</span></div>'
    };





*** Agronomica30_MaculaturaPero ***

 kendodata.tooltip = [
        {
            column: "Data",
            content: TraduzioneMultiResx(datiMeteoResx, "DatoSfasatoDiOttoOreIndietro", "<div>Dato sfasato di 8 ore indietro.</div>")
        },
        {
            column: "Temp",
            content: TraduzioneMultiResx(datiMeteoResx, "DatoSfasatoDiOttoOreIndietro", "<div>Dato sfasato di 8 ore indietro.</div>")
        },
        {
            column: "Prec",
            content: TraduzioneMultiResx(datiMeteoResx, "DatoSfasatoDiOttoOreIndietro", "<div>Dato sfasato di 8 ore indietro.</div>")
        },
        {
            column: "UmRel",
            content: TraduzioneMultiResx(datiMeteoResx, "DatoSfasatoDiOttoOreIndietro", "<div>Dato sfasato di 8 ore indietro.</div>")
        },
        {
            column: "LW",
            content: TraduzioneMultiResx(datiMeteoResx, "DatoSfasatoDiOttoOreIndietro", "<div>Dato sfasato di 8 ore indietro.</div>")
        },
        {
            column: "BSPspor",
            content: TraduzioneMultiResx(datiMeteoResx, "IndiceSporulazione", "<div>Indice sporulazione</div>")
        },
        {
            column: "BSPspor3gg",
            content: TraduzioneMultiResx(datiMeteoResx, "MediaUltimiTreGiorniBSPspor", "<div>Media mobile ultimi 3 giorni di BSPspor</div>")
        },
        {
            column: "BSPcast",
            content: TraduzioneMultiResx(datiMeteoResx, "IndiceCondizioniAmbientaliFavorevoliPerInfezioni", "<div>Indice condizioni ambientali favorevoli per le infezioni</div>")
        },
        {
            column: "BSPcast3gg",
            content: TraduzioneMultiResx(datiMeteoResx, "MediaUltimiTreGiorniBSPcast", "<div>Media mobile ultimi 3 giorni di BSPcast</div>")
        },
        {
            column: "Global_risk",
            content: TraduzioneMultiResx(datiMeteoResx, "IndiceCombinatoBSPsporEBSPcast", "<div>Indice combinato tra BSPspor e BSPcast, tiene conto sia delle condizioni che influenzano la sporulazione sia delle condizioni climatiche favorevoli all'infezione</div>")
        }
    ];





*** Agronomica30_Peronospora ***

  let fungo = "url(images/Indicatori/fungo-pieno.png)";

        let arr_tooltip = [
            {
                column: "PMO",
                content: {
                    title: "Intensità coorte oospore (%)",
                    content: "<span>Ogni evento piovoso innesca la germinazione di una coorte di oospore. L'intensità potenziale di una coorte dipende dalla quantità di spore mature (</span>" +
                        "<span style='background-image: " + fungo + "; background-size: contain; background-repeat: no-repeat; padding-left: 1.5em;'>scarsa, </span>" +
                        "<span style='background-image: " + fungo + ", " + fungo + "; background-position: 0 top, 1.5em top;background-size: contain; background-repeat: no-repeat; padding-left: 3em;'>moderata, </span>" +
                        "<span style='background-image: " + fungo + ", " + fungo + ", " + fungo + "; background-position: 0 top, 1.5em top, 3em top; background-size: contain; background-repeat: no-repeat; padding-left: 4.5em;'>elevata</span>" +
                        "<span>).</span>"
                }
            },
            {
                column: "GER0",
                content: {
                    title: "Data inizio germinazione oospore",
                    content: "Il processo di germinazione delle oospore innescato da una pioggia dipende dalla temperatura e dalla bagnatura della lettiera. Il modello simula la data in cui le coorti di oospore sono pronte a germinare."
                }
            },
            {
                column: "GER1",
                content: {
                    title: "% Germinazione / Data emissione sporangi", 
                    content: "<div>Al termine del processo di germinazione le oospore producono i macrozoosporangi che, in condizioni di bagnatura favorevoli, sono in grado di rilasciare zoospore nella lettiera.</div>" +
                        "<div style='margin-top: 15px; margin-bottom: 5px; font-weight: bold;'>" + TraduzioneMultiResx(datiMeteoResx, "SopravvivenzaMacrozoosporangi", "Sopravvivenza dei macrozoosporangi") + "</div>" +
                        "<div>Il modello simula la data di emissione dei macrozoosporangi e ne calcola il periodo di sopravvivenza. Superato questo periodo, in assenza di un velo d'acqua a determinate condizioni di temperatura e umidità relativa i macrozoosporangi muoiono (non si verifica alcuna infezione).</div>"
                }
            },
            {
                column: "ZRE",
                content: {
                    title: "Data rilascio zoospore",
                    content: "In condizioni favorevoli (presenza di un film d'acqua a temperatura e umidità relativa elevate) i macrozoosporangi rilasciano zoospore nella lettiera."
                }
            },
            {
                column: "ZDI",
                content: {
                    title: "Data dispersione zoospore per pioggia", 
                    content: "In questa fase le zoospore, molto delicate, nuotano nel film liquido e si devitalizzano solo se esposte a condizioni climatiche sfavorevoli (assenza di bagnatura della lettiera). Al contrario, le piogge sono in grado di veicolare le zoospore sulla vegetazione suscettibile (tramite gli schizzi d'acqua)."
                }
            },
            {
                column: "info",
                content: {
                    title: "Destino coorte",
                    content: "<div style='margin-bottom: 3px;'>In sintesi, le coorti possono avere destini diversi:</div>" +
                        "<ul style='padding-left: 20px;'>" +
                        "<li>in assenza di un'adeguata bagnatura della lettiera i macrozoosporangi muoiono senza rilasciare zoospore (<i>Morte sporangi</i>)</li>" +
                        "<li>una volta rilasciate le zoospore, un'interruzione di bagnatura della lettiera ne provoca la morte (<i>Morte zoospore</i>)</li>" +
                        "</ul>" +
                        "<div style='margin-bottom: 3px; margin-top: 5px;'>Ogni pioggia è in grado di veicolare zoospore vitali sul tessuto fogliare suscettibile:</div>" +
                        "<ul style='padding-left: 20px;'>" +
                        "<li>in assenza di adeguate condizioni di temperatura e bagnatura fogliare il processo si interrompe prima di poter innescare un'infezione (<i>No infezione</i>);</li>" +
                        "<li>in condizioni favorevoli di temperatura e bagnatura fogliare il processo infettivo si innesca (<i>INFEZIONE</i>).</li>" +
                        "</ul>",
                    max_width: "45em"
                }
            },
            {
                column: { column: "incub2", parent: true },
                content: {
                    title: "% Incubazione / Data comparsa sintomi",
                    content: "Nel caso in cui l'evento sia infettivo, il modello stima la percentuale di incubazione e le date di inizio e fine comparsa sintomi su foglie."
                }
            }
        ];

        kendodata.tooltip = [];

        $.each(arr_tooltip, function (i, e) {

            if (!e.content.max_width) {

                max_width = "40em";

            } else {

                max_width = e.content.max_width;
            }

            let content = "<div style='text-align: center; font-size: larger; margin-bottom: 10px;'>" + e.content.title + "</div>";
            content += "<div style='max-width: " + max_width + "; white-space: normal; text-align: justify;'>"
            content += e.content.content;
            content += "</div>";

            kendodata.tooltip.push({ column: e.column, content: content });
        });





*** Agronomica30_BotriteVite ***

kendodata.tooltip = [
            {
                column: "SEV1",
                content: '<div><span>' + kendo.format(TraduzioneMultiResx(datiMeteoResx, 'SeveritàInfezioneInStadiBBCHDaA', 'Severità infezione nello stadio BBCH da {0} a {1}'), 53, 73) +
                    '<br/>' + TraduzioneMultiResx(datiMeteoResx, 'ValoreCumulatoInfettivitàRelativa', 'Valore cumulato dell\'infettività relativa nelle fasi da infiorescenze a giovani grappoli') + '</span></div>' +
                    //'<div style="font-size: larger; text-align: center;">BBCH Riproduttivo</div>' +
                    //'<div style="font-size: smaller;">' +
                    '<div>' +
                    '<dl class="table-display">' +
                    '<dt>53</dt><dd>' + TraduzioneMultiResx(datiMeteoResx, 'InflorescenzaVisibile', 'Inflorescenza chiaramente visibile') + '</dd>' +
                    '<dt>55</dt><dd>' + TraduzioneMultiResx(datiMeteoResx, 'PrimiBoccioliVisibili', 'Primi boccioli visibili (poco sviluppati)') + '</dd>' +
                    '<dt>59</dt><dd>' + TraduzioneMultiResx(datiMeteoResx, 'BoccioliSviluppatiConPetali', 'Boccioli sviluppati con petali visibili') + '</dd>' +
                    '<dt>60</dt><dd>' + TraduzioneMultiResx(datiMeteoResx, 'PrimiFioriAperti', 'Primi fiori aperti') + '</dd>' +
                    '<dt>61</dt><dd>' + kendo.format(TraduzioneMultiResx(datiMeteoResx, 'PercentualeFioriAperti', '{0} dei fiori aperti'), '10%') + '</dd>' +
                    '<dt>62</dt><dd>' + kendo.format(TraduzioneMultiResx(datiMeteoResx, 'PercentualeFioriAperti', '{0} dei fiori aperti'), '20%') + '</dd>' +
                    '<dt>63</dt><dd>' + kendo.format(TraduzioneMultiResx(datiMeteoResx, 'PercentualeFioriAperti', '{0} dei fiori aperti'), '30%') + '</dd>' +
                    '<dt>64</dt><dd>' + kendo.format(TraduzioneMultiResx(datiMeteoResx, 'PercentualeFioriAperti', '{0} dei fiori aperti'), '40%') + '</dd>' +
                    '<dt>65</dt><dd>' + TraduzioneMultiResx(datiMeteoResx, 'PienaFiorituraPiùDiMetàDeiFioriAperti', 'Piena fioritura: almeno 50% dei fiori aperti') + '</dd>' +
                    '<dt>67</dt><dd>' + TraduzioneMultiResx(datiMeteoResx, 'FioriAppassiti', 'Fiori per lo più appassiti') + '</dd>' +
                    '<dt>69</dt><dd>' + TraduzioneMultiResx(datiMeteoResx, 'FineFiorituraPetaliCaduti', 'Fine della fioritura: tutti i petali caduti') + '</dd>' +
                    '<dt>71</dt><dd>' + TraduzioneMultiResx(datiMeteoResx, 'IngrossamentoOvari', 'Ingrossamento degli ovari: frutti al 10% delle dimensioni finali') + '</dd>' +
                    '<dt>72</dt><dd>' + TraduzioneMultiResx(datiMeteoResx, 'FruttiVentiPerCentoDimensioni', 'Frutti al 20% delle dimensioni finali') + '</dd>' +
                    '<dt>73</dt><dd>' + TraduzioneMultiResx(datiMeteoResx, 'AciniDimensioniMignolatura', 'Acini delle dimensioni di un granello di pepe (mignolatura)') + '</dd>' +
                    '</dl>' +
                    '</div>'
            },
            {
                column: { column: "SEVtot", parent: true },
                content: '<div><span>' + kendo.format(TraduzioneMultiResx(datiMeteoResx, 'SeveritàInfezioneInStadiBBCHDaA', 'Severità infezione nello stadio BBCH da {0} a {1}'), 79, 89) +
                    '<br/>' + TraduzioneMultiResx(datiMeteoResx, 'InfettivitàConidiEMicelio', 'Infettività dovuta a conidi e da micelio passante da acino ad acino') + '</span></div>' +
                    //'<div style="font-size: larger; text-align: center;">BBCH Riproduttivo</div>' +
                    //'<div style="font-size: smaller;">' +
                    '<dl class="table-display">' +
                    '<dt>79</dt><dd>' + TraduzioneMultiResx(datiMeteoResx, 'MaggioranzaAciniAdiacenti', 'La maggior parte degli acini si tocca') + '</dd>' +
                    '<dt>81</dt><dd>' + TraduzioneMultiResx(datiMeteoResx, 'InizioMaturazioneBacche', 'Inizio della maturazione: le bacche iniziano a manifestare il colore tipico della cultivar') + '</dd>' +
                    '<dt>85</dt><dd>' + TraduzioneMultiResx(datiMeteoResx, 'MaturazioneAvanzata', 'Maturazione avanzata') + '</dd>' +
                    '<dt>89</dt><dd>' + TraduzioneMultiResx(datiMeteoResx, 'GrappoliProntiPerLaRaccolta', 'Grappoli pronti per la raccolta') + '</dd>' +
                    '</dl>' +
                    '</div>'
            },
            {
                column: { column: "SEVtot" },
                content: TraduzioneMultiResx(datiMeteoResx, 'SommaInF2InfettivitàConidiEMicelio', '<div>Somma in F2 dei valori dell\'infettività da conidi e da micelio</div>')
            }
        ];




*** Agronomica30_ColpoDiFuoco ***

 let tooltip = [];
    if (col !== "") {
        let hh = col.replace("TRV_", "");
        tooltip.push({
            column: col,
            content: "<div>Indice di rischio: Somma mobile " + hh + " ore dell'indice TRV (Temperature Risk Value)</div>"
        });
    }

    tooltip.push({
        column: "Rischio1",
        content: "<div>Caso 1: Colpo di fuoco non presente nel frutteto l'anno precedente</div>"
    });
    tooltip.push({
        column: "Rischio2",
        content: "<div>Caso 2: Colpo di fuoco presente nel frutteto l'anno precedente</div>"
    });
    tooltip.push({
        column: "Rischio3",
        content: "<div>Caso 3: Colpo di fuoco attivo</div>"
    });

    kendodata.tooltip = tooltip
#End If



End Module