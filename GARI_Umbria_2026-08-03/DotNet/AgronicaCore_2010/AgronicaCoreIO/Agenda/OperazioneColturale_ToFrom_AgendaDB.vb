Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider
Imports System.Web
Imports AgronicaCoreModello
Imports AgronicaCoreModello.Agenda
Imports AgronicaCoreModello.Agenda.Operazioni_Colturali
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreModelsSTD.attivita.Attivita
Imports AgronicaCoreModelsSTD.attivita

Namespace Agenda


    Public Class OperazioneColturale_ToFrom_AgendaDB

        Dim Operazione As AgronicaCoreModello.Agenda.Operazioni_Colturali.Operazione_Colturale.I_Operazione_Colturale
        Dim objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri

        Sub New(ByVal Operazione_ByVal As AgronicaCoreModello.Agenda.Operazioni_Colturali.Operazione_Colturale.I_Operazione_Colturale, ByRef objParametri_In As AgronicaCoreDataProvider.AgronicaCoreParametri)
            objParametri = objParametri_In
            Operazione = Operazione_ByVal
        End Sub

        Public Function Leggi_Operazione() As AgronicaCoreModello.Agenda.Operazioni_Colturali.Operazione_Colturale.I_Operazione_Colturale

            Dim piva As String = Operazione.Piva
            Dim Id_agenda As Integer = Operazione.ID_Agenda

            Dim objAgenda As New Agenda_Operazione_Helper
            Dim Agenda As New AgronicaCoreModello.OperazioneAgenda_Temp.Operazione_Agenda
            Agenda = objAgenda.Leggi(piva,
                                         0,
                                         Id_agenda,
                                         0,
                                        objParametri)


            If IsNothing(Agenda) Then
                Throw New ApplicationException
                Return Nothing
            End If
            'Sa_Cod <> Agenda.Sa_Cod perché è a zero in parametriagenda e in I_operazione_Colturale,
            'dato che l'id_agenda è univoco, quindi lo devo impostare io in I_operazione_Colturale
            If Not (piva = Agenda.Piva AndAlso Operazione.Lav_Cod = Agenda.Lav_Cod) Then
                Throw New ApplicationException
                Return Nothing
            End If

            '------------------------------------------------------------------------------------------
            '------------------------------------------------------------------------------------------
            '------------------------------------------------------------------------------------------
            Operazione.Sa_Cod = Agenda.Sa_Cod
            '------------------------------------------------------------------------------------------
            '------------------------------------------------------------------------------------------
            '------------------------------------------------------------------------------------------

            'NOTE
            If Not IsNothing(Agenda.Note) Then
                For i = 0 To Agenda.Note.Count - 1
                    Operazione.Note_Codificate.Add(Agenda.Note(i).Nota_Cod)
                Next
            End If

            'MOVIMENTI
            If Not IsNothing(Agenda.Movimenti) Then

                Dim MovimentiCosti As List(Of Movimento) = New List(Of Movimento)

                For i = 0 To Agenda.Movimenti.Count - 1

                    'controllo corrispondeza piva con agenda
                    If Agenda.Piva <> Agenda.Movimenti(i).Piva Then
                        Throw New ApplicationException
                    End If

                    Dim conteggioMovimentiOperazione As Integer = 0
                    Dim conteggioMovimentiScarico As Integer = 0
                    Select Case Agenda.Movimenti(i).Cau_Mov

                        Case enum_Agenda_Causali.RILIEVO_CAMPO, enum_Agenda_Causali.TRATTAMENTO

                            conteggioMovimentiOperazione += 1
                            If conteggioMovimentiOperazione > 1 Then
                                Throw New NotImplementedException 'dovrebbe esserci un solo movimento operazione per agenda
                            End If

                            Select Case CInt(Agenda.Lav_Cod)
                                Case LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA
                                    LeggiMovimentoAgenda_InstallazioneTrappole_CattureMassa(Agenda.Movimenti(i))
                                Case LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE
                                    LeggiMovimentoAgenda_Confusione_Disorientamento_Sessuale(Agenda.Movimenti(i))
                                Case LAVCOD_REINNESCO_TRAPPOLE, LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE
                                    Leggi_MovimentoAgenda_Reinnesco_Rilievo_Trappole(Agenda.Movimenti(i))
                                Case Else
                                    Throw New NotImplementedException
                            End Select

                        Case enum_Agenda_Causali.SCARICO
                            conteggioMovimentiScarico += 1
                            If conteggioMovimentiScarico > 1 Then
                                'Throw New NotImplementedException 'dovrebbe esserci un solo movimento scarico per agenda
                            End If
                            'MOVIMENTI_DETTAGLI
                            If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli) Then

                                For j = 0 To Agenda.Movimenti(i).Movimenti_Dettagli.Count - 1

                                    '------------------------'------------------------'------------------------
                                    '------------------------'------------------------'------------------------
                                    '------------COSTO ACCESSORIO DA INGEGNERIZ'------------------------
                                    'cerco se costo accessorio
                                    If Agenda.Movimenti(i).Movimenti_Dettagli(j).Elem_Cod <> TRAPPOLE AndAlso
                                        Agenda.Movimenti(i).Movimenti_Dettagli(j).Elem_Cod <> INNESCHI Then
                                        'è un movimento dovuto ad un costo accessorio
                                        'Throw New NotImplementedException
                                        MovimentiCosti.Add(Agenda.Movimenti(i))
                                        Exit For
                                    End If
                                    '------------------------'------------------------'------------------------
                                    '------------------------'------------------------'------------------------

                                    'controllo corrispondeza  piva con agenda, sa_cod potrebbe cambiare per chi ha fabbricato in altro centro
                                    If Agenda.Piva <> Agenda.Movimenti(i).Movimenti_Dettagli(j).Piva Then
                                        Throw New ApplicationException
                                    End If
                                    Dim conteggioMovDet As Integer = Agenda.Movimenti(i).Movimenti_Dettagli.Count
                                    Select Case CInt(Agenda.Lav_Cod)
                                        Case LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA
                                            Leggi_MovimentoAgenda_Scarico_Trappole_CattureMassa(Agenda.Movimenti(i).Movimenti_Dettagli(j), conteggioMovDet)
                                        Case LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE
                                            Leggi_MovimentoAgenda_Scarico_ConfusDisorientSess(Agenda.Movimenti(i).Movimenti_Dettagli(j), conteggioMovDet)
                                        Case LAVCOD_REINNESCO_TRAPPOLE, LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE
                                            Leggi_MovimentoAgenda_Scarico_Reinnesco_Rilievo_Trappole(Agenda.Movimenti(i).Movimenti_Dettagli(j), conteggioMovDet)
                                        Case Else
                                            Throw New NotImplementedException
                                    End Select

                                Next

                            End If

                        Case CAU_IMPUTAZIONE_PARCOMACCHINE
                            MovimentiCosti.Add(Agenda.Movimenti(i))

                        Case CAU_IMPUTAZIONE_MANODOPERA
                            MovimentiCosti.Add(Agenda.Movimenti(i))

                        Case CAU_IMPUTAZIONE_TERZISTI
                            MovimentiCosti.Add(Agenda.Movimenti(i))

                        Case CAU_IMPUTAZIONE_UTILIZZO_PRODOTTI
                            MovimentiCosti.Add(Agenda.Movimenti(i))

                        Case CAU_IMPUTAZIONE_TECNICO_RESPONSABILE
                            MovimentiCosti.Add(Agenda.Movimenti(i))

                        Case Else
                            Throw New NotImplementedException

                    End Select
                Next

                Dim objParametriAgenda As New AgronicaCoreModello.ParametriAgenda_Temp.ParametriAgenda
                objParametriAgenda.Movimenti = MovimentiCosti

            End If

            Return Operazione
        End Function

        '-------------------------------------------------------------------------------------------
        '-------------------------------------------------------------------------------------------
        '-------------------------------------------------------------------------------------------
        '-------------------------------------------------------------------------------------------
        '-------------------------------------------------------------------------------------------
        '-------------------------------------------------------------------------------------------
        '-------------------------------------------------------------------------------------------
        Function Salva_Operazione(ByRef messaggiErrore As AgronicaCoreUtility.Messages_Str, ByRef Id_Agenda_return As Integer) As Boolean


            Dim res As Boolean = False

            Try


                '-----------------------------------------------------
                '----------- CONNESSIONE E TRANSAZIONE ---------------
                Dim objDP As New AgronicaCoreDataProvider.ConnessioniTransazioni
                ConnessioniTransazioni.ApriConnessione(True, objParametri)
                '-----------------------------------------------------



                'Dim Id_Agenda_Old As Integer = Operazione.ID_Agenda
                Select Case Operazione.OperazioneMulticentro

                    '--------------------------------------------------
                    '--------------------------------------------------
                    '--------- OPERAZIONE MULTI-CENTRO    -------------
                    '--------------------------------------------------
                    '--------------------------------------------------

                    'per il multicentro devo gestire il sa_cod, creando un agenda e movimento per ciascun centro!
                    'per il multicentro devo gestire il sa_cod, creando un agenda e movimento per ciascun centro!
                    'per il multicentro devo gestire il sa_cod, creando un agenda e movimento per ciascun centro!
                    'per il multicentro devo gestire il sa_cod, creando un agenda e movimento per ciascun centro!

                    Case True
                        Throw New Exception(" Operazione multicentro non supportata! ")
                        Return False

                    Case Else

                        '--------------------------------------------------
                        '--------------------------------------------------
                        '--------- OPERAZIONE SINGOLO CENTRO    -----------
                        '--------------------------------------------------
                        '--------------------------------------------------

                        'per le operazioni singolo centro devo gestire il sa_cod,
                        ' devo avere un sa_Cod valido e non zero, e creo un solo  agenda e movimento !
                        If Operazione.Sa_Cod = 0 Then
                            'per ora non è gestito il recupero dei centri e il multicentro nell'oggetto operazione,
                            'da fare, è importante per usare l'oggetto per le operazioni multicentro,
                            'Occorre gestire l'uilizzo di Sa-Cod dell'oggetto in maniera ottimizzata
                            Throw New Exception(" Occorre selezionare almeno un centro aziendale ")
                            Return False
                        End If


                        '--------------------------------------------------
                        '--------------------------------------------------
                        '--------- CREAZIONE OGGETTO DA SALVARE   ---------
                        '--------------------------------------------------
                        '--------------------------------------------------

                        Dim Agenda As AgronicaCoreModello.OperazioneAgenda_Temp.Operazione_Agenda = CreaOggettoAgenda(Id_Agenda_return, messaggiErrore)

                        If IsNothing(Agenda) Then
                            Throw New Exception(" Non è stato possibile creare l'operazione. ")
                        End If


                        ' ''-----------------------------------------------------
                        ' ''----------- CONNESSIONE E TRANSAZIONE ---------------
                        ''Dim objDP As New AgronicaCoreDataProvider.ConnessioniTransazioni
                        ''AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, objParametri)
                        ' ''-----------------------------------------------------

                        Dim objAgendaScrivi As New Agenda_Operazione_Helper
                        Dim Id_Agenda_Nuovo As Integer

                        If Operazione.Tipo_Operazione = enum_TipoOperazioneDB.Modifica Then

                            'Recupero i vecchi costi CdG prima che vengano cancellati
                            Dim mdr_Rif As New Agenda_Movimenti_Dettagli_Riferimenti_Helper()
                            Dim lista_mdRif As List(Of Movimento_Dettaglio_Riferimento) = mdr_Rif.LeggiRiferimentiAgenda(Agenda.Piva, 0, Agenda.Id_Agenda, 0, "", objParametri)

                            'Dal 06/23 conserviamo la data e l'username di creazione originali per tutti i record Agenda
                            allinea_DataUsernameCreazione(Agenda, objParametri, Tipo_Attivita.QuadernoDiCampagna, 0)

                            Dim CancellataOperazione As Boolean = objAgendaScrivi.Cancella(Operazione.Piva, Operazione.Sa_Cod, Operazione.ID_Agenda, False, objParametri, logCancellazione:=False)

                            'Riscrivo i vecchi costi
                            Dim mdRif_helper As New Agenda_Movimenti_Dettagli_Riferimenti_Helper
                            For Each mdRif As Movimento_Dettaglio_Riferimento In lista_mdRif
                                If mdRif.Lav_Cod_Rif = LAVCOD_COSTI_CDG Then
                                    mdRif_helper.Scrivi(mdRif, objParametri)
                                    Exit For
                                End If
                            Next

                        End If

                        Id_Agenda_Nuovo = objAgendaScrivi.Scrivi(Agenda, objParametri)


                        'Id_Agenda_Nuovo = objAgendaScrivi.Scrivi(Agenda, objParametri)


                        ''Questa parte è stata copiata da utility operazioni per salvare le operazioni di scarico e il ddt e i riferimnenti nel caso di utilizzo di magazzino esterno
                        ''non spostare, l'operazione agenda vecchia in modifica devo eliminarla dopo
                        ''Dim util As New Utility_NS.Utility_Operazioni()
                        'Gestisci_Magazzino_Aziendale(Agenda, Id_Agenda_Nuovo)


                        ''se la scrittura è andata a buon fine e sono in modifica
                        ''CANCELLO la vecchia operazione

                        'If Id_Agenda_Nuovo <> 0 And Operazione.Tipo_Operazione = enum_TipoOperazioneDB.Modifica Then
                        '    If Operazione.ID_Agenda > 0 Then
                        '        Dim CancellataOperazione As Boolean = False

                        '        CancellataOperazione = objAgendaScrivi.Cancella(Operazione.Piva, _
                        '                                                         Operazione.Sa_Cod, _
                        '                                                         Operazione.ID_Agenda, False, _
                        '                                                         objParametri)


                        '        'modifico l'aggancio alla ricetta
                        '        Dim objRicetta As New AgronicaCoreContabDAL.RicettexAgenda_W
                        '        Dim ModRicetta As Boolean
                        '        ModRicetta = objRicetta.Modifica_Agenda(0, 0, _
                        '                                                Operazione.ID_Agenda, _
                        '                                                Id_Agenda_Nuovo, _
                        '                                                AGRODATAINIZIO, _
                        '                                                AGRODATAFINE, _
                        '                                                "", _
                        '                                                objParametri)
                        '    End If
                        'End If

                        objAgendaScrivi = Nothing

                        Id_Agenda_return = Id_Agenda_Nuovo


                End Select







                res = True


                'commit transazione
                ConnessioniTransazioni.ChiudiTransazione(1, objParametri)

            Catch ex As Exception

                'commit transazione rollback
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri)

                'Throw New Exception("[ Agenda_Operazione_Helper.scrivi() ] : " & ex.Message)

                'Messaggi.AgroMsgBox("Attenzione, l'operazione non è stata registrata! <br> " & ex.Message, Page, , _
                '                   CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))

                messaggiErrore.add(ex.Message)
                res = False

            Finally

                'chiudi connessione
                ConnessioniTransazioni.ChiudiConnessione(objParametri)

            End Try



            Return res
        End Function

