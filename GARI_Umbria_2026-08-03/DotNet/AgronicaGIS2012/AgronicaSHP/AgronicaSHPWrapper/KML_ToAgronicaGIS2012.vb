Imports <xmlns="http://www.agronica.it/grafica/">


Imports System.Configuration.ConfigurationManager
Imports AgronicaGIS2012.Commons
Imports AgronicaConversioneCartografiaGias.FormatsConverter
Imports AgronicaConversioneCartografiaGias.Agronica
Imports AgronicaSHPWrapper.TestShapeFile

Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Imports AgronicaSHPWrapper.InterpretaDatiDBF
Imports AgronicaSHPWrapper.InterpretaDatiDBF.InterpretaDatiDBF

Imports AgronicaCoreDataProvider
Imports System.IO


Public Class KML_ToAgronicaGIS2012
    Implements xxx_toAgronicaGIS2012



    Private _objParametri_Server As AgronicaCoreParametri
    Private head As String =
            "<DatiEntita xmlns=""http://www.agronica.it/grafica/"" xmlns:gml=""http://www.opengis.net/gml"">"

    Private tail As String =
        "</DatiEntita>"

    Public Function convert(configurazioneImportazione As ConfigurazioneImportazione, ByVal objParametri_server As AgronicaCoreParametri, ByVal objParametri_Utenti As AgronicaCoreParametri) As String Implements xxx_toAgronicaGIS2012.convert

        _objParametri_Server = objParametri_server

        Dim xDocRval As XDocument = XDocument.Parse("<DatiEntita xmlns=""http://www.agronica.it/grafica/"" xmlns:gml=""http://www.opengis.net/gml""></DatiEntita>")
        Dim lay = <layersdescrizioni>
                      <layer tipologia_layer="1" nome_layer="entità">
                      </layer>
                  </layersdescrizioni>

        xDocRval.Root.Add(lay)


        Dim loadedDXFfile As New KML
        loadedDXFfile.Load(configurazioneImportazione.ShapeFileFullFileName)

        Dim cconverter As New CoordinateConverter
        Dim iteration As Integer = 0
        Dim i As Integer = 0

        Dim rval As String = ""

        Dim wktHelp As New WKT
        Dim wktToGeoML As New wkt_gml
        wktToGeoML.Soglia_ConsideraPuntiUguali = 0.000000005

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

        Dim gmlHlp As New GML

        Dim stopME As Integer = -1
        If Debugger.IsAttached Then
            stopME = -1
        End If
        For Each forma As KmlShape In loadedDXFfile.ListaOggetti

            If forma.Tipo = Enum_kmlShape_tipo.Poligono Then

                Dim inviato As String
                If configurazioneImportazione.TipoImportazioneAgronica = Tipo_Importazione.Tipo_Importazione_ShapeFile.Importazione_Agrea_Crea_Planning Then
                    inviato = "-1"
                Else
                    inviato = "0"
                End If


                Dim lElementoGrafico_cod As Integer = 0
                Dim lTipoOperazioneDB As String = "1"
                Dim lUpdateEntitaCod As Integer = 0


                Dim EntitaElement As XElement

                Dim nome = My.Resources.AgronicaSHPWrapper.Nome
                Dim testo As String = nome & "§ " & forma.Name & "|"

                Dim descrizioneParticella As String = ""
                EntitaElement =
                    <Entita TipoOperazioneDB=<%= lTipoOperazioneDB %> ecolor="256" eline="256" rad="15" text=<%= testo %> validita_inizio="01/01/1900" validita_fine="31/12/2100" username_creazione="" username_modifica="">
                        <layers>
                            <layer tipologia_layer="1"><%= configurazioneImportazione.LayerCod %></layer>
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
                                <PROV></PROV>
                                <COM></COM>
                                <SEZIONE>0</SEZIONE>
                                <FOGLIO>0</FOGLIO>
                                <NUMERO>0</NUMERO>
                                <SUBALTERNO>0</SUBALTERNO>
                                <Programmazione_Entita_Cod>0</Programmazione_Entita_Cod>
                                <Programmazione_Cod>0</Programmazione_Cod>
                                <Id_Agenda>0</Id_Agenda>
                                <id_mov_det>0</id_mov_det>
                                <Ricetta_Operazione_Cod>0</Ricetta_Operazione_Cod>
                                <analisi_campione_cod>0</analisi_campione_cod>
                                <OLDGrafica_ID></OLDGrafica_ID>
                                <inviato>0</inviato>
                                <Data_Creazione><%= DataOraHelper.CreateISO8601DateTimeFromSystemDateTime(Now) %></Data_Creazione>
                                <Data_Modifica><%= DataOraHelper.CreateISO8601DateTimeFromSystemDateTime(Now) %></Data_Modifica>
                                <Username_Creazione>agronica</Username_Creazione>
                                <Username_Modifica>agronica</Username_Modifica>
                                <Validita_Inizio><%= configurazioneImportazione.DataInizioValidita.ToString("s") %></Validita_Inizio>
                                <Validita_Fine><%= configurazioneImportazione.DataFineValidita.ToString("s") %></Validita_Fine>
                            </DatoGias>
                        </EntitaGIAS>
                    </Entita>



                Dim sNodeDoc As XDocument

                Dim ruota As Boolean = False
                Dim boundaryXYZ = forma.OggettoGraficoOuterBoundaryXYZ
                If boundaryXYZ.Count > 0 Then

                    If boundaryXYZ.Count > 2 Then
                        ruota = True
                    End If

                    Dim wkt As String = ""
                    If configurazioneImportazione.TrasformaSistemaRiferimento Then
                        wkt = cconverter.WKTPolygonWGS84_from_WKTPolygonED50(wktHelp.CreaPoligonoDaCoordinate(boundaryXYZ), ruota, ParametriCartografici)
                    Else
                        wkt = wktHelp.CreaPoligonoDaCoordinate(boundaryXYZ)
                    End If

                    sNodeDoc = XDocument.Parse(
                        wktToGeoML.Trasforma(
                            wkt,
                            False,
                            configurazioneImportazione.TrasformaSistemaRiferimento,
                            configurazioneImportazione.TrasformaSistemaRiferimento,
                            lElementoGrafico_cod
                        )
                    )

                    sNodeDoc.Root.@Flag_GPS = 0

                    EntitaElement.Add(sNodeDoc.FirstNode)
                    xDocRval.Root.Add(EntitaElement)

                    iteration += 1
                    If iteration = stopME And stopME <> -1 Then
                        Exit For
                    End If
                End If

            End If

        Next



        Dim ListaNS As New List(Of String)
        ListaNS.Add("http://www.opengis.net/gml")


        rval = xmlHelper.RemoveNamespace(xDocRval, ListaNS).ToString.Replace("xmlns=""""", "")

        Dim ScriviElementiGrafici As New AgronicaCoreGisBIZ.GIS_Entita_W


        Dim tmpDoc As XDocument = XDocument.Parse(rval)

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

        Return ""

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
End Class
