

Imports System.Xml

Public Class PianoConcimazioneVariabile


    ''' <summary>
    ''' Elabora e restituisce un FODM xml
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <param name="wktPoligono"></param>
    Public Shared Function Elabora(ByVal wktPoligono As String) As String
        'Dim ConsiglioMedioConcimazione As Double
        'Dim CPCV As CalcoloPianoConcimazioneVariabile.CalcoloPianoConcimazioneVariabile
        'Dim OutputFileGif, OutputFileXml As String

        'Dim DescrizioneSpecie As String

        ''Verifica dei parametri necessari x l'elaborazione
        'If VerificaParametriElaborazione() Then
        '    Try
        '        'Calcolo del consiglio medio di concimazione
        '        ConsiglioMedioConcimazione = 120 'CalcolaConsiglioMedioConcimazioneAzotata()

        '        'DEBUG
        '        '-----
        '        '-----
        '        'DEBUG

        '        'Calcolo del piano di concimazione variabile



        '        OutputFileGif = ""
        '        OutputFileXml = ""

        '        DescrizioneSpecie = "" 'MyUtility.DescrizioneDaElemento(pDocumento.Parametri(260808, 1).Valori(1).Valore)
        '        'CPCV = New CalcoloPianoConcimazioneVariabile.CalcoloPianoConcimazioneVariabile(ConsiglioMedioConcimazione, GetPoligonoAppezzamento(pDocumento), MS, TipoRaster.TipiRaster.Ortofoto, OutputFileGif, OutputFileXml, DescrizioneSpecie)
        '        '2012 nuova versione vanni

        '        CPCV = New CalcoloPianoConcimazioneVariabile.CalcoloPianoConcimazioneVariabile(ConsiglioMedioConcimazione, GetPoligonoAppezzamento(wktPoligono), OutputFileGif, OutputFileXml, DescrizioneSpecie)
        '        CPCV.Elabora()


        '    Catch ex As Exception
        '        'dettagli dell'errore

        '    End Try
        'End If

    End Function

    Private Shared Function GetPoligonoAppezzamento(ByRef wktPolygon As String) As SharpMap.Geometries.Polygon
        Dim PoligonoAppezzamento As SharpMap.Geometries.Polygon = _
            SharpMap.Geometries.Polygon.GeomFromText(wktPolygon)

        Return PoligonoAppezzamento
    End Function

    'Return: true ==> OK; false ==> Errori;
    Private Shared Function VerificaParametriElaborazione() As Boolean


        Return True
    End Function

End Class
