

Imports System.Text
Imports AgronicaConversioneCartografiaGias.Agronica
Imports AgronicaConversioneCartografiaGias.FormatsConverter
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreModelloInSviluppo
Imports AgronicaCoreVarieBIZ
Imports AgronicaGIS2012.Commons
Imports SharpMap.Layers

Public Class LetturaDatiElaboratiSensore

    Public Function LetturaDatiElaboratiSuSensore2(poligonoWKT As String, Zoom As Integer, sensore As Integer, DataInizio As String, DataFine As String) As String

        Dim letturaSlippy As New TilesHelper

        Return letturaSlippy.LatLong2tileSlippyFormat(poligonoWKT, Zoom)

    End Function

    ''' <summary>
    ''' Ri-proizione delle coordinate con lib agronica
    ''' </summary>
    ''' <param name="poligonoWKT"></param>
    ''' <param name="GEORiferimento_COD">da tabella GIS_SistemiRiferimentoCartografia</param>
    ''' <param name="objParametri_Server"></param>
    ''' <returns></returns>
    Public Function proiezionePoligono(ByVal poligonoWKT As String, ByVal GEORiferimento_COD As Integer, ByVal objParametri_Server As AgronicaCoreParametri) As String
        'trasformazione

        Dim leggiTrasformazione As New AgronicaCoreGisDAL.GIS_SistemiRiferimentoCartografia_R
        Dim dtLeggiTrasformazione As DataTable = leggiTrasformazione.Leggi(GEORiferimento_COD, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)


        Dim parametriRiproiezione As New ParametriCoordinateConverter With {
            .CSFromText = dtLeggiTrasformazione(0)("CSFrom"),
            .CStoText = dtLeggiTrasformazione(0)("CSTo"),
            .CStoGeoText = dtLeggiTrasformazione(0)("CStoGeo"),
            .AgronicaLatOffset = dtLeggiTrasformazione(0)("AgronicaLatOffset"),
            .AgronicaLonOffset = dtLeggiTrasformazione(0)("AgronicaLonOffset"),
            .LibreriaDaUsare = dtLeggiTrasformazione(0)("LibreriaDaUsare")
        }


        Dim cconverter As New CoordinateConverter


        Dim rval As String = ""
        If poligonoWKT.Contains("POLYGON") Then
            rval = cconverter.WKTPolygonWGS84_from_WKTPolygonED50(poligonoWKT, False, parametriRiproiezione, True)
        Else
            Dim vPoint As String() = poligonoWKT.Replace("POINT", "").Replace("(", "").Replace(")", "").Split(" ")
            cconverter.getPointObject(vPoint(0), vPoint(1), False, parametriRiproiezione, rval, vPoint, ",", ".")
            rval = "POINT (" & rval.Replace(",", "") & ")"
        End If


        Return rval

    End Function

    Public Function LetturaDatiElaboratiSuSensore(poligonoWKT As String, Zoom As Integer, sensore As String, DataInizio As String, DataFine As String, ByVal objParametri_server As AgronicaCoreParametri) As Gis_Sat_Sentinel_Overlay_list

    End Function

    Public Function RateoPianoVariabileSuPoligono(
        fileImmagineSatellitareBasePath As String,
        PoligonoWKT As String,
        cellsize As Integer,
        Sensore As String,
        DataRiferimento As String,
        ConsiglioMedioConcimazione As Double,
        ByVal objParametri_server As AgronicaCoreParametri
    ) As rispostaStandard(Of RateoPianoVariabileSuPoligono_Response)

        Dim listaOverlay As New Gis_Sat_Sentinel_Overlay_list
        Dim letturaOverlay As New AgronicaCoreGisBIZ.Sat_Sentinel
        Dim d1 As Date
        Dim d2 As Date
        d1 = DataRiferimento
        d2 = DataRiferimento
        d1 = d1.AddDays(-1)
        d2 = d2.AddDays(1)
        listaOverlay = letturaOverlay.customMapOverlayBaseInizializzaCalendario(PoligonoWKT, d1, d2, objParametri_server)


        Dim rval As rispostaStandard(Of RateoPianoVariabileSuPoligono_Response)

        If listaOverlay.ListaOverlayer.Count = 0 Then
            rval = New rispostaStandard(Of RateoPianoVariabileSuPoligono_Response)
            rval.RispostaOK = False
        End If


        Dim Passaggio As Gis_Sat_Sentinel_Overlay_Passaggio = listaOverlay.ListaOverlayer.FirstOrDefault.Passaggi.FirstOrDefault
        Dim cSensore As Gis_Sat_Sentinel_Overlay_Sensore = (From pSens In Passaggio.Sensore Where pSens.CodiceSensore = Sensore).FirstOrDefault
        Dim GEORiferimento_COD As Integer =
            Passaggio.Tile.GEORiferimento_COD


        Dim GEORiferimento_Cod_Inverso As Integer = 1

        PoligonoWKT = proiezionePoligono(PoligonoWKT, GEORiferimento_COD, objParametri_server)


        Dim CPCV As CalcoloPianoConcimazioneVariabile.CalcoloPianoConcimazioneVariabile

        Dim fileImmagineSatellitare As String = fileImmagineSatellitareBasePath & Passaggio.url & "\" & Sensore & ".tif"


        If My.Computer.FileSystem.FileExists(fileImmagineSatellitare) Then


            Dim OutputFileGif, OutputFileXml As String
            OttieneFileTemporaneiGifXml(OutputFileGif, OutputFileXml)
            Dim DescrizioneSpecie As String

            CPCV = New CalcoloPianoConcimazioneVariabile.CalcoloPianoConcimazioneVariabile(ConsiglioMedioConcimazione, fileImmagineSatellitare, GetPoligonoAppezzamento(PoligonoWKT), OutputFileGif, OutputFileXml, DescrizioneSpecie)
            CPCV.Elabora(cellsize)

            rval = New rispostaStandard(Of RateoPianoVariabileSuPoligono_Response)
            rval.RispostaOK = True
            rval.RispostaStringa = New RateoPianoVariabileSuPoligono_Response With {.EsriiAscii = CPCV.EsriiAcii, .GEORiferimento_COD = GEORiferimento_Cod_Inverso}

        End If

        If rval Is Nothing Then
            rval = New rispostaStandard(Of RateoPianoVariabileSuPoligono_Response)
            rval.RispostaOK = False
        End If

        Return rval

    End Function

    Public Function LetturaDatiElaboratiSuSensoreListaValori(fileImmagineSatellitareBasePath As String, poligonoWKT As String, Zoom As Integer, sensore As String, DataInizio As String, DataFine As String, ByVal objParametri_server As AgronicaCoreParametri) As List(Of Gis_Sat_Sentinel_Overlay_Sensore_DatoRilevato)



        'lettura dei file compresi nel'intervallo ... 

        'Dim fileImmagineSatellitareBasePath As String = "C:\Dati\GIS\Sentinel2\"
        Dim fileImmagineSatellitare As String


        Dim listaOverlay As New Gis_Sat_Sentinel_Overlay_list
        Dim letturaOverlay As New AgronicaCoreGisBIZ.Sat_Sentinel
        listaOverlay = letturaOverlay.customMapOverlayBaseInizializzaCalendario(poligonoWKT, DataInizio, DataFine, objParametri_server)

        Dim GEORiferimento_COD As Integer =
            listaOverlay.ListaOverlayer.FirstOrDefault.Passaggi.FirstOrDefault.Tile.GEORiferimento_COD


        poligonoWKT = proiezionePoligono(poligonoWKT, GEORiferimento_COD, objParametri_server)

        Dim ConsiglioMedioConcimazione As Double
        Dim CPCV As CalcoloPianoConcimazioneVariabile.CalcoloPianoConcimazioneVariabile
        Dim OutputFileGif, OutputFileXml As String

        Dim DescrizioneSpecie As String

        'Calcolo del consiglio medio di concimazione
        ConsiglioMedioConcimazione = 120 'CalcolaConsiglioMedioConcimazioneAzotata()

        'DEBUG
        '-----
        '-----
        'DEBUG

        'Calcolo del piano di concimazione variabile

        Dim xletturaconfig As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        OttieneFileTemporaneiGifXml(OutputFileGif, OutputFileXml)

        DescrizioneSpecie = "" 'MyUtility.DescrizioneDaElemento(pDocumento.Parametri(260808, 1).Valori(1).Valore)
        'CPCV = New CalcoloPianoConcimazioneVariabile.CalcoloPianoConcimazioneVariabile(ConsiglioMedioConcimazione, GetPoligonoAppezzamento(pDocumento), MS, TipoRaster.TipiRaster.Ortofoto, OutputFileGif, OutputFileXml, DescrizioneSpecie)
        '2012 nuova versione vanni

        Dim rval As New List(Of Gis_Sat_Sentinel_Overlay_Sensore_DatoRilevato)

        For Each overLay As Gis_Sat_Sentinel_Overlay In listaOverlay.ListaOverlayer

            Dim dataRilievo As Date = overLay.DataRiferimento

            For Each passaggio In overLay.Passaggi

                Dim cSensore As Gis_Sat_Sentinel_Overlay_Sensore = (From pSens In passaggio.Sensore Where pSens.CodiceSensore = sensore).FirstOrDefault

                If Not cSensore Is Nothing Then

                    fileImmagineSatellitare = fileImmagineSatellitareBasePath & passaggio.url & "\" & sensore & ".tif"

                    If My.Computer.FileSystem.FileExists(fileImmagineSatellitare) Then

                        CPCV = New CalcoloPianoConcimazioneVariabile.CalcoloPianoConcimazioneVariabile(ConsiglioMedioConcimazione, fileImmagineSatellitare, GetPunto(poligonoWKT), OutputFileGif, OutputFileXml, DescrizioneSpecie)

                        Dim dato As Double =
                        CPCV.LetturaDatiElaborati()

                        cSensore.DatiRilevati = New List(Of Gis_Sat_Sentinel_Overlay_Sensore_DatoRilevato)

                        rval.Add(New Gis_Sat_Sentinel_Overlay_Sensore_DatoRilevato With {.DataRiferimento = dataRilievo, .Valore = dato})

                    End If


                End If


            Next
            'passaggio



        Next

        Return rval

    End Function

    Private Shared Sub OttieneFileTemporaneiGifXml(ByRef OutputFileGif As String, ByRef OutputFileXml As String)
        Dim pFileTemporanei As String
        'Try
        '    pFileTemporanei = xletturaconfig.Leggi_Valore(0, "GestioneEsportazioni_Repository", "", "", objParametri_server)
        'Catch ex As Exception

        'End Try


        If pFileTemporanei = "" Then
            pFileTemporanei = "C:\GIASLAN\File_Esportazioni\"
        End If

        pFileTemporanei &= "EsportazionePF\RateoVariabileDebug\"

        Dim fileName As String
        fileName = My.Computer.FileSystem.GetTempFileName()
        Dim vFileNAme As String() = fileName.Split("\")

        OutputFileGif = pFileTemporanei & vFileNAme(vFileNAme.Length - 1).Replace(".tmp", ".gif")
        OutputFileXml = pFileTemporanei & vFileNAme(vFileNAme.Length - 1).Replace(".tmp", ".xml")
    End Sub

    Private Shared Function GetPoligonoAppezzamento(ByRef wktPolygon As String) As SharpMap.Geometries.Polygon
        Dim PoligonoAppezzamento As SharpMap.Geometries.Polygon =
            SharpMap.Geometries.Polygon.GeomFromText(wktPolygon)

        Return PoligonoAppezzamento
    End Function


    Private Shared Function GetPunto(ByRef wktPolygon As String) As SharpMap.Geometries.Point
        Dim punto As SharpMap.Geometries.Point =
            SharpMap.Geometries.Polygon.GeomFromText(wktPolygon)

        Return punto
    End Function

    Public Function LetturaProiezioniDatoPoligono(poligonoWKT As String, objParametri_server As AgronicaCoreParametri) As rispostaStandard(Of LetturaProiezioniDatoPoligono_Response)

        Dim rval As New rispostaStandard(Of LetturaProiezioniDatoPoligono_Response)
        Try
            Dim letturaOverlay As New AgronicaCoreGisDAL.Gis_Sat_Sentinel_Overlay_Tile_R
            Dim dtTile As DataTable =
                letturaOverlay.LeggiDatiTile("", poligonoWKT, "", "", objParametri_server)

            If dtTile.Rows.Count > 0 Then
                rval.RispostaStringa = New LetturaProiezioniDatoPoligono_Response With {
                    .GEORiferimento_COD = dtTile.Rows(0)("GEORiferimento_COD"),
                    .GEORiferimento_COD_2 = dtTile.Rows(0)("GEORiferimento_COD_2")
                }
            Else
                rval.RispostaOK = False
                rval.Errore = "Non è stato possible ottenere una proiezione dal poligono"
            End If


            '' VAnni: 25/10/2022: RATEO VARIABILE: versione alternativa, dal momento che ora su Agrosat non funziona la lettura dei sistemi di riferimento ... 
            'rval.RispostaOK = True
            'rval.RispostaStringa = New LetturaProiezioniDatoPoligono_Response With {
            '    .GEORiferimento_COD = 13,
            '    .GEORiferimento_COD_2 = 19
            '}


        Catch ex As Exception

            rval.RispostaOK = False
            rval.RispostaStringa = New LetturaProiezioniDatoPoligono_Response
            rval.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return rval

    End Function
End Class
