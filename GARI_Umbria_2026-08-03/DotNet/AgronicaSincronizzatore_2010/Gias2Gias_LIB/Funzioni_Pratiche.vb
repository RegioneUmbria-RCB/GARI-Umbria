Imports System.Xml
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports System.Text
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModello.Anagrafe
Imports AgronicaCoreModello
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreUtility
Imports AgronicaCoreG2GLocalDal
Imports System.Data.Objects
Imports Newtonsoft.Json

Partial Public Class Funzioni


#Region "Pratiche"

    Public Sub Elabora_XML_Pratiche_Salva(
                            ByVal objOpzioniImportImpresa As clsImpresa,
                            ByVal efG2G As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                            ByVal objOpzioni As clsOpzioni,
                            ByRef Log_Import As StringBuilder,
                            ByRef Log_Errori As StringBuilder,
                            ByRef Log_Riepilogo As StringBuilder,
                            ByVal Piva_Origine As String,
                            ByVal Piva_Destinazione As String,
                            ByVal TipoOperazione_DB As enum_TipoOperazioneDB
                    )


        Const NomeFunzione As String = "Elabora_XML_Pratiche_Salva"

        Try
            Dim leggiPratiche As New AgronicaCoreG2GLocalDal.G2GPratiche_R

            Dim oPratiche As AgronicaCoreModello.G2G_Pratiche = leggiPratiche.LeggiPerGias2Gias(True, objOpzioniImportImpresa, Piva_Origine, Piva_Destinazione, objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser, objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser, objOpzioni.objParametri_Server_GIAS_ORIGINE)

            Dim sXmlPratiche As String = AgronicaCoreUtility.XMLUtility.SerializzaOggetto(Of G2G_Pratiche)(oPratiche, "")

            Dim mainDoc As String = Funzioni_MasterG2G.XmlMasterDataPiva(enum_TipoOperazioneDB.Scrittura, Piva_Origine, "", objOpzioni)

            Dim sStringaDaSalvare As String = AgronicaCoreUtility.XDocUtils.IniettaSottoAlberoDaStringaXml(mainDoc, sXmlPratiche, "//utente/DatiImprese/Impresa", "DatiPratiche")

            Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS With {.Url = objOpzioni.wsimportaGiasURl, .Timeout = _timeout_ws_Importa}
            Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)

            '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
            sStringaDaSalvare = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, sStringaDaSalvare)

            Dim sXmlRisposta As String = wsimportazione.Scrivi_XmlFull(Str_Credenziali_WS, sStringaDaSalvare)

            ' leggo la struttura restituita contenente i recode da inserire/modificare/cancellare
            Dim xmlRisposta As XDocument = XDocument.Parse(sXmlRisposta)
            Dim xmlRispostaContatti As XElement = xmlRisposta.Element("Risposta").Element("Risposta_DatiPratiche")
            Dim rispostaContatti = xmlRispostaContatti.FirstNode.ToString
            'Dim rispostaContatti = AgroZip.DeCompressioneBase64(1, wsimportazione.Scrivi_ContattiXmlPrivato(Str_Credenziali_WS, AgroZip.CompressioneBase64(1, sXmlContatti)))
            Dim risposta As G2G_Recode = XMLUtility.DeserializzaOggetto(Of G2G_Recode)(rispostaContatti, "")

            Dim messaggioErrore As String

            ' se la scrittura su destinazione è andata a buon fine aggiorno i recode sull'origine
            If String.IsNullOrEmpty(risposta.MessaggioErrore) Then
                messaggioErrore = G2GUtility.Scrivi_G2G_Recode(risposta, objOpzioni.objParametri_Server_GIAS_ORIGINE)
            Else
                messaggioErrore = risposta.MessaggioErrore
            End If

            ' log trasferimento dati
            If String.IsNullOrEmpty(messaggioErrore) Then
                If Not String.IsNullOrEmpty(risposta.LogRecode) Then
                    Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Pratiche: " & risposta.LogRecode)
                Else
                    'nuove, modificate, eliminate
                    Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Pratiche: " & oPratiche.pratiche_insert.Count & " nuovi " & oPratiche.pratiche_update.Count & " modificati " & oPratiche.G2G_Pratiche_Recode_delete.Count & " cancellati ")
                    Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Pratiche - Passaggi di Stato: " & oPratiche.pratiche_stati_insert.Count & " nuovi " & oPratiche.pratiche_stati_update.Count & " modificati " & oPratiche.pratiche_stati_attuali_delete.Count & " cancellati ")
                End If
            Else
                Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Pratiche ERRORE: " & messaggioErrore)
            End If

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: " & ex.Message
            Log_Import.AppendLine(msg)
            Log_Errori.AppendLine(msg)
            Throw New Exception(msg)
        End Try

    End Sub

    Public Sub Elabora_XML_Pratiche_Salva_Reverse(
                            ByVal objOpzioniImportImpresa As clsImpresa,
                            ByVal efG2G As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                            ByVal objOpzioni As clsOpzioni,
                            ByRef Log_Import As StringBuilder,
                            ByRef Log_Errori As StringBuilder,
                            ByRef Log_Riepilogo As StringBuilder,
                            ByVal Piva_Origine As String,
                            ByVal Piva_Destinazione As String,
                            ByVal TipoOperazione_DB As enum_TipoOperazioneDB
                    )


        Const NomeFunzione As String = "Elabora_XML_Pratiche_Salva_Reverse"

        Try
            Dim leggiPratiche As New AgronicaCoreG2GLocalDal.G2GPratiche_R

            Dim oPratiche As AgronicaCoreModello.G2G_Pratiche_Reverse = leggiPratiche.LeggiPerGias2GiasReverse(True, objOpzioniImportImpresa, Piva_Origine, Piva_Destinazione, objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser, objOpzioni.objParametri_Server_GIAS_ORIGINE)

            Dim sXmlPratiche As String = AgronicaCoreUtility.XMLUtility.SerializzaOggetto(Of G2G_Pratiche_Reverse)(oPratiche, "")

            Dim mainDoc As String = Funzioni_MasterG2G.XmlMasterDataPiva(enum_TipoOperazioneDB.Scrittura, Piva_Origine, "", objOpzioni)

            Dim sStringaDaSalvare As String = AgronicaCoreUtility.XDocUtils.IniettaSottoAlberoDaStringaXml(mainDoc, sXmlPratiche, "//utente/DatiImprese/Impresa", "DatiPratiche")

            Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS With {.Url = objOpzioni.wsimportaGiasURl, .Timeout = _timeout_ws_Importa}
            Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)

            '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
            sStringaDaSalvare = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, sStringaDaSalvare)

            Dim sXmlRisposta As String = wsimportazione.Scrivi_XmlFull(Str_Credenziali_WS, sStringaDaSalvare)

            ' leggo la struttura restituita contenente i recode da inserire/modificare/cancellare
            Dim xmlRisposta As XDocument = XDocument.Parse(sXmlRisposta)
            Dim xmlRispostaContatti As XElement = xmlRisposta.Element("Risposta").Element("Risposta_DatiPratiche")
            Dim rispostaContatti = xmlRispostaContatti.FirstNode.ToString
            'Dim rispostaContatti = AgroZip.DeCompressioneBase64(1, wsimportazione.Scrivi_ContattiXmlPrivato(Str_Credenziali_WS, AgroZip.CompressioneBase64(1, sXmlContatti)))
            Dim risposta As G2G_Recode = XMLUtility.DeserializzaOggetto(Of G2G_Recode)(rispostaContatti, "")

            Dim messaggioErrore As String

            ' se la scrittura su destinazione è andata a buon fine aggiorno i recode sull'origine
            If String.IsNullOrEmpty(risposta.MessaggioErrore) Then
                messaggioErrore = G2GUtility.Scrivi_G2G_Recode(risposta, objOpzioni.objParametri_Server_GIAS_ORIGINE)
            Else
                messaggioErrore = risposta.MessaggioErrore
            End If

            ' log trasferimento dati
            If String.IsNullOrEmpty(messaggioErrore) Then
                If Not String.IsNullOrEmpty(risposta.LogRecode) Then
                    Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Pratiche: " & risposta.LogRecode)
                Else
                    'nuove, modificate, eliminate
                    Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Pratiche: " & oPratiche.pratiche_insert.Count & " nuovi " & oPratiche.pratiche_update.Count & " modificati " & risposta.G2GRecodePraticheToDelete.Count & " cancellati ")
                    Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Pratiche - Passaggi di Stato: " & oPratiche.pratiche_stati_insert.Count & " nuovi " & oPratiche.pratiche_stati_update.Count & " modificati " & risposta.G2GRecodePraticheStatiToDelete.Count & " cancellati ")
                End If
            Else
                Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Pratiche ERRORE: " & messaggioErrore)
            End If

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: " & ex.Message
            Log_Import.AppendLine(msg)
            Log_Errori.AppendLine(msg)
            Throw New Exception(msg)
        End Try

    End Sub

    Public Sub Elabora_XML_Pratiche_Pull_Salva(
                            ByVal objOpzioniImportImpresa As clsImpresa,
                            ByVal efG2G As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                            ByVal objOpzioni As clsOpzioni,
                            ByRef Log_Import As StringBuilder,
                            ByRef Log_Errori As StringBuilder,
                            ByRef Log_Riepilogo As StringBuilder,
                            ByVal Piva_Origine As String,
                            ByVal Piva_Destinazione As String,
                            ByVal TipoOperazione_DB As enum_TipoOperazioneDB
                    )

        Const NomeFunzione As String = "Elabora_XML_Pratiche_Pull_Salva"

        Dim oG2G_RecodeRequest As New G2G_Recodes_Request
        Dim oG2G_RecodeResponse As New G2G_Recode

        Try

            Dim nXmlRequest As XNode = <impresa>
                                           <Piva_SuperUser_origine><%= objOpzioni.Import_CodFiscale_ORIGINE %></Piva_SuperUser_origine>
                                           <Piva_SuperUser_destinazione><%= objOpzioni.Import_CodFiscale_DESTINAZIONE %></Piva_SuperUser_destinazione>
                                           <origine><%= Piva_Origine %></origine>
                                           <destinazione><%= Piva_Destinazione %></destinazione>
                                           <flagimporta_pratiche_pull>
                                               <validita_inizio>
                                                   <%= objOpzioniImportImpresa.ValiditaInizio_pratiche_pull %>
                                               </validita_inizio>
                                               <configurazione>
                                                   <%= objOpzioniImportImpresa.configurazione_pratiche_pull %>
                                               </configurazione>
                                           </flagimporta_pratiche_pull>
                                       </impresa>

            Dim mainDoc As String = Funzioni_MasterG2G.XmlMasterDataPiva(enum_TipoOperazioneDB.Scrittura, "", "", objOpzioni)

            mainDoc = AgronicaCoreUtility.XDocUtils.IniettaSottoAlberoDaStringaXml(mainDoc, nXmlRequest.ToString, "//utente", "imprese")

            Dim sStringaDaSalvare As String = mainDoc

            Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS
            wsimportazione.Url = objOpzioni.wsimportaGiasURl
            wsimportazione.Timeout = _timeout_ws_Importa
            Dim Str_Credenziali_WS As String = Funzioni.Get_Str_Credenziali_WS(objOpzioni)

            '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
            sStringaDaSalvare = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, sStringaDaSalvare)

            Dim outputPratiche As String =
            wsimportazione.Scrivi_XmlFull(
                Str_Credenziali_WS,
                sStringaDaSalvare)

            'riporto della risposta
            Dim oResponse As New G2G_Pratiche
            Dim xDocResponse As XDocument = XDocument.Parse(outputPratiche)
            Dim nsAgronicaCoreModello As XNamespace = "http://schemas.datacontract.org/2004/07/AgronicaCoreModello"
            Dim elemResponse As XElement = xDocResponse.Element("Risposta").Element("Risposta_DatiPratiche").Element(nsAgronicaCoreModello + "G2G_Pratiche")
            Dim strResponse As String = elemResponse.ToString()
            oResponse = AgronicaCoreUtility.XMLUtility.DeserializzaOggetto(Of G2G_Pratiche)(strResponse, "")


            'inserisco i passaggi di stato che sono nuovi ed aggiorno quanto va aggiornato...
            If oResponse.pratiche_stati_insert.Count > 0 Then

                Dim gefutils As New Gias_EF_Utility
                Dim EFConnString As String = gefutils.GetEntityConnectionString(objOpzioni.objParametri_Server_GIAS_ORIGINE.StringaConnessione)
                Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                Dim psAppoggio As New List(Of Pratiche_Stati)
                'For Each l1 As Pratiche_Stati In oResponse.pratiche_stati_insert

                '    'sto cercando dati che vengono memorizzati in origine con le codifiche della destinazione
                '    Dim recodeDaVerificare As G2G_Recode_Pratiche_Stati = (
                '        From s In GiasContext.G2G_Recode_Pratiche_Stati
                '        Where s.From_PassaggioDiStato_Cod = l1.PassaggioDiStato_cod _
                '            And s.From_PivaSuperUser = l1.Piva_SuperUser).FirstOrDefault

                '    If Not recodeDaVerificare Is Nothing Then

                '        If oResponse.pratiche_stati_update Is Nothing Then
                '            oResponse.pratiche_stati_update = New List(Of Pratiche_Stati)
                '        End If
                '        psAppoggio.Add(l1)

                '        Dim dcompare As Date = recodeDaVerificare.datainvio
                '        Dim dCompare1 As Date = l1.Data_Modifica

                '        If dcompare <= dCompare1 Then
                '            oResponse.pratiche_stati_update.Add(l1)
                '        End If


                '    End If
                'Next

                For Each upd In psAppoggio
                    oResponse.pratiche_stati_insert.Remove(upd)
                Next

                'scrivo lo stato attuale solo se è presente negli stati "esclusi" 
                '(quindi va scritta poichè lo stato viene fatto avanzare in destinazione).
                Dim listaVerificaContains As New List(Of String)
                Dim listaVerificaStatoAttuale As New List(Of String)

                Dim oConfigurazione As G2G_Configurazione_FiltriReq_Pratiche
                If objOpzioniImportImpresa.configurazione_pratiche <> "" Then
                    oConfigurazione = JsonConvert.DeserializeObject(Of G2G_Configurazione_FiltriReq_Pratiche)(objOpzioniImportImpresa.configurazione_pratiche)
                Else
                    oConfigurazione = New G2G_Configurazione_FiltriReq_Pratiche
                    oConfigurazione.listaServiziWWorkflow = New List(Of G2G_Configurazione_FiltriReq_Pratiche_Servizi)
                End If


                G2GUtility.RecuparaListe(oConfigurazione, listaVerificaContains, listaVerificaStatoAttuale)
                Dim listaStatiAttualiDaEscludere As New List(Of Pratiche_Stati_Attuali)

                'For Each statoAtt As Pratiche_Stati_Attuali In oResponse.pratiche_stati_attuali_update

                '    'cerco la pratica per dedurne il tipo di servizio
                '    Dim p As Pratiche = (
                '                    From p1 In GiasContext.Pratiche
                '                    Join g2g In GiasContext.G2G_Recode_Pratiche
                '                        On p1.Pratica_Cod Equals g2g.From_Pratica_Cod _
                '                        And p1.Piva_SuperUser Equals g2g.From_PivaSuperUser
                '                    Where g2g.To_Pratica_Cod = statoAtt.Pratica_Cod _
                '                        And g2g.To_PivaSuperUser = statoAtt.Piva_SuperUser
                '                    Select p1).FirstOrDefault

                '    If Not p Is Nothing Then
                '        Dim k As String =
                '            p.Servizio_Cod & G2GUtility.SeparatoreChiave & statoAtt.Stato_Cod

                '        If Not listaVerificaStatoAttuale.Contains(k) Then
                '            listaStatiAttualiDaEscludere.Add(statoAtt)
                '        End If

                '    End If

                'Next

                For Each daEscludere In listaStatiAttualiDaEscludere
                    oResponse.pratiche_stati_attuali_update.Remove(daEscludere)
                Next
                'fine scrivo lo stato attuale solo se è presente negli stati "esclusi".

                Dim scriviPraticheStati As New AgronicaCoreG2GLocalDal.G2GPratiche_W

                'scambio la p.iva del super user perchè è come se il G2G lavorarasse all'inverso, da destinazione ad origine
                Dim recode As G2G_Recode = scriviPraticheStati.Scrivi_Pratiche_G2G(
                    False,
                    objOpzioni.SuperUser_CodFiscale_DESTINAZIONE,
                    objOpzioni.SuperUser_CodFiscale_ORIGINE,
                    oResponse,
                    objOpzioni.objParametri_Server_GIAS_ORIGINE
                )


                Dim sXmlRecode As String = AgronicaCoreUtility.XMLUtility.SerializzaOggetto(Of AgronicaCoreModello.G2G_Recode)(recode, "")

                Dim mainDoc1 As String = Funzioni_MasterG2G.XmlMasterDataRecodes(enum_TipoOperazioneDB.Scrittura, objOpzioni)
                mainDoc1 = AgronicaCoreUtility.XDocUtils.IniettaSottoAlberoDaStringaXml(mainDoc1, sXmlRecode, "//utente/G2G_Recodes", "G2G_Recode")

                '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
                Dim sStringaDaSalvare1 As String = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, mainDoc1)
                Dim outputRecode = wsimportazione.Scrivi_XmlFull(Str_Credenziali_WS, sStringaDaSalvare1)

                Log_Import.AppendLine(CStr(Date.Now) & " - " & "outputRecode: " & outputRecode)


                ' log trasferimento dati
                If Not String.IsNullOrEmpty(recode.MessaggioErrore) Then
                    Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Pratiche Pull ERRORE: " & recode.MessaggioErrore)
                End If

            End If

        Catch ex As Exception
            Dim msg As String
            msg = CStr(Date.Now) + " - " + NomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try





    End Sub
#End Region
End Class
