Imports System.Text
Imports System.Text.RegularExpressions
Imports AgronicaCoreAcciseDAL
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class NotificheController

    Private ReadOnly _signHelper As ISignHelper
    Private ReadOnly _logger As AcciseLogger
    Private ReadOnly _soapFactory As SoapControllerFactory
    Private ReadOnly _persister As FileSystemPersister
    Private ReadOnly _dataManager As IDataManager
    Private ReadOnly _fileManager As IFileManager
    Public Sub New(
            ByVal signHelper As ISignHelper,
            ByVal logger As AcciseLogger,
            ByVal soapFactory As SoapControllerFactory,
            ByVal dataManager As IDataManager,
            ByVal persister As FileSystemPersister,
            ByVal fileManager As IFileManager
        )
        _signHelper = signHelper
        _logger = logger
        _soapFactory = soapFactory
        _dataManager = dataManager
        _persister = persister
        _fileManager = fileManager
    End Sub

    Public Function LeggiMessaggiRisposte() As List(Of String)

        Dim nomeProcedura = "NotificheController.LeggiMessaggiRisposte"
        Dim errori = New List(Of String)
        Dim ultimoErrore = String.Empty
        Dim messaggioErrore = String.Empty

        _logger.Logga(nomeProcedura, "Richiesta di lettura messaggi di risposta ricevuta")

        Dim inAttessaRisposta = _dataManager.ACCDAA_LeggiMessaggiRisposta()

        If Not inAttessaRisposta Is Nothing AndAlso inAttessaRisposta.Any() Then

            inAttessaRisposta.ForEach(Sub(rc)
                                          Try

                                              If String.IsNullOrEmpty(rc.Txt_Messaggio_C) Then
                                                  Throw New Exception(String.Format("Il contenuto del campo Txt_Messaggio_C non è valorizzato. Nome del file completo: {0}", rc.NomeFileCompleto))
                                              End If
                                              Dim tipoMessaggio = rc.Txt_Messaggio_C.Split(ControlChars.CrLf.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Skip(1).First().Substring(0, 5).ToUpper()

                                              If MessageStateManager.Recognized(tipoMessaggio) Then

                                                  Dim nomeFile = rc.NomeFileCompleto.Split(".").FirstOrDefault()
                                                  Dim progressivoFile = rc.NomeFileCompleto.Split(".").Skip(1).FirstOrDefault()
                                                  Dim risposta = Me.DIR(nomeFile + ".D*", nomeFile, progressivoFile, "D")

                                                  If risposta Is Nothing Then
                                                      _logger.Logga(nomeProcedura, String.Format("File risposta non disponibile per il messaggio con nome file completo {0}", rc.NomeFileCompleto))
                                                      Return
                                                  End If

                                                  ' Scarico e salvo contenuto messaggio D
                                                  Dim nomeFileRicevuta = risposta.NomeFileRicevuta
                                                  Dim signedData = [GET](nomeFileRicevuta, ultimoErrore)
                                                  If signedData Is Nothing OrElse Not String.IsNullOrEmpty(ultimoErrore) Then
                                                      messaggioErrore = String.Format("Errore durante la lettura del file risposta {0} per il messaggio con nome file completo {1}", nomeFileRicevuta, rc.NomeFileCompleto)
                                                      _logger.Logga(nomeProcedura, messaggioErrore)
                                                      Throw New Exception(messaggioErrore)
                                                  End If

                                                  Dim unsignedData = _signHelper.LeggiContenutoSegnato(signedData, nomeFileRicevuta)
                                                  If unsignedData Is Nothing Then
                                                      messaggioErrore = String.Format("Errore durante la lettura del contenuto firmato file risposta {0} per il messaggio con nome file completo {1}", nomeFileRicevuta, rc.NomeFileCompleto)
                                                      _logger.Logga(nomeProcedura, messaggioErrore)
                                                      Throw New Exception(messaggioErrore)
                                                  End If

                                                  rc.Txt_Messaggio_D = Encoding.UTF8.GetString(unsignedData.ToArray())
                                                  rc.Codice_File_Acquisito = risposta.CodiceFile

                                                  Select Case tipoMessaggio
                                                      Case "ALCOA"
                                                          Risposta_ALCOAV(rc, tipoMessaggio)
                                                      Case Else
                                                          Risposta_IE818(rc, tipoMessaggio)
                                                  End Select

                                              Else
                                                  _logger.Logga(nomeProcedura, String.Format("Trovato messaggio di tipo {0} non gestito. Nome del file completo: {1}", tipoMessaggio, rc.NomeFileCompleto))
                                              End If

                                          Catch ex As Exception
                                              _logger.Logga(nomeProcedura, ex.Message)
                                              errori.Add(ex.Message)
                                          End Try
                                      End Sub)

        Else
            _logger.Logga(nomeProcedura, "Nessun messaggio torvato in attesa di risposta")
        End If

        _logger.Logga(nomeProcedura, "Richiesta di lettura messaggi di risposta evasa")

        Return errori

    End Function

    Private Sub Risposta_ALCOAV(ByVal rc As ACCDAA_W_StoricoMessaggiDogane,
                                ByVal tipoMessaggio As String)

        Dim riepilogo_testata = _dataManager.Leggi_ACCDAA_ACC_RiepilogoInvioDati(rc.RiepilogoInvioDati_id)
        If riepilogo_testata Is Nothing Then
            Throw New Exception(String.Format("Impossibile trovare il record nella tabella ACCDAA_ACC_RiepilogoInvioDati con RiepilogoInvioDati_id {0}", rc.RiepilogoInvioDati_id))
        End If

        ' verifica se messaggio risposta senza errori
        Dim righeErrori = D_Parser.Parse(rc.Txt_Messaggio_D).ToList()
        If righeErrori.Count() = 1 AndAlso righeErrori.First().NonErrore() Then
            rc.Stato_Manager = MessageStateManager.GetState(tipoMessaggio, DAA_Step_Enum.RispostaRicevuta, Object_State.Messaggio, True, rc.Stato_Manager)
            _dataManager.ACCDAA_StoricoMessaggiDogane_Aggiorna(rc)
        Else
            rc.Stato_Manager = MessageStateManager.GetState(tipoMessaggio, DAA_Step_Enum.RispostaRicevuta, Object_State.Messaggio, False, rc.Stato_Manager)
            riepilogo_testata.Stato = MessageStateManager.GetState(tipoMessaggio, DAA_Step_Enum.RispostaRicevuta, Object_State.Testata, False, rc.Stato_Manager)
            _dataManager.ACCDAA_StoricoMessaggiDogane_Aggiorna(rc)
            _dataManager.ACCDAA_ACC_RiepilogoInvioDati_Aggiorna(riepilogo_testata)
            _dataManager.ACCDAA_StoricoMessaggiDogane_Errori_Logga(rc.id, righeErrori)
        End If

    End Sub

    Private Sub Risposta_IE818(ByVal rc As ACCDAA_W_StoricoMessaggiDogane,
                                ByVal tipoMessaggio As String)

        Dim daa_testata = _dataManager.Leggi_ACCDAA_Testata(rc.DAA_ID)
        If daa_testata Is Nothing Then
            Throw New Exception(String.Format("Impossibile trovare il record nella tabella ACCDAA_DAA_Testata con DAA_ID {0}", rc.DAA_ID))
        End If

        ' verifica se messaggio risposta senza errori
        Dim righeErrori = D_Parser.Parse(rc.Txt_Messaggio_D).ToList()
        If righeErrori.Count() = 1 AndAlso righeErrori.First().NonErrore() Then
            rc.Stato_Manager = MessageStateManager.GetState(tipoMessaggio, DAA_Step_Enum.RispostaRicevuta, Object_State.Messaggio, True, rc.Stato_Manager)
            _dataManager.ACCDAA_StoricoMessaggiDogane_Aggiorna(rc)
        Else
            rc.Stato_Manager = MessageStateManager.GetState(tipoMessaggio, DAA_Step_Enum.RispostaRicevuta, Object_State.Messaggio, False, rc.Stato_Manager)
            daa_testata.Stato_Manager = MessageStateManager.GetState(tipoMessaggio, DAA_Step_Enum.RispostaRicevuta, Object_State.Testata, False, rc.Stato_Manager)
            _dataManager.ACCDAA_StoricoMessaggiDogane_Aggiorna(rc)
            _dataManager.ACCDAA_DAA_Testata_Aggiorna(daa_testata)
            _dataManager.ACCDAA_StoricoMessaggiDogane_Errori_Logga(rc.id, righeErrori)
        End If

    End Sub

    Public Function LeggiEsiti() As List(Of String)

        Dim nomeProcedura = "NotificheController.LeggiEsiti"
        Dim errori = New List(Of String)
        Dim ultimoErrore = String.Empty
        Dim messaggioErrore = String.Empty


        _logger.Logga(nomeProcedura, "Richiesta di lettura messaggi di esito ricevuta")

        Dim inAttessaEsito = _dataManager.ACCDAA_LeggiEsiti()

        If Not inAttessaEsito Is Nothing AndAlso inAttessaEsito.Any() Then

            inAttessaEsito.ForEach(Sub(rc)
                                       Try

                                           If String.IsNullOrEmpty(rc.Txt_Messaggio_C) Then
                                               Throw New Exception(String.Format("Il contenuto del campo Txt_Messaggio_C non è valorizzato. Nome del file completo: {0}", rc.NomeFileCompleto))
                                           End If
                                           Dim tipoMessaggio = rc.Txt_Messaggio_C.Split(ControlChars.CrLf.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Skip(1).First().Substring(0, 5).ToUpper()

                                           If MessageStateManager.Recognized(tipoMessaggio) Then
                                               Dim nomeFile = rc.NomeFileCompleto.Split(".").FirstOrDefault()
                                               Dim progressivoFile = rc.NomeFileCompleto.Split(".").Skip(1).FirstOrDefault()
                                               Dim risposta = Me.DIR(nomeFile + ".J*", nomeFile, progressivoFile, "J")

                                               If risposta Is Nothing Then
                                                   _logger.Logga(nomeProcedura, String.Format("File esito non disponibile per il messaggio con nome file completo {0}", rc.NomeFileCompleto))
                                                   Return
                                               End If

                                               ' Scarico e salvo contenuto messaggio D
                                               Dim nomeFileRicevuta = risposta.NomeFileRicevuta
                                               Dim signedData = [GET](nomeFileRicevuta, ultimoErrore)
                                               If signedData Is Nothing OrElse Not String.IsNullOrEmpty(ultimoErrore) Then
                                                   messaggioErrore = String.Format("Errore durante la lettura del file esito {0}  per il messaggio con nome file completo {1}", nomeFileRicevuta, rc.NomeFileCompleto)
                                                   _logger.Logga(nomeProcedura, messaggioErrore)
                                                   Throw New Exception(messaggioErrore)
                                               End If

                                               Dim unsignedData = _signHelper.LeggiContenutoSegnato(signedData, nomeFileRicevuta)
                                               If unsignedData Is Nothing Then
                                                   messaggioErrore = String.Format("Errore durante la lettura del contenuto firmato file esito {0}  per il messaggio con nome file completo {1}", nomeFileRicevuta, rc.NomeFileCompleto)
                                                   _logger.Logga(nomeProcedura, messaggioErrore)
                                                   Throw New Exception(messaggioErrore)
                                               End If

                                               rc.Txt_Messaggio_J = Encoding.UTF8.GetString(unsignedData.ToArray())
                                               rc.Codice_File_Acquisito = risposta.CodiceFile

                                               Select Case tipoMessaggio
                                                   Case "ALCOA"
                                                       Esito_ALCOAV(rc, tipoMessaggio)
                                                   Case Else
                                                       Esito_IE818(rc, tipoMessaggio)
                                               End Select

                                           Else
                                               _logger.Logga(nomeProcedura, String.Format("Trovato messaggio di tipo {0} non gestito per il DAA_ID {1}", tipoMessaggio, rc.DAA_ID))
                                           End If
                                       Catch ex As Exception
                                           _logger.Logga(nomeProcedura, ex.Message)
                                           errori.Add(ex.Message)
                                       End Try
                                   End Sub)

        Else
            _logger.Logga(nomeProcedura, "Nessun DAA torvato in attesa di messaggio di esito")
        End If

        _logger.Logga(nomeProcedura, "Richiesta di lettura messaggi di esito evasa")

        Return errori


    End Function
    Private Sub Esito_ALCOAV(ByVal rc As ACCDAA_W_StoricoMessaggiDogane,
                                ByVal tipoMessaggio As String)

        Dim riepilogo_testata = _dataManager.Leggi_ACCDAA_ACC_RiepilogoInvioDati(rc.RiepilogoInvioDati_id)
        If riepilogo_testata Is Nothing Then
            Throw New Exception(String.Format("Impossibile trovare il record nella tabella ACCDAA_ACC_RiepilogoInvioDati con RiepilogoInvioDati_id {0}", rc.RiepilogoInvioDati_id))
        End If

        Dim righeFileRisposta = J_Parser_ALCOAV.Parse(rc.Txt_Messaggio_J)
        If righeFileRisposta.Count() = 1 AndAlso righeFileRisposta.FirstOrDefault().Esito = "P" Then

            'risposta positiva (aggiorna storico messaggi , testata daa e mov_dettaglio_tecnico_extra)
            rc.Stato_Manager = MessageStateManager.GetState(tipoMessaggio, DAA_Step_Enum.EsitoRicevuto, Object_State.Messaggio, True, rc.Stato_Manager)
            riepilogo_testata.Stato = MessageStateManager.GetState(tipoMessaggio, DAA_Step_Enum.EsitoRicevuto, Object_State.Testata, True, riepilogo_testata.Stato)

        Else

            'risposta negativa (aggiorna storico messaggi , testata daa e scrive in storico messaggi errore
            rc.Stato_Manager = MessageStateManager.GetState(tipoMessaggio, DAA_Step_Enum.EsitoRicevuto, Object_State.Messaggio, False, rc.Stato_Manager)
            riepilogo_testata.Stato = MessageStateManager.GetState(tipoMessaggio, DAA_Step_Enum.EsitoRicevuto, Object_State.Testata, False, riepilogo_testata.Stato)
            _dataManager.ACCDAA_StoricoMessaggiDogane_Errori_Logga(rc.id, righeFileRisposta.Skip(1).ToList())

        End If

        _dataManager.ACCDAA_StoricoMessaggiDogane_Aggiorna(rc)
        _dataManager.ACCDAA_ACC_RiepilogoInvioDati_Aggiorna(riepilogo_testata)


    End Sub
    Private Sub Esito_IE818(ByVal rc As ACCDAA_W_StoricoMessaggiDogane,
                                ByVal tipoMessaggio As String)

        Dim daa_testata = _dataManager.Leggi_ACCDAA_Testata(rc.DAA_ID)
        If daa_testata Is Nothing Then
            Throw New Exception(String.Format("Impossibile trovare il record nella tabella ACCDAA_DAA_Testata con DAA_ID {0}", rc.DAA_ID))
        End If

        Dim righeFileRisposta = J_Parser.Parse(rc.Txt_Messaggio_J).ToList()
        If righeFileRisposta.Count() = 1 AndAlso righeFileRisposta.FirstOrDefault().Esito = "P" Then

            'risposta positiva (aggiorna storico messaggi , testata daa e mov_dettaglio_tecnico_extra)
            Dim esito = righeFileRisposta.FirstOrDefault()

            Dim dataConvalida As DateTime = AGRODATAINIZIO
            If Not String.IsNullOrEmpty(esito.Data) Then
                dataConvalida = DateTime.ParseExact(esito.Data, "ddMMyyyy", Nothing)
            End If

            Dim ProgressivoArc As Integer = 0
            rc.Stato_Manager = MessageStateManager.GetState(tipoMessaggio, DAA_Step_Enum.EsitoRicevuto, Object_State.Messaggio, True, rc.Stato_Manager)
            daa_testata.Stato_Manager = MessageStateManager.GetState(tipoMessaggio, DAA_Step_Enum.EsitoRicevuto, Object_State.Testata, True, daa_testata.Stato_Manager)
            daa_testata.Data_riferimento = dataConvalida

            If tipoMessaggio.Equals("IE815") Then
                daa_testata.ARC = esito.ARC
            End If

            If tipoMessaggio.Equals("IE815") OrElse tipoMessaggio.Equals("IE813") Then
                Int32.TryParse(esito.ProgressivoARC, ProgressivoArc)
                daa_testata.ARC_Progressivo = ProgressivoArc
            End If

            Dim mvde = _dataManager.Mov_Dettaglio_Tecnico_Leggi(rc.DAA_ID)
            If mvde Is Nothing Then
                Throw New Exception(String.Format("Impossibile trovare il record nella tabella Mov_Dettaglio_Tecnico_Extra con DAA_ID {0}", rc.DAA_ID))
            End If
            mvde.Codice_Alternativo = daa_testata.ARC

            _dataManager.ACCDAA_StoricoMessaggiDogane_Aggiorna(rc)
            _dataManager.ACCDAA_DAA_Testata_Aggiorna(daa_testata)
            _dataManager.Mov_Dettaglio_Tecnico_Extra_Aggiorna(mvde)

        Else

            'risposta negativa (aggiorna storico messaggi , testata daa e scrive in storico messaggi errore
            rc.Stato_Manager = MessageStateManager.GetState(tipoMessaggio, DAA_Step_Enum.EsitoRicevuto, Object_State.Messaggio, False, rc.Stato_Manager)
            daa_testata.Stato_Manager = MessageStateManager.GetState(tipoMessaggio, DAA_Step_Enum.EsitoRicevuto, Object_State.Testata, False, daa_testata.Stato_Manager)
            _dataManager.ACCDAA_StoricoMessaggiDogane_Errori_Logga(rc.id, righeFileRisposta)
            _dataManager.ACCDAA_StoricoMessaggiDogane_Aggiorna(rc)
            _dataManager.ACCDAA_DAA_Testata_Aggiorna(daa_testata)

        End If

    End Sub

    Public Function DIR(ByVal searchPattern As String,
                        ByVal nomeFile As String,
                        ByVal progressivoFile As String,
                        ByVal tipoMessaggio As String) As FileRisposta

        Dim errore As String = ""

        Dim sc = _soapFactory.Create(Of SOAPControllerWsFtp)
        Dim risposte = sc.DIR(searchPattern, errore)

        If risposte Is Nothing OrElse Not risposte.Any Then
            Return Nothing
        End If

        Dim match = tipoMessaggio + Regex.Replace(progressivoFile, "\D", "")
        Dim nomeFileRisposta = String.Format("{0}.{1}.p7m", nomeFile, match).ToUpper()

        Dim rispostaEsatta = risposte.Where(Function(r)
                                                Return r.NomeFileRicevuta.ToUpper().Equals(nomeFileRisposta)
                                            End Function).FirstOrDefault()
        If Not rispostaEsatta Is Nothing Then
            Return rispostaEsatta
        Else
            Return risposte.FirstOrDefault()
        End If

    End Function

    Public Function [GET](ByVal nomeFileRisposta As String, ByRef errore As String) As Byte()

        Dim sc = _soapFactory.Create(Of SOAPControllerWsFtp)
        Return sc.GET(nomeFileRisposta, errore)

    End Function


End Class


