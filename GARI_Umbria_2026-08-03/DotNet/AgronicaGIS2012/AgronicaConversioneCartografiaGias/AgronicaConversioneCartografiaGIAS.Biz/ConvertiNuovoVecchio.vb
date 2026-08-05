Imports <xmlns="http://www.agronica.it/grafica/">


Imports AgronicaConversioneCartografiaGIAS.FormatsConverter
Imports AgronicaConversioneCartografiaGIAS.Agronica
Imports System.Configuration.ConfigurationManager
Imports AgronicaGIS2012.Commons
Imports AgronicaGIS2012.Commons.DataOraHelper
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports Gias2Gias_LIB

Imports System.Web

Imports System.Text

Public Class ConvertiNuovoVecchio


    ''' <summary>
    ''' Importazione della grafica di 1 centro aziendale.
    ''' </summary>
    ''' <param name="newXmlGrafica"></param>
    ''' <param name="pivasuperuser"></param>
    ''' <param name="PivaPadre"></param>
    ''' <param name="piva"></param>
    ''' <param name="sa_cod"></param>
    ''' <param name="objOpzioni"></param>
    ''' <param name="LayertipoEntita">Tipo di entità nel nuovo formato (22 = impianti, ecc..)</param>
    ''' <param name="swapLatLong">scambia i valori di lat e long (X,Y se ED50)</param>
    ''' <param name="ImportaLayerSpecieDaImpianto"></param>
    ''' <param name="GEORiferimento_COD"></param>
    ''' <returns></returns>
    ''' <remarks>un layer alla volta per ora.</remarks>
    Public Function ConvertiXml(ByVal newXmlGrafica As String, ByVal pivasuperuser As String, ByVal PivaPadre As String, piva As String, sa_cod As Integer, ByVal swapLatLong As Boolean, ByVal ImportaLayerSpecieDaImpianto As Boolean, ByVal GEORiferimento_COD As Integer, ByVal cconverter As CoordinateConverter, objparametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri, ByVal OldCondizioneWS As String) As String


        newXmlGrafica = newXmlGrafica.Replace("<DatiEntita>", "<DatiEntita xmlns=""http://www.agronica.it/grafica/"">")

        Dim xDocNew As XDocument = XDocument.Parse(newXmlGrafica)
        Dim xDocOld As XDocument = XDocument.Parse("<DatiEntita />")



        Dim ParametriCartografici As ParametriCoordinateConverter = Nothing
        Dim proiezione As String = "E" 'Proiezione UTM (E=ED50; G=Gauss-Boaga; W=WGS84)
        If GEORiferimento_COD <> -1 Then

            Dim leggiTrasformazione As New AgronicaCoreGisDAL.GIS_SistemiRiferimentoCartografia_R
            Dim dtLeggiTrasformazione As DataTable = leggiTrasformazione.Leggi(GEORiferimento_COD, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objparametri_server)

            ParametriCartografici = New ParametriCoordinateConverter With { _
                .CSFromText = dtLeggiTrasformazione(0)("CSFrom"), _
                .CStoText = dtLeggiTrasformazione(0)("CSTo"), _
                .CStoGeoText = dtLeggiTrasformazione(0)("CStoGeo"), _
                .AgronicaLatOffset = dtLeggiTrasformazione(0)("AgronicaLatOffset"), _
                .AgronicaLonOffset = dtLeggiTrasformazione(0)("AgronicaLonOffset"), _
                .LibreriaDaUsare = dtLeggiTrasformazione(0)("LibreriaDaUsare") _
            }

        Else
            proiezione = "W"
        End If

        Dim wktHelp As New WKT
        Dim gmlHelp As New GML

        Dim appezza As Integer
        Dim idReg As Integer
        Dim id_particella As Integer
        Dim id_entita_cod As Integer
        Dim programmazione_entita_cod As Integer

        Dim baseCode As Integer = GetBasecod(sa_cod)

        Dim hexChiave As String = ""

        Dim newTipologiaFiltro As String = "-1"
        Dim oldTiplogiaFiltro As String = OldCondizioneWS.Split("'")(1)(0).ToString
        Dim oldTiplogiaFiltroLayer As String = "-1"

        Select Case oldTiplogiaFiltro
            Case "A"
                newTipologiaFiltro = "1"

            Case "D"
                newTipologiaFiltro = "3"

            Case "I"
                newTipologiaFiltro = "19"
            Case "W"
                newTipologiaFiltro = "12"
            Case "O"
                newTipologiaFiltro = "8"
            Case "G"
                newTipologiaFiltro = "33"
        End Select

        For Each entita In _
            (From e In xDocNew.<DatiEntita>.<Entita>
            Where e.<layers>.<layer>.@tipologia_layer = "1" AndAlso
                  e.<layers>.<layer>.Value = newTipologiaFiltro).ToList



            Dim EntitaDaTrasformare As List(Of xyz) = GetEntitaDaTrasformare(entita, False)
            Dim ArrayEntitaDaTrasformare As xyz() = EntitaDaTrasformare.ToArray

            Dim wktFinal As String
            If ParametriCartografici Is Nothing Then
                wktFinal = wktHelp.CreaPoligonoDaCoordinate(EntitaDaTrasformare)
            Else
                wktFinal = cconverter.WKTPolygonWGS84_from_WKTPolygonED50(wktHelp.CreaPoligonoDaCoordinate(EntitaDaTrasformare), Not swapLatLong, ParametriCartografici)
            End If


            Dim FinalXYZ As List(Of xyz) = wktHelp.CreaCoordinateDaPoligono(wktFinal)



            resettachiavi(appezza, idReg, id_particella, id_entita_cod)
            resettachiaviHEx(hexChiave)

            Select Case newTipologiaFiltro

                Case "1"
                    'appezzamenti (scritti in formato A00000001, con base code incorporato
                    appezza = (From g In entita.<EntitaGIAS>.<DatoGias>.<Appezza>).Value
                    hexChiave = "A" & appezza.ToString("X").PadLeft(8, "0")


                Case "19" 'impianti 
                    '(scritti in formato I00010001, dove i primi 4 numeri sono appezza, gli altri 4 impianti, bisogna aggiungere il base cod)
                    appezza = (From g In entita.<EntitaGIAS>.<DatoGias>.<Appezza>).Value
                    idReg = (From g In entita.<EntitaGIAS>.<DatoGias>.<Id_Imp>).Value
                    appezza -= baseCode
                    idReg -= baseCode

                    hexChiave = "I" & appezza.ToString("X").PadLeft(4, "0") & idReg.ToString("X").PadLeft(4, "0")

                Case "3"
                    'Particelle catastali
                    id_particella = (From g In entita.<EntitaGIAS>.<DatoGias>.<part_cod>).Value
                    hexChiave = "D" & id_particella.ToString("X").PadLeft(8, "0")

                Case "33"
                    programmazione_entita_cod = (From g In entita.<EntitaGIAS>.<DatoGias>.<Programmazione_Entita_Cod>).Value
                    hexChiave = "G" & programmazione_entita_cod.ToString("X").PadLeft(8, "0")

                Case Else

                    ''  Vanni, 23/04/2014 16:09:25: associo al vecchio codice oppure al nuovo se si tratta di nuova entit
                    Dim idle As String = (From g In entita.<EntitaGIAS>.<DatoGias>.<OLDGrafica_ID>).Value
                    If Not String.IsNullOrEmpty(idle) Then
                        hexChiave = idle
                    Else
                        id_entita_cod = (From g In entita.<EntitaGIAS>.<DatoGias>.<Entita_Cod>).Value
                        hexChiave = oldTiplogiaFiltro & id_entita_cod.ToString("X").PadLeft(8, "0")
                    End If


            End Select

            Dim iNVertex As Integer = 1
            Dim iDescr As String
            Dim xCodice As Integer
            For Each xyz In FinalXYZ

                Dim x As String = xyz.X.ToString.Replace(".", ",")
                Dim y As String = xyz.Y.ToString.Replace(".", ",")
                Dim lat As String = ArrayEntitaDaTrasformare(iNVertex - 1).Y.ToString.Replace(".", ",")
                Dim lon As String = ArrayEntitaDaTrasformare(iNVertex - 1).X.ToString.Replace(".", ",")

                If iNVertex = 1 Then
                    Dim hexChiaveTestata As String
                    Select Case hexChiave(0).ToString
                        Case "I"
                            hexChiaveTestata = "F"
                        Case "D"
                            hexChiaveTestata = "Q"
                        Case "A"
                            hexChiaveTestata = "E"
                        Case "W"
                            hexChiaveTestata = "K"
                        Case "O"
                            hexChiaveTestata = "P"
                        Case "G"
                            hexChiaveTestata = "H"
                        Case Else
                            hexChiaveTestata = "F"
                    End Select

                    Dim lParteHex As String = hexChiave.Substring(1, 8)
                    hexChiaveTestata &= lParteHex

                    Dim value As Long = Long.Parse(lParteHex, System.Globalization.NumberStyles.HexNumber)
                    xCodice = value.ToString

                    oldTiplogiaFiltroLayer = CInt(newTipologiaFiltro).ToString("X")

                    Dim area As Double = AgronicaGIS2012.Commons.xyz.myCDBL((From g In entita.<geodata>.<DatiCalcolati>.<geodata_Area>).Value)
                    Dim perimetro As Double = AgronicaGIS2012.Commons.xyz.myCDBL((From g In entita.<geodata>.<DatiCalcolati>.<geodata_Perimetro>).Value)

                    Dim centro As String = (From g In entita.<geodata>.<DatiCalcolati>.<geodata_Baricentro>).Value
                    Dim xyzCentro As List(Of xyz) = GetEntitaPuntoDaTrasformare(centro)

                    Dim wktFinalCentro As String

                    If ParametriCartografici Is Nothing Then
                        wktFinalCentro = wktHelp.CreaPoligonoDaCoordinate(xyzCentro)
                    Else
                        wktFinalCentro = cconverter.WKTPolygonWGS84_from_WKTPolygonED50(wktHelp.CreaPoligonoDaCoordinate(xyzCentro), Not swapLatLong, ParametriCartografici)
                    End If


                    Dim finalxyzCentro As List(Of xyz) = GetEntitaPuntoDaTrasformare(wktFinalCentro)

                    '1981 Pesco - May grand 254,443 Ha
                    Dim text As String = entita.@text

                    Dim xTestoDescrittivo = <Entita TipoOperazioneDB="0" recno="" deleted="" section="E" id=<%= hexChiaveTestata %> prefisso=<%= hexChiave(0) %> codice=<%= xCodice %> descr="TEXT      22" ecolor="" layer=<%= oldTiplogiaFiltroLayer %> eline="" vx1=<%= finalxyzCentro.FirstOrDefault.Y.ToString.Replace(".", ",") %> vy1=<%= finalxyzCentro.FirstOrDefault.X.ToString.Replace(".", ",") %> vx2="" vy2="1" rad="15" text=<%= text %> gps="1" lat=<%= xyzCentro.FirstOrDefault.Y.ToString.Replace(".", ",") %> lon=<%= xyzCentro.FirstOrDefault.X.ToString.Replace(".", ",") %> pdop="0" fuso="0" proiezione=<%= proiezione %> quota="0" delta_nord="0" delta_est="0" validita_inizio="01/01/1900" validita_fine="31/12/2100"/>
                    xDocOld.Root.Add(xTestoDescrittivo)

                    Dim testata1 = <Entita TipoOperazioneDB="0" recno="" deleted="" section="E" id=<%= hexChiave %> prefisso=<%= hexChiave(0) %> codice=<%= xCodice %> descr="POLYGON   5" ecolor="" layer=<%= oldTiplogiaFiltroLayer %> eline="" vx1="" vy1="" vx2=<%= perimetro.ToString.Replace(".", ",") %> vy2=<%= area.ToString.Replace(".", ",") %> rad="" text="" gps="0" lat="0" lon="0" pdop="0" fuso="0" proiezione=<%= proiezione %> quota="0" delta_nord="0" delta_est="0" validita_inizio="01/01/1900" validita_fine="31/12/2100"/>
                    xDocOld.Root.Add(testata1)

                    'marco come esportato ed associo al nuovo poligono il vecchio ID
                    Dim Entita_cod As Integer = entita.<EntitaGIAS>.<DatoGias>.<Entita_Cod>.Value

                    Dim dp As New AgronicaCoreDataProvider.DataProvider
                    dp.EseguiQuery_Scrittura(objparametri_server, "UPDATE GIS_Entita SET OLDGrafica_ID = '" & hexChiave & "' where entita_cod = " & Entita_cod, "")




                End If

                iDescr = "VERTEX    " & iNVertex.ToString



                Dim nodoOLD = <Entita TipoOperazioneDB="0" recno="" deleted="" section="P" id=<%= hexChiave %> prefisso=<%= hexChiave(0) %> codice=<%= xCodice %> descr=<%= iDescr %> ecolor="" layer="" eline="" vx1=<%= x %> vy1=<%= y %> vx2="" vy2="" rad="" text="" gps="1" lat=<%= lat %> lon=<%= lon %> pdop="0" analisi_campione_cod="0" validita_inizio="01/01/1900" validita_fine="31/12/2100"/>
                xDocOld.Root.Add(nodoOLD)

                iNVertex += 1

            Next
        Next



        Dim ListaNS As New List(Of String)

        Return xmlHelper.RemoveNamespace(xDocOld, ListaNS).ToString.Replace("xmlns=""""", "")


    End Function

    Private Shared Function GetEntitaPuntoDaTrasformare(ByVal sElemento As String) As List(Of xyz)
        Dim rval As New List(Of xyz)
        Dim xyz As New xyz

        Dim app As String() = sElemento.Replace("POINT ", "").Replace("(", "").Replace(")", "").Split(" ")
        xyz.X = xyz.myCDBL(app(0))
        xyz.Y = xyz.myCDBL(app(1))

        rval.Add(xyz)

        Return rval
    End Function



    Private Shared Function GetEntitaDaTrasformare(ByVal sElemento As XElement, ByVal swapLatLong As Boolean) As List(Of xyz)

        Dim gmlHelp As New GML

        Dim EntitaDaTrasformare As New List(Of xyz)
        Dim appPunti As String() = GML.DammiArrayCoordinateDatoXml("http://www.opengis.net/gml", sElemento.<geodata>.FirstOrDefault)

        Dim xyz As xyz

        For i As Integer = 0 To appPunti.Length - 1 Step 2

            xyz = New xyz
            If swapLatLong Then
                xyz.X = xyz.myCDBL(appPunti(i))
                xyz.Y = xyz.myCDBL(appPunti(i + 1))

            Else
                xyz.Y = xyz.myCDBL(appPunti(i))
                xyz.X = xyz.myCDBL(appPunti(i + 1))

            End If

            EntitaDaTrasformare.Add(xyz)

        Next

        Return EntitaDaTrasformare


    End Function

    Private Sub resettachiavi(ByRef appezza As Integer, ByRef idReg As Integer, ByRef id_particella As Integer, ByRef id_entita_cod As Integer)
        appezza = 0
        idReg = 0
        id_particella = 0
        id_entita_cod = 0
    End Sub

    Private Sub resettachiaviHEx(ByRef hexChiave As String)
        hexChiave = ""

    End Sub

    Private Shared Function GetBasecod(ByVal Sa_cod As Integer) As Integer

        Return (Sa_cod \ (2 ^ 17)) * (2 ^ 17)

    End Function

End Class
