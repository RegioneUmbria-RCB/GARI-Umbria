
Imports <xmlns="http://www.agronica.it/grafica/">

Imports System.Configuration.ConfigurationManager
Imports AgronicaGIS2012.Commons
Imports AgronicaConversioneCartografiaGias.FormatsConverter
Imports AgronicaConversioneCartografiaGias.Agronica

Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Imports AgronicaSHPWrapper.InterpretaDatiDBF
Imports AgronicaSHPWrapper.InterpretaDatiDBF.InterpretaDatiDBF

Imports AgronicaCoreDataProvider
Imports System.IO
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreScadenziario


Public Class ShapeFileToAgronicaGis2012
    Inherits LogProvider
    Implements xxx_toAgronicaGIS2012

    Private _objParametri_Server As AgronicaCoreParametri
    Private _objParametri_Utenti As AgronicaCoreParametri
    'Private ReadOnly head As String = "<DatiEntita xmlns=""http://www.agronica.it/grafica/"" xmlns:gml=""http://www.opengis.net/gml"">"
    'Private ReadOnly tail As String = "</DatiEntita>"

    Public Const AGRODATAINIZIO As Date = #1/1/1900#
    Public Const AGRODATAFINE As Date = #12/31/2100#
    Private ReadOnly _LayersImprese As New List(Of String)

    Private _loadedShapefile As ShapeFile_ESRI.ShapeFile
    Public Property LoadedShapefile() As ShapeFile_ESRI.ShapeFile
        Get
            Return _loadedShapefile
        End Get
        Set(value As ShapeFile_ESRI.ShapeFile)
            _loadedShapefile = value
        End Set
    End Property

    Private _shapeFile_fullFileName As String
    Public Property ShapeFile_fullFileName() As String
        Get
            Return _shapeFile_fullFileName
        End Get
        Set(value As String)
            _shapeFile_fullFileName = value
        End Set
    End Property

    'Private ReadOnly _parametriGIAS As AgronicaCoreDataProvider.AgronicaCoreParametri

    Private Function getCau_MovDatoLav_cod(ByVal lav_cod As Integer) As Integer
        Dim cau_mov As Integer
        Select Case lav_cod
            Case 18
                cau_mov = CAU_TRATTAMENTO
            Case 74
                cau_mov = CAU_TRATTAMENTO
            Case Else
                cau_mov = CAU_LAVORAZIONE
        End Select

        Return cau_mov
    End Function


    Private Function LeggiImpostazioneSalvataggioFilesAllegati(objParametri_Utenti As AgronicaCoreParametri) As Boolean

        Dim xLetturaImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim dt As DataTable =
            xLetturaImpostazioni.Leggi(
                enum_Impostazioni_Utenti.SUPERUSER_DOCUMENTALE_SALVA_ALLEGATO_SU_DB, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

        Dim rval As Boolean
        If dt.Rows.Count > 0 Then
            rval = dt.Rows(0)("Impostazione_Valore_1")
        Else
            rval = True
        End If

        Return rval

    End Function

    Public Sub convertAllegatiDocumentiSaveZipFolder(
            configurazioneImportazione As ConfigurazioneImportazione,
            zipStream As Byte(),
            objParametri_Server As AgronicaCoreParametri
    )

        Dim objWebConfig As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim GestioneTmpRepository As String = objWebConfig.Leggi_Valore(0, "PathFileTemporanei", "", "", objParametri_Server)

        Dim nomefile_zip As String = ""
        Dim nomeFileZiptoPass As String = ""
        Dim tmpPath As String

        If Not IsNothing(zipStream) Then

            tmpPath = AgronicaCoreUtility.FileSystemHelper.AggiungiSlashSeNonEsiste(GestioneTmpRepository)
            configurazioneImportazione.ShapeFileFullFileName = tmpPath
            nomefile_zip = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) & ".zip"


            nomeFileZiptoPass = tmpPath & nomefile_zip
            My.Computer.FileSystem.WriteAllBytes(nomeFileZiptoPass, zipStream, False)
        Else
            nomeFileZiptoPass = configurazioneImportazione.ShapeFileFullFileName
            configurazioneImportazione.ShapeFileFullFileName = configurazioneImportazione.ShapeFileFullFileName.Replace(".zip", ".shp")
        End If

        Dim folder As String = nomeFileZiptoPass.Replace(".zip", "")
        AgronicaCoreUtility.AgroZip.UnZip(nomeFileZiptoPass, folder)

        'controllo che esista il file shp nella folder, altrimenti ricerco ricorsivamente nelle sotto cartelle

        Dim AllSHPFiles = Directory.GetFiles(folder, "*.shp", SearchOption.AllDirectories)
        If AllSHPFiles.Length > 0 Then
            configurazioneImportazione.ShapeFileFullFileName = AllSHPFiles(0)
        End If

    End Sub

    ''' <summary>
    ''' A partire da un allegato in tabella "allegati_Documenti" (file Zip che contiene uno shapeFile) viene processato il contenuto e riportato in un allegato in formato AgronicaGIS2012 (xml)
    ''' </summary>
    ''' <param name="Allegati_Documenti_Cod">Impostare il parametro Allegati_Documenti_Cod</param>
    ''' <param name="objParametriServer"></param>
    ''' <param name="objParametriUtenti"></param>
    ''' <returns></returns>
    Public Function convertAllegatiDocumenti(
            Allegati_Documenti_Cod As Integer,
            ByVal objParametriServer As AgronicaCoreParametri,
            ByVal objParametriUtenti As AgronicaCoreParametri
    ) As RispostaStandard


        Dim rval As New RispostaStandard
        Try

            If Allegati_Documenti_Cod = 0 Then
                Throw New Exception("Impostare il parametro  configurazioneImportazione.Allegati_Documenti_Cod")
            End If

            'Completamento dei dati partendo dall'allegato
            Dim leggiFileAllegatoDB As Boolean =
                LeggiImpostazioneSalvataggioFilesAllegati(objParametriUtenti)

            Dim leggiEntita As New Alert_Entita_R
            Dim dtEntita As DataTable =
                leggiEntita.Leggi_con_documenti(leggiFileAllegatoDB, Allegati_Documenti_Cod, 0, "", "", objParametriServer)

            If dtEntita.Rows.Count = 0 Then
                Throw New Exception("Nessun dato letto per  configurazioneImportazione.Allegati_Documenti_Cod = " & Allegati_Documenti_Cod)
            End If

            Dim ConfigurazioneImportazione As New ConfigurazioneImportazione(
                shapeFile_fullFileName:="",
                objParametriServer.PivaSuperUser,
                piva:=dtEntita.Rows(0)("piva"),
                sa_cod:=dtEntita.Rows(0)("sa_Cod"),
                campo_cod:=dtEntita.Rows(0)("campo_cod"),
                appezza:=dtEntita.Rows(0)("appezza"),
                reg_impianto:=dtEntita.Rows(0)("id_imp"),
                programmazione_cod:=0,
                lav_cod:=0,
                lav_des:=0,
                cod_risum:=0,
                cod_mac:=0,
                Layer_cod:=0,
                tipo_importazione_agronica:=2,
                anno:=2011,
                ASG_Utente_Username:="",
                ASG_Utente_Password:="",
                ProgressivoGias:=0,
                trasformaSistemaRiferimento:=False,
                0,
                lTipoEntita_Cod:=55,
                Codice_Fiscale_Tecnico:="",
                GestioneRiportoDatiInGias:=Tipo_Importazione.Tipo_GestioneRiportoDatiInGias.RiportoSuAllegati
            )

            ConfigurazioneImportazione.CategoriaDocumento = enum_CategorieDocumenti.PrecisionFarming_MappaProduzione
            ConfigurazioneImportazione.Ricetta_Operazione_Cod = dtEntita.Rows(0)("Ricetta_Operazione_Cod")
            ConfigurazioneImportazione.LayerCod = enum_Gis_LayerElementiGrafici_std.Dettagli_Precision_Farming

            'se cfg per leggere da db, estrazione dello Zip Su Cartella temporanea (o leggendo da flusso di byte), altrimenti leggo il file da file system.
            If leggiFileAllegatoDB Then
                Dim zipStream As Byte() = dtEntita.Rows(0)("File_Allegato_DB")
                convertAllegatiDocumentiSaveZipFolder(ConfigurazioneImportazione, zipStream, objParametriServer)
            Else
                ConfigurazioneImportazione.ShapeFileFullFileName = dtEntita.Rows(0)("Allegati_Documenti_NomeFile")
                convertAllegatiDocumentiSaveZipFolder(ConfigurazioneImportazione, Nothing, objParametriServer)
            End If

            'genero un nuovo allegato del tipo indicato partendo dallo shapefile, che contiene i dati nella colonna allegatiDocumentiXML
            ConfigurazioneImportazione.Allegati_Documenti_Cod_collegamento = Allegati_Documenti_Cod
            convert(ConfigurazioneImportazione, objParametriServer, objParametriUtenti)
            rval.RispostaOK = True
            rval.RispostaStringa = "Operazione eseguita"

        Catch ex As Exception
            rval.RispostaOK = False
            rval.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return rval

    End Function


    Private Function Shape2XDocument(shape As ShapeFile_ESRI.Shape, cfg As ConfigurazioneImportazione, par_cc As ParametriCoordinateConverter, el_grafico_cod As Integer, ByRef ObjParametri_Server As AgronicaCoreParametri) As XDocument

        Dim lxyz As New List(Of xyz)
        Dim hxyz As Dictionary(Of Integer, List(Of xyz)) = Nothing
        Dim tipoWKT As String = "POLYGON"

        Select Case shape.Type


            Case ShapeFile_ESRI.ShapeType.Point

                Dim point As ShapeFile_ESRI.ShapePoint = DirectCast(shape, ShapeFile_ESRI.ShapePoint)

                lxyz.Add(New xyz With {.X = point.Point.X, .Y = point.Point.Y})

                tipoWKT = "POINT"


            Case ShapeFile_ESRI.ShapeType.PointZ

                Dim point As ShapeFile_ESRI.ShapePointZ = DirectCast(shape, ShapeFile_ESRI.ShapePointZ)

                lxyz.Add(New xyz With {.X = point.Point.X, .Y = point.Point.Y})

                tipoWKT = "POINT"


            Case ShapeFile_ESRI.ShapeType.Polygon

                Dim poly As ShapeFile_ESRI.ShapePolygon = DirectCast(shape, ShapeFile_ESRI.ShapePolygon)

                If poly.Parts.Count > 0 Then

                    For Each p In poly.Parts(0)
                        lxyz.Add(New xyz With {.X = p.X, .Y = p.Y})
                    Next

                    If poly.Parts.Count > 1 Then
                        hxyz = New Dictionary(Of Integer, List(Of xyz))

                        For i As Integer = 1 To poly.Parts.Count - 1
                            hxyz.Add(i, New List(Of xyz))
                            For Each p In poly.Parts(i)
                                hxyz(i).Add(New xyz With {.X = p.X, .Y = p.Y})
                            Next
                        Next
                    End If

                End If


            Case ShapeFile_ESRI.ShapeType.PolygonZ

                Dim poly As ShapeFile_ESRI.ShapePolygonZ = DirectCast(shape, ShapeFile_ESRI.ShapePolygonZ)

                If poly.Parts.Count > 0 Then

                    For Each p In poly.Parts(0)
                        lxyz.Add(New xyz With {.X = p.X, .Y = p.Y})
                    Next

                    If poly.Parts.Count > 1 Then
                        hxyz = New Dictionary(Of Integer, List(Of xyz))

                        For i As Integer = 1 To poly.Parts.Count - 1
                            hxyz.Add(i, New List(Of xyz))
                            For Each p In poly.Parts(i)
                                hxyz(i).Add(New xyz With {.X = p.X, .Y = p.Y})
                            Next
                        Next
                    End If

                End If


            Case ShapeFile_ESRI.ShapeType.MultiPoint

                Dim mpoint = DirectCast(shape, ShapeFile_ESRI.ShapeMultiPoint)

                For Each p In mpoint.Points
                    lxyz.Add(New xyz With {.X = p.X, .Y = p.Y})
                Next

                tipoWKT = "MULTIPOINT"


            Case ShapeFile_ESRI.ShapeType.PolyLine

                Dim pline = DirectCast(shape, ShapeFile_ESRI.ShapePolyLine)

                If pline.Parts.Count > 0 Then

                    For Each p In pline.Parts(0)
                        lxyz.Add(New xyz With {.X = p.X, .Y = p.Y})
                    Next

                End If

                tipoWKT = "LINESTRING"


            Case ShapeFile_ESRI.ShapeType.PolyLineZ

                Dim pline = DirectCast(shape, ShapeFile_ESRI.ShapePolyLineZ)

                If pline.Parts.Count > 0 Then

                    For Each p In pline.Parts(0)
                        lxyz.Add(New xyz With {.X = p.X, .Y = p.Y})
                    Next

                End If

                tipoWKT = "LINESTRING"



        End Select

        'se lxyz e vuoto???

        Dim wktHelp As New WKT
        Dim wktToGeoML As New wkt_gml With {
            .Soglia_ConsideraPuntiUguali = 0.000000005
        }
        Dim cconverter As New CoordinateConverter

        Dim sNodeDoc As XDocument

        If cfg.TrasformaSistemaRiferimento Then

            For Each p In lxyz

                Dim op = cconverter.convertPoint(p.X, p.Y, True, par_cc)

                p.X = op.XCoord
                p.Y = op.YCoord
            Next

            'Lavez - 7/10/2025 - correzione trasformazione coordinate per buchi
            If hxyz IsNot Nothing Then
                For Each h In hxyz
                    For Each p In h.Value
                        Dim op = cconverter.convertPoint(p.X, p.Y, True, par_cc)

                        p.X = op.XCoord
                        p.Y = op.YCoord
                    Next
                Next
            End If
        Else

            ' VAnni: 9/3/2023: imposto lo scambio di lat lng su elementi di tipo punto
            If shape.Type = ShapeFile_ESRI.ShapeType.Point Or shape.Type = ShapeFile_ESRI.ShapeType.PointZ Then
                For Each p In lxyz
                    Dim p1 As Double
                    p1 = p.Y
                    p.Y = p.X
                    p.X = p1
                Next
            End If

        End If

        'Dim swapCoord As Boolean = (shape.Type = ShapeFile_ESRI.ShapeType.Point OrElse shape.Type = ShapeFile_ESRI.ShapeType.PointZ)

        Dim wkt = wktHelp.CreaWKT(lxyz, tipoWKT, hxyz)

        Dim xTest As New AgronicaCoreGisDAL.GIS_ElementiGrafici_W

        If xTest.TestaPoligonoWKTValid(wkt, ObjParametri_Server, False) = False Then
            Scrivi_LOG(ObjParametri_Server, "ShapeFileToAgronicaGis2012.Shape2XDocument", "wkt non valido tento inversione punti" & vbCrLf & wkt, False)
            Dim newWKTString = PolygonOrder.InvertiPoligono_wkt(wkt, False, False)
            If xTest.TestaPoligonoWKTValid(newWKTString, ObjParametri_Server, False) = True Then
                wkt = newWKTString
            Else
                Scrivi_LOG(ObjParametri_Server, "ShapeFileToAgronicaGis2012.Shape2XDocument", "wkt invertito non valido lascio dato originale" & vbCrLf & newWKTString, False)
            End If
        End If


        If cfg.TrasformaSistemaRiferimento Then

            sNodeDoc = XDocument.Parse(wktToGeoML.Trasforma(wkt, False, True, True, el_grafico_cod))
        Else
            ' VAnni: 9/3/2023: imposto lo scambio di lat lng se non viene effettuata una trasformazione del sistema di riferimento
            sNodeDoc = XDocument.Parse(wktToGeoML.Trasforma(wkt, True, False, False, el_grafico_cod))
        End If

        Return sNodeDoc
    End Function


    Private Class ShapeCatasto

        Private ColNameCodBelfiore As String
        Private ColNameIstatP As String
        Private ColNameIstatC As String
        Private ColNameIdSezC As String
        Private ColNameFoglio As String
        Private ColNameParticella As String
        Private ColNameSub As String

        Private LetturaBelfiore As DecodeComuniIstatCodBelfiore_controller

        Private _IstatP As String
        Private _IstatC As String
        Private _Sezione As String
        Private _Foglio As String
        Private _Particella As String
        Private _Subalterno As String

        Public Sub New(cfg As ConfigurazioneImportazione, objParametriServer As AgronicaCoreParametri)

            ColNameCodBelfiore = "Cod_Belfiore"
            ColNameIstatP = "istatp"
            ColNameIstatC = "istatc"
            ColNameIdSezC = "id_sezc"
            ColNameFoglio = "foglio"
            ColNameParticella = "particella"
            ColNameSub = "sub"

            If cfg IsNot Nothing AndAlso cfg.ConfigurazioneImportazione_Catasto IsNot Nothing Then

                ColNameCodBelfiore = If(cfg.ConfigurazioneImportazione_Catasto.CodBelfiore, "")
                ColNameIstatP = If(cfg.ConfigurazioneImportazione_Catasto.Prov, "")
                ColNameIstatC = If(cfg.ConfigurazioneImportazione_Catasto.Com, "")
                ColNameIdSezC = If(cfg.ConfigurazioneImportazione_Catasto.Sezione, "")
                ColNameFoglio = If(cfg.ConfigurazioneImportazione_Catasto.Foglio, "")
                ColNameParticella = If(cfg.ConfigurazioneImportazione_Catasto.Particella, "")
                ColNameSub = If(cfg.ConfigurazioneImportazione_Catasto.Subalterno, "")
            End If

            LetturaBelfiore = New DecodeComuniIstatCodBelfiore_controller(objParametriServer)
        End Sub

        Private Function MetadataOrDefault(shape As ShapeFile_ESRI.Shape, colname As String, _default As String) As String

            Dim result As String = shape.GetMetadata(colname)

            If String.IsNullOrEmpty(result) Then
                result = _default
            End If
            Return result
        End Function

        Public Sub readFromShapeMetadata(shape As ShapeFile_ESRI.Shape)

            Dim CodBelfiore As String = MetadataOrDefault(shape, ColNameCodBelfiore, "")
            _IstatP = MetadataOrDefault(shape, ColNameIstatP, "0")
            _IstatC = MetadataOrDefault(shape, ColNameIstatC, "0")
            _Sezione = MetadataOrDefault(shape, ColNameIdSezC, "0")
            _Foglio = MetadataOrDefault(shape, ColNameFoglio, "-1")
            _Particella = MetadataOrDefault(shape, ColNameParticella, "-1")
            _Subalterno = MetadataOrDefault(shape, ColNameSub, "0")

            ' VAnni: 29/7/2019: se esiste un foglio catastale allora sto importando il catasto. Leggo codifica prov com da codice belfiore
            If _Foglio <> "-1" AndAlso CodBelfiore <> "" AndAlso _IstatP = "0" AndAlso _IstatC = "0" Then
                LetturaBelfiore.LeggiDecodeBelfioreDaDB(CodBelfiore, _IstatP, _IstatC)
            End If
        End Sub

        Public ReadOnly Property IstatP As String
            Get
                Return _IstatP
            End Get
        End Property
        Public ReadOnly Property IstatC As String
            Get
                Return _IstatC
            End Get
        End Property
        Public ReadOnly Property Sezione As String
            Get
                Return _Sezione
            End Get
        End Property
        Public ReadOnly Property Foglio As String
            Get
                Return _Foglio
            End Get
        End Property
        Public ReadOnly Property Particella As String
            Get
                Return _Particella
            End Get
        End Property
        Public ReadOnly Property Subalterno As String
            Get
                Return _Subalterno
            End Get
        End Property

        Public Sub initialize()
            _IstatP = "0"
            _IstatC = "0"
            _Sezione = "0"
            _Foglio = "-1"
            _Particella = "-1"
            _Subalterno = "0"
        End Sub

    End Class

    ''' <summary>
    ''' restituisce il file in formato agronica.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <param name="lTipoEntita_Cod"></param>
    ''' <param name="trasformaSistemaRiferimento"></param>
    Public Function convert(configurazioneImportazione As ConfigurazioneImportazione, ByVal objParametriServer As AgronicaCoreParametri, ByVal objParametriUtenti As AgronicaCoreParametri) As String Implements xxx_toAgronicaGIS2012.convert

        _objParametri_Server = objParametriServer
        _objParametri_Utenti = objParametriUtenti

        _shapeFile_fullFileName = configurazioneImportazione.ShapeFileFullFileName
        _loadedShapefile = New ShapeFile_ESRI.ShapeFile(_shapeFile_fullFileName)

        Dim xDocRval As XDocument = XDocument.Parse("<DatiEntita xmlns=""http://www.agronica.it/grafica/"" xmlns:gml=""http://www.opengis.net/gml""></DatiEntita>")
        Dim lay = <layersdescrizioni>
                      <layer tipologia_layer="1" nome_layer="entità">
                      </layer>
                  </layersdescrizioni>

        xDocRval.Root.Add(lay)

        Dim letturaCodBelfiore As New AgronicaCoreMetaSchemaDAL.ISTAT_Comuni_R

        'trasformazione
        Dim leggiTrasformazione As New AgronicaCoreGisDAL.GIS_SistemiRiferimentoCartografia_R
        Dim dtLeggiTrasformazione As DataTable = leggiTrasformazione.Leggi(configurazioneImportazione.GeoRiferimentoCod, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", _objParametri_Server)

        Dim ParametriCartografici As New ParametriCoordinateConverter With {
            .CSFromText = dtLeggiTrasformazione(0)("CSFrom"),
            .CStoText = dtLeggiTrasformazione(0)("CSTo"),
            .CStoGeoText = dtLeggiTrasformazione(0)("CStoGeo"),
            .AgronicaLatOffset = dtLeggiTrasformazione(0)("AgronicaLatOffset"),
            .AgronicaLonOffset = dtLeggiTrasformazione(0)("AgronicaLonOffset"),
            .LibreriaDaUsare = dtLeggiTrasformazione(0)("LibreriaDaUsare")
        }


        Dim stopME As Integer = -1
        If Debugger.IsAttached Then
            stopME = -1
        End If


        Dim iteration As Integer = 0

        Dim ListaDati As New List(Of String)

        Dim lProgrammazioneEntitaCod As Integer = 0
        Dim lProgrammazione_cod As Integer = 0
        Dim Ricetta_Operazione_Cod As Integer = 0
        Dim chiusuraTranansazione As Boolean
        Dim inviato As String
        Dim progressivoImportCatasto As Integer = 0

        If configurazioneImportazione.TipoImportazioneAgronica = Tipo_Importazione.Tipo_Importazione_ShapeFile.importaCatasto_DXF Then

            Dim AgroSequenze As New AgronicaCoreDataProvider.Agro_Sequenze

            'progressivoImportCatasto = AgroSequenze.Agronica_SequenzaTabelle_NuovoID("progressivoImportCatasto", _objParametri_Server)
            'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
            progressivoImportCatasto = AgroSequenze.NuovoId_Tabella("progressivoImportCatasto", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, _objParametri_Server)
        End If

        Dim shpCatasto As New ShapeCatasto(configurazioneImportazione, objParametriServer)

        Dim countShapes As Integer = _loadedShapefile.Count

        Scrivi_LOG(objParametriServer, "ShapeFileToAgronicaGis2012.convert", $"trovati { countShapes.ToString()} poligoni da importare", False)
        For Each shape In _loadedShapefile

            If shape.Type = ShapeFile_ESRI.ShapeType.Null Then
                Continue For
            End If

            Dim layer_codAtt As Integer
            If configurazioneImportazione.LayerCod = -1 Then
                layer_codAtt = iteration + 1
            Else
                layer_codAtt = configurazioneImportazione.LayerCod
            End If

            Dim text As String = ""

            Dim attributes As New List(Of String)
            Dim metadata = shape.GetMetadataNames()
            If metadata IsNot Nothing Then
                For Each md In metadata
                    attributes.Add(md & "§ " & shape.GetMetadata(md))
                Next

                If metadata.Count = 1 Then
                    'per evitare il bug in dbo.fGetKeyValue(...) nella query lettura
                    attributes.Add("DUMMY§ *")
                End If
            End If

            text = String.Join("|", attributes)

            If configurazioneImportazione.LayerCod = -1 OrElse iteration = 0 Then

                Dim newDescr = <valori codice=<%= layer_codAtt %>><%= text %></valori>
                xDocRval.<DatiEntita>.<layersdescrizioni>.<layer>.First.Add(newDescr)
            End If

            Dim vshapeFile_fullFileName = configurazioneImportazione.ShapeFileFullFileName.Split("\")

            Dim cau_mov As Integer = getCau_MovDatoLav_cod(configurazioneImportazione.LavCod)

            ListaDati.Add("NomeDir§ " & vshapeFile_fullFileName(vshapeFile_fullFileName.Length - 2) &
                          "|PivaSuperUser§ " & configurazioneImportazione.PivaSuperUSer &
                          "|Piva§ " & configurazioneImportazione.Piva &
                          "|sa_cod§ " & configurazioneImportazione.SaCod &
                          "|campo_cod§ " & configurazioneImportazione.CampoCod &
                          "|Appezza§ " & configurazioneImportazione.Appezza &
                          "|reg_impianto§ " & configurazioneImportazione.RegImpianto &
                          "|cod_risum§ " & configurazioneImportazione.CodRisum &
                          "|programmazione_cod§ " & configurazioneImportazione.ProgrammazioneCod &
                          "|cod_mac§ " & configurazioneImportazione.CodMac &
                          "|lav_cod§ " & configurazioneImportazione.LavCod &
                          "|lav_des§ " & configurazioneImportazione.LavDes &
                          "|cau_mov§ " & cau_mov &
                          "|anno§ " & configurazioneImportazione.Anno &
                          "|" & text)

            If configurazioneImportazione.TipoImportazioneAgronica = Tipo_Importazione.Tipo_Importazione_ShapeFile.importaCatasto_DXF Then
                shpCatasto.readFromShapeMetadata(shape)
            Else
                shpCatasto.initialize()
            End If

            Try

                Dim DatiArray As String() = ListaDati.ToArray
                ListaDati.Clear()

                chiusuraTranansazione = False
                Dim Aggregatore As New AgronicaCoreGisDAL.GIS_OperazioniCartograficheDB

                If configurazioneImportazione.TipoImportazioneAgronica = Tipo_Importazione.Tipo_Importazione_ShapeFile.Importazione_Trimble And Ricetta_Operazione_Cod = 0 And configurazioneImportazione.ShapeFileFullFileName.ToLower.EndsWith("coverage.shp") Then

                    AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, _objParametri_Server)

                    Dim helperGiasDati As New InterpretaDatiDBF.InterpretaDatiDBF
                    helperGiasDati.ImportaDatiDBF(DatiArray, {"|"c, "§"c}, configurazioneImportazione.TipoImportazioneAgronica, _objParametri_Server)

                    Ricetta_Operazione_Cod = GetValue(DatiArray(0).Split({"|"c, "§"c}), "Ricetta_Operazione_Cod")

                    chiusuraTranansazione = True
                End If

                If configurazioneImportazione.TipoImportazioneAgronica = Tipo_Importazione.Tipo_Importazione_ShapeFile.Importazione_Agrea_Crea_Planning Then

                    'Lavez - 05/11/2024 - gestione sequence inizializzazione se seq mancante
                    If AgronicaCoreDataProvider.Agro_Sequenze.CheckAllowAppSettingsFlagUseSequence() Then
                        If Not AgronicaCoreDataProvider.Agro_Sequenze.InizializzaSequence("Gis_Entita", 0, 2000000, _objParametri_Server) Then
                            Throw New Exception("Errore in inizializzazione sequence per Gis_Entita")
                        End If
                        If Not AgronicaCoreDataProvider.Agro_Sequenze.InizializzaSequence("Gis_ElementiGrafici", 0, 2000000, _objParametri_Server) Then
                            Throw New Exception("Errore in inizializzazione sequence per Gis_ElementiGrafici")
                        End If
                    End If

                    AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, _objParametri_Server)

                    Aggregatore.AggregaAppezzamentiContiguiComeParticelle(13, 13, 0, _objParametri_Server)

                    chiusuraTranansazione = True
                End If

                If configurazioneImportazione.TipoImportazioneAgronica = Tipo_Importazione.Tipo_Importazione_ShapeFile.Importazione_Agrea_Crea_Planning Then

                    AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, _objParametri_Server)

                    Dim helperGiasDati As New InterpretaDatiDBF.InterpretaDatiDBF
                    helperGiasDati.ImportaDatiDBF(DatiArray, {"|"c, "§"c}, configurazioneImportazione.TipoImportazioneAgronica, _objParametri_Server)

                    lProgrammazioneEntitaCod = GetValue(DatiArray(0).Split({"|"c, "§"c}), "ProgrammazioneEntita_cod")
                    lProgrammazione_cod = GetValue(DatiArray(0).Split({"|"c, "§"c}), "Programmazione_cod")

                    chiusuraTranansazione = True
                End If

                If chiusuraTranansazione Then

                    G2G_Chiusura_Transazione(1)
                    chiusuraTranansazione = False
                End If

            Catch ex As Exception

                G2G_Chiusura_Transazione(2)
                chiusuraTranansazione = False

            End Try

            Select Case configurazioneImportazione.TipoImportazioneAgronica

                Case Tipo_Importazione.Tipo_Importazione_ShapeFile.Importazione_Agrea_Crea_Planning
                    inviato = "-1"

                Case Tipo_Importazione.Tipo_Importazione_ShapeFile.importaCatasto_DXF
                    inviato = progressivoImportCatasto

                Case Else
                    inviato = "0"
            End Select

            If lProgrammazione_cod = 0 Then
                lProgrammazione_cod = configurazioneImportazione.ProgrammazioneCod
            End If

            Dim lElementoGrafico_cod As Integer = 0
            Dim lTipoOperazioneDB As String = "1"
            Dim lUpdateEntitaCod As Integer = 0
            Dim dtOperazione As DataTable = Nothing

            If configurazioneImportazione.TipoImportazioneAgronica = Tipo_Importazione.Tipo_Importazione_ShapeFile.importaCatasto_DXF Then

                Dim leggiDati As New AgronicaCoreGisDAL.GIS_Entita_R

                dtOperazione = leggiDati.Leggi(
                    _objParametri_Server.PivaSuperUser,
                    0,
                    TipiEnumerativi.enum_GIS2012_TipoEntita.CATASTO,
                    "",
                    0,
                    0,
                    0,
                    0,
                    shpCatasto.IstatP,
                    shpCatasto.IstatC,
                    shpCatasto.Sezione,
                    shpCatasto.Foglio,
                    shpCatasto.Particella,
                    shpCatasto.Subalterno,
                    0,
                    0,
                    0,
                    0,
                    configurazioneImportazione.CodiceFiscaleTecnico,
                    "",
                    Nothing,
                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                    "",
                    "",
                    _objParametri_Server,
                    _objParametri_Utenti
                    )
            End If
            'leggo per update particelle

            Dim lDtOperazioneRowsCount As Integer = 0
            If dtOperazione IsNot Nothing Then
                lDtOperazioneRowsCount = dtOperazione.Rows.Count
            End If

            'procedi con scrittura
            If configurazioneImportazione.TipoImportazioneAgronica <> Tipo_Importazione.Tipo_Importazione_ShapeFile.importaCatasto_DXF Or (
                    configurazioneImportazione.TipoImportazioneAgronica = Tipo_Importazione.Tipo_Importazione_ShapeFile.importaCatasto_DXF And (
                    lDtOperazioneRowsCount = 0 Or
                    (lDtOperazioneRowsCount > 0 AndAlso configurazioneImportazione.ConfigurazioneImportazione_Catasto.AzioneSuDati_1Sovrascrive_2Ignora_3Aggiunge = 1)
                    )
                    ) Then

                Dim lDataCreazioneLetto As DateTime = Now
                'se l'importazione non riguarda il catasto, oppure se si tratta di inserimento di nuova particella...
                If configurazioneImportazione.TipoImportazioneAgronica <> Tipo_Importazione.Tipo_Importazione_ShapeFile.importaCatasto_DXF Or
                    lDtOperazioneRowsCount = 0 Then

                    lTipoOperazioneDB = 1
                Else
                    'se sono qui ho la certezza che dtOperazione è diverso da nothing
                    lTipoOperazioneDB = 2
                    lElementoGrafico_cod = dtOperazione.Rows(0)("ElementoGrafico_COD")
                    lDataCreazioneLetto = dtOperazione.Rows(0)("Data_Creazione")
                    lUpdateEntitaCod = dtOperazione.Rows(0)("Entita_cod")
                End If

                If Ricetta_Operazione_Cod = 0 Then

                    Ricetta_Operazione_Cod = configurazioneImportazione.Ricetta_Operazione_Cod
                End If

                Dim newEntitaElement = <Entita TipoOperazioneDB=<%= lTipoOperazioneDB %> ecolor="256" eline="256" rad="15" text=<%= text %> validita_inizio="01/01/1900" validita_fine="31/12/2100" username_creazione="" username_modifica="">
                                           <layers>
                                               <layer tipologia_layer="1"><%= layer_codAtt %></layer>
                                           </layers>
                                           <EntitaGIAS>
                                               <DatoGias>
                                                   <PivaSuperUser><%= configurazioneImportazione.PivaSuperUSer %></PivaSuperUser>
                                                   <Entita_Cod><%= lUpdateEntitaCod %></Entita_Cod>
                                                   <TipoEntita_Cod><%= configurazioneImportazione.LTipoEntitaCod %></TipoEntita_Cod>
                                                   <Piva><%= configurazioneImportazione.Piva %></Piva>
                                                   <Sa_Cod><%= configurazioneImportazione.SaCod %></Sa_Cod>
                                                   <Appezza><%= configurazioneImportazione.Appezza %></Appezza>
                                                   <Campo_Cod><%= configurazioneImportazione.CampoCod %></Campo_Cod>
                                                   <Id_Imp><%= configurazioneImportazione.RegImpianto %></Id_Imp>
                                                   <PROV><%= shpCatasto.IstatP %></PROV>
                                                   <COM><%= shpCatasto.IstatC %></COM>
                                                   <SEZIONE><%= shpCatasto.Sezione %></SEZIONE>
                                                   <FOGLIO><%= shpCatasto.Foglio %></FOGLIO>
                                                   <NUMERO><%= shpCatasto.Particella %></NUMERO>
                                                   <SUBALTERNO><%= shpCatasto.Subalterno %></SUBALTERNO>
                                                   <Programmazione_Entita_Cod><%= lProgrammazioneEntitaCod %></Programmazione_Entita_Cod>
                                                   <Programmazione_Cod><%= lProgrammazione_cod %></Programmazione_Cod>
                                                   <Id_Agenda>0</Id_Agenda>
                                                   <id_mov_det>0</id_mov_det>
                                                   <Ricetta_Operazione_Cod><%= Ricetta_Operazione_Cod %></Ricetta_Operazione_Cod>
                                                   <analisi_campione_cod>0</analisi_campione_cod>
                                                   <OLDGrafica_ID></OLDGrafica_ID>
                                                   <inviato><%= inviato %></inviato>
                                                   <Data_Creazione><%= AgronicaCoreUtility.DataOra.DataOraToDate_SQL_ISO(lDataCreazioneLetto) %></Data_Creazione>
                                                   <Data_Modifica><%= AgronicaCoreUtility.DataOra.DataOraToDate_SQL_ISO(Now) %></Data_Modifica>
                                                   <Username_Creazione>agronica</Username_Creazione>
                                                   <Username_Modifica>agronica</Username_Modifica>
                                                   <Validita_Inizio><%= configurazioneImportazione.DataInizioValidita.ToString("s") %></Validita_Inizio>
                                                   <Validita_Fine><%= configurazioneImportazione.DataFineValidita.ToString("s") %></Validita_Fine>
                                               </DatoGias>
                                           </EntitaGIAS>
                                       </Entita>

                Dim sNodeDoc As XDocument = Shape2XDocument(shape, configurazioneImportazione, ParametriCartografici, lElementoGrafico_cod, objParametriServer)

                ' VAnni: 23/3/2017: indico che il dato è stato importato
                sNodeDoc.Root.@Flag_GPS = 2

                newEntitaElement.Add(sNodeDoc.FirstNode)
                xDocRval.Root.Add(newEntitaElement)

                iteration += 1
                If iteration = stopME And stopME <> -1 Then

                    Exit For
                End If

            End If
            'procedi con scrittura...

        Next 'shape In _loadedShapefile
        Scrivi_LOG(objParametriServer, "ShapeFileToAgronicaGis2012.convert", $"preparato dataset per { iteration.ToString()} nuovi poligoni da importare", False)
