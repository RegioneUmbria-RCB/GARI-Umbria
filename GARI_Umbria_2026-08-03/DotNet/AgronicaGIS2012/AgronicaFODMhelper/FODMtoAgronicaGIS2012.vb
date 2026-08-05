
Imports <xmlns="http://grafica_new">
Imports AgronicaConversioneCartografiaGias.FormatsConverter
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaGIS2012.Commons.xyz
Imports AgronicaGIS2012.Commons.DataOraHelper
Imports AgronicaGIS2012.Commons
Imports AgronicaConversioneCartografiaGias.Agronica
Imports AgronicaCoreDataProvider

Public Class FODMtoAgronicaGIS2012


    ''' <summary>
    ''' xml in formato FODM
    ''' </summary>
    ''' <param name="lTipoEntita_Cod"></param>
    ''' <param name="lLayer"></param>
    ''' <param name="sFodm"></param>
    ''' <param name="PivaSuperUser"></param>
    ''' <param name="piva"></param>
    ''' <param name="sa_cod"></param>
    ''' <param name="appezza"></param>
    ''' <param name="campo_cod"></param>
    ''' <param name="regImpianto"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetEntitaGraficaFromFodm(
            PoligonoEnvelope As String,
            ByVal sFodm As String,
            ByVal PivaSuperUser As String,
            ByVal piva As String,
            ByVal sa_cod As Integer,
            ByVal appezza As Integer,
            ByVal campo_cod As Integer,
            ByVal regImpianto As Integer,
            ByVal programmazione_Cod As Integer,
            ByVal Ricetta_Operazione_cod As Integer,
            ByVal lLayer As Integer,
            ByVal lTipoEntita_Cod As String,
            ByVal ParametriCoordinate As ParametriCoordinateConverter,
            objparametri_server As AgronicaCoreParametri
    ) As String

        Dim xFrom As XDocument = XDocument.Parse(sFodm)

        Dim gisDal As New AgronicaCoreGisDAL.GIS_Entita_R
        Dim sNodeDoc As XDocument
        Dim wktHelp As New WKT
        Dim wktToGeoML As New wkt_gml

        Dim ElemFinalXdoc As XElement = <DatiEntita xmlns="http://www.agronica.it/grafica/"
                                            xmlns:gml="http://www.opengis.net/gml"></DatiEntita>

        Dim FinalDoc As New XDocument
        FinalDoc.Add(ElemFinalXdoc)


        If PoligonoEnvelope <> "" Then
            Dim xDocEvelope As XDocument = XDocument.Parse(PoligonoEnvelope)
            PoligonoEnvelope = xDocEvelope.Element("DatiEntita").Element("Entita").Element("geodata").FirstNode.ToString()
        End If

        For Each e In (
            From f In xFrom.Elements("GeoAnswer").Elements("Records").Elements("Record")
            )

            Dim wkt As String = e.Elements("GeometryWKT").Value
            Dim Rateo As String = e.Elements("Value").Value

            Dim dRateo As Double = myCDBL(Rateo)
            Dim drateoSec As Double = Rateo / 10


            Dim data_creazione As DateTime = Now



            Dim RateoFinal As String = "CAP_N+§ " & dRateo.ToString.Replace(".", ",") & "|Prod#_secc§ " & dRateo.ToString.Replace(".", ",") & "|"

            Dim newEntitaElement = <Entita TipoOperazioneDB="1" recno="" deleted="" section="E" id="" descr=<%= RateoFinal %> ecolor="256" eline="256" rad="15" text=<%= RateoFinal %> gps="1" lat="0" lon="0" pdop="0" validita_inizio="01/09/2009" validita_fine="31/08/2010" username_creazione="" username_modifica="">
                                       <layers>
                                           <layer tipologia_layer="1"><%= lLayer %></layer>
                                       </layers>
                                       <EntitaGIAS>
                                           <DatoGias>
                                               <PivaSuperUser><%= PivaSuperUser %></PivaSuperUser>
                                               <Entita_Cod>0</Entita_Cod>
                                               <TipoEntita_Cod><%= lTipoEntita_Cod %></TipoEntita_Cod>
                                               <Piva><%= piva %></Piva>
                                               <Sa_Cod><%= sa_cod %></Sa_Cod>
                                               <Appezza><%= appezza %></Appezza>
                                               <Campo_Cod><%= campo_cod %></Campo_Cod>
                                               <Id_Imp><%= regImpianto %></Id_Imp>
                                               <PROV></PROV>
                                               <COM></COM>
                                               <SEZIONE>0</SEZIONE>
                                               <FOGLIO>0</FOGLIO>
                                               <NUMERO>0</NUMERO>
                                               <SUBALTERNO></SUBALTERNO>
                                               <Programmazione_Entita_Cod>0</Programmazione_Entita_Cod>
                                               <Programmazione_Cod><%= programmazione_Cod %></Programmazione_Cod>
                                               <Id_Agenda>0</Id_Agenda>
                                               <id_mov_det>0</id_mov_det>
                                               <Ricetta_Operazione_Cod><%= Ricetta_Operazione_cod %></Ricetta_Operazione_Cod>
                                               <OLDGrafica_ID></OLDGrafica_ID>
                                               <analisi_campione_cod>0</analisi_campione_cod>
                                               <inviato>0</inviato>
                                               <Data_Creazione><%= GetData_Creazione(data_creazione) %></Data_Creazione>
                                               <Data_Modifica><%= GetData_Creazione(data_creazione) %></Data_Modifica>
                                               <Username_Creazione>agronica</Username_Creazione>
                                               <Username_Modifica>agronica</Username_Modifica>
                                               <Validita_Inizio>1900-01-01T00:00:00</Validita_Inizio>
                                               <Validita_Fine>2100-12-31T00:00:00</Validita_Fine>
                                           </DatoGias>
                                       </EntitaGIAS>
                                   </Entita>

            Dim cconverter As New CoordinateConverter

            Dim xFinalXmlString As String

            If ParametriCoordinate Is Nothing Then
                xFinalXmlString = wktToGeoML.Trasforma(
                   wkt,
                    False,
                    False,
                    True,
                    0)
                sNodeDoc = ReadXmlFromString(xFinalXmlString)
            Else
                xFinalXmlString =
                wktToGeoML.Trasforma(
                    cconverter.WKTPolygonWGS84_from_WKTPolygonED50(
                        wktHelp.CreaPoligonoDaCoordinate(wktHelp.CreaCoordinateDaPoligono(wkt)),
                        True, ParametriCoordinate),
                    False,
                    False,
                    True,
                    0)
                sNodeDoc = ReadXmlFromString(xFinalXmlString)

            End If

            Dim Intersezione As Boolean = True
            Dim p1 As String = sNodeDoc.Root.FirstNode.ToString()
            p1 = AgronicaGIS2012.Commons.PolygonOrder.InvertiPoligono(p1, True)
            If PoligonoEnvelope <> "" Then
                Intersezione = gisDal.VerificaIntersezioneFraDuePoligoniGML(False, PoligonoEnvelope, p1, objparametri_server)
            End If

            If Intersezione Then


                Dim area As Double = 0
                Try
                    area = gisDal.AreaDaGML(p1, objparametri_server)
                Catch ex As Exception

                End Try

                Dim datiCalcolati = <DatiCalcolati>
                                        <geodata_Area><%= area.ToString().Replace(",", ".") %></geodata_Area>
                                        <geodata_Perimetro>0</geodata_Perimetro>
                                    </DatiCalcolati>

                sNodeDoc.Root.Add(datiCalcolati)
                sNodeDoc.Root.@Flag_GPS = 0


                newEntitaElement.Add(sNodeDoc.FirstNode)
                FinalDoc.Root.Add(newEntitaElement)

            End If
            'not intersezione


        Next

        Dim ListaNS As New List(Of String)
        ListaNS.Add("http://www.opengis.net/gml")

        Dim rval As String

        rval = xmlHelper.RemoveNamespace(FinalDoc, ListaNS).ToString.Replace("xmlns=""""", "")
        Return rval

    End Function

    Private Shared Function GetData_Creazione(ByVal dataCreazione As DateTime) As String
        Return CreateISO8601DateTimeFromSystemDateTime(dataCreazione)
    End Function

    Private Function ReadXmlFromString(ByVal stringaXml As String) As XDocument
        Return XDocument.Parse(stringaXml)
    End Function
End Class
