Imports System.IO
Imports AgronicaCoreRegVinoDAL
Imports AgronicaCoreUtility

Public Class Prodotti
    Inherits TeleregistriManager

    Dim reader As xDBProdottiSiRPV_R
    Dim writer As xDBProdottiSiRPV_W

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
        reader = New xDBProdottiSiRPV_R
        writer = New xDBProdottiSiRPV_W
    End Sub

    Public Overrides Sub checkResult()
        'Dim listaCodOper = reader.getListaCodOper(ObjParametri_Server)
        'For Each CodOper In listaCodOper
        '    Dim listaCodIcqrf As List(Of String) = reader.getListaCodIcqrf(ObjParametri_Server, CodOper)
        '    Dim corretto As Boolean = True
        '    For Each codIcqrf As String In listaCodIcqrf
        '        Dim personaFisica As Boolean = reader.personaFisica(ObjParametri_Server, codOper)
        '        Dim listaCodProd As List(Of String) = reader.getDBProddottiPerCodIcqrf(ObjParametri_Server, codIcqrf, CodOper)

        '        Dim visProd As New VisProdottiSiRPV()
        '        Dim vis = visProd.sVisProdottiSiRPVInput(username, password, codIcqrf, codOper, personaFisica)
        '        Dim rispostaInvio = caller.ChiamataWSSync(vis, urlSync)

        '        Dim converted = False
        '        Dim result As New VisProdSiRPVOutput
        '        Try
        '            XMLUtility.getObjectFromResponse(rispostaInvio, result, Utility.getSoapenv)
        '            converted = True
        '        Catch ex As Exception
        '            Throw New TeleregistriExceptionResponseParsing("Prodotti", "checkResult")
        '        End Try
        '        If converted Then
        '            If Not result.ProdCatalogo Is Nothing Then
        '                Dim listaProdotti As New List(Of String)
        '                For Each prod As VisProdSiRPVOutputProdCatalogo In result.ProdCatalogo
        '                    listaProdotti.Add(prod.CodiceProdotto.CodPrimario + "|" + prod.CodiceProdotto.CodSecondario)
        '                Next

        '                For Each prodInterno As String In listaCodProd
        '                    If listaProdotti.Contains(prodInterno.ToUpper) Then
        '                        listaProdotti.Remove(prodInterno.ToUpper)
        '                    Else
        '                        Throw New TeleregistriExceptionIncoherentDBSincro("Prodotti", "checkResult")
        '                    End If
        '                Next
        '                If listaProdotti.Count <> 0 Then
        '                    Throw New TeleregistriExceptionIncoherentDBSincro("Prodotti", "checkResult")
        '                End If
        '            End If
        '        End If
        '    Next
        'Next
    End Sub

    Public Overrides Function eliminaTuttoInput() As Boolean
        Dim madeChanges = False
        'Dim listaCodOper = reader.getListaCodOper(ObjParametri_Server)
        'For Each codOper In listaCodOper
        '    Dim prod As New xProdottiSiRPV(ObjParametri_Server, ObjParametri_Utenti)
        '    Dim listaCodIcqrf As List(Of String) = reader.getListaCodIcqrf(ObjParametri_Server, codOper)
        '    For Each codIcqrf As String In listaCodIcqrf
        '        Dim personaFisica As Boolean = reader.personaFisica(ObjParametri_Server, codOper)

        '        Dim listaProdottiDaEliminare = listaProdottiPerRichiesta(2, codOper, codIcqrf, Utility.StatoGIAS.creata)
        '        If listaProdottiDaEliminare.Count > 0 Then
        '            Dim prodDaEliminare = prod.sCancProdottiSiRPV(username, password, codIcqrf, codOper, personaFisica, listaProdottiDaEliminare)

        '            Dim listaProdottiValidi = listaProdottiPerRichiesta(2, codOper, codIcqrf, Utility.StatoGIAS.valida_per_invio)
        '            If listaProdottiValidi.Count > 0 Then

        '                Dim prodottiValidi = prod.sCancProdottiSiRPV(username, password, codIcqrf, codOper, personaFisica, listaProdottiValidi)
        '                'Chiamo il webService
        '                writer.aggiornaStato(ObjParametri_Server, listaProdottiValidi, codIcqrf, codOper, Utility.StatoGIAS.invio_in_corso)
        '                Dim rispostaInvio = caller.ChiamataWSAsync(prodottiValidi, urlASync)
        '                madeChanges = True
        '                Dim converted = False
        '                Dim result As New CancProdSiRPVOutput
        '                Try
        '                    XMLUtility.getObjectFromResponse(rispostaInvio, result, Utility.getSoapenv)
        '                    converted = True
        '                Catch ex As Exception
        '                    writer.aggiornaStato(ObjParametri_Server, listaProdottiValidi, codIcqrf, codOper, Utility.StatoGIAS.invio_non_riuscito)
        '                    writer.inserisciLogInvio(ObjParametri_Server, listaProdottiValidi, codIcqrf, prodottiValidi, "", Utility.CancProdSiRPV, True, "", codOper)
        '                    Throw New TeleregistriExceptionResponseParsing("Prodotti", "eliminaTuttoInput")
        '                End Try
        '                If converted Then
        '                    If result.IdTrasmissione Is Nothing Then
        '                        writer.aggiornaStato(ObjParametri_Server, listaProdottiValidi, codIcqrf, codOper, Utility.StatoGIAS.invio_non_riuscito)
        '                        writer.inserisciLogInvio(ObjParametri_Server, listaProdottiValidi, codIcqrf, prodottiValidi, "", Utility.CancProdSiRPV, True, "", codOper)
        '                    Else
        '                        writer.aggiornaStato(ObjParametri_Server, listaProdottiValidi, codIcqrf, codOper, Utility.StatoGIAS.invio_effettuato_correttamente)
        '                        writer.inserisciLogInvio(ObjParametri_Server, listaProdottiValidi, codIcqrf, prodottiValidi, "", Utility.CancProdSiRPV, False, result.IdTrasmissione, codOper)
        '                    End If
        '                End If
        '            End If
        '        End If
        '    Next
        'Next
        Return madeChanges
    End Function

    Public Overrides Function eliminaTuttoOutput() As Boolean
        Dim madeChanges = False
        'Dim op = reader.getOperazioniDaControllare(ObjParametri_Server, Utility.CancProdSiRPV)
        'For Each operation As DataRow In op.Rows
        '    Dim prod As New xProdottiSiRPV(ObjParametri_Server, ObjParametri_Utenti)
        '    Dim processed = False
        '    Dim checkResult As String
        '    Dim checkRes As New GetCancProdSiRPVOutput
        '    Dim received = False
        '    Dim listaProdottiDaAggiornare As List(Of String) = reader.listaProdottiDaAggiornare(ObjParametri_Server, operation.Item("ws_RegVino_LogInvio_Cod"))
        '    Dim codIcqrf As String = reader.getCodIcqrfFromLogInvioID(ObjParametri_Server, operation.Item("ws_RegVino_LogInvio_Cod"))
        '    Dim codOper As String = reader.getCodOperFromCodIcqrf(ObjParametri_Server, codIcqrf)
        '    checkResult = caller.ChiamataWSSync(prod.sGetCancProdSiRPV(username, password, operation.Item("idTrasmissione_SIAN")), urlSync)
        '    madeChanges = True
        '    Try
        '        XMLUtility.getObjectFromResponse(checkResult, checkRes, Utility.getSoapenv)
        '    Catch ex As Exception
        '        utils.AggiornaControllataLogInvio(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), True)
        '        utils.inserisciErrore(ObjParametri_Server, operation.Item("ws_RegVino_LogInvio_Cod"), "EXP", "XML Parse Element Response Exception on Operation:" + checkRes.GetType.Name)
        '        writer.aggiornaStato(ObjParametri_Server, listaProdottiDaAggiornare, codIcqrf, codOper, Utility.StatoGIAS.in_fase_di_verifica)
        '        Throw New TeleregistriExceptionResponseParsing("Prodotti", "eliminaTuttoOutput")
        '    End Try
        '    Try
        '        If Not checkRes.Esito Is Nothing Then

        '            If checkRes.Esito.codice <> "000" Then
        '                'Ricevuto dati validi
        '                Dim typecode = utils.getTypeCode(ObjParametri_Server, checkRes.Esito.codice)
        '                Select Case typecode
        '                    Case Utility.TipoRitornoErrore
        '                        utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), checkRes.Esito.codice, checkRes.Esito.messaggio)
        '                        writer.aggiornaStato(ObjParametri_Server, listaProdottiDaAggiornare, codIcqrf, codOper, Utility.StatoGIAS.errori_rilevati_dal_Sian)
        '                    Case Utility.TipoRitornoWarning
        '                        utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), checkRes.Esito.codice, checkRes.Esito.messaggio)
        '                        writer.aggiornaStato(ObjParametri_Server, listaProdottiDaAggiornare, codIcqrf, codOper, Utility.StatoGIAS.errori_rilevati_dal_Sian)
        '                    Case Utility.TipoRitornoInfo

        '                    Case Else
        '                        utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), checkRes.Esito.codice, checkRes.Esito.messaggio)
        '                        writer.aggiornaStato(ObjParametri_Server, listaProdottiDaAggiornare, codIcqrf, codOper, Utility.StatoGIAS.errori_rilevati_dal_Sian)
        '                End Select
        '                utils.AggiornaControllataLogInvio(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), True)
        '                received = True
        '            Else
        '                'Richiesta non ancora valutata
        '                writer.aggiornaStato(ObjParametri_Server, listaProdottiDaAggiornare, codIcqrf, codOper, Utility.StatoGIAS.in_fase_di_verifica)
        '            End If
        '        Else
        '            utils.AggiornaControllataLogInvio(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), True)
        '            received = True
        '        End If
        '        If received Then
        '            For Each prodRes As EliminaProdCatalogoOutput In checkRes.EliminaProdOutput
        '                Dim tipoRitorno As String = utils.getTypeCode(ObjParametri_Server, prodRes.Esito.codice)
        '                Select Case tipoRitorno
        '                    Case Utility.TipoRitornoErrore
        '                        utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), prodRes.Esito.codice, prodRes.Esito.messaggio)
        '                        writer.aggiornaStato(ObjParametri_Server, prodRes.CodiceProdotto.CodPrimario + "|" + prodRes.CodiceProdotto.CodSecondario, codIcqrf, codOper, Utility.StatoGIAS.errori_rilevati_dal_Sian)
        '                    Case Utility.TipoRitornoWarning
        '                        utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), prodRes.Esito.codice, prodRes.Esito.messaggio)
        '                        writer.aggiornaStato(ObjParametri_Server, prodRes.CodiceProdotto.CodPrimario + "|" + prodRes.CodiceProdotto.CodSecondario, codIcqrf, codOper, Utility.StatoGIAS.errori_rilevati_dal_Sian)
        '                    Case Utility.TipoRitornoInfo
        '                        writer.aggiornaStato(ObjParametri_Server, prodRes.CodiceProdotto.CodPrimario + "|" + prodRes.CodiceProdotto.CodSecondario, codIcqrf, codOper, Utility.StatoGIAS.eliminata_nel_sian)
        '                    Case Else
        '                        utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), prodRes.Esito.codice, prodRes.Esito.messaggio)
        '                        writer.aggiornaStato(ObjParametri_Server, prodRes.CodiceProdotto.CodPrimario + "|" + prodRes.CodiceProdotto.CodSecondario, codIcqrf, codOper, Utility.StatoGIAS.errori_rilevati_dal_Sian)
        '                End Select
        '            Next
        '        End If
        '    Catch ex As Exception
        '        utils.AggiornaControllataLogInvio(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), False)
        '    End Try
        'Next
        Return madeChanges
    End Function

    Public Overrides Function inserisciAggiornaTuttoInput(tipoRichiesta As Integer) As Boolean
        'GestioneModificati()
        Dim madeChanges = False
        'Dim listaCodOper = reader.getListaCodOper(ObjParametri_Server)
        'For Each codOper In listaCodOper
        '    Dim prodotti As New xProdottiSiRPV(ObjParametri_Server, ObjParametri_Utenti)
        '    Dim listaCodIcqrf As List(Of String) = reader.getListaCodIcqrf(ObjParametri_Server, codOper)
        '    For Each codIcqrf As String In listaCodIcqrf
        '        Dim personaFisica As Boolean = reader.personaFisica(ObjParametri_Server, codOper)
        '        'trovo prodotti da inviare
        '        Dim listaProdottidaInviare = listaProdottiPerRichiesta(tipoRichiesta, codOper, codIcqrf, Utility.StatoGIAS.creata)
        '        If listaProdottidaInviare.Count > 0 Then
        '            Dim prodottiDaInviare = prodotti.sProdottiSiRPV(username, password, listaProdottidaInviare, tipoRichiesta, codIcqrf, codOper, personaFisica)

        '            'Valido Prodotti
        '            Dim listaProdottiValidi = listaProdottiPerRichiesta(tipoRichiesta, codOper, codIcqrf, Utility.StatoGIAS.valida_per_invio)
        '            If listaProdottiValidi.Count > 0 Then
        '                Dim prodValidi = prodotti.sProdottiSiRPV(username, password, listaProdottiPerRichiesta(tipoRichiesta, codOper, codIcqrf, Utility.StatoGIAS.valida_per_invio), tipoRichiesta, codIcqrf, codOper, personaFisica)

        '                'Chiamo il webService
        '                writer.aggiornaStato(ObjParametri_Server, listaProdottiValidi, codIcqrf, codOper, Utility.StatoGIAS.invio_in_corso)
        '                Dim rispostaInvio = caller.ChiamataWSAsync(prodValidi, urlASync)
        '                madeChanges = True
        '                Dim converted = False
        '                Dim result As New ProdSiRPVOutput
        '                Try
        '                    XMLUtility.getObjectFromResponse(rispostaInvio, result, Utility.getSoapenv)
        '                    converted = True
        '                Catch ex As Exception
        '                    writer.aggiornaStato(ObjParametri_Server, listaProdottiValidi, codIcqrf, codOper, Utility.StatoGIAS.invio_non_riuscito)
        '                    writer.inserisciLogInvio(ObjParametri_Server, listaProdottiValidi, codIcqrf, prodValidi, "", Utility.ProdSiRPV, True, "", codOper)
        '                    Throw New TeleregistriExceptionResponseParsing("Prodotti", "inserisciAggiornaTuttoInput")
        '                End Try
        '                If converted Then
        '                    If result.IdTrasmissione Is Nothing Then
        '                        writer.aggiornaStato(ObjParametri_Server, listaProdottiValidi, codIcqrf, codOper, Utility.StatoGIAS.invio_non_riuscito)
        '                        writer.inserisciLogInvio(ObjParametri_Server, listaProdottiValidi, codIcqrf, prodValidi, "", Utility.ProdSiRPV, True, "", codOper)
        '                    Else
        '                        writer.aggiornaStato(ObjParametri_Server, listaProdottiValidi, codIcqrf, codOper, Utility.StatoGIAS.invio_effettuato_correttamente)
        '                        writer.inserisciLogInvio(ObjParametri_Server, listaProdottiValidi, codIcqrf, prodValidi, "", Utility.ProdSiRPV, False, result.IdTrasmissione, codOper)
        '                    End If
        '                End If
        '            End If
        '        End If
        '    Next
        'Next
        Return madeChanges
    End Function

    Public Overrides Function inserisciAggiornaTuttoOutput() As Boolean
        Dim madeChanges = False
        'Dim op = reader.getOperazioniDaControllare(ObjParametri_Server, Utility.ProdSiRPV)
        'For Each operation As DataRow In op.Rows
        '    Dim prod As New xProdottiSiRPV(ObjParametri_Server, ObjParametri_Utenti)
        '    Dim processed = False
        '    Dim checkResult As String
        '    Dim checkRes As New GetProdSiRPVOutput
        '    Dim received = False
        '    Dim listaProdottiDaAggiornare As List(Of String) = reader.listaProdottiDaAggiornare(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")))
        '    Dim codIcqrf As String = reader.getCodIcqrfFromLogInvioID(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")))
        '    Dim codOper As String = reader.getCodOperFromCodIcqrf(ObjParametri_Server, codIcqrf)
        '    checkResult = caller.ChiamataWSSync(prod.sGetProdottiSiRPV(username, password, operation.Item("idTrasmissione_SIAN")), urlSync)
        '    madeChanges = True
        '    Try
        '        XMLUtility.getObjectFromResponse(checkResult, checkRes, Utility.getSoapenv)
        '    Catch ex As Exception
        '        utils.AggiornaControllataLogInvio(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), True)
        '        utils.inserisciErrore(ObjParametri_Server, operation.Item("ws_RegVino_LogInvio_Cod"), "EXP", "XML Parse Element Response Exception on Operation:" + checkRes.GetType.Name)
        '        writer.aggiornaStato(ObjParametri_Server, listaProdottiDaAggiornare, codIcqrf, codOper, Utility.StatoGIAS.in_fase_di_verifica)
        '        Throw New TeleregistriExceptionResponseParsing("Prodotti", "inserisciAggiornaTuttoOutput")
        '    End Try
        '    Try
        '        If Not checkRes.Esito Is Nothing Then

        '            If checkRes.Esito.codice <> "000" Then
        '                'Ricevuto dati validi
        '                Dim typecode = utils.getTypeCode(ObjParametri_Server, checkRes.Esito.codice)
        '                Select Case typecode
        '                    Case Utility.TipoRitornoErrore
        '                        utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), checkRes.Esito.codice, checkRes.Esito.messaggio)
        '                        writer.aggiornaStato(ObjParametri_Server, listaProdottiDaAggiornare, codIcqrf, codOper, Utility.StatoGIAS.errori_rilevati_dal_Sian)
        '                    Case Utility.TipoRitornoWarning
        '                        utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), checkRes.Esito.codice, checkRes.Esito.messaggio)
        '                        writer.aggiornaStato(ObjParametri_Server, listaProdottiDaAggiornare, codIcqrf, codOper, Utility.StatoGIAS.errori_rilevati_dal_Sian)
        '                    Case Utility.TipoRitornoInfo
        '                    Case Else
        '                        utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), checkRes.Esito.codice, checkRes.Esito.messaggio)
        '                        writer.aggiornaStato(ObjParametri_Server, listaProdottiDaAggiornare, codIcqrf, codOper, Utility.StatoGIAS.errori_rilevati_dal_Sian)
        '                End Select
        '                utils.AggiornaControllataLogInvio(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), True)
        '                received = True
        '            Else
        '                'Richiesta non ancora valutata
        '                writer.aggiornaStato(ObjParametri_Server, listaProdottiDaAggiornare, codIcqrf, codOper, Utility.StatoGIAS.in_fase_di_verifica)
        '            End If
        '        Else
        '            utils.AggiornaControllataLogInvio(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), True)
        '            received = True
        '        End If
        '        If received Then
        '            For Each prodRes As ProdottoEsito In checkRes.ProdottoEsito
        '                Dim tipoRitorno As String = utils.getTypeCode(ObjParametri_Server, prodRes.Esito.codice)
        '                Select Case tipoRitorno
        '                    Case Utility.TipoRitornoErrore
        '                        utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), prodRes.Esito.codice, prodRes.Esito.messaggio)
        '                        writer.aggiornaStato(ObjParametri_Server, prodRes.CodiceProdotto.CodPrimario + "|" + prodRes.CodiceProdotto.CodSecondario, codIcqrf, codOper, Utility.StatoGIAS.errori_rilevati_dal_Sian)
        '                    Case Utility.TipoRitornoWarning
        '                        utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), prodRes.Esito.codice, prodRes.Esito.messaggio)
        '                        writer.aggiornaStato(ObjParametri_Server, prodRes.CodiceProdotto.CodPrimario + "|" + prodRes.CodiceProdotto.CodSecondario, codIcqrf, codOper, Utility.StatoGIAS.errori_rilevati_dal_Sian)
        '                    Case Utility.TipoRitornoInfo
        '                        writer.aggiornaStato(ObjParametri_Server, prodRes.CodiceProdotto.CodPrimario + "|" + prodRes.CodiceProdotto.CodSecondario, codIcqrf, codOper, Utility.StatoGIAS.valida_nel_sian)
        '                    Case Else
        '                        utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), prodRes.Esito.codice, prodRes.Esito.messaggio)
        '                        writer.aggiornaStato(ObjParametri_Server, prodRes.CodiceProdotto.CodPrimario + "|" + prodRes.CodiceProdotto.CodSecondario, codIcqrf, codOper, Utility.StatoGIAS.errori_rilevati_dal_Sian)
        '                End Select
        '            Next
        '        End If
        '    Catch ex As Exception
        '        utils.AggiornaControllataLogInvio(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), False)
        '    End Try

        'Next
        Return madeChanges
    End Function

    Private Function listaProdottiPerRichiesta(tRichiesta As TipoRichiesta, codOper As String, codIcqrf As String, statoGIAS As Utility.StatoGIAS) As List(Of String)
        Dim dt As DataTable
        Dim list As New List(Of String)
        Select Case tRichiesta
            Case TipoRichiesta.A
                dt = reader.LeggiProdottiPerRichiesta(ObjParametri_Server, statoGIAS, "A", codOper, codIcqrf)
            Case TipoRichiesta.I
                dt = reader.LeggiProdottiPerRichiesta(ObjParametri_Server, statoGIAS, "I", codOper, codIcqrf)
            Case 2
                dt = reader.LeggiProdottiPerRichiesta(ObjParametri_Server, statoGIAS, "E", codOper, codIcqrf)
        End Select
        For Each row As DataRow In dt.Rows
            list.Add(CStr(row.Item("Mat_Cod")) + "|" + CStr(row.Item("Lotto")))
        Next
        Return list
    End Function

    Private Sub GestioneModificati()
        Dim DT = reader.prodottiRead(ObjParametri_Server, "", "", "", 0, "", 1, Nothing)
        For Each row As DataRow In DT.Rows
            Dim stato As Integer = row.Item("GIAS_Stato")
            Dim tipoRichiesta As String = row.Item("TipoRichiesta")
            Dim canInsertUpdate = False
            Dim nuovaRichiesta As String = ""
            Select Case tipoRichiesta
                Case "I"
                    If stato = 18000002 Or stato = 18000007 Or stato = 18000008 Then
                        canInsertUpdate = True
                        nuovaRichiesta = "I"
                    ElseIf stato = 18000009 Then
                        canInsertUpdate = True
                        nuovaRichiesta = "A"
                    Else
                        canInsertUpdate = False
                    End If
                Case "A"
                    If stato = 18000002 Or stato = 18000007 Or stato = 18000008 Then
                        canInsertUpdate = True
                        nuovaRichiesta = "A"
                    ElseIf stato = 18000009 Then
                        canInsertUpdate = True
                        nuovaRichiesta = "A"
                    Else
                        canInsertUpdate = False
                    End If
                Case "E"
                    If stato = 18000002 Or stato = 18000007 Or stato = 18000008 Then
                        canInsertUpdate = True
                        nuovaRichiesta = "A"
                    ElseIf stato = 18000009 Then
                        canInsertUpdate = True
                        nuovaRichiesta = "I"
                    Else
                        canInsertUpdate = False
                    End If
            End Select

            If canInsertUpdate Then
                writer.aggiornaModificato(ObjParametri_Server, CStr(row.Item("Mat_Cod")) + "|" + CStr(row.Item("Lotto")), row.Item("CodIcqrf"), row.Item("CodOper"), 0, nuovaRichiesta)
                writer.aggiornaStato(ObjParametri_Server, CStr(row.Item("Mat_Cod")) + "|" + CStr(row.Item("Lotto")), row.Item("CodIcqrf"), row.Item("CodOper"), Utility.StatoGIAS.creata)
            End If
        Next
    End Sub

    Public Sub AggiornaCatalogoProdotto()
        Dim visGiac As New VisGiacSiRPV
        Dim listaCodOper = reader.getListaCodOper(ObjParametri_Server)
        Dim prodList As New List(Of String)
        Dim listaNonMappati As New List(Of String)
        Dim visProd As New VisProdottiSiRPV
        For Each codOper In listaCodOper
            Dim personaFisica As Boolean = reader.personaFisica(ObjParametri_Server, codOper)
            Dim listaCodIcqrf As List(Of String) = reader.getListaCodIcqrf(ObjParametri_Server, codOper)
            For Each codIcqrf In listaCodIcqrf
                'Dim fileName As String = "Giacenze " & codOper & "-" & codIcqrf & "-" & Date.Now.AddDays(-1).Year & "-" & Date.Now.AddDays(-1).Month & "-" & Date.Now.AddDays(-1).Day & ".xml"
                'Dim path = DirectoryFileEsportazioni + "\" + fileName
                'If Not File.Exists(path) Then
                '    Dim giacCall = visGiac.sVisGiacSiRPVInput(username, password, codIcqrf, codOper, personaFisica, Date.Now.AddDays(-1))
                '    Dim rispostaInvio = caller.ChiamataWSSync(giacCall, urlSync, CertificateFile)
                '    Dim result As New VisGiacSiRPVOutput
                '    XMLUtility.getObjectFromResponse(rispostaInvio, result, Utility.getSoapenv)
                '    File.WriteAllText(path, rispostaInvio)
                '    If result.ProdGiacenza IsNot Nothing Then
                '        For Each prodotto In result.ProdGiacenza
                '            If prodotto.QtaGiacenza > 0 Then
                '                prodList.Add(codIcqrf + "-" + prodotto.CodiceProdotto.CodPrimario + "-" + prodotto.CodiceProdotto.CodSecondario)
                '            End If
                '        Next
                '    End If
                'End If

                If codIcqrf <> "" Then
                    'CHIAMATA RICERCA PRODOTTI
                    Dim fileNameProd As String = "Prodotti " & codOper & "-" & codIcqrf & ".xml"
                    Dim pathProd = DirectoryFileEsportazioni + "\" + fileNameProd
                    If reader.prodottiRead(ObjParametri_Server, codOper, codIcqrf, "", 0, "", Nothing, Utility.StatoGIAS.Aggiornamento_Prodotto).Rows.Count > 0 Then
                        Dim prodCall = visProd.sVisProdottiSiRPVInput(username, password, codIcqrf, codOper, personaFisica)
                        Dim rispostaInvio = caller.ChiamataWSSync(prodCall, urlSync, CertificateFile)
                        File.WriteAllText(pathProd, rispostaInvio)
                        Dim result As New VisProdSiRPVOutput
                        XMLUtility.getObjectFromResponse(rispostaInvio, result, Utility.getSoapenv)
                        Dim i = 0
                        If result.ProdCatalogo IsNot Nothing Then
                            For Each prodotto In result.ProdCatalogo
                                'If prodList.Contains(codIcqrf + "-" + prodotto.CodiceProdotto.CodPrimario + "-" + prodotto.CodiceProdotto.CodSecondario) Then
                                Try
                                    Dim mappato = False
                                    aggiornaProdotto(codOper, codIcqrf, prodotto, mappato)
                                    If Not mappato Then
                                        listaNonMappati.Add(codIcqrf + "-" + prodotto.CodiceProdotto.CodPrimario + "-" + prodotto.CodiceProdotto.CodSecondario)
                                    End If
                                Catch ex As Exception
                                    objLog.Scrivi_LOG(ObjParametri_Server,
                                                      "AgronicaCoreRegVinoBIZ.Prodotti.AggiornaCatalogoProdotto",
                                                      $"Errore su Prodotto:{prodotto.CodiceProdotto.CodPrimario}-{prodotto.CodiceProdotto.CodSecondario} {ex.Message}",
                                                      CustomLOGParams:=customLOGParams)
                                End Try

                                'End If
                                i += 1
                            Next
                        End If
                    End If

                    For Each prod In reader.prodottiRead(ObjParametri_Server, codOper, codIcqrf, "", 0, "", Nothing, Utility.StatoGIAS.Aggiornamento_Prodotto).Rows
                        writer.aggiornaStato(ObjParametri_Server, prod.item("codIcqrf"), prod.item("codOper"), prod.item("Mat_Cod"), prod.item("Lotto"), 18000000)
                    Next
                End If



            Next
        Next

        If listaNonMappati.Count > 0 Then
            objLog.Scrivi_LOG(ObjParametri_Server,
                  "AgronicaCoreRegVinoBIZ.Prodotti.AggiornaCatalogoProdotto",
                  "PRODOTTI NON MAPPATI: ",
                  CustomLOGParams:=customLOGParams)
        End If

        For Each nonMapped In listaNonMappati
            objLog.Scrivi_LOG(ObjParametri_Server,
                  "AgronicaCoreRegVinoBIZ.Prodotti.AggiornaCatalogoProdotto",
                  nonMapped,
                  CustomLOGParams:=customLOGParams)
        Next

    End Sub

    Private Sub aggiornaProdotto(codOper As String, codIcqrf As String, prodotto As VisProdSiRPVOutputProdCatalogo, ByRef mappato As Boolean)
        Dim codPrimario As String = prodotto.CodiceProdotto.CodPrimario
        Dim codSecondario As String = prodotto.CodiceProdotto.CodSecondario

        Dim AltreVarieta As String = ""
        Dim Annata As String = ""
        Dim PercAnnata As String = ""
        Dim AttoCert As String = ""
        Dim Biologico As String = ""
        Dim CodCategoria As String = ""
        Dim CodClassificazione As String = ""
        Dim CodColore As String = ""
        Dim CodDopIgp As String = ""
        Dim CodEbacchus As String = ""
        Dim CodPartita As String = ""
        Dim CodSottozona As String = ""
        Dim CodStatoFisico As String = ""
        Dim CodVigna As String = ""
        Dim CodZonaViticola As String = ""
        Dim DataCertDOP As Date = AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO
        Dim MassaVolumica As Decimal
        Dim Menzioni As New List(Of String)
        Dim NumCertDOP As String = ""
        Dim OrigineUve As String = ""
        Dim PaesiProvenienza As New List(Of String)
        Dim Provenienza As String = ""
        Dim varieta As New List(Of String)
        Dim PraticheEnologiche As New List(Of String)
        Dim PercVarieta As New List(Of String)

        Dim AltreVarietaSpecified As Boolean = False
        Dim AnnataSpecified As Boolean = False
        Dim PercAnnataSpecified As Boolean = False
        Dim AttoCertSpecified As Boolean = False
        Dim BiologicoSpecified As Boolean = False
        Dim CodCategoriaSpecified As Boolean = False
        Dim CodClassificazioneSpecified As Boolean = False
        Dim CodColoreSpecified As Boolean = False
        Dim CodDopIgpSpecified As Boolean = False
        Dim CodEbacchusSpecified As Boolean = False
        Dim CodPartitaSpecified As Boolean = False
        Dim CodSottozonaSpecified As Boolean = False
        Dim CodStatoFisicoSpecified As Boolean = False
        Dim CodVignaSpecified As Boolean = False
        Dim CodZonaViticolaSpecified As Boolean = False
        Dim DataCertDOPSpecified As Boolean = False
        Dim MassaVolumicaSpecified As Boolean = False
        Dim MenzioniSpecified As Boolean = False
        Dim NumCertDOPSpecified As Boolean = False
        Dim OrigineUveSpecified As Boolean = False
        Dim PaesiProvenienzaSpecified As Boolean = False
        Dim ProvenienzaSpecified As Boolean = False
        Dim varietaSpecified As Boolean = False
        Dim PraticheEnologicheSpecified As Boolean = False
        Dim PercVarietaSpecified As Boolean = False

        If prodotto.Designazione.AltreVarieta IsNot Nothing AndAlso prodotto.Designazione.AltreVarieta <> "" Then
            AltreVarietaSpecified = True
            AltreVarieta = prodotto.Designazione.AltreVarieta
        End If

        If prodotto.Designazione.Annata IsNot Nothing AndAlso prodotto.Designazione.Annata.Codice <> 0 Then
            Annata = prodotto.Designazione.Annata.Codice
            AnnataSpecified = True
            If prodotto.Designazione.Annata.PercentualeSpecified Then
                PercAnnata = CStr(prodotto.Designazione.Annata.Percentuale)
                PercAnnataSpecified = True
            End If
        End If

        If prodotto.Designazione.AttoCert IsNot Nothing AndAlso prodotto.Designazione.AttoCert <> "" Then
            AttoCert = prodotto.Designazione.AttoCert
            AttoCertSpecified = True
        End If

        If prodotto.Designazione.Biologico IsNot Nothing AndAlso prodotto.Designazione.Biologico <> "" Then
            Biologico = prodotto.Designazione.Biologico
            BiologicoSpecified = True
        End If

        If prodotto.Designazione.CodCategoria IsNot Nothing AndAlso prodotto.Designazione.CodCategoria <> "" Then
            CodCategoria = prodotto.Designazione.CodCategoria
            CodCategoriaSpecified = True
        End If

        If prodotto.Designazione.CodClassificazione IsNot Nothing AndAlso prodotto.Designazione.CodClassificazione <> "" Then
            CodClassificazione = prodotto.Designazione.CodClassificazione
            CodClassificazioneSpecified = True
        End If

        If prodotto.Designazione.CodColore IsNot Nothing AndAlso prodotto.Designazione.CodColore <> "" Then
            CodColore = prodotto.Designazione.CodColore
            CodColoreSpecified = True
        End If

        If prodotto.Designazione.CodDopIgp IsNot Nothing AndAlso prodotto.Designazione.CodDopIgp <> "" Then
            CodDopIgp = prodotto.Designazione.CodDopIgp
            CodDopIgpSpecified = True
        End If

        If prodotto.Designazione.CodEbacchus IsNot Nothing AndAlso prodotto.Designazione.CodEbacchus <> "" Then
            CodEbacchus = prodotto.Designazione.CodEbacchus
            CodEbacchusSpecified = True
        End If

        If prodotto.Designazione.CodPartita IsNot Nothing AndAlso prodotto.Designazione.CodPartita <> "" Then
            CodPartita = prodotto.Designazione.CodPartita
            CodPartitaSpecified = True
        End If

        If prodotto.Designazione.CodSottozona IsNot Nothing AndAlso prodotto.Designazione.CodSottozona <> "" Then
            CodSottozona = prodotto.Designazione.CodSottozona
            CodSottozonaSpecified = True
        End If

        If prodotto.Designazione.CodStatoFisico IsNot Nothing AndAlso prodotto.Designazione.CodStatoFisico <> "" Then
            CodStatoFisico = prodotto.Designazione.CodStatoFisico
            CodStatoFisicoSpecified = True
        End If

        If prodotto.Designazione.CodVigna IsNot Nothing AndAlso prodotto.Designazione.CodVigna <> "" Then
            CodVigna = prodotto.Designazione.CodVigna
            CodVignaSpecified = True
        End If

        If prodotto.Designazione.CodZonaViticola IsNot Nothing AndAlso prodotto.Designazione.CodZonaViticola <> "" Then
            CodZonaViticola = prodotto.Designazione.CodZonaViticola
            CodZonaViticolaSpecified = True
        End If

        If prodotto.Designazione.DataCertDOPSpecified = True Then
            DataCertDOP = prodotto.Designazione.DataCertDOP
            DataCertDOPSpecified = True
        End If

        If prodotto.Designazione.MassaVolumicaSpecified = True Then
            MassaVolumica = prodotto.Designazione.MassaVolumica
            MassaVolumicaSpecified = True
        End If

        If prodotto.Designazione.Menzioni IsNot Nothing AndAlso prodotto.Designazione.Menzioni.Length > 0 Then
            For Each menzione In prodotto.Designazione.Menzioni
                Menzioni.Add(menzione.Codice)
            Next
            MenzioniSpecified = True
        End If

        If prodotto.Designazione.NumCertDOP IsNot Nothing AndAlso prodotto.Designazione.NumCertDOP <> "" Then
            NumCertDOP = prodotto.Designazione.NumCertDOP
            NumCertDOPSpecified = True
        End If

        If prodotto.Designazione.OrigineUve IsNot Nothing AndAlso prodotto.Designazione.OrigineUve <> "" Then
            OrigineUve = prodotto.Designazione.OrigineUve
            OrigineUveSpecified = True
        End If

        If prodotto.Designazione.PaesiProvenienza IsNot Nothing AndAlso prodotto.Designazione.PaesiProvenienza.Length > 0 Then
            For Each Paese In prodotto.Designazione.PaesiProvenienza
                PaesiProvenienza.Add(Paese.Codice)
            Next
            PaesiProvenienzaSpecified = True
        End If

        If prodotto.Designazione.PraticheEnologiche IsNot Nothing Then
            For Each pratica In prodotto.Designazione.PraticheEnologiche
                PraticheEnologiche.Add(pratica.Codice)
            Next
            PraticheEnologicheSpecified = True
        End If

        If prodotto.Designazione.Provenienza IsNot Nothing AndAlso prodotto.Designazione.Provenienza <> "" Then
            Provenienza = prodotto.Designazione.Provenienza
            ProvenienzaSpecified = True
        End If

        If prodotto.Designazione.Varieta IsNot Nothing AndAlso prodotto.Designazione.Varieta.Length > 0 Then
            For Each var In prodotto.Designazione.Varieta
                varieta.Add(var.Codice)
                If var.PercentualeSpecified Then
                    PercVarieta.Add(var.Percentuale)
                    PercVarietaSpecified = True
                End If
            Next
            varietaSpecified = True
        End If

        Dim dtProdottiDB = reader.getProdotto(ObjParametri_Server, codOper, codIcqrf, "", "")
        For Each prod In dtProdottiDB.Rows

            Dim codPrimarioProd = prod.item("CodPrimario")
            Dim codSecondarioProd = prod.item("CodSecondario")

            'If IsDBNull(codPrimarioProd) Or IsDBNull(codSecondarioProd) Then
            Dim AltreVarietaProd = prod.item("AltreVarieta")
            Dim CodCategoriaProd = prod.item("CodCategoria")
            Dim attoCertProd = prod.item("attoCert")
            Dim CodClassificazioneProd = prod.item("CodClassificazione")
            Dim CodDopIgpProd = prod.item("CodDopIgp")
            Dim CodEbacchusProd = prod.item("CodEbacchus")
            Dim OrigineUveProd = prod.item("OrigineUve")
            Dim ProvenienzaProd = prod.item("Provenienza")
            Dim CodZonaViticolaProd = prod.item("CodZonaViticola")
            Dim CodSottozonaProd = prod.item("CodSottozona")
            Dim CodVignaProd = prod.item("CodVigna")
            Dim CodColoreProd = prod.item("CodColore")
            Dim BiologicoProd = prod.item("Biologico")
            Dim CodPartitaProd = prod.item("CodPartita")
            Dim AnnataProd = prod.item("Annata")
            Dim CodStatoFisicoProd = prod.item("CodStatoFisico")
            Dim NumCertDOPProd = prod.item("NumCertDOP")
            Dim PercAnnataProd = prod.item("PercAnnata")
            Dim DataCertDOPProd = prod.item("DataCertDOP")
            Dim MassaVolumicaProd = prod.item("MassaVolumica")
            Dim varietaProd = prod.item("varieta")
            Dim PraticheEnologicheProd = prod.item("PraticheEnologiche")
            Dim PaesiProvenienzaProd = prod.item("PaesiProvenienza")
            Dim MenzioniProd = prod.item("Menzioni")
            Dim Mat_CodProd = prod.item("Mat_Cod")
            Dim LottoProd = prod.item("Lotto")

            Dim elui As Boolean = True
            If (prod.item("GIAS_Stato") = Utility.StatoGIAS.Aggiornamento_Prodotto) Then

                controlloDatoString(elui, CodCategoria, CodCategoriaSpecified, CodCategoriaProd)
                controlloDatoString(elui, CodDopIgp, CodDopIgpSpecified, CodDopIgpProd, True)
                controlloDatoString(elui, CodClassificazione, CodClassificazioneSpecified, CodClassificazioneProd)
                controlloDatoString(elui, Annata, AnnataSpecified, AnnataProd)

                controlloDatoString(elui, AttoCert, AttoCertSpecified, attoCertProd)
                controlloDatoString(elui, Provenienza, ProvenienzaSpecified, ProvenienzaProd)
                controlloDatoString(elui, CodZonaViticola, CodZonaViticolaSpecified, CodZonaViticolaProd)
                controlloDatoPreciso(elui, CodSottozona, CodSottozonaSpecified, CodSottozonaProd, True)
                controlloDatoString(elui, CodColore, CodColoreSpecified, CodColoreProd)
                controlloDatoString(elui, CodStatoFisico, CodStatoFisicoSpecified, CodStatoFisicoProd)

                controlloDatoString(elui, AltreVarieta, AltreVarietaSpecified, AltreVarietaProd)
                controlloDatoString(elui, CodEbacchus, CodEbacchusSpecified, CodEbacchusProd)
                controlloDatoPreciso(elui, OrigineUve, OrigineUveSpecified, OrigineUveProd, True)
                controlloDatoString(elui, CodVigna, CodVignaSpecified, CodVignaProd)
                controlloDatoString(elui, Biologico, BiologicoSpecified, BiologicoProd, True)
                controlloDatoPreciso(elui, CodPartita, CodPartitaSpecified, CodPartitaProd, True)
                controlloDatoString(elui, NumCertDOP, NumCertDOPSpecified, NumCertDOPProd)

                controlloDatoPreciso(elui, PercAnnata, PercAnnataSpecified, PercAnnataProd, True)

                controlloDatoDate(elui, DataCertDOP, DataCertDOPSpecified, DataCertDOPProd)

                controlloDatoDecimal(elui, MassaVolumica, MassaVolumicaSpecified, MassaVolumicaProd)

                controlloDatoList(elui, varieta, varietaSpecified, varietaProd)
                controlloDatoPraticheEnologiche(elui, PraticheEnologiche, PraticheEnologicheSpecified, PraticheEnologicheProd)
                controlloDatoList(elui, PaesiProvenienza, PaesiProvenienzaSpecified, PaesiProvenienzaProd)
                controlloDatoMenzioni(elui, Menzioni, MenzioniSpecified, MenzioniProd)

                'Se passa tutti i controlli allora aggiorno il prodotto
                If elui Then

                    If IsDBNull(codPrimarioProd) And IsDBNull(codSecondarioProd) Then

                        writer.AggiornaCodiciSIAN(ObjParametri_Server,
                                                  prod.item("CodOper"),
                                                  prod.item("CodIcqrf"),
                                                  prod.item("Mat_Cod"),
                                                  prod.item("Lotto"),
                                                  prodotto.CodiceProdotto.CodPrimario,
                                                  prodotto.CodiceProdotto.CodSecondario)
                        writer.aggiornaModificato(ObjParametri_Server, CStr(prod.item("Mat_Cod")) & "|" & prod.item("Lotto"), codIcqrf, codOper, 0, "I")
                        writer.aggiornaStato(ObjParametri_Server, prod.item("codIcqrf"), prod.item("codOper"), prod.item("Mat_Cod"), prod.item("Lotto"), 18000000)
                        mappato = True
                    Else

                        If CStr(codPrimarioProd) <> codPrimario Or CStr(codSecondarioProd) <> codSecondario Then
                            Dim messaggio As New Text.StringBuilder($"Prodotto: Mat_Cod: {CStr(Mat_CodProd)} Lotto:'{LottoProd}' ")
                            messaggio.Append($"è già stato codificato con {codPrimarioProd}-{codSecondarioProd} ")
                            messaggio.Append($"e coincide anche con {codPrimario}-{codSecondario}")
                            objLog.Scrivi_LOG(ObjParametri_Server,
                                              "AgronicaCoreRegVinoBIZ.Prodotti.aggiornaProdotto",
                                              messaggio.ToString(),
                                              CustomLOGParams:=customLOGParams)
                        End If
                    End If
                End If
            End If
            'End If

        Next

        dtProdottiDB = reader.getProdotto(ObjParametri_Server, codOper, codIcqrf, "", "")
        For Each prod In dtProdottiDB.Rows
            If prod.item("Modificato") = 1 Then
                writer.aggiornaModificato(ObjParametri_Server, CStr(prod.item("Mat_Cod")) & "|" & prod.item("Lotto"), prod.item("codIcqrf"), prod.item("codOper"), 0, "I")
                'writer.aggiornaStato(ObjParametri_Server, prod.item("codIcqrf"), prod.item("codOper"), prod.item("Mat_Cod"), prod.item("Lotto"), 18000000)
            End If
        Next


    End Sub

    Public Sub controlloDatoString(ByRef elui As Boolean, datoSian As String, datoSianSpecified As String, datoDB As Object, Optional ValueNull0 As Boolean = False)
        If elui Then
            If datoSianSpecified Then
                If datoSian.Trim = datoDB.Trim Then
                    elui = True
                Else
                    elui = False
                End If
            Else
                If ValueNull0 = True Then
                    If IsDBNull(datoDB) Then
                        elui = True
                    ElseIf datoDB = "" Then
                        elui = True
                    ElseIf datoDB = "0" Then
                        elui = True
                    Else
                        elui = False
                    End If
                Else
                    'If IsDBNull(datoDB) Then
                    '    elui = True
                    'ElseIf datoDB = "" Then
                    '    elui = True
                    'Else
                    '    elui = False
                    'End If
                    elui = True
                End If


            End If
        End If
    End Sub

    Public Sub controlloDatoPreciso(ByRef elui As Boolean, datoSian As String, datoSianSpecified As String, datoDB As Object, Optional ValueNull0 As Boolean = False)
        If elui Then
            If datoSianSpecified Then
                If datoSian.Trim = datoDB.Trim Then
                    elui = True
                Else
                    elui = False
                End If
            Else
                If ValueNull0 = True Then
                    If IsDBNull(datoDB) Then
                        elui = True
                    ElseIf datoDB = "" Then
                        elui = True
                    ElseIf datoDB = "0" Then
                        elui = True
                    Else
                        elui = False
                    End If
                Else
                    If IsDBNull(datoDB) Then
                        elui = True
                    ElseIf datoDB = "" Then
                        elui = True
                    Else
                        elui = False
                    End If
                    'elui = True
                End If


            End If
        End If
    End Sub

    Public Sub controlloDatoBiologico(ByRef elui As Boolean, datoSian As String, datoSianSpecified As String, datoDB As Object, Optional ValueNull0 As Boolean = False)
        If elui Then
            If datoSianSpecified Then
                If datoSian.Trim = datoDB.Trim Then
                    elui = True
                Else
                    elui = False
                End If
            Else
                If ValueNull0 = True Then
                    If IsDBNull(datoDB) Then
                        elui = True
                    ElseIf datoDB = "" Then
                        elui = True
                    ElseIf datoDB = "0" Then
                        elui = True
                    Else
                        elui = False
                    End If
                Else
                    If IsDBNull(datoDB) Then
                        elui = True
                    ElseIf datoDB = "" Then
                        elui = True
                    Else
                        elui = False
                    End If
                End If


            End If
        End If
    End Sub

    Public Sub controlloDatoDecimal(ByRef elui As Boolean, datoSian As Decimal, datoSianSpecified As String, datoDB As Object)
        If elui Then
            If datoSianSpecified Then
                If IsDBNull(datoDB) Then
                    elui = False
                Else
                    If datoSian = datoDB Then
                        elui = True
                    Else
                        elui = False
                    End If
                End If
            Else
                If IsDBNull(datoDB) Then
                    elui = True
                Else
                    If datoDB = 0 Then
                        elui = True
                    Else
                        elui = False
                    End If
                End If
            End If
        End If
    End Sub

    Public Sub controlloDatoDate(ByRef elui As Boolean, datoSian As Date, datoSianSpecified As String, datoDB As Date)
        If elui Then
            If datoSianSpecified Then
                If datoSian.ToString("dd/MM/yyyy") = datoDB.ToString("dd/MM/yyyy") Then
                    elui = True
                Else
                    elui = False
                End If
            Else
                If IsDBNull(datoDB) Or
                    datoDB = AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO Or
                    datoDB = AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAFINE Then
                    elui = True
                Else
                    elui = False
                End If
            End If
        End If
    End Sub

    Public Sub controlloDatoList(ByRef elui As Boolean, datoSian As List(Of String), datoSianSpecified As String, datoDB As Object)
        If elui Then
            If IsDBNull(datoDB) Then
                If datoSian.Count = 0 Then
                    elui = True
                Else
                    elui = False
                End If
            Else
                Dim listDB As New List(Of String)
                For Each singleDBDate In datoDB.ToString.Split("|")
                    If singleDBDate.Trim <> "" Then
                        listDB.Add(singleDBDate.Trim())
                    End If
                Next
                If listDB.Count = datoSian.Count Then
                    For Each sian In datoSian
                        If listDB.Contains(sian) Then
                            listDB.Remove(sian)
                        ElseIf listDB.Contains(CStr(CInt(sian))) Then
                            listDB.Remove(CStr(CInt(sian)))
                        End If

                    Next
                    If listDB.Count = 0 Then
                        elui = True
                    Else
                        elui = False
                    End If
                Else
                    elui = False
                End If
            End If
        End If
    End Sub

    Public Sub controlloDatoPraticheEnologiche(ByRef elui As Boolean, datoSian As List(Of String), datoSianSpecified As String, datoDB As Object)
        If elui Then
            If IsDBNull(datoDB) Then
                If datoSian.Count = 0 Then
                    elui = True
                Else
                    elui = False
                End If
            Else
                Dim listDB As New List(Of String)
                For Each singleDBDate In datoDB.ToString.Split("|")
                    If singleDBDate.Trim <> "" Then
                        listDB.Add(singleDBDate.Trim())
                    End If
                Next
                If listDB.Count = datoSian.Count Then
                    For Each sian In datoSian
                        If listDB.Contains(sian) Then
                            listDB.Remove(sian)
                        End If
                    Next
                    If listDB.Count = 0 Then
                        elui = True
                    Else
                        elui = False
                    End If
                Else
                    If datoSianSpecified = False And datoDB = "00" Then
                        elui = True
                    Else
                        elui = False
                    End If
                End If
            End If
        End If
    End Sub

    Public Sub controlloDatoMenzioni(ByRef elui As Boolean, datoSian As List(Of String), datoSianSpecified As String, datoDB As Object)
        If elui Then
            If IsDBNull(datoDB) Then
                If datoSian.Count = 0 Then
                    elui = True
                Else
                    elui = False
                End If
            Else
                Dim listDB As New List(Of String)
                For Each singleDBDate In datoDB.ToString.Split("|")
                    If singleDBDate.Trim <> "" Then
                        listDB.Add(singleDBDate.Trim())
                    End If
                Next
                If listDB.Count = datoSian.Count Then
                    For Each sian In datoSian
                        If listDB.Contains(sian) Then
                            listDB.Remove(sian)
                        End If
                    Next
                    If listDB.Count = 0 Then
                        elui = True
                    Else
                        elui = False
                    End If
                Else
                    If datoSianSpecified = False And datoDB = "0" Then
                        elui = True
                    Else
                        elui = False
                    End If
                End If
            End If
        End If
    End Sub

End Class
