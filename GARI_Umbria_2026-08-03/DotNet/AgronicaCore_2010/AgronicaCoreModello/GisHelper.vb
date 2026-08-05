Imports AgronicaConversioneCartografiaGias
Imports AgronicaConversioneCartografiaGias.Agronica
Imports AgronicaConversioneCartografiaGias.FormatsConverter
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreVarieBIZ
Imports AgronicaGIS2012.Commons

Public Class GisHelper


    ''' <summary>
    ''' Scrive un dato cartografico in formato WKT, associando a quanto passato nei vari pararametri (piva, ecc..), il dato può essere trasformato se si passa il parametro wkt_geoRiferimento_Cod
    ''' </summary>
    ''' <param name="objParametri_Server"></param>
    ''' <param name="Programmazione_Entita_Cod"></param>
    ''' <param name="Programmazione_Cod"></param>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="Appezza"></param>
    ''' <param name="id_Reg"></param>
    ''' <param name="id_agenda"></param>
    ''' <param name="id_mov_det"></param>
    ''' <param name="wkt"></param>
    ''' <param name="wkt_georiferimento_cod">codice da tabella GIS_SistemiRiferimentoCartografia</param>
    ''' <param name="FlagGPS">Indicare se il dato proviene da GPS (1) oppure no (0)</param>
    ''' <param name="entita_cod">Deve essere 0 se si vuole creare un elemento nuovo</param>
    ''' <param name="elementoGrafico_Cod">Deve essere 0 se si vuole creare un elemento nuovo</param>
    ''' <param name="TipoOperazioneDB"></param>
    ''' <returns></returns>
    Public Function ScriviDatoCartografico(
        ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
        ByVal Programmazione_Entita_Cod As Integer,
        ByVal Programmazione_Cod As Integer,
        ByVal Piva As String,
        ByVal Sa_Cod As Integer,
        ByVal Appezza As Integer,
        ByVal id_Reg As Integer,
        ByVal id_agenda As Integer,
        ByVal id_mov_det As Integer,
        ByVal wkt As String,
        ByVal wkt_georiferimento_cod As String,
        ByVal FlagGPS As Integer,
        ByVal layerCod As Integer,
        ByVal TipoEntita_cod As Integer,
        ByVal entita_cod As Integer,
        ByVal elementoGrafico_Cod As Integer,
        ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
        Optional ByVal ElementoGrafico_Des As String = "",
        Optional ByVal swapLatLong As Boolean = False
    ) As RispostaStandard

        Dim rVal As New RispostaStandard
        rVal.RispostaOK = True

        Dim tipoOp As Integer = TipoOperazioneDB

        If TipoEntita_cod <= 0 OrElse layerCod <= 0 Then
            rVal.Errore &= "Non è stato passato il TipoEntita_cod o il layerCod"
            rVal.RispostaOK = False
            Return rVal
        End If


        'non è agenda
        'If id_agenda = 0 Then

        '    Dim LetturaEntitaCod As New AgronicaCoreGisDAL.GIS_Entita_R

        '    If Appezza <> 0 Then
        '        TipoEntita_cod = LetturaEntitaCod.TipoEntita_Cod_Leggi_Da_Impianto(Piva, Sa_Cod, Appezza, id_Reg, objParametri_Server)
        '    End If

        '    If Programmazione_Cod <> 0 Then
        '        TipoEntita_cod = enum_GIS2012_TipoEntita.PLANNING_IMPIANTI_PIANIFICATI
        '    End If

        '    If Programmazione_Cod <> 0 Then
        '        layerCod = enum_Gis_LayerElementiGrafici_std.Impianti_Pianificati_Entita
        '    Else
        '        layerCod = enum_Gis_LayerElementiGrafici_std.IMPIANTI
        '    End If

        'Else

        '    'Agenda, destinazione oppure no..?
        '    If id_mov_det <> 0 Then

        '        layerCod = enum_Gis_LayerElementiGrafici_std.AGENDA_RILIEVI
        '        TipoEntita_cod = enum_GIS2012_TipoEntita.Destinazione_Agenda

        '    Else

        '        layerCod = enum_Gis_LayerElementiGrafici_std.Op_Agenda
        '        TipoEntita_cod = enum_GIS2012_TipoEntita.OpAgenda

        '    End If

        'End If

        Dim ScriviElementiGrafici As New AgronicaCoreGisBIZ.GIS_Entita_W

        Dim ParametriCartografici As ParametriCoordinateConverter = Nothing

        If wkt_georiferimento_cod <> "-1" Then


            Dim leggiTrasformazione As New AgronicaCoreGisDAL.GIS_SistemiRiferimentoCartografia_R
            Dim dtLeggiTrasformazione As DataTable = leggiTrasformazione.Leggi(wkt_georiferimento_cod, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)


            ParametriCartografici = New ParametriCoordinateConverter With {
                .CSFromText = dtLeggiTrasformazione(0)("CSFrom"),
                .CStoText = dtLeggiTrasformazione(0)("CSTo"),
                .CStoGeoText = dtLeggiTrasformazione(0)("CStoGeo"),
                .AgronicaLatOffset = dtLeggiTrasformazione(0)("AgronicaLatOffset"),
                .AgronicaLonOffset = dtLeggiTrasformazione(0)("AgronicaLonOffset"),
                .LibreriaDaUsare = dtLeggiTrasformazione(0)("LibreriaDaUsare")
            }


        End If

        Dim FinalDoc As New XDocument

        Dim sNodeDoc As XDocument

        Dim wktHelp As New WKT
        Dim wktToGeoML As New wkt_gml
        Dim cconverter As New Agronica.CoordinateConverter

        Dim ElemFinalXdoc As XElement = <DatiEntita xmlns="http://www.agronica.it/grafica/"
                                            xmlns:gml="http://www.opengis.net/gml"></DatiEntita>

        FinalDoc.Add(ElemFinalXdoc)

        If TipoOperazioneDB = enum_TipoOperazioneDB.Cancellazione Then
            wkt = "POINT (12 44)"
        End If

        If ParametriCartografici Is Nothing Then

            Dim DatiCartograficiOriginali_WGS84 As List(Of xyz) =
                    wktHelp.CreaCoordinateDaWkt(wkt, swapLatLong)

            sNodeDoc = ReadXmlFromString(
                    wktToGeoML.Trasforma(
                        wktHelp.CreaPoligonoDaCoordinate(
                            DatiCartograficiOriginali_WGS84,
                            (DatiCartograficiOriginali_WGS84.Count = 1)
                        ),
                        False,
                        False,
                        True,
                        0)
                )

        Else



            sNodeDoc = ReadXmlFromString(
                    wktToGeoML.Trasforma(
                        cconverter.WKTPolygonWGS84_from_WKTPolygonED50(wkt, True, ParametriCartografici),
                        False,
                        False,
                        True,
                        0)
                )

        End If



        sNodeDoc.Root.@Flag_GPS = FlagGPS
        sNodeDoc.Root.@text = "test"
        'sNodeDoc.<geodata>.@ElementoGrafico_Cod = entita_cod
        sNodeDoc.Root.@ElementoGrafico_Cod = elementoGrafico_Cod

        Dim newEntitaElement = <Entita TipoOperazioneDB=<%= tipoOp %> recno="" deleted="" section="E" id="1" descr="" ecolor="256" eline="256" rad="15" text=<%= ElementoGrafico_Des %> gps="0" lat="0" lon="0" pdop="0" validita_inizio="01/09/2009" validita_fine="31/08/2010" username_creazione="" username_modifica="">
                                   <layers>
                                       <layer tipologia_layer="1"><%= layerCod %></layer>
                                   </layers>
                                   <EntitaGIAS>
                                       <DatoGias>
                                           <PivaSuperUser><%= objParametri_Server.PivaSuperUser %></PivaSuperUser>
                                           <Entita_Cod><%= entita_cod %></Entita_Cod>
                                           <TipoEntita_Cod><%= TipoEntita_cod %></TipoEntita_Cod>
                                           <Piva><%= Piva %></Piva>
                                           <Sa_Cod><%= Sa_Cod %></Sa_Cod>
                                           <Appezza><%= Appezza %></Appezza>
                                           <Campo_Cod>0</Campo_Cod>
                                           <Id_Imp><%= id_Reg %></Id_Imp>
                                           <PROV>0</PROV>
                                           <COM>0</COM>
                                           <SEZIONE>0</SEZIONE>
                                           <FOGLIO>0</FOGLIO>
                                           <NUMERO>0</NUMERO>
                                           <SUBALTERNO>0</SUBALTERNO>
                                           <Programmazione_Entita_Cod><%= Programmazione_Entita_Cod %></Programmazione_Entita_Cod>
                                           <Programmazione_Cod><%= Programmazione_Cod %></Programmazione_Cod>
                                           <Id_Agenda><%= id_agenda %></Id_Agenda>
                                           <id_mov_det><%= id_mov_det %></id_mov_det>
                                           <Ricetta_Operazione_Cod>0</Ricetta_Operazione_Cod>
                                           <OLDGrafica_ID></OLDGrafica_ID>
                                           <analisi_campione_cod>0</analisi_campione_cod>
                                           <inviato>0</inviato>
                                           <Data_Creazione><%= GetData_Creazione(Now) %></Data_Creazione>
                                           <Data_Modifica><%= GetData_Creazione(Now) %></Data_Modifica>
                                           <Username_Creazione><%= objParametri_Server.UtenteUsername %></Username_Creazione>
                                           <Username_Modifica><%= objParametri_Server.UtenteUsername %></Username_Modifica>
                                           <Validita_Inizio>1900-01-01T00:00:00</Validita_Inizio>
                                           <Validita_Fine>2100-12-31T00:00:00</Validita_Fine>
                                       </DatoGias>
                                   </EntitaGIAS>
                               </Entita>



        newEntitaElement.Add(sNodeDoc.FirstNode)
        FinalDoc.Root.Add(newEntitaElement)

        Dim ns1 As XNamespace = "http://www.agronica.it/grafica/"

        Dim ListaNS As New List(Of String)
        ListaNS.Add("http://www.opengis.net/gml")

        Dim sFinalDoc1 As String = xmlHelper.RemoveNamespace(FinalDoc, ListaNS).ToString.Replace("xmlns=""""", "")
        Dim FinalDoc1 As XDocument = XDocument.Parse(sFinalDoc1)

        For Each elemento In (
            From a In FinalDoc1.Elements(ns1 + "DatiEntita").Elements(ns1 + "Entita")
            Select a).ToList()


            Dim FlagTransazioneLocale As Boolean = False
            Dim FlagConnessioneLocale As Boolean = False

            Try

                '------------------------------
                'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
                AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri_Server)


                Dim idle As Integer
                ScriviElementiGrafici.scrivi(elemento.ToString, idle, objParametri_Server)

                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)


            Catch ex As Exception

                If Not objParametri_Server.objTransazione Is Nothing Then
                    'objParametri.objTransazione.Rollback()
                    'objParametri.objTransazione = Nothing
                    AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
                End If

                'uso questa funzione per ottenere il Messaggio..:
                rVal.Errore &= AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
                rVal.RispostaOK = False


            Finally

                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

            End Try


        Next

        Return rVal

    End Function



    Private Function ReadXmlFromString(ByVal stringaXml As String) As XDocument
        Return XDocument.Parse(stringaXml)
    End Function


    Private Function GetData_Creazione(ByVal dataCreazione As DateTime) As String
        Return AgronicaCoreUtility.DataOra.DataOraToDate_JSON_ISO8601(dataCreazione)
    End Function

End Class
