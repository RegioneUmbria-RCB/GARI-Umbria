Imports System.Text
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreG2GLocalDal
Imports AgronicaCoreModello
Imports Gias2Gias_LIB
Imports Newtonsoft.Json

Partial Public Class GIAS_2_GIAS

    Private _objScriviHelper As Gias2Gias_LIB.Funzioni
    Private _opzioni As Gias2Gias_LIB.clsOpzioni
    Private _objOpzioniImportSingolaImpresa As clsImpresa

    Private _Log_Errori As StringBuilder
    Private _Log_G2G As StringBuilder
    Private _Log_Riepilogo As StringBuilder

    '###########################################################################################################
    Private Sub G2G_Chiusura_Transazione(ByVal Flag_Commit1_Rollback2 As Integer)

        Dim NomeRoutine As String = "G2G_Chiusura_Transazione"

        Try
            'chiudo la transazione sull'origine
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(Flag_Commit1_Rollback2, _opzioni.objParametri_Server_GIAS_ORIGINE)
            If (_opzioni.isGias2Gias_local) Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(Flag_Commit1_Rollback2, _opzioni.objParametri_Server_GIAS_DESTINAZIONE)
            Else
                '_Log_G2G.Append("il G2G è su web service: nessuna transazione" & vbCrLf)
            End If

        Catch ex As Exception
            _Log_G2G.Append("Errore durante la chiusura della transazione" & vbCrLf)
        End Try

    End Sub

