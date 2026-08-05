
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq


Public Class OutputMeteo

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