#Region "Trappole Crea Agenda Per Scrittura"


        Private Function CreaOggettoAgenda(ByVal Id_Agenda As Integer, ByRef messaggiErrore As AgronicaCoreUtility.Messages_Str) As AgronicaCoreModello.OperazioneAgenda_Temp.Operazione_Agenda

            Dim Agenda As AgronicaCoreModello.OperazioneAgenda_Temp.Operazione_Agenda
            Dim inneschiNumTotale As Integer = 0 'usato solo per trappole e massa, ignorato per conf e dis sessuale

            '--------------AGENDA------------------
            If Not Crea_Agenda(Id_Agenda, Agenda) Then
                messaggiErrore.add(" Non è stato possibile creare l'oggetto agenda ")
                Return Nothing
            End If


            '--------------NOTE------------------
            If Not Crea_Agenda_Note(Id_Agenda, Agenda) Then
                messaggiErrore.add(" Non è stato possibile creare le note ")
                Return Nothing
            End If


            '---------------MOVIMENTI COSTI ACCESSORI--------
            If Not Crea_Agenda_Movimento_CostiAccessori(Id_Agenda, Agenda) Then
                messaggiErrore.add(" Non è stato possibile creare i costi accessori ")
                Return Nothing
            End If


            '------------- MOVIMENTO RILIEVO / TRATTAMENTO---------
            'inneschiNumTotale usato solo per trappole e massa, ignorato per conf e dis sessuale
            Select Case CInt(Operazione.Lav_Cod)
                Case LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA
                    If Not Crea_Agenda_Movimento_InstallazioneTrappole_CattureMassa(Id_Agenda, inneschiNumTotale, Agenda, messaggiErrore) Then
                        messaggiErrore.add(" Non è stato possibile creare il movimento installazione ; ")
                        Return Nothing
                    End If

                Case LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE
                    If Not Crea_Agenda_Movimento_Confusione_Disorientamento_Sessuale(Id_Agenda, Agenda, messaggiErrore) Then
                        messaggiErrore.add(" Non è stato possibile creare  il movimento ")
                        Return Nothing
                    End If

                Case LAVCOD_REINNESCO_TRAPPOLE, LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE
                    If Not Crea_Agenda_Movimento_Reinnesco_RilievoAvv_Trappola(Id_Agenda, Agenda, messaggiErrore) Then
                        messaggiErrore.add(" Non è stato possibile creare  il movimento ")
                        Return Nothing
                    End If

                Case Else
                    Throw New NotImplementedException
            End Select


            '-------------- MOVIMENTO SCARICO---------------------------
            'modifica per multicentro,solo se l'operazione agenda riguarda il magazzino del centro selezionato
            'If Sa_Cod = Mag_Sa_Cod AndAlso objParametriAgenda.Fabbricato <> "0" Then
            If Not IsNothing(Operazione.Magazzino) Then
                'se è selezionato il magazzino
                'uno scarico totale per le trappole e uno per gli inneschi (solo per massa e trappole)
                'inneschiNumTotale usato solo per trappole e massa, ignorato per conf e dis sessuale
                If Not Crea_Agenda_Movimento_Scarico(Id_Agenda, Agenda) Then
                    messaggiErrore.add(" Non è stato possibile creare  il movimento di scarico ")
                    Return Nothing
                End If
            End If

            Return Agenda

        End Function


        Private Function Crea_Agenda(ByVal Id_Agenda As Integer, ByRef Agenda As AgronicaCoreModello.OperazioneAgenda_Temp.Operazione_Agenda) As Boolean



            Dim BaseCode As Integer
            Dim TopCode As Integer

            '------------------------------------------------
            '----- Calcolo i valori di BaseCode e TopCode
            '------------------------------------------------
            UtilityProvider.Calcola_BaseCode_TopCode(BaseCode, TopCode,
                                                     HttpContext.Current.Session("ASG_ProgressivoGIAS"))



            '------------------------------------------------
            '----- AGENDA
            '------------------------------------------------
            Agenda = New AgronicaCoreModello.OperazioneAgenda_Temp.Operazione_Agenda
            Agenda.Tipo_Operazione = Operazione.Tipo_Operazione
            Agenda.Id_Agenda = Id_Agenda
            Agenda.Data = Operazione.DataOperazione
            Agenda.Piva = Operazione.Piva
            Agenda.Sa_Cod = Operazione.Sa_Cod
            Agenda.Lav_Cod = Operazione.Lav_Cod
            Agenda.Des_Lib = Operazione.Lav_Des '& " (" & Operazione.Veg_Des & "  [" + StrVarieta + "])"

            Agenda.BaseCode = BaseCode
            Agenda.TopCode = TopCode

            Return True
        End Function


        '##########################################################################################
        Private Function Crea_Agenda_Note(ByVal Id_Agenda As Integer, ByRef Agenda As AgronicaCoreModello.OperazioneAgenda_Temp.Operazione_Agenda) As Boolean
            Dim Nota As Nota
            'Dim ListaConsigli As List(Of Nota)
            'ListaConsigli = CType(Master, Operazione).GetConsigli()
            If Operazione.Note_Codificate.Count > 0 Then
                Agenda.Note = New List(Of Nota)
                For i = 0 To Operazione.Note_Codificate.Count - 1
                    Nota = New Nota
                    Nota.Id_Agenda = Id_Agenda
                    Nota.Nota_Cod = Operazione.Note_Codificate(i) 'Nota_Cod
                    Agenda.Note.Add(Nota)
                Next
            End If
            Return True
        End Function


        'gestione salvataggio costi accessori, da modificare creando modellon apposito

        Private Function Crea_Agenda_Movimento_CostiAccessori(ByVal Id_Agenda As Integer, ByRef Agenda As AgronicaCoreModello.OperazioneAgenda_Temp.Operazione_Agenda) As Boolean
            For i = 0 To Operazione.Costi_Accessori_Temporaneo.Count - 1
                Operazione.Costi_Accessori_Temporaneo(i).Id_Agenda = Id_Agenda
                Operazione.Costi_Accessori_Temporaneo(i).Sa_Cod = Agenda.Sa_Cod
                If Not IsNothing(Operazione.Costi_Accessori_Temporaneo(i).Movimenti_Dettagli) Then
                    Dim j As Integer
                    For j = 0 To Operazione.Costi_Accessori_Temporaneo(i).Movimenti_Dettagli.Count - 1
                        Operazione.Costi_Accessori_Temporaneo(i).Movimenti_Dettagli(j).Id_Agenda = Id_Agenda
                        If Operazione.Costi_Accessori_Temporaneo(i).Movimenti_Dettagli(j).Sa_Cod = 0 Then
                            Operazione.Costi_Accessori_Temporaneo(i).Movimenti_Dettagli(j).Sa_Cod = Agenda.Sa_Cod
                        End If
                    Next
                End If

                If Not IsNothing(Operazione.Costi_Accessori_Temporaneo(i).Movimenti_Dettagli_Tecnici) Then
                    Dim j As Integer
                    For j = 0 To Operazione.Costi_Accessori_Temporaneo(i).Movimenti_Dettagli_Tecnici.Count - 1
                        Operazione.Costi_Accessori_Temporaneo(i).Movimenti_Dettagli_Tecnici(j).Id_Agenda = Id_Agenda
                        If Operazione.Costi_Accessori_Temporaneo(i).Movimenti_Dettagli_Tecnici(j).Sa_Cod = 0 Then
                            Operazione.Costi_Accessori_Temporaneo(i).Movimenti_Dettagli_Tecnici(j).Sa_Cod = Agenda.Sa_Cod
                        End If
                    Next
                End If


                Agenda.Movimenti.Add(Operazione.Costi_Accessori_Temporaneo(i))
            Next

            Return True
        End Function


        Private Function Crea_Agenda_Movimento_Reinnesco_RilievoAvv_Trappola(ByVal Id_Agenda As Integer, ByRef Agenda As AgronicaCoreModello.OperazioneAgenda_Temp.Operazione_Agenda, ByRef messaggiErrore As AgronicaCoreUtility.Messages_Str) As Boolean

            Dim Movimento_Dettaglio_Qta_Reinnesco As Decimal = 1
            Dim Movimento_Dettaglio_Contabilizzato_Reinnesco As Integer = 1
            Dim Movimento_Dettaglio_Pendente_Reinnesco As Integer = 3
            Dim Movimento_Dettaglio_Extra_Date As Date = AGRODATAINIZIO
            Dim Movimento_Dettaglio_Anno As Integer = 1900


            Dim Operazione_Specifica As Operazione_Colturale.Aggiornamento_Trappole.Reinnesco_Trappole =
                  DirectCast(Operazione, Operazione_Colturale.Aggiornamento_Trappole.Reinnesco_Trappole)

            Dim NumeMovDetTecTrappole As Integer = 0


            Select Case CInt(Operazione.Lav_Cod)
                Case LAVCOD_REINNESCO_TRAPPOLE, LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE
                    'continua, questo metodo deve essere chioamato solo in questi casi altrimenti genero accezione
                Case Else
                    Throw New Exception
            End Select


            Dim Movimento_OperazioneColturale As New Movimento

            Movimento_OperazioneColturale.Id_Agenda = Id_Agenda
            Movimento_OperazioneColturale.Piva = Operazione.Piva
            Movimento_OperazioneColturale.Sa_Cod = Operazione.Sa_Cod
            Movimento_OperazioneColturale.Lav_Cod = Operazione.Lav_Cod
            Movimento_OperazioneColturale.Cau_Mov = Operazione.Cau_Mov
            Movimento_OperazioneColturale.Mov_Desc = Operazione.Nota
            Movimento_OperazioneColturale.Data = Operazione.DataOperazione

            Movimento_OperazioneColturale.BaseCode = Agenda.BaseCode
            Movimento_OperazioneColturale.TopCode = Agenda.TopCode

            '------------------------------------------------
            '----- MOVIMENTI DETTAGLI
            '------------------------------------------------
            If Operazione_Specifica.Reinneschi.Count = 0 Then
                messaggiErrore.add(" Non è possibile salvare l'operazione senza reinneschi o rilievi ")
                Return False
            End If

            Dim xCalcolo_QD_SuperficieTotale As Decimal = (
                From ST In Operazione_Specifica.Reinneschi
                Select CType(ST.Trappola.Impianto_Di_Installazione.Qta2, Decimal)
            ).Sum

            'aggiungo una operazione MOVIMENTI DETTAGLI per ciascun reinnesco
            For i = 0 To Operazione_Specifica.Reinneschi.Count - 1

                Dim Movimento_Dettaglio As New Movimento_Dettaglio

                Movimento_Dettaglio.Id_Agenda = Id_Agenda
                Movimento_Dettaglio.Piva = Operazione.Piva
                Movimento_Dettaglio.Sa_Cod = Operazione.Sa_Cod
                Movimento_Dettaglio.Elem_Cod = TRAPPOLE
                Movimento_Dettaglio.Pro_Cod = Operazione_Specifica.Reinneschi(i).Trappola.Prodotto_ID
                Movimento_Dettaglio.Udm_Cod = enum_UnitaMisura.Numero_Trappole ' unità di misura n.trappole
                Movimento_Dettaglio.Qta = Movimento_Dettaglio_Qta_Reinnesco
                Movimento_Dettaglio.Contabilizzato = Movimento_Dettaglio_Contabilizzato_Reinnesco
                Movimento_Dettaglio.Pendente = Movimento_Dettaglio_Pendente_Reinnesco
                Movimento_Dettaglio.Extra_Date = Movimento_Dettaglio_Extra_Date
                Movimento_Dettaglio.Anno = Movimento_Dettaglio_Anno
                Movimento_Dettaglio.Data = Operazione.DataOperazione
                Movimento_Dettaglio.BaseCode = Agenda.BaseCode
                Movimento_Dettaglio.TopCode = Agenda.TopCode


                '------------------------------------------------
                '----- MOVIMENTI DESTINAZIONI
                '------------------------------------------------

                'per ciascun movimento dettaglio creo un figlio mov. destinazione
                Dim Movimento_Destinazione As New Movimento_Destinazione

                Movimento_Destinazione.Id_Agenda = Id_Agenda
                Movimento_Destinazione.Data = Operazione.DataOperazione
                Movimento_Destinazione.Piva = Operazione_Specifica.Reinneschi(i).Trappola.Impianto_Di_Installazione.Piva
                Movimento_Destinazione.Sa_Cod = Operazione_Specifica.Reinneschi(i).Trappola.Impianto_Di_Installazione.Sa_Cod
                '--------------------CONTROLLO COERENZA PIVA SACOD-------------
                'Dato che il reinnesco non supporta il multicentro, e ancore
                'occorre gestirlo anche direttammente nell'oggetto operazione,
                'verifico che tutte le trappole e quindi l'operazione sia riferita allo stesso centro 
                'Verifico anche piva per ulteriore controllo
                'Comunque il controllo viene fatto anche quando:
                ' si preme il pulsante per generare l'impianto (che ci sia un centro nella combo selezionato): ImageButton_MostraTrappole_Click
                ' quando si genera la lista reinneschi per l'oggetto operazione da salvare: Genera_Lista_Reinneschi_Nella_OperazioneColturale_Da_TabellaTrappole_X_Reinnesco
                'quando si salva l'operazione tramite IO utility OperazioneColturale_ToFrom_AgendaDB: si controlla che operazione.sa_cod non sia 0 e che destinazioni piva e sacod corrispondano a quelli dell'operazione
                If Movimento_Destinazione.Piva <> Operazione_Specifica.Piva Then
                    messaggiErrore.add(" Piva operazione e piva impianto destinazione non corrispondono ")
                    Return False
                End If
                If Movimento_Destinazione.Sa_Cod <> Operazione_Specifica.Sa_Cod Then
                    messaggiErrore.add(" Centro operazione e centro impianto destinazione non corrispondono, questa operazione non gestisce il multicentro ")
                    Return False
                End If
                '--------------------CONTROLLO FINE-------------
                Movimento_Destinazione.Appezza = Operazione_Specifica.Reinneschi(i).Trappola.Impianto_Di_Installazione.Appezza
                Movimento_Destinazione.Id_Destinazione = Operazione_Specifica.Reinneschi(i).Trappola.Impianto_Di_Installazione.ID_Reg

                Movimento_Destinazione.Qta = 0
                Movimento_Destinazione.Qta2 = Operazione_Specifica.Reinneschi(i).Trappola.Impianto_Di_Installazione.Qta2

                If xCalcolo_QD_SuperficieTotale <> 0 Then
                    Movimento_Destinazione.QuotaDistribuzione = Movimento_Destinazione.Qta2 / xCalcolo_QD_SuperficieTotale
                End If

                Movimento_Destinazione.BaseCode = Agenda.BaseCode
                Movimento_Destinazione.TopCode = Agenda.TopCode


                '------------------------------------------------
                '----- MOVIMENTO DETTAGLIO TECNICO 
                '------------------------------------------------

                Dim Movimento_Dettaglio_Tecnico As New Movimento_Dettaglio_Tecnico

                Select Case CInt(Operazione_Specifica.Lav_Cod)

                    Case LAVCOD_REINNESCO_TRAPPOLE
                        '------------------------------------------------
                        '----- MOVIMENTO DETTAGLIO TECNICO X reinnesco TRAPPOLE
                        '------------------------------------------------
                        Movimento_Dettaglio_Tecnico.Id_Agenda = Id_Agenda
                        Movimento_Dettaglio_Tecnico.Piva = Operazione_Specifica.Piva
                        Movimento_Dettaglio_Tecnico.Sa_Cod = Operazione_Specifica.Sa_Cod
                        Movimento_Dettaglio_Tecnico.Data = Operazione_Specifica.DataOperazione
                        Movimento_Dettaglio_Tecnico.Ditta_cod = Operazione_Specifica.Reinneschi(i).Trappola.Ditta_ID
                        Movimento_Dettaglio_Tecnico.Dose = Operazione_Specifica.Reinneschi(i).NumeroReinneschi
                        Movimento_Dettaglio_Tecnico.Sigla_av = Operazione_Specifica.Reinneschi(i).Trappola.Avversita_Sigla
                        Movimento_Dettaglio_Tecnico.Trap_num = Operazione_Specifica.Reinneschi(i).Trappola.ID
                        Movimento_Dettaglio_Tecnico.Freatimetro = Operazione_Specifica.Reinneschi(i).Trappola.ID_Personalizzato
                        Movimento_Dettaglio_Tecnico.Inn1_data = Operazione_Specifica.DataOperazione
                        Movimento_Dettaglio_Tecnico.Av_Cod = Operazione_Specifica.Reinneschi(i).Trappola.Avversita_ID
                        Movimento_Dettaglio_Tecnico.dett_cod = enum_UnitaMisura.Numero_Trappole
                        Movimento_Dettaglio_Tecnico.Data_Ril = Operazione_Specifica.DataOperazione
                    Case LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE
                        '------------------------------------------------
                        '----- MOVIMENTO DETTAGLIO TECNICO X rilievo
                        '------------------------------------------------
                        Movimento_Dettaglio_Tecnico.Id_Agenda = Id_Agenda
                        Movimento_Dettaglio_Tecnico.Piva = Operazione_Specifica.Piva
                        Movimento_Dettaglio_Tecnico.Sa_Cod = Operazione_Specifica.Sa_Cod
                        Movimento_Dettaglio_Tecnico.Data = Operazione_Specifica.DataOperazione

                        'diversi
                        Movimento_Dettaglio_Tecnico.Ditta_cod = 0
                        Movimento_Dettaglio_Tecnico.Dose = 0
                        Movimento_Dettaglio_Tecnico.Qta_Ril = Operazione_Specifica.Reinneschi(i).NumeroReinneschi
                        Movimento_Dettaglio_Tecnico.Id_Insetto = Operazione_Specifica.Reinneschi(i).Trappola.Avversita_ID

                        Movimento_Dettaglio_Tecnico.Sigla_av = Operazione_Specifica.Reinneschi(i).Trappola.Avversita_Sigla
                        Movimento_Dettaglio_Tecnico.Trap_num = Operazione_Specifica.Reinneschi(i).Trappola.ID

                        'diverso
                        Movimento_Dettaglio_Tecnico.Freatimetro = 0

                        'sarebbe corretto null
                        Movimento_Dettaglio_Tecnico.Inn1_data = Operazione_Specifica.DataOperazione

                        Movimento_Dettaglio_Tecnico.Av_Cod = Operazione_Specifica.Reinneschi(i).Trappola.Avversita_ID
                        Movimento_Dettaglio_Tecnico.dett_cod = enum_UnitaMisura.Numero_Trappole
                        Movimento_Dettaglio_Tecnico.Data_Ril = Operazione_Specifica.DataOperazione
                    Case Else
                        Throw New NotImplementedException
                End Select




                Movimento_Dettaglio_Tecnico.BaseCode = Agenda.BaseCode
                Movimento_Dettaglio_Tecnico.TopCode = Agenda.TopCode

                Movimento_Dettaglio.Movimenti_Dettagli_Tecnici.Add(Movimento_Dettaglio_Tecnico)



                'aggiungo il movimento destinazione al movimento detaglio
                Movimento_Dettaglio.Movimenti_Destinazioni.Add(Movimento_Destinazione)

                'aggiungo il movimento dettaglio al movimento
                Movimento_OperazioneColturale.Movimenti_Dettagli.Add(Movimento_Dettaglio)


            Next


            'aggiungo il movimento rilievo in campo-installazione trappola all'operazione agenda
            Agenda.Movimenti.Add(Movimento_OperazioneColturale)
            Return True

        End Function


        Private Function Crea_Agenda_Movimento_InstallazioneTrappole_CattureMassa(ByVal Id_Agenda As Integer, ByRef inneschiNumTotale As Integer, ByRef Agenda As AgronicaCoreModello.OperazioneAgenda_Temp.Operazione_Agenda, ByRef messaggiErrore As AgronicaCoreUtility.Messages_Str) As Boolean

        End Function


        Private Function Crea_Agenda_Movimento_Confusione_Disorientamento_Sessuale(ByVal Id_Agenda As Integer, ByRef Agenda As AgronicaCoreModello.OperazioneAgenda_Temp.Operazione_Agenda, ByRef messaggiErrore As AgronicaCoreUtility.Messages_Str) As Boolean


        End Function


        Private Function Crea_Agenda_Movimento_Scarico(ByVal Id_Agenda As Integer, ByRef Agenda As AgronicaCoreModello.OperazioneAgenda_Temp.Operazione_Agenda) As Boolean


            'gestione del magazzino esterno, in quel caso devo usare quello predefinito per la creazione del movimento di scarico e non quello esterno
            Dim sa_cod_magazzino As String = ""
            Dim fabbricatox_Cod As String = ""

            If Not IsNothing(Operazione.Magazzino) Then

                Dim magazzinoEsterno As Boolean = True
                If Operazione.Magazzino.Piva <> Agenda.Piva Then
                    'se magazzino è della azienda padre
                    magazzinoEsterno = True
                    Dim sa_cod_magazzino_predefinito_azienda As Integer
                    Dim fabbricatox_Cod_magazzino_predefinito_azienda As Integer
                    Dim fabbricati As New AgronicaCoreAnagrafeDAL.Fabbricati_R()
                    fabbricati.Ricava_PrimoMagazzino_Impresa(Agenda.Piva, sa_cod_magazzino_predefinito_azienda, fabbricatox_Cod_magazzino_predefinito_azienda, objParametri)
                    sa_cod_magazzino = sa_cod_magazzino_predefinito_azienda
                    fabbricatox_Cod = fabbricatox_Cod_magazzino_predefinito_azienda

                Else
                    'se magazzino è quello dell'azienda
                    magazzinoEsterno = False
                    sa_cod_magazzino = Operazione.Magazzino.Sa_Cod
                    fabbricatox_Cod = Operazione.Magazzino.Fabbricato_Cod

                End If

            End If

            Select Case CInt(Operazione.Lav_Cod)
                Case LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA
                    'If Crea_Agenda_Movimento_Scarico_Trappole_CattureMassa(inneschiNumTotale, Agenda) Then
                    '    Return true
                    'End If

                Case LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE
                    'If Crea_Agenda_Movimento_Scarico_Confusione_Disorientamento_Sessuale(Agenda) Then
                    '    Return true
                    'End If

                Case LAVCOD_REINNESCO_TRAPPOLE, LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE
                    Return Crea_Agenda_Movimento_Scarico_Reinnesco_RilievoAvv_Trappola(Id_Agenda, Agenda, sa_cod_magazzino, fabbricatox_Cod)

                Case Else
                    Throw New NotImplementedException
            End Select

            Return False
        End Function

        Private Function Crea_Agenda_Movimento_Scarico_Reinnesco_RilievoAvv_Trappola(ByVal Id_Agenda As Integer, Agenda As AgronicaCoreModello.OperazioneAgenda_Temp.Operazione_Agenda, ByVal sa_cod_magazzino As String, ByVal fabbricatox_Cod As String) As Boolean

            Dim Movimento_Dettaglio_Contabilizzato_Magazzino As Integer = 1
            Dim Movimento_Dettaglio_Pendente_Magazzino As Integer = 4

            Dim Operazione_Specifica As Operazione_Colturale.Aggiornamento_Trappole.Reinnesco_Trappole =
                DirectCast(Operazione, Operazione_Colturale.Aggiornamento_Trappole.Reinnesco_Trappole)

            Dim Movimento_Scarico As New Movimento
            Movimento_Scarico.Id_Agenda = Id_Agenda
            Movimento_Scarico.Piva = Operazione.Piva
            Movimento_Scarico.Sa_Cod = sa_cod_magazzino
            Movimento_Scarico.Data = Operazione.DataOperazione
            Movimento_Scarico.Lav_Cod = Operazione.Lav_Cod
            Movimento_Scarico.Cau_Mov = CAU_SCARICO
            Movimento_Scarico.Mov_Desc = "Scarico Magazzino"

            Movimento_Scarico.BaseCode = Agenda.BaseCode
            Movimento_Scarico.TopCode = Agenda.TopCode


            '------------------------------------------------
            '----- MOVIMENTI DETTAGLI SCARICO INNESCO
            '------------------------------------------------
            'aggiungo una operazione MOVIMENTI DETTAGLI per ciascun reinnesco
            For i = 0 To Operazione_Specifica.Reinneschi.Count - 1

                Dim Movimento_Dettaglio_Scarico_Innesco As New Movimento_Dettaglio
                Movimento_Dettaglio_Scarico_Innesco.Id_Agenda = Id_Agenda
                Movimento_Dettaglio_Scarico_Innesco.Piva = Operazione.Piva
                Movimento_Dettaglio_Scarico_Innesco.Sa_Cod = sa_cod_magazzino
                Movimento_Dettaglio_Scarico_Innesco.Data = Operazione.DataOperazione
                Movimento_Dettaglio_Scarico_Innesco.Elem_Cod = INNESCHI
                Movimento_Dettaglio_Scarico_Innesco.Pro_Cod = Operazione_Specifica.Reinneschi(i).Trappola.Avversita_ID
                Movimento_Dettaglio_Scarico_Innesco.Mov_Det_Des = "Scarico di Inneschi Reinneschi Trappole"
                Movimento_Dettaglio_Scarico_Innesco.Udm_Cod = enum_UnitaMisura.Numero_Inneschi
                Movimento_Dettaglio_Scarico_Innesco.Qta = Operazione_Specifica.Reinneschi(i).NumeroReinneschi
                Movimento_Dettaglio_Scarico_Innesco.Contabilizzato = Movimento_Dettaglio_Contabilizzato_Magazzino
                Movimento_Dettaglio_Scarico_Innesco.Pendente = Movimento_Dettaglio_Pendente_Magazzino
                'Movimento_Dettaglio_Scarico_Innesco.Lav_Cod = Operazione.Lav_Cod
                Movimento_Dettaglio_Scarico_Innesco.Cau_Mov = CAU_SCARICO


                '------------------------------------------------
                '----- MOVIMENTI DESTINAZIONI SCARICO INNESCO
                '------------------------------------------------
                Dim Movimento_Destinazione_Scarico_Innesco As New Movimento_Destinazione
                Movimento_Destinazione_Scarico_Innesco.Id_Agenda = Id_Agenda
                Movimento_Destinazione_Scarico_Innesco.Data = Operazione.DataOperazione
                Movimento_Destinazione_Scarico_Innesco.Piva = Operazione.Piva
                Movimento_Destinazione_Scarico_Innesco.Sa_Cod = sa_cod_magazzino
                Movimento_Destinazione_Scarico_Innesco.Id_Destinazione = fabbricatox_Cod
                Movimento_Destinazione_Scarico_Innesco.Tipo = MAGAZZINO
                Movimento_Destinazione_Scarico_Innesco.Qta = Operazione_Specifica.Reinneschi(i).NumeroReinneschi

                Movimento_Destinazione_Scarico_Innesco.BaseCode = Agenda.BaseCode
                Movimento_Destinazione_Scarico_Innesco.TopCode = Agenda.TopCode

                'INNESCHI
                '-------------- aggiungo il movimento destinazione al movimento dettaglio
                Movimento_Dettaglio_Scarico_Innesco.Movimenti_Destinazioni.Add(Movimento_Destinazione_Scarico_Innesco)
                '-------------- aggiungo il movimento dettaglio allo scarico
                Movimento_Scarico.Movimenti_Dettagli.Add(Movimento_Dettaglio_Scarico_Innesco)

            Next




            '-------------- aggiungo il movimento scarico all'agenda
            Agenda.Movimenti.Add(Movimento_Scarico)

            Return True


        End Function


