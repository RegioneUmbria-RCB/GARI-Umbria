Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreModelsSTD
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.exceptions
Imports AgronicaCoreMapper
Imports AgronicaCoreUtility

''' <summary>
''' Elabora un DataTable di capi ParmaFrance già letto e validato strutturalmente,
''' applicando lookup BDN, controlli di visibilità e scrittura su DB (insert + update).
'''
''' Miglioramenti rispetto all'implementazione originale (import_ParmaFrance):
'''   - Singolo EF context per tutta la sessione di import (Using block).
'''   - Cache delle razze (razza FR→BDN→Agronica) per evitare query ripetute.
'''   - Cache del codice detentore (CUAA) per PIVA.
'''   - Pre-caricamento unico della tabella stati accrescimento con filtro in-memory.
'''   - Filtrone di visibilità caricato una sola volta per sessione (non per stalla).
'''   - Check esistenza animali via query batch invece di query per-riga.
'''   - Batch pre-load degli animali da aggiornare e delle stalle di svezzamento.
'''   - listaCapiAnimali correttamente resettata ad ogni data di ingresso (bug fix).
'''   - GEN_COD e SPE_COD letti dal record stalla (non più hardcoded a 1).
''' </summary>
Public Class ParmaFranceImporter

    Private ReadOnly _parametriServer As AgronicaCoreParametri
    Private ReadOnly _parametriUtenti As AgronicaCoreParametri
    Private ReadOnly _log As New AgronicaCoreDataProvider.LogProvider
    Private ReadOnly _stallaR As New AgronicaCoreAnagrafeDAL.Stalla_R
    Private ReadOnly _stallaRaggruppamentiR As New AgronicaCoreAnagrafeDAL.Stalla_Raggruppamenti_R
    Private ReadOnly _attivitaZooToAgenda As New AttivitaZootecnicaToAgenda

    ' Cache sessione: popolate al primo accesso e riusate su tutti i file elaborati
    Private ReadOnly _razzaCache As New Dictionary(Of Integer, Integer)()
    Private ReadOnly _detentoreCache As New Dictionary(Of String, String)()
    Private ReadOnly _statiTemplateCache As New Dictionary(Of String, List(Of AgronicaCoreEntityFramework_POCO.Zoo_Animali_Lista_Stati_Accrescimento))()
    Private ReadOnly _filtrone As Lazy(Of DataTable)
    Private ReadOnly _efConnString As String

    Public Sub New(parametriServer As AgronicaCoreParametri, parametriUtenti As AgronicaCoreParametri)
        _parametriServer = parametriServer
        _parametriUtenti = parametriUtenti
        _filtrone = New Lazy(Of DataTable)(Function() CaricaFiltrone())
        _efConnString = New Gias_EF_Utility().GetEntityConnectionString(parametriServer.StringaConnessione)
    End Sub

    ''' <summary>
    ''' Pre-carica il filtrone prima che vengano aperte connessioni/DataReader dall'EF context.
    ''' Va chiamato subito dopo la costruzione, fuori dal loop dei file.
    ''' </summary>
    Public Sub PreWarmFiltrone()
        Dim dtIgnored As DataTable = _filtrone.Value
    End Sub

    ''' <summary>
    ''' Importa i capi contenuti nel DataTable già letto dal file Excel.
    ''' </summary>
    Public Function ImportaCapi(PivaSelezionata As String,
                                dtCapiTotali As DataTable,
                                LogDirectory As String,
                                LogFileName As String,
                                ByRef Messaggio As String) As Boolean
        Dim customLOGParams As New CustomLOGParams With {
            .LogDescrizioneUtente = _parametriServer.LogDescrizioneUtente,
            .LogDirectory = LogDirectory,
            .LogFileName = LogFileName
        }

        Try
            _log.Scrivi_LOG(_parametriServer, System.Reflection.MethodBase.GetCurrentMethod().Name,
                            "Inizio importazione", CustomLOGParams:=customLOGParams)

            ' 1. Validazione strutturale di tutte le righe e raccolta stalles distinte
            Dim listaStalle As New List(Of String)
            For Each row As DataRow In dtCapiTotali.Rows
                If Not IsDate(row("F4")) Then
                    Throw New Exception("Data di nascita non valida per il capo " & row("F1") & " — valore nel file: """ & row("F4") & """")
                End If
                If Not IsDate(row("F9")) Then
                    Throw New Exception("Data di partenza non valida per il capo " & row("F1") & " — valore nel file: """ & row("F9") & """")
                End If
                If Not IsDate(row("F10")) Then
                    Throw New Exception("Data di arrivo non valida per il capo " & row("F1") & " — valore nel file: """ & row("F10") & """")
                End If
                If row("F3") <> "M" And row("F3") <> "F" Then
                    Throw New Exception("Sesso non riconosciuto per il capo " & row("F1") & " — valore nel file: """ & row("F3") & """ (attesi: M o F)")
                End If
                If Not listaStalle.Contains(row("F19")) Then
                    listaStalle.Add(row("F19"))
                End If
            Next

            ' 2. Carica filtrone di visibilità (dalla cache di sessione: viene costruito una sola volta)
            Dim dtFiltrone As DataTable = _filtrone.Value

            Using giasCtx As New Gias_DeveloperServer_Entities(_efConnString)

                ' 3. Pre-carica l'intera tabella stati accrescimento una volta sola
                Dim tuttiStati = giasCtx.Zoo_Animali_Lista_Stati_Accrescimento.ToList()

                ' 4. Elaborazione per stalla
                For Each staDes In listaStalle
                    Try
                        Messaggio &= " <h5>Stalla " & staDes & " </h5>"
                        Dim dtCapiStalla As DataTable = dtCapiTotali.Select(" F19 = '" & staDes & "' ").CopyToDataTable()

                        Dim piva As String = ""
                        Dim saCod As Integer = 0
                        Dim staNum As Integer = 0
                        Dim raggruppamentoCod As Integer = 0
                        Dim genCod As Integer = 0
                        Dim speCod As Integer = 0
                        RisolveDatiStalla(PivaSelezionata, dtCapiStalla, piva, saCod, staNum, raggruppamentoCod, genCod, speCod)

                        If Not VerificaVisibilita(dtFiltrone, piva, saCod) Then
                            Messaggio &= "Non è possibile sincronizzare la stalla perché non si dispone dei permessi di visibilità necessari"
                            Continue For
                        End If

                        ' 5. Assegna Matricola e Stalla_Nascita a tutte le righe
                        For Each row In dtCapiStalla.Rows
                            If IsDBNull(row("F1")) Then Continue For
                            row("Matricola") = CStr(row("F1")).Trim()
                            row("Stalla_Nascita") = row("F2")
                        Next

                        ' 5b. Carica dizionario stalle svezzamento una sola volta per stalla (riusato da CaricaCapi e AggiornaCapi)
                        Dim svezzamentoDictStalla As Dictionary(Of String, String) = CaricaDizionarioSvezzamento(dtCapiStalla, giasCtx)

                        ' 6. Check esistenza animali con query batch (una sola round-trip)
                        Dim allMatricole = dtCapiStalla.Rows.Cast(Of DataRow).
                            Where(Function(r) Not IsDBNull(r("F1"))).
                            Select(Function(r) CStr(r("Matricola"))).
                            Distinct().ToList()

                        Dim matricoleEsistenti As New HashSet(Of String)(
                            giasCtx.Zoo_Animali.
                            Where(Function(z) allMatricole.Contains(z.Matricola) AndAlso z.PIVA = piva).
                            Select(Function(z) z.Matricola).ToList())

                        Dim dtCapiNew As DataTable = dtCapiStalla.Clone()
                        Dim dtCapiUpdate As DataTable = dtCapiStalla.Clone()

                        For Each row As DataRow In dtCapiStalla.Rows
                            If IsDBNull(row("F1")) Then Continue For
                            If matricoleEsistenti.Contains(CStr(row("Matricola"))) Then
                                dtCapiUpdate.ImportRow(row)
                            Else
                                dtCapiNew.ImportRow(row)
                            End If
                        Next

                        If dtCapiNew.Rows.Count > 0 Then
                            Messaggio &= " <p>Capi Inseriti:</p>"
                            CaricaCapi(dtCapiNew, saCod, staNum, raggruppamentoCod, piva,
                                       genCod, speCod, Messaggio, giasCtx, tuttiStati, svezzamentoDictStalla, customLOGParams)
                        End If

                        If dtCapiUpdate.Rows.Count > 0 Then
                            Messaggio &= " <p>Capi Aggiornati:</p>"
                            AggiornaCapi(dtCapiUpdate, saCod, staNum, raggruppamentoCod, piva,
                                         Messaggio, giasCtx, svezzamentoDictStalla, customLOGParams)
                        End If

                    Catch ex As Exception
                        Dim stackFlat As String = If(ex.StackTrace IsNot Nothing, ex.StackTrace.Replace(vbCrLf, " | ").Replace(vbLf, " | "), "(no stack)")
                        _log.Scrivi_LOG(_parametriServer, System.Reflection.MethodBase.GetCurrentMethod().Name,
                                        "Errore elaborazione stalla " & staDes & ": " & ex.Message & " [STACK: " & stackFlat & "]", CustomLOGParams:=customLOGParams)
                        Messaggio &= "<p>Si è verificato un errore durante l'elaborazione della stalla " & staDes & ": " & ex.Message & "</p>"
                    End Try
                Next

            End Using

            _log.Scrivi_LOG(_parametriServer, System.Reflection.MethodBase.GetCurrentMethod().Name,
                            "Fine importazione", CustomLOGParams:=customLOGParams)
            Return True

        Catch ex As Exception
            Dim stackFlat As String = If(ex.StackTrace IsNot Nothing, ex.StackTrace.Replace(vbCrLf, " | ").Replace(vbLf, " | "), "(no stack)")
            _log.Scrivi_LOG(_parametriServer, System.Reflection.MethodBase.GetCurrentMethod().Name,
                            "Errore scrittura dati: " & ex.Message & " [STACK: " & stackFlat & "]", CustomLOGParams:=customLOGParams)
            Messaggio = "Si è verificato un errore imprevisto durante l'importazione. Consultare il log per i dettagli."
            Return False
        End Try
    End Function

    ' ─────────────────────────────────────────────────────────────
    '  Stalla / raggruppamento / visibilità
    ' ─────────────────────────────────────────────────────────────

    ''' <summary>
    ''' Risolve piva, saCod, staNum, raggruppamentoCod, genCod, speCod
    ''' partendo dal codice BDN stalla (F19) presente nel DataTable capi.
    ''' GEN_COD e SPE_COD vengono letti direttamente dal record Stalla
    ''' (non più hardcoded), preparando il terreno per futuri import multi-specie.
    ''' </summary>
    Private Sub RisolveDatiStalla(PivaSelezionata As String,
                                   dtCapi As DataTable,
                                   ByRef piva As String,
                                   ByRef saCod As Integer,
                                   ByRef staNum As Integer,
                                   ByRef raggruppamentoCod As Integer,
                                   ByRef genCod As Integer,
                                   ByRef speCod As Integer)
        Dim row As DataRow = dtCapi.Select("").FirstOrDefault()
        Dim BDN_Codice_Azienda As String = row("F19")

        Dim dtStalla As DataTable = _stallaR.Leggi("", 0, 0,
                                                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                    "Stalla.BDN_Codice_Azienda = '" & BDN_Codice_Azienda & "'",
                                                    "", _parametriServer)

        If IsNothing(dtStalla) OrElse dtStalla.Rows.Count = 0 Then
            Throw New GiasException("Stalla " & BDN_Codice_Azienda & " non presente in GIAS")
        ElseIf dtStalla.Rows.Count > 1 Then
            Dim drStallaF = dtStalla.Select(" Piva = '" & PivaSelezionata & "' ")
            If drStallaF.Length <> 1 Then
                Throw New GiasException("Al codice Azienda " & BDN_Codice_Azienda & " corrispondono più stalle su GIAS")
            End If
            dtStalla = drStallaF.CopyToDataTable()
        End If

        Dim drStalla As DataRow = dtStalla.Select("").FirstOrDefault()
        piva = drStalla("PIVA")
        saCod = drStalla("sa_cod")
        staNum = drStalla("STA_NUM")
        genCod = drStalla("GEN_COD")
        speCod = drStalla("SPE_COD")

        Dim dtRaggruppamenti As DataTable = _stallaRaggruppamentiR.Leggi_x_anagrafica(
            _parametriServer.PivaSuperUser, piva, saCod, staNum, raggruppamentoCod,
            " Stalla_Raggruppamenti.flag_bdn = 1 ", "", _parametriServer)

        If IsNothing(dtRaggruppamenti) OrElse dtRaggruppamenti.Rows.Count = 0 Then
            Throw New GiasException("Stalla " & BDN_Codice_Azienda & " senza un raggruppamento correttamente configurato per l'importazione")
        End If

        raggruppamentoCod = dtRaggruppamenti.Select("").FirstOrDefault()("raggruppamento_cod")
    End Sub

    Private Function CaricaFiltrone() As DataTable
        Dim classFiltrone As New AgronicaCoreUtility.Filtrone
        Dim classJoin As New JoinFiltrone
        classJoin.bCentriAziendali = True
        classJoin.bGerarchiaImprese = True
        Return classFiltrone.CreaDTFiltrone(_parametriServer, "",
                                            enum_TipoSelect_FiltroneSuperNova.CentriAziendali_Visibilita_Appoggio,
                                            "", classJoin)
    End Function

    Private Function VerificaVisibilita(dtFiltrone As DataTable, piva As String, saCod As Integer) As Boolean
        If IsNothing(dtFiltrone) OrElse dtFiltrone.Rows.Count = 0 Then Return False
        Dim listaImprese = dtFiltrone.ToExpandoObject().ToList()
        Dim listaAziende As List(Of String) = listaImprese.Select(Of String)(Function(r) r("PIVA")).ToList()
        If Not listaAziende.Contains(piva) Then Return False
        Dim listaCentri As List(Of Integer) =
            listaImprese.Where(Function(r) r("PIVA") = piva).Select(Of Integer)(Function(r) r("sa_cod")).ToList()
        Return listaCentri.Contains(saCod)
    End Function

    ' ─────────────────────────────────────────────────────────────
    '  Inserimento nuovi capi
    ' ─────────────────────────────────────────────────────────────

    Private Sub CaricaCapi(dtCapiNew As DataTable,
                            saCod As Integer,
                            staNum As Integer,
                            raggruppamentoCod As Integer,
                            piva As String,
                            genCod As Integer,
                            speCod As Integer,
                            ByRef Messaggio As String,
                            giasCtx As Gias_DeveloperServer_Entities,
                            tuttiStati As List(Of AgronicaCoreEntityFramework_POCO.Zoo_Animali_Lista_Stati_Accrescimento),
                            svezzamentoDict As Dictionary(Of String, String),
                            customLOGParams As CustomLOGParams)

        Dim codDetentore As String = GetCodDetentore(piva)
        ' Escludi righe con F10 (data arrivo) DBNull: ClosedXML restituisce DBNull per celle vuote.
        Dim listaIngressi = dtCapiNew.Rows.Cast(Of DataRow)().
            Where(Function(r) Not IsDBNull(r("F10"))).
            Select(Function(r) r("F10")).
            Distinct().ToList()

        ' svezzamentoDict pre-caricato dal chiamante (una volta per stalla)

        Dim objAttivita As New attivita.Attivita
        objAttivita.fine = AGRODATAFINE
        objAttivita.job = New attivita.Zootecnia(LAVCOD_ACQUISTO_ANIMALI, "")
        objAttivita.centroAziendale = New anagrafiche.CentroAziendale With {
            .primaryKey = New anagrafiche.CentroAziendale.PK(saCod, piva)
        }
        objAttivita.fabbricatoCod = staNum

        For Each ingresso In listaIngressi
            ' FIX: lista resettata ad ogni data ingresso (nel codice originale non lo era,
            '      causando accumulo e ri-inserimento degli animali delle date precedenti)
            Dim listaCapiAnimali As New List(Of attivita.centri_di_costo.CentroDiCosto)
            Try
                Dim capiFiltrati = dtCapiNew.Rows.Cast(Of DataRow)().
                    Where(Function(x) Not IsDBNull(x("F10")) AndAlso Object.Equals(x("F10"), ingresso)).
                    ToList()
                If capiFiltrati.Count = 0 Then Continue For

                _log.Scrivi_LOG(_parametriServer, System.Reflection.MethodBase.GetCurrentMethod().Name,
                                "Inizio carico " & capiFiltrati.Count & " capi in data " & CDate(ingresso).ToShortDateString(),
                                CustomLOGParams:=customLOGParams)

                For Each capo In capiFiltrati
                    Dim codiceCapo As String = capo("Matricola")
                    Try
                        Dim codAziendaNascita As String = capo("Stalla_Nascita")
                        Dim sesso As String = capo("F3")
                        Dim dataNascita As String = capo("F4")

                        ' Se RazzaRisolta è presente (da risoluzione BDN) usa quella;
                        ' altrimenti (valore numerico diretto o risoluzione razza non abilitata) usa la procedura legacy.
                        Dim codRazza As Integer
                        If dtCapiNew.Columns.Contains("RazzaRisolta") AndAlso Not IsDBNull(capo("RazzaRisolta")) Then
                            codRazza = CInt(capo("RazzaRisolta"))
                        Else
                            codRazza = RicavaRazzaCapoAnimale(CInt(capo("F5")))
                        End If

                        ' F6/F7 (razza padre/madre) sono opzionali per suini: se assenti o non numerici usa 0.
                        Dim rawF6 As String = If(IsDBNull(capo("F6")), "", CStr(capo("F6")).Trim())
                        Dim codRazzaPadre As Integer = If(IsNumeric(rawF6) AndAlso rawF6 <> "",
                                                          RicavaRazzaCapoAnimale(CInt(rawF6)), 0)
                        Dim rawF7 As String = If(IsDBNull(capo("F7")), "", CStr(capo("F7")).Trim())
                        Dim codRazzaMadre As Integer = If(IsNumeric(rawF7) AndAlso rawF7 <> "",
                                                          RicavaRazzaCapoAnimale(CInt(rawF7)), 0)
                        Dim numCertificato As String = capo("F14")

                        Dim matricolaMadre As String = ""
                        If Not IsDBNull(capo("F20")) Then
                            matricolaMadre = CStr(capo("F20"))
                        End If

                        If IsDate(dataNascita) AndAlso CDate(dataNascita) > CDate(ingresso) Then
                            Throw New Exception("La data di nascita (" & CDate(dataNascita).ToShortDateString() & ") è successiva alla data di arrivo (" & CDate(ingresso).ToShortDateString() & ")")
                        End If

                        Dim codContatto_StallaSvezz As String = ""
                        If Not IsDBNull(capo("Stalla_Svezzamento")) Then
                            Dim nomeSvezz As String = CStr(capo("Stalla_Svezzamento")).Trim()
                            If nomeSvezz <> "" Then
                                codContatto_StallaSvezz = RisolveStallasvezzamento(nomeSvezz, codiceCapo, svezzamentoDict)
                            End If
                        End If

                        Dim objAnimale = CreaCapoAnimale(piva, saCod, codiceCapo, dataNascita, sesso,
                                                         codRazza, ingresso, numCertificato,
                                                         raggruppamentoCod, genCod, speCod,
                                                         codRazzaMadre, codRazzaPadre, codAziendaNascita,
                                                         matricolaMadre, codContatto_StallaSvezz,
                                                         codDetentore, tuttiStati)
                        listaCapiAnimali.Add(objAnimale)

                        ' Traccia audit per riga
                        Dim auditStatus = If(dtCapiNew.Columns.Contains("RazzaUsedDefault") AndAlso
                                             Not IsDBNull(capo("RazzaUsedDefault")) AndAlso
                                             CBool(capo("RazzaUsedDefault")),
                                            "SUCCESS_WITH_WARNING", "SUCCESS")
                        _log.Scrivi_LOG(_parametriServer, System.Reflection.MethodBase.GetCurrentMethod().Name,
                                        FormattaAuditLog(capo, auditStatus),
                                        CustomLOGParams:=customLOGParams)

                    Catch ex As Exception
                        Messaggio &= "<p> Errore matricola " & codiceCapo & ": " & ex.Message & "</p>"
                        ' Traccia audit errore per riga
                        _log.Scrivi_LOG(_parametriServer, System.Reflection.MethodBase.GetCurrentMethod().Name,
                                        FormattaAuditLog(capo, "ERROR", ex.Message),
                                        CustomLOGParams:=customLOGParams)
                    End Try
                Next

                objAttivita.inizio = CDate(ingresso)
                objAttivita.centriDiCosto = listaCapiAnimali

                If listaCapiAnimali.Count > 0 Then
                    Dim idAgenda As Integer = _attivitaZooToAgenda.ScriviAttivitaZootecnicaToAgenda(objAttivita, _parametriServer)
                    BloccaOperazione(piva, idAgenda)
                End If

                Dim lstMatricole = (From a As attivita.centri_di_costo.CapoAnimaleCDC In listaCapiAnimali
                                    Select a.capoAnimale.matricola).ToList()
                Messaggio &= "<ul><li>" & String.Join("</li><li>", lstMatricole) & "</li></ul>"

                _log.Scrivi_LOG(_parametriServer, System.Reflection.MethodBase.GetCurrentMethod().Name,
                                "Fine carico " & capiFiltrati.Count & " capi in data " &
                                CDate(ingresso).ToShortDateString() & ": " & String.Join(",", lstMatricole),
                                CustomLOGParams:=customLOGParams)

            Catch ex As Exception
                Dim msgEx = If(ex.InnerException IsNot Nothing,
                               ex.Message & " — " & ex.InnerException.Message,
                               ex.Message)
                Messaggio &= "<p>Errore nell'importazione del " & CDate(ingresso).ToShortDateString() & ": " & msgEx & "</p>"
                _log.Scrivi_LOG(_parametriServer, System.Reflection.MethodBase.GetCurrentMethod().Name,
                                "Errore carico in data " & CDate(ingresso).ToShortDateString() &
                                vbCrLf & " Errore:" & msgEx,
                                CustomLOGParams:=customLOGParams)
            End Try
        Next
    End Sub

    Private Sub BloccaOperazione(piva As String, idAgenda As Integer)
        Dim agendaDal As New AgronicaCoreContabDAL.Agenda_W
        agendaDal.ModificaPuntuale(piva, 0, idAgenda, _parametriServer, Tipo_Accettazione:=1)
    End Sub

    ' ─────────────────────────────────────────────────────────────
    '  Aggiornamento capi esistenti
    ' ─────────────────────────────────────────────────────────────

    Private Sub AggiornaCapi(DTCapiDaAggiornare As DataTable,
                              saCod As Integer,
                              staNum As Integer,
                              raggruppamentoCod As Integer,
                              piva As String,
                              ByRef Messaggio As String,
                              giasCtx As Gias_DeveloperServer_Entities,
                              svezzamentoDict As Dictionary(Of String, String),
                              customLOGParams As CustomLOGParams)
        Try
            ' Batch pre-load degli animali da aggiornare (una sola query)
            Dim matricoleDaAggiornare = DTCapiDaAggiornare.Rows.Cast(Of DataRow).
                Select(Function(r) CStr(r("Matricola"))).Distinct().ToList()
            Dim capiDict = giasCtx.Zoo_Animali.
                Where(Function(z) matricoleDaAggiornare.Contains(z.Matricola) AndAlso z.PIVA = piva).
                ToList().ToDictionary(Function(z) z.Matricola)

            ' Batch pre-load stalle svezzamento (pre-caricato dal chiamante, una volta per stalla)

            Dim listMatricole As New List(Of String)

            For Each capoUpd As DataRow In DTCapiDaAggiornare.Rows
                Dim matricolaCapo As String = capoUpd("Matricola")
                listMatricole.Add(matricolaCapo)

                If Not capiDict.ContainsKey(matricolaCapo) Then Continue For
                Dim capo = capiDict(matricolaCapo)

                If Not IsDBNull(capoUpd("F20")) Then
                    capo.MAT_MADRE = CStr(capoUpd("F20"))
                End If

                If Not IsDBNull(capoUpd("Stalla_Svezzamento")) AndAlso CStr(capoUpd("Stalla_Svezzamento")).Trim() <> "" Then
                    Dim nomeSvezz As String = CStr(capoUpd("Stalla_Svezzamento")).Trim()
                    capo.Stalla_Svezzamento = RisolveStallasvezzamento(nomeSvezz, matricolaCapo, svezzamentoDict)
                End If

                capo.Certificato = CStr(capoUpd("F14"))
            Next

            Messaggio &= "<ul><li>" & String.Join("</li><li>", listMatricole) & "</li></ul>"

            giasCtx.SaveChanges()
            giasCtx.Core.AcceptAllChanges()

            _log.Scrivi_LOG(_parametriServer, System.Reflection.MethodBase.GetCurrentMethod().Name,
                            "Fine AggiornaCapi " & DTCapiDaAggiornare.Rows.Count,
                            CustomLOGParams:=customLOGParams)

        Catch ex As Exception
            Dim msgEx = If(ex.InnerException IsNot Nothing,
                           ex.Message & " — " & ex.InnerException.Message,
                           ex.Message)
            Messaggio &= "<p>Errore durante l'aggiornamento dei capi: " & msgEx & "</p>"
            _log.Scrivi_LOG(_parametriServer, System.Reflection.MethodBase.GetCurrentMethod().Name,
                            "Errore in AggiornaCapi: " & msgEx,
                            CustomLOGParams:=customLOGParams)
        End Try
    End Sub

    ' ─────────────────────────────────────────────────────────────
    '  Costruzione CapoAnimale
    ' ─────────────────────────────────────────────────────────────

    Private Function CreaCapoAnimale(piva As String,
                                     sa_cod As Integer,
                                     matricolaCapo As String,
                                     dataNascita As String,
                                     sesso As String,
                                     codRazza As Integer,
                                     ingresso As String,
                                     numCertificato As String,
                                     raggruppamentoCod As Integer,
                                     genCod As Integer,
                                     speCod As Integer,
                                     codRazzaMadre As Integer,
                                     codRazzaPadre As Integer,
                                     codAziendaNascita As String,
                                     matricolaMadre As String,
                                     stallaSvezzamento As String,
                                     codDetentore As String,
                                     tuttiStati As List(Of AgronicaCoreEntityFramework_POCO.Zoo_Animali_Lista_Stati_Accrescimento)) As attivita.centri_di_costo.CapoAnimaleCDC

        Dim objCapo As New CapoAnimale With {
            .partitaIva = piva,
            .matricola = matricolaCapo,
            .sesso = sesso,
            .codice = 0,
            .nome = "",
            .collare = "",
            .lottoFornitore = "",
            .dataNascita = dataNascita,
            .numCertificato = numCertificato,
            .codiceFiscaleDetentore = codDetentore,
            .codiceFiscaleProprietario = "",
            .codiceAziendaNascita = codAziendaNascita,
            .stallaSvezzamento = stallaSvezzamento,
            .indirizzoProd = New metaschema.IndirizzoProduttivo(0),
            .categoria = New metaschema.Categoria(0),
            .tipologia = New metaschema.TipologiaCapoAnimale(1),
            .metodoProduzione = New metaschema.MetodoProduzione(1),
            .idCapo_BDN = 0,
            .genere = New metaschema.Genere(genCod),
            .specie = New metaschema.utilizzi.Specie(speCod),
            .razza = New metaschema.Razza(codRazza),
            .esercizi = New List(Of AgronicaCoreModelsSTD.anagrafiche.EsercizioCapoAnimale),
            .statiAccrescimento = New List(Of AgronicaCoreModelsSTD.anagrafiche.StatoAccrescimento),
            .fornitore = New AgronicaCoreModelsSTD.anagrafiche.Contatto With {
                .primaryKey = New AgronicaCoreModelsSTD.anagrafiche.Contatto.PK With {.partitaIva = ""}
            },
            .validita = New anagrafiche.IntervalloTemporale With {
                .inizio = ingresso,
                .fine = AGRODATAFINE
            },
            .validitaConversione = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale With {
                .inizio = AGRODATAINIZIO,
                .fine = AGRODATAFINE
            },
            .madre = New CapoAnimale With {
                .razza = New metaschema.Razza(codRazzaMadre),
                .codice = 0,
                .matricola = matricolaMadre
            },
            .padre = New CapoAnimale With {
                .razza = New metaschema.Razza(codRazzaPadre),
                .codice = 0,
                .matricola = ""
            },
            .ingresso_modello4_data_prenotazione = ingresso
        }

        CreaDistinte(objCapo)
        CreaStatiAccrescimento(objCapo, tuttiStati)

        Dim capoCDC As New attivita.centri_di_costo.CapoAnimaleCDC()
        capoCDC.codice = New attivita.centri_di_costo.CentroDiCosto.CodeType(sa_cod)
        capoCDC.capoAnimale = objCapo
        capoCDC.sottogruppoStalla_ingresso = New SottogruppoStalla With {.codice = raggruppamentoCod}
        capoCDC.sottogruppoStalla_uscita = New SottogruppoStalla With {.codice = raggruppamentoCod}

        Return capoCDC
    End Function

    Private Sub CreaDistinte(ByRef objCapo As CapoAnimale)
        Dim objEsercizio As New AgronicaCoreModelsSTD.anagrafiche.EsercizioCapoAnimale
        objEsercizio.codice_capo_animale = objCapo.codice
        objEsercizio.validita = New IntervalloTemporale With {
            .inizio = objCapo.validita.inizio,
            .fine = objCapo.validita.fine
        }
        objEsercizio.progettoNome = ""
        objCapo.esercizi.Add(objEsercizio)
    End Sub

    ''' <summary>
    ''' Calcola gli stati di accrescimento del capo usando la lista pre-caricata dal DB.
    ''' I template (riga per riga di configurazione) sono cachati per chiave
    ''' piva|genCod|speCod|tipoCod per evitare filtraggi in-memory ripetuti.
    ''' </summary>
    Private Sub CreaStatiAccrescimento(ByRef objCapo As CapoAnimale,
                                        tuttiStati As List(Of AgronicaCoreEntityFramework_POCO.Zoo_Animali_Lista_Stati_Accrescimento))
        Dim piva As String = objCapo.partitaIva
        Dim genCod As Integer = objCapo.genere.codice
        Dim speCod As Integer = objCapo.specie.codice
        Dim tipoCod As Integer = objCapo.tipologia.codice
        Dim cacheKey As String = objCapo.partitaIva & "|" & genCod & "|" & speCod & "|" & tipoCod

        Dim statiTemplate As List(Of AgronicaCoreEntityFramework_POCO.Zoo_Animali_Lista_Stati_Accrescimento)
        If Not _statiTemplateCache.TryGetValue(cacheKey, statiTemplate) Then
            statiTemplate = (From zan In tuttiStati
                             Where (zan.PIVA = piva Or zan.Sa_Cod = -1) AndAlso
                                    zan.GEN_COD = genCod AndAlso zan.SPE_COD = speCod AndAlso zan.TIPO_COD = tipoCod
                             Order By zan.Giorno_Da).ToList()
            _statiTemplateCache(cacheKey) = statiTemplate
        End If

        If statiTemplate Is Nothing OrElse statiTemplate.Count = 0 Then
            Throw New GiasException("Non sono configurati gli stati di accrescimento per questo tipo di capo. Verificare la configurazione in GIAS.")
        End If

        For Each sa In statiTemplate
            Dim objStatoAccr As New StatoAccrescimento With {
                .codice = sa.STATO_COD,
                .descrizione = sa.Stato_Des,
                .validita = New IntervalloTemporale With {
                    .inizio = If(IsNothing(sa.Giorno_Da), objCapo.dataNascita, objCapo.dataNascita.AddDays(sa.Giorno_Da)),
                    .fine = If(IsNothing(sa.Giorno_A), AGRODATAFINE, objCapo.dataNascita.AddDays(sa.Giorno_A))
                }
            }
            objCapo.statiAccrescimento.Add(objStatoAccr)
        Next
    End Sub

    ' ─────────────────────────────────────────────────────────────
    '  Audit log helper
    ' ─────────────────────────────────────────────────────────────

    ''' <summary>
    ''' Costruisce un messaggio di audit strutturato per Scrivi_LOG.
    ''' Usa il LogProvider esistente invece di una tabella dedicata.
    ''' </summary>
    Private Shared Function FormattaAuditLog(
            capo As DataRow,
            importStatus As String,
            Optional errorMsg As String = "") As String

        Dim dt As DataTable = capo.Table
        Dim sb As New System.Text.StringBuilder
        sb.Append("[AUDIT|ParmaFrance]")

        If dt.Columns.Contains("NumeroSequenza") AndAlso Not IsDBNull(capo("NumeroSequenza")) Then
            sb.Append(" Seq=").Append(CStr(capo("NumeroSequenza")))
        End If

        If dt.Columns.Contains("Matricola") AndAlso Not IsDBNull(capo("Matricola")) Then
            sb.Append(" Matricola=").Append(CStr(capo("Matricola")))
        End If

        If dt.Columns.Contains("Specie") AndAlso Not IsDBNull(capo("Specie")) Then
            sb.Append(" Specie=").Append(capo("Specie").ToString())
        End If

        If dt.Columns.Contains("F5") AndAlso Not IsDBNull(capo("F5")) Then
            sb.Append(" RazzaOriginale=").Append(CStr(capo("F5")))
        End If

        If dt.Columns.Contains("RazzaRisolta") AndAlso Not IsDBNull(capo("RazzaRisolta")) Then
            sb.Append(" RazzaApplicata=").Append(CStr(capo("RazzaRisolta")))
        End If

        If dt.Columns.Contains("RazzaLookupStatus") AndAlso Not IsDBNull(capo("RazzaLookupStatus")) Then
            sb.Append(" LookupStatus=").Append(CStr(capo("RazzaLookupStatus")))
        End If

        If dt.Columns.Contains("RazzaUsedDefault") AndAlso Not IsDBNull(capo("RazzaUsedDefault")) AndAlso
           CBool(capo("RazzaUsedDefault")) Then
            sb.Append(" UsedDefault=True")
            If dt.Columns.Contains("RazzaDefaultReason") AndAlso Not IsDBNull(capo("RazzaDefaultReason")) Then
                sb.Append(" DefaultReason=").Append(CStr(capo("RazzaDefaultReason")))
            End If
        End If

        sb.Append(" ImportStatus=").Append(importStatus)

        If Not String.IsNullOrEmpty(errorMsg) Then
            sb.Append(" Error=").Append(errorMsg)
        End If

        Return sb.ToString()
    End Function

    ' ─────────────────────────────────────────────────────────────
    '  Cache e lookup helpers
    ' ─────────────────────────────────────────────────────────────

    ''' <summary>
    ''' Risolve il codice razza Agronica a partire dal codice razza FR.
    ''' Il risultato è cachato in sessione per evitare query ripetute sulle
    ''' stesse razze (una tabella di lookup statica interrogata 3 volte per capo).
    ''' NOTA: usa InfoAgg_Cod = 9, specifico per bovini.
    ''' Per future specie (es. suini) sarà necessaria una strategia alternativa.
    ''' </summary>
    Private Function RicavaRazzaCapoAnimale(codRazza As Integer) As Integer
        If _razzaCache.ContainsKey(codRazza) Then Return _razzaCache(codRazza)

        Dim dp As New DataProvider
        Dim sql As New StringBuilder
        sql.AppendLine("SELECT *")
        sql.AppendLine("FROM Cac_Codifica_InfoAggiuntive")
        sql.AppendLine("WHERE InfoAgg_Cod = 9 AND Argomento_Cod = " & codRazza)

        Dim dtFR As DataTable = dp.EseguiQuery_Lettura(_parametriServer, sql.ToString(), "estrazioneCodRazzaBDNDaCodRazzaFR")
        Dim rowFR As DataRow = dtFR.Select("").FirstOrDefault()
        If IsNothing(rowFR) Then
            Throw New GiasException("Errore! Razza UE " & codRazza & " non mappata in GIAS.")
        End If

        Dim razzaBDN As String = rowFR.Item("TestoAux_1")
        Dim codiceRazzaBDN As Integer = rowFR.Item("CodiceAux_3")

        Dim obj_RazzeAnimali_R As New AgronicaCoreMetaSchemaDAL.Codifica_BDN_RazzeAnimali
        Dim dtBDN As DataTable = obj_RazzeAnimali_R.leggi(_parametriServer, "", "", "", "", "",
                                                           enum_Esportazioni_Sistema_Cod.BDN, razzaBDN)

        If IsNothing(dtBDN) OrElse dtBDN.Rows.Count = 0 Then
            Throw New GiasException("Errore! Razza BDN " & razzaBDN & " non mappata in GIAS.")
        End If

        Dim razzaAgronica As Integer
        If dtBDN.Rows.Count > 1 AndAlso codiceRazzaBDN > 0 Then
            Dim rowMultipla As DataRow = dtBDN.Select(" RAZZA_ID = " & codiceRazzaBDN).FirstOrDefault()
            If IsNothing(rowMultipla) Then
                Throw New GiasException("Errore! Razza BDN " & razzaBDN & " non mappata in GIAS.")
            End If
            razzaAgronica = CInt(rowMultipla.Item("RAZ_COD"))
        Else
            razzaAgronica = CInt(dtBDN.Rows(0)("RAZ_COD"))
        End If

        _razzaCache(codRazza) = razzaAgronica
        Return razzaAgronica
    End Function

    ''' <summary>
    ''' Recupera il codice CUAA detentore per la PIVA indicata.
    ''' Il risultato è cachato: la chiamata DAL viene effettuata una sola volta per PIVA.
    ''' </summary>
    Private Function GetCodDetentore(piva As String) As String
        If _detentoreCache.ContainsKey(piva) Then Return _detentoreCache(piva)
        Dim objImprese As New Imprese_Read
        Dim dt As DataTable = objImprese.Leggi_x_anagrafica(piva, "", "", _parametriServer)
        Dim row As DataRow = dt.Select("").FirstOrDefault()
        Dim codDetentore As String = row.Item("Codice_Cuaa")
        _detentoreCache(piva) = codDetentore
        Return codDetentore
    End Function

    ''' <summary>
    ''' Pre-carica in batch dal DB le stalle di svezzamento presenti nel DataTable.
    ''' Sostituisce le query per-riga precedenti con una singola round-trip EF.
    ''' </summary>
    Private Function CaricaDizionarioSvezzamento(dtCapi As DataTable,
                                                   giasCtx As Gias_DeveloperServer_Entities) As Dictionary(Of String, String)
        Dim nomi = dtCapi.Rows.Cast(Of DataRow).
            Where(Function(r) Not IsDBNull(r("Stalla_Svezzamento")) AndAlso
                               CStr(r("Stalla_Svezzamento")).Trim() <> "").
            Select(Function(r) CStr(r("Stalla_Svezzamento")).Trim()).
            Distinct().ToList()

        If nomi.Count = 0 Then Return New Dictionary(Of String, String)()

        Return giasCtx.Risorse_Umane.
            Where(Function(r) nomi.Contains(r.Settore_Des) AndAlso
                               (r.Cod_Rapporto = COD_ALLEVATORE Or r.Cod_Rapporto = COD_FORNITORE)).
            ToList().
            GroupBy(Function(r) r.Settore_Des).
            ToDictionary(Function(g) g.Key, Function(g) g.First().Cod_Contatto)
    End Function

    Private Function RisolveStallasvezzamento(nomeSvezzamento As String,
                                               codiceCapo As String,
                                               svezzamentoDict As Dictionary(Of String, String)) As String
        If Not svezzamentoDict.ContainsKey(nomeSvezzamento) Then
            Throw New Exception("La stalla di svezzamento " & nomeSvezzamento &
                                " inserita per il capo " & codiceCapo & " non è presente su GIAS.")
        End If
        Return svezzamentoDict(nomeSvezzamento)
    End Function

End Class