#If False Then
        For Each shape In _loadedShapefile.Records

            lxyz.Clear()


            For Each p In shape.Points
                Dim tmpXyz As New xyz
                If configurazioneImportazione.TrasformaSistemaRiferimento Then
                    tmpXyz.X = p.X
                    tmpXyz.Y = p.Y
                Else
                    tmpXyz.X = p.Y
                    tmpXyz.Y = p.X
                End If

                lxyz.Add(tmpXyz)

            Next



            Dim layer_codAtt As Integer
            If configurazioneImportazione.LayerCod = -1 Then
                layer_codAtt = iteration + 1
            Else
                layer_codAtt = configurazioneImportazione.LayerCod
            End If

            Dim text As String = collapseAttributes(shape, "§ ")

            If configurazioneImportazione.LayerCod = -1 Then
                Dim newDescr = <valori codice=<%= layer_codAtt %>><%= text %></valori>
                xDocRval.<DatiEntita>.<layersdescrizioni>.<layer>.First.Add(newDescr)
            Else
                If iteration = 0 Then
                    Dim newDescr = <valori codice=<%= layer_codAtt %>><%= text %></valori>
                    xDocRval.<DatiEntita>.<layersdescrizioni>.<layer>.First.Add(newDescr)
                End If

            End If


            Dim vshapeFile_fullFileName = configurazioneImportazione.ShapeFileFullFileName.Split("\")

            Dim cau_mov As Integer
            cau_mov = getCau_MovDatoLav_cod(configurazioneImportazione.LavCod)


            ListaDati.Add("NomeDir§ " & vshapeFile_fullFileName(vshapeFile_fullFileName.Length - 2) & "|PivaSuperUser§ " & configurazioneImportazione.PivaSuperUSer & "|Piva§ " & configurazioneImportazione.Piva & "|sa_cod§ " & configurazioneImportazione.SaCod & "|campo_cod§ " & configurazioneImportazione.CampoCod & "|Appezza§ " & configurazioneImportazione.Appezza & "|reg_impianto§ " & configurazioneImportazione.RegImpianto & "|cod_risum§ " & configurazioneImportazione.CodRisum & "|programmazione_cod§ " & configurazioneImportazione.ProgrammazioneCod & "|cod_mac§ " & configurazioneImportazione.CodMac & "|lav_cod§ " & configurazioneImportazione.LavCod & "|lav_des§ " & configurazioneImportazione.LavDes & "|cau_mov§ " & cau_mov & "|anno§ " & configurazioneImportazione.Anno & "|" & text)

            Dim CodBelfiore As String = ""
            Dim lIstatP As String = ""
            Dim listatc As String = ""
            Dim lid_sezc As String = ""
            Dim lFoglio As String = ""
            Dim lParticella As String = ""
            Dim lSub As String = ""

            ConfigurazioneImportazioneController.RecuperaChiaviLetturaCatasto(configurazioneImportazione, shape, CodBelfiore, lIstatP, listatc, lid_sezc, lFoglio, lParticella, lSub)

            ' VAnni: 29/7/2019: se esiste un foglio catastale allora sto importando il catasto. Leggo codifica prov com da codice belfiore
            If lFoglio <> "-1" AndAlso CodBelfiore <> "" AndAlso lIstatP = "0" AndAlso listatc = "0" Then
                LetturaBelfiore.LeggiDecodeBelfioreDaDB(CodBelfiore, lIstatP, listatc)
            End If


            Try

                Dim DatiArray As String() = ListaDati.ToArray
                ListaDati.Clear()


                chiusuraTranansazione = False
                Dim Aggregatore As New AgronicaCoreGisDAL.GIS_OperazioniCartograficheDB
                If configurazioneImportazione.TipoImportazioneAgronica = Tipo_Importazione.Tipo_Importazione_ShapeFile.Importazione_Trimble And Ricetta_Operazione_Cod = 0 And configurazioneImportazione.ShapeFileFullFileName.ToLower.EndsWith("coverage.shp") Then
                    AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, _objParametri_Server)

                    Dim helperGiasDati As New AgronicaSHPWrapper.InterpretaDatiDBF.InterpretaDatiDBF
                    helperGiasDati.ImportaDatiDBF(DatiArray, {"|"c, "§"c}, configurazioneImportazione.TipoImportazioneAgronica, _objParametri_Server)

                    Ricetta_Operazione_Cod = GetValue(DatiArray(0).Split({"|"c, "§"c}), "Ricetta_Operazione_Cod")

                    chiusuraTranansazione = True
                End If

                If configurazioneImportazione.TipoImportazioneAgronica = Tipo_Importazione.Tipo_Importazione_ShapeFile.Importazione_Agrea_Crea_Planning Then
                    AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, _objParametri_Server)
                    Aggregatore.AggregaAppezzamentiContiguiComeParticelle(13, 13, 0, _objParametri_Server)

                    chiusuraTranansazione = True
                End If

                If configurazioneImportazione.TipoImportazioneAgronica = Tipo_Importazione.Tipo_Importazione_ShapeFile.Importazione_Agrea_Crea_Planning Then
                    AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, _objParametri_Server)
                    Dim helperGiasDati As New AgronicaSHPWrapper.InterpretaDatiDBF.InterpretaDatiDBF
                    helperGiasDati.ImportaDatiDBF(DatiArray, {"|"c, "§"c}, configurazioneImportazione.TipoImportazioneAgronica, _objParametri_Server)

                    lProgrammazioneEntitaCod = GetValue(DatiArray(0).Split({"|"c, "§"c}), "ProgrammazioneEntita_cod")
                    lProgrammazione_cod = GetValue(DatiArray(0).Split({"|"c, "§"c}), "Programmazione_cod")
                    chiusuraTranansazione = True
                End If

                If chiusuraTranansazione Then
                    G2G_Chiusura_Transazione(1)
                    chiusuraTranansazione = False
                End If


            Catch ex As Exception

                G2G_Chiusura_Transazione(2)
                chiusuraTranansazione = False

            End Try


            Select Case configurazioneImportazione.TipoImportazioneAgronica
                Case Tipo_Importazione.Tipo_Importazione_ShapeFile.Importazione_Agrea_Crea_Planning
                    inviato = "-1"

                Case Tipo_Importazione.Tipo_Importazione_ShapeFile.importaCatasto_DXF
                    inviato = progressivoImportCatasto

                Case Else
                    inviato = "0"
            End Select



            If lProgrammazione_cod = 0 Then
                lProgrammazione_cod = configurazioneImportazione.ProgrammazioneCod
            End If

            Dim dAdesso As DateTime = Now
            Dim Adesso As String = AgronicaCoreUtility.DataOra.DataOraToDate_SQL_ISO(dAdesso)

            Dim lElementoGrafico_cod As Integer = 0
            Dim lTipoOperazioneDB As String = "1"
            Dim lUpdateEntitaCod As Integer = 0
            Dim dtOperazione As DataTable

            If configurazioneImportazione.TipoImportazioneAgronica = Tipo_Importazione.Tipo_Importazione_ShapeFile.importaCatasto_DXF Then



                Dim leggiDati As New AgronicaCoreGisDAL.GIS_Entita_R
                dtOperazione = leggiDati.Leggi(
                    _objParametri_Server.PivaSuperUser,
                    0,
                    TipiEnumerativi.enum_GIS2012_TipoEntita.CATASTO,
                    "",
                    0,
                    0,
                    0,
                    0,
                    lIstatP,
                    listatc,
                    lid_sezc,
                    lFoglio,
                    lParticella,
                    lSub,
                    0,
                    0,
                    0,
                    0,
                   configurazioneImportazione.CodiceFiscaleTecnico,
                    "",
                    Nothing,
                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                    "",
                    "",
                    _objParametri_Server
                    )

            End If
            'leggo per update particelle

            Dim lDtOperazioneRowsCount As Integer = 0
            If Not dtOperazione Is Nothing Then
                lDtOperazioneRowsCount = dtOperazione.Rows.Count
            End If


            'procedi con scrittura
            If configurazioneImportazione.TipoImportazioneAgronica <> Tipo_Importazione.Tipo_Importazione_ShapeFile.importaCatasto_DXF Or (
                    configurazioneImportazione.TipoImportazioneAgronica = Tipo_Importazione.Tipo_Importazione_ShapeFile.importaCatasto_DXF And (
                    lDtOperazioneRowsCount = 0 Or
                    (lDtOperazioneRowsCount > 0 AndAlso configurazioneImportazione.ConfigurazioneImportazione_Catasto.AzioneSuDati_1Sovrascrive_2Ignora_3Aggiunge = 1)
                )
            ) Then


                Dim lDataCreazioneLetto As DateTime = Now
                'se l'importazione non riguarda il catasto, oppure se si tratta di inserimento di nuova particella...
                If configurazioneImportazione.TipoImportazioneAgronica <> Tipo_Importazione.Tipo_Importazione_ShapeFile.importaCatasto_DXF Or
                    lDtOperazioneRowsCount = 0 Then

                    lTipoOperazioneDB = 1
                Else
                    'se sono qui ho la certezza che dtOperazione è diverso da nothing
                    lTipoOperazioneDB = 2
                    lElementoGrafico_cod = dtOperazione.Rows(0)("ElementoGrafico_COD")
                    lDataCreazioneLetto = dtOperazione.Rows(0)("Data_Creazione")
                    lUpdateEntitaCod = dtOperazione.Rows(0)("Entita_cod")
                End If

                If Ricetta_Operazione_Cod = 0 Then
                    Ricetta_Operazione_Cod = configurazioneImportazione.Ricetta_Operazione_Cod
                End If

                Dim newEntitaElement = <Entita TipoOperazioneDB=<%= lTipoOperazioneDB %> ecolor="256" eline="256" rad="15" text=<%= text %> validita_inizio="01/01/1900" validita_fine="31/12/2100" username_creazione="" username_modifica="">
                                           <layers>
                                               <layer tipologia_layer="1"><%= layer_codAtt %></layer>
                                           </layers>
                                           <EntitaGIAS>
                                               <DatoGias>
                                                   <PivaSuperUser><%= configurazioneImportazione.PivaSuperUSer %></PivaSuperUser>
                                                   <Entita_Cod><%= lUpdateEntitaCod %></Entita_Cod>
                                                   <TipoEntita_Cod><%= configurazioneImportazione.LTipoEntitaCod %></TipoEntita_Cod>
                                                   <Piva><%= configurazioneImportazione.Piva %></Piva>
                                                   <Sa_Cod><%= configurazioneImportazione.SaCod %></Sa_Cod>
                                                   <Appezza><%= configurazioneImportazione.Appezza %></Appezza>
                                                   <Campo_Cod><%= configurazioneImportazione.CampoCod %></Campo_Cod>
                                                   <Id_Imp><%= configurazioneImportazione.RegImpianto %></Id_Imp>
                                                   <PROV><%= lIstatP %></PROV>
                                                   <COM><%= listatc %></COM>
                                                   <SEZIONE><%= lid_sezc %></SEZIONE>
                                                   <FOGLIO><%= lFoglio %></FOGLIO>
                                                   <NUMERO><%= lParticella %></NUMERO>
                                                   <SUBALTERNO><%= lSub %></SUBALTERNO>
                                                   <Programmazione_Entita_Cod><%= lProgrammazioneEntitaCod %></Programmazione_Entita_Cod>
                                                   <Programmazione_Cod><%= lProgrammazione_cod %></Programmazione_Cod>
                                                   <Id_Agenda>0</Id_Agenda>
                                                   <id_mov_det>0</id_mov_det>
                                                   <Ricetta_Operazione_Cod><%= Ricetta_Operazione_Cod %></Ricetta_Operazione_Cod>
                                                   <analisi_campione_cod>0</analisi_campione_cod>
                                                   <OLDGrafica_ID></OLDGrafica_ID>
                                                   <inviato><%= inviato %></inviato>
                                                   <Data_Creazione><%= AgronicaCoreUtility.DataOra.DataOraToDate_SQL_ISO(lDataCreazioneLetto) %></Data_Creazione>
                                                   <Data_Modifica><%= AgronicaCoreUtility.DataOra.DataOraToDate_SQL_ISO(Now) %></Data_Modifica>
                                                   <Username_Creazione>agronica</Username_Creazione>
                                                   <Username_Modifica>agronica</Username_Modifica>
                                                   <Validita_Inizio>1900-01-01T00:00:00</Validita_Inizio>
                                                   <Validita_Fine>2100-12-31T00:00:00</Validita_Fine>
                                               </DatoGias>
                                           </EntitaGIAS>
                                       </Entita>


                Dim sNodeDoc As XDocument


                If configurazioneImportazione.TrasformaSistemaRiferimento Then
                    sNodeDoc = XDocument.Parse(
                    wktToGeoML.Trasforma(
                        cconverter.WKTPolygonWGS84_from_WKTPolygonED50(wktHelp.CreaPoligonoDaCoordinate(lxyz), True, ParametriCartografici),
                        False,
                        True,
                        True,
                        lElementoGrafico_cod
                    )
                )
                Else
                    sNodeDoc = XDocument.Parse(
                    wktToGeoML.Trasforma(
                        wktHelp.CreaPoligonoDaCoordinate(lxyz),
                        False,
                        False,
                        False,
                        lElementoGrafico_cod
                    )
                )
                End If


                ' VAnni: 23/3/2017: indico che il dato è stato importato
                sNodeDoc.Root.@Flag_GPS = 2

                newEntitaElement.Add(sNodeDoc.FirstNode)
                xDocRval.Root.Add(newEntitaElement)

                iteration += 1
                If iteration = stopME And stopME <> -1 Then
                    Exit For
                End If

            End If
            'procedi con scrittura...


        Next 'shape In _loadedShapefile.Records
