Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreVarieDAL
Imports AgronicaGSB_Gestione_Servizi_in_Background.Utility

Public Class Agro_Servizio



#Region "Parametri"

    Private WithEvents _t As Timers.Timer
    Public Event _EventoErrore As EventoErroreServizioHandler
    Public Event _EventoImportazione As EventoServizioHandler


    Private CeUnTaskThreadInEsecuzione As Boolean
    Private _lock As Object

    'contiene tutti i parametri che identificano il servizio
    'se alla creazione il record è errato o ci sono problemi ne crea uno fittizio,
    'ma imposta attivo=0 sia nell'oggetto che nel db,
    'la descrizione del servizio con l'errore e lo stato errore,
    'alla successiva rilettura della riga se i dati sono stati corretti potrebbe riattivarsi,
    'altrimenti occorre fermare il servizio windows e riattivarlo
    Private _configurazioneServizio As Configurazione_Servizio
    
    'parametri impostabili
    Public LogFileName As String = ""
    Public Ferma As Boolean = False
    Public ID As String = "Servizio riga"

    'variabili interne
    Private _dataPrimaSincronizzazione As DateTime
    Private _dataInizioUltimaSincronizzazione As DateTime
    Private _sincronizzazioniEseguiteNellaFinestra As Integer = -1
    Private _objParametriSuperServer As AgronicaCoreParametri
    Private _objParametriServer As AgronicaCoreParametri
    Private _objParametriUtenti As AgronicaCoreParametri
    Public Log As EventLog
    Private Configurazione_Servizi_R As AgronicaCoreVarieDAL.Configurazione_Servizi_R
    Private _contatore As Integer = 0

#End Region



#Region "Init"

    Sub New(ByVal pivaSuperuser As String,
            ByVal idServizio As enum_Id_Servizio,
            ByVal tipoSincro As enum_Tipi_Servizi_Background,
            ByVal idRiga As Integer,
            ByVal _Id_Db As Integer,
            ByVal objParametriSuperServer As AgronicaCoreParametri,
            ByVal objParametriServer As AgronicaCoreParametri,
            ByVal objParametriUtenti As AgronicaCoreParametri,
            ByRef log As EventLog)

        '-----------------INIZIALIZZAZIONE--------------------
        'attenzione,  Configurazione_Servizio non ancora creati

        '-------0--------------------associo objParametri
        _objParametriSuperServer = objParametriSuperServer
        _objParametriServer = objParametriServer
        _objParametriUtenti = objParametriUtenti

        '-------1--------------------controllo _PivaSuperuser
        ControlloSuperuser(pivaSuperuser)


        '-------2--------------------inizializzo la classe Configurazione_Servizi_R usata in vari punti
        Configurazione_Servizi_R = New AgronicaCoreVarieDAL.Configurazione_Servizi_R()


        '-------3--------------------Creo Configurazione_Servizio
        'vengono riletti ad ogni chiamata di avviaServizio per verificare se cambiare la finestra, se attivarlo o disattivarlo etc.. 
        Leggi_ConfigurazioneServizi(pivaSuperuser, idServizio, tipoSincro, idRiga)
        'Configurazione_Servizio ora è valorizzato


        '-------4--------------------associo EventLog
        Me.Log = log


        '-------5--------------------creo l'ID per il log
        ID = "" & _configurazioneServizio.PivaSuperuser & "-" & _configurazioneServizio.Id_Servizio & "-" & _configurazioneServizio.Tipo_Sincro & "-" & _configurazioneServizio.Id_Riga & ""


        '-------6--------------------Aggiorno lo stato
        Dim nuovoStato As Utility.ID_eventi = Utility.ID_eventi.ServizioAvviato
        SetStato(nuovoStato)

        _contatore = 0

        CeUnTaskThreadInEsecuzione = False
        _lock = New Object

    End Sub


#End Region



