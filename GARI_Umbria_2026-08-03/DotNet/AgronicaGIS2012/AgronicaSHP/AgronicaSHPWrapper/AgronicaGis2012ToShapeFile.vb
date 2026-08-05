Imports <xmlns="http://www.agronica.it/grafica/">


Imports AgronicaConversioneCartografiaGias.FormatsConverter
Imports AgronicaGIS2012.Commons
Imports System.Windows
Imports System.Xml
Imports System.Xml.XPath

Public Class AgronicaGis2012ToShapeFile



    Private _loadedSapefile As New TestShapeFile.ShapeFile
    Public Property LoadedSapefile() As TestShapeFile.ShapeFile
        Get
            Return _loadedSapefile
        End Get
        Set(value As TestShapeFile.ShapeFile)
            _loadedSapefile = value
        End Set
    End Property

    Public Function Converti_Da_DT(ByVal dt As DataTable, ByVal filename As String) As Boolean

        Dim estraiCoordinate As New GML

        Dim totalLength As Integer = 50 'size in 16bit word format
        Dim AgronicaEntityLength As Integer = 0 'size in 16bit word format

        Dim headerXMin, headerXMax, headerYMin, headerYMax As Double
        Dim XMin, XMax, YMin, YMax As Double

        headerXMin = 9000000000
        headerXMax = -9000000000
        headerYMin = 9000000000
        headerYMax = -9000000000

        Dim cRecordCount As Integer = 1

        Dim AttributeTable As New DataTable
        For Each col In dt.Columns
            If col.ColumnName <> "Gml" Then
                AttributeTable.Columns.Add(New DataColumn(col.columnName, GetType(String)))
            End If
        Next

        For Each row In dt.Rows

            XMin = 9000000000
            XMax = -9000000000
            YMin = 9000000000
            YMax = -9000000000

            AgronicaEntityLength = 0
            Dim shape As New TestShapeFile.ShapeFileRecord

            Dim coord As New List(Of xyz)
            coord = estraiCoordinate.EstraiCoordinateDaGML(row("Gml").ToString, True, True)
            Dim coordCount As Integer = coord.Count

            For Each c In coord
                shape.Points.Add(New System.Windows.Point With {
                                 .X = c.X,
                                 .Y = c.Y
                                 })

                setxyHeader(headerXMin, headerXMax, headerYMin, headerYMax, c)
                setxyHeader(XMin, XMax, YMin, YMax, c)

            Next

            'due valori double, cioè 1 solo valore in formato word
            AgronicaEntityLength += (coordCount * TestShapeFile.ShapeFileConstants.DoubleSizeBytes)

            'PolyLine & Polygon Record Contents
            'Byte 0 Shape Type 3 Integer 1 Little
            'Byte 4 Box Box Double 4 Little
            'Byte 36 NumParts NumParts Integer 1 Little
            'Byte 40 NumPoints NumPoints Integer 1 Little
            'Byte 44 Parts Parts Integer NumParts Little
            'Byte X Points Points Point NumPoints Little

            'quindi sono 48 byte diviso due 24

            Select Case coordCount
                Case 1
                    shape.ShapeType = TestShapeFile.ShapeType.Point
                    AgronicaEntityLength += 2
                Case 2
                    shape.ShapeType = TestShapeFile.ShapeType.PolyLine
                    AgronicaEntityLength += 24
                Case Else
                    shape.ShapeType = TestShapeFile.ShapeType.Polygon
                    AgronicaEntityLength += 24
            End Select

            Dim attr_row As DataRow = AttributeTable.NewRow
            For Each col In AttributeTable.Columns
                attr_row(col.columnName) = row(col.ColumnName)
            Next

            shape.Attributes = attr_row
            shape.RecordNumber = cRecordCount
            shape.ContentLength = AgronicaEntityLength

            totalLength += AgronicaEntityLength + (TestShapeFile.ShapeFileConstants.ShapeRecordHeaderByteLength / 2)

            shape.Parts.Add(0)
            shape.XMax = XMax
            shape.YMax = YMax
            shape.XMin = XMin
            shape.YMin = YMin

            'reset
            AgronicaEntityLength = 0
            cRecordCount += 1

            _loadedSapefile.Records.Add(shape)

        Next


        _loadedSapefile.FileHeader.FileLength = totalLength
        _loadedSapefile.FileHeader.Version = TestShapeFile.ShapeFileConstants.VersionCode
        _loadedSapefile.FileHeader.ShapeType = _loadedSapefile.Records(0).ShapeType 'todo, verificare...
        _loadedSapefile.FileHeader.XMax = headerXMax
        _loadedSapefile.FileHeader.YMax = headerYMax
        _loadedSapefile.FileHeader.XMin = headerXMin
        _loadedSapefile.FileHeader.YMin = headerYMin


        _loadedSapefile.Save(filename)

        Return True

    End Function

    Public Function Convert(
            ByVal xmlToConvert As String,
            ByVal filename As String,
            ByVal importaAttributiTipizzati As Boolean,
            DBFRimappaturaDati As List(Of DBFDataModel_MappaturaDati)) As Boolean

        Dim Xdoc As XDocument = XDocument.Parse(xmlToConvert)

        Dim estraiCoordinate As New GML

        Dim totalLength As Integer = 50 'size in 16bit word format
        Dim AgronicaEntityLength As Integer = 0 'size in 16bit word format

        Dim headerXMin, headerXMax, headerYMin, headerYMax As Double
        Dim XMin, XMax, YMin, YMax As Double

        headerXMin = 9000000000
        headerXMax = -9000000000
        headerYMin = 9000000000
        headerYMax = -9000000000

        Dim cRecordCount As Integer = 1


        For Each CurrEnt As XElement In (
            From n In Xdoc.<DatiEntita>.<Entita>
            Select n)


            XMin = 9000000000
            XMax = -9000000000
            YMin = 9000000000
            YMax = -9000000000

            AgronicaEntityLength = 0
            Dim shape As New TestShapeFile.ShapeFileRecord

            Dim coord As New List(Of xyz)
            coord = estraiCoordinate.EstraiCoordinateDaPoligono(CurrEnt.ToString, True)
            Dim coordCount As Integer = coord.Count


            For Each c In coord
                shape.Points.Add(New System.Windows.Point With {
                                 .X = c.X,
                                 .Y = c.Y
                                 })

                setxyHeader(headerXMin, headerXMax, headerYMin, headerYMax, c)
                setxyHeader(XMin, XMax, YMin, YMax, c)

            Next


            'record header (for Each record), therefore, contributes ( [8 byte = 4 word] + content length) --> due valori integer ciè 1 solo in formato word
            'GABRIELE AgronicaEntityLength = AgronicaEntityLength + (TestShapeFile.ShapeFileConstants.ShapeRecordHeaderByteLength / 2)

            'due valori double, cioè 1 solo valore in formato word
            AgronicaEntityLength += (coordCount * TestShapeFile.ShapeFileConstants.DoubleSizeBytes)

            'PolyLine & Polygon Record Contents
            'Byte 0 Shape Type 3 Integer 1 Little
            'Byte 4 Box Box Double 4 Little
            'Byte 36 NumParts NumParts Integer 1 Little
            'Byte 40 NumPoints NumPoints Integer 1 Little
            'Byte 44 Parts Parts Integer NumParts Little
            'Byte X Points Points Point NumPoints Little

            'quindi sono 48 byte diviso due 24

            Select Case coordCount
                Case 1
                    shape.ShapeType = TestShapeFile.ShapeType.Point
                    AgronicaEntityLength += 2
                Case 2
                    shape.ShapeType = TestShapeFile.ShapeType.PolyLine
                    AgronicaEntityLength += 24
                Case Else
                    shape.ShapeType = TestShapeFile.ShapeType.Polygon
                    AgronicaEntityLength += 24
            End Select

            shape.Attributes = CreateDataRowFromText(CurrEnt, importaAttributiTipizzati, DBFRimappaturaDati)
            shape.RecordNumber = cRecordCount
            shape.ContentLength = AgronicaEntityLength

            totalLength += AgronicaEntityLength + (TestShapeFile.ShapeFileConstants.ShapeRecordHeaderByteLength / 2)

            shape.Parts.Add(0)
            shape.XMax = XMax
            shape.YMax = YMax
            shape.XMin = XMin
            shape.YMin = YMin

            'reset
            AgronicaEntityLength = 0
            cRecordCount += 1

            _loadedSapefile.Records.Add(shape)

        Next


        _loadedSapefile.FileHeader.FileLength = totalLength
        _loadedSapefile.FileHeader.Version = TestShapeFile.ShapeFileConstants.VersionCode
        _loadedSapefile.FileHeader.ShapeType = _loadedSapefile.Records(0).ShapeType 'todo, verificare...
        _loadedSapefile.FileHeader.XMax = headerXMax
        _loadedSapefile.FileHeader.YMax = headerYMax
        _loadedSapefile.FileHeader.XMin = headerXMin
        _loadedSapefile.FileHeader.YMin = headerYMin


        _loadedSapefile.Save(filename)

        Return True
    End Function

    Private Shared Sub setxyHeader(ByRef headerXMin As Double, ByRef headerXMax As Double, ByRef headerYMin As Double, ByRef headerYMax As Double, ByVal c As xyz)
        If headerXMin > c.X Then
            headerXMin = c.X
        End If

        If headerYMin > c.Y Then
            headerYMin = c.Y
        End If

        If headerXMax < c.X Then
            headerXMax = c.X
        End If


        If headerYMax < c.Y Then
            headerYMax = c.Y
        End If
    End Sub
    Private Function CreateDataRowFromText(
            CurrEnt As XElement,
            ByVal importaAttributiTipizzati As Boolean,
            DBFRimappaturaDati As List(Of DBFDataModel_MappaturaDati)) As DataRow

        Dim AttributeTable As New DataTable

        Dim text As String

        text = CreateDataRowFromTextRimappatura(CurrEnt, DBFRimappaturaDati)

        Dim splitted As String() = text.Split("|")
        Dim key_value As String()

        Dim drow As DataRow = AttributeTable.NewRow

        Dim fatto As Boolean = False

        For Each s In splitted

            If String.IsNullOrEmpty(s) Then
                s = "Info§-"
            End If

            fatto = False
            If Not s.Contains("§") Then
                s = "Info§" & s
            End If
            key_value = s.Split("§")
            key_value(1) = key_value(1).TrimStart(" ")

            If importaAttributiTipizzati Then
                Try
                    If IsNumeric(key_value(1)) Then
                        Dim dblApp As Double = xyz.myCDBL(key_value(1))
                        AttributeTable.Columns.Add(New DataColumn(key_value(0), GetType(Double)))
                        drow.Item(key_value(0)) = dblApp
                        fatto = True
                    End If
                Catch e As Exception
                End Try

                If Not fatto Then
                    Try
                        If (key_value(1).Contains("/") Or key_value(1).Contains("-")) AndAlso IsDate(key_value(1)) Then
                            Dim datetimeApp As DateTime = CType(key_value(1), DateTime)
                            AttributeTable.Columns.Add(New DataColumn(key_value(0), GetType(DateTime)))
                            drow.Item(key_value(0)) = datetimeApp
                            fatto = True
                        End If
                    Catch ex As Exception

                    End Try

                End If
            End If

            If Not fatto Then
                AttributeTable.Columns.Add(New DataColumn(key_value(0), GetType(String)))
                drow.Item(key_value(0)) = key_value(1)
                fatto = True
            End If





        Next


        Return drow

    End Function

    Private Function CreateDataRowFromTextRimappatura(curElem As XElement, Rimappatura As List(Of DBFDataModel_MappaturaDati)) As String

        Dim txtElem As String = curElem.@text

        Dim listaRimappa As List(Of String) = txtElem.Split("|").ToList
        Dim listaFinale As New List(Of String)

        If IsNothing(Rimappatura) OrElse Rimappatura.Count = 0 Then
            Return txtElem
        End If

        For Each cfg In Rimappatura

            Select Case cfg.Operazione
                Case DBFDataModel_MappaturaDati_Costanti.Operazione_NuovaEtichetta
                    Dim v1 As String = (From a In listaRimappa Where a.StartsWith(cfg.CampoDaRimappare)).FirstOrDefault
                    If Not String.IsNullOrEmpty(v1) Then


                        If cfg.ValoreDefault.Contains("xpath") Then

                            Dim namespaceManager As New XmlNamespaceManager(New NameTable())
                            namespaceManager.AddNamespace("empty", "http://www.agronica.it/grafica/")
                            Dim oDoc As XDocument = New XDocument()
                            oDoc.Add(curElem)
                            Dim operations As String() = cfg.ValoreDefault.Split("|")
                            Dim expression As String = operations(1)
                            Dim ValueModifier As String() = {}
                            If operations.Length > 2 Then
                                ValueModifier = operations(2).Split("§")
                            End If
                            Dim e = CType(oDoc.XPathSelectElement(expression, namespaceManager), XElement)
                            Dim valFromXpath As String
                            If Not IsNothing(e) Then
                                valFromXpath = e.Value

                                If ValueModifier.Length > 0 Then
                                    For index = 0 To ValueModifier.Length - 2 Step 2

                                        Select Case ValueModifier(index)
                                            Case "double"
                                                Dim d1 As Double
                                                Dim sDeci As String = AgronicaCoreUtility.CulturaHelper.SeparatoreDecimaleVB
                                                Dim ok1 As Boolean = Double.TryParse(valFromXpath.Replace(".", sDeci), d1)
                                                If ok1 Then
                                                    ValueModifier(index + 1) = ValueModifier(index + 1).Replace("value", d1.ToString().Replace(",", sDeci))


                                                    Dim stf As New AgronicaCoreUtility.StringToFormula
                                                    d1 = stf.Eval(ValueModifier(index + 1))

                                                    valFromXpath = d1.ToString.Replace(sDeci, ".")
                                                End If
                                            Case "string"
                                            Case "date"
                                        End Select

                                    Next
                                End If

                                listaFinale.Add(cfg.NuovoCampo & "§ " & valFromXpath)
                            Else
                                Throw New Exception("CreateDataRowFromTextRimappatura, Espressione xpath non valida")
                            End If
                        Else
                            listaFinale.Add(cfg.NuovoCampo & "§ " & cfg.ValoreDefault)
                        End If

                    Else
                        listaFinale.Add(v1)
                    End If

                Case DBFDataModel_MappaturaDati_Costanti.Operazione_RimappaEtichetta
                    Dim v1 As String = (From a In listaRimappa Where a.StartsWith(cfg.CampoDaRimappare)).First.Replace(cfg.CampoDaRimappare, cfg.NuovoCampo)
                    listaFinale.Add(v1)
                Case cfg.Operazione = DBFDataModel_MappaturaDati_Costanti.Operazione_RimuoviEtichetta
            End Select

        Next

        Return String.Join("|", listaFinale)

    End Function

End Class
