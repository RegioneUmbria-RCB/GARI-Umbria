Imports AgronicaCoreRegVinoDAL
Imports AgronicaCoreUtility

Public Class Operazioni
    Inherits TeleregistriManager

    Dim reader As xDBOperazioniSiRPV_R
    Dim writer As xDBOperazioniSiRPV_W

    Dim startOperationIndex As Integer
    Dim stepOperationIndex As Integer

    Dim sendAllAttr

    Public Sub New(user As String, pwd As String, urlS As String, urlA As String, server As AgronicaCoreDataProvider.AgronicaCoreParametri, utenti As AgronicaCoreDataProvider.AgronicaCoreParametri, _Configurazione_Servizio As AgronicaCoreVarieDAL.Configurazione_Servizio, ByVal sendAllAttr As Boolean, CertificateFile As String, startOperationIndex As Integer, stepOperationIndex As Integer)
        MyBase.New(user, pwd, urlS, urlA, server, utenti, _Configurazione_Servizio, CertificateFile)
        Me.sendAllAttr = sendAllAttr
        Me.startOperationIndex = startOperationIndex
        Me.stepOperationIndex = stepOperationIndex
    End Sub

    Public Overrides Sub inizializzaReaderWriter()
        reader = New xDBOperazioniSiRPV_R
        writer = New xDBOperazioniSiRPV_W
    End Sub

    Public Overrides Sub checkResult()
        Dim dti = reader.LeggiOperazioniModificate(ObjParametri_Server, 1)
        Dim listaCodOper = reader.getListaCodOper(ObjParametri_Server)
        For Each codOper In listaCodOper
            If dti.Rows.Count > 0 Then
                For Each operazioneDB As DataRow In dti.Rows
                    IAOperazione(operazioneDB.Item("ws_RegVino_Operazione_cod"), codOper, New AgronicaCoreStampeDAL.Cantina_RegistriTelematici, ObjParametri_Server)
                Next
            End If
        Next

        Dim dtel = reader.LeggiOperazioniEliminate(ObjParametri_Server, 1)
        For Each codOper In listaCodOper
            If dtel.Rows.Count > 0 Then
                For Each operazioneDB As DataRow In dtel.Rows
                    If operazioneDB.Item("GIAS_Stato") <> Utility.StatoGIAS.errori_rilevati_dal_Sian Then
                        EOperazione(operazioneDB.Item("ws_RegVino_Operazione_cod"), codOper, New AgronicaCoreStampeDAL.Cantina_RegistriTelematici, ObjParametri_Server)
                    End If
                Next
            End If
        Next

        Dim eli = reader.leggiOperazioniStato(ObjParametri_Server, Utility.StatoGIAS.eliminata_nel_sian)
        For Each codoper In listaCodOper
            If eli.Rows.Count > 0 Then
                For Each operazioneDb As DataRow In eli.Rows
                    writer.aggiornaStato(ObjParametri_Server, operazioneDb.Item("ws_RegVino_Operazione_cod"), 18000000)
                Next
            End If
        Next

        'Dim eli2 = reader.leggiOperazioniDaEliminare(ObjParametri_Server)
        'For Each codoper In listaCodOper
        '    If eli2.Rows.Count > 0 Then
        '        For Each operazioneDb As DataRow In eli2.Rows
        '            writer.eliminaOperazione(ObjParametri_Server, operazioneDb.Item("ws_RegVino_Operazione_cod"))
        '        Next
        '    End If
        'Next

        'Elimino Operazioni dove non esiste ID_Agenda
        Dim noAgenda = reader.leggiOperazioniSenzaID_Agenda(ObjParametri_Server)
        For Each oper As DataRow In noAgenda.Rows
            writer.eliminaOperazione(ObjParametri_Server, oper.Item("ws_regVino_Operazione_Cod"))
        Next


    End Sub

    Function IAOperazione(id As String, codOper As String, objCoreStampeDAL As AgronicaCoreStampeDAL.Cantina_RegistriTelematici, objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String
        Dim reader As New xDBOperazioniSiRPV_R
        Dim writer As New xDBOperazioniSiRPV_W
        Dim msg As String = id + ": "
        Dim dti As DataTable = reader.LeggiWs_RegVino_Operazioni(objParametri_Server, id)
        Dim canInsertUpdate = False
        Dim nuovaRichiesta As String = ""
        possoInserireAggiornare(dti, canInsertUpdate, nuovaRichiesta, msg)
        If canInsertUpdate Then
            Dim insertCom As Boolean = True
            Dim insertDest As Boolean = True
            Dim insertForn As Boolean = True
            Dim committente = dti.Rows(0).Item("CodCommittente")
            If committente <> "" Then
                insertCom = controllaSoggetto(committente, objParametri_Server)
            End If
            Dim fornitore = dti.Rows(0).Item("CodFornitore")
            If fornitore <> "" Then
                insertForn = controllaSoggetto(fornitore, objParametri_Server)
            End If
            Dim destinatario = dti.Rows(0).Item("CodDestinatario")
            If destinatario <> "" Then
                insertDest = controllaSoggetto(destinatario, objParametri_Server)
            End If
            If insertCom And insertDest And insertForn Then
                writer.InserisciAggiornaWs_RegVino_Operazioni(objParametri_Server, id, nuovaRichiesta, 18000001)
                writer.aggiornaModificato(objParametri_Server, id, 0)
            Else
                msg = id + ": Soggetto coinvolto nell'operazione non ancora telematizzato"
            End If
        End If
        Return msg
    End Function

    Function EOperazione(id As String, pivasu As String, objCoreStampeDAL As AgronicaCoreStampeDAL.Cantina_RegistriTelematici, objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String
        Dim reader As New xDBOperazioniSiRPV_R
        Dim writer As New xDBOperazioniSiRPV_W
        Dim msg As String = id + ": "
        Dim dti = reader.LeggiWs_RegVino_Operazioni(objParametri_Server, id)
        Dim canDelete = False
        Dim nuovaRichiesta As String = ""
        Dim richiesta As String = CStr(dti.Rows(0).Item("TipoRichiesta"))
        Dim stato As Integer = CInt(dti.Rows(0).Item("Gias_Stato"))
        If stato = 0 Or stato = -1 Or stato = 18000000 Then
            writer.aggiornaEliminato(objParametri_Server, id, 0)
            writer.aggiornaStato(objParametri_Server, id, Utility.StatoGIAS.eliminata_nel_sian)
        End If
        possoEliminare(dti, canDelete, nuovaRichiesta, msg)
        If canDelete Then
            writer.EliminaWs_RegVino_Operazioni(objParametri_Server, id)
            writer.aggiornaEliminato(objParametri_Server, id, 0)
        End If
        Return msg
    End Function

    Public Overrides Function eliminaTuttoInput() As Boolean
        Dim madeChanges = False
        Dim listaCodOper = reader.getListaCodOper(ObjParametri_Server)
        For Each codOper In listaCodOper
            Dim ope As New xOperSiRPV(ObjParametri_Server, ObjParametri_Utenti, sendAllAttr, Me)
            Dim listaCodIcqrf As List(Of String) = reader.getListaCodIcqrf(ObjParametri_Server, codOper)
            For Each codIcqrf As String In listaCodIcqrf
                Dim personaFisica As Boolean = reader.personaFisica(ObjParametri_Server, codOper)

                Dim listaOperazioniDaEliminare = listaOperazioniPerRichiesta(2, codIcqrf, Utility.StatoGIAS.creata, codOper)
                If listaOperazioniDaEliminare.Count > 0 Then
                    Dim operazioniDaEliminare = ope.sCancOperSiRPV(username, password, codIcqrf, codOper, personaFisica, listaOperazioniDaEliminare)

                    Dim listaOperazioniValide = listaOperazioniPerRichiesta(2, codIcqrf, Utility.StatoGIAS.valida_per_invio, codOper)
                    If listaOperazioniValide.Count > 0 Then

                        Dim operazioniValide = ope.sCancOperSiRPV(username, password, codIcqrf, codOper, personaFisica, listaOperazioniValide)
                        'Chiamo il webService
                        writer.aggiornaStato(ObjParametri_Server, listaOperazioniValide, Utility.StatoGIAS.invio_in_corso)
                        madeChanges = True
                        Dim converted = False
                        Dim result As New CancOperSiRPVOutput
                        Try
                            Dim rispostaInvio = caller.ChiamataWSAsync(operazioniValide, urlASync, CertificateFile)
                            XMLUtility.getObjectFromResponse(rispostaInvio, result, Utility.getSoapenv())
                            converted = True
                        Catch ex As Exception
                            writer.aggiornaStato(ObjParametri_Server, listaOperazioniValide, Utility.StatoGIAS.invio_non_riuscito)
                            writer.inserisciLogInvio(ObjParametri_Server, listaOperazioniValide, operazioniValide, "", Utility.CancOperSiRPV, True, "")
                            Throw New TeleregistriExceptionResponseParsing("Operazioni", "eliminaTuttoInput " + ex.Message)
                        End Try
                        If converted Then
                            If result.IdTrasmissione Is Nothing Then
                                writer.aggiornaStato(ObjParametri_Server, listaOperazioniValide, Utility.StatoGIAS.invio_non_riuscito)
                                writer.inserisciLogInvio(ObjParametri_Server, listaOperazioniValide, operazioniValide, "", Utility.CancOperSiRPV, True, "")
                            Else
                                writer.aggiornaStato(ObjParametri_Server, listaOperazioniValide, Utility.StatoGIAS.invio_effettuato_correttamente)
                                writer.inserisciLogInvio(ObjParametri_Server, listaOperazioniValide, operazioniValide, "", Utility.CancOperSiRPV, False, result.IdTrasmissione)
                            End If
                        End If
                    End If
                End If
            Next
        Next
        Return madeChanges
    End Function

    Public Overrides Function eliminaTuttoOutput() As Boolean
        Dim madeChanges = False
        Dim op = reader.getOperazioniDaControllare(ObjParametri_Server, Utility.CancOperSiRPV)
        For Each operation As DataRow In op.Rows
            Dim ope As New xOperSiRPV(ObjParametri_Server, ObjParametri_Utenti, sendAllAttr, Me)
            Dim processed = False
            Dim checkResult As String
            Dim checkRes As New GetCancOperSiRPVOutput
            Dim received = False
            Dim listaOperazioniDaAggiornare As List(Of String) = reader.listaOperazioniDaAggiornare(ObjParametri_Server, operation.Item("ws_RegVino_LogInvio_Cod"))
            Dim codIcqrf As String = reader.getCodIcqrfFromLogInvioID(ObjParametri_Server, operation.Item("ws_RegVino_LogInvio_Cod"))
            Dim codOper As String = reader.getCodOperFromCodIcqrf(ObjParametri_Server, codIcqrf)
            madeChanges = True
            Try
                checkResult = caller.ChiamataWSSync(ope.sGetCancOperSiRPV(username, password, operation.Item("idTrasmissione_SIAN")), urlSync, CertificateFile)
                XMLUtility.getObjectFromResponse(checkResult, checkRes, Utility.getSoapenv)
            Catch ex As Exception
                logga(2, "AgronicaCoreRegVinoBIZ.Operazioni.eliminaTuttoOutput",
                      $"Aggiorna controllata da log invio: Operazioni OP: {CStr(CInt(operation.Item("ws_RegVino_LogInvio_Cod")))} riga 171")
                utils.AggiornaControllataLogInvio(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), False)
                utils.inserisciErrore(ObjParametri_Server, operation.Item("ws_RegVino_LogInvio_Cod"), "EXP", "XML Parse Element Response Exception on Operation:" + checkRes.GetType.Name, 0, "", "", "", codOper)
                writer.aggiornaStato(ObjParametri_Server, listaOperazioniDaAggiornare, Utility.StatoGIAS.in_fase_di_verifica)
                Throw New TeleregistriExceptionResponseParsing("Operazioni", "eliminaTuttoOutput " + ex.Message)
            End Try
            Try
                If Not checkRes.Esito Is Nothing Then

                    If checkRes.Esito.codice <> "000" Then
                        'Ricevuto dati validi
                        Dim typecode = utils.getTypeCode(ObjParametri_Server, checkRes.Esito.codice)
                        Select Case typecode
                            Case Utility.TipoRitornoErrore
                                utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), checkRes.Esito.codice, checkRes.Esito.messaggio, 0, "", "", "", codOper)
                                writer.aggiornaStato(ObjParametri_Server, listaOperazioniDaAggiornare, Utility.StatoGIAS.errori_rilevati_dal_Sian)
                            Case Utility.TipoRitornoWarning
                                utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), checkRes.Esito.codice, checkRes.Esito.messaggio, 0, "", "", "", codOper)
                                writer.aggiornaStato(ObjParametri_Server, listaOperazioniDaAggiornare, Utility.StatoGIAS.errori_rilevati_dal_Sian)
                            Case Utility.TipoRitornoInfo

                            Case Else
                                utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), checkRes.Esito.codice, checkRes.Esito.messaggio, 0, "", "", "", codOper)
                                writer.aggiornaStato(ObjParametri_Server, listaOperazioniDaAggiornare, Utility.StatoGIAS.errori_rilevati_dal_Sian)
                        End Select
                        utils.AggiornaControllataLogInvio(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), True)
                        received = True
                    Else
                        'Richiesta non ancora valutata
                        writer.aggiornaStato(ObjParametri_Server, listaOperazioniDaAggiornare, Utility.StatoGIAS.in_fase_di_verifica)
                    End If
                Else
                    utils.AggiornaControllataLogInvio(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), True)
                    received = True
                End If
                If received Then
                    If checkRes.EliminaOperazioneOutput IsNot Nothing Then
                        For Each operRes As EliminaOperazioneOutput In checkRes.EliminaOperazioneOutput
                            Dim tipoRitorno As String = utils.getTypeCode(ObjParametri_Server, operRes.Esito.codice)
                            Dim codiceOperazione As Integer = reader.getCodiceOperazioneFromControlloEsito(ObjParametri_Server, operRes.DataOperazione, operRes.NumOperazione, operRes.CodOperazione)
                            If codiceOperazione <> 0 Then
                                Select Case tipoRitorno
                                    Case Utility.TipoRitornoErrore
                                        utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), operRes.Esito.codice, operRes.Esito.messaggio, codiceOperazione, "", "", "", codOper)
                                        writer.aggiornaStato(ObjParametri_Server, codiceOperazione, Utility.StatoGIAS.errori_rilevati_dal_Sian)
                                    Case Utility.TipoRitornoWarning
                                        utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), operRes.Esito.codice, operRes.Esito.messaggio, codiceOperazione, "", "", "", codOper)
                                        writer.aggiornaStato(ObjParametri_Server, codiceOperazione, Utility.StatoGIAS.errori_rilevati_dal_Sian)
                                    Case Utility.TipoRitornoInfo
                                        'writer.aggiornaStato(ObjParametri_Server, codiceOperazione, Utility.StatoGIAS.eliminata_nel_sian)
                                        writer.InserisciAggiornaWs_RegVino_Operazioni(ObjParametri_Server, codiceOperazione, "I", Utility.StatoGIAS.eliminata_nel_sian)
                                    Case Else
                                        utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), operRes.Esito.codice, operRes.Esito.messaggio, codiceOperazione, "", "", "", codOper)
                                        writer.aggiornaStato(ObjParametri_Server, codiceOperazione, Utility.StatoGIAS.errori_rilevati_dal_Sian)
                                End Select
                            Else
                                utils.AggiornaControllataLogInvio(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), True)
                            End If
                        Next
                    End If

                End If
            Catch ex As Exception
                utils.AggiornaControllataLogInvio(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), False)
            End Try
        Next
        Return madeChanges
    End Function

    Public Overrides Function inserisciAggiornaTuttoInput(tipoRichiesta As Integer) As Boolean
        Dim madeChanges = False
        Dim listaCodOper = reader.getListaCodOper(ObjParametri_Server)
        For Each codOper In listaCodOper
            Dim operazioni As New xOperSiRPV(ObjParametri_Server, ObjParametri_Utenti, sendAllAttr, Me)
            Dim listaCodIcqrf As List(Of String) = reader.getListaCodIcqrf(ObjParametri_Server, codOper)
            For Each codIcqrf As String In listaCodIcqrf
                Dim personaFisica As Boolean = reader.personaFisica(ObjParametri_Server, codOper)
                'trovo operazioni da inviare
                Dim listaOperazionidaInviare = listaOperazioniPerRichiesta(tipoRichiesta, codIcqrf, Utility.StatoGIAS.creata, codOper)
                If listaOperazionidaInviare.Count > 0 Then
                    'Calcolo il progressivo delle operazioni da inviare direttamente rileggendole dal DB (da modificare)
                    If tipoRichiesta = AgronicaCoreRegVinoBIZ.TipoRichiesta.I Then
                        CalcolaProgressiviOperazioniDaInviare()
                    End If
                    Dim operazioniDaInviare = operazioni.sOperSiRPV(username, password, listaOperazionidaInviare, tipoRichiesta, codIcqrf, codOper, personaFisica)

                    'Valido operazioni
                    Dim listaOperazioniValide = listaOperazioniPerRichiesta(tipoRichiesta, codIcqrf, Utility.StatoGIAS.valida_per_invio, codOper)
                    If listaOperazioniValide.Count > 0 Then
                        Dim operazioniValide = operazioni.sOperSiRPV(username, password, listaOperazioniPerRichiesta(tipoRichiesta, codIcqrf, Utility.StatoGIAS.valida_per_invio, codOper), tipoRichiesta, codIcqrf, codOper, personaFisica)

                        'Chiamo il webService
                        writer.aggiornaStato(ObjParametri_Server, listaOperazioniValide, Utility.StatoGIAS.invio_in_corso)
                        madeChanges = True
                        Dim converted = False
                        Dim result As New OperSiRPVOutput
                        Try
                            Dim rispostaInvio = caller.ChiamataWSAsync(operazioniValide, urlASync, CertificateFile)
                            XMLUtility.getObjectFromResponse(rispostaInvio, result, Utility.getSoapenv)
                            converted = True
                        Catch ex As Exception
                            writer.aggiornaStato(ObjParametri_Server, listaOperazioniValide, Utility.StatoGIAS.invio_non_riuscito)
                            writer.inserisciLogInvio(ObjParametri_Server, listaOperazioniValide, operazioniValide, "", Utility.OperSiRPV, True, "")
                            Throw New TeleregistriExceptionResponseParsing("Operazioni", "inserisciAggiornaTuttoInput " + ex.Message)
                        End Try
                        If converted Then
                            If result.IdTrasmissione Is Nothing Then
                                writer.aggiornaStato(ObjParametri_Server, listaOperazioniValide, Utility.StatoGIAS.invio_non_riuscito)
                                writer.inserisciLogInvio(ObjParametri_Server, listaOperazioniValide, operazioniValide, "", Utility.OperSiRPV, True, "")
                            Else
                                writer.aggiornaStato(ObjParametri_Server, listaOperazioniValide, Utility.StatoGIAS.invio_effettuato_correttamente)
                                writer.inserisciLogInvio(ObjParametri_Server, listaOperazioniValide, operazioniValide, "", Utility.OperSiRPV, False, result.IdTrasmissione)
                            End If
                        End If
                    End If
                End If
            Next
        Next
        Return madeChanges
    End Function

    Public Overrides Function inserisciAggiornaTuttoOutput() As Boolean
        Dim madeChanges = False
        Dim op = reader.getOperazioniDaControllare(ObjParametri_Server, Utility.OperSiRPV)
        For Each operation As DataRow In op.Rows
            Dim oper As New xOperSiRPV(ObjParametri_Server, ObjParametri_Utenti, sendAllAttr, Me)
            Dim processed = False
            Dim checkResult As String
            Dim checkRes As New GetOperSiRPVOutput
            Dim received = False
            Dim listaOperazioniDaAggiornare As List(Of String) = reader.listaOperazioniDaAggiornare(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")))
            Dim codIcqrf As String = reader.getCodIcqrfFromLogInvioID(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")))
            Dim codOper As String = reader.getCodOperFromCodIcqrf(ObjParametri_Server, codIcqrf)
            madeChanges = True
            Try
                checkResult = caller.ChiamataWSSync(oper.sGetOperSiRPV(username, password, operation.Item("idTrasmissione_SIAN")), urlSync, CertificateFile)
                XMLUtility.getObjectFromResponse(checkResult, checkRes, Utility.getSoapenv)
            Catch ex As Exception
                utils.AggiornaControllataLogInvio(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), False)
                'utils.inserisciErrore(ObjParametri_Server, operation.Item("ws_RegVino_LogInvio_Cod"), "EXP", "XML Parse Element Response Exception on Operation:" + checkRes.GetType.Name)
                writer.aggiornaStato(ObjParametri_Server, listaOperazioniDaAggiornare, Utility.StatoGIAS.in_fase_di_verifica)
                Throw New TeleregistriExceptionResponseParsing("Operazioni", "inserisciAggiornaTuttoOutput " + ex.Message)
            End Try
            Try
                If Not checkRes.Esito Is Nothing Then

                    If checkRes.Esito.codice <> "000" Then
                        'Ricevuto dati validi
                        Dim typecode = utils.getTypeCode(ObjParametri_Server, checkRes.Esito.codice)
                        Select Case typecode
                            Case Utility.TipoRitornoErrore
                                utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), checkRes.Esito.codice, checkRes.Esito.messaggio, 0, "", "", "", codOper)
                                writer.aggiornaStato(ObjParametri_Server, listaOperazioniDaAggiornare, Utility.StatoGIAS.errori_rilevati_dal_Sian)
                                For Each codiceOperazione In listaOperazioniDaAggiornare
                                    writer.impostaProgressivo(CInt(codiceOperazione), startOperationIndex, ObjParametri_Server)
                                Next
                            Case Utility.TipoRitornoWarning
                                utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), checkRes.Esito.codice, checkRes.Esito.messaggio, 0, "", "", "", codOper)
                                writer.aggiornaStato(ObjParametri_Server, listaOperazioniDaAggiornare, Utility.StatoGIAS.errori_rilevati_dal_Sian)
                                For Each codiceOperazione In listaOperazioniDaAggiornare
                                    writer.impostaProgressivo(CInt(codiceOperazione), startOperationIndex, ObjParametri_Server)
                                Next
                            Case Utility.TipoRitornoInfo
                            Case Else
                                utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), checkRes.Esito.codice, checkRes.Esito.messaggio, 0, "", "", "", codOper)
                                writer.aggiornaStato(ObjParametri_Server, listaOperazioniDaAggiornare, Utility.StatoGIAS.errori_rilevati_dal_Sian)
                                For Each codiceOperazione In listaOperazioniDaAggiornare
                                    writer.impostaProgressivo(CInt(codiceOperazione), startOperationIndex, ObjParametri_Server)
                                Next
                        End Select
                        received = True
                        utils.AggiornaControllataLogInvio(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), True)

                    Else
                        'Richiesta non ancora valutata
                        writer.aggiornaStato(ObjParametri_Server, listaOperazioniDaAggiornare, Utility.StatoGIAS.in_fase_di_verifica)
                    End If
                Else
                    utils.AggiornaControllataLogInvio(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), True)
                    received = True
                End If
                If received Then
                    If checkRes.ControlloEsito IsNot Nothing Then
                        For Each operRes As ControlloEsito In checkRes.ControlloEsito
                            Dim tipoRitorno As String = utils.getTypeCode(ObjParametri_Server, operRes.Esito.codice)
                            Dim codiceOperazione As Integer = reader.getCodiceOperazioneFromControlloEsito(ObjParametri_Server, operRes.DataOperazione, operRes.NumOperazione, operRes.CodOperazione)
                            Select Case tipoRitorno
                                Case Utility.TipoRitornoErrore
                                    utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), operRes.Esito.codice, operRes.Esito.messaggio, codiceOperazione, "", "", "", codOper)
                                    writer.aggiornaStato(ObjParametri_Server, codiceOperazione, Utility.StatoGIAS.errori_rilevati_dal_Sian)
                                    writer.impostaProgressivo(codiceOperazione, startOperationIndex, ObjParametri_Server)
                                Case Utility.TipoRitornoWarning
                                    utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), operRes.Esito.codice, operRes.Esito.messaggio, codiceOperazione, "", "", "", codOper)
                                    writer.aggiornaStato(ObjParametri_Server, codiceOperazione, Utility.StatoGIAS.errori_rilevati_dal_Sian)
                                    writer.impostaProgressivo(codiceOperazione, startOperationIndex, ObjParametri_Server)
                                Case Utility.TipoRitornoInfo
                                    writer.aggiornaStato(ObjParametri_Server, codiceOperazione, Utility.StatoGIAS.valida_nel_sian)
                                Case Else
                                    utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), operRes.Esito.codice, operRes.Esito.messaggio, codiceOperazione, "", "", "", codOper)
                                    writer.aggiornaStato(ObjParametri_Server, codiceOperazione, Utility.StatoGIAS.errori_rilevati_dal_Sian)
                                    writer.impostaProgressivo(codiceOperazione, startOperationIndex, ObjParametri_Server)
                            End Select
                        Next
                    End If
                End If
            Catch ex As Exception
                utils.AggiornaControllataLogInvio(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), False)
            End Try
        Next
        Return madeChanges
    End Function

    Private Function listaOperazioniPerRichiesta(tRichiesta As TipoRichiesta, codIcqrf As String, statoGIAS As Utility.StatoGIAS, codOper As String) As Object
        Dim dt As DataTable
        Dim list As New List(Of String)
        Select Case tRichiesta
            Case TipoRichiesta.A
                dt = reader.LeggiOperazioniPerRichiesta(ObjParametri_Server, statoGIAS, "A", codIcqrf, codOper, "")
            Case TipoRichiesta.I
                dt = reader.LeggiOperazioniPerRichiesta(ObjParametri_Server, statoGIAS, "I", codIcqrf, codOper, "")
            Case 2
                dt = reader.LeggiOperazioniPerRichiesta(ObjParametri_Server, statoGIAS, "E", codIcqrf, codOper, "ORDER BY NumOperazione DESC, DataOperazione DESC, ID_Agenda DESC")
        End Select
        For Each row As DataRow In dt.Rows
            list.Add(row.Item("ws_RegVino_Operazione_cod"))
        Next
        Return list
    End Function

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
                        nuovaRichiesta = "I"
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

    Private Function controllaSoggetto(committente As Object, objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Object
        Dim reader As New xDBSoggSiRPV_R
        Dim dti As DataTable = reader.Leggiws_RegVino_Soggetti(objParametri, committente)
        If dti.Rows.Count > 0 Then
            Dim stato As Integer = dti.Rows(0).Item("GIAS_Stato")
            If stato = 18000009 Then
                Return True
            Else
                Return False
            End If
        End If
        Return False
    End Function

    Private Sub possoEliminare(ByRef dti As DataTable, ByRef canDelete As Boolean, ByRef nuovaRichiesta As String, ByRef msg As String)
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
                        canDelete = False
                    End If
                    'Stati di Errore, Inserisco
                    If stato = 18000003 Or _
                       stato = 18000007 Or _
                       stato = 18000008 Then
                        msg += "Elimino"
                        canDelete = True
                    End If

                    'Valida, Aggiorno
                    If stato = 18000009 Then
                        msg += "Elimino"
                        canDelete = True
                        nuovaRichiesta = "E"
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
                        canDelete = False
                    End If
                    'Stati di Errore, Aggiorno
                    If stato = 18000003 Or _
                       stato = 18000007 Or _
                       stato = 18000008 Then
                        msg += "Elimino"
                        canDelete = True
                        nuovaRichiesta = "E"
                    End If

                    'Valida, Aggiorno
                    If stato = 18000009 Then
                        msg += "Elimino"
                        canDelete = True
                        nuovaRichiesta = "E"
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
                        canDelete = False
                    End If
                    'Stati di Errore, Aggiorno
                    If stato = 18000003 Or _
                       stato = 18000007 Or _
                       stato = 18000008 Then
                        msg += "Elimino"
                        canDelete = True
                        nuovaRichiesta = "E"
                    End If

                    'Valida, Inserisco
                    If stato = 18000011 Or stato = 18000009 Then
                        msg += "Già eliminata precedentemente"
                        canDelete = False
                        nuovaRichiesta = "E"
                    End If
            End Select
        Else
            msg += "Non presente nel SIAN"
            canDelete = False
        End If
    End Sub

    Private Sub CalcolaProgressiviOperazioniDaInviare()
        Dim progressivo = startOperationIndex
        Dim stato = Utility.StatoGIAS.creata
        Dim dt As DataTable = reader.leggiOperazioniDaAggiornare(progressivo, stato, ObjParametri_Server)
        For Each row In dt.Rows
            Dim data_Operazione As Date = CDate(row.item("DataOperazione"))
            Dim MaxProgressivo As Integer = reader.leggiMaxProgressivo(data_Operazione, ObjParametri_Server, startOperationIndex)
            writer.impostaProgressivo(CInt(row.item("ws_RegVino_Operazione_Cod")), MaxProgressivo + stepOperationIndex, ObjParametri_Server)
        Next
    End Sub

End Class
