Imports System.Configuration
Imports System.Web
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreVarieDAL
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreModelsSTD.exceptions

Public Class Utility_Operazioni

    Public Shared Function Ricette_CancellaRicettaConvertiCosti(ricetta_cod As Integer, ricetta_operazione_cod As Integer, in_uso As Integer, APP_Ricetta_Operazione_ID As String, Optional ByVal usaTransazione As Boolean = True)

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))


        If ricetta_cod <= 0 Then
            Dim __ = Utils.MANCATA_TRADUZIONE
            r.Errore = "Il parametro ricetta_cod non è stato valorizzato."
            Return r
        End If

        If ricetta_operazione_cod <= 0 Then
            Dim __ = Utils.MANCATA_TRADUZIONE
            r.Errore = "Il parametro ricetta_operazione_cod non è stato valorizzato."
            Return r
        End If

        If in_uso <> 0 Then
            Dim __ = Utils.MANCATA_TRADUZIONE
            r.Errore = "Impossibile cancellare una ricetta in uso" ' AgronicaAgenda_2010.ImpossibileCancellareRicettaInUso
            Return r
        End If

        Try
            If usaTransazione Then
                'apro una transazione
                ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)
            End If

            Dim CDG_APP As New AgronicaCoreContabBIZ.CDG_APP
            Dim esito As Boolean = CDG_APP.ConvertiMovimentiCDGInterventiAPP(APP_Ricetta_Operazione_ID, objParametri_Server)

            If esito = False Then
                Throw New Exception("Errore durante la conversione dei costi")
            End If

            r = Ricette_Cancella(ricetta_cod, ricetta_operazione_cod, in_uso)

            If usaTransazione Then
                'Se è andato tutto bene
                ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server) 'Flag_Commit1_Rollback2
            End If


        Catch ex As Exception

            If usaTransazione Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server) 'Flag_Commit1_Rollback2
            End If

            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r

    End Function


    Public Shared Function Ricette_VerificaSeCostiCollegatiECancella(ricetta_cod As Integer, ricetta_operazione_cod As Integer, in_uso As Integer, APP_Ricetta_Operazione_ID As String, Optional ByVal usaTransazione As Boolean = True)
        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

        If ricetta_cod <= 0 Then
            Dim __ = Utils.MANCATA_TRADUZIONE
            r.Errore = "Il parametro ricetta_cod non è stato valorizzato."
            Return r
        End If

        If ricetta_operazione_cod <= 0 Then
            Dim __ = Utils.MANCATA_TRADUZIONE
            r.Errore = "Il parametro ricetta_operazione_cod non è stato valorizzato."
            Return r
        End If

        If in_uso <> 0 Then
            Dim __ = Utils.MANCATA_TRADUZIONE
            r.Errore = "Impossibile cancellare una ricetta in uso" ' AgronicaAgenda_2010.ImpossibileCancellareRicettaInUso
            Return r
        End If

        Dim CDG_APP As New AgronicaCoreContabBIZ.CDG_APP
        Dim dtMovimentiCDG = CDG_APP.LeggiRiferimentiCDGInterventiAPP(APP_Ricetta_Operazione_ID, objParametri_Server)

        If dtMovimentiCDG.Rows.Count > 0 Then  'Allora ci sono ore APP collegate
            r.RispostaOK = True
            Dim __ = Utils.MANCATA_TRADUZIONE
            r.RispostaStringa = "Esistono costi collegati quindi è impossibile esseguire la cancellazione" ' AgronicaAgenda_2010.EsistonoCostiCollegatiImpossibileCancellare
        Else
            r = Ricette_Cancella(ricetta_cod, ricetta_operazione_cod, in_uso, isBrogliaccio:=True, usaTransazione)
        End If

        Return r
    End Function

    Public Shared Function Ricette_Cancella(ricetta_cod As Integer, ricetta_operazione_cod As Integer, in_uso As Integer,
                                            Optional isBrogliaccio As Boolean = False, Optional ByVal usaTransazione As Boolean = True) As RispostaStandard
        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Dim objParametri_Utenti As AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))


        'Controllo permessi cancellazione...
        Dim ID_Attivita As enum_Security_Attivita = enum_Security_Attivita.Gest_Ricette
        If isBrogliaccio Then
            ID_Attivita = enum_Security_Attivita.Brogliaccio
        End If

        Dim permesso As Boolean = controlloPermessi(ID_Attivita, enum_Security_Operazione.Modifica)
        If permesso = False Then
            r.Errore = AgronicaCoreDataProvider.My.Resources.Gias.OperazioneNonCancellataWarning & "<br>" & "<br>" & AgronicaCoreDataProvider.My.Resources.Gias.MancanzaPermessiCancellazioneTipoOperazione
            Return r
        End If

        If ricetta_cod <= 0 Then
            r.Errore = "Il parametro ricetta_cod non è stato valorizzato."
            Return r
        End If

        If ricetta_operazione_cod <= 0 Then
            r.Errore = "Il parametro ricetta_operazione_cod non è stato valorizzato."
            Return r
        End If

        If in_uso <> 0 Then
            Dim __ = Utils.MANCATA_TRADUZIONE
            r.Errore = "Impossibile cancellare una ricetta in uso" ' AgronicaAgenda_2010.ImpossibileCancellareRicettaInUso
            Return r
        End If

        Dim a As New AgronicaCoreContabBIZ.Ricette_Operazioni_W
        Dim msgErr As String = a.Ricetta_Operazione_Cancella_ESeUnicaAncheLaRicettaPadre(ricetta_cod, ricetta_operazione_cod, objParametri_Server, usaTransazione, objParametri_Utenti:=objParametri_Utenti)

        If msgErr = "" Then
            r.RispostaOK = True
        Else
            r.Errore = msgErr
        End If

        Return r

    End Function

    Public Shared Function elimina_operazione_multipla(ByVal strChiaviComposite As String,
                                                       proseguiInCasoDiAlert As Boolean,
                                                       ByRef objParametri_Server As AgronicaCoreParametri,
                                                       ByRef objParametri_Utenti As AgronicaCoreParametri)

        Dim r As New RispostaStandard

        Dim msgErrBloccante As String = ""
        Dim msgErrNonBloccante As String = ""

        strChiaviComposite = strChiaviComposite.Split("|")(1)
        Dim listaChiaviComposite As String() = strChiaviComposite.Split(",")


        Dim lista_IdAgenda_Cancellati As New List(Of Integer)
        If listaChiaviComposite.Length > 1 Then
            'verifico permesso multi-cancellazione
            Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            Dim UtenteAbilitato_MultiCancellazione As Boolean = objPermessi.Controlla_Permessi_Utente(
                                                                HttpContext.Current.Session("ASG_Utente_Username"),
                                                                HttpContext.Current.Session("ASG_IdServizio"),
                                                                enum_Security_Attivita.ManutenzioneArchivi_MultiCancellazioneInterventi,
                                                                enum_Security_Operazione.Modifica,
                                                                Date.Now, "", objParametri_Utenti)

            If UtenteAbilitato_MultiCancellazione = False Then
                r.RispostaOK = False
                Dim __ = Utils.MANCATA_TRADUZIONE
                r.Errore = "Eliminare un operazione alla volta" ' Resources.AgronicaAgenda_2010.EliminareUnOperazioneAllaVolta
                Return r
            End If
        End If

        '-----------------------------------------------------
        '----------- CONNESSIONE E TRANSAZIONE ---------------
        ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)
        '-----------------------------------------------------
        Dim list_msgOperazioniCollegate As New List(Of String)

        Dim list_IdAgenda As New List(Of Integer)
        For Each chiaveComposita As String In listaChiaviComposite
            Dim chiave = chiaveComposita.Split("_")
            Dim id_agenda As String = chiave(1)
            If list_IdAgenda.Contains(id_agenda) = False Then
                list_IdAgenda.Add(id_agenda)
            End If
        Next

        For Each chiaveComposita As String In listaChiaviComposite
            Dim chiave = chiaveComposita.Split("_")
            Dim data As String = chiave(0)
            Dim id_agenda As String = chiave(1)
            Dim lav_cod As String = chiave(2)
            Dim blocco_flag As String = chiave(5)
            Dim veg_cod As String = chiave(6)
            Dim piva As String = If(chiave.Length > 7, chiave(7), "")


            Dim OUT_ErroreBloccante As Boolean = True

            Dim list_IdAgenda_DaCancellare As List(Of Integer) = list_IdAgenda.ToList()
            list_IdAgenda_DaCancellare.Remove(id_agenda)

            If Not lista_IdAgenda_Cancellati.Contains(id_agenda) Then
                Dim msgErr = elimina_operazione_singola_VerificaPermessi(piva, data,
                                                                         id_agenda, lav_cod, veg_cod,
                                                                         blocco_flag, proseguiInCasoDiAlert,
                                                                         objParametri_Server, objParametri_Utenti, OUT_ErroreBloccante,
                                                                         lista_IdAgenda_Cancellati:=lista_IdAgenda_Cancellati,
                                                                         list_msgOperazioniCollegate:=list_msgOperazioniCollegate,
                                                                         list_IdAgenda_DaCancellare:=list_IdAgenda_DaCancellare) & vbCrLf & vbCrLf

                If msgErr.Trim() <> "" Then
                    If OUT_ErroreBloccante Then
                        msgErrBloccante &= msgErr
                    Else
                        msgErrNonBloccante &= msgErr
                    End If
                End If

            End If

        Next

        If msgErrBloccante.Trim() = "" AndAlso msgErrNonBloccante.Trim() = "" Then
            'chiudi connessione e commit transazione
            ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server)
            ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)

            r.RispostaOK = True
            Dim __ = Utils.MANCATA_TRADUZIONE
            r.RispostaStringa = "Operazione cancellata" ' Resources.AgronicaAgenda_2010.OperazioneCancellata
        Else
            'inserimento fallito
            ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)

            r.RispostaOK = False

            'Utilizzo il campo RispostaConferma per indicare che ho bisogno di una conferma per andare avanti
            If msgErrBloccante.Trim() <> "" Then
                Dim __ = Utils.MANCATA_TRADUZIONE
                Dim testoErrore = "Operazione non cancellata: "
                If listaChiaviComposite.Count > 1 Then
                    testoErrore = "Operazioni non cancellate: "
                End If
                r.Errore = testoErrore & "<br>" & "<br>" & msgErrBloccante.Trim().Replace(vbCrLf, "<br>") ' Resources.AgronicaAgenda_2010.OperazioneNonCancellataWarning & "<br>" & "<br>" & msgErrBloccante.Trim().Replace(vbCrLf, "<br>")
                r.RispostaConferma = False
            Else
                If list_msgOperazioniCollegate.Count > 1 Then
                    Dim messaggio As String = Gias.OperazioniRegistrateInsiemeAltreContinuandoVerrannoTutteCancellate & NEWLINE
                    messaggio &= String.Join(NEWLINE, list_msgOperazioniCollegate)
                    messaggio &= NEWLINE & NEWLINE & Gias.SiDesideraProseguire

                    r.Errore = messaggio
                Else
                    r.Errore = msgErrNonBloccante.Trim().Replace(vbCrLf, "<br>")
                End If
                r.RispostaConferma = True
            End If

        End If

        Return r
    End Function

    Public Shared Function elimina_operazione_singola_VerificaPermessi(ByVal piva As String,
                                                                       ByVal data As String,
                                                                       ByVal id_agenda As String,
                                                                       ByVal lav_cod As String,
                                                                       ByVal veg_cod As String,
                                                                       ByVal blocco_flag As String,
                                                                       ByVal proseguiInCasoDiAlert As Boolean,
                                                                       ByRef objParametri_Server As AgronicaCoreParametri,
                                                                       ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                                       ByRef OUT_ErroreBloccante As Boolean,
                                                                        Optional lista_IdAgenda_Cancellati As List(Of Integer) = Nothing,
                                                                        Optional ByRef list_msgOperazioniCollegate As List(Of String) = Nothing,
                                                                       Optional ByRef list_IdAgenda_DaCancellare As List(Of Integer) = Nothing) As String

        Dim lav_des As String = ""
        Dim rOperazioni = New AgronicaCoreAnagrafeDAL.Operazioni_R
        lav_des = rOperazioni.Lav_Des_From_Lav_Cod(lav_cod, objParametri_Server)

        Try

            OUT_ErroreBloccante = True

            'Il controllo viene fatto lato client, ma non si sa mai...
            If blocco_flag = 1 Then
                Dim __ = Utils.MANCATA_TRADUZIONE
                Return "Impossibile Modificare l'operazione in quanto risulta bloccata" ' Resources.AgronicaAgenda_2010.ImpossibileModificareOperazioneBlocc
            End If

            'verifico permesso op contabili e magazzino
            'permessi op contabili e magazzino
            Dim permesso As Boolean = AgronicaCoreModello.Utility_Operazioni.PermessiOpContabiliEMagazzino(lav_cod, enum_TipoOperazioneDB.Cancellazione, objParametri_Server, objParametri_Utenti, HttpContext.Current.Session)
            If permesso = False Then
                Dim __ = Utils.MANCATA_TRADUZIONE
                Return "Non si hanno i permessi di cancellazione su questo gruppo di operazioni." ' AgronicaAgenda_2010.MancanzaPermessiCancellazioneSuGruppoDiOperazioni
            End If

            'Verifico Permessi per operazioni colturali
            Dim olav As New AgronicaCoreMetaSchemaDAL.Operazioni_R
            Dim gru_op As Integer = olav.Gru_Op_from_LavorazioneCod(CInt(lav_cod), objParametri_Server)
            If Not {6, 10, 20}.Contains(gru_op) Then
                Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
                permesso = objPermessi.Controlla_Permessi_Utente(
            HttpContext.Current.Session("ASG_Utente_Username"),
            HttpContext.Current.Session("ASG_IdServizio"),
            enum_Security_Attivita.Agenda_AccessoMenu_NG,
            enum_Security_Operazione.Modifica,
            Date.Now, "", objParametri_Utenti)
            End If
            If permesso = False Then
                Dim __ = Utils.MANCATA_TRADUZIONE
                Return "Non si hanno i permessi di cancellazione su questo gruppo di operazioni." ' AgronicaAgenda_2010.MancanzaPermessiCancellazioneSuGruppoDiOperazioni
            End If

            'Creo l'oggetto ParametriAgenda
            Dim objParametriAgenda As New ParametriAgenda
            If Not String.IsNullOrEmpty(piva) Then
                objParametriAgenda.Piva = piva
            End If
            objParametriAgenda.Data = data
            objParametriAgenda.Id_Agenda = id_agenda
            objParametriAgenda.Lav_Cod = lav_cod
            objParametriAgenda.Tipo_Operazione = CStr(CInt(enum_TipoOperazioneDB.Cancellazione))

            'pratica ecologica (eliminazione id_agenda)
            Select Case lav_cod
                Case LAVCOD_PRATICA_ECOLOGICA, LAVCOD_FORMAZIONE
                    Dim res As Boolean = OperazioneAgendaAudit_Cancella(objParametriAgenda, objParametri_Server)
                    If Not res Then
                        Dim __ = Utils.MANCATA_TRADUZIONE
                        Return "Non è stato possibile eliminare l'operazione." ' AgronicaAgenda_2010.ImpossibileEliminareOperazione
                    Else
                        Return ""
                    End If
            End Select

            If veg_cod <> "" AndAlso veg_cod > 0 Then

                Dim objSpecVeg As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
                Dim Dt As DataTable = objSpecVeg.SpecieVegetali_GestioneFiltroUtente_Leggi(veg_cod, 0,
                                                                                           "", "",
                                                                                           "", "",
                                                                                           objParametri_Utenti)
                If Dt.Rows.Count = 0 Then
                    Dim __ = Utils.MANCATA_TRADUZIONE
                    Return "Non è possibile modificare questa operazione, non si ha il permesso su questa specie" ' Resources.AgronicaAgenda_2010.NoModificaNoPermessoSpecie
                End If

            End If

            Dim matrice_delete(,) As String
            Dim messaggio1 As String = ""
            Dim messaggio2 As String = ""

            If ControllaOperazione(matrice_delete, objParametriAgenda, objParametri_Server,
                                   messaggio1, proseguiInCasoDiAlert, messaggio2,
                                   list_msgOperazioniCollegate:=list_msgOperazioniCollegate,
                                   list_IdAgenda_DaCancellare:=list_IdAgenda_DaCancellare) Then
                Dim res As Boolean = Operazione_Agenda_Utility.Cancella_Operazione_E_Collegate(objParametriAgenda,
                                                                                               objParametri_Server,
                                                                                               messaggio1,
                                                                                               True,
                                                                                               objParametri_Utenti:=objParametri_Utenti,
                                                                                               lista_IdAgenda_Cancellati:=lista_IdAgenda_Cancellati,
                                                                                               list_IdAgenda_DaCancellare:=list_IdAgenda_DaCancellare)
                If Not res Then
                    Dim __ = Utils.MANCATA_TRADUZIONE
                    Return "Operazione non cancellata: " & messaggio1 ' Resources.AgronicaAgenda_2010.OperazioneNonCancellataWarning
                End If
            Else
                OUT_ErroreBloccante = False
                Dim __ = Utils.MANCATA_TRADUZIONE
                Return "Avviso" & ": " & messaggio1 & " " & messaggio2 ' AgronicaAgenda_2010.Avviso & ": " & messaggio1 & " " & messaggio2
            End If

            Return ""
        Catch ex As GiasException
            Dim __ = Utils.MANCATA_TRADUZIONE
            Return lav_des & " del " & data & " (ID:" & id_agenda & ") Non è stato possibile eliminare l'operazione. " & ex.Message
        Catch ex As Exception
            Dim __ = Utils.MANCATA_TRADUZIONE
            Return "Non è stato possibile eliminare l'operazione." ' AgronicaAgenda_2010.ImpossibileEliminareOperazione & " " & ex.Message
        End Try

    End Function

    Public Shared Function InfoModificaOperazioneSingola(ByVal type As Integer, ByVal data As String, ByVal id_agenda As String, ByVal lav_cod As String, ByVal blocco_flag As String, ByVal veg_cod As Integer,
                                                           objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri, fromAngular As Boolean,
                                                           Optional piva As String = "") As RispostaStandard
        Dim r As New RispostaStandard


        Dim permessi = New PermessiUtente()
        Dim objParametriAgenda As New ParametriAgenda

        Dim TargetUrl As String = ""

        '---------------------------------------------
        ' CONTROLLI

        'verifico permesso op contabili e magazzino
        Dim permesso As Boolean = AgronicaCoreModello.Utility_Operazioni.PermessiOpContabiliEMagazzino(lav_cod, type, objParametri_Server, objParametri_Utenti, HttpContext.Current.Session)
        If permesso = False Then
            r.RispostaOK = False
            Dim mancata = Utils.MANCATA_TRADUZIONE
            r.Errore = "Mancanza Permessi Operazione Scelta Su Gruppo Di Operazioni"
            Return r
        End If

        Dim Permesso_Da_Controllare = enum_Security_Attivita.Agenda_AccessoMenu

        If fromAngular Then
            Permesso_Da_Controllare = enum_Security_Attivita.Agenda_AccessoMenu_NG
        End If

        'Verifico Permessi per operazioni colturali
        Dim olav As New AgronicaCoreMetaSchemaDAL.Operazioni_R
        Dim gru_op As Integer = olav.Gru_Op_from_LavorazioneCod(CInt(lav_cod), objParametri_Server)
        If Not {6, 10, 20}.Contains(gru_op) Then
            Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            permesso = objPermessi.Controlla_Permessi_Utente(
                       HttpContext.Current.Session("ASG_Utente_Username"),
                       HttpContext.Current.Session("ASG_IdServizio"),
                       Permesso_Da_Controllare,
                       type,
                       Date.Now,
                       "",
                       objParametri_Utenti)
        End If
        If permesso = False Then
            r.RispostaOK = False
            Dim mancata = Utils.MANCATA_TRADUZIONE
            r.Errore = "Mancanza Permessi Operazione Scelta Su Gruppo Di Operazioni"
            Return r
        End If


        'AUDIT

        Select Case lav_cod

            Case LAVCOD_PRATICA_ECOLOGICA,
                LAVCOD_FORMAZIONE

                Dim Audit_Cod As Integer = 0
                Dim objAgenda As New AgronicaCoreContabDAL.Agenda_R
                Dim DtAgenda As DataTable = objAgenda.Leggi("", 0, id_agenda, lav_cod, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                If DtAgenda IsNot Nothing AndAlso DtAgenda.Rows.Count > 0 Then
                    Audit_Cod = DtAgenda.Rows(0).Item("audit_cod")
                End If
                Dim TipoAudit As enum_AuditTipi

                Select Case lav_cod
                    Case LAVCOD_PRATICA_ECOLOGICA
                        TipoAudit = enum_AuditTipi.AuditTipi_PraticheEcologicheAPOT
                    Case LAVCOD_FORMAZIONE
                        TipoAudit = enum_AuditTipi.AuditTipi_Formazione
                End Select

                Dim LinkAgronicaAgenda2010 As String = ""

                If Not IsNothing(ConfigurationManager.AppSettings("LinkAgronicaAgenda2010")) AndAlso ConfigurationManager.AppSettings("LinkAgronicaAgenda2010") <> "" Then
                    LinkAgronicaAgenda2010 = ConfigurationManager.AppSettings("LinkAgronicaAgenda2010")
                End If

                TargetUrl = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_Sito_AgronicaAuditSicurezzaGlobalCoop_PassandoDirettamente_Parametri(
                                 Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                 TipoAudit,
                                 HttpContext.Current.Session("ASG_Utente_CodFiscale").ToString,
                                 objParametriAgenda.Piva,
                                 0,
                                 type,
                                 1,
                                 Audit_Cod,
                                 LinkAgronicaAgenda2010)

                r.RispostaOK = True
                r.RispostaStringa = TargetUrl

                Return r

        End Select





        If veg_cod > 0 Then

            Dim objSpecVeg As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
            Dim Dt As DataTable = objSpecVeg.SpecieVegetali_GestioneFiltroUtente_Leggi(veg_cod, 0, "", "", "", "", objParametri_Utenti)
            If Dt.Rows.Count = 0 Then
                r.RispostaOK = False
                Dim mancata = Utils.MANCATA_TRADUZIONE
                r.Errore = "No Eliminazione No Permesso Specie"
                Return r
            End If

        End If


        If lav_cod = LAVCOD_CURA Then
            Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            Dim UtenteAbilitato As Boolean = objPermessi.Controlla_Permessi_Utente(
                                        HttpContext.Current.Session("ASG_Utente_Username"),
                                        HttpContext.Current.Session("ASG_IdServizio"),
                                        enum_Security_Attivita.Agenda_Operazione_Di_Cura,
                                        enum_Security_Operazione.Lettura,
                                        Date.Now,
                                        "",
                                        objParametri_Utenti)
            If Not UtenteAbilitato Then
                r.RispostaOK = False
                Dim mancata = Utils.MANCATA_TRADUZIONE
                r.Errore = "No Permessi Operazione Cura"
                Return r
            End If
        End If


        'impedisco di modificare una raccolta senza selezionare prima il centro
        'If lav_cod = LAVCOD_RACCOLTA AndAlso objParametriAgenda.Sa_Cod = "0" Then
        '    Dim cf As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        '    Dim dt As DataTable = cf.Leggi(0, "RaccoltaNew", " valore = 'true' ", "", HttpContext.Current.Session("ASG_objParametri_Server"))
        '    If dt.Rows.Count = 1 Then
        '    Else
        '        r.RispostaOK = False
        '        Dim mancata = Utils.MANCATA_TRADUZIONE
        '        r.Errore = "Bisogna Selezionare Un Centro Per La Raccolta"
        '        Return r
        '    End If
        'End If

        '---------------------------------------------

        objParametriAgenda.Data = data
        objParametriAgenda.Id_Agenda = id_agenda
        objParametriAgenda.Sa_Cod = 0
        objParametriAgenda.Lav_Cod = lav_cod
        objParametriAgenda.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.QuadernoDiCampagna
        objParametriAgenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Reale
        objParametriAgenda.Programmazione_Cod = 0
        objParametriAgenda.Tipo_Operazione = type
        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Menu_BS

        Dim fromBootstrapToBootstrap As Boolean = False
        If fromAngular Then
            objParametriAgenda.SitoOrigine = Enum_SiteRedirector.GiasNG
        Else
            objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
            fromBootstrapToBootstrap = True
        End If
        If Not String.IsNullOrEmpty(piva) Then
            objParametriAgenda.Piva = piva
        End If

        Dim OpUtil As New Utility_Operazioni
        TargetUrl = OpUtil.LinkPagina_from_LavCod_NEW(lav_cod, objParametriAgenda,
                                                      fromBootstrapToBootstrap:=fromBootstrapToBootstrap)

        'Controllo blocchi se non sono in modifica
        If type <> 0 Then
            Try
                Dim matrice_delete(,) As String
                If ControllaOperazione(matrice_delete, objParametriAgenda, objParametri_Server, "", False, objParametri_Utenti:=objParametri_Utenti) Then
                End If
            Catch ex As Exception
                r.RispostaOK = False
                r.Errore = ex.Message
                Return r
            End Try
        End If

        r.RispostaOK = True
        r.RispostaStringa = TargetUrl

        Return r
    End Function

#Region "Link"

    Public Shared Function CopiaOperazioneSingola(ByRef dto As CopiaOperazioniDto, objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri) As CopiaOperazioneResult

        ' Lingua.Gias_InizializzaCultura_DaSession()
        ' TODO Razvan. Da aggiungere Gias_InizializzaCultura_DaSession appena mettiamo in piedi un metodo
        '              di gestire le traduzioni in Angular. In questo momento non esiste tale modalità.
        ' Appena funziona da aggiungere AgronicaAgenda_2010.NonÈPossibileCopiareQuestOperazionePlurale

        Dim id_agenda_array As String() = dto.id_agenda_checked.Split(",")
        Dim lav_cod_array As String() = dto.lav_cod_checked.Split(",")

        Dim id_agenda_copiabili_array As New List(Of String)
        Dim lav_cod_copiabili_array As New List(Of String)

        'Creo un nuovo elenco di operazioni d'agenda copiabili
        For i = 0 To id_agenda_array.Length - 1

            'Se è il -1, lo includo
            If id_agenda_array(i) = "-1" Then
                id_agenda_copiabili_array.Add(id_agenda_array(i))
                lav_cod_copiabili_array.Add(lav_cod_array(i))
                Continue For
            End If

            'Controllo che il lav_cod sia tra quelli copiabili
            If dto.LAV_COD_COPIABILI.Contains(lav_cod_array(i)) = False Then
                Continue For
            End If

            'Controllo se l'utente ha permessi di scrittura sull'operazione in oggetto

            'permessi op contabili e magazzino
            Dim permesso As Boolean = AgronicaCoreModello.Utility_Operazioni.PermessiOpContabiliEMagazzino(dto.lav_cod, enum_TipoOperazioneDB.Copia,
                                                                                                           objParametri_Server, objParametri_Utenti,
                                                                                                           HttpContext.Current.Session)
            If permesso = False Then
                Continue For
            End If

            'Verifico Permessi per operazioni colturali
            Dim op_R As New AgronicaCoreMetaSchemaDAL.Operazioni_R
            Dim gru_op As Integer = op_R.Gru_Op_from_LavorazioneCod(dto.lav_cod, objParametri_Server)

            If Not {6, 10, 20}.Contains(gru_op) Then
                Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
                permesso = objPermessi.Controlla_Permessi_Utente(
                           objParametri_Server.UtenteUsername,
                           5,
                           enum_Security_Attivita.Agenda_AccessoMenu_NG,
                           enum_Security_Operazione.Modifica,
                           Date.Now, "", objParametri_Utenti)
            End If

            If permesso = False Then
                Continue For
            End If

            'Aggiungo l'elemento tra i copiabili
            id_agenda_copiabili_array.Add(id_agenda_array(i))
            lav_cod_copiabili_array.Add(lav_cod_array(i))
        Next

        Dim result As New CopiaOperazioneResult
        If id_agenda_copiabili_array.Count = 0 OrElse (id_agenda_copiabili_array.Count = 1 AndAlso id_agenda_copiabili_array(0) = "-1") Then
            Throw New Exception("Non è possibile copiare quest'operazione plurale")
        End If

        dto.id_agenda_checked = String.Join(",", id_agenda_copiabili_array)

        Dim objImpre As New AgronicaCoreAnagrafeDAL.Imprese_Read
        Dim objCentriAziendali As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
        Dim objParametriAgenda As New ParametriAgenda

        result.data = dto.data
        result.Id_Agenda = id_agenda_copiabili_array(0)
        result.Lav_Cod = lav_cod_copiabili_array(0)
        result.Piva = dto.piva
        result.Sa_Cod = dto.sa_cod
        result.RagSoc = objImpre.RagSoc_from_Piva(dto.piva, objParametri_Server)
        result.SaNome = objCentriAziendali.SaNome_from_SaCod(dto.piva, dto.sa_cod, objParametri_Server)
        result.QueryStringIdAgendaChecked = dto.id_agenda_checked

        Return result
    End Function

    '##############################################################
    Public Function LinkPagina_from_LavCod(ByVal Lav_Cod As Integer,
                                           Optional ByVal LeggiFlagConfigurazioneSiti As Boolean = True) As String

        Dim PaginaLink As String = ""

        Select Case Lav_Cod

            '--- FERTILIZZAZIONE
            Case LAVCOD_FERTIRRIGAZIONE,
                    LAVCOD_CONCIMAZIONE_FOGLIARE,
                    LAVCOD_DISTRIBUZIONE_CONCIME,
                    LAVCOD_DISTRIBUZIONE_AMMENDANTI,
                    LAVCOD_SARCHIATURA_CONCIMAZIONE,
                    LAVCOD_TRATTAMENTO_ANTIBUTTERATURA

                PaginaLink = "../Operazioni/Trattamenti_2.aspx"

            '--- TRATTAMENTI
            Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO,
                    LAVCOD_DISERBO,
                    LAVCOD_DISSECCAMENTO,
                    LAVCOD_GEODISINFESTAZIONE,
                    LAVCOD_CONCIA_SEME,
                    LAVCOD_TRATTAMENTO_FITOREGOLATORE

                PaginaLink = "../Operazioni/Trattamenti_2.aspx"

            '--- DISTRIBUZIONE INSETTI
            Case LAVCOD_DISTRIBUZIONE_INSETTI
                PaginaLink = "../Operazioni/Distribuzione_Insetti.aspx"

            '--- LAVORAZIONi
            Case LAVCOD_ANDANAMENTO, LAVCOD_ARATURA, LAVCOD_DEFOGLIAZIONE, LAVCOD_ASPORTAZIONE_ORGANI_INFETTI, LAVCOD_ASSOLCATURA,
                 LAVCOD_CARICO_MANUALE_FRUTTA, LAVCOD_CIMATURA, LAVCOD_DIRADAMENTO_MANUALE, LAVCOD_DISSODAMENTO,
                 LAVCOD_ERPICATURA, LAVCOD_ESTIRPATURA, LAVCOD_ESPIANTO, LAVCOD_FALCIACONDIZIONATURA,
                 LAVCOD_FALCIATURA_ERBAI, LAVCOD_FORMAZIONE_ARGINELLI, LAVCOD_FRANGIZOLLATURA, LAVCOD_FRESATURA,
                 LAVCOD_IMBALLO_FIENO_ROTOLI, LAVCOD_INTERRAMENTO_PAGLIE, LAVCOD_LAVORAZIONE_TRA_FILA, LAVCOD_LAVORAZIONE_SU_FILA,
                 LAVCOD_LEGATURA, LAVCOD_LIVELLAMENTO, LAVCOD_MANUTENZIONE_ARGINI, LAVCOD_MESSA_DIMORA_PIANTE,
                 LAVCOD_MIETITREBBIATURA, LAVCOD_MINIMUM_TILLAGE, LAVCOD_PACCIAMATURA, LAVCOD_POTATURA_SECCA,
                 LAVCOD_POTATURA_VERDE, LAVCOD_PRESSATURA, LAVCOD_RACCOLTA_LEGNA_POTATURA, LAVCOD_RACCOLTA_MANUALE,
                 LAVCOD_RACCOLTA_MECCANICA, LAVCOD_RANGHINATURA, LAVCOD_RINCALZATURA, LAVCOD_RIPPATURA,
                 LAVCOD_RIPUNTATURA, LAVCOD_RIVOLTAMENTO_FORAGGIO, LAVCOD_RULLATURA, LAVCOD_SARCHIATURA,
                 LAVCOD_SCARIFICATURA, LAVCOD_SCASSO, LAVCOD_TRINCIATURA, LAVCOD_VANGATURA,
                 LAVCOD_ZAPPATURA, LAVCOD_GEBIATURA, LAVCOD_ROMPICROSTA, LAVCOD_LAVORAZIONE_CONBINATA,
                 LAVCOD_ERPICATURA_ROTANTE, LAVCOD_INTERVENTO_ANTIBRINA, LAVCOD_STRIGLIATURA, LAVCOD_PIRODISERBO
                'Andanamento, Aratura, Asportazione Organi Infetti, Assolcatura ,Carico Manuale Frutta, Cimatura
                'Diradamento Manuale, Dissodamento, Erpicatura, Erstirpatura, Espianto, Falciacondizionatura, Falciatura erbai
                'Frangizollatura, Fresatura, Imballo fieno e rotoli, Interramento paglie, Lavorazione tra fila
                'Lavorazione su fila, Legatura, Livellamento, Manutenzione argini, Messa dimora piante, Mietitrebbiatura
                'Minimum tillage, Pacciamatura , Potatura secca, Potatura verde, Pressatura, Raccolta legna potatura
                'Raccolta manuale, Raccolta meccanica, Ranghinatura, Rincalzatura, Rippatura, Ripuntatura
                'Rivoltaggio foraggio, Rullatura, Sarchiatura, Scarificatura, Scasso, Sod sedding
                'Trinciatura, Vangatura, Zappatura, Gebiatura, Rompicrosta, Lavorazione Combinata,Erpicatura Rotante, Strigliatura, pirodiserbo

                PaginaLink = "../Operazioni/Trattamenti_2.aspx"

            Case LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA,
                    LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE
                PaginaLink = "../Operazioni/Installazione_Trappole.aspx"

            '--- REINNESCO E RILIEVI TRAPPOLE
            Case LAVCOD_REINNESCO_TRAPPOLE, LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE
                PaginaLink = "../Operazioni/Reinnesco_Rilievi_Trappole.aspx" '"../Classi/Agro_Pages/Agenda_Pages/Operazioni_Pages/Reinnesco_Trappole/Reinnesco_Trappole.aspx"

            '--- RILIEVI
            Case LAVCOD_RILIEVO_AVVERSITA_CAMPO, LAVCOD_FASI_FENOLOGICHE, LAVCOD_RILIEVO_INDICI_MATURITA

                PaginaLink = "../Operazioni/RilieviBS.aspx"

            '--- RILIEVO ERBE INFESTANTI
            Case LAVCOD_RILIEVO_ERBE_INFESTANTI

                PaginaLink = "../Operazioni/Rilievi.aspx"

            '--- VISITA E RILIEVO RACCOLTA
            Case LAVCOD_VISITA, LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA
                PaginaLink = "../Operazioni/RilieviBS.aspx"

            '--- IRRIGAZIONE
            Case LAVCOD_IRRIGAZIONE
                PaginaLink = "../Operazioni/Irrigazione.aspx"

            '--- RILIEVO_PIOGGE
            Case LAVCOD_RILIEVO_PIOGGE
                PaginaLink = "../Operazioni/RilievoPiogge.aspx"

            '--- SEMINA E TRAPIANTO
            Case LAVCOD_SEMINA, LAVCOD_TRAPIANTO, LAVCOD_SOVESCIO, LAVCOD_SOD_SEDDING
                PaginaLink = "../Operazioni/Semina_E_Trapianto_1.aspx"

            'Operazione Deperecata
            '--- CURA
            'Case LAVCOD_CURA
            '    PaginaLink = "../GestioneMagazzini/OperazioneDiCura.aspx"

            '--- GESTIONE RIFIUTI
            Case LAVCOD_GESTIONE_RIFIUTI
                PaginaLink = "../Operazioni/GestioneRifiuti.aspx"

            Case Else
                PaginaLink = ""

        End Select

        Return PaginaLink

    End Function

    Public Function LinkPagina_from_LavCod(ByVal Lav_Cod As Integer,
                                           ByRef objParametriAgenda As ParametriAgenda,
                                           Optional ByVal PaginaSitoAgendaOrigine As enum_PagineAgenda_2010 = enum_PagineAgenda_2010.Menu,
                                           Optional ByVal LeggiFlagConfigurazioneSiti As Boolean = True,
                                           Optional ByVal DocumentoRicevutoLight As Boolean = False,
                                           Optional ByVal ServizioCod As Integer = 0,
                                           Optional ByVal Parametri_Aggiuntivi_for_Redirect As JObject = Nothing
                                           ) As String

        Dim PaginaLink As String = ""

        Dim objParametri_Server As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim objParametri_Utenti As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

        If PaginaSitoAgendaOrigine = enum_PagineAgenda_2010.Menu Then
            Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim dtConfigSiti As DataTable = objConfigSiti.Leggi(0, "MenuAgendaBS", "", "", objParametri_Server)
            If Not IsNothing(dtConfigSiti) AndAlso dtConfigSiti.Rows.Count > 0 AndAlso LCase(dtConfigSiti.Rows(0).Item("Valore")) = "true" Then
                PaginaSitoAgendaOrigine = enum_PagineAgenda_2010.Menu_BS
            End If
        End If

        objParametriAgenda.SitoDestinazione = Enum_SiteRedirector.Sito_AgronicaAgenda_2010

        Select Case Lav_Cod

            '--- GESTIONE COSTI
            Case LAVCOD_COSTI_CDG

                PaginaLink = "../AnalisiCostiProduzione/GestioneCosti.aspx"

                PaginaLink &= "?p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder) &
                      "&id_agenda=" & Stringa_Codifica(objParametriAgenda.Id_Agenda, AgroKey_EncoderDecoder) &
                      "&origine=" & Stringa_Codifica("../GestioneMagazzini/GestioneMagazziniBS.aspx", AgroKey_EncoderDecoder) &
                      "&entrata_diretta=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Nothing) &
                      "&op=" & Stringa_Codifica(objParametriAgenda.Tipo_Operazione, AgroKey_EncoderDecoder, Nothing)

            '--- LIQUIDAZIONE SOCI
            Case LAVCOD_PROCEDURA_LIQUIDAZIONE_SOCI

                PaginaLink = "../GestioneContabilita/Liquidazione/Liquidazione.aspx"

            '--- GESTIONE MAGAZZINI
            Case LAVCOD_SCARICO, LAVCOD_CARICO, LAVCOD_VENDITA, LAVCOD_ACQUISTO, LAVCOD_TRASFERIMENTO

                Dim gestioneCaricoScaricoDocContab = SeNuovaGestioneCaricoScarico(Lav_Cod,
                                                                                  objParametriAgenda,
                                                                                  objParametri_Server,
                                                                                  DocumentoRicevutoLight:=DocumentoRicevutoLight)

                If gestioneCaricoScaricoDocContab Then
                    PaginaLink = PaginaLink_CaricoScaricoDocContab(Lav_Cod, objParametriAgenda, PaginaSitoAgendaOrigine, ServizioCod)
                Else
                    PaginaLink = PaginaLink_GestioneMagazzini(Lav_Cod, objParametriAgenda, PaginaSitoAgendaOrigine, ServizioCod)
                End If

            '--- DOCUMENTI CONTABILI
            Case LAVCOD_FATTURA_EMESSA, LAVCOD_FATTURA_RICEVUTA, LAVCOD_BOLLA_RICEVUTA, LAVCOD_BOLLA_EMESSA

                If SeSetupDoc2021(objParametri_Server) Then

                    PaginaLink = VirtualPathUtility.ToAbsolute("~/GestioneContabilita/DocContabile.aspx") &
                        "?p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder) &
                        "&o=" & Stringa_Codifica(objParametriAgenda.Tipo_Operazione, AgroKey_EncoderDecoder) &
                        "&s=" & Stringa_Codifica(objParametriAgenda.Sa_Cod, AgroKey_EncoderDecoder) &
                        "&i=" & Stringa_Codifica(objParametriAgenda.Id_Agenda, AgroKey_EncoderDecoder) &
                        "&l=" & Stringa_Codifica(Lav_Cod, AgroKey_EncoderDecoder) &
                        "&md=" & Stringa_Codifica(0, AgroKey_EncoderDecoder) &
                        "&orig=" & Stringa_Codifica(PaginaSitoAgendaOrigine, AgroKey_EncoderDecoder) &
                        "&ricercatype=" & Stringa_Codifica("", AgroKey_EncoderDecoder) &
                        "&ricercadoc=" & Stringa_Codifica("", AgroKey_EncoderDecoder) &
                        "&ifr=" & Stringa_Codifica(0, AgroKey_EncoderDecoder) &
                        "&sc=" & Stringa_Codifica(ServizioCod, AgroKey_EncoderDecoder)

                Else

                    Dim tipo As Integer = 0
                    'objParametriAgenda.Sa_Cod=0 per tutti i doc contabili
                    PaginaLink = VirtualPathUtility.ToAbsolute("~/GestioneContabilita/DocumentoContabileGenerico.aspx") &
                        "?p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder) &
                        "&o=" & Stringa_Codifica(objParametriAgenda.Tipo_Operazione, AgroKey_EncoderDecoder) &
                        "&s=" & Stringa_Codifica(0, AgroKey_EncoderDecoder) &
                        "&i=" & Stringa_Codifica(objParametriAgenda.Id_Agenda, AgroKey_EncoderDecoder) &
                        "&l=" & Stringa_Codifica(Lav_Cod, AgroKey_EncoderDecoder) &
                        "&d=" & Stringa_Codifica(CStr(objParametriAgenda.Data), AgroKey_EncoderDecoder) &
                        "&tf=" & Stringa_Codifica(tipo, AgroKey_EncoderDecoder) &
                        "&rs=" & Stringa_Codifica(objParametriAgenda.RagSoc, AgroKey_EncoderDecoder) &
                        "&orig=" & Stringa_Codifica(PaginaSitoAgendaOrigine, AgroKey_EncoderDecoder) &
                        "&sc=" & Stringa_Codifica(ServizioCod, AgroKey_EncoderDecoder)

                End If

            '--- NOTE ACCREDITO
            Case LAVCOD_NOTA_ACCREDITO_RICEVUTA, LAVCOD_NOTA_ACCREDITO_EMESSA

                Dim tipo As Integer = 0
                'objParametriAgenda.Sa_Cod=0 per tutti i doc contabili
                PaginaLink = VirtualPathUtility.ToAbsolute("~/GestioneContabilita/DocumentoContabileGenerico.aspx") &
                    "?p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder) &
                    "&o=" & Stringa_Codifica(objParametriAgenda.Tipo_Operazione, AgroKey_EncoderDecoder) &
                    "&s=" & Stringa_Codifica(0, AgroKey_EncoderDecoder) &
                    "&i=" & Stringa_Codifica(objParametriAgenda.Id_Agenda, AgroKey_EncoderDecoder) &
                    "&l=" & Stringa_Codifica(Lav_Cod, AgroKey_EncoderDecoder) &
                    "&d=" & Stringa_Codifica(CStr(objParametriAgenda.Data), AgroKey_EncoderDecoder) &
                    "&tf=" & Stringa_Codifica(tipo, AgroKey_EncoderDecoder) &
                    "&rs=" & Stringa_Codifica(objParametriAgenda.RagSoc, AgroKey_EncoderDecoder) &
                    "&orig=" & Stringa_Codifica(PaginaSitoAgendaOrigine, AgroKey_EncoderDecoder) &
                    "&sc=" & Stringa_Codifica(ServizioCod, AgroKey_EncoderDecoder)

            '--- FERTILIZZAZIONE
            Case LAVCOD_FERTIRRIGAZIONE,
                 LAVCOD_CONCIMAZIONE_FOGLIARE,
                 LAVCOD_DISTRIBUZIONE_CONCIME,
                 LAVCOD_DISTRIBUZIONE_AMMENDANTI,
                 LAVCOD_SARCHIATURA_CONCIMAZIONE,
                 LAVCOD_TRATTAMENTO_ANTIBUTTERATURA


                If LeggiFlagConfigurazioneSiti Then
                    Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                    Dim dtConfigSiti As DataTable

                    dtConfigSiti = objConfigSiti.Leggi(0, "OperazioniAgendaNG", "", "", objParametri_Server)
                    If Not IsNothing(dtConfigSiti) AndAlso dtConfigSiti.Rows.Count > 0 AndAlso LCase(dtConfigSiti.Rows(0).Item("Valore")) = "true" AndAlso
                        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.GiasNG Then

                        Dim Parametri_Aggiuntivi As New JObject
                        Parametri_Aggiuntivi.Item("Id_Agenda") = objParametriAgenda.Id_Agenda
                        Parametri_Aggiuntivi.Item("TipoOperazioneDB") = objParametriAgenda.Tipo_Operazione
                        Parametri_Aggiuntivi.Item("QSF") = objParametriAgenda.QueryStringFiltrino

                        MenuBS_2017_RedirectGestione.RedirectGenerico(objParametriAgenda.Piva, Enum_SiteRedirector.GiasNG,
                                                                      enum_PagineGiasNG.Pagina_Edit_Attivita, PaginaLink,
                                                                      objParametri_Server, Parametri_Aggiuntivi)

                        objParametriAgenda.SitoDestinazione = Enum_SiteRedirector.GiasNG
                    Else
                        PaginaLink = "../Operazioni/Trattamenti_2.aspx"
                    End If
                Else
                    PaginaLink = "../Operazioni/Trattamenti_2.aspx"
                End If

            '--- TRATTAMENTI
            Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO,
                 LAVCOD_DISERBO,
                 LAVCOD_DISSECCAMENTO,
                 LAVCOD_GEODISINFESTAZIONE,
                 LAVCOD_CONCIA_SEME,
                 LAVCOD_TRATTAMENTO_FITOREGOLATORE

                If LeggiFlagConfigurazioneSiti Then


                    Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                    Dim dtConfigSiti As DataTable

                    dtConfigSiti = objConfigSiti.Leggi(0, "OperazioniAgendaNG", "", "", objParametri_Server)
                    If Not IsNothing(dtConfigSiti) AndAlso dtConfigSiti.Rows.Count > 0 AndAlso LCase(dtConfigSiti.Rows(0).Item("Valore")) = "true" AndAlso
                        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.GiasNG Then

                        Dim Parametri_Aggiuntivi As New JObject
                        Parametri_Aggiuntivi.Item("Id_Agenda") = objParametriAgenda.Id_Agenda
                        Parametri_Aggiuntivi.Item("TipoOperazioneDB") = objParametriAgenda.Tipo_Operazione
                        Parametri_Aggiuntivi.Item("QSF") = objParametriAgenda.QueryStringFiltrino

                        MenuBS_2017_RedirectGestione.RedirectGenerico(objParametriAgenda.Piva, Enum_SiteRedirector.GiasNG,
                                                                      enum_PagineGiasNG.Pagina_Edit_Attivita, PaginaLink,
                                                                      objParametri_Server, Parametri_Aggiuntivi)

                        objParametriAgenda.SitoDestinazione = Enum_SiteRedirector.GiasNG

                    Else
                        PaginaLink = "../Operazioni/Trattamenti_2.aspx"
                    End If
                Else
                    PaginaLink = "../Operazioni/Trattamenti_2.aspx"
                End If

            '--- DISTRIBUZIONE INSETTI
            Case LAVCOD_DISTRIBUZIONE_INSETTI

                PaginaLink = "../Operazioni/Distribuzione_Insetti.aspx"

            '--- LAVORAZIONI
            Case LAVCOD_ANDANAMENTO, LAVCOD_ARATURA, LAVCOD_DEFOGLIAZIONE, LAVCOD_ASPORTAZIONE_ORGANI_INFETTI, LAVCOD_ASSOLCATURA,
                 LAVCOD_CARICO_MANUALE_FRUTTA, LAVCOD_CIMATURA, LAVCOD_DIRADAMENTO_MANUALE, LAVCOD_DISSODAMENTO,
                 LAVCOD_ERPICATURA, LAVCOD_ESTIRPATURA, LAVCOD_ESPIANTO, LAVCOD_FALCIACONDIZIONATURA, LAVCOD_FALCIATURA_ERBAI,
                 LAVCOD_FORMAZIONE_ARGINELLI, LAVCOD_FRANGIZOLLATURA, LAVCOD_FRESATURA, LAVCOD_IMBALLO_FIENO_ROTOLI,
                 LAVCOD_INTERRAMENTO_PAGLIE, LAVCOD_LAVORAZIONE_TRA_FILA, LAVCOD_LAVORAZIONE_SU_FILA, LAVCOD_LEGATURA,
                 LAVCOD_LIVELLAMENTO, LAVCOD_MANUTENZIONE_ARGINI, LAVCOD_MESSA_DIMORA_PIANTE, LAVCOD_MIETITREBBIATURA,
                 LAVCOD_MINIMUM_TILLAGE, LAVCOD_PACCIAMATURA, LAVCOD_POTATURA_SECCA, LAVCOD_POTATURA_VERDE, LAVCOD_PRESSATURA,
                 LAVCOD_RACCOLTA_LEGNA_POTATURA, LAVCOD_RACCOLTA_MANUALE, LAVCOD_RACCOLTA_MECCANICA, LAVCOD_RANGHINATURA,
                 LAVCOD_RINCALZATURA, LAVCOD_RIPPATURA, LAVCOD_RIPUNTATURA, LAVCOD_RIVOLTAMENTO_FORAGGIO, LAVCOD_RULLATURA,
                 LAVCOD_SARCHIATURA, LAVCOD_SCARIFICATURA, LAVCOD_SCASSO, LAVCOD_TRINCIATURA, LAVCOD_VANGATURA, LAVCOD_ZAPPATURA,
                 LAVCOD_GEBIATURA, LAVCOD_ROMPICROSTA, LAVCOD_LAVORAZIONE_CONBINATA, LAVCOD_ERPICATURA_ROTANTE, LAVCOD_INTERVENTO_ANTIBRINA,
                 LAVCOD_ALTRE_OPERAZIONI, LAVCOD_STRIGLIATURA, LAVCOD_PIRODISERBO, LAVCOD_ABBATTIMENTOIMPIANTI

                If LeggiFlagConfigurazioneSiti Then

                    Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                    Dim dtConfigSiti As DataTable

                    dtConfigSiti = objConfigSiti.Leggi(0, "OperazioniAgendaNG", "", "", objParametri_Server)
                    If Not IsNothing(dtConfigSiti) AndAlso dtConfigSiti.Rows.Count > 0 AndAlso LCase(dtConfigSiti.Rows(0).Item("Valore")) = "true" AndAlso
                        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.GiasNG Then

                        Dim Parametri_Aggiuntivi As New JObject
                        Parametri_Aggiuntivi.Item("Id_Agenda") = objParametriAgenda.Id_Agenda
                        Parametri_Aggiuntivi.Item("TipoOperazioneDB") = objParametriAgenda.Tipo_Operazione
                        Parametri_Aggiuntivi.Item("QSF") = objParametriAgenda.QueryStringFiltrino

                        MenuBS_2017_RedirectGestione.RedirectGenerico(objParametriAgenda.Piva, Enum_SiteRedirector.GiasNG,
                                                                      enum_PagineGiasNG.Pagina_Edit_Attivita, PaginaLink,
                                                                      objParametri_Server, Parametri_Aggiuntivi)

                        objParametriAgenda.SitoDestinazione = Enum_SiteRedirector.GiasNG

                    Else


                        PaginaLink = "../Operazioni/Trattamenti_2.aspx"
                    End If

                Else

                    PaginaLink = "../Operazioni/Trattamenti_2.aspx"

                End If

            '--- INSTALLAZIONE TRAPPOLE
            Case LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA,
                 LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE

                PaginaLink = "../Operazioni/Installazione_Trappole.aspx"

            '--- REINNESCO/RILIEVI TRAPPOLE
            Case LAVCOD_REINNESCO_TRAPPOLE, LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE

                PaginaLink = "../Operazioni/Reinnesco_Rilievi_Trappole.aspx"

            '--- RILIEVO ERBE INFESTANTI
            Case LAVCOD_RILIEVO_ERBE_INFESTANTI

                PaginaLink = "../Operazioni/Rilievi.aspx"

            '--- RILIEVI
            Case LAVCOD_RILIEVO_AVVERSITA_CAMPO, LAVCOD_FASI_FENOLOGICHE, LAVCOD_RILIEVO_INDICI_MATURITA

                If LeggiFlagConfigurazioneSiti Then

                    Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                    Dim dtConfigSiti As DataTable = objConfigSiti.Leggi(0, "RilieviBS", "", "", objParametri_Server)

                    If Not IsNothing(dtConfigSiti) AndAlso dtConfigSiti.Rows.Count > 0 AndAlso LCase(dtConfigSiti.Rows(0).Item("Valore")) = "true" Then
                        PaginaLink = "../Operazioni/RilieviBS.aspx"
                    Else
                        PaginaLink = "../Operazioni/Rilievi.aspx"
                    End If

                Else

                    PaginaLink = "../Operazioni/Rilievi.aspx"

                End If

            '--- RILIEVI
            Case LAVCOD_VISITA, LAVCOD_DANNI_RACCOLTA, LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA

                PaginaLink = "../Operazioni/RilieviBS.aspx"

            '--- IRRIGAZIONE
            Case LAVCOD_IRRIGAZIONE

                If LeggiFlagConfigurazioneSiti Then

                    Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                    Dim dtConfigSiti As DataTable = objConfigSiti.Leggi(0, "IrrigazioneBS", "", "", objParametri_Server)

                    If Not IsNothing(dtConfigSiti) AndAlso dtConfigSiti.Rows.Count > 0 AndAlso LCase(dtConfigSiti.Rows(0).Item("Valore")) = "true" Then
                        PaginaLink = "../Operazioni/IrrigazioneBS.aspx"
                    Else
                        PaginaLink = "../Operazioni/Irrigazione.aspx"
                    End If

                Else

                    PaginaLink = "../Operazioni/Irrigazione.aspx"

                End If

            '--- RILIEVO PIOGGE
            Case LAVCOD_RILIEVO_PIOGGE

                PaginaLink = "../Operazioni/RilievoPiogge.aspx"

            '--- TRATTAMENTI
            Case LAVCOD_SEMINA, LAVCOD_TRAPIANTO, LAVCOD_SOVESCIO, LAVCOD_SOD_SEDDING

                If LeggiFlagConfigurazioneSiti Then

                    Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                    Dim dtConfigSiti As DataTable

                    dtConfigSiti = objConfigSiti.Leggi(0, "OperazioniAgendaNG", "", "", objParametri_Server)
                    If Not IsNothing(dtConfigSiti) AndAlso dtConfigSiti.Rows.Count > 0 AndAlso LCase(dtConfigSiti.Rows(0).Item("Valore")) = "true" AndAlso
                        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.GiasNG Then

                        Dim Parametri_Aggiuntivi As New JObject
                        Parametri_Aggiuntivi.Item("Id_Agenda") = objParametriAgenda.Id_Agenda
                        Parametri_Aggiuntivi.Item("TipoOperazioneDB") = objParametriAgenda.Tipo_Operazione
                        Parametri_Aggiuntivi.Item("QSF") = objParametriAgenda.QueryStringFiltrino

                        MenuBS_2017_RedirectGestione.RedirectGenerico(objParametriAgenda.Piva, Enum_SiteRedirector.GiasNG,
                                                                      enum_PagineGiasNG.Pagina_Edit_Attivita, PaginaLink,
                                                                      objParametri_Server, Parametri_Aggiuntivi)

                        objParametriAgenda.SitoDestinazione = Enum_SiteRedirector.GiasNG

                    Else
                        dtConfigSiti = objConfigSiti.Leggi(0, "SeminaBS", "", "", objParametri_Server)

                        If Not IsNothing(dtConfigSiti) AndAlso dtConfigSiti.Rows.Count > 0 AndAlso LCase(dtConfigSiti.Rows(0).Item("Valore")) = "true" Then
                            PaginaLink = "../Operazioni/Trattamenti_2.aspx"
                        Else
                            PaginaLink = "../Operazioni/Semina_E_Trapianto_1.aspx"
                        End If
                    End If

                Else

                    PaginaLink = "../Operazioni/Semina_E_Trapianto_1.aspx"

                End If

            '--- RACCOLTA
            Case LAVCOD_RACCOLTA
                Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                Dim dtConfigSiti As DataTable

                dtConfigSiti = objConfigSiti.Leggi(0, "OperazioniAgendaNG", "", "", objParametri_Server)
                If Not IsNothing(dtConfigSiti) AndAlso dtConfigSiti.Rows.Count > 0 AndAlso LCase(dtConfigSiti.Rows(0).Item("Valore")) = "true" AndAlso
                        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.GiasNG Then

                    Dim Parametri_Aggiuntivi As New JObject
                    Parametri_Aggiuntivi.Item("Id_Agenda") = objParametriAgenda.Id_Agenda
                    Parametri_Aggiuntivi.Item("TipoOperazioneDB") = objParametriAgenda.Tipo_Operazione
                    Parametri_Aggiuntivi.Item("QSF") = objParametriAgenda.QueryStringFiltrino

                    MenuBS_2017_RedirectGestione.RedirectGenerico(objParametriAgenda.Piva, Enum_SiteRedirector.GiasNG,
                                                                      enum_PagineGiasNG.Pagina_Edit_Attivita, PaginaLink,
                                                                      objParametri_Server, Parametri_Aggiuntivi)

                    objParametriAgenda.SitoDestinazione = Enum_SiteRedirector.GiasNG

                Else

                    PaginaLink = PaginaLink_Raccolta(objParametriAgenda,
                                                 objParametri_Server,
                                                 PaginaSitoAgendaOrigine)
                End If

            '--- POST RACCOLTA
            Case LAVCOD_TRATTAMENTO_POST_RACCOLTA

                PaginaLink = "../Operazioni/Trattamenti_PostRaccolta.aspx"

            '--- DICHIARAZIONI
            Case LAVCOD_FERTILIZZAZIONI_DICHIARAZIONE_NON_UTILIZZO, LAVCOD_TRATTAMENTO_DICHIARAZIONE_NON_UTILIZZO

                PaginaLink = "../Operazioni/Trattamenti_2.aspx"
                'PaginaLink = "../Operazioni/NonUtilizzo.aspx"

            ' Operazione Deperecata
            '--- CURA                    
            'Case LAVCOD_CURA

            '    PaginaLink = "../GestioneMagazzini/OperazioneDiCura.aspx"

            '--- MANUTENZIONE MACCHINE
            Case LAVCOD_MANUTENZIONE_MACCHINE, LAVCOD_REVISIONE_MACCHINE

                PaginaLink = "../Operazioni/ManutenzioneMacchine.aspx"

            '--- GESTIONE RIFIUTI
            Case LAVCOD_GESTIONE_RIFIUTI

                PaginaLink = "../Operazioni/GestioneRifiuti.aspx"

            '--- ZOO: CARICO
            Case LAVCOD_INCREMENTO_CONSISTENZE_ZOO, LAVCOD_NASCITA_ANIMALI, LAVCOD_ACQUISTO_ANIMALI

                PaginaLink = "../Zoo/Zoo_Carico.aspx"

            '--- ZOO: SPOSTAMENTI
            Case LAVCOD_SPOSTAMENTI_ZOO

                PaginaLink = "../Zoo/Zoo_Spostamento.aspx"

            '--- ZOO: ALIMENTAZIONE
            Case LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MANUALE_FORAGGI,
                 LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MECCANICA_FORAGGI,
                 LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MANUALE_MANGIMI,
                 LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MECCANICA_MANGIMI

                PaginaLink = "../Zoo/Zoo_Alimentazione.aspx"

            '--- ZOO: PESATURA
            Case LAVCOD_PESATURA_ANIMALI

                PaginaLink = "../Zoo/Zoo_Pesatura.aspx"

            '--- ZOO: SCARICO
            Case LAVCOD_MORTE_ANIMALI, LAVCOD_MACELLAZIONE_ANIMALI, LAVCOD_DECREMENTO_CONSISTENZE_ZOO, LAVCOD_VENDITA_ANIMALI, LAVCOD_TRASFERIMENTO_ANIMALI

                PaginaLink = "../Zoo/Zoo_Scarico.aspx"

            '--- ZOO: TRATTAMENTI
            Case LAVCOD_CUREMEDICAMENTI_ANIMALI

                PaginaLink = "../Zoo/Zoo_Trattamento.aspx"

            '--- ZOO: ALTRE LAVORAZIONI
            Case LAVCOD_ALTRE_LAVORAZIONI_ZOO

                PaginaLink = "../Zoo/Zoo_Altre_Lavorazioni.aspx"

           '--- CONFUSIONE/DISORIENTAMENTO SESSUALE, PASCOLAMENTO PROPRIO, PASCOLAMENTO TERZI
            Case LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE,
                LAVCOD_PASCOLAMENTO_PROPRIO,
                LAVCOD_PASCOLAMENTO_TERZI

                Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                Dim dtConfigSiti As DataTable

                dtConfigSiti = objConfigSiti.Leggi(0, "OperazioniAgendaNG", "", "", objParametri_Server)

                Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
                Dim UtenteAbilitato = objPermessi.Controlla_Permessi_Utente(
                                        objParametri_Utenti.UtenteUsername,
                                        5,
                                        enum_Security_Attivita.Agenda_AccessoMenu_NG,
                                        objParametriAgenda.Tipo_Operazione,
                                        Date.Now,
                                        "",
                                        objParametri_Utenti)

                If Not IsNothing(dtConfigSiti) AndAlso dtConfigSiti.Rows.Count > 0 AndAlso LCase(dtConfigSiti.Rows(0).Item("Valore")) = "true" AndAlso UtenteAbilitato Then
                    Dim Parametri_Aggiuntivi As New JObject
                    Parametri_Aggiuntivi.Item("Id_Agenda") = objParametriAgenda.Id_Agenda
                    Parametri_Aggiuntivi.Item("TipoOperazioneDB") = objParametriAgenda.Tipo_Operazione
                    Parametri_Aggiuntivi.Item("QSF") = objParametriAgenda.QueryStringFiltrino

                    Dim Ricetta_Cod = MenuBS_2017_RedirectGestione.getValueParametriAggiuntivi(Parametri_Aggiuntivi_for_Redirect, "Ricetta_Cod")
                    If Not IsNothing(Ricetta_Cod) AndAlso Not String.IsNullOrEmpty(Ricetta_Cod) Then
                        Parametri_Aggiuntivi.Item("Ricetta_Cod") = Parametri_Aggiuntivi_for_Redirect.Item("Ricetta_Cod")
                    End If

                    Dim Ricetta_Operazione_Cod = MenuBS_2017_RedirectGestione.getValueParametriAggiuntivi(Parametri_Aggiuntivi_for_Redirect, "Ricetta_Operazione_Cod")
                    If Not IsNothing(Ricetta_Operazione_Cod) AndAlso Not String.IsNullOrEmpty(Ricetta_Operazione_Cod) Then
                        Parametri_Aggiuntivi.Item("Ricetta_Operazione_Cod") = Parametri_Aggiuntivi_for_Redirect.Item("Ricetta_Operazione_Cod")
                    End If

                    Select Case objParametriAgenda.TipoOperazioneAgenda
                        Case enum_Tipo_Operazione_Agenda.QuadernoDiCampagna

                            Parametri_Aggiuntivi.Item("TipoOperazioneAgenda") = AgronicaCoreModelsSTD.attivita.Attivita.Tipo_Attivita.QuadernoDiCampagna

                        Case enum_Tipo_Operazione_Agenda.Ricetta, enum_Tipo_Operazione_Agenda.RicettaBrogliaccio

                            Parametri_Aggiuntivi.Item("TipoOperazioneAgenda") = AgronicaCoreModelsSTD.attivita.Attivita.Tipo_Attivita.Ricetta
                            Parametri_Aggiuntivi.Item("TipoRicetta") = objParametriAgenda.TipoRicetta

                            If objParametriAgenda.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.Ricetta Then
                                Parametri_Aggiuntivi.Item("Stato") = AgronicaCoreModelsSTD.attivita.Attivita.Stati.Da_Eseguire
                            Else
                                Parametri_Aggiuntivi.Item("Stato") = AgronicaCoreModelsSTD.attivita.Attivita.Stati.Eseguita
                            End If

                    End Select


                    MenuBS_2017_RedirectGestione.RedirectGenerico(objParametriAgenda.Piva, Enum_SiteRedirector.GiasNG,
                                                                      enum_PagineGiasNG.Pagina_Edit_Attivita, PaginaLink,
                                                                      objParametri_Server, Parametri_Aggiuntivi)

                    objParametriAgenda.SitoDestinazione = Enum_SiteRedirector.GiasNG
                End If

            Case Else

                PaginaLink = ""

        End Select

        Return PaginaLink

    End Function

    Public Function SeNuovaGestioneCaricoScarico(ByVal Lav_Cod As Integer,
                                                 ByRef objParametriAgenda As ParametriAgenda,
                                                 ByRef objParametriServer As AgronicaCoreParametri,
                                                 Optional ByVal DocumentoRicevutoLight As Boolean = False
                                                 ) As Boolean

        Dim gestioneCaricoScaricoDocContab As Boolean = False

        If (Not DocumentoRicevutoLight) AndAlso (Lav_Cod = LAVCOD_SCARICO OrElse Lav_Cod = LAVCOD_CARICO) Then

            Dim permessi As New PermessiUtente

            Dim permessoNuoviBottoniMagazzino = permessi.getPermesso(enum_Security_Attivita.Contabilita_CarichiScarichi_Magazzino).Scrittura

            Dim setupDoc2021 As Boolean = False
            If permessoNuoviBottoniMagazzino Then
                setupDoc2021 = SeSetupDoc2021(objParametriServer)
            End If

            If permessoNuoviBottoniMagazzino AndAlso setupDoc2021 Then

                Dim idAgenda = CInt(objParametriAgenda.Id_Agenda)

                If idAgenda = 0 Then

                    '--- INSERIMENTO
                    gestioneCaricoScaricoDocContab = True

                Else

                    '--- MODIFICA
                    '    In objParametriAgenda il Sa_Cod potrebbe venire passato a 0 anche se non lo è,
                    '    per cui lo devo leggere.

                    Dim saCod = LeggiSaCodAgenda(objParametriAgenda.Piva,
                                                idAgenda,
                                                objParametriServer)


                    '------------------------------------------------------------------------------------------
                    'Per poter entrare nella nuova gestione documenti anche se il movimento è nato con la vecchia
                    'If saCod = 0 Then
                    gestioneCaricoScaricoDocContab = True
                    'End If
                    '------------------------------------------------------------------------------------------

                End If

            End If

        End If

        Return gestioneCaricoScaricoDocContab

    End Function

    Public Function SeSetupDoc2021(objParametriServer As AgronicaCoreParametri) As Boolean

        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim setupDoc2021 As String = objConfigSiti.Recupera_Valore_ByChiave(0,
                                                                            "DocContabile2021DaAgenda",
                                                                            objParametriServer)

        If setupDoc2021 = "1" Then
            Return True
        Else
            Return False
        End If

    End Function

    Private Function LeggiSaCodAgenda(ByVal piva As String,
                                      ByVal idAgenda As Integer,
                                      ByRef objParametriServer As AgronicaCoreParametri) As Integer

        Dim saCod As Integer

        Dim dtAgenda As DataTable
        Dim objAgenda As New AgronicaCoreContabDAL.Agenda_R

        dtAgenda = objAgenda.Leggi(piva,
                                   0,
                                   idAgenda,
                                   0,
                                   enumSelezioneVariabile.Selezione_TabellaCompleta,
                                   "",
                                   "",
                                   objParametriServer)

        If dtAgenda.Rows.Count > 0 Then

            saCod = dtAgenda.Rows(0).Item("SA_COD")

        End If

        Return saCod

    End Function

    Private Function PaginaLink_CaricoScaricoDocContab(ByVal Lav_Cod As Integer,
                                                       ByRef objParametriAgenda As ParametriAgenda,
                                                       ByVal PaginaSitoAgendaOrigine As enum_PagineAgenda_2010,
                                                       Optional ByVal ServizioCod As Integer = -1
                                                       ) As String

        Dim PaginaLink As String = ""

        PaginaLink = VirtualPathUtility.ToAbsolute("~/GestioneContabilita/DocContabile.aspx") &
                     "?p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder) &
                     "&o=" & Stringa_Codifica(objParametriAgenda.Tipo_Operazione, AgroKey_EncoderDecoder) &
                     "&s=" & Stringa_Codifica(objParametriAgenda.Sa_Cod, AgroKey_EncoderDecoder) &
                     "&i=" & Stringa_Codifica(objParametriAgenda.Id_Agenda, AgroKey_EncoderDecoder) &
                     "&l=" & Stringa_Codifica(Lav_Cod, AgroKey_EncoderDecoder) &
                     "&md=" & Stringa_Codifica(0, AgroKey_EncoderDecoder) &
                     "&orig=" & Stringa_Codifica(PaginaSitoAgendaOrigine, AgroKey_EncoderDecoder) &
                     "&ricercatype=" & Stringa_Codifica("", AgroKey_EncoderDecoder) &
                     "&ricercadoc=" & Stringa_Codifica("", AgroKey_EncoderDecoder) &
                     "&ifr=" & Stringa_Codifica(0, AgroKey_EncoderDecoder) &
                     "&sc=" & Stringa_Codifica(ServizioCod, AgroKey_EncoderDecoder)

        Return PaginaLink

    End Function

    Private Function PaginaLink_GestioneMagazzini(ByVal Lav_Cod As Integer,
                                                  ByRef objParametriAgenda As ParametriAgenda,
                                                  ByVal PaginaSitoAgendaOrigine As enum_PagineAgenda_2010,
                                                  Optional ByVal ServizioCod As Integer = -1
                                                  ) As String

        Dim PaginaLink As String = "../GestioneMagazzini/FormProdotto.aspx"

        Dim dto = PaginaLinkGestioneMagazziniQueryStringDto(Lav_Cod, objParametriAgenda.Piva, objParametriAgenda.Sa_Cod)

        PaginaLink = PaginaLink &
                     "?k=" & Stringa_Codifica(dto.k, AgroKey_EncoderDecoder) &
                     "&c=" & Stringa_Codifica(dto.c, AgroKey_EncoderDecoder) &
                     "&o=" & Stringa_Codifica(objParametriAgenda.Tipo_Operazione, AgroKey_EncoderDecoder) &
                     "&orig=" & Stringa_Codifica(PaginaSitoAgendaOrigine, AgroKey_EncoderDecoder) &
                     "&mode=" & Stringa_Codifica(dto.mode, AgroKey_EncoderDecoder) &
                     "&l=" & Stringa_Codifica(Lav_Cod, AgroKey_EncoderDecoder) &
                     "&d=" & Stringa_Codifica(CStr(objParametriAgenda.Data), AgroKey_EncoderDecoder) &
                     "&s=" & Stringa_Codifica(objParametriAgenda.Sa_Cod, AgroKey_EncoderDecoder) &
                     "&a=" & Stringa_Codifica(objParametriAgenda.Id_Agenda, AgroKey_EncoderDecoder) &
                     "&sc=" & Stringa_Codifica(ServizioCod, AgroKey_EncoderDecoder)

        Return PaginaLink

    End Function

    Public Function PaginaLinkGestioneMagazziniQueryStringDto(ByVal Lav_Cod As Integer, piva As String, sa_cod As Integer) As PaginaLinkGestioneMagazziniQueryStringDto
        Dim xChiave As String = ""
        Dim mode As String = ""
        Dim caricoScarico As String = ""
        Call Albero.ChiaveAlbero_Codifica(xChiave, enum_TipoNodo.x_GiacenzeMagazzino, piva, sa_cod, , , , , , , , , , , , , , , , , , )

        If Lav_Cod = LAVCOD_TRASFERIMENTO Then
            caricoScarico = "T"
            mode = "trasferimento"
        End If

        If Lav_Cod = LAVCOD_SCARICO OrElse Lav_Cod = LAVCOD_CARICO Then
            If Lav_Cod = LAVCOD_CARICO Then
                caricoScarico = "C"
            ElseIf Lav_Cod = LAVCOD_SCARICO Then
                caricoScarico = "S"
            End If
            mode = "magazzino"
        End If

        If Lav_Cod = LAVCOD_VENDITA OrElse Lav_Cod = LAVCOD_ACQUISTO Then
            If Lav_Cod = LAVCOD_ACQUISTO Then
                caricoScarico = "C"
            ElseIf Lav_Cod = LAVCOD_VENDITA Then
                caricoScarico = "S"
            End If
            mode = "compravendita"
        End If

        Return New PaginaLinkGestioneMagazziniQueryStringDto With {
            .k = xChiave,
            .c = caricoScarico,
            .mode = mode
        }
    End Function


    Private Function PaginaLink_Raccolta(ByRef objParametriAgenda As ParametriAgenda,
                                         ByRef objParametriServer As AgronicaCoreParametri,
                                         ByVal PaginaSitoAgendaOrigine As enum_PagineAgenda_2010
                                         ) As String

        Dim PaginaLink As String = ""

        'gestione raccolta new

        Try
            If Not IsNothing(objParametriServer) Then

                'Dim cf As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                'Dim dt As DataTable = cf.Leggi(0, "RaccoltaNew", " valore = 'true' ", "", objParametriServer)
                'If dt.Rows.Count = 1 Then
                '    'raccolta new
                '    PaginaLink = "../Operazioni/Raccolta.aspx"
                '    Return PaginaLink
                'Else
                '    'raccolta 2003
                'End If

                '(09/08/2018 fede) in attesa di concludere la raccolta sulla trattamenti_2
                'sulla nuova si va solo se si proviene dal GIS

                If PaginaSitoAgendaOrigine = enum_PagineAgenda_2010.Gis Then

                    Dim cf As New AgronicaCoreVarieDAL.Configurazione_Siti_R

                    Dim dtConfigSiti As DataTable = cf.Leggi(0, "OperazioniAgendaBS", "", "", objParametriServer)

                    If Not IsNothing(dtConfigSiti) AndAlso dtConfigSiti.Rows.Count > 0 AndAlso LCase(dtConfigSiti.Rows(0).Item("Valore")) = "true" Then

                        'leggo le eventuali IMPOSTAZIONI UTENTE
                        Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                        Dim Tipo_Raccolta_Val As String = ObjUtenti.Impostazione_Valore_From_Impostazione_Cod_Utente_Poi_SuperUser(
                                                                    enum_Impostazioni_Utenti.UTENTE_COD_RACCOLTA_TIPO,
                                                                    HttpContext.Current.Session("ASG_objParametri_Utenti"))

                        Dim Tipo_Raccolta As enum_RACCOLTA_TIPO = If(IsNumeric(Tipo_Raccolta_Val), CInt(Tipo_Raccolta_Val), enum_RACCOLTA_TIPO.Fast)

                        Select Case Tipo_Raccolta
                            Case enum_RACCOLTA_TIPO.Fast, enum_RACCOLTA_TIPO.Leggera
                                PaginaLink = "../Operazioni/Trattamenti_2.aspx"
                            Case Else
                                PaginaLink = "../Operazioni/Raccolta.aspx"
                        End Select

                    Else

                        PaginaLink = "../Operazioni/Raccolta.aspx"

                    End If

                Else

                    Dim cf As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                    Dim dtConfigSiti As DataTable = cf.Leggi(0, "RaccoltaBS", "", "", objParametriServer)

                    If Not IsNothing(dtConfigSiti) AndAlso dtConfigSiti.Rows.Count > 0 AndAlso LCase(dtConfigSiti.Rows(0).Item("Valore")) = "true" Then

                        'leggo le eventuali IMPOSTAZIONI UTENTE
                        Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                        Dim Tipo_Raccolta_Val As String = ObjUtenti.Impostazione_Valore_From_Impostazione_Cod_Utente_Poi_SuperUser(
                                                                    enum_Impostazioni_Utenti.UTENTE_COD_RACCOLTA_TIPO,
                                                                    HttpContext.Current.Session("ASG_objParametri_Utenti"))

                        Dim Tipo_Raccolta As enum_RACCOLTA_TIPO = If(IsNumeric(Tipo_Raccolta_Val), CInt(Tipo_Raccolta_Val), enum_RACCOLTA_TIPO.Fast)

                        Select Case Tipo_Raccolta
                            Case enum_RACCOLTA_TIPO.Fast, enum_RACCOLTA_TIPO.Leggera
                                PaginaLink = "../Operazioni/Trattamenti_2.aspx"
                            Case Else
                                PaginaLink = "../Operazioni/Raccolta.aspx"
                        End Select

                    Else

                        PaginaLink = "../Operazioni/Raccolta.aspx"

                    End If

                End If

                Return PaginaLink

            End If

        Catch ex As Exception
            'raccolta 2003
        End Try

        If objParametriAgenda.Sa_Cod = "0" OrElse objParametriAgenda.Sa_Cod = "" Then
            Return ""
        End If

        Dim objGiasOnline As New ParametriGiasOnline With {
                .Cul_Cod = objParametriAgenda.Cul_Cod,
                .DataSelezionata = objParametriAgenda.Data,
                .Id_Agenda = objParametriAgenda.Id_Agenda,
                .Lavorazione = LAVCOD_RACCOLTA,
                .Operazione = objParametriAgenda.Tipo_Operazione,
                .PaginaRichiesta = enum_PagineGiasOnline.Agenda_Raccolta,
                .Piva = objParametriAgenda.Piva,
                .Sa_Cod = objParametriAgenda.Sa_Cod
                }

        Dim specie As Integer = 0
        If IsNumeric(objParametriAgenda.Veg_Cod.Split("/")(0)) AndAlso CInt(objParametriAgenda.Veg_Cod.Split("/")(0)) > 0 Then
            specie = CInt(objParametriAgenda.Veg_Cod.Split("/")(0))
        End If
        objGiasOnline.Veg_Cod = specie

        PaginaLink = RedirectGestione.IndirizzoCompleto_SitoOnline_PassandoDirettamente_ParametriGiasOnline(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objGiasOnline)

        Return PaginaLink

    End Function

    Function LinkPagina_from_LavCod(Operazione_Colturale_Generica As Agenda.Operazioni_Colturali.Operazione_Colturale.I_Operazione_Colturale,
                                    Optional ByVal LeggiFlagConfigurazioneSiti As Boolean = True) As String

        Dim PaginaLink As String = ""

        Select Case Operazione_Colturale_Generica.Lav_Cod

            '--- FERTILIZZAZIONE
            Case LAVCOD_FERTIRRIGAZIONE,
                    LAVCOD_CONCIMAZIONE_FOGLIARE,
                    LAVCOD_DISTRIBUZIONE_CONCIME,
                    LAVCOD_DISTRIBUZIONE_AMMENDANTI,
                    LAVCOD_SARCHIATURA_CONCIMAZIONE,
                    LAVCOD_TRATTAMENTO_ANTIBUTTERATURA

                PaginaLink = "../Operazioni/Trattamenti_2.aspx"

            '--- TRATTAMENTI
            Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO,
                    LAVCOD_DISERBO,
                    LAVCOD_DISSECCAMENTO,
                    LAVCOD_GEODISINFESTAZIONE,
                    LAVCOD_CONCIA_SEME,
                    LAVCOD_TRATTAMENTO_FITOREGOLATORE

                PaginaLink = "../Operazioni/Trattamenti_2.aspx"

            '--- DISTRIBUZIONE INSETTI
            Case LAVCOD_DISTRIBUZIONE_INSETTI
                PaginaLink = "../Operazioni/Distribuzione_Insetti.aspx"

            '--- LAVORAZIONI
            Case LAVCOD_ANDANAMENTO, LAVCOD_ARATURA, LAVCOD_DEFOGLIAZIONE, LAVCOD_ASPORTAZIONE_ORGANI_INFETTI, LAVCOD_ASSOLCATURA,
                 LAVCOD_CARICO_MANUALE_FRUTTA, LAVCOD_CIMATURA, LAVCOD_DIRADAMENTO_MANUALE, LAVCOD_DISSODAMENTO,
                 LAVCOD_ERPICATURA, LAVCOD_ESTIRPATURA, LAVCOD_ESPIANTO, LAVCOD_FALCIACONDIZIONATURA,
                 LAVCOD_FALCIATURA_ERBAI, LAVCOD_FORMAZIONE_ARGINELLI, LAVCOD_FRANGIZOLLATURA, LAVCOD_FRESATURA,
                 LAVCOD_IMBALLO_FIENO_ROTOLI, LAVCOD_INTERRAMENTO_PAGLIE, LAVCOD_LAVORAZIONE_TRA_FILA, LAVCOD_LAVORAZIONE_SU_FILA,
                 LAVCOD_LEGATURA, LAVCOD_LIVELLAMENTO, LAVCOD_MANUTENZIONE_ARGINI, LAVCOD_MESSA_DIMORA_PIANTE,
                 LAVCOD_MIETITREBBIATURA, LAVCOD_MINIMUM_TILLAGE, LAVCOD_PACCIAMATURA, LAVCOD_POTATURA_SECCA,
                 LAVCOD_POTATURA_VERDE, LAVCOD_PRESSATURA, LAVCOD_RACCOLTA_LEGNA_POTATURA, LAVCOD_RACCOLTA_MANUALE,
                 LAVCOD_RACCOLTA_MECCANICA, LAVCOD_RANGHINATURA, LAVCOD_RINCALZATURA, LAVCOD_RIPPATURA,
                 LAVCOD_RIPUNTATURA, LAVCOD_RIVOLTAMENTO_FORAGGIO, LAVCOD_RULLATURA, LAVCOD_SARCHIATURA,
                 LAVCOD_SCARIFICATURA, LAVCOD_SCASSO, LAVCOD_TRINCIATURA, LAVCOD_VANGATURA,
                 LAVCOD_ZAPPATURA, LAVCOD_GEBIATURA, LAVCOD_ROMPICROSTA, LAVCOD_LAVORAZIONE_CONBINATA,
                 LAVCOD_ERPICATURA_ROTANTE, LAVCOD_INTERVENTO_ANTIBRINA, LAVCOD_STRIGLIATURA, LAVCOD_PIRODISERBO
                'Andanamento, Aratura, Asportazione Organi Infetti, Assolcatura ,Carico Manuale Frutta, Cimatura
                'Diradamento Manuale, Dissodamento, Erpicatura, Erstirpatura, Espianto, Falciacondizionatura, Falciatura erbai
                'Frangizollatura, Fresatura, Imballo fieno e rotoli, Interramento paglie, Lavorazione tra fila
                'Lavorazione su fila, Legatura, Livellamento, Manutenzione argini, Messa dimora piante, Mietitrebbiatura
                'Minimum tillage, Pacciamatura , Potatura secca, Potatura verde, Pressatura, Raccolta legna potatura
                'Raccolta manuale, Raccolta meccanica, Ranghinatura, Rincalzatura, Rippatura, Ripuntatura
                'Rivoltaggio foraggio, Rullatura, Sarchiatura, Scarificatura, Scasso, Sod sedding
                'Trinciatura, Vangatura, Zappatura, Gebiatura, Rompicrosta, Lavorazione Combinata,Erpicatura Rotante, Strigliatura, pirodiserbo

                PaginaLink = "../Operazioni/Trattamenti_2.aspx"

            '--- INSTALLAZIONE TRAPPOLE
            Case LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA,
                    LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE
                PaginaLink = "../Operazioni/Installazione_Trappole.aspx"

            '--- REINNESCO E RILIEVO TRAPPOLE
            Case LAVCOD_REINNESCO_TRAPPOLE, LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE
                PaginaLink = "../Operazioni/Reinnesco_Rilievi_Trappole.aspx" '"../Classi/Agro_Pages/Agenda_Pages/Operazioni_Pages/Reinnesco_Trappole/Reinnesco_Trappole.aspx"

            '--- IRRIGAZIONE
            Case LAVCOD_IRRIGAZIONE
                PaginaLink = "../Operazioni/Irrigazione.aspx"

            '--- SEMINA E TRAPIANTO
            Case LAVCOD_SEMINA, LAVCOD_TRAPIANTO, LAVCOD_SOVESCIO, LAVCOD_SOD_SEDDING
                PaginaLink = "../Operazioni/Semina_E_Trapianto_1.aspx"

                ' Operazione Deperecata
                '--- CURA
                'Case LAVCOD_CURA
                '    PaginaLink = "../GestioneMagazzini/OperazioneDiCura.aspx"

            Case Else
                PaginaLink = ""

        End Select

        Return PaginaLink
    End Function

    Public Shared Function Link_GiasOnline_STR(ByVal PaginaRichiesta As enum_PagineGiasOnline,
                                                   ByRef objParametriAgenda As ParametriAgenda)

        Dim objGiasOnline As New ParametriGiasOnline
        Dim xChiave As String = ""
        Call Albero.ChiaveAlbero_Codifica(xChiave,
                                              enum_TipoNodo.p_PortafoglioProdotti,
                                              objParametriAgenda.Piva,
                                              objParametriAgenda.Sa_Cod, , , , , , , , , , , ,)

        objGiasOnline.xChiave = xChiave
        objGiasOnline.Operazione = enum_TipoOperazioneDB.Scrittura
        objGiasOnline.DataSelezionata = objParametriAgenda.Data
        objGiasOnline.Piva = objParametriAgenda.Piva

        objGiasOnline.PaginaRichiesta = PaginaRichiesta
        objGiasOnline.Xml_Generico.Length = 0

        objGiasOnline.RisorsaEditQueryString = objParametriAgenda.StrGenericaXlinkGiasOnline

        If Not IsNothing(ConfigurationManager.AppSettings("LinkAgronicaAgenda2010")) AndAlso ConfigurationManager.AppSettings("LinkAgronicaAgenda2010") <> "" Then
            objGiasOnline.LinkAgronicaAgenda2010 = ConfigurationManager.AppSettings("LinkAgronicaAgenda2010")
        Else
            objGiasOnline.LinkAgronicaAgenda2010 = ""
        End If

        Dim link As String = ""
        If objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010 Then
            link = RedirectGestione.IndirizzoCompleto_SitoOnline_PassandoDirettamente_ParametriGiasOnline(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objGiasOnline)
        Else
            link = RedirectGestione.IndirizzoCompleto_SitoOnline_PassandoDirettamente_ParametriGiasOnline(Enum_SiteRedirector.Sito_GiasOnline_2010, objGiasOnline)
        End If

        Return link

    End Function

    Public Shared Sub Link_GiasOnline(ByVal PaginaRichiesta As enum_PagineGiasOnline,
                                          ByRef objParametriAgenda As ParametriAgenda,
                                          ByRef page As System.Web.UI.Page)

        Dim objGiasOnline As New ParametriGiasOnline
        Dim xChiave As String = ""
        Call Albero.ChiaveAlbero_Codifica(xChiave,
                                              enum_TipoNodo.p_PortafoglioProdotti,
                                              objParametriAgenda.Piva,
                                              objParametriAgenda.Sa_Cod, , , , , , , , , , , ,)

        objGiasOnline.xChiave = xChiave
        objGiasOnline.Operazione = enum_TipoOperazioneDB.Scrittura
        objGiasOnline.DataSelezionata = objParametriAgenda.Data
        objGiasOnline.Piva = objParametriAgenda.Piva

        objGiasOnline.PaginaRichiesta = PaginaRichiesta
        objGiasOnline.Xml_Generico.Length = 0

        If Not IsNothing(ConfigurationManager.AppSettings("LinkAgronicaAgenda2010")) AndAlso ConfigurationManager.AppSettings("LinkAgronicaAgenda2010") <> "" Then
            objGiasOnline.LinkAgronicaAgenda2010 = ConfigurationManager.AppSettings("LinkAgronicaAgenda2010")
        Else
            objGiasOnline.LinkAgronicaAgenda2010 = ""
        End If

        Dim link As String = RedirectGestione.IndirizzoCompleto_SitoOnline_PassandoDirettamente_ParametriGiasOnline(Enum_SiteRedirector.Sito_GiasOnline_2010,
                                                                                           objGiasOnline)
        page.Response.Redirect(link)

    End Sub


    Public Shared Function PermessiOpContabiliEMagazzino(ByVal Lav_Cod As String,
                                                         ByVal Operazione As String,
                                                         ByRef objParametri_Server As AgronicaCoreParametri,
                                                         ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                         ByRef session As System.Web.SessionState.HttpSessionState
                                                         ) As Boolean

        Dim olav As New AgronicaCoreMetaSchemaDAL.Operazioni_R
        Dim gruOp As Integer = olav.Gru_Op_from_LavorazioneCod(Lav_Cod, objParametri_Server)

        'Estraggio il permesso da controllare
        Dim attivita As enum_Security_Attivita
        Select Case gruOp
            Case 6
                attivita = enum_Security_Attivita.Gest_Contabilita
            Case 10
                attivita = enum_Security_Attivita.Gest_Magazzino
            Case 20

                Select Case Lav_Cod
                    Case LAVCOD_GESTIONE_RIFIUTI
                        attivita = enum_Security_Attivita.Gestione_Rifiuti
                    Case LAVCOD_MONITORAGGIO_TEMPI_RIENTRO, LAVCOD_VISITA_GENERICA, LAVCOD_VISITA
                        attivita = enum_Security_Attivita.Gest_CartellaAziendale_VisiteIspettive
                    Case LAVCOD_MONITORAGGIO_CE
                        attivita = enum_Security_Attivita.Gest_CartellaAziendale_MonitoraggioCE
                    Case LAVCOD_PRATICA_ECOLOGICA
                        attivita = enum_Security_Attivita.CheckList_Pratiche_Ecologiche_APOT
                    Case LAVCOD_FORMAZIONE
                        attivita = enum_Security_Attivita.CheckList_Formazione
                    Case Else
                        attivita = enum_Security_Attivita.Gest_CartellaAziendale_VisiteIspettive
                End Select

            Case Else
                Return True
        End Select

        'Estraggo il tipo di accesso: lettura/modifica
        Dim tipoOperazione As enum_Security_Operazione
        If IsNumeric(Operazione) Then
            Select Case Operazione
                Case enum_TipoOperazioneDB.Lettura, enum_TipoOperazioneDB.Modifica
                    tipoOperazione = CInt(Operazione)
                Case enum_TipoOperazioneDB.Scrittura, enum_TipoOperazioneDB.Cancellazione, enum_TipoOperazioneDB.Trasferimento, enum_TipoOperazioneDB.Copia
                    tipoOperazione = enum_TipoOperazioneDB.Modifica
                Case Else
                    Return False
            End Select
        Else
            Return False
        End If

        'Controllo il permesso
        Dim acUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim utenteAbilitato As Boolean = acUtenti.Controlla_Permessi_Utente(session("ASG_Utente_Username"), session("ASG_IdServizio"), attivita, tipoOperazione, Now, "", objParametri_Utenti)

        Return utenteAbilitato

    End Function


#End Region

    Public Function getLinkPaginaforQdC_Angular(ByVal LeggiLink As AgronicaCoreDTOStd.InData.Agenda.LeggiLink_Operazione,
                                                ByVal objParametri_Server As AgronicaCoreParametri,
                                                ByVal objParametri_Utenti As AgronicaCoreParametri) As RispostaStandard

        Dim leggiLingua As New Lingue_Read
        Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod,
                                                                  AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                  "", "", objParametri_Server)

        Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
        Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo(linguaCodiceISO)


        Dim Lav_Cod As Integer = 0

        Dim objParametriAgenda As New ParametriAgenda

        If Not String.IsNullOrEmpty(LeggiLink.id_agenda) Then
            objParametriAgenda.Id_Agenda = LeggiLink.id_agenda
        Else
            objParametriAgenda.Id_Agenda = "0"
        End If

        If Not IsNothing(LeggiLink.impresa) Then
            objParametriAgenda.Piva = LeggiLink.impresa.partitaIva

            objParametriAgenda.RagSoc = LeggiLink.impresa.ragioneSociale
        Else
            objParametriAgenda.Piva = ""

            objParametriAgenda.RagSoc = ""
        End If

        If Not IsNothing(LeggiLink.lavorazione) Then
            Lav_Cod = LeggiLink.lavorazione.primaryKey.codice
        End If

        objParametriAgenda.Tipo_Operazione = LeggiLink.tipo_operazione

        If Not IsNothing(LeggiLink.data) Then
            objParametriAgenda.Data = LeggiLink.data
        Else
            objParametriAgenda.Data = DateTime.Now
        End If

        If Not IsNothing(LeggiLink.centroaziendale) Then
            objParametriAgenda.Sa_Cod = LeggiLink.centroaziendale.primaryKey.codice

            objParametriAgenda.SaNome = LeggiLink.centroaziendale.nome
        Else
            objParametriAgenda.Sa_Cod = "0"

            objParametriAgenda.SaNome = ""
        End If

        objParametriAgenda.Veg_Cod = "0"

        objParametriAgenda.Cul_Cod = "0"

        Dim r As New RispostaStandard

        r.RispostaOK = True

        Dim strErrore As String = ""

        Dim PaginaLink = LinkPagina_from_LavCod_NEW(Lav_Cod, objParametriAgenda, strErrore:=strErrore)

        If PaginaLink = "" Then

            r.RispostaOK = False
            r.RispostaStringa = ""
            r.Errore = AgronicaCoreDataProvider.My.Resources.Gias.Errore
            r.ErroriGias.Add(New ErroreGias With {
                                .severity = ErroreGias_Severity.Bloccante,
                                .tipo = ErroreGias_Tipo.Generico,
                                .messaggio = If(strErrore <> "", strErrore, Gias.OperazioneinManutenzione)
                            })

        Else
            r.RispostaStringa = PaginaLink
        End If

        Return r

    End Function

    Private Shared Function controlloPermessi(tipoOp As enum_Security_Attivita,
                                              type As enum_Security_Operazione) As Boolean

        Dim objParametri_Utenti = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim permesso As Boolean = objPermessi.Controlla_Permessi_Utente(
                           HttpContext.Current.Session("ASG_Utente_Username"),
                           HttpContext.Current.Session("ASG_IdServizio"),
                           tipoOp,
                           type,
                           Date.Now,
                           "",
                           objParametri_Utenti)

        Return permesso
    End Function

    Public Function Gestisci_Redirect_Cambio_Operazione(ByRef objParametriAgenda As ParametriAgenda,
                                                        ByVal objParametri_Server As AgronicaCoreParametri,
                                                        Optional fromBootstrapToBootstrap As Boolean = False) As String

        Dim PaginaLink As String = ""

        Select Case objParametriAgenda.SitoOrigine
            Case Enum_SiteRedirector.GiasNG
                'Se la pagina di origine è il Menu Agenda NG allora forzo che al cambio di operazione vada nella pagina del QdC
                If objParametriAgenda.PaginaSitoOrigine = enum_PagineGiasNG.Pagina_Menu_Agenda Then
                    objParametriAgenda.PaginaSitoOrigine = enum_PagineGiasNG.Pagina_Edit_Attivita
                End If

                PaginaLink = LinkPagina_from_LavCod_NEW(objParametriAgenda.Lav_Cod, objParametriAgenda)

                If PaginaLink = LinkQdCAngular Then

                    Dim Parametri_Aggiuntivi As New JObject

                    Parametri_Aggiuntivi("Lav_Cod") = objParametriAgenda.Lav_Cod.ToString()

                    Parametri_Aggiuntivi("Lav_Des") = objParametriAgenda.Lav_Des

                    Parametri_Aggiuntivi("TipoOperazioneDB") = objParametriAgenda.Tipo_Operazione.ToString()

                    AgronicaCoreGestioneRichieste.MenuBS_2017_RedirectGestione.RedirectGenerico(objParametriAgenda.Piva,
                                                                                                    Enum_SiteRedirector.GiasNG,
                                                                                                    objParametriAgenda.PaginaSitoOrigine,
                                                                                                    PaginaLink,
                                                                                                    objParametri_Server,
                                                                                                    Parametri_Aggiuntivi)
                Else

                    objParametriAgenda.PaginaSitoOrigine = enum_PagineGiasNG.Pagina_Menu_Agenda

                    PaginaLink = LinkPagina_from_LavCod_NEW(objParametriAgenda.Lav_Cod, objParametriAgenda)
                End If


            Case Else
                PaginaLink = LinkPagina_from_LavCod_NEW(objParametriAgenda.Lav_Cod, objParametriAgenda,
                                                        fromBootstrapToBootstrap:=fromBootstrapToBootstrap)
        End Select

        Return PaginaLink

    End Function







    Public Function LinkPagina_from_LavCod_NEW(ByVal Lav_Cod As Integer,
                                               ByRef objParametriAgenda As ParametriAgenda,
                                               Optional ByVal PaginaSitoAgendaOrigine As enum_PagineAgenda_2010 = enum_PagineAgenda_2010.Menu,
                                               Optional ByVal LeggiFlagConfigurazioneSiti As Boolean = True,
                                               Optional ByVal DocumentoRicevutoLight As Boolean = False,
                                               Optional ByVal ServizioCod As Integer = 0,
                                               Optional ByVal Parametri_Aggiuntivi_for_Redirect As JObject = Nothing,
                                               Optional ByRef strErrore As String = "",
                                               Optional fromBootstrapToBootstrap As Boolean = False,
                                               Optional redirectPortateDomandaIrrigua As Boolean = False
                                               ) As String

        objParametriAgenda.SitoDestinazione = Enum_SiteRedirector.Sito_AgronicaAgenda_2010

        Dim PaginaLink As String = ""

        Dim objParametri_Server As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Dim objParametri_Utenti As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim UtenteAbilitatoBS As Boolean

        Dim Tipo_Operazione As enum_TipoOperazioneDB
        Select Case objParametriAgenda.Tipo_Operazione
            Case 1, 2
                Tipo_Operazione = enum_TipoOperazioneDB.Modifica
            Case 0
                Tipo_Operazione = enum_TipoOperazioneDB.Lettura
        End Select

        Dim usoAgendaNG, usoAgendaBS,
            redirectFromBS As Boolean

        Dim Id_Agenda As Integer = 0
        Dim Sa_Cod As Integer = 0

        If Not String.IsNullOrEmpty(objParametriAgenda.Id_Agenda) Then
            Id_Agenda = CInt(objParametriAgenda.Id_Agenda)
        End If
        If Not String.IsNullOrEmpty(objParametriAgenda.Sa_Cod) Then
            Sa_Cod = CInt(objParametriAgenda.Sa_Cod)
        End If

        Dim Ricetta_Cod As Integer = 0
        Dim Ricetta_Operazione_Cod As Integer = 0

        If Not String.IsNullOrEmpty(MenuBS_2017_RedirectGestione.getValueParametriAggiuntivi(Parametri_Aggiuntivi_for_Redirect, "Ricetta_Cod")) Then
            Ricetta_Cod = MenuBS_2017_RedirectGestione.getValueParametriAggiuntivi(Parametri_Aggiuntivi_for_Redirect, "Ricetta_Cod")
        End If
        If Not String.IsNullOrEmpty(MenuBS_2017_RedirectGestione.getValueParametriAggiuntivi(Parametri_Aggiuntivi_for_Redirect, "Ricetta_Operazione_Cod")) Then
            Ricetta_Operazione_Cod = MenuBS_2017_RedirectGestione.getValueParametriAggiuntivi(Parametri_Aggiuntivi_for_Redirect, "Ricetta_Operazione_Cod")
        End If

        If PaginaSitoAgendaOrigine = enum_PagineAgenda_2010.Menu Then
            Dim dtConfigSiti As DataTable = objConfigSiti.Leggi(0, "MenuAgendaBS", "", "", objParametri_Server)
            If Not IsNothing(dtConfigSiti) AndAlso dtConfigSiti.Rows.Count > 0 AndAlso LCase(dtConfigSiti.Rows(0).Item("Valore")) = "true" Then
                PaginaSitoAgendaOrigine = enum_PagineAgenda_2010.Menu_BS
            End If
        End If

        'Se arriviamo da menu vecchio, ma stiamo cercando di aprire operazioni gestite solo su NG, devo fare dei controlli
        Dim checkOperazioniNG As Boolean = False

        If LeggiFlagConfigurazioneSiti Then

            If fromBootstrapToBootstrap Then
                'Se vengo dal menu vecchio, ma sto cercando di aprire una di queste operazioni controllo i permessi NG
                If (objParametriAgenda.Raccoglitore_Cod <> "0" AndAlso Not get_isRilievo(Lav_Cod)) OrElse
                        Lav_Cod = LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE OrElse
                        Lav_Cod = LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA OrElse
                        Lav_Cod = LAVCOD_PASCOLAMENTO_PROPRIO OrElse
                        Lav_Cod = LAVCOD_PASCOLAMENTO_TERZI OrElse
                        Lav_Cod = LAVCOD_RACCOLTA OrElse
                        Lav_Cod = LAVCOD_CONCIA_SEME OrElse
                        Lav_Cod = LAVCOD_REINNESCO_TRAPPOLE OrElse
                        Lav_Cod = LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE OrElse
                        ((Lav_Cod = LAVCOD_DISTRIBUZIONE_INSETTI OrElse Lav_Cod = LAVCOD_TRATTAMENTO_POST_RACCOLTA) AndAlso (objParametriAgenda.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.Ricetta OrElse objParametriAgenda.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.RicettaBrogliaccio)) Then
                    fromBootstrapToBootstrap = False
                End If
                checkOperazioniNG = True
            End If

            'Se vengo dal menu vecchio devo restare sul vecchio, salto tutta la parte NG
            If Not fromBootstrapToBootstrap Then
                usoAgendaNG = Utente_Abilitato_OperazioniAgendaNG(Tipo_Operazione, objParametri_Server, objParametri_Utenti)

                'Se provengo dal sito agenda e posso fare il redirect, devo fare un redirect diverso
                redirectFromBS = If(usoAgendaNG AndAlso (objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010 OrElse objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_GiasOnline OrElse objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_GiasOnline_2010), True, False)
            End If

            'B) SE NON HO I PERMESSI PER UTILIZZARE ANGULAR OPPURE VENGO DAL MENU_AGENDA_BS LEGGO ABILITAZIONE AD AGENDA BS
            If Not (usoAgendaNG) Then
                Dim dtConfigSitiBS As DataTable = objConfigSiti.Leggi(0, "OperazioniAgendaBS", "", "", objParametri_Server)
                UtenteAbilitatoBS = objPermessi.Controlla_Permessi_Utente(
                                            objParametri_Utenti.UtenteUsername,
                                            enum_Id_Servizio.GiasOnline,
                                            enum_Security_Attivita.Agenda_AccessoMenu,
                                            Tipo_Operazione,
                                            Date.Now,
                                            "",
                                            objParametri_Utenti)
                usoAgendaBS = If(Not IsNothing(dtConfigSitiBS) AndAlso dtConfigSitiBS.Rows.Count > 0 AndAlso LCase(dtConfigSitiBS.Rows(0).Item("Valore")) = "true" AndAlso UtenteAbilitatoBS, True, False)
            End If
        End If

        'C) SE SIA PT. A CHE PT.B SONO FALSI, VADO ALLE PAGINE VECCHIE

        'Se non c'è l'abilitazione ad angular, ma stiamo cercando di aprire un multioperazione (esclusi i rilievi), blocco
        If Not usoAgendaNG AndAlso objParametriAgenda.Raccoglitore_Cod <> "0" AndAlso Not get_isRilievo(Lav_Cod) Then
            strErrore = Gias.OperazioneRegistrataNGNoPermessi
            Exit Function
        End If

        Select Case Lav_Cod

            'Il caso di Lav_Cod 0 è possibile solo se sto cercando di fare una nuova Operazione dal QdC nuovo 
            Case 0
                If usoAgendaNG Then
                    PaginaLink = CostantiPersonalizzate.LinkQdCAngular
                Else
                    PaginaLink = ""
                    strErrore = Gias.OperazioneRegistrataNGNoPermessi
                End If

