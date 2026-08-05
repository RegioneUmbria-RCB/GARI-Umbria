
Imports AgronicaGIS2012.Commons
Imports AgronicaConversioneCartografiaGias.FormatsConverter
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaConversioneCartografiaGias.Agronica
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreModelloInSviluppo
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class PianoConcimazioneRateoVariabile


    Private Enum IDPulsanti
        CalcoloPianoConcimazioneVariabile = 260413
    End Enum

    Public Sub New()

    End Sub


    ''' <summary>
    ''' Dato un poligono ne calcola il reateo variabile (usando le mappe ad infrarosso)
    ''' </summary>
    ''' <param name="wktPoligono"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CalcolaPianoConcimazioneRateoVariabile(ByVal wktPoligono As String) As String

        Dim rval As String = ""
        Try
            rval = PianoConcimazioneVariabile.Elabora(wktPoligono)
            Return rval
        Catch e As Exception
        End Try

        Return ""

    End Function


    ''' <summary>
    '''  Dato un poligono ne calcola il reateo variabile (usando altre info)
    ''' </summary>
    ''' <param name="DataRiferimentoPerLetturaDatiSentinel">Passare AgroDataInizio per non usare i dati satellitari</param>
    ''' <param name="ChiaveAlbero"></param>
    ''' <param name="cellsize"></param>
    ''' <param name="Codice_Fiscale_Tecnico"></param>
    ''' <param name="objParametri_Server"></param>
    ''' <returns></returns>
    Public Function CalcolaPianoConcimazioneRateoVariabileLocal(
        ByVal DescrizioneDelPiano As String,
        ByVal DataRiferimentoPerLetturaDatiSentinel As Date,
        ByVal ChiaveAlbero As String,
        ByVal cellsize As Integer,
        ByVal Codice_Fiscale_Tecnico As String,
        ByVal passwordSuperUser As String,
        ByRef oCfgLetturaPF As AgronicaCoreModello.PrecisionModel_CFGLetturaDati,
        ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
        ByVal objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) _
    As RispostaStandard


        Dim leggiWKT As New AgronicaCoreGisDAL.GIS_EstrattoreGrafica_R
        Dim leggiEntita As New AgronicaCoreGisDAL.GIS_Entita_R

        Dim entitaCodImpiantoAssociato As Integer = 0



        Dim Piva As String = ""
        Dim Sa_Cod As Integer
        Dim Appezza As Integer
        Dim Id_reg As Integer
        Dim Ricetta_Operazione_Cod As Integer

        Dim rval As New RispostaStandard
        Try
            rval.RispostaOK = True
            rval.RispostaStringa = "Piano creato, è possibile visualizzarne i dettagli aggiornando la visualizzazione GIS."

            Albero.ChiaveAlbero_Decodifica_Ricetta_Operazione_cod_x_json(ChiaveAlbero, Piva, Sa_Cod, Appezza, Id_reg, Ricetta_Operazione_Cod)

            Dim dtAppDatiVerificaRicette As DataTable =
                leggiEntita.Leggi(objParametri_Server.PivaSuperUser, 0, 0, Piva, Sa_Cod, Appezza, 0, Id_reg, "", "", "-1", -1, -1, "-1", 0, Ricetta_Operazione_Cod, 0, 0, Codice_Fiscale_Tecnico, "", Nothing, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server, objParametri_Utenti)

            If dtAppDatiVerificaRicette.Rows.Count > 0 Then
                rval.RispostaStringa = "IMPOSSIBILE PROCEDERE: Esiste già una mappa di prescrizione per la ricetta."
                Return rval
            End If

            Dim llayerDatoGraficoDiPartenza As String = "34"

            Dim pf As New AgronicaCoreGisDAL.PrecisionFarming
            Dim dtAppPlanningDatoImpianto As DataTable =
            pf.LeggiPlanningAssociatoImpiantoDataRicetta(
                Ricetta_Operazione_Cod,
                enumSelezioneVariabile.Selezione_TabellaCompleta,
                "",
                "",
                objParametri_Server
            )

            Dim dtAppDati As DataTable = Nothing
            Dim programmazione_cod As Integer = 0
            If dtAppPlanningDatoImpianto.Rows.Count > 0 Then
                programmazione_cod = dtAppPlanningDatoImpianto.Rows(0)("programmazione_cod")

                If programmazione_cod <> 0 Then
                    dtAppDati = GetDtAppDati(objParametri_Server, objParametri_Utenti, leggiEntita, "", 0, 0, 0, programmazione_cod, llayerDatoGraficoDiPartenza, Codice_Fiscale_Tecnico)
                End If
            End If

            If dtAppDati Is Nothing OrElse dtAppDati.Rows.Count = 0 Then
                llayerDatoGraficoDiPartenza = 19
                dtAppDati = GetDtAppDati(objParametri_Server, objParametri_Utenti, leggiEntita, Piva, Sa_Cod, Appezza, Id_reg, programmazione_cod, llayerDatoGraficoDiPartenza, Codice_Fiscale_Tecnico)

                If dtAppDati.Rows.Count = 0 Then

                    rval.RispostaStringa = "IMPOSSBILE PROCEDERE: Non esiste la cartografia per l'impianto associato alla ricetta."
                    rval.Errore = rval.RispostaStringa
                    Return rval
                End If
            End If

            entitaCodImpiantoAssociato = dtAppDati(0)("entita_cod")

            Dim qtaFertilizzante As Double = 120
            Dim leggiDettagli As New AgronicaCoreContabDAL.Ricette_Dettagli_R
            Dim dtleggiDettagli As DataTable =
            leggiDettagli.Leggi(
                0,
                Ricetta_Operazione_Cod,
                0,
                "",
                0,
                3,
                0,
                0,
                CostantiPersonalizzate.AGRODATAINIZIO,
                CostantiPersonalizzate.AGRODATAFINE,
                enumSelezioneVariabile.Selezione_TabellaCompleta,
                "",
                "",
                objParametri_Server
            )

            If dtleggiDettagli.Rows.Count > 0 Then
                qtaFertilizzante = dtleggiDettagli.Rows(0)("qta")
            End If


            Dim leggiTrasformazione As New AgronicaCoreGisDAL.GIS_SistemiRiferimentoCartografia_R
            Dim wkthelp As New WKT
            Dim cconverter As New CoordinateConverter


            Dim dtLeggiTrasformazioneAppoggio As DataTable = Nothing
            Dim wktPolyEnvelope As String = Nothing

            Dim leggiEnvelope As New AgronicaCoreGisDAL.GIS_ElementiGrafici_R
            wktPolyEnvelope = leggiEnvelope.Poligono_GetEnvelope(entitaCodImpiantoAssociato, 1, objParametri_Server)
            Dim BaricentroPoligono As String = leggiEnvelope.Poligono_GetBaricentro(entitaCodImpiantoAssociato, 1, objParametri_Server, "EnvelopeCenter")

            'ottengo la trasformazione da utilizzare in base a tile sentinel 2
            'TODO, capire come gestire la mancanza di proiezione, per adesso ho impostato le costanti per UTM FUSO 32
            Dim proiezioni As LetturaProiezioniDatoPoligono_Response
            If DataRiferimentoPerLetturaDatiSentinel = CostantiPersonalizzate.AGRODATAINIZIO Then
                proiezioni = New LetturaProiezioniDatoPoligono_Response With {.GEORiferimento_COD = 13, .GEORiferimento_COD_2 = 19}
            Else
                Dim rvalPrj As rispostaStandard(Of LetturaProiezioniDatoPoligono_Response) =
                    SentinelTileOttieniProiezioni(BaricentroPoligono, objParametri_Server, objParametri_Utenti, passwordSuperUser)

                If rvalPrj.RispostaOK AndAlso Not rvalPrj.RispostaStringa.GEORiferimento_COD Is Nothing Then
                    proiezioni = rvalPrj.RispostaStringa
                Else
                    proiezioni = New LetturaProiezioniDatoPoligono_Response With {.GEORiferimento_COD = 13, .GEORiferimento_COD_2 = 19}
                End If
            End If

            dtLeggiTrasformazioneAppoggio = leggiTrasformazione.Leggi(proiezioni.GEORiferimento_COD, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

            Dim ParametriCartografici_WGS84_UTM As New ParametriCoordinateConverter With {
            .CSFromText = dtLeggiTrasformazioneAppoggio(0)("CSFrom"),
                .CStoText = dtLeggiTrasformazioneAppoggio(0)("CSTo"),
                .CStoGeoText = dtLeggiTrasformazioneAppoggio(0)("CStoGeo"),
                .AgronicaLatOffset = dtLeggiTrasformazioneAppoggio(0)("AgronicaLatOffset"),
                .AgronicaLonOffset = dtLeggiTrasformazioneAppoggio(0)("AgronicaLonOffset"),
                .LibreriaDaUsare = dtLeggiTrasformazioneAppoggio(0)("LibreriaDaUsare")
            }


            Dim dOriz As Double = Nothing
            Dim dVert As Double = Nothing
            Dim envelope As String = ""

            Dim dPunto_lbottom_UTM As xyz = Nothing
            LeggiParametriPerEAG_Locale(leggiEnvelope, objParametri_Server, entitaCodImpiantoAssociato, wktPolyEnvelope, wkthelp, dOriz, dVert, ParametriCartografici_WGS84_UTM, cconverter, dPunto_lbottom_UTM)

            Dim idleWKT As String = PoligonoWKT_Esteso(cellsize, dOriz, dVert, dPunto_lbottom_UTM)

            dtLeggiTrasformazioneAppoggio = leggiTrasformazione.Leggi(proiezioni.GEORiferimento_COD_2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

            Dim ParametriCartografici_UTM_WGS84 As New ParametriCoordinateConverter With {
                    .CSFromText = dtLeggiTrasformazioneAppoggio(0)("CSFrom"),
                    .CStoText = dtLeggiTrasformazioneAppoggio(0)("CSTo"),
                    .CStoGeoText = dtLeggiTrasformazioneAppoggio(0)("CStoGeo"),
                    .AgronicaLatOffset = dtLeggiTrasformazioneAppoggio(0)("AgronicaLatOffset"),
                    .AgronicaLonOffset = dtLeggiTrasformazioneAppoggio(0)("AgronicaLonOffset"),
                    .LibreriaDaUsare = dtLeggiTrasformazioneAppoggio(0)("LibreriaDaUsare")
                }

            'genera localmente il rateo variabile oppure legge da API Mappe 2013..:
            Dim DatiPrecision As rispostaStandard(Of RateoPianoVariabileSuPoligono_Response)
            If DataRiferimentoPerLetturaDatiSentinel = CostantiPersonalizzate.AGRODATAINIZIO Then

                DatiPrecision = GeneraEAGLocale(cellsize, qtaFertilizzante, dOriz, dVert, dPunto_lbottom_UTM)
            Else

                wktPolyEnvelope = cconverter.WKTPolygonWGS84_from_WKTPolygonED50(
                        wkthelp.CreaPoligonoDaCoordinate(wkthelp.CreaCoordinateDaPoligono(idleWKT)),
                        False, ParametriCartografici_UTM_WGS84, True)

                DatiPrecision = GeneraEAGDaWsMappe(cellsize, qtaFertilizzante, wktPolyEnvelope, DataRiferimentoPerLetturaDatiSentinel, objParametri_Server, objParametri_Utenti, passwordSuperUser)
            End If

            DatiPrecision.RispostaStringa.GEORiferimento_COD = proiezioni.GEORiferimento_COD_2

            'legge le linee guida per ruotare i poligoni
            Dim leggiOperazioniRicette As New AgronicaCoreGisDAL.PrecisionFarming

            If llayerDatoGraficoDiPartenza = 34 Then
                Appezza = 0
                Id_reg = 0
            End If

            Dim dtleggiOperazioniRicette As DataTable =
            leggiOperazioniRicette.LeggiLineeGuidaAB(objParametri_Server.PivaSuperUser, 0, 55, "", Piva, Sa_Cod, Appezza, 0, Id_reg, "", "", "-1", -1, -1, "-1", 0, programmazione_cod, 0, 0, "-1", 0, CostantiPersonalizzate.AGRODATAINIZIO, CostantiPersonalizzate.AGRODATAFINE, llayerDatoGraficoDiPartenza, Codice_Fiscale_Tecnico, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, " Entita.TipoEntita_Cod = 55 ", "", objParametri_Server)

            Dim curEntita_Cod As Integer
            Dim pPuntiRiferimentoRotazione As New List(Of xyz)
            Dim leggiLineeGuidaGIS As New AgronicaCoreGisDAL.GIS_Entita_R

            If dtleggiOperazioniRicette.Rows.Count > 0 Then
                Dim iContaLineeGuida As Integer = 0
                For Each lineaGuida In dtleggiOperazioniRicette.Rows
                    If iContaLineeGuida < 3 Then
                        curEntita_Cod = lineaGuida("Entita_cod")

                        Dim xmlGIS As String =
                    leggiLineeGuidaGIS.LeggiXML(objParametri_Server.PivaSuperUser, curEntita_Cod, 0, "", "", 0, 0, 0, 0, "", "", "-1", -1, -1, "-1", 0, 0, 0, 0, -1, Codice_Fiscale_Tecnico, "", Nothing, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server, objParametri_Utenti)

                        Dim coord As String() = AgronicaConversioneCartografiaGias.FormatsConverter.GML.DammiArrayCoordinateDatoXml(xmlGIS)
                        If coord.Length = 2 Then
                            Dim lXYZ As New List(Of xyz)
                            lXYZ.Add(
                            New xyz With {
                                .X = xyz.myCDBL(coord(1)),
                                .Y = xyz.myCDBL(coord(0))
                            }
                        )


                            Dim pointRifWKT As String =
                            wkthelp.CreaPoligonoDaCoordinate(lXYZ)

                            Dim dPuntoRifED50 As xyz = wkthelp.CreaCoordinateDaPoligono(
                            cconverter.WKTPolygonWGS84_from_WKTPolygonED50(pointRifWKT, True, ParametriCartografici_WGS84_UTM)
                        )(0)

                            pPuntiRiferimentoRotazione.Add(dPuntoRifED50)

                        End If
                    End If
                    iContaLineeGuida += 1
                Next
            End If


            Dim wktbaricentroPoligonoAppezzamento As String
            Dim app As New AgronicaCoreGisDAL.GIS_ElementiGrafici_R

            wktbaricentroPoligonoAppezzamento =
            app.Poligono_GetBaricentro(entitaCodImpiantoAssociato, 1, objParametri_Server).Replace("POINT (", "").Replace(")", "")

            Dim appwktbaricentroPoligonoAppezzamento As String() = wktbaricentroPoligonoAppezzamento.Split(" ")
            Dim xyzB As New List(Of xyz)
            xyzB.Add(New xyz With {.X = xyz.myCDBL(appwktbaricentroPoligonoAppezzamento(1)), .Y = xyz.myCDBL(appwktbaricentroPoligonoAppezzamento(0))})

            wktbaricentroPoligonoAppezzamento = wkthelp.CreaPoligonoDaCoordinate(xyzB)

            Dim baricentroPoligonoAppezzamento As xyz =
            wkthelp.CreaCoordinateDaPoligono(
                cconverter.WKTPolygonWGS84_from_WKTPolygonED50(
                   wktbaricentroPoligonoAppezzamento,
                    True,
                    ParametriCartografici_WGS84_UTM
                )
            )(0)

            Dim strFodmDdoc As String = ""
            strFodmDdoc = AgronicaFODMhelper.CalcoloPianoConcimazioneVariabile.ESRIAsciiToXml.Convertstring(
                DatiPrecision.RispostaStringa.EsriiAscii.Split(vbCrLf), "Records", "", "", "", "-1", "", wktPolyEnvelope, "", "-1", "", "", False, ParametriCartografici_UTM_WGS84, ParametriCartografici_WGS84_UTM, pPuntiRiferimentoRotazione, baricentroPoligonoAppezzamento)

            Dim agroH As New AgronicaFODMhelper.FODMtoAgronicaGIS2012

            Dim campo_cod As Integer
            Dim regImpianto As Integer


            Dim dtE As DataTable =
            leggiEntita.Leggi(
                objParametri_Server.PivaSuperUser _
                , entitaCodImpiantoAssociato _
                , 0 _
                , "" _
                , 0 _
                , 0 _
                , 0 _
                , 0 _
                , "" _
                , "" _
                , "-1" _
                , -1 _
                , -1 _
                , "-1" _
                , 0 _
                , 0 _
                , 0 _
                , 0 _
                , Codice_Fiscale_Tecnico _
                , "" _
                , Nothing _
                , enumSelezioneVariabile.Selezione_TabellaCompleta _
                , "" _
                , "" _
                , objParametri_Server _
                , objParametri_Utenti
            )

            Dim leggiG As New AgronicaCoreGisDAL.GIS_ElementiGrafici_R
            Dim sPoliGML As String = leggiG.LeggiGML(objParametri_Server.PivaSuperUser, 0, entitaCodImpiantoAssociato, 0, enumFromatoCartograficoConvertito.GML, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)


            Piva = dtE(0)("Piva")
            Sa_Cod = dtE(0)("Sa_cod")
            Appezza = dtE(0)("appezza")
            regImpianto = dtE(0)("id_imp")

            Dim xmlAgronica2012 As String = agroH.GetEntitaGraficaFromFodm(
                sPoliGML _
                , strFodmDdoc _
                , objParametri_Server.PivaSuperUser _
                , Piva _
                , Sa_Cod _
                , Appezza _
                , campo_cod _
                , regImpianto _
                , programmazione_cod _
                , Ricetta_Operazione_Cod _
                , 51 _
                , 51 _
                , ParametriCartografici_UTM_WGS84 _
                , objParametri_Server
            )

            Dim OUTPUT_Allegati_Documenti_Cod As Integer

            'Scrivo le tabelle degli allegati in transazione
            Dim ScriviSuAllegati As New AgronicaCoreAnagrafeBIZ.Allegati_Documenti_W
            Dim rvalScriviAllegati As RispostaStandard = ScriviSuAllegati.PrecisionFarmingScriviSuAllegati(
                enum_CategorieDocumenti.PrecisionFarming_MappaPrescrizione,
                DescrizioneDelPiano,
                "",
                objParametri_Server,
                Piva,
                Sa_Cod,
                Appezza,
                Ricetta_Operazione_Cod,
                regImpianto,
                xmlAgronica2012,
                Nothing,
                "",
                OUTPUT_Allegati_Documenti_Cod
            )

            If rvalScriviAllegati.RispostaOK Then
                If oCfgLetturaPF.ListaCodici_RicetteOperazioniCod_MappePrescrizione Is Nothing Then
                    oCfgLetturaPF.ListaCodici_RicetteOperazioniCod_MappePrescrizione = New List(Of Integer)
                End If
                oCfgLetturaPF.ListaCodici_RicetteOperazioniCod_MappePrescrizione.Add(Ricetta_Operazione_Cod)
            Else
                Throw New Exception("Errore in fase di salvataggio del piano a rateo variabile: " & rvalScriviAllegati.Errore)
            End If


            Dim tmpDoc As XDocument = XDocument.Parse(xmlAgronica2012)
            Dim AgroNS As XNamespace = "http://www.agronica.it/grafica/"
            Dim conteggioElementiScrittiCorretti As Integer = (
            From a In tmpDoc.Elements(AgroNS + "DatiEntita").Elements(AgroNS + "Entita")
            Select a).ToList().Count

            rval.RispostaStringa &= " - Elementi memorizzati: " & conteggioElementiScrittiCorretti & "."

        Catch ex As Exception

            rval.RispostaOK = False
            rval.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            rval.RispostaStringa = "False"

        End Try
        'globale

        Return rval

    End Function



    Private Shared Sub LeggiParametriPerEAG_Locale(ByVal leggiEnvelope As AgronicaCoreGisDAL.GIS_ElementiGrafici_R, ByRef objParametri_Server As AgronicaCoreParametri, entitaCodImpiantoAssociato As Integer, ByRef wktPolyEnvelope As String, ByRef wkthelp As WKT, ByRef dOriz As Double, ByRef dVert As Double, ByRef ParametriCartograficiWGS84ED50 As ParametriCoordinateConverter, ByRef cconverter As CoordinateConverter, ByRef dPunto_lbottom_UTM As xyz)



        Dim pPunti As List(Of xyz)
        pPunti = wkthelp.CreaCoordinateDaPoligono(wktPolyEnvelope)


        Dim ltop As New List(Of xyz)
        ltop.Add(pPunti(1))

        Dim rtop As New List(Of xyz)
        rtop.Add(pPunti(2))

        Dim lbottom As New List(Of xyz)
        lbottom.Add(pPunti(0))

        Dim ltop_Wkt As String = wkthelp.CreaPoligonoDaCoordinate(ltop, True).Replace("((", "(").Replace("))", ")")
        Dim rtop_Wkt As String = wkthelp.CreaPoligonoDaCoordinate(rtop, True).Replace("((", "(").Replace("))", ")")
        Dim lbottom_Wkt As String = wkthelp.CreaPoligonoDaCoordinate(lbottom, True).Replace("((", "(").Replace("))", ")")

        dVert = leggiEnvelope.Poligono_CalcolaDistanza(ltop_Wkt, rtop_Wkt, 1, objParametri_Server)
        dOriz = leggiEnvelope.Poligono_CalcolaDistanza(ltop_Wkt, lbottom_Wkt, 1, objParametri_Server)



        dPunto_lbottom_UTM = wkthelp.CreaCoordinateDaPoligono(
            cconverter.WKTPolygonWGS84_from_WKTPolygonED50(lbottom_Wkt, True, ParametriCartograficiWGS84ED50)
        )(0)

    End Sub


    Private Shared Function SentinelTileOttieniProiezioni(
       PoligonoWKT As String,
        objparametri_Server As AgronicaCoreParametri, objparametri_Utenti As AgronicaCoreParametri,
        passwordSuperUser As String
    ) As rispostaStandard(Of LetturaProiezioniDatoPoligono_Response)

        Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim permessiAnalisiDatiSat As Boolean = ObjUtenti.Controlla_Permessi_Utente(
                                       objparametri_Server.UtenteUsername,
                                       5,
                                       enum_Security_Attivita.GIS_SAT_AnalisiDatiSatellitari,
                                       enum_Security_Operazione.Modifica,
                                       Date.Now, "", objparametri_Utenti)

        Dim r As New rispostaStandard(Of LetturaProiezioniDatoPoligono_Response)
        If Not permessiAnalisiDatiSat Then
            r = New rispostaStandard(Of LetturaProiezioniDatoPoligono_Response)
            r.RispostaStringa = New LetturaProiezioniDatoPoligono_Response
            r.RispostaOK = False
            r.Errore = "Non si dispone del permesso di analisi dati"
            Return r
        End If

        Dim leggiBasePath As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim basePath As String = leggiBasePath.Leggi_Valore(0, "GiasOnline_WS_Mappe_2013", "", "", objparametri_Server)
        basePath &= "MappeApi.svc/LetturaProiezioniDatoPoligono"

        'PoligonoWKT As String, cellsize As Integer, Sensore As String, DataRiferimento As String, QtaDaPonderare
        Dim payload As String = " { ""PoligonoWKT"": """ & PoligonoWKT & """  } "


        Dim user As String = objparametri_Server.SuperUserUsername
        Dim nomeOggettoEmbed As String = "d"

        Dim xUtil As New AgronicaCoreUtility.Http

        Dim xRisp As String =
            xUtil.RestPostBasicAuth(basePath, payload, user, passwordSuperUser, nomeOggettoEmbed)


        r.RispostaStringa = New LetturaProiezioniDatoPoligono_Response

        Dim o As JObject = JObject.Parse(xRisp)
        r.RispostaStringa.GEORiferimento_COD = o("RispostaStringa")("GEORiferimento_COD")
        r.RispostaStringa.GEORiferimento_COD_2 = o("RispostaStringa")("GEORiferimento_COD_2")

        Dim r1 As rispostaStandard(Of RateoPianoVariabileSuPoligono_Response) =
            JsonConvert.DeserializeObject(Of rispostaStandard(Of RateoPianoVariabileSuPoligono_Response))(xRisp)

        Return r

    End Function

    Private Shared Function GeneraEAGDaWsMappe(
        cellsize As Integer, qtaFertilizzante As Double, PoligonoWKT As String, DataRiferimento As String,
        objparametri_Server As AgronicaCoreParametri, objparametri_Utenti As AgronicaCoreParametri,
        passwordSuperUser As String
    ) As rispostaStandard(Of RateoPianoVariabileSuPoligono_Response)

        Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim permessiAnalisiDatiSat As Boolean = ObjUtenti.Controlla_Permessi_Utente(
                                       objparametri_Server.UtenteUsername,
                                       5,
                                       enum_Security_Attivita.GIS_SAT_AnalisiDatiSatellitari,
                                       enum_Security_Operazione.Modifica,
                                       Date.Now, "", objparametri_Utenti)

        Dim r As New rispostaStandard(Of RateoPianoVariabileSuPoligono_Response)
        If Not permessiAnalisiDatiSat Then
            r = New rispostaStandard(Of RateoPianoVariabileSuPoligono_Response)
            r.RispostaStringa = New RateoPianoVariabileSuPoligono_Response
            r.RispostaOK = False
            r.Errore = "Non si dispone del permesso di analisi dati"
            Return r
        End If

        Dim leggiBasePath As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim basePath As String = leggiBasePath.Leggi_Valore(0, "GiasOnline_WS_Mappe_2013", "", "", objparametri_Server)
        basePath &= "MappeApi.svc/RateoPianoVariabileSuPoligono"

        'PoligonoWKT As String, cellsize As Integer, Sensore As String, DataRiferimento As String, QtaDaPonderare
        Dim payload As String = " { ""PoligonoWKT"": """ & PoligonoWKT & """, ""cellsize"": " & cellsize & ", ""Sensore"": ""NDVI"", ""DataRiferimento"": """ & DataRiferimento & """, ""QtaDaPonderare"": " & qtaFertilizzante.ToString.Replace(",", ".") & " } "


        Dim user As String = objparametri_Server.SuperUserUsername
        Dim nomeOggettoEmbed As String = "d"

        Dim xUtil As New AgronicaCoreUtility.Http

        Dim xRisp As String =
            xUtil.RestPostBasicAuth(basePath, payload, user, passwordSuperUser, nomeOggettoEmbed)


        r.RispostaStringa = New RateoPianoVariabileSuPoligono_Response

        Dim o As JObject = JObject.Parse(xRisp)
        r.RispostaStringa.EsriiAscii = o("RispostaStringa")("EsriiAscii")
        r.RispostaStringa.GEORiferimento_COD = o("RispostaStringa")("GEORiferimento_COD")

        Dim r1 As rispostaStandard(Of RateoPianoVariabileSuPoligono_Response) =
            JsonConvert.DeserializeObject(Of rispostaStandard(Of RateoPianoVariabileSuPoligono_Response))(xRisp)

        Return r

    End Function

    Private Shared Function PoligonoWKT_Esteso(cellsize As Integer, dimensioneOriz As Double, dimensioneVert As Double, dPunto_lbottom_UTM As xyz) As String

        'TODO ... 

        Dim punto_RBottom_UTM As New xyz
        Dim punto_Rtop_UTM As New xyz
        Dim punto_Ltop_UTM As New xyz
        Dim punto_LBottom_Utm As New xyz

        RicalcolaCoordinatePuntoLBottom(dimensioneOriz, dimensioneVert, dPunto_lbottom_UTM)


        'StampaPunto(dPunto_lbottom_UTM)

        punto_LBottom_Utm.X = dPunto_lbottom_UTM.X
        punto_LBottom_Utm.Y = dPunto_lbottom_UTM.Y
        'StampaPunto(punto_LBottom_Utm)

        punto_RBottom_UTM.X = dPunto_lbottom_UTM.X + dimensioneVert * 2
        punto_RBottom_UTM.Y = dPunto_lbottom_UTM.Y
        'StampaPunto(punto_RBottom_UTM)

        punto_Rtop_UTM.X = punto_RBottom_UTM.X
        punto_Rtop_UTM.Y = punto_RBottom_UTM.Y + dimensioneOriz * 2
        'StampaPunto(punto_Rtop_UTM)

        punto_Ltop_UTM.X = dPunto_lbottom_UTM.X
        punto_Ltop_UTM.Y = punto_Rtop_UTM.Y
        'StampaPunto(punto_Ltop_UTM)

        Dim rval = "POLYGON ((" &
            punto_LBottom_Utm.X.ToString.Replace(",", ".") & " " & punto_LBottom_Utm.Y.ToString.Replace(",", ".") & "," &
            punto_RBottom_UTM.X.ToString.Replace(",", ".") & " " & punto_RBottom_UTM.Y.ToString.Replace(",", ".") & "," &
            punto_Rtop_UTM.X.ToString.Replace(",", ".") & " " & punto_Rtop_UTM.Y.ToString.Replace(",", ".") & "," &
            punto_Ltop_UTM.X.ToString.Replace(",", ".") & " " & punto_Ltop_UTM.Y.ToString.Replace(",", ".") & "," &
            punto_LBottom_Utm.X.ToString.Replace(",", ".") & " " & punto_LBottom_Utm.Y.ToString.Replace(",", ".") &
         "))"

        'Debug.WriteLine(rval)

        Return rval

    End Function

    Private Shared Function StampaPunto(dPunto_lbottom_UTM As xyz) As String
        Dim m As String = "POINT(" & dPunto_lbottom_UTM.X.ToString().Replace(",", ".") & " " & dPunto_lbottom_UTM.Y.ToString().Replace(",", ".") & ")"
        Debug.WriteLine(m)
    End Function

    Private Shared Function GeneraEAGLocale(cellsize As Integer, qtaFertilizzante As Double, dimensioneOriz As Double, dimensioneVert As Double, dPunto_lbottom_UTM As xyz) As rispostaStandard(Of RateoPianoVariabileSuPoligono_Response)

        Dim eag As New System.Text.StringBuilder

        Dim nCols As Integer = CInt(dimensioneVert / cellsize) * 2
        Dim nRows As Integer = CInt(dimensioneOriz / cellsize) * 2

        RicalcolaCoordinatePuntoLBottom(dimensioneOriz, dimensioneVert, dPunto_lbottom_UTM)

        eag.AppendLine("ncols" & Right("          " & nCols, 10))
        eag.AppendLine("nrows" & Right("          " & nRows, 10))

        eag.AppendLine("xllcorner    " & dPunto_lbottom_UTM.X.ToString.Replace(",", "."))
        eag.AppendLine("yllcorner    " & dPunto_lbottom_UTM.Y.ToString.Replace(",", "."))
        eag.AppendLine("cellsize     " & cellsize)

        For i As Integer = 0 To nRows
            For j As Integer = 0 To nCols
                eag.Append(" " & qtaFertilizzante)
            Next
            eag.Append(vbCrLf)
        Next


        'ncols 37
        'nrows 22
        'xllcorner    750788.289912345
        'yllcorner    4933115.18329868
        'cellsize 10
        ' 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 124.905005802684 0 0 0      

        Return New rispostaStandard(Of RateoPianoVariabileSuPoligono_Response) With {
            .RispostaOK = True,
            .RispostaStringa = New RateoPianoVariabileSuPoligono_Response With {
                .EsriiAscii = eag.ToString
            }
        }

    End Function

    Private Shared Sub RicalcolaCoordinatePuntoLBottom(dOriz As Double, dVert As Double, dPunto_lbottom_UTM As xyz)
        dPunto_lbottom_UTM.X -= CInt(dVert / 2)
        dPunto_lbottom_UTM.Y -= CInt(dOriz / 2)
    End Sub

    Private Shared Function GetDtAppDati(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri, ByVal leggiEntita As AgronicaCoreGisDAL.GIS_Entita_R, ByVal Piva As String, ByVal Sa_Cod As Integer, ByVal Appezza As Integer, ByVal Id_reg As Integer, ByVal Programmazione_cod As Integer, ByVal llayerDatoGraficoDiPartenza As String, ByVal Codice_Fiscale_Tecnico As String) As DataTable
        Dim dtAppDati As DataTable =
                        leggiEntita.Leggi(objParametri_Server.PivaSuperUser, 0, 0, Piva, Sa_Cod, Appezza, 0, Id_reg, "", "", "-1", -1, -1, "-1", 0, 0, Programmazione_cod, 0, Codice_Fiscale_Tecnico, "", Nothing, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, " Entita.TipoEntita_Cod in (select tipoEntita_cod from gis_tipoentita where layerElementiGrafici_cod = " & llayerDatoGraficoDiPartenza & " )", "", objParametri_Server, objParametri_Utenti)
        Return dtAppDati
    End Function



    Private Shared Sub EspandiDistanza(ByRef pPunti As List(Of xyz), ByVal i As Integer, ByVal X_DistanzalTopRtop As Double, ByVal Y_DistanzalTopLBotom As Double)
        pPunti(i).X += X_DistanzalTopRtop
        pPunti(i).Y += Y_DistanzalTopLBotom
    End Sub



    Private Sub G2G_Chiusura_Transazione(
                            ByVal Flag_Commit1_Rollback2 As Integer _
                            , ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri
                            )

        Dim NomeRoutine As String = "G2G_Chiusura_Transazione"

        Try
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(Flag_Commit1_Rollback2, objParametri_server)

        Catch ex As Exception
        End Try

    End Sub

End Class
