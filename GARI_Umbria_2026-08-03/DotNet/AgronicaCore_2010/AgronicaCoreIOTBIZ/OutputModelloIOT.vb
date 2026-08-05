Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports Newtonsoft.Json.Linq

Public Class OutputModelloIOT
    Private DT As DataTable
    Private Cols As List(Of ColonneNome)
    Private Vals As List(Of Object)

    Public Sub New()
        DT = New DataTable
        Cols = New List(Of ColonneNome)
        Vals = New List(Of Object)
    End Sub

    Private Function creaColonna(ByVal nomeDT As String,
                               ByVal dataType As Type,
                               ByVal nomeOut As String,
                               ByVal formato As String,
                               ByVal ancheNull As Boolean,
                               ByVal stringaNull As String) As ColonneNome

        Dim c As ColonneNome

        If dataType = GetType(Date) OrElse dataType = GetType(DateTime) Then

            c = New ColonneNome(nomeDT, nomeOut, "date")

            c._formatDataOra = formato

            If Not ancheNull Then
                c._FormatoParticolare = "#=kendo.toString(" + nomeDT + ", '" + formato + "')#"
            Else
                c._FormatoParticolare = "#:" + nomeDT + " != null ? kendo.toString(" + nomeDT + ", '" + formato + "') : '" + stringaNull + "'#"
            End If

        ElseIf dataType = GetType(String) Then

            c = New ColonneNome(nomeDT, nomeOut, "string")

        ElseIf dataType = GetType(Boolean) Then

            c = New ColonneNome(nomeDT, nomeOut, "boolean")

            If Not String.IsNullOrEmpty(formato) Then
                Dim vero_falso = formato.Split("|")
                Dim vero As String = vero_falso(0)
                Dim falso As String = If(vero_falso.Count > 1, vero_falso(1), "")
                c._FormatoParticolare = "#=(" + nomeDT + " === true) ? '" + vero + "' : '" + falso + "'#"
            End If
            c._css = "allineacentro"
            c._cssHeader = "allineacentro"

        Else

            c = New ColonneNome(nomeDT, nomeOut, "number")
            c._formatNr = formato
            c._css = "allineadestra"
            c._cssHeader = "allineadestra"

        End If

        c._Filtrabile = False

        Return c

    End Function

    Public Function aggiungiColonna(ByVal nomeDT As String, ByVal nomeOut As String) As ColonneNome
        Return aggiungiColonna(nomeDT, GetType(String), nomeOut, "")
    End Function

    Public Function aggiungiColonna(ByVal nomeDT As String,
                               ByVal dataType As Type,
                               ByVal nomeOut As String,
                               ByVal formato As String,
                               Optional ByVal ancheNull As Boolean = False,
                               Optional ByVal stringaNull As String = "") As ColonneNome

        DT.Columns.Add(New DataColumn(nomeDT, dataType))
        Dim col As ColonneNome = creaColonna(nomeDT, dataType, nomeOut, formato, ancheNull, stringaNull)
        Cols.Add(col)

        Return col

    End Function

    Public Sub AddField(ByVal val As Object)
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

    Public Function Output(Optional ByRef indicators As List(Of OutputIndicator) = Nothing, Optional ByRef bands As PlotBands = Nothing) As String
        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        js.Editabile_Deafault = False
        Dim result As String = js.JSON_DataTable_Kendo(DT, Cols, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaDiscesa)

        If indicators IsNot Nothing AndAlso indicators.Count > 0 Then

            Dim jsonObj = JObject.Parse(result)

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

            jsonObj.Add(New JProperty("indicators", arrIndicators))

            If bands IsNot Nothing Then

                jsonObj.Add(New JProperty("plotBands", JObject.FromObject(bands)))
            End If

            result = jsonObj.ToString()
        End If

        Return result
    End Function

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
            .bands = New List(Of PlotBands.Band)
        }
        For Each s In Stops
            bands.Bands.Add(New PlotBands.Band With {.StopValue = s.Limite, .Color = s.Colore.HexColor(), .opacity = opacity})
        Next

        Return bands
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
