Imports AgronicaGIS2012.Commons

Public Class WKT

    Public Function CreaPoligonoDaCoordinate(ByVal Coordinate As String) As String
        Return "POLYGON ((" & Coordinate & "))"
    End Function

    Public Function CreaWKT(coordinate As List(Of xyz), tipoWKT As String, Optional ByVal holes As Dictionary(Of Integer, List(Of xyz)) = Nothing) As String

        Dim rval As String = ""
        Dim rvalHoles As List(Of String) = Nothing
        For Each coord In coordinate

            rval &= CStr(coord.X).Replace(",", ".") & " " & CStr(coord.Y).Replace(",", ".") & ", "
        Next

        If holes IsNot Nothing Then
            rvalHoles = New List(Of String)
            For Each hole In holes.Values
                Dim rvalHole = ""
                For Each coordHole In hole
                    rvalHole &= CStr(coordHole.X).Replace(",", ".") & " " & CStr(coordHole.Y).Replace(",", ".") & ", "
                Next
                rvalHoles.Add(rvalHole)
            Next
        End If


        ' VAnni: 20/12/2017: si considerano per ora poligoni con il solo outerBound
        Dim parentesiAperte As String = "("
        Dim parentesiChiuse As String = ")"

        If tipoWKT = "POLYGON" Then

            rval &= CStr(coordinate.First.X).Replace(",", ".") & " " & CStr(coordinate.First.Y).Replace(",", ".") & ", "

            parentesiAperte = "(("
            parentesiChiuse = "))"
        End If

        If holes IsNot Nothing Then
            Dim rStr = ""
            rStr = tipoWKT & " " & parentesiAperte & rval.TrimEnd(" ").TrimEnd(",") & ") "
            For Each rvalHole In rvalHoles
                rStr &= ",( " & rvalHole.TrimEnd(" ").TrimEnd(",") & ")"
            Next

            Return rStr.TrimEnd(" ").TrimEnd(",") & ")"
        Else
            Return tipoWKT & " " & parentesiAperte & rval.TrimEnd(" ").TrimEnd(",") & parentesiChiuse
        End If


    End Function

    Public Function CreaPoligonoDaCoordinate(ByVal coordinate As List(Of xyz), Optional ByVal swapLatLong As Boolean = False) As String

        Dim rval As String = ""
        For Each coord In coordinate
            If swapLatLong Then
                rval &= CStr(coord.Y).Replace(",", ".") & " " & CStr(coord.X).Replace(",", ".") & ", "
            Else
                rval &= CStr(coord.X).Replace(",", ".") & " " & CStr(coord.Y).Replace(",", ".") & ", "
            End If

        Next

        If Not (coordinate.First.X = coordinate.Last.X And _
            coordinate.First.Y = coordinate.Last.Y) Then
            If swapLatLong Then
                rval &= CStr(coordinate.First.Y).Replace(",", ".") & " " & CStr(coordinate.First.X).Replace(",", ".") & ", "
            Else
                rval &= CStr(coordinate.First.X).Replace(",", ".") & " " & CStr(coordinate.First.Y).Replace(",", ".") & ", "
            End If

        End If

        Dim tipo As String = "POLYGON"

        ' VAnni: 20/12/2017: si considerano per ora poligoni con il solo outerBound
        Dim parentesiAperte As String = "(("
        Dim parentesiChiuse As String = "))"

        If coordinate.Count = 1 Then
            tipo = "POINT"
            parentesiAperte = "("
            parentesiChiuse = ")"
        End If

        If coordinate.Count = 2 Then
            tipo = "LINESTRING"
            parentesiAperte = "("
            parentesiChiuse = ")"
        End If

        Return tipo & " " & parentesiAperte & rval.TrimEnd(" ").TrimEnd(",") & parentesiChiuse

    End Function

    Public Function CreaCoordinateDaPunto(ByVal poligono As String) As List(Of xyz)
        Dim punto As String() = poligono.Split("(")(1).Split(" ")
        Dim rval As New List(Of xyz)
        rval.Add(New xyz With {
                 .X = xyz.myCDBL(punto(0).Replace(",", "")),
                 .Y = xyz.myCDBL(punto(1).Replace(")", ""))
            })
        Return rval
    End Function

    Public Function CreaCoordinateDaPoligono(ByVal poligono As String, Optional ByVal swapLatLong As Boolean = False) As List(Of xyz)
        Dim rVal As New List(Of xyz)
        Dim appPunti() As String = {}

        'Su indicazione di Valerio, in caso di poligono con buco, elimino per ora il buco
        If poligono.Contains("),") Then
            poligono = Left(poligono, poligono.IndexOf("),")) & "))"
        ElseIf poligono.Contains(") ,") Then
            poligono = Left(poligono, poligono.IndexOf(") ,")) & "))"
        End If
        'appPunti = poligono.Split("((")(2).Replace("))", "").Replace(ControlChars.Quote, "").Split(", ")
        appPunti = poligono.Split("((")(2).Replace("))", "").Split(", ")

        Dim xyz As xyz

        For i As Integer = 0 To appPunti.Length - 1

            xyz = New xyz
            Dim appPuntixy As String() = appPunti(i).TrimStart(" ").Split(" ")
            If swapLatLong Then
                xyz.Y = xyz.myCDBL(appPuntixy(0))
                xyz.X = xyz.myCDBL(appPuntixy(1))
            Else
                xyz.X = xyz.myCDBL(appPuntixy(0))
                xyz.Y = xyz.myCDBL(appPuntixy(1))

            End If

            rVal.Add(xyz)

        Next

        Return rVal
    End Function

    Public Function CreaCoordinateDaWkt(ByVal poligono As String, Optional ByVal swapLatLong As Boolean = False) As List(Of xyz)

        Dim tipoOggetto As String = poligono.Split("(")(0).Trim.ToUpper

        Select Case tipoOggetto
            Case "POINT"
                Return CreaCoordinateDaPunto(poligono)

            Case "POLYGON"
                Return CreaCoordinateDaPoligono(poligono, swapLatLong)

            Case "MULTIPOLYGON"
                Throw New NotImplementedException

            Case "LINESTRING"
                Throw New NotImplementedException

            Case "MULTILINESTRING"
                Throw New NotImplementedException

            Case Else
                Return New List(Of xyz)
        End Select

        Return New List(Of xyz)

    End Function

End Class
