

Imports <xmlns="http://www.agronica.it/grafica/">


Imports System.Configuration.ConfigurationManager
Imports AgronicaGIS2012.Commons.DataOraHelper
Imports AgronicaConversioneCartografiaGias.FormatsConverter
Imports AgronicaGIS2012.Commons
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreUtility.DataOra
Imports System.Text

Public Class iMotion_ToAgronicaGIS2012
    Implements xxx_toAgronicaGIS2012

    Private _objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri



    Public Function AggiornaPosizioneCorrente(
        ByVal vehicle_id As Integer,
        ByVal vehicle_Name As String,
        ByVal vehicle_plate As String,
        ByVal timestamp As Integer,
        ByVal Latitudine As String,
        ByVal Longitudine As String,
        ByVal altitudine As Decimal,
        ByVal speed As Decimal,
        ByVal heading As Decimal,
        ByVal Layer_Cod As Integer,
        ByVal Entita_cod As Integer,
        ByVal ElementoGrafico_cod As Integer,
        ByVal objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As RispostaStandard

        Dim rvalSTD As New RispostaStandard

        _objParametri_Server = objParametriServer



        Dim xDocRval As XDocument = XDocument.Parse("<DatiEntita xmlns=""http://www.agronica.it/grafica/"" xmlns:gml=""http://www.opengis.net/gml""></DatiEntita>")
        Dim lay = <layersdescrizioni>
                      <layer tipologia_layer="1" nome_layer="entità">
                      </layer>
                  </layersdescrizioni>

        xDocRval.Root.Add(lay)

        Dim iFromTime As Integer = timestamp
        Dim iToTime As Integer = timestamp

        Dim wktToGeoML As New wkt_gml

        wktToGeoML.Soglia_ConsideraPuntiUguali = 0.000000005

        Dim wkt As String = "POINT ((" & Latitudine & " " & Longitudine & "))"

        wkt = GeneraEntitaGis(
                    Entita_cod,
                    ElementoGrafico_cod,
                    vehicle_id,
                    vehicle_Name,
                    vehicle_plate,
                    objParametriServer.PivaSuperUser,
                    Layer_Cod,
                    xDocRval,
                    wktToGeoML,
                    iFromTime,
                    iToTime,
                    wkt,
                    AgronicaCoreDataProvider.TipiEnumerativi.enum_iMotion_tipoPercorsoGIS.UltimaPosizione
                  )


        'scrittura ..

        Dim ListaNS As New List(Of String)
        ListaNS.Add("http://www.opengis.net/gml")

        Dim rval As String = ""
        rval = xmlHelper.RemoveNamespace(xDocRval, ListaNS).ToString.Replace("xmlns=""""", "")

        Dim ScriviElementiGrafici As New AgronicaCoreGisBIZ.GIS_Entita_W


        Dim tmpDoc As XDocument = XDocument.Parse(rval)


        'scrive i layer (verificare)

        'scrive le entità
        Dim listOFEntita = (
                    From a In tmpDoc.<DatiEntita>.<Entita>
                    Select a).ToList()


        Dim conteggioImportati As Integer = 0
        Dim conteggioNonImportati As Integer = 0




        Dim messaggioErrore As String = ""

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False


        For Each elemento In listOFEntita


            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Apro la connessione al DB e la transazione
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                    FlagTransazioneLocale,
                                                                    _objParametri_Server)


            'per test formato database ...
            Try

                'transizione sulla singola chiamata di un entità grafica/GIAS

                Dim idle As Integer
                ScriviElementiGrafici.scrivi(elemento.ToString, idle, _objParametri_Server)


                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                'Chiudo la connessione al DB
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, _objParametri_Server)
                '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

                conteggioImportati += 1
            Catch ex As Exception

                conteggioNonImportati += 1

                'Faccio il rollback della transazione
                If Not _objParametri_Server.objTransazione Is Nothing Then
                    'objParametri.objTransazione.Rollback()
                    'objParametri.objTransazione = Nothing
                    AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, _objParametri_Server)

                End If

                rvalSTD.Errore &= AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True) & vbCrLf
                rvalSTD.RispostaOK = False

                'If Not My.Computer.FileSystem.GetFileInfo(NonImportatiSQL).IsReadOnly Then
                '    My.Computer.FileSystem.WriteAllText(NonImportatiSQL, head & elemento.ToString & tail, True)
                'End If

            Finally

                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, _objParametri_Server)


            End Try


        Next

        Return rvalSTD


    End Function


    Public Function convert(
        ByVal ImportaPuntiDiSosta As Boolean,
        ByVal ImportaTragitti As Boolean,
        ByVal SecondiOffsetPuntiInPercorsi As Integer,
        ByVal DurataMinimaSostaConsiderataComeRaccolta As Integer,
        ByVal vehicle_id As Integer,
        ByVal vehicle_Name As String,
        ByVal vehicle_Plate As String,
        ByVal DataRiferimento As DateTime,
        ByVal jSon_Imotion As String,
        PivaSuperUSer As String,
        Layer_cod As Integer,
        objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
        ByRef conteggioNonImportati As Integer,
        ByRef conteggioImportati As Integer
    ) As RispostaStandard

        _objParametri_Server = objParametriServer

        Dim rvalSTD As New RispostaStandard
        rvalSTD.RispostaOK = True
        rvalSTD.RispostaStringa = "Importazione su GIS effettuata. " & vbCrLf

        Dim xDocRval As XDocument = XDocument.Parse("<DatiEntita xmlns=""http://www.agronica.it/grafica/"" xmlns:gml=""http://www.opengis.net/gml""></DatiEntita>")
        Dim lay = <layersdescrizioni>
                      <layer tipologia_layer="1" nome_layer="entità">
                      </layer>
                  </layersdescrizioni>

        xDocRval.Root.Add(lay)


        Dim wktHelp As New WKT
        Dim wktToGeoML As New wkt_gml
        Dim Soglia_ConsideraPuntiUguali As Double = 0.000000005

        Dim stopME As Integer = -1
        If Debugger.IsAttached Then
            stopME = -1
        End If

        Dim iteration As Integer = 0

        Dim gmlHlp As New GML

        'ciclare su ciascun track, salvo percorsi come un unico linestring oppure salvo i punti di stop..
        Dim obV As JObject = JObject.Parse(jSon_Imotion)

        Dim wkt As String
        Dim wktSTB As New StringBuilder


        '-----
        ' Track
        '-----

        If ImportaTragitti Then

            Dim listatracks = (
                       From s In obV("tracks")
                       Select s).ToList



            For Each cTrack In listatracks

                wktSTB.Length = 0
                wktSTB.Clear()


                wkt = "LINESTRING (("

                Dim cTrack_pos = cTrack("pos")
                Dim CURcTrackPos As Integer = 0
                Dim ctrackPos_count As Integer = cTrack_pos.Count

                Dim prec_tStamp As Integer = 0
                For Each cTrackPos In cTrack_pos

                    Dim tStamp As Integer = CType(cTrackPos, JProperty).Name
                    CURcTrackPos += 1

                    If CURcTrackPos = ctrackPos_count Then
                        SecondiOffsetPuntiInPercorsi = 0
                    End If

                    If SecondiOffsetPuntiInPercorsi = 0 OrElse (tStamp - prec_tStamp > SecondiOffsetPuntiInPercorsi) Then

                        prec_tStamp = tStamp

                        Dim cLat As String = cTrackPos.Children()(0).First()
                        Dim cLong As String = cTrackPos.Children()(1).First()

                        wktSTB.Append(cLat)
                        wktSTB.Append(" ")
                        wktSTB.Append(cLong)



                        If CURcTrackPos <> ctrackPos_count Then
                            wktSTB.Append(",")
                        End If

                    End If



                Next

                wkt &= wktSTB.ToString

                wkt &= "))"


                'scrivo il mio linestring...
                Dim EntitaElement As XElement
                Dim DatiPF As String = ""

                Dim iFromTime As Integer = cTrack("from_time")
                Dim iToTime As Integer = cTrack("to_time")

                DatiPF &= "|vehicle_id§ " & vehicle_id
                DatiPF &= "|vehicle_plate§ " & vehicle_Plate
                DatiPF &= "|vehicle_name§ " & vehicle_Name
                DatiPF &= "|from_time§ " & UnixTimeStampToDateTime(iFromTime).ToString
                DatiPF &= "|to_time§ " & UnixTimeStampToDateTime(iToTime).ToString
                DatiPF &= "|isStop§ 0"

                EntitaElement = <Entita TipoOperazioneDB="1" ecolor="256" eline="256" rad="15" text=<%= DatiPF %> validita_inizio="01/01/1900" validita_fine="31/12/2100" username_creazione="" username_modifica="">
                                    <layers>
                                        <layer tipologia_layer="1"><%= Layer_cod %></layer>
                                    </layers>
                                    <EntitaGIAS>
                                        <DatoGias>
                                            <PivaSuperUser><%= PivaSuperUSer %></PivaSuperUser>
                                            <Entita_Cod>0</Entita_Cod>
                                            <TipoEntita_Cod>0</TipoEntita_Cod>
                                            <Piva></Piva>
                                            <Sa_Cod>0</Sa_Cod>
                                            <Appezza>0</Appezza>
                                            <Campo_Cod>0</Campo_Cod>
                                            <Id_Imp>0</Id_Imp>
                                            <PROV>0</PROV>
                                            <COM>0</COM>
                                            <SEZIONE>-1</SEZIONE>
                                            <FOGLIO>-1</FOGLIO>
                                            <NUMERO>-1</NUMERO>
                                            <SUBALTERNO>-1</SUBALTERNO>
                                            <Programmazione_Entita_Cod>0</Programmazione_Entita_Cod>
                                            <Programmazione_Cod>0</Programmazione_Cod>
                                            <Id_Agenda>0</Id_Agenda>
                                            <Ricetta_Operazione_Cod>0</Ricetta_Operazione_Cod>
                                            <analisi_campione_cod>0</analisi_campione_cod>
                                            <OLDGrafica_ID></OLDGrafica_ID>
                                            <inviato>0</inviato>
                                            <Data_Creazione><%= CreateISO8601DateTimeFromSystemDateTime(Now) %></Data_Creazione>
                                            <Data_Modifica><%= CreateISO8601DateTimeFromSystemDateTime(Now) %></Data_Modifica>
                                            <Username_Creazione>agronica</Username_Creazione>
                                            <Username_Modifica>agronica</Username_Modifica>
                                            <Validita_Inizio>1900-01-01T00:00:00</Validita_Inizio>
                                            <Validita_Fine>2100-12-31T00:00:00</Validita_Fine>
                                        </DatoGias>
                                    </EntitaGIAS>
                                </Entita>


                Dim sNodeDoc As XDocument
                sNodeDoc = XDocument.Parse(
                    wktToGeoML.Trasforma(
                        wkt,
                        False,
                        False,
                        False,
                        0
                    )
                )

                sNodeDoc.Root.@Flag_GPS = 1

                EntitaElement.Add(sNodeDoc.FirstNode)
                xDocRval.Root.Add(EntitaElement)


            Next

        End If 'importaTragitti = true




        '-----
        ' STOP
        '-----
        If ImportaPuntiDiSosta Then


            Dim listaStop = (
                From s In obV("stops")
                Select s).ToList

            For Each s1 In listaStop

                Dim iFromTime As Integer = s1("from_time")
                Dim iToTime As Integer = s1("to_time")
                Dim mLat As String = s1("lat")
                Dim mLong As String = s1("lon")
                'Dim Punto As String = 

                If DurataMinimaSostaConsiderataComeRaccolta = 0 OrElse ((iToTime - iFromTime) > DurataMinimaSostaConsiderataComeRaccolta) Then

                    wkt = "POINT ((" & mLat & " " & mLong & "))"

                    wkt = GeneraEntitaGis(
                        0,
                        0,
                        vehicle_id,
                        vehicle_Name,
                        vehicle_Plate,
                        PivaSuperUSer,
                        Layer_cod,
                        xDocRval,
                        wktToGeoML,
                        iFromTime,
                        iToTime,
                        wkt,
                        AgronicaCoreDataProvider.TipiEnumerativi.enum_iMotion_tipoPercorsoGIS.PuntoDiStop
                      )

                End If


            Next 'Punto di stop
        End If 'ImportaPuntiDiSosta = true


        'scrittura ..

        Dim ListaNS As New List(Of String)
        ListaNS.Add("http://www.opengis.net/gml")

        Dim rval As String = ""
        rval = xmlHelper.RemoveNamespace(xDocRval, ListaNS).ToString.Replace("xmlns=""""", "")

        Dim ScriviElementiGrafici As New AgronicaCoreGisBIZ.GIS_Entita_W


        Dim tmpDoc As XDocument = XDocument.Parse(rval)

        'scrive i layer (verificare)

        'scrive le entità
        Dim listOFEntita = (
                    From a In tmpDoc.<DatiEntita>.<Entita>
                    Select a).ToList()



        For Each elemento In listOFEntita

            'per test formato database ...
            Try

                'transizione sulla singola chiamata di un entità grafica/GIAS

                Dim idle As Integer
                AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, _objParametri_Server)
                ScriviElementiGrafici.scrivi(elemento.ToString, idle, _objParametri_Server)


                G2G_Chiusura_Transazione(1)
                conteggioImportati += 1
            Catch ex As Exception

                conteggioNonImportati += 1
                G2G_Chiusura_Transazione(2)

                rvalSTD.Errore &= AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True) & vbCrLf
                rvalSTD.RispostaOK = False

                'If Not My.Computer.FileSystem.GetFileInfo(NonImportatiSQL).IsReadOnly Then
                '    My.Computer.FileSystem.WriteAllText(NonImportatiSQL, head & elemento.ToString & tail, True)
                'End If


            End Try


        Next

        rvalSTD.RispostaStringa &=
            " N.Elementi importati = " & conteggioImportati & vbCrLf &
            " N.Elementi NON importati = " & conteggioNonImportati & vbCrLf

        Return rvalSTD

    End Function

    Private Shared Function GeneraEntitaGis(Entita_Cod As Integer, ElementoGrafico_Cod As Integer, vehicle_id As Integer, vehicle_Name As String, vehicle_Plate As String, PivaSuperUSer As String, Layer_cod As Integer, xDocRval As XDocument, wktToGeoML As wkt_gml, iFromTime As Integer, iToTime As Integer, wktDati As String, ByVal isStop As Integer) As String


        Dim tipoOperazioneDB As Integer

        If Entita_Cod = 0 Then
            tipoOperazioneDB = AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura
        Else
            tipoOperazioneDB = AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Modifica
        End If

        Dim EntitaElement As XElement
        Dim DatiPF As String = ""

        Dim vehicleId = "vehicle_id"
        Dim vehiclePlate = "vehicle_plate"
        Dim vehicleName = "vehicle_name"
        Dim fromTime = "from_time"
        Dim toTime = "to_time"
        Dim isStopLab = "isStop"

        DatiPF &= vehicleId & "|§ " & vehicle_id
        DatiPF &= vehiclePlate & "|" & "§ " & vehicle_Plate
        DatiPF &= vehicleName & "|" & "§ " & vehicle_Name
        DatiPF &= fromTime & "|§ " & UnixTimeStampToDateTime(iFromTime).ToString
        DatiPF &= toTime & "|" & "§ " & UnixTimeStampToDateTime(iToTime).ToString
        DatiPF &= isStopLab & "|§ " & isStop

        EntitaElement = <Entita TipoOperazioneDB=<%= tipoOperazioneDB %> ecolor="256" eline="256" rad="15" text=<%= DatiPF %> validita_inizio="01/01/1900" validita_fine="31/12/2100" username_creazione="" username_modifica="">
                            <layers>
                                <layer tipologia_layer="1"><%= Layer_cod %></layer>
                            </layers>
                            <EntitaGIAS>
                                <DatoGias>
                                    <PivaSuperUser><%= PivaSuperUSer %></PivaSuperUser>
                                    <Entita_Cod><%= Entita_Cod %></Entita_Cod>
                                    <TipoEntita_Cod>0</TipoEntita_Cod>
                                    <Piva></Piva>
                                    <Sa_Cod>0</Sa_Cod>
                                    <Appezza>0</Appezza>
                                    <Campo_Cod>0</Campo_Cod>
                                    <Id_Imp>0</Id_Imp>
                                    <PROV>0</PROV>
                                    <COM>0</COM>
                                    <SEZIONE>-1</SEZIONE>
                                    <FOGLIO>-1</FOGLIO>
                                    <NUMERO>-1</NUMERO>
                                    <SUBALTERNO>-1</SUBALTERNO>
                                    <Programmazione_Entita_Cod>0</Programmazione_Entita_Cod>
                                    <Programmazione_Cod>0</Programmazione_Cod>
                                    <Id_Agenda>0</Id_Agenda>
                                    <id_mov_det>0</id_mov_det>
                                    <Ricetta_Operazione_Cod>0</Ricetta_Operazione_Cod>
                                    <analisi_campione_cod>0</analisi_campione_cod>
                                    <OLDGrafica_ID></OLDGrafica_ID>
                                    <inviato>0</inviato>
                                    <Data_Creazione><%= CreateISO8601DateTimeFromSystemDateTime(Now) %></Data_Creazione>
                                    <Data_Modifica><%= CreateISO8601DateTimeFromSystemDateTime(Now) %></Data_Modifica>
                                    <Username_Creazione>agronica</Username_Creazione>
                                    <Username_Modifica>agronica</Username_Modifica>
                                    <Validita_Inizio>1900-01-01T00:00:00</Validita_Inizio>
                                    <Validita_Fine>2100-12-31T00:00:00</Validita_Fine>
                                </DatoGias>
                            </EntitaGIAS>
                        </Entita>


        Dim sNodeDoc As XDocument
        sNodeDoc = XDocument.Parse(
            wktToGeoML.Trasforma(
                wktDati,
                False,
                False,
                False,
                0
            )
        )

        sNodeDoc.Root.@Flag_GPS = 1

        If ElementoGrafico_Cod <> 0 Then
            sNodeDoc.Root.@ElementoGrafico_Cod = ElementoGrafico_Cod
        End If

        EntitaElement.Add(sNodeDoc.FirstNode)
        xDocRval.Root.Add(EntitaElement)
        Return wktDati
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


    Public Function convert(configurazioneImportazione As ConfigurazioneImportazione, objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri, objParametriUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As String Implements xxx_toAgronicaGIS2012.convert

    End Function

End Class