#Region "Impresa"

    Private Sub G2G_Gestione_DopoSalvataggioUltimaImpresa(ByVal objOpzioniGlobali As clsDatiGlobali, ByRef Log_G2G As StringBuilder, ByRef Log_Errori As StringBuilder, ByRef Log_Riepilogo As StringBuilder, ByRef ErrFLAG As Integer)

        Dim NomeRoutine As String = "G2G_Gestione_DopoSalvataggioUltimaImpresa"

        Try

            If _opzioni.isGias2Gias_local Then '  Marco Grilli, 28/08/2014 13:08:53: DA SISTEMARE ED ELIMINARE...
                G2G_AnalisiTipologia(objOpzioniGlobali, Log_G2G, Log_Errori, Log_Riepilogo, ErrFLAG)
                G2G_PianiDiCampionamento(objOpzioniGlobali, Log_G2G, Log_Errori, Log_Riepilogo, ErrFLAG)
            End If



        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine(" [" & NomeRoutine & "] : " & ex.Message.ToString)
        End Try


    End Sub

    Private Sub G2G_GestioneRiferimentiAgenda(ByVal piva As String, ByVal objOpzioniGlobali As clsDatiGlobali, ByRef Log_G2G As StringBuilder, ByRef Log_Errori As StringBuilder, ByRef Log_Riepilogo As StringBuilder, ByRef ErrFLAG As Integer)

        Dim NomeRoutine As String = "G2G_GestioneRiferimentiAgenda"
        Try

            _objScriviHelper.Elabora_RiferimentiAgenda(
                      piva _
                    , _opzioni _
                    , Log_G2G _
                    , Log_Errori _
                    , Log_Riepilogo
                    )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine(" [" & NomeRoutine & "] : " & ex.Message.ToString)
        End Try

    End Sub

    Private Sub G2G_GestioneRiferimentiAgenda_WS(ByVal piva As String, ByVal objOpzioniGlobali As clsDatiGlobali, ByRef Log_G2G As StringBuilder, ByRef Log_Errori As StringBuilder, ByRef Log_Riepilogo As StringBuilder, ByRef ErrFLAG As Integer)

        Dim NomeRoutine As String = "G2G_GestioneRiferimentiAgenda_WS"
        Try

            _objScriviHelper.Elabora_RiferimentiAgenda_WS(
                      piva _
                    , _opzioni _
                    , Log_G2G _
                    , Log_Errori _
                    , Log_Riepilogo
                    )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine(" [" & NomeRoutine & "] : " & ex.Message.ToString)
        End Try

    End Sub

    Private Sub G2G_GestioneRiferimentiAgenda_WSReverse(ByVal piva As String, ByVal objOpzioniGlobali As clsDatiGlobali, ByRef Log_G2G As StringBuilder, ByRef Log_Errori As StringBuilder, ByRef Log_Riepilogo As StringBuilder, ByRef ErrFLAG As Integer)

        Dim NomeRoutine As String = "G2G_GestioneRiferimentiAgenda_WSReverse"
        Try

            _objScriviHelper.Elabora_RiferimentiAgenda_WSReverse(
                      piva _
                    , _opzioni _
                    , Log_G2G _
                    , Log_Errori _
                    , Log_Riepilogo
                    )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine(" [" & NomeRoutine & "] : " & ex.Message.ToString)
        End Try

    End Sub

    Private Sub G2G_AnalisiTipologia(ByVal objOpzioniGlobali As clsDatiGlobali, ByRef Log_G2G As StringBuilder, ByRef Log_Errori As StringBuilder, ByRef Log_Riepilogo As StringBuilder, ByRef ErrFLAG As Integer)
        Dim NomeRoutine As String = "G2G_AnalisiTipologia"
        Try

            _objScriviHelper.Elabora_Analisi_tipologia(
                    _opzioni _
                    , Log_G2G _
                    , Log_Errori _
                    , Log_Riepilogo
                    )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine(" [" & NomeRoutine & "] : " & ex.Message.ToString)
        End Try

    End Sub

    Private Sub G2G_PianiDiCampionamento(ByVal objOpzioniGlobali As clsDatiGlobali, ByRef Log_G2G As StringBuilder, ByRef Log_Errori As StringBuilder, ByRef Log_Riepilogo As StringBuilder, ByRef ErrFLAG As Integer)

        Try
            If objOpzioniGlobali.Flagimporta_pianicampionamento Then
                _objScriviHelper.Elabora_Piani_di_campionamento_Salva(
                        _opzioni _
                        , Log_G2G _
                        , Log_Errori _
                        , Log_Riepilogo
                        )
            End If
        Catch ex As Exception

        End Try

    End Sub


    Private Sub G2G_ApriTransazione()

        'Apro, Inizializzo la transazione
        AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, _opzioni.objParametri_Server_GIAS_ORIGINE)
        If (_opzioni.isGias2Gias_local) Then
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, _opzioni.objParametri_Server_GIAS_DESTINAZIONE)
        End If

    End Sub
    Private Sub G2G_Gestione_DopoSalvataggioUltimaImpresa(ByVal objOpzioniGlobali As clsDatiGlobali, ByRef ErrFLAG As Integer, ByRef Log_G2G As StringBuilder, ByRef Log_Errori As StringBuilder, ByRef Log_Riepilogo As StringBuilder, ByVal i_corrente As Integer, ByVal num_tot_imprese As Integer)
        If i_corrente = num_tot_imprese Then
            ErrFLAG = 0

            G2G_ApriTransazione()

            G2G_Gestione_DopoSalvataggioUltimaImpresa(objOpzioniGlobali, Log_G2G, Log_Errori, Log_Riepilogo, ErrFLAG)
            If ErrFLAG = 0 Then
                G2G_Chiusura_Transazione(1)
            Else
                G2G_Chiusura_Transazione(2)
            End If
        End If
    End Sub

    '###########################################################################################################
    Private Sub G2G_Gestione_Impresa(
                                    ByVal objOpzioniGlobali As clsDatiGlobali,
                                    ByVal objOpzioniImportImpresa As clsImpresa,
                                    ByVal objOpzioni As Gias2Gias_LIB.clsOpzioni,
                                    ByRef ErrFLAG As Integer,
                                    ByRef Log_G2G As StringBuilder,
                                    ByRef Log_Errori As StringBuilder,
                                    ByRef Log_Riepilogo As StringBuilder,
                                    ByVal i_corrente As Integer,
                                    ByVal num_tot_imprese As Integer
                                    )

        _opzioni = objOpzioni
        _objOpzioniImportSingolaImpresa = objOpzioniImportImpresa
        _Log_G2G = Log_G2G
        _Log_Errori = Log_Errori
        _Log_Riepilogo = Log_Riepilogo

        _objScriviHelper = New Gias2Gias_LIB.Funzioni(objOpzioni.objParametri_Server_GIAS_ORIGINE, objOpzioni.TimeOut_Chiamata_WS)
        _objFunzioni_Global = New Gias2Gias_LIB.FunzioniGLOBAL(objOpzioni.objParametri_Server_GIAS_ORIGINE, objOpzioni.TimeOut_Chiamata_WS)
        _objScriviHelper.FunzioniGLOBAL = _objFunzioni_Global

        Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
        Dim ConnectionString As String = gefutils.GetEntityConnectionString(_opzioni.objParametri_Server_GIAS_ORIGINE.StringaConnessione)
        Dim efG2G As New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(ConnectionString)

        '  Vanni, 19/05/2014 16:19:00: caricamento di tutte le ri-codifiche ...
        _objFunzioni_Global.Recode_Caricamento(efG2G)
        _objScriviHelper.Recode_Caricamento(efG2G)

        'TODO : Recupero dei parametri

        Dim KeyImpresa_ORIGINE As New clsKeyImpresa()
        Dim KeyImpresa_DESTINAZIONE As New clsKeyImpresa()
        Dim KeyImpresa_Padre_DESTINAZIONE As New clsKeyImpresa()

        KeyImpresa_ORIGINE.Piva = _objOpzioniImportSingolaImpresa.Piva_ORIGINE
        KeyImpresa_DESTINAZIONE.Piva = _objOpzioniImportSingolaImpresa.Piva_DESTINAZIONE
        KeyImpresa_Padre_DESTINAZIONE.Piva = objOpzioniImportImpresa.PivaPadre_DESTINAZIONE

        '---------------------------------
        'Inizializzazioni
        Dim CodiceGias_ORIGINE As New clsCodiceGias(_opzioni.ProgressivoGIAS_ORIGINE)
        Dim CodiceGias_DESTINAZIONE As New clsCodiceGias(_opzioni.ProgressivoGIAS_DESTINAZIONE)



        ErrFLAG = 0

        Log_G2G.Append("----- Impresa " & KeyImpresa_ORIGINE.Piva & " -----" & vbCrLf)

        '------------------------------
        Try
            G2G_ApriTransazione()

            'Parte pubblica
            If i_corrente = 1 Then
                '    'i dati pubblici li gestisco a questo punto
                '    'perchè prima deve essere stata scritta almeno l'azienda superuser
                '    'altrimenti quando si legge e si af il join con utentiximprese
                '    'non vengono trovati i record inseriti

                '    'sono alla prima impresa
                '    'devo salvare tutti i contatti, macchine e materie prime pubbliche

                '    '------------------------------ CONTATTI
                '    '--------- CONTATTI pubblici

                '    G2G_Copia_ContattiPubbliciNoImpreseGias( _
                '        objOpzioni, _
                '        ErrFLAG _
                '        )

                '    If ErrFLAG <> 0 Then
                '        G2G_Chiusura_Transazione(2)
                '        Exit Sub
                '    End If


                '    '------------------------------ PARCO MACCHINE
                '    '--------- macchine pubbliche

                '    G2G_Copia_ParcoMacchine(KeyImpresa_ORIGINE, _
                '                                KeyImpresa_DESTINAZIONE, _
                '                                objOpzioni, _
                '                                ErrFLAG, _
                '                                True)

                '    If ErrFLAG <> 0 Then
                '        G2G_Chiusura_Transazione(2)
                '        Exit Sub
                '    End If

                '    '------------------------------ MATERIE PRIME
                '    '--------- materie prime pubbliche
                '    ' If _objSingleCopyOpzioni.flagimporta_agenda Or _objSingleCopyOpzioni.Flagimporta_materieprime Then
                '    'commento l'if, perchè se per sbaglio la prima impresa non ha i flag = true
                '    'i pubblici non vengono elaborati!
                'G2G_Copia_MateriePrime_NEW(KeyImpresa_ORIGINE, _
                '                            KeyImpresa_DESTINAZIONE, _
                '                            objOpzioni, _
                '                            ErrFLAG, _
                '                            True)

                'If ErrFLAG <> 0 Then
                '    G2G_Chiusura_Transazione(2)
                '    Exit Sub
                'End If
                '    'End If

                '    'gestione pagamenti per copia dati comuni: causali, istituti credito
                '    G2G_Copia_GestionePagamentiPublic( _
                '        objOpzioni, _
                '        ErrFLAG)

                '    If ErrFLAG <> 0 Then
                '        G2G_Chiusura_Transazione(2)
                '        Exit Sub
                '    End If


            End If 'prima impresa x dati pubblici


            If i_corrente = num_tot_imprese Then
                'sono all'ultima impresa
                'salvo la profilazione globale

                '--- DEFAULT GLOBALI ------- PROFILAZIONE
                If objOpzioniGlobali.Flagimporta_profilazione Then
                    G2G_Copia_Profilazione(KeyImpresa_ORIGINE,
                                                  KeyImpresa_DESTINAZIONE,
                                                  ErrFLAG,
                                                  True)
                    If ErrFLAG <> 0 Then
                        Throw New Exception
                    End If
                End If


                ''vv ''--- DEFAULT GLOBALI ------- RICETTE                        
                G2G_Copia_RicetteGlobali(ErrFLAG)
                If ErrFLAG <> 0 Then
                    Throw New Exception
                End If

                'vv ''--- CAC_CODIFICA            
                If objOpzioniGlobali.Flagimporta_cac_codifica Then
                    G2G_Copia_CAC_Codifica(ErrFLAG)

                    If ErrFLAG <> 0 Then
                        Throw New Exception
                    End If
                End If

            End If
            '----fine parte globale

            '--------------parte singola impresa 

            'TODO : Verifico se l'impresa scelta esiste in ORIGINE
            Dim leggiImpresa As New AgronicaCoreAnagrafeDAL.Imprese_Read
            If Not leggiImpresa.VerificaEsistenza_PivaGIAS(KeyImpresa_ORIGINE.Piva, _opzioni.objParametri_Server_GIAS_ORIGINE) Then
                ErrFLAG = 1
                Log_G2G.Append("Impresa " & KeyImpresa_ORIGINE.Piva & " : non esiste nel database di origine. non è stato trasferito alcun dato. " & vbCrLf)
                Throw New Exception
            End If

            'TODO : Verifico se l'impresa scelta esiste gia' in DESTINAZIONE
            Dim esisteImpresaDestinazione As Boolean

            If (objOpzioni.isGias2Gias_local) Then
                esisteImpresaDestinazione = leggiImpresa.VerificaEsistenza_PivaGIAS(KeyImpresa_DESTINAZIONE.Piva, _opzioni.objParametri_Server_GIAS_DESTINAZIONE)
            Else

                Dim Str_Credenziali_WS As String = Gias2Gias_LIB.Funzioni.Get_Str_Credenziali_WS(_opzioni)

                Dim strErr As String = ""
                Dim ws As New WS_Importa_GIAS_2014.ImportaWS
                ws.Url = _opzioni.wsimportaGiasURl
                ws.Timeout = Integer.MaxValue
                esisteImpresaDestinazione = ws.Verifica_EsistenzaPivaGIAS(Str_Credenziali_WS, KeyImpresa_DESTINAZIONE.Piva, strErr)
                '  Marco Grilli, 20/08/2014 16:29:19: se ho ricevuto un errore, piuttosto che duplicare tutto, esco.
                If (strErr <> "") Then
                    Throw New Exception
                End If

                ' verifico se esiste l'impresa padre nel gias destinazione altrimenti imposto padre = superpiva destinazione
                Dim Piva_Padre As String = KeyImpresa_Padre_DESTINAZIONE.Piva
                If String.IsNullOrEmpty(Piva_Padre) Then
                    Dim objGera As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R
                    Piva_Padre = objGera.LeggiPadre(KeyImpresa_ORIGINE.Piva, objOpzioni.objParametri_Server_GIAS_ORIGINE, "")
                End If

                If Not String.IsNullOrEmpty(Piva_Padre) AndAlso ws.Verifica_EsistenzaPivaGIAS(Str_Credenziali_WS, Piva_Padre, strErr) Then
                    KeyImpresa_Padre_DESTINAZIONE.Piva = Piva_Padre
                Else
                    KeyImpresa_Padre_DESTINAZIONE.Piva = objOpzioni.SuperUser_CodFiscale_DESTINAZIONE
                End If

            End If

            '  Vanni, 20/05/2014 15:24:46: accoda sempre e comunque
            objOpzioniImportImpresa.FlagAccodaDatiSeEsistePivaDestinazione = True

            'TODO: Verifico se esiste l'impresa padre nella DESTINAZIONE: posso fare affidamento sul fatto che l'impresa padre sia nelle imprese da importare?        
            If esisteImpresaDestinazione AndAlso Not objOpzioniImportImpresa.FlagAccodaDatiSeEsistePivaDestinazione Then
                ErrFLAG = 1
                Log_G2G.Append("Impresa " & KeyImpresa_DESTINAZIONE.Piva & " : Esiste già. non è stato trasferito alcun dato. " & vbCrLf)
                Throw New Exception
            End If

            If Not esisteImpresaDestinazione Then

                '------------------------------ IMPRESA
                G2G_Copia_Impresa(KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG)
                If ErrFLAG <> 0 Then
                    Throw New Exception
                End If

                '------------------------------ INDIRIZZO
                G2G_Copia_Indirizzo_Impresa(KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG)
                If ErrFLAG <> 0 Then
                    Throw New Exception
                End If

                '------------------------------ CODICI
                G2G_Copia_Imprese_Codici(KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG)
                If ErrFLAG <> 0 Then
                    Throw New Exception
                End If

                '------------------------------ GERARCHIA IMPRESE
                G2G_Scrivi_GerarchiaImprese(KeyImpresa_ORIGINE, KeyImpresa_Padre_DESTINAZIONE, KeyImpresa_DESTINAZIONE, ErrFLAG)
                If ErrFLAG <> 0 Then
                    Throw New Exception
                End If

                '------------------------------ UTENTI x IMPRESE
                G2G_Scrivi_UtentixImprese(KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG)
                If ErrFLAG <> 0 Then
                    Throw New Exception
                End If

                ''------------------------------ CONTATTI - impresa contatto (sotto superuser)
                G2G_Copia_ContattoImpresa(KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, objOpzioni, ErrFLAG)
                If ErrFLAG <> 0 Then
                    Throw New Exception
                End If

                '------------------------------ CONTATTI - contatti privati dell'azienda
                G2G_Copia_ContattiPrivatiNoImpreseGias(KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, objOpzioni, ErrFLAG)
                If ErrFLAG <> 0 Then
                    Throw New Exception
                End If

                '------------------------------ PARCO MACCHINE - macchine private dell'azienda
                G2G_Copia_ParcoMacchine(KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, objOpzioni, ErrFLAG, False)
                If ErrFLAG <> 0 Then
                    Throw New Exception
                End If

                '------------------------------ SPECIE VEGETALI DEFAULT
                G2G_Copia_SpecieVegetaliDefault(KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG)
                If ErrFLAG <> 0 Then
                    Throw New Exception
                End If

                'vv------------------------------ PROFILAZIONE
                If _objOpzioniImportSingolaImpresa.Flagimporta_profilazione Then
                    G2G_Copia_Profilazione(KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG, False)
                    If ErrFLAG <> 0 Then
                        Throw New Exception
                    End If
                End If

                ''vv ''------------------------------ AREE OMOGENEA
                If _objOpzioniImportSingolaImpresa.flagimporta_planning Then
                    G2G_Copia_AreaOmogenea(KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG)
                    If ErrFLAG <> 0 Then
                        Throw New Exception
                    End If
                End If

            End If

            '  Marco Grilli, 24/02/2015 17:28:08: l'ho tirata fuori dal if di sopra perché l'utente potrebbe aggiungere materie prime anche in seguito
            'maga------------------------------ MATERIE PRIME AZIENDALI
            '--------- materie prime private dell'azienda
            'If _objSingleCopyOpzioni.flagimporta_agenda OrElse _objSingleCopyOpzioni.Flagimporta_materieprime Then

            '    G2G_Copia_MateriePrime_NEW(KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, objOpzioni, ErrFLAG, False)
            '    If ErrFLAG <> 0 Then
            '        Throw New Exception
            '    End If
            'End If

            '------------------------------ CENTRI AZIENDALI
            G2G_Lista_Centri(
                objOpzioniImportImpresa,
                KeyImpresa_ORIGINE,
                KeyImpresa_DESTINAZIONE,
                CodiceGias_ORIGINE,
                CodiceGias_DESTINAZIONE,
                ErrFLAG
            )

            If ErrFLAG <> 0 Then
                Throw New Exception
            End If

            If objOpzioniImportImpresa.flagimporta_planning Then
                G2G_Copia_Planning_Impresa(KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG)
                If ErrFLAG <> 0 Then
                    Throw New Exception
                End If
            End If

            If _objOpzioniImportSingolaImpresa.FlagImporta_Audit OrElse _objOpzioniImportSingolaImpresa.FlagImporta_Audit_Interviste Then
                Dim auditHelper As New Audit
                auditHelper.ImportaAudit(
                    Log_G2G,
                    Log_Errori,
                    Log_Riepilogo,
                    KeyImpresa_ORIGINE.Piva,
                    KeyImpresa_DESTINAZIONE.Piva,
                    objOpzioni)

                If ErrFLAG <> 0 Then
                    Throw New Exception
                End If
            End If

            ' ''------------  BIO Zeta
            If _objOpzioniImportSingolaImpresa.flagimporta_papz Then
                G2G_Copia_bioZeta_Centro(KeyImpresa_ORIGINE.Piva, KeyImpresa_DESTINAZIONE.Piva, ErrFLAG)

                If ErrFLAG <> 0 Then
                    Throw New Exception
                End If
            End If

            ''vv ''------------------------------ LIQUIDITA'
            If (_opzioni.isGias2Gias_local) Then ' ------------------------------> DA RIMUOVERE (MARCO G)
                If _objOpzioniImportSingolaImpresa.flagimporta_agenda Then

                    G2G_Copia_Liquidita(KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG)
                    If ErrFLAG <> 0 Then
                        Throw New Exception
                    End If
                End If
            End If

            ''vv ''------------------------------ AGENDA
            If _objOpzioniImportSingolaImpresa.flagimporta_agenda Then

                'cancellazione
                G2G_Copia_Agenda(objOpzioniImportImpresa, efG2G, KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG, enum_TipoOperazioneDB.Cancellazione)
                If ErrFLAG <> 0 Then
                    Throw New Exception
                End If
                _objScriviHelper.Recode_Salvataggio_Agenda(efG2G, Log_G2G, Log_Errori, Log_Riepilogo, ErrFLAG)
                If ErrFLAG <> 0 Then
                    Throw New Exception
                End If

                'modifica
                G2G_Copia_Agenda(objOpzioniImportImpresa, efG2G, KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG, enum_TipoOperazioneDB.Modifica)
                If ErrFLAG <> 0 Then
                    Throw New Exception
                End If
                _objScriviHelper.Recode_Salvataggio_Agenda(efG2G, Log_G2G, Log_Errori, Log_Riepilogo, ErrFLAG)
                If ErrFLAG <> 0 Then
                    Throw New Exception
                End If

                'scrittura
                G2G_Copia_Agenda(objOpzioniImportImpresa, efG2G, KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG, enum_TipoOperazioneDB.Scrittura)
                If ErrFLAG <> 0 Then
                    Throw New Exception
                End If
                _objScriviHelper.Recode_Salvataggio_Agenda(efG2G, Log_G2G, Log_Errori, Log_Riepilogo, ErrFLAG)
                If ErrFLAG <> 0 Then
                    Throw New Exception
                End If


            End If


            ''vv------------------------------ BIO Notifica x impresa
            If _objOpzioniImportSingolaImpresa.flagimporta_notificabio Then
                G2G_Copia_bio_notificaXImpresa(KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG)
                If ErrFLAG <> 0 Then
                    Throw New Exception
                End If
            End If

            If _objOpzioniImportSingolaImpresa.flagimporta_ricette Then
                G2G_Copia_RicetteImpresa(KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG)
                If ErrFLAG <> 0 Then
                    Throw New Exception
                End If
            End If

            If _objOpzioniImportSingolaImpresa.flagimporta_pua Then
                G2G_Copia_PUA(KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG)
                If ErrFLAG <> 0 Then
                    Throw New Exception
                End If
            End If

            If _objOpzioniImportSingolaImpresa.Flagimporta_LineeProduttive Then
                G2G_Copia_Lotti_Configurazione(KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG)
                If ErrFLAG <> 0 Then
                    Throw New Exception
                End If
            End If

            If _objOpzioniImportSingolaImpresa.Flagimporta_LineeProduttive Then
                G2G_Copia_LineeProduttive(KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG)
                If ErrFLAG <> 0 Then
                    Throw New Exception
                End If
            End If

            If _objOpzioniImportSingolaImpresa.Flagimporta_analisi Then
                G2G_Gestione_ImportaAnalisi(objOpzioni, ErrFLAG, Log_G2G, Log_Errori, Log_Riepilogo, KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, enum_AnalisiTipo.Analisi_Fitofarmaci)
                G2G_Gestione_ImportaAnalisi(objOpzioni, ErrFLAG, Log_G2G, Log_Errori, Log_Riepilogo, KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, enum_AnalisiTipo.Analisi_Terreno)

                If ErrFLAG <> 0 Then
                    Throw New Exception
                End If
            End If

            G2G_GestioneRiferimentiAgenda(KeyImpresa_ORIGINE.Piva, objOpzioniGlobali, Log_G2G, Log_Errori, Log_Riepilogo, ErrFLAG)
            If ErrFLAG <> 0 Then
                Throw New Exception
            End If

            'fine --------------parte singola impresa 

            '  Vanni, 19/05/2014 16:19:00: salvataggio di tutte le ri-codifiche ...
            _objFunzioni_Global.Recode_Salvataggio(efG2G, Log_G2G, Log_Errori, Log_Riepilogo, ErrFLAG)
            If ErrFLAG <> 0 Then
                Throw New Exception
            End If
            _objScriviHelper.Recode_Salvataggio(efG2G, Log_G2G, Log_Errori, Log_Riepilogo, ErrFLAG)
            If ErrFLAG <> 0 Then
                Throw New Exception
            End If

            G2G_Chiusura_Transazione(1)
            Log_G2G.Append("Impresa " & KeyImpresa_ORIGINE.Piva & " : Trasferimento OK " & vbCrLf)
        Catch ex As Exception
            G2G_Chiusura_Transazione(2)
            Log_G2G.Append("Impresa " & KeyImpresa_ORIGINE.Piva & " : Trasferimento non riuscito !!! " & vbCrLf)
        Finally
            'parte da fare per tutte le imprese dopo il salvataggio dell'ultima impresa.
            '  Marco Grilli, 28/08/2014 13:10:23: GUARDARE DENTRO LA FUNZIONE, C'E' ALTRO DA RIMUOVERE
            If i_corrente = num_tot_imprese Then
                G2G_Gestione_DopoSalvataggioUltimaImpresa(objOpzioniGlobali, ErrFLAG, Log_G2G, Log_Errori, Log_Riepilogo, i_corrente, num_tot_imprese)

                If ErrFLAG <> 0 Then
                    Throw New Exception
                End If
            End If

        End Try

    End Sub

    ''' <summary>
    ''' Legge la rispettiva piva in destinazione, basando la ricerca sulle impostazioni G2G (piva o codici anagrafe)
    ''' </summary>
    ''' <param name="Piva_Origine"></param>
    ''' <param name="Fittizia"></param>
    ''' <returns></returns>
    Private Function G2G_Leggi_Piva_Destinazione(ByVal Piva_Origine As String, ByVal Fittizia As Boolean) As String

        Dim codiceImpresaOrigine As String = Piva_Origine
        Dim pivaImpresaDestinazione As String = ""

        'ricavo il codice impresa origine per usarlo in seguito...
        If _opzioni.CodiceImpresa_ORIGINE <> 0 Then

            Dim objImpreseCodici As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
            Dim codiceImpresa = objImpreseCodici.Leggi_Codice_from_Imprese_Codici(Piva_Origine, _opzioni.CodiceImpresa_ORIGINE, _opzioni.objParametri_Server_GIAS_ORIGINE)

            ' Se non trovo il codice impresa uso la piva origine per ricavare il codice azienda destinazione
            If Not String.IsNullOrEmpty(codiceImpresa) Then
                codiceImpresaOrigine = codiceImpresa
            Else
                Throw New Exception("G2G_Leggi_Piva_Destinazione: Si è richiesta la decodiifica della p.iva attraverso un codice anagrafe inesistente per l'impresa con piva: " & Piva_Origine)
            End If

        End If

        ' cerco il codice impresa origine sulla destinazione per farmi restituire la piva corrispondente, se non la trova restituisce una piva fittizia
        If _opzioni.CodiceImpresa_DESTINAZIONE <> 0 Then

            Dim Str_Credenziali_WS As String = Gias2Gias_LIB.Funzioni.Get_Str_Credenziali_WS(_opzioni)
            Dim strErr As String = ""
            Dim ws As New WS_Importa_GIAS_2014.ImportaWS
            ws.Url = _opzioni.wsimportaGiasURl
            ws.Timeout = Integer.MaxValue
            pivaImpresaDestinazione = ws.Verifica_EsistenzaPivaGIAS_G2G(Str_Credenziali_WS, codiceImpresaOrigine, _opzioni.CodiceImpresa_DESTINAZIONE, Fittizia, strErr)

            'se l'impresa non esiste per niente in destinazione allora la piva è quella di origine
            If String.IsNullOrEmpty(pivaImpresaDestinazione) Then
                pivaImpresaDestinazione = Piva_Origine
            End If

        Else
            pivaImpresaDestinazione = Piva_Origine

        End If

        Return pivaImpresaDestinazione

    End Function

    ' Ricava la PIVA dell'impresa in destinazione ed eventualmente dell'impresa padre
    Private Function G2G_Recode_Impresa_WS(ByVal KeyImpresa_ORIGINE As clsKeyImpresa, ByRef KeyImpresa_DESTINAZIONE As clsKeyImpresa, ByRef KeyImpresa_Padre_DESTINAZIONE As clsKeyImpresa) As Boolean

        ' x chiamata WS Importa GIAS
        Dim strErr As String = ""
        Dim Str_Credenziali_WS As String = Funzioni.Get_Str_Credenziali_WS(_opzioni)
        Dim ws As New WS_Importa_GIAS_2014.ImportaWS With {.Url = _opzioni.wsimportaGiasURl, .Timeout = Integer.MaxValue}

        ' Se non è indicata la piva destinazione la ricavo dalla mappatura azienda
        If String.IsNullOrEmpty(KeyImpresa_DESTINAZIONE.Piva) Then
            'se la p.iva è fittizia in origine, allora non la posso inviare tal quale, perchè in destinazione qualcuno
            'potrebbe avere richiesto una piva fittizia con la stessa numerazione per un'azienda differente.
            'imposto quindi il flag "Fittizia" a true: questo farà sì che, se non esiste la piva in destinazione, viene creata una piva fittizia
            'con opportuna numerazione in destinazione
            Dim Fittizia As Boolean = KeyImpresa_ORIGINE.Piva.StartsWith("F")
            KeyImpresa_DESTINAZIONE.Piva = G2G_Leggi_Piva_Destinazione(KeyImpresa_ORIGINE.Piva, Fittizia)
        End If

        'Verifico se l'impresa scelta esiste gia' in DESTINAZIONE
        Dim esisteImpresaDestinazione As Boolean = ws.Verifica_EsistenzaPivaGIAS(Str_Credenziali_WS, KeyImpresa_DESTINAZIONE.Piva, strErr)
        '  Marco Grilli, 20/08/2014 16:29:19: se ho ricevuto un errore, piuttosto che duplicare tutto, esco.
        If strErr <> "" Then
            Throw New Exception
        End If

        ' Se non è indicata la piva padre destinazione la ricavo dalla mappatura azienda padre in origine
        If String.IsNullOrEmpty(KeyImpresa_Padre_DESTINAZIONE.Piva) Then
            Dim objGera As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R
            Dim Piva_Padre As String = objGera.LeggiPadre(KeyImpresa_ORIGINE.Piva, _opzioni.objParametri_Server_GIAS_ORIGINE, "")
            If Not String.IsNullOrEmpty(Piva_Padre) Then
                KeyImpresa_Padre_DESTINAZIONE.Piva = G2G_Leggi_Piva_Destinazione(Piva_Padre, False)
            End If
        End If

        ' Verifico se esiste l'impresa padre nel gias destinazione altrimenti imposto padre = superpiva destinazione
        If String.IsNullOrEmpty(KeyImpresa_Padre_DESTINAZIONE.Piva) OrElse Not ws.Verifica_EsistenzaPivaGIAS(Str_Credenziali_WS, KeyImpresa_Padre_DESTINAZIONE.Piva, strErr) Then
            KeyImpresa_Padre_DESTINAZIONE.Piva = _opzioni.SuperUser_CodFiscale_DESTINAZIONE
        End If

        Return esisteImpresaDestinazione

    End Function

    Private Sub G2G_Gestione_Impresa_WS(
                                    ByVal objOpzioniGlobali As clsDatiGlobali,
                                    ByVal listaImprese As List(Of clsImpresa),
                                    ByVal objOpzioniImportImpresa As clsImpresa,
                                    ByVal objOpzioni As Gias2Gias_LIB.clsOpzioni,
                                    ByRef ErrFLAG As Integer,
                                    ByRef Log_G2G As StringBuilder,
                                    ByRef Log_Errori As StringBuilder,
                                    ByRef Log_Riepilogo As StringBuilder,
                                    ByVal i_corrente As Integer,
                                    ByVal num_tot_imprese As Integer,
                                    ByVal NomeCartella As String,
                                    ByVal nomeFileLog As String,
                                    ByVal nomeFileErrori As String,
                                    ByRef messaggioDiRitorno As StringBuilder)

        _opzioni = objOpzioni
        _objOpzioniImportSingolaImpresa = objOpzioniImportImpresa
        _Log_G2G = Log_G2G
        _Log_Errori = Log_Errori
        _Log_Riepilogo = Log_Riepilogo

        Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
        Dim ConnectionString As String = gefutils.GetEntityConnectionString(_opzioni.objParametri_Server_GIAS_ORIGINE.StringaConnessione)
        Dim efG2G As New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(ConnectionString)

        Dim nuovaLogicaRecode As Boolean = True
        Dim nuovaLogicaRecodePC As Boolean = objOpzioniImportImpresa.nuovalogica_pianocolturale OrElse Not objOpzioniImportImpresa.Flagimporta_pianocolturale
        Dim nuovaLogicaRecodeAG As Boolean = objOpzioniImportImpresa.nuovalogica_agenda OrElse Not objOpzioniImportImpresa.flagimporta_agenda
        _objScriviHelper = New Gias2Gias_LIB.Funzioni(objOpzioni.objParametri_Server_GIAS_ORIGINE, efG2G, nuovaLogicaRecode, nuovaLogicaRecodePC, nuovaLogicaRecodeAG, objOpzioni.TimeOut_Chiamata_WS)
        _objFunzioni_Global = New Gias2Gias_LIB.FunzioniGLOBAL(objOpzioni.objParametri_Server_GIAS_ORIGINE, objOpzioni.TimeOut_Chiamata_WS)
        _objScriviHelper.FunzioniGLOBAL = _objFunzioni_Global

        '  Vanni, 19/05/2014 16:19:00: caricamento di tutte le ri-codifiche ...
        _objScriviHelper.Recode_Caricamento(efG2G)

        Dim KeyImpresa_ORIGINE As New clsKeyImpresa()
        Dim KeyImpresa_DESTINAZIONE As New clsKeyImpresa()
        Dim KeyImpresa_Padre_DESTINAZIONE As New clsKeyImpresa()

        KeyImpresa_ORIGINE.Piva = _objOpzioniImportSingolaImpresa.Piva_ORIGINE
        KeyImpresa_DESTINAZIONE.Piva = _objOpzioniImportSingolaImpresa.Piva_DESTINAZIONE
        KeyImpresa_Padre_DESTINAZIONE.Piva = _objOpzioniImportSingolaImpresa.PivaPadre_DESTINAZIONE

        '---------------------------------
        'Inizializzazioni
        Dim CodiceGias_ORIGINE As New clsCodiceGias(_opzioni.ProgressivoGIAS_ORIGINE)
        Dim CodiceGias_DESTINAZIONE As New clsCodiceGias(_opzioni.ProgressivoGIAS_DESTINAZIONE)

        ErrFLAG = 0

        Try

            '------------------------------ DATI GLOBALI

            If i_corrente = 1 Then

                Log_G2G.AppendLine(vbCrLf & "----- Dati Globali -----")

                ' Trasferisce contatti pubblici
                If ErrFLAG = 0 AndAlso objOpzioniGlobali.Flagimporta_contatti Then
                    Dim oConfigurazione = ConfigurazioneG2GLeggiContatti(objOpzioniGlobali, objOpzioni, listaImprese)
                    G2G_Copia_Contatti_Pubblico_WS(ErrFLAG, oConfigurazione)
                    Logga(NomeCartella, nomeFileLog, nomeFileErrori, Log_G2G, Log_Errori, messaggioDiRitorno)
                End If

                ' Trasferisce parco macchine pubblico
                If ErrFLAG = 0 AndAlso objOpzioniGlobali.Flagimporta_macchine Then
                    Dim oConfigurazione = ConfigurazioneG2GLeggiMacchine(objOpzioniGlobali, objOpzioni, listaImprese)
                    G2G_Copia_ParcoMacchine_Pubblico_WS(ErrFLAG, oConfigurazione)
                    Logga(NomeCartella, nomeFileLog, nomeFileErrori, Log_G2G, Log_Errori, messaggioDiRitorno)
                End If

                ' Trasferisce materie prime pubbliche
                If ErrFLAG = 0 AndAlso objOpzioniGlobali.Flagimporta_materieprime Then
                    Dim oConfigurazione = ConfigurazioneG2GLeggiMateriePrime(objOpzioniGlobali, objOpzioni, listaImprese)
                    G2G_Copia_MateriePrime_Pubblico_WS(ErrFLAG, oConfigurazione)
                    Logga(NomeCartella, nomeFileLog, nomeFileErrori, Log_G2G, Log_Errori, messaggioDiRitorno)
                End If

                ' Trasferisce Materie Prime Campionature
                If ErrFLAG = 0 AndAlso objOpzioniGlobali.Flagimporta_materieprimecampionature Then
                    Dim oConfigurazione = ConfigurazioneG2GLeggiMateriePrimeCampionature(objOpzioniGlobali, objOpzioni, listaImprese)
                    G2G_Copia_Materie_Prime_Campionature_Pubblico_WS(ErrFLAG, oConfigurazione)
                    Logga(NomeCartella, nomeFileLog, nomeFileErrori, Log_G2G, Log_Errori, messaggioDiRitorno)
                End If

                ' Trasferisce Analisi condivise
                If ErrFLAG = 0 AndAlso objOpzioniGlobali.Flagimporta_analisicondivise Then
                    Dim oConfigurazione = ConfigurazioneG2GLeggiAnalisiCondivise(objOpzioniGlobali, objOpzioni, listaImprese)
                    G2G_Copia_Analisi_Condivise_Pubblico_WS(ErrFLAG, oConfigurazione)
                    Logga(NomeCartella, nomeFileLog, nomeFileErrori, Log_G2G, Log_Errori, messaggioDiRitorno)
                End If

                If ErrFLAG = 0 AndAlso objOpzioniGlobali.Flagimporta_attivita Then
                    Dim oConfigurazione = ConfigurazioneG2GLeggiAttivita(objOpzioniGlobali, objOpzioniImportImpresa, objOpzioni, listaImprese)
                    G2G_Copia_Attivita_WS(ErrFLAG, oConfigurazione)
                    Logga(NomeCartella, nomeFileLog, nomeFileErrori, Log_G2G, Log_Errori, messaggioDiRitorno)
                End If

            End If

            '------------------------------ ANAGRAFICHE

            If ErrFLAG = 0 Then

                'Verifico se l'impresa scelta esiste in ORIGINE
                Dim leggiImpresa As New AgronicaCoreAnagrafeDAL.Imprese_Read
                If leggiImpresa.VerificaEsistenza_PivaGIAS(KeyImpresa_ORIGINE.Piva, _opzioni.objParametri_Server_GIAS_ORIGINE) Then

                    '' VAnni: 4/7/2019: legge dalle tabelle di ricodifica per impostare la p.iva in destinazione, laddove già ricodificata.
                    KeyImpresa_DESTINAZIONE.Piva = (
                            From i1 In efG2G.G2G_Recode_Imprese
                            Where i1.FROM_Piva = KeyImpresa_ORIGINE.Piva AndAlso
                                  i1.To_PivaSuperUser = _opzioni.Import_CodFiscale_DESTINAZIONE AndAlso
                                  i1.FROM_SaCod = 0
                            Select i1.To_Piva
                            ).FirstOrDefault

                    ' ricava la piva destinazione da usare e verifica se esiste l'impresa in destinazione
                    Dim esisteImpresaDestinazione = G2G_Recode_Impresa_WS(KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, KeyImpresa_Padre_DESTINAZIONE)

                    ' VAnni: 4/7/2019: imposto la piva di destinazione anche nell'oggetto.
                    objOpzioniImportImpresa.Piva_DESTINAZIONE = KeyImpresa_DESTINAZIONE.Piva

                    'Estraggo la Ragione sociale
                    Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
                    Dim rag_soc As String = objImprese.RagSoc_from_Piva(KeyImpresa_ORIGINE.Piva, _opzioni.objParametri_Server_GIAS_ORIGINE)
                    Log_G2G.AppendLine(vbCrLf & "----- Impresa " & i_corrente & " di " & num_tot_imprese & " - " & KeyImpresa_ORIGINE.Piva & " - " & rag_soc & " -----")

                    '------------------------------ IMPRESA
                    If _objScriviHelper.NuovaLogicaRecode Then
                        G2G_Copia_Impresa_WS_EF(KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, KeyImpresa_Padre_DESTINAZIONE, ErrFLAG)
                    Else
                        G2G_Copia_Impresa_WS(KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, KeyImpresa_Padre_DESTINAZIONE, ErrFLAG, esisteImpresaDestinazione)
                    End If

                Else

                    Log_G2G.AppendLine("Impresa " & KeyImpresa_ORIGINE.Piva & " : non esiste nel database di origine. non è stato trasferito alcun dato.")
                    ErrFLAG = 1

                End If

                Logga(NomeCartella, nomeFileLog, nomeFileErrori, Log_G2G, Log_Errori, messaggioDiRitorno)

            End If

            '------------------------------ PRATICHE (Messo qui perché in reg_impianti_codici ci sono dei riferimenti alle pratiche, che vanno rimappati)
            If ErrFLAG = 0 AndAlso _objOpzioniImportSingolaImpresa.flagimporta_pratiche Then
                G2G_Gestione_Pratiche_WS(objOpzioniImportImpresa, efG2G, KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG, enum_TipoOperazioneDB.Scrittura, enum_AnalisiTipo.Analisi_Terreno)

                Logga(NomeCartella, nomeFileLog, nomeFileErrori, Log_G2G, Log_Errori, messaggioDiRitorno)
            End If

            '------------------------------ CENTRI AZIENDALI
            If ErrFLAG = 0 Then
                G2G_Lista_Centri_WS(
                    objOpzioniImportImpresa,
                    KeyImpresa_ORIGINE,
                    KeyImpresa_DESTINAZIONE,
                    CodiceGias_ORIGINE,
                    CodiceGias_DESTINAZIONE,
                    ErrFLAG
                )

                Logga(NomeCartella, nomeFileLog, nomeFileErrori, Log_G2G, Log_Errori, messaggioDiRitorno)

            End If

            ' scrive recode con vecchia logica
            If ErrFLAG = 0 AndAlso (Not _objScriviHelper.NuovaLogicaRecode OrElse Not _objScriviHelper.NuovaLogicaRecodePC) Then
                G2G_ApriTransazione()
                _objScriviHelper.Recode_Salvataggio(efG2G, Log_G2G, Log_Errori, Log_Riepilogo, ErrFLAG)
                G2G_Chiusura_Transazione(If(ErrFLAG = 0, 1, 2))

                Logga(NomeCartella, nomeFileLog, nomeFileErrori, Log_G2G, Log_Errori, messaggioDiRitorno)

            End If

            '------------------------------ CONTATTI - contatti privati dell'azienda
            If ErrFLAG = 0 Then
                G2G_Copia_Contatti_WS(KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG, False, Nothing)

                Logga(NomeCartella, nomeFileLog, nomeFileErrori, Log_G2G, Log_Errori, messaggioDiRitorno)
            End If

            '------------------------------ PARCO MACCHINE - macchine private dell'azienda
            If ErrFLAG = 0 Then
                G2G_Copia_ParcoMacchine_WS(KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG, False, Nothing)

                Logga(NomeCartella, nomeFileLog, nomeFileErrori, Log_G2G, Log_Errori, messaggioDiRitorno)
            End If

            '------------------------------ MATERIE PRIME - materie prime dell'azienda
            If ErrFLAG = 0 AndAlso _objOpzioniImportSingolaImpresa.Flagimporta_materieprime Then
                G2G_Copia_MateriePrime_WS(KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG, False, Nothing)

                Logga(NomeCartella, nomeFileLog, nomeFileErrori, Log_G2G, Log_Errori, messaggioDiRitorno)
            End If

            '------------------------------ AGENDA
            If ErrFLAG = 0 AndAlso _objOpzioniImportSingolaImpresa.flagimporta_agenda Then

                If Not _objScriviHelper.NuovaLogicaRecodeAG Then
                    G2G_ApriTransazione()
                End If

                'cancellazione
                If ErrFLAG = 0 Then
                    G2G_Copia_Agenda(objOpzioniImportImpresa, efG2G, KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG, enum_TipoOperazioneDB.Cancellazione)
                End If
                If ErrFLAG = 0 AndAlso Not _objScriviHelper.NuovaLogicaRecodeAG Then
                    _objScriviHelper.Recode_Salvataggio_Agenda(efG2G, Log_G2G, Log_Errori, Log_Riepilogo, ErrFLAG)
                End If

                'modifica
                If ErrFLAG = 0 Then
                    G2G_Copia_Agenda(objOpzioniImportImpresa, efG2G, KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG, enum_TipoOperazioneDB.Modifica)
                End If
                If ErrFLAG = 0 AndAlso Not _objScriviHelper.NuovaLogicaRecodeAG Then
                    _objScriviHelper.Recode_Salvataggio_Agenda(efG2G, Log_G2G, Log_Errori, Log_Riepilogo, ErrFLAG)
                End If

                'scrittura
                If ErrFLAG = 0 Then
                    G2G_Copia_Agenda(objOpzioniImportImpresa, efG2G, KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG, enum_TipoOperazioneDB.Scrittura)
                End If
                If ErrFLAG = 0 AndAlso Not _objScriviHelper.NuovaLogicaRecodeAG Then
                    _objScriviHelper.Recode_Salvataggio_Agenda(efG2G, Log_G2G, Log_Errori, Log_Riepilogo, ErrFLAG)
                End If

                If ErrFLAG = 0 Then
                    If _objScriviHelper.NuovaLogicaRecodeAG AndAlso objOpzioniImportImpresa.nuovalogica_agenda Then
                        G2G_GestioneRiferimentiAgenda_WS(KeyImpresa_ORIGINE.Piva, objOpzioniGlobali, Log_G2G, Log_Errori, Log_Riepilogo, ErrFLAG)
                    Else
                        G2G_GestioneRiferimentiAgenda(KeyImpresa_ORIGINE.Piva, objOpzioniGlobali, Log_G2G, Log_Errori, Log_Riepilogo, ErrFLAG)
                    End If
                End If

                If Not _objScriviHelper.NuovaLogicaRecodeAG Then
                    G2G_Chiusura_Transazione(If(ErrFLAG = 0, 1, 2))
                End If

                Logga(NomeCartella, nomeFileLog, nomeFileErrori, Log_G2G, Log_Errori, messaggioDiRitorno)

            End If

            '------------------------------ PRATICHE PULL 
            If ErrFLAG = 0 AndAlso _objOpzioniImportSingolaImpresa.flagimporta_pratiche_pull Then
                G2G_Gestione_Pratiche_Pull_WS(objOpzioniImportImpresa, efG2G, KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG, enum_TipoOperazioneDB.Scrittura, enum_AnalisiTipo.Analisi_Terreno)

                Logga(NomeCartella, nomeFileLog, nomeFileErrori, Log_G2G, Log_Errori, messaggioDiRitorno)
            End If

            '------------------------------ PLANNING
            If ErrFLAG = 0 AndAlso _objOpzioniImportSingolaImpresa.flagimporta_planning Then
                G2G_Copia_Planning_WS(objOpzioniImportImpresa, KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG)

                Logga(NomeCartella, nomeFileLog, nomeFileErrori, Log_G2G, Log_Errori, messaggioDiRitorno)
            End If

            '------------------------------ ANALISI
            If ErrFLAG = 0 AndAlso _objOpzioniImportSingolaImpresa.Flagimporta_analisi Then
                G2G_Gestione_ImportaAnalisi_WS(objOpzioniImportImpresa, efG2G, KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG, enum_TipoOperazioneDB.Scrittura, enum_AnalisiTipo.Analisi_Terreno)

                Logga(NomeCartella, nomeFileLog, nomeFileErrori, Log_G2G, Log_Errori, messaggioDiRitorno)
            End If

            '------------------------------ ALLEGATI
            If ErrFLAG = 0 AndAlso _objOpzioniImportSingolaImpresa.flagimporta_allegati Then
                G2G_Gestione_Allegati_WS(objOpzioniImportImpresa, efG2G, KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG, enum_TipoOperazioneDB.Scrittura)

                Logga(NomeCartella, nomeFileLog, nomeFileErrori, Log_G2G, Log_Errori, messaggioDiRitorno)
            End If

            '------------------------------ PIANI DI CONCIMAZIONE
            If ErrFLAG = 0 AndAlso _objOpzioniImportSingolaImpresa.flagimporta_piano_concimazione Then
                G2G_Gestione_ImportaPiano_Concimazione_WS(objOpzioniImportImpresa, efG2G, KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG, enum_TipoOperazioneDB.Scrittura)

                Logga(NomeCartella, nomeFileLog, nomeFileErrori, Log_G2G, Log_Errori, messaggioDiRitorno)
            End If

            '------------------------------ PUA
            If ErrFLAG = 0 AndAlso _objOpzioniImportSingolaImpresa.flagimporta_pua Then
                G2G_Copia_PUA_WS(objOpzioniImportImpresa, KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG)

                Logga(NomeCartella, nomeFileLog, nomeFileErrori, Log_G2G, Log_Errori, messaggioDiRitorno)
            End If

            '------------------------------ ParticelleCatastalixVincoliAgronomici (Serve per il PUA)
            If ErrFLAG = 0 AndAlso _objOpzioniImportSingolaImpresa.flagimporta_pua Then
                G2G_Copia_ParticelleCatastalixVincoliAgronomici_WS(objOpzioniImportImpresa, KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG)

                Logga(NomeCartella, nomeFileLog, nomeFileErrori, Log_G2G, Log_Errori, messaggioDiRitorno)
            End If

            '------------------------------ PUA_LetamazioniPrecedenti (Serve per il PUA)
            If ErrFLAG = 0 AndAlso _objOpzioniImportSingolaImpresa.flagimporta_pua Then
                G2G_Copia_PUA_LetamazioniPrecedenti_WS(objOpzioniImportImpresa, KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG)

                Logga(NomeCartella, nomeFileLog, nomeFileErrori, Log_G2G, Log_Errori, messaggioDiRitorno)
            End If

            '------------------------------ Anagrafe_VincoliAgronomici (Serve per il PUA)
            If ErrFLAG = 0 AndAlso _objOpzioniImportSingolaImpresa.flagimporta_pua Then
                G2G_Copia_Anagrafe_VincoliAgronomici_WS(objOpzioniImportImpresa, KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG)

                Logga(NomeCartella, nomeFileLog, nomeFileErrori, Log_G2G, Log_Errori, messaggioDiRitorno)
            End If

            '------------------------------ PUA_Effluente_PeriodoDivieto (Serve per il PUA)
            If ErrFLAG = 0 AndAlso _objOpzioniImportSingolaImpresa.flagimporta_pua Then
                G2G_Copia_PUA_Effluente_PeriodoDivieto_WS(objOpzioniImportImpresa, KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG)

                Logga(NomeCartella, nomeFileLog, nomeFileErrori, Log_G2G, Log_Errori, messaggioDiRitorno)
            End If

            '------------------------------ RICETTE
            If ErrFLAG = 0 AndAlso _objOpzioniImportSingolaImpresa.flagimporta_ricette Then
                G2G_Gestione_ImportaRicette_WS(objOpzioniImportImpresa, efG2G, KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG, enum_TipoOperazioneDB.Scrittura)

                Logga(NomeCartella, nomeFileLog, nomeFileErrori, Log_G2G, Log_Errori, messaggioDiRitorno)
            End If

            If ErrFLAG = 0 Then
                Log_G2G.AppendLine(CStr(Date.Now) & " - Impresa " & KeyImpresa_ORIGINE.Piva & " : Trasferimento OK ")
            Else
                Log_G2G.AppendLine(CStr(Date.Now) & " - Impresa " & KeyImpresa_ORIGINE.Piva & " : Trasferimento non riuscito !!! ")
            End If

            Logga(NomeCartella, nomeFileLog, nomeFileErrori, Log_G2G, Log_Errori, messaggioDiRitorno)

        Catch ex As Exception

            Log_G2G.AppendLine(CStr(Date.Now) & " - Impresa " & KeyImpresa_ORIGINE.Piva & " : Errore : " & ex.Message)

        End Try

    End Sub

    Private Sub Logga(ByVal NomeCartella As String, nomeFileLog As String, nomeFileErrori As String, ByRef Log_G2G As StringBuilder, ByRef Log_Errori As StringBuilder, ByRef messaggioDiRitorno As StringBuilder)
        My.Computer.FileSystem.WriteAllText(NomeCartella & nomeFileLog, Log_G2G.ToString, True)
        My.Computer.FileSystem.WriteAllText(NomeCartella & nomeFileErrori, Log_Errori.ToString, True)
        messaggioDiRitorno.Append(Log_G2G.ToString)
        Log_G2G.Length = 0
        Log_Errori.Length = 0
    End Sub


    Private Sub G2G_Gestione_Impresa_WS_Reverse(
                                    ByVal objOpzioniGlobali As clsDatiGlobali,
                                    ByVal listaImprese As List(Of clsImpresa),
                                    ByVal objOpzioniImportImpresa As clsImpresa,
                                    ByVal objOpzioni As Gias2Gias_LIB.clsOpzioni,
                                    ByRef ErrFLAG As Integer,
                                    ByRef Log_G2G As StringBuilder,
                                    ByRef Log_Errori As StringBuilder,
                                    ByRef Log_Riepilogo As StringBuilder,
                                    ByVal i_corrente As Integer,
                                    ByVal num_tot_imprese As Integer,
                                    ByVal NomeCartella As String,
                                    ByVal nomeFileLog As String,
                                    ByVal nomeFileErrori As String,
                                    ByRef messaggioDiRitorno As StringBuilder)

        _opzioni = objOpzioni
        _objOpzioniImportSingolaImpresa = objOpzioniImportImpresa
        _Log_G2G = Log_G2G
        _Log_Errori = Log_Errori
        _Log_Riepilogo = Log_Riepilogo

        Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
        Dim ConnectionString As String = gefutils.GetEntityConnectionString(_opzioni.objParametri_Server_GIAS_ORIGINE.StringaConnessione)
        Dim efG2G As New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(ConnectionString)

        Dim nuovaLogicaRecode As Boolean = True
        Dim nuovaLogicaRecodePC As Boolean = objOpzioniImportImpresa.nuovalogica_pianocolturale OrElse Not objOpzioniImportImpresa.Flagimporta_pianocolturale
        Dim nuovaLogicaRecodeAG As Boolean = objOpzioniImportImpresa.nuovalogica_agenda OrElse Not objOpzioniImportImpresa.flagimporta_agenda
        _objScriviHelper = New Gias2Gias_LIB.Funzioni(objOpzioni.objParametri_Server_GIAS_ORIGINE, efG2G, nuovaLogicaRecode, nuovaLogicaRecodePC, nuovaLogicaRecodeAG, objOpzioni.TimeOut_Chiamata_WS)
        _objFunzioni_Global = New Gias2Gias_LIB.FunzioniGLOBAL(objOpzioni.objParametri_Server_GIAS_ORIGINE, objOpzioni.TimeOut_Chiamata_WS)
        _objScriviHelper.FunzioniGLOBAL = _objFunzioni_Global

        '  Vanni, 19/05/2014 16:19:00: caricamento di tutte le ri-codifiche ...
        _objScriviHelper.Recode_Caricamento(efG2G)

        Dim KeyImpresa_ORIGINE As New clsKeyImpresa()
        Dim KeyImpresa_DESTINAZIONE As New clsKeyImpresa()
        Dim KeyImpresa_Padre_DESTINAZIONE As New clsKeyImpresa()

        KeyImpresa_ORIGINE.Piva = _objOpzioniImportSingolaImpresa.Piva_ORIGINE
        KeyImpresa_DESTINAZIONE.Piva = _objOpzioniImportSingolaImpresa.Piva_DESTINAZIONE
        KeyImpresa_Padre_DESTINAZIONE.Piva = _objOpzioniImportSingolaImpresa.PivaPadre_DESTINAZIONE

        '---------------------------------
        'Inizializzazioni
        Dim CodiceGias_ORIGINE As New clsCodiceGias(_opzioni.ProgressivoGIAS_ORIGINE)
        Dim CodiceGias_DESTINAZIONE As New clsCodiceGias(_opzioni.ProgressivoGIAS_DESTINAZIONE)

        ErrFLAG = 0

        Try

            '------------------------------ DATI GLOBALI

            If i_corrente = 1 Then

                Log_G2G.AppendLine(vbCrLf & "----- Dati Globali -----")

                ' Trasferisce contatti pubblici
                If ErrFLAG = 0 AndAlso objOpzioniGlobali.Flagimporta_contatti Then
                    Dim oConfigurazione = ConfigurazioneG2GLeggiContatti(objOpzioniGlobali, objOpzioni, listaImprese)
                    G2G_Copia_Contatti_Pubblico_WSReverse(ErrFLAG, oConfigurazione)
                    Logga(NomeCartella, nomeFileLog, nomeFileErrori, Log_G2G, Log_Errori, messaggioDiRitorno)
                End If

                ' Trasferisce parco macchine pubblico
                If ErrFLAG = 0 AndAlso objOpzioniGlobali.Flagimporta_macchine Then
                    Dim oConfigurazione = ConfigurazioneG2GLeggiMacchine(objOpzioniGlobali, objOpzioni, listaImprese)
                    G2G_Copia_ParcoMacchine_Pubblico_WSReverse(ErrFLAG, oConfigurazione)
                    Logga(NomeCartella, nomeFileLog, nomeFileErrori, Log_G2G, Log_Errori, messaggioDiRitorno)
                End If

                ' Trasferisce materie prime pubbliche
                If ErrFLAG = 0 AndAlso objOpzioniGlobali.Flagimporta_materieprime Then
                    Dim oConfigurazione = ConfigurazioneG2GLeggiMateriePrime(objOpzioniGlobali, objOpzioni, listaImprese)
                    G2G_Copia_MateriePrime_Pubblico_WSReverse(ErrFLAG, oConfigurazione)
                    Logga(NomeCartella, nomeFileLog, nomeFileErrori, Log_G2G, Log_Errori, messaggioDiRitorno)
                End If

                ' Trasferisce Materie Prime Campionature
                If ErrFLAG = 0 AndAlso objOpzioniGlobali.Flagimporta_materieprimecampionature Then
                    Dim oConfigurazione = ConfigurazioneG2GLeggiMateriePrimeCampionature(objOpzioniGlobali, objOpzioni, listaImprese)
                    G2G_Copia_Materie_Prime_Campionature_Pubblico_WSReverse(ErrFLAG, oConfigurazione)
                    Logga(NomeCartella, nomeFileLog, nomeFileErrori, Log_G2G, Log_Errori, messaggioDiRitorno)
                End If

                ' Trasferisce Analisi condivise
                If ErrFLAG = 0 AndAlso objOpzioniGlobali.Flagimporta_analisicondivise Then
                    Dim oConfigurazione = ConfigurazioneG2GLeggiAnalisiCondivise(objOpzioniGlobali, objOpzioni, listaImprese)
                    G2G_Copia_Analisi_Condivise_Pubblico_WSReverse(ErrFLAG, oConfigurazione)
                    Logga(NomeCartella, nomeFileLog, nomeFileErrori, Log_G2G, Log_Errori, messaggioDiRitorno)
                End If

                If ErrFLAG = 0 AndAlso objOpzioniGlobali.Flagimporta_attivita Then
                    Dim oConfigurazione = ConfigurazioneG2GLeggiAttivita(objOpzioniGlobali, objOpzioniImportImpresa, objOpzioni, listaImprese)
                    G2G_Copia_Attivita_WSReverse(ErrFLAG, oConfigurazione)
                    Logga(NomeCartella, nomeFileLog, nomeFileErrori, Log_G2G, Log_Errori, messaggioDiRitorno)
                End If

            End If

            '------------------------------ ANAGRAFICHE
            If ErrFLAG = 0 Then

                'Verifico se l'impresa scelta esiste in ORIGINE
                Dim leggiImpresa As New AgronicaCoreAnagrafeDAL.Imprese_Read
                If leggiImpresa.VerificaEsistenza_PivaGIAS(KeyImpresa_ORIGINE.Piva, _opzioni.objParametri_Server_GIAS_ORIGINE) Then

                    '' VAnni: 4/7/2019: legge dalle tabelle di ricodifica per impostare la p.iva in destinazione, laddove già ricodificata.
                    KeyImpresa_DESTINAZIONE.Piva = (
                            From i1 In efG2G.G2G_Recode_Imprese
                            Where i1.FROM_Piva = KeyImpresa_ORIGINE.Piva AndAlso
                                  i1.From_PivaSuperUser = _opzioni.Import_CodFiscale_DESTINAZIONE AndAlso
                                  i1.FROM_SaCod = 0
                            Select i1.To_Piva
                            ).FirstOrDefault

                    ' ricava la piva destinazione da usare e verifica se esiste l'impresa in destinazione
                    Dim esisteImpresaDestinazione = G2G_Recode_Impresa_WS(KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, KeyImpresa_Padre_DESTINAZIONE)

                    ' VAnni: 4/7/2019: imposto la piva di destinazione anche nell'oggetto.
                    objOpzioniImportImpresa.Piva_DESTINAZIONE = KeyImpresa_DESTINAZIONE.Piva

                    'Estraggo la Ragione sociale
                    Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
                    Dim rag_soc As String = objImprese.RagSoc_from_Piva(KeyImpresa_ORIGINE.Piva, _opzioni.objParametri_Server_GIAS_ORIGINE)
                    Log_G2G.AppendLine(vbCrLf & "----- Impresa " & i_corrente & " di " & num_tot_imprese & " - " & KeyImpresa_ORIGINE.Piva & " - " & rag_soc & " -----")

                    '------------------------------ IMPRESA
                    If _objScriviHelper.NuovaLogicaRecode Then
                        G2G_Copia_Impresa_WS_EF_Reverse(KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, KeyImpresa_Padre_DESTINAZIONE, ErrFLAG)
                    Else
                        'G2G_Copia_Impresa_WS(KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, KeyImpresa_Padre_DESTINAZIONE, ErrFLAG, esisteImpresaDestinazione)
                        Throw New Exception("Vecchia modalità Reverse non gestita")
                    End If

                Else

                    Log_G2G.AppendLine("Impresa " & KeyImpresa_ORIGINE.Piva & " : non esiste nel database di origine. non è stato trasferito alcun dato.")
                    ErrFLAG = 1

                End If

            End If

            '------------------------------ PRATICHE (Messo qui perché in reg_impianti_codici ci sono dei riferimenti alle pratiche, che vanno rimappati)
            If ErrFLAG = 0 AndAlso _objOpzioniImportSingolaImpresa.flagimporta_pratiche Then
                G2G_Gestione_Pratiche_WS_Reverse(objOpzioniImportImpresa, efG2G, KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG, enum_TipoOperazioneDB.Scrittura, enum_AnalisiTipo.Analisi_Terreno)
                Logga(NomeCartella, nomeFileLog, nomeFileErrori, Log_G2G, Log_Errori, messaggioDiRitorno)
            End If

            '------------------------------ CENTRI AZIENDALI
            If ErrFLAG = 0 Then
                G2G_Lista_Centri_WSReverse(
                    objOpzioniImportImpresa,
                    KeyImpresa_ORIGINE,
                    KeyImpresa_DESTINAZIONE,
                    CodiceGias_ORIGINE,
                    CodiceGias_DESTINAZIONE,
                    ErrFLAG
                )
                Logga(NomeCartella, nomeFileLog, nomeFileErrori, Log_G2G, Log_Errori, messaggioDiRitorno)
            End If

            '------------------------------ CONTATTI - contatti privati dell'azienda
            If ErrFLAG = 0 Then
                G2G_Copia_Contatti_WSReverse(KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG, False, Nothing)
                Logga(NomeCartella, nomeFileLog, nomeFileErrori, Log_G2G, Log_Errori, messaggioDiRitorno)
            End If

            '------------------------------ PARCO MACCHINE - macchine private dell'azienda
            If ErrFLAG = 0 Then
                G2G_Copia_ParcoMacchine_WSReverse(KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG, False, Nothing)
                Logga(NomeCartella, nomeFileLog, nomeFileErrori, Log_G2G, Log_Errori, messaggioDiRitorno)
            End If

            '------------------------------ MATERIE PRIME - materie prime dell'azienda
            If ErrFLAG = 0 AndAlso _objOpzioniImportSingolaImpresa.Flagimporta_materieprime Then
                G2G_Copia_MateriePrime_WSReverse(KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG, False, Nothing)
                Logga(NomeCartella, nomeFileLog, nomeFileErrori, Log_G2G, Log_Errori, messaggioDiRitorno)
            End If

            '------------------------------ AGENDA
            If ErrFLAG = 0 AndAlso _objOpzioniImportSingolaImpresa.flagimporta_agenda Then

                'cancellazione
                If ErrFLAG = 0 Then
                    G2G_Copia_AgendaReverse(objOpzioniImportImpresa, efG2G, KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG, enum_TipoOperazioneDB.Cancellazione)
                End If

                'modifica
                If ErrFLAG = 0 Then
                    G2G_Copia_AgendaReverse(objOpzioniImportImpresa, efG2G, KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG, enum_TipoOperazioneDB.Modifica)
                End If

                'scrittura
                If ErrFLAG = 0 Then
                    G2G_Copia_AgendaReverse(objOpzioniImportImpresa, efG2G, KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG, enum_TipoOperazioneDB.Scrittura)
                End If

                If ErrFLAG = 0 Then
                    G2G_GestioneRiferimentiAgenda_WSReverse(KeyImpresa_ORIGINE.Piva, objOpzioniGlobali, Log_G2G, Log_Errori, Log_Riepilogo, ErrFLAG)
                End If
                Logga(NomeCartella, nomeFileLog, nomeFileErrori, Log_G2G, Log_Errori, messaggioDiRitorno)
            End If

            ''------------------------------ PRATICHE PULL 
            'If ErrFLAG = 0 AndAlso _objOpzioniImportSingolaImpresa.flagimporta_pratiche_pull Then
            '    G2G_Gestione_Pratiche_Pull_WS(objOpzioniImportImpresa, efG2G, KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG, enum_TipoOperazioneDB.Scrittura, enum_AnalisiTipo.Analisi_Terreno)
            'End If

            '------------------------------ PLANNING
            If ErrFLAG = 0 AndAlso _objOpzioniImportSingolaImpresa.flagimporta_planning Then
                G2G_Copia_Planning_WSReverse(objOpzioniImportImpresa, KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG)
                Logga(NomeCartella, nomeFileLog, nomeFileErrori, Log_G2G, Log_Errori, messaggioDiRitorno)
            End If

            '------------------------------ ANALISI
            If ErrFLAG = 0 AndAlso _objOpzioniImportSingolaImpresa.Flagimporta_analisi Then
                G2G_Gestione_ImportaAnalisi_WSReverse(objOpzioniImportImpresa, efG2G, KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG, enum_TipoOperazioneDB.Scrittura, enum_AnalisiTipo.Analisi_Terreno)
                Logga(NomeCartella, nomeFileLog, nomeFileErrori, Log_G2G, Log_Errori, messaggioDiRitorno)
            End If

            '------------------------------ ALLEGATI
            If ErrFLAG = 0 AndAlso _objOpzioniImportSingolaImpresa.flagimporta_allegati Then
                G2G_Gestione_Allegati_WSReverse(objOpzioniImportImpresa, efG2G, KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG, enum_TipoOperazioneDB.Scrittura)
                Logga(NomeCartella, nomeFileLog, nomeFileErrori, Log_G2G, Log_Errori, messaggioDiRitorno)
            End If

            '------------------------------ PIANI DI CONCIMAZIONE
            If ErrFLAG = 0 AndAlso _objOpzioniImportSingolaImpresa.flagimporta_piano_concimazione Then
                G2G_Gestione_ImportaPiano_Concimazione_WSReverse(objOpzioniImportImpresa, efG2G, KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG, enum_TipoOperazioneDB.Scrittura)
                Logga(NomeCartella, nomeFileLog, nomeFileErrori, Log_G2G, Log_Errori, messaggioDiRitorno)
            End If

            '------------------------------ PUA
            If ErrFLAG = 0 AndAlso _objOpzioniImportSingolaImpresa.flagimporta_pua Then
                G2G_Copia_PUA_WSReverse(objOpzioniImportImpresa, KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG)
                Logga(NomeCartella, nomeFileLog, nomeFileErrori, Log_G2G, Log_Errori, messaggioDiRitorno)
            End If

            '------------------------------ ParticelleCatastalixVincoliAgronomici (Serve per il PUA)
            If ErrFLAG = 0 AndAlso _objOpzioniImportSingolaImpresa.flagimporta_pua Then
                G2G_Copia_ParticelleCatastalixVincoliAgronomici_WSReverse(objOpzioniImportImpresa, KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG)
                Logga(NomeCartella, nomeFileLog, nomeFileErrori, Log_G2G, Log_Errori, messaggioDiRitorno)
            End If

            '------------------------------ PUA_LetamazioniPrecedenti (Serve per il PUA)
            If ErrFLAG = 0 AndAlso _objOpzioniImportSingolaImpresa.flagimporta_pua Then
                G2G_Copia_PUA_LetamazioniPrecedenti_WSReverse(objOpzioniImportImpresa, KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG)
                Logga(NomeCartella, nomeFileLog, nomeFileErrori, Log_G2G, Log_Errori, messaggioDiRitorno)
            End If

            '------------------------------ Anagrafe_VincoliAgronomici (Serve per il PUA)
            If ErrFLAG = 0 AndAlso _objOpzioniImportSingolaImpresa.flagimporta_pua Then
                G2G_Copia_Anagrafe_VincoliAgronomici_WSReverse(objOpzioniImportImpresa, KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG)
                Logga(NomeCartella, nomeFileLog, nomeFileErrori, Log_G2G, Log_Errori, messaggioDiRitorno)
            End If

            '------------------------------ RICETTE
            If ErrFLAG = 0 AndAlso _objOpzioniImportSingolaImpresa.flagimporta_ricette Then
                G2G_Gestione_ImportaRicette_WSReverse(objOpzioniImportImpresa, efG2G, KeyImpresa_ORIGINE, KeyImpresa_DESTINAZIONE, ErrFLAG, enum_TipoOperazioneDB.Scrittura)
                Logga(NomeCartella, nomeFileLog, nomeFileErrori, Log_G2G, Log_Errori, messaggioDiRitorno)
            End If

            If ErrFLAG = 0 Then
                Log_G2G.AppendLine(CStr(Date.Now) & " - Impresa " & KeyImpresa_ORIGINE.Piva & " : Trasferimento OK ")
            Else
                Log_G2G.AppendLine(CStr(Date.Now) & " - Impresa " & KeyImpresa_ORIGINE.Piva & " : Trasferimento non riuscito !!! ")
            End If

        Catch ex As Exception

            Log_G2G.AppendLine(CStr(Date.Now) & " - Impresa " & KeyImpresa_ORIGINE.Piva & " : Errore : " & ex.Message)

        End Try

    End Sub

    Private Sub Chiama_G2G_Gestione_Impresa_WS_Reverse(
                                    ByVal objOpzioniGlobali As clsDatiGlobali,
                                    ByVal listaImprese As List(Of clsImpresa),
                                    ByVal objOpzioniImportImpresa As clsImpresa,
                                    ByVal objOpzioni As Gias2Gias_LIB.clsOpzioni,
                                    ByRef ErrFLAG As Integer,
                                    ByRef Log_G2G As StringBuilder,
                                    ByRef Log_Errori As StringBuilder,
                                    ByRef Log_Riepilogo As StringBuilder,
                                    ByVal i_corrente As Integer,
                                    ByVal num_tot_imprese As Integer,
                                    Gestore As Integer,
                                    ByRef Messaggio_di_Ritorno As String)

        _opzioni = objOpzioni
        _objOpzioniImportSingolaImpresa = objOpzioniImportImpresa
        _Log_G2G = Log_G2G
        _Log_Errori = Log_Errori
        _Log_Riepilogo = Log_Riepilogo

        Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
        Dim ConnectionString As String = gefutils.GetEntityConnectionString(_opzioni.objParametri_Server_GIAS_ORIGINE.StringaConnessione)
        Dim efG2G As New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(ConnectionString)

        Dim nuovaLogicaRecode As Boolean = True
        Dim nuovaLogicaRecodePC As Boolean = objOpzioniImportImpresa.nuovalogica_pianocolturale OrElse Not objOpzioniImportImpresa.Flagimporta_pianocolturale
        Dim nuovaLogicaRecodeAG As Boolean = objOpzioniImportImpresa.nuovalogica_agenda OrElse Not objOpzioniImportImpresa.flagimporta_agenda
        _objScriviHelper = New Gias2Gias_LIB.Funzioni(objOpzioni.objParametri_Server_GIAS_ORIGINE, efG2G, nuovaLogicaRecode, nuovaLogicaRecodePC, nuovaLogicaRecodeAG, objOpzioni.TimeOut_Chiamata_WS)
        _objFunzioni_Global = New Gias2Gias_LIB.FunzioniGLOBAL(objOpzioni.objParametri_Server_GIAS_ORIGINE, objOpzioni.TimeOut_Chiamata_WS)
        _objScriviHelper.FunzioniGLOBAL = _objFunzioni_Global

        '  Vanni, 19/05/2014 16:19:00: caricamento di tutte le ri-codifiche ...
        _objScriviHelper.Recode_Caricamento(efG2G)

        Dim KeyImpresa_ORIGINE As New clsKeyImpresa()
        Dim KeyImpresa_DESTINAZIONE As New clsKeyImpresa()
        Dim KeyImpresa_Padre_DESTINAZIONE As New clsKeyImpresa()

        KeyImpresa_ORIGINE.Piva = _objOpzioniImportSingolaImpresa.Piva_ORIGINE
        KeyImpresa_DESTINAZIONE.Piva = _objOpzioniImportSingolaImpresa.Piva_DESTINAZIONE
        KeyImpresa_Padre_DESTINAZIONE.Piva = _objOpzioniImportSingolaImpresa.PivaPadre_DESTINAZIONE

        '---------------------------------
        'Inizializzazioni
        Dim CodiceGias_ORIGINE As New clsCodiceGias(_opzioni.ProgressivoGIAS_ORIGINE)
        Dim CodiceGias_DESTINAZIONE As New clsCodiceGias(_opzioni.ProgressivoGIAS_DESTINAZIONE)

        ErrFLAG = 0

        Try

            Try

                _objScriviHelper.Elabora_XML_Imprese_Salva_Reverse(_opzioni, Log_G2G, Log_Errori, Log_Riepilogo, KeyImpresa_ORIGINE.Piva, Gestore, "", Messaggio_di_Ritorno)

            Catch ex As Exception
                ErrFLAG = 1
                _Log_G2G.AppendLine("copia Analisi: " & ex.Message.ToString)
            End Try

            If ErrFLAG = 0 Then
                Log_G2G.AppendLine(CStr(Date.Now) & " - Impresa " & KeyImpresa_ORIGINE.Piva & " : Trasferimento OK ")
            Else
                Log_G2G.AppendLine(CStr(Date.Now) & " - Impresa " & KeyImpresa_ORIGINE.Piva & " : Trasferimento non riuscito !!! ")
            End If

        Catch ex As Exception

            Log_G2G.AppendLine(CStr(Date.Now) & " - Impresa " & KeyImpresa_ORIGINE.Piva & " : Errore : " & ex.Message)

        End Try

    End Sub

    Private Sub G2G_Gestione_ImportaAnalisi_WS(ByVal objOpzioniImportImpresa As clsImpresa,
        ByVal efG2G As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
        ByVal KeyImpresa_origine As clsKeyImpresa,
        ByRef KeyImpresa_Destinazione As clsKeyImpresa,
        ByRef ErrFLAG As Integer,
        ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
        ByVal lEnum_AnalisiTipo As enum_AnalisiTipo
    )

        Try
            _objScriviHelper.Elabora_XML_Analisi_Salva(
                objOpzioniImportImpresa,
                efG2G,
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyImpresa_origine.Piva,
                KeyImpresa_Destinazione.Piva,
                TipoOperazioneDB
            )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine("copia Analisi: " & ex.Message.ToString)
        End Try

    End Sub

    Private Sub G2G_Gestione_ImportaAnalisi_WSReverse(ByVal objOpzioniImportImpresa As clsImpresa,
        ByVal efG2G As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
        ByVal KeyImpresa_origine As clsKeyImpresa,
        ByRef KeyImpresa_Destinazione As clsKeyImpresa,
        ByRef ErrFLAG As Integer,
        ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
        ByVal lEnum_AnalisiTipo As enum_AnalisiTipo
    )

        Try
            _objScriviHelper.Elabora_XML_Analisi_SalvaReverse(
                objOpzioniImportImpresa,
                efG2G,
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyImpresa_origine.Piva,
                KeyImpresa_Destinazione.Piva,
                TipoOperazioneDB
            )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine("copia Analisi: " & ex.Message.ToString)
        End Try

    End Sub


    Private Sub G2G_Gestione_ImportaPiano_Concimazione_WS(ByVal objOpzioniImportImpresa As clsImpresa,
        ByVal efG2G As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
        ByVal KeyImpresa_origine As clsKeyImpresa,
        ByRef KeyImpresa_Destinazione As clsKeyImpresa,
        ByRef ErrFLAG As Integer,
        ByVal TipoOperazioneDB As enum_TipoOperazioneDB
    )

        Try
            _objScriviHelper.Elabora_XML_Piano_Concimazione_Salva(
                objOpzioniImportImpresa,
                efG2G,
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyImpresa_origine.Piva,
                KeyImpresa_Destinazione.Piva,
                TipoOperazioneDB
            )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine("copia Piano Concimazione: " & ex.Message.ToString)
        End Try

    End Sub

    Private Sub G2G_Gestione_ImportaPiano_Concimazione_WSReverse(ByVal objOpzioniImportImpresa As clsImpresa,
        ByVal efG2G As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
        ByVal KeyImpresa_origine As clsKeyImpresa,
        ByRef KeyImpresa_Destinazione As clsKeyImpresa,
        ByRef ErrFLAG As Integer,
        ByVal TipoOperazioneDB As enum_TipoOperazioneDB
    )

        Try
            _objScriviHelper.Elabora_XML_Piano_Concimazione_SalvaReverse(
                objOpzioniImportImpresa,
                efG2G,
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyImpresa_origine.Piva,
                KeyImpresa_Destinazione.Piva,
                TipoOperazioneDB
            )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine("copia Piano Concimazione: " & ex.Message.ToString)
        End Try

    End Sub

    Private Sub G2G_Gestione_ImportaRicette_WS(ByVal objOpzioniImportImpresa As clsImpresa,
        ByVal efG2G As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
        ByVal KeyImpresa_origine As clsKeyImpresa,
        ByRef KeyImpresa_Destinazione As clsKeyImpresa,
        ByRef ErrFLAG As Integer,
        ByVal TipoOperazioneDB As enum_TipoOperazioneDB
    )

        Try
            _objScriviHelper.Elabora_XML_Ricette_Salva(
                objOpzioniImportImpresa,
                efG2G,
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyImpresa_origine.Piva,
                KeyImpresa_Destinazione.Piva,
                TipoOperazioneDB
            )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine("copia Ricette: " & ex.Message.ToString)
        End Try

    End Sub

    Private Sub G2G_Gestione_ImportaRicette_WSReverse(ByVal objOpzioniImportImpresa As clsImpresa,
        ByVal efG2G As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
        ByVal KeyImpresa_origine As clsKeyImpresa,
        ByRef KeyImpresa_Destinazione As clsKeyImpresa,
        ByRef ErrFLAG As Integer,
        ByVal TipoOperazioneDB As enum_TipoOperazioneDB
    )

        Try
            _objScriviHelper.Elabora_XML_Ricette_Salva_Reverse(
                objOpzioniImportImpresa,
                efG2G,
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyImpresa_origine.Piva,
                KeyImpresa_Destinazione.Piva,
                TipoOperazioneDB
            )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine("copia Ricette: " & ex.Message.ToString)
        End Try

    End Sub

    Private Sub G2G_Gestione_Pratiche_WS(ByVal objOpzioniImportImpresa As clsImpresa,
        ByVal efG2G As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
        ByVal KeyImpresa_origine As clsKeyImpresa,
        ByRef KeyImpresa_Destinazione As clsKeyImpresa,
        ByRef ErrFLAG As Integer,
        ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
        ByVal lEnum_AnalisiTipo As enum_AnalisiTipo
    )

        Try
            _objScriviHelper.Elabora_XML_Pratiche_Salva(
                objOpzioniImportImpresa,
                efG2G,
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyImpresa_origine.Piva,
                KeyImpresa_Destinazione.Piva,
                TipoOperazioneDB
            )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine("copia Pratiche: " & ex.Message.ToString)
        End Try

    End Sub

    Private Sub G2G_Gestione_Pratiche_WS_Reverse(ByVal objOpzioniImportImpresa As clsImpresa,
        ByVal efG2G As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
        ByVal KeyImpresa_origine As clsKeyImpresa,
        ByRef KeyImpresa_Destinazione As clsKeyImpresa,
        ByRef ErrFLAG As Integer,
        ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
        ByVal lEnum_AnalisiTipo As enum_AnalisiTipo
    )

        Try
            _objScriviHelper.Elabora_XML_Pratiche_Salva_Reverse(
                objOpzioniImportImpresa,
                efG2G,
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyImpresa_origine.Piva,
                KeyImpresa_Destinazione.Piva,
                TipoOperazioneDB
            )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine("copia Pratiche: " & ex.Message.ToString)
        End Try

    End Sub

    Private Sub G2G_Gestione_Pratiche_Pull_WS(ByVal objOpzioniImportImpresa As clsImpresa,
        ByVal efG2G As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
        ByVal KeyImpresa_origine As clsKeyImpresa,
        ByRef KeyImpresa_Destinazione As clsKeyImpresa,
        ByRef ErrFLAG As Integer,
        ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
        ByVal lEnum_AnalisiTipo As enum_AnalisiTipo
    )

        Try
            _objScriviHelper.Elabora_XML_Pratiche_Pull_Salva(
                objOpzioniImportImpresa,
                efG2G,
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyImpresa_origine.Piva,
                KeyImpresa_Destinazione.Piva,
                TipoOperazioneDB
            )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine("copia Pratiche_Pull: " & ex.Message.ToString)
        End Try

    End Sub
    Private Sub G2G_Gestione_Allegati_WS(ByVal objOpzioniImportImpresa As clsImpresa,
        ByVal efG2G As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
        ByVal KeyImpresa_origine As clsKeyImpresa,
        ByRef KeyImpresa_Destinazione As clsKeyImpresa,
        ByRef ErrFLAG As Integer,
        ByVal TipoOperazioneDB As enum_TipoOperazioneDB
    )

        Try
            _objScriviHelper.Elabora_XML_Allegati_Salva(
                objOpzioniImportImpresa,
                efG2G,
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyImpresa_origine.Piva,
                KeyImpresa_Destinazione.Piva,
                TipoOperazioneDB
            )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine("copia Allegati: " & ex.Message.ToString)
        End Try

    End Sub

    Private Sub G2G_Gestione_Allegati_WSReverse(ByVal objOpzioniImportImpresa As clsImpresa,
        ByVal efG2G As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
        ByVal KeyImpresa_origine As clsKeyImpresa,
        ByRef KeyImpresa_Destinazione As clsKeyImpresa,
        ByRef ErrFLAG As Integer,
        ByVal TipoOperazioneDB As enum_TipoOperazioneDB
    )

        Try
            _objScriviHelper.Elabora_XML_Allegati_SalvaReverse(
                objOpzioniImportImpresa,
                efG2G,
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyImpresa_origine.Piva,
                KeyImpresa_Destinazione.Piva,
                TipoOperazioneDB
            )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine("copia Allegati: " & ex.Message.ToString)
        End Try

    End Sub

    Private Sub G2G_Gestione_ImportaAnalisi(ByVal objOpzioni As Gias2Gias_LIB.clsOpzioni, ByRef ErrFLAG As Integer, ByRef Log_G2G As StringBuilder, ByRef Log_Errori As StringBuilder, ByRef Log_Riepilogo As StringBuilder, ByVal KeyImpresa_ORIGINE As clsKeyImpresa, ByVal KeyImpresa_DESTINAZIONE As clsKeyImpresa, ByVal lEnum_AnalisiTipo As enum_AnalisiTipo)




        Dim HT_Testata_Cod As New Hashtable


        _objScriviHelper.Import_PrelevaTestataCod(
                    KeyImpresa_ORIGINE.Piva,
                    KeyImpresa_DESTINAZIONE.Piva,
                    objOpzioni,
                    Log_G2G,
                    Log_Errori,
                    Log_Riepilogo,
                    HT_Testata_Cod,
                    lEnum_AnalisiTipo
                )

        Dim Filtro_TestataCOD As String = _objScriviHelper.FiltroTestataCOD_from_TestataCOD(HT_Testata_Cod)


        _objScriviHelper.Import_Analisi_Gias2Gias(
                    KeyImpresa_ORIGINE.Piva,
                    KeyImpresa_DESTINAZIONE.Piva,
                            objOpzioni,
                            Log_G2G,
                            Log_Errori,
                            Log_Riepilogo,
                            HT_Testata_Cod,
                            Filtro_TestataCOD,
                            ErrFLAG
        )
    End Sub
    '###########################################################################################################

    Private Sub G2G_Copia_Impresa(ByVal KeyImpresa_ORIGINE As clsKeyImpresa,
                                  ByVal KeyImpresa_DESTINAZIONE As clsKeyImpresa,
                                  ByRef ErrFLAG As Integer)

        Dim NomeRoutine As String = "G2G_Copia_Imprese"

        Try
            Dim DT As New DataTable
            Dim objORIGINE As New AgronicaCoreAnagrafeDAL.Imprese_Read()

            DT = objORIGINE.Leggi(KeyImpresa_ORIGINE.Piva,
                                  enumSelezioneVariabile.Selezione_TabellaCompleta,
                                  "", "",
                                  _opzioni.objParametri_Server_GIAS_ORIGINE)

            If (Not IsNothing(DT)) AndAlso (DT.Rows.Count > 0) Then

                Dim objDESTINAZIONE As New AgronicaCoreAnagrafeDAL.Imprese_Write()

                'v
                objDESTINAZIONE.Scrivi(
                                    KeyImpresa_DESTINAZIONE.Piva,
                                    CType(DT.Rows(0).Item("Rag_Soc"), String),
                                    CType(DT.Rows(0).Item("Delega"), String),
                                    CType(DT.Rows(0).Item("AT_Prevalente"), String),
                                    CType(DT.Rows(0).Item("Forma_Giuridica"), String),
                                    CType(DT.Rows(0).Item("Forma_Conduzione"), String),
                                    CType(DT.Rows(0).Item("Sup_Totale"), Double),
                                    CType(DT.Rows(0).Item("TipoImpresaGerarchia"), Integer),
                                    CType(DT.Rows(0).Item("Blk_Inizio_Note"), String),
                                    CType(DT.Rows(0).Item("Validita_Inizio"), Date),
                                    CType(DT.Rows(0).Item("Validita_Fine"), Date),
                                    _opzioni.objParametri_Server_GIAS_DESTINAZIONE,
                                    CType(DT.Rows(0).Item("Data_Creazione"), Date),
                                    CType(DT.Rows(0).Item("Data_Modifica"), Date),
                                    CType(DT.Rows(0).Item("Username_Creazione"), String),
                                    CType(DT.Rows(0).Item("Username_Modifica"), String),
                                    CType(DT.Rows(0).Item("Validazione"), Integer),
                                    CType(DT.Rows(0).Item("Data_Validazione"), DateTime),
                                    CType(DT.Rows(0).Item("UserName_Validazione"), String),
                                    CType(DT.Rows(0).Item("Blk_Flag"), Integer),
                                    CType(DT.Rows(0).Item("Blk_Inizio_Data"), DateTime),
                                    CType(DT.Rows(0).Item("Blk_Inizio_Username"), String),
                                    CType(DT.Rows(0).Item("Blk_Fine_Data"), DateTime),
                                    CType(DT.Rows(0).Item("Blk_Fine_Username"), String),
                                    CType(DT.Rows(0).Item("Blk_Fine_Note"), String)
                                    )

                DT.Dispose()

            End If

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine(" [" & NomeRoutine & "] : " & ex.Message.ToString)
        End Try

    End Sub

    '###########################################################################################################
    Private Sub G2G_Copia_Indirizzo_Impresa(
                                ByVal KeyImpresa_ORIGINE As clsKeyImpresa,
                                ByVal KeyImpresa_DESTINAZIONE As clsKeyImpresa,
                                ByRef ErrFLAG As Integer)

        Dim KeyIndirizzo_ORIGINE As New clsKeyIndirizzo

        Dim validita_inizio As Date
        Dim validita_fine As Date
        Dim Data_Creazione As Date
        Dim Data_Modifica As Date
        Dim Username_Creazione As String = ""
        Dim Username_Modifica As String = ""

        G2G_Leggi_ImpresexIndirizzi(
                            KeyImpresa_ORIGINE,
                            KeyIndirizzo_ORIGINE,
                            ErrFLAG,
                            validita_inizio,
                            validita_fine,
                            Data_Creazione,
                            Data_Modifica,
                            Username_Creazione,
                            Username_Modifica
                            )

        If ErrFLAG <> 0 Then
            Exit Sub
        End If

        Dim KeyIndirizzo_DESTINAZIONE As New clsKeyIndirizzo

        G2G_Copia_Indirizzo(
                            KeyIndirizzo_ORIGINE,
                            KeyIndirizzo_DESTINAZIONE,
                            ErrFLAG)

        If ErrFLAG <> 0 Then
            Exit Sub
        End If

        G2G_Scrivi_ImpresexIndirizzi(
                            KeyImpresa_DESTINAZIONE,
                            KeyIndirizzo_DESTINAZIONE,
                            ErrFLAG,
                            validita_inizio,
                            validita_fine,
                            Data_Creazione,
                            Data_Modifica,
                            Username_Creazione,
                            Username_Modifica)

        If ErrFLAG <> 0 Then
            Exit Sub
        End If

    End Sub



    '###########################################################################################################
    Private Sub G2G_Leggi_ImpresexIndirizzi(
                                ByVal KeyImpresa_ORIGINE As clsKeyImpresa,
                                ByRef KeyIndirizzo_ORIGINE As clsKeyIndirizzo,
                                ByRef ErrFLAG As Integer,
                                ByRef Validita_Inizio As Date,
                                ByRef Validita_Fine As Date,
                                ByRef Data_Creazione As Date,
                                ByRef Data_Modifica As Date,
                                ByRef Username_Creazione As String,
                                ByRef Username_Modifica As String)

        Dim NomeRoutine As String = "G2G_Leggi_ImpresexIndirizzi"

        KeyIndirizzo_ORIGINE.Cod_Indirizzo = 0

        Try
            Dim Cod_Indirizzo As Integer
            Dim objORIGINE As New AgronicaCoreAnagrafeDAL.ImpresexIndirizzi_R

            Cod_Indirizzo = objORIGINE.CodIndirizzo_from_Piva(
                                                            KeyImpresa_ORIGINE.Piva,
                                                            _opzioni.objParametri_Server_GIAS_ORIGINE)

            Dim DT As DataTable =
                objORIGINE.Leggi(
                    KeyImpresa_ORIGINE.Piva,
                    0,
                    0,
                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                    "",
                    "",
                    _opzioni.objParametri_Server_GIAS_ORIGINE
                )

            Validita_Inizio = DT.Rows(0)("Validita_inizio")
            Validita_Fine = DT.Rows(0)("validita_fine")
            Data_Creazione = DT.Rows(0)("Data_Creazione")
            Data_Modifica = DT.Rows(0)("Data_Modifica")
            Username_Creazione = DT.Rows(0)("Username_Creazione")
            Username_Modifica = DT.Rows(0)("Username_Modifica")


            KeyIndirizzo_ORIGINE.Cod_Indirizzo = Cod_Indirizzo

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine(" [" & NomeRoutine & "] : " & ex.Message.ToString)
        End Try

    End Sub


    '###########################################################################################################
    Private Sub G2G_Copia_Indirizzo(ByVal KeyIndirizzo_ORIGINE As clsKeyIndirizzo,
                                    ByRef KeyIndirizzo_DESTINAZIONE As clsKeyIndirizzo,
                                    ByRef ErrFLAG As Integer)

        Dim NomeRoutine As String = "G2G_Copia_Indirizzo"

        Try
            Dim DT As New DataTable
            Dim objORIGINE As New AgronicaCoreAnagrafeDAL.Indirizzi_Read

            DT = objORIGINE.Leggi(KeyIndirizzo_ORIGINE.Cod_Indirizzo,
                                  enumSelezioneVariabile.Selezione_TabellaCompleta,
                                  "", "",
                                  _opzioni.objParametri_Server_GIAS_ORIGINE)

            If (Not IsNothing(DT)) AndAlso (DT.Rows.Count > 0) Then

                'TODO : Ricavo un nuovo codice indirizzo
                Dim objContatore As New AgronicaCoreDataProvider.Agro_Sequenze

                KeyIndirizzo_DESTINAZIONE.Cod_Indirizzo = objContatore.NuovoId_Tabella(
                                                                                    "indirizzi",
                                                                                    0, 2000000000,
                                                                                    _opzioni.objParametri_Server_GIAS_DESTINAZIONE)

                Dim objDESTINAZIONE As New AgronicaCoreAnagrafeDAL.Indirizzi_Write

                'v
                objDESTINAZIONE.Scrivi(
                                    KeyIndirizzo_DESTINAZIONE.Cod_Indirizzo,
                                    CType(DT.Rows(0).Item("Ind_Des"), String),
                                    CType(DT.Rows(0).Item("Frz_Des"), String),
                                    CType(DT.Rows(0).Item("CAP"), String),
                                    CType(DT.Rows(0).Item("Com_Des"), String),
                                    CType(DT.Rows(0).Item("Pro_Cod"), String),
                                    CType(DT.Rows(0).Item("Stato"), String),
                                    CType(DT.Rows(0).Item("Note"), String),
                                    CType(DT.Rows(0).Item("Pro_Cod_Istat"), String),
                                    CType(DT.Rows(0).Item("Com_Cod_Istat"), String),
                                    CType(DT.Rows(0).Item("Validita_Inizio"), Date),
                                    CType(DT.Rows(0).Item("Validita_Fine"), Date),
                                    _opzioni.objParametri_Server_GIAS_DESTINAZIONE,
                                    CType(DT.Rows(0).Item("Data_Creazione"), DateTime),
                                    CType(DT.Rows(0).Item("Data_Modifica"), DateTime),
                                    CType(DT.Rows(0).Item("Username_Creazione"), String),
                                    CType(DT.Rows(0).Item("Username_Modifica"), String),
                                    CType(DT.Rows(0).Item("Validazione"), Integer),
                                    CType(DT.Rows(0).Item("Data_Validazione"), DateTime),
                                    CType(DT.Rows(0).Item("UserName_Validazione"), String),
                                    CType(DT.Rows(0).Item("Codice_Lingua"), String),
                                    CType(DT.Rows(0).Item("Codice_Alternativo"), String)
                )

                DT.Dispose()

            Else
                ErrFLAG = 1
                _Log_G2G.Append(" [" & NomeRoutine & "]  :" & "Indirizzo di origine " & KeyIndirizzo_ORIGINE.Cod_Indirizzo & " non trovato")
            End If

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine(" [" & NomeRoutine & "] : " & ex.Message.ToString)
        End Try

    End Sub


    '###########################################################################################################
    Private Sub G2G_Scrivi_ImpresexIndirizzi(
                                ByVal KeyImpresa_DESTINAZIONE As clsKeyImpresa,
                                ByVal KeyIndirizzo_DESTINAZIONE As clsKeyIndirizzo,
                                ByRef ErrFLAG As Integer,
                                ByVal validita_inizio As Date,
                                ByVal validita_fine As Date,
                                ByVal Data_Creazione As Date,
                                ByVal Data_Modifica As Date,
                                ByVal Username_Creazione As String,
                                ByVal Username_Modifica As String
                            )

        Dim NomeRoutine As String = "G2G_Scrivi_ImpresexIndirizzi"

        Try
            Dim objDESTINAZIONE As New AgronicaCoreAnagrafeDAL.ImpresexIndirizzi_W

            'v
            objDESTINAZIONE.Scrivi(
                                KeyImpresa_DESTINAZIONE.Piva,
                                KeyIndirizzo_DESTINAZIONE.Cod_Indirizzo,
                                1, validita_inizio, validita_fine,
                                _opzioni.objParametri_Server_GIAS_DESTINAZIONE,
                                Data_Creazione,
                                Data_Modifica,
                                Username_Creazione,
                                Username_Modifica
                            )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine(" [" & NomeRoutine & "] : " & ex.Message.ToString)
        End Try

    End Sub


    '###########################################################################################################
    Private Sub G2G_Copia_Imprese_Codici(ByVal KeyImpresa_ORIGINE As clsKeyImpresa,
                                         ByVal KeyImpresa_DESTINAZIONE As clsKeyImpresa,
                                         ByRef ErrFLAG As Integer)

        Dim NomeRoutine As String = "G2G_Copia_Imprese_Codici"

        Try
            Dim DT As New DataTable
            Dim objORIGINE As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read

            DT = objORIGINE.Leggi(KeyImpresa_ORIGINE.Piva,
                                  0,
                                  enumSelezioneVariabile.Selezione_TabellaCompleta,
                                  "", "",
                                  _opzioni.objParametri_Server_GIAS_ORIGINE)

            If (Not IsNothing(DT)) AndAlso (DT.Rows.Count > 0) Then

                Dim objDESTINAZIONE As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Write

                Dim i As Integer

                For i = 0 To DT.Rows.Count - 1

                    'v
                    objDESTINAZIONE.Scrivi(KeyImpresa_DESTINAZIONE.Piva,
                                           CType(DT.Rows(i).Item("Id_Cod"), Integer),
                                           CType(DT.Rows(i).Item("Val_Cod"), String),
                                           CType(DT.Rows(i).Item("Imprese_Codici_Validita_Inizio"), Date),
                                           CType(DT.Rows(i).Item("Imprese_Codici_Validita_Fine"), Date),
                                           _opzioni.objParametri_Server_GIAS_DESTINAZIONE,
                                           CType(DT.Rows(i).Item("Imprese_Codici_Data_Creazione"), DateTime),
                                           CType(DT.Rows(i).Item("Imprese_Codici_Data_Modifica"), DateTime),
                                           CType(DT.Rows(i).Item("Imprese_Codici_Username_Creazione"), String),
                                           CType(DT.Rows(i).Item("Imprese_Codici_Username_Modifica"), String),
                                           CType(DT.Rows(i).Item("Imprese_Codici_Validazione"), Integer),
                                           CType(DT.Rows(i).Item("Imprese_Codici_Data_Validazione"), DateTime),
                                           CType(DT.Rows(i).Item("Imprese_Codici_UserName_Validazione"), String)
                                           )

                Next

                DT.Dispose()

            End If

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine(" [" & NomeRoutine & "] : " & ex.Message.ToString)
        End Try

    End Sub


    '###########################################################################################################
    Private Sub G2G_Scrivi_GerarchiaImprese(
                                ByVal KeyImpresa_ORIGINE As clsKeyImpresa,
                                ByVal KeyImpresa_Padre_DESTINAZIONE As clsKeyImpresa,
                                ByVal KeyImpresa_DESTINAZIONE As clsKeyImpresa,
                                ByRef ErrFLAG As Integer)

        Dim NomeRoutine As String = "G2G_Scrivi_GerarchiaImprese"

        Try
            Dim objORIGINE As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R
            Dim objDESTINAZIONE As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_W

            Dim DT As DataTable =
                objORIGINE.LeggixFiglio(
                    KeyImpresa_ORIGINE.Piva,
                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                    "",
                    "",
                    _opzioni.objParametri_Server_GIAS_ORIGINE
            )


            'v
            Dim abilitaDisabilita As String = " DISABLE "
            Dim strSQLDisableTrigger As String
            Dim stb As New StringBuilder

            stb.Append(" if exists ( " & vbCrLf)
            stb.Append(" Select 1 " & vbCrLf)
            stb.Append(" from sys.objects " & vbCrLf)
            stb.Append(" where name = 'TR_GerarchiaImpreseEsplosa' " & vbCrLf)
            stb.Append(" and type = 'TR' " & vbCrLf)
            stb.Append(" ) " & vbCrLf)
            stb.Append(" begin " & vbCrLf)
            stb.Append("    " & abilitaDisabilita & " trigger dbo.TR_GerarchiaImpreseEsplosa on GerarchiaImprese " & vbCrLf)
            stb.Append(" End " & vbCrLf)

            strSQLDisableTrigger = stb.ToString

            objDESTINAZIONE.EseguiQuery_Scrittura(_opzioni.objParametri_Server_GIAS_DESTINAZIONE, strSQLDisableTrigger, "Trigger OFF")
            Try



                objDESTINAZIONE.Scrivi(
                                    KeyImpresa_Padre_DESTINAZIONE.Piva,
                                    KeyImpresa_DESTINAZIONE.Piva,
                                    CType(DT.Rows(0).Item("Validita_Inizio"), Date),
                                    CType(DT.Rows(0).Item("Validita_Fine"), Date),
                                    _opzioni.objParametri_Server_GIAS_DESTINAZIONE,
                                    _opzioni.objParametri_Utenti_GIAS_DESTINAZIONE,
                                     CType(DT.Rows(0).Item("Data_Creazione"), Date),
                                    CType(DT.Rows(0).Item("Data_Modifica"), Date),
                                    CType(DT.Rows(0).Item("Username_Creazione"), String),
                                    CType(DT.Rows(0).Item("Username_Modifica"), String)
                               )

                strSQLDisableTrigger = strSQLDisableTrigger.Replace(abilitaDisabilita, "ENABLE")
                objDESTINAZIONE.EseguiQuery_Scrittura(_opzioni.objParametri_Server_GIAS_DESTINAZIONE, strSQLDisableTrigger, "Trigger ON")
            Catch ex As Exception

                strSQLDisableTrigger = strSQLDisableTrigger.Replace(abilitaDisabilita, "ENABLE")
                objDESTINAZIONE.EseguiQuery_Scrittura(_opzioni.objParametri_Server_GIAS_DESTINAZIONE, strSQLDisableTrigger, "Trigger ON")

                Throw ex
            End Try


        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine(" [" & NomeRoutine & "] : " & ex.Message.ToString)
        End Try

    End Sub

    Private Sub G2G_Copia_AreaOmogenea(
                                ByVal KeyImpresa_ORIGINE As clsKeyImpresa,
                                ByVal KeyImpresa_DESTINAZIONE As clsKeyImpresa,
                                ByRef ErrFLAG As Integer)


        Dim NomeRoutine As String = "G2G_Copia_AreaOmogenea"

        Try
            _objScriviHelper.Elabora_XML_AreaOmogenea_Salva(
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyImpresa_ORIGINE.Piva,
                KeyImpresa_DESTINAZIONE.Piva
            )
        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine("copia area omog.: " & ex.Message.ToString)
        End Try

    End Sub

    ''######################################
    ''PRIMA VERSIONE DI VANNI
    'Private Sub G2G_Copia_MateriePrime( _
    '                            ByVal KeyImpresa_ORIGINE As clsKeyImpresa, _
    '                            ByVal KeyImpresa_DESTINAZIONE As clsKeyImpresa, _
    '                            ByRef ErrFLAG As Integer)


    '    Dim NomeRoutine As String = "G2G_Copia_MateriePrime"

    '    Try
    '        _objScriviHelper.Elabora_XML_Imprese_MateriePrime_Salva( _
    '            _opzioni, _
    '            _Log_G2G, _
    '            _Log_Errori, _
    '            _Log_Riepilogo, _
    '            KeyImpresa_ORIGINE.Piva, _
    '            KeyImpresa_DESTINAZIONE.Piva _
    '        )
    '    Catch ex As Exception
    '        ErrFLAG = 1
    '        _Log_G2G.Append("copia materie prime: " & ex.Message.ToString & vbCrLf)
    '    End Try

    'End Sub

    Private Sub G2G_Copia_GestionePagamentiPublic(
                                        ByVal objOpzioni As Gias2Gias_LIB.clsOpzioni,
                                        ByRef ErrFLAG As Integer
                                        )

        Dim NomeRoutine As String = "G2G_Copia_GestionePagamentiPublic"

        Try
            _objScriviHelper.Elabora_Liquidita_Salva(
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                "",
                ""
            )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine("copia materie prime: " & ex.Message.ToString)
        End Try
    End Sub

    '###########################################################################################################
    'NUOVA VERSIONE CHE SOSTITUISCE QUELLA DI VANNI
    Private Sub G2G_Copia_MateriePrime_NEW(ByVal KeyImpresa_ORIGINE As clsKeyImpresa,
                                        ByVal KeyImpresa_DESTINAZIONE As clsKeyImpresa,
                                        ByVal objOpzioni As Gias2Gias_LIB.clsOpzioni,
                                        ByRef ErrFLAG As Integer,
                                        ByVal Flag_Pubblico As Boolean
                                        )

        Dim NomeRoutine As String = "G2G_Copia_MateriePrime_NEW"

        Try

            '=======================================================
            '========= 1) LETTURA DATI =============
            '=======================================================
            Dim objMP_R As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
            Dim DT_MP As DataTable
            Dim Piva_Destinazione As String

            If Flag_Pubblico Then

                '=======================================================
                '========= MATERIE PRIME PUBBLICHE =============
                '=======================================================

                DT_MP = objMP_R.Leggi3("",
                                        0,
                                        PUBBLICO,
                                        0, "",
                                        True,
                                        "", "",
                                        objOpzioni.objParametri_Server_GIAS_ORIGINE)

                'le MP pubbliche le importo sotto al supeuser
                Piva_Destinazione = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser


            Else

                '=======================================================
                '========= MATERIE PRIME PRIVATE =============
                '=======================================================

                DT_MP = objMP_R.Leggi3(KeyImpresa_ORIGINE.Piva,
                                        0,
                                        PRIVATO,
                                        0, "",
                                        True,
                                        "", "",
                                        objOpzioni.objParametri_Server_GIAS_ORIGINE)

                'le MP private rimangono sempre sotto all'impresa
                Piva_Destinazione = KeyImpresa_DESTINAZIONE.Piva

            End If

            _objScriviHelper.Elabora_XML_Imprese_MateriePrime_Salva_NEW(
                                     _opzioni,
                                     _Log_G2G,
                                     _Log_Errori,
                                     _Log_Riepilogo,
                                     KeyImpresa_ORIGINE.Piva,
                                     Piva_Destinazione,
                                     DT_MP)

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine(" [" & NomeRoutine & "] : " & ex.Message.ToString)
        End Try


    End Sub


    '###########################################################################################################
    Private Sub G2G_Copia_ParcoMacchine(ByVal KeyImpresa_ORIGINE As clsKeyImpresa,
                                        ByVal KeyImpresa_DESTINAZIONE As clsKeyImpresa,
                                        ByVal objOpzioni As Gias2Gias_LIB.clsOpzioni,
                                        ByRef ErrFLAG As Integer,
                                        ByVal Flag_Pubblico As Boolean
                                        )

        Dim NomeRoutine As String = "G2G_Copia_ParcoMacchine"

        Try

            '=======================================================
            '========= 1) LETTURA DATI =============
            '=======================================================
            Dim objMacchine_R As New AgronicaCoreContabDAL.Parco_Macchine_R
            Dim objMacchineCod_R As New AgronicaCoreContabDAL.Parco_Macchine_Codici_R
            Dim objCosti_R As New AgronicaCoreContabDAL.Prodotti_Costi_R
            Dim MovDet_R As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
            Dim DT_Macchine, DT_MacchineCod, DT_Costi, Dt_MovDet As DataTable
            Dim Piva_Destinazione As String

            If Flag_Pubblico Then

                '=======================================================
                '========= MACCHINE PUBBLICHE =============
                '=======================================================

                DT_Macchine = objMacchine_R.Leggi2("",
                                                    0,
                                                    PUBBLICO,
                                                    0, "",
                                                    True,
                                                    "", "",
                                                     objOpzioni.objParametri_Server_GIAS_ORIGINE)

                DT_Costi = objCosti_R.LeggiCostiParcoMacchine("",
                                                               0,
                                                               PUBBLICO,
                                                                "", "",
                                                                objOpzioni.objParametri_Server_GIAS_ORIGINE)

                DT_MacchineCod = objMacchineCod_R.LeggiJoinParcoMacchine("",
                                                                         0,
                                                                         PUBBLICO,
                                                                         "", "",
                                                                         objOpzioni.objParametri_Server_GIAS_ORIGINE)

                Dt_MovDet = MovDet_R.LeggiOperazioneMacchine("",
                                                              0, 0, 0, 0, 0,
                                                              PUBBLICO,
                                                              "", "",
                                                               objOpzioni.objParametri_Server_GIAS_ORIGINE)

                'le macchine pubbliche le importo sotto al supeuser
                Piva_Destinazione = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser


            Else

                '=======================================================
                '========= MACCHINE PRIVATE =============
                '=======================================================

                DT_Macchine = objMacchine_R.Leggi2(KeyImpresa_ORIGINE.Piva,
                                                    0,
                                                    PRIVATO,
                                                    0, "",
                                                    True,
                                                    "", "",
                                                     objOpzioni.objParametri_Server_GIAS_ORIGINE)

                DT_Costi = objCosti_R.LeggiCostiParcoMacchine(KeyImpresa_ORIGINE.Piva,
                                                 0,
                                                 PRIVATO,
                                                  "", "",
                                                  objOpzioni.objParametri_Server_GIAS_ORIGINE)

                'DT_Costi = objCosti_R.Leggi(KeyImpresa_ORIGINE.Piva, _
                '                            MACCHINE, _
                '                             "", _
                '                             0, 0, 0, 0, 0, _
                '                               enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
                '                            "", "", _
                '                            objOpzioni.objParametri_Server_GIAS_ORIGINE)

                DT_MacchineCod = objMacchineCod_R.LeggiJoinParcoMacchine(KeyImpresa_ORIGINE.Piva,
                                                             0,
                                                             PRIVATO,
                                                             "", "",
                                                             objOpzioni.objParametri_Server_GIAS_ORIGINE)

                'DT_MacchineCod = objMacchineCod_R.Leggi(KeyImpresa_ORIGINE.Piva, _
                '                                        0, _
                '                                         0, 0, "", _
                '                                           enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
                '                                         "", "", _
                '                                         objOpzioni.objParametri_Server_GIAS_ORIGINE)

                Dt_MovDet = MovDet_R.LeggiOperazioneMacchine(KeyImpresa_ORIGINE.Piva,
                                                              0, 0, 0, 0, 0,
                                                              PRIVATO,
                                                              "", "",
                                                               objOpzioni.objParametri_Server_GIAS_ORIGINE)

                'le macchine private rimangono sempre sotto all'impresa
                Piva_Destinazione = KeyImpresa_DESTINAZIONE.Piva

            End If


            ' If (Not IsNothing(DT)) AndAlso (DT.Rows.Count > 0) Then

            'For Each dRow As DataRow In DT.Rows

            '_objScriviHelper.Elabora_XML_Contatto_Salva( _
            '    objOpzioni, _
            '    _Log_G2G, _
            '    _Log_Errori, _
            '    _Log_Riepilogo, _
            '    dRow("Piva"), _
            '    dRow("Cod_contatto"), _
            '    )


            ' VAnni: 20/5/2019: TODO: verificare se, in questo caso, è sufficiente passare una lista impianti vuota.
            _objScriviHelper.Elabora_ParcoMacchine_Salva(
                _objOpzioniImportSingolaImpresa,
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyImpresa_ORIGINE.Piva,
                Piva_Destinazione,
                DT_Macchine,
                DT_MacchineCod,
                DT_Costi,
                Dt_MovDet
            )
            '   Next

            '  End If



        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine(" [" & NomeRoutine & "] : " & ex.Message.ToString)
        End Try


    End Sub

    Private Function ConfigurazioneG2GLeggiImprese(objOpzioni As clsOpzioni, imprese As List(Of clsImpresa)) As List(Of String)
        Dim listaImprese = New List(Of String)
        ' listaImprese.Add(objOpzioni.SuperUser_CodFiscale_ORIGINE)
        For Each impresa In imprese
            listaImprese.Add(impresa.Piva_ORIGINE)
        Next
        Return listaImprese
    End Function

    Private Function ConfigurazioneG2GLeggiContatti(objOpzioniGlobali As clsDatiGlobali, objOpzioni As clsOpzioni, listaImprese As List(Of clsImpresa)) As G2G_Configurazione_FiltriReq_Contatti
        Dim oConfigurazione As New G2G_Configurazione_FiltriReq_Contatti
        If Not String.IsNullOrEmpty(objOpzioniGlobali.configurazione_contatti) Then
            oConfigurazione = JsonConvert.DeserializeObject(Of G2G_Configurazione_FiltriReq_Contatti)(objOpzioniGlobali.configurazione_contatti)
        Else
            oConfigurazione.listaCodRapporto = New List(Of Integer)
        End If
        oConfigurazione.listaImprese = ConfigurazioneG2GLeggiImprese(objOpzioni, listaImprese)
        oConfigurazione.dataValidita = objOpzioniGlobali.Validita_Inizio_contatti
        Return oConfigurazione
    End Function

    Private Function ConfigurazioneG2GLeggiMateriePrime(objOpzioniGlobali As clsDatiGlobali, objOpzioni As clsOpzioni, listaImprese As List(Of clsImpresa)) As G2G_Configurazione_FiltriReq_Materie_Prime
        Dim oConfigurazione As New G2G_Configurazione_FiltriReq_Materie_Prime
        oConfigurazione.listaImprese = ConfigurazioneG2GLeggiImprese(objOpzioni, listaImprese)
        Return oConfigurazione
    End Function

    Private Function ConfigurazioneG2GLeggiMateriePrimeCampionature(objOpzioniGlobali As clsDatiGlobali, objOpzioni As clsOpzioni, listaImprese As List(Of clsImpresa)) As G2G_Configurazione_FiltriReq_Materie_Prime_Campionature
        Dim oConfigurazione As New G2G_Configurazione_FiltriReq_Materie_Prime_Campionature
        oConfigurazione.listaImprese = ConfigurazioneG2GLeggiImprese(objOpzioni, listaImprese)
        oConfigurazione.dataValidita = objOpzioniGlobali.Validita_Inizio_materieprimecampionature
        Return oConfigurazione
    End Function

    Private Function ConfigurazioneG2GLeggiAnalisiCondivise(objOpzioniGlobali As clsDatiGlobali, objOpzioni As clsOpzioni, listaImprese As List(Of clsImpresa)) As G2G_Configurazione_FiltriReq_Analisi_Condivise
        Dim oConfigurazione As New G2G_Configurazione_FiltriReq_Analisi_Condivise
        oConfigurazione.listaImprese = ConfigurazioneG2GLeggiImprese(objOpzioni, listaImprese)
        oConfigurazione.dataValidita = objOpzioniGlobali.Validita_Inizio_analisicondivise
        Return oConfigurazione
    End Function

    Private Function ConfigurazioneG2GLeggiAttivita(objOpzioniGlobali As clsDatiGlobali, objOpzioniImportImpresa As clsImpresa, objOpzioni As clsOpzioni, listaImprese As List(Of clsImpresa)) As G2G_Configurazione_FiltriReq_Attivita
        Dim oConfigurazione As New G2G_Configurazione_FiltriReq_Attivita
        oConfigurazione.listaImprese = ConfigurazioneG2GLeggiImprese(objOpzioni, listaImprese)
        oConfigurazione.dataValidita_Inizio = objOpzioniImportImpresa.ValiditaInizio_agenda
        oConfigurazione.dataValidita_Fine = objOpzioniImportImpresa.ValiditaFine_agenda
        If Not String.IsNullOrEmpty(objOpzioniImportImpresa.configurazione_agenda) Then
            Dim oConfigurazioneAgenda = JsonConvert.DeserializeObject(Of G2G_Configurazione_FiltriReq_Agenda)(objOpzioniImportImpresa.configurazione_agenda)
            oConfigurazione.ListaLav_Cod = oConfigurazioneAgenda.listaLavCod
        Else
            oConfigurazione.ListaLav_Cod = New List(Of Integer)
        End If

        Return oConfigurazione
    End Function


    Private Function ConfigurazioneG2GLeggiMacchine(objOpzioniGlobali As clsDatiGlobali, objOpzioni As clsOpzioni, listaImprese As List(Of clsImpresa)) As G2G_Configurazione_FiltriReq_Macchine
        Dim oConfigurazione As New G2G_Configurazione_FiltriReq_Macchine
        If Not String.IsNullOrEmpty(objOpzioniGlobali.configurazione_macchine) Then
            oConfigurazione = JsonConvert.DeserializeObject(Of G2G_Configurazione_FiltriReq_Macchine)(objOpzioniGlobali.configurazione_macchine)
        End If
        oConfigurazione.listaImprese = ConfigurazioneG2GLeggiImprese(objOpzioni, listaImprese)
        oConfigurazione.dataValidita = objOpzioniGlobali.Validita_Inizio_macchine
        Return oConfigurazione
    End Function

    Private Sub G2G_Copia_Contatti_Pubblico_WS(
        ByRef ErrFLAG As Integer,
        ByVal Configurazione As G2G_Configurazione_FiltriReq_Contatti
    )

        Try
            _objScriviHelper.Elabora_XML_Contatti_Salva(
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                "",
                "",
                True,
                Configurazione
            )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine("copia contatti pubblici: " & ex.Message.ToString)
        End Try

    End Sub

    Private Sub G2G_Copia_Contatti_Pubblico_WSReverse(
        ByRef ErrFLAG As Integer,
        ByVal Configurazione As G2G_Configurazione_FiltriReq_Contatti
    )

        Try
            _objScriviHelper.Elabora_XML_Contatti_SalvaReverse(
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                "",
                "",
                True,
                Configurazione
            )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine("copia contatti pubblici: " & ex.Message.ToString)
        End Try

    End Sub

    Private Sub G2G_Copia_MateriePrime_WS(
        ByVal KeyImpresa_Origine As clsKeyImpresa,
        ByRef KeyImpresa_Destinazione As clsKeyImpresa,
        ByRef ErrFLAG As Integer,
        ByVal Flag_Pubblico As Boolean,
        ByVal Configurazione As G2G_Configurazione_FiltriReq_Materie_Prime
    )

        Try
            _objScriviHelper.Elabora_XML_MateriePrime_Salva(
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyImpresa_Origine.Piva,
                KeyImpresa_Destinazione.Piva,
                Flag_Pubblico,
                Configurazione
            )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine("copia materie prime: " & ex.Message.ToString)
        End Try

    End Sub

    Private Sub G2G_Copia_MateriePrime_WSReverse(
        ByVal KeyImpresa_Origine As clsKeyImpresa,
        ByRef KeyImpresa_Destinazione As clsKeyImpresa,
        ByRef ErrFLAG As Integer,
        ByVal Flag_Pubblico As Boolean,
        ByVal Configurazione As G2G_Configurazione_FiltriReq_Materie_Prime
    )

        Try
            _objScriviHelper.Elabora_XML_MateriePrime_SalvaReverse(
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyImpresa_Origine.Piva,
                KeyImpresa_Destinazione.Piva,
                Flag_Pubblico,
                Configurazione
            )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine("copia materie prime: " & ex.Message.ToString)
        End Try

    End Sub

    Private Sub G2G_Copia_MateriePrime_Pubblico_WS(
        ByRef ErrFLAG As Integer,
        ByVal Configurazione As G2G_Configurazione_FiltriReq_Materie_Prime
    )

        Try
            _objScriviHelper.Elabora_XML_MateriePrime_Salva(
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                "",
                "",
                True,
                Configurazione
            )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine("copia materie prime pubbliche: " & ex.Message.ToString)
        End Try

    End Sub

    Private Sub G2G_Copia_MateriePrime_Pubblico_WSReverse(
        ByRef ErrFLAG As Integer,
        ByVal Configurazione As G2G_Configurazione_FiltriReq_Materie_Prime
    )

        Try
            _objScriviHelper.Elabora_XML_MateriePrime_SalvaReverse(
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                "",
                "",
                True,
                Configurazione
            )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine("copia materie prime pubbliche: " & ex.Message.ToString)
        End Try

    End Sub

    Private Sub G2G_Copia_Materie_Prime_Campionature_Pubblico_WS(
        ByRef ErrFLAG As Integer,
        ByVal Configurazione As G2G_Configurazione_FiltriReq_Materie_Prime_Campionature
    )

        Try
            _objScriviHelper.Elabora_XML_MateriePrimeCampionature_Salva(
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                "",
                "",
                True,
                Configurazione
            )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine("copia materie prime campionature pubbliche: " & ex.Message.ToString)
        End Try

    End Sub

    Private Sub G2G_Copia_Materie_Prime_Campionature_Pubblico_WSReverse(
        ByRef ErrFLAG As Integer,
        ByVal Configurazione As G2G_Configurazione_FiltriReq_Materie_Prime_Campionature
    )

        Try
            _objScriviHelper.Elabora_XML_MateriePrimeCampionature_SalvaReverse(
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                "",
                "",
                True,
                Configurazione
            )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine("copia materie prime campionature pubbliche: " & ex.Message.ToString)
        End Try

    End Sub

    Private Sub G2G_Copia_Analisi_Condivise_Pubblico_WS(
        ByRef ErrFLAG As Integer,
        ByVal Configurazione As G2G_Configurazione_FiltriReq_Analisi_Condivise
    )

        Try
            Const NomeFunzione As String = "Elabora_XML_AnalisiCondivise_Salva"
            Dim listaImpreseAnalisi As New List(Of String)

            Try

                ' leggo struttura dati serializzata su origine contenente materie prime campionature da inserire/modificare/cancellare 
                Dim sXmlAnalisiCondivise = _objScriviHelper.LeggiAnalisiCondiviseXML("", "", _opzioni, True, Configurazione, _opzioni.objParametri_Server_GIAS_ORIGINE, listaImpreseAnalisi)

                ' scrivo dati serializzati richiamando web service destinazione
                Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS With {.Url = _opzioni.wsimportaGiasURl, .Timeout = Integer.MaxValue}
                Dim Str_Credenziali_WS As String = Gias2Gias_LIB.Funzioni.Get_Str_Credenziali_WS(_opzioni)

                ' creo l'azienda del contatto pubblico su destinazione se non esiste
                If True AndAlso listaImpreseAnalisi.Count > 0 Then

                    Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
                    Dim ConnectionString As String = gefutils.GetEntityConnectionString(_opzioni.objParametri_Server_GIAS_ORIGINE.StringaConnessione)
                    Dim efG2G As New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(ConnectionString)
                    Dim objOpzioniImportImpresa As New clsImpresa

                    For Each piva In listaImpreseAnalisi
                        If Not Configurazione.listaImprese.Contains(piva) Then
                            Dim strErr As String = ""
                            Dim pivaEsistente = wsimportazione.Verifica_EsistenzaPivaGIAS(Str_Credenziali_WS, piva, strErr)

                            G2GUtility.Log(_Log_G2G, "Trasferimento analisi condivise - Impresa: " & piva)

                            ErrFLAG = 0
                            _objScriviHelper.ScriviImpreseXML(
                                    _opzioni,
                                    _Log_G2G,
                                    _Log_Errori,
                                    _Log_Riepilogo,
                                    piva,
                                    piva,
                                    _opzioni.SuperUser_CodFiscale_DESTINAZIONE,
                                    True
                                )

                            Dim KeyImpresa_ORIGINE As New clsKeyImpresa()
                            Dim KeyImpresa_DESTINAZIONE As New clsKeyImpresa()

                            KeyImpresa_ORIGINE.Piva = piva
                            KeyImpresa_DESTINAZIONE.Piva = piva

                            Dim CodiceGias_ORIGINE As New clsCodiceGias(_opzioni.ProgressivoGIAS_ORIGINE)
                            Dim CodiceGias_DESTINAZIONE As New clsCodiceGias(_opzioni.ProgressivoGIAS_DESTINAZIONE)

                            G2G_Lista_Centri_WS(
                                objOpzioniImportImpresa,
                                KeyImpresa_ORIGINE,
                                KeyImpresa_DESTINAZIONE,
                                CodiceGias_ORIGINE,
                                CodiceGias_DESTINAZIONE,
                                ErrFLAG
                            )

                            If ErrFLAG = 0 Then

                                _objScriviHelper.Elabora_XML_Analisi_Salva(
                                    objOpzioniImportImpresa,
                                    efG2G,
                                    _opzioni,
                                    _Log_G2G,
                                    _Log_Errori,
                                    _Log_Riepilogo,
                                    piva,
                                    piva, enum_TipoOperazioneDB.Scrittura
                                )

                            End If
                        End If
                    Next
                End If

            Catch ex As Exception
                Dim msg As String = CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: " & ex.Message
                _Log_G2G.AppendLine(msg)
                _Log_G2G.AppendLine(msg)
                Throw New Exception(msg)
            End Try


        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine("copia analisi condivise: " & ex.Message.ToString)
        End Try

    End Sub

    Private Sub G2G_Copia_Analisi_Condivise_Pubblico_WSReverse(
        ByRef ErrFLAG As Integer,
        ByVal Configurazione As G2G_Configurazione_FiltriReq_Analisi_Condivise
    )

        Try
            Const NomeFunzione As String = "G2G_Copia_Analisi_Condivise_Pubblico_WSReverse"
            Dim listaImpreseAnalisi As New List(Of String)

            Try

                ' leggo struttura dati serializzata su origine contenente materie prime campionature da inserire/modificare/cancellare 
                Dim sXmlAnalisiCondivise = _objScriviHelper.LeggiAnalisiCondiviseXMLReverse("", "", _opzioni, True, Configurazione, _opzioni.objParametri_Server_GIAS_ORIGINE, listaImpreseAnalisi)

                ' scrivo dati serializzati richiamando web service destinazione
                Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS With {.Url = _opzioni.wsimportaGiasURl, .Timeout = Integer.MaxValue}
                Dim Str_Credenziali_WS As String = Gias2Gias_LIB.Funzioni.Get_Str_Credenziali_WS(_opzioni)

                ' creo l'azienda del contatto pubblico su destinazione se non esiste
                If True AndAlso listaImpreseAnalisi.Count > 0 Then

                    Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
                    Dim ConnectionString As String = gefutils.GetEntityConnectionString(_opzioni.objParametri_Server_GIAS_ORIGINE.StringaConnessione)
                    Dim efG2G As New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(ConnectionString)
                    Dim objOpzioniImportImpresa As New clsImpresa

                    For Each piva In listaImpreseAnalisi
                        If Not Configurazione.listaImprese.Contains(piva) Then
                            Dim strErr As String = ""
                            Dim pivaEsistente = wsimportazione.Verifica_EsistenzaPivaGIAS(Str_Credenziali_WS, piva, strErr)

                            G2GUtility.Log(_Log_G2G, "Trasferimento analisi condivise - Impresa: " & piva)

                            ErrFLAG = 0
                            _objScriviHelper.ScriviImpreseXML_Reverse(
                                    _opzioni,
                                    _Log_G2G,
                                    _Log_Errori,
                                    _Log_Riepilogo,
                                    piva,
                                    piva,
                                    _opzioni.SuperUser_CodFiscale_DESTINAZIONE,
                                    True
                                )

                            Dim KeyImpresa_ORIGINE As New clsKeyImpresa()
                            Dim KeyImpresa_DESTINAZIONE As New clsKeyImpresa()

                            KeyImpresa_ORIGINE.Piva = piva
                            KeyImpresa_DESTINAZIONE.Piva = piva

                            Dim CodiceGias_ORIGINE As New clsCodiceGias(_opzioni.ProgressivoGIAS_ORIGINE)
                            Dim CodiceGias_DESTINAZIONE As New clsCodiceGias(_opzioni.ProgressivoGIAS_DESTINAZIONE)

                            G2G_Lista_Centri_WSReverse(
                                objOpzioniImportImpresa,
                                KeyImpresa_ORIGINE,
                                KeyImpresa_DESTINAZIONE,
                                CodiceGias_ORIGINE,
                                CodiceGias_DESTINAZIONE,
                                ErrFLAG
                            )

                            If ErrFLAG = 0 Then

                                _objScriviHelper.Elabora_XML_Analisi_SalvaReverse(
                                    objOpzioniImportImpresa,
                                    efG2G,
                                    _opzioni,
                                    _Log_G2G,
                                    _Log_Errori,
                                    _Log_Riepilogo,
                                    piva,
                                    piva, enum_TipoOperazioneDB.Scrittura
                                )

                            End If
                        End If
                    Next
                End If

            Catch ex As Exception
                Dim msg As String = CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: " & ex.Message
                _Log_G2G.AppendLine(msg)
                _Log_G2G.AppendLine(msg)
                Throw New Exception(msg)
            End Try


        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine("copia analisi condivise: " & ex.Message.ToString)
        End Try

    End Sub


    Private Sub G2G_Copia_Attivita_WS(
        ByRef ErrFLAG As Integer,
        ByVal Configurazione As G2G_Configurazione_FiltriReq_Attivita
    )

        Try
            Const NomeFunzione As String = "Elabora_XML_AnalisiCondivise_Salva"
            Dim listaImpreseAnalisi As New List(Of String)

            Try

                ' leggo struttura dati serializzata su origine contenente materie prime campionature da inserire/modificare/cancellare 
                _objScriviHelper.LeggiAttivitaXML("",
                                                                             "",
                                                                             _opzioni,
                                                                             _Log_G2G,
                                                                             _Log_Errori,
                                                                             _Log_Riepilogo,
                                                                             True,
                                                                             Configurazione,
                                                                             _opzioni.objParametri_Server_GIAS_ORIGINE,
                                                                             listaImpreseAnalisi)


            Catch ex As Exception
                Dim msg As String = CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: " & ex.Message
                _Log_G2G.AppendLine(msg)
                _Log_G2G.AppendLine(msg)
                Throw New Exception(msg)
            End Try


        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine("copia attivita condivise: " & ex.Message.ToString)
        End Try

    End Sub

    Private Sub G2G_Copia_Attivita_WSReverse(
        ByRef ErrFLAG As Integer,
        ByVal Configurazione As G2G_Configurazione_FiltriReq_Attivita
    )

        Try
            Const NomeFunzione As String = "G2G_Copia_Attivita_WSReverse"
            Dim listaImpreseAnalisi As New List(Of String)

            Try

                ' leggo struttura dati serializzata su origine contenente materie prime campionature da inserire/modificare/cancellare 
                _objScriviHelper.LeggiAttivitaXMLReverse("",
                                                                             "",
                                                                             _opzioni,
                                                                             _Log_G2G,
                                                                             _Log_Errori,
                                                                             _Log_Riepilogo,
                                                                             True,
                                                                             Configurazione,
                                                                             _opzioni.objParametri_Server_GIAS_ORIGINE,
                                                                             listaImpreseAnalisi)


            Catch ex As Exception
                Dim msg As String = CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: " & ex.Message
                _Log_G2G.AppendLine(msg)
                _Log_G2G.AppendLine(msg)
                Throw New Exception(msg)
            End Try


        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine("copia attivita condivise: " & ex.Message.ToString)
        End Try

    End Sub

    Private Sub G2G_Copia_Contatti_WS(
        ByVal KeyImpresa_Origine As clsKeyImpresa,
        ByRef KeyImpresa_Destinazione As clsKeyImpresa,
        ByRef ErrFLAG As Integer,
        ByVal Flag_Pubblico As Boolean,
        ByVal Configurazione As G2G_Configurazione_FiltriReq_Contatti
    )

        Try
            _objScriviHelper.Elabora_XML_Contatti_Salva(
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyImpresa_Origine.Piva,
                KeyImpresa_Destinazione.Piva,
                Flag_Pubblico,
                Configurazione
            )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine("copia contatti: " & ex.Message.ToString)
        End Try

    End Sub

    Private Sub G2G_Copia_Contatti_WSReverse(
        ByVal KeyImpresa_Origine As clsKeyImpresa,
        ByRef KeyImpresa_Destinazione As clsKeyImpresa,
        ByRef ErrFLAG As Integer,
        ByVal Flag_Pubblico As Boolean,
        ByVal Configurazione As G2G_Configurazione_FiltriReq_Contatti
    )

        Try
            _objScriviHelper.Elabora_XML_Contatti_SalvaReverse(
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyImpresa_Origine.Piva,
                KeyImpresa_Destinazione.Piva,
                Flag_Pubblico,
                Configurazione
            )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine("copia contatti: " & ex.Message.ToString)
        End Try

    End Sub

    Private Sub G2G_Copia_ParcoMacchine_Pubblico_WS(
        ByRef ErrFLAG As Integer,
        ByVal Configurazione As G2G_Configurazione_FiltriReq_Macchine
    )

        Try
            _objScriviHelper.Elabora_XML_Parco_Macchine_Salva(
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                "",
                "",
                True,
                Configurazione
            )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine("copia parco macchine pubblico: " & ex.Message.ToString)
        End Try

    End Sub

    Private Sub G2G_Copia_ParcoMacchine_Pubblico_WSReverse(
        ByRef ErrFLAG As Integer,
        ByVal Configurazione As G2G_Configurazione_FiltriReq_Macchine
    )

        Try
            _objScriviHelper.Elabora_XML_Parco_Macchine_SalvaReverse(
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                "",
                "",
                True,
                Configurazione
            )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine("copia parco macchine pubblico: " & ex.Message.ToString)
        End Try

    End Sub

    Private Sub G2G_Copia_ParcoMacchine_WS(
        ByVal KeyImpresa_Origine As clsKeyImpresa,
        ByRef KeyImpresa_Destinazione As clsKeyImpresa,
        ByRef ErrFLAG As Integer,
        ByVal Flag_Pubblico As Boolean,
        ByVal Configurazione As G2G_Configurazione_FiltriReq_Macchine
    )

        Try
            _objScriviHelper.Elabora_XML_Parco_Macchine_Salva(
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyImpresa_Origine.Piva,
                KeyImpresa_Destinazione.Piva,
                Flag_Pubblico,
                Configurazione
            )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine("copia parco macchine: " & ex.Message.ToString)
        End Try

    End Sub

    Private Sub G2G_Copia_ParcoMacchine_WSReverse(
        ByVal KeyImpresa_Origine As clsKeyImpresa,
        ByRef KeyImpresa_Destinazione As clsKeyImpresa,
        ByRef ErrFLAG As Integer,
        ByVal Flag_Pubblico As Boolean,
        ByVal Configurazione As G2G_Configurazione_FiltriReq_Macchine
    )

        Try
            _objScriviHelper.Elabora_XML_Parco_Macchine_SalvaReverse(
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyImpresa_Origine.Piva,
                KeyImpresa_Destinazione.Piva,
                Flag_Pubblico,
                Configurazione
            )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine("copia parco macchine: " & ex.Message.ToString)
        End Try

    End Sub
    '###########################################################################################################
    Private Sub G2G_Scrivi_UtentixImprese(
                                ByVal KeyImpresa_ORIGINE As clsKeyImpresa,
                                ByVal KeyImpresa_DESTINAZIONE As clsKeyImpresa,
                                ByRef ErrFLAG As Integer)

        Dim NomeRoutine As String = "G2G_Scrivi_UtentixImprese"

        Try

            Dim objORIGINE As New AgronicaCoreAnagrafeDAL.UtentixImprese_Read
            Dim objDESTINAZIONE As New AgronicaCoreAnagrafeDAL.UtentixImprese_Write

            Dim DT As DataTable =
                objORIGINE.Leggi(
                    KeyImpresa_ORIGINE.Piva,
                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                    "",
                    "",
                    _opzioni.objParametri_Server_GIAS_ORIGINE
                )

            'v
            objDESTINAZIONE.Scrivi(KeyImpresa_DESTINAZIONE.Piva,
                                   CType(DT.Rows(0).Item("UtentiXImprese_Validita_Inizio"), Date),
                                   CType(DT.Rows(0).Item("UtentiXImprese_Validita_Fine"), Date),
                                   _opzioni.objParametri_Server_GIAS_DESTINAZIONE,
                                   CType(DT.Rows(0).Item("UtentiXImprese_Data_Creazione"), Date),
                                   CType(DT.Rows(0).Item("UtentiXImprese_Data_Modifica"), Date),
                                   CType(DT.Rows(0).Item("UtentiXImprese_Username_Creazione"), String),
                                   CType(DT.Rows(0).Item("UtentiXImprese_Username_Modifica"), String),
                                   CType(DT.Rows(0).Item("UtentiXImprese_Validazione"), Integer),
                                   CType(DT.Rows(0).Item("UtentiXImprese_Data_Validazione"), Date),
                                   CType(DT.Rows(0).Item("UtentiXImprese_UserName_Validazione"), String)
                                   )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine(" [" & NomeRoutine & "] : " & ex.Message.ToString)
        End Try

    End Sub

    '###########################################################################################################
    Private Sub G2G_Copia_ContattoImpresa(
                                        ByVal KeyImpresa_ORIGINE As clsKeyImpresa,
                                        ByVal KeyImpresa_DESTINAZIONE As clsKeyImpresa,
                                        ByVal objOpzioni As Gias2Gias_LIB.clsOpzioni,
                                        ByRef ErrFLAG As Integer
                                        )

        Dim NomeRoutine As String = "G2G_Copia_ContattoImpresa"

        Try

            Dim Piva_Destinazione As String = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser

            'l'impresa gias contatto ha:
            'piva = piva superuser
            'cod_contatto = piva impresa
            _objScriviHelper.Elabora_XML_Contatto_Salva(
                                                objOpzioni,
                                                _Log_G2G,
                                                _Log_Errori,
                                                _Log_Riepilogo,
                                                objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser,
                                                KeyImpresa_ORIGINE.Piva,
                                                Piva_Destinazione)


        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine(" [" & NomeRoutine & "] : " & ex.Message.ToString)
        End Try


    End Sub

    '###########################################################################################################
    Private Sub G2G_Copia_ContattiPubbliciNoImpreseGias(ByVal objOpzioni As Gias2Gias_LIB.clsOpzioni,
                                                        ByRef ErrFLAG As Integer)

        Dim NomeRoutine As String = "G2G_Copia_Contatti"

        Try
            Dim DT As New DataTable
            Dim objORIGINE As New AgronicaCoreAnagrafeDAL.Contatti_R

            DT = objORIGINE.LeggiContattiNoImpreseGias("",
                                                       "",
                                                       PUBBLICO,
                                                       "", "",
                                                       objOpzioni.objParametri_Server_GIAS_ORIGINE)

            'i contatti pubblici vengono salvati sotto al superuser di destinazione 
            Dim Piva_Destinazione As String = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser

            If (Not IsNothing(DT)) AndAlso (DT.Rows.Count > 0) Then

                For Each dRow As DataRow In DT.Rows

                    _objScriviHelper.Elabora_XML_Contatto_Salva(
                        objOpzioni,
                        _Log_G2G,
                        _Log_Errori,
                        _Log_Riepilogo,
                        dRow("Piva"),
                        dRow("Cod_contatto"),
                        Piva_Destinazione)
                Next

                DT.Dispose()

            End If

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine(" [" & NomeRoutine & "] : " & ex.Message.ToString)
        End Try

    End Sub

    '###########################################################################################################
    Private Sub G2G_Copia_ContattiPrivatiNoImpreseGias(
                                ByVal KeyImpresa_ORIGINE As clsKeyImpresa,
                                ByVal KeyImpresa_DESTINAZIONE As clsKeyImpresa,
                                ByVal objOpzioni As Gias2Gias_LIB.clsOpzioni,
                                ByRef ErrFLAG As Integer
                                )

        Dim NomeRoutine As String = "G2G_Copia_Contatti"

        Try
            Dim DT As New DataTable
            Dim objORIGINE As New AgronicaCoreAnagrafeDAL.Contatti_R

            'DT = objORIGINE.LeggiContattoSpecifico(KeyImpresa_ORIGINE.Piva, _
            '                                        "", _
            '                                        PRIVATO, _
            '                                        enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
            '                                        "", "", _
            '                                        objOpzioni.objParametri_Server_GIAS_ORIGINE)

            DT = objORIGINE.LeggiContattiNoImpreseGias(KeyImpresa_ORIGINE.Piva,
                                                       "",
                                                       PRIVATO,
                                                       "", "",
                                                       objOpzioni.objParametri_Server_GIAS_ORIGINE)

            Dim Piva_Destinazione As String 'rimane l'impresa stessa

            If (Not IsNothing(DT)) AndAlso (DT.Rows.Count > 0) Then

                For Each dRow As DataRow In DT.Rows

                    Piva_Destinazione = dRow("Piva")

                    _objScriviHelper.Elabora_XML_Contatto_Salva(
                        objOpzioni,
                        _Log_G2G,
                        _Log_Errori,
                        _Log_Riepilogo,
                        dRow("Piva"),
                        dRow("Cod_contatto"),
                        Piva_Destinazione)
                Next

                DT.Dispose()

            End If

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine(" [" & NomeRoutine & "] : " & ex.Message.ToString)
        End Try

    End Sub

    '##########################################################
    Private Sub G2G_Copia_SpecieVegetaliDefault(
                               ByVal KeyImpresa_ORIGINE As clsKeyImpresa,
                               ByRef KeyImpresa_DESTINAZIONE As clsKeyImpresa,
                               ByRef ErrFLAG As Integer
                           )

        Dim NomeRoutine As String = "G2G_Copia_SpecieVegetaliDefault"

        Try

            _objScriviHelper.Elabora_SpecieVegetali_Default_Salva(
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyImpresa_ORIGINE.Piva,
                KeyImpresa_DESTINAZIONE.Piva
            )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine(" [" & NomeRoutine & "] : " & ex.Message.ToString)
        End Try

    End Sub

    Private Sub G2G_Copia_RicetteGlobali(
                                       ByRef ErrFLAG As Integer
                                        )

        Try

            _objScriviHelper.Elabora_Ricette_XML_Salva(
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                "",
                "",
                0,
                0,
                True
                )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.Append("copia ricette globali: " & ex.Message.ToString & vbCrLf)
        End Try

    End Sub

    Private Sub G2G_Copia_RicetteImpresa(
                                       ByVal KeyImpresa_ORIGINE As clsKeyImpresa,
                                       ByRef KeyImpresa_DESTINAZIONE As clsKeyImpresa,
                                       ByRef ErrFLAG As Integer
                                        )
        Try

            _objScriviHelper.Elabora_Ricette_XML_Salva(
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyImpresa_ORIGINE.Piva,
                KeyImpresa_DESTINAZIONE.Piva,
                0,
                0,
                True
                )
        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.Append("copia Ricetta: " & ex.Message.ToString & vbCrLf)
        End Try

    End Sub



    Private Sub G2G_Copia_LineeProduttive(
                                       ByVal KeyImpresa_ORIGINE As clsKeyImpresa,
                                       ByRef KeyImpresa_DESTINAZIONE As clsKeyImpresa,
                                       ByRef ErrFLAG As Integer
                                        )

        Try

            _objScriviHelper.Elabora_LineeProduttive_Salva(
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyImpresa_ORIGINE.Piva,
                KeyImpresa_DESTINAZIONE.Piva
                )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.Append("Copia LineeProduttive] : " & ex.Message.ToString & vbCrLf)
        End Try
    End Sub



    Private Sub G2G_Copia_Lotti_Configurazione(
                                       ByVal KeyImpresa_ORIGINE As clsKeyImpresa,
                                       ByRef KeyImpresa_DESTINAZIONE As clsKeyImpresa,
                                       ByRef ErrFLAG As Integer
                                        )

        Try

            _objScriviHelper.Elabora_Lotto_Configurazione_Salva(
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyImpresa_ORIGINE.Piva,
                KeyImpresa_DESTINAZIONE.Piva
                )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.Append("Copia LineeProduttive] : " & ex.Message.ToString & vbCrLf)
        End Try
    End Sub


    Private Sub G2G_Copia_ParticelleCatastalixVincoliAgronomici_WS(
                                ByVal objOpzioniImportImpresa As clsImpresa,
                                ByVal KeyImpresa_ORIGINE As clsKeyImpresa,
                                ByRef KeyImpresa_DESTINAZIONE As clsKeyImpresa,
                                ByRef ErrFLAG As Integer
                            )

        Try

            _objScriviHelper.Elabora_XML_ParticelleCatastalixVincoliAgronomici_Salva(
                objOpzioniImportImpresa,
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyImpresa_ORIGINE.Piva,
                KeyImpresa_DESTINAZIONE.Piva,
                _opzioni.SuperUser_CodFiscale_DESTINAZIONE,
                _opzioni.SuperUser_CodFiscale_DESTINAZIONE
                )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.Append("Copia ParticelleCatastalixVincoliAgronomici] : " & ex.Message.ToString & vbCrLf)
        End Try

    End Sub

    Private Sub G2G_Copia_ParticelleCatastalixVincoliAgronomici_WSReverse(
                                ByVal objOpzioniImportImpresa As clsImpresa,
                                ByVal KeyImpresa_ORIGINE As clsKeyImpresa,
                                ByRef KeyImpresa_DESTINAZIONE As clsKeyImpresa,
                                ByRef ErrFLAG As Integer
                            )

        Try

            _objScriviHelper.Elabora_XML_ParticelleCatastalixVincoliAgronomici_SalvaReverse(
                objOpzioniImportImpresa,
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyImpresa_ORIGINE.Piva,
                KeyImpresa_DESTINAZIONE.Piva,
                _opzioni.SuperUser_CodFiscale_DESTINAZIONE,
                _opzioni.SuperUser_CodFiscale_DESTINAZIONE
                )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.Append("Copia ParticelleCatastalixVincoliAgronomici] : " & ex.Message.ToString & vbCrLf)
        End Try

    End Sub

    Private Sub G2G_Copia_PUA_LetamazioniPrecedenti_WS(
                                ByVal objOpzioniImportImpresa As clsImpresa,
                                ByVal KeyImpresa_ORIGINE As clsKeyImpresa,
                                ByRef KeyImpresa_DESTINAZIONE As clsKeyImpresa,
                                ByRef ErrFLAG As Integer
                            )

        Try

            _objScriviHelper.Elabora_XML_PUA_LetamazioniPrecedenti_Salva(
                objOpzioniImportImpresa,
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyImpresa_ORIGINE.Piva,
                KeyImpresa_DESTINAZIONE.Piva,
                _opzioni.SuperUser_CodFiscale_DESTINAZIONE,
                _opzioni.SuperUser_CodFiscale_DESTINAZIONE
                )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine("Copia PUA_LetamazioniPrecedenti] : " & ex.Message.ToString)
        End Try

    End Sub

    Private Sub G2G_Copia_PUA_LetamazioniPrecedenti_WSReverse(
                                ByVal objOpzioniImportImpresa As clsImpresa,
                                ByVal KeyImpresa_ORIGINE As clsKeyImpresa,
                                ByRef KeyImpresa_DESTINAZIONE As clsKeyImpresa,
                                ByRef ErrFLAG As Integer
                            )

        Try

            _objScriviHelper.Elabora_XML_PUA_LetamazioniPrecedenti_SalvaReverse(
                objOpzioniImportImpresa,
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyImpresa_ORIGINE.Piva,
                KeyImpresa_DESTINAZIONE.Piva,
                _opzioni.SuperUser_CodFiscale_DESTINAZIONE,
                _opzioni.SuperUser_CodFiscale_DESTINAZIONE
                )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine("Copia PUA_LetamazioniPrecedenti] : " & ex.Message.ToString)
        End Try

    End Sub

    Private Sub G2G_Copia_PUA_Effluente_PeriodoDivieto_WS(ByVal objOpzioniImportImpresa As clsImpresa,
                                ByVal KeyImpresa_ORIGINE As clsKeyImpresa,
                                ByRef KeyImpresa_DESTINAZIONE As clsKeyImpresa,
                                ByRef ErrFLAG As Integer)

        Try

            _objScriviHelper.Elabora_XML_PUA_Effluente_PeriodoDivieto_Salva(
                objOpzioniImportImpresa,
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyImpresa_ORIGINE.Piva,
                KeyImpresa_DESTINAZIONE.Piva,
                _opzioni.SuperUser_CodFiscale_DESTINAZIONE,
                _opzioni.SuperUser_CodFiscale_DESTINAZIONE
                )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine("Copia PUA_Effluente_PeriodoDivieto] : " & ex.Message.ToString)
        End Try


    End Sub

    Private Sub G2G_Copia_Anagrafe_VincoliAgronomici_WS(
                                ByVal objOpzioniImportImpresa As clsImpresa,
                                ByVal KeyImpresa_ORIGINE As clsKeyImpresa,
                                ByRef KeyImpresa_DESTINAZIONE As clsKeyImpresa,
                                ByRef ErrFLAG As Integer
                            )

        Try

            _objScriviHelper.Elabora_XML_Anagrafe_VincoliAgronomici_Salva(
                objOpzioniImportImpresa,
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyImpresa_ORIGINE.Piva,
                KeyImpresa_DESTINAZIONE.Piva,
                _opzioni.SuperUser_CodFiscale_DESTINAZIONE,
                _opzioni.SuperUser_CodFiscale_DESTINAZIONE
                )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine("Copia Anagrafe_VincoliAgronomici] : " & ex.Message.ToString)
        End Try

    End Sub

    Private Sub G2G_Copia_Anagrafe_VincoliAgronomici_WSReverse(
                                ByVal objOpzioniImportImpresa As clsImpresa,
                                ByVal KeyImpresa_ORIGINE As clsKeyImpresa,
                                ByRef KeyImpresa_DESTINAZIONE As clsKeyImpresa,
                                ByRef ErrFLAG As Integer
                            )

        Try

            _objScriviHelper.Elabora_XML_Anagrafe_VincoliAgronomici_SalvaReverse(
                objOpzioniImportImpresa,
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyImpresa_ORIGINE.Piva,
                KeyImpresa_DESTINAZIONE.Piva,
                _opzioni.SuperUser_CodFiscale_DESTINAZIONE,
                _opzioni.SuperUser_CodFiscale_DESTINAZIONE
                )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine("Copia Anagrafe_VincoliAgronomici] : " & ex.Message.ToString)
        End Try

    End Sub

    Private Sub G2G_Copia_PUA_WS(
                                ByVal objOpzioniImportImpresa As clsImpresa,
                                ByVal KeyImpresa_ORIGINE As clsKeyImpresa,
                                ByRef KeyImpresa_DESTINAZIONE As clsKeyImpresa,
                                ByRef ErrFLAG As Integer
                            )

        Try

            _objScriviHelper.Elabora_XML_PUA_Salva(
                objOpzioniImportImpresa,
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyImpresa_ORIGINE.Piva,
                KeyImpresa_DESTINAZIONE.Piva,
                _opzioni.SuperUser_CodFiscale_DESTINAZIONE,
                _opzioni.SuperUser_CodFiscale_DESTINAZIONE
                )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.Append("Copia PUA] : " & ex.Message.ToString & vbCrLf)
        End Try

    End Sub

    Private Sub G2G_Copia_PUA_WSReverse(
                                ByVal objOpzioniImportImpresa As clsImpresa,
                                ByVal KeyImpresa_ORIGINE As clsKeyImpresa,
                                ByRef KeyImpresa_DESTINAZIONE As clsKeyImpresa,
                                ByRef ErrFLAG As Integer
                            )

        Try

            _objScriviHelper.Elabora_XML_PUA_SalvaReverse(
                objOpzioniImportImpresa,
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyImpresa_ORIGINE.Piva,
                KeyImpresa_DESTINAZIONE.Piva,
                _opzioni.SuperUser_CodFiscale_DESTINAZIONE,
                _opzioni.SuperUser_CodFiscale_DESTINAZIONE
                )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.Append("Copia PUA] : " & ex.Message.ToString & vbCrLf)
        End Try

    End Sub

    Private Sub G2G_Copia_PUA(
                                       ByVal KeyImpresa_ORIGINE As clsKeyImpresa,
                                       ByRef KeyImpresa_DESTINAZIONE As clsKeyImpresa,
                                       ByRef ErrFLAG As Integer
                                        )

        Try

            _objScriviHelper.Elabora_PUA_Salva(
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyImpresa_ORIGINE.Piva,
                KeyImpresa_DESTINAZIONE.Piva
                )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.Append("Copia PUA] : " & ex.Message.ToString & vbCrLf)
        End Try

    End Sub



    '##########################################################
    Private Sub G2G_Copia_Profilazione(
                                       ByVal KeyImpresa_ORIGINE As clsKeyImpresa,
                                       ByRef KeyImpresa_DESTINAZIONE As clsKeyImpresa,
                                       ByRef ErrFLAG As Integer,
                                       ByVal Flag_DefaultGlobali As Boolean
                                        )

        Dim NomeRoutine As String = "G2G_Copia_Profilazione"

        Try

            _objScriviHelper.Elabora_Profilazione_Salva(
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyImpresa_ORIGINE.Piva,
                KeyImpresa_DESTINAZIONE.Piva,
                Flag_DefaultGlobali)

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine(" [" & NomeRoutine & "] : " & ex.Message.ToString)
        End Try


    End Sub

    '##########################################################
    Private Sub G2G_Copia_CAC_Codifica(ByRef ErrFLAG As Integer)

        Dim NomeRoutine As String = "G2G_Copia_CAC_Codifica"

        Try

            _objScriviHelper.Elabora_CAC_Codifica_Animali_Salva(
                            _opzioni,
                            _Log_G2G,
                            _Log_Errori,
                            _Log_Riepilogo)



            _objScriviHelper.Elabora_CAC_Codifica_Zone_Salva(
                            _opzioni,
                            _Log_G2G,
                            _Log_Errori,
                            _Log_Riepilogo)


            _objScriviHelper.Elabora_CAC_Codifica_Cultivar_Salva(
                            _opzioni,
                            _Log_G2G,
                            _Log_Errori,
                            _Log_Riepilogo)

            _objScriviHelper.Elabora_CAC_Codifica_FormeAllevamento_Salva(
                            _opzioni,
                            _Log_G2G,
                            _Log_Errori,
                            _Log_Riepilogo)

            _objScriviHelper.Elabora_CAC_Codifica_InfoAggiuntive_Salva(
                            _opzioni,
                            _Log_G2G,
                            _Log_Errori,
                            _Log_Riepilogo)

            _objScriviHelper.Elabora_CAC_Codifica_Portinnesti_Salva(
                            _opzioni,
                            _Log_G2G,
                            _Log_Errori,
                            _Log_Riepilogo)

            _objScriviHelper.Elabora_CAC_Codifica_Veg_Cod_Salva(
                            _opzioni,
                            _Log_G2G,
                            _Log_Errori,
                            _Log_Riepilogo)

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine(" [" & NomeRoutine & "] : " & ex.Message.ToString)
        End Try


    End Sub

