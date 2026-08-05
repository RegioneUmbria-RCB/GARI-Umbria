Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreModelsSTD.attivita.Attivita
Imports System.Xml
Imports AgronicaCoreModello.My.Resources
Imports AgronicaCoreModelsSTD.exceptions

Namespace OperazioneAgenda_Temp

    Public Module Operazione_Agenda_Utility

        Private Enum enum_Matrice_Delete
            Piva = 0
            Sa_Cod = 1
            Id_Agenda = 2
            Id_Mov = 3
            Id_Mov_Det = 4
        End Enum

        'Cancella l'operazione e quelle collegate se presenti,
        'se viene lanciata una eccezione allora non è possibile cancellare l'operazione richiesta, verificare il messaggio dell'eccezione.
        'Se non viene lanciata una eccezione ma ritorna false allora l'operazione non è stata cancellata a causa di un warning,
        'per cancellare comunque l'operazione a prescindere dai warning occorre impostare a true IgnoraAvvisoWarning 
        Public Function Cancella_Operazione_E_Collegate(ByRef objParametriAgenda As ParametriAgenda_Temp.ParametriAgenda,
                                                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                        ByRef messaggio As String,
                                                        ByVal IgnoraAvvisoWarning As Boolean,
                                                            Optional ByVal CancellaOperazioneSingolaNoCorrelate As Boolean = False,
                                                            Optional ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri = Nothing,
                                                            Optional lista_IdAgenda_Cancellati As List(Of Integer) = Nothing,
                                                        Optional ByRef list_IdAgenda_DaCancellare As List(Of Integer) = Nothing
                                                        ) As Boolean

            Select Case CInt(objParametriAgenda.Lav_Cod)

                'Case LAVCOD_CARICO, LAVCOD_SCARICO
                '    '1022 = carico   1023 = scarico
                '    Throw New Exception("Per eliminare un carico o uno scarico occorre andare nella gestione magazzini o nella gestione contabilità!")

                'Case 3000 To 3999

                '    Dim obj_zoo As New AgronicaCoreAnagrafeBIZ.Zoo

                '    obj_zoo.Elimina_Operazione(objParametriAgenda.Piva, objParametriAgenda.Id_Agenda, objParametri_Server)

                Case Else
                    'Case LAVCOD_RACCOLTA, LAVCOD_SEMINA, LAVCOD_TRAPIANTO, LAVCOD_SOVESCIO, _
                    '            LAVCOD_BOLLA_RICEVUTA...
                    'LAVCOD_RACCOLTA
                    'Modifica in data 29/07/2009:
                    'per cancellare una raccolta ci si deve occupare di effetuare dei controlli per 
                    'verificare se effettivamente l'operazione si può cancellare oppure no
                    'questo perchè la raccolta può essere legata ad una bolla di conferimento
                    '(tramite la gestione dei riferimenti)
                    'e per evitare di mandare in negativo la giacenza di un semilavorato:
                    'se il semilavorato raccolto è stato scaricato con una normale bolla emessa
                    '(quindi non ci sono riferimenti e si è bypassato il controllo sopra), 
                    'non viene consentita la cancellazione 

                    'LAVCOD_SEMINA
                    'Modifica in data 16/02/2012:
                    'per cancellare una semina ci si deve occupare di effetuare dei controlli per 
                    'verificare se effettivamente l'operazione si può cancellare oppure no
                    'questo perchè la semina può essere legata ad una fattura, ddt, 
                    'oppure alle operazioni di movimentazione di magazzino fittizie
                    'per scarico semente da magazzino di un'impresa padre
                    '(tramite la gestione dei riferimenti)

                    If Not GestisciCancellazione(objParametriAgenda,
                                                 objParametri_Server,
                                                 messaggio,
                                                 IgnoraAvvisoWarning,
                                                 CancellaOperazioneSingolaNoCorrelate,
                                                 objParametri_Utenti:=objParametri_Utenti,
                                                 lista_IdAgenda_Cancellati:=lista_IdAgenda_Cancellati,
                                                 list_IdAgenda_DaCancellare:=list_IdAgenda_DaCancellare) Then
                        Return False
                    End If

                    'Case Else

                    '    'Per le altre operazioni proseguo normalmente eliminando il record agenda e quelli nelle tabelle collegate
                    '    'verifico per sicurezza che non ci sia un record nella tabella riferimenti
                    '    Dim objAgenda As New Agenda_Operazione_Helper
                    '    objAgenda.Cancella(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, objParametriAgenda.Id_Agenda, True, objParametri_Server)

            End Select


            Return True


        End Function


        Public Function GestisciCancellazione(ByRef objParametriAgenda As ParametriAgenda_Temp.ParametriAgenda,
                                              ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                              ByRef messaggio As String, ByVal IgnoraAvvisoWarning As Boolean,
                                                  Optional ByVal CancellaOperazioneSingolaNoCorrelate As Boolean = False,
                                                  Optional ByVal Id_Mov As Integer = 0,
                                                  Optional ByVal Id_Mov_Det As Integer = 0,
                                                  Optional ByVal Modalita_Protetta As Integer = 0,
                                                  Optional ByVal Bypass_Delete_Exceptions As Boolean = False,
                                                  Optional ByVal Preserva_Costi As Boolean = False,
                                                  Optional ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri = Nothing,
                                                  Optional ByRef docData As Date = Nothing,
                                                  Optional ByRef docDesLib As String = "",
                                                  Optional lista_IdAgenda_Cancellati As List(Of Integer) = Nothing,
                                                  Optional ByVal Preserva_GHG As Boolean = False,
                                                  Optional ByVal idServizio As enum_Id_Servizio = enum_Id_Servizio.GiasOnline,
                                                  Optional ByVal origine As enum_SistemiEsterni = enum_SistemiEsterni.gias,
                                                  Optional ByRef list_IdAgenda_DaCancellare As List(Of Integer) = Nothing,
                                                  Optional ByRef erroreInCancellazione As Boolean = False
                                              ) As Boolean

            If objParametriAgenda.Id_Agenda = 0 Then
                Throw New GiasException("Specificare l'idAgenda da cancellare")
            End If

            Dim obj_Agenda_R As New AgronicaCoreContabDAL.Agenda_R
            Dim DT_Agenda = obj_Agenda_R.Leggi(objParametriAgenda.Piva,
                                               0,
                                               objParametriAgenda.Id_Agenda,
                                               0,
                                               enumSelezioneVariabile.Selezione_TabellaCompleta,
                                               "",
                                               "",
                                               objParametri_Server)

            If DT_Agenda.Rows.Count > 0 Then

                If IsTestataDaCancellare(DT_Agenda, Id_Mov, Id_Mov_Det, Bypass_Delete_Exceptions) Then

                    UtilityHelper.SeControllaPermessiStatoWorkflowCancellazione(
                        DT_Agenda.Rows(0).Item("Lav_Cod"),
                        objParametriAgenda.Piva,
                        objParametriAgenda.Id_Agenda,
                        objParametri_Server,
                        objParametri_Utenti)

                End If

                Dim permessiGruppiMerce As Boolean = ControllaPermessiVisibilitaGruppiMerce(
                    objParametri_Server,
                    objParametri_Utenti,
                    objParametriAgenda.Piva,
                    objParametriAgenda.Id_Agenda,
                    DT_Agenda.Rows(0).Item("Lav_Cod"))

                If permessiGruppiMerce = False Then
                    Throw New GiasException("Il documento contiene dettagli non visualizzabili per il gruppo merce a loro associato, non è possibile continuare con la cancellazione")
                End If

                docData = DT_Agenda.Rows(0).Item("Validita_Inizio")
                docDesLib = DT_Agenda.Rows(0).Item("Des_Lib")

                Dim logErroriCanc As String = ""

                If CancellaOperazioneSingolaNoCorrelate = False Then

                    'Elimino le Agende con lo stesso Raccoglitore_Cod ed elimino anche le Agende collegate con i Riferimenti (Carichi,Scarichi,DDT Emessi,DDT Ricevuti,Reinneschi delle Installazioni Trappole)

                    Dim list_Agenda_ToDelete As New List(Of Tuple(Of String, Integer, Integer))

                    If Not IsDBNull(DT_Agenda.Rows(0).Item("Raccoglitore_Cod")) AndAlso DT_Agenda.Rows(0).Item("Raccoglitore_Cod") <> 0 Then

                        Dim DT_Agenda_Racc As DataTable = obj_Agenda_R.Leggi_Raccoglitore(DT_Agenda.Rows(0).Item("Raccoglitore_Cod"), objParametriAgenda.Piva, objParametri_Server)

                        For Each Agenda_row In DT_Agenda_Racc.Rows

                            list_Agenda_ToDelete.Add(Tuple.Create(CStr(Agenda_row("Piva")), CInt(Agenda_row("Sa_Cod")), CInt(Agenda_row("Id_Agenda"))))

                            'Solo per Operazioni QDC
                            If DT_Agenda.Rows(0).Item("Lav_Cod") > 0 And DT_Agenda.Rows(0).Item("Lav_Cod") < 1000 Then
                                Trova_Agende_Dai_Riferimenti_Da_Cancellare(Agenda_row("Piva"), Agenda_row("Sa_Cod"), Agenda_row("Id_Agenda"), objParametri_Server, list_Agenda_ToDelete)
                            End If

                        Next

                    Else

                        list_Agenda_ToDelete.Add(Tuple.Create(objParametriAgenda.Piva, CInt(objParametriAgenda.Sa_Cod), CInt(objParametriAgenda.Id_Agenda)))

                        'Solo per Operazioni QDC
                        If DT_Agenda.Rows(0).Item("Lav_Cod") > 0 And DT_Agenda.Rows(0).Item("Lav_Cod") < 1000 Then
                            Trova_Agende_Dai_Riferimenti_Da_Cancellare(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, objParametriAgenda.Id_Agenda, objParametri_Server, list_Agenda_ToDelete)
                        End If

                    End If


                    If list_Agenda_ToDelete.Count > 0 Then
                        For Each Agenda_ToDelete In list_Agenda_ToDelete

                            objParametriAgenda.Piva = Agenda_ToDelete.Item1
                            objParametriAgenda.Sa_Cod = Agenda_ToDelete.Item2
                            objParametriAgenda.Id_Agenda = Agenda_ToDelete.Item3

                            Dim matrice_delete(,) As String

                            Erase matrice_delete

                            If ControllaOperazione(matrice_delete, objParametriAgenda, objParametri_Server, messaggio, IgnoraAvvisoWarning, "",
                                                    Id_Mov, Id_Mov_Det, Modalita_Protetta, Bypass_Delete_Exceptions, objParametri_Utenti:=objParametri_Utenti,
                                                    list_IdAgenda_DaCancellare:=list_IdAgenda_DaCancellare) Then

                                Dim rval As Boolean = OperazioneAgenda_Cancella(matrice_delete, objParametriAgenda, objParametri_Server, False, idServizio, origine, logErroriCanc)
                                If rval = False Then
                                    messaggio = logErroriCanc
                                    erroreInCancellazione = True
                                    Return rval
                                End If

                                If lista_IdAgenda_Cancellati IsNot Nothing Then
                                    lista_IdAgenda_Cancellati.Add(objParametriAgenda.Id_Agenda)
                                End If

                            Else
                                'Se la funzione di controllo restituisce false senza dare eccezione, significa che:
                                '- sta restituendo una messaggio di warning e
                                '- il flag IgnoraAvvisoWarning mi è arrivato false
                                'quindi fermo la cancellazione e restituisco il messaggio all'utente.
                                'Tipicamente questo dovrebbe avvenire sulla operazione di agenda richiesta da cancellare,
                                'ma potrebbe in linea teorica anche avvenire su una delle sue operazioni correlate censite nella lista 'list_Agenda_ToDelete'.
                                'Nel caso sia l'agenda da cancellare, che quelle correlate dovessero avere un messaggio di warning ne verrebbe mostrato solo uno,
                                'ma questo è un edge-case, è più importante restituire False ed il messaggio, soprattutto nello scenario più comune di un solo elemento
                                'in 'list_Agenda_ToDelete', altrimenti il chiamante notifica all'utente una avvenuta cancellazione non effettuata. Anche perché per
                                'le operazioni non QdC le correlate vengono censite dalla funzione ControllaOperazione stessa valorizzando l'argomento matrice_delete.
                                Return False
                            End If
                        Next
                    End If

                    Return True

                Else

                    Dim matrice_delete(4, 0) As String
                    matrice_delete(enum_Matrice_Delete.Piva, 0) = objParametriAgenda.Piva
                    matrice_delete(enum_Matrice_Delete.Sa_Cod, 0) = objParametriAgenda.Sa_Cod
                    matrice_delete(enum_Matrice_Delete.Id_Agenda, 0) = objParametriAgenda.Id_Agenda
                    matrice_delete(enum_Matrice_Delete.Id_Mov, 0) = Id_Mov
                    matrice_delete(enum_Matrice_Delete.Id_Mov_Det, 0) = Id_Mov_Det

                    Dim rval As Boolean = OperazioneAgenda_Cancella(matrice_delete, objParametriAgenda, objParametri_Server, CancellaOperazioneSingolaNoCorrelate, idServizio, origine, logErroriCanc)
                    If rval = False Then
                        messaggio = logErroriCanc
                        erroreInCancellazione = True
                    End If
                    Return rval

                End If

            End If

            Return False

        End Function

        ''' <summary>
        ''' Cerca nelle tabelle dei riferimenti le agende collegate a quella passata
        ''' e le aggiunge alla lista di agende da cancellare 
        ''' (per ora cancello solo le operazioni contabili e i reiinschi se sto facendo un'installazione trappole)
        ''' </summary>
        ''' <param name="Piva"></param>
        ''' <param name="Sa_Cod"></param>
        ''' <param name="Id_Agenda"></param>
        ''' <param name="objParametri_Server"></param>
        ''' <param name="list_Agenda_ToDelete"></param>
        Private Sub Trova_Agende_Dai_Riferimenti_Da_Cancellare(ByVal Piva As String, ByVal Sa_Cod As Integer, ByVal Id_Agenda As Integer,
                                                               ByVal objParametri_Server As AgronicaCoreParametri, ByRef list_Agenda_ToDelete As List(Of Tuple(Of String, Integer, Integer)))

            Dim obj_Mov_Det_Riferimenti As New AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R

            Dim DT_Rif = obj_Mov_Det_Riferimenti.Leggi_Specifica(Piva, Sa_Cod, Id_Agenda, 0, 0, 0, "",
                                                        "", 0, 0, 0, 0,
                                                        0, "", "", "", objParametri_Server)

            If Not IsNothing(DT_Rif) AndAlso DT_Rif.Rows.Count > 0 Then
                For Each Agenda_rif In DT_Rif.Rows

                    If Agenda_rif("Lav_Cod_Rif") = LAVCOD_CARICO OrElse Agenda_rif("Lav_Cod_Rif") = LAVCOD_SCARICO OrElse
                        Agenda_rif("Lav_Cod_Rif") = LAVCOD_BOLLA_EMESSA OrElse Agenda_rif("Lav_Cod_Rif") = LAVCOD_BOLLA_RICEVUTA OrElse
                        (Agenda_rif("Lav_Cod") = LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA AndAlso Agenda_rif("Lav_Cod_Rif") = LAVCOD_REINNESCO_TRAPPOLE) Then

                        list_Agenda_ToDelete.Add(Tuple.Create(CStr(Agenda_rif("Piva_Rif")), CInt(Agenda_rif("Sa_Cod_Rif")), CInt(Agenda_rif("Id_Agenda_Rif"))))

                    End If
                Next
            End If
        End Sub

        Private Function IsTestataDaCancellare(ByVal DT_Agenda As DataTable,
                                               ByVal Id_Mov As Integer,
                                               ByVal Id_Mov_Det As Integer,
                                               ByVal Bypass_Delete_Exceptions As Boolean) As Boolean

            'Controllo che sia valorizzato il LavCod
            If Not IsDBNull(DT_Agenda.Rows(0).Item("Lav_Cod")) AndAlso DT_Agenda.Rows(0).Item("Lav_Cod") <> 0 Then

                'Controllo che sia una testata e una effettiva cancellazione
                If Id_Mov = 0 And Id_Mov_Det = 0 And Bypass_Delete_Exceptions = False Then

                    Return True

                End If

            End If

            Return False

        End Function

        Public Function ControllaOperazione(ByRef matrice_delete(,) As String,
                                            ByRef objParametriAgenda As ParametriAgenda_Temp.ParametriAgenda,
                                            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            ByRef messaggio As String,
                                            ByVal IgnoraAvvisoWarning As Boolean,
                                            Optional ByRef messaggio2 As String = "",
                                            Optional ByVal Id_Mov As Integer = 0,
                                            Optional ByVal Id_Mov_Det As Integer = 0,
                                            Optional ByRef Modalita_Protetta As Integer = 0,
                                            Optional ByVal Bypass_Delete_Exceptions As Boolean = False,
                                            Optional ByVal Preserva_Costi As Boolean = False,
                                            Optional ByRef list_msgOperazioniCollegate As List(Of String) = Nothing,
                                            Optional ByVal Preserva_GHG As Boolean = False,
                                            Optional ByVal objParametri_Utenti As AgronicaCoreParametri = Nothing,
                                            Optional ByRef list_IdAgenda_DaCancellare As List(Of Integer) = Nothing
                                            ) As Boolean

            Dim Filtro_Aggiuntivo As String = ""
            Dim strFiltro_Dettaglio As String = ""
            Dim Flag_ProdottoScaricato As Boolean = False
            Dim Des_Lib_Scarico As String = ""
            Dim PermettoEliminazioneRaccoltaPercheCura As Boolean = False
            Dim Data_Movimento_Scarico As String = ""
            Dim msg As String = ""
            Dim strMessaggio_Info As String
            Dim strMessaggio_Info_Azione As String
            Dim bBypass As Boolean

            '================================================================================
            ' Caricamento matrice cancellazione + messaggio tipo operazione
            '================================================================================

            Select Case objParametriAgenda.Tipo_Operazione

                Case enum_TipoOperazioneDB.Modifica

                    strMessaggio_Info = Gias.Modifica
                    strMessaggio_Info_Azione = Gias.Modificare

                Case enum_TipoOperazioneDB.Cancellazione

                    strMessaggio_Info = Gias.Cancellazione
                    strMessaggio_Info_Azione = Gias.Eliminare

                    If matrice_delete Is Nothing Then

                        ReDim matrice_delete(4, 0)

                        matrice_delete(enum_Matrice_Delete.Piva, 0) = objParametriAgenda.Piva
                        matrice_delete(enum_Matrice_Delete.Sa_Cod, 0) = objParametriAgenda.Sa_Cod
                        matrice_delete(enum_Matrice_Delete.Id_Agenda, 0) = objParametriAgenda.Id_Agenda
                        matrice_delete(enum_Matrice_Delete.Id_Mov, 0) = Id_Mov
                        matrice_delete(enum_Matrice_Delete.Id_Mov_Det, 0) = Id_Mov_Det

                    Else


                        ReDim Preserve matrice_delete(4, UBound(matrice_delete, 2) + 1)
                        matrice_delete(enum_Matrice_Delete.Piva, UBound(matrice_delete, 2)) = objParametriAgenda.Piva
                        matrice_delete(enum_Matrice_Delete.Sa_Cod, UBound(matrice_delete, 2)) = objParametriAgenda.Sa_Cod
                        matrice_delete(enum_Matrice_Delete.Id_Agenda, UBound(matrice_delete, 2)) = objParametriAgenda.Id_Agenda
                        matrice_delete(enum_Matrice_Delete.Id_Mov, UBound(matrice_delete, 2)) = Id_Mov
                        matrice_delete(enum_Matrice_Delete.Id_Mov_Det, UBound(matrice_delete, 2)) = Id_Mov_Det

                    End If

                Case Else

                    strMessaggio_Info = "" 'Eccezione
                    strMessaggio_Info_Azione = ""

            End Select

            Dim bTELEREGISTRI As Boolean = False
            Dim Modulo_Generazione As Integer


            '================================================================================
            'Lettura del Modulo collegato all'agenda
            '================================================================================

            Dim leggiAgenda As New AgronicaCoreContabDAL.Agenda_R
            Dim DTAgenda As New DataTable

            DTAgenda = leggiAgenda.Leggi(objParametriAgenda.Piva, 0, objParametriAgenda.Id_Agenda, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

            If DTAgenda.Rows.Count > 0 Then

                If IsDBNull(DTAgenda(0)("Modulo")) Then
                    Modulo_Generazione = 0
                Else
                    Modulo_Generazione = DTAgenda(0)("Modulo")
                End If

            End If

            '================================================================================
            ' Controlli: Teleregistri
            '================================================================================
            bTELEREGISTRI = True

            If Modulo_Generazione = 1 AndAlso bTELEREGISTRI Then

                'Todo

                'Controllo Invio Teleregistri
                'If cCar.CheckBloccoStatoxTeleRegistri(objParametriAgenda.Piva, objParametriAgenda.Id_Agenda, "cancellazione") Then

            End If


            '================================================================================
            ' Controlli: Dettagli riferimenti
            '================================================================================

            Dim DT_Rif As DataTable = New AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R().Recupera_DT_Rif_Unificato(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod,
                                                                                                                       objParametriAgenda.Id_Agenda,
                                                                                                                       Id_Mov, Id_Mov_Det,
                                                                                                                       0, "",
                                                                                                                       objParametri_Server)

            Dim bypassConferimentoBollaEmessa As Boolean = False

            If DT_Rif.Rows.Count <> 0 Then

                For Each dr_rif As DataRow In DT_Rif.Rows

                    Dim Lav_Cod As Integer = dr_rif.Item("Lav_Cod")
                    Dim Cau_Mov As String = dr_rif.Item("Cau_Mov")
                    Dim Lav_Cod_Rif As Integer = dr_rif.Item("Lav_Cod_Risultato")

                    '----------------------------------------------------------------------------------------------
                    '------------------------Raccolta e cura-------------------------------------------------------
                    '----------------------------------------------------------------------------------------------

                    If ((Lav_Cod = LAVCOD_CURA AndAlso Lav_Cod_Rif = LAVCOD_SCARICO) OrElse
                        (Lav_Cod = LAVCOD_CURA AndAlso Lav_Cod_Rif = LAVCOD_CARICO) OrElse
                        (Lav_Cod = LAVCOD_RACCOLTA AndAlso Lav_Cod_Rif = LAVCOD_SCARICO) OrElse
                        (Lav_Cod = LAVCOD_RACCOLTA AndAlso Lav_Cod_Rif = LAVCOD_CARICO) OrElse
                        (Lav_Cod = LAVCOD_SCARICO AndAlso Lav_Cod_Rif = LAVCOD_CURA) OrElse
                        (Lav_Cod = LAVCOD_CARICO AndAlso Lav_Cod_Rif = LAVCOD_CURA) OrElse
                        (Lav_Cod = LAVCOD_SCARICO AndAlso Lav_Cod_Rif = LAVCOD_RACCOLTA) OrElse
                        (Lav_Cod = LAVCOD_CARICO AndAlso Lav_Cod_Rif = LAVCOD_RACCOLTA)) AndAlso
                       Cau_Mov = "" AndAlso
                       dr_rif.Item("Id_Mov") = -1 AndAlso
                       dr_rif.Item("Id_Mov_Risultato") = -1 AndAlso
                       dr_rif.Item("Id_Mov_Det") = -1 AndAlso
                       dr_rif.Item("Id_Mov_Det_Risultato") = -1 Then

                        'Gestione Operazione di cura a seguito di raccolta
                        'Se ho lo scarico/carico diretto allora dopo la cura
                        'viene fatto uno scarico dall'azienda di cura e un carico nell'azienda di origine
                        'collego le tre operazioni, solo agenda non i movimenti o i dettagli,
                        'lascio come idagendarif quello della cura, come idagenda quello del carico e scarico

                        If Lav_Cod = LAVCOD_CURA OrElse Lav_Cod = LAVCOD_RACCOLTA Then

                            'ok, posso modificare o eliminare
                            InserisciOperazioniEdit(matrice_delete, objParametriAgenda.Tipo_Operazione, dr_rif)

                            'ok
                            PermettoEliminazioneRaccoltaPercheCura = True

                        End If

                        If Lav_Cod_Rif = LAVCOD_CURA Then

                            'blocco
                            msg = ControllaOperazione_GeneraMsg("", dr_rif, Gias.NonPrevisto & ".")
                            If Lav_Cod = LAVCOD_CARICO Then
                                msg = ControllaOperazione_GeneraMsg("", dr_rif, Gias.La & " ***replace*** " & AgronicaCoreModelloRes.OperazioneCaricoNonConsentitaOpCura)
                            ElseIf Lav_Cod = LAVCOD_SCARICO Then
                                msg = ControllaOperazione_GeneraMsg("", dr_rif, Gias.La & " ***replace*** " & AgronicaCoreModelloRes.OperazioneScaricoNonConsentitaOpCura)
                            End If

                            Throw New GiasException(ReplaceMessaggio(msg, Id_Mov_Det, strMessaggio_Info))
                            Throw New GiasException(ReplaceMessaggio(msg, Id_Mov_Det, strMessaggio_Info))

                        End If

                        If Lav_Cod_Rif = LAVCOD_RACCOLTA Then

                            'blocco
                            msg = ControllaOperazione_GeneraMsg("", dr_rif, Gias.NonPrevisto & ".")
                            If Lav_Cod = LAVCOD_CARICO Then
                                msg = ControllaOperazione_GeneraMsg("", dr_rif, Gias.La & " ***replace*** " & AgronicaCoreModelloRes.OperazioneCaricoNonConsentitaRaccoltaOpCura)
                            ElseIf Lav_Cod = LAVCOD_SCARICO Then
                                msg = ControllaOperazione_GeneraMsg("", dr_rif, Gias.La & " ***replace*** " & AgronicaCoreModelloRes.OperazioneScaricoNonConsentitaRaccoltaOpCura)
                            End If

                            Throw New GiasException(ReplaceMessaggio(msg, Id_Mov_Det, strMessaggio_Info))

                        End If

                        '----------------------------------------------------------------------------------------------
                        '----------------------------------------------------------------------------------------------
                        '----------------------------------------------------------------------------------------------

                    ElseIf Lav_Cod = LAVCOD_VISITA Then

                        'Cancello le operazioni collegate
                        InserisciOperazioniEdit(matrice_delete, objParametriAgenda.Tipo_Operazione, dr_rif)

                    ElseIf Lav_Cod = LAVCOD_INCREMENTO_CONSISTENZE_ZOO Then

                        If AnimaliMovimentati_Next(objParametriAgenda.Piva, objParametriAgenda.Id_Agenda, objParametri_Server) Then

                            msg = ControllaOperazione_GeneraMsg("", dr_rif, Gias.La & " ***replace*** " & AgronicaCoreModelloRes.NonConsentitaAnimaliMovimentati)

                            Throw New GiasException(ReplaceMessaggio(msg, Id_Mov_Det, strMessaggio_Info))

                        End If

                        InserisciOperazioniEdit(matrice_delete, objParametriAgenda.Tipo_Operazione, dr_rif)

                    Else

                        bBypass = True

                        '########################################################################################################################
                        '##################################### CONTROLLI PRELIMINARI ############################################################
                        '########################################################################################################################

                        'Operazioni collegate da cancellare sempre in modo non bloccante

                        Select Case dr_rif.Item("Lav_Cod_Risultato")

                            Case LAVCOD_PARTITA_DOPPIA

                                messaggio &= ControllaOperazione_GeneraMsg(messaggio, dr_rif, Gias.Attenzione & " " & AgronicaCoreModelloRes.IlMovimentoCheSiDesidera & " " & strMessaggio_Info_Azione & " " & AgronicaCoreModelloRes.AssociatoContabilizzazione & "<BR>" &
                                                                                    dr_rif.Item("Des_Lib_Risultato") & " " & Gias.del & " " & CDate(dr_rif.Item("Validita_Inizio_Risultato")).ToShortDateString & "." & "<BR>")

                                InserisciOperazioniEdit(matrice_delete, objParametriAgenda.Tipo_Operazione, dr_rif)

                            Case LAVCOD_COSTI_CDG

                                messaggio &= ControllaOperazione_GeneraMsg(messaggio, dr_rif, Gias.Attenzione & " " & AgronicaCoreModelloRes.MovimentoAssociatoOreCosti)
                                messaggio = Replace(messaggio, "***replace***", strMessaggio_Info, 1)

                                If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Cancellazione AndAlso
                                   Preserva_Costi = True Then
                                    'Sto di fatto cancellando per poter riscrivere il dettaglio, quindi devo evitare che venga brasata l'operazione dei costi collegata
                                Else
                                    InserisciOperazioniEdit(matrice_delete, objParametriAgenda.Tipo_Operazione, dr_rif)
                                End If

                            Case LAVCOD_GHG

                                If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Cancellazione And Not Preserva_GHG Then
                                    InserisciOperazioniEdit(matrice_delete, objParametriAgenda.Tipo_Operazione, dr_rif)
                                End If

                            Case Else

                                '########################################################################################################################
                                '##################################### CONTROLLO OPERAZIONE #############################################################
                                '########################################################################################################################

                                Select Case objParametriAgenda.Lav_Cod

                                    Case LAVCOD_DAA_EMESSO

                                              'Consentito DAA --> nessuna cancellazione documento allegato

                                    Case LAVCOD_NOTA_ACCREDITO_RICEVUTA, LAVCOD_NOTA_ACCREDITO_EMESSA

                                             'Nota a Credito/Debito --> Allegata Fattura --> nessuna cancellazione documento allegato

                                    Case LAVCOD_FATTURA_EMESSA,
                                            LAVCOD_FATTURA_LIQ_CONF_EMESSA, LAVCOD_FATTURA_LIQ_CONF_RICEVUTA,
                                            LAVCOD_AUTOFATTURA_LIQ_CONF_EMESSA, LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA

                                        Select Case dr_rif.Item("Lav_Cod_Risultato")

                                            Case LAVCOD_DAA_EMESSO

                                                Select Case objParametriAgenda.Tipo_Operazione

                                                    Case enum_TipoOperazioneDB.Modifica

                                                        'Richiesta cancellazione fattura allegata a daa
                                                        messaggio &= ControllaOperazione_GeneraMsg(messaggio, dr_rif, Gias.Attenzione & " " & Gias.Il & " ***replace2*** " & AgronicaCoreModelloRes.LegatoOperazione_ & "<BR>" &
                                                            dr_rif.Item("Des_Lib_Risultato") & " " & Gias.del & " " & CDate(dr_rif.Item("Validita_Inizio_Risultato")).ToShortDateString & "." & "<BR>" &
                                                           "")

                                                        Modalita_Protetta = 1

                                                    Case enum_TipoOperazioneDB.Cancellazione

                                                        'Richiesta cancellazione fattura allegata a daa
                                                        msg = ControllaOperazione_GeneraMsg("", dr_rif, Gias.Attenzione & " " & Gias.Il & " ***replace2*** " & AgronicaCoreModelloRes.LegatoOperazione_ & "<BR>" &
                                                                dr_rif.Item("Des_Lib_Risultato") & " " & Gias.del & " " & CDate(dr_rif.Item("Validita_Inizio_Risultato")).ToShortDateString & "." & "<BR>" &
                                                               Gias.La & " ***replace*** " & Gias.del & " ***replace2*** " & AgronicaCoreModelloRes.PertantoConsentitaSoloPrevia & " ***replace*** " & AgronicaCoreModelloRes.DAAAllegato)

                                                        Throw New GiasException(ReplaceMessaggio(msg, Id_Mov_Det, strMessaggio_Info))

                                                End Select

                                            Case Else

                                                bBypass = False 'Controllo carico/scarico collegato ad imprese GIAS

                                        End Select


                                    Case LAVCOD_ORDINE_VENDITA, LAVCOD_ORDINE_ACQUISTO

                                        Select Case objParametriAgenda.Tipo_Operazione

                                            Case enum_TipoOperazioneDB.Modifica

                                                'Richiesta modifica ordine allegato a documento
                                                messaggio &= ControllaOperazione_GeneraMsg(messaggio, dr_rif, Gias.Attenzione & " " & Gias.Il & " ***replace2*** " & AgronicaCoreModelloRes.LegatoOperazione_ & "<BR>" &
                                                                                            dr_rif.Item("Des_Lib_Risultato") & " " & Gias.del & " " & CDate(dr_rif.Item("Validita_Inizio_Risultato")).ToShortDateString & "." & "<BR>")

                                                Modalita_Protetta = 1

                                            Case enum_TipoOperazioneDB.Cancellazione

                                                If Bypass_Delete_Exceptions AndAlso
                                                   objParametriAgenda.Lav_Cod = LAVCOD_ORDINE_VENDITA AndAlso
                                                   dr_rif.Item("Lav_Cod_Risultato") = LAVCOD_TESTATE_ORDINE_LAVORAZIONE Then

                                                    'ByPass modifica Ordine Vendita collegato a Ordine Lavorazione

                                                Else

                                                    'Richiesta cancellazione ordine allegato a documento
                                                    msg = ControllaOperazione_GeneraMsg("", dr_rif, Gias.Attenzione & " " & Gias.Il & " ***replace2*** " & AgronicaCoreModelloRes.LegatoOperazione_ & "<BR>" &
                                                                                        dr_rif.Item("Des_Lib_Risultato") & " " & Gias.del & " " &
                                                                                        CDate(dr_rif.Item("Validita_Inizio_Risultato")).ToShortDateString & "." & "<BR>" &
                                                                                        Gias.La & " ***replace*** " & Gias.del & " ***replace2*** " & AgronicaCoreModelloRes.PertantoConsentitaSoloPrevia & " ***replace*** " & Gias.del & " ***replace3***.")

                                                    Throw New GiasException(ReplaceMessaggio(msg, Id_Mov_Det, strMessaggio_Info))

                                                End If

                                        End Select

                                    Case LAVCOD_SEMINA, LAVCOD_TRAPIANTO, LAVCOD_TRAPIANTO_IN_SERRA

                                        'Controllo generazione movimenti fittizi creati dalla semina e da cancellare

                                        If CInt(dr_rif.Item("Preserva_Legame")) = 0 Then

                                            Select Case objParametriAgenda.Tipo_Operazione

                                                Case enum_TipoOperazioneDB.Modifica

                                                    'Do nothing

                                                Case enum_TipoOperazioneDB.Cancellazione

                                                    InserisciOperazioniEdit(matrice_delete, objParametriAgenda.Tipo_Operazione, dr_rif)

                                            End Select

                                        Else

                                            'Nesssuna Cancellazione Ulteriore

                                        End If

                                        bBypass = False

                                    Case LAVCOD_RACCOLTA

                                        'Controllo Raccolta Agganciata a Conferimento Passivo (Carico) vedi FF... in caso di conferimento attivo occorre poterla modificare e conseguentemente anche il ddt di conferimento attivo

                                        Select Case dr_rif.Item("Lav_Cod_Risultato")

                                            Case LAVCOD_DISTINTA_CARICO, LAVCOD_DISTINTA_CARICO_ACCETTAZIONE, LAVCOD_AUTO_DDT_EMESSO, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE, LAVCOD_ACCETTAZIONE_DIVERSI

                                                'Edit Raccolta Non Consentita                                        

                                                msg = ControllaOperazione_GeneraMsg("", dr_rif, Gias.Attenzione & " " & Gias.Il & " ***replace2*** " & AgronicaCoreModelloRes.LegatoOperazione_ & "<BR>" &
                                                                                                                    dr_rif.Item("Des_Lib_Risultato") & " " & Gias.del & " " & CDate(dr_rif.Item("Validita_Inizio_Risultato")).ToShortDateString & "." & "<BR>" &
                                                                                                                   Gias.La & " ***replace*** " & AgronicaCoreModelloRes.DellaRaccolta & " " & AgronicaCoreModelloRes.PertantoConsentitaSoloPrevia & " ***replace*** " & AgronicaCoreModelloRes.DelDocumentoAllegato)

                                                Throw New GiasException(ReplaceMessaggio(msg, Id_Mov_Det, strMessaggio_Info))

                                            Case Else

                                                messaggio &= ControllaOperazione_GeneraMsg(messaggio, dr_rif, Gias.Attenzione & " " & AgronicaCoreModelloRes.RaccoltaCollegataDocumento & "<BR>" &
                                                                                          dr_rif.Item("Des_Lib_Risultato") & " " & Gias.del & " " & CDate(dr_rif.Item("Validita_Inizio_Risultato")).ToShortDateString & "." & "<BR>")

                                                InserisciOperazioniEdit(matrice_delete, objParametriAgenda.Tipo_Operazione, dr_rif)

                                        End Select

                                    Case Else

                                        'Ulteriori Controlli Necessari (vedi sotto)

                                        bBypass = False

                                End Select

                        End Select

                        If Not bBypass Then

                            '########################################################################################################################
                            '########################## CONTROLLO RIFERIMENTO COLLEGATO #############################################################
                            '########################################################################################################################

                            Select Case dr_rif.Item("Lav_Cod_Risultato")

                                Case LAVCOD_BOLLA_RICEVUTA

                                    Select Case Cau_Mov

                                        'l'op colturale è agganciata a un ddt ricevuto

                                        Case CAU_TRATTAMENTO, CAU_RILIEVO_CAMPO, CAU_LAVORAZIONE ', CAU_RILIEVO_RACCOLTA

                                            'Se non esiste il collegamento ad uno ed uno solo movimento contabile mi genera una eccezione voluta,
                                            'ma avendo già filtrato per LAVCOD_BOLLA_RICEVUTA devo averlo per forza
                                            'se si vuole solo controllare bisogna usare Esiste_Almeno_Un_Doc_Contabile_Riferito come nela cancellazione

                                            Dim Doc_Numero As Integer = New AgronicaCoreContabDAL.Movimenti_R().DocNumero_from_IdAgenda(dr_rif.Item("Piva_Risultato"),
                                                                                                                                                                    dr_rif.Item("Id_Agenda_Risultato"),
                                                                                                                                                                    "", objParametri_Server)

                                            If Doc_Numero = 0 Then

                                                'bisogna verificare se il ddt è fittizio oppure no
                                                'va verificato se il doc_numero è =0
                                                'se doc_numero =0
                                                'si salva il suo id_agenda per poterlo cancellare in automatico
                                                'salvo il movimento associato da cancellare

                                                InserisciOperazioniEdit(matrice_delete, objParametriAgenda.Tipo_Operazione, dr_rif)

                                            Else

                                                'se doc_numero <>0 il ddt non è fittizio
                                                'quindi cancello solo la semina

                                            End If

                                    End Select

                                Case LAVCOD_SEMINA, LAVCOD_TRAPIANTO, LAVCOD_SOVESCIO, LAVCOD_SOD_SEDDING

                                    'l'operazione in oggetto è collegata a una semina
                                    'possono essere diverse, verifico qual è

                                    Select Case Lav_Cod

                                        Case LAVCOD_SCARICO, LAVCOD_BOLLA_RICEVUTA

                                            'questi sono i movimenti fittizi di movimento di magazzino
                                            'creati per fare in modo di usare le giacenze di un'impresa padre

                                            Select Case objParametriAgenda.Tipo_Operazione

                                                Case enum_TipoOperazioneDB.Modifica

                                                    messaggio = ControllaOperazione_GeneraMsg(messaggio, dr_rif, Gias.Attenzione & " " & AgronicaCoreModelloRes.MovimentoAllegatoSemina & "<BR>" &
                                                                            dr_rif.Item("Des_Lib_Risultato") & " " & Gias.del & " " & CDate(dr_rif.Item("Validita_Inizio_Risultato")).ToShortDateString & "." & "<BR>" &
                                                                           "")


                                                    '28/03/2024 Permetto la cancellazione di carichi e scarichi fittizi
                                                    'Case enum_TipoOperazioneDB.Cancellazione

                                                    '    'CANCELLAZIONE NON CONSENTITA

                                                    '    msg = ControllaOperazione_GeneraMsg("", dr_rif, Gias.Attenzione & " " & AgronicaCoreModelloRes.MovimentoAllegatoSemina & "<BR>" &
                                                    '                            dr_rif.Item("Des_Lib_Risultato") & " " & Gias.Del & " " & CDate(dr_rif.Item("Validita_Inizio_Risultato")).ToShortDateString & "." & "<BR>" &
                                                    '                           AgronicaCoreModelloRes.CancellazioneAutomaticaDaSemina)

                                                    '    Throw New Exception(ReplaceMessaggio(msg, Id_Mov_Det, strMessaggio_Info))

                                            End Select

                                    End Select

                                Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_TRATTAMENTO_FITOREGOLATORE, LAVCOD_CONCIA_SEME,
                                     LAVCOD_GEODISINFESTAZIONE, LAVCOD_DISSECCAMENTO, LAVCOD_DISERBO

                                    Select Case Lav_Cod

                                        Case LAVCOD_BOLLA_EMESSA

                                            Select Case objParametriAgenda.Tipo_Operazione

                                                Case enum_TipoOperazioneDB.Modifica

                                                    messaggio &= ControllaOperazione_GeneraMsg(messaggio, dr_rif, Gias.Attenzione & " " & Gias.Il & " ***replace2*** " & AgronicaCoreModelloRes.LegatoOperazione_ & "<BR>" &
                                                                dr_rif.Item("Des_Lib_Risultato") & " " & Gias.del & " " & CDate(dr_rif.Item("Validita_Inizio_Risultato")).ToShortDateString & " " & AgronicaCoreModelloRes.DellaImpresa & " " & dr_rif.Item("rag_soc_Risultato") & "." & "<BR>")

                                                    Modalita_Protetta = 1

                                                Case enum_TipoOperazioneDB.Cancellazione

                                                    'CANCELLAZIONE NON CONSENTITA

                                                    msg = ControllaOperazione_GeneraMsg("", dr_rif, Gias.Attenzione & " " & Gias.Il & " ***replace2*** " & AgronicaCoreModelloRes.LegatoOperazione_ & "<BR>" &
                                                                dr_rif.Item("Des_Lib_Risultato") & " " & Gias.del & " " & CDate(dr_rif.Item("Validita_Inizio_Risultato")).ToShortDateString & " " & AgronicaCoreModelloRes.DellaImpresa & " " & dr_rif.Item("rag_soc_Risultato") & "." & "<BR>" &
                                                               AgronicaCoreModelloRes.CancellazioneAutomaticaDaOperazioneCollegata)

                                                    Throw New GiasException(ReplaceMessaggio(msg, Id_Mov_Det, strMessaggio_Info))

                                            End Select

                                        Case LAVCOD_FATTURA_RICEVUTA

                                            Select Case Cau_Mov

                                                'l'op colturale è agganciata a un ddt ricevuto

                                                Case CAU_TRATTAMENTO, CAU_RILIEVO_CAMPO, CAU_LAVORAZIONE ', CAU_RILIEVO_RACCOLTA

                                                    'Se non esiste il collegamento ad uno ed uno solo movimento contabile mi genera una eccezione voluta,
                                                    'ma avendo già filtrato per LAVCOD_BOLLA_RICEVUTA devo averlo per forza
                                                    'se si vuole solo controllare bisogna usare Esiste_Almeno_Un_Doc_Contabile_Riferito come nela cancellazione

                                                    Dim Doc_Numero As Integer = New AgronicaCoreContabDAL.Movimenti_R().DocNumero_from_IdAgenda(dr_rif.Item("Piva_Risultato"),
                                                                                                                                                            dr_rif.Item("Id_Agenda_Risultato"),
                                                                                                                                                            "", objParametri_Server)

                                                    If Doc_Numero = 0 Then

                                                        'bisogna verificare se il ddt è fittizio oppure no
                                                        'va verificato se il doc_numero è =0
                                                        'se doc_numero =0
                                                        'si salva il suo id_agenda per poterlo cancellare in automatico
                                                        'salvo il movimento associato da cancellare

                                                        InserisciOperazioniEdit(matrice_delete, objParametriAgenda.Tipo_Operazione, dr_rif)

                                                    Else

                                                        'se doc_numero <> 0 il ddt non è fittizio
                                                        'quindi cancello solo la semina

                                                    End If

                                                Case CAU_REGISTRAZIONI

                                                    Select Case objParametriAgenda.Tipo_Operazione

                                                        Case enum_TipoOperazioneDB.Modifica

                                                            messaggio &= ControllaOperazione_GeneraMsg(messaggio, dr_rif, Gias.Attenzione & " " & Gias.Il & " ***replace2*** " & AgronicaCoreModelloRes.LegatoFattura_ & "<BR>" &
                                                                        dr_rif.Item("Des_Lib_Risultato") & " " & Gias.del & " " & CDate(dr_rif.Item("Validita_Inizio_Risultato")).ToShortDateString & "." & "<BR>")

                                                            Modalita_Protetta = 1

                                                        Case enum_TipoOperazioneDB.Cancellazione

                                                            'CANCELLAZIONE DELLA BOLLA ALLEGATA NON CONSENTITA!!!!!

                                                            msg = ControllaOperazione_GeneraMsg("", dr_rif, Gias.Attenzione & " " & Gias.Il & " ***replace2*** " & AgronicaCoreModelloRes.LegatoFattura_ & "<BR>" &
                                                                        dr_rif.Item("Des_Lib_Risultato") & " " & Gias.del & " " & CDate(dr_rif.Item("Validita_Inizio_Risultato")).ToShortDateString & "." & "<BR>" &
                                                                        Gias.La & " ***replace*** " & Gias.del & " ***replace2*** " & AgronicaCoreModelloRes.PertantoConsentitaSoloPrevia & " ***replace*** " & Gias.del & " ***replace3***.")

                                                            Throw New GiasException(ReplaceMessaggio(msg, Id_Mov_Det, strMessaggio_Info))

                                                    End Select

                                            End Select

                                    End Select

                                Case LAVCOD_FATTURA_EMESSA, LAVCOD_FATTURA_LIQ_CONF_EMESSA, LAVCOD_FATTURA_LIQ_CONF_RICEVUTA, LAVCOD_AUTOFATTURA_LIQ_CONF_EMESSA, LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA

                                    Select Case objParametriAgenda.Tipo_Operazione

                                        Case enum_TipoOperazioneDB.Modifica

                                            messaggio &= ControllaOperazione_GeneraMsg(messaggio, dr_rif, Gias.Attenzione & " " & Gias.Il & " ***replace2*** " & AgronicaCoreModelloRes.LegatoFattura_ & "<BR>" &
                                                                        dr_rif.Item("Des_Lib_Risultato") & " " & Gias.del & " " & CDate(dr_rif.Item("Validita_Inizio_Risultato")).ToShortDateString & "." & "<BR>")

                                            Modalita_Protetta = 1

                                        Case enum_TipoOperazioneDB.Cancellazione

                                            'CANCELLAZIONE DELLA BOLLA ALLEGATA NON CONSENTITA!!!!!

                                            msg = ControllaOperazione_GeneraMsg("", dr_rif, Gias.Attenzione & " " & Gias.Il & " ***replace2*** " & AgronicaCoreModelloRes.LegatoFattura_ & "<BR>" &
                                                                        dr_rif.Item("Des_Lib_Risultato") & " " & Gias.del & " " & CDate(dr_rif.Item("Validita_Inizio_Risultato")).ToShortDateString & "." & "<BR>" &
                                                                       Gias.La & " ***replace*** " & Gias.del & " ***replace2*** " & AgronicaCoreModelloRes.PertantoConsentitaSoloPrevia & " ***replace*** " & Gias.del & " ***replace3***.")

                                            Throw New GiasException(ReplaceMessaggio(msg, Id_Mov_Det, strMessaggio_Info))

                                    End Select

                                Case LAVCOD_BOLLA_EMESSA

                                    Select Case Lav_Cod

                                        Case LAVCOD_FATTURA_RICEVUTA, LAVCOD_FATTURA_LIQ_CONF_RICEVUTA, LAVCOD_FATTURA_LIQ_CONF_EMESSA,
                                             LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA, LAVCOD_AUTOFATTURA_LIQ_CONF_EMESSA

                                            'Ok

                                        Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_TRATTAMENTO_FITOREGOLATORE, LAVCOD_CONCIA_SEME,
                                             LAVCOD_GEODISINFESTAZIONE, LAVCOD_DISSECCAMENTO, LAVCOD_DISERBO,
                                             LAVCOD_SEMINA, LAVCOD_TRAPIANTO, LAVCOD_TRAPIANTO_IN_SERRA

                                            'verifico se il DDT legato all'operazione corrente è legato anche ad altre operazioni 
                                            '(caso salvataggio operazioni multi-centro con scarico dal terzista --> creano un solo DDT emesso dal terzista)

                                            Dim DT_Rif_2 As DataTable = New AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R().Recupera_DT_Rif_Unificato(
                                                                                                                                                   dr_rif.Item("Piva_Risultato"),
                                                                                                                                                   dr_rif.Item("Sa_Cod_Risultato"),
                                                                                                                                                   dr_rif.Item("Id_Agenda_Risultato"),
                                                                                                                                                   0, 0, 0, "", objParametri_Server)

                                            For Each dr_rif_2 As DataRow In DT_Rif_2.Rows

                                                If dr_rif_2.Item("Id_Agenda_Risultato") <> objParametriAgenda.Id_Agenda Then

                                                    InserisciOperazioniEdit(matrice_delete, objParametriAgenda.Tipo_Operazione, dr_rif_2)

                                                End If

                                            Next

                                            Select Case objParametriAgenda.Tipo_Operazione

                                                Case enum_TipoOperazioneDB.Modifica

                                                    msg = ControllaOperazione_GeneraMsg("", dr_rif, Gias.Attenzione & " " & Gias.Il & " ***replace2*** " & AgronicaCoreModelloRes.DaModificareAssociatoA & "<BR>" &
                                                                                                  dr_rif.Item("Des_Lib_Risultato") & " " & Gias.del & " " & CDate(dr_rif.Item("Validita_Inizio_Risultato")).ToShortDateString & " " & AgronicaCoreModelloRes.DellaImpresa & " " & dr_rif.Item("rag_soc_Risultato") & "." & "<BR>" &
                                                                                                  AgronicaCoreModelloRes.NecessarioReinserireMovimento)
                                                    Throw New GiasException(ReplaceMessaggio(msg, Id_Mov_Det, strMessaggio_Info))

                                                Case enum_TipoOperazioneDB.Cancellazione

                                                    messaggio &= ControllaOperazione_GeneraMsg(messaggio, dr_rif, Gias.Attenzione & " " & Gias.Il & " ***replace2*** " & AgronicaCoreModelloRes.DaEliminareAssociatoA & "<BR>" &
                                                                                              dr_rif.Item("Des_Lib_Risultato") & " " & Gias.del & " " & CDate(dr_rif.Item("Validita_Inizio_Risultato")).ToShortDateString & " " & AgronicaCoreModelloRes.DellaImpresa & " " & dr_rif.Item("rag_soc_Risultato") & "." & "<BR>" &
                                                                                              AgronicaCoreModelloRes.SiDesideraEliminareOperazioneEDocumento)

                                            End Select

                                            InserisciOperazioniEdit(matrice_delete, objParametriAgenda.Tipo_Operazione, dr_rif)

                                        Case LAVCOD_ACCETTAZIONE_DIVERSI, LAVCOD_DISTINTA_CARICO_ACCETTAZIONE, LAVCOD_AUTO_DDT_EMESSO, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE
                                            'Uguale alla parte del LAVCOD_SCARICO

                                            bypassConferimentoBollaEmessa = bypassConferimentoBollaEmessa OrElse dr_rif("Tipo_Associazione") = 2

                                            Select Case objParametriAgenda.Tipo_Operazione

                                                Case enum_TipoOperazioneDB.Modifica

                                                Case enum_TipoOperazioneDB.Cancellazione

                                                    'Se la colonna Tipo_Associazione è uguale a 2 sono nel caso di conferimento collegato a bolla emessa creata automaticamente
                                                    'su azienda agricola a seguito di raccolta, quindi posso cancellare la bolla senza chiedere conferma
                                                    If dr_rif("Tipo_Associazione") <> 2 Then
                                                        'Richiesta cancellazione ddt reso allegato ad accettazione
                                                        messaggio &= ControllaOperazione_GeneraMsg(messaggio, dr_rif, Gias.Attenzione & " " & Gias.Il & " ***replace2*** " & AgronicaCoreModelloRes.LegatoOperazione_ & "<BR>" &
                                                                                                    dr_rif.Item("Des_Lib_Risultato") & " " & Gias.del & " " & CDate(dr_rif.Item("Validita_Inizio_Risultato")).ToShortDateString & "." & "<BR>" &
                                                                                                   AgronicaCoreModelloRes.ConfermaCancellaDueDocumenti)
                                                    End If

                                                    InserisciOperazioniEdit(matrice_delete, objParametriAgenda.Tipo_Operazione, dr_rif)

                                            End Select

                                    End Select

                                Case LAVCOD_SCARICO

                                    Select Case Lav_Cod
                                        Case LAVCOD_ACCETTAZIONE_DIVERSI, LAVCOD_DISTINTA_CARICO_ACCETTAZIONE, LAVCOD_AUTO_DDT_EMESSO, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE
                                            'Uguale alla parte del LAVCOD_BOLLA_EMESSA

                                            bypassConferimentoBollaEmessa = bypassConferimentoBollaEmessa OrElse dr_rif("Tipo_Associazione") = 2

                                            Select Case objParametriAgenda.Tipo_Operazione

                                                Case enum_TipoOperazioneDB.Modifica

                                                Case enum_TipoOperazioneDB.Cancellazione

                                                    'Se la colonna Tipo_Associazione è uguale a 2 sono nel caso di conferimento collegato a scarico creato automaticamente
                                                    'su azienda agricola a seguito di raccolta, quindi posso cancellare lo scarico senza chiedere conferma. Questa modalità
                                                    'è usata nel caso di auto-conferimento
                                                    If dr_rif("Tipo_Associazione") <> 2 Then
                                                        'Richiesta cancellazione ddt reso allegato ad accettazione
                                                        messaggio &= ControllaOperazione_GeneraMsg(messaggio, dr_rif, Gias.Attenzione & " " & Gias.Il & " ***replace2*** " & AgronicaCoreModelloRes.LegatoOperazione_ & "<BR>" &
                                                                                                    dr_rif.Item("Des_Lib_Risultato") & " " & Gias.del & " " & CDate(dr_rif.Item("Validita_Inizio_Risultato")).ToShortDateString & "." & "<BR>" &
                                                                                                   AgronicaCoreModelloRes.ConfermaCancellaDueDocumenti)
                                                    End If

                                                    InserisciOperazioniEdit(matrice_delete, objParametriAgenda.Tipo_Operazione, dr_rif)

                                            End Select

                                    End Select

                                Case LAVCOD_ORDINE_VENDITA, LAVCOD_ORDINE_ACQUISTO

                                    Select Case objParametriAgenda.Tipo_Operazione

                                        Case enum_TipoOperazioneDB.Modifica

                                            ''Richiesta modifica ordine allegato a documento
                                            'messaggio = ControllaOperazione_GeneraMsg(dr_rif, Gias.Attenzione & " " & Gias.Il & " ***replace2*** " & AgronicaCoreModelloRes.LegatoOperazione_ & "<BR>" &
                                            '                                            dr_rif.Item("Des_Lib_Risultato") & " " & Gias.Del & " " & CDate(dr_rif.Item("Validita_Inizio_Risultato")).ToShortDateString & "." & "<BR>" &
                                            '                                           "")

                                        Case enum_TipoOperazioneDB.Cancellazione

                                            'Do nothing

                                    End Select

                                Case LAVCOD_VISITA

                                    msg = ControllaOperazione_GeneraMsg("", dr_rif, Gias.AttenzioneOperazioneGenerataDaVisitaGestireDirettamenteVisita)

                                    Throw New GiasException(ReplaceMessaggio(msg, Id_Mov_Det, strMessaggio_Info))

                                Case LAVCOD_RACCOLTA

                                    Select Case dr_rif.Item("Lav_Cod")

                                        Case LAVCOD_ACCETTAZIONE_DIVERSI, LAVCOD_DISTINTA_CARICO_ACCETTAZIONE, LAVCOD_AUTO_DDT_EMESSO, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE

                                            'Conferimento legato a raccolta
                                            'Se la raccolta pre-esiste al conferimento, allora questa non verrà cancellata in fase di modifica/cancellazione del conferimento

                                            If dr_rif("Tipo_Associazione") = 0 Then
                                                'Raccolta creata a partire da conferimento

                                                If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Cancellazione AndAlso Bypass_Delete_Exceptions = False Then
                                                    'Sono nell'effettiva cancellazione di una riga di conferimento collegata a raccolta automatica:
                                                    'verifico se in quest'ultima ci sono dei costi collegati e nel caso, impedisco la cancellazione

                                                    Dim dtRifRaccolta As DataTable = New AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R().Recupera_DT_Rif_Unificato(
                                                                                                                                                           dr_rif.Item("Piva_Risultato"),
                                                                                                                                                           dr_rif.Item("Sa_Cod_Risultato"),
                                                                                                                                                           dr_rif.Item("Id_Agenda_Risultato"),
                                                                                                                                                           0, 0, 0, "", objParametri_Server)

                                                    Dim rifRaccoltaCosti = dtRifRaccolta.AsEnumerable().Where(Function(drRifRaccolta) drRifRaccolta.Item("Lav_Cod_Risultato") = LAVCOD_COSTI_CDG).ToList()

                                                    If rifRaccoltaCosti.Count > 0 Then

                                                        'msg = ControllaOperazione_GeneraMsg("", dr_rif, Gias.Attenzione & " Il dettaglio del conferimento che si desidera eliminare è associato alla raccolta <BR>" &
                                                        '                                        dr_rif.Item("Des_Lib_Risultato") & " " & Gias.Del & " " & CDate(dr_rif.Item("Validita_Inizio_Risultato")).ToShortDateString &
                                                        '                                        " la quale a sua volta è associata all'operazione dei costi <BR>" & rifRaccoltaCosti.First().Item("Des_Lib_Risultato") & " ." & "<BR>" &
                                                        '                                        "La cancellazione è consentita solo previa eliminazione dell'operazione dei costi.")
                                                        msg = ControllaOperazione_GeneraMsg("", dr_rif, String.Format(AgronicaCoreModelloRes.EliminareConferimentoRaccoltaCosti,
                                                                      dr_rif.Item("Des_Lib_Risultato"),
                                                                      CDate(dr_rif.Item("Validita_Inizio_Risultato")).ToShortDateString,
                                                                      rifRaccoltaCosti.First().Item("Des_Lib_Risultato"))
                                                        )

                                                        Throw New GiasException(ReplaceMessaggio(msg, Id_Mov_Det, strMessaggio_Info))
                                                    End If

                                                End If

                                                'La raccolta automatica può essere cancellata, se il tipo_operazione non è cancellazione il chiamante non userà matrice_delete
                                                InserisciOperazioniEdit(matrice_delete, objParametriAgenda.Tipo_Operazione, dr_rif)

                                            End If

                                        Case Else

                                            Select Case objParametriAgenda.Tipo_Operazione

                                                Case enum_TipoOperazioneDB.Modifica

                                                    messaggio &= ControllaOperazione_GeneraMsg(messaggio, dr_rif, Gias.Attenzione & " " & Gias.Il & " ***replace2*** " & AgronicaCoreModelloRes.DaModificareAssociatoBollaAccettazione & "<BR>" &
                                                                                            dr_rif.Item("Des_Lib_Risultato") & " " & Gias.del & " " & CDate(dr_rif.Item("Validita_Inizio_Risultato")).ToShortDateString & "." & "<BR>")

                                                Case enum_TipoOperazioneDB.Cancellazione

                                                    'Movimento Non CANCELLABILE (carico/scarico da bolla di conferimento)

                                                    msg = ControllaOperazione_GeneraMsg("", dr_rif, Gias.Attenzione & " " & Gias.Il & " ***replace2*** " & AgronicaCoreModelloRes.DaEliminareareAssociatoBollaAccettazione & "<BR>" &
                                                                                            dr_rif.Item("Des_Lib_Risultato") & " " & Gias.del & " " & CDate(dr_rif.Item("Validita_Inizio_Risultato")).ToShortDateString & "." & "<BR>" &
                                                                                            Gias.La & " ***replace*** " & Gias.del & " ***replace2*** " & AgronicaCoreModelloRes.PertantoConsentitaSoloPrevia & " ***replace*** " & Gias.del & " ***replace3***.")

                                                    Throw New GiasException(ReplaceMessaggio(msg, Id_Mov_Det, strMessaggio_Info))

                                            End Select

                                    End Select

                                Case LAVCOD_ACCETTAZIONE_DIVERSI, LAVCOD_DISTINTA_CARICO_ACCETTAZIONE, LAVCOD_AUTO_DDT_EMESSO, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE

                                    Select Case dr_rif.Item("Cau_Mov_Risultato")

                                        Case CAU_CARICO, CAU_SCARICO

                                            Select Case objParametriAgenda.Tipo_Operazione

                                                Case enum_TipoOperazioneDB.Modifica

                                                    'Se la colonna Tipo_Associazione è uguale a 2 sono nel caso di bolla emessa creata automaticamente su azienda agricola
                                                    'da parte di un conferimento, quindi demando la gestione dei permessi sulla bolla alla pagina di modifica
                                                    If dr_rif("Tipo_Associazione") <> 2 Then

                                                        messaggio &= ControllaOperazione_GeneraMsg(messaggio, dr_rif, Gias.Attenzione & " " & Gias.Il & " ***replace2*** " & AgronicaCoreModelloRes.DaModificareAssociatoBollaAccettazione & "<BR>" &
                                                                                            dr_rif.Item("Des_Lib_Risultato") & " " & Gias.del & " " & CDate(dr_rif.Item("Validita_Inizio_Risultato")).ToShortDateString & "." & "<BR>" &
                                                                                            "")
                                                    End If

                                                Case enum_TipoOperazioneDB.Cancellazione

                                                    'Movimento Non CANCELLABILE (carico/scarico da bolla di conferimento)

                                                    msg = ControllaOperazione_GeneraMsg("", dr_rif, Gias.Attenzione & " " & Gias.Il & " ***replace2*** " & AgronicaCoreModelloRes.DaEliminareareAssociatoBollaAccettazione & "<BR>" &
                                                                                            dr_rif.Item("Des_Lib_Risultato") & " " & Gias.del & " " & CDate(dr_rif.Item("Validita_Inizio_Risultato")).ToShortDateString & "." & "<BR>" &
                                                                                            Gias.La & " ***replace*** " & Gias.del & " ***replace2*** " & AgronicaCoreModelloRes.PertantoConsentitaSoloPrevia & " ***replace*** " & Gias.del & " ***replace3***.")

                                                    Throw New GiasException(ReplaceMessaggio(msg, Id_Mov_Det, strMessaggio_Info))

                                            End Select

                                            'Case CAU_CONFERIMENTO, CAU_CONFERIMENTO_DIVERSI
                                            '    Select Case objParametriAgenda.Tipo_Operazione
                                            '        Case enum_TipoOperazioneDB.Modifica
                                            '            messaggio = ControllaOperazione_GeneraMsg(dr_rif, Gias.Attenzione & " " & Gias.Il & " ***replace2*** che si desidera modificare è associato alla bolla di conferimento: " & "<BR>" &
                                            '                                                                                        dr_rif.Item("Des_Lib_Risultato") & " " & Gias.Del & " " & CDate(dr_rif.Item("Validita_Inizio_Risultato")).ToShortDateString & "." & "<BR>" &
                                            '                                                                                        "")
                                            '        Case enum_TipoOperazioneDB.Cancellazione
                                            '            'Movimento Non CANCELLABILE (carico/scarico da bolla di conferimento)
                                            '            msg = ControllaOperazione_GeneraMsg(dr_rif, Gias.Attenzione & " " & Gias.Il & " ***replace2*** che si desidera eliminare è associato alla bolla di conferimento: " & "<BR>" &
                                            '                                                                                        dr_rif.Item("Des_Lib_Risultato") & " " & Gias.Del & " " & CDate(dr_rif.Item("Validita_Inizio_Risultato")).ToShortDateString & "." & "<BR>" &
                                            '                                                                                        Gias.La & " ***replace*** " & Gias.Del & " ***replace2*** " & AgronicaCoreModelloRes.PertantoConsentitaSoloPrevia & " ***replace*** " & Gias.Del & " ***replace3***.")
                                            '            '"Cancellando il movimento, l'associazione 'Rilievo Produzione e Data raccolta - Conferimento' non verrà più riconosciuta."
                                            '            Throw New Exception(ReplaceMessaggio(msg, Id_Mov_Det, strMessaggio_Info))
                                            '    End Select

                                        Case CAU_REGISTRAZIONI

                                            Select Case dr_rif.Item("Lav_Cod_Risultato")

                                                Case LAVCOD_NOTA_ACCREDITO_RICEVUTA, LAVCOD_NOTA_ACCREDITO_EMESSA

                                                    Select Case objParametriAgenda.Tipo_Operazione

                                                        Case enum_TipoOperazioneDB.Modifica

                                                            messaggio &= ControllaOperazione_GeneraMsg(messaggio, dr_rif, Gias.Attenzione & " " & AgronicaCoreModelloRes.FatturaAllegataNotaAccredito & "<BR>" &
                                                                                                        dr_rif.Item("Des_Lib_Risultato") & " " & Gias.del & " " & CDate(dr_rif.Item("Validita_Inizio_Risultato")).ToShortDateString & "." & "<BR>")

                                                            Modalita_Protetta = 1

                                                        Case enum_TipoOperazioneDB.Cancellazione

                                                            msg = ControllaOperazione_GeneraMsg("", dr_rif, Gias.Attenzione & " " & "<BR>" &
                                                                                                       dr_rif.Item("Des_Lib_Risultato") & " " & Gias.del & " " & CDate(dr_rif.Item("Validita_Inizio_Risultato")).ToShortDateString & "." & "<BR>" &
                                                                                                       AgronicaCoreModelloRes.CancellareFatturaNotaAccredito)

                                                            Throw New GiasException(ReplaceMessaggio(msg, Id_Mov_Det, strMessaggio_Info))

                                                    End Select

                                            End Select

                                        Case CAU_MUNGITURA 'da gestire

                                            Throw New Exception("CAU_MUNGITURA da gestire")

                                        Case CAU_MACELLAZIONE 'da gestire

                                            Throw New Exception("CAU_MACELLAZIONE da gestire")

                                        Case Else

                                            'Carico/Scarico Tra Imprese GIAS

                                            InserisciOperazioniEdit(matrice_delete, objParametriAgenda.Tipo_Operazione, dr_rif)

                                            'Throw New Exception(dr_rif.Item("Cau_Mov_Risultato") & " da gestire")

                                    End Select

                            End Select

                        End If

                    End If

                Next

            End If


            '================================================================================
            ' Controlli: Azienda in verifica + Sportello
            '================================================================================
            Select Case objParametriAgenda.Tipo_Operazione

                Case enum_TipoOperazioneDB.Cancellazione, enum_TipoOperazioneDB.Modifica

                    Dim dataMin As Date = AGRODATAINIZIO
                    Dim dataMax As Date = AGRODATAFINE

                    Dim SportelloAperto As Boolean = True
                    Dim Servizio_cod As Integer

                    'Select Case objParametriAgenda.Lav_Cod
                    '    Case LAVCOD_DISTRIBUZIONE_CONCIME, LAVCOD_DISTRIBUZIONE_AMMENDANTI, LAVCOD_CONCIMAZIONE_FOGLIARE, LAVCOD_FERTIRRIGAZIONE, LAVCOD_FERTILIZZAZIONI_DICHIARAZIONE_NON_UTILIZZO
                    '        Servizio_cod = enum_Servizi.PUA
                    '    Case Else
                    '        Servizio_cod = enum_Servizi.Quaderno_Campagna_Caa
                    'End Select

                    Servizio_cod = enum_Servizi.Quaderno_Campagna_Caa

                    Dim objPraticheBIZ As New AgronicaCoreProfilazioneBIZ.Pratiche_R

                    'Azienda in Verifica

                    Dim AziendaInVerifica As Boolean = False
                    Dim dataMinxVerifica = AGRODATAINIZIO
                    Dim dataMaxxVerifica = AGRODATAFINE
                    objPraticheBIZ.Limitazione_Data_Per_VerificaInCorso(objParametriAgenda.Piva,
                                                               Servizio_cod,
                                                               objParametriAgenda.Data,
                                                               AziendaInVerifica,
                                                               dataMinxVerifica,
                                                               dataMaxxVerifica,
                                                               objParametri_Server,
                                                               objParametri_Utenti)

                    objPraticheBIZ.VerificaInCorso_ChiamataSecondaria_SeNessunCambiamento(objParametriAgenda.Piva,
                                                                                        enum_Servizi.QuadernoCampagnaBio,
                                                                                        objParametriAgenda.Data,
                                                                                        False,
                                                                                        AGRODATAINIZIO,
                                                                                        AGRODATAFINE,
                                                                                        AziendaInVerifica,
                                                                                        dataMinxVerifica,
                                                                                        dataMaxxVerifica,
                                                                                        objParametri_Server,
                                                                                        objParametri_Utenti)

                    'Controllo Sportello

                    objPraticheBIZ.Data_Sportello_Da_Servizio(objParametriAgenda.Piva,
                                                               Servizio_cod,
                                                               objParametriAgenda.Data,
                                                               SportelloAperto,
                                                               dataMin,
                                                               dataMax,
                                                               objParametri_Server,
                                                               objParametri_Utenti)

                    objPraticheBIZ.Sportello_ChiamataSecondaria_SeNessunCambiamento(objParametriAgenda.Piva,
                                                                                        enum_Servizi.QuadernoCampagnaBio,
                                                                                        objParametriAgenda.Data,
                                                                                        True,
                                                                                        AGRODATAINIZIO,
                                                                                        AGRODATAFINE,
                                                                                        SportelloAperto,
                                                                                        dataMin,
                                                                                        dataMax,
                                                                                        objParametri_Server,
                                                                                        objParametri_Utenti)

                    If dataMinxVerifica > dataMin Then
                        dataMin = dataMinxVerifica
                        SportelloAperto = True
                        AziendaInVerifica = True
                    End If

                    If Not SportelloAperto OrElse (objParametriAgenda.Data < dataMin OrElse objParametriAgenda.Data > dataMax) AndAlso AziendaInVerifica = False Then

                        Select Case objParametriAgenda.Tipo_Operazione

                            Case enum_TipoOperazioneDB.Cancellazione
                                Throw New GiasException(AgronicaCoreModelloRes.SportelloChiusoImpossibileEliminareOperazione)

                            Case enum_TipoOperazioneDB.Modifica
                                Throw New GiasException(AgronicaCoreModelloRes.SportelloChiusoImpossibileModificareOperazione)

                        End Select

                    ElseIf AziendaInVerifica Then

                        Select Case objParametriAgenda.Tipo_Operazione

                            Case enum_TipoOperazioneDB.Cancellazione
                                Throw New GiasException(AgronicaCoreModelloRes.AziendaInVerificaImpossibileEliminareOperazione)

                            Case enum_TipoOperazioneDB.Modifica
                                Throw New GiasException(AgronicaCoreModelloRes.AziendaInVerificaImpossibileModificareOperazione)

                        End Select

                    End If

            End Select


            '============================================================================================================
            'Controllo Invio SDI
            '------------------------------------------------------------------------------------------------------------
            Dim obj_SDI_R As New AgronicaCoreEFatturaDAL.SDI_Log_R(objParametri_Server)
            Dim DT_SDI As New DataTable
            DT_SDI = obj_SDI_R.LeggixCancellazione(objParametri_Server, objParametriAgenda.Id_Agenda)

            If DT_SDI.Rows.Count > 0 Then

                Select Case objParametriAgenda.Tipo_Operazione

                    Case enum_TipoOperazioneDB.Modifica

                        'Richiesta modifica ordine allegato a documento
                        messaggio &= ControllaOperazione_GeneraMsg(messaggio, objParametri_Server, objParametriAgenda, Gias.Attenzione & " " & Gias.Il & " ***replace2*** è stato inviato allo Sdi." & "<BR>")
                    Case enum_TipoOperazioneDB.Cancellazione

                        'Richiesta cancellazione fattura inviata a SDI
                        msg = Gias.Attenzione & " Non è possibile eliminare un documento già inviato allo Sdi."

                        Throw New GiasException(ReplaceMessaggio(msg, 0, strMessaggio_Info))

                End Select

            End If


            '================================================================================
            ' Controlli: Codice Raccoglitore
            '================================================================================
            Select Case objParametriAgenda.Tipo_Operazione

                Case enum_TipoOperazioneDB.Modifica

                Case enum_TipoOperazioneDB.Cancellazione

                    Dim obj_Agenda_R As New AgronicaCoreContabDAL.Agenda_R
                    Dim DT_Agenda = obj_Agenda_R.Leggi(objParametriAgenda.Piva, 0, objParametriAgenda.Id_Agenda, 0,
                                                       enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

                    If DT_Agenda.Rows.Count > 0 AndAlso Not IsDBNull(DT_Agenda.Rows(0).Item("Raccoglitore_Cod")) AndAlso DT_Agenda.Rows(0).Item("Raccoglitore_Cod") <> 0 Then
                        messaggio &= ControllaOperazione_GeneraMsg(messaggio, objParametri_Server, objParametriAgenda,
                                                                   Gias.OperazioneRegistrataInsiemeAltreContinuandoVerrannoTutteCancellate,
                                                                   list_msgOperazioniCollegate:=list_msgOperazioniCollegate)
                        'Estraggo le altre operazioni dello stesso stesso raccoglitore
                        Dim DT_Agenda_Raccoglitori = obj_Agenda_R.Leggi(objParametriAgenda.Piva, 0, 0, 0,
                                                                        enumSelezioneVariabile.Selezione_TabellaCompleta, " (Raccoglitore_Cod = " & DT_Agenda.Rows(0).Item("Raccoglitore_Cod") & " AND Id_Agenda <> " & DT_Agenda.Rows(0).Item("Id_Agenda") & ") ", "", objParametri_Server)

                        For Each operazione In DT_Agenda_Raccoglitori.Rows
                            If list_msgOperazioniCollegate IsNot Nothing Then
                                list_msgOperazioniCollegate.Add("- <b>" & operazione.Item("des_lib") & "</b> " & Gias.del & " " & operazione.Item("validita_inizio"))
                            End If
                        Next

                        'Cancello anche i reinneschi creati se cancello l'installazione trappole
                        If Not IsNothing(list_msgOperazioniCollegate) AndAlso objParametriAgenda.Lav_Cod = LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA Then

                            Dim list_Id_Agenda_Installazione As New List(Of Integer) From {
                                objParametriAgenda.Id_Agenda
                            }

                            If Not IsNothing(DT_Agenda_Raccoglitori) AndAlso DT_Agenda_Raccoglitori.Rows.Count > 0 Then

                                For Each dr As DataRow In DT_Agenda_Raccoglitori.Rows
                                    list_Id_Agenda_Installazione.Add(dr.Item("Id_Agenda"))
                                Next
                            End If


                            If Not IsNothing(list_Id_Agenda_Installazione) AndAlso list_Id_Agenda_Installazione.Count > 0 Then

                                Dim list_Id_Agenda_Reinneschi As New List(Of String)

                                Dim xFiltroAggiuntivo As String = " Mov_Dettagli_Riferimenti.Id_Agenda_Rif IN (" & String.Join(",", list_Id_Agenda_Installazione) & ") AND Mov_Dettagli_Riferimenti.Lav_Cod = " & LAVCOD_REINNESCO_TRAPPOLE

                                Dim objMovDetRif_R As New AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R
                                Dim DT_Reinneschi As DataTable = objMovDetRif_R.LeggixChiave(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, 0, 0, 0, 0,
                                                                                             0, 0, 0, "", "", objParametri_Server, xFiltroAggiuntivo)


                                If Not IsNothing(DT_Reinneschi) AndAlso DT_Reinneschi.Rows.Count > 0 Then
                                    For Each dr_reinnesco As DataRow In DT_Reinneschi.Rows

                                        Dim Id_Agenda As Integer = dr_reinnesco.Item("Id_Agenda")

                                        If Not list_Id_Agenda_Reinneschi.Contains(Id_Agenda) Then

                                            list_msgOperazioniCollegate.Add("- <b>" & dr_reinnesco.Item("Des_Lib") & "</b> " & Gias.del & " " & dr_reinnesco.Item("Validita_Inizio_Agenda"))

                                            list_Id_Agenda_Reinneschi.Add(Id_Agenda)
                                        End If
                                    Next
                                End If
                            End If
                        End If
                    End If
            End Select


            '==============================================================================================================================================
            'Controllo Conferimento Beni/Preparazioni. Verifico se i beni caricati non sono stati in qualche altro modo scaricati
            '----------------------------------------------------------------------------------------------------------------------------------------------
            'Rilievo Produzione                               125
            'Macellazione                                     3004
            'Mungitura a Secchio                              3010
            'Mungitura a Gruppi                               3011
            'Mungitura nella Sala Latte                       3012
            'Preparazioni                                     5000

            Select Case objParametriAgenda.Lav_Cod

                Case LAVCOD_RACCOLTA, LAVCOD_MACELLAZIONE_ANIMALI, LAVCOD_MUNGITURA_SECCHIO_POSTA, LAVCOD_MUNGITURA_GRUPPI_POSTA, LAVCOD_MUNGITURA_SALA_LATTE, LAVCOD_PREPARAZIONE

                    '=================================================================================================================================================
                    'Controllo Trasformazioni Bloccate
                    '-------------------------------------------------------------------------------------------------------------------------------------------------

                    If CLng(objParametriAgenda.Lav_Cod) = LAVCOD_PREPARAZIONE Then

                        Dim Dt_Preparazione As DataTable = New AgronicaCoreContabDAL.Linee_Preparazioni_R().Leggi_Codice_Generazione(objParametriAgenda.Piva, objParametriAgenda.Id_Agenda, objParametri_Server)

                        If Dt_Preparazione.Rows.Count > 0 Then

                            Select Case Dt_Preparazione(0).Item("Codice_Generazione")

                                Case -180 'Assemblamento Lotto

                                    msg = "La trasformazione è generata automaticamente e non può essere ***replace*** dagli archivi."

                                    Throw New GiasException(ReplaceMessaggio(msg, Id_Mov_Det, strMessaggio_Info))

                                Case Else

                            End Select

                        End If

                    End If
                    '=================================================================================================================================================

                    Filtro_Aggiuntivo = " And Movimenti.Cau_Mov In ('" & CAU_CARICO & "','" & CAU_ACCETTAZIONE_BENI_DA_DIVERSI & "')"

                    Dim Dt_Mov As DataTable = New AgronicaCoreContabDAL.Movimenti_Dettagli_R().MovimentiDettagli_Leggi(objParametriAgenda.Piva, 0,
                                                            objParametriAgenda.Id_Agenda, 0, 0, 0, 0, 0, 0, 0, LOTTO_NONDEFINITO, 0, 0, 0, "", False, Filtro_Aggiuntivo, "", objParametri_Server)

                    For Each dr_mov As DataRow In Dt_Mov.Rows

                        'Costruzione Filtro
                        strFiltro_Dettaglio &= "(Movimenti_Dettagli.Elem_Cod     = " & CInt(dr_mov.Item("elem_cod")) & " AND " &
                                                "Movimenti_Dettagli.Pro_Cod      = " & CInt(dr_mov.Item("pro_cod")) & " AND " &
                                                "Movimenti_Dettagli.Mat_Cod      = " & CInt(dr_mov.Item("mat_cod")) & " AND " &
                                                "Movimenti_Dettagli.Udm_Cod      = " & CInt(dr_mov.Item("udm_cod")) & " AND " &
                                                "Movimenti_Dettagli.Cod_Progetto = " & CInt(dr_mov.Item("cod_progetto")) & " AND " &
                                                "Movimenti_Dettagli.Fase_Cod     = " & CInt(dr_mov.Item("fase_cod")) & " AND " &
                                                "Movimenti_Dettagli.Cal_Cod      = " & CInt(dr_mov.Item("cal_cod")) & " AND " &
                                                "Movimenti_Dettagli.Lotto        = '" & UtilityProvider.Agro_SQL_SaveText(CStr(dr_mov.Item("lotto"))) & "') OR "

                    Next

                    Dt_Mov = Nothing

                    If strFiltro_Dettaglio <> "" Then

                        strFiltro_Dettaglio = " AND (" & Left(strFiltro_Dettaglio, strFiltro_Dettaglio.Length - 3) & ")" 'Elimino l'ultimo OR

                        strFiltro_Dettaglio &= " AND Data_Movimento >= " & UtilityProvider.Agro_SQL_SaveDate(objParametriAgenda.Data) & " "

                        Flag_ProdottoScaricato = New AgronicaCoreContabDAL.Giacenze_R().Verifica_ScarichiProdotto(Des_Lib_Scarico, Data_Movimento_Scarico, strFiltro_Dettaglio, objParametri_Server)

                    End If

                    If Flag_ProdottoScaricato = True Then

                        If PermettoEliminazioneRaccoltaPercheCura AndAlso Not IsNothing(objParametriAgenda.Tipo_Operazione) AndAlso objParametriAgenda.Tipo_Operazione = "3" Then
                            messaggio2 = " Attenzione, l'operazione di raccolta è seguita da una cura, se si cancella l'operazione il prodotto raccolto sarà cancellato anche nel centro di cura ed è opportuno eliminare anche l'operazione di cura collegata se non lo si è già fatto.  Proseguire con l'eliminazione? L'operazione non è annullabile. "
                            Return True
                        End If

                        msg = ControllaOperazione_GeneraMsg("", objParametri_Server, objParametriAgenda, Gias.Attenzione & " " & AgronicaCoreModelloRes.ProdottiCaricatiOperazione & " " & objParametriAgenda.Lav_Des &
                                " " & AgronicaCoreModelloRes.RisultanoEssereScaricatiOperazione & " " & Des_Lib_Scarico & " " & Gias.del & " " & Data_Movimento_Scarico & "." & "<BR>" &
                                AgronicaCoreModelloRes.AlFineDiGarantireLaRintracciabilita & " " & Gias.La & " ***replace*** " & AgronicaCoreModelloRes.ConsentitaSoloPreviaCancellazioneScarico)

                        Throw New GiasException(ReplaceMessaggio(msg, Id_Mov_Det, strMessaggio_Info))

                    End If

                Case LAVCOD_BOLLA_RICEVUTA, LAVCOD_ACCETTAZIONE_DIVERSI, LAVCOD_AUTO_DDT_EMESSO, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE

                    Select Case True

                        '--- GIAS Cantine

                        Case Modulo_Generazione = enum_Omni_Modulo_Generazione.Cantine

                            'In caso di accettazione GIAS cantine occorre che il prodotto da vinificare non venga scaricato con altri documenti (il cal_cod viene ricreato e sarà diverso da quello di scarico)

                            Filtro_Aggiuntivo = " And Jolly_Int = 0"

                            Dim Dt_Mov As DataTable = New AgronicaCoreContabDAL.Movimenti_Dettagli_R().MovimentiDettagli_Leggi(objParametriAgenda.Piva, 0,
                                                            objParametriAgenda.Id_Agenda, 0, Id_Mov_Det, 0, 0, 0, 0, 0, LOTTO_NONDEFINITO, 0, 0, 0, "", False, Filtro_Aggiuntivo, "", objParametri_Server)


                            If Dt_Mov.Rows.Count > 0 Then

                                For Each dr_mov As DataRow In Dt_Mov.Rows


                                    'Controllo che il prodotto con lo stesso lotto e cal_cod non sia stato scaricato.
                                    'Altrimenti non si aggiorna la giacenza (cal_cod ricalcolato) e quindi il calcolo della vinificazione è errato

                                    Filtro_Aggiuntivo = "And Agenda.Lav_Cod <> 5000 And Movimenti_Dettagli.Jolly_Int = 0"

                                    Dim Dt_Mov_Scarico As DataTable = New AgronicaCoreContabDAL.Movimenti_Dettagli_R().MovimentiDettagli_Leggi(objParametriAgenda.Piva, 0,
                                                            0, 0, 0, dr_mov.Item("Elem_Cod"), 0, 0, 0, 0, dr_mov.Item("Lotto"), dr_mov.Item("Cal_Cod"), 0, 0, CAU_SCARICO, False, Filtro_Aggiuntivo, "", objParametri_Server)

                                    If Dt_Mov_Scarico.Rows.Count > 0 Then

                                        Select Case objParametriAgenda.Tipo_Operazione

                                            Case enum_TipoOperazioneDB.Modifica

                                                msg = ControllaOperazione_GeneraMsg("", objParametri_Server, objParametriAgenda, Gias.Attenzione & " Il lotto di vinificazione " & Dt_Mov_Scarico(0).Item("Lotto") &
                                                                              " " & AgronicaCoreModelloRes.RisultaEssereScaricatoOperazione & " " & Dt_Mov_Scarico(0).Item("Des_Lib") & " del" & Dt_Mov_Scarico(0).Item("Data_Movimento") & "." & "<BR>" &
                                                                              Gias.La & " ***replace*** " & Gias.del & " ***replace2*** " & AgronicaCoreModelloRes.ConsentitaSoloPreviaCancellazioneScarico)

                                                Throw New GiasException(ReplaceMessaggio(msg, Id_Mov_Det, strMessaggio_Info))

                                            Case enum_TipoOperazioneDB.Cancellazione

                                        End Select

                                    End If

                                Next

                            End If

                        '--- GIAS Fresh & Food

                            'Dal 23/11/2020 in accettazione possono entrare anche trasformati animali e posso avere
                            'nello stesso documento sia trasformati animali che vegetali; quindi in agenda non viene
                            'più impostato il modulo anagrafe in testata tranne che non sia conferimento pomodoro.
                            'Invece del modulo viene verificato che si tratti di una accettazione.

                        Case UtilityHelper.IsAccettazione(objParametriAgenda.Lav_Cod)

                            'Filtro Cau_Mov

                            Filtro_Aggiuntivo = " And Movimenti.Cau_Mov In ('" & CAU_CARICO & "','" & CAU_ACCETTAZIONE_BENI_DA_DIVERSI & "') And Movimenti_Dettagli.Elem_Cod = 210"

                            Dim Dt_Mov As DataTable = New AgronicaCoreContabDAL.Movimenti_Dettagli_R().MovimentiDettagli_Leggi(objParametriAgenda.Piva, 0,
                                                                objParametriAgenda.Id_Agenda, 0, Id_Mov_Det, 0, 0, 0, 0, 0, LOTTO_NONDEFINITO, 0, 0, 0, "", False, Filtro_Aggiuntivo, "", objParametri_Server)

                            If Dt_Mov.Rows.Count > 0 Then

                                strFiltro_Dettaglio = " (Agenda.Id_Agenda <> " & CLng(objParametriAgenda.Id_Agenda) & ") And ("

                                For Each dr_mov As DataRow In Dt_Mov.Rows

                                    'Costruzione Filtro

                                    strFiltro_Dettaglio &= "(Movimenti_Dettagli.Elem_Cod     = " & CInt(dr_mov.Item("elem_cod")) & " AND " &
                                                    "Movimenti_Dettagli.Pro_Cod      = " & CInt(dr_mov.Item("pro_cod")) & " AND " &
                                                    "Movimenti_Dettagli.Mat_Cod      = " & CInt(dr_mov.Item("mat_cod")) & " AND " &
                                                    "Movimenti_Dettagli.Udm_Cod      = " & CInt(dr_mov.Item("udm_cod")) & " AND " &
                                                    "Movimenti_Dettagli.Cod_Progetto = " & CInt(dr_mov.Item("cod_progetto")) & " AND " &
                                                    "Movimenti_Dettagli.Fase_Cod     = " & CInt(dr_mov.Item("fase_cod")) & " AND " &
                                                    "Movimenti_Dettagli.Cal_Cod      = " & CInt(dr_mov.Item("cal_cod")) & " AND " &
                                                    "Movimenti_Dettagli.Lotto        = '" & UtilityProvider.Agro_SQL_SaveText(CStr(dr_mov.Item("lotto"))) & "') OR "

                                Next

                                strFiltro_Dettaglio = " And (" & Left(strFiltro_Dettaglio, strFiltro_Dettaglio.Length - 3) & ")) And Movimenti_Dettagli.Cal_Cod <> 0 " ' And Data_Movimento >= " & Agro_SQL_SaveDate(objParametriAgenda.Data) 'Elimino l'ultimo OR

                                If bypassConferimentoBollaEmessa = True Then
                                    strFiltro_Dettaglio &= String.Format(" AND Agenda.Lav_Cod NOT IN({0}, {1}) ", LAVCOD_BOLLA_EMESSA, LAVCOD_SCARICO)
                                End If

                                Dim Dt_Mov_Scarico As DataTable = New AgronicaCoreContabDAL.Movimenti_Dettagli_R().MovimentiDettagli_Leggi(objParametriAgenda.Piva, 0,
                                                                 0, 0, 0, 0, 0, 0, 0, 0, LOTTO_NONDEFINITO, 0, 0, 0, CAU_SCARICO, False, strFiltro_Dettaglio, "", objParametri_Server)

                                If Dt_Mov_Scarico.Rows.Count > 0 Then

                                    Select Case objParametriAgenda.Tipo_Operazione

                                        Case enum_TipoOperazioneDB.Modifica

                                            messaggio &= ControllaOperazione_GeneraMsg(messaggio, objParametri_Server, objParametriAgenda, Gias.Attenzione & " " & AgronicaCoreModelloRes.ProdottiCampionatiOperazione & " " & objParametriAgenda.Des_lib &
                                                                                    " " & AgronicaCoreModelloRes.RisultanoEssereScaricatiOperazione & " " & Dt_Mov_Scarico(0).Item("Des_Lib") & " " & Gias.del & " " & Dt_Mov_Scarico(0).Item("Data_Movimento") & "." & "<BR>")

                                        Case enum_TipoOperazioneDB.Cancellazione

                                            If Not Bypass_Delete_Exceptions Then

                                                msg = ControllaOperazione_GeneraMsg("", objParametri_Server, objParametriAgenda, Gias.Attenzione & " " & AgronicaCoreModelloRes.ProdottiCampionatiOperazione & " " & objParametriAgenda.Des_lib &
                                                                                    " " & AgronicaCoreModelloRes.RisultanoEssereScaricatiOperazione & " " & Dt_Mov_Scarico(0).Item("Des_Lib") & " " & Gias.del & " " & Dt_Mov_Scarico(0).Item("Data_Movimento") & "." & "<BR>" &
                                                                                    AgronicaCoreModelloRes.AlFineDiGarantireLaRintracciabilita & " " & Gias.La & " ***replace*** " & AgronicaCoreModelloRes.PertantoConsentitaSoloPrevia & " ***replace*** " & AgronicaCoreModelloRes.DelMovimentoDiScarico)

                                                Throw New GiasException(ReplaceMessaggio(msg, Id_Mov_Det, strMessaggio_Info))

                                            End If

                                    End Select

                                End If

                            End If

                            'Lettura Blocchi Campionamento Conferito

                            Dim Dt_Mov_Camp As DataTable = New AgronicaCoreContabDAL.FF_CampionamentoConferimento_R().LeggiBlocchiCampionamentoConferito(objParametri_Server.PivaSuperUser, objParametriAgenda.Piva, objParametriAgenda.Id_Agenda, objParametri_Server)

                            If Dt_Mov_Camp.Rows.Count > 0 Then

                                Select Case objParametriAgenda.Tipo_Operazione

                                    Case enum_TipoOperazioneDB.Modifica

                                        messaggio &= ControllaOperazione_GeneraMsg(messaggio, objParametri_Server, objParametriAgenda, Gias.Attenzione & " " & AgronicaCoreModelloRes.ProdottiCaricatiOperazione & " " & objParametriAgenda.Des_lib &
                                                                                " " & AgronicaCoreModelloRes.RisultanoEssereCampionati & "<BR>")

                                        Modalita_Protetta = 1

                                    Case enum_TipoOperazioneDB.Cancellazione

                                        If Not Bypass_Delete_Exceptions Then

                                            msg = ControllaOperazione_GeneraMsg("", objParametri_Server, objParametriAgenda, Gias.Attenzione & " " & AgronicaCoreModelloRes.ProdottiCaricatiOperazione & " " & objParametriAgenda.Des_lib &
                                                                                        " " & AgronicaCoreModelloRes.RisultanoEssereCampionati & "<BR>" &
                                                                                        AgronicaCoreModelloRes.AlFineDiGarantireLaRintracciabilita & " " & Gias.La & " ***replace*** " & AgronicaCoreModelloRes.PertantoConsentitaSoloPrevia & " ***replace*** " & AgronicaCoreModelloRes.DelleCampionature)

                                            Throw New GiasException(ReplaceMessaggio(msg, Id_Mov_Det, strMessaggio_Info))

                                        End If

                                End Select

                            End If

                            'Lettura Blocchi Campionamento Conferito Liquidato

                            Dim Dt_Mov_Liq As DataTable = New AgronicaCoreContabDAL.FF_LiquidazioneSoci_R().LeggiBlocchiLiquid_Mov_CampionamentoConferito(objParametri_Server.PivaSuperUser, objParametriAgenda.Piva, objParametriAgenda.Id_Agenda, objParametri_Server)

                            If Dt_Mov_Liq.Rows.Count > 0 Then

                                Select Case objParametriAgenda.Tipo_Operazione

                                    Case enum_TipoOperazioneDB.Modifica

                                        messaggio &= ControllaOperazione_GeneraMsg(messaggio, objParametri_Server, objParametriAgenda, Gias.Attenzione & " " & AgronicaCoreModelloRes.ProdottiCaricatiOperazione & " " & objParametriAgenda.Des_lib &
                                                                                " " & AgronicaCoreModelloRes.RisultanoEssereLiquidati & " " & "<b>" & Dt_Mov_Liq.Rows(0).Item("descrizione") & "</b>" & "<BR>")

                                        Modalita_Protetta = 1

                                    Case enum_TipoOperazioneDB.Cancellazione

                                        If Not Bypass_Delete_Exceptions Then

                                            msg = ControllaOperazione_GeneraMsg("", objParametri_Server, objParametriAgenda, Gias.Attenzione & " " & AgronicaCoreModelloRes.ProdottiCaricatiOperazione & " " & objParametriAgenda.Des_lib &
                                                                                    " " & AgronicaCoreModelloRes.RisultanoEssereLiquidati & " " & "<b>" & Dt_Mov_Liq.Rows(0).Item("descrizione") & "</b>" & "<BR>" &
                                                                                    AgronicaCoreModelloRes.AlFineDiGarantireLaRintracciabilita & " " & Gias.La & " ***replace*** " & AgronicaCoreModelloRes.PertantoConsentitaSoloPrevia & " ***replace*** " & AgronicaCoreModelloRes.DellaLiquidazione)

                                            Throw New GiasException(ReplaceMessaggio(msg, Id_Mov_Det, strMessaggio_Info))

                                        End If

                                End Select

                            End If

                            'Lettura Blocchi Campionamento Conferito Liquidato Variazioni
                            ' 18/12/2023 Scattolin - non necessario perchè già intercettate dala passo sopra "Lettura Blocchi Campionamento Conferito Liquidato"
                            ''''Dim Dt_Mov_Liq2 As DataTable = New AgronicaCoreContabDAL.FF_LiquidazioneSoci_R().LeggiBlocchiLiquid_Mov_FattVariaz_CampionamentoConferito(objParametri_Server.PivaSuperUser, objParametriAgenda.Piva, objParametriAgenda.Id_Agenda, objParametri_Server)

                            ''''If Dt_Mov_Liq2.Rows.Count > 0 Then

                            ''''    Select Case objParametriAgenda.Tipo_Operazione

                            ''''        Case enum_TipoOperazioneDB.Modifica

                            ''''            messaggio &= ControllaOperazione_GeneraMsg(messaggio, objParametri_Server, objParametriAgenda, Gias.Attenzione & " " & AgronicaCoreModelloRes.ProdottiCaricatiOperazione & " " & objParametriAgenda.Des_lib &
                            ''''                                                   " " & AgronicaCoreModelloRes.RisultanoEssereLiquidati & "<BR>")

                            ''''            Modalita_Protetta = 1

                            ''''        Case enum_TipoOperazioneDB.Cancellazione

                            ''''            If Not Bypass_Delete_Exceptions Then

                            ''''                msg = ControllaOperazione_GeneraMsg("", objParametri_Server, objParametriAgenda, Gias.Attenzione & " " & AgronicaCoreModelloRes.ProdottiCaricatiOperazione & " " & objParametriAgenda.Des_lib &
                            ''''                                                        " " & AgronicaCoreModelloRes.RisultanoEssereLiquidati & "<BR>" &
                            ''''                                                        AgronicaCoreModelloRes.AlFineDiGarantireLaRintracciabilita & " " & Gias.La & " ***replace*** " & AgronicaCoreModelloRes.PertantoConsentitaSoloPrevia & " ***replace*** " & AgronicaCoreModelloRes.DellaLiquidazione)

                            ''''                Throw New Exception(ReplaceMessaggio(msg, Id_Mov_Det, strMessaggio_Info))

                            ''''            End If

                            ''''    End Select

                            ''''End If

                            'Lettura Blocchi Campionamento Conferito Liquidato x Calibro
                            ' 18/12/2023 Scattolin - non necessario perchè già intercettate dala passo sopra "Lettura Blocchi Campionamento Conferito Liquidato"

                            ''''Dim Dt_Mov_Liq3 As DataTable = New AgronicaCoreContabDAL.FF_LiquidazioneSoci_R().Liquid_Mov_PerCalibro_CampionamentoConferito(objParametri_Server.PivaSuperUser, objParametriAgenda.Piva, objParametriAgenda.Id_Agenda, objParametri_Server)

                            ''''If Dt_Mov_Liq3.Rows.Count > 0 Then

                            ''''    Select Case objParametriAgenda.Tipo_Operazione

                            ''''        Case enum_TipoOperazioneDB.Modifica

                            ''''            messaggio &= ControllaOperazione_GeneraMsg(messaggio, objParametri_Server, objParametriAgenda, Gias.Attenzione & " " & AgronicaCoreModelloRes.ProdottiCaricatiOperazione & " " & objParametriAgenda.Des_lib &
                            ''''                                                   " " & AgronicaCoreModelloRes.RisultanoEssereLiquidati & "<BR>")

                            ''''            Modalita_Protetta = 1

                            ''''        Case enum_TipoOperazioneDB.Cancellazione

                            ''''            If Not Bypass_Delete_Exceptions Then

                            ''''                msg = ControllaOperazione_GeneraMsg("", objParametri_Server, objParametriAgenda, Gias.Attenzione & " " & AgronicaCoreModelloRes.ProdottiCaricatiOperazione & " " & objParametriAgenda.Des_lib &
                            ''''                                                        " " & AgronicaCoreModelloRes.RisultanoEssereLiquidati & "<BR>" &
                            ''''                                                        AgronicaCoreModelloRes.AlFineDiGarantireLaRintracciabilita & " " & Gias.La & " ***replace*** " & AgronicaCoreModelloRes.PertantoConsentitaSoloPrevia & " ***replace*** " & AgronicaCoreModelloRes.DellaCalibratura)

                            ''''                Throw New Exception(ReplaceMessaggio(msg, Id_Mov_Det, strMessaggio_Info))

                            ''''            End If

                            ''''    End Select

                            ''''End If

                    End Select

                Case Else

                    'Operazione NON PERICOLOSA
                    'Return True

            End Select


            '================================================================================
            ' Controlli: Operazioni Zootecniche
            '================================================================================
            If {LAVCOD_NASCITA_ANIMALI, LAVCOD_INCREMENTO_CONSISTENZE_ZOO,
                LAVCOD_ACQUISTO_ANIMALI, LAVCOD_SPOSTAMENTI_ZOO}.Contains(objParametriAgenda.Lav_Cod) AndAlso
                objParametriAgenda.Tipo_Operazione = "3" Then
                Dim objZooBiz As New AgronicaCoreAnagrafeBIZ.Zoo


                Dim listAnimaliMovimentati = objZooBiz.AnimaliMovimentati_Next_List(objParametriAgenda.Piva, objParametriAgenda.Id_Agenda, list_IdAgenda_DaCancellare, objParametri_Server)

                If listAnimaliMovimentati.Count > 0 Then
                    Throw New GiasException("I seguenti capi sono stati movimenti: " & String.Join(", ", listAnimaliMovimentati))
                End If

            End If



            If {LAVCOD_NASCITA_ANIMALI, LAVCOD_INCREMENTO_CONSISTENZE_ZOO, LAVCOD_DECREMENTO_CONSISTENZE_ZOO, LAVCOD_MORTE_ANIMALI, LAVCOD_MACELLAZIONE_ANIMALI, LAVCOD_CUREMEDICAMENTI_ANIMALI, LAVCOD_ACQUISTO_ANIMALI, LAVCOD_VENDITA_ANIMALI}.Contains(objParametriAgenda.Lav_Cod) Then


                Dim alert As String = ""
                alert = OperazioneZoo_Sincronizzata(objParametriAgenda.Piva, objParametriAgenda.Id_Agenda, objParametriAgenda.Lav_Cod,
                                                    objParametriAgenda, objParametri_Server)
                If alert <> "" Then
                    Modalita_Protetta = 1
                    messaggio &= alert

                    'InserisciOperazioniEdit(matrice_delete, objParametriAgenda.Tipo_Operazione, dr_rif)
                End If
            End If


            If messaggio <> "" Then
                messaggio = ReplaceMessaggio(messaggio, Id_Mov_Det, strMessaggio_Info)

                messaggio = Trim(messaggio) & NEWLINE & NEWLINE & Gias.SiDesideraProseguire

                If IgnoraAvvisoWarning Then
                    'proseguo 
                Else

                    Return False

                End If

            End If

            Return True

        End Function

        Private Function ReplaceMessaggio(ByVal Messaggio As String, ByVal Id_Mov_Det As Integer, ByVal strMessaggio_Info As String) As String

            Messaggio = Replace(Messaggio, "***replace***", strMessaggio_Info)

            Select Case Id_Mov_Det

                Case 0

                    Messaggio = Replace(Messaggio, "***replace2***", AgronicaCoreModelloRes.Documento)
                    Messaggio = Replace(Messaggio, "***replace3***", AgronicaCoreModelloRes.LaRegistrazioneAllegata)

                Case Else

                    Messaggio = Replace(Messaggio, "***replace2***", AgronicaCoreModelloRes.DettaglioRegistrazioneAllegata)
                    Messaggio = Replace(Messaggio, "***replace3***", AgronicaCoreModelloRes.DettaglioAllegato)

            End Select

            Return Messaggio





        End Function


        Private Sub InserisciOperazioniEdit(ByRef matrice_delete(,) As String, ByVal TipoOperazione As enum_TipoOperazioneDB, ByVal dr As DataRow)
            Dim bDuplicato As Boolean = False
            Dim i As Integer = 0

            Select Case TipoOperazione

                Case enum_TipoOperazioneDB.Cancellazione

                    'Controllo presenza in struttura
                    For i = 0 To UBound(matrice_delete, 2)

                        If matrice_delete(enum_Matrice_Delete.Piva, i) = dr.Item("Piva_Risultato") AndAlso
                           matrice_delete(enum_Matrice_Delete.Sa_Cod, i) = dr.Item("Sa_Cod_Risultato") AndAlso
                           matrice_delete(enum_Matrice_Delete.Id_Agenda, i) = dr.Item("Id_Agenda_Risultato") AndAlso
                           matrice_delete(enum_Matrice_Delete.Id_Mov, i) = dr.Item("Id_Mov_Risultato") AndAlso
                           matrice_delete(enum_Matrice_Delete.Id_Mov_Det, i) = dr.Item("Id_Mov_Det_Risultato") Then

                            bDuplicato = True

                            Exit For
                        End If

                    Next

                    If Not bDuplicato Then

                        ReDim Preserve matrice_delete(4, UBound(matrice_delete, 2) + 1)

                        matrice_delete(enum_Matrice_Delete.Piva, UBound(matrice_delete, 2)) = dr.Item("Piva_Risultato")
                        matrice_delete(enum_Matrice_Delete.Sa_Cod, UBound(matrice_delete, 2)) = dr.Item("Sa_Cod_Risultato")
                        matrice_delete(enum_Matrice_Delete.Id_Agenda, UBound(matrice_delete, 2)) = dr.Item("Id_Agenda_Risultato")
                        matrice_delete(enum_Matrice_Delete.Id_Mov, UBound(matrice_delete, 2)) = dr.Item("Id_Mov_Risultato")
                        matrice_delete(enum_Matrice_Delete.Id_Mov_Det, UBound(matrice_delete, 2)) = dr.Item("Id_Mov_Det_Risultato")

                    End If

                Case Else

            End Select


        End Sub
        Private Function ControllaOperazione_GeneraMsg(ByVal messaggio As String, dr_Rif As DataRow, msg As String) As String

            Select Case Trim(messaggio)

                Case ""

                    Return "<BR><b>" & Gias.Operazione & ": </b>" & dr_Rif.Item("des_lib").Split("(")(0) & " " & Gias.del & " " & CDate(dr_Rif.Item("validita_inizio")).ToShortDateString() & "<BR> <BR><b>" & Gias.Messaggio & ":</b> " & msg


                Case Else

                    'Inserita solo se non duplicata
                    If InStr(messaggio, msg) = 0 Then
                        Return "<BR><b>" & Gias.Messaggio & ":</b> " & msg
                    Else
                        Return ""
                    End If

            End Select

        End Function

        Private Function ControllaOperazione_GeneraMsg(ByVal messaggio As String, objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                       objParametriAgenda As ParametriAgenda_Temp.ParametriAgenda,
                                                       msg As String,
                                                       Optional ByRef list_msgOperazioniCollegate As List(Of String) = Nothing) As String
            Dim data As String

            Dim descrizione As String = objParametriAgenda.Des_lib.Split("(")(0)
            If descrizione = "" Then
                Dim a_R As New AgronicaCoreContabDAL.Agenda_R
                Dim dt As DataTable = a_R.Leggi(objParametriAgenda.Piva, 0, objParametriAgenda.Id_Agenda, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_server)
                descrizione = dt.Rows(0).Item("des_lib")
                data = dt.Rows(0).Item("validita_inizio")
            Else
                data = objParametriAgenda.Data.ToShortDateString()
            End If

            Select Case messaggio
                Case ""
                    If list_msgOperazioniCollegate IsNot Nothing Then
                        list_msgOperazioniCollegate.Add("- <b>" & descrizione & "</b> " & Gias.del & " " & data)
                    End If
                    Return msg & NEWLINE &
                    "<b>" & descrizione & "</b> " & Gias.del & " " & data
                Case Else

                    If InStr(messaggio, msg) = 0 Then
                        Return NEWLINE & "<b>" & Gias.Messaggio & ":</b> " & msg
                    Else
                        Return ""
                    End If
            End Select

        End Function


        Public Function OperazioneAgenda_Cancella(ByVal matrice_delete As String(,),
                                                  ByRef objParametriAgenda As ParametriAgenda_Temp.ParametriAgenda,
                                                  ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                  Optional ByVal CancellaOperazioneSingolaNoCorrelate As Boolean = False,
                                                  Optional ByVal idServizio As enum_Id_Servizio = enum_Id_Servizio.GiasOnline,
                                                  Optional ByVal origine As enum_SistemiEsterni = enum_SistemiEsterni.gias,
                                                  Optional ByRef Log_Errori As String = ""
                                                  ) As Boolean


            Dim objDP As New AgronicaCoreDataProvider.ConnessioniTransazioni

            ' variabili per connessione
            Dim FlagTransazioneLocale As Boolean
            Dim FlagConnessioneLocale As Boolean
            '------------------------------

            Dim rval As Boolean

            Try

                '------------------------------
                'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
                AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, objParametri_Server)

                Dim objAgendaScrivi As New Agenda_Operazione_Helper
                Dim objDettaglioScrivi As New Agenda_Movimenti_Dettagli_Helper

                For i As Integer = 0 To UBound(matrice_delete, 2)

                    'controllo che per qualche errore non arriva id_agenda=0
                    If CInt(matrice_delete(enum_Matrice_Delete.Id_Agenda, i)) = 0 Then
                        Throw New Exception("Operazione_Agenda_Utility.OperazioneAgenda_Cancella: Attenzione, non tutte le operazioni sono state cancellate, il processo si è interrotto alla riga " & i & " su " & CInt(UBound(matrice_delete, 2)) & " perchè l'id agenda da cancellare è 0.")
                    End If

                    If i = 0 AndAlso CLng(matrice_delete(enum_Matrice_Delete.Id_Mov_Det, i)) > 0 Then

                        'Cancellazione Dettaglio

                        If objDettaglioScrivi.Cancella(piva:=CStr(matrice_delete(enum_Matrice_Delete.Piva, i)),
                                                       saCod:=0,
                                                       idAgenda:=CInt(matrice_delete(enum_Matrice_Delete.Id_Agenda, i)),
                                                       idMov:=CInt(matrice_delete(enum_Matrice_Delete.Id_Mov, i)),
                                                       idMovDet:=CInt(matrice_delete(enum_Matrice_Delete.Id_Mov_Det, i)),
                                                       objParametri:=objParametri_Server) = False Then
                            Throw New Exception(Gias.ImpossibileEliminareOperazione)

                        End If

                    Else

                        'Cancellazione Agenda
                        If objAgendaScrivi.Cancella(piva:=CStr(matrice_delete(enum_Matrice_Delete.Piva, i)),
                                                    saCod:=0,
                                                    idAgenda:=CInt(matrice_delete(enum_Matrice_Delete.Id_Agenda, i)),
                                                    cancellaAggancioRicetta:=True,
                                                    objParametri:=objParametri_Server,
                                                    idServizio:=idServizio,
                                                    CancellaOperazioneSingolaNoCorrelate:=CancellaOperazioneSingolaNoCorrelate,
                                                    origine:=origine) = False Then
                            Throw New Exception(Gias.ImpossibileEliminareOperazione)

                        End If
                    End If
                Next

                If FlagTransazioneLocale Then
                    AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)
                End If

                rval = True

            Catch exc As Exception

                'Faccio il rollback della transazione
                If objParametri_Server.objTransazione IsNot Nothing AndAlso FlagTransazioneLocale Then
                    AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
                End If

                Log_Errori = exc.Message

                rval = False

            Finally

                If FlagConnessioneLocale Then
                    AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)
                End If

            End Try

            Return rval

        End Function


        Public Function OperazioneAgendaAudit_Cancella(ByRef objParametriAgenda As ParametriAgenda_Temp.ParametriAgenda, ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean


            'Dim objDP As New AgronicaCoreDataProvider.ConnessioniTransazioni
            Dim Log_Errori As String = ""

            ' variabili per connessione
            Dim FlagTransazioneLocale As Boolean
            Dim FlagConnessioneLocale As Boolean
            '------------------------------

            Try

                '------------------------------
                'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
                AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                        FlagTransazioneLocale,
                                                                                        objParametri_Server)



                Dim Audit_Cod As Integer = 0
                Dim objAgendaR As New AgronicaCoreContabDAL.Agenda_R
                Dim DtAgenda As DataTable
                DtAgenda = objAgendaR.Leggi("", 0, objParametriAgenda.Id_Agenda, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                If DtAgenda IsNot Nothing AndAlso DtAgenda.Rows.Count > 0 Then
                    Audit_Cod = DtAgenda.Rows(0).Item("audit_cod")
                End If

                If Audit_Cod <> 0 Then
                    Dim objAgendaW As New AgronicaCoreContabDAL.Agenda_W
                    Dim objAuditW As New AgronicaCoreAuditDAL.Audit_W
                    Dim objAuditRisposteW As New AgronicaCoreAuditDAL.Audit_Risposte_W

                    If objAgendaW.Cancella(objParametriAgenda.Piva, 0, objParametriAgenda.Id_Agenda, "", objParametri_Server) = True Then
                        objAuditW.Cancellazione(Audit_Cod, objParametri_Server.PivaSuperUser, "", objParametri_Server)
                        objAuditRisposteW.Cancellazione(0, 0, Audit_Cod, "", 0, "", objParametri_Server)
                    End If
                End If


                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)

            Catch exc As Exception

                'Faccio il rollback della transazione
                If objParametri_Server.objTransazione IsNot Nothing Then
                    AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
                    AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)
                End If
                Return False

            Finally

                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

            End Try


            Return True

        End Function

        Private Function AnimaliMovimentati_Next(Piva As String, ID_Agenda As Integer, objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
            Dim gefutils As New Gias_EF_Utility
            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametriServer.StringaConnessione)
            'Dim scope As New TransactionScope()
            Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim animaliMovimentati As Boolean = False

            Dim Movimenti_Animali = (
                From a In GiasContext.Movimenti_dettagli
                Join b In GiasContext.Movimenti On a.PIVA Equals b.PIVA And a.Id_Agenda Equals b.Id_Agenda And a.Id_Mov Equals b.Id_Mov
                Where a.PIVA = Piva And
                    a.Id_Agenda = ID_Agenda And
                    a.Cod_Progetto <> 0 And
                    a.Elem_Cod = 300
                Select a, b.Data_Movimento)

            For Each Mov_animale In Movimenti_Animali

                If animaleMovimentato_Next(Mov_animale.a.PIVA, Mov_animale.a.Cod_Progetto, Mov_animale.Data_Movimento, GiasContext) Then
                    animaliMovimentati = True
                    Exit For
                End If

            Next

            Return animaliMovimentati

        End Function

        Private Function animaleMovimentato_Next(Piva As String, Cod_Progetto As Integer, DataRiferimento As DateTime, GiasContext As Gias_DeveloperServer_Entities) As Boolean
            Dim animale_Movimentato As Boolean = False

            Dim Movimenti_Animali = (
                From a In GiasContext.Movimenti_dettagli
                Join b In GiasContext.Movimenti On a.PIVA Equals b.PIVA And a.Id_Agenda Equals b.Id_Agenda And a.Id_Mov Equals b.Id_Mov
                Where a.PIVA = Piva And
                    a.Cod_Progetto = Cod_Progetto And
                    a.Elem_Cod = 300 And
                    b.Data_Movimento > DataRiferimento
                Select a, b.Data_Movimento)

            If Movimenti_Animali.Count > 0 Then
                animale_Movimentato = True
            End If

            Return animale_Movimentato

        End Function

        ''' <summary>
        ''' Verifica se l'operazione zootecnica è sincronizzata con un carico/scarico in BDN
        ''' </summary>
        ''' <param name="Piva"></param>
        ''' <param name="Id_Agenda"></param>
        ''' <param name="Lav_Cod"></param>
        ''' <param name="objParametriAgenda"></param>
        ''' <param name="objParametriServer"></param>
        ''' <returns></returns>
        Private Function OperazioneZoo_Sincronizzata(ByVal Piva As String,
                                                     ByVal Id_Agenda As Integer,
                                                     ByVal Lav_Cod As Integer,
                                                     ByRef objParametriAgenda As ParametriAgenda_Temp.ParametriAgenda,
                                                     ByRef objParametriServer As AgronicaCoreParametri) As String
            Dim resp = ""
            Dim operazioneSincronizzata As Boolean = False

            Dim objZoo As New AgronicaCoreAnagrafeDAL.Zoo_Animali
            'Dim gefutils As New Gias_EF_Utility
            'Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametriServer.StringaConnessione)
            ''Dim scope As New TransactionScope()
            'Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim objMov As New AgronicaCoreContabDAL.Movimenti_R
            Dim dtMov = objMov.Leggi(Piva, 0, Id_Agenda, 0, 0, "", enumSelezioneVariabile.Selezione_TabellaCompleta, " Movimenti.CAU_MOV <> '" & CAU_REGISTRAZIONI & "' ", "", objParametriServer)
            'Dim mov = GiasContext.Movimenti.Where(Function(m) m.Id_Agenda = Id_Agenda AndAlso m.Cau_Mov <> CAU_REGISTRAZIONI).FirstOrDefault
            'Dim idMov As Integer = mov.Id_Mov
            Dim idMov As Integer = dtMov(0)("Id_Mov")


            Dim objMovDett As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
            Dim dtMovDett = objMovDett.Leggi(Piva, 0, Id_Agenda, idMov, 0, 0, 0, 0, "", 0, 0, 0, 0, 0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametriServer)
            'Dim movDett = GiasContext.Movimenti_dettagli.Where(Function(md) md.Id_Agenda = Id_Agenda AndAlso md.Id_Mov = idMov)

            Dim objMovDest As New AgronicaCoreContabDAL.Mov_Destinazioni_R
            Dim dtMovDest = objMovDest.Leggi(Piva, 0, Id_Agenda, idMov, 0, 0, 0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametriServer)
            'Dim movDest = GiasContext.Mov_Destinazioni.Where(Function(md) md.Id_Agenda = Id_Agenda AndAlso md.Id_Mov = idMov)

            Dim dtMovReg = objMov.Leggi(Piva, 0, Id_Agenda, 0, 0, CAU_REGISTRAZIONI, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametriServer)
            'Dim movReg = GiasContext.Movimenti.Where(Function(m) m.Id_Agenda = Id_Agenda AndAlso m.Cau_Mov = CAU_REGISTRAZIONI).FirstOrDefault
            Dim idMovReg As Integer = 0
            Dim movReg_Extra_Str = ""
            'Dim movDTE As New AgronicaCoreEntityFramework_POCO.Mov_Dettaglio_Tecnico_Extra
            If Not IsNothing(dtMovReg) AndAlso dtMovReg.Rows.Count > 0 Then
                idMovReg = dtMovReg(0)("Id_Mov")
                movReg_Extra_Str = dtMovReg(0)("Extra_Str")
                'movDTE = GiasContext.Mov_Dettaglio_Tecnico_Extra.Where(Function(md) md.Id_Agenda = Id_Agenda AndAlso md.Id_Mov = idMovReg).FirstOrDefault
            End If

            Select Case Lav_Cod
                Case LAVCOD_NASCITA_ANIMALI, LAVCOD_INCREMENTO_CONSISTENZE_ZOO, LAVCOD_ACQUISTO_ANIMALI
                    For Each md As DataRow In dtMovDett.Rows
                        If Not IsDBNull(md("Id_Mov_Esterno")) AndAlso md("Id_Mov_Esterno") <> 0 Then
                            operazioneSincronizzata = True
                            resp &= ControllaOperazione_GeneraMsg(resp, objParametriServer, objParametriAgenda,
                                                                  Gias.Attenzione & " Questo carico risulta essere sincronizzato in BDN." & "<BR>")
                            Exit For
                        End If
                    Next

                    If Not operazioneSincronizzata AndAlso idMovReg <> 0 AndAlso movReg_Extra_Str <> "" Then
                        resp &= ControllaOperazione_GeneraMsg(resp, objParametriServer, objParametriAgenda,
                                                              Gias.Attenzione & " Questo carico risulta essere sincronizzato in BDN con un modello 4 - " & movReg_Extra_Str & "." & "<BR>")
                        operazioneSincronizzata = True
                    End If

                    'MR - Commentato controllo perché non deve avvisare se cancello un capo che ha il campo ID_Capo_BDN valorizzato, potrebbe essere valorizzato perché presente in un'altra stalla
                    'If Not operazioneSincronizzata Then
                    '    For Each md As DataRow In dtMovDett.Rows
                    '        Dim codProg As Integer = md("Cod_Progetto")
                    '        Dim dtZoo = objZoo.Leggi(Piva, codProg, "", 0, objParametriServer)
                    '        If Not IsNothing(dtZoo) AndAlso dtZoo.Rows.Count > 0 AndAlso dtZoo(0)("ID_Capo_BDN") > 0 Then
                    '            operazioneSincronizzata = True
                    '            resp &= ControllaOperazione_GeneraMsg(resp, objParametriServer, objParametriAgenda,
                    '                                                  Gias.Attenzione & " Uno o più capi risultano essere presenti in BDN." & "<BR>")
                    '            Exit For
                    '        End If
                    '    Next
                    'End If

                Case LAVCOD_DECREMENTO_CONSISTENZE_ZOO, LAVCOD_MORTE_ANIMALI, LAVCOD_MACELLAZIONE_ANIMALI, LAVCOD_VENDITA_ANIMALI, LAVCOD_TRASFERIMENTO_ANIMALI
                    For Each md As DataRow In dtMovDett.Rows
                        If Not IsDBNull(md("Id_Mov_Esterno")) AndAlso md("Id_Mov_Esterno") <> 0 Then
                            operazioneSincronizzata = True
                            resp &= ControllaOperazione_GeneraMsg(resp, objParametriServer, objParametriAgenda,
                                                                  Gias.Attenzione & " Questo scarico risulta essere sincronizzato in BDN." & "<BR>")
                            Exit For
                        End If
                    Next

                    If Not operazioneSincronizzata AndAlso (Lav_Cod = LAVCOD_MACELLAZIONE_ANIMALI Or Lav_Cod = LAVCOD_VENDITA_ANIMALI Or Lav_Cod = LAVCOD_TRASFERIMENTO_ANIMALI) Then
                        If Not operazioneSincronizzata AndAlso idMovReg <> 0 AndAlso movReg_Extra_Str <> "" Then
                            resp &= ControllaOperazione_GeneraMsg(resp, objParametriServer, objParametriAgenda,
                                                                  Gias.Attenzione & " Questo scarico risulta essere sincronizzato in BDN con un modello 4 - " & movReg_Extra_Str & "." & "<BR>")
                            operazioneSincronizzata = True
                            Exit Select
                        End If
                    End If

                Case LAVCOD_CUREMEDICAMENTI_ANIMALI
                    For Each md As DataRow In dtMovDett.Rows
                        If md("Rif_Esterno") <> "" Then
                            operazioneSincronizzata = True
                            resp &= ControllaOperazione_GeneraMsg(resp, objParametriServer, objParametriAgenda,
                                                                  Gias.Attenzione & " Questo trattamento risulta essere sincronizzato in BDN." & "<BR>")
                            Exit For
                        End If
                    Next

                Case Else
                    operazioneSincronizzata = False

            End Select

            Return resp

        End Function

        ''' <summary>
        ''' Verifica se la gestione di visibilità fra gruppi utenti e merci per l'impresa passata è attiva e, nel caso, verifica che tutti i dettagli del documento
        ''' siano visualizzabili dall'utente corrente. Restituisce sempre True se l'utente è superuser
        ''' </summary>
        ''' <param name="objParametri_Server"></param>
        ''' <param name="objParametri_Utenti"></param>
        ''' <param name="piva"></param>
        ''' <param name="idAgenda"></param>
        ''' <param name="lavCod">Se valorizzato, controlla se l'operazione d'agenda si riferisce ad un documento contabile, se non lo è restituisce True senza effettuare ulteriori controlli</param>
        ''' <returns></returns>
        Public Function ControllaPermessiVisibilitaGruppiMerce(ByRef objParametri_Server As AgronicaCoreParametri, ByRef objParametri_Utenti As AgronicaCoreParametri, ByVal piva As String, ByVal idAgenda As Integer, ByVal lavCod As Integer) As Boolean

            Dim utenteAbilitatoDocumento As Boolean = True

            If objParametri_Server.UtenteUsername = objParametri_Server.SuperUserUsername Then
                Return True
            End If

            'Dim lavCodDDTAcquisto As Integer() = {LAVCOD_BOLLA_RICEVUTA}
            'Dim lavCodAccettazione As Integer() = {LAVCOD_ACCETTAZIONE_DIVERSI, LAVCOD_DISTINTA_CARICO_ACCETTAZIONE, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE, LAVCOD_DISTINTA_CARICO, LAVCOD_AUTO_DDT_EMESSO}
            'Dim lavCodDDTVendita As Integer() = {LAVCOD_DDT_CONTABILIZZATO_EMESSO, LAVCOD_BOLLA_EMESSA}
            'Dim lavCodFattAcquisto As Integer() = {LAVCOD_FATTURA_RICEVUTA, LAVCOD_NOTA_ACCREDITO_RICEVUTA}
            'Dim lavCodFattVendita As Integer() = {LAVCOD_FATTURA_EMESSA}
            'Dim lavCodNotaAccrVendita As Integer() = {LAVCOD_NOTA_ACCREDITO_EMESSA}
            'Dim lavCodOrdineAcquisto As Integer() = {LAVCOD_ORDINE_ACQUISTO}
            'Dim lavCodOrdineVendita As Integer() = {LAVCOD_ORDINE_VENDITA}
            'Dim lavCodTrasferimento As Integer() = {LAVCOD_TRASFERIMENTO}
            'Dim lavCodLavorazioni As Integer() = {LAVCOD_TESTATE_ORDINE_LAVORAZIONE, LAVCOD_TRASFORMAZIONI}
            'Dim lavCodMagazzino As Integer() = {LAVCOD_CARICO, LAVCOD_SCARICO}
            'Dim lavCodContrattoAffitto As Integer() = {LAVCOD_CONTRATTO_AFFITTO}
            Dim lavCodDocContabile As Integer() = {
                LAVCOD_BOLLA_RICEVUTA, LAVCOD_ACCETTAZIONE_DIVERSI, LAVCOD_DISTINTA_CARICO_ACCETTAZIONE, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE,
                LAVCOD_DISTINTA_CARICO, LAVCOD_AUTO_DDT_EMESSO, LAVCOD_DDT_CONTABILIZZATO_EMESSO, LAVCOD_BOLLA_EMESSA,
                LAVCOD_FATTURA_RICEVUTA, LAVCOD_NOTA_ACCREDITO_RICEVUTA, LAVCOD_FATTURA_EMESSA, LAVCOD_NOTA_ACCREDITO_EMESSA,
                LAVCOD_ORDINE_ACQUISTO, LAVCOD_ORDINE_VENDITA, LAVCOD_TRASFERIMENTO, LAVCOD_CARICO, LAVCOD_SCARICO
            }

            If lavCod <> 0 AndAlso Not lavCodDocContabile.Contains(lavCod) Then
                Return True
            End If

            Dim impreseImpostazioniR As New Imprese_Impostazioni_R
            Dim defaultCategorie As String = impreseImpostazioniR.LeggiScalareMulticentroAziendaSuperUser(piva, Nothing,
                                                                                              enum_Impostazioni_Utenti.Default_GruppoMerce_CategoriaProdotto,
                                                                                              "",
                                                                                              objParametri_Utenti,
                                                                                              objParametri_Server)


            If Not String.IsNullOrEmpty(defaultCategorie) Then
                'Se l'impostazione è valorizzata sulla piva corrente significa che sono presenti delle associazioni fra elem_cod e gruppi merce
                'di conseguenza è attiva la gestione dei gruppi merce

                Dim handleGruppiUtentiMerce As New Gruppi_UtenteXGruppi_Merce_R(objParametri_Server, objParametri_Utenti)

                Dim gruppiUtentiMerceImpresa = handleGruppiUtentiMerce.Leggi("Gruppi_UtenteXGruppi_Merce.Piva = '" & piva & "'", "")

                If gruppiUtentiMerceImpresa.Rows.Count > 0 Then
                    'Se sono presenti dei record in Gruppi_UtenteXGruppi_Merce per la piva passata, l'utente può visualizzare solo i prodotti che appartengono ai gruppi merce
                    'collegati ai gruppi utente di cui fa parte; di conseguenza verifico che tutti i dettagli del documento siano visualizzabili:
                    utenteAbilitatoDocumento = handleGruppiUtentiMerce.DettagliDocumentoVisibili_X_GruppiUtente_X_GruppiMerce(
                                piva, idAgenda, "mov_det.ordine_det <> 1000 AND mov_det.Elem_Cod <> " & RIGA_DESCRIZIONE)
                End If

            End If

            Return utenteAbilitatoDocumento

        End Function

        Public Sub allinea_DataUsernameCreazione(ByRef agenda As Operazione_Agenda, objParametri_Server As AgronicaCoreParametri, Tipo_Attivita As Tipo_Attivita, Ricetta_Cod As String)

            Dim objAgendaScrivi As New Agenda_Operazione_Helper
            Dim agendaOld As Operazione_Agenda

            Dim Data_Creazione As Date
            Dim Username_Creazione As String = ""

            'DT: recupero i dati della riga principale di agenda che voglio preservare da cancellazione/inserimento
            If Tipo_Attivita = Tipo_Attivita.QuadernoDiCampagna Then
                agendaOld = objAgendaScrivi.Leggi(agenda.Piva, saCod:=0, agenda.Id_Agenda, 0, objParametri_Server)
                If agendaOld IsNot Nothing Then
                    Data_Creazione = agendaOld.Data_Creazione
                    Username_Creazione = agendaOld.Username_Creazione
                End If
            Else
                Dim objRicetteOperazioni As New AgronicaCoreContabDAL.Ricette_Operazioni_R
                Dim DT = objRicetteOperazioni.Leggi_DataUsernameCreazione(Ricetta_Cod, agenda.Id_Agenda, "", "", objParametri_Server)

                If DT IsNot Nothing AndAlso DT.Rows.Count > 0 Then
                    Data_Creazione = DT(0).Item("Data_Creazione")
                    Username_Creazione = DT(0).Item("Username_Creazione")
                End If
            End If


            If agendaOld IsNot Nothing OrElse Tipo_Attivita = Tipo_Attivita.Ricetta Then

                agenda.Data_Creazione = Data_Creazione
                agenda.Username_Creazione = Username_Creazione

                For Each movimento In agenda.Movimenti
                    movimento.Data_Creazione = Data_Creazione
                    movimento.Username_Creazione = Username_Creazione

                    For Each dettaglio In movimento.Movimenti_Dettagli
                        dettaglio.Data_Creazione = Data_Creazione
                        dettaglio.Username_Creazione = Username_Creazione

                        For Each destinazione In dettaglio.Movimenti_Destinazioni
                            destinazione.Data_Creazione = Data_Creazione
                            destinazione.Username_Creazione = Username_Creazione
                        Next

                        For Each dettaglio_tecnico In dettaglio.Movimenti_Dettagli_Tecnici
                            dettaglio_tecnico.Data_Creazione = Data_Creazione
                            dettaglio_tecnico.Username_Creazione = Username_Creazione
                        Next
                    Next

                    For Each dettaglio_tecnico In movimento.Movimenti_Dettagli_Tecnici
                        dettaglio_tecnico.Data_Creazione = Data_Creazione
                        dettaglio_tecnico.Username_Creazione = Username_Creazione
                    Next

                    For Each dettaglio_tecnico_extra In movimento.Movimenti_Dettagli_Tecnici_Extra
                        dettaglio_tecnico_extra.Data_Creazione = Data_Creazione
                        dettaglio_tecnico_extra.Username_Creazione = Username_Creazione
                    Next
                Next

                For Each nota In agenda.Note
                    nota.Data_Creazione = Data_Creazione
                    nota.Username_Creazione = Username_Creazione
                Next

                For Each rif In agenda.Agenda_Riferimenti
                    rif.Data_Creazione = Data_Creazione
                    rif.Username_Creazione = Username_Creazione
                Next
            End If
        End Sub
    End Module

End Namespace
