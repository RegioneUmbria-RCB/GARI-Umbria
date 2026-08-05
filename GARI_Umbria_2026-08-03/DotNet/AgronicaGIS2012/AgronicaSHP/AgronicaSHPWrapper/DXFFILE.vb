


'// http://www.ucancode.net/read-dxf-file-write-dxf-file-draw-dxf-file-autocad-source-code.htm


Imports System.IO
Imports System.Collections
Imports System.ComponentModel
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Data



Namespace DXFImporter


    Public Class DXFFile

#Region "Properties"

        Private multipleSelect As Boolean = False
        Private clicked As Boolean = False

        Private XMax As Double, XMin As Double
        Private YMax As Double, YMin As Double

        Private scaleX As Double = 1
        Private scaleY As Double = 1
        Private mainScale As Double = 1

        Private aPoint As Point
        Private sizeChanged As Boolean = False

        Private startPoint As Point
        Private endPoint As Point

        Private Shared exPoint As Point

        Private drawingList As New ArrayList
        Private objectIdentifier As New ArrayList

        Public onCanvas As Boolean = False
        Private thePolyLine As polyline = Nothing

        Private polyLineStarting As Boolean = True
        Private CanIDraw As Boolean = False

        Private theSourceFile As FileInfo

        Public ReadOnly Property ListaOggetti() As ArrayList
            Get
                Return drawingList
            End Get
        End Property

        Public ReadOnly Property ListaTipiOggetti() As ArrayList
            Get
                Return objectIdentifier
            End Get
        End Property

#End Region


#Region "Helper Methods"


        Private Function CalculateRadius() As Double
            'this helper function is used to calculate the radius for the circle-drawing mode.
            Dim circleRadius As Double = Math.Sqrt((endPoint.X - startPoint.X) * (endPoint.X - startPoint.X) + (endPoint.Y - startPoint.Y) * (endPoint.Y - startPoint.Y))
            Return circleRadius
        End Function


        'Public Sub RecalculateScale()
        '    If XMax > Me.pictureBox1.Size.Width Then
        '        scaleX = CDbl(Me.pictureBox1.Size.Width) / CDbl(XMax)
        '    End If

        '    If YMax > Me.pictureBox1.Size.Height Then
        '        scaleY = CDbl(Me.pictureBox1.Size.Height) / CDbl(YMax)
        '    End If

        '    mainScale = Math.Min(scaleX, scaleY)
        'End Sub



#End Region

