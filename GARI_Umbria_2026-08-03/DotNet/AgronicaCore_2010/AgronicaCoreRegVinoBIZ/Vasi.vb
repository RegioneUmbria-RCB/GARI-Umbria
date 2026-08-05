Imports AgronicaCoreRegVinoDAL
Imports AgronicaCoreUtility

Public Class Vasi
    Inherits TeleregistriManager

    Dim reader As xDBVasiSiRPV_R
    Dim writer As xDBVasiSiRPV_W

    Public Sub New(user As String, pwd As String, urlS As String, urlA As String, server As AgronicaCoreDataProvider.AgronicaCoreParametri, utenti As AgronicaCoreDataProvider.AgronicaCoreParametri, _Configurazione_Servizio As AgronicaCoreVarieDAL.Configurazione_Servizio, CertificateFile As String)
        MyBase.New(user, pwd, urlS, urlA, server, utenti, _Configurazione_Servizio, CertificateFile)
    End Sub

    Public Overrides Sub inizializzaReaderWriter()
        reader = New xDBVasiSiRPV_R
        writer = New xDBVasiSiRPV_W
    End Sub

    Public Overrides Sub checkResult()
        'Dim listaCodIcqrf As List(Of String) = reader.getListaCodIcqrf(ObjParametri_Server)
        'Dim corretto As Boolean = True
        'For Each codIcqrf As String In listaCodIcqrf
        '    Dim codOper As String = reader.getCodOperFromCodIcqrf(ObjParametri_Server, codIcqrf)
        '    Dim personaFisica As Boolean = reader.personaFisica(ObjParametri_Server, codOper)
        '    Dim listaCodVas As List(Of String) = reader.getDBVasiPerCodIcqrf(ObjParametri_Server, codIcqrf)

        '    Dim visVas As New VisVasiSiRPV()
        '    Dim vis = visVas.sVisVasiSiRPVInput(username, password, codIcqrf, codOper, personaFisica)
        '    Dim rispostaInvio = caller.ChiamataWSSync(vis, urlASync)

        '    Dim converted = False
        '    Dim result As New VisVasiSiRPVOutput
        '    Try
        '        XMLUtility.getObjectFromResponse(rispostaInvio, result, Utility.getSoapenv)
        '        converted = True
        '    Catch ex As Exception
        '        Throw New TeleregistriExceptionResponseParsing("Vasi", "checkResult")
        '    End Try
        '    If converted Then
        '        If Not result.DettaglioVasi Is Nothing Then
        '            Dim listaVasiVis As New List(Of String)
        '            For Each vas As VasoVinario In result.DettaglioVasi
        '                listaVasiVis.Add(vas.CodVaso)
        '            Next

        '            For Each vasoInterno As String In listaCodVas
        '                If listaVasiVis.Contains(vasoInterno.ToUpper) Then
        '                    listaVasiVis.Remove(vasoInterno.ToUpper)
        '                Else
        '                    Throw New TeleregistriExceptionIncoherentDBSincro("Vasi", "checkResult")
        '                End If
        '            Next
        '            If listaVasiVis.Count <> 0 Then
        '                Throw New TeleregistriExceptionIncoherentDBSincro("Vasi", "checkResult")
        '            End If
        '        End If
        '    End If
        'Next
        Dim listaCodOper = reader.getListaCodOper(ObjParametri_Server)
        For Each codOper In listaCodOper
            Dim listaCodIcqrf As List(Of String) = reader.getListaCodIcqrf(ObjParametri_Server, codOper)
            Dim corretto As Boolean = True
            For Each codIcqrf As String In listaCodIcqrf
                Dim dt = reader.LeggiVasiPerRichiesta(ObjParametri_Server, Utility.StatoGIAS.eliminata_nel_sian, "E", codIcqrf, codOper)
                For Each vasoEl As DataRow In dt.Rows
                    writer.eliminaVaso(ObjParametri_Server, vasoEl.Item("CodVaso"), codIcqrf, codOper)
                Next
            Next
        Next
    End Sub

    Public Overrides Function eliminaTuttoInput() As Boolean
        Dim madeChanges = False
        Dim listaCodOper = reader.getListaCodOper(ObjParametri_Server)
        For Each codOper In listaCodOper
            Dim vas As New xVasiSiRPV(ObjParametri_Server, ObjParametri_Utenti)
            Dim listaCodIcqrf As List(Of String) = reader.getListaCodIcqrf(ObjParametri_Server, codOper)
            For Each codIcqrf As String In listaCodIcqrf
                Dim personaFisica As Boolean = reader.personaFisica(ObjParametri_Server, codOper)

                Dim listaVasiDaEliminare = listaVasiPerRichiesta(2, codIcqrf, Utility.StatoGIAS.creata, codOper)
                If listaVasiDaEliminare.Count > 0 Then
                    Dim vasiDaEliminare = vas.sCancVasiSiRPV(username, password, codIcqrf, codOper, personaFisica, listaVasiDaEliminare)

                    Dim listaVasiValidi = listaVasiPerRichiesta(2, codIcqrf, Utility.StatoGIAS.valida_per_invio, codOper)
                    If listaVasiValidi.Count > 0 Then

                        Dim vasiValidi = vas.sCancVasiSiRPV(username, password, codIcqrf, codOper, personaFisica, listaVasiValidi)
                        'Chiamo il webService
                        writer.aggiornaStato(ObjParametri_Server, listaVasiValidi, codIcqrf, Utility.StatoGIAS.invio_in_corso, codOper)
                        madeChanges = True
                        Dim converted = False
                        Dim result As New CancVasiSiRPVOutput
                        Try
                            Dim rispostaInvio = caller.ChiamataWSAsync(vasiValidi, urlASync, CertificateFile)
                            XMLUtility.getObjectFromResponse(rispostaInvio, result, Utility.getSoapenv)
                            converted = True
                        Catch ex As Exception
                            writer.aggiornaStato(ObjParametri_Server, listaVasiValidi, codIcqrf, Utility.StatoGIAS.invio_non_riuscito, codOper)
                            writer.inserisciLogInvio(ObjParametri_Server, listaVasiValidi, codIcqrf, vasiValidi, "", Utility.CancVasiSiRPV, True, "", codOper)
                            Throw New TeleregistriExceptionResponseParsing("Vasi", "eliminaTuttoInput " + ex.Message)
                        End Try
                        If converted Then
                            If result.IdTrasmissione Is Nothing Then
                                writer.aggiornaStato(ObjParametri_Server, listaVasiValidi, codIcqrf, Utility.StatoGIAS.invio_non_riuscito, codOper)
                                writer.inserisciLogInvio(ObjParametri_Server, listaVasiValidi, codIcqrf, vasiValidi, "", Utility.CancVasiSiRPV, True, "", codOper)
                            Else
                                writer.aggiornaStato(ObjParametri_Server, listaVasiValidi, codIcqrf, Utility.StatoGIAS.invio_effettuato_correttamente, codOper)
                                writer.inserisciLogInvio(ObjParametri_Server, listaVasiValidi, codIcqrf, vasiValidi, "", Utility.CancVasiSiRPV, False, result.IdTrasmissione, codOper)
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
        Dim op = reader.getOperazioniDaControllare(ObjParametri_Server, Utility.CancVasiSiRPV)
        For Each operation As DataRow In op.Rows
            Dim vas As New xVasiSiRPV(ObjParametri_Server, ObjParametri_Utenti)
            Dim processed = False
            Dim checkResult As String
            Dim checkRes As New GetCancVasiSiRPVOutput
            Dim received = False
            Dim listaVasiDaAggiornare As List(Of String) = reader.listaVasiDaAggiornare(ObjParametri_Server, operation.Item("ws_RegVino_LogInvio_Cod"))
            Dim codIcqrf As String = reader.getCodIcqrfFromLogInvioID(ObjParametri_Server, operation.Item("ws_RegVino_LogInvio_Cod"))
            Dim codOper As String = reader.getCodOperFromLogInvioID(ObjParametri_Server, operation.Item("ws_RegVino_LogInvio_Cod"))
            madeChanges = True
            Try
                checkResult = caller.ChiamataWSSync(vas.sGetCancVasiSiRPV(username, password, operation.Item("idTrasmissione_SIAN")), urlSync, CertificateFile)
                XMLUtility.getObjectFromResponse(checkResult, checkRes, Utility.getSoapenv)
            Catch ex As Exception
                utils.AggiornaControllataLogInvio(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), False)
                utils.inserisciErrore(ObjParametri_Server, operation.Item("ws_RegVino_LogInvio_Cod"), "EXP", "XML Parse Element Response Exception on Operation:" + checkRes.GetType.Name, 0, "", "", "", codOper)
                writer.aggiornaStato(ObjParametri_Server, listaVasiDaAggiornare, codIcqrf, Utility.StatoGIAS.in_fase_di_verifica, codOper)
                Throw New TeleregistriExceptionResponseParsing("Vasi", "eliminaTuttoOutput " + ex.Message)
            End Try
            Try
                If Not checkRes.Esito Is Nothing Then

                    If checkRes.Esito.codice <> "000" Then
                        'Ricevuto dati validi
                        Dim typecode = utils.getTypeCode(ObjParametri_Server, checkRes.Esito.codice)
                        Select Case typecode
                            Case Utility.TipoRitornoErrore
                                utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), checkRes.Esito.codice, checkRes.Esito.messaggio, 0, "", "", "", codOper)
                                writer.aggiornaStato(ObjParametri_Server, listaVasiDaAggiornare, codIcqrf, Utility.StatoGIAS.errori_rilevati_dal_Sian, codOper)
                            Case Utility.TipoRitornoWarning
                                utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), checkRes.Esito.codice, checkRes.Esito.messaggio, 0, "", "", "", codOper)
                                writer.aggiornaStato(ObjParametri_Server, listaVasiDaAggiornare, codIcqrf, Utility.StatoGIAS.errori_rilevati_dal_Sian, codOper)
                            Case Utility.TipoRitornoInfo

                            Case Else
                                utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), checkRes.Esito.codice, checkRes.Esito.messaggio, 0, "", "", "", codOper)
                                writer.aggiornaStato(ObjParametri_Server, listaVasiDaAggiornare, codIcqrf, Utility.StatoGIAS.errori_rilevati_dal_Sian, codOper)
                        End Select
                        utils.AggiornaControllataLogInvio(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), True)
                        received = True
                    Else
                        'Richiesta non ancora valutata
                        writer.aggiornaStato(ObjParametri_Server, listaVasiDaAggiornare, codIcqrf, Utility.StatoGIAS.in_fase_di_verifica, codOper)
                    End If
                Else
                    utils.AggiornaControllataLogInvio(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), True)
                    received = True
                End If
                If received Then
                    For Each vasRes As VasoVinarioOutput In checkRes.VasiOutput
                        Dim tipoRitorno As String = utils.getTypeCode(ObjParametri_Server, vasRes.Esito.codice)
                        Select Case tipoRitorno
                            Case Utility.TipoRitornoErrore
                                utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), vasRes.Esito.codice, vasRes.Esito.messaggio, 0, "", vasRes.CodVaso, checkRes.CodiceIcqrf, codOper)
                                writer.aggiornaStato(ObjParametri_Server, vasRes.CodVaso, codIcqrf, Utility.StatoGIAS.errori_rilevati_dal_Sian, codOper)
                            Case Utility.TipoRitornoWarning
                                utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), vasRes.Esito.codice, vasRes.Esito.messaggio, 0, "", vasRes.CodVaso, checkRes.CodiceIcqrf, codOper)
                                writer.aggiornaStato(ObjParametri_Server, vasRes.CodVaso, codIcqrf, Utility.StatoGIAS.errori_rilevati_dal_Sian, codOper)
                            Case Utility.TipoRitornoInfo
                                writer.aggiornaStato(ObjParametri_Server, vasRes.CodVaso, codIcqrf, Utility.StatoGIAS.eliminata_nel_sian, codOper)
                            Case Else
                                utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), vasRes.Esito.codice, vasRes.Esito.messaggio, 0, "", vasRes.CodVaso, checkRes.CodiceIcqrf, codOper)
                                writer.aggiornaStato(ObjParametri_Server, vasRes.CodVaso, codIcqrf, Utility.StatoGIAS.errori_rilevati_dal_Sian, codOper)
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
        Dim madeChanges = False
        Dim listaCodOper = reader.getListaCodOper(ObjParametri_Server)
        For Each codOper In listaCodOper
            Dim vasi As New xVasiSiRPV(ObjParametri_Server, ObjParametri_Utenti)
            Dim listaCodIcqrf As List(Of String) = reader.getListaCodIcqrf(ObjParametri_Server, codOper)
            For Each codIcqrf As String In listaCodIcqrf
                Dim personaFisica As Boolean = reader.personaFisica(ObjParametri_Server, codOper)
                'trovo vasi da inviare
                Dim listaVasiDaInviare = listaVasiPerRichiesta(tipoRichiesta, codIcqrf, Utility.StatoGIAS.creata, codOper)
                If listaVasiDaInviare.Count > 0 Then
                    Dim vasiDaInviare = vasi.sVasiSiRPV(username, password, listaVasiDaInviare, tipoRichiesta, codIcqrf, codOper, personaFisica)

                    'Valido Vasi
                    Dim listaVasiValidi = listaVasiPerRichiesta(tipoRichiesta, codIcqrf, Utility.StatoGIAS.valida_per_invio, codOper)
                    If listaVasiValidi.Count > 0 Then
                        Dim vasiValidi = vasi.sVasiSiRPV(username, password, listaVasiPerRichiesta(tipoRichiesta, codIcqrf, Utility.StatoGIAS.valida_per_invio, codOper), tipoRichiesta, codIcqrf, codOper, personaFisica)

                        'Chiamo il webService
                        writer.aggiornaStato(ObjParametri_Server, listaVasiValidi, codIcqrf, Utility.StatoGIAS.invio_in_corso, codOper)
                        madeChanges = True
                        Dim converted = False
                        Dim result As New VasiSiRPVOutput
                        Try
                            Dim rispostaInvio = caller.ChiamataWSAsync(vasiValidi, urlASync, CertificateFile)
                            XMLUtility.getObjectFromResponse(rispostaInvio, result, Utility.getSoapenv)
                            converted = True
                        Catch ex As Exception
                            writer.aggiornaStato(ObjParametri_Server, listaVasiValidi, codIcqrf, Utility.StatoGIAS.invio_non_riuscito, codOper)
                            writer.inserisciLogInvio(ObjParametri_Server, listaVasiValidi, codIcqrf, vasiValidi, "", Utility.VasiSiRPV, True, "", codOper)
                            Throw New TeleregistriExceptionResponseParsing("Vasi", "inserisciAggiornaTuttoInput " + ex.Message)
                        End Try
                        If converted Then
                            If result.IdTrasmissione Is Nothing Then
                                writer.aggiornaStato(ObjParametri_Server, listaVasiValidi, codIcqrf, Utility.StatoGIAS.invio_non_riuscito, codOper)
                                writer.inserisciLogInvio(ObjParametri_Server, listaVasiValidi, codIcqrf, vasiValidi, "", Utility.VasiSiRPV, True, "", codOper)
                            Else
                                writer.aggiornaStato(ObjParametri_Server, listaVasiValidi, codIcqrf, Utility.StatoGIAS.invio_effettuato_correttamente, codOper)
                                writer.inserisciLogInvio(ObjParametri_Server, listaVasiValidi, codIcqrf, vasiValidi, "", Utility.VasiSiRPV, False, result.IdTrasmissione, codOper)
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
        Dim op = reader.getOperazioniDaControllare(ObjParametri_Server, Utility.VasiSiRPV)
        For Each operation As DataRow In op.Rows
            Dim vas As New xVasiSiRPV(ObjParametri_Server, ObjParametri_Utenti)
            Dim processed = False
            Dim checkResult As String
            Dim checkRes As New GetVasiSiRPVOutput
            Dim received = False
            Dim listaVasiDaAggiornare As List(Of String) = reader.listaVasiDaAggiornare(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")))
            Dim codIcqrf As String = reader.getCodIcqrfFromLogInvioID(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")))
            Dim codOper As String = reader.getCodOperFromLogInvioID(ObjParametri_Server, operation.Item("ws_RegVino_LogInvio_Cod"))
            madeChanges = True
            Try
                checkResult = caller.ChiamataWSSync(vas.sGetVasiSiRPV(username, password, operation.Item("idTrasmissione_SIAN")), urlSync, CertificateFile)
                XMLUtility.getObjectFromResponse(checkResult, checkRes, Utility.getSoapenv)
            Catch ex As Exception
                utils.AggiornaControllataLogInvio(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), False)
                utils.inserisciErrore(ObjParametri_Server, operation.Item("ws_RegVino_LogInvio_Cod"), "EXP", "XML Parse Element Response Exception on Operation:" + checkRes.GetType.Name, 0, "", "", "", codOper)
                writer.aggiornaStato(ObjParametri_Server, listaVasiDaAggiornare, codIcqrf, Utility.StatoGIAS.in_fase_di_verifica, codOper)
                Throw New TeleregistriExceptionResponseParsing("Vasi", "inserisciAggiornaTuttoOutput " + ex.Message)
            End Try
            Try
                If Not checkRes.Esito Is Nothing Then

                    If checkRes.Esito.codice <> "000" Then
                        'Ricevuto dati validi
                        Dim typecode = utils.getTypeCode(ObjParametri_Server, checkRes.Esito.codice)
                        Select Case typecode
                            Case Utility.TipoRitornoErrore
                                utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), checkRes.Esito.codice, checkRes.Esito.messaggio, 0, "", "", "", codOper)
                                writer.aggiornaStato(ObjParametri_Server, listaVasiDaAggiornare, codIcqrf, Utility.StatoGIAS.errori_rilevati_dal_Sian, codOper)
                            Case Utility.TipoRitornoWarning
                                utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), checkRes.Esito.codice, checkRes.Esito.messaggio, 0, "", "", "", codOper)
                                writer.aggiornaStato(ObjParametri_Server, listaVasiDaAggiornare, codIcqrf, Utility.StatoGIAS.errori_rilevati_dal_Sian, codOper)
                            Case Utility.TipoRitornoInfo
                            Case Else
                                utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), checkRes.Esito.codice, checkRes.Esito.messaggio, 0, "", "", "", codOper)
                                writer.aggiornaStato(ObjParametri_Server, listaVasiDaAggiornare, codIcqrf, Utility.StatoGIAS.errori_rilevati_dal_Sian, codOper)
                        End Select
                        utils.AggiornaControllataLogInvio(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), True)
                        received = True
                    Else
                        'Richiesta non ancora valutata
                        writer.aggiornaStato(ObjParametri_Server, listaVasiDaAggiornare, codIcqrf, Utility.StatoGIAS.in_fase_di_verifica, codOper)
                    End If
                Else
                    utils.AggiornaControllataLogInvio(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), True)
                    received = True
                End If
                If received Then
                    Try
                        If checkRes.Vasi IsNot Nothing Then
                            For Each vasoRes As VasoVinarioOutput In checkRes.Vasi
                                Dim tipoRitorno As String = utils.getTypeCode(ObjParametri_Server, vasoRes.Esito.codice)
                                Select Case tipoRitorno
                                    Case Utility.TipoRitornoErrore
                                        utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), vasoRes.Esito.codice, vasoRes.Esito.messaggio, 0, "", vasoRes.CodVaso, checkRes.CodiceIcqrf, codOper)
                                        writer.aggiornaStato(ObjParametri_Server, vasoRes.CodVaso, codIcqrf, Utility.StatoGIAS.errori_rilevati_dal_Sian, codOper)
                                    Case Utility.TipoRitornoWarning
                                        utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), vasoRes.Esito.codice, vasoRes.Esito.messaggio, 0, "", vasoRes.CodVaso, checkRes.CodiceIcqrf, codOper)
                                        writer.aggiornaStato(ObjParametri_Server, vasoRes.CodVaso, codIcqrf, Utility.StatoGIAS.errori_rilevati_dal_Sian, codOper)
                                    Case Utility.TipoRitornoInfo
                                        writer.aggiornaStato(ObjParametri_Server, vasoRes.CodVaso, codIcqrf, Utility.StatoGIAS.valida_nel_sian, codOper)
                                    Case Else
                                        utils.inserisciErrore(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), vasoRes.Esito.codice, vasoRes.Esito.messaggio, 0, "", vasoRes.CodVaso, checkRes.CodiceIcqrf, codOper)
                                        writer.aggiornaStato(ObjParametri_Server, vasoRes.CodVaso, codIcqrf, Utility.StatoGIAS.errori_rilevati_dal_Sian, codOper)
                                End Select
                            Next
                        Else
                            Dim listaVasi = CInt(operation.Item("ws_RegVino_LogInvio_Cod"))
                            writer.aggiornaStato(ObjParametri_Server, listaVasi, codIcqrf, Utility.StatoGIAS.errori_rilevati_dal_Sian, codOper)
                        End If
                    Catch ex As Exception
                        Dim listaVasi = CInt(operation.Item("ws_RegVino_LogInvio_Cod"))
                        writer.aggiornaStato(ObjParametri_Server, listaVasi, codIcqrf, Utility.StatoGIAS.errori_rilevati_dal_Sian, codOper)
                    End Try
                End If
            Catch ex As Exception
                utils.AggiornaControllataLogInvio(ObjParametri_Server, CInt(operation.Item("ws_RegVino_LogInvio_Cod")), False)
            End Try
        Next
        Return madeChanges
    End Function

    Private Function listaVasiPerRichiesta(tRichiesta As TipoRichiesta, codIcqrf As String, statoGIAS As Utility.StatoGIAS, codOper As String) As Object
        Dim dt As DataTable
        Dim list As New List(Of String)
        Select Case tRichiesta
            Case TipoRichiesta.A
                dt = reader.LeggiVasiPerRichiesta(ObjParametri_Server, statoGIAS, "A", codIcqrf, codOper)
            Case TipoRichiesta.I
                dt = reader.LeggiVasiPerRichiesta(ObjParametri_Server, statoGIAS, "I", codIcqrf, codOper)
            Case 2
                dt = reader.LeggiVasiPerRichiesta(ObjParametri_Server, statoGIAS, "E", codIcqrf, codOper)
        End Select
        For Each row As DataRow In dt.Rows
            list.Add(row.Item("CodVaso"))
        Next
        Return list
    End Function

End Class
