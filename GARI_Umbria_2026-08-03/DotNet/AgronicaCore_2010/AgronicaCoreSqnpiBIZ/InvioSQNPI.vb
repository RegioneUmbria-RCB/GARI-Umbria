
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility.Http
Imports AgronicaCoreUtility
Imports System.Net
Imports AgronicaCoreVarieBIZ

Imports AgronicaCoreDataProvider.ConnessioniTransazioni
Imports AgronicaCoreDataProvider.TipiEnumerativi


Public Class InvioSQNPI

    Public Function VerificaDomanda(ByVal ServiceTimeOut As String, ByVal dirXml As String, ByVal UrlServizio As String, ByVal username As String, ByVal Password As String, ByVal objParametri_Server As AgronicaCoreParametri) As RispostaStandard

        Dim xLetturaDati As New AgronicaCoreSqnpiDAL.SQNPI_R

        Dim rispostaStandardCompleta As New RispostaStandard
        Dim rispostaStandardSingoloFascicolo As RispostaStandard

        
        Dim dtDatiAppezzamenti As DataTable
        dtDatiAppezzamenti = xLetturaDati.LeggiPerVerifica("", "", objParametri_Server)

        Dim dtDatiAppezzamenti_Rows As DataRowCollection = dtDatiAppezzamenti.Rows

        rispostaStandardCompleta.RispostaOK = True
        rispostaStandardCompleta.RispostaStringa = "Verifica di domande in fase di Verifica_DPI: " & dtDatiAppezzamenti_Rows.Count & " Domande trovate."


        For Each curdtDatiAppezzamenti_Rows In dtDatiAppezzamenti_Rows

            Dim oRich As New consultazioneDomandaRequest
            oRich.identificativoFlusso = curdtDatiAppezzamenti_Rows("IdentificativoFlusso")
            oRich.skXml = curdtDatiAppezzamenti_Rows("skXml")

            rispostaStandardSingoloFascicolo = ChiamataWS(curdtDatiAppezzamenti_Rows("ws_SQNPI_LogInvio_Cod"), ServiceTimeOut, username, Password, Nothing, oRich, "", UrlServizio, objParametri_Server)

            rispostaStandardCompleta.RispostaOK = (rispostaStandardCompleta.RispostaOK And rispostaStandardSingoloFascicolo.RispostaOK)

            rispostaStandardCompleta.Errore &= rispostaStandardSingoloFascicolo.Errore
            rispostaStandardCompleta.RispostaStringa &= rispostaStandardSingoloFascicolo.RispostaStringa

        Next


        Return rispostaStandardCompleta

    End Function

    Public Function InvioDati(ByVal ServiceTimeOut As String, ByVal dirXml As String, ByVal UrlServizio As String, ByVal username As String, ByVal Password As String, ByVal objParametri_Server As AgronicaCoreParametri) As RispostaStandard


        Dim xLetturaDati As New AgronicaCoreSqnpiDAL.SQNPI_R

        Dim rispostaStandardCompleta As New RispostaStandard
        Dim rispostaStandardSingoloFascicolo As New RispostaStandard


        'CInt(enum_WWorflow_WAnagraficaStati.Sistema_Qualità_Nazionale_Produzione_Integrata_In_Fase_di_verifica_per_errori_formali), _
        '        CInt(enum_WWorflow_WAnagraficaStati.Sistema_Qualità_Nazionale_Produzione_Integrata_Errori_formali_su_tracciato_xml), _
        '        CInt(enum_WWorflow_WAnagraficaStati.Sistema_Qualità_Nazionale_Produzione_Integrata_Inviata_correttamente_in_valutazione_per_errori_sostanziali), _
        '        CInt(enum_WWorflow_WAnagraficaStati.Sistema_Qualità_Nazionale_Produzione_Integrata_Errori_sostanziali_riscontrati_su_xml) _

        Dim dtDatiAppezzamenti As DataTable
        dtDatiAppezzamenti = xLetturaDati.LeggiPerInvio( _
            "", _
            String.Join(",", _
               ({ _
                CInt(enum_WWorflow_WAnagraficaStati.Sistema_Qualità_Nazionale_Produzione_Integrata_In_fase_di_invio) _
               }).ToList
            ), _
            "", "", objParametri_Server)


        Dim xFascicolo As New List(Of String)


        xFascicolo = ( _
            From xF In dtDatiAppezzamenti.AsEnumerable _
            Select CStr(xF("identificativoFlusso")) _
        ).Distinct.ToList



        For Each FascicoloCorrente In xFascicolo


            Dim Fascicolo As New invioDomandaRequest

            Dim xProtocollo As String = Nothing
            If Not IsDBNull(dtDatiAppezzamenti(0)("protocollo")) Then
                xProtocollo = dtDatiAppezzamenti(0)("protocollo")
            End If

            Fascicolo.identificativoFlusso = dtDatiAppezzamenti(0)("identificativoFlusso")
            Fascicolo.dataAdesione = dtDatiAppezzamenti(0)("dataAdesione")
            Fascicolo.skOdc = dtDatiAppezzamenti(0)("skOdc")
            Fascicolo.cuaaOp = dtDatiAppezzamenti(0)("cuaaOp")
            Fascicolo.destinatariResponso = LeggiDestinatari(dtDatiAppezzamenti(0)("destinatariResponso"))
            Fascicolo.skTipoDomanda = dtDatiAppezzamenti(0)("skTipoDomanda")
            Fascicolo.protocollo = xProtocollo
            Fascicolo.skTipoScopo = dtDatiAppezzamenti(0)("skTipoScopo")

            Dim xSoggetti As List(Of String) = ( _
                From xS In dtDatiAppezzamenti.AsEnumerable _
                Where xS("identificativoFlusso") = FascicoloCorrente _
                Select CStr(xS("Cuaa_Richiedente")) _
            ).Distinct.ToList



            Dim ListaSoggetti As New List(Of invioDomandaRequestSoggetto)

            For Each SoggettoCorrente In xSoggetti

                Dim soggetto As New invioDomandaRequestSoggetto
                soggetto.cuaa = SoggettoCorrente

                Dim xFindSoggetto As String = SoggettoCorrente

                Dim xParticelle As New List(Of invioDomandaRequestSoggettoDatoCatastale)

                xParticelle = ( _
                    From xP In dtDatiAppezzamenti.AsEnumerable _
                    Where xP("identificativoFlusso") = FascicoloCorrente _
                    And xP("Cuaa_Richiedente") = xFindSoggetto _
                    Select New invioDomandaRequestSoggettoDatoCatastale With { _
                        .codiIstaProv = xP("PROV"), _
                        .codiIstaComu = xP("COM"), _
                        .seziCens = IIf(String.IsNullOrEmpty(xP("SEZIONE")), Nothing, xP("SEZIONE")), _
                        .foglio = xP("FOGLIO"), _
                        .numeroParticella = xP("NUMERO"), _
                        .subalterno = IIf(String.IsNullOrEmpty(xP("SUBALTERNO")), Nothing, xP("SUBALTERNO")), _
                        .dettagliTerreno = {New dettaglioTerrenoType With { _
                            .resaMedia = xP("ResaMedia") _
                        }}, _
                        .codificaFascicolo = New codificaFascicoloType With { _
                            .Item = New codificaFascicoloTypeTripletta With { _
                                .codiMacrouso = xP("MacroUso"), _
                                .codiProdotto = xP("Prodotto_AGEA"), _
                                .codiVarieta = xP("Varietà_AGEA") _
                            }
                        }
                    } _
                ).Distinct.ToList


                soggetto.datiCatastali = xParticelle.ToArray

                ListaSoggetti.Add(soggetto)


            Next

            Fascicolo.soggetti = ListaSoggetti.ToArray


            'invio del fascicolo al web service.

            If Not String.IsNullOrEmpty(UrlServizio) Then
                rispostaStandardSingoloFascicolo = ChiamataWS(0, ServiceTimeOut, username, Password, Fascicolo, Nothing, FascicoloCorrente, UrlServizio, objParametri_Server)
            End If

            Dim xmlDoc As XDocument = Utility.getIntestazione(username, Password, "acquisizioneDomanda")

            Dim req = XMLUtility.getBodyRequest(Fascicolo, Nothing, Nothing)

            XMLUtility.addBody(xmlDoc, req, Utility.getSoapenv)

            Utility.cleanXmlSend(xmlDoc)

            xmlDoc.Save(dirXml & "\" & FascicoloCorrente.Replace("\", "_").Replace("/", "_") & "__" & AgronicaCoreUtility.FileSystemHelper.NomeFileUnivoco("") & ".xml")



            rispostaStandardSingoloFascicolo = New RispostaStandard
            rispostaStandardSingoloFascicolo.RispostaOK = True
            rispostaStandardSingoloFascicolo.RispostaStringa = "Nessun invio: xml memorizzato."





            rispostaStandardCompleta.RispostaOK = (rispostaStandardCompleta.RispostaOK And rispostaStandardSingoloFascicolo.RispostaOK)

            rispostaStandardCompleta.Errore &= rispostaStandardSingoloFascicolo.Errore
            rispostaStandardCompleta.RispostaStringa &= rispostaStandardSingoloFascicolo.RispostaStringa


        Next


        Return rispostaStandardCompleta

    End Function


    Private Function ChiamataWS(ByVal param_ws_SQNPI_LogInvio_Cod As Integer, ByVal ServiceTimeOut As Integer, ByVal username As String, ByVal password As String, ByVal oDomanda As invioDomandaRequest, ByVal oVerificaDomanda As consultazioneDomandaRequest, ByVal IdentificativoFlusso As String, ByVal url As String, ByVal objParametri_Server As AgronicaCoreParametri) As RispostaStandard

        Dim rs As New RispostaStandard


        Const NomeRoutine As String = "ChiamataREST"

        Dim xHttp As New AgronicaCoreUtility.Http


        Dim oResponse As New domandaPIResponse


        Dim oSqnpiDal As New AgronicaCoreSqnpiDAL.SQNPI_W
        Dim oSqnpi_log As New AgronicaCoreSqnpiDAL.SQNPI_LogInvio_W
        Dim oSqnpi_dettaglio_log As New AgronicaCoreSqnpiDAL.SQNPI_LogInvio_Dettaglio_W
        Dim oSqnpi_Errori_log As New AgronicaCoreSqnpiDAL.SQNPI_LogInvio_Dettaglio_Errori_W




        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False


        Dim oSequenzaTabelle As New Agro_Sequenze

        Try

            'sResponse = xHttpREST.chiamaWS(Request, url, url, "text/xml; charset=utf-8", "POST", "text/xml", "POST")
            'XMLUtility.getObjectFromResponse(sResponse, oResponse, Utility.getSoapenv)


            Dim ws_Timeout As Boolean = False
            Dim skXML_Local As Integer = 0

            Try


                Dim TestProduzione As String = "Produzione"
                If url.ToLower.Contains("test") Then
                    TestProduzione = "Test"
                End If

                Dim dPIWS As New domandaPIWS
                dPIWS.Url = url
                If ServiceTimeOut <> 0 Then
                    dPIWS.Timeout = CInt(ServiceTimeOut) * 1000
                End If

                dPIWS.ClientCertificates.Add(xHttp.GetCertFromUrl(url))

                If Not oDomanda Is Nothing Then

                    dPIWS.SOAPAutenticazioneValue = New SOAPAutenticazione With {.username = username, .password = password, .nomeServizio = "acquisizioneDomanda"}
                    oResponse = dPIWS.acquisizioneDomanda(oDomanda)

                    'Recupero un skXML (deve essere allineato, lo faccio avanzare comunque..)
                    'oSequenzaTabelle.Agronica_SequenzaTabelle_NuovoID("ws_SQNPI_LogInvio_Cod_skXML_" & TestProduzione, objParametri_Server)
                    'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
                    oSequenzaTabelle.NuovoId_Tabella("ws_SQNPI_LogInvio_Cod_skXML_" & TestProduzione, 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri_Server)

                Else

                    dPIWS.SOAPAutenticazioneValue = New SOAPAutenticazione With {.username = username, .password = password, .nomeServizio = "consultazioneDomanda"}
                    oResponse = dPIWS.consultazioneDomanda(oVerificaDomanda)
                End If
            Catch ex As TimeoutException
                ws_Timeout = True
            End Try



            'apro la transazione

            'Apro la connessione al DB
            ApriConnessioneXCoreBiz( _
                FlagConnessioneLocale, _
                FlagTransazioneLocale, _
                objParametri_Server _
            )

            Dim ws_SQNPI_LogInvio_Cod As Integer
            If param_ws_SQNPI_LogInvio_Cod = 0 Then
                'ws_SQNPI_LogInvio_Cod = oSequenzaTabelle.Agronica_SequenzaTabelle_NuovoID("ws_SQNPI_LogInvio_Cod", objParametri_Server)
                'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
                ws_SQNPI_LogInvio_Cod = oSequenzaTabelle.NuovoId_Tabella("ws_SQNPI_LogInvio_Cod", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri_Server)
            Else
                ws_SQNPI_LogInvio_Cod = param_ws_SQNPI_LogInvio_Cod
            End If

            Dim ws_SQNPI_LogInvio_des As String = ""

            'scrittura della risposta..
            'Testata

            Dim iProgressivoErrore As Integer = 0
            Dim TestoOkErrore As String = " correttamente "

            If oResponse.errori.Count > 0 Then
                TestoOkErrore = " con errori "
            End If

            Dim MessaggioFinale As String
            MessaggioFinale = "Invio eseguito " & TestoOkErrore & " per la trasmissione con id flusso: " & IdentificativoFlusso


            Dim Stato_DaImpostare As Integer = 0

            If oResponse Is Nothing OrElse Not oResponse.dataElaborazioneSpecified Then
                Stato_DaImpostare = enum_WWorflow_WAnagraficaStati.Sistema_Qualità_Nazionale_Produzione_Integrata_In_Fase_di_verifica_per_errori_formali
            End If

            If param_ws_SQNPI_LogInvio_Cod = 0 Then

                
                If Not oResponse Is Nothing Then

                    oSqnpi_log.Scrivi( _
                        ws_SQNPI_LogInvio_Cod, _
                        MessaggioFinale, _
                        Now, _
                        oResponse.dataElaborazione, _
                        oResponse.esitoRicezione, _
                        oResponse.skXml, _
                        objParametri_Server _
                    )

                Else

                    'il servizio è andato in timeout .. memorizzo ugualmente un skXML ottenuto localmente.
                    oSqnpi_log.Scrivi( _
                        ws_SQNPI_LogInvio_Cod, _
                        MessaggioFinale, _
                        Now, _
                        Now, _
                        "", _
                        skXML_Local, _
                        objParametri_Server _
                    )

                End If


                oSqnpi_dettaglio_log.Scrivi(ws_SQNPI_LogInvio_Cod, IdentificativoFlusso, objParametri_Server)

            Else

                'Aggiorno i dati..
                If Not oResponse Is Nothing AndAlso oResponse.dataElaborazioneSpecified Then
                    oSqnpi_log.Modifica( _
                        ws_SQNPI_LogInvio_Cod, _
                        MessaggioFinale, _
                        Now, _
                        oResponse.dataElaborazione, _
                        oResponse.esitoRicezione, _
                        objParametri_Server _
                    )

                End If



            End If


            If Not ws_Timeout Then

                For Each oErr As domandaPIResponseErrore In oResponse.errori

                    iProgressivoErrore += 1

                    oSqnpi_Errori_log.Scrivi( _
                        ws_SQNPI_LogInvio_Cod, _
                        0, _
                        oErr.descrizioneErrore, _
                        iProgressivoErrore, _
                        oErr.skTipoLog, _
                        oErr.datoErrato, _
                        objParametri_Server _
                    )

                Next

                If iProgressivoErrore = 0 Then

                    If param_ws_SQNPI_LogInvio_Cod = 0 Then
                        oSqnpi_Errori_log.Scrivi( _
                            ws_SQNPI_LogInvio_Cod, _
                            1, _
                            "Nessun Errore formale riscontrato", _
                            iProgressivoErrore, _
                            -1, _
                             "", _
                            objParametri_Server _
                        )
                    End If

                    If oResponse.dataElaborazioneSpecified Then
                        Stato_DaImpostare = enum_WWorflow_WAnagraficaStati.Sistema_Qualità_Nazionale_Produzione_Integrata_Inviata_correttamente__in_valutazione_per_errori_sostanziali
                    End If


                Else
                    Stato_DaImpostare = enum_WWorflow_WAnagraficaStati.Sistema_Qualità_Nazionale_Produzione_Integrata_Errori_formali_su_tracciato_xml
                End If


            End If


            If IdentificativoFlusso = "" Then
                IdentificativoFlusso = oVerificaDomanda.identificativoFlusso
            End If

            oSqnpiDal.AvanzamentoDiStato(IdentificativoFlusso, "ws_SQNPI_Domanda", Stato_DaImpostare, 0, objParametri_Server)
            oSqnpiDal.AvanzamentoDiStato("", "ws_SQNPI_LogInvio", Stato_DaImpostare, ws_SQNPI_LogInvio_Cod, objParametri_Server)
            oSqnpiDal.AvanzamentoDiStato_Planning_dato_IdentificativoFlusso(IdentificativoFlusso, Stato_DaImpostare, objParametri_Server)

            


            rs.RispostaStringa = MessaggioFinale
            rs.RispostaOK = True


            'chiudo
            ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)


        Catch ex As Exception

            'rollback
            'Faccio il rollback della transazione
            If Not objParametri_Server.objTransazione Is Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                ChiudiTransazione(2, objParametri_Server)

            End If

            rs.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            rs.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True, source:=True)

            ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[" & NomeRoutine & "] : " & rs.Errore, ex)

        Finally

            ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

        End Try

        If Not oDomanda Is Nothing Then
            Dim xM As New AgronicaCoreUtility.Mail
            Dim dest As String()
            dest = oDomanda.destinatariResponso

            Try
                Dim __skXml As String = ""
                If Not oResponse Is Nothing Then
                    __skXml = oResponse.skXml
                End If
                xM.invia(objParametri_Server, "sqnpi@agronica.it", oDomanda.destinatariResponso, Nothing, Nothing, "Inviati dati a servizio SQNPI (skxml = " & __skXml & ", Flusso: " & oDomanda.identificativoFlusso & ") per OP con CUAA:" & oDomanda.cuaaOp, rs.RispostaStringa, True, Nothing)
            Catch ex As Exception

            End Try


        End If

        Return rs

    End Function

    Private Function LeggiDestinatari(ByVal sDest As String) As String()
        Return sDest.Split(",")
    End Function

End Class

