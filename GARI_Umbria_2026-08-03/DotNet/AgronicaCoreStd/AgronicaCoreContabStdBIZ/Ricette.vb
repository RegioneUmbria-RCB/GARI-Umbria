
Imports AgronicaCoreContabStdDAL
Imports AgronicaCoreDataProviderSTD.CostantiPersonalizzate
Imports AgronicaCoreDataProviderSTD.TipiEnumerativi
Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models

Public Class Ricette

    Public Shared Function ClasseAttivitaDaLavCod(Lav_Cod As Integer) As enum_Classi_Attivita

        Dim rVal As enum_Classi_Attivita

        Select Case Lav_Cod

            Case LAVCOD_RILIEVO_AVVERSITA_CAMPO,
                 LAVCOD_RILIEVO_INDICI_MATURITA,
                 LAVCOD_FASI_FENOLOGICHE

                rVal = enum_Classi_Attivita.Rilievo

            Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO,
                LAVCOD_DISERBO,
                LAVCOD_DISSECCAMENTO,
                LAVCOD_GEODISINFESTAZIONE,
                LAVCOD_CONCIA_SEME,
                LAVCOD_TRATTAMENTO_FITOREGOLATORE

                rVal = enum_Classi_Attivita.Trattamento

            Case LAVCOD_SEMINA,
                LAVCOD_TRAPIANTO,
                LAVCOD_SOD_SEDDING,
                LAVCOD_SOVESCIO

                rVal = enum_Classi_Attivita.SeminaTrapianto

            Case LAVCOD_RACCOLTA

                rVal = enum_Classi_Attivita.Raccolta

            Case LAVCOD_FERTIRRIGAZIONE,
                LAVCOD_CONCIMAZIONE_FOGLIARE,
                LAVCOD_DISTRIBUZIONE_CONCIME,
                LAVCOD_DISTRIBUZIONE_AMMENDANTI,
                LAVCOD_SARCHIATURA_CONCIMAZIONE,
                LAVCOD_TRATTAMENTO_ANTIBUTTERATURA

                rVal = enum_Classi_Attivita.Fertilizzazione

            Case Else

                rVal = enum_Classi_Attivita.LavorazioneBase

        End Select

        Return rVal

    End Function

    Public Shared Function OggettoDaLavCod(Lav_Cod As Integer) As AgronicaCoreModelloSTD.FormOperazioneConfig

        Dim rVal As New AgronicaCoreModelloSTD.FormOperazioneConfig

        Select Case Lav_Cod

            Case LAVCOD_RILIEVO_AVVERSITA_CAMPO

                rVal.RilievoVisibile = True
                rVal.RilievoAvversitaVisibile = True

                rVal.OggettoDettaglioPerTipo = New AgronicaCoreModelloSTD.Ricette_Dettagli_Rilievi

            Case LAVCOD_RILIEVO_INDICI_MATURITA

                rVal.RilievoVisibile = True
                rVal.RilievoIndiciMaturitaVisibile = True

                rVal.OggettoDettaglioPerTipo = New AgronicaCoreModelloSTD.Ricette_Dettagli_Rilievi

            Case LAVCOD_FASI_FENOLOGICHE

                rVal.RilievoVisibile = True
                rVal.RilievoFasiFenologicheVisibile = True

                rVal.OggettoDettaglioPerTipo = New AgronicaCoreModelloSTD.Ricette_Dettagli_Rilievi

            Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO,
                LAVCOD_DISERBO,
                LAVCOD_DISSECCAMENTO,
                LAVCOD_GEODISINFESTAZIONE,
                LAVCOD_CONCIA_SEME,
                LAVCOD_TRATTAMENTO_FITOREGOLATORE,
                LAVCOD_SEMINA,
                LAVCOD_TRAPIANTO,
                LAVCOD_SOD_SEDDING,
                LAVCOD_SOVESCIO,
                LAVCOD_DISTRIBUZIONE_INSETTI,
                LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE,
                LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA

                If Lav_Cod = LAVCOD_SEMINA OrElse
                   Lav_Cod = LAVCOD_TRAPIANTO OrElse
                   Lav_Cod = LAVCOD_SOD_SEDDING OrElse
                   Lav_Cod = LAVCOD_SOVESCIO OrElse
                   Lav_Cod = LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA Then
                    rVal.QtaProdotto = True
                Else
                    rVal.AcquaVisibile = True
                    If Not (Lav_Cod = LAVCOD_TRATTAMENTO_FITOREGOLATORE OrElse
                            Lav_Cod = LAVCOD_DISSECCAMENTO) Then
                        rVal.AvversitaVisibile = True
                    End If
                End If

                rVal.OggettoDettaglioPerTipo = New AgronicaCoreModelloSTD.Ricette_Dettagli_Trattamento

            Case LAVCOD_FERTIRRIGAZIONE,
                LAVCOD_CONCIMAZIONE_FOGLIARE,
                LAVCOD_DISTRIBUZIONE_CONCIME,
                LAVCOD_DISTRIBUZIONE_AMMENDANTI,
                LAVCOD_SARCHIATURA_CONCIMAZIONE,
                LAVCOD_TRATTAMENTO_ANTIBUTTERATURA


                rVal.EpocaVisibile = True
                rVal.EpocaAbilitata = True

                rVal.N_Visibile = True
                rVal.P_Visibile = True
                rVal.K_Visibile = True
                rVal.Cu_Visibile = True

                If Lav_Cod = LAVCOD_DISTRIBUZIONE_AMMENDANTI Then
                    rVal.N_Abilitata = True
                    rVal.P_Abilitata = True
                    rVal.k_Abilitata = True
                End If

                If Lav_Cod = LAVCOD_CONCIMAZIONE_FOGLIARE OrElse
                   Lav_Cod = LAVCOD_FERTIRRIGAZIONE OrElse
                   Lav_Cod = LAVCOD_TRATTAMENTO_ANTIBUTTERATURA Then

                    rVal.AcquaVisibile = True

                End If

                rVal.OggettoDettaglioPerTipo = New AgronicaCoreModelloSTD.Ricette_Dettagli_Fertilizzazione

            Case Else

                rVal.OggettoDettaglioPerTipo = New AgronicaCoreModelloSTD.Ricette_Dettagli_Lavorazioni

        End Select

        Return rVal

    End Function


    ''' <summary>
    ''' Genera una copia della ricetta e la restituisce come oggetto 
    ''' </summary>
    ''' <param name="ricettaLetta"></param>
    ''' <returns></returns>
    ''' <remarks>la nuova ricetta non viene memorizzata su database, ma è già pronta per farlo passandola al metodo "salva"</remarks>
    Public Sub Clona(ricettaLetta As AgronicaCoreModelloSTD.Ricette)

        'imposto il riferimento alla ricetta
        For Each Operazione In ricettaLetta.RicetteOperazioni

            Operazione.Ricetta_Operazione_Rif = New AgronicaCoreModelloSTD.Ricette_Operazioni With {
                .Ricetta_Operazione_Cod = Operazione.Ricetta_Operazione_Cod
            }

            'imposta lo stato in "eseguita"
            Operazione.W_Anagrafica_Stati_Cod = enum_WWorflow_WAnagraficaStati.Esecuzione_ed_avanzamento_delle_ricette_Eseguita

        Next

        'reset di tutti gli id
        RicettaResetID(ricettaLetta)

        ricettaLetta.ricetta_cod = 0
        ricettaLetta.TipoOperazioneDB = enum_TipoOperazioneDB.Copia

    End Sub

    Private Shared Sub RicettaResetID(ricettaLetta As AgronicaCoreModelloSTD.Ricette)

        For Each operazione_corrente In ricettaLetta.RicetteOperazioni

            operazione_corrente.Ricetta_Operazione_Cod = 0

            For Each dettaglio_corrente In operazione_corrente.Dettagli
                dettaglio_corrente.Ricetta_Dettaglio_Cod = 0

                For Each destinazione_corrente In dettaglio_corrente.ImpiantiInteressati
                    destinazione_corrente.Ricetta_Destinazione_Cod = 0
                Next

            Next

        Next

    End Sub

    Public Function LeggiConSingolaOperazione(dbContext As GiasDbContext,
                                              Ricetta_Operazione_Cod As Integer
                                              ) As AgronicaCoreModelloSTD.Ricette

        Dim letturaOperazioni As New Ricette_Operazioni_R
        Dim appListaOperazioni As List(Of APP_Ricette_Operazioni)

        appListaOperazioni = letturaOperazioni.Leggi(dbContext, 0, Ricetta_Operazione_Cod)

        If (appListaOperazioni.Count > 0) Then
            Return Leggi(dbContext, appListaOperazioni.FirstOrDefault.Ricetta_Cod, Ricetta_Operazione_Cod)
        End If

        Return Nothing

    End Function

    Public Sub CancellaSingolaRicettaConTransazione(dbContext As GiasDbContext,
                                                    RicettaCod As Integer,
                                                    RicettaOperazioneCod As Integer)

        Using transaction = dbContext.Database.BeginTransaction()

            Try

                '1. riporto se necessario lo stato dell'operazione padre in "da fare"
                Dim letturaOperazioneCollegata As New Ricette_Operazioni_R
                Dim opApp As APP_Ricette_Operazioni = letturaOperazioneCollegata.Leggi(dbContext,
                                                                                       RicettaCod,
                                                                                       RicettaOperazioneCod
                                                                                       ).FirstOrDefault
                If opApp IsNot Nothing Then

                    If opApp.Ricetta_Operazione_Cod_RIF <> 0 Then
                        Dim scritturaStato As New Ricette_Operazioni_W
                        scritturaStato.ImpostaStatoSuOperazione(dbContext, opApp.Ricetta_Operazione_Cod_RIF, enum_WWorflow_WAnagraficaStati.Esecuzione_ed_avanzamento_delle_ricette_Da_Eseguire)
                    End If

                End If

                '2. procedo ad eliminazione
                CancellaSingolaRicetta(dbContext, RicettaCod, RicettaOperazioneCod)

                transaction.Commit()

            Catch ex As Exception
                transaction.Rollback()
                Throw ex
            End Try

        End Using

    End Sub

    Public Sub CancellaSingolaRicetta(dbContext As GiasDbContext,
                                      ricettaCod As Integer,
                                      RicettaOperazioneCod As Integer,
                                      Optional CancellaTempiRisorse As Boolean = True)

        Try

            Dim ricetteW As New Ricette_W
            Dim ricetteXnoteW As New RicetteXnote_W
            Dim ricetteOperazioniW As New Ricette_Operazioni_W
            Dim ricetteDettagliW As New Ricette_Dettagli_W
            Dim ricetteDettaglioTecnicoW As New Ricette_Dettaglio_Tecnico_W
            Dim ricetteDestinazioniW As New Ricette_Destinazioni_W

            ricetteXnoteW.CancellaDaRicettaOperazioneCod(dbContext, RicettaOperazioneCod)
            ricetteOperazioniW.CancellaDaRicettaOperazioneCod(dbContext, RicettaOperazioneCod)
            ricetteDettagliW.CancellaDaRicettaOperazioneCod(dbContext, RicettaOperazioneCod)
            ricetteDettaglioTecnicoW.CancellaDaRicettaOperazioneCod(dbContext, RicettaOperazioneCod)
            ricetteDestinazioniW.CancellaDaRicettaOperazioneCod(dbContext, RicettaOperazioneCod)

            ' Cancello attività collegate
            If CancellaTempiRisorse Then
                Dim letturaRiferimenti As New TempiRisorse_Riferimenti_R
                Dim listaRiferimenti = letturaRiferimenti.Leggi(dbContext, ricettaCod)
                If listaRiferimenti.Count > 0 Then
                    Dim tempiRisorseBiz As New TempiRisorse
                    Dim idCdgGenerale = listaRiferimenti.First().Id_Cdg_Generale_Rif
                    tempiRisorseBiz.CancellaTempiRisorse(dbContext, idCdgGenerale)
                End If
            End If

            'Posso rimuovere la testata soltanto se si tratta di una testata generata localmente, quindi con id negativo
            'If ricettaCod < 0 Then
            ricetteW.CancellaDaRicettaCod(dbContext, ricettaCod)
            'End If

        Catch ex As Exception
            Throw ex
        End Try

    End Sub

    ''' <summary>
    ''' Verifica se esistono dati da ricaricare su server
    ''' </summary>
    ''' <param name="dbContext"></param>
    ''' <returns>true se esistono dati</returns>
    Public Function VerificaEsistenzaDatiLocali(dbContext As GiasDbContext) As Boolean

        Dim ver As New Ricette_Operazioni_R
        Return (ver.LeggiPerVerificaPresenzaDatiLocali(dbContext).Count > 0)

    End Function

    ''' <summary>
    ''' Lettura di un'operazione di una ricetta
    ''' </summary>
    ''' <param name="dbContext"></param>
    ''' <param name="Ricetta_Cod"></param>
    ''' <param name="Ricetta_Operazione_Cod"></param>
    ''' <returns></returns>
    Public Function Leggi(dbContext As GiasDbContext,
                          Ricetta_Cod As Integer,
                          Ricetta_Operazione_Cod As Integer
                          ) As AgronicaCoreModelloSTD.Ricette

        Dim ricettaLetta As New AgronicaCoreModelloSTD.Ricette

        Dim letturaRicetta As New Ricette_R
        Dim letturaOperazioni As New Ricette_Operazioni_R
        Dim letturaDettagli As New Ricette_Dettagli_R
        Dim letturaDettaglioTecnico As New Ricette_Dettaglio_Tecnico_R
        Dim letturaDestinazioni As New Ricette_Destinazioni_R

        Dim appRicettaLetta As APP_Ricette
        Dim appListaOperazioni As List(Of APP_Ricette_Operazioni)
        Dim appListaDettagli As List(Of APP_Ricette_Dettagli)
        Dim appListaDettaglioTecnico As List(Of APP_Ricette_Dettaglio_Tecnico)
        Dim appListaDestinazioni As List(Of APP_Ricette_Destinazioni)

        appRicettaLetta = letturaRicetta.Leggi(dbContext, Ricetta_Cod).FirstOrDefault
        appListaOperazioni = letturaOperazioni.Leggi(dbContext, Ricetta_Cod, Ricetta_Operazione_Cod)

        ricettaLetta.ricetta_cod = appRicettaLetta.ricetta_cod
        ricettaLetta.ricetta_des = appRicettaLetta.ricetta_des
        ricettaLetta.ricetta_numero = appRicettaLetta.Ricetta_Numero
        ricettaLetta.note = appRicettaLetta.note
        ricettaLetta.Tipo_Ricetta = CType(appRicettaLetta.Tipo_Ricetta, enum_TipoRicetta_DB)

        Dim pivaPerLetturaProdotti As String = ""
        If appRicettaLetta.piva <> "" Then
            pivaPerLetturaProdotti = appRicettaLetta.piva
        End If

        ricettaLetta.RicetteOperazioni = New List(Of AgronicaCoreModelloSTD.Ricette_Operazioni)

        For Each app_operazioneCorrente In appListaOperazioni

            Dim operazione As New AgronicaCoreModelloSTD.Ricette_Operazioni

            Ricette_Operazioni.LeggiOperazione(operazione, app_operazioneCorrente)

            appListaDettagli = letturaDettagli.Leggi(dbContext, Ricetta_Cod,
                                                     app_operazioneCorrente.Ricetta_Operazione_Cod)

            operazione.Dettagli = New List(Of AgronicaCoreModelloSTD.Ricette_Dettagli)


            Dim testTipo As AgronicaCoreModelloSTD.Ricette_Dettagli = OggettoDaLavCod(app_operazioneCorrente.Lav_Cod).OggettoDettaglioPerTipo

            If TypeOf testTipo Is AgronicaCoreModelloSTD.Ricette_Dettagli_Fertilizzazione OrElse
               TypeOf testTipo Is AgronicaCoreModelloSTD.Ricette_Dettagli_Trattamento Then

                ' VAnni: 21/6/2018: dettaglio tecnico legato ad operazione per rilettura Acqua.
                Dim appDettaglioTecnicoAcqua As APP_Ricette_Dettaglio_Tecnico =
                    letturaDettaglioTecnico.Leggi(dbContext, Ricetta_Cod,
                                                  app_operazioneCorrente.Ricetta_Operazione_Cod,
                                                  0).FirstOrDefault

                If appDettaglioTecnicoAcqua IsNot Nothing Then
                    If appDettaglioTecnicoAcqua.Qta_Ril > 0 Then
                        operazione.Acqua = appDettaglioTecnicoAcqua.Qta_Ril
                    Else
                        operazione.Acqua = -appDettaglioTecnicoAcqua.Qta_Ril
                        operazione.Acqua_Totale_o_Ha = 1
                    End If
                End If

            End If

            Dim appListaDettagliDistribuzioni As List(Of APP_Ricette_Dettagli) = (
                From d In appListaDettagli
                Where Not {CAU_IMPUTAZIONE_PARCOMACCHINE,
                        CAU_IMPUTAZIONE_MANODOPERA,
                        CAU_IMPUTAZIONE_TECNICO_RESPONSABILE,
                        CAU_IMPUTAZIONE_TERZISTI,
                        CAU_SCARICO,
                        CAU_CARICO
                       }.Contains(CStr(d.Cau_Mov))
                ).ToList()

            appListaDestinazioni = letturaDestinazioni.Leggi(dbContext, Ricetta_Cod,
                                                             app_operazioneCorrente.Ricetta_Operazione_Cod,
                                                             0)

            If pivaPerLetturaProdotti = "" AndAlso
               appListaDestinazioni IsNot Nothing AndAlso appListaDestinazioni.Count > 0 Then
                pivaPerLetturaProdotti = appListaDestinazioni.First.Piva
            End If

            ' calcola superficie totale trattata
            Dim superficieTotale As Decimal = 0
            Dim listaImpianti = New List(Of String)
            For Each impiantoDestinazione In appListaDestinazioni
                Dim chiave = impiantoDestinazione.Piva & "_" & impiantoDestinazione.Sa_Cod & "_" & impiantoDestinazione.Appezza & "_" & impiantoDestinazione.Id_Reg

                If impiantoDestinazione.Tipo_Destinazione = 0 AndAlso
                   Not listaImpianti.Contains(chiave) Then
                    superficieTotale += impiantoDestinazione.Qta2
                    listaImpianti.Add(chiave)
                End If


                'la lettura dei dettagli nei rilievi viene fatta diversamente, partendo dalle destinazioni
                If TypeOf testTipo Is AgronicaCoreModelloSTD.Ricette_Dettagli_Rilievi Then

                    appListaDettaglioTecnico =
                        letturaDettaglioTecnico.Leggi(dbContext, Ricetta_Cod, app_operazioneCorrente.Ricetta_Operazione_Cod, impiantoDestinazione.Ricetta_Dettaglio_Cod)

                    Dim appDettaglioCorrente As APP_Ricette_Dettagli = (
                        From d In appListaDettagliDistribuzioni
                        Where d.Ricetta_Dettaglio_Cod = impiantoDestinazione.Ricetta_Dettaglio_Cod
                    ).FirstOrDefault

                    Dim appTecnico As APP_Ricette_Dettaglio_Tecnico = appListaDettaglioTecnico.FirstOrDefault


                    Dim dettaglioCorrente As New AgronicaCoreModelloSTD.Ricette_Dettagli_Rilievi
                    Ricette_Dettagli_Rilievi.LeggiDettagli(dbContext, dettaglioCorrente, appDettaglioCorrente,
                                                           appTecnico, impiantoDestinazione)

                    Dim distribuzioneDaPopolare As New AgronicaCoreModelloSTD.Ricette_DistribuzioniSuImpianti
                    Ricette_Destinazioni.LeggiDestinazioni(distribuzioneDaPopolare, impiantoDestinazione)

                    If dettaglioCorrente.ImpiantiInteressati Is Nothing Then
                        dettaglioCorrente.ImpiantiInteressati = New List(Of AgronicaCoreModelloSTD.Ricette_DistribuzioniSuImpianti)
                    End If

                    dettaglioCorrente.ImpiantiInteressati.Add(distribuzioneDaPopolare)

                    LeggiCentroSpecie(dbContext, operazione, distribuzioneDaPopolare)

                    dettaglioCorrente.Operazione = operazione
                    operazione.Dettagli.Add(dettaglioCorrente)

                End If

            Next

            If TypeOf testTipo IsNot AgronicaCoreModelloSTD.Ricette_Dettagli_Rilievi Then

                For Each app_dettaglioCorrente In appListaDettagliDistribuzioni

                    appListaDettaglioTecnico =
                        letturaDettaglioTecnico.Leggi(dbContext, Ricetta_Cod, app_operazioneCorrente.Ricetta_Operazione_Cod, app_dettaglioCorrente.Ricetta_Dettaglio_Cod)

                    Dim dettaglioCorrente As AgronicaCoreModelloSTD.Ricette_Dettagli = Nothing

                    Dim appDettaglioMagazzino As APP_Ricette_Dettagli = (
                            From d In appListaDettagli
                            Where d.Cau_Mov = CInt(CAU_SCARICO) AndAlso
                                  d.Elem_Cod = app_dettaglioCorrente.Elem_Cod AndAlso
                                  d.Pro_Cod = app_dettaglioCorrente.Pro_Cod AndAlso
                                  d.Mat_Cod = app_dettaglioCorrente.Mat_Cod
                            ).FirstOrDefault

                    Dim appListaDestinazioniMagazzino As New List(Of APP_Ricette_Destinazioni)
                    If appDettaglioMagazzino IsNot Nothing Then
                        appListaDestinazioniMagazzino = (
                                From dd In appListaDestinazioni
                                Where dd.Ricetta_Dettaglio_Cod = appDettaglioMagazzino.Ricetta_Dettaglio_Cod AndAlso
                                      dd.Tipo_Destinazione = 20
                             ).ToList()

                    End If

                    If TypeOf testTipo Is AgronicaCoreModelloSTD.Ricette_Dettagli_Trattamento Then

                        dettaglioCorrente = New AgronicaCoreModelloSTD.Ricette_Dettagli_Trattamento
                        Ricette_Dettagli_Trattamenti.LeggiDettagli(
                            dbContext,
                            pivaPerLetturaProdotti,
                            CType(dettaglioCorrente, AgronicaCoreModelloSTD.Ricette_Dettagli_Trattamento),
                            app_dettaglioCorrente,
                            appListaDettaglioTecnico.FirstOrDefault,
                            appDettaglioMagazzino,
                            appListaDestinazioniMagazzino
                         )

                        ' forzo qta per semine/trapianti (mail Grilli del 28/03/2018)
                        If app_dettaglioCorrente.Cau_Mov = CInt(CAU_LAVORAZIONE) AndAlso superficieTotale > 0 AndAlso app_dettaglioCorrente.Qta_Extra_Totale = 0 Then
                            CType(dettaglioCorrente, AgronicaCoreModelloSTD.Ricette_Dettagli_Trattamento).Dose_Ha_Reale = app_dettaglioCorrente.Qta / superficieTotale
                            CType(dettaglioCorrente, AgronicaCoreModelloSTD.Ricette_Dettagli_Trattamento).Dose_Totale_Reale = app_dettaglioCorrente.Qta
                            'CType(dettaglioCorrente, AgronicaCoreModelloSTD.Ricette_Dettagli_Trattamento).Dose_Hl_Reale = app_dettaglioCorrente.Qta_Extra
                        End If

                    End If


                    If TypeOf testTipo Is AgronicaCoreModelloSTD.Ricette_Dettagli_Fertilizzazione Then

                        dettaglioCorrente = New AgronicaCoreModelloSTD.Ricette_Dettagli_Fertilizzazione
                        Ricette_Dettagli_Fertilizzazioni.LeggiDettagli(
                            dbContext,
                            pivaPerLetturaProdotti,
                            CType(dettaglioCorrente, AgronicaCoreModelloSTD.Ricette_Dettagli_Fertilizzazione),
                            app_dettaglioCorrente,
                            appListaDettaglioTecnico.FirstOrDefault,
                            appDettaglioMagazzino,
                            appListaDestinazioniMagazzino
                        )

                    End If

                    If TypeOf testTipo Is AgronicaCoreModelloSTD.Ricette_Dettagli_Lavorazioni Then

                        dettaglioCorrente = New AgronicaCoreModelloSTD.Ricette_Dettagli_Lavorazioni
                        Ricette_Dettagli_Lavorazioni.LeggiDettagli(CType(dettaglioCorrente, AgronicaCoreModelloSTD.Ricette_Dettagli_Lavorazioni), app_dettaglioCorrente)

                    End If


                    'non deve succedere...
                    If dettaglioCorrente Is Nothing Then
                        Throw New Exception("Lav_cod non riconosciuto...")
                    End If

                    dettaglioCorrente.ImpiantiInteressati = New List(Of AgronicaCoreModelloSTD.Ricette_DistribuzioniSuImpianti)

                    'filtro per i soli impianti interessati (tipo = 0)
                    For Each app_destinazione_corrente In appListaDestinazioni.Where(Function(imp) imp.Ricetta_Dettaglio_Cod = dettaglioCorrente.Ricetta_Dettaglio_Cod AndAlso imp.Tipo_Destinazione = 0)

                        Dim distribuzioneDaScrivere As New AgronicaCoreModelloSTD.Ricette_DistribuzioniSuImpianti
                        Ricette_Destinazioni.LeggiDestinazioni(distribuzioneDaScrivere,
                                                               app_destinazione_corrente)

                        dettaglioCorrente.ImpiantiInteressati.Add(distribuzioneDaScrivere)

                        LeggiCentroSpecie(dbContext, operazione, distribuzioneDaScrivere)

                    Next 'destinazione

                    dettaglioCorrente.Operazione = operazione

                    operazione.Dettagli.Add(dettaglioCorrente)

                Next 'dettaglio

            End If
            'se non si tratta di un rilievo
            ricettaLetta.RicetteOperazioni.Add(operazione)

        Next 'Operazione

        Return ricettaLetta

    End Function

    Private Sub LeggiCentroSpecie(dbContext As GiasDbContext,
                                  Operazione As AgronicaCoreModelloSTD.Ricette_Operazioni,
                                  DistribuzioneDaScrivere As AgronicaCoreModelloSTD.Ricette_DistribuzioniSuImpianti)

        If Operazione.CentroAziendale Is Nothing Then
            Operazione.CentroAziendale = New AgronicaCoreModelloSTD.Centri_Aziendali With {
                                            .Piva = DistribuzioneDaScrivere.Impianto.piva,
                                            .Sa_Cod = DistribuzioneDaScrivere.Impianto.sa_cod
                                        }

            If Operazione.Specie Is Nothing Then

                Dim letturaSpecie As New AgronicaCoreAnagrafeStdDAL.SpecieVegetali_R
                Dim specieImpianto As AgronicaCoreModelloSTD.SpecieVegetaliDestinazioni =
                    letturaSpecie.EstraiSpecieVegetaliImpianto(
                        dbContext,
                        DistribuzioneDaScrivere.Impianto.piva,
                        DistribuzioneDaScrivere.Impianto.sa_cod,
                        DistribuzioneDaScrivere.Impianto.appezza,
                        DistribuzioneDaScrivere.Impianto.id_reg
                    )

                Operazione.Specie = specieImpianto
            End If

        End If
    End Sub

    ''' <summary>
    ''' Scrittura di una nuova Operazione in ricetta
    ''' </summary>
    ''' <param name="dbContext"></param>
    ''' <param name="Ricetta"></param>
    Public Sub Scrivi(dbContext As GiasDbContext,
                      Ricetta As AgronicaCoreModelloSTD.Ricette,
                      username As String,
                      utente As String)

        Dim appRicetta As New APP_Ricette

        Dim listaOperazioni As New List(Of APP_Ricette_Operazioni)
        Dim listaDettagli As New List(Of APP_Ricette_Dettagli)
        Dim listaDettaglioTecnico As New List(Of APP_Ricette_Dettaglio_Tecnico)
        Dim listaDestinazioni As New List(Of APP_Ricette_Destinazioni)

        Dim agroSequenze As New AgronicaCoreDataProviderSTD.Agro_Sequenze

        Dim ricettaCodPrecedente As Integer = Ricetta.ricetta_cod

        Dim ricettaOperazioneCodPrecedente As Integer =
            Ricetta.RicetteOperazioni.FirstOrDefault.Ricetta_Operazione_Cod

        If Ricetta.TipoOperazioneDB = enum_TipoOperazioneDB.Modifica Then
            RicettaResetID(Ricetta)
            Ricetta.ricetta_cod = ricettaCodPrecedente
            Ricetta.RicetteOperazioni.FirstOrDefault.Ricetta_Operazione_Cod = ricettaOperazioneCodPrecedente
        End If

        If Ricetta.ricetta_cod = 0 Then
            Ricetta.ricetta_cod =
                -(agroSequenze.NuovoId_Tabella_EF(dbContext, "ricette", AgroSequenzeBase0, AgroSequenzeEndUpperBound))
            If Ricetta.TipoOperazioneDB <> enum_TipoOperazioneDB.Copia Then
                Dim progressivo = agroSequenze.NuovoId_Tabella_EF(dbContext, "ricette_" & DateTime.Now.ToString("yyyyMMdd"), AgroSequenzeBase0, AgroSequenzeEndUpperBound)
                Ricetta.ricetta_des = utente & "_" & progressivo
                Ricetta.ricetta_numero = utente & "_" & progressivo
            End If
            dbContext.SaveChanges()
        End If

        appRicetta.ricetta_cod = Ricetta.ricetta_cod
        appRicetta.Tipo_Ricetta = Ricetta.Tipo_Ricetta
        appRicetta.ricetta_des = Ricetta.ricetta_des
        appRicetta.Ricetta_Numero = Ricetta.ricetta_numero
        appRicetta.note = Ricetta.note
        appRicetta.Veg_Cod = -1

        Dim listaOperazioniStatoAggiorna As New List(Of Integer)

        For Each operazioneCorrente In Ricetta.RicetteOperazioni

            Dim operazioneConfig = OggettoDaLavCod(operazioneCorrente.Operazione.lav_cod)

            If operazioneCorrente.W_Anagrafica_Stati_Cod = enum_WWorflow_WAnagraficaStati.Esecuzione_ed_avanzamento_delle_ricette_Eseguita AndAlso
               Ricetta.TipoOperazioneDB = enum_TipoOperazioneDB.Copia AndAlso
               operazioneCorrente.Ricetta_Operazione_Rif IsNot Nothing Then

                listaOperazioniStatoAggiorna.Add(operazioneCorrente.Ricetta_Operazione_Rif.Ricetta_Operazione_Cod)

            End If

            Dim appRicettaOperazioni As New APP_Ricette_Operazioni

            If operazioneCorrente.Ricetta_Operazione_Cod = 0 Then
                operazioneCorrente.Ricetta_Operazione_Cod =
                    -(agroSequenze.NuovoId_Tabella_EF(dbContext, "ricette_operazioni", AgroSequenzeBase0, AgroSequenzeEndUpperBound))
                dbContext.SaveChanges()
            End If

            Ricette_Operazioni.ScriviOperazione(Ricetta.ricetta_cod, operazioneCorrente, appRicettaOperazioni, operazioneConfig)


            'se si tratta di un copia, la ricetta originale va impostata in stato "eseguita"
            If operazioneCorrente.Ricetta_Operazione_Rif IsNot Nothing AndAlso
               Ricetta.TipoOperazioneDB = enum_TipoOperazioneDB.Copia Then

                Dim xImpostaStato As New Ricette_Operazioni_W
                xImpostaStato.ImpostaStatoSuOperazione(
                    dbContext,
                    operazioneCorrente.Ricetta_Operazione_Rif.Ricetta_Operazione_Cod,
                    enum_WWorflow_WAnagraficaStati.Esecuzione_ed_avanzamento_delle_ricette_Eseguita
                )

            End If

            For Each dettaglioCorrente In operazioneCorrente.Dettagli

                If dettaglioCorrente.Ricetta_Dettaglio_Cod = 0 Then
                    dettaglioCorrente.Ricetta_Dettaglio_Cod =
                    -(agroSequenze.NuovoId_Tabella_EF(dbContext, "ricette_dettagli", AgroSequenzeBase0, AgroSequenzeEndUpperBound))
                    dbContext.SaveChanges()

                End If

                Dim appDettaglio As New APP_Ricette_Dettagli
                Dim appDettaglioTecnico As New APP_Ricette_Dettaglio_Tecnico

                'imposta il magazzino, se necessario
                If TypeOf dettaglioCorrente Is AgronicaCoreModelloSTD.Ricette_Dettagli_Prodotti Then
                    Dim dettaglioProdPerMagazzino As AgronicaCoreModelloSTD.Ricette_Dettagli_Prodotti = CType(dettaglioCorrente, AgronicaCoreModelloSTD.Ricette_Dettagli_Prodotti)

                    If dettaglioProdPerMagazzino.MagazziniMovimentazioni IsNot Nothing AndAlso
                       dettaglioProdPerMagazzino.MagazziniMovimentazioni.Magazzino IsNot Nothing Then

                        Dim ricettaDettaglioMagazzinoCod As Integer =
                            -(agroSequenze.NuovoId_Tabella_EF(dbContext, "ricette_dettagli", AgroSequenzeBase0, AgroSequenzeEndUpperBound))
                        dbContext.SaveChanges()


                        'dettaglio per magazzino
                        Dim appDettaglioMagazzino As New APP_Ricette_Dettagli
                        Dim dettaglioProdottoMagazzino As New AgronicaCoreModelloSTD.Ricette_Dettagli_Prodotti With {
                            .Ricetta_Dettaglio_Cod = ricettaDettaglioMagazzinoCod,
                            .Dose_Ha_Reale = dettaglioProdPerMagazzino.Dose_Totale_Reale,
                            .Prodotto = New AgronicaCoreModelloSTD.Prodotto With {
                                .Elem_Cod = dettaglioProdPerMagazzino.Prodotto.Elem_Cod,
                                .Prodotto_Cod = dettaglioProdPerMagazzino.Prodotto.Prodotto_Cod,
                                .Udm = New AgronicaCoreModelloSTD.UnitaMisura With {.udm_cod = dettaglioProdPerMagazzino.MagazziniMovimentazioni.udm.udm_cod}
                            },
                            .MagazziniMovimentazioni = dettaglioProdPerMagazzino.MagazziniMovimentazioni,
                            .Udm = New AgronicaCoreModelloSTD.UnitaMisura With {.udm_cod = dettaglioProdPerMagazzino.MagazziniMovimentazioni.udm.udm_cod},
                            .Udm_Indicata = New AgronicaCoreModelloSTD.UnitaMisura With {.udm_cod = 0}
                            }

                        Ricette_Dettagli_Prodotti.ScriviDettagli(Ricetta.ricetta_cod, operazioneCorrente.Ricetta_Operazione_Cod, dettaglioProdottoMagazzino, appDettaglioMagazzino)

                        appDettaglioMagazzino.Cau_Mov = CInt(CAU_SCARICO)

                        listaDettagli.Add(appDettaglioMagazzino)

                        'destinazione magazzino
                        Dim ricettaDestinazioneMagazzinoCod As Integer =
                            -(agroSequenze.NuovoId_Tabella_EF(dbContext, "ricette_destinazioni", AgroSequenzeBase0, AgroSequenzeEndUpperBound))
                        dbContext.SaveChanges()

                        Dim appDestinazioneMagazzino As New APP_Ricette_Destinazioni With {
                            .Piva = dettaglioProdPerMagazzino.MagazziniMovimentazioni.Magazzino.Piva,
                            .Sa_Cod = dettaglioProdPerMagazzino.MagazziniMovimentazioni.Magazzino.Sa_Cod,
                            .Tipo_Destinazione = 20,
                            .Id_Reg = dettaglioProdPerMagazzino.MagazziniMovimentazioni.Magazzino.Fabbricato_Cod,
                            .Qta = dettaglioProdPerMagazzino.Dose_Totale_Reale,
                            .Ricetta_Cod = Ricetta.ricetta_cod,
                            .Ricetta_Operazione_Cod = operazioneCorrente.Ricetta_Operazione_Cod,
                            .Ricetta_Destinazione_Cod = ricettaDestinazioneMagazzinoCod,
                            .Ricetta_Dettaglio_Cod = appDettaglioMagazzino.Ricetta_Dettaglio_Cod
                        }

                        listaDestinazioni.Add(appDestinazioneMagazzino)

                    End If

                End If

                Dim superficieTotale As Decimal = 0
                For Each impiantoCorrente In dettaglioCorrente.ImpiantiInteressati
                    superficieTotale += impiantoCorrente.SuperficieTrattata
                Next

                'imposta destinazioni
                For Each impiantoCorrente In dettaglioCorrente.ImpiantiInteressati

                    Dim appDestinazione As New APP_Ricette_Destinazioni

                    If impiantoCorrente.Ricetta_Destinazione_Cod = 0 Then
                        impiantoCorrente.Ricetta_Destinazione_Cod =
                        -(agroSequenze.NuovoId_Tabella_EF(dbContext, "ricette_destinazioni", AgroSequenzeBase0, AgroSequenzeEndUpperBound))
                        dbContext.SaveChanges()

                    End If

                    If appRicetta.piva = "" Then
                        appRicetta.piva = impiantoCorrente.Impianto.piva
                        appRicetta.sa_cod = impiantoCorrente.Impianto.sa_cod
                    End If

                    'se si tratta di scarico di prodotti calcolo la quantità distribuita
                    If TypeOf dettaglioCorrente Is AgronicaCoreModelloSTD.Ricette_Dettagli_Prodotti Then
                        impiantoCorrente.QuantitaDistribuita = impiantoCorrente.SuperficieTrattata * CType(dettaglioCorrente, AgronicaCoreModelloSTD.Ricette_Dettagli_Prodotti).Dose_Ha_Reale
                    End If


                    Ricette_Destinazioni.ScriviDestinazioni(
                        Ricetta.ricetta_cod,
                        operazioneCorrente.Ricetta_Operazione_Cod,
                        dettaglioCorrente.Ricetta_Dettaglio_Cod,
                        impiantoCorrente,
                        appDestinazione,
                        superficieTotale
                    )


                    listaDestinazioni.Add(appDestinazione)

                Next 'Destinazione


                'scrittura della parte particolare ...
                If TypeOf dettaglioCorrente Is AgronicaCoreModelloSTD.Ricette_Dettagli_Lavorazioni Then
                    Ricette_Dettagli_Lavorazioni.ScriviDettagli(Ricetta.ricetta_cod, operazioneCorrente.Ricetta_Operazione_Cod, CType(dettaglioCorrente, AgronicaCoreModelloSTD.Ricette_Dettagli_Lavorazioni), appDettaglio)
                    If operazioneCorrente.Operazione.lav_cod = LAVCOD_RACCOLTA Then
                        appDettaglio.Cau_Mov = CInt(CAU_RILIEVO_RACCOLTA)
                        ' forzo campi per la raccolta (Mail Grillo del 07/05/2019)
                        appDettaglio.Lotto = ""
                        appDettaglio.Mezzo_Det = -1
                        appDettaglio.Elem_Cod = 210
                    Else
                        appDettaglio.Cau_Mov = CInt(CAU_LAVORAZIONE)
                    End If
                End If

                'Rilievi
                If TypeOf dettaglioCorrente Is AgronicaCoreModelloSTD.Ricette_Dettagli_Rilievi Then

                    Ricette_Dettagli_Rilievi.ScriviDettagli(Ricetta.ricetta_cod, operazioneCorrente.Ricetta_Operazione_Cod, CType(dettaglioCorrente, AgronicaCoreModelloSTD.Ricette_Dettagli_Rilievi), appDettaglio, appDettaglioTecnico)

                    If operazioneCorrente.Operazione.lav_cod = LAVCOD_RILIEVO_INDICI_MATURITA Then
                        appDettaglio.Cau_Mov = CInt(CAU_RILIEVO_RACCOLTA)
                    Else
                        appDettaglio.Cau_Mov = CInt(CAU_RILIEVO_CAMPO)
                    End If

                    appDettaglioTecnico.Ricetta_Tecnico_Cod =
                    -(agroSequenze.NuovoId_Tabella_EF(dbContext, "ricette_dettaglio_tecnico", AgroSequenzeBase0, AgroSequenzeEndUpperBound))
                    dbContext.SaveChanges()

                End If
                'fine Rilievi

                Dim prodottoValorizzato As Boolean = False

                If TypeOf dettaglioCorrente Is AgronicaCoreModelloSTD.Ricette_Dettagli_Trattamento Then

                    ' Se operazione di semina cambio cau_mov e non salvo dettaglio tecnico
                    If operazioneConfig.QtaProdotto Then

                        appDettaglio.Cau_Mov = CInt(CAU_LAVORAZIONE)
                        appDettaglioTecnico = Nothing

                    Else

                        If appDettaglioTecnico.Ricetta_Tecnico_Cod = 0 Then
                            appDettaglioTecnico.Ricetta_Tecnico_Cod =
                                -(agroSequenze.NuovoId_Tabella_EF(dbContext, "ricette_dettaglio_tecnico", AgroSequenzeBase0, AgroSequenzeEndUpperBound))
                            dbContext.SaveChanges()

                        End If

                        appDettaglio.Cau_Mov = CInt(CAU_TRATTAMENTO)

                    End If

                    Ricette_Dettagli_Trattamenti.ScriviDettagli(Ricetta.ricetta_cod, operazioneCorrente.Ricetta_Operazione_Cod, CType(dettaglioCorrente, AgronicaCoreModelloSTD.Ricette_Dettagli_Trattamento), appDettaglio, appDettaglioTecnico)

                    ' forzo qta per semine/trapianti (mail Grilli del 28/03/2018)
                    If CStr(appDettaglio.Cau_Mov) = CAU_LAVORAZIONE Then
                        appDettaglio.Qta = appDettaglio.Qta_Extra_Totale
                        appDettaglio.Qta_Extra_Totale = 0
                    End If

                    prodottoValorizzato = True

                End If

                If TypeOf dettaglioCorrente Is AgronicaCoreModelloSTD.Ricette_Dettagli_Fertilizzazione Then
                    If appDettaglioTecnico.Ricetta_Tecnico_Cod = 0 Then
                        appDettaglioTecnico.Ricetta_Tecnico_Cod =
                            -(agroSequenze.NuovoId_Tabella_EF(dbContext, "ricette_dettaglio_tecnico", AgroSequenzeBase0, AgroSequenzeEndUpperBound))
                        dbContext.SaveChanges()

                    End If
                    Ricette_Dettagli_Fertilizzazioni.ScriviDettagli(Ricetta.ricetta_cod, operazioneCorrente.Ricetta_Operazione_Cod, CType(dettaglioCorrente, AgronicaCoreModelloSTD.Ricette_Dettagli_Fertilizzazione), appDettaglio, appDettaglioTecnico)

                    appDettaglio.Cau_Mov = CInt(CAU_LAVORAZIONE)

                    prodottoValorizzato = True

                End If

                If Not prodottoValorizzato AndAlso TypeOf dettaglioCorrente Is AgronicaCoreModelloSTD.Ricette_Dettagli_Prodotti Then
                    Ricette_Dettagli_Prodotti.ScriviDettagli(Ricetta.ricetta_cod, operazioneCorrente.Ricetta_Operazione_Cod, CType(dettaglioCorrente, AgronicaCoreModelloSTD.Ricette_Dettagli_Prodotti), appDettaglio)
                End If

                'fertilizzazioni o trattamenti --> Acqua
                If TypeOf dettaglioCorrente Is AgronicaCoreModelloSTD.Ricette_Dettagli_Fertilizzazione OrElse
                   TypeOf dettaglioCorrente Is AgronicaCoreModelloSTD.Ricette_Dettagli_Trattamento Then

                    'scrittura del dettaglio tecnico x Acqua (solo se Acqua è stata indicata e non ancora aggiunto il dettaglio tecnico
                    If operazioneCorrente.Acqua <> 0 AndAlso (listaDettaglioTecnico.Count = 0 OrElse (From tOp In listaDettaglioTecnico Where tOp.Ricetta_Dettaglio_Cod = 0).Count = 0) Then

                        Dim appDettaglioTecnicoH2O As New APP_Ricette_Dettaglio_Tecnico

                        'chiavi primarie
                        appDettaglioTecnicoH2O.Ricetta_Cod = Ricetta.ricetta_cod
                        appDettaglioTecnicoH2O.Ricetta_Operazione_Cod = operazioneCorrente.Ricetta_Operazione_Cod
                        appDettaglioTecnicoH2O.Ricetta_Dettaglio_Cod = 0
                        appDettaglioTecnicoH2O.Ricetta_Tecnico_Cod =
                            -(agroSequenze.NuovoId_Tabella_EF(dbContext, "ricette_dettaglio_tecnico", AgroSequenzeBase0, AgroSequenzeEndUpperBound))
                        dbContext.SaveChanges()


                        'dati, se H2O Totale (vale 0), allora positiva, Se Acqua Per ha, allora negativa 
                        If operazioneCorrente.Acqua_Totale_o_Ha = 0 Then
                            appDettaglioTecnicoH2O.Qta_Ril = operazioneCorrente.Acqua
                        Else
                            appDettaglioTecnicoH2O.Qta_Ril = -operazioneCorrente.Acqua
                        End If


                        'valori predefiniti
                        appDettaglioTecnicoH2O.Inn1_data = AGRODATAINIZIO
                        appDettaglioTecnicoH2O.Inn2_data = AGRODATAINIZIO

                        listaDettaglioTecnico.Add(appDettaglioTecnicoH2O)

                    End If

                End If 'fine Acqua


                listaDettagli.Add(appDettaglio)

                If appDettaglioTecnico IsNot Nothing AndAlso appDettaglioTecnico.Ricetta_Tecnico_Cod <> 0 Then
                    listaDettaglioTecnico.Add(appDettaglioTecnico)
                End If


            Next 'Dettaglio

            listaOperazioni.Add(appRicettaOperazioni)

        Next 'Operazione



        Using transaction = dbContext.Database.BeginTransaction()

            Try

                If listaOperazioniStatoAggiorna.Count > 0 Then

                    Dim scritturaStato As New Ricette_Operazioni_W
                    For Each operazioneStatoAggiorna As Integer In listaOperazioniStatoAggiorna
                        scritturaStato.ImpostaStatoSuOperazione(dbContext, operazioneStatoAggiorna, enum_WWorflow_WAnagraficaStati.Esecuzione_ed_avanzamento_delle_ricette_Eseguita)
                    Next

                End If

                If Ricetta.TipoOperazioneDB = enum_TipoOperazioneDB.Modifica Then
                    ' Imposta bozza su attivita collegata
                    Dim tempiRisorseBiz As New TempiRisorse
                    tempiRisorseBiz.ImpostaBozzaRicettaAttivita(dbContext, listaOperazioni.FirstOrDefault)
                    CancellaSingolaRicetta(dbContext, ricettaCodPrecedente, ricettaOperazioneCodPrecedente, False)
                End If

                'la testata deve essere memorizzata solo in scrittura, altrimenti in modifica se il codice ricetta è negativo (cioè se è un'operazione locale).
                If Ricetta.TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura OrElse
                    (Ricetta.TipoOperazioneDB = enum_TipoOperazioneDB.Modifica AndAlso Ricetta.ricetta_cod < 0) OrElse
                    (Ricetta.TipoOperazioneDB = enum_TipoOperazioneDB.Copia AndAlso Ricetta.ricetta_cod < 0) Then

                    ScritturaDatiComuni(appRicetta, username)
                    dbContext.APP_Ricette.Add(appRicetta)
                    dbContext.SaveChanges()
                End If


                For Each operazioneDaSCrivere In listaOperazioni
                    ScritturaDatiComuni(operazioneDaSCrivere, username)
                    dbContext.APP_Ricette_Operazioni.Add(operazioneDaSCrivere)
                Next
                dbContext.SaveChanges()

                For Each dettaglioDaScrivere In listaDettagli
                    ScritturaDatiComuni(dettaglioDaScrivere, username)
                    dbContext.Add(dettaglioDaScrivere)
                Next
                dbContext.SaveChanges()

                For Each dettaglioTecnicoDaScrivere In listaDettaglioTecnico
                    ScritturaDatiComuni(dettaglioTecnicoDaScrivere, username)
                    dbContext.Add(dettaglioTecnicoDaScrivere)
                Next
                dbContext.SaveChanges()

                For Each destinazioneDaScrivere In listaDestinazioni
                    ScritturaDatiComuni(destinazioneDaScrivere, username)
                    dbContext.Add(destinazioneDaScrivere)
                Next
                dbContext.SaveChanges()

                transaction.Commit()

            Catch ex As Exception
                transaction.Rollback()
                Throw ex
            End Try
        End Using

    End Sub


    Private Sub ScritturaDatiComuni(oggettoDoveScrivere As APP_Agronica_Entity, username As String)

        Dim dataOra As Date = DateTime.Now()

        oggettoDoveScrivere.Data_Creazione = dataOra
        oggettoDoveScrivere.Data_Modifica = dataOra
        oggettoDoveScrivere.Username_Creazione = username
        oggettoDoveScrivere.Username_Modifica = username

        'valori non memorizzati danno luogo a data "01/01/0001", quindi allineo su agro-data.inizio/fine

        If oggettoDoveScrivere.Validita_Inizio < AGRODATAINIZIO Then
            oggettoDoveScrivere.Validita_Inizio = AGRODATAINIZIO
        End If

        If oggettoDoveScrivere.Validita_Fine < AGRODATAINIZIO Then
            oggettoDoveScrivere.Validita_Fine = AGRODATAFINE
        End If

    End Sub

    Public Sub CancellaRicetteScaricatePerAzienda(dbContext As GiasDbContext, Piva As String)

        Dim ricetteW As New Ricette_W
        Dim ricetteXnoteW As New RicetteXnote_W
        Dim ricetteOperazioniW As New Ricette_Operazioni_W
        Dim ricetteDettagliW As New Ricette_Dettagli_W
        Dim ricetteDettaglioTecnicoW As New Ricette_Dettaglio_Tecnico_W
        Dim ricetteDestinazioniW As New Ricette_Destinazioni_W

        ricetteXnoteW.CancellaDataImpresa(dbContext, Piva)
        ricetteOperazioniW.CancellaDataImpresa(dbContext, Piva)
        ricetteDettagliW.CancellaDataImpresa(dbContext, Piva)
        ricetteDettaglioTecnicoW.CancellaDataImpresa(dbContext, Piva)
        ricetteDestinazioniW.CancellaDataImpresa(dbContext, Piva)
        ricetteW.CancellaDataImpresa(dbContext, Piva)

        ' Attività Tempi Risorse
        Dim tempiRisorseBiz As New TempiRisorse
        tempiRisorseBiz.CancellaTempiRisorse(dbContext, Piva)

    End Sub

    Public Function LeggiRicetteMemorizzateSuSmartPhone(dbContext As GiasDbContext,
                                                        piva As String,
                                                        GestioneBozze As Boolean
                                                        ) As AgronicaCoreModelloSTD.RicettePerScarico

        Dim ricettaLetta As New AgronicaCoreModelloSTD.RicettePerScarico

        Dim ricetteR As New Ricette_R
        Dim ricetteXnoteR As New RicetteXnote_R
        Dim ricetteOperazioniR As New Ricette_Operazioni_R
        Dim ricetteDettagliR As New Ricette_Dettagli_R
        Dim ricetteDettaglioTecnicoR As New Ricette_Dettaglio_Tecnico_R
        Dim ricetteDestinazioniR As New Ricette_Destinazioni_R

        ricettaLetta.Ricette = ricetteR.LeggiPerRicaricoDati(dbContext, piva)
        ricettaLetta.RicetteOperazioni = ricetteOperazioniR.LeggiPerRicaricoDati(dbContext, piva)
        ricettaLetta.RicetteDettagli = ricetteDettagliR.LeggiPerRicaricoDati(dbContext, piva)
        ricettaLetta.RicetteDettaglioTecnico = ricetteDettaglioTecnicoR.LeggiPerRicaricoDati(dbContext, piva)
        ricettaLetta.RicetteDestinazioni = ricetteDestinazioniR.LeggiPerRicaricoDati(dbContext, piva)
        ricettaLetta.RicetteXNote = ricetteXnoteR.LeggiPerRicaricoDati(dbContext, piva)

        ' Attività Tempi Risorse
        Dim tempiRisorseBiz As New TempiRisorse
        ricettaLetta.Attivita = tempiRisorseBiz.LeggiAttivita(dbContext, piva)
        ricettaLetta.AttivitaMovimenti = tempiRisorseBiz.LeggiAttivitaMovimenti(dbContext, piva)
        ricettaLetta.AttivitaOperazioni = tempiRisorseBiz.LeggiAttivitaOperazioni(dbContext, piva)

        ' Escludi Bozze
        If GestioneBozze Then

            Dim escludiRicette = ricetteOperazioniR.LeggiPerEsclusioneBozze(dbContext, piva)
            ricettaLetta.Ricette = (From r In ricettaLetta.Ricette Where Not escludiRicette.Contains(r.ricetta_cod) Select r).ToList()
            ricettaLetta.RicetteOperazioni = (From r In ricettaLetta.RicetteOperazioni Where Not escludiRicette.Contains(r.Ricetta_Cod) Select r).ToList()
            ricettaLetta.RicetteDettagli = (From r In ricettaLetta.RicetteDettagli Where Not escludiRicette.Contains(r.Ricetta_Cod) Select r).ToList()
            ricettaLetta.RicetteDettaglioTecnico = (From r In ricettaLetta.RicetteDettaglioTecnico Where Not escludiRicette.Contains(r.Ricetta_Cod) Select r).ToList()
            ricettaLetta.RicetteDestinazioni = (From r In ricettaLetta.RicetteDestinazioni Where Not escludiRicette.Contains(r.Ricetta_Cod) Select r).ToList()
            ricettaLetta.RicetteXNote = (From r In ricettaLetta.RicetteXNote Where Not escludiRicette.Contains(r.Ricetta_Cod) Select r).ToList()

            Dim escludiAttivita = tempiRisorseBiz.LeggiPerEsclusioneBozze(dbContext, piva)
            ricettaLetta.Attivita = (From r In ricettaLetta.Attivita Where Not escludiAttivita.Contains(r.Id_Cdg_Generale) Select r).ToList()
            ricettaLetta.AttivitaMovimenti = (From r In ricettaLetta.AttivitaMovimenti Where Not escludiAttivita.Contains(r.Id_Cdg_Generale) Select r).ToList()
            ricettaLetta.AttivitaOperazioni = (From r In ricettaLetta.AttivitaOperazioni Where Not escludiAttivita.Contains(r.Id_Cdg_Generale_Rif) Select r).ToList()

        End If

        Return ricettaLetta

    End Function

    Public Function VerificaEsistenzaDatiDaInviare(dbContext As GiasDbContext, piva As String) As Boolean

        Dim letturaOperazioni As New Ricette_Operazioni_R
        Dim operazioniDaCancellare = letturaOperazioni.LeggiPerCancellazione(dbContext, piva)

        Dim tempiRisorseBiz As New TempiRisorse
        Dim attivitaDaCancellare = tempiRisorseBiz.LeggiPerCancellazione(dbContext, piva)

        Return (operazioniDaCancellare.Count + attivitaDaCancellare.Count) > 0

    End Function

    Public Sub CancellaRicetteInviate(dbContext As GiasDbContext, piva As String)

        Dim letturaOperazioni As New Ricette_Operazioni_R
        Dim operazioniDaCancellare = letturaOperazioni.LeggiPerCancellazione(dbContext, piva)
        For Each operazione In operazioniDaCancellare
            CancellaSingolaRicetta(dbContext, operazione.Ricetta_Cod, operazione.Ricetta_Operazione_Cod)
        Next

        Dim tempiRisorseBiz As New TempiRisorse
        Dim attivitaDaCancellare = tempiRisorseBiz.LeggiPerCancellazione(dbContext, piva)
        For Each attivita In attivitaDaCancellare
            tempiRisorseBiz.CancellaTempiRisorse(dbContext, attivita)
        Next

    End Sub

    Public Sub ScriviRicetteScaricate(dbContext As GiasDbContext,
                                      piva As String,
                                      Ricetta As AgronicaCoreModelloSTD.RicettePerScarico)

        'CancellaRicetteScaricatePerAzienda(dbContext, piva)
        For Each operazioneDaScrivere In Ricetta.RicetteOperazioni
            CancellaSingolaRicetta(dbContext, operazioneDaScrivere.Ricetta_Cod,
                                   operazioneDaScrivere.Ricetta_Operazione_Cod)
        Next

        For Each ricettaDaScrivere In Ricetta.Ricette
            dbContext.APP_Ricette.Add(ricettaDaScrivere)
            dbContext.SaveChanges()
        Next

        For Each operazioneDaScrivere In Ricetta.RicetteOperazioni
            Dim truncDesc = operazioneDaScrivere.Ricetta_Operazione_Des.IndexOf(" (")
            If truncDesc > 0 Then
                operazioneDaScrivere.Ricetta_Operazione_Des = operazioneDaScrivere.Ricetta_Operazione_Des.Substring(0, truncDesc)
            End If
            dbContext.APP_Ricette_Operazioni.Add(operazioneDaScrivere)
        Next
        dbContext.SaveChanges()

        For Each dettaglioDaScrivere In Ricetta.RicetteDettagli
            dbContext.Add(dettaglioDaScrivere)
        Next
        dbContext.SaveChanges()

        For Each dettaglioTecnicoDaScrivere In Ricetta.RicetteDettaglioTecnico
            dbContext.Add(dettaglioTecnicoDaScrivere)
        Next
        dbContext.SaveChanges()

        For Each destinazioneDaScrivere In Ricetta.RicetteDestinazioni
            dbContext.Add(destinazioneDaScrivere)
        Next
        dbContext.SaveChanges()

        For Each ricettaNodeDascrivere In Ricetta.RicetteXNote
            dbContext.Add(ricettaNodeDascrivere)
        Next
        dbContext.SaveChanges()

    End Sub

End Class
