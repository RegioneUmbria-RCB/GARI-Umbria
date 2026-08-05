Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreMapper.Utility
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.attivita
Imports AgronicaCoreModelsSTD.attivita.centri_di_costo
Imports AgronicaCoreModelsSTD.attivita.dettagli
Imports AgronicaCoreModelsSTD.attivita.risorse
Imports AgronicaCoreModelsSTD.contabilita
Imports AgronicaCoreModelsSTD.metaschema
Imports AgronicaCoreModelsSTD.costanti
Imports AgronicaCoreModello
Imports AgronicaCoreModello.Utility_Agenda
Imports AgronicaCoreModelsSTD.attivita.dettagli.Opzioni_Raccolta
Imports AgronicaCoreModelsSTD.attivita.Attivita
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports Newtonsoft.Json
Imports AgronicaCoreModelsSTD.meteo
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelsSTD.exceptions

Public Class AgendaToAttivita

    Public Function AgendaSuAttivita(
                                    agenda As Operazione_Agenda,
                                    verbose As Boolean,
                                    objParametri_Super_Server As AgronicaCoreParametri,
                                    objParametri_Server As AgronicaCoreParametri,
                                    objParametri_Utenti As AgronicaCoreParametri,
                                    Optional ByRef listParametriAggiuntivi As List(Of Parametri_Aggiuntivi_Attivita) = Nothing,
                                    Optional ByVal leggiDisciplinari As Boolean = True
                                    ) As Attivita

        '----- ATTIVITA
        Dim attivita As New Attivita

        '----- JOB
        CreaJob(attivita, agenda.Lav_Cod, objParametri_Server)

        Dim InfoOperazione As InfoOperazione = GetInfoOperazione(agenda.Lav_Cod, Attivita.Tipo_Attivita.QuadernoDiCampagna)

        '----- MOVIMENTO OPERAZIONE PRINCIPALE
        Dim MovimentoOperazione As Movimento = GetMovimentoFromCausale(agenda, InfoOperazione.Cau_Mov)

        '----- MOVIMENTO SCARICO
        Dim MovimentoScarico As Movimento = GetMovimentoFromCausale(agenda, CAU_SCARICO)

        '----- MOVIMENTO CARICO
        Dim MovimentoCarico As Movimento = GetMovimentoFromCausale(agenda, CAU_CARICO)

        Dim latitudine As Double = 0
        Dim longitudine As Double = 0
        If Not String.IsNullOrEmpty(agenda.GisWkt) Then
            Dim vWkt As String() = agenda.GisWkt.Split(",")
            latitudine = vWkt(0).Trim().Replace(".", ",")
            longitudine = vWkt(1).Trim().Replace(".", ",")
        End If
        With attivita
            .tipo = Attivita.Tipo_Attivita.QuadernoDiCampagna
            .codice = agenda.Id_Agenda
            .descrizione = agenda.Des_Lib
            .inizio = agenda.Data
            .oraInizio = MovimentoOperazione.Ora
            .oraFine = MovimentoOperazione.OraFine
            .statoWorkflow = CType(agenda.Stato_Cod, StatiWorkflowQdC) 'ora valorizzato solo per le visite
            .daRemoto = agenda.DaRemoto 'ora valorizzato solo per le visite
            .raccoglitore = agenda.Raccoglitore_Cod
            .note = MovimentoOperazione.Mov_Desc
            .modalita = MovimentoOperazione.Modalita
            .longitude = longitudine
            .latitude = latitudine
            .origine = agenda.Origine
            .appRicettaOperazioneID = ""
            .blocco = New Blocco(agenda.Blocco_Flag, agenda.Blocco_Username, agenda.Blocco_Data)
        End With

        attivita.associazionePK = LoadAssociazionePK(attivita, objParametri_Server)

        attivita.modalitaApplicazione = LoadModalitaApplicazione(MovimentoOperazione.Modalita_Applicazione, verbose, objParametri_Server)

        Dim superficieTrattataTotale As Decimal = 0
        Dim acquaTotale As Decimal = 0

        '----- CENTRO AZIENDALE
        attivita.centroAziendale = CreaCentroAziendale(agenda.Piva, agenda.Sa_Cod)

        If Not (InfoOperazione.IsCarico Or InfoOperazione.IsScarico) Then

            '----- UTILIZZO TERRENO
            If InfoOperazione.TipoCentroDiCosto = centri_di_costo.Tipo.ProdottoDaTrattare Then
                Dim primoProdottoDaTrattare As Movimento_Dettaglio = GetPrimoProdottoDaTrattare(agenda, InfoOperazione)
                If primoProdottoDaTrattare IsNot Nothing Then
                    attivita.utilizzoTerreno = BuildUtilizzoTerrenoFromProdottoDaTrattare(primoProdottoDaTrattare.Piva, primoProdottoDaTrattare.Elem_Cod, primoProdottoDaTrattare.Mat_Cod, verbose, objParametri_Server)
                End If
            Else
                Dim primoImpianto As Movimento_Destinazione = GetPrimoImpianto(agenda, InfoOperazione)
                If primoImpianto IsNot Nothing Then
                    attivita.utilizzoTerreno = BuildUtilizzoTerreno(primoImpianto.Piva, primoImpianto.Sa_Cod, primoImpianto.Appezza, primoImpianto.Id_Destinazione, verbose, objParametri_Server)
                End If
            End If


            '----- DISCIPLINARE
            If leggiDisciplinari Then
                attivita.disciplinare = Utility.BuildDisciplinare(InfoOperazione, MovimentoOperazione.Num_Protocollo, MovimentoOperazione.Doc_Numero, attivita.utilizzoTerreno, attivita.job.primaryKey.codice, attivita.inizio, verbose, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
            End If

            '----- EPOCA DPI/EPOCA FERTILIZZAZIONE/TIPO RACCOLTA
            CreaExtraInt(attivita, MovimentoOperazione, InfoOperazione)

            '----- ATTIVITA PERSONALIZZATA
            CreaAttivitaPersonalizzata(attivita, agenda)

            '----- NOTE
            CreaNote(attivita, agenda, verbose, objParametri_Server)

            '----- COSTI ACCESSORI
            CreaCostiAccessori(attivita, agenda, verbose, objParametri_Server)

            '----- ASSEGNATARI VISITE
            If InfoOperazione.IsVisita Then
                CreaAssegnatariVisita(attivita, agenda, verbose, objParametri_Server)
                CreaAttivitaCollegataVisita(agenda, attivita, verbose, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
            End If

            If InfoOperazione.IsRilievo Then
                Dim idVisitaCollegata = GetIdVisitaCollegata(agenda, attivita, verbose, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)

                If listParametriAggiuntivi IsNot Nothing Then

                    If idVisitaCollegata <> 0 Then
                        Dim paramAggiuntivo = New Parametri_Aggiuntivi_Attivita
                        paramAggiuntivo.key = Key_Parametri_Aggiuntivi_Attivita.Id_Visita_Collegata
                        paramAggiuntivo.value = idVisitaCollegata
                        listParametriAggiuntivi.Add(paramAggiuntivo)
                    End If
                End If

            End If

            If InfoOperazione.TipoCentroDiCosto = centri_di_costo.Tipo.ProdottoDaTrattare Then
                '----- GIACENZA MAGAZZINO CDC
                superficieTrattataTotale = CreaProdottoDaTrattareCdC(attivita, InfoOperazione, MovimentoOperazione)
            Else
                '----- ESERCIZI CDC
                superficieTrattataTotale = CreaEserciziCdC(attivita, InfoOperazione, MovimentoOperazione)
            End If

            '----- MOVIMENTO DETTAGLIO TECNICO X ACQUA
            acquaTotale = CreaRisorsaAcqua(attivita, agenda, MovimentoOperazione, superficieTrattataTotale)

            '----- MOVIMENTO DETTAGLIO TECNICO X CAUSALE (ora solo Rilievo produzione prevista)
            CreaRisorseCausale(attivita, agenda, MovimentoOperazione)
        Else
            If InfoOperazione.IsRegistrazione Then
                CreaRegistrazioneContabile(attivita, agenda, verbose, objParametri_Server)
            End If
        End If

        If MovimentoOperazione IsNot Nothing Then
            Dim creaDettagliIrrigazioneDefault As Boolean = InfoOperazione.IsFertirrigazione
            For Each movimentoDettaglioOperazione In MovimentoOperazione.Movimenti_Dettagli
                If InfoOperazione.IsTrattamento Then
                    Dim dettCorrente = CreaDettaglioTrattamento(agenda, InfoOperazione, attivita, movimentoDettaglioOperazione, MovimentoScarico, superficieTrattataTotale, acquaTotale, verbose, objParametri_Super_Server, objParametri_Server, objParametri_Utenti, MovimentoOperazione)
                    If dettCorrente IsNot Nothing Then
                        attivita.risorse.Add(dettCorrente)
                    End If
                End If
                If InfoOperazione.IsFertilizzazione AndAlso Not movimentoDettaglioOperazione.IsDettaglioIrrigazione Then
                    Dim dettCorrente = CreaDettaglioFertilizzazione(agenda, attivita, InfoOperazione, movimentoDettaglioOperazione, MovimentoScarico, superficieTrattataTotale, acquaTotale, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
                    If dettCorrente IsNot Nothing Then
                        attivita.risorse.Add(dettCorrente)
                    End If
                End If
                If InfoOperazione.IsSemina Then
                    Dim dettCorrente = CreaDettaglioSemina(agenda, attivita, InfoOperazione, movimentoDettaglioOperazione, MovimentoScarico, superficieTrattataTotale, acquaTotale, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
                    If dettCorrente IsNot Nothing Then
                        attivita.risorse.Add(dettCorrente)
                    End If
                End If
                If InfoOperazione.IsRaccolta Then
                    Dim dettCorrente = CreaDettaglioRaccolta(attivita, InfoOperazione, agenda, movimentoDettaglioOperazione, MovimentoCarico, superficieTrattataTotale, acquaTotale, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
                    Dim found As DettaglioRaccolta

                    dettCorrente.Opzioni_Raccolta.Ripartizione = MovimentoOperazione.Mezzo
                    dettCorrente.Opzioni_Raccolta.Modalita = MovimentoOperazione.Modalita
                    If attivita.tipoRaccolta = Attivita.Tipo_Raccolta.Leggera_Con_Dettagli_Magazzino AndAlso Not IsNothing(MovimentoCarico) Then
                        dettCorrente.dataIngresso = New DateTime(MovimentoCarico.Data.Year, MovimentoCarico.Data.Month, MovimentoCarico.Data.Day, MovimentoCarico.Ora.Hour, MovimentoCarico.Ora.Minute, 0)
                    End If

                    If dettCorrente.MagazziniMovimentazioni IsNot Nothing Then
                        found = attivita.risorse _
                            .Where(Function(r) r.classType = ClassType.DettaglioRaccolta) _
                            .Where(Function(r As RisorsaProdotto) r.prodotto.codice = dettCorrente.prodotto.codice AndAlso
                                r.MagazziniMovimentazioni.Any()) _
                            .FirstOrDefault(Function(r As RisorsaProdotto) _
                                                r.MagazziniMovimentazioni.First().Lotto.ToUpper = dettCorrente.MagazziniMovimentazioni.First().Lotto.ToUpper AndAlso
                                                r.MagazziniMovimentazioni.First().Magazzino.primaryKey.codice = dettCorrente.MagazziniMovimentazioni.First().Magazzino.primaryKey.codice)

                        If dettCorrente.MagazziniMovimentazioni.FirstOrDefault().Cod_Progetto <> 0 Then
                            dettCorrente.Opzioni_Raccolta.GenerazioneLotto = enum_Generazione_Lotto_Raccolta.DA_ESERCIZO
                        End If
                    Else
                        found = attivita.risorse _
                            .Where(Function(r) r.classType = ClassType.DettaglioRaccolta) _
                            .FirstOrDefault(Function(r As RisorsaProdotto) (r.prodotto.codice = dettCorrente.prodotto.codice AndAlso r.MagazziniMovimentazioni Is Nothing))
                    End If

                    dettCorrente.quantitaTotaleReale = If(IsNothing(movimentoDettaglioOperazione), 0, movimentoDettaglioOperazione.Qta)
                    If found IsNot Nothing Then
                        Dim quantitaSuImpianto As New QuantitaSuImpianto
                        quantitaSuImpianto.esercizioCDC = dettCorrente.QuantitaSuImpianti.First().esercizioCDC
                        quantitaSuImpianto.Qta = dettCorrente.QuantitaSuImpianti.First().Qta
                        found.QuantitaSuImpianti.Add(quantitaSuImpianto)
                        found.quantitaTotaleReale += dettCorrente.quantitaTotaleReale
                    Else
                        'Aggiornamento delle quantità su impianto già eseguito in CreaDettaglioRaccolta
                        If (dettCorrente.quantitaTotaleReale = 0 AndAlso dettCorrente.prodotto.codice > 0) Then
                            'Evito di inserire dettagli creati con quantità nulla
                            Continue For
                        End If
                        attivita.risorse.Add(dettCorrente)
                    End If
                End If
                If InfoOperazione.IsRilievo Then
                    Dim dettCorrenteList = CreaDettagliRilievo(attivita, InfoOperazione, agenda, movimentoDettaglioOperazione)
                    If dettCorrenteList IsNot Nothing Then
                        attivita.risorse.AddRange(dettCorrenteList)
                    End If
                End If
                If InfoOperazione.IsIrrigazione Then
                    Dim dettCorrenteList = CreaDettagliIrrigazione(attivita, InfoOperazione, agenda, movimentoDettaglioOperazione)
                    If dettCorrenteList IsNot Nothing Then
                        attivita.risorse.AddRange(dettCorrenteList)
                    End If
                End If
                If InfoOperazione.IsFertirrigazione AndAlso movimentoDettaglioOperazione.IsDettaglioIrrigazione Then
                    Dim dettCorrenteList = CreaDettagliIrrigazione(attivita, InfoOperazione, agenda, movimentoDettaglioOperazione)
                    If dettCorrenteList IsNot Nothing Then
                        attivita.risorse.AddRange(dettCorrenteList)
                        creaDettagliIrrigazioneDefault = False
                    End If
                End If
                If InfoOperazione.IsVisita Then
                    Dim risorsaCorrente = CreaRisorsaVisita(movimentoDettaglioOperazione, verbose, objParametri_Server)
                    If risorsaCorrente IsNot Nothing Then
                        attivita.risorse.Add(risorsaCorrente)
                    End If
                End If

                If InfoOperazione.IsCarico Then
                    Dim dettCorrente = CreaRegistrazione(agenda, InfoOperazione, movimentoDettaglioOperazione, MovimentoCarico, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
                    If dettCorrente IsNot Nothing Then
                        attivita.risorse.Add(dettCorrente)
                    End If
                End If
                If InfoOperazione.IsScarico Then
                    Dim dettCorrente = CreaRegistrazione(agenda, InfoOperazione, movimentoDettaglioOperazione, MovimentoScarico, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
                    If dettCorrente IsNot Nothing Then
                        attivita.risorse.Add(dettCorrente)
                    End If
                End If
            Next

            ' creo i dettagli irrigazione default per la nuova fertirrigazione
            If InfoOperazione.IsFertirrigazione AndAlso creaDettagliIrrigazioneDefault Then
                Dim doseAcqua As Decimal = acquaTotale / superficieTrattataTotale / 10
                Dim dettagliIrrigazione = CreaDettagliIrrigazione(attivita, doseAcqua)
                If dettagliIrrigazione IsNot Nothing Then
                    attivita.risorse.AddRange(dettagliIrrigazione)
                End If
            End If

            If InfoOperazione.IsTrattamento AndAlso agenda.Lav_Cod = LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA Then
                CreaAttivitaCollegataInstallazioneTrappole(agenda, attivita, verbose, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
            End If

        End If

        If verbose Then
            CompleteVerbose(attivita, isRibaltamentoToAgenda:=False, listParametriAggiuntivi:=Nothing, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
        End If

        Return attivita

    End Function



#Region "Funzioni di creazione oggetti del nuovo modello"
    Public Sub CreaJob(attivita As Attivita, Lav_Cod As String, objParametri_Server As AgronicaCoreParametri)
        Dim job As Job = Nothing

        Dim objOperazioni As New AgronicaCoreAnagrafeDAL.Operazioni_R
        Dim dtOperazioni = objOperazioni.GruppoOperazioni_From_Lav_Cod(Lav_Cod, objParametri_Server)
        If dtOperazioni IsNot Nothing AndAlso dtOperazioni.Rows.Count > 0 Then
            Dim tipo As String = dtOperazioni.Rows(0).Item("tipo")
            Select Case tipo
                Case "C", "V"
                    job = New AgronicaCoreModelsSTD.attivita.Lavorazione(Lav_Cod)
                Case "E"
                    job = New Registrazione(Lav_Cod)
            End Select
        End If
        If job IsNot Nothing Then
            attivita.job = job
        Else
            Dim messaggio = String.Format(My.Resources.AgronicaCoreMapper.TipoLavorazioneNonGestito, Lav_Cod)
            Throw New GiasException(messaggio)
        End If
    End Sub

    Public Function CreaCentroAziendale(Piva As String, Sa_Cod As Integer) As CentroAziendale

        Dim centroAziendale = New CentroAziendale(New CentroAziendale.PK(Sa_Cod, Piva))
        Return centroAziendale
    End Function
    Private Sub CreaAttivitaPersonalizzata(attivita As Attivita, agenda As Operazione_Agenda)

        If (agenda.Lav_Cod = LAVCOD_ALTRE_OPERAZIONI OrElse agenda.Lav_Cod = LAVCOD_VISITA) AndAlso agenda.Id_Attivita > 0 Then
            attivita.attivitaPersonalizzata = New AttivitaPersonalizzata(agenda.Id_Attivita)
        End If

    End Sub

    Private Sub CreaExtraInt(attivita As Attivita, MovimentoOperazione As Movimento, infoOperazione As InfoOperazione)
        If MovimentoOperazione.Extra_Int <> 0 Then

            If infoOperazione.IsRaccolta Then
                attivita.tipoRaccolta = MovimentoOperazione.Extra_Int
            Else
                Dim epoca As New Epoca(MovimentoOperazione.Extra_Int)
                attivita.epoca = epoca
            End If

        End If
    End Sub

    Private Sub CreaNote(attivita As Attivita, agenda As Operazione_Agenda, verbose As Boolean, objParametri_Server As AgronicaCoreParametri)

        attivita.noteIntervento = New List(Of note_intervento.NoteIntervento)

        Dim NoteBiz As New AgronicaControlli_2010.STD_Note
        Dim noteList = NoteBiz.LeggiNote(attivita.tipo,
                                             meno1tutti_0nonVisibili_1soloVisibili:=-1,
                                             objParametri_Server)

        For Each nota In agenda.Note
            Dim notaIntervento As New note_intervento.NoteIntervento(nota.Nota_Cod)

            Dim notaItem = noteList.FirstOrDefault(Function(p) p.codice = nota.Nota_Cod)
            If notaItem IsNot Nothing Then
                If notaItem.noteInterventoGruppi IsNot Nothing Then
                    notaIntervento.noteInterventoGruppi = New note_intervento.NoteInterventoGruppi(notaItem.noteInterventoGruppi.codice)
                End If

                If verbose Then
                    notaIntervento.descrizione = notaItem.descrizione
                End If

            End If

            attivita.noteIntervento.Add(notaIntervento)
        Next

    End Sub

    Private Function CreaRisorsaAcqua(attivita As Attivita, agenda As Operazione_Agenda, movimentoOperazione As Movimento, superficieTrattataTotale As Decimal) As Decimal

        Dim acquaTotale As Decimal = 0

        Select Case agenda.Lav_Cod

            Case LAVCOD_CONCIMAZIONE_FOGLIARE, LAVCOD_FERTIRRIGAZIONE, LAVCOD_TRATTAMENTO_ANTIBUTTERATURA,
                 LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_DISERBO,
                 LAVCOD_DISSECCAMENTO, LAVCOD_GEODISINFESTAZIONE,
                 LAVCOD_CONCIA_SEME, LAVCOD_TRATTAMENTO_FITOREGOLATORE,
                 LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE,
                 LAVCOD_TRATTAMENTO_POST_RACCOLTA

                'DT: si assume che ci sia al massimo un movimento tecnico acqua
                Dim Movimento_Dettaglio_Tecnico_Acqua As Movimento_Dettaglio_Tecnico = movimentoOperazione.Movimenti_Dettagli_Tecnici _
                    .FirstOrDefault(Function(c) (c.Id_Mov_Det = 0 AndAlso c.dett_cod = 0))

                If Movimento_Dettaglio_Tecnico_Acqua IsNot Nothing Then
                    Dim risorsaAcqua = New RisorsaAcqua

                    If Movimento_Dettaglio_Tecnico_Acqua.Qta_Ril > 0 Then
                        risorsaAcqua.acqua = Movimento_Dettaglio_Tecnico_Acqua.Qta_Ril
                        risorsaAcqua.doseAcqua = RisorsaAcqua.TipoDoseAcqua.TOTALE

                        acquaTotale = risorsaAcqua.acqua
                    Else
                        risorsaAcqua.acqua = -Movimento_Dettaglio_Tecnico_Acqua.Qta_Ril
                        risorsaAcqua.doseAcqua = RisorsaAcqua.TipoDoseAcqua.HA

                        acquaTotale = risorsaAcqua.acqua * superficieTrattataTotale
                    End If

                    attivita.risorse.Add(risorsaAcqua)

                End If

        End Select

        Return acquaTotale

    End Function

    Private Sub CreaRisorseCausale(attivita As Attivita, agenda As Operazione_Agenda, movimentoOperazione As Movimento)

        Select Case agenda.Lav_Cod

            Case LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA

                attivita.risorse.AddRange(
                    movimentoOperazione.Movimenti_Dettagli_Tecnici _
                        .Where(Function(c) (c.Id_Mov_Det = 0 AndAlso c.dett_cod <> 0)) _
                        .Select(Function(c) New RisorsaCausale With {.id = c.dett_cod})
                    )

        End Select

    End Sub


    Private Sub CreaCostiAccessori(attivita As Attivita, agenda As Operazione_Agenda, verbose As Boolean, objParametri_Server As AgronicaCoreParametri)
        'DT: udm, qta e prezzo non vengono salvate, si usa il "Salva e vai ai costi" o comunque il CdG 

        Dim causaliCostiAccessori = New List(Of String)() From {
            CAU_IMPUTAZIONE_PARCOMACCHINE,
            CAU_IMPUTAZIONE_MANODOPERA,
            CAU_IMPUTAZIONE_TECNICO_RESPONSABILE,
            CAU_IMPUTAZIONE_TERZISTI
        }

        Dim Movimenti_Costi_Accessori As List(Of Movimento) = (From m In agenda.Movimenti Where causaliCostiAccessori.Contains(m.Cau_Mov.ToString) Select m).ToList

        For Each costoAccessorio In Movimenti_Costi_Accessori

            For Each costoAccessorioDettaglio In costoAccessorio.Movimenti_Dettagli

                Select Case costoAccessorio.Cau_Mov
                    Case CAU_IMPUTAZIONE_PARCOMACCHINE
                        Dim risorsa = New RisorsaMacchina
                        risorsa.macchina = New ParcoMacchine
                        risorsa.macchina.codice = costoAccessorioDettaglio.Mat_Cod

                        attivita.risorse.Add(risorsa)

                    Case CAU_IMPUTAZIONE_MANODOPERA, CAU_IMPUTAZIONE_TECNICO_RESPONSABILE, CAU_IMPUTAZIONE_TERZISTI
                        Dim risorsa = New RisorsaPersona
                        risorsa.risorsaUmana = New RisorseUmane
                        risorsa.risorsaUmana.codice = costoAccessorioDettaglio.Mat_Cod

                        risorsa.risorsaUmana = BuildRisorsaUmana(risorsa.risorsaUmana.codice, True, objParametri_Server)

                        attivita.risorse.Add(risorsa)

                End Select

            Next

        Next

    End Sub

    Private Sub CreaAssegnatariVisita(attivita As Attivita, agenda As Operazione_Agenda, verbose As Boolean, objParametri_Server As AgronicaCoreParametri)

        Dim Movimenti_Assegnatari_Visita As List(Of Movimento) = (From m In agenda.Movimenti Where m.Cau_Mov = CAU_ASSEGNATARIO_VISITA Select m).ToList

        For Each assegnatarioVisita In Movimenti_Assegnatari_Visita

            For Each assegnatarioVisitaDettaglio In assegnatarioVisita.Movimenti_Dettagli

                Dim risorsa = New RisorsaAssegnatarioVisita
                risorsa.risorsaUmana = New RisorseUmane
                risorsa.risorsaUmana.codice = assegnatarioVisitaDettaglio.Mat_Cod

                risorsa.risorsaUmana = BuildRisorsaUmana(risorsa.risorsaUmana.codice, verbose, objParametri_Server)

                attivita.risorse.Add(risorsa)

            Next

        Next

    End Sub


    Public Function CreaEserciziCdC(attivita As Attivita, infoOperazione As InfoOperazione, movimentoOperazione As Movimento) As Decimal

        Dim superficieTrattataTotale As Decimal = 0

        Dim impiantiDict As List(Of Integer) = New List(Of Integer)

        For Each movimentoDettaglio In movimentoOperazione.Movimenti_Dettagli
            For Each movimentoDestinazione In movimentoDettaglio.Movimenti_Destinazioni

                If Not impiantiDict.Contains(movimentoDestinazione.Appezza) Then
                    impiantiDict.Add(movimentoDestinazione.Appezza)

                    superficieTrattataTotale += movimentoDestinazione.Qta2

                    Dim esercizioCDC As EsercizioCDC = CreaEsercizioCdC(attivita, infoOperazione, movimentoDestinazione.Piva, movimentoDestinazione.Sa_Cod, movimentoDestinazione.Appezza, movimentoDestinazione.Id_Destinazione,
                                                                            movimentoDestinazione.Progetto_Cod, movimentoDestinazione.Qta2,
                                                                            movimentoDestinazione.Sup_Riduzione_BufferZone, movimentoDestinazione.Perc_Riduzione_Deriva)

                    If Not IsNothing(esercizioCDC) Then
                        attivita.centriDiCosto.Add(esercizioCDC)
                    End If

                End If

            Next
        Next

        Return superficieTrattataTotale

    End Function

    Public Function CreaProdottoDaTrattareCdC(attivita As Attivita, infoOperazione As InfoOperazione, movimentoOperazione As Movimento) As Decimal

        Dim quantitaTrattataTotale As Decimal = 0

        Dim prodottiDict As New List(Of String)

        For Each movimentoDettaglio In movimentoOperazione.Movimenti_Dettagli

            For Each movimentoDestinazione In movimentoDettaglio.Movimenti_Destinazioni
                If movimentoDestinazione.Tipo = MAGAZZINO AndAlso movimentoDestinazione.Qta = 0 Then

                    Dim chiaveProdotto As String = CStr(movimentoDestinazione.Piva) + "_" + CStr(movimentoDestinazione.Sa_Cod) + "_" + CStr(movimentoDestinazione.Id_Destinazione) + "_" +
                                                   CStr(movimentoDettaglio.Mat_Cod) + "_" + CStr(movimentoDettaglio.Cod_Progetto) + "_" + CStr(movimentoDettaglio.Lotto)

                    If Not prodottiDict.Contains(chiaveProdotto) Then

                        prodottiDict.Add(chiaveProdotto)

                        quantitaTrattataTotale += movimentoDettaglio.Qta / 100

                        Dim prodottoDaTrattareCdC As ProdottoDaTrattareCDC = CreaProdottoDaTrattareCdC(attivita, infoOperazione, movimentoDettaglio, movimentoDestinazione)

                        attivita.centriDiCosto.Add(prodottoDaTrattareCdC)

                    End If
                End If
            Next
        Next

        Return quantitaTrattataTotale

    End Function


    Private Function CreaEsercizioCdC(ByVal attivita As Attivita, ByVal infoOperazione As InfoOperazione,
                                      ByVal Piva As String, ByVal Sa_Cod As Integer, ByVal Appezza As Integer,
                                      ByVal Id_Destinazione As Integer, ByVal Progetto_Cod As Integer,
                                      ByVal Qta2 As Decimal, ByVal Sup_Riduzione_BufferZone As Decimal, ByVal Perc_Riduzione_Deriva As Decimal) As EsercizioCDC

        Dim esercizioCDC As EsercizioCDC = Nothing

        If Piva <> "" AndAlso Sa_Cod > 0 AndAlso Appezza > 0 AndAlso Id_Destinazione > 0 Then

            Dim appezzamentoPK = New Appezzamento.PK(Appezza, attivita.centroAziendale.primaryKey)
            Dim impiantoPK = New Impianto.PK(Id_Destinazione, appezzamentoPK)

            Dim esercizio = New Esercizio(Progetto_Cod, "") With {
                .impiantoPK = impiantoPK
            }

            esercizioCDC = New EsercizioCDC With {
                .superficieTrattata = Qta2,
                .esercizio = esercizio
            }

            If infoOperazione.IsTrattamento OrElse infoOperazione.IsFertilizzazione Then
                esercizioCDC.superficieRiduzioneBufferZone = Sup_Riduzione_BufferZone
                esercizioCDC.percentualeRiduzioneDeriva = Perc_Riduzione_Deriva
            End If

        End If

        Return esercizioCDC

    End Function

    Private Function CreaProdottoDaTrattareCdC(attivita As Attivita, infoOperazione As InfoOperazione, movimentoDettaglio As Movimento_Dettaglio, movimentoDestinazione As Movimento_Destinazione) As ProdottoDaTrattareCDC

        Dim magazzinoPK = New Fabbricato(movimentoDestinazione.Piva, movimentoDestinazione.Sa_Cod, movimentoDestinazione.Id_Destinazione, "")
        Dim prodottoPK = New Prodotto(movimentoDettaglio.Mat_Cod) With {
            .elemCod = movimentoDettaglio.Elem_Cod
        }

        Dim prodottoDaTrattareCdC As New ProdottoDaTrattareCDC With {
            .giacenzaMagazzino = New MovimentoDiMagazzino With {
                .Magazzino = magazzinoPK,
                .Prodotto = prodottoPK,
                .codice_progetto = movimentoDettaglio.Cod_Progetto,
                .Lotto = movimentoDettaglio.Lotto,
                .UdM = New UnitaDiMisura(movimentoDettaglio.Udm_Cod)
            },
            .qtaTrattata = movimentoDettaglio.Qta / 100
        }

        Return prodottoDaTrattareCdC

    End Function

    Private Function CreaEsercizioRilievoCdC(attivita As Attivita, infoOperazione As InfoOperazione, movimentoDestinazione As Movimento_Destinazione) As EsercizioRilievoCDC

        Dim esercizioRilievoCDC As EsercizioRilievoCDC = Nothing
        Dim esercizioCDC As EsercizioCDC = CreaEsercizioCdC(attivita, infoOperazione, movimentoDestinazione.Piva, movimentoDestinazione.Sa_Cod, movimentoDestinazione.Appezza, movimentoDestinazione.Id_Destinazione,
                                                                            movimentoDestinazione.Progetto_Cod, movimentoDestinazione.Qta2,
                                                                            movimentoDestinazione.Sup_Riduzione_BufferZone, movimentoDestinazione.Perc_Riduzione_Deriva)

        If Not IsNothing(esercizioCDC) Then
            esercizioRilievoCDC = New EsercizioRilievoCDC With {
             .esercizio = esercizioCDC.esercizio,
             .superficieTrattata = esercizioCDC.superficieTrattata
            }
        End If


        Return esercizioRilievoCDC
    End Function

    Private Sub CreaRegistrazioneContabile(attivita As Attivita, agenda As Operazione_Agenda, verbose As Boolean, objParametri_Server As AgronicaCoreParametri)

        Dim MovimentoRegistrazione As Movimento = GetMovimentoFromCausale(agenda, CAU_REGISTRAZIONI)

        If MovimentoRegistrazione IsNot Nothing Then

            Dim registrazioneContabile As New RegistrazioneContabile(MovimentoRegistrazione.Doc_Numero, MovimentoRegistrazione.Mov_Desc)

            registrazioneContabile.rifDocumento = MovimentoRegistrazione.Doc_Numero_Sin
            registrazioneContabile.anno = MovimentoRegistrazione.Doc_Numero_Des
            registrazioneContabile.colli = MovimentoRegistrazione.Colli

            Dim risorsa = New RisorsaPersona
            risorsa.risorsaUmana = New RisorseUmane
            risorsa.risorsaUmana.codice = MovimentoRegistrazione.Cod_Risum

            risorsa.risorsaUmana = BuildRisorsaUmana(risorsa.risorsaUmana.codice, verbose, objParametri_Server)

            registrazioneContabile.contraente = risorsa.risorsaUmana

            attivita.registrazioneContabile = registrazioneContabile

        End If

    End Sub

    Private Function CreaDettaglioFertilizzazione(agenda As Operazione_Agenda, attivita As Attivita, infoOperazione As InfoOperazione, MovimentoDettaglioOperazione As Movimento_Dettaglio, MovimentoScarico As Movimento, superficieTrattataTotale As Decimal, acquaTotale As Decimal,
                                                  ByVal objParametri_Super_Server As AgronicaCoreParametri, ByVal objParametri_Server As AgronicaCoreParametri, ByVal objParametri_Utenti As AgronicaCoreParametri) As dettagli.DettaglioFertilizzazione

        Dim dettCorrente = New dettagli.DettaglioFertilizzazione

        CreaRisorsaProdotto(agenda, attivita, infoOperazione, dettCorrente, MovimentoDettaglioOperazione, MovimentoScarico, superficieTrattataTotale, acquaTotale, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)

        If dettCorrente IsNot Nothing Then
            If MovimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici IsNot Nothing AndAlso MovimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici.Any() Then
                dettCorrente.N = MovimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici(0).N
                dettCorrente.P = MovimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici(0).P
                dettCorrente.K = MovimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici(0).K
                dettCorrente.Cu = MovimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici(0).Cu
                dettCorrente.Mg = MovimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici(0).M
                dettCorrente.efficienza = MovimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici(0).Efficienza

            End If
        End If

        Return dettCorrente

    End Function

    Private Function CreaDettaglioSemina(agenda As Operazione_Agenda, attivita As Attivita, infoOperazione As InfoOperazione, MovimentoDettaglioOperazione As Movimento_Dettaglio, MovimentoScarico As Movimento, superficieTrattataTotale As Decimal, acquaTotale As Decimal,
                                         ByVal objParametri_Super_Server As AgronicaCoreParametri, ByVal objParametri_Server As AgronicaCoreParametri, ByVal objParametri_Utenti As AgronicaCoreParametri) As dettagli.DettaglioSemina

        Dim dettCorrente = New dettagli.DettaglioSemina

        CreaRisorsaProdotto(agenda, attivita, infoOperazione, dettCorrente, MovimentoDettaglioOperazione, MovimentoScarico, superficieTrattataTotale, acquaTotale, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)

        Return dettCorrente

    End Function

    Private Sub CreaAttivitaCollegataVisita(agenda As Operazione_Agenda, attivita As Attivita, verbose As Boolean, objParametri_Super_Server As AgronicaCoreParametri, objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri)

        If agenda.Movimenti IsNot Nothing AndAlso agenda.Movimenti.Any() Then

            For Each movimento In agenda.Movimenti

                If movimento.Movimenti_Dettagli IsNot Nothing AndAlso movimento.Movimenti_Dettagli.Any() Then

                    For Each movimento_dettaglio In movimento.Movimenti_Dettagli

                        If movimento_dettaglio.Movimenti_Dettagli_Riferimenti IsNot Nothing AndAlso movimento_dettaglio.Movimenti_Dettagli_Riferimenti.Any() Then

                            Dim objAgenda As New Agenda_Operazione_Helper
                            Dim map As New AgronicaCoreMapper.AgendaToAttivita

                            For Each movimento_dettaglio_rif In movimento_dettaglio.Movimenti_Dettagli_Riferimenti

                                If movimento_dettaglio_rif.Cau_Mov_Rif = CAU_ASSEGNATARIO_VISITA Then

                                    Dim agendaRif As Operazione_Agenda = objAgenda.Leggi(movimento_dettaglio_rif.Piva_Rif, 0, movimento_dettaglio_rif.Id_Agenda_Rif, 0, objParametri_Server)
                                    If agendaRif IsNot Nothing Then

                                        Dim attivitaCollegata = map.AgendaSuAttivita(agendaRif, verbose, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)

                                        If attivita.attivitaCollegate Is Nothing Then
                                            attivita.attivitaCollegate = New List(Of Attivita)
                                        End If

                                        attivita.attivitaCollegate.Add(attivitaCollegata)

                                    End If

                                End If

                            Next

                        End If

                    Next

                End If

            Next

        End If

    End Sub

    Private Function GetIdVisitaCollegata(agenda As Operazione_Agenda, attivita As Attivita, verbose As Boolean, objParametri_Super_Server As AgronicaCoreParametri, objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri) As Integer

        Dim idVisitaCollegata As Integer = 0

        Dim objMovRif = New AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R

        Dim dtRif = objMovRif.LeggixChiave(agenda.Piva, 0, 0, 0, 0, agenda.Id_Agenda, 0, 0, 0, "", "", objParametri_Server)

        If dtRif IsNot Nothing AndAlso dtRif.Rows.Count > 0 Then
            For Each drRif In dtRif.Rows
                If drRif.item("Cau_Mov_Rif") = CAU_ASSEGNATARIO_VISITA AndAlso drRif.item("Id_Agenda") <> 0 Then

                    idVisitaCollegata = CInt(drRif.item("Id_Agenda"))

                End If
            Next
        End If

        Return idVisitaCollegata

    End Function

    Private Function CreaDettaglioRaccolta(attivita As Attivita, infoOperazione As InfoOperazione, agenda As Operazione_Agenda, MovimentoDettaglioOperazione As Movimento_Dettaglio, MovimentoCarico As Movimento, superficieTrattataTotale As Decimal, acquaTotale As Decimal,
                                           ByVal objParametri_Super_Server As AgronicaCoreParametri, ByVal objParametri_Server As AgronicaCoreParametri, ByVal objParametri_Utenti As AgronicaCoreParametri) As dettagli.DettaglioRaccolta

        Dim dettCorrente = New dettagli.DettaglioRaccolta

        CreaRisorsaProdotto(agenda, attivita, infoOperazione, dettCorrente, MovimentoDettaglioOperazione, MovimentoCarico, superficieTrattataTotale, acquaTotale, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)

        If dettCorrente IsNot Nothing Then
            dettCorrente.calCod = MovimentoDettaglioOperazione.Cal_Cod
        End If

        Return dettCorrente

    End Function

    Private Function CreaRisorsaVisita(MovimentoDettaglioOperazione As Movimento_Dettaglio, verbose As Boolean, objParametri_Server As AgronicaCoreParametri) As Risorsa

        Dim risorsaCorrente As Risorsa = Nothing

        If MovimentoDettaglioOperazione.Veg_Cod > 0 Then

            Dim veg_des As String = ""

            If verbose Then
                veg_des = New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R().VegDes_from_VegCod(MovimentoDettaglioOperazione.Veg_Cod, objParametri_Server)
            End If

            risorsaCorrente = New RisorsaSpecie() With {
                    .specie = New utilizzi.Specie(MovimentoDettaglioOperazione.Veg_Cod, veg_des)
                }

        ElseIf MovimentoDettaglioOperazione.Id_Cod > 0 Then

            Dim id_des As String = ""

            If verbose Then
                Dim codice_R As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R
                Dim DT = codice_R.Leggi(MovimentoDettaglioOperazione.Id_Cod, "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

                If (DT.Rows.Count = 1) Then
                    id_des = DT.Rows(0)("descrizione")
                End If
            End If

            risorsaCorrente = New RisorsaDestinazioneUso() With {
                .destinazioneUso = New utilizzi.DestinazioneUso(MovimentoDettaglioOperazione.Id_Cod) With {
                       .descrizione = id_des
                   }
            }

        ElseIf MovimentoDettaglioOperazione.Gen_Cod > 0 AndAlso MovimentoDettaglioOperazione.Spe_Cod > 0 Then

            Dim zoo_des As String = ""

            If verbose Then
                Dim codice_R As New AgronicaCoreZooDAL.Lista_Indprod_Animali_R
                Dim DT = codice_R.LeggiRisorseZootecniche(MovimentoDettaglioOperazione.Gen_Cod, MovimentoDettaglioOperazione.Spe_Cod, MovimentoDettaglioOperazione.IPro_Cod, "", "", objParametri_Server)

                If (DT.Rows.Count = 1) Then
                    zoo_des = DT.Rows(0)("ZOO_DES")
                End If
            End If

            risorsaCorrente = New RisorsaZootecnica(MovimentoDettaglioOperazione.Gen_Cod, MovimentoDettaglioOperazione.Spe_Cod, MovimentoDettaglioOperazione.IPro_Cod) With {
                .descrizione = zoo_des
            }
        End If


        Return risorsaCorrente

    End Function

    Private Function CreaDettaglioTrattamento(agenda As Operazione_Agenda, infoOperazione As InfoOperazione,
                                              attivita As Attivita,
                                              MovimentoDettaglioOperazione As Movimento_Dettaglio,
                                              MovimentoScarico As Movimento,
                                              superficieTrattataTotale As Decimal,
                                              acquaTotale As Decimal,
                                              verbose As Boolean,
                                              ByVal objParametri_Super_Server As AgronicaCoreParametri,
                                              ByVal objParametri_Server As AgronicaCoreParametri,
                                              ByVal objParametri_Utenti As AgronicaCoreParametri,
                                              MovimentoOperazione As Movimento) As dettagli.DettaglioTrattamento

        Dim dettCorrente = New dettagli.DettaglioTrattamento

        If infoOperazione.TipoCentroDiCosto = centri_di_costo.Tipo.ProdottoDaTrattare AndAlso
            (MovimentoDettaglioOperazione.Elem_Cod = TRASFORMATI_VEGETALI OrElse MovimentoDettaglioOperazione.Elem_Cod = SEMENTI) Then
            'Quando il centro di costo è un prodotto da trattare, non devo creare un DettaglioTrattamento dal current MovimentoDettaglioOperazione
            Return Nothing
        End If

        CreaRisorsaProdotto(agenda, attivita, infoOperazione, dettCorrente, MovimentoDettaglioOperazione, MovimentoScarico, superficieTrattataTotale, acquaTotale, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)

        If dettCorrente IsNot Nothing Then

            If infoOperazione.PregressoConMultiAvversita Then
                If MovimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici IsNot Nothing AndAlso
                    MovimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici.Count = 0 Then
                    'Caso di apertura pregresso con N Avversità collegate al movimento e non al dettaglio:
                    'Prendo solo la prima avversità e la collego a tutti i dettagli presenti

                    Dim Movimento_Dettaglio_Tecnico = MovimentoOperazione.Movimenti_Dettagli_Tecnici(0)

                    For Each dettaglio In MovimentoOperazione.Movimenti_Dettagli
                        'Aggiungo la chiave del dettaglio a cui lo collego e inserisco il nuovo dettaglio tecnico ora figlio del dettaglio
                        Movimento_Dettaglio_Tecnico.Id_Mov_Det = dettaglio.Id_Mov_Det
                        dettaglio.Movimenti_Dettagli_Tecnici.Add(Movimento_Dettaglio_Tecnico)
                    Next
                ElseIf MovimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici IsNot Nothing AndAlso
                    MovimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici.Count > 1 Then
                    'Caso di apertura pregresso con N Avversità collegate al dettaglio:
                    'Prendo solo la prima avversità

                    Dim Dummy = MovimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici(0)
                    MovimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici.Clear()
                    MovimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici.Add(Dummy)
                End If
            End If

            If MovimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici IsNot Nothing AndAlso MovimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici.Any() Then

                Dim movimentoDettaglioAvversita = MovimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici(0)
                dettCorrente.avversitaGruppo = GetAvversitaGruppo(movimentoDettaglioAvversita.Av_Cod, movimentoDettaglioAvversita.Av_Gru)

                If Not IsNothing(dettCorrente.avversitaGruppo) Then

                    Dim Pro_Cod As Integer = If(movimentoDettaglioAvversita.Av_Cod <> 0, movimentoDettaglioAvversita.Av_Cod, movimentoDettaglioAvversita.Av_Gru)

                    CreaRilevamentiMagazzino(Nothing, dettCorrente.avversitaGruppo,
                                              infoOperazione, MovimentoScarico, superficieTrattataTotale, acquaTotale,
                                              agenda.Piva, agenda.Sa_Cod, agenda.Id_Agenda, Pro_Cod, 0, INNESCHI,
                                              "", 0,
                                              agenda.Agenda_Riferimenti, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)

                End If

                dettCorrente.tipoFormulato = GetTipoFormulato(dettCorrente.avversitaGruppo, agenda.Lav_Cod)
                dettCorrente.soglia = GetSoglia(movimentoDettaglioAvversita.Soglia_Cod, movimentoDettaglioAvversita.Soglia_Quantita)

                If infoOperazione.Elem_Cod = INSETTI Then
                    If movimentoDettaglioAvversita.Sigla_av = "IMPOLL" Then
                        dettCorrente.isImpollinatore = True
                    End If
                End If

            End If

            dettCorrente.dosiEtichetta = DosiEtichetta_from_Stringhe(MovimentoDettaglioOperazione.DoseEtichetta, MovimentoDettaglioOperazione.DoseEtichetta_Value, verbose, objParametri_Server)
            dettCorrente.principiAttivi = GetPrincipiAttivi(MovimentoDettaglioOperazione.PrincipiAttivi, MovimentoDettaglioOperazione.PrincipiAttiviPesi, MovimentoDettaglioOperazione.PrincipiAttiviPercAbb)
            dettCorrente.bufferzone = GetBufferZone(MovimentoDettaglioOperazione.Buffer)
            dettCorrente.dettaglioProdotto = GetDettaglioProdotto(MovimentoDettaglioOperazione.Extra_Str)
            dettCorrente.tempoCarenza = MovimentoDettaglioOperazione.TempoCarenza
            dettCorrente.polverulento = MovimentoDettaglioOperazione.Polverulento
            dettCorrente.ripartizioneTrappole = Nothing

            If agenda.Lav_Cod = LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA OrElse agenda.Lav_Cod = LAVCOD_REINNESCO_TRAPPOLE Then
                dettCorrente.quantitaSuImpianti = GetQuantitaSuImpiantiDettaglioTrattamento(attivita, infoOperazione, dettCorrente.prodotto, dettCorrente.unitaDiMisuraIndicata,
                                                                                            "", Nothing, MovimentoDettaglioOperazione)

                dettCorrente.ripartizioneTrappole = MovimentoOperazione.Mezzo
            End If

        End If

        Return dettCorrente

    End Function

    Private Function CreaDettagliRilievo(attivita As Attivita, infoOperazione As InfoOperazione, agenda As Operazione_Agenda, MovimentoDettaglioOperazione As Movimento_Dettaglio) As List(Of dettagli.DettaglioRilievo)

        Dim dettagliRilievi As New List(Of dettagli.DettaglioRilievo)

        If MovimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici IsNot Nothing AndAlso MovimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici.Any() Then
            For Each Movimento_Dettaglio_Tecnico In MovimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici

                If MovimentoDettaglioOperazione.Movimenti_Destinazioni IsNot Nothing AndAlso MovimentoDettaglioOperazione.Movimenti_Destinazioni.Any() Then
                    For Each Movimento_Dettaglio_Destinazione In MovimentoDettaglioOperazione.Movimenti_Destinazioni

                        Dim dettCorrente = New dettagli.DettaglioRilievo
                        dettCorrente.unitaDiMisura = New UnitaDiMisura(Movimento_Dettaglio_Tecnico.dett_cod)
                        dettCorrente.QtaRilevata = Movimento_Dettaglio_Destinazione.Qta
                        dettCorrente.QtaRilevataString = Movimento_Dettaglio_Destinazione.Qta.ToString
                        dettCorrente.esercizioCDC = CreaEsercizioRilievoCdC(attivita, infoOperazione, Movimento_Dettaglio_Destinazione)

                        Select Case agenda.Lav_Cod
                            Case LAVCOD_RILIEVO_AVVERSITA_CAMPO
                                dettCorrente.avversitaGruppo = GetAvversitaGruppo(Movimento_Dettaglio_Tecnico.Av_Cod, Movimento_Dettaglio_Tecnico.Av_Gru)

                            Case LAVCOD_RILIEVO_INDICI_MATURITA
                                dettCorrente.IndiceMaturita = Movimento_Dettaglio_Tecnico.ff_classe
                                dettCorrente.DataOraRilievo = Movimento_Dettaglio_Destinazione.Data
                                dettCorrente.Note = Movimento_Dettaglio_Destinazione.Extra_Str

                            Case LAVCOD_DANNI_RACCOLTA
                                dettCorrente.DannoRaccolta = Movimento_Dettaglio_Tecnico.ff_classe

                            Case LAVCOD_FASI_FENOLOGICHE
                                dettCorrente.faseFenologica = New FaseFenologica(Movimento_Dettaglio_Tecnico.ff_classe)
                                dettCorrente.DataOraRilievo = Movimento_Dettaglio_Destinazione.Data

                            Case LAVCOD_RILIEVO_ERBE_INFESTANTI
                                dettCorrente.erbaInfestante = GetAvversitaGruppo(Movimento_Dettaglio_Tecnico.Av_Cod, Movimento_Dettaglio_Tecnico.Av_Gru)

                            Case LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA
                                dettCorrente.IndiceResa = Movimento_Dettaglio_Tecnico.ff_classe

                            Case LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE
                                dettCorrente.avversitaGruppo = GetAvversitaGruppo(Movimento_Dettaglio_Tecnico.Av_Cod, Movimento_Dettaglio_Tecnico.Av_Gru)
                                dettCorrente.risorsaProdotto = New RisorsaProdotto() With {
                                    .prodotto = New Prodotto(MovimentoDettaglioOperazione.Pro_Cod, MovimentoDettaglioOperazione.Elem_Cod)
                                    }

                        End Select

                        dettagliRilievi.Add(dettCorrente)

                    Next
                End If

            Next
        End If

        Return dettagliRilievi

    End Function


    Private Function CreaDettagliIrrigazione(attivita As Attivita, doseAcqua As Decimal) As List(Of DettaglioIrrigazione)

        Dim dettagliIrrigazione As New List(Of DettaglioIrrigazione)

        For Each cdc In attivita.centriDiCosto
            If cdc.classType = ClassType.EsercizioCDC Then
                Dim esercizioCDC As EsercizioCDC = CType(cdc, EsercizioCDC)
                Dim dettCorrente = New DettaglioIrrigazione
                dettCorrente.QtaRilevata = doseAcqua
                dettCorrente.Ore = 0
                dettCorrente.Portata = 0
                dettCorrente.Efficienza = 100D
                dettCorrente.Frequenza = 1
                dettCorrente.DataInizio = attivita.inizio
                dettCorrente.DataFine = attivita.inizio
                dettCorrente.tipoIrrigazione = New TipoIrrigazione(0)
                dettCorrente.unitaDiMisura = New UnitaDiMisura(enum_UnitaMisura.METRI3__HA)
                dettCorrente.QtaTotale = doseAcqua * esercizioCDC.superficieTrattata
                dettCorrente.esercizioCDC = esercizioCDC
                dettCorrente.consiglioIrrigazione = New Consiglio_Irrigazione(0, "") With {.qtaAcqua = 0, .dataConsiglio = AGRODATAINIZIO}
                dettCorrente.macchina = Nothing 'New ParcoMacchine With {.codice = 0}
                dettagliIrrigazione.Add(dettCorrente)
            End If
        Next

        Return dettagliIrrigazione

    End Function

    Private Function CreaDettagliIrrigazione(attivita As Attivita, infoOperazione As InfoOperazione, agenda As Operazione_Agenda, MovimentoDettaglioOperazione As Movimento_Dettaglio) As List(Of dettagli.DettaglioIrrigazione)

        Dim dettagliIrrigazione As New List(Of dettagli.DettaglioIrrigazione)

        If MovimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici IsNot Nothing AndAlso MovimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici.Any() Then
            For Each Movimento_Dettaglio_Tecnico In MovimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici

                If MovimentoDettaglioOperazione.Movimenti_Destinazioni IsNot Nothing AndAlso MovimentoDettaglioOperazione.Movimenti_Destinazioni.Any() Then
                    For Each Movimento_Dettaglio_Destinazione In MovimentoDettaglioOperazione.Movimenti_Destinazioni

                        Dim dettCorrente = New dettagli.DettaglioIrrigazione
                        dettCorrente.QtaRilevata = Movimento_Dettaglio_Tecnico.Qta_Ril
                        dettCorrente.Ore = Movimento_Dettaglio_Tecnico.Dose
                        dettCorrente.Portata = Movimento_Dettaglio_Tecnico.Parziale

                        If Movimento_Dettaglio_Tecnico.Efficienza = 0 Then 'irrigazione salvata tramite interfaccia vecchia (IrrigazioneBS.aspx)
                            dettCorrente.Efficienza = 100
                            If Movimento_Dettaglio_Tecnico.Nitrati = 0 Then
                                dettCorrente.Frequenza = 1
                                dettCorrente.DataInizio = agenda.Data
                                dettCorrente.DataFine = agenda.Data
                            Else
                                dettCorrente.Frequenza = Movimento_Dettaglio_Tecnico.Nitrati
                                dettCorrente.DataInizio = Movimento_Dettaglio_Tecnico.Inn1_data
                                dettCorrente.DataFine = Movimento_Dettaglio_Tecnico.Inn2_data
                            End If
                        Else 'irrigazione salvata tramite interfaccia nuova (Angular)
                            dettCorrente.Efficienza = Movimento_Dettaglio_Tecnico.Efficienza
                            dettCorrente.Frequenza = Movimento_Dettaglio_Tecnico.Nitrati
                            dettCorrente.DataInizio = Movimento_Dettaglio_Tecnico.Inn1_data
                            dettCorrente.DataFine = Movimento_Dettaglio_Tecnico.Inn2_data
                        End If

                        dettCorrente.tipoIrrigazione = New TipoIrrigazione(Movimento_Dettaglio_Tecnico.Freatimetro)
                        dettCorrente.unitaDiMisura = New UnitaDiMisura(Movimento_Dettaglio_Tecnico.dett_cod)
                        dettCorrente.QtaTotale = Movimento_Dettaglio_Destinazione.Qta
                        dettCorrente.esercizioCDC = CreaEsercizioCdC(attivita, infoOperazione, Movimento_Dettaglio_Destinazione.Piva, Movimento_Dettaglio_Destinazione.Sa_Cod, Movimento_Dettaglio_Destinazione.Appezza, Movimento_Dettaglio_Destinazione.Id_Destinazione,
                                                                            Movimento_Dettaglio_Destinazione.Progetto_Cod, Movimento_Dettaglio_Destinazione.Qta2,
                                                                            Movimento_Dettaglio_Destinazione.Sup_Riduzione_BufferZone, Movimento_Dettaglio_Destinazione.Perc_Riduzione_Deriva)
                        dettCorrente.consiglioIrrigazione = CreaConsiglio_Irrigazione(Movimento_Dettaglio_Tecnico)
                        If Movimento_Dettaglio_Tecnico.Ditta_cod <> 0 Then
                            dettCorrente.macchina = New ParcoMacchine With {.codice = Movimento_Dettaglio_Tecnico.Ditta_cod}
                        Else
                            dettCorrente.macchina = Nothing
                        End If
                        dettagliIrrigazione.Add(dettCorrente)

                    Next
                End If

            Next
        End If

        Return dettagliIrrigazione

    End Function

    Private Function CreaRegistrazione(agenda As Operazione_Agenda, infoOperazione As InfoOperazione, MovimentoDettaglioOperazione As Movimento_Dettaglio, MovimentoMagazzino As Movimento,
                                       ByVal objParametri_Super_Server As AgronicaCoreParametri, ByVal objParametri_Server As AgronicaCoreParametri, ByVal objParametri_Utenti As AgronicaCoreParametri) As risorse.RisorsaRegistrazione

        Dim dettCorrente = New risorse.RisorsaRegistrazione

        CreaRisorsaRegistrazione(agenda, infoOperazione, dettCorrente, MovimentoDettaglioOperazione, MovimentoMagazzino, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)

        If dettCorrente IsNot Nothing Then
            If MovimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici IsNot Nothing AndAlso MovimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici.Any() Then
                dettCorrente.dettaglioRegistrazione = New DettaglioRegistrazione
                dettCorrente.dettaglioRegistrazione.N = MovimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici(0).N
                dettCorrente.dettaglioRegistrazione.P = MovimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici(0).P
                dettCorrente.dettaglioRegistrazione.K = MovimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici(0).K
            End If
        End If

        Return dettCorrente

    End Function


    Private Sub CreaRisorsaProdotto(agenda As Operazione_Agenda, attivita As Attivita, infoOperazione As InfoOperazione, ByRef Dettaglio As RisorsaProdotto, MovimentoDettaglioOperazione As Movimento_Dettaglio, MovimentoMagazzino As Movimento, superficieTrattataTotale As Decimal, acquaTotale As Decimal,
                                    ByVal objParametri_Super_Server As AgronicaCoreParametri, ByVal objParametri_Server As AgronicaCoreParametri, ByVal objParametri_Utenti As AgronicaCoreParametri)

        Dim ProdottoCod As Integer = GetCodiceProdotto(MovimentoDettaglioOperazione.Pro_Cod, MovimentoDettaglioOperazione.Mat_Cod, infoOperazione)

        With Dettaglio
            .prodotto = New Prodotto(ProdottoCod, MovimentoDettaglioOperazione.Elem_Cod)
            .unitaDiMisura = New UnitaDiMisura(MovimentoDettaglioOperazione.Udm_Cod)
        End With

        If infoOperazione.IsRaccolta Then
            If MovimentoDettaglioOperazione.Movimenti_Destinazioni IsNot Nothing AndAlso MovimentoDettaglioOperazione.Movimenti_Destinazioni.Any() Then

                Dim quantitaSuImpiantiList As New List(Of QuantitaSuImpianto)

                For Each Movimento_Dettaglio_Destinazione In MovimentoDettaglioOperazione.Movimenti_Destinazioni

                    Dim quantitaSuImpianto As New QuantitaSuImpianto
                    quantitaSuImpianto.Qta = Movimento_Dettaglio_Destinazione.Qta
                    quantitaSuImpianto.esercizioCDC = CreaEsercizioCdC(attivita, infoOperazione, Movimento_Dettaglio_Destinazione.Piva, Movimento_Dettaglio_Destinazione.Sa_Cod, Movimento_Dettaglio_Destinazione.Appezza, Movimento_Dettaglio_Destinazione.Id_Destinazione,
                                                                            Movimento_Dettaglio_Destinazione.Progetto_Cod, Movimento_Dettaglio_Destinazione.Qta2,
                                                                            Movimento_Dettaglio_Destinazione.Sup_Riduzione_BufferZone, Movimento_Dettaglio_Destinazione.Perc_Riduzione_Deriva)
                    'I campi Magazzino e Lotto vengono inizializzati assieme ai rilevamenti di magazzino
                    'Intanto metto indefinito come valore default
                    quantitaSuImpianto.Lotto = ""

                    quantitaSuImpiantiList.Add(quantitaSuImpianto)

                Next

                Dim dettaglioConQtaManuali As DettaglioRaccolta = CType(Dettaglio, dettagli.DettaglioRaccolta)
                dettaglioConQtaManuali.QuantitaSuImpianti = quantitaSuImpiantiList

                dettaglioConQtaManuali.calCod = MovimentoDettaglioOperazione.Cal_Cod

            End If
        Else
            With Dettaglio
                .doseHaReale = MovimentoDettaglioOperazione.Qta
                .doseHlReale = MovimentoDettaglioOperazione.Qta_Extra
                .quantitaTotaleReale = MovimentoDettaglioOperazione.Qta_Extra_Totale
                .flagTipoDose = MovimentoDettaglioOperazione.Mezzo_Det
                .flagDoseQuantitaTotale = If(MovimentoDettaglioOperazione.Udm_Cod_Extra <> 0, MovimentoDettaglioOperazione.Udm_Cod_Extra, 10) 'DT: se non è applicabile, viene valorizzata con 10 (qta totale) (es: semina)
                .unitaDiMisuraIndicata = If(MovimentoDettaglioOperazione.Extra_Int <> 0, New UnitaDiMisura(MovimentoDettaglioOperazione.Extra_Int), New UnitaDiMisura(MovimentoDettaglioOperazione.Udm_Cod)) 'DT: se non è applicabile l'udm indicata, viene valorizzata come l'udm (es: semina)
            End With

            If infoOperazione.IsSemina Then
                Dettaglio.doseHaReale = If(superficieTrattataTotale <> 0, MovimentoDettaglioOperazione.Qta / superficieTrattataTotale, 0)
                Dettaglio.quantitaTotaleReale = MovimentoDettaglioOperazione.Qta

                If ProdottoCod = 0 Then
                    Dettaglio = Nothing
                End If
            End If

            If attivita.job.primaryKey.codice = LAVCOD_DISTRIBUZIONE_INSETTI AndAlso MovimentoDettaglioOperazione.Qta_Extra_Totale = 0 Then
                Dettaglio.quantitaTotaleReale = CInt(MovimentoDettaglioOperazione.Qta * superficieTrattataTotale)
            End If

        End If

        CreaRilevamentiMagazzino(Dettaglio, Nothing,
                                  infoOperazione, MovimentoMagazzino, superficieTrattataTotale, acquaTotale,
                                  agenda.Piva, agenda.Sa_Cod, agenda.Id_Agenda, MovimentoDettaglioOperazione.Pro_Cod, MovimentoDettaglioOperazione.Mat_Cod, MovimentoDettaglioOperazione.Elem_Cod,
                                  MovimentoDettaglioOperazione.Lotto, MovimentoDettaglioOperazione.Qta,
                                  agenda.Agenda_Riferimenti, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)

    End Sub

    Private Sub CreaRisorsaRegistrazione(agenda As Operazione_Agenda, infoOperazione As InfoOperazione, ByRef Dettaglio As RisorsaRegistrazione, MovimentoDettaglioOperazione As Movimento_Dettaglio, MovimentoMagazzino As Movimento,
                                         ByVal objParametri_Super_Server As AgronicaCoreParametri, ByVal objParametri_Server As AgronicaCoreParametri, ByVal objParametri_Utenti As AgronicaCoreParametri)

        Dim ProdottoCod As Integer = GetCodiceProdotto(MovimentoDettaglioOperazione.Pro_Cod, MovimentoDettaglioOperazione.Mat_Cod, infoOperazione)

        If ProdottoCod <> 0 Then

            With Dettaglio
                .prodotto = New Prodotto(ProdottoCod, MovimentoDettaglioOperazione.Elem_Cod)
                .qta = MovimentoDettaglioOperazione.Qta
                .unitaDiMisura = New UnitaDiMisura(MovimentoDettaglioOperazione.Udm_Cod)
                .causale = New Causale(MovimentoDettaglioOperazione.Pendente)
            End With

            CreaRilevamentiMagazzino(Dettaglio, Nothing,
                                      infoOperazione, MovimentoMagazzino, 0, 0,
                                      agenda.Piva, agenda.Sa_Cod, agenda.Id_Agenda, MovimentoDettaglioOperazione.Pro_Cod, MovimentoDettaglioOperazione.Mat_Cod, MovimentoDettaglioOperazione.Elem_Cod,
                                      MovimentoDettaglioOperazione.Lotto, MovimentoDettaglioOperazione.Qta,
                                      agenda.Agenda_Riferimenti, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)

        Else
            Dettaglio = Nothing
        End If

    End Sub

    ''' <summary>
    ''' Crea i rilevamenti di magazzino associati ad una operazione di magazzino
    ''' Nel caso dell'operazione INSTALLAZIONE TRAPPOLE CATTURA DI MASSA allora potrei avere dei rilevamenti di magazzino associate all'avversità
    ''' </summary>
    ''' <param name="Dettaglio"></param>
    ''' <param name="avversita"></param>
    ''' <param name="infoOperazione"></param>
    ''' <param name="MovimentoMagazzino"></param>
    ''' <param name="superficieTrattataTotale"></param>
    ''' <param name="acquaTotale"></param>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="Id_Agenda"></param>
    ''' <param name="Pro_Cod"></param>
    ''' <param name="Mat_Cod"></param>
    ''' <param name="Elem_Cod"></param>
    ''' <param name="Lotto"></param>
    ''' <param name="Qta"></param>
    ''' <param name="MovimentiRiferimenti"></param>
    ''' <param name="objParametri_Super_Server"></param>
    ''' <param name="objParametri_Server"></param>
    ''' <param name="objParametri_Utenti"></param>

    Public Sub CreaRilevamentiMagazzino(ByRef Dettaglio As Risorsa, ByRef avversita As avversita.AvversitaGruppo, infoOperazione As InfoOperazione, MovimentoMagazzino As Movimento, superficieTrattataTotale As Decimal, acquaTotale As Decimal,
                                         ByVal Piva As String, ByVal Sa_Cod As Integer, ByVal Id_Agenda As Integer, ByVal Pro_Cod As Integer, ByVal Mat_Cod As Integer,
                                         ByVal Elem_Cod As Integer, ByVal Lotto As String, ByVal Qta As Decimal, ByVal MovimentiRiferimenti As List(Of Movimento_Dettaglio_Riferimento),
                                         ByVal objParametri_Super_Server As AgronicaCoreParametri, ByVal objParametri_Server As AgronicaCoreParametri, ByVal objParametri_Utenti As AgronicaCoreParametri)

        If MovimentoMagazzino IsNot Nothing AndAlso MovimentoMagazzino.Movimenti_Dettagli IsNot Nothing AndAlso MovimentoMagazzino.Movimenti_Dettagli.Any() Then

            ' Tengo contto della quantità totale raccolta per ogni impianto
            Dim QsI As New List(Of QuantitaSuImpianto)

            Dim List_Agenda_Carico_Scarico As New List(Of Operazione_Agenda)

            Dim movimentiDettaglioMagazzino As List(Of Movimento_Dettaglio)

            If infoOperazione.IsCarico OrElse infoOperazione.IsScarico Then

                movimentiDettaglioMagazzino = (From d In MovimentoMagazzino.Movimenti_Dettagli
                                               Where Utility.Controlla_Se_Stesso_Prodotto(d.Elem_Cod, d.Pro_Cod, d.Mat_Cod, d.Lotto, 0,
                                                                                              Elem_Cod, Pro_Cod,
                                                                                              Mat_Cod, Lotto, 0) AndAlso
                                                       d.Qta = Qta And
                                                   d.Lotto = Lotto
                                               Select d).ToList
            Else
                movimentiDettaglioMagazzino = (From d In MovimentoMagazzino.Movimenti_Dettagli
                                               Where Utility.Controlla_Se_Stesso_Prodotto(d.Elem_Cod, d.Pro_Cod, d.Mat_Cod, d.Lotto, 0,
                                                                                              Elem_Cod, Pro_Cod,
                                                                                               Mat_Cod, Lotto, 0)
                                               Select d).ToList
            End If

            If Not IsNothing(MovimentiRiferimenti) AndAlso MovimentiRiferimenti.Any() Then

                Dim MovimentiRiferimento_Carico_Scarico = (From m_r In MovimentiRiferimenti
                                                           Where m_r.Piva = Piva AndAlso
                                                            m_r.Sa_Cod = Sa_Cod AndAlso
                                                            m_r.Id_Agenda = Id_Agenda AndAlso
                                                            m_r.Id_Mov = -1 AndAlso
                                                            m_r.Id_Mov_Det = -1 AndAlso
                                                            m_r.Id_Mov_Rif = -1 AndAlso
                                                            m_r.Id_Mov_Det_Rif = -1 AndAlso
                                                            Utility.Carico_Scarico_Collegate_Al_QdC(m_r)
                                                           Select m_r)

                If Not IsNothing(MovimentiRiferimento_Carico_Scarico) AndAlso MovimentiRiferimento_Carico_Scarico.Any() Then

                    Dim objAgenda As New Agenda_Operazione_Helper

                    For Each MovimentoRiferimento_Carico_Scarico As Movimento_Dettaglio_Riferimento In MovimentiRiferimento_Carico_Scarico

                        Dim agenda_Carico_Scarico As Operazione_Agenda = objAgenda.Leggi(MovimentoRiferimento_Carico_Scarico.Piva_Rif, 0, MovimentoRiferimento_Carico_Scarico.Id_Agenda_Rif, 0, objParametri_Server)

                        If Not IsNothing(agenda_Carico_Scarico) AndAlso agenda_Carico_Scarico.Id_Agenda > 0 Then
                            List_Agenda_Carico_Scarico.Add(agenda_Carico_Scarico)
                        End If
                    Next
                End If
            End If

            Dim List_Magazzini_Scarico As New List(Of Tuple(Of Fabbricato, Decimal, List(Of Attivita)))

            For Each movimentoDettaglioMagazzino In movimentiDettaglioMagazzino

                If movimentoDettaglioMagazzino IsNot Nothing AndAlso movimentoDettaglioMagazzino.Movimenti_Destinazioni.Any() Then

                    Dim xDestinazione = movimentoDettaglioMagazzino.Movimenti_Destinazioni(0)

                    Dim qtaTrasformata As Decimal = 0

                    Dim dettaglioRisorsa = Nothing

                    If Not IsNothing(Dettaglio) AndAlso IsNothing(avversita) Then

                        If Dettaglio.GetType Is GetType(RisorsaRegistrazione) Then
                            dettaglioRisorsa = CType(Dettaglio, RisorsaRegistrazione)
                        Else
                            dettaglioRisorsa = CType(Dettaglio, RisorsaProdotto)
                            If infoOperazione.IsRaccolta Then
                                qtaTrasformata = movimentoDettaglioMagazzino.Qta
                                If (QsI.Count = 0) Then
                                    QsI = dettaglioRisorsa.QuantitaSuImpianti
                                    dettaglioRisorsa.QuantitaSuImpianti = New List(Of QuantitaSuImpianto)
                                End If
                            Else
                                If infoOperazione.IsSemina Then
                                    qtaTrasformata = movimentoDettaglioMagazzino.Qta
                                Else
                                    qtaTrasformata = GetDosePerMagazzino(movimentoDettaglioMagazzino.Qta, dettaglioRisorsa.unitaDiMisuraIndicata.codice)
                                End If
                            End If

                        End If

                    End If

                    If Not IsNothing(avversita) AndAlso IsNothing(Dettaglio) Then

                        Dim Udm_Cod_Indicata As Integer = 0

                        If movimentoDettaglioMagazzino.Extra_Int = 0 AndAlso movimentoDettaglioMagazzino.Udm_Cod > 0 Then
                            Udm_Cod_Indicata = movimentoDettaglioMagazzino.Udm_Cod
                        ElseIf movimentoDettaglioMagazzino.Extra_Int > 0 Then
                            Udm_Cod_Indicata = movimentoDettaglioMagazzino.Extra_Int
                        End If

                        qtaTrasformata = GetDosePerMagazzino(movimentoDettaglioMagazzino.Qta, Udm_Cod_Indicata)
                    End If

                    Dim ListAttivita_Carico_Scarico_x_Prodotto_Lotto As List(Of Attivita) = GetListAttivita_Carico_Scarico_x_Prodotto_Lotto(movimentoDettaglioMagazzino, xDestinazione, List_Agenda_Carico_Scarico,
                                                                                                                                            objParametri_Super_Server, objParametri_Server, objParametri_Utenti)

                    If xDestinazione.Id_Destinazione <> 0 Then

                        Dim ProdottoCod As Integer = GetCodiceProdotto(Pro_Cod, Mat_Cod, infoOperazione)

                        Dim rilevamentoMagazzino As New RilevamentoDiMagazzino With {
                                    .Lotto = movimentoDettaglioMagazzino.Lotto,
                                    .Magazzino = CreaFabbricato(xDestinazione.Piva, xDestinazione.Sa_Cod, xDestinazione.Id_Destinazione),
                                    .udm = New UnitaDiMisura(movimentoDettaglioMagazzino.Udm_Cod),
                                    .Qta = movimentoDettaglioMagazzino.Qta,
                                    .Prodotto = New Prodotto(ProdottoCod, movimentoDettaglioMagazzino.Elem_Cod) With {
                                        .unitaDiMisura = New UnitaDiMisura(movimentoDettaglioMagazzino.Udm_Cod)
                                    },
                                    .TipoRilevamento = RilevamentoDiMagazzino.RilevamentoMagazzinoTipo.giacenza,
                                    .doseHaIndicata = If(superficieTrattataTotale <> 0, qtaTrasformata / superficieTrattataTotale, 0),
                                    .doseHlIndicata = If(acquaTotale <> 0, qtaTrasformata / acquaTotale, 0),
                                    .Cal_Cod = movimentoDettaglioMagazzino.Cal_Cod,
                                    .Cod_Progetto = movimentoDettaglioMagazzino.Cod_Progetto
                                }

                        rilevamentoMagazzino.registrazioniCollegate = New List(Of Attivita)

                        If ListAttivita_Carico_Scarico_x_Prodotto_Lotto.Count > 0 Then
                            rilevamentoMagazzino.registrazioniCollegate.AddRange(ListAttivita_Carico_Scarico_x_Prodotto_Lotto)
                        End If

                        If Not IsNothing(dettaglioRisorsa) AndAlso IsNothing(avversita) Then

                            If dettaglioRisorsa.MagazziniMovimentazioni Is Nothing Then
                                dettaglioRisorsa.MagazziniMovimentazioni = New List(Of RilevamentoDiMagazzino)
                            End If

                            If infoOperazione.IsRaccolta Then
                                For Each q In QsI
                                    Dim quantitaSuImpianto = q.Clone()
                                    quantitaSuImpianto.Magazzino = rilevamentoMagazzino.Magazzino
                                    quantitaSuImpianto.Lotto = rilevamentoMagazzino.Lotto

                                    If movimentoDettaglioMagazzino.Cod_Progetto = 0 Then
                                        ' Aggiungo un record di QuantitaSuImpianto con quantità proporzionata a quella nel Carico di magazzino
                                        quantitaSuImpianto.Qta = If(Qta <> 0,
                                        quantitaSuImpianto.Qta * movimentoDettaglioMagazzino.Qta / Qta,
                                        0
                                    )
                                    ElseIf movimentoDettaglioMagazzino.Cod_Progetto = q.esercizioCDC.esercizio.codice Then
                                        ' Aggiungo il rilevamento è collegato specificatamente a questo esercizio
                                        quantitaSuImpianto.Qta = movimentoDettaglioMagazzino.Qta
                                    Else
                                        Continue For
                                    End If

                                    dettaglioRisorsa.QuantitaSuImpianti.Add(quantitaSuImpianto)
                                Next

                                If rilevamentoMagazzino.Cod_Progetto <> 0 Then
                                    dettaglioRisorsa.Opzioni_Raccolta.GenerazioneLotto = enum_Generazione_Lotto_Raccolta.DA_ESERCIZO
                                End If
                            End If

                            dettaglioRisorsa.MagazziniMovimentazioni.Add(rilevamentoMagazzino)

                        End If

                        If Not IsNothing(avversita) AndAlso IsNothing(dettaglioRisorsa) Then

                            If avversita.MagazziniMovimentazioni Is Nothing Then
                                avversita.MagazziniMovimentazioni = New List(Of RilevamentoDiMagazzino)
                            End If

                            avversita.MagazziniMovimentazioni.Add(rilevamentoMagazzino)

                        End If

                    End If

                End If
            Next

        End If
    End Sub

    Public Function CreaFabbricato(Piva As String, Sa_Cod As Integer, Id_Destinazione As Integer) As Fabbricato

        Dim fabbricato As Fabbricato = New Fabbricato With {
            .primaryKey = New Fabbricato.PK With {
                .centroAziendalePK = CreaCentroAziendale(Piva, Sa_Cod).primaryKey,
                .codice = Id_Destinazione
            }
        }

        Return fabbricato

    End Function

    Private Function GetListAttivita_Carico_Scarico_x_Prodotto_Lotto(ByVal movimentoDettaglioMagazzino As Movimento_Dettaglio, ByVal movimentoDestinazioneMagazzino As Movimento_Destinazione, ByVal List_Agenda_Carico_Scarico As List(Of Operazione_Agenda),
                                                                     ByVal objParametri_Super_Server As AgronicaCoreParametri, ByVal objParametri_Server As AgronicaCoreParametri, ByVal objParametri_Utenti As AgronicaCoreParametri) As List(Of Attivita)

        Dim ListAttivita_Carico_Scarico_x_Prodotto_Lotto As New List(Of Attivita)

        If Not IsNothing(List_Agenda_Carico_Scarico) AndAlso List_Agenda_Carico_Scarico.Count > 0 Then

            Dim objStr As String = JsonConvert.SerializeObject(List_Agenda_Carico_Scarico)

            Dim List_Agenda_Carico_Scarico_Copy As List(Of Operazione_Agenda) = JsonConvert.DeserializeObject(Of List(Of Operazione_Agenda))(objStr)

            For Each Agenda_Carico_Scarico As Operazione_Agenda In List_Agenda_Carico_Scarico_Copy

                If Not IsNothing(Agenda_Carico_Scarico) AndAlso Agenda_Carico_Scarico.Id_Agenda > 0 Then

                    If Not IsNothing(Agenda_Carico_Scarico.Movimenti) AndAlso Agenda_Carico_Scarico.Movimenti.Count > 0 Then

                        Dim movimento_Carico_Scarico = (From m In Agenda_Carico_Scarico.Movimenti
                                                        Where m.Cau_Mov = CAU_CARICO Or m.Cau_Mov = CAU_SCARICO
                                                        Select m)

                        If Not IsNothing(movimento_Carico_Scarico) AndAlso movimento_Carico_Scarico.Count = 1 AndAlso
                            Not IsNothing(movimento_Carico_Scarico(0).Movimenti_Dettagli) AndAlso
                            movimento_Carico_Scarico(0).Movimenti_Dettagli.Any() Then

                            Dim Mov_Destinazione_Carico As New List(Of Movimento_Destinazione)

                            Dim Mov_Destinazione_Scarico As New List(Of Movimento_Destinazione)

                            'In un unico Scarico ci possono essere più righe con lo stesso proodtto ma Magazzino diverso.
                            'In questo caso devo creare altri oggetti di Scarico dedicati per ogni Magazzino.

                            Dim List_Key As New List(Of Tuple(Of String, Integer, Integer, Integer, Integer))

                            For Each movimento_dettaglio_Carico_Scarico As Movimento_Dettaglio In movimento_Carico_Scarico(0).Movimenti_Dettagli

                                If movimentoDettaglioMagazzino.Qta = movimento_dettaglio_Carico_Scarico.Qta Then
                                    If movimento_Carico_Scarico(0).Cau_Mov = CAU_CARICO Then

                                        Mov_Destinazione_Carico.AddRange((From m_d In movimento_dettaglio_Carico_Scarico.Movimenti_Destinazioni
                                                                          Where m_d.Piva = movimentoDestinazioneMagazzino.Piva AndAlso
                                                                                       m_d.Sa_Cod = movimentoDestinazioneMagazzino.Sa_Cod AndAlso
                                                                                       m_d.Id_Destinazione = movimentoDestinazioneMagazzino.Id_Destinazione
                                                                          Select m_d).ToList())

                                    ElseIf movimento_Carico_Scarico(0).Cau_Mov = CAU_SCARICO Then

                                        Mov_Destinazione_Scarico.AddRange(movimento_dettaglio_Carico_Scarico.Movimenti_Destinazioni)

                                    End If

                                    List_Key.Add(Tuple.Create(movimento_dettaglio_Carico_Scarico.Piva, movimento_dettaglio_Carico_Scarico.Sa_Cod, movimento_dettaglio_Carico_Scarico.Id_Agenda, movimento_dettaglio_Carico_Scarico.Id_Mov, movimento_dettaglio_Carico_Scarico.Id_Mov_Det))
                                End If

                            Next

                            If Not IsNothing(List_Key) AndAlso List_Key.Any() Then
                                Agenda_Carico_Scarico.Movimenti = (From mov In Agenda_Carico_Scarico.Movimenti _
                                                                       .Where(Function(m) m.Sa_Cod = 0)
                                                                   Join key In List_Key
                                                                   On key.Item1 Equals mov.Piva _
                                                                   And key.Item3 Equals mov.Id_Agenda _
                                                                   And key.Item4 Equals mov.Id_Mov
                                                                   Select mov).ToList()

                                For Each mov In Agenda_Carico_Scarico.Movimenti
                                    mov.Movimenti_Dettagli = (From mov_det In mov.Movimenti_Dettagli
                                                              Join key In List_Key
                                                              On key.Item1 Equals mov_det.Piva _
                                                              And key.Item2 Equals mov_det.Sa_Cod _
                                                              And key.Item3 Equals mov_det.Id_Agenda _
                                                              And key.Item4 Equals mov_det.Id_Mov _
                                                              And key.Item5 Equals mov_det.Id_Mov_Det
                                                              Select mov_det).ToList()
                                Next

                            End If

                            If Not IsNothing(Mov_Destinazione_Carico) AndAlso Mov_Destinazione_Carico.Any() Then

                                Dim movimenti_dettaglio_Carico_x_Prodotto_Lotto = (From m_d In movimento_Carico_Scarico(0).Movimenti_Dettagli
                                                                                   Join Mov_Dest In Mov_Destinazione_Carico
                                                                                               On Mov_Dest.Piva Equals m_d.Piva And
                                                                                                  Mov_Dest.Sa_Cod Equals m_d.Sa_Cod And
                                                                                                  Mov_Dest.Id_Agenda Equals m_d.Id_Agenda And
                                                                                                  Mov_Dest.Id_Mov Equals m_d.Id_Mov And
                                                                                                  Mov_Dest.Id_Mov_Det Equals m_d.Id_Mov_Det
                                                                                   Where Utility.Controlla_Se_Stesso_Prodotto(m_d.Elem_Cod, m_d.Pro_Cod, m_d.Mat_Cod,
                                                                                                                                                "", 0,
                                                                                                                                                movimentoDettaglioMagazzino.Elem_Cod, movimentoDettaglioMagazzino.Pro_Cod,
                                                                                                                                                movimentoDettaglioMagazzino.Mat_Cod, "", 0) And
                                                                                                  m_d.Lotto.ToUpper() = movimentoDettaglioMagazzino.Lotto.ToUpper()
                                                                                   Select m_d)



                                If Not IsNothing(movimenti_dettaglio_Carico_x_Prodotto_Lotto) AndAlso movimenti_dettaglio_Carico_x_Prodotto_Lotto.Any() Then

                                    For Each movimento_dettaglio_Carico_Scarico_x_Prodotto_Lotto In movimenti_dettaglio_Carico_x_Prodotto_Lotto

                                        Dim Attivita_Carico As Attivita = AgendaSuAttivita(Agenda_Carico_Scarico, True, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)

                                        ListAttivita_Carico_Scarico_x_Prodotto_Lotto.Add(Attivita_Carico)
                                    Next

                                End If
                            End If

                            If Not IsNothing(Mov_Destinazione_Scarico) AndAlso Mov_Destinazione_Scarico.Any() Then
                                Dim movimenti_dettaglio_Scarico_x_Prodotto_Lotto = (From m_d In movimento_Carico_Scarico(0).Movimenti_Dettagli
                                                                                    Join Mov_Dest In Mov_Destinazione_Scarico
                                                                                               On Mov_Dest.Piva Equals m_d.Piva And
                                                                                                  Mov_Dest.Sa_Cod Equals m_d.Sa_Cod And
                                                                                                  Mov_Dest.Id_Agenda Equals m_d.Id_Agenda And
                                                                                                  Mov_Dest.Id_Mov Equals m_d.Id_Mov And
                                                                                                  Mov_Dest.Id_Mov_Det Equals m_d.Id_Mov_Det
                                                                                    Where Utility.Controlla_Se_Stesso_Prodotto(m_d.Elem_Cod, m_d.Pro_Cod, m_d.Mat_Cod,
                                                                                                                                                "", 0,
                                                                                                                                                movimentoDettaglioMagazzino.Elem_Cod, movimentoDettaglioMagazzino.Pro_Cod,
                                                                                                                                                movimentoDettaglioMagazzino.Mat_Cod, "", 0) And
                                                                                                  m_d.Lotto.ToUpper() = movimentoDettaglioMagazzino.Lotto.ToUpper()
                                                                                    Select m_d)

                                If Not IsNothing(movimenti_dettaglio_Scarico_x_Prodotto_Lotto) AndAlso movimenti_dettaglio_Scarico_x_Prodotto_Lotto.Any() Then

                                    For Each movimento_dettaglio_Scarico_x_Prodotto_Lotto In movimenti_dettaglio_Scarico_x_Prodotto_Lotto

                                        Dim Attivita_Scarico As Attivita = AgendaSuAttivita(Agenda_Carico_Scarico, True, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)

                                        ListAttivita_Carico_Scarico_x_Prodotto_Lotto.Add(Attivita_Scarico)
                                    Next

                                End If


                            End If

                        End If

                    End If
                End If
            Next

        End If

        Return ListAttivita_Carico_Scarico_x_Prodotto_Lotto

    End Function

    Private Function CreaConsiglio_Irrigazione(ByVal Movimento_Dettaglio_Tecnico As Movimento_Dettaglio_Tecnico) As Consiglio_Irrigazione

        'Se è stato scelto un DSS irrigazione allora il codice e la descrizione saranno valorizzati con quelli (tabella DSS_Irrigazione)
        'Altrimenti valorizzo soltanto la quantità di acqua e la data del consiglio inserite manualemnte nell'operazione di Irrigazione (valori custom)

        Dim consiglioIrrigazione As New Consiglio_Irrigazione(IIf(IsDBNull(Movimento_Dettaglio_Tecnico.Extra_Int), 0, Movimento_Dettaglio_Tecnico.Extra_Int), "") With {
                            .qtaAcqua = If(IsDBNull(Movimento_Dettaglio_Tecnico.ExtraStr) OrElse Not IsNumeric(Movimento_Dettaglio_Tecnico.ExtraStr), 0, CDec(Movimento_Dettaglio_Tecnico.ExtraStr)),
                            .dataConsiglio = IIf(IsDBNull(Movimento_Dettaglio_Tecnico.Extra_Date), AGRODATAINIZIO, Movimento_Dettaglio_Tecnico.Extra_Date)
                            }

        Return consiglioIrrigazione

    End Function

    Private Function GetQuantitaSuImpiantiDettaglioTrattamento(ByVal attivita As Attivita, ByVal infoOperazione As InfoOperazione,
                                                               ByVal prodotto As Prodotto, ByVal unitaDiMisuraIndicata As UnitaDiMisura,
                                                               ByVal lotto As String, ByVal magazzino As Fabbricato, ByVal MovimentoDettaglioOperazione As Movimento_Dettaglio) As List(Of QuantitaSuImpianto)

        Dim quantitaSuImpianti As New List(Of QuantitaSuImpianto)

        If Not IsNothing(MovimentoDettaglioOperazione) Then


            If Not IsNothing(MovimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici) AndAlso MovimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici.Count > 0 Then
                Dim movimenti_destinazioni As List(Of Movimento_Destinazione) = MovimentoDettaglioOperazione.Movimenti_Destinazioni.FindAll(Function(mov) mov.Id_Mov_Det = MovimentoDettaglioOperazione.Id_Mov_Det)

                If Not IsNothing(movimenti_destinazioni) AndAlso movimenti_destinazioni.Count > 0 Then

                    For Each movimento_destinazione In movimenti_destinazioni

                        'La movimento_destinazione.Qta è salvata sempre in kg/litri oppure le altre udm di base
                        'Per questo motivo devo riconvertire la qta con la GetDosePerMagazzino

                        Dim quantitaSuImpianto As New QuantitaSuImpianto With {
                                 .esercizioCDC = CreaEsercizioCdC(attivita, infoOperazione, movimento_destinazione.Piva, movimento_destinazione.Sa_Cod, movimento_destinazione.Appezza, movimento_destinazione.Id_Destinazione,
                                                                                    movimento_destinazione.Progetto_Cod, movimento_destinazione.Qta2,
                                                                                    movimento_destinazione.Sup_Riduzione_BufferZone, movimento_destinazione.Perc_Riduzione_Deriva),
                                 .Prodotto = prodotto,
                                 .Lotto = lotto,
                                 .Magazzino = magazzino,
                                 .Qta = GetDosePerMagazzino(movimento_destinazione.Qta, unitaDiMisuraIndicata.codice)
                            }


                        quantitaSuImpianti.Add(quantitaSuImpianto)

                    Next
                End If

            End If
        End If

        Return quantitaSuImpianti
    End Function

    Private Sub CreaAttivitaCollegataInstallazioneTrappole(agenda As Operazione_Agenda, attivita As Attivita, verbose As Boolean, objParametri_Super_Server As AgronicaCoreParametri, objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri)

        If Not IsNothing(agenda.Movimenti) AndAlso agenda.Movimenti.Count > 0 Then

            For Each movimento In agenda.Movimenti

                If Not IsNothing(movimento.Movimenti_Dettagli) AndAlso movimento.Movimenti_Dettagli.Count > 0 Then

                    For Each movimento_dettaglio In movimento.Movimenti_Dettagli

                        If Not IsNothing(movimento_dettaglio.Movimenti_Dettagli_Riferimenti) AndAlso movimento_dettaglio.Movimenti_Dettagli_Riferimenti.Count > 0 Then

                            Dim List_Distinct_Id_Agenda_Mov_Dettagli_Riferimenti As List(Of Movimento_Dettaglio_Riferimento) = movimento_dettaglio.Movimenti_Dettagli_Riferimenti.Select(Of Movimento_Dettaglio_Riferimento)(Function(mov_dett_rif) New Movimento_Dettaglio_Riferimento With {
                                                                                                                                            .Piva_Rif = mov_dett_rif.Piva_Rif,
                                                                                                                                            .Id_Agenda_Rif = mov_dett_rif.Id_Agenda_Rif,
                                                                                                                                            .Lav_Cod_Rif = mov_dett_rif.Lav_Cod_Rif,
                                                                                                                                            .Cau_Mov_Rif = mov_dett_rif.Cau_Mov_Rif
                                                                                                                                        }).Distinct().ToList()

                            If Not IsNothing(List_Distinct_Id_Agenda_Mov_Dettagli_Riferimenti) AndAlso List_Distinct_Id_Agenda_Mov_Dettagli_Riferimenti.Count > 0 Then

                                Dim objAgenda As New Agenda_Operazione_Helper
                                Dim map As New AgronicaCoreMapper.AgendaToAttivita

                                For Each movimento_dettaglio_rif In List_Distinct_Id_Agenda_Mov_Dettagli_Riferimenti

                                    If movimento_dettaglio_rif.Lav_Cod_Rif = LAVCOD_REINNESCO_TRAPPOLE AndAlso movimento_dettaglio_rif.Cau_Mov_Rif = CAU_TRATTAMENTO Then

                                        Dim agendaRif As Operazione_Agenda = objAgenda.Leggi(movimento_dettaglio_rif.Piva_Rif, 0, movimento_dettaglio_rif.Id_Agenda_Rif, 0, objParametri_Server)

                                        If Not IsNothing(agendaRif) Then

                                            Dim attivitaCollegata = map.AgendaSuAttivita(agendaRif, verbose, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)

                                            If IsNothing(attivita.attivitaCollegate) Then
                                                attivita.attivitaCollegate = New List(Of Attivita)
                                            End If

                                            attivita.attivitaCollegate.Add(attivitaCollegata)

                                        End If

                                    End If

                                Next

                            End If



                        End If

                    Next

                End If

            Next

        End If

    End Sub
#End Region


#Region "Funzioni private di estrazione di parti del vecchio modello"

    Private Function GetPrimoImpianto(agenda As Operazione_Agenda, infoOperazione As InfoOperazione) As Movimento_Destinazione

        Dim primoImpianto As Movimento_Destinazione = Nothing

        Dim MovimentoOperazione As Movimento = GetMovimentoFromCausale(agenda, infoOperazione.Cau_Mov)

        If MovimentoOperazione IsNot Nothing AndAlso MovimentoOperazione.Movimenti_Dettagli IsNot Nothing _
           AndAlso MovimentoOperazione.Movimenti_Dettagli.Any() _
           AndAlso MovimentoOperazione.Movimenti_Dettagli(0).Movimenti_Destinazioni IsNot Nothing _
           AndAlso MovimentoOperazione.Movimenti_Dettagli(0).Movimenti_Destinazioni.Any() Then

            primoImpianto = MovimentoOperazione.Movimenti_Dettagli(0).Movimenti_Destinazioni(0)

        End If

        Return primoImpianto
    End Function

    Private Function GetPrimoProdottoDaTrattare(agenda As Operazione_Agenda, infoOperazione As InfoOperazione) As Movimento_Dettaglio

        Dim primoProdottoDaTrattare As Movimento_Dettaglio = Nothing

        Dim MovimentoOperazione As Movimento = GetMovimentoFromCausale(agenda, infoOperazione.Cau_Mov)

        If MovimentoOperazione IsNot Nothing AndAlso MovimentoOperazione.Movimenti_Dettagli IsNot Nothing AndAlso
            MovimentoOperazione.Movimenti_Dettagli.Any() Then

            For Each movimento_dettaglio In MovimentoOperazione.Movimenti_Dettagli
                If (movimento_dettaglio.Elem_Cod = SEMENTI OrElse movimento_dettaglio.Elem_Cod = TRASFORMATI_VEGETALI) AndAlso
                movimento_dettaglio.Mat_Cod <> 0 Then
                    primoProdottoDaTrattare = movimento_dettaglio
                    Exit For
                End If
            Next
        End If

        Return primoProdottoDaTrattare
    End Function

    Private Function GetMovimentoFromCausale(agenda As Operazione_Agenda, causale As String) As Movimento
        Dim Movimento As Movimento = agenda.Movimenti.FirstOrDefault(Function(c) (c.Cau_Mov = causale))
        Return Movimento
    End Function
#End Region

End Class