#End Region

        '-------------------------------------------------------------------------------------------
        '-------------------------------------------------------------------------------------------
        '-------------------------------------------------------------------------------------------
        '-------------------------------------------------------------------------------------------
        '-------------------------------------------------------------------------------------------
        '-------------------------------------------------------------------------------------------

#Region "Trappole Leggi"

        Private Sub Leggi_MovimentoAgenda_Reinnesco_Rilievo_Trappole(movimento As Movimento)

            Dim Operazione_Specifica As Operazione_Colturale.Aggiornamento_Trappole.Reinnesco_Trappole =
                DirectCast(Operazione, Operazione_Colturale.Aggiornamento_Trappole.Reinnesco_Trappole)



            '------------------------------------------------------------------------------------------
            '------------------------------------------------------------------------------------------
            '------------------------------------------------------------------------------------------
            Operazione_Specifica.DataOperazione = movimento.Data
            Operazione_Specifica.Nota = movimento.Mov_Desc
            '------------------------------------------------------------------------------------------
            '------------------------------------------------------------------------------------------
            '------------------------------------------------------------------------------------------


            'MOVIMENTO_DETTAGLIO TECNICO (dovrebbe essere nothing)
            If Not IsNothing(movimento.Movimenti_Dettagli_Tecnici) Then

                For j = 0 To movimento.Movimenti_Dettagli_Tecnici.Count - 1

                    'controllo corrispondeza piva con agenda
                    If Operazione.Piva <> movimento.Piva Then
                        Throw New ApplicationException
                    End If

                    'non dovrebbe essercene nessuno
                    Throw New NotImplementedException
                Next

            End If



            '---------------------
            '----MOVIMENTI_DETTAGLI
            '---------------------
            'uno per ciascun reinnesco
            If Not IsNothing(movimento.Movimenti_Dettagli) Then

                For j = 0 To movimento.Movimenti_Dettagli.Count - 1

                    'controllo corrispondeza piva con agenda
                    'sa cod è uguale ma potrebbe cambiare se...
                    If Operazione.Piva <> movimento.Movimenti_Dettagli(j).Piva Then
                        Throw New ApplicationException
                    End If

                    'Movimento_Dettaglio.Elem_Cod = TRAPPOLE
                    'implicito ma lo controllo dato che lo riscrivo
                    If TRAPPOLE <> movimento.Movimenti_Dettagli(j).Elem_Cod Then
                        Throw New ApplicationException
                    End If

                    'Movimento_Dettaglio.Udm_Cod = unità_misura_num_trappole ' unità di misura n.trappole
                    'implicito
                    If enum_UnitaMisura.Numero_Trappole <> movimento.Movimenti_Dettagli(j).Udm_Cod Then
                        Throw New ApplicationException
                    End If

                    Dim TrappolaCodiceProdotto As Integer = movimento.Movimenti_Dettagli(j).Pro_Cod

                    '---------------------
                    'MOVIMENTO_DESTINAZIONI
                    '---------------------
                    'una sola destinazione 

                    If IsNothing(movimento.Movimenti_Dettagli(j).Movimenti_Destinazioni) OrElse
                                 movimento.Movimenti_Dettagli(j).Movimenti_Destinazioni.Count <> 1 Then
                        'non devono esistere piu di 1
                        Throw New ApplicationException
                    End If
                    If (movimento.Movimenti_Dettagli(j).Qta <> 1) Then
                        Throw New ApplicationException
                    End If
                    Dim Imp_Piva As String = movimento.Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Piva
                    Dim Imp_Sa_Cod As Integer = movimento.Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Sa_Cod
                    Dim Imp_Appezza As Integer = movimento.Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Appezza
                    Dim Imp_ID_Reg As Integer = movimento.Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Id_Destinazione
                    Dim Imp_Qta2 As Integer = movimento.Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Qta2

                    'creo l'impianto di installazione della trappola
                    Dim objImpianto As New Anagrafe.Impianto_Colturale(Imp_Piva, Imp_Sa_Cod, Imp_Appezza, Imp_ID_Reg, 0, AGRODATAINIZIO, AGRODATAFINE, Imp_Qta2)

                    'per fare in modo che la master imposti le combo dei centri e specie
                    Dim objParametriAgenda As New AgronicaCoreModello.ParametriAgenda_Temp.ParametriAgenda
                    objParametriAgenda.Sa_Cod = Imp_Sa_Cod
                    objParametriAgenda.Veg_Cod = New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read().VegCod_from_PivaSaCodAppezzaIdimp(Imp_Piva, Imp_Sa_Cod, Imp_Appezza, Imp_ID_Reg, "", "", objParametri)



                    'Dim ProCod_Reinnesco As Integer = movimento.Movimenti_Dettagli(j).Pro_Cod


                    '---------------------
                    'MOVIMENTI_DETTAGLI_TECNICI
                    '---------------------
                    If IsNothing(movimento.Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici) OrElse
                                  movimento.Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici.Count <> 1 Then
                        'ce ne deve essere solo uno
                        Throw New ApplicationException
                    End If

                    '------------------------------------------------------

                    Dim TrappolaCodiceDittaTrappola As Integer = 0
                    Dim TrappolaSiglaAv As String = ""
                    Dim TrappolaAvCod As Integer = 0
                    Dim TrappolaID As Integer = 0
                    Dim TrappolaIDPersonalizzato As Integer = 0
                    Dim numerReinneschi As Decimal = 0

                    Select Case CInt(Operazione_Specifica.Lav_Cod)

                        Case LAVCOD_REINNESCO_TRAPPOLE
                            '------------------------------------------------
                            '----- MOVIMENTO DETTAGLIO TECNICO X reinnesco TRAPPOLE
                            '------------------------------------------------
                            TrappolaCodiceDittaTrappola = movimento.Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Ditta_cod
                            TrappolaSiglaAv = movimento.Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Sigla_av
                            TrappolaAvCod = movimento.Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Av_Cod
                            TrappolaID = movimento.Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Trap_num
                            TrappolaIDPersonalizzato = movimento.Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Freatimetro
                            numerReinneschi = movimento.Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Dose
                        Case LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE
                            '------------------------------------------------
                            '----- MOVIMENTO DETTAGLIO TECNICO X rilievo
                            '------------------------------------------------

                            'diversi
                            '0=movimento.Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Ditta_cod
                            '0=movimento.Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Dose
                            numerReinneschi = movimento.Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Qta_Ril
                            TrappolaAvCod = movimento.Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Id_Insetto

                            TrappolaSiglaAv = movimento.Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Sigla_av
                            TrappolaID = movimento.Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Trap_num

                            'diverso
                            '0=movimento.Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Freatimetro 

                            ''sarebbe corretto null
                            'Movimento_Dettaglio_Tecnico.Inn1_data = Operazione_Specifica.DataOperazione

                            'Movimento_Dettaglio_Tecnico.Av_Cod = Operazione_Specifica.Reinneschi(i).Trappola.Avversita_ID
                            'Movimento_Dettaglio_Tecnico.dett_cod = enum_UnitaDiMisura.Numero_Trappole
                            'Movimento_Dettaglio_Tecnico.Data_Ril = Operazione_Specifica.DataOperazione

                            TrappolaIDPersonalizzato = New AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R().TrappolaCodicePersonalizzato_from_Trap_Num_Avv(Operazione.Piva, Operazione.Sa_Cod, TrappolaID, TrappolaSiglaAv, objParametri)
                            TrappolaCodiceDittaTrappola = New AgronicaCoreMetaSchemaDAL.Trappole_R().DittaCod_from_TrapCod(TrappolaCodiceProdotto, objParametri)

                        Case Else
                            Throw New NotImplementedException
                    End Select
                    '------------------------------


                    'creo la trappola
                    Dim TrapObj As New Operazione_Colturale.Installazione_Trappole.Con_Inneschi.Trappola(
                                                                                            TrappolaID,
                                                                                            TrappolaIDPersonalizzato)
                    'aggiungo il tipo 
                    TrapObj.Prodotto_ID = TrappolaCodiceProdotto
                    TrapObj.Avversita_ID = TrappolaAvCod
                    TrapObj.Avversita_Sigla = TrappolaSiglaAv
                    TrapObj.Ditta_ID = TrappolaCodiceDittaTrappola

                    'aggiungo l'impianto
                    TrapObj.Impianto_Di_Installazione = objImpianto

                    'creo il reinnesco
                    Dim reinnescoObj As New Operazione_Colturale.Aggiornamento_Trappole.Reinnesco(TrapObj, numerReinneschi)

                    '------------------------------------------------------------------------------------------
                    '------------------------------------------------------------------------------------------
                    '------------------------------------------------------------------------------------------
                    'aggiungo il reinnesco alla lista
                    Operazione_Specifica.Reinneschi.Add(reinnescoObj)
                    '------------------------------------------------------------------------------------------
                    '------------------------------------------------------------------------------------------
                    '------------------------------------------------------------------------------------------

                Next

            Else
                'ci seve essere almeno un movimento dettaglio per l'ìinstallazione di una trappola in un appezzamento
                Throw New ApplicationException
            End If


        End Sub


        Private Sub LeggiMovimentoAgenda_InstallazioneTrappole_CattureMassa(movimento As Movimento)

            Throw New NotImplementedException

        End Sub

        Private Sub LeggiMovimentoAgenda_Confusione_Disorientamento_Sessuale(movimento As Movimento)

            Throw New NotImplementedException

        End Sub


        Private Sub Leggi_MovimentoAgenda_Scarico_Trappole_CattureMassa(movimentoDet As Movimento_Dettaglio, ByVal Conteggio As Integer)

            If Conteggio > 2 Then
                Throw New NotImplementedException 'non previsto
            End If
            Dim scaricoTrappola As Integer = 0
            Dim scaricoInnesco As Integer = 0
            Select Case movimentoDet.Elem_Cod
                Case TRAPPOLE
                    scaricoTrappola += 1
                    If scaricoTrappola > 1 Then
                        Throw New NotImplementedException 'non previsto
                    End If
                    LeggiMovimentoAgenda_ScaricoTrappole(movimentoDet)
                Case INNESCHI
                    scaricoInnesco += 1
                    If scaricoInnesco > 1 Then
                        Throw New NotImplementedException 'non previsto
                    End If
                    LeggiMovimentoAgenda_ScaricoInneschi(movimentoDet)
                Case Else   'non previsto
                    Throw New NotImplementedException
            End Select
        End Sub

        Private Sub Leggi_MovimentoAgenda_Scarico_ConfusDisorientSess(movimentoDet As Movimento_Dettaglio, ByVal Conteggio As Integer)
            If Conteggio > 1 Then
                Throw New NotImplementedException 'non previsto
            End If
            Select Case movimentoDet.Elem_Cod
                Case TRAPPOLE
                    LeggiMovimentoAgenda_ScaricoTrappole(movimentoDet)
                Case Else   'non previsto
                    Throw New NotImplementedException
            End Select
        End Sub

        Private Sub Leggi_MovimentoAgenda_Scarico_Reinnesco_Rilievo_Trappole(movimentoDet As Movimento_Dettaglio, ByVal Conteggio As Integer)
            'If Conteggio > num trap Then
            '    Throw New NotImplementedException 'non previsto
            'End If
            Select Case movimentoDet.Elem_Cod
                Case INNESCHI
                    LeggiMovimentoAgenda_ScaricoInneschi(movimentoDet)
                Case Else   'non previsto
                    Throw New NotImplementedException
            End Select
        End Sub

        Private Sub LeggiMovimentoAgenda_ScaricoTrappole(ByRef movimentoDet As Movimento_Dettaglio)
            Dim Destinazione As Integer
            Dim SaCodDestinazione As Integer
            Dim PivaDestinazione As String
            Dim trappoleNumTotale As Integer = movimentoDet.Qta
            ''Movimento_Dettaglio_Scarico_Trappola.Data = Data
            'Movimento_Dettaglio_Scarico_Trappola.Elem_Cod = TRAPPOLE
            'Movimento_Dettaglio_Scarico_Trappola.Pro_Cod = cmb_Trappola.SelectedItem.Value 'Dispenser_Cod
            ''Movimento_Dettaglio_Scarico_Trappola.Mov_det_des = "Scarico di Trappole"
            ''Movimento_Dettaglio_Scarico_Trappola.Udm_Cod = enum_UnitaDiMisura.Numero_Trappole
            'Movimento_Dettaglio_Scarico_Trappola.Qta = CInt(Txt_NumeroTrappole.Text)
            ''Movimento_Dettaglio_Scarico_Trappola.Contabilizzato = Movimento_Dettaglio_Contabilizzato_Magazzino
            ''Movimento_Dettaglio_Scarico_Trappola.Pendente = Movimento_Dettaglio_Pendente_Magazzino
            'Movimento_Dettaglio_Scarico_Trappola.Lav_Cod = Lav_Cod
            'Movimento_Dettaglio_Scarico_Trappola.Cau_Mov = CAU_SCARICO

            ''controllo coerensza numero scarichi
            'If trappoleNumTotale <> Operazione.numeroTrappole Then
            '    Throw New NotImplementedException
            'End If

            If Not IsNothing(movimentoDet.Movimenti_Destinazioni) Then
                For x = 0 To movimentoDet.Movimenti_Destinazioni.Count - 1
                    ''Destinazione = movimento.Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Destinazione
                    ''SaCodDestinazione = movimento.Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Sa_Cod
                    ''objParametriAgenda.Fabbricato = Destinazione.ToString & "|" & SaCodDestinazione.ToString
                    ''Movimento_Destinazione_Scarico_Trappola.Data = Data
                    'Movimento_Destinazione_Scarico_Trappola.Id_Destinazione = Split(objParametriAgenda.Fabbricato, "|")(0)
                    ''Movimento_Destinazione_Scarico_Trappola.Tipo = MAGAZZINO
                    'Movimento_Destinazione_Scarico_Trappola.Qta = CInt(Txt_NumeroTrappole.Text)

                    Destinazione = movimentoDet.Movimenti_Destinazioni(x).Id_Destinazione
                    PivaDestinazione = movimentoDet.Movimenti_Destinazioni(x).Piva

                    'agenda vecchia crea movimenti scarico anche se non ci sono movimenti magazzino reali,
                    'per capire che una operazione di movimento scarico non  reale la destinazione del movimento è 0
                    'in questi caso ignoro il movimento
                    If Destinazione = 0 Then
                        Exit For
                    End If

                    SaCodDestinazione = movimentoDet.Movimenti_Destinazioni(x).Sa_Cod

                    Operazione.Magazzino = New Anagrafe.Magazzino(PivaDestinazione, SaCodDestinazione, Destinazione)
                Next
            End If
        End Sub

        Private Sub LeggiMovimentoAgenda_ScaricoInneschi(ByRef movimentoDet As Movimento_Dettaglio)
            Dim Destinazione As Integer
            Dim SaCodDestinazione As Integer
            Dim PivaDestinazione As String
            Dim inneschiNumTotale As Integer = movimentoDet.Qta
            'Movimento_Dettaglio_Scarico_Innesco.Elem_Cod = INNESCHI
            'Movimento_Dettaglio_Scarico_Innesco.Pro_Cod = Me.cmb_Avversita.SelectedItem.Value
            ''Movimento_Dettaglio_Scarico_Innesco.Mov_det_des = "Scarico di Inneschi Installazione Trappole"
            'Movimento_Dettaglio_Scarico_Innesco.Udm_Cod = enum_UnitaDiMisura.Numero_Inneschi
            'Movimento_Dettaglio_Scarico_Innesco.Qta = inneschiNumTotale
            ''Movimento_Dettaglio_Scarico_Innesco.Contabilizzato = 1
            ''Movimento_Dettaglio_Scarico_Innesco.Pendente = 4
            'Movimento_Dettaglio_Scarico_Innesco.Lav_Cod = Lav_Cod
            'Movimento_Dettaglio_Scarico_Innesco.Cau_Mov = CAU_SCARICO

            ''controllo coerensza numero scarichi
            'If inneschiNumTotale <> Operazione.numeroInneschi Then
            '    Throw New NotImplementedException
            'End If

            If Not IsNothing(movimentoDet.Movimenti_Destinazioni) Then
                For x = 0 To movimentoDet.Movimenti_Destinazioni.Count - 1
                    ''Movimento_Destinazione_Scarico_Trappola.Data = Data
                    ''Movimento_Destinazione_Scarico_Innesco.Piva = objParametriAgenda.Piva
                    ''Movimento_Destinazione_Scarico_Innesco.Sa_Cod = Split(objParametriAgenda.Fabbricato, "|")(1)
                    ''Movimento_Destinazione_Scarico_Innesco.Id_Destinazione = Split(objParametriAgenda.Fabbricato, "|")(0)
                    ''Movimento_Destinazione_Scarico_Innesco.Tipo = MAGAZZINO
                    ''Movimento_Destinazione_Scarico_Innesco.Qta = inneschiNumTotale
                    ''Movimento_Destinazione_Scarico_Innesco.BaseCode = BaseCode
                    ''Movimento_Destinazione_Scarico_Innesco.TopCode = TopCode

                    Destinazione = movimentoDet.Movimenti_Destinazioni(x).Id_Destinazione

                    'agenda vecchia crea movimenti scarico anche se non ci sono movimenti magazzino reali,
                    'per capire che una operazione di movimento scarico non  reale la destinazione del movimento è 0
                    'in questi caso ignoro il movimento
                    If Destinazione = 0 Then
                        Exit For
                    End If

                    SaCodDestinazione = movimentoDet.Movimenti_Destinazioni(x).Sa_Cod
                    PivaDestinazione = movimentoDet.Movimenti_Destinazioni(x).Piva
                    Operazione.Magazzino = New Anagrafe.Magazzino(PivaDestinazione, SaCodDestinazione, Destinazione)
                Next
            End If

        End Sub


