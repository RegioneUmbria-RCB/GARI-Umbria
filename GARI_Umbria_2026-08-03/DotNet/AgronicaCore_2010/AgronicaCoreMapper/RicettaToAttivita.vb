Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreContabDAL
Imports AgronicaCoreModelsSTD.attivita
Imports AgronicaCoreModelsSTD.attivita.Attivita
Imports AgronicaCoreModelsSTD.attivita.centri_di_costo
Imports AgronicaCoreModelsSTD.attivita.risorse
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.metaschema
Imports AgronicaCoreMapper.Utility
Imports AgronicaCoreModelsSTD.attivita.dettagli
Imports AgronicaCoreModelsSTD.costanti
Imports AttivitaPersonalizzata = AgronicaCoreModelsSTD.attivita.AttivitaPersonalizzata
Imports AgronicaCoreModello
Imports AgronicaCoreModello.Utility_Agenda
Imports AgronicaCoreModelsSTD.attivita.dettagli.Opzioni_Raccolta
Imports Newtonsoft.Json
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports InData.Anagrafica
Imports AgronicaCoreModelsSTD.meteo
Imports AgronicaCoreModelsSTD.exceptions

Public Class RicettaToAttivita

    Public Function RicettaOperazioneSuAttivita(ricetta_operazione As Ricette_Operazioni, ByRef listParametriAggiuntivi As List(Of Parametri_Aggiuntivi_Attivita), verbose As Boolean, objParametri_Super_Server As AgronicaCoreParametri, objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri, Optional isRibaltamentoToAgenda As Boolean = False, Optional ByVal leggiDisciplinari As Boolean = True) As AgronicaCoreModelsSTD.attivita.Attivita

        If ricetta_operazione Is Nothing Then
            Return Nothing
        End If

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
        Dim db As New Gias_DeveloperServer_Entities(EFConnString)

        Dim InfoOperazione As InfoOperazione = GetInfoOperazione(ricetta_operazione.Lav_Cod, AgronicaCoreModelsSTD.attivita.Attivita.Tipo_Attivita.Ricetta)

        Dim Piva = ricetta_operazione.Ricetta_SuperUser

        Dim ricetta As Ricette = EFRicette.ReadRicetta(db, Piva, ricetta_operazione.Ricetta_Cod)

        Dim listaDettagli As List(Of Ricette_Dettagli)
        Dim listaDettaglioTecnico As List(Of Ricette_Dettaglio_Tecnico)
        Dim listaDestinazioni As List(Of Ricette_Destinazioni)

        Dim epoca As Epoca = Nothing
        Dim tipoRaccolta As Tipo_Raccolta = 0
        Dim codiceAttivitaPersonalizzata As Integer = 0
        If ricetta_operazione.Extra_Int IsNot Nothing AndAlso ricetta_operazione.Extra_Int <> 0 Then

            If InfoOperazione.IsRaccolta Then
                tipoRaccolta = ricetta_operazione.Extra_Int
            Else
                If InfoOperazione.IsLavorazione Then
                    codiceAttivitaPersonalizzata = ricetta_operazione.Extra_Int 'DT: se si arriva da APP, l'id_attivita è qui
                Else
                    epoca = New Epoca(ricetta_operazione.Extra_Int)
                End If
            End If
        End If

        Dim attivita As New AgronicaCoreModelsSTD.attivita.Attivita With {
            .tipo = Tipo_Attivita.Ricetta,
            .codice = ricetta_operazione.Ricetta_Operazione_Cod,
            .descrizione = ricetta_operazione.Ricetta_Operazione_Des,
            .inizio = ricetta_operazione.Validita_Inizio,
            .oraInizio = IIf(ricetta_operazione.Ora IsNot Nothing, ricetta_operazione.Ora, ricetta_operazione.Validita_Inizio),
            .fine = ricetta_operazione.Validita_Fine,
            .note = ricetta_operazione.Note,
            .stato = CType(ricetta_operazione.W_Anagrafica_Stati_Cod, Stati),
            .epoca = epoca,
            .tipoRaccolta = tipoRaccolta,
            .tipoRicetta = ricetta.Tipo_Ricetta,
            .raccoglitore = ricetta_operazione.Raccoglitore_Cod,
            .inviaRicetta = ricetta_operazione.Invia_App,
            .origine = ricetta.Origine,
            .appRicettaOperazioneID = ricetta_operazione.APP_Ricetta_Operazione_ID
            }

        attivita.job = New AgronicaCoreModelsSTD.attivita.Lavorazione(ricetta_operazione.Lav_Cod)

        attivita.testataRicetta = GetTestataRicetta(attivita, ricetta.Ricetta_Cod, ricetta.Ricetta_Des, ricetta.Ricetta_Des_Long,
                                                    ricetta.Ricetta_Numero, ricetta.Note, ricetta.Validita_Inizio,
                                                    ricetta.Validita_Fine, ricetta_operazione.Lav_Cod, ricetta.Programmazione_Cod, ricetta.Piva, verbose, objParametri_Server, objParametri_Utenti, objParametri_Super_Server)

        attivita.associazionePK = LoadAssociazionePK(attivita, objParametri_Server)

        listaDettagli = EFRicette.ReadRicettaDettagli(db, Piva, ricetta_operazione.Ricetta_Cod, ricetta_operazione.Ricetta_Operazione_Cod)

        Dim causali = New List(Of String)() From {
            CAU_IMPUTAZIONE_PARCOMACCHINE,
            CAU_IMPUTAZIONE_MANODOPERA,
            CAU_IMPUTAZIONE_TECNICO_RESPONSABILE,
            CAU_IMPUTAZIONE_TERZISTI,
            CAU_SCARICO,
            CAU_CARICO
        }
        Dim listaDettagliCampagna As List(Of Ricette_Dettagli) = (From d In listaDettagli Where Not causali.Contains(d.Cau_Mov.ToString) Select d).ToList
        listaDestinazioni = EFRicette.ReadRicettaDestinazioni(db, Piva, ricetta_operazione.Ricetta_Cod, ricetta_operazione.Ricetta_Operazione_Cod, 0)

        Dim pivaRicettaOperazione As String = ricetta.Piva
        Dim saCodRicettaOperazione As Integer = ricetta.Sa_Cod
        If listaDestinazioni IsNot Nothing AndAlso listaDestinazioni.Count > 0 Then

            Dim destinazione
            Select Case InfoOperazione.TipoCentroDiCosto
                Case centri_di_costo.Tipo.Esercizio
                    destinazione = listaDestinazioni.Where(Function(d) d.Tipo_Destinazione = CostantiPersonalizzate.TIPO_DESTINAZIONE_IMPIANTO).FirstOrDefault
                Case centri_di_costo.Tipo.ProdottoDaTrattare
                    destinazione = listaDestinazioni.Where(Function(d) d.Tipo_Destinazione = CostantiPersonalizzate.MAGAZZINO AndAlso d.Qta = 0).FirstOrDefault
            End Select

            If Not IsNothing(destinazione) Then
                pivaRicettaOperazione = destinazione.Piva
                saCodRicettaOperazione = destinazione.Sa_Cod
            End If
        End If
        attivita.centroAziendale = LeggiCentroAziendale(pivaRicettaOperazione, saCodRicettaOperazione)

        Dim superficieTrattataTotale As Decimal = 0
        Dim listaImpianti = New Dictionary(Of String, Tuple(Of EsercizioCDC, List(Of Ricette_Destinazioni)))
        Dim listaGiacenzeMagazzino = New Dictionary(Of String, Tuple(Of ProdottoDaTrattareCDC, List(Of Ricette_Destinazioni)))
        Dim creaDettagliIrrigazioneDefault As Boolean = InfoOperazione.IsFertirrigazione

        Select Case InfoOperazione.TipoCentroDiCosto
            Case centri_di_costo.Tipo.Esercizio
                For Each destinazione In listaDestinazioni
                    Dim chiave As String = destinazione.Piva & "_" + destinazione.Sa_Cod.ToString & "_" + destinazione.Appezza.ToString & "_" + destinazione.Id_Reg.ToString

                    If destinazione.Tipo_Destinazione = TIPO_DESTINAZIONE_IMPIANTO AndAlso Not listaImpianti.ContainsKey(chiave) Then

                        Dim impianto As New Reg_Impianti With {
                            .PIVA = destinazione.Piva,
                            .SA_COD = destinazione.Sa_Cod,
                            .APPEZZA = destinazione.Appezza,
                            .ID_REG = destinazione.Id_Reg
                        }

                        Dim cdc As New EsercizioCDC With {
                            .quantita = destinazione.Qta,
                            .superficieTrattata = destinazione.Qta2,
                            .esercizio = LeggiEsercizio(impianto, destinazione, attivita.inizio, objParametri_Server)
                        }

                        If InfoOperazione.IsTrattamento OrElse InfoOperazione.IsFertilizzazione Then
                            cdc.superficieRiduzioneBufferZone = destinazione.Sup_Riduzione_BufferZone
                            cdc.percentualeRiduzioneDeriva = destinazione.Perc_Riduzione_Deriva
                        End If

                        attivita.centriDiCosto.Add(cdc)

                        superficieTrattataTotale += destinazione.Qta2

                        Dim list As New List(Of Ricette_Destinazioni)
                        list.Add(destinazione)

                        listaImpianti.Add(chiave, New Tuple(Of EsercizioCDC, List(Of Ricette_Destinazioni))(cdc, list))

                        attivita.utilizzoTerreno = BuildUtilizzoTerreno(destinazione.Piva, destinazione.Sa_Cod, destinazione.Appezza, destinazione.Id_Reg, verbose, objParametri_Server)

                        'DT: i rilievi non sono ricettabili;
                        'se arrivano da APP transitano direttamente in agenda tramite il metodo AgronicaCoreContabBIZ.Ricette_W.ImportaAgendDaTabelleAPP
                        'Pertanto si commenta la relativa gestione (almeno per il momento)

                        'If InfoOperazione.IsRilievo Then
                        '    'DT: per i rilievi gli esercizi cdc sono le destinazioni di tipo 0, distinte
                        '    Dim cdc As New EsercizioCDC
                        '    LeggiEsercizioCDC(cdc, impiantoDestinazione, attivita.inizio, objParametri_Server)
                        '    attivita.centriDiCosto.Add(cdc)
                        'End If

                        If InfoOperazione.IsIrrigazione Then
                            'DT: per le irrigazioni gli esercizi cdc sono le destinazioni di tipo 0, distinte
                            Dim esercizioCDC As New EsercizioCDC
                            LeggiEsercizioCDC(esercizioCDC, destinazione, attivita.inizio, objParametri_Server)
                            attivita.centriDiCosto.Add(cdc)
                        End If


                    ElseIf listaImpianti.ContainsKey(chiave) Then
                        listaImpianti.Item(chiave).Item2.Add(destinazione)
                    End If

                    'DT: i rilievi non sono ricettabili
                    'If InfoOperazione.IsRilievo Then
                    '    'DT: i rilievi non hanno carichi nè scarichi, pertanto tutte le destinazioni sono impianti
                    '    listaDettaglioTecnico = EFRicette.ReadRicettaDettaglioTecnico(db, Piva, ricetta_operazione.Ricetta_Cod, ricetta_operazione.Ricetta_Operazione_Cod, impiantoDestinazione.Ricetta_Dettaglio_Cod)
                    '    Dim dettaglioCorrente As Ricette_Dettagli = (From d In listaDettagliDistribuzioni Where d.Ricetta_Dettaglio_Cod = impiantoDestinazione.Ricetta_Dettaglio_Cod Select d).FirstOrDefault
                    '    Dim tecnico As Ricette_Dettaglio_Tecnico = listaDettaglioTecnico.FirstOrDefault

                    '    'DT: si crea un dettaglioRilievo per ogni destinazione, prendendo i dati del rilievo dal dettaglio tecnico e la qta della destinazione
                    '    Dim dettCorrente = New dettagli.DettaglioRilievo
                    '    LeggiDettaglioRilievo(ricetta_operazione.Lav_Cod, attivita, InfoOperazione, dettCorrente, dettaglioCorrente, tecnico, impiantoDestinazione, objParametri_Server)
                    '    attivita.risorse.Add(dettCorrente)
                    'End If


                    If InfoOperazione.IsIrrigazione Then
                        'DT: le irrigazioni non hanno carichi nè scarichi, pertanto tutte le destinazioni sono impianti

                        listaDettaglioTecnico = EFRicette.ReadRicettaDettaglioTecnico(db, Piva, ricetta_operazione.Ricetta_Cod, ricetta_operazione.Ricetta_Operazione_Cod, destinazione.Ricetta_Dettaglio_Cod)
                        Dim dettaglioCorrente As Ricette_Dettagli = (From d In listaDettagli Where d.Ricetta_Dettaglio_Cod = destinazione.Ricetta_Dettaglio_Cod Select d).FirstOrDefault
                        Dim tecnico As Ricette_Dettaglio_Tecnico = listaDettaglioTecnico.FirstOrDefault

                        'DT: si crea un dettaglioIrrigazione per ogni destinazione, prendendo i dati dell'irrigazione dal dettaglio tecnico e la qta della destinazione
                        Dim dettCorrente = New dettagli.DettaglioIrrigazione
                        LeggiDettaglioIrrigazione(attivita.inizio, dettCorrente, dettaglioCorrente, tecnico, destinazione, objParametri_Server)

                        attivita.risorse.Add(dettCorrente)
                    End If

                    If InfoOperazione.IsFertirrigazione Then
                        listaDettaglioTecnico = EFRicette.ReadRicettaDettaglioTecnico(db, Piva, ricetta_operazione.Ricetta_Cod, ricetta_operazione.Ricetta_Operazione_Cod, destinazione.Ricetta_Dettaglio_Cod)
                        Dim dettaglioCorrente As Ricette_Dettagli = (From d In listaDettagli Where d.Ricetta_Dettaglio_Cod = destinazione.Ricetta_Dettaglio_Cod Select d).FirstOrDefault
                        Dim tecnico As Ricette_Dettaglio_Tecnico = listaDettaglioTecnico.FirstOrDefault
                        If dettaglioCorrente.Elem_Cod = CostantiPersonalizzate.ALTRE_MATERIE AndAlso dettaglioCorrente.Mat_Cod = CostantiPersonalizzate.MAT_COD_ACQUA_IRRIGAZIONE Then
                            Dim dettCorrente = New dettagli.DettaglioIrrigazione
                            LeggiDettaglioIrrigazione(attivita.inizio, dettCorrente, dettaglioCorrente, tecnico, destinazione, objParametri_Server)
                            attivita.risorse.Add(dettCorrente)
                            creaDettagliIrrigazioneDefault = False
                        End If
                    End If

                Next

            Case centri_di_costo.Tipo.ProdottoDaTrattare
                For Each dettaglio In listaDettagli
                    For Each destinazione In listaDestinazioni
                        If destinazione.Ricetta_Dettaglio_Cod = dettaglio.Ricetta_Dettaglio_Cod Then
                            Dim chiaveProdotto As String = CStr(dettaglio.Mat_Cod) + "_" + CStr(dettaglio.Lotto)

                            If destinazione.Tipo_Destinazione = CostantiPersonalizzate.MAGAZZINO AndAlso destinazione.Qta = 0 AndAlso Not listaGiacenzeMagazzino.ContainsKey(chiaveProdotto) Then

                                Dim magazzinoPK = New Fabbricato(destinazione.Piva, destinazione.Sa_Cod, destinazione.Id_Reg, "")
                                Dim prodottoDaTrattareCdC As New ProdottoDaTrattareCDC With {
                                        .giacenzaMagazzino = New MovimentoDiMagazzino With {
                                        .Magazzino = magazzinoPK,
                                        .Prodotto = LeggiProdottoDaTrattare(dettaglio, destinazione, objParametri_Server),
                                        .codice_progetto = 0, 'Nelle ricette raggruppiamo escludendo il progetto_cod
                                        .Lotto = dettaglio.Lotto
                                    },
                                    .qtaTrattata = dettaglio.Qta / 100
                                }

                                attivita.centriDiCosto.Add(prodottoDaTrattareCdC)

                                superficieTrattataTotale += dettaglio.Qta / 100

                                Dim list As New List(Of Ricette_Destinazioni)
                                list.Add(destinazione)

                                listaGiacenzeMagazzino.Add(chiaveProdotto, New Tuple(Of ProdottoDaTrattareCDC, List(Of Ricette_Destinazioni))(prodottoDaTrattareCdC, list))

                                attivita.utilizzoTerreno = BuildUtilizzoTerrenoFromProdottoDaTrattare(destinazione.Piva, dettaglio.Elem_Cod, dettaglio.Mat_Cod, verbose, objParametri_Server)

                            ElseIf listaGiacenzeMagazzino.ContainsKey(chiaveProdotto) Then
                                listaGiacenzeMagazzino.Item(chiaveProdotto).Item2.Add(destinazione)
                            End If
                        End If
                    Next
                Next
        End Select

        Dim acquaTotale As Decimal = 0
        If InfoOperazione.IsFertilizzazione OrElse InfoOperazione.IsTrattamento Then
            Dim dettaglioTecnicoAcqua As Ricette_Dettaglio_Tecnico = EFRicette.ReadRicettaDettaglioTecnico(db, Piva, ricetta_operazione.Ricetta_Cod, ricetta_operazione.Ricetta_Operazione_Cod, 0).FirstOrDefault

            If dettaglioTecnicoAcqua IsNot Nothing Then
                Dim risorsaAcqua = New RisorsaAcqua

                If dettaglioTecnicoAcqua.Qta_Ril > 0 Then
                    risorsaAcqua.acqua = dettaglioTecnicoAcqua.Qta_Ril
                    risorsaAcqua.doseAcqua = RisorsaAcqua.TipoDoseAcqua.TOTALE

                    acquaTotale = risorsaAcqua.acqua

                Else
                    risorsaAcqua.acqua = -dettaglioTecnicoAcqua.Qta_Ril
                    risorsaAcqua.doseAcqua = RisorsaAcqua.TipoDoseAcqua.HA

                    acquaTotale = risorsaAcqua.acqua * superficieTrattataTotale

                End If

                attivita.risorse.Add(risorsaAcqua)
            End If
        End If

        If leggiDisciplinari Then
            attivita.disciplinare = Utility.BuildDisciplinare(InfoOperazione, ricetta_operazione.Num_Protocollo, ricetta_operazione.Id_Rcdpi, attivita.utilizzoTerreno, ricetta_operazione.Lav_Cod, ricetta_operazione.Validita_Inizio, verbose, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
        End If

        If Not InfoOperazione.IsRilievo Then

            For Each dettaglioCampagna In listaDettagliCampagna
                listaDettaglioTecnico = EFRicette.ReadRicettaDettaglioTecnico(db, Piva, ricetta_operazione.Ricetta_Cod, ricetta_operazione.Ricetta_Operazione_Cod, dettaglioCampagna.Ricetta_Dettaglio_Cod)

                Dim dettagliMagazzino As New List(Of Ricette_Dettagli)

                Dim dettaglioMagazzinoInnesco As Ricette_Dettagli = Nothing

                Dim destinazioneMagazzinoInnesco As Ricette_Destinazioni = Nothing

                If InfoOperazione.IsSemina OrElse InfoOperazione.IsRaccolta Then
                    dettagliMagazzino = (From d In listaDettagli Where (d.Cau_Mov.ToString() = CAU_SCARICO OrElse d.Cau_Mov.ToString() = CAU_CARICO) AndAlso d.Elem_Cod = dettaglioCampagna.Elem_Cod AndAlso d.Mat_Cod = dettaglioCampagna.Mat_Cod AndAlso d.Lotto.ToUpper = dettaglioCampagna.Lotto.ToUpper Select d).ToList()
                Else
                    dettagliMagazzino = (From d In listaDettagli Where (d.Cau_Mov.ToString() = CAU_SCARICO OrElse d.Cau_Mov.ToString() = CAU_CARICO) AndAlso d.Elem_Cod = dettaglioCampagna.Elem_Cod AndAlso d.Pro_Cod = dettaglioCampagna.Pro_Cod).ToList()

                    'Controllo se ci sono degli inneschi scaricati da magazzino
                    If InfoOperazione.IsTrattamento Then

                        Dim Av_Cod As Integer = listaDettaglioTecnico.FirstOrDefault.Av_Cod

                        Dim Av_Gru_Cod As Integer = listaDettaglioTecnico.FirstOrDefault.Av_Gru

                        dettaglioMagazzinoInnesco = (From d In listaDettagli Where (d.Cau_Mov.ToString() = CAU_SCARICO OrElse d.Cau_Mov.ToString() = CAU_CARICO) AndAlso d.Elem_Cod = INNESCHI AndAlso (d.Pro_Cod = Av_Cod OrElse d.Pro_Cod = Av_Gru_Cod)).FirstOrDefault()

                        If Not IsNothing(dettaglioMagazzinoInnesco) Then
                            destinazioneMagazzinoInnesco = (From dd In listaDestinazioni Where dd.Ricetta_Dettaglio_Cod = dettaglioMagazzinoInnesco.Ricetta_Dettaglio_Cod AndAlso dd.Tipo_Destinazione = 20 Select dd).FirstOrDefault()
                        End If

                    End If
                End If

                'se non ci sono scarichi, deve comunque procedere a creare i dettagli (escamotage per entrare nel for each seguente)
                If dettagliMagazzino.Count = 0 Then
                    dettagliMagazzino.Add(New Ricette_Dettagli())
                End If

                For Each dettaglioMagazzino In dettagliMagazzino
                    Dim destinazioneMagazzino = New Ricette_Destinazioni

                    'se il dettaglio dello scarico esiste, aggancio la sua destinazione
                    If dettaglioMagazzino.Ricetta_Dettaglio_Cod > 0 Then
                        destinazioneMagazzino = (From dd In listaDestinazioni Where dd.Ricetta_Dettaglio_Cod = dettaglioMagazzino.Ricetta_Dettaglio_Cod AndAlso dd.Tipo_Destinazione = 20 Select dd).FirstOrDefault()
                    Else
                        dettaglioMagazzino = Nothing 'era il dettaglio creato ad hoc per entrare nel ciclo, ma non esiste, lo rimetto a nothing
                    End If

                    If InfoOperazione.IsLavorazione AndAlso dettaglioCampagna.ID_Attivita <> 0 Then
                        codiceAttivitaPersonalizzata = dettaglioCampagna.ID_Attivita 'DT: se si arriva da Gias, l'id_attivita è qui
                    End If

                    If InfoOperazione.IsTrattamento Then

                        If InfoOperazione.TipoCentroDiCosto = centri_di_costo.Tipo.ProdottoDaTrattare AndAlso
                           (dettaglioCampagna.Elem_Cod = TRASFORMATI_VEGETALI OrElse dettaglioCampagna.Elem_Cod = SEMENTI) Then
                            'Quando il centro di costo è un prodotto da trattare, non devo creare un DettaglioTrattamento dal current listaDettagliCampagna
                        Else
                            Dim dettCorrente = New dettagli.DettaglioTrattamento
                            LeggiDettaglioTrattamento(pivaRicettaOperazione, dettCorrente, dettaglioCampagna, dettaglioMagazzinoInnesco, listaDettaglioTecnico.FirstOrDefault, dettaglioMagazzino, destinazioneMagazzino,
                                                      destinazioneMagazzinoInnesco, verbose, objParametri_Server, superficieTrattataTotale, acquaTotale,
                                                      ricetta_operazione.Lav_Cod, attivita, ricetta_operazione.Mezzo, InfoOperazione, listaImpianti)

                            If dettaglioCampagna.Cau_Mov.ToString = CAU_LAVORAZIONE AndAlso superficieTrattataTotale > 0 AndAlso dettaglioCampagna.Qta_Extra_Totale = 0 Then
                                dettCorrente.doseHaReale = If(superficieTrattataTotale <> 0, dettaglioCampagna.Qta / superficieTrattataTotale, 0)
                                dettCorrente.quantitaTotaleReale = If(superficieTrattataTotale <> 0, dettaglioCampagna.Qta / superficieTrattataTotale, 0)
                            End If

                            attivita.risorse.Add(dettCorrente)
                        End If
                    End If

                    If InfoOperazione.IsFertilizzazione Then
                        If dettaglioCampagna.Elem_Cod <> CostantiPersonalizzate.ALTRE_MATERIE OrElse dettaglioCampagna.Mat_Cod <> CostantiPersonalizzate.MAT_COD_ACQUA_IRRIGAZIONE Then
                            Dim dettCorrente = New dettagli.DettaglioFertilizzazione
                            LeggiDettaglioFertilizzazione(pivaRicettaOperazione, CType(dettCorrente, dettagli.DettaglioFertilizzazione), dettaglioCampagna, listaDettaglioTecnico.FirstOrDefault, dettaglioMagazzino, destinazioneMagazzino, superficieTrattataTotale, acquaTotale, attivita)
                            attivita.risorse.Add(dettCorrente)
                        End If
                    End If

                    If InfoOperazione.IsSemina Then
                        Dim dettCorrente = New dettagli.DettaglioSemina
                        LeggiDettaglioSemina(pivaRicettaOperazione, CType(dettCorrente, dettagli.DettaglioSemina), dettaglioCampagna, listaDettaglioTecnico.FirstOrDefault, dettaglioMagazzino, destinazioneMagazzino, superficieTrattataTotale, acquaTotale, attivita)

                        If dettCorrente.prodotto IsNot Nothing AndAlso dettCorrente.prodotto.codice > 0 Then
                            If dettaglioCampagna.Cau_Mov.ToString = CAU_LAVORAZIONE AndAlso superficieTrattataTotale > 0 AndAlso dettaglioCampagna.Qta_Extra_Totale = 0 Then
                                dettCorrente.doseHaReale = If(superficieTrattataTotale <> 0, dettaglioCampagna.Qta / superficieTrattataTotale, 0)
                                dettCorrente.quantitaTotaleReale = dettaglioCampagna.Qta
                            End If

                            attivita.risorse.Add(dettCorrente)
                        End If

                    End If

                    If InfoOperazione.IsRaccolta Then
                        'If dettaglioCampagna.Lotto.toUpper <> dettaglioMagazzino.Lotto.toUpper AndAlso dettaglioCampagna.Mat_Cod <> dettaglioMagazzino.Mat_Cod Then
                        '    'Il dettaglio di magazzino non è collegato a quest dettaglio prodotto!
                        '    Continue For
                        'End If

                        Dim dettCorrente = creaBaseDettaglioRaccolta(pivaRicettaOperazione,
                                               dettaglioCampagna, listaDettaglioTecnico.FirstOrDefault,
                                               dettaglioMagazzino, destinazioneMagazzino,
                                               superficieTrattataTotale, acquaTotale, attivita)

                        ' listaDestinazioni contiene destinazioni impianti e magazzini per il singolo centro
                        ' Prendo la chiave dell'impianto interessato dal dettaglio corrente
                        Dim keyImpianti = listaDestinazioni.Where(Function(d) d.Ricetta_Dettaglio_Cod = dettaglioCampagna.Ricetta_Dettaglio_Cod AndAlso d.Tipo_Destinazione = 0).
                            Select(Function(d) d.Piva & "_" & d.Sa_Cod & "_" & d.Appezza & "_" & d.Id_Reg).ToList()
                        Dim giaAggiunti As New List(Of String)

                        If dettCorrente.MagazziniMovimentazioni.Count > 0 Then
                            dettCorrente.quantitaTotaleReale = dettCorrente.MagazziniMovimentazioni.FirstOrDefault().Qta

                            ' trovo la risorsa con stessa combinazione prodotto/lotto/magazzino
                            Dim found = attivita.risorse.Where(Function(r) r.classType = "DettaglioRaccolta").Cast(Of DettaglioRaccolta).
                            Where(Function(r) r.prodotto.codice = dettCorrente.prodotto.codice AndAlso
                                r.MagazziniMovimentazioni.FirstOrDefault().Lotto.ToUpper = dettCorrente.MagazziniMovimentazioni.FirstOrDefault().Lotto.ToUpper AndAlso
                                r.MagazziniMovimentazioni.FirstOrDefault().Magazzino.primaryKey.codice = dettCorrente.MagazziniMovimentazioni.FirstOrDefault().Magazzino.primaryKey.codice AndAlso
                                r.MagazziniMovimentazioni.FirstOrDefault().Magazzino.primaryKey.centroAziendalePK.codice = dettCorrente.MagazziniMovimentazioni.FirstOrDefault().Magazzino.primaryKey.centroAziendalePK.codice AndAlso
                                r.MagazziniMovimentazioni.FirstOrDefault().Magazzino.primaryKey.centroAziendalePK.partitaIva = dettCorrente.MagazziniMovimentazioni.FirstOrDefault().Magazzino.primaryKey.centroAziendalePK.partitaIva
                            ).FirstOrDefault()

                            If Not IsNothing(found) Then
                                '-- La risorsa è già stata aggiunta, evitiamo di duplicarla
                                Continue For
                            End If
                        End If

                        For Each key In keyImpianti
                            If Not giaAggiunti.Contains(key) Then
                                aggiungiSuImpianto(dettCorrente, key, listaImpianti)
                                giaAggiunti.Add(key)
                            End If
                        Next

                        Dim superficieTotaleDettaglio As Decimal = 0

                        If dettCorrente IsNot Nothing AndAlso dettCorrente.QuantitaSuImpianti IsNot Nothing AndAlso dettCorrente.QuantitaSuImpianti.Count > 0 Then
                            superficieTotaleDettaglio = dettCorrente.QuantitaSuImpianti.
                                Select(Function(q) q.esercizioCDC.superficieTrattata).
                                Aggregate(Function(s1, s2) s1 + s2)
                        End If

                        For Each impianto In dettCorrente.QuantitaSuImpianti

                            If AppHelper.FindValueInDictOrigineAttivitaToImport(attivita.origine) Then 'TODO_DT: verificare se si può migliorare, soprattutto se rifaremo l'import ricetta da app passando dal modello e non da tabelle app_*
                                Dim dettaglioCampagnaCorrente = listaDettagliCampagna.Where(Function(d) d.Lotto.ToUpper = impianto.Lotto.ToUpper).FirstOrDefault
                                Dim dettaglioDestinazioneCorrente = listaDestinazioni.Where(Function(d) d.Ricetta_Dettaglio_Cod = dettaglioCampagnaCorrente.Ricetta_Dettaglio_Cod AndAlso d.Tipo_Destinazione = 0 AndAlso
                             d.Piva = impianto.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva AndAlso d.Sa_Cod = impianto.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice AndAlso d.Appezza = impianto.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.codice AndAlso d.Id_Reg = impianto.esercizioCDC.esercizio.impiantoPK.codice).FirstOrDefault

                                impianto.Qta = dettaglioDestinazioneCorrente.Qta
                            Else
                                Dim quotaSuperficieImpianto = impianto.esercizioCDC.superficieTrattata / superficieTotaleDettaglio
                                impianto.Qta = Agro_Math.RoundNumber_2Decimali(dettCorrente.quantitaTotaleReale * quotaSuperficieImpianto)
                            End If


                        Next

                        attivita.risorse.Add(dettCorrente)
                    End If
                Next
            Next
        End If

        ' creo i dettagli irrigazione default per la nuova fertirrigazione
        If InfoOperazione.IsFertirrigazione AndAlso creaDettagliIrrigazioneDefault Then
            Dim doseAcqua As Decimal = acquaTotale / superficieTrattataTotale / 10
            Dim dettagliIrrigazione = CreaDettagliIrrigazione(attivita, doseAcqua)
            If dettagliIrrigazione IsNot Nothing Then
                attivita.risorse.AddRange(dettagliIrrigazione)
            End If
        End If

        LeggiNoteIntervento(attivita, verbose, objParametri_Server)
        LeggiCostiAccessori(attivita, ricetta, verbose, objParametri_Server)

        attivita.attivitaPersonalizzata = If(codiceAttivitaPersonalizzata <> 0, New AttivitaPersonalizzata(codiceAttivitaPersonalizzata), Nothing)

        'Split agenzie
        Dim listaRisorseToAdd As New List(Of RisorsaProdotto)
        If attivita.risorse IsNot Nothing Then
            For Each risorsa In attivita.risorse
                If risorsa.GetType.BaseType Is GetType(AgronicaCoreModelsSTD.attivita.risorse.RisorsaProdotto) Then
                    Dim risorsaProdotto As AgronicaCoreModelsSTD.attivita.risorse.RisorsaProdotto = CType(risorsa, AgronicaCoreModelsSTD.attivita.risorse.RisorsaProdotto)
                    If risorsaProdotto.MagazziniMovimentazioni IsNot Nothing AndAlso risorsaProdotto.MagazziniMovimentazioni.Count > 1 Then
                        Dim risorsaProdottoOrig = risorsaProdotto.Clona 'serve per mantenere la lista originale dei rilevamenti

                        'creo una risorsa prodotto per ogni rilevamento di magazzino, ognuno con un rilevamento di magazzino (invece di n come può capitare se ci sono agenzie)
                        For i = 0 To risorsaProdotto.MagazziniMovimentazioni.Count - 1
                            If i = 0 Then 'preservo la prima risorsaProdotto (per non doverla rimuovere da attivita.risorse) ma lascio solo il primo rilevamento di magazzino
                                risorsaProdotto.MagazziniMovimentazioni.Clear()
                                risorsaProdotto.MagazziniMovimentazioni.Add(risorsaProdottoOrig.MagazziniMovimentazioni(i))
                            Else
                                Dim risorsaProdottoNew = risorsaProdottoOrig.Clona
                                risorsaProdottoNew.MagazziniMovimentazioni.Clear()
                                risorsaProdottoNew.MagazziniMovimentazioni.Add(risorsaProdottoOrig.MagazziniMovimentazioni(i))
                                listaRisorseToAdd.Add(risorsaProdottoNew)
                            End If
                        Next

                    End If
                End If
            Next
        End If

        attivita.risorse.AddRange(listaRisorseToAdd)


        If verbose Then
            CompleteVerbose(attivita, isRibaltamentoToAgenda, listParametriAggiuntivi, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
        End If

        Return attivita

    End Function

    Private Sub aggiungiSuImpianto(dettCorrente As DettaglioRaccolta, key As String, listaImpianti As Dictionary(Of String, Tuple(Of EsercizioCDC, List(Of Ricette_Destinazioni))))
        Dim itemImpianto = listaImpianti.Item(key)
        Dim quantitaSuImpianto = New QuantitaSuImpianto

        quantitaSuImpianto.Prodotto = dettCorrente.prodotto
        quantitaSuImpianto.esercizioCDC = itemImpianto.Item1
        quantitaSuImpianto.Qta = 0

        If IsNothing(dettCorrente.MagazziniMovimentazioni.FirstOrDefault()) Then
            quantitaSuImpianto.Lotto = ""
            quantitaSuImpianto.Magazzino = Nothing
        Else
            quantitaSuImpianto.Lotto = dettCorrente.MagazziniMovimentazioni.FirstOrDefault().Lotto
            quantitaSuImpianto.Magazzino = dettCorrente.MagazziniMovimentazioni.FirstOrDefault().Magazzino
        End If

        dettCorrente.QuantitaSuImpianti.Add(quantitaSuImpianto)
    End Sub

    Private Function creaBaseDettaglioRaccolta(pivaRicettaOperazione As String, dettaglioCorrente As Ricette_Dettagli, dettaglioTecnico As Ricette_Dettaglio_Tecnico,
                                               dettaglioMagazzino As Ricette_Dettagli, destinazioneMagazzino As Ricette_Destinazioni,
                                               superficieTrattataTotale As Decimal, acquaTotale As Decimal, ByVal attivita As AgronicaCoreModelsSTD.attivita.Attivita) As DettaglioRaccolta
        Dim dettCorrente = New DettaglioRaccolta
        LeggiDettaglioRaccolta(pivaRicettaOperazione, dettCorrente,
                               dettaglioCorrente, dettaglioTecnico,
                               dettaglioMagazzino, destinazioneMagazzino,
                               superficieTrattataTotale, acquaTotale, attivita)

        dettCorrente.Opzioni_Raccolta = New Opzioni_Raccolta
        dettCorrente.Opzioni_Raccolta.Ripartizione = enum_Ripartizione_Raccolta.AUTO_SUPERFICIE
        dettCorrente.Opzioni_Raccolta.GenerazioneLotto = enum_Generazione_Lotto_Raccolta.MANUALE
        dettCorrente.Opzioni_Raccolta.Modalita = enum_Modalita_Raccolta.MECCANICA

        dettCorrente.dataIngresso = dettaglioCorrente.Validita_Inizio

        If dettaglioCorrente.Cau_Mov.ToString = CAU_LAVORAZIONE AndAlso superficieTrattataTotale > 0 AndAlso dettaglioCorrente.Qta_Extra_Totale = 0 Then
            dettCorrente.doseHaReale = If(superficieTrattataTotale <> 0, dettaglioCorrente.Qta / superficieTrattataTotale, 0)
            dettCorrente.quantitaTotaleReale = dettaglioCorrente.Qta
        End If

        dettCorrente.QuantitaSuImpianti = New List(Of QuantitaSuImpianto)

        Return dettCorrente
    End Function

    Private Function LeggiEsercizio(ByVal reg_impianto As Reg_Impianti, ByVal impiantoDestinazione As Ricette_Destinazioni, data As Date, objParametri_Server As AgronicaCoreParametri) As Esercizio
        Dim centroAziendale = LeggiCentroAziendale(reg_impianto.PIVA, reg_impianto.SA_COD)

        Dim appezzamentoPK = New AgronicaCoreModelsSTD.anagrafiche.Appezzamento.PK(reg_impianto.APPEZZA, centroAziendale.primaryKey)

        Dim appezzamento = New AgronicaCoreModelsSTD.anagrafiche.Appezzamento(appezzamentoPK)

        Dim impiantoPK = New AgronicaCoreModelsSTD.anagrafiche.Impianto.PK(reg_impianto.ID_REG, appezzamentoPK)

        Dim impianto = New Impianto(impiantoPK)

        Dim progetto_cod As Integer = 0
        If impiantoDestinazione.Tipo_Destinazione = 0 Then
            Dim objImpreseProgetti As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
            Dim dtImpreseProgetti = objImpreseProgetti.Progetto_from_PivaSaCodAppezzaIdreg(impiantoDestinazione.Piva,
                                                                                impiantoDestinazione.Sa_Cod,
                                                                                impiantoDestinazione.Appezza,
                                                                                impiantoDestinazione.Id_Reg,
                                                                                data,
                                                                                objParametri_Server)

            If dtImpreseProgetti IsNot Nothing AndAlso dtImpreseProgetti.Rows.Count > 0 Then
                progetto_cod = dtImpreseProgetti.Rows(0).Item("Progetto_Cod")
            End If

        End If


        Dim esercizio = New Esercizio(progetto_cod, "") With {
            .impiantoPK = impianto.primaryKey
        }

        Return esercizio

    End Function

    Private Sub LeggiEsercizioCDC(ByVal esercizioCDC As EsercizioCDC, ByVal impiantoDestinazione As Ricette_Destinazioni, data As Date, objParametri_Server As AgronicaCoreParametri)
        Dim impianto = New Reg_Impianti With {
            .PIVA = impiantoDestinazione.Piva,
            .SA_COD = impiantoDestinazione.Sa_Cod,
            .APPEZZA = impiantoDestinazione.Appezza,
            .ID_REG = impiantoDestinazione.Id_Reg
        }
        esercizioCDC.quantita = impiantoDestinazione.Qta
        esercizioCDC.superficieTrattata = impiantoDestinazione.Qta2
        esercizioCDC.esercizio = LeggiEsercizio(impianto, impiantoDestinazione, data, objParametri_Server)
    End Sub


    Private Function LeggiProdottoDaTrattare(ByVal prodottoDettaglio As Ricette_Dettagli, prodottoDestinazione As Ricette_Destinazioni, objParametri_Server As AgronicaCoreParametri) As Prodotto

        Dim veg_cod As Integer
        Dim veg_des As String = ""

        Dim objMateriePrime As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
        objMateriePrime.VegCod_CulCod_from_MatCod(prodottoDestinazione.Piva,
                                               prodottoDettaglio.Elem_Cod,
                                               prodottoDettaglio.Mat_Cod,
                                               veg_cod,
                                               0,
                                               objParametri_Server,
                                               verbose:=True,
                                               Veg_Des:=veg_des)

        Dim prodotto = New Prodotto(prodottoDettaglio.Mat_Cod) With {
            .elemCod = prodottoDettaglio.Elem_Cod,
            .specie = New utilizzi.Specie(veg_cod, veg_des)
        }

        Return prodotto

    End Function

    'Private Sub LeggiDettaglioRilievo(ByVal lavCod As Integer, ByVal attivita As AgronicaCoreModelsSTD.attivita.Attivita, ByVal infoOperazione As InfoOperazione, ByVal DettaglioDaLeggere As dettagli.DettaglioRilievo, ByVal Dettagli As Ricette_Dettagli, ByVal Tecnico As Ricette_Dettaglio_Tecnico, ByVal Destinazione As Ricette_Destinazioni, ByVal objParametri_Server As AgronicaCoreParametri)

    '    DettaglioDaLeggere.unitaDiMisura = New UnitaDiMisura(Tecnico.Dett_Cod)
    '    DettaglioDaLeggere.QtaRilevata = Destinazione.Qta
    '    DettaglioDaLeggere.QtaRilevataString = Destinazione.Qta.ToString

    '    Dim cdc As New EsercizioRilievoCDC
    '    LeggiEsercizioCDC(cdc, Destinazione, attivita.inizio, objParametri_Server)
    '    DettaglioDaLeggere.esercizioCDC = cdc

    '    Select Case lavCod
    '        Case LAVCOD_RILIEVO_AVVERSITA_CAMPO
    '            DettaglioDaLeggere.avversitaGruppo = GetAvversitaGruppo(Tecnico.Av_Cod, Tecnico.Av_Gru)

    '        Case LAVCOD_RILIEVO_INDICI_MATURITA
    '            DettaglioDaLeggere.IndiceMaturita = Tecnico.FF_Classe

    '        Case LAVCOD_DANNI_RACCOLTA
    '            DettaglioDaLeggere.DannoRaccolta = Tecnico.FF_Classe

    '        Case LAVCOD_FASI_FENOLOGICHE
    '            DettaglioDaLeggere.FaseFenologica = Tecnico.FF_Classe
    '            DettaglioDaLeggere.DataOraRilievo = Destinazione.Validita_Inizio

    '        Case LAVCOD_RILIEVO_ERBE_INFESTANTI
    '            DettaglioDaLeggere.erbaInfestante = GetAvversitaGruppo(Tecnico.Av_Cod, Tecnico.Av_Gru)

    '        Case LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA
    '            DettaglioDaLeggere.IndiceResa = Tecnico.FF_Classe

    '    End Select

    'End Sub

    Private Sub LeggiDettaglioFertilizzazione(ByVal pivaPerLetturaProdotti As String, ByVal DettaglioDaLeggere As dettagli.DettaglioFertilizzazione, ByVal Dettagli As Ricette_Dettagli, ByVal Tecnico As Ricette_Dettaglio_Tecnico, ByVal DettaglioMagazzino As Ricette_Dettagli, ByVal DestinazioneMagazzino As Ricette_Destinazioni, ByVal superficieTrattataTotale As Decimal, ByVal acquaTotale As Decimal, ByVal attivita As AgronicaCoreModelsSTD.attivita.Attivita)
        LeggiRisorsaProdotto(pivaPerLetturaProdotti, DettaglioDaLeggere, Dettagli, DettaglioMagazzino, DestinazioneMagazzino, superficieTrattataTotale, acquaTotale, attivita)

        If Tecnico IsNot Nothing Then
            DettaglioDaLeggere.N = Tecnico.N
            DettaglioDaLeggere.P = Tecnico.P
            DettaglioDaLeggere.K = Tecnico.K
            DettaglioDaLeggere.Cu = Tecnico.Cu
            DettaglioDaLeggere.Mg = Tecnico.Mg
            DettaglioDaLeggere.efficienza = Tecnico.Efficienza
        End If

    End Sub

    Private Sub LeggiDettaglioIrrigazione(ByVal dataAttivita As Date, ByVal DettaglioDaLeggere As dettagli.DettaglioIrrigazione, ByVal Dettagli As Ricette_Dettagli, ByVal Tecnico As Ricette_Dettaglio_Tecnico, ByVal Destinazione As Ricette_Destinazioni, ByVal objParametri_Server As AgronicaCoreParametri)

        If Tecnico IsNot Nothing Then
            DettaglioDaLeggere.QtaRilevata = Tecnico.Qta_Ril
            DettaglioDaLeggere.Ore = Tecnico.Dose
            DettaglioDaLeggere.Portata = Tecnico.Parziale

            If Tecnico.Efficienza = 0 Then 'irrigazione salvata tramite interfaccia vecchia (IrrigazioneBS.aspx)

                DettaglioDaLeggere.Efficienza = 100.0

                If Tecnico.Nitrati = 0 Then
                    DettaglioDaLeggere.Frequenza = 1
                    DettaglioDaLeggere.DataInizio = dataAttivita
                    DettaglioDaLeggere.DataFine = dataAttivita
                Else
                    DettaglioDaLeggere.Frequenza = Tecnico.Nitrati
                    DettaglioDaLeggere.DataInizio = Tecnico.Inn1_Data
                    DettaglioDaLeggere.DataFine = Tecnico.Inn2_Data
                End If
            Else 'irrigazione salvata tramite interfaccia nuova (Angular)

                DettaglioDaLeggere.Efficienza = Tecnico.Efficienza
                DettaglioDaLeggere.Frequenza = Tecnico.Nitrati
                DettaglioDaLeggere.DataInizio = Tecnico.Inn1_Data
                DettaglioDaLeggere.DataFine = Tecnico.Inn2_Data

            End If

            DettaglioDaLeggere.tipoIrrigazione = New TipoIrrigazione(Tecnico.Freatimetro)
            DettaglioDaLeggere.unitaDiMisura = New UnitaDiMisura(Tecnico.Dett_Cod)
            DettaglioDaLeggere.consiglioIrrigazione = New Consiglio_Irrigazione(0, "") With {.qtaAcqua = 0, .dataConsiglio = AGRODATAINIZIO}
            If Tecnico.Ditta_Cod <> 0 Then
                DettaglioDaLeggere.macchina = New ParcoMacchine With {.codice = Tecnico.Ditta_Cod}
            Else
                DettaglioDaLeggere.macchina = Nothing
            End If
        End If

        If Destinazione IsNot Nothing Then
            DettaglioDaLeggere.QtaTotale = Destinazione.Qta
            Dim cdc As New EsercizioCDC
            LeggiEsercizioCDC(cdc, Destinazione, dataAttivita, objParametri_Server)
            DettaglioDaLeggere.esercizioCDC = cdc
        End If

    End Sub

    Private Sub LeggiDettaglioSemina(ByVal pivaPerLetturaProdotti As String, ByVal DettaglioDaLeggere As dettagli.DettaglioSemina, ByVal Dettagli As Ricette_Dettagli, ByVal Tecnico As Ricette_Dettaglio_Tecnico, ByVal DettaglioMagazzino As Ricette_Dettagli, ByVal DestinazioneMagazzino As Ricette_Destinazioni, ByVal superficieTrattataTotale As Decimal, ByVal acquaTotale As Decimal, ByVal attivita As AgronicaCoreModelsSTD.attivita.Attivita)
        LeggiRisorsaProdotto(pivaPerLetturaProdotti, DettaglioDaLeggere, Dettagli, DettaglioMagazzino, DestinazioneMagazzino, superficieTrattataTotale, acquaTotale, attivita)
    End Sub

    Private Sub LeggiDettaglioRaccolta(ByVal pivaPerLetturaProdotti As String, ByVal DettaglioDaLeggere As dettagli.DettaglioRaccolta, ByVal Dettagli As Ricette_Dettagli, ByVal Tecnico As Ricette_Dettaglio_Tecnico, ByVal DettaglioMagazzino As Ricette_Dettagli, ByVal DestinazioneMagazzino As Ricette_Destinazioni, ByVal superficieTrattataTotale As Decimal, ByVal acquaTotale As Decimal, ByVal attivita As AgronicaCoreModelsSTD.attivita.Attivita)
        LeggiRisorsaProdotto(pivaPerLetturaProdotti, DettaglioDaLeggere, Dettagli, DettaglioMagazzino, DestinazioneMagazzino, superficieTrattataTotale, acquaTotale, attivita)
    End Sub

    Public Sub LeggiDettaglioTrattamento(ByVal pivaPerLetturaProdotti As String, ByVal DettaglioDaLeggere As dettagli.DettaglioTrattamento, ByVal dettaglioCampagna As Ricette_Dettagli, ByVal DettaglioInnesco As Ricette_Dettagli, ByVal Tecnico As Ricette_Dettaglio_Tecnico, ByVal DettaglioMagazzino As Ricette_Dettagli,
                                         ByVal DestinazioneMagazzino As Ricette_Destinazioni, ByVal DestinazioneMagazzinoInnesco As Ricette_Destinazioni, ByVal verbose As Boolean, ByVal objParametri_Server As AgronicaCoreParametri, ByVal superficieTrattataTotale As Decimal, ByVal acquaTotale As Decimal, ByVal lav_cod As Integer, ByVal attivita As AgronicaCoreModelsSTD.attivita.Attivita,
                                         ByVal mezzo As Integer, ByVal infoOperazione As InfoOperazione, ByVal listaImpianti As Dictionary(Of String, Tuple(Of EsercizioCDC, List(Of Ricette_Destinazioni))))

        LeggiRisorsaProdotto(pivaPerLetturaProdotti, DettaglioDaLeggere, dettaglioCampagna, DettaglioMagazzino, DestinazioneMagazzino, superficieTrattataTotale, acquaTotale, attivita)

        If Tecnico IsNot Nothing Then

            DettaglioDaLeggere.avversitaGruppo = GetAvversitaGruppo(Tecnico.Av_Cod, Tecnico.Av_Gru)

            If Not IsNothing(DettaglioDaLeggere.avversitaGruppo) Then

                DettaglioDaLeggere.avversitaGruppo.MagazziniMovimentazioni = New List(Of RilevamentoDiMagazzino)

                If Not IsNothing(DettaglioInnesco) AndAlso Not IsNothing(DestinazioneMagazzinoInnesco) Then

                    DettaglioDaLeggere.avversitaGruppo.MagazziniMovimentazioni = LeggiRilevamentiMagazzino(DettaglioInnesco.Pro_Cod, DettaglioInnesco,
                                                                                                            DestinazioneMagazzinoInnesco, superficieTrattataTotale, acquaTotale, attivita)
                End If

            End If

            DettaglioDaLeggere.tipoFormulato = GetTipoFormulato(DettaglioDaLeggere.avversitaGruppo, lav_cod)
            DettaglioDaLeggere.soglia = GetSoglia(Tecnico.Soglia_Cod, Tecnico.Soglia_Quantita)

        End If

#Region "Dati che non arrivano da APP"

        DettaglioDaLeggere.dosiEtichetta = DosiEtichetta_from_Stringhe(dettaglioCampagna.DoseEtichetta, dettaglioCampagna.DoseEtichetta_Value, verbose, objParametri_Server)
        DettaglioDaLeggere.principiAttivi = GetPrincipiAttivi(dettaglioCampagna.PrincipiAttivi, dettaglioCampagna.PrincipiAttiviPesi, dettaglioCampagna.PrincipiAttiviPercAbb)
        DettaglioDaLeggere.bufferzone = GetBufferZone(dettaglioCampagna.Buffer)
        DettaglioDaLeggere.dettaglioProdotto = GetDettaglioProdotto(dettaglioCampagna.Extra_Str)
        DettaglioDaLeggere.tempoCarenza = If(dettaglioCampagna.TempoCarenza IsNot Nothing, dettaglioCampagna.TempoCarenza, 0)
        DettaglioDaLeggere.polverulento = If(dettaglioCampagna.Polverulento IsNot Nothing, dettaglioCampagna.Polverulento, 0)

#End Region

        DettaglioDaLeggere.ripartizioneTrappole = Nothing

        If lav_cod = LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA OrElse lav_cod = LAVCOD_REINNESCO_TRAPPOLE Then
            DettaglioDaLeggere.quantitaSuImpianti = GetQuantitaSuImpiantiDettaglioTrattamento(DettaglioDaLeggere, dettaglioCampagna, listaImpianti)

            DettaglioDaLeggere.ripartizioneTrappole = mezzo
        End If

    End Sub

    Private Sub LeggiCostiAccessori(attivita As AgronicaCoreModelsSTD.attivita.Attivita, ricetta As Ricette, verbose As Boolean, objParametri_Server As AgronicaCoreParametri)
        'DT: udm, qta e prezzo non vengono salvate, si usa il "Salva e vai ai costi" o comunque il CdG 

        Dim causaliCostiAccessori = New List(Of String)() From {
            CAU_IMPUTAZIONE_PARCOMACCHINE,
            CAU_IMPUTAZIONE_MANODOPERA,
            CAU_IMPUTAZIONE_TECNICO_RESPONSABILE,
            CAU_IMPUTAZIONE_TERZISTI
        }

        Dim objRicettaDettaglio As New Ricette_Dettagli_R
        Dim DtRicettaDettaglio As DataTable = objRicettaDettaglio.Leggi(attivita.testataRicetta.Ricetta_Cod, attivita.codice,
                                                                        0, "", 0, 0, 0, 0,
                                                                        AGRODATAINIZIO, AGRODATAFINE,
                                                                        AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                        "", "",
                                                                        objParametri_Server)

        For Each costoAccessorio As DataRow In DtRicettaDettaglio.Rows
            Dim CAU_MOV = costoAccessorio.Item("CAU_MOV")
            Dim Mat_Cod = costoAccessorio.Item("Mat_Cod")

            Select Case CAU_MOV
                Case CAU_IMPUTAZIONE_PARCOMACCHINE
                    Dim risorsa = New RisorsaMacchina
                    risorsa.macchina = New ParcoMacchine
                    risorsa.macchina.codice = Mat_Cod

                    attivita.risorse.Add(risorsa)

                Case CAU_IMPUTAZIONE_MANODOPERA, CAU_IMPUTAZIONE_TECNICO_RESPONSABILE, CAU_IMPUTAZIONE_TERZISTI
                    Dim risorsa = New RisorsaPersona
                    risorsa.risorsaUmana = New RisorseUmane
                    risorsa.risorsaUmana.codice = Mat_Cod
                    risorsa.risorsaUmana = BuildRisorsaUmana(risorsa.risorsaUmana.codice, verbose, objParametri_Server)

                    attivita.risorse.Add(risorsa)
            End Select
        Next
    End Sub

    Private Function CreaDettagliIrrigazione(attivita As AgronicaCoreModelsSTD.attivita.Attivita, doseAcqua As Decimal) As List(Of DettaglioIrrigazione)

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

    Private Sub LeggiNoteIntervento(ByRef attivita As AgronicaCoreModelsSTD.attivita.Attivita, verbose As Boolean, objParametri_Server As AgronicaCoreParametri)

        attivita.noteIntervento = New List(Of AgronicaCoreModelsSTD.attivita.note_intervento.NoteIntervento)

        Dim oNote As New Note_Intervento_R
        Dim oGruppi As New Note_Intervento_Gruppi_R

        Dim NoteBiz As New AgronicaControlli_2010.STD_Note
        Dim noteList = NoteBiz.LeggiNote(attivita.tipo,
                                             meno1tutti_0nonVisibili_1soloVisibili:=-1,
                                             objParametri_Server)

        Dim objRicettexNote As New AgronicaCoreContabDAL.RicettexNote_R
        Dim DtRicettexNote As DataTable = objRicettexNote.Leggi(attivita.testataRicetta.Ricetta_Cod, attivita.codice,
                                                   0, AGRODATAINIZIO, AGRODATAFINE,
                                                   AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                   "", "",
                                                   objParametri_Server)

        For Each nota As DataRow In DtRicettexNote.Rows
            Dim Nota_Cod As Integer = nota.Item("nota_cod")
            Dim notaIntervento As New AgronicaCoreModelsSTD.attivita.note_intervento.NoteIntervento(Nota_Cod)

            Dim notaItem = noteList.Find(Function(p) p.codice = Nota_Cod)
            If notaItem IsNot Nothing Then
                If notaItem.noteInterventoGruppi IsNot Nothing Then
                    notaIntervento.noteInterventoGruppi = New AgronicaCoreModelsSTD.attivita.note_intervento.NoteInterventoGruppi(notaItem.noteInterventoGruppi.codice)
                End If

                If verbose Then
                    notaIntervento.descrizione = notaItem.descrizione
                End If
                'Else
                '    Dim dtNote = oNote.Leggi(Nota_Cod, 0, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
                '    If dtNote IsNot Nothing AndAlso dtNote.Rows.Count > 0 Then
                '        If verbose Then
                '            notaIntervento.descrizione = dtNote(0).Item("Nota_Des")
                '        End If

                '        Dim dtGruppi = oGruppi.Leggi(dtNote(0).Item("NotaGruppo_Cod"), AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
                '        If dtGruppi IsNot Nothing AndAlso dtGruppi.Rows.Count > 0 Then
                '            notaIntervento.noteInterventoGruppi = New AgronicaCoreModelsSTD.attivita.note_intervento.NoteInterventoGruppi(dtGruppi(0).Item("NotaGruppo_Cod"))

                '            If verbose Then
                '                notaIntervento.noteInterventoGruppi.descrizione = dtGruppi(0).Item("NotaGruppo_Des")
                '            End If

                '        End If
                '    End If
            End If

            attivita.noteIntervento.Add(notaIntervento)
        Next

    End Sub

    Private Sub LeggiRisorsaProdotto(ByVal piva As String, ByVal DettaglioDaLeggere As RisorsaProdotto, ByVal Dettagli As Ricette_Dettagli, ByVal DettaglioMagazzino As Ricette_Dettagli, ByVal DestinazioneMagazzino As Ricette_Destinazioni, ByVal superficieTrattataTotale As Decimal, ByVal acquaTotale As Decimal, ByVal attivita As AgronicaCoreModelsSTD.attivita.Attivita)
        Dim ProdottoCod As Integer

        If Dettagli.Mat_Cod = 0 Then
            ProdottoCod = Dettagli.Pro_Cod
        Else
            ProdottoCod = Math.Abs(CInt(If(Dettagli.Mat_Cod Is Nothing, 0, Dettagli.Mat_Cod)))
        End If

        If DettaglioDaLeggere.prodotto Is Nothing Then
            DettaglioDaLeggere.prodotto = New Prodotto(ProdottoCod) With {
                .elemCod = Dettagli.Elem_Cod
            }
        End If

        DettaglioDaLeggere.doseHaReale = Dettagli.Qta
        DettaglioDaLeggere.doseHlReale = Dettagli.Qta_Extra
        DettaglioDaLeggere.quantitaTotaleReale = Dettagli.Qta_Extra_Totale
        DettaglioDaLeggere.flagTipoDose = Dettagli.Mezzo_Det
        DettaglioDaLeggere.flagDoseQuantitaTotale = If(Dettagli.Udm_Cod_Extra <> 0, Dettagli.Udm_Cod_Extra, 10) 'DT: se non è applicabile, viene valorizzata con 10 (qta totale) (es: semina)
        DettaglioDaLeggere.unitaDiMisura = New UnitaDiMisura(Dettagli.Udm_Cod)
        DettaglioDaLeggere.unitaDiMisuraIndicata = If(Dettagli.Extra_Int <> 0, New UnitaDiMisura(Dettagli.Extra_Int), New UnitaDiMisura(Dettagli.Udm_Cod)) 'DT: se non è applicabile l'udm indicata, viene valorizzata come l'udm (es: semina)
        DettaglioDaLeggere.MagazziniMovimentazioni = LeggiRilevamentiMagazzino(ProdottoCod, DettaglioMagazzino, DestinazioneMagazzino, superficieTrattataTotale, acquaTotale, attivita)

    End Sub

    Public Function LeggiFabbricato(ByVal piva As String, ByVal sa_cod As Integer, ByVal codice As Integer, ByVal descrizione As String) As Fabbricato
        Return New Fabbricato With {
            .primaryKey = New Fabbricato.PK With {
                .centroAziendalePK = LeggiCentroAziendale(piva, sa_cod).primaryKey,
                .codice = codice
            },
            .descrizione = descrizione
        }
    End Function

    Public Function LeggiMagazziniEsterni(magazzinoEsternoCod_List As String, magazzinoEsternoDes_List As String, magazzinoEsternoDettagli_List As String) As List(Of Tuple(Of Fabbricato, Decimal, enum_MagazzinoEsterno_Tipo))

        Dim magazziniEsterni As New List(Of Tuple(Of Fabbricato, Decimal, enum_MagazzinoEsterno_Tipo))

        If magazzinoEsternoCod_List IsNot Nothing AndAlso magazzinoEsternoCod_List.Length > 0 AndAlso
                magazzinoEsternoDes_List IsNot Nothing AndAlso magazzinoEsternoDes_List.Length > 0 AndAlso
                magazzinoEsternoDettagli_List IsNot Nothing AndAlso magazzinoEsternoDettagli_List.Length > 0 Then

            Dim magazzinoEsternoCod_Array = magazzinoEsternoCod_List.Split("|")
            Dim magazzinoEsternoDes_Array = magazzinoEsternoDes_List.Split("|")
            Dim magazzinoEsternoDettagli_Array = magazzinoEsternoDettagli_List.Split("|")

            For i = 0 To magazzinoEsternoCod_Array.Length - 1
                Dim magazzinoEsternoCod As String = magazzinoEsternoCod_Array(i)
                Dim magazzinoEsternoDes As String = magazzinoEsternoDes_Array(i)
                Dim magazzinoEsternoQta = CDec(magazzinoEsternoDettagli_Array(i))

                Dim arrChiavi As String() = magazzinoEsternoCod.Split("-")

                If arrChiavi IsNot Nothing Then

                    Dim MagazzinoEsterno_Tipo = enum_MagazzinoEsterno_Tipo.Agenzia

                    If arrChiavi.Count = 4 AndAlso arrChiavi(3) = enum_MagazzinoEsterno_Tipo.Uso_da_Terzi Then
                        MagazzinoEsterno_Tipo = enum_MagazzinoEsterno_Tipo.Uso_da_Terzi
                    End If

                    Dim Piva As String = arrChiavi(0)
                    Dim Sa_Cod As Integer = CInt(arrChiavi(1))
                    Dim Id_Destinazione As Integer = CInt(arrChiavi(2))

                    Dim qta As Decimal = CDec(magazzinoEsternoDettagli_Array(i))
                    Dim Fabbricato = New Fabbricato With {
                            .primaryKey = New Fabbricato.PK With {
                                .centroAziendalePK = LeggiCentroAziendale(Piva, Sa_Cod).primaryKey,
                                .codice = Id_Destinazione
                            },
                            .descrizione = magazzinoEsternoDes
                        }

                    magazziniEsterni.Add(Tuple.Create(Fabbricato, magazzinoEsternoQta, MagazzinoEsterno_Tipo))

                End If

            Next
        End If

        Return magazziniEsterni

    End Function

    Public Function LeggiCentroAziendale(ByVal piva As String, ByVal sa_cod As Integer) As CentroAziendale
        Return New CentroAziendale(New CentroAziendale.PK(sa_cod, piva))
    End Function

    Public Function GetTestataRicetta(ByVal attivita As AgronicaCoreModelsSTD.attivita.Attivita, ByVal Ricetta_Cod As Integer, ByVal Ricetta_Des As String, ByVal Ricetta_Des_Long As String,
                                     ByVal Ricetta_Numero As String, ByVal Note As String, ByVal Data_Da As Date,
                                      ByVal Data_A As Date, ByVal Lav_Cod As Integer, ByVal Programmazione_Cod As Integer, ByVal Piva As String,
                                      ByVal verbose As Boolean, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                      ByVal objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                      ByVal objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As AgronicaCoreModelsSTD.attivita.TestataRicetta

        Dim testataRicetta As New AgronicaCoreModelsSTD.attivita.TestataRicetta With {
            .Ricetta_Cod = Ricetta_Cod,
            .Ricetta_Des = Ricetta_Des,
            .Ricetta_Des_Long = Ricetta_Des_Long,
            .Ricetta_Numero = Ricetta_Numero,
            .Note = Note,
            .Data_Da = Data_Da,
            .Data_A = Data_A
            }

        'Valorizzo il Pua_Cod solo se sono in una ricetta di tipo PUA e con LAVCOD_DISTRIBUZIONE_AMMENDANTI
        If Lav_Cod = LAVCOD_DISTRIBUZIONE_AMMENDANTI AndAlso
            attivita.tipo = Tipo_Attivita.Ricetta AndAlso
            attivita.tipoRicetta = Tipo_Ricetta.PianoDistribuzionePua AndAlso
            attivita.stato = Stati.Da_Eseguire Then

            Dim objPUA As New AgronicaCorePUA_DAL.PUA_Testata_R

            testataRicetta.pua = New Pua(Programmazione_Cod)

            Dim Dt As DataTable = objPUA.Leggi(0, Programmazione_Cod, Piva, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server)

            If Not IsNothing(Dt) AndAlso Dt.Rows.Count = 1 Then

                Dim InfoOperazione As InfoOperazione = GetInfoOperazione(Lav_Cod, AgronicaCoreModelsSTD.attivita.Attivita.Tipo_Attivita.Ricetta)

                testataRicetta.pua.disciplinare = Utility.BuildDisciplinare(InfoOperazione, Dt(0)("Regolamento_Cod"), 0, attivita.utilizzoTerreno, Lav_Cod, testataRicetta.Data_Da, verbose, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)

                If verbose Then
                    testataRicetta.pua.descrizione = "PUA " + String.Format("{0:00000}", Dt(0)("PUA_Anno"))
                End If
            End If

        Else
            testataRicetta.pua = Nothing
        End If

        Return testataRicetta
    End Function

    ''' <summary>
    ''' Crea una attivita di scarico fittizia partendo dalla operazione di Campagna (necessario per memorizzare il Magazzino Esterno nelle Ricette/Brogliaccio)
    ''' </summary>
    Private Function CreaAttivitaScaricoFittizia(ByVal prodotto As Prodotto, ByVal rilevamentoMagazzino As RilevamentoDiMagazzino, ByVal magazziniEsterni As Tuple(Of Fabbricato, Decimal, enum_MagazzinoEsterno_Tipo), ByVal attivita As AgronicaCoreModelsSTD.attivita.Attivita) As AgronicaCoreModelsSTD.attivita.Attivita

        Dim attivitaScarico = attivita.Clona

        attivitaScarico.codice = "0"

        attivitaScarico.codiceOperazioneRicetta = "0"

        attivitaScarico.descrizione = ""

        attivitaScarico.job = New AgronicaCoreModelsSTD.attivita.Lavorazione(LAVCOD_SCARICO, "")

        attivitaScarico.risorse = New List(Of Risorsa)

        Dim risorsaRegistrazioneScarico = New AgronicaCoreModelsSTD.attivita.risorse.RisorsaRegistrazione()

        risorsaRegistrazioneScarico.prodotto = prodotto

        Dim rilevamentoMagazzinoScarico = rilevamentoMagazzino.Clona

        rilevamentoMagazzinoScarico.Magazzino = magazziniEsterni.Item1

        rilevamentoMagazzinoScarico.Qta = magazziniEsterni.Item2

        risorsaRegistrazioneScarico.MagazziniMovimentazioni = New List(Of RilevamentoDiMagazzino)

        risorsaRegistrazioneScarico.MagazziniMovimentazioni.Add(rilevamentoMagazzinoScarico)

        attivitaScarico.risorse.Add(risorsaRegistrazioneScarico)

        Return attivitaScarico
    End Function


    Private Function GetQuantitaSuImpiantiDettaglioTrattamento(ByVal dettaglioTrattamento As dettagli.DettaglioTrattamento, ByVal dettaglioCampagna As Ricette_Dettagli,
                                                               ByVal listaImpianti As Dictionary(Of String, Tuple(Of EsercizioCDC, List(Of Ricette_Destinazioni)))) As List(Of QuantitaSuImpianto)


        Dim quantitaSuImpianti As New List(Of QuantitaSuImpianto)

        For Each imp In listaImpianti.Values

            Dim ricetta_destinazioni As List(Of Ricette_Destinazioni) = imp.Item2.FindAll(Function(destinazione) destinazione.Ricetta_SuperUser = dettaglioCampagna.Ricetta_SuperUser AndAlso
                                                                                                                 destinazione.Ricetta_Cod = dettaglioCampagna.Ricetta_Cod AndAlso
                                                                                                                 destinazione.Ricetta_Operazione_Cod = dettaglioCampagna.Ricetta_Operazione_Cod AndAlso
                                                                                                                 destinazione.Ricetta_Dettaglio_Cod = dettaglioCampagna.Ricetta_Dettaglio_Cod)

            If Not IsNothing(ricetta_destinazioni) AndAlso ricetta_destinazioni.Count > 0 Then

                For Each ricetta_destinazione In ricetta_destinazioni

                    Dim quantitaSuImpianto As New QuantitaSuImpianto With {
                            .esercizioCDC = imp.Item1,
                            .Prodotto = dettaglioTrattamento.prodotto,
                            .Qta = ricetta_destinazione.Qta
                    }

                    If Not IsNothing(dettaglioTrattamento.MagazziniMovimentazioni) AndAlso dettaglioTrattamento.MagazziniMovimentazioni.Count = 1 Then
                        quantitaSuImpianto.Lotto = dettaglioTrattamento.MagazziniMovimentazioni(0).Lotto
                        quantitaSuImpianto.Magazzino = dettaglioTrattamento.MagazziniMovimentazioni(0).Magazzino
                    End If

                    quantitaSuImpianti.Add(quantitaSuImpianto)

                Next

            End If

        Next

        Return quantitaSuImpianti
    End Function

    Private Function LeggiRilevamentiMagazzino(ByVal codiceProdotto As Integer, ByVal DettaglioMagazzino As Ricette_Dettagli, ByVal DestinazioneMagazzino As Ricette_Destinazioni,
                                               ByVal superficieTrattataTotale As Decimal, ByVal acquaTotale As Decimal, ByVal attivita As AgronicaCoreModelsSTD.attivita.Attivita) As List(Of RilevamentoDiMagazzino)

        Dim MagazziniMovimentazioni As List(Of RilevamentoDiMagazzino) = Nothing

        If IsNothing(DettaglioMagazzino) Then
            MagazziniMovimentazioni = New List(Of RilevamentoDiMagazzino)
            Return MagazziniMovimentazioni
        End If

        Dim qtaTrasformata As Decimal = GetDosePerMagazzino(DettaglioMagazzino.Qta, DettaglioMagazzino.Udm_Cod)

        If DestinazioneMagazzino.Id_Reg <> 0 Then

            MagazziniMovimentazioni = New List(Of RilevamentoDiMagazzino)

            Dim rilevamentoMagazzino = New RilevamentoDiMagazzino With {
                .Lotto = DettaglioMagazzino.Lotto,
                .Magazzino = LeggiFabbricato(DestinazioneMagazzino.Piva, DestinazioneMagazzino.Sa_Cod, DestinazioneMagazzino.Id_Reg, ""),
                .udm = New UnitaDiMisura(DettaglioMagazzino.Udm_Cod) With {
                    .descrizione = ""
                },
                .Qta = DettaglioMagazzino.Qta,
                .Prodotto = New Prodotto(codiceProdotto) With {
                    .elemCod = DettaglioMagazzino.Elem_Cod,
                    .descrizione = "",
                    .unitaDiMisura = New UnitaDiMisura(DettaglioMagazzino.Udm_Cod) With {
                        .descrizione = ""
                    }
                },
                .TipoRilevamento = RilevamentoDiMagazzino.RilevamentoMagazzinoTipo.giacenza,
                .Descrizione = "" & "[" + DestinazioneMagazzino.Id_Reg.ToString & "]",
                .doseHaIndicata = If(superficieTrattataTotale <> 0, qtaTrasformata / superficieTrattataTotale, 0),
                .doseHlIndicata = If(acquaTotale <> 0, qtaTrasformata / acquaTotale, 0)
            }

            If DestinazioneMagazzino.MagazzinoEsterno_Cod <> "" Then
                Dim qtaMagazzinoInterno As Decimal = DettaglioMagazzino.Qta

                Dim magazziniEsterniQta As List(Of Tuple(Of Fabbricato, Decimal, enum_MagazzinoEsterno_Tipo)) = LeggiMagazziniEsterni(DestinazioneMagazzino.MagazzinoEsterno_Cod, DestinazioneMagazzino.MagazzinoEsterno_Des, DestinazioneMagazzino.MagazzinoEsterno_Dettagli)
                If magazziniEsterniQta IsNot Nothing AndAlso magazziniEsterniQta.Count > 0 Then
                    For Each magazzinoEsternoQta In magazziniEsterniQta

                        Dim qtaTrasformataMagazzinoEsterno As Decimal = GetDosePerMagazzino(magazzinoEsternoQta.Item2, If(DettaglioMagazzino.Extra_Int <> 0, DettaglioMagazzino.Extra_Int, DettaglioMagazzino.Udm_Cod))

                        Dim rilevamentoMagazzinoMagazzinoEsterno = rilevamentoMagazzino.Clona

                        rilevamentoMagazzinoMagazzinoEsterno.Qta = magazzinoEsternoQta.Item2
                        rilevamentoMagazzinoMagazzinoEsterno.doseHaIndicata = If(superficieTrattataTotale <> 0, qtaTrasformataMagazzinoEsterno / superficieTrattataTotale, 0)
                        rilevamentoMagazzinoMagazzinoEsterno.doseHlIndicata = If(acquaTotale <> 0, qtaTrasformataMagazzinoEsterno / acquaTotale, 0)

                        If magazzinoEsternoQta.Item3 = enum_MagazzinoEsterno_Tipo.Agenzia Then
                            rilevamentoMagazzinoMagazzinoEsterno.Agenzia = magazzinoEsternoQta.Item1

                            rilevamentoMagazzinoMagazzinoEsterno.registrazioniCollegate = New List(Of AgronicaCoreModelsSTD.attivita.Attivita)
                        Else
                            rilevamentoMagazzinoMagazzinoEsterno.Agenzia = Nothing

                            rilevamentoMagazzinoMagazzinoEsterno.registrazioniCollegate = New List(Of AgronicaCoreModelsSTD.attivita.Attivita)

                            Dim attivitaScarico As AgronicaCoreModelsSTD.attivita.Attivita = CreaAttivitaScaricoFittizia(rilevamentoMagazzino.Prodotto, rilevamentoMagazzinoMagazzinoEsterno, magazzinoEsternoQta, attivita)

                            rilevamentoMagazzinoMagazzinoEsterno.registrazioniCollegate.Add(attivitaScarico)
                        End If

                        MagazziniMovimentazioni.Add(rilevamentoMagazzinoMagazzinoEsterno)

                        qtaMagazzinoInterno -= magazzinoEsternoQta.Item2
                    Next

                    If qtaMagazzinoInterno > 0 Then

                        Dim qtaTrasformataMagazzinoInterno As Decimal = GetDosePerMagazzino(qtaMagazzinoInterno, DettaglioMagazzino.Udm_Cod)

                        rilevamentoMagazzino.Agenzia = Nothing
                        rilevamentoMagazzino.Qta = qtaMagazzinoInterno
                        rilevamentoMagazzino.doseHaIndicata = If(superficieTrattataTotale <> 0, qtaTrasformataMagazzinoInterno / superficieTrattataTotale, 0)
                        rilevamentoMagazzino.doseHlIndicata = If(acquaTotale <> 0, qtaTrasformataMagazzinoInterno / acquaTotale, 0)

                        MagazziniMovimentazioni.Add(rilevamentoMagazzino)
                    End If
                End If
            Else
                MagazziniMovimentazioni.Add(rilevamentoMagazzino)
            End If

        End If

        Return MagazziniMovimentazioni

    End Function

#Region "Funzioni private di estrazione di parti del vecchio modello"

    Private Function GetPrimoImpianto(db As Gias_DeveloperServer_Entities, ricetta_operazione As Ricette_Operazioni, causale As String) As Ricette_Destinazioni

        Dim primoImpianto As Ricette_Destinazioni = Nothing

        Dim Dettaglio As Ricette_Dettagli = GetMovimentoFromCausale(db, ricetta_operazione, causale)


        If Dettaglio IsNot Nothing Then

            Dim listaDestinazioni As List(Of Ricette_Destinazioni) = EFRicette.ReadRicettaDestinazioni(db, ricetta_operazione.Ricetta_SuperUser, ricetta_operazione.Ricetta_Cod, ricetta_operazione.Ricetta_Operazione_Cod, Dettaglio.Ricetta_Dettaglio_Cod)

            If listaDestinazioni IsNot Nothing AndAlso listaDestinazioni.Count > 0 Then

                primoImpianto = listaDestinazioni(0)

            End If
        End If

        Return primoImpianto

    End Function

    Private Function GetMovimentoFromCausale(db As Gias_DeveloperServer_Entities, ricetta_operazione As Ricette_Operazioni, causale As String) As Ricette_Dettagli

        Dim listaDettagli As List(Of Ricette_Dettagli) = EFRicette.ReadRicettaDettagli(db, ricetta_operazione.Ricetta_SuperUser, ricetta_operazione.Ricetta_Cod, ricetta_operazione.Ricetta_Operazione_Cod)

        Dim Dettaglio As Ricette_Dettagli = listaDettagli.FindAll(Function(c) (c.Cau_Mov = causale)).FirstOrDefault
        Return Dettaglio

    End Function

    Private Function ClonaDestinazione(ricettaDestinazioneOrig As Ricette_Destinazioni) As Ricette_Destinazioni
        Try
            Dim output As String = JsonConvert.SerializeObject(ricettaDestinazioneOrig)
            Dim ricettaDestinazioneCloned As Ricette_Destinazioni = JsonConvert.DeserializeObject(output)
            Return ricettaDestinazioneCloned

        Catch ex As Exception
            Throw New Exception("Ricette_Destinazioni.Clona: " + ex.Message)
        End Try

    End Function

#End Region

#Region "Controlli"
    Public Shared Function controlloMagazziniRicettexApp_Menu(listaRicette As List(Of AgronicaCoreDTOStd.InData.Agenda.APP_Ricette_Operazioni),
                                                              objParametri_Utenti As AgronicaCoreParametri,
                                                              objParametri_Server As AgronicaCoreParametri,
                                                              objParametri_Super_Server As AgronicaCoreParametri
                                                              ) As List(Of ErroreGias)
        Dim listErrorixRicetta As New List(Of String)
        Dim listErroriRicetteMultiAttivita As New List(Of String)
        Dim listErroriRicetteNonGestite As New List(Of String)
        Dim listErrori As New List(Of String)

        Dim listErroriGias As New List(Of ErroreGias)

        Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
        Dim db As New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(EFConnString)

        Dim List_OP_NON_GESTITE_APP = STR_OP_NON_GESTITE_APP.Split(",").AsEnumerable().Select(Function(s) Convert.ToInt32(s)).ToList()

        Dim ricetteToRemove As New List(Of AgronicaCoreDTOStd.InData.Agenda.APP_Ricette_Operazioni)

        For Each Ricetta_Operazione In listaRicette

            Dim ricettaOP As AgronicaCoreEntityFramework_POCO.Ricette_Operazioni = AgronicaCoreContabDAL.EFRicette.ReadRicettaOperazione(db, objParametri_Server.PivaSuperUser, Ricetta_Operazione.Ricetta_Operazione_Cod)

            Dim currentAttivitaDes As String = ricettaOP.Ricetta_Operazione_Des

            Dim ricettaErrataDes As String = "• " & ricettaOP.Validita_Inizio & " - " & currentAttivitaDes

            If List_OP_NON_GESTITE_APP.Contains(ricettaOP.Lav_Cod) Then
                listErroriRicetteNonGestite.Add(ricettaErrataDes)
                AddErroreToRicettexApp(Ricetta_Operazione, ricetteToRemove)
            End If

            If ricettaOP.Raccoglitore_Cod <> 0 Then
                'NON GESTIAMO IL MULTI OPERAZIONE IN APP
                'AF TODO MSG
                listErroriRicetteMultiAttivita.Add(ricettaErrataDes)
                AddErroreToRicettexApp(Ricetta_Operazione, ricetteToRemove)

            Else

                listErrorixRicetta.Clear()

                Dim map As New RicettaToAttivita
                Dim attivita = map.RicettaOperazioneSuAttivita(ricettaOP, listParametriAggiuntivi:=Nothing, verbose:=True, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)

                esegui_ControlloMagazziniRicettexApp(attivita, listErrorixRicetta, listErrori, objParametri_Server)
                componiErrorexRicetta_controlloMagazziniRicettexApp(attivita, listErrorixRicetta, listErrori, True)

                If listErrorixRicetta.Count > 0 Then
                    AddErroreToRicettexApp(Ricetta_Operazione, ricetteToRemove)
                End If
            End If
        Next

        For Each ricetta In ricetteToRemove
            listaRicette.Remove(ricetta)
        Next

        listErroriGias = componiErroreFinale_controlloMagazziniRicettexApp(True, listErrori, listErroriRicetteMultiAttivita, listErroriRicetteNonGestite, ricetteToRemove.Count)

        Return listErroriGias

    End Function
#End Region


End Class
