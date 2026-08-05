Imports System.Security.Policy
Imports System.Text
Imports AgronicaConversioneCartografiaGias.FormatsConverter
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDTOStd.Identity
Imports AgronicaCoreVarieBIZ
Imports AgronicaGIS2012.Commons
Imports Piano_Concimazione_Variabile

Public Class GoogleStaticMaps


    ''' <summary>
    ''' Restituisce un'immagine statica sotto forma di byte Array che rappresenta lo stream della mappa statica
    ''' </summary>
    ''' <param name="strWKT"></param>
    ''' <returns></returns>
    Public Function AggiornaElementoGraficoConMappaStatica(InData As GeneraMappaStaticaInData,
                                                           Optional ByRef bytesMappa As Byte() = Nothing) As RispostaStandard

        Dim rval As New RispostaStandard

        Try

            If InData.EntitaCod = 0 Then
                Throw New Exception("Static map: Missing EntitaCod")
            End If

            'leggi in base ad entita_Cod il WKT
            Dim letturaWKT As New AgronicaCoreGisDAL.GIS_ElementiGrafici_R
            Dim dtLetturaWKT As DataTable = letturaWKT.LeggiWKT(InData.EntitaCod, "", InData.objParametri_Server)

            If dtLetturaWKT.Rows.Count = 0 Then
                Throw New Exception("Static map: EntitaCod not found")
            End If

            InData.strWKT = dtLetturaWKT.Rows(0)("geo")

            GeneraMappaStaticaVerifyParam(InData)

            If Not InData.strWKT.Contains("POLYGON") Then
                Throw New Exception("Static map: Not a polygon")
            End If

            'Riporta il byte Array su DB
            Dim myBytes As Byte() = GeneraMappaStaticaComeByteArray(InData)

            If myBytes IsNot Nothing AndAlso myBytes.Length > 0 Then
                Dim aggiornaStaticMap As New AgronicaCoreGisDAL.GIS_ElementiGrafici_W
                aggiornaStaticMap.AggiornaStaticMap(InData.EntitaCod, myBytes, InData.objParametri_Server)
                If Not String.IsNullOrEmpty(InData.DebugImgPathPerDump) Then
                    'Verifico se esiste la DIRECTORY indicata ... altrimenti la creo
                    If System.IO.Directory.Exists(InData.DebugImgPathPerDump) = False Then
                        System.IO.Directory.CreateDirectory(InData.DebugImgPathPerDump)
                    End If
                    My.Computer.FileSystem.WriteAllBytes(AgronicaCoreUtility.FileSystemHelper.AggiungiSlashSeNonEsiste(InData.DebugImgPathPerDump) & AgronicaCoreUtility.FileSystemHelper.NomeFileUnivoco("png"), myBytes, True)
                End If
            End If

            bytesMappa = myBytes
            rval.RispostaOK = True
            rval.RispostaStringa = "Riporto Eseguito Correttamente"

        Catch ex As Exception

            rval.RispostaOK = False
            rval.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

            Dim objLog As New LogProvider
            objLog.Scrivi_LOG(InData.objParametri_Server, "GoogleStaticMaps\AggiornaElementoGraficoConMappaStatica()", ex.Message, CustomLOGParams:=New CustomLOGParams() With {.LogDirectory = InData.objParametri_Server.LogDirectory, .LogDescrizioneUtente = InData.objParametri_Server.LogDescrizioneUtente, .LogFileName = $"ReadByteArrayFromUrlGet_{Guid.NewGuid()}.txt"})

        End Try

        Return rval

    End Function


    ''' <summary>
    ''' Restituisce un'immagine statica sotto forma di byte Array che rappresenta lo stream della mappa statica
    ''' </summary>
    ''' <param name="strWKT"></param>
    ''' <returns></returns>
    Public Function GeneraMappaStatica(InData As GeneraMappaStaticaInData) As rispostaStandard(Of Byte())

        Dim rval As New rispostaStandard(Of Byte())

        Try

            GeneraMappaStaticaVerifyParam(InData)

            rval.RispostaOK = True
            rval.RispostaStringa = GeneraMappaStaticaComeByteArray(InData)

        Catch ex As Exception

            rval.RispostaOK = False
            rval.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return rval

    End Function

    Private Sub GeneraMappaStaticaSetDefault(InData As GeneraMappaStaticaInData)

        If InData.SizeX = 0 Then
            InData.SizeX = 400
        End If

        If InData.SizeY = 0 Then
            InData.SizeY = 400
        End If

        If String.IsNullOrEmpty(InData.Color) Then
            InData.Color = "green"
        End If

        If String.IsNullOrEmpty(InData.FillColor) Then
            InData.FillColor = "green"
        End If

    End Sub

    Private Sub GeneraMappaStaticaVerifyParam(InData As GeneraMappaStaticaInData)

        If InData.EntitaCod = 0 AndAlso String.IsNullOrEmpty(InData.strWKT) Then
            Throw New Exception("Static map: Missing WKT")
        End If

        If String.IsNullOrEmpty(InData.MapsApikey) Then
            Throw New Exception("Static map: Missing Gmaps api key")
        End If

    End Sub

    Private Function GeneraMappaStaticaComeByteArray(InData As GeneraMappaStaticaInData) As Byte()

        'Esempio
        'https://maps.googleapis.com/maps/api/staticmap?size=400x400&center=40.653279,-73.959816&zoom=11&path=fillcolor:0xAA000033%7Ccolor:0xFFFFFF00%7Cenc:___DATA___&key=YOUR_API_KEY

        Dim urlToGet As String = "https://maps.googleapis.com/maps/api/staticmap?size=" & InData.SizeX & "x" & InData.SizeY & "&maptype=" & InData.TipoMappaSfondo & "&path=fillcolor:" & InData.FillColor & "%7Ccolor:" & InData.Color & "%7Cenc:___DATA___&key=" & InData.MapsApikey
        Dim polylineEncoded As String = GeneraEncodedPolyline(InData.strWKT)

        urlToGet = urlToGet.Replace("___DATA___", polylineEncoded)

        If Not String.IsNullOrEmpty(InData.SignPrivateKey) Then
            Dim signedString As String = GoogleStaticMapsConverter.Sign(urlToGet, InData.SignPrivateKey)
            urlToGet = signedString
        End If

        Dim restSharpHelper As New AgronicaCoreUtility.Http
        Dim byteResponse As Byte() = restSharpHelper.ReadByteArrayFromUrlGet(urlToGet, InData.TimeoutRequestInSeconds, InData.objParametri_Server)

        Return byteResponse

    End Function

    Private Function strWKTListOfPoints(ByVal strWKT As String) As List(Of GoogleStaticMapsConverter.CoordinateEntity)

        Dim rval As New List(Of GoogleStaticMapsConverter.CoordinateEntity)
        Dim cc As New WKT
        Dim pp As List(Of xyz) = cc.CreaCoordinateDaPoligono(strWKT)
        For Each p In pp

            Dim newP As New GoogleStaticMapsConverter.CoordinateEntity
            newP.Latitude = p.Y
            newP.Longitude = p.X

            rval.Add(newP)

        Next

        Return rval
    End Function


    ''' <summary>
    ''' Genera un link per la chiamata ad una static API di google
    ''' </summary>
    ''' <param name="strWKT"></param>
    ''' <returns></returns>
    Private Function GeneraEncodedPolyline(ByVal strWKT As String) As String

        Dim pp As List(Of GoogleStaticMapsConverter.CoordinateEntity) =
            strWKTListOfPoints(strWKT)

        Dim rval As String =
            GoogleStaticMapsConverter.Encode(pp)

        Return rval
    End Function


End Class

Public Class GeneraMappaStaticaInData
    Public Property StaticMapAttive As Boolean
    Public Property EntitaCod As Integer
    Public Property strWKT As String
    Public Property MapsApikey As String
    Public Property SignPrivateKey As String
    ''' <summary>
    ''' In Formato ARGB, es: FF0000FF
    ''' </summary>
    ''' <returns></returns>
    Public Property FillColor As String
    ''' <summary>
    ''' In Formato ARGB, es: FF0000FF
    ''' </summary>
    ''' <returns></returns>
    Public Property Color As String
    Public Property TipoMappaSfondo As String
    Public Property SizeX As Integer
    Public Property SizeY As Integer

    Public Property objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    Public Property objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri

    Public Property DebugImgPathPerDump As String

    Public Property LayerAbilitati As List(Of Integer)

    Public Property TimeoutRequestInSeconds As Integer

    Public Property GeneraStaticMapDaSincroAPP As Boolean

End Class

