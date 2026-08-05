Imports System.IO
Imports AgronicaCoreAcciseCommon.DAA.Messaggi.Ie815
Imports AgronicaCoreAcciseDAL
Imports AgronicaCoreEntityFramework_POCO

Public Class UploadController

    Private ReadOnly _signHelper As ISignHelper
    Private ReadOnly _logger As AcciseLogger
    Private ReadOnly _soapFactory As SoapControllerFactory
    Private ReadOnly _dataManager As IDataManager
    Private ReadOnly _fileSystemPersister As FileSystemPersister
    Private ReadOnly _fileManager As IFileManager
    Public Sub New(
            ByVal signHelper As ISignHelper,
            ByVal logger As AcciseLogger,
            ByVal soapFactory As SoapControllerFactory,
            ByVal dataManager As IDataManager,
            ByVal fileSystemPersister As FileSystemPersister,
            ByVal fileManager As IFileManager
        )
        _signHelper = signHelper
        _logger = logger
        _soapFactory = soapFactory
        _dataManager = dataManager
        _fileSystemPersister = fileSystemPersister
        _fileManager = fileManager
    End Sub

    Public Function InviaMessaggiCreati() As List(Of String)

        Dim nomeProcedura = "UploadController.InviaMessaggiCreati"
        Dim errori = New List(Of String)

        _logger.Logga(nomeProcedura, "Richiesta di invio messaggi creati ricevuta")

        Dim richiesteCreate = _dataManager.ACCDAA_LeggiDaaDaSpedire()

        If Not richiesteCreate Is Nothing AndAlso richiesteCreate.Any() Then

            Dim soapController = _soapFactory.Create(Of SOAPControllerWsFtp)()

            richiesteCreate.ForEach(Sub(rc)
                                        Try


                                            Dim errore As String = String.Empty
                                            If String.IsNullOrEmpty(rc.Txt_Messaggio_C) Then
                                                Throw New Exception(String.Format("Il contenuto del campo Txt_Messaggio_C non è valorizzato. Nome del file completo: {0}", rc.NomeFileCompleto))
                                            End If
                                            Dim tipoMessaggio = rc.Txt_Messaggio_C.Split(ControlChars.CrLf.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Skip(1).First().Substring(0, 5).ToUpper()

                                            If MessageStateManager.Recognized(tipoMessaggio) Then
                                                Select Case tipoMessaggio
                                                    Case "ALCOA"
                                                        Invia_ALCOAV(rc, tipoMessaggio, soapController, errori)
                                                    Case Else
                                                        Invia_IE818(rc, tipoMessaggio, soapController, errori)
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
            _logger.Logga(nomeProcedura, "Nessun messaggio trovato da inviare all'Agenzia delle Dogane")
        End If

        _logger.Logga(nomeProcedura, "Richiesta di invio messaggi creati evasa")

        Return errori

    End Function

    Private Sub Invia_ALCOAV(ByVal rc As ACCDAA_W_StoricoMessaggiDogane,
                    ByVal tipoMessaggio As String,
                    ByVal soapController As ISOAPController,
                    ByRef errori As List(Of String))

        Dim errore As String = ""

        Dim riepilogo_testata = _dataManager.Leggi_ACCDAA_ACC_RiepilogoInvioDati(rc.RiepilogoInvioDati_id)
        If riepilogo_testata Is Nothing Then
            Throw New Exception(String.Format("Impossibile trovare il record nella tabella ACCDAA_ACC_RiepilogoInvioDati con RiepilogoInvioDati_id {0}", rc.RiepilogoInvioDati_id))
        End If

        Dim plainData = Text.Encoding.UTF8.GetBytes(rc.Txt_Messaggio_C)
        Dim tempDaaFile = _fileSystemPersister.SaveDAAInTempFile(plainData, rc.NomeFileCompleto)

        Dim nomeFile = rc.NomeFileCompleto.Replace(".TXT", ".p7m")
        Dim percorsoFileSegnato = _signHelper.Sign(plainData, tempDaaFile)
        Dim signedData = File.ReadAllBytes(percorsoFileSegnato)

        If Not soapController.PUT(signedData, nomeFile, errore) Then
            errori.Add(String.Format("Errore durante l'invio del file: {0}. Errore {1}", nomeFile, errore))
            _dataManager.ACCDAA_StoricoMessaggiDogane_Errori_Logga(rc.id, errore, "", "", "")
        Else
            ' file caricato senza errori (aggiorno stato avanzamento workflow
            rc.Stato_Manager = MessageStateManager.GetState(tipoMessaggio, DAA_Step_Enum.Caricamento, Object_State.Messaggio, True, rc.Stato_Manager)
            riepilogo_testata.Stato = MessageStateManager.GetState(tipoMessaggio, DAA_Step_Enum.Caricamento, Object_State.Testata, True, riepilogo_testata.Stato)

            _dataManager.ACCDAA_StoricoMessaggiDogane_Errori_Elimina(rc.id)
            _dataManager.ACCDAA_ACC_RiepilogoInvioDati_Aggiorna(riepilogo_testata)
            _dataManager.ACCDAA_StoricoMessaggiDogane_Aggiorna(rc)

        End If

        _fileManager.Delete(Path.Combine(_fileManager.OttieniPercorso(AccisePath.Temp), tempDaaFile))
        _fileManager.Delete(percorsoFileSegnato)

    End Sub
    Private Sub Invia_IE818(ByVal rc As ACCDAA_W_StoricoMessaggiDogane,
                    ByVal tipoMessaggio As String,
                    ByVal soapController As ISOAPController,
                    ByRef errori As List(Of String))

        Dim errore As String = ""

        Dim daa_testata = _dataManager.Leggi_ACCDAA_Testata(rc.DAA_ID)
        If daa_testata Is Nothing Then
            Throw New Exception(String.Format("Impossibile trovare il record nella tabella ACCDAA_DAA_Testata con DAA_ID {0}", rc.DAA_ID))
        End If

        Dim plainData = Text.Encoding.UTF8.GetBytes(rc.Txt_Messaggio_C)
        Dim tempDaaFile = _fileSystemPersister.SaveDAAInTempFile(plainData, rc.NomeFileCompleto)

        Dim nomeFile = rc.NomeFileCompleto.Replace(".TXT", ".p7m")
        Dim percorsoFileSegnato = _signHelper.Sign(plainData, tempDaaFile)
        Dim signedData = File.ReadAllBytes(percorsoFileSegnato)

        If Not soapController.PUT(signedData, nomeFile, errore) Then
            errori.Add(String.Format("Errore durante l'invio del file: {0}. Errore {1}", nomeFile, errore))
            _dataManager.ACCDAA_StoricoMessaggiDogane_Errori_Logga(rc.id, errore, "", "", "")
        Else
            ' file caricato senza errori (aggiorno stato avanzamento workflow
            rc.Stato_Manager = MessageStateManager.GetState(tipoMessaggio, DAA_Step_Enum.Caricamento, Object_State.Messaggio, True, rc.Stato_Manager)
            daa_testata.Stato_Manager = MessageStateManager.GetState(tipoMessaggio, DAA_Step_Enum.Caricamento, Object_State.Testata, True, daa_testata.Stato_Manager)

            _dataManager.ACCDAA_StoricoMessaggiDogane_Errori_Elimina(rc.id)
            _dataManager.ACCDAA_DAA_Testata_Aggiorna(daa_testata)
            _dataManager.ACCDAA_StoricoMessaggiDogane_Aggiorna(rc)

        End If

        _fileManager.Delete(Path.Combine(_fileManager.OttieniPercorso(AccisePath.Temp), tempDaaFile))
        _fileManager.Delete(percorsoFileSegnato)

    End Sub

End Class
