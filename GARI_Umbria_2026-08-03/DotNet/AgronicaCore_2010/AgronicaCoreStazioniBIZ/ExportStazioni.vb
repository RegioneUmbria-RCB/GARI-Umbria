Imports System.Net
Imports System.Text
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreMetaSchemaDAL
Imports AgronicaCoreUtility
Imports Newtonsoft.Json
Imports OutData.Infragri

Public Class ExportStazioni

    Private objParametri_Server As AgronicaCoreParametri
    Private objParametri_Utenti As AgronicaCoreParametri
    Private configServizio As AgronicaCoreVarieDAL.Configurazione_Servizio
    Private objLog As AgronicaCoreDataProvider.LogProvider

    Private Shared objSecurity As Sicurezza
    Private Shared configSiti_R As AgronicaCoreVarieDAL.Configurazione_Siti_R

    Sub New(ByVal _Configurazione_Servizio As AgronicaCoreVarieDAL.Configurazione_Servizio,
            ByVal objP_Utenti As AgronicaCoreParametri,
            ByVal objP_Server As AgronicaCoreParametri)

        objParametri_Server = objP_Server
        objParametri_Utenti = objP_Utenti
        configServizio = _Configurazione_Servizio

        objSecurity = New Sicurezza
        configSiti_R = New AgronicaCoreVarieDAL.Configurazione_Siti_R
        objLog = New AgronicaCoreDataProvider.LogProvider

    End Sub

    Public Class ParametriExtra
        Public PIVA As String()
        Public Class_Code As String()
        Public Esito As String()
        Public TipologiaAllegato As Integer
        Public EndpointToken As String
        Public grant_type As String
        Public client_id As String
        Public client_secret As String
        Public scope As String
        Public EndpointDispositivo As String
        Public EndpointFile As String
        Public TimeoutCallEndPoint As Integer

        Public IsEncrypted As Boolean

        Public Sub GetDecryptedClientIDClientSecret(ByRef objP_SuperServer As AgronicaCoreParametri)

            If IsEncrypted Then

                Try
                    Dim cr = configSiti_R.Leggi_Valore(0, "cr", "", "", objP_SuperServer)
                    client_id = objSecurity.DecryptString(client_id, cr)
                    client_secret = objSecurity.DecryptString(client_secret, cr)
                    IsEncrypted = False
                Catch ex As Exception
                    Throw New Exception("Errore decriptazione client_id API: " & ex.Message)
                End Try

            End If

        End Sub

    End Class

    Public Function Esporta(parametriExtra As String,
                            sistemaCod As enum_Esportazioni_Sistema_Cod,
                            sistemiEsterni As enum_SistemiEsterni,
                            ByRef Messaggio_di_Ritorno_Opzionale As String) As Boolean

        Dim endPointExport As New Util
        Dim result As Boolean = True
        Dim Parametri_Extra = JsonConvert.DeserializeObject(Of ParametriExtra)(parametriExtra)

        'Dim errorInExport As String = ""
        Dim errorInExport As New StringBuilder
        errorInExport.Length = 0

        Try

            Dim AgroDanagrafeLogDAL As New AgronicaLogAnagrafe_R
            Dim AgroMacchineBIZ As New AgronicaCoreContabBIZ.Parco_Macchine_R

            Dim condizioniAnag As New List(Of String)

            ' ---- condizione fissa NON CONSIDERIAMO LA CANCELLAZIONE ----
            condizioniAnag.Add("logAnagrafe.Tipo_Operazione <> " & enum_TipoOperazioneDB.Cancellazione & " ")

            ' ---- PIVA ----
            Dim pivaConditionAnag = BuildInCondition("logAnagrafe.Param1", Parametri_Extra.PIVA)
            If pivaConditionAnag <> "" Then
                condizioniAnag.Add(pivaConditionAnag)
            End If

            ' ---- ESITO ----
            Dim esitoConditionAnag = BuildInCondition("logInvioEsito.Esito", Parametri_Extra.Esito)
            If esitoConditionAnag <> "" Then
                condizioniAnag.Add(esitoConditionAnag)
            End If

            ' ---- filtro finale ----
            Dim filtroXlogAnagrafe As String = String.Join(" AND ", condizioniAnag)

            Dim orderBy As String = ""

            Dim dataTableResult As DataTable = AgroDanagrafeLogDAL.LeggiLogAnagrafeJoinInvio(0,
                                                                                             "",
                                                                                             "",
                                                                                             sistemaCod,
                                                                                             enum_TipoOperazioneDB.Lettura,
                                                                                             sistemiEsterni,
                                                                                             enum_TipoEntita_Des.ParcoMacchine,
                                                                                             "",
                                                                                             "",
                                                                                             "",
                                                                                             "",
                                                                                             "",
                                                                                             "",
                                                                                             "",
                                                                                             0,
                                                                                             filtroXlogAnagrafe,
                                                                                             orderBy,
                                                                                             objParametri_Server)

            Dim IDChiamata As Integer = -1
            Dim chiaveMacchina As String()
            Dim userNameModificaStazione As String

            For Each row As DataRow In dataTableResult.Rows

                Dim FlagTransazioneLocale As Boolean = False
                Dim FlagConnessioneLocale As Boolean = False

                Dim pacchettoDaInviare As New Dispositivo()

                Dim esitoCorrente = Util_Costanti.ESITO_OK
                Dim errorMessageCorrente = ""

                Dim TipoOperazione As enum_TipoOperazioneDB

                Try

                    'Apro la connessione al DB
                    ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, objParametri_Server)

                    If row("Esito") = Util_Costanti.ESITO_KO OrElse row("Esito") = Util_Costanti.ESITO_BLK Then
                        IDChiamata = row("ID_Chiamata")
                    Else
                        IDChiamata = -1
                    End If

                    chiaveMacchina = row("Chiave").ToString().Split("_"c)
                    TipoOperazione = row("Tipo_Operazione")

                    pacchettoDaInviare = MapMacchinaToStazione(chiaveMacchina(0), chiaveMacchina(1), chiaveMacchina(2), Parametri_Extra.Class_Code, configServizio.Destinatari, IDChiamata, TipoOperazione, sistemaCod, userNameModificaStazione, objParametri_Utenti, objParametri_Server)

                    If pacchettoDaInviare IsNot Nothing Then

                        'risparmiamoci la chiamata alla funzione, se queste non sono criptate. Lo stesso controllo viene fatto all'interno
                        If Parametri_Extra.IsEncrypted Then
                            Parametri_Extra.GetDecryptedClientIDClientSecret(objParametri_Server)
                        End If

                        Dim token As String = endPointExport.GetAccessToken(Parametri_Extra.EndpointToken,
                                                                               Parametri_Extra.grant_type,
                                                                               Parametri_Extra.client_id,
                                                                               Parametri_Extra.client_secret,
                                                                               Parametri_Extra.scope)

                        If Not endPointExport.CallEndpoint(token,
                                                            Parametri_Extra.EndpointDispositivo,
                                                            pacchettoDaInviare,
                                                            esitoCorrente,
                                                            errorMessageCorrente,
                                                            Parametri_Extra.TimeoutCallEndPoint) Then

                            If esitoCorrente = Util_Costanti.ESITO_BLK Then
                                SendMailToUpdateUser(My.Resources.AgronicaCoreStazioniBIZ.SubjectExportStazioneBLK,
                                                     errorMessageCorrente,
                                                     configServizio.Destinatari,
                                                     userNameModificaStazione)
                            End If

                        Else

                            SendMailToUpdateUser(My.Resources.AgronicaCoreStazioniBIZ.ExportStazioneSuccesso,
                                                 String.Format(My.Resources.AgronicaCoreStazioniBIZ.DetailsExportStazioneSuccess, pacchettoDaInviare.serialNumberDispositivo, pacchettoDaInviare.codiceModelloDispositivo, pacchettoDaInviare.codiceContratto),
                                                 configServizio.Destinatari,
                                                 userNameModificaStazione)
                        End If


                        Scrivi_Log_Invio_Chiamate_Dispositivo(IDChiamata,
                                                               pacchettoDaInviare,
                                                               row("Chiave"),
                                                               chiaveMacchina(0),
                                                               chiaveMacchina(1),
                                                               chiaveMacchina(2),
                                                               TipoOperazione,
                                                               esitoCorrente,
                                                               errorMessageCorrente,
                                                               Date.Now,
                                                               sistemaCod,
                                                               objParametri_Server)

                        If errorMessageCorrente <> "" Then
                            errorInExport.AppendLine("")
                            errorInExport.AppendLine(errorMessageCorrente)
                        End If

                    End If

                Catch ex As Exception

                    Dim errorMessage = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
                    'mandiamo mail 
                    errorInExport.AppendLine("")
                    errorInExport.AppendLine(ex.Message)

                Finally

                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    'Chiudo la transazione e la connessione al DB
                    ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)
                    ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

                End Try

            Next

            '*******************************************************
            '** REGIONE ALLEGATI
            '*******************************************************

            'qui mando eventualmente gli Allegati delle Stazioni

            Dim condizioniAllegato As New List(Of String)

            ' ---- CLASS_CODE ----
            If Parametri_Extra.Class_Code IsNot Nothing AndAlso Parametri_Extra.Class_Code.Any() Then
                Dim classConditions = Parametri_Extra.Class_Code.
                                      Select(Function(c) $"AnagMac.Class_Code LIKE '{c}'")

                condizioniAllegato.Add("( " & String.Join(" OR ", classConditions) & " )")
            End If

            ' ---- PIVA ----
            Dim pivaConditionAllegato = BuildInCondition("logAnagrafe.Piva", Parametri_Extra.PIVA)
            If pivaConditionAllegato <> "" Then
                condizioniAllegato.Add(pivaConditionAllegato)
            End If

            ' ---- ESITO ----
            Dim esitoConditionAllegato = BuildInCondition("LogEsitoAllegato.Esito", Parametri_Extra.Esito)
            If esitoConditionAllegato <> "" Then
                condizioniAllegato.Add(esitoConditionAllegato)
            End If

            ' ---- filtro finale ----
            Dim filtroXlogAllegati As String = ""
            If condizioniAllegato.Any() Then
                filtroXlogAllegati = String.Join(" AND ", condizioniAllegato)
            End If

            dataTableResult = AgroDanagrafeLogDAL.LeggiLogAllegatoInvio("",
                                                                        0,
                                                                        0,
                                                                        0,
                                                                        sistemaCod,
                                                                        Parametri_Extra.TipologiaAllegato,
                                                                        filtroXlogAllegati,
                                                                        orderBy,
                                                                        objParametri_Server)

            For Each row As DataRow In dataTableResult.Rows

                Dim FlagTransazioneLocale As Boolean = False
                Dim FlagConnessioneLocale As Boolean = False

                Try

                    'Apro la connessione al DB
                    ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, objParametri_Server)

                    Dim leggiFile As New AgronicaCoreScadenziario_BIZ.Allegati
                    'devo costruire il path dell'endpoint per esportare gli allegati
                    Dim endPointFile As String = $"{Parametri_Extra.EndpointFile}/{row("codiceContratto")}/{row("serialNumberDispositivo")}"

                    Dim esitoCorrente As String = Util_Costanti.ESITO_OK
                    Dim errorMessageCorrente As String = ""

                    If Parametri_Extra.IsEncrypted Then
                        Parametri_Extra.GetDecryptedClientIDClientSecret(objParametri_Server)
                    End If

                    Dim token As String = endPointExport.GetAccessToken(Parametri_Extra.EndpointToken,
                                                                        Parametri_Extra.grant_type,
                                                                        Parametri_Extra.client_id,
                                                                        Parametri_Extra.client_secret,
                                                                        Parametri_Extra.scope)

                    Dim files As DataTable = leggiFile.Leggi_File_Da_AllegatoCod(row("Allegati_Documenti_Piva"), row("Allegati_Documenti_Cod"), objParametri_Server)

                    For Each file As DataRow In files.Rows

                        Dim chiaveLogAllegati As String = file("Allegati_Documenti_SuperUser") & "_" & file("Allegati_Documenti_Piva") & "_" & file("Allegati_Documenti_Cod")

                        Dim fileObjectToLog As New AttachedFile(row("Allegati_Documenti_Cod"), file("Allegati_Documenti_NomeFile"), file("File_Allegato_DB"), row("codiceContratto"), row("serialNumberDispositivo"))

                        If endPointExport.CallEndPointAttachedFiles(token,
                                                                    row("serialNumberDispositivo"),
                                                                    row("codiceModelloDispositivo"),
                                                                    row("codiceContratto"),
                                                                    file("Allegati_Documenti_NomeFile"),
                                                                    file("File_Allegato_DB"),
                                                                    endPointFile,
                                                                    esitoCorrente,
                                                                    errorMessageCorrente) Then

                            SendMailToUpdateUser(My.Resources.AgronicaCoreStazioniBIZ.ExportAllegatoStazioneSuccesso,
                                                 String.Format(My.Resources.AgronicaCoreStazioniBIZ.DetailsExportAllegatoStazioneSuccess, row("serialNumberDispositivo"), row("codiceModelloDispositivo"), row("codiceContratto"), file("Allegati_Documenti_NomeFile")),
                                                 configServizio.Destinatari,
                                                 file("Username_Modifica"))

                        Else

                            If esitoCorrente = Util_Costanti.ESITO_BLK Then
                                SendMailToUpdateUser(My.Resources.AgronicaCoreStazioniBIZ.ExportAllegatoStazioneBLK,
                                                     errorMessageCorrente,
                                                     configServizio.Destinatari,
                                                     file("Username_Modifica"))
                            End If

                        End If

                        'occorre loggare il mancato invio dell'allegato, così si capisce cosa abbiamo mandato ed è andato storto
                        Scrivi_Log_Invio_Allegati(If(IsDBNull(row("IdInvioAllegato")), 0, row("IdInvioAllegato")),
                                                    fileObjectToLog,
                                                    chiaveLogAllegati,
                                                    row("Mac_Cod"),
                                                    If(IsDBNull(row("IdInvioAllegato")), enum_TipoOperazioneDB.Scrittura, enum_TipoOperazioneDB.Modifica),
                                                    esitoCorrente,
                                                    errorMessageCorrente,
                                                    Date.Now,
                                                    sistemaCod,
                                                    objParametri_Server)

                        If errorMessageCorrente <> "" Then
                            errorInExport.AppendLine("")
                            errorInExport.AppendLine(errorMessageCorrente)
                        End If

                    Next

                Catch ex As Exception

                    errorInExport.AppendLine("")
                    errorInExport.AppendLine(ex.Message)

                Finally

                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    'Chiudo la transazione e la connessione al DB
                    ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)
                    ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

                End Try

            Next

            If errorInExport.Length <> 0 Then
                SendMail(String.Format(My.Resources.AgronicaCoreStazioniBIZ.ErroreEsportazioneStazioniData, Now.ToShortDateString, Now.ToShortTimeString),
                         String.Format(My.Resources.AgronicaCoreStazioniBIZ.ElencoErrori, errorInExport.ToString),
                         configServizio.Destinatari)
            End If

        Catch ex As Exception

            result = False

            'String.Format(My.Resources.AgronicaCoreInfragriBIZ.ErroreHttpExport, resp.ResponseStatus, resp.StatusCode, url, resp.ErrorMessage, resp.Content)

            Messaggio_di_Ritorno_Opzionale &= String.Format(My.Resources.AgronicaCoreStazioniBIZ.ErroreEsportazioneStazioni, Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True, source:=True))
            endPointExport.Chiama_ScriviLOG(configServizio.Tipo_Sincro.ToString, Messaggio_di_Ritorno_Opzionale, configServizio.DirectoryLOG, configServizio.Tipo_Sincro.ToString & "_log.txt", objParametri_Server)

            'mandiamo mail 
            SendMail(My.Resources.AgronicaCoreStazioniBIZ.ErroreProceduraExportStazioni, Messaggio_di_Ritorno_Opzionale, configServizio.Destinatari)

        End Try

        Return result

    End Function

    Private Function BuildInCondition(columnName As String, values As IEnumerable(Of String)) As String
        If values Is Nothing OrElse Not values.Any() Then Return ""

        Dim formatted = String.Join(",",
        values.Select(Function(v)
                          Return If(String.IsNullOrEmpty(v), "''", "'" & v.Replace("'", "''") & "'")
                      End Function))

        Return $" ISNULL({columnName}, '') IN ({formatted}) "
    End Function

    Private Function MapMacchinaToStazione(ByVal Piva As String,
                                           ByVal SaCod As String,
                                           ByVal MacCod As Integer,
                                           ByVal Class_Code As String(),
                                           ByVal listaMailAnomalie As String,
                                           ByVal IdChiamata As Integer,
                                           ByVal TipoOperazione As enum_TipoOperazioneDB,
                                           ByVal SistemaCod As enum_Esportazioni_Sistema_Cod,
                                           ByRef UserNameModifica As String,
                                           ByRef objParametri_Utenti As AgronicaCoreParametri,
                                           ByRef objParametri_Server As AgronicaCoreParametri) As Dispositivo

        Dim objMacchineBIZ_R As New AgronicaCoreContabBIZ.Parco_Macchine_R
        Dim objMacchina As New AgronicaCoreModelsSTD.anagrafiche.ParcoMacchine
        Dim dispositivo As New Dispositivo
        Dim classCode As String = ""

        objMacchina = objMacchineBIZ_R.Leggi_Macchina(Piva,
                                                      MacCod,
                                                      objParametri_Server)

        UserNameModifica = objMacchina.Username_Modifica

        'generiamo il classCode
        classCode = objMacchina.tipo.codice

        If objMacchina.dettaglio_1.codice <> "" Then
            classCode = classCode & "." & objMacchina.dettaglio_1.codice
        End If

        If objMacchina.dettaglio_2.codice <> "" Then
            classCode = classCode & "." & objMacchina.dettaglio_2.codice
        End If

        If MatchesAny(classCode, Class_Code) Then

            dispositivo.codiceContratto = objMacchina.Contratto_Installazione
            dispositivo.codiceAzienda = objMacchina.contatto.primaryKey.codice
            dispositivo.codiceModelloDispositivo = objMacchina.Tipologia_Installazione
            dispositivo.noteInstallazione = objMacchina.Distinta_Installazione.Replace(vbCrLf, " ").Replace(vbCr, " ").Replace(vbLf, " ")

            'utilizziamo il DateTimeOffset, più preciso per il chiamante
            Dim dt As DateTime = objMacchina.Data_Inizio_Installazione

            Dim tz As TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time")

            Dim offset As TimeSpan = tz.GetUtcOffset(dt)

            dispositivo.dataOraInstallazione = New DateTimeOffset(dt, offset)

            dispositivo.latitudineInstallazione = objMacchina.Latitudine_Installazione
            dispositivo.longitudineInstallazione = objMacchina.Longitudine_Installazione
            dispositivo.serialNumberDispositivo = objMacchina.codice_stringa

            If objMacchina.Stato_Installazione = "IT" Then

                dispositivo.comuneInstallazione = objMacchina.Provincia_Istat_Installazione & objMacchina.Comune_Istat_Installazione

            Else
                'Dim nazioniDAL As New ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166_R
                'Dim nazione As DataTable
                'nazione = nazioniDAL.Leggi(objMacchina.Stato_Installazione, "", "", objParametri_Server)

                'If nazione.Rows.Count > 1 Then
                '    Throw New Exception
                'Else

                '    stazione.comuneInstallazione = nazione.Rows(0)("Descrizione").ToString() & " " & objMacchina.Indirizzo_Installazione

                'End If

                dispositivo.comuneInstallazione = objMacchina.Stato_Installazione

            End If

            'verifichiamo che abbia il contatto valorizzato, altrimenti mando mail di notifica
            If objMacchina.contatto.primaryKey.codice = "" Then

                SendMailToUpdateUser(String.Format(My.Resources.AgronicaCoreStazioniBIZ.SubjecStazioneSenzaContatto,
                                   objMacchina.codice_stringa,
                                   objMacchina.Tipologia_Installazione,
                                   objMacchina.Contratto_Installazione),
                                 My.Resources.AgronicaCoreStazioniBIZ.BodyMailStazioneSenzaContatto,
                                 listaMailAnomalie,
                                 UserNameModifica)

                Dim chiave = New List(Of String) From {Piva, SaCod, MacCod}

                Scrivi_Log_Invio_Chiamate_Dispositivo(IdChiamata,
                                                      dispositivo,
                                                      String.Join("_", chiave),
                                                      Piva,
                                                      SaCod,
                                                      MacCod,
                                                      TipoOperazione,
                                                      Util_Costanti.ESITO_BLK,
                                                      "",
                                                      Date.Now,
                                                      SistemaCod,
                                                      objParametri_Server)

                dispositivo = Nothing
                Return dispositivo

            End If

        Else

            dispositivo = Nothing

        End If

        Return dispositivo

    End Function

    Private Function MatchesAny(value As String, patterns() As String) As Boolean
        For Each p As String In patterns
            Dim patternFixed As String = p.Replace("%", "*")
            If value Like patternFixed Then
                Return True
            End If
        Next
        Return False
    End Function

    Private Shared Sub Scrivi_Log_Invio_Chiamate_Dispositivo(ID As Integer, pacchettoDaInviare As Dispositivo, Chiave As String, piva As String, sa_cod As String, mac_cod As Integer, TipoOperazione As enum_TipoOperazioneDB, Esito As String, Dati_Ricevuti As String, Data_Invio As DateTime, SistemaCod As enum_Esportazioni_Sistema_Cod, objParametri_Server As AgronicaCoreParametri)

        Dim objlog_invioChiamate As New AgronicaCoreVarieBIZ.Agronica_Log_Invio_Chiamate_W
        Dim DatiMacchina As String = ""

        If pacchettoDaInviare IsNot Nothing Then
            Dim pacchettoDaLoggare As String = ""
            pacchettoDaLoggare = JsonConvert.SerializeObject(pacchettoDaInviare)

            Dim tzh As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
            DatiMacchina = JsonConvert.SerializeObject(pacchettoDaLoggare, tzh)
        End If

        If ID = -1 Then
            objlog_invioChiamate.Scrivi_Log_Invio_Anagrafe(SistemaCod, DatiMacchina, enum_TipoEntita_Des.ParcoMacchine, Chiave, "", piva, sa_cod, 0, 0, 0, 0, 0, "", TipoOperazione, Esito, Dati_Ricevuti, objParametri_Server, Mac_Cod:=mac_cod, Data_Invio:=Data_Invio)
        Else
            objlog_invioChiamate.Update_Log_Invio_Chiamate(ID, SistemaCod, DatiMacchina, TipoOperazione, Esito, Dati_Ricevuti, objParametri_Server, Data_Invio)
        End If

    End Sub

    Private Shared Sub Scrivi_Log_Invio_Allegati(ID As Integer, fileObject As AttachedFile, Chiave As String, MacCod As Integer, TipoOperazione As enum_TipoOperazioneDB, Esito As String, Dati_Ricevuti As String, Data_Invio As DateTime, SistemaCod As enum_Esportazioni_Sistema_Cod, objParametri_Server As AgronicaCoreParametri)

        Dim objlog_invioChiamateBIZ As New AgronicaCoreVarieBIZ.Agronica_Log_Invio_Chiamate_W
        Dim file As String = ""

        If fileObject IsNot Nothing Then
            Dim pacchettoDaLoggare As String = ""
            pacchettoDaLoggare = JsonConvert.SerializeObject(fileObject)

            Dim tzh As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
            file = JsonConvert.SerializeObject(pacchettoDaLoggare, tzh)
        End If

        If ID = 0 Then
            objlog_invioChiamateBIZ.Scrivi_Log_Invio_Chiamate(SistemaCod, file, Data_Invio, "0", TipoOperazione, Esito, Dati_Ricevuti, objParametri_Server, Dettaglio1:=Chiave, Dettaglio2:=MacCod)
        Else
            objlog_invioChiamateBIZ.Update_Log_Invio_Chiamate(ID, SistemaCod, file, TipoOperazione, Esito, Dati_Ricevuti, objParametri_Server, Data_Invio)
        End If
    End Sub

    Private Shared Function Leggi_Log_Invio_Allegati(Chiave As String, SistemaCod As enum_Esportazioni_Sistema_Cod, objParametri_Server As AgronicaCoreParametri) As DataTable

        'Dim objlog_invioChiamate As New AgronicaCoreVarieBIZ.Agronica_Log_Invio_Chiamate_W
        Dim objlog_invioChiamate As New AgronicaCoreVarieDAL.Agronica_Log_Invio_Chiamate_R

        Return objlog_invioChiamate.Leggi(SistemaCod, "", 0, "", objParametri_Server, Dettaglio1:=Chiave)

    End Function

    Private Sub SendMailToUpdateUser(ByVal Oggetto As String,
                                     ByVal Message As String,
                                     ByVal listaMailAnomalie As String,
                                     ByVal usernameModifica As String
                                     )

        Dim objUtentiDettagliDAL As New AgronicaCoreUtentiBIZ.Utenti
        Dim utentiDettagli As DataTable = objUtentiDettagliDAL.LeggiUtentiDettagliDaCodFisc(usernameModifica, objParametri_Utenti)

        For Each row In utentiDettagli.Rows
            Dim mailUtenteModifica As String = row("Email")

            If Not String.IsNullOrWhiteSpace(mailUtenteModifica) Then
                If Not String.IsNullOrWhiteSpace(listaMailAnomalie) Then
                    listaMailAnomalie &= ", " & mailUtenteModifica.Trim()
                Else
                    listaMailAnomalie = mailUtenteModifica.Trim()
                End If
            End If

            SendMail(Oggetto,
                    Message,
                    listaMailAnomalie)
        Next

    End Sub

    Private Sub SendMail(ByVal Oggetto As String, ByVal Message As String, ByVal listaMail As String)
        'mandiamo mail

        Dim mailErr = (New Mail()).invia(objParametri_Server, configServizio.Mittente,
                                             listaMail, Nothing, Nothing,
                                             Oggetto,
                                             Message,
                                             False, Nothing)

        If Not String.IsNullOrEmpty(mailErr) Then
            Dim NomeRoutine As String = "AgronicaCoreStazioniBIZ.ExportStazioni.SendMail()"
            objLog.Scrivi_LOG(objParametri_Server, NomeRoutine, String.Format(My.Resources.AgronicaCoreStazioniBIZ.InvioMailFallito, mailErr))
        End If

    End Sub

End Class