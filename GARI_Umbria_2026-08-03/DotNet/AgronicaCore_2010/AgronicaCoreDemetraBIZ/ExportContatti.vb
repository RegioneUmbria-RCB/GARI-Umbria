Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports Newtonsoft.Json
Imports AgronicaCoreUtility
Imports AgronicaCoreDTOStd.InData.importazioni
Imports System.Net
Imports AgronicaCoreDTOStd.InData.Anagrafica
Imports AgronicaCoreDemetraBIZ.Util_Costanti
Imports AgronicaCoreAnagrafeDAL

Public Class ParametriExtra_Contatti_Export_Demetra
    Public DemetraBaseUrl As String
    Public apikey As String
    Public CUAA As String()

    Public FiltroEsportazione As Integer = -1
    Public Ambiente As String
    Public FiltroAggRapportiContab As String = ""

    Public UsaWS As Boolean = True 'Per Debug
    Public InviaPubblici As Boolean = True
End Class

Public Class ExportContatti

    Private _objParametri_Super_Server As AgronicaCoreParametri = Nothing
    Private _objParametri_Server As AgronicaCoreParametri = Nothing
    Private _objParametri_Utenti As AgronicaCoreParametri = Nothing

    Private _dataLoader As D2G_Contatto_Loader = Nothing
    Private _mapper As D2G_Contatti_Export_Mapper = Nothing
    Private _intercambio As D2G_Interscambio_Contatti = Nothing

    Private _piveAmmesse As List(Of String) = New List(Of String)
    Private _rapportiContabiliAmmessi As List(Of IDictionary(Of String, Object)) = Nothing
    Private _parametriExtra As ParametriExtra_Contatti_Export_Demetra = Nothing

    Private _configServizio As AgronicaCoreVarieDAL.Configurazione_Servizio = Nothing

    Public Sub New()

    End Sub

    Public Sub New(ByVal parametriExtra As String,
                   ByVal configServizio As AgronicaCoreVarieDAL.Configurazione_Servizio,
                   ByRef objParametri_Super_Server As AgronicaCoreParametri,
                   ByRef objParametri_Server As AgronicaCoreParametri,
                   ByRef objParametri_Utenti As AgronicaCoreParametri)

        _objParametri_Super_Server = objParametri_Super_Server
        _objParametri_Server = objParametri_Server
        _objParametri_Utenti = objParametri_Utenti

        _dataLoader = New D2G_Contatto_Loader(objParametri_Server, objParametri_Utenti)
        Inizializza(parametriExtra)

        _intercambio = New D2G_Interscambio_Contatti(objParametri_Server, objParametri_Utenti, _dataLoader)
        _mapper = New D2G_Contatti_Export_Mapper(objParametri_Server, objParametri_Utenti, _intercambio, _rapportiContabiliAmmessi)

        _configServizio = configServizio
    End Sub

    Private Sub Inizializza(ByVal parametriExtra As String)

        _parametriExtra = JsonConvert.DeserializeObject(Of ParametriExtra_Contatti_Export_Demetra)(parametriExtra)

        Dim xFiltroAggiuntivo = " terzista = 1 OR dipendente = 1 "
        If Not String.IsNullOrWhiteSpace(_parametriExtra.FiltroAggRapportiContab) Then
            xFiltroAggiuntivo = _parametriExtra.FiltroAggRapportiContab
        End If
        _rapportiContabiliAmmessi = _dataLoader.Leggi_Rapporti_Contabili_Ammessi(xFiltroAggiuntivo)

        If _parametriExtra.CUAA IsNot Nothing AndAlso _parametriExtra.CUAA.Length > 0 Then
            For Each cuaa As String In _parametriExtra.CUAA
                _piveAmmesse.Add(_dataLoader.Piva_From_CUAA(cuaa))
            Next
        End If

    End Sub


    Public Function Esporta(ByRef Messaggio_di_Ritorno_Opzionale As String) As Boolean

        Dim objDemetraBIZ_util As New Util

        Try

            Dim FlagConnessioneLocale As Boolean = True
            Dim FlagTransazioneLocale As Boolean = True

            Dim timeStamp As DateTime = DateTime.Now
            Dim contattiNonInviati As List(Of IDictionary(Of String, Object)) = _dataLoader.Contatti_NonInviati(_piveAmmesse)
            If IsNothing(contattiNonInviati) Then
                Return True
            End If

            Dim pacchettoDaInviare As ImportDemetra = Nothing

            ' Scartare i tipi operazione 3 (Cancellazione)

            Dim handleRisorseUmane As New Risorse_Umane_R()

            For Each cni As IDictionary(Of String, Object) In contattiNonInviati

                Dim contattoGias As IDictionary(Of String, Object) = Nothing

                Dim Piva As String = cni("piva")
                Dim Cod_Contatto As String = cni("cod_contatto")
                Dim Esito As String = cni("esito")
                Dim ultimaOperazione As enum_TipoOperazioneDB = cni("ultimaoperazione")

                Dim retrial As Boolean = False
                Dim onUpdate As Boolean = False
                Dim IDChiamata As Integer = -1
                Dim blocked As Boolean = False

                If Esito = Util_Costanti.ESITO_KO OrElse Esito = Util_Costanti.ESITO_BLK Then
                    retrial = True
                    IDChiamata = CInt(cni("id_chiamata"))
                Else
                    retrial = False
                    IDChiamata = -1
                End If

                Dim TipoOperazione As enum_TipoOperazioneDB
                Dim anagrafica As AnagraficaWrapper(Of AgronicaCoreDTOStd.InData.Demetra.Contatto) = Nothing

                Try

                    ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, _objParametri_Server)

                    ' creazione pacchetto da inviare con CUAA senza dati serializzati
                    Dim tzh As New JsonSerializerSettings With {.DateFormatString = "yyyy-MM-ddT00:00:00Z"}
                    pacchettoDaInviare = New ImportDemetra With {
                        .dati = Nothing,
                        .CUAA = _dataLoader.Leggi_CUAA(Piva)
                    }

                    If ultimaOperazione = enum_TipoOperazioneDB.Cancellazione Then
                        Dim msgBLK = String.Format("{0}Cancellazione Contatto non gestita in invio a Demetra", BLK_MESSAGE_PREFIX)
                        If retrial Then
                            Update_Log_Invio_Chiamate(IDChiamata, pacchettoDaInviare, TipoOperazione, ESITO_BLK, msgBLK, timeStamp)
                        Else
                            Chiama_Scrivi_Log_Invio_Contatti(pacchettoDaInviare, Piva, Cod_Contatto, anagrafica.codice_esterno, TipoOperazione, ESITO_BLK, msgBLK, timeStamp)
                        End If
                        Continue For
                    End If

                    contattoGias = _dataLoader.Carica_Contatto_Esistente(Piva, Cod_Contatto, True, True)

                    If Not IsNothing(contattoGias) Then

                        Dim contattoDemetra As AgronicaCoreDTOStd.InData.Demetra.Contatto = _mapper.Mappa(Piva, Cod_Contatto, contattoGias)

                        TipoOperazione = CInt(cni("ultimaoperazione"))
                        Select Case TipoOperazione
                            Case enum_TipoOperazioneDB.Modifica
                                onUpdate = True
                            Case enum_TipoOperazioneDB.Scrittura
                                onUpdate = False
                        End Select

                        anagrafica = _mapper.Mappa_Elemento_Anagrafico(Piva, Cod_Contatto, contattoDemetra)

                        Dim datoCompresso As String = AgroZip.CompressioneBase64(1, JsonConvert.SerializeObject(anagrafica, tzh))
                        pacchettoDaInviare.dati = datoCompresso

                        ' GESTIONE CASI DA NON INVIARE E DA MARCARE COME BLK    
                        '1) Nessun rapporto contabile inviato
                        If IsNothing(contattoDemetra.ruolo) OrElse contattoDemetra.ruolo.Count = 0 Then
                            blocked = True
                            Dim msgBLK = String.Format("{0}Nessun Ruolo Esportabile", BLK_MESSAGE_PREFIX)
                            If retrial Then
                                Update_Log_Invio_Chiamate(IDChiamata, pacchettoDaInviare, TipoOperazione, ESITO_BLK, msgBLK, timeStamp)
                            Else
                                Chiama_Scrivi_Log_Invio_Contatti(pacchettoDaInviare, Piva, Cod_Contatto, anagrafica.codice_esterno, TipoOperazione, ESITO_BLK, msgBLK, timeStamp)
                            End If
                        ElseIf _parametriExtra.InviaPubblici = False AndAlso contattoGias(FIELD_MAPPER.SA_COD) = PUBBLICO Then
                            '2) Ignoro l'invio dei lavoratori pubblici a meno che non siano impiegati in operazioni QdCA

                            Dim dtUsoQdca = handleRisorseUmane.VerificaUtilizzoContattoQdca(_objParametri_Server, String.Join(",", contattoDemetra.ruolo.Select(Function(elem) elem.codice)))

                            If dtUsoQdca.Rows.Count = 0 Then
                                blocked = True 'Imposto la variabile per il workflow, ma non li blocco effettivamente perché potrebbero essere utilizzati in futuro
                            End If

                        End If

                        If Not blocked Then

                            Dim errorMessage As String = ""
                            If _parametriExtra.UsaWS AndAlso Not objDemetraBIZ_util.CallEndpoint(_parametriExtra.apikey, _parametriExtra.DemetraBaseUrl, pacchettoDaInviare, errorMessage) Then
                                Throw New Exception(errorMessage)
                            End If

                            '------------------------------------
                            '   SCRITTURA LOG INVIO CONTATTI 'OK'
                            '------------------------------------

                            If retrial Then
                                Update_Log_Invio_Chiamate(IDChiamata, pacchettoDaInviare, TipoOperazione, ESITO_OK, "", timeStamp)
                            Else
                                Chiama_Scrivi_Log_Invio_Contatti(pacchettoDaInviare, Piva, Cod_Contatto, anagrafica.codice_esterno, TipoOperazione, ESITO_OK, "", timeStamp)
                            End If

                        End If

                    Else

                        Throw New Exception(String.Format("Errore nella lettura del contatto: Piva {0}, Cod_Contatto {1}, Ultima Operazione su Contatto {2}", Piva, Cod_Contatto, ultimaOperazione.ToString))

                    End If

                Catch ex As Exception

                    Dim errorMessage = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

                    '-----------------------------------
                    '   SCRITTURA LOG INVIO CONTATTI 'KO'
                    '-----------------------------------
                    If retrial Then
                        Update_Log_Invio_Chiamate(IDChiamata, pacchettoDaInviare, TipoOperazione, ESITO_KO, ex.Message, timeStamp)
                    Else
                        Chiama_Scrivi_Log_Invio_Contatti(pacchettoDaInviare, Piva, Cod_Contatto, If(IsNothing(anagrafica), "", anagrafica.codice_esterno), TipoOperazione, ESITO_KO, ex.Message, timeStamp)
                    End If

                Finally

                    ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, _objParametri_Server)
                    ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, _objParametri_Server)

                End Try

            Next

            If _parametriExtra.FiltroEsportazione <> Enum_FiltroEsportazione_to_ElasticSearch.Nessuno Then
                Dim objLogger As New ExtractLog
                Dim Tipo_Esportazione As New List(Of String) From {
                        enum_Esportazioni_Sistema_Cod.Demetra_Export_LavoratoriQDC
                    }
                objLogger.ExtractLog(_configServizio, Tipo_Esportazione, _parametriExtra.FiltroEsportazione, _parametriExtra.Ambiente, _objParametri_Server, Messaggio_di_Ritorno_Opzionale)
            End If

        Catch ex As Exception

            Messaggio_di_Ritorno_Opzionale &= " Errore durante l'esportazione dei lavoratori QdC: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True, source:=True)
            objDemetraBIZ_util.Chiama_ScriviLOG(_configServizio.Tipo_Sincro.ToString, Messaggio_di_Ritorno_Opzionale, _configServizio.DirectoryLOG, _configServizio.Tipo_Sincro.ToString & "_log.txt", _objParametri_Server)

        End Try

        Return True

    End Function

    Private Sub Update_Log_Invio_Chiamate(ID As Integer, pacchettoDaInviare As ImportDemetra, TipoOperazione As enum_TipoOperazioneDB, Esito As String, Dati_Ricevuti As String, Data_Invio As DateTime)

        Dim objLogInvioChiamate As New AgronicaCoreVarieBIZ.Agronica_Log_Invio_Chiamate_W

        Dim DatiContatto As String = String.Empty

        If Not IsNothing(pacchettoDaInviare) Then
            Dim pacchettoDaLoggare As New ImportDemetra
            pacchettoDaLoggare.CUAA = pacchettoDaInviare.CUAA

            Dim tzh As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
            If Not IsNothing(pacchettoDaInviare.dati) Then
                pacchettoDaLoggare.dati = AgroZip.DeCompressioneBase64(1, pacchettoDaInviare.dati)
            Else
                pacchettoDaLoggare.dati = String.Empty
            End If
            DatiContatto = JsonConvert.SerializeObject(pacchettoDaLoggare, tzh)

        End If

        objLogInvioChiamate.Update_Log_Invio_Chiamate(ID, enum_Esportazioni_Sistema_Cod.Demetra_Export_LavoratoriQDC, DatiContatto, TipoOperazione, Esito, Dati_Ricevuti, _objParametri_Server, Data_Invio)

    End Sub

    Private Sub Chiama_Scrivi_Log_Invio_Contatti(pacchettoDaInviare As ImportDemetra, Piva As String, Cod_Contatto As String, chiave_esterna As String, TipoOperazione As enum_TipoOperazioneDB, Esito As String, Dati_Ricevuti As String, Data_Invio As DateTime)

        Dim objlog_invioChiamate As New AgronicaCoreVarieBIZ.Agronica_Log_Invio_Chiamate_W

        Dim DatiContatto As String = String.Empty

        If Not IsNothing(pacchettoDaInviare) Then
            Dim pacchettoDaLoggare As New ImportDemetra
            pacchettoDaLoggare.CUAA = pacchettoDaInviare.CUAA

            Dim tzh As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
            If Not IsNothing(pacchettoDaInviare.dati) Then
                pacchettoDaLoggare.dati = AgroZip.DeCompressioneBase64(1, pacchettoDaInviare.dati)
            Else
                pacchettoDaLoggare.dati = String.Empty
            End If
            DatiContatto = JsonConvert.SerializeObject(pacchettoDaLoggare, tzh)

        End If

        objlog_invioChiamate.Scrivi_Log_Invio_Contatti(enum_Esportazioni_Sistema_Cod.Demetra_Export_LavoratoriQDC, DatiContatto, Piva, Cod_Contatto, chiave_esterna, TipoOperazione, Esito, Dati_Ricevuti, _objParametri_Server, UtilizzaTransazione:=False, Data_Invio)

    End Sub