#Region "DOC CONTABILE"
            '--- GESTIONE COSTI
            Case LAVCOD_COSTI_CDG

                PaginaLink = "../AnalisiCostiProduzione/GestioneCosti.aspx"

                PaginaLink &= "?p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder) &
                      "&id_agenda=" & Stringa_Codifica(Id_Agenda.ToString(), AgroKey_EncoderDecoder) &
                      "&origine=" & Stringa_Codifica("../GestioneMagazzini/GestioneMagazziniBS.aspx", AgroKey_EncoderDecoder) &
                      "&entrata_diretta=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Nothing) &
                      "&op=" & Stringa_Codifica(objParametriAgenda.Tipo_Operazione, AgroKey_EncoderDecoder, Nothing)


            '--- LIQUIDAZIONE SOCI
            Case LAVCOD_PROCEDURA_LIQUIDAZIONE_SOCI

                PaginaLink = "../GestioneContabilita/Liquidazione/Liquidazione.aspx"


            '--- GESTIONE MAGAZZINI
            Case LAVCOD_SCARICO, LAVCOD_CARICO, LAVCOD_VENDITA, LAVCOD_ACQUISTO, LAVCOD_TRASFERIMENTO

                Dim gestioneCaricoScaricoDocContab = SeNuovaGestioneCaricoScarico(Lav_Cod,
                                                                                  objParametriAgenda,
                                                                                  objParametri_Server,
                                                                                  DocumentoRicevutoLight:=DocumentoRicevutoLight)

                If gestioneCaricoScaricoDocContab Then
                    PaginaLink = PaginaLink_CaricoScaricoDocContab(Lav_Cod, objParametriAgenda, PaginaSitoAgendaOrigine, ServizioCod)
                Else
                    PaginaLink = PaginaLink_GestioneMagazzini(Lav_Cod, objParametriAgenda, PaginaSitoAgendaOrigine, ServizioCod)
                End If


            '--- DOCUMENTI CONTABILI
            Case LAVCOD_FATTURA_EMESSA, LAVCOD_FATTURA_RICEVUTA, LAVCOD_BOLLA_RICEVUTA, LAVCOD_BOLLA_EMESSA

                If SeSetupDoc2021(objParametri_Server) Then

                    PaginaLink = VirtualPathUtility.ToAbsolute("~/GestioneContabilita/DocContabile.aspx") &
                        "?p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder) &
                        "&o=" & Stringa_Codifica(objParametriAgenda.Tipo_Operazione, AgroKey_EncoderDecoder) &
                        "&s=" & Stringa_Codifica(Sa_Cod.ToString(), AgroKey_EncoderDecoder) &
                        "&i=" & Stringa_Codifica(Id_Agenda.ToString(), AgroKey_EncoderDecoder) &
                        "&l=" & Stringa_Codifica(Lav_Cod, AgroKey_EncoderDecoder) &
                        "&md=" & Stringa_Codifica(0, AgroKey_EncoderDecoder) &
                        "&orig=" & Stringa_Codifica(PaginaSitoAgendaOrigine, AgroKey_EncoderDecoder) &
                        "&ricercatype=" & Stringa_Codifica("", AgroKey_EncoderDecoder) &
                        "&ricercadoc=" & Stringa_Codifica("", AgroKey_EncoderDecoder) &
                        "&ifr=" & Stringa_Codifica(0, AgroKey_EncoderDecoder) &
                        "&sc=" & Stringa_Codifica(ServizioCod, AgroKey_EncoderDecoder)

                Else

                    Dim tipo As Integer = 0
                    'objParametriAgenda.Sa_Cod=0 per tutti i doc contabili
                    PaginaLink = VirtualPathUtility.ToAbsolute("~/GestioneContabilita/DocumentoContabileGenerico.aspx") &
                        "?p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder) &
                        "&o=" & Stringa_Codifica(objParametriAgenda.Tipo_Operazione, AgroKey_EncoderDecoder) &
                        "&s=" & Stringa_Codifica(0, AgroKey_EncoderDecoder) &
                        "&i=" & Stringa_Codifica(Id_Agenda.ToString(), AgroKey_EncoderDecoder) &
                        "&l=" & Stringa_Codifica(Lav_Cod, AgroKey_EncoderDecoder) &
                        "&d=" & Stringa_Codifica(CStr(objParametriAgenda.Data), AgroKey_EncoderDecoder) &
                        "&tf=" & Stringa_Codifica(tipo, AgroKey_EncoderDecoder) &
                        "&rs=" & Stringa_Codifica(objParametriAgenda.RagSoc, AgroKey_EncoderDecoder) &
                        "&orig=" & Stringa_Codifica(PaginaSitoAgendaOrigine, AgroKey_EncoderDecoder) &
                        "&sc=" & Stringa_Codifica(ServizioCod, AgroKey_EncoderDecoder)

                End If


            '--- NOTE ACCREDITO
            Case LAVCOD_NOTA_ACCREDITO_RICEVUTA, LAVCOD_NOTA_ACCREDITO_EMESSA

                Dim tipo As Integer = 0
                'objParametriAgenda.Sa_Cod=0 per tutti i doc contabili
                PaginaLink = VirtualPathUtility.ToAbsolute("~/GestioneContabilita/DocumentoContabileGenerico.aspx") &
                    "?p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder) &
                    "&o=" & Stringa_Codifica(objParametriAgenda.Tipo_Operazione, AgroKey_EncoderDecoder) &
                    "&s=" & Stringa_Codifica(0, AgroKey_EncoderDecoder) &
                    "&i=" & Stringa_Codifica(Id_Agenda.ToString(), AgroKey_EncoderDecoder) &
                    "&l=" & Stringa_Codifica(Lav_Cod, AgroKey_EncoderDecoder) &
                    "&d=" & Stringa_Codifica(CStr(objParametriAgenda.Data), AgroKey_EncoderDecoder) &
                    "&tf=" & Stringa_Codifica(tipo, AgroKey_EncoderDecoder) &
                    "&rs=" & Stringa_Codifica(objParametriAgenda.RagSoc, AgroKey_EncoderDecoder) &
                    "&orig=" & Stringa_Codifica(PaginaSitoAgendaOrigine, AgroKey_EncoderDecoder) &
                    "&sc=" & Stringa_Codifica(ServizioCod, AgroKey_EncoderDecoder)
