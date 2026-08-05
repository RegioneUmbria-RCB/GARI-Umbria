Imports <xmlns="http://www.agronica.it/grafica/">

Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Imports AgronicaGIS2012.Commons
Imports System.Xml

Imports AgronicaCoreModelsSTD.Gis
Imports AgronicaCoreGisDAL

Public Class DatiEntita_R

    ''' <summary>
    ''' Lettura ricorsiva dei dati cartografici data la piva di un impresa (legge i dati dell'impresa e di tutti i sui discendenti)
    ''' </summary>
    ''' <param name="PivaSuperUser"></param>
    ''' <param name="Piva"></param>
    ''' <param name="ForDelete"></param>
    ''' <param name="AllAttributes"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function LeggiPerGerarchiaImpreseSQLXML(
            ByVal PivaSuperUser As String,
            ByVal PivaPadre As String,
            ByVal Entita_Cod As Int32,
            ByVal TipoEntita_Cod As Int32,
            ByVal Piva As String,
            ByVal Sa_Cod As Int32,
            ByVal Appezza As Int32,
            ByVal Campo_Cod As Int32,
            ByVal ID_Imp As Int32,
            ByVal Prov As String,
            ByVal Com As String,
            ByVal Sezione As String,
            ByVal Foglio As Int32,
            ByVal Numero As Int32,
            ByVal Subalterno As String,
            ByVal ID_Agenda As Int32,
            ByVal Programmazione_Entita_cod As Integer,
            ByVal programmazione_cod As Integer,
            ByVal Ricetta_Operazione_Cod As Integer,
            ByVal AggiungiLayer_Imprese As Boolean,
            ByVal AggiungiLayer_SpecieVegetali As Boolean,
            ByVal ForDelete As Boolean,
            ByVal AllAttributes As Boolean,
            ByVal Sementieri_Sportello_Configurazione_cod As Integer,
            ByVal Codice_Fiscale_Tecnico As String,
            ByVal wktBoundaySTIntersects As String,
            ByVal cfgAlbero As ConfigurazioneAlbero,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
            ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As String


        Dim sXmlRval As String = ""

        Dim LetturaEntita As New AgronicaCoreGisDAL.GIS_Entita_R
        Dim letturaLayers As New AgronicaCoreGisDAL.GIS_LayerElementiGrafici_R


        Dim xmlFinaleElaborato As XDocument =
            LetturaEntita.LeggiXDocument(
                PivaSuperUser,
                Entita_Cod,
                TipoEntita_Cod,
                PivaPadre,
                Piva,
                Sa_Cod,
                Appezza,
                Campo_Cod,
                ID_Imp,
                Prov,
                Com,
                Sezione,
                Foglio,
                Numero,
                Subalterno,
                ID_Agenda,
                Ricetta_Operazione_Cod,
                programmazione_cod,
                Programmazione_Entita_cod,
                Sementieri_Sportello_Configurazione_cod,
                Codice_Fiscale_Tecnico,
                wktBoundaySTIntersects,
                cfgAlbero,
                enumSelezioneVariabile.Selezione_TabellaCompleta,
                "",
                "",
                objParametri,
                objParametri_Utenti
        )

        Dim sLetturaLayer As XDocument =
            letturaLayers.LeggiXDocument(
                PivaSuperUser,
                "",
                 0,
                 AggiungiLayer_Imprese,
                 AggiungiLayer_SpecieVegetali,
                "",
                "",
                objParametri
            )

        Dim nsGrafica As XNamespace = "http://www.agronica.it/grafica/"
        Dim nsGml As XNamespace = "http://www.opengis.net/gml"

        If Not xmlFinaleElaborato Is Nothing Then

            '' modalità che converte in stringa 
            'Dim lRoot As XElement = xmlFinaleElaborato.Root
            'lRoot.Name = nsGrafica + lRoot.Name.LocalName
            'lRoot.Add(New XAttribute(XNamespace.Xmlns + "gml", nsGml))
            'lRoot.Add(sLetturaLayer.Root)
            'xmlFinaleElaborato = XDocument.Parse(lRoot.ToString.Replace("xmlns=""""", ""))
            'sXmlRval = xmlFinaleElaborato.ToString
            ''''''''''''''''''''''''''''''''''''''''
            Dim lRoot As XElement = xmlFinaleElaborato.Root
            lRoot.Name = nsGrafica + lRoot.Name.LocalName
            lRoot.Add(New XAttribute(XNamespace.Xmlns + "gml", nsGml))


            Dim x1 As XDocument = xmlFinaleElaborato
            Dim x2 As XDocument = sLetturaLayer

            x1.Root.Add(x2.Root)


            sXmlRval = x1.ToString.Replace("xmlns=""""", "")


        Else
            xmlFinaleElaborato = XDocument.Parse("<DatiEntita xmlns=""http://www.agronica.it/grafica/"" xmlns:gml=""http://www.opengis.net/gml"">" &
                                                 sLetturaLayer.Root.ToString.Replace("xmlns=""""", "") &
                                                 "</DatiEntita>")

            'Dim xLetturaLayer As XElement = xmlFinaleElaborato.Root
            'xLetturaLayer.Name = nsGrafica + xLetturaLayer.Name.LocalName
            'xLetturaLayer.Add(New XAttribute(XNamespace.Xmlns + "gml", nsGml))

            sXmlRval = xmlFinaleElaborato.ToString

        End If



        'Dim listaNS As New List(Of String)
        'listaNS.Add("http://www.opengis.net/gml")

        'sXmlRval = xmlHelper.RemoveNamespace(xmlFinaleElaborato, listaNS).ToString '.Replace("xmlns:g=", "xmlns=")


        Return sXmlRval

    End Function

    ''' <summary>
    ''' Lettura ricorsiva dei dati cartografici data la piva di un impresa (legge i dati dell'impresa e di tutti i sui discendenti)
    ''' </summary>
    ''' <param name="PivaSuperUser"></param>
    ''' <param name="Piva"></param>
    ''' <param name="ForDelete"></param>
    ''' <param name="AllAttributes"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function LeggiPerGerarchiaImpreseSQLXML_XDoc(ByVal PivaSuperUser As String,
                                                        ByVal PivaPadre As String,
                                                        ByVal Entita_Cod As Int32,
                                                        ByVal TipoEntita_Cod As Int32,
                                                        ByVal Piva As String,
                                                        ByVal Sa_Cod As Int32,
                                                        ByVal Appezza As Int32,
                                                        ByVal Campo_Cod As Int32,
                                                        ByVal ID_Imp As Int32,
                                                        ByVal Prov As String,
                                                        ByVal Com As String,
                                                        ByVal Sezione As String,
                                                        ByVal Foglio As Int32,
                                                        ByVal Numero As Int32,
                                                        ByVal Subalterno As String,
                                                        ByVal ID_Agenda As Int32,
                                                        ByVal Programmazione_Entita_cod As Integer,
                                                        ByVal programmazione_cod As Integer,
                                                        ByVal Ricetta_Operazione_Cod As Integer,
                                                        ByVal AggiungiLayer_Imprese As Boolean,
                                                        ByVal AggiungiLayer_SpecieVegetali As Boolean,
                                                        ByVal ForDelete As Boolean,
                                                        ByVal AllAttributes As Boolean,
                                                        ByVal Sementieri_Sportello_Configurazione_cod As Integer,
                                                        ByVal Codice_Fiscale_Tecnico As String,
                                                        ByVal wktBoundaySTIntersects As String,
                                                        ByVal cfgAlbero As ConfigurazioneAlbero,
                                                        ByVal xFiltroAggiuntivo As String,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                        Optional ByVal leggiLayerNonVisibili As Boolean = False,
                                                        Optional ByVal permessiWMS As Boolean = False
                                                        ) As XDocument

        Dim sXmlRval As String = ""

        Dim LetturaEntita As New AgronicaCoreGisDAL.GIS_Entita_R
        Dim letturaLayers As New AgronicaCoreGisDAL.GIS_LayerElementiGrafici_R

        Dim xmlFinaleElaborato As XDocument = LetturaEntita.LeggiXDocument(PivaSuperUser,
                                                                           Entita_Cod,
                                                                           TipoEntita_Cod,
                                                                           PivaPadre,
                                                                           Piva,
                                                                           Sa_Cod,
                                                                           Appezza,
                                                                           Campo_Cod,
                                                                           ID_Imp,
                                                                           Prov,
                                                                           Com,
                                                                           Sezione,
                                                                           Foglio,
                                                                           Numero,
                                                                           Subalterno,
                                                                           ID_Agenda,
                                                                           Ricetta_Operazione_Cod,
                                                                           programmazione_cod,
                                                                           Programmazione_Entita_cod,
                                                                           Sementieri_Sportello_Configurazione_cod,
                                                                           Codice_Fiscale_Tecnico,
                                                                           wktBoundaySTIntersects,
                                                                           cfgAlbero,
                                                                           enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                           xFiltroAggiuntivo,
                                                                           "",
                                                                           objParametri,
                                                                           objParametri_Utenti,
                                                                           leggiLayerNonVisibili:=leggiLayerNonVisibili)

        Dim sLetturaLayer As XDocument = letturaLayers.LeggiXDocument(PivaSuperUser,
                                                                      "",
                                                                      0,
                                                                      AggiungiLayer_Imprese,
                                                                      AggiungiLayer_SpecieVegetali,
                                                                      "",
                                                                      "",
                                                                      objParametri,
                                                                      leggiLayerNonVisibili:=leggiLayerNonVisibili,
                                                                      permessiWMS)

        If Not xmlFinaleElaborato Is Nothing Then
            'sLetturaEntita = "<DatiEntita xmlns=""http://www.agronica.it/grafica/"" xmlns:gml=""http://www.opengis.net/gml"">" & sLetturaEntita.Replace("<DatiEntita>", "").Replace("</DatiEntita>", "") & vbCrLf & sLetturaLayer & "</DatiEntita>"

            Dim nsGrafica As XNamespace = "http://www.agronica.it/grafica/"
            'Dim nsGml As XNamespace = "http://www.opengis.net/gml"

            Dim lRoot As XElement = xmlFinaleElaborato.Root
            'lRoot.Name = nsGrafica + lRoot.Name.LocalName
            'lRoot.Add(New XAttribute(XNamespace.Xmlns + "gml", nsGml))

            xmlHelper.SetDefaultNamespace(lRoot, nsGrafica)

            Dim xLetturaLayer As XElement = sLetturaLayer.Root
            xmlHelper.SetDefaultNamespace(xLetturaLayer, nsGrafica)

            xmlFinaleElaborato.Root.Add(xLetturaLayer)
        Else
            xmlFinaleElaborato = XDocument.Parse("<DatiEntita xmlns=""http://www.agronica.it/grafica/"" xmlns:gml=""http://www.opengis.net/gml"">" & sLetturaLayer.Root.ToString & "</DatiEntita>")
        End If

        Dim listaNS As New List(Of String)
        listaNS.Add("http://www.opengis.net/gml")

        'xmlHelper.RemoveNamespace2()
        'xmlFinaleElaborato = stripDocumentNamespacewhite(xmlFinaleElaborato)

        'xmlFinaleElaborato.Descendants().Attributes().Where(Function(a) a.IsNamespaceDeclaration).Remove()
        'xmlFinaleElaborato.Save(filePath, SaveOptions.DisableFormatting)

        Return xmlFinaleElaborato

    End Function





    ''' <summary>
    ''' Lettura ricorsiva dei dati cartografici data la piva di un impresa (legge i dati dell'impresa e di tutti i sui discendenti)
    ''' </summary>
    ''' <param name="PivaSuperUser"></param>
    ''' <param name="Piva"></param>
    ''' <param name="ForDelete"></param>
    ''' <param name="AllAttributes"></param>
    ''' <param name="objParametri"></param>
    ''' <param name="objParametri_Utenti"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function LeggiPerGerarchiaImprese(
                                      ByVal PivaSuperUser As String _
                                    , ByVal Piva As String _
                                    , ByVal Codice_Fiscale_Tecnico As String _
                                    , ByVal wktBoundaySTIntersects As String _
                                    , ByVal cfgAlbero As ConfigurazioneAlbero _
                                    , ByVal ForDelete As Boolean _
                                    , ByVal AllAttributes As Boolean _
                                    , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    , ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As String


        Dim sXmlRval As String = ""

        Dim LetturaEntita As New AgronicaCoreGisDAL.GIS_Entita_R
        Dim LetturaPoligoni As New AgronicaCoreGisBIZ.GIS_ElementiGrafici_R

        Dim dtLetturaEntita As DataTable =
            LetturaEntita.Leggi(
                PivaSuperUser,
                0,
                0,
                Piva,
                0,
                0,
                0,
                0,
                "",
                "",
                "-1",
                -1,
                -1,
                "-1",
                0,
                0,
                0,
                0,
                Codice_Fiscale_Tecnico,
                wktBoundaySTIntersects,
                cfgAlbero,
                enumSelezioneVariabile.Selezione_TabellaCompleta,
                "",
                "",
                objParametri,
                objParametri_Utenti
        )


        Dim xmlFinaleElaborato As XDocument = XDocument.Parse("<DatiEntita xmlns=""http://www.agronica.it/grafica/"" xmlns:gml=""http://www.opengis.net/gml""></DatiEntita>")
        Dim Layers = <layersdescrizioni>
                         <layer tipologia_layer="1" nome_layer="entità">
                             <valori codice="1">Appezzamento</valori>
                             <valori codice="13">Catasto</valori>
                         </layer>
                         <layer tipologia_layer="10" nome_layer="Conversione coordinate">
                             <valori codice="1">GPS Originali</valori>
                             <valori codice="2">Disegnate</valori>
                         </layer>
                     </layersdescrizioni>



        Dim _LayersImprese As New List(Of String)

        For Each dRow In dtLetturaEntita.Rows

            Dim nodoentita = <Entita TipoOperazioneDB="1" recno="" deleted="" section="E" descr="TEXT      22"
                                 ecolor="256" eline="256" rad="15" text=<%= dRow("ElementoGrafico_DES") %> gps="1" lat="0" lon="0" pdop="0" validita_inizio="01/09/2009" validita_fine="31/08/2010" username_creazione="" username_modifica="">
                                 <layers>
                                     <layer tipologia_layer="1"><%= dRow("layerElementiGrafici_cod") %></layer>
                                 </layers>
                                 <EntitaGIAS>
                                     <DatoGias>
                                         <PivaSuperUser><%= dRow("PivaSuperUser") %></PivaSuperUser>
                                         <Entita_Cod><%= dRow("Entita_Cod") %></Entita_Cod>
                                         <TipoEntita_Cod><%= dRow("TipoEntita_Cod") %></TipoEntita_Cod>
                                         <Piva><%= dRow("Piva") %></Piva>
                                         <Sa_Cod><%= dRow("Sa_Cod") %></Sa_Cod>
                                         <Appezza><%= dRow("Appezza") %></Appezza>
                                         <Campo_Cod><%= dRow("Campo_Cod") %></Campo_Cod>
                                         <Id_Imp><%= dRow("Id_Imp") %></Id_Imp>
                                         <PROV><%= dRow("PROV") %></PROV>
                                         <COM><%= dRow("COM") %></COM>
                                         <SEZIONE><%= dRow("SEZIONE") %></SEZIONE>
                                         <FOGLIO><%= dRow("FOGLIO") %></FOGLIO>
                                         <NUMERO><%= dRow("NUMERO") %></NUMERO>
                                         <SUBALTERNO><%= dRow("SUBALTERNO") %></SUBALTERNO>
                                         <Programmazione_Entita_Cod><%= dRow("Programmazione_Entita_Cod") %></Programmazione_Entita_Cod>
                                         <Programmazione_Cod><%= dRow("Programmazione_Cod") %></Programmazione_Cod>
                                         <Id_Agenda><%= dRow("Id_Agenda") %></Id_Agenda>
                                         <inviato><%= dRow("inviato") %></inviato>
                                         <Data_Creazione><%= dRow("Data_Creazione") %></Data_Creazione>
                                         <Data_Modifica><%= dRow("Data_Modifica") %></Data_Modifica>
                                         <Username_Creazione><%= dRow("Username_Creazione") %></Username_Creazione>
                                         <Username_Modifica><%= dRow("Username_Modifica") %></Username_Modifica>
                                         <Validita_Inizio><%= dRow("Validita_Inizio") %></Validita_Inizio>
                                         <Validita_Fine><%= dRow("Validita_Fine") %></Validita_Fine>
                                     </DatoGias>
                                 </EntitaGIAS>
                             </Entita>


            If Not _LayersImprese.Contains(dRow("Piva")) Then
                _LayersImprese.Add(dRow("Piva"))
            End If

            Dim xGeoData As String = LetturaPoligoni.LeggiDatiGML(
                dRow("PivaSuperUser"),
                0,
                dRow("Entita_Cod"),
                0,
                enumFromatoCartograficoConvertito.GML,
                "",
                "",
                objParametri
            )

            Dim appXGeoDAta As XDocument = XDocument.Parse(xGeoData)
            Dim xGeoDataNode = appXGeoDAta.Elements("DatiEntita").Elements("Entita").Elements("geodata").FirstOrDefault

            nodoentita.Add(xGeoDataNode)
            xmlFinaleElaborato.Root.Add(nodoentita)

        Next

        xmlFinaleElaborato.Root.Add(Layers)

        Dim listaNS As New List(Of String)
        listaNS.Add("http://www.opengis.net/gml")


        sXmlRval = xmlHelper.RemoveNamespace(xmlFinaleElaborato, listaNS).ToString.Replace("xmlns=""""", "")

        Return sXmlRval


    End Function

    Public Function LeggiPerImpresa(
                         ByVal PivaSuperUser As String _
            , ByVal Piva As String _
            , ByVal ForDelete As Boolean _
            , ByVal AllAttributes As Boolean _
            , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As String

    End Function


#Region "Nuove Chiamate GIS"
    Public Function LeggiPerGerarchiaImprese(
            ByVal PivaSuperUser As String,
            ByVal PivaPadre As String,
            ByVal Entita_Cod As Int32,
            ByVal TipoEntita_Cod As Int32,
            ByVal Piva As String,
            ByVal Sa_Cod As Int32,
            ByVal Appezza As Int32,
            ByVal Campo_Cod As Int32,
            ByVal Veg_Cod As Int32,
            ByVal DestinazioneUso_Cod As Int32,
            ByVal ID_Imp As Int32,
            ByVal Prov As String,
            ByVal Com As String,
            ByVal Sezione As String,
            ByVal Foglio As Int32,
            ByVal Numero As Int32,
            ByVal Subalterno As String,
            ByVal ID_Agenda As Int32,
            ByVal Programmazione_Entita_cod As Integer,
            ByVal programmazione_cod As Integer,
            ByVal Ricetta_Operazione_Cod As Integer,
            ByVal AggiungiLayer_Imprese As Boolean,
            ByVal AggiungiLayer_SpecieVegetali As Boolean,
            ByVal ForDelete As Boolean,
            ByVal AllAttributes As Boolean,
            ByVal Sementieri_Sportello_Configurazione_cod As Integer,
            ByVal Codice_Fiscale_Tecnico As String,
            ByVal wktBoundaySTIntersects As String,
            ByVal cfgAlbero As ConfigurazioneAlbero,
            ByVal xFiltroAggiuntivo As String,
            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
            ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
            Optional ByVal filtroTemporaleAvanzato As FiltroTemporaleAvanzato = Nothing,
            Optional ByVal layerElementiGrafici_cod As List(Of Integer) = Nothing,
            Optional ByVal LayersVisibili As LayerVisibiliUtenteTipologia = Nothing
        ) As DataTable

        Dim sXmlRval As String = ""

        Dim LetturaEntita As New AgronicaCoreGisDAL.GIS_Entita_R
        Dim letturaLayers As New AgronicaCoreGisDAL.GIS_LayerElementiGrafici_R

        'Lavez - 19/11/2025 - ripristino vecchia lettura layer visibili ma condizionata al fatto che non vengano specificati dai livelli superiori
        If LayersVisibili Is Nothing Then
            'lavez - 20/11/2023 - elenco layer visibili
            LayersVisibili = New GIS_LayerElementiGrafici().GetElencoLayerVisibiliPerTipologia_e_Utente(PivaSuperUser, cfgAlbero.TipologiaLayer_Cod, objParametri_Server)
            If LayersVisibili.elencoLayers Is Nothing Then
                LayersVisibili.elencoLayers = New List(Of ElencoLayerVisibiliUtenteTipologia)
            End If

            'Lavez - 18/07/2024 - (se provengo da fuori il gis) ottimizzazione carico query se ho dei layer specifici sostituisci quelli precedentemente letti
            If layerElementiGrafici_cod IsNot Nothing Then
                If layerElementiGrafici_cod.Count > 0 Then
                    LayersVisibili.elencoLayers.Clear()

                    For Each l In layerElementiGrafici_cod
                        LayersVisibili.elencoLayers.Add(New ElencoLayerVisibiliUtenteTipologia With {
                                                        .LayerElementiGrafici_Cod = l,
                                                        .Flag_Attivo = True,
                                                        .Flag_Visbile = True,
                                                        .LayerElementiGrafici_Des = CType(l, AgronicaCoreDataProvider.TipiEnumerativi.enum_Gis_LayerElementiGrafici_std).ToString()
                                                    })
                    Next
                End If
            End If
        End If


        Return LetturaEntita.LeggiDocumentiGIS(
                PivaSuperUser,
                Entita_Cod,
                TipoEntita_Cod,
                PivaPadre,
                Piva,
                Sa_Cod,
                Appezza,
                Campo_Cod,
                Veg_Cod,
                DestinazioneUso_Cod,
                ID_Imp,
                Prov,
                Com,
                Sezione,
                Foglio,
                Numero,
                Subalterno,
                ID_Agenda,
                Ricetta_Operazione_Cod,
                programmazione_cod,
                Programmazione_Entita_cod,
                Sementieri_Sportello_Configurazione_cod,
                Codice_Fiscale_Tecnico,
                wktBoundaySTIntersects,
                cfgAlbero,
                layerElementiGrafici_cod,
                enumSelezioneVariabile.Selezione_TabellaCompleta,
                xFiltroAggiuntivo,
                "",
                objParametri_Server,
                objParametri_Utenti,
                filtroTemporaleAvanzato:=filtroTemporaleAvanzato,
                ElencoLayerVisibili:=LayersVisibili)
    End Function

    Public Function LeggiXPuntiGPS(ByVal DataInizio As String,
                                   ByVal DataFine As String,
                                   ByVal PermessiUtente As Boolean,
                                   ByVal UltimaPosizione As Boolean,
                                   ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                   ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                   ) As DataTable
        Dim DT As DataTable

        Dim xRead As New APP_EntrateUsciteCoordinate_R

        Dim DataInizioDate As Date
        Dim DataFineDate As Date

        If Not Date.TryParse(DataInizio, DataInizioDate) Then
            Throw New Exception("La data di inizio deve essere una data")
        End If

        If Not Date.TryParse(DataFine, DataFineDate) Then
            Throw New Exception("La data di fine deve essere una data")
        End If

        If UltimaPosizione Then
            DT = xRead.LeggiXPuntiGpsUltimaPosizione(Not PermessiUtente, DataInizioDate, DataFineDate, "", "", objParametri_Server, objParametri_Utenti)
        Else
            DT = xRead.LeggiXPuntiGPS(Not PermessiUtente, DataInizioDate, DataFineDate, "", "", objParametri_Server, objParametri_Utenti)
        End If

        Return DT

    End Function

    Public Function LeggiPerGerarchiaImpreseXFiltroTemporale(
            ByVal PivaSuperUser As String,
            ByVal PivaPadre As String,
            ByVal Piva As String,
            ByVal Sa_Cod As Int32,
            ByVal ID_Imp As Int32,
            ByVal AggiungiLayer_Imprese As Boolean,
            ByVal AggiungiLayer_SpecieVegetali As Boolean,
            ByVal ForDelete As Boolean,
            ByVal AllAttributes As Boolean,
            ByVal Sementieri_Sportello_Configurazione_cod As Integer,
            ByVal Codice_Fiscale_Tecnico As String,
            ByVal wktBoundaySTIntersects As String,
            ByVal cfgAlbero As ConfigurazioneAlbero,
            ByVal xFiltroAggiuntivo As String,
            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
            ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
            Optional ByVal filtroTemporaleAvanzato As FiltroTemporaleAvanzato = Nothing,
            Optional ByVal layerElementiGrafici_cod As List(Of Integer) = Nothing
        ) As DataTable

        Dim sXmlRval As String = ""

        Dim LetturaEntita As New AgronicaCoreGisDAL.GIS_Entita_R
        Dim letturaLayers As New AgronicaCoreGisDAL.GIS_LayerElementiGrafici_R


        'lavez - 20/11/2023 - elenco layer visibili
        Dim LayersVisibili = New GIS_LayerElementiGrafici().GetElencoLayerVisibiliPerTipologia_e_Utente(PivaSuperUser, cfgAlbero.TipologiaLayer_Cod, objParametri_Server)
        If LayersVisibili.elencoLayers Is Nothing Then
            LayersVisibili.elencoLayers = New List(Of ElencoLayerVisibiliUtenteTipologia)
        ElseIf LayersVisibili.elencoLayers.FirstOrDefault(Function(e) e.LayerElementiGrafici_Cod = 19) IsNot Nothing Then
            LayersVisibili.elencoLayers.Clear()
            LayersVisibili.elencoLayers.Add(New ElencoLayerVisibiliUtenteTipologia() With {
                            .LayerElementiGrafici_Cod = 19,
                            .LayerElementiGrafici_Des = "IMPIANTI",
                            .Flag_Attivo = True,
                            .Flag_Visbile = True
                        })
        End If

        Return LetturaEntita.LeggiDocumentiGIS(
                PivaSuperUser,
                0,
                0,
                PivaPadre,
                Piva,
                Sa_Cod,
                0,
                0,
                0,
                0,
                ID_Imp,
                "",
                "",
                "-1",
                -1,
                -1,
                "-1",
                0,
                0,
                0,
                0,
                Sementieri_Sportello_Configurazione_cod,
                Codice_Fiscale_Tecnico,
                wktBoundaySTIntersects,
                cfgAlbero,
                layerElementiGrafici_cod,
                enumSelezioneVariabile.Selezione_TabellaCompleta,
                xFiltroAggiuntivo,
                "",
                objParametri_Server,
                objParametri_Utenti,
                filtroTemporaleAvanzato:=filtroTemporaleAvanzato,
                ElencoLayerVisibili:=LayersVisibili)
    End Function

#End Region

End Class



Public Class DatiEntita_W
    Inherits AgronicaCoreDataProvider.DataProvider

    ''' <summary>
    ''' Scrive nel database tutte le entità presenti nell'xml passato come parametro.
    ''' </summary>
    ''' <param name="xmlDatiEntita"></param>
    ''' <param name="OUTPUT_EntitaCod"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function scrivi( _
                          ByVal xmlDatiEntita As String, _
                          ByRef OUTPUT_EntitaCod As Integer, _
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
            ) As Boolean

        Const NomeRoutine As String = "DatiEntita_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim xRisp As Boolean = True

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False


        Try

            '------------------------------
            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, _
                                                                                    FlagTransazioneLocale, _
                                                                                    objParametri)


            'spacchetto l'xml e salvo gli elementi
            Dim DocumentoSalva As XDocument = XDocument.Parse(xmlDatiEntita)

            Dim ScriviEntita As New AgronicaCoreGisBIZ.GIS_Entita_W

            Dim dummy As Integer
            For Each SingleEntita In DocumentoSalva.<DatiEntita>.<Entita>
                ScriviEntita.scrivi(SingleEntita.ToString, dummy, objParametri)

            Next

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

        Catch ex As Exception

            xRisp = False

            'Faccio il rollback della transazione
            If Not objParametri.objTransazione Is Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            '//////////////////////////////////////////////////////////////////////
            MessaggioErrore = "(Entita_cod=" + OUTPUT_EntitaCod + ")" + _
                              " : " + ex.Message
            '//////////////////////////////////////////////////////////////////////

            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally

            ''Chiudo la connessione se è stata aperta in questa routine
            'If (FlagConnessioneLocale = True) AndAlso (Not objParametri.objConnessione Is Nothing) Then
            '    objParametri.objConnessione.Close()
            'End If
            'Chiudo la connessione se è stata aperta in questa routine
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)

        End Try


        Return xRisp


    End Function


End Class
