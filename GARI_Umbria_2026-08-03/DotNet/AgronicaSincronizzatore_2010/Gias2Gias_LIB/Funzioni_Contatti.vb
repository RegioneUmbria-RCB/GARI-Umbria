Imports System.Xml
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports System.Text
Imports AgronicaCoreModello.Anagrafe
Imports AgronicaCoreModello
Imports AgronicaCoreG2GLocalDal
Imports AgronicaCoreUtility
Imports AgronicaCoreEntityFramework_POCO

Partial Public Class Funzioni

#Region "Contatti"

    Public Function LeggiContattiXML(ByVal Piva_Origine As String,
                                     ByVal Piva_Destinazione As String,
                                     ByRef objOpzioni As clsOpzioni,
                                     ByVal Flag_Pubblico As Boolean,
                                     ByVal Configurazione As G2G_Configurazione_FiltriReq_Contatti,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                     ByRef listaImpreseContatti As List(Of String)) As String

        Dim From_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_ORIGINE
        Dim To_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_DESTINAZIONE
        Dim From_Piva = Piva_Origine
        Dim To_Piva = Piva_Destinazione

        ' per i contatti pubblici forzo la piva destinazione
        'If String.IsNullOrEmpty(Piva_Destinazione) Then
        '    To_Piva = To_PivaSuperUser
        'End If

        Dim listaImprese = New List(Of String)
        Dim listaCodRapporto = New List(Of Integer)
        Dim listaImpreseIndirizzi As New List(Of String)
        Dim dataValidita = AGRODATAINIZIO

        If Configurazione IsNot Nothing Then
            listaImprese = Configurazione.listaImprese
            listaCodRapporto = Configurazione.listaCodRapporto
            dataValidita = Configurazione.dataValidita
        End If

        Dim objLeggi As New G2GContatti_R
        Dim g2g = objLeggi.Nuovo_Contatti_G2G(From_PivaSuperUser, To_PivaSuperUser, From_Piva, To_Piva)
        Dim leggi = objLeggi.Leggi_Contatti_G2G(From_Piva, Flag_Pubblico, listaImprese, listaImpreseIndirizzi, listaCodRapporto, dataValidita, g2g, objParametri)

        ' se contatti pubblici restituisco la lista delle imprese relative
        If Flag_Pubblico Then
            listaImpreseContatti = (From c In g2g.ContattiToInsert Select c.Contatto.Piva).Distinct().ToList()
            For Each piva In listaImpreseIndirizzi
                If Not listaImpreseContatti.Contains(piva) Then
                    listaImpreseContatti.Add(piva)
                End If
            Next
        End If

        Return If(leggi, XMLUtility.SerializzaOggetto(Of G2G_Contatti)(g2g, ""), "")

    End Function

    Public Function LeggiContattiXMLReverse(ByVal Piva_Origine As String,
                                     ByVal Piva_Destinazione As String,
                                     ByRef objOpzioni As clsOpzioni,
                                     ByVal Flag_Pubblico As Boolean,
                                     ByVal Configurazione As G2G_Configurazione_FiltriReq_Contatti,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                     ByRef listaImpreseContatti As List(Of String)) As String

        Dim From_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_ORIGINE
        Dim To_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_DESTINAZIONE
        Dim From_Piva = Piva_Origine
        Dim To_Piva = Piva_Destinazione

        ' per i contatti pubblici forzo la piva destinazione
        'If String.IsNullOrEmpty(Piva_Destinazione) Then
        '    To_Piva = To_PivaSuperUser
        'End If

        Dim listaImprese = New List(Of String)
        Dim listaCodRapporto = New List(Of Integer)
        Dim listaImpreseIndirizzi As New List(Of String)
        Dim dataValidita = AGRODATAINIZIO

        If Configurazione IsNot Nothing Then
            listaImprese = Configurazione.listaImprese
            listaCodRapporto = Configurazione.listaCodRapporto
            dataValidita = Configurazione.dataValidita
        End If

        Dim objLeggi As New G2GContatti_R
        Dim g2g = objLeggi.Nuovo_Contatti_G2GReverse(From_PivaSuperUser, To_PivaSuperUser, From_Piva, To_Piva)
        Dim leggi = objLeggi.Leggi_Contatti_G2GReverse(From_Piva, Flag_Pubblico, listaImprese, listaImpreseIndirizzi, listaCodRapporto, dataValidita, g2g, objParametri)

        ' se contatti pubblici restituisco la lista delle imprese relative
        If Flag_Pubblico Then
            listaImpreseContatti = (From c In g2g.ContattiToInsert Select c.Contatto.Piva).Distinct().ToList()
            For Each piva In listaImpreseIndirizzi
                If Not listaImpreseContatti.Contains(piva) Then
                    listaImpreseContatti.Add(piva)
                End If
            Next
        End If

        Return If(leggi, XMLUtility.SerializzaOggetto(Of G2G_Contatti_Reverse)(g2g, ""), "")

    End Function

    Public Sub Elabora_XML_Contatti_Salva(
                            ByVal objOpzioni As clsOpzioni,
                            ByRef Log_Import As StringBuilder,
                            ByRef Log_Errori As StringBuilder,
                            ByRef Log_Riepilogo As StringBuilder,
                            ByVal Piva_Origine As String,
                            ByVal Piva_Destinazione As String,
                            ByVal Flag_Pubblico As Boolean,
                            ByVal Configurazione As G2G_Configurazione_FiltriReq_Contatti)

        Const NomeFunzione As String = "Elabora_XML_Contatti_Salva"
        Dim MessaggioErrore As String = ""
        Dim listaImpreseContatti As New List(Of String)

        Try

            ' leggo struttura dati serializzata su origine contenente i contatti azienda da inserire/modificare/cancellare 
            Dim sXmlContatti = LeggiContattiXML(Piva_Origine, Piva_Destinazione, objOpzioni, Flag_Pubblico, Configurazione, objOpzioni.objParametri_Server_GIAS_ORIGINE, listaImpreseContatti)

            ' scrivo dati serializzati richiamando web service destinazione
            Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS
            wsimportazione.Url = objOpzioni.wsimportaGiasURl
            wsimportazione.Timeout = _timeout_ws_Importa
            Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)

            ' creo l'azienza del contatto pubblico su destinazione se non esiste
            If Flag_Pubblico And listaImpreseContatti.Count > 0 Then
                For Each piva In listaImpreseContatti
                    If Not Configurazione.listaImprese.Contains(piva) Then
                        Dim strErr As String = ""
                        Dim pivaEsistente = wsimportazione.Verifica_EsistenzaPivaGIAS(Str_Credenziali_WS, piva, strErr)
                        If NuovaLogicaRecode Then
                            ScriviImpreseXML(
                                        objOpzioni,
                                        Log_Import,
                                        Log_Errori,
                                        Log_Riepilogo,
                                        piva,
                                        piva,
                                        objOpzioni.SuperUser_CodFiscale_DESTINAZIONE,
                                        True
                                    )
                        ElseIf Not pivaEsistente Then
                            Elabora_XML_Imprese_Salva(
                                        objOpzioni,
                                        Log_Import,
                                        Log_Errori,
                                        Log_Riepilogo,
                                        piva,
                                        piva,
                                        objOpzioni.SuperUser_CodFiscale_DESTINAZIONE,
                                        objOpzioni.SuperUser_CodFiscale_DESTINAZIONE,
                                        False
                                    )
                        End If
                    End If
                Next
            End If

            If Not String.IsNullOrEmpty(sXmlContatti) Then

                Dim mainDoc = Funzioni_MasterG2G.XmlMasterDataPiva(enum_TipoOperazioneDB.Scrittura, Piva_Origine, "", objOpzioni)
                mainDoc = XDocUtils.IniettaSottoAlberoDaStringaXml(mainDoc, sXmlContatti, "//utente/" & If(Flag_Pubblico, "DatiGlobali", "DatiImprese/Impresa"), "DatiContatti")
                Dim sXmlRisposta As String = wsimportazione.Scrivi_XmlFull(Str_Credenziali_WS, AgroZip.CompressioneBase64(1, mainDoc))

                ' leggo la struttura restituita contenente i recode da inserire/modificare/cancellare
                Dim xmlRisposta As XDocument = XDocument.Parse(sXmlRisposta)
                Dim xmlRispostaContatti As XElement = xmlRisposta.Element("Risposta").Element("Risposta_DatiContatti")
                Dim rispostaContatti = xmlRispostaContatti.FirstNode.ToString
                'Dim rispostaContatti = AgroZip.DeCompressioneBase64(1, wsimportazione.Scrivi_ContattiXmlPrivato(Str_Credenziali_WS, AgroZip.CompressioneBase64(1, sXmlContatti)))
                Dim risposta As G2G_Recode = XMLUtility.DeserializzaOggetto(Of G2G_Recode)(rispostaContatti, "")

                ' se la scrittura su destinazione è andata a buon fine aggiorno i recode sull'origine
                If String.IsNullOrEmpty(risposta.MessaggioErrore) Then
                    MessaggioErrore = G2GUtility.Scrivi_G2G_Recode(risposta, objOpzioni.objParametri_Server_GIAS_ORIGINE)
                Else
                    MessaggioErrore = risposta.MessaggioErrore
                End If

                ' log trasferimento dati
                If String.IsNullOrEmpty(MessaggioErrore) Then
                    G2GUtility.Log(Log_Import, "Trasferimento contatti: " & risposta.LogRecode)
                Else
                    G2GUtility.Log(Log_Import, "Trasferimento contatti ERRORE: " & MessaggioErrore)
                End If

            Else

                G2GUtility.Log(Log_Import, "Trasferimento contatti: nessun dato da trasferire")

            End If

        Catch ex As Exception
            Dim msg As String
            msg = CStr(Date.Now) + " - " + NomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            If ex.InnerException IsNot Nothing Then
                msg &= "InnerException:" & ex.InnerException.Message
            End If
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try

    End Sub

    Public Sub Elabora_XML_Contatti_SalvaReverse(
                            ByVal objOpzioni As clsOpzioni,
                            ByRef Log_Import As StringBuilder,
                            ByRef Log_Errori As StringBuilder,
                            ByRef Log_Riepilogo As StringBuilder,
                            ByVal Piva_Origine As String,
                            ByVal Piva_Destinazione As String,
                            ByVal Flag_Pubblico As Boolean,
                            ByVal Configurazione As G2G_Configurazione_FiltriReq_Contatti)

        Const NomeFunzione As String = "Elabora_XML_Contatti_SalvaReverse"
        Dim MessaggioErrore As String = ""
        Dim listaImpreseContatti As New List(Of String)

        Try

            ' leggo struttura dati serializzata su origine contenente i contatti azienda da inserire/modificare/cancellare 
            Dim sXmlContatti = LeggiContattiXMLReverse(Piva_Origine, Piva_Destinazione, objOpzioni, Flag_Pubblico, Configurazione, objOpzioni.objParametri_Server_GIAS_ORIGINE, listaImpreseContatti)

            ' scrivo dati serializzati richiamando web service destinazione
            Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS
            wsimportazione.Url = objOpzioni.wsimportaGiasURl
            wsimportazione.Timeout = _timeout_ws_Importa
            Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)

            ' creo l'azienza del contatto pubblico su destinazione se non esiste
            If Flag_Pubblico And listaImpreseContatti.Count > 0 Then
                For Each piva In listaImpreseContatti
                    If Not Configurazione.listaImprese.Contains(piva) Then
                        Dim strErr As String = ""
                        Dim pivaEsistente = wsimportazione.Verifica_EsistenzaPivaGIAS(Str_Credenziali_WS, piva, strErr)
                        If NuovaLogicaRecode Then
                            ScriviImpreseXML_Reverse(
                                        objOpzioni,
                                        Log_Import,
                                        Log_Errori,
                                        Log_Riepilogo,
                                        piva,
                                        piva,
                                        objOpzioni.SuperUser_CodFiscale_DESTINAZIONE,
                                        True
                                    )
                        ElseIf Not pivaEsistente Then
                            Throw New Exception("Vecchia logica recode non supportata")
                        End If
                    End If
                Next
            End If

            If Not String.IsNullOrEmpty(sXmlContatti) Then

                Dim mainDoc = Funzioni_MasterG2G.XmlMasterDataPiva(enum_TipoOperazioneDB.Scrittura, Piva_Origine, "", objOpzioni)
                mainDoc = XDocUtils.IniettaSottoAlberoDaStringaXml(mainDoc, sXmlContatti, "//utente/" & If(Flag_Pubblico, "DatiGlobali", "DatiImprese/Impresa"), "DatiContatti")
                Dim sXmlRisposta As String = wsimportazione.Scrivi_XmlFull(Str_Credenziali_WS, AgroZip.CompressioneBase64(1, mainDoc))

                ' leggo la struttura restituita contenente i recode da inserire/modificare/cancellare
                Dim xmlRisposta As XDocument = XDocument.Parse(sXmlRisposta)
                Dim xmlRispostaContatti As XElement = xmlRisposta.Element("Risposta").Element("Risposta_DatiContatti")
                Dim rispostaContatti = xmlRispostaContatti.FirstNode.ToString
                'Dim rispostaContatti = AgroZip.DeCompressioneBase64(1, wsimportazione.Scrivi_ContattiXmlPrivato(Str_Credenziali_WS, AgroZip.CompressioneBase64(1, sXmlContatti)))
                Dim risposta As G2G_Recode = XMLUtility.DeserializzaOggetto(Of G2G_Recode)(rispostaContatti, "")

                ' se la scrittura su destinazione è andata a buon fine aggiorno i recode sull'origine
                If String.IsNullOrEmpty(risposta.MessaggioErrore) Then
                    MessaggioErrore = G2GUtility.Scrivi_G2G_Recode(risposta, objOpzioni.objParametri_Server_GIAS_ORIGINE)
                Else
                    MessaggioErrore = risposta.MessaggioErrore
                End If

                ' log trasferimento dati
                If String.IsNullOrEmpty(MessaggioErrore) Then
                    G2GUtility.Log(Log_Import, "Trasferimento contatti: " & risposta.LogRecode)
                Else
                    G2GUtility.Log(Log_Import, "Trasferimento contatti ERRORE: " & MessaggioErrore)
                End If

            Else

                G2GUtility.Log(Log_Import, "Trasferimento contatti: nessun dato da trasferire")

            End If

        Catch ex As Exception
            Dim msg As String
            msg = CStr(Date.Now) + " - " + NomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            If ex.InnerException IsNot Nothing Then
                msg &= "InnerException:" & ex.InnerException.Message
            End If
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try

    End Sub

    '###################################################################################
    Public Sub Elabora_XML_Contatto_Salva(ByVal objOpzioni As clsOpzioni,
                                            ByRef Log_Import As StringBuilder,
                                            ByRef Log_Errori As StringBuilder,
                                            ByRef Log_Riepilogo As StringBuilder,
                                            ByVal Piva_Origine As String,
                                            ByVal Cod_contatto As String,
                                            ByVal Piva_Destinazione As String)

        Const NomeFunzione As String = "Elabora_XML_Contatto_Salva"
        'Dim Cod_Risum_DESTINAZIONE As Integer = 0

        Try

            'Dim objContInd_R As New AgronicaCoreAnagrafeDAL.ContattiXIndirizzi_R
            Dim objRisUm As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R
            Dim DT_RisUm_ORIGINE As DataTable
            Dim DT_RisUm_DESTINAZIONE As DataTable

            'verifica se il contatto è presente nel db DESTINAZIONE 
            'controllo sulla piva di destinazione
            DT_RisUm_DESTINAZIONE = objRisUm.LeggiSoloContatto(
                                     Piva_Destinazione,
                                     0,
                                     Cod_contatto,
                                     0,
                                     0,
                                     "",
                                     True,
                                     AGRODATAINIZIO, AGRODATAFINE,
                                     enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                     "", "",
                                     objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)

            If Not IsNothing(DT_RisUm_DESTINAZIONE) AndAlso DT_RisUm_DESTINAZIONE.Rows.Count > 0 Then
                'il contatto è presente
            Else
                '- non c'è: salva il contatto e ritorna codice

                Dim objContatti_R As New AgronicaCoreAnagrafeBIZ.Contatti_MultiHost_R
                Dim objContatti_W As New AgronicaCoreAnagrafeBIZ.Contatti_W
                Dim Str_XML_Contatto As String
                Dim flag_insert As Boolean
                Dim OUTPUT_Piva As String = ""
                Dim OUTPUT_Cod_Contatto As String = ""

                Str_XML_Contatto = objContatti_R.Contatto_Leggi(Piva_Origine,
                                                                Cod_contatto,
                                                                "",
                                                                False,
                                                                objOpzioni.objParametri_Server_GIAS_ORIGINE)

                If Str_XML_Contatto <> "" Then
                    Str_XML_Contatto =
                        Elabora_XML_Contatto_SistemaXml(objOpzioni,
                            Log_Import,
                            Log_Errori,
                            Log_Riepilogo,
                            objOpzioni.objParametri_Server_GIAS_ORIGINE,
                            objOpzioni.objParametri_Utenti_GIAS_ORIGINE,
                            objOpzioni.objParametri_Server_GIAS_DESTINAZIONE,
                            objOpzioni.objParametri_Utenti_GIAS_DESTINAZIONE,
                            Str_XML_Contatto,
                            Piva_Destinazione)

                    flag_insert = objContatti_W.Contatto_Scrivi(Str_XML_Contatto,
                                                                OUTPUT_Piva,
                                                                OUTPUT_Cod_Contatto,
                                                                objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)

                    'leggo i cod_risum del contatto appena inserito
                    DT_RisUm_DESTINAZIONE = objRisUm.LeggiSoloContatto(
                                                                 OUTPUT_Piva,
                                                                 0,
                                                                 OUTPUT_Cod_Contatto,
                                                                 0,
                                                                 0,
                                                                 "",
                                                                 True,
                                                                 AGRODATAINIZIO, AGRODATAFINE,
                                                                 enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                 "", "",
                                                                 objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)
                End If

            End If

            'controlla se nel db di destinazione ci sono gli stessi cod_rapporto
            'in caso negativo li aggiunge
            'e salva il mapping dei cod_risum

            DT_RisUm_ORIGINE = objRisUm.LeggiSoloContatto(
                                     Piva_Origine,
                                     0,
                                     Cod_contatto,
                                     0,
                                     0,
                                     "",
                                     True,
                                     AGRODATAINIZIO, AGRODATAFINE,
                                     enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                     "", "",
                                     objOpzioni.objParametri_Server_GIAS_ORIGINE)

            If Not IsNothing(DT_RisUm_ORIGINE) AndAlso DT_RisUm_ORIGINE.Rows.Count > 0 Then
                Dim i As Integer
                Dim Cod_Risum_ORIGINE, Cod_Risum_DESTINAZIONE, cod_rapporto As Integer
                Dim DR_Destinazione() As DataRow
                For i = 0 To DT_RisUm_ORIGINE.Rows.Count - 1

                    Cod_Risum_ORIGINE = DT_RisUm_ORIGINE.Rows(i).Item("Cod_RisUm")
                    cod_rapporto = DT_RisUm_ORIGINE.Rows(i).Item("cod_rapporto")

                    DR_Destinazione = DT_RisUm_DESTINAZIONE.Select("cod_rapporto =" + Agro_SQL_SaveNum(cod_rapporto))
                    If Not IsNothing(DR_Destinazione) AndAlso DR_Destinazione.Length > 0 Then
                        Cod_Risum_DESTINAZIONE = DR_Destinazione(0).Item("Cod_RisUm")
                    Else
                        'va aggiunta la risorsa umana con quel cod_rapporto!
                        'da fare!
                    End If

                    If Cod_Risum_DESTINAZIONE = 0 Then
                        Throw New Exception("Cod_Risum_DESTINAZIONE = 0 ")
                    End If

                    FunzioniGLOBAL.ContattiADD(New _
                                  G2G_Recode_Contatti With {
                                      .From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser,
                                      .To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser,
                                      .From_Cod_RisUm = Cod_Risum_ORIGINE,
                                      .To_Cod_Risum = Cod_Risum_DESTINAZIONE}
                                  )

                Next

            End If

        Catch ex As Exception
            Dim msg As String
            msg = CStr(Date.Now) + " - " + NomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try

        '   Return Cod_Risum_DESTINAZIONE

    End Sub


    '###################################################################################
    Private Function Elabora_XML_Contatto_SistemaXml(ByVal objOpzioni As clsOpzioni,
                                                    ByRef Log_Import As StringBuilder,
                                                    ByRef Log_Errori As StringBuilder,
                                                    ByRef Log_Riepilogo As StringBuilder,
                                                    ByRef objParametri_Server_GIAS_Origine As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                    ByRef objParametri_Utenti_GIAS_Origine As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                    ByRef objParametri_Server_GIAS_Destinazione As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                    ByRef objParametri_Utenti_GIAS_Destinazione As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                    ByRef Str_XML_Contatto As String,
                                                    ByVal Piva_Destinazione As String) As String

        Const NomeFunzione As String = "Elabora_XML_Contatto_SistemaXml"

        Dim Str_XML_Output As String = ""

        Dim XMLs_Nodi As XmlNodeList
        Dim XML_Nodo As XmlElement
        Dim i As Integer

        Try

            Dim XmlDocCont As New XmlDocument
            XmlDocCont.LoadXml(Str_XML_Contatto)

            'tipo operazione = scrittura
            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@TipoOperazioneDB]")
            For i = 0 To XMLs_Nodi.Count - 1
                XML_Nodo = XMLs_Nodi(i)
                XML_Nodo.SetAttribute("TipoOperazioneDB", enum_TipoOperazioneDB.Scrittura)
                If Piva_Destinazione <> "" Then
                    XML_Nodo.SetAttribute("piva", Piva_Destinazione)
                End If
            Next

            'basecode e topcode su tutti gli elementi
            XMLs_Nodi = XmlDocCont.SelectNodes("//*")
            For i = 0 To XMLs_Nodi.Count - 1
                XML_Nodo = XMLs_Nodi.Item(i)
                XML_Nodo.SetAttribute("basecode", objOpzioni.BaseCode_DESTINAZIONE)
                XML_Nodo.SetAttribute("topcode", objOpzioni.TopCode_DESTINAZIONE)
                If Piva_Destinazione <> "" Then
                    XML_Nodo.SetAttribute("piva", Piva_Destinazione)
                End If
            Next

            'azzeramento cod_indirizzo
            XMLs_Nodi = XmlDocCont.SelectNodes("//Indirizzo")
            For i = 0 To XMLs_Nodi.Count - 1
                XML_Nodo = XMLs_Nodi.Item(i)
                XML_Nodo.SetAttribute("cod_indirizzo", 0)
                If Piva_Destinazione <> "" Then
                    XML_Nodo.SetAttribute("piva", Piva_Destinazione)
                End If
            Next

            'azzeramento cod_rubrica
            XMLs_Nodi = XmlDocCont.SelectNodes("//Rubrica")
            For i = 0 To XMLs_Nodi.Count - 1
                XML_Nodo = XMLs_Nodi.Item(i)
                XML_Nodo.SetAttribute("cod_rubrica", 0)
            Next

            'azzeramento cod_risum
            XMLs_Nodi = XmlDocCont.SelectNodes("//RapCon")
            For i = 0 To XMLs_Nodi.Count - 1
                XML_Nodo = XMLs_Nodi.Item(i)
                XML_Nodo.SetAttribute("cod_risum", 0)
                If Piva_Destinazione <> "" Then
                    XML_Nodo.SetAttribute("piva", Piva_Destinazione)
                End If
            Next

            Str_XML_Output = XmlDocCont.OuterXml

        Catch ex As Exception
            Dim msg As String
            msg = CStr(Date.Now) + " - " + NomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try

        Return Str_XML_Output


    End Function

    Public Function recode_return_CodRisUm(ByVal oldCod_RisUm As Integer, ByVal Piva_SuperUser_Destinazione As String) As Integer

        'viene effettuata di nuovo la verifica perchè questa funzione viene chiamata senza "controlli", quindi può arrivare "Zero"
        If oldCod_RisUm = 0 Then
            Return 0
        End If

        Dim risumDestinazione = If(_efG2G Is Nothing,
            (
                From c In FunzioniGLOBAL.Contatti
                Where c.From_Cod_RisUm = oldCod_RisUm _
                    And c.To_PivaSuperUser = Piva_SuperUser_Destinazione
                Select c.To_Cod_Risum).FirstOrDefault,
            (
                From c In _efG2G.G2G_Recode_Contatti
                Where c.From_Cod_RisUm = oldCod_RisUm _
                    And c.To_PivaSuperUser = Piva_SuperUser_Destinazione
                Select c.To_Cod_Risum).FirstOrDefault)

        If risumDestinazione = 0 Then
            Dim messaggio As String
            messaggio = "cod_Risum in origine = " & CStr(oldCod_RisUm) & vbCrLf & " Errore, non esiste una decodifica del cod_Risum, funzione recode_return_CodRisUm."
            Throw New Exception(messaggio)
        End If

        Return risumDestinazione
    End Function


    Public Function recode_return_CodRisUmReverse(ByVal oldCod_RisUm As Integer, ByVal Piva_SuperUser_Destinazione As String) As Integer

        'viene effettuata di nuovo la verifica perchè questa funzione viene chiamata senza "controlli", quindi può arrivare "Zero"
        If oldCod_RisUm = 0 Then
            Return 0
        End If

        Dim risumDestinazione = If(_efG2G Is Nothing,
            (
                From c In FunzioniGLOBAL.Contatti
                Where c.To_Cod_Risum = oldCod_RisUm _
                    And c.From_PivaSuperUser = Piva_SuperUser_Destinazione
                Select c.From_Cod_RisUm).FirstOrDefault,
            (
                From c In _efG2G.G2G_Recode_Contatti
                Where c.To_Cod_Risum = oldCod_RisUm _
                    And c.From_PivaSuperUser = Piva_SuperUser_Destinazione
                Select c.From_Cod_RisUm).FirstOrDefault)

        If risumDestinazione = 0 Then
            Dim messaggio As String
            messaggio = "cod_Risum in origine = " & CStr(oldCod_RisUm) & vbCrLf & " Errore, non esiste una decodifica del cod_Risum, funzione recode_return_CodRisUm."
            Throw New Exception(messaggio)
        End If

        Return risumDestinazione
    End Function

#End Region



End Class
