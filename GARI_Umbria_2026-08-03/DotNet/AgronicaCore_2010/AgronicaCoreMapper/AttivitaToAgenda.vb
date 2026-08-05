Imports System.Reflection
Imports System.Net
Imports System.Transactions
Imports System.Web
Imports AgronicaControlli_2010
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreContabBIZ
Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDTOStd.InData.Agenda
Imports AgronicaCoreEsitoVerificaConformitaBIZ
Imports AgronicaCoreMapper.My.Resources.AgronicaCoreMapper
Imports AgronicaCoreMapper.Utility
Imports AgronicaCoreModello
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreModello.Utility_Agenda
Imports AgronicaCoreModelsSTD
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.attivita
Imports AgronicaCoreModelsSTD.attivita.Attivita
Imports AgronicaCoreModelsSTD.attivita.centri_di_costo
Imports AgronicaCoreModelsSTD.attivita.dettagli
Imports AgronicaCoreModelsSTD.attivita.dettagli.Opzioni_Raccolta
Imports AgronicaCoreModelsSTD.attivita.risorse
Imports AgronicaCoreModelsSTD.costanti
Imports AgronicaCoreModelsSTD.exceptions
Imports AgronicaCoreModelsSTD.metaschema
Imports AgronicaCoreModelsSTD.metaschema.utilizzi
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreVarieDAL
Imports Newtonsoft.Json

Public Class AttivitaToAgenda

    Public Function ScriviListaAttivitaToRaccoglitore(listaAttivitaAgende As List(Of (attivita As Attivita, agenda As Operazione_Agenda)),
                                                      lista_ParametriAggiuntivi As List(Of Parametri_Aggiuntivi_Attivita),
                                                      lista_Impianti As List(Of Impianto),
                                                      ByRef lista_Errori As List(Of ErroreGias),
                                                      agendeToDelete As List(Of Integer),
                                                      eseguiSoloVerificheConformita As Boolean,
                                                      creaCaricoMagazzinoXOriginePUA As Boolean,
                                                      objParametri_Super_Server As AgronicaCoreParametri,
                                                      objParametri_Server As AgronicaCoreParametri,
                                                      objParametri_Utenti As AgronicaCoreParametri
                                                      ) As List(Of Attivita)


        Dim listaAttivita As New List(Of Attivita)
        Dim listaXMLRicettaOperazione As New List(Of String)

        Dim currentAttivitaDes = ""

        Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
        Dim GiasContext As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities = Nothing

        Dim FlagConnessioneLocale, FlagTransazioneLocale As Boolean

        Dim trOpt As New TransactionOptions With {
            .Timeout = New TimeSpan(0, 10, 0),
            .IsolationLevel = IsolationLevel.ReadUncommitted
        }
        'Istanzio la transazione forzando l'uso di una nuova transazione
        Using ts As New TransactionScope(TransactionScopeOption.RequiresNew, trOpt)
            Dim OpenNewTransaction As Boolean = False

            Try
                GiasContext = New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(EFConnString)
                'Open the contextObject connection state explicitly
                GiasContext.Database.Connection.Open()

                ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, objParametri_Server, System.Data.IsolationLevel.ReadUncommitted)

                Dim nrIterazioni As Integer
                Select Case eseguiSoloVerificheConformita
                    Case True
                        'Se va tutto liscio senza warning, il codice si "autogestisce" e richiama la stessa funzione procedendo con le scritture,
                        'Altrimenti sparo fuori l'errore / la lista di warning
                        nrIterazioni = 2
                    Case False
                        'La funzione ScriviListaAttivitaToRaccoglitore è stata chiamata una seconda volta da interfaccia.
                        'L'utente ha dato risposto "SI" ad una lista di Warning, procedo con la scrittura
                        nrIterazioni = 1

                End Select

                'Elenco di Agende collegate (Mov_Dettagli_Riferimenti) che sono da eliminare. Per ora solo nel caso di Prodotti provienti da Magazzini Esterni
                'dove vengono fatti i Carichi/Scarichi associati
                Dim lista_Agende_Rif_ToDelete As New List(Of Movimento_Dettaglio_Riferimento)

                'Recupero i vecchi costi CdG prima che vengano cancellati
                Dim mdr_Rif As New Agenda_Movimenti_Dettagli_Riferimenti_Helper()
                Dim lista_mdRif As New List(Of Movimento_Dettaglio_Riferimento)
                Dim objAgendaScrivi As New Agenda_Operazione_Helper

                For i = 1 To nrIterazioni

                    If i = nrIterazioni AndAlso Not eseguiSoloVerificheConformita Then
                        'elimino definitivamente le agende che in un salvataggio multicentro non sono state riconfermate (causa deselezione propri impianti/centri)
                        Try
                            If listaAttivitaAgende(0).attivita.tipo = Tipo_Attivita.QuadernoDiCampagna Then
                                If agendeToDelete IsNot Nothing AndAlso agendeToDelete.Count > 0 Then
                                    For Each agendaToDelete In agendeToDelete

                                        'Recupero i vecchi costi CdG prima che vengano cancellati
                                        lista_mdRif.AddRange(mdr_Rif.LeggiRiferimentiAgenda(listaAttivitaAgende(0).attivita.centroAziendale.primaryKey.partitaIva, 0, agendaToDelete, 0, "", objParametri_Server))

                                        Dim CancellataOperazione As Boolean = objAgendaScrivi.Cancella(listaAttivitaAgende(0).attivita.centroAziendale.primaryKey.partitaIva, 0, agendaToDelete,
                                                    False,
                                                    objParametri_Server, logCancellazione:=True)


                                    Next
                                End If
                            End If
                        Catch ex As Exception
                        End Try


                        If Not IsNothing(lista_mdRif) AndAlso lista_mdRif.Count > 0 Then
                            Dim Agenda_Rif_Carico_Scarico = (From rif In lista_mdRif
                                                             Where Utility.Carico_Scarico_Collegate_Al_QdC(rif)
                                                             Select rif)

                            If Not IsNothing(Agenda_Rif_Carico_Scarico) AndAlso Agenda_Rif_Carico_Scarico.Count > 0 Then
                                lista_Agende_Rif_ToDelete.AddRange(Agenda_Rif_Carico_Scarico.ToList())
                            End If
                        End If

                        For Each attivitaAgenda In listaAttivitaAgende

                            Dim agenda As Operazione_Agenda = attivitaAgenda.agenda

                            If agenda.Id_Agenda <> 0 Then
                                Dim Agenda_Rif = mdr_Rif.LeggiRiferimentiAgenda(agenda.Piva, 0, agenda.Id_Agenda, 0, "", objParametri_Server)

                                If Not IsNothing(Agenda_Rif) AndAlso Agenda_Rif.Count > 0 Then
                                    Dim Agenda_Rif_Carico_Scarico = (From rif In Agenda_Rif
                                                                     Where Utility.Carico_Scarico_Collegate_Al_QdC(rif)
                                                                     Select rif)

                                    If Not IsNothing(Agenda_Rif_Carico_Scarico) AndAlso Agenda_Rif_Carico_Scarico.Count > 0 Then
                                        lista_Agende_Rif_ToDelete.AddRange(Agenda_Rif_Carico_Scarico.ToList())
                                    End If
                                End If
                            End If
                        Next

                        If Not IsNothing(lista_Agende_Rif_ToDelete) AndAlso lista_Agende_Rif_ToDelete.Count > 0 Then
                            For Each Agende_rif In lista_Agende_Rif_ToDelete
                                objAgendaScrivi.Cancella(Agende_rif.Piva_Rif, Agende_rif.Sa_Cod_Rif, Agende_rif.Id_Agenda_Rif,
                                                               False,
                                                               objParametri_Server, logCancellazione:=False)
                            Next
                        End If

                    End If

                    Dim ultimaAttivita = listaAttivitaAgende.Last().attivita

                    Dim ricetta_cod = 0

                    'id minore = id minore fra le agende che sopravvivono al salvataggio. Se non ce n'è neanche uno, 0, vuol dire che sono stati deselezionati tutti gli impianti precedenti
                    Dim minIdAgenda As Integer = Integer.MaxValue

                    For Each item In listaAttivitaAgende

                        Dim isLast As Boolean = item.attivita.Equals(ultimaAttivita)

                        currentAttivitaDes = item.attivita.job.descrizione
                        Dim id_Agenda = ScriviAttivitaToAgenda(item.attivita, item.agenda,
                                                               lista_ParametriAggiuntivi, lista_Impianti,
                                                               objParametri_Super_Server, objParametri_Server, objParametri_Utenti,
                                                               eseguiSoloVerificheConformita, currentAttivitaDes, creaCaricoMagazzinoXOriginePUA,
                                                               listaXMLRicettaOperazione, lista_Errori,
                                                               GiasContext:=GiasContext,
                                                               OpenNewTransaction:=OpenNewTransaction,
                                                               isLast,
                                                               agendeToDelete)

                        If id_Agenda > 0 AndAlso id_Agenda < minIdAgenda Then
                            minIdAgenda = id_Agenda
                        End If

                        'Se passo il flag eseguiVerificheConformita non mi viene restiuito nessun ID_agenda, salto la parte sotto
                        If Not (eseguiSoloVerificheConformita) Then
                            If item.attivita.tipo = Tipo_Attivita.QuadernoDiCampagna Then
                                item.attivita.codice = id_Agenda

                            ElseIf item.attivita.tipo = Tipo_Attivita.Ricetta AndAlso isLast Then
                                ricetta_cod = id_Agenda
                            End If
                            listaAttivita.Add(item.attivita)
                        End If

                        If Not IsNothing(lista_Errori) AndAlso lista_Errori.FindAll(Function(c) (c.severity = ErroreGias_Severity.Bloccante)).Count > 0 Then
                            Exit Function
                        End If
                    Next

                    If lista_mdRif IsNot Nothing AndAlso lista_mdRif.Count > 0 Then

                        'Riscrivo i vecchi costi, associandoli però all'agenda con id più piccolo a parità di raccoglitore (ovvero tutte quelle che arrivano in listaAttivitaAgende)
                        Dim mdRif_helper As New Agenda_Movimenti_Dettagli_Riferimenti_Helper
                        For Each mdRif As Movimento_Dettaglio_Riferimento In lista_mdRif
                            If mdRif.Lav_Cod_Rif = LAVCOD_COSTI_CDG Then
                                mdRif.Id_Agenda = minIdAgenda
                                mdRif_helper.Scrivi(mdRif, objParametri_Server)
                                Exit For
                            End If
                        Next
                    End If

                    If ricetta_cod <> 0 Then
                        For Each item In listaAttivitaAgende
                            If item.attivita.tipo = Tipo_Attivita.Ricetta Then
                                item.attivita.testataRicetta.Ricetta_Cod = ricetta_cod
                            End If
                        Next
                    End If

                    'Se dopo aver fatto le verifiche di conformita di tutte le attivita trovo warning,
                    'li sparo fuori e chiedo all'utente se procedere (SI/NO)
                    If lista_Errori IsNot Nothing AndAlso lista_Errori.Count > 0 Then
                        Exit Function
                    Else
                        'Se non ci sono stati errori, rieseguo il ciclo procedendo con la scrittura
                        eseguiSoloVerificheConformita = False

                    End If
                Next

                ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)

                ts.Complete()

            Catch ex As GiasException

                If objParametri_Server.objTransazione IsNot Nothing Then
                    ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
                End If

                Throw ex
            Catch ex As Exception

                If objParametri_Server.objTransazione IsNot Nothing Then
                    ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
                End If

                Throw New Exception(currentAttivitaDes & ": " & ex.Message, ex)
            Finally

                ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

                ts.Dispose()

                If GiasContext IsNot Nothing AndAlso GiasContext.Database.Connection.State = ConnectionState.Open Then
                    GiasContext.Database.Connection.Close()
                    GiasContext.Dispose()
                End If

            End Try
        End Using

        Return listaAttivita

    End Function

    Public Function CheckConformita(attivita As Attivita) As List(Of ErroreGias)

        Dim listaErrori As New List(Of ErroreGias)

        If attivita.disciplinare Is Nothing Then
            Dim erroreGias As New ErroreGias With {
                .severity = ErroreGias_Severity.Bloccante,
                .tipo = ErroreGias_Tipo.Generico,
                .messaggio = attivita.job.descrizione & ": " & My.Resources.AgronicaCoreMapper.NessunDisciplinareIndicato
            }
            listaErrori.Add(erroreGias)
            Return listaErrori
        End If

        Dim veg_cod = GetVegCodFromUtilizzoTerreno(attivita.utilizzoTerreno)
        If veg_cod <= 0 Then
            Dim erroreGias As New ErroreGias With {
                .severity = ErroreGias_Severity.Bloccante,
                .tipo = ErroreGias_Tipo.Generico,
                .messaggio = attivita.job.descrizione & ": " & My.Resources.AgronicaCoreMapper.NessunaSpecieIndicata
            }
            listaErrori.Add(erroreGias)
            Return listaErrori
        End If

        'Dim Agenda As Operazione_Agenda
        'Agenda = MappaAttivitaToAgenda(attivita, obj

        'If IsNothing(Agenda) Then
        '    Exit Sub
        'End If

        Dim Dpi_Cod As Integer = 0
        Dim Dpi_PubblicoPrivato As Integer = 0
        Dim Bio As Boolean = False

        ''una volta creato l'oggetto agenda posso invocare il suo metodo che mi genera l'xml
        'Dim Helper As New Agenda_Operazione_Helper
        'Dim strXML As String = ""
        'If IsTrattamento = True Then
        '    strXML = Helper.GeneraXML_CAU_TRATTAMENTO(Agenda, Dpi_Cod, Dpi_PubblicoPrivato)
        'End If
        'If IsFertilizzazione = True Then
        '    strXML = Helper.GeneraXML_CAU_LAVORAZIONI(Agenda, Dpi_Cod, Dpi_PubblicoPrivato)
        'End If

        'strXML = Replace(strXML, "TipoOperazioneDB=""1""", "TipoOperazioneDB=""" & Agenda.Tipo_Operazione & """")


        Return listaErrori

    End Function

    Public Function MappaAttivitaToAgenda(attivita As Attivita,
                                          objParametri_Super_Server As AgronicaCoreParametri,
                                          objParametri_Server As AgronicaCoreParametri,
                                          objParametri_Utenti As AgronicaCoreParametri,
                                          eseguiVerificheConformita As Boolean) As Operazione_Agenda

        '----- AGENDA
        Dim Agenda As New Operazione_Agenda With {
            .Data = attivita.inizio,
            .Piva = attivita.centroAziendale.primaryKey.partitaIva,
            .Sa_Cod = attivita.centroAziendale.primaryKey.codice,
            .Lav_Cod = If(attivita.job.primaryKey.classType.Equals(ClassType.Lavorazione), attivita.job.primaryKey.codice, LAVCOD_COSTI_CDG),
            .Raccoglitore_Cod = attivita.raccoglitore,
            .Invia_App = If(attivita.inviaRicetta, 1, 0),
            .Stato_Cod = attivita.statoWorkflow,
            .DaRemoto = If(attivita.daRemoto, 1, 0),
            .Origine = attivita.origine,
            .Blocco_Flag = If(attivita.blocco IsNot Nothing, attivita.blocco.tipo, 0),
            .Blocco_Username = If(attivita.blocco IsNot Nothing, attivita.blocco.utente, ""),
            .Blocco_Data = If(attivita.blocco IsNot Nothing, attivita.blocco.data, AGRODATAINIZIO)
        }

        Agenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
        If attivita.codice <> "" AndAlso attivita.codice <> "0" Then 'TODO_DT IMPORTANTE: i valori negativi sono ok, arrivano da APP --> verificare con Carlo. I codice negativi su scriviagenda vengono considerati come nuovi da inserire
            Agenda.Id_Agenda = attivita.codice
            Agenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica
        End If

        '----- tipo operazione, cau_mov, elem_cod, baseCode, topCode 
        Dim InfoOperazione As InfoOperazione = GetInfoOperazione(Agenda.Lav_Cod, attivita.tipo)
        Agenda.BaseCode = InfoOperazione.BaseCode
        Agenda.TopCode = InfoOperazione.TopCode

        If InfoOperazione.IsRaccolta Then
            Dim isRicettaBrogliaccio As Boolean = Not (attivita.tipo.Equals(Tipo_Attivita.QuadernoDiCampagna))
            If isRicettaBrogliaccio Then
                Dim dettagliRaccolta As List(Of dettagli.DettaglioRaccolta) = attivita.risorse.FindAll(Function(c) (c.classType = ClassType.DettaglioRaccolta)).ConvertAll(Function(obj1) CType(obj1, dettagli.DettaglioRaccolta))

                For Each dettaglioRaccolta In dettagliRaccolta
                    dettaglioRaccolta.Opzioni_Raccolta.Ripartizione = Opzioni_Raccolta.enum_Ripartizione_Raccolta.AUTO_SUPERFICIE
                    dettaglioRaccolta.Opzioni_Raccolta.GenerazioneLotto = Opzioni_Raccolta.enum_Generazione_Lotto_Raccolta.MANUALE
                    dettaglioRaccolta.Opzioni_Raccolta.Modalita = Opzioni_Raccolta.enum_Modalita_Raccolta.MECCANICA
                Next
            End If
        End If

        '----- SUPERFICIE TRATTATA TOTALE
        Dim SuperficieTrattataTotale As Decimal = CalcolaSuperficieTrattataTotale(attivita, InfoOperazione)
        If SuperficieTrattataTotale <= 0 AndAlso Not InfoOperazione.isNonUtilizzo AndAlso Not InfoOperazione.IsVisita AndAlso Not RilievoSenzaImpianti(attivita) Then
            If InfoOperazione.TipoCentroDiCosto = centri_di_costo.Tipo.ProdottoDaTrattare Then
                Throw New GiasException(My.Resources.AgronicaCoreMapper.QtaProdottoTrattataMaggioreZero)
            Else
                Throw New GiasException(My.Resources.AgronicaCoreMapper.SuperficieTrattataMaggioreZero)
            End If
        End If

        '----- DES_LIB
        Agenda.Des_Lib = CreaDescrizione(attivita, objParametri_Server)

        Agenda.Id_Attivita = CreaAttivitaPersonalizzata(attivita, Agenda)

        '----- NOTE
        Agenda.Note = CreaNote(attivita, Agenda, objParametri_Server)

        '----- MOVIMENTI
        Agenda.Movimenti = New List(Of Movimento)

        '----- COSTI ACCESSORI
        Dim movimentiRisorse As List(Of Movimento) = CreaMovimentiCostiAccessori(attivita, Agenda, objParametri_Server)
        Agenda.Movimenti.AddRange(movimentiRisorse)

        '----- ASSEGNATARI VISITA
        If InfoOperazione.IsVisita Then
            Dim movimentiAssegnatariVisita As List(Of Movimento) = CreaMovimentiAssegnatariVisita(attivita, Agenda, objParametri_Server)
            Agenda.Movimenti.AddRange(movimentiAssegnatariVisita)

            If attivita.attivitaCollegate IsNot Nothing AndAlso attivita.attivitaCollegate.Count > 0 Then
                Agenda.Stato_Cod = StatiWorkflowQdC.Eseguito
            End If
        End If

        '----- MOVIMENTO CAMPAGNA
        Dim MovimentoCampagna As Movimento = CreaMovimentoCampagna(attivita, Agenda, InfoOperazione)
        MovimentoCampagna.Movimenti_Dettagli = New List(Of Movimento_Dettaglio)
        MovimentoCampagna.Movimenti_Dettagli_Tecnici = New List(Of Movimento_Dettaglio_Tecnico)
        Agenda.Movimenti.Add(MovimentoCampagna)

        '----- MOVIMENTO DETTAGLIO TECNICO X ACQUA
        Dim acquaTotale As Decimal = 0
        Dim Movimento_Dettaglio_Tecnico_Acqua As Movimento_Dettaglio_Tecnico = CreaMovimentoDettaglioTecnicoAcqua(attivita, Agenda, InfoOperazione, SuperficieTrattataTotale, acquaTotale)
        If Movimento_Dettaglio_Tecnico_Acqua IsNot Nothing Then
            MovimentoCampagna.Movimenti_Dettagli_Tecnici.Add(Movimento_Dettaglio_Tecnico_Acqua)
        End If

        '----- MOVIMENTO DETTAGLIO TECNICO X CAUSALE (ora solo Rilievo produzione prevista)
        Dim Movimenti_Dettaglio_Tecnico_Causali As List(Of Movimento_Dettaglio_Tecnico) = CreaMovimentiDettagliTecniciCausali(attivita, Agenda, InfoOperazione)
        If Movimenti_Dettaglio_Tecnico_Causali IsNot Nothing AndAlso Movimenti_Dettaglio_Tecnico_Causali.Count > 0 Then
            MovimentoCampagna.Movimenti_Dettagli_Tecnici.AddRange(Movimenti_Dettaglio_Tecnico_Causali)
        End If

        '----- MOVIMENTI DETTAGLI 
        Dim Movimenti_Dettaglio As List(Of Movimento_Dettaglio) = CreaMovimentiDettaglio(attivita, Agenda, InfoOperazione, MovimentoCampagna, SuperficieTrattataTotale, acquaTotale, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
        MovimentoCampagna.Movimenti_Dettagli.AddRange(Movimenti_Dettaglio)

        If InfoOperazione.IsRaccolta Then
            Dim dettaglioRaccolta = CType(attivita.risorse.Find(Function(r) (r.classType = ClassType.DettaglioRaccolta)), DettaglioRaccolta)
            If Not IsNothing(dettaglioRaccolta) Then
                MovimentoCampagna.Mezzo = dettaglioRaccolta.Opzioni_Raccolta.Ripartizione
                MovimentoCampagna.Modalita = dettaglioRaccolta.Opzioni_Raccolta.Modalita
            End If
        End If

        If Not InfoOperazione.IsSemina AndAlso Not InfoOperazione.IsRaccolta AndAlso Not InfoOperazione.IsIrrigazione Then
            'DT: Modalità del movimento di campagna viene valorizzato con i dati del primo dettaglio
            If MovimentoCampagna.Movimenti_Dettagli IsNot Nothing AndAlso MovimentoCampagna.Movimenti_Dettagli.Count > 0 Then
                MovimentoCampagna.Modalita = MovimentoCampagna.Movimenti_Dettagli(0).Udm_Cod_Extra
            End If

            If Agenda.Lav_Cod = LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA OrElse Agenda.Lav_Cod = LAVCOD_REINNESCO_TRAPPOLE Then
                Dim dettaglioTrattamento = CType(attivita.risorse.Find(Function(r) (r.classType = ClassType.DettaglioTrattamento)), DettaglioTrattamento)
                If Not IsNothing(dettaglioTrattamento) Then
                    MovimentoCampagna.Mezzo = dettaglioTrattamento.ripartizioneTrappole
                End If
            Else
                MovimentoCampagna.Mezzo = 1 'DT: IMPORTANTE: il Mezzo viene sempre valorizzato a 1 per compatibilità con il pregresso (mettendo 0 non tornano le quantità a video)
            End If
        End If

        Dim wkt As String = BuildWkt(attivita.latitude, attivita.longitude)
        If Not String.IsNullOrEmpty(wkt) Then

            Dim GisWktSistemaRiferimento As String = "-1"
            Dim GisWktGps = "1"
            Dim GisLayerCod As Integer = 0
            Dim GisTipoEntita_cod As Integer = 0

            If InfoOperazione.IsRilievo Then
                GisLayerCod = enum_Gis_LayerElementiGrafici_std.AGENDA_RILIEVI
                GisTipoEntita_cod = enum_GIS2012_TipoEntita.Destinazione_Agenda

            ElseIf InfoOperazione.IsVisita Then
                GisLayerCod = enum_Gis_LayerElementiGrafici_std.Op_Agenda
                GisTipoEntita_cod = enum_GIS2012_TipoEntita.OpAgenda

            End If

            If GisLayerCod <> 0 Then

                Dim foundDestinazioni As Boolean = False

                For Each mov_dettaglio In MovimentoCampagna.Movimenti_Dettagli
                    For Each mov_destinazione In mov_dettaglio.Movimenti_Destinazioni
                        foundDestinazioni = True
                        With mov_destinazione
                            .GisWkt = wkt
                            .GisWktSistemaRiferimento = GisWktSistemaRiferimento
                            .GisWktGps = GisWktGps
                            .GisLayerCod = GisLayerCod
                            .GisTipoEntita_cod = GisTipoEntita_cod
                        End With
                    Next
                Next

                If Not foundDestinazioni Then
                    With Agenda
                        .GisWkt = wkt
                        .GisWktSistemaRiferimento = GisWktSistemaRiferimento
                        .GisWktGps = GisWktGps
                        .GisLayerCod = GisLayerCod
                        .GisTipoEntita_cod = GisTipoEntita_cod
                    End With
                End If

            End If

        End If

        '----- MOVIMENTO SCARICO
        Dim MovimentoScarico As Movimento = CreaMovimentoScarico(attivita, Agenda, InfoOperazione, eseguiVerificheConformita, objParametri_Server)
        If MovimentoScarico IsNot Nothing AndAlso MovimentoScarico.Movimenti_Dettagli IsNot Nothing AndAlso MovimentoScarico.Movimenti_Dettagli.Count > 0 Then
            MovimentoScarico.Mezzo = 1 'DT: IMPORTANTE: il Mezzo viene sempre valorizzato a 1 per compatibilità con il pregresso

            If InfoOperazione.IsSemina Then
                MovimentoScarico.Mezzo = 0
            End If

            Agenda.Movimenti.Add(MovimentoScarico)
        End If

        '----- MOVIMENTO CARICO
        Dim MovimentoCarico As Movimento = CreaMovimentoCarico(attivita, Agenda, InfoOperazione, eseguiVerificheConformita)
        If MovimentoCarico IsNot Nothing AndAlso MovimentoCarico.Movimenti_Dettagli IsNot Nothing AndAlso MovimentoCarico.Movimenti_Dettagli.Count > 0 Then
            MovimentoCarico.Mezzo = 1 'DT: IMPORTANTE: il Mezzo viene sempre valorizzato a 1 per compatibilità con il pregresso

            If InfoOperazione.IsRaccolta Then
                MovimentoCarico.Mezzo = 0
                '' Data e ora vengono già valorizzati nella funzione CreaMovimentiDettaglioCarico
                'MovimentoCarico.Data = Agenda.Data
                'MovimentoCarico.Ora = MovimentoCampagna.Ora
            End If

            Agenda.Movimenti.Add(MovimentoCarico)
        End If

        Return Agenda

    End Function

    Public Function GestisciSplitAttivita(ByRef attivitaList As List(Of Attivita), codiciAttivita_x_CentriAziendali_List As List(Of CodiciAttivita_x_CentriAziendali), objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri) As List(Of Integer)

        Dim infoOperazione = GetInfoOperazione(attivitaList(0).job.primaryKey.codice, Tipo_Attivita.QuadernoDiCampagna)

        If infoOperazione.IsRilievo Then
            SplitRilievo(attivitaList, objParametri_Server, objParametri_Utenti)
        ElseIf infoOperazione.IsRaccolta Then
            SplitRaccolta(attivitaList, objParametri_Server, objParametri_Utenti)
        Else
            SplitAttivita(attivitaList, infoOperazione, objParametri_Server)
        End If

        Dim listAgendeToDelete = RiassociaCodiciAttivita(attivitaList, codiciAttivita_x_CentriAziendali_List)

        Return listAgendeToDelete

    End Function

    Public Function SplitByImpianto(ByRef attivitaList As List(Of Attivita), objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri) As Boolean

        Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim creaUnaOperazionePerOgniImpianto As Integer = CInt(objUtentiImpostazioni.LeggiValoreImpostazioneScalare(enum_Impostazioni_Utenti.SUPERUSER_COD_RICETTE_CREA_UNA_OPERAZIONE_PER_OGNI_IMPIANTO, objParametri_Utenti.UtenteUsername, objParametri_Utenti))

        If creaUnaOperazionePerOgniImpianto = 1 Then

            'DT: la creazione di una ricetta per impianto si fa solo se:
            ' - si tratta di ricetta
            ' - si è in fase di creazione ricetta
            ' - non ci sono semine

            Dim impianti As New List(Of centri_di_costo.EsercizioCDC)

            For Each attivitaToSplit In attivitaList
                If attivitaToSplit.tipo.Equals(Tipo_Attivita.QuadernoDiCampagna) Then
                    Return False
                End If
                If attivitaToSplit.codice <> "0" Then
                    Return False
                End If
                Dim InfoOperazione As InfoOperazione = GetInfoOperazione(attivitaToSplit.job.primaryKey.codice, attivitaToSplit.tipo)
                If InfoOperazione.IsSemina Then
                    Return False
                End If
                impianti = attivitaToSplit.centriDiCosto.FindAll(Function(c) (c.classType = ClassType.EsercizioCDC)).ConvertAll(Function(obj1) CType(obj1, centri_di_costo.EsercizioCDC))
                If impianti Is Nothing OrElse impianti.Count < 2 Then
                    Return False
                End If
            Next

            Dim objSequenze As New Agro_Sequenze
            Dim raccoglitorePerImpianto As New Dictionary(Of Integer, Integer)
            For Each impianto In impianti
                If attivitaList.Count > 1 Then

                    'Dim codiceRaccoglitore = objSequenze.Agronica_SequenzaTabelle_NuovoID("raccoglitore", objParametri_Server)
                    'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
                    Dim codiceRaccoglitore = objSequenze.NuovoId_Tabella("raccoglitore", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri_Server)


                    raccoglitorePerImpianto.Add(impianto.esercizio.codice, codiceRaccoglitore)
                Else
                    raccoglitorePerImpianto.Add(impianto.esercizio.codice, 0)
                End If
            Next

            Dim attivitaSplitted As New List(Of Attivita)

            'Gli impianti sono gli stessi per ogni attivita, prendo quelli della prima per calcolare la superficie trattata totale 
            Dim superficieTrattataTotale As Decimal = CalcolaSuperficieTrattataTotale(attivitaList(0), GetInfoOperazione(attivitaList(0).job.primaryKey.codice, attivitaList(0).tipo))

            For Each attivitaToSplit In attivitaList

                Dim acquaTotale As Decimal = CalcolaAcquaTotale(attivitaToSplit, superficieTrattataTotale)

                Dim acquaImpianti As Decimal = 0

                For i = 1 To impianti.Count

                    Dim impianto = impianti(i - 1)

                    Dim attivitaImpianto = attivitaToSplit.Clona
                    attivitaImpianto.centriDiCosto = New List(Of centri_di_costo.CentroDiCosto)
                    attivitaImpianto.centriDiCosto.Add(impianto)
                    attivitaImpianto.centroAziendale = New CentroAziendale(impianto.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK)

                    attivitaImpianto.raccoglitore = raccoglitorePerImpianto(impianto.esercizio.codice)

                    Dim superficieTrattataImpianto As Decimal = impianto.superficieTrattata

                    'DT: riproporziono i valori totali (non i dosaggi) rispetto alla proporzione fra la superficie trattata dell'impianto e quella totale dell'attivita
                    Dim ratio As Decimal = superficieTrattataImpianto / superficieTrattataTotale

                    'gestione sfridi decimali (tutti per differenza sull'ultimo impianto)
                    Dim risorsaAcquaImpianto As risorse.RisorsaAcqua = (From a In attivitaImpianto.risorse Where a.classType = costanti.ClassType.RisorsaAcqua).FirstOrDefault
                    If risorsaAcquaImpianto IsNot Nothing Then
                        If risorsaAcquaImpianto.doseAcqua = AgronicaCoreModelsSTD.attivita.risorse.RisorsaAcqua.TipoDoseAcqua.TOTALE Then
                            If i = impianti.Count Then
                                risorsaAcquaImpianto.acqua = acquaTotale - acquaImpianti
                            Else
                                risorsaAcquaImpianto.acqua = acquaTotale * ratio
                                acquaImpianti += risorsaAcquaImpianto.acqua
                            End If
                        End If
                    End If

                    Dim risorseProdotto As List(Of risorse.RisorsaProdotto) = attivitaImpianto.risorse.FindAll(Function(c) (c.classType = ClassType.DettaglioTrattamento OrElse c.classType = ClassType.DettaglioFertilizzazione OrElse c.classType = ClassType.DettaglioSemina)).ConvertAll(Function(obj1) CType(obj1, risorse.RisorsaProdotto))
                    If risorseProdotto IsNot Nothing AndAlso risorseProdotto.Count > 0 Then
                        For Each risorsaProdotto In risorseProdotto
                            If risorsaProdotto.prodotto IsNot Nothing AndAlso risorsaProdotto.prodotto.codice <> 0 Then
                                If risorsaProdotto.MagazziniMovimentazioni IsNot Nothing AndAlso risorsaProdotto.MagazziniMovimentazioni.Count > 0 Then
                                    For Each movimentoMagazzino In risorsaProdotto.MagazziniMovimentazioni
                                        movimentoMagazzino.Qta *= ratio
                                    Next
                                Else
                                    risorsaProdotto.quantitaTotaleReale *= ratio
                                End If
                            End If
                        Next
                    End If

                    Dim dettagliRaccolta As List(Of dettagli.DettaglioRaccolta) = attivitaImpianto.risorse.FindAll(Function(c) (c.classType = ClassType.DettaglioRaccolta)).ConvertAll(Function(obj1) CType(obj1, dettagli.DettaglioRaccolta))
                    If dettagliRaccolta IsNot Nothing AndAlso dettagliRaccolta.Count > 0 Then
                        For Each dettaglioRaccolta In dettagliRaccolta
                            Dim qtaSuImpiantiCentro = New List(Of QuantitaSuImpianto)
                            If dettaglioRaccolta.QuantitaSuImpianti IsNot Nothing AndAlso dettaglioRaccolta.QuantitaSuImpianti.Count > 0 Then
                                For Each qtaSuImpianti In dettaglioRaccolta.QuantitaSuImpianti
                                    If qtaSuImpianti.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice = attivitaImpianto.centroAziendale.primaryKey.codice AndAlso
                                        qtaSuImpianti.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva = attivitaImpianto.centroAziendale.primaryKey.partitaIva Then

                                        qtaSuImpiantiCentro.Add(qtaSuImpianti)
                                    End If
                                Next
                            End If
                            dettaglioRaccolta.QuantitaSuImpianti = qtaSuImpiantiCentro
                            dettaglioRaccolta.quantitaTotaleReale = dettaglioRaccolta.quantitaTotaleReale * ratio
                            dettaglioRaccolta.MagazziniMovimentazioni.ForEach(Sub(m) m.Qta = dettaglioRaccolta.quantitaTotaleReale)
                        Next
                    End If

                    attivitaSplitted.Add(attivitaImpianto)

                Next
            Next

            attivitaList = attivitaSplitted

            Return True

        Else

            Return False

        End If

    End Function


#Region "Funzioni private di validazione"
    Private Shared Function CheckProdottiMagazzini(attivita As Attivita, ByRef erroreGias As ErroreGias) As Boolean

        erroreGias = Nothing

        If attivita.risorse Is Nothing Then
            Return True
        End If

        Dim risorseProdotto As List(Of risorse.RisorsaProdotto) = attivita.risorse.FindAll(Function(c) (c.classType = costanti.ClassType.DettaglioTrattamento OrElse c.classType = ClassType.DettaglioFertilizzazione OrElse c.classType = ClassType.DettaglioSemina)).ConvertAll(Function(obj1) CType(obj1, risorse.RisorsaProdotto))

        'DT: se lo stesso prodotto appare in risorse distinte, alcune con magazzino e altre no, errore
        Dim codiciProdottoDistinti As List(Of Integer) = (From r In risorseProdotto Select r.prodotto.codice).Distinct.ToList
        For Each codiceProdotto In codiciProdottoDistinti
            Dim conMagazzino As Boolean = False
            Dim senzaMagazzino As Boolean = False
            For Each risorsaProdotto In risorseProdotto.FindAll(Function(r) (r.prodotto.codice = codiceProdotto))
                If risorsaProdotto.MagazziniMovimentazioni IsNot Nothing AndAlso risorsaProdotto.MagazziniMovimentazioni.Count > 0 AndAlso risorsaProdotto.MagazziniMovimentazioni(0).Magazzino.primaryKey IsNot Nothing Then
                    conMagazzino = True
                End If
                If risorsaProdotto.MagazziniMovimentazioni Is Nothing OrElse risorsaProdotto.MagazziniMovimentazioni.Count = 0 Then
                    senzaMagazzino = True
                End If
                If conMagazzino AndAlso senzaMagazzino Then
                    Exit For
                End If
            Next
            If conMagazzino AndAlso senzaMagazzino Then
                erroreGias = New ErroreGias With {
                .severity = ErroreGias_Severity.Bloccante,
                    .tipo = ErroreGias_Tipo.Generico,
                    .messaggio = attivita.job.descrizione & ": " & String.Format(My.Resources.AgronicaCoreMapper.ProdottoXConSenzaMagazzino, codiceProdotto.ToString)
                }
                Return False
            End If
        Next

        Return True

    End Function
#End Region


#Region "Funzioni private"
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="attivita"></param>
    ''' <param name="eseguiSoloVerificheConformita"></param>
    ''' <param name="eseguiValiditaFormale"></param> MONDO CONTROLLI ValiditaFormaleOperazione.ValiditaFormaleOperazione() 
    ''' <param name="eseguiControlliAnagrafica"></param> MONDO CONTROLLI  
    ''' <returns></returns>
    Private Function ScriviAttivitaToAgenda(attivita As Attivita,
                                            agenda As Operazione_Agenda,
                                            parametriAggiuntiviList As List(Of Parametri_Aggiuntivi_Attivita),
                                            impiantiList As List(Of Impianto),
                                            objParametri_Super_Server As AgronicaCoreParametri,
                                            objParametri_Server As AgronicaCoreParametri,
                                            objParametri_Utenti As AgronicaCoreParametri,
                                            eseguiSoloVerificheConformita As Boolean,
                                            currentAttivitaDes As String,
                                            creaCaricoMagazzinoXOriginePUA As Boolean,
                                            ByRef listaXMLRicettaOperazione As List(Of String),
                                            Optional ByRef listaErrori As List(Of ErroreGias) = Nothing,
                                            Optional ByRef GiasContext As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities = Nothing,
                                            Optional ByVal OpenNewTransaction As Boolean = True,
                                            Optional isLast As Boolean = False,
                                            Optional agendeToDelete As List(Of Integer) = Nothing
                                            ) As String

        Dim Id_Agenda As Integer = 0
        Dim messaggio As String = ""

        Dim objAgendaScrivi As New Agenda_Operazione_Helper

        Dim lista_mdRif As New List(Of Movimento_Dettaglio_Riferimento)

        If Not (eseguiSoloVerificheConformita) Then

            If agenda.Id_Agenda <> 0 AndAlso attivita.tipo = Tipo_Attivita.QuadernoDiCampagna Then

                'Dal 06/23 conserviamo la data e l'username di creazione originali per tutti i record Agenda
                allinea_DataUsernameCreazione(agenda, objParametri_Server, Tipo_Attivita.QuadernoDiCampagna, 0)
                'Recupero i vecchi costi CdG prima che vengano cancellati
                Dim mdr_Rif As New Agenda_Movimenti_Dettagli_Riferimenti_Helper()
                lista_mdRif = mdr_Rif.LeggiRiferimentiAgenda(agenda.Piva, 0, agenda.Id_Agenda, 0, "", objParametri_Server)


                Dim CancellataOperazione As Boolean = objAgendaScrivi.Cancella(agenda.Piva, agenda.Sa_Cod, agenda.Id_Agenda,
                                                                               False,
                                                                               objParametri_Server, logCancellazione:=False)

                'Riscrivo i vecchi costi
                Dim mdRif_helper As New Agenda_Movimenti_Dettagli_Riferimenti_Helper
                For Each mdRif As Movimento_Dettaglio_Riferimento In lista_mdRif
                    If mdRif.Lav_Cod_Rif = LAVCOD_COSTI_CDG Then
                        mdRif_helper.Scrivi(mdRif, objParametri_Server)
                    End If
                Next
            End If
        End If


        Dim InfoOperazione As InfoOperazione = GetInfoOperazione(agenda.Lav_Cod, Tipo_Attivita.QuadernoDiCampagna)

        Dim isSeminaConFrazionamento As Boolean = False
        Dim isSeminaConAggiornamentoAnagrafica As Boolean = False
        Dim opzioniOperazione As Integer
        For Each parametroAggiuntivo In parametriAggiuntiviList
            If parametroAggiuntivo.key = Key_Parametri_Aggiuntivi_Attivita.Opzione_Semina Then
                If parametroAggiuntivo.value = enum_SEMINA_TIPO.Frazionamento_Appezzamenti_perLotto_e_Semina_sui_Singoli_Default Then
                    isSeminaConFrazionamento = True
                ElseIf parametroAggiuntivo.value = enum_SEMINA_TIPO.Semina_e_Modifica_Appezzamenti_Default Then
                    isSeminaConAggiornamentoAnagrafica = True
                End If
            End If
            If parametroAggiuntivo.key = Key_Parametri_Aggiuntivi_Attivita.Opzione_Raccolta_Aggiornamento_Anagrafica Then
                opzioniOperazione = parametroAggiuntivo.value
            End If
            If parametroAggiuntivo.key = Key_Parametri_Aggiuntivi_Attivita.Opzione_Abbattimento_Aggiornamento_Anagrafica Then
                opzioniOperazione = parametroAggiuntivo.value
            End If
        Next

        'Controllo se è un'Operazione Zootecnica
        If InfoOperazione.IsZoo Then
            Dim attivitaZoo_W As New AgronicaCoreMapper.AttivitaZootecnicaToAgenda

            If attivita.tipo = Tipo_Attivita.QuadernoDiCampagna Then
                '===================================
                '   SCRITTURA AGENDA
                '-----------------------------------
                attivitaZoo_W.ScriviAttivitaZootecnicaToAgenda(attivita, objParametri_Server)

            ElseIf attivita.tipo = Tipo_Attivita.Ricetta Then
                '===================================
                '   SCRITTURA AGENDA DA RICETTA ZOO
                '-----------------------------------
                attivitaZoo_W.ScriviAgendaFromRicettaZootecnica(attivita, objParametri_Server)

            End If

        Else

            If InfoOperazione.IsSemina AndAlso isSeminaConFrazionamento Then

                ScriviAttivitaToAgenda_SeminaConFrazionamento(attivita, InfoOperazione,
                                                              currentAttivitaDes, parametriAggiuntiviList,
                                                              objParametri_Super_Server, objParametri_Server, objParametri_Utenti,
                                                              eseguiSoloVerificheConformita,
                                                              listaErrori, Id_Agenda,
                                                              GiasContext, OpenNewTransaction)

            ElseIf InfoOperazione.IsSemina AndAlso isSeminaConAggiornamentoAnagrafica Then

                ScriviAttivitaToAgenda_SeminaConAggiornamentoAnagrafica(attivita, InfoOperazione, parametriAggiuntiviList,
                                                                        eseguiSoloVerificheConformita, currentAttivitaDes,
                                                                        listaErrori, Id_Agenda,
                                                                        objParametri_Super_Server,
                                                                        objParametri_Server,
                                                                        objParametri_Utenti,
                                                                        GiasContext, OpenNewTransaction)

            Else

                If InfoOperazione.IsRaccolta OrElse (InfoOperazione.IsAbbattimento AndAlso attivita.tipo = Tipo_Attivita.QuadernoDiCampagna) Then
                    '===================================
                    '   CONTROLLI E MODIFICA ANAGRAFICHE
                    '-----------------------------------
                    listaErrori = ModificaAnagrafichePostOperazione.ModificaAnagrafichePostOperazione(
                        agenda, InfoOperazione,
                        eseguiSoloVerificheConformita,
                        currentAttivitaDes, opzioniOperazione,
                        objParametri_Server, objParametri_Utenti
                    )
                    If listaErrori IsNot Nothing AndAlso listaErrori.Count > 0 Then
                        Exit Function
                    End If
                End If

                If Not (eseguiSoloVerificheConformita) Then

                    If attivita.tipo = Tipo_Attivita.QuadernoDiCampagna Then
                        '===================================
                        '   SCRITTURA AGENDA
                        '-----------------------------------
                        Id_Agenda = scriviAgenda(agenda, attivita, parametriAggiuntiviList, agendeToDelete, lista_mdRif, creaCaricoMagazzinoXOriginePUA, objParametri_Server, objParametri_Utenti)

                    ElseIf attivita.tipo = Tipo_Attivita.Ricetta Then
                        '===================================
                        '   PREPARAZIONE XML RICETTE_OPERAZIONI
                        '-----------------------------------

                        agenda.Id_Agenda = If(attivita.associazionePK IsNot Nothing, attivita.associazionePK.id_agenda, 0)

                        listaXMLRicettaOperazione.Add(Prepara_XMLRicetteOperazioni(agenda, attivita, objParametri_Server, InfoOperazione))

                        If attivita.codice <> "" AndAlso attivita.codice <> "0" Then
                            '===================================
                            '   CANCELLAZIONE RICETTA DA MODIFICA
                            '-----------------------------------

                            Dim objRicettaOperazione_R As New AgronicaCoreContabBIZ.Ricette_Operazioni_R
                            Dim ricettaOperazioneDaCancellare As String = objRicettaOperazione_R.Ricetta_Operazioni_Leggi(attivita.testataRicetta.Ricetta_Cod, attivita.codice, 0, 0, AGRODATAINIZIO, AGRODATAFINE, True, objParametri_Server)

                            Dim ret_Operazione_Ricetta As Integer = 0
                            Dim objRicettaOperazione_W As New AgronicaCoreContabBIZ.Ricette_Operazioni_W
                            objRicettaOperazione_W.Ricetta_Operazione_Scrivi(agenda.Piva, agenda.Sa_Cod, ricettaOperazioneDaCancellare, attivita.testataRicetta.Ricetta_Cod, enum_TipoRicetta.Standard_Destinazioni, ret_Operazione_Ricetta, objParametri_Server, OpenTransaction:=OpenNewTransaction, OpenConnection:=False)

                        End If

                        If isLast Then
                            If listaXMLRicettaOperazione.Count > 0 Then

                                '===================================
                                '   SCRITTURA RICETTA
                                '-----------------------------------
                                Dim Ricetta_Cod = scriviRicetta(agenda, attivita, objParametri_Server, listaXMLRicettaOperazione)
                                Id_Agenda = Ricetta_Cod
                            End If

                        End If
                    End If

                End If
            End If
        End If


        If listaErrori IsNot Nothing AndAlso listaErrori.Count > 0 Then
            Exit Function
        End If

        Return Id_Agenda

    End Function

    Private Sub ScriviAttivitaToAgenda_SeminaConFrazionamento(attivita As AgronicaCoreModelsSTD.attivita.Attivita,
                                                              infoOperazione As InfoOperazione,
                                                              currentAttivitaDes As String,
                                                              parametriAggiuntiviList As List(Of Parametri_Aggiuntivi_Attivita),
                                                              objParametri_Super_Server As AgronicaCoreParametri,
                                                              objParametri_Server As AgronicaCoreParametri,
                                                              objParametri_Utenti As AgronicaCoreParametri,
                                                              eseguiSoloVerificheConformita As Boolean,
                                                              ByRef listaErrori As List(Of ErroreGias),
                                                              ByRef idAgenda_to_return As Integer,
                                                                    Optional ByRef GiasContext As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities = Nothing,
                                                                    Optional OpenNewTransaction As Boolean = True
                                                              )


        Dim objAgendaScrivi As New Agenda_Operazione_Helper

        Dim listAttivitaxDettaglioSemina As New List(Of AgronicaCoreModelsSTD.attivita.Attivita)
        Dim originalEsercizioCDC As New centri_di_costo.EsercizioCDC 'Riempito nella funzione più interna in Verifica_Frazionamento
        Dim DtNuoviImpianti As New DataTable


        '===================================
        '   CONTROLLI VALIDITA FORMALE
        '-----------------------------------
        Dim objValiditaFormaleOperazione As New ValiditaFormaleOperazione
        listaErrori = objValiditaFormaleOperazione.Verifica_SeminaConFrazionamento(attivita, parametriAggiuntiviList,
                                                                                   eseguiSoloVerificheConformita, currentAttivitaDes,
                                                                                   objParametri_Server, objParametri_Utenti,
                                                                                   GiasContext:=GiasContext, OpenNewTransaction:=OpenNewTransaction,
                                                                                   listAttivitaxDettaglioSemina:=listAttivitaxDettaglioSemina,
                                                                                   originalEsercizioCDC:=originalEsercizioCDC,
                                                                                   DtNuoviImpianti:=DtNuoviImpianti)

        If listaErrori.Count > 0 Then
            Exit Sub
        End If


        If Not (eseguiSoloVerificheConformita) Then
            '===================================
            '   SCRITTURA AGENDA
            '-----------------------------------
            For Each AttivitaxDettaglioSemina In listAttivitaxDettaglioSemina
                Dim agendaxDettaglioSemina = MappaAttivitaToAgenda(AttivitaxDettaglioSemina, objParametri_Super_Server, objParametri_Server, objParametri_Utenti, eseguiSoloVerificheConformita)


                Dim Id_Agenda = scriviAgenda(agendaxDettaglioSemina, AttivitaxDettaglioSemina, parametriAggiuntiviList, agendeToDelete:=Nothing, agenda_Rif:=Nothing, creaCaricoMagazzinoXOriginePUA:=False, objParametri_Server, objParametri_Utenti)

                'Setto l'id agenda da ritornare, nel caso della semina con frazionamento riporto solo il primo ID_Agenda generato
                If idAgenda_to_return = 0 Then
                    idAgenda_to_return = Id_Agenda
                End If
            Next


            '===================================
            '   FRAZIONAMENTO AGENDE PRECEDENTI
            '-----------------------------------
            Dim ht_CDG As New Hashtable
            Dim objPostOp As New ModificaOperazioniPostOperazione
            objPostOp.FrazionaAgenda(originalEsercizioCDC,
                                 DtNuoviImpianti, idAgenda_to_return,
                                 ht_CDG, objParametri_Server,
                                 infoOperazione.TopCode, infoOperazione.BaseCode)


            '===================================
            '   CDG
            '-----------------------------------
            If ht_CDG.Count > 0 Then
                objPostOp.CancellamentoVecchiCdG_e_AllineamentoDate(originalEsercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                    ht_CDG, objParametri_Server)


                objPostOp.InserimentoCdG(originalEsercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                         ht_CDG, objParametri_Server, objParametri_Utenti,
                                         GiasContext, OpenNewTransaction, listaErrori)

                If listaErrori IsNot Nothing AndAlso listaErrori.Count > 0 Then
                    'In questa lista errori vengono inseriti errori del bombardino che richiedono l'assistenza dell'Amministratore, il messaggio deve essere visibile e gestito
                    Exit Sub
                End If

            End If
        End If

    End Sub

    Private Sub ScriviAttivitaToAgenda_SeminaConAggiornamentoAnagrafica(attivita As AgronicaCoreModelsSTD.attivita.Attivita,
                                                                        infoOperazione As InfoOperazione,
                                                                        parametriAggiuntiviList As List(Of Parametri_Aggiuntivi_Attivita),
                                                                        eseguiSoloVerificheConformita As Boolean,
                                                                        currentAttivitaDes As String,
                                                                        ByRef listaErrori As List(Of ErroreGias),
                                                                        ByRef idAgenda_to_return As Integer,
                                                                        objParametri_Super_Server As AgronicaCoreParametri,
                                                                        objParametri_Server As AgronicaCoreParametri,
                                                                        objParametri_Utenti As AgronicaCoreParametri,
                                                                            Optional ByRef GiasContext As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities = Nothing,
                                                                            Optional OpenNewTransaction As Boolean = True
                                                                        )


        Dim objAgendaScrivi As New Agenda_Operazione_Helper

        Dim DT_AgendeDaSistemare As New DataTable
        Dim DT_AgendeTot As New DataTable
        generaDTAgendeDaSistemare(DT_AgendeDaSistemare, DT_AgendeTot)

        Dim HashIdAgendaDaSistemareTmp As New Hashtable
        Dim HashIdAgendaDaSistemare As New Hashtable
        Dim ht_CDG As New Hashtable

        '===================================
        '   CONTROLLI VALIDITA FORMALE
        '-----------------------------------
        Dim objValiditaFormaleOperazione As New ValiditaFormaleOperazione
        listaErrori = objValiditaFormaleOperazione.Verifica_SeminaConAggiornamentoAnagrafica(attivita, eseguiSoloVerificheConformita, currentAttivitaDes,
                                                                                             DT_AgendeDaSistemare, DT_AgendeTot, HashIdAgendaDaSistemareTmp,
                                                                                             objParametri_Server, objParametri_Utenti,
                                                                                             GiasContext:=GiasContext, OpenNewTransaction:=OpenNewTransaction)

        If listaErrori.Count > 0 Then
            Exit Sub
        End If


        If Not (eseguiSoloVerificheConformita) Then
            Dim objPostOp As New ModificaOperazioniPostOperazione
            objPostOp.SistemaAgenda(attivita.centroAziendale.primaryKey.partitaIva,
                                    HashIdAgendaDaSistemareTmp, HashIdAgendaDaSistemare,
                                    DT_AgendeDaSistemare, DT_AgendeTot,
                                    ht_CDG,
                                    objParametri_Server)

            '===================================
            '   SCRITTURA AGENDA
            '-----------------------------------
            Dim agenda = MappaAttivitaToAgenda(attivita, objParametri_Super_Server, objParametri_Server, objParametri_Utenti, eseguiSoloVerificheConformita)

            idAgenda_to_return = scriviAgenda(agenda, attivita, parametriAggiuntiviList, agendeToDelete:=Nothing, agenda_Rif:=Nothing, creaCaricoMagazzinoXOriginePUA:=False, objParametri_Server, objParametri_Utenti)

            '===================================
            '   CDG
            '-----------------------------------
            If ht_CDG.Count > 0 Then
                objPostOp.CancellamentoVecchiCdG_e_AllineamentoDate(attivita.centroAziendale.primaryKey.partitaIva, ht_CDG, objParametri_Server)


                objPostOp.InserimentoCdG(attivita.centroAziendale.primaryKey.partitaIva,
                                         ht_CDG, objParametri_Server, objParametri_Utenti,
                                         GiasContext, OpenNewTransaction,
                                         listaErrori)

                If listaErrori IsNot Nothing AndAlso listaErrori.Count > 0 Then
                    'In questa lista errori vengono inseriti errori del bombardino che richiedono l'assistenza dell'Amministratore, il messaggio deve essere visibile e gestito
                    Exit Sub
                End If

            End If

        End If

    End Sub

    Private Sub generaDTAgendeDaSistemare(ByRef DtAgendeDaSistemare As DataTable,
                                          ByRef DTAgendeTot As DataTable)

        DtAgendeDaSistemare.Columns.Add(New DataColumn("Id_Agenda", GetType(Integer)))
        DtAgendeDaSistemare.Columns.Add(New DataColumn("Piva", GetType(String)))
        DtAgendeDaSistemare.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
        DtAgendeDaSistemare.Columns.Add(New DataColumn("Appezza", GetType(Integer)))
        DtAgendeDaSistemare.Columns.Add(New DataColumn("Id_Destinazione", GetType(Integer)))
        DtAgendeDaSistemare.Columns.Add(New DataColumn("Qta2", GetType(Decimal))) 'sup_trattata

        DTAgendeTot = DtAgendeDaSistemare.Clone

    End Sub

    Private Shared Function scriviAgenda(agenda As Operazione_Agenda,
                                         attivita As Attivita,
                                         parametriAggiuntiviList As List(Of Parametri_Aggiuntivi_Attivita),
                                         agendeToDelete As List(Of Integer),
                                         agenda_Rif As List(Of Movimento_Dettaglio_Riferimento),
                                         creaCaricoMagazzinoXOriginePUA As Boolean,
                                         objParametri_Server As AgronicaCoreParametri,
                                         objParametri_Utenti As AgronicaCoreParametri
                                         ) As Integer

        Dim documentoPrevisionale = False
        Dim InfoOperazione As InfoOperazione = GetInfoOperazione(agenda.Lav_Cod, Attivita.Tipo_Attivita.QuadernoDiCampagna)
        If InfoOperazione.IsVisita Then
            documentoPrevisionale = True
        End If

        Dim objAgendaScrivi As New Agenda_Operazione_Helper
        Dim Id_Agenda As Integer = objAgendaScrivi.Scrivi(agenda, objParametri_Server, flagUsaOraReale:=True, documentoPrevisionale:=documentoPrevisionale)

        'DT: se sono in fase di ribaltamento
        If attivita.codice = "0" AndAlso attivita.associazionePK IsNot Nothing Then

            Dim objRicettaxAgenda As New RicettexAgenda_W
            If Not objRicettaxAgenda.Scrivi(attivita.associazionePK.Ricetta_Cod, attivita.associazionePK.Ricetta_Operazione_Cod, Id_Agenda, CostantiPersonalizzate.AGRODATAINIZIO, CostantiPersonalizzate.AGRODATAFINE, objParametri_Server) Then
                Throw New Exception(My.Resources.AgronicaCoreMapper.ErroreAggancioRicettaAgenda)
            End If

            Dim objAppHelper As New AppHelper
            If Not objAppHelper.Aggiorna_Agenda_Dati_APP(Id_Agenda, attivita.associazionePK.Ricetta_Cod, attivita.associazionePK.Ricetta_Operazione_Cod, objParametri_Server) Then
                Throw New Exception(My.Resources.AgronicaCoreMapper.ErroreAggancioAgendaAppDati)
            End If
        End If

        'Creazione mov_det_riferimenti fra visita e rilievo (previa eliminazione degli eventuali precedenti)
        'Assegnazione stato "eseguita" alla visita
        If InfoOperazione.IsRilievo Then

            Dim idVisitaCollegata As Integer = 0

            For Each parametroAggiuntivo In parametriAggiuntiviList
                If parametroAggiuntivo.key = Key_Parametri_Aggiuntivi_Attivita.Id_Visita_Collegata Then
                    Try
                        idVisitaCollegata = CInt(parametroAggiuntivo.value)
                    Catch ex As Exception
                    End Try
                End If
            Next

            If idVisitaCollegata > 0 Then

                Dim objAgendaHelper As New Agenda_Operazione_Helper
                objAgendaHelper.ModificaPuntuale(agenda.Piva, 0, idVisitaCollegata, objParametri_Server, statoCod:=StatiWorkflowQdC.Eseguito)

                Dim objRiferimentoHelper As New Agenda_Movimenti_Dettagli_Riferimenti_Helper
                Dim objComRif = New AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R

                If agendeToDelete IsNot Nothing AndAlso agendeToDelete.Count > 0 Then
                    For Each agendaToDelete In agendeToDelete

                        Dim dtRifEsistenti = objComRif.LeggixChiave(
                                agenda.Piva,
                                0,
                                idVisitaCollegata,
                                0, 0, agendaToDelete, 0, 0,
                                LAVCOD_VISITA,
                                CAU_VISITE_ISPETTIVE,
                                "",
                                objParametri_Server)

                        If dtRifEsistenti IsNot Nothing AndAlso dtRifEsistenti.Rows.Count > 0 Then
                            For Each drRifEsistenti In dtRifEsistenti.Rows
                                If drRifEsistenti("Lav_Cod_Rif") = agenda.Lav_Cod Then
                                    objRiferimentoHelper.Cancella(agenda.Piva, 0, idVisitaCollegata, drRifEsistenti("Id_Mov"), drRifEsistenti("Id_Mov_Det"), objParametri_Server)
                                End If
                            Next
                        End If

                    Next
                End If

                Dim objMovimentoHelper As New Agenda_Movimenti_Helper
                Dim movimenti = objMovimentoHelper.Leggi(agenda.Piva, 0, idVisitaCollegata, objParametri_Server)
                If movimenti IsNot Nothing AndAlso movimenti.Count > 0 Then
                    For Each movimento In movimenti
                        If movimento.Cau_Mov = CAU_VISITE_ISPETTIVE Then

                            If movimento.Movimenti_Dettagli IsNot Nothing AndAlso movimento.Movimenti_Dettagli.Count > 0 Then

                                Dim objRiferimento = New Movimento_Dettaglio_Riferimento With {
                                    .Piva = movimento.Movimenti_Dettagli(0).Piva,
                                    .Sa_Cod = movimento.Movimenti_Dettagli(0).Sa_Cod,
                                    .Id_Agenda = idVisitaCollegata,
                                    .Id_Mov = movimento.Movimenti_Dettagli(0).Id_Mov,
                                    .Id_Mov_Det = movimento.Movimenti_Dettagli(0).Id_Mov_Det,
                                    .Lav_Cod = LAVCOD_VISITA,
                                    .Cau_Mov = CAU_VISITE_ISPETTIVE,
                                    .Piva_Rif = agenda.Piva,
                                    .Sa_Cod_Rif = agenda.Sa_Cod,
                                    .Id_Agenda_Rif = Id_Agenda,
                                    .Id_Mov_Rif = -1,
                                    .Id_Mov_Det_Rif = -1,
                                    .Lav_Cod_Rif = agenda.Lav_Cod,
                                    .Cau_Mov_Rif = CAU_ASSEGNATARIO_VISITA,
                                    .Tipo_Associazione = 0
                                }

                                Dim dtRifEsistenti = objComRif.LeggixChiave(
                                                                            movimento.Movimenti_Dettagli(0).Piva,
                                                                            movimento.Movimenti_Dettagli(0).Sa_Cod,
                                                                            idVisitaCollegata,
                                                                            0,
                                                                            0, Id_Agenda, 0, 0,
                                                                            LAVCOD_VISITA,
                                                                            CAU_VISITE_ISPETTIVE,
                                                                            "",
                                                                            objParametri_Server)

                                If dtRifEsistenti Is Nothing OrElse dtRifEsistenti.Rows.Count = 0 Then
                                    objRiferimentoHelper.Scrivi(objRiferimento, objParametri_Server)
                                End If

                            End If

                            Exit For

                        End If
                    Next
                End If

            End If

        End If

        If creaCaricoMagazzinoXOriginePUA Then

            CreaCaricoMagazzinoXPUA(agenda, attivita, objParametri_Server, objParametri_Utenti)

        Else

            If attivita.tipo = Tipo_Attivita.QuadernoDiCampagna AndAlso Not IsNothing(attivita.risorse) AndAlso attivita.risorse.Count > 0 Then

                Dim RisorseConRegistrazioniContabili = (From r In attivita.risorse
                                                        Where (r.classType = ClassType.DettaglioTrattamento AndAlso
                                                                       CType(r, DettaglioTrattamento).MagazziniMovimentazioni.FindIndex(Function(m) Not IsNothing(m.registrazioniCollegate) AndAlso m.registrazioniCollegate.Count > 0) > -1) OrElse
                                                                      (r.classType = ClassType.DettaglioFertilizzazione AndAlso
                                                                       CType(r, DettaglioFertilizzazione).MagazziniMovimentazioni.FindIndex(Function(m) Not IsNothing(m.registrazioniCollegate) AndAlso m.registrazioniCollegate.Count > 0) > -1) OrElse
                                                                       (r.classType = ClassType.DettaglioSemina AndAlso
                                                                       CType(r, DettaglioSemina).MagazziniMovimentazioni.FindIndex(Function(m) Not IsNothing(m.registrazioniCollegate) AndAlso m.registrazioniCollegate.Count > 0) > -1)
                                                        Select r)

                If Not IsNothing(RisorseConRegistrazioniContabili) AndAlso RisorseConRegistrazioniContabili.Count > 0 Then
                    CreaOperazioniContabili(agenda, attivita.job, attivita.disciplinare, RisorseConRegistrazioniContabili.ToList(), agenda_Rif, objParametri_Server, objParametri_Utenti)
                End If

            End If

        End If


        If Not IsNothing(attivita.job) AndAlso attivita.job.getCodice() = LAVCOD_REINNESCO_TRAPPOLE AndAlso
            Not IsNothing(attivita.attivitaCollegate) AndAlso attivita.attivitaCollegate.Count > 0 Then

            CollegaInstallazioneTrappole(agenda, attivita, objParametri_Server, objParametri_Utenti)

        End If


        Return Id_Agenda

    End Function

    Private Shared Function Prepara_XMLRicetteOperazioni(agenda As Operazione_Agenda,
                                                         attivita As Attivita,
                                                         objParametri_Server As AgronicaCoreParametri,
                                                         InfoOperazione As InfoOperazione
                                                         ) As String


        'TODO_AF: sti tre parametri nella Trattamenti_2 non vengono usati assolutamente mai, nè vengono riempiti prima di essere passati byref
        Dim rif_Lav_Cod As Integer
        Dim rif_Des_Lib As String = ""
        Dim rif_Data As Date
        Dim rif_Note As String = ""

        Dim Dpi_Cod, Dpi_PubblicoPrivato As Integer
        Dim progressivo_gias As Integer = 0 'progressivo_gias 'TODO_AF: verificare cos'è, a cosa serve


        '===================================
        '   XML OPERAZIONE
        '-----------------------------------
        'Invoco il metodo che mi genera l'XML, in base al tipo operazione
        Dim StringaXmlOperazione As String = ""
        Dim Helper As New Agenda_Operazione_Helper
        If InfoOperazione.IsTrattamento Then
            StringaXmlOperazione = Helper.GeneraXML_CAU_TRATTAMENTO(agenda, Dpi_Cod, Dpi_PubblicoPrivato)
        End If

        If InfoOperazione.IsLavorazione OrElse InfoOperazione.IsSemina OrElse InfoOperazione.IsRaccolta OrElse
            InfoOperazione.isNonUtilizzo OrElse InfoOperazione.IsAbbattimento OrElse InfoOperazione.IsIrrigazione Then
            StringaXmlOperazione = Helper.GeneraXML_CAU_LAVORAZIONI(agenda)
        End If

        If InfoOperazione.IsFertilizzazione Then
            StringaXmlOperazione = Helper.GeneraXML_CAU_LAVORAZIONI(agenda, Dpi_Cod, Dpi_PubblicoPrivato)
        End If


        '===================================
        '   XML RICETTA OPERAZIONE
        '-----------------------------------
        'Creo la Ricetta_Operazione dall'XML dell'agenda
        Dim objRicettaOp As New AgronicaCoreContabBIZ.Ricette_Operazioni_R

        Dim Tipo_Operazione_Agenda As enum_Tipo_Operazione_Agenda = If(attivita.stato = Stati.Eseguita, enum_Tipo_Operazione_Agenda.RicettaBrogliaccio, enum_Tipo_Operazione_Agenda.Ricetta)

        If attivita.codice <> "" AndAlso attivita.codice <> "0" Then
            'Dal 06/23 conserviamo la data e l'username di creazione originali per tutti i record Agenda
            allinea_DataUsernameCreazione(agenda, objParametri_Server, Tipo_Attivita.Ricetta, attivita.testataRicetta.Ricetta_Cod)
        End If

        Dim guidRicetta = ""
        If Not String.IsNullOrEmpty(attivita.guid) Then
            guidRicetta = attivita.guid
        End If

        Dim StringaXmlRicettaOperazione As String = objRicettaOp.XML_GeneraStringa_Ricetta_Operazione(agenda.Tipo_Operazione, attivita.testataRicetta.Ricetta_Cod, attivita.codice,
                                                                                                      Tipo_Operazione_Agenda, progressivo_gias, objParametri_Server,
                                                                                                      StringaXmlOperazione, rif_Lav_Cod, rif_Des_Lib, rif_Data, rif_Note,
                                                                                                      agenda.Data_Creazione, agenda.Username_Creazione, guidRicetta)

        Return StringaXmlRicettaOperazione
    End Function

    Private Function scriviRicetta(agenda As Operazione_Agenda,
                                   attivita As Attivita,
                                   objParametri_Server As AgronicaCoreParametri,
                                   listaXMLRicettaOperazione As List(Of String)
                                   ) As Integer

        Dim Ricetta_Data_Da As Date = AGRODATAINIZIO
        Dim Ricetta_Data_A As Date = AGRODATAFINE

        If attivita.testataRicetta.Data_Da = Nothing OrElse attivita.testataRicetta.Data_Da = AGRODATAINIZIO Then
            Ricetta_Data_Da = attivita.inizio
            Ricetta_Data_A = attivita.inizio
        Else
            Ricetta_Data_Da = attivita.testataRicetta.Data_Da
            Ricetta_Data_A = attivita.testataRicetta.Data_A
        End If

        If attivita.testataRicetta.Ricetta_Cod <> 0 Then
            impostaDateRicetta(Ricetta_Data_Da, Ricetta_Data_A, agenda.Data, attivita.testataRicetta.Ricetta_Cod, attivita.codice, objParametri_Server)
        End If

        If attivita.testataRicetta.Ricetta_Des = "" Then
            attivita.testataRicetta.Ricetta_Des = CreaDescrizioneTestataRicetta(attivita, objParametri_Server)
            attivita.testataRicetta.Ricetta_Des_Long = attivita.testataRicetta.Ricetta_Des
        End If

        Dim pivaR As String = agenda.Piva
        Dim sa_codR As Integer = agenda.Sa_Cod
        Dim Veg_CodR As Integer = GetVegCodFromUtilizzoTerreno(attivita.utilizzoTerreno)
        Dim OrigineR As String = ""

        Dim Programmazione_CodR As Integer = 0

        If attivita.tipo = Tipo_Attivita.Ricetta AndAlso
           attivita.tipoRicetta = Tipo_Ricetta.PianoDistribuzionePua AndAlso
           Not IsNothing(attivita.testataRicetta.pua) Then

            Programmazione_CodR = attivita.testataRicetta.pua.codice

        End If

        'DT: se sono in modifica rileggo tutti i dati della testata
        If attivita.testataRicetta.Ricetta_Cod <> 0 Then
            Dim objRicetteRDAL As New AgronicaCoreContabDAL.Ricette_R
            Dim dtTestata = objRicetteRDAL.Leggi(attivita.testataRicetta.Ricetta_Cod, "", 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
            If dtTestata IsNot Nothing AndAlso dtTestata.Rows.Count > 0 Then
                pivaR = dtTestata.Rows(0)("Piva")
                sa_codR = dtTestata.Rows(0)("sa_cod")
                Veg_CodR = dtTestata.Rows(0)("Veg_Cod")

                If Programmazione_CodR = 0 Then
                    Programmazione_CodR = dtTestata.Rows(0)("Programmazione_Cod")
                End If

                OrigineR = dtTestata.Rows(0)("Origine")
            End If
        End If

        If attivita.testataRicetta.Ricetta_Numero = "" Then
            Dim objR As New AgronicaCoreContabBIZ.Ricette_R
            attivita.testataRicetta.Ricetta_Numero = objR.Genera_Nuovo_Numero_Ricetta(pivaR, attivita.inizio, objParametri_Server)
        End If

        Dim progressivo_gias As Integer = 0 'progressivo_gias 'TODO_AF: verificare cos'è        ', a cosa serve

        Dim objRicetteR As New AgronicaCoreContabBIZ.Ricette_R
        Dim strXMLRicetta As String = objRicetteR.XML_GeneraStringa_Ricetta(pivaR, sa_codR, Veg_CodR,
                                                                            attivita.testataRicetta.Ricetta_Cod, attivita.testataRicetta.Ricetta_Des,
                                                                            Ricetta_Data_Da, Ricetta_Data_A,
                                                                            attivita.testataRicetta.Ricetta_Numero, attivita.testataRicetta.Note,
                                                                            agenda.Tipo_Operazione, attivita.tipoRicetta, Programmazione_CodR,
                                                                            listaXMLRicettaOperazione,
                                                                            progressivo_gias, objParametri_Server, OrigineR)


        Dim objRicetta_Write As New AgronicaCoreContabBIZ.Ricette_W
        Dim Ricetta_Cod = 0
        objRicetta_Write.Ricetta_Scrivi(strXMLRicetta, Ricetta_Cod, objParametri_Server)

        'Se sono in modifica, restituisco  il cod corrente perchè non viene restituito dal metodo Ricetta_Scrivi
        If Ricetta_Cod = 0 AndAlso attivita.testataRicetta.Ricetta_Cod <> 0 Then
            Ricetta_Cod = attivita.testataRicetta.Ricetta_Cod
        End If

        Return Ricetta_Cod

    End Function

    Private Sub SplitAttivita(ByRef attivitaList As List(Of Attivita), infoOperazione As InfoOperazione, objParametri_Server As AgronicaCoreParametri)

        If attivitaList IsNot Nothing AndAlso attivitaList.Count > 0 Then

            Dim raccoglitoreList = attivitaList.GroupBy(Function(x) x.raccoglitore).Select(Function(x) x.First).Where(Function(x) x.raccoglitore <> 0).ToList

            If raccoglitoreList.Count > 1 Then
                Throw New GiasException(My.Resources.AgronicaCoreMapper.RaccoglitoreNonOmogeneo)
            End If

            'Gli impianti sono gli stessi per ogni attivita, prendo quelli della prima per calcolare la superficie trattata totala
            Dim superficieTrattataTotale As Decimal = CalcolaSuperficieTrattataTotale(attivitaList(0), infoOperazione)

            Dim centriDict As New HashSet(Of Integer)
            For Each attivita In attivitaList

                If attivita.centriDiCosto IsNot Nothing AndAlso attivita.centriDiCosto.Count > 0 Then
                    'centri aziendali desunti da impianti
                    For Each centroDiCosto In attivita.centriDiCosto
                        If (centroDiCosto.classType.Equals(ClassType.EsercizioCDC)) Then
                            Dim esercizioCDC = CType(centroDiCosto, centri_di_costo.EsercizioCDC)
                            Dim sa_cod = esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice
                            centriDict.Add(sa_cod)
                        ElseIf (centroDiCosto.classType.Equals(ClassType.ProdottoDaTrattareCDC)) Then
                            Dim prodottoDaTrattareCdC = CType(centroDiCosto, centri_di_costo.ProdottoDaTrattareCDC)

                            Dim sa_cod = prodottoDaTrattareCdC.giacenzaMagazzino.Magazzino.primaryKey.centroAziendalePK.codice
                            centriDict.Add(sa_cod)
                        End If
                    Next
                Else
                    If attivita.centroAziendale IsNot Nothing AndAlso attivita.centroAziendale.primaryKey IsNot Nothing AndAlso attivita.centroAziendale.primaryKey.codice <> 0 Then
                        'centro aziendale da testata (senza impianti)
                        Dim sa_cod = attivita.centroAziendale.primaryKey.codice
                        centriDict.Add(sa_cod)
                    Else

                        If infoOperazione.isNonUtilizzo Then

                            'aggiungo tutti i centri relativi all'azienda

                            Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read

                            Dim dtCentri = objCentri.Leggi_Filtro_Data(attivita.centroAziendale.primaryKey.partitaIva,
                                                        0,
                                                        attivita.inizio,
                                                        enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                        "",
                                                        "",
                                                        objParametri_Server)

                            For Each drCentro In dtCentri.Rows
                                centriDict.Add(drCentro.Item("Sa_Cod"))
                            Next
                        End If
                    End If
                End If
            Next

            If centriDict.Count = 0 Then
                Throw New GiasException(My.Resources.AgronicaCoreMapper.CentroAziendaleNonValorizzato)
            End If

            Dim codiceRaccoglitore = 0

            'DT: multicentro totale
            If attivitaList.Count > 1 OrElse centriDict.Count > 1 Then
                If raccoglitoreList.Count = 0 Then
                    Dim objSequenze As New Agro_Sequenze
                    'codiceRaccoglitore = objSequenze.Agronica_SequenzaTabelle_NuovoID("raccoglitore", objParametri_Server)
                    'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
                    codiceRaccoglitore = objSequenze.NuovoId_Tabella("raccoglitore", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri_Server)
                Else
                    codiceRaccoglitore = raccoglitoreList(0).raccoglitore
                End If
            End If

            For Each attivita In attivitaList
                attivita.codice = 0
                attivita.raccoglitore = codiceRaccoglitore
            Next

            Dim attivitaBySaCod As New List(Of Attivita)

            For Each attivitaToSplit In attivitaList

                Dim acquaTotale As Decimal = 0
                Dim acquaCentri As Decimal = 0

                Dim risorsaAcquaApplicata As risorse.RisorsaAcqua = (From a In attivitaToSplit.risorse Where a.classType = costanti.ClassType.RisorsaAcqua).FirstOrDefault
                If risorsaAcquaApplicata IsNot Nothing Then
                    If risorsaAcquaApplicata.doseAcqua = AgronicaCoreModelsSTD.attivita.risorse.RisorsaAcqua.TipoDoseAcqua.TOTALE Then
                        acquaTotale = risorsaAcquaApplicata.acqua
                    End If
                End If

                Dim i As Integer = 0
                For Each sa_cod In centriDict
                    i += 1

                    'splitto per centro aziendale e assegno il centro aziendale corretto (anche in caso di mono centro ma che arriva con sa_cod=0)
                    Dim superficieTrattataCentro As Decimal = 0
                    Dim attivitaCentro = attivitaToSplit.Clona
                    attivitaCentro.centriDiCosto = New List(Of centri_di_costo.CentroDiCosto)

                    If attivitaToSplit.centriDiCosto IsNot Nothing AndAlso attivitaToSplit.centriDiCosto.Count > 0 Then

                        'ci sono degli impianti
                        For Each centroDiCosto In attivitaToSplit.centriDiCosto
                            If (centroDiCosto.classType.Equals(ClassType.EsercizioCDC)) Then
                                Dim esercizioCDC = CType(centroDiCosto, centri_di_costo.EsercizioCDC)
                                If esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice = sa_cod Then
                                    attivitaCentro.centriDiCosto.Add(centroDiCosto)
                                    attivitaCentro.centroAziendale = New CentroAziendale(esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK)

                                    superficieTrattataCentro += esercizioCDC.superficieTrattata
                                End If
                            ElseIf (centroDiCosto.classType.Equals(ClassType.ProdottoDaTrattareCDC)) Then
                                Dim prodottoDaTrattareCdC = CType(centroDiCosto, centri_di_costo.ProdottoDaTrattareCDC)
                                If prodottoDaTrattareCdC.giacenzaMagazzino.Magazzino.primaryKey.centroAziendalePK.codice = sa_cod Then
                                    attivitaCentro.centriDiCosto.Add(centroDiCosto)
                                    attivitaCentro.centroAziendale = New CentroAziendale(prodottoDaTrattareCdC.giacenzaMagazzino.Magazzino.primaryKey.centroAziendalePK)

                                    superficieTrattataCentro += prodottoDaTrattareCdC.qtaTrattata
                                End If
                            End If
                        Next
                    Else
                        If infoOperazione.isNonUtilizzo Then
                            'aggiungo il centro relativo al sa_cod su cui itero (creato leggendo i centri dell'azienda)
                            Dim pkCentro = New CentroAziendale.PK(sa_cod, attivitaToSplit.centroAziendale.primaryKey.partitaIva)
                            attivitaCentro.centroAziendale = New CentroAziendale(pkCentro)
                        Else
                            'non ci sono impianti, ma c'è il centro aziendale
                            attivitaCentro.centroAziendale = New CentroAziendale(attivitaToSplit.centroAziendale.primaryKey)
                        End If
                    End If

                    'serve solo in caso di più centri
                    If centriDict.Count > 1 Then
                        'riproporziono i valori totali (non i dosaggi) rispetto alla proporzione fra la superficie trattata del centro e quella totale dell'operazione multicentro
                        Dim ratio As Decimal = If(superficieTrattataTotale > 0, superficieTrattataCentro / superficieTrattataTotale, 1)

                        'gestione sfridi decimali (tutti per differenza sull'ultimo centro)
                        Dim risorsaAcquaCentro As risorse.RisorsaAcqua = (From a In attivitaCentro.risorse Where a.classType = costanti.ClassType.RisorsaAcqua).FirstOrDefault
                        If risorsaAcquaCentro IsNot Nothing Then
                            If risorsaAcquaCentro.doseAcqua = AgronicaCoreModelsSTD.attivita.risorse.RisorsaAcqua.TipoDoseAcqua.TOTALE Then
                                If i = centriDict.Count Then
                                    risorsaAcquaCentro.acqua = acquaTotale - acquaCentri
                                Else
                                    risorsaAcquaCentro.acqua = acquaTotale * ratio
                                    acquaCentri += risorsaAcquaCentro.acqua
                                End If
                            End If
                        End If
                        Dim risorseProdotto As List(Of risorse.RisorsaProdotto) = attivitaCentro.risorse.FindAll(Function(c) (c.classType = ClassType.DettaglioTrattamento OrElse c.classType = ClassType.DettaglioFertilizzazione OrElse c.classType = ClassType.DettaglioSemina)).ConvertAll(Function(obj1) CType(obj1, risorse.RisorsaProdotto))
                        If risorseProdotto IsNot Nothing AndAlso risorseProdotto.Count > 0 Then
                            For Each risorsaProdotto In risorseProdotto
                                If risorsaProdotto.prodotto IsNot Nothing AndAlso risorsaProdotto.prodotto.codice <> 0 Then
                                    If risorsaProdotto.MagazziniMovimentazioni IsNot Nothing AndAlso risorsaProdotto.MagazziniMovimentazioni.Count > 0 Then
                                        UpdateQtaMagazzino(risorsaProdotto.MagazziniMovimentazioni, ratio)
                                    Else
                                        risorsaProdotto.quantitaTotaleReale *= ratio
                                    End If
                                End If
                            Next
                        End If

                        'Splitto le quantitaSuImpianti e le Qta degli Inneschi scaricati da Magazzino in base al sa_cod
                        Dim dettagliTrattamento As List(Of dettagli.DettaglioTrattamento) = attivitaCentro.risorse.FindAll(Function(c) (c.classType = ClassType.DettaglioTrattamento)).ConvertAll(Function(obj1) CType(obj1, dettagli.DettaglioTrattamento))

                        If Not IsNothing(dettagliTrattamento) AndAlso dettagliTrattamento.Count > 0 Then
                            For Each dettaglioTrattamento In dettagliTrattamento
                                If Not IsNothing(dettaglioTrattamento.quantitaSuImpianti) AndAlso dettaglioTrattamento.quantitaSuImpianti.Count > 0 Then
                                    dettaglioTrattamento.quantitaSuImpianti = dettaglioTrattamento.quantitaSuImpianti.FindAll(Function(qt) Not IsNothing(qt.esercizioCDC) AndAlso qt.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice = sa_cod)
                                End If

                                If Not IsNothing(dettaglioTrattamento.avversitaGruppo) AndAlso dettaglioTrattamento.avversitaGruppo.codice > 0 AndAlso
                                   Not IsNothing(dettaglioTrattamento.avversitaGruppo.MagazziniMovimentazioni) AndAlso dettaglioTrattamento.avversitaGruppo.MagazziniMovimentazioni.Count > 0 Then

                                    UpdateQtaMagazzino(dettaglioTrattamento.avversitaGruppo.MagazziniMovimentazioni, ratio)

                                End If
                            Next
                        End If
                    End If

                    If infoOperazione.IsIrrigazione OrElse infoOperazione.IsFertirrigazione Then
                        Dim risorseDaRimuovere = attivitaCentro.risorse.Where(Function(r) TypeOf r Is DettaglioIrrigazione).Cast(Of DettaglioIrrigazione)().Where(Function(d) d.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice <> attivitaCentro.centroAziendale.primaryKey.codice).ToList()

                        For Each risorsaDaRimuovere In risorseDaRimuovere
                            attivitaCentro.risorse.Remove(risorsaDaRimuovere)
                        Next
                    End If

                    attivitaBySaCod.Add(attivitaCentro)

                Next
            Next

            attivitaList = attivitaBySaCod

        End If

    End Sub

    Private Sub SplitRaccolta(ByRef attivitaList As List(Of Attivita), objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri)

        If attivitaList IsNot Nothing AndAlso attivitaList.Count > 0 Then

            If attivitaList.Count = 1 Then

                Dim attivitaToSplit = attivitaList(0)
                Dim piva As String = attivitaToSplit.centroAziendale.primaryKey.partitaIva

                Dim attivitaByCentro As New List(Of Attivita)

                Dim dettagliRaccoltaTotali As New List(Of dettagli.DettaglioRaccolta)
                dettagliRaccoltaTotali.AddRange(attivitaToSplit.risorse.FindAll(Function(c) (c.classType = ClassType.DettaglioRaccolta)).ConvertAll(Function(obj1) CType(obj1, dettagli.DettaglioRaccolta)))

                Dim superficieTrattataTotale As Decimal = 0
                Dim hashCentri As New HashSet(Of String)
                Dim hashImpianti As New HashSet(Of String)
                For Each dettaglioRaccolta In dettagliRaccoltaTotali
                    If dettaglioRaccolta.QuantitaSuImpianti IsNot Nothing AndAlso dettaglioRaccolta.QuantitaSuImpianti.Count > 0 Then
                        For Each qtaSuImpianti In dettaglioRaccolta.QuantitaSuImpianti
                            Dim centroAziendaleKey As String = qtaSuImpianti.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice 'La piva è per forza uguale
                            Dim impiantoKey = qtaSuImpianti.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva &
                                             qtaSuImpianti.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice &
                                             qtaSuImpianti.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.codice &
                                             qtaSuImpianti.esercizioCDC.esercizio.impiantoPK.codice &
                                             qtaSuImpianti.esercizioCDC.esercizio.codice
                            If Not hashImpianti.Contains(impiantoKey) Then
                                superficieTrattataTotale += qtaSuImpianti.esercizioCDC.superficieTrattata
                                hashImpianti.Add(impiantoKey)
                            End If
                            hashCentri.Add(centroAziendaleKey)
                        Next
                    End If
                Next

                Dim raccoglitoreList = attivitaList.GroupBy(Function(x) x.raccoglitore).Select(Function(x) x.First).Where(Function(x) x.raccoglitore <> 0).ToList
                Dim codiceRaccoglitore = 0

                If raccoglitoreList.Count > 1 Then
                    Throw New GiasException(My.Resources.AgronicaCoreMapper.RaccoglitoreNonOmogeneo)
                Else
                    If hashCentri.Count > 1 Then
                        If raccoglitoreList.Count = 0 Then
                            Dim objSequenze As New Agro_Sequenze
                            'codiceRaccoglitore = objSequenze.Agronica_SequenzaTabelle_NuovoID("raccoglitore", objParametri_Server)
                            'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
                            codiceRaccoglitore = objSequenze.NuovoId_Tabella("raccoglitore", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri_Server)
                        Else
                            codiceRaccoglitore = raccoglitoreList(0).raccoglitore
                        End If
                    End If
                End If

                For Each sa_cod In hashCentri

                    Dim attivitaNew = attivitaToSplit.Clona
                    attivitaNew.codice = 0
                    attivitaNew.raccoglitore = codiceRaccoglitore
                    attivitaNew.risorse.RemoveAll(Function(c) (c.classType = ClassType.DettaglioRaccolta))
                    attivitaNew.centriDiCosto = New List(Of centri_di_costo.CentroDiCosto)

                    Dim centroAziendalePK As New CentroAziendale.PK(sa_cod, piva)
                    attivitaNew.centroAziendale = New CentroAziendale(centroAziendalePK)

                    Dim impiantiHash As New HashSet(Of String)
                    For Each dettaglioRaccolta In dettagliRaccoltaTotali
                        Dim superficieTrattataCentro As Decimal = 0

                        If dettaglioRaccolta.QuantitaSuImpianti IsNot Nothing AndAlso dettaglioRaccolta.QuantitaSuImpianti.Count > 0 Then

                            Dim qtaSuImpiantiCentro = New List(Of QuantitaSuImpianto)

                            For Each qtaSuImpianti In dettaglioRaccolta.QuantitaSuImpianti
                                If qtaSuImpianti.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice = sa_cod Then
                                    qtaSuImpiantiCentro.Add(qtaSuImpianti)
                                    superficieTrattataCentro += qtaSuImpianti.esercizioCDC.superficieTrattata

                                    Dim impiantoKey = qtaSuImpianti.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva &
                                             qtaSuImpianti.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice &
                                             qtaSuImpianti.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.codice &
                                             qtaSuImpianti.esercizioCDC.esercizio.impiantoPK.codice &
                                             qtaSuImpianti.esercizioCDC.esercizio.codice
                                    If Not impiantiHash.Contains(impiantoKey) Then
                                        attivitaNew.centriDiCosto.Add(qtaSuImpianti.esercizioCDC)
                                        impiantiHash.Add(impiantoKey)
                                    End If
                                End If
                            Next


                            Dim ratio As Decimal = superficieTrattataCentro / superficieTrattataTotale
                            If qtaSuImpiantiCentro.Count > 0 Then
                                Dim dettaglioRaccoltaCentro As DettaglioRaccolta = dettaglioRaccolta.Clona
                                dettaglioRaccoltaCentro.QuantitaSuImpianti = qtaSuImpiantiCentro
                                If dettaglioRaccoltaCentro.Opzioni_Raccolta.Ripartizione = enum_Ripartizione_Raccolta.MANUALE Then
                                    dettaglioRaccoltaCentro.quantitaTotaleReale = qtaSuImpiantiCentro.Select(Function(q) q.Qta).Aggregate(Function(a, b) a + b)
                                Else
                                    dettaglioRaccoltaCentro.quantitaTotaleReale *= ratio
                                End If

                                dettaglioRaccoltaCentro.MagazziniMovimentazioni.ForEach(Sub(m) m.Qta = dettaglioRaccoltaCentro.quantitaTotaleReale)

                                attivitaNew.risorse.Add(dettaglioRaccoltaCentro)
                            End If

                        End If
                    Next

                    attivitaByCentro.Add(attivitaNew)
                Next

                attivitaList = attivitaByCentro

            Else

                'TODO_DT: resource
                Throw New GiasException("Non possono arrivare più attività per la raccolta")

            End If

        End If

    End Sub

    Public Sub SplitRilievo(ByRef attivitaList As List(Of Attivita), objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri)

        'DT: la raccolta non è un'operazione combinabile con altre, quindi ne arriva sempre solo una
        'per i rilievi non fasi fenologiche, la ff è sempre stringa vuota, il procedimento regge (la fase, oltre al centro, è motivo di split)

        If attivitaList IsNot Nothing AndAlso attivitaList.Count > 0 Then

            If attivitaList.Count = 1 Then

                Dim attivitaToSplit = attivitaList(0)

                Dim piva As String = attivitaToSplit.centroAziendale.primaryKey.partitaIva

                Dim rilievoSenzaImpianti As Boolean = Utility.RilievoSenzaImpianti(attivitaToSplit)

                Dim attivitaByFF As New List(Of Attivita)

                Dim dettagliRilievoTotali As New List(Of dettagli.DettaglioRilievo)
                For Each attivitaToSplit In attivitaList
                    dettagliRilievoTotali.AddRange(attivitaToSplit.risorse.FindAll(Function(c) (c.classType = ClassType.DettaglioRilievo)).ConvertAll(Function(obj1) CType(obj1, dettagli.DettaglioRilievo)))
                Next

                Dim hashCentri As New HashSet(Of String)
                For Each dettaglioRilievo In dettagliRilievoTotali
                    Dim ffKey As String = "0"
                    Dim centroAziendaleKey As String = ""

                    If Not IsNothing(dettaglioRilievo.faseFenologica) Then
                        ffKey = dettaglioRilievo.faseFenologica.codice.ToString()
                    End If

                    'Se il rilievo solo con il centro aziendale e senza aver scelto un impianto prendo il centro
                    'da attivita.centroAziendalePK
                    If rilievoSenzaImpianti Then
                        centroAziendaleKey = attivitaToSplit.centroAziendale.primaryKey.codice
                    Else
                        centroAziendaleKey = dettaglioRilievo.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice 'La piva è per forza uguale
                    End If

                    hashCentri.Add(ffKey & "-" & centroAziendaleKey)
                Next

                Dim raccoglitoreList = attivitaList.GroupBy(Function(x) x.raccoglitore).Select(Function(x) x.First).Where(Function(x) x.raccoglitore <> 0).ToList
                Dim codiceRaccoglitore = 0

                If raccoglitoreList.Count > 1 Then
                    Throw New GiasException(My.Resources.AgronicaCoreMapper.RaccoglitoreNonOmogeneo)
                Else
                    If hashCentri.Count > 1 Then
                        If raccoglitoreList.Count = 0 Then
                            Dim objSequenze As New Agro_Sequenze
                            'codiceRaccoglitore = objSequenze.Agronica_SequenzaTabelle_NuovoID("raccoglitore", objParametri_Server)
                            'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
                            codiceRaccoglitore = objSequenze.NuovoId_Tabella("raccoglitore", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri_Server)
                        Else
                            codiceRaccoglitore = raccoglitoreList(0).raccoglitore
                        End If
                    End If
                End If

                For Each hashItem In hashCentri

                    Dim ff = hashItem.Split("-")(0)
                    Dim sa_cod = hashItem.Split("-")(1)

                    Dim attivitaNew = attivitaToSplit.Clona
                    attivitaNew.codice = 0
                    attivitaNew.raccoglitore = codiceRaccoglitore
                    attivitaNew.risorse.RemoveAll(Function(c) (c.classType = ClassType.DettaglioRilievo))
                    attivitaNew.centriDiCosto = New List(Of centri_di_costo.CentroDiCosto)

                    Dim centroAziendalePK As New CentroAziendale.PK(sa_cod, piva)
                    attivitaNew.centroAziendale = New CentroAziendale(centroAziendalePK)

                    Dim impiantiHash As New HashSet(Of String)

                    For Each dettaglioRilievo In dettagliRilievoTotali

                        'TODO Capire come fare la condizione
                        If Not rilievoSenzaImpianti Then
                            If (ff = "0" OrElse (Not IsNothing(dettaglioRilievo.faseFenologica) AndAlso dettaglioRilievo.faseFenologica.codice = ff)) AndAlso
                                dettaglioRilievo.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice = sa_cod Then

                                attivitaNew.risorse.Add(dettaglioRilievo)

                                Dim Appezza = dettaglioRilievo.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.codice
                                Dim Id_Reg = dettaglioRilievo.esercizioCDC.esercizio.impiantoPK.codice
                                Dim impiantoPk = Appezza & "-" & Id_Reg
                                If Not impiantiHash.Contains(impiantoPk) Then
                                    attivitaNew.centriDiCosto.Add(dettaglioRilievo.esercizioCDC)
                                    impiantiHash.Add(impiantoPk)
                                End If
                            End If
                        Else
                            attivitaNew.risorse.Add(dettaglioRilievo)
                        End If

                    Next

                    attivitaByFF.Add(attivitaNew)
                Next


                attivitaList = attivitaByFF
            Else
                'TODO_DT: resource
                Throw New GiasException("Non possono arrivare più attività per il rilievo")

            End If

        End If

    End Sub



    Private Function RiassociaCodiciAttivita(attivitaList As List(Of Attivita), codiciAttivita_x_CentriAziendali_List As List(Of CodiciAttivita_x_CentriAziendali)) As List(Of Integer)

        Dim listAgende As New List(Of Integer) 'elenco id_agenda riagganciati in salvataggio
        Dim listAgendeToDelete As New List(Of Integer) 'elenco id_agenda non riagganciati in salvataggio, quindi da eliminare

        If codiciAttivita_x_CentriAziendali_List IsNot Nothing AndAlso codiciAttivita_x_CentriAziendali_List.Count > 0 Then
            If attivitaList IsNot Nothing AndAlso attivitaList.Count > 0 Then
                For Each codiceAttivita_x_CentriAziendali In codiciAttivita_x_CentriAziendali_List
                    For Each attivita In attivitaList
                        If attivita.codice = 0 Then 'se non ho già assegnato l'attivita (serve per le fasi fenologiche perchè ci possono essere più attivita sullo stesso centro/lav_cod
                            If attivita.centroAziendale.primaryKey.codice = codiceAttivita_x_CentriAziendali.Sa_Cod AndAlso attivita.job.getCodice = codiceAttivita_x_CentriAziendali.Lav_Cod Then
                                If attivita.tipo = Tipo_Attivita.QuadernoDiCampagna Then
                                    attivita.codice = codiceAttivita_x_CentriAziendali.ID_Agenda
                                    If codiceAttivita_x_CentriAziendali.Ricetta_Cod <> 0 Then 'se provengo da un ribaltamento
                                        attivita.associazionePK = New AssociazionePK
                                        attivita.associazionePK.Ricetta_Cod = codiceAttivita_x_CentriAziendali.Ricetta_Cod
                                        attivita.associazionePK.Ricetta_Operazione_Cod = codiceAttivita_x_CentriAziendali.Ricetta_Operazione_Cod
                                    End If
                                Else
                                    attivita.codice = codiceAttivita_x_CentriAziendali.Ricetta_Operazione_Cod
                                    attivita.testataRicetta.Ricetta_Cod = codiceAttivita_x_CentriAziendali.Ricetta_Cod
                                End If

                                listAgende.Add(attivita.codice)
                                Exit For
                            End If
                        End If
                    Next
                Next

                'TODO_DT: attualmente la cancellazione è solo per le agenda, fare anche per le ricette
                'oppure bloccare la deselezione degli impianti per le operazioni multicentro
                For Each codiceAttivita_x_CentriAziendali In codiciAttivita_x_CentriAziendali_List
                    If codiceAttivita_x_CentriAziendali.ID_Agenda <> 0 Then
                        If Not listAgende.Contains(codiceAttivita_x_CentriAziendali.ID_Agenda) Then
                            If Not listAgendeToDelete.Contains(codiceAttivita_x_CentriAziendali.ID_Agenda) Then
                                listAgendeToDelete.Add(codiceAttivita_x_CentriAziendali.ID_Agenda)
                            End If
                        End If
                    End If
                Next
            End If
        End If

        'rimuoviRaccolteExtra(attivitaList, listAgendeToDelete)

        Return listAgendeToDelete
    End Function

    Private Sub rimuoviRaccolteExtra(ByRef attivitaList As List(Of Attivita), ByRef listAgendeToDelete As List(Of Integer))
        If attivitaList(0).job.primaryKey.codice <> LAVCOD_RACCOLTA Then
            Return
        End If
        Dim isFast = True
        Dim isManuale = False
        For Each attivita In attivitaList
            Dim dettagli = attivita.risorse.Where(Function(r) r.classType = "DettaglioRaccolta").Cast(Of DettaglioRaccolta)
            Dim QtaTot = dettagli.Select(Of Decimal)(Function(d) d.quantitaTotaleReale).Aggregate(Function(q1, q2) q1 + q2)
            Dim QtaSum = dettagli.Select(Of Decimal)(Function(d) d.QuantitaSuImpianti.Select(Of Decimal)(Function(suImpianto) suImpianto.Qta).
                                                                                                  Aggregate(Function(q1, q2) q1 + q2)).
                                      Aggregate(Function(q1, q2) q1 + q2)
            isManuale = isManuale OrElse dettagli(0).Opzioni_Raccolta.Ripartizione = enum_Ripartizione_Raccolta.MANUALE
            isFast = isFast AndAlso QtaTot = 0 AndAlso QtaSum = 0
        Next
        If isManuale AndAlso Not isFast Then
            For Each attivita In attivitaList
                Dim dettagli = attivita.risorse.Where(Function(r) r.classType = "DettaglioRaccolta").Cast(Of DettaglioRaccolta)
                Dim QtaSum = dettagli.Select(Of Decimal)(Function(d) d.QuantitaSuImpianti.Select(Of Decimal)(Function(suImpianto) suImpianto.Qta).
                                                                                                      Aggregate(Function(q1, q2) q1 + q2)).
                                          Aggregate(Function(q1, q2) q1 + q2)
                If QtaSum = 0 Then
                    ' In una raccolta manuale le quantità sono calcolate in base a quelle indicate nei singoli impianti.
                    ' In questo caso le quantità indicate risultano nulle ma non siamo in un caso di raccolta fast!
                    listAgendeToDelete.Add(attivita.codice)
                    attivitaList.Remove(attivita)
                End If
            Next
        End If
    End Sub

    Private Function CalcolaAcquaTotale(attivita As Attivita, superficieTrattataTotale As Decimal) As Decimal

        Dim acquaTotale As Decimal = 0

        'DT: si assume che ci sia al massimo una risorsa acqua
        Dim risorsaAcquaApplicata As risorse.RisorsaAcqua = (From a In attivita.risorse Where a.classType = costanti.ClassType.RisorsaAcqua).FirstOrDefault

        If risorsaAcquaApplicata IsNot Nothing Then

            Select Case risorsaAcquaApplicata.doseAcqua
                Case AgronicaCoreModelsSTD.attivita.risorse.RisorsaAcqua.TipoDoseAcqua.TOTALE
                    acquaTotale = risorsaAcquaApplicata.acqua

                Case AgronicaCoreModelsSTD.attivita.risorse.RisorsaAcqua.TipoDoseAcqua.HA
                    acquaTotale = risorsaAcquaApplicata.acqua * superficieTrattataTotale

            End Select

        End If

        Return acquaTotale

    End Function

    Private Function CalcolaSuperficieTrattataTotale(attivita As Attivita, InfoOperazione As InfoOperazione) As Decimal


        Dim superficieTrattataTotale As Decimal = 0

        If Not IsNothing(attivita.centriDiCosto) Then
            Dim prodottoDaTrattareCdC As centri_di_costo.ProdottoDaTrattareCDC
            If InfoOperazione.TipoCentroDiCosto = centri_di_costo.Tipo.ProdottoDaTrattare Then
                For Each centroDiCosto In attivita.centriDiCosto
                    If TypeOf centroDiCosto Is ProdottoDaTrattareCDC Then
                        prodottoDaTrattareCdC = CType(centroDiCosto, ProdottoDaTrattareCDC)
                        superficieTrattataTotale += prodottoDaTrattareCdC.qtaTrattata
                    End If
                Next
            Else
                Dim esercizioCDC As centri_di_costo.EsercizioCDC
                For Each centroDiCosto In attivita.centriDiCosto
                    If TypeOf centroDiCosto Is centri_di_costo.EsercizioCDC Then
                        esercizioCDC = CType(centroDiCosto, centri_di_costo.EsercizioCDC)
                        superficieTrattataTotale += esercizioCDC.superficieTrattata
                    End If
                Next
            End If
        End If

        Return superficieTrattataTotale

    End Function

    Private Function CreaDescrizioneTestataRicetta(attivita As Attivita, objParametri_Server As AgronicaCoreParametri) As String
        Dim descrizione = ""

        If attivita.testataRicetta IsNot Nothing Then

            'DT: se la descrizione arriva già popolata, non viene ricostruita per rilettura da DB
            If Not String.IsNullOrEmpty(attivita.testataRicetta.Ricetta_Des) Then
                Return attivita.testataRicetta.Ricetta_Des
            End If

            If attivita.job IsNot Nothing Then
                descrizione = GetListJobDescription(attivita.job, objParametri_Server)
            End If

            Dim objImpianto = New AgronicaCoreAnagrafeBIZ.Impianto_R

            Dim listaVarieta As New List(Of String)
            Dim VegDes As String = ""

            For Each centroDiCosto In attivita.centriDiCosto
                If (centroDiCosto.classType.Equals(costanti.ClassType.EsercizioCDC)) Then

                    Dim esercizioCDC As centri_di_costo.EsercizioCDC = CType(centroDiCosto, centri_di_costo.EsercizioCDC)

                    Dim Piva = esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva
                    Dim Sa_Cod = esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice
                    Dim Appezza = esercizioCDC.esercizio.impiantoPK.appezzamentoPK.codice
                    Dim Id_Reg = esercizioCDC.esercizio.impiantoPK.codice

                    Dim CulDes As String = ""
                    objImpianto.Descrizioni_From_Piva_Sa_Cod_Appezza_Id_Reg(Piva, Sa_Cod, Appezza, Id_Reg, CulDes, VegDes, objParametri_Server)

                End If
            Next

            descrizione &= " " & VegDes & " " & attivita.inizio.ToString("dd/MM/yyyy")

        End If

        Return descrizione

    End Function

    Private Function CreaDescrizione(attivita As Attivita, objParametri_Server As AgronicaCoreParametri) As String

        'DT: se la descrizione arriva già popolata, non viene ricostruita per rilettura da DB
        If Not String.IsNullOrEmpty(attivita.descrizione) Then
            Return attivita.descrizione
        End If

        Dim descrizione = ""

        If attivita.job IsNot Nothing Then
            descrizione = GetListJobDescription(attivita.job, objParametri_Server)
        End If

        Dim objImpianto = New AgronicaCoreAnagrafeBIZ.Impianto_R

        Dim listaVarieta As New List(Of String)
        Dim VegDes As String = ""

        For Each centroDiCosto In attivita.centriDiCosto
            If (centroDiCosto.classType.Equals(costanti.ClassType.EsercizioCDC)) Then

                Dim esercizioCDC As centri_di_costo.EsercizioCDC = CType(centroDiCosto, centri_di_costo.EsercizioCDC)

                Dim Piva = esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva
                Dim Sa_Cod = esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice
                Dim Appezza = esercizioCDC.esercizio.impiantoPK.appezzamentoPK.codice
                Dim Id_Reg = esercizioCDC.esercizio.impiantoPK.codice

                Dim CulDes As String = ""
                objImpianto.Descrizioni_From_Piva_Sa_Cod_Appezza_Id_Reg(Piva, Sa_Cod, Appezza, Id_Reg, CulDes, VegDes, objParametri_Server)
                listaVarieta.Add(CulDes)

            End If
        Next

        Dim strVarieta As String = String.Join(", ", listaVarieta.Distinct().ToList())

        If attivita.utilizzoTerreno IsNot Nothing Then
            Select Case attivita.utilizzoTerreno.classType
                Case ClassType.Varieta
                    If VegDes <> "" Then
                        descrizione &= " (" & VegDes
                    End If
                    If strVarieta <> "" Then
                        descrizione &= "  [" & strVarieta & "]"
                    End If
                    If VegDes <> "" Then
                        descrizione &= ")"
                    End If

                Case ClassType.DestinazioneUso
                    If VegDes <> "" Then
                        descrizione &= " (" & VegDes & ")"
                    End If

            End Select
        End If

        Return descrizione
    End Function

    Private Function CreaAttivitaPersonalizzata(attivita As Attivita, agenda As Operazione_Agenda) As Integer

        Dim Id_Attivita As Integer = 0
        If attivita.attivitaPersonalizzata IsNot Nothing AndAlso attivita.attivitaPersonalizzata.codice > 0 AndAlso (agenda.Lav_Cod = LAVCOD_ALTRE_OPERAZIONI OrElse agenda.Lav_Cod = LAVCOD_VISITA) Then
            Id_Attivita = attivita.attivitaPersonalizzata.codice
        End If

        Return Id_Attivita

    End Function

    ''' <summary>
    ''' Crea una lista di note a partire dalle note d'intervento indicate nell'attività passata.
    ''' </summary>
    ''' <remarks>
    ''' In alcuni casi è possibile che le note non siano state inizializzate su angular.
    ''' Riconosco il caso perché nelle note dell'attività è presente un unico elemento con codice 0,
    ''' mentre solitamente le note hanno tutte codice negativo. In questo caso carico le note di
    ''' default per la combinazione di lavorazione/specie indicata.
    ''' </remarks>
    Private Function CreaNote(attivita As Attivita, agenda As Operazione_Agenda, objParametri_Server As AgronicaCoreParametri) As List(Of Nota)
        Dim note As New List(Of Nota)
        'DT: si prendono tutte le note presenti sul modello nuovo, indipendentemente dalla visibilità
        If attivita.noteIntervento IsNot Nothing Then

            If attivita.noteIntervento.Count = 1 AndAlso attivita.noteIntervento.First.codice = 0 Then
                attivita.noteIntervento = New List(Of note_intervento.NoteIntervento)

                If Not IsNothing(attivita.utilizzoTerreno) Then

                    Dim noteReader As New AgronicaControlli_2010.STD_Note

                    Dim specie As Specie = Nothing

                    If attivita.utilizzoTerreno.classType = ClassType.Varieta Then
                        Dim varieta = CType(attivita.utilizzoTerreno, Varieta)

                        If Not IsNothing(varieta) AndAlso Not IsNothing(varieta.specie) Then
                            specie = varieta.specie
                        End If
                    End If

                    Return noteReader.LeggiDaSpecieLavorazione(specie, attivita.job, objParametri_Server, agenda.Piva).
                            Select(Function(n) New Nota With {.Id_Agenda = agenda.Id_Agenda, .Nota_Cod = n.codice}).ToList
                End If


            Else
                For Each notaIntervento In attivita.noteIntervento
                    Dim nota As New Nota With {
                        .Id_Agenda = agenda.Id_Agenda,
                        .Nota_Cod = notaIntervento.codice
                    }
                    note.Add(nota)
                Next
            End If
        End If

        Return note
    End Function

    Private Function CreaMovimentiCostiAccessori(attivita As Attivita, agenda As Operazione_Agenda, objParametri_Server As AgronicaCoreParametri) As List(Of Movimento)
        'DT: udm, qta e prezzo non vengono salvate, si usa il "Salva e vai ai costi" o comunque il CdG 

        Dim movimentiCostiAccessori As New List(Of Movimento)
        Dim Cau_Mov As String
        Dim Elem_Cod As Integer
        Dim Mat_Cod As Integer

        If IsNothing(attivita.risorse) Then
            Return movimentiCostiAccessori
        End If

        'DT: dictionary risorse: chiave = CAU_MOV, valore = lista delle risorse associate al CAU_MOV
        'necessario perchè il CAU_MOV è funzione sia del classtype (Macchina o Persona), sia del rapporto contabile (per le persone)
        Dim risorseDict As New Dictionary(Of String, List(Of risorse.Risorsa))
        Dim risorse_umane_R As New AgronicaCoreAnagrafeBIZ.RisorseUmane_R

        For Each risorsa In attivita.risorse

            Select Case risorsa.classType
                Case ClassType.RisorsaMacchina
                    Cau_Mov = CAU_IMPUTAZIONE_PARCOMACCHINE
                Case ClassType.RisorsaPersona

                    Dim risorsaUmana = CType(risorsa, risorse.RisorsaPersona).risorsaUmana

                    Mat_Cod = risorsaUmana.codice

                    'DT: se il rapporto contabile arriva già popolato, non viene ricostruito per rilettura da DB
                    Dim rapportoContabile As Object
                    If risorsaUmana.rapportoContabile IsNot Nothing Then
                        rapportoContabile = risorsaUmana.rapportoContabile
                    Else
                        Dim risorsaUmanaDecoded = risorse_umane_R.Decodifica(Piva:="", Mat_Cod, objParametri_Server, verbose:=True)
                        rapportoContabile = risorsaUmanaDecoded.rapportoContabile
                    End If

                    Select Case rapportoContabile.codice
                        Case COD_TERZISTA
                            Cau_Mov = CAU_IMPUTAZIONE_TERZISTI
                        Case COD_TECNICORESPONSABILE
                            Cau_Mov = CAU_IMPUTAZIONE_TECNICO_RESPONSABILE
                        Case Else
                            Cau_Mov = CAU_IMPUTAZIONE_MANODOPERA
                    End Select
                Case Else
                    Cau_Mov = ""
            End Select

            If Cau_Mov <> "" Then
                If Not risorseDict.ContainsKey(Cau_Mov) Then
                    risorseDict.Add(Cau_Mov, New List(Of risorse.Risorsa))
                End If
                risorseDict(Cau_Mov).Add(risorsa)
            End If

        Next

        'DT: per ogni cau_mov si crea un movimento, a cui si associano n movimenti di dettaglio (n = numero risorse di quel cau_mov)
        For Each itemDict In risorseDict

            Dim movimento As New Movimento With {
                .Cau_Mov = itemDict.Key,
                .Cod_Risum = 0,
                .Data = agenda.Data,
                .Id_Agenda = agenda.Id_Agenda,
                .Piva = agenda.Piva,
                .Sa_Cod = agenda.Sa_Cod,
                .Lav_Cod = 0,
                .Mezzo = 0
            }

            movimento.Movimenti_Dettagli = New List(Of Movimento_Dettaglio)

            For Each risorsa In itemDict.Value

                Select Case risorsa.classType
                    Case costanti.ClassType.RisorsaMacchina
                        Elem_Cod = CostantiPersonalizzate.MACCHINE
                        Mat_Cod = CType(risorsa, risorse.RisorsaMacchina).macchina.codice

                    Case costanti.ClassType.RisorsaPersona
                        Elem_Cod = CostantiPersonalizzate.ELEMCOD_MANODOPERA
                        Mat_Cod = CType(risorsa, risorse.RisorsaPersona).risorsaUmana.codice
                End Select
                Dim movDet As New Movimento_Dettaglio With {
                    .Data = agenda.Data,
                    .Mat_Cod = Mat_Cod,
                    .Elem_Cod = Elem_Cod,
                    .Pro_Cod = 0,
                    .Udm_Cod = 0,
                    .Piva = agenda.Piva,
                    .Sa_Cod = agenda.Sa_Cod,
                    .Contabilizzato = NONCONTABILE
                }

                movimento.Movimenti_Dettagli.Add(movDet)

            Next

            movimentiCostiAccessori.Add(movimento)
        Next

        Return movimentiCostiAccessori

    End Function

    Private Function CreaMovimentiAssegnatariVisita(attivita As Attivita, agenda As Operazione_Agenda, objParametri_Server As AgronicaCoreParametri) As List(Of Movimento)

        Dim movimentiAssegnatariVisita As New List(Of Movimento)

        Dim risorseAssegnatariVisita = attivita.risorse.FindAll(Function(c) (c.classType = ClassType.RisorsaAssegnatarioVisita))

        If risorseAssegnatariVisita IsNot Nothing AndAlso risorseAssegnatariVisita.Count > 0 Then

            Dim movimento As New Movimento With {
                .Cau_Mov = CAU_ASSEGNATARIO_VISITA,
                .Cod_Risum = 0,
                .Data = agenda.Data,
                .Id_Agenda = agenda.Id_Agenda,
                .Piva = agenda.Piva,
                .Sa_Cod = agenda.Sa_Cod,
                .Lav_Cod = 0,
                .Mezzo = 0
            }

            movimento.Movimenti_Dettagli = New List(Of Movimento_Dettaglio)

            For Each risorsa In risorseAssegnatariVisita

                Dim movDet As New Movimento_Dettaglio With {
                        .Data = agenda.Data,
                        .Mat_Cod = CType(risorsa, risorse.RisorsaAssegnatarioVisita).risorsaUmana.codice,
                        .Elem_Cod = CostantiPersonalizzate.ELEMCOD_MANODOPERA,
                        .Pro_Cod = 0,
                        .Udm_Cod = 0,
                        .Piva = agenda.Piva,
                        .Sa_Cod = agenda.Sa_Cod,
                        .Contabilizzato = NONCONTABILE
                    }

                movimento.Movimenti_Dettagli.Add(movDet)

            Next

            movimentiAssegnatariVisita.Add(movimento)

        End If

        Return movimentiAssegnatariVisita

    End Function

    Private Function CreaMovimentoCampagna(attivita As Attivita, agenda As Operazione_Agenda, infoOperazione As InfoOperazione) As Movimento

        Dim MovimentoCampagna As New Movimento With {
            .Id_Agenda = agenda.Id_Agenda,
            .Piva = agenda.Piva,
            .Sa_Cod = agenda.Sa_Cod,
            .Data = agenda.Data,
            .Ora = New DateTime(agenda.Data.Year, agenda.Data.Month, agenda.Data.Day, attivita.oraInizio.Hour, attivita.oraInizio.Minute, attivita.oraInizio.Second),
            .OraFine = New DateTime(agenda.Data.Year, agenda.Data.Month, agenda.Data.Day, attivita.oraFine.Hour, attivita.oraFine.Minute, attivita.oraFine.Second),
            .Lav_Cod = agenda.Lav_Cod,
            .Cau_Mov = infoOperazione.Cau_Mov,
            .Mov_Desc = attivita.note,
            .Num_Protocollo = 0,
            .Extra_Int = 0,
            .Mezzo = 0,
            .Modalita = attivita.modalita
        }

        If Not IsNothing(attivita.modalitaApplicazione) Then
            MovimentoCampagna.Modalita_Applicazione = attivita.modalitaApplicazione.codice
        End If

        If infoOperazione.IsTrattamento Then
            If attivita.disciplinare IsNot Nothing Then
                Select Case attivita.disciplinare.codice
                    Case -999 'NessunDpiNessunaEtichetta
                        MovimentoCampagna.Num_Protocollo = 0
                    Case 0
                        MovimentoCampagna.Num_Protocollo = -1
                    Case -2   'BIO
                        MovimentoCampagna.Num_Protocollo = -2
                    Case Else   'DPI
                        MovimentoCampagna.Num_Protocollo = attivita.disciplinare.codice
                        If attivita.disciplinare.raggruppamentiColturaliDPI IsNot Nothing Then
                            MovimentoCampagna.Doc_Numero = attivita.disciplinare.raggruppamentiColturaliDPI.codice
                        End If
                        MovimentoCampagna.Disciplinare_PubblicoPrivato = attivita.disciplinare.disciplinarePubblicoPrivato
                        If attivita.epoca IsNot Nothing AndAlso attivita.epoca.codice <> 0 Then
                            MovimentoCampagna.Extra_Int = attivita.epoca.codice
                        End If
                End Select
            End If
        End If

        If infoOperazione.IsFertilizzazione Then
            If attivita.disciplinare IsNot Nothing AndAlso attivita.disciplinare.regolamentoConcimazione IsNot Nothing Then
                'DT il disciplinare - 999 NessunDpiNessunaEtichetta non è applicabile alle Fertilizzazioni, pertanto
                'se dovesse arrivare causa multioperazione, al momento del salvataggio -999 verrà equiparato allo 0
                Select Case attivita.disciplinare.regolamentoConcimazione.codice
                    Case -999, 0 'NessunDpiNessunaEtichetta e NessunDpi
                        MovimentoCampagna.Num_Protocollo = 0
                    Case -2   'BIO
                        MovimentoCampagna.Num_Protocollo = -2
                    Case Else   'DPI
                        MovimentoCampagna.Num_Protocollo = attivita.disciplinare.regolamentoConcimazione.codice
                End Select
            End If
            If attivita.epoca IsNot Nothing AndAlso attivita.epoca.codice <> 0 Then
                MovimentoCampagna.Extra_Int = attivita.epoca.codice
            End If
        End If

        If infoOperazione.IsRilievo Then
            If attivita.disciplinare IsNot Nothing Then
                Select Case attivita.disciplinare.codice
                    Case -999, 0 'NessunDpiNessunaEtichetta e NessunDpi
                        MovimentoCampagna.Num_Protocollo = 0
                    Case -2   'BIO
                        MovimentoCampagna.Num_Protocollo = -2
                    Case Else  'DPI
                        MovimentoCampagna.Num_Protocollo = attivita.disciplinare.codice
                        If attivita.disciplinare.raggruppamentiColturaliDPI IsNot Nothing Then
                            MovimentoCampagna.Doc_Numero = attivita.disciplinare.raggruppamentiColturaliDPI.codice
                        End If
                        MovimentoCampagna.Disciplinare_PubblicoPrivato = attivita.disciplinare.disciplinarePubblicoPrivato
                End Select
            Else
                MovimentoCampagna.Num_Protocollo = -1
                MovimentoCampagna.Doc_Numero = 0
                MovimentoCampagna.Disciplinare_PubblicoPrivato = 0
            End If
        End If

        If infoOperazione.IsRaccolta Then
            'DT: se non è presente il prodotto --> Fast
            '    se c'è il prodotto ma non c'è il magazzino --> Leggera
            '    se c'è il  magazzino --> Leggera_Con_Dettagli_Magazzino

            'Versione 2 creata sulla base dei dati passati da angualr
            '--- GESTIONE TIPO RACCOLTA
            Dim dettagliRaccolta = attivita.risorse.FindAll(Function(r) (r.classType = ClassType.DettaglioRaccolta)).
                                                    ConvertAll(Function(obj1) CType(obj1, risorse.RisorsaProdotto))

            Dim tipo_raccolta As Integer = Attivita.Tipo_Raccolta.Fast
            If dettagliRaccolta.FindAll(Function(r) (r.prodotto IsNot Nothing AndAlso r.prodotto.codice <> 0)).Count() > 0 Then
                tipo_raccolta = Attivita.Tipo_Raccolta.Leggera

                If dettagliRaccolta.FindAll(Function(r) (r.MagazziniMovimentazioni.Count > 0 AndAlso r.MagazziniMovimentazioni.First.Magazzino.primaryKey.codice <> 0)).Count() Then
                    tipo_raccolta = Attivita.Tipo_Raccolta.Leggera_Con_Dettagli_Magazzino
                End If
            End If
            attivita.tipoRaccolta = tipo_raccolta
            MovimentoCampagna.Extra_Int = tipo_raccolta

            If dettagliRaccolta IsNot Nothing AndAlso dettagliRaccolta.Count > 0 Then
                MovimentoCampagna.Mezzo = CType(dettagliRaccolta.First(), DettaglioRaccolta).Opzioni_Raccolta.Ripartizione
            Else
                MovimentoCampagna.Mezzo = Opzioni_Raccolta.enum_Ripartizione_Raccolta.AUTO_SUPERFICIE
            End If

        End If

        If infoOperazione.IsSemina Then
            MovimentoCampagna.Extra_Int = 30 '30=Solo Semina RBL_Tipo_Semina.SelectedValue
        End If

        MovimentoCampagna.BaseCode = infoOperazione.BaseCode
        MovimentoCampagna.TopCode = infoOperazione.TopCode

        Return MovimentoCampagna
    End Function

    Private Function CreaMovimentoScarico(attivita As Attivita, agenda As Operazione_Agenda, infoOperazione As InfoOperazione, eseguiVerificheConformita As Boolean, objParametri_Server As AgronicaCoreParametri) As Movimento
        'TODO_DT: rivedere tutte le parti commentate

        Dim MovimentoScarico As Movimento = Nothing

        'TODO_DT FARE: verificare che esista almeno un movimentoMagazzino in un dettaglio --> scriviMagazzino = True altrimenti esci
        Dim scriviMagazzino As Boolean = False
        'For Each row In Dt_Dosi.Rows
        '    If CStr(row("piva")) <> "" AndAlso CInt(row("Sa_Cod")) <> 0 AndAlso CInt(row("Fabbricato_Cod")) <> 0 Then
        '        scriviMagazzino = True
        '        Exit For
        '    End If
        'Next

        scriviMagazzino = True 'TODO_DT: togliere
        If scriviMagazzino <> "0" AndAlso (infoOperazione.IsTrattamento OrElse infoOperazione.IsFertilizzazione OrElse infoOperazione.IsSemina) Then

            'Dim piva_magazzino As String = ""
            'Dim sa_cod_magazzino As String = ""
            'Dim fabbricatox_Cod As String = ""

            Dim magazzinoEsterno As Boolean = True
            Dim magazzinoGerarchico As Boolean = False

            If (eseguiVerificheConformita) Then
                'verificaConformitaMagazzini().---> TODO_DT: fare se eseguiVerificheConformita = True
            End If

            'If objParametriAgenda.Fabbricato <> "0" AndAlso
            '            Split(objParametriAgenda.Fabbricato, "|").Count = 3 AndAlso Split(objParametriAgenda.Fabbricato, "|")(2) <> objParametriAgenda.Piva Then
            '    magazzinoEsterno = True
            '    magazzinoGerarchico = isMagazzinoGerarchico(Split(objParametriAgenda.Fabbricato, "|")(2))
            'Else
            '    magazzinoEsterno = False
            'End If

            'If magazzinoGerarchico = False Then
            '    piva_magazzino = Split(objParametriAgenda.Fabbricato, "|")(2)
            '    sa_cod_magazzino = Split(objParametriAgenda.Fabbricato, "|")(1)
            '    fabbricatox_Cod = Split(objParametriAgenda.Fabbricato, "|")(0)
            'Else
            '    'se magazzino è della azienda padre
            '    Dim sa_cod_magazzino_predefinito_azienda As Integer
            '    Dim fabbricatox_Cod_magazzino_predefinito_azienda As Integer
            '    Dim fabbricati As New AgronicaCoreAnagrafeDAL.Fabbricati_R()
            '    fabbricati.Ricava_PrimoMagazzino_Impresa(Agenda.Piva, sa_cod_magazzino_predefinito_azienda, fabbricatox_Cod_magazzino_predefinito_azienda, objParametri_Server)
            '    sa_cod_magazzino = sa_cod_magazzino_predefinito_azienda
            '    fabbricatox_Cod = fabbricatox_Cod_magazzino_predefinito_azienda
            'End If

            ''Verifico che se il magazzino è gerarchico o se è del terzista ci sia selezionato solo quello tra gli scarichi del prodotto
            'Dim distinctFabbricati = Dt_Dosi.DefaultView.ToTable(True, "Piva", "Sa_Cod", "Fabbricato_Cod")
            'Dim Terzista As Boolean = False
            'Dim Gerarchico As Boolean = False
            'For Each row In distinctFabbricati.Rows
            '    If row("piva") <> "" AndAlso row("sa_cod") <> 0 AndAlso row("fabbricato_cod") <> 0 AndAlso row("piva") <> objParametriAgenda.Piva Then
            '        If isMagazzinoGerarchico(row("piva")) Then
            '            Gerarchico = True
            '        Else
            '            Terzista = True
            '        End If
            '    End If
            '    If (Terzista = True Or Gerarchico = True) AndAlso row("piva") = objParametriAgenda.Piva Then
            '        Throw New GiasException("Non è possibile selezionare più magazzini se uno di questi è di un terzista e/o è gerarchico")
            '    End If
            'Next

            'TODO_DT: per ora vanno bene così, gestire in seguito
            magazzinoEsterno = False
            magazzinoGerarchico = False

            If magazzinoEsterno = False OrElse
               magazzinoGerarchico OrElse
               (magazzinoEsterno AndAlso magazzinoGerarchico = False AndAlso infoOperazione.TipoOperazioneAgenda <> enum_Tipo_Operazione_Agenda.QuadernoDiCampagna) Then

                MovimentoScarico = New Movimento With {
                    .Id_Agenda = agenda.Id_Agenda,
                    .Piva = agenda.Piva,
                    .Sa_Cod = 0,
                    .Data = agenda.Data,
                    .Lav_Cod = agenda.Lav_Cod,
                    .Cau_Mov = CAU_SCARICO,
                    .Mov_Desc = "Scarico Magazzino",
                    .BaseCode = infoOperazione.BaseCode,
                    .TopCode = infoOperazione.TopCode
                }

                '----- MOVIMENTI DETTAGLI
                Dim Movimenti_Dettaglio_Scarico As List(Of Movimento_Dettaglio) = CreaMovimentiDettaglioScarico(attivita, agenda, infoOperazione, objParametri_Server)
                MovimentoScarico.Movimenti_Dettagli.AddRange(Movimenti_Dettaglio_Scarico)

            End If
        End If

        Return MovimentoScarico

    End Function

    Private Function CreaMovimentoCarico(attivita As Attivita, agenda As Operazione_Agenda, infoOperazione As InfoOperazione, eseguiVerificheConformita As Boolean) As Movimento
        'TODO_DT: rivedere tutte le parti commentate

        Dim MovimentoCarico As Movimento = Nothing

        'TODO_DT FARE: verificare che esista almeno un movimentoMagazzino in un dettaglio --> scriviMagazzino = True altrimenti esci
        Dim scriviMagazzino As Boolean = False
        'For Each row In Dt_Dosi.Rows
        '    If CStr(row("piva")) <> "" AndAlso CInt(row("Sa_Cod")) <> 0 AndAlso CInt(row("Fabbricato_Cod")) <> 0 Then
        '        scriviMagazzino = True
        '        Exit For
        '    End If
        'Next

        If infoOperazione.IsRaccolta AndAlso attivita.tipoRaccolta = Attivita.Tipo_Raccolta.Leggera_Con_Dettagli_Magazzino Then

            'Dim piva_magazzino As String = ""
            'Dim sa_cod_magazzino As String = ""
            'Dim fabbricatox_Cod As String = ""

            Dim magazzinoEsterno As Boolean = True
            Dim magazzinoGerarchico As Boolean = False

            If (eseguiVerificheConformita) Then
                'verificaConformitaMagazzini().---> TODO_DT: fare se eseguiVerificheConformita = True
            End If

            'TODO_DT: per ora vanno bene così, gestire in seguito
            magazzinoEsterno = False
            magazzinoGerarchico = False

            If magazzinoEsterno = False OrElse
               magazzinoGerarchico OrElse
               (magazzinoEsterno AndAlso magazzinoGerarchico = False AndAlso infoOperazione.TipoOperazioneAgenda <> enum_Tipo_Operazione_Agenda.QuadernoDiCampagna) Then

                MovimentoCarico = New Movimento With {
                    .Id_Agenda = agenda.Id_Agenda,
                    .Piva = agenda.Piva,
                    .Sa_Cod = agenda.Sa_Cod,
                    .Data = agenda.Data,
                    .Lav_Cod = agenda.Lav_Cod,
                    .Cau_Mov = CAU_CARICO,
                    .Mov_Desc = "Carico di Magazzino Da Raccolta",
                    .BaseCode = infoOperazione.BaseCode,
                    .TopCode = infoOperazione.TopCode
                }

                '----- MOVIMENTI DETTAGLI
                Dim dataIngresso As DateTime = AGRODATAFINE
                Dim oraIngresso As DateTime = AGRODATAFINE
                Dim Movimenti_Dettaglio_Carico As List(Of Movimento_Dettaglio) = CreaMovimentiDettaglioCarico(attivita, agenda, infoOperazione, dataIngresso, oraIngresso)
                MovimentoCarico.Data = dataIngresso
                MovimentoCarico.Ora = oraIngresso

                MovimentoCarico.Movimenti_Dettagli.AddRange(Movimenti_Dettaglio_Carico)

            End If
        End If

        Return MovimentoCarico

    End Function

    Private Function CreaMovimentoDettaglioTecnicoAcqua(attivita As Attivita, agenda As Operazione_Agenda, infoOperazione As InfoOperazione, SuperficieTrattataTotale As Decimal, ByRef acquaTotale As Decimal) As Movimento_Dettaglio_Tecnico

        Dim Movimento_Dettaglio_Tecnico_Acqua As Movimento_Dettaglio_Tecnico = Nothing

        If (infoOperazione.Elem_Cod = INSETTI AndAlso Not (attivita.tipo.Equals(Tipo_Attivita.QuadernoDiCampagna))) Then
            'Se stiamo salvando una ricetta di Distribuzione Insetti, e non mi arriva l'acqua dummy, l'aggiungo a mano
            'Nell'xml trattamernto, l'acqua è obbligatoria. Per non mettere mano al DAL, aggiungiamo qui
            If ((From a In attivita.risorse Where a.classType = costanti.ClassType.RisorsaAcqua).FirstOrDefault) Is Nothing Then
                attivita.risorse.Add(New RisorsaAcqua())
            End If
        End If

        If infoOperazione.IsTrattamento OrElse agenda.Lav_Cod = LAVCOD_CONCIMAZIONE_FOGLIARE OrElse agenda.Lav_Cod = LAVCOD_FERTIRRIGAZIONE OrElse agenda.Lav_Cod = LAVCOD_TRATTAMENTO_ANTIBUTTERATURA Then

            If Not IsNothing(attivita.risorse) Then

                'DT: si assume che ci sia al massimo una risorsa acqua
                Dim risorsaAcquaApplicata As risorse.RisorsaAcqua = (From a In attivita.risorse Where a.classType = costanti.ClassType.RisorsaAcqua).FirstOrDefault
                If risorsaAcquaApplicata IsNot Nothing Then
                    Movimento_Dettaglio_Tecnico_Acqua = New Movimento_Dettaglio_Tecnico With {
                                .Id_Agenda = agenda.Id_Agenda,
                                .Piva = agenda.Piva,
                                .Sa_Cod = agenda.Sa_Cod,
                                .Data = agenda.Data
                    }

                    Select Case risorsaAcquaApplicata.doseAcqua
                        Case AgronicaCoreModelsSTD.attivita.risorse.RisorsaAcqua.TipoDoseAcqua.TOTALE
                            Movimento_Dettaglio_Tecnico_Acqua.Qta_Ril = risorsaAcquaApplicata.acqua
                            acquaTotale = risorsaAcquaApplicata.acqua

                        Case AgronicaCoreModelsSTD.attivita.risorse.RisorsaAcqua.TipoDoseAcqua.HA
                            Movimento_Dettaglio_Tecnico_Acqua.Qta_Ril = 0 - risorsaAcquaApplicata.acqua
                            acquaTotale = risorsaAcquaApplicata.acqua * SuperficieTrattataTotale

                    End Select

                    Movimento_Dettaglio_Tecnico_Acqua.BaseCode = infoOperazione.BaseCode
                    Movimento_Dettaglio_Tecnico_Acqua.TopCode = infoOperazione.TopCode
                End If
            End If

        End If

        Return Movimento_Dettaglio_Tecnico_Acqua

    End Function

    Private Function CreaMovimentiDettagliTecniciCausali(attivita As Attivita, agenda As Operazione_Agenda, infoOperazione As InfoOperazione) As List(Of Movimento_Dettaglio_Tecnico)

        Dim Movimenti_Dettagli_Tecnici_Causali As List(Of Movimento_Dettaglio_Tecnico) = New List(Of Movimento_Dettaglio_Tecnico)

        If agenda.Lav_Cod = LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA Then
            If Not IsNothing(attivita.risorse) Then

                Dim risorseCausale As List(Of risorse.RisorsaCausale) = attivita.risorse.OfType(Of risorse.RisorsaCausale)().Where(Function(a) a.classType = ClassType.RisorsaCausale).ToList()

                If risorseCausale IsNot Nothing AndAlso risorseCausale.Count > 0 Then

                    For Each risorsaCausale In risorseCausale

                        Dim Movimento_Dettaglio_Tecnico_Causale = New Movimento_Dettaglio_Tecnico With {
                            .Id_Agenda = agenda.Id_Agenda,
                            .Piva = agenda.Piva,
                            .Sa_Cod = agenda.Sa_Cod,
                            .Data = agenda.Data,
                            .dett_cod = risorsaCausale.id,
                            .BaseCode = infoOperazione.BaseCode,
                            .TopCode = infoOperazione.TopCode
                        }

                        Movimenti_Dettagli_Tecnici_Causali.Add(Movimento_Dettaglio_Tecnico_Causale)

                    Next

                End If
            End If

        End If

        Return Movimenti_Dettagli_Tecnici_Causali

    End Function

    Private Function CreaMovimentoDettaglioTecnico(attivita As Attivita, agenda As Operazione_Agenda, risorsa As risorse.Risorsa, infoOperazione As InfoOperazione, dose As Decimal, objParametri_Super_Server As AgronicaCoreParametri, objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri) As Movimento_Dettaglio_Tecnico
        Dim Movimento_Dettaglio_Tecnico As Movimento_Dettaglio_Tecnico = Nothing
        If infoOperazione.IsTrattamento Then
            Dim trattamento As dettagli.DettaglioTrattamento = CType(risorsa, dettagli.DettaglioTrattamento)
            Dim av_cod As Integer = 0
            Dim av_gru As Integer = 0
            STD_Utility.GetCodiciAvversita(trattamento.avversitaGruppo, trattamento.tipoFormulato, av_cod, av_gru)
            Dim soglia_cod As Integer
            Dim soglia_des As String = ""
            Dim soglia_qta As Decimal
            Dim sigla_av As String = ""
            Dim extra_str As String = ""

            Dim ditta_cod As Integer = 0

            Dim freatimetro As Decimal   'CODICE PERSONALIZZATO TRAPPOLA, SALVIAMO?
            Dim trap_num As Integer

            If trattamento.soglia IsNot Nothing Then
                soglia_cod = trattamento.soglia.codice
                soglia_des = trattamento.soglia.descrizione
                soglia_qta = trattamento.soglia.quantita

                'DT: se la descrizione della soglia arriva vuota, viene ricostruita mediante richiamo al WS
                If String.IsNullOrEmpty(trattamento.soglia.descrizione) Then
                    Dim soglia = STD_Utility.getSoglia(soglia_cod, attivita.disciplinare, trattamento.avversitaGruppo, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)

                    If soglia IsNot Nothing Then
                        soglia_des = soglia.descrizione
                    End If
                End If

            End If

            If infoOperazione.Elem_Cod = INSETTI AndAlso trattamento.isImpollinatore Then
                av_cod = -1
                av_gru = -1

                sigla_av = "IMPOLL"
                extra_str = "IMPOLL"
            End If

            If attivita.job.primaryKey.codice = LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE OrElse
                attivita.job.primaryKey.codice = LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA OrElse
                attivita.job.primaryKey.codice = LAVCOD_REINNESCO_TRAPPOLE Then
                sigla_av = trattamento.avversitaGruppo.abbreviazione
            End If


            Movimento_Dettaglio_Tecnico = New Movimento_Dettaglio_Tecnico With {
                    .Av_Cod = av_cod,
                    .Av_Gru = av_gru,
                    .Soglia_Cod = soglia_cod,
                    .Soglia_Des = soglia_des,
                    .Soglia_Quantita = soglia_qta,
                    .Sigla_av = sigla_av,
                    .ExtraStr = extra_str,
                    .Ditta_cod = ditta_cod,
                    .Dose = dose,
                    .Freatimetro = freatimetro,
                    .Trap_num = trap_num
        }
        End If

        If infoOperazione.IsFertilizzazione Then

            Dim fertilizzazione As dettagli.DettaglioFertilizzazione = CType(risorsa, dettagli.DettaglioFertilizzazione)

            Movimento_Dettaglio_Tecnico = New Movimento_Dettaglio_Tecnico With {
                .N = fertilizzazione.N,
                .P = fertilizzazione.P,
                .K = fertilizzazione.K,
                .M = fertilizzazione.Mg,
                .Cu = fertilizzazione.Cu,
                .Efficienza = fertilizzazione.efficienza
            }

        End If

        If infoOperazione.IsIrrigazione Then

            Dim irrigazione As dettagli.DettaglioIrrigazione = CType(risorsa, dettagli.DettaglioIrrigazione)

            ImpostaDateLimiteSeNecessario(irrigazione)

            Movimento_Dettaglio_Tecnico = New Movimento_Dettaglio_Tecnico With {
                .Qta_Ril = irrigazione.QtaRilevata,
                .dett_cod = If(irrigazione.unitaDiMisura IsNot Nothing, irrigazione.unitaDiMisura.codice, 0),
                .Dose = irrigazione.Ore,
                .Parziale = irrigazione.Portata,
                .Efficienza = irrigazione.Efficienza,
                .Nitrati = irrigazione.Frequenza,
                .Freatimetro = If(irrigazione.tipoIrrigazione IsNot Nothing, irrigazione.tipoIrrigazione.codice, 0),
                .Ditta_cod = If(irrigazione.macchina IsNot Nothing, irrigazione.macchina.codice, 0),
                .Inn1_data = irrigazione.DataInizio,
                .Inn2_data = irrigazione.DataFine,
                .Extra_Int = If(irrigazione.consiglioIrrigazione IsNot Nothing, irrigazione.consiglioIrrigazione.codice, 0),
                .ExtraStr = If(irrigazione.consiglioIrrigazione IsNot Nothing, irrigazione.consiglioIrrigazione.qtaAcqua.ToString(), ""),
                .Extra_Date = If(irrigazione.consiglioIrrigazione IsNot Nothing, irrigazione.consiglioIrrigazione.dataConsiglio, Nothing)
            }

        End If

        If infoOperazione.IsRilievo Then
            Movimento_Dettaglio_Tecnico = CreaMovimentoDettaglioTecnicoRilievo(risorsa, agenda)
        End If

        If Movimento_Dettaglio_Tecnico IsNot Nothing Then
            With Movimento_Dettaglio_Tecnico
                .Id_Agenda = agenda.Id_Agenda
                .Piva = agenda.Piva
                .Sa_Cod = agenda.Sa_Cod
                .Data = agenda.Data
                .BaseCode = infoOperazione.BaseCode
                .TopCode = infoOperazione.TopCode
            End With
        End If

        Return Movimento_Dettaglio_Tecnico

    End Function

    Private Sub ImpostaDateLimiteSeNecessario(ByRef irrigazione As DettaglioIrrigazione)
        If irrigazione.esercizioCDC?.esercizio?.validita?.inizio IsNot Nothing AndAlso irrigazione.esercizioCDC.esercizio.validita.inizio > irrigazione.DataInizio Then
            irrigazione.DataInizio = irrigazione.esercizioCDC.esercizio.validita.inizio
        End If

        If irrigazione.esercizioCDC?.esercizio?.validita?.fine IsNot Nothing AndAlso irrigazione.esercizioCDC.esercizio.validita.fine < irrigazione.DataFine Then
            irrigazione.DataFine = irrigazione.esercizioCDC.esercizio.validita.fine
        End If
    End Sub

    Private Function CreaMovimentiDettaglioTecnicoIrrigazione(attivita As Attivita, agenda As Operazione_Agenda, irrigazione As DettaglioIrrigazione, infoOperazione As InfoOperazione, objParametri_Super_Server As AgronicaCoreParametri, objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri) As List(Of Movimento_Dettaglio_Tecnico)

        Dim Movimenti_Dettaglio_Tecnico As New List(Of Movimento_Dettaglio_Tecnico)

        If irrigazione IsNot Nothing Then

            ImpostaDateLimiteSeNecessario(irrigazione)

            Dim Movimento_Dettaglio_Tecnico = New Movimento_Dettaglio_Tecnico With {
                .Id_Agenda = agenda.Id_Agenda,
                .Data = agenda.Data,
                .Piva = agenda.Piva,
                .Sa_Cod = agenda.Sa_Cod,
                .BaseCode = infoOperazione.BaseCode,
                .TopCode = infoOperazione.TopCode,
                .Qta_Ril = irrigazione.QtaRilevata,
                .dett_cod = If(irrigazione.unitaDiMisura IsNot Nothing, irrigazione.unitaDiMisura.codice, 0),
                .Dose = irrigazione.Ore,
                .Parziale = irrigazione.Portata,
                .Efficienza = irrigazione.Efficienza,
                .Nitrati = irrigazione.Frequenza,
                .Freatimetro = If(irrigazione.tipoIrrigazione IsNot Nothing, irrigazione.tipoIrrigazione.codice, 0),
                .Ditta_cod = If(irrigazione.macchina IsNot Nothing, irrigazione.macchina.codice, 0),
                .Inn1_data = irrigazione.DataInizio,
                .Inn2_data = irrigazione.DataFine,
                .Extra_Int = If(irrigazione.consiglioIrrigazione IsNot Nothing, irrigazione.consiglioIrrigazione.codice, 0),
                .ExtraStr = If(irrigazione.consiglioIrrigazione IsNot Nothing, irrigazione.consiglioIrrigazione.qtaAcqua.ToString(), ""),
                .Extra_Date = If(irrigazione.consiglioIrrigazione IsNot Nothing, irrigazione.consiglioIrrigazione.dataConsiglio, Nothing)
            }

            Movimenti_Dettaglio_Tecnico.Add(Movimento_Dettaglio_Tecnico)

        End If

        Return Movimenti_Dettaglio_Tecnico

    End Function

    Private Function CreaMovimentoDettaglioTecnicoRilievo(risorsa As risorse.Risorsa, agenda As Operazione_Agenda) As Movimento_Dettaglio_Tecnico
        Dim rilievo As dettagli.DettaglioRilievo = CType(risorsa, dettagli.DettaglioRilievo)
        Dim av_cod As Integer = 0
        Dim av_gru As Integer = 0
        Dim ff_classe As Integer = 0
        Dim qta_ril As Decimal = 0
        Dim id_insetto As Integer = 0

        Select Case agenda.Lav_Cod
            Case LAVCOD_RILIEVO_AVVERSITA_CAMPO
                STD_Utility.GetCodiciAvversita(rilievo.avversitaGruppo, tipoFormulato:=0, av_cod, av_gru)

            Case LAVCOD_RILIEVO_INDICI_MATURITA
                ff_classe = rilievo.IndiceMaturita

            Case LAVCOD_DANNI_RACCOLTA
                ff_classe = rilievo.DannoRaccolta

            Case LAVCOD_FASI_FENOLOGICHE
                If Not IsNothing(rilievo.faseFenologica) Then
                    ff_classe = rilievo.faseFenologica.codice
                End If

            Case LAVCOD_RILIEVO_ERBE_INFESTANTI
                STD_Utility.GetCodiciAvversita(rilievo.erbaInfestante, tipoFormulato:=0, av_cod, av_gru)

            Case LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA
                ff_classe = rilievo.IndiceResa

            Case LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE
                STD_Utility.GetCodiciAvversita(rilievo.avversitaGruppo, tipoFormulato:=0, av_cod, av_gru)
                qta_ril = rilievo.QtaRilevata

        End Select

        Return New Movimento_Dettaglio_Tecnico With {
            .Qta_Ril = qta_ril,
            .Id_Insetto = id_insetto,
            .dett_cod = If(rilievo.unitaDiMisura IsNot Nothing, rilievo.unitaDiMisura.codice, 0),
            .Av_Cod = av_cod,
            .Av_Gru = av_gru,
            .ff_classe = ff_classe
        }
    End Function

    Private Function CreaMovimentiDettaglio(attivita As Attivita, agenda As Operazione_Agenda, infoOperazione As InfoOperazione, ByRef MovimentoCampagna As Movimento, superficieTrattataTotale As Decimal, acquaTotale As Decimal, objParametri_Super_Server As AgronicaCoreParametri, ByVal objParametri_Server As AgronicaCoreParametri, ByVal objParametri_Utenti As AgronicaCoreParametri) As List(Of Movimento_Dettaglio)
        'DT: possono arrivare situazioni complesse, con tutti i seguenti casi presenti:
        '- risorseProdotti senza prodotto (solo per la semina: verranno salvate solo le destinazioni, associate a un prodotto 0 a qta 0)
        '- risorseProdotti con prodotto senza magazzino (anche più risorse con lo stesso prodotto identico)
        '- risorseProdotti con prodotto e con magazzino 
        '    - nel magazzino ci possono essere i lotti o meno (lotto vuoto), può esserci anche lo stesso lotto ripetuto
        '    - lo stesso lotto può apparire in diverse risorseProdotto e/o in diversi rilevamenti di magazzino
        'Vengono pertanto creati tanti movimenti di dettaglio quante sono le chiavi distinte delle risorse prodotto:
        '- per la semina --> prodotto/lotto
        '- per la fertilizzazione --> prodotto
        '- per il trattamento --> prodotto
        '- per la raccolta --> prodotto/lotto
        'sommando le quantita

        Dim Lav_Cod As Integer = CInt(attivita.job.primaryKey.codice)

        If infoOperazione.IsRilievo Then
            Return CreaMovimentiDettaglioRilievo(attivita, agenda, infoOperazione, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
        End If

        Dim risorseProdotto As List(Of risorse.RisorsaProdotto) = attivita.risorse.FindAll(Function(c) (c.classType = ClassType.DettaglioTrattamento OrElse c.classType = ClassType.DettaglioFertilizzazione OrElse c.classType = ClassType.DettaglioSemina OrElse c.classType = ClassType.DettaglioRaccolta)).ConvertAll(Function(obj1) CType(obj1, risorse.RisorsaProdotto))
        Dim foundRisorsa As Boolean = False

        Dim Movimenti_Dettagli As New List(Of Movimento_Dettaglio)

        If risorseProdotto IsNot Nothing AndAlso risorseProdotto.Count > 0 Then

            'Se sono nel caso di raccolta con lotto generato da codice esercizio
            If infoOperazione.IsRaccolta AndAlso attivita.tipoRaccolta <> Attivita.Tipo_Raccolta.Fast AndAlso
                (CType(risorseProdotto.First(), DettaglioRaccolta).Opzioni_Raccolta.GenerazioneLotto.Equals(enum_Generazione_Lotto_Raccolta.DA_ESERCIZO) OrElse
                    CType(risorseProdotto.First(), DettaglioRaccolta).Opzioni_Raccolta.GenerazioneLotto.Equals(enum_Generazione_Lotto_Raccolta.MANUALE)) Then

                EsplodiImpiantiRaccolta(risorseProdotto, attivita)
            End If

            For Each risorsaProdotto In risorseProdotto

                If risorsaProdotto.prodotto Is Nothing OrElse risorsaProdotto.prodotto.codice = 0 Then
                    Continue For
                End If

                foundRisorsa = True

                Dim pkProdotto = risorsaProdotto.prodotto.codice
                Dim pkSaCod = agenda.Sa_Cod

                If risorsaProdotto.MagazziniMovimentazioni IsNot Nothing AndAlso risorsaProdotto.MagazziniMovimentazioni.Count > 0 Then
                    For Each movimentoMagazzino In risorsaProdotto.MagazziniMovimentazioni

                        Dim pkLotto = ""
                        If infoOperazione.IsSemina Then
                            pkLotto = movimentoMagazzino.Lotto.ToUpper

                        ElseIf infoOperazione.IsRaccolta Then
                            Dim dettaglioProdotto = CType(risorsaProdotto, DettaglioRaccolta)
                            pkSaCod = dettaglioProdotto.QuantitaSuImpianti.First().esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice

                            If pkProdotto = -1 Then
                                pkProdotto = GeneraProdotto(CType(attivita.utilizzoTerreno, Varieta).specie, TRASFORMATI_VEGETALI,
                                                                objParametri_Utenti, objParametri_Server)

                                risorsaProdotto.prodotto.codice = pkProdotto
                                movimentoMagazzino.Prodotto.codice = pkProdotto
                            End If

                            pkLotto = GeneraLottoRaccolta(risorsaProdotto, agenda, objParametri_Server)

                            '' Eliminata gestione calCod
                            'If IsNothing(dettaglioProdotto.calCod) OrElse dettaglioProdotto.calCod = 0 Then
                            '    dettaglioProdotto.calCod = GeneraCampionatura(agenda.Data, objParametri_Server)
                            'End If
                        End If

                        Dim movimentoDettaglio As Movimento_Dettaglio = Nothing
                        If infoOperazione.IsTrattamento OrElse infoOperazione.IsFertilizzazione Then
                            movimentoDettaglio = Movimenti_Dettagli.Find(Function(c) (c.Pro_Cod = pkProdotto))
                        ElseIf infoOperazione.IsRaccolta OrElse infoOperazione.IsSemina Then
                            movimentoDettaglio = Movimenti_Dettagli.Find(Function(c) (c.Mat_Cod = pkProdotto AndAlso
                                                                                 c.Lotto.ToUpper = pkLotto.ToUpper AndAlso
                                                                                 c.Sa_Cod = pkSaCod))
                        End If

                        If movimentoDettaglio Is Nothing Then
                            movimentoDettaglio = CreaMovimentoDettaglio(risorsaProdotto, movimentoMagazzino, attivita, agenda, infoOperazione, superficieTrattataTotale, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)

                            Movimenti_Dettagli.Add(movimentoDettaglio)
                        Else
                            checkCompatibilita(agenda, attivita, risorsaProdotto, movimentoDettaglio, infoOperazione, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
                        End If

                        updateQtaLottoMovimentoDettaglio(movimentoDettaglio, GetDosePerMagazzino(movimentoMagazzino.Qta, risorsaProdotto.unitaDiMisuraIndicata.codice), movimentoMagazzino.doseHlIndicata, movimentoMagazzino.doseHaIndicata, superficieTrattataTotale, acquaTotale, movimentoMagazzino.Lotto, infoOperazione, Nothing)

                    Next
                Else 'Non ho movimentazioni di magazzino
                    Dim movimentoDettaglio As Movimento_Dettaglio = Movimenti_Dettagli.Find(Function(c) ((c.Pro_Cod = pkProdotto OrElse c.Mat_Cod = pkProdotto) AndAlso (c.Lotto = "")))

                    If movimentoDettaglio Is Nothing Then
                        movimentoDettaglio = CreaMovimentoDettaglio(risorsaProdotto, movimentoMagazzino:=Nothing, attivita, agenda, infoOperazione, superficieTrattataTotale, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
                        Movimenti_Dettagli.Add(movimentoDettaglio)
                    Else
                        checkCompatibilita(agenda, attivita, risorsaProdotto, movimentoDettaglio, infoOperazione, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
                    End If

                    updateQtaLottoMovimentoDettaglio(movimentoDettaglio, risorsaProdotto.quantitaTotaleReale, risorsaProdotto.doseHlReale, risorsaProdotto.doseHaReale, superficieTrattataTotale, acquaTotale, lotto:="", infoOperazione, Nothing)

                End If
            Next
        End If



        Dim risorseIrrigazione As List(Of dettagli.DettaglioIrrigazione) = attivita.risorse.FindAll(Function(c) (c.classType = ClassType.DettaglioIrrigazione)).ConvertAll(Function(obj1) CType(obj1, dettagli.DettaglioIrrigazione))

        If risorseIrrigazione IsNot Nothing AndAlso risorseIrrigazione.Count > 0 Then

            For Each dettaglioIrrigazione In risorseIrrigazione

                Dim Movimento_Dettaglio_Irrigazione As New Movimento_Dettaglio With {
                    .Id_Agenda = agenda.Id_Agenda,
                    .Piva = agenda.Piva,
                    .Sa_Cod = agenda.Sa_Cod,
                    .Data = agenda.Data,
                    .Lav_Cod = agenda.Lav_Cod,
                    .Cau_Mov = infoOperazione.Cau_Mov,
                    .Elem_Cod = If(infoOperazione.IsFertirrigazione, CostantiPersonalizzate.ALTRE_MATERIE, infoOperazione.Elem_Cod),
                    .Mat_Cod = If(infoOperazione.IsFertirrigazione, CostantiPersonalizzate.MAT_COD_ACQUA_IRRIGAZIONE, 0),
                    .Contabilizzato = NONCONTABILE,
                    .BaseCode = infoOperazione.BaseCode,
                    .TopCode = infoOperazione.TopCode,
                    .Mezzo_Det = -1
                }

                Movimenti_Dettagli.Add(Movimento_Dettaglio_Irrigazione)
                Movimento_Dettaglio_Irrigazione.Movimenti_Destinazioni = CreaMovimentoDestinazioneIrrigazione(attivita, agenda, infoOperazione, dettaglioIrrigazione, superficieTrattataTotale)
                Movimento_Dettaglio_Irrigazione.Movimenti_Dettagli_Tecnici = CreaMovimentiDettaglioTecnicoIrrigazione(attivita, agenda, dettaglioIrrigazione, infoOperazione, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)

            Next

        End If

        If infoOperazione.IsVisita Then

            Dim risorseVisita As List(Of risorse.Risorsa) = attivita.risorse.FindAll(Function(c) (c.classType = ClassType.RisorsaSpecie OrElse c.classType = ClassType.RisorsaDestinazioneUso OrElse c.classType = ClassType.RisorsaZootecnica)).ConvertAll(Function(obj1) CType(obj1, risorse.Risorsa))
            Dim foundSpecie = False
            If risorseVisita IsNot Nothing AndAlso risorseVisita.Count > 0 Then
                For Each risorsaVisita In risorseVisita

                    Dim Movimento_Dettaglio_Visita As New Movimento_Dettaglio With {
                        .Id_Agenda = agenda.Id_Agenda,
                        .Piva = agenda.Piva,
                        .Sa_Cod = agenda.Sa_Cod,
                        .Data = agenda.Data,
                        .Lav_Cod = agenda.Lav_Cod,
                        .Cau_Mov = infoOperazione.Cau_Mov,
                        .Elem_Cod = infoOperazione.Elem_Cod,
                        .Contabilizzato = NONCONTABILE,
                        .BaseCode = infoOperazione.BaseCode,
                        .TopCode = infoOperazione.TopCode,
                        .Mezzo_Det = -1,
                        .Mov_Det_Des = attivita.descrizione,
                        .Pendente = 3,
                        .Anno = 1900
                    }

                    Select Case risorsaVisita.classType

                        Case ClassType.RisorsaSpecie
                            Dim risorsaSpecie = CType(risorsaVisita, RisorsaSpecie)
                            If risorsaSpecie IsNot Nothing AndAlso risorsaSpecie.specie IsNot Nothing Then
                                If risorsaSpecie.specie.codice > 0 Then
                                    Movimento_Dettaglio_Visita.Veg_Cod = risorsaSpecie.specie.codice
                                    Movimenti_Dettagli.Add(Movimento_Dettaglio_Visita)

                                    foundSpecie = True
                                End If
                            End If

                        Case ClassType.RisorsaDestinazioneUso
                            Dim risorsaDestinazioneUso = CType(risorsaVisita, RisorsaDestinazioneUso)
                            If risorsaDestinazioneUso IsNot Nothing AndAlso risorsaDestinazioneUso.destinazioneUso IsNot Nothing Then
                                If risorsaDestinazioneUso.destinazioneUso.codice > 0 Then
                                    Movimento_Dettaglio_Visita.Id_Cod = risorsaDestinazioneUso.destinazioneUso.codice
                                    Movimenti_Dettagli.Add(Movimento_Dettaglio_Visita)

                                    foundSpecie = True
                                End If
                            End If

                        Case ClassType.RisorsaZootecnica
                            Dim risorsaZootecnica = CType(risorsaVisita, RisorsaZootecnica)
                            If risorsaZootecnica IsNot Nothing AndAlso risorsaZootecnica.genere IsNot Nothing AndAlso risorsaZootecnica.specie IsNot Nothing AndAlso risorsaZootecnica.indirizzoProd IsNot Nothing Then
                                If risorsaZootecnica.genere.codice > 0 AndAlso risorsaZootecnica.specie.codice > 0 Then
                                    Movimento_Dettaglio_Visita.Gen_Cod = risorsaZootecnica.genere.codice
                                    Movimento_Dettaglio_Visita.Spe_Cod = risorsaZootecnica.specie.codice
                                    Movimento_Dettaglio_Visita.IPro_Cod = risorsaZootecnica.indirizzoProd.codice
                                    Movimenti_Dettagli.Add(Movimento_Dettaglio_Visita)

                                    foundSpecie = True
                                End If
                            End If

                    End Select

                Next

            End If

            If Not foundSpecie Then

                Dim Movimento_Dettaglio_Visita As New Movimento_Dettaglio With {
                    .Id_Agenda = agenda.Id_Agenda,
                    .Piva = agenda.Piva,
                    .Sa_Cod = agenda.Sa_Cod,
                    .Data = agenda.Data,
                    .Lav_Cod = agenda.Lav_Cod,
                    .Cau_Mov = infoOperazione.Cau_Mov,
                    .Elem_Cod = infoOperazione.Elem_Cod,
                    .Contabilizzato = NONCONTABILE,
                    .BaseCode = infoOperazione.BaseCode,
                    .TopCode = infoOperazione.TopCode,
                    .Mezzo_Det = -1,
                    .Mov_Det_Des = attivita.descrizione,
                    .Pendente = 3,
                    .Anno = 1900
                }
                Movimenti_Dettagli.Add(Movimento_Dettaglio_Visita)

            End If

        End If

        Dim Movimento_Dettaglio As New Movimento_Dettaglio With {
                    .Id_Agenda = agenda.Id_Agenda,
                    .Piva = agenda.Piva,
                    .Sa_Cod = agenda.Sa_Cod,
                    .Data = agenda.Data,
                    .Lav_Cod = agenda.Lav_Cod,
                    .Elem_Cod = infoOperazione.Elem_Cod,
                    .Contabilizzato = NONCONTABILE,
                    .BaseCode = infoOperazione.BaseCode,
                    .TopCode = infoOperazione.TopCode,
                    .Mezzo_Det = -1
            }

        If infoOperazione.IsLavorazione OrElse infoOperazione.isNonUtilizzo OrElse infoOperazione.IsAbbattimento Then
            Movimenti_Dettagli.Add(Movimento_Dettaglio)
        End If

        'DT: esiste la semina senza prodotti, va creato un movimento di dettaglio con prodotto=0 e qta=0 a cui associare le destinazioni
        If Not foundRisorsa AndAlso infoOperazione.IsSemina Then
            With Movimento_Dettaglio
                .Mov_Det_Des = My.Resources.AgronicaCoreMapper.DettagliTecniciMateriaPrimaSeminaTrapianto
                .Pendente = 2
                .Anno = 1900
            End With

            Movimenti_Dettagli.Add(Movimento_Dettaglio)
        End If

        'TODO_DT: raccolta FAST? Anny
        If Not foundRisorsa AndAlso infoOperazione.IsRaccolta Then
            With Movimento_Dettaglio
                .Mov_Det_Des = My.Resources.AgronicaCoreMapper.DettagliProdottiAziendaliRaccolta
                .Pendente = 3
                .Anno = 1900
                .Lotto = ""
            End With

            If IsNothing(Movimento_Dettaglio.Cal_Cod) OrElse Movimento_Dettaglio.Cal_Cod = 0 Then
                ' -- 06/06/2023 Si è deciso di eliminare la gestione dei Cal_Cod per la raccolta
                'Movimento_Dettaglio.Cal_Cod = 12 ' GeneraCampionatura(Movimento_Dettaglio.Data, objParametri_Server)
                Movimento_Dettaglio.Cal_Cod = 0
            End If

            Movimenti_Dettagli.Add(Movimento_Dettaglio)
        End If

        For Each movimentoDettaglio In Movimenti_Dettagli

            If infoOperazione.IsRaccolta AndAlso attivita.tipoRaccolta = Attivita.Tipo_Raccolta.Leggera_Con_Dettagli_Magazzino Then
                'If IsNothing(movimentoDettaglio.Cal_Cod) OrElse movimentoDettaglio.Cal_Cod = 0 Then
                '    movimentoDettaglio.Cal_Cod = GeneraCampionatura(agenda.Data, objParametri_Server)
                'End If
                movimentoDettaglio.Cal_Cod = 0
            End If

            If superficieTrattataTotale > 0 Then

                '----- MOVIMENTI DESTINAZIONI
                Dim DoseHA_Trasformata As Decimal = 0
                If infoOperazione.IsSemina OrElse infoOperazione.IsRaccolta OrElse infoOperazione.Elem_Cod = TRAPPOLE Then
                    'Tutte le operazioni che lavorano esclusivamente per QUANTITA' TOTALE devono passare da qui
                    DoseHA_Trasformata = GetDoseTrasformata(movimentoDettaglio.Qta, movimentoDettaglio.Extra_Int) / superficieTrattataTotale
                Else
                    DoseHA_Trasformata = GetDoseTrasformata(movimentoDettaglio.Qta, movimentoDettaglio.Extra_Int)
                End If

                If infoOperazione.IsRaccolta Then
                    Dim codiceProdotto = movimentoDettaglio.Mat_Cod
                    Dim lotto = movimentoDettaglio.Lotto

                    Dim prodotti As List(Of DettaglioRaccolta) = risorseProdotto.ConvertAll(Function(obj) CType(obj, DettaglioRaccolta))
                    prodotti = (From prd In prodotti
                                Where prd.prodotto.codice = codiceProdotto AndAlso
                                    prd.QuantitaSuImpianti.First().Lotto.ToUpper = lotto.ToUpper
                                Select prd).ToList()

                    Dim QuantitaSuImpianti = RipartizionaQuantitaRaccolta(prodotti, Function(q) (q.Lotto))

                    If QuantitaSuImpianti IsNot Nothing AndAlso QuantitaSuImpianti.Count > 0 Then
                        movimentoDettaglio.Movimenti_Destinazioni = CreaMovimentiDestinazioneDaQuantitaSuImpianti(QuantitaSuImpianti.Values.ToList(),
                                                                                          agenda, infoOperazione,
                                                                                          superficieTrattataTotale)
                    Else
                        movimentoDettaglio.Movimenti_Destinazioni = CreaMovimentiDestinazione(agenda, infoOperazione, attivita.centriDiCosto, DoseHA_Trasformata, superficieTrattataTotale)
                    End If
                Else

                    If Lav_Cod = LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA OrElse Lav_Cod = LAVCOD_REINNESCO_TRAPPOLE Then
                        movimentoDettaglio.Movimenti_Destinazioni = CreaMovimentiDestinazioniTrappole(attivita, agenda, movimentoDettaglio, infoOperazione, superficieTrattataTotale)
                    Else

                        If Not infoOperazione.IsIrrigazione AndAlso infoOperazione.TipoCentroDiCosto <> centri_di_costo.Tipo.ProdottoDaTrattare Then
                            If Not infoOperazione.IsFertirrigazione OrElse Not movimentoDettaglio.IsDettaglioIrrigazione Then
                                movimentoDettaglio.Movimenti_Destinazioni = CreaMovimentiDestinazione(agenda, infoOperazione, attivita.centriDiCosto, DoseHA_Trasformata, superficieTrattataTotale)
                            End If
                        End If

                    End If


                End If

            End If

            '----- MOVIMENTI DETTAGLI RIFERIMENTI
            'TODO_DT? fare?? attenzione alla cardinalità
            movimentoDettaglio.Movimenti_Dettagli_Riferimenti = CreaMovimentiDettagliRiferimenti(attivita, agenda, infoOperazione)

        Next

        If infoOperazione.TipoCentroDiCosto = centri_di_costo.Tipo.ProdottoDaTrattare Then
            'Creiamo una riga di dettaglio per ogni Prodotto da Trattare selezionato a cui verrà associata la relativa destinazione (provenienza risorse trattate)
            For Each centroDiCosto In attivita.centriDiCosto
                Dim prodottoDaTrattareCdC As centri_di_costo.ProdottoDaTrattareCDC = CType(centroDiCosto, centri_di_costo.ProdottoDaTrattareCDC)

                Dim prodotto As Prodotto = prodottoDaTrattareCdC.giacenzaMagazzino.Prodotto

                Dim qtaToKG As Decimal = AgronicaCoreMetaSchemaDAL.UnitaMisura_R.Converti(enum_UnitaMisura.Quintali, prodottoDaTrattareCdC.qtaTrattata, enum_UnitaMisura.KG)

                Dim Movimento_Dettaglio_GiacenzaMagazzino As New Movimento_Dettaglio With {
                        .Id_Agenda = agenda.Id_Agenda,
                        .Piva = agenda.Piva,
                        .Sa_Cod = agenda.Sa_Cod,
                        .Data = agenda.Data,
                        .Lav_Cod = agenda.Lav_Cod,
                        .Cau_Mov = CAU_TRATTAMENTO,
                        .Elem_Cod = prodotto.elemCod,
                        .Contabilizzato = NONCONTABILE,
                        .BaseCode = infoOperazione.BaseCode,
                        .TopCode = infoOperazione.TopCode,
                        .Mezzo_Det = -1,
                        .Mat_Cod = prodotto.codice,
                        .Cod_Progetto = prodottoDaTrattareCdC.giacenzaMagazzino.codice_progetto, 'todo
                        .Cal_Cod = 0, 'todo?
                        .Lotto = prodottoDaTrattareCdC.giacenzaMagazzino.Lotto,
                        .Udm_Cod = enum_UnitaMisura.KG,
                        .Qta = qtaToKG 'La dose in griglia è sempre espressa in quintali, su db salviamo sempre in KG/Lt
                    }

                If superficieTrattataTotale > 0 Then
                    '----- MOVIMENTI DESTINAZIONI
                    Dim DoseHA_Trasformata As Decimal = GetDoseTrasformata(Movimento_Dettaglio_GiacenzaMagazzino.Qta, Movimento_Dettaglio_GiacenzaMagazzino.Extra_Int)

                    Dim Movimento_Destinazione As New Movimento_Destinazione With {
                        .Id_Agenda = agenda.Id_Agenda,
                        .Data = agenda.Data,
                        .Piva = prodottoDaTrattareCdC.giacenzaMagazzino.Magazzino.primaryKey.centroAziendalePK.partitaIva,
                        .Sa_Cod = prodottoDaTrattareCdC.giacenzaMagazzino.Magazzino.primaryKey.centroAziendalePK.codice,
                        .Appezza = 0,
                        .Id_Destinazione = prodottoDaTrattareCdC.giacenzaMagazzino.Magazzino.primaryKey.codice,
                        .Tipo = MAGAZZINO,
                        .BaseCode = infoOperazione.BaseCode,
                        .TopCode = infoOperazione.TopCode,
                        .Programmazione_Entita_Cod = 0 'DT: valorizzato solo in caso di salvataggio planning
                    }
                    Movimento_Dettaglio_GiacenzaMagazzino.Movimenti_Destinazioni = New List(Of Movimento_Destinazione) From {Movimento_Destinazione}
                End If

                Movimenti_Dettagli.Add(Movimento_Dettaglio_GiacenzaMagazzino)
            Next

        End If
        Return Movimenti_Dettagli
    End Function

    Private Function CreaMovimentiDettaglioRilievo(attivita As Attivita, agenda As Operazione_Agenda, infoOperazione As InfoOperazione, objParametri_Super_Server As AgronicaCoreParametri, objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri) As List(Of Movimento_Dettaglio)

        Dim Movimenti_Dettagli As New List(Of Movimento_Dettaglio)

        Dim risorseRilievo As List(Of dettagli.DettaglioRilievo) = attivita.risorse.FindAll(Function(c) (c.classType = ClassType.DettaglioRilievo)).ConvertAll(Function(obj1) CType(obj1, dettagli.DettaglioRilievo))
        If risorseRilievo IsNot Nothing AndAlso risorseRilievo.Count > 0 Then

            Dim rilievoSenzaImpianti As Boolean = Utility.RilievoSenzaImpianti(attivita)

            Dim dictRilievi As New HashSet(Of String)
            For Each dettaglioRilievo In risorseRilievo

                Dim rilievoKey As String = ""
                Select Case agenda.Lav_Cod
                    Case LAVCOD_RILIEVO_AVVERSITA_CAMPO, LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE

                        Dim av_cod As Integer = 0
                        Dim av_gru As Integer = 0
                        Dim udm_cod As Integer = 0
                        Dim pro_cod As Integer = 0

                        If dettaglioRilievo.avversitaGruppo IsNot Nothing Then
                            STD_Utility.GetCodiciAvversita(dettaglioRilievo.avversitaGruppo, tipoFormulato:=0, av_cod, av_gru)
                        End If

                        If dettaglioRilievo.unitaDiMisura IsNot Nothing Then
                            udm_cod = dettaglioRilievo.unitaDiMisura.codice
                        End If

                        If dettaglioRilievo.risorsaProdotto IsNot Nothing AndAlso dettaglioRilievo.risorsaProdotto.prodotto IsNot Nothing Then
                            pro_cod = dettaglioRilievo.risorsaProdotto.prodotto.codice
                        End If

                        rilievoKey = av_cod & "-" & av_gru & "-" & udm_cod & "-" & pro_cod

                    Case LAVCOD_RILIEVO_ERBE_INFESTANTI

                        Dim av_cod As Integer = 0
                        Dim av_gru As Integer = 0

                        If dettaglioRilievo.erbaInfestante IsNot Nothing Then
                            STD_Utility.GetCodiciAvversita(dettaglioRilievo.erbaInfestante, tipoFormulato:=0, av_cod, av_gru)
                        End If

                        rilievoKey = av_cod & "-" & av_gru

                    Case LAVCOD_DANNI_RACCOLTA
                        rilievoKey = dettaglioRilievo.DannoRaccolta

                    Case LAVCOD_RILIEVO_INDICI_MATURITA
                        rilievoKey = dettaglioRilievo.IndiceMaturita

                    Case LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA
                        rilievoKey = dettaglioRilievo.IndiceResa

                    Case LAVCOD_FASI_FENOLOGICHE
                        If Not IsNothing(dettaglioRilievo.faseFenologica) Then
                            rilievoKey = dettaglioRilievo.faseFenologica.codice
                        End If
                End Select

                Dim impiantoKey As String = GetImpiantoRilievoKey(attivita, dettaglioRilievo)

                If dictRilievi.Contains(rilievoKey & impiantoKey) Then
                    Throw New GiasException(My.Resources.AgronicaCoreMapper.RilievoEsistentePerImpianto)
                Else
                    dictRilievi.Add(rilievoKey & impiantoKey)
                End If

            Next

            Select Case agenda.Lav_Cod
                Case LAVCOD_RILIEVO_AVVERSITA_CAMPO, LAVCOD_RILIEVO_ERBE_INFESTANTI, LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE

                    Dim dictAvversita As New Dictionary(Of String, (Movimento_Dettaglio, Decimal))

                    For Each dettaglioRilievo In risorseRilievo

                        Dim av_cod As Integer = 0
                        Dim av_gru As Integer = 0
                        Dim udm_cod As Integer = 0
                        Dim pro_cod As Integer = 0

                        Select Case agenda.Lav_Cod
                            Case LAVCOD_RILIEVO_AVVERSITA_CAMPO,
                                 LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE
                                If dettaglioRilievo.avversitaGruppo IsNot Nothing Then
                                    STD_Utility.GetCodiciAvversita(dettaglioRilievo.avversitaGruppo, tipoFormulato:=0, av_cod, av_gru)
                                End If

                                If dettaglioRilievo.unitaDiMisura IsNot Nothing Then
                                    udm_cod = dettaglioRilievo.unitaDiMisura.codice
                                End If

                                If dettaglioRilievo.risorsaProdotto IsNot Nothing AndAlso dettaglioRilievo.risorsaProdotto.prodotto IsNot Nothing Then
                                    pro_cod = dettaglioRilievo.risorsaProdotto.prodotto.codice
                                End If

                            Case LAVCOD_RILIEVO_ERBE_INFESTANTI
                                If dettaglioRilievo.erbaInfestante IsNot Nothing Then
                                    STD_Utility.GetCodiciAvversita(dettaglioRilievo.erbaInfestante, tipoFormulato:=0, av_cod, av_gru)
                                End If
                        End Select

                        If av_cod <> 0 OrElse av_gru <> 0 Then

                            Dim avKey As String = av_cod & "-" & av_gru & "-" & udm_cod & "-" & pro_cod

                            If Not dictAvversita.ContainsKey(avKey) Then

                                Dim elem_cod As Integer = infoOperazione.Elem_Cod

                                Dim udm_cod_dettaglio As Integer = If(dettaglioRilievo.unitaDiMisura IsNot Nothing, dettaglioRilievo.unitaDiMisura.codice, 0)

                                Dim qta As Decimal = 0

                                If agenda.Lav_Cod = LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE Then

                                    If Not IsNothing(dettaglioRilievo.risorsaProdotto) AndAlso Not IsNothing(dettaglioRilievo.risorsaProdotto.prodotto) Then
                                        elem_cod = dettaglioRilievo.risorsaProdotto.prodotto.elemCod
                                    End If

                                    udm_cod_dettaglio = enum_UnitaMisura.Numero_Trappole

                                    qta = 1
                                End If

                                Dim Movimento_Dettaglio_Rilievo As New Movimento_Dettaglio With {
                                        .Id_Agenda = agenda.Id_Agenda,
                                        .Piva = agenda.Piva,
                                        .Sa_Cod = agenda.Sa_Cod,
                                        .Data = agenda.Data,
                                        .Lav_Cod = agenda.Lav_Cod,
                                        .Cau_Mov = infoOperazione.Cau_Mov,
                                        .Elem_Cod = elem_cod,
                                        .Pro_Cod = pro_cod,
                                        .Contabilizzato = NONCONTABILE,
                                        .Pendente = 3,
                                        .Anno = 1900,
                                        .BaseCode = infoOperazione.BaseCode,
                                        .TopCode = infoOperazione.TopCode,
                                        .Mezzo_Det = -1,
                                        .Udm_Cod = udm_cod_dettaglio,
                                        .Qta = qta
                                    }

                                Movimento_Dettaglio_Rilievo.Movimenti_Destinazioni = New List(Of Movimento_Destinazione)

                                Movimento_Dettaglio_Rilievo.Movimenti_Dettagli_Tecnici = New List(Of Movimento_Dettaglio_Tecnico)
                                Dim Movimento_Dettaglio_Tecnico = CreaMovimentoDettaglioTecnico(attivita, agenda, dettaglioRilievo, infoOperazione, 0, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
                                If Movimento_Dettaglio_Tecnico IsNot Nothing Then
                                    Movimento_Dettaglio_Rilievo.Movimenti_Dettagli_Tecnici.Add(Movimento_Dettaglio_Tecnico)
                                End If

                                dictAvversita.Add(avKey, (Movimento_Dettaglio_Rilievo, 0))

                                Movimenti_Dettagli.Add(Movimento_Dettaglio_Rilievo)

                            End If

                            Dim dictValue = dictAvversita(avKey)

                            If Not rilievoSenzaImpianti AndAlso Not IsNothing(dettaglioRilievo.esercizioCDC) Then

                                dictValue.Item2 += dettaglioRilievo.esercizioCDC.superficieTrattata

                            End If

                            dictAvversita(avKey) = dictValue
                        End If
                    Next

                    For Each dettaglioRilievo In risorseRilievo

                        Dim av_cod As Integer = 0
                        Dim av_gru As Integer = 0
                        Dim udm_cod As Integer = 0
                        Dim pro_cod As Integer = 0

                        Select Case agenda.Lav_Cod
                            Case LAVCOD_RILIEVO_AVVERSITA_CAMPO, LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE
                                If dettaglioRilievo.avversitaGruppo IsNot Nothing Then
                                    STD_Utility.GetCodiciAvversita(dettaglioRilievo.avversitaGruppo, tipoFormulato:=0, av_cod, av_gru)
                                End If

                                If dettaglioRilievo.unitaDiMisura IsNot Nothing Then
                                    udm_cod = dettaglioRilievo.unitaDiMisura.codice
                                End If

                                If dettaglioRilievo.risorsaProdotto IsNot Nothing AndAlso dettaglioRilievo.risorsaProdotto.prodotto IsNot Nothing Then
                                    pro_cod = dettaglioRilievo.risorsaProdotto.prodotto.codice
                                End If

                            Case LAVCOD_RILIEVO_ERBE_INFESTANTI
                                If dettaglioRilievo.erbaInfestante IsNot Nothing Then
                                    STD_Utility.GetCodiciAvversita(dettaglioRilievo.erbaInfestante, tipoFormulato:=0, av_cod, av_gru)
                                End If
                        End Select

                        If av_cod <> 0 OrElse av_gru <> 0 Then
                            Dim avKey As String = av_cod & "-" & av_gru & "-" & udm_cod & "-" & pro_cod

                            Dim dictValue = dictAvversita(avKey)

                            Dim movimentoDettaglioCorrente As Movimento_Dettaglio = dictValue.Item1
                            movimentoDettaglioCorrente.Movimenti_Destinazioni.Add(CreaMovimentoDestinazioneRilievo(attivita, agenda, infoOperazione, dettaglioRilievo, dictValue.Item2))
                        End If

                    Next

                Case LAVCOD_DANNI_RACCOLTA, LAVCOD_RILIEVO_INDICI_MATURITA, LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA, LAVCOD_FASI_FENOLOGICHE

                    Dim dictFF As New Dictionary(Of String, (Movimento_Dettaglio, Decimal))

                    For Each dettaglioRilievo In risorseRilievo

                        Dim ffKey As String = ""
                        Select Case agenda.Lav_Cod
                            Case LAVCOD_DANNI_RACCOLTA
                                ffKey = dettaglioRilievo.DannoRaccolta

                            Case LAVCOD_RILIEVO_INDICI_MATURITA
                                ffKey = dettaglioRilievo.IndiceMaturita

                            Case LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA
                                ffKey = dettaglioRilievo.IndiceResa

                            Case LAVCOD_FASI_FENOLOGICHE
                                If Not IsNothing(dettaglioRilievo.faseFenologica) Then
                                    ffKey = dettaglioRilievo.faseFenologica.codice
                                End If
                        End Select

                        If ffKey <> "" Then

                            If Not dictFF.ContainsKey(ffKey) Then

                                Dim Movimento_Dettaglio_Rilievo As New Movimento_Dettaglio With {
                                    .Id_Agenda = agenda.Id_Agenda,
                                    .Piva = agenda.Piva,
                                    .Sa_Cod = agenda.Sa_Cod,
                                    .Data = agenda.Data,
                                    .Lav_Cod = agenda.Lav_Cod,
                                    .Cau_Mov = infoOperazione.Cau_Mov,
                                    .Elem_Cod = infoOperazione.Elem_Cod,
                                    .Contabilizzato = NONCONTABILE,
                                    .Pendente = 3,
                                    .Anno = 1900,
                                    .BaseCode = infoOperazione.BaseCode,
                                    .TopCode = infoOperazione.TopCode,
                                    .Mezzo_Det = -1,
                                    .Udm_Cod = If(dettaglioRilievo.unitaDiMisura IsNot Nothing, dettaglioRilievo.unitaDiMisura.codice, 0)
                                }

                                Movimento_Dettaglio_Rilievo.Movimenti_Destinazioni = New List(Of Movimento_Destinazione)

                                Movimento_Dettaglio_Rilievo.Movimenti_Dettagli_Tecnici = New List(Of Movimento_Dettaglio_Tecnico)
                                Dim Movimento_Dettaglio_Tecnico = CreaMovimentoDettaglioTecnico(attivita, agenda, dettaglioRilievo, infoOperazione, 0, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
                                If Movimento_Dettaglio_Tecnico IsNot Nothing Then
                                    Movimento_Dettaglio_Rilievo.Movimenti_Dettagli_Tecnici.Add(Movimento_Dettaglio_Tecnico)
                                End If

                                dictFF.Add(ffKey, (Movimento_Dettaglio_Rilievo, 0))

                                Movimenti_Dettagli.Add(Movimento_Dettaglio_Rilievo)
                            End If

                            Dim dictValue = dictFF(ffKey)

                            If Not rilievoSenzaImpianti AndAlso Not IsNothing(dettaglioRilievo.esercizioCDC) Then

                                dictValue.Item2 += dettaglioRilievo.esercizioCDC.superficieTrattata

                            End If

                            dictFF(ffKey) = dictValue

                        End If
                    Next

                    For Each dettaglioRilievo In risorseRilievo

                        Dim ffKey As String = ""
                        Select Case agenda.Lav_Cod
                            Case LAVCOD_DANNI_RACCOLTA
                                ffKey = dettaglioRilievo.DannoRaccolta

                            Case LAVCOD_RILIEVO_INDICI_MATURITA
                                ffKey = dettaglioRilievo.IndiceMaturita

                            Case LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA
                                ffKey = dettaglioRilievo.IndiceResa

                            Case LAVCOD_FASI_FENOLOGICHE
                                If Not IsNothing(dettaglioRilievo.faseFenologica) Then
                                    ffKey = dettaglioRilievo.faseFenologica.codice
                                End If
                        End Select

                        If ffKey <> "" Then

                            Dim dictValue = dictFF(ffKey)

                            Dim movimentoDettaglioCorrente As Movimento_Dettaglio = dictValue.Item1
                            movimentoDettaglioCorrente.Movimenti_Destinazioni.Add(CreaMovimentoDestinazioneRilievo(attivita, agenda, infoOperazione, dettaglioRilievo, dictValue.Item2))

                        End If

                    Next

            End Select

        End If

        Return Movimenti_Dettagli

    End Function


    Private Sub updateQtaLottoMovimentoDettaglio(ByRef movimentoDettaglio As Movimento_Dettaglio, qtaTotale As Decimal, doseHl As Decimal, doseHa As Decimal, superficieTrattataTotale As Decimal, acquaTotale As Decimal, lotto As String, infoOperazione As InfoOperazione, quantitaSuImpianto As QuantitaSuImpianto)

        ' si concatenano i lotti dei vari movimenti di magazzino per trattamenti e fertilizzazioni, possono essere n distinti ma c'è solo una riga di prodotto
        If infoOperazione.IsTrattamento OrElse infoOperazione.IsFertilizzazione Then
            Dim lottoToInsert = lotto.Trim
            If lottoToInsert <> "" Then
                Dim found As Boolean = False
                Dim lottoArray As String() = movimentoDettaglio.Lotto.ToUpper.Split(" ")
                If lottoArray IsNot Nothing AndAlso lottoArray.Length > 0 Then
                    If lottoArray.Contains(lottoToInsert.ToUpper) Then
                        found = True
                    End If
                End If
                If Not found Then
                    If Not String.IsNullOrEmpty(movimentoDettaglio.Lotto) Then
                        movimentoDettaglio.Lotto &= " "
                    End If
                    movimentoDettaglio.Lotto &= lottoToInsert
                End If
            End If
        End If

        'DT: default flagDoseQuantitaTotale: Dose/Ha
        If movimentoDettaglio.Udm_Cod_Extra = 0 Then
            movimentoDettaglio.Udm_Cod_Extra = enum_TipoModalitaDistribuzione.Dose
            movimentoDettaglio.Mezzo_Det = enum_TipoMezzo.Ettaro
        End If

        Select Case movimentoDettaglio.Udm_Cod_Extra 'flagDoseQuantitaTotale 

            Case enum_TipoModalitaDistribuzione.Totale

                'sommo la qta totale
                movimentoDettaglio.Qta_Extra_Totale += qtaTotale

                'calcolo la qta/hl
                movimentoDettaglio.Qta_Extra = If(acquaTotale > 0, movimentoDettaglio.Qta_Extra_Totale / acquaTotale, 0)

                'calcolo la qta/ha
                If infoOperazione.IsSemina OrElse infoOperazione.IsRaccolta OrElse infoOperazione.Elem_Cod = TRAPPOLE Then
                    movimentoDettaglio.Qta = movimentoDettaglio.Qta_Extra_Totale
                Else
                    movimentoDettaglio.Qta = movimentoDettaglio.Qta_Extra_Totale / superficieTrattataTotale
                End If

            Case enum_TipoModalitaDistribuzione.Dose

                Select Case movimentoDettaglio.Mezzo_Det 'flagTipoDose

                    Case enum_TipoMezzo.Ettolitro

                        'sommo la qta/hl
                        movimentoDettaglio.Qta_Extra += doseHl

                        'calcolo la qta totale
                        movimentoDettaglio.Qta_Extra_Totale = movimentoDettaglio.Qta_Extra * acquaTotale

                        'sommo la qta/ha
                        If infoOperazione.IsSemina OrElse infoOperazione.IsRaccolta Then
                            movimentoDettaglio.Qta = movimentoDettaglio.Qta_Extra_Totale
                        Else
                            movimentoDettaglio.Qta = movimentoDettaglio.Qta_Extra_Totale / superficieTrattataTotale
                        End If

                    Case enum_TipoMezzo.Ettaro

                        'sommo la qta/ha
                        If infoOperazione.IsSemina OrElse infoOperazione.IsRaccolta Then
                            movimentoDettaglio.Qta += qtaTotale
                        Else
                            movimentoDettaglio.Qta += doseHa
                        End If

                        'calcolo la qta totale
                        If infoOperazione.IsSemina OrElse infoOperazione.IsRaccolta Then
                            movimentoDettaglio.Qta_Extra_Totale = movimentoDettaglio.Qta
                        Else
                            movimentoDettaglio.Qta_Extra_Totale = movimentoDettaglio.Qta * superficieTrattataTotale
                        End If

                        'calcolo la qta/hl
                        movimentoDettaglio.Qta_Extra = If(acquaTotale > 0, movimentoDettaglio.Qta_Extra_Totale / acquaTotale, 0)

                End Select


        End Select

        If infoOperazione.IsTrattamento AndAlso Not IsNothing(quantitaSuImpianto) AndAlso Not IsNothing(quantitaSuImpianto.esercizioCDC) Then

            If Not IsNothing(movimentoDettaglio.Movimenti_Dettagli_Tecnici) AndAlso movimentoDettaglio.Movimenti_Dettagli_Tecnici.Count = 1 Then

                movimentoDettaglio.Movimenti_Dettagli_Tecnici(0).Dose += quantitaSuImpianto.Qta

            End If

        End If

    End Sub

    Private Sub checkCompatibilita(agenda As Operazione_Agenda, attivita As Attivita, risorsaProdotto As risorse.RisorsaProdotto, movimentoDettaglio As Movimento_Dettaglio, InfoOperazione As InfoOperazione, objParametri_Super_Server As AgronicaCoreParametri, objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri)

        'DT: controllo l'uniformità dei campi relativi alla lavorazione che devono necessariamente essere uguali

        'UDM
        If risorsaProdotto.unitaDiMisuraIndicata.codice <> movimentoDettaglio.Extra_Int Then
            Throw New GiasException(String.Format(My.Resources.AgronicaCoreMapper.SelezionareUnitaDiMisuraUniche, risorsaProdotto.prodotto.descrizione))
        End If
        Dim Movimento_Dettaglio_Tecnico_Current = CreaMovimentoDettaglioTecnico(attivita, agenda, risorsaProdotto, InfoOperazione, 0, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
        If InfoOperazione.IsTrattamento Then
            Dim trattamento As dettagli.DettaglioTrattamento = CType(risorsaProdotto, dettagli.DettaglioTrattamento)

            If trattamento IsNot Nothing Then

                'AVVERSITA e SOGLIA
                If trattamento.avversitaGruppo IsNot Nothing Then
                    If Movimento_Dettaglio_Tecnico_Current IsNot Nothing AndAlso movimentoDettaglio.Movimenti_Dettagli_Tecnici IsNot Nothing AndAlso movimentoDettaglio.Movimenti_Dettagli_Tecnici.Count > 0 Then
                        Dim Movimento_Dettaglio_Tecnico = movimentoDettaglio.Movimenti_Dettagli_Tecnici(0)

                        If Movimento_Dettaglio_Tecnico_Current.Av_Cod <> Movimento_Dettaglio_Tecnico.Av_Cod OrElse
                            Movimento_Dettaglio_Tecnico_Current.Av_Gru <> Movimento_Dettaglio_Tecnico.Av_Gru Then

                            Throw New GiasException(String.Format(My.Resources.AgronicaCoreMapper.SelezionareAvversitaUniche, risorsaProdotto.prodotto.descrizione))

                        End If

                        If Movimento_Dettaglio_Tecnico_Current.Soglia_Cod <> Movimento_Dettaglio_Tecnico.Soglia_Cod OrElse
                            Movimento_Dettaglio_Tecnico_Current.Soglia_Quantita <> Movimento_Dettaglio_Tecnico.Soglia_Quantita Then

                            Throw New GiasException(String.Format(My.Resources.AgronicaCoreMapper.SelezionareSoglieUniche, risorsaProdotto.prodotto.descrizione))

                        End If

                    End If
                End If

                'DOSI ETICHETTA
                If trattamento.dosiEtichetta IsNot Nothing AndAlso trattamento.dosiEtichetta.Count > 0 Then

                    Dim arrDosiInserite As String() = If(String.IsNullOrEmpty(movimentoDettaglio.DoseEtichetta_Value), New String() {}, Split(movimentoDettaglio.DoseEtichetta_Value, "<br>"))
                    Dim dosiInserite As New List(Of DoseEtichetta)
                    For Each itemDose In arrDosiInserite
                        Dim arrItemDose = Split(itemDose, "$")
                        dosiInserite.Add(New DoseEtichetta(arrItemDose(0)))
                    Next

                    If Not checkDosiEtichettaUguali(trattamento.dosiEtichetta, dosiInserite) Then
                        Throw New GiasException(String.Format(My.Resources.AgronicaCoreMapper.SelezionareDosiEtichettaUniche, risorsaProdotto.prodotto.descrizione))
                    End If

                End If
            End If
        End If

        If InfoOperazione.IsFertilizzazione Then

            'N P K M CU + Efficienza
            If Movimento_Dettaglio_Tecnico_Current IsNot Nothing AndAlso movimentoDettaglio.Movimenti_Dettagli_Tecnici IsNot Nothing AndAlso movimentoDettaglio.Movimenti_Dettagli_Tecnici.Count > 0 Then
                Dim Movimento_Dettaglio_Tecnico = movimentoDettaglio.Movimenti_Dettagli_Tecnici(0)

                If Movimento_Dettaglio_Tecnico_Current.N <> Movimento_Dettaglio_Tecnico.N OrElse
                    Movimento_Dettaglio_Tecnico_Current.P <> Movimento_Dettaglio_Tecnico.P OrElse
                    Movimento_Dettaglio_Tecnico_Current.K <> Movimento_Dettaglio_Tecnico.K OrElse
                    Movimento_Dettaglio_Tecnico_Current.M <> Movimento_Dettaglio_Tecnico.M OrElse
                    Movimento_Dettaglio_Tecnico_Current.Cu <> Movimento_Dettaglio_Tecnico.Cu OrElse
                    Movimento_Dettaglio_Tecnico_Current.Efficienza <> Movimento_Dettaglio_Tecnico.Efficienza Then

                    Throw New GiasException(String.Format(My.Resources.AgronicaCoreMapper.SelezionareTitoliUnici, risorsaProdotto.prodotto.descrizione))

                End If

            End If

        End If

    End Sub

    Private Function CreaMovimentoDettaglio(risorsaProdotto As risorse.RisorsaProdotto, movimentoMagazzino As RilevamentoDiMagazzino, attivita As Attivita, agenda As Operazione_Agenda, infoOperazione As InfoOperazione, SuperficieTrattataTotale As Decimal, ByVal objParametri_Super_Server As AgronicaCoreParametri, ByVal objParametri_Server As AgronicaCoreParametri, ByVal objParametri_Utenti As AgronicaCoreParametri) As Movimento_Dettaglio
        Dim Movimento_Dettaglio_Prodotto As New Movimento_Dettaglio With {
            .Id_Agenda = agenda.Id_Agenda,
            .Piva = agenda.Piva,
            .Sa_Cod = agenda.Sa_Cod,
            .Data = agenda.Data,
            .Lav_Cod = agenda.Lav_Cod,
            .Cau_Mov = infoOperazione.Cau_Mov,
            .Elem_Cod = infoOperazione.Elem_Cod,
            .Lotto = "",
            .Contabilizzato = NONCONTABILE,
            .BaseCode = infoOperazione.BaseCode,
            .TopCode = infoOperazione.TopCode,
            .Extra_Int = risorsaProdotto.unitaDiMisuraIndicata.codice,
            .Mezzo_Det = risorsaProdotto.flagTipoDose,
            .Udm_Cod_Extra = risorsaProdotto.flagDoseQuantitaTotale,
            .Qta = 0,
            .Qta_Extra = 0,
            .Qta_Extra_Totale = 0
        }

        'DT: se risorsaProdotto.unitaDiMisura non è valorizzata, la si ricava da quella indicata
        If risorsaProdotto.unitaDiMisura Is Nothing OrElse risorsaProdotto.unitaDiMisura.codice = 0 Then
            Dim qta As Integer = 0
            Dim udmBase = STD_Utility.getUdmBasefromUdmIndicata(risorsaProdotto.unitaDiMisuraIndicata, qta, objParametri_Server)
            Movimento_Dettaglio_Prodotto.Udm_Cod = udmBase.codice
        Else
            Movimento_Dettaglio_Prodotto.Udm_Cod = risorsaProdotto.unitaDiMisura.codice
        End If

        Dim tipoOperazione As enum_Tipo_Operazione_Agenda = STD_Utility.getTipoOperazione(attivita.tipo, attivita.stato)

        If infoOperazione.IsTrattamento Then
            Dim trattamento As dettagli.DettaglioTrattamento = CType(risorsaProdotto, dettagli.DettaglioTrattamento)

            Dim doseText As String = ""
            Dim doseValue As String = ""
            STD_Utility.Stringhe_from_DosiEtichetta(trattamento.dosiEtichetta, doseText, doseValue, objParametri_Server)

            With Movimento_Dettaglio_Prodotto
                .Pro_Cod = If(tipoOperazione = enum_Tipo_Operazione_Agenda.RicettaBrogliaccio, risorsaProdotto.prodotto.codice, Math.Abs(risorsaProdotto.prodotto.codice)) 'su brogliaccio da Demetra possono arrivare codici negativi (-1) per segnalare "nessun prodotto"
                .Mat_Cod = 0
                .TempoCarenza = trattamento.tempoCarenza
                .Extra_Str = trattamento.dettaglioProdotto
                .DoseEtichetta = doseText
                .DoseEtichetta_Value = doseValue
                .PrincipiAttivi = GetPrincipiAttiviTitoli(trattamento.principiAttivi)
                .PrincipiAttiviPesi = GetPrincipiAttiviPesi(trattamento.principiAttivi)
                .PrincipiAttiviPercAbb = GetPrincipiAttiviPercentualeSuperficieTrattabile(trattamento.principiAttivi)
                .CLassiTossicologiche = "" 'DT: non più gestito
                .Buffer = STD_Utility.BuildBufferStr(trattamento.bufferzone)
                .Polverulento = trattamento.polverulento
            End With

        End If

        If infoOperazione.IsFertilizzazione Then
            With Movimento_Dettaglio_Prodotto
                .Pro_Cod = If(tipoOperazione = enum_Tipo_Operazione_Agenda.RicettaBrogliaccio, risorsaProdotto.prodotto.codice, Math.Abs(risorsaProdotto.prodotto.codice)) 'su brogliaccio da Demetra possono arrivare codici negativi (-1) per segnalare "nessun prodotto"
                .Mat_Cod = 0
            End With
        End If

        If infoOperazione.IsSemina Then

            With Movimento_Dettaglio_Prodotto
                .Pro_Cod = 0
                .Mat_Cod = Math.Abs(risorsaProdotto.prodotto.codice)
                .Mov_Det_Des = My.Resources.AgronicaCoreMapper.DettagliTecniciMateriaPrimaSeminaTrapianto
                .Pendente = 2
                .Anno = 1900
                .Lotto = If(movimentoMagazzino IsNot Nothing, movimentoMagazzino.Lotto, "")
                .Extra_Str = GetExtraStr(risorsaProdotto, movimentoMagazzino, infoOperazione)
            End With

        End If

        If infoOperazione.IsIrrigazione OrElse infoOperazione.Elem_Cod = TRAPPOLE OrElse
            attivita.job.primaryKey.codice = LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA OrElse attivita.job.primaryKey.codice = LAVCOD_REINNESCO_TRAPPOLE Then
            With Movimento_Dettaglio_Prodotto
                .Pendente = 3
                .Anno = 1900
            End With
        End If

        If infoOperazione.IsRaccolta Then
            Dim dettaglioProdotto = CType(risorsaProdotto, DettaglioRaccolta)

            With Movimento_Dettaglio_Prodotto
                .Pro_Cod = 0
                .Mat_Cod = Math.Abs(risorsaProdotto.prodotto.codice)
                .Mov_Det_Des = My.Resources.AgronicaCoreMapper.DettagliProdottiAziendaliRaccolta
                .Pendente = 3
                .Anno = 1900
                .Lotto = If(IsNothing(movimentoMagazzino), "", movimentoMagazzino.Lotto)
                '-- Cod_Prodotto valorizzato solo per carichi di magazzino con lotto da esercizio
                .Cod_Progetto = 0 'If(IsNothing(movimentoMagazzino), 0, movimentoMagazzino.Cod_Progetto)
                .Udm_Cod_Extra = enum_TipoModalitaDistribuzione.Totale
                .Sa_Cod = dettaglioProdotto.QuantitaSuImpianti.First().
                                            esercizioCDC.esercizio.impiantoPK.
                                            appezzamentoPK.centroAziendalePK.codice
            End With

            If Not IsNothing(dettaglioProdotto.calCod) Then
                ' -- 06/06/2023 Si è deciso di eliminare la gestione dei Cal_Cod per la raccolta
                'Movimento_Dettaglio_Prodotto.Cal_Cod = dettaglioProdotto.calCod
                Movimento_Dettaglio_Prodotto.Cal_Cod = 0

            Else 'vedremo che fare
                Dim er = 0
            End If
        End If

        '----- MOVIMENTO DETTAGLIO TECNICO (si prendono i dati della prima risorsa relativa al prodotto corrente)
        Movimento_Dettaglio_Prodotto.Movimenti_Dettagli_Tecnici = New List(Of Movimento_Dettaglio_Tecnico)

        Dim Lav_Cod As Integer = CInt(attivita.job.primaryKey.codice)

        If infoOperazione.IsTrattamento AndAlso
            (Lav_Cod = LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA OrElse Lav_Cod = LAVCOD_REINNESCO_TRAPPOLE) Then

            Dim dettaglioTrattamento = CType(risorsaProdotto, dettagli.DettaglioTrattamento)

            If Not IsNothing(dettaglioTrattamento.quantitaSuImpianti) AndAlso dettaglioTrattamento.quantitaSuImpianti.Count > 0 Then
                Dim Movimento_Dettaglio_Tecnico = CreaMovimentoDettaglioTecnico(attivita, agenda, risorsaProdotto, infoOperazione, dettaglioTrattamento.quantitaSuImpianti.First.Qta, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
                If Movimento_Dettaglio_Tecnico IsNot Nothing Then
                    Movimento_Dettaglio_Prodotto.Movimenti_Dettagli_Tecnici.Add(Movimento_Dettaglio_Tecnico)
                End If
            End If
        Else
            Dim Movimento_Dettaglio_Tecnico = CreaMovimentoDettaglioTecnico(attivita, agenda, risorsaProdotto, infoOperazione, 0, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
            If Movimento_Dettaglio_Tecnico IsNot Nothing Then
                Movimento_Dettaglio_Prodotto.Movimenti_Dettagli_Tecnici.Add(Movimento_Dettaglio_Tecnico)
            End If
        End If

        Return Movimento_Dettaglio_Prodotto

    End Function

    Private Function CreaMovimentiDettaglioScarico(attivita As Attivita, agenda As Operazione_Agenda, infoOperazione As InfoOperazione, objParametri_Server As AgronicaCoreParametri) As List(Of Movimento_Dettaglio)

        Dim Movimenti_Dettagli_Scarico As New List(Of Movimento_Dettaglio)

        'DT: chiave: prodotto,elem_cod,lotto,piva,sa_cod,fabbricato_cod
        Dim dictScarico As New Dictionary(Of (Integer, Integer, String, String, Integer, Integer), (movimentoDettaglio As Movimento_Dettaglio, movimentoDestinazione As Movimento_Destinazione))
        Dim risorseProdotto As List(Of risorse.RisorsaProdotto) = attivita.risorse.FindAll(Function(c) (c.classType = ClassType.DettaglioTrattamento OrElse c.classType = ClassType.DettaglioFertilizzazione OrElse c.classType = ClassType.DettaglioSemina)).ConvertAll(Function(obj1) CType(obj1, risorse.RisorsaProdotto))

        For Each risorsaProdotto In risorseProdotto

            If agenda.Lav_Cod = LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA OrElse
               agenda.Lav_Cod = LAVCOD_REINNESCO_TRAPPOLE Then
                Dim dettaglioTrattamento As dettagli.DettaglioTrattamento = CType(risorsaProdotto, dettagli.DettaglioTrattamento)

                If Not IsNothing(dettaglioTrattamento) AndAlso Not IsNothing(dettaglioTrattamento.avversitaGruppo) AndAlso
                    Not IsNothing(dettaglioTrattamento.avversitaGruppo.MagazziniMovimentazioni) AndAlso dettaglioTrattamento.avversitaGruppo.MagazziniMovimentazioni.Count = 1 Then

                    Dim risorsaProdottoInnesco As New RisorsaProdotto() With {
                        .prodotto = dettaglioTrattamento.avversitaGruppo.MagazziniMovimentazioni(0).Prodotto,
                        .unitaDiMisura = dettaglioTrattamento.avversitaGruppo.MagazziniMovimentazioni(0).udm,
                        .unitaDiMisuraIndicata = dettaglioTrattamento.avversitaGruppo.MagazziniMovimentazioni(0).udm
                    }

                    SetDictionaryScarico(dictScarico, dettaglioTrattamento.avversitaGruppo.MagazziniMovimentazioni,
                                            attivita, risorsaProdottoInnesco, agenda, infoOperazione)

                End If
            End If

            SetDictionaryScarico(dictScarico, risorsaProdotto.MagazziniMovimentazioni,
                                            attivita, risorsaProdotto, agenda, infoOperazione)

        Next

        For Each itemDistinctScarico In dictScarico
            Dim movimentoDettaglio = itemDistinctScarico.Value.movimentoDettaglio
            Movimenti_Dettagli_Scarico.Add(movimentoDettaglio)

            '----- MOVIMENTI DESTINAZIONI

            movimentoDettaglio.Movimenti_Destinazioni = New List(Of Movimento_Destinazione)

            With itemDistinctScarico.Value.movimentoDestinazione
                .Data = agenda.Data
                .Id_Agenda = agenda.Id_Agenda
                .Piva = movimentoDettaglio.Piva
                .Sa_Cod = movimentoDettaglio.Sa_Cod
                .Appezza = 0
                .Id_Destinazione = itemDistinctScarico.Key.Item6
                .Tipo = MAGAZZINO
                .Qta = movimentoDettaglio.Qta
                .BaseCode = infoOperazione.BaseCode
                .TopCode = infoOperazione.TopCode
            End With

            movimentoDettaglio.Movimenti_Destinazioni.Add(itemDistinctScarico.Value.movimentoDestinazione)
        Next

        Return Movimenti_Dettagli_Scarico

    End Function

    Private Function CreaMovimentiDettaglioCarico(attivita As Attivita, agenda As Operazione_Agenda, infoOperazione As InfoOperazione, ByRef dataIngresso As DateTime, ByRef oraIngresso As DateTime) As List(Of Movimento_Dettaglio)

        Dim Movimenti_Dettagli_Carico As New List(Of Movimento_Dettaglio)

        Dim dictCarico As New Dictionary(Of (Integer, String, String, Integer, Integer, Integer, Integer), Movimento_Dettaglio)
        Dim risorseProdotto As List(Of risorse.RisorsaProdotto) = attivita.risorse.FindAll(Function(c) (c.classType = ClassType.DettaglioRaccolta)).ConvertAll(Function(obj1) CType(obj1, risorse.RisorsaProdotto))

        For Each risorsaProdotto In risorseProdotto
            If risorsaProdotto.MagazziniMovimentazioni IsNot Nothing Then
                For Each movimentoMagazzino In risorsaProdotto.MagazziniMovimentazioni
                    If movimentoMagazzino.Prodotto IsNot Nothing AndAlso
                        movimentoMagazzino.Magazzino IsNot Nothing AndAlso
                        movimentoMagazzino.Magazzino.primaryKey IsNot Nothing AndAlso
                        movimentoMagazzino.Magazzino.primaryKey.centroAziendalePK IsNot Nothing Then

                        Dim codiceProdotto As Integer = movimentoMagazzino.Prodotto.codice
                        Dim lotto As String = movimentoMagazzino.Lotto
                        Dim pIva As String = movimentoMagazzino.Magazzino.primaryKey.centroAziendalePK.partitaIva
                        Dim sa_cod_magazzino As Integer = movimentoMagazzino.Magazzino.primaryKey.centroAziendalePK.codice
                        Dim sa_cod As Integer = agenda.Sa_Cod
                        Dim fabbricato_cod As Integer = movimentoMagazzino.Magazzino.primaryKey.codice
                        Dim cod_progetto = 0

                        'Se ho una raccolta multicentro
                        If infoOperazione.IsRaccolta AndAlso attivita.centriDiCosto.Select(Function(c) CType(c, EsercizioCDC).esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice).Distinct().Count() > 1 Then
                            ' imposto il sa_cod come quello dell'impianto di riferimento
                            sa_cod = CType(risorsaProdotto, DettaglioRaccolta).QuantitaSuImpianti.First().esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice
                        End If

                        If pIva <> "" AndAlso sa_cod_magazzino <> 0 AndAlso fabbricato_cod <> 0 Then

                            If Not dictCarico.ContainsKey((codiceProdotto, lotto.ToUpper, pIva, sa_cod, sa_cod_magazzino, fabbricato_cod, cod_progetto)) Then

                                Dim Movimento_Dettaglio_Carico As New Movimento_Dettaglio With {
                                    .Id_Agenda = agenda.Id_Agenda,
                                    .Piva = pIva,
                                    .Sa_Cod = movimentoMagazzino.Magazzino.primaryKey.centroAziendalePK.codice,
                                    .Data = agenda.Data,
                                    .Lav_Cod = agenda.Lav_Cod,
                                    .Cau_Mov = CAU_CARICO,
                                    .Elem_Cod = infoOperazione.Elem_Cod,
                                    .Pro_Cod = codiceProdotto,
                                    .Mat_Cod = 0,
                                    .Lotto = lotto,
                                    .Extra_Int = 0,
                                    .Contabilizzato = NONCONTABILE,
                                    .BaseCode = infoOperazione.BaseCode,
                                    .TopCode = infoOperazione.TopCode,
                                    .Udm_Cod = risorsaProdotto.unitaDiMisura.codice, 'TODO_DT: si può assumere che le udm siano omogenee? TESTARE BENE con udm diverse
                                    .Qta = 0, 'DT: inizializzazione in vista di somma a parità di chiave
                                    .Cal_Cod = 0 'TODO_DT: gestire (almeno in raccolta)
                                }

                                If infoOperazione.IsSemina Then
                                    With Movimento_Dettaglio_Carico
                                        .Pro_Cod = 0
                                        .Mat_Cod = Math.Abs(codiceProdotto) 'DT: inversione di segno perchè nel modello i matcod arrivano negativi
                                    End With
                                End If

                                If infoOperazione.IsRaccolta Then
                                    With Movimento_Dettaglio_Carico
                                        .Pro_Cod = 0
                                        .Mat_Cod = Math.Abs(codiceProdotto) 'DT: inversione di segno perché nel modello i matcod arrivano negativi
                                        .Mov_Det_Des = My.Resources.AgronicaCoreMapper.CaricoRaccolta
                                    End With

                                    Dim dettaglioProdotto = CType(risorsaProdotto, DettaglioRaccolta)
                                    If dettaglioProdotto.Opzioni_Raccolta.GenerazioneLotto = enum_Generazione_Lotto_Raccolta.DA_ESERCIZO Then
                                        cod_progetto = dettaglioProdotto.QuantitaSuImpianti.FirstOrDefault().esercizioCDC.esercizio.codice
                                        Movimento_Dettaglio_Carico.Cod_Progetto = cod_progetto
                                    End If

                                    If Not IsNothing(dettaglioProdotto.calCod) Then
                                        ' -- 06/06/2023 Si è deciso di eliminare la gestione dei Cal_Cod per la raccolta
                                        'Movimento_Dettaglio_Carico.Cal_Cod = dettaglioProdotto.calCod
                                        Movimento_Dettaglio_Carico.Cal_Cod = 0
                                    End If
                                    dataIngresso = dettaglioProdotto.dataIngresso.Date
                                    oraIngresso = New DateTime(AGRODATAFINE.Year, AGRODATAFINE.Month, AGRODATAFINE.Day, dettaglioProdotto.dataIngresso.Hour, dettaglioProdotto.dataIngresso.Minute, 0)

                                End If

                                dictCarico.Add((codiceProdotto, lotto.ToUpper, pIva, sa_cod, sa_cod_magazzino, fabbricato_cod, cod_progetto), Movimento_Dettaglio_Carico)

                            End If

                            Dim Dose_Trasformata As Decimal = GetDoseTrasformata(movimentoMagazzino.Qta, movimentoMagazzino.udm.codice)
                            dictCarico((codiceProdotto, lotto.ToUpper, pIva, sa_cod, sa_cod_magazzino, fabbricato_cod, cod_progetto)).Qta += movimentoMagazzino.Qta

                        End If
                    End If
                Next
            End If
        Next

        For Each itemDistinctCarico In dictCarico
            Dim movimentoDettaglio = itemDistinctCarico.Value
            Movimenti_Dettagli_Carico.Add(movimentoDettaglio)

            '----- MOVIMENTI DESTINAZIONI

            movimentoDettaglio.Movimenti_Destinazioni = New List(Of Movimento_Destinazione)

            Dim Movimento_Destinazione_Carico As New Movimento_Destinazione With {
                .Data = agenda.Data,
                .Id_Agenda = agenda.Id_Agenda,
                .Piva = movimentoDettaglio.Piva,
                .Sa_Cod = movimentoDettaglio.Sa_Cod,
                .Appezza = 0,
                .Id_Destinazione = itemDistinctCarico.Key.Item6,
                .Tipo = MAGAZZINO,
                .Qta = movimentoDettaglio.Qta,
                .BaseCode = infoOperazione.BaseCode,
                .TopCode = infoOperazione.TopCode
            }

            If infoOperazione.IsRaccolta Then
                Movimento_Destinazione_Carico.Data = itemDistinctCarico.Value.Data
            End If

            movimentoDettaglio.Movimenti_Destinazioni.Add(Movimento_Destinazione_Carico)
        Next

        Return Movimenti_Dettagli_Carico

    End Function

    Private Function CreaMovimentoDestinazioneIrrigazione(attivita As Attivita, agenda As Operazione_Agenda, infoOperazione As InfoOperazione, dettaglioIrrigazione As dettagli.DettaglioIrrigazione, SuperficieTrattataTotale As Decimal) As List(Of Movimento_Destinazione)

        Dim Movimenti_Destinazioni As New List(Of Movimento_Destinazione)

        If dettaglioIrrigazione IsNot Nothing Then

            Dim Movimento_Destinazione As New Movimento_Destinazione With {
                .Id_Agenda = agenda.Id_Agenda,
                .Data = agenda.Data,
                .Piva = dettaglioIrrigazione.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                .Sa_Cod = dettaglioIrrigazione.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                .Appezza = dettaglioIrrigazione.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.codice,
                .Id_Destinazione = dettaglioIrrigazione.esercizioCDC.esercizio.impiantoPK.codice,
                .Tipo = 0,
                .Qta = dettaglioIrrigazione.QtaTotale,
                .Qta2 = dettaglioIrrigazione.esercizioCDC.superficieTrattata,
                .BaseCode = infoOperazione.BaseCode,
                .TopCode = infoOperazione.TopCode,
                .Programmazione_Entita_Cod = 0, 'DT: valorizzato solo in caso di salvataggio planning
                .QuotaDistribuzione = If(SuperficieTrattataTotale <> 0, dettaglioIrrigazione.esercizioCDC.superficieTrattata / SuperficieTrattataTotale, 0)
            }

            Movimenti_Destinazioni.Add(Movimento_Destinazione)

        End If

        Return Movimenti_Destinazioni

    End Function

    Private Function CreaMovimentoDestinazioneRilievo(attivita As Attivita, agenda As Operazione_Agenda, infoOperazione As InfoOperazione, dettaglioRilievo As dettagli.DettaglioRilievo, SuperficieTrattataTotale As Decimal) As Movimento_Destinazione

        If dettaglioRilievo IsNot Nothing Then

            Dim Movimento_Destinazione As New Movimento_Destinazione With {
                .Id_Agenda = agenda.Id_Agenda,
                .Piva = attivita.centroAziendale.primaryKey.partitaIva,
                .Sa_Cod = attivita.centroAziendale.primaryKey.codice,
                .Appezza = 0,
                .Id_Destinazione = 0,
                .Tipo = 0,
                .Qta = dettaglioRilievo.QtaRilevata,
                .Qta2 = 0,
                .BaseCode = infoOperazione.BaseCode,
                .TopCode = infoOperazione.TopCode,
                .Programmazione_Entita_Cod = 0, 'TODO_DT: capire come fare in modifica
                .QuotaDistribuzione = 0
            }

            'Completo le chiavi degli impianti se il rilievo è stato registrato con selezionandoli
            If Not RilievoSenzaImpianti(attivita) AndAlso Not IsNothing(dettaglioRilievo.esercizioCDC) Then

                Movimento_Destinazione.Piva = dettaglioRilievo.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva

                Movimento_Destinazione.Sa_Cod = dettaglioRilievo.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice

                Movimento_Destinazione.Appezza = dettaglioRilievo.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.codice

                Movimento_Destinazione.Id_Destinazione = dettaglioRilievo.esercizioCDC.esercizio.impiantoPK.codice

                Movimento_Destinazione.Qta2 = dettaglioRilievo.esercizioCDC.superficieTrattata

                Movimento_Destinazione.QuotaDistribuzione = If(SuperficieTrattataTotale <> 0, dettaglioRilievo.esercizioCDC.superficieTrattata / SuperficieTrattataTotale, 0)
            End If

            Movimento_Destinazione.Data = attivita.inizio

            Select Case agenda.Lav_Cod
                Case LAVCOD_FASI_FENOLOGICHE
                    Movimento_Destinazione.Data = dettaglioRilievo.DataOraRilievo
                Case LAVCOD_RILIEVO_INDICI_MATURITA
                    If Not IsNothing(dettaglioRilievo.DataOraRilievo) AndAlso dettaglioRilievo.DataOraRilievo <> AGRODATAINIZIO AndAlso dettaglioRilievo.DataOraRilievo <> AGRODATAFINE Then
                        Movimento_Destinazione.Data = dettaglioRilievo.DataOraRilievo
                    End If

                    Movimento_Destinazione.Extra_Str = dettaglioRilievo.Note
            End Select

            Return Movimento_Destinazione

        End If

        Return Nothing

    End Function

    Private Function CreaMovimentiDestinazione(agenda As Operazione_Agenda, infoOperazione As InfoOperazione, centriDiCosto As List(Of centri_di_costo.CentroDiCosto), Dose As Decimal, SuperficieTrattataTotale As Decimal) As List(Of Movimento_Destinazione)

        Dim Movimenti_Destinazioni As List(Of Movimento_Destinazione) = Nothing

        If Not IsNothing(centriDiCosto) Then

            Movimenti_Destinazioni = New List(Of Movimento_Destinazione)

            For Each centroDiCosto In centriDiCosto
                If (centroDiCosto.classType.Equals(costanti.ClassType.EsercizioCDC)) Then

                    Dim esercizioCDC As centri_di_costo.EsercizioCDC = CType(centroDiCosto, centri_di_costo.EsercizioCDC)

                    Dim Movimento_Destinazione As New Movimento_Destinazione With {
                        .Id_Agenda = agenda.Id_Agenda,
                        .Data = agenda.Data,
                        .Piva = esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                        .Sa_Cod = esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                        .Appezza = esercizioCDC.esercizio.impiantoPK.appezzamentoPK.codice,
                        .Id_Destinazione = esercizioCDC.esercizio.impiantoPK.codice,
                        .Tipo = 0,
                        .Qta = Dose * esercizioCDC.superficieTrattata,
                        .Qta2 = esercizioCDC.superficieTrattata,
                        .BaseCode = infoOperazione.BaseCode,
                        .TopCode = infoOperazione.TopCode,
                        .Programmazione_Entita_Cod = 0, 'DT: valorizzato solo in caso di salvataggio planning
                        .QuotaDistribuzione = If(SuperficieTrattataTotale <> 0, esercizioCDC.superficieTrattata / SuperficieTrattataTotale, 0)
                    }

                    If infoOperazione.IsTrattamento OrElse infoOperazione.IsFertilizzazione Then
                        Movimento_Destinazione.Sup_Riduzione_BufferZone = esercizioCDC.superficieRiduzioneBufferZone
                        Movimento_Destinazione.Perc_Riduzione_Deriva = esercizioCDC.percentualeRiduzioneDeriva
                    End If

                    Movimenti_Destinazioni.Add(Movimento_Destinazione)

                End If
            Next
        End If

        Return Movimenti_Destinazioni

    End Function

    Private Function CreaMovimentiDestinazioneDaQuantitaSuImpianti(QuantitaSuImpianti As List(Of QuantitaSuImpianto), agenda As Operazione_Agenda, infoOperazione As InfoOperazione, SuperficieTrattataTotale As Decimal) As List(Of Movimento_Destinazione)
        If QuantitaSuImpianti Is Nothing Then
            Return Nothing
        End If

        Dim Movimenti_Destinazioni As New List(Of Movimento_Destinazione)

        For Each quantitaSuImpianto In QuantitaSuImpianti

            Dim esercizioCDC = quantitaSuImpianto.esercizioCDC
            Dim Piva = esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva
            Dim Sa_Cod = esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice
            Dim Appezza = esercizioCDC.esercizio.impiantoPK.appezzamentoPK.codice
            Dim Id_Destinazione = esercizioCDC.esercizio.impiantoPK.codice

            Dim Movimento_Destinazione = Movimenti_Destinazioni.Find(Function(c) (c.Piva = Piva _
                                                                         AndAlso c.Sa_Cod = Sa_Cod _
                                                                         AndAlso c.Appezza = Appezza _
                                                                         AndAlso c.Id_Destinazione = Id_Destinazione))

            If Movimento_Destinazione Is Nothing Then
                Movimento_Destinazione = New Movimento_Destinazione With {
                        .Id_Agenda = agenda.Id_Agenda,
                        .Data = agenda.Data,
                        .Piva = Piva,
                        .Sa_Cod = Sa_Cod,
                        .Appezza = Appezza,
                        .Id_Destinazione = Id_Destinazione,
                        .Tipo = 0,
                        .Qta = 0,
                        .Qta2 = esercizioCDC.superficieTrattata,
                        .BaseCode = infoOperazione.BaseCode,
                        .TopCode = infoOperazione.TopCode,
                        .Programmazione_Entita_Cod = 0, 'DT: valorizzato solo in caso di salvataggio planning
                        .QuotaDistribuzione = If(SuperficieTrattataTotale <> 0, esercizioCDC.superficieTrattata / SuperficieTrattataTotale, 0)
                    }

                Movimenti_Destinazioni.Add(Movimento_Destinazione)

            End If

            Movimento_Destinazione.Qta += quantitaSuImpianto.Qta


        Next

        Return Movimenti_Destinazioni
    End Function

    Private Function CreaMovimentiDettagliRiferimenti(attivita As Attivita, agenda As Operazione_Agenda, infoOperazione As InfoOperazione) As List(Of Movimento_Dettaglio_Riferimento)

        Dim Movimenti_Dettagli_Riferimenti As New List(Of Movimento_Dettaglio_Riferimento)

        If infoOperazione.IsVisita AndAlso attivita.attivitaCollegate IsNot Nothing Then

            For Each attivitaCollegata In attivita.attivitaCollegate

                Dim mov_det_rif = New Movimento_Dettaglio_Riferimento

                mov_det_rif.Piva_Rif = attivitaCollegata.centroAziendale.primaryKey.partitaIva
                mov_det_rif.Sa_Cod_Rif = attivitaCollegata.centroAziendale.primaryKey.codice
                mov_det_rif.Id_Agenda_Rif = attivitaCollegata.codice
                mov_det_rif.Id_Mov = -1
                mov_det_rif.Id_Mov_Det = -1
                mov_det_rif.Cau_Mov = ""
                mov_det_rif.Id_Mov_Rif = -1
                mov_det_rif.Id_Mov_Det_Rif = -1
                mov_det_rif.Lav_Cod_Rif = attivitaCollegata.job.primaryKey.codice
                mov_det_rif.Cau_Mov_Rif = CAU_ASSEGNATARIO_VISITA

                Movimenti_Dettagli_Riferimenti.Add(mov_det_rif)

            Next

        End If

        Return Movimenti_Dettagli_Riferimenti
    End Function

    Private Function BuildWkt(latitude As Double, longitude As Double) As String

        Dim wkt As String = ""

        If longitude <> 0 AndAlso latitude <> 0 Then
            wkt = "POINT (" & longitude.ToString.Replace(",", ".").Trim() & ", " & latitude.ToString.Replace(",", ".").Trim() & ")"
        End If

        Return wkt

    End Function

    Private Function GetListJobDescription(job As Job, objParametri_Server As AgronicaCoreParametri) As String
        Dim objLavorazione = New AgronicaCoreAnagrafeBIZ.Lavorazione_R

        Dim listJobDescription = objLavorazione.GetLavDes(job.primaryKey.codice, objParametri_Server)

        Return listJobDescription
    End Function

    Private Function GetPrincipiAttiviTitoli(principiAttivi As List(Of PrincipioAttivo)) As String
        Dim principiAttiviTitoli As String = ""

        'pa_cod1§titolo1|pa_cod2§titolo2 ...
        If principiAttivi IsNot Nothing Then
            For Each principioAttivo As PrincipioAttivo In principiAttivi
                If principiAttiviTitoli <> "" Then
                    principiAttiviTitoli += "|"
                End If
                principiAttiviTitoli &= principioAttivo.codice & "§" & principioAttivo.titolo.ToString.Replace(",", ".")
            Next
        End If

        Return principiAttiviTitoli
    End Function

    Private Function GetPrincipiAttiviPesi(principiAttivi As List(Of PrincipioAttivo)) As String
        Dim principiAttiviPesi As String = ""

        'pa_cod1§peso1|pa_cod2§peso2 ...
        If principiAttivi IsNot Nothing Then
            For Each principioAttivo As PrincipioAttivo In principiAttivi
                If principiAttiviPesi <> "" Then
                    principiAttiviPesi += "|"
                End If
                principiAttiviPesi &= principioAttivo.codice & "§" & principioAttivo.peso.ToString.Replace(",", ".")
            Next
        End If

        Return principiAttiviPesi
    End Function

    Private Function GetPrincipiAttiviPercentualeSuperficieTrattabile(principiAttivi As List(Of PrincipioAttivo)) As String
        Dim principiAttiviPercentualeSuperficieTrattabile As String = ""

        'pa_cod1§percSupTratt1|pa_cod2§percSupTratt2 ...
        If principiAttivi IsNot Nothing Then
            For Each principioAttivo As PrincipioAttivo In principiAttivi
                If principioAttivo.percentualeSuperficieTrattabile <> 0 Then
                    If principiAttiviPercentualeSuperficieTrattabile <> "" Then
                        principiAttiviPercentualeSuperficieTrattabile += "|"
                    End If
                    'DT: su db la superficie trattabile è sempre scritta con la virgola decimale, gliela forzo
                    principiAttiviPercentualeSuperficieTrattabile &= principioAttivo.codice & "§" & principioAttivo.percentualeSuperficieTrattabile.ToString.Replace(".", ",")
                End If
            Next
        End If

        Return principiAttiviPercentualeSuperficieTrattabile
    End Function

    ''' <summary>
    ''' Crea un dizionario contenente il totale delle quantità di prodotto
    ''' raccolto per ogni appezzamento, separando i movimenti con lotto diverso
    ''' o a magazzini diversi
    ''' </summary>
    ''' <param name="dettagliProdotti">Dettagli raccolta su cui effettuare la ripartizione</param>
    ''' <param name="assegnamentoLotto">Funzione di assegnamento del lotto</param>
    ''' <returns></returns>
    Private Function RipartizionaQuantitaRaccolta(dettagliProdotti As List(Of DettaglioRaccolta),
            assegnamentoLotto As Func(Of QuantitaSuImpianto, String)
        ) As Dictionary(Of (ProdottoCod As Integer, Id_esercizio As String, Id_Magazzino As String, Lotto As String), QuantitaSuImpianto)

        Dim DictQtaSuImpianti As New Dictionary(Of (Integer, String, String, String), QuantitaSuImpianto)
        Dim Id_esercizio As String
        Dim Id_Magazzino As String
        Dim Lotto As String
        Dim quantita As Double
        Dim piante As Double

        Dim QtaTotImpianto As QuantitaSuImpianto

        For Each risorsa In dettagliProdotti
            Dim ProdottoCod As Integer = risorsa.prodotto.codice

            Dim Sup_Tot As Double = (From q In risorsa.QuantitaSuImpianti
                                     Select q.esercizioCDC.superficieTrattata).
                                ToList().Aggregate(Function(s1, s2) s1 + s2)

            Dim Piante_Tot As Double = (From q In risorsa.QuantitaSuImpianti
                                        Select q.esercizioCDC.superficieTrattata * q.esercizioCDC.esercizio.piante_Ha).
                                ToList().Aggregate(Function(s1, s2) s1 + s2)

            Dim lastLotto As String = ""
            For Each impianto In risorsa.QuantitaSuImpianti
                Id_esercizio = getEsercizioCDCKey(impianto.esercizioCDC)
                Id_Magazzino = getMagazzinoKey(impianto.Magazzino)
                Lotto = assegnamentoLotto.Invoke(impianto)
                Lotto = If(IsNothing(Lotto), lastLotto, Lotto)

                If risorsa.Opzioni_Raccolta.Ripartizione.Equals(enum_Ripartizione_Raccolta.MANUALE) Then
                    quantita = impianto.Qta

                ElseIf risorsa.Opzioni_Raccolta.Ripartizione.Equals(enum_Ripartizione_Raccolta.AUTO_PIANTE) AndAlso Piante_Tot > 0 Then
                    piante = impianto.esercizioCDC.superficieTrattata * impianto.esercizioCDC.esercizio.piante_Ha
                    quantita = risorsa.quantitaTotaleReale * piante / Piante_Tot

                Else
                    quantita = risorsa.quantitaTotaleReale * impianto.esercizioCDC.superficieTrattata / Sup_Tot

                End If

                If quantita = 0 AndAlso ProdottoCod <> 0 Then
                    Continue For 'Evito di aggiungere gli impianti per cui non è stata specificata una quantità
                End If

                If DictQtaSuImpianti.ContainsKey((ProdottoCod, Id_esercizio, Id_Magazzino, Lotto.ToUpper)) Then
                    quantita += DictQtaSuImpianti.Item((ProdottoCod, Id_esercizio, Id_Magazzino, Lotto.ToUpper)).Qta

                    DictQtaSuImpianti.Remove((ProdottoCod, Id_esercizio, Id_Magazzino, Lotto.ToUpper))
                End If

                QtaTotImpianto = New QuantitaSuImpianto()
                QtaTotImpianto.Qta = quantita
                QtaTotImpianto.esercizioCDC = impianto.esercizioCDC

                If IsNothing(impianto.Magazzino) Then
                    QtaTotImpianto.Magazzino = New Fabbricato()
                    QtaTotImpianto.Magazzino.primaryKey = New Fabbricato.PK()
                    QtaTotImpianto.Magazzino.primaryKey.codice = 0
                    QtaTotImpianto.Magazzino.primaryKey.centroAziendalePK = New CentroAziendale.PK()
                    QtaTotImpianto.Magazzino.primaryKey.centroAziendalePK.codice = 0
                    QtaTotImpianto.Magazzino.primaryKey.centroAziendalePK.partitaIva = ""
                Else
                    QtaTotImpianto.Magazzino = impianto.Magazzino
                End If

                lastLotto = Lotto
                QtaTotImpianto.Lotto = Lotto
                QtaTotImpianto.Prodotto = risorsa.prodotto

                DictQtaSuImpianti.Add((ProdottoCod, Id_esercizio, Id_Magazzino, Lotto.ToUpper), QtaTotImpianto)
            Next

        Next

        Return DictQtaSuImpianti
    End Function

    ''' <summary>
    ''' Genera un prodotto a partire da una specie (varietà Altre).
    ''' 
    ''' </summary>
    ''' <param name="specie">Specie da cui derivare il prodotto.</param>
    ''' <param name="Elem_Cod">SEMILAVORATI_VEGETALI o TRASFORMATI_VEGETALI.</param>
    ''' <param name="objParametri_Server"></param>
    ''' <returns>Il codice del prodotto in formato stringa</returns>
    Private Function GeneraProdotto(specie As Specie, Elem_Cod As Integer, objParametri_Utenti As AgronicaCoreParametri, objParametri_Server As AgronicaCoreParametri) As String

        If Elem_Cod <> TRASFORMATI_VEGETALI AndAlso Elem_Cod <> SEMILAVORATI_VEGETALI Then
            Throw New Exception(My.Resources.AgronicaCoreMapper.ErroreMateriaPrima)
        End If

        Dim OUTPUT_Mat_Cod As Integer
        Dim OUTPUT_Mat_Des As String

        Dim Piva_Creazione As String = objParametri_Server.PivaSuperUser 'azienda che lo crea
        Dim Sa_Cod_Creazione As Integer = PUBBLICO 'pubblico

        '--- Controllo che esista la varietà "Altre"
        Dim Cul_Cod As Integer = New AgronicaCoreMetaSchemaDAL.Cultivar_R().CulCod_Altre_from_VegCod(specie.codice, objParametri_Server)
        If Cul_Cod < 1 Then
            Throw New Exception(My.Resources.AgronicaCoreMapper.NoVarietaAltre)
        End If

        Dim objImportaGias As New AgronicaCoreAnagrafeBIZ.Importa_GIAS
        Dim Regolamento As enum_Cod_Regolamento = enum_Cod_Regolamento.Regolamento_Nessuno
        Dim Flag_Biologico As Boolean = False

        Dim Cod_Articolo As String = "RACC/" & Right("000" & specie.codice, 3) &
                           "/" & Right("00000000" & CStr(Cul_Cod), 8)

        OUTPUT_Mat_Des = specie.descrizione & " - Altre"

        Dim objCore_MP_R As New AgronicaCoreAnagrafeDAL.Materie_Prime_R

        '--- Controllo l'esistenza del prodotto
        If checkProductExistence(specie.codice, Cul_Cod, Elem_Cod, objParametri_Server) Then
            Dim dt As DataTable = objCore_MP_R.Leggi(Piva_Creazione, -1, Elem_Cod, 0, "",
                                                    specie.codice, Cul_Cod,
                                                    0, 0, 0, 0, 0, "",
                                                    0, "", False, False, "",
                                                    enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                    "", "", objParametri_Server)

            OUTPUT_Mat_Cod = dt.Rows(0).Item("Mat_Cod")

            Return OUTPUT_Mat_Cod

        End If

        '--- Se il prodotto non esiste ne creo uno nuovo
        Dim objCore_XML_Anagrafe As New AgronicaCoreXML.XML_Anagrafe
        Dim objCore_MP_W As New AgronicaCoreAnagrafeBIZ.Materie_Prime_W
        Dim Flag_Insert As Boolean
        Dim Basecode As Integer = 0
        Dim Topcode As Integer = 0

        Dim progressivoGias = getProgressivoGias(objParametri_Utenti)

        UtilityProvider.Calcola_BaseCode_TopCode(Basecode, Topcode, progressivoGias)


        Select Case Elem_Cod
            Case SEMILAVORATI_VEGETALI
                objImportaGias.Creazione_Automatica_SemilavoratoVegetale(objParametri_Server,
                                    objCore_XML_Anagrafe, objCore_MP_W,
                                    Flag_Insert, OUTPUT_Mat_Cod, Basecode, Topcode,
                                    Piva_Creazione, Sa_Cod_Creazione, Cod_Articolo,
                                    OUTPUT_Mat_Des, specie.codice, Cul_Cod,
                                    Regolamento, Flag_Biologico)
            Case TRASFORMATI_VEGETALI
                objImportaGias.Creazione_Automatica_TrasformatoVegetale(objParametri_Server,
                                    objCore_XML_Anagrafe, objCore_MP_W,
                                    Flag_Insert, OUTPUT_Mat_Cod, Basecode, Topcode,
                                    Piva_Creazione, Sa_Cod_Creazione, Cod_Articolo,
                                    OUTPUT_Mat_Des, specie.codice, Cul_Cod,
                                    Regolamento, Flag_Biologico)
        End Select

        Return OUTPUT_Mat_Cod
    End Function

    Private Function checkProductExistence(Veg_Cod As Integer, Cul_Cod As Integer, Elem_Cod As Integer,
                                           objParametri_Server As AgronicaCoreParametri)

        Dim Piva As String = objParametri_Server.PivaSuperUser
        Dim xFiltro As String = " Sa_Cod = -1 "
        Dim objCore_MP_R As New AgronicaCoreAnagrafeDAL.Materie_Prime_R

        Select Case Elem_Cod
            Case SEMILAVORATI_VEGETALI
                Return objCore_MP_R.Esiste_SemilavoratoVegetale(Piva, Veg_Cod,
                                                                Cul_Cod, 0,
                                                                xFiltro,
                                                                objParametri_Server)
            Case TRASFORMATI_VEGETALI
                Return objCore_MP_R.Esiste_TrasformatoVegetale(Piva, Veg_Cod,
                                                               Cul_Cod, 0,
                                                               xFiltro,
                                                               objParametri_Server)
        End Select

        Throw New Exception(My.Resources.AgronicaCoreMapper.ErroreMateriaPrima)

    End Function

    Private Function getProgressivoGias(objParametri_Utenti As AgronicaCoreParametri)
        Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_CodiciGiasPRO_R
        Dim dt As DataTable = objUtenti.Leggi("", "", objParametri_Utenti)
        Dim progressivogias As Long = dt.Rows(0).Item("ProgressivoGIAS")

        Return progressivogias
    End Function

    Private Function setNewProductCode(codiceProdotto As String, Elem_Cod As Integer, ByRef risorsa As risorse.RisorsaProdotto)
        risorsa.prodotto = New risorse.Prodotto()
        risorsa.prodotto.codice = codiceProdotto
        risorsa.prodotto.elemCod = Elem_Cod

        If risorsa.MagazziniMovimentazioni IsNot Nothing Then
            For Each movimento In risorsa.MagazziniMovimentazioni
                If movimento.Prodotto IsNot Nothing AndAlso movimento.Prodotto.codice <> -1 Then
                    Continue For
                End If
                movimento.Prodotto = risorsa.prodotto
            Next
        End If

        Return True
    End Function

    Private Function GeneraLottoRaccolta(ByRef risorsaProdotto As risorse.RisorsaProdotto,
                                         agenda As Operazione_Agenda,
                                         objParametri_Server As AgronicaCoreParametri) As String
        If risorsaProdotto.classType <> ClassType.DettaglioRaccolta Then
            Return ""
        End If

        Dim raccolta = CType(risorsaProdotto, DettaglioRaccolta)
        Dim lotto As String = ""

        If raccolta.MagazziniMovimentazioni Is Nothing OrElse
raccolta.MagazziniMovimentazioni.Count = 0 OrElse
            raccolta.MagazziniMovimentazioni.First().Magazzino.primaryKey.codice = 0 Then

            For Each q In raccolta.QuantitaSuImpianti
                q.Lotto = lotto
            Next
            For Each mov In raccolta.MagazziniMovimentazioni
                mov.Lotto = lotto
            Next

            Return lotto
        End If
        Select Case (raccolta.Opzioni_Raccolta.GenerazioneLotto)
            Case enum_Generazione_Lotto_Raccolta.DA_DATA
                lotto = agenda.Data.Year & "" &
                       agenda.Data.Month.ToString.PadLeft(2, "0") & "" &
                       agenda.Data.Day.ToString.PadLeft(2, "0") & ""

            Case enum_Generazione_Lotto_Raccolta.UNIVOCO

                If raccolta.QuantitaSuImpianti.First().Lotto IsNot Nothing AndAlso
                    raccolta.QuantitaSuImpianti.First().Lotto.StartsWith("LR/") Then

                    lotto = raccolta.QuantitaSuImpianti.First().Lotto
                    Exit Select
                End If

                Dim seq As New AgronicaCoreDataProvider.Agro_Sequenze
                Dim id As Integer = seq.NuovoId_Tabella("GeneraIdLottoRaccolta", 0, 2000000000, objParametri_Server)

                'Il codice anche se ha dei valori parlanti non deve essere parlante, basta ce ne sia uno univoco
                'i dati importati da arpt avranno in testata 14-02- e il codice loro di 10 cifre ricavato dall'azienda e progressivo loro aziendale
                lotto = CStr(id).PadLeft(10, "0") '10 caratteri per id
                lotto = lotto & "/" & objParametri_Server.PivaSuperUser
                lotto = "LR/" & lotto

            Case enum_Generazione_Lotto_Raccolta.DA_ESERCIZO,
                 enum_Generazione_Lotto_Raccolta.MANUALE
                If raccolta.MagazziniMovimentazioni IsNot Nothing AndAlso raccolta.MagazziniMovimentazioni.Count > 0 Then
                    If raccolta.MagazziniMovimentazioni.First().Lotto <> "" Then
                        lotto = raccolta.MagazziniMovimentazioni.First().Lotto
                    End If
                End If

        End Select

        For Each mov In risorsaProdotto.MagazziniMovimentazioni
            mov.Lotto = lotto
        Next
        For Each mov In raccolta.MagazziniMovimentazioni
            mov.Lotto = lotto
        Next
        For Each q In raccolta.QuantitaSuImpianti
            q.Lotto = lotto
        Next

        Return lotto
    End Function

    Private Sub EsplodiImpiantiRaccolta(ByRef risorseProdotto As List(Of risorse.RisorsaProdotto), ByRef attivita As Attivita)

        Dim nuoveRisorse As New List(Of DettaglioRaccolta)
        Dim risorsa As DettaglioRaccolta
        Dim nuovaRisorsa As DettaglioRaccolta
        Dim dict As Dictionary(Of (ProdottoCod As Integer, Id_esercizio As String, Id_Magazzino As String, Lotto As String), QuantitaSuImpianto)

        Dim dettagliProdotti As List(Of DettaglioRaccolta) = risorseProdotto.Where(Function(r) (r.classType = ClassType.DettaglioRaccolta)).ToList().
                                                                            ConvertAll(Function(obj) (CType(obj, DettaglioRaccolta)))

        Dim modalitaGenerazioneLotto = dettagliProdotti.First().Opzioni_Raccolta.GenerazioneLotto

        attivita.risorse.RemoveAll(Function(r) (r.classType = ClassType.DettaglioRaccolta))
        risorseProdotto.RemoveAll(Function(r) (r.classType = ClassType.DettaglioRaccolta))
        If modalitaGenerazioneLotto = enum_Generazione_Lotto_Raccolta.DA_ESERCIZO OrElse
           modalitaGenerazioneLotto = enum_Generazione_Lotto_Raccolta.MANUALE Then
            'dict = RipartizionaQuantitaRaccolta(dettagliProdotti, Function(q) (q.esercizioCDC.esercizio.descrizione))

            dict = RipartizionaQuantitaRaccolta(dettagliProdotti, Function(q) (q.Lotto))
        Else
            dict = RipartizionaQuantitaRaccolta(dettagliProdotti, Function(q) (q.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice))
        End If

        For Each impianto In dict.Keys
            risorsa = dettagliProdotti.Find(Function(r) (r.prodotto.codice = impianto.ProdottoCod AndAlso
                    If(impianto.Id_Magazzino = "", True, impianto.Id_Magazzino = getMagazzinoKey(r.MagazziniMovimentazioni.First().Magazzino))))

            '-- Raggruppa se stesso prodotto, lotto, magazzino
            nuovaRisorsa = nuoveRisorse.Find(Function(r) (r.prodotto.codice = impianto.ProdottoCod AndAlso
                                                 r.QuantitaSuImpianti.First().Lotto.ToUpper = impianto.Lotto.ToUpper AndAlso
                                                 If(impianto.Id_Magazzino = "", True, impianto.Id_Magazzino = getMagazzinoKey(r.MagazziniMovimentazioni.First().Magazzino))))

            If risorsa.Opzioni_Raccolta.GenerazioneLotto = enum_Generazione_Lotto_Raccolta.DA_ESERCIZO Then
                '-- In questo caso suddivido ulteriormente rendendo il codice dell'esercizio una chiave
                nuovaRisorsa = nuoveRisorse.Find(Function(r) (
                                                     r.prodotto.codice = impianto.ProdottoCod AndAlso
                                                     r.QuantitaSuImpianti.First().Lotto.ToUpper = impianto.Lotto.ToUpper AndAlso
                                                     If(impianto.Id_Magazzino = "", True, impianto.Id_Magazzino = getMagazzinoKey(r.MagazziniMovimentazioni.First().Magazzino)) AndAlso
                                                     impianto.Id_esercizio.Split("_").LastOrDefault() = r.QuantitaSuImpianti.First().esercizioCDC.esercizio.codice
                                                     ))
            End If

            If nuovaRisorsa IsNot Nothing Then
                nuovaRisorsa.QuantitaSuImpianti.Add(dict.Item(impianto))
                nuovaRisorsa.quantitaTotaleReale += dict.Item(impianto).Qta

                If nuovaRisorsa.MagazziniMovimentazioni.Count > 0 Then
                    nuovaRisorsa.MagazziniMovimentazioni.ForEach(Sub(m) m.Qta += dict.Item(impianto).Qta)
                End If

                If risorsa.QuantitaSuImpianti.Count() <> nuovaRisorsa.QuantitaSuImpianti.Count() Then
                    Continue For
                End If

                '-- Le liste hanno la stessa lunghezza, potrebbero avere gli stessi elementi
                Dim hasSameElements = True

                For Each item In nuovaRisorsa.QuantitaSuImpianti
                    Dim found = risorsa.QuantitaSuImpianti.Find(Function(q) (
                                                                    q.Qta = item.Qta AndAlso
                                                                    q.Lotto.ToUpper = item.Lotto.ToUpper AndAlso
                                                                    q.esercizioCDC.esercizio.codice = item.esercizioCDC.esercizio.codice
                                                                    ))
                    If found Is Nothing Then
                        hasSameElements = False
                        Exit For
                    End If
                Next

                If hasSameElements Then
                    '-- Se le liste posseggono gli stessi elementi, copio la
                    ' quantitàTotaleReale dell'oggetto originale in quello nuovo
                    ' per evitare alterazioni dovute ai float

                    nuovaRisorsa.quantitaTotaleReale = risorsa.quantitaTotaleReale
                    For Each mov In nuovaRisorsa.MagazziniMovimentazioni
                        mov.Qta = risorsa.quantitaTotaleReale
                    Next
                End If

                Continue For
            End If
            '-- End regrouping

            nuovaRisorsa = risorsa.Clona()

            nuovaRisorsa.QuantitaSuImpianti.Clear()
            nuovaRisorsa.QuantitaSuImpianti.Add(dict.Item(impianto))

            If nuovaRisorsa.MagazziniMovimentazioni IsNot Nothing AndAlso nuovaRisorsa.MagazziniMovimentazioni.Count > 0 Then
                nuovaRisorsa.MagazziniMovimentazioni.ForEach(Sub(m) m.Lotto = impianto.Lotto)
                nuovaRisorsa.MagazziniMovimentazioni.ForEach(Sub(m) m.Qta = dict.Item(impianto).Qta)

                If risorsa.Opzioni_Raccolta.GenerazioneLotto = enum_Generazione_Lotto_Raccolta.DA_ESERCIZO Then
                    nuovaRisorsa.MagazziniMovimentazioni.ForEach(Sub(m) m.Cod_Progetto = impianto.Id_esercizio.Split("_").LastOrDefault())
                End If

            End If
            nuovaRisorsa.quantitaTotaleReale = dict.Item(impianto).Qta

            nuoveRisorse.Add(nuovaRisorsa)
        Next

        attivita.risorse.AddRange(nuoveRisorse)
        risorseProdotto.AddRange(nuoveRisorse)

    End Sub

    Private Function getEsercizioCDCKey(cdc As EsercizioCDC) As String
        If IsNothing(cdc) Then
            Return ""
        End If
        Return cdc.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva & "_" &
                cdc.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice & "_" &
                cdc.esercizio.impiantoPK.appezzamentoPK.codice & "_" &
                cdc.esercizio.impiantoPK.codice & "_" &
                cdc.esercizio.codice
    End Function

    Private Function getMagazzinoKey(magazzino As Fabbricato) As String
        If IsNothing(magazzino) Then
            Return ""
        End If
        Return magazzino.primaryKey.centroAziendalePK.partitaIva & "_" &
            magazzino.primaryKey.centroAziendalePK.codice & "_" &
            magazzino.primaryKey.codice
    End Function

    Private Function CreaMovimentiDestinazioniTrappole(ByVal attivita As Attivita, ByVal agenda As Operazione_Agenda,
                                                       ByVal movimentoDettaglio As Movimento_Dettaglio, ByVal infoOperazione As InfoOperazione, ByVal SuperficieTrattataTotale As Decimal) As List(Of Movimento_Destinazione)

        Dim MovimentiDestinazioni As New List(Of Movimento_Destinazione)

        If Not IsNothing(attivita.risorse) AndAlso attivita.risorse.Count > 0 Then

            Dim dettagliTrattamento As List(Of dettagli.DettaglioTrattamento) = attivita.risorse.FindAll(Function(c) (c.classType = ClassType.DettaglioTrattamento)).ConvertAll(Function(obj1) CType(obj1, dettagli.DettaglioTrattamento))

            If Not IsNothing(dettagliTrattamento) AndAlso dettagliTrattamento.Count > 0 Then

                dettagliTrattamento = dettagliTrattamento.FindAll(Function(c) (c.prodotto.codice = movimentoDettaglio.Pro_Cod AndAlso ((movimentoDettaglio.Lotto = "" AndAlso (c.GetLotti().Count = 0 OrElse c.GetLotti()(0) = "")) OrElse
                                                                                                                                        (movimentoDettaglio.Lotto <> "" AndAlso c.GetLotti().Count = 1 AndAlso c.GetLotti()(0) = movimentoDettaglio.Lotto))))

                If Not IsNothing(dettagliTrattamento) AndAlso dettagliTrattamento.Count > 0 Then

                    MovimentiDestinazioni = CreaMovimentiDestinazione(agenda, infoOperazione, attivita.centriDiCosto, 0, SuperficieTrattataTotale)

                    For Each dettaglioTrattamento In dettagliTrattamento

                        If Not IsNothing(dettaglioTrattamento.quantitaSuImpianti) AndAlso dettaglioTrattamento.quantitaSuImpianti.Count > 0 Then

                            For Each quantitaSuImpianto In dettaglioTrattamento.quantitaSuImpianti

                                If Not IsNothing(quantitaSuImpianto.esercizioCDC) Then

                                    Dim index As Integer = MovimentiDestinazioni.FindIndex(Function(mov) mov.Piva = quantitaSuImpianto.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva AndAlso
                                                                                                            mov.Sa_Cod = quantitaSuImpianto.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice AndAlso
                                                                                                           mov.Appezza = quantitaSuImpianto.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.codice AndAlso
                                                                                                            mov.Id_Destinazione = quantitaSuImpianto.esercizioCDC.esercizio.impiantoPK.codice)

                                    If index > -1 Then
                                        Dim DoseHA_Trasformata = GetDoseTrasformata(quantitaSuImpianto.Qta, dettaglioTrattamento.unitaDiMisuraIndicata.codice)

                                        MovimentiDestinazioni(index).Qta += DoseHA_Trasformata
                                    End If
                                End If

                            Next
                        End If
                    Next
                End If

            End If

        End If

        Return MovimentiDestinazioni

    End Function
#End Region


#Region "Controlli"
    Public Sub CheckMagazzino_ListaAttivita(lista_Attivita As List(Of AgronicaCoreModelsSTD.attivita.Attivita),
                                            ByRef lista_Errori As List(Of ErroreGias),
                                            mostraWarning_CheckMagazzino As Boolean,
                                            CaricoMagazzinoAutomatico As Boolean,
                                            objParametri_Super_Server As AgronicaCoreParametri,
                                            objParametri_Server As AgronicaCoreParametri,
                                            objParametri_Utenti As AgronicaCoreParametri
                                            )

        Dim currentAttivitaDes = ""

        Try


            '===================================
            '   CONTROLLI TERZISTA/MAZ. GERARCHICO     to do!!
            '-----------------------------------
            'For Each attivita In lista_Attivita

            '    currentAttivitaDes = attivita.job.descrizione
            '    Dim InfoOperazione As InfoOperazione = GetInfoOperazione(attivita.job.primaryKey.codice, Tipo_Attivita.QuadernoDiCampagna)
            '    Dim agenda As Operazione_Agenda = lista_Agende.First(Function(x) x.Key = attivita.job.primaryKey.codice).Value

            '    '===================================
            '    '   CONTROLLI TERZISTA/MAZ. GERARCHICO
            '    '-----------------------------------
            '    'ROBE TERZISTA...
            '    CheckMagazzino(attivita, agenda, InfoOperazione, currentAttivitaDes, mostraWarning_CheckMagazzino, objParametri_Super_Server, objParametri_Server, objParametri_Utenti, lista_Errori)
            '    If lista_Errori.FindAll(Function(c) (c.severity = ErroreGias_Severity.Bloccante)).Count > 0 Then
            '        'alla prima occorrenza di errore bloccante esco
            '        Exit Sub
            '    End If
            'Next

            '===================================
            '   CONTROLLI CONFORMITA GIACENZE
            '-----------------------------------
            If esisteAlmenoUnaMovimentazione(lista_Attivita) AndAlso Not CaricoMagazzinoAutomatico Then
                CheckGiacenze(mostraWarning_CheckMagazzino, lista_Attivita, 0, lista_Errori, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)

                If lista_Errori.FindAll(Function(c) (c.severity = ErroreGias_Severity.Bloccante)).Count > 0 Then
                    'alla prima occorrenza di errore bloccante esco
                    Exit Sub
                End If
            End If

        Catch ex As GiasException
            Throw ex
        Catch ex As Exception
            Throw New Exception(currentAttivitaDes, ex)
        End Try

    End Sub

    Private Shared Sub CheckGiacenze(mostraWarning_CheckMagazzino As Boolean,
                                     lista_Attivita As List(Of Attivita),
                                     ByRef Id_Agenda_Scarico As Integer,
                                     ByRef lista_Errori As List(Of ErroreGias),
                                     objParametri_Super_Server As AgronicaCoreParametri,
                                     objParametri_Server As AgronicaCoreParametri,
                                     objParametri_Utenti As AgronicaCoreParametri)

        'TODO_DT: IMPORTANTE correggere per magazzino esterno
        Dim mapper As New AgronicaCoreMapper.AttivitaToAgenda
        Dim dic_AgendexElemCod As Dictionary(Of Integer, Operazione_Agenda) = sommatoriaAttivita_xElemCod_MagazziniEsterni(lista_Attivita, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)

        Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim BloccoGiacenze As Integer = CInt(objUtentiImpostazioni.LeggiValoreImpostazioneScalare(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE, objParametri_Utenti.UtenteUsername, objParametri_Utenti))

        Dim objImpreseImpostazioni As New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_R

        For Each dic In dic_AgendexElemCod
            Dim _agenda As Operazione_Agenda = dic.Value

            Dim InfoOperazione As InfoOperazione = GetInfoOperazione(_agenda.Lav_Cod, Tipo_Attivita.QuadernoDiCampagna)
            If Not InfoOperazione.IsRaccolta Then

                Dim tipoOperazione As enum_Tipo_Operazione_Agenda = STD_Utility.getTipoOperazione(lista_Attivita(0).tipo, lista_Attivita(0).stato)

                Dim pua As Pua = Nothing

                If tipoOperazione = enum_Tipo_Operazione_Agenda.Ricetta AndAlso Not IsNothing(lista_Attivita(0).testataRicetta.pua) AndAlso lista_Attivita(0).testataRicetta.pua.codice > 0 Then
                    pua = lista_Attivita(0).testataRicetta.pua
                End If

                For Each _movimento In _agenda.Movimenti
                    If _movimento.Cau_Mov = CAU_SCARICO Then
                        For Each _dettaglio In _movimento.Movimenti_Dettagli
                            For Each _destinazione In _dettaglio.Movimenti_Destinazioni

                                Dim Piva_destinazione As String = _destinazione.Piva
                                Dim SaCod_destinazione As String = _destinazione.Sa_Cod
                                Dim FabbricatoCod_destinazione As String = _destinazione.Id_Destinazione
                                Dim Elem_Cod As Integer = _dettaglio.Elem_Cod

                                Dim DescrizioneFabbricato = AgronicaCoreAnagrafeDAL.Fabbricati_R.FabbricatoDes_from_FabbricatoCod(Piva_destinazione, SaCod_destinazione, FabbricatoCod_destinazione, objParametri_Server)


                                'DT: bisognerebbe prendere i centri dei fabbricati, non quelli degli impianti.
                                'ma si è valutato di fermarsi a livello di super user o azienda, quindi per il momento è sufficiente passare la PIVA
                                Dim SoloPresenti As Integer = CInt(objImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser_ElemCod(Piva_destinazione, Nothing,
                                                                                                enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_GIACENZE,
                                                                                                Elem_Cod, enum_Gestione_Giacenze.TuttiProdotti,
                                                                                                objParametri_Utenti, objParametri_Server))


                                Dim severity As Integer = If(BloccoGiacenze = 1 OrElse SoloPresenti = 1, ErroreGias_Severity.WarningBloccante, ErroreGias_Severity.Warning)

                                'Il controllo di giacenza per le ricette non può essere bloccante
                                If severity = ErroreGias_Severity.WarningBloccante Then
                                    If tipoOperazione = enum_Tipo_Operazione_Agenda.Ricetta Then
                                        severity = ErroreGias_Severity.Warning
                                    End If
                                End If

                                Dim Lotto As String = _dettaglio.Lotto

                                Dim Mat_Cod As Integer = _dettaglio.Mat_Cod
                                Dim Pro_Cod As Integer = _dettaglio.Pro_Cod
                                Dim Udm_Cod As Integer = _dettaglio.Udm_Cod

                                Dim Eff_Cod As Integer = 0
                                Dim Carico As Decimal = 0
                                Dim Udm_Effluente As New UnitaDiMisura

                                Dim Qta_Tot As Decimal = _destinazione.Qta
                                Dim Qta_In_Data As Decimal = 0

                                Dim Av_Cod As Integer = 0

                                Dim Av_Gru_Cod As Integer = 0

                                Dim DescrizioneProdotto As String = ""

                                Dim DescrizioneUnitaMisura As String = GetDescrizioneUDM(Udm_Cod, objParametri_Server)

                                Dim disciplinare As Disciplinare = Nothing

                                Dim attivita_del_Lav_Cod As Attivita = lista_Attivita.Find(Function(a) CInt(a.job.primaryKey.codice) = _agenda.Lav_Cod)

                                If Not IsNothing(attivita_del_Lav_Cod) Then

                                    disciplinare = attivita_del_Lav_Cod.disciplinare

                                    If Not IsNothing(attivita_del_Lav_Cod.risorse) AndAlso attivita_del_Lav_Cod.risorse.Count > 0 Then

                                        'Recupero l'effluente dal dettaglioFertilizzazione
                                        Dim dettagliFertilizzazione As List(Of DettaglioFertilizzazione) = attivita_del_Lav_Cod.risorse.FindAll(Function(r) r.classType = ClassType.DettaglioFertilizzazione).ConvertAll(Function(obj1) CType(obj1, DettaglioFertilizzazione))

                                        If Not IsNothing(dettagliFertilizzazione) AndAlso dettagliFertilizzazione.Count > 0 Then

                                            Dim dettaglioFertilizzazione = dettagliFertilizzazione.Find(Function(d) d.prodotto.codice = Pro_Cod)

                                            If Not IsNothing(dettaglioFertilizzazione) AndAlso Not IsNothing(dettaglioFertilizzazione.effluente) Then
                                                Eff_Cod = dettaglioFertilizzazione.effluente.codice
                                                Carico = dettaglioFertilizzazione.effluente.carico
                                                Udm_Effluente = dettaglioFertilizzazione.effluente.udm
                                            End If
                                        End If


                                        'Recupero l'Av_Cod e Av_Gru_Cod dall'Avversita del dettaglioTrattamento
                                        If Elem_Cod = INNESCHI Then
                                            Dim dettagliTrattamento As List(Of DettaglioTrattamento) = attivita_del_Lav_Cod.risorse.FindAll(Function(r) r.classType = ClassType.DettaglioTrattamento).ConvertAll(Function(obj1) CType(obj1, DettaglioTrattamento))

                                            If Not IsNothing(dettagliTrattamento) AndAlso dettagliTrattamento.Count > 0 Then

                                                Dim dettaglioTrattamento = dettagliTrattamento.Find(Function(d) Not IsNothing(d.avversitaGruppo) AndAlso d.avversitaGruppo.codice = Pro_Cod)

                                                If Not IsNothing(dettaglioTrattamento) Then
                                                    If dettaglioTrattamento.avversitaGruppo.classType = ClassType.Avversita Then
                                                        Av_Cod = dettaglioTrattamento.avversitaGruppo.codice
                                                    ElseIf dettaglioTrattamento.avversitaGruppo.classType = ClassType.GruppoAvversita Then
                                                        Av_Gru_Cod = dettaglioTrattamento.avversitaGruppo.codice
                                                    End If
                                                End If
                                            End If
                                        End If

                                    End If
                                End If

                                DescrizioneProdotto = Utility_Agenda.GetDescrizioneProdotto(Elem_Cod, Mat_Cod, Pro_Cod, _agenda.Piva, Av_Cod, Av_Gru_Cod, objParametri_Server)

                                'TODO Da Verificare con Monti e correggere:
                                'Ora nel Verifica_Giacenze_Con_Pua per ottenere le qta alla data e totale fa una sottrazione tra la 
                                'qta indicata nel PUA (con unita di misura che sarà quintali o m3) e la qta delle ricette (con unita di misura che kg o l)
                                'senza fare prima un'adeguata conversione
                                If Not IsNothing(pua) AndAlso pua.codice > 0 AndAlso
                                    _agenda.Lav_Cod = LAVCOD_DISTRIBUZIONE_AMMENDANTI AndAlso
                                    tipoOperazione = enum_Tipo_Operazione_Agenda.Ricetta AndAlso
                                    Not IsNothing(disciplinare) AndAlso
                                    Not IsNothing(disciplinare.regolamentoConcimazione) AndAlso
                                    disciplinare.regolamentoConcimazione.tipo = enum_PUARegolamenti_Tipo.PUA Then

                                    'Guardo se la quantità è conforme per la data di intervento
                                    Dim Qs_Ricetta_Cod As Integer = attivita_del_Lav_Cod.testataRicetta.Ricetta_Cod
                                    Dim Qs_Operazione_Ricetta As Integer = attivita_del_Lav_Cod.codice
                                    Dim objRicDet As New AgronicaCoreContabDAL.Ricette_Dettagli_R

                                    Dim Qta_KG_L_Effluente_PUA As Decimal = Carico
                                    STD_Utility.getUdmBasefromUdmIndicata(Udm_Effluente, Qta_KG_L_Effluente_PUA, objParametri_Server)

                                    Qta_In_Data = objRicDet.Verifica_Giacenze_KG_L_Con_Pua_NEW(
                                                                  Qta_KG_L_Effluente_PUA,
                                                                  _agenda.Piva, Qs_Ricetta_Cod,
                                                                  Elem_Cod, Pro_Cod, Mat_Cod,
                                                                  AGRODATAINIZIO, CDate(_agenda.Data),
                                                                  If(IsNumeric(Qs_Operazione_Ricetta), Qs_Operazione_Ricetta, 0),
                                                                  objParametri_Server)

                                    If Qta_Tot > Math.Round(Qta_In_Data, 4) Then
                                        If Not (severity = ErroreGias_Severity.Warning AndAlso mostraWarning_CheckMagazzino = False) Then
                                            Dim messaggio As String = String.Format(QuantitaDichiaratoPUANonSuffScarico, DescrizioneProdotto, _agenda.Data.ToShortDateString, Qta_In_Data, DescrizioneUnitaMisura)
                                            lista_Errori.Add(generaErroreGias(severity, "", messaggio, Key_Parametri_Aggiuntivi_Attivita.mostraWarning_CheckMagazzino))
                                        End If
                                    End If

                                    'Guardo se la quantità di giacenza è conforme a prescindere dalla data
                                    Qta_In_Data = objRicDet.Verifica_Giacenze_KG_L_Con_Pua_NEW(
                                                                  Qta_KG_L_Effluente_PUA,
                                                                  _agenda.Piva, Qs_Ricetta_Cod,
                                                                  Elem_Cod, Pro_Cod, Mat_Cod,
                                                                  AGRODATAINIZIO, AGRODATAFINE,
                                                                  If(IsNumeric(Qs_Operazione_Ricetta), Qs_Operazione_Ricetta, 0),
                                                                  objParametri_Server)

                                    If Qta_Tot > Math.Round(Qta_In_Data, 4) Then
                                        If Not (severity = ErroreGias_Severity.Warning AndAlso mostraWarning_CheckMagazzino = False) Then
                                            Dim messaggio As String = String.Format(GiacenzaDichiaratoPUANonSuffScarico, DescrizioneProdotto, Qta_In_Data, DescrizioneUnitaMisura)
                                            lista_Errori.Add(generaErroreGias(severity, "", messaggio, Key_Parametri_Aggiuntivi_Attivita.mostraWarning_CheckMagazzino))
                                        End If
                                    End If

                                End If

                                'TODO_DT: verificare piva e sacod in caso di magazzino esterno
                                'Guardo se la quantità è conforme per la data di intervento
                                Dim objMovDet As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
                                Qta_In_Data = objMovDet.Verifica_Giacenze_Con_Magazzino_Esterno(Piva_destinazione, SaCod_destinazione, FabbricatoCod_destinazione,
                                                                                            Elem_Cod, Pro_Cod, Mat_Cod,
                                                                                            0, 0, Lotto,
                                                                                            0, Udm_Cod,
                                                                                            AGRODATAINIZIO,
                                                                                            CDate(_agenda.Data),
                                                                                            _agenda.Piva, _agenda.Sa_Cod, _agenda.Id_Agenda,
                                                                                            objParametri_Server,
                                                                                            Id_Agenda_Scarico,
                                                                                            conRaccoglitore:=True
                                                                                            )

                                If Qta_Tot > Math.Round(Qta_In_Data, 4) Then
                                    If Not (severity = ErroreGias_Severity.Warning AndAlso mostraWarning_CheckMagazzino = False) Then
                                        Dim messaggio = String.Format(QuantitaNonSuffScarico, DescrizioneProdotto, DescrizioneFabbricato, _agenda.Data.ToShortDateString, Qta_In_Data, DescrizioneUnitaMisura)
                                        lista_Errori.Add(generaErroreGias(severity, "", messaggio, Key_Parametri_Aggiuntivi_Attivita.mostraWarning_CheckMagazzino))
                                    End If
                                End If

                                Dim dataRiferimento = CDate(_agenda.Data)
                                dataRiferimento = dataRiferimento.AddDays(1)

                                ControllaGiacenzePerData(lista_Errori, severity, mostraWarning_CheckMagazzino, _agenda,
                                                         Piva_destinazione, SaCod_destinazione, FabbricatoCod_destinazione,
                                                         DescrizioneFabbricato,
                                                         Elem_Cod, Pro_Cod, Mat_Cod,
                                                         0, 0, Lotto, 0,
                                                         Udm_Cod, dataRiferimento, AGRODATAFINE,
                                                         Qta_Tot, DescrizioneProdotto, DescrizioneUnitaMisura,
                                                         objParametri_Server, objParametri_Utenti, conRaccoglitore:=True)

                            Next
                        Next
                    End If

                Next
            End If
        Next

    End Sub


    Public Shared Sub CheckMagazzino(attivita As Attivita,
                                     agenda As Operazione_Agenda,
                                     InfoOperazione As InfoOperazione,
                                     currentAttivitaDes As String,
                                     mostraWarning_CheckMagazzino As Boolean,
                                     objParametri_Super_Server As AgronicaCoreParametri,
                                     objParametri_Server As AgronicaCoreParametri,
                                     objParametri_Utenti As AgronicaCoreParametri,
                                     ByRef lista_Errori As List(Of ErroreGias)
                                     )

        Try
            Exit Sub ' to do

            Dim PivaAttivita As String = attivita.centroAziendale.primaryKey.partitaIva

            Dim risorsaProdottoList As List(Of risorse.RisorsaProdotto) =
                attivita.risorse.FindAll(Function(c) (c.classType = InfoOperazione.classType)).ConvertAll(Function(obj1) CType(obj1, risorse.RisorsaProdotto))
            Dim MagazzinoList As New List(Of Fabbricato)

            For Each ris In risorsaProdottoList
                If ris.MagazziniMovimentazioni.Count > 0 Then
                    For Each mov In ris.MagazziniMovimentazioni
                        MagazzinoList.Add(mov.Magazzino)
                    Next
                End If
            Next
            verificaConformitaMagazzini(MagazzinoList, PivaAttivita, currentAttivitaDes, objParametri_Server, objParametri_Utenti, lista_Errori)
            For Each mag In MagazzinoList
                Dim PivaMagazzino = mag.primaryKey.centroAziendalePK.partitaIva
                Dim magazzinoGerarchico As Boolean = isMagazzinoGerarchico(PivaAttivita, PivaMagazzino, objParametri_Server, objParametri_Utenti)
                If Not magazzinoGerarchico Then
                    Dim TerzistaPresente As Boolean = False
                    Dim CodRisUm As Integer = 0
                    For i = 0 To agenda.Movimenti.Count - 1
                        Select Case agenda.Movimenti(i).Cau_Mov
                            Case CAU_IMPUTAZIONE_TERZISTI
                                If agenda.Movimenti(i).Movimenti_Dettagli.Count > 0 Then
                                    For c = 0 To agenda.Movimenti(i).Movimenti_Dettagli.Count - 1
                                        CodRisUm = agenda.Movimenti(i).Movimenti_Dettagli(c).Mat_Cod

                                        Dim ObjCR As New AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R
                                        Dim DtRC As DataTable
                                        DtRC = ObjCR.RapportiContabilixCostiAccessori_ImpreseGias(PivaAttivita,
                                                                                                  False, True,
                                                                                                  enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                                  " cod_risum = " & CodRisUm.ToString, "", objParametri_Server)

                                        For cc = 0 To DtRC.Rows.Count - 1
                                            If DtRC.Rows(cc).Item("cod_contatto") = mag.primaryKey.centroAziendalePK.partitaIva Then
                                                TerzistaPresente = True
                                                Exit For
                                            End If
                                        Next
                                    Next
                                End If
                        End Select
                    Next
                    If Not TerzistaPresente Then
                        Dim messaggio As String = String.Format(RisorseProvenientiDaMagazzinoNecessarioSelezionareTerzista, mag.descrizione) & NEWLINE
                        lista_Errori.Add(generaErroreGias(ErroreGias_Severity.Bloccante, currentAttivitaDes, messaggio, Key_Parametri_Aggiuntivi_Attivita.mostraWarning_CheckMagazzino))
                    End If
                End If
            Next

        Catch ex As GiasException
            Throw ex
        Catch ex As Exception
            Throw New Exception(currentAttivitaDes, ex)
        End Try

    End Sub

    Private Shared Sub ControllaGiacenzePerData(ByRef lista_Errori As List(Of ErroreGias),
                                                severity As Integer,
                                                mostraWarning_CheckMagazzino As Boolean,
                                                agenda As Operazione_Agenda,
                                                Piva_Magazzino As String,
                                                Sa_Cod_Magazzino As Integer,
                                                Id_Destinazione_Magazzino As Integer,
                                                descrizioneFabbricato As String,
                                                Elem_Cod As Integer,
                                                Pro_Cod As Integer,
                                                Mat_Cod As Integer,
                                                Cod_Progetto As Integer,
                                                Fase_Cod As Integer,
                                                Lotto As String,
                                                Cal_Cod As Integer,
                                                Udm_Cod As Integer,
                                                Dal_Data_Verifica As Date,
                                                Al_Data_Verifica As Date,
                                                QtaTot As Decimal,
                                                DescrizioneProdotto As String,
                                                DescrizioneUnitaMisura As String,
                                                ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                Optional ByVal conRaccoglitore As Boolean = False
                                                )

        Dim dtMovimenti As DataTable
        Dim objMovimenti As New AgronicaCoreContabDAL.Movimenti_R
        Dim objMovDet As New AgronicaCoreContabDAL.Movimenti_Dettagli_R

        dtMovimenti = objMovimenti.SchedaMovimentiMagazzino(CDate(Dal_Data_Verifica), CDate(Al_Data_Verifica),
                                                            Piva_Magazzino, CInt(Sa_Cod_Magazzino),
                                                            Id_Destinazione_Magazzino,
                                                            CInt(Elem_Cod), CInt(Pro_Cod),
                                                            CInt(Mat_Cod), CInt(Cal_Cod),
                                                            CInt(Cod_Progetto), Fase_Cod,
                                                            CInt(Udm_Cod), Lotto,
                                                            "", "",
                                                            "", "",
                                                            "", "",
                                                            "", "",
                                                            "", "",
                                                            "",
                                                            "Movimenti_dettagli.Elem_Cod, Movimenti_dettagli.Pro_Cod, Movimenti_dettagli.Mat_Cod, Movimenti_dettagli.Cod_Progetto, Movimenti_dettagli.Lotto, Movimenti_dettagli.Cal_Cod, Movimenti_dettagli.Udm_Cod, Movimenti.Data_Movimento",
                                                            objParametri_Server, objParametri_Utenti,
                                                            flagRecuperaCodArticolo:=True,
                                                            codArticolo:="",
                                                            cercaCodArticoloPerLike:=True)

        For Each movimento_row In dtMovimenti.Rows

            Dim Qta_In_Data = objMovDet.Verifica_Giacenze_Con_Magazzino_Esterno(Piva_Magazzino, Sa_Cod_Magazzino,
                                                                                Id_Destinazione_Magazzino,
                                                                                Elem_Cod, Pro_Cod, Mat_Cod,
                                                                                0, 0, Lotto,
                                                                                0, CInt(Udm_Cod),
                                                                                AGRODATAINIZIO, movimento_row("Data_Movimento"),
                                                                                agenda.Piva, agenda.Sa_Cod, agenda.Id_Agenda,
                                                                                objParametri_Server, conRaccoglitore:=conRaccoglitore)

            If QtaTot > Math.Round(Qta_In_Data, 4) Then
                If Not (severity = ErroreGias_Severity.Warning AndAlso mostraWarning_CheckMagazzino = False) Then
                    Dim messaggio As String = String.Format(QuantitaNonSuffScaricoPerOperazione, DescrizioneProdotto, descrizioneFabbricato, agenda.Data.ToShortDateString, Qta_In_Data, DescrizioneUnitaMisura, movimento_row("Des_Lib"), CDate(movimento_row("Data_Movimento")).ToShortDateString)
                    lista_Errori.Add(generaErroreGias(severity, "", messaggio, Key_Parametri_Aggiuntivi_Attivita.mostraWarning_CheckMagazzino))
                End If
                Exit Sub
            End If

        Next
    End Sub

    Private Shared Function isMagazzinoGerarchico(PivaAttivita As String,
                                                 PivaMagazzino As String,
                                           objParametri_Server As AgronicaCoreParametri,
                                           objParametri_Utenti As AgronicaCoreParametri
                                           ) As Boolean

        Dim magazzinoGerarchico As Boolean = False

        Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read()
        Dim Impostazione_Mag As String = objUtenti.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.UTENTE_GESTIONE_MAGAZZINO_2, objParametri_Utenti, 1)
        'Dim piva As String = objParametriAgenda.Fabbricato.Split("|")(2)



        If (Impostazione_Mag = 1) Then

            If (PivaMagazzino = objParametri_Server.PivaSuperUser) Then
                magazzinoGerarchico = True
            End If

        ElseIf (Impostazione_Mag = 2) Then

            Dim objGerarchia As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R
            Dim Str_Pive As String = objGerarchia.Ricava_Stringa_PivePadre(PivaAttivita, "", objParametri_Server)

            Dim listaPive As String() = Str_Pive.Replace("(", "").Replace(")", "").Replace("'", "").Split(",")

            If listaPive.Contains(PivaMagazzino) Then
                magazzinoGerarchico = True
            End If
        End If

        Return magazzinoGerarchico

    End Function

    Private Shared Sub verificaConformitaMagazzini(MagazzinoList As List(Of Fabbricato),
                                                   PivaAttivita As String,
                                                   currentAttivitaDes As String,
                                                   objParametri_Server As AgronicaCoreParametri,
                                                   objParametri_Utenti As AgronicaCoreParametri,
                                                   ByRef lista_Errori As List(Of ErroreGias)
                                                   )



        Dim conformi As Boolean = True

        Dim magazzinoTerzista As Boolean = False
        Dim magazzinoGerarchico As Boolean = False
        Dim PivaMagazzino As String = ""

        For Each mag In MagazzinoList
            PivaMagazzino = mag.primaryKey.centroAziendalePK.partitaIva

            If PivaMagazzino = PivaAttivita AndAlso (magazzinoGerarchico OrElse magazzinoTerzista) Then
                Dim messaggio As String = SceltaMagazziniNonConforme
                lista_Errori.Add(generaErroreGias(ErroreGias_Severity.Bloccante, currentAttivitaDes, messaggio, Key_Parametri_Aggiuntivi_Attivita.mostraWarning_CheckMagazzino))

                Exit Sub
            End If

            If PivaMagazzino <> PivaAttivita Then
                If isMagazzinoGerarchico(PivaAttivita, PivaMagazzino, objParametri_Server, objParametri_Utenti) Then
                    magazzinoGerarchico = True
                Else
                    magazzinoTerzista = True
                End If
            End If
        Next


        'Dim conformi As Boolean = True
        'Dim Dt_Dosi As New DataTable
        'Dt_Dosi = Session("vs_dtDosi")

        'Dim magazzinoTerzista As Boolean = False
        'Dim magazzinoGerarchico As Boolean = False
        'Dim PivaMagazzino As String = ""

        'For Each rowDosi In Dt_Dosi.Rows
        '    PivaMagazzino = rowDosi("Piva")
        '    If rowDosi("Piva") = objParametriAgenda.Piva And (magazzinoGerarchico Or magazzinoTerzista) Then
        '        Throw New Exception("Scelta magazzini non conforme")
        '    End If

        '    If rowDosi("Piva") <> objParametriAgenda.Piva Then

        '        If isMagazzinoGerarchico(rowDosi("Piva")) Then
        '            magazzinoGerarchico = True
        '        Else
        '            magazzinoTerzista = True
        '        End If

        '    End If

        'Next

        'Dim PivaObjParametri = ""
        'If objParametriAgenda.Fabbricato <> "" AndAlso objParametriAgenda.Fabbricato <> "0" Then
        '    PivaObjParametri = objParametriAgenda.Fabbricato.Split("|")(2)
        'End If

        'If PivaMagazzino <> PivaObjParametri Then
        '    Throw New Exception("Scelta magazzini non conforme")
        'End If

        'Return conformi

    End Sub

    Private Shared Function sommatoriaAttivita_xElemCod_MagazziniEsterni(lista_Attivita As List(Of AgronicaCoreModelsSTD.attivita.Attivita),
                                                        objParametri_Super_Server As AgronicaCoreParametri,
                                                        objParametri_Server As AgronicaCoreParametri,
                                                        objParametri_Utenti As AgronicaCoreParametri
                                                        ) As Dictionary(Of Integer, Operazione_Agenda)


        Dim Elem_Cod As Integer = 0
        Dim classType As String = ""
        Dim risorseAttivita As List(Of risorse.RisorsaProdotto) = Nothing

        Dim dic_AttivitaxElem_Cod As New Dictionary(Of Integer, AgronicaCoreModelsSTD.attivita.Attivita)

        For Each attivita In lista_Attivita

            Dim InfoOperazione As InfoOperazione = GetInfoOperazione(attivita.job.primaryKey.codice, Tipo_Attivita.QuadernoDiCampagna)

            If InfoOperazione.IsTrattamento AndAlso Not (isInsettiUtiliTrappole(InfoOperazione)) Then
                Elem_Cod = FORMULATI
                classType = costanti.ClassType.DettaglioTrattamento
            End If
            If InfoOperazione.IsTrattamento AndAlso isInsettiUtiliTrappole(InfoOperazione) Then
                Elem_Cod = INSETTI
                classType = costanti.ClassType.DettaglioTrattamento
            End If
            If InfoOperazione.IsFertilizzazione Then
                Elem_Cod = FERTILIZZANTI
                classType = costanti.ClassType.DettaglioFertilizzazione
            End If
            If InfoOperazione.IsRaccolta Then
                Elem_Cod = TRASFORMATI_VEGETALI
                classType = costanti.ClassType.DettaglioRaccolta
            End If
            If InfoOperazione.IsSemina Then
                Elem_Cod = SEMENTI
                classType = costanti.ClassType.DettaglioSemina
            End If

            If Not dic_AttivitaxElem_Cod.ContainsKey(Elem_Cod) Then
                Dim cloneAttivita As Attivita = attivita.Clona
                cloneAttivita.risorse = New List(Of risorse.Risorsa)
                dic_AttivitaxElem_Cod.Add(Elem_Cod, cloneAttivita)
            End If

            Dim risorseToAdd = New List(Of risorse.Risorsa)
            'esclusione risorse con magazzini esterni (perchè sono per definizione sempre coperte a livello di giacenza, avendo carico e scarico su magazzini interno fatto sempre a fronte dello scarico da magazzino esterno
            For Each risorsa As RisorsaProdotto In attivita.risorse.FindAll(Function(c) (c.classType = classType))
                Dim risorsaToAdd As Boolean = True
                If risorsa.MagazziniMovimentazioni IsNot Nothing AndAlso risorsa.MagazziniMovimentazioni.Count > 0 Then
                    For Each rilevamentoMagazzino In risorsa.MagazziniMovimentazioni
                        If rilevamentoMagazzino.registrazioniCollegate IsNot Nothing AndAlso rilevamentoMagazzino.registrazioniCollegate.Count > 0 Then
                            risorsaToAdd = False

                            For Each registrazioneCollegata In rilevamentoMagazzino.registrazioniCollegate
                                If registrazioneCollegata.job.primaryKey.codice = LAVCOD_SCARICO AndAlso registrazioneCollegata.risorse IsNot Nothing AndAlso registrazioneCollegata.risorse.Count > 0 Then
                                    Dim risorsaRegistrazione = risorsa.Clona
                                    risorsaRegistrazione.MagazziniMovimentazioni = CType(registrazioneCollegata.risorse(0), RisorsaRegistrazione).MagazziniMovimentazioni
                                    risorseToAdd.Add(risorsaRegistrazione)
                                End If
                            Next

                        End If
                    Next
                End If
                If risorsaToAdd Then
                    risorseToAdd.Add(risorsa)
                End If
            Next

            dic_AttivitaxElem_Cod(Elem_Cod).risorse.AddRange(risorseToAdd)

        Next

        'Converto le nuove Attività in Agende
        Dim dic_AgendexElemCod As New Dictionary(Of Integer, Operazione_Agenda)
        Dim mapper As New AgronicaCoreMapper.AttivitaToAgenda
        For Each dictAttivita In dic_AttivitaxElem_Cod
            Dim agenda = mapper.MappaAttivitaToAgenda(dictAttivita.Value, objParametri_Super_Server, objParametri_Server, objParametri_Utenti, False)
            dic_AgendexElemCod.Add(dictAttivita.Key, agenda)
        Next

        Return dic_AgendexElemCod

    End Function


    Public Sub CheckDPI_listaAttivita(
        lista_Attivita As List(Of Attivita),
        listaAttivitaAgende As List(Of (attivita As Attivita, agenda As Operazione_Agenda)),
        ByRef listaErrori As List(Of ErroreGias),
        mostraWarning_ControlloDPI As Boolean,
        objParametri_Super_Server As AgronicaCoreParametri,
        objParametri_Server As AgronicaCoreParametri,
        objParametri_Utenti As AgronicaCoreParametri,
        bearerToken As String,
        ByRef complianceResultStatus As ComplianceResultStatus,
        ByVal idTestata As Integer
    )
        Dim p As New ObjParams With {
            .ObjParametri_SuperServer = objParametri_Super_Server,
            .ObjParametri_Server = objParametri_Server,
            .ObjParametri_Utenti = objParametri_Utenti
        }

        'Se esistono più attività devo:
        'accorpare i prodotti ---COMMENTATO PER ORA,
        'ricalcolare le dosi di CU per il CheckDPI
        If lista_Attivita.Count > 1 Then
            Dim paramVerificaDPIMultiAttivita As New VerificaDPIMultiAttivita
            'listaAttivitaAgende = sommatoriaProdottixVerificaDPI(lista_Attivita, paramVerificaDPIMultiAttivita, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
            estraiRameOperazioni(lista_Attivita, paramVerificaDPIMultiAttivita)
        End If

        Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim valoreImpostazione = CInt(objUtentiImpostazioni.LeggiValoreImpostazioneScalare(
            enum_Impostazioni_Utenti.SUPERUSER_ModalitaVerificaConformita,
            objParametri_Utenti.UtenteUsername,
            objParametri_Utenti))
        Dim useNewApi = (valoreImpostazione = 2)

        Try
            If useNewApi Then
                CheckActivitiesComplianceNew(listaAttivitaAgende, mostraWarning_ControlloDPI, idTestata, bearerToken, complianceResultStatus, listaErrori, p)
            Else
                CheckActivitiesCompliance(listaAttivitaAgende, mostraWarning_ControlloDPI, listaErrori, p)
            End If
        Catch ex As GiasException
            Throw ex
        End Try
    End Sub

    Private Function GetActivityListForCheckDPI(
        listaAttivitaAgende As List(Of (attivita As Attivita, agenda As Operazione_Agenda)),
        p As ObjParams
    ) As ComplianceAnalysisDataParms
        Dim severityErroreTrattamenti As Integer
        Dim valoreImpostazioneBloccoSalvaNoConformeTrattamenti As Integer
        getSeverityDaValoreImpostazione_0NessunBlocco_1WarningBloccante_2Warning(
            severityErroreTrattamenti, enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME,
            p.ObjParametri_Utenti, valoreImpostazioneBloccoSalvaNoConformeTrattamenti)

        Dim severityErroreFertilizzazioni As Integer
        Dim valoreImpostazioneBloccoSalvaNoConformeFertilizzazioni As Integer
        getSeverityDaValoreImpostazione_0NessunBlocco_1WarningBloccante_2Warning(
            severityErroreFertilizzazioni, enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_BLOCCO_SALVA_FERTILIZZAZIONI,
            p.ObjParametri_Utenti, valoreImpostazioneBloccoSalvaNoConformeFertilizzazioni)

        Dim severityErroreRaccolte As Integer
        getSeverityDaValoreImpostazione_0Warning_1WarningBloccante(
            severityErroreRaccolte, enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_RACCOLTA_CARENZA_NON_RISPETTATA,
            p.ObjParametri_Utenti)

        Dim listaAttivitaOggettoDiVerifica As New List(Of Attivita)
        Dim listaSeverityAttivitaOggettoDiVerifica As New List(Of Integer)

        For Each item In listaAttivitaAgende
            'eseguo la verifica conformità se esiste almeno un'attività che lo necessita
            Dim InfoOperazione As InfoOperazione = GetInfoOperazione(item.attivita.job.primaryKey.codice, Tipo_Attivita.QuadernoDiCampagna)
            Dim operazioneCheNecessitaVerifica = eseguiCheckDPI(InfoOperazione, item.attivita)

            If operazioneCheNecessitaVerifica Then

                If InfoOperazione.IsTrattamento Then
                    If valoreImpostazioneBloccoSalvaNoConformeTrattamenti <> 0 Then
                        listaAttivitaOggettoDiVerifica.Add(item.attivita)
                        listaSeverityAttivitaOggettoDiVerifica.Add(severityErroreTrattamenti)
                    End If
                ElseIf InfoOperazione.IsFertilizzazione Then
                    If valoreImpostazioneBloccoSalvaNoConformeFertilizzazioni <> 0 Then
                        listaAttivitaOggettoDiVerifica.Add(item.attivita)
                        listaSeverityAttivitaOggettoDiVerifica.Add(severityErroreFertilizzazioni)
                    End If
                ElseIf InfoOperazione.IsRaccolta Then
                    listaSeverityAttivitaOggettoDiVerifica.Add(severityErroreRaccolte)
                    listaAttivitaOggettoDiVerifica.Add(item.attivita)
                End If

            End If
        Next

        Return New ComplianceAnalysisDataParms(
            severityErroreRaccolte, severityErroreFertilizzazioni, severityErroreTrattamenti,
            listaAttivitaOggettoDiVerifica, listaSeverityAttivitaOggettoDiVerifica)
    End Function

    Private Sub SendActivityForCheckCompliance(
        attivitaPerVerifica As AttivitaPerVerifica,
        bearerToken As String,
        ByRef complianceResultStatus As ComplianceResultStatus,
        ByRef listaErrori As List(Of ErroreGias),
        p As ObjParams
    )
        Dim client As New Http.HttpClient()
        Try

#If DEBUG Then
            ServicePointManager.ServerCertificateValidationCallback = Function(s, c, h, e) True
#End If

            Dim payloadAsString = JsonConvert.SerializeObject(attivitaPerVerifica)
            Dim content As New Http.StringContent(payloadAsString, Text.Encoding.UTF8, "application/json")
            client.DefaultRequestHeaders.Authorization = New Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken)

            Dim configurazioniSitiRead As New Configurazione_Siti_R
            Dim qdcaComplianceUri = configurazioniSitiRead.Leggi_Valore(0, "GiasOnline_QdCACompliance_API", "", "", p.ObjParametri_Server)
            Dim uri = qdcaComplianceUri & "/RichiesteVerifica/SendAttivita"

            Dim utility As New AgronicaCoreUtility.CallNetCore()
            utility.AggiustaUrl(uri, p.ObjParametri_Server)

            Dim hr As Http.HttpResponseMessage = client.PostAsync(uri, content).GetAwaiter().GetResult()
            hr.EnsureSuccessStatusCode()
            Dim response = hr.Content.ReadAsStringAsync().Result

            complianceResultStatus.ComplianceResponse = JsonConvert.DeserializeObject(Of ComplianceResponse)(response)

        Catch ex As Exception
            listaErrori.Add(generaErroreGias(ErroreGias_Severity.Bloccante, "", "Richiesta di verifica conformità non riuscita", Key_Parametri_Aggiuntivi_Attivita.mostraWarning_CheckDPI))
        Finally
            client.Dispose()
        End Try
    End Sub

    Private Sub HandleComplianceAnalysisResults(
        dt As DataTable,
        analysisDataParams As ComplianceAnalysisDataParms,
        attivitaPerVerifica As AttivitaPerVerifica,
        showWarning As Boolean,
        ByRef currentAttivitaDes As String,
        ByRef complianceResultStatus As ComplianceResultStatus,
        ByRef listaErrori As List(Of ErroreGias),
        p As ObjParams
    )
        complianceResultStatus.Timeout = False

        Dim utentiImpostazioniR As New Utenti_Impostazioni_Read
        Dim impostazioniUtentePerControlliBloccanti = utentiImpostazioniR.LeggiImpostazioniUtentePerControlliBloccanti(p.ObjParametri_Utenti)

        For Each attivita As Attivita In attivitaPerVerifica.AttivitaOggettoDiVerifica
            Dim controlliNonConformi = dt.AsEnumerable().Where(Function(dr) CInt(dr("Conforme")) = 0 AndAlso CInt(dr("Id_Agenda")) = CInt(attivita.codicePerVerificaConformita)).ToList()
            If Not controlliNonConformi.Any() Then
                Continue For
            End If

            Dim infoOperazione As InfoOperazione = GetInfoOperazione(attivita.job.primaryKey.codice, Tipo_Attivita.QuadernoDiCampagna)
            currentAttivitaDes = attivita.job.descrizione

            Dim severity As Integer
            If infoOperazione.IsTrattamento Then
                severity = analysisDataParams.TreatmentErrorSeverity
            ElseIf infoOperazione.IsFertilizzazione Then
                severity = analysisDataParams.FertilizationErrorSeverity
            ElseIf infoOperazione.IsRaccolta Then
                severity = analysisDataParams.HarvestErrorSeverity
            End If

            Dim aggiungiErrori As Boolean = Not (severity = ErroreGias_Severity.Warning AndAlso Not showWarning)
            If aggiungiErrori Then
                For Each controlloNonConforme As DataRow In controlliNonConformi

                    Dim errore = CStr(controlloNonConforme("Dettagli"))
                    errore = Utility_Agenda.rimuoviDuplicatiMessaggioErroreVerificaDPI(errore)
                    Dim codiceErrore = CInt(controlloNonConforme("Err_Code"))

                    If infoOperazione.IsTrattamento Then
                        If MustAddErrorForTrattamento(impostazioniUtentePerControlliBloccanti, codiceErrore, errore) Then
                            listaErrori.Add(generaErroreGias(severity, currentAttivitaDes, errore, Key_Parametri_Aggiuntivi_Attivita.mostraWarning_CheckDPI))
                        End If

                    ElseIf infoOperazione.IsFertilizzazione Then
                        If MustAddErrorForFertilizzazione(impostazioniUtentePerControlliBloccanti, codiceErrore) Then
                            listaErrori.Add(generaErroreGias(severity, currentAttivitaDes, errore, Key_Parametri_Aggiuntivi_Attivita.mostraWarning_CheckDPI))
                        End If

                    Else
                        listaErrori.Add(generaErroreGias(severity, currentAttivitaDes, errore, Key_Parametri_Aggiuntivi_Attivita.mostraWarning_CheckDPI))
                    End If

                Next
            End If
        Next
    End Sub

    Private Sub CheckComplianceRequestResults(
        analysisDataParams As ComplianceAnalysisDataParms,
        attivitaPerVerifica As AttivitaPerVerifica,
        showWarning As Boolean,
        ByRef complianceResultStatus As ComplianceResultStatus,
        ByRef listaErrori As List(Of ErroreGias),
        p As ObjParams
    )
        Dim verificheEsitiRead As New VerificheEsitiReaderWithPolling(p.ObjParametri_Server)
        Dim dt = verificheEsitiRead.LeggiRisultatiVerificaConformita(complianceResultStatus.ComplianceResponse.IdTestata, timeoutInSecondi:=60)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            complianceResultStatus.Timeout = True
            If analysisDataParams.ActivitySeverity.Contains(CInt(ErroreGias_Severity.WarningBloccante)) Then
                listaErrori.Add(generaErroreGias(
                    ErroreGias_Severity.WarningBloccante, "",
                    VerificaConformitaNonCompletataOperazioneNonSalvata,
                    Key_Parametri_Aggiuntivi_Attivita.mostraWarning_CheckDPI
                ))
            ElseIf analysisDataParams.ActivitySeverity.Contains(CInt(ErroreGias_Severity.Warning)) Then
                listaErrori.Add(generaErroreGias(
                    ErroreGias_Severity.Warning, "",
                    VerificaConformitaNonCompletata & " " & SeSalvataggioVerificaConformitaNonVerraEffettuata,
                    Key_Parametri_Aggiuntivi_Attivita.mostraWarning_CheckDPI
                ))
            End If

        Else
            Dim currentAttivitaDes = ""
            Try
                HandleComplianceAnalysisResults(
                    dt, analysisDataParams, attivitaPerVerifica, showWarning,
                    currentAttivitaDes, complianceResultStatus, listaErrori, p)
            Catch ex As Exception
                Throw New InvalidOperationException(currentAttivitaDes, ex)
            End Try
        End If
    End Sub

    Private Function GetComplianceResultStatus(
        idTestata As Integer,
        attivitaPerVerifica As AttivitaPerVerifica,
        bearerToken As String,
        ByRef listaErrori As List(Of ErroreGias),
        p As ObjParams
    ) As ComplianceResultStatus
        Dim complianceResultStatus As New ComplianceResultStatus
        If idTestata = 0 Then
            'richiesta di verifica conformità effettuata per la prima volta
            SendActivityForCheckCompliance(attivitaPerVerifica, bearerToken, complianceResultStatus, listaErrori, p)

        Else
            'la richiesta di verifica conformità è stata effettuata nelle chiamate precedenti e l'utente ha scelto di aspettare
            Dim verificheEsitiRead As New VerificheEsitiReaderWithPolling(p.ObjParametri_Server)
            complianceResultStatus.ComplianceResponse = New ComplianceResponse
            complianceResultStatus.ComplianceResponse.IdTestata = idTestata

            Dim dt = verificheEsitiRead.LeggiStatoRichiesta(complianceResultStatus.ComplianceResponse.IdTestata, timeoutInSecondi:=60)
            If dt IsNot Nothing AndAlso dt.Rows.Count = 1 AndAlso dt.Rows(0).Field(Of Integer)("status") = -1 Then
                complianceResultStatus.ComplianceResponse.Error = dt.Rows(0).Field(Of String)("errore")
            End If
        End If

        Return complianceResultStatus
    End Function

    Private Sub CheckActivitiesComplianceNew(
        listaAttivitaAgende As List(Of (attivita As Attivita, agenda As Operazione_Agenda)),
        showWarning As Boolean,
        idTestata As Integer,
        bearerToken As String,
        ByRef complianceResultStatus As ComplianceResultStatus,
        ByRef listaErrori As List(Of ErroreGias),
        p As ObjParams
    )
        Dim analysisDataParams = GetActivityListForCheckDPI(listaAttivitaAgende, p)
        If Not analysisDataParams.ActivityList.Any() Then
            Return
        End If

        Dim listaAttivita = listaAttivitaAgende.Select(Function(a) a.attivita).ToList()
        Dim operazioneNuova As Boolean = listaAttivita(0).codice = "0" 'codice = "0" significa operazione nuova
        If operazioneNuova Then
            Dim codicePerVerificaConformita As Integer = -1

            For Each attivita As Attivita In listaAttivita
                attivita.codicePerVerificaConformita = CStr(codicePerVerificaConformita)
                codicePerVerificaConformita = codicePerVerificaConformita - 1
            Next
        Else
            For Each attivita As Attivita In listaAttivita
                attivita.codicePerVerificaConformita = attivita.codice
            Next
        End If

        If Not showWarning AndAlso Not analysisDataParams.ActivitySeverity.Contains(CInt(ErroreGias_Severity.WarningBloccante)) Then
            Return
        End If

        Dim attivitaPerVerifica As New AttivitaPerVerifica
        attivitaPerVerifica.AttivitaOggettoDiVerifica = analysisDataParams.ActivityList
        attivitaPerVerifica.AttivitaNonOggettoDiVerifica = listaAttivita.Where(Function(a) Not analysisDataParams.ActivityList.Contains(a)).ToList()

        complianceResultStatus = GetComplianceResultStatus(idTestata, attivitaPerVerifica, bearerToken, listaErrori, p)
        If (listaErrori.Any(Function(e) e.severity = ErroreGias_Severity.Bloccante)) Then
            Return
        End If

        CheckComplianceRequestResults(analysisDataParams, attivitaPerVerifica, showWarning, complianceResultStatus, listaErrori, p)
    End Sub

    Private Sub CheckActivitiesCompliance(
        listaAttivitaAgende As List(Of (attivita As Attivita, agenda As Operazione_Agenda)),
        mostraWarning_ControlloDPI As Boolean,
        ByRef listaErrori As List(Of ErroreGias),
        p As ObjParams
    )
        Dim paramVerificaDPIMultiAttivita As New VerificaDPIMultiAttivita
        For Each item In listaAttivitaAgende
            Dim currentAttivitaDes = item.attivita.job.descrizione
            Dim InfoOperazione As InfoOperazione = GetInfoOperazione(item.attivita.job.primaryKey.codice, Tipo_Attivita.QuadernoDiCampagna)
            Try
                If eseguiCheckDPI(InfoOperazione, item.attivita) Then
                    CheckDPI(item.agenda, InfoOperazione, currentAttivitaDes, mostraWarning_ControlloDPI,
                             p.ObjParametri_SuperServer, p.ObjParametri_Server, p.ObjParametri_Utenti,
                             listaErrori, paramVerificaDPIMultiAttivita)

                    If listaErrori.FindAll(Function(c) (c.severity = ErroreGias_Severity.Bloccante)).Count > 0 Then
                        'alla prima occorrenza di errore bloccante esco
                        Return
                    End If
                End If
            Catch ex As Exception
                Throw New InvalidOperationException(currentAttivitaDes, ex)
            End Try
        Next
    End Sub

    Private Function MustAddErrorForFertilizzazione(ByVal impostazioni As ImpostazioniUtentePerControlliBloccanti, Err_Code As enTipoErrCode_Verifica) As Boolean
        Dim InserisciErrore = True

        Select Case impostazioni.UTENTE_COD_DEFAULT_BLOCCO_SALVA_FERTILIZZAZIONI

            Case "0" 'nessun blocco
                InserisciErrore = False
            Case "1", "2" 'blocco, warning (verifica singole impostazioni)
                Select Case Err_Code
                    Case enTipoErrCode_Verifica.ProdottoNonBiologico
                        If impostazioni.UTENTE_COD_BLOCCA_PRODOTTO_NON_BIO_REGOLAMENTO_BIO_FERTI <> "1" Then
                            InserisciErrore = False
                        End If
                    Case enTipoErrCode_Verifica.Superato_N_Max,
                         enTipoErrCode_Verifica.Superato_P_Max,
                         enTipoErrCode_Verifica.Superato_K_Max,
                         enTipoErrCode_Verifica.Superato_M_Max
                        If impostazioni.UTENTE_COD_BLOCCA_SE_SUPERA_LIMITIMAS <> "1" Then
                            InserisciErrore = False
                        End If
                    Case enTipoErrCode_Verifica.Superato_N_Max_Intervento
                        If impostazioni.UTENTE_COD_BLOCCA_MASSIMALE_DISTRIBUZIONE_N_DPI_FERTI <> "1" Then
                            InserisciErrore = False
                        End If
                    Case enTipoErrCode_Verifica.DoseEccessivaRame
                        If impostazioni.UTENTE_COD_BLOCCA_DOSERAMEANNOMAX_FERTI <> "1" Then
                            InserisciErrore = False
                        End If
                    Case enTipoErrCode_Verifica.DoseEccessivaRame5Anni
                        If impostazioni.UTENTE_COD_BLOCCA_DOSERAME7ANNIMAX_FERTI <> "1" Then
                            InserisciErrore = False
                        End If
                End Select

        End Select

        Return InserisciErrore
    End Function

    Private Function MustAddErrorForTrattamento(ByVal impostazioni As ImpostazioniUtentePerControlliBloccanti, Err_Code As enTipoErrCode_Verifica, Err_Des As String) As Boolean

        Dim InserisciErrore = True

        Select Case impostazioni.UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME

            Case "0" ', "ND" 'nessun blocco
                InserisciErrore = False

            Case "1", "2" 'blocco, warning (verifica singole impostazioni)

                Select Case Err_Code

                    Case enTipoErrCode_Verifica.AvversitaNonGiustificataDPI
                        If impostazioni.UTENTE_COD_BLOCCA_AVVERSITANODPI_SALVATAGGIO <> "1" Then
                            InserisciErrore = False
                        End If
                    Case enTipoErrCode_Verifica.ProdottoNonGiustificatoSuAvversita
                        If impostazioni.UTENTE_COD_BLOCCA_PRODOTTONOAVVERSITA_SALVATAGGIO <> "1" Then
                            InserisciErrore = False
                        End If
                    Case enTipoErrCode_Verifica.DoseNonDisponibile
                        If impostazioni.UTENTE_COD_BLOCCA_DOSENONDISPONIBILE_SALVATAGGIO <> "1" Then
                            InserisciErrore = False
                        End If
                    Case enTipoErrCode_Verifica.UnitaMisuraNonCompatibile
                        If impostazioni.UTENTE_COD_BLOCCA_DOSEUDMNONCONFORME_SALVATAGGIO <> "1" Then
                            InserisciErrore = False
                        End If
                    Case enTipoErrCode_Verifica.DoseEccessiva
                        If impostazioni.UTENTE_COD_BLOCCA_DOSEMASSIMA_ETICHETTA_SALVATAGGIO <> "1" Then
                            InserisciErrore = False
                        End If
                    Case enTipoErrCode_Verifica.DoseInsufficiente
                        If impostazioni.UTENTE_COD_BLOCCA_DOSEMINIMA_ETICHETTA_SALVATAGGIO <> "1" Then
                            InserisciErrore = False
                        End If
                    Case enTipoErrCode_Verifica.AcquaNonCorretta
                        'acqua superiore
                        If InStr(Err_Des, "superiore") <> 0 Then
                            If impostazioni.UTENTE_COD_BLOCCA_ACQUAMASSIMA_ETICHETTA_SALVATAGGIO <> "1" Then
                                InserisciErrore = False
                            End If
                        End If
                        'acqua inferiore
                        If InStr(Err_Des, "inferiore") <> 0 Then
                            If impostazioni.UTENTE_COD_BLOCCA_ACQUAMINIMA_ETICHETTA_SALVATAGGIO <> "1" Then
                                InserisciErrore = False
                            End If
                        End If

                    Case enTipoErrCode_Verifica.DataInterventoMin
                        If impostazioni.UTENTE_COD_BLOCCA_DATADPIMIN_SALVATAGGIO <> "1" Then
                            InserisciErrore = False
                        End If

                    Case enTipoErrCode_Verifica.DataInterventoMax
                        If impostazioni.UTENTE_COD_BLOCCA_DATADPIMAX_SALVATAGGIO <> "1" Then
                            InserisciErrore = False
                        End If

                    Case enTipoErrCode_Verifica.ImpiantiNonCoerentiDPI,
                        enTipoErrCode_Verifica.ImpiantiRaggruppamentoColturaleNonOmogeneo,
                        enTipoErrCode_Verifica.ImpiantiCoperturaNonOmogeneo,
                        enTipoErrCode_Verifica.ImpiantiDPINonImpostato,
                        enTipoErrCode_Verifica.ImpiantiDpiNonOmogeneo
                        If impostazioni.UTENTE_COD_BLOCCA_AVVERSITANODPI_SALVATAGGIO <> "1" Then
                            InserisciErrore = False
                        End If

                    Case enTipoErrCode_Verifica.DoseDiserboEccessiva
                        If impostazioni.UTENTE_COD_BLOCCA_DOSEDPIDISERBOMAX_SALVATAGGIO <> "1" Then
                            InserisciErrore = False
                        End If

                    Case enTipoErrCode_Verifica.CarenzaNonRispettata
                        If impostazioni.UTENTE_COD_BLOCCA_CARENZA_NON_RISPETTATA_SALVATAGGIO <> "1" Then
                            InserisciErrore = False
                        End If

                    Case enTipoErrCode_Verifica.SuperatoVolumeMaxAcquaDpi
                        If impostazioni.UTENTE_COD_BLOCCA_ACQUAMASSIMA_DPI_SALVATAGGIO <> "1" Then
                            InserisciErrore = False
                        End If

                    Case enTipoErrCode_Verifica.Epoca_Etichetta
                        If impostazioni.UTENTE_COD_BLOCCA_FINO_FIORITURA_ETICHETTA_SALVATAGGIO <> "1" Then
                            InserisciErrore = False
                        End If

                    Case enTipoErrCode_Verifica.BufferZoneNonRispettata
                        If impostazioni.UTENTE_COD_BLOCCA_BUFFERZONE_ETICHETTA_SALVATAGGIO <> "1" Then
                            InserisciErrore = False
                        End If

                    Case enTipoErrCode_Verifica.DoseEccessivaEtichetta_Anno
                        If impostazioni.UTENTE_COD_BLOCCA_DOSEMASSIMA_ANNO_ETICHETTA_SALVATAGGIO <> "1" Then
                            InserisciErrore = False
                        End If

                    Case enTipoErrCode_Verifica.DoseEccessivaEtichettaAvversita_Anno
                        If impostazioni.UTENTE_COD_BLOCCA_DOSEMASSIMA_ANNO_ETICHETTA_SALVATAGGIO <> "1" Then
                            InserisciErrore = False
                        End If

                    Case enTipoErrCode_Verifica.ProdottoVincolatoFormulato
                        If impostazioni.UTENTE_COD_BLOCCA_PRODOTTO_RELAZIONE_FORMULATO_SALVATAGGIO <> "1" Then
                            InserisciErrore = False
                        End If

                    Case enTipoErrCode_Verifica.ProdottoNonUtilizzabileXDisciplinarexStatoImpianto
                        If impostazioni.UTENTE_COD_BLOCCA_PRODOTTO_ETA_IMPIANTO_SALVATAGGIO <> "1" Then
                            InserisciErrore = False
                        End If

                    Case enTipoErrCode_Verifica.MixPolveruentiENon
                        If impostazioni.UTENTE_COD_BLOCCA_MIX_POLVERULENTI_NONPOLVERULENTI_SALVATAGGIO <> "1" Then
                            InserisciErrore = False
                        End If
                    Case enTipoErrCode_Verifica.ProdottoNonUtilizzabileXData
                        If impostazioni.UTENTE_COD_BLOCCA_PRODOTTO_NON_UTILIZZABILE_X_DATA <> "1" Then
                            InserisciErrore = False
                        End If
                    Case enTipoErrCode_Verifica.ProdottoNonRegistratoSuColtura
                        If impostazioni.UTENTE_COD_BLOCCA_PRODOTTO_NON_REGISTRATO_SU_COLTURA <> "1" Then
                            InserisciErrore = False
                        End If
                    Case enTipoErrCode_Verifica.AvversitaNonTrattabileDPI
                        If impostazioni.UTENTE_COD_BLOCCA_AVVERSITA_NON_TRATTABILE_DPI <> "1" Then
                            InserisciErrore = False
                        End If
                    Case enTipoErrCode_Verifica.ProdottoNonGiustificatoSuAvversitaDPI
                        If impostazioni.UTENTE_COD_BLOCCA_PRODOTTO_NON_REGISTRATO_SU_AVVERSITA_DPI <> "1" Then
                            InserisciErrore = False
                        End If
                    Case enTipoErrCode_Verifica.ProdottoNonUtilizzabileXDisciplinare
                        If impostazioni.UTENTE_COD_BLOCCA_PRODOTTO_NON_UTILIZZABILE_DPI <> "1" Then
                            InserisciErrore = False
                        End If
                    Case enTipoErrCode_Verifica.InterventoNonConsentito
                        If impostazioni.UTENTE_COD_BLOCCA_INTERVENTO_NON_CONSENTITO <> "1" Then
                            InserisciErrore = False
                        End If
                    Case enTipoErrCode_Verifica.ProdottoNonBiologico
                        If impostazioni.UTENTE_COD_BLOCCA_PRODOTTO_NON_BIO_REGOLAMENTO_BIO_FITO <> "1" Then
                            InserisciErrore = False
                        End If
                End Select

        End Select

        Return InserisciErrore

    End Function


#Region "NON USATA PER ADESSO (sommatoria per il count utilizzi PA)"
    'Private Shared Function sommatoriaProdottixVerificaDPI(lista_Attivita As List(Of AgronicaCoreModelsSTD.attivita.Attivita),
    '                                                      ByRef paramVerificaDPIMultiAttivita As VerificaDPIMultiAttivita,
    '                                                      objParametri_Super_Server As AgronicaCoreParametri,
    '                                                      objParametri_Server As AgronicaCoreParametri,
    '                                                      objParametri_Utenti As AgronicaCoreParametri
    '                                                      ) As List(Of (attivita As Attivita, agenda As Operazione_Agenda))


    '    Dim classType As String = ""
    '    Dim risorseFertilizzazione As New List(Of risorse.Risorsa)
    '    Dim risorseTrattamento As New Dictionary(Of Integer, List(Of risorse.Risorsa))
    '    Dim principiAttivi As New List(Of PrincipioAttivo)

    '    Dim clone_ListaAttivita As New List(Of AgronicaCoreModelsSTD.attivita.Attivita)

    '    For Each attivita In lista_Attivita
    '        Dim InfoOperazione As InfoOperazione = GetInfoOperazione(attivita.job.primaryKey.codice, Tipo_Attivita.QuadernoDiCampagna)

    '        If InfoOperazione.IsFertilizzazione Then
    '            classType = costanti.ClassType.DettaglioFertilizzazione

    '            'TIRO FUORI TUTTI I PRODOTTI USATI NEI TRATTAMENTI
    '            risorseFertilizzazione.AddRange(attivita.risorse.FindAll(Function(c) c.classType = classType))

    '        End If

    '        If InfoOperazione.IsTrattamento Then
    '            classType = costanti.ClassType.DettaglioTrattamento

    '            'TIRO FUORI TUTTI I PRODOTTI USATI NEI TRATTAMENTI
    '            risorseTrattamento.Add(attivita.job.primaryKey.codice, attivita.risorse.FindAll(Function(c) c.classType = classType))

    '            'PER OGNI ATTIVITA' MI SALVO I PRODOTTI REALI SALVATI
    '            Dim listaProdotti As New List(Of Integer)
    '            For Each risorsa In attivita.risorse.FindAll(Function(c) c.classType = classType)
    '                Dim trattamento As dettagli.DettaglioTrattamento = CType(risorsa, dettagli.DettaglioTrattamento)
    '                listaProdotti.Add(trattamento.prodotto.codice)
    '            Next
    '            If listaProdotti.Count > 0 Then
    '                paramVerificaDPIMultiAttivita.dic_ProdottiRealixLavCod.Add(attivita.job.primaryKey.codice, listaProdotti)
    '            End If
    '        End If


    '        Dim cloneAttivita As Attivita = attivita.Clona
    '        'Dall'Attivita clonata rimuovo prima i prodotti, poi me la metto da parte
    '        cloneAttivita.risorse.RemoveAll(Function(c) c.classType = classType)
    '        clone_ListaAttivita.Add(cloneAttivita)
    '    Next

    '    If risorseTrattamento.Count > 0 Then
    '        paramVerificaDPIMultiAttivita.TotCUxHa_Trattamenti_MultiAttivita_xDistinta = estraiRameTrattamenti(risorseTrattamento, paramVerificaDPIMultiAttivita.dic_CUxHaTrattamenti_xDistintaxLavCod, lista_Attivita(0))
    '    End If
    '    If risorseFertilizzazione.Count > 0 Then
    '        paramVerificaDPIMultiAttivita.TotCUxHa_Fertilizzazione_MultiAttivita_xDistinta = estraiRameFertilizzazione(risorseFertilizzazione, lista_Attivita(0))
    '    End If


    '    'Converto le nuove Attività in Agende
    '    Dim listaAttivitaAgendexAllProducts As New List(Of (attivita As Attivita, agenda As Operazione_Agenda))
    '    Dim mapper As New AgronicaCoreMapper.AttivitaToAgenda
    '    For Each attivita_clone In clone_ListaAttivita
    '        Dim InfoOperazione As InfoOperazione = GetInfoOperazione(attivita_clone.job.primaryKey.codice, Tipo_Attivita.QuadernoDiCampagna)
    '        'REINSERISCO LE RISORSE NEI CLONI
    '        If InfoOperazione.IsTrattamento Then
    '            For Each risorsa In risorseTrattamento
    '                attivita_clone.risorse.AddRange(risorsa.Value)
    '            Next
    '        End If

    '        If InfoOperazione.IsFertilizzazione Then
    '            attivita_clone.risorse.AddRange(risorseFertilizzazione)
    '        End If

    '        'Genero le agende da passare al verifica DPI
    '        Dim agenda = mapper.MappaAttivitaToAgenda(attivita_clone, objParametri_Super_Server, objParametri_Server, objParametri_Utenti, False)
    '        listaAttivitaAgendexAllProducts.Add((attivita_clone, agenda))
    '    Next

    '    Return listaAttivitaAgendexAllProducts

    'End Function
#End Region

    Public Shared Sub estraiRameOperazioni(lista_Attivita As List(Of AgronicaCoreModelsSTD.attivita.Attivita),
                                            ByRef paramVerificaDPIMultiAttivita As VerificaDPIMultiAttivita)

        Dim risorseFertilizzazione As New List(Of risorse.Risorsa)
        Dim risorseTrattamento As New Dictionary(Of Integer, List(Of risorse.Risorsa))
        Dim classType As String = ""

        For Each attivita In lista_Attivita
            Dim InfoOperazione As InfoOperazione = GetInfoOperazione(attivita.job.primaryKey.codice, Tipo_Attivita.QuadernoDiCampagna)

            If InfoOperazione.IsFertilizzazione Then
                classType = costanti.ClassType.DettaglioFertilizzazione

                'TIRO FUORI TUTTI I PRODOTTI USATI NELL'UNICA FERTILIZZAZIONE
                If risorseFertilizzazione.Count = 0 Then
                    risorseFertilizzazione.AddRange(attivita.risorse.FindAll(Function(c) c.classType = classType))
                End If

            End If

            If InfoOperazione.IsTrattamento AndAlso Not isInsettiUtiliTrappole(InfoOperazione) Then
                classType = costanti.ClassType.DettaglioTrattamento
                'TIRO FUORI TUTTI I PRODOTTI USATI NEI TRATTAMENTI
                If Not risorseTrattamento.ContainsKey(attivita.job.primaryKey.codice) Then
                    risorseTrattamento.Add(attivita.job.primaryKey.codice, attivita.risorse.FindAll(Function(c) c.classType = classType))
                End If
            End If
        Next

        If risorseTrattamento.Count > 0 Then
            paramVerificaDPIMultiAttivita.TotCUxHa_Trattamenti_MultiAttivita_xDistinta = estraiRameTrattamenti(risorseTrattamento, paramVerificaDPIMultiAttivita.dic_CUxHaTrattamenti_xDistintaxLavCod, lista_Attivita(0))
        End If
        If risorseFertilizzazione.Count > 0 Then
            paramVerificaDPIMultiAttivita.TotCUxHa_Fertilizzazione_MultiAttivita_xDistinta = estraiRameFertilizzazione(risorseFertilizzazione, lista_Attivita(0))
        End If

    End Sub

    Private Shared Function estraiRameTrattamenti(ByRef dic_risorseTrattamento As Dictionary(Of Integer, List(Of risorse.Risorsa)),
                                                  ByRef dic_CUxHATrattamenti_xDistintaxLavCod As Dictionary(Of ((String, Integer, Integer, Integer), Integer), Decimal), '((PIVA, SA_COD, APPEZZA, ID_REG), LAV_COD), CU_xDISTINTAxLAV_COD)
                                                  attivita As Attivita) As Dictionary(Of (String, Integer, Integer, Integer), Decimal)

        'Lista di tutte le sostanze che congono del rame
        Dim PA_Rameici As Integer() = {102, 338, 350, 368, 369, 370, 371, 372, 529, 616, 636, 692, 883}

        Dim TotCUxHA_Trattamenti_MultiAttivita_xDistinta As New Dictionary(Of (String, Integer, Integer, Integer), Decimal)
        Dim TotCUxHa_DistintaxLavCod As Decimal = 0
        Dim TotCUxHa_TrattamentixDistinta As Decimal = 0

        'Ricavo gli impianti su cui calcolare il CU effettivo
        Dim EserciziCDC As List(Of EsercizioCDC) = attivita.centriDiCosto.FindAll(Function(c) (c.classType = ClassType.EsercizioCDC)).ConvertAll(Function(obj1) CType(obj1, EsercizioCDC))


        For Each imp In EserciziCDC
            TotCUxHa_TrattamentixDistinta = 0
            Dim piva As String = imp.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva
            Dim sa_cod As Integer = imp.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice
            Dim appezza As Integer = imp.esercizio.impiantoPK.appezzamentoPK.codice
            Dim id_reg As Integer = imp.esercizio.impiantoPK.codice

            'Ciclo le risorse e mi salvo TotCU_Trattamento e il totalone finale TotCU_TrattamentixDistinta
            For Each dic In dic_risorseTrattamento
                TotCUxHa_DistintaxLavCod = 0
                For Each risorsa In dic.Value
                    If risorsa.classType = costanti.ClassType.DettaglioTrattamento Then
                        Dim trattamento As dettagli.DettaglioTrattamento = CType(risorsa, dettagli.DettaglioTrattamento)
                        For Each PA In trattamento.principiAttivi
                            '102,338,350,368,369,370,371,372,529,616,636,692,883
                            If PA_Rameici.Contains(PA.codice) Then

                                Dim Qta_Prodotto_HA As Decimal = trattamento.doseHaReale
                                Dim QTA_Prodotto_HA_Kg_L As Decimal = 0
                                Dim Udm_Cod_Trasformato As Integer
                                Dim Moltiplicatore As Decimal

                                'Conversione forzata Kg/L 
                                AgronicaCoreMetaSchemaDAL.UnitaMisura_R.ConvertiToKG_L(trattamento.unitaDiMisuraIndicata.codice, Udm_Cod_Trasformato, Moltiplicatore)
                                'Trovo la QTA di prodotto utilizzata sul singolo impianto
                                QTA_Prodotto_HA_Kg_L = Qta_Prodotto_HA * Moltiplicatore

                                'Calcolo il CU e lo aggiungo ai totali (globali e per singolo LAV_COD)
                                If PA.peso = 0 Then
                                    TotCUxHa_DistintaxLavCod += CDec(QTA_Prodotto_HA_Kg_L * PA.titolo / 100)
                                    TotCUxHa_TrattamentixDistinta += CDec(QTA_Prodotto_HA_Kg_L * PA.titolo / 100)
                                Else
                                    TotCUxHa_DistintaxLavCod += CDec(QTA_Prodotto_HA_Kg_L * PA.peso / 1000)
                                    TotCUxHa_TrattamentixDistinta += CDec(QTA_Prodotto_HA_Kg_L * PA.peso / 1000)
                                End If

                            End If
                        Next
                    End If
                Next

                If TotCUxHa_DistintaxLavCod <> 0 Then
                    'Aggiungo al dic il totale del rame trovato x Distinta x LAV_COD
                    dic_CUxHATrattamenti_xDistintaxLavCod.Add(((piva, sa_cod, appezza, id_reg), dic.Key), TotCUxHa_DistintaxLavCod)
                End If

            Next

            If TotCUxHa_TrattamentixDistinta <> 0 Then
                'Aggiungo al dic il totale del rame trovato x Distinta 
                TotCUxHA_Trattamenti_MultiAttivita_xDistinta.Add((piva, sa_cod, appezza, id_reg), TotCUxHa_TrattamentixDistinta)
            End If
        Next

        Return TotCUxHA_Trattamenti_MultiAttivita_xDistinta

    End Function

    Private Shared Function estraiRameFertilizzazione(ByRef risorseFertilizzazione As List(Of risorse.Risorsa),
                                                      attivita As Attivita) As Dictionary(Of (String, Integer, Integer, Integer), Decimal)


        Dim TotCUxHA_Fertilizzazione_MultiAttivita_xDistinta As New Dictionary(Of (String, Integer, Integer, Integer), Decimal)
        Dim TotCUxHA_FertilizzazionexDistinta As Decimal = 0

        Dim EserciziCDC As List(Of EsercizioCDC) = attivita.centriDiCosto.FindAll(Function(c) (c.classType = ClassType.EsercizioCDC)).ConvertAll(Function(obj1) CType(obj1, EsercizioCDC))
        For Each imp In EserciziCDC
            TotCUxHA_FertilizzazionexDistinta = 0
            Dim piva As String = imp.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva
            Dim sa_cod As Integer = imp.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice
            Dim appezza As Integer = imp.esercizio.impiantoPK.appezzamentoPK.codice
            Dim id_reg As Integer = imp.esercizio.impiantoPK.codice
            For Each risorsa In risorseFertilizzazione
                If risorsa.classType = costanti.ClassType.DettaglioFertilizzazione Then
                    Dim fertilizzazione As dettagli.DettaglioFertilizzazione = CType(risorsa, dettagli.DettaglioFertilizzazione)


                    Dim Qta_Prodotto_HA As Decimal = fertilizzazione.doseHaReale
                    Dim QTA_Prodotto_HA_Kg_L As Decimal = 0
                    Dim Udm_Cod_Trasformato As Integer
                    Dim Moltiplicatore As Decimal

                    'Conversione forzata Kg/L 
                    AgronicaCoreMetaSchemaDAL.UnitaMisura_R.ConvertiToKG_L(fertilizzazione.unitaDiMisuraIndicata.codice, Udm_Cod_Trasformato, Moltiplicatore)

                    'Trovo la QTA di prodotto utilizzata sul singolo impianto
                    QTA_Prodotto_HA_Kg_L = Qta_Prodotto_HA * Moltiplicatore
                    TotCUxHA_FertilizzazionexDistinta += CDec(QTA_Prodotto_HA_Kg_L * fertilizzazione.Cu / 100)
                End If
            Next
            'Aggiungo al dic il totale del rame trovato x Distinta 
            TotCUxHA_Fertilizzazione_MultiAttivita_xDistinta.Add((piva, sa_cod, appezza, id_reg), TotCUxHA_FertilizzazionexDistinta)
        Next

        Return TotCUxHA_Fertilizzazione_MultiAttivita_xDistinta

    End Function

    ''' <param name="TotCU_Trattamenti_MultiAttivita_xDistinta"> ((PIVA, SA_COD, APPEZZA, ID_REG), CU_xDISTINTA)
    ''' MI SALVO IL TOT DI RAME USATO NEI TRATTAMENTI X OGNI DISTINTA --> DA PASSARE AL VERIFICA CONCIMAZIONE
    ''' </param>
    ''' <param name="TotCU_Fertilizzazione_MultiAttivita_xDistinta"> ((PIVA, SA_COD, APPEZZA, ID_REG), CU_xDISTINTA)
    ''' MI SALVO IL TOT DI RAME USATO NELLE CONCIMAZIONI X OGNI DISTINTA --> DA PASSARE AL VERIFICA TRATTAMENTI
    ''' </param>
    ''' <param name="dic_CUTrattamenti_xDistintaxLavCod"> ((PIVA, SA_COD, APPEZZA, ID_REG), LAV_COD), CU_xDISTINTAxLAV_COD)
    ''' MI SALVO IL TOT DI RAME USATO NELLE CONCIMAZIONI X OGNI DISTINTA X OGNI LAV_COD --> DA PASSARE AL VERIFICA TRATTAMENTI, COSI' DA POTER CONTARE I MULTI TRATTAMENTI
    ''' </param>
    Public Shared Sub CheckDPI(agenda As Operazione_Agenda,
                               infoOperazione As InfoOperazione,
                               currentAttivitaDes As String,
                               mostraWarning_ControlloDPI As Boolean,
                               objParametri_Super_Server As AgronicaCoreParametri,
                               objParametri_Server As AgronicaCoreParametri,
                               objParametri_Utenti As AgronicaCoreParametri,
                               ByRef lista_Errori As List(Of ErroreGias),
                               paramVerificaDPIMultiAttivita As VerificaDPIMultiAttivita
                               )

        Try
            Dim valoreImpostazione_BloccoTrattamento As Integer 'Questo nome fa schifo, da cambiare
            Dim valoreImpostazioneBloccoFertilizzazione As Integer

            Dim severity As Integer

            'una volta creato l'oggetto agenda posso invocare il suo metodo che mi genera l'xml
            Dim Dpi_Cod As Integer = 0
            Dim Dpi_PubblicoPrivato As Integer = 0

            Dim strXML = extraiXMLAgenda(agenda, infoOperazione, Dpi_Cod, Dpi_PubblicoPrivato)

            If infoOperazione.IsTrattamento Then
                getSeverityDaValoreImpostazione_0NessunBlocco_1WarningBloccante_2Warning(severity, enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME, objParametri_Utenti, valoreImpostazione_BloccoTrattamento)
                If valoreImpostazione_BloccoTrattamento = 0 Then
                    'Impostazione utente = Nessun Blocco ---> ESCO
                    Exit Sub
                End If
            End If

            If infoOperazione.IsFertilizzazione Then
                getSeverityDaValoreImpostazione_0NessunBlocco_1WarningBloccante_2Warning(severity, enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_BLOCCO_SALVA_FERTILIZZAZIONI, objParametri_Utenti, valoreImpostazioneBloccoFertilizzazione)
                If valoreImpostazioneBloccoFertilizzazione = 0 Then
                    'Impostazione utente = Nessun Blocco ---> ESCO
                    Exit Sub
                End If
            End If

            If infoOperazione.IsRaccolta Then
                getSeverityDaValoreImpostazione_0Warning_1WarningBloccante(severity, enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_RACCOLTA_CARENZA_NON_RISPETTATA, objParametri_Utenti)
            End If

            Dim Conforme As Boolean

            Dim objDpiVerifica As New AgronicaCoreDpiBIZ.DPI_Verifica
            Dim rval As New rispostaStandard(Of AgronicaCoreDpiBIZ.Verifica_Disciplinare_Intervento)
            rval = objDpiVerifica.Verifica_Conformita_Intervento_New(objParametri_Server, objParametri_Utenti,
                                                                     agenda.Piva, strXML,
                                                                     0, agenda.Id_Agenda,
                                                                     True,
                                                                     enum_Disciplinare_Operazione.QuelloDellOperazione, 0,
                                                                     "", 0, 0, 0,
                                                                     objParametri_Super_Server:=objParametri_Super_Server,
                                                                     isFromAgendaNG:=True,
                                                                     paramVerificaDPIMultiAttivita:=paramVerificaDPIMultiAttivita)

            If Not rval.RispostaOK Then
                Throw New Exception(currentAttivitaDes & ": " & rval.Errore)
            End If

            Dim errore As String

            If rval.RispostaOK Then
                errore = rval.RispostaStringa.strNonConformita
                Conforme = rval.RispostaStringa.Conforme

                If Not Conforme Then
                    If Not (severity = ErroreGias_Severity.Warning AndAlso mostraWarning_ControlloDPI = False) Then
                        errore = Utility_Agenda.rimuoviDuplicatiMessaggioErroreVerificaDPI(errore)
                        lista_Errori.Add(generaErroreGias(severity, currentAttivitaDes, errore, Key_Parametri_Aggiuntivi_Attivita.mostraWarning_CheckDPI))
                    End If
                End If
            End If

        Catch ex As GiasException
            Throw ex
        Catch ex As Exception
            Throw New Exception(currentAttivitaDes, ex)
        End Try

    End Sub

    Public Sub CheckAttivita_ListaAttivita(lista_Attivita As List(Of AgronicaCoreModelsSTD.attivita.Attivita),
                                           parametriAggiuntiviList As List(Of Parametri_Aggiuntivi_Attivita),
                                           mostraWarning_CheckListaAttivita As Boolean,
                                           objParametri_Super_Server As AgronicaCoreParametri,
                                           objParametri_Server As AgronicaCoreParametri,
                                           objParametri_Utenti As AgronicaCoreParametri,
                                           ByRef lista_Errori As List(Of ErroreGias))

        Dim currentAttivitaDes = ""

        Try

            lista_Errori.AddRange(CheckProdottoMultiplo(lista_Attivita))
            If lista_Errori.FindAll(Function(c) (c.severity = ErroreGias_Severity.Bloccante OrElse c.severity = ErroreGias_Severity.WarningBloccante)).Count > 0 Then
                'alla prima occorrenza di errore bloccante  esco
                Exit Sub
            End If

            lista_Errori.AddRange(CheckSottoscrizioneSevizioQDCFromLista_Attivita(lista_Attivita, objParametri_Server))
            If lista_Errori.FindAll(Function(c) (c.severity = ErroreGias_Severity.Bloccante OrElse c.severity = ErroreGias_Severity.WarningBloccante)).Count > 0 Then
                'alla prima occorrenza di errore bloccante  esco
                Exit Sub
            End If

            For Each attivita In lista_Attivita
                currentAttivitaDes = attivita.job.descrizione

                lista_Errori.AddRange(CheckAttivita(attivita, objParametri_Super_Server, objParametri_Server, objParametri_Utenti, mostraWarning_CheckListaAttivita, parametriAggiuntiviList:=parametriAggiuntiviList))
                If lista_Errori.FindAll(Function(c) (c.severity = ErroreGias_Severity.Bloccante)).Count > 0 Then
                    'alla prima occorrenza di errore bloccante esco
                    Exit Sub
                End If
            Next

        Catch ex As GiasException
            Throw ex
        Catch ex As Exception
            Throw New Exception(currentAttivitaDes, ex)
        End Try

    End Sub

    Private Function CheckProdottoMultiplo(lista_Attivita As List(Of Attivita)) As List(Of ErroreGias)

        'Lo stesso prodotto non può essere usato su due attività distinte dello stesso tipo (elem_cod).
        'Es. lo stesso codice prodotto in Trattamento Antiparassitario e Diserbo va impedito, perchè sarebbe lo stesso e la cosa va vietata
        'perchè complica i controlli di giacenza
        'invece lo stesso codice prodotti in trattamenti e concimazioni non significa che sia lo stesso prodotto
        Dim lista_Errori As New List(Of ErroreGias)

        Dim dic_ProdottoxElem_Cod As New Dictionary(Of (Integer, Integer), Integer)

        For Each attivita In lista_Attivita
            Dim InfoOperazione As InfoOperazione = GetInfoOperazione(attivita.job.primaryKey.codice, Tipo_Attivita.QuadernoDiCampagna)

            For Each risorsa In attivita.risorse.FindAll(Function(c) (c.classType = costanti.ClassType.DettaglioTrattamento OrElse c.classType = ClassType.DettaglioFertilizzazione OrElse c.classType = ClassType.DettaglioSemina OrElse c.classType = ClassType.DettaglioRaccolta)).ConvertAll(Function(obj1) CType(obj1, risorse.RisorsaProdotto))
                If Not dic_ProdottoxElem_Cod.ContainsKey((risorsa.prodotto.codice, InfoOperazione.Elem_Cod)) Then
                    dic_ProdottoxElem_Cod.Add((risorsa.prodotto.codice, InfoOperazione.Elem_Cod), attivita.job.primaryKey.codice)
                Else
                    Dim prevLavCod = dic_ProdottoxElem_Cod((risorsa.prodotto.codice, InfoOperazione.Elem_Cod))
                    If prevLavCod <> attivita.job.primaryKey.codice Then
                        Dim messaggio = String.Format(My.Resources.AgronicaCoreMapper.ProdottoMultiplo, risorsa.prodotto.descrizione)
                        lista_Errori.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, attivita.job.descrizione, messaggio, Key_Parametri_Aggiuntivi_Attivita.mostraWarning_CheckListaAttivita))
                    End If
                End If
            Next
        Next
        Return lista_Errori
    End Function
    Public Shared Function CheckAttivita(attivita As Attivita,
                                         objParametri_Super_Server As AgronicaCoreParametri,
                                         objParametri_Server As AgronicaCoreParametri,
                                         objParametri_Utenti As AgronicaCoreParametri,
                                         Optional mostraWarning_CheckListaAttivita As Boolean = True,
                                         Optional parametriAggiuntiviList As List(Of Parametri_Aggiuntivi_Attivita) = Nothing) As List(Of ErroreGias)


        Dim lista_Errori As New List(Of ErroreGias)

        Dim Lav_Cod = attivita.job.primaryKey.codice
        Dim InfoOperazione As InfoOperazione = GetInfoOperazione(Lav_Cod, tipoAttivita:=1) 'RR: Ho fissato tipoAttivita a QDC perchè non si controlla serve sapere solo operazione("Trattamento..")


        '===================================
        '   CONTROLLI VALIDITA GENERALI    
        '-----------------------------------
        Dim messaggio As String = ""
        If attivita.inizio = AGRODATAINIZIO Then
            messaggio = My.Resources.AgronicaCoreMapper.IndicareUnaData
            lista_Errori.Add(generaErroreGias(ErroreGias_Severity.Bloccante, "", messaggio, Key_Parametri_Aggiuntivi_Attivita.mostraWarning_CheckListaAttivita))
            Return lista_Errori
        End If

        If attivita.job.primaryKey.classType.Equals(costanti.ClassType.Lavorazione) AndAlso attivita.job.primaryKey.codice = "" Then
            messaggio = My.Resources.AgronicaCoreMapper.SelezionareUnOperazione
            lista_Errori.Add(generaErroreGias(ErroreGias_Severity.Bloccante, "", messaggio, Key_Parametri_Aggiuntivi_Attivita.mostraWarning_CheckListaAttivita))
            Return lista_Errori
        End If

        'ACQUA
        Dim countAcqua As Integer = attivita.risorse.Where(Function(x) x.classType = costanti.ClassType.RisorsaAcqua).Count()
        If countAcqua > 1 Then
            messaggio = My.Resources.AgronicaCoreMapper.AcquaMultipla
            lista_Errori.Add(generaErroreGias(ErroreGias_Severity.Bloccante, "", messaggio, Key_Parametri_Aggiuntivi_Attivita.mostraWarning_CheckListaAttivita))
            Return lista_Errori
        End If

        Dim valoreAcqua As Decimal = 0
        If countAcqua = 1 Then
            Dim risorsaAcqua As risorse.RisorsaAcqua = attivita.risorse.Where(Function(x) x.classType = costanti.ClassType.RisorsaAcqua).FirstOrDefault
            If Not checkAcqua(risorsaAcqua, attivita.job, attivita.risorse, lista_Errori) Then
                Return lista_Errori
            End If
        End If

        'TODO_DT --> PER ORA NON PUO' CAPITARE, SOSPENDERE
        'PRODOTTI / MAGAZZINI
        'Dim erroreGiasMagazzino As New ErroreGias
        'If Not CheckProdottiMagazzini(attivita, erroreGiasMagazzino) Then
        '    lista_Errori.Add(erroreGiasMagazzino)
        '    Return lista_Errori
        'End If

        If Not IsNothing(attivita.centriDiCosto) AndAlso attivita.centriDiCosto.FindIndex(Function(c) c.classType = ClassType.EsercizioCDC) > -1 Then

            Dim EserciziCDC As List(Of EsercizioCDC) = attivita.centriDiCosto.FindAll(Function(c) (c.classType = ClassType.EsercizioCDC)).ConvertAll(Function(obj1) CType(obj1, EsercizioCDC))

            If Not CheckEserciziCDC(EserciziCDC, attivita.inizio, objParametri_Server, lista_Errori) Then
                Return lista_Errori
            End If
        End If

        Dim MacchinaList As List(Of risorse.RisorsaMacchina) =
                        attivita.risorse.FindAll(Function(c) (c.classType = ClassType.RisorsaMacchina)).ConvertAll(Function(obj1) CType(obj1, risorse.RisorsaMacchina))
        Dim OperatoreList As List(Of risorse.RisorsaPersona) =
                            attivita.risorse.FindAll(Function(c) (c.classType = ClassType.RisorsaPersona)).ConvertAll(Function(obj1) CType(obj1, risorse.RisorsaPersona))
        Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim severity As Integer


        If Not CheckValiditaMacchine(MacchinaList, attivita.inizio, lista_Errori) Then
            Return lista_Errori
        End If

        '===================================
        '   CONTROLLI IMPOSTAZIONI UTENTE    
        '-----------------------------------
        Dim ValoreImpostazione As Integer

        '--- UTENTE_COD_BLOCCA_SE_SENZA_MAGAZZINO
        ValoreImpostazione = CInt(objUtentiImpostazioni.LeggiValoreImpostazioneScalare(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_SE_SENZA_MAGAZZINO, objParametri_Utenti.UtenteUsername, objParametri_Utenti))
        If ValoreImpostazione = 1 Then
            Dim risorseProdotto As List(Of risorse.RisorsaProdotto) = attivita.risorse.FindAll(Function(c) (c.classType = costanti.ClassType.DettaglioTrattamento OrElse c.classType = ClassType.DettaglioFertilizzazione OrElse c.classType = ClassType.DettaglioSemina)).ConvertAll(Function(obj1) CType(obj1, risorse.RisorsaProdotto))

            'DT: se lo stesso prodotto appare in risorse distinte, alcune con magazzino e altre no, errore
            Dim codiciProdottoDistinti As List(Of Integer) = (From r In risorseProdotto Select r.prodotto.codice).Distinct.ToList
            Dim senzaMagazzino As Boolean = False
            For Each codiceProdotto In codiciProdottoDistinti
                For Each risorsaProdotto In risorseProdotto.FindAll(Function(r) (r.prodotto.codice = codiceProdotto))
                    If risorsaProdotto.MagazziniMovimentazioni Is Nothing OrElse risorsaProdotto.MagazziniMovimentazioni.Count = 0 Then
                        senzaMagazzino = True
                        Exit For
                    End If
                Next

                If senzaMagazzino Then

                    messaggio = ImpostazioniUtenteMagazzinoObbligatorio
                    lista_Errori.Add(generaErroreGias(ErroreGias_Severity.Bloccante, attivita.job.descrizione, messaggio, Key_Parametri_Aggiuntivi_Attivita.mostraWarning_CheckListaAttivita))
                    Return lista_Errori
                End If
            Next
        End If

        '--- UTENTE_COD_DEFAULT_BLOCCO_INSERIMENTO_NOTE
        ValoreImpostazione = CInt(objUtentiImpostazioni.LeggiValoreImpostazioneScalare(enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_BLOCCO_INSERIMENTO_NOTE, objParametri_Utenti.UtenteUsername, objParametri_Utenti))
        If ValoreImpostazione = 1 Then
            If InfoOperazione.IsTrattamento AndAlso (IsNothing(attivita.noteIntervento) OrElse attivita.noteIntervento.Count = 0) Then

                messaggio = ImpostazioniUtenteNotaObbligatoria
                lista_Errori.Add(generaErroreGias(ErroreGias_Severity.Bloccante, attivita.job.descrizione, messaggio, Key_Parametri_Aggiuntivi_Attivita.mostraWarning_CheckListaAttivita))

                Return lista_Errori
            End If
        End If

        '--- UTENTE_COD_DEFAULT_BLOCCO_INSERIMENTO_MACCHINE
        ValoreImpostazione = CInt(objUtentiImpostazioni.LeggiValoreImpostazioneScalare(enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_BLOCCO_INSERIMENTO_MACCHINE, objParametri_Utenti.UtenteUsername, objParametri_Utenti))
        If ValoreImpostazione = 1 Then
            If InfoOperazione.IsTrattamento AndAlso MacchinaList.Count = 0 AndAlso Not isInsettiUtiliTrappole(InfoOperazione) Then

                messaggio = ImpostazioniUtenteMacchinaObbligatoria
                lista_Errori.Add(generaErroreGias(ErroreGias_Severity.Bloccante, attivita.job.descrizione, messaggio, Key_Parametri_Aggiuntivi_Attivita.mostraWarning_CheckListaAttivita))

                Return lista_Errori
            End If
        End If

        '--- UTENTE_COD_DEFAULT_BLOCCO_INSERIMENTO_OPERATORE
        ValoreImpostazione = CInt(objUtentiImpostazioni.LeggiValoreImpostazioneScalare(enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_BLOCCO_INSERIMENTO_OPERATORE, objParametri_Utenti.UtenteUsername, objParametri_Utenti))
        If ValoreImpostazione = 1 Then
            If InfoOperazione.IsTrattamento AndAlso OperatoreList.Count = 0 AndAlso Not isInsettiUtiliTrappole(InfoOperazione) Then

                messaggio = ImpostazioniUtenteOperatorebbligatorio
                lista_Errori.Add(generaErroreGias(ErroreGias_Severity.Bloccante, attivita.job.descrizione, messaggio, Key_Parametri_Aggiuntivi_Attivita.mostraWarning_CheckListaAttivita))

                Return lista_Errori
            End If
        End If

        If InfoOperazione.IsTrattamento AndAlso Not isInsettiUtiliTrappole(InfoOperazione) Then
            '===================================
            '   CONTROLLO PATENTINO OPERATORE  
            '-----------------------------------
            Dim controlloOperatore As Integer
            Dim listaMessaggiOperatore As New List(Of String)
            Dim almenoUnPatentinoPresente As Boolean = False

            getSeverityDaValoreImpostazione_0NessunBlocco_1Warning_2WarningBloccante(severity, enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_ProdottiTossiciPatentinoMovimenti, objParametri_Utenti, controlloOperatore)
            If controlloOperatore <> 0 AndAlso Not (severity = ErroreGias_Severity.Warning AndAlso mostraWarning_CheckListaAttivita = False) Then 'to do... <> 0
                'Se su quattro operatori almeno uno ha il patentino valido, passa, altrimenti è errore 
                'Se nessun operatore ha il patentino NON PASSO 
                If OperatoreList.Count > 0 Then 'to do... <> 0
                    For Each operatore In OperatoreList
                        Dim nomeOperatore As String = operatore.risorsaUmana.contatto.nome & " " & operatore.risorsaUmana.contatto.cognome
                        If Not IsNothing(operatore.risorsaUmana.contatto.documenti) Then
                            Dim listDocumenti = operatore.risorsaUmana.contatto.documenti.OrderByDescending(Function(o) o.Data_Scadenza).ToList()
                            If listDocumenti.Count > 0 Then
                                Dim ultimoDocumento = listDocumenti(0)
                                If ultimoDocumento.Data_Scadenza < attivita.inizio Then
                                    listaMessaggiOperatore.Add(String.Format(PatentinoOperatoreScaduto, nomeOperatore, ultimoDocumento.Data_Scadenza)) 'TODO traduzione
                                Else
                                    'Non mi va bene AGRODATAFINE 
                                    If ultimoDocumento.Data_Scadenza <> AGRODATAFINE Then
                                        almenoUnPatentinoPresente = True
                                        'Basta che ci sia un patentino valido, esco
                                        Exit For
                                    End If
                                End If
                            End If
                        End If
                    Next
                End If

                If Not almenoUnPatentinoPresente Then
                    If listaMessaggiOperatore.Count > 0 Then
                        'Tutti gli operatori selezionati hanno il patentino scaduto, errore!
                        For Each mes In listaMessaggiOperatore
                            lista_Errori.Add(generaErroreGias(severity, attivita.job.descrizione, mes, Key_Parametri_Aggiuntivi_Attivita.mostraWarning_CheckListaAttivita))
                        Next
                    Else
                        'Nessun operatore ha il patentino, errore!
                        messaggio = SelezionareOperatorePatentinoValido
                        lista_Errori.Add(generaErroreGias(severity, attivita.job.descrizione, messaggio, Key_Parametri_Aggiuntivi_Attivita.mostraWarning_CheckListaAttivita))
                    End If
                End If
            End If



            '===================================
            '   CONTROLLO TARATURA MACCHINA      to do...se non c'è una data cosa faccio?
            '-----------------------------------
            Dim controlloMacchina As Integer
            Dim listaMessaggiMacchina As New List(Of String)
            Dim almenoUnaTaraturaPresente As Boolean = False

            getSeverityDaValoreImpostazione_0NessunBlocco_1Warning_2WarningBloccante(severity, enum_Impostazioni_Utenti.UTENTE_COD_BLOCCO_TARATURA_ATOMIZZATORE_SCADUTA, objParametri_Utenti, controlloMacchina)
            If controlloMacchina <> 0 AndAlso Not (severity = ErroreGias_Severity.Warning AndAlso mostraWarning_CheckListaAttivita = False) Then 'to do... <> 0
                If MacchinaList.Count > 0 Then
                    For Each mac In MacchinaList
                        'Controllo solo le tarature delle MACCHINE-ATOMIZZATORE
                        'Se su quattro MACCHINA-ATOMIZZATORE almeno una ha la taratura valida, passa, altrimenti è errore 
                        'Se nessuna MACCHINA-ATOMIZZATORE ha la taratura valida (NOTHING O AGRODATAINIZIO) NON PASSO 
                        Dim macchina_atomizzatore As ParcoMacchine = mac.macchina
                        If macchina_atomizzatore.dettaglio_2.codice = "01" OrElse macchina_atomizzatore.dettaglio_2.descrizione = "atomizzatori" Then 'TODO... é corretto?
                            Dim nomeMacchina As String = macchina_atomizzatore.marca.descrizione & " - " & macchina_atomizzatore.modello & " - '" & macchina_atomizzatore.descrizione & "' (" & Atomizzatore & ")"
                            'To do... cosa fare se la scadenza = nothing oppure = agrodatafine?
                            If macchina_atomizzatore.scadenza_Taratura < attivita.inizio Then
                                listaMessaggiMacchina.Add(String.Format(TaraturaMacchinaScaduta, nomeMacchina, macchina_atomizzatore.scadenza_Taratura))
                            Else
                                'Non mi va bene AGRODATAFINE 
                                If macchina_atomizzatore.scadenza_Taratura <> AGRODATAFINE Then
                                    almenoUnaTaraturaPresente = True
                                    'Basta che ci sia una taratura valida, esco
                                    Exit For
                                End If
                            End If
                        End If
                    Next
                End If

                If Not almenoUnaTaraturaPresente Then
                    If listaMessaggiMacchina.Count > 0 Then
                        'Tutti le MACCHINE-ATOMIZZATORE selezionate hanno scadenza taratura non valida, errore!
                        For Each mes In listaMessaggiMacchina
                            lista_Errori.Add(generaErroreGias(severity, attivita.job.descrizione, mes, Key_Parametri_Aggiuntivi_Attivita.mostraWarning_CheckListaAttivita))
                        Next
                    Else
                        'Nessuna MACCHINA-ATOMIZZATORE ha la scadenza taratura impostata, errore!
                        messaggio = SelezionareMacchinaAtomizzatoreConTaraturaValida
                        lista_Errori.Add(generaErroreGias(severity, attivita.job.descrizione, messaggio, Key_Parametri_Aggiuntivi_Attivita.mostraWarning_CheckListaAttivita))
                    End If
                End If
            End If
        End If

        '===================================
        '   CONTROLLO TRATTAMENTI   
        '-----------------------------------
        If InfoOperazione.IsTrattamento AndAlso Not isInsettiUtiliTrappole(InfoOperazione) Then
            Dim dettagliTrattamento As List(Of dettagli.DettaglioTrattamento
                ) = attivita.risorse.FindAll(Function(c) (c.classType = ClassType.DettaglioTrattamento)).ConvertAll(Function(obj1) CType(obj1, dettagli.DettaglioTrattamento))
            Dim tipoFormulato_NoAvversita As Integer() = {enum_TipoFormulato.Coadiuvanti, enum_TipoFormulato.Corroboranti_Fisiofarmaci, enum_TipoFormulato.Fitoregolatori, enum_TipoFormulato.Disseccanti}

            For Each dettaglioTrattamento In dettagliTrattamento

                'TODO Chiedere se va bene così il controllo
                If ((Not tipoFormulato_NoAvversita.Contains(dettaglioTrattamento.tipoFormulato) AndAlso
                    attivita.disciplinare IsNot Nothing AndAlso
                    attivita.disciplinare.codice <> enum_Disciplinare_Operazione.NessunDpiNessunaEtichetta) OrElse
                    (dettaglioTrattamento.tipoFormulato = enum_TipoFormulato.ConfusioneSessuale OrElse
                     dettaglioTrattamento.tipoFormulato = enum_TipoFormulato.DisorientamentoSessuale)) Then

                    If dettaglioTrattamento.avversitaGruppo Is Nothing OrElse dettaglioTrattamento.avversitaGruppo.codice = 0 Then
                        lista_Errori.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, attivita.job.descrizione, My.Resources.AgronicaCoreMapper.ENecessarioSelezionareUnAvversità, Key_Parametri_Aggiuntivi_Attivita.mostraWarning_CheckListaAttivita))
                    End If
                End If
            Next

            CheckInstallazioneTrappoleCatturaDiMassa(Lav_Cod, dettagliTrattamento, lista_Errori)


        End If


        '===================================
        '   CONTROLLO CARENZA RACCOLTA   
        '-----------------------------------
        If InfoOperazione.IsRaccolta Then
            If Not CheckRaccolta(attivita, mostraWarning_CheckListaAttivita, parametriAggiuntiviList,
                                 objParametri_Server, objParametri_Utenti, lista_Errori) Then
                Return lista_Errori
            End If
        End If

        '===================================
        '   CONTROLLO SEMINA CON FRAZIONAMENTO   
        '-----------------------------------
        If InfoOperazione.IsSemina AndAlso parametriAggiuntiviList IsNot Nothing Then
            Dim paramAggiuntiviList As List(Of Parametri_Aggiuntivi_Attivita) =
                parametriAggiuntiviList.FindAll(Function(c) c.key = Key_Parametri_Aggiuntivi_Attivita.Opzione_Semina)
            For Each parametroAggiuntivo In paramAggiuntiviList
                If parametroAggiuntivo.value = enum_SEMINA_TIPO.Frazionamento_Appezzamenti_perLotto_e_Semina_sui_Singoli_Default Then

                    '=============================================
                    '   CONTROLLO  NUMERO IMPIANTI SELEZIONATI
                    '---------------------------------------------
                    Dim EsercizioCDC As List(Of EsercizioCDC) = attivita.centriDiCosto.FindAll(Function(c) (c.classType = ClassType.EsercizioCDC)).ConvertAll(Function(obj1) CType(obj1, EsercizioCDC))
                    If EsercizioCDC.Count > 1 Then
                        'Se sono stati selezionati più impianti blocco la procedura.
                        'Il frazionamento si può fare con un solo impianto selezionato.
                        messaggio = Gias.OpzioneFrazionamentoPossibileSelezionareUnImpiantoPerVolta
                        lista_Errori.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, attivita.job.descrizione, messaggio, Key_Parametri_Aggiuntivi_Attivita.mostraWarning_ScriviAttivitaToAgenda))
                    End If

                    '==================================================
                    '   CONTROLLO  APPEZZAMENTO IMPIANTO SELEZIONATO
                    '--------------------------------------------------
                    Dim impiantoSelezionato As EsercizioCDC = EsercizioCDC(0)
                    Dim PIVA = impiantoSelezionato.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva
                    Dim SA_COD = impiantoSelezionato.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice
                    Dim APPEZZA = impiantoSelezionato.esercizio.impiantoPK.appezzamentoPK.codice

                    controllaAppezzamentoHasUnicoImpianto(PIVA, SA_COD, APPEZZA, lista_Errori, objParametri_Server, objParametri_Utenti)
                    If lista_Errori.Count > 0 Then
                        Return lista_Errori
                    End If
                End If
            Next
        End If

        If ((InfoOperazione.IsIrrigazione OrElse InfoOperazione.IsFertirrigazione) AndAlso parametriAggiuntiviList IsNot Nothing) Then
            Dim paramVerificaCompatibilitaMicro = parametriAggiuntiviList.FirstOrDefault(Function(p) p.key = Key_Parametri_Aggiuntivi_Attivita.Verifica_Compatibilita_Microirrigazione)

            If (paramVerificaCompatibilitaMicro IsNot Nothing AndAlso paramVerificaCompatibilitaMicro.value = "true") Then
                For Each risorsa In attivita.risorse

                    Dim dettaglioIrrigazione As DettaglioIrrigazione = TryCast(risorsa, DettaglioIrrigazione)

                    If dettaglioIrrigazione IsNot Nothing Then
                        Dim errorCondition1 = dettaglioIrrigazione.tipoIrrigazione IsNot Nothing AndAlso dettaglioIrrigazione.tipoIrrigazione.codice <> 31 AndAlso dettaglioIrrigazione.tipoIrrigazione.codice <> 33 AndAlso dettaglioIrrigazione.tipoIrrigazione.codice <> 111 AndAlso dettaglioIrrigazione.tipoIrrigazione.codice <> 112
                        Dim errorCondition2 = dettaglioIrrigazione.macchina IsNot Nothing AndAlso dettaglioIrrigazione.macchina.codice_impianto <> 31 AndAlso dettaglioIrrigazione.macchina.codice_impianto <> 33 AndAlso dettaglioIrrigazione.macchina.codice_impianto <> 111 AndAlso dettaglioIrrigazione.macchina.codice_impianto <> 112

                        If (errorCondition1 OrElse errorCondition2) Then
                            messaggio = Gias.VerificaCompatibilitaMicroirrigazioneFallita
                            lista_Errori.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, attivita.job.descrizione, messaggio, Key_Parametri_Aggiuntivi_Attivita.Verifica_Compatibilita_Microirrigazione))
                        End If
                    End If

                    If lista_Errori.Count > 0 Then
                        Return lista_Errori
                    End If
                Next

            End If
        End If

        '===================================
        'CONTROLLO FASI FENOLOGICHE INSERITE
        'Controllo che siano state inserite delle Fasi Fenologiche compatibili con la Specie Vegetale
        '-----------------------------------
        CheckRilievo(InfoOperazione, attivita, Lav_Cod, objParametri_Server, objParametri_Utenti, objParametri_Super_Server,
                            messaggio, lista_Errori)

        Return lista_Errori

    End Function

    Public Shared Function checkMagazziniRicettexApp_daAttivita(listaAttivita As List(Of Attivita),
                                                              objParametri_Utenti As AgronicaCoreParametri,
                                                              objParametri_Server As AgronicaCoreParametri,
                                                              objParametri_Super_Server As AgronicaCoreParametri
                                                              ) As List(Of ErroreGias)
        Dim listErrorixRicetta As New List(Of String)
        Dim listErroriRicetteMultiAttivita As New List(Of String)
        Dim listErrori As New List(Of String)
        Dim listErroriRicetteNonGestite As New List(Of String)

        Dim listErroriGias As New List(Of ErroreGias)

        Dim ultimaAttivita = listaAttivita.Last()

        Dim List_OP_NON_GESTITE_APP = STR_OP_NON_GESTITE_APP.Split(",").AsEnumerable().Select(Function(s) Convert.ToInt32(s)).ToList()

        If listaAttivita.FindIndex(Function(a) List_OP_NON_GESTITE_APP.Contains(CInt(a.job.primaryKey.codice))) > -1 Then
            listErroriRicetteNonGestite.Add("")
        End If

        For Each attivita In listaAttivita
            listErrorixRicetta.Clear()

            esegui_ControlloMagazziniRicettexApp(attivita, listErrorixRicetta, listErrori, objParametri_Server)
            componiErrorexRicetta_controlloMagazziniRicettexApp(attivita, listErrorixRicetta, listErrori, False)
        Next

        If listaAttivita.Count > 1 Then
            listErroriRicetteMultiAttivita.Add("")
        End If

        listErroriGias = componiErroreFinale_controlloMagazziniRicettexApp(False, listErrori, listErroriRicetteMultiAttivita, listErroriRicetteNonGestite, listErrorixRicetta.Count)

        Return listErroriGias
    End Function
#End Region


#Region "utility"
    Public Function getSaCodDaAttivita_Magazzino(lista_Attivita As List(Of AgronicaCoreModelsSTD.attivita.Attivita)) As List(Of Integer)

        Dim lista_SaCod As New List(Of Integer)
        Dim lista_RilevamentoDiMagazzino As New List(Of RilevamentoDiMagazzino)

        For Each attivita In lista_Attivita
            Dim listaRisorse As List(Of risorse.RisorsaProdotto) = attivita.risorse.FindAll(Function(c) (c.classType = costanti.ClassType.DettaglioTrattamento OrElse c.classType = ClassType.DettaglioFertilizzazione OrElse c.classType = ClassType.DettaglioSemina)).ConvertAll(Function(obj1) CType(obj1, risorse.RisorsaProdotto))

            For Each ris In listaRisorse
                lista_RilevamentoDiMagazzino.AddRange(ris.MagazziniMovimentazioni)
            Next
        Next

        For Each mag In lista_RilevamentoDiMagazzino
            lista_SaCod.Add(mag.Magazzino.primaryKey.centroAziendalePK.codice)
        Next

        Return lista_SaCod

    End Function

    Public Function MappaListaAttivitaToListaAgenda(lista_Attivita As List(Of AgronicaCoreModelsSTD.attivita.Attivita),
                                                    objParametri_Super_Server As AgronicaCoreParametri,
                                                    objParametri_Server As AgronicaCoreParametri,
                                                    objParametri_Utenti As AgronicaCoreParametri
                                                    ) As List(Of (attivita As Attivita, agenda As Operazione_Agenda))

        Dim listaAttivitaAgende = New List(Of (attivita As Attivita, agenda As Operazione_Agenda))
        For Each attivita In lista_Attivita

            'If attivita.risorse.Count = 0 Then
            '    Continue For
            'End If

            Dim agenda = MappaAttivitaToAgenda(attivita, objParametri_Super_Server, objParametri_Server, objParametri_Utenti, False)

            listaAttivitaAgende.Add((attivita, agenda))

        Next

        Return listaAttivitaAgende
    End Function

    Private Function GeneraCampionatura(Validita_Inizio As Date, objParametri_Server As AgronicaCoreParametri) As Integer
        Dim Progr As Integer = 12
        'il cal_cod dell'operazione ha il 12 , indefinito in Materie_Prime_Calibri
        'il cal_cod del carico invece ha il codice 'progressivo' nella tabella Materie_Prime_Campionature
        'viene creato uno nuovo ad ogni salvataggio,
        'vedi nota nel 2003:
        '' '' '' '' '' '' '' '' ''MODIFICA IN DATA 23/07/2009 BY MAGA:
        '' '' '' '' '' '' '' '' ''se il lavorato raccolto è stato utilizzato in scarichi e/o bolle emesse e/o fatture emesse
        '' '' '' '' '' '' '' '' ''(ovvero con causale <> conferimento)
        '' '' '' '' '' '' '' '' ''andando a modificare la raccolta, il cal_cod viene rigenerato, per cui cambia la chiave del prodotto
        '' '' '' '' '' '' '' '' ''(composta da elem_cod,pro_cod,mat_cod,cod_progetto,fase_cod,lotto,cal_cod,udm_cod)
        '' '' '' '' '' '' '' '' ''e di conseguenza si disallinea la giacenza: risulta caricato un prodotto, mentre ne viene scaricato un altro

        '' '' '' '' '' '' '' '' ''Introdotto, quindi, il controllo sul prodotto: se risultano presenti scarichi o bolle emesse o fatture emesse di esso
        '' '' '' '' '' '' '' '' ''IL SALVATAGGIO VIENE DISABILITATO e viene suggerito di cancellare prima l'operazione di uscita del prodotto.
        'viene creato automaticapete dal biz, e nel 2003 viene attaccato l'xml al dettaglio con XML_Agenda_RaccoltoCampionatura,
        'il biz crea la nuova voce.
        'agenda non ha il biz, quindi mi tocca creare la voce ora a mano e naturalmente con indefinito
        'che schifezza l'online 2003, il lan, le tabelle agenda e compagnia bella

        ' -- 06/06/2023 Si è deciso di eliminare la gestione dei Cal_Cod per la raccolta,
        ' ora viene salvato a 0 sia per il dettaglio prodotto che per quello di carico
        Dim Materie_Prime_Campionature As New AgronicaCoreContabBIZ.Materie_Prime_Campionature_W
        Dim ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

        Progr = ObjSequenze.NuovoId_Tabella("Materie_Prime_Campionature",
                                            0, 2000000000,
                                            objParametri_Server)
        Progr = -Math.Abs(Progr)
        Materie_Prime_Campionature.Scrivi_Progressivo_Calibro(Progr, "Indefinito",
                                                              0, 0, "",
                                                              Validita_Inizio, AGRODATAFINE,
                                                              objParametri_Server)
        Return Progr
    End Function

    Private Shared Sub controllaAppezzamentoHasUnicoImpianto(Piva As String,
                                                             Sa_Cod As Integer,
                                                             Appezza As Integer,
                                                             ByRef lista_Errori As List(Of ErroreGias),
                                                             ByRef objParametri_Server As AgronicaCoreParametri,
                                                             ByRef objParametri_Utenti As AgronicaCoreParametri)

        Dim objAppezzamento_R As New AgronicaCoreAnagrafeBIZ.Appezzamento_R
        Dim appezzamento = objAppezzamento_R.Leggi_Appezzamento_Anagrafica(Piva, Sa_Cod, Appezza, IdReg:=0,
                                                                           Leggi_Impianti:=True,
                                                                           Leggi_Indirizzi:=False,
                                                                           Leggi_Catasto:=False,
                                                                           data:=AGRODATAINIZIO,
                                                                           filtroData:=False,
                                                                           Leggi_Distinte:=False,
                                                                           Leggi_Cartografia:=False,
                                                                           objParametri_Super_Server:=Nothing,
                                                                           objParametri_Server,
                                                                           objParametri_Utenti)

        If appezzamento.impianti.Count > 1 Then
            Dim messaggio As String = ImpossibileFrazionareAppezzamentiPiuImpianti
            lista_Errori.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, "", messaggio, Key_Parametri_Aggiuntivi_Attivita.mostraWarning_ScriviAttivitaToAgenda))
        End If

    End Sub

    Private Shared Function GetExtraStr(risorsaProdotto As risorse.RisorsaProdotto, movimentoMagazzino As RilevamentoDiMagazzino, infoOperazione As InfoOperazione) As String

        Dim extraStr As String = ""

        If infoOperazione.IsSemina Then

            Dim dettaglioSemina = CType(risorsaProdotto, dettagli.DettaglioSemina)

            Dim lottoStr = If(movimentoMagazzino IsNot Nothing, movimentoMagazzino.Lotto, "")
            extraStr = risorsaProdotto.prodotto.descrizione & " " & String.Format(My.Resources.AgronicaCoreMapper.CodX0LottoX1, dettaglioSemina.codArticolo, lottoStr)

            If dettaglioSemina.regolamento = 4 Then
                extraStr &= " - BIO"
            End If

        End If

        Return extraStr

    End Function
#End Region

    Private Shared Sub impostaDateRicetta(ByRef Ricetta_DataInizio As Date,
ByRef Ricetta_DataFine As Date,
Data_Operazione As Date,
Ricetta_Cod As Integer,
Ricetta_Operazione_Cod As Integer,
                                          ByRef objParametri_Server As AgronicaCoreParametri)

        'DT: Aggiustamento intervallo date Ricetta in base alle date delle operazioni su db collegate alla stessa Ricetta e dell'operazione corrente
        Dim objRicette_Operazioni As New AgronicaCoreContabDAL.Ricette_R
        Dim maxData = objRicette_Operazioni.Leggi_MaxData(Ricetta_Cod, " Ricette_Operazioni.Ricetta_Operazione_Cod <> " & AgronicaCoreDataProvider.UtilityProvider.Agro_SQL_SaveNum(Ricetta_Operazione_Cod), objParametri_Server)
        Dim minData = objRicette_Operazioni.Leggi_MinData(Ricetta_Cod, " Ricette_Operazioni.Ricetta_Operazione_Cod <> " & AgronicaCoreDataProvider.UtilityProvider.Agro_SQL_SaveNum(Ricetta_Operazione_Cod), objParametri_Server)
        If Data_Operazione > maxData Then
            maxData = Data_Operazione
        End If
        If Data_Operazione < minData Then
            minData = Data_Operazione
        End If

        If Ricetta_DataInizio > minData Then
            Ricetta_DataInizio = minData
        End If

        If Ricetta_DataFine < maxData Then
            Ricetta_DataFine = maxData
        End If

    End Sub

    Private Shared Sub CreaOperazioniContabili(ByVal agenda As Operazione_Agenda,
                                               ByVal Job_Op_Campagna As Job,
                                               ByVal disciplinare As Disciplinare,
                                               ByVal risorse As List(Of Risorsa),
                                               ByVal agenda_Rif As List(Of Movimento_Dettaglio_Riferimento),
                                                ByVal objParametri_Server As AgronicaCoreParametri,
                                                ByVal objParametri_Utenti As AgronicaCoreParametri)

        Const nomeRoutine = "AgronicaCoreMapper.AttivitaToAgenda.CreaOperazioniContabili()"

        Try

            Dim ListAttivitaCarico As New List(Of Attivita)

            Dim ListAttivitaScarico As New List(Of Attivita)

            Crea_ListAttivita_Carico_Scarico(risorse, ListAttivitaCarico, ListAttivitaScarico)

            Dim ScritturaOperazioniScarico As Boolean = False

            Dim ScritturaOperazioniCarico As Boolean = False

            Dim List_Id_Agenda_Scarico_Old As New List(Of Integer)

            Dim List_Id_Agenda_Carico_Old As New List(Of Integer)

            If Not IsNothing(agenda_Rif) AndAlso agenda_Rif.Count > 0 Then

                For Each m_r As Movimento_Dettaglio_Riferimento In agenda_Rif

                    If m_r.Lav_Cod_Rif = LAVCOD_SCARICO OrElse m_r.Lav_Cod_Rif = LAVCOD_BOLLA_EMESSA Then
                        List_Id_Agenda_Scarico_Old.Add(m_r.Id_Agenda_Rif)
                    End If

                    If m_r.Lav_Cod_Rif = LAVCOD_CARICO OrElse m_r.Lav_Cod_Rif = LAVCOD_BOLLA_RICEVUTA Then
                        List_Id_Agenda_Carico_Old.Add(m_r.Id_Agenda_Rif)
                    End If
                Next

            End If

            ScritturaOperazioniScarico = Crea_Operazioni_Carico_Scarico(agenda, Job_Op_Campagna, disciplinare, List_Id_Agenda_Scarico_Old, ListAttivitaScarico, objParametri_Server, objParametri_Utenti)

            ScritturaOperazioniCarico = Crea_Operazioni_Carico_Scarico(agenda, Job_Op_Campagna, disciplinare, List_Id_Agenda_Carico_Old, ListAttivitaCarico, objParametri_Server, objParametri_Utenti)

        Catch ex As Exception
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        End Try

    End Sub

    Private Shared Function Crea_Operazioni_Carico_Scarico(ByVal Agenda_Op_Campagna As Operazione_Agenda,
                                                           ByVal Job_Op_Campagna As Job,
                                                           ByVal disciplinare As Disciplinare,
                                                            ByVal List_Id_Agenda_Carico_Scarico_Old As List(Of Integer),
                                                            ByVal ListAttivita_Carico_Scarico As List(Of Attivita),
                                                            ByVal objParametri_Server As AgronicaCoreParametri,
                                                            ByVal objParametri_Utenti As AgronicaCoreParametri) As Boolean
        Dim xRisp As Boolean = False

        Try

            For Each attivita As Attivita In ListAttivita_Carico_Scarico

                Dim Lav_Cod As Integer = attivita.job.primaryKey.codice

                Dim Cau_Mov As String = ""

                If Lav_Cod = LAVCOD_SCARICO OrElse Lav_Cod = LAVCOD_BOLLA_EMESSA Then
                    Cau_Mov = CAU_SCARICO
                ElseIf Lav_Cod = LAVCOD_CARICO OrElse Lav_Cod = LAVCOD_BOLLA_RICEVUTA Then
                    Cau_Mov = CAU_CARICO
                End If

                Dim Id_Agenda As Integer = 0

                'Elimino gli Id_Agenda delle Operazioni Contabili già utilizzati
                If List_Id_Agenda_Carico_Scarico_Old.Count > 0 Then
                    Id_Agenda = List_Id_Agenda_Carico_Scarico_Old.First()

                    List_Id_Agenda_Carico_Scarico_Old.RemoveAt(List_Id_Agenda_Carico_Scarico_Old.FindIndex(Function(Id_Agenda_Old) Id_Agenda_Old = Id_Agenda))
                End If

                Dim Registrazioni As List(Of RisorsaRegistrazione) = (From risorsa In attivita.risorse
                                                                      Where risorsa.classType = ClassType.RisorsaRegistrazione
                                                                      Select CType(risorsa, RisorsaRegistrazione)).ToList()


                If Not IsNothing(Registrazioni) AndAlso Registrazioni.Count > 0 Then
                    xRisp = Crea_Operazione_Carico_Scarico(Agenda_Op_Campagna, Registrazioni, 1, Lav_Cod, Job_Op_Campagna,
                                                           disciplinare, enum_Pendenza.MovPendente, Cau_Mov, Id_Agenda, objParametri_Server, objParametri_Utenti)

                    If xRisp Then
                        xRisp = Crea_Riferimento_Contabile(Agenda_Op_Campagna, Registrazioni, Id_Agenda, Lav_Cod, Cau_Mov, objParametri_Server)
                    End If
                End If


            Next

        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try

        Return xRisp
    End Function

    Private Shared Function Crea_Operazione_Carico_Scarico(ByVal Agenda_Op_Campagna As Operazione_Agenda,
                                                            ByVal Registrazioni As List(Of RisorsaRegistrazione),
                                                            ByVal Blocco_Flag As Integer,
                                                            ByVal Lav_Cod As Integer,
                                                            ByVal Job_Op_Campagna As Job,
                                                            ByVal disciplinare As Disciplinare,
                                                            ByVal pendente As enum_Pendenza,
                                                            ByVal Cau_Mov As String,
                                                            ByRef Id_Agenda As Integer,
                                                            ByVal objParametri_Server As AgronicaCoreParametri,
                                                            ByVal objParametri_Utenti As AgronicaCoreParametri) As Boolean

        Dim Id_Mov As Integer = 0

        Dim xRisp As Boolean = False

        Try
            Dim objContabilitaHelper_Dettaglio As New AgronicaCoreModello.ContabilitaHelper_Dettaglio(objParametri_Server, objParametri_Utenti)

            If IsNothing(Registrazioni) OrElse Registrazioni.Count = 0 OrElse IsNothing(Registrazioni(0).MagazziniMovimentazioni(0).Magazzino) Then
                Throw New Exception("Dati in input non valorizzati")
            End If

            Dim Uso_da_terzi As Boolean = Registrazioni(0).MagazziniMovimentazioni(0).Magazzino.usoDaTerzi

            Dim Piva_Magazzino As String = Registrazioni(0).MagazziniMovimentazioni(0).Magazzino.primaryKey.centroAziendalePK.partitaIva

            Dim ScaricoCaricoSemplice As Boolean = False

            If Lav_Cod = LAVCOD_SCARICO OrElse LAVCOD_CARICO Then
                ScaricoCaricoSemplice = True
            End If

            Dim DescrizioneAggiuntiva As String = ""

            If Uso_da_terzi Then

                Dim Lav_Des_Campagna As String = Job_Op_Campagna.descrizione

                Dim obj_imprese As New AgronicaCoreAnagrafeDAL.Imprese_Read

                Dim Rag_Soc_Campagna As String = obj_imprese.RagSoc_from_Piva(Agenda_Op_Campagna.Piva, objParametri_Server)

                DescrizioneAggiuntiva = String.Format(My.Resources.AgronicaCoreMapper.RelativoaAziendaPIVA, Lav_Des_Campagna, Rag_Soc_Campagna, Agenda_Op_Campagna.Piva)
            End If

            Dim objContabTestata As Contabilita_Output = objContabilitaHelper_Dettaglio.ScriviTestataCaricoScaricoAziendaAgricola(Agenda_Op_Campagna, Piva_Magazzino, Id_Agenda, ScaricoCaricoSemplice, Cau_Mov, Blocco_Flag, DescrizioneAggiuntiva)

            If objContabTestata.Risultato Then

                Id_Agenda = objContabTestata.Id_Agenda

                Id_Mov = objContabTestata.Id_Mov_Testata

                For Each RegistrazioneScarico As RisorsaRegistrazione In Registrazioni

                    For Each magazzinoMovimentazione As RilevamentoDiMagazzino In RegistrazioneScarico.MagazziniMovimentazioni

                        Dim contabilita_riga As Contabilita_Riga = Crea_Contabilita_Riga(Piva_Magazzino, magazzinoMovimentazione, magazzinoMovimentazione.Qta,
                                                                                         RegistrazioneScarico.unitaDiMisura, disciplinare, pendente, objContabTestata)


                        Dim objDettaglio As Contabilita_Output = objContabilitaHelper_Dettaglio.ScriviSingolaRigaDocumento(contabilita_riga, Lav_Cod, Cau_Mov, objContabTestata.Id_Mov_Testata, objContabTestata.Id_Mov_Testata, Agenda_Op_Campagna.Data, objParametri_Server.UsernameOperazione, True)

                        If Not objDettaglio.Risultato Then
                            Throw New Exception(objDettaglio.MsgError)
                        End If
                    Next

                Next

                xRisp = True
            Else
                Throw New Exception(objContabTestata.MsgError)
            End If

        Catch ex As Exception
            Dim exception As String = ex.Message

            If Cau_Mov = CAU_SCARICO Then
                exception += String.Format("Errore Scrittura Operazione di Scarico ( Lav_Cod {0}, Id_Agenda {1}, Id_Mov {2} )", Lav_Cod, Id_Agenda, Id_Mov)
            ElseIf Cau_Mov = CAU_CARICO Then
                exception += String.Format("Errore Scrittura Operazione di Carico ( Lav_Cod {0}, Id_Agenda {1}, Id_Mov {2} )", Lav_Cod, Id_Agenda, Id_Mov)
            End If

            Throw New Exception(exception)

            Return xRisp
        End Try

        Return xRisp
    End Function

    Private Shared Function Crea_Contabilita_Riga(ByVal piva As String, ByVal magazzinoMovimentazione As RilevamentoDiMagazzino, ByVal Qta As Decimal, ByVal UdM As UnitaDiMisura,
                                                  ByVal disciplinare As Disciplinare, ByVal pendente As enum_Pendenza, ByVal objContabTestata As Contabilita_Output,
                                                  Optional Id_Agenda As Integer = 0, Optional Tipo_Destinazione As Integer? = Nothing) As Contabilita_Riga

        Dim contabilita_riga As Contabilita_Riga = Nothing

        Try

            If IsNothing(magazzinoMovimentazione) OrElse IsNothing(magazzinoMovimentazione.Prodotto) OrElse
                IsNothing(magazzinoMovimentazione.Magazzino) OrElse IsNothing(magazzinoMovimentazione.udm) OrElse
                IsNothing(magazzinoMovimentazione.Magazzino.primaryKey.centroAziendalePK) Then

                Throw New Exception("Dati in input non valorizzati")

            End If

            Dim contabilita_magazzino As New Contabilita_Magazzino With {
                        .TipoDestinazione = If(Tipo_Destinazione, 20),
                        .SaCod = magazzinoMovimentazione.Magazzino.primaryKey.centroAziendalePK.codice,
                        .IdDestinazione = magazzinoMovimentazione.Magazzino.primaryKey.codice
                        }

            Dim Mat_Cod As Integer = 0

            Dim Pro_Cod As Integer = 0

            Dim Mat_Des As String = String.Empty

            Select Case magazzinoMovimentazione.Prodotto.elemCod
                Case SEMENTI, TRASFORMATI_VEGETALI, SEMILAVORATI_VEGETALI
                    Mat_Cod = magazzinoMovimentazione.Prodotto.codice
                Case Else
                    Pro_Cod = magazzinoMovimentazione.Prodotto.codice
            End Select

            Mat_Des = String.Format("{0} ({1})", magazzinoMovimentazione.Prodotto.descrizione, magazzinoMovimentazione.Prodotto.codice.ToString())

            'Regolamento
            Dim PuaRegolamento As Integer = 0
            If Not IsNothing(disciplinare) AndAlso Not IsNothing(disciplinare.regolamentoConcimazione) AndAlso disciplinare.regolamentoConcimazione.tipo = enum_PUARegolamenti_Tipo.PUA Then
                PuaRegolamento = disciplinare.regolamentoConcimazione.codice
            End If

            contabilita_riga = New Contabilita_Riga With {
                .AliquotaIva = 0,
                .Anno = 0,
                .CalCod = 0,
                .ChkImpiantiIndefiniti = False,
                .CodIva = 0,
                .Cod_Progetto = 0,
                .ContoEconomico = 0,
                .ContoPatrimoniale = 0,
                .Cu = magazzinoMovimentazione.Cu,
                .CulCod = 0,
                .Degrado = 0,
                .DestinazioneCarico = contabilita_magazzino,
                .DestinazioneScarico = contabilita_magazzino,
                .ElemCod = magazzinoMovimentazione.Prodotto.elemCod,
                .ExtraInt = UdM.codice,
                .Fornitore = 0,
                .ForzaIva = False,
                .IdAgenda = If(objContabTestata.Id_Agenda, Id_Agenda),
                .IdMov = 0,
                .IdMovDet = 0,
                .ImponibileTotale = 0,
                .ImponibileTotaleNetto = 0,
                .ImportoTotale = 0,
                .ImportoUnitario = 0,
                .Iva = 0,
                .IvaIndetraibile = 0,
                .K2O = magazzinoMovimentazione.K2O,
                .KgLordi = 0,
                .KgNetti = 0,
                .ListRifMovDettaglio = New List(Of Movimento_Dettaglio_Riferimento),
                .MatCod = Mat_Cod,
                .MatCodAlias = 0,
                .MatDes = Mat_Des,
                .ModuloGias = enum_Omni_Modulo_Generazione.Nessuno,
                .N = magazzinoMovimentazione.N,
                .NumConfezioni = 0,
                .NumContenitori = 0,
                .NumImballi = 0,
                .OrdineDet = 0,
                .P2O5 = magazzinoMovimentazione.P2O5,
                .Pendente = pendente,
                .Piva = piva,
                .Prezzo = 0,
                .PrezzoEffettivoKgL = 0,
                .PrezzoNetto = 0,
                .PrezzoRiferitoA = enum_PrezzoLivello.Udm_principale,
                .ProCod = Pro_Cod,
                .ProvvigioneAgente = 0,
                .ProvvigioneCapoArea = 0,
                .ProvvigionePercAgente = 0,
                .ProvvigionePercCapoArea = 0,
                .PuaRegolamento = PuaRegolamento,
                .Qta_Extra = 0,
                .Quantita = Qta,
                .Raccolte = New List(Of Movimento_Dettaglio),
                .RigheImpianti = New List(Of Contabilita_Impianti_Raccolta),
                .Riscontrati_PesoLordo = 0,
                .Riscontrati_PesoNetto = 0,
                .SaCod = 0,
                .ScontoAddiz1 = 0,
                .ScontoAddiz2 = 0,
                .ScontoAddiz3 = 0,
                .ScontoBase = 0,
                .ScontoCalcolato = 0,
                .ScontoModalita = 0,
                .Tara = 0,
                .Tipo_Associazione = 0,
                .UdM = magazzinoMovimentazione.udm.codice,
                .Udm_Cod_Extra = 0,
                .ValoreRiferimentoPrezzo = enum_EditImporto.PrezzoUnitario,
                .VegCod = 0,
                .jolly_int = 0,
                .Lotto = magazzinoMovimentazione.Lotto
                }

        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try

        Return contabilita_riga
    End Function

    ''' <summary>
    ''' Utilizzata nel giro Magazzino Esterno
    ''' Scrive nella Movimento_Dettaglio_Riferimento della operazione di Campagna il riferimento alle Operazioni di Carico nel magazzino dell'azienda che sta facendo l'Operazione di Campagna e
    ''' il riferimento alle Operazioni di Scarico nel Magazzino dell'azienda esterna (Magazzino con flag Uso da Terzi).
    ''' </summary>
    Private Shared Function Crea_Riferimento_Contabile(ByVal Agenda_Op_Campagna As Operazione_Agenda, ByVal Registrazioni As List(Of RisorsaRegistrazione), ByVal Id_Agenda_Rif As Integer, ByVal Lav_Cod_Rif As Integer, ByVal Cau_Mov_Rif As String, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim scritturaRiferimento As Boolean = False

        Dim Piva As String = Agenda_Op_Campagna.Piva

        Dim Sa_Cod As String = Agenda_Op_Campagna.Sa_Cod

        Dim Id_Agenda As Integer = Agenda_Op_Campagna.Id_Agenda

        Dim Lav_Cod As Integer = Agenda_Op_Campagna.Lav_Cod

        Dim Piva_Rif As String = Registrazioni(0).MagazziniMovimentazioni(0).Magazzino.primaryKey.centroAziendalePK.partitaIva

        Dim Sa_Cod_Rif As Integer = Registrazioni(0).MagazziniMovimentazioni(0).Magazzino.primaryKey.centroAziendalePK.codice

        Try
            Dim helperDettagliRiferimenti As New Agenda_Movimenti_Dettagli_Riferimenti_Helper()

            Dim detRifScarico As New Movimento_Dettaglio_Riferimento With {
                .Piva = Agenda_Op_Campagna.Piva,
                .Sa_Cod = Agenda_Op_Campagna.Sa_Cod,
                .Id_Agenda = Agenda_Op_Campagna.Id_Agenda,
                .Id_Mov = -1,
                .Id_Mov_Det = -1,
                .Lav_Cod = Agenda_Op_Campagna.Lav_Cod,
                .Cau_Mov = "",
                .Piva_Rif = Piva_Rif,
                .Sa_Cod_Rif = Sa_Cod_Rif,
                .Id_Agenda_Rif = Id_Agenda_Rif,
                .Id_Mov_Rif = -1,
                .Id_Mov_Det_Rif = -1,
                .Lav_Cod_Rif = Lav_Cod_Rif,
                .Cau_Mov_Rif = Cau_Mov_Rif,
                .Tipo_Associazione = 0
            }

            scritturaRiferimento = helperDettagliRiferimenti.Scrivi(detRifScarico, objParametri_Server)

        Catch ex As Exception
            Dim exception As String = ex.Message

            If Cau_Mov_Rif = CAU_SCARICO Then
                exception += String.Format("Errore Scrittura Riferimento Operazione di Scarico ( Piva {0}, Lav_Cod {1}, Id_Agenda {2}, Piva_Rif {3}, Lav_Cod_Rif {4}, Id_Agenda_Rif {5} )", Piva, Lav_Cod, Id_Agenda, Piva_Rif, Lav_Cod_Rif, Id_Agenda_Rif)
            ElseIf Cau_Mov_Rif = CAU_CARICO Then
                exception += String.Format("Errore Scrittura Riferimento Operazione di Carico ( Piva {0}, Lav_Cod {1}, Id_Agenda {2}, Piva_Rif {3}, Lav_Cod_Rif {4}, Id_Agenda_Rif {5} )", Piva, Lav_Cod, Id_Agenda, Piva_Rif, Lav_Cod_Rif, Id_Agenda_Rif)
            End If

            Throw New Exception(exception)

            Return scritturaRiferimento
        End Try

        Return scritturaRiferimento
    End Function

    Private Shared Sub Crea_ListAttivita_Carico_Scarico(ByVal risorse As List(Of Risorsa), ByRef ListAttivitaCarico As List(Of Attivita), ByRef ListAttivitaScarico As List(Of Attivita))

        For Each r As Risorsa In risorse

            Dim magazziniMovimentazioni As New List(Of RilevamentoDiMagazzino)

            Select Case r.classType
                Case ClassType.DettaglioTrattamento
                    magazziniMovimentazioni = CType(r, DettaglioTrattamento).MagazziniMovimentazioni
                Case ClassType.DettaglioFertilizzazione
                    magazziniMovimentazioni = CType(r, DettaglioFertilizzazione).MagazziniMovimentazioni
                Case ClassType.DettaglioSemina
                    magazziniMovimentazioni = CType(r, DettaglioSemina).MagazziniMovimentazioni
            End Select

            For Each m As RilevamentoDiMagazzino In magazziniMovimentazioni

                If Not IsNothing(m.registrazioniCollegate) AndAlso m.registrazioniCollegate.Count > 0 Then

                    For Each attivitaContabile As Attivita In m.registrazioniCollegate

                        Dim newAttivitaContabile = attivitaContabile.Clona()

                        newAttivitaContabile.risorse = New List(Of Risorsa)

                        For Each risorsaContabile As Risorsa In attivitaContabile.risorse

                            If risorsaContabile.classType = ClassType.RisorsaRegistrazione AndAlso
                               Not IsNothing(CType(risorsaContabile, RisorsaRegistrazione).MagazziniMovimentazioni) AndAlso
                                CType(risorsaContabile, RisorsaRegistrazione).MagazziniMovimentazioni.Count > 0 Then

                                Dim Flag_Carico As Boolean = False

                                Dim Flag_Scarico As Boolean = False

                                If attivitaContabile.job.primaryKey.codice = LAVCOD_CARICO OrElse attivitaContabile.job.primaryKey.codice = LAVCOD_BOLLA_RICEVUTA Then
                                    Flag_Carico = True
                                ElseIf attivitaContabile.job.primaryKey.codice = LAVCOD_SCARICO OrElse attivitaContabile.job.primaryKey.codice = LAVCOD_BOLLA_EMESSA Then
                                    Flag_Scarico = True
                                End If

                                Dim MagazziniMovimentazioni_Carico_Scarico = CType(risorsaContabile, RisorsaRegistrazione).MagazziniMovimentazioni

                                Dim newRisorsaContabile = CType(risorsaContabile, RisorsaRegistrazione).Clona()

                                newRisorsaContabile.MagazziniMovimentazioni = New List(Of RilevamentoDiMagazzino)

                                For Each MagazzinoMovimentazione As RilevamentoDiMagazzino In MagazziniMovimentazioni_Carico_Scarico

                                    Dim index_attivita_carico_scarico As Integer = 0

                                    Dim ListAttivita As New List(Of Attivita)

                                    If Flag_Carico Then
                                        ListAttivita = ListAttivitaCarico
                                    ElseIf Flag_Scarico Then
                                        ListAttivita = ListAttivitaScarico
                                    End If

                                    If Not IsNothing(ListAttivita) AndAlso ListAttivita.Count > 0 Then

                                        Dim index_stesso_prodotto_stesso_magazzino As Integer = -1

                                        Dim index_rilevamento_magazzino As Integer = -1

                                        For index_attivita_carico_scarico = 0 To ListAttivita.Count - 1

                                            Dim index_registrazione_carico_scarico As Integer = 0

                                            If Not IsNothing(ListAttivita(index_attivita_carico_scarico).risorse) AndAlso ListAttivita(index_attivita_carico_scarico).risorse.Count > 0 Then

                                                For index_registrazione_carico_scarico = 0 To ListAttivita(index_attivita_carico_scarico).risorse.Count - 1

                                                    If ListAttivita(index_attivita_carico_scarico).risorse(index_registrazione_carico_scarico).classType = ClassType.RisorsaRegistrazione Then

                                                        Dim Risorsa = CType(ListAttivita(index_attivita_carico_scarico).risorse(index_registrazione_carico_scarico), RisorsaRegistrazione)

                                                        If Not IsNothing(Risorsa.MagazziniMovimentazioni) AndAlso Risorsa.MagazziniMovimentazioni.Count > 0 Then

                                                            Dim Elem_Cod_Contabile As Integer = newRisorsaContabile.prodotto.elemCod

                                                            Dim Pro_Cod_Contabile As Integer = newRisorsaContabile.prodotto.codice

                                                            Dim Mat_Cod_Contabile As Integer = newRisorsaContabile.prodotto.codice

                                                            Dim Elem_Cod As Integer = Risorsa.prodotto.elemCod

                                                            Dim Pro_Cod As Integer = Risorsa.prodotto.codice

                                                            Dim Mat_Cod As Integer = Risorsa.prodotto.codice


                                                            index_stesso_prodotto_stesso_magazzino = Risorsa.MagazziniMovimentazioni.FindIndex(Function(movi) movi.Magazzino.primaryKey.codice = MagazzinoMovimentazione.Magazzino.primaryKey.codice AndAlso
                                                                                                                                                   movi.Magazzino.primaryKey.centroAziendalePK.codice = MagazzinoMovimentazione.Magazzino.primaryKey.centroAziendalePK.codice AndAlso
                                                                                                                                                    movi.Magazzino.primaryKey.centroAziendalePK.partitaIva = MagazzinoMovimentazione.Magazzino.primaryKey.centroAziendalePK.partitaIva AndAlso
                                                                                                                                                    movi.Lotto.ToUpper() = MagazzinoMovimentazione.Lotto.ToUpper() AndAlso
                                                                                                                                                    Utility.Controlla_Se_Stesso_Prodotto(Elem_Cod_Contabile, Pro_Cod_Contabile, Mat_Cod_Contabile, MagazzinoMovimentazione.Lotto, MagazzinoMovimentazione.Magazzino.primaryKey.centroAziendalePK.codice,
                                                                                                                                                    Elem_Cod, Pro_Cod, Mat_Cod, movi.Lotto, movi.Magazzino.primaryKey.centroAziendalePK.codice))


                                                            If index_stesso_prodotto_stesso_magazzino = -1 Then

                                                                index_rilevamento_magazzino = Risorsa.MagazziniMovimentazioni.FindIndex(Function(movi) movi.Magazzino.primaryKey.centroAziendalePK.codice = MagazzinoMovimentazione.Magazzino.primaryKey.centroAziendalePK.codice AndAlso
                                                                                                                                            movi.Magazzino.primaryKey.centroAziendalePK.partitaIva = MagazzinoMovimentazione.Magazzino.primaryKey.centroAziendalePK.partitaIva)

                                                            Else

                                                                If Flag_Carico Then
                                                                    CType(ListAttivitaCarico(index_attivita_carico_scarico).risorse(index_registrazione_carico_scarico), RisorsaRegistrazione).MagazziniMovimentazioni(index_stesso_prodotto_stesso_magazzino).Qta += MagazzinoMovimentazione.Qta
                                                                ElseIf Flag_Scarico Then
                                                                    CType(ListAttivitaScarico(index_attivita_carico_scarico).risorse(index_registrazione_carico_scarico), RisorsaRegistrazione).MagazziniMovimentazioni(index_stesso_prodotto_stesso_magazzino).Qta += MagazzinoMovimentazione.Qta
                                                                End If

                                                            End If

                                                            If index_rilevamento_magazzino > -1 OrElse index_stesso_prodotto_stesso_magazzino > -1 Then
                                                                Exit For
                                                            End If

                                                        End If

                                                    End If

                                                Next
                                            End If

                                            If index_rilevamento_magazzino > -1 OrElse index_stesso_prodotto_stesso_magazzino > -1 Then
                                                Exit For
                                            End If
                                        Next

                                        If index_stesso_prodotto_stesso_magazzino = -1 Then

                                            newRisorsaContabile.MagazziniMovimentazioni.Add(MagazzinoMovimentazione)

                                            If index_rilevamento_magazzino > -1 Then
                                                If Flag_Carico Then
                                                    ListAttivitaCarico(index_attivita_carico_scarico).risorse.Add(newRisorsaContabile)
                                                ElseIf Flag_Scarico Then
                                                    ListAttivitaScarico(index_attivita_carico_scarico).risorse.Add(newRisorsaContabile)
                                                End If
                                            Else
                                                newAttivitaContabile.risorse.Add(newRisorsaContabile)

                                                If Flag_Carico Then
                                                    ListAttivitaCarico.Add(newAttivitaContabile)
                                                ElseIf Flag_Scarico Then
                                                    ListAttivitaScarico.Add(newAttivitaContabile)
                                                End If
                                            End If
                                        End If

                                    Else
                                        newRisorsaContabile.MagazziniMovimentazioni.Add(MagazzinoMovimentazione)

                                        newAttivitaContabile.risorse.Add(newRisorsaContabile)

                                        If Flag_Carico Then
                                            ListAttivitaCarico.Add(newAttivitaContabile)
                                        ElseIf Flag_Scarico Then
                                            ListAttivitaScarico.Add(newAttivitaContabile)
                                        End If

                                    End If

                                Next

                            End If
                        Next
                    Next
                End If
            Next
        Next
    End Sub

    Private Function CheckSottoscrizioneSevizioQDCFromLista_Attivita(ByVal lista_Attivita As List(Of Attivita), ByVal objParametri_Server As AgronicaCoreParametri) As List(Of ErroreGias)

        Dim lista_Errori As New List(Of ErroreGias)

        If Not IsNothing(lista_Attivita) AndAlso lista_Attivita.Count > 0 Then

            Dim attivita As Attivita = lista_Attivita.Find(Function(a) Not IsNothing(a.centroAziendale) AndAlso Not IsNothing(a.centroAziendale.primaryKey))

            If Not IsNothing(attivita) AndAlso Not IsNothing(attivita.job) Then

                Dim Lav_Cod As Integer = attivita.job.primaryKey.codice

                Dim InfoOperazione As InfoOperazione = GetInfoOperazione(Lav_Cod, attivita.tipo)

                Dim Piva_Azienda As String = ""

                If Not IsNothing(attivita.centroAziendale) AndAlso Not IsNothing(attivita.centroAziendale.primaryKey) Then
                    Piva_Azienda = attivita.centroAziendale.primaryKey.partitaIva
                End If

                If Not String.IsNullOrEmpty(Piva_Azienda) AndAlso InfoOperazione.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.QuadernoDiCampagna Then

                    Dim Utility As New AgronicaCoreVarieBIZ.SottoscrizioneServizioQDC

                    If Not Utility.VerificaSottoscrizioneServizioQDC(Piva_Azienda, attivita.inizio, objParametri_Server) Then
                        lista_Errori.Add(generaErroreGias(ErroreGias_Severity.Bloccante, "", AgronicaCoreMapper.My.Resources.AgronicaCoreMapper.PassaggioAllePraticheInserimentoOpNonEffettuato, Key_Parametri_Aggiuntivi_Attivita.mostraWarning_CheckListaAttivita))
                    End If
                End If
            End If
        End If

        Return lista_Errori
    End Function

    Private Shared Sub CheckRilievo(ByVal InfoOperazione As InfoOperazione, ByVal attivita As Attivita, ByVal Lav_Cod As Integer,
                                         ByVal objParametri_Server As AgronicaCoreParametri, ByVal objParametri_Utenti As AgronicaCoreParametri, ByVal objParametri_Super_Server As AgronicaCoreParametri,
                                         ByRef messaggio As String, ByRef lista_Errori As List(Of ErroreGias))

        If InfoOperazione.IsRilievo Then

            If Lav_Cod = LAVCOD_FASI_FENOLOGICHE Then
                Dim dettagliRilievo As List(Of dettagli.DettaglioRilievo) = attivita.risorse.FindAll(Function(c) c.classType = costanti.ClassType.DettaglioRilievo).ConvertAll(Function(obj1) CType(obj1, dettagli.DettaglioRilievo))

                If Not IsNothing(dettagliRilievo) AndAlso dettagliRilievo.Count > 0 Then

                    Dim objControlli As New AgronicaControlli_2010.Rilievi

                    Dim Veg_Cod As Integer = GetVegCodFromUtilizzoTerreno(attivita.utilizzoTerreno)

                    If Veg_Cod > 0 Then

                        Dim FasiFenologiche As List(Of AgronicaCoreMetaSchemaBIZ.FaseFenologica) = objControlli.LetturaFasiFenologiche(Veg_Cod, False, True, True,
                                                                                                                                        objParametri_Server, objParametri_Super_Server, objParametri_Utenti)

                        If Not IsNothing(FasiFenologiche) AndAlso FasiFenologiche.Count > 0 Then

                            Dim FasiFenologicheNonCompatibili As New List(Of String)

                            For Each dettaglioRilievo As DettaglioRilievo In dettagliRilievo
                                Dim Index As Integer = FasiFenologiche.FindIndex(Function(c) Not IsNothing(dettaglioRilievo.faseFenologica) AndAlso
                                                                                     dettaglioRilievo.faseFenologica.codice = c.Cod_SS)

                                If Index = -1 Then

                                    Dim FasiFenologicaNonCompatibile As String = ""

                                    If Not IsNothing(dettaglioRilievo.esercizioCDC) AndAlso Not IsNothing(dettaglioRilievo.esercizioCDC.esercizio) AndAlso
                                        dettaglioRilievo.esercizioCDC.esercizio.descrizione <> "" Then

                                        FasiFenologicaNonCompatibile = "<br> - " & dettaglioRilievo.esercizioCDC.esercizio.descrizione
                                    End If

                                    If dettaglioRilievo.Descrizione <> "" Then
                                        FasiFenologicaNonCompatibile &= " " & dettaglioRilievo.Descrizione
                                    End If

                                    If Not IsNothing(dettaglioRilievo.DataOraRilievo) AndAlso dettaglioRilievo.DataOraRilievo <> AGRODATAINIZIO AndAlso dettaglioRilievo.DataOraRilievo <> AGRODATAFINE Then
                                        FasiFenologicaNonCompatibile &= " " & dettaglioRilievo.DataOraRilievo.ToShortDateString()
                                    End If

                                    FasiFenologicheNonCompatibili.Add(FasiFenologicaNonCompatibile)
                                End If
                            Next

                            If Not IsNothing(FasiFenologicheNonCompatibili) AndAlso FasiFenologicheNonCompatibili.Count > 0 Then

                                Dim DescrizioniFasiFenologiche As String = String.Join(" ", FasiFenologicheNonCompatibili)

                                messaggio = String.Format(My.Resources.AgronicaCoreMapper.FasiFenoNonCompatibili, GetDescrizioneFromUtilizzoTerreno(attivita.utilizzoTerreno), DescrizioniFasiFenologiche)

                                lista_Errori.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, attivita.job.descrizione, messaggio, Key_Parametri_Aggiuntivi_Attivita.mostraWarning_CheckListaAttivita))
                            End If

                        End If
                    End If
                End If
            End If


            If lista_Errori.Count = 0 AndAlso Utility.RilievoSenzaImpianti(attivita) Then

                If IsNothing(attivita.centroAziendale) OrElse attivita.centroAziendale.primaryKey.codice = 0 Then

                    messaggio = String.Format(Gias.SelezionareUnCentroAziendale)

                    lista_Errori.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, attivita.job.descrizione, messaggio, Key_Parametri_Aggiuntivi_Attivita.mostraWarning_CheckListaAttivita))
                End If

            End If

        End If
    End Sub

    Private Function GetImpiantoRilievoKey(ByVal attivita As Attivita, ByVal dettaglioRilievo As dettagli.DettaglioRilievo) As String

        Dim key As String = "0-0-0-0-0"

        If Not Utility.RilievoSenzaImpianti(attivita) Then
            key = dettaglioRilievo.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva & "-" &
                    dettaglioRilievo.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice & "-" &
                    dettaglioRilievo.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.codice & "-" &
                    dettaglioRilievo.esercizioCDC.esercizio.impiantoPK.codice & "-" &
                    dettaglioRilievo.esercizioCDC.esercizio.codice
        End If


        Return key
    End Function

    ''' <summary>
    ''' Controlla la validità delle macchine in base alla data di operazione
    ''' </summary>
    ''' <param name="MacchinaList"></param>
    ''' <param name="data_operazione"></param>
    ''' <param name="lista_Errori"></param>
    ''' <returns></returns>
    Private Shared Function CheckValiditaMacchine(ByVal MacchinaList As List(Of RisorsaMacchina),
                                                    ByVal data_operazione As Date,
                                                    ByRef lista_Errori As List(Of ErroreGias)) As Boolean


        Dim listMacchineNonValide As New List(Of String)

        If Not IsNothing(MacchinaList) AndAlso MacchinaList.Count > 0 Then

            For Each ris In MacchinaList
                If Not IsNothing(ris.macchina) AndAlso Not IsNothing(ris.macchina.validita) Then

                    Dim validitaInizio As Date = ris.macchina.validita.inizio

                    Dim validitaFine As Date = ris.macchina.validita.fine

                    If validitaInizio > data_operazione OrElse validitaFine < data_operazione Then

                        Dim class_desc As String = String.Empty

                        If Not IsNothing(ris.macchina.dettaglio_2) AndAlso Not String.IsNullOrEmpty(ris.macchina.dettaglio_2.descrizione) Then
                            class_desc = ris.macchina.dettaglio_2.descrizione
                        End If

                        Dim mac_desc As String = ris.macchina.descrizione

                        Dim ditta_des As String = String.Empty

                        If Not IsNothing(ris.macchina.marca) AndAlso Not String.IsNullOrEmpty(ris.macchina.marca.descrizione) Then
                            ditta_des = ris.macchina.marca.descrizione
                        End If

                        Dim modello As String = ris.macchina.modello

                        Dim objProfilazione As New AgronicaCoreProfilazioneBIZ.Profilazione_R

                        Dim descrizione As String = objProfilazione.GetDescrizioneMacchina(class_desc, mac_desc, ditta_des, modello, ris.macchina.scadenza_Taratura)

                        listMacchineNonValide.Add(descrizione & " " & String.Format("( {0} - {1} )", validitaInizio.ToShortDateString(), validitaFine.ToShortDateString()))
                    End If
                End If
            Next

        End If

        If listMacchineNonValide.Count > 0 Then

            Dim messaggio As String = String.Empty

            If listMacchineNonValide.Count = 1 Then
                messaggio = String.Format(My.Resources.AgronicaCoreMapper.MacchinaNonValidaInData, listMacchineNonValide(0), data_operazione.ToShortDateString())
            Else
                messaggio = String.Format(My.Resources.AgronicaCoreMapper.MacchineNonValideInData, data_operazione.ToShortDateString(), "<br> - " & String.Join("<br> - ", listMacchineNonValide))
            End If

            lista_Errori.Add(generaErroreGias(ErroreGias_Severity.Bloccante, "", messaggio, Key_Parametri_Aggiuntivi_Attivita.mostraWarning_CheckListaAttivita))

            Return False
        End If

        Return True

    End Function

    Private Sub SetDictionaryScarico(ByRef dictScarico As Dictionary(Of (Integer, Integer, String, String, Integer, Integer), (movimentoDettaglio As Movimento_Dettaglio, movimentoDestinazione As Movimento_Destinazione)),
                                                 ByVal MagazziniMovimentazioni As List(Of RilevamentoDiMagazzino), ByVal attivita As Attivita, ByVal risorsaProdotto As RisorsaProdotto,
                                                 ByVal agenda As Operazione_Agenda, ByVal infoOperazione As InfoOperazione)

        If MagazziniMovimentazioni IsNot Nothing Then
            For Each movimentoMagazzino In MagazziniMovimentazioni
                If movimentoMagazzino.Prodotto IsNot Nothing AndAlso
                    movimentoMagazzino.Magazzino IsNot Nothing AndAlso
                    movimentoMagazzino.Magazzino.primaryKey IsNot Nothing AndAlso
                    movimentoMagazzino.Magazzino.primaryKey.centroAziendalePK IsNot Nothing Then

                    Dim codiceProdotto As Integer = Math.Abs(movimentoMagazzino.Prodotto.codice)
                    Dim elem_cod As Integer = movimentoMagazzino.Prodotto.elemCod
                    Dim lotto As String = If(movimentoMagazzino.Lotto IsNot Nothing, movimentoMagazzino.Lotto, "")
                    Dim piva As String = movimentoMagazzino.Magazzino.primaryKey.centroAziendalePK.partitaIva
                    Dim sa_cod As Integer = movimentoMagazzino.Magazzino.primaryKey.centroAziendalePK.codice
                    Dim fabbricato_cod As Integer = movimentoMagazzino.Magazzino.primaryKey.codice

                    If Not dictScarico.ContainsKey((codiceProdotto, elem_cod, lotto.ToUpper, piva, sa_cod, fabbricato_cod)) Then

                        Dim Movimento_Dettaglio_Scarico As New Movimento_Dettaglio With {
                                .Id_Agenda = agenda.Id_Agenda,
                                .Piva = piva,
                                .Sa_Cod = sa_cod,
                                .Data = agenda.Data,
                                .Lav_Cod = agenda.Lav_Cod,
                                .Cau_Mov = CAU_SCARICO,
                                .Elem_Cod = elem_cod,
                                .Pro_Cod = codiceProdotto,
                                .Mat_Cod = 0,
                                .Lotto = lotto,
                                .Extra_Int = 0,
                                .Contabilizzato = NONCONTABILE,
                                .BaseCode = infoOperazione.BaseCode,
                                .TopCode = infoOperazione.TopCode,
                                .Udm_Cod = risorsaProdotto.unitaDiMisura.codice,
                                .Qta = 0 'DT: inizializzazione in vista di somma a parità di chiave
                            }

                        If infoOperazione.IsSemina Then
                            With Movimento_Dettaglio_Scarico
                                .Pro_Cod = 0
                                .Mat_Cod = codiceProdotto
                                .Anno = 1900
                                .Mov_Det_Des = "Utilizzo Di Materia Prima per Semina/Trapianto"
                                .Pendente = 2
                                .Extra_Str = GetExtraStr(risorsaProdotto, movimentoMagazzino, infoOperazione)
                            End With
                        End If

                        If infoOperazione.IsRaccolta Then
                            With Movimento_Dettaglio_Scarico
                                .Pro_Cod = 0
                                .Mat_Cod = codiceProdotto
                            End With
                        End If

                        If elem_cod = FORMULATI AndAlso
                            (attivita.job.primaryKey.codice = LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA OrElse attivita.job.primaryKey.codice = LAVCOD_REINNESCO_TRAPPOLE) Then
                            With Movimento_Dettaglio_Scarico
                                .Mov_Det_Des = "Scarico di Trappole"
                                .Pendente = 4
                            End With
                        End If

                        If elem_cod = INNESCHI Then

                            Dim Mov_Det_Des_Inneschi As String = ""

                            If attivita.job.primaryKey.codice = LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA Then
                                Mov_Det_Des_Inneschi = "Scarico di Inneschi Installazione Trappole"
                            ElseIf attivita.job.primaryKey.codice = LAVCOD_REINNESCO_TRAPPOLE Then
                                Mov_Det_Des_Inneschi = "Scarico di Inneschi Reinneschi Trappole"
                            End If

                            With Movimento_Dettaglio_Scarico
                                .Mov_Det_Des = Mov_Det_Des_Inneschi
                                .Pendente = 4
                            End With
                        End If

                        dictScarico.Add((codiceProdotto, elem_cod, lotto.ToUpper, piva, sa_cod, fabbricato_cod), (Movimento_Dettaglio_Scarico, New Movimento_Destinazione))

                    End If

                    dictScarico((codiceProdotto, elem_cod, lotto.ToUpper, piva, sa_cod, fabbricato_cod)).movimentoDettaglio.Qta += movimentoMagazzino.Qta

                    If movimentoMagazzino.Agenzia IsNot Nothing Then
                        Dim magazzinoesterno_cod As String = BuildMagazzinoEsternoKey(movimentoMagazzino.Agenzia, enum_MagazzinoEsterno_Tipo.Agenzia)
                        Dim magazzinoesterno_des As String = If(movimentoMagazzino.Agenzia IsNot Nothing, movimentoMagazzino.Agenzia.descrizione.Replace("|", ""), "")
                        Dim magazzinoesterno_dettagli As String = CDbl(movimentoMagazzino.Qta).ToString()

                        Dim magazzinoesterno_cod_list = dictScarico((codiceProdotto, elem_cod, lotto.ToUpper, piva, sa_cod, fabbricato_cod)).movimentoDestinazione.MagazzinoEsterno_Cod
                        Dim magazzinoesterno_des_list = dictScarico((codiceProdotto, elem_cod, lotto.ToUpper, piva, sa_cod, fabbricato_cod)).movimentoDestinazione.MagazzinoEsterno_Des
                        Dim magazzinoesterno_dettagli_list = dictScarico((codiceProdotto, elem_cod, lotto.ToUpper, piva, sa_cod, fabbricato_cod)).movimentoDestinazione.MagazzinoEsterno_Dettagli
                        If magazzinoesterno_cod_list <> "" Then
                            magazzinoesterno_cod_list &= "|"
                            magazzinoesterno_des_list &= "|"
                            magazzinoesterno_dettagli_list &= "|"
                        End If
                        magazzinoesterno_cod_list &= magazzinoesterno_cod
                        magazzinoesterno_des_list &= magazzinoesterno_des
                        magazzinoesterno_dettagli_list &= magazzinoesterno_dettagli

                        dictScarico((codiceProdotto, elem_cod, lotto.ToUpper, piva, sa_cod, fabbricato_cod)).movimentoDestinazione.MagazzinoEsterno_Cod = magazzinoesterno_cod_list
                        dictScarico((codiceProdotto, elem_cod, lotto.ToUpper, piva, sa_cod, fabbricato_cod)).movimentoDestinazione.MagazzinoEsterno_Des = magazzinoesterno_des_list
                        dictScarico((codiceProdotto, elem_cod, lotto.ToUpper, piva, sa_cod, fabbricato_cod)).movimentoDestinazione.MagazzinoEsterno_Dettagli = magazzinoesterno_dettagli_list
                    End If

                    If Not IsNothing(movimentoMagazzino.registrazioniCollegate) AndAlso movimentoMagazzino.registrazioniCollegate.Count > 0 Then

                        For Each attivita In movimentoMagazzino.registrazioniCollegate

                            Dim Lav_Cod As Integer = CInt(attivita.job.primaryKey.codice)

                            If Lav_Cod = LAVCOD_SCARICO OrElse Lav_Cod = LAVCOD_BOLLA_EMESSA Then

                                For Each risorsa In attivita.risorse
                                    If risorsa.classType = ClassType.RisorsaRegistrazione Then

                                        Dim risorsaRegistrazione = CType(risorsa, RisorsaRegistrazione)

                                        If Not IsNothing(risorsaRegistrazione) AndAlso Not IsNothing(risorsaRegistrazione.MagazziniMovimentazioni) AndAlso risorsaRegistrazione.MagazziniMovimentazioni.Count > 0 Then

                                            For Each movimentoMagazzinoRegistrazione In risorsaRegistrazione.MagazziniMovimentazioni

                                                If Not IsNothing(movimentoMagazzinoRegistrazione.Magazzino) AndAlso movimentoMagazzinoRegistrazione.Magazzino.usoDaTerzi Then
                                                    Dim magazzinoesterno_cod As String = BuildMagazzinoEsternoKey(movimentoMagazzinoRegistrazione.Magazzino, enum_MagazzinoEsterno_Tipo.Uso_da_Terzi)
                                                    Dim magazzinoesterno_des As String = If(movimentoMagazzinoRegistrazione.Magazzino IsNot Nothing, movimentoMagazzinoRegistrazione.Magazzino.descrizione, "")
                                                    Dim magazzinoesterno_dettagli As String = CDbl(movimentoMagazzinoRegistrazione.Qta).ToString()

                                                    Dim magazzinoesterno_cod_list = dictScarico((codiceProdotto, elem_cod, lotto.ToUpper, piva, sa_cod, fabbricato_cod)).movimentoDestinazione.MagazzinoEsterno_Cod
                                                    Dim magazzinoesterno_des_list = dictScarico((codiceProdotto, elem_cod, lotto.ToUpper, piva, sa_cod, fabbricato_cod)).movimentoDestinazione.MagazzinoEsterno_Des
                                                    Dim magazzinoesterno_dettagli_list = dictScarico((codiceProdotto, elem_cod, lotto.ToUpper, piva, sa_cod, fabbricato_cod)).movimentoDestinazione.MagazzinoEsterno_Dettagli
                                                    If magazzinoesterno_cod_list <> "" Then
                                                        magazzinoesterno_cod_list &= "|"
                                                        magazzinoesterno_des_list &= "|"
                                                        magazzinoesterno_dettagli_list &= "|"
                                                    End If
                                                    magazzinoesterno_cod_list &= magazzinoesterno_cod
                                                    magazzinoesterno_des_list &= magazzinoesterno_des
                                                    magazzinoesterno_dettagli_list &= magazzinoesterno_dettagli

                                                    dictScarico((codiceProdotto, elem_cod, lotto.ToUpper, piva, sa_cod, fabbricato_cod)).movimentoDestinazione.MagazzinoEsterno_Cod = magazzinoesterno_cod_list
                                                    dictScarico((codiceProdotto, elem_cod, lotto.ToUpper, piva, sa_cod, fabbricato_cod)).movimentoDestinazione.MagazzinoEsterno_Des = magazzinoesterno_des_list
                                                    dictScarico((codiceProdotto, elem_cod, lotto.ToUpper, piva, sa_cod, fabbricato_cod)).movimentoDestinazione.MagazzinoEsterno_Dettagli = magazzinoesterno_dettagli_list
                                                End If


                                            Next
                                        End If

                                    End If
                                Next

                            End If
                        Next
                    End If

                End If
            Next
        End If

    End Sub

    Private Sub UpdateQtaMagazzino(ByRef MagazziniMovimentazioni As List(Of RilevamentoDiMagazzino), ByVal ratio As Decimal)

        For Each movimentoMagazzino In MagazziniMovimentazioni
            movimentoMagazzino.Qta *= ratio

            'Aggiorno la Qta anche nelle eventuali RisorseRegistrazioni
            If Not IsNothing(movimentoMagazzino.registrazioniCollegate) AndAlso movimentoMagazzino.registrazioniCollegate.Count > 0 Then
                For Each attivitaContabile In movimentoMagazzino.registrazioniCollegate
                    If Not IsNothing(attivitaContabile) AndAlso attivitaContabile.risorse.Count > 0 Then
                        For Each risorsaContabile In attivitaContabile.risorse

                            If risorsaContabile.classType = ClassType.RisorsaRegistrazione Then

                                Dim movimentiMagazziniContabili = CType(risorsaContabile, RisorsaRegistrazione).MagazziniMovimentazioni

                                If Not IsNothing(movimentiMagazziniContabili) AndAlso movimentiMagazziniContabili.Count > 0 Then
                                    For Each movimentoMagazzinoContabile In movimentiMagazziniContabili
                                        movimentoMagazzinoContabile.Qta = movimentoMagazzino.Qta
                                    Next
                                End If
                            End If
                        Next
                    End If
                Next
            End If
        Next

    End Sub

    Public Shared Function CreaCaricoMagazzinoXPUA(ByVal Agenda_Op_Campagna As Operazione_Agenda,
                                                    ByVal Attivita_Op_Campagna As Attivita,
                                                    ByVal objParametri_Server As AgronicaCoreParametri,
                                                    ByVal objParametri_Utenti As AgronicaCoreParametri) As Boolean

        Dim XRisp As Boolean = False

        Try
            If Not IsNothing(Attivita_Op_Campagna) AndAlso Not IsNothing(Attivita_Op_Campagna.risorse) AndAlso Attivita_Op_Campagna.risorse.Count > 0 Then

                Dim risorseDettaglioFertilizzazione = Attivita_Op_Campagna.risorse.FindAll(Function(c) (c.classType = ClassType.DettaglioFertilizzazione)).ConvertAll(Function(obj1) CType(obj1, dettagli.DettaglioFertilizzazione))

                If Not IsNothing(risorseDettaglioFertilizzazione) AndAlso risorseDettaglioFertilizzazione.Count > 0 Then
                    Dim Registrazioni As New List(Of RisorsaRegistrazione)

                    For Each risorsa In risorseDettaglioFertilizzazione

                        Dim Registrazione As New RisorsaRegistrazione() With {
                            .prodotto = risorsa.prodotto,
                            .qta = risorsa.quantitaTotaleReale,
                            .unitaDiMisura = risorsa.unitaDiMisuraIndicata,
                            .MagazziniMovimentazioni = risorsa.MagazziniMovimentazioni
                        }

                        Registrazioni.Add(Registrazione)
                    Next

                    Dim Id_Agenda_Carico As Integer = 0

                    If Not IsNothing(Registrazioni) AndAlso Registrazioni.Count > 0 Then
                        XRisp = Crea_Operazione_Carico_Scarico(Agenda_Op_Campagna,
                                                                Registrazioni,
                                                                0,
                                                                LAVCOD_CARICO,
                                                                Attivita_Op_Campagna.job,
                                                                Attivita_Op_Campagna.disciplinare,
                                                                enum_Pendenza.AutoProduzione,
                                                                CAU_CARICO,
                                                                Id_Agenda_Carico,
                                                                objParametri_Server,
                                                                objParametri_Utenti)
                    End If

                End If

            End If

        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try

        Return XRisp
    End Function

    ''/ <summary>
    ''' Collega le trappole installate nell'operazione di installazione trappole con i reinneschi delle stesse nell'operazione di reinnesco trappole
    ''' </summary>
    ''' <param name="Agenda_Op_Campagna">Operazione Agenda di Reinnesco Trappole</param>
    ''' <param name="Attivita_Op_Campagna">Attività di Reinnesco Trappole</param>
    ''' <param name="objParametri_Server"></param>
    ''' <param name="objParametri_Utenti"></param>
    Public Shared Sub CollegaInstallazioneTrappole(ByVal Agenda_Op_Campagna As Operazione_Agenda,
                                                    ByVal Attivita_Op_Campagna As Attivita,
                                                    ByVal objParametri_Server As AgronicaCoreParametri,
                                                    ByVal objParametri_Utenti As AgronicaCoreParametri)

        Const nomeRoutine = "AgronicaCoreMapper.AttivitaToAgenda.CollegaInstallazioneTrappole()"

        Try

            Dim Movimenti_Dettagli_ReinnescoTrappole As New List(Of AgronicaCoreModello.OperazioneAgenda_Temp.Movimento_Dettaglio)

            If Not IsNothing(Agenda_Op_Campagna) Then
                Movimenti_Dettagli_ReinnescoTrappole = Get_Movimenti_Dettagli_Trappole(Agenda_Op_Campagna)
            End If

            Dim Movimenti_Dettagli_InstallazioneTrappole As New List(Of AgronicaCoreModello.OperazioneAgenda_Temp.Movimento_Dettaglio)

            If Not IsNothing(Attivita_Op_Campagna.attivitaCollegate) AndAlso Attivita_Op_Campagna.attivitaCollegate.Count > 0 Then

                Dim ListAgendeInstallazioneTrappole As New List(Of Operazione_Agenda)

                Dim Keys_InstallazioneTrappole = (From a In Attivita_Op_Campagna.attivitaCollegate
                                                  Where a.codice <> "0" AndAlso
                                                                   Not IsNothing(a.job) AndAlso a.job.getCodice() = LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA AndAlso
                                                                     Not IsNothing(a.centroAziendale) AndAlso Not IsNothing(a.centroAziendale.primaryKey) AndAlso a.centroAziendale.primaryKey.partitaIva <> ""
                                                  Select New With {.Piva = a.centroAziendale.primaryKey.partitaIva, .Id_Agenda = CInt(a.codice)})

                For Each Key_InstallazioneTrappole In Keys_InstallazioneTrappole
                    Dim objAgenda As New Agenda_Operazione_Helper
                    Dim agenda As Operazione_Agenda = objAgenda.Leggi(Key_InstallazioneTrappole.Piva, 0, Key_InstallazioneTrappole.Id_Agenda, 0, objParametri_Server)
                    Movimenti_Dettagli_InstallazioneTrappole.AddRange(Get_Movimenti_Dettagli_Trappole(agenda))
                Next
            End If

            If Not IsNothing(Movimenti_Dettagli_ReinnescoTrappole) AndAlso Movimenti_Dettagli_ReinnescoTrappole.Count > 0 AndAlso
               Not IsNothing(Movimenti_Dettagli_InstallazioneTrappole) AndAlso Movimenti_Dettagli_InstallazioneTrappole.Count > 0 Then

                For Each dettaglioReinnesco In Movimenti_Dettagli_ReinnescoTrappole
                    Dim index_trappola_installata = Movimenti_Dettagli_InstallazioneTrappole.FindIndex(Function(d) d.Piva = dettaglioReinnesco.Piva AndAlso
                                                                                                                   d.Sa_Cod = dettaglioReinnesco.Sa_Cod AndAlso
                                                                                                                   d.Pro_Cod = dettaglioReinnesco.Pro_Cod AndAlso
                                                                                                                   d.Elem_Cod = dettaglioReinnesco.Elem_Cod)
                    If index_trappola_installata > -1 Then

                        ScritturaRiferimentiReinnesco(dettaglioReinnesco, Movimenti_Dettagli_InstallazioneTrappole(index_trappola_installata), objParametri_Server)

                        ScritturaRiferimentiInstallazione(dettaglioReinnesco, Movimenti_Dettagli_InstallazioneTrappole(index_trappola_installata), objParametri_Server)

                    End If
                Next

            End If

        Catch ex As Exception
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        End Try
    End Sub

    Private Shared Function Get_Movimenti_Dettagli_Trappole(ByVal Agenda As Operazione_Agenda) As List(Of AgronicaCoreModello.OperazioneAgenda_Temp.Movimento_Dettaglio)
        Dim Movimenti_Dettagli_ReinnescoTrappole As New List(Of AgronicaCoreModello.OperazioneAgenda_Temp.Movimento_Dettaglio)

        If Not IsNothing(Agenda.Movimenti) AndAlso Agenda.Movimenti.Count > 0 Then

            Dim Movimento_ReinnescoTrappole = (From m In Agenda.Movimenti
                                               Where m.Cau_Mov = CAU_TRATTAMENTO
                                               Select m).FirstOrDefault()

            If Not IsNothing(Movimento_ReinnescoTrappole) Then
                Movimenti_Dettagli_ReinnescoTrappole.AddRange((From m In Movimento_ReinnescoTrappole.Movimenti_Dettagli
                                                               Where m.Pro_Cod <> 0 AndAlso m.Elem_Cod = FORMULATI
                                                               Select m).ToList())
            End If

        End If

        Return Movimenti_Dettagli_ReinnescoTrappole
    End Function

    Private Shared Sub ScritturaRiferimentiReinnesco(ByVal dettaglioReinnesco As Movimento_Dettaglio, ByVal Movimento_Dettaglio_Installazione As Movimento_Dettaglio, ByVal objParametri_Server As AgronicaCoreParametri)

        Try

            Dim helperDettagliRiferimenti As New Agenda_Movimenti_Dettagli_Riferimenti_Helper()

            Dim detRifReinnesco As New Movimento_Dettaglio_Riferimento With {
                .Piva = dettaglioReinnesco.Piva,
                .Sa_Cod = dettaglioReinnesco.Sa_Cod,
                .Id_Agenda = dettaglioReinnesco.Id_Agenda,
                .Id_Mov = dettaglioReinnesco.Id_Mov,
                .Id_Mov_Det = dettaglioReinnesco.Id_Mov_Det,
                .Lav_Cod = dettaglioReinnesco.Lav_Cod,
                .Cau_Mov = dettaglioReinnesco.Cau_Mov,
                .Piva_Rif = Movimento_Dettaglio_Installazione.Piva,
                .Sa_Cod_Rif = Movimento_Dettaglio_Installazione.Sa_Cod,
                .Id_Agenda_Rif = Movimento_Dettaglio_Installazione.Id_Agenda,
                .Id_Mov_Rif = Movimento_Dettaglio_Installazione.Id_Mov,
                .Id_Mov_Det_Rif = Movimento_Dettaglio_Installazione.Id_Mov_Det,
                .Lav_Cod_Rif = Movimento_Dettaglio_Installazione.Lav_Cod,
                .Cau_Mov_Rif = Movimento_Dettaglio_Installazione.Cau_Mov,
                .Tipo_Associazione = 0
            }

            helperDettagliRiferimenti.Scrivi(detRifReinnesco, objParametri_Server)

        Catch ex As Exception

            Dim exception As String = ex.Message

            exception += String.Format("Errore Scrittura Riferimento Reinnesco ( Id_Agenda_Reinnesco {0}, Id_Mov_Reinnesco {1}, Id_Mov_Det_Reinnesco {2}, Id_Agenda_Installazione {3}, Id_Mov_Installazione {4}, Id_Mov_Det_Installazione {5} )",
                                                                                dettaglioReinnesco.Id_Agenda, dettaglioReinnesco.Id_Mov, dettaglioReinnesco.Id_Mov_Det, Movimento_Dettaglio_Installazione.Id_Agenda, Movimento_Dettaglio_Installazione.Id_Mov, Movimento_Dettaglio_Installazione.Id_Mov_Det)

            Throw New Exception(exception)
        End Try
    End Sub

    Private Shared Sub ScritturaRiferimentiInstallazione(ByVal dettaglioReinnesco As Movimento_Dettaglio, ByVal Movimento_Dettaglio_Installazione As Movimento_Dettaglio, ByVal objParametri_Server As AgronicaCoreParametri)

        Try

            Dim helperDettagliRiferimenti As New Agenda_Movimenti_Dettagli_Riferimenti_Helper()

            Dim detRifInstallazione As New Movimento_Dettaglio_Riferimento With {
                .Piva = Movimento_Dettaglio_Installazione.Piva,
                .Sa_Cod = Movimento_Dettaglio_Installazione.Sa_Cod,
                .Id_Agenda = Movimento_Dettaglio_Installazione.Id_Agenda,
                .Id_Mov = Movimento_Dettaglio_Installazione.Id_Mov,
                .Id_Mov_Det = Movimento_Dettaglio_Installazione.Id_Mov_Det,
                .Lav_Cod = Movimento_Dettaglio_Installazione.Lav_Cod,
                .Cau_Mov = Movimento_Dettaglio_Installazione.Cau_Mov,
                .Piva_Rif = dettaglioReinnesco.Piva,
                .Sa_Cod_Rif = dettaglioReinnesco.Sa_Cod,
                .Id_Agenda_Rif = dettaglioReinnesco.Id_Agenda,
                .Id_Mov_Rif = dettaglioReinnesco.Id_Mov,
                .Id_Mov_Det_Rif = dettaglioReinnesco.Id_Mov_Det,
                .Lav_Cod_Rif = dettaglioReinnesco.Lav_Cod,
                .Cau_Mov_Rif = dettaglioReinnesco.Cau_Mov,
                .Tipo_Associazione = 0
            }

            helperDettagliRiferimenti.Scrivi(detRifInstallazione, objParametri_Server)

        Catch ex As Exception

            Dim exception As String = ex.Message

            exception += String.Format("Errore Scrittura Riferimento Installazione ( Id_Agenda_Reinnesco {0}, Id_Mov_Reinnesco {1}, Id_Mov_Det_Reinnesco {2}, Id_Agenda_Installazione {3}, Id_Mov_Installazione {4}, Id_Mov_Det_Installazione {5} )",
                                                                                dettaglioReinnesco.Id_Agenda, dettaglioReinnesco.Id_Mov, dettaglioReinnesco.Id_Mov_Det, Movimento_Dettaglio_Installazione.Id_Agenda, Movimento_Dettaglio_Installazione.Id_Mov, Movimento_Dettaglio_Installazione.Id_Mov_Det)

            Throw New Exception(exception)

        End Try

    End Sub


    ''/ <summary>
    ''' Controlla che non sia stato inserito più di una volta uno stesso innesco/avversita in una operazione di installazione trappole cattura di massa
    ''' </summary>
    ''' <param name="Lav_Cod"></param>
    ''' <param name="dettagliTrattamento"></param>
    ''' <param name="listErroriGias"></param>
    Private Shared Sub CheckInstallazioneTrappoleCatturaDiMassa(ByVal Lav_Cod As Integer, ByVal dettagliTrattamento As List(Of dettagli.DettaglioTrattamento), ByRef listErroriGias As List(Of ErroreGias))

        If Not IsNothing(listErroriGias) AndAlso listErroriGias.Count > 0 Then
            Return
        End If

        If (Lav_Cod = LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA OrElse Lav_Cod = LAVCOD_REINNESCO_TRAPPOLE) AndAlso
            Not IsNothing(dettagliTrattamento) AndAlso dettagliTrattamento.Count > 0 Then

            Dim KeyDettagliInneschi = dettagliTrattamento.Select(Function(d) As String
                                                                     Dim Av_Cod As Integer = 0
                                                                     Dim Av_Gru As Integer = 0
                                                                     STD_Utility.GetCodiciAvversita(d.avversitaGruppo, d.tipoFormulato, Av_Cod, Av_Gru)
                                                                     Return String.Join("_", Av_Cod, Av_Gru)
                                                                 End Function).ToList()

            If Not IsNothing(KeyDettagliInneschi) AndAlso KeyDettagliInneschi.Count > 0 Then

                Dim dettagliConInneschiUguali = (From d In KeyDettagliInneschi
                                                 Group d By Key = d Into Group
                                                 Where Group.Count() > 1
                                                 Select Key, Count = Group.Count()).ToList()

                If Not IsNothing(dettagliConInneschiUguali) AndAlso dettagliConInneschiUguali.Count > 0 Then
                    listErroriGias.Add(generaErroreGias(ErroreGias_Severity.Bloccante, "", My.Resources.AgronicaCoreMapper.NonPossibileInserirePiùVolteStessoInnesco, Key_Parametri_Aggiuntivi_Attivita.mostraWarning_CheckListaAttivita))
                End If

            End If

        End If

    End Sub

    ''' <summary>
    ''' Controlla se gli esercizi selezionati sono validi alla data di operazione.
    ''' Era successo che su Coldiretti erano stati salvati degli esercizi non validi alla data operazione (ticket: 205679)
    ''' </summary>
    ''' <param name="centridiCosto"></param>
    ''' <param name="data_operazione"></param>
    ''' <param name="objParametri_Server"></param>
    ''' <param name="lista_Errori"></param>
    ''' <returns></returns>
    Private Shared Function CheckEserciziCDC(ByVal EserciziCDC As List(Of centri_di_costo.EsercizioCDC), ByVal data_operazione As Date,
                                            ByVal objParametri_Server As AgronicaCoreParametri, ByRef lista_Errori As List(Of ErroreGias)) As Boolean

        Dim esericiziCorretti As Boolean = True

        If Not IsNothing(EserciziCDC) AndAlso EserciziCDC.Count > 0 Then

            Dim listchiavi As List(Of (String, Integer, Integer, Integer, Integer)) = EserciziCDC.Select(Function(esercizioCDC)

                                                                                                             Dim Piva As String = ""

                                                                                                             Dim Sa_Cod As Integer = 0

                                                                                                             Dim Appezza As Integer = 0

                                                                                                             Dim Id_Reg As Integer = 0

                                                                                                             Dim Progetto_Cod As Integer = 0

                                                                                                             If Not IsNothing(esercizioCDC.esercizio) Then

                                                                                                                 Progetto_Cod = esercizioCDC.esercizio.codice

                                                                                                                 If Not IsNothing(esercizioCDC.esercizio.impiantoPK) Then

                                                                                                                     Id_Reg = esercizioCDC.esercizio.impiantoPK.codice

                                                                                                                     If Not IsNothing(esercizioCDC.esercizio.impiantoPK.appezzamentoPK) Then

                                                                                                                         Appezza = esercizioCDC.esercizio.impiantoPK.appezzamentoPK.codice

                                                                                                                         If Not IsNothing(esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK) Then

                                                                                                                             Sa_Cod = esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice

                                                                                                                             Piva = esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva

                                                                                                                         End If
                                                                                                                     End If
                                                                                                                 End If

                                                                                                             End If

                                                                                                             Return (Piva, Sa_Cod, Appezza, Id_Reg, Progetto_Cod)

                                                                                                         End Function).ToList()

            If Not IsNothing(listchiavi) AndAlso listchiavi.Count > 0 Then

                Dim obj_Impresa_Progetti As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R

                Dim DT As DataTable = obj_Impresa_Progetti.Leggi_filtro_TempEsercizio(listchiavi, "", "", objParametri_Server, data_operazione, data_operazione)

                If Not IsNothing(DT) AndAlso DT.Rows.Count <> EserciziCDC.Count Then
                    Dim messaggio As String = String.Format(My.Resources.AgronicaCoreMapper.EserciziNonCompatibiliConDataOp, data_operazione.ToShortDateString())
                    lista_Errori.Add(generaErroreGias(ErroreGias_Severity.Bloccante, "", messaggio, Key_Parametri_Aggiuntivi_Attivita.mostraWarning_CheckListaAttivita))
                    esericiziCorretti = False
                End If
            End If

        End If

        Return esericiziCorretti
    End Function


    Private Shared Function CheckRaccolta(ByVal attivita As Attivita,
                                         ByVal mostraWarning_CheckListaAttivita As Boolean,
                                         ByVal parametriAggiuntiviList As List(Of Parametri_Aggiuntivi_Attivita),
                                         ByVal objParametri_Server As AgronicaCoreParametri,
                                         ByVal objParametri_Utenti As AgronicaCoreParametri,
                                         ByRef lista_Errori As List(Of ErroreGias)) As Boolean

        Dim raccoltaCorretta As Boolean = True

        '===================================
        '   CONTROLLO ANCHE GLI ESERCIZI INDICATI NELLA LISTA QuantitaSuImpianti   
        '-----------------------------------

        If attivita.risorse IsNot Nothing AndAlso attivita.risorse.Count > 0 Then

            Dim dettagliRaccolta As List(Of dettagli.DettaglioRaccolta) = attivita.risorse.FindAll(Function(c) (c.classType = ClassType.DettaglioRaccolta)).ConvertAll(Function(obj1) CType(obj1, dettagli.DettaglioRaccolta))

            If dettagliRaccolta IsNot Nothing AndAlso dettagliRaccolta.Count > 0 Then

                Dim distinctEserciziCDC As New List(Of EsercizioCDC)

                For Each dettaglioRaccolta In dettagliRaccolta

                    If dettaglioRaccolta.QuantitaSuImpianti IsNot Nothing AndAlso dettaglioRaccolta.QuantitaSuImpianti.Count > 0 Then

                        For Each qtaSuImpianti In dettaglioRaccolta.QuantitaSuImpianti

                            If qtaSuImpianti.esercizioCDC IsNot Nothing AndAlso qtaSuImpianti.esercizioCDC.esercizio IsNot Nothing Then
                                If distinctEserciziCDC.FindIndex(Function(e) e.esercizio IsNot Nothing AndAlso
                                                                             e.esercizio.GetKey = qtaSuImpianti.esercizioCDC.esercizio.GetKey) = -1 Then
                                    distinctEserciziCDC.Add(qtaSuImpianti.esercizioCDC)
                                End If
                            End If
                        Next
                    End If
                Next

                If distinctEserciziCDC IsNot Nothing AndAlso distinctEserciziCDC.Count > 0 Then
                    If Not CheckEserciziCDC(distinctEserciziCDC, attivita.inizio, objParametri_Server, lista_Errori) Then
                        raccoltaCorretta = False
                    End If
                End If
            End If
        End If


        '===================================
        '   CONTROLLO CARENZA RACCOLTA   
        '-----------------------------------

        If raccoltaCorretta AndAlso parametriAggiuntiviList IsNot Nothing AndAlso mostraWarning_CheckListaAttivita Then

            Dim EsercizioCDC As List(Of EsercizioCDC) = attivita.centriDiCosto.FindAll(Function(c) (c.classType = ClassType.EsercizioCDC)).ConvertAll(Function(obj1) CType(obj1, EsercizioCDC))

            Dim paramAggiuntiviList As List(Of Parametri_Aggiuntivi_Attivita) =
                parametriAggiuntiviList.FindAll(Function(c) c.key = Key_Parametri_Aggiuntivi_Attivita.list_DataCarenzaRaccolta_x_Impianto)
            Dim list_DataCarenzaRaccolta_x_Impianto As List(Of DataCarenzaRaccolta_x_Impianto) = JsonConvert.DeserializeObject(Of List(Of DataCarenzaRaccolta_x_Impianto))(paramAggiuntiviList(0).value)

            Dim messaggio_temp As String = ""
            Dim messaggio_carenza As String

            If list_DataCarenzaRaccolta_x_Impianto IsNot Nothing Then

                For Each carenza In list_DataCarenzaRaccolta_x_Impianto
                    If carenza.DataCarenza IsNot Nothing AndAlso attivita.inizio < carenza.DataCarenza Then
                        messaggio_temp &= "- " & carenza.App_Nome & ": " & carenza.CarenzaStr & NEWLINE
                    End If

                Next

                If messaggio_temp <> "" Then
                    messaggio_carenza = My.Resources.AgronicaCoreMapper.AttenzioneOperazioneNonRegistrata & NEWLINE
                    messaggio_carenza &= My.Resources.AgronicaCoreMapper.TempoCarenzaNonRispettatoPerAppezzamenti & NEWLINE
                    messaggio_carenza &= messaggio_temp

                    Dim severity As Integer = -1

                    getSeverityDaValoreImpostazione_0Warning_1WarningBloccante(severity, enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_RACCOLTA_CARENZA_NON_RISPETTATA, objParametri_Utenti)
                    lista_Errori.Add(generaErroreGias(severity, attivita.job.descrizione, messaggio_carenza, Key_Parametri_Aggiuntivi_Attivita.mostraWarning_CheckListaAttivita))
                    raccoltaCorretta = False
                End If

            End If

        End If

        Return raccoltaCorretta
    End Function
End Class
