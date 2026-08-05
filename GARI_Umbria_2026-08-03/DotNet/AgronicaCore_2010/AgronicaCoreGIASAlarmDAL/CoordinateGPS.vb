Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.DataProvider
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports System.Text

Public Class CoordinateGPS_R


    Public Function LeggiUltimoID() As String


        Dim context As New GIASAlarmEntities
        Dim last As Integer = (From a In context.CoordinateGPS _
                     Order By a.ID_Coordinata Descending _
                     Select a.ID_Coordinata).FirstOrDefault



        Return last
    End Function



    Public Function LeggiPercorso(ByVal IMEI As String, ByVal Numero_Max_coordinate As Integer, ByVal SoloUltimo As Boolean) As String
        Dim context As New GIASAlarmEntities
        Dim lista2 As List(Of Coordinate_GPS_Vista_LatLong)




        'If Numero_Max_coordinate <> -1 Then
        lista2 = (From a In context.Coordinate_GPS_Vista_LatLong _
                    Where a.imei = IMEI _
                     Order By a.DataOra Descending _
                     Take Numero_Max_coordinate
                     Select a).ToList()
        'Else
        'lista2 = (From a In context.Coordinate_GPS_Vista_LatLong _
        '         Where a.IMEI = IMEI _
        '    Order By a.DataOra Descending _
        '    Select a).ToList()
        'End If


        Dim lista As New List(Of Coordinate_GPS_Vista_LatLong)
        'raggruppo tutti quelli in modalita 1

        Dim lat_media, long_media, altezza, DOP As Decimal
        Dim numero_media As Integer
        Dim precedente As Integer = -1
        Dim i As Integer
 
        For i = 0 To lista2.Count - 1
            If lista2(i).TipologiaMovimento = precedente Then
                If lista2(i).TipologiaMovimento = 1 Then
                    lat_media = lat_media + lista2(i).Lat
                    long_media = long_media + lista2(i).Long
                    DOP = DOP + lista2(i).DOP
                    altezza = altezza + lista2(i).Altezza
                    numero_media = numero_media + 1
                Else
                    lista.Add(lista2(i))
                    precedente = lista2(i).TipologiaMovimento
                End If
            Else
                If precedente = 1 Then
                    Dim nuovo As New Coordinate_GPS_Vista_LatLong
                    nuovo.ID_Coordinata = lista2(i - 1).ID_Coordinata
                    nuovo.Altezza = (altezza / numero_media)
                    nuovo.DataOra = lista2(i - 1).DataOra
                    nuovo.ID_Coordinata = lista2(i - 1).DOP
                    nuovo.Fix = lista2(i - 1).Fix
                    nuovo.IMEI = lista2(i - 1).IMEI
                    nuovo.Lat = (lat_media / numero_media)
                    nuovo.Long = (long_media / numero_media)
                    nuovo.Speed = 0
                    nuovo.TipologiaMovimento = 1
                    lista.Add(nuovo)

                    lista.Add(lista2(i))
                    precedente = lista2(i).TipologiaMovimento
                Else
                    If lista2(i).TipologiaMovimento = 1 Then
                        lat_media = lista2(i).Lat
                        long_media = lista2(i).Long
                        DOP = lista2(i).DOP
                        altezza = lista2(i).Altezza
                        numero_media = 1
                        precedente = lista2(i).TipologiaMovimento
                    Else
                        lista.Add(lista2(i))
                        precedente = lista2(i).TipologiaMovimento
                    End If
                    
                End If
            End If
        Next





        Dim xdoc As XDocument

        Dim nodo As XElement = <Entita xmlns="http://www.agronica.it/grafica/" TipoOperazioneDB="1" ecolor="256" eline="256" rad="15" text=<%= IMEI %> validita_inizio="01/09/2009" validita_fine="31/08/2010" username_creazione="" username_modifica="" TipoIcona="Trattore">
                                   <layers>
                                       <layer tipologia_layer="1">66</layer>
                                   </layers>
                                   <autorizzazioni>
                                       <gis>
                                           <inserimento>False</inserimento>
                                           <modifica>False</modifica>
                                           <cancellazione>False</cancellazione>
                                       </gis>
                                   </autorizzazioni>
                                   <EntitaGIAS>
                                       <DatoGias>
                                           <PivaSuperUser></PivaSuperUser>
                                           <Entita_Cod>0</Entita_Cod>
                                           <TipoEntita_Cod>66</TipoEntita_Cod>
                                           <Piva>80062590379</Piva>
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
                                           <Programmazione_Entita_Cod>-1</Programmazione_Entita_Cod>
                                           <Programmazione_Cod>-1</Programmazione_Cod>
                                           <ID_Agenda>-1</ID_Agenda>
                                           <Ricetta_Operazione_Cod>0</Ricetta_Operazione_Cod>
                                           <inviato>0</inviato>
                                           <Data_Creazione>2012-03-22T17:49:31.997</Data_Creazione>
                                           <Data_Modifica>2012-03-22T17:49:31.997</Data_Modifica>
                                           <Username_Creazione>agronica</Username_Creazione>
                                           <Username_Modifica>agronica</Username_Modifica>
                                           <Validita_Inizio>1900-01-01T00:00:00</Validita_Inizio>
                                           <Validita_Fine>2100-12-31T00:00:00</Validita_Fine>
                                       </DatoGias>
                                   </EntitaGIAS>
                                   <geodata Flag_GPS="0" ElementoGrafico_Cod="0">
                                   </geodata>
                               </Entita>

        Dim myXPath As XNamespace = "http://www.opengis.net/gml"


        Dim nodoGml As XElement

        If SoloUltimo = True Then
            nodoGml =
             <gml:Point xmlns:gml="http://www.opengis.net/gml">
                 <gml:pos><%= lista(0).Long.ToString.Replace(",", ".") & " " & lista(0).Lat.ToString.Replace(",", ".") %></gml:pos>
             </gml:Point>
        Else
            Dim str_coordinate As String = ""

            For i = 0 To lista.Count - 1
                If str_coordinate.Length = 0 Then
                    str_coordinate = lista(i).Long.ToString + " " + lista(i).Lat.ToString
                Else
                    str_coordinate = str_coordinate + " " + lista(i).Long.ToString + " " + lista(i).Lat.ToString
                End If
            Next
            str_coordinate = str_coordinate.Replace(",", ".")

            nodoGml =
                <gml:LineString xmlns:gml="http://www.opengis.net/gml">
                    <gml:posList><%= str_coordinate %></gml:posList>
                </gml:LineString>
        End If


        xdoc = XDocument.Parse(nodo.ToString)

        Dim ns As XNamespace = "http://www.agronica.it/grafica/"

        xdoc.Elements(ns + "Entita").Elements(ns + "geodata").FirstOrDefault.Add(nodoGml)


        Return xdoc.ToString

    End Function