#End Region

            '--- FERTILIZZAZIONE
            Case LAVCOD_FERTIRRIGAZIONE,
                 LAVCOD_CONCIMAZIONE_FOGLIARE,
                 LAVCOD_DISTRIBUZIONE_CONCIME,
                 LAVCOD_SARCHIATURA_CONCIMAZIONE,
                 LAVCOD_TRATTAMENTO_ANTIBUTTERATURA

                If redirectFromBS Then
                    LinkRedirectFromBS(objParametriAgenda, Parametri_Aggiuntivi_for_Redirect, PaginaLink, objParametri_Server)
                    Exit Select
                End If

                If usoAgendaNG Then
                    PaginaLink = CostantiPersonalizzate.LinkQdCAngular
                Else
                    PaginaLink = "../Operazioni/Trattamenti_2.aspx"
                End If


                 '--- FERTILIZZAZIONE
            Case LAVCOD_DISTRIBUZIONE_AMMENDANTI

                'Se provengo dall'agenda verso angular, leggo un link più complesso
                If redirectFromBS AndAlso Id_Agenda = 0 AndAlso Ricetta_Operazione_Cod = 0 Then
                    LinkRedirectFromBS(objParametriAgenda, Parametri_Aggiuntivi_for_Redirect, PaginaLink, objParametri_Server)
                    Exit Select
                End If

                If usoAgendaNG Then
                    PaginaLink = CostantiPersonalizzate.LinkQdCAngular

                    If Id_Agenda > 0 OrElse Ricetta_Operazione_Cod > 0 Then

                        'Su angular non gestiamo il Regolamento del PUA (con ddl zoo), redirect alla vecchia
                        Dim regolamento_cod As Integer
                        Select Case objParametriAgenda.TipoOperazioneAgenda
                            Case enum_Tipo_Operazione_Agenda.QuadernoDiCampagna
                                Dim objMov As New AgronicaCoreContabDAL.Movimenti_R
                                regolamento_cod = objMov.Leggi_NumProtocollo(objParametriAgenda.Piva, Sa_Cod, Id_Agenda, objParametri_Server)
                            Case enum_Tipo_Operazione_Agenda.Ricetta, enum_Tipo_Operazione_Agenda.RicettaBrogliaccio
                                Dim objMov As New AgronicaCoreContabDAL.Ricette_Operazioni_R
                                regolamento_cod = objMov.Leggi_NumProtocollo(Ricetta_Cod, Ricetta_Operazione_Cod, objParametri_Server)
                        End Select

                        If regolamento_cod = enum_PUARegolamenti.PUA_ER_2018 Then
                            PaginaLink = "../Operazioni/Trattamenti_2.aspx"
                        Else
                            If redirectFromBS Then
                                LinkRedirectFromBS(objParametriAgenda, Parametri_Aggiuntivi_for_Redirect, PaginaLink, objParametri_Server)
                            End If
                        End If
                    End If
                Else
                    PaginaLink = "../Operazioni/Trattamenti_2.aspx"
                End If


            '--- TRATTAMENTI
            Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO,
                 LAVCOD_DISERBO,
                 LAVCOD_DISSECCAMENTO,
                 LAVCOD_GEODISINFESTAZIONE,
                 LAVCOD_TRATTAMENTO_FITOREGOLATORE

                'Se provengo dall'agenda verso angular, leggo un link più complesso
                If redirectFromBS Then
                    LinkRedirectFromBS(objParametriAgenda, Parametri_Aggiuntivi_for_Redirect, PaginaLink, objParametri_Server)
                    Exit Select
                End If

                If usoAgendaNG Then
                    PaginaLink = CostantiPersonalizzate.LinkQdCAngular
                Else
                    PaginaLink = "../Operazioni/Trattamenti_2.aspx"
                End If


            Case LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE,
                 LAVCOD_PASCOLAMENTO_PROPRIO,
                 LAVCOD_PASCOLAMENTO_TERZI,
                 LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA

                'Se provengo dall'agenda verso angular, leggo un link più complesso
                If redirectFromBS Then
                    LinkRedirectFromBS(objParametriAgenda, Parametri_Aggiuntivi_for_Redirect, PaginaLink, objParametri_Server)
                    Exit Select
                End If

                If usoAgendaNG Then
                    PaginaLink = CostantiPersonalizzate.LinkQdCAngular
                Else
                    strErrore = Gias.OperazioneRegistrataNGNoPermessi
                End If

            '--- DISTRIBUZIONE INSETTI
            Case LAVCOD_DISTRIBUZIONE_INSETTI

                'Se provengo dall'agenda verso angular, leggo un link più complesso
                If redirectFromBS Then
                    LinkRedirectFromBS(objParametriAgenda, Parametri_Aggiuntivi_for_Redirect, PaginaLink, objParametri_Server)
                    Exit Select
                End If

                If usoAgendaNG Then
                    PaginaLink = CostantiPersonalizzate.LinkQdCAngular
                Else
                    If objParametriAgenda.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.Ricetta OrElse objParametriAgenda.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.RicettaBrogliaccio Then
                        'Le ricette/brogliacci di Distribuzione insetti non esistono sulla vecchia
                        strErrore = Gias.OperazioneRegistrataNGNoPermessi
                    Else
                        'Niente controllo su usoAgendaBS, è l'unica pagina possibile
                        PaginaLink = "../Operazioni/Distribuzione_Insetti.aspx"
                    End If
                End If


            '--- LAVORAZIONI
            Case LAVCOD_ANDANAMENTO, LAVCOD_ARATURA, LAVCOD_DEFOGLIAZIONE, LAVCOD_ASPORTAZIONE_ORGANI_INFETTI, LAVCOD_ASSOLCATURA,
                 LAVCOD_CARICO_MANUALE_FRUTTA, LAVCOD_CIMATURA, LAVCOD_DIRADAMENTO_MANUALE, LAVCOD_DISSODAMENTO,
                 LAVCOD_ERPICATURA, LAVCOD_ESTIRPATURA, LAVCOD_ESPIANTO, LAVCOD_FALCIACONDIZIONATURA, LAVCOD_FALCIATURA_ERBAI,
                 LAVCOD_FORMAZIONE_ARGINELLI, LAVCOD_FRANGIZOLLATURA, LAVCOD_FRESATURA, LAVCOD_IMBALLO_FIENO_ROTOLI,
                 LAVCOD_INTERRAMENTO_PAGLIE, LAVCOD_LAVORAZIONE_TRA_FILA, LAVCOD_LAVORAZIONE_SU_FILA, LAVCOD_LEGATURA,
                 LAVCOD_LIVELLAMENTO, LAVCOD_MANUTENZIONE_ARGINI, LAVCOD_MESSA_DIMORA_PIANTE, LAVCOD_MIETITREBBIATURA,
                 LAVCOD_MINIMUM_TILLAGE, LAVCOD_PACCIAMATURA, LAVCOD_POTATURA_SECCA, LAVCOD_POTATURA_VERDE, LAVCOD_PRESSATURA,
                 LAVCOD_RACCOLTA_LEGNA_POTATURA, LAVCOD_RACCOLTA_MANUALE, LAVCOD_RACCOLTA_MECCANICA, LAVCOD_RANGHINATURA,
                 LAVCOD_RINCALZATURA, LAVCOD_RIPPATURA, LAVCOD_RIPUNTATURA, LAVCOD_RIVOLTAMENTO_FORAGGIO, LAVCOD_RULLATURA,
                 LAVCOD_SARCHIATURA, LAVCOD_SCARIFICATURA, LAVCOD_SCASSO, LAVCOD_TRINCIATURA, LAVCOD_VANGATURA, LAVCOD_ZAPPATURA,
                 LAVCOD_GEBIATURA, LAVCOD_ROMPICROSTA, LAVCOD_LAVORAZIONE_CONBINATA, LAVCOD_ERPICATURA_ROTANTE, LAVCOD_INTERVENTO_ANTIBRINA,
                 LAVCOD_ALTRE_OPERAZIONI, LAVCOD_STRIGLIATURA, LAVCOD_PIRODISERBO, LAVCOD_ABBATTIMENTOIMPIANTI

                'Se provengo dall'agenda verso angular, leggo un link più complesso
                If redirectFromBS Then
                    LinkRedirectFromBS(objParametriAgenda, Parametri_Aggiuntivi_for_Redirect, PaginaLink, objParametri_Server)
                    Exit Select
                End If

                If usoAgendaNG Then
                    PaginaLink = CostantiPersonalizzate.LinkQdCAngular
                Else
                    PaginaLink = "../Operazioni/Trattamenti_2.aspx"
                End If


            '--- INSTALLAZIONE TRAPPOLE
            Case LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA,
                 LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE

                PaginaLink = "../Operazioni/Installazione_Trappole.aspx"


            '--- REINNESCO E RILIEVO AVVERSITA TRAPPOLE
            Case LAVCOD_REINNESCO_TRAPPOLE,
                 LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE

                Dim CreateLinkForRedirectToNG As Boolean = False

                'Se provengo dall'agenda verso angular, leggo un link più complesso
                'Se checkOperazioniNG = true, significa che sto arrivando dal menu vecchio, non posso fare il redirect diretto
                If redirectFromBS AndAlso Not (checkOperazioniNG) AndAlso
                    Id_Agenda = 0 Then

                    CreateLinkForRedirectToNG = True
                End If

                If usoAgendaNG AndAlso Not (checkOperazioniNG) Then
                    PaginaLink = CostantiPersonalizzate.LinkQdCAngular

                    'Controllo solo l'agenda. Non esistono Ricette di Reinnesco e Rilievo Avversità Trappole ne sul vecchio e neanche sul nuovo
                    If Id_Agenda > 0 Then
                        '-- Se è stata fatto un Reinnesco Trappole per una lavorazione collegata alla categoria magazzino "Trappole", apro la pagina aspx
                        Dim objCatMag As New AgronicaCoreMetaSchemaDAL.Categorie_Magazzino_R
                        Dim ElemCod = objCatMag.Leggi_ElemCod_Da_IdAgenda(Id_Agenda, objParametri_Server)
                        If ElemCod.Contains(TRAPPOLE) Then
                            PaginaLink = "../Operazioni/Reinnesco_Rilievi_Trappole.aspx"
                        Else
                            If redirectFromBS Then
                                CreateLinkForRedirectToNG = True
                            End If
                        End If
                    End If
                Else

                    If Id_Agenda > 0 Then

                        Dim objCatMag As New AgronicaCoreMetaSchemaDAL.Categorie_Magazzino_R
                        Dim ElemCod = objCatMag.Leggi_ElemCod_Da_IdAgenda(Id_Agenda, objParametri_Server)

                        'Se non ho il permesso per Angular, ma la lavorazione è collegata a trappole,
                        'apro la pagina aspx, altrimenti se vengo da BS e non ho i permessi per Angular non apro proprio il link

                        If ElemCod.Contains(FORMULATI) Then
                            If Not (usoAgendaNG AndAlso checkOperazioniNG) Then
                                strErrore = Gias.OperazioneRegistrataNGNoPermessi
                                Exit Select
                            ElseIf usoAgendaNG Then
                                CreateLinkForRedirectToNG = True
                            End If
                        Else
                            PaginaLink = "../Operazioni/Reinnesco_Rilievi_Trappole.aspx"
                        End If

                    Else
                        PaginaLink = "../Operazioni/Reinnesco_Rilievi_Trappole.aspx"
                    End If

                End If

                If CreateLinkForRedirectToNG Then

                    'Il Reinnesco Trappole non può essere aperta in modifica quindi forzo il tipo operazione a lettura,
                    'così da poter fare il redirect anche se l'utente ha solo permessi di lettura su Angular

                    If Lav_Cod = LAVCOD_REINNESCO_TRAPPOLE Then
                        objParametriAgenda.Tipo_Operazione = "0"
                    End If

                    LinkRedirectFromBS(objParametriAgenda, Parametri_Aggiuntivi_for_Redirect, PaginaLink, objParametri_Server)
                End If