#Region "Task"


    Public Sub Task_Da_Eseguire()


        If _configurazioneServizio.periodoPolling > 0 Then
            Try

                'avvio la prima importazione, in _t_elapsed avvio le successive
                Task()

                _t = New Timers.Timer With {
                    .Interval = _configurazioneServizio.periodoPolling * 1000
                }
                _t.Start()

            Catch ex As Exception

                LoggaErr(ex, -1)
                Dim err As New EventoErroreServizio("Errore Inizializza Servizio " & ID & " : " & ex.Message)
                RaiseEvent _EventoErrore(Me, err)

            End Try

        End If

        Dim ev As New EventoServizio("Servizio " & ID & " pronto.")
        RaiseEvent _EventoImportazione(Me, ev)


    End Sub


    Private Sub _t_Elapsed(ByVal sender As Object, ByVal e As Timers.ElapsedEventArgs) Handles _t.Elapsed

        Try

            Task()

        Catch ex As Exception

            LoggaErr(ex, -1)
            Dim err As New EventoErroreServizio("Eccezione: " & ex.Message)
            RaiseEvent _EventoErrore(Me, err)

        End Try

    End Sub


    Public Sub Task()

        Dim contatoreQuestoLoop As Integer = 0

        SyncLock _lock
            _contatore += 1
            contatoreQuestoLoop = _contatore

            'Verifico subito se c'è un thread già in esecuzione
            If CeUnTaskThreadInEsecuzione Then
                'task non completato perché fuori dalla finestra o già eseguito frequenza volte nella finestra
                LoggaWarning("Task non completato" & vbCrLf & "C'è già un Task in esecuzione non ancora terminato (CeUnTaskThreadInEsecuzione=true).", contatoreQuestoLoop)
                Exit Sub
            End If
            
            'verifico come impostato sul db
            If GetStato() = Utility.ID_eventi.Task_Avviato Then
                LoggaWarning("Task non completato" & vbCrLf & "C'è già un Task in esecuzione non ancora terminato  (getStato = Utility.ID_eventi.Task_Avviato)", contatoreQuestoLoop)
                Exit Sub
            End If

            'prendo il lock del task
            CeUnTaskThreadInEsecuzione = True

        End SyncLock



        Try

            SetStato(Utility.ID_eventi.Task_Avviato)

            'vengono riletti ad ogni chiamata di avviaServizio 
            'per verificare se cambiare la finestra, se attivarlo o disattivarlo etc.. 
            'per questo nel costruttore ho solo la chiave della riga
            Aggiorna_ConfigurazioneServizi()

            If _configurazioneServizio.Attivo = 1 Then

                'esegui il task
                Dim Messaggio_di_Ritorno_Opzionale As String = ""
                Dim res As Integer = Task_VerificaTempi_E_Esegui(Messaggio_di_Ritorno_Opzionale, contatoreQuestoLoop)

                Select Case res

                    Case 1
                        'task eseguito e completato
                        Dim msg As String = "Task completato" & vbCrLf & "Messaggio: " & Messaggio_di_Ritorno_Opzionale
                        LoggaOk(msg, contatoreQuestoLoop)
                        SetStato(Utility.ID_eventi.Task_Terminato_Correttamente)

                        MandaEmail(msg)

                        Exit Sub


                    Case 0
                        'task non completato perché fuori dalla finestra o già eseguito frequenza volte nella finestra
                        LoggaWarning("Task non completato" & vbCrLf & "Messaggio: " & Messaggio_di_Ritorno_Opzionale, contatoreQuestoLoop)
                        SetStato(Utility.ID_eventi.Task_Saltato)
                        Exit Sub


                    Case -1
                        'task eseguito ma la funzione della classe richiamata dal selettore ha restituito false
                        Dim msg As String = "Task completato con errori" & vbCrLf & "Messaggio: " & Messaggio_di_Ritorno_Opzionale
                        LoggaErr(msg, contatoreQuestoLoop)
                        'Return False
                        SetStato(Utility.ID_eventi.Task_Terminato_Con_Errori)

                        MandaEmail(msg)

                        Exit Sub


                    Case Else
                        'Non previsto questo valore
                        LoggaErr("Il task ha restituito un valore non previsto" & vbCrLf & "Messaggio: " & Messaggio_di_Ritorno_Opzionale, contatoreQuestoLoop)
                        'Return False
                        SetStato(Utility.ID_eventi.Task_Terminato_Con_Errori)
                        Exit Sub


                End Select


            Else


                'Il task è impostato come non attivo in configurazione_servizi
                LoggaOk("Task non attivo", contatoreQuestoLoop)
                SetStato(Utility.ID_eventi.Task_Sospeso)
                Exit Sub


            End If

        Catch ex As Exception

            loggaErr("Eccezione generata" & vbCrLf & "Messaggio: " & ex.Message, contatoreQuestoLoop)
            SetStato(Utility.ID_eventi.Task_Terminato_Con_Errori)

        Finally

            CeUnTaskThreadInEsecuzione = False 'prima di uscire libero il task

        End Try

    End Sub


    '0 non nella finestra 
    '1 eseguito
    '-1 ritornato false
    Private Function Task_VerificaTempi_E_Esegui(ByRef Messaggio_di_Ritorno_Opzionale As String,
                                                 ByVal contatoreQuestoLoop As Integer) As Integer

        Dim dataCorrente As DateTime = DateTime.Now

        'inizializzo al primo giro
        If _sincronizzazioniEseguiteNellaFinestra = -1 Then
            _sincronizzazioniEseguiteNellaFinestra = 0
            _dataPrimaSincronizzazione = DateTime.Now
            _dataInizioUltimaSincronizzazione = _dataPrimaSincronizzazione
        End If

        Dim ora As Integer = dataCorrente.Hour




        'Controllo il giorno della settimana
        If _configurazioneServizio.Giorno_Settimana_Esecuzione = enum_Giorno_Settimana.Indifferente Or
           _configurazioneServizio.Giorno_Settimana_Esecuzione = dataCorrente.DayOfWeek Then

            'ok proseguo

            'ipotesi ore successive (nello stesso giorno) esempio 1 -> 7
            If _configurazioneServizio.Esecuzione_Ora_Intervallo_Inizio < _configurazioneServizio.Esecuzione_Ora_Intervallo_Fine Then

                ' se l'ora è fuori dall'intervallo indicata esco
                If ora < _configurazioneServizio.Esecuzione_Ora_Intervallo_Inizio Or
                     ora > _configurazioneServizio.Esecuzione_Ora_Intervallo_Fine Then

                    Messaggio_di_Ritorno_Opzionale = "L'orario è al di fuori dell'intervallo impostato per l'esecuzione."
                    Return 0 'salta il giro

                End If

                'ore consecutive + data odierna (maggiore non può essere)
                If _dataInizioUltimaSincronizzazione.Date >= dataCorrente.Date Then
                    'la sincronizzazione precedente è della data odierna,  (maggiore non può essere)
                    'quindi la eseguo solo non ne è già stata eseguita un'altra oggi,
                    'dato che la frequenza la imposto ad 1.
                    'evito così anche sovrapposizioni.

                    'se non sono state eseguite sincronizzazione la eseguo
                    If _sincronizzazioniEseguiteNellaFinestra = 0 Then
                        'ok proseguo
                    Else
                        If _sincronizzazioniEseguiteNellaFinestra >= _configurazioneServizio.Frequenza_Nell_intervallo Then
                            Messaggio_di_Ritorno_Opzionale = "Il task è già stato eseguito  " & _sincronizzazioniEseguiteNellaFinestra & " volta nel giorno corrente."
                            Return 0 'salta il giro
                        Else
                            ' ok proseguo
                        End If
                    End If

                Else
                    'l'ultima sincro è avvenuta almeno un giorno prima
                    'ok proseguo e azzero
                    _sincronizzazioniEseguiteNellaFinestra = 0
                End If


            ElseIf _configurazioneServizio.Esecuzione_Ora_Intervallo_Inizio = _configurazioneServizio.Esecuzione_Ora_Intervallo_Fine Then
                Messaggio_di_Ritorno_Opzionale = "Il task non è stato eseguito perché l'ora inizio e dine sono le stesse. Inizio: " & _configurazioneServizio.Esecuzione_Ora_Intervallo_Inizio & " = Fine:" & _configurazioneServizio.Esecuzione_Ora_Intervallo_Fine & "."
                Return 0 'salta il giro

           
            ElseIf _configurazioneServizio.Esecuzione_Ora_Intervallo_Inizio > _configurazioneServizio.Esecuzione_Ora_Intervallo_Fine Then

                ' ipotesi ore non successive, quindi di giorni differenti  esempio 18 -> 7

                ' se l'ora è fuori dall'intervallo indicata esco
                'attenzione che l'ora è del giorno prima quindi 
                'ho un and perché Esecuzione_Ora_Intervallo_Inizio è maggiore di Esecuzione_Ora_Intervallo_Fine
                If ora < _configurazioneServizio.Esecuzione_Ora_Intervallo_Inizio And
                     ora > _configurazioneServizio.Esecuzione_Ora_Intervallo_Fine Then
                    Messaggio_di_Ritorno_Opzionale = "L'orario è al di fuori dell'intervallo impostato per l'esecuzione."
                    Return 0 'salta il giro

                End If
                'per comodità differenzio il caso in cui l'ora sia compresa
                'tra 0 e Configurazione_Servizio.Esecuzione_Ora_Intervallo_Inizio e tra
                ' Configurazione_Servizio.Esecuzione_Ora_Intervallo_Fine e 24


                If ora < _configurazioneServizio.Esecuzione_Ora_Intervallo_Inizio Then
                    'esempio sono tra 0 e le 7 ( da 18->7 finestra)

                    'se il giorno è lo stesso allora sicuramente già eseguita
                    If _dataInizioUltimaSincronizzazione.Date = dataCorrente.Date Then
                        If _sincronizzazioniEseguiteNellaFinestra >= _configurazioneServizio.Frequenza_Nell_intervallo Then
                            Messaggio_di_Ritorno_Opzionale = "Il task è già stato eseguito  " & _sincronizzazioniEseguiteNellaFinestra & " volta nel giorno corrente."
                            Return 0 'salta il giro
                        Else
                            ' ok proseguo
                        End If
                    End If

                    'se il giorno è il precedente allora controllo l'ora 
                    If _dataInizioUltimaSincronizzazione.Date.AddDays(1) = dataCorrente.Date Then
                        If _dataInizioUltimaSincronizzazione.Hour >= _configurazioneServizio.Esecuzione_Ora_Intervallo_Inizio Then
                            'es iniziata alle 20 del g prima, sono le 5 e intervallo è 18->7 quindi già eseguita
                            If _sincronizzazioniEseguiteNellaFinestra >= _configurazioneServizio.Frequenza_Nell_intervallo Then
                                Messaggio_di_Ritorno_Opzionale = "Il task è già stato eseguito  " & _sincronizzazioniEseguiteNellaFinestra & " volta nel giorno corrente."
                                Return 0 'salta il giro
                            Else
                                ' ok proseguo
                            End If
                        End If

                        'se il giorno è il precedente allora controllo l'ora 
                        If _dataInizioUltimaSincronizzazione.Hour <= _configurazioneServizio.Esecuzione_Ora_Intervallo_Fine Then
                            'es iniziata alle 5 e intervallo è 18->7 quindi è nella finestra precedente e posso eseguire
                            'ok proseguo e azzero 
                            _sincronizzazioniEseguiteNellaFinestra = 0
                        End If
                    End If

                    'da due g prima ok
                    If _dataInizioUltimaSincronizzazione.Date.AddDays(1) < dataCorrente.Date Then
                        'ok proseguo e azzero 
                        _sincronizzazioniEseguiteNellaFinestra = 0
                    End If




                ElseIf ora > _configurazioneServizio.Esecuzione_Ora_Intervallo_Fine Then
                    'esempio sono tra 18 e le 24 ( da 18->7 finestra)

                    'se il giorno è lo stesso allora controllo l'ora
                    If _dataInizioUltimaSincronizzazione.Date = dataCorrente.Date Then
                        If _dataInizioUltimaSincronizzazione.Hour >= _configurazioneServizio.Esecuzione_Ora_Intervallo_Inizio Then
                            'es iniziata alle 19, sono le 23 e intervallo è 18->7 quindi già eseguita
                            If _sincronizzazioniEseguiteNellaFinestra >= _configurazioneServizio.Frequenza_Nell_intervallo Then
                                Messaggio_di_Ritorno_Opzionale = "Il task è già stato eseguito  " & _sincronizzazioniEseguiteNellaFinestra & " volta nel giorno corrente."
                                Return 0 'salta il giro
                            Else
                                ' ok proseguo
                            End If
                        Else
                            'es iniziata alle 5, sono le 19 e intervallo è 18->7 quindi è nella finestra precedente e posso eseguire
                            'ok proseguo e azzero 
                            _sincronizzazioniEseguiteNellaFinestra = 0
                        End If

                    Else
                        'giorni precedenti
                        'es iniziata alle 5, sono le 19 e intervallo è 18->7 quindi è nella finestra precedente e posso eseguire
                        'ok proseguo e azzero 
                        _sincronizzazioniEseguiteNellaFinestra = 0

                    End If


                End If

            End If

        Else

            Messaggio_di_Ritorno_Opzionale = "Il task non è stato eseguito perché il giorno di esecuzione previsto è il " & _configurazioneServizio.Giorno_Settimana_Esecuzione.ToString & " e ora è " & DirectCast(CInt(dataCorrente.DayOfWeek), enum_Giorno_Settimana).ToString & "."
            Return 0 'salta il giro

        End If

        _sincronizzazioniEseguiteNellaFinestra += 1
        _dataInizioUltimaSincronizzazione = dataCorrente


        Dim selettore As New Selettore_Classe_Servizio

        Try
            LoggaOk("Avvio task", contatoreQuestoLoop)

            Dim resOk As Boolean = selettore.Avvia(_configurazioneServizio,
                                                   Messaggio_di_Ritorno_Opzionale,
                                                   _objParametriSuperServer, _objParametriServer, _objParametriUtenti)

            If resOk Then
                'completato con successo, Messaggio_di_Ritorno_Opzionale forse contiene un messaggio della funzione
                Return 1 'ok
            Else
                'completato con insuccesso, Messaggio_di_Ritorno_Opzionale forse contiene un messaggio della funzione
                Return -1
            End If

        Catch ex As Exception
            LoggaErr(ex, contatoreQuestoLoop)
            Messaggio_di_Ritorno_Opzionale = ex.Message
            Return -1 'errore
        End Try

        Messaggio_di_Ritorno_Opzionale = "Task_VerificaTempi_E_Esegui conclusa senza eseguire il task, non previsto"
        Return -1

    End Function



