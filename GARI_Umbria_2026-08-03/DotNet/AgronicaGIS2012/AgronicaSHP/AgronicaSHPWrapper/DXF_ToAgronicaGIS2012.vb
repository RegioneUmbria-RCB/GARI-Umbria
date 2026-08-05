Imports <xmlns="http://www.agronica.it/grafica/">


Imports System.Configuration.ConfigurationManager

Imports AgronicaGIS2012.Commons
Imports AgronicaGIS2012.Commons.DataOraHelper

Imports AgronicaConversioneCartografiaGias.FormatsConverter
Imports AgronicaSHPWrapper.InterpretaDatiDBF
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaConversioneCartografiaGias.Agronica
Imports System.Text

Public Class DXF_ToAgronicaGIS2012

    Private Const layerLettoDaDXF_PARTICELLE As String = "PARTICELLE"
    Private Const layerLettoDaDXF_ACQUE As String = "ACQUE"

    Private _objParametri_Server As AgronicaCoreParametri
    Private _objParametri_Utenti As AgronicaCoreParametri

    Private head As String =
            "<DatiEntita xmlns=""http://www.agronica.it/grafica/"" xmlns:gml=""http://www.opengis.net/gml"">"

    Private tail As String =
        "</DatiEntita>"



    Private _CacheListaComuni As New List(Of DecodeComuniIstatCodBelfiore)



    ''' <summary>
    ''' restituisce il file in formato agronica.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <param name="lTipoEntita_Cod"></param>
    ''' <param name="trasformaSistemaRiferimento"></param>
    Public Function convert(
                configurazioneImportazione As ConfigurazioneImportazione,
                ByVal objParametri_Server As AgronicaCoreParametri,
                ByVal objParametri_Utenti As AgronicaCoreParametri
                ) As String


        Dim lDXFFile_fullFileNameSplit As String() = configurazioneImportazione.ShapeFileFullFileName.Split("\")
        Dim soloNomeFile As String = lDXFFile_fullFileNameSplit(lDXFFile_fullFileNameSplit.Length - 1)

        _objParametri_Server = objParametri_Server
        _objParametri_Utenti = objParametri_Utenti

        Dim progressivoImportCatasto As Integer
        Dim AgroSequenze As New AgronicaCoreDataProvider.Agro_Sequenze

        'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
        progressivoImportCatasto =
            AgroSequenze.NuovoId_Tabella("progressivoImportCatasto", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, _objParametri_Server)
        'progressivoImportCatasto = AgroSequenze.Agronica_SequenzaTabelle_NuovoID(
        '    "progressivoImportCatasto",
        '    _objParametri_Server
        ')

        Dim rval As String = Nothing
        Dim ScriviElementiGrafici As New AgronicaCoreGisBIZ.GIS_Entita_W
        Dim tmpDoc As XDocument = Nothing


        DXF_CaricaFileSuAgroXml(
            configurazioneImportazione.ConfigurazioneImportazione_Catasto.FiltroParticelleCatastali,
            configurazioneImportazione.ShapeFileFullFileName,
            configurazioneImportazione.PivaSuperUSer,
            configurazioneImportazione.CampoCod,
            configurazioneImportazione.Appezza,
            configurazioneImportazione.RegImpianto,
            configurazioneImportazione.TipoImportazioneAgronica,
            configurazioneImportazione.TrasformaSistemaRiferimento,
            configurazioneImportazione.GeoRiferimentoCod,
            configurazioneImportazione.LTipoEntitaCod,
            configurazioneImportazione.ConfigurazioneImportazione_Catasto.ListaLayersDXFAgenziaEntrate,
            configurazioneImportazione.ConfigurazioneImportazione_Catasto.AzioneSuDati_1Sovrascrive_2Ignora_3Aggiunge,
            configurazioneImportazione.CodiceFiscaleTecnico,
            soloNomeFile,
            progressivoImportCatasto,
            rval,
            tmpDoc,
            _objParametri_Server,
            _objParametri_Utenti
        )



        'scrive i layer (verificare)

        'scrive le entità
        Dim listOFEntita = (
                    From a In tmpDoc.<DatiEntita>.<Entita>
                    Select a).ToList()

        Dim conteggioNonImportati As Integer = 0

        For Each elemento In listOFEntita

            'per test formato database ...
            Try

                'transizione sulla singola chiamata di un entità grafica/GIAS

                Dim idle As Integer
                AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, _objParametri_Server)
                ScriviElementiGrafici.scrivi(elemento.ToString, idle, _objParametri_Server)

                G2G_Chiusura_Transazione(1)

            Catch ex As Exception

                conteggioNonImportati += 1
                G2G_Chiusura_Transazione(2)

                'If Not My.Computer.FileSystem.GetFileInfo(NonImportatiSQL).IsReadOnly Then
                '    My.Computer.FileSystem.WriteAllText(NonImportatiSQL, head & elemento.ToString & tail, True)
                'End If


            End Try


        Next

        'FinalizzaRisultato(NonImportatiSQL)

        If configurazioneImportazione.ConfigurazioneImportazione_Catasto.ListaLayersDXFAgenziaEntrate.Contains("PARTICELLE") Then

            ConfigurazioneImportazioneController.RiportoCatastoDaGis(
            configurazioneImportazione.ConfigurazioneImportazione_Catasto.AzioneSuDati_1Sovrascrive_2Ignora_3Aggiunge,
            configurazioneImportazione.ConfigurazioneImportazione_Catasto.CreaLayerTestuale, progressivoImportCatasto, _objParametri_Server)

        End If

        Return rval

    End Function


    Public Sub DXF_CaricaFileSuAgroXml(
            FiltroParticelle As String,
            DXFFile_fullFileName As String,
            PivaSuperUSer As String,
            campo_cod As Integer,
            appezza As Integer,
            reg_impianto As Integer,
            tipo_importazione_agronica As Tipo_Importazione.Tipo_Importazione_ShapeFile,
            trasformaSistemaRiferimento As Boolean,
            GEORiferimento_COD As Integer,
            lTipoEntita_Cod As String,
            ListaLayers As String,
            AzioneSuDati_1Sovrascrive_2Ignora_3Aggiunge As Short,
            Codice_Fiscale_Tecnico As String,
            soloNomeFile As String,
            progressivoImportCatasto As Integer,
            ByRef rval As String,
            ByRef tmpDoc As XDocument,
            Optional ByVal objParametri_Server As AgronicaCoreParametri = Nothing,
            Optional ByVal objParametri_Utenti As AgronicaCoreParametri = Nothing)

        If _objParametri_Server Is Nothing Then
            _objParametri_Server = objParametri_Server
        End If

        If _objParametri_Utenti Is Nothing Then
            _objParametri_Utenti = objParametri_Utenti
        End If

        Dim loadedDXFfile As New DXFImporter.DXFFile

        loadedDXFfile.ReadFromFile(DXFFile_fullFileName)

        rval = ""

        Dim xDocRval As XDocument = XDocument.Parse("<DatiEntita xmlns=""http://www.agronica.it/grafica/"" xmlns:gml=""http://www.opengis.net/gml""></DatiEntita>")
        Dim lay = <layersdescrizioni>
                      <layer tipologia_layer="1" nome_layer="entità">
                      </layer>
                  </layersdescrizioni>

        xDocRval.Root.Add(lay)


        Dim wktHelp As New WKT
        Dim wktToGeoML As New wkt_gml
        wktToGeoML.Soglia_ConsideraPuntiUguali = 0.000000005

        'trasformazione
        Dim leggiTrasformazione As New AgronicaCoreGisDAL.GIS_SistemiRiferimentoCartografia_R
        Dim dtLeggiTrasformazione As DataTable = leggiTrasformazione.Leggi(GEORiferimento_COD, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", _objParametri_Server)


        Dim ParametriCartografici As New ParametriCoordinateConverter With {
            .CSFromText = dtLeggiTrasformazione(0)("CSFrom"),
            .CStoText = dtLeggiTrasformazione(0)("CSTo"),
            .CStoGeoText = dtLeggiTrasformazione(0)("CStoGeo"),
            .AgronicaLatOffset = dtLeggiTrasformazione(0)("AgronicaLatOffset"),
            .AgronicaLonOffset = dtLeggiTrasformazione(0)("AgronicaLonOffset"),
            .LibreriaDaUsare = dtLeggiTrasformazione(0)("LibreriaDaUsare")
        }

        Dim codBelfioreLettura As New DecodeComuniIstatCodBelfiore_controller(_objParametri_Server)

        Dim stopME As Integer = -1
        If Debugger.IsAttached Then
            stopME = -1
        End If

        Dim iteration As Integer = 0

        Dim lxyz As New List(Of xyz)
        Dim ListaDati As New List(Of String)

        Dim lProgrammazioneEntitaCod As Integer = 0
        Dim Ricetta_Operazione_Cod As Integer = 0

        Dim cconverter As New CoordinateConverter

        Dim lIstatP As String = GetDatoParticellaDaNomeFile("istatp", soloNomeFile, codBelfioreLettura, Nothing)
        If lIstatP = "" Then
            lIstatP = "0"
        End If

        Dim listatc As String = GetDatoParticellaDaNomeFile("istatc", soloNomeFile, codBelfioreLettura, Nothing)
        If listatc = "0" Then
            listatc = "0"
        End If

        Dim lid_sezc As String = GetDatoParticellaDaNomeFile("id_sezc", soloNomeFile, codBelfioreLettura, Nothing)
        If lid_sezc = "" Then
            lid_sezc = "0"
        End If

        Dim lFoglio As String = GetDatoParticellaDaNomeFile("foglio", soloNomeFile, codBelfioreLettura, Nothing)
        If lFoglio = "" Then
            lFoglio = "-1"
        End If

        Dim lParticella As String
        Dim lSub As String

        Dim gmlHlp As New GML


        If FiltroParticelle Is Nothing Then
            FiltroParticelle = ""
        End If

        Dim ListaFiltroParticelle As List(Of String) = FiltroParticelle.Split(",").ToList

        For Each forma As DXFImporter.Shape In loadedDXFfile.ListaOggetti


            lxyz.Clear()

            Dim layerLettoDaDXF As String = ""

            If forma.shapeIdentifier = 5 AndAlso ListaLayers.Contains(CType(forma, DXFImporter.polyline).Layer) Then

                layerLettoDaDXF = CType(forma, DXFImporter.polyline).Layer

                Dim LayerDestinazione As Integer

                Select Case layerLettoDaDXF
                    Case layerLettoDaDXF_PARTICELLE
                        LayerDestinazione = TipiEnumerativi.enum_Gis_LayerElementiGrafici_std.CATASTO
                    Case layerLettoDaDXF_ACQUE
                        LayerDestinazione = TipiEnumerativi.enum_Gis_LayerElementiGrafici_std.Corpi_idrici_superficiali
                End Select



                Dim parte As DXFImporter.polyline = CType(forma, DXFImporter.polyline)

                lParticella = parte.CodiceAssociato
                If Not IsNumeric(lParticella) Then
                    lParticella = "-1"
                    lSub = parte.CodiceAssociato
                Else
                    lSub = "0"
                End If

                If String.IsNullOrEmpty(FiltroParticelle) OrElse ListaFiltroParticelle.Contains(lParticella) Then


                    For Each p As DXFImporter.Line In parte.ListOfLines
                        Dim tmpXyz As New xyz

                        'TODO: Vanni 17/10/2016, versione senza trasposizione del segno (Problema forse risolto in DXFFile.vb, riga 366).
                        tmpXyz.X = p.GetStartPoint.X
                        tmpXyz.Y = p.GetStartPoint.Y

                        '  Vanni, 20/06/2013 10:15:51: da capire la presenza o meno del segno ...
                        'tmpXyz.Y = Math.Abs(p.GetStartPoint.Y)
                        'tmpXyz.Y = p.GetStartPoint.Y

                        'Vanni, verificare il segno.
                        'If trasformaSistemaRiferimento Then
                        '    tmpXyz.Y = -p.GetStartPoint.Y
                        'Else
                        '    tmpXyz.Y = p.GetStartPoint.Y
                        'End If


                        lxyz.Add(tmpXyz)

                    Next

                    Dim inviato As String
                    If tipo_importazione_agronica = Tipo_Importazione.Tipo_Importazione_ShapeFile.Importazione_Agrea_Crea_Planning Then
                        inviato = "-1"
                    Else
                        inviato = "0"
                    End If

                    Dim descrizioneParticella As String = ""
                    Dim tipoEntitaAssegnato As String = TipiEnumerativi.enum_Gis_LayerElementiGrafici_std.Corpi_idrici_superficiali

                    Dim istatP As String = "0"
                    Dim istatC As String = "0"
                    Dim istatSezC As String = "0"
                    Dim istatFoglio As String = "0"
                    Dim istatParticella As String = "0"
                    Dim istatsubalterno As String = "0"

                    Dim lElementoGrafico_cod As Integer = 0
                    Dim lTipoOperazioneDB As String = "1"
                    Dim lUpdateEntitaCod As Integer = 0

                    Dim dtOperazione As DataTable

                    If layerLettoDaDXF = layerLettoDaDXF_PARTICELLE Then
                        istatP = lIstatP
                        istatC = listatc
                        istatSezC = lid_sezc
                        istatFoglio = lFoglio
                        istatParticella = lParticella
                        istatsubalterno = lSub
                        descrizioneParticella = getdescrizioneParticella(lIstatP, listatc, lid_sezc, lFoglio, lParticella, lSub)
                        tipoEntitaAssegnato = TipiEnumerativi.enum_Gis_LayerElementiGrafici_std.CATASTO




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
                                Codice_Fiscale_Tecnico,
                                "",
                                Nothing,
                                enumSelezioneVariabile.Selezione_TabellaCompleta,
                                "",
                                "",
                                _objParametri_Server,
                                _objParametri_Utenti
                            )


                    End If
                    'fine layer particelle

                    Dim lDtOperazioneRowsCount As Integer = 0
                    If Not dtOperazione Is Nothing Then
                        lDtOperazioneRowsCount = dtOperazione.Rows.Count
                    End If

                    If lDtOperazioneRowsCount = 0 Or
                            (lDtOperazioneRowsCount > 0 And AzioneSuDati_1Sovrascrive_2Ignora_3Aggiunge = 1) Then

                        Dim lDataCreazioneLetto As DateTime = Now
                        If lDtOperazioneRowsCount = 0 Then
                            lTipoOperazioneDB = 1
                        Else
                            lTipoOperazioneDB = 2
                            lElementoGrafico_cod = dtOperazione.Rows(0)("ElementoGrafico_COD")
                            lDataCreazioneLetto = dtOperazione.Rows(0)("Data_Creazione")
                            lUpdateEntitaCod = dtOperazione.Rows(0)("Entita_cod")
                        End If

                        Dim layer_codAtt As String = LayerDestinazione.ToString()
                        Dim EntitaElement As XElement



                        If parte.LType <> "INTERNA" Then


                            EntitaElement = <Entita TipoOperazioneDB=<%= lTipoOperazioneDB %> ecolor="256" eline="256" rad="15" text=<%= descrizioneParticella %> validita_inizio="01/01/1900" validita_fine="31/12/2100" username_creazione="" username_modifica="">
                                                <layers>
                                                    <layer tipologia_layer="1"><%= layer_codAtt %></layer>
                                                </layers>
                                                <EntitaGIAS>
                                                    <DatoGias>
                                                        <PivaSuperUser><%= PivaSuperUSer %></PivaSuperUser>
                                                        <Entita_Cod><%= lUpdateEntitaCod %></Entita_Cod>
                                                        <TipoEntita_Cod><%= tipoEntitaAssegnato %></TipoEntita_Cod>
                                                        <Piva></Piva>
                                                        <Sa_Cod>0</Sa_Cod>
                                                        <Appezza><%= appezza %></Appezza>
                                                        <Campo_Cod><%= campo_cod %></Campo_Cod>
                                                        <Id_Imp><%= reg_impianto %></Id_Imp>
                                                        <PROV><%= istatP %></PROV>
                                                        <COM><%= istatC %></COM>
                                                        <SEZIONE><%= istatSezC %></SEZIONE>
                                                        <FOGLIO><%= istatFoglio %></FOGLIO>
                                                        <NUMERO><%= istatParticella %></NUMERO>
                                                        <SUBALTERNO><%= istatsubalterno %></SUBALTERNO>
                                                        <Programmazione_Entita_Cod><%= lProgrammazioneEntitaCod %></Programmazione_Entita_Cod>
                                                        <Programmazione_Cod>0</Programmazione_Cod>
                                                        <Id_Agenda>0</Id_Agenda>
                                                        <id_mov_det>0</id_mov_det>
                                                        <Ricetta_Operazione_Cod><%= Ricetta_Operazione_Cod %></Ricetta_Operazione_Cod>
                                                        <analisi_campione_cod>0</analisi_campione_cod>
                                                        <OLDGrafica_ID></OLDGrafica_ID>
                                                        <inviato><%= progressivoImportCatasto %></inviato>
                                                        <Data_Creazione><%= CreateISO8601DateTimeFromSystemDateTime(lDataCreazioneLetto) %></Data_Creazione>
                                                        <Data_Modifica><%= CreateISO8601DateTimeFromSystemDateTime(Now) %></Data_Modifica>
                                                        <Username_Creazione>agronica</Username_Creazione>
                                                        <Username_Modifica>agronica</Username_Modifica>
                                                        <Validita_Inizio>1900-01-01T00:00:00</Validita_Inizio>
                                                        <Validita_Fine>2100-12-31T00:00:00</Validita_Fine>
                                                    </DatoGias>
                                                </EntitaGIAS>
                                            </Entita>

                        Else
                            EntitaElement = (
                                From i In xDocRval.<DatiEntita>.<Entita>
                                Where i.<EntitaGIAS>.<DatoGias>.<PROV>.Value = istatP _
                                 And i.<EntitaGIAS>.<DatoGias>.<COM>.Value = istatC _
                                 And i.<EntitaGIAS>.<DatoGias>.<SEZIONE>.Value = istatSezC _
                                 And i.<EntitaGIAS>.<DatoGias>.<FOGLIO>.Value = istatFoglio _
                                 And i.<EntitaGIAS>.<DatoGias>.<NUMERO>.Value = istatParticella _
                                 And i.<EntitaGIAS>.<DatoGias>.<SUBALTERNO>.Value = istatsubalterno
                            ).FirstOrDefault

                        End If


                        Dim sNodeDoc As XDocument

                        Dim ruota As Boolean = False
                        If lxyz.Count > 2 Then
                            ruota = True
                        End If

                        Dim wkt As String = ""
                        If trasformaSistemaRiferimento Then
                            wkt = cconverter.WKTPolygonWGS84_from_WKTPolygonED50(wktHelp.CreaPoligonoDaCoordinate(lxyz), ruota, ParametriCartografici)
                        Else
                            wkt = wktHelp.CreaPoligonoDaCoordinate(lxyz)
                        End If

                        If parte.LType = "INTERNA" Then

                            gmlHlp.AggiungiInterior(
                                    EntitaElement,
                                    wktToGeoML.get_coordinates(wkt, False, False, True)
                                )

                        Else



                            sNodeDoc = XDocument.Parse(
                                    wktToGeoML.Trasforma(
                                        wkt,
                                        False,
                                        trasformaSistemaRiferimento,
                                        trasformaSistemaRiferimento,
                                        lElementoGrafico_cod
                                    )
                                )

                            sNodeDoc.Root.@Flag_GPS = 0

                            EntitaElement.Add(sNodeDoc.FirstNode)
                            xDocRval.Root.Add(EntitaElement)

                        End If


                    End If

                    iteration += 1
                    If iteration = stopME And stopME <> -1 Then
                        Exit For
                    End If

                End If
                'solo particelle selezionate da filtro
            End If


        Next 'shape In _loadedShapefile.Records

        Dim ListaNS As New List(Of String)
        ListaNS.Add("http://www.opengis.net/gml")

        rval = xmlHelper.RemoveNamespace(xDocRval, ListaNS).ToString.Replace("xmlns=""""", "")


        tmpDoc = XDocument.Parse(rval)
    End Sub

    Private Function GetDatoParticellaDaNomeFile(p1 As String, p2 As String, ByRef CodBelfiore As DecodeComuniIstatCodBelfiore_controller, ByVal configurazioneImportazione As ConfigurazioneImportazione) As String

        Dim rval As String = ""

        Dim ColNameCodBelfiore As String = Nothing
        Dim ColNameIstatP As String = Nothing
        Dim ColNameIstatC As String = Nothing
        Dim ColNameIdSezC As String = Nothing
        Dim ColNameFoglio As String = Nothing
        Dim ColNameParticella As String = Nothing
        Dim ColNameSub As String = Nothing

        ConfigurazioneImportazioneController.recuperaChiaviConfigCatasto(configurazioneImportazione, ColNameCodBelfiore, ColNameIstatP, ColNameIstatC, ColNameIdSezC, ColNameFoglio, ColNameParticella, ColNameSub)


        Dim lP2ToUpperReplace As String = p2.ToUpper.Replace(".DXF", "")
        If lP2ToUpperReplace.Length = "CCCCZFFFFAS".Length Then
            Dim lBelfiore As String = lP2ToUpperReplace.Substring(0, 4)


            Select Case p1
                Case ColNameIstatP
                    Dim prov As String
                    Dim com As String
                    CodBelfiore.LeggiDecodeBelfioreDaDB(lBelfiore, prov, com)

                    rval = prov

                Case ColNameIstatC
                    Dim prov As String
                    Dim com As String
                    CodBelfiore.LeggiDecodeBelfioreDaDB(lBelfiore, prov, com)

                    rval = com

                Case ColNameIdSezC
                    rval = lP2ToUpperReplace.Substring(4, 1)
                    If rval = "_" Then
                        rval = ""
                    End If
                Case ColNameFoglio
                    rval = lP2ToUpperReplace.Substring(5, 4)
                Case ColNameParticella

                Case ColNameSub

            End Select
        End If



        Return rval
    End Function



    Private Sub G2G_Chiusura_Transazione(
                                ByVal Flag_Commit1_Rollback2 As Integer
                                )

        Dim NomeRoutine As String = "G2G_Chiusura_Transazione"

        Try
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(Flag_Commit1_Rollback2, _objParametri_Server)

        Catch ex As Exception
        End Try

    End Sub

    Private Function getdescrizioneParticella(lIstatP As String, listatc As String, lid_sezc As String, lFoglio As String, lParticella As String, lSub As String) As String
        Return "Prov: " & lIstatP & ", Com: " & listatc & ", sezione: " & lid_sezc & ", Foglio: " & lFoglio & ", Numero Particella: " & lParticella & ", Subalterno: " & lSub
    End Function


End Class
