
Namespace Agronica


    Public Class CoordinateConverter


        ''' <summary>
        ''' dato un kwt in un sistema di riferimento riproietta tutti i punti in base alle opzioni
        ''' </summary>
        ''' <param name="WKTElementED50"></param>
        ''' <param name="swapCoorinates"></param>
        ''' <param name="objParametri"></param>
        ''' <returns></returns>
        Public Function WKTPolygonWGS84_from_WKTPolygonED50(
                                                        ByVal WKTElementED50 As String,
                                                        ByVal swapCoorinates As Boolean,
                                                        ByRef objParametri As ParametriCoordinateConverter,
                                                        Optional ByVal riportaPrimoPuntoComeUltimo As Boolean = False) _
                                                            As String

            '---------------------------------------------------------------------------------------------------
            'Conversione di un poligono espresso come WKT da coordinate ED50 a WGS84

            'FROM  :  "POLYGON ((760964 4895906, 760992 4895656, 760746 4895635, 760721 4895880, 760964 4895906))"
            'TO    :  POLYGON ((12.2631415391318 44.168099212167, 12.2633671279921 44.1658421309422, 12.2602849054072 44.1657412011106, 12.2600941823675 44.167952267579))

            'NOTA BENE : nel formato WKT il primo e l'ultimo vertice coincidono !!!
            '---------------------------------------------------------------------------------------------------

            'vanni, 02/08/2017: occorre gestire l'inner bound di poligoni tipo ciambella.


            'Dim sWKT As String
            'sWKT = WKTElementED50

            Dim AppDataOuterBound As String() = Nothing
            Dim AppDataInnerBound As Dictionary(Of Integer, String()) = New Dictionary(Of Integer, String())


            wktGetOuterAndInnerBoundArray(WKTElementED50, AppDataOuterBound, AppDataInnerBound)



            Dim rVal As [String] = WKTElementED50.Split(" ")(0) & " (("

            Dim sourceDecimalSeparator As String
            Dim finalDecimalSeparator As String = "."

            Try
                sourceDecimalSeparator = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator
            Catch e As Exception
                sourceDecimalSeparator = ","
            End Try

            If AppDataOuterBound.Length = 2 Then
                getPointObject(AppDataOuterBound(0), AppDataOuterBound(1), Not swapCoorinates, objParametri, rVal, AppDataOuterBound, sourceDecimalSeparator, finalDecimalSeparator)
            Else
                'outer bound
                For i As Integer = 0 To AppDataOuterBound.Length - 3 Step 2
                    getPointObject(AppDataOuterBound(i), AppDataOuterBound(i + 1), swapCoorinates, objParametri, rVal, AppDataOuterBound, sourceDecimalSeparator, finalDecimalSeparator)
                Next

                'inner bound
                For Each innerBound In AppDataInnerBound.Keys
                    If AppDataInnerBound(innerBound).Length > 0 Then
                        rVal = rVal.Substring(0, rVal.Length - 3) & "), ("
                        For i As Integer = 0 To AppDataInnerBound(innerBound).Length - 3 Step 2
                            getPointObject(AppDataInnerBound(innerBound)(i), AppDataInnerBound(innerBound)(i + 1), swapCoorinates, objParametri, rVal, AppDataInnerBound(innerBound), sourceDecimalSeparator, finalDecimalSeparator)
                        Next
                    End If
                Next


            End If

            If riportaPrimoPuntoComeUltimo Then
                If rVal.Contains("POLYGON") Then
                    Dim v As String() = rVal.Split(",")
                    Dim primoPunto As String = v(0).Replace("POLYGON", "").Replace("((", "").TrimStart(" ").TrimEnd(" ")
                    Dim ultimoPunto As String = v(v.Length - 2).TrimStart

                    If primoPunto <> ultimoPunto Then
                        rVal &= primoPunto & ",  "
                    End If

                End If
            End If

            rVal = rVal.Substring(0, rVal.Length - 3) & "))"

            Return rVal

        End Function

        Public Shared Sub wktGetOuterAndInnerBoundArray(WKTElementED50 As String, ByRef AppDataOuterBound() As String, ByRef AppDataInnerBound As Dictionary(Of Integer, String()))

            Dim wktConInnerBound As String() = WKTElementED50.Split("(")

            If Not WKTElementED50.ToLower.StartsWith("polygon") Then

                'caso solo point:          dimensione= 3, es: POINT ||30 10)
                AppDataOuterBound = wktConInnerBound(1).TrimEnd(" ").TrimEnd(",").TrimEnd(")").Split(" ")

                'inizializzazione farlocca..
                AppDataInnerBound.Add(1, New String() {})

            Else

                'caso solo outer boud:          dimensione= 3, es: POLYGON ||30 10, 40 40, 20 40, 10 20, 30 10))
                'caso outer boud + inner bound: dimensione= 4, es: POLYGON ||35 10, 45 45, 15 40, 10 20, 35 10), |20 30, 35 35, 30 20, 20 30))

                Dim sep As [Char]() = New [Char](1) {}
                sep(0) = ","c
                sep(1) = " "c

                AppDataOuterBound = wktConInnerBound(2).TrimEnd(" ").TrimEnd(",").TrimEnd(")").Split(sep)
                AppDataOuterBound = (From oD In AppDataOuterBound Where oD <> "" Select oD).ToArray

                If wktConInnerBound.Length = 4 Then
                    Dim tmpAppDataInnerBound As String()
                    tmpAppDataInnerBound = wktConInnerBound(3).TrimEnd(")").Split(sep)
                    tmpAppDataInnerBound = (From oD In tmpAppDataInnerBound Where oD <> "" Select oD).ToArray

                    AppDataInnerBound.Add(1, tmpAppDataInnerBound)

                Else
                    If wktConInnerBound.Length > 2 Then
                        Dim c As Integer = 0
                        For i As Integer = 3 To wktConInnerBound.Length - 1
                            Dim tmpAppDataInnerBound As String()
                            tmpAppDataInnerBound = wktConInnerBound(i).TrimEnd(")").Split(sep)
                            tmpAppDataInnerBound = (From oD In tmpAppDataInnerBound Where oD <> "" Select oD).ToArray
                            AppDataInnerBound.Add(c, tmpAppDataInnerBound)
                            c = c + 1
                        Next
                    Else
                        'inizializzazione farlocca..
                        AppDataInnerBound.Add(1, New String() {})
                    End If
                End If
            End If
        End Sub

        Public Sub getPointObject(ByVal sXcoord As String, ByVal sYcoord As String, ByVal swapCoorinates As Boolean, ByRef objParametri As ParametriCoordinateConverter, ByRef rVal As [String], ByVal AppData As [String](), ByVal sourceDecimalSeparator As String, ByVal finalDecimalSeparator As String)



            Dim vertex = New PointObject()
            vertex.XCoord = Double.Parse(sXcoord.Replace(".", sourceDecimalSeparator))
            vertex.YCoord = Double.Parse(sYcoord.Replace(".", sourceDecimalSeparator))

            Dim objp As CoordinateConverterAbstract
            Select Case objParametri.LibreriaDaUsare
                Case "PROJ.NET"
                    objp = New CoordinateConverterPROJNET
                Case "PROJ4"
                    objp = New CoordinateConverterProj4
                Case Else
                    objp = New CoordinateConverterPROJNET
            End Select

            Dim vertexTarget As PointObject = objp.ProiettaPunto(vertex, swapCoorinates, objParametri)
            rVal = rVal + vertexTarget.XCoord.ToString().Replace(",", finalDecimalSeparator) + " " & vertexTarget.YCoord.ToString().Replace(",", finalDecimalSeparator) & ", "
        End Sub



        Public Function convertPoint(XCoord As Double, YCoord As Double, swapCoorinates As Boolean, ByRef objParametri As ParametriCoordinateConverter) As PointObject

            Dim vertex = New PointObject With {
                .XCoord = XCoord,
                .YCoord = YCoord
            }

            Dim objp As CoordinateConverterAbstract
            Select Case objParametri.LibreriaDaUsare
                Case "PROJ.NET"
                    objp = New CoordinateConverterPROJNET
                Case "PROJ4"
                    objp = New CoordinateConverterProj4
                Case Else
                    objp = New CoordinateConverterPROJNET
            End Select

            Dim vertexTarget As PointObject = objp.ProiettaPunto(vertex, swapCoorinates, objParametri)

            Return vertexTarget

        End Function




        Public Function WGS84_from_ED50(objParametri As ParametriCoordinateConverter, North As Double, East As Double, ByRef Lat As Double, ByRef Lng As Double) As Boolean

            '---------------------------------------------------------------------------------------------------
            'Conversione di un punto da coordinate ED50 a WGS84
            '---------------------------------------------------------------------------------------------------

            Dim objp As CoordinateConverterAbstract
            Select Case objParametri.LibreriaDaUsare
                Case "PROJ.NET"
                    objp = New CoordinateConverterPROJNET
                Case "PROJ4"
                    objp = New CoordinateConverterProj4
                Case Else
                    objp = New CoordinateConverterPROJNET
            End Select

            Dim src As New PointObject With {
                .XCoord = East,
                .YCoord = North
            }

            Dim dst As PointObject = objp.ProiettaPunto(src, True, objParametri)

            Lat = dst.XCoord
            Lng = dst.YCoord

            Return True
        End Function

    End Class


End Namespace
