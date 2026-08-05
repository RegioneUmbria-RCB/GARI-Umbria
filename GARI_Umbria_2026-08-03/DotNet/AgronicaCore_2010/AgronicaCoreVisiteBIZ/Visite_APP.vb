Imports System.Data.Entity
Imports System.Text
Imports System.Transactions
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModello
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreScadenziario_BIZ
Imports AgronicaCoreVisiteBIZ.My.Resources


Public Class Visite_APP
    Inherits AgronicaCoreDataProvider.DataProvider

    ' scarica visite da app
    Public Sub ScriviVisiteAPP(
        ByVal unid As String,
        Visite As List(Of APP_Visite),
        VisiteDettagli As List(Of APP_Visite_Dettagli),
        VisiteDestinazioni As List(Of APP_Visite_Destinazioni),
        objParametri_Server As AgronicaCoreParametri,
        Optional cancellazione As Boolean = False
    )

        Dim EFArrayToInsert As New ArrayList
        Dim EFArrayToUpdate As New ArrayList
        Dim EFArrayToDelete As New ArrayList

        Try

            For Each visiteDaScrivere In Visite
                visiteDaScrivere.ID = unid & "|" & visiteDaScrivere.Visita_Cod
                EFArrayToInsert.Add(visiteDaScrivere)
            Next

            For Each dettaglioDaScrivere In VisiteDettagli
                dettaglioDaScrivere.ID = unid & "|" & dettaglioDaScrivere.Visita_Dettaglio_Cod
                EFArrayToInsert.Add(dettaglioDaScrivere)
            Next

            For Each destinazioneDaScrivere In VisiteDestinazioni
                destinazioneDaScrivere.ID = unid & "|" & destinazioneDaScrivere.Visita_Destinazione_Cod
                EFArrayToInsert.Add(destinazioneDaScrivere)
            Next

            Dim err As String = ""
            If cancellazione Then
                err = Aggiorna_VisiteAPP(EFArrayToInsert, EFArrayToUpdate, EFArrayToDelete, objParametri_Server, unid)
            Else
                err = AggiornaVisiteAPP(EFArrayToInsert, EFArrayToUpdate, EFArrayToDelete, objParametri_Server)
            End If

            If err <> "" Then
                Throw New Exception(err)
            End If

        Catch ex As Exception

            Throw ex

        End Try

    End Sub

    ' Scrive visite app su tabelle frontiera
    Public Function Aggiorna_VisiteAPP(
                ByVal EFArrayToInsert As ArrayList,
                ByVal EFArrayToUpdate As ArrayList,
                ByVal EFArrayToDelete As ArrayList,
                ByRef objParametri As AgronicaCoreParametri,
                Optional ByVal unid As String = ""
                ) As String

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Ricette_W.Aggiorna_VisiteAPP()"
        Dim MessaggioErrore As String = ""

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Using scope As New TransactionScope()

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    If Not String.IsNullOrEmpty(unid) Then
                        GiasContext.APP_Visite.RemoveRange(GiasContext.APP_Visite.Where(Function(x) x.ID.Contains(unid)))
                        GiasContext.APP_Visite_Dettagli.RemoveRange(GiasContext.APP_Visite_Dettagli.Where(Function(x) x.ID.Contains(unid)))
                        GiasContext.APP_Visite_Destinazioni.RemoveRange(GiasContext.APP_Visite_Destinazioni.Where(Function(x) x.ID.Contains(unid)))
                        GiasContext.SaveChanges()
                    End If

                    For Each curOggetto In EFArrayToDelete
                        GiasContext.Entry(curOggetto).State = EntityState.Deleted
                        GiasContext.SaveChanges()
                    Next

                    For Each curOggetto In EFArrayToInsert
                        GiasContext.Entry(curOggetto).State = EntityState.Added
                        GiasContext.SaveChanges()
                    Next

                    For Each curOggetto In EFArrayToUpdate
                        GiasContext.Entry(curOggetto).State = EntityState.Modified
                        GiasContext.SaveChanges()
                    Next

                    ' COMIT Effettivo
                    scope.Complete()

                End Using

            End Using

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)

        End Try

        Return MessaggioErrore

    End Function

    ' Scrive visite app su tabelle frontiera
    Public Function AggiornaVisiteAPP(
                ByVal EFArrayToInsert As ArrayList,
                ByVal EFArrayToUpdate As ArrayList,
                ByVal EFArrayToDelete As ArrayList,
                ByRef objParametri As AgronicaCoreParametri
                ) As String

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

        Dim retries As Integer = 3
        Dim success As Boolean = True

        Dim NomeRoutine As String = "AgronicaCoreVisiteBIZ.Visite_APP.AggiornaVisiteAPP()"
        Dim MessaggioErrore As String = ""

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Using scope As New TransactionScope()

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    Dim idSeq As Integer = 0

                    For Each curOggetto In EFArrayToInsert

                        success = False
                        For i As Integer = 0 To retries - 1

                            Try

                                'GiasContext.AddObject(curOggetto.GetType.ToString.Replace("AgronicaCoreEntityFramework_POCO.", ""), curOggetto)
                                GiasContext.Entry(curOggetto).State = EntityState.Added
                                GiasContext.SaveChanges()
                                success = True

                                Exit For
                            Catch ex As Exception
                                Threading.Thread.Sleep(500) ' 500 milliseconds = 0.5 seconds
                            End Try

                        Next

                        ' Al primo errore evito di continuare le modifiche
                        If Not success Then
                            MessaggioErrore = "Non sono riuscito ad aggiornare i dati dopo " & retries & " tentativi."
                            Exit For
                        End If

                    Next

                    If success Then
                        For Each listFattVar In EFArrayToUpdate
                            'GiasContext.Attach(listFattVar)
                            'GiasContext.ObjectStateManager.ChangeObjectState(listFattVar, EntityState.Modified)
                            GiasContext.Entry(listFattVar).State = EntityState.Modified
                            GiasContext.SaveChanges()
                        Next

                        For Each listFattVar In EFArrayToDelete
                            'GiasContext.AttachTo(listFattVar.GetType.ToString.Replace("AgronicaCoreEntityFramework_POCO.", ""), listFattVar)
                            'GiasContext.DeleteObject(listFattVar)
                            GiasContext.Entry(listFattVar).State = EntityState.Deleted
                            GiasContext.SaveChanges()
                        Next

                        ' COMMIT Effettivo
                        scope.Complete()
                    End If

                End Using

            End Using

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)

        End Try

        Return MessaggioErrore

    End Function


    Public Function ImportaVisiteAPP(ByVal Piva As String,
                                     ByVal progressivoGias As Integer,
                                     ByRef msgFinale As StringBuilder,
                                     ByVal objParametri_Server As AgronicaCoreParametri,
                                     Optional ByVal guid As String = "",
                                     Optional ByRef riferimento As String = "",
                                     Optional ByRef rilievo As String = "",
                                     Optional ByRef dataCreazione As Date = Nothing,
                                     Optional ByRef usernameCreazione As String = "") As Boolean

        Dim NomeRoutine As String = "AgronicaCoreVisiteBIZ.Visite_APP.ImportaVisiteAPP()"
        Dim esitoFinale As Boolean = False
        Dim msgErr As New StringBuilder

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

        Dim visiteDaImportare As Integer = 0
        Dim visiteImportate As Integer = 0

        'Using scope As New TransactionScope()

        Try

            Dim visite = (From v In GiasContext.APP_Visite
                          Where v.Tipo_Visita = 0 AndAlso (Piva = "" OrElse v.Piva = Piva) AndAlso (guid = "" OrElse v.ID.Contains(guid))
                          Select v).ToList()

            If visite.Count > 0 Then

                visiteDaImportare = visite.Count

                For Each visita In visite

                    Dim messaggio_errore As String = ""
                    Dim id = visita.ID.Split("|".ToCharArray).First

                    Dim dettaglio = (From d In GiasContext.APP_Visite_Dettagli
                                     Where d.ID.StartsWith(id) AndAlso d.Visita_Cod = visita.Visita_Cod
                                     Select d).FirstOrDefault()

                    Dim destinazioni = (From d In GiasContext.APP_Visite_Destinazioni
                                        Where d.ID.StartsWith(id) AndAlso d.Visita_Cod = visita.Visita_Cod
                                        Select d).ToList()

                    If dettaglio.Id_Attivita <> 0 AndAlso String.IsNullOrEmpty(visita.Note) Then
                        Dim attivita = (From a In GiasContext.Attivita
                                        Where a.ID_Attivita = dettaglio.Id_Attivita
                                        Select a).FirstOrDefault()

                        visita.Note = attivita.Desc
                    End If

                    visita.Data_Modifica = Date.Now
                    visita.Username_Modifica = objParametri_Server.UsernameOperazione
                    Dim Visita_Cod As Integer = If(String.IsNullOrEmpty(riferimento), 0, CInt(riferimento))
                    If Visita_Cod > 0 AndAlso Not String.IsNullOrEmpty(usernameCreazione) Then
                        visita.Visita_Cod = Visita_Cod
                        visita.Data_Creazione = dataCreazione
                        visita.Username_Creazione = usernameCreazione
                    End If

                    Dim id_agenda_collegata As Integer = If(String.IsNullOrEmpty(rilievo), 0, CInt(rilievo))

                    ' se il salvataggio va a buon fine marco la visita come importata
                    If SalvaOperazioneAgenda(visita, dettaglio, destinazioni, progressivoGias, messaggio_errore, objParametri_Server, id_agenda_collegata) Then
                        visita.Tipo_Visita = 2
                        GiasContext.Entry(visita).State = EntityState.Modified
                        GiasContext.SaveChanges()
                        visiteImportate += 1
                    Else
                        visita.Note = messaggio_errore
                        visita.Tipo_Visita = 9
                        GiasContext.Entry(visita).State = EntityState.Modified
                        GiasContext.SaveChanges()
                        msgErr.Append(messaggio_errore & "<br>")
                    End If

                    If Not String.IsNullOrEmpty(guid) AndAlso id_agenda_collegata <> 0 Then
                        riferimento = CStr(id_agenda_collegata)
                    End If

                Next

                msgFinale.Append(String.Format(CoreVisiteBIZ.NrVisiteImportateCorrettamente, visiteImportate, visiteDaImportare))

                If msgErr.Length > 0 Then
                    msgFinale.Append("<br><br>" & CoreVisiteBIZ.Errori & "<br>" & msgErr.ToString)
                End If

            Else

                msgFinale.Append(CoreVisiteBIZ.NessunaVisitaDaImportare)

            End If

            esitoFinale = True

            'scope.Complete()
            'scope.Dispose()

        Catch ex As Exception

            esitoFinale = False
            Scrivi_LOG(objParametri_Server, NomeRoutine, ex.Message)
            msgFinale.Append(Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True))

            'scope.Dispose()

        Finally

            GiasContext.Dispose()

        End Try

        'End Using

        Return esitoFinale

    End Function

    Private Function SalvaOperazioneAgenda(ByRef visita As APP_Visite,
                                           ByRef dettaglio As APP_Visite_Dettagli,
                                           ByRef destinazioni As List(Of APP_Visite_Destinazioni),
                                           ByVal progressivoGias As Integer,
                                           ByRef messaggio_errore As String,
                                           ByRef objParametri_Server As AgronicaCoreParametri,
                                           ByRef Id_Agenda As Integer) As Boolean

        Dim res As Boolean = False

        Try

            '-----------------------------------------------------
            '----------- CONNESSIONE E TRANSAZIONE ---------------
            ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)
            '-----------------------------------------------------

            Dim objAgendaHelper As New Agenda_Operazione_Helper
            Dim listaOpCollegate As New List(Of Operazione_Agenda)
            Dim AgendaCollegata As Operazione_Agenda = Nothing

            If Id_Agenda > 0 Then
                AgendaCollegata = objAgendaHelper.Leggi(visita.Piva, 0, Id_Agenda, 0, objParametri_Server)
                If IsNothing(AgendaCollegata) Then
                    Throw New Exception(CoreVisiteBIZ.ImpossibileCreareOperazione)
                End If

                'i rilievi per fasi fenologiche possono essere composti da più agende
                If AgendaCollegata.Raccoglitore_Cod > 0 Then
                    Dim listaAgendeRaccoglitore = objAgendaHelper.LeggiLista_DaRaccoglitore(visita.Piva, AgendaCollegata.Raccoglitore_Cod, objParametri_Server)
                    listaOpCollegate.AddRange(listaAgendeRaccoglitore)
                Else
                    listaOpCollegate.Add(AgendaCollegata)
                End If

                Id_Agenda = 0
            End If

            'Creo l'oggetto visita
            Dim AgendaVisita As Operazione_Agenda = CreaOggettoAgendaVisita(progressivoGias, visita, dettaglio, destinazioni, listaOpCollegate, objParametri_Server)

            If IsNothing(AgendaVisita) Then
                Throw New Exception(CoreVisiteBIZ.ImpossibileCreareOperazione)
            End If

            Id_Agenda = objAgendaHelper.Scrivi(AgendaVisita, objParametri_Server, flagUsaOraReale:=True, documentoPrevisionale:=True)

            res = True

            ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server)

        Catch ex As Exception

            ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            messaggio_errore = ex.Message
            res = False

        Finally

            'chiudi connessione
            ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)

        End Try

        Return res

    End Function

    Private Function CreaOggettoAgendaVisita(ByVal progressivoGias As Integer,
                                             ByRef visita As APP_Visite,
                                             ByRef dettaglio As APP_Visite_Dettagli,
                                             ByRef destinazioni As List(Of APP_Visite_Destinazioni),
                                             ByRef listaOpCollegate As List(Of Operazione_Agenda),
                                             ByRef objParametri_Server As AgronicaCoreParametri) As Operazione_Agenda


        Dim Agenda As New Operazione_Agenda
        Dim BaseCode As Integer = 0
        Dim TopCode As Integer = 0

        Dim Utente As String = visita.Username_Creazione
        Dim Data As Date = AGRODATAINIZIO
        If visita.Data_Visita IsNot Nothing Then
            Data = visita.Data_Visita.Value
        End If
        Dim Ora As Date = Data.AddSeconds(-Data.Second).AddMilliseconds(-Data.Millisecond)
        Data = Data.Date

        '------------------------------------------------
        '----- Calcolo i valori di BaseCode e TopCode
        '------------------------------------------------
        Call Calcola_BaseCode_TopCode(BaseCode, TopCode, progressivoGias)


        '------------------------------------------------
        '----- AGENDA
        '------------------------------------------------

        Agenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
        Agenda.Id_Agenda = 0
        Agenda.Data = Data
        Agenda.Piva = visita.Piva
        Agenda.Sa_Cod = CInt(visita.Sa_Cod)
        Agenda.Lav_Cod = LAVCOD_VISITA
        Agenda.Des_Lib = dettaglio.Descrizione
        Agenda.Username_Creazione = Utente
        Agenda.BaseCode = BaseCode
        Agenda.TopCode = TopCode
        Agenda.Origine = enum_OrigineApp.GiasApp
        Agenda.Stato_Cod = enum_WWorflow_WAnagraficaStati.QdC_Eseguito
        Agenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
        Agenda.DaRemoto = 0 'essendo eseguita da APP, è necessariamente sul posto

        Agenda.Id_Attivita = 0
        If dettaglio.Id_Attivita IsNot Nothing Then
            Agenda.Id_Attivita = dettaglio.Id_Attivita.Value
        End If

        ' visite eseguite su app, riprendo i dati delle visite creata su web
        If visita.Visita_Cod > 0 Then
            Agenda.Id_Agenda = visita.Visita_Cod
            Agenda.Username_Creazione = visita.Username_Creazione
            Agenda.Data_Creazione = CDate(visita.Data_Creazione)
            Agenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica
        End If

        '------------------------------------------------
        '----- GPS
        '------------------------------------------------
        Dim wkt As String = ""
        If Not String.IsNullOrEmpty(visita.Posizione) Then
            Dim vWkt As String() = visita.Posizione.Split("|".ToCharArray)
            wkt = "POINT (" & vWkt(1).Trim() & ", " & vWkt(0).Trim() & ")"
            Agenda.GisWkt = wkt
            Agenda.GisWktGps = "1"
            Agenda.GisWktSistemaRiferimento = "-1"
            Agenda.GisLayerCod = enum_Gis_LayerElementiGrafici_std.Op_Agenda
            Agenda.GisTipoEntita_cod = enum_GIS2012_TipoEntita.OpAgenda
        End If

        '------------------------------------------------
        '------------------------------------------------
        '----- MOVIMENTO LAVORAZIONE
        '------------------------------------------------
        '------------------------------------------------
        Dim Movimento_Visita = New Movimento

        Movimento_Visita.Id_Agenda = Agenda.Id_Agenda
        Movimento_Visita.Piva = Agenda.Piva
        Movimento_Visita.Sa_Cod = Agenda.Sa_Cod
        Movimento_Visita.Data = Agenda.Data
        Movimento_Visita.Ora = Ora
        Movimento_Visita.Lav_Cod = Agenda.Lav_Cod
        Movimento_Visita.Cau_Mov = CAU_VISITE_ISPETTIVE
        Movimento_Visita.Mov_Desc = ""
        Movimento_Visita.Username_Creazione = Utente
        Movimento_Visita.BaseCode = BaseCode
        Movimento_Visita.TopCode = TopCode
        Movimento_Visita.OraFine = If(visita.Validita_Fine, AGRODATAFINE)
        Movimento_Visita.Num_Protocollo = -1  ' Da verificare

        Agenda.Movimenti.Add(Movimento_Visita)

        'Il movimento di dettaglio legato al cau mov 500 delle visite c'è sempre, eventualmente senza indicazione delle specie (se non sono state indicate).
        'serve anche come base per agganciare il mov_rif per le attività collegate

        Dim Movimento_Dettaglio_Visita = New Movimento_Dettaglio With {
                .Id_Agenda = Agenda.Id_Agenda,
                .Piva = Agenda.Piva,
                .Sa_Cod = Agenda.Sa_Cod,
                .Data = Agenda.Data,
                .Lav_Cod = Agenda.Lav_Cod,
                .Cau_Mov = CAU_VISITE_ISPETTIVE,
                .Mov_Det_Des = "",
                .Contabilizzato = NONCONTABILE,
                .Username_Creazione = Utente,
                .BaseCode = BaseCode,
                .TopCode = TopCode,
                .Veg_Cod = If(dettaglio.Dettaglio_VegCod IsNot Nothing, CInt(dettaglio.Dettaglio_VegCod), 0), 'Specie vegetale, se presente
                .Id_Cod = If(dettaglio.Dettaglio_IdCod IsNot Nothing, CInt(dettaglio.Dettaglio_IdCod), 0) 'Destinazione d'uso, se presente
            }

        Movimento_Visita.Movimenti_Dettagli.Add(Movimento_Dettaglio_Visita)


        '-------------------------------------------
        '----- MOVIMENTI DETTAGLI --> SPECIE ANIMALE
        '-------------------------------------------
        If dettaglio.Dettaglio_GenCod > 0 Then

            Dim Movimento_Dettaglio_Zoo = New Movimento_Dettaglio With {
                .Id_Agenda = Agenda.Id_Agenda,
                .Piva = Agenda.Piva,
                .Sa_Cod = Agenda.Sa_Cod,
                .Data = Agenda.Data,
                .Lav_Cod = Agenda.Lav_Cod,
                .Cau_Mov = CAU_VISITE_ISPETTIVE,
                .Mov_Det_Des = "",
                .Contabilizzato = NONCONTABILE,
                .Username_Creazione = Utente,
                .BaseCode = BaseCode,
                .TopCode = TopCode,
                .Gen_Cod = If(dettaglio.Dettaglio_GenCod IsNot Nothing, CInt(dettaglio.Dettaglio_GenCod), 0), 'Genere zoo, se presente
                .Spe_Cod = If(dettaglio.Dettaglio_SpeCod IsNot Nothing, CInt(dettaglio.Dettaglio_SpeCod), 0), 'Specie zoo, se presente
                .IPro_Cod = If(dettaglio.Dettaglio_IProCod IsNot Nothing, CInt(dettaglio.Dettaglio_IProCod), 0) 'Indirizzo produttivo zoo, se presente
            }

            Movimento_Visita.Movimenti_Dettagli.Add(Movimento_Dettaglio_Zoo)

        End If


        '------------------------------------------------
        '----- MOVIMENTI DESTINAZIONI 
        '------------------------------------------------

        If destinazioni.Count > 0 Then

            Dim xCalcolo_QD_SuperficieTotale As Decimal = (
                    From ST In destinazioni
                    Select CType(ST.Qta2, Decimal)
                ).Sum

            For Each mov_dettaglio In Movimento_Visita.Movimenti_Dettagli
                For Each destinazione In destinazioni

                    Dim qta As Decimal = Decimal.Parse(destinazione.Qta2.ToString)

                    Dim Movimento_Destinazione = New Movimento_Destinazione With {
                        .Id_Agenda = Agenda.Id_Agenda,
                        .Data = Agenda.Data,
                        .Piva = destinazione.Piva,
                        .Tipo = 0,
                        .Qta2 = qta,
                        .Username_Creazione = Utente,
                        .BaseCode = BaseCode,
                        .TopCode = TopCode,
                        .Sa_Cod = If(destinazione.Sa_Cod IsNot Nothing, destinazione.Sa_Cod.Value, Agenda.Sa_Cod),
                        .Appezza = If(destinazione.Appezza IsNot Nothing, destinazione.Appezza.Value, 0),
                        .Id_Destinazione = If(destinazione.Id_Reg IsNot Nothing, destinazione.Id_Reg.Value, 0),
                        .QuotaDistribuzione = If(xCalcolo_QD_SuperficieTotale <> 0, qta / xCalcolo_QD_SuperficieTotale, 0)
                    }

                    '------------------------------------------------
                    '----- GPS DESTINAZIONI
                    '------------------------------------------------
                    If Not String.IsNullOrEmpty(wkt) Then
                        Movimento_Destinazione.GisWkt = wkt
                        Movimento_Destinazione.GisWktGps = "1"
                        Movimento_Destinazione.GisWktSistemaRiferimento = "-1"
                        Movimento_Destinazione.GisLayerCod = enum_Gis_LayerElementiGrafici_std.Op_Agenda
                        Movimento_Destinazione.GisTipoEntita_cod = enum_GIS2012_TipoEntita.OpAgenda
                    End If

                    mov_dettaglio.Movimenti_Destinazioni.Add(Movimento_Destinazione)

                Next
            Next
        Else

            'prendo le destinazioni del rilievo associato (sono uguali in tutti i rilievi, basta prendere il primo
            If listaOpCollegate IsNot Nothing AndAlso listaOpCollegate.Count > 0 Then

                Dim rilievoCollegato = listaOpCollegate(0)

                Dim MovimentoOperazione As Movimento = rilievoCollegato.Movimenti.FindAll(Function(c) (CInt(c.Cau_Mov) >= 2000 And CInt(c.Cau_Mov) <= 3000)).FirstOrDefault

                If MovimentoOperazione IsNot Nothing AndAlso MovimentoOperazione.Movimenti_Dettagli IsNot Nothing _
                   AndAlso MovimentoOperazione.Movimenti_Dettagli.Count > 0 _
                   AndAlso MovimentoOperazione.Movimenti_Dettagli(0).Movimenti_Destinazioni IsNot Nothing _
                   AndAlso MovimentoOperazione.Movimenti_Dettagli(0).Movimenti_Destinazioni.Count > 0 Then

                    Dim xCalcolo_QD_SuperficieTotale As Decimal = (
                        From ST In MovimentoOperazione.Movimenti_Dettagli(0).Movimenti_Destinazioni
                        Select CType(ST.Qta2, Decimal)
                    ).Sum


                    Dim hashImpianti As New HashSet(Of String)

                    For Each destinazione In MovimentoOperazione.Movimenti_Dettagli(0).Movimenti_Destinazioni

                        Dim impiantoKey = destinazione.Piva &
                                             destinazione.Sa_Cod &
                                             destinazione.Appezza &
                                            destinazione.Id_Destinazione

                        If Not hashImpianti.Contains(impiantoKey) Then

                            hashImpianti.Add(impiantoKey)

                            Dim qta As Decimal = Decimal.Parse(destinazione.Qta2.ToString)

                            Dim Movimento_Destinazione = New Movimento_Destinazione With {
                                .Id_Agenda = Agenda.Id_Agenda,
                                .Data = Agenda.Data,
                                .Piva = destinazione.Piva,
                                .Tipo = 0,
                                .Qta2 = qta,
                                .Username_Creazione = Utente,
                                .BaseCode = BaseCode,
                                .TopCode = TopCode,
                                .Sa_Cod = destinazione.Sa_Cod,
                                .Appezza = destinazione.Appezza,
                                .Id_Destinazione = destinazione.Id_Destinazione,
                                .QuotaDistribuzione = If(xCalcolo_QD_SuperficieTotale <> 0, qta / xCalcolo_QD_SuperficieTotale, 0)
                            }

                            '------------------------------------------------
                            '----- GPS DESTINAZIONI
                            '------------------------------------------------
                            If Not String.IsNullOrEmpty(wkt) Then
                                Movimento_Destinazione.GisWkt = wkt
                                Movimento_Destinazione.GisWktGps = "1"
                                Movimento_Destinazione.GisWktSistemaRiferimento = "-1"
                                Movimento_Destinazione.GisLayerCod = enum_Gis_LayerElementiGrafici_std.Op_Agenda
                                Movimento_Destinazione.GisTipoEntita_cod = enum_GIS2012_TipoEntita.OpAgenda
                            End If

                            Movimento_Dettaglio_Visita.Movimenti_Destinazioni.Add(Movimento_Destinazione)

                        End If
                    Next

                End If

            End If

        End If

        '------------------------------------------------
        '----- ASSEGNATARIO
        '------------------------------------------------

        Dim assegnatario As Integer = 0
        Dim contatti_R As New AgronicaCoreAnagrafeDAL.Contatti_R
        Dim dtContatti = contatti_R.Leggi_Contatti_Utenti_Gias(objParametri_Server, visita.Operatore, "", CStr(enum_Rapporti_Contabili_Standard.Tecnico))
        If dtContatti.Rows.Count > 0 Then
            assegnatario = CInt(dtContatti.Rows(0).Item("Cod_RisUm"))

            If assegnatario <> 0 Then

                Dim movAssegnatario As New Movimento With {
                    .Cau_Mov = CAU_ASSEGNATARIO_VISITA,
                    .Cod_Risum = 0,
                    .Data = Agenda.Data,
                    .Id_Agenda = Agenda.Id_Agenda,
                    .Piva = Agenda.Piva,
                    .Sa_Cod = Agenda.Sa_Cod,
                    .Lav_Cod = 0,
                    .Mezzo = 0
                }

                Dim movDetAssegnatario As New Movimento_Dettaglio With {
                        .Data = Agenda.Data,
                        .Mat_Cod = assegnatario,
                        .Elem_Cod = CostantiPersonalizzate.ELEMCOD_MANODOPERA,
                        .Pro_Cod = 0,
                        .Udm_Cod = 0,
                        .Piva = Agenda.Piva,
                        .Sa_Cod = Agenda.Sa_Cod,
                        .Contabilizzato = NONCONTABILE
                    }

                movAssegnatario.Movimenti_Dettagli.Add(movDetAssegnatario)

                Agenda.Movimenti.Add(movAssegnatario)

            End If

        End If

        '----------------------------------------------------------
        '----- MOVIMENTI DETTAGLI RIFERIMENTI PER AGENDE COLLEGATE
        '----------------------------------------------------------

        For Each op As Operazione_Agenda In listaOpCollegate

            Dim Movimento_Dettaglio_Rif = New Movimento_Dettaglio_Riferimento With {
                .Id_Agenda = Agenda.Id_Agenda,
                .Piva = Agenda.Piva,
                .Sa_Cod = Agenda.Sa_Cod,
                .Id_Mov = -1,
                .Id_Mov_Det = -1,
                .Lav_Cod = Agenda.Lav_Cod,
                .Cau_Mov = "",
                .Id_Agenda_Rif = op.Id_Agenda,
                .Piva_Rif = op.Piva,
                .Sa_Cod_Rif = op.Sa_Cod,
                .Id_Mov_Rif = -1,
                .Id_Mov_Det_Rif = -1,
                .Lav_Cod_Rif = op.Lav_Cod,
                .Cau_Mov_Rif = CAU_ASSEGNATARIO_VISITA,
                .Qta = 0,
                .Username_Creazione = Utente,
                .BaseCode = Agenda.BaseCode,
                .TopCode = Agenda.TopCode
            }

            Movimento_Dettaglio_Visita.Movimenti_Dettagli_Riferimenti.Add(Movimento_Dettaglio_Rif)

        Next

        Return Agenda

    End Function

    Public Function ScriviDocumentiVisiteAPP(ByVal unid As String,
        ByVal Documenti As List(Of DocumentoPerScarico),
        objParametri_Server As AgronicaCoreParametri) As Boolean

        Dim esito As Boolean = True
        Dim errore As String = ""

        Try

            For Each Documento In Documenti

                Dim FileName As String = ""
                Dim FileAllegato As Byte() = Nothing
                Allegati.ConvertiAllegati(Documento.Allegati, FileName, FileAllegato)

                Dim DocumentoID As String = unid & "|" & Documento.Documento_Cod
                Dim RicettaOperazioneID As String = ""
                Dim RicettaDestinazioneID As String = ""

                If Documento.Ricetta_Operazione_Cod <> 0 Then
                    RicettaOperazioneID = unid & "|" & Documento.Ricetta_Operazione_Cod
                End If

                If Documento.Ricetta_Destinazione_Cod <> 0 Then
                    RicettaDestinazioneID = unid & "|" & Documento.Ricetta_Destinazione_Cod
                End If

                Dim allegatiScrivi As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_W
                esito = esito And allegatiScrivi.Scrivi_DocumentoAPP(
                    DocumentoID,
                    Documento.Piva, Documento.Documento_Cod, Documento.ID_Tipologia,
                    Documento.Descrizione, Documento.Data_Scadenza, Documento.Note,
                    FileName, RicettaOperazioneID, RicettaDestinazioneID, FileAllegato, objParametri_Server, Documento.Visita_Cod)
            Next

        Catch ex As Exception

            esito = False
            errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return esito

    End Function

End Class