#End If

        Dim rval As String = xmlHelper.RemoveNamespace(xDocRval, {"http://www.opengis.net/gml"}.ToList).ToString.Replace("xmlns=""""", "")

        Dim tmpDoc As XDocument = XDocument.Parse(rval)

        'scrive i layer (verificare)

        If configurazioneImportazione.GestioneRiportoDatiInGias = Tipo_Importazione.Tipo_GestioneRiportoDatiInGias.RiportoSuAllegati Then

            Dim xmlAgronica2012 As String = tmpDoc.ToString

            Dim FlagTransazioneLocale As Boolean = False
            Dim FlagConnessioneLocale As Boolean = False

            'TODO: Gestire la transazione
            Try

                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                'Apro la connessione al DB
                AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                FlagTransazioneLocale,
                                                                                _objParametri_Server)

                Dim ScriviSuAllegati As New AgronicaCoreAnagrafeBIZ.Allegati_Documenti_W
                Dim rvalScriviAllegati As RispostaStandard

                If configurazioneImportazione.Allegati_Documenti_Cod = 0 Then

                    rvalScriviAllegati = ScriviSuAllegati.PrecisionFarmingScriviSuAllegati(
                        configurazioneImportazione.CategoriaDocumento,
                        "",
                        "",
                        _objParametri_Server,
                        configurazioneImportazione.Piva,
                        configurazioneImportazione.SaCod,
                        configurazioneImportazione.Appezza,
                        Ricetta_Operazione_Cod,
                        configurazioneImportazione.RegImpianto,
                        xmlAgronica2012,
                        Nothing,
                        "",
                        configurazioneImportazione.OUTPUT_Allegati_Documenti_Cod
                    )

                    If rvalScriviAllegati.RispostaOK = False Then
                        Throw New Exception("Errore scrittura allegati documenti")
                    End If

                    Dim AlertEnittaR As New AgronicaCoreScadenziario.Alert_Entita_R
                    Dim AlertIndiceW As New AgronicaCoreScadenziario.Alert_Indice_W

                    Dim dtAlertEntita As DataTable =
                        AlertEnittaR.Leggi_con_documenti(False, configurazioneImportazione.OUTPUT_Allegati_Documenti_Cod, 0, "", "", _objParametri_Server)

                    Dim iDAlertEntita As Integer = dtAlertEntita.Rows(0)("ID_Alert_Entita")
                    AlertIndiceW.ScriviEntitaxIndice(
                        configurazioneImportazione.Piva,
                        iDAlertEntita,
                        -1,
                        0, "", configurazioneImportazione.Allegati_Documenti_Cod_collegamento,
                        AGRODATAINIZIO, AGRODATAFINE,
                        _objParametri_Server
                    )
                Else
                    'update
                    rvalScriviAllegati = ScriviSuAllegati.PrecisionFarmingAggiornaAllegati(
                        configurazioneImportazione.CategoriaDocumento,
                        configurazioneImportazione.Allegati_Documenti_Cod,
                        "",
                        "",
                        _objParametri_Server,
                        configurazioneImportazione.Piva,
                        configurazioneImportazione.SaCod,
                        configurazioneImportazione.Appezza,
                        Ricetta_Operazione_Cod,
                        configurazioneImportazione.RegImpianto,
                        xmlAgronica2012,
                        Nothing,
                        ""
                    )

                    If rvalScriviAllegati.RispostaOK = False Then
                        Throw New Exception("Errore aggiornamento allegati documenti")
                    End If
                End If
                'se devo associare l'allegato

                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                'Chiudo la transazione e la connessione al DB
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, _objParametri_Server)
                '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            Catch ex As Exception

                'Faccio il rollback della transazione
                If Not _objParametri_Server.objTransazione Is Nothing Then
                    'objParametri.objTransazione.Rollback()
                    'objParametri.objTransazione = Nothing
                    AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, _objParametri_Server)

                End If

                Dim messaggioErrore As String = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)
                Dim Messaggio As String = ""

                If messaggioErrore <> "" Then

                    Messaggio += "ERR: Sono stati rilevati i seguenti errori : " & vbCrLf
                    Messaggio += "" & vbCrLf
                    Messaggio += messaggioErrore
                    Messaggio += "" & vbCrLf
                    Messaggio += "Ritentare il salvataggio dopo la correzione ..."

                End If

                Throw New Exception(Messaggio, ex)

            Finally

                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, _objParametri_Server)

            End Try

        End If

        If configurazioneImportazione.GestioneRiportoDatiInGias = Tipo_Importazione.Tipo_GestioneRiportoDatiInGias.RiportoAutomaticoDeiDati Then

            'scrive le entità
            Dim listOFEntita = (
                    From a In tmpDoc.<DatiEntita>.<Entita>
                    Select a).ToList()

            Dim conteggioNonImportati As Integer = 0

            Dim ScriviElementiGrafici As New AgronicaCoreGisBIZ.GIS_Entita_W

            Scrivi_LOG(objParametriServer, "ShapeFileToAgronicaGis2012.convert", "inizio scrittura poligoni GIS", False)

            For Each elemento In listOFEntita

                'per test formato database ...

                Try

                    'transizione sulla singola chiamata di un entità grafica/GIAS

                    Dim idle As Integer
                    AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, _objParametri_Server)
                    ScriviElementiGrafici.scrivi(elemento.ToString, idle, _objParametri_Server)

                    G2G_Chiusura_Transazione(1)

                Catch ex As Exception
                    Scrivi_LOG(objParametriServer, "ShapeFileToAgronicaGis2012.convert", "errore scrittura poligoni GIS" & vbCrLf & ex.Message & vbCrLf & elemento.ToString(), False)

                    conteggioNonImportati += 1
                    G2G_Chiusura_Transazione(2)

                    'If Not My.Computer.FileSystem.GetFileInfo(NonImportatiSQL).IsReadOnly Then
                    '    My.Computer.FileSystem.WriteAllText(NonImportatiSQL, head & elemento.ToString & tail, True)
                    'End If

                End Try
            Next

            Scrivi_LOG(objParametriServer, "ShapeFileToAgronicaGis2012.convert", "fine scrittura poligoni GIS", False)

            If configurazioneImportazione.TipoImportazioneAgronica = Tipo_Importazione.Tipo_Importazione_ShapeFile.importaCatasto_DXF Then
                Scrivi_LOG(objParametriServer, "ShapeFileToAgronicaGis2012.convert", "inizio riporto catasto da poligoni GIS", False)
                ConfigurazioneImportazioneController.RiportoCatastoDaGis(
                    configurazioneImportazione.ConfigurazioneImportazione_Catasto.AzioneSuDati_1Sovrascrive_2Ignora_3Aggiunge,
                    configurazioneImportazione.ConfigurazioneImportazione_Catasto.CreaLayerTestuale,
                    inviato,
                    _objParametri_Server
                )
                Scrivi_LOG(objParametriServer, "ShapeFileToAgronicaGis2012.convert", "fine riporto catasto da poligoni GIS", False)
            End If

            If configurazioneImportazione.ProgrammazioneCod <> 0 Then

                Try
                    AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, _objParametri_Server)

                    Dim AssegnaOperazioni As New AgronicaCoreGisDAL.GIS_Entita_W
                    AssegnaOperazioni.AssegnaOperazioniPF_Impianto(
                        configurazioneImportazione.PivaSuperUSer,
                        configurazioneImportazione.Piva,
                        configurazioneImportazione.SaCod,
                        configurazioneImportazione.ProgrammazioneCod,
                        "",
                        "",
                        _objParametri_Server
                    )

                    G2G_Chiusura_Transazione(1)

                Catch ex As Exception

                    G2G_Chiusura_Transazione(2)
                End Try

            End If
        End If
        'se riporto automatico

        Return rval
    End Function

    Private Sub G2G_Chiusura_Transazione(ByVal Flag_Commit1_Rollback2 As Integer)

        Try
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(Flag_Commit1_Rollback2, _objParametri_Server)

        Catch ex As Exception

        End Try
    End Sub

    'Private Sub FinalizzaRisultato(ByVal fileDaSalvare As String)
    '    Dim finalData As String
    '    finalData = My.Computer.FileSystem.ReadAllText(fileDaSalvare)


    '    finalData = finalData.Replace(head, "")
    '    finalData = finalData.Replace(tail, "")

    '    finalData = head & finalData & tail

    '    Dim xmlFinaleElaborato As XDocument = XDocument.Parse(finalData)
    '    Dim layers = <layersdescrizioni>
    '                     <layer tipologia_layer="1" nome_layer="entità">
    '                         <valori codice="1">Azienda</valori>
    '                     </layer>
    '                     <layer tipologia_layer="10" nome_layer="Conversione coordinate">
    '                         <valori codice="1">GPS Originali</valori>
    '                         <valori codice="2">Convertite</valori>
    '                     </layer>
    '                 </layersdescrizioni>

    '    Dim l = <layer tipologia_layer="100" nome_layer="Aziende Sementiere"></layer>
    '    For Each az In _LayersImprese
    '        Dim valore = <valori codice=<%= az.Split("|")(0) %>><%= az.Split("|")(1) %></valori>
    '        l.Add(valore)

    '    Next

    '    layers.Add(l)

    '    Dim ListaNS As New List(Of String)
    '    ListaNS.Add("http://www.opengis.net/gml")
    '    xmlFinaleElaborato.Root.Add(layers)

    '    finalData = xmlHelper.RemoveNamespace(xmlFinaleElaborato, ListaNS).ToString.Replace("xmlns=""""", "")

    '    If Not My.Computer.FileSystem.GetFileInfo(fileDaSalvare).IsReadOnly Then
    '        My.Computer.FileSystem.WriteAllText(fileDaSalvare, finalData, False)
    '    End If

    'End Sub

    'Private Function getAttribute(ByVal colName As String, ByVal record As ShapeFileRecord) As String
    '    Dim rval As String = ""
    '    If Not record.Attributes Is Nothing Then
    '        For i As Integer = 0 To record.Attributes.ItemArray.GetLength(0) - 1
    '            If record.Attributes.Table.Columns.Item(i).ColumnName.ToLower = colName.ToLower Then
    '                rval = record.Attributes(i).ToString()
    '                Exit For
    '            End If
    '        Next
    '    End If

    '    Return rval
    'End Function

    'Private Function collapseAttributes(ByVal record As ShapeFileRecord, ByVal separaCampi As String) As String
    '    Dim attr As String = ""
    '    If Not record.Attributes Is Nothing Then
    '        For i As Integer = 0 To record.Attributes.ItemArray.GetLength(0) - 1
    '            attr += (record.Attributes.Table.Columns.Item(i).ColumnName & separaCampi & record.Attributes(i).ToString() & "|")
    '        Next
    '    End If

    '    Return attr
    'End Function

End Class