#End Region



#Region "Centri aziendali"

    '###########################################################################################################
    Private Sub G2G_Lista_Centri_WS(ByVal objOpzioniImportImpresa As clsImpresa,
                                    ByVal KeyImpresa_ORIGINE As clsKeyImpresa,
                                    ByVal KeyImpresa_DESTINAZIONE As clsKeyImpresa,
                                    ByVal CodiceGias_ORIGINE As clsCodiceGias,
                                    ByVal CodiceGias_DESTINAZIONE As clsCodiceGias,
                                    ByRef ErrFLAG As Integer)

        Dim NomeRoutine As String = "G2G_Lista_Centri_WS"

        Dim KeyCentro_ORIGINE As New clsKeyCentro
        Dim KeyCentro_DESTINAZIONE As New clsKeyCentro

        Try
            _opzioni.objParametri_Server_GIAS_ORIGINE.FlagVisibilita = enumVisibilita.Visibilita_Tutti

            Dim objORIGINE As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read

            Dim DT = objORIGINE.Leggi(KeyImpresa_ORIGINE.Piva, 0,
                                      enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                      "", "", _opzioni.objParametri_Server_GIAS_ORIGINE)

            _opzioni.objParametri_Server_GIAS_ORIGINE.FlagVisibilita = enumVisibilita.visibilita_SoloNonInviati

            If (Not IsNothing(DT)) AndAlso (DT.Rows.Count > 0) Then

                For Each row As Data.DataRow In DT.Rows

                    KeyCentro_ORIGINE.Piva = KeyImpresa_ORIGINE.Piva
                    KeyCentro_ORIGINE.Sa_Cod = row.Item("Sa_Cod")
                    KeyCentro_DESTINAZIONE.Piva = KeyImpresa_DESTINAZIONE.Piva

                    G2GUtility.Log(_Log_G2G, "Centro " & KeyCentro_ORIGINE.Sa_Cod & " : Inizio trasferimento")

                    G2G_Gestione_Centro_WS(
                                objOpzioniImportImpresa,
                                KeyCentro_ORIGINE,
                                KeyCentro_DESTINAZIONE,
                                CodiceGias_ORIGINE,
                                CodiceGias_DESTINAZIONE,
                                ErrFLAG)

                    If ErrFLAG = 0 Then
                        G2GUtility.Log(_Log_G2G, "Centro " & KeyCentro_ORIGINE.Sa_Cod & " : Fine trasferimento")
                    Else
                        Exit Sub
                    End If

                Next

                DT.Dispose()

            End If

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.Append("Centro " & KeyCentro_ORIGINE.Sa_Cod & " : Errore : " & ex.Message & vbCrLf)
        End Try

    End Sub


    Private Sub G2G_Lista_Centri_WSReverse(ByVal objOpzioniImportImpresa As clsImpresa,
                                           ByVal KeyImpresa_ORIGINE As clsKeyImpresa,
                                           ByVal KeyImpresa_DESTINAZIONE As clsKeyImpresa,
                                           ByVal CodiceGias_ORIGINE As clsCodiceGias,
                                           ByVal CodiceGias_DESTINAZIONE As clsCodiceGias,
                                           ByRef ErrFLAG As Integer)

        Dim NomeRoutine As String = "G2G_Lista_Centri_WS"

        Dim KeyCentro_ORIGINE As New clsKeyCentro
        Dim KeyCentro_DESTINAZIONE As New clsKeyCentro

        Try
            _opzioni.objParametri_Server_GIAS_ORIGINE.FlagVisibilita = enumVisibilita.Visibilita_Tutti

            Dim objORIGINE As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read

            Dim DT = objORIGINE.Leggi(KeyImpresa_ORIGINE.Piva, 0,
                                      enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                      "", "", _opzioni.objParametri_Server_GIAS_ORIGINE)

            _opzioni.objParametri_Server_GIAS_ORIGINE.FlagVisibilita = enumVisibilita.visibilita_SoloNonInviati

            If (Not IsNothing(DT)) AndAlso (DT.Rows.Count > 0) Then

                For Each row As Data.DataRow In DT.Rows

                    KeyCentro_ORIGINE.Piva = KeyImpresa_ORIGINE.Piva
                    KeyCentro_ORIGINE.Sa_Cod = row.Item("Sa_Cod")
                    KeyCentro_DESTINAZIONE.Piva = KeyImpresa_DESTINAZIONE.Piva

                    G2GUtility.Log(_Log_G2G, "Centro " & KeyCentro_ORIGINE.Sa_Cod & " : Inizio trasferimento")

                    G2G_Gestione_Centro_WSReverse(
                                objOpzioniImportImpresa,
                                KeyCentro_ORIGINE,
                                KeyCentro_DESTINAZIONE,
                                CodiceGias_ORIGINE,
                                CodiceGias_DESTINAZIONE,
                                ErrFLAG)

                    If ErrFLAG = 0 Then
                        G2GUtility.Log(_Log_G2G, "Centro " & KeyCentro_ORIGINE.Sa_Cod & " : Fine trasferimento")
                    Else
                        Exit Sub
                    End If

                Next

                DT.Dispose()

            End If

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.Append("Centro " & KeyCentro_ORIGINE.Sa_Cod & " : Errore : " & ex.Message & vbCrLf)
        End Try

    End Sub

    '###########################################################################################################
    Private Sub G2G_Lista_Centri(ByVal objOpzioniImportImpresa As clsImpresa,
                                 ByVal KeyImpresa_ORIGINE As clsKeyImpresa,
                                 ByVal KeyImpresa_DESTINAZIONE As clsKeyImpresa,
                                 ByVal CodiceGias_ORIGINE As clsCodiceGias,
                                 ByVal CodiceGias_DESTINAZIONE As clsCodiceGias,
                                 ByRef ErrFLAG As Integer)

        Dim NomeRoutine As String = "G2G_Lista_Centri"

        Dim KeyCentro_ORIGINE As New clsKeyCentro
        Dim KeyCentro_DESTINAZIONE As New clsKeyCentro

        Try
            _opzioni.objParametri_Server_GIAS_ORIGINE.FlagVisibilita = enumVisibilita.Visibilita_Tutti

            Dim DT As New DataTable
            Dim objORIGINE As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read

            DT = objORIGINE.Leggi(KeyImpresa_ORIGINE.Piva, 0,
                                  enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                  "", "",
                                  _opzioni.objParametri_Server_GIAS_ORIGINE)

            _opzioni.objParametri_Server_GIAS_ORIGINE.FlagVisibilita = enumVisibilita.visibilita_SoloNonInviati

            If (Not IsNothing(DT)) AndAlso (DT.Rows.Count > 0) Then

                For Each row As Data.DataRow In DT.Rows

                    'Vanni, invio sempre i centri a prescindere dal filtro poiché potrebbe trattarsi di centri che ospitano magazzini o altre entità
                    '   non dipendenti da impianti filtrati

                    '' VAnni: 20/5/2019: verifico se il centro aziendale va inviato o meno rispetto alla lista degli impianti.. 
                    ''    esiste impianto su centro oppure lista vuota = invia tutto
                    'Dim xInvioCentro As Boolean = ((From iC In KeyImpresa_ORIGINE.ListaImpianti
                    '                                Where iC.sa_cod = CInt(row.Item("Sa_cod"))).ToList.Count > 0)

                    'If KeyImpresa_ORIGINE.ListaImpianti.Count = 0 OrElse xInvioCentro Then


                    KeyCentro_ORIGINE.Piva = KeyImpresa_ORIGINE.Piva
                    KeyCentro_ORIGINE.Sa_Cod = row.Item("Sa_Cod")
                    KeyCentro_DESTINAZIONE.Piva = KeyImpresa_DESTINAZIONE.Piva

                    _Log_G2G.Append("Centro " & KeyCentro_ORIGINE.Sa_Cod & " : Inizio trasferimento " & vbCrLf)

                    Dim CentroGiaInviato As Boolean = Nothing

                    G2G_Gestione_Centro(
                                objOpzioniImportImpresa,
                                KeyCentro_ORIGINE,
                                KeyCentro_DESTINAZIONE,
                                CodiceGias_ORIGINE,
                                CodiceGias_DESTINAZIONE,
                                ErrFLAG,
                                CentroGiaInviato)

                    '  Vanni, 19/05/2014 15:14:18: test linq to entities...
                    If Not CentroGiaInviato Then
                        _objFunzioni_Global.ImpreseADD(
                             New G2G_Recode_Imprese With {
                                .From_PivaSuperUser = _opzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser,
                                .To_PivaSuperUser = _opzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser,
                                .FROM_Piva = KeyCentro_ORIGINE.Piva,
                                .FROM_SaCod = KeyCentro_ORIGINE.Sa_Cod,
                                .To_Piva = KeyCentro_DESTINAZIONE.Piva,
                                .TO_SaCod = KeyCentro_DESTINAZIONE.Sa_Cod
                            })
                    End If

                    If ErrFLAG = 0 Then
                        _Log_G2G.Append("Centro " & KeyCentro_ORIGINE.Sa_Cod & " : Trasferito ! " & vbCrLf)
                    Else
                        Exit Sub
                    End If


                    'End If
                    ''verifica se il centro va inviato

                Next
                'centro aziendale

                DT.Dispose()

            End If

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.Append("Centro " & KeyCentro_ORIGINE.Sa_Cod & " : Errore : " & ex.Message & vbCrLf)
        End Try

    End Sub

    '###########################################################################################################
    Private Sub G2G_Copia_Impresa_WS_EF(
                                ByVal KeyImpresa_ORIGINE As clsKeyImpresa,
                                ByVal KeyImpresa_DESTINAZIONE As clsKeyImpresa,
                                ByVal KeyImpresa_Padre_DESTINAZIONE As clsKeyImpresa,
                                ByRef ErrFLAG As Integer)

        Try

            _objScriviHelper.ScriviImpreseXML(
                    _opzioni,
                    _Log_G2G,
                    _Log_Errori,
                    _Log_Riepilogo,
                    KeyImpresa_ORIGINE.Piva,
                    KeyImpresa_DESTINAZIONE.Piva,
                    KeyImpresa_Padre_DESTINAZIONE.Piva
                )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.Append("copia impresa: " & ex.Message.ToString & vbCrLf)
        End Try

    End Sub

    Private Sub G2G_Copia_Impresa_WS_EF_Reverse(
                                ByVal KeyImpresa_ORIGINE As clsKeyImpresa,
                                ByVal KeyImpresa_DESTINAZIONE As clsKeyImpresa,
                                ByVal KeyImpresa_Padre_DESTINAZIONE As clsKeyImpresa,
                                ByRef ErrFLAG As Integer)

        Try

            _objScriviHelper.ScriviImpreseXML_Reverse(
                    _opzioni,
                    _Log_G2G,
                    _Log_Errori,
                    _Log_Riepilogo,
                    KeyImpresa_ORIGINE.Piva,
                    KeyImpresa_DESTINAZIONE.Piva,
                    KeyImpresa_Padre_DESTINAZIONE.Piva
                )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.Append("copia impresa: " & ex.Message.ToString & vbCrLf)
        End Try

    End Sub

    '###########################################################################################################
    Private Sub G2G_Copia_Impresa_WS(
                                ByVal KeyImpresa_ORIGINE As clsKeyImpresa,
                                ByVal KeyImpresa_DESTINAZIONE As clsKeyImpresa,
                                ByVal KeyImpresa_Padre_DESTINAZIONE As clsKeyImpresa,
                                ByRef ErrFLAG As Integer,
                                ByRef esisteImpresaDestinazione As Boolean)

        Try

            _objScriviHelper.Elabora_XML_Imprese_Salva(
                    _opzioni,
                    _Log_G2G,
                    _Log_Errori,
                    _Log_Riepilogo,
                    KeyImpresa_ORIGINE.Piva,
                    KeyImpresa_DESTINAZIONE.Piva,
                    KeyImpresa_Padre_DESTINAZIONE.Piva,
                    _opzioni.SuperUser_CodFiscale_DESTINAZIONE,
                    esisteImpresaDestinazione
                )

            _objScriviHelper.Elabora_XML_Imprese_Modifica(
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyImpresa_ORIGINE.Piva,
                KeyImpresa_DESTINAZIONE.Piva,
                KeyImpresa_Padre_DESTINAZIONE.Piva,
                _opzioni.SuperUser_CodFiscale_DESTINAZIONE
            )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.Append("copia impresa: " & ex.Message.ToString & vbCrLf)
        End Try

    End Sub

    '###########################################################################################################
    Private Function G2G_Copia_Centro_WS_EF(ByVal KeyCentro_ORIGINE As clsKeyCentro,
                                         ByRef KeyCentro_DESTINAZIONE As clsKeyCentro,
                                         ByRef ErrFLAG As Integer) As Boolean

        Try

            Dim CentroAggiornato =
                _objScriviHelper.Elabora_XML_CentriAziendali_Salva(
                    _opzioni,
                    _Log_G2G,
                    _Log_Errori,
                    _Log_Riepilogo,
                    KeyCentro_ORIGINE.Piva,
                    KeyCentro_ORIGINE.Sa_Cod,
                    KeyCentro_DESTINAZIONE.Piva
                )

            ' ricavo la decodifica del centro aziendale sulla destinazione
            Dim recode = _objScriviHelper.Recode_ImpreseCentri(_opzioni, KeyCentro_ORIGINE.Piva, KeyCentro_ORIGINE.Sa_Cod)
            If recode IsNot Nothing Then
                KeyCentro_DESTINAZIONE.Piva = recode.To_Piva
                KeyCentro_DESTINAZIONE.Sa_Cod = recode.TO_SaCod
            End If

            Return CentroAggiornato

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.Append("copia centro: " & ex.Message.ToString & vbCrLf)
            Return False

        End Try

    End Function

    Private Function G2G_Copia_Centro_WS_EFReverse(ByVal KeyCentro_ORIGINE As clsKeyCentro,
                                         ByRef KeyCentro_DESTINAZIONE As clsKeyCentro,
                                         ByRef ErrFLAG As Integer) As Boolean

        Try

            Dim CentroAggiornato =
                _objScriviHelper.Elabora_XML_CentriAziendali_SalvaReverse(
                    _opzioni,
                    _Log_G2G,
                    _Log_Errori,
                    _Log_Riepilogo,
                    KeyCentro_ORIGINE.Piva,
                    KeyCentro_ORIGINE.Sa_Cod,
                    KeyCentro_DESTINAZIONE.Piva
                )

            ' ricavo la decodifica del centro aziendale sulla destinazione
            Dim recode = _objScriviHelper.Recode_ImpreseCentriReverse(_opzioni, KeyCentro_ORIGINE.Piva, KeyCentro_ORIGINE.Sa_Cod)
            If recode IsNot Nothing Then
                KeyCentro_DESTINAZIONE.Piva = recode.FROM_Piva
                KeyCentro_DESTINAZIONE.Sa_Cod = recode.FROM_SaCod
            End If

            Return CentroAggiornato

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.Append("copia centro: " & ex.Message.ToString & vbCrLf)
            Return False

        End Try

    End Function

    '###########################################################################################################
    Private Function G2G_Copia_Centro_WS(ByVal KeyCentro_ORIGINE As clsKeyCentro,
                                         ByRef KeyCentro_DESTINAZIONE As clsKeyCentro,
                                         ByRef ErrFLAG As Integer) As Boolean

        Try

            ' ricavo la decodifica del centro aziendale sulla destinazione
            Dim recode = _objScriviHelper.Recode_ImpreseCentri(_opzioni, KeyCentro_ORIGINE.Piva, KeyCentro_ORIGINE.Sa_Cod)

            If recode Is Nothing Then

                Return _objScriviHelper.Elabora_XML_CentriAziendali_Salva(
                    _opzioni,
                    _Log_G2G,
                    _Log_Errori,
                    _Log_Riepilogo,
                    KeyCentro_ORIGINE.Piva,
                    KeyCentro_ORIGINE.Sa_Cod,
                    KeyCentro_DESTINAZIONE.Piva,
                    KeyCentro_DESTINAZIONE.Sa_Cod
                )

            Else

                KeyCentro_DESTINAZIONE.Piva = recode.To_Piva
                KeyCentro_DESTINAZIONE.Sa_Cod = recode.TO_SaCod

                Return _objScriviHelper.Elabora_XML_CentriAziendali_Modifica(
                    _opzioni,
                    _Log_G2G,
                    _Log_Errori,
                    _Log_Riepilogo,
                    KeyCentro_ORIGINE.Piva,
                    KeyCentro_ORIGINE.Sa_Cod,
                    KeyCentro_DESTINAZIONE.Piva,
                    KeyCentro_DESTINAZIONE.Sa_Cod
                )

            End If

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.Append("copia centro: " & ex.Message.ToString & vbCrLf)
            Return False

        End Try

    End Function

    '###########################################################################################################
    Private Sub G2G_Gestione_Centro_WS(ByVal objOpzioniImportImpresa As clsImpresa,
                                       ByVal KeyCentro_ORIGINE As clsKeyCentro,
                                       ByRef KeyCentro_DESTINAZIONE As clsKeyCentro,
                                       ByRef CodiceGias_ORIGINE As clsCodiceGias,
                                       ByRef CodiceGias_DESTINAZIONE As clsCodiceGias,
                                       ByRef ErrFLAG As Integer)

        Dim CentroAggiornato As Boolean = False

        ' copia Centro Aziendale
        If _objScriviHelper.NuovaLogicaRecode Then
            CentroAggiornato = G2G_Copia_Centro_WS_EF(KeyCentro_ORIGINE, KeyCentro_DESTINAZIONE, ErrFLAG)
        Else
            CentroAggiornato = G2G_Copia_Centro_WS(KeyCentro_ORIGINE, KeyCentro_DESTINAZIONE, ErrFLAG)
        End If

        ' copia Particelle Catastali (solo se è stato inserito/modificato il centro aziendale)
        If ErrFLAG = 0 AndAlso CentroAggiornato AndAlso _objOpzioniImportSingolaImpresa.Flagimporta_catasto Then
            If _objScriviHelper.NuovaLogicaRecode Then
                G2G_CopiaCentri_Particelle_WS_EF(objOpzioniImportImpresa, KeyCentro_ORIGINE, KeyCentro_DESTINAZIONE, ErrFLAG)
            Else
                G2G_CopiaCentri_Particelle_WS(objOpzioniImportImpresa, KeyCentro_ORIGINE, KeyCentro_DESTINAZIONE, ErrFLAG)
            End If
        End If

        ' copia Fabbricati
        If ErrFLAG = 0 Then
            If _objScriviHelper.NuovaLogicaRecode Then
                G2G_CopiaCentri_Fabbricati_WS_EF(objOpzioniImportImpresa, KeyCentro_ORIGINE, KeyCentro_DESTINAZIONE, ErrFLAG)
            Else
                G2G_CopiaCentri_Fabbricati_WS(KeyCentro_ORIGINE, KeyCentro_DESTINAZIONE, ErrFLAG)
            End If
        End If

        ' copia Piano Colturale (CAMPI, APPEZZAMENTI, IMPIANTI)
        If ErrFLAG = 0 AndAlso _objOpzioniImportSingolaImpresa.Flagimporta_pianocolturale Then
            If _objScriviHelper.NuovaLogicaRecodePC Then
                G2G_Copia_Piano_Colturale_WS_EF(objOpzioniImportImpresa, KeyCentro_ORIGINE, KeyCentro_DESTINAZIONE, ErrFLAG)
            Else
                G2G_Copia_Appezzamenti_Centro_WS(objOpzioniImportImpresa, KeyCentro_ORIGINE, KeyCentro_DESTINAZIONE, ErrFLAG)
            End If
        End If

        ' copia Distinte
        If ErrFLAG = 0 AndAlso _objOpzioniImportSingolaImpresa.flagimporta_distinta Then
            If Not _objScriviHelper.NuovaLogicaRecodePC Then
                G2G_Copia_Distinta_Centro(objOpzioniImportImpresa, KeyCentro_ORIGINE, KeyCentro_DESTINAZIONE, ErrFLAG)
            End If
        End If


    End Sub

    Private Sub G2G_Gestione_Centro_WSReverse(ByVal objOpzioniImportImpresa As clsImpresa,
                                       ByVal KeyCentro_ORIGINE As clsKeyCentro,
                                       ByRef KeyCentro_DESTINAZIONE As clsKeyCentro,
                                       ByRef CodiceGias_ORIGINE As clsCodiceGias,
                                       ByRef CodiceGias_DESTINAZIONE As clsCodiceGias,
                                       ByRef ErrFLAG As Integer)

        Dim CentroAggiornato As Boolean = False

        ' copia Centro Aziendale
        If _objScriviHelper.NuovaLogicaRecode Then
            CentroAggiornato = G2G_Copia_Centro_WS_EFReverse(KeyCentro_ORIGINE, KeyCentro_DESTINAZIONE, ErrFLAG)
        Else
            Throw New Exception("Vecchia logica recode non supportata")
        End If

        ' copia Particelle Catastali (solo se è stato inserito/modificato il centro aziendale)
        If ErrFLAG = 0 AndAlso CentroAggiornato AndAlso _objOpzioniImportSingolaImpresa.Flagimporta_catasto Then
            If _objScriviHelper.NuovaLogicaRecode Then
                G2G_CopiaCentri_Particelle_WS_EFReverse(objOpzioniImportImpresa, KeyCentro_ORIGINE, KeyCentro_DESTINAZIONE, ErrFLAG)
            Else
                Throw New Exception("Vecchia logica recode non supportata")
            End If
        End If

        ' copia Fabbricati
        If ErrFLAG = 0 Then
            If _objScriviHelper.NuovaLogicaRecode Then
                G2G_CopiaCentri_Fabbricati_WS_EFReverse(KeyCentro_ORIGINE, KeyCentro_DESTINAZIONE, ErrFLAG)
            Else
                Throw New Exception("Vecchia logica recode non supportata")
            End If
        End If

        ' copia Piano Colturale (CAMPI, APPEZZAMENTI, IMPIANTI)
        If ErrFLAG = 0 AndAlso _objOpzioniImportSingolaImpresa.Flagimporta_pianocolturale Then
            If _objScriviHelper.NuovaLogicaRecodePC Then
                G2G_Copia_Piano_Colturale_WS_EFReverse(objOpzioniImportImpresa, KeyCentro_ORIGINE, KeyCentro_DESTINAZIONE, ErrFLAG)
            Else
                Throw New Exception("Vecchia logica recode non supportata")
            End If
        End If

    End Sub

    '###########################################################################################################
    Private Sub G2G_Gestione_Centro(
                                ByVal objOpzioniImportImpresa As clsImpresa,
                                ByVal KeyCentro_ORIGINE As clsKeyCentro,
                                ByRef KeyCentro_DESTINAZIONE As clsKeyCentro,
                                ByRef CodiceGias_ORIGINE As clsCodiceGias,
                                ByRef CodiceGias_DESTINAZIONE As clsCodiceGias,
                                ByRef ErrFLAG As Integer,
                                ByRef CentroGiaInviato As Boolean
                                )

        '  Marco Grilli, 30/07/2014 10:17:50: Ricerco se il centro è già sul G2G
        Dim objORIGINE As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
        CentroGiaInviato = objORIGINE.VerificaEsistenzaCAsuG2G(
                                                    KeyCentro_ORIGINE.Piva,
                                                    KeyCentro_ORIGINE.Sa_Cod,
                                                    _opzioni.objParametri_Server_GIAS_ORIGINE
                                                    )

        If Not CentroGiaInviato Then

            '------------------------------ CENTRO AZIENDALE
            G2G_Copia_Centro(
                                KeyCentro_ORIGINE,
                                KeyCentro_DESTINAZIONE,
                                CodiceGias_ORIGINE,
                                CodiceGias_DESTINAZIONE,
                                ErrFLAG)

            If ErrFLAG <> 0 Then
                G2G_Chiusura_Transazione(2)
                Exit Sub
            End If

            '------------------------------ INDIRIZZO
            G2G_Copia_Indirizzo_Centro(
                                    KeyCentro_ORIGINE,
                                    KeyCentro_DESTINAZIONE,
                                    ErrFLAG)

            If ErrFLAG <> 0 Then
                G2G_Chiusura_Transazione(2)
                Exit Sub
            End If

            '------------------------------ CODICI
            G2G_Copia_Centri_Codici(
                                    KeyCentro_ORIGINE,
                                    KeyCentro_DESTINAZIONE,
                                    ErrFLAG)

            If ErrFLAG <> 0 Then
                G2G_Chiusura_Transazione(2)
                Exit Sub
            End If

            '------------------------------ UTENTI x STRUTTURE
            G2G_Scrivi_UtentixStrutture(
                                    KeyCentro_ORIGINE,
                                    KeyCentro_DESTINAZIONE,
                                    ErrFLAG)

            If ErrFLAG <> 0 Then
                G2G_Chiusura_Transazione(2)
                Exit Sub
            End If

            '------------------------------ RUBRICA
            G2G_Copia_Rubrica_Centro(
                                    KeyCentro_ORIGINE,
                                    KeyCentro_DESTINAZIONE,
                                    ErrFLAG)

            If ErrFLAG <> 0 Then
                G2G_Chiusura_Transazione(2)
                Exit Sub
            End If

        Else

            KeyCentro_DESTINAZIONE.Sa_Cod = (
                From gg In _objFunzioni_Global.Imprese
                Where gg.FROM_Piva = KeyCentro_ORIGINE.Piva AndAlso
                      gg.FROM_SaCod = KeyCentro_ORIGINE.Sa_Cod
                Select gg.TO_SaCod).FirstOrDefault

        End If

        ' vv''------------------------------ Piano Colturale (APPEZZAMENTI, IMPIANTI)
        If _objOpzioniImportSingolaImpresa.Flagimporta_pianocolturale Then
            G2G_Copia_Appezzamenti_Centro(
                objOpzioniImportImpresa,
                KeyCentro_ORIGINE,
                KeyCentro_DESTINAZIONE,
                ErrFLAG)

            If ErrFLAG <> 0 Then
                G2G_Chiusura_Transazione(2)
                Exit Sub
            End If
        End If

        ''vv ''------------------------------ DISTINTA
        If _objOpzioniImportSingolaImpresa.flagimporta_distinta Then
            G2G_Copia_Distinta_Centro(
                objOpzioniImportImpresa,
                KeyCentro_ORIGINE,
                KeyCentro_DESTINAZIONE,
                ErrFLAG)

            If ErrFLAG <> 0 Then
                G2G_Chiusura_Transazione(2)
                Exit Sub
            End If
        End If

        ''''vv
        If _objOpzioniImportSingolaImpresa.Flagimporta_catasto Then
            G2G_CopiaCentri_Particelle(
                objOpzioniImportImpresa,
                KeyCentro_ORIGINE,
                KeyCentro_DESTINAZIONE,
                ErrFLAG
                )
        End If

        If Not CentroGiaInviato Then
            ''''vv
            If _objOpzioniImportSingolaImpresa.Flagimporta_gis Then
                G2G_CentriXsfondi(
                    KeyCentro_ORIGINE,
                    KeyCentro_DESTINAZIONE,
                    ErrFLAG
                    )
            End If
        End If

        'TODO : 
        G2G_CopiaCentri_Fabbricati(
            KeyCentro_ORIGINE,
            KeyCentro_DESTINAZIONE,
            ErrFLAG
            )


        'TODO :     G2G_Copia_Giacenze.. agenda?

        'TODO : G2G_Lista_Campi()


        'vv ''------------------------------ GRAFICA
        If _objOpzioniImportSingolaImpresa.Flagimporta_gis Then
            G2G_Copia_Grafica_Centro(
                             KeyCentro_ORIGINE,
                             KeyCentro_DESTINAZIONE,
                             ErrFLAG)

            If ErrFLAG <> 0 Then
                G2G_Chiusura_Transazione(2)
                Exit Sub
            End If
        End If



        ''vv ''------------------------------ PLANNING
        If _objOpzioniImportSingolaImpresa.flagimporta_planning Then
            G2G_Copia_Planning_Centro(
                             KeyCentro_ORIGINE,
                             KeyCentro_DESTINAZIONE,
                             ErrFLAG)

            If ErrFLAG <> 0 Then
                G2G_Chiusura_Transazione(2)
                Exit Sub
            End If
        End If




        ''vv------------------------------ BIO
        If _objOpzioniImportSingolaImpresa.flagimporta_pap Then
            G2G_Copia_bio_Centro(
                             KeyCentro_ORIGINE,
                             KeyCentro_DESTINAZIONE,
                             ErrFLAG)

            If ErrFLAG <> 0 Then
                G2G_Chiusura_Transazione(2)
                Exit Sub
            End If
        End If

        ''vv------------------------------ BIO Notifica x centro
        If _objOpzioniImportSingolaImpresa.flagimporta_notificabio Then
            G2G_Copia_bio_notificaXcentro(
                             KeyCentro_ORIGINE,
                             KeyCentro_DESTINAZIONE,
                             ErrFLAG)

            If ErrFLAG <> 0 Then
                G2G_Chiusura_Transazione(2)
                Exit Sub
            End If
        End If

        'vv ''------------------------------ RICETTE relative a centri aziendali
        If _objOpzioniImportSingolaImpresa.flagimporta_ricette Then
            G2G_Copia_ricette_centri(
                             KeyCentro_ORIGINE,
                             KeyCentro_DESTINAZIONE,
                             ErrFLAG)

            If ErrFLAG <> 0 Then
                G2G_Chiusura_Transazione(2)
                Exit Sub
            End If
        End If



        'vv ''------------------------------ Cantine, vasche
        If _objOpzioniImportSingolaImpresa.Flagimporta_LineeProduttive Then
            G2G_Copia_vasche_centri(
                             KeyCentro_ORIGINE,
                             KeyCentro_DESTINAZIONE,
                             ErrFLAG)

            If ErrFLAG <> 0 Then
                G2G_Chiusura_Transazione(2)
                Exit Sub
            End If
        End If


    End Sub

    Private Sub G2G_CentriXsfondi(
                                ByVal KeyCentro_ORIGINE As clsKeyCentro,
                                ByRef KeyCentro_DESTINAZIONE As clsKeyCentro,
                                ByRef ErrFLAG As Integer
                            )
        Try
            _objScriviHelper.Elabora_XML_CentriXSfondi_Salva(
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyCentro_ORIGINE.Piva,
                KeyCentro_ORIGINE.Sa_Cod,
                KeyCentro_DESTINAZIONE.Piva,
                KeyCentro_DESTINAZIONE.Sa_Cod
            )
        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.Append("copia sfondi: " & ex.Message.ToString & vbCrLf)
        End Try


    End Sub

    Private Sub G2G_Copia_Distinta_Centro(
        ByVal objOpzioniImportImpresa As clsImpresa,
        ByVal KeyCentro_ORIGINE As clsKeyCentro,
        ByRef KeyCentro_DESTINAZIONE As clsKeyCentro,
        ByRef ErrFLAG As Integer
    )

        Try
            _objScriviHelper.Elabora_XML_Distinta_Salva(
                objOpzioniImportImpresa,
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyCentro_ORIGINE.Piva,
                KeyCentro_DESTINAZIONE.Piva,
                KeyCentro_ORIGINE.Sa_Cod,
                KeyCentro_DESTINAZIONE.Sa_Cod
            )

            _objScriviHelper.Elabora_XML_Distinta_Modifica(
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyCentro_ORIGINE.Piva,
                KeyCentro_DESTINAZIONE.Piva,
                KeyCentro_ORIGINE.Sa_Cod,
                KeyCentro_DESTINAZIONE.Sa_Cod
            )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.Append("copia Distinta: " & ex.Message.ToString & vbCrLf)
        End Try


    End Sub

    Private Sub G2G_Copia_Appezzamenti_Centro(ByVal objOpzioniImportImpresa As clsImpresa,
                                              ByVal KeyCentro_ORIGINE As clsKeyCentro,
                                              ByRef KeyCentro_DESTINAZIONE As clsKeyCentro,
                                              ByRef ErrFLAG As Integer)

        Try
            _objScriviHelper.Elabora_XML_Appezzamenti_Salva(
                objOpzioniImportImpresa,
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyCentro_ORIGINE.Piva,
                KeyCentro_ORIGINE.Sa_Cod,
                KeyCentro_DESTINAZIONE.Piva,
                KeyCentro_DESTINAZIONE.Sa_Cod
            )

            _objScriviHelper.Elabora_XML_Appezzamenti_Modificati(
                objOpzioniImportImpresa,
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyCentro_ORIGINE.Piva,
                KeyCentro_ORIGINE.Sa_Cod,
                KeyCentro_DESTINAZIONE.Piva,
                KeyCentro_DESTINAZIONE.Sa_Cod
            )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.Append("copia appezzamenti: " & ex.Message.ToString & vbCrLf)
        End Try



    End Sub

    Private Sub G2G_Copia_Piano_Colturale_WS_EF(ByVal objOpzioniImportImpresa As clsImpresa,
                                                ByVal KeyCentro_ORIGINE As clsKeyCentro,
                                                ByRef KeyCentro_DESTINAZIONE As clsKeyCentro,
                                                ByRef ErrFLAG As Integer)

        Try

            _objScriviHelper.ScriviPianoColturaleXML(
                objOpzioniImportImpresa,
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyCentro_ORIGINE.Piva,
                KeyCentro_ORIGINE.Sa_Cod,
                KeyCentro_DESTINAZIONE.Piva,
                KeyCentro_DESTINAZIONE.Sa_Cod
            )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.Append("copia piano colturale: " & ex.Message.ToString & vbCrLf)
        End Try

    End Sub

    Private Sub G2G_Copia_Piano_Colturale_WS_EFReverse(ByVal objOpzioniImportImpresa As clsImpresa,
                                                ByVal KeyCentro_ORIGINE As clsKeyCentro,
                                                ByRef KeyCentro_DESTINAZIONE As clsKeyCentro,
                                                ByRef ErrFLAG As Integer)

        Try

            _objScriviHelper.ScriviPianoColturaleXMLReverse(
                objOpzioniImportImpresa,
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyCentro_ORIGINE.Piva,
                KeyCentro_ORIGINE.Sa_Cod,
                KeyCentro_DESTINAZIONE.Piva,
                KeyCentro_DESTINAZIONE.Sa_Cod
            )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.Append("copia piano colturale: " & ex.Message.ToString & vbCrLf)
        End Try

    End Sub

    Private Sub G2G_Copia_Appezzamenti_Centro_WS(ByVal objOpzioniImportImpresa As clsImpresa,
                                                 ByVal KeyCentro_ORIGINE As clsKeyCentro,
                                                 ByRef KeyCentro_DESTINAZIONE As clsKeyCentro,
                                                 ByRef ErrFLAG As Integer)

        Try
            _objScriviHelper.Elabora_XML_Impianti_Salva(
                objOpzioniImportImpresa,
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyCentro_ORIGINE.Piva,
                KeyCentro_ORIGINE.Sa_Cod,
                KeyCentro_DESTINAZIONE.Piva,
                KeyCentro_DESTINAZIONE.Sa_Cod
            )

            _objScriviHelper.Elabora_XML_Impianti_Modificati(
                objOpzioniImportImpresa,
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyCentro_ORIGINE.Piva,
                KeyCentro_ORIGINE.Sa_Cod,
                KeyCentro_DESTINAZIONE.Piva,
                KeyCentro_DESTINAZIONE.Sa_Cod
            )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.Append("copia impianti: " & ex.Message.ToString & vbCrLf)
        End Try

    End Sub


    Private Sub G2G_Copia_Grafica_Centro(ByVal KeyCentro_ORIGINE As clsKeyCentro,
                                         ByRef KeyCentro_DESTINAZIONE As clsKeyCentro,
                                         ByRef ErrFLAG As Integer)

        Try
            _objScriviHelper.Elabora_XML_Grafica_Salva(
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyCentro_ORIGINE.Piva,
                KeyCentro_ORIGINE.Sa_Cod,
                KeyCentro_DESTINAZIONE.Piva,
                KeyCentro_DESTINAZIONE.Sa_Cod,
                1
            )
        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.Append("copia grafica centro: " & ex.Message.ToString & vbCrLf)
        End Try




    End Sub

    Private Sub G2G_Copia_Planning_Centro(
                                ByVal KeyCentro_ORIGINE As clsKeyCentro,
                                ByRef KeyCentro_DESTINAZIONE As clsKeyCentro,
                                ByRef ErrFLAG As Integer
                            )

        Try
            _objScriviHelper.Elabora_Planning_Salva(
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyCentro_ORIGINE.Piva,
                KeyCentro_ORIGINE.Sa_Cod,
                KeyCentro_DESTINAZIONE.Piva,
                KeyCentro_DESTINAZIONE.Sa_Cod
            )
        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.Append("copia planning centro: " & ex.Message.ToString & vbCrLf)
        End Try

    End Sub

    Private Sub G2G_Copia_Planning_Impresa(
                                ByVal KeyImpresa_ORIGINE As clsKeyImpresa,
                                ByRef KeyImpresa_DESTINAZIONE As clsKeyImpresa,
                                ByRef ErrFLAG As Integer
                            )

        Try
            _objScriviHelper.Elabora_Planning_Salva(
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyImpresa_ORIGINE.Piva,
                0,
                KeyImpresa_DESTINAZIONE.Piva,
                0
            )
        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.Append("copia planning impresa: " & ex.Message.ToString & vbCrLf)
        End Try

    End Sub

    Private Sub G2G_Copia_Planning_WS(
        ByVal objOpzioniImportImpresa As clsImpresa,
        ByVal KeyImpresa_ORIGINE As clsKeyImpresa,
        ByRef KeyImpresa_DESTINAZIONE As clsKeyImpresa,
        ByRef ErrFLAG As Integer
    )

        Try

            _objScriviHelper.Elabora_XML_Planning_Salva_WS(
                objOpzioniImportImpresa,
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyImpresa_ORIGINE.Piva,
                KeyImpresa_DESTINAZIONE.Piva
                )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.Append("Copia Planning] : " & ex.Message.ToString & vbCrLf)
        End Try

    End Sub

    Private Sub G2G_Copia_Planning_WSReverse(
        ByVal objOpzioniImportImpresa As clsImpresa,
        ByVal KeyImpresa_ORIGINE As clsKeyImpresa,
        ByRef KeyImpresa_DESTINAZIONE As clsKeyImpresa,
        ByRef ErrFLAG As Integer
    )

        Try

            _objScriviHelper.Elabora_XML_Planning_Salva_WSReverse(
                objOpzioniImportImpresa,
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyImpresa_ORIGINE.Piva,
                KeyImpresa_DESTINAZIONE.Piva
                )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.Append("Copia Planning] : " & ex.Message.ToString & vbCrLf)
        End Try

    End Sub

    Private Sub G2G_Copia_bio_Centro(
                                ByVal KeyCentro_ORIGINE As clsKeyCentro,
                                ByRef KeyCentro_DESTINAZIONE As clsKeyCentro,
                                ByRef ErrFLAG As Integer
                            )

        Try
            _objScriviHelper.Elabora_BIO_Salva(
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyCentro_ORIGINE.Piva,
                KeyCentro_DESTINAZIONE.Piva,
                KeyCentro_ORIGINE.Sa_Cod,
                KeyCentro_DESTINAZIONE.Sa_Cod
            )
        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.Append("copia bio: " & ex.Message.ToString & vbCrLf)
        End Try

    End Sub

    Private Sub G2G_Copia_ricette_centri(
                                ByVal KeyCentro_ORIGINE As clsKeyCentro,
                                ByRef KeyCentro_DESTINAZIONE As clsKeyCentro,
                                ByRef ErrFLAG As Integer
                            )

        Try
            _objScriviHelper.Elabora_Ricette_XML_Salva(
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyCentro_ORIGINE.Piva,
                KeyCentro_DESTINAZIONE.Piva,
                KeyCentro_ORIGINE.Sa_Cod,
                KeyCentro_DESTINAZIONE.Sa_Cod,
                False
            )
        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.Append("copia Ricetta: " & ex.Message.ToString & vbCrLf)
        End Try

    End Sub


    Private Sub G2G_Copia_vasche_centri(
                                ByVal KeyCentro_ORIGINE As clsKeyCentro,
                                ByRef KeyCentro_DESTINAZIONE As clsKeyCentro,
                                ByRef ErrFLAG As Integer
                            )

        Try
            _objScriviHelper.Elabora_Vasche_Salva(
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyCentro_ORIGINE.Piva,
                KeyCentro_DESTINAZIONE.Piva,
                KeyCentro_ORIGINE.Sa_Cod,
                KeyCentro_DESTINAZIONE.Sa_Cod
            )
        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.Append("copia vasche: " & ex.Message.ToString & vbCrLf)
        End Try

    End Sub

    Private Sub G2G_Copia_bio_notificaXcentro(
                                    ByVal KeyCentro_ORIGINE As clsKeyCentro,
                                    ByRef KeyCentro_DESTINAZIONE As clsKeyCentro,
                                    ByRef ErrFLAG As Integer
                                )

        Try
            _objScriviHelper.Elabora_BIO_Notifica_Salva(
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyCentro_ORIGINE.Piva,
                KeyCentro_DESTINAZIONE.Piva,
                KeyCentro_ORIGINE.Sa_Cod,
                KeyCentro_DESTINAZIONE.Sa_Cod
            )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.Append("copia bio notifica: " & ex.Message.ToString & vbCrLf)
        End Try


    End Sub

    Private Sub G2G_Copia_bio_notificaXImpresa(
                                ByVal KeyImpresa_ORIGINE As clsKeyImpresa,
                                ByRef KeyImpresa_DESTINAZIONE As clsKeyImpresa,
                                ByRef ErrFLAG As Integer
                            )

        Try
            _objScriviHelper.Elabora_BIO_Notifica_Salva(
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyImpresa_ORIGINE.Piva,
                KeyImpresa_DESTINAZIONE.Piva,
                0,
                0
            )
        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.Append("copia bio notifica: " & ex.Message.ToString & vbCrLf)
        End Try

    End Sub

    Private Sub G2G_Copia_bioZeta_Centro(
                                ByVal Piva_ORIGINE As String,
                                ByRef Piva_DESTINAZIONE As String,
                                ByRef ErrFLAG As Integer
                            )

        Try
            _objScriviHelper.Elabora_BIO_Zeta_Salva(
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                Piva_ORIGINE,
                Piva_DESTINAZIONE
            )
        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.Append("copia bio zeta: " & ex.Message.ToString & vbCrLf)
        End Try


    End Sub


    Private Sub G2G_Copia_Liquidita(
                                ByVal KeyImpresa_origine As clsKeyImpresa,
                                ByRef KeyImpresa_Destinazione As clsKeyImpresa,
                                ByRef ErrFLAG As Integer
                            )

        Try
            _objScriviHelper.Elabora_Liquidita_Salva(
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyImpresa_origine.Piva,
                KeyImpresa_Destinazione.Piva
            )
        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.Append("copia liquidita : " & ex.Message.ToString & vbCrLf)
        End Try

    End Sub


    Private Sub G2G_Copia_Agenda(
        ByVal objOpzioniImportImpresa As clsImpresa,
        ByVal efG2G As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
        ByVal KeyImpresa_origine As clsKeyImpresa,
        ByRef KeyImpresa_Destinazione As clsKeyImpresa,
        ByRef ErrFLAG As Integer,
        ByVal TipoOperazioneDB As enum_TipoOperazioneDB
    )

        Try
            _objScriviHelper.Elabora_XML_Agenda_Salva(
                objOpzioniImportImpresa,
                efG2G,
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyImpresa_origine.Piva,
                KeyImpresa_Destinazione.Piva,
                TipoOperazioneDB
            )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.Append("copia Agenda centro: " & ex.Message.ToString & vbCrLf)
        End Try

    End Sub

    Private Sub G2G_Copia_AgendaReverse(
        ByVal objOpzioniImportImpresa As clsImpresa,
        ByVal efG2G As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
        ByVal KeyImpresa_origine As clsKeyImpresa,
        ByRef KeyImpresa_Destinazione As clsKeyImpresa,
        ByRef ErrFLAG As Integer,
        ByVal TipoOperazioneDB As enum_TipoOperazioneDB
    )

        Try
            _objScriviHelper.Elabora_XML_Agenda_SalvaReverse(
                objOpzioniImportImpresa,
                efG2G,
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyImpresa_origine.Piva,
                KeyImpresa_Destinazione.Piva,
                TipoOperazioneDB
            )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.Append("copia Agenda centro: " & ex.Message.ToString & vbCrLf)
        End Try

    End Sub

    Private Sub G2G_CopiaCentri_Fabbricati(
                                ByVal KeyCentro_ORIGINE As clsKeyCentro,
                                ByRef KeyCentro_DESTINAZIONE As clsKeyCentro,
                                ByRef ErrFLAG As Integer
                                )

        Dim NomeRoutine As String = "G2G_CopiaCentri_Fabbricati"

        Try

            _objScriviHelper.Elabora_XML_Fabbricati_Salva(
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyCentro_ORIGINE.Piva,
                KeyCentro_ORIGINE.Sa_Cod,
                KeyCentro_DESTINAZIONE.Piva,
                KeyCentro_DESTINAZIONE.Sa_Cod
            )


        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine(" [" & NomeRoutine & "] : " & ex.Message.ToString)
        End Try

    End Sub

    Private Sub G2G_CopiaCentri_Fabbricati_WS_EF(
                                ByVal objOpzioniImportImpresa As clsImpresa,
                                ByVal KeyCentro_ORIGINE As clsKeyCentro,
                                ByRef KeyCentro_DESTINAZIONE As clsKeyCentro,
                                ByRef ErrFLAG As Integer)

        Dim NomeRoutine As String = "G2G_CopiaCentri_Fabbricati_WS_EF"

        Try

            _objScriviHelper.ScriviFabbricatiXML(
                objOpzioniImportImpresa,
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyCentro_ORIGINE.Piva,
                KeyCentro_ORIGINE.Sa_Cod,
                KeyCentro_DESTINAZIONE.Piva,
                KeyCentro_DESTINAZIONE.Sa_Cod
            )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine(" [" & NomeRoutine & "] : " & ex.Message.ToString)
        End Try

    End Sub

    Private Sub G2G_CopiaCentri_Fabbricati_WS_EFReverse(
                                ByVal KeyCentro_ORIGINE As clsKeyCentro,
                                ByRef KeyCentro_DESTINAZIONE As clsKeyCentro,
                                ByRef ErrFLAG As Integer)

        Dim NomeRoutine As String = "G2G_CopiaCentri_Fabbricati_WS_EF"

        Try

            _objScriviHelper.ScriviFabbricatiXMLReverse(
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyCentro_ORIGINE.Piva,
                KeyCentro_ORIGINE.Sa_Cod,
                KeyCentro_DESTINAZIONE.Piva,
                KeyCentro_DESTINAZIONE.Sa_Cod
            )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine(" [" & NomeRoutine & "] : " & ex.Message.ToString)
        End Try

    End Sub

    Private Sub G2G_CopiaCentri_Fabbricati_WS(
                                ByVal KeyCentro_ORIGINE As clsKeyCentro,
                                ByRef KeyCentro_DESTINAZIONE As clsKeyCentro,
                                ByRef ErrFLAG As Integer)

        Dim NomeRoutine As String = "G2G_CopiaCentri_Fabbricati_WS"

        Try

            _objScriviHelper.Elabora_XML_Fabbricati_Salva(
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyCentro_ORIGINE.Piva,
                KeyCentro_ORIGINE.Sa_Cod,
                KeyCentro_DESTINAZIONE.Piva,
                KeyCentro_DESTINAZIONE.Sa_Cod
            )

            _objScriviHelper.Elabora_XML_Fabbricati_Modifica(
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyCentro_ORIGINE.Piva,
                KeyCentro_ORIGINE.Sa_Cod,
                KeyCentro_DESTINAZIONE.Piva,
                KeyCentro_DESTINAZIONE.Sa_Cod
            )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine(" [" & NomeRoutine & "] : " & ex.Message.ToString)
        End Try

    End Sub

    Private Sub G2G_CopiaCentri_Particelle(
                                            ByVal objOpzioniImportImpresa As clsImpresa,
                                            ByVal KeyCentro_ORIGINE As clsKeyCentro,
                                            ByRef KeyCentro_DESTINAZIONE As clsKeyCentro,
                                            ByRef ErrFLAG As Integer
                                            )

        Try

            _objScriviHelper.Elabora_XML_Particelle_Salva(
                objOpzioniImportImpresa,
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyCentro_ORIGINE.Piva,
                KeyCentro_ORIGINE.Sa_Cod,
                KeyCentro_DESTINAZIONE.Piva,
                KeyCentro_DESTINAZIONE.Sa_Cod
            )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.Append("copia particelle: " & ex.Message.ToString & vbCrLf)
        End Try

    End Sub

    Private Sub G2G_CopiaCentri_Particelle_WS_EF(
                                            ByVal objOpzioniImportImpresa As clsImpresa,
                                            ByVal KeyCentro_ORIGINE As clsKeyCentro,
                                            ByRef KeyCentro_DESTINAZIONE As clsKeyCentro,
                                            ByRef ErrFLAG As Integer)
        Try

            _objScriviHelper.ScriviParticelleXML(
                objOpzioniImportImpresa,
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyCentro_ORIGINE.Piva,
                KeyCentro_ORIGINE.Sa_Cod,
                KeyCentro_DESTINAZIONE.Piva,
                KeyCentro_DESTINAZIONE.Sa_Cod
            )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.Append("copia particelle: " & ex.Message.ToString & vbCrLf)
        End Try

    End Sub

    Private Sub G2G_CopiaCentri_Particelle_WS_EFReverse(
                                            ByVal objOpzioniImportImpresa As clsImpresa,
                                            ByVal KeyCentro_ORIGINE As clsKeyCentro,
                                            ByRef KeyCentro_DESTINAZIONE As clsKeyCentro,
                                            ByRef ErrFLAG As Integer)
        Try

            _objScriviHelper.ScriviParticelleXMLReverse(
                objOpzioniImportImpresa,
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyCentro_ORIGINE.Piva,
                KeyCentro_ORIGINE.Sa_Cod,
                KeyCentro_DESTINAZIONE.Piva,
                KeyCentro_DESTINAZIONE.Sa_Cod
            )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.Append("copia particelle: " & ex.Message.ToString & vbCrLf)
        End Try

    End Sub

    Private Sub G2G_CopiaCentri_Particelle_WS(
                                            ByVal objOpzioniImportImpresa As clsImpresa,
                                            ByVal KeyCentro_ORIGINE As clsKeyCentro,
                                            ByRef KeyCentro_DESTINAZIONE As clsKeyCentro,
                                            ByRef ErrFLAG As Integer
                                            )

        Try

            _objScriviHelper.Elabora_XML_Particelle_Modifica(
                objOpzioniImportImpresa,
                _opzioni,
                _Log_G2G,
                _Log_Errori,
                _Log_Riepilogo,
                KeyCentro_ORIGINE.Piva,
                KeyCentro_ORIGINE.Sa_Cod,
                KeyCentro_DESTINAZIONE.Piva,
                KeyCentro_DESTINAZIONE.Sa_Cod
            )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.Append("copia particelle: " & ex.Message.ToString & vbCrLf)
        End Try

    End Sub

    '###########################################################################################################
    Private Sub G2G_Copia_Centro(ByVal KeyCentro_ORIGINE As clsKeyCentro,
                                 ByRef KeyCentro_DESTINAZIONE As clsKeyCentro,
                                 ByVal CodiceGias_ORIGINE As clsCodiceGias,
                                 ByVal CodiceGias_DESTINAZIONE As clsCodiceGias,
                                 ByRef ErrFLAG As Integer)

        Dim NomeRoutine As String = "G2G_Copia_Centro"

        Try
            Dim DT As New DataTable
            Dim objORIGINE As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read

            DT = objORIGINE.Leggi(KeyCentro_ORIGINE.Piva,
                                  KeyCentro_ORIGINE.Sa_Cod,
                                  enumSelezioneVariabile.Selezione_TabellaCompleta,
                                  "", "",
                                  _opzioni.objParametri_Server_GIAS_ORIGINE)

            If (Not IsNothing(DT)) AndAlso (DT.Rows.Count > 0) Then

                'Recupero un nuovo Sa_Cod
                Dim objSequenze As New AgronicaCoreDataProvider.Agro_Sequenze

                KeyCentro_DESTINAZIONE.Sa_Cod = objSequenze.NuovoId_CentriAziendali(
                                                                        KeyCentro_DESTINAZIONE.Piva,
                                                                        CodiceGias_DESTINAZIONE.BaseCode,
                                                                        CodiceGias_DESTINAZIONE.TopCode,
                                                                        _opzioni.objParametri_Server_GIAS_DESTINAZIONE)

                Dim objDESTINAZIONE As New AgronicaCoreAnagrafeDAL.CentriAziendali_Write

                'v
                objDESTINAZIONE.Scrivi(
                                    KeyCentro_DESTINAZIONE.Piva,
                                    KeyCentro_DESTINAZIONE.Sa_Cod,
                                    CType(DT.Rows(0).Item("Sa_Nome"), String),
                                    CType(DT.Rows(0).Item("X"), Double),
                                    CType(DT.Rows(0).Item("Y"), Double),
                                    CType(DT.Rows(0).Item("ZSLM"), Double),
                                    CType(DT.Rows(0).Item("long"), Double),
                                    CType(DT.Rows(0).Item("lat"), Double),
                                    CType(DT.Rows(0).Item("area"), Double),
                                    CType(DT.Rows(0).Item("ca_sipi"), String),
                                    CType(DT.Rows(0).Item("AT_Prevalente"), String),
                                    CType(DT.Rows(0).Item("Forma_Possesso"), String),
                                    CType(DT.Rows(0).Item("TitoloPossesso"), Integer),
                                    CType(DT.Rows(0).Item("Sup_SAU_Convenzionale"), Double),
                                    CType(DT.Rows(0).Item("Sup_SAU_Conversione"), Double),
                                    CType(DT.Rows(0).Item("Sup_SAU_Biologico"), Double),
                                    CType(DT.Rows(0).Item("Sup_Totale"), Double),
                                    CType(DT.Rows(0).Item("Sup_Bosco"), Double),
                                    CType(DT.Rows(0).Item("Sup_Tare"), Double),
                                    CType(DT.Rows(0).Item("Sup_SAU"), Double),
                                    CType(DT.Rows(0).Item("Sup_Prati"), Double),
                                    CType(DT.Rows(0).Item("Validita_Inizio"), Date),
                                    CType(DT.Rows(0).Item("Validita_Fine"), Date),
                                    _opzioni.objParametri_Server_GIAS_DESTINAZIONE,
                                    _opzioni.objParametri_Utenti_GIAS_DESTINAZIONE,
                                    CType(DT.Rows(0).Item("Data_Creazione"), Date),
                                    CType(DT.Rows(0).Item("Data_Modifica"), Date),
                                    CType(DT.Rows(0).Item("Username_Creazione"), String),
                                    CType(DT.Rows(0).Item("Username_Modifica"), String)
                                )

                DT.Dispose()

            End If

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine(" [" & NomeRoutine & "] : " & ex.Message.ToString)
        End Try

    End Sub

    '###########################################################################################################
    Private Sub G2G_Copia_Indirizzo_Centro(
                                ByVal KeyCentro_ORIGINE As clsKeyCentro,
                                ByVal KeyCentro_DESTINAZIONE As clsKeyCentro,
                                ByRef ErrFLAG As Integer
                                )

        Dim KeyIndirizzo_ORIGINE As New clsKeyIndirizzo

        Dim Validita_Inizio As Date
        Dim Validita_Fine As Date
        Dim Data_Creazione As Date
        Dim Data_Modifica As Date
        Dim Username_Creazione As String = ""
        Dim Usermane_Modifica As String = ""

        G2G_Leggi_CentrixIndirizzi(
                            KeyCentro_ORIGINE,
                            KeyIndirizzo_ORIGINE,
                            ErrFLAG,
                            Validita_Inizio,
                            Validita_Fine,
                            Data_Creazione,
                            Data_Modifica,
                            Username_Creazione,
                            Usermane_Modifica
        )

        If ErrFLAG <> 0 Then
            Exit Sub
        End If

        Dim KeyIndirizzo_DESTINAZIONE As New clsKeyIndirizzo

        G2G_Copia_Indirizzo(
                            KeyIndirizzo_ORIGINE,
                            KeyIndirizzo_DESTINAZIONE,
                            ErrFLAG
            )

        If ErrFLAG <> 0 Then
            Exit Sub
        End If

        G2G_Scrivi_CentrixIndirizzi(
                            KeyCentro_DESTINAZIONE,
                            KeyIndirizzo_DESTINAZIONE,
                            ErrFLAG,
                            Validita_Inizio,
                            Validita_Fine,
                            Data_Creazione,
                            Data_Modifica,
                            Username_Creazione,
                            Usermane_Modifica)

        If ErrFLAG <> 0 Then
            Exit Sub
        End If

    End Sub

    '###########################################################################################################
    Private Sub G2G_Leggi_CentrixIndirizzi(
                                ByVal KeyCentro_ORIGINE As clsKeyCentro,
                                ByRef KeyIndirizzo_ORIGINE As clsKeyIndirizzo,
                                ByRef ErrFLAG As Integer,
                                ByRef Validita_Inizio As Date,
                                ByRef Validita_Fine As Date,
                                ByRef Data_Creazione As Date,
                                ByRef Data_Modifica As Date,
                                ByRef Username_Creazione As String,
                                ByRef Usermane_Modifica As String
                        )

        Dim NomeRoutine As String = "G2G_Leggi_CentrixIndirizzi"

        KeyIndirizzo_ORIGINE.Cod_Indirizzo = 0

        Try
            Dim Cod_Indirizzo As Integer = 0
            Dim objORIGINE As New AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read

            Cod_Indirizzo = objORIGINE.CodIndirizzo_from_PivaSaCod(
                                                            KeyCentro_ORIGINE.Piva,
                                                            KeyCentro_ORIGINE.Sa_Cod,
                                                            _opzioni.objParametri_Server_GIAS_ORIGINE)

            KeyIndirizzo_ORIGINE.Cod_Indirizzo = Cod_Indirizzo


            Dim dt As DataTable =
                objORIGINE.Leggi(
                    KeyCentro_ORIGINE.Piva,
                    KeyCentro_ORIGINE.Sa_Cod,
                    0,
                    0,
                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                    "",
                    "",
                    _opzioni.objParametri_Server_GIAS_ORIGINE
                )

            Validita_Inizio = dt.Rows(0)("CentrixIndirizzi_validita_inizio")
            Validita_Fine = dt.Rows(0)("CentrixIndirizzi_Validita_Fine")
            Data_Creazione = dt.Rows(0)("CentrixIndirizzi_Data_Creazione")
            Data_Modifica = dt.Rows(0)("CentrixIndirizzi_Data_Modifica")

            Username_Creazione = dt.Rows(0)("CentrixIndirizzi_Username_Creazione")
            Usermane_Modifica = dt.Rows(0)("CentrixIndirizzi_Username_Modifica")



        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine(" [" & NomeRoutine & "] : " & ex.Message.ToString)
        End Try

    End Sub

    '###########################################################################################################
    Private Sub G2G_Scrivi_CentrixIndirizzi(
                                ByVal KeyCentro_DESTINAZIONE As clsKeyCentro,
                                ByVal KeyIndirizzo_DESTINAZIONE As clsKeyIndirizzo,
                                ByRef ErrFLAG As Integer,
                                ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                ByVal Data_Creazione As Date,
                                ByVal Data_Modifica As Date,
                                ByVal Username_Creazione As String,
                                ByVal Usermane_Modifica As String)

        Dim NomeRoutine As String = "G2G_Scrivi_CentrixIndirizzi"

        Try
            Dim objDESTINAZIONE As New AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Write

            'V
            objDESTINAZIONE.Scrivi(
                                KeyCentro_DESTINAZIONE.Piva,
                                KeyCentro_DESTINAZIONE.Sa_Cod,
                                KeyIndirizzo_DESTINAZIONE.Cod_Indirizzo,
                                1, Validita_Inizio, Validita_Fine,
                                _opzioni.objParametri_Server_GIAS_DESTINAZIONE,
                                Data_Creazione,
                                Data_Modifica,
                                Username_Creazione,
                                Usermane_Modifica)

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine(" [" & NomeRoutine & "] : " & ex.Message.ToString)
        End Try

    End Sub

    '###########################################################################################################
    Private Sub G2G_Copia_Centri_Codici(ByVal KeyCentro_ORIGINE As clsKeyCentro,
                                        ByVal KeyCentro_DESTINAZIONE As clsKeyCentro,
                                        ByRef ErrFLAG As Integer)

        Dim NomeRoutine As String = "G2G_Copia_Centri_Codici"

        Try
            Dim DT As New DataTable
            Dim objORIGINE As New AgronicaCoreAnagrafeDAL.Centri_Codici_Read

            DT = objORIGINE.Leggi(KeyCentro_ORIGINE.Piva,
                                  KeyCentro_ORIGINE.Sa_Cod,
                                  0, "", "",
                                  enumSelezioneVariabile.Selezione_TabellaCompleta,
                                  "", "",
                                  _opzioni.objParametri_Server_GIAS_ORIGINE)

            If (Not IsNothing(DT)) AndAlso (DT.Rows.Count > 0) Then

                Dim objDESTINAZIONE As New AgronicaCoreAnagrafeDAL.Centri_Codici_Write

                Dim i As Integer

                For i = 0 To DT.Rows.Count - 1

                    'v
                    objDESTINAZIONE.Scrivi(
                                        KeyCentro_DESTINAZIONE.Piva,
                                        KeyCentro_DESTINAZIONE.Sa_Cod,
                                        CType(DT.Rows(i).Item("Id_Cod"), Integer),
                                        CType(DT.Rows(i).Item("Val_Cod"), String),
                                        CType(DT.Rows(i).Item("Centri_Aziendali_Codici_Validita_Inizio"), Date),
                                        CType(DT.Rows(i).Item("Centri_Aziendali_Codici_Validita_Fine"), Date),
                                        _opzioni.objParametri_Server_GIAS_DESTINAZIONE,
                                        CType(DT.Rows(0).Item("Centri_Aziendali_Codici_Data_Creazione"), Date),
                                        CType(DT.Rows(0).Item("Centri_Aziendali_Codici_Data_Modifica"), Date),
                                        CType(DT.Rows(0).Item("Centri_Aziendali_Codici_Username_Creazione"), String),
                                        CType(DT.Rows(0).Item("Centri_Aziendali_Codici_Username_Modifica"), String)
                                        )

                Next

                DT.Dispose()

            End If

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine(" [" & NomeRoutine & "] : " & ex.Message.ToString)
        End Try

    End Sub

    '###########################################################################################################
    Private Sub G2G_Scrivi_UtentixStrutture(ByVal KeyCentro_ORIGINE As clsKeyCentro,
                                            ByVal KeyCentro_DESTINAZIONE As clsKeyCentro,
                                            ByRef ErrFLAG As Integer)

        Dim NomeRoutine As String = "G2G_Scrivi_UtentixStrutture"

        Try
            Dim objDESTINAZIONE As New AgronicaCoreAnagrafeDAL.UtentixStrutture_Write
            Dim objORIGINE As New AgronicaCoreAnagrafeDAL.UtentixStrutture_Read

            Dim dt As DataTable = objORIGINE.Leggi(KeyCentro_ORIGINE.Piva,
                                                   KeyCentro_ORIGINE.Sa_Cod,
                                                   enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                   "", "",
                                                   _opzioni.objParametri_Server_GIAS_ORIGINE)

            'v
            objDESTINAZIONE.Scrivi(
                                KeyCentro_DESTINAZIONE.Piva,
                                KeyCentro_DESTINAZIONE.Sa_Cod,
                                CType(dt.Rows(0).Item("UtentixStrutture_validita_inizio"), Date),
                                CType(dt.Rows(0).Item("UtentixStrutture_Validita_Fine"), Date),
                                _opzioni.objParametri_Server_GIAS_DESTINAZIONE,
                                CType(dt.Rows(0).Item("UtentixStrutture_Data_Creazione"), Date),
                                CType(dt.Rows(0).Item("UtentixStrutture_Data_Modifica"), Date),
                                CType(dt.Rows(0).Item("UtentixStrutture_Username_Creazione"), String),
                                CType(dt.Rows(0).Item("UtentixStrutture_Username_Modifica"), String)
            )

        Catch ex As Exception
            ErrFLAG = 1
            _Log_G2G.AppendLine(" [" & NomeRoutine & "] : " & ex.Message.ToString)
        End Try

    End Sub

    '###########################################################################################################
    Private Sub G2G_Copia_Rubrica_Centro(
                                ByVal KeyCentro_ORIGINE As clsKeyCentro,
                                ByVal KeyCentro_DESTINAZIONE As clsKeyCentro,
                                ByRef ErrFLAG As Integer
                            )

        'Dim KeyRubrica_ORIGINE As New clsKeyRubrica

        'G2G_Leggi_CentrixRubrica( _
        '                    KeyCentro_ORIGINE, _
        '                    KeyIndirizzo_ORIGINE, _
        '                    objParametri_Server_ORIGINE, _
        '                    ErrFLAG, Log)

        'If ErrFLAG <> 0 Then
        '    Exit Sub
        'End If

        'Dim KeyRubrica_DESTINAZIONE As New clsKeyRubrica

        'G2G_Copia_Rubrica( _
        '                    KeyIndirizzo_ORIGINE, _
        '                    KeyIndirizzo_DESTINAZIONE, _
        '                    objParametri_Server_ORIGINE, _
        '                    objParametri_Server_DESTINAZIONE, _
        '                    ErrFLAG, Log)

        'If ErrFLAG <> 0 Then
        '    Exit Sub
        'End If

        'G2G_Scrivi_CentrixRubrica( _
        '                    KeyCentro_DESTINAZIONE, _
        '                    KeyIndirizzo_DESTINAZIONE, _
        '                    objParametri_Server_DESTINAZIONE, _
        '                    ErrFLAG, Log)

        If ErrFLAG <> 0 Then
            Exit Sub
        End If

    End Sub




#End Region








End Class