#Region "RILIEVI"
            '--- RILIEVO ERBE INFESTANTI
            Case LAVCOD_RILIEVO_ERBE_INFESTANTI

                'Se provengo dall'agenda verso angular, leggo un link più complesso
                If redirectFromBS Then
                    LinkRedirectFromBS(objParametriAgenda, Parametri_Aggiuntivi_for_Redirect, PaginaLink, objParametri_Server)
                    Exit Select
                End If

                PaginaLink = CostantiPersonalizzate.LinkQdCAngular


            '--- RILIEVI
            Case LAVCOD_RILIEVO_AVVERSITA_CAMPO, LAVCOD_FASI_FENOLOGICHE, LAVCOD_RILIEVO_INDICI_MATURITA

                'Se provengo dall'agenda verso angular, leggo un link più complesso
                If redirectFromBS Then
                    LinkRedirectFromBS(objParametriAgenda, Parametri_Aggiuntivi_for_Redirect, PaginaLink, objParametri_Server)
                    Exit Select
                End If

                If usoAgendaNG Then
                    PaginaLink = CostantiPersonalizzate.LinkQdCAngular
                Else
                    PaginaLink = "../Operazioni/RilieviBS.aspx"
                End If


            '--- RILIEVI RACCOLTA
            Case LAVCOD_DANNI_RACCOLTA, LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA

                'Se provengo dall'agenda verso angular, leggo un link più complesso
                If redirectFromBS Then
                    LinkRedirectFromBS(objParametriAgenda, Parametri_Aggiuntivi_for_Redirect, PaginaLink, objParametri_Server)
                    Exit Select
                End If

                If usoAgendaNG Then
                    PaginaLink = CostantiPersonalizzate.LinkQdCAngular
                Else
                    PaginaLink = "../Operazioni/RilieviBS.aspx"
                End If

            '--- RILIEVO PIOGGE
            Case LAVCOD_RILIEVO_PIOGGE

                PaginaLink = "../Operazioni/RilievoPiogge.aspx"