#End Region




        'Questa parte è stata copiata da utility operazioni per salvare le operazioni di scarico e il ddt e i riferimnenti nel caso di utilizzo di magazzino esterno
#Region "Gestione Magazzino Azienda Padre"


        Public Sub Gestisci_Magazzino_Aziendale(ByVal Agenda As Operazione_Agenda, ByVal Id_Agenda As Integer)



            '--------------------------------------------------------------------------------
            '--------- CREAZIONE SCRITTURA OGGETTI PER MOVIMENTI MAGAZZINO AZIENDALE   ------
            '--------------------------------------------------------------------------------

            ' se l'agenda ha dei movimenti del magazzino e se il magazzino selezionato è quello dell'impresa padre
            'devo creare lo scarico dal padre e il ddt
            If Not IsNothing(Operazione.Magazzino) AndAlso Operazione.Magazzino.Piva <> Agenda.Piva Then


                Dim DT_Dettagli As DataTable = New AgronicaCoreXML.XML_Contab().DtForXml_Genera_MovimentiDettagli
                Dim DT_Destinazioni As DataTable = New AgronicaCoreXML.XML_Contab().DtForXml_Genera_MovDestinazioni

                'prendo il magazzino con sacod piu basso, devo modificare anche lo scarico dell'agenda, anche li se sono nel magazzino del
                'centro padre devo prendere lo stesso magazzino di default
                Dim sa_cod_magazzino_predefinito_azienda As Integer
                Dim Fabbricato_Cod_magazzino_predefinito_azienda As Integer
                Dim fabbricati As New AgronicaCoreAnagrafeDAL.Fabbricati_R()
                fabbricati.Ricava_PrimoMagazzino_Impresa(Agenda.Piva, sa_cod_magazzino_predefinito_azienda, Fabbricato_Cod_magazzino_predefinito_azienda, objParametri)
                If sa_cod_magazzino_predefinito_azienda = 0 OrElse Fabbricato_Cod_magazzino_predefinito_azienda = 0 Then
                    Throw New Exception("L'azienda non ha un magazzino, per utilizzare un magazzino esterno è necessario avere un magazzino aziendale.")
                End If

                Dim objRif As New AgronicaCoreContabDAL.Mov_Det_Riferimenti_W
                Dim objMov As New AgronicaCoreContabDAL.Movimenti_R
                Dim objAgenda_W As New AgronicaCoreContabBIZ.Agenda_W
                Dim flag_insert As Boolean = False

                Dim Piva_Magazzino As String = Operazione.Magazzino.Piva
                Dim Sa_Cod_Magazzino As String = Operazione.Magazzino.Sa_Cod
                Dim Fabbricato_Cod_Magazzino As String = Operazione.Magazzino.Fabbricato_Cod


                Dim MovimentiDet_Scarico As New List(Of Movimento_Dettaglio)

                For Each movimentoagenda As Movimento In Agenda.Movimenti
                    If movimentoagenda.Cau_Mov = CAU_SCARICO Then
                        MovimentiDet_Scarico = movimentoagenda.Movimenti_Dettagli
                    End If
                Next


                For Each movdet As Movimento_Dettaglio In MovimentiDet_Scarico


                    Dim Elem_Cod As Integer = movdet.Elem_Cod ' SEMENTI 'fisso
                    Dim Pro_Cod As Integer = movdet.Pro_Cod '0 se magazzino, sem_cod altrimenti
                    Dim Mat_Cod As Integer = movdet.Mat_Cod 'Mat_Cod siempre
                    Dim Udm_Cod As Integer = movdet.Udm_Cod
                    Dim lotto As String = movdet.Lotto
                    Dim Qta As Decimal = movdet.Qta
                    Dim Data As Date = movdet.Data
                    Dim Tipo As Integer = MAGAZZINO

                    Dim Descrizione_Prodotto As String = New AgronicaCoreAnagrafeDAL.Materie_Prime_R().MatDes_from_MatCod("", 0, Mat_Cod, "", "", "", objParametri)


                    '--------------------------------------------------------------------------------
                    '--------- SCRITTURA MOVIMENTO SCARICO AZIENDA ESTERNA   ------------------------
                    '--------------------------------------------------------------------------------
                    Dim str_MovScarico As String = ""
                    str_MovScarico = Crea_Scarico_Da_Az_Padre(Agenda, Descrizione_Prodotto, Elem_Cod, Mat_Cod, Udm_Cod, Qta, Data)

                    '1) scrittura movimento scarico magazzino impresa padre
                    Dim OUTPUT_ID_Agenda_scarico As Integer
                    If str_MovScarico <> "" Then
                        flag_insert = False
                        flag_insert = objAgenda_W.Agenda_Scrivi(str_MovScarico,
                                                              OUTPUT_ID_Agenda_scarico,
                                                                0,
                                                                5,
                                                                0,
                                                                "",
                                                                objParametri)

                    Else
                        Throw New Exception("<b>Non è riuscita la creazione dello scarico dall'azienda padre </b> <br>")
                    End If
                    If OUTPUT_ID_Agenda_scarico = 0 OrElse Not flag_insert Then
                        Throw New Exception("<b>Non è riuscita la creazione dello scarico dall'azienda padre </b> <br>")
                    End If

                    '--------------------------------------------------------------------------------
                    '--------- SCRITTURA RIFERIMENTO CON SCARICO AZIENDA ESTERNA  ------------------
                    '--------------------------------------------------------------------------------
                    'scrittura riferimenti x collegare le operazioni
                    If OUTPUT_ID_Agenda_scarico <> 0 Then

                        'Ricavo id movimento scarico che mi servirà per collegare i movimenti nella tabella riferimenti
                        Dim IdMov_Scarico As Integer
                        IdMov_Scarico = objMov.IdMov_from_IdAgendaCauMov(Piva_Magazzino,
                                                                       OUTPUT_ID_Agenda_scarico,
                                                                       CAU_SCARICO,
                                                                       "",
                                                                        objParametri)

                        Dim IdMov_Semina As Integer
                        IdMov_Semina = objMov.IdMov_from_IdAgendaCauMov(Agenda.Piva,
                                                                       Id_Agenda,
                                                                       enum_Agenda_Causali.LAVORAZIONE,
                                                                       "",
                                                                        objParametri)

                        '2) scrittura del ddt fittizio di carico del magazzino dell'impresa che fa la semina
                        ' semina agganciata a scarico magazzino
                        'magazzino non è quello dell'impresa, ma di un'impresa padre
                        flag_insert = False
                        flag_insert = objRif.Scrivi(Agenda.Piva,
                                     Agenda.Sa_Cod,
                                     Id_Agenda,
                                      IdMov_Semina,
                                     -1,
                                     Operazione.Lav_Cod,
                                     Operazione.Cau_Mov,
                                     Piva_Magazzino,
                                     Sa_Cod_Magazzino,
                                     OUTPUT_ID_Agenda_scarico,
                                    IdMov_Scarico,
                                    -1,
                                     LAVCOD_SCARICO,
                                     CAU_SCARICO,
                                     0,
                                     Agenda.Data,
                                     AGRODATAFINE,
                                     objParametri)

                        If Not flag_insert Then
                            Throw New Exception("<b>Non è riuscita l'aggancio dell' operazione di agenda con lo scarico dal magazzino dell'azienda padre </b> <br>")
                        End If

                    Else
                        Throw New Exception("<b>Non è riuscita l'aggancio delle operazioni di agenda di scarico e DDT </b> <br>")
                    End If



                    '----------------------------------------------------------------------------------------------------------------
                    ' Aggiungo righe al DT dei dettagli da aggiungere all'unico DDT
                    Dim Mov_Det_Des As String = "DDT fittizio per " & Operazione.Lav_Cod & ": " & Descrizione_Prodotto
                    Aggiungi_Righe_Dettagli_Destinazioni_alle_Tabelle_DDT(Agenda, DT_Dettagli, DT_Destinazioni, sa_cod_magazzino_predefinito_azienda, Fabbricato_Cod_magazzino_predefinito_azienda, Elem_Cod, Pro_Cod, Mat_Cod, Udm_Cod, lotto, Qta, Data, Mov_Det_Des)
                    '----------------------------------------------------------------------------------------------------------------


                Next


                '----------------------------------------------------------------------------------
                '--------- SCRITTURA DDT FITTIZIO--------------------------------------------------
                '----------------------------------------------------------------------------------
                '----3) creazione del ddt Fittizio per il carico di magazzino dell'impresa e aggiunta degli n dettagli
                Dim str_DDTricevutoFittizio As String
                str_DDTricevutoFittizio = Crea_DDTricevutoFittizio(Agenda, DT_Dettagli, DT_Destinazioni)
                Dim OUTPUT_ID_Agenda_ddt As Integer
                flag_insert = False
                If str_DDTricevutoFittizio <> "" Then
                    flag_insert = objAgenda_W.Agenda_Scrivi(str_DDTricevutoFittizio,
                                                         OUTPUT_ID_Agenda_ddt,
                                                           0,
                                                           5,
                                                           0,
                                                           "",
                                                           objParametri)
                Else
                    Throw New Exception("<b>Non è riuscita la creazione del DDT dall'azienda padre </b> <br>")
                End If
                If OUTPUT_ID_Agenda_ddt = 0 OrElse Not flag_insert Then
                    Throw New Exception("<b>Non è riuscita la creazione del DDT dall'azienda padre </b> <br>")
                End If

                '--------------------------------------------------------------------------------
                '--------- SCRITTURA RIFERIMENTO DDT FITTIZIO----------------  ------------------
                '--------------------------------------------------------------------------------
                '4) scrittura riferimenti x collegare le operazioni
                If OUTPUT_ID_Agenda_ddt <> 0 Then


                    Dim IdMov_Semina As Integer
                    IdMov_Semina = objMov.IdMov_from_IdAgendaCauMov(Agenda.Piva,
                                                                   Id_Agenda,
                                                                   enum_Agenda_Causali.LAVORAZIONE,
                                                                   "",
                                                                    objParametri)


                    '3.2) semina agganciata a dtt ricevuto
                    'id_mov = id_mov semina
                    'id_mov_rif = id_mov movimento contabile del ddt (quello con cau_mov = 4000)
                    Dim IdMov_Cont_DDT As Integer

                    IdMov_Cont_DDT = objMov.IdMov_from_IdAgendaCauMov(Agenda.Piva,
                                                                   OUTPUT_ID_Agenda_ddt,
                                                                   CAU_REGISTRAZIONI,
                                                                   "",
                                                                    objParametri)
                    flag_insert = False
                    flag_insert = objRif.Scrivi(Agenda.Piva,
                                                  Agenda.Sa_Cod,
                                                  Id_Agenda,
                                                  IdMov_Semina,
                                                 -1,
                                                  Operazione.Lav_Cod,
                                                  Operazione.Cau_Mov,
                                                  Agenda.Piva,
                                                  0,
                                                  OUTPUT_ID_Agenda_ddt,
                                                 IdMov_Cont_DDT,
                                                 -1,
                                                  LAVCOD_BOLLA_RICEVUTA,
                                                  CAU_REGISTRAZIONI,
                                                  0,
                                                  Agenda.Data,
                                                  AGRODATAFINE,
                                                  objParametri)

                    If Not flag_insert Then
                        Throw New Exception("<b>Non è riuscita l'aggancio dell' operazione di agenda con il DDT </b> <br>")
                    End If
                Else
                    Throw New Exception("<b>Non è riuscita l'aggancio delle operazioni di agenda di scarico e DDT </b> <br>")
                End If



                '--------------------------------------------------------------------------------
                '---------FINE  SCRITTURA OGGETTI AGENDA  ---------------------------------------
                '--------------------------------------------------------------------------------

            End If




            '--------------------------------------------------------------------------------
            '---------CANCELLAZIONE OGGETTI PRECEDENTI IN MODIFICA --------------------------
            '--------------------------------------------------------------------------------

            'In modifica Devo controllare se l'operazione precedente utilizzava il magazzino esterno, in quel caso devo eliminare le operazioni 
            'di scarico, il ddt e i collegamenti presenti nella tabella riferimenti

            If Operazione.Tipo_Operazione = enum_TipoOperazioneDB.Modifica Then

                Dim Id_Agenda_Old As Integer = Operazione.ID_Agenda
                Dim objRif As New AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R
                Dim seminaOldConAltraImpresa As Boolean
                seminaOldConAltraImpresa = objRif.Verifica_Operazione_ConMagazzinoAltraimpresa(Agenda.Piva,
                                                                                            Agenda.Sa_Cod,
                                                                                            Id_Agenda_Old,
                                                                                            Operazione.Lav_Cod,
                                                                                            "",
                                                                                            objParametri)

                If seminaOldConAltraImpresa Then

                    'ricavo una matrice con tutti gli id agenda etc presenti nella tabella dei riferimenti
                    Dim MatriceIdAgendaOld(,) As String

                    MatriceIdAgendaOld = objRif.MatriceChiaviAgenda_MovRiferiti(Agenda.Piva,
                                                                                  Agenda.Sa_Cod,
                                                                                  Id_Agenda_Old,
                                                                                  "",
                                                                                  objParametri)


                    If Not IsNothing(MatriceIdAgendaOld) Then

                        Dim CancellataOperazione As Boolean = False
                        Dim Piva_Canc As String
                        Dim Sa_Cod_Canc As Integer
                        Dim Id_Agenda_Canc As Integer
                        Dim objAgendaScrivi As New Agenda_Operazione_Helper
                        Dim objRifCanc As New AgronicaCoreContabDAL.Mov_Det_Riferimenti_W
                        Dim objMov_R As New AgronicaCoreContabDAL.Movimenti_R


                        For i = 0 To UBound(MatriceIdAgendaOld, 2)

                            Piva_Canc = MatriceIdAgendaOld(0, i)
                            Sa_Cod_Canc = MatriceIdAgendaOld(1, i)
                            Id_Agenda_Canc = MatriceIdAgendaOld(2, i)

                            Dim cancellabile As Boolean = True
                            Dim Doc_Numero As Decimal = -1

                            If objMov_R.Esiste_Almeno_Un_Doc_Contabile_Riferito(Piva_Canc,
                                                                  Id_Agenda_Canc,
                                                                   "", Doc_Numero,
                                                                   objParametri) Then

                                If Doc_Numero <= 0 Then
                                    cancellabile = True
                                    'non è un ddt reale,m quindi posso cancellare il riferimento
                                Else
                                    'è un ddt reale quindi non lo cancello
                                    cancellabile = False
                                End If

                            Else
                                'non c'è un collegamento al ddt ma ad un magazzino, quindi posso cancellare
                                cancellabile = True
                            End If



                            'SE NON E' UN DDT BUONO, oppure è il riferimento allo scarico, ALLORA CANCELLO I RIFERIMENTI
                            If cancellabile Then

                                'cancello le operazioni collegate di scarico e ddt 
                                CancellataOperazione = objAgendaScrivi.Cancella(Piva_Canc,
                                                                                  Sa_Cod_Canc,
                                                                                  Id_Agenda_Canc, False,
                                                                                  objParametri)
                                'cancello il riferimento 
                                CancellataOperazione = objRifCanc.Cancella_byChiaveRif(Piva_Canc,
                                                                                        Sa_Cod_Canc,
                                                                                        Id_Agenda_Canc,
                                                                                        0, 0,
                                                                                        "",
                                                                                        objParametri)

                            End If


                        Next


                    End If

                End If
            End If


        End Sub


        Private Function Crea_Scarico_Da_Az_Padre(ByVal Agenda As Operazione_Agenda, ByVal Descrizione_Prodotto As String, ByVal Elem_Cod As Integer, ByVal Mat_Cod As Integer, ByVal Udm_Cod As Integer, ByVal Qta As Decimal, ByVal Data As Date) As String

            'magazzino non è quello dell'impresa, ma di un'impresa padre
            Dim Piva_Magazzino As String = Operazione.Magazzino.Piva
            Dim Sa_Cod_Magazzino As String = Operazione.Magazzino.Sa_Cod
            Dim Fabbricato_Cod_Magazzino As String = Operazione.Magazzino.Fabbricato_Cod


            'If True Then

            '    'controllo giacenza 
            '    Dim objGiacenze As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
            '    Dim Giacenza As Decimal = objGiacenze.Verifica_Giacenze(Piva_Magazzino, _
            '                                                            Sa_Cod_Magazzino, _
            '                                                            Fabbricato_Cod_Magazzino, _
            '                                                            Elem_Cod, _
            '                                                            0, _
            '                                                            Mat_Cod, _
            '                                                            0, 0, LOTTO_NONDEFINITO, _
            '                                                            0, Udm_Cod, _
            '                                                            AGRODATAINIZIO, _
            '                                                            CDate(Operazione.DataOperazione), _
            '                                                            Operazione.ID_Agenda, _
            '                                                            objParametri)
            '    If Qta > Giacenza Then

            '        Dim Messaggio_Errore As String = "ATTENZIONE!<br>La quantità di " & Descrizione_Prodotto & " <b>" & _
            '                      "</b> al <b>" & Operazione.DataOperazione & "</b> è pari a <b>" & Giacenza & " " & New AgronicaCoreMetaSchemaDAL.UnitaMisura_R().UdmDes_from_UdmCod(Udm_Cod, "", objParametri_Server) & " </b> " & " quindi non sufficiente per lo scarico!<br><br> " & vbCr

            '        Throw New Exception("<b>Non è riuscita la creazione dello scarico dall'azienda padre </b> " & Messaggio_Errore & "<br>")

            '    End If

            'End If


            Dim str_MovScarico As String

            Dim Des_Lib As String = "Scarico di magazzino " & Descrizione_Prodotto
            Dim Mov_Desc As String = "Scarico di magazzino " & Descrizione_Prodotto
            Dim Mov_Det_Des As String = "Scarico di magazzino per  " & Operazione.Lav_Cod & ": " & Descrizione_Prodotto

            Dim Log_Errori As String = ""
            Dim objXML As New AgronicaCoreXML.XML_Contab
            str_MovScarico = objXML.MacroXML_CaricoScaricoMagazzino(Log_Errori,
                                                                    Nothing,
                                                                    2,
                                                                    Piva_Magazzino,
                                                                    Sa_Cod_Magazzino,
                                                                    Fabbricato_Cod_Magazzino,
                                                                    Des_Lib,
                                                                    Data,
                                                                    AGRODATAINIZIO,
                                                                    Mov_Desc,
                                                                    Mov_Det_Des,
                                                                    Elem_Cod,
                                                                    0,
                                                                    Mat_Cod,
                                                                    0,
                                                                    0,
                                                                    "",
                                                                    0,
                                                                    Udm_Cod,
                                                                    Qta,
                                                                    0,
                                                                    0,
                                                                    0,
                                                                    0,
                                                                    0,
                                                                    0,
                                                                    enum_Pendenza.MovPendente,
                                                                    0,
                                                                    0,
                                                                    0,
                                                                    Agenda.BaseCode,
                                                                    Agenda.TopCode,
                                                                    1,
                                                                    Operazione.DataOperazione,
                                                                    objParametri.UsernameOperazione,
                                                                    0)
            If Log_Errori <> "" Then
                Throw New Exception("<b>Non è riuscita la creazione dello scarico dall'azienda padre </b> " & Log_Errori & "<br>")
            End If
            Return str_MovScarico
        End Function


        Private Shared Sub Aggiungi_Righe_Dettagli_Destinazioni_alle_Tabelle_DDT(ByVal Agenda As Operazione_Agenda, ByRef DT_Dettagli As DataTable, ByRef DT_Destinazioni As DataTable, ByVal sa_cod_magazzino_predefinito_azienda As Integer, ByVal Fabbricato_Cod_magazzino_predefinito_azienda As Integer, ByVal Elem_Cod As Integer, ByVal Pro_Cod As Integer, ByVal Mat_Cod As Integer, ByVal Udm_Cod As Integer, ByVal lotto As String, ByVal Qta As Decimal, ByVal Data As Date, ByVal Mov_Det_Des As String)
            Dim objXML As New AgronicaCoreXML.XML_Contab
            objXML.DtForXml_InserisciRiga_MovimentiDettagli(
                                        DT_Dettagli,
                                        enum_TipoOperazioneDB.Scrittura,
                                        Agenda.Piva,
                                        sa_cod_magazzino_predefinito_azienda,
                                         0, 0, 0,
                                        Mov_Det_Des,
                                        Elem_Cod,
                                        Pro_Cod,
                                        Mat_Cod,
                                        0,
                                        0,
                                        lotto,
                                        0,
                                        Udm_Cod,
                                        ,
                                        Qta,
                                        0,
                                        0, 0, 0, 0,
                                        0, 0, 0, 0,
                                        0, 0, 0,
                                        0,
                                        ,
                                        ,
                                        , , ,
                                        Data,
                                        AGRODATAFINE,
                                        0, 0, 0, 0, 0, 0, 0,
                                        Agenda.BaseCode,
                                        Agenda.TopCode,
                                        Fabbricato_Cod_magazzino_predefinito_azienda,
                                        0,
                                        CAU_CARICO)

            '-------------------------------------------
            '---- Creazione DT delle destinazioni ------
            'aggiungo righe al dt destinazioni
            objXML.DtForXml_InserisciRiga_MovDestinazioni( _
                                        DT_Destinazioni, _
                                        enum_TipoOperazioneDB.Scrittura, _
                                        Agenda.Piva, _
                                        sa_cod_magazzino_predefinito_azienda, _
                                        0, 0, 0, 0, _
                                        Fabbricato_Cod_magazzino_predefinito_azienda, _
                                        MAGAZZINO, _
                                        Qta, _
                                        0, _
                                        0, 0, _
                                        Data, _
                                        AGRODATAFINE, _
                                        Agenda.BaseCode, _
                                        Agenda.TopCode)
        End Sub


        Private Function Crea_DDTricevutoFittizio(ByVal Agenda As Operazione_Agenda, ByVal DT_Dettagli As DataTable, ByVal DT_Destinazioni As DataTable) As String

            Dim str_DDTricevutoFittizio As String
            Dim f_prefisso_numero_ddt As String = ""
            Dim f_numero_ddt As Integer = 0
            Dim f_suffisso_numero_ddt As String = ""
            Dim f_data_ddt As Date = Operazione.DataOperazione
            Dim f_numero_colli As Integer = 1
            Dim f_causale_trasporto As String = "CONTO ACQUISTO"
            Dim f_aspetto_beni As String = "VISIBILE"
            Dim Natura_Beni As String = ""
            Dim f_peso As Integer = 0
            Dim f_data_consegna As Date = Operazione.DataOperazione
            Dim f_ora_consegna = AGRODATAINIZIO
            Dim Num_Protocollo As Integer = 0
            Dim f_note_ddt As String = "DDT fittizio creato in automatico per scarico da magazzino impresa padre"
            Dim Jolly_Int As Integer = MagazzinoMovimentato
            Dim Contabilizzato As Integer = NONCONTABILE
            Dim Pendente As Integer = enum_Pendenza.MovPendente
            Dim Cod_IndirizzoRisum As Integer = 0
            Dim DDT_Blocco_Flag As Integer = 1
            Dim DDT_Blocco_Data As Date = Operazione.DataOperazione
            Dim DDT_Blocco_Username As String = objParametri.UsernameOperazione
            Dim Rag_Soc_Fornitore As String
            Dim Des_Lib_DDT As String
            Dim Mov_Desc_Magazzino As String

            'PREREQUISITO
            'verifica se l'impresa padre è già salvato come contatto fornitore
            'in caso contrario aggiungere il rapporto contabile fornitore
            'chiamare funzione in anagrafe biz che restituisce cod_risum e cod_indirizzorisum
            'magazzino non è quello dell'impresa, ma di un'impresa padre
            Dim Piva_Magazzino As String = Operazione.Magazzino.Piva
            Dim Sa_Cod_Magazzino As String = Operazione.Magazzino.Sa_Cod
            Dim Fabbricato_Cod_Magazzino As String = Operazione.Magazzino.Fabbricato_Cod
            Dim contatti As New AgronicaCoreAnagrafeBIZ.Contatti_MultiHost_R
            Dim OUT_Piva_Contatto As String
            ' Dim OUT_Sa_Cod As String
            Dim OUT_Cod_Risum As String
            Dim OUT_Cod_Indirizzo As String

            contatti.RicavaScrive_Contatto_Fornitore("", _
                                                     Piva_Magazzino, _
                                                     OUT_Piva_Contatto, _
                                                     OUT_Cod_Risum, _
                                                     OUT_Cod_Indirizzo, _
                                                     Rag_Soc_Fornitore, _
                                                     objParametri)

            If Rag_Soc_Fornitore = "" Then
                Dim objImp As New AgronicaCoreAnagrafeDAL.Imprese_Read
                Rag_Soc_Fornitore = objImp.RagSoc_from_Piva(Piva_Magazzino, objParametri)
            End If

            Des_Lib_DDT = "Documento di Trasporto Ricevuto (n.0 Rif:" & Rag_Soc_Fornitore & ") "
            Mov_Desc_Magazzino = "Carico di Articoli relativi al ricevimento DDT n.0 del " & f_data_ddt & " Rif: " & Rag_Soc_Fornitore & " "

            Dim ErroreMessaggio As String
            str_DDTricevutoFittizio = New AgronicaCoreXML.XML_Contab().MacroXML_Contabilita_DDT( _
                                            ErroreMessaggio, _
                                            Nothing, _
                                            objParametri.PivaSuperUser, _
                                            Agenda.Piva, _
                                            LAVCOD_BOLLA_RICEVUTA, _
                                            Des_Lib_DDT, _
                                            f_prefisso_numero_ddt, _
                                            f_numero_ddt, _
                                            f_suffisso_numero_ddt, _
                                            f_data_ddt, _
                                            f_numero_colli, _
                                            f_causale_trasporto, _
                                            f_aspetto_beni, _
                                            Natura_Beni, _
                                            f_peso, _
                                            f_data_consegna, _
                                            f_ora_consegna, _
                                            0, _
                                            f_note_ddt, _
                                            OUT_Cod_Risum, _
                                            Cod_IndirizzoRisum, _
                                            CAU_CARICO, _
                                            Mov_Desc_Magazzino, _
                                            Agenda.BaseCode, _
                                            Agenda.TopCode, _
                                            Nothing, _
                                            Nothing, _
                                            DT_Dettagli, _
                                            DT_Destinazioni, _
                                            DDT_Blocco_Flag, _
                                            DDT_Blocco_Data, _
                                            DDT_Blocco_Username, _
                                            0, _
                                            0, 0, _
                                            0, _
                                            0, _
                                            0, _
                                            0, _
                                            0, _
                                            0, _
                                            0, _
                                            "", _
                                            "", _
                                            0, _
                                            AGRODATAINIZIO, _
                                            0, _
                                            0, _
                                            Agenda.Data, _
                                            0, _
                                            0, _
                                            0, _
                                            0)

            If ErroreMessaggio <> "" Then
                Throw New Exception("<b>Non è riuscita la creazione dello scarico dal DDT dall'azienda padre </b> " & ErroreMessaggio & "<br>")
            End If

            Return str_DDTricevutoFittizio

        End Function



#End Region




    End Class



End Namespace

