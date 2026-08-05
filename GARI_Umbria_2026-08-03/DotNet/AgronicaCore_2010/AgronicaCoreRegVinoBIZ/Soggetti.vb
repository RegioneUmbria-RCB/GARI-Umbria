Imports AgronicaCoreRegVinoDAL
Imports AgronicaCoreUtility

Public Class Soggetti
    Inherits TeleregistriManager

    Dim reader As xDBSoggSiRPV_R
    Dim writer As xDBSoggSiRPV_W

    Sub New(user As String, pwd As String, urlS As String, urlA As String, server As AgronicaCoreDataProvider.AgronicaCoreParametri, utenti As AgronicaCoreDataProvider.AgronicaCoreParametri, _Configurazione_Servizio As AgronicaCoreVarieDAL.Configurazione_Servizio, CertificateFile As String)
        MyBase.New(user, pwd, urlS, urlA, server, utenti, _Configurazione_Servizio, CertificateFile)
    End Sub

    Public Overrides Sub checkResult()
        Dim listaCodOper As List(Of String) = reader.getListaCodOper(ObjParametri_Server)
        Dim corretto As Boolean = True
        For Each codOper As String In listaCodOper
            'Dim personaFisica As Boolean = reader.personaFisica(ObjParametri_Server, codOper)
            'Dim DT As DataTable = reader.getDBSoggettiPerCodOper(ObjParametri_Server, codOper)

            'Dim visSog As New VisSoggSiRPV
            'Dim vis = visSog.sVisSoggSiRPVInput(username, password, codOper, personaFisica)
            'Dim rispostaInvio = caller.ChiamataWSSync(vis, urlSync)

            'Dim converted = False
            'Dim result As New VisSoggSiRPVOutput
            'Try
            '    XMLUtility.getObjectFromResponse(rispostaInvio, result, Utility.getSoapenv)
            '    converted = True
            'Catch ex As Exception
            '    Throw New TeleregistriExceptionResponseParsing("Soggetti", "checkResult")
            'End Try
            'If converted Then
            '    Dim listaSoggettiVis As New List(Of String)
            '    If result.DettaglioSoggetto IsNot Nothing Then
            '        For Each sogg As Soggetto In result.DettaglioSoggetto
            '            listaSoggettiVis.Add(sogg.CodiceSoggetto)
            '        Next

            '        For Each soggettoInterno As DataRow In DT.Rows
            '            If listaSoggettiVis.Contains(soggettoInterno.Item("CodiceSoggetto").ToString.ToUpper) Then
            '                listaSoggettiVis.Remove(soggettoInterno.Item("CodiceSoggetto").ToString.ToUpper)
            '            End If
            '        Next

            '        Dim soggettiDaAggiungere As New List(Of Soggetto)
            '        If listaSoggettiVis.Count <> 0 Then
            '            For Each soggDaAgg In listaSoggettiVis
            '                For Each sogg As Soggetto In result.DettaglioSoggetto
            '                    If soggDaAgg = sogg.CodiceSoggetto Then
            '                        soggettiDaAggiungere.Add(sogg)
            '                    End If
            '                Next
            '            Next

            '            For Each sogg In soggettiDaAggiungere
            '                Dim tipoSoggetto As String
            '                Select Case sogg.TipoSoggetto
            '                    Case SoggettoTipoSoggetto.IT
            '                        tipoSoggetto = "IT"
            '                    Case SoggettoTipoSoggetto.UE
            '                        tipoSoggetto = "UE"
            '                    Case SoggettoTipoSoggetto.EX
            '                        tipoSoggetto = "EX"
            '                End Select
            '                Dim cuaaFisiche As Integer
            '                Select Case sogg.CodiceCUAA.ItemElementName
            '                    Case ItemChoiceType.PersonaFisica
            '                        cuaaFisiche = 1
            '                    Case ItemChoiceType.PersonaGiuridica
            '                        cuaaFisiche = 0
            '                End Select
            '                writer.inserisciSoggetti(ObjParametri_Server, sogg.CodiceSoggetto, codOper, "G", "I", sogg.CodiceCUAA.Item, cuaaFisiche, tipoSoggetto, sogg.Nome, sogg.Cognome, sogg.RagioneSociale, sogg.IndirizzoSede.CAP, sogg.IndirizzoSede.Indirizzo1, sogg.IndirizzoSede.Comune, sogg.IndirizzoSede.Provincia, sogg.IndirizzoSede.Stato, Utility.StatoGIAS.valida_nel_sian)
            '            Next
            '        End If
            '    End If
            'End If

            'Dim objCoreStampeDAL As New AgronicaCoreStampeDAL.Cantina_RegistriTelematici

            'Dim dtEliminati = reader.LeggiSoggetti("", "", ObjParametri_Server, codOper, , Utility.StatoGIAS.eliminata_nel_sian, "E")
            'For Each soggEl As DataRow In dtEliminati.Rows
            '    writer.eliminaSoggetto(ObjParametri_Server, soggEl.Item("CodiceSoggetto"), codOper)
            'Next

            ''Leggo soggetti da Telematizzare perché utilizzati nelle operazioni
            'Dim listaSoggetti As New List(Of String)
            'listaSoggetti = reader.leggiSoggettiDaOperazioni(ObjParametri_Server, codOper)
            'For Each sogg In listaSoggetti
            '    Dim dtS = reader.LeggiSoggetti("", "", ObjParametri_Server, codOper, sogg)
            '    If dtS.Rows.Count > 0 Then
            '        Dim tipoRichiesta = dtS.Rows(0).Item("TipoRichiesta")
            '        Dim stato = dtS.Rows(0).Item("GIAS_Stato")
            '        If tipoRichiesta = "E" And stato = Utility.StatoGIAS.eliminata_nel_sian Then
            '            'Da Inserire
            '            writer.aggiornaSoggetto(ObjParametri_Server, codOper, sogg, "I", Utility.StatoGIAS.creata)
            '        End If
            '    Else
            '        'Da Inserire
            '        Dim msg As String = ""
            '        Dim erroreStr As String = ""
            '        Dim id = sogg
            '        ' 1 Guardo se è presente una riga nelle tabelle di transcodifica
            '        Dim dti = reader.Leggiws_RegVino_Soggetti(ObjParametri_Server, id, codOper)
            '        Dim canInsertRefresh = False
            '        Dim nuovaRichiesta As String = ""
            '        possoInserireAggiornare(dti, canInsertRefresh, nuovaRichiesta, msg)
            '        If canInsertRefresh Then
            '            Dim dtSoggetto = objCoreStampeDAL.LeggiAnagrafiche("", _
            '                                    0, _
            '                                    0, _
            '                                    "", "", _
            '                                    False, ObjParametri_Server, codOper, "", id)
            '            If dtSoggetto.Rows.Count > 0 Then
            '                Dim soggetto = dtSoggetto.Rows(0)
            '                Dim errore = False
            '                Dim codiceSoggetto As String = soggetto.Item("CodiceSoggetto")
            '                If codiceSoggetto = "" Or codiceSoggetto.Contains("/") Or codiceSoggetto.Contains("\") Or codiceSoggetto.Contains("-") Or codiceSoggetto.Contains(".") Then
            '                    errore = True
            '                    erroreStr = "Codice Soggetto non valido"
            '                End If
            '                Dim codOper_FisicheGiuridiche As String = "G"
            '                Dim cuaa As String
            '                Dim cuaa_fisiche As Boolean
            '                Dim nome As String = ""
            '                Dim cognome As String = ""
            '                Dim rag_soc As String = ""
            '                If (Not IsDBNull(soggetto.Item("CUAA_PersonaFisica"))) AndAlso soggetto.Item("CUAA_PersonaFisica") <> "" Then
            '                    cuaa = soggetto.Item("CUAA_PersonaFisica")
            '                    If cuaa = "" Or cuaa.Contains("/") Or cuaa.Contains("\") Or cuaa.Contains("-") Or cuaa.Contains(".") Then
            '                        errore = True
            '                        erroreStr = "Codice Soggetto non valido"
            '                    End If
            '                    cuaa_fisiche = True
            '                ElseIf (Not IsDBNull(soggetto.Item("CUAA_PersonaGiuridica"))) AndAlso soggetto.Item("CUAA_PersonaGiuridica") <> "" Then
            '                    cuaa = soggetto.Item("CUAA_PersonaGiuridica")
            '                    If cuaa = "" Or cuaa.Contains("/") Or cuaa.Contains("\") Or cuaa.Contains("-") Or cuaa.Contains(".") Then
            '                        errore = True
            '                        erroreStr = "Codice Soggetto non valido"
            '                    End If
            '                    cuaa_fisiche = False
            '                Else
            '                    cuaa = ""
            '                End If
            '                If cuaa_fisiche Then
            '                    nome = soggetto.Item("Nome")
            '                    cognome = soggetto.Item("Cognome")
            '                    If nome = "" And cognome = "" Then
            '                        rag_soc = soggetto.Item("Rag_Soc")
            '                        Dim rr = rag_soc.Split(" ")
            '                        If rr.Length >= 2 Then
            '                            nome = rr(rr.Length - 1)
            '                            Dim i = 0
            '                            While (i <= (rr.Length - 2))
            '                                cognome = rr(i) + " "
            '                                i += 1
            '                            End While
            '                            nome = nome.Trim
            '                            cognome = cognome.Trim
            '                            rag_soc = ""
            '                        End If
            '                    End If
            '                Else
            '                    rag_soc = soggetto.Item("Rag_Soc")
            '                End If
            '                If nome = "" And cognome = "" And rag_soc = "" Then
            '                    errore = True
            '                    erroreStr = "Nome Cognome e Ragione Sociale assenti"
            '                End If
            '                Dim tiposoggetto As String = soggetto.Item("TipoSoggetto")

            '                Dim cap As String = ""
            '                Dim indirizzo As String = soggetto.Item("IndirizzoSede_Indirizzo")
            '                If indirizzo = "" Then
            '                    errore = True
            '                    erroreStr = "Indirizzo incompleto"
            '                End If
            '                Dim comune As String = ""
            '                Dim provincia As String = ""
            '                Dim stato As String = soggetto.Item("TipoSoggetto")
            '                If stato = "IT" Then
            '                    cap = soggetto.Item("IndirizzoSede_CAP")
            '                    comune = soggetto.Item("IstatCom")
            '                    provincia = soggetto.Item("IstatProv")
            '                    stato = "380"
            '                    If cap = "" Or comune = "" Or provincia = "" Then
            '                        errore = True
            '                        erroreStr = "Indirizzo incompleto"
            '                    End If
            '                Else
            '                    cuaa = ""
            '                    cap = ""
            '                    comune = ""
            '                    provincia = ""
            '                    stato = soggetto.Item("Codice_numerico_Stato")
            '                End If
            '                If tiposoggetto = "" Then
            '                    errore = True
            '                    erroreStr = "Tipo Soggetto Errato"
            '                End If
            '                Dim TipoRichiesta As String = nuovaRichiesta
            '                Dim Gias_Stato As Integer = 18000001
            '                If errore Then
            '                    writer.InserisciAggiornaWs_RegVino_Soggetti(ObjParametri_Server, _
            '                                                                      codiceSoggetto, _
            '                                                                      codOper, _
            '                                                                      codOper_FisicheGiuridiche, _
            '                                                                      TipoRichiesta, _
            '                                                                      cuaa, _
            '                                                                      cuaa_fisiche, _
            '                                                                      tiposoggetto, _
            '                                                                      nome, _
            '                                                                      cognome, _
            '                                                                      rag_soc, _
            '                                                                      cap, _
            '                                                                      indirizzo, _
            '                                                                      comune, _
            '                                                                      provincia, _
            '                                                                      stato, _
            '                                                                      18000003)
            '                    msg += id + ": Errore - " + erroreStr
            '                Else
            '                    writer.InserisciAggiornaWs_RegVino_Soggetti(ObjParametri_Server, _
            '                                                                      codiceSoggetto, _
            '                                                                      codOper, _
            '                                                                      codOper_FisicheGiuridiche, _
            '                                                                      TipoRichiesta, _
            '                                                                      cuaa, _
            '                                                                      cuaa_fisiche, _
            '                                                                      tiposoggetto, _
            '                                                                      nome, _
            '                                                                      cognome, _
            '                                                                      rag_soc, _
            '                                                                      cap, _
            '                                                                      indirizzo, _
            '                                                                      comune, _
            '                                                                      provincia, _
            '                                                                      stato, _
            '                                                                      Gias_Stato)
            '                End If
            '            End If
            '        End If
            '    End If
            'Next
        Next
    End Sub

    Private Sub possoInserireAggiornare(ByRef dti As DataTable, ByRef canInsertRefresh As Boolean, ByRef nuovaRichiesta As String, ByRef msg As String)
        If dti.Rows.Count > 0 Then
            Dim richiesta As String = CStr(dti.Rows(0).Item("TipoRichiesta"))
            Dim stato As Integer = CInt(dti.Rows(0).Item("Gias_Stato"))
            Select Case richiesta
                Case "I"
                    'In questi stati non si può modificare
                    If stato = 18000001 Or _
                       stato = 18000002 Or _
                       stato = 18000004 Or _
                       stato = 18000005 Or _
                       stato = 18000006 Or _
                       stato = 18000010 Then
                        msg += "Non modificabile Attualmente"
                        canInsertRefresh = False
                    End If
                    'Stati di Errore, Inserisco
                    If stato = 18000003 Or _
                       stato = 18000007 Or _
                       stato = 18000008 Then
                        msg += "Inserito"
                        canInsertRefresh = True
                        nuovaRichiesta = "I"
                    End If

                    'Valida, Aggiorno
                    If stato = 18000009 Then
                        msg += "Aggiornato"
                        canInsertRefresh = True
                        nuovaRichiesta = "A"
                    End If

                    If Not (stato = 18000001 Or
                        stato = 18000002 Or
                        stato = 18000003 Or
                        stato = 18000004 Or
                        stato = 18000005 Or
                        stato = 18000006 Or
                        stato = 18000007 Or
                        stato = 18000008 Or
                        stato = 18000009 Or
                        stato = 180000010) Then
                        msg += "Inserito"
                        canInsertRefresh = True
                        nuovaRichiesta = "I"
                    End If


                Case "A"
                    'In questi stati non si può modificare
                    If stato = 18000001 Or _
                       stato = 18000002 Or _
                       stato = 18000004 Or _
                       stato = 18000005 Or _
                       stato = 18000006 Or _
                       stato = 18000010 Then
                        msg += "Non modificabile Attualmente"
                        canInsertRefresh = False
                    End If
                    'Stati di Errore, Aggiorno
                    If stato = 18000003 Or _
                       stato = 18000007 Or _
                       stato = 18000008 Then
                        msg += "Aggiorno"
                        canInsertRefresh = True
                        nuovaRichiesta = "A"
                    End If

                    'Valida, Aggiorno
                    If stato = 18000009 Then
                        msg += "Aggiorno"
                        canInsertRefresh = True
                        nuovaRichiesta = "A"
                    End If
                Case "E"
                    'In questi stati non si può modificare
                    If stato = 18000001 Or _
                       stato = 18000002 Or _
                       stato = 18000004 Or _
                       stato = 18000005 Or _
                       stato = 18000006 Or _
                       stato = 18000010 Then
                        msg += "Non modificabile Attualmente"
                        canInsertRefresh = False
                    End If
                    'Stati di Errore, Aggiorno
                    If stato = 18000003 Or _
                       stato = 18000007 Or _
                       stato = 18000008 Then
                        msg += "Aggiorno"
                        canInsertRefresh = True
                        nuovaRichiesta = "A"
                    End If

                    'Valida, Inserisco
                    If stato = 18000011 Then
                        msg += "Inserito"
                        canInsertRefresh = True
                        nuovaRichiesta = "I"
                    End If
            End Select
        Else
            msg += "Inserito"
            canInsertRefresh = True
            nuovaRichiesta = "I"
        End If
    End Sub

    Public Overrides Sub inizializzaReaderWriter()
        reader = New xDBSoggSiRPV_R()
        writer = New xDBSoggSiRPV_W()
    End Sub

    Public Overrides Function eliminaTuttoInput() As Boolean
        Dim madeChanges = False
        Dim sogg As New xSoggSiRPV(ObjParametri_Server, ObjParametri_Utenti, Me)
        Dim listaCodOper As List(Of String) = reader.getListaCodOper(ObjParametri_Server)
        For Each codOper As String In listaCodOper
            Dim personaFisica As Boolean = reader.personaFisica(ObjParametri_Server, codOper)
            'trovo soggetti da inviare
            Dim listaSoggettiDaEliminare = listaSoggettiPerRichiesta(2, codOper, Utility.StatoGIAS.creata)
            If listaSoggettiDaEliminare.Count > 0 Then
                Dim soggettiDaEliminare = sogg.sCancSoggSiRPV(username, password, listaSoggettiDaEliminare, codOper, personaFisica)
                'Valido soggetti
                Dim listaSoggettiValidi = listaSoggettiPerRichiesta(2, codOper, Utility.StatoGIAS.valida_per_invio)
                If listaSoggettiValidi.Count > 0 Then
                    'Dim soggettiValidi = sogg.sSoggSiRPV(username, password, listaSoggettiPerRichiesta(TipoRichiesta.A, codOper, Utility.StatoGIAS.valida_per_invio), TipoRichiesta.A, codOper, personaFisica)
                    Dim soggettiValidi = sogg.sCancSoggSiRPV(username, password, listaSoggettiValidi, codOper, personaFisica)
                    'Chiamo il webService
                    madeChanges = True
                    writer.aggiornaStato(ObjParametri_Server, listaSoggettiValidi, codOper, Utility.StatoGIAS.invio_in_corso)
                    Dim converted = False
                    Dim result As New CancSoggSiRPVOutput
                    Try
                        Dim rispostaInvio = caller.ChiamataWSAsync(soggettiValidi, urlASync, CertificateFile)
                        XMLUtility.getObjectFromResponse(rispostaInvio, result, Utility.getSoapenv)
                        converted = True
                    Catch ex As Exception
                        writer.aggiornaStato(ObjParametri_Server, listaSoggettiValidi, codOper, Utility.StatoGIAS.invio_non_riuscito)
                        writer.inserisciLogInvio(ObjParametri_Server, listaSoggettiValidi, codOper, soggettiValidi, "", Utility.CancSoggSiRPV, True, "")
                        Throw New TeleregistriExceptionResponseParsing("Soggetti", "eliminaTuttoInput " + ex.Message)
                    End Try
                    If converted Then
                        If result.IdTrasmissione Is Nothing Then
                            writer.aggiornaStato(ObjParametri_Server, listaSoggettiValidi, codOper, Utility.StatoGIAS.invio_non_riuscito)
                            writer.inserisciLogInvio(ObjParametri_Server, listaSoggettiValidi, codOper, soggettiValidi, "", Utility.CancSoggSiRPV, True, "")
                        Else
                            writer.aggiornaStato(ObjParametri_Server, listaSoggettiValidi, codOper, Utility.StatoGIAS.invio_effettuato_correttamente)
                            writer.inserisciLogInvio(ObjParametri_Server, listaSoggettiValidi, codOper, soggettiValidi, "", Utility.CancSoggSiRPV, False, result.IdTrasmissione)
                        End If
                    End If
                End If
            End If
        Next
        Return madeChanges
    End Function

    Public Overrides Function eliminaTuttoOutput() As Boolean
        Dim madeChanges = False
        Dim op = reader.getOperazioniDaControllare(ObjParametri_Server, Utility.CancSoggSiRPV)
        For Each operation As DataRow In op.Rows
            Dim sogg As New xSoggSiRPV(ObjParametri_Server, ObjParametri_Utenti, Me)
            Dim processed = False
            Dim checkResult As String
            Dim checkRes As New GetCancSoggSiRPVOutput
            Dim received = False
            Dim listaSoggettiDaAggiornare As List(Of String) = reader.listaSoggettiDaAggiornare(ObjParametri_Server, operation.Item("ws_RegVino_LogInvio_Cod"))
            Dim codOper As String = reader.getCodOperFromLogInvioID(ObjParametri_Server, operation.Item("ws_RegVino_LogInvio_Cod"))
            madeChanges = True
            Try
                checkResult = caller.ChiamataWSSync(sogg.sGetCancSoggSiRPV(username, password, operation.Item("idTrasmissione_SIAN")), urlSync, CertificateFile)
                XMLUtility.getObjectFromResponse(checkResult, checkRes, Utility.getSoapenv)
            Catch ex As Exception
                utils.AggiornaControllataLogInvio(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), False)
                utils.inserisciErrore(ObjParametri_Server, operation.Item("ws_RegVino_LogInvio_Cod"), "EXP", "XML Parse Element Response Exception on Operation:" + checkRes.GetType.Name, 0, "", "", "", codOper)
                writer.aggiornaStato(ObjParametri_Server, listaSoggettiDaAggiornare, codOper, Utility.StatoGIAS.in_fase_di_verifica)
                Throw New TeleregistriExceptionResponseParsing("Soggetti", "eliminaTuttoOutput " + ex.Message)
            End Try
            Try
                If Not checkRes.Esito Is Nothing Then

                    If checkRes.Esito.codice <> "000" Then
                        'Ricevuto dati validi
                        Dim typecode = utils.getTypeCode(ObjParametri_Server, checkRes.Esito.codice)
                        Select Case typecode
                            Case Utility.TipoRitornoErrore
                                utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), checkRes.Esito.codice, checkRes.Esito.messaggio, 0, "", "", "", codOper)
                                writer.aggiornaStato(ObjParametri_Server, listaSoggettiDaAggiornare, codOper, Utility.StatoGIAS.errori_rilevati_dal_Sian)
                            Case Utility.TipoRitornoWarning
                                utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), checkRes.Esito.codice, checkRes.Esito.messaggio, 0, "", "", "", codOper)
                                writer.aggiornaStato(ObjParametri_Server, listaSoggettiDaAggiornare, codOper, Utility.StatoGIAS.errori_rilevati_dal_Sian)
                            Case Utility.TipoRitornoInfo
                            Case Else
                                utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), checkRes.Esito.codice, checkRes.Esito.messaggio, 0, "", "", "", codOper)
                                writer.aggiornaStato(ObjParametri_Server, listaSoggettiDaAggiornare, codOper, Utility.StatoGIAS.errori_rilevati_dal_Sian)
                        End Select
                        utils.AggiornaControllataLogInvio(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), True)
                        received = True
                    Else
                        'Richiesta non ancora valutata
                        writer.aggiornaStato(ObjParametri_Server, listaSoggettiDaAggiornare, codOper, Utility.StatoGIAS.in_fase_di_verifica)
                    End If
                Else
                    utils.AggiornaControllataLogInvio(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), True)
                    received = True
                End If
                If received Then
                    For Each soggRes As SoggettoOutput In checkRes.SoggettoOutput
                        Dim tipoRitorno As String = utils.getTypeCode(ObjParametri_Server, soggRes.Esito.codice)
                        Select Case tipoRitorno
                            Case Utility.TipoRitornoErrore
                                utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), soggRes.Esito.codice, soggRes.Esito.messaggio, 0, soggRes.CodiceSoggetto, "", "", codOper)
                                writer.aggiornaStato(ObjParametri_Server, soggRes.CodiceSoggetto, codOper, Utility.StatoGIAS.errori_rilevati_dal_Sian)
                            Case Utility.TipoRitornoWarning
                                utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), soggRes.Esito.codice, soggRes.Esito.messaggio, 0, soggRes.CodiceSoggetto, "", "", codOper)
                                writer.aggiornaStato(ObjParametri_Server, soggRes.CodiceSoggetto, codOper, Utility.StatoGIAS.errori_rilevati_dal_Sian)
                            Case Utility.TipoRitornoInfo
                                writer.aggiornaStato(ObjParametri_Server, soggRes.CodiceSoggetto, codOper, Utility.StatoGIAS.eliminata_nel_sian)
                            Case Else
                                utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), soggRes.Esito.codice, soggRes.Esito.messaggio, 0, soggRes.CodiceSoggetto, "", "", codOper)
                                writer.aggiornaStato(ObjParametri_Server, soggRes.CodiceSoggetto, codOper, Utility.StatoGIAS.errori_rilevati_dal_Sian)
                        End Select
                    Next
                End If
            Catch ex As Exception
                utils.AggiornaControllataLogInvio(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), False)
            End Try
        Next
        Return madeChanges
    End Function

    Public Overrides Function inserisciAggiornaTuttoInput(tipoRichiesta As Integer) As Boolean
        Dim sogg As New xSoggSiRPV(ObjParametri_Server, ObjParametri_Utenti, Me)
        Dim listaCodOper As List(Of String) = reader.getListaCodOper(ObjParametri_Server)
        Dim madeChanges = False
        For Each codOper As String In listaCodOper
            Dim personaFisica As Boolean = reader.personaFisica(ObjParametri_Server, codOper)
            'trovo soggetti da inviare
            Dim listaSoggettiDaInviare = listaSoggettiPerRichiesta(tipoRichiesta, codOper, Utility.StatoGIAS.creata)
            If listaSoggettiDaInviare.Count > 0 Then
                Dim soggettiDaInviare = sogg.sSoggSiRPV(username, password, listaSoggettiDaInviare, tipoRichiesta, codOper, personaFisica)

                'Valido soggetti
                Dim listaSoggettiValidi = listaSoggettiPerRichiesta(tipoRichiesta, codOper, Utility.StatoGIAS.valida_per_invio)
                If listaSoggettiValidi.Count > 0 Then
                    Dim soggettiValidi = sogg.sSoggSiRPV(username, password, listaSoggettiPerRichiesta(tipoRichiesta, codOper, Utility.StatoGIAS.valida_per_invio), tipoRichiesta, codOper, personaFisica)

                    'Chiamo il webService
                    writer.aggiornaStato(ObjParametri_Server, listaSoggettiValidi, codOper, Utility.StatoGIAS.invio_in_corso)
                    madeChanges = False
                    Dim converted = False
                    Dim result As New SoggSiRPVOutput
                    Try
                        Dim rispostaInvio = caller.ChiamataWSAsync(soggettiValidi, urlASync, CertificateFile)
                        XMLUtility.getObjectFromResponse(rispostaInvio, result, Utility.getSoapenv)
                        converted = True
                    Catch ex As Exception
                        writer.aggiornaStato(ObjParametri_Server, listaSoggettiValidi, codOper, Utility.StatoGIAS.invio_non_riuscito)
                        writer.inserisciLogInvio(ObjParametri_Server, listaSoggettiValidi, codOper, soggettiValidi, "", Utility.SoggSiRPV, True, "")
                        Throw New TeleregistriExceptionResponseParsing("Soggetti", "inserisciAggiornaTuttoInput " + ex.Message)
                    End Try
                    If converted Then
                        If result.IdTrasmissione Is Nothing Then
                            writer.aggiornaStato(ObjParametri_Server, listaSoggettiValidi, codOper, Utility.StatoGIAS.invio_non_riuscito)
                            writer.inserisciLogInvio(ObjParametri_Server, listaSoggettiValidi, codOper, soggettiValidi, "", Utility.SoggSiRPV, True, "")
                        Else
                            writer.aggiornaStato(ObjParametri_Server, listaSoggettiValidi, codOper, Utility.StatoGIAS.invio_effettuato_correttamente)
                            writer.inserisciLogInvio(ObjParametri_Server, listaSoggettiValidi, codOper, soggettiValidi, "", Utility.SoggSiRPV, False, result.IdTrasmissione)
                        End If
                    End If
                End If
            End If
        Next
        Return madeChanges
    End Function

    Public Overrides Function inserisciAggiornaTuttoOutput() As Boolean
        Dim op = reader.getOperazioniDaControllare(ObjParametri_Server, Utility.SoggSiRPV)
        Dim madeChanges = False
        For Each operation As DataRow In op.Rows
            Dim sogg As New xSoggSiRPV(ObjParametri_Server, ObjParametri_Utenti, Me)
            Dim processed = False
            Dim checkResult As String
            Dim checkRes As New GetSoggSiRPVOutput
            Dim received = False
            Dim listaSoggettiDaAggiornare As List(Of String) = reader.listaSoggettiDaAggiornare(ObjParametri_Server, operation.Item("ws_RegVino_LogInvio_Cod"))
            Dim codOper As String = reader.getCodOperFromLogInvioID(ObjParametri_Server, operation.Item("ws_RegVino_LogInvio_Cod"))
            madeChanges = True
            Try
                checkResult = caller.ChiamataWSSync(sogg.sGetSoggSiRPV(username, password, operation.Item("idTrasmissione_SIAN")), urlSync, CertificateFile)
                XMLUtility.getObjectFromResponse(checkResult, checkRes, Utility.getSoapenv)
            Catch ex As Exception
                utils.AggiornaControllataLogInvio(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), False)
                utils.inserisciErrore(ObjParametri_Server, operation.Item("ws_RegVino_LogInvio_Cod"), "EXP", "XML Parse Element Response Exception on Operation:" + checkRes.GetType.Name, 0, "", "", "", codOper)
                writer.aggiornaStato(ObjParametri_Server, listaSoggettiDaAggiornare, codOper, Utility.StatoGIAS.in_fase_di_verifica)
                Throw New TeleregistriExceptionResponseParsing("Soggetti", "inserisciAggiornaTuttoOutput " + ex.Message)
            End Try
            Try
                If Not checkRes.Esito Is Nothing Then

                    If checkRes.Esito.codice <> "000" Then
                        'Ricevuto dati validi
                        Dim typecode = utils.getTypeCode(ObjParametri_Server, checkRes.Esito.codice)
                        Select Case typecode
                            Case Utility.TipoRitornoErrore
                                utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), checkRes.Esito.codice, checkRes.Esito.messaggio, 0, "", "", "", codOper)
                                writer.aggiornaStato(ObjParametri_Server, listaSoggettiDaAggiornare, codOper, Utility.StatoGIAS.errori_rilevati_dal_Sian)
                            Case Utility.TipoRitornoWarning
                                utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), checkRes.Esito.codice, checkRes.Esito.messaggio, 0, "", "", "", codOper)
                                writer.aggiornaStato(ObjParametri_Server, listaSoggettiDaAggiornare, codOper, Utility.StatoGIAS.errori_rilevati_dal_Sian)
                            Case Utility.TipoRitornoInfo
                            Case Else
                                utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), checkRes.Esito.codice, checkRes.Esito.messaggio, 0, "", "", "", codOper)
                                writer.aggiornaStato(ObjParametri_Server, listaSoggettiDaAggiornare, codOper, Utility.StatoGIAS.errori_rilevati_dal_Sian)
                        End Select
                        utils.AggiornaControllataLogInvio(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), True)
                        received = True
                    Else
                        'Richiesta non ancora valutata
                        writer.aggiornaStato(ObjParametri_Server, listaSoggettiDaAggiornare, codOper, Utility.StatoGIAS.in_fase_di_verifica)
                    End If
                Else
                    utils.AggiornaControllataLogInvio(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), True)
                    received = True
                End If
                If received Then
                    Try
                        If checkRes.Soggetto IsNot Nothing Then
                            For Each soggRes As SoggettoOutput In checkRes.Soggetto
                                Dim tipoRitorno As String = utils.getTypeCode(ObjParametri_Server, soggRes.Esito.codice)
                                Select Case tipoRitorno
                                    Case Utility.TipoRitornoErrore
                                        utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), soggRes.Esito.codice, soggRes.Esito.messaggio, 0, soggRes.CodiceSoggetto, "", "", codOper)
                                        writer.aggiornaStato(ObjParametri_Server, soggRes.CodiceSoggetto, codOper, Utility.StatoGIAS.errori_rilevati_dal_Sian)
                                    Case Utility.TipoRitornoWarning
                                        utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), soggRes.Esito.codice, soggRes.Esito.messaggio, 0, soggRes.CodiceSoggetto, "", "", codOper)
                                        writer.aggiornaStato(ObjParametri_Server, soggRes.CodiceSoggetto, codOper, Utility.StatoGIAS.errori_rilevati_dal_Sian)
                                    Case Utility.TipoRitornoInfo
                                        writer.aggiornaStato(ObjParametri_Server, soggRes.CodiceSoggetto, codOper, Utility.StatoGIAS.valida_nel_sian)
                                    Case Else
                                        utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), soggRes.Esito.codice, soggRes.Esito.messaggio, 0, soggRes.CodiceSoggetto, "", "", codOper)
                                        writer.aggiornaStato(ObjParametri_Server, soggRes.CodiceSoggetto, codOper, Utility.StatoGIAS.errori_rilevati_dal_Sian)
                                End Select
                            Next
                        Else
                            Dim listaSoggetti = CInt(operation.Item("ws_RegVino_LogInvio_Cod"))
                            writer.aggiornaStato(ObjParametri_Server, listaSoggetti, codOper, Utility.StatoGIAS.errori_rilevati_dal_Sian)
                        End If
                    Catch ex As Exception
                        Dim listaSoggetti = CInt(operation.Item("ws_RegVino_LogInvio_Cod"))
                        writer.aggiornaStato(ObjParametri_Server, listaSoggetti, codOper, Utility.StatoGIAS.errori_rilevati_dal_Sian)
                    End Try
                End If
            Catch ex As Exception
                utils.AggiornaControllataLogInvio(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), False)
            End Try
        Next
        Return madeChanges
    End Function

    Private Function listaSoggettiPerRichiesta(trichiesta As Integer, codOper As String, ByVal statoGIAS As Integer) As List(Of String)
        Dim dt As DataTable
        Dim list As New List(Of String)
        Select Case trichiesta
            Case TipoRichiesta.A
                dt = reader.LeggiSoggetti("", Nothing, ObjParametri_Server, codOper, Nothing, statoGIAS, "A")
            Case TipoRichiesta.I
                dt = reader.LeggiSoggetti("", Nothing, ObjParametri_Server, codOper, Nothing, statoGIAS, "I")
            Case 2
                dt = reader.LeggiSoggetti("", Nothing, ObjParametri_Server, codOper, Nothing, statoGIAS, "E")
        End Select
        For Each row As DataRow In dt.Rows
            list.Add(row.Item(0))
        Next
        Return list
    End Function

End Class