#End Region





#Region "Utility"

    Private Sub ControlloSuperuser(ByVal pivaSuperuser As String)
        'Leggo DatiSuperuser
        Dim utenti = New AgronicaCoreUtentiDAL.Utenti_Read()
        Dim dtSuperUser = utenti.Leggi_SuperUser(pivaSuperuser, _objParametriUtenti.StringaConnessione)
        'Deve essercene uno solo selezionato
        If dtSuperUser.Rows.Count <> 1 Then
            Throw New Exception("Ci sono troppi superuser per la piva " & pivaSuperuser)
        End If
        If pivaSuperuser <> dtSuperUser.Rows(0).Item("CodFisc") Then
            Throw New Exception("_PivaSuperuser <> dtSuperUser.Rows(0).Item('CodFisc') ")
        End If

    End Sub

    Private Sub Aggiorna_ConfigurazioneServizi()
        Leggi_ConfigurazioneServizi(_configurazioneServizio.PivaSuperuser,
                                    _configurazioneServizio.Id_Servizio,
                                    _configurazioneServizio.Tipo_Sincro,
                                    _configurazioneServizio.Id_Riga)
    End Sub

    Private Sub Leggi_ConfigurazioneServizi(ByVal pivaSuperuser As String,
                                            ByVal idServizio As enum_Id_Servizio,
                                            ByVal tipoSincro As enum_Tipi_Servizi_Background,
                                            ByVal idRiga As Integer)

        _configurazioneServizio = Configurazione_Servizi_R.LeggiSingolo(pivaSuperuser,
                                                                        idServizio,
                                                                        tipoSincro,
                                                                        idRiga,
                                                                        "",
                                                                        _objParametriSuperServer)

        Try
            If _configurazioneServizio Is Nothing Then
                Throw New Exception("Configurazione_Servizio Is Nothing, probabilmente è stata letta più di una riga o nessuna riga con i valori selezionati.")
            End If

            If _configurazioneServizio.PivaSuperuser <> pivaSuperuser Or
               _configurazioneServizio.Id_Servizio <> idServizio Or
               _configurazioneServizio.Tipo_Sincro <> tipoSincro Or
               _configurazioneServizio.Id_Riga <> idRiga Then
                Throw New Exception("Configurazione_Servizio.chiave <> _chiave lettura ")
            End If

            If _configurazioneServizio.PivaSuperuser = "" Or
               _configurazioneServizio.Id_Servizio = 0 Or
               _configurazioneServizio.Tipo_Sincro = 0 Or
               _configurazioneServizio.Id_Riga = 0 Then
                Throw New Exception("Configurazione_Servizio.chiave = '' o 0 ")
            End If
        Catch ex As Exception

            'Il servizio è nullo, non corretto, ne creo uno fittizio per fare i log etc ma imposto come disattivato

            'creo un servizio fittizio e lo disabilito
            _configurazioneServizio = New Configurazione_Servizio With {
                .PivaSuperuser = pivaSuperuser,
                .Id_Servizio = idServizio,
                .Tipo_Sincro = tipoSincro,
                .Id_Riga = idRiga,
                .Descrizione = ex.Message
            }
            _configurazioneServizio.Attivo = 0 'IMPORTANTISSIMO CHE SIA DISABILITATO

            'Non previsto questo valore
            LoggaErr("Configurazione_Servizio non creato correttamente, servizio impostato non attivo" & vbCrLf & "Messaggio: " & ex.Message, _contatore)
            'Return False
            SetStato(Utility.ID_eventi.Task_Terminato_Con_Errori)
            'disabilito su db attivo=0
            'Disabilita()

        End Try


    End Sub

    Private Sub SetStato(ByVal nuovoStato As Utility.ID_eventi)
        Configurazione_Servizi_R.SetStato(_configurazioneServizio.PivaSuperuser,
                                          _configurazioneServizio.Id_Servizio,
                                          _configurazioneServizio.Tipo_Sincro,
                                          _configurazioneServizio.Id_Riga,
                                          nuovoStato,
                                          _objParametriSuperServer)
    End Sub

    Private Function GetStato() As Utility.ID_eventi
        Return Configurazione_Servizi_R.GetStato(_configurazioneServizio.PivaSuperuser,
                                                 _configurazioneServizio.Id_Servizio,
                                                 _configurazioneServizio.Tipo_Sincro,
                                                 _configurazioneServizio.Id_Riga,
                                                 _objParametriSuperServer)
    End Function

    Private Sub Disabilita()
        Configurazione_Servizi_R.AbilitaDisabilita(_configurazioneServizio.PivaSuperuser,
                                                   _configurazioneServizio.Id_Servizio,
                                                   _configurazioneServizio.Tipo_Sincro,
                                                   _configurazioneServizio.Id_Riga,
                                                   False,
                                                   _objParametriSuperServer)
    End Sub

    Private Sub LoggaWarning(ByVal msg As String, ByVal numLoop As Integer)

        If Not (_configurazioneServizio.GSB_Livello_Log = enum_GSB_Livello_Log.Errori Or
            _configurazioneServizio.GSB_Livello_Log = enum_GSB_Livello_Log.NessunLog) Then

            Utility.ScriviLogServizio(Log, getTitolo(numLoop) & msg, EventLogEntryType.Warning)
        End If


    End Sub

    Private Sub LoggaOk(ByVal msg As String, ByVal numLoop As Integer)

        If _configurazioneServizio.GSB_Livello_Log = enum_GSB_Livello_Log.Tutto Then
            Utility.ScriviLogServizio(Log, GetTitolo(numLoop) & msg, EventLogEntryType.Information)
        End If


    End Sub

    Private Sub LoggaErr(ByVal ex As Exception, ByVal numLoop As Integer)
        If _configurazioneServizio.GSB_Livello_Log <> enum_GSB_Livello_Log.NessunLog Then
            Utility.ScriviLogServizio(Log, GetTitolo(numLoop) &
                    Utility.estraiMessaggiEccezioni(ex), EventLogEntryType.Error)
        End If

    End Sub

    Private Sub LoggaErr(ByVal msg As String, ByVal numLoop As Integer)
        If _configurazioneServizio.GSB_Livello_Log <> enum_GSB_Livello_Log.NessunLog Then
            Utility.ScriviLogServizio(Log, GetTitolo(numLoop) & msg, EventLogEntryType.Error)
        End If

    End Sub

    Private Function GetTitolo(ByVal numLoop As Integer) As String
        Dim titolo As String = ""
        titolo &= "Task:      " & _configurazioneServizio.Descrizione & vbCrLf
        titolo &= "ID:        " & ID & vbCrLf
        titolo &= "Loop:      " & numLoop & vbCrLf
        titolo &= "" & vbCrLf

        Return titolo

    End Function