End Class

Public Class D2G_Contatti_Export_Mapper

    Private _objParametri_Utente As AgronicaCoreParametri = Nothing
    Private _objParametri_Server As AgronicaCoreParametri = Nothing
    Private _interscambio As D2G_Interscambio_Contatti = Nothing

    Private _rapportiContabiliAmmessi As List(Of IDictionary(Of String, Object)) = Nothing

    Public Sub New(ByRef objParametri_Server As AgronicaCoreParametri,
                   ByRef objParametri_Utente As AgronicaCoreParametri,
                   ByVal interscambio As D2G_Interscambio_Contatti,
                   ByVal rapportiContabiliAmmessi As List(Of IDictionary(Of String, Object))
                   )

        _objParametri_Server = objParametri_Server
        _objParametri_Utente = objParametri_Utente
        _interscambio = interscambio
        _rapportiContabiliAmmessi = rapportiContabiliAmmessi

    End Sub

    Public Function Mappa(
                            ByVal Piva As String,
                            ByVal Cod_Contatto As String,
                            ByVal contattoGias As IDictionary(Of String, Object)) As AgronicaCoreDTOStd.InData.Demetra.Contatto

        Dim contattoDemetra As New AgronicaCoreDTOStd.InData.Demetra.Contatto

        ' Per prima cosa verifico se al contatto sono agganciati rapporti_contabili recepibili da Demetra
        Dim rapportiContabiliGias As List(Of IDictionary(Of String, Object)) = contattoGias("rapporti_contabili")
        If IsNothing(rapportiContabiliGias) OrElse rapportiContabiliGias.Count = 0 Then
            Return contattoDemetra
        End If

        ' ------------------- MAPPATURA DATI CONTATTO -------------------

        Dim id_cf As Integer = CInt(contattoGias(FIELD_MAPPER.ID_CF))
        Dim isEstero As Boolean = (id_cf = CONTATTO_ESTERO)
        If isEstero Then
            id_cf = PERSONA_GIURIDICA
        End If

        Dim nome As String = String.Empty
        Dim cognome As String = String.Empty
        Dim ragSoc As String = String.Empty
        Dim codiceFiscale As String = String.Empty
        Dim partitaIva As String = String.Empty

        contattoDemetra.codice_contatto = contattoGias(FIELD_MAPPER.COD_CONTATTO)
        contattoDemetra.Id_CF = id_cf
        Select Case id_cf
            Case PERSONA_FISICA
                nome = contattoGias(FIELD_MAPPER.NOME)
                cognome = contattoGias(FIELD_MAPPER.COGNOME)
                codiceFiscale = contattoGias(FIELD_MAPPER.CODICE_FISCALE)
            Case PERSONA_GIURIDICA
                ragSoc = contattoGias(FIELD_MAPPER.RAG_SOC)
                partitaIva = contattoGias(FIELD_MAPPER.CODICE_FISCALE)
        End Select
        contattoDemetra.nome = nome
        contattoDemetra.cognome = cognome
        contattoDemetra.ragione_Sociale = ragSoc
        contattoDemetra.Cod_Fisc = codiceFiscale
        contattoDemetra.partita_iva = partitaIva

        ' ------------------- MAPPATURA DATI RAPPORTI CONTABILI  -------------------
        For Each rcGias In rapportiContabiliGias

            Dim ruolo = New AgronicaCoreDTOStd.InData.Demetra.RapportoContabile
            Dim codRapporto As Integer = CInt(rcGias(FIELD_MAPPER.COD_RAPPORTO))

            If Rapporto_Contabile_Da_Inviare(codRapporto) Then

                Dim cod_RisUm As Integer = CInt(rcGias(FIELD_MAPPER.COD_RISUM))
                ruolo.codice = cod_RisUm.ToString
                Dim interscambioRuolo = _interscambio.Risorse_Umane_Leggi_Tabella_Interscambio_Chiave_GIAS(Piva, cod_RisUm)
                If Not IsNothing(interscambioRuolo) Then
                    ruolo.codice_ruolo_esterno = interscambioRuolo("codice_esterno").ToString
                End If
                ruolo.cod_rapporto = codRapporto.ToString
                ruolo.flag_cancellazione = False
                ruolo.data_ultima_modifica = CDate(rcGias(FIELD_MAPPER.DATA_MODIFICA))
                ruolo.rapporto_des = Ottieni_Descrizione_Ruolo(codRapporto)
                ruolo.utente_ultima_modifica = rcGias(FIELD_MAPPER.USERNAME_MODIFICA)
                ruolo.validita = New AgronicaCoreDTOStd.InData.Demetra.Validita With
                {
                    .inizio = CDate(rcGias(FIELD_MAPPER.VALIDITA_INIZIO)),
                    .fine = CDate(rcGias(FIELD_MAPPER.VALIDITA_FINE))
                }
                contattoDemetra.ruolo.Add(ruolo)
            End If

        Next

        ' ------------------- MAPPATURA ALLEGATI  -------------------
        Dim documenti As List(Of IDictionary(Of String, Object)) = contattoGias("documenti")
        If Not IsNothing(documenti) AndAlso documenti.Count > 0 Then

            For Each docGias In documenti

                Dim doc = New AgronicaCoreDTOStd.InData.Demetra.Documento
                Dim Id_Tipologia As Integer = CInt(docGias(FIELD_MAPPER.ID_TIPOLOGIA))

                ' Processo solo i patentini
                If Id_Tipologia = enum_ID_Area_Tipologia.Patentino_trattamenti Then

                    Dim Id_Elenco = CInt(docGias(FIELD_MAPPER.ID_ELENCO))
                    Dim codice As String = String.Format("{0}_{1}_{2}", Piva, Cod_Contatto, Id_Elenco.ToString)

                    doc.codice = codice
                    Dim interscambioDoc = _interscambio.Documenti_Leggi_Tabella_Interscambio_Chiave_GIAS(Piva, Cod_Contatto, Id_Elenco)
                    If Not IsNothing(interscambioDoc) Then
                        doc.codice_patentino_esterno = interscambioDoc("codice_esterno").ToString
                    End If

                    doc.Data_Rilascio = CDate(docGias(FIELD_MAPPER.VALIDAZIONE_DATA))
                    doc.Data_Scadenza = CDate(docGias(FIELD_MAPPER.DATA_SCADENZA))
                    doc.Numero = docGias(FIELD_MAPPER.ALLEGATI_DOCUMENTI_NUMERO)
                    doc.Descrizione = docGias(FIELD_MAPPER.DESCRIZIONE_SCADENZA)

                    contattoDemetra.documenti.Add(doc)

                End If

            Next

        End If

        Return contattoDemetra

    End Function

    Private Function Rapporto_Contabile_Da_Inviare(ByVal cod_Rapporto As Integer) As Boolean

        Dim rapContGias As IDictionary(Of String, Object) = _rapportiContabiliAmmessi.FirstOrDefault(Function(r) CInt(r("cod_rapporto")).Equals(cod_Rapporto))
        If IsNothing(rapContGias) Then
            Return False
        End If

        Return True

    End Function

    Public Function Mappa_Elemento_Anagrafico(
                                             ByVal Piva As String,
                                             ByVal Cod_Contatto As String,
                                             ByVal contattoDemetra As AgronicaCoreDTOStd.InData.Demetra.Contatto) As AnagraficaWrapper(Of AgronicaCoreDTOStd.InData.Demetra.Contatto)

        Dim codice As String = String.Format("{0}_{1}", Piva, Cod_Contatto)
        Dim codice_Esterno As String = String.Empty

        Dim interscambioContatti = _interscambio.Contatti_Leggi_Tabella_Interscambio_Chiave_GIAS(Piva, Cod_Contatto)
        If Not IsNothing(interscambioContatti) Then
            codice_Esterno = interscambioContatti("codice_esterno")
        End If

        Dim anagrafica As New AnagraficaWrapper(Of AgronicaCoreDTOStd.InData.Demetra.Contatto) With
           {
               .codice = codice,
               .codice_esterno = codice_Esterno,
               .elemento_anagrafico = contattoDemetra
           }

        Return anagrafica

    End Function


    Private Function Ottieni_Descrizione_Ruolo(ByVal cod_Rapporto As Integer) As String

        Dim rapContGias As IDictionary(Of String, Object) = _rapportiContabiliAmmessi.FirstOrDefault(Function(r) CInt(r("cod_rapporto")).Equals(cod_Rapporto))
        Dim desRapporto As String = If(IsNothing(rapContGias), String.Empty, rapContGias("rapporto_des"))
        Return desRapporto

    End Function

End Class


