
Imports <xmlns="http://grafica_new">
Imports AgronicaConversioneCartografiaGias.FormatsConverter

Public Class EsriAsciiToAgronicaGIS2012

    Public Function Converti(ByVal esriiString As String) As String

        Dim ElemFinalXdoc As XElement = <DatiEntita xmlns="http://www.agronica.it/grafica/"
                                            xmlns:gml="http://www.opengis.net/gml"></DatiEntita>
        Dim FinalDoc As New XDocument
        FinalDoc.Add(ElemFinalXdoc)

        Dim sNodeDoc As XDocument
        Dim wktHelp As New WKT
        Dim wktToGeoML As New wkt_gml



        '    sNodeDoc = ReadXmlFromString( _
        '    wktToGeoML.Trasforma( _
        '        cconverter.WKTPolygonWGS84_from_WKTPolygonED50(wktHelp.CreaPoligonoDaCoordinate(DatiCartograficiED50), True, ParametriCartografici), _
        '        swapLatLong, _
        '        False, _
        '        True, _
        '        EntitaDaTrasformare.ElementoGrafico_cod) _
        ')

        '    sNodeDoc.Root.@Flag_GPS = 0






        'Dim newEntitaElement = <Entita TipoOperazioneDB=<%= EntitaDaTrasformare.TipoOperazioneDB %> recno="" deleted="" section="E" id=<%= EntitaDaTrasformare.CodiceGias %> descr=<%= EntitaDaTrasformare.Descr %> ecolor="256" eline="256" rad="15" text=<%= EntitaDaTrasformare.Text %> gps="1" lat="0" lon="0" pdop="0" validita_inizio="01/09/2009" validita_fine="31/08/2010" username_creazione="" username_modifica="">
        '                           <layers>
        '                               <layer tipologia_layer="1"><%= lLayer_nuovo %></layer>
        '                               <layer tipologia_layer="10"><%= EntitaDaTrasformare.DatoConvertito %></layer>
        '                               <layer tipologia_layer="100"><%= PivaPadre %></layer>
        '                               <layer tipologia_layer="5"><%= veg_cod %></layer>
        '                           </layers>
        '                           <EntitaGIAS>
        '                               <DatoGias>
        '                                   <PivaSuperUser><%= pivasuperuser %></PivaSuperUser>
        '                                   <Entita_Cod><%= EntitaDaTrasformare.Entita_Cod %></Entita_Cod>
        '                                   <TipoEntita_Cod><%= EntitaDaTrasformare.NuovoTipoEntita %></TipoEntita_Cod>
        '                                   <Piva><%= piva %></Piva>
        '                                   <Sa_Cod><%= sa_cod %></Sa_Cod>
        '                                   <Appezza><%= EntitaDaTrasformare.Appezza %></Appezza>
        '                                   <Campo_Cod><%= EntitaDaTrasformare.Campo_cod %></Campo_Cod>
        '                                   <Id_Imp><%= EntitaDaTrasformare.RegImpianto %></Id_Imp>
        '                                   <PROV><%= EntitaDaTrasformare.PROV %></PROV>
        '                                   <COM><%= EntitaDaTrasformare.COM %></COM>
        '                                   <SEZIONE><%= EntitaDaTrasformare.SEZIONE %></SEZIONE>
        '                                   <FOGLIO><%= EntitaDaTrasformare.FOGLIO %></FOGLIO>
        '                                   <NUMERO><%= EntitaDaTrasformare.NUMERO %></NUMERO>
        '                                   <SUBALTERNO><%= EntitaDaTrasformare.SUBALTERNO %></SUBALTERNO>
        '                                   <Programmazione_Entita_Cod>0</Programmazione_Entita_Cod>
        '                                   <Id_Agenda>0</Id_Agenda>
        '                                   <Ricetta_Operazione_Cod>0</Ricetta_Operazione_Cod>
        '                                   <OLDGrafica_ID><%= EntitaDaTrasformare.CodiceGias %></OLDGrafica_ID>
        '                                   <analisi_campione_cod><%= EntitaDaTrasformare.Analisi_campione_cod %></analisi_campione_cod>
        '                                   <inviato>0</inviato>
        '                                   <Data_Creazione><%= GetData_Creazione(EntitaDaTrasformare.Data_Modifica) %></Data_Creazione>
        '                                   <Data_Modifica><%= GetData_Creazione(EntitaDaTrasformare.Data_Modifica) %></Data_Modifica>
        '                                   <Username_Creazione>agronica</Username_Creazione>
        '                                   <Username_Modifica>agronica</Username_Modifica>
        '                                   <Validita_Inizio>1900-01-01T00:00:00</Validita_Inizio>
        '                                   <Validita_Fine>2100-12-31T00:00:00</Validita_Fine>
        '                               </DatoGias>
        '                           </EntitaGIAS>
        '                       </Entita>

        'newEntitaElement.Add(sNodeDoc.FirstNode)
        'FinalDoc.Root.Add(newEntitaElement)

    End Function

    Private Function ReadXmlFromString(ByVal stringaXml As String) As XDocument
        Return XDocument.Parse(stringaXml)
    End Function
End Class