#End Region

    Private Sub MandaEmail(msg As String)

        'Configurazione_Servizio.
        Dim oggetto As String = _configurazioneServizio.Descrizione
        Dim hostSmtp As String = _configurazioneServizio.Host_Smtp
        Dim mittente As String = _configurazioneServizio.Mittente

        If hostSmtp <> "" And mittente <> "" And Not IsNothing(_configurazioneServizio.Destinatari) Then

            Dim objMail As New Net.Mail.MailMessage With {
                .From = New Net.Mail.MailAddress(mittente)
            }

            Dim destinatari() As String = _configurazioneServizio.Destinatari.Split(";")
            If destinatari.Count > 0 Then
                For i = 0 To destinatari.Count - 1
                    Dim destinatario = destinatari(i).Trim
                    If destinatario <> "" AndAlso destinatario.Contains("@") Then
                        objMail.To.Add(New Net.Mail.MailAddress(destinatario))
                    End If
                Next
            End If

            If objMail.To.Count = 0 Then
                objMail.To.Add(New Net.Mail.MailAddress("programmatori@diagramgroup.it"))
                oggetto &= " Attenzione i destinatari non sono presenti"
            End If

            msg = Replace(msg, vbCrLf, "<br />")

            objMail.IsBodyHtml = True
            objMail.Subject = oggetto
            objMail.Body = msg

            Dim objSent As New Net.Mail.SmtpClient(hostSmtp)

            If IsNumeric(_configurazioneServizio.Host_Smtp_Porta) Then
                objSent.Port = _configurazioneServizio.Host_Smtp_Porta
            End If

            If _configurazioneServizio.smtp_user <> "" AndAlso _configurazioneServizio.smtp_password <> "" Then
                objSent.Credentials = New Net.NetworkCredential(_configurazioneServizio.smtp_user, _configurazioneServizio.smtp_password)
            End If

            If _configurazioneServizio.smtp_enablessl.ToLower = "true" OrElse _configurazioneServizio.smtp_enablessl.ToLower = "false" Then
                objSent.EnableSsl = _configurazioneServizio.smtp_enablessl
            End If

            objSent.Send(objMail)
        End If

    End Sub

End Class