#Region "DXF Data Extraction and Interpretation"

        Public Sub ReadFromFile(textFile As String)
            'Reads a text file (in fact a DXF file) for importing an Autocad drawing.
            'In the DXF File structure, data is stored in two-line groupings ( or bi-line, coupling line ...whatever you call it)
            'in this grouping the first line defines the data, the second line contains the data value.
            '..as a result there is always even number of lines in the DXF file..
            Dim line1 As String, line2 As String
            'these line1 and line2 is used for getting the a/m data groups...
            line1 = "0"
            'line1 and line2 are are initialized here...
            line2 = "0"

            Dim position As Long = 0

            theSourceFile = New FileInfo(textFile)
            'the sourceFile is set.
            Dim reader As StreamReader = Nothing
            'a reader is prepared...
            Try
                'the reader is set ...
                reader = theSourceFile.OpenText()
            Catch e As FileNotFoundException
                'MessageBox.Show(e.FileName.ToString() & " cannot be found")
            Catch
                'MessageBox.Show("An error occured while opening the DXF file")
                Return
            End Try


            Dim cerchio As circle = Nothing

            Do
                '''/////////////////////////////////////////////////////////////////
                'This part interpretes the drawing objects found in the DXF file...
                '''/////////////////////////////////////////////////////////////////



                If line1 = "0" AndAlso line2 = "LINE" Then
                    LineModule(reader)

                ElseIf line1 = "0" AndAlso line2 = "TEXT" Then
                    TextModule(reader, cerchio)

                ElseIf line1 = "0" AndAlso line2 = "POLYLINE" Then
                    PolylineModule(reader, cerchio)

                ElseIf line1 = "0" AndAlso line2 = "CIRCLE" Then
                    CircleModule(reader)

                ElseIf line1 = "0" AndAlso line2 = "ARC" Then
                    ArcModule(reader)
                End If

                '''/////////////////////////////////////////////////////////////////
                '''/////////////////////////////////////////////////////////////////


                'the related method is called for iterating through the text file and assigning values to line1 and line2...
                GetLineCouple(reader, line1, line2)
            Loop While line2 <> "EOF"



            reader.DiscardBufferedData()
            'reader is cleared...
            theSourceFile = Nothing


            reader.Close()
            '...and closed.
        End Sub


        Private Sub GetLineCouple(theReader As StreamReader, ByRef line1 As String, ByRef line2 As String)
            'this method is used to iterate through the text file and assign values to line1 and line2
            Dim ci As System.Globalization.CultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture
            Dim decimalSeparator As String = ci.NumberFormat.CurrencyDecimalSeparator

            line1 = ""
            line2 = ""

            If theReader Is Nothing Then
                Return
            End If

            line1 = theReader.ReadLine()
            If line1 IsNot Nothing Then
                line1 = line1.Trim()

                line1 = line1.Replace("."c, decimalSeparator(0))
            End If
            line2 = theReader.ReadLine()
            If line2 IsNot Nothing Then
                line2 = line2.Trim()
                line2 = line2.Replace("."c, decimalSeparator(0))
            End If
        End Sub


        Private Sub LineModule(reader As StreamReader)
            'Interpretes line objects in the DXF file
            Dim line1 As String, line2 As String
            line1 = "0"
            line2 = "0"

            Dim x1 As Decimal = 0
            Dim y1 As Decimal = 0
            Dim x2 As Decimal = 0
            Dim y2 As Decimal = 0

            Do
                GetLineCouple(reader, line1, line2)

                If line1 = "10" Then
                    x1 = Convert.ToDouble(line2)

                    If x1 > XMax Then
                        XMax = x1
                    End If

                    If x1 < XMin Then
                        XMin = x1
                    End If
                End If

                If line1 = "20" Then
                    y1 = Convert.ToDouble(line2)
                    If y1 > YMax Then
                        YMax = y1
                    End If

                    If y1 < YMin Then
                        YMin = y1
                    End If
                End If

                If line1 = "11" Then
                    x2 = Convert.ToDouble(line2)

                    If x2 > XMax Then
                        XMax = x2
                    End If

                    If x2 < XMin Then
                        XMin = x2
                    End If
                End If

                If line1 = "21" Then
                    y2 = Convert.ToDouble(line2)

                    If y2 > YMax Then
                        YMax = y2
                    End If

                    If y2 < YMin Then
                        YMin = y2
                    End If


                End If
            Loop While line1 <> "21"





            '****************************************************************************************************//
            '***************This Part is related with the drawing editor...the data taken from the dxf file******//
            '***************is interpreted hereinafter***********************************************************//

            'If (Math.Abs(XMax) - Math.Abs(XMin)) > Me.pictureBox1.Size.Width Then
            '    scaleX = CDbl(Me.pictureBox1.Size.Width) / CDbl(Math.Abs(XMax) - Math.Abs(XMin))
            'Else
            '    scaleX = 1
            'End If


            'If (Math.Abs(YMax) - Math.Abs(YMin)) > Me.pictureBox1.Size.Height Then
            '    scaleY = CDbl(Me.pictureBox1.Size.Height) / CDbl(Math.Abs(YMax) - Math.Abs(YMin))
            'Else
            '    scaleY = 1
            'End If

            'mainScale = Math.Min(scaleX, scaleY)




            Dim ix As Integer = drawingList.Add(New Line(New Point(x1, -y1), New Point(x2, -y2), 1))
            objectIdentifier.Add(New DrawingObject(2, ix))

            ' '''////////////////////////////////////////////////////////////////////////////////////////////////////
            ' '''////////////////////////////////////////////////////////////////////////////////////////////////////

        End Sub


        Private Sub PolylineModule(reader As StreamReader, ByRef CerchioPerCodiceParticella As circle)
            'Interpretes polyline objects in the DXF file
            Dim line1 As String, line2 As String
            line1 = "0"
            line2 = "0"

            Dim x1 As Double = 0
            Dim y1 As Double = 0
            Dim x2 As Double = 0
            Dim y2 As Double = 0


            thePolyLine = New polyline(1)

            If Not CerchioPerCodiceParticella Is Nothing Then
                thePolyLine.CodiceAssociato = CerchioPerCodiceParticella.CodiceAssociato
            End If


            Dim counter As Integer = 0
            Dim numberOfVertices As Integer = 1
            Dim openOrClosed As Integer = 0
            Dim pointList As New ArrayList()


            Do
                GetLineCouple(reader, line1, line2)

                If line1 = "66" Then
                    thePolyLine.Colore = line2
                End If

                If line1 = "8" Then
                    thePolyLine.Layer = line2
                End If

                If line1 = "6" Then
                    thePolyLine.LType = line2
                End If

                If line1 = "90" Then
                    numberOfVertices = Convert.ToInt32(line2)
                End If

                If line1 = "70" Then
                    openOrClosed = Convert.ToInt32(line2)
                End If


                If line1 = "10" Then
                    x1 = Convert.ToDouble(line2)
                    If x1 > XMax Then
                        XMax = x1
                    End If

                    If x1 < XMin Then
                        XMin = x1
                    End If
                End If

                If line1 = "20" And Not line2.StartsWith("0") Then
                    y1 = Convert.ToDouble(line2)

                    If y1 > YMax Then
                        YMax = y1
                    End If

                    If y1 < YMin Then
                        YMin = y1
                    End If

                    'TODO: Vanni, 17/10/2016: verificare il perchè nella versione originale viene letto un segno meno...
                    'pointList.Add(New Point(CInt(Math.Truncate(x1)), CInt(Math.Truncate(-y1))))
                    pointList.Add(New Point(x1, y1))
                    counter += 1


                End If
                'Loop While counter < numberOfVertices
            Loop While line2 <> "SEQEND"
            numberOfVertices = counter

            If String.IsNullOrEmpty(thePolyLine.LType) Then
                thePolyLine.LType = "CONTINUA"
            End If



            ''****************************************************************************************************//
            ''***************This Part is related with the drawing editor...the data taken from the dxf file******//
            ''***************is interpreted hereinafter***********************************************************//


            For i As Integer = 1 To numberOfVertices - 1
                thePolyLine.AppendLine(New Line(DirectCast(pointList(i - 1), Point), DirectCast(pointList(i), Point), 1))
            Next

            If openOrClosed = 1 Then
                thePolyLine.AppendLine(New Line(DirectCast(pointList(numberOfVertices - 1), Point), DirectCast(pointList(0), Point), 1))
            End If

            thePolyLine.shapeIdentifier = 5
            Dim ix As Integer = drawingList.Add(thePolyLine)
            objectIdentifier.Add(New DrawingObject(5, ix))

            'If (Math.Abs(XMax) - Math.Abs(XMin)) > Me.pictureBox1.Size.Width Then
            '    scaleX = CDbl(Me.pictureBox1.Size.Width) / CDbl(Math.Abs(XMax) - Math.Abs(XMin))
            'Else
            '    scaleX = 1
            'End If


            'If (Math.Abs(YMax) - Math.Abs(YMin)) > Me.pictureBox1.Size.Height Then
            '    scaleY = CDbl(Me.pictureBox1.Size.Height) / CDbl(Math.Abs(YMax) - Math.Abs(YMin))
            'Else
            '    scaleY = 1
            'End If

            'mainScale = Math.Min(scaleX, scaleY)

            ' '''///////////////////////////////////////////////////////////////////////////////////////////////////
            ' '''///////////////////////////////////////////////////////////////////////////////////////////////////


        End Sub


        Private Sub CircleModule(reader As StreamReader)
            'Interpretes circle objects in the DXF file
            Dim line1 As String, line2 As String
            line1 = "0"
            line2 = "0"

            Dim x1 As Double = 0
            Dim y1 As Double = 0

            Dim radius As Double = 0

            Do
                GetLineCouple(reader, line1, line2)

                If line1 = "10" Then

                    x1 = Convert.ToDouble(line2)
                End If


                If line1 = "20" Then

                    y1 = Convert.ToDouble(line2)
                End If


                If line1 = "40" Then
                    radius = Convert.ToDouble(line2)

                    If (x1 + radius) > XMax Then
                        XMax = x1 + radius
                    End If

                    If (x1 - radius) < XMin Then
                        XMin = x1 - radius
                    End If

                    If y1 + radius > YMax Then
                        YMax = y1 + radius
                    End If

                    If (y1 - radius) < YMin Then
                        YMin = y1 - radius

                    End If



                End If
            Loop While line1 <> "40"

            ''****************************************************************************************************//
            ''***************This Part is related with the drawing editor...the data taken from the dxf file******//
            ''***************is interpreted hereinafter***********************************************************//


            'If (Math.Abs(XMax) - Math.Abs(XMin)) > Me.pictureBox1.Size.Width Then
            '    scaleX = CDbl(Me.pictureBox1.Size.Width) / CDbl(Math.Abs(XMax) - Math.Abs(XMin))
            'Else
            '    scaleX = 1
            'End If


            'If (Math.Abs(YMax) - Math.Abs(YMin)) > Me.pictureBox1.Size.Height Then
            '    scaleY = CDbl(Me.pictureBox1.Size.Height) / CDbl(Math.Abs(YMax) - Math.Abs(YMin))
            'Else
            '    scaleY = 1
            'End If

            'mainScale = Math.Min(scaleX, scaleY)


            Dim ix As Integer = drawingList.Add(New circle(New Point(x1, -y1), radius, 1))
            objectIdentifier.Add(New DrawingObject(4, ix))

            ' '''///////////////////////////////////////////////////////////////////////////////////////////////////
            ' '''///////////////////////////////////////////////////////////////////////////////////////////////////

        End Sub


        Private Sub ArcModule(reader As StreamReader)
            'Interpretes arc objects in the DXF file
            Dim line1 As String, line2 As String
            line1 = "0"
            line2 = "0"

            Dim x1 As Double = 0
            Dim y1 As Double = 0

            Dim radius As Double = 0
            Dim angle1 As Double = 0
            Dim angle2 As Double = 0

            Do
                GetLineCouple(reader, line1, line2)

                If line1 = "10" Then
                    x1 = Convert.ToDouble(line2)
                    If x1 > XMax Then
                        XMax = x1
                    End If
                    If x1 < XMin Then
                        XMin = x1

                    End If
                End If


                If line1 = "20" Then
                    y1 = Convert.ToDouble(line2)
                    If y1 > YMax Then
                        YMax = y1
                    End If
                    If y1 < YMin Then
                        YMin = y1
                    End If
                End If


                If line1 = "40" Then
                    radius = Convert.ToDouble(line2)

                    If (x1 + radius) > XMax Then
                        XMax = x1 + radius
                    End If

                    If (x1 - radius) < XMin Then
                        XMin = x1 - radius
                    End If

                    If y1 + radius > YMax Then
                        YMax = y1 + radius
                    End If

                    If (y1 - radius) < YMin Then
                        YMin = y1 - radius
                    End If
                End If

                If line1 = "50" Then
                    angle1 = Convert.ToDouble(line2)
                End If

                If line1 = "51" Then
                    angle2 = Convert.ToDouble(line2)


                End If
            Loop While line1 <> "51"


            '****************************************************************************************************//
            ''***************This Part is related with the drawing editor...the data taken from the dxf file******//
            ''***************is interpreted hereinafter***********************************************************//


            'If (Math.Abs(XMax) - Math.Abs(XMin)) > Me.pictureBox1.Size.Width Then
            '    scaleX = CDbl(Me.pictureBox1.Size.Width) / CDbl(Math.Abs(XMax) - Math.Abs(XMin))
            'Else
            '    scaleX = 1
            'End If


            'If (Math.Abs(YMax) - Math.Abs(YMin)) > Me.pictureBox1.Size.Height Then
            '    scaleY = CDbl(Me.pictureBox1.Size.Height) / CDbl(Math.Abs(YMax) - Math.Abs(YMin))
            'Else
            '    scaleY = 1
            'End If

            'mainScale = Math.Min(scaleX, scaleY)


            Dim ix As Integer = drawingList.Add(New arc(New Point(x1, -y1), radius, angle1, angle2, 1))
            objectIdentifier.Add(New DrawingObject(6, ix))

            ' '''///////////////////////////////////////////////////////////////////////////////////////////////////
            ' '''///////////////////////////////////////////////////////////////////////////////////////////////////

        End Sub


#End Region

        Private Sub TextModule(reader As StreamReader, ByRef punto As circle)
            'Interpretes arc objects in the DXF file
            Dim line1 As String, line2 As String
            line1 = "0"
            line2 = "0"

            Dim x1 As Double = 0
            Dim y1 As Double = 0
            Dim CodiceAssociato As String = ""
            Dim radius As Double

            Do
                GetLineCouple(reader, line1, line2)

                If line1 = "1" Then
                    CodiceAssociato = line2
                End If

                If line1 = "10" Then
                    x1 = Convert.ToDouble(line2)
                End If


                If line1 = "20" Then
                    y1 = Convert.ToDouble(line2)
                End If

                If line1 = "40" Then
                    radius = Convert.ToDouble(line2)
                End If

            Loop While line1 <> "1" 'verificare se usare 50 o 1 ...

            punto = New circle(New Point(x1, -y1), radius, 1)
            punto.CodiceAssociato = CodiceAssociato

            Dim ix As Integer = drawingList.Add(punto)
            objectIdentifier.Add(New DrawingObject(4, ix))


        End Sub



    End Class
End Namespace