End Class





Public Class CoordinateGPS_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal DatoGIS As Dati_GPS, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False
        Dim NomeRoutine As String = "AgronicaCoreGiasAlarm.CoordinateGPS_W.Scrivi()"

        Try

            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO   CoordinateGPS ")
            StrSQL.Append("                   ( ")
            StrSQL.Append("                    ID_Coordinata,               DataOra,    ")
            StrSQL.Append("                    IMEI,         Altezza,    ")
            StrSQL.Append("                    Speed,    DOP,    ")
            StrSQL.Append("                    Fix,    ")

            StrSQL.Append("                    TipologiaMovimento,            Poligono_Geoentity ")
            StrSQL.Append("                   ) ")

            StrSQL.Append("VALUES (")

            StrSQL.Append("          " & Agro_vb_SaveNum(DatoGIS.ID_Coordinata) & " ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(DatoGIS.DataOra) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(DatoGIS.IMEI) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(DatoGIS.Altezza) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(DatoGIS.Speed) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(DatoGIS.DOP) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(DatoGIS.FIX) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(DatoGIS.TipologiaMovimento) & "' ")

            StrSQL.Append("         , " & Agro_SQL_SaveGeograpyFromWKTString("POINT(" & DatoGIS.Latitudine.Replace(",", ".") & " " & DatoGIS.Longitudine.Replace(",", ".") & ")") & " ")

            StrSQL.Append(" )")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception


            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp
    End Function
    'Public Function Scrivi(ByVal DatoGIS As Dati_GPS) As Boolean


    '    Dim context As New GIASAlarmEntities

    '    Dim nuovo = New CoordinateGPS()
    '    nuovo.ID_Coordinata = DatoGIS.ID_Coordinata
    '    nuovo.TipologiaMovimento = DatoGIS.TipologiaMovimento
    '    nuovo.IMEI = DatoGIS.IMEI
    '    nuovo.DataOra = DatoGIS.DataOra
    '    nuovo.Altezza = DatoGIS.Altezza
    '    nuovo.DOP = DatoGIS.DOP
    '    nuovo.Fix = DatoGIS.FIX



    '    'nuovo.Latitudine = DatoGIS.Latitudine
    '    'nuovo.Longitudine = DatoGIS.Longitudine

    '    nuovo.Speed = DatoGIS.Speed

    '    context.AddToCoordinateGPS(nuovo)
    '    context.SaveChanges()


    'End Function

End Class