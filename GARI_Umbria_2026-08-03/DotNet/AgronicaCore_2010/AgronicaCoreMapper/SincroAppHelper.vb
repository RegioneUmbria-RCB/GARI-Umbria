Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Transactions
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreContabBIZ
Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModello
Imports AgronicaCoreModello.AppHelper
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreModelsSTD
Imports AgronicaCoreModelsSTD.attivita
Imports AgronicaCoreModelsSTD.attivita.centri_di_costo.CentroDiCosto
Imports AgronicaCoreModelsSTD.exceptions
Imports AgronicaCoreModelsSTD.pianiDiCampionamento
Imports AgronicaCoreModelsSTD.Zoo
Imports AgronicaCorePianidiCampionamentoBiz
Imports AgronicaCoreScadenziario
Imports AgronicaCoreScadenziario_BIZ
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreVarieDAL
Imports AgronicaCoreVisiteBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class SincroAppHelper

    Private ReadOnly objParametri_Super_Server As AgronicaCoreParametri
    Private ReadOnly objParametri_Server As AgronicaCoreParametri
    Private ReadOnly objParametri_Utenti As AgronicaCoreParametri

    Private cancellaDatiApp As Boolean = False
    Private messaggioErrore As String
    Private dataCreazione As Date
    Private usernameCreazione As String

    Public Sub New(ByVal objParametriSuperServer As AgronicaCoreParametri,
                   ByVal objParametriServer As AgronicaCoreParametri,
                   ByVal objParametriUtenti As AgronicaCoreParametri,
                   Optional ByVal cancellazione As Boolean = False)
        objParametri_Super_Server = objParametriSuperServer
        objParametri_Server = objParametriServer
        objParametri_Utenti = objParametriUtenti
        cancellaDatiApp = cancellazione
        messaggioErrore = ""
        dataCreazione = Date.Now
        usernameCreazione = objParametriServer.UtenteUsername
    End Sub

    ' imposta lingua utente
    Public Function ImpostaLingua() As String
        Dim leggiLingua As New Lingue_Read
        Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
        Dim linguaCodiceISO As String = If(dtLingua.Rows.Count > 0, dtLingua.Rows(0)("CodiceISO"), "it")
        Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)
        Return linguaCodiceISO
    End Function

    ' importa da tabella app nelle tabelle ricette/brogliaccio
    Public Function ImportaRicette(Optional ByVal guid As String = "", Optional ByRef riferimento As String = "") As Boolean
        Dim objRic_W As New AgronicaCoreContabBIZ.Ricette_W
        Dim esito = objRic_W.ImportaRicetteDaTabelleAPP(objParametri_Server, messaggioErrore, "", guid, riferimento)

        If Not esito AndAlso Not String.IsNullOrEmpty(messaggioErrore) Then
            Throw New Exception(messaggioErrore)
        End If

        Return esito
    End Function

    Public Function ImportaListaAttivitaFromTipo(ByVal tipo As enum_Dati_App, ByRef msgFinale As String, Optional ByVal guid As String = "", Optional ByRef riferimento As String = "") As Boolean

        Dim esito = ImportaAttivitaFromTipo(tipo, msgFinale, guid, riferimento)

        If Not esito AndAlso Not String.IsNullOrEmpty(msgFinale) Then
            Throw New Exception(msgFinale)
        End If

        Return esito

    End Function

    ' importa da tabella app direttamente in agenda (rilievi)
    Public Function ImportaAgenda(Optional ByVal guid As String = "", Optional ByRef riferimento As String = "", Optional ByRef posizione As String = "") As Boolean
        Dim destinazioni As New List(Of String)
        Dim objRic_W As New AgronicaCoreContabBIZ.Ricette_W
        Dim esito = objRic_W.ImportaAgendDaTabelleAPP(objParametri_Server, messaggioErrore, "", guid, riferimento, destinazioni)

        ' scrive posizione rilievo
        If esito AndAlso Not String.IsNullOrEmpty(posizione) AndAlso destinazioni.Count > 0 Then
            ScriviPosizione(posizione, destinazioni)
        End If

        Return esito
    End Function

    Public Function ImportaRilievo(ByRef unid As String, Optional ByRef riferimento As String = "", Optional ByVal visita As Boolean = False, Optional ByVal origine As enum_SistemiEsterni = enum_SistemiEsterni.GiasAPP) As Boolean

        Dim esito As Boolean = True

        Dim objAppDati As New APP_Dati_R
        Dim dtRilievi = objAppDati.Leggi_DatiAPP(unid, If(visita, enum_Dati_App.Visite, enum_Dati_App.Rilievi), True, "", "", objParametri_Server)

        For Each row As DataRow In dtRilievi.Rows

            Dim id_agenda As Integer = If(riferimento = "", 0, CInt(riferimento))
            Dim rilievo = JsonConvert.DeserializeObject(Of AgronicaCoreModelsSTD.attivita.Attivita)(row("Dati"))
            rilievo.codice = id_agenda 'DT: se vuoto forza la creazione di una nuova agenda, altrimenti va in modifica

            ' forzo il job del rilievo se composito altrimenti va in errore il mapping
            If rilievo.job.getTipo() = TipiJob.JOB_COMPOSITE Then
                Dim jobRilievo As JobComposito = rilievo.job
                rilievo.job = jobRilievo.lavorazione
            End If

            Dim mapAttivitaSuAgenda As New AttivitaToAgenda

            Dim listaRilievi As New List(Of AgronicaCoreModelsSTD.attivita.Attivita)({rilievo})
            mapAttivitaSuAgenda.SplitRilievo(listaRilievi, objParametri_Server, objParametri_Utenti)

            For Each rilievo In listaRilievi
                Dim agenda As Operazione_Agenda = mapAttivitaSuAgenda.MappaAttivitaToAgenda(rilievo, objParametri_Super_Server, objParametri_Server, objParametri_Utenti, False)

                Dim objAgendaScrivi As New Agenda_Operazione_Helper
                id_agenda = objAgendaScrivi.Scrivi(agenda, objParametri_Server, documentoPrevisionale:=True, origine:=origine)

                If id_agenda <> 0 Then
                    riferimento = CStr(id_agenda)
                Else
                    esito = False
                End If
            Next

        Next

        Return esito

    End Function

    ' importa ricetta in agenda usando il modello attività
    Public Function ImportaAttivita(Optional ByVal guid As String = "", Optional ByRef riferimento As String = "", Optional ByVal importAgenda As String = "", Optional ByVal eseguiVerifiche As Boolean = False, Optional ByVal tipo As enum_Dati_App = Nothing, Optional ByVal origine As enum_SistemiEsterni = enum_SistemiEsterni.GiasAPP) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreMapper.SincroAppHelper.ImportaAttivita()"

        Dim risultatoImportazione As Boolean = True
        Dim ricettaOperazioneCod As String = String.Empty

        Try

            Dim opzioniTransazione As New TransactionOptions()
            opzioniTransazione.IsolationLevel = IsolationLevel.ReadUncommitted
            opzioniTransazione.Timeout = TransactionManager.MaximumTimeout

            Using scopeImportazione As New TransactionScope(TransactionScopeOption.Required, opzioniTransazione)

                ' riferimento a agenda|brogliaccio
                Dim Id_Agenda As Integer = 0
                Dim rif As String() = riferimento.Split("|")
                If rif.Length > 0 AndAlso rif(0) <> "" Then
                    Id_Agenda = CInt(rif(0))
                End If

                If DictOrigineAttivitaToImport.ContainsKey(tipo) Then
                    Dim msg = ""
                    risultatoImportazione = ImportaListaAttivitaFromTipo(tipo, msg, guid, riferimento)
                Else
                    ' importa ricette da tabelle app
                    risultatoImportazione = ImportaRicette(guid, riferimento)
                End If


                If risultatoImportazione AndAlso riferimento <> "" AndAlso Not String.IsNullOrEmpty(importAgenda) Then

                    'restituisce le ricette non ancora riportate in agenda
                    Dim objAppDati As New APP_Dati_R
                    Dim dtRicette = objAppDati.Leggi_RicetteAPPDaImportareInAgenda(objParametri_Server, guid, importAgenda)

                    ' Aggiunta gestione raccoglitore per attivita miste app
                    Dim seq As New Agro_Sequenze()
                    Dim Raccoglitore_Cod As Integer = 0
                    If Not String.IsNullOrEmpty(guid) AndAlso dtRicette.Rows.Count > 1 Then
                        'Raccoglitore_Cod = seq.Agronica_SequenzaTabelle_NuovoID("raccoglitore", objParametri_Server)
                        'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
                        Raccoglitore_Cod = seq.NuovoId_Tabella("raccoglitore", 0, UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri_Server)
                    End If

                    'ogni ricetta_operazione viene letta, trasformata in nuovo modello e scritta in agenda (con gestione verifica) 
                    For Each row As DataRow In dtRicette.Rows

                        ' forzo la creazione dell'agenda per le miste
                        If Raccoglitore_Cod <> 0 Then
                            Id_Agenda = 0
                        End If

                        ricettaOperazioneCod = row.Item("Ricetta_Operazione_Cod")

                        Dim gefutils As New Gias_EF_Utility
                        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
                        Dim db As New Gias_DeveloperServer_Entities(EFConnString)

                        Dim ricetta_operazione As Ricette_Operazioni = EFRicette.ReadRicettaOperazione(db, row.Item("Ricetta_SuperUser"), row.Item("Ricetta_Operazione_Cod"))

                        Dim mapRicettaSuAttivita As New RicettaToAttivita
                        Dim attivita As AgronicaCoreModelsSTD.attivita.Attivita = mapRicettaSuAttivita.RicettaOperazioneSuAttivita(ricetta_operazione, listParametriAggiuntivi:=Nothing, verbose:=False, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
                        attivita.codice = Id_Agenda 'DT: se vuoto forza la creazione di una nuova agenda, altrimenti va in modifica

                        Dim mapAttivitaSuAgenda As New AttivitaToAgenda
                        Dim agenda As Operazione_Agenda = mapAttivitaSuAgenda.MappaAttivitaToAgenda(attivita, objParametri_Super_Server, objParametri_Server, objParametri_Utenti, eseguiVerifiche)
                        agenda.Raccoglitore_Cod = Raccoglitore_Cod

                        Dim objAgendaScrivi As New Agenda_Operazione_Helper
                        Id_Agenda = objAgendaScrivi.Scrivi(agenda, objParametri_Server, documentoPrevisionale:=True, origine:=origine)

                        Dim objRicettaxAgenda As New RicettexAgenda_W
                        If Not objRicettaxAgenda.Scrivi(row.Item("Ricetta_Cod"), row.Item("Ricetta_Operazione_Cod"), Id_Agenda, AGRODATAINIZIO, AGRODATAFINE, objParametri_Server) Then
                            Throw New GiasException(My.Resources.AgronicaCoreMapper.ErroreAggancioRicettaAgenda)
                        End If

                        ' Importa i costi inseriti da APP collegati all'agenda
                        If Id_Agenda <> 0 Then
                            Dim msgFinale As New StringBuilder
                            Dim xLettura As New AgronicaCoreContabBIZ.CDG_APP
                            Dim Piva As String = attivita.centroAziendale.primaryKey.partitaIva
                            Dim esitoFinale As Boolean = xLettura.ImportaCDGDaTabelleAPP(Piva, Id_Agenda, msgFinale, objParametri_Server, objParametri_Utenti)
                            If Not esitoFinale Then
                                messaggioErrore = msgFinale.ToString
                                Throw New Exception(msgFinale.ToString)
                            End If
                        End If

                    Next

                    If Not String.IsNullOrEmpty(guid) Then
                        riferimento = CStr(Id_Agenda) & "|" & riferimento
                    End If

                End If

                ' fix per riferimento a solo brogliaccio
                If Not String.IsNullOrEmpty(guid) AndAlso riferimento.Split("|").Length = 1 Then
                    riferimento = "0|" & riferimento
                End If

                scopeImportazione.Complete()

            End Using

        Catch ex As Exception
            riferimento = ""
            risultatoImportazione = False
            messaggioErrore = String.Format(My.Resources.AgronicaCoreMapper.ErroreRicettaOperazioneX, ricettaOperazioneCod) & ":  " & ex.Message

            Dim log As New LogProvider
            log.Scrivi_LOG(objParametri_Server, NomeRoutine, messaggioErrore)

            'TODO: verificare se gestire meglio il messaggio di errore (mail? tabelle di frontiera?)
            'TODO_DT: rimane solo su app_dati l'errore ora
        End Try

        Return risultatoImportazione

    End Function

    Public Function CancellaAttivita(ByVal guid As String, Optional ByVal origine As enum_SistemiEsterni = enum_SistemiEsterni.GiasAPP) As Boolean

        If String.IsNullOrEmpty(guid) Then
            Return False
        End If

        Dim esito As Boolean = True
        Dim objAppDati As New APP_Dati_R
        Dim dtAttivita = objAppDati.Leggi_AttivitaAPP(objParametri_Server, guid)
        Dim cancellaAgenda As Boolean = True

        For Each row As DataRow In dtAttivita.Rows

            Try

                ' cancella agenda
                If cancellaAgenda AndAlso Not IsDBNull(row.Item("Id_Agenda")) AndAlso row.Item("Id_Agenda") <> 0 Then

                    Dim messaggio As String = ""
                    Dim objParametriAgenda As New ParametriAgenda(False) With {
                       .Piva = row.Item("Piva"),
                       .Id_Agenda = row.Item("Id_Agenda"),
                       .Lav_Cod = row.Item("Lav_Cod"),
                       .Tipo_Operazione = enum_TipoOperazioneDB.Cancellazione
                    }

                    ' cancella solo la prima agenda in caso di attivita miste dal moento che cancella anche le collegate
                    If GestisciCancellazione(objParametriAgenda, objParametri_Server, messaggio, True, objParametri_Utenti:=objParametri_Utenti, origine:=origine) Then
                        cancellaAgenda = False
                    End If

                End If

                ' cancella ricetta
                If esito AndAlso Not IsDBNull(row.Item("Ricetta_Cod")) Then
                    Dim Ricetta_Cod As Integer = row.Item("Ricetta_Cod")
                    Dim Ricetta_Operazione_Cod As Integer = row.Item("Ricetta_Operazione_Cod")
                    Dim r_Read As New AgronicaCoreContabBIZ.Ricette_R
                    Dim r_Write As New AgronicaCoreContabBIZ.Ricette_W
                    Dim DatiRicetta As String = r_Read.Ricetta_Leggi(Ricetta_Cod, "", 0, 0, 0, True, objParametri_Server)
                    If DatiRicetta <> "" Then
                        esito = r_Write.Ricetta_Scrivi(DatiRicetta, Ricetta_Cod, objParametri_Server)
                    End If
                End If

            Catch ex As Exception
                esito = False
            End Try

        Next

        Return esito

    End Function

    Public Function ImportaAttivitaCdG(Optional ByVal guid As String = "", Optional ByRef riferimento As String = "") As Boolean
        Dim objCDGAPP As New CDG_APP
        Dim msgFinale As New StringBuilder
        Dim esito = objCDGAPP.ImportaCDGDaTabelleAPP("", 0, msgFinale, objParametri_Server, objParametri_Utenti, guid, riferimento)
        messaggioErrore = msgFinale.ToString
        Return esito
    End Function

    Public Function ImportaAttivitaZoo(Optional ByVal unid As String = "", Optional ByRef riferimento As String = "") As Boolean

        Dim esito As Boolean = True
        Dim objAppDati As New APP_Dati_R
        Dim dtAttivitaZoo = objAppDati.Leggi_DatiAPP(unid, enum_Dati_App.AttivitaZoo, True, "", "", objParametri_Server)
        Dim objAttivitaZootecnicaScrivi As New AgronicaCoreMapper.AttivitaZootecnicaToAgenda

        For Each row As DataRow In dtAttivitaZoo.Rows

            Dim id_agenda As Integer = If(riferimento = "", 0, CInt(riferimento))
            Dim attivita = JsonConvert.DeserializeObject(Of AgronicaCoreModelsSTD.attivita.Attivita)(row("Dati"))
            id_agenda = objAttivitaZootecnicaScrivi.ScriviAttivitaZootecnicaToAgenda(attivita, objParametri_Server, idAgenda:=id_agenda, origine:="APP")

            If id_agenda <> 0 Then
                riferimento = CStr(id_agenda)
            Else
                esito = False
            End If

        Next

        Return esito

    End Function

    Public Function ImportaAttivitaMovimentoGruppi(Optional ByVal unid As String = "", Optional ByRef riferimento As String = "") As Boolean

        Dim esito As Boolean = True
        Dim objAppDati As New APP_Dati_R
        Dim dtAppDati = objAppDati.Leggi_DatiAPP(unid, enum_Dati_App.MovimentiGruppo, True, "", "", objParametri_Server)
        Dim objAttivitaZootecnicaScrivi As New AgronicaCoreMapper.AttivitaZootecnicaToAgenda

        'Creo l'oggetto attività
        Dim attivita As New AgronicaCoreModelsSTD.attivita.Attivita

        Dim id_agenda As Integer = If(riferimento = "", 0, CInt(riferimento))
        Dim spostamento = JsonConvert.DeserializeObject(Of InData.Zoo.SpostamentoGruppi)(dtAppDati(0)("Dati"))

        Dim piva As String = spostamento.Piva
        Dim saCod As Integer = spostamento.SaCod
        Dim staNum As Integer = spostamento.StaNum

        attivita.codice = spostamento.Codice
        attivita.inizio = spostamento.Inizio
        attivita.fine = AGRODATAFINE
        attivita.centroAziendale = New anagrafiche.CentroAziendale With {
            .primaryKey = New anagrafiche.CentroAziendale.PK With {
                .partitaIva = piva,
                .codice = spostamento.SaCod
                }
            }
        attivita.fabbricatoCod = staNum
        attivita.centriDiCosto = New List(Of AgronicaCoreModelsSTD.attivita.centri_di_costo.CentroDiCosto)

        For Each gruppoOrigine In spostamento.GruppiOrigine
            Dim ZooBIZ As New AgronicaCoreAnagrafeDAL.Zoo_Animali
            Dim dtGiacenzeZoo = ZooBIZ.Leggi_Giacenze(piva, saCod, staNum, gruppoOrigine.CodiceGruppo, 0, Date.Now, objParametri_Server)

            If Not IsNothing(dtGiacenzeZoo) AndAlso dtGiacenzeZoo.Rows.Count > 0 Then

                For Each row In dtGiacenzeZoo.Rows
                    'Dim capo As New AgronicaCoreModelsSTD.anagrafiche.CapoAnimale(row("Piva"), row("Cod_Progetto"), row("Matricola"))
                    Dim cdcAnimale = New AgronicaCoreModelsSTD.attivita.centri_di_costo.CapoAnimaleCDC()
                    Dim gruppoIngresso = New anagrafiche.SottogruppoStalla With {
                        .codice = spostamento.GruppoDestinazione.CodiceGruppo,
                        .nome = spostamento.GruppoDestinazione.DescrizioneGruppo,
                        .stallaPK = New anagrafiche.FabbricatoLight.PK(piva, saCod, staNum)
                        }
                    Dim gruppoUscita = New anagrafiche.SottogruppoStalla With {
                        .codice = gruppoOrigine.CodiceGruppo,
                        .nome = gruppoOrigine.DescrizioneGruppo,
                        .stallaPK = New anagrafiche.FabbricatoLight.PK(piva, saCod, staNum)
                        }
                    cdcAnimale.codice = New CodeType(saCod)
                    cdcAnimale.sottogruppoStalla_ingresso = gruppoIngresso
                    cdcAnimale.sottogruppoStalla_uscita = gruppoUscita
                    cdcAnimale.capoAnimale = New AgronicaCoreModelsSTD.anagrafiche.CapoAnimale(row("Piva"), row("Cod_Animale"), row("Matricola"))
                    attivita.centriDiCosto.Add(cdcAnimale)
                Next

            End If

        Next

        attivita.job = New AgronicaCoreModelsSTD.attivita.Zootecnia(LAVCOD_SPOSTAMENTI_ZOO, "")

        id_agenda = objAttivitaZootecnicaScrivi.ScriviAttivitaZootecnicaToAgenda(attivita, objParametri_Server, idAgenda:=id_agenda, origine:="APP")
        If id_agenda <> 0 Then
            riferimento = CStr(id_agenda)
        Else
            esito = False
        End If
        Return esito
    End Function

    Public Function ImportaPianoCampionamento(Optional ByVal unid As String = "", Optional ByRef riferimento As String = "") As Boolean

        Dim esito As Boolean = True
        Dim objAppDati As New APP_Dati_R
        Dim dtPDCZoo = objAppDati.Leggi_DatiAPP(unid, enum_Dati_App.PianoCampionamento, True, "", "", objParametri_Server)

        ' ricavo base e top per creare PDC
        Dim progressivo As Integer
        Dim baseCode As Integer = 0
        Dim topCode As Integer = 0
        CodiciProgressivi.Trova_Base_e_Top(progressivo, baseCode, topCode, objParametri_Utenti, objParametri_Server)

        For Each row As DataRow In dtPDCZoo.Rows

            Dim id_pdc As Integer = If(riferimento = "", 0, CInt(riferimento))
            Dim pdc As PianoDiCampionamento = JsonConvert.DeserializeObject(Of PianoDiCampionamento)(row("Dati"))
            Dim pdc_testata As New AgronicaCorePianidiCampionamentoBiz.PDC_Testata With {
                .Id_PDC_Testata = id_pdc,
                .PDC_Testata_Des = pdc.descrizione,
                .PDC_Data_Istantanea = pdc.data_istantanea,
                .PDC_Stato = pdc.stato.codice,
                .Da_Campagna = If(pdc.daCampagna, 1, 0),
                .Da_Zoo = If(pdc.daZoo, 1, 0)
            }
            If pdc.impresa IsNot Nothing Then
                pdc_testata.PivaOwner = pdc.impresa.partitaIva
            End If
            If pdc.stalla IsNot Nothing Then
                pdc_testata.PivaOwner = pdc.stalla.partitaIva
                pdc_testata.Sa_CodOwner = pdc.stalla.saCod
                pdc_testata.Fabbricato_CodOwner = pdc.stalla.fabbricato_cod
            End If
            If pdc.validita Is Nothing Then
                pdc_testata.Validita_Inizio = AGRODATAINIZIO
                pdc_testata.Validita_Fine = AGRODATAFINE
            Else
                pdc_testata.Validita_Inizio = pdc.validita.inizio
                pdc_testata.Validita_Fine = pdc.validita.fine
            End If
            If pdc.dettagliZoo IsNot Nothing Then
                For Each dettaglio In pdc.dettagliZoo
                    Dim giacenza As GiacenzaZoo = dettaglio.elementoAnagrafico
                    ' sostituisce dettagli/campioni animali già presenti
                    If id_pdc <> 0 AndAlso giacenza.capo.codice <> 0 Then
                        PDC_Dettagli_Helper.CancellaDettagliZoo(id_pdc, giacenza.capo.codice, objParametri_Server)
                    End If
                    Dim pdc_dettaglio As New AgronicaCorePianidiCampionamentoBiz.PDC_Dettagli With {
                        .Id_PDC_Testata = id_pdc,
                        .Cod_Animale = giacenza.capo.codice,
                        .Cod_Progetto = giacenza.capo.codice,
                        .Sesso = giacenza.capo.sesso,
                        .Data_Nascita = giacenza.capo.dataNascita,
                        .Matricola = giacenza.capo.matricola,
                        .Lotto = giacenza.lotto
                    }
                    If giacenza.raggruppamento IsNot Nothing Then
                        pdc_dettaglio.Raggruppamento_Cod = giacenza.raggruppamento.codice
                    End If
                    If giacenza.capo.genere IsNot Nothing Then
                        pdc_dettaglio.GEN_COD = giacenza.capo.genere.codice
                    End If
                    If giacenza.capo.specie IsNot Nothing Then
                        pdc_dettaglio.SPE_COD = giacenza.capo.specie.codice
                    End If
                    If giacenza.capo.razza IsNot Nothing Then
                        pdc_dettaglio.RAZ_COD = giacenza.capo.razza.codice
                    End If
                    If giacenza.capo.dataNascita < AGRODATAINIZIO Then
                        pdc_dettaglio.Data_Nascita = AGRODATAINIZIO
                    End If
                    Dim esercizi = giacenza.capo.esercizi
                    If esercizi IsNot Nothing AndAlso esercizi.Count > 0 Then
                        pdc_dettaglio.Cod_Progetto = esercizi.First.codice
                    End If
                    If dettaglio.campioni IsNot Nothing Then
                        pdc_dettaglio.Flag_PDC = If(dettaglio.campioni.Count > 0, -1, 0)
                        For Each campione In dettaglio.campioni
                            Dim pdc_campione As New AgronicaCorePianidiCampionamentoBiz.PDC_Campioni With {
                                .Id_PDC_Testata = id_pdc,
                                .ID_PDC_Stato_Campione = 1,
                                .Descrizione_PuntoDiPrelievo = CleanString(campione.descrizione),
                                .Codice_Campione = CleanString(campione.codice_campione),
                                .Data_Campionamento = campione.data_campionamento
                            }
                            If campione.stato IsNot Nothing AndAlso campione.stato.codice <> 0 Then
                                pdc_campione.ID_PDC_Stato_Campione = campione.stato.codice
                            End If
                            pdc_dettaglio.CampioniAssociati.Add(pdc_campione)
                        Next
                    End If
                    pdc_testata.PDC_Dettagli.Add(pdc_dettaglio)
                Next
            End If
            messaggioErrore = PDC_Testata_Helper.Salva(pdc_testata, objParametri_Server, baseCode, topCode)

            If pdc_testata.Id_PDC_Testata <> 0 Then
                riferimento = CStr(pdc_testata.Id_PDC_Testata)
            Else
                esito = False
            End If

        Next

        Return esito

    End Function


    Public Function ImportaPianoCampionamentoConSpostamento(Optional ByVal unid As String = "", Optional ByRef riferimento As String = "") As Boolean

        Dim esito As Boolean = True
        Dim objAppDati As New APP_Dati_R
        Dim dtPDCZoo = objAppDati.Leggi_DatiAPP(unid, enum_Dati_App.PianoCampionamentoConSpostamento, True, "", "", objParametri_Server)

        ' ricavo base e top per creare PDC
        Dim progressivo As Integer
        Dim baseCode As Integer = 0
        Dim topCode As Integer = 0
        CodiciProgressivi.Trova_Base_e_Top(progressivo, baseCode, topCode, objParametri_Utenti, objParametri_Server)

        For Each row As DataRow In dtPDCZoo.Rows

            Dim id_pdc As Integer = If(riferimento = "", 0, CInt(riferimento))
            Dim pdc As PianoDiCampionamentoSpostamento = JsonConvert.DeserializeObject(Of PianoDiCampionamentoSpostamento)(row("Dati"))
            Dim pdc_testata As New AgronicaCorePianidiCampionamentoBiz.PDC_Testata With {
                .Id_PDC_Testata = id_pdc,
                .PDC_Testata_Des = pdc.descrizione,
                .PDC_Data_Istantanea = pdc.data_istantanea,
                .PDC_Stato = pdc.stato.codice,
                .Da_Campagna = If(pdc.daCampagna, 1, 0),
                .Da_Zoo = If(pdc.daZoo, 1, 0)
            }
            If pdc.impresa IsNot Nothing Then
                pdc_testata.PivaOwner = pdc.impresa.partitaIva
            End If
            If pdc.stalla IsNot Nothing Then
                pdc_testata.PivaOwner = pdc.stalla.partitaIva
                pdc_testata.Sa_CodOwner = pdc.stalla.saCod
                pdc_testata.Fabbricato_CodOwner = pdc.stalla.fabbricato_cod
            End If
            If pdc.validita Is Nothing Then
                pdc_testata.Validita_Inizio = AGRODATAINIZIO
                pdc_testata.Validita_Fine = AGRODATAFINE
            Else
                pdc_testata.Validita_Inizio = pdc.validita.inizio
                pdc_testata.Validita_Fine = pdc.validita.fine
            End If
            If pdc.dettagliZoo IsNot Nothing Then
                For Each dettaglio In pdc.dettagliZoo
                    Dim giacenza As GiacenzaZoo = dettaglio.elementoAnagrafico
                    ' sostituisce dettagli/campioni animali già presenti
                    If id_pdc <> 0 AndAlso giacenza.capo.codice <> 0 Then
                        PDC_Dettagli_Helper.CancellaDettagliZoo(id_pdc, giacenza.capo.codice, objParametri_Server)
                    End If
                    Dim pdc_dettaglio As New AgronicaCorePianidiCampionamentoBiz.PDC_Dettagli With {
                        .Id_PDC_Testata = id_pdc,
                        .Cod_Animale = giacenza.capo.codice,
                        .Cod_Progetto = giacenza.capo.codice,
                        .Sesso = giacenza.capo.sesso,
                        .Data_Nascita = giacenza.capo.dataNascita,
                        .Matricola = giacenza.capo.matricola,
                        .Lotto = giacenza.lotto
                    }
                    If giacenza.raggruppamento IsNot Nothing Then
                        pdc_dettaglio.Raggruppamento_Cod = giacenza.raggruppamento.codice
                    End If
                    If giacenza.capo.genere IsNot Nothing Then
                        pdc_dettaglio.GEN_COD = giacenza.capo.genere.codice
                    End If
                    If giacenza.capo.specie IsNot Nothing Then
                        pdc_dettaglio.SPE_COD = giacenza.capo.specie.codice
                    End If
                    If giacenza.capo.razza IsNot Nothing Then
                        pdc_dettaglio.RAZ_COD = giacenza.capo.razza.codice
                    End If
                    If giacenza.capo.dataNascita < AGRODATAINIZIO Then
                        pdc_dettaglio.Data_Nascita = AGRODATAINIZIO
                    End If
                    Dim esercizi = giacenza.capo.esercizi
                    If esercizi IsNot Nothing AndAlso esercizi.Count > 0 Then
                        pdc_dettaglio.Cod_Progetto = esercizi.First.codice
                    End If
                    If dettaglio.campioni IsNot Nothing Then
                        pdc_dettaglio.Flag_PDC = If(dettaglio.campioni.Count > 0, -1, 0)
                        For Each campione In dettaglio.campioni
                            Dim pdc_campione As New AgronicaCorePianidiCampionamentoBiz.PDC_Campioni With {
                                .Id_PDC_Testata = id_pdc,
                                .ID_PDC_Stato_Campione = 1,
                                .Descrizione_PuntoDiPrelievo = CleanString(campione.descrizione),
                                .Codice_Campione = CleanString(campione.codice_campione),
                                .Data_Campionamento = campione.data_campionamento
                            }
                            If campione.stato IsNot Nothing AndAlso campione.stato.codice <> 0 Then
                                pdc_campione.ID_PDC_Stato_Campione = campione.stato.codice
                            End If
                            pdc_dettaglio.CampioniAssociati.Add(pdc_campione)
                        Next
                    End If
                    pdc_testata.PDC_Dettagli.Add(pdc_dettaglio)
                Next
            End If
            Dim usernameOperazione As String = row("Username_Creazione")
            objParametri_Server.UsernameOperazione = usernameOperazione
            messaggioErrore = PDC_Testata_Helper.Salva(pdc_testata, objParametri_Server, baseCode, topCode)

            If pdc_testata.Id_PDC_Testata <> 0 Then
                riferimento = CStr(pdc_testata.Id_PDC_Testata)
            Else
                esito = False
            End If

            If esito AndAlso pdc.eseguiSpostamento Then
                creaSpostamentiDaPianoCampionamento(pdc)
            End If

        Next

        Return esito

    End Function

    Function creaSpostamentiDaPianoCampionamento(pdc As PianoDiCampionamentoSpostamento) As Boolean
        Dim resp = False
        If pdc.stalla Is Nothing Then
            Return False
        End If

        Dim objZoo As New AgronicaCoreAnagrafeDAL.Zoo_Animali
        Dim DtGiacenze = objZoo.Leggi_Giacenze(pdc.stalla.partitaIva, pdc.stalla.saCod, pdc.stalla.fabbricato_cod, 0, 0, DateTime.Now, objParametri_Server)
        Dim listaSpostamenti = creaListaSpotamentoDaPDC(pdc, DtGiacenze)
        Dim listaSpostamentiMassivi = creaListaSpotamentoMassivoDaPDC(listaSpostamenti)
        Dim listaAttivita = creaListaAttivitaDaSpostamentoMassivoDaPDC(listaSpostamentiMassivi)
        Dim objAttivitaZootecnicaScrivi As New AgronicaCoreMapper.AttivitaZootecnicaToAgenda

        Dim boolTuttiImportati = True
        For Each attivita In listaAttivita
            Dim id_agenda = objAttivitaZootecnicaScrivi.ScriviAttivitaZootecnicaToAgenda(attivita,
                                                                                         objParametri_Server,
                                                                                         origine:="APP",
                                                                                         verificaGiacenzeSpostamento:=False)
            If id_agenda = 0 Then
                boolTuttiImportati = False
            End If
        Next

        Return resp
    End Function

    Private Function creaListaAttivitaDaSpostamentoMassivoDaPDC(listaSpostamentiMassivi As List(Of SpostamentoMassivoDaPDC)) As List(Of AgronicaCoreModelsSTD.attivita.Attivita)
        Dim attivitaList As New List(Of AgronicaCoreModelsSTD.attivita.Attivita)
        For Each spostamento In listaSpostamentiMassivi
            attivitaList.Add(creaAttivitaDaSpostamentoMassivoDaPDC(spostamento))
        Next
        Return attivitaList
    End Function

    Private Function creaAttivitaDaSpostamentoMassivoDaPDC(spostamento As SpostamentoMassivoDaPDC) As AgronicaCoreModelsSTD.attivita.Attivita
        Dim attivita As New AgronicaCoreModelsSTD.attivita.Attivita

        Dim piva As String = spostamento.Piva
        Dim saCod As Integer = spostamento.SaCod
        Dim staNum As Integer = spostamento.StaNum

        ''Da Verificare
        attivita.codice = 0
        attivita.inizio = DateTime.Now
        attivita.fine = AGRODATAFINE
        attivita.centroAziendale = New anagrafiche.CentroAziendale With {
            .primaryKey = New anagrafiche.CentroAziendale.PK With {
                .partitaIva = piva,
                .codice = spostamento.SaCod
                }
            }
        attivita.fabbricatoCod = staNum
        attivita.centriDiCosto = New List(Of AgronicaCoreModelsSTD.attivita.centri_di_costo.CentroDiCosto)
        attivita.job = New AgronicaCoreModelsSTD.attivita.Zootecnia(LAVCOD_SPOSTAMENTI_ZOO, "")

        For Each codProgetto In spostamento.Cod_Progetto
            Dim cdcProgetto As New AgronicaCoreModelsSTD.attivita.centri_di_costo.CapoAnimaleCDC
            cdcProgetto.codice = New CodeType(saCod)
            cdcProgetto.capoAnimale = New AgronicaCoreModelsSTD.anagrafiche.CapoAnimale(piva, codProgetto, "")
            cdcProgetto.capoAnimaleNonPresente = New baseClass.BaseCodeDescr With {
                .codice = codProgetto,
                .descrizione = "Capo " & codProgetto
            }
            cdcProgetto.sottogruppoStalla_ingresso = New anagrafiche.SottogruppoStalla With {
                .codice = spostamento.Raggruppamento_Destinazione_Cod,
                .stallaPK = New anagrafiche.FabbricatoLight.PK(piva, saCod, staNum)
            }
            cdcProgetto.sottogruppoStalla_uscita = New anagrafiche.SottogruppoStalla With {
                .codice = spostamento.Raggruppamento_Origine_Cod,
                .stallaPK = New anagrafiche.FabbricatoLight.PK(piva, saCod, staNum)
            }
            attivita.centriDiCosto.Add(cdcProgetto)
        Next

        Return attivita
    End Function


    Private Function creaListaSpotamentoDaPDC(pdc As PianoDiCampionamento, DtGiacenze As DataTable) As List(Of SpostamentoDaPDC)
        Dim spostamenti As New List(Of SpostamentoDaPDC)

        For Each dettaglio In pdc.dettagliZoo
            Dim Cod_Progetto = dettaglio.elementoAnagrafico.capo.codice
            Dim Raggruppamento_Destinazione_Cod = dettaglio.elementoAnagrafico.raggruppamento.codice
            Dim Raggruppamento_Origine_Cod = DtGiacenze.Select("Cod_Animale = " & Cod_Progetto)(0).Item("Raggruppamento_Cod")

            If Raggruppamento_Destinazione_Cod = Raggruppamento_Origine_Cod Then
                'Il capo è già nel box corretto, non serve creare uno spostamento
                Continue For
            End If

            Dim spostamento As New SpostamentoDaPDC With {
                .Piva = pdc.stalla.partitaIva,
                .SaCod = pdc.stalla.saCod,
                .StaNum = pdc.stalla.fabbricato_cod,
                .Cod_Progetto = Cod_Progetto,
                .Raggruppamento_Destinazione_Cod = Raggruppamento_Destinazione_Cod,
                .Raggruppamento_Origine_Cod = Raggruppamento_Origine_Cod
            }
            spostamenti.Add(spostamento)
        Next

        Return spostamenti

    End Function

    Private Function creaListaSpotamentoMassivoDaPDC(spostamenti As List(Of SpostamentoDaPDC)) As List(Of SpostamentoMassivoDaPDC)
        Dim spostamentiMassivi As New List(Of SpostamentoMassivoDaPDC)

        spostamentiMassivi = spostamenti.
        GroupBy(Function(s) New With {
            Key .Piva = s.Piva,
            Key .SaCod = s.SaCod,
            Key .StaNum = s.StaNum,
            Key .Destinazione = s.Raggruppamento_Destinazione_Cod,
            Key .Origine = s.Raggruppamento_Origine_Cod
        }).
        Select(Function(gruppo) New SpostamentoMassivoDaPDC With {
            .Piva = gruppo.Key.Piva,
            .SaCod = gruppo.Key.SaCod,
            .StaNum = gruppo.Key.StaNum,
            .Raggruppamento_Destinazione_Cod = gruppo.Key.Destinazione,
            .Raggruppamento_Origine_Cod = gruppo.Key.Origine,
            .Cod_Progetto = gruppo.Select(Function(s) s.Cod_Progetto).ToList()
        }).
        ToList()

        Return spostamentiMassivi
    End Function

    Private Class SpostamentoDaPDC
        Public Piva As String
        Public SaCod As Integer
        Public StaNum As Integer
        Public Cod_Progetto As Integer
        Public Raggruppamento_Destinazione_Cod As Integer
        Public Raggruppamento_Origine_Cod As Integer
    End Class

    Private Class SpostamentoMassivoDaPDC
        Public Piva As String
        Public SaCod As Integer
        Public StaNum As Integer
        Public Cod_Progetto As List(Of Integer)
        Public Raggruppamento_Destinazione_Cod As Integer
        Public Raggruppamento_Origine_Cod As Integer
    End Class

    Public Function CleanString(input As String) As String
        If String.IsNullOrEmpty(input) Then
            Return String.Empty
        End If

        ' Rimuove spazi, tab, newline e caratteri di controllo dall'inizio e dalla fine
        Dim pulito As String = Regex.Replace(input, "^[\s\p{C}]+|[\s\p{C}]+$", "", RegexOptions.None, TimeSpan.FromSeconds(3))

        Return pulito
    End Function

    Public Function ImportaAttivitaFromTipo(ByVal tipo As enum_Dati_App, ByRef msgFinale As String, Optional ByVal guid As String = "", Optional ByRef riferimento As String = "") As Boolean

        Dim msgErr As String = ""
        Dim esitoFinale As Boolean = False

        Dim attivitaImportate As Integer = 0
        Dim attivitaDaImportare As Integer = 0

        Try

            Dim origine As String = String.Empty

            If DictOrigineAttivitaToImport.ContainsKey(tipo) Then
                origine = DictOrigineAttivitaToImport(tipo)
            End If

            Dim objAppDati As New APP_Dati_R
            Dim dtAttivita = objAppDati.Leggi_DatiAPP(guid, tipo, True, "", "", objParametri_Server)
            Dim mapper As New AgronicaCoreMapper.AttivitaToAgenda

            attivitaDaImportare = dtAttivita.Rows.Count()

            For Each row As DataRow In dtAttivita.Rows

                'Dim FlagTransazioneLocale As Boolean = False
                'Dim FlagConnessioneLocale As Boolean = False

                Try

                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    'Apro la connessione al DB
                    'ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                    '                                               FlagTransazioneLocale,
                    '                                               objParametri_Server)



                    Dim listaAttivita = JsonConvert.DeserializeObject(Of List(Of AgronicaCoreModelsSTD.attivita.Attivita))(row("Dati"))

                    For Each attivita As AgronicaCoreModelsSTD.attivita.Attivita In listaAttivita
                        attivita.guid = guid
                        attivita.origine = origine
                    Next

                    Dim listaAttivitaAgende = mapper.MappaListaAttivitaToListaAgenda(listaAttivita, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)

                    Dim lista_Errori As New List(Of ErroreGias)

                    Dim listaAttivita_toReturn = mapper.ScriviListaAttivitaToRaccoglitore(listaAttivitaAgende,
                                                                                 lista_ParametriAggiuntivi:=New List(Of Parametri_Aggiuntivi_Attivita),
                                                                                 lista_Impianti:=New List(Of AgronicaCoreModelsSTD.anagrafiche.Impianto),
                                                                                 lista_Errori,
                                                                                 agendeToDelete:=Nothing,
                                                                                 eseguiSoloVerificheConformita:=False,
                                                                                 creaCaricoMagazzinoXOriginePUA:=False,
                                                                                 objParametri_Super_Server, objParametri_Server, objParametri_Utenti)

                    If listaAttivita_toReturn IsNot Nothing AndAlso listaAttivita_toReturn.Count > 0 AndAlso listaAttivita_toReturn(0).testataRicetta IsNot Nothing AndAlso listaAttivita_toReturn(0).testataRicetta.Ricetta_Cod <> 0 Then
                        riferimento = CStr(listaAttivita_toReturn(0).testataRicetta.Ricetta_Cod)
                    Else
                        Dim errMsg = String.Format(My.Resources.AgronicaCoreMapper.ErroreSalvataggioAttivita, row("codice"))
                        Throw New Exception(errMsg)
                    End If

                    Dim RicetteW As New AgronicaCoreContabBIZ.Ricette_W
                    Dim r = RicetteW.Update_Origine(riferimento, origine, objParametri_Server)

                    attivitaImportate += 1


                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    'Chiudo la connessione al DB
                    'ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)

                Catch ex As Exception

                    msgErr &= ex.Message & "<br>"

                    'Faccio il rollback della transazione
                    '    If objParametri_Server.objTransazione IsNot Nothing Then
                    '        ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
                    '    End If

                    'Finally

                    '    ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

                End Try


            Next

            If String.IsNullOrEmpty(guid) Then
                esitoFinale = True
                msgFinale &= "<br><br>"

                Dim errMsg = String.Format(My.Resources.AgronicaCoreMapper.AttivaImportateCorrettamente, attivitaImportate, attivitaDaImportare)
                msgFinale &= errMsg

                If Not String.IsNullOrEmpty(msgErr) Then

                    msgFinale &= "<br><br>" & My.Resources.AgronicaCoreMapper.Errori & "<br>" & msgErr
                End If
            Else
                esitoFinale = attivitaImportate = attivitaDaImportare
                If Not String.IsNullOrEmpty(msgErr) Then
                    msgFinale &= msgErr
                End If
            End If

        Catch ex As Exception

            esitoFinale = False
            msgFinale &= Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return esitoFinale

    End Function

    Public Function CancellaAttivitaCdG(ByVal guid As String) As Boolean

        If String.IsNullOrEmpty(guid) Then
            Return False
        End If

        Dim esito As Boolean = True
        Dim objAppDati As New APP_Dati_R
        Dim dtAttivitaCdG = objAppDati.Leggi_AttivitaCdGAPP(objParametri_Server, guid)

        For Each row As DataRow In dtAttivitaCdG.Rows

            Try

                If Not IsDBNull(row.Item("Id_CDG")) AndAlso row.Item("Id_CDG") <> 0 Then
                    Dim objCDG_w As New CDG_DAL_W
                    objCDG_w.CancellaCDG_Dettagli(row.Item("Piva"), row.Item("Id_CDG"), objParametri_Server)
                    objCDG_w.CancellaCDG_Testata(row.Item("Piva"), row.Item("Id_CDG"), objParametri_Server)
                End If

            Catch ex As Exception
                esito = False
            End Try

        Next

        Return esito

    End Function


    Public Function CancellaAgenda(ByVal guid As String, Optional ByVal origine As enum_SistemiEsterni = enum_SistemiEsterni.GiasAPP) As Boolean

        If String.IsNullOrEmpty(guid) Then
            Return False
        End If

        Dim esito As Boolean = True
        Dim objAppDati As New APP_Dati_R
        Dim dtAttivita = objAppDati.Leggi_AgendaAPP(objParametri_Server, guid)

        For Each row As DataRow In dtAttivita.Rows

            Try

                ' cancella agenda
                If Not IsDBNull(row.Item("Id_Agenda")) AndAlso row.Item("Id_Agenda") <> 0 Then

                    dataCreazione = row.Item("Data_Creazione")
                    usernameCreazione = row.Item("Username_Creazione")

                    Dim messaggio As String = ""
                    Dim objParametriAgenda As New ParametriAgenda(False) With {
                       .Piva = row.Item("Piva"),
                       .Id_Agenda = row.Item("Id_Agenda"),
                       .Lav_Cod = row.Item("Lav_Cod"),
                       .Tipo_Operazione = enum_TipoOperazioneDB.Cancellazione
                    }

                    esito = GestisciCancellazione(objParametriAgenda, objParametri_Server, messaggio, True,
                                                  objParametri_Utenti:=objParametri_Utenti, origine:=origine)

                End If

            Catch ex As Exception
                esito = False
            End Try

        Next

        Return esito

    End Function

    Public Function CancellaPianoCampionamento(ByVal guid As String) As Boolean

        If String.IsNullOrEmpty(guid) Then
            Return False
        End If

        Dim esito As Boolean = True
        Dim objAppDati As New APP_Dati_R
        Dim dtPDC = objAppDati.Leggi_PianoCampionamentoAPP(objParametri_Server, guid)

        For Each row As DataRow In dtPDC.Rows
            Try
                ' cancella piano campionamento
                messaggioErrore = PDC_Testata_Helper.Cancella(row.Item("Id_PDC_Testata"), objParametri_Server)
                esito = String.IsNullOrEmpty(messaggioErrore)
            Catch ex As Exception
                esito = False
            End Try
        Next

        Return esito

    End Function

    Public Sub AggiornaDocumentiAttivita()

        Dim objAppDatiR As New APP_Dati_R
        Dim objAppDatiW As New APP_Dati_W

        Dim dtDocAttivita = objAppDatiR.Leggi_DocumentiAttivitaAPP(objParametri_Server)

        For Each row As DataRow In dtDocAttivita.Rows

            ' aggiorna documento attività agenda
            objAppDatiW.Aggiorna_DocumentiAttivitaAPP(row.Item("id_alert_entita"), row.Item("id_agenda"), row.Item("ricetta_operazione_cod"), objParametri_Server)

        Next

    End Sub

    Public Function CancellaDocumenti(ByVal guid As String) As Boolean

        If String.IsNullOrEmpty(guid) Then
            Return False
        End If

        Dim esito As Boolean = True

        Dim objW As New Alert_W
        Dim objAppDati As New APP_Dati_R
        Dim dtDocumenti = objAppDati.Leggi_DocumentiAPP(objParametri_Server, guid)

        Dim LeggiConfSiti As New Configurazione_Siti_R
        Dim dt_Conf As DataTable = LeggiConfSiti.Leggi(6, "GestioneAllegati_Repository", "", "", objParametri_Server)
        Dim path As String = dt_Conf.Rows(0).Item("Valore")

        For Each row As DataRow In dtDocumenti.Rows

            Try
                ' cancella documento
                If Not IsDBNull(row.Item("Id_Elenco")) AndAlso row.Item("Id_Elenco") <> 0 Then
                    objW.Cancella(row.Item("Id_Elenco"), path, objParametri_Server)
                End If

            Catch ex As Exception
                esito = False
            End Try

        Next

        Return esito

    End Function

    Public Function SincronizzaDatiApp(ByRef messaggioRitorno As String, Optional ByVal tipoDati As String = "", Optional ByVal documenti As Boolean = False, Optional ByVal agenda As String = "", Optional logDebug As Boolean = False, Optional azienda As String = "") As Boolean

        Dim objAppHelper As New AppHelper
        Dim tipi = tipoDati.Split(",")
        Dim dati = objAppHelper.Leggi_Dati_Import(tipoDati, objParametri_Server, azienda)

        Dim datiDaImportare As Integer = dati.Rows.Count
        Dim datiImportati As Integer = 0
        Dim datiCancellati As Integer = 0
        Dim datiErrati As Integer = 0
        Dim log As New StringBuilder

        ' verifico configurazione e permesso per operazioni agenda NG
        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim dtConfigSitiNG As DataTable = objConfigSiti.Leggi(0, "OperazioniAgendaNG", "", "", objParametri_Server)
        Dim operazioniAgendaNG As Boolean = Not IsNothing(dtConfigSitiNG) AndAlso dtConfigSitiNG.Rows.Count > 0 AndAlso LCase(dtConfigSitiNG.Rows(0).Item("Valore")) = "true"

        log.AppendLine("IMPORTAZIONE DATI APP " & objParametri_Server.PivaSuperUser & " (" & objParametri_Server.UtenteUsername & ")")
        log.AppendLine("------------------------------------------------------------------------------")

        For Each row In dati.Rows

            Dim esito As Boolean = True
            Dim unid As String = row.Item("ID")
            Dim tipo As String = row.Item("Tipo")
            Dim piva As String = row.Item("Piva")
            Dim riferimento As String = row.Item("Riferimento")
            Dim cancellato As Boolean = row.Item("cancellato") = "1"
            Dim aggiornamento As Boolean = Not String.IsNullOrEmpty(riferimento)
            Dim timestamp As String = CStr(Date.Now)
            Dim posizione As String = ""
            Dim rifDocumento As String = ""
            Dim rilievoNG As Boolean = False
            Dim codice As String = row.Item("Codice")

            If tipo = enum_Dati_App.Rilievi Then
                If operazioniAgendaNG Then
                    rilievoNG = True
                Else
                    Dim ricette = JsonConvert.DeserializeObject(Of RicettePerScarico)(row("Dati"))
                    posizione = ricette.posizione
                End If
            End If

            'Ticket 169588, controlliamo che codice <> -1 per distinguere visita con rilievo da visita senza rilievo
            If tipo = enum_Dati_App.Visite AndAlso operazioniAgendaNG AndAlso codice <> "-1" Then
                rilievoNG = True
            End If

            ' importa dati app
            esito = ImportaDatiApp(unid, tipo, riferimento, aggiornamento, cancellato, If(rilievoNG, "NG", agenda), posizione, rifDocumento)

            ' importa documenti allegati
            If esito AndAlso documenti AndAlso tipo <> enum_Dati_App.Documenti Then
                If rifDocumento = "" Then
                    rifDocumento = riferimento
                End If
                esito = ImportaDatiApp(unid, enum_Dati_App.Documenti, rifDocumento, aggiornamento, cancellato)
            End If

            ' contatori
            If esito Then
                datiImportati += 1
                If cancellato Then
                    datiCancellati += 1
                End If
            Else
                datiErrati += 1
            End If

            If logDebug OrElse Not esito Then

                log.Append(CStr(Date.Now) & " " & If(esito, "[OK]", "[ERRORE]") & " ")
                log.Append(If(cancellato, "Cancellazione", "Importazione") & " ")

                Select Case tipo
                    Case enum_Dati_App.Attivita
                        log.Append("Attività")
                    Case enum_Dati_App.Ricette
                        log.Append("Ricette")
                    Case enum_Dati_App.AttivitaCdG
                        log.Append("Attività CdG")
                    Case enum_Dati_App.Documenti
                        log.Append("Documento")
                    Case enum_Dati_App.Manutenzioni
                        log.Append("Manutenzione macchina")
                    Case enum_Dati_App.Movimenti
                        log.Append("Movimento di carico")
                    Case enum_Dati_App.Acquisti
                        log.Append("DDT ricevuti")
                    Case enum_Dati_App.Rilievi
                        log.Append("Rilievo")
                    Case enum_Dati_App.Visite
                        log.Append("Visita")
                    Case Else
                        log.Append("Altro")
                End Select

                log.Append(" (ID: " & unid)

                If Not String.IsNullOrEmpty(piva) Then
                    log.Append(", Piva: " & piva)
                End If

                If Not String.IsNullOrEmpty(riferimento) Then
                    log.Append(", Riferimento: " & riferimento)
                End If

                log.AppendLine(") ")

            End If

        Next

        ' importo le attività CdG collegate a operazioni di brogliaccio passate in agenda
        If datiDaImportare = 0 AndAlso tipi.Length = 1 AndAlso tipi.Contains(enum_Dati_App.AttivitaCdG) Then
            Dim xLettura As New AgronicaCoreContabBIZ.CDG_APP
            Dim esito = xLettura.ImportaCDGDaTabelleAPP(azienda, 0, log, objParametri_Server, objParametri_Utenti)
            messaggioRitorno = log.ToString
        End If

        ' messaggio restituito
        If datiDaImportare > 0 Then
            If logDebug OrElse datiErrati > 0 Then
                log.AppendLine("------------------------------------------------------------------------------")
            End If
            log.AppendLine("Dati da importare: " & datiDaImportare)
            log.AppendLine("Dati importati: " & datiImportati)
            log.AppendLine("Dati cancellati: " & datiCancellati)
            log.AppendLine("Dati con errori: " & datiErrati)
            messaggioRitorno = log.ToString
        End If

        Return datiImportati = datiDaImportare

    End Function

    Public Function SincroDatiApp(ByRef unid As String,
                                  ByVal tipo As enum_Dati_App,
                                  ByVal dati As String,
                                  ByRef riferimento As String,
                                  ByVal aggiornamento As Boolean,
                                  ByVal cancellato As Boolean,
                                  Optional ByVal importazione As Boolean = False,
                                  Optional ByVal piva As String = "",
                                  Optional ByVal codice As String = "",
                                  Optional ByVal documenti As Boolean = False,
                                  Optional ByVal agenda As String = "",
                                  Optional ByVal posizione As String = "",
                                  Optional ByVal userAgent As String = "",
                                  Optional ByRef erroreImport As String = "",
                                  Optional ByVal versione As String = "",
                                  Optional ByVal origine As enum_SistemiEsterni = enum_SistemiEsterni.GiasAPP,
                                  Optional ByVal riferimentoPianificata As String = Nothing) As Boolean

        Dim esito As Boolean = True
        Dim objAppHelper As New AppHelper

        ' se dati gia inviati al server ricavo il riferimento e tipo
        If Not String.IsNullOrEmpty(riferimento) Then
            aggiornamento = True
        ElseIf Not String.IsNullOrEmpty(unid) AndAlso riferimento = "" Then
            Dim appDatiCancellato As Boolean
            If tipo = enum_Dati_App.Attivita OrElse tipo = enum_Dati_App.AttivitaDemetra OrElse tipo = enum_Dati_App.Movimenti OrElse tipo = enum_Dati_App.MovimentiDemetra OrElse tipo = enum_Dati_App.Acquisti OrElse tipo = enum_Dati_App.AcquistiDemetra OrElse tipo = enum_Dati_App.Ricette OrElse tipo = enum_Dati_App.RicetteDemetra Then
                riferimento = objAppHelper.Leggi_Riferimento_APP(unid, "", objParametri_Server, appDatiCancellato)
            Else
                riferimento = objAppHelper.Leggi_Riferimento_APP(unid, tipo, objParametri_Server, appDatiCancellato)
            End If
        End If

        ' scrivo dati app
        If Not cancellaDatiApp AndAlso cancellato Then
            esito = objAppHelper.Aggiorna_Dati_APP(unid, tipo, "", riferimento, cancellato, importazione, objParametri_Server,
                                                   userAgent, versione, riferimentoPianificata)
        Else
            esito = objAppHelper.Scrivi_Dati_APP(unid, tipo, dati, riferimento, cancellato, objParametri_Server, piva, codice,
                                                 userAgent, versione, riferimentoPianificata)
        End If

        ' importa dati app
        If esito AndAlso importazione Then
            Dim rifDocumento As String = ""
            esito = ImportaDatiApp(unid, tipo, riferimento, aggiornamento, cancellato, agenda, posizione, rifDocumento, origine:=origine)
            ' importa documenti allegati
            If esito AndAlso documenti Then
                If rifDocumento = "" Then
                    rifDocumento = riferimento
                End If
                esito = ImportaDatiApp(unid, enum_Dati_App.Documenti, rifDocumento, aggiornamento, cancellato, origine:=origine)
            End If
        End If

        If Not esito Then
            erroreImport = messaggioErrore
        End If

        Return esito

    End Function

    Public Function ImportaDatiApp(ByRef unid As String,
                                   ByVal tipo As enum_Dati_App,
                                   ByRef riferimento As String,
                                   ByVal aggiornamento As Boolean,
                                   ByVal cancellato As Boolean,
                                   Optional ByVal agenda As String = "",
                                   Optional ByVal posizione As String = "",
                                   Optional ByRef rifDocumento As String = "",
                                   Optional ByVal origine As enum_SistemiEsterni = enum_SistemiEsterni.GiasAPP) As Boolean

        Dim esito As Boolean = True
        Dim objAppHelper As New AppHelper

        Try

            'apri transazione
            'ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)

            dataCreazione = Date.Now
            usernameCreazione = objParametri_Server.UtenteUsername

            ' cancella dati
            If aggiornamento Then
                Select Case tipo
                    Case enum_Dati_App.PianoCampionamento, enum_Dati_App.PianoCampionamentoConSpostamento
                        If cancellato Then
                            esito = CancellaPianoCampionamento(unid)
                        End If
                    Case enum_Dati_App.AttivitaZoo,
                         enum_Dati_App.Movimenti, enum_Dati_App.Acquisti,
                         enum_Dati_App.Visite, enum_Dati_App.Rilievi
                        esito = CancellaAgenda(unid, origine:=origine)
                    Case enum_Dati_App.Documenti
                        esito = CancellaDocumenti(unid)
                    Case enum_Dati_App.Attivita, enum_Dati_App.Ricette
                        esito = CancellaAttivita(unid, origine:=origine)
                    Case enum_Dati_App.AttivitaCdG
                        esito = CancellaAttivitaCdG(unid)
                    Case Else
                        If DictOrigineAttivitaToImport.ContainsKey(tipo) Then
                            esito = CancellaAttivita(unid, origine:=origine)
                        End If
                End Select
            End If

            ' importa dati
            If esito AndAlso Not cancellato Then
                Select Case tipo
                    Case enum_Dati_App.PianoCampionamento
                        esito = ImportaPianoCampionamento(unid, riferimento)
                    Case enum_Dati_App.PianoCampionamentoConSpostamento
                        esito = ImportaPianoCampionamentoConSpostamento(unid, riferimento)
                    Case enum_Dati_App.AttivitaZoo
                        esito = ImportaAttivitaZoo(unid, riferimento)
                    Case enum_Dati_App.Movimenti
                        esito = ImportaMovimenti(unid, riferimento, origine:=origine)
                    Case enum_Dati_App.Acquisti
                        esito = ImportaAcquisti(unid, riferimento, origine:=origine)
                    Case enum_Dati_App.Documenti
                        esito = ImportaDocumenti(unid, riferimento)
                    Case enum_Dati_App.Visite
                        Dim rilievo As String = ""
                        If agenda = "NG" Then
                            esito = ImportaRilievo(unid, rilievo, True, origine:=origine)
                        Else
                            esito = ImportaAgenda(unid, rilievo, posizione)
                        End If
                        esito = ImportaVisite(unid, riferimento, rilievo)
                        If rilievo <> "" Then
                            rifDocumento = rilievo
                        End If
                    Case enum_Dati_App.Rilievi
                        If agenda = "NG" Then
                            esito = ImportaRilievo(unid, riferimento, origine:=origine)
                        Else
                            esito = ImportaAgenda(unid, riferimento, posizione)
                        End If
                    Case enum_Dati_App.Attivita, enum_Dati_App.Ricette
                        esito = ImportaAttivita(unid, riferimento, agenda, origine:=origine)
                    Case enum_Dati_App.AttivitaCdG
                        esito = ImportaAttivitaCdG(unid, riferimento)
                    Case enum_Dati_App.MovimentiGruppo
                        esito = ImportaAttivitaMovimentoGruppi(unid, riferimento)
                    Case Else
                        If DictOrigineAttivitaToImport.ContainsKey(tipo) Then
                            esito = ImportaAttivita(unid, riferimento, agenda, tipo:=tipo, origine:=origine)
                        End If
                End Select
            End If

            'commit transazione
            'ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server)

        Catch ex As Exception

            'commit transazione rollback
            'ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            messaggioErrore = ex.Message
            esito = False

        Finally

            'chiudi connessione
            'ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)

        End Try

        ' cancella / aggiorna dati app
        If esito AndAlso cancellaDatiApp AndAlso cancellato Then
            esito = objAppHelper.Cancella_Dati_APP(unid, tipo, objParametri_Server)
        Else
            Dim errore As String = If(String.IsNullOrEmpty(messaggioErrore), "Errore importazione", messaggioErrore)
            errore = If(esito, "", errore.Substring(0, Math.Min(2000, errore.Length)))
            objAppHelper.Aggiorna_Dati_APP(unid, tipo, errore, riferimento, cancellato, True, objParametri_Server)
        End If

        Return esito

    End Function

    Public Function ImportaVisite(ByRef unid As String, Optional ByRef riferimento As String = "", Optional ByRef rilievo As String = "") As Boolean
        Dim msgFinale As New StringBuilder
        Dim objVisite As New Visite_APP
        Dim objUtenti As New Utenti_CodiciGiasPRO_R
        Dim dt As DataTable = objUtenti.Leggi("", "", objParametri_Utenti)
        Dim progressivoGias As Integer = dt.Rows(0).Item("ProgressivoGIAS")
        Dim esito = objVisite.ImportaVisiteAPP("", progressivoGias, msgFinale, objParametri_Server, unid, riferimento, rilievo, dataCreazione, usernameCreazione)
        messaggioErrore = msgFinale.ToString
        Return esito
    End Function

    Public Function ImportaDocumenti(ByRef unid As String, Optional ByRef riferimento As String = "") As Boolean

        Dim esito As Boolean = True
        Dim objUtentiDettagliR As New Utenti_Dettagli_R
        Dim objAllegatiDocumentiR As New Allegati_Documenti_R
        Dim objAllegatiDocumentiW As New Allegati_Documenti_W
        Dim dtDocumenti As DataTable = objAllegatiDocumentiR.LeggiApp_Documenti(False, " ID LIKE '%" & unid & "%' ", "", objParametri_Server)

        For Each row As DataRow In dtDocumenti.Rows

            Dim id_elenco As Integer = -1
            Dim rif As String() = riferimento.Split("|")
            Dim username_upload As String = objUtentiDettagliR.Username_From_CodFisc(row("username_creazione"), objParametri_Utenti)

            Dim errore = SalvaDocumento(
                row("Piva"),
                0,
                row("id_tipologia"),
                row("NomeFile"),
                Convert.ToBase64String(DirectCast(row("FileAllegato"), Byte())),
                username_upload,
                row("Data_Scadenza"),
                row("Descrizione"),
                Nothing,
                0,
                If(rif.Length > 0 AndAlso rif(0) <> "", CInt(rif(0)), 0),
                If(rif.Length > 1 AndAlso rif(1) <> "", CInt(rif(1)), 0),
                row("ID"),
                id_elenco,
                row("Note"),
                Allegati_Documenti_Numero:=row("Numero_Documento"),
                Allegati_Documenti_Data:=IIf(IsDate(row("Data_Documento")), row("Data_Documento"), AGRODATAINIZIO)
            )

            If Not String.IsNullOrEmpty(errore) Then
                objAllegatiDocumentiW.Modifica_ImportazioneApp(row("ID"), errore, Date.Now, objParametri_Server)
                esito = False
            Else
                riferimento = CStr(id_elenco)
            End If

        Next

        Return esito

    End Function

    Public Function ImportaMovimenti(ByRef unid As String, Optional ByRef riferimento As String = "", Optional ByVal origine As enum_SistemiEsterni = enum_SistemiEsterni.GiasAPP) As Boolean

        Dim esito As Boolean = True
        Dim objAppHelper As New AppHelper
        Dim objAgendaHelper As New Agenda_Operazione_Helper

        Dim objAppDati As New APP_Dati_R
        Dim dtMovimenti = objAppDati.Leggi_DatiAPP(unid, enum_Dati_App.Movimenti, True, "", "", objParametri_Server)

        For Each row As DataRow In dtMovimenti.Rows

            Dim id_agenda As Integer = If(riferimento = "", 0, CInt(riferimento))
            Dim movimento = JsonConvert.DeserializeObject(Of MovimentoDiMagazzino)(row("Dati"))
            Dim agenda As Operazione_Agenda = objAppHelper.Genera_Agenda_Da_MovimentoAPP(movimento, id_agenda)
            id_agenda = objAgendaHelper.Scrivi(agenda, objParametri_Server, documentoPrevisionale:=True, origine:=origine)

            If id_agenda <> 0 Then
                riferimento = CStr(id_agenda)
            Else
                esito = False
            End If

        Next

        Return esito

    End Function


    Public Function ImportaAcquisti(ByRef unid As String, Optional ByRef riferimento As String = "", Optional ByVal origine As enum_SistemiEsterni = enum_SistemiEsterni.GiasAPP) As Boolean

        Dim esito As Boolean = True
        Dim objAppHelper As New AppHelper
        Dim objAgendaHelper As New Agenda_Operazione_Helper

        Dim objAppDati As New APP_Dati_R
        Dim dtAcquisti = objAppDati.Leggi_DatiAPP(unid, enum_Dati_App.Acquisti, True, "", "", objParametri_Server)

        For Each row As DataRow In dtAcquisti.Rows

            Dim id_agenda As Integer = If(riferimento = "", 0, CInt(riferimento))
            Dim acquisto = JsonConvert.DeserializeObject(Of Acquisto)(row("Dati"))
            Dim agenda As Operazione_Agenda = objAppHelper.Genera_Agenda_Da_AcquistoAPP(acquisto, id_agenda, objParametri_Server)
            id_agenda = objAgendaHelper.Scrivi(agenda, objParametri_Server, flagUsaOraReale:=True, documentoPrevisionale:=True, origine:=origine)

            If id_agenda <> 0 Then
                riferimento = CStr(id_agenda)
            Else
                esito = False
            End If

        Next

        Return esito

    End Function

    Public Function SalvaDocumento(ByVal Piva As String,
                                    ByVal Id_Area As Integer,
                                    ByVal Id_Tipologia As Integer,
                                    ByVal Nome_File As String,
                                    ByVal File_Allegato As String,
                                    ByVal Username_Upload As String,
                                    ByVal Data_scadenza As Date,
                                    ByVal Descrizione_Scadenza As String,
                                    ByVal EntitaxIndici As JArray,
                                    ByVal Richiesta_Cod As Integer,
                                    ByVal Id_Agenda As Integer,
                                    ByVal Ricetta_Operazione_cod As Integer,
                                    Optional ByVal ID_App As String = "",
                                    Optional ByRef ret_Id_Elenco As Integer = -1,
                                    Optional ByVal Note As String = "",
                                    Optional ByVal Allegati_Documenti_Numero As String = "",
                                    Optional ByVal Allegati_Documenti_Data As Date = AGRODATAINIZIO) As String

        'Questa funzione gestisce il salvataggio di un allegato sul documentale senza passare dall'interfaccia

        Dim NomeRoutine As String = "AgronicaCoreMapper.SincroAppHelper.SalvaDocumento()"

        Dim alert_W As New AgronicaCoreScadenziario_BIZ.Alert_W
        Dim strerr As String = String.Empty

        Try

            Dim objImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim DTSalvaAllegato As New DataTable
            Dim SalvaAllegato As Integer = 0 'File System Default

            DTSalvaAllegato = objImpostazioni.Leggi2(2, objParametri_Server.SuperUserUsername, enum_Impostazioni_Utenti.SUPERUSER_DOCUMENTALE_SALVA_ALLEGATO_SU_DB, "", "", objParametri_Utenti)

            If Not IsNothing(DTSalvaAllegato) AndAlso DTSalvaAllegato.Rows.Count > 0 Then
                SalvaAllegato = CInt(DTSalvaAllegato.Rows(0).Item("Impostazione_Valore_1"))
            End If

            Dim objE As New AgronicaCoreScadenziario.Alert_Entita

            Dim Sa_Cod As Integer = 0
            Dim appezza As Integer = 0
            Dim Cod_Contatto As String = ""
            Dim Analisi_Testata_Cod As Integer = 0
            Dim PC_Testata_Cod As Integer = 0
            Dim Pua_Cod As Integer = 0
            Dim Id_Schema_Template As Integer = 0
            Dim ChkStorico As Integer = 0
            Dim Mac_Cod As Integer = 0

            '========================================================================================================================
            'Campi non più usati
            '------------------------------------------------------------------------------------------------------------------------
            Dim data_rilascio As Date = AGRODATAINIZIO
            Dim ente_rilascio As String = ""
            '========================================================================================================================

            If String.IsNullOrEmpty(Descrizione_Scadenza) Then
                Dim ObjTipologia = New Alert_Tipologia_R
                Dim dtTipologia As DataTable = ObjTipologia.Leggi(Id_Area, Id_Tipologia, "", False, objParametri_Server)
                If Not IsNothing(dtTipologia) AndAlso dtTipologia.Rows.Count = 1 Then
                    Descrizione_Scadenza = dtTipologia.Rows(0)("Nome")
                End If
            End If


            'Rename dell'allegato
            Dim ObjAgenda_W As New AgronicaCoreContabDAL.Agenda_W
            Nome_File = ObjAgenda_W.RenameAllegato(Piva, Id_Agenda, Nome_File, objParametri_Server)


            Dim validazione_flag As String = "0"
            Dim data_upload As DateTime = Now

            'Determino se è una scadenza o un documento
            Dim chkdocumento As Integer = 0

            If Not IsNothing(Nome_File) AndAlso Nome_File <> "" Then
                chkdocumento = 1 'Documento
                If Data_scadenza <> AGRODATAFINE Then
                    chkdocumento = 2 'Hybrid
                End If
            Else
                chkdocumento = 0 'Scadenza
            End If

            Dim Analisi_Campione_Cod As Integer = 0
            Dim Campo_Cod As Integer = 0
            Dim COM As Integer = 0
            Dim FOGLIO As Integer = 0
            Dim Id_Imp As Integer = 0
            Dim NUMERO As Integer = 0
            Dim Programmazione_Entita_Cod As Integer = 0
            Dim PROV As Integer = 0
            'Dim Ricetta_Operazione_cod As Integer = 0
            Dim SEZIONE As Integer = 0
            Dim SUBALTERNO As Integer = 0
            Dim TipoEntita_Cod As Integer = 0

            Dim LeggiConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim dt_Conf As DataTable = LeggiConfSiti.Leggi(6, "GestioneAllegati_Repository", "", "", objParametri_Server)
            Dim Percorso As String = dt_Conf.Rows(0).Item("Valore")

            'identifico il tipoentita_cod
            Dim objxTipoInput As New AgronicaCoreScadenziario.Tipo_Entita_Chiavi_R
            TipoEntita_Cod = objxTipoInput.GetTipoEntitaCod(Id_Tipologia, objParametri_Server,
                                                            Piva, Sa_Cod, appezza, Analisi_Testata_Cod,
                                                            PC_Testata_Cod, Pua_Cod,
                                                            Richiesta_Cod, Id_Agenda, Ricetta_Operazione_cod)


            'Preparazione Oggetto Entità
            objE = New AgronicaCoreScadenziario.Alert_Entita With {
                .ChkDocumento = chkdocumento,
                .ChkStorico = ChkStorico,
                .analisi_campione_cod = Analisi_Campione_Cod,
                .Appezza = appezza,
                .Campo_Cod = Campo_Cod,
                .COM = COM,
                .FOGLIO = FOGLIO,
                .ID_Agenda = Id_Agenda,
                .Id_Imp = Id_Imp,
                .NUMERO = NUMERO,
                .Piva = Piva,
                .PivaSuperUser = objParametri_Server.PivaSuperUser,
                .Programmazione_Entita_Cod = Programmazione_Entita_Cod,
                .PROV = PROV,
                .Ricetta_Operazione_cod = Ricetta_Operazione_cod,
                .Sa_Cod = Sa_Cod,
                .SEZIONE = SEZIONE,
                .SUBALTERNO = SUBALTERNO,
                .Cod_Contatto = Cod_Contatto,
                .TipoEntita_Cod = TipoEntita_Cod,
                .Analisi_Testata_Cod = Analisi_Testata_Cod,
                .PC_Testata_Cod = PC_Testata_Cod,
                .PUA_Cod = Pua_Cod,
                .Mac_Cod = Mac_Cod,
                .Richiesta_Cod = Richiesta_Cod,
                .Id_Schema_Template = Id_Schema_Template
            }

            'Dim ret_Id_Elenco As Integer = -1

            'scrivo 
            strerr = alert_W.Scrivi(Id_Tipologia, objE, Data_scadenza, Descrizione_Scadenza,
                                    Nome_File, Percorso,
                                    AGRODATAINIZIO, AGRODATAFINE,
                                    objParametri_Server, Note, ret_Id_Elenco, EntitaxIndici,
                                    ente_rilascio, validazione_flag,
                                    Username_Upload, data_upload,
                                    File_Allegato, Allegati_Documenti_Numero, SalvaAllegato, ID_App, Allegati_Documenti_Data:=Allegati_Documenti_Data)


        Catch ex As Exception
            strerr = ex.Message
        End Try

        Return strerr

    End Function

    Public Sub CancellaDocumento(ByVal unid As String)
        Dim allegatiScrivi As New Allegati_Documenti_W
        allegatiScrivi.Cancella_DocumentoAPP(unid, objParametri_Server)
    End Sub

    Public Sub ScriviDocumento(ByVal piva As String, ByVal documento As AgronicaCoreModelsSTD.documenti.Documento, ByVal unid As String, Optional ByVal cancellazione As Boolean = False)
        Dim DocumentoPerScarico = New DocumentoPerScarico
        DocumentoPerScarico.Documento_Cod = -1
        DocumentoPerScarico.Piva = piva
        DocumentoPerScarico.Descrizione = documento.Descrizione
        DocumentoPerScarico.Note = documento.Note
        DocumentoPerScarico.Data_Scadenza = documento.Data_Scadenza
        If DocumentoPerScarico.Data_Scadenza < AGRODATAINIZIO Then
            DocumentoPerScarico.Data_Scadenza = AGRODATAFINE
        End If
        DocumentoPerScarico.ID_Tipologia = documento.ID_Tipologia
        DocumentoPerScarico.Allegati = New List(Of DocumentoAllegato)
        For Each allegato In documento.Allegati
            DocumentoPerScarico.Allegati.Add(New DocumentoAllegato With {.FileName = allegato.FileName, .FileByte = allegato.FileByte})
        Next
        ScriviDocumento(DocumentoPerScarico, unid, cancellazione)
    End Sub


    Public Sub ScriviDocumentoPerImport(ByVal piva As String, ByVal documento As DocumentoAllegatoPerImport, ByVal unid As String, ByRef errorMessage As String)

        Try
            Dim DocumentoPerScarico = New DocumentoPerScarico

            Try

                DocumentoPerScarico.Documento_Cod = -1
                DocumentoPerScarico.Piva = piva
                DocumentoPerScarico.Descrizione = ""
                DocumentoPerScarico.Note = ""

                If Not IsDate(documento.Data_Scadenza) Then
                    DocumentoPerScarico.Data_Scadenza = AGRODATAFINE
                Else
                    DocumentoPerScarico.Data_Scadenza = documento.Data_Scadenza
                    If documento.Data_Scadenza < AGRODATAINIZIO Then
                        DocumentoPerScarico.Data_Scadenza = AGRODATAFINE
                    End If
                End If


                DocumentoPerScarico.ID_Tipologia = documento.Tipologia_Documento

                DocumentoPerScarico.cancellato = documento.Cancellato
                DocumentoPerScarico.Allegati_Documenti_Numero = documento.Chiave_Doc_Prefisso & documento.Chiave_Doc_Nr & documento.Chiave_Doc_Suffisso

                If IsDate(documento.Data_Documento) Then
                    DocumentoPerScarico.Data_Documento = documento.Data_Documento
                End If


                DocumentoPerScarico.Allegati = New List(Of DocumentoAllegato)

                DocumentoPerScarico.Allegati.Add(New DocumentoAllegato With {.FileName = documento.FileName, .FileByte = documento.FileByte})

                ScriviDocumento(DocumentoPerScarico, unid, False)


            Catch ex As Exception

                errorMessage = "Importazione documenti non riuscita!. " & ex.Message

            Finally

            End Try

        Catch ex As Exception
            errorMessage = ex.Message
        End Try

    End Sub


    Public Sub ScriviDocumento(ByVal Documento As DocumentoPerScarico, ByVal unid As String, Optional ByVal cancellazione As Boolean = False)

        Dim allegatiScrivi As New Allegati_Documenti_W
        Dim Data_Documento As DateTime = AGRODATAINIZIO
        Dim Numero_Documento As String = ""


        ' cancellazione documento app
        If cancellazione Then
            allegatiScrivi.Cancella_DocumentoAPP(unid, objParametri_Server)
        End If

        ' inserimento documento app
        If Not Documento.cancellato Then

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

            If IsDate(Documento.Data_Documento) Then
                Data_Documento = Documento.Data_Documento
            End If

            Numero_Documento = Documento.Allegati_Documenti_Numero


            allegatiScrivi.Scrivi_DocumentoAPP(
                DocumentoID,
                Documento.Piva,
                Documento.Documento_Cod,
                Documento.ID_Tipologia,
                Documento.Descrizione,
                Documento.Data_Scadenza,
                Documento.Note,
                FileName,
                RicettaOperazioneID,
                RicettaDestinazioneID,
                FileAllegato,
                objParametri_Server,
                Documento.Visita_Cod,
                Data_Documento:=Data_Documento,
                Numero_Documento:=Numero_Documento
            )

        End If

    End Sub

    Public Sub ScriviPosizione(ByVal posizione As String, ByVal destinazioni As List(Of String))

        Dim scriviGIS As New GisHelper
        Dim vWkt As String() = posizione.Split("|")
        Dim wkt As String = "POINT (" & vWkt(1).Trim() & ", " & vWkt(0).Trim() & ")"

        For Each destinazione In destinazioni
            Dim d As String() = destinazione.Split("|")
            scriviGIS.ScriviDatoCartografico(
                objParametri_Server,
                0,
                0,
                d(0),
                d(1),
                d(2),
                d(3),
                d(4),
                d(5),
                wkt,
                "-1",
                "1",
                enum_Gis_LayerElementiGrafici_std.AGENDA_RILIEVI,
                enum_GIS2012_TipoEntita.Destinazione_Agenda,
                0, 0, enum_TipoOperazioneDB.Scrittura, "", swapLatLong:=True)
        Next

    End Sub

End Class
