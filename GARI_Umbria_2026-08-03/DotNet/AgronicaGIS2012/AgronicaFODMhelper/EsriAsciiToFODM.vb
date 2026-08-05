Imports System.Configuration
Imports System.Collections.Generic
Imports System.Text
Imports AgronicaGIS2012.Commons
Imports AgronicaConversioneCartografiaGias.Agronica


Namespace CalcoloPianoConcimazioneVariabile
    Public Class ESRIAsciiToXml

        Private Shared _AgronicaPuntoConcimazioneLatOffest As Double = 0

        Public Shared Sub ConvertFile( _
                          ByVal sNomeInputFile As String _
                        , sTipoIrrigazione As String _
                        , sUnitaBase As String _
                        , sGradoConfidenza As String _
                        , sDataFineValidita As String _
                        , sTargetFile As String _
                        , ByVal sColtivation As String _
                        , sPoligonoAppezzamentoWKT As String _
                        , sCoordinateSystem As String _
                        , ByVal parconverterED50WG84 As ParametriCoordinateConverter _
                        , ByVal parconverterWG84ED50 As ParametriCoordinateConverter _
                        , ByVal pPuntiRiferimentoRotazione As List(Of xyz) _
            )


            ' - Controllo dell'esistenza del file di input
            If Not System.IO.File.Exists(sNomeInputFile) Then
                Throw New Exception("Il file di input non esiste.")
            End If

            If Not System.IO.Directory.Exists(sTargetFile.Substring(0, sTargetFile.LastIndexOf("\"))) Then
                Throw New Exception("La directory di input non esiste.")
            End If

            Dim InputFileContent As String() = System.IO.File.ReadAllLines(sNomeInputFile, Encoding.ASCII)
            Convertstring(InputFileContent, sTipoIrrigazione, sUnitaBase, sGradoConfidenza, sDataFineValidita, sTargetFile, sColtivation, sPoligonoAppezzamentoWKT, sCoordinateSystem, "", "", "", False, parconverterED50WG84, parconverterWG84ED50, pPuntiRiferimentoRotazione, Nothing)


        End Sub

        Public Shared Function Convertstring( _
                       sEsriSting As String() _
                     , sTipoIrrigazione As String _
                     , sUnitaBase As String _
                     , sGradoConfidenza As String _
                     , sDataFineValidita As String _
                     , sTargetFile As String _
                     , sColtivation As String _
                     , sPoligonoAppezzamentoWKT As String _
                     , sCoordinateSystem As String _
                     , ByVal FileName As String _
                     , ByVal EXAppezzamento As String _
                     , ByVal EXImpianto As String _
                     , ByVal ApplicaTrasformazione As Boolean _
                     , ByVal parconverterED50WG84 As ParametriCoordinateConverter _
                     , ByVal parconverterWG84ED50 As ParametriCoordinateConverter _
                     , ByVal pPuntiRiferimentoRotazione As List(Of xyz) _
                     , ByVal baricentroPoligonoAppezzamento As xyz _
                    ) As String
            ' - Dichiarazione delle variabili per gli argomenti 

            ' - Dichiarazione delle variabili per le informazioni contenute nel file di input
            Dim nCols As Integer = 0
            Dim nRows As Integer = 0

            Dim XLowerLeftCorner As [Double] = 0
            Dim YLowerLeftCorner As [Double] = 0
            Dim CellSize As [Double] = 0

            Dim YUpperLeftCorner As [Double] = 0
            '
            '            // - Lettura degli argomenti
            '            

            ' - Argomenti base
            '                sNomeInputFile = sNomeInputFile;
            '                sTargetFile = sTargetFile;
            '                 - Informazioni da aggiungere nel file
            '                sTipoIrrigazione = sTipoIrrigazione;
            '                sUnitaBase = sUnitaBase;
            '                sGradoConfidenza = sGradoConfidenza;
            '                sDataFineValidita = sDataFineValidita;
            '                sColtivation = sColtivation;
            '                


            Try
            Catch exc As Exception
                Dim sError As String = "Errore: " & exc.Message & " - Source: " & exc.Source
                Throw New Exception(sError)
            End Try

            Try
                '
                '                // - Validazione degli argomenti
                '                


                '
                '                // - Inizio trasformazione
                '                


                'System.IO.StreamReader oR = new System.IO.StreamReader(sNomeInputFile, System.Text.Encoding.ASCII);	
                '                string sFile = oR.ReadToEnd();
                '                string[] InputFileContent = sFile.Split('\n');
                '				
                '                 for(int iIndex =0; iIndex < InputFileContent.Length; iIndex++)
                '                {
                '                    if (InputFileContent[iIndex].Length > 0)
                '                    {
                '                        if(InputFileContent[iIndex][InputFileContent[iIndex].Length-1].CompareTo('\r') == 0)
                '                            InputFileContent[iIndex] = InputFileContent[iIndex].Substring(0, InputFileContent[iIndex].Length-1);
                '                    }
                '                } 

                Dim InputFileContent As String() = sEsriSting

                ' - Righe e colonne
                nCols = sEsriSting(5).Split(" ").Length - 2 ' Convert.ToInt32(InputFileContent(0).Replace("ncols", "").TrimStart())
                nRows = sEsriSting.Length - 7 ' Convert.ToInt32(InputFileContent(1).Replace("nrows", "").TrimStart())

                ' - Coordinate angolo in basso a sx
                XLowerLeftCorner = Convert.ToDouble(InputFileContent(2).Substring(13).Replace(".", ","))
                YLowerLeftCorner = Convert.ToDouble(InputFileContent(3).Substring(13).Replace(".", ","))

                ' - Unità di misura
                CellSize = Convert.ToDouble(InputFileContent(4).Substring(13).Replace(".", ","))

                ' - Calcolo della coordinata Y dell'angolo in alto a sx
                YUpperLeftCorner = YLowerLeftCorner + (nRows * CellSize)

                Dim oMatrix As Object(,) = New Object(nRows, nCols) {}
                ' - Calcolo le coordinate per ogni valore
                For iIndex As Integer = 5 To InputFileContent.GetUpperBound(0)
                    Dim sTemp As String = InputFileContent(iIndex).TrimStart().TrimEnd()
                    Dim charSeparators As Char() = New Char() {" "c}
                    Dim FileRow As String() = sTemp.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries)
                    System.Diagnostics.Debug.Write(iIndex.ToString())
                    For jIndex As Integer = 0 To FileRow.GetUpperBound(0)
                        System.Diagnostics.Debug.Write(jIndex.ToString())
                        Dim fValue As [Double] = Convert.ToDouble(FileRow(jIndex).Replace(".", ","))
                        Dim XPointCenter As [Double] = XLowerLeftCorner + (jIndex * CellSize) + (CellSize / 2)
                        Dim YPointCenter As [Double] = YUpperLeftCorner - ((iIndex - 5) * CellSize) - (CellSize / 2)

                        Dim oPoint As New PointObject()
                        oPoint.Value = fValue
                        oPoint.XCoord = XPointCenter
                        oPoint.YCoord = YPointCenter


                        oMatrix(iIndex - 5, jIndex) = oPoint                        
                    Next
                Next
                ' System.Diagnostics.Debug.
                ' - Creazione del file XML

                Dim oTargetDom As New System.Xml.XmlDocument()


                'creazione da modello
                If System.IO.File.Exists(FileName) Then
                    oTargetDom.Load(FileName)
                Else
                    FileName = "-1"
                End If

                Dim oTipoIrr As System.Xml.XmlNode
                If FileName = "-1" Then
                    Dim oGeoNode As System.Xml.XmlNode = oTargetDom.AppendChild(oTargetDom.CreateNode(System.Xml.XmlNodeType.Element, "GeoAnswer", ""))

                    Dim oNTmp As System.Xml.XmlNode = oGeoNode.AppendChild(oTargetDom.CreateNode(System.Xml.XmlNodeType.Element, "EndPeriod", ""))
                    oNTmp.InnerText = sDataFineValidita

                    oNTmp = oGeoNode.AppendChild(oTargetDom.CreateNode(System.Xml.XmlNodeType.Element, "Cultivation", ""))
                    oNTmp.InnerText = sColtivation

                    oTipoIrr = oGeoNode.AppendChild(oTargetDom.CreateNode(System.Xml.XmlNodeType.Element, sTipoIrrigazione, ""))
                    Dim oAttr1 As System.Xml.XmlAttribute = oTargetDom.CreateAttribute("BaseUnit")
                    oAttr1.Value = sUnitaBase
                    Dim oAttr2 As System.Xml.XmlAttribute = oTargetDom.CreateAttribute("PixelUnit")
                    oAttr2.Value = CellSize.ToString()
                    Dim oAttr3 As System.Xml.XmlAttribute = oTargetDom.CreateAttribute("ConfidentialLevel")
                    oAttr3.Value = sGradoConfidenza

                    oTipoIrr.Attributes.Append(oAttr1)
                    oTipoIrr.Attributes.Append(oAttr2)
                    oTipoIrr.Attributes.Append(oAttr3)
                Else
                    oTipoIrr = oTargetDom.SelectSingleNode("//FODM/Resources/RxList/RxPoly/Records")

                    'clear veloce!
                    oTipoIrr.InnerXml = ""
                End If

                ''la distanza fra punti dipende ovviamente dal sistema di riferimento

                Dim p1 As PointObject, p2 As PointObject
                If ApplicaTrasformazione Then
                    p1 = getMinLatitude(oMatrix)
                    p2 = getMinLatitudeWKTPolygon(TransformWKTPolygonED50ToWGS84(sPoligonoAppezzamentoWKT, True, parconverterWG84ED50))
                    _AgronicaPuntoConcimazioneLatOffest = TransformED50ToWGSDecimal(p2, parconverterED50WG84).YCoord - TransformED50ToWGSDecimal(p1, parconverterED50WG84).YCoord
                End If
                'fine 

                Dim distanceBetweenPoints As Double
                distanceBetweenPoints = CellSize

                'if (ConfigurationSettings.AppSettings["ApplicaTrasformazione"] == "true")
                '    distanceBetweenPoints = CellSize;
                '    //distanceBetweenPoints = getDistance(TransformED50ToWGSDecimal ((PointObject)oMatrix[0, 0]), TransformED50ToWGSDecimal ((PointObject)oMatrix[0, 1]));
                'else
                '    distanceBetweenPoints = CellSize;

                Dim theta As Double = 0

                For i As Integer = 0 To oMatrix.GetUpperBound(0)
                    For j As Integer = 0 To oMatrix.GetUpperBound(1)
                        Dim oPoint As PointObject = DirectCast(oMatrix(i, j), PointObject)

                        'Formato ISOBUS
                        '
                        '                        System.Xml.XmlNode oPointNode = oTargetDom.CreateNode(System.Xml.XmlNodeType.Element, "point", "");
                        '                        System.Xml.XmlAttribute oPAttr1 = oTargetDom.CreateAttribute("E");
                        '                        
                        '                        //oPAttr1.Value = oPoint.XCoord.ToString();
                        '                        oPAttr1.Value = oTranslatedWGS84.XCoord.ToString();
                        '                        System.Xml.XmlAttribute oPAttr2 = oTargetDom.CreateAttribute("N");
                        '                        
                        '                        //oPAttr2.Value = oPoint.YCoord.ToString();
                        '                        oPAttr2.Value = oTranslatedWGS84.YCoord.ToString();
                        '
                        '                        oPointNode.Attributes.Append(oPAttr1);
                        '                        oPointNode.Attributes.Append(oPAttr2);
                        '
                        '                        oPointNode.InnerText = oPoint.Value.ToString("#0.000");
                        '                        oTipoIrr.AppendChild(oPointNode);
                        '                         

                        'Fine Formato ISOBUS


                        'Formato FDOM

                        'Piano di prescrizione variabile



                        If oPoint.Value <> 0.0 Then
                            Dim oPointRecord As System.Xml.XmlNode = oTargetDom.CreateNode(System.Xml.XmlNodeType.Element, "Record", "")
                            Dim oPointGeometryWKT As System.Xml.XmlNode = oTargetDom.CreateNode(System.Xml.XmlNodeType.Element, "GeometryWKT", "")
                            Dim oPointValue As System.Xml.XmlNode = oTargetDom.CreateNode(System.Xml.XmlNodeType.Element, "Value", "")
                            Dim oValueKey As System.Xml.XmlAttribute = oTargetDom.CreateAttribute("ValueKey")
                            oValueKey.Value = "a:1"
                            oPointValue.Attributes.Append(oValueKey)
                            oPointValue.InnerText = oPoint.Value.ToString("#0.000")
                            oPointGeometryWKT.InnerText = DatoPuntoDammiQuadrato_WKTFormat(oPoint, distanceBetweenPoints, ApplicaTrasformazione, parconverterED50WG84, pPuntiRiferimentoRotazione, theta, baricentroPoligonoAppezzamento)

                            oPointRecord.AppendChild(oPointGeometryWKT)
                            oPointRecord.AppendChild(oPointValue)
                            oTipoIrr.AppendChild(oPointRecord)

                        End If
                    Next
                Next

                'Poligono appezzamento ed impianto
                If FileName = "-1" Then
                    Dim oPoligonoAppezzamento As System.Xml.XmlNode = oTargetDom.CreateNode(System.Xml.XmlNodeType.Element, "Appezzamento", "")
                    oPoligonoAppezzamento.InnerText = TransformWKTPolygonED50ToWGS84(sPoligonoAppezzamentoWKT, False, Nothing)
                    oTipoIrr.AppendChild(oPoligonoAppezzamento)
                Else
                    oTargetDom.SelectSingleNode("//FODM/Resources/DomainList/Domain[@uri='" & EXAppezzamento & "']/BoundaryWKT").InnerText = TransformWKTPolygonED50ToWGS84(sPoligonoAppezzamentoWKT, False, Nothing)

                    oTargetDom.SelectSingleNode("//FODM/Resources/DomainList/Domain[@uri='" & EXImpianto & "']/BoundaryWKT").InnerText = TransformWKTPolygonED50ToWGS84(sPoligonoAppezzamentoWKT, False, Nothing)
                End If

                'System.Xml.XmlNode oCoordinateSystem = oTargetDom.CreateNode(System.Xml.XmlNodeType.Element, "SistemaRiferimento", "");
                'oCoordinateSystem.InnerText = sCoordinateSystem;
                'oTipoIrr.AppendChild(oCoordinateSystem);

                If sTargetFile <> "-1" Then
                    oTargetDom.Save(sTargetFile)


                    ' - Creazione del file ASCII
                    Dim oStream As System.IO.FileStream = System.IO.File.Open(sTargetFile.Replace(".xml", ".txt"), System.IO.FileMode.CreateNew)
                    Dim oStreamW As New System.IO.StreamWriter(oStream)
                    For i As Integer = 0 To oMatrix.GetUpperBound(0)
                        For j As Integer = 0 To oMatrix.GetUpperBound(1)
                            Dim oPoint As PointObject = DirectCast(oMatrix(i, j), PointObject)
                            oStreamW.WriteLine(oPoint.XCoord.ToString() & ";" & oPoint.YCoord.ToString() & ";" & oPoint.Value.ToString("#0.000"))
                        Next
                    Next

                    oStreamW.Flush()
                    oStreamW.Close()
                End If

                Return oTargetDom.OuterXml

            Catch exc As Exception
                Dim sError As String = "Errore: " & exc.Message & " - Source: " & exc.Source
                Throw New Exception(sError)
            End Try


            Return ""
        End Function

        Private Shared Function getMinLatitude(oMatrix As Object(,)) As PointObject
            Dim rval As New PointObject()
            Dim app As New PointObject()

            Dim minx As Double = 0, minY As Double = 500000000.0

            For i As Integer = 0 To oMatrix.GetUpperBound(0)
                For j As Integer = 0 To oMatrix.GetUpperBound(1)

                    app = DirectCast(oMatrix(i, j), PointObject)
                    If app.YCoord < minY AndAlso app.Value <> 0 Then
                        minx = app.XCoord

                        minY = app.YCoord

                    End If
                Next
            Next

            rval.XCoord = app.XCoord
            rval.YCoord = app.YCoord
            Return rval
        End Function

        Private Shared Function getDistance(p1 As PointObject, p2 As PointObject) As Double
            Return Math.Sqrt(Math.Pow((p1.XCoord - p2.XCoord), 2) + Math.Pow((p1.YCoord - p2.YCoord), 2))
        End Function

        Public Shared Function DatoPuntoDammiQuadrato_WKTFormat(Center As PointObject, DistanzaFraPunti As Double, ByVal ApplicaTrasformazione As Boolean, ByVal parametriTrasformazione As ParametriCoordinateConverter, ByVal pPuntiRiferimentoRotazione As List(Of xyz), ByRef Theta As Double, ByVal BaricentroPoligonoAppezzamento As xyz) As [String]

            Dim Center_XCoord As Double = 0 'Double.Parse(ConfigurationSettings.AppSettings("AgronicaPuntoConcimazioneLonOffest"))
            Dim Center_YCoordFromWEBConfig As Double = 0 ' Double.Parse(ConfigurationSettings.AppSettings("AgronicaPuntoConcimazioneLatOffest"))

            Dim Center_YCoord As Double = _AgronicaPuntoConcimazioneLatOffest + Center_YCoordFromWEBConfig


            Dim p1x As Double, p1y As Double, p2x As Double, p2y As Double, p3x As Double, p3y As Double, _
             p4x As Double, p4y As Double
            Dim appoggio As New PointObject()

            Dim applicaRotazione As Boolean = _
                Not pPuntiRiferimentoRotazione.Count = 0

            p1x = (Center.XCoord - (DistanzaFraPunti / 2))
            p1y = (Center.YCoord - (DistanzaFraPunti / 2))

            p2x = (Center.XCoord - (DistanzaFraPunti / 2))
            p2y = (Center.YCoord + (DistanzaFraPunti / 2))

            p3x = (Center.XCoord + (DistanzaFraPunti / 2))
            p3y = (Center.YCoord + (DistanzaFraPunti / 2))

            p4x = (Center.XCoord + (DistanzaFraPunti / 2))
            p4y = (Center.YCoord - (DistanzaFraPunti / 2))

            If applicaRotazione Then
                Dim listaXYZRotazione As New List(Of xyz)
                aggiungiXY(p1x, p1y, listaXYZRotazione)
                aggiungiXY(p2x, p2y, listaXYZRotazione)
                aggiungiXY(p3x, p3y, listaXYZRotazione)
                aggiungiXY(p4x, p4y, listaXYZRotazione)


                Dim orientamento As Integer = 1
                If Theta = 0 Then
                    Dim pAppoggioPuntiXCalcoloTheta As List(Of xyz) = matematicaGeometriaHelper.GetPPuntiRiferimentoRotazione(listaXYZRotazione, orientamento)
                    Theta = matematicaGeometriaHelper.AngoloFraSegmenti(pAppoggioPuntiXCalcoloTheta, pPuntiRiferimentoRotazione, -1)
                End If

                listaXYZRotazione = matematicaGeometriaHelper.Rotazione(listaXYZRotazione, pPuntiRiferimentoRotazione, Orientamento, Theta, BaricentroPoligonoAppezzamento, DistanzaFraPunti)

                Dim appArr As Array = _
                    listaXYZRotazione.ToArray

                p1x = CType(appArr(0), xyz).X
                p1y = CType(appArr(0), xyz).Y

                p2x = CType(appArr(1), xyz).X
                p2y = CType(appArr(1), xyz).Y

                p3x = CType(appArr(2), xyz).X
                p3y = CType(appArr(2), xyz).Y

                p4x = CType(appArr(3), xyz).X
                p4y = CType(appArr(3), xyz).Y

            End If

            If ApplicaTrasformazione Then
                trasforma(parametriTrasformazione, Center_XCoord, Center_YCoord, p1x, p1y, appoggio)
                trasforma(parametriTrasformazione, Center_XCoord, Center_YCoord, p2x, p2y, appoggio)
                trasforma(parametriTrasformazione, Center_XCoord, Center_YCoord, p3x, p3y, appoggio)
                trasforma(parametriTrasformazione, Center_XCoord, Center_YCoord, p4x, p4y, appoggio)
            End If


            'POLYGON ((p1x p1y,p2x p2y, p3x p3y, p4x p4y))
            Return ("POLYGON ((" & p1x.ToString().Replace(",", ".") & " " & p1y.ToString().Replace(",", ".") & ", " & p2x.ToString().Replace(",", ".") & " " & p2y.ToString().Replace(",", ".") & ", " & p3x.ToString().Replace(",", ".") & " " & p3y.ToString().Replace(",", ".") & ", " & p4x.ToString().Replace(",", ".") & " " & p4y.ToString().Replace(",", ".") & "))")

        End Function

        Private Shared Sub aggiungiXY(ByVal px As Double, ByVal py As Double, ByRef listaXYZ As List(Of xyz))
            listaXYZ.Add(New xyz With {.X = px, .Y = py})
        End Sub
        Private Shared Sub trasforma(ByVal parametriTrasformazione As ParametriCoordinateConverter, ByVal Center_XCoord As Double, ByVal Center_YCoord As Double, ByRef px As Double, ByRef py As Double, ByVal appoggio As PointObject)
            appoggio.XCoord = px
            appoggio.YCoord = py
            appoggio = TransformED50ToWGSDecimal(appoggio, parametriTrasformazione)
            px = appoggio.XCoord + Center_XCoord
            py = appoggio.YCoord + Center_YCoord
        End Sub
        Public Shared Function getMinLatitudeWKTPolygon(sWKT As [String]) As PointObject
            Dim sep As [Char]() = New [Char](1) {}
            sep(0) = ","c
            sep(1) = " "c
            Dim AppData As [String]() = sWKT.Substring(10, sWKT.Length - 11).Split(sep)

            Dim minx As Double = 0, miny As Double = 500000000.0

            Dim sourceDecimalSeparator As String

            Try

                sourceDecimalSeparator = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator
            Catch e As Exception
                sourceDecimalSeparator = ","
            End Try

            Dim rval As New PointObject()
            Dim cur As New PointObject()

            For i As Integer = 0 To AppData.Length - 4 Step 3

                cur.XCoord = Double.Parse(AppData(i).Replace(".", sourceDecimalSeparator))
                cur.YCoord = Double.Parse(AppData(i + 1).Replace(".", sourceDecimalSeparator))
                If cur.YCoord < miny Then
                    minx = cur.XCoord
                    miny = cur.YCoord

                End If
            Next

            rval.XCoord = minx
            rval.YCoord = miny
            Return rval
        End Function

        Public Shared Function TransformWKTPolygonED50ToWGS84(sWKT As [String], ByVal ApplicaTrasformazionePoligono As Boolean, ByVal parametric As ParametriCoordinateConverter) As [String]

            Dim rVal As [String] = "POLYGON (("
            Dim sep As [Char]() = New [Char](1) {}
            sep(0) = ","c
            sep(1) = " "c
            Dim AppData As [String]() = sWKT.Substring(10, sWKT.Length - 11).Split(sep)

            Dim vertex As PointObject
            Dim vertexTarget As PointObject

            Dim sourceDecimalSeparator As String
            Dim finalDecimalSeparator As String = "."

            Try

                sourceDecimalSeparator = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator
            Catch e As Exception
                sourceDecimalSeparator = ","
            End Try

            For i As Integer = 0 To AppData.Length - 4 Step 3
                vertex = New PointObject()
                vertex.XCoord = Double.Parse(AppData(i).Replace(".", sourceDecimalSeparator))
                vertex.YCoord = Double.Parse(AppData(i + 1).Replace(".", sourceDecimalSeparator))
                If ApplicaTrasformazionePoligono Then
                    vertexTarget = TransformED50ToWGSDecimal(vertex, parametric)
                Else
                    vertexTarget = vertex
                End If
                rVal = rVal & vertexTarget.XCoord.ToString().Replace(",", finalDecimalSeparator) & " " & vertexTarget.YCoord.ToString().Replace(",", finalDecimalSeparator) & ", "
            Next

            Return rVal.Substring(0, rVal.Length - 3) & "))"

        End Function


        Private Shared Function TransformED50ToWGSDecimal([In] As PointObject, ByVal parametri As AgronicaConversioneCartografiaGias.Agronica.ParametriCoordinateConverter) As PointObject
            Dim wkt As New AgronicaConversioneCartografiaGias.FormatsConverter.WKT
            Dim lxyz As New List(Of xyz)
            lxyz.Add(New xyz With {.X = [In].XCoord, .Y = [In].YCoord})

            Dim convertitore As New AgronicaConversioneCartografiaGias.Agronica.CoordinateConverter
            Dim toPoint As String = convertitore.WKTPolygonWGS84_from_WKTPolygonED50(wkt.CreaPoligonoDaCoordinate(lxyz, True), False, parametri)


            Dim toPointXyz As xyz = wkt.CreaCoordinateDaPoligono(toPoint).FirstOrDefault


            Dim rVal As New PointObject()
            rVal.XCoord = toPointXyz.X
            rVal.YCoord = toPointXyz.Y
            rVal.Value = [In].Value

            Return rVal

        End Function
    End Class

End Namespace
