Imports <xmlns="http://G2G">

Imports AgronicaCoreModello
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class LetturaDatiImpreseXML
    Public Function GeneraXmlDaImportare(
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                ByVal urlWsimportaGias As String,
                ByVal lConnessione_SERVER_ORIGINE As String,
                ByVal lConnessione_SERVER_DESTINAZIONE As String,
                ByVal lConnessione_UTENTI_ORIGINE As String,
                ByVal lConnessione_UTENTI_DESTINAZIONE As String,
                ByVal lProgressivoGIAS_ORIGINE As String,
                ByVal lProgressivoGIAS_DESTINAZIONE As String,
                ByVal lPivaSuperUser_ORIGINE As String,
                ByVal lPivaSuperUser_DESTINAZIONE As String,
                ByVal lUsernameSuperUser_Origine As String,
                ByVal lUsernameSuperUser_Destinazione As String,
                ByVal lImport_CodFiscale_ORIGINE As String,
                ByVal lImport_CodFiscale_DESTINAZIONE As String,
                ByVal lImport_Username_ORIGINE As String,
                ByVal lImport_Username_DESTINAZIONE As String,
                ByVal lImport_CodiceImpresa_ORIGINE As String,
                ByVal lImport_CodiceImpresa_DESTINAZIONE As String,
                ByVal lImpostazioniTrasformazioni As String,
                ByVal lPercorsoConnessioni As String,
                ByVal lCacheImprese As String,
                ByVal cfgImpresa As List(Of clsDatiImpresaLista),
                ByVal cfgGlobali As List(Of clsDatiImpresaLista),
                ByVal filtrone As String,
                ByVal filtronerisultato As String,
                ByVal filtronerisultato_azienda As String,
                Optional ByVal xFiltroAggiuntivo As String = ""
        ) As String


        Dim xdoc As XDocument = XDocument.Parse("<dati xmlns=""http://G2G""></dati>")

        Dim nodoConfigurazione As XNode = Gias2Gias_LIB.Funzioni_MasterG2G.ImpostaNodoConfigurazione(
            urlWsimportaGias,
            lConnessione_SERVER_ORIGINE,
            lConnessione_SERVER_DESTINAZIONE,
            lConnessione_UTENTI_ORIGINE,
            lConnessione_UTENTI_DESTINAZIONE,
            lProgressivoGIAS_ORIGINE,
            lProgressivoGIAS_DESTINAZIONE,
            lPivaSuperUser_ORIGINE,
            lPivaSuperUser_DESTINAZIONE,
            lUsernameSuperUser_Origine,
            lUsernameSuperUser_Destinazione,
            lImport_CodFiscale_ORIGINE,
            lImport_CodFiscale_DESTINAZIONE,
            lImport_Username_ORIGINE,
            lImport_Username_DESTINAZIONE,
            lImport_CodiceImpresa_ORIGINE,
            lImport_CodiceImpresa_DESTINAZIONE,
            lImpostazioniTrasformazioni,
            lPercorsoConnessioni
        )

        Dim nodoDettImprese As String

        If lCacheImprese = "true" Then
            Dim leggiImprese As New AgronicaCoreG2GLocalDal.G2GLocal_R
            nodoDettImprese = leggiImprese.LeggiImpreseXML(False, objParametri)

        Else
            'nodoDettImprese = leggiNodoImpreseDataCache(lCacheImprese)
            nodoDettImprese = "<imprese xmlns=""http://G2G""></imprese>"
        End If

        Dim nodoImprese As XElement = XElement.Parse(nodoDettImprese)

        xdoc.Root.Add(nodoConfigurazione)
        xdoc.Root.Add(nodoImprese)

        'Impresa
        Dim nodoDatiimpresa As XElement = XElement.Parse("<datiimpresa></datiimpresa>")

        For Each elementoDatiImpresa In cfgImpresa
            Dim nodoCfg As XElement = New XElement(elementoDatiImpresa.NomeElemento)
            If elementoDatiImpresa.ValiditaInizio <> AGRODATAINIZIO Then
                Dim nodoCfgDAtaInizio As XElement = New XElement("validita_inizio")
                nodoCfgDAtaInizio.Value = elementoDatiImpresa.ValiditaInizio
                nodoCfg.AddFirst(nodoCfgDAtaInizio)
            End If
            If elementoDatiImpresa.ValiditaFine <> AGRODATAFINE Then
                Dim nodoCfgDAtaFine As XElement = New XElement("validita_fine")
                nodoCfgDAtaFine.Value = elementoDatiImpresa.ValiditaFine
                nodoCfg.AddFirst(nodoCfgDAtaFine)
            End If
            If elementoDatiImpresa.Configurazione <> "" Then
                Dim nodoCfgConfig As XElement = New XElement("configurazione")
                nodoCfgConfig.Value = elementoDatiImpresa.Configurazione
                nodoCfg.AddFirst(nodoCfgConfig)
            End If
            nodoDatiimpresa.Add(nodoCfg)
        Next

        nodoImprese.Add(nodoDatiimpresa)




        'Globali
        Dim nodoDatiGlobali As XElement = XElement.Parse("<datiglobali></datiglobali>")

        For Each elementoDatiGlobali In cfgGlobali
            Dim nodoCfg As New XElement(elementoDatiGlobali.NomeElemento)
            If elementoDatiGlobali.ValiditaInizio <> AGRODATAINIZIO AndAlso elementoDatiGlobali.NomeElemento <> "flagallinea_recodes" Then
                Dim nodoCfgDAtaInizio As New XElement("validita_inizio")
                nodoCfgDAtaInizio.Value = elementoDatiGlobali.ValiditaInizio
                nodoCfg.AddFirst(nodoCfgDAtaInizio)
            End If
            If elementoDatiGlobali.ValiditaFine <> AGRODATAFINE AndAlso elementoDatiGlobali.NomeElemento <> "flagallinea_recodes" Then
                Dim nodoCfgDAtaFine As New XElement("validita_fine")
                nodoCfgDAtaFine.Value = elementoDatiGlobali.ValiditaFine
                nodoCfg.AddFirst(nodoCfgDAtaFine)
            End If
            If elementoDatiGlobali.Configurazione <> "" AndAlso elementoDatiGlobali.NomeElemento <> "flagallinea_recodes" Then
                Dim nodoCfgConfig As New XElement("configurazione")
                nodoCfgConfig.Value = elementoDatiGlobali.Configurazione
                nodoCfg.AddFirst(nodoCfgConfig)
            End If
            nodoDatiGlobali.Add(nodoCfg)
        Next

        nodoImprese.Add(nodoDatiGlobali)







        'Filtrone
        Dim nodofiltrone As XElement = New XElement("filtrone")
        nodofiltrone.Value = filtrone
        Dim nodofiltronerisultato As XElement = New XElement("filtronerisultato")
        nodofiltronerisultato.Value = filtronerisultato
        Dim nodofiltronerisultato_azienda As XElement = New XElement("filtronerisultato_azienda")
        nodofiltronerisultato_azienda.Value = filtronerisultato_azienda

        nodoImprese.Add(nodofiltrone)
        nodoImprese.Add(nodofiltronerisultato)
        nodoImprese.Add(nodofiltronerisultato_azienda)


        Dim listaNS As New List(Of String)
        listaNS.Add("http://")


        Return RemoveNamespace(xdoc, listaNS).ToString.Replace("xmlns=""""", "")


    End Function

    Private Shared Function ImpostaNodoConfigurazione(
        urlWsimportaGias As String,
        lConnessione_SERVER_ORIGINE As String,
        lConnessione_SERVER_DESTINAZIONE As String,
        lConnessione_UTENTI_ORIGINE As String,
        lConnessione_UTENTI_DESTINAZIONE As String,
        lProgressivoGIAS_ORIGINE As String,
        lProgressivoGIAS_DESTINAZIONE As String,
        lPivaSuperUser_ORIGINE As String,
        lPivaSuperUser_DESTINAZIONE As String,
        lUsernameSuperUser_Origine As String,
        lUsernameSuperUser_Destinazione As String,
        lImport_CodFiscale_ORIGINE As String,
        lImport_CodFiscale_DESTINAZIONE As String,
        lImport_Username_ORIGINE As String,
        lImport_Username_DESTINAZIONE As String,
        lPercorsoConnessioni As String
     ) As XNode

        Return <configurazione>


                   <urlWsimportaGias><%= urlWsimportaGias %></urlWsimportaGias>

                   <connessioni>
                       <connessione>
                           <Connessione_SERVER_ORIGINE><%= lConnessione_SERVER_ORIGINE %></Connessione_SERVER_ORIGINE>
                           <Connessione_SERVER_DESTINAZIONE><%= lConnessione_SERVER_DESTINAZIONE %></Connessione_SERVER_DESTINAZIONE>
                           <Connessione_UTENTI_ORIGINE><%= lConnessione_UTENTI_ORIGINE %></Connessione_UTENTI_ORIGINE>
                           <Connessione_UTENTI_DESTINAZIONE><%= lConnessione_UTENTI_DESTINAZIONE %></Connessione_UTENTI_DESTINAZIONE>
                       </connessione>
                   </connessioni>

                   <!-- coldiretti-->
                   <parametri>
                       <ProgressivoGIAS_ORIGINE><%= lProgressivoGIAS_ORIGINE %></ProgressivoGIAS_ORIGINE>
                       <ProgressivoGIAS_DESTINAZIONE><%= lProgressivoGIAS_DESTINAZIONE %></ProgressivoGIAS_DESTINAZIONE>
                       <PivaSuperUser_ORIGINE><%= lPivaSuperUser_ORIGINE %></PivaSuperUser_ORIGINE>
                       <PivaSuperUser_DESTINAZIONE><%= lPivaSuperUser_DESTINAZIONE %></PivaSuperUser_DESTINAZIONE>
                       <UsernameSuperUser_Origine><%= lUsernameSuperUser_Origine %></UsernameSuperUser_Origine>
                       <UsernameSuperUser_Destinazione><%= lUsernameSuperUser_Destinazione %></UsernameSuperUser_Destinazione>
                       <Import_CodFiscale_ORIGINE><%= lImport_CodFiscale_ORIGINE %></Import_CodFiscale_ORIGINE>
                       <Import_CodFiscale_DESTINAZIONE><%= lImport_CodFiscale_DESTINAZIONE %></Import_CodFiscale_DESTINAZIONE>
                       <Import_Username_ORIGINE><%= lImport_Username_ORIGINE %></Import_Username_ORIGINE>
                       <Import_Username_DESTINAZIONE><%= lImport_Username_DESTINAZIONE %></Import_Username_DESTINAZIONE>
                       <percorsoConnessioni><%= lPercorsoConnessioni %></percorsoConnessioni>
                   </parametri>
               </configurazione>
    End Function

    ''' <summary>
    ''' formato = "piva|rag_soc|sa_Cod|destinazione|pivapadre|flagimporta_agenda...§piva|sa_cod..."
    ''' </summary>
    ''' <param name="cacheImprese"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function leggiNodoImpreseDataCache(ByVal cacheImprese As String) As String


        'da leggere via configurazione
        Dim xnodeImprese As XDocument = XDocument.Parse("<imprese xmlns=""http://G2G""></imprese>")
        Dim datiGlobali As XNode = <datiglobali>
                                       <flagimporta_note/>
                                       <flagimporta_profilazione/>
                                       <flagimporta_pianicampionamento/>
                                   </datiglobali>
        '<flagimporta_cac_codifica/>
        'fine da leggere via configurazione

        xnodeImprese.Root.Add(datiGlobali)


        Return xnodeImprese.ToString
    End Function



    Public Function GeneraXmlDaImportare( _
                ByVal includiCentriAziendali As Boolean, _
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                Optional ByVal xFiltroAggiuntivo As String = "", _
                Optional ByVal SoloDatiNonImportatiGrafica As Boolean = False, _
                Optional ByVal includiRicette As Boolean = False _
        ) As String


        Dim xdoc As XDocument = XDocument.Parse("<dati xmlns=""http://G2G""></dati>")

        Dim nodoConfigurazione As XNode =
              <configurazione>

                  <connessioni>
                      <connessione>
                          <Connessione_SERVER_ORIGINE>cnG2Gserver_BO_Origine</Connessione_SERVER_ORIGINE>
                          <Connessione_SERVER_DESTINAZIONE>cnG2Gserver_NAZ_Destinazione</Connessione_SERVER_DESTINAZIONE>
                          <Connessione_UTENTI_ORIGINE>cnG2GUtenti_BO_Origine</Connessione_UTENTI_ORIGINE>
                          <Connessione_UTENTI_DESTINAZIONE>cnG2GUtenti_NAZ_Destinazione</Connessione_UTENTI_DESTINAZIONE>
                      </connessione>
                  </connessioni>

                  <!-- coldiretti-->
                  <parametri>
                      <ProgressivoGIAS_ORIGINE>564</ProgressivoGIAS_ORIGINE>
                      <ProgressivoGIAS_DESTINAZIONE>651</ProgressivoGIAS_DESTINAZIONE>
                      <PivaSuperUser_ORIGINE>05390270014</PivaSuperUser_ORIGINE>
                      <PivaSuperUser_DESTINAZIONE>05644051004</PivaSuperUser_DESTINAZIONE>
                      <UsernameSuperUser_Origine>ColdirettiER</UsernameSuperUser_Origine>
                      <UsernameSuperUser_Destinazione>coldirettinazionale</UsernameSuperUser_Destinazione>
                      <Import_CodFiscale_ORIGINE>05644051004</Import_CodFiscale_ORIGINE>
                      <Import_CodFiscale_DESTINAZIONE>import00L00L000L</Import_CodFiscale_DESTINAZIONE>
                      <Import_Username_ORIGINE>coldirettinazionale</Import_Username_ORIGINE>
                      <Import_Username_DESTINAZIONE>importatoreDati</Import_Username_DESTINAZIONE>
                      <percorsoConnessioni>C:\agroconnessioni\connessioni.ini</percorsoConnessioni>
                  </parametri>

              </configurazione>



        Dim leggiImprese As New AgronicaCoreG2GLocalDal.G2GLocal_R

        Dim nodoDettImprese As String = leggiImprese.LeggiImpreseXML(includiCentriAziendali, objParametri, xFiltroAggiuntivo, SoloDatiNonImportatiGrafica)
        If includiRicette Then
            nodoDettImprese = nodoDettImprese.Replace("</imprese>", "<impresa><origine></origine><rag_soc></rag_soc><sa_cod>0</sa_cod><destinazione></destinazione><padre><impresa><piva>00730120268</piva><livello>2</livello></impresa></padre><Livello>0</Livello><flagimporta_agenda></flagimporta_agenda><flagimporta_distinta></flagimporta_distinta><flagimporta_pianocolturale></flagimporta_pianocolturale><flagimporta_gis></flagimporta_gis></impresa></imprese>")
        End If

        Dim nodoImprese As XElement = XElement.Parse(nodoDettImprese)


        xdoc.Root.Add(nodoConfigurazione)
        xdoc.Root.Add(nodoImprese)

        Dim listaNS As New List(Of String)
        listaNS.Add("http://")


        Return RemoveNamespace(xdoc, listaNS).ToString.Replace("xmlns=""""", "")


    End Function

    Public Shared Function getImprese(ByVal imprese As XDocument) As List(Of clsImpresa)
        Dim listOfDataImport As List(Of clsImpresa) = _
            (From d In imprese.<dati>.<imprese>.<impresa> _
             Select New clsImpresa With { _
                 .Piva_ORIGINE = d.<origine>.Value, _
                 .Piva_DESTINAZIONE = d.<destinazione>.Value, _
                 .PivaPadre_DESTINAZIONE = d.<padre>.Value, _
                 .RagioneSociale = d.<rag_soc>.Value, _
                 .Opzionale_Sa_Cod_Origine = CInt(d.<sa_cod>.Value) _
             }).ToList

        Return listOfDataImport
    End Function


#Region "Utility xml"
    Public Shared Function RemoveNamespace(xdoc As XDocument, listaDiNamespaceDaPreservare As List(Of String)) As XDocument

        If listaDiNamespaceDaPreservare Is Nothing Then
            listaDiNamespaceDaPreservare = New List(Of String)
        End If

        For Each e As XElement In xdoc.Root.Descendants()
            If e.Name.[Namespace] <> XNamespace.None AndAlso Not listaDiNamespaceDaPreservare.Contains(e.Name.NamespaceName.ToString) Then
                e.Name = XNamespace.None.GetName(e.Name.LocalName)
            End If
            If e.Attributes().Where(Function(a) a.IsNamespaceDeclaration OrElse a.Name.[Namespace] <> XNamespace.None).Any() Then
                e.ReplaceAttributes(e.Attributes().[Select](Function(a) If(a.IsNamespaceDeclaration, Nothing, If(a.Name.[Namespace] <> XNamespace.None, New XAttribute(XNamespace.None.GetName(a.Name.LocalName), a.Value), a))))
            End If
        Next
        Return xdoc
    End Function
#End Region

End Class
