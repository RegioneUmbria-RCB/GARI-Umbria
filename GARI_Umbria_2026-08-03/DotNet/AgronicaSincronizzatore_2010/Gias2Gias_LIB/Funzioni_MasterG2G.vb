


Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class Funzioni_MasterG2G

    Public Shared Function XmlMasterDataPiva(
        ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
        ByVal piva As String,
        ByVal RagSoc As String,
        ByVal objOpzioni As clsOpzioni) As String

        Dim iTipoOperazioneDB As Integer = TipoOperazioneDB

        Dim x = <utente username="" password="">
                    <DatiGlobali>

                    </DatiGlobali>
                    <DatiImprese>

                    </DatiImprese>
                </utente>

        If piva <> "" Then

            Dim p = <Impresa TipoOperazioneDB=<%= iTipoOperazioneDB %> piva=<%= piva %> rag_soc=<%= RagSoc %> tipoimpresagerarchia="1" validita_inizio="01/01/1900" validita_fine="31/12/2100">

                    </Impresa>

            x.Element("DatiImprese").AddFirst(p)

        End If

        Dim configurazione As String =
                    Funzioni_MasterG2G.ImpostaNodoConfigurazione(objOpzioni)

        Dim mainDoc As String = x.ToString()
        mainDoc = AgronicaCoreUtility.XDocUtils.IniettaSottoAlberoDaStringaXml(mainDoc, configurazione, "//utente", "")


        Return mainDoc

    End Function


    Public Shared Function XmlMasterDataRecodes(
        ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
        ByVal objOpzioni As clsOpzioni) As String

        Dim iTipoOperazioneDB As Integer = TipoOperazioneDB

        Dim x = <utente username="" password="">
                    <G2G_Recodes>

                    </G2G_Recodes>
                </utente>

        Dim configurazione As String =
                    Funzioni_MasterG2G.ImpostaNodoConfigurazione(objOpzioni)

        Dim mainDoc As String = x.ToString()
        mainDoc = AgronicaCoreUtility.XDocUtils.IniettaSottoAlberoDaStringaXml(mainDoc, configurazione, "//utente", "")


        Return mainDoc

    End Function

    Public Shared Function ImpostaNodoConfigurazione(objOpzioni As clsOpzioni) As String
        Dim nodoConfigurazione As XNode = ImpostaNodoConfigurazione(
                           urlWsimportaGias:="",
                           lConnessione_SERVER_ORIGINE:="",
                           lConnessione_SERVER_DESTINAZIONE:="",
                           lConnessione_UTENTI_ORIGINE:="",
                           lConnessione_UTENTI_DESTINAZIONE:="",
                           lProgressivoGIAS_ORIGINE:=objOpzioni.ProgressivoGIAS_ORIGINE,
                           lProgressivoGIAS_DESTINAZIONE:=objOpzioni.ProgressivoGIAS_DESTINAZIONE,
                           lPivaSuperUser_ORIGINE:=objOpzioni.SuperUser_CodFiscale_ORIGINE,
                           lPivaSuperUser_DESTINAZIONE:=objOpzioni.SuperUser_CodFiscale_DESTINAZIONE,
                           lUsernameSuperUser_Origine:=objOpzioni.SuperUser_Username_ORIGINE,
                           lUsernameSuperUser_Destinazione:=objOpzioni.SuperUser_Username_DESTINAZIONE,
                           lImport_CodFiscale_ORIGINE:=objOpzioni.Import_CodFiscale_ORIGINE,
                           lImport_CodFiscale_DESTINAZIONE:=objOpzioni.Import_CodFiscale_DESTINAZIONE,
                           lImport_Username_ORIGINE:=objOpzioni.Import_Username_ORIGINE,
                           lImport_Username_DESTINAZIONE:=objOpzioni.Import_Username_DESTINAZIONE,
                           ltxtImport_CodiceImpresa_ORIGINE:=objOpzioni.CodiceImpresa_ORIGINE,
                           ltxtImport_CodiceImpresa_DESTINAZIONE:=objOpzioni.CodiceImpresa_DESTINAZIONE,
                           ImpostazioniTrasformazioni:=objOpzioni.ImpostazioniTrasformazioni,
                           lPercorsoConnessioni:=objOpzioni.PercorsoConnessioni
                        )

        Dim configurazione As String = nodoConfigurazione.ToString()
        Return configurazione
    End Function

    Public Shared Function ImpostaNodoConfigurazione(
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
        ltxtImport_CodiceImpresa_ORIGINE As String,
        ltxtImport_CodiceImpresa_DESTINAZIONE As String,
        ImpostazioniTrasformazioni As String,
        lPercorsoConnessioni As String
     ) As XNode


        Dim elementoConfigurazione As XElement =
            <configurazione>


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
                    <ImpostazioniTrasformazioni><%= ImpostazioniTrasformazioni %></ImpostazioniTrasformazioni>
                    <percorsoConnessioni><%= lPercorsoConnessioni %></percorsoConnessioni>
                </parametri>
            </configurazione>

        If ltxtImport_CodiceImpresa_ORIGINE <> "" Then
            Dim xnOrigin = <CodiceImpresa_ORIGINE><%= ltxtImport_CodiceImpresa_ORIGINE %></CodiceImpresa_ORIGINE>
            elementoConfigurazione.Element("parametri").Add(xnOrigin)
        End If

        If ltxtImport_CodiceImpresa_DESTINAZIONE <> "" Then
            Dim xnDest = <CodiceImpresa_DESTINAZIONE><%= ltxtImport_CodiceImpresa_DESTINAZIONE %></CodiceImpresa_DESTINAZIONE>
            elementoConfigurazione.Element("parametri").Add(xnDest)
        End If

        Return elementoConfigurazione

    End Function


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="xElemCfg"></param>
    ''' <param name="NamespacePerLettura">da impostare se esiste un namespace http://G2G</param>
    ''' <returns></returns>
    Public Shared Function LeggiOpzioniDaXml(xElemCfg As XElement, NamespacePerLettura As String) As clsOpzioni

        Dim xN As XNamespace = NamespacePerLettura
        Dim opt As New clsOpzioni
        Dim o As XElement = xElemCfg.Element(xN + "configurazione")

        Dim o1 = o.Element(xN + "urlWsimportaGias")

        With opt
            .wsimportaGiasURl = o.Element(xN + "urlWsimportaGias").Value
            .Connessione_Server_GIAS_Origine = o.Element(xN + "connessioni").Element(xN + "connessione").Element(xN + "Connessione_SERVER_ORIGINE").Value
            .Connessione_Server_GIAS_Destinazione = o.Element(xN + "connessioni").Element(xN + "connessione").Element(xN + "Connessione_SERVER_DESTINAZIONE").Value
            .Connessione_Utenti_GIAS_Origine = o.Element(xN + "connessioni").Element(xN + "connessione").Element(xN + "Connessione_UTENTI_ORIGINE").Value
            .Connessione_Utenti_GIAS_Destinazione = o.Element(xN + "connessioni").Element(xN + "connessione").Element(xN + "Connessione_UTENTI_DESTINAZIONE").Value
            .ProgressivoGIAS_ORIGINE = o.Element(xN + "parametri").Element(xN + "ProgressivoGIAS_ORIGINE").Value
            .ProgressivoGIAS_DESTINAZIONE = o.Element(xN + "parametri").Element(xN + "ProgressivoGIAS_DESTINAZIONE").Value
            .PercorsoConnessioni = o.Element(xN + "parametri").Element(xN + "percorsoConnessioni").Value
            .SuperUser_CodFiscale_ORIGINE = o.Element(xN + "parametri").Element(xN + "PivaSuperUser_ORIGINE").Value
            .SuperUser_CodFiscale_DESTINAZIONE = o.Element(xN + "parametri").Element(xN + "PivaSuperUser_DESTINAZIONE").Value
            .SuperUser_Username_ORIGINE = o.Element(xN + "parametri").Element(xN + "UsernameSuperUser_Origine").Value
            .SuperUser_Username_DESTINAZIONE = o.Element(xN + "parametri").Element(xN + "UsernameSuperUser_Destinazione").Value
            .Import_CodFiscale_ORIGINE = o.Element(xN + "parametri").Element(xN + "Import_CodFiscale_ORIGINE").Value
            .Import_CodFiscale_DESTINAZIONE = o.Element(xN + "parametri").Element(xN + "Import_CodFiscale_DESTINAZIONE").Value
            .Import_Username_ORIGINE = o.Element(xN + "parametri").Element(xN + "Import_Username_ORIGINE").Value
            .Import_Username_DESTINAZIONE = o.Element(xN + "parametri").Element(xN + "Import_Username_DESTINAZIONE").Value

            Dim xElementCodImpresaOrigine As XElement = o.Element(xN + "parametri").Element(xN + "CodiceImpresa_ORIGINE")

            If xElementCodImpresaOrigine Is Nothing Then
                .CodiceImpresa_ORIGINE = 0
            Else
                .CodiceImpresa_ORIGINE = xElementCodImpresaOrigine.Value
            End If


            Dim xElementCodImpresaDestinazione As XElement = o.Element(xN + "parametri").Element(xN + "CodiceImpresa_DESTINAZIONE")
            If xElementCodImpresaDestinazione Is Nothing Then
                .CodiceImpresa_DESTINAZIONE = 0
            Else
                .CodiceImpresa_DESTINAZIONE = xElementCodImpresaDestinazione.Value
            End If

            Dim xElementCodImpostazioniTrasformazioni As XElement = o.Element(xN + "parametri").Element(xN + "ImpostazioniTrasformazioni")
            If xElementCodImpostazioniTrasformazioni Is Nothing Then
                .ImpostazioniTrasformazioni = ""
            Else
                .ImpostazioniTrasformazioni = xElementCodImpostazioniTrasformazioni.Value
            End If

            Dim xElementCodTimeOut_WS_Importa As XElement = o.Element(xN + "parametri").Element(xN + "TimeOut_WS_Importa")
            If xElementCodTimeOut_WS_Importa Is Nothing Then
                .TimeOut_Chiamata_WS = 1800000
            Else
                .TimeOut_Chiamata_WS = xElementCodTimeOut_WS_Importa.Value
            End If

        End With

        Return opt

    End Function
End Class