#End Region

            '--- VISITE
            Case LAVCOD_VISITA

                Dim Permesso_Visite_NG As Boolean = objPermessi.Controlla_Permessi_Utente(
                                            objParametri_Utenti.UtenteUsername,
                                            enum_Id_Servizio.GiasOnline,
                                            enum_Security_Attivita.Visite_Lista_NG,
                                            Tipo_Operazione,
                                            Date.Now,
                                            "",
                                            objParametri_Utenti)


                'Se provengo dall'agenda verso angular, leggo un link più complesso
                If redirectFromBS AndAlso Permesso_Visite_NG Then
                    LinkRedirectFromBS(objParametriAgenda, Parametri_Aggiuntivi_for_Redirect, PaginaLink, objParametri_Server, enum_PagineGiasNG.Pagina_Edit_Visite)
                    Exit Select
                End If

                If usoAgendaNG AndAlso Permesso_Visite_NG Then
                    PaginaLink = CostantiPersonalizzate.LinkVisiteEditAngular

                    If Id_Agenda > 0 Then
                        'Su angular gestiamo solo le visiste con una sola operazione collegata
                        Dim objRif As New Agenda_Movimenti_Dettagli_Riferimenti_Helper
                        Dim DT = objRif.Leggi(objParametriAgenda.Piva, Sa_Cod,
                                          Id_Agenda,
                                          0, 0, 0, "",
                                          objParametri_Server,
                                          "(Mov_Dettagli_Riferimenti.Lav_Cod_Rif > 0 And Mov_Dettagli_Riferimenti.Lav_Cod_Rif < 1000)")


                        If DT.Count > 1 Then
                            PaginaLink = "../Operazioni/RilieviBS.aspx"
                        End If
                    End If
                Else
                    PaginaLink = "../Operazioni/RilieviBS.aspx"
                End If


                '--- IRRIGAZIONE
            Case LAVCOD_IRRIGAZIONE

                'Se provengo dall'agenda verso angular, leggo un link più complesso
                If redirectFromBS Then
                    LinkRedirectFromBS(objParametriAgenda, Parametri_Aggiuntivi_for_Redirect, PaginaLink, objParametri_Server)
                    Exit Select
                End If

                If usoAgendaNG Then
                    PaginaLink = CostantiPersonalizzate.LinkQdCAngular
                Else
                    If LeggiFlagConfigurazioneSiti Then

                        Dim dtConfigSiti As DataTable = objConfigSiti.Leggi(0, "IrrigazioneBS", "", "", objParametri_Server)

                        If Not IsNothing(dtConfigSiti) AndAlso dtConfigSiti.Rows.Count > 0 AndAlso LCase(dtConfigSiti.Rows(0).Item("Valore")) = "true" Then
                            PaginaLink = "../Operazioni/IrrigazioneBS.aspx"
                        Else
                            PaginaLink = "../Operazioni/Irrigazione.aspx"
                        End If

                    Else
                        PaginaLink = "../Operazioni/Irrigazione.aspx"
                    End If
                End If

            '--- SEMINA
            Case LAVCOD_SEMINA, LAVCOD_TRAPIANTO, LAVCOD_SOVESCIO, LAVCOD_SOD_SEDDING

                'Se provengo dall'agenda verso angular, leggo un link più complesso
                If redirectFromBS Then
                    LinkRedirectFromBS(objParametriAgenda, Parametri_Aggiuntivi_for_Redirect, PaginaLink, objParametri_Server)
                    Exit Select
                End If

                If Check_Operazione_Con_Magazzino_SuperUser(objParametriAgenda, objParametri_Server) Then
                    PaginaLink = RedirectSemina(LeggiFlagConfigurazioneSiti, objParametri_Server)
                    Exit Select
                End If

                If usoAgendaNG Then
                    PaginaLink = CostantiPersonalizzate.LinkQdCAngular
                Else
                    PaginaLink = RedirectSemina(LeggiFlagConfigurazioneSiti, objParametri_Server)
                End If


            '--- RACCOLTA
            Case LAVCOD_RACCOLTA
                Dim _prosegui As Boolean
                'Se provengo dall'agenda verso angular, leggo un link più complesso
                'Se checkOperazioniNG = true, significa che sto arrivando dal menu vecchio, non posso fare il redirect diretto
                If redirectFromBS AndAlso Not (checkOperazioniNG) AndAlso
                    Id_Agenda = 0 AndAlso Ricetta_Operazione_Cod = 0 Then
                    LinkRedirectFromBS(objParametriAgenda, Parametri_Aggiuntivi_for_Redirect, PaginaLink, objParametri_Server)
                    Exit Select
                End If

                If usoAgendaNG AndAlso Not (checkOperazioniNG) Then
                    PaginaLink = CostantiPersonalizzate.LinkQdCAngular

                    'Controllo solo l'agenda e ricette perchè sul vecchio esistono Ricette di Raccolta
                    If Id_Agenda > 0 OrElse Ricetta_Operazione_Cod > 0 Then
                        '-- Se è stata fatta una raccolta di un semilavorato vegetale faccio il redirect alla pagina della Raccolta.aspx vecchia altrimenti vado su angular
                        Dim objCatMag As New AgronicaCoreMetaSchemaDAL.Categorie_Magazzino_R
                        Dim ElemCod = objCatMag.Leggi_ElemCod_Da_IdAgenda(Id_Agenda, objParametri_Server)
                        If ElemCod.Contains(SEMILAVORATI_VEGETALI) Then
                            PaginaLink = "../Operazioni/Raccolta.aspx"
                        Else
                            If redirectFromBS Then
                                LinkRedirectFromBS(objParametriAgenda, Parametri_Aggiuntivi_for_Redirect, PaginaLink, objParametri_Server)
                            End If
                        End If
                    End If
                Else
                    If Id_Agenda > 0 OrElse Ricetta_Operazione_Cod > 0 Then
                        'QUI NON SIAMO ABILITATI AD USARE ANGULAR --- BLOCCO PER LE SEGUENTI CASISTICHE (esistenti solo su NG):
                        '1) AGENDE Raccolta con più prodotti (max 1 prodotto + 1 magazzino)
                        '2) RICETTE con Prodotti (esiste solo la FAST (no prodotto))

                        Dim bloccoRedirect As Boolean = False
                        Select Case objParametriAgenda.TipoOperazioneAgenda
                            Case enum_Tipo_Operazione_Agenda.QuadernoDiCampagna
                                'Leggo i dettagli di carico della raccolta: se > 1, blocco (sull'agenda BS di Raccolta è possibile inserire un solo prodotto)
                                Dim objMov As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
                                Dim dt = objMov.MovimentiDettagli_Leggi(objParametriAgenda.Piva, Sa_Cod, Id_Agenda,
                                                                        0, 0, 0, 0, 0, 0, 0,
                                                                        LOTTO_NONDEFINITO, 0, 0,
                                                                        Lav_Cod, CAU_CARICO, True, "", "",
                                                                        objParametri_Server)

                                If dt.Rows.Count > 1 AndAlso Not (usoAgendaNG AndAlso checkOperazioniNG) Then
                                    'E' un caso non gestito sulla vecchia ma:
                                    '- Vengo dal menu agenda (checkOperazioniNG = true)
                                    '- Non sono abilitato all'uso NG (usoAgendaNG = false)
                                    'BLOCCO IL REDIRECT
                                    bloccoRedirect = True
                                Else
                                    'E' un caso non gestito sulla vecchia ma:
                                    '- Vengo dal menu agenda (checkOperazioniNG = true)
                                    '- Sono abilitato all'uso NG (usoAgendaNG = true)
                                    'FACCIO IL REDIRECT
                                    If usoAgendaNG Then
                                        LinkRedirectFromBS(objParametriAgenda, Parametri_Aggiuntivi_for_Redirect, PaginaLink, objParametri_Server)
                                        _prosegui = True
                                        Exit Select
                                    End If
                                End If

                            Case enum_Tipo_Operazione_Agenda.Ricetta, enum_Tipo_Operazione_Agenda.RicettaBrogliaccio
                                'Leggo i dettagli di carico della raccolta: se > 0, blocco (sulle ricette BS di Raccolta NON gestiamo quelle con prodotti)
                                Dim objMov As New AgronicaCoreContabDAL.Ricette_Dettagli_R
                                Dim dt = objMov.Leggi(Ricetta_Cod, Ricetta_Operazione_Cod, 0, CAU_CARICO,
                                                      0, 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE,
                                                      enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                      "", "",
                                                      objParametri_Server)

                                If dt.Rows.Count > 0 AndAlso Not (usoAgendaNG AndAlso checkOperazioniNG) Then
                                    'E' un caso non gestito sulla vecchia ma:
                                    '- Vengo dal menu agenda (checkOperazioniNG = true)
                                    '- Non sono abilitato all'uso NG (usoAgendaNG = false)
                                    'BLOCCO IL REDIRECT
                                    bloccoRedirect = True
                                Else
                                    'E' un caso non gestito sulla vecchia ma:
                                    '- Vengo dal menu agenda (checkOperazioniNG = true)
                                    '- Sono abilitato all'uso NG (usoAgendaNG = true)
                                    'FACCIO IL REDIRECT
                                    If usoAgendaNG Then
                                        LinkRedirectFromBS(objParametriAgenda, Parametri_Aggiuntivi_for_Redirect, PaginaLink, objParametri_Server)
                                        _prosegui = True
                                        Exit Select
                                    End If
                                End If
                        End Select

                        If bloccoRedirect Then
                            strErrore = Gias.OperazioneRegistrataNGNoPermessi
                            Exit Select
                        End If
                        If _prosegui Then
                            Exit Select
                        End If
                    End If

                    PaginaLink = PaginaLink_Raccolta(objParametriAgenda,
                                                     objParametri_Server,
                                                     PaginaSitoAgendaOrigine)
                End If

            Case LAVCOD_TRATTAMENTO_POST_RACCOLTA

                'Se provengo dall'agenda verso angular, leggo un link più complesso
                If redirectFromBS Then
                    LinkRedirectFromBS(objParametriAgenda, Parametri_Aggiuntivi_for_Redirect, PaginaLink, objParametri_Server)
                    Exit Select
                End If

                If usoAgendaNG Then
                    PaginaLink = CostantiPersonalizzate.LinkQdCAngular

                    'Controllo solo l'agenda perchè sul vecchio non esistono Ricette LAVCOD_TRATTAMENTO_POST_RACCOLTA
                    If Id_Agenda > 0 Then
                        '-- Se è stata fatta una post raccolta su semilavorato vegetale faccio il redirect alla pagina Trattamenti_PostRaccolta.aspx vecchia altrimenti vado su angular
                        Dim objCatMag As New AgronicaCoreMetaSchemaDAL.Categorie_Magazzino_R
                        Dim ElemCod = objCatMag.Leggi_ElemCod_Da_IdAgenda(Id_Agenda, objParametri_Server)
                        If ElemCod.Contains(SEMILAVORATI_VEGETALI) Then
                            PaginaLink = "../Operazioni/Trattamenti_PostRaccolta.aspx"
                        Else
                            If redirectFromBS Then
                                LinkRedirectFromBS(objParametriAgenda, Parametri_Aggiuntivi_for_Redirect, PaginaLink, objParametri_Server)
                            End If
                        End If
                    End If
                Else
                    If Ricetta_Operazione_Cod > 0 Then
                        'Le ricette per questa operazione esistono solo angular
                        strErrore = Gias.OperazioneRegistrataNGNoPermessi
                        Exit Select
                    Else
                        PaginaLink = "../Operazioni/Trattamenti_PostRaccolta.aspx"
                    End If
                End If

            Case LAVCOD_CONCIA_SEME
                Dim _prosegui As Boolean

                'Se checkOperazioniNG = true, significa che sto arrivando dal menu vecchio, non posso fare il redirect diretto
                If redirectFromBS AndAlso Not (checkOperazioniNG) AndAlso
                    Id_Agenda = 0 AndAlso Ricetta_Operazione_Cod = 0 Then
                    LinkRedirectFromBS(objParametriAgenda, Parametri_Aggiuntivi_for_Redirect, PaginaLink, objParametri_Server)
                    Exit Select
                End If

                If usoAgendaNG AndAlso Not (checkOperazioniNG) Then
                    PaginaLink = CostantiPersonalizzate.LinkQdCAngular

                    'Controllo solo l'agenda e ricette perchè sul vecchio esistono Ricette di Concia del Seme (con impianti)
                    If Id_Agenda > 0 OrElse Ricetta_Operazione_Cod > 0 Then
                        Dim PresenzaImpianti As Boolean = False

                        Select Case objParametriAgenda.TipoOperazioneAgenda
                            Case enum_Tipo_Operazione_Agenda.QuadernoDiCampagna
                                Dim objMovDest As New AgronicaCoreContabDAL.Mov_Destinazioni_R
                                PresenzaImpianti = objMovDest.Leggi_PresenzaImpianti_Da_IdAgenda(Id_Agenda, objParametri_Server)
                            Case enum_Tipo_Operazione_Agenda.Ricetta, enum_Tipo_Operazione_Agenda.RicettaBrogliaccio
                                Dim objRicetteDest As New AgronicaCoreContabDAL.Ricette_Destinazioni_R
                                PresenzaImpianti = objRicetteDest.Leggi_PresenzaImpianti_Da_RicettaOperazioneCod(Ricetta_Operazione_Cod, objParametri_Server)
                        End Select

                        '-- Se è stata fatta una concia del seme su impianti faccio il redirect alla pagina Trattamenti_2.aspx altrimenti vado su angular
                        If PresenzaImpianti Then
                            PaginaLink = "../Operazioni/Trattamenti_2.aspx"
                        Else
                            If redirectFromBS Then
                                LinkRedirectFromBS(objParametriAgenda, Parametri_Aggiuntivi_for_Redirect, PaginaLink, objParametri_Server)
                            End If
                        End If
                    End If
                Else
                    If Id_Agenda > 0 OrElse Ricetta_Operazione_Cod > 0 Then
                        Dim bloccoRedirect As Boolean = False
                        'QUI NON SIAMO ABILITATI AD USARE ANGULAR --- BLOCCO PER LE SEGUENTI CASISTICHE (esistenti solo su NG):

                        '1) AGENDE Concia del seme su Prodotto da Trattare
                        '2) RICETTE di Trattamento Post Raccolta, Concia del seme su Prodotto da Trattare

                        Dim PresenzaImpianti As Boolean = False
                        Select Case objParametriAgenda.TipoOperazioneAgenda
                            Case enum_Tipo_Operazione_Agenda.QuadernoDiCampagna
                                Dim objMovDest As New AgronicaCoreContabDAL.Mov_Destinazioni_R
                                PresenzaImpianti = objMovDest.Leggi_PresenzaImpianti_Da_IdAgenda(Id_Agenda, objParametri_Server)
                            Case enum_Tipo_Operazione_Agenda.Ricetta, enum_Tipo_Operazione_Agenda.RicettaBrogliaccio
                                Dim objRicetteDest As New AgronicaCoreContabDAL.Ricette_Destinazioni_R
                                PresenzaImpianti = objRicetteDest.Leggi_PresenzaImpianti_Da_RicettaOperazioneCod(Ricetta_Operazione_Cod, objParametri_Server)
                        End Select

                        If Not PresenzaImpianti AndAlso Not (usoAgendaNG AndAlso checkOperazioniNG) Then
                            'E' un caso non gestito sulla vecchia ma:
                            '- Vengo dal menu agenda (checkOperazioniNG = true)
                            '- Non sono abilitato all'uso NG (usoAgendaNG = false)
                            'BLOCCO IL REDIRECT
                            bloccoRedirect = True
                        Else
                            'E' un caso non gestito sulla vecchia ma:
                            '- Vengo dal menu agenda (checkOperazioniNG = true)
                            '- Sono abilitato all'uso NG (usoAgendaNG = true)
                            '- L'operazione non è registrata su impianti ma su Prodotti Magazzino
                            'FACCIO IL REDIRECT
                            If usoAgendaNG AndAlso Not PresenzaImpianti Then
                                LinkRedirectFromBS(objParametriAgenda, Parametri_Aggiuntivi_for_Redirect, PaginaLink, objParametri_Server)
                                _prosegui = True
                                Exit Select
                            End If
                        End If

                        If bloccoRedirect Then
                            strErrore = Gias.OperazioneRegistrataNGNoPermessi
                            Exit Select
                        End If
                        If _prosegui Then
                            Exit Select
                        End If
                        PaginaLink = "../Operazioni/Trattamenti_2.aspx"
                    Else
                        PaginaLink = "../Operazioni/Trattamenti_2.aspx"
                    End If
                End If


            '--- DICHIARAZIONI
            Case LAVCOD_FERTILIZZAZIONI_DICHIARAZIONE_NON_UTILIZZO, LAVCOD_TRATTAMENTO_DICHIARAZIONE_NON_UTILIZZO

                'Se provengo dall'agenda verso angular, leggo un link più complesso
                If redirectFromBS Then
                    LinkRedirectFromBS(objParametriAgenda, Parametri_Aggiuntivi_for_Redirect, PaginaLink, objParametri_Server)
                    Exit Select
                End If

                If usoAgendaNG Then
                    PaginaLink = CostantiPersonalizzate.LinkQdCAngular
                Else
                    PaginaLink = "../Operazioni/Trattamenti_2.aspx"
                End If

            ' Operazione Deperecata
            '--- CURA               
            'Case LAVCOD_CURA

            '    PaginaLink = "../GestioneMagazzini/OperazioneDiCura.aspx"


            '--- MANUTENZIONE MACCHINE
            Case LAVCOD_MANUTENZIONE_MACCHINE, LAVCOD_REVISIONE_MACCHINE

                PaginaLink = "../Operazioni/ManutenzioneMacchine.aspx"


            '--- GESTIONE RIFIUTI
            Case LAVCOD_GESTIONE_RIFIUTI

                PaginaLink = "../Operazioni/GestioneRifiuti.aspx"


            '--- ZOO: CARICO
            Case LAVCOD_INCREMENTO_CONSISTENZE_ZOO, LAVCOD_NASCITA_ANIMALI, LAVCOD_ACQUISTO_ANIMALI

                PaginaLink = "../Zoo/Zoo_Carico.aspx"


            '--- ZOO: SPOSTAMENTI
            Case LAVCOD_SPOSTAMENTI_ZOO

                PaginaLink = "../Zoo/Zoo_Spostamento.aspx"


            '--- ZOO: ALIMENTAZIONE
            Case LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MANUALE_FORAGGI,
                 LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MECCANICA_FORAGGI,
                 LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MANUALE_MANGIMI,
                 LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MECCANICA_MANGIMI

                PaginaLink = "../Zoo/Zoo_Alimentazione.aspx"


            '--- ZOO: PESATURA
            Case LAVCOD_PESATURA_ANIMALI

                PaginaLink = "../Zoo/Zoo_Pesatura.aspx"


            '--- ZOO: SCARICO
            Case LAVCOD_MORTE_ANIMALI, LAVCOD_MACELLAZIONE_ANIMALI, LAVCOD_DECREMENTO_CONSISTENZE_ZOO, LAVCOD_VENDITA_ANIMALI, LAVCOD_TRASFERIMENTO_ANIMALI

                PaginaLink = "../Zoo/Zoo_Scarico.aspx"


            '--- ZOO: TRATTAMENTI
            Case LAVCOD_CUREMEDICAMENTI_ANIMALI

                'DCA 178930: per ora commentata la modifica di un movimento di scarico farmaco
                'PaginaLink = "../Zoo/Zoo_Trattamento.aspx"
                If PaginaSitoAgendaOrigine = enum_PagineAgenda_2010.Pagina_GestioneMagazziniBS Then
                    strErrore = Gias.FunzioneNonAncoraImplementata
                Else
                    PaginaLink = "../Zoo/Zoo_Trattamento.aspx"
                End If

            '--- ZOO: ALTRE LAVORAZIONI
            Case LAVCOD_ALTRE_LAVORAZIONI_ZOO

                PaginaLink = "../Zoo/Zoo_Altre_Lavorazioni.aspx"

        End Select

        ' Per le operazioni portate su AgronicaDomandaIrrigua, redirect cross-site.
        ' Costruisce un ParametriDomandaIrrigua (schema "single object") con tutti i campi necessari
        ' e ritorna l'URL completo verso DomandaIrrigua/GestioneRichieste.aspx.
        If redirectPortateDomandaIrrigua AndAlso PaginaLink <> "" AndAlso PaginaLink <> CostantiPersonalizzate.LinkQdCAngular Then
            Dim paginaRichiestaPortata As Integer = PaginaPortataDomandaIrrigua(PaginaLink)
            If paginaRichiestaPortata >= 0 Then
                PaginaLink = CostruisciRedirectDomandaIrrigua(objParametriAgenda, Lav_Cod, paginaRichiestaPortata)
                objParametriAgenda.SitoDestinazione = Enum_SiteRedirector.Sito_AgronicaDomandaIrrigua
            End If
        End If

        'Se devo andare su Angular, imposto il sito destinazione corretto
        If PaginaLink = CostantiPersonalizzate.LinkQdCAngular Then
            objParametriAgenda.SitoDestinazione = Enum_SiteRedirector.GiasNG
        End If

        Return PaginaLink

    End Function

    ''' <summary>
    ''' Mappa il PaginaLink (path relativo) ai valori dell'enum enum_PagineAgronicaDomandaIrrigua
    ''' per le pagine operazione portate su AgronicaDomandaIrrigua. Ritorna -1 se la pagina non è portata.
    ''' I valori restituiti coincidono con i corrispondenti di enum_PagineAgenda_2010 (compatibilità
    ''' con il payload Angular).
    ''' </summary>
    Private Shared Function PaginaPortataDomandaIrrigua(ByVal PaginaLink As String) As Integer
        Select Case PaginaLink
            Case "../Operazioni/Trattamenti_2.aspx"
                Return enum_PagineAgronicaDomandaIrrigua.Pagina_Trattamenti_B
            Case "../Operazioni/Installazione_Trappole.aspx"
                Return enum_PagineAgronicaDomandaIrrigua.Pagina_Installazione_Trapppole
            Case "../Operazioni/Reinnesco_Rilievi_Trappole.aspx"
                Return enum_PagineAgronicaDomandaIrrigua.Pagina_Reinnesco_Trappole
            Case "../Operazioni/RilieviBS.aspx"
                Return enum_PagineAgronicaDomandaIrrigua.Pagina_RilieviBS
            Case "../Operazioni/Raccolta.aspx"
                Return enum_PagineAgronicaDomandaIrrigua.Pagina_Raccolta
            Case "../Operazioni/Trattamenti_PostRaccolta.aspx"
                Return enum_PagineAgronicaDomandaIrrigua.Pagina_TrattamentiPostRaccolta
            Case "../Operazioni/Distribuzione_Insetti.aspx"
                Return enum_PagineAgronicaDomandaIrrigua.Pagina_Distribuzione_Insetti
            Case Else
                Return -1
        End Select
    End Function

    ''' <summary>
    ''' Per le operazioni portate su AgronicaDomandaIrrigua, costruisce un ParametriDomandaIrrigua
    ''' con tutti i campi necessari, lo salva in sessione e ritorna l'URL completo per il redirect cross-site.
    ''' Schema "single object": l'XML in uscita contiene SOLO il blocco ParametriDomandaIrrigua, evitando
    ''' la coesistenza con ParametriAgenda_2010 e i conseguenti conflitti di parsing/precedenza.
    ''' </summary>
    Private Shared Function CostruisciRedirectDomandaIrrigua(
        ByRef objParametriAgenda As ParametriAgenda,
        ByVal Lav_Cod As Integer,
        ByVal paginaRichiesta As Integer
    ) As String

        Dim objPDI As New ParametriDomandaIrrigua()
        Dim tmpInt As Integer = 0

        objPDI.PaginaRichiesta = paginaRichiesta
        objPDI.PaginaProvenienza = objParametriAgenda.PaginaSitoOrigine
        objPDI.Piva = If(objParametriAgenda.Piva, "")
        objPDI.DataSelezionata = objParametriAgenda.Data
        objPDI.Lav_Cod = Lav_Cod

        If Not String.IsNullOrEmpty(objParametriAgenda.Id_Agenda) AndAlso Integer.TryParse(objParametriAgenda.Id_Agenda, tmpInt) Then
            objPDI.Id_Agenda = tmpInt
        End If

        If Not String.IsNullOrEmpty(objParametriAgenda.Sa_Cod) AndAlso Integer.TryParse(objParametriAgenda.Sa_Cod, tmpInt) Then
            objPDI.Sa_Cod = tmpInt
        End If

        ' Veg_Cod su ParametriDomandaIrrigua è String (può contenere "specie/var/clone"); su ParametriAgenda_2010
        ' era Integer e qui veniva troncato. Manteniamo la stringa originale per non perdere info.
        objPDI.Veg_Cod = If(objParametriAgenda.Veg_Cod, "")

        If Not String.IsNullOrEmpty(objParametriAgenda.Appezza) AndAlso Integer.TryParse(objParametriAgenda.Appezza, tmpInt) Then
            objPDI.Appezza = tmpInt
        End If

        If Not String.IsNullOrEmpty(objParametriAgenda.Id_Imp) AndAlso Integer.TryParse(objParametriAgenda.Id_Imp, tmpInt) Then
            objPDI.Id_Reg = tmpInt
        End If

        If Not String.IsNullOrEmpty(objParametriAgenda.TipoOperazioneAgenda) AndAlso Integer.TryParse(objParametriAgenda.TipoOperazioneAgenda, tmpInt) Then
            objPDI.TipoOperazioneAgenda = CType(tmpInt, enum_Tipo_Operazione_Agenda)
        End If

        objPDI.TargetOperazione = objParametriAgenda.TargetOperazione
        objPDI.Programmazione_Cod = objParametriAgenda.Programmazione_Cod
        objPDI.TipoRicetta = objParametriAgenda.TipoRicetta

        If Not String.IsNullOrEmpty(objParametriAgenda.Tipo_Operazione) AndAlso Integer.TryParse(objParametriAgenda.Tipo_Operazione, tmpInt) Then
            objPDI.Tipo_Operazione = CType(tmpInt, enum_TipoOperazioneDB)
        End If

        objPDI.RedirectUrl = If(objParametriAgenda.RedirectUrl, "")
        objPDI.QueryStringFiltrino = If(objParametriAgenda.QueryStringFiltrino, "")

        ' Determino il SitoOrigine
        Dim sitoOrigine As Enum_SiteRedirector = CType(objParametriAgenda.SitoOrigine, Enum_SiteRedirector)
        If sitoOrigine = 0 Then sitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010

        ' Salva in sessione + serializza su web_parametri e ritorna l'URL cross-site
        objPDI.Salva()
        Return RedirectGestione.IndirizzoCompleto_Sito_AgronicaDomandaIrrigua_PassandoDirettamente_ParametriDomandaIrrigua(
            sitoOrigine, objPDI)
    End Function

    Private Shared Sub LinkRedirectFromBS(ByVal objParametriAgenda As ParametriAgenda,
                                          ByVal Parametri_Aggiuntivi_for_Redirect As JObject,
                                          ByRef PaginaLink As String,
                                          ByVal objParametri_Server As AgronicaCoreParametri,
                                          Optional ByVal paginaRichiesta As enum_PagineGiasNG = enum_PagineGiasNG.Pagina_Edit_Attivita)


        Dim Parametri_Aggiuntivi As New JObject
        Parametri_Aggiuntivi.Item("Id_Agenda") = objParametriAgenda.Id_Agenda
        Parametri_Aggiuntivi.Item("TipoOperazioneDB") = objParametriAgenda.Tipo_Operazione
        Parametri_Aggiuntivi.Item("QSF") = objParametriAgenda.QueryStringFiltrino
        Parametri_Aggiuntivi.Item("Pagina_Provenienza") = objParametriAgenda.PaginaSitoOrigine

        Dim Ricetta_Cod = MenuBS_2017_RedirectGestione.getValueParametriAggiuntivi(Parametri_Aggiuntivi_for_Redirect, "Ricetta_Cod")
        If Not IsNothing(Ricetta_Cod) AndAlso Not String.IsNullOrEmpty(Ricetta_Cod) Then
            Parametri_Aggiuntivi.Item("Ricetta_Cod") = Parametri_Aggiuntivi_for_Redirect.Item("Ricetta_Cod")
        End If

        Dim Ricetta_Operazione_Cod = MenuBS_2017_RedirectGestione.getValueParametriAggiuntivi(Parametri_Aggiuntivi_for_Redirect, "Ricetta_Operazione_Cod")
        If Not IsNothing(Ricetta_Operazione_Cod) AndAlso Not String.IsNullOrEmpty(Ricetta_Operazione_Cod) Then
            Parametri_Aggiuntivi.Item("Ricetta_Operazione_Cod") = Parametri_Aggiuntivi_for_Redirect.Item("Ricetta_Operazione_Cod")
        End If

        Select Case objParametriAgenda.TipoOperazioneAgenda
            Case enum_Tipo_Operazione_Agenda.QuadernoDiCampagna

                Parametri_Aggiuntivi.Item("TipoOperazioneAgenda") = AgronicaCoreModelsSTD.attivita.Attivita.Tipo_Attivita.QuadernoDiCampagna

            Case enum_Tipo_Operazione_Agenda.Ricetta, enum_Tipo_Operazione_Agenda.RicettaBrogliaccio

                Parametri_Aggiuntivi.Item("TipoOperazioneAgenda") = AgronicaCoreModelsSTD.attivita.Attivita.Tipo_Attivita.Ricetta
                Parametri_Aggiuntivi.Item("TipoRicetta") = objParametriAgenda.TipoRicetta

                If objParametriAgenda.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.Ricetta Then
                    Parametri_Aggiuntivi.Item("Stato") = AgronicaCoreModelsSTD.attivita.Attivita.Stati.Da_Eseguire
                Else
                    Parametri_Aggiuntivi.Item("Stato") = AgronicaCoreModelsSTD.attivita.Attivita.Stati.Eseguita
                End If

        End Select


        MenuBS_2017_RedirectGestione.RedirectGenerico(objParametriAgenda.Piva, Enum_SiteRedirector.GiasNG,
                                                      paginaRichiesta, PaginaLink,
                                                      objParametri_Server, Parametri_Aggiuntivi, objParametriAgenda.SitoOrigine)

        objParametriAgenda.SitoDestinazione = Enum_SiteRedirector.GiasNG

    End Sub
    Public Function Utente_Abilitato_OperazioniAgendaNG(ByVal Tipo_Operazione As enum_Security_Operazione, ByVal objParametri_Server As AgronicaCoreParametri, ByVal objParametri_Utenti As AgronicaCoreParametri)


        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

        'CONTROLLO ABILITAZIONE AD USO NG
        Dim dtConfigSitiNG As DataTable = objConfigSiti.Leggi(0, "OperazioniAgendaNG", "", "", objParametri_Server)

        Dim UtenteAbilitatoNG As Boolean = Verifica_Permessi_OperazioniAgenda_NG(Tipo_Operazione, objParametri_Utenti)

        Dim usoAgendaNG As Boolean = If(Not IsNothing(dtConfigSitiNG) AndAlso dtConfigSitiNG.Rows.Count > 0 AndAlso LCase(dtConfigSitiNG.Rows(0).Item("Valore")) = "true" AndAlso UtenteAbilitatoNG, True, False)

        Return usoAgendaNG
    End Function

    Public Function Utente_Abilitato_MenuAgendaNG(ByVal Tipo_Operazione As enum_Security_Operazione, ByVal objParametri_Server As AgronicaCoreParametri, ByVal objParametri_Utenti As AgronicaCoreParametri)


        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

        'CONTROLLO ABILITAZIONE AD USO NG
        Dim dtConfigSitiNG As DataTable = objConfigSiti.Leggi(0, "MenuAgendaNG", "", "", objParametri_Server)

        Dim UtenteAbilitatoNG As Boolean = Verifica_Permessi_OperazioniAgenda_NG(Tipo_Operazione, objParametri_Utenti)

        Dim usoAgendaNG As Boolean = If(Not IsNothing(dtConfigSitiNG) AndAlso dtConfigSitiNG.Rows.Count > 0 AndAlso LCase(dtConfigSitiNG.Rows(0).Item("Valore")) = "true" AndAlso UtenteAbilitatoNG, True, False)

        Return usoAgendaNG
    End Function

    Private Function Verifica_Permessi_OperazioniAgenda_NG(ByVal Tipo_Operazione As enum_Security_Operazione, ByVal objParametri_Utenti As AgronicaCoreParametri) As Boolean

        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

        Dim UtenteAbilitatoNG As Boolean = objPermessi.Controlla_Permessi_Utente(
                                objParametri_Utenti.UtenteUsername,
                                enum_Id_Servizio.GiasOnline,
                                enum_Security_Attivita.Agenda_AccessoMenu_NG,
                                Tipo_Operazione,
                                Date.Now,
                                "",
                                objParametri_Utenti)

        Return UtenteAbilitatoNG
    End Function

    ''' <summary>
    ''' Funzione da richiamare solo per le Pagine dell'AgronicaAgenda_2010:
    ''' Permette di aprire dalla Pagina delle Operazioni di Angular le pagine dell'AgronicaAgenda_2010 anche se si ha abilitato solo la nuova Operazioni NG
    ''' e non si ha il permesso per le pagine BS.
    ''' </summary>
    Public Sub Verifica_Permessi_OperazioniAgenda_X_PagineAgronicaAgenda(ByVal objParametri_Server As AgronicaCoreParametri, ByVal objParametri_Utenti As AgronicaCoreParametri, ByRef UtenteAbilitatoLettura As Boolean, ByRef UtenteAbilitatoModifica As Boolean)

        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

        Dim UtenteAbilitatoLetturaBS As Boolean = False
        Dim UtenteAbilitatoModificaBS As Boolean = False
        Dim UtenteAbilitatoLetturaNG As Boolean = False
        Dim UtenteAbilitatoModificaNG As Boolean = False

        UtenteAbilitatoLetturaBS = objPermessi.Controlla_Permessi_Utente(
                                        objParametri_Utenti.UtenteUsername,
                                        enum_Id_Servizio.GiasOnline,
                                        enum_Security_Attivita.Agenda_AccessoMenu,
                                        enum_Security_Operazione.Lettura,
                                        Date.Now,
                                        "",
                                        objParametri_Utenti)


        UtenteAbilitatoModificaBS = objPermessi.Controlla_Permessi_Utente(
                                                                            objParametri_Utenti.UtenteUsername,
                                                                            enum_Id_Servizio.GiasOnline,
                                                                            enum_Security_Attivita.Agenda_AccessoMenu,
                                                                            enum_Security_Operazione.Modifica,
                                                                            Date.Now,
                                                                            "",
                                                                            objParametri_Utenti)

        If Not UtenteAbilitatoLetturaBS Then
            UtenteAbilitatoLetturaNG = Utente_Abilitato_OperazioniAgendaNG(enum_Security_Operazione.Lettura, objParametri_Server, objParametri_Utenti)
        End If

        If Not UtenteAbilitatoModificaBS Then
            UtenteAbilitatoModificaNG = Utente_Abilitato_OperazioniAgendaNG(enum_Security_Operazione.Modifica, objParametri_Server, objParametri_Utenti)
        End If

        UtenteAbilitatoLettura = IIf(UtenteAbilitatoLetturaBS OrElse UtenteAbilitatoLetturaNG, True, False)

        UtenteAbilitatoModifica = IIf(UtenteAbilitatoModificaBS OrElse UtenteAbilitatoModificaNG, True, False)

    End Sub

    Private Shared Function get_isRilievo(lav_cod As Integer) As Boolean

        Dim isRilievo As Boolean = False

        Select Case lav_cod
            Case LAVCOD_RILIEVO_FALDA,
                 LAVCOD_FASI_FENOLOGICHE, LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE,
                 LAVCOD_RILIEVO_AVVERSITA_CAMPO, LAVCOD_RILIEVO_ERBE_INFESTANTI,
                 LAVCOD_RILIEVO_PIOGGE, LAVCOD_REINNESCO_TRAPPOLE, LAVCOD_MONITORAGGIO_ACQUE,
                 LAVCOD_DANNI_RACCOLTA, LAVCOD_RILIEVO_INDICI_MATURITA,
                 LAVCOD_RACCOLTA, LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA

                isRilievo = True
        End Select
        Return isRilievo
    End Function

    Private Shared Function RedirectSemina(ByVal LeggiFlagConfigurazioneSiti As Boolean, ByVal objParametri_Server As AgronicaCoreParametri) As String

        Dim PaginaLink As String = ""

        If LeggiFlagConfigurazioneSiti Then

            Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

            Dim dtConfigSiti As DataTable = objConfigSiti.Leggi(0, "SeminaBS", "", "", objParametri_Server)

            If Not IsNothing(dtConfigSiti) AndAlso dtConfigSiti.Rows.Count > 0 AndAlso LCase(dtConfigSiti.Rows(0).Item("Valore")) = "true" Then
                PaginaLink = "../Operazioni/Trattamenti_2.aspx"
            Else
                PaginaLink = "../Operazioni/Semina_E_Trapianto_1.aspx"
            End If
        End If

        Return PaginaLink
    End Function

    ''' <summary>
    ''' Controlla se l'operazione è stata creata dalla Trattamenti_2 con scarico da magazzino superuser.
    ''' In tal caso in modifica è apribile solamente dalla Trattamenti_2.
    ''' </summary>
    ''' <param name="objParametriAgenda"></param>
    ''' <param name="objParametri_Server"></param>
    ''' <returns></returns>
    Private Shared Function Check_Operazione_Con_Magazzino_SuperUser(ByVal objParametriAgenda As ParametriAgenda, ByVal objParametri_Server As AgronicaCoreParametri) As Boolean

        Dim op_terzista_old As Boolean = False

        If objParametriAgenda.Id_Agenda <> 0 AndAlso Not String.IsNullOrEmpty(objParametriAgenda.Piva) Then

            Dim objAgronicaCoreContab As New AgronicaCoreContabDAL.Mov_Destinazioni_R

            Dim DT = objAgronicaCoreContab.Leggi("", 0, objParametriAgenda.Id_Agenda, 0, 0, 0, 0, 20,
                                                        enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "Mov_Destinazioni.Piva <> '" & objParametriAgenda.Piva & "'", "", objParametri_Server)

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                op_terzista_old = True
            End If

        End If

        Return op_terzista_old
    End Function
End Class